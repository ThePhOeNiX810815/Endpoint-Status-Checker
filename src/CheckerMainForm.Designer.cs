namespace EndpointChecker
{
    partial class CheckerMainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CheckerMainForm));
            this.lv_Endpoints = new System.Windows.Forms.ListView();
            this.ch_EndpointName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_Protocol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_Port = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_EndpointURL = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_IPAddress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_ResponseTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_Code = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_Message = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_LastSeenOnline = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_MACAddress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_PingTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_Server = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_UserName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_NetworkShares = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_DNSName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_HTTPContentType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_HTTPContentLenght = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_HTTPExpires = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_HTTPETag = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList_ListViewIcons_20pix = new System.Windows.Forms.ImageList(this.components);
            this.cb_AutomaticRefresh = new System.Windows.Forms.CheckBox();
            this.bw_GetStatus = new System.ComponentModel.BackgroundWorker();
            this.TIMER_Refresh = new System.Windows.Forms.Timer(this.components);
            this.num_RefreshInterval = new System.Windows.Forms.NumericUpDown();
            this.lbl_TimerIntervalMinutesText = new System.Windows.Forms.Label();
            this.lbl_NoEndpoints = new System.Windows.Forms.Label();
            this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.trayContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tray_Refresh = new System.Windows.Forms.ToolStripMenuItem();
            this.tray_Separator = new System.Windows.Forms.ToolStripSeparator();
            this.tray_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.cb_TrayBalloonNotify = new System.Windows.Forms.CheckBox();
            this.lbl_RequestTimeoutSecondsText = new System.Windows.Forms.Label();
            this.num_HTTPRequestTimeout = new System.Windows.Forms.NumericUpDown();
            this.lbl_RequestTimeout = new System.Windows.Forms.Label();
            this.cb_AllowAutoRedirect = new System.Windows.Forms.CheckBox();
            this.lbl_Copyright = new System.Windows.Forms.Label();
            this.lbl_Version = new System.Windows.Forms.Label();
            this.lbl_ProgressCount = new System.Windows.Forms.Label();
            this.lbl_AutomaticRefresh = new System.Windows.Forms.Label();
            this.num_FTPRequestTimeout = new System.Windows.Forms.NumericUpDown();
            this.lbl_FTPRequestTimeout = new System.Windows.Forms.Label();
            this.lbl_FTPRequestTimeoutSecondsText = new System.Windows.Forms.Label();
            this.cb_RefreshAutoSet = new System.Windows.Forms.CheckBox();
            this.cb_ContinuousRefresh = new System.Windows.Forms.CheckBox();
            this.num_ParallelThreadsCount = new System.Windows.Forms.NumericUpDown();
            this.lbl_ParallelThreadsCount = new System.Windows.Forms.Label();
            this.instanceWatcher = new System.IO.FileSystemWatcher();
            this.cb_ResolveNetworkShares = new System.Windows.Forms.CheckBox();
            this.cb_ExportEndpointsStatus_XLSX = new System.Windows.Forms.CheckBox();
            this.folderBrowserExportDir = new System.Windows.Forms.FolderBrowserDialog();
            this.TIMER_StartupRefresh = new System.Windows.Forms.Timer(this.components);
            this.cb_SaveResponse = new System.Windows.Forms.CheckBox();
            this.cb_ResolvePageMetaInfo = new System.Windows.Forms.CheckBox();
            this.cb_ValidateSSLCertificate = new System.Windows.Forms.CheckBox();
            this.bw_LoadEndpointReferences = new System.ComponentModel.BackgroundWorker();
            this.groupBox_EndpointSelection = new System.Windows.Forms.GroupBox();
            this.lbl_CheckAllErrors = new System.Windows.Forms.Label();
            this.btn_UncheckAll = new System.Windows.Forms.Button();
            this.btn_CheckAll = new System.Windows.Forms.Button();
            this.lbl_CheckAll = new System.Windows.Forms.Label();
            this.btn_CheckAllErrors = new System.Windows.Forms.Button();
            this.lbl_UncheckAll = new System.Windows.Forms.Label();
            this.btn_CheckAllAvailable = new System.Windows.Forms.Button();
            this.lbl_CheckAllAvailable = new System.Windows.Forms.Label();
            this.lbl_ConfigFile = new System.Windows.Forms.Label();
            this.btn_EndpointsList = new System.Windows.Forms.Button();
            this.btn_ConfigFile = new System.Windows.Forms.Button();
            this.lbl_EndpointsList = new System.Windows.Forms.Label();
            this.lbl_Refresh = new System.Windows.Forms.Label();
            this.lbl_Terminate = new System.Windows.Forms.Label();
            this.lbl_BrowseExportDir = new System.Windows.Forms.Label();
            this.lbl_SpeedTest = new System.Windows.Forms.Label();
            this.imageList_Icons_32pix = new System.Windows.Forms.ImageList(this.components);
            this.num_PingTimeout = new System.Windows.Forms.NumericUpDown();
            this.lbl_PingTimeout = new System.Windows.Forms.Label();
            this.lbl_PingTimeoutSecondsText = new System.Windows.Forms.Label();
            this.lbl_Validate = new System.Windows.Forms.Label();
            this.comboBox_Validate = new System.Windows.Forms.ComboBox();
            this.lv_Endpoints_ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem_Details = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator_2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem_Browse = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_AdminBrowse = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_HTTP = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_FTP = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator_1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem_RDP = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_VNC = new System.Windows.Forms.ToolStripMenuItem();
            this.btn_Terminate = new System.Windows.Forms.Button();
            this.btn_Refresh = new System.Windows.Forms.Button();
            this.btn_SpeedTest = new System.Windows.Forms.Button();
            this.btn_BrowseExportDir = new System.Windows.Forms.Button();
            this.pb_Progress = new System.Windows.Forms.PictureBox();
            this.pb_Progress_Init = new System.Windows.Forms.PictureBox();
            this.cb_ExportEndpointsStatus_HTML = new System.Windows.Forms.CheckBox();
            this.lbl_LastUpdate_Label = new System.Windows.Forms.Label();
            this.lbl_LastUpdate = new System.Windows.Forms.Label();
            this.cb_ExportEndpointsStatus_JSON = new System.Windows.Forms.CheckBox();
            this.cb_ExportEndpointsStatus_XML = new System.Windows.Forms.CheckBox();
            this.groupBox_Export = new System.Windows.Forms.GroupBox();
            this.TIMER_TrayIconAnimation = new System.Windows.Forms.Timer(this.components);
            this.cb_RemoveURLParameters = new System.Windows.Forms.CheckBox();
            this.groupBox_CommonOptions = new System.Windows.Forms.GroupBox();
            this.cb_ResolvePageLinks = new System.Windows.Forms.CheckBox();
            this.groupBox_HTTPOptions = new System.Windows.Forms.GroupBox();
            this.pb_ITNetwork = new System.Windows.Forms.PictureBox();
            this.link_AuthorMail = new System.Windows.Forms.LinkLabel();
            this.pb_FeatureRequest = new System.Windows.Forms.PictureBox();
            this.TIMER_ListAndLogsFilesWatcher = new System.Windows.Forms.Timer(this.components);
            this.pb_GitHub = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.num_RefreshInterval)).BeginInit();
            this.trayContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_HTTPRequestTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_FTPRequestTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_ParallelThreadsCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.instanceWatcher)).BeginInit();
            this.groupBox_EndpointSelection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_PingTimeout)).BeginInit();
            this.lv_Endpoints_ContextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Progress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Progress_Init)).BeginInit();
            this.groupBox_Export.SuspendLayout();
            this.groupBox_CommonOptions.SuspendLayout();
            this.groupBox_HTTPOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_ITNetwork)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_FeatureRequest)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_GitHub)).BeginInit();
            this.SuspendLayout();
            // 
            // lv_Endpoints
            // 
            this.lv_Endpoints.AllowColumnReorder = true;
            this.lv_Endpoints.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lv_Endpoints.BackColor = System.Drawing.Color.Gainsboro;
            this.lv_Endpoints.CheckBoxes = true;
            this.lv_Endpoints.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ch_EndpointName,
            this.ch_Protocol,
            this.ch_Port,
            this.ch_EndpointURL,
            this.ch_IPAddress,
            this.ch_ResponseTime,
            this.ch_Code,
            this.ch_Message,
            this.ch_LastSeenOnline,
            this.ch_MACAddress,
            this.ch_PingTime,
            this.ch_Server,
            this.ch_UserName,
            this.ch_NetworkShares,
            this.ch_DNSName,
            this.ch_HTTPContentType,
            this.ch_HTTPContentLenght,
            this.ch_HTTPExpires,
            this.ch_HTTPETag});
            this.lv_Endpoints.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lv_Endpoints.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lv_Endpoints.FullRowSelect = true;
            this.lv_Endpoints.GridLines = true;
            this.lv_Endpoints.HideSelection = false;
            this.lv_Endpoints.LargeImageList = this.imageList_ListViewIcons_20pix;
            this.lv_Endpoints.Location = new System.Drawing.Point(12, 9);
            this.lv_Endpoints.Name = "lv_Endpoints";
            this.lv_Endpoints.ShowItemToolTips = true;
            this.lv_Endpoints.Size = new System.Drawing.Size(1084, 281);
            this.lv_Endpoints.SmallImageList = this.imageList_ListViewIcons_20pix;
            this.lv_Endpoints.TabIndex = 0;
            this.lv_Endpoints.UseCompatibleStateImageBehavior = false;
            this.lv_Endpoints.View = System.Windows.Forms.View.Details;
            this.lv_Endpoints.Visible = false;
            this.lv_Endpoints.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lv_Endpoints_ColumnClick);
            this.lv_Endpoints.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lv_Endpoints_ItemChecked);
            this.lv_Endpoints.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lv_Endpoints_ItemSelectionChanged);
            this.lv_Endpoints.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lv_Endpoints_KeyUp);
            this.lv_Endpoints.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lv_Endpoints_MouseClick);
            this.lv_Endpoints.MouseLeave += new System.EventHandler(this.Lv_Endpoints_MouseLeave);
            this.lv_Endpoints.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lv_Endpoints_MouseMove);
            // 
            // ch_EndpointName
            // 
            this.ch_EndpointName.Tag = "";
            this.ch_EndpointName.Text = "Endpoint Name";
            this.ch_EndpointName.Width = 200;
            // 
            // ch_Protocol
            // 
            this.ch_Protocol.Text = "Protocol";
            // 
            // ch_Port
            // 
            this.ch_Port.Text = "Port";
            this.ch_Port.Width = 50;
            // 
            // ch_EndpointURL
            // 
            this.ch_EndpointURL.Text = "Endpoint URL";
            this.ch_EndpointURL.Width = 300;
            // 
            // ch_IPAddress
            // 
            this.ch_IPAddress.Text = "IP Address";
            this.ch_IPAddress.Width = 105;
            // 
            // ch_ResponseTime
            // 
            this.ch_ResponseTime.Text = "Time";
            this.ch_ResponseTime.Width = 75;
            // 
            // ch_Code
            // 
            this.ch_Code.Text = "Code";
            this.ch_Code.Width = 55;
            // 
            // ch_Message
            // 
            this.ch_Message.Text = "Status Message";
            this.ch_Message.Width = 220;
            // 
            // ch_LastSeenOnline
            // 
            this.ch_LastSeenOnline.Text = "Last Seen Online";
            // 
            // ch_MACAddress
            // 
            this.ch_MACAddress.Text = "MAC Address";
            // 
            // ch_PingTime
            // 
            this.ch_PingTime.Text = "Ping Roundtrip Time";
            // 
            // ch_Server
            // 
            this.ch_Server.Text = "Server";
            // 
            // ch_UserName
            // 
            this.ch_UserName.Text = "User Name";
            // 
            // ch_NetworkShares
            // 
            this.ch_NetworkShares.Text = "Network Shares";
            // 
            // ch_DNSName
            // 
            this.ch_DNSName.Text = "DNS Name";
            // 
            // ch_HTTPContentType
            // 
            this.ch_HTTPContentType.Text = "HTTP Content Type";
            // 
            // ch_HTTPContentLenght
            // 
            this.ch_HTTPContentLenght.Text = "HTTP Content Lenght";
            // 
            // ch_HTTPExpires
            // 
            this.ch_HTTPExpires.Text = "HTTP Expires";
            // 
            // ch_HTTPETag
            // 
            this.ch_HTTPETag.Text = "HTTP ETag";
            // 
            // imageList_ListViewIcons_20pix
            // 
            this.imageList_ListViewIcons_20pix.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_ListViewIcons_20pix.ImageStream")));
            this.imageList_ListViewIcons_20pix.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList_ListViewIcons_20pix.Images.SetKeyName(0, "passed.ico");
            this.imageList_ListViewIcons_20pix.Images.SetKeyName(1, "error.ico");
            this.imageList_ListViewIcons_20pix.Images.SetKeyName(2, "notAvailable.ico");
            this.imageList_ListViewIcons_20pix.Images.SetKeyName(3, "notKnown.ico");
            this.imageList_ListViewIcons_20pix.Images.SetKeyName(4, "information.ico");
            this.imageList_ListViewIcons_20pix.Images.SetKeyName(5, "warning.ico");
            this.imageList_ListViewIcons_20pix.Images.SetKeyName(6, "disabled.ico");
            this.imageList_ListViewIcons_20pix.Images.SetKeyName(7, "plus.ico");
            this.imageList_ListViewIcons_20pix.Images.SetKeyName(8, "radioactive.ico");
            this.imageList_ListViewIcons_20pix.Images.SetKeyName(9, "refresh.ico");
            this.imageList_ListViewIcons_20pix.Images.SetKeyName(10, "star.ico");
            this.imageList_ListViewIcons_20pix.Images.SetKeyName(11, "minus.ico");
            // 
            // cb_AutomaticRefresh
            // 
            this.cb_AutomaticRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cb_AutomaticRefresh.Checked = true;
            this.cb_AutomaticRefresh.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_AutomaticRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cb_AutomaticRefresh.Enabled = false;
            this.cb_AutomaticRefresh.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_AutomaticRefresh.Location = new System.Drawing.Point(10, 41);
            this.cb_AutomaticRefresh.Name = "cb_AutomaticRefresh";
            this.cb_AutomaticRefresh.Size = new System.Drawing.Size(217, 19);
            this.cb_AutomaticRefresh.TabIndex = 2;
            this.cb_AutomaticRefresh.Text = "Enable Automatic Refresh";
            this.cb_AutomaticRefresh.UseVisualStyleBackColor = true;
            this.cb_AutomaticRefresh.CheckedChanged += new System.EventHandler(this.cb_AutomaticRefresh_CheckedChanged);
            // 
            // bw_GetStatus
            // 
            this.bw_GetStatus.WorkerSupportsCancellation = true;
            this.bw_GetStatus.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bw_GetStatus_DoWork);
            this.bw_GetStatus.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bw_GetStatus_RunWorkerCompleted);
            // 
            // TIMER_Refresh
            // 
            this.TIMER_Refresh.Enabled = true;
            this.TIMER_Refresh.Interval = 60000;
            this.TIMER_Refresh.Tick += new System.EventHandler(this.timer_Refresh_Tick);
            // 
            // num_RefreshInterval
            // 
            this.num_RefreshInterval.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.num_RefreshInterval.BackColor = System.Drawing.Color.LightGray;
            this.num_RefreshInterval.Cursor = System.Windows.Forms.Cursors.Hand;
            this.num_RefreshInterval.Enabled = false;
            this.num_RefreshInterval.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.num_RefreshInterval.Location = new System.Drawing.Point(150, 295);
            this.num_RefreshInterval.Maximum = new decimal(new int[] {
            1440,
            0,
            0,
            0});
            this.num_RefreshInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_RefreshInterval.Name = "num_RefreshInterval";
            this.num_RefreshInterval.Size = new System.Drawing.Size(47, 23);
            this.num_RefreshInterval.TabIndex = 3;
            this.num_RefreshInterval.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.num_RefreshInterval.ValueChanged += new System.EventHandler(this.num_RefreshInterval_ValueChanged);
            // 
            // lbl_TimerIntervalMinutesText
            // 
            this.lbl_TimerIntervalMinutesText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbl_TimerIntervalMinutesText.AutoSize = true;
            this.lbl_TimerIntervalMinutesText.Cursor = System.Windows.Forms.Cursors.Default;
            this.lbl_TimerIntervalMinutesText.Enabled = false;
            this.lbl_TimerIntervalMinutesText.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_TimerIntervalMinutesText.Location = new System.Drawing.Point(201, 297);
            this.lbl_TimerIntervalMinutesText.Name = "lbl_TimerIntervalMinutesText";
            this.lbl_TimerIntervalMinutesText.Size = new System.Drawing.Size(52, 15);
            this.lbl_TimerIntervalMinutesText.TabIndex = 4;
            this.lbl_TimerIntervalMinutesText.Text = "minutes";
            // 
            // lbl_NoEndpoints
            // 
            this.lbl_NoEndpoints.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_NoEndpoints.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_NoEndpoints.ForeColor = System.Drawing.Color.DarkOrange;
            this.lbl_NoEndpoints.Location = new System.Drawing.Point(0, 5);
            this.lbl_NoEndpoints.Name = "lbl_NoEndpoints";
            this.lbl_NoEndpoints.Size = new System.Drawing.Size(1104, 291);
            this.lbl_NoEndpoints.TabIndex = 6;
            this.lbl_NoEndpoints.Text = "List of predefined endpoints is empty";
            this.lbl_NoEndpoints.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_NoEndpoints.Visible = false;
            // 
            // trayIcon
            // 
            this.trayIcon.ContextMenuStrip = this.trayContextMenu;
            this.trayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("trayIcon.Icon")));
            this.trayIcon.Visible = true;
            this.trayIcon.BalloonTipClicked += new System.EventHandler(this.trayIcon_BalloonTipClicked);
            this.trayIcon.BalloonTipClosed += new System.EventHandler(this.trayIcon_BalloonTipClosed);
            this.trayIcon.BalloonTipShown += new System.EventHandler(this.trayIcon_BalloonTipShown);
            this.trayIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.trayIcon_MouseClick);
            // 
            // trayContextMenu
            // 
            this.trayContextMenu.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trayContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tray_Refresh,
            this.tray_Separator,
            this.tray_Exit});
            this.trayContextMenu.Name = "tray_contextMenu";
            this.trayContextMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.trayContextMenu.Size = new System.Drawing.Size(121, 54);
            // 
            // tray_Refresh
            // 
            this.tray_Refresh.Name = "tray_Refresh";
            this.tray_Refresh.Size = new System.Drawing.Size(120, 22);
            this.tray_Refresh.Text = "Refresh";
            this.tray_Refresh.Visible = false;
            this.tray_Refresh.Click += new System.EventHandler(this.tray_Refresh_Click);
            // 
            // tray_Separator
            // 
            this.tray_Separator.Name = "tray_Separator";
            this.tray_Separator.Size = new System.Drawing.Size(117, 6);
            this.tray_Separator.Visible = false;
            // 
            // tray_Exit
            // 
            this.tray_Exit.Name = "tray_Exit";
            this.tray_Exit.Size = new System.Drawing.Size(120, 22);
            this.tray_Exit.Text = "Close";
            this.tray_Exit.Click += new System.EventHandler(this.tray_Exit_Click);
            // 
            // cb_TrayBalloonNotify
            // 
            this.cb_TrayBalloonNotify.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cb_TrayBalloonNotify.Checked = true;
            this.cb_TrayBalloonNotify.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_TrayBalloonNotify.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cb_TrayBalloonNotify.Enabled = false;
            this.cb_TrayBalloonNotify.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_TrayBalloonNotify.Location = new System.Drawing.Point(10, 75);
            this.cb_TrayBalloonNotify.Name = "cb_TrayBalloonNotify";
            this.cb_TrayBalloonNotify.Size = new System.Drawing.Size(217, 19);
            this.cb_TrayBalloonNotify.TabIndex = 7;
            this.cb_TrayBalloonNotify.Text = "Enable Tray Notifcations (on Errors)";
            this.cb_TrayBalloonNotify.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cb_TrayBalloonNotify.UseVisualStyleBackColor = true;
            this.cb_TrayBalloonNotify.CheckedChanged += new System.EventHandler(this.cb_TrayBalloonNotify_CheckedChanged);
            // 
            // lbl_RequestTimeoutSecondsText
            // 
            this.lbl_RequestTimeoutSecondsText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbl_RequestTimeoutSecondsText.AutoSize = true;
            this.lbl_RequestTimeoutSecondsText.Cursor = System.Windows.Forms.Cursors.Default;
            this.lbl_RequestTimeoutSecondsText.Enabled = false;
            this.lbl_RequestTimeoutSecondsText.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_RequestTimeoutSecondsText.Location = new System.Drawing.Point(201, 353);
            this.lbl_RequestTimeoutSecondsText.Name = "lbl_RequestTimeoutSecondsText";
            this.lbl_RequestTimeoutSecondsText.Size = new System.Drawing.Size(50, 15);
            this.lbl_RequestTimeoutSecondsText.TabIndex = 9;
            this.lbl_RequestTimeoutSecondsText.Text = "seconds";
            // 
            // num_HTTPRequestTimeout
            // 
            this.num_HTTPRequestTimeout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.num_HTTPRequestTimeout.BackColor = System.Drawing.Color.LightGray;
            this.num_HTTPRequestTimeout.Cursor = System.Windows.Forms.Cursors.Hand;
            this.num_HTTPRequestTimeout.Enabled = false;
            this.num_HTTPRequestTimeout.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.num_HTTPRequestTimeout.Location = new System.Drawing.Point(150, 351);
            this.num_HTTPRequestTimeout.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.num_HTTPRequestTimeout.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_HTTPRequestTimeout.Name = "num_HTTPRequestTimeout";
            this.num_HTTPRequestTimeout.Size = new System.Drawing.Size(47, 23);
            this.num_HTTPRequestTimeout.TabIndex = 8;
            this.num_HTTPRequestTimeout.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.num_HTTPRequestTimeout.ValueChanged += new System.EventHandler(this.num_RequestTimeout_ValueChanged);
            // 
            // lbl_RequestTimeout
            // 
            this.lbl_RequestTimeout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbl_RequestTimeout.AutoSize = true;
            this.lbl_RequestTimeout.Enabled = false;
            this.lbl_RequestTimeout.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_RequestTimeout.Location = new System.Drawing.Point(10, 353);
            this.lbl_RequestTimeout.Name = "lbl_RequestTimeout";
            this.lbl_RequestTimeout.Size = new System.Drawing.Size(131, 15);
            this.lbl_RequestTimeout.TabIndex = 10;
            this.lbl_RequestTimeout.Text = "HTTP Request Timeout";
            // 
            // cb_AllowAutoRedirect
            // 
            this.cb_AllowAutoRedirect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cb_AllowAutoRedirect.Checked = true;
            this.cb_AllowAutoRedirect.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_AllowAutoRedirect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cb_AllowAutoRedirect.Enabled = false;
            this.cb_AllowAutoRedirect.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_AllowAutoRedirect.Location = new System.Drawing.Point(10, 16);
            this.cb_AllowAutoRedirect.Name = "cb_AllowAutoRedirect";
            this.cb_AllowAutoRedirect.Size = new System.Drawing.Size(207, 19);
            this.cb_AllowAutoRedirect.TabIndex = 11;
            this.cb_AllowAutoRedirect.Text = "Allow Auto Redirect";
            this.cb_AllowAutoRedirect.UseVisualStyleBackColor = true;
            this.cb_AllowAutoRedirect.CheckedChanged += new System.EventHandler(this.cb_AllowAutoRedirect_CheckedChanged);
            // 
            // lbl_Copyright
            // 
            this.lbl_Copyright.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbl_Copyright.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbl_Copyright.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Copyright.Location = new System.Drawing.Point(5, 501);
            this.lbl_Copyright.Name = "lbl_Copyright";
            this.lbl_Copyright.Size = new System.Drawing.Size(143, 20);
            this.lbl_Copyright.TabIndex = 12;
            this.lbl_Copyright.Text = "Copyright © 2014-2021";
            this.lbl_Copyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_Version
            // 
            this.lbl_Version.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_Version.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Version.Location = new System.Drawing.Point(802, 501);
            this.lbl_Version.Name = "lbl_Version";
            this.lbl_Version.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lbl_Version.Size = new System.Drawing.Size(300, 20);
            this.lbl_Version.TabIndex = 13;
            this.lbl_Version.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbl_ProgressCount
            // 
            this.lbl_ProgressCount.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lbl_ProgressCount.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_ProgressCount.Location = new System.Drawing.Point(249, 489);
            this.lbl_ProgressCount.Name = "lbl_ProgressCount";
            this.lbl_ProgressCount.Size = new System.Drawing.Size(610, 30);
            this.lbl_ProgressCount.TabIndex = 19;
            this.lbl_ProgressCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_AutomaticRefresh
            // 
            this.lbl_AutomaticRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbl_AutomaticRefresh.AutoSize = true;
            this.lbl_AutomaticRefresh.Enabled = false;
            this.lbl_AutomaticRefresh.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_AutomaticRefresh.Location = new System.Drawing.Point(10, 297);
            this.lbl_AutomaticRefresh.Name = "lbl_AutomaticRefresh";
            this.lbl_AutomaticRefresh.Size = new System.Drawing.Size(127, 15);
            this.lbl_AutomaticRefresh.TabIndex = 21;
            this.lbl_AutomaticRefresh.Text = "Auto Refresh  Interval";
            // 
            // num_FTPRequestTimeout
            // 
            this.num_FTPRequestTimeout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.num_FTPRequestTimeout.BackColor = System.Drawing.Color.LightGray;
            this.num_FTPRequestTimeout.Cursor = System.Windows.Forms.Cursors.Hand;
            this.num_FTPRequestTimeout.Enabled = false;
            this.num_FTPRequestTimeout.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.num_FTPRequestTimeout.Location = new System.Drawing.Point(150, 379);
            this.num_FTPRequestTimeout.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.num_FTPRequestTimeout.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_FTPRequestTimeout.Name = "num_FTPRequestTimeout";
            this.num_FTPRequestTimeout.Size = new System.Drawing.Size(47, 23);
            this.num_FTPRequestTimeout.TabIndex = 22;
            this.num_FTPRequestTimeout.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.num_FTPRequestTimeout.ValueChanged += new System.EventHandler(this.num_FTPRequestTimeout_ValueChanged);
            // 
            // lbl_FTPRequestTimeout
            // 
            this.lbl_FTPRequestTimeout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbl_FTPRequestTimeout.AutoSize = true;
            this.lbl_FTPRequestTimeout.Enabled = false;
            this.lbl_FTPRequestTimeout.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_FTPRequestTimeout.Location = new System.Drawing.Point(10, 381);
            this.lbl_FTPRequestTimeout.Name = "lbl_FTPRequestTimeout";
            this.lbl_FTPRequestTimeout.Size = new System.Drawing.Size(123, 15);
            this.lbl_FTPRequestTimeout.TabIndex = 24;
            this.lbl_FTPRequestTimeout.Text = "FTP Request Timeout";
            // 
            // lbl_FTPRequestTimeoutSecondsText
            // 
            this.lbl_FTPRequestTimeoutSecondsText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbl_FTPRequestTimeoutSecondsText.AutoSize = true;
            this.lbl_FTPRequestTimeoutSecondsText.Cursor = System.Windows.Forms.Cursors.Default;
            this.lbl_FTPRequestTimeoutSecondsText.Enabled = false;
            this.lbl_FTPRequestTimeoutSecondsText.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_FTPRequestTimeoutSecondsText.Location = new System.Drawing.Point(201, 381);
            this.lbl_FTPRequestTimeoutSecondsText.Name = "lbl_FTPRequestTimeoutSecondsText";
            this.lbl_FTPRequestTimeoutSecondsText.Size = new System.Drawing.Size(50, 15);
            this.lbl_FTPRequestTimeoutSecondsText.TabIndex = 23;
            this.lbl_FTPRequestTimeoutSecondsText.Text = "seconds";
            // 
            // cb_RefreshAutoSet
            // 
            this.cb_RefreshAutoSet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cb_RefreshAutoSet.Checked = true;
            this.cb_RefreshAutoSet.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_RefreshAutoSet.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cb_RefreshAutoSet.Enabled = false;
            this.cb_RefreshAutoSet.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_RefreshAutoSet.Location = new System.Drawing.Point(10, 58);
            this.cb_RefreshAutoSet.Name = "cb_RefreshAutoSet";
            this.cb_RefreshAutoSet.Size = new System.Drawing.Size(217, 19);
            this.cb_RefreshAutoSet.TabIndex = 26;
            this.cb_RefreshAutoSet.Text = "Auto Adjust Refresh Interval";
            this.cb_RefreshAutoSet.UseVisualStyleBackColor = true;
            this.cb_RefreshAutoSet.CheckedChanged += new System.EventHandler(this.cb_RefreshAutoSet_CheckedChanged);
            // 
            // cb_ContinuousRefresh
            // 
            this.cb_ContinuousRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cb_ContinuousRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cb_ContinuousRefresh.Enabled = false;
            this.cb_ContinuousRefresh.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_ContinuousRefresh.Location = new System.Drawing.Point(10, 24);
            this.cb_ContinuousRefresh.Name = "cb_ContinuousRefresh";
            this.cb_ContinuousRefresh.Size = new System.Drawing.Size(217, 19);
            this.cb_ContinuousRefresh.TabIndex = 27;
            this.cb_ContinuousRefresh.Text = "Enable Continuous Refresh";
            this.cb_ContinuousRefresh.UseVisualStyleBackColor = true;
            this.cb_ContinuousRefresh.CheckedChanged += new System.EventHandler(this.cb_ContinuousRefresh_CheckedChanged);
            // 
            // num_ParallelThreadsCount
            // 
            this.num_ParallelThreadsCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.num_ParallelThreadsCount.BackColor = System.Drawing.Color.LightGray;
            this.num_ParallelThreadsCount.Cursor = System.Windows.Forms.Cursors.Hand;
            this.num_ParallelThreadsCount.Enabled = false;
            this.num_ParallelThreadsCount.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.num_ParallelThreadsCount.Location = new System.Drawing.Point(150, 407);
            this.num_ParallelThreadsCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_ParallelThreadsCount.Name = "num_ParallelThreadsCount";
            this.num_ParallelThreadsCount.Size = new System.Drawing.Size(47, 23);
            this.num_ParallelThreadsCount.TabIndex = 29;
            this.num_ParallelThreadsCount.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.num_ParallelThreadsCount.ValueChanged += new System.EventHandler(this.num_ParallelThreadsCount_ValueChanged);
            // 
            // lbl_ParallelThreadsCount
            // 
            this.lbl_ParallelThreadsCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbl_ParallelThreadsCount.AutoSize = true;
            this.lbl_ParallelThreadsCount.Enabled = false;
            this.lbl_ParallelThreadsCount.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_ParallelThreadsCount.Location = new System.Drawing.Point(10, 409);
            this.lbl_ParallelThreadsCount.Name = "lbl_ParallelThreadsCount";
            this.lbl_ParallelThreadsCount.Size = new System.Drawing.Size(129, 15);
            this.lbl_ParallelThreadsCount.TabIndex = 30;
            this.lbl_ParallelThreadsCount.Text = "Parallel Threads Count";
            // 
            // instanceWatcher
            // 
            this.instanceWatcher.EnableRaisingEvents = true;
            this.instanceWatcher.NotifyFilter = System.IO.NotifyFilters.FileName;
            this.instanceWatcher.SynchronizingObject = this;
            this.instanceWatcher.Created += new System.IO.FileSystemEventHandler(this.instanceWatcher_Created);
            // 
            // cb_ResolveNetworkShares
            // 
            this.cb_ResolveNetworkShares.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cb_ResolveNetworkShares.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cb_ResolveNetworkShares.Enabled = false;
            this.cb_ResolveNetworkShares.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_ResolveNetworkShares.Location = new System.Drawing.Point(10, 92);
            this.cb_ResolveNetworkShares.Name = "cb_ResolveNetworkShares";
            this.cb_ResolveNetworkShares.Size = new System.Drawing.Size(217, 19);
            this.cb_ResolveNetworkShares.TabIndex = 31;
            this.cb_ResolveNetworkShares.Text = "Resolve Network Shares";
            this.cb_ResolveNetworkShares.UseVisualStyleBackColor = true;
            this.cb_ResolveNetworkShares.CheckedChanged += new System.EventHandler(this.cb_ResolveNetworkShares_CheckedChanged);
            // 
            // cb_ExportEndpointsStatus_XLSX
            // 
            this.cb_ExportEndpointsStatus_XLSX.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cb_ExportEndpointsStatus_XLSX.AutoSize = true;
            this.cb_ExportEndpointsStatus_XLSX.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cb_ExportEndpointsStatus_XLSX.Enabled = false;
            this.cb_ExportEndpointsStatus_XLSX.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_ExportEndpointsStatus_XLSX.Location = new System.Drawing.Point(42, 15);
            this.cb_ExportEndpointsStatus_XLSX.Name = "cb_ExportEndpointsStatus_XLSX";
            this.cb_ExportEndpointsStatus_XLSX.Size = new System.Drawing.Size(52, 19);
            this.cb_ExportEndpointsStatus_XLSX.TabIndex = 32;
            this.cb_ExportEndpointsStatus_XLSX.Text = "XLSX";
            this.cb_ExportEndpointsStatus_XLSX.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cb_ExportEndpointsStatus_XLSX.UseVisualStyleBackColor = true;
            this.cb_ExportEndpointsStatus_XLSX.CheckedChanged += new System.EventHandler(this.cb_ExportEndpointsStatus_XLSX_CheckedChanged);
            // 
            // folderBrowserExportDir
            // 
            this.folderBrowserExportDir.Description = "Browse directory for Endpoints Status Export";
            // 
            // TIMER_StartupRefresh
            // 
            this.TIMER_StartupRefresh.Interval = 1000;
            this.TIMER_StartupRefresh.Tick += new System.EventHandler(this.TIMER_StartupRefresh_Tick);
            // 
            // cb_SaveResponse
            // 
            this.cb_SaveResponse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cb_SaveResponse.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cb_SaveResponse.Enabled = false;
            this.cb_SaveResponse.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_SaveResponse.Location = new System.Drawing.Point(10, 101);
            this.cb_SaveResponse.Name = "cb_SaveResponse";
            this.cb_SaveResponse.Size = new System.Drawing.Size(207, 19);
            this.cb_SaveResponse.TabIndex = 35;
            this.cb_SaveResponse.Text = "Save Response To File";
            this.cb_SaveResponse.UseVisualStyleBackColor = true;
            this.cb_SaveResponse.CheckedChanged += new System.EventHandler(this.cb_SaveResponse_CheckedChanged);
            // 
            // cb_ResolvePageMetaInfo
            // 
            this.cb_ResolvePageMetaInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cb_ResolvePageMetaInfo.Checked = true;
            this.cb_ResolvePageMetaInfo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_ResolvePageMetaInfo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cb_ResolvePageMetaInfo.Enabled = false;
            this.cb_ResolvePageMetaInfo.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_ResolvePageMetaInfo.Location = new System.Drawing.Point(10, 67);
            this.cb_ResolvePageMetaInfo.Name = "cb_ResolvePageMetaInfo";
            this.cb_ResolvePageMetaInfo.Size = new System.Drawing.Size(207, 19);
            this.cb_ResolvePageMetaInfo.TabIndex = 36;
            this.cb_ResolvePageMetaInfo.Text = "Resolve HTML Meta Info";
            this.cb_ResolvePageMetaInfo.UseVisualStyleBackColor = true;
            this.cb_ResolvePageMetaInfo.CheckedChanged += new System.EventHandler(this.cb_ResolvePageMetaInfo_CheckedChanged);
            // 
            // cb_ValidateSSLCertificate
            // 
            this.cb_ValidateSSLCertificate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cb_ValidateSSLCertificate.Checked = true;
            this.cb_ValidateSSLCertificate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_ValidateSSLCertificate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cb_ValidateSSLCertificate.Enabled = false;
            this.cb_ValidateSSLCertificate.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_ValidateSSLCertificate.Location = new System.Drawing.Point(10, 33);
            this.cb_ValidateSSLCertificate.Name = "cb_ValidateSSLCertificate";
            this.cb_ValidateSSLCertificate.Size = new System.Drawing.Size(207, 19);
            this.cb_ValidateSSLCertificate.TabIndex = 37;
            this.cb_ValidateSSLCertificate.Text = "Validate SSL Certificate";
            this.cb_ValidateSSLCertificate.UseVisualStyleBackColor = true;
            this.cb_ValidateSSLCertificate.CheckedChanged += new System.EventHandler(this.cb_ValidateSSLCertificate_CheckedChanged);
            // 
            // bw_LoadEndpointReferences
            // 
            this.bw_LoadEndpointReferences.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bw_LoadEndpointReferences_DoWork);
            this.bw_LoadEndpointReferences.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bw_LoadEndpointReferences_RunWorkerCompleted);
            // 
            // groupBox_EndpointSelection
            // 
            this.groupBox_EndpointSelection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox_EndpointSelection.Controls.Add(this.lbl_CheckAllErrors);
            this.groupBox_EndpointSelection.Controls.Add(this.btn_UncheckAll);
            this.groupBox_EndpointSelection.Controls.Add(this.btn_CheckAll);
            this.groupBox_EndpointSelection.Controls.Add(this.lbl_CheckAll);
            this.groupBox_EndpointSelection.Controls.Add(this.btn_CheckAllErrors);
            this.groupBox_EndpointSelection.Controls.Add(this.lbl_UncheckAll);
            this.groupBox_EndpointSelection.Controls.Add(this.btn_CheckAllAvailable);
            this.groupBox_EndpointSelection.Controls.Add(this.lbl_CheckAllAvailable);
            this.groupBox_EndpointSelection.Enabled = false;
            this.groupBox_EndpointSelection.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox_EndpointSelection.Location = new System.Drawing.Point(766, 290);
            this.groupBox_EndpointSelection.Name = "groupBox_EndpointSelection";
            this.groupBox_EndpointSelection.Size = new System.Drawing.Size(189, 120);
            this.groupBox_EndpointSelection.TabIndex = 38;
            this.groupBox_EndpointSelection.TabStop = false;
            this.groupBox_EndpointSelection.Text = "Endpoints Selection";
            // 
            // lbl_CheckAllErrors
            // 
            this.lbl_CheckAllErrors.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_CheckAllErrors.AutoSize = true;
            this.lbl_CheckAllErrors.Enabled = false;
            this.lbl_CheckAllErrors.Font = new System.Drawing.Font("Comic Sans MS", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_CheckAllErrors.Location = new System.Drawing.Point(134, 81);
            this.lbl_CheckAllErrors.Name = "lbl_CheckAllErrors";
            this.lbl_CheckAllErrors.Size = new System.Drawing.Size(50, 16);
            this.lbl_CheckAllErrors.TabIndex = 22;
            this.lbl_CheckAllErrors.Text = "FAILED";
            // 
            // btn_UncheckAll
            // 
            this.btn_UncheckAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_UncheckAll.BackColor = System.Drawing.Color.DarkGray;
            this.btn_UncheckAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_UncheckAll.Enabled = false;
            this.btn_UncheckAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_UncheckAll.Image = global::EndpointChecker.Properties.Resources.minus;
            this.btn_UncheckAll.Location = new System.Drawing.Point(8, 67);
            this.btn_UncheckAll.Name = "btn_UncheckAll";
            this.btn_UncheckAll.Size = new System.Drawing.Size(40, 40);
            this.btn_UncheckAll.TabIndex = 15;
            this.btn_UncheckAll.UseVisualStyleBackColor = false;
            this.btn_UncheckAll.Click += new System.EventHandler(this.btn_UncheckAll_Click);
            // 
            // btn_CheckAll
            // 
            this.btn_CheckAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_CheckAll.BackColor = System.Drawing.Color.DarkGray;
            this.btn_CheckAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_CheckAll.Enabled = false;
            this.btn_CheckAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_CheckAll.Image = global::EndpointChecker.Properties.Resources.plus;
            this.btn_CheckAll.Location = new System.Drawing.Point(8, 25);
            this.btn_CheckAll.Name = "btn_CheckAll";
            this.btn_CheckAll.Size = new System.Drawing.Size(40, 40);
            this.btn_CheckAll.TabIndex = 17;
            this.btn_CheckAll.UseVisualStyleBackColor = false;
            this.btn_CheckAll.Click += new System.EventHandler(this.btn_CheckAll_Click);
            // 
            // lbl_CheckAll
            // 
            this.lbl_CheckAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_CheckAll.AutoSize = true;
            this.lbl_CheckAll.Enabled = false;
            this.lbl_CheckAll.Font = new System.Drawing.Font("Comic Sans MS", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_CheckAll.Location = new System.Drawing.Point(47, 38);
            this.lbl_CheckAll.Name = "lbl_CheckAll";
            this.lbl_CheckAll.Size = new System.Drawing.Size(28, 16);
            this.lbl_CheckAll.TabIndex = 21;
            this.lbl_CheckAll.Text = "ALL";
            // 
            // btn_CheckAllErrors
            // 
            this.btn_CheckAllErrors.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_CheckAllErrors.BackColor = System.Drawing.Color.DarkGray;
            this.btn_CheckAllErrors.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_CheckAllErrors.Enabled = false;
            this.btn_CheckAllErrors.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_CheckAllErrors.Image = global::EndpointChecker.Properties.Resources.error;
            this.btn_CheckAllErrors.Location = new System.Drawing.Point(95, 67);
            this.btn_CheckAllErrors.Name = "btn_CheckAllErrors";
            this.btn_CheckAllErrors.Size = new System.Drawing.Size(40, 40);
            this.btn_CheckAllErrors.TabIndex = 16;
            this.btn_CheckAllErrors.UseVisualStyleBackColor = false;
            this.btn_CheckAllErrors.Click += new System.EventHandler(this.btn_CheckAllErrors_Click);
            // 
            // lbl_UncheckAll
            // 
            this.lbl_UncheckAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_UncheckAll.AutoSize = true;
            this.lbl_UncheckAll.Enabled = false;
            this.lbl_UncheckAll.Font = new System.Drawing.Font("Comic Sans MS", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_UncheckAll.Location = new System.Drawing.Point(47, 79);
            this.lbl_UncheckAll.Name = "lbl_UncheckAll";
            this.lbl_UncheckAll.Size = new System.Drawing.Size(42, 16);
            this.lbl_UncheckAll.TabIndex = 20;
            this.lbl_UncheckAll.Text = "NONE";
            // 
            // btn_CheckAllAvailable
            // 
            this.btn_CheckAllAvailable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_CheckAllAvailable.BackColor = System.Drawing.Color.DarkGray;
            this.btn_CheckAllAvailable.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_CheckAllAvailable.Enabled = false;
            this.btn_CheckAllAvailable.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_CheckAllAvailable.Image = global::EndpointChecker.Properties.Resources.passed;
            this.btn_CheckAllAvailable.Location = new System.Drawing.Point(96, 25);
            this.btn_CheckAllAvailable.Name = "btn_CheckAllAvailable";
            this.btn_CheckAllAvailable.Size = new System.Drawing.Size(40, 40);
            this.btn_CheckAllAvailable.TabIndex = 18;
            this.btn_CheckAllAvailable.UseVisualStyleBackColor = false;
            this.btn_CheckAllAvailable.Click += new System.EventHandler(this.btn_CheckAllAvailable_Click);
            // 
            // lbl_CheckAllAvailable
            // 
            this.lbl_CheckAllAvailable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_CheckAllAvailable.AutoSize = true;
            this.lbl_CheckAllAvailable.Enabled = false;
            this.lbl_CheckAllAvailable.Font = new System.Drawing.Font("Comic Sans MS", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_CheckAllAvailable.Location = new System.Drawing.Point(134, 38);
            this.lbl_CheckAllAvailable.Name = "lbl_CheckAllAvailable";
            this.lbl_CheckAllAvailable.Size = new System.Drawing.Size(53, 16);
            this.lbl_CheckAllAvailable.TabIndex = 19;
            this.lbl_CheckAllAvailable.Text = "PASSED";
            // 
            // lbl_ConfigFile
            // 
            this.lbl_ConfigFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_ConfigFile.AutoSize = true;
            this.lbl_ConfigFile.Enabled = false;
            this.lbl_ConfigFile.Font = new System.Drawing.Font("Comic Sans MS", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_ConfigFile.Location = new System.Drawing.Point(900, 431);
            this.lbl_ConfigFile.Name = "lbl_ConfigFile";
            this.lbl_ConfigFile.Size = new System.Drawing.Size(53, 16);
            this.lbl_ConfigFile.TabIndex = 26;
            this.lbl_ConfigFile.Text = "CONFIG";
            // 
            // btn_EndpointsList
            // 
            this.btn_EndpointsList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_EndpointsList.BackColor = System.Drawing.Color.DarkGray;
            this.btn_EndpointsList.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_EndpointsList.Enabled = false;
            this.btn_EndpointsList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_EndpointsList.Image = ((System.Drawing.Image)(resources.GetObject("btn_EndpointsList.Image")));
            this.btn_EndpointsList.Location = new System.Drawing.Point(774, 419);
            this.btn_EndpointsList.Name = "btn_EndpointsList";
            this.btn_EndpointsList.Size = new System.Drawing.Size(40, 40);
            this.btn_EndpointsList.TabIndex = 23;
            this.btn_EndpointsList.UseVisualStyleBackColor = false;
            this.btn_EndpointsList.Click += new System.EventHandler(this.btn_EndpointsList_Click);
            // 
            // btn_ConfigFile
            // 
            this.btn_ConfigFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_ConfigFile.BackColor = System.Drawing.Color.DarkGray;
            this.btn_ConfigFile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_ConfigFile.Enabled = false;
            this.btn_ConfigFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_ConfigFile.Image = global::EndpointChecker.Properties.Resources.appConfig;
            this.btn_ConfigFile.Location = new System.Drawing.Point(861, 419);
            this.btn_ConfigFile.Name = "btn_ConfigFile";
            this.btn_ConfigFile.Size = new System.Drawing.Size(40, 40);
            this.btn_ConfigFile.TabIndex = 24;
            this.btn_ConfigFile.UseVisualStyleBackColor = false;
            this.btn_ConfigFile.Click += new System.EventHandler(this.btn_ConfigFile_Click);
            // 
            // lbl_EndpointsList
            // 
            this.lbl_EndpointsList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_EndpointsList.AutoSize = true;
            this.lbl_EndpointsList.Enabled = false;
            this.lbl_EndpointsList.Font = new System.Drawing.Font("Comic Sans MS", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_EndpointsList.Location = new System.Drawing.Point(813, 431);
            this.lbl_EndpointsList.Name = "lbl_EndpointsList";
            this.lbl_EndpointsList.Size = new System.Drawing.Size(36, 16);
            this.lbl_EndpointsList.TabIndex = 25;
            this.lbl_EndpointsList.Text = "LIST";
            // 
            // lbl_Refresh
            // 
            this.lbl_Refresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_Refresh.AutoSize = true;
            this.lbl_Refresh.Enabled = false;
            this.lbl_Refresh.Font = new System.Drawing.Font("Comic Sans MS", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Refresh.Location = new System.Drawing.Point(1018, 310);
            this.lbl_Refresh.Name = "lbl_Refresh";
            this.lbl_Refresh.Size = new System.Drawing.Size(59, 16);
            this.lbl_Refresh.TabIndex = 39;
            this.lbl_Refresh.Text = "REFRESH";
            // 
            // lbl_Terminate
            // 
            this.lbl_Terminate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_Terminate.AutoSize = true;
            this.lbl_Terminate.Enabled = false;
            this.lbl_Terminate.Font = new System.Drawing.Font("Comic Sans MS", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Terminate.Location = new System.Drawing.Point(1018, 370);
            this.lbl_Terminate.Name = "lbl_Terminate";
            this.lbl_Terminate.Size = new System.Drawing.Size(79, 16);
            this.lbl_Terminate.TabIndex = 40;
            this.lbl_Terminate.Text = "TERMINATE";
            // 
            // lbl_BrowseExportDir
            // 
            this.lbl_BrowseExportDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_BrowseExportDir.AutoSize = true;
            this.lbl_BrowseExportDir.Enabled = false;
            this.lbl_BrowseExportDir.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_BrowseExportDir.Location = new System.Drawing.Point(331, 16);
            this.lbl_BrowseExportDir.Name = "lbl_BrowseExportDir";
            this.lbl_BrowseExportDir.Size = new System.Drawing.Size(85, 15);
            this.lbl_BrowseExportDir.TabIndex = 42;
            this.lbl_BrowseExportDir.Text = "Output Folder";
            // 
            // lbl_SpeedTest
            // 
            this.lbl_SpeedTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_SpeedTest.AutoSize = true;
            this.lbl_SpeedTest.Enabled = false;
            this.lbl_SpeedTest.Font = new System.Drawing.Font("Comic Sans MS", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_SpeedTest.Location = new System.Drawing.Point(1018, 430);
            this.lbl_SpeedTest.Name = "lbl_SpeedTest";
            this.lbl_SpeedTest.Size = new System.Drawing.Size(75, 16);
            this.lbl_SpeedTest.TabIndex = 44;
            this.lbl_SpeedTest.Text = "SPEEDTEST";
            // 
            // imageList_Icons_32pix
            // 
            this.imageList_Icons_32pix.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_Icons_32pix.ImageStream")));
            this.imageList_Icons_32pix.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList_Icons_32pix.Images.SetKeyName(0, "passed.ico");
            this.imageList_Icons_32pix.Images.SetKeyName(1, "error.ico");
            this.imageList_Icons_32pix.Images.SetKeyName(2, "notAvailable.ico");
            this.imageList_Icons_32pix.Images.SetKeyName(3, "notKnown.ico");
            this.imageList_Icons_32pix.Images.SetKeyName(4, "information.ico");
            this.imageList_Icons_32pix.Images.SetKeyName(5, "warning.ico");
            this.imageList_Icons_32pix.Images.SetKeyName(6, "disabled.ico");
            this.imageList_Icons_32pix.Images.SetKeyName(7, "plus.ico");
            this.imageList_Icons_32pix.Images.SetKeyName(8, "radioactive.ico");
            this.imageList_Icons_32pix.Images.SetKeyName(9, "refresh.ico");
            this.imageList_Icons_32pix.Images.SetKeyName(10, "star.ico");
            this.imageList_Icons_32pix.Images.SetKeyName(11, "minus.ico");
            this.imageList_Icons_32pix.Images.SetKeyName(12, "loadingProgressWheel_Frame_1.gif");
            this.imageList_Icons_32pix.Images.SetKeyName(13, "loadingProgressWheel_Frame_2.gif");
            this.imageList_Icons_32pix.Images.SetKeyName(14, "loadingProgressWheel_Frame_3.gif");
            this.imageList_Icons_32pix.Images.SetKeyName(15, "loadingProgressWheel_Frame_4.gif");
            this.imageList_Icons_32pix.Images.SetKeyName(16, "loadingProgressWheel_Frame_5.gif");
            this.imageList_Icons_32pix.Images.SetKeyName(17, "loadingProgressWheel_Frame_6.gif");
            this.imageList_Icons_32pix.Images.SetKeyName(18, "loadingProgressWheel_Frame_7.gif");
            this.imageList_Icons_32pix.Images.SetKeyName(19, "loadingProgressWheel_Frame_8.gif");
            this.imageList_Icons_32pix.Images.SetKeyName(20, "loadingProgressWheel_Frame_9.gif");
            // 
            // num_PingTimeout
            // 
            this.num_PingTimeout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.num_PingTimeout.BackColor = System.Drawing.Color.LightGray;
            this.num_PingTimeout.Cursor = System.Windows.Forms.Cursors.Hand;
            this.num_PingTimeout.Enabled = false;
            this.num_PingTimeout.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.num_PingTimeout.Location = new System.Drawing.Point(150, 323);
            this.num_PingTimeout.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.num_PingTimeout.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_PingTimeout.Name = "num_PingTimeout";
            this.num_PingTimeout.Size = new System.Drawing.Size(47, 23);
            this.num_PingTimeout.TabIndex = 45;
            this.num_PingTimeout.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.num_PingTimeout.ValueChanged += new System.EventHandler(this.num_PingTimeout_ValueChanged);
            // 
            // lbl_PingTimeout
            // 
            this.lbl_PingTimeout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbl_PingTimeout.AutoSize = true;
            this.lbl_PingTimeout.Enabled = false;
            this.lbl_PingTimeout.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_PingTimeout.Location = new System.Drawing.Point(10, 325);
            this.lbl_PingTimeout.Name = "lbl_PingTimeout";
            this.lbl_PingTimeout.Size = new System.Drawing.Size(79, 15);
            this.lbl_PingTimeout.TabIndex = 47;
            this.lbl_PingTimeout.Text = "Ping Timeout";
            // 
            // lbl_PingTimeoutSecondsText
            // 
            this.lbl_PingTimeoutSecondsText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbl_PingTimeoutSecondsText.AutoSize = true;
            this.lbl_PingTimeoutSecondsText.Cursor = System.Windows.Forms.Cursors.Default;
            this.lbl_PingTimeoutSecondsText.Enabled = false;
            this.lbl_PingTimeoutSecondsText.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_PingTimeoutSecondsText.Location = new System.Drawing.Point(201, 325);
            this.lbl_PingTimeoutSecondsText.Name = "lbl_PingTimeoutSecondsText";
            this.lbl_PingTimeoutSecondsText.Size = new System.Drawing.Size(50, 15);
            this.lbl_PingTimeoutSecondsText.TabIndex = 46;
            this.lbl_PingTimeoutSecondsText.Text = "seconds";
            // 
            // lbl_Validate
            // 
            this.lbl_Validate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbl_Validate.AutoSize = true;
            this.lbl_Validate.Enabled = false;
            this.lbl_Validate.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Validate.Location = new System.Drawing.Point(10, 437);
            this.lbl_Validate.Name = "lbl_Validate";
            this.lbl_Validate.Size = new System.Drawing.Size(108, 15);
            this.lbl_Validate.TabIndex = 48;
            this.lbl_Validate.Text = "Validation Method";
            // 
            // comboBox_Validate
            // 
            this.comboBox_Validate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.comboBox_Validate.BackColor = System.Drawing.Color.LightGray;
            this.comboBox_Validate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.comboBox_Validate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Validate.Enabled = false;
            this.comboBox_Validate.FormattingEnabled = true;
            this.comboBox_Validate.Items.AddRange(new object[] {
            "Protocol",
            "Ping"});
            this.comboBox_Validate.Location = new System.Drawing.Point(150, 435);
            this.comboBox_Validate.Name = "comboBox_Validate";
            this.comboBox_Validate.Size = new System.Drawing.Size(95, 23);
            this.comboBox_Validate.TabIndex = 49;
            this.comboBox_Validate.SelectedIndexChanged += new System.EventHandler(this.comboBox_Validate_SelectedIndexChanged);
            // 
            // lv_Endpoints_ContextMenuStrip
            // 
            this.lv_Endpoints_ContextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.lv_Endpoints_ContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_Details,
            this.toolStripSeparator_2,
            this.toolStripMenuItem_Browse,
            this.toolStripMenuItem_AdminBrowse,
            this.toolStripMenuItem_HTTP,
            this.toolStripMenuItem_FTP,
            this.toolStripSeparator_1,
            this.toolStripMenuItem_RDP,
            this.toolStripMenuItem_VNC});
            this.lv_Endpoints_ContextMenuStrip.Name = "lv_Endpoints_ContextMenuStrip";
            this.lv_Endpoints_ContextMenuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.lv_Endpoints_ContextMenuStrip.Size = new System.Drawing.Size(161, 170);
            // 
            // toolStripMenuItem_Details
            // 
            this.toolStripMenuItem_Details.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.toolStripMenuItem_Details.Name = "toolStripMenuItem_Details";
            this.toolStripMenuItem_Details.Size = new System.Drawing.Size(160, 22);
            this.toolStripMenuItem_Details.Text = "Details";
            this.toolStripMenuItem_Details.Click += new System.EventHandler(this.toolStripMenuItem_Details_Click);
            // 
            // toolStripSeparator_2
            // 
            this.toolStripSeparator_2.Name = "toolStripSeparator_2";
            this.toolStripSeparator_2.Size = new System.Drawing.Size(157, 6);
            // 
            // toolStripMenuItem_Browse
            // 
            this.toolStripMenuItem_Browse.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.toolStripMenuItem_Browse.Name = "toolStripMenuItem_Browse";
            this.toolStripMenuItem_Browse.Size = new System.Drawing.Size(160, 22);
            this.toolStripMenuItem_Browse.Text = "Browse";
            this.toolStripMenuItem_Browse.Click += new System.EventHandler(this.toolStripMenuItem_Browse_Click);
            // 
            // toolStripMenuItem_AdminBrowse
            // 
            this.toolStripMenuItem_AdminBrowse.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.toolStripMenuItem_AdminBrowse.Name = "toolStripMenuItem_AdminBrowse";
            this.toolStripMenuItem_AdminBrowse.Size = new System.Drawing.Size(160, 22);
            this.toolStripMenuItem_AdminBrowse.Text = "Admin Browse";
            this.toolStripMenuItem_AdminBrowse.Click += new System.EventHandler(this.toolStripMenuItem_AdminBrowse_Click);
            // 
            // toolStripMenuItem_HTTP
            // 
            this.toolStripMenuItem_HTTP.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.toolStripMenuItem_HTTP.Name = "toolStripMenuItem_HTTP";
            this.toolStripMenuItem_HTTP.Size = new System.Drawing.Size(160, 22);
            this.toolStripMenuItem_HTTP.Text = "Open as HTTP";
            this.toolStripMenuItem_HTTP.Click += new System.EventHandler(this.toolStripMenuItem_HTTP_Click);
            // 
            // toolStripMenuItem_FTP
            // 
            this.toolStripMenuItem_FTP.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.toolStripMenuItem_FTP.Name = "toolStripMenuItem_FTP";
            this.toolStripMenuItem_FTP.Size = new System.Drawing.Size(160, 22);
            this.toolStripMenuItem_FTP.Text = "Open as FTP";
            this.toolStripMenuItem_FTP.Click += new System.EventHandler(this.toolStripMenuItem_FTP_Click);
            // 
            // toolStripSeparator_1
            // 
            this.toolStripSeparator_1.Name = "toolStripSeparator_1";
            this.toolStripSeparator_1.Size = new System.Drawing.Size(157, 6);
            // 
            // toolStripMenuItem_RDP
            // 
            this.toolStripMenuItem_RDP.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.toolStripMenuItem_RDP.Name = "toolStripMenuItem_RDP";
            this.toolStripMenuItem_RDP.Size = new System.Drawing.Size(160, 22);
            this.toolStripMenuItem_RDP.Text = "RDP Connect";
            this.toolStripMenuItem_RDP.Click += new System.EventHandler(this.toolStripMenuItem_RDP_Click);
            // 
            // toolStripMenuItem_VNC
            // 
            this.toolStripMenuItem_VNC.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.toolStripMenuItem_VNC.Name = "toolStripMenuItem_VNC";
            this.toolStripMenuItem_VNC.Size = new System.Drawing.Size(160, 22);
            this.toolStripMenuItem_VNC.Text = "VNC Connect";
            this.toolStripMenuItem_VNC.Click += new System.EventHandler(this.toolStripMenuItem_VNC_Click);
            // 
            // btn_Terminate
            // 
            this.btn_Terminate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Terminate.BackColor = System.Drawing.Color.DarkGray;
            this.btn_Terminate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Terminate.Enabled = false;
            this.btn_Terminate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Terminate.Image = global::EndpointChecker.Properties.Resources.notAvailable;
            this.btn_Terminate.Location = new System.Drawing.Point(979, 357);
            this.btn_Terminate.Name = "btn_Terminate";
            this.btn_Terminate.Size = new System.Drawing.Size(40, 40);
            this.btn_Terminate.TabIndex = 28;
            this.btn_Terminate.UseVisualStyleBackColor = false;
            this.btn_Terminate.Click += new System.EventHandler(this.btn_Terminate_Click);
            // 
            // btn_Refresh
            // 
            this.btn_Refresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Refresh.BackColor = System.Drawing.Color.DarkGray;
            this.btn_Refresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Refresh.Enabled = false;
            this.btn_Refresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Refresh.Image = global::EndpointChecker.Properties.Resources.refresh;
            this.btn_Refresh.Location = new System.Drawing.Point(979, 297);
            this.btn_Refresh.Name = "btn_Refresh";
            this.btn_Refresh.Size = new System.Drawing.Size(40, 40);
            this.btn_Refresh.TabIndex = 1;
            this.btn_Refresh.UseVisualStyleBackColor = false;
            this.btn_Refresh.Click += new System.EventHandler(this.btn_Refresh_Click);
            // 
            // btn_SpeedTest
            // 
            this.btn_SpeedTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_SpeedTest.BackColor = System.Drawing.Color.DarkGray;
            this.btn_SpeedTest.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_SpeedTest.Enabled = false;
            this.btn_SpeedTest.FlatAppearance.BorderColor = System.Drawing.Color.Lime;
            this.btn_SpeedTest.FlatAppearance.BorderSize = 2;
            this.btn_SpeedTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_SpeedTest.Image = ((System.Drawing.Image)(resources.GetObject("btn_SpeedTest.Image")));
            this.btn_SpeedTest.Location = new System.Drawing.Point(979, 417);
            this.btn_SpeedTest.Name = "btn_SpeedTest";
            this.btn_SpeedTest.Size = new System.Drawing.Size(40, 40);
            this.btn_SpeedTest.TabIndex = 43;
            this.btn_SpeedTest.UseVisualStyleBackColor = false;
            this.btn_SpeedTest.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Btn_SpeedTest_MouseClick);
            // 
            // btn_BrowseExportDir
            // 
            this.btn_BrowseExportDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_BrowseExportDir.BackColor = System.Drawing.Color.LightGray;
            this.btn_BrowseExportDir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_BrowseExportDir.Enabled = false;
            this.btn_BrowseExportDir.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_BrowseExportDir.Image = global::EndpointChecker.Properties.Resources.browseFolder_16x16;
            this.btn_BrowseExportDir.Location = new System.Drawing.Point(308, 14);
            this.btn_BrowseExportDir.Name = "btn_BrowseExportDir";
            this.btn_BrowseExportDir.Size = new System.Drawing.Size(20, 20);
            this.btn_BrowseExportDir.TabIndex = 41;
            this.btn_BrowseExportDir.UseVisualStyleBackColor = false;
            this.btn_BrowseExportDir.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btn_BrowseExportDir_MouseClick);
            // 
            // pb_Progress
            // 
            this.pb_Progress.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.pb_Progress.Image = global::EndpointChecker.Properties.Resources.loadingProgressSmall;
            this.pb_Progress.Location = new System.Drawing.Point(445, 464);
            this.pb_Progress.Name = "pb_Progress";
            this.pb_Progress.Size = new System.Drawing.Size(219, 22);
            this.pb_Progress.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_Progress.TabIndex = 14;
            this.pb_Progress.TabStop = false;
            this.pb_Progress.Visible = false;
            // 
            // pb_Progress_Init
            // 
            this.pb_Progress_Init.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pb_Progress_Init.Image = global::EndpointChecker.Properties.Resources.loadingProgress;
            this.pb_Progress_Init.Location = new System.Drawing.Point(461, 56);
            this.pb_Progress_Init.Name = "pb_Progress_Init";
            this.pb_Progress_Init.Size = new System.Drawing.Size(186, 186);
            this.pb_Progress_Init.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_Progress_Init.TabIndex = 5;
            this.pb_Progress_Init.TabStop = false;
            // 
            // cb_ExportEndpointsStatus_HTML
            // 
            this.cb_ExportEndpointsStatus_HTML.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cb_ExportEndpointsStatus_HTML.AutoSize = true;
            this.cb_ExportEndpointsStatus_HTML.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cb_ExportEndpointsStatus_HTML.Enabled = false;
            this.cb_ExportEndpointsStatus_HTML.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_ExportEndpointsStatus_HTML.Location = new System.Drawing.Point(161, 15);
            this.cb_ExportEndpointsStatus_HTML.Name = "cb_ExportEndpointsStatus_HTML";
            this.cb_ExportEndpointsStatus_HTML.Size = new System.Drawing.Size(57, 19);
            this.cb_ExportEndpointsStatus_HTML.TabIndex = 50;
            this.cb_ExportEndpointsStatus_HTML.Text = "HTML";
            this.cb_ExportEndpointsStatus_HTML.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cb_ExportEndpointsStatus_HTML.UseVisualStyleBackColor = true;
            this.cb_ExportEndpointsStatus_HTML.CheckedChanged += new System.EventHandler(this.cb_ExportEndpointsStatus_HTML_CheckedChanged);
            // 
            // lbl_LastUpdate_Label
            // 
            this.lbl_LastUpdate_Label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbl_LastUpdate_Label.AutoSize = true;
            this.lbl_LastUpdate_Label.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_LastUpdate_Label.Location = new System.Drawing.Point(10, 468);
            this.lbl_LastUpdate_Label.Name = "lbl_LastUpdate_Label";
            this.lbl_LastUpdate_Label.Size = new System.Drawing.Size(78, 15);
            this.lbl_LastUpdate_Label.TabIndex = 51;
            this.lbl_LastUpdate_Label.Text = "LAST UPDATE";
            this.lbl_LastUpdate_Label.Visible = false;
            // 
            // lbl_LastUpdate
            // 
            this.lbl_LastUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbl_LastUpdate.AutoSize = true;
            this.lbl_LastUpdate.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_LastUpdate.Location = new System.Drawing.Point(148, 466);
            this.lbl_LastUpdate.Name = "lbl_LastUpdate";
            this.lbl_LastUpdate.Size = new System.Drawing.Size(0, 18);
            this.lbl_LastUpdate.TabIndex = 52;
            this.lbl_LastUpdate.Visible = false;
            // 
            // cb_ExportEndpointsStatus_JSON
            // 
            this.cb_ExportEndpointsStatus_JSON.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cb_ExportEndpointsStatus_JSON.AutoSize = true;
            this.cb_ExportEndpointsStatus_JSON.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cb_ExportEndpointsStatus_JSON.Enabled = false;
            this.cb_ExportEndpointsStatus_JSON.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_ExportEndpointsStatus_JSON.Location = new System.Drawing.Point(224, 15);
            this.cb_ExportEndpointsStatus_JSON.Name = "cb_ExportEndpointsStatus_JSON";
            this.cb_ExportEndpointsStatus_JSON.Size = new System.Drawing.Size(54, 19);
            this.cb_ExportEndpointsStatus_JSON.TabIndex = 53;
            this.cb_ExportEndpointsStatus_JSON.Text = "JSON";
            this.cb_ExportEndpointsStatus_JSON.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cb_ExportEndpointsStatus_JSON.UseVisualStyleBackColor = true;
            this.cb_ExportEndpointsStatus_JSON.CheckedChanged += new System.EventHandler(this.cb_ExportEndpointsStatus_JSON_CheckedChanged);
            // 
            // cb_ExportEndpointsStatus_XML
            // 
            this.cb_ExportEndpointsStatus_XML.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cb_ExportEndpointsStatus_XML.AutoSize = true;
            this.cb_ExportEndpointsStatus_XML.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cb_ExportEndpointsStatus_XML.Enabled = false;
            this.cb_ExportEndpointsStatus_XML.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_ExportEndpointsStatus_XML.Location = new System.Drawing.Point(103, 15);
            this.cb_ExportEndpointsStatus_XML.Name = "cb_ExportEndpointsStatus_XML";
            this.cb_ExportEndpointsStatus_XML.Size = new System.Drawing.Size(50, 19);
            this.cb_ExportEndpointsStatus_XML.TabIndex = 54;
            this.cb_ExportEndpointsStatus_XML.Text = "XML";
            this.cb_ExportEndpointsStatus_XML.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cb_ExportEndpointsStatus_XML.UseVisualStyleBackColor = true;
            this.cb_ExportEndpointsStatus_XML.CheckedChanged += new System.EventHandler(this.cb_ExportEndpointsStatus_XML_CheckedChanged);
            // 
            // groupBox_Export
            // 
            this.groupBox_Export.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox_Export.Controls.Add(this.cb_ExportEndpointsStatus_HTML);
            this.groupBox_Export.Controls.Add(this.cb_ExportEndpointsStatus_XML);
            this.groupBox_Export.Controls.Add(this.cb_ExportEndpointsStatus_XLSX);
            this.groupBox_Export.Controls.Add(this.cb_ExportEndpointsStatus_JSON);
            this.groupBox_Export.Controls.Add(this.btn_BrowseExportDir);
            this.groupBox_Export.Controls.Add(this.lbl_BrowseExportDir);
            this.groupBox_Export.Enabled = false;
            this.groupBox_Export.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold);
            this.groupBox_Export.Location = new System.Drawing.Point(283, 419);
            this.groupBox_Export.Name = "groupBox_Export";
            this.groupBox_Export.Size = new System.Drawing.Size(468, 40);
            this.groupBox_Export.TabIndex = 55;
            this.groupBox_Export.TabStop = false;
            this.groupBox_Export.Text = "Status Export";
            // 
            // TIMER_TrayIconAnimation
            // 
            this.TIMER_TrayIconAnimation.Interval = 50;
            this.TIMER_TrayIconAnimation.Tick += new System.EventHandler(this.TIMER_TrayIconAnimation_Tick);
            // 
            // cb_RemoveURLParameters
            // 
            this.cb_RemoveURLParameters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cb_RemoveURLParameters.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cb_RemoveURLParameters.Enabled = false;
            this.cb_RemoveURLParameters.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_RemoveURLParameters.Location = new System.Drawing.Point(10, 50);
            this.cb_RemoveURLParameters.Name = "cb_RemoveURLParameters";
            this.cb_RemoveURLParameters.Size = new System.Drawing.Size(207, 19);
            this.cb_RemoveURLParameters.TabIndex = 56;
            this.cb_RemoveURLParameters.Text = "Remove URL Parameters";
            this.cb_RemoveURLParameters.UseVisualStyleBackColor = true;
            this.cb_RemoveURLParameters.CheckedChanged += new System.EventHandler(this.cb_RemoveURLParameters_CheckedChanged);
            // 
            // groupBox_CommonOptions
            // 
            this.groupBox_CommonOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox_CommonOptions.Controls.Add(this.cb_AutomaticRefresh);
            this.groupBox_CommonOptions.Controls.Add(this.cb_TrayBalloonNotify);
            this.groupBox_CommonOptions.Controls.Add(this.cb_RefreshAutoSet);
            this.groupBox_CommonOptions.Controls.Add(this.cb_ContinuousRefresh);
            this.groupBox_CommonOptions.Controls.Add(this.cb_ResolveNetworkShares);
            this.groupBox_CommonOptions.Enabled = false;
            this.groupBox_CommonOptions.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.groupBox_CommonOptions.Location = new System.Drawing.Point(283, 290);
            this.groupBox_CommonOptions.Name = "groupBox_CommonOptions";
            this.groupBox_CommonOptions.Size = new System.Drawing.Size(229, 123);
            this.groupBox_CommonOptions.TabIndex = 57;
            this.groupBox_CommonOptions.TabStop = false;
            this.groupBox_CommonOptions.Text = "Common Options";
            // 
            // cb_ResolvePageLinks
            // 
            this.cb_ResolvePageLinks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cb_ResolvePageLinks.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cb_ResolvePageLinks.Enabled = false;
            this.cb_ResolvePageLinks.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_ResolvePageLinks.Location = new System.Drawing.Point(10, 84);
            this.cb_ResolvePageLinks.Name = "cb_ResolvePageLinks";
            this.cb_ResolvePageLinks.Size = new System.Drawing.Size(207, 19);
            this.cb_ResolvePageLinks.TabIndex = 59;
            this.cb_ResolvePageLinks.Text = "Resolve HTML Page Links";
            this.cb_ResolvePageLinks.UseVisualStyleBackColor = true;
            this.cb_ResolvePageLinks.CheckedChanged += new System.EventHandler(this.cb_ResolvePageLinks_CheckedChanged);
            // 
            // groupBox_HTTPOptions
            // 
            this.groupBox_HTTPOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox_HTTPOptions.Controls.Add(this.cb_ResolvePageLinks);
            this.groupBox_HTTPOptions.Controls.Add(this.cb_AllowAutoRedirect);
            this.groupBox_HTTPOptions.Controls.Add(this.cb_ValidateSSLCertificate);
            this.groupBox_HTTPOptions.Controls.Add(this.cb_ResolvePageMetaInfo);
            this.groupBox_HTTPOptions.Controls.Add(this.cb_SaveResponse);
            this.groupBox_HTTPOptions.Controls.Add(this.cb_RemoveURLParameters);
            this.groupBox_HTTPOptions.Enabled = false;
            this.groupBox_HTTPOptions.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.groupBox_HTTPOptions.Location = new System.Drawing.Point(526, 290);
            this.groupBox_HTTPOptions.Name = "groupBox_HTTPOptions";
            this.groupBox_HTTPOptions.Size = new System.Drawing.Size(225, 122);
            this.groupBox_HTTPOptions.TabIndex = 59;
            this.groupBox_HTTPOptions.TabStop = false;
            this.groupBox_HTTPOptions.Text = "HTTP Options";
            // 
            // pb_ITNetwork
            // 
            this.pb_ITNetwork.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pb_ITNetwork.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pb_ITNetwork.Image = global::EndpointChecker.Properties.Resources.itnetwork_logo_text;
            this.pb_ITNetwork.Location = new System.Drawing.Point(979, 470);
            this.pb_ITNetwork.Name = "pb_ITNetwork";
            this.pb_ITNetwork.Size = new System.Drawing.Size(123, 30);
            this.pb_ITNetwork.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_ITNetwork.TabIndex = 60;
            this.pb_ITNetwork.TabStop = false;
            this.pb_ITNetwork.Click += new System.EventHandler(this.pb_ITNetwork_Click);
            // 
            // link_AuthorMail
            // 
            this.link_AuthorMail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.link_AuthorMail.AutoSize = true;
            this.link_AuthorMail.Cursor = System.Windows.Forms.Cursors.Hand;
            this.link_AuthorMail.Font = new System.Drawing.Font("Arial", 9.75F);
            this.link_AuthorMail.Location = new System.Drawing.Point(146, 503);
            this.link_AuthorMail.Name = "link_AuthorMail";
            this.link_AuthorMail.Size = new System.Drawing.Size(85, 16);
            this.link_AuthorMail.TabIndex = 61;
            this.link_AuthorMail.TabStop = true;
            this.link_AuthorMail.Text = "Peter Machaj";
            this.link_AuthorMail.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.link_AuthorMail_LinkClicked);
            // 
            // pb_FeatureRequest
            // 
            this.pb_FeatureRequest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pb_FeatureRequest.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pb_FeatureRequest.Image = ((System.Drawing.Image)(resources.GetObject("pb_FeatureRequest.Image")));
            this.pb_FeatureRequest.Location = new System.Drawing.Point(912, 466);
            this.pb_FeatureRequest.Name = "pb_FeatureRequest";
            this.pb_FeatureRequest.Size = new System.Drawing.Size(30, 30);
            this.pb_FeatureRequest.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_FeatureRequest.TabIndex = 62;
            this.pb_FeatureRequest.TabStop = false;
            this.pb_FeatureRequest.Click += new System.EventHandler(this.pb_FeatureRequest_Click);
            // 
            // TIMER_ListAndLogsFilesWatcher
            // 
            this.TIMER_ListAndLogsFilesWatcher.Enabled = true;
            this.TIMER_ListAndLogsFilesWatcher.Interval = 1000;
            this.TIMER_ListAndLogsFilesWatcher.Tick += new System.EventHandler(this.TIMER_ListAndLogsFilesWatcher_Tick);
            // 
            // pb_GitHub
            // 
            this.pb_GitHub.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pb_GitHub.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pb_GitHub.Image = ((System.Drawing.Image)(resources.GetObject("pb_GitHub.Image")));
            this.pb_GitHub.Location = new System.Drawing.Point(934, 469);
            this.pb_GitHub.Name = "pb_GitHub";
            this.pb_GitHub.Size = new System.Drawing.Size(53, 30);
            this.pb_GitHub.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_GitHub.TabIndex = 63;
            this.pb_GitHub.TabStop = false;
            this.pb_GitHub.Click += new System.EventHandler(this.pb_GitHub_Click);
            // 
            // CheckerMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(1108, 522);
            this.Controls.Add(this.pb_FeatureRequest);
            this.Controls.Add(this.pb_ITNetwork);
            this.Controls.Add(this.pb_GitHub);
            this.Controls.Add(this.lbl_ConfigFile);
            this.Controls.Add(this.btn_EndpointsList);
            this.Controls.Add(this.lbl_EndpointsList);
            this.Controls.Add(this.btn_ConfigFile);
            this.Controls.Add(this.link_AuthorMail);
            this.Controls.Add(this.groupBox_HTTPOptions);
            this.Controls.Add(this.groupBox_CommonOptions);
            this.Controls.Add(this.groupBox_Export);
            this.Controls.Add(this.lbl_LastUpdate);
            this.Controls.Add(this.lbl_LastUpdate_Label);
            this.Controls.Add(this.comboBox_Validate);
            this.Controls.Add(this.lbl_Validate);
            this.Controls.Add(this.num_PingTimeout);
            this.Controls.Add(this.lbl_PingTimeout);
            this.Controls.Add(this.lbl_PingTimeoutSecondsText);
            this.Controls.Add(this.btn_SpeedTest);
            this.Controls.Add(this.lbl_SpeedTest);
            this.Controls.Add(this.lbl_Terminate);
            this.Controls.Add(this.lbl_Refresh);
            this.Controls.Add(this.groupBox_EndpointSelection);
            this.Controls.Add(this.num_ParallelThreadsCount);
            this.Controls.Add(this.lbl_ParallelThreadsCount);
            this.Controls.Add(this.num_FTPRequestTimeout);
            this.Controls.Add(this.lbl_FTPRequestTimeout);
            this.Controls.Add(this.lbl_FTPRequestTimeoutSecondsText);
            this.Controls.Add(this.lbl_AutomaticRefresh);
            this.Controls.Add(this.num_HTTPRequestTimeout);
            this.Controls.Add(this.lbl_RequestTimeout);
            this.Controls.Add(this.lbl_RequestTimeoutSecondsText);
            this.Controls.Add(this.lbl_TimerIntervalMinutesText);
            this.Controls.Add(this.num_RefreshInterval);
            this.Controls.Add(this.btn_Terminate);
            this.Controls.Add(this.lbl_ProgressCount);
            this.Controls.Add(this.pb_Progress);
            this.Controls.Add(this.lbl_Version);
            this.Controls.Add(this.lbl_Copyright);
            this.Controls.Add(this.pb_Progress_Init);
            this.Controls.Add(this.btn_Refresh);
            this.Controls.Add(this.lv_Endpoints);
            this.Controls.Add(this.lbl_NoEndpoints);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(1124, 528);
            this.Name = "CheckerMainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Endpoint Status Checker v";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CheckerMainForm_FormClosing);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.num_RefreshInterval)).EndInit();
            this.trayContextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.num_HTTPRequestTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_FTPRequestTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_ParallelThreadsCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.instanceWatcher)).EndInit();
            this.groupBox_EndpointSelection.ResumeLayout(false);
            this.groupBox_EndpointSelection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_PingTimeout)).EndInit();
            this.lv_Endpoints_ContextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pb_Progress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Progress_Init)).EndInit();
            this.groupBox_Export.ResumeLayout(false);
            this.groupBox_Export.PerformLayout();
            this.groupBox_CommonOptions.ResumeLayout(false);
            this.groupBox_HTTPOptions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pb_ITNetwork)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_FeatureRequest)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_GitHub)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.ComponentModel.BackgroundWorker bw_GetStatus;
        public System.Windows.Forms.Timer TIMER_Refresh;
        public System.Windows.Forms.NumericUpDown num_RefreshInterval;
        public System.Windows.Forms.Label lbl_TimerIntervalMinutesText;
        public System.Windows.Forms.ColumnHeader ch_EndpointName;
        public System.Windows.Forms.ColumnHeader ch_Code;
        public System.Windows.Forms.ColumnHeader ch_EndpointURL;
        public System.Windows.Forms.Label lbl_NoEndpoints;
        public System.Windows.Forms.NotifyIcon trayIcon;
        public System.Windows.Forms.ContextMenuStrip trayContextMenu;
        public System.Windows.Forms.ToolStripMenuItem tray_Refresh;
        public System.Windows.Forms.ToolStripMenuItem tray_Exit;
        public System.Windows.Forms.Label lbl_RequestTimeoutSecondsText;
        public System.Windows.Forms.NumericUpDown num_HTTPRequestTimeout;
        public System.Windows.Forms.ColumnHeader ch_Port;
        public System.Windows.Forms.ColumnHeader ch_Protocol;
        public System.Windows.Forms.Button btn_UncheckAll;
        public System.Windows.Forms.Button btn_CheckAllErrors;
        public System.Windows.Forms.Button btn_CheckAll;
        public System.Windows.Forms.Button btn_CheckAllAvailable;
        public System.Windows.Forms.Label lbl_ProgressCount;
        public System.Windows.Forms.NumericUpDown num_FTPRequestTimeout;
        public System.Windows.Forms.Label lbl_FTPRequestTimeoutSecondsText;
        public System.Windows.Forms.ColumnHeader ch_ResponseTime;
        public System.Windows.Forms.ColumnHeader ch_Message;
        public System.Windows.Forms.NumericUpDown num_ParallelThreadsCount;
        public System.Windows.Forms.ColumnHeader ch_IPAddress;
        public System.IO.FileSystemWatcher instanceWatcher;
        public System.Windows.Forms.ColumnHeader ch_PingTime;
        public System.Windows.Forms.ColumnHeader ch_Server;
        public System.Windows.Forms.ColumnHeader ch_UserName;
        public System.Windows.Forms.ColumnHeader ch_NetworkShares;
        public System.Windows.Forms.FolderBrowserDialog folderBrowserExportDir;
        public System.Windows.Forms.ColumnHeader ch_DNSName;
        public System.Windows.Forms.Timer TIMER_StartupRefresh;
        public System.Windows.Forms.ColumnHeader ch_HTTPContentLenght;
        public System.ComponentModel.BackgroundWorker bw_LoadEndpointReferences;
        public System.Windows.Forms.Button btn_SpeedTest;
        public System.Windows.Forms.Button btn_BrowseExportDir;
        public System.Windows.Forms.ColumnHeader ch_HTTPContentType;
        public System.Windows.Forms.NumericUpDown num_PingTimeout;
        public System.Windows.Forms.Label lbl_PingTimeoutSecondsText;
        public System.Windows.Forms.ComboBox comboBox_Validate;
        public System.Windows.Forms.ColumnHeader ch_MACAddress;
        public System.Windows.Forms.ContextMenuStrip lv_Endpoints_ContextMenuStrip;
        public System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Details;
        public System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Browse;
        public System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_HTTP;
        public System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_FTP;
        public System.Windows.Forms.ImageList imageList_Icons_32pix;
        public System.Windows.Forms.ImageList imageList_ListViewIcons_20pix;
        public System.Windows.Forms.Timer TIMER_TrayIconAnimation;
        public System.Windows.Forms.ColumnHeader ch_LastSeenOnline;
        public System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_AdminBrowse;
        public System.Windows.Forms.ColumnHeader ch_HTTPExpires;
        public System.Windows.Forms.GroupBox groupBox_CommonOptions;
        public System.Windows.Forms.CheckBox cb_RemoveURLParameters;
        public System.Windows.Forms.ListView lv_Endpoints;
        public System.Windows.Forms.Button btn_Refresh;
        public System.Windows.Forms.CheckBox cb_AutomaticRefresh;
        public System.Windows.Forms.PictureBox pb_Progress_Init;
        public System.Windows.Forms.CheckBox cb_TrayBalloonNotify;
        public System.Windows.Forms.ToolStripSeparator tray_Separator;
        public System.Windows.Forms.Label lbl_RequestTimeout;
        public System.Windows.Forms.CheckBox cb_AllowAutoRedirect;
        public System.Windows.Forms.Label lbl_Copyright;
        public System.Windows.Forms.Label lbl_Version;
        public System.Windows.Forms.PictureBox pb_Progress;
        public System.Windows.Forms.Label lbl_AutomaticRefresh;
        public System.Windows.Forms.Label lbl_FTPRequestTimeout;
        public System.Windows.Forms.CheckBox cb_RefreshAutoSet;
        public System.Windows.Forms.CheckBox cb_ContinuousRefresh;
        public System.Windows.Forms.Button btn_Terminate;
        public System.Windows.Forms.Label lbl_ParallelThreadsCount;
        public System.Windows.Forms.CheckBox cb_ResolveNetworkShares;
        public System.Windows.Forms.CheckBox cb_ExportEndpointsStatus_XLSX;
        public System.Windows.Forms.CheckBox cb_SaveResponse;
        public System.Windows.Forms.CheckBox cb_ResolvePageMetaInfo;
        public System.Windows.Forms.CheckBox cb_ValidateSSLCertificate;
        public System.Windows.Forms.Label lbl_Terminate;
        public System.Windows.Forms.Label lbl_Refresh;
        public System.Windows.Forms.GroupBox groupBox_EndpointSelection;
        public System.Windows.Forms.Label lbl_CheckAllErrors;
        public System.Windows.Forms.Label lbl_CheckAll;
        public System.Windows.Forms.Label lbl_UncheckAll;
        public System.Windows.Forms.Label lbl_CheckAllAvailable;
        public System.Windows.Forms.Label lbl_SpeedTest;
        public System.Windows.Forms.Label lbl_BrowseExportDir;
        public System.Windows.Forms.Label lbl_PingTimeout;
        public System.Windows.Forms.Label lbl_Validate;
        public System.Windows.Forms.ToolStripSeparator toolStripSeparator_2;
        public System.Windows.Forms.ToolStripSeparator toolStripSeparator_1;
        public System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_RDP;
        public System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_VNC;
        public System.Windows.Forms.CheckBox cb_ExportEndpointsStatus_HTML;
        public System.Windows.Forms.Label lbl_LastUpdate;
        public System.Windows.Forms.Label lbl_LastUpdate_Label;
        public System.Windows.Forms.GroupBox groupBox_Export;
        public System.Windows.Forms.CheckBox cb_ExportEndpointsStatus_XML;
        public System.Windows.Forms.CheckBox cb_ExportEndpointsStatus_JSON;
        public System.Windows.Forms.CheckBox cb_ResolvePageLinks;
        private System.Windows.Forms.GroupBox groupBox_HTTPOptions;
        private System.Windows.Forms.PictureBox pb_ITNetwork;
        public System.Windows.Forms.ColumnHeader ch_HTTPETag;
        public System.Windows.Forms.LinkLabel link_AuthorMail;
        public System.Windows.Forms.PictureBox pb_FeatureRequest;
        public System.Windows.Forms.Label lbl_ConfigFile;
        public System.Windows.Forms.Button btn_EndpointsList;
        public System.Windows.Forms.Button btn_ConfigFile;
        public System.Windows.Forms.Label lbl_EndpointsList;
        public System.Windows.Forms.Timer TIMER_ListAndLogsFilesWatcher;
        private System.Windows.Forms.PictureBox pb_GitHub;
    }
}

