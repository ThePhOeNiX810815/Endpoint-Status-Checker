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
            this.lbl_SpeedTest_Latency = new System.Windows.Forms.Label();
            this.lbl_SpeedTest_Distance = new System.Windows.Forms.Label();
            this.lbl_SpeedTest_HostedBy = new System.Windows.Forms.Label();
            this.lbl_SpeedTest_TestServer = new System.Windows.Forms.Label();
            this.pb_SpeedTestProgress = new System.Windows.Forms.PictureBox();
            this.lbl_SpeedTest_CurrentCountry = new System.Windows.Forms.Label();
            this.rtb_SpeedTest_LogConsole = new System.Windows.Forms.RichTextBox();
            this.lbl_SpeedTest_Mbps_Download_Label = new System.Windows.Forms.Label();
            this.lbl_SpeedTest_Mbps_Upload_Label = new System.Windows.Forms.Label();
            this.lbl_SpeedTest_Download_Label = new System.Windows.Forms.Label();
            this.lbl_SpeedTest_Upload_Label = new System.Windows.Forms.Label();
            this.lbl_SpeedTest_ExternalIP = new System.Windows.Forms.Label();
            this.aGauge_DownloadSpeed = new System.Windows.Forms.AGauge();
            this.aGauge_UploadSpeed = new System.Windows.Forms.AGauge();
            this.cb_SpeedTest_TestServer = new System.Windows.Forms.ComboBox();
            this.lbl_SpeedTest_ExternalIP_Value = new System.Windows.Forms.Label();
            this.lbl_SpeedTest_CurrentCountry_Value = new System.Windows.Forms.Label();
            this.lbl_SpeedTest_Latency_Value = new System.Windows.Forms.Label();
            this.lbl_SpeedTest_Distance_Value = new System.Windows.Forms.Label();
            this.lbl_SpeedTest_HostedBy_Value = new System.Windows.Forms.Label();
            this.btn_SpeedTest_GetServers = new System.Windows.Forms.Button();
            this.pb_GO = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_SpeedTest_ServerScope = new System.Windows.Forms.Label();
            this.lbl_SpeedTest_Calculation = new System.Windows.Forms.Label();
            this.cb_SpeedTest_ServerScope = new System.Windows.Forms.ComboBox();
            this.cb_SpeedTest_Calculation = new System.Windows.Forms.ComboBox();
            this.pBar_Upload = new EndpointChecker.ProgressBar_Red();
            this.pBar_Download = new EndpointChecker.ProgressBar_Green();
            ((System.ComponentModel.ISupportInitialize)(this.pb_SpeedTestProgress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_GO)).BeginInit();
            this.SuspendLayout();
            // 
            // lbl_SpeedTest_Latency
            // 
            this.lbl_SpeedTest_Latency.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_SpeedTest_Latency.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_SpeedTest_Latency.Location = new System.Drawing.Point(7, 184);
            this.lbl_SpeedTest_Latency.Name = "lbl_SpeedTest_Latency";
            this.lbl_SpeedTest_Latency.Size = new System.Drawing.Size(134, 25);
            this.lbl_SpeedTest_Latency.TabIndex = 78;
            this.lbl_SpeedTest_Latency.Text = "Latency";
            this.lbl_SpeedTest_Latency.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_SpeedTest_Distance
            // 
            this.lbl_SpeedTest_Distance.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_SpeedTest_Distance.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_SpeedTest_Distance.Location = new System.Drawing.Point(7, 155);
            this.lbl_SpeedTest_Distance.Name = "lbl_SpeedTest_Distance";
            this.lbl_SpeedTest_Distance.Size = new System.Drawing.Size(134, 25);
            this.lbl_SpeedTest_Distance.TabIndex = 76;
            this.lbl_SpeedTest_Distance.Text = "Distance";
            this.lbl_SpeedTest_Distance.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_SpeedTest_HostedBy
            // 
            this.lbl_SpeedTest_HostedBy.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_SpeedTest_HostedBy.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_SpeedTest_HostedBy.Location = new System.Drawing.Point(7, 126);
            this.lbl_SpeedTest_HostedBy.Name = "lbl_SpeedTest_HostedBy";
            this.lbl_SpeedTest_HostedBy.Size = new System.Drawing.Size(134, 25);
            this.lbl_SpeedTest_HostedBy.TabIndex = 74;
            this.lbl_SpeedTest_HostedBy.Text = "Hosted By";
            this.lbl_SpeedTest_HostedBy.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_SpeedTest_TestServer
            // 
            this.lbl_SpeedTest_TestServer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_SpeedTest_TestServer.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_SpeedTest_TestServer.Location = new System.Drawing.Point(7, 97);
            this.lbl_SpeedTest_TestServer.Name = "lbl_SpeedTest_TestServer";
            this.lbl_SpeedTest_TestServer.Size = new System.Drawing.Size(134, 25);
            this.lbl_SpeedTest_TestServer.TabIndex = 70;
            this.lbl_SpeedTest_TestServer.Text = "Test Server";
            this.lbl_SpeedTest_TestServer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pb_SpeedTestProgress
            // 
            this.pb_SpeedTestProgress.BackColor = System.Drawing.Color.Transparent;
            this.pb_SpeedTestProgress.Image = global::EndpointChecker.Properties.Resources.progressWheel_GreenDots;
            this.pb_SpeedTestProgress.Location = new System.Drawing.Point(284, 472);
            this.pb_SpeedTestProgress.Name = "pb_SpeedTestProgress";
            this.pb_SpeedTestProgress.Size = new System.Drawing.Size(120, 120);
            this.pb_SpeedTestProgress.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_SpeedTestProgress.TabIndex = 68;
            this.pb_SpeedTestProgress.TabStop = false;
            // 
            // lbl_SpeedTest_CurrentCountry
            // 
            this.lbl_SpeedTest_CurrentCountry.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_SpeedTest_CurrentCountry.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_SpeedTest_CurrentCountry.Location = new System.Drawing.Point(7, 68);
            this.lbl_SpeedTest_CurrentCountry.Name = "lbl_SpeedTest_CurrentCountry";
            this.lbl_SpeedTest_CurrentCountry.Size = new System.Drawing.Size(134, 25);
            this.lbl_SpeedTest_CurrentCountry.TabIndex = 88;
            this.lbl_SpeedTest_CurrentCountry.Text = "City / Country";
            this.lbl_SpeedTest_CurrentCountry.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // rtb_SpeedTest_LogConsole
            // 
            this.rtb_SpeedTest_LogConsole.BackColor = System.Drawing.Color.DimGray;
            this.rtb_SpeedTest_LogConsole.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rtb_SpeedTest_LogConsole.DetectUrls = false;
            this.rtb_SpeedTest_LogConsole.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtb_SpeedTest_LogConsole.ForeColor = System.Drawing.Color.Black;
            this.rtb_SpeedTest_LogConsole.Location = new System.Drawing.Point(7, 244);
            this.rtb_SpeedTest_LogConsole.Name = "rtb_SpeedTest_LogConsole";
            this.rtb_SpeedTest_LogConsole.ReadOnly = true;
            this.rtb_SpeedTest_LogConsole.Size = new System.Drawing.Size(676, 157);
            this.rtb_SpeedTest_LogConsole.TabIndex = 73;
            this.rtb_SpeedTest_LogConsole.Text = "";
            // 
            // lbl_SpeedTest_Mbps_Download_Label
            // 
            this.lbl_SpeedTest_Mbps_Download_Label.BackColor = System.Drawing.Color.Silver;
            this.lbl_SpeedTest_Mbps_Download_Label.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lbl_SpeedTest_Mbps_Download_Label.ForeColor = System.Drawing.Color.Black;
            this.lbl_SpeedTest_Mbps_Download_Label.Location = new System.Drawing.Point(33, 629);
            this.lbl_SpeedTest_Mbps_Download_Label.Name = "lbl_SpeedTest_Mbps_Download_Label";
            this.lbl_SpeedTest_Mbps_Download_Label.Size = new System.Drawing.Size(210, 28);
            this.lbl_SpeedTest_Mbps_Download_Label.TabIndex = 81;
            this.lbl_SpeedTest_Mbps_Download_Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_SpeedTest_Mbps_Upload_Label
            // 
            this.lbl_SpeedTest_Mbps_Upload_Label.BackColor = System.Drawing.Color.Silver;
            this.lbl_SpeedTest_Mbps_Upload_Label.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lbl_SpeedTest_Mbps_Upload_Label.ForeColor = System.Drawing.Color.Black;
            this.lbl_SpeedTest_Mbps_Upload_Label.Location = new System.Drawing.Point(444, 629);
            this.lbl_SpeedTest_Mbps_Upload_Label.Name = "lbl_SpeedTest_Mbps_Upload_Label";
            this.lbl_SpeedTest_Mbps_Upload_Label.Size = new System.Drawing.Size(210, 28);
            this.lbl_SpeedTest_Mbps_Upload_Label.TabIndex = 82;
            this.lbl_SpeedTest_Mbps_Upload_Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_SpeedTest_Download_Label
            // 
            this.lbl_SpeedTest_Download_Label.BackColor = System.Drawing.Color.Black;
            this.lbl_SpeedTest_Download_Label.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_SpeedTest_Download_Label.Font = new System.Drawing.Font("Segoe UI Semibold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lbl_SpeedTest_Download_Label.ForeColor = System.Drawing.Color.Silver;
            this.lbl_SpeedTest_Download_Label.Location = new System.Drawing.Point(37, 409);
            this.lbl_SpeedTest_Download_Label.Name = "lbl_SpeedTest_Download_Label";
            this.lbl_SpeedTest_Download_Label.Size = new System.Drawing.Size(206, 34);
            this.lbl_SpeedTest_Download_Label.TabIndex = 83;
            this.lbl_SpeedTest_Download_Label.Text = "Download Speed";
            this.lbl_SpeedTest_Download_Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_SpeedTest_Upload_Label
            // 
            this.lbl_SpeedTest_Upload_Label.BackColor = System.Drawing.Color.Black;
            this.lbl_SpeedTest_Upload_Label.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_SpeedTest_Upload_Label.Font = new System.Drawing.Font("Segoe UI Semibold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lbl_SpeedTest_Upload_Label.ForeColor = System.Drawing.Color.Silver;
            this.lbl_SpeedTest_Upload_Label.Location = new System.Drawing.Point(448, 409);
            this.lbl_SpeedTest_Upload_Label.Name = "lbl_SpeedTest_Upload_Label";
            this.lbl_SpeedTest_Upload_Label.Size = new System.Drawing.Size(206, 34);
            this.lbl_SpeedTest_Upload_Label.TabIndex = 84;
            this.lbl_SpeedTest_Upload_Label.Text = "Upload Speed";
            this.lbl_SpeedTest_Upload_Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_SpeedTest_ExternalIP
            // 
            this.lbl_SpeedTest_ExternalIP.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_SpeedTest_ExternalIP.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_SpeedTest_ExternalIP.Location = new System.Drawing.Point(7, 39);
            this.lbl_SpeedTest_ExternalIP.Name = "lbl_SpeedTest_ExternalIP";
            this.lbl_SpeedTest_ExternalIP.Size = new System.Drawing.Size(134, 25);
            this.lbl_SpeedTest_ExternalIP.TabIndex = 93;
            this.lbl_SpeedTest_ExternalIP.Text = "Public IP / ISP";
            this.lbl_SpeedTest_ExternalIP.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // aGauge_DownloadSpeed
            // 
            this.aGauge_DownloadSpeed.BaseArcColor = System.Drawing.Color.Gray;
            this.aGauge_DownloadSpeed.BaseArcRadius = 80;
            this.aGauge_DownloadSpeed.BaseArcStart = 135;
            this.aGauge_DownloadSpeed.BaseArcSweep = 270;
            this.aGauge_DownloadSpeed.BaseArcWidth = 2;
            this.aGauge_DownloadSpeed.Center = new System.Drawing.Point(100, 100);
            this.aGauge_DownloadSpeed.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.aGauge_DownloadSpeed.Location = new System.Drawing.Point(37, 445);
            this.aGauge_DownloadSpeed.MaxValue = 50F;
            this.aGauge_DownloadSpeed.MinValue = 0F;
            this.aGauge_DownloadSpeed.Name = "aGauge_DownloadSpeed";
            this.aGauge_DownloadSpeed.NeedleColor1 = System.Windows.Forms.AGaugeNeedleColor.Gray;
            this.aGauge_DownloadSpeed.NeedleColor2 = System.Drawing.Color.DimGray;
            this.aGauge_DownloadSpeed.NeedleRadius = 80;
            this.aGauge_DownloadSpeed.NeedleType = System.Windows.Forms.NeedleType.Advance;
            this.aGauge_DownloadSpeed.NeedleWidth = 4;
            this.aGauge_DownloadSpeed.ScaleLinesInterColor = System.Drawing.Color.Black;
            this.aGauge_DownloadSpeed.ScaleLinesInterInnerRadius = 73;
            this.aGauge_DownloadSpeed.ScaleLinesInterOuterRadius = 80;
            this.aGauge_DownloadSpeed.ScaleLinesInterWidth = 3;
            this.aGauge_DownloadSpeed.ScaleLinesMajorColor = System.Drawing.Color.Black;
            this.aGauge_DownloadSpeed.ScaleLinesMajorInnerRadius = 70;
            this.aGauge_DownloadSpeed.ScaleLinesMajorOuterRadius = 80;
            this.aGauge_DownloadSpeed.ScaleLinesMajorStepValue = 10F;
            this.aGauge_DownloadSpeed.ScaleLinesMajorWidth = 2;
            this.aGauge_DownloadSpeed.ScaleLinesMinorColor = System.Drawing.Color.Gray;
            this.aGauge_DownloadSpeed.ScaleLinesMinorInnerRadius = 75;
            this.aGauge_DownloadSpeed.ScaleLinesMinorOuterRadius = 80;
            this.aGauge_DownloadSpeed.ScaleLinesMinorTicks = 9;
            this.aGauge_DownloadSpeed.ScaleLinesMinorWidth = 1;
            this.aGauge_DownloadSpeed.ScaleNumbersColor = System.Drawing.Color.Black;
            this.aGauge_DownloadSpeed.ScaleNumbersFormat = null;
            this.aGauge_DownloadSpeed.ScaleNumbersRadius = 95;
            this.aGauge_DownloadSpeed.ScaleNumbersRotation = 0;
            this.aGauge_DownloadSpeed.ScaleNumbersStartScaleLine = 0;
            this.aGauge_DownloadSpeed.ScaleNumbersStepScaleLines = 1;
            this.aGauge_DownloadSpeed.Size = new System.Drawing.Size(206, 188);
            this.aGauge_DownloadSpeed.TabIndex = 95;
            this.aGauge_DownloadSpeed.Value = 0F;
            // 
            // aGauge_UploadSpeed
            // 
            this.aGauge_UploadSpeed.BaseArcColor = System.Drawing.Color.Gray;
            this.aGauge_UploadSpeed.BaseArcRadius = 80;
            this.aGauge_UploadSpeed.BaseArcStart = 135;
            this.aGauge_UploadSpeed.BaseArcSweep = 270;
            this.aGauge_UploadSpeed.BaseArcWidth = 2;
            this.aGauge_UploadSpeed.Center = new System.Drawing.Point(100, 100);
            this.aGauge_UploadSpeed.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.aGauge_UploadSpeed.Location = new System.Drawing.Point(448, 445);
            this.aGauge_UploadSpeed.MaxValue = 50F;
            this.aGauge_UploadSpeed.MinValue = 0F;
            this.aGauge_UploadSpeed.Name = "aGauge_UploadSpeed";
            this.aGauge_UploadSpeed.NeedleColor1 = System.Windows.Forms.AGaugeNeedleColor.Gray;
            this.aGauge_UploadSpeed.NeedleColor2 = System.Drawing.Color.DimGray;
            this.aGauge_UploadSpeed.NeedleRadius = 80;
            this.aGauge_UploadSpeed.NeedleType = System.Windows.Forms.NeedleType.Advance;
            this.aGauge_UploadSpeed.NeedleWidth = 4;
            this.aGauge_UploadSpeed.ScaleLinesInterColor = System.Drawing.Color.Black;
            this.aGauge_UploadSpeed.ScaleLinesInterInnerRadius = 73;
            this.aGauge_UploadSpeed.ScaleLinesInterOuterRadius = 80;
            this.aGauge_UploadSpeed.ScaleLinesInterWidth = 3;
            this.aGauge_UploadSpeed.ScaleLinesMajorColor = System.Drawing.Color.Black;
            this.aGauge_UploadSpeed.ScaleLinesMajorInnerRadius = 70;
            this.aGauge_UploadSpeed.ScaleLinesMajorOuterRadius = 80;
            this.aGauge_UploadSpeed.ScaleLinesMajorStepValue = 10F;
            this.aGauge_UploadSpeed.ScaleLinesMajorWidth = 2;
            this.aGauge_UploadSpeed.ScaleLinesMinorColor = System.Drawing.Color.Gray;
            this.aGauge_UploadSpeed.ScaleLinesMinorInnerRadius = 75;
            this.aGauge_UploadSpeed.ScaleLinesMinorOuterRadius = 80;
            this.aGauge_UploadSpeed.ScaleLinesMinorTicks = 9;
            this.aGauge_UploadSpeed.ScaleLinesMinorWidth = 1;
            this.aGauge_UploadSpeed.ScaleNumbersColor = System.Drawing.Color.Black;
            this.aGauge_UploadSpeed.ScaleNumbersFormat = null;
            this.aGauge_UploadSpeed.ScaleNumbersRadius = 95;
            this.aGauge_UploadSpeed.ScaleNumbersRotation = 0;
            this.aGauge_UploadSpeed.ScaleNumbersStartScaleLine = 0;
            this.aGauge_UploadSpeed.ScaleNumbersStepScaleLines = 1;
            this.aGauge_UploadSpeed.Size = new System.Drawing.Size(206, 188);
            this.aGauge_UploadSpeed.TabIndex = 96;
            this.aGauge_UploadSpeed.Value = 0F;
            // 
            // cb_SpeedTest_TestServer
            // 
            this.cb_SpeedTest_TestServer.BackColor = System.Drawing.Color.DimGray;
            this.cb_SpeedTest_TestServer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cb_SpeedTest_TestServer.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cb_SpeedTest_TestServer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_SpeedTest_TestServer.Enabled = false;
            this.cb_SpeedTest_TestServer.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_SpeedTest_TestServer.FormattingEnabled = true;
            this.cb_SpeedTest_TestServer.Location = new System.Drawing.Point(147, 97);
            this.cb_SpeedTest_TestServer.MaxDropDownItems = 100;
            this.cb_SpeedTest_TestServer.Name = "cb_SpeedTest_TestServer";
            this.cb_SpeedTest_TestServer.Size = new System.Drawing.Size(537, 26);
            this.cb_SpeedTest_TestServer.TabIndex = 97;
            this.cb_SpeedTest_TestServer.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.Custom_ComboBox_DrawItem);
            this.cb_SpeedTest_TestServer.SelectedIndexChanged += new System.EventHandler(this.cb_SpeedTest_TestServer_SelectedIndexChanged);
            // 
            // lbl_SpeedTest_ExternalIP_Value
            // 
            this.lbl_SpeedTest_ExternalIP_Value.BackColor = System.Drawing.Color.DimGray;
            this.lbl_SpeedTest_ExternalIP_Value.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_SpeedTest_ExternalIP_Value.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_SpeedTest_ExternalIP_Value.Location = new System.Drawing.Point(148, 39);
            this.lbl_SpeedTest_ExternalIP_Value.Name = "lbl_SpeedTest_ExternalIP_Value";
            this.lbl_SpeedTest_ExternalIP_Value.Size = new System.Drawing.Size(535, 25);
            this.lbl_SpeedTest_ExternalIP_Value.TabIndex = 103;
            this.lbl_SpeedTest_ExternalIP_Value.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_SpeedTest_CurrentCountry_Value
            // 
            this.lbl_SpeedTest_CurrentCountry_Value.BackColor = System.Drawing.Color.DimGray;
            this.lbl_SpeedTest_CurrentCountry_Value.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_SpeedTest_CurrentCountry_Value.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_SpeedTest_CurrentCountry_Value.Location = new System.Drawing.Point(148, 68);
            this.lbl_SpeedTest_CurrentCountry_Value.Name = "lbl_SpeedTest_CurrentCountry_Value";
            this.lbl_SpeedTest_CurrentCountry_Value.Size = new System.Drawing.Size(536, 25);
            this.lbl_SpeedTest_CurrentCountry_Value.TabIndex = 102;
            this.lbl_SpeedTest_CurrentCountry_Value.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_SpeedTest_Latency_Value
            // 
            this.lbl_SpeedTest_Latency_Value.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_SpeedTest_Latency_Value.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_SpeedTest_Latency_Value.Location = new System.Drawing.Point(148, 184);
            this.lbl_SpeedTest_Latency_Value.Name = "lbl_SpeedTest_Latency_Value";
            this.lbl_SpeedTest_Latency_Value.Size = new System.Drawing.Size(536, 25);
            this.lbl_SpeedTest_Latency_Value.TabIndex = 101;
            this.lbl_SpeedTest_Latency_Value.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_SpeedTest_Distance_Value
            // 
            this.lbl_SpeedTest_Distance_Value.BackColor = System.Drawing.Color.DimGray;
            this.lbl_SpeedTest_Distance_Value.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_SpeedTest_Distance_Value.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_SpeedTest_Distance_Value.Location = new System.Drawing.Point(148, 155);
            this.lbl_SpeedTest_Distance_Value.Name = "lbl_SpeedTest_Distance_Value";
            this.lbl_SpeedTest_Distance_Value.Size = new System.Drawing.Size(536, 25);
            this.lbl_SpeedTest_Distance_Value.TabIndex = 100;
            this.lbl_SpeedTest_Distance_Value.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_SpeedTest_HostedBy_Value
            // 
            this.lbl_SpeedTest_HostedBy_Value.BackColor = System.Drawing.Color.DimGray;
            this.lbl_SpeedTest_HostedBy_Value.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_SpeedTest_HostedBy_Value.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_SpeedTest_HostedBy_Value.Location = new System.Drawing.Point(148, 126);
            this.lbl_SpeedTest_HostedBy_Value.Name = "lbl_SpeedTest_HostedBy_Value";
            this.lbl_SpeedTest_HostedBy_Value.Size = new System.Drawing.Size(536, 25);
            this.lbl_SpeedTest_HostedBy_Value.TabIndex = 99;
            this.lbl_SpeedTest_HostedBy_Value.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btn_SpeedTest_GetServers
            // 
            this.btn_SpeedTest_GetServers.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn_SpeedTest_GetServers.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_SpeedTest_GetServers.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_SpeedTest_GetServers.Font = new System.Drawing.Font("Segoe UI Semibold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_SpeedTest_GetServers.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.btn_SpeedTest_GetServers.Location = new System.Drawing.Point(265, 409);
            this.btn_SpeedTest_GetServers.Name = "btn_SpeedTest_GetServers";
            this.btn_SpeedTest_GetServers.Size = new System.Drawing.Size(161, 34);
            this.btn_SpeedTest_GetServers.TabIndex = 104;
            this.btn_SpeedTest_GetServers.Text = "GET SERVERS";
            this.btn_SpeedTest_GetServers.UseVisualStyleBackColor = false;
            this.btn_SpeedTest_GetServers.Visible = false;
            this.btn_SpeedTest_GetServers.Click += new System.EventHandler(this.btn_SpeedTest_GetServers_Click);
            // 
            // pb_GO
            // 
            this.pb_GO.BackColor = System.Drawing.Color.Transparent;
            this.pb_GO.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pb_GO.Image = global::EndpointChecker.Properties.Resources.goLogo;
            this.pb_GO.Location = new System.Drawing.Point(312, 500);
            this.pb_GO.Name = "pb_GO";
            this.pb_GO.Size = new System.Drawing.Size(64, 64);
            this.pb_GO.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_GO.TabIndex = 105;
            this.pb_GO.TabStop = false;
            this.pb_GO.Visible = false;
            this.pb_GO.Click += new System.EventHandler(this.pb_GO_Click);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.LightSkyBlue;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(7, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(676, 30);
            this.label1.TabIndex = 108;
            this.label1.Text = "This feature is using OOKLA\'s SpeedTest API, therefore the base measurement funci" +
    "onality depends on it";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_SpeedTest_ServerScope
            // 
            this.lbl_SpeedTest_ServerScope.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_SpeedTest_ServerScope.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_SpeedTest_ServerScope.Location = new System.Drawing.Point(7, 213);
            this.lbl_SpeedTest_ServerScope.Name = "lbl_SpeedTest_ServerScope";
            this.lbl_SpeedTest_ServerScope.Size = new System.Drawing.Size(134, 25);
            this.lbl_SpeedTest_ServerScope.TabIndex = 110;
            this.lbl_SpeedTest_ServerScope.Text = "Server Scope";
            this.lbl_SpeedTest_ServerScope.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_SpeedTest_Calculation
            // 
            this.lbl_SpeedTest_Calculation.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_SpeedTest_Calculation.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_SpeedTest_Calculation.Location = new System.Drawing.Point(388, 213);
            this.lbl_SpeedTest_Calculation.Name = "lbl_SpeedTest_Calculation";
            this.lbl_SpeedTest_Calculation.Size = new System.Drawing.Size(134, 25);
            this.lbl_SpeedTest_Calculation.TabIndex = 112;
            this.lbl_SpeedTest_Calculation.Text = "Values Calculation";
            this.lbl_SpeedTest_Calculation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cb_SpeedTest_ServerScope
            // 
            this.cb_SpeedTest_ServerScope.BackColor = System.Drawing.Color.DimGray;
            this.cb_SpeedTest_ServerScope.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cb_SpeedTest_ServerScope.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cb_SpeedTest_ServerScope.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_SpeedTest_ServerScope.Enabled = false;
            this.cb_SpeedTest_ServerScope.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_SpeedTest_ServerScope.FormattingEnabled = true;
            this.cb_SpeedTest_ServerScope.Location = new System.Drawing.Point(147, 213);
            this.cb_SpeedTest_ServerScope.MaxDropDownItems = 100;
            this.cb_SpeedTest_ServerScope.Name = "cb_SpeedTest_ServerScope";
            this.cb_SpeedTest_ServerScope.Size = new System.Drawing.Size(206, 26);
            this.cb_SpeedTest_ServerScope.TabIndex = 113;
            this.cb_SpeedTest_ServerScope.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.Custom_ComboBox_DrawItem);
            this.cb_SpeedTest_ServerScope.SelectedIndexChanged += new System.EventHandler(this.cb_SpeedTest_ServerScope_SelectedIndexChanged);
            // 
            // cb_SpeedTest_Calculation
            // 
            this.cb_SpeedTest_Calculation.BackColor = System.Drawing.Color.DimGray;
            this.cb_SpeedTest_Calculation.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cb_SpeedTest_Calculation.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cb_SpeedTest_Calculation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_SpeedTest_Calculation.Enabled = false;
            this.cb_SpeedTest_Calculation.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_SpeedTest_Calculation.FormattingEnabled = true;
            this.cb_SpeedTest_Calculation.Location = new System.Drawing.Point(528, 213);
            this.cb_SpeedTest_Calculation.MaxDropDownItems = 100;
            this.cb_SpeedTest_Calculation.Name = "cb_SpeedTest_Calculation";
            this.cb_SpeedTest_Calculation.Size = new System.Drawing.Size(156, 26);
            this.cb_SpeedTest_Calculation.TabIndex = 114;
            this.cb_SpeedTest_Calculation.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.Custom_ComboBox_DrawItem);
            this.cb_SpeedTest_Calculation.SelectedIndexChanged += new System.EventHandler(this.cb_SpeedTest_Calculation_SelectedIndexChanged);
            // 
            // pBar_Upload
            // 
            this.pBar_Upload.Location = new System.Drawing.Point(444, 629);
            this.pBar_Upload.MarqueeAnimationSpeed = 20;
            this.pBar_Upload.Name = "pBar_Upload";
            this.pBar_Upload.Size = new System.Drawing.Size(210, 28);
            this.pBar_Upload.Step = 1;
            this.pBar_Upload.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pBar_Upload.TabIndex = 107;
            this.pBar_Upload.Visible = false;
            // 
            // pBar_Download
            // 
            this.pBar_Download.Location = new System.Drawing.Point(33, 629);
            this.pBar_Download.MarqueeAnimationSpeed = 20;
            this.pBar_Download.Name = "pBar_Download";
            this.pBar_Download.Size = new System.Drawing.Size(210, 28);
            this.pBar_Download.Step = 1;
            this.pBar_Download.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pBar_Download.TabIndex = 106;
            this.pBar_Download.Visible = false;
            // 
            // SpeedTestDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.ClientSize = new System.Drawing.Size(691, 662);
            this.Controls.Add(this.cb_SpeedTest_Calculation);
            this.Controls.Add(this.cb_SpeedTest_ServerScope);
            this.Controls.Add(this.lbl_SpeedTest_Calculation);
            this.Controls.Add(this.lbl_SpeedTest_ServerScope);
            this.Controls.Add(this.pBar_Upload);
            this.Controls.Add(this.pBar_Download);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pb_GO);
            this.Controls.Add(this.btn_SpeedTest_GetServers);
            this.Controls.Add(this.lbl_SpeedTest_ExternalIP_Value);
            this.Controls.Add(this.lbl_SpeedTest_CurrentCountry_Value);
            this.Controls.Add(this.lbl_SpeedTest_Latency_Value);
            this.Controls.Add(this.lbl_SpeedTest_Distance_Value);
            this.Controls.Add(this.lbl_SpeedTest_HostedBy_Value);
            this.Controls.Add(this.cb_SpeedTest_TestServer);
            this.Controls.Add(this.lbl_SpeedTest_Download_Label);
            this.Controls.Add(this.lbl_SpeedTest_Upload_Label);
            this.Controls.Add(this.lbl_SpeedTest_ExternalIP);
            this.Controls.Add(this.lbl_SpeedTest_CurrentCountry);
            this.Controls.Add(this.lbl_SpeedTest_Latency);
            this.Controls.Add(this.lbl_SpeedTest_Distance);
            this.Controls.Add(this.lbl_SpeedTest_HostedBy);
            this.Controls.Add(this.lbl_SpeedTest_TestServer);
            this.Controls.Add(this.pb_SpeedTestProgress);
            this.Controls.Add(this.rtb_SpeedTest_LogConsole);
            this.Controls.Add(this.lbl_SpeedTest_Mbps_Download_Label);
            this.Controls.Add(this.lbl_SpeedTest_Mbps_Upload_Label);
            this.Controls.Add(this.aGauge_UploadSpeed);
            this.Controls.Add(this.aGauge_DownloadSpeed);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(711, 705);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(711, 705);
            this.Name = "SpeedTestDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SpeedTest";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SpeedTestDialog_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pb_SpeedTestProgress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_GO)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.Label lbl_SpeedTest_Latency;
        public System.Windows.Forms.Label lbl_SpeedTest_Distance;
        public System.Windows.Forms.Label lbl_SpeedTest_HostedBy;
        public System.Windows.Forms.Label lbl_SpeedTest_TestServer;
        public System.Windows.Forms.PictureBox pb_SpeedTestProgress;
        public System.Windows.Forms.Label lbl_SpeedTest_CurrentCountry;
        public System.Windows.Forms.RichTextBox rtb_SpeedTest_LogConsole;
        public System.Windows.Forms.Label lbl_SpeedTest_Mbps_Download_Label;
        public System.Windows.Forms.Label lbl_SpeedTest_Mbps_Upload_Label;
        public System.Windows.Forms.Label lbl_SpeedTest_Download_Label;
        public System.Windows.Forms.Label lbl_SpeedTest_Upload_Label;
        public System.Windows.Forms.Label lbl_SpeedTest_ExternalIP;
        public System.Windows.Forms.AGauge aGauge_DownloadSpeed;
        public System.Windows.Forms.AGauge aGauge_UploadSpeed;
        public System.Windows.Forms.ComboBox cb_SpeedTest_TestServer;
        public System.Windows.Forms.Label lbl_SpeedTest_ExternalIP_Value;
        public System.Windows.Forms.Label lbl_SpeedTest_CurrentCountry_Value;
        public System.Windows.Forms.Label lbl_SpeedTest_Latency_Value;
        public System.Windows.Forms.Label lbl_SpeedTest_Distance_Value;
        public System.Windows.Forms.Label lbl_SpeedTest_HostedBy_Value;
        public System.Windows.Forms.Button btn_SpeedTest_GetServers;
        public System.Windows.Forms.PictureBox pb_GO;
        public ProgressBar_Green pBar_Download;
        public ProgressBar_Red pBar_Upload;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label lbl_SpeedTest_ServerScope;
        public System.Windows.Forms.Label lbl_SpeedTest_Calculation;
        public System.Windows.Forms.ComboBox cb_SpeedTest_ServerScope;
        public System.Windows.Forms.ComboBox cb_SpeedTest_Calculation;
    }
}