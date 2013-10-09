using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
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

		private int calibrationIterations = 30;
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
                    gyXOffset, gyYOffset, gyZOffset,
                    accXDir, accYDir, accZDir;
		private bool running;
		private bool comOpen = false;
		private Input input;

		private int lastReading;
        //private int velocity;
		private double global_angle;

		private KalmanAngle k_acc;
        private KalmanAngle k_gy;
        private KalmanAngle k_projected_acc;
        private KalmanAngle k_projected_gy;
        private KalmanAngle k_projected_angle;
        private KalmanAngle k_projected_global_angle;

        private KalmanAngle kx;
        private KalmanAngle ky;
        private KalmanAngle kz;

        private KalmanAngle kxg;
        private KalmanAngle kyg;
        private KalmanAngle kzg;

		private DateTime LastDT;

		public static String port;

        Vector3D zero;
        Vector3D direction;
        Vector3D velocity;
        Vector3D gyro_previus;
        Vector3D gyro_direction;

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
            k_acc = new KalmanAngle();
            k_gy = new KalmanAngle();
            k_projected_acc = new KalmanAngle();
            k_projected_gy = new KalmanAngle();
            k_projected_angle = new KalmanAngle();
            k_projected_global_angle = new KalmanAngle();
            kx = new KalmanAngle();
            ky = new KalmanAngle();
            kz = new KalmanAngle();
            kxg = new KalmanAngle();
            kyg = new KalmanAngle();
            kzg = new KalmanAngle();
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
		}

		private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e) {
			if (input == Input.COM && comOpen && running) {
				readFromCom = serialPort.ReadLine();
				readings++;
				printRawData(readFromCom);
			}
		}

        private void SetupFilter() {
            try { k_projected_acc.setQangle(Convert.ToDouble(tbxAccQAngle.Text, CultureInfo.InvariantCulture)); }
            catch (Exception) { }
            try { k_projected_acc.setQbias(Convert.ToDouble(tbxAccQBias.Text, CultureInfo.InvariantCulture)); }
            catch (Exception) { }
            try { k_projected_acc.setRmeasure(Convert.ToDouble(tbxAccRMeasure.Text, CultureInfo.InvariantCulture)); }
            catch (Exception) { }

            try { k_projected_gy.setQangle(Convert.ToDouble(tbxGyQAngle.Text, CultureInfo.InvariantCulture)); }
            catch (Exception) { }
            try { k_projected_gy.setQbias(Convert.ToDouble(tbxGyQBias.Text, CultureInfo.InvariantCulture)); }
            catch (Exception) { }
            try { k_projected_gy.setRmeasure(Convert.ToDouble(tbxGyRMeasure.Text, CultureInfo.InvariantCulture)); }
            catch (Exception) { }

            //k_projected_angle.setQangle(0.1);
        }

		private void printRawData(String data) {
			if (input == Input.COM && readings < jitterCount) {return;}

			double DeltaSecs = (double)DateTime.Now.Subtract(LastDT).Milliseconds / 1000.0;
			LastDT = DateTime.Now;





			rawData = data;

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

				double gy_x_raw = Int32.Parse(dataArr[4]);
				double gy_y_raw = Int32.Parse(dataArr[5]);
				double gy_z_raw = Int32.Parse(dataArr[6]);

				// Calibration - Start
				if (chk_tsmTools_CalibrateSignal.Checked && calibrate && calibrationCount <= calibrationIterations) {
					if (calibrationCount == calibrationIterations) {
						accXOffset = accXOffset / (calibrationIterations);
						accYOffset = accYOffset / (calibrationIterations);
						accZOffset = accZOffset / (calibrationIterations);
                        zero = new Vector3D(accXOffset, accYOffset, accZOffset);

						gyXOffset = gyXOffset / (calibrationIterations);
						gyYOffset = gyYOffset / (calibrationIterations);
						gyZOffset = gyZOffset / (calibrationIterations);
                        calibrate = false;
                        gyro_previus = new Vector3D(gyXOffset, gyYOffset, gyZOffset);
					}
					else {
						accXOffset += acc_x_raw;
						accYOffset += acc_y_raw;
						accZOffset += acc_z_raw;

						gyXOffset += gy_x_raw;
						gyYOffset += gy_y_raw;
						gyZOffset += gy_z_raw;

						calibrationCount++;
						return;
					}
				}
				else if (chk_tsmTools_CalibrateSignal.Checked && calibrate) {
					accXOffset = acc_x_raw;
					accYOffset = acc_y_raw;
					accZOffset = acc_z_raw;

					gyXOffset = gy_x_raw;
					gyYOffset = gy_y_raw;
					gyZOffset = gy_z_raw;
					calibrate = false;
                }
                if (calFwdAcc || calFwdGy) {
                    accXDir = kx.getAngle(acc_x_raw - accXOffset, 0, DeltaSecs);
                    accYDir = ky.getAngle(acc_y_raw - accYOffset, 0, DeltaSecs);
                    accZDir = kz.getAngle(acc_z_raw - accZOffset, 0, DeltaSecs);
                    double gyXDir = kxg.getAngle(gy_x_raw - gyXOffset, 0, DeltaSecs);
                    double gyYDir = kyg.getAngle(gy_y_raw - gyYOffset, 0, DeltaSecs);
                    double gyZDir = kzg.getAngle(gy_z_raw - gyZOffset, 0, DeltaSecs);
                    Vector3D v1 = new Vector3D(accXDir, accYDir, accZDir);
                    Vector3D v2 = new Vector3D(gyXDir, gyYDir, gyZDir);
                    double v2_len = v2.Length();
                    if (chkForceRg.Checked) AddNewPoint("Rgy", readingIndex++, v2_len);
                    if (!calFwdAcc && v2_len > 10000) {
                        gyro_direction = v2;
                        calFwdGy = false;
                        MessageBox.Show("gyro calibrated");
                    } else
                    if (calFwdAcc && v1.Length() > 5000) {
                        direction = v1;
                        calFwdAcc = false;
                        MessageBox.Show("accelerometer calibrated");
                    }
                    return;
                }
                //direction = new Vector3D(0, -300, 0);
                // Calibration - Stop

				// Accelerometer
				double acc_x = (acc_x_raw - accXOffset);
				double acc_y = (acc_y_raw - accYOffset);
				double acc_z = (acc_z_raw - accZOffset);

				// Gyro
				double gy_x = (gy_x_raw - gyXOffset);
				double gy_y = (gy_y_raw - gyYOffset);
				double gy_z = (gy_z_raw - gyZOffset);

				 // Kalman filter
                SetupFilter();

                //acc_x = k_acc.getAngle(acc_x, gy_x, DeltaSecs);
                //acc_y = k_acc.getAngle(acc_y, gy_y, DeltaSecs);
                //acc_z = k_acc.getAngle(acc_z, gy_z, DeltaSecs);

                Vector3D reading = new Vector3D(acc_x, acc_y, acc_z);
                Vector3D gyroReading = new Vector3D(gy_x, gy_y, gy_z);
                Vector3D gy_projection = Vector.Projection(gyroReading, gyro_direction);
                double gyroAngle = Vector.Angle(gyroReading, gyro_direction);

                Vector3D acceleration = Vector.Projection(reading, direction);
                //if ()

				double accTot = (acc_x_raw + acc_y_raw + acc_z_raw) / 3000;
				double gy_direction = (gy_x_raw + gy_y_raw + gy_z_raw) / 3000;


                Vector3D v_acc = Vector.Projection(reading, direction);
                double accelerationValue = Vector.ProjectionLenght(reading, direction);

                double angle = k_projected_angle.getAngle(Vector.Angle(reading, direction), 0, DeltaSecs);
                double angle45max = CalcAngle(angle);



                //accelerationValue = accelerationValue / ((Math.Abs(CalcAngle(angle)) * 100 / 45) + 1);
                double gyroValue = gy_projection.Length();

                double gyroValue_k = k_projected_gy.getAngle(gyroValue, 0, DeltaSecs);

                double accelerationValue_k = 0;
                //if (Vector.Angle(reading, direction) < 10)
                accelerationValue_k = k_projected_acc.getAngle(accelerationValue, 0, DeltaSecs);

                //global_angle = k_projected_global_angle.getAngle(reading.Length(), gyroReading.Length(), DeltaSecs);
                if (gyroValue > 10000)
                    if (gyroAngle < 90)
                        global_angle -= (int)(gyroValue * DeltaSecs) * 3;
                    else
                        global_angle += (int)(gyroValue * DeltaSecs) * 3;

                gyro_previus = gyroReading;

                //int reading = R - (angle / 3);
                //int difference = Math.Abs(lastReading - reading);
                //if (difference > 20 && difference < 200) {
                //    if (reading < lastReading) {
                //        velocity -= reading;
                //    }
                //    else {
                //        velocity += reading;
                //    }
                //    lastReading = reading;
                //}

                //int vector_da = Math.Abs((int)(accXDir * acc_x + accYDir * acc_y + accZDir * acc_z)) / (int)Math.Sqrt(Math.Pow(accXDir, 2) + Math.Pow(accYDir, 2) + Math.Pow(accZDir, 2));

                //int da_k = Convert.ToInt32(k_acc.getAngle(vector_da, Rg, DeltaSecs));

				//Q_angle = 0.001;
				//Q_bias = 0.003;
				//R_measure = 0.03;
				if (chkAccX.Checked) AddNewPoint("acc_x", readingIndex, acc_x);
				if (chkAccY.Checked) AddNewPoint("acc_y", readingIndex, acc_y);
				if (chkAccZ.Checked) AddNewPoint("acc_z", readingIndex, acc_z);

				if (chkGyX.Checked) AddNewPoint("gy_x", readingIndex, gy_x);
				if (chkGyY.Checked) AddNewPoint("gy_y", readingIndex, gy_y);
				if (chkGyZ.Checked) AddNewPoint("gy_z", readingIndex, gy_z);

				if (chkForceR.Checked) AddNewPoint("Racc", readingIndex, gyroAngle);
                if (chkForceRg.Checked) AddNewPoint("Rgy", readingIndex, global_angle);
                if (chkForceRt.Checked) AddNewPoint("Rtot", readingIndex, accelerationValue_k);
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
