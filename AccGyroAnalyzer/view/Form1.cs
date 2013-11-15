using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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
using Andidegn.Util.Collections;
using Andidegn.Util.Collections.QueueArray;
using AsyncModel;
using ECTunes.Properties;
using ECTunes.Util;
using ECTunes.view;

namespace ECTunes {
	public partial class Form1 : Form {
		private const double ANGLE_MULTIPLIER = 3;
		private static int G_NOISE_CALIBRATION_THRESHOLD = 0;
		private static int G_GYRO_THRESHOLD = 2000;
		private static int G_ACCELERATION_THRESHOLD = 0;
		//private int g_velocity_calibration_threshold = 0;

		private const int g_num_of_recent_readings = 3;

		//private Vector3D g_v_acc_zero_default = new Vector3D(346600, -7141, -31589);
		//private Vector3D g_v_acc_direction_default = new Vector3D(-552, -14231, -435);
		//private Vector3D g_v_gy_zero_default = new Vector3D(817, 679, -4468);
		//private Vector3D g_v_gy_direction_default = new Vector3D(7764, 2615, -257611);
		private const bool G_RUN_CALIBRATE = true;
		private bool isCalibrationStored = false;

		private const double G_ACC_OFFSET_ACCELERATE = 10.0;
		private const double G_ACC_OFFSET_DECELERATE = 10.0;

		private const int G_SOUND_CUTOFF_VELOCITY = 20000;
		private const int G_SOUND_DEVIDER = 300;

		private int g_jitterCount = 5;
		private double g_accLength;
		private double g_gyro_last;

		private int g_calibrationIterations = 10;
		private int g_calibrationCount;
		private bool g_calibrate;
		private bool g_calFwdAcc;
		private bool g_calFwdGy;

		private bool g_threadKill;

		private SerialPort g_serialPort;
		//private String g_readFromCom;
		private String g_rawData;
		private int g_readings;
		private int g_readingIndex;
		private double g_accXOffset, g_accYOffset, g_accZOffset,
					g_gyXOffset, g_gyYOffset, g_gyZOffset;
		private bool g_running;
		private bool g_comOpen = false;
		private Input g_input;

		private double g_velocity;
		private double g_angle;
		bool g_fwd;
		bool g_readyToRun;


		private KalmanAngle g_k_acc_x;
		private KalmanAngle g_k_acc_y;
		private KalmanAngle g_k_acc_z;

		private KalmanAngle g_k_gy_x;
		private KalmanAngle g_k_gy_y;
		private KalmanAngle g_k_gy_z;

		private KalmanAngle g_k_angle;
		private KalmanAngle g_k_g_angle;

		private DateTime g_LastDT;

		public static String g_port;

		Vector3D g_v_acc_zero;
		Vector3D g_v_acc_direction;
		Vector3D g_v_gy_zero;
		Vector3D g_v_gy_direction;
		Vector3D g_v_gy_last;

		double g_v_acc_zero_length;

		DateTime startTime;

		IQueue<Vector3D> g_recent;

		//double angle_offset;

		private System.IO.StreamWriter g_file;

		private enum Input {
			COM,
			CSV
		}

		public Form1() {
			InitializeComponent();
			InitOtherStuff();
			InitChart();
			InitChk();
			InitCalibration();

			g_calFwdAcc = true;
			g_calFwdGy = true;
			g_running = false;
			btnRefreshComPort_Click(this, null);
		}

		#region Initialization
		private void InitOtherStuff() {
			g_k_acc_x = new KalmanAngle();
			g_k_acc_y = new KalmanAngle();
			g_k_acc_z = new KalmanAngle();
			g_k_gy_x = new KalmanAngle();
			g_k_gy_y = new KalmanAngle();
			g_k_gy_z = new KalmanAngle();
			g_k_angle = new KalmanAngle();
			g_k_g_angle = new KalmanAngle();
			g_v_acc_zero = new Vector3D();
			g_v_acc_direction = new Vector3D();
			g_v_gy_zero = new Vector3D();
			g_v_gy_direction = new Vector3D();
			g_LastDT = DateTime.Now;
			g_calibrate = true;
			g_readyToRun = false;
			g_serialPort = new SerialPort();
			g_comOpen = Util.SerialPortConnector.SerialSetup(g_serialPort, g_port);
			g_serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);
			g_recent = new QueueArray<Vector3D>(g_num_of_recent_readings);

			this.tbxAccelerationThreshold.Text = Settings.Default["acceleration_threshold"].ToString();
			this.tbxNoiseThreshold.Text = Settings.Default["noise_calibration_threshold"].ToString();

			UpdateThresholds();
		}

		private void InitChk() {
            //chkAccX.Checked = true;
            //chkAccY.Checked = true;
            //chkAccZ.Checked = true;

			//chkGyX.Checked = true;
			//chkGyY.Checked = true;
			//chkGyZ.Checked = true;

            chkRacc.Checked = true;
            chkRgy.Checked = true;
            chkRtot.Checked = true;
            chkVelocity.Checked = true;
		}

		private void InitChart() {
			g_readings = 0;
			g_readingIndex = 0;
			chart1.Series.Clear();
			chart1.ChartAreas[0].AxisX.Minimum = 0;
			ChartControl.ChartSetup(chart1, "acc_x", 2, Color.DarkRed, SeriesChartType.Line, ChartValueType.Int32);
			ChartControl.ChartSetup(chart1, "acc_y", 2, Color.DarkGreen, SeriesChartType.Line, ChartValueType.Int32);
			ChartControl.ChartSetup(chart1, "acc_z", 2, Color.DarkBlue, SeriesChartType.Line, ChartValueType.Int32);
			ChartControl.ChartSetup(chart1, "gy_x", 2, Color.Red, SeriesChartType.Line, ChartValueType.Int32);
			ChartControl.ChartSetup(chart1, "gy_y", 2, Color.Green, SeriesChartType.Line, ChartValueType.Int32);
			ChartControl.ChartSetup(chart1, "gy_z", 2, Color.Blue, SeriesChartType.Line, ChartValueType.Int32);
			ChartControl.ChartSetup(chart1, "Racc", 2, Color.DarkOrange, SeriesChartType.Line, ChartValueType.Int32);
			ChartControl.ChartSetup(chart1, "GAng", 2, Color.Yellow, SeriesChartType.Line, ChartValueType.Int32);
			ChartControl.ChartSetup(chart1, "AccLen", 2, Color.Black, SeriesChartType.Line, ChartValueType.Int32);
			ChartControl.ChartSetup(chart1, "Velocity", 2, Color.Purple, SeriesChartType.Line, ChartValueType.Int32);
		}

		private void InitCalibration() {
			g_calibrate = true;
			g_calFwdAcc = true;
			g_calFwdGy = true;
			isCalibrationStored = false;
			g_calibrationCount = 0;
			g_angle = 0;
			g_velocity = 0;

			g_accXOffset = 0;
			g_accYOffset = 0;
			g_accZOffset = 0;
			g_gyXOffset = 0;
			g_gyYOffset = 0;
			g_gyZOffset = 0;
			SetCalibrateIndicator(0, false);
			SetCalibrateIndicator(1, false);
			SetCalibrateIndicator(2, false);
			g_k_acc_x.Reset();
			g_k_acc_y.Reset();
			g_k_acc_z.Reset();
			g_k_gy_x.Reset();
			g_k_gy_y.Reset();
			g_k_gy_z.Reset();
			g_k_angle.Reset();
		}

		private void SetupFilter() {
			try { g_k_acc_x.setQangle(Convert.ToDouble(tbxAccQAngle.Text, CultureInfo.InvariantCulture)); }
			catch (Exception) { }
			try { g_k_acc_x.setQbias(Convert.ToDouble(tbxAccQBias.Text, CultureInfo.InvariantCulture)); }
			catch (Exception) { }
			try { g_k_acc_x.setRmeasure(Convert.ToDouble(tbxAccRMeasure.Text, CultureInfo.InvariantCulture)); }
			catch (Exception) { }

			try { g_k_acc_y.setQangle(Convert.ToDouble(tbxAccQAngle.Text, CultureInfo.InvariantCulture)); }
			catch (Exception) { }
			try { g_k_acc_y.setQbias(Convert.ToDouble(tbxAccQBias.Text, CultureInfo.InvariantCulture)); }
			catch (Exception) { }
			try { g_k_acc_y.setRmeasure(Convert.ToDouble(tbxAccRMeasure.Text, CultureInfo.InvariantCulture)); }
			catch (Exception) { }

			try { g_k_acc_z.setQangle(Convert.ToDouble(tbxAccQAngle.Text, CultureInfo.InvariantCulture)); }
			catch (Exception) { }
			try { g_k_acc_z.setQbias(Convert.ToDouble(tbxAccQBias.Text, CultureInfo.InvariantCulture)); }
			catch (Exception) { }
			try { g_k_acc_z.setRmeasure(Convert.ToDouble(tbxAccRMeasure.Text, CultureInfo.InvariantCulture)); }
			catch (Exception) { }


			try { g_k_gy_x.setQangle(Convert.ToDouble(tbxGyQAngle.Text, CultureInfo.InvariantCulture)); }
			catch (Exception) { }
			try { g_k_gy_x.setQbias(Convert.ToDouble(tbxGyQBias.Text, CultureInfo.InvariantCulture)); }
			catch (Exception) { }
			try { g_k_gy_x.setRmeasure(Convert.ToDouble(tbxGyRMeasure.Text, CultureInfo.InvariantCulture)); }
			catch (Exception) { }

			try { g_k_gy_y.setQangle(Convert.ToDouble(tbxGyQAngle.Text, CultureInfo.InvariantCulture)); }
			catch (Exception) { }
			try { g_k_gy_y.setQbias(Convert.ToDouble(tbxGyQBias.Text, CultureInfo.InvariantCulture)); }
			catch (Exception) { }
			try { g_k_gy_y.setRmeasure(Convert.ToDouble(tbxGyRMeasure.Text, CultureInfo.InvariantCulture)); }
			catch (Exception) { }

			try { g_k_gy_z.setQangle(Convert.ToDouble(tbxGyQAngle.Text, CultureInfo.InvariantCulture)); }
			catch (Exception) { }
			try { g_k_gy_z.setQbias(Convert.ToDouble(tbxGyQBias.Text, CultureInfo.InvariantCulture)); }
			catch (Exception) { }
			try { g_k_gy_z.setRmeasure(Convert.ToDouble(tbxGyRMeasure.Text, CultureInfo.InvariantCulture)); }
			catch (Exception) { }

			//g_k_angle.setQangle(0.1);
		}
		#endregion

		#region Calibration
		private void Calibrate(Vector3D v_reading_acc_raw, Vector3D v_reading_gy_raw) {
			if (chk_tsmTools_CalibrateSignal.Checked) {
				if (g_calibrate) {
					CalibrateZero(v_reading_acc_raw, v_reading_gy_raw);
					return;
				}
				else if (g_calFwdAcc) {
					CalibrateFwdAcc(v_reading_acc_raw);
					return;
				}
				else if (g_calFwdGy) {
					CalibrateFwdGy(v_reading_gy_raw);
					return;
				}
				else if (!isCalibrationStored) {
					StoreSettings();
					MessageBox.Show(String.Format("acc_zero: {0}\nacc_direction: {1}\ngy_zero: {3}\ngy_direction: {2}", g_v_acc_zero.ToString(), g_v_acc_direction.ToString(), g_v_gy_direction, g_v_gy_zero));
                    isCalibrationStored = true;
                    chk_tsmTools_CalibrateSignal.Checked = false;
				}
			}
			else if (g_calibrate) {
				if (!loaded) {
					LoadSettings();
					loaded = true;
				}
				g_calibrate = false;
				g_calFwdAcc = false;
				g_calFwdGy = false;
                g_readyToRun = true;
                chk_tsmTools_CalibrateSignal.Checked = false;
			}
		}
		bool loaded = false;

		private void CalibrateZero(Vector3D acc, Vector3D gy) {
			if (g_calibrationCount == g_calibrationIterations) {
				g_calibrationCount = 0;
				g_v_acc_zero = g_v_acc_zero / g_calibrationIterations;
				g_v_gy_zero = g_v_gy_zero / g_calibrationIterations;
				g_v_gy_last = g_v_gy_zero;
				g_v_acc_zero_length = g_v_acc_zero.Length();
				g_calibrate = false;
                SetCalibrateIndicator(0, true);
                MessageBox.Show("Zero calibrated, calibrate forward motion next.\nPress 'OK' when ready.");
			}
			else {
				g_v_acc_zero += acc;
				g_v_gy_zero += gy;

				g_calibrationCount++;
			}
		}

		private void CalibrateFwdAcc(Vector3D acc) {
			if (acc.Length() > 2200) {
				if (g_calibrationCount == g_calibrationIterations) {
					g_v_acc_direction /= g_calibrationIterations;
					double x = g_v_acc_direction.x;
					double y = g_v_acc_direction.y;
					double z = g_v_acc_direction.z;
					g_calFwdAcc = false;
                    SetCalibrateIndicator(1, true);
                    MessageBox.Show("Forward motion calibrated, calibrate gyro next.\nPress 'OK' when ready.");
					//g_v_gy_direction = new Vector3D(y * z, x * z, x * y);
					////g_v_gy_direction = new Vector3D(z, x, y);
					//SetCalibrateIndicator(2, true);
					//g_angle = 0;
					//g_velocity = 0;
					//g_readyToRun = true;
				}
				else {
					g_v_acc_direction += acc;
					g_calibrationCount++;
				}
			}
		}

		private void CalibrateFwdGy(Vector3D gy) {
			double gy_len = gy.Length();
			if (gy_len > 5500) {
				g_v_gy_direction = gy;
				g_calFwdGy = false;
				SetCalibrateIndicator(2, true);
				g_angle = 0;
				g_velocity = 0;
				g_readyToRun = true;
			}
		}

		private void CorrectByOffset(ref Vector3D v_reading_acc_raw, ref Vector3D v_reading_gy_raw) {
			// Accelerometer
			v_reading_acc_raw = v_reading_acc_raw - g_v_acc_zero;

			// Gyro
			v_reading_gy_raw = v_reading_gy_raw - g_v_gy_zero;
		}

		private void StoreSettings() {
			Settings.Default["acc_zero_x"] = g_v_acc_zero.x;
			Settings.Default["acc_zero_y"] = g_v_acc_zero.y;
			Settings.Default["acc_zero_z"] = g_v_acc_zero.z;

			Settings.Default["acc_direction_x"] = g_v_acc_direction.x;
			Settings.Default["acc_direction_y"] = g_v_acc_direction.y;
			Settings.Default["acc_direction_z"] = g_v_acc_direction.z;

			Settings.Default["gy_zero_x"] = g_v_gy_zero.x;
			Settings.Default["gy_zero_y"] = g_v_gy_zero.y;
			Settings.Default["gy_zero_z"] = g_v_gy_zero.z;

			Settings.Default["gy_direction_x"] = g_v_gy_direction.x;
			Settings.Default["gy_direction_y"] = g_v_gy_direction.y;
			Settings.Default["gy_direction_z"] = g_v_gy_direction.z;
			Settings.Default.Save();
		}

		private void LoadSettings() {
			g_v_acc_zero.x = (double)Settings.Default["acc_zero_x"];
			g_v_acc_zero.y = (double)Settings.Default["acc_zero_y"];
			g_v_acc_zero.z = (double)Settings.Default["acc_zero_z"];

			g_v_acc_direction.x = (double)Settings.Default["acc_direction_x"];
			g_v_acc_direction.y = (double)Settings.Default["acc_direction_y"];
			g_v_acc_direction.z = (double)Settings.Default["acc_direction_z"];

			g_v_gy_zero.x = (double)Settings.Default["gy_zero_x"];
			g_v_gy_zero.y = (double)Settings.Default["gy_zero_y"];
			g_v_gy_zero.z = (double)Settings.Default["gy_zero_z"];

			g_v_gy_direction.x = (double)Settings.Default["gy_direction_x"];
			g_v_gy_direction.y = (double)Settings.Default["gy_direction_y"];
			g_v_gy_direction.z = (double)Settings.Default["gy_direction_z"];
		}

		/// <summary>
		/// Sets a visual indicator on the GUI on which systems are calibrated
		///     0: Zero
		///     1: Accelerometer Fwd
		///     2: Gyro Fwd
		/// </summary>
		/// <param name="indicator"></param>
		/// <param name="state"></param>
		private delegate void SCI_delegate(int indicator, bool state);
		private void SetCalibrateIndicator(int indicator, bool state) {
			if (InvokeRequired) {
				BeginInvoke(new SCI_delegate(SetCalibrateIndicator), indicator, state);
				return;
			}
			switch (indicator) {
				case 0: chkZeroCalibrated.Checked = state;
					break;
                case 1: chkAccFwdCalibrated.Checked = state;
					break;
				case 2: chkGyroFwdCalibrated.Checked = state;
					break;
				default:
					break;
			}
		}
		#endregion

		#region Filter
		private void FilterAcc(ref Vector3D acc, ref Vector3D gy, double dt) {
			acc.x = g_k_acc_x.getAngle(acc.x, 0, dt);
			acc.y = g_k_acc_y.getAngle(acc.y, 0, dt);
			acc.z = g_k_acc_z.getAngle(acc.z, 0, dt);
		}

		private void FilterGyro(ref Vector3D gy, double dt) {
			gy.x = g_k_gy_x.getAngle(gy.x, 0, dt);
			gy.y = g_k_gy_y.getAngle(gy.y, 0, dt);
			gy.z = g_k_gy_z.getAngle(gy.z, 0, dt);
		}
		#endregion

		/// <summary>
		/// Splits the data String and generates a accelerometer and a gyro vector.
		/// Returns a bool determining if the operation was successful
		/// </summary>
		/// <param name="data"></param>
		/// <param name="acc"></param>
		/// <param name="gy"></param>
		/// <returns></returns>
		private bool GetReadings(String data, out Vector3D acc, out Vector3D gy, out double deltaTime) {
			data.Replace(@"\r", "");
			string[] dataArr = data.Split(',');

			// Data only - Start
			StringBuilder sb = new StringBuilder();
			int i = 0;
			foreach (String s in dataArr) {
				sb.Append(string.Format("v{0}:{1}- ", ++i, s));
			}
			g_rawData = sb.ToString();
			// Data only - Stop

			if (dataArr.Length > 8) {

				int loops = Int16.Parse(dataArr[8]);
				double acc_x_raw = Int32.Parse(dataArr[1]);
				double acc_y_raw = Int32.Parse(dataArr[2]);
				double acc_z_raw = Int32.Parse(dataArr[3]);
				acc = new Vector3D(acc_x_raw, acc_y_raw, acc_z_raw);

				double gy_x_raw = Int32.Parse(dataArr[4]);
				double gy_y_raw = Int32.Parse(dataArr[5]);
				double gy_z_raw = Int32.Parse(dataArr[6]);
				gy = new Vector3D(gy_x_raw, gy_y_raw, gy_z_raw);

				if (dataArr.Length > 9)
					deltaTime = Convert.ToDouble(dataArr[9]) / 1000;
				else
					deltaTime = 0.030;
				try { this.Invoke(new EventHandler(PrintRawDataEH)); }
				catch { }
				return true;
			}
			else {
				acc = null;
				gy = null;
				deltaTime = 0;
				return false;
			}
		}

		private void printRawData(Vector3D v_acc, Vector3D v_gy, double dt) {
			if (g_input == Input.COM && g_readings < g_jitterCount) { return; }

			// Gets the projection vector for both accelerometer and gyro. 
			// This is the directional vector with the correct orientation
			Vector v_acc_projection = Vector.Projection(v_acc, g_v_acc_direction);

			// Gets the angle between the directionally calibrated vector 
			// and the current measured vector from the gyro and the accelerometer
			double gyroAngle = v_gy.Angle(g_v_gy_direction);
            Vector3D v_cross = g_v_acc_zero.CrossProduct(g_v_acc_direction);
            Vector3D v_acc_project_on_plane = v_acc.ProjectionOnPlane(v_cross.Normalize());
            //double accAngle = AccelerationTest(v_acc, v_gy, dt);
            double accAngle = v_acc_project_on_plane.Angle(g_v_acc_direction);
            double accAngle1 = v_acc.Angle(g_v_acc_direction);

            // Gets the length of the projected (directional) vector 
			// (ie. the acceleration in the desired direction)
			g_accLength = Vector.ProjectionLenght(v_acc, g_v_acc_direction);

			// Setting the thresholds for the gyro and accelerometer
			UpdateThresholds();


			Vector v_gy_projection = Vector.Projection(v_gy, g_v_gy_direction);
			double gyroValue = v_gy_projection.Length();
            if (gyroValue > 30)
                //g_angle = g_accLength;
                if (gyroAngle < 90) {
                    //if (gyroValue > g_gyro_last)
                    g_angle -= (gyroValue * dt) * ANGLE_MULTIPLIER;
                }
                else {
                    //if (gyroValue < g_gyro_last)
                    g_angle += (gyroValue * dt) * ANGLE_MULTIPLIER;
                }

			//g_angle += (v_gy.z * dt) * ANGLE_MULTIPLIER;
			g_gyro_last = gyroValue;


			double acceleration = 0.0;

            acceleration = v_acc_projection.Length() - Math.Abs(g_angle);
            //acceleration = v_acc_projection.Length();
			//acceleration = v_acc.Length() - g_v_acc_zero.Length();
			//acceleration = g_accLength;
			//}
            //acceleration = accLength - v_test_length;
            if (g_readings % 2 != 0) { return; }
			if (acceleration > G_ACCELERATION_THRESHOLD)
				CalculateAcceleration(accAngle, acceleration, dt);

			if (chkAccX.Checked) AddNewPoint("acc_x", g_readingIndex, v_acc.x);
			if (chkAccY.Checked) AddNewPoint("acc_y", g_readingIndex, v_acc.y);
			if (chkAccZ.Checked) AddNewPoint("acc_z", g_readingIndex, v_acc.z);

            if (chkGyX.Checked) AddNewPoint("gy_x", g_readingIndex, v_gy.x);
            if (chkGyY.Checked) AddNewPoint("gy_y", g_readingIndex, v_gy.y);
            //if (chkGyY.Checked) AddNewPoint("gy_y", g_readingIndex, accAngle1);
            //if (chkGyZ.Checked) AddNewPoint("gy_z", g_readingIndex, v_gy.z);
            if (chkGyZ.Checked) AddNewPoint("gy_z", g_readingIndex, accAngle);

			if (chkRacc.Checked) AddNewPoint("Racc", g_readingIndex, acceleration);
			if (chkRgy.Checked) AddNewPoint("GAng", g_readingIndex, g_angle);
			if (chkRtot.Checked) AddNewPoint("AccLen", g_readingIndex, g_accLength);
			if (chkVelocity.Checked) AddNewPoint("Velocity", g_readingIndex, Math.Abs(g_velocity));
		}

		private double AccelerationTest(Vector3D v_acc, Vector3D v_gy, double dt) {
			try {
				Vector3D v_cross = g_v_acc_zero.CrossProduct(g_v_acc_direction);
				Vector3D v_projection_on_plane = v_acc.ProjectionOnPlane(v_cross) - g_v_acc_zero;
				Vector v_projection = v_acc.Projection(g_v_acc_direction);
				double k = 100;
				double angle = v_projection_on_plane.Angle(g_v_acc_direction);
				g_k_angle.setRmeasure(0.3);
				g_k_angle.setQangle(0.0001);
                g_k_angle.setQbias(0.001);
				g_k_angle.getAngle(angle, v_gy.Length(), dt);
				return angle;
				//return v_projection.Length() - k * angle;

			}
			catch (Exception e) { MessageBox.Show(e.StackTrace + "\n" + e.Message); return 0; }
		}

		//private void printRawData(Vector3D v_acc, Vector3D v_gy, double dt) {
		//    if (g_input == Input.COM && g_readings < g_jitterCount) { return; }

		//    // Gets the projection vector for both accelerometer and gyro. 
		//    // This is the directional vector with the correct orientation
		//    Vector v_gy_projection = Vector.Projection(v_gy, g_v_gy_direction);
		//    Vector v_acc_projection = Vector.Projection(v_acc, g_v_acc_direction);

		//    // Gets the angle between the directionally calibrated vector 
		//    // and the current measured vector from the gyro and the accelerometer
		//    double gyroAngle = v_gy.Angle(g_v_gy_direction);
		//    double accAngle = v_acc.Angle(g_v_acc_direction);

		//    // Gets the length of the projected (directional) vector 
		//    // (ie. the acceleration in the desired direction)
		//    g_accLength = Vector.ProjectionLenght(v_acc, g_v_acc_direction);

		//    double gyroValue = v_gy_projection.Length();

		//    // Setting the thresholds for the gyro and accelerometer
		//    UpdateThresholds();

		//    double acceleration = 0.0;
		//    //if (gyroValue > g_gyro_last) {
		//    g_k_g_angle.setQangle(0.1);
		//    g_k_g_angle.setQbias(0.03);
		//    //g_angle = g_k_g_angle.getAngle(g_accLength, gyroValue, dt);
		//    if (gyroAngle < 90) {
		//        //if (gyroValue > g_gyro_last)
		//        g_angle -= (gyroValue * dt) * ANGLE_MULTIPLIER;
		//    }
		//    else {
		//        //if (gyroValue < g_gyro_last)
		//        g_angle += (gyroValue * dt) * ANGLE_MULTIPLIER;
		//    }
		//    //}
		//    g_gyro_last = gyroValue;

		//    //if (gyroValue < G_GYRO_THRESHOLD) {
		//    //g_angle = accLength;
		//    //acceleration = g_accLength - Math.Abs(g_angle);
		//    acceleration = g_accLength;
		//    //}
		//    //acceleration = accLength - v_test_length;
		//    if (acceleration > G_ACCELERATION_THRESHOLD)
		//        CalculateAcceleration(accAngle, acceleration, dt);

		//    //else if (g_velocity > 1000000)
		//    //    g_velocity = 1000000;

		//    //if (chkAccX.Checked) AddNewPoint("acc_x", g_readingIndex, GetAverage().Length());
		//    //if (chkAccY.Checked) AddNewPoint("acc_y", g_readingIndex, g_recent.First().Length());
		//    //if (chkAccZ.Checked) AddNewPoint("acc_z", g_readingIndex, v_gy_projection.Length());
		//    if (chkAccX.Checked) AddNewPoint("acc_x", g_readingIndex, v_acc.x);
		//    if (chkAccY.Checked) AddNewPoint("acc_y", g_readingIndex, v_acc.y);
		//    if (chkAccZ.Checked) AddNewPoint("acc_z", g_readingIndex, v_acc.z);

		//    if (chkGyX.Checked) AddNewPoint("gy_x", g_readingIndex, v_gy.x);
		//    if (chkGyY.Checked) AddNewPoint("gy_y", g_readingIndex, v_gy.y);
		//    //if (chkGyZ.Checked) AddNewPoint("gy_z", g_readingIndex, v_gy.z);
		//    if (chkGyZ.Checked) AddNewPoint("gy_z", g_readingIndex, G_ACCELERATION_THRESHOLD);

		//    if (chkRacc.Checked) AddNewPoint("Racc", g_readingIndex, acceleration);
		//    //if (chkRacc.Checked) AddNewPoint("Racc", g_readingIndex, v_acc.Length());
		//    if (chkRgy.Checked) AddNewPoint("GAng", g_readingIndex, g_angle);
		//    if (chkRtot.Checked) AddNewPoint("AccLen", g_readingIndex, g_accLength);
		//    if (chkVelocity.Checked) AddNewPoint("Velocity", g_readingIndex, Math.Abs(g_velocity));
		//}

		private void CalculateAcceleration(double accAngle, double acceleration, double deltaTime) {
            if (Math.Abs(g_velocity) < 50) {
                if (accAngle <= 90)
                    g_fwd = true;
                else
                    g_fwd = false;
            }
            if (g_fwd) {
				if (accAngle < 90)
					g_velocity += acceleration * deltaTime * G_ACC_OFFSET_ACCELERATE;
				else
					g_velocity -= acceleration * deltaTime * G_ACC_OFFSET_DECELERATE;
            }
            else {
                if (accAngle >= 90)
                    g_velocity += acceleration * deltaTime * G_ACC_OFFSET_DECELERATE;
                else
                    g_velocity -= acceleration * deltaTime * G_ACC_OFFSET_ACCELERATE;
            }
            if (g_velocity < 0)
                g_velocity = 51;
		}

		private double CalcAngle(double value) {
			if (value > 90)
				value = value - 90;
			return 45 - Math.Abs(45 - value);
		}

		private void UpdateThresholds() {
			try { G_NOISE_CALIBRATION_THRESHOLD = Convert.ToInt32(tbxNoiseThreshold.Text); }
			catch (Exception) { }
			try { G_ACCELERATION_THRESHOLD = Convert.ToInt32(tbxAccelerationThreshold.Text); }
			catch (Exception) { }
		}


		delegate void invokeAddNewPoint(String seriesName, double x, double y);
		private void AddNewPoint(String seriesName, double x, double y) {
			//if (x < 300) return;
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
				case "GAng": lblForceRg.Text = y.ToString();
					//y = g_fwd ? 10000 : -10000;
					break;
				case "AccLen": lblForceRt.Text = y.ToString(); break;
				case "Velocity": lblVelocity.Text = y.ToString(); break;
				default: break;
			}
			lblDirection.Text = g_fwd ? "FWD" : "RWD";
			chart1.Series[seriesName].Points.AddXY((double)x, (double)y);
			ZoomTrigger();
		}

		private void ZoomTrigger() {
			if (!chkFullScale.Checked) {
				chart1.ChartAreas[0].CursorX.AutoScroll = true;
				chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
				chart1.ChartAreas[0].AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;
				chart1.ChartAreas[0].AxisX.ScaleView.Zoom(g_readingIndex - tkbScale.Value, g_readingIndex);
			}
			else {
				chart1.ChartAreas[0].CursorX.AutoScroll = false;
				chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = false;
				chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset();
			}
		}

		public void PrintCSV(StreamReader sr) {
			String line;
			while ((line = sr.ReadLine()) != null) {
				ProcessData(line);
			}
		}

        private void ProcessData(String data) {
            g_readings++;
            //if (g_readings % 2 != 0) { return; }

			// Creates variables
			Vector3D acc;
			Vector3D gy;
			double deltaTime;

			//if (g_readings % 3 != 0) { return; }
			try {   // Gets the readings from the data String and stores them in the out variables
				if (GetReadings(data, out acc, out gy, out deltaTime)) {
					g_r_index++;
					// If zero has been calibrated, correct the offset
					if (!g_calibrate)
						CorrectByOffset(ref acc, ref gy);
					// Goto Calibrate section
					Calibrate(acc, gy);
					// If readyToRun (is set when fully calibrated)
                    if (g_readyToRun) {
                        g_readingIndex++;
                        //Floating calibration
                        if (g_readings > g_num_of_recent_readings && IsStationary(acc, GetAverage(), G_NOISE_CALIBRATION_THRESHOLD)) {
                            g_velocity = 0;
                            g_angle = g_accLength * 0.01;
                        }

                        g_LastDT = DateTime.Now;

                        // Sets up the filter using the values from the GUI
                        SetupFilter();

                        // Filters the accelerometer and the gyro data
                        //FilterAcc(ref acc, ref gy, deltaTime);
                        //FilterGyro(ref gy, deltaTime);

                        // Displays the data
                        printRawData(acc, gy, deltaTime);
                        //if (g_input == Input.CSV)
                        //    Thread.Sleep((int)(deltaTime * 1000) + 6);
                    }
                    else
                        g_readingIndex = 0;
					g_recent.Enqueue(acc);
				}

			}
			catch {}
		}

		private Vector3D GetAverage() {
			Vector3D temp = new Vector3D(0, 0, 0);
			for (int i = 0; i < g_num_of_recent_readings; i++) {
				temp += g_recent.Get(i);
			}
			return temp / g_num_of_recent_readings;
		}


		public void PlaySound() {
			while (g_running) {
				if (g_velocity > G_SOUND_CUTOFF_VELOCITY)
					Sound.BeepBeep(500, Convert.ToInt32(g_velocity / G_SOUND_DEVIDER), 20);
			}
		}

		public bool IsStationary(Vector3D a, Vector3D b, double accuracy) {
			bool x = a.x >= b.x - accuracy && a.x <= b.x + accuracy;
			bool y = a.y >= b.y - accuracy && a.y <= b.y + accuracy;
			bool z = a.z >= b.z - accuracy && a.z <= b.z + accuracy;
			return x && y && z && a.Length() - g_v_acc_zero.Length() < 2000;
		}
		int g_r_index = 0;
		#region Eventhandlers
		private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e) {
			if (g_input == Input.COM && g_comOpen && g_running) {
				String readFromCom = g_serialPort.ReadLine();

				double dt = DateTime.Now.Subtract(g_LastDT).Milliseconds;

				// Writing to file

				if (readFromCom.Length > 1)
					try { g_file.WriteLine(readFromCom.Substring(0, readFromCom.Length - 1) + "," + dt); } catch { }

				ProcessData(readFromCom);
			}
		}

		private void PrintRawDataEH(Object sender, EventArgs e) {
			if (!g_threadKill)
				tbxRawData.AppendText(string.Format("{0} - {1}\n", g_readingIndex, g_rawData));
		}

		private void btnDataChart_Click(object sender, EventArgs e) {
			if (pnlChart.Visible) {
				btnDataChart.Text = "Chart";
				pnlChart.Visible = false;
			}
			else {
				btnDataChart.Text = "Data";
				pnlChart.Visible = true;
			}
		}

		private void btnExit_Click(object sender, EventArgs e) {
			Application.Exit();
		}

		private void btnReset_Click(object sender, EventArgs e) {
			//running = false;
			InitChart();
			InitCalibration();
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
			startTime = DateTime.Now;
			g_r_index = 0;
			tbxRawData.Clear();
		}

		private void btnStartPause_Click(object sender, EventArgs e) {
			//if (input != Input.COM)
			//    input = Input.COM;
			if (btnStartPause.Text == "Start") {
				startTime = DateTime.Now;
				btnStartPause.Text = "Pause";
				g_running = true;
				if (g_input == Input.COM && !g_serialPort.IsOpen) {
					g_port = cbbComPort.SelectedItem.ToString();
					if (g_file != null)
						try { g_file.Close(); } catch { }
					String path = Directory.GetCurrentDirectory() + "\\Reading " + startTime.ToString("yyyy-MM-dd HH-mm-ss") + ".txt";
					g_file = new System.IO.StreamWriter(path, true);
					InitOtherStuff();
				}
				//Thread t = new Thread(() => PlaySound());
				//t.IsBackground = true;
				//t.Start();
			}
			else {
				btnStartPause.Text = "Start";
				g_running = false;
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

			chkRacc.Checked = true;
			chkRgy.Checked = true;
		}

		private void btnCalibrate_Click(object sender, EventArgs e) {
			chk_tsmTools_CalibrateSignal.Checked = true;
			InitCalibration();
		}

		private void tsmFile_open_Click(object sender, EventArgs e) {
			g_input = Input.CSV;

			g_threadKill = true;
			InitChart();
			//chk_tsmTools_CalibrateSignal.Checked = true;

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
					//this.Cursor = Cursors.WaitCursor;
					sr = new System.IO.StreamReader(fileStream);
					//this.Cursor = Cursors.Default;
				}
				catch (Exception ex) {
					MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
				}

				if (sr != null) {
					if (btnStartPause.Text == "Start") btnStartPause_Click(sender, e);
					Thread t = new Thread(() => PrintCSV(sr));
					t.IsBackground = true;
					g_threadKill = false;
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
			//MessageBox.Show("Measurements frequency: " + g_r_index / ((double)(DateTime.Now - startTime).TotalMilliseconds / 1000));
			Thread.Sleep(250);
			UpdateThresholds();
			try { g_file.Close(); } catch { }
			if (G_NOISE_CALIBRATION_THRESHOLD > 0) Settings.Default["noise_calibration_threshold"] = G_NOISE_CALIBRATION_THRESHOLD;
			if (G_ACCELERATION_THRESHOLD > 0) Settings.Default["acceleration_threshold"] = G_ACCELERATION_THRESHOLD;
			Settings.Default.Save();
			Thread.Sleep(250);
		}

		private void printToolStripMenuItem_Click(object sender, EventArgs e) {
			chart1.Printing.Print(true);
		}
		#endregion

		private void btnResetScale_Click(object sender, EventArgs e) {
			chart1.Series.Clear();
		}
	}
}
