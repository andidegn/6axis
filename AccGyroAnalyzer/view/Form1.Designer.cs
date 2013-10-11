using System.Windows.Forms;
namespace ECTunes {
    partial class Form1 {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.pnlChart = new System.Windows.Forms.Panel();
            this.chkGyroFwdCalibrated = new System.Windows.Forms.CheckBox();
            this.gbGy = new System.Windows.Forms.GroupBox();
            this.tbxGyRMeasure = new System.Windows.Forms.TextBox();
            this.lblGyRMeasure = new System.Windows.Forms.Label();
            this.tbxGyQBias = new System.Windows.Forms.TextBox();
            this.lblGyQBias = new System.Windows.Forms.Label();
            this.tbxGyQAngle = new System.Windows.Forms.TextBox();
            this.lblGyQAngle = new System.Windows.Forms.Label();
            this.chkAccFwdCalibrated = new System.Windows.Forms.CheckBox();
            this.gbAcc = new System.Windows.Forms.GroupBox();
            this.tbxAccRMeasure = new System.Windows.Forms.TextBox();
            this.lblAccRMeasure = new System.Windows.Forms.Label();
            this.tbxAccQBias = new System.Windows.Forms.TextBox();
            this.lblAccQBias = new System.Windows.Forms.Label();
            this.tbxAccQAngle = new System.Windows.Forms.TextBox();
            this.lblAccQAngle = new System.Windows.Forms.Label();
            this.chkZeroCalibrated = new System.Windows.Forms.CheckBox();
            this.lblForceRt = new System.Windows.Forms.Label();
            this.chkForceRt = new System.Windows.Forms.CheckBox();
            this.lblForceRg = new System.Windows.Forms.Label();
            this.chkForceRg = new System.Windows.Forms.CheckBox();
            this.chkForceR = new System.Windows.Forms.CheckBox();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.chkGyZ = new System.Windows.Forms.CheckBox();
            this.chkGyY = new System.Windows.Forms.CheckBox();
            this.chkGyX = new System.Windows.Forms.CheckBox();
            this.chkAccZ = new System.Windows.Forms.CheckBox();
            this.chkAccY = new System.Windows.Forms.CheckBox();
            this.chkAccX = new System.Windows.Forms.CheckBox();
            this.lblScaleValue = new System.Windows.Forms.Label();
            this.lblScale = new System.Windows.Forms.Label();
            this.tkbScale = new System.Windows.Forms.TrackBar();
            this.chkFullScale = new System.Windows.Forms.CheckBox();
            this.btnDataChart = new System.Windows.Forms.Button();
            this.tbxRawData = new System.Windows.Forms.TextBox();
            this.btnStartPause = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.lblAccX = new System.Windows.Forms.Label();
            this.lblAccY = new System.Windows.Forms.Label();
            this.lblAccZ = new System.Windows.Forms.Label();
            this.lblGyX = new System.Windows.Forms.Label();
            this.lblGyY = new System.Windows.Forms.Label();
            this.lblGyZ = new System.Windows.Forms.Label();
            this.lblForceR = new System.Windows.Forms.Label();
            this.btnCalibrate = new System.Windows.Forms.Button();
            this.msMenu = new System.Windows.Forms.MenuStrip();
            this.tsmFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmFile_open = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmTools = new System.Windows.Forms.ToolStripMenuItem();
            this.chk_tsmTools_CalibrateSignal = new System.Windows.Forms.ToolStripMenuItem();
            this.cbbComPort = new System.Windows.Forms.ComboBox();
            this.btnRefreshComPort = new System.Windows.Forms.Button();
            this.tbxGyroThreshold = new System.Windows.Forms.TextBox();
            this.tbxAccelerationThreshold = new System.Windows.Forms.TextBox();
            this.lblGyroThreshold = new System.Windows.Forms.Label();
            this.lblAccelerationThreshold = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.pnlChart.SuspendLayout();
            this.gbGy.SuspendLayout();
            this.gbAcc.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tkbScale)).BeginInit();
            this.msMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // chart1
            // 
            this.chart1.BorderlineColor = System.Drawing.Color.Black;
            this.chart1.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea2.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chart1.Legends.Add(legend2);
            this.chart1.Location = new System.Drawing.Point(29, 15);
            this.chart1.Name = "chart1";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chart1.Series.Add(series2);
            this.chart1.Size = new System.Drawing.Size(944, 461);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(149, 46);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 2;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(1165, 46);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // pnlChart
            // 
            this.pnlChart.Controls.Add(this.chkGyroFwdCalibrated);
            this.pnlChart.Controls.Add(this.gbGy);
            this.pnlChart.Controls.Add(this.chkAccFwdCalibrated);
            this.pnlChart.Controls.Add(this.gbAcc);
            this.pnlChart.Controls.Add(this.chkZeroCalibrated);
            this.pnlChart.Controls.Add(this.lblForceRt);
            this.pnlChart.Controls.Add(this.chkForceRt);
            this.pnlChart.Controls.Add(this.lblForceRg);
            this.pnlChart.Controls.Add(this.chkForceRg);
            this.pnlChart.Controls.Add(this.chkForceR);
            this.pnlChart.Controls.Add(this.btnSelectAll);
            this.pnlChart.Controls.Add(this.chkGyZ);
            this.pnlChart.Controls.Add(this.chkGyY);
            this.pnlChart.Controls.Add(this.chkGyX);
            this.pnlChart.Controls.Add(this.chkAccZ);
            this.pnlChart.Controls.Add(this.chkAccY);
            this.pnlChart.Controls.Add(this.chkAccX);
            this.pnlChart.Controls.Add(this.lblScaleValue);
            this.pnlChart.Controls.Add(this.lblScale);
            this.pnlChart.Controls.Add(this.tkbScale);
            this.pnlChart.Controls.Add(this.chkFullScale);
            this.pnlChart.Controls.Add(this.chart1);
            this.pnlChart.Location = new System.Drawing.Point(39, 75);
            this.pnlChart.Name = "pnlChart";
            this.pnlChart.Size = new System.Drawing.Size(1201, 527);
            this.pnlChart.TabIndex = 3;
            // 
            // chkGyroFwdCalibrated
            // 
            this.chkGyroFwdCalibrated.AutoSize = true;
            this.chkGyroFwdCalibrated.Location = new System.Drawing.Point(1140, 112);
            this.chkGyroFwdCalibrated.Name = "chkGyroFwdCalibrated";
            this.chkGyroFwdCalibrated.Size = new System.Drawing.Size(64, 17);
            this.chkGyroFwdCalibrated.TabIndex = 23;
            this.chkGyroFwdCalibrated.Text = "Gyro Dir";
            this.chkGyroFwdCalibrated.UseVisualStyleBackColor = true;
            // 
            // gbGy
            // 
            this.gbGy.Controls.Add(this.tbxGyRMeasure);
            this.gbGy.Controls.Add(this.lblGyRMeasure);
            this.gbGy.Controls.Add(this.tbxGyQBias);
            this.gbGy.Controls.Add(this.lblGyQBias);
            this.gbGy.Controls.Add(this.tbxGyQAngle);
            this.gbGy.Controls.Add(this.lblGyQAngle);
            this.gbGy.Location = new System.Drawing.Point(979, 391);
            this.gbGy.Name = "gbGy";
            this.gbGy.Size = new System.Drawing.Size(206, 101);
            this.gbGy.TabIndex = 23;
            this.gbGy.TabStop = false;
            this.gbGy.Text = "Gyro";
            // 
            // tbxGyRMeasure
            // 
            this.tbxGyRMeasure.Location = new System.Drawing.Point(90, 69);
            this.tbxGyRMeasure.Name = "tbxGyRMeasure";
            this.tbxGyRMeasure.Size = new System.Drawing.Size(100, 20);
            this.tbxGyRMeasure.TabIndex = 5;
            this.tbxGyRMeasure.Text = "0.03";
            // 
            // lblGyRMeasure
            // 
            this.lblGyRMeasure.AutoSize = true;
            this.lblGyRMeasure.Location = new System.Drawing.Point(7, 72);
            this.lblGyRMeasure.Name = "lblGyRMeasure";
            this.lblGyRMeasure.Size = new System.Drawing.Size(59, 13);
            this.lblGyRMeasure.TabIndex = 4;
            this.lblGyRMeasure.Text = "R Measure";
            // 
            // tbxGyQBias
            // 
            this.tbxGyQBias.Location = new System.Drawing.Point(90, 43);
            this.tbxGyQBias.Name = "tbxGyQBias";
            this.tbxGyQBias.Size = new System.Drawing.Size(100, 20);
            this.tbxGyQBias.TabIndex = 3;
            this.tbxGyQBias.Text = "0.003";
            // 
            // lblGyQBias
            // 
            this.lblGyQBias.AutoSize = true;
            this.lblGyQBias.Location = new System.Drawing.Point(7, 46);
            this.lblGyQBias.Name = "lblGyQBias";
            this.lblGyQBias.Size = new System.Drawing.Size(38, 13);
            this.lblGyQBias.TabIndex = 2;
            this.lblGyQBias.Text = "Q Bias";
            // 
            // tbxGyQAngle
            // 
            this.tbxGyQAngle.Location = new System.Drawing.Point(90, 17);
            this.tbxGyQAngle.Name = "tbxGyQAngle";
            this.tbxGyQAngle.Size = new System.Drawing.Size(100, 20);
            this.tbxGyQAngle.TabIndex = 1;
            this.tbxGyQAngle.Text = "0.1";
            // 
            // lblGyQAngle
            // 
            this.lblGyQAngle.AutoSize = true;
            this.lblGyQAngle.Location = new System.Drawing.Point(7, 20);
            this.lblGyQAngle.Name = "lblGyQAngle";
            this.lblGyQAngle.Size = new System.Drawing.Size(45, 13);
            this.lblGyQAngle.TabIndex = 0;
            this.lblGyQAngle.Text = "Q Angle";
            // 
            // chkAccFwdCalibrated
            // 
            this.chkAccFwdCalibrated.AutoSize = true;
            this.chkAccFwdCalibrated.Location = new System.Drawing.Point(1140, 88);
            this.chkAccFwdCalibrated.Name = "chkAccFwdCalibrated";
            this.chkAccFwdCalibrated.Size = new System.Drawing.Size(61, 17);
            this.chkAccFwdCalibrated.TabIndex = 22;
            this.chkAccFwdCalibrated.Text = "Acc Dir";
            this.chkAccFwdCalibrated.UseVisualStyleBackColor = true;
            // 
            // gbAcc
            // 
            this.gbAcc.Controls.Add(this.tbxAccRMeasure);
            this.gbAcc.Controls.Add(this.lblAccRMeasure);
            this.gbAcc.Controls.Add(this.tbxAccQBias);
            this.gbAcc.Controls.Add(this.lblAccQBias);
            this.gbAcc.Controls.Add(this.tbxAccQAngle);
            this.gbAcc.Controls.Add(this.lblAccQAngle);
            this.gbAcc.Location = new System.Drawing.Point(978, 284);
            this.gbAcc.Name = "gbAcc";
            this.gbAcc.Size = new System.Drawing.Size(206, 101);
            this.gbAcc.TabIndex = 22;
            this.gbAcc.TabStop = false;
            this.gbAcc.Text = "Accelerometer";
            // 
            // tbxAccRMeasure
            // 
            this.tbxAccRMeasure.Location = new System.Drawing.Point(90, 69);
            this.tbxAccRMeasure.Name = "tbxAccRMeasure";
            this.tbxAccRMeasure.Size = new System.Drawing.Size(100, 20);
            this.tbxAccRMeasure.TabIndex = 5;
            this.tbxAccRMeasure.Text = "0.03";
            // 
            // lblAccRMeasure
            // 
            this.lblAccRMeasure.AutoSize = true;
            this.lblAccRMeasure.Location = new System.Drawing.Point(7, 72);
            this.lblAccRMeasure.Name = "lblAccRMeasure";
            this.lblAccRMeasure.Size = new System.Drawing.Size(59, 13);
            this.lblAccRMeasure.TabIndex = 4;
            this.lblAccRMeasure.Text = "R Measure";
            // 
            // tbxAccQBias
            // 
            this.tbxAccQBias.Location = new System.Drawing.Point(90, 43);
            this.tbxAccQBias.Name = "tbxAccQBias";
            this.tbxAccQBias.Size = new System.Drawing.Size(100, 20);
            this.tbxAccQBias.TabIndex = 3;
            this.tbxAccQBias.Text = "0.003";
            // 
            // lblAccQBias
            // 
            this.lblAccQBias.AutoSize = true;
            this.lblAccQBias.Location = new System.Drawing.Point(7, 46);
            this.lblAccQBias.Name = "lblAccQBias";
            this.lblAccQBias.Size = new System.Drawing.Size(38, 13);
            this.lblAccQBias.TabIndex = 2;
            this.lblAccQBias.Text = "Q Bias";
            // 
            // tbxAccQAngle
            // 
            this.tbxAccQAngle.Location = new System.Drawing.Point(90, 17);
            this.tbxAccQAngle.Name = "tbxAccQAngle";
            this.tbxAccQAngle.Size = new System.Drawing.Size(100, 20);
            this.tbxAccQAngle.TabIndex = 1;
            this.tbxAccQAngle.Text = "0.1";
            // 
            // lblAccQAngle
            // 
            this.lblAccQAngle.AutoSize = true;
            this.lblAccQAngle.Location = new System.Drawing.Point(7, 20);
            this.lblAccQAngle.Name = "lblAccQAngle";
            this.lblAccQAngle.Size = new System.Drawing.Size(45, 13);
            this.lblAccQAngle.TabIndex = 0;
            this.lblAccQAngle.Text = "Q Angle";
            // 
            // chkZeroCalibrated
            // 
            this.chkZeroCalibrated.AutoSize = true;
            this.chkZeroCalibrated.Location = new System.Drawing.Point(1140, 65);
            this.chkZeroCalibrated.Name = "chkZeroCalibrated";
            this.chkZeroCalibrated.Size = new System.Drawing.Size(48, 17);
            this.chkZeroCalibrated.TabIndex = 21;
            this.chkZeroCalibrated.Text = "Zero";
            this.chkZeroCalibrated.UseVisualStyleBackColor = true;
            // 
            // lblForceRt
            // 
            this.lblForceRt.AutoSize = true;
            this.lblForceRt.Location = new System.Drawing.Point(1066, 262);
            this.lblForceRt.Name = "lblForceRt";
            this.lblForceRt.Size = new System.Drawing.Size(13, 13);
            this.lblForceRt.TabIndex = 20;
            this.lblForceRt.Text = "0";
            // 
            // chkForceRt
            // 
            this.chkForceRt.AutoSize = true;
            this.chkForceRt.Checked = true;
            this.chkForceRt.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkForceRt.Location = new System.Drawing.Point(978, 261);
            this.chkForceRt.Name = "chkForceRt";
            this.chkForceRt.Size = new System.Drawing.Size(82, 17);
            this.chkForceRt.TabIndex = 21;
            this.chkForceRt.Text = "Force (Rtot)";
            this.chkForceRt.UseVisualStyleBackColor = true;
            // 
            // lblForceRg
            // 
            this.lblForceRg.AutoSize = true;
            this.lblForceRg.Location = new System.Drawing.Point(1066, 239);
            this.lblForceRg.Name = "lblForceRg";
            this.lblForceRg.Size = new System.Drawing.Size(13, 13);
            this.lblForceRg.TabIndex = 18;
            this.lblForceRg.Text = "0";
            // 
            // chkForceRg
            // 
            this.chkForceRg.AutoSize = true;
            this.chkForceRg.Checked = true;
            this.chkForceRg.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkForceRg.Location = new System.Drawing.Point(978, 238);
            this.chkForceRg.Name = "chkForceRg";
            this.chkForceRg.Size = new System.Drawing.Size(81, 17);
            this.chkForceRg.TabIndex = 19;
            this.chkForceRg.Text = "Force (Rgy)";
            this.chkForceRg.UseVisualStyleBackColor = true;
            // 
            // chkForceR
            // 
            this.chkForceR.AutoSize = true;
            this.chkForceR.Checked = true;
            this.chkForceR.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkForceR.Location = new System.Drawing.Point(978, 215);
            this.chkForceR.Name = "chkForceR";
            this.chkForceR.Size = new System.Drawing.Size(88, 17);
            this.chkForceR.TabIndex = 17;
            this.chkForceR.Text = "Force (Racc)";
            this.chkForceR.UseVisualStyleBackColor = true;
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Location = new System.Drawing.Point(978, 38);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(34, 23);
            this.btnSelectAll.TabIndex = 16;
            this.btnSelectAll.Text = "All";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // chkGyZ
            // 
            this.chkGyZ.AutoSize = true;
            this.chkGyZ.Location = new System.Drawing.Point(978, 186);
            this.chkGyZ.Name = "chkGyZ";
            this.chkGyZ.Size = new System.Drawing.Size(58, 17);
            this.chkGyZ.TabIndex = 15;
            this.chkGyZ.Text = "Gyro Z";
            this.chkGyZ.UseVisualStyleBackColor = true;
            // 
            // chkGyY
            // 
            this.chkGyY.AutoSize = true;
            this.chkGyY.Location = new System.Drawing.Point(978, 163);
            this.chkGyY.Name = "chkGyY";
            this.chkGyY.Size = new System.Drawing.Size(58, 17);
            this.chkGyY.TabIndex = 14;
            this.chkGyY.Text = "Gyro Y";
            this.chkGyY.UseVisualStyleBackColor = true;
            // 
            // chkGyX
            // 
            this.chkGyX.AutoSize = true;
            this.chkGyX.Location = new System.Drawing.Point(978, 140);
            this.chkGyX.Name = "chkGyX";
            this.chkGyX.Size = new System.Drawing.Size(58, 17);
            this.chkGyX.TabIndex = 13;
            this.chkGyX.Text = "Gyro X";
            this.chkGyX.UseVisualStyleBackColor = true;
            // 
            // chkAccZ
            // 
            this.chkAccZ.AutoSize = true;
            this.chkAccZ.Location = new System.Drawing.Point(978, 112);
            this.chkAccZ.Name = "chkAccZ";
            this.chkAccZ.Size = new System.Drawing.Size(55, 17);
            this.chkAccZ.TabIndex = 12;
            this.chkAccZ.Text = "Acc Z";
            this.chkAccZ.UseVisualStyleBackColor = true;
            // 
            // chkAccY
            // 
            this.chkAccY.AutoSize = true;
            this.chkAccY.Location = new System.Drawing.Point(978, 89);
            this.chkAccY.Name = "chkAccY";
            this.chkAccY.Size = new System.Drawing.Size(55, 17);
            this.chkAccY.TabIndex = 11;
            this.chkAccY.Text = "Acc Y";
            this.chkAccY.UseVisualStyleBackColor = true;
            // 
            // chkAccX
            // 
            this.chkAccX.AutoSize = true;
            this.chkAccX.Location = new System.Drawing.Point(978, 66);
            this.chkAccX.Name = "chkAccX";
            this.chkAccX.Size = new System.Drawing.Size(55, 17);
            this.chkAccX.TabIndex = 10;
            this.chkAccX.Text = "Acc X";
            this.chkAccX.UseVisualStyleBackColor = true;
            // 
            // lblScaleValue
            // 
            this.lblScaleValue.AutoSize = true;
            this.lblScaleValue.Location = new System.Drawing.Point(833, 483);
            this.lblScaleValue.Name = "lblScaleValue";
            this.lblScaleValue.Size = new System.Drawing.Size(19, 13);
            this.lblScaleValue.TabIndex = 9;
            this.lblScaleValue.Text = "25";
            // 
            // lblScale
            // 
            this.lblScale.AutoSize = true;
            this.lblScale.Location = new System.Drawing.Point(75, 483);
            this.lblScale.Name = "lblScale";
            this.lblScale.Size = new System.Drawing.Size(34, 13);
            this.lblScale.TabIndex = 8;
            this.lblScale.Text = "Scale";
            // 
            // tkbScale
            // 
            this.tkbScale.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tkbScale.LargeChange = 50;
            this.tkbScale.Location = new System.Drawing.Point(110, 483);
            this.tkbScale.Maximum = 500;
            this.tkbScale.Minimum = 10;
            this.tkbScale.Name = "tkbScale";
            this.tkbScale.Size = new System.Drawing.Size(716, 45);
            this.tkbScale.TabIndex = 7;
            this.tkbScale.Value = 25;
            this.tkbScale.Scroll += new System.EventHandler(this.tkbScale_Scroll);
            // 
            // chkFullScale
            // 
            this.chkFullScale.AutoSize = true;
            this.chkFullScale.Location = new System.Drawing.Point(978, 15);
            this.chkFullScale.Name = "chkFullScale";
            this.chkFullScale.Size = new System.Drawing.Size(72, 17);
            this.chkFullScale.TabIndex = 6;
            this.chkFullScale.Text = "Full Scale";
            this.chkFullScale.UseVisualStyleBackColor = true;
            this.chkFullScale.CheckedChanged += new System.EventHandler(this.chkFullScale_CheckedChanged);
            // 
            // btnDataChart
            // 
            this.btnDataChart.Location = new System.Drawing.Point(559, 46);
            this.btnDataChart.Name = "btnDataChart";
            this.btnDataChart.Size = new System.Drawing.Size(75, 23);
            this.btnDataChart.TabIndex = 4;
            this.btnDataChart.Text = "Data";
            this.btnDataChart.UseVisualStyleBackColor = true;
            this.btnDataChart.Click += new System.EventHandler(this.btnDataChart_Click);
            // 
            // tbxRawData
            // 
            this.tbxRawData.BackColor = System.Drawing.Color.White;
            this.tbxRawData.Location = new System.Drawing.Point(68, 90);
            this.tbxRawData.Multiline = true;
            this.tbxRawData.Name = "tbxRawData";
            this.tbxRawData.ReadOnly = true;
            this.tbxRawData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbxRawData.Size = new System.Drawing.Size(549, 461);
            this.tbxRawData.TabIndex = 1;
            // 
            // btnStartPause
            // 
            this.btnStartPause.Location = new System.Drawing.Point(68, 46);
            this.btnStartPause.Name = "btnStartPause";
            this.btnStartPause.Size = new System.Drawing.Size(75, 23);
            this.btnStartPause.TabIndex = 1;
            this.btnStartPause.Text = "Start";
            this.btnStartPause.UseVisualStyleBackColor = true;
            this.btnStartPause.Click += new System.EventHandler(this.btnStartPause_Click);
            // 
            // lblAccX
            // 
            this.lblAccX.AutoSize = true;
            this.lblAccX.Location = new System.Drawing.Point(1105, 141);
            this.lblAccX.Name = "lblAccX";
            this.lblAccX.Size = new System.Drawing.Size(13, 13);
            this.lblAccX.TabIndex = 6;
            this.lblAccX.Text = "0";
            // 
            // lblAccY
            // 
            this.lblAccY.AutoSize = true;
            this.lblAccY.Location = new System.Drawing.Point(1105, 164);
            this.lblAccY.Name = "lblAccY";
            this.lblAccY.Size = new System.Drawing.Size(13, 13);
            this.lblAccY.TabIndex = 7;
            this.lblAccY.Text = "0";
            // 
            // lblAccZ
            // 
            this.lblAccZ.AutoSize = true;
            this.lblAccZ.Location = new System.Drawing.Point(1105, 187);
            this.lblAccZ.Name = "lblAccZ";
            this.lblAccZ.Size = new System.Drawing.Size(13, 13);
            this.lblAccZ.TabIndex = 8;
            this.lblAccZ.Text = "0";
            // 
            // lblGyX
            // 
            this.lblGyX.AutoSize = true;
            this.lblGyX.Location = new System.Drawing.Point(1105, 215);
            this.lblGyX.Name = "lblGyX";
            this.lblGyX.Size = new System.Drawing.Size(13, 13);
            this.lblGyX.TabIndex = 9;
            this.lblGyX.Text = "0";
            // 
            // lblGyY
            // 
            this.lblGyY.AutoSize = true;
            this.lblGyY.Location = new System.Drawing.Point(1105, 238);
            this.lblGyY.Name = "lblGyY";
            this.lblGyY.Size = new System.Drawing.Size(13, 13);
            this.lblGyY.TabIndex = 10;
            this.lblGyY.Text = "0";
            // 
            // lblGyZ
            // 
            this.lblGyZ.AutoSize = true;
            this.lblGyZ.Location = new System.Drawing.Point(1105, 261);
            this.lblGyZ.Name = "lblGyZ";
            this.lblGyZ.Size = new System.Drawing.Size(13, 13);
            this.lblGyZ.TabIndex = 11;
            this.lblGyZ.Text = "0";
            // 
            // lblForceR
            // 
            this.lblForceR.AutoSize = true;
            this.lblForceR.Location = new System.Drawing.Point(1105, 290);
            this.lblForceR.Name = "lblForceR";
            this.lblForceR.Size = new System.Drawing.Size(13, 13);
            this.lblForceR.TabIndex = 12;
            this.lblForceR.Text = "0";
            // 
            // btnCalibrate
            // 
            this.btnCalibrate.Location = new System.Drawing.Point(230, 46);
            this.btnCalibrate.Name = "btnCalibrate";
            this.btnCalibrate.Size = new System.Drawing.Size(75, 23);
            this.btnCalibrate.TabIndex = 13;
            this.btnCalibrate.Text = "Calibrate";
            this.btnCalibrate.UseVisualStyleBackColor = true;
            this.btnCalibrate.Click += new System.EventHandler(this.btnCalibrate_Click);
            // 
            // msMenu
            // 
            this.msMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmFile,
            this.tsmTools});
            this.msMenu.Location = new System.Drawing.Point(0, 0);
            this.msMenu.Name = "msMenu";
            this.msMenu.Size = new System.Drawing.Size(1265, 24);
            this.msMenu.TabIndex = 14;
            this.msMenu.Text = "menuStrip1";
            // 
            // tsmFile
            // 
            this.tsmFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmFile_open});
            this.tsmFile.Name = "tsmFile";
            this.tsmFile.Size = new System.Drawing.Size(37, 20);
            this.tsmFile.Text = "File";
            // 
            // tsmFile_open
            // 
            this.tsmFile_open.Name = "tsmFile_open";
            this.tsmFile_open.Size = new System.Drawing.Size(103, 22);
            this.tsmFile_open.Text = "Open";
            this.tsmFile_open.Click += new System.EventHandler(this.tsmFile_open_Click);
            // 
            // tsmTools
            // 
            this.tsmTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chk_tsmTools_CalibrateSignal});
            this.tsmTools.Name = "tsmTools";
            this.tsmTools.Size = new System.Drawing.Size(48, 20);
            this.tsmTools.Text = "Tools";
            // 
            // chk_tsmTools_CalibrateSignal
            // 
            this.chk_tsmTools_CalibrateSignal.Checked = true;
            this.chk_tsmTools_CalibrateSignal.CheckOnClick = true;
            this.chk_tsmTools_CalibrateSignal.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_tsmTools_CalibrateSignal.Name = "chk_tsmTools_CalibrateSignal";
            this.chk_tsmTools_CalibrateSignal.Size = new System.Drawing.Size(156, 22);
            this.chk_tsmTools_CalibrateSignal.Text = "Calibrate Signal";
            // 
            // cbbComPort
            // 
            this.cbbComPort.FormattingEnabled = true;
            this.cbbComPort.Location = new System.Drawing.Point(338, 46);
            this.cbbComPort.Name = "cbbComPort";
            this.cbbComPort.Size = new System.Drawing.Size(121, 21);
            this.cbbComPort.TabIndex = 15;
            // 
            // btnRefreshComPort
            // 
            this.btnRefreshComPort.Location = new System.Drawing.Point(466, 46);
            this.btnRefreshComPort.Name = "btnRefreshComPort";
            this.btnRefreshComPort.Size = new System.Drawing.Size(75, 23);
            this.btnRefreshComPort.TabIndex = 16;
            this.btnRefreshComPort.Text = "Refresh";
            this.btnRefreshComPort.UseVisualStyleBackColor = true;
            this.btnRefreshComPort.Click += new System.EventHandler(this.btnRefreshComPort_Click);
            // 
            // tbxGyroThreshold
            // 
            this.tbxGyroThreshold.Location = new System.Drawing.Point(749, 48);
            this.tbxGyroThreshold.Name = "tbxGyroThreshold";
            this.tbxGyroThreshold.Size = new System.Drawing.Size(100, 20);
            this.tbxGyroThreshold.TabIndex = 17;
            this.tbxGyroThreshold.Text = "10000";
            // 
            // tbxAccelerationThreshold
            // 
            this.tbxAccelerationThreshold.Location = new System.Drawing.Point(989, 49);
            this.tbxAccelerationThreshold.Name = "tbxAccelerationThreshold";
            this.tbxAccelerationThreshold.Size = new System.Drawing.Size(100, 20);
            this.tbxAccelerationThreshold.TabIndex = 18;
            this.tbxAccelerationThreshold.Text = "10000";
            // 
            // lblGyroThreshold
            // 
            this.lblGyroThreshold.AutoSize = true;
            this.lblGyroThreshold.Location = new System.Drawing.Point(664, 51);
            this.lblGyroThreshold.Name = "lblGyroThreshold";
            this.lblGyroThreshold.Size = new System.Drawing.Size(79, 13);
            this.lblGyroThreshold.TabIndex = 19;
            this.lblGyroThreshold.Text = "Gyro Threshold";
            // 
            // lblAccelerationThreshold
            // 
            this.lblAccelerationThreshold.AutoSize = true;
            this.lblAccelerationThreshold.Location = new System.Drawing.Point(867, 52);
            this.lblAccelerationThreshold.Name = "lblAccelerationThreshold";
            this.lblAccelerationThreshold.Size = new System.Drawing.Size(116, 13);
            this.lblAccelerationThreshold.TabIndex = 20;
            this.lblAccelerationThreshold.Text = "Acceleration Threshold";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1265, 629);
            this.Controls.Add(this.lblAccelerationThreshold);
            this.Controls.Add(this.lblGyroThreshold);
            this.Controls.Add(this.tbxAccelerationThreshold);
            this.Controls.Add(this.tbxGyroThreshold);
            this.Controls.Add(this.btnRefreshComPort);
            this.Controls.Add(this.cbbComPort);
            this.Controls.Add(this.btnCalibrate);
            this.Controls.Add(this.lblForceR);
            this.Controls.Add(this.lblGyZ);
            this.Controls.Add(this.lblGyY);
            this.Controls.Add(this.lblGyX);
            this.Controls.Add(this.lblAccZ);
            this.Controls.Add(this.lblAccY);
            this.Controls.Add(this.lblAccX);
            this.Controls.Add(this.btnStartPause);
            this.Controls.Add(this.pnlChart);
            this.Controls.Add(this.btnDataChart);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.tbxRawData);
            this.Controls.Add(this.msMenu);
            this.MainMenuStrip = this.msMenu;
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.pnlChart.ResumeLayout(false);
            this.pnlChart.PerformLayout();
            this.gbGy.ResumeLayout(false);
            this.gbGy.PerformLayout();
            this.gbAcc.ResumeLayout(false);
            this.gbAcc.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tkbScale)).EndInit();
            this.msMenu.ResumeLayout(false);
            this.msMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.Button btnStartPause;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Panel pnlChart;
        private System.Windows.Forms.Button btnDataChart;
        private System.Windows.Forms.TextBox tbxRawData;
        private CheckBox chkFullScale;
        private Label lblScale;
        private Label lblScaleValue;
        private TrackBar tkbScale;
        private ToolTip toolTip1;
        private CheckBox chkGyZ;
        private CheckBox chkGyY;
        private CheckBox chkGyX;
        private CheckBox chkAccZ;
        private CheckBox chkAccY;
        private CheckBox chkAccX;
        private Button btnSelectAll;
        private Label lblAccX;
        private Label lblAccY;
        private Label lblAccZ;
        private Label lblGyX;
        private Label lblGyY;
        private Label lblGyZ;
        private CheckBox chkForceR;
        private Label lblForceR;
        private Button btnCalibrate;
        private MenuStrip msMenu;
        private ToolStripMenuItem tsmFile;
        private ToolStripMenuItem tsmFile_open;
        private ToolStripMenuItem tsmTools;
        private ToolStripMenuItem chk_tsmTools_CalibrateSignal;
        private ComboBox cbbComPort;
        private Button btnRefreshComPort;
        private Label lblForceRg;
        private CheckBox chkForceRg;
        private Label lblForceRt;
        private CheckBox chkForceRt;
        private GroupBox gbGy;
        private TextBox tbxGyRMeasure;
        private Label lblGyRMeasure;
        private TextBox tbxGyQBias;
        private Label lblGyQBias;
        private TextBox tbxGyQAngle;
        private Label lblGyQAngle;
        private GroupBox gbAcc;
        private TextBox tbxAccRMeasure;
        private Label lblAccRMeasure;
        private TextBox tbxAccQBias;
        private Label lblAccQBias;
        private TextBox tbxAccQAngle;
        private Label lblAccQAngle;
        private TextBox tbxGyroThreshold;
        private TextBox tbxAccelerationThreshold;
        private Label lblGyroThreshold;
        private Label lblAccelerationThreshold;
        private CheckBox chkGyroFwdCalibrated;
        private CheckBox chkAccFwdCalibrated;
        private CheckBox chkZeroCalibrated;
    }
}

