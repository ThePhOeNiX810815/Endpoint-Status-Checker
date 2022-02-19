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
            this.rb_AllServers = new System.Windows.Forms.RadioButton();
            this.rb_AllServersExceptCurrCountry = new System.Windows.Forms.RadioButton();
            this.rb_CurrentCountryServersOnly = new System.Windows.Forms.RadioButton();
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
            this.pBar_Upload = new EndpointChecker.ProgressBar_Red();
            this.pBar_Download = new EndpointChecker.ProgressBar_Green();
            ((System.ComponentModel.ISupportInitialize)(this.pb_SpeedTestProgress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_GO)).BeginInit();
            this.SuspendLayout();
            // 
            // lbl_SpeedTest_Latency
            // 
            this.lbl_SpeedTest_Latency.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_SpeedTest_Latency.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_SpeedTest_Latency.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_SpeedTest_Latency.Location = new System.Drawing.Point(7, 151);
            this.lbl_SpeedTest_Latency.Name = "lbl_SpeedTest_Latency";
            this.lbl_SpeedTest_Latency.Size = new System.Drawing.Size(134, 25);
            this.lbl_SpeedTest_Latency.TabIndex = 78;
            this.lbl_SpeedTest_Latency.Text = "Latency";
            this.lbl_SpeedTest_Latency.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_SpeedTest_Distance
            // 
            this.lbl_SpeedTest_Distance.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_SpeedTest_Distance.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_SpeedTest_Distance.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_SpeedTest_Distance.Location = new System.Drawing.Point(7, 123);
            this.lbl_SpeedTest_Distance.Name = "lbl_SpeedTest_Distance";
            this.lbl_SpeedTest_Distance.Size = new System.Drawing.Size(134, 25);
            this.lbl_SpeedTest_Distance.TabIndex = 76;
            this.lbl_SpeedTest_Distance.Text = "Distance";
            this.lbl_SpeedTest_Distance.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_SpeedTest_HostedBy
            // 
            this.lbl_SpeedTest_HostedBy.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_SpeedTest_HostedBy.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_SpeedTest_HostedBy.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_SpeedTest_HostedBy.Location = new System.Drawing.Point(7, 94);
            this.lbl_SpeedTest_HostedBy.Name = "lbl_SpeedTest_HostedBy";
            this.lbl_SpeedTest_HostedBy.Size = new System.Drawing.Size(134, 25);
            this.lbl_SpeedTest_HostedBy.TabIndex = 74;
            this.lbl_SpeedTest_HostedBy.Text = "Hosted By";
            this.lbl_SpeedTest_HostedBy.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_SpeedTest_TestServer
            // 
            this.lbl_SpeedTest_TestServer.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_SpeedTest_TestServer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_SpeedTest_TestServer.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_SpeedTest_TestServer.Location = new System.Drawing.Point(7, 65);
            this.lbl_SpeedTest_TestServer.Name = "lbl_SpeedTest_TestServer";
            this.lbl_SpeedTest_TestServer.Size = new System.Drawing.Size(134, 25);
            this.lbl_SpeedTest_TestServer.TabIndex = 70;
            this.lbl_SpeedTest_TestServer.Text = "Test Server";
            this.lbl_SpeedTest_TestServer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pb_SpeedTestProgress
            // 
            this.pb_SpeedTestProgress.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pb_SpeedTestProgress.BackColor = System.Drawing.Color.Transparent;
            this.pb_SpeedTestProgress.Image = global::EndpointChecker.Properties.Resources.progressWheel_GreenDots;
            this.pb_SpeedTestProgress.Location = new System.Drawing.Point(284, 441);
            this.pb_SpeedTestProgress.Name = "pb_SpeedTestProgress";
            this.pb_SpeedTestProgress.Size = new System.Drawing.Size(120, 120);
            this.pb_SpeedTestProgress.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_SpeedTestProgress.TabIndex = 68;
            this.pb_SpeedTestProgress.TabStop = false;
            // 
            // lbl_SpeedTest_CurrentCountry
            // 
            this.lbl_SpeedTest_CurrentCountry.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_SpeedTest_CurrentCountry.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_SpeedTest_CurrentCountry.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_SpeedTest_CurrentCountry.Location = new System.Drawing.Point(7, 36);
            this.lbl_SpeedTest_CurrentCountry.Name = "lbl_SpeedTest_CurrentCountry";
            this.lbl_SpeedTest_CurrentCountry.Size = new System.Drawing.Size(134, 25);
            this.lbl_SpeedTest_CurrentCountry.TabIndex = 88;
            this.lbl_SpeedTest_CurrentCountry.Text = "City / Country";
            this.lbl_SpeedTest_CurrentCountry.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // rtb_SpeedTest_LogConsole
            // 
            this.rtb_SpeedTest_LogConsole.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rtb_SpeedTest_LogConsole.BackColor = System.Drawing.Color.DimGray;
            this.rtb_SpeedTest_LogConsole.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rtb_SpeedTest_LogConsole.DetectUrls = false;
            this.rtb_SpeedTest_LogConsole.Font = new System.Drawing.Font("Comic Sans MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtb_SpeedTest_LogConsole.ForeColor = System.Drawing.Color.Black;
            this.rtb_SpeedTest_LogConsole.Location = new System.Drawing.Point(7, 209);
            this.rtb_SpeedTest_LogConsole.Name = "rtb_SpeedTest_LogConsole";
            this.rtb_SpeedTest_LogConsole.ReadOnly = true;
            this.rtb_SpeedTest_LogConsole.Size = new System.Drawing.Size(676, 161);
            this.rtb_SpeedTest_LogConsole.TabIndex = 73;
            this.rtb_SpeedTest_LogConsole.Text = "";
            // 
            // lbl_SpeedTest_Mbps_Download_Label
            // 
            this.lbl_SpeedTest_Mbps_Download_Label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_SpeedTest_Mbps_Download_Label.BackColor = System.Drawing.Color.Silver;
            this.lbl_SpeedTest_Mbps_Download_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lbl_SpeedTest_Mbps_Download_Label.ForeColor = System.Drawing.Color.Black;
            this.lbl_SpeedTest_Mbps_Download_Label.Location = new System.Drawing.Point(33, 598);
            this.lbl_SpeedTest_Mbps_Download_Label.Name = "lbl_SpeedTest_Mbps_Download_Label";
            this.lbl_SpeedTest_Mbps_Download_Label.Size = new System.Drawing.Size(210, 28);
            this.lbl_SpeedTest_Mbps_Download_Label.TabIndex = 81;
            this.lbl_SpeedTest_Mbps_Download_Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_SpeedTest_Mbps_Upload_Label
            // 
            this.lbl_SpeedTest_Mbps_Upload_Label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_SpeedTest_Mbps_Upload_Label.BackColor = System.Drawing.Color.Silver;
            this.lbl_SpeedTest_Mbps_Upload_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lbl_SpeedTest_Mbps_Upload_Label.ForeColor = System.Drawing.Color.Black;
            this.lbl_SpeedTest_Mbps_Upload_Label.Location = new System.Drawing.Point(444, 598);
            this.lbl_SpeedTest_Mbps_Upload_Label.Name = "lbl_SpeedTest_Mbps_Upload_Label";
            this.lbl_SpeedTest_Mbps_Upload_Label.Size = new System.Drawing.Size(210, 28);
            this.lbl_SpeedTest_Mbps_Upload_Label.TabIndex = 82;
            this.lbl_SpeedTest_Mbps_Upload_Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_SpeedTest_Download_Label
            // 
            this.lbl_SpeedTest_Download_Label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_SpeedTest_Download_Label.BackColor = System.Drawing.Color.Black;
            this.lbl_SpeedTest_Download_Label.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_SpeedTest_Download_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lbl_SpeedTest_Download_Label.ForeColor = System.Drawing.Color.Silver;
            this.lbl_SpeedTest_Download_Label.Location = new System.Drawing.Point(37, 378);
            this.lbl_SpeedTest_Download_Label.Name = "lbl_SpeedTest_Download_Label";
            this.lbl_SpeedTest_Download_Label.Size = new System.Drawing.Size(206, 26);
            this.lbl_SpeedTest_Download_Label.TabIndex = 83;
            this.lbl_SpeedTest_Download_Label.Text = "Download Speed";
            this.lbl_SpeedTest_Download_Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_SpeedTest_Upload_Label
            // 
            this.lbl_SpeedTest_Upload_Label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_SpeedTest_Upload_Label.BackColor = System.Drawing.Color.Black;
            this.lbl_SpeedTest_Upload_Label.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_SpeedTest_Upload_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lbl_SpeedTest_Upload_Label.ForeColor = System.Drawing.Color.Silver;
            this.lbl_SpeedTest_Upload_Label.Location = new System.Drawing.Point(448, 378);
            this.lbl_SpeedTest_Upload_Label.Name = "lbl_SpeedTest_Upload_Label";
            this.lbl_SpeedTest_Upload_Label.Size = new System.Drawing.Size(206, 26);
            this.lbl_SpeedTest_Upload_Label.TabIndex = 84;
            this.lbl_SpeedTest_Upload_Label.Text = "Upload Speed";
            this.lbl_SpeedTest_Upload_Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rb_AllServers
            // 
            this.rb_AllServers.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rb_AllServers.AutoSize = true;
            this.rb_AllServers.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rb_AllServers.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold);
            this.rb_AllServers.Location = new System.Drawing.Point(7, 183);
            this.rb_AllServers.Name = "rb_AllServers";
            this.rb_AllServers.Size = new System.Drawing.Size(188, 22);
            this.rb_AllServers.TabIndex = 90;
            this.rb_AllServers.Text = "Servers from All Countries";
            this.rb_AllServers.UseVisualStyleBackColor = true;
            this.rb_AllServers.CheckedChanged += new System.EventHandler(this.rb_AllServers_CheckedChanged);
            // 
            // rb_AllServersExceptCurrCountry
            // 
            this.rb_AllServersExceptCurrCountry.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rb_AllServersExceptCurrCountry.AutoSize = true;
            this.rb_AllServersExceptCurrCountry.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rb_AllServersExceptCurrCountry.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold);
            this.rb_AllServersExceptCurrCountry.Location = new System.Drawing.Point(223, 183);
            this.rb_AllServersExceptCurrCountry.Name = "rb_AllServersExceptCurrCountry";
            this.rb_AllServersExceptCurrCountry.Size = new System.Drawing.Size(238, 22);
            this.rb_AllServersExceptCurrCountry.TabIndex = 91;
            this.rb_AllServersExceptCurrCountry.Text = "All Servers Except Current Country";
            this.rb_AllServersExceptCurrCountry.UseVisualStyleBackColor = true;
            this.rb_AllServersExceptCurrCountry.CheckedChanged += new System.EventHandler(this.rb_AllServersExceptCurrCountry_CheckedChanged);
            // 
            // rb_CurrentCountryServersOnly
            // 
            this.rb_CurrentCountryServersOnly.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rb_CurrentCountryServersOnly.AutoSize = true;
            this.rb_CurrentCountryServersOnly.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rb_CurrentCountryServersOnly.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold);
            this.rb_CurrentCountryServersOnly.Location = new System.Drawing.Point(478, 183);
            this.rb_CurrentCountryServersOnly.Name = "rb_CurrentCountryServersOnly";
            this.rb_CurrentCountryServersOnly.Size = new System.Drawing.Size(206, 22);
            this.rb_CurrentCountryServersOnly.TabIndex = 92;
            this.rb_CurrentCountryServersOnly.Text = "Current Country Servers Only";
            this.rb_CurrentCountryServersOnly.UseVisualStyleBackColor = true;
            this.rb_CurrentCountryServersOnly.CheckedChanged += new System.EventHandler(this.rb_CurrentCountryServersOnly_CheckedChanged);
            // 
            // lbl_SpeedTest_ExternalIP
            // 
            this.lbl_SpeedTest_ExternalIP.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_SpeedTest_ExternalIP.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_SpeedTest_ExternalIP.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_SpeedTest_ExternalIP.Location = new System.Drawing.Point(7, 7);
            this.lbl_SpeedTest_ExternalIP.Name = "lbl_SpeedTest_ExternalIP";
            this.lbl_SpeedTest_ExternalIP.Size = new System.Drawing.Size(134, 25);
            this.lbl_SpeedTest_ExternalIP.TabIndex = 93;
            this.lbl_SpeedTest_ExternalIP.Text = "Public IP / ISP";
            this.lbl_SpeedTest_ExternalIP.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // aGauge_DownloadSpeed
            // 
            this.aGauge_DownloadSpeed.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.aGauge_DownloadSpeed.BaseArcColor = System.Drawing.Color.Gray;
            this.aGauge_DownloadSpeed.BaseArcRadius = 80;
            this.aGauge_DownloadSpeed.BaseArcStart = 135;
            this.aGauge_DownloadSpeed.BaseArcSweep = 270;
            this.aGauge_DownloadSpeed.BaseArcWidth = 2;
            this.aGauge_DownloadSpeed.Center = new System.Drawing.Point(100, 100);
            this.aGauge_DownloadSpeed.Location = new System.Drawing.Point(37, 414);
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
            this.aGauge_UploadSpeed.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.aGauge_UploadSpeed.BaseArcColor = System.Drawing.Color.Gray;
            this.aGauge_UploadSpeed.BaseArcRadius = 80;
            this.aGauge_UploadSpeed.BaseArcStart = 135;
            this.aGauge_UploadSpeed.BaseArcSweep = 270;
            this.aGauge_UploadSpeed.BaseArcWidth = 2;
            this.aGauge_UploadSpeed.Center = new System.Drawing.Point(100, 100);
            this.aGauge_UploadSpeed.Location = new System.Drawing.Point(448, 414);
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
            this.cb_SpeedTest_TestServer.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold);
            this.cb_SpeedTest_TestServer.FormattingEnabled = true;
            this.cb_SpeedTest_TestServer.Location = new System.Drawing.Point(147, 64);
            this.cb_SpeedTest_TestServer.MaxDropDownItems = 100;
            this.cb_SpeedTest_TestServer.Name = "cb_SpeedTest_TestServer";
            this.cb_SpeedTest_TestServer.Size = new System.Drawing.Size(537, 27);
            this.cb_SpeedTest_TestServer.TabIndex = 97;
            this.cb_SpeedTest_TestServer.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cb_SpeedTest_TestServer_DrawItem);
            this.cb_SpeedTest_TestServer.SelectedIndexChanged += new System.EventHandler(this.cb_SpeedTest_TestServer_SelectedIndexChanged);
            // 
            // lbl_SpeedTest_ExternalIP_Value
            // 
            this.lbl_SpeedTest_ExternalIP_Value.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_SpeedTest_ExternalIP_Value.BackColor = System.Drawing.Color.DimGray;
            this.lbl_SpeedTest_ExternalIP_Value.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_SpeedTest_ExternalIP_Value.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_SpeedTest_ExternalIP_Value.Location = new System.Drawing.Point(148, 7);
            this.lbl_SpeedTest_ExternalIP_Value.Name = "lbl_SpeedTest_ExternalIP_Value";
            this.lbl_SpeedTest_ExternalIP_Value.Size = new System.Drawing.Size(535, 25);
            this.lbl_SpeedTest_ExternalIP_Value.TabIndex = 103;
            this.lbl_SpeedTest_ExternalIP_Value.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_SpeedTest_CurrentCountry_Value
            // 
            this.lbl_SpeedTest_CurrentCountry_Value.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_SpeedTest_CurrentCountry_Value.BackColor = System.Drawing.Color.DimGray;
            this.lbl_SpeedTest_CurrentCountry_Value.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_SpeedTest_CurrentCountry_Value.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_SpeedTest_CurrentCountry_Value.Location = new System.Drawing.Point(148, 36);
            this.lbl_SpeedTest_CurrentCountry_Value.Name = "lbl_SpeedTest_CurrentCountry_Value";
            this.lbl_SpeedTest_CurrentCountry_Value.Size = new System.Drawing.Size(536, 25);
            this.lbl_SpeedTest_CurrentCountry_Value.TabIndex = 102;
            this.lbl_SpeedTest_CurrentCountry_Value.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_SpeedTest_Latency_Value
            // 
            this.lbl_SpeedTest_Latency_Value.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_SpeedTest_Latency_Value.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_SpeedTest_Latency_Value.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_SpeedTest_Latency_Value.Location = new System.Drawing.Point(148, 151);
            this.lbl_SpeedTest_Latency_Value.Name = "lbl_SpeedTest_Latency_Value";
            this.lbl_SpeedTest_Latency_Value.Size = new System.Drawing.Size(536, 25);
            this.lbl_SpeedTest_Latency_Value.TabIndex = 101;
            this.lbl_SpeedTest_Latency_Value.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_SpeedTest_Distance_Value
            // 
            this.lbl_SpeedTest_Distance_Value.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_SpeedTest_Distance_Value.BackColor = System.Drawing.Color.DimGray;
            this.lbl_SpeedTest_Distance_Value.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_SpeedTest_Distance_Value.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_SpeedTest_Distance_Value.Location = new System.Drawing.Point(148, 123);
            this.lbl_SpeedTest_Distance_Value.Name = "lbl_SpeedTest_Distance_Value";
            this.lbl_SpeedTest_Distance_Value.Size = new System.Drawing.Size(536, 25);
            this.lbl_SpeedTest_Distance_Value.TabIndex = 100;
            this.lbl_SpeedTest_Distance_Value.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_SpeedTest_HostedBy_Value
            // 
            this.lbl_SpeedTest_HostedBy_Value.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_SpeedTest_HostedBy_Value.BackColor = System.Drawing.Color.DimGray;
            this.lbl_SpeedTest_HostedBy_Value.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_SpeedTest_HostedBy_Value.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_SpeedTest_HostedBy_Value.Location = new System.Drawing.Point(148, 94);
            this.lbl_SpeedTest_HostedBy_Value.Name = "lbl_SpeedTest_HostedBy_Value";
            this.lbl_SpeedTest_HostedBy_Value.Size = new System.Drawing.Size(536, 25);
            this.lbl_SpeedTest_HostedBy_Value.TabIndex = 99;
            this.lbl_SpeedTest_HostedBy_Value.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btn_SpeedTest_GetServers
            // 
            this.btn_SpeedTest_GetServers.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btn_SpeedTest_GetServers.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn_SpeedTest_GetServers.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_SpeedTest_GetServers.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_SpeedTest_GetServers.Font = new System.Drawing.Font("Gadugi", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_SpeedTest_GetServers.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.btn_SpeedTest_GetServers.Location = new System.Drawing.Point(265, 378);
            this.btn_SpeedTest_GetServers.Name = "btn_SpeedTest_GetServers";
            this.btn_SpeedTest_GetServers.Size = new System.Drawing.Size(161, 26);
            this.btn_SpeedTest_GetServers.TabIndex = 104;
            this.btn_SpeedTest_GetServers.Text = "GET SERVERS";
            this.btn_SpeedTest_GetServers.UseVisualStyleBackColor = false;
            this.btn_SpeedTest_GetServers.Visible = false;
            this.btn_SpeedTest_GetServers.Click += new System.EventHandler(this.btn_SpeedTest_GetServers_Click);
            // 
            // pb_GO
            // 
            this.pb_GO.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pb_GO.BackColor = System.Drawing.Color.Transparent;
            this.pb_GO.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pb_GO.Image = global::EndpointChecker.Properties.Resources.goLogo;
            this.pb_GO.Location = new System.Drawing.Point(312, 469);
            this.pb_GO.Name = "pb_GO";
            this.pb_GO.Size = new System.Drawing.Size(64, 64);
            this.pb_GO.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_GO.TabIndex = 105;
            this.pb_GO.TabStop = false;
            this.pb_GO.Visible = false;
            this.pb_GO.Click += new System.EventHandler(this.pb_GO_Click);
            // 
            // pBar_Upload
            // 
            this.pBar_Upload.Location = new System.Drawing.Point(444, 598);
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
            this.pBar_Download.Location = new System.Drawing.Point(33, 598);
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
            this.ClientSize = new System.Drawing.Size(691, 633);
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
            this.Controls.Add(this.rb_CurrentCountryServersOnly);
            this.Controls.Add(this.rb_AllServersExceptCurrCountry);
            this.Controls.Add(this.rb_AllServers);
            this.Controls.Add(this.lbl_SpeedTest_CurrentCountry);
            this.Controls.Add(this.lbl_SpeedTest_Latency);
            this.Controls.Add(this.lbl_SpeedTest_Distance);
            this.Controls.Add(this.lbl_SpeedTest_HostedBy);
            this.Controls.Add(this.lbl_SpeedTest_TestServer);
            this.Controls.Add(this.pb_SpeedTestProgress);
            this.Controls.Add(this.rtb_SpeedTest_LogConsole);
            this.Controls.Add(this.pBar_Upload);
            this.Controls.Add(this.pBar_Download);
            this.Controls.Add(this.lbl_SpeedTest_Mbps_Download_Label);
            this.Controls.Add(this.lbl_SpeedTest_Mbps_Upload_Label);
            this.Controls.Add(this.aGauge_UploadSpeed);
            this.Controls.Add(this.aGauge_DownloadSpeed);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SpeedTestDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SpeedTest";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SpeedTestDialog_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pb_SpeedTestProgress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_GO)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        public System.Windows.Forms.RadioButton rb_AllServers;
        public System.Windows.Forms.RadioButton rb_AllServersExceptCurrCountry;
        public System.Windows.Forms.RadioButton rb_CurrentCountryServersOnly;
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
    }
}