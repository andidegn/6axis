using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Andidegn.Util;
using AsyncModel;
using ECTunes.Util;
using ECTunes.view;

namespace ECTunes {
	public partial class Form1 : Form {

		private int jitterCount = 5;

		private int calibrationIterations = 3;
		private int calibrationCount;
        private bool calibrate;
        private bool calFwdAcc;
        private bool calFwdGy;

		private bool threadKill;

		private SerialPort serialPort;
		private String readFromCom;
		private String rawData;
		private int readings;
		private int readingIndex;
		private double accXOffset, accYOffset, accZOffset, 
                    gyXOffset, gyYOffset, gyZOffset;
		private bool running;
		private bool comOpen = false;
		private Input input;

		private double velocity;
        private double global_angle;
        bool fwd;

        private int gyroThreshold, accelerationThreshold;

        private KalmanAngle k_acc_x;
        private KalmanAngle k_acc_y;
        private KalmanAngle k_acc_z;

        private KalmanAngle k_gy_x;
        private KalmanAngle k_gy_y;
        private KalmanAngle k_gy_z;

		private DateTime LastDT;

		public static String port;

        Vector3D v_acc_zero;
        Vector3D v_acc_direction;
        Vector3D v_gy_zero;
        Vector3D v_gy_last;
        Vector3D v_gy_direction;

		private enum Input {
			COM,
			CSV
		}

		public Form1() {
			InitializeComponent();
			InitOtherStuff();
			ChartInit();
			CalibrationInit();

            calFwdAcc = true;
            calFwdGy = true;
			running = false;
			btnRefreshComPort_Click(this, null);
		}

		private void InitOtherStuff() {
            k_acc_x = new KalmanAngle();
            k_acc_y = new KalmanAngle();
            k_acc_z = new KalmanAngle();
            k_gy_x = new KalmanAngle();
            k_gy_y = new KalmanAngle();
            k_gy_z = new KalmanAngle();
            gyroThreshold = 10000;
            accelerationThreshold = 10000;
			LastDT = DateTime.Now;
			serialPort = new SerialPort();
			comOpen = Util.SerialPortConnector.SerialSetup(serialPort, port);
			serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);
		}

		private void ChartInit() {
			readings = 0;
			readingIndex = 0;
			chart1.Series.Clear();
			chart1.ChartAreas[0].AxisX.Minimum = 0;
			ChartControl.ChartSetup(chart1, "acc_x", 2, Color.DarkRed, SeriesChartType.Line, ChartValueType.Int32);
			ChartControl.ChartSetup(chart1, "acc_y", 2, Color.DarkGreen, SeriesChartType.Line, ChartValueType.Int32);
			ChartControl.ChartSetup(chart1, "acc_z", 2, Color.DarkBlue, SeriesChartType.Line, ChartValueType.Int32);
			ChartControl.ChartSetup(chart1, "gy_x", 2, Color.Red, SeriesChartType.Line, ChartValueType.Int32);
			ChartControl.ChartSetup(chart1, "gy_y", 2, Color.Green, SeriesChartType.Line, ChartValueType.Int32);
			ChartControl.ChartSetup(chart1, "gy_z", 2, Color.Blue, SeriesChartType.Line, ChartValueType.Int32);
			ChartControl.ChartSetup(chart1, "Racc", 2, Color.DarkOrange, SeriesChartType.Line, ChartValueType.Int32);
			ChartControl.ChartSetup(chart1, "Rgy", 2, Color.Yellow, SeriesChartType.Line, ChartValueType.Int32);
            ChartControl.ChartSetup(chart1, "Rtot", 2, Color.Black, SeriesChartType.Line, ChartValueType.Int32);
            ChartControl.ChartSetup(chart1, "Velocity", 2, Color.Purple, SeriesChartType.Line, ChartValueType.Int32);
		}

		private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e) {
			if (input == Input.COM && comOpen && running) {
				readFromCom = serialPort.ReadLine();
				readings++;
				printRawData(readFromCom);
			}
		}

        private void SetupFilter() {
            try { k_acc_x.setQangle(Convert.ToDouble(tbxAccQAngle.Text, CultureInfo.InvariantCulture)); }
            catch (Exception) { }
            try { k_acc_x.setQbias(Convert.ToDouble(tbxAccQBias.Text, CultureInfo.InvariantCulture)); }
            catch (Exception) { }
            try { k_acc_x.setRmeasure(Convert.ToDouble(tbxAccRMeasure.Text, CultureInfo.InvariantCulture)); }
            catch (Exception) { }

            try { k_acc_y.setQangle(Convert.ToDouble(tbxAccQAngle.Text, CultureInfo.InvariantCulture)); }
            catch (Exception) { }
            try { k_acc_y.setQbias(Convert.ToDouble(tbxAccQBias.Text, CultureInfo.InvariantCulture)); }
            catch (Exception) { }
            try { k_acc_y.setRmeasure(Convert.ToDouble(tbxAccRMeasure.Text, CultureInfo.InvariantCulture)); }
            catch (Exception) { }

            try { k_acc_z.setQangle(Convert.ToDouble(tbxAccQAngle.Text, CultureInfo.InvariantCulture)); }
            catch (Exception) { }
            try { k_acc_z.setQbias(Convert.ToDouble(tbxAccQBias.Text, CultureInfo.InvariantCulture)); }
            catch (Exception) { }
            try { k_acc_z.setRmeasure(Convert.ToDouble(tbxAccRMeasure.Text, CultureInfo.InvariantCulture)); }
            catch (Exception) { }


            try { k_gy_x.setQangle(Convert.ToDouble(tbxAccQAngle.Text, CultureInfo.InvariantCulture)); }
            catch (Exception) { }
            try { k_gy_x.setQbias(Convert.ToDouble(tbxAccQBias.Text, CultureInfo.InvariantCulture)); }
            catch (Exception) { }
            try { k_gy_x.setRmeasure(Convert.ToDouble(tbxAccRMeasure.Text, CultureInfo.InvariantCulture)); }
            catch (Exception) { }

            try { k_gy_y.setQangle(Convert.ToDouble(tbxAccQAngle.Text, CultureInfo.InvariantCulture)); }
            catch (Exception) { }
            try { k_gy_y.setQbias(Convert.ToDouble(tbxAccQBias.Text, CultureInfo.InvariantCulture)); }
            catch (Exception) { }
            try { k_gy_y.setRmeasure(Convert.ToDouble(tbxAccRMeasure.Text, CultureInfo.InvariantCulture)); }
            catch (Exception) { }

            try { k_gy_z.setQangle(Convert.ToDouble(tbxAccQAngle.Text, CultureInfo.InvariantCulture)); }
            catch (Exception) { }
            try { k_gy_z.setQbias(Convert.ToDouble(tbxAccQBias.Text, CultureInfo.InvariantCulture)); }
            catch (Exception) { }
            try { k_gy_z.setRmeasure(Convert.ToDouble(tbxAccRMeasure.Text, CultureInfo.InvariantCulture)); }
            catch (Exception) { }
        }

        private void CalibrateZero(Vector3D acc, Vector3D gy) {
            if (calibrationCount == calibrationIterations) {
                accXOffset = accXOffset / (calibrationIterations);
                accYOffset = accYOffset / (calibrationIterations);
                accZOffset = accZOffset / (calibrationIterations);
                v_acc_zero = new Vector3D(accXOffset, accYOffset, accZOffset);

                gyXOffset = gyXOffset / (calibrationIterations);
                gyYOffset = gyYOffset / (calibrationIterations);
                gyZOffset = gyZOffset / (calibrationIterations);
                calibrate = false;
                v_gy_zero = new Vector3D(gyXOffset, gyYOffset, gyZOffset);
                v_gy_last = v_gy_zero;
                MessageBox.Show("zero calibrated");
            }
            else {
                accXOffset += acc.x;
                accYOffset += acc.y;
                accZOffset += acc.z;

                gyXOffset += gy.x;
                gyYOffset += gy.y;
                gyZOffset += gy.z;

                calibrationCount++;
                return;
            }
        }

        private void CalibrateFwd(Vector3D acc, Vector3D gy) {
            double gy_len = gy.Length();
            if (!calFwdAcc && gy_len > 10000) {
                v_gy_direction = gy;
                calFwdGy = false;
                global_angle = 0;
            }
            else
                if (calFwdAcc && acc.Length() > 5000) {
                    v_acc_direction = acc;
                    calFwdAcc = false;
                }
            return;
        }

        private void FilterAcc(ref Vector3D acc, ref Vector3D gy, double dt) {
            acc.x = k_acc_x.getAngle(acc.x, 0, dt);
            acc.y = k_acc_y.getAngle(acc.y, 0, dt);
            acc.z = k_acc_z.getAngle(acc.z, 0, dt);
        }

        private void FilterGyro(ref Vector3D gy, double dt) {
            gy.x = k_gy_x.getAngle(gy.x, 0, dt);
            gy.y = k_gy_y.getAngle(gy.y, 0, dt);
            gy.z = k_gy_z.getAngle(gy.z, 0, dt);
        }

		private void printRawData(String data) {
			if (input == Input.COM && readings < jitterCount) {return;}

			double DeltaSecs = (double)DateTime.Now.Subtract(LastDT).Milliseconds / 1000.0;
			LastDT = DateTime.Now;

			data.Replace(@"\r", "");
			string[] dataArr = data.Split(',');

			// Data only - Start
			StringBuilder sb = new StringBuilder();
			int i = 0;
			foreach (String s in dataArr) {
				sb.Append(string.Format("v{0}:{1}- ", ++i, s));
			}
			rawData = sb.ToString();
			// Data only - Stop

			if (dataArr.Length > 8) {

				int loops = Int16.Parse(dataArr[8]);
				double acc_x_raw = Int32.Parse(dataArr[1]);
				double acc_y_raw = Int32.Parse(dataArr[2]);
				double acc_z_raw = Int32.Parse(dataArr[3]);
                Vector3D v_reading_acc_raw = new Vector3D(acc_x_raw, acc_y_raw, acc_z_raw);

				double gy_x_raw = Int32.Parse(dataArr[4]);
				double gy_y_raw = Int32.Parse(dataArr[5]);
				double gy_z_raw = Int32.Parse(dataArr[6]);
                Vector3D v_reading_gy_raw = new Vector3D(gy_x_raw, gy_y_raw, gy_z_raw);

				// Calibration - Start
				if (chk_tsmTools_CalibrateSignal.Checked && calibrate) {
                    CalibrateZero(v_reading_acc_raw, v_reading_gy_raw);
                    return;
				}

				// Accelerometer
                Vector3D v_acc = v_reading_acc_raw - v_acc_zero;

				// Gyro
                Vector3D v_gy = v_reading_gy_raw - v_gy_zero;
                FilterAcc(ref v_acc, ref v_gy, DeltaSecs);
                FilterGyro(ref v_gy, DeltaSecs);

                if (calFwdAcc || calFwdGy) {
                    CalibrateFwd(v_acc, v_gy);
                    return;
                }
                // Calibration - Stop

				// Kalman filter
                SetupFilter();

                Vector3D v_gy_projection = Vector.Projection(v_gy, v_gy_direction);
                Vector3D v_acc_projection = Vector.Projection(v_acc, v_acc_direction);
                double gyroAngle = Vector.Angle(v_gy, v_gy_direction);
                double accLength = Vector.ProjectionLenght(v_acc, v_acc_direction);

                double angle = Vector.Angle(v_acc, v_acc_direction);
                double angle45max = CalcAngle(angle);

                if (accLength < 1000) {
                    global_angle /= 2;
                    velocity /= 2;
                }



                //accelerationValue = accelerationValue / ((Math.Abs(CalcAngle(angle)) * 100 / 45) + 1);
                double gyroValue = v_gy_projection.Length();

                //double gyroValue_k = k_projected_gy.getAngle(gyroValue, 0, DeltaSecs);

                //double accelerationValue_k = 0;
                //if (Vector.Angle(reading, direction) < 10)
                //accelerationValue_k = k_projected_acc.getAngle(accelerationValue, 0, DeltaSecs);

                //global_angle = k_projected_global_angle.getAngle(reading.Length(), gyroReading.Length(), DeltaSecs);

                try { gyroThreshold = Convert.ToInt32(tbxGyroThreshold.Text); } catch (Exception) { }
                try { accelerationThreshold = Convert.ToInt32(tbxAccelerationThreshold.Text); }
                catch (Exception) { }

                if (gyroValue > 10000)
                    if (gyroAngle < 90)
                        global_angle -= (gyroValue * DeltaSecs) * 2;
                    else
                        global_angle += (gyroValue * DeltaSecs) * 2;

                double acceleration = accLength - Math.Abs(global_angle);

                if (acceleration > accelerationThreshold) {
                    if (velocity < 100) {
                        if (angle < 90)
                            fwd = true;
                        else
                            fwd = false;
                    }
                    if (fwd)
                        if (angle < 90)
                            velocity += acceleration;
                        else
                            velocity -= acceleration;
                    else
                        if (angle >= 90)
                            velocity += acceleration;
                        else
                            velocity -= acceleration;
                }

                if (velocity < 0)
                    velocity = 0;
                else if (velocity > 400000)
                    velocity = 400000;

                v_gy_last = v_gy;

				if (chkAccX.Checked) AddNewPoint("acc_x", readingIndex, v_acc.x);
				if (chkAccY.Checked) AddNewPoint("acc_y", readingIndex, v_acc.y);
				if (chkAccZ.Checked) AddNewPoint("acc_z", readingIndex, v_acc.z);

				if (chkGyX.Checked) AddNewPoint("gy_x", readingIndex, v_gy.x);
				if (chkGyY.Checked) AddNewPoint("gy_y", readingIndex, v_gy.y);
				if (chkGyZ.Checked) AddNewPoint("gy_z", readingIndex, v_gy.z);

				if (chkForceR.Checked) AddNewPoint("Racc", readingIndex, acceleration);
                if (chkForceRg.Checked) AddNewPoint("Rgy", readingIndex, global_angle);
                if (chkForceRt.Checked) AddNewPoint("Rtot", readingIndex, accLength);
                if (chkForceRt.Checked) AddNewPoint("Velocity", readingIndex, velocity);
				try { this.Invoke(new EventHandler(PrintRawDataEH)); } catch { }
			}
		}

        private double CalcAngle(double value) {
            if (value > 90)
                value = value - 90;
            return 45 - Math.Abs(45 - value);
        }

		public void PrintCSV(StreamReader sr) {
			String line;
			while ((line = sr.ReadLine()) != null) {
				if (threadKill)
					break;
				while (!running)
					Thread.Sleep(1);
				printRawData(line);
				Thread.Sleep(100);
			}
		}


        delegate void invokeAddNewPoint(String seriesName, double x, double y);

		private void AddNewPoint(String seriesName, double x, double y) {
			if (this.InvokeRequired) {
				this.BeginInvoke(new invokeAddNewPoint(AddNewPoint), seriesName, x, y);
				return;
			}

			switch (seriesName) {
				case "acc_x": lblAccX.Text = y.ToString(); break;
				case "acc_y": lblAccY.Text = y.ToString(); break;
				case "acc_z": lblAccZ.Text = y.ToString(); break;
				case "gy_x": lblGyX.Text = y.ToString(); break;
				case "gy_y": lblGyY.Text = y.ToString(); break;
				case "gy_z": lblGyZ.Text = y.ToString(); break;
				case "Racc": lblForceR.Text = y.ToString(); break;
				case "Rgy": lblForceRg.Text = y.ToString(); break;
				case "Rtot": lblForceRt.Text = y.ToString(); break;
                //case "Velocity": Console.Beep((int)(y * 32000 / 100000) + 37, 1); 
                //    SystemSounds.Asterisk.Play(); break;
				default: break;
			}
			chart1.Series[seriesName].Points.AddXY((double)x, (double)y);
			ZoomTrigger();
		}

		private void CalibrationInit() {
			calibrate = true;
			calibrationCount = 0;

			accXOffset = 0;
			accYOffset = 0;
			accZOffset = 0;
			gyXOffset = 0;
			gyYOffset = 0;
			gyZOffset = 0;
		}

		private void ZoomTrigger() {
			if (!chkFullScale.Checked) {
				chart1.ChartAreas[0].CursorX.AutoScroll = true;
				chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
				chart1.ChartAreas[0].AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;
				chart1.ChartAreas[0].AxisX.ScaleView.Zoom(readingIndex - tkbScale.Value, readingIndex);
			} else {
				chart1.ChartAreas[0].CursorX.AutoScroll = false;
				chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = false;
				chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset();
			}
		}


		// ---------------------------------------------
		// ------------- EventHandlers -----------------
		// ---------------------------------------------

		private void PrintRawDataEH(Object sender, EventArgs e) {
			if (!threadKill)
				tbxRawData.AppendText(string.Format("{0} - {1}\n", ++readingIndex, rawData));
		}

		private void btnDataChart_Click(object sender, EventArgs e) {
			if (pnlChart.Visible) {
				btnDataChart.Text = "Chart";
				pnlChart.Visible = false;
			} else {
				btnDataChart.Text = "Data";
				pnlChart.Visible = true;
			}
		}

		private void btnExit_Click(object sender, EventArgs e) {
			Application.Exit();
		}

		private void btnReset_Click(object sender, EventArgs e) {
			//running = false;
			ChartInit();
			CalibrationInit();
			this.Invoke(new EventHandler(ResetEH));
			//if (serialPort.IsOpen)
			//    try {
			//        serialPort.Close();
			//    } catch (Exception) {
			//        MessageBox.Show("Connection error!");
			//    }
			//btnStartPause.Text = "Start";
		}

		private void ResetEH(Object sender, EventArgs e) {
			tbxRawData.Clear();
		}

		private void btnStartPause_Click(object sender, EventArgs e) {
			//if (input != Input.COM)
			//    input = Input.COM;
			if (btnStartPause.Text == "Start") {
				btnStartPause.Text = "Pause";
				running = true;
				port = cbbComPort.SelectedItem.ToString();
				if (input == Input.COM && !serialPort.IsOpen) {
					InitOtherStuff();
				}
			} else {
				btnStartPause.Text = "Start";
				running = false;
			}
		}

		private void tkbScale_Scroll(object sender, EventArgs e) {
			String scaleValue = tkbScale.Value.ToString();
			this.toolTip1.SetToolTip(tkbScale, scaleValue);
			this.lblScaleValue.Text = scaleValue;
			if (chkFullScale.Checked)
				chkFullScale.Checked = false;
			ZoomTrigger();
		}

		private void chkFullScale_CheckedChanged(object sender, EventArgs e) {
			ZoomTrigger();
		}

		private void btnSelectAll_Click(object sender, EventArgs e) {
			chkAccX.Checked = true;
			chkAccY.Checked = true;
			chkAccZ.Checked = true;

			chkGyX.Checked = true;
			chkGyY.Checked = true;
			chkGyZ.Checked = true;

			chkForceR.Checked = true;
			chkForceRg.Checked = true;
		}

		private void btnCalibrate_Click(object sender, EventArgs e) {
			CalibrationInit();
		}

		private void tsmFile_open_Click(object sender, EventArgs e) {
			input = Input.CSV;

			threadKill = true;
			
			btnReset_Click(sender, e);

			OpenFileDialog openFileDialog1 = new OpenFileDialog();

			openFileDialog1.Filter = "Text Files (.txt)|*.txt|All Files (*.*)|*.*";
			openFileDialog1.FilterIndex = 1;

			openFileDialog1.Multiselect = false;


			if (openFileDialog1.ShowDialog() == DialogResult.OK) {
				StreamReader sr = null;
				try {
					System.IO.Stream fileStream = openFileDialog1.OpenFile();

					// Just a good practice -- change the cursor to a 
					// wait cursor while the nodes populate
					//       this.Cursor = Cursors.WaitCursor;
					sr = new System.IO.StreamReader(fileStream);
				}
				catch (Exception ex) {
					MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
				}

				if (sr != null) {
					if (btnStartPause.Text == "Start") btnStartPause_Click(sender, e);
					Thread t = new Thread(() => PrintCSV(sr));
					threadKill = false;
					t.Start();
				}
			}
		}

		private void btnRefreshComPort_Click(object sender, EventArgs e) {
			cbbComPort.Items.Clear();
			foreach (string port in Util.SerialPortConnector.getAvalComPort()) {
				cbbComPort.Items.Add(port);
			}
			if (cbbComPort.Items.Count > 0)
				cbbComPort.SelectedIndex = 0;
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
			Thread.Sleep(500);
		}
	}
}
