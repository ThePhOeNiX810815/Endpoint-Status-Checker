namespace EndpointChecker
{
    partial class SpeedTestDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpeedTestDialog));
            this.tb_SpeedTest_Latency = new System.Windows.Forms.TextBox();
            this.lbl_SpeedTest_Latency = new System.Windows.Forms.Label();
            this.tb_SpeedTest_Distance = new System.Windows.Forms.TextBox();
            this.lbl_SpeedTest_Distance = new System.Windows.Forms.Label();
            this.tb_SpeedTest_HostedBy = new System.Windows.Forms.TextBox();
            this.lbl_SpeedTest_HostedBy = new System.Windows.Forms.Label();
            this.tb_SpeedTest_TestServer = new System.Windows.Forms.TextBox();
            this.lbl_SpeedTest_TestServer = new System.Windows.Forms.Label();
            this.btn_SpeedTest_Refresh = new System.Windows.Forms.Button();
            this.pb_SpeedTestProgress = new System.Windows.Forms.PictureBox();
            this.lbl_SpeedTest_CurrentCountry = new System.Windows.Forms.Label();
            this.tb_SpeedTest_CurrentCountry = new System.Windows.Forms.TextBox();
            this.rtb_SpeedTest_LogConsole = new System.Windows.Forms.RichTextBox();
            this.lbl_SpeedTest_Mbps_Download_Label = new System.Windows.Forms.Label();
            this.lbl_SpeedTest_Mbps_Upload_Label = new System.Windows.Forms.Label();
            this.lbl_SpeedTest_Download_Label = new System.Windows.Forms.Label();
            this.lbl_SpeedTest_Upload_Label = new System.Windows.Forms.Label();
            this.aGauge_UploadSpeed = new EndpointChecker.AGauge();
            this.aGauge_DownloadSpeed = new EndpointChecker.AGauge();
            this.rb_AllServers = new System.Windows.Forms.RadioButton();
            this.rb_AllServersExceptCurrCountry = new System.Windows.Forms.RadioButton();
            this.rb_CurrentCountryServersOnly = new System.Windows.Forms.RadioButton();
            this.tb_SpeedTest_ExternalIP = new System.Windows.Forms.TextBox();
            this.lbl_SpeedTest_ExternalIP = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pb_SpeedTestProgress)).BeginInit();
            this.SuspendLayout();
            // 
            // tb_SpeedTest_Latency
            // 
            this.tb_SpeedTest_Latency.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tb_SpeedTest_Latency.BackColor = System.Drawing.Color.Silver;
            this.tb_SpeedTest_Latency.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tb_SpeedTest_Latency.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tb_SpeedTest_Latency.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tb_SpeedTest_Latency.Location = new System.Drawing.Point(151, 157);
            this.tb_SpeedTest_Latency.Multiline = true;
            this.tb_SpeedTest_Latency.Name = "tb_SpeedTest_Latency";
            this.tb_SpeedTest_Latency.ReadOnly = true;
            this.tb_SpeedTest_Latency.Size = new System.Drawing.Size(438, 23);
            this.tb_SpeedTest_Latency.TabIndex = 79;
            this.tb_SpeedTest_Latency.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbl_SpeedTest_Latency
            // 
            this.lbl_SpeedTest_Latency.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_SpeedTest_Latency.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SpeedTest_Latency.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_SpeedTest_Latency.Location = new System.Drawing.Point(11, 157);
            this.lbl_SpeedTest_Latency.Name = "lbl_SpeedTest_Latency";
            this.lbl_SpeedTest_Latency.Size = new System.Drawing.Size(134, 23);
            this.lbl_SpeedTest_Latency.TabIndex = 78;
            this.lbl_SpeedTest_Latency.Text = "Latency";
            this.lbl_SpeedTest_Latency.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tb_SpeedTest_Distance
            // 
            this.tb_SpeedTest_Distance.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tb_SpeedTest_Distance.BackColor = System.Drawing.Color.Silver;
            this.tb_SpeedTest_Distance.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tb_SpeedTest_Distance.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tb_SpeedTest_Distance.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tb_SpeedTest_Distance.Location = new System.Drawing.Point(151, 129);
            this.tb_SpeedTest_Distance.Multiline = true;
            this.tb_SpeedTest_Distance.Name = "tb_SpeedTest_Distance";
            this.tb_SpeedTest_Distance.ReadOnly = true;
            this.tb_SpeedTest_Distance.Size = new System.Drawing.Size(438, 23);
            this.tb_SpeedTest_Distance.TabIndex = 77;
            this.tb_SpeedTest_Distance.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbl_SpeedTest_Distance
            // 
            this.lbl_SpeedTest_Distance.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_SpeedTest_Distance.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SpeedTest_Distance.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_SpeedTest_Distance.Location = new System.Drawing.Point(11, 129);
            this.lbl_SpeedTest_Distance.Name = "lbl_SpeedTest_Distance";
            this.lbl_SpeedTest_Distance.Size = new System.Drawing.Size(134, 23);
            this.lbl_SpeedTest_Distance.TabIndex = 76;
            this.lbl_SpeedTest_Distance.Text = "Distance";
            this.lbl_SpeedTest_Distance.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tb_SpeedTest_HostedBy
            // 
            this.tb_SpeedTest_HostedBy.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tb_SpeedTest_HostedBy.BackColor = System.Drawing.Color.Silver;
            this.tb_SpeedTest_HostedBy.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tb_SpeedTest_HostedBy.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tb_SpeedTest_HostedBy.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tb_SpeedTest_HostedBy.Location = new System.Drawing.Point(151, 101);
            this.tb_SpeedTest_HostedBy.Multiline = true;
            this.tb_SpeedTest_HostedBy.Name = "tb_SpeedTest_HostedBy";
            this.tb_SpeedTest_HostedBy.ReadOnly = true;
            this.tb_SpeedTest_HostedBy.Size = new System.Drawing.Size(438, 23);
            this.tb_SpeedTest_HostedBy.TabIndex = 75;
            this.tb_SpeedTest_HostedBy.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbl_SpeedTest_HostedBy
            // 
            this.lbl_SpeedTest_HostedBy.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_SpeedTest_HostedBy.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SpeedTest_HostedBy.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_SpeedTest_HostedBy.Location = new System.Drawing.Point(11, 101);
            this.lbl_SpeedTest_HostedBy.Name = "lbl_SpeedTest_HostedBy";
            this.lbl_SpeedTest_HostedBy.Size = new System.Drawing.Size(134, 23);
            this.lbl_SpeedTest_HostedBy.TabIndex = 74;
            this.lbl_SpeedTest_HostedBy.Text = "Hosted By";
            this.lbl_SpeedTest_HostedBy.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tb_SpeedTest_TestServer
            // 
            this.tb_SpeedTest_TestServer.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tb_SpeedTest_TestServer.BackColor = System.Drawing.Color.Silver;
            this.tb_SpeedTest_TestServer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tb_SpeedTest_TestServer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tb_SpeedTest_TestServer.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tb_SpeedTest_TestServer.Location = new System.Drawing.Point(151, 73);
            this.tb_SpeedTest_TestServer.Multiline = true;
            this.tb_SpeedTest_TestServer.Name = "tb_SpeedTest_TestServer";
            this.tb_SpeedTest_TestServer.ReadOnly = true;
            this.tb_SpeedTest_TestServer.Size = new System.Drawing.Size(438, 23);
            this.tb_SpeedTest_TestServer.TabIndex = 71;
            this.tb_SpeedTest_TestServer.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbl_SpeedTest_TestServer
            // 
            this.lbl_SpeedTest_TestServer.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_SpeedTest_TestServer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SpeedTest_TestServer.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_SpeedTest_TestServer.Location = new System.Drawing.Point(11, 73);
            this.lbl_SpeedTest_TestServer.Name = "lbl_SpeedTest_TestServer";
            this.lbl_SpeedTest_TestServer.Size = new System.Drawing.Size(134, 23);
            this.lbl_SpeedTest_TestServer.TabIndex = 70;
            this.lbl_SpeedTest_TestServer.Text = "Test Server";
            this.lbl_SpeedTest_TestServer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btn_SpeedTest_Refresh
            // 
            this.btn_SpeedTest_Refresh.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btn_SpeedTest_Refresh.BackColor = System.Drawing.Color.DarkGray;
            this.btn_SpeedTest_Refresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_SpeedTest_Refresh.Location = new System.Drawing.Point(607, 16);
            this.btn_SpeedTest_Refresh.Name = "btn_SpeedTest_Refresh";
            this.btn_SpeedTest_Refresh.Size = new System.Drawing.Size(79, 23);
            this.btn_SpeedTest_Refresh.TabIndex = 69;
            this.btn_SpeedTest_Refresh.Text = "Refresh";
            this.btn_SpeedTest_Refresh.UseVisualStyleBackColor = false;
            this.btn_SpeedTest_Refresh.Click += new System.EventHandler(this.Btn_SpeedTest_Refresh_Click);
            // 
            // pb_SpeedTestProgress
            // 
            this.pb_SpeedTestProgress.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pb_SpeedTestProgress.BackColor = System.Drawing.Color.Transparent;
            this.pb_SpeedTestProgress.Image = global::EndpointChecker.Properties.Resources.loadingProgressWheel;
            this.pb_SpeedTestProgress.Location = new System.Drawing.Point(607, 44);
            this.pb_SpeedTestProgress.Name = "pb_SpeedTestProgress";
            this.pb_SpeedTestProgress.Size = new System.Drawing.Size(78, 79);
            this.pb_SpeedTestProgress.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_SpeedTestProgress.TabIndex = 68;
            this.pb_SpeedTestProgress.TabStop = false;
            this.pb_SpeedTestProgress.Visible = false;
            // 
            // lbl_SpeedTest_CurrentCountry
            // 
            this.lbl_SpeedTest_CurrentCountry.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_SpeedTest_CurrentCountry.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SpeedTest_CurrentCountry.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_SpeedTest_CurrentCountry.Location = new System.Drawing.Point(11, 45);
            this.lbl_SpeedTest_CurrentCountry.Name = "lbl_SpeedTest_CurrentCountry";
            this.lbl_SpeedTest_CurrentCountry.Size = new System.Drawing.Size(134, 23);
            this.lbl_SpeedTest_CurrentCountry.TabIndex = 88;
            this.lbl_SpeedTest_CurrentCountry.Text = "Current Country";
            this.lbl_SpeedTest_CurrentCountry.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tb_SpeedTest_CurrentCountry
            // 
            this.tb_SpeedTest_CurrentCountry.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tb_SpeedTest_CurrentCountry.BackColor = System.Drawing.Color.Silver;
            this.tb_SpeedTest_CurrentCountry.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tb_SpeedTest_CurrentCountry.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tb_SpeedTest_CurrentCountry.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tb_SpeedTest_CurrentCountry.Location = new System.Drawing.Point(151, 45);
            this.tb_SpeedTest_CurrentCountry.Multiline = true;
            this.tb_SpeedTest_CurrentCountry.Name = "tb_SpeedTest_CurrentCountry";
            this.tb_SpeedTest_CurrentCountry.ReadOnly = true;
            this.tb_SpeedTest_CurrentCountry.Size = new System.Drawing.Size(438, 23);
            this.tb_SpeedTest_CurrentCountry.TabIndex = 89;
            this.tb_SpeedTest_CurrentCountry.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // rtb_SpeedTest_LogConsole
            // 
            this.rtb_SpeedTest_LogConsole.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rtb_SpeedTest_LogConsole.BackColor = System.Drawing.Color.Silver;
            this.rtb_SpeedTest_LogConsole.DetectUrls = false;
            this.rtb_SpeedTest_LogConsole.Font = new System.Drawing.Font("Comic Sans MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.rtb_SpeedTest_LogConsole.ForeColor = System.Drawing.SystemColors.WindowText;
            this.rtb_SpeedTest_LogConsole.Location = new System.Drawing.Point(9, 224);
            this.rtb_SpeedTest_LogConsole.Name = "rtb_SpeedTest_LogConsole";
            this.rtb_SpeedTest_LogConsole.ReadOnly = true;
            this.rtb_SpeedTest_LogConsole.Size = new System.Drawing.Size(676, 226);
            this.rtb_SpeedTest_LogConsole.TabIndex = 73;
            this.rtb_SpeedTest_LogConsole.Text = "";
            // 
            // lbl_SpeedTest_Mbps_Download_Label
            // 
            this.lbl_SpeedTest_Mbps_Download_Label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_SpeedTest_Mbps_Download_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lbl_SpeedTest_Mbps_Download_Label.Location = new System.Drawing.Point(143, 389);
            this.lbl_SpeedTest_Mbps_Download_Label.Name = "lbl_SpeedTest_Mbps_Download_Label";
            this.lbl_SpeedTest_Mbps_Download_Label.Size = new System.Drawing.Size(127, 24);
            this.lbl_SpeedTest_Mbps_Download_Label.TabIndex = 81;
            this.lbl_SpeedTest_Mbps_Download_Label.Text = "Mbps";
            this.lbl_SpeedTest_Mbps_Download_Label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lbl_SpeedTest_Mbps_Download_Label.Visible = false;
            // 
            // lbl_SpeedTest_Mbps_Upload_Label
            // 
            this.lbl_SpeedTest_Mbps_Upload_Label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_SpeedTest_Mbps_Upload_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lbl_SpeedTest_Mbps_Upload_Label.Location = new System.Drawing.Point(554, 389);
            this.lbl_SpeedTest_Mbps_Upload_Label.Name = "lbl_SpeedTest_Mbps_Upload_Label";
            this.lbl_SpeedTest_Mbps_Upload_Label.Size = new System.Drawing.Size(127, 24);
            this.lbl_SpeedTest_Mbps_Upload_Label.TabIndex = 82;
            this.lbl_SpeedTest_Mbps_Upload_Label.Text = "Mbps";
            this.lbl_SpeedTest_Mbps_Upload_Label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lbl_SpeedTest_Mbps_Upload_Label.Visible = false;
            // 
            // lbl_SpeedTest_Download_Label
            // 
            this.lbl_SpeedTest_Download_Label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_SpeedTest_Download_Label.AutoSize = true;
            this.lbl_SpeedTest_Download_Label.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_SpeedTest_Download_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lbl_SpeedTest_Download_Label.Location = new System.Drawing.Point(217, 238);
            this.lbl_SpeedTest_Download_Label.Name = "lbl_SpeedTest_Download_Label";
            this.lbl_SpeedTest_Download_Label.Size = new System.Drawing.Size(97, 26);
            this.lbl_SpeedTest_Download_Label.TabIndex = 83;
            this.lbl_SpeedTest_Download_Label.Text = "Download";
            this.lbl_SpeedTest_Download_Label.Visible = false;
            // 
            // lbl_SpeedTest_Upload_Label
            // 
            this.lbl_SpeedTest_Upload_Label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_SpeedTest_Upload_Label.AutoSize = true;
            this.lbl_SpeedTest_Upload_Label.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_SpeedTest_Upload_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lbl_SpeedTest_Upload_Label.Location = new System.Drawing.Point(407, 238);
            this.lbl_SpeedTest_Upload_Label.Name = "lbl_SpeedTest_Upload_Label";
            this.lbl_SpeedTest_Upload_Label.Size = new System.Drawing.Size(72, 26);
            this.lbl_SpeedTest_Upload_Label.TabIndex = 84;
            this.lbl_SpeedTest_Upload_Label.Text = "Upload";
            this.lbl_SpeedTest_Upload_Label.Visible = false;
            // 
            // aGauge_UploadSpeed
            // 
            this.aGauge_UploadSpeed.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.aGauge_UploadSpeed.BaseArcColor = System.Drawing.Color.Gray;
            this.aGauge_UploadSpeed.BaseArcRadius = 80;
            this.aGauge_UploadSpeed.BaseArcStart = 145;
            this.aGauge_UploadSpeed.BaseArcSweep = 216;
            this.aGauge_UploadSpeed.BaseArcWidth = 2;
            this.aGauge_UploadSpeed.Cap_Idx = ((byte)(1));
            this.aGauge_UploadSpeed.CapColors = new System.Drawing.Color[] {
        System.Drawing.Color.Black,
        System.Drawing.Color.Black,
        System.Drawing.Color.Black,
        System.Drawing.Color.Black,
        System.Drawing.Color.Black};
            this.aGauge_UploadSpeed.CapPosition = new System.Drawing.Point(10, 10);
            this.aGauge_UploadSpeed.CapsPosition = new System.Drawing.Point[] {
        new System.Drawing.Point(10, 10),
        new System.Drawing.Point(10, 10),
        new System.Drawing.Point(10, 10),
        new System.Drawing.Point(10, 10),
        new System.Drawing.Point(10, 10)};
            this.aGauge_UploadSpeed.CapsText = new string[] {
        "",
        "",
        "",
        "",
        ""};
            this.aGauge_UploadSpeed.CapText = "";
            this.aGauge_UploadSpeed.Center = new System.Drawing.Point(100, 100);
            this.aGauge_UploadSpeed.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.aGauge_UploadSpeed.Location = new System.Drawing.Point(451, 257);
            this.aGauge_UploadSpeed.MaxValue = 150F;
            this.aGauge_UploadSpeed.MinValue = 0F;
            this.aGauge_UploadSpeed.Name = "aGauge_UploadSpeed";
            this.aGauge_UploadSpeed.NeedleColor1 = EndpointChecker.AGauge.NeedleColorEnum.Gray;
            this.aGauge_UploadSpeed.NeedleColor2 = System.Drawing.Color.DimGray;
            this.aGauge_UploadSpeed.NeedleRadius = 80;
            this.aGauge_UploadSpeed.NeedleType = 0;
            this.aGauge_UploadSpeed.NeedleWidth = 5;
            this.aGauge_UploadSpeed.Range_Idx = ((byte)(2));
            this.aGauge_UploadSpeed.RangeColor = System.Drawing.Color.Lime;
            this.aGauge_UploadSpeed.RangeEnabled = true;
            this.aGauge_UploadSpeed.RangeEndValue = 400F;
            this.aGauge_UploadSpeed.RangeInnerRadius = 70;
            this.aGauge_UploadSpeed.RangeOuterRadius = 80;
            this.aGauge_UploadSpeed.RangesColor = new System.Drawing.Color[] {
        System.Drawing.Color.Red,
        System.Drawing.Color.DodgerBlue,
        System.Drawing.Color.Lime,
        System.Drawing.SystemColors.Control,
        System.Drawing.SystemColors.Control};
            this.aGauge_UploadSpeed.RangesEnabled = new bool[] {
        true,
        true,
        true,
        false,
        false};
            this.aGauge_UploadSpeed.RangesEndValue = new float[] {
        100F,
        350F,
        400F,
        0F,
        0F};
            this.aGauge_UploadSpeed.RangesInnerRadius = new int[] {
        70,
        70,
        70,
        70,
        70};
            this.aGauge_UploadSpeed.RangesOuterRadius = new int[] {
        80,
        80,
        80,
        80,
        80};
            this.aGauge_UploadSpeed.RangesStartValue = new float[] {
        0F,
        300F,
        350F,
        0F,
        0F};
            this.aGauge_UploadSpeed.RangeStartValue = 350F;
            this.aGauge_UploadSpeed.ScaleLinesInterColor = System.Drawing.Color.Black;
            this.aGauge_UploadSpeed.ScaleLinesInterInnerRadius = 73;
            this.aGauge_UploadSpeed.ScaleLinesInterOuterRadius = 80;
            this.aGauge_UploadSpeed.ScaleLinesInterWidth = 1;
            this.aGauge_UploadSpeed.ScaleLinesMajorColor = System.Drawing.Color.Black;
            this.aGauge_UploadSpeed.ScaleLinesMajorInnerRadius = 70;
            this.aGauge_UploadSpeed.ScaleLinesMajorOuterRadius = 80;
            this.aGauge_UploadSpeed.ScaleLinesMajorStepValue = 10F;
            this.aGauge_UploadSpeed.ScaleLinesMajorWidth = 2;
            this.aGauge_UploadSpeed.ScaleLinesMinorColor = System.Drawing.Color.Gray;
            this.aGauge_UploadSpeed.ScaleLinesMinorInnerRadius = 75;
            this.aGauge_UploadSpeed.ScaleLinesMinorNumOf = 9;
            this.aGauge_UploadSpeed.ScaleLinesMinorOuterRadius = 80;
            this.aGauge_UploadSpeed.ScaleLinesMinorWidth = 1;
            this.aGauge_UploadSpeed.ScaleNumbersColor = System.Drawing.Color.Black;
            this.aGauge_UploadSpeed.ScaleNumbersFormat = null;
            this.aGauge_UploadSpeed.ScaleNumbersRadius = 95;
            this.aGauge_UploadSpeed.ScaleNumbersRotation = 0;
            this.aGauge_UploadSpeed.ScaleNumbersStartScaleLine = 0;
            this.aGauge_UploadSpeed.ScaleNumbersStepScaleLines = 1;
            this.aGauge_UploadSpeed.Size = new System.Drawing.Size(206, 188);
            this.aGauge_UploadSpeed.TabIndex = 80;
            this.aGauge_UploadSpeed.Value = 0F;
            this.aGauge_UploadSpeed.Visible = false;
            // 
            // aGauge_DownloadSpeed
            // 
            this.aGauge_DownloadSpeed.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.aGauge_DownloadSpeed.BaseArcColor = System.Drawing.Color.Gray;
            this.aGauge_DownloadSpeed.BaseArcRadius = 80;
            this.aGauge_DownloadSpeed.BaseArcStart = 145;
            this.aGauge_DownloadSpeed.BaseArcSweep = 216;
            this.aGauge_DownloadSpeed.BaseArcWidth = 2;
            this.aGauge_DownloadSpeed.Cap_Idx = ((byte)(1));
            this.aGauge_DownloadSpeed.CapColors = new System.Drawing.Color[] {
        System.Drawing.Color.Black,
        System.Drawing.Color.Black,
        System.Drawing.Color.Black,
        System.Drawing.Color.Black,
        System.Drawing.Color.Black};
            this.aGauge_DownloadSpeed.CapPosition = new System.Drawing.Point(10, 10);
            this.aGauge_DownloadSpeed.CapsPosition = new System.Drawing.Point[] {
        new System.Drawing.Point(10, 10),
        new System.Drawing.Point(10, 10),
        new System.Drawing.Point(10, 10),
        new System.Drawing.Point(10, 10),
        new System.Drawing.Point(10, 10)};
            this.aGauge_DownloadSpeed.CapsText = new string[] {
        "",
        "",
        "",
        "",
        ""};
            this.aGauge_DownloadSpeed.CapText = "";
            this.aGauge_DownloadSpeed.Center = new System.Drawing.Point(100, 100);
            this.aGauge_DownloadSpeed.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.aGauge_DownloadSpeed.Location = new System.Drawing.Point(40, 257);
            this.aGauge_DownloadSpeed.MaxValue = 150F;
            this.aGauge_DownloadSpeed.MinValue = 0F;
            this.aGauge_DownloadSpeed.Name = "aGauge_DownloadSpeed";
            this.aGauge_DownloadSpeed.NeedleColor1 = EndpointChecker.AGauge.NeedleColorEnum.Gray;
            this.aGauge_DownloadSpeed.NeedleColor2 = System.Drawing.Color.DimGray;
            this.aGauge_DownloadSpeed.NeedleRadius = 80;
            this.aGauge_DownloadSpeed.NeedleType = 0;
            this.aGauge_DownloadSpeed.NeedleWidth = 5;
            this.aGauge_DownloadSpeed.Range_Idx = ((byte)(2));
            this.aGauge_DownloadSpeed.RangeColor = System.Drawing.Color.Lime;
            this.aGauge_DownloadSpeed.RangeEnabled = true;
            this.aGauge_DownloadSpeed.RangeEndValue = 400F;
            this.aGauge_DownloadSpeed.RangeInnerRadius = 70;
            this.aGauge_DownloadSpeed.RangeOuterRadius = 80;
            this.aGauge_DownloadSpeed.RangesColor = new System.Drawing.Color[] {
        System.Drawing.Color.Red,
        System.Drawing.Color.DodgerBlue,
        System.Drawing.Color.Lime,
        System.Drawing.SystemColors.Control,
        System.Drawing.SystemColors.Control};
            this.aGauge_DownloadSpeed.RangesEnabled = new bool[] {
        true,
        true,
        true,
        false,
        false};
            this.aGauge_DownloadSpeed.RangesEndValue = new float[] {
        100F,
        350F,
        400F,
        0F,
        0F};
            this.aGauge_DownloadSpeed.RangesInnerRadius = new int[] {
        70,
        70,
        70,
        70,
        70};
            this.aGauge_DownloadSpeed.RangesOuterRadius = new int[] {
        80,
        80,
        80,
        80,
        80};
            this.aGauge_DownloadSpeed.RangesStartValue = new float[] {
        0F,
        300F,
        350F,
        0F,
        0F};
            this.aGauge_DownloadSpeed.RangeStartValue = 350F;
            this.aGauge_DownloadSpeed.ScaleLinesInterColor = System.Drawing.Color.Black;
            this.aGauge_DownloadSpeed.ScaleLinesInterInnerRadius = 73;
            this.aGauge_DownloadSpeed.ScaleLinesInterOuterRadius = 80;
            this.aGauge_DownloadSpeed.ScaleLinesInterWidth = 1;
            this.aGauge_DownloadSpeed.ScaleLinesMajorColor = System.Drawing.Color.Black;
            this.aGauge_DownloadSpeed.ScaleLinesMajorInnerRadius = 70;
            this.aGauge_DownloadSpeed.ScaleLinesMajorOuterRadius = 80;
            this.aGauge_DownloadSpeed.ScaleLinesMajorStepValue = 10F;
            this.aGauge_DownloadSpeed.ScaleLinesMajorWidth = 2;
            this.aGauge_DownloadSpeed.ScaleLinesMinorColor = System.Drawing.Color.Gray;
            this.aGauge_DownloadSpeed.ScaleLinesMinorInnerRadius = 75;
            this.aGauge_DownloadSpeed.ScaleLinesMinorNumOf = 9;
            this.aGauge_DownloadSpeed.ScaleLinesMinorOuterRadius = 80;
            this.aGauge_DownloadSpeed.ScaleLinesMinorWidth = 1;
            this.aGauge_DownloadSpeed.ScaleNumbersColor = System.Drawing.Color.Black;
            this.aGauge_DownloadSpeed.ScaleNumbersFormat = null;
            this.aGauge_DownloadSpeed.ScaleNumbersRadius = 95;
            this.aGauge_DownloadSpeed.ScaleNumbersRotation = 0;
            this.aGauge_DownloadSpeed.ScaleNumbersStartScaleLine = 0;
            this.aGauge_DownloadSpeed.ScaleNumbersStepScaleLines = 1;
            this.aGauge_DownloadSpeed.Size = new System.Drawing.Size(206, 188);
            this.aGauge_DownloadSpeed.TabIndex = 72;
            this.aGauge_DownloadSpeed.Text = "Mbps";
            this.aGauge_DownloadSpeed.Value = 0F;
            this.aGauge_DownloadSpeed.Visible = false;
            // 
            // rb_AllServers
            // 
            this.rb_AllServers.AutoSize = true;
            this.rb_AllServers.Checked = true;
            this.rb_AllServers.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rb_AllServers.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold);
            this.rb_AllServers.Location = new System.Drawing.Point(9, 192);
            this.rb_AllServers.Name = "rb_AllServers";
            this.rb_AllServers.Size = new System.Drawing.Size(188, 22);
            this.rb_AllServers.TabIndex = 90;
            this.rb_AllServers.TabStop = true;
            this.rb_AllServers.Text = "Servers from All Countries";
            this.rb_AllServers.UseVisualStyleBackColor = true;
            this.rb_AllServers.CheckedChanged += new System.EventHandler(this.rb_AllServers_CheckedChanged);
            // 
            // rb_AllServersExceptCurrCountry
            // 
            this.rb_AllServersExceptCurrCountry.AutoSize = true;
            this.rb_AllServersExceptCurrCountry.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rb_AllServersExceptCurrCountry.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold);
            this.rb_AllServersExceptCurrCountry.Location = new System.Drawing.Point(223, 192);
            this.rb_AllServersExceptCurrCountry.Name = "rb_AllServersExceptCurrCountry";
            this.rb_AllServersExceptCurrCountry.Size = new System.Drawing.Size(238, 22);
            this.rb_AllServersExceptCurrCountry.TabIndex = 91;
            this.rb_AllServersExceptCurrCountry.Text = "All Servers Except Current Country";
            this.rb_AllServersExceptCurrCountry.UseVisualStyleBackColor = true;
            this.rb_AllServersExceptCurrCountry.CheckedChanged += new System.EventHandler(this.rb_AllServersExceptCurrCountry_CheckedChanged);
            // 
            // rb_CurrentCountryServersOnly
            // 
            this.rb_CurrentCountryServersOnly.AutoSize = true;
            this.rb_CurrentCountryServersOnly.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rb_CurrentCountryServersOnly.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold);
            this.rb_CurrentCountryServersOnly.Location = new System.Drawing.Point(485, 192);
            this.rb_CurrentCountryServersOnly.Name = "rb_CurrentCountryServersOnly";
            this.rb_CurrentCountryServersOnly.Size = new System.Drawing.Size(206, 22);
            this.rb_CurrentCountryServersOnly.TabIndex = 92;
            this.rb_CurrentCountryServersOnly.Text = "Current Country Servers Only";
            this.rb_CurrentCountryServersOnly.UseVisualStyleBackColor = true;
            this.rb_CurrentCountryServersOnly.CheckedChanged += new System.EventHandler(this.rb_CurrentCountryServersOnly_CheckedChanged);
            // 
            // tb_SpeedTest_ExternalIP
            // 
            this.tb_SpeedTest_ExternalIP.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tb_SpeedTest_ExternalIP.BackColor = System.Drawing.Color.Silver;
            this.tb_SpeedTest_ExternalIP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tb_SpeedTest_ExternalIP.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tb_SpeedTest_ExternalIP.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tb_SpeedTest_ExternalIP.Location = new System.Drawing.Point(151, 17);
            this.tb_SpeedTest_ExternalIP.Multiline = true;
            this.tb_SpeedTest_ExternalIP.Name = "tb_SpeedTest_ExternalIP";
            this.tb_SpeedTest_ExternalIP.ReadOnly = true;
            this.tb_SpeedTest_ExternalIP.Size = new System.Drawing.Size(438, 23);
            this.tb_SpeedTest_ExternalIP.TabIndex = 94;
            this.tb_SpeedTest_ExternalIP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbl_SpeedTest_ExternalIP
            // 
            this.lbl_SpeedTest_ExternalIP.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_SpeedTest_ExternalIP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SpeedTest_ExternalIP.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_SpeedTest_ExternalIP.Location = new System.Drawing.Point(11, 17);
            this.lbl_SpeedTest_ExternalIP.Name = "lbl_SpeedTest_ExternalIP";
            this.lbl_SpeedTest_ExternalIP.Size = new System.Drawing.Size(134, 23);
            this.lbl_SpeedTest_ExternalIP.TabIndex = 93;
            this.lbl_SpeedTest_ExternalIP.Text = "External Public IP";
            this.lbl_SpeedTest_ExternalIP.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SpeedTestDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(696, 462);
            this.Controls.Add(this.tb_SpeedTest_ExternalIP);
            this.Controls.Add(this.lbl_SpeedTest_ExternalIP);
            this.Controls.Add(this.rb_CurrentCountryServersOnly);
            this.Controls.Add(this.rb_AllServersExceptCurrCountry);
            this.Controls.Add(this.rb_AllServers);
            this.Controls.Add(this.tb_SpeedTest_CurrentCountry);
            this.Controls.Add(this.lbl_SpeedTest_CurrentCountry);
            this.Controls.Add(this.lbl_SpeedTest_Upload_Label);
            this.Controls.Add(this.lbl_SpeedTest_Download_Label);
            this.Controls.Add(this.lbl_SpeedTest_Mbps_Upload_Label);
            this.Controls.Add(this.lbl_SpeedTest_Mbps_Download_Label);
            this.Controls.Add(this.tb_SpeedTest_Latency);
            this.Controls.Add(this.lbl_SpeedTest_Latency);
            this.Controls.Add(this.tb_SpeedTest_Distance);
            this.Controls.Add(this.lbl_SpeedTest_Distance);
            this.Controls.Add(this.tb_SpeedTest_HostedBy);
            this.Controls.Add(this.lbl_SpeedTest_HostedBy);
            this.Controls.Add(this.tb_SpeedTest_TestServer);
            this.Controls.Add(this.lbl_SpeedTest_TestServer);
            this.Controls.Add(this.btn_SpeedTest_Refresh);
            this.Controls.Add(this.pb_SpeedTestProgress);
            this.Controls.Add(this.rtb_SpeedTest_LogConsole);
            this.Controls.Add(this.aGauge_UploadSpeed);
            this.Controls.Add(this.aGauge_DownloadSpeed);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(716, 373);
            this.Name = "SpeedTestDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SpeedTest";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SpeedTestDialog_FormClosing);
            this.Shown += new System.EventHandler(this.SpeedTestDialog_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pb_SpeedTestProgress)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.TextBox tb_SpeedTest_Latency;
        public System.Windows.Forms.Label lbl_SpeedTest_Latency;
        public System.Windows.Forms.TextBox tb_SpeedTest_Distance;
        public System.Windows.Forms.Label lbl_SpeedTest_Distance;
        public System.Windows.Forms.TextBox tb_SpeedTest_HostedBy;
        public System.Windows.Forms.Label lbl_SpeedTest_HostedBy;
        public System.Windows.Forms.TextBox tb_SpeedTest_TestServer;
        public System.Windows.Forms.Label lbl_SpeedTest_TestServer;
        public System.Windows.Forms.Button btn_SpeedTest_Refresh;
        public System.Windows.Forms.PictureBox pb_SpeedTestProgress;
        public System.Windows.Forms.Label lbl_SpeedTest_CurrentCountry;
        public System.Windows.Forms.TextBox tb_SpeedTest_CurrentCountry;
        public AGauge aGauge_DownloadSpeed;
        public AGauge aGauge_UploadSpeed;
        public System.Windows.Forms.RichTextBox rtb_SpeedTest_LogConsole;
        public System.Windows.Forms.Label lbl_SpeedTest_Mbps_Download_Label;
        public System.Windows.Forms.Label lbl_SpeedTest_Mbps_Upload_Label;
        public System.Windows.Forms.Label lbl_SpeedTest_Download_Label;
        public System.Windows.Forms.Label lbl_SpeedTest_Upload_Label;
        public System.Windows.Forms.RadioButton rb_AllServers;
        public System.Windows.Forms.RadioButton rb_AllServersExceptCurrCountry;
        public System.Windows.Forms.RadioButton rb_CurrentCountryServersOnly;
        public System.Windows.Forms.TextBox tb_SpeedTest_ExternalIP;
        public System.Windows.Forms.Label lbl_SpeedTest_ExternalIP;
    }
}