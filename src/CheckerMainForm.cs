using ArpLookup;
using ClosedXML.Excel;
using EndpointChecker.Properties;
using Flurl;
using HtmlAgilityPack;
using Microsoft.Win32;
using Newtonsoft.Json;
using Spire.Xls;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Cache;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using static EndpointChecker.Program;

namespace EndpointChecker
{
    public partial class CheckerMainForm : Form
    {
        // THIS PREVENTS GDI+ GENERIC ERROR WHEN GETTING ICON BY INDEX FROM IMAGELIST
        [DllImport("user32.dll", EntryPoint = "DestroyIcon")]
        public static extern bool DestroyIcon(IntPtr hIcon);

        // FOR 'FLUSH LOCAL DNS CACHE' PURPOSE
        [DllImport("dnsapi.dll", EntryPoint = "DnsFlushResolverCache")]
        private static extern uint DnsFlushResolverCache();

        // CUSTOM VALIDATION METHOD ENUM
        public enum ValidationMethod
        {
            Protocol = 0,
            Ping = 1
        }

        // CUSTOM LISTVIEW REFRESH METHOD ENUM
        public enum ListViewRefreshMethod
        {
            CurrentState,
            CheckAll,
            UncheckAll,
            CheckAllPassed,
            CheckAllFailed
        };

        // CUSTOM ENDPOINT STATUS DEFINITIONS ENUM
        public enum EndpointStatus
        {
            [Description("Ping Check Only")]
            PINGCHECK = 1,
            [Description("Not Checked (Endpoint Disabled)")]
            DISABLED = 2,
            [Description("Not Checked (Terminated)")]
            TERMINATED = 3,
            [Description("Not Checked Yet")]
            NOTCHECKED = 4
        };


        // THIS SWITCH INDICATES THAT TRAY ICON BALLOON TOOLTIP IS ACTUALLY DISPLAYED
        private bool balloonVisible = false;

        // VARIABLES FOR TRAY ICON ANIMATION PURPOSES
        private int trayAnimation_Index = 0;
        private readonly List<Icon> trayAnimation_Icons = new List<Icon>();

        // THIS SWITCH INDICATES THAT ENDPOINTS LISTVIEW IS ACTUALLY UPDATING
        private bool listUpdating = false;

        // THIS SWITCH INDICATES STATE OF APPLICATION CLOSE
        private bool onClose = false;

        // VNC VIEWER EXECUTABLE [FOR 'FTP' CONNECTION PURPOSE]
        public static string appExecutable_VNC = string.Empty;

        // PUTTY EXECUTABLE [FOR 'SSH' CONNECTION PURPOSE]
        public static string appExecutable_Putty = string.Empty;

        // ENDPOINTS STATUS EXPORT FILES STREAMS [FOR EXCLUSIVE LOCK PURPOSE]
        private FileStream definitionsStatusExport_JSON_FileStream = null;
        private FileStream definitionsStatusExport_XLSX_FileStream = null;
        private FileStream definitionsStatusExport_XML_FileStream = null;
        private FileStream definitionsStatusExport_HTML_Info_FileStream = null;
        private FileStream definitionsStatusExport_HTML_HTTP_FileStream = null;
        private FileStream definitionsStatusExport_HTML_FTP_FileStream = null;

        // WORKING LIST OF ENDPOINTS
        private List<EndpointDefinition> endpointsList = new List<EndpointDefinition>();

        // WORKING LIST OF DISABLED ENDPOINTS
        private List<string> endpointsList_Disabled = new List<string>();

        // WORKING LIST OF 'LAST SEEN ONLINE' VALUES OF ENDPOINTS
        private Dictionary<string, string> endpointsList_LastSeenOnline = new Dictionary<string, string>();

        // ENDPOINTS LISTVIEW TOPITEM INDEX [FOR PRESERVING SCROLLED POSITION AFTER LIST UPDATE]
        private int lv_Endpoints_TopItemIndex = 0;

        // ENDPOINTS LISTVIEW SELECTED ITEM(S) INDEXES [FOR PRESERVING ITEM(S) SELECTION AFTER LIST UPDATE]
        private readonly List<int> lv_Endpoints_SelectedItems = new List<int>();

        // ENDPOINTS LISTVIEW SELECTED ITEM(S) LIST
        public static List<EndpointDefinition> lv_Endpoints_SelectedEndpointsList = new List<EndpointDefinition>();

        // GET LOCAL GATEWAY IP AND MAC ADDRESSES
        public static List<string> localDNSAndGWIPAddresses;
        public static List<string> localDNSAndGWMACAddresses;

        // SELECTED VALIDATION METHOD
        public static ValidationMethod validationMethod;

        // ACTIVE .NET FRAMEWORK VERSION
        private readonly TargetFrameworkAttribute dotNetFramework_TargetVersion =
            (TargetFrameworkAttribute)Assembly.GetExecutingAssembly().
            GetCustomAttributes(typeof(TargetFrameworkAttribute), false).
            SingleOrDefault();

        // ENDPOINT DETAILS DIALOG INSTANCE
        private EndpointDetailsDialog dialog_EndpointDetails = null;

        // SPEEDTEST DIALOG INSTANCE
        private SpeedTestDialog dialog_SpeedTest = null;
        private readonly System.Windows.Forms.Timer premiumUiPulseTimer = new System.Windows.Forms.Timer();
        private int premiumUiPulseTick = 0;
        private GroupBox groupBox_ListOptions;
        private ContextMenuStrip endpointColumnsContextMenu;
        private readonly Dictionary<ColumnHeader, int> endpointColumnWidths = new Dictionary<ColumnHeader, int>();
        private bool adjustingEndpointFillerColumn = false;
        private Button btn_ColumnsChooser;
        private Label lbl_ColumnsChooser;
        private Panel endpointHeaderCornerPatch;
        private GroupBox groupBox_ScanProgress;

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        public CheckerMainForm()
        {
            InitializeComponent();

            // COMMON EXCEPTION HANDLERS
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);
            Application.ThreadException += new ThreadExceptionEventHandler(ThreadExceptionHandler);

            // SET DOUBLE BUFFER
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            // THREAD POOL SETTINGS
            ThreadPool.GetMinThreads(out int minWorker, out int minIOC);
            ThreadPool.SetMinThreads(100, minIOC);

            // Ensure the system proxy uses Windows credentials — required in .NET 5+ where
            // SocketsHttpHandler backs HttpWebRequest and may not inherit proxy auth automatically.
            if (WebRequest.DefaultWebProxy != null)
                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultNetworkCredentials;

            // MAIN PROCESS PRIORITY
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

            // GET LOCAL DNS AND GATEWAY SERVERS IP AND MAC ADDRESSES [ON BACKGROUND]
            NewBackgroundThread(() =>
            {
                GetLocalDNSAndGWAddresses(out localDNSAndGWIPAddresses, out localDNSAndGWMACAddresses);
            });

            // ASSIGN RESIZED IMAGES TO ENDPOINT LIST CONTEXT MENU STRIP ITEMS
            toolStripMenuItem_AdminBrowse.Image = ResizeImage(Resources.browse_Admin_Share, lv_Endpoints_ContextMenuStrip.ImageScalingSize.Width, lv_Endpoints_ContextMenuStrip.ImageScalingSize.Height);
            toolStripMenuItem_Browse.Image = ResizeImage(Resources.browse_Share, lv_Endpoints_ContextMenuStrip.ImageScalingSize.Width, lv_Endpoints_ContextMenuStrip.ImageScalingSize.Height);
            toolStripMenuItem_Details.Image = ResizeImage(Resources.information.ToBitmap(), lv_Endpoints_ContextMenuStrip.ImageScalingSize.Width, lv_Endpoints_ContextMenuStrip.ImageScalingSize.Height);
            toolStripMenuItem_FTP.Image = ResizeImage(Resources.browse_FTP, lv_Endpoints_ContextMenuStrip.ImageScalingSize.Width, lv_Endpoints_ContextMenuStrip.ImageScalingSize.Height);
            toolStripMenuItem_HTTP.Image = ResizeImage(Resources.browse_HTTP, lv_Endpoints_ContextMenuStrip.ImageScalingSize.Width, lv_Endpoints_ContextMenuStrip.ImageScalingSize.Height);
            toolStripMenuItem_RDP.Image = ResizeImage(Resources.connect_RDP, lv_Endpoints_ContextMenuStrip.ImageScalingSize.Width, lv_Endpoints_ContextMenuStrip.ImageScalingSize.Height);
            toolStripMenuItem_VNC.Image = ResizeImage(Resources.connect_VNC, lv_Endpoints_ContextMenuStrip.ImageScalingSize.Width, lv_Endpoints_ContextMenuStrip.ImageScalingSize.Height);
            toolStripMenuItem_SSH.Image = ResizeImage(Resources.ssh_2, lv_Endpoints_ContextMenuStrip.ImageScalingSize.Width, lv_Endpoints_ContextMenuStrip.ImageScalingSize.Height);

            // ASSIGN RESIZED IMAGES TO TRAY CONTEXT MENU STRIP ITEMS
            tray_Notifications_Enable.Image = ResizeImage(Resources.notifications_ON.ToBitmap(), trayContextMenu.ImageScalingSize.Width, trayContextMenu.ImageScalingSize.Height);
            tray_Notifications_Disable.Image = ResizeImage(Resources.notifications_OFF.ToBitmap(), trayContextMenu.ImageScalingSize.Width, trayContextMenu.ImageScalingSize.Height);
            tray_RunCheck.Image = ResizeImage(Resources.icon_RunCheck, trayContextMenu.ImageScalingSize.Width, trayContextMenu.ImageScalingSize.Height);
            tray_SpeedTest.Image = ResizeImage(Resources.speedTest.ToBitmap(), trayContextMenu.ImageScalingSize.Width, trayContextMenu.ImageScalingSize.Height);
            tray_CheckForUpdate.Image = ResizeImage(Resources.updateIcon, trayContextMenu.ImageScalingSize.Width, trayContextMenu.ImageScalingSize.Height);
            tray_Exit.Image = ResizeImage(Resources.error.ToBitmap(), trayContextMenu.ImageScalingSize.Width, trayContextMenu.ImageScalingSize.Height);

            // SET VERSION / BUILD LABELS
            Text = app_Title;
            ApplyApplicationIcon();

            lbl_Copyright.Text = app_Copyright;
            lbl_Version.Text += "Version: " + app_VersionString +
                                ", Built: " + app_Built_DateTime;

            // SET CONTROLS TOOLTIPS
            SetControlsTooltips();

            // ADD CLOUDFLARE BYPASS SETTINGS TO MAIN MENU (before Exit item)
            ToolStripMenuItem mainMenu_CfBypass = new ToolStripMenuItem
            {
                Text = "CF BYPASS",
                ToolTipText = "Configure Cloudflare challenge bypass (Playwright / FlareSolverr)",
                Image = ResizeImage(Resources.robot, 16, 16)
            };
            mainMenu_CfBypass.Click += (s, e) => mainMenu_CfBypass_Click(s, e);
            MainMenuStrip.Items.Insert(MainMenuStrip.Items.IndexOf(mainMenu_Exit), mainMenu_CfBypass);

            ToolStripMenuItem mainMenu_Columns = new ToolStripMenuItem
            {
                Text = "Columns",
                ToolTipText = "Choose visible endpoint list columns",
                Image = CreateCommandIcon(CommandIcon.Columns, Color.FromArgb(80, 200, 255), 16)
            };
            mainMenu_Columns.Click += (s, e) => ShowEndpointColumnChooser(new Point(16, 20));
            MainMenuStrip.Items.Insert(MainMenuStrip.Items.IndexOf(mainMenu_CfBypass), mainMenu_Columns);

            // APPLY PREMIUM VISUAL THEME
            ApplyPremiumTheme();
            InitializeEndpointListOptions();
            InitializePremiumMotionEffects();
            Resize += CheckerMainForm_Resize;
            MouseUp += CheckerMainForm_MouseUp;
            HandleCreated += (s, e) => TryUseDarkTitleBar();
            ApplyBottomPanelsLayout();

            // LOAD 'LAST SEEN ONLINE' LIST
            RestoreLastSeenOnlineList();
        }

        public void SetControlsTooltips()
        {
            // SET TOOLTIP FOR 'ALL' ENDPOINTS SELECTION BUTTON
            ToolTip toolTip_EndpointSelection_All = new ToolTip
            {
                ToolTipIcon = ToolTipIcon.Info,
                IsBalloon = true,
                ToolTipTitle = "Endpoints Selection"
            };
            toolTip_EndpointSelection_All.SetToolTip(btn_CheckAll, "Select ALL EndPoints on list");

            // SET TOOLTIP FOR 'PASSED' ENDPOINTS SELECTION BUTTON
            ToolTip toolTip_EndpointSelection_Passed = new ToolTip
            {
                ToolTipIcon = ToolTipIcon.Info,
                IsBalloon = true,
                ToolTipTitle = "Endpoints Selection"
            };
            toolTip_EndpointSelection_Passed.SetToolTip(btn_CheckAllAvailable, "Select only previously PASSED EndPoints on list");

            // SET TOOLTIP FOR 'NONE' ENDPOINTS SELECTION BUTTON
            ToolTip toolTip_EndpointSelection_None = new ToolTip
            {
                ToolTipIcon = ToolTipIcon.Info,
                IsBalloon = true,
                ToolTipTitle = "Endpoints Selection"
            };
            toolTip_EndpointSelection_None.SetToolTip(btn_UncheckAll, "Unselect ALL EndPoints on list");

            // SET TOOLTIP FOR 'FAILED' ENDPOINTS SELECTION BUTTON
            ToolTip toolTip_EndpointSelection_Failed = new ToolTip
            {
                ToolTipIcon = ToolTipIcon.Info,
                IsBalloon = true,
                ToolTipTitle = "Endpoints Selection"
            };
            toolTip_EndpointSelection_Failed.SetToolTip(btn_CheckAllErrors, "Select only previously FAILED EndPoints on list");

            // SET TOOLTIP FOR 'REPORT OUTPUT FOLDER' BROWSE BUTTON
            ToolTip toolTip_BrowseReportOutputFolder = new ToolTip
            {
                ToolTipIcon = ToolTipIcon.Info,
                IsBalloon = true,
                ToolTipTitle = "Report(s) Output location"
            };
            toolTip_BrowseReportOutputFolder.SetToolTip(btn_BrowseExportDir, "Browse for Report(s) output folder");

            // SET TOOLTIP FOR 'RUN CHECK' BUTTON
            ToolTip toolTip_LoadList = new ToolTip {
                ToolTipIcon = ToolTipIcon.Warning,
                IsBalloon = true,
                ToolTipTitle = "Load Endpoints Definitions List"
            };
            toolTip_LoadList.SetToolTip(btn_LoadList, "To reload Endpoints Definitions List, you must press the CTRL key when you click here");

            // SET TOOLTIP FOR 'RUN CHECK' BUTTON
            ToolTip toolTip_RunCheck = new ToolTip
            {
                ToolTipIcon = ToolTipIcon.Info,
                IsBalloon = true,
                ToolTipTitle = "Run Check"
            };
            toolTip_RunCheck.SetToolTip(btn_RunCheck, "Refresh EndPoints list status");

            // SET TOOLTIP FOR 'TERMINATE' BUTTON
            ToolTip toolTip_Terminate = new ToolTip
            {
                ToolTipIcon = ToolTipIcon.Info,
                IsBalloon = true,
                ToolTipTitle = "Terminate Check"
            };
            toolTip_Terminate.SetToolTip(btn_Terminate, "Terminate EndPoint status check process");

            // SET TOOLTIP FOR 'SET FILTER' TEXTBOX
            ToolTip toolTip_Filter = new ToolTip
            {
                ToolTipIcon = ToolTipIcon.Info,
                IsBalloon = true,
                ToolTipTitle = "Endpoint Filter"
            };
            toolTip_Filter.SetToolTip(tb_ListFilter, "Enter EndPoints filter text. You can enter multiple filter values, separated by ; character.");

            // SET TOOLTIP FOR 'CLEAR FILTER' BUTTON
            ToolTip toolTip_ClearFilter = new ToolTip
            {
                ToolTipIcon = ToolTipIcon.Info,
                IsBalloon = true,
                ToolTipTitle = "Endpoint Filter"
            };
            toolTip_ClearFilter.SetToolTip(pb_ListFilterClear, "Clear EndPoints filter text");
        }

        private void ApplyApplicationIcon()
        {
            try
            {
                string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.ico");
                if (!File.Exists(iconPath))
                {
                    iconPath = Path.Combine(Application.StartupPath, "app.ico");
                }

                if (File.Exists(iconPath))
                {
                    Icon appIcon = new Icon(iconPath);
                    Icon = appIcon;
                    trayIcon.Icon = appIcon;
                }
            }
            catch
            {
            }
        }

        public void LoadConfiguration()
        {
            if (Settings.Default.HasSavedConfiguration)
            {
                try
                {
                    cb_AutomaticRefresh.Checked = Settings.Default.Config_EnableAutomaticRefresh;
                    cb_ContinuousRefresh.Checked = Settings.Default.Config_EnableContinuousRefresh;
                    cb_RefreshAutoSet.Checked = Settings.Default.Config_AutoAdjustRefreshInterval;
                    comboBox_Validate.SelectedIndex = Settings.Default.Config_ValidationMethod;
                    num_RefreshInterval.Value = Settings.Default.Config_AutomaticRefreshIntervalSeconds;
                    num_PingTimeout.Value = Settings.Default.Config_PingTimeoutSeconds;
                    num_HTTPRequestTimeout.Value = Settings.Default.Config_HTTP_RequestTimeoutSeconds;
                    num_FTPRequestTimeout.Value = Settings.Default.Config_FTP_RequestTimeoutSeconds;
                    cb_TrayBalloonNotify.Checked = Settings.Default.Config_EnableTrayNotificationsOnError;
                    cb_AllowAutoRedirect.Checked = Settings.Default.Config_AllowAutoRedirect;
                    cb_ValidateSSLCertificate.Checked = Settings.Default.Config_ValidateSSLCertificate;
                    num_ParallelThreadsCount.Value = Settings.Default.Config_ParallelThreadsCount;
                    cb_ResolveNetworkShares.Checked = Settings.Default.Config_ResolveNetworkShares;
                    cb_ExportEndpointsStatus_XLSX.Checked = Settings.Default.Config_ExportEndpointsStatus_XLSX;
                    cb_ExportEndpointsStatus_JSON.Checked = Settings.Default.Config_ExportEndpointsStatus_JSON;
                    cb_ExportEndpointsStatus_XML.Checked = Settings.Default.Config_ExportEndpointsStatus_XML;
                    cb_ExportEndpointsStatus_HTML.Checked = Settings.Default.Config_ExportEndpointsStatus_HTML;
                    cb_ResolvePageMetaInfo.Checked = Settings.Default.Config_ResolvePageMetaInfo;
                    cb_RemoveURLParameters.Checked = Settings.Default.Config_RemoveURLParameters;
                    cb_ResolvePageLinks.Checked = Settings.Default.Config_ResolvePageLinks;
                    cb_SaveResponse.Checked = Settings.Default.Config_SaveResponse;
                    cb_Resolve_DNS_Names.Checked = Settings.Default.Config_Resolve_DNS_Names;
                    cb_Resolve_IPAddresses.Checked = Settings.Default.Config_Resolve_IP_Addresses;
                    cb_Resolve_NIC_MACs.Checked = Settings.Default.Config_Resolve_MAC_Addresses;
                    cb_TestPing.Checked = Settings.Default.Config_TestPing;

                    cb_RefreshOnStartup.Checked = app_ScanOnStartup;

                    if (Directory.Exists(Settings.Default.Config_EndpointsStatusExportDirectory))
                    {
                        statusExport_Directory = Settings.Default.Config_EndpointsStatusExportDirectory;
                    }

                    if (!string.IsNullOrEmpty(Settings.Default.Config_Executable_VNCViewer))
                    {
                        appExecutable_VNC = Settings.Default.Config_Executable_VNCViewer;
                    }

                    if (!string.IsNullOrEmpty(Settings.Default.Config_Executable_Putty))
                    {
                        appExecutable_Putty = Settings.Default.Config_Executable_Putty;
                    }

                    SaveConfiguration();
                }
                catch
                {
                    RestoreSavedSettingsError("Configuration");
                }
            }

            // SET REFRESH TIMER INTERVAL VALUE
            TIMER_AutomaticRefresh.Interval = (int)num_RefreshInterval.Value * 60000;
        }

        public void SaveConfiguration()
        {
            ThreadSafeInvoke(() =>
            {
                if (lv_Endpoints.Visible)
                {
                    Settings.Default.Config_EnableAutomaticRefresh = cb_AutomaticRefresh.Checked;
                    Settings.Default.Config_EnableContinuousRefresh = cb_ContinuousRefresh.Checked;
                    Settings.Default.Config_AutoAdjustRefreshInterval = cb_RefreshAutoSet.Checked;
                    Settings.Default.Config_AutomaticRefreshIntervalSeconds = num_RefreshInterval.Value;
                    Settings.Default.Config_PingTimeoutSeconds = num_PingTimeout.Value;
                    Settings.Default.Config_HTTP_RequestTimeoutSeconds = num_HTTPRequestTimeout.Value;
                    Settings.Default.Config_FTP_RequestTimeoutSeconds = num_FTPRequestTimeout.Value;
                    Settings.Default.Config_EnableTrayNotificationsOnError = cb_TrayBalloonNotify.Checked;
                    Settings.Default.Config_AllowAutoRedirect = cb_AllowAutoRedirect.Checked;
                    Settings.Default.Config_ValidateSSLCertificate = cb_ValidateSSLCertificate.Checked;
                    Settings.Default.Config_EndpointsStatusExportDirectory = statusExport_Directory;
                    Settings.Default.Config_ValidationMethod = comboBox_Validate.SelectedIndex;
                    Settings.Default.Config_ParallelThreadsCount = num_ParallelThreadsCount.Value;
                    Settings.Default.Config_ResolveNetworkShares = cb_ResolveNetworkShares.Checked;
                    Settings.Default.Config_ExportEndpointsStatus_XLSX = cb_ExportEndpointsStatus_XLSX.Checked;
                    Settings.Default.Config_ExportEndpointsStatus_JSON = cb_ExportEndpointsStatus_JSON.Checked;
                    Settings.Default.Config_ExportEndpointsStatus_XML = cb_ExportEndpointsStatus_XML.Checked;
                    Settings.Default.Config_ExportEndpointsStatus_HTML = cb_ExportEndpointsStatus_HTML.Checked;
                    Settings.Default.Config_ResolvePageMetaInfo = cb_ResolvePageMetaInfo.Checked;
                    Settings.Default.Config_RemoveURLParameters = cb_RemoveURLParameters.Checked;
                    Settings.Default.Config_ResolvePageLinks = cb_ResolvePageLinks.Checked;
                    Settings.Default.Config_Resolve_DNS_Names = cb_Resolve_DNS_Names.Checked;
                    Settings.Default.Config_Resolve_IP_Addresses = cb_Resolve_IPAddresses.Checked;
                    Settings.Default.Config_Resolve_MAC_Addresses = cb_Resolve_NIC_MACs.Checked;
                    Settings.Default.Config_TestPing = cb_TestPing.Checked;
                    Settings.Default.Config_SaveResponse = cb_SaveResponse.Checked;
                    Settings.Default.VirusTotal_API_Key = apiKey_VirusTotal;
                    Settings.Default.GoogleMaps_API_Key = apiKey_GoogleMaps;
                    Settings.Default.Config_Executable_VNCViewer = appExecutable_VNC;
                    Settings.Default.Config_Executable_Putty = appExecutable_Putty;
                    Settings.Default.Config_ScanOnStartup = cb_RefreshOnStartup.Checked;
                    Settings.Default.HasSavedConfiguration = true;
                    Settings.Default.Save();
                }
            });
        }

        public void ListEndpoints(ListViewRefreshMethod refreshMethod)
        {
            listUpdating = true;
            SetProgressStatus(0, 0, "Updating Endpoints list ...", Color.DodgerBlue);
            Application.DoEvents();

            List<string> _endpointsList_Disabled = new List<string>();

            lv_Endpoints.BeginUpdate();
            lv_Endpoints.Items.Clear();

            foreach (EndpointDefinition endpointItem in endpointsList)
            {
                // CREATE ITEM
                ListViewItem refreshedItem = new ListViewItem(
                    endpointItem.Name,
                    GetStatusImageIndex(
                        endpointItem.ResponseCode,
                        endpointItem.PingRoundtripTime,
                        endpointItem.ResponseMessage));

                // ADD SUBITEMS
                refreshedItem.SubItems.Add(endpointItem.Protocol);
                refreshedItem.SubItems.Add(endpointItem.Port);
                refreshedItem.SubItems.Add(BuildUpConnectionString(endpointItem));
                refreshedItem.SubItems.Add(string.Join(", ", endpointItem.IPAddress));
                refreshedItem.SubItems.Add(endpointItem.ResponseTime);
                refreshedItem.SubItems.Add(endpointItem.ResponseCode);
                refreshedItem.SubItems.Add(endpointItem.ResponseMessage);
                refreshedItem.SubItems.Add(endpointItem.LastSeenOnline);
                refreshedItem.SubItems.Add(string.Join(", ", endpointItem.MACAddress));
                refreshedItem.SubItems.Add(endpointItem.PingRoundtripTime);
                refreshedItem.SubItems.Add(endpointItem.ServerID);
                refreshedItem.SubItems.Add(endpointItem.LoginName);
                refreshedItem.SubItems.Add(string.Join(", ", endpointItem.NetworkShare));
                refreshedItem.SubItems.Add(string.Join(", ", endpointItem.DNSName));
                refreshedItem.SubItems.Add(endpointItem.HTTPcontentType);
                refreshedItem.SubItems.Add(endpointItem.HTTPcontentLength);
                refreshedItem.SubItems.Add(endpointItem.HTTPexpires);
                refreshedItem.SubItems.Add(endpointItem.HTTPetag);

                // ADD SUBITEMS NAME
                refreshedItem.Name = "Endpoint Name";
                refreshedItem.SubItems[1].Name = "Protocol";
                refreshedItem.SubItems[2].Name = "Port";
                refreshedItem.SubItems[3].Name = "Response URL";
                refreshedItem.SubItems[4].Name = "IP Address";
                refreshedItem.SubItems[5].Name = "Response Time";
                refreshedItem.SubItems[6].Name = "Status Code";
                refreshedItem.SubItems[7].Name = "Status Message";
                refreshedItem.SubItems[8].Name = "Last Seen Online";
                refreshedItem.SubItems[9].Name = "MAC Address";
                refreshedItem.SubItems[10].Name = "Ping Roundtrip Time";
                refreshedItem.SubItems[11].Name = "Server";
                refreshedItem.SubItems[12].Name = "User Name";
                refreshedItem.SubItems[13].Name = "Network Shares";
                refreshedItem.SubItems[14].Name = "DNS Name";
                refreshedItem.SubItems[15].Name = "HTTP Content Type";
                refreshedItem.SubItems[16].Name = "HTTP Content Length";
                refreshedItem.SubItems[17].Name = "HTTP Expires";
                refreshedItem.SubItems[18].Name = "HTTP ETag";

                // SET BACKGROUND AND TEXT COLOR BY STATUS CODE
                refreshedItem.BackColor = GetColorByStatus(endpointItem.ResponseCode, endpointItem.PingRoundtripTime, endpointItem.ResponseMessage);
                refreshedItem.ForeColor = GetForeColorByStatus(endpointItem.ResponseCode, endpointItem.PingRoundtripTime, endpointItem.ResponseMessage);

                // SET CHECKED [ENABLED] STATUS - DEPENDING ON REFRESH METHOD
                if (refreshMethod == ListViewRefreshMethod.CurrentState)
                {
                    refreshedItem.Checked = !endpointsList_Disabled.Contains(endpointItem.Name);
                }
                else if (refreshMethod == ListViewRefreshMethod.CheckAll)
                {
                    refreshedItem.Checked = true;
                }
                else if (refreshMethod == ListViewRefreshMethod.UncheckAll)
                {
                    refreshedItem.Checked = false;
                }
                else if (refreshMethod == ListViewRefreshMethod.CheckAllPassed)
                {
                    if (refreshedItem.ImageIndex == 0)
                    {
                        refreshedItem.Checked = true;
                    }
                    else if (refreshedItem.ImageIndex == 1 ||
                             refreshedItem.ImageIndex == 2 ||
                             refreshedItem.ImageIndex == 5)
                    {
                        refreshedItem.Checked = false;
                    }
                }
                else if (refreshMethod == ListViewRefreshMethod.CheckAllFailed)
                {
                    if (refreshedItem.ImageIndex == 0)
                    {
                        refreshedItem.Checked = false;
                    }
                    else if (refreshedItem.ImageIndex == 1 ||
                             refreshedItem.ImageIndex == 2 ||
                             refreshedItem.ImageIndex == 5)
                    {
                        refreshedItem.Checked = true;
                    }
                }

                if (!refreshedItem.Checked)
                {
                    _endpointsList_Disabled.Add(refreshedItem.Text);
                }

                bool filterMatch = true;
                foreach (string filterValue in tb_ListFilter.Text.ToLower().Split(';'))
                {
                    if (!refreshedItem.Text.ToLower().Contains(filterValue))
                    {
                        filterMatch = false;
                    }
                }

                if (filterMatch)
                {
                    lv_Endpoints.Items.Add(refreshedItem);
                }
            }

            endpointsList_Disabled = _endpointsList_Disabled;

            lv_Endpoints.EndUpdate();

            if (endpointsList.Count() > 0)
            {
                // IF FILTER IS NOT USED (ALL ITEMS VISIBLE)
                if (string.IsNullOrEmpty(tb_ListFilter.Text))
                {
                    try
                    {
                        // RESTORE TOPITEM
                        if (lv_Endpoints_TopItemIndex < lv_Endpoints.Items.Count)
                        {
                            lv_Endpoints.TopItem = lv_Endpoints.Items[lv_Endpoints_TopItemIndex];
                        }

                        // RESTORE SELECTION
                        foreach (int selectedItemIndex in lv_Endpoints_SelectedItems)
                        {
                            if (selectedItemIndex < lv_Endpoints.Items.Count)
                            {
                                lv_Endpoints.Items[selectedItemIndex].Selected = true;
                            }
                        }
                    }
                    catch
                    {
                    }
                }

                ThreadSafeInvoke(() =>
                {
                    SetControls(false, false);
                    btn_RunCheck.Enabled = lv_Endpoints.Items.Count > 0;
                    lbl_EndpointsListLoading.Visible = false;
                    lv_Endpoints.Visible = true;
                });
            }
            else
            {
                ThreadSafeInvoke(() =>
                {
                    SetControls(false, true);
                    lbl_EndpointsListLoading.ForeColor = Color.BlueViolet;
                    lbl_EndpointsListLoading.Text = "Endpoints definitions file \"" + endpointDefinitionsFile + "\" doesn't contains any valid Endpoint definition.";
                    lbl_EndpointsListLoading.Visible = true;
                    lv_Endpoints.Visible = false;
                });
            }

            listUpdating = false;
        }

        public void bw_GetStatus_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                SetProgressStatus(0, 0, "Initializing Endpoints Status refresh ...", Color.DarkOrchid);

                // WORKING VARIABLES
                // UI control properties are captured on the UI thread via ThreadSafeInvoke
                // to avoid cross-thread violations under .NET 5+ strict enforcement.
                ConcurrentBag<EndpointDefinition> updatedEndpointsList = new ConcurrentBag<EndpointDefinition>();
                EndpointCheckOptions checkOptions = null;
                ThreadSafeInvoke(() =>
                {
                    checkOptions = EndpointCheckOptions.FromUiValues(
                        cb_AllowAutoRedirect.Checked,
                        cb_ValidateSSLCertificate.Checked,
                        cb_RefreshAutoSet.Checked,
                        cb_ResolveNetworkShares.Checked,
                        cb_ResolvePageMetaInfo.Checked,
                        cb_RemoveURLParameters.Checked,
                        cb_ResolvePageLinks.Checked,
                        cb_SaveResponse.Checked,
                        cb_TestPing.Checked,
                        cb_Resolve_DNS_Names.Checked,
                        cb_Resolve_IPAddresses.Checked,
                        cb_Resolve_NIC_MACs.Checked,
                        (int)num_ParallelThreadsCount.Value,
                        (int)num_PingTimeout.Value,
                        (int)num_HTTPRequestTimeout.Value,
                        (int)num_FTPRequestTimeout.Value);
                });

                int endpointsCount_Current = 0;

                // TextBox.Text calls GetWindowText (Win32) and throws
                // InvalidOperationException from non-UI threads in .NET 5+.
                // Capture the filter value on the UI thread before going parallel.
                string listFilter = string.Empty;
                ThreadSafeInvoke(() => listFilter = tb_ListFilter.Text.ToLower());

                int endpointsCount_Enabled =
                    endpointsList.Where(
                        eItem =>
                                 !endpointsList_Disabled.Contains(eItem.Name) &&
                                 eItem.Name.ToLower().Contains(listFilter)).Count();

                // FLUSH LOCAL DNS CACHE
                DnsFlushResolverCache();

                if (checkOptions.ValidateSslCertificate)
                {   // VALIDATE SERVER CERTIFICATE [HTTPS]
                    ServicePointManager.ServerCertificateValidationCallback = null;
                }
                else
                {
                    // BYPASS SERVER CERTIFICATE VALIDATION
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                }

                // ADJUST THREADS COUNT SETTING BY ENABLED ITEMS COUNT [IF LESS]
                checkOptions = checkOptions.WithThreadCountAdjustedForEnabledEndpoints(endpointsCount_Enabled);

                // STORE PROGRESS START DATE/TIME [FOR 'EXPORT' AND 'AUTO ADJUST REFRESH INTERVAL' PURPOSES]
                DateTime startDT_List = DateTime.Now;

                // EXECUTE PARALLEL PROCESS 
                Parallel.ForEach(
                    endpointsList,
                    new ParallelOptions { MaxDegreeOfParallelism = checkOptions.ThreadsCount },
                    endpointItem =>
                    {
                        if (!endpointItem.Name.ToLower().Contains(listFilter))
                        {
                            // ADD CURRENT STATUS DEFINITION TO LIST
                            // [NOT VISIBLE ON THE LIST, IS FILTERED OUT]
                            updatedEndpointsList.Add(endpointItem);
                        }
                        else
                        {
                            try
                            {
                            // RESCAN ENDPOINT STATUS
                            Uri endpointURI = new Uri(endpointItem.Address);
                            Uri responseURI = new Uri(endpointItem.ResponseAddress);
                            string durationTime_Item = status_NotAvailable;

                            EndpointDefinition endpoint = EndpointCheckResultFactory.CreatePendingResult(endpointItem);

                            if (endpointsList_Disabled.Contains(endpoint.Name))
                            {
                                // ENDPOINT IS DISBALED, DON'T CHECK
                                endpoint.ResponseMessage = GetEnumDescriptionString(EndpointStatus.DISABLED);
                            }
                            else
                            {
                                // ENDPOINT IS ENABLED, GO ON
                                if (!BW_GetStatus.CancellationPending)
                                {
                                    // INCREMENT PROGRESS COUNTER
                                    Interlocked.Increment(ref endpointsCount_Current);

                                    // SET PROGRESS STATUS LABEL
                                    SetProgressStatus(endpointsCount_Enabled, endpointsCount_Current);

                                    // CREATE STOPWATCH FOR ITEM CHECK DURATION [FOR 'EXPORT' PURPOSE]
                                    Stopwatch sw_ItemProgress = new Stopwatch();

                                    if (validationMethod == ValidationMethod.Protocol &&
                                        !BW_GetStatus.CancellationPending &&
                                        (endpoint.Protocol.ToLower() == Uri.UriSchemeHttp ||
                                         endpoint.Protocol.ToLower() == Uri.UriSchemeHttps))
                                    {
                                        // AUTO-REDIRECT SWITCH [BY 'LOCATION' HEADER OF '3xx' RESPONSE CODE]
                                        bool autoRedirect_Followed = false;

                                        // HTTP OR HTTPS PROTOCOL SCHEME
                                        HttpWebRequest httpWebRequest = null;
                                        HttpWebResponse httpWebResponse = null;

                                        // START STOPWATCH FOR ITEM CHECK DURATION
                                        sw_ItemProgress.Start();

                                        try
                                        {
                                            // PREPARE WEBREQUEST
                                            httpWebRequest = PrepareHTTPWebRequest(
                                                endpoint,
                                                endpointURI,
                                                checkOptions.HttpRequestTimeout,
                                                checkOptions.AllowAutoRedirect,
                                                checkOptions.RemoveUrlParameters);

                                            try
                                            {
                                                // TRY TO GET RESPONSE
                                                httpWebResponse = GetHTTPWebResponse(httpWebRequest, 3);

                                                // HANDLE POSSIBLE REDIRECT [3xx]
                                                if (checkOptions.AllowAutoRedirect && ((int)httpWebResponse.StatusCode).ToString().StartsWith("3"))
                                                {
                                                    throw new WebException(
                                                        "HTTP Response Code: " + (int)httpWebResponse.StatusCode,
                                                        null,
                                                        WebExceptionStatus.UnknownError,
                                                        httpWebResponse);
                                                }
                                            }
                                            catch (WebException wEX)
                                            {
                                                // IF RESULT CODE IS '3xx', DO A SECOND CALL ON 'LOCATION'
                                                if (checkOptions.AllowAutoRedirect &&
                                                    wEX.Response is HttpWebResponse _httpWebResponse &&
                                                    ((int)_httpWebResponse.StatusCode).ToString().StartsWith("3") &&
                                                    _httpWebResponse.Headers.AllKeys.Contains("Location") &&
                                                    !string.IsNullOrEmpty(_httpWebResponse.GetResponseHeader("Location")))
                                                {
                                                    // GET 'LOCATION' HEADER VALUE
                                                    string locationHeaderValue = _httpWebResponse.GetResponseHeader("Location");

                                                    // IF IS RELATIVE PATH
                                                    if (Uri.IsWellFormedUriString(locationHeaderValue, UriKind.Relative))
                                                    {
                                                        locationHeaderValue = Url.Combine(_httpWebResponse.ResponseUri.OriginalString, locationHeaderValue);
                                                    }

                                                    // PREPARE WEBREQUEST
                                                    HttpWebRequest httpWebRequest_Redirected = PrepareHTTPWebRequest(
                                                        endpoint,
                                                        new Uri(locationHeaderValue),
                                                        checkOptions.HttpRequestTimeout,
                                                        checkOptions.AllowAutoRedirect,
                                                        checkOptions.RemoveUrlParameters,
                                                        _httpWebResponse.Cookies);

                                                    autoRedirect_Followed = true;

                                                    // GET RESPONSE FROM 'LOCATION'
                                                    httpWebResponse = GetHTTPWebResponse(httpWebRequest_Redirected, 3);
                                                }
                                                else
                                                {
                                                    throw wEX;
                                                }
                                            }

                                            // STOP STOPWATCH FOR ITEM CHECK DURATION
                                            sw_ItemProgress.Stop();

                                            // GET RESPONSE HEADERS
                                            GetHTTPWebHeaders(endpoint.HTTPResponseHeaders.PropertyItem, httpWebResponse.Headers);

                                            // GET SSL INFO
                                            GetSSLCertificateInfo(httpWebRequest, endpoint);

                                            responseURI = httpWebResponse.ResponseUri;
                                            endpoint.Port = responseURI.Port.ToString();
                                            endpoint.Protocol = responseURI.Scheme.ToUpper();
                                            endpoint.ResponseCode = ((int)httpWebResponse.StatusCode).ToString();

                                            // SERVER IDENTIFICATION
                                            if (!string.IsNullOrEmpty(httpWebResponse.Server))
                                            {
                                                endpoint.ServerID = Regex.Replace(httpWebResponse.Server, "<.*?>", string.Empty);
                                            }

                                            // STATUS MESSAGE
                                            if (!string.IsNullOrEmpty(httpWebResponse.StatusDescription))
                                            {
                                                // STATUS DESCRIPTION
                                                endpoint.ResponseMessage = httpWebResponse.StatusDescription;
                                            }
                                            else
                                            {
                                                // STATUS CODE [STRING]
                                                endpoint.ResponseMessage = httpWebResponse.StatusCode.ToString();
                                            }

                                            // GET AUTO REDIRECTS COUNT
                                            if (checkOptions.AllowAutoRedirect)
                                            {
                                                FieldInfo fieldInfo = httpWebRequest.GetType().GetField("_AutoRedirects", BindingFlags.NonPublic | BindingFlags.Instance);
                                                // _AutoRedirects was removed in .NET 10; guard against null fieldInfo
                                                int httpAutoRedirects = fieldInfo != null ? (int)fieldInfo.GetValue(httpWebRequest) : 0;
                                                endpoint.HTTPautoRedirects = httpAutoRedirects.ToString();

                                                // CHECK AUTO REDIRECT URL [COMPARE REQUEST AND RESPONSE ENDPOINT URIs]
                                                if (endpointURI.Scheme != responseURI.Scheme ||
                                                    endpointURI.Port != responseURI.Port ||
                                                    endpointURI.Host != responseURI.Host ||
                                                    autoRedirect_Followed)
                                                {
                                                    endpoint.ResponseMessage += " (Redirected from \"" + endpointURI.OriginalString + "\")";
                                                }
                                            }

                                            // GET 'CONTENT TYPE' META VALUE FROM RESPONSE HEADER
                                            endpoint.HTTPcontentType = GetContentType(httpWebResponse.ContentType);

                                            // GET 'EXPIRES' META VALUE FROM RESPONSE HEADER
                                            DateTime _httpExpiresDT = DateTime.MinValue;
                                            if (!string.IsNullOrEmpty(httpWebResponse.Headers["Expires"]) &&
                                                TryParseHttpDate(httpWebResponse.Headers["Expires"], out _httpExpiresDT) &&
                                                _httpExpiresDT > DateTime.MinValue)
                                            {
                                                endpoint.HTTPexpires = _httpExpiresDT.ToString("dd.MM.yyyy HH:mm");
                                            }

                                            // GET 'ETAG' META VALUE FROM RESPONSE HEADER
                                            if (!string.IsNullOrEmpty(httpWebResponse.Headers["ETag"]))
                                            {
                                                endpoint.HTTPetag = httpWebResponse.Headers["ETag"]
                                                    .ToString()
                                                    .TrimStart()
                                                    .TrimEnd()
                                                    .TrimStart('"')
                                                    .TrimEnd('"');
                                            }

                                            // GET CONTENT Length FROM RESPONSE HEADER
                                            long contentLength = httpWebResponse.ContentLength;

                                            if (!string.IsNullOrEmpty(httpWebResponse.Headers["Content-Length"]))
                                            {
                                                long.TryParse(httpWebResponse.Headers["Content-Length"], out contentLength);
                                            }

                                            GetWebResponseContentLengthString(endpoint, contentLength);

                                            // TRY TO GET HEADER ENCODING FROM RESPONSE HEADER
                                            endpoint.HTTPencoding = GetEncoding(httpWebResponse.ContentType);

                                            if (checkOptions.SaveResponse || checkOptions.ResolvePageMetaInfo)
                                            {
                                                // GET RESPONSE STREAM
                                                using (BinaryReader httpWebResponseBinaryReader = new BinaryReader(httpWebResponse.GetResponseStream()))
                                                {
                                                    MemoryStream httpWebResponseMemoryStream = new MemoryStream();

                                                    byte[] httpWebResponseByteArray;
                                                    byte[] httpWebResponseBuffer = httpWebResponseBinaryReader.ReadBytes(1024);
                                                    while (httpWebResponseBuffer.Length > 0 && httpWebResponseMemoryStream.Length < (http_SaveResponse_MaxLength_Bytes + 1024))
                                                    {
                                                        httpWebResponseMemoryStream.Write(httpWebResponseBuffer, 0, httpWebResponseBuffer.Length);
                                                        httpWebResponseBuffer = httpWebResponseBinaryReader.ReadBytes(1024);
                                                    }

                                                    httpWebResponseByteArray = new byte[(int)httpWebResponseMemoryStream.Length];
                                                    httpWebResponseMemoryStream.Position = 0;
                                                    httpWebResponseMemoryStream.Read(httpWebResponseByteArray, 0, httpWebResponseByteArray.Length);

                                                    // GET CONTENT Length FROM FULL RESPONSE
                                                    contentLength = httpWebResponseMemoryStream.Length;
                                                    GetWebResponseContentLengthString(endpoint, contentLength);

                                                    if (checkOptions.SaveResponse &&
                                                        !string.IsNullOrEmpty(endpoint.HTTPcontentType) &&
                                                        CheckWebResponseContentLength(endpoint, httpWebResponse, contentLength))
                                                    {
                                                        // GET FILE EXTENSION BY CONTENT TYPE
                                                        string fileExtension = GetFileExtensionByContentType(endpoint.HTTPcontentType);

                                                        // SAVE RESPONSE TO FILE
                                                        SaveWebResponseStream(
                                                                              startDT_List,
                                                                              endpoint.Name,
                                                                              httpWebResponseByteArray,
                                                                              fileExtension);
                                                    }

                                                    if (endpoint.HTTPcontentType == "text/html")
                                                    {
                                                        // GET HTML METADATA
                                                        if (checkOptions.ResolvePageMetaInfo)
                                                        {
                                                            // READ RESPONSE STREAM [HTTP HEADER ENCODING] AND GET HTML META INFO
                                                            ResolvePageMetaInfo(ReadHTTPResponseStream(httpWebResponseMemoryStream, endpoint.HTTPencoding), endpoint, responseURI, checkOptions.ResolvePageLinks);

                                                            // IF ENCODING NOT PRESENT IN HTTP HEADER, READ AGAIN WITH ENCODING FROM HTML META
                                                            if (endpoint.HTTPencoding == null)
                                                            {
                                                                // READ AGAIN WITH ENCODING FROM HTML META [IF PRESENT]
                                                                if (endpoint.HTMLencoding != null)
                                                                {
                                                                    // READ RESPONSE STREAM [HML META ENCODING] AND GET HTML META INFO
                                                                    ResolvePageMetaInfo(ReadHTTPResponseStream(httpWebResponseMemoryStream, endpoint.HTMLencoding), endpoint, responseURI, checkOptions.ResolvePageLinks);
                                                                }
                                                                else
                                                                {
                                                                    // SET DEAFULT HTML STREAM ENCODING
                                                                    endpoint.HTMLencoding = endpoint.HTMLdefaultStreamEncoding;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        catch (WebException webException)
                                        {
                                            // STOP STOPWATCH FOR ITEM CHECK DURATION
                                            sw_ItemProgress.Stop();

                                            httpWebResponse = webException.Response as HttpWebResponse;

                                            if (httpWebResponse != null)
                                            {
                                            try
                                            {
                                                // RESPONSE CODE
                                                endpoint.ResponseCode = ((int)httpWebResponse.StatusCode).ToString();

                                                // STATUS MESSAGE
                                                if (!string.IsNullOrEmpty(httpWebResponse.StatusDescription))
                                                {
                                                    // STATUS DESCRIPTION
                                                    endpoint.ResponseMessage = httpWebResponse.StatusDescription;

                                                    // ERROR MESSAGE
                                                    if (!webException.Message.Contains(endpoint.ResponseCode))
                                                    {
                                                        endpoint.ResponseMessage += " -> " + webException.Message;
                                                    }
                                                }
                                                else
                                                {
                                                    // STATUS CODE [STRING]
                                                    endpoint.ResponseMessage = httpWebResponse.StatusCode.ToString();

                                                    // ERROR MESSAGE
                                                    if (!webException.Message.Contains(endpoint.ResponseCode))
                                                    {
                                                        endpoint.ResponseMessage += " -> " + webException.Message;
                                                    }
                                                }

                                                // GET RESPONSE HEADERS
                                                GetHTTPWebHeaders(endpoint.HTTPResponseHeaders.PropertyItem, httpWebResponse.Headers);

                                                // GET SSL INFO
                                                GetSSLCertificateInfo(httpWebRequest, endpoint);

                                                // CLOUDFLARE BOT-PROTECTION DETECTION
                                                // CF-RAY header is present on all Cloudflare-proxied responses.
                                                // A 403/429/503 with CF headers means the endpoint exists but is
                                                // behind a security challenge that cannot be solved automatically.
                                                if (IsCloudflareProtected(httpWebResponse))
                                                {
                                                    string cfRay = httpWebResponse.Headers["CF-RAY"];
                                                    endpoint.ResponseMessage +=
                                                        " [Cloudflare Bot Protection" +
                                                        (!string.IsNullOrEmpty(cfRay) ? " | CF-RAY: " + cfRay : string.Empty) +
                                                        "]";

                                                    // ATTEMPT BYPASS VIA CONFIGURED METHOD
                                                    CloudflareBypassMethod cfBypassMethod =
                                                        (CloudflareBypassMethod)Settings.Default.Config_CloudflareBypass_Method;
                                                    if (cfBypassMethod != CloudflareBypassMethod.Disabled)
                                                    {
                                                        try
                                                        {
                                                            CloudflareBypassResult cfBypassResult =
                                                                CloudflareBypassChecker.Check(
                                                                    endpoint.ResponseAddress ?? endpoint.Address,
                                                                    cfBypassMethod,
                                                                    Settings.Default.Config_FlareSolverr_URL);
                                                            if (cfBypassResult != null && cfBypassResult.Success)
                                                            {
                                                                endpoint.ResponseCode = cfBypassResult.StatusCode.ToString();
                                                                endpoint.ResponseMessage = cfBypassResult.StatusMessage;
                                                            }
                                                            else if (cfBypassResult != null)
                                                            {
                                                                endpoint.ResponseMessage +=
                                                                    " | Bypass(" + cfBypassResult.MethodUsed + "): " +
                                                                    cfBypassResult.StatusMessage;
                                                            }
                                                        }
                                                        catch (Exception bypassEx)
                                                        {
                                                            endpoint.ResponseMessage += " | Bypass error: " + bypassEx.Message;
                                                        }
                                                    }
                                                }
                                            }
                                            catch (Exception responseEx)
                                            {
                                                // Any unexpected exception inside the WebException handler
                                                // (e.g. reflection, SSL cert reading) marks the endpoint as error
                                                // instead of crashing the whole scan.
                                                endpoint.ResponseCode = status_Error;
                                                endpoint.ResponseMessage =
                                                    responseEx.GetType().Name.Replace("Exception", string.Empty) +
                                                    " -> " + responseEx.Message;
                                            }
                                            }
                                            else
                                            {
                                                // EXCEPTION CODE
                                                endpoint.ResponseCode = status_Error;

                                                // EXCEPTION STATUS
                                                endpoint.ResponseMessage = webException.Status.ToString();
                                                endpoint.ResponseMessage += " -> " + webException.Message;

                                                // Walk the full inner exception chain to expose the root cause.
                                                // In .NET 10, WebException wraps HttpRequestException wraps
                                                // AuthenticationException/IOException — one level isn't enough.
                                                Exception innerEx = webException.InnerException;
                                                while (innerEx != null)
                                                {
                                                    if (!string.IsNullOrEmpty(innerEx.Message) &&
                                                        !endpoint.ResponseMessage.Contains(innerEx.Message))
                                                        endpoint.ResponseMessage += " -> " + innerEx.Message;
                                                    innerEx = innerEx.InnerException;
                                                }
                                            }
                                        }
                                        catch (Exception exception)
                                        {
                                            // STOP STOPWATCH FOR ITEM CHECK DURATION
                                            sw_ItemProgress.Stop();

                                            // CODE
                                            endpoint.ResponseCode = status_Error;

                                            // EXCEPTION TYPE
                                            endpoint.ResponseMessage = exception.GetType().Name.Replace("Exception", string.Empty);

                                            // MESSAGE
                                            endpoint.ResponseMessage += " -> " + exception.Message;

                                            // INNER EXCEPTION MESSAGE
                                            if (exception.InnerException != null &&
                                                !string.IsNullOrEmpty(exception.InnerException.Message) &&
                                                !endpoint.ResponseMessage.Contains(exception.InnerException.Message))
                                            {
                                                endpoint.ResponseMessage += " -> " + exception.InnerException.Message;
                                            }
                                        }
                                        finally
                                        {
                                            if (httpWebResponse != null)
                                            {
                                                // CLOSE
                                                httpWebResponse.Close();
                                            }
                                        }
                                    }
                                    else if (validationMethod == ValidationMethod.Protocol &&
                                             !BW_GetStatus.CancellationPending &&
                                             endpoint.Protocol.ToLower() == Uri.UriSchemeFtp)
                                    {
                                        // FTP PROTOCOL SCHEME
                                        FtpWebRequest ftpWebRequest;
                                        FtpWebResponse ftpWebResponse = null;

                                        try
                                        {
                                            // CREATE REQUEST
                                            ftpWebRequest = (FtpWebRequest)WebRequest.Create(endpointURI.OriginalString);
                                            ftpWebRequest.Method = WebRequestMethods.Ftp.PrintWorkingDirectory;
                                            ftpWebRequest.Timeout = checkOptions.FtpRequestTimeout;
                                            ftpWebRequest.ReadWriteTimeout = checkOptions.FtpRequestTimeout;
                                            ftpWebRequest.UsePassive = false;
                                            ftpWebRequest.UseBinary = false;
                                            ftpWebRequest.KeepAlive = false;
                                            ftpWebRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                                            ftpWebRequest.Proxy = null;

                                            if (endpoint.LoginName == status_NotAvailable ||
                                                endpoint.LoginPass == status_NotAvailable)
                                            {
                                                // GET DEFAULT CREDENTIALS FROM URI [USERNAME]
                                                endpoint.LoginName = ftpWebRequest.Credentials.GetCredential(endpointURI, string.Empty).UserName;
                                                endpoint.LoginPass = anonymousFTPPassword;
                                            }

                                            // SET CREDENTIALS
                                            ftpWebRequest.Credentials = new NetworkCredential(endpoint.LoginName, endpoint.LoginPass);

                                            // START STOPWATCH FOR ITEM CHECK DURATION
                                            sw_ItemProgress.Start();

                                            // GET RESPONSE
                                            ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                                            responseURI = ftpWebResponse.ResponseUri;
                                            endpoint.Port = responseURI.Port.ToString();
                                            endpoint.Protocol = responseURI.Scheme.ToUpper();

                                            // STOP STOPWATCH FOR ITEM CHECK DURATION
                                            sw_ItemProgress.Stop();

                                            // GET STATUS CODE AND MESSAGE
                                            FTPWebResponseStatusMessage(ftpWebResponse, null, endpoint);
                                        }
                                        catch (WebException webException)
                                        {
                                            // STOP STOPWATCH FOR ITEM CHECK DURATION
                                            sw_ItemProgress.Stop();

                                            // GET STATUS CODE AND MESSAGE
                                            FTPWebResponseStatusMessage(null, webException, endpoint);
                                        }
                                        catch (Exception exception)
                                        {
                                            // STOP STOPWATCH FOR ITEM CHECK DURATION
                                            sw_ItemProgress.Stop();

                                            // CODE
                                            endpoint.ResponseCode = status_Error;

                                            // EXCEPTION TYPE
                                            endpoint.ResponseMessage = exception.GetType().Name.Replace("Exception", string.Empty);

                                            // EXCEPTION MESSAGE
                                            endpoint.ResponseMessage += " -> " + exception.Message;

                                            // INNER EXCEPTION MESSAGE
                                            if (exception.InnerException != null &&
                                                !string.IsNullOrEmpty(exception.InnerException.Message) &&
                                                !endpoint.ResponseMessage.Contains(exception.InnerException.Message))
                                            {
                                                endpoint.ResponseMessage += " -> " + exception.InnerException.Message;
                                            }
                                        }
                                        finally
                                        {
                                            if (ftpWebResponse != null)
                                            {
                                                // CLOSE
                                                ftpWebResponse.Close();
                                            }
                                        }
                                    }

                                    // FILL UP 'IP ADDRESS' / 'DNS NAME' [FAST, FROM INPUT, BY REGEX]
                                    if (Regex.IsMatch(responseURI.Host, @"^[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$"))
                                    {
                                        // IS IP ADDRESS
                                        endpoint.IPAddress = new string[] { responseURI.Host };
                                    }
                                    else
                                    {
                                        // IS DNS NAME
                                        endpoint.DNSName = new string[] { responseURI.Host };
                                    }

                                    // DECLARE TEMPORARY LISTS
                                    List<string> endpointIPAddressesStringList = new List<string>();
                                    List<string> endpointDNSNamesStringList = new List<string>();
                                    List<string> endpointMACAddressStringList = new List<string>();

                                    // GET ITEM CHECK DURATION TIME [FOR 'EXPORT' PURPOSE]
                                    if (validationMethod == ValidationMethod.Protocol &&
                                        !BW_GetStatus.CancellationPending)
                                    {
                                        durationTime_Item = sw_ItemProgress.ElapsedMilliseconds.ToString() + " ms";
                                    }

                                    if (!BW_GetStatus.CancellationPending &&
                                        checkOptions.ResolveIpAddresses)
                                    {
                                        // RESOLVE IP ADDRESS(ES)
                                        try
                                        {
                                            // GET LIST
                                            IPAddress[] endpoint_IP_Address_List = Dns.GetHostAddresses(responseURI.Host);

                                            // PROCESS LIST
                                            foreach (IPAddress endpointIPAddress in endpoint_IP_Address_List)
                                            {
                                                if (endpointIPAddress.AddressFamily == AddressFamily.InterNetwork)
                                                {
                                                    endpointIPAddressesStringList.Add(endpointIPAddress.ToString());
                                                }
                                            }
                                        }
                                        catch
                                        {
                                        }
                                    }

                                    if (!BW_GetStatus.CancellationPending &&
                                        checkOptions.ResolveNetworkShares)
                                    {
                                        // RESOLVE NETWORK SHARES
                                        try
                                        {
                                            // GET LIST
                                            List<string> netSharesList = GetNetShares(responseURI.Host);

                                            // PROCESS LIST
                                            if (netSharesList.Count > 0)
                                            {
                                                netSharesList.Sort();
                                                endpoint.NetworkShare = netSharesList.ToArray();
                                            }
                                        }
                                        catch
                                        {
                                        }
                                    }

                                    if (!BW_GetStatus.CancellationPending &&
                                        checkOptions.ResolveDnsNames)
                                    {
                                        // RESOLVE DNS NAME(S)
                                        foreach (string _IP_Address in endpointIPAddressesStringList)
                                        {
                                            try
                                            {
                                                // GET DNS NAME
                                                IPHostEntry hostEntry = Dns.GetHostEntry(_IP_Address);
                                                endpointDNSNamesStringList.Add(hostEntry.HostName);
                                            }
                                            catch
                                            {
                                            }
                                        }
                                    }

                                    if (!BW_GetStatus.CancellationPending &&
                                        checkOptions.ResolveMacAddresses)
                                    {
                                        foreach (string _IP_Address in endpointIPAddressesStringList)
                                        {
                                            try
                                            {
                                                // RESOLVE MAC ADDRESS
                                                string macAddress = WindowsLookupService.Lookup(IPAddress.Parse(_IP_Address));

                                                // IF ENDPOINT IP ADDRESS IS ANY OF 'DNS SERVER OR DEFAULT GATEWAY' IPs
                                                // OR
                                                // RESOLVED MAC IS NOT ANY OF 'DNS SERVER OR DEFAULT GATEWAY' MAC ADDRESSes
                                                if (!string.IsNullOrEmpty(macAddress) &&
                                                   (!localDNSAndGWMACAddresses.Contains(macAddress) ||
                                                    localDNSAndGWIPAddresses.Contains(_IP_Address.ToString())))
                                                {
                                                    endpointMACAddressStringList.Add(macAddress);
                                                }
                                            }
                                            catch
                                            {
                                            }
                                        }
                                    }

                                    if (!BW_GetStatus.CancellationPending &&
                                        checkOptions.TestPing)
                                    {
                                        if (validationMethod == ValidationMethod.Ping)
                                        {
                                            endpoint.ResponseMessage = GetEnumDescriptionString(EndpointStatus.PINGCHECK);
                                        }

                                        // TEST PING
                                        try
                                        {
                                            string pingRoundtripTime = GetPingTime(responseURI.Host, checkOptions.PingTimeout, 1);

                                            if (!string.IsNullOrEmpty(pingRoundtripTime))
                                            {
                                                endpoint.PingRoundtripTime = pingRoundtripTime;
                                            }
                                        }
                                        catch
                                        {
                                        }
                                    }

                                    // FILL IP ADDRESS(ES) LIST
                                    if (endpointIPAddressesStringList.Count > 0)
                                    {
                                        endpoint.IPAddress = endpointIPAddressesStringList.ToArray();
                                    }

                                    // FILL DNS NAME(S) LIST
                                    if (endpointDNSNamesStringList.Count > 0)
                                    {
                                        endpoint.DNSName = endpointDNSNamesStringList.ToArray();
                                    }

                                    // FILL MAC ADDRESS(ES) LIST
                                    if (endpointMACAddressStringList.Count > 0)
                                    {
                                        endpoint.MACAddress = endpointMACAddressStringList.ToArray();
                                    }

                                    // SET PROGRESS STATUS LABEL
                                    SetProgressStatus(endpointsCount_Enabled, endpointsCount_Current);
                                }
                            }

                            // UPDATE ADDRESSES
                            endpoint.Address = endpointURI.OriginalString;
                            endpoint.ResponseAddress = responseURI.AbsoluteUri;

                            // UPDATE RESPONSE TIME
                            endpoint.ResponseTime = durationTime_Item;

                            // CHECK 'TERMINATED' STATUS
                            if (BW_GetStatus.CancellationPending)
                            {
                                endpoint.ResponseCode = status_NotAvailable;
                                endpoint.ResponseMessage = GetEnumDescriptionString(EndpointStatus.TERMINATED);
                            }
                            else
                            {
                                // UPDATE 'LAST SEEN ONLINE' VALUE
                                if ((validationMethod == ValidationMethod.Protocol &&
                                     endpoint.ResponseCode != status_Error &&
                                     endpoint.ResponseCode != status_NotAvailable) ||
                                    (validationMethod == ValidationMethod.Ping &&
                                     endpoint.PingRoundtripTime != status_NotAvailable))
                                {
                                    endpoint.LastSeenOnline = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                }
                            }

                            // ADD UPDATED STATUS DEFINITION TO LIST
                            updatedEndpointsList.Add(endpoint);
                            }
                            catch (Exception lambdaEx)
                            {
                                // Top-level safety net: no exception should escape the lambda and
                                // crash the whole scan — mark this endpoint as error and continue.
                                try
                                {
                                    updatedEndpointsList.Add(EndpointCheckResultFactory.CreateUnhandledExceptionResult(endpointItem, lambdaEx));
                                }
                                catch { /* if even the fallback fails, skip this endpoint */ }
                            }
                        }
                    });

                // GET PROGRESS DURATION TIME [FOR 'EXPORT' AND 'AUTO ADJUST REFRESH INTERVAL' PURPOSES]
                DateTime endDT_List = DateTime.Now;
                int durationTime_List = (int)(endDT_List - startDT_List).TotalSeconds;

                if (checkOptions.AutoAdjustRefreshTimer)
                {
                    // ADJUST AUTO REFRESH INTERVAL BY LAST PROGRESS DURATION TIME (+ 1 MINUTE RESERVE]
                    decimal durationTime_List_Minutes = durationTime_List / 60000;
                    ThreadSafeInvoke(() =>
                    {
                        if (num_RefreshInterval.Value < durationTime_List_Minutes + 1)
                        {
                            num_RefreshInterval.Value = durationTime_List_Minutes + 1;
                        }
                    });
                }

                // UPDATE ENDPOINTS LIST
                endpointsList = updatedEndpointsList.ToList();

                // UPDATE AND SAVE 'LAST SEEN ONLINE' LIST
                UpdateLastSeenOnlineList();
                SaveLastSeenOnlineList();

                // SORT ENDPOINTS LIST BY ENDPOINT NAME
                endpointsList.Sort((s, t) => string.Compare(s.Name, t.Name));

                // EXPORT UPDATED LIST
                EndpointsStatusExport(
                                      startDT_List.ToString("dd.MM.yyyy HH:mm:ss"),
                                      endDT_List.ToString("dd.MM.yyyy HH:mm:ss"),
                                      durationTime_List,
                                      checkOptions.PingTimeout / 1000,
                                      checkOptions.HttpRequestTimeout / 1000,
                                      checkOptions.FtpRequestTimeout / 1000,
                                      FormatBoolToString(checkOptions.AllowAutoRedirect),
                                      FormatBoolToString(checkOptions.ValidateSslCertificate),
                                      checkOptions.ThreadsCount.ToString(),
                                      FormatBoolToString(checkOptions.ResolveNetworkShares),
                                      FormatBoolToString(checkOptions.ResolvePageMetaInfo),
                                      FormatBoolToString(checkOptions.SaveResponse),
                                      FormatBoolToString(checkOptions.TestPing),
                                      FormatBoolToString(checkOptions.ResolveDnsNames)
                                      );
            }
            catch (Exception eX)
            {
                ThreadSafeInvoke(() =>
                {
                    ExceptionNotify(this, eX, string.Empty, true);
                });
            }
        }

        public string FormatBoolToString(bool boolSwitch)
        {
            if (boolSwitch)
            {
                return "Enabled";
            }
            else
            {
                return "Disabled";
            }
        }

        public void GetSSLCertificateInfo(HttpWebRequest httpWebRequest, EndpointDefinition endpoint)
        {
            if (httpWebRequest.ServicePoint.Certificate != null)
            {
                try
                {
                    X509Certificate2 sslCert2 = new X509Certificate2(httpWebRequest.ServicePoint.Certificate);

                    endpoint.SSLCertificateProperties.PropertyItem.Add(new Property { ItemName = "Archived", ItemValue = sslCert2.Archived.ToString() });
                    endpoint.SSLCertificateProperties.PropertyItem.Add(new Property { ItemName = "Has Private Key", ItemValue = sslCert2.HasPrivateKey.ToString() });
                    endpoint.SSLCertificateProperties.PropertyItem.Add(new Property { ItemName = "Valid To", ItemValue = sslCert2.NotAfter.ToString() });
                    endpoint.SSLCertificateProperties.PropertyItem.Add(new Property { ItemName = "Valid From", ItemValue = sslCert2.NotBefore.ToString() });
                    endpoint.SSLCertificateProperties.PropertyItem.Add(new Property { ItemName = "Version", ItemValue = sslCert2.Version.ToString() });
                    endpoint.SSLCertificateProperties.PropertyItem.Add(new Property { ItemName = "Public Key", ItemValue = sslCert2.GetPublicKeyString() });

                    if (!string.IsNullOrEmpty(sslCert2.SignatureAlgorithm.FriendlyName)) { endpoint.SSLCertificateProperties.PropertyItem.Add(new Property { ItemName = "Signature Algorithm", ItemValue = sslCert2.SignatureAlgorithm.FriendlyName }); };
                    if (!string.IsNullOrEmpty(sslCert2.FriendlyName)) { endpoint.SSLCertificateProperties.PropertyItem.Add(new Property { ItemName = "Friendly Name", ItemValue = sslCert2.FriendlyName }); };
                    if (!string.IsNullOrEmpty(sslCert2.Issuer)) { endpoint.SSLCertificateProperties.PropertyItem.Add(new Property { ItemName = "Issuer Name", ItemValue = sslCert2.Issuer }); };
                    if (!string.IsNullOrEmpty(sslCert2.SerialNumber)) { endpoint.SSLCertificateProperties.PropertyItem.Add(new Property { ItemName = "Serial Number", ItemValue = sslCert2.SerialNumber }); };
                    if (!string.IsNullOrEmpty(sslCert2.Subject)) { endpoint.SSLCertificateProperties.PropertyItem.Add(new Property { ItemName = "Subject", ItemValue = sslCert2.Subject }); };
                    if (!string.IsNullOrEmpty(sslCert2.Thumbprint)) { endpoint.SSLCertificateProperties.PropertyItem.Add(new Property { ItemName = "Thumbprint", ItemValue = sslCert2.Thumbprint }); };
                }
                catch
                {
                }
            }
        }

        public HttpWebRequest PrepareHTTPWebRequest(
            EndpointDefinition endpoint,
            Uri endpointURI,
            int httpRequestTimeout,
            bool allowAutoRedirect,
            bool removeURLParameters,
            CookieCollection cookies = null)
        {
            HttpWebRequest httpWebRequest = EndpointHttpRequestFactory.Create(
                endpoint,
                endpointURI,
                httpRequestTimeout,
                allowAutoRedirect,
                removeURLParameters,
                http_UserAgent,
                cookies);

            // GET REQUEST HEADERS
            GetHTTPWebHeaders(endpoint.HTTPRequestHeaders.PropertyItem, httpWebRequest.Headers);

            return httpWebRequest;
        }

        // CF-RAY is injected by Cloudflare's edge on every proxied response, including
        // bot-challenge pages.  Its presence + a non-2xx status means the origin is
        // reachable but Cloudflare's security layer is blocking automated access —
        // the endpoint is NOT truly down.
        private static bool IsCloudflareProtected(HttpWebResponse response)
        {
            if (response == null) return false;

            return EndpointHttpResponseClassifier.IsCloudflareProtected(response.Headers, response.Server);
        }

        public void GetHTTPWebHeaders(
            List<Property> propertyItemCollection,
            WebHeaderCollection headerCollection)
        {
            propertyItemCollection.Clear();

            if (headerCollection != null &&
                headerCollection.Count > 0)
            {
                foreach (string headerName in headerCollection.Keys)
                {
                    propertyItemCollection.Add(new Property { ItemName = headerName, ItemValue = headerCollection[headerName] });
                }
            }

            propertyItemCollection = propertyItemCollection.OrderBy(p => p.ItemName).ToList();
        }

        public string ReadHTTPResponseStream(MemoryStream httpWebResponseMemoryStream, Encoding encoding)
        {
            // SET MEMORY RESPONSE STREAM POSITION TO BEGINNING AND GET RESPONSE STRING
            httpWebResponseMemoryStream.Seek(0, SeekOrigin.Begin);

            if (encoding != null)
            {
                StreamReader httpWebResponseStreamReader = new StreamReader(httpWebResponseMemoryStream, encoding);
                return HttpUtility.HtmlDecode(httpWebResponseStreamReader.ReadToEnd());
            }
            else
            {
                StreamReader httpWebResponseStreamReader = new StreamReader(httpWebResponseMemoryStream);
                return HttpUtility.HtmlDecode(httpWebResponseStreamReader.ReadToEnd());
            }
        }

        public bool TryParseHttpDate(string httpDate, out DateTime parsedDate)
        {
            // http://tools.ietf.org/html/rfc7231#section-7.1.1.1
            string[] formats = new[] {
                "r",							// preferred
                "dddd, dd-MMM-yy HH:mm:ss GMT",	// obsolete RFC 850 format
                "ddd MMM  d HH:mm:ss yyyy"		// ANSI C's asctime() format
            };

            return DateTime.TryParseExact(httpDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out parsedDate);
        }
        public bool CheckWebResponseContentLength(EndpointDefinition endpoint, HttpWebResponse httpWebResponse, long contentLength)
        {
            if (contentLength > http_SaveResponse_MaxLength_Bytes)
            {
                MessageBox.Show(
                    "Response content is too big for download (" + http_SaveResponse_MaxLength_Bytes + " bytes limit)" +
                    Environment.NewLine +
                    Environment.NewLine +
                    "Endpoint Name:  " + endpoint.Name +
                    Environment.NewLine +
                    "Endpoint URL:  " + httpWebResponse.ResponseUri.AbsoluteUri +
                    Environment.NewLine +
                    "Response Content Type:  " + endpoint.HTTPcontentType +
                    Environment.NewLine +
                    "Response Content Length:  " + endpoint.HTTPcontentLength,
                    "Download Response",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            return contentLength <= http_SaveResponse_MaxLength_Bytes;
        }
        public void GetWebResponseContentLengthString(EndpointDefinition endpoint, long contentLength)
        {
            if (contentLength == -1)
            {
                endpoint.HTTPcontentLength = status_NotAvailable;
            }
            else if (contentLength >= 1073741824)
            {
                endpoint.HTTPcontentLength = (contentLength / 1073741824).ToString("0.00") + " GB";
            }
            else
            {
                endpoint.HTTPcontentLength = contentLength >= 1048576
                    ? (contentLength / 1048576).ToString("0.00") + " MB"
                    : contentLength >= 1024 ? (contentLength / 1024).ToString("0.00") + " kB" : contentLength + " bytes";
            }
        }

        public PropertyItems GetDocumentLinks(Uri responseURI, HtmlAgilityPack.HtmlDocument htmlResponseDOC, string[] elements)
        {
            PropertyItems linksList = new PropertyItems() { PropertyItem = new List<Property>() };

            foreach (string element in elements)
            {
                HtmlNodeCollection elementNodeList = htmlResponseDOC.DocumentNode.SelectNodes("//*/@" + element);

                if (elementNodeList != null)
                {
                    foreach (HtmlNode linkNode in elementNodeList)
                    {
                        foreach (HtmlAttribute linkNodeAttribute in linkNode.Attributes)
                        {
                            if (linkNodeAttribute.Name.ToLower() == element &&
                                !string.IsNullOrEmpty(linkNodeAttribute.Value) &&
                                (linkNodeAttribute.Value.ToLower().StartsWith(Uri.UriSchemeHttp.ToLower()) ||
                                 linkNodeAttribute.Value.ToLower().StartsWith(Uri.UriSchemeHttps.ToLower()) ||
                                 linkNodeAttribute.Value.ToLower().StartsWith(Uri.UriSchemeFtp.ToLower())))
                            {
                                if (linksList.PropertyItem.Where(item => item.ItemValue.ToLower().TrimEnd('/') == linkNodeAttribute.Value.ToLower().TrimEnd('/')).Count() == 0 &&
                                    linksList.PropertyItem.Where(item => item.ItemValue.ToLower() == linkNodeAttribute.Value.ToLower()).Count() == 0 &&
                                    responseURI.AbsoluteUri.ToLower().TrimEnd('/') != linkNodeAttribute.Value.ToLower().TrimEnd('/'))
                                {
                                    linksList.PropertyItem.Add(new Property { ItemName = linkNodeAttribute.Name, ItemValue = linkNodeAttribute.Value.TrimEnd('/') });
                                }
                            }
                        }
                    }
                }
            }

            return linksList;
        }

        public void ResolvePageMetaInfo(string htmlResponseDocumentString, EndpointDefinition endpoint, Uri responseURI, bool resolvePageLinks)
        {
            // REPLACE SPECIAL CHARACTERS
            htmlResponseDocumentString = htmlResponseDocumentString.Replace(Environment.NewLine, string.Empty).Replace('\'', '"').Replace("&nbsp", " ");

            endpoint.HTMLMetaInfo.PropertyItem.Clear();
            endpoint.HTMLTitle = status_NotAvailable;
            endpoint.HTMLDescription = status_NotAvailable;
            endpoint.HTMLAuthor = status_NotAvailable;

            // LOAD HTML DOCUMENT
            HtmlAgilityPack.HtmlDocument htmlResponseDOC = new HtmlAgilityPack.HtmlDocument();
            htmlResponseDOC.LoadHtml(htmlResponseDocumentString);
            htmlResponseDOC.OptionFixNestedTags = true;

            if (resolvePageLinks)
            {
                // GET PAGE LINKS
                endpoint.HTMLPageLinks = GetDocumentLinks(responseURI, htmlResponseDOC, new string[] { "href", "src" });
            }

            // GET ROOT 'html' NODE
            HtmlNode[] htmlRootNodeList = htmlResponseDOC.DocumentNode.Descendants().Where(node => node.Name.ToLower() == "html").ToArray();

            foreach (HtmlNode htmlRootNode in htmlRootNodeList)
            {
                if (endpoint.HTMLContentLanguage == status_NotAvailable)
                {
                    // GET DOCUMENT LANGUAGE [ROOT]
                    foreach (HtmlAttribute rootNodeAttribute in htmlRootNode.Attributes)
                    {
                        if (rootNodeAttribute.Name.ToLower().Contains("lang") &&
                            !string.IsNullOrEmpty(rootNodeAttribute.Value))
                        {
                            GetContentLanguage(endpoint, rootNodeAttribute.Value);
                        }
                    }
                }

                // GET 'head' NODE
                HtmlNode[] htmlHeadNodeList = htmlRootNode.Descendants().Where(node => node.Name.ToLower() == "head").ToArray();

                foreach (HtmlNode htmlHeadNode in htmlHeadNodeList)
                {
                    if (endpoint.HTMLTitle == status_NotAvailable)
                    {
                        // GET PAGE 'TITLE'
                        foreach (HtmlNode htmlNodeChild in htmlHeadNode.ChildNodes)
                        {
                            if (htmlNodeChild.OriginalName.ToLower() == "title" &&
                                !string.IsNullOrEmpty(htmlNodeChild.InnerText.TrimStart().TrimEnd()))
                            {
                                endpoint.HTMLTitle = htmlNodeChild.InnerText.TrimStart().TrimEnd();

                                break;
                            }
                        }
                    }

                    foreach (HtmlNode htmlNodeChild in htmlHeadNode.ChildNodes)
                    {
                        // GET META TAG[S]
                        if (htmlNodeChild.OriginalName.ToLower() == "meta")
                        {
                            GetMetaTag(endpoint, htmlNodeChild);
                        }
                    }
                }
            }

            // GET DOCUMENT LANGUAGE[META]
            if (endpoint.HTMLContentLanguage == status_NotAvailable)
            {
                string _contentLanguage = GetMetaInfoValueByKey(endpoint, "content-language");

                if (_contentLanguage != status_NotAvailable)
                {
                    GetContentLanguage(endpoint, _contentLanguage);
                }
            }

            // GET PAGE 'AUTHOR'
            endpoint.HTMLAuthor = GetMetaInfoValueByKey(endpoint, "author");

            // [HACK] OPTIONAL 'autor'
            if (endpoint.HTMLAuthor == status_NotAvailable)
            {
                endpoint.HTMLAuthor = GetMetaInfoValueByKey(endpoint, "autor");
            }

            // [HACK] OPTIONAL 'web_author'
            if (endpoint.HTMLAuthor == status_NotAvailable)
            {
                endpoint.HTMLAuthor = GetMetaInfoValueByKey(endpoint, "web_author");
            }

            // GET PAGE 'DESCRIPTION'
            endpoint.HTMLDescription = GetMetaInfoValueByKey(endpoint, "description");

            // GET PAGE 'THEME COLOR'
            string _colorThemeCodeString = GetMetaInfoValueByKey(endpoint, "theme-color");

            if (_colorThemeCodeString != status_NotAvailable)
            {
                try
                {
                    endpoint.HTMLThemeColor = ColorTranslator.FromHtml(_colorThemeCodeString);
                }
                catch
                {
                }
            }

            // GET DEFAULT STREAM ENCODING
            endpoint.HTMLdefaultStreamEncoding = htmlResponseDOC.StreamEncoding;
        }

        public void GetContentLanguage(EndpointDefinition endpoint, string contentLanguage)
        {
            if (contentLanguage.ToLower() == "mul")
            {
                endpoint.HTMLContentLanguage = "Multi-Language (mul)";
            }
            else
            {
                try
                {
                    endpoint.HTMLContentLanguage = new CultureInfo(contentLanguage).NativeName;
                }
                catch
                {
                }
            }
        }

        public void GetMetaTag(EndpointDefinition endpoint, HtmlNode subNode)
        {
            // GET META TAGS
            string metaName = string.Empty;
            string metaValue = string.Empty;

            foreach (HtmlAttribute subNodeAttribute in subNode.Attributes)
            {
                if (subNodeAttribute.OriginalName.ToLower() == "charset")
                {
                    // CHARSET [ENCODING FROM HEAD ROOT]
                    GetEncodingFromMetaTag(endpoint, "charset=" + subNodeAttribute.Value);
                }
                else if (subNodeAttribute.OriginalName.ToLower() == "http-equiv" ||
                         subNodeAttribute.OriginalName.ToLower() == "name" ||
                         subNodeAttribute.OriginalName.ToLower() == "property")
                {
                    // META TAG NAME
                    metaName = subNodeAttribute.Value.TrimStart().TrimEnd();
                }
                else if (subNodeAttribute.OriginalName.ToLower() == "content")
                {
                    // [HACK] META TAG VALUE FIX
                    metaValue = subNodeAttribute.Value.Replace("<br>", string.Empty).Replace("\n", " ").TrimStart().TrimEnd();
                }
            }

            if (!string.IsNullOrEmpty(metaName))
            {
                endpoint.HTMLMetaInfo.PropertyItem.Add(new Property { ItemName = metaName, ItemValue = metaValue });

                // TITLE [OPTIONAL, FROM HTML META]
                if (endpoint.HTMLTitle == status_NotAvailable &&
                    (metaName.ToLower() == "title" ||
                     (metaName.ToLower().Split(':').Length > 1 &&
                      metaName.ToLower().Split(':')[1] == "title")) &&
                    !string.IsNullOrEmpty(metaValue))
                {
                    endpoint.HTMLTitle = metaValue;
                }

                // [HACK] CHARSET [ENCODING FROM HTML META] ->> GET LAST DEFINITION FOR CASES, IF MORE ENCODING TAGS ARE PRESENT
                if (metaName.ToLower() == "content-type")
                {
                    GetEncodingFromMetaTag(endpoint, metaValue);
                }
            }
        }

        public void GetEncodingFromMetaTag(EndpointDefinition endpoint, string encodingValue)
        {
            if (endpoint.HTMLencoding == null)
            {
                Encoding htmlMetaEncoding = GetEncoding(encodingValue);

                if (htmlMetaEncoding != null)
                {
                    endpoint.HTMLencoding = htmlMetaEncoding;
                }
            }
        }

        public static string GetPingTime(string host, int timeout, int maxRetryCount, int retryCount = 0)
        {
            string pingTime = string.Empty;

            PingReply pingReply = new Ping().Send(host, timeout);
            if (pingReply.Status == IPStatus.Success)
            {
                pingTime = pingReply.RoundtripTime.ToString() + " ms";
            }
            else if (pingReply.Status == IPStatus.TimedOut &&
                     retryCount < maxRetryCount)
            {
                retryCount++;
                pingTime = GetPingTime(host, timeout, maxRetryCount, retryCount);
            }

            return pingTime;
        }

        public static HttpWebResponse GetHTTPWebResponse(HttpWebRequest httpWebRequest, int maxRetryCount, int retryCount = 0)
        {
            HttpWebResponse webResponse;

            try
            {
                webResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            }
            catch (WebException webException)
            {
                if (webException.Status == WebExceptionStatus.Timeout &&
                    retryCount < maxRetryCount)
                {
                    retryCount++;
                    webResponse = GetHTTPWebResponse(httpWebRequest, maxRetryCount, retryCount);
                }
                else
                {
                    throw webException;
                }
            }

            return webResponse;
        }

        public string GetContentType(string valueString)
        {
            string contentType = status_NotAvailable;

            if (!string.IsNullOrEmpty(valueString))
            {
                string[] valueStringArray = valueString.Split(new char[]
                                        {
                                            ' ',
                                            ';',
                                            ','
                                        });

                foreach (string value in valueStringArray)
                {
                    if (value.ToLower().Contains("/"))
                    {
                        // PARSE AND FORMAT STRING
                        contentType = value
                            .ToLower()
                            .Replace("<", string.Empty)  // <
                            .Replace(">", string.Empty)  // >
                            .Replace("\"", string.Empty) // "
                            .Replace("'", string.Empty)  // '
                            .Replace(@"\", string.Empty) // \
                            .TrimStart()
                            .TrimEnd();
                    }
                }
            }

            return contentType;
        }

        public Encoding GetEncoding(string valueString)
        {
            Encoding encoding = null;

            if (!string.IsNullOrEmpty(valueString.TrimStart().TrimEnd()))
            {
                string[] valueStringArray = valueString.Split(new char[]
                                        {
                                            ' ',
                                            ';',
                                            ',',
                                            '"'
                                        });

                foreach (string value in valueStringArray)
                {
                    if (value.ToLower().Contains("charset="))
                    {
                        try
                        {
                            // PARSE AND FORMAT ENCODING STRING
                            string formattedValue = value
                            .ToLower()
                            .Replace("charset=", string.Empty)
                            .Replace("<", string.Empty)  // <
                            .Replace(">", string.Empty)  // >
                            .Replace("\"", string.Empty) // "
                            .Replace("'", string.Empty)  // '
                            .Replace("/", string.Empty)  // /
                            .Replace(@"\", string.Empty) // \
                            .Replace("utf7", "utf-7")
                            .Replace("utf8", "utf-8")
                            .Replace("utf32", "utf-32")
                            .Replace("cp1250", "windows-1250")
                            .TrimStart()
                            .TrimEnd();

                            if (!string.IsNullOrEmpty(formattedValue))
                            {
                                // GET ENCODING
                                encoding = Encoding.GetEncoding(formattedValue);
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }

            return encoding;
        }

        public static string GetEncodingName(Encoding encoding)
        {
            return encoding != null &&
                !string.IsNullOrEmpty(encoding.EncodingName)
                ? encoding.EncodingName
                : status_NotAvailable;
        }

        public string GetMetaInfoValueByKey(EndpointDefinition endpoint, string key)
        {
            string metaValue = string.Empty;

            if (endpoint.HTMLMetaInfo.PropertyItem != null)
            {
                // TRY TO GET META INFO VALUE BY KEY
                if (endpoint.HTMLMetaInfo.PropertyItem.Where(metaInfo => metaInfo.ItemName.ToLower() == key.ToLower()).Count() > 0)
                {
                    metaValue = endpoint.HTMLMetaInfo.PropertyItem.Where
                                    (metaInfo => metaInfo.ItemName.ToLower() == key.ToLower())
                                        .FirstOrDefault().ItemValue.TrimStart().TrimEnd();
                }

                if (string.IsNullOrEmpty(metaValue.TrimStart().TrimEnd()))
                {
                    // TRY TO GET OPTIONAL META INFO VALUE BY KEY
                    if (endpoint.HTMLMetaInfo.PropertyItem.Where(metaInfo => metaInfo.ItemName.ToLower().Split(':').Length > 1 &&
                                                                 metaInfo.ItemName.ToLower().Split(':')[1] == key.ToLower()).Count() > 0)
                    {
                        foreach (Property metaInfo in endpoint.HTMLMetaInfo.PropertyItem)
                        {
                            if (metaInfo.ItemName.Split(':').Length > 1 &&
                                metaInfo.ItemName.Split(':')[1].ToLower() == key.ToLower())
                            {
                                metaValue = metaInfo.ItemValue.TrimStart().TrimEnd();
                            }
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(metaValue))
            {
                metaValue = status_NotAvailable;
            }

            return metaValue;
        }

        public string GetFileExtensionByContentType(string mimeType)
        {
            RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type\" + mimeType, false);
            object value = key?.GetValue("Extension", null);
            return value != null ? value.ToString() : string.Empty;
        }

        public void SaveWebResponseStream(DateTime timeStamp, string endpointName, byte[] htmlResponseByteArray, string fileExtension)
        {
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(new string(Path.GetInvalidFileNameChars()))));
            string htmlResponseDirectory = timeStamp.ToString("yyyy-MM-dd HH-mm-ss");
            string htmlResponseFileName = r
                                           .Replace(endpointName, "-")
                                           .TrimStart()
                                           .TrimEnd()
                                           .TrimEnd('-')
                                           .Replace("--", "-")
                                           .Replace("--", "-")
                                           .Replace(".", "-");

            string htmlResponseFullPath = Path.Combine(htmlResponseDirectory, htmlResponseFileName + fileExtension);

            if (!Directory.Exists(htmlResponseDirectory))
            {
                // CREATE OUTPUT DIRECTORY [IN CURRENT DIRECTORY]
                Directory.CreateDirectory(htmlResponseDirectory);
            }

            // SAVE RESPONSE
            Application.DoEvents();

            using (FileStream htmlResponseFileStream = new FileStream(htmlResponseFullPath, FileMode.Create))
            {
                htmlResponseFileStream.Write(htmlResponseByteArray, 0, htmlResponseByteArray.Length);
            }

            Application.DoEvents();
        }

        public void FTPWebResponseStatusMessage(FtpWebResponse ftpWebResponse, WebException webException, EndpointDefinition endpoint)
        {
            endpoint.ResponseCode = status_Error;
            endpoint.ResponseMessage = status_NotAvailable;

            if (ftpWebResponse == null &&
                webException != null &&
                webException.Response != null)
            {
                // RESPONSE IS NULL, TRY TO HANDLE WEBEXCEPTION.RESPONSE
                try
                {
                    ftpWebResponse = webException.Response as FtpWebResponse;
                }
                catch
                {
                }
            }

            if (ftpWebResponse != null &&
                ftpWebResponse.StatusCode != FtpStatusCode.Undefined)
            {
                // HANDLE STATUS CODE
                endpoint.ResponseCode = ((int)ftpWebResponse.StatusCode).ToString();

                // HANDLE STATUS MESSAGE
                if (!string.IsNullOrEmpty(ftpWebResponse.StatusDescription) &&
                    !endpoint.ResponseCode.StartsWith("2"))
                {
                    endpoint.ResponseMessage = ftpWebResponse.StatusDescription
                        .Replace(endpoint.ResponseCode, string.Empty) // REMOVE ACTUAL STATUS CODE FROM MESSAGE, IF PRESENT
                        .TrimStart('-').TrimStart().TrimEnd(); // REMOVE SPACES AND '-' CHARACTER FROM START AND END OF MESSAGE, IF PRESENT
                }
                else if (!string.IsNullOrEmpty(ftpWebResponse.BannerMessage) &&
                         ftpWebResponse.BannerMessage.StartsWith(((int)FtpStatusCode.SendUserCommand).ToString()))
                {
                    endpoint.ResponseCode = ((int)FtpStatusCode.SendUserCommand).ToString();
                    endpoint.ResponseMessage = ftpWebResponse.BannerMessage
                        .Replace(endpoint.ResponseCode, string.Empty) // REMOVE 'SendUserCommand' [220] STATUS CODE FROM MESSAGE, IF PRESENT
                        .TrimStart('-').TrimStart().TrimEnd(); // REMOVE SPACES AND '-' CHARACTER FROM START AND END OF MESSAGE, IF PRESENT
                }
                else if (!string.IsNullOrEmpty(ftpWebResponse.WelcomeMessage) &&
                         ftpWebResponse.WelcomeMessage.StartsWith(((int)FtpStatusCode.LoggedInProceed).ToString()))
                {
                    endpoint.ResponseCode = ((int)FtpStatusCode.LoggedInProceed).ToString();
                    endpoint.ResponseMessage = ftpWebResponse.WelcomeMessage
                        .Replace(endpoint.ResponseCode, string.Empty) // REMOVE 'LoggedInProceed' [230] STATUS CODE FROM MESSAGE, IF PRESENT
                        .TrimStart('-').TrimStart().TrimEnd(); // REMOVE SPACES AND '-' CHARACTER FROM START AND END OF MESSAGE, IF PRESENT
                }
                else if (!string.IsNullOrEmpty(ftpWebResponse.StatusDescription))
                {
                    endpoint.ResponseMessage = ftpWebResponse.StatusDescription
                        .Replace(endpoint.ResponseCode, string.Empty) // REMOVE ACTUAL STATUS CODE FROM MESSAGE, IF PRESENT
                        .TrimStart('-').TrimStart().TrimEnd(); // REMOVE SPACES AND '-' CHARACTER FROM START AND END OF MESSAGE, IF PRESENT
                }

                // BANNER MESSAGE
                if (!string.IsNullOrEmpty(ftpWebResponse.BannerMessage))
                {
                    endpoint.FTPBannerMessage = ftpWebResponse.BannerMessage.TrimStart().TrimEnd();
                }

                // WELCOME MESSAGE
                if (!string.IsNullOrEmpty(ftpWebResponse.WelcomeMessage))
                {
                    endpoint.FTPWelcomeMessage = ftpWebResponse.WelcomeMessage.TrimStart().TrimEnd();
                }

                // EXIT MESSAGE
                if (!string.IsNullOrEmpty(ftpWebResponse.ExitMessage))
                {
                    endpoint.FTPExitMessage = ftpWebResponse.ExitMessage.TrimStart().TrimEnd();
                }

                // STATUS DESCRIPTION
                if (!string.IsNullOrEmpty(ftpWebResponse.StatusDescription))
                {
                    endpoint.FTPStatusDescription = ftpWebResponse.StatusDescription.TrimStart().TrimEnd();
                }
            }
            else if (webException != null)
            {
                // STATUS
                endpoint.ResponseMessage = webException.Status.ToString();

                // MESSAGE
                if (!string.IsNullOrEmpty(webException.Message))
                {
                    endpoint.ResponseMessage += " -> " + webException.Message;
                }

                // INNER EXCEPTION MESSAGE
                if (webException.InnerException != null &&
                    !string.IsNullOrEmpty(webException.InnerException.Message) &&
                    !endpoint.ResponseMessage.Contains(webException.InnerException.Message))
                {
                    endpoint.ResponseMessage += " -> " + webException.InnerException.Message;
                }
            }
        }

        public void SetProgressStatus(
                                      int endpointsCount_Enabled,
                                      int endpointsCount_Current,
                                      string statusMessage = null,
                                      Color? statusMessageColor = null)
        {
            Color statusColor = Color.Black;

            if (statusMessageColor.HasValue)
            {
                statusColor = statusMessageColor.Value;
            }

            if (statusMessage == null)
            {
                // GET ACTIVE THREADS COUNT
                ThreadPool.GetMaxThreads(out int threadCountMax_WT, out int threadCountMax_CPT);
                ThreadPool.GetAvailableThreads(out int threadCountAvailable_WT, out int threadCountAvailable_CPT);
                int threadCountUsed = threadCountMax_WT - threadCountAvailable_WT;

                // SET STATUS INFORMATION
                if (BW_GetStatus.CancellationPending)
                {
                    // TERMINATING
                    statusColor = Color.Red;
                    statusMessage = "Terminating";

                    if (onClose)
                    {
                        // CLOSING
                        statusMessage += " and Closing";
                    }
                }
                else
                {
                    // PROGRESS
                    statusColor = Color.Green;
                    statusMessage = "Checking Endpoint " +
                        endpointsCount_Current + " of " + endpointsCount_Enabled;
                }

                // ADD ACTIVE THREAD COUNT TO STATUS
                statusMessage += " [" + GetFormattedValueCountString(threadCountUsed, "thread", true) + "] ...";

                // SET TASKBAR PROGRESS
                if (Visible &&
                    Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.IsPlatformSupported)
                {
                    // TaskbarManager uses STA COM; guard against failures on MTA thread-pool threads
                    try
                    {
                        Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager taskBarInstance = Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance;

                        if (BW_GetStatus.CancellationPending ||
                            onClose)
                        {
                            taskBarInstance.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.Paused);
                            taskBarInstance.SetProgressValue(100, 100);
                        }
                        else
                        {
                            taskBarInstance.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.Normal);
                            taskBarInstance.SetProgressValue(endpointsCount_Current, endpointsCount_Enabled);
                        }
                    }
                    catch { }
                }
            }

            // SET STATUS LABEL TEXT AND COLOR
            ThreadSafeInvoke(() =>
            {
                Application.DoEvents();
                lbl_ProgressCount.ForeColor = statusColor;
                lbl_ProgressCount.Text = statusMessage;
                if (endpointsCount_Enabled > 0)
                {
                    pb_RefreshProcess.Maximum = endpointsCount_Enabled;
                    pb_RefreshProcess.Value   = endpointsCount_Current;
                }
                Application.DoEvents();
            });
        }

        public void btn_RunCheck_Click(object sender, EventArgs e)
        {
            if (IsHandleCreated &&
                !BW_GetStatus.IsBusy &&
                !onClose &&
                dialog_EndpointDetails == null &&
                dialog_SpeedTest == null)
            {
                if (lv_Endpoints.Items.Count > 0 && lv_Endpoints.CheckedItems.Count == 0)
                {
                    SetCheckButtons(false);
                    ListEndpoints(ListViewRefreshMethod.CheckAll);
                    SetCheckButtons(true);
                }

                if (lv_Endpoints.CheckedItems.Count == 0)
                {
                    return;
                }

                btn_RunCheck.Enabled = false;

                SetControls(true, true);

                BW_GetStatus.RunWorkerAsync();
            }
        }

        public void SetControls(bool inProgress, bool locked)
        {
            // SET TRAY CONTROLS (TRAY ICON AND TOOLTIP TEXT)
            SetTrayControls(inProgress);

            // VISIBLE OR ENABLED DURING PROGRESS
            btn_Terminate.Enabled = inProgress && locked;
            lbl_Terminate.Enabled = inProgress && locked;
            lbl_ProgressCount.Visible = inProgress && locked;
            pb_RefreshProcess.Visible = inProgress && locked;
            if (groupBox_ScanProgress != null)
            {
                groupBox_ScanProgress.Enabled = true;
            }
            LayoutEndpointListOverlays();
            if (inProgress && locked)
                pb_RefreshProcess.StartAnimation();
            else
                pb_RefreshProcess.StopAnimation();

            // NOT VISIBLE OR ENABLED DURING PROGRESS
            SetCheckButtons(!inProgress && !locked);
            lbl_LoadList.Enabled = !inProgress && !locked;
            btn_LoadList.Enabled = !inProgress && !locked;
            groupBox_Export.Enabled = true;
            groupBox_CommonOptions.Enabled = true;
            groupBox_HTTPOptions.Enabled = true;
            lv_Endpoints.CheckBoxes = !inProgress;
            comboBox_Validate.Enabled = !inProgress && !locked;
            lbl_Validate.Enabled = !inProgress && !locked;
            lbl_AutomaticRefresh.Enabled = !inProgress && !locked;
            cb_RefreshOnStartup.Enabled = !inProgress && !locked;
            cb_AutomaticRefresh.Enabled = !inProgress && !locked;
            cb_ContinuousRefresh.Enabled = !inProgress && !locked;
            cb_TrayBalloonNotify.Enabled = !inProgress && !locked;
            cb_AllowAutoRedirect.Enabled = !inProgress && !locked;
            cb_ValidateSSLCertificate.Enabled = !inProgress && !locked;
            cb_TestPing.Enabled = !inProgress && !locked;
            cb_Resolve_DNS_Names.Enabled = !inProgress && !locked;
            cb_Resolve_IPAddresses.Enabled = !inProgress && !locked;
            cb_Resolve_NIC_MACs.Enabled = !inProgress && !locked;
            cb_RefreshAutoSet.Enabled = !inProgress && !locked;
            cb_ResolveNetworkShares.Enabled = !inProgress && !locked;
            cb_ExportEndpointsStatus_XLSX.Enabled = !inProgress && !locked;
            cb_ExportEndpointsStatus_JSON.Enabled = !inProgress && !locked;
            cb_ExportEndpointsStatus_XML.Enabled = !inProgress && !locked;
            cb_ExportEndpointsStatus_HTML.Enabled = !inProgress && !locked;
            cb_ResolvePageMetaInfo.Enabled = !inProgress && !locked;
            cb_RemoveURLParameters.Enabled = !inProgress && !locked;
            cb_ResolvePageLinks.Enabled = !inProgress && !locked;
            cb_SaveResponse.Enabled = !inProgress && !locked;
            num_RefreshInterval.Enabled = !inProgress && !locked;
            num_PingTimeout.Enabled = !inProgress && !locked;
            lbl_PingTimeout.Enabled = !inProgress && !locked;
            lbl_PingTimeoutSecondsText.Enabled = !inProgress && !locked;
            num_HTTPRequestTimeout.Enabled = !inProgress && !locked;
            lbl_RequestTimeout.Enabled = !inProgress && !locked;
            lbl_RequestTimeoutSecondsText.Enabled = !inProgress && !locked;
            num_FTPRequestTimeout.Enabled = !inProgress && !locked;
            lbl_FTPRequestTimeout.Enabled = !inProgress && !locked;
            lbl_FTPRequestTimeoutSecondsText.Enabled = !inProgress && !locked;
            lbl_TimerIntervalMinutesText.Enabled = !inProgress && !locked;
            num_ParallelThreadsCount.Enabled = !inProgress && !locked;
            lbl_ParallelThreadsCount.Enabled = !inProgress && !locked;
            tray_Separator_1.Visible = !inProgress && !locked && dialog_SpeedTest == null && dialog_EndpointDetails == null;
            tray_RunCheck.Visible = !inProgress && !locked && dialog_SpeedTest == null && dialog_EndpointDetails == null;
            tray_SpeedTest.Visible = !inProgress && !locked && dialog_SpeedTest == null && dialog_EndpointDetails == null;
            btn_BrowseExportDir.Enabled = !inProgress && !locked;
            mainMenu_SpeedTest.Enabled = !inProgress && !locked;
            lbl_RunCheck.Enabled = !inProgress && !locked;
            btn_RunCheck.Enabled = !inProgress && !locked;
            lbl_BrowseExportDir.Enabled = !inProgress && !locked;

            lbl_ListFilter.Enabled = !inProgress && !locked && endpointsList.Count > 0;
            tb_ListFilter.Enabled = !inProgress && !locked && endpointsList.Count > 0;

            pb_ListFilterClear.Visible = !inProgress && !locked && endpointsList.Count > 0 && !string.IsNullOrEmpty(tb_ListFilter.Text);

            tb_ListFilter.BackColor = string.IsNullOrEmpty(tb_ListFilter.Text) ? Color.LightGray : lv_Endpoints.Items.Count > 0 ? Color.Honeydew : Color.MistyRose;
            ApplyPremiumControlState(inProgress, locked);
            ApplyBottomPanelsLayout();
        }

        public void SetTrayControls(bool inProgress)
        {
            if (inProgress)
            {

                if (onClose)
                {
                    // TERMINATING PROCESS AND CLOSING APP
                    // SET TRAY ICON [BLINKING RED DOT ICON]
                    SetTrayIcon(21, 23, 500);

                    // SET TRAY TOOLTIP MESSAGE
                    SetTrayTooltipText(Environment.NewLine + "Terminating process and closing app ...");
                }
                else if (BW_GetStatus.CancellationPending)
                {
                    // TERMINATING PROCESS
                    // SET TRAY ICON [SPINNING WHEEL ANIMATION]
                    SetTrayIcon(12, 20);

                    // SET TRAY TOOLTIP MESSAGE
                    SetTrayTooltipText(Environment.NewLine + "Terminating process ...");
                }
                else
                {
                    // IN PROGRESS
                    // SET TRAY ICON [SPINNING WHEEL ANIMATION]
                    SetTrayIcon(12, 20);

                    // SET TRAY TOOLTIP MESSAGE
                    SetTrayTooltipText(Environment.NewLine + "Endpoints check in progress ...");
                }

                // CLEAR PROGRESS COUNTER INFORMATION LABEL
                SetProgressStatus(0, 0, string.Empty);
            }
            else if (Visible && Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.IsPlatformSupported)
            {
                // SET TASKBAR PROGRESS TO 'NO PROGRESS'
                Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance
                    .SetProgressState(
                    Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.NoProgress);
            }
        }

        public int GetStatusImageIndex(string statusCode, string pingTime, string statusMessage)
        {
            if (statusCode == status_NotAvailable)
            {
                if (statusMessage == GetEnumDescriptionString(EndpointStatus.NOTCHECKED) ||
                    statusMessage == GetEnumDescriptionString(EndpointStatus.TERMINATED))
                {
                    // NOT CHECKED / TERMINATED
                    return 3;
                }
                else if (statusMessage == GetEnumDescriptionString(EndpointStatus.DISABLED))
                {
                    // DISABLED
                    return 6;
                }
            }

            if (validationMethod == ValidationMethod.Protocol)
            {
                if (statusMessage == GetEnumDescriptionString(EndpointStatus.PINGCHECK))
                {
                    // PING CHECK ONLY
                    return 11;
                }
                else if (statusCode == status_Error)
                {
                    // ERROR
                    return 2;
                }
                else if (statusCode[0].ToString() == "2")
                {
                    // SUCCESS [PROTOCOL CODE - 2xx]
                    return 0;
                }
                else if (statusCode[0].ToString() == "4")
                {
                    // ERROR [PROTOCOL CODE - 4xx]
                    return 1;
                }
                else
                {
                    // WARNING [PROTOCOL CODE]
                    return 5;
                }
            }
            else
            {
                if (pingTime == status_NotAvailable)
                {
                    // ERROR
                    return 1;
                }
                else
                {
                    // SUCCESS
                    return 0;
                }
            }
        }

        public void bw_GetStatus_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!onClose)
            {
                // UPDATE LIST
                ListEndpoints(ListViewRefreshMethod.CurrentState);

                // LAST UPDATE LABEL AND ICON
                lbl_LastUpdate.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
                pb_LastUpdate.Visible = true;
                lbl_LastUpdate.Visible = true;
                lbl_LastUpdate_Label.Visible = true;

                // TRAY ICON
                RefreshTrayIcon();

                // GARBAGE COLLECTOR
                GC.Collect();
                GC.WaitForPendingFinalizers();

                // CONTINUOUS REFRESH
                if (cb_ContinuousRefresh.Checked)
                {
                    TIMER_ContinuousRefresh.Start();
                }
            }
            else
            {
                Close();
            }
        }

        public void TIMER_AutomaticRefresh_Tick(object sender, EventArgs e)
        {
            if (btn_RunCheck.Enabled)
            {
                btn_RunCheck_Click(this, null);
            }
        }

        public void num_RefreshInterval_ValueChanged(object sender, EventArgs e)
        {
            lbl_TimerIntervalMinutesText.Text = GetFormattedValueCountString((int)num_RefreshInterval.Value, "minute");

            SaveConfiguration();
            TIMER_AutomaticRefresh.Interval = (int)num_RefreshInterval.Value * 60000;
        }

        public void RefreshTrayIcon()
        {
            if (!listUpdating)
            {
                int itemsOKCount = 0;
                int itemsNotCheckedCount = 0;

                List<string> itemsWarning = new List<string>();
                List<string> itemsError = new List<string>();

                if (!BW_GetStatus.IsBusy)
                {
                    foreach (ListViewItem item in lv_Endpoints.Items)
                    {
                        if (!endpointsList_Disabled.Contains(item.Text))
                        {
                            if (item.ImageIndex == 1)
                            {
                                itemsError.Add(item.Text + " (Code " + item.SubItems[6].Text + ")");
                            }
                            else if (item.ImageIndex == 2)
                            {
                                itemsError.Add(item.Text + " (Response Error)");
                            }
                            else if (item.ImageIndex == 5)
                            {
                                itemsWarning.Add(item.Text + " (Code " + item.SubItems[6].Text + ")");
                            }
                            else if (item.ImageIndex == 0)
                            {
                                itemsOKCount++;
                            }
                            else
                            {
                                itemsNotCheckedCount++;
                            }
                        }
                        else
                        {
                            itemsNotCheckedCount++;
                        }
                    }

                    string toolTipText = Environment.NewLine;

                    if (!string.IsNullOrEmpty(lbl_LastUpdate.Text))
                    {
                        toolTipText += "Last Update: " + lbl_LastUpdate.Text;
                        toolTipText += Environment.NewLine;
                    }

                    if (itemsOKCount > 0)
                    {
                        toolTipText += "Success: " + itemsOKCount;
                        toolTipText += Environment.NewLine;
                    }

                    if (itemsWarning.Count > 0)
                    {
                        toolTipText += "Warning: " + itemsWarning.Count;
                        toolTipText += Environment.NewLine;
                    }

                    if (itemsError.Count > 0)
                    {
                        toolTipText += "ERROR: " + itemsError.Count;
                        toolTipText += Environment.NewLine;
                    }

                    SetTrayTooltipText(toolTipText);

                    if (!cb_AutomaticRefresh.Checked && !cb_ContinuousRefresh.Checked)
                    {
                        // NOT REFRESHING
                        SetTrayIcon(11, 11);

                        SetTrayTooltipText(
                                           Environment.NewLine +
                                           "Endpoints List Automatic / Continuous Refresh Disabled");
                    }
                    else if (itemsError.Count > 0)
                    {
                        // ERROR
                        SetTrayIcon(1, 1);
                    }
                    else if (itemsWarning.Count > 0)
                    {
                        // WARNING
                        SetTrayIcon(5, 5);
                    }
                    else if (itemsOKCount > 0)
                    {
                        // SUCCESS
                        SetTrayIcon(0, 0);
                    }
                    else if (itemsNotCheckedCount > 0)
                    {
                        SetTrayIcon(6, 6);

                        SetTrayTooltipText(
                                           Environment.NewLine +
                                           "Not Any Endpoint Checked");
                    }
                    else
                    {
                        // NO DEFINITIONS ON LIST
                        SetTrayIcon(2, 2);

                        SetTrayTooltipText(
                                           Environment.NewLine +
                                           "Not Any Endpoint Defined");
                    }
                }

                if (itemsError.Count > 0)
                {
                    foreach (string warning in itemsWarning)
                    {
                        itemsError.Add(warning);
                    }

                    ShowTrayBalloonTip(itemsError, "State Errors", ToolTipIcon.Error, 30000);
                }
                else if (itemsWarning.Count > 0)
                {
                    ShowTrayBalloonTip(itemsWarning, "State Warnings", ToolTipIcon.Warning, 30000);
                }
            }
        }

        public Icon GetIconFromListByIndex(int index)
        {
            IntPtr UnmanagedIconHandle = ((Bitmap)ResizeImage(
                imageList_Icons_32pix.Images[index], 16, 16))
                .GetHicon();

            // Clone FromHandle result so we can destroy the unmanaged handle version of the icon before the converted object is passed out.
            Icon icon = Icon.FromHandle(UnmanagedIconHandle).Clone() as Icon;

            // Unfortunately, GetHicon creates an unmanaged handle which must be manually destroyed otherwise a generic error will occur in GDI+.
            DestroyIcon(UnmanagedIconHandle);

            return icon;
        }

        public void trayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (Visible)
                {
                    Hide();
                }
                else
                {
                    RestoreFromTray();
                }
            }
        }

        public void lv_Endpoints_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (!listUpdating)
            {
                if (e.Item.Checked)
                {
                    endpointsList_Disabled.Remove(e.Item.Text);
                }
                else
                {
                    endpointsList_Disabled.Add(e.Item.Text);
                }

                RefreshTrayIcon();
                btn_RunCheck.Enabled = lv_Endpoints.Items.Count > 0;
            }
        }

        public void SetTrayTooltipText(string text)
        {
            try
            {
                Type t = typeof(NotifyIcon);
                BindingFlags hidden = BindingFlags.NonPublic | BindingFlags.Instance;
                // .NET 5+ renamed "text"→"_text" and "added"→"_added"; try new name first
                FieldInfo textField  = t.GetField("_text",  hidden) ?? t.GetField("text",  hidden);
                FieldInfo addedField = t.GetField("_added", hidden) ?? t.GetField("added", hidden);
                MethodInfo updateMethod = t.GetMethod("UpdateIcon", hidden);
                if (textField != null)
                    textField.SetValue(trayIcon, Text + Environment.NewLine + text);
                if (addedField != null && updateMethod != null && (bool)addedField.GetValue(trayIcon))
                    updateMethod.Invoke(trayIcon, new object[] { true });
            }
            catch { }
        }

        public void ShowTrayBalloonTip(List<string> itemsList, string title, ToolTipIcon icon, int timeout)
        {
            if (!balloonVisible &&
                cb_TrayBalloonNotify.Checked &&
                cb_AutomaticRefresh.Checked &&
                (WindowState == FormWindowState.Minimized || !Visible))
            {
                string text = string.Empty;

                foreach (string itemText in itemsList)
                {
                    if (text == string.Empty)
                    {
                        text = itemText.Trim();
                    }
                    else
                    {
                        text += Environment.NewLine;
                        text += itemText.Trim();
                    }
                }

                if (text.Length >= 255)
                {
                    text = "There are " + itemsList.Count + " " + title;
                }

                trayIcon.BalloonTipIcon = icon;
                trayIcon.BalloonTipTitle = Text + ", " + title;
                trayIcon.BalloonTipText = text;
                trayIcon.ShowBalloonTip(timeout);
            }
        }

        public void tray_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public void tray_Refresh_Click(object sender, EventArgs e)
        {
            if (!BW_GetStatus.IsBusy)
            {
                btn_RunCheck_Click(this, null);
            }
        }

        public void num_RequestTimeout_ValueChanged(object sender, EventArgs e)
        {
            lbl_RequestTimeoutSecondsText.Text = GetFormattedValueCountString((int)num_HTTPRequestTimeout.Value, "second");

            SaveConfiguration();
        }

        public void cb_AllowAutoRedirect_CheckedChanged(object sender, EventArgs e)
        {
            SaveConfiguration();
        }

        public void trayIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            balloonVisible = false;

            if (!Visible)
            {
                RestoreFromTray();
            }
        }

        public void btn_CheckAll_Click(object sender, EventArgs e)
        {
            SetCheckButtons(false);
            ListEndpoints(ListViewRefreshMethod.CheckAll);
            SetCheckButtons(true);

            RefreshTrayIcon();
            btn_RunCheck.Enabled = lv_Endpoints.Items.Count > 0;
        }

        public void btn_UncheckAll_Click(object sender, EventArgs e)
        {
            SetCheckButtons(false);
            ListEndpoints(ListViewRefreshMethod.UncheckAll);
            SetCheckButtons(true);

            RefreshTrayIcon();
            btn_RunCheck.Enabled = lv_Endpoints.Items.Count > 0;
        }

        public void btn_CheckAllAvailable_Click(object sender, EventArgs e)
        {
            SetCheckButtons(false);
            ListEndpoints(ListViewRefreshMethod.CheckAllPassed);
            SetCheckButtons(true);

            RefreshTrayIcon();
            btn_RunCheck.Enabled = lv_Endpoints.Items.Count > 0;
        }

        public void btn_CheckAllErrors_Click(object sender, EventArgs e)
        {
            SetCheckButtons(false);
            ListEndpoints(ListViewRefreshMethod.CheckAllFailed);
            SetCheckButtons(true);

            RefreshTrayIcon();
            btn_RunCheck.Enabled = lv_Endpoints.Items.Count > 0;
        }

        public void SetCheckButtons(bool enabled)
        {
            btn_CheckAll.Enabled = enabled;
            btn_UncheckAll.Enabled = enabled;
            btn_CheckAllAvailable.Enabled = enabled;
            btn_CheckAllErrors.Enabled = enabled;
            lbl_CheckAll.Enabled = enabled;
            lbl_UncheckAll.Enabled = enabled;
            lbl_CheckAllAvailable.Enabled = enabled;
            lbl_CheckAllErrors.Enabled = enabled;
            groupBox_EndpointSelection.Enabled = enabled;
            groupBox_Actions.Enabled = enabled;
        }

        public void cb_ValidateSSLCertificate_CheckedChanged(object sender, EventArgs e)
        {
            SaveConfiguration();
        }

        public void cb_TrayBalloonNotify_CheckedChanged(object sender, EventArgs e)
        {
            tray_Notifications_Enable.Visible = !cb_TrayBalloonNotify.Checked;
            tray_Notifications_Disable.Visible = cb_TrayBalloonNotify.Checked;

            SaveConfiguration();
        }

        public void cb_FTPPassiveMode_CheckedChanged(object sender, EventArgs e)
        {
            SaveConfiguration();
        }

        public void num_FTPRequestTimeout_ValueChanged(object sender, EventArgs e)
        {
            lbl_FTPRequestTimeoutSecondsText.Text = GetFormattedValueCountString((int)num_FTPRequestTimeout.Value, "second");

            SaveConfiguration();
        }

        public void cb_RefreshAutoSet_CheckedChanged(object sender, EventArgs e)
        {
            SaveConfiguration();
        }

        public void EndpointsStatusExport(
                                          string startDT,
                                          string endDT,
                                          int durationSeconds,
                                          int pingTimeout,
                                          int httpRequestTimeout,
                                          int ftpRequestTimeout,
                                          string httpAutoRedirection,
                                          string sslCertificateValidation,
                                          string threadsCount,
                                          string resolveNetworkShares,
                                          string resolvePageMetaInfo,
                                          string saveResponse,
                                          string pingHost,
                                          string dnsLookupOnHost
            )
        {
            // ERROR LIST
            List<string> errorsList = new List<string>();

            // GET ENABLED AND VISIBLE ENDPOINTS DEFINITIONS ITEMS LIST
            List<EndpointDefinition> exportList = new List<EndpointDefinition>();

            foreach (EndpointDefinition exportItem in endpointsList)
            {
                if (!endpointsList_Disabled.Contains(exportItem.Name) &&
                    exportItem.Name.ToLower().Contains(tb_ListFilter.Text.ToLower()))
                {
                    exportList.Add(exportItem);
                }
            }

            if (exportList.Count > 0)
            {
                if (cb_ExportEndpointsStatus_JSON.Checked ||
                    cb_ExportEndpointsStatus_XML.Checked)
                {
                    // SERIALIZE ENDPOINTS LIST TO JSON
                    string jsonString = JsonConvert.SerializeObject(exportList, Newtonsoft.Json.Formatting.Indented);

                    if (cb_ExportEndpointsStatus_JSON.Checked)
                    {
                        // JSON EXPORT
                        // ===========
                        // UNLOCK, SAVE AND LOCK JSON
                        SetProgressStatus(0, 0, "Generating Endpoint Status JSON Export ...", Color.BlueViolet);


                        try
                        {
                            CloseFileStream(definitionsStatusExport_JSON_FileStream);
                            File.WriteAllText(Path.Combine(statusExport_Directory, statusExport_JSONFile), jsonString, Encoding.UTF8);
                            definitionsStatusExport_JSON_FileStream = OpenFileStream(Path.Combine(statusExport_Directory, statusExport_JSONFile));
                        }
                        catch (Exception ex)
                        {
                            errorsList.Add(
                                "There was an error saving JSON Status Export:" +
                                Environment.NewLine +
                                ex.Message);
                        }
                    }

                    if (cb_ExportEndpointsStatus_XML.Checked)
                    {
                        // XML EXPORT
                        // ==========
                        // UNLOCK, SAVE AND LOCK XML
                        SetProgressStatus(0, 0, "Generating Endpoint Status XML Export ...", Color.BlueViolet);
                        XmlDocument xmlExport = JsonConvert.DeserializeXmlNode("{\"EndpointStatus\":" + jsonString.Replace("Encoding+", "Encoding_") + "}", "EndpointStatus");

                        try
                        {
                            CloseFileStream(definitionsStatusExport_XML_FileStream);
                            xmlExport.Save(Path.Combine(statusExport_Directory, statusExport_XMLFile));
                            definitionsStatusExport_XML_FileStream = OpenFileStream(Path.Combine(statusExport_Directory, statusExport_XMLFile));
                        }
                        catch (Exception ex)
                        {
                            errorsList.Add(
                                "There was an error saving XML Status Export:" +
                                Environment.NewLine +
                                ex.Message);
                        }
                    }
                }

                if (cb_ExportEndpointsStatus_XLSX.Checked)
                {
                    // XLSX EXPORT
                    // ===========
                    SetProgressStatus(0, 0, "Generating Endpoint Status XLSX Export ...", Color.BlueViolet);

                    try
                    {
                        // CREATE EXCEL WORKBOOK AND ADD SHEETS
                        XLWorkbook endpointsStatusExport_WorkBook = new XLWorkbook();
                        IXLWorksheet endpointsStatusExport_Summary_WorkSheet = endpointsStatusExport_WorkBook.Worksheets.Add("Summary");
                        IXLWorksheet endpointsStatusExport_HTTP_WorkSheet = endpointsStatusExport_WorkBook.Worksheets.Add("HTTP Endpoints");
                        IXLWorksheet endpointsStatusExport_FTP_WorkSheet = endpointsStatusExport_WorkBook.Worksheets.Add("FTP Endpoints");

                        // SET APP NAME AND VERSION AS AUTHOR
                        foreach (IXLWorksheet xlsxWorksheet in endpointsStatusExport_WorkBook.Worksheets)
                        {
                            xlsxWorksheet.Author = Text;
                        }

                        // WORKING LINE COUNTERS
                        int httpWorkSheetLineNumber = 1;
                        int ftpWorkSheetLineNumber = 1;

                        // ADD HEADER [HTTP ENDPOINTS WORKSHEET]
                        endpointsStatusExport_HTTP_WorkSheet.Cell("A" + httpWorkSheetLineNumber).SetValue("Endpoint Name");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("B" + httpWorkSheetLineNumber).SetValue("Protocol");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("C" + httpWorkSheetLineNumber).SetValue("Target Port");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("D" + httpWorkSheetLineNumber).SetValue("Endpoint Response URL");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("E" + httpWorkSheetLineNumber).SetValue("Endpoint IP Address(es)");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("F" + httpWorkSheetLineNumber).SetValue("Endpoint NIC MAC Address(es)");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("G" + httpWorkSheetLineNumber).SetValue("Endpoint DNS Name(s)");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("H" + httpWorkSheetLineNumber).SetValue("Response Time");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("I" + httpWorkSheetLineNumber).SetValue("Status Code");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("J" + httpWorkSheetLineNumber).SetValue("Status Message");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("K" + httpWorkSheetLineNumber).SetValue("Last Seen Online");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("L" + httpWorkSheetLineNumber).SetValue("Ping Roundtrip Time");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("M" + httpWorkSheetLineNumber).SetValue("UserName [Basic Auth]");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("N" + httpWorkSheetLineNumber).SetValue("Network Share(s)");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("O" + httpWorkSheetLineNumber).SetValue("HTTP Server ID");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("P" + httpWorkSheetLineNumber).SetValue("HTTP Auto Redirects");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("Q" + httpWorkSheetLineNumber).SetValue("HTTP Content Type");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("R" + httpWorkSheetLineNumber).SetValue("HTTP Content Length");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("S" + httpWorkSheetLineNumber).SetValue("HTTP Expires");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("T" + httpWorkSheetLineNumber).SetValue("HTTP ETag");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("U" + httpWorkSheetLineNumber).SetValue("HTTP Encoding");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("V" + httpWorkSheetLineNumber).SetValue("HTML Encoding");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("W" + httpWorkSheetLineNumber).SetValue("HTML Page Title");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("X" + httpWorkSheetLineNumber).SetValue("HTML Page Author");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("Y" + httpWorkSheetLineNumber).SetValue("HTML Page Description");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("Z" + httpWorkSheetLineNumber).SetValue("HTML Content Language");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("AA" + httpWorkSheetLineNumber).SetValue("HTML Theme Color");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("AB" + httpWorkSheetLineNumber).SetValue("HTML Page Links Count");
                        httpWorkSheetLineNumber++;

                        // ADD HEADER [FTP ENDPOINTS WORKSHEET]
                        endpointsStatusExport_FTP_WorkSheet.Cell("A" + ftpWorkSheetLineNumber).SetValue("Endpoint Name");
                        endpointsStatusExport_FTP_WorkSheet.Cell("B" + ftpWorkSheetLineNumber).SetValue("Protocol");
                        endpointsStatusExport_FTP_WorkSheet.Cell("C" + ftpWorkSheetLineNumber).SetValue("Target Port");
                        endpointsStatusExport_FTP_WorkSheet.Cell("D" + ftpWorkSheetLineNumber).SetValue("Endpoint Response URL");
                        endpointsStatusExport_FTP_WorkSheet.Cell("E" + ftpWorkSheetLineNumber).SetValue("Endpoint IP Address(es)");
                        endpointsStatusExport_FTP_WorkSheet.Cell("F" + ftpWorkSheetLineNumber).SetValue("Endpoint NIC MAC Address(es)");
                        endpointsStatusExport_FTP_WorkSheet.Cell("G" + ftpWorkSheetLineNumber).SetValue("Endpoint DNS Name(s)");
                        endpointsStatusExport_FTP_WorkSheet.Cell("H" + ftpWorkSheetLineNumber).SetValue("Response Time");
                        endpointsStatusExport_FTP_WorkSheet.Cell("I" + ftpWorkSheetLineNumber).SetValue("Status Code");
                        endpointsStatusExport_FTP_WorkSheet.Cell("J" + ftpWorkSheetLineNumber).SetValue("Status Message");
                        endpointsStatusExport_FTP_WorkSheet.Cell("K" + ftpWorkSheetLineNumber).SetValue("Last Seen Online");
                        endpointsStatusExport_FTP_WorkSheet.Cell("L" + ftpWorkSheetLineNumber).SetValue("Ping Roundtrip Time");
                        endpointsStatusExport_FTP_WorkSheet.Cell("M" + ftpWorkSheetLineNumber).SetValue("UserName");
                        endpointsStatusExport_FTP_WorkSheet.Cell("N" + ftpWorkSheetLineNumber).SetValue("Network Share(s)");
                        ftpWorkSheetLineNumber++;

                        // ADD ENDPOINTS ITEMS TO SHEETS 
                        foreach (EndpointDefinition endpointItem in exportList)
                        {
                            if (endpointItem.Protocol == Uri.UriSchemeHttp.ToUpper() ||
                                endpointItem.Protocol == Uri.UriSchemeHttps.ToUpper())
                            {
                                string connectionString = BuildUpConnectionString(endpointItem);

                                // ADD ENDPOINT ITEM TO HTTP SHEET
                                endpointsStatusExport_HTTP_WorkSheet.Cell("A" + httpWorkSheetLineNumber).SetValue(endpointItem.Name);
                                endpointsStatusExport_HTTP_WorkSheet.Cell("B" + httpWorkSheetLineNumber).SetValue(endpointItem.Protocol);
                                endpointsStatusExport_HTTP_WorkSheet.Cell("C" + httpWorkSheetLineNumber).SetValue(endpointItem.Port);
                                endpointsStatusExport_HTTP_WorkSheet.Cell("D" + httpWorkSheetLineNumber).SetValue(connectionString);

                                // CREATE RESPONSE ADDRESS HYPERLINK
                                endpointsStatusExport_HTTP_WorkSheet.Cell("D" + httpWorkSheetLineNumber).SetHyperlink(new XLHyperlink(connectionString));

                                endpointsStatusExport_HTTP_WorkSheet.Cell("E" + httpWorkSheetLineNumber).SetValue(string.Join(Environment.NewLine, endpointItem.IPAddress));
                                endpointsStatusExport_HTTP_WorkSheet.Cell("F" + httpWorkSheetLineNumber).SetValue(string.Join(Environment.NewLine, endpointItem.MACAddress));
                                endpointsStatusExport_HTTP_WorkSheet.Cell("G" + httpWorkSheetLineNumber).SetValue(string.Join(Environment.NewLine, endpointItem.DNSName));
                                endpointsStatusExport_HTTP_WorkSheet.Cell("H" + httpWorkSheetLineNumber).SetValue(endpointItem.ResponseTime);
                                endpointsStatusExport_HTTP_WorkSheet.Cell("I" + httpWorkSheetLineNumber).SetValue(endpointItem.ResponseCode);
                                endpointsStatusExport_HTTP_WorkSheet.Cell("J" + httpWorkSheetLineNumber).SetValue(endpointItem.ResponseMessage);
                                endpointsStatusExport_HTTP_WorkSheet.Cell("K" + httpWorkSheetLineNumber).SetValue(endpointItem.LastSeenOnline);
                                endpointsStatusExport_HTTP_WorkSheet.Cell("L" + httpWorkSheetLineNumber).SetValue(endpointItem.PingRoundtripTime);
                                endpointsStatusExport_HTTP_WorkSheet.Cell("M" + httpWorkSheetLineNumber).SetValue(endpointItem.LoginName);
                                endpointsStatusExport_HTTP_WorkSheet.Cell("N" + httpWorkSheetLineNumber).SetValue(string.Join(Environment.NewLine, endpointItem.NetworkShare));
                                endpointsStatusExport_HTTP_WorkSheet.Cell("O" + httpWorkSheetLineNumber).SetValue(endpointItem.ServerID);
                                endpointsStatusExport_HTTP_WorkSheet.Cell("P" + httpWorkSheetLineNumber).SetValue(endpointItem.HTTPautoRedirects);
                                endpointsStatusExport_HTTP_WorkSheet.Cell("Q" + httpWorkSheetLineNumber).SetValue(endpointItem.HTTPcontentType);
                                endpointsStatusExport_HTTP_WorkSheet.Cell("R" + httpWorkSheetLineNumber).SetValue(endpointItem.HTTPcontentLength);
                                endpointsStatusExport_HTTP_WorkSheet.Cell("S" + httpWorkSheetLineNumber).SetValue(endpointItem.HTTPexpires);
                                endpointsStatusExport_HTTP_WorkSheet.Cell("T" + httpWorkSheetLineNumber).SetValue(endpointItem.HTTPetag);
                                endpointsStatusExport_HTTP_WorkSheet.Cell("U" + httpWorkSheetLineNumber).SetValue(GetEncodingName(endpointItem.HTTPencoding));
                                endpointsStatusExport_HTTP_WorkSheet.Cell("V" + httpWorkSheetLineNumber).SetValue(GetEncodingName(endpointItem.HTMLencoding));
                                endpointsStatusExport_HTTP_WorkSheet.Cell("W" + httpWorkSheetLineNumber).SetValue(endpointItem.HTMLTitle);
                                endpointsStatusExport_HTTP_WorkSheet.Cell("X" + httpWorkSheetLineNumber).SetValue(endpointItem.HTMLAuthor);
                                endpointsStatusExport_HTTP_WorkSheet.Cell("Y" + httpWorkSheetLineNumber).SetValue(endpointItem.HTMLDescription);
                                endpointsStatusExport_HTTP_WorkSheet.Cell("Z" + httpWorkSheetLineNumber).SetValue(endpointItem.HTMLContentLanguage);
                                endpointsStatusExport_HTTP_WorkSheet.Cell("AA" + httpWorkSheetLineNumber).SetValue(GetKnownColorNameString(endpointItem.HTMLThemeColor));
                                endpointsStatusExport_HTTP_WorkSheet.Cell("AB" + httpWorkSheetLineNumber).SetValue(endpointItem.HTMLPageLinks.PropertyItem.Count().ToString());

                                // SET BACKGROUND COLOR BY STATUS CODE
                                endpointsStatusExport_HTTP_WorkSheet.Row(httpWorkSheetLineNumber)
                                    .CellsUsed().Style.Fill.BackgroundColor = XLColor.FromColor(GetColorByStatus(endpointItem.ResponseCode, endpointItem.PingRoundtripTime, endpointItem.ResponseMessage));

                                // INCREMENT ROW COUNTER
                                httpWorkSheetLineNumber++;
                            }
                            else if (endpointItem.Protocol == Uri.UriSchemeFtp.ToUpper())
                            {
                                string connectionString = BuildUpConnectionString(endpointItem);

                                // ADD ENDPOINT ITEM TO FTP SHEET
                                endpointsStatusExport_FTP_WorkSheet.Cell("A" + ftpWorkSheetLineNumber).SetValue(endpointItem.Name);
                                endpointsStatusExport_FTP_WorkSheet.Cell("B" + ftpWorkSheetLineNumber).SetValue(endpointItem.Protocol);
                                endpointsStatusExport_FTP_WorkSheet.Cell("C" + ftpWorkSheetLineNumber).SetValue(endpointItem.Port);
                                endpointsStatusExport_FTP_WorkSheet.Cell("D" + ftpWorkSheetLineNumber).SetValue(connectionString);

                                // CREATE RESPONSE ADDRESS HYPERLINK
                                endpointsStatusExport_FTP_WorkSheet.Cell("D" + ftpWorkSheetLineNumber).SetHyperlink(new XLHyperlink(connectionString));
                                endpointsStatusExport_FTP_WorkSheet.Cell("E" + ftpWorkSheetLineNumber).SetValue(string.Join(Environment.NewLine, endpointItem.IPAddress));
                                endpointsStatusExport_FTP_WorkSheet.Cell("F" + ftpWorkSheetLineNumber).SetValue(string.Join(Environment.NewLine, endpointItem.MACAddress));
                                endpointsStatusExport_FTP_WorkSheet.Cell("G" + ftpWorkSheetLineNumber).SetValue(string.Join(Environment.NewLine, endpointItem.DNSName));
                                endpointsStatusExport_FTP_WorkSheet.Cell("H" + ftpWorkSheetLineNumber).SetValue(endpointItem.ResponseTime);
                                endpointsStatusExport_FTP_WorkSheet.Cell("I" + ftpWorkSheetLineNumber).SetValue(endpointItem.ResponseCode);
                                endpointsStatusExport_FTP_WorkSheet.Cell("J" + ftpWorkSheetLineNumber).SetValue(endpointItem.ResponseMessage);
                                endpointsStatusExport_FTP_WorkSheet.Cell("K" + ftpWorkSheetLineNumber).SetValue(endpointItem.LastSeenOnline);
                                endpointsStatusExport_FTP_WorkSheet.Cell("L" + ftpWorkSheetLineNumber).SetValue(endpointItem.PingRoundtripTime);
                                endpointsStatusExport_FTP_WorkSheet.Cell("M" + ftpWorkSheetLineNumber).SetValue(endpointItem.LoginName);
                                endpointsStatusExport_FTP_WorkSheet.Cell("N" + ftpWorkSheetLineNumber).SetValue(string.Join(Environment.NewLine, endpointItem.NetworkShare));

                                // SET BACKGROUND COLOR BY STATUS CODE
                                endpointsStatusExport_FTP_WorkSheet.Row(ftpWorkSheetLineNumber)
                                    .CellsUsed().Style.Fill.BackgroundColor = XLColor.FromColor(GetColorByStatus(endpointItem.ResponseCode, endpointItem.PingRoundtripTime, endpointItem.ResponseMessage));

                                // INCREMENT ROW COUNTER
                                ftpWorkSheetLineNumber++;
                            }

                            Application.DoEvents();
                        }

                        // ADD SUMMARY WORKSHEET
                        endpointsStatusExport_Summary_WorkSheet.Cell("A1").SetValue("Endpoint Checker Application");
                        endpointsStatusExport_Summary_WorkSheet.Cell("B1").SetValue("Version " + app_VersionString + " (built " + app_Built_DateTime + ")");
                        endpointsStatusExport_Summary_WorkSheet.Cell("A2").SetValue("Operating System");
                        endpointsStatusExport_Summary_WorkSheet.Cell("B2").SetValue(os_VersionString);
                        endpointsStatusExport_Summary_WorkSheet.Cell("A3").SetValue("Target Framework Version");
                        endpointsStatusExport_Summary_WorkSheet.Cell("B3").SetValue(dotNetFramework_TargetVersion.FrameworkDisplayName);
                        endpointsStatusExport_Summary_WorkSheet.Cell("A4").SetValue("System Memory (RAM)");
                        endpointsStatusExport_Summary_WorkSheet.Cell("B4").SetValue(systemMemorySize);
                        endpointsStatusExport_Summary_WorkSheet.Cell("A5").SetValue("User Name");
                        endpointsStatusExport_Summary_WorkSheet.Cell("B5").SetValue(Environment.UserName);
                        endpointsStatusExport_Summary_WorkSheet.Cell("A6").SetValue("Domain");
                        endpointsStatusExport_Summary_WorkSheet.Cell("B6").SetValue(Environment.UserDomainName);
                        endpointsStatusExport_Summary_WorkSheet.Cell("A7").SetValue("Computer Name");
                        endpointsStatusExport_Summary_WorkSheet.Cell("B7").SetValue(Environment.MachineName);

                        endpointsStatusExport_Summary_WorkSheet.Cell("D1").SetValue("Check Started");
                        endpointsStatusExport_Summary_WorkSheet.Cell("E1").SetValue(startDT);
                        endpointsStatusExport_Summary_WorkSheet.Cell("D2").SetValue("Check Ended");
                        endpointsStatusExport_Summary_WorkSheet.Cell("E2").SetValue(endDT);
                        endpointsStatusExport_Summary_WorkSheet.Cell("D3").SetValue("Check Duration");
                        endpointsStatusExport_Summary_WorkSheet.Cell("E3").SetValue(durationSeconds + " " + GetFormattedValueCountString(durationSeconds, "second"));
                        endpointsStatusExport_Summary_WorkSheet.Cell("D4").SetValue("HTTP Endpoints Count");
                        endpointsStatusExport_Summary_WorkSheet.Cell("E4").SetValue((httpWorkSheetLineNumber - 2).ToString());
                        endpointsStatusExport_Summary_WorkSheet.Cell("D5").SetValue("FTP Endpoints Count");
                        endpointsStatusExport_Summary_WorkSheet.Cell("E5").SetValue((ftpWorkSheetLineNumber - 2).ToString());
                        endpointsStatusExport_Summary_WorkSheet.Cell("D6").SetValue("Parallel Threads Count");
                        endpointsStatusExport_Summary_WorkSheet.Cell("E6").SetValue(threadsCount);
                        endpointsStatusExport_Summary_WorkSheet.Cell("D7").SetValue("Ping Timeout");
                        endpointsStatusExport_Summary_WorkSheet.Cell("E7").SetValue(pingTimeout + " " + GetFormattedValueCountString(pingTimeout, "second"));
                        endpointsStatusExport_Summary_WorkSheet.Cell("D8").SetValue("HTTP Request Timeout");
                        endpointsStatusExport_Summary_WorkSheet.Cell("E8").SetValue(httpRequestTimeout + " " + GetFormattedValueCountString(httpRequestTimeout, "second"));
                        endpointsStatusExport_Summary_WorkSheet.Cell("D9").SetValue("FTP Request Timeout");
                        endpointsStatusExport_Summary_WorkSheet.Cell("E9").SetValue(ftpRequestTimeout + " " + GetFormattedValueCountString(ftpRequestTimeout, "second"));
                        endpointsStatusExport_Summary_WorkSheet.Cell("D10").SetValue("Server Certificate Validation [HTTPS]");
                        endpointsStatusExport_Summary_WorkSheet.Cell("E10").SetValue(sslCertificateValidation);
                        endpointsStatusExport_Summary_WorkSheet.Cell("D11").SetValue("Auto Redirection [HTTP]");
                        endpointsStatusExport_Summary_WorkSheet.Cell("E11").SetValue(httpAutoRedirection);
                        endpointsStatusExport_Summary_WorkSheet.Cell("D12").SetValue("Resolve Network Shares");
                        endpointsStatusExport_Summary_WorkSheet.Cell("E12").SetValue(resolveNetworkShares);
                        endpointsStatusExport_Summary_WorkSheet.Cell("D13").SetValue("Resolve Page Meta Info [HTTP/HTML]");
                        endpointsStatusExport_Summary_WorkSheet.Cell("E13").SetValue(resolvePageMetaInfo);
                        endpointsStatusExport_Summary_WorkSheet.Cell("D14").SetValue("Save Response [HTTP]");
                        endpointsStatusExport_Summary_WorkSheet.Cell("E14").SetValue(saveResponse);
                        endpointsStatusExport_Summary_WorkSheet.Cell("D15").SetValue("Ping Host");
                        endpointsStatusExport_Summary_WorkSheet.Cell("E15").SetValue(pingHost);
                        endpointsStatusExport_Summary_WorkSheet.Cell("D16").SetValue("DNS / MAC Lookup on Host");
                        endpointsStatusExport_Summary_WorkSheet.Cell("E16").SetValue(dnsLookupOnHost);

                        // SETTINGS FOR HTTP ENDPOINTS WORKSHEET
                        endpointsStatusExport_HTTP_WorkSheet.Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                            .Alignment.SetVertical(XLAlignmentVerticalValues.Top);
                        endpointsStatusExport_HTTP_WorkSheet.SheetView.FreezeRows(1);
                        endpointsStatusExport_HTTP_WorkSheet.SheetView.FreezeColumns(1);
                        endpointsStatusExport_HTTP_WorkSheet.RangeUsed().SetAutoFilter();
                        endpointsStatusExport_HTTP_WorkSheet.Rows().AdjustToContents();
                        endpointsStatusExport_HTTP_WorkSheet.Columns().AdjustToContents(10, (double)70);
                        endpointsStatusExport_HTTP_WorkSheet.CellsUsed().Style.NumberFormat.Format = "@";
                        endpointsStatusExport_HTTP_WorkSheet.Row(1).CellsUsed().Style.Fill.BackgroundColor = XLColor.CoolGrey;
                        endpointsStatusExport_HTTP_WorkSheet.CellsUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        // SETTINGS FOR FTP ENDPOINTS WORKSHEET
                        endpointsStatusExport_FTP_WorkSheet.Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                            .Alignment.SetVertical(XLAlignmentVerticalValues.Top);
                        endpointsStatusExport_FTP_WorkSheet.SheetView.FreezeRows(1);
                        endpointsStatusExport_FTP_WorkSheet.SheetView.FreezeColumns(1);
                        endpointsStatusExport_FTP_WorkSheet.RangeUsed().SetAutoFilter();
                        endpointsStatusExport_FTP_WorkSheet.Rows().AdjustToContents();
                        endpointsStatusExport_FTP_WorkSheet.Columns().AdjustToContents(10, (double)70);
                        endpointsStatusExport_FTP_WorkSheet.CellsUsed().Style.NumberFormat.Format = "@";
                        endpointsStatusExport_FTP_WorkSheet.Row(1).CellsUsed().Style.Fill.BackgroundColor = XLColor.CoolGrey;
                        endpointsStatusExport_FTP_WorkSheet.CellsUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        // SETTINGS FOR SUMMARY WORKSHEET
                        endpointsStatusExport_Summary_WorkSheet.SheetView.FreezeColumns(1);
                        endpointsStatusExport_Summary_WorkSheet.Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                            .Alignment.SetVertical(XLAlignmentVerticalValues.Top);
                        endpointsStatusExport_Summary_WorkSheet.Rows().AdjustToContents();
                        endpointsStatusExport_Summary_WorkSheet.Columns().AdjustToContents();
                        endpointsStatusExport_Summary_WorkSheet.Column(1).CellsUsed().Style.Fill.BackgroundColor = XLColor.CoolGrey;
                        endpointsStatusExport_Summary_WorkSheet.Column(2).CellsUsed().Style.Fill.BackgroundColor = XLColor.LightBlue;
                        endpointsStatusExport_Summary_WorkSheet.Column(4).CellsUsed().Style.Fill.BackgroundColor = XLColor.CoolGrey;
                        endpointsStatusExport_Summary_WorkSheet.Column(5).CellsUsed().Style.Fill.BackgroundColor = XLColor.LightBlue;
                        endpointsStatusExport_Summary_WorkSheet.CellsUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        // REMOVE UNUSED COLUMNS IN WORKSHEETS
                        foreach (IXLColumn column in endpointsStatusExport_HTTP_WorkSheet.Columns())
                        {
                            if (column.CellsUsed().Where(c => c.Value.ToString() != status_NotAvailable).Count() == 1)
                            {
                                column.Hide();
                            }
                        }

                        foreach (IXLColumn column in endpointsStatusExport_FTP_WorkSheet.Columns())
                        {
                            if (column.CellsUsed().Where(c => c.Value.ToString() != status_NotAvailable).Count() == 1)
                            {
                                column.Hide();
                            }
                        }

                        // REMOVE EMPTY WORKSHEET (IF ANY)
                        if (endpointsStatusExport_HTTP_WorkSheet.RowsUsed().Count() < 2)
                        {
                            endpointsStatusExport_HTTP_WorkSheet.Delete();
                        }
                        else if (endpointsStatusExport_FTP_WorkSheet.RowsUsed().Count() < 2)
                        {
                            endpointsStatusExport_FTP_WorkSheet.Delete();
                        }

                        try
                        {
                            // SAVE XLSX
                            Application.DoEvents();
                            CloseFileStream(definitionsStatusExport_XLSX_FileStream);
                            endpointsStatusExport_WorkBook.SaveAs(Path.Combine(statusExport_Directory, statusExport_XLSFile), new SaveOptions { ValidatePackage = true });
                            definitionsStatusExport_XLSX_FileStream = OpenFileStream(Path.Combine(statusExport_Directory, statusExport_XLSFile));
                            Application.DoEvents();
                        }
                        catch (Exception ex)
                        {
                            errorsList.Add(
                                "There was an error saving XLSX Status Export:" +
                                Environment.NewLine +
                                ex.Message);
                        }

                        if (cb_ExportEndpointsStatus_HTML.Checked)
                        {
                            // HTML EXPORT
                            // ===========
                            SetProgressStatus(0, 0, "Generating Endpoint Status HTML Export ...", Color.BlueViolet);

                            // SAVE AND LOCK HTML(S)
                            Workbook xlsxWorkBook = new Workbook();
                            xlsxWorkBook.LoadFromFile(Path.Combine(statusExport_Directory, statusExport_XLSFile));

                            Worksheet summaryWorkSheet = xlsxWorkBook.Worksheets["Summary"];

                            // SET WHITE BACKGROUND FOR UNUSED CELLS [CREATE SEPARATE CLASS FOR IT]
                            summaryWorkSheet["A09"].Style.Color = Color.White;

                            if (cb_ExportEndpointsStatus_JSON.Checked)
                            {
                                // ADD 'JSON' HYPERLINK PLACEHOLDER TO 'SUMMARY' PAGE           
                                RichText jsonPageHyperlink = summaryWorkSheet["A10"].RichText;
                                jsonPageHyperlink.Text = "xHTML_JSONx";
                                summaryWorkSheet["A10"].Style.Color = Color.DarkOrange;
                                summaryWorkSheet["A10"].Style.Font.IsBold = true;
                                summaryWorkSheet["A10"].Style.HorizontalAlignment = HorizontalAlignType.Center;
                            }

                            if (cb_ExportEndpointsStatus_XML.Checked)
                            {
                                // ADD 'XML' HYPERLINK PLACEHOLDER TO 'SUMMARY' PAGE           
                                RichText xmlPageHyperlink = summaryWorkSheet["A11"].RichText;
                                xmlPageHyperlink.Text = "xHTML_XMLx";
                                summaryWorkSheet["A11"].Style.Color = Color.Red;
                                summaryWorkSheet["A11"].Style.Font.IsBold = true;
                                summaryWorkSheet["A11"].Style.HorizontalAlignment = HorizontalAlignType.Center;
                            }

                            // ADD 'XLSX' HYPERLINK PLACEHOLDER TO 'SUMMARY' PAGE           
                            RichText xlsxPageHyperlink = summaryWorkSheet["A12"].RichText;
                            xlsxPageHyperlink.Text = "xHTML_XLSXx";
                            summaryWorkSheet["A12"].Style.Color = Color.DarkViolet;
                            summaryWorkSheet["A12"].Style.Font.IsBold = true;
                            summaryWorkSheet["A12"].Style.HorizontalAlignment = HorizontalAlignType.Center;

                            if (xlsxWorkBook.Worksheets.Where(w => w.Name == "HTTP Endpoints").Count() == 1)
                            {
                                try
                                {
                                    CloseFileStream(definitionsStatusExport_HTML_Info_FileStream);
                                    CloseFileStream(definitionsStatusExport_HTML_HTTP_FileStream);
                                    CloseFileStream(definitionsStatusExport_HTML_FTP_FileStream);

                                    // SAVE HTML [HTTP PAGE]
                                    Application.DoEvents();
                                    xlsxWorkBook.Worksheets["HTTP Endpoints"].SaveToHtml(Path.Combine(statusExport_Directory, statusExport_HTMLFile_HTTPPage));
                                    Application.DoEvents();

                                    // ADD 'HTTP' HTML FIXED REFRESH BUTTON
                                    string httpHTMLString = File.ReadAllText(Path.Combine(statusExport_Directory, statusExport_HTMLFile_HTTPPage));
                                    httpHTMLString = CreateEndpointURLHyperLink(httpHTMLString);
                                    httpHTMLString = AddRefreshCSSButtonToHTMLString(httpHTMLString);

                                    // SAVE HTML STRING [HTTP PAGE]
                                    Application.DoEvents();
                                    File.WriteAllText(Path.Combine(statusExport_Directory, statusExport_HTMLFile_HTTPPage), httpHTMLString, Encoding.UTF8);
                                    Application.DoEvents();

                                    // LOCK 'HTTP' HTML
                                    definitionsStatusExport_HTML_HTTP_FileStream = OpenFileStream(Path.Combine(statusExport_Directory, statusExport_HTMLFile_HTTPPage));

                                    // ADD 'HTTP' HYPERLINK PLACEHOLDER TO 'SUMMARY' PAGE           
                                    RichText httpPageHyperlink = summaryWorkSheet["A14"].RichText;
                                    httpPageHyperlink.Text = "xHTML_HTTPx";
                                    summaryWorkSheet["A14"].Style.Color = Color.Green;
                                    summaryWorkSheet["A14"].Style.Font.IsBold = true;
                                    summaryWorkSheet["A14"].Style.HorizontalAlignment = HorizontalAlignType.Center;
                                }
                                catch (Exception ex)
                                {
                                    errorsList.Add(
                                        "There was an error saving HTML Status Export (HTTP Page):" +
                                        Environment.NewLine +
                                        ex.Message);
                                }
                            }

                            if (xlsxWorkBook.Worksheets.Where(w => w.Name == "FTP Endpoints").Count() == 1)
                            {
                                try
                                {
                                    // SAVE HTML [FTP PAGE]
                                    Application.DoEvents();
                                    xlsxWorkBook.Worksheets["FTP Endpoints"].SaveToHtml(Path.Combine(statusExport_Directory, statusExport_HTMLFile_FTPPage));
                                    Application.DoEvents();

                                    // ADD 'FTP' HTML FIXED REFRESH BUTTON
                                    string ftpHTMLString = File.ReadAllText(Path.Combine(statusExport_Directory, statusExport_HTMLFile_FTPPage));
                                    ftpHTMLString = CreateEndpointURLHyperLink(ftpHTMLString);
                                    ftpHTMLString = AddRefreshCSSButtonToHTMLString(ftpHTMLString);

                                    // SAVE HTML STRING [FTP PAGE]
                                    Application.DoEvents();
                                    File.WriteAllText(Path.Combine(statusExport_Directory, statusExport_HTMLFile_FTPPage), ftpHTMLString, Encoding.UTF8);
                                    Application.DoEvents();

                                    // LOCK 'FTP' HTML
                                    definitionsStatusExport_HTML_FTP_FileStream = OpenFileStream(Path.Combine(statusExport_Directory, statusExport_HTMLFile_FTPPage));

                                    // ADD 'FTP' HYPERLINK PLACEHOLDER TO 'SUMMARY' PAGE           
                                    RichText ftpPageHyperlink = summaryWorkSheet["A15"].RichText;
                                    ftpPageHyperlink.Text = "xHTML_FTPx";
                                    summaryWorkSheet["A15"].Style.Color = Color.Blue;
                                    summaryWorkSheet["A15"].Style.Font.IsBold = true;
                                    summaryWorkSheet["A15"].Style.HorizontalAlignment = HorizontalAlignType.Center;
                                }
                                catch (Exception ex)
                                {
                                    errorsList.Add(
                                        "There was an error saving HTML Status Export (FTP Page):" +
                                        Environment.NewLine +
                                        ex.Message);
                                }
                            }

                            try
                            {
                                // SAVE HTML [SUMMARY PAGE]
                                Application.DoEvents();
                                summaryWorkSheet.SaveToHtml(Path.Combine(statusExport_Directory, statusExport_HTMLFile_InfoPage));
                                Application.DoEvents();

                                // REPLACE HYPERLINKS ON 'SUMMARY' PAGE
                                string summaryHTMLstring = File.ReadAllText(Path.Combine(statusExport_Directory, statusExport_HTMLFile_InfoPage));
                                summaryHTMLstring = summaryHTMLstring
                                    .Replace("xHTML_XLSXx", "<a href=\"" + statusExport_XLSFile + "\" style=\"color:white;\">Endpoints Status XLSX Export</a>")
                                    .Replace("xHTML_JSONx", "<a href=\"" + statusExport_JSONFile + "\" style=\"color:white;\">Endpoints Status JSON Export</a>")
                                    .Replace("xHTML_XMLx", "<a href=\"" + statusExport_XMLFile + "\" style=\"color:white;\">Endpoints Status XML Export</a>")
                                    .Replace("xHTML_HTTPx", "<a href=\"" + statusExport_HTMLFile_HTTPPage + "\" style=\"color:white;\">HTTP Endpoints Status List</a>")
                                    .Replace("xHTML_FTPx", "<a href=\"" + statusExport_HTMLFile_FTPPage + "\" style=\"color:white;\">FTP Endpoints Status List</a>");

                                // ADD HTML AUTO REFRESH
                                summaryHTMLstring = AddAutoRefreshToHTMLString(summaryHTMLstring, 30);

                                // SAVE HTML STRING [SUMMARY PAGE]
                                Application.DoEvents();
                                File.WriteAllText(Path.Combine(statusExport_Directory, statusExport_HTMLFile_InfoPage), summaryHTMLstring, Encoding.UTF8);
                                Application.DoEvents();

                                // LOCK 'SUMMARY' HTML
                                definitionsStatusExport_HTML_Info_FileStream = OpenFileStream(Path.Combine(statusExport_Directory, statusExport_HTMLFile_InfoPage));
                            }
                            catch (Exception ex)
                            {
                                errorsList.Add(
                                    "There was an error saving HTML Status Export (Summary Page):" +
                                    Environment.NewLine +
                                    ex.Message);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        ThreadSafeInvoke(() =>
                        {
                            ExceptionNotify(this, exception, string.Empty, true);
                        });
                    }
                }
            }

            if (errorsList.Count > 0)
            {
                MessageBox.Show(
                    string.Join(
                        Environment.NewLine +
                        Environment.NewLine +
                        Environment.NewLine,
                        errorsList),
                    "Endpoints Status Export",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        public string AddAutoRefreshToHTMLString(string inputHTML, int refreshIntervalSeconds)
        {
            return inputHTML.Replace("<head>", "<head>" + Environment.NewLine + "<meta http-equiv=\"refresh\" content=\"" + refreshIntervalSeconds + "\">");
        }

        public string CreateEndpointURLHyperLink(string inputHTML)
        {
            // REMOVE ESCAPE CHARACTERS
            string inputXMLString = inputHTML
                .Replace("&nbsp;", " ")
                .Replace("&", "&amp;");

            // LOAD AS XML
            XmlDocument inputHTMLDoc = new XmlDocument();
            inputHTMLDoc.LoadXml(inputXMLString);

            XmlNodeList trNodesList = inputHTMLDoc.GetElementsByTagName("tr");

            int trNodeIndex = 0;
            foreach (XmlNode trNode in trNodesList)
            {
                if (trNodeIndex > 0)
                {
                    // ADD 'ONCLICK' HANDLER
                    XmlAttribute attr = inputHTMLDoc.CreateAttribute("onclick");
                    attr.Value = "location.href = '" +
                                 trNode.ChildNodes[3].ChildNodes[0].InnerXml +
                                 "'";

                    trNode.ChildNodes[3].ChildNodes[0].Attributes.Append(attr);

                    using (StringWriter stringWriter = new StringWriter())
                    using (XmlWriter xmlTextWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true, NewLineOnAttributes = false, OmitXmlDeclaration = true }))
                    {
                        inputHTMLDoc.WriteTo(xmlTextWriter);
                        xmlTextWriter.Flush();
                        inputHTML = stringWriter.GetStringBuilder().ToString();
                    }
                }

                trNodeIndex++;
            }

            // SET 'HAND' CURSOR TO CLASSES DEFINING HYPERLINK [UNDERLINED STYLE]
            foreach (string htmlLine in inputXMLString.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (htmlLine.StartsWith(".X") &&
                    htmlLine.Contains("text-decoration:underline"))
                {
                    // SET 'HAND' CURSOR TO AFFECTED NODES
                    inputHTML = inputHTML.Replace(
                        htmlLine.Split('{')[1],
                        "cursor:pointer;" + htmlLine.Split('{')[1]);
                }
            }

            return inputHTML;
        }

        public string AddRefreshCSSButtonToHTMLString(string inputHTML)
        {
            return inputHTML.Replace(
                                    @"<html xmlns=""http://www.w3.org/1999/xhtml"">
  <head>
    <style type=""text/css"">table",
                                    @"<html xmlns=""http://www.w3.org/1999/xhtml"">
<INPUT TYPE=""button"" onClick=""window.location.reload()"" VALUE=""Refresh"" ID=""refreshBTN"">
  <head>
    <style type=""text/css"">
    body {
            background - color: #CCC;
            margin: 32px 0px 0px 0px;
                                }
                                INPUT#refreshBTN {
                                position: fixed;
                                top: 0px;
                                left: 0px;
                                width: 100%;
                                color: #7CFC00;
                                background: #333;
                                padding: 5px;
                                cursor:pointer;
                                }
                            table");
        }

        public Color GetColorByStatus(string statusCode, string pingTime, string statusMessage)
        {
            if (statusCode == status_NotAvailable)
            {
                if (statusMessage == GetEnumDescriptionString(EndpointStatus.TERMINATED))
                    return Color.FromArgb(10, 40, 84);    // dark steel blue
                if (statusMessage == GetEnumDescriptionString(EndpointStatus.NOTCHECKED) ||
                    statusMessage == GetEnumDescriptionString(EndpointStatus.DISABLED))
                    return Color.FromArgb(38, 42, 62);    // dark slate (disabled)
            }

            if (validationMethod == ValidationMethod.Protocol)
            {
                if (statusMessage == GetEnumDescriptionString(EndpointStatus.PINGCHECK))
                    return Color.FromArgb(34, 38, 58);    // near-surface (not checked)
                if (statusCode == status_Error)
                    return Color.FromArgb(90, 14, 14);    // dark crimson
                if (statusCode[0].ToString() == "2")
                    return Color.FromArgb(20, 61, 30);    // dark forest green
                if (statusCode[0].ToString() == "4")
                    return Color.FromArgb(71, 16, 58);    // dark magenta (4xx / CF-blocked)
                return Color.FromArgb(74, 48, 8);         // dark amber (3xx / other)
            }
            else
            {
                if (pingTime == status_NotAvailable)
                    return Color.FromArgb(90, 14, 14);    // dark crimson
                return Color.FromArgb(20, 61, 30);        // dark forest green
            }
        }

        public Color GetForeColorByStatus(string statusCode, string pingTime, string statusMessage)
        {
            Color muted  = Color.FromArgb( 90, 105, 140);  // dimmed — for disabled/not-checked
            Color bright = Color.FromArgb(220, 235, 255);  // off-white — default readable text

            if (statusCode == status_NotAvailable)
            {
                if (statusMessage == GetEnumDescriptionString(EndpointStatus.TERMINATED))
                    return Color.FromArgb(102, 204, 255);  // bright cyan
                if (statusMessage == GetEnumDescriptionString(EndpointStatus.NOTCHECKED) ||
                    statusMessage == GetEnumDescriptionString(EndpointStatus.DISABLED))
                    return Color.FromArgb(218, 228, 245);
                return muted;
            }

            if (validationMethod == ValidationMethod.Protocol)
            {
                if (statusMessage == GetEnumDescriptionString(EndpointStatus.PINGCHECK))
                    return muted;
                if (statusCode == status_Error)
                    return Color.FromArgb(255, 130, 130);  // bright red
                if (statusCode[0].ToString() == "2")
                    return Color.FromArgb(130, 255, 160);  // bright green
                if (statusCode[0].ToString() == "4")
                    return Color.FromArgb(255, 160, 230);  // bright pink
                return Color.FromArgb(255, 210, 100);      // bright amber
            }
            else
            {
                if (pingTime == status_NotAvailable)
                    return Color.FromArgb(255, 130, 130);  // bright red
                return Color.FromArgb(130, 255, 160);      // bright green
            }
        }

        public void cb_AutomaticRefresh_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_AutomaticRefresh.Checked)
            {
                TIMER_ContinuousRefresh.Stop();
                TIMER_AutomaticRefresh.Start();

                if (endpointsList.Count > 0 && !onClose)
                {
                    TIMER_AutomaticRefresh_Tick(this, null);
                }
            }
            else
            {
                TIMER_AutomaticRefresh.Stop();
            }

            SaveConfiguration();
            RefreshTrayIcon();
        }

        public void cb_ContinuousRefresh_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_ContinuousRefresh.Checked)
            {
                TIMER_AutomaticRefresh.Stop();
                TIMER_ContinuousRefresh.Start();

                if (endpointsList.Count > 0 && !onClose)
                {
                    TIMER_ContinuousRefresh_Tick(this, null);
                }
            }
            else
            {
                TIMER_ContinuousRefresh.Stop();
            }

            SaveConfiguration();
            RefreshTrayIcon();
        }

        public void btn_Terminate_Click(object sender, EventArgs e)
        {
            if (e != null)
            {
                // DISABLE 'AUTO REFRESH' OPTIONS
                cb_AutomaticRefresh.Checked = false;
                cb_ContinuousRefresh.Checked = false;
            }
            else
            {
                // DISABLE TRAY ICON CONTEXT MENU
                trayIcon.ContextMenuStrip = null;

                // HIDE MAIN FORM TO TRAY WHILE TERMINATES THE PROCESS AND CLOSE
                Hide();
            }

            // DISABLE ITSELF
            btn_Terminate.Enabled = false;
            lbl_Terminate.Enabled = false;

            // TERMINATE WORKER
            BW_GetStatus.CancelAsync();

            SetTrayControls(true);
            SetProgressStatus(0, 0);
        }

        public void num_ParallelThreadsCount_ValueChanged(object sender, EventArgs e)
        {
            SaveConfiguration();
        }

        public void CheckerMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            onClose = true;
            premiumUiPulseTimer.Stop();

            // DISABLE FORM CLOSE WHILE ASYNC WORKER IS IN PROGRESS
            if (BW_GetStatus.IsBusy ||
                dialog_EndpointDetails != null ||
                dialog_SpeedTest != null)
            {
                // CANCEL ACTUAL CLOSE EVENT
                e.Cancel = true;

                // TERMINATE PROCESS AND THEN CLOSE APPLICATION
                btn_Terminate_Click(this, null);
            }
            else
            {
                // SAVE SETTINGS
                SaveWindowSizeAndPosition();
                SaveListViewColumnsWidthAndOrder();
                SaveDisabledItemsListAndFilter();
                SaveConfiguration();

                // IF UPDATE AVAILABLE, RUN UPDATER
                if (app_AutoUpdateNow)
                {
                    ExecuteUpdater();
                }
            }
        }

        public void trayIcon_BalloonTipClosed(object sender, EventArgs e)
        {
            balloonVisible = false;
        }

        public void trayIcon_BalloonTipShown(object sender, EventArgs e)
        {
            balloonVisible = true;
        }

        public void cb_ResolveNetworkShares_CheckedChanged(object sender, EventArgs e)
        {
            if (lv_Endpoints.Visible &&
                cb_ResolveNetworkShares.Checked)
            {
                MessageBox.Show("This option may cause check operation take a long time", "Resolve Network Shares", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            SaveConfiguration();
        }

        [DllImport("Netapi32.dll", CharSet = CharSet.Unicode)]
        public static extern int NetShareEnum(
             StringBuilder ServerName,
             int level,
             ref IntPtr bufPtr,
             uint prefmaxlen,
             ref int entriesread,
             ref int totalentries,
             ref int resume_handle
             );

        [DllImport("Netapi32.dll", SetLastError = true)]
        private static extern int NetApiBufferFree(IntPtr Buffer);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SHARE_INFO_1
        {
            public string shi1_netname;
            public uint shi1_type;
            public string shi1_remark;
        }

        public List<string> GetNetShares(string hostName)
        {
            List<string> netSharesList = new List<string>();

            int entriesread = 0;
            int totalentries = 0;
            int resume_handle = 0;
            int nStructSize = Marshal.SizeOf(typeof(SHARE_INFO_1));
            IntPtr bufPtr = IntPtr.Zero;
            StringBuilder host = new StringBuilder(hostName);
            int ret = NetShareEnum(host, 1, ref bufPtr, 0xFFFFFFFF, ref entriesread, ref totalentries, ref resume_handle);
            if (ret == 0)
            {
                IntPtr currentPtr = bufPtr;
                for (int i = 0; i < entriesread; i++)
                {
                    SHARE_INFO_1 shi1 = (SHARE_INFO_1)Marshal.PtrToStructure(currentPtr, typeof(SHARE_INFO_1));
                    string netShareitem = "[" + NetShareType(shi1.shi1_type) + "] " + shi1.shi1_netname;
                    if (!string.IsNullOrEmpty(shi1.shi1_remark))
                    {
                        netShareitem += " (" + shi1.shi1_remark + ")";
                    }

                    netSharesList.Add(netShareitem);
                    currentPtr = new IntPtr(currentPtr.ToInt32() + nStructSize);
                }
                NetApiBufferFree(bufPtr);
            }
            else
            {
                netSharesList.Add(status_NotAvailable + " (" + NetShareError(ret) + ")");
            }

            return netSharesList;
        }

        public string NetShareError(int code)
        {
            Dictionary<int, string> codeList = new Dictionary<int, string>
            {
                { 0, "OK" },
                { 5, "The user has insufficient privilege for this operation" },
                { 8, "Not enough memory" },
                { 65, "Network access is denied" },
                { 87, "Invalid parameter specified" },
                { 53, "The network path was not found" },
                { 123, "Invalid name" },
                { 124, "Invalid level parameter" },
                { 234, "More data available, buffer too small" },
                { 2102, "Device driver not installed" },
                { 2106, "This operation can be performed only on a server" },
                { 2114, "Server service not installed" },
                { 2123, "Buffer too small for fixed-length data" },
                { 2127, "Error encountered while executing function remotely" },
                { 2138, "The Workstation service is not started" },
                { 2141, "The server is not configured for this transaction (IPC$ is not shared)" },
                { 2351, "Invalid computername specified" }
            };

            return codeList.ContainsKey(code) ? codeList[code] : "Result Code: " + code.ToString();
        }

        public string NetShareType(uint code)
        {
            Dictionary<uint, string> codeList = new Dictionary<uint, string>
            {
                { 0, "Folder" },
                { 1, "Printer" },
                { 2, "Device" },
                { 3, "IPC" },
                { 2147483648, "Admin/Folder" },
                { 2147483649, "Admin/Printer" },
                { 2147483650, "Admin/Device" },
                { 2147483651, "Admin/IPC" }
            };

            return codeList.ContainsKey(code) ? codeList[code] : "Type Code: " + code.ToString();
        }

        private ToolTip endpointToolTip;
        private Point endpointToolTipLastPosition = new Point(-1, -1);
        public void lv_Endpoints_MouseMove(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo hitTestItem = lv_Endpoints.HitTest(e.X, e.Y);

            if (endpointToolTip == null)
            {
                endpointToolTip = new ToolTip();
            }

            if (endpointToolTipLastPosition != e.Location)
            {
                if (hitTestItem.Item != null &&
                    hitTestItem.SubItem != null &&
                    hitTestItem.SubItem.Name == "Endpoint Name")
                {
                    // TEXT
                    string infoText = Environment.NewLine;
                    infoText += "Left mouse button doubleclick for check / uncheck EndPoint";
                    infoText += Environment.NewLine;
                    infoText += Environment.NewLine;
                    infoText += "Right mouse button click for more EndPoint options context menu";
                    infoText += Environment.NewLine;
                    infoText += Environment.NewLine;
                    infoText += "Select single or more EndPoints and press CTRL+C keys to copy details to clipboard";
                    endpointToolTip.ToolTipTitle = hitTestItem.Item.Text;
                    endpointToolTip.Show(infoText, hitTestItem.Item.ListView, e.X + 20, e.Y + 25, 20000);
                }
                else
                {
                    Lv_Endpoints_MouseLeave(this, null);
                }
            }

            endpointToolTipLastPosition = e.Location;
        }

        public void RestoreWindowSizeAndPosition()
        {
            if (Settings.Default.HasSavedFormWindowSizeAndPosition)
            {
                try
                {
                    WindowState = Settings.Default.FormWindow_WindowState;

                    if (Settings.Default.FormWindow_Hidden)
                    {
                        Hide();
                    }
                    else
                    {
                        Show();
                    }
                }
                catch
                {
                    RestoreSavedSettingsError("Window State");
                }

                try
                {
                    Location = Settings.Default.FormWindow_Location;
                }
                catch
                {
                    RestoreSavedSettingsError("Window Location");
                }

                try
                {
                    Size = Settings.Default.FormWindow_Size;
                }
                catch
                {
                    RestoreSavedSettingsError("Window Size");
                }
            }
        }

        public void RestoreFromTray()
        {
            // SHOW FPORM AND BRING WINDOW TO FRONT
            TopMost = true;
            Show();
            TopMost = false;

            // SET TO NORMAL SIZE IF MINIMIZED
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
            }
        }

        public void SaveWindowSizeAndPosition()
        {
            if (WindowState == FormWindowState.Normal)
            {
                Settings.Default.FormWindow_Location = Location;
                Settings.Default.FormWindow_Size = Size;
            }
            else
            {
                Settings.Default.FormWindow_Location = RestoreBounds.Location;
                Settings.Default.FormWindow_Size = RestoreBounds.Size;
            }

            Settings.Default.FormWindow_WindowState = WindowState;
            Settings.Default.FormWindow_Hidden = !Visible;

            Settings.Default.HasSavedFormWindowSizeAndPosition = true;
            Settings.Default.Save();
        }

        public void SaveListViewColumnsWidthAndOrder()
        {
            ThreadSafeInvoke(() =>
            {
                // RESTORE ALL COLUMNS BEFORE SAVE [PROTOCOL VALIDATION MODE]
                if (comboBox_Validate.SelectedIndex == 0)
                {
                    // SAVE COLUMNS WIDTH
                    Settings.Default.ListView_ColWidth_Service = GetEndpointColumnWidthForSave(ch_EndpointName);
                    Settings.Default.ListView_ColWidth_Protocol = GetEndpointColumnWidthForSave(ch_Protocol);
                    Settings.Default.ListView_ColWidth_Port = GetEndpointColumnWidthForSave(ch_Port);
                    Settings.Default.ListView_ColWidth_Endpoint = GetEndpointColumnWidthForSave(ch_EndpointURL);
                    Settings.Default.ListView_ColWidth_IPAddress = GetEndpointColumnWidthForSave(ch_IPAddress);
                    Settings.Default.ListView_ColWidth_ResponseTime = GetEndpointColumnWidthForSave(ch_ResponseTime);
                    Settings.Default.ListView_ColWidth_Code = GetEndpointColumnWidthForSave(ch_Code);
                    Settings.Default.ListView_ColWidth_Message = GetEndpointColumnWidthForSave(ch_Message);
                    Settings.Default.ListView_ColWidth_LastSeenOnline = GetEndpointColumnWidthForSave(ch_LastSeenOnline);
                    Settings.Default.ListView_ColWidth_MACAddress = GetEndpointColumnWidthForSave(ch_MACAddress);
                    Settings.Default.ListView_ColWidth_PingTime = GetEndpointColumnWidthForSave(ch_PingTime);
                    Settings.Default.ListView_ColWidth_Server = GetEndpointColumnWidthForSave(ch_Server);
                    Settings.Default.ListView_ColWidth_UserName = GetEndpointColumnWidthForSave(ch_UserName);
                    Settings.Default.ListView_ColWidth_NetworkShares = GetEndpointColumnWidthForSave(ch_NetworkShares);
                    Settings.Default.ListView_ColWidth_DNSName = GetEndpointColumnWidthForSave(ch_DNSName);
                    Settings.Default.ListView_ColWidth_ContentLength = GetEndpointColumnWidthForSave(ch_HTTPContentLength);
                    Settings.Default.ListView_ColWidth_ContentType = GetEndpointColumnWidthForSave(ch_HTTPContentType);
                    Settings.Default.ListView_ColWidth_Expires = GetEndpointColumnWidthForSave(ch_HTTPExpires);
                    Settings.Default.ListView_ColWidth_ETag = GetEndpointColumnWidthForSave(ch_HTTPETag);
                    Settings.Default.Config_VisibleColumns = string.Join(",", GetUserEndpointColumns().Where(column => column.Width > 0).Select(column => column.Name));

                    // SAVE COLUMNS DISPLAY INDEX [ORDER]
                    Settings.Default.ListView_DisplayIndex_Service = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_EndpointName)].DisplayIndex;
                    Settings.Default.ListView_DisplayIndex_Protocol = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_Protocol)].DisplayIndex;
                    Settings.Default.ListView_DisplayIndex_Port = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_Port)].DisplayIndex;
                    Settings.Default.ListView_DisplayIndex_Endpoint = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_EndpointURL)].DisplayIndex;
                    Settings.Default.ListView_DisplayIndex_IPAddress = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_IPAddress)].DisplayIndex;
                    Settings.Default.ListView_DisplayIndex_ResponseTime = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_ResponseTime)].DisplayIndex;
                    Settings.Default.ListView_DisplayIndex_Code = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_Code)].DisplayIndex;
                    Settings.Default.ListView_DisplayIndex_Message = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_Message)].DisplayIndex;
                    Settings.Default.ListView_DisplayIndex_LastSeenOnline = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_LastSeenOnline)].DisplayIndex;
                    Settings.Default.ListView_DisplayIndex_MACAddress = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_MACAddress)].DisplayIndex;
                    Settings.Default.ListView_DisplayIndex_PingTime = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_PingTime)].DisplayIndex;
                    Settings.Default.ListView_DisplayIndex_Server = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_Server)].DisplayIndex;
                    Settings.Default.ListView_DisplayIndex_UserName = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_UserName)].DisplayIndex;
                    Settings.Default.ListView_DisplayIndex_NetworkShares = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_NetworkShares)].DisplayIndex;
                    Settings.Default.ListView_DisplayIndex_DNSName = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_DNSName)].DisplayIndex;
                    Settings.Default.ListView_DisplayIndex_ContentLength = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_HTTPContentLength)].DisplayIndex;
                    Settings.Default.ListView_DisplayIndex_ContentType = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_HTTPContentType)].DisplayIndex;
                    Settings.Default.ListView_DisplayIndex_Expires = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_HTTPExpires)].DisplayIndex;
                    Settings.Default.ListView_DisplayIndex_ETag = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_HTTPETag)].DisplayIndex;

                    Settings.Default.Save();
                }
            });
        }

        public void RestoreListViewColumnsWidthAndOrder()
        {
            try
            {
                // RESTORE COLUMNS WIDTH
                if (Settings.Default.ListView_ColWidth_Service != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_EndpointName)].Width = Settings.Default.ListView_ColWidth_Service; }
                if (Settings.Default.ListView_ColWidth_Protocol != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_Protocol)].Width = Settings.Default.ListView_ColWidth_Protocol; }
                if (Settings.Default.ListView_ColWidth_Port != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_Port)].Width = Settings.Default.ListView_ColWidth_Port; }
                if (Settings.Default.ListView_ColWidth_Endpoint != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_EndpointURL)].Width = Settings.Default.ListView_ColWidth_Endpoint; }
                if (Settings.Default.ListView_ColWidth_IPAddress != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_IPAddress)].Width = Settings.Default.ListView_ColWidth_IPAddress; }
                if (Settings.Default.ListView_ColWidth_ResponseTime != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_ResponseTime)].Width = Settings.Default.ListView_ColWidth_ResponseTime; }
                if (Settings.Default.ListView_ColWidth_Code != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_Code)].Width = Settings.Default.ListView_ColWidth_Code; }
                if (Settings.Default.ListView_ColWidth_Message != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_Message)].Width = Settings.Default.ListView_ColWidth_Message; }
                if (Settings.Default.ListView_ColWidth_LastSeenOnline != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_LastSeenOnline)].Width = Settings.Default.ListView_ColWidth_LastSeenOnline; }
                if (Settings.Default.ListView_ColWidth_MACAddress != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_MACAddress)].Width = Settings.Default.ListView_ColWidth_MACAddress; }
                if (Settings.Default.ListView_ColWidth_PingTime != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_PingTime)].Width = Settings.Default.ListView_ColWidth_PingTime; }
                if (Settings.Default.ListView_ColWidth_Server != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_Server)].Width = Settings.Default.ListView_ColWidth_Server; }
                if (Settings.Default.ListView_ColWidth_UserName != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_UserName)].Width = Settings.Default.ListView_ColWidth_UserName; }
                if (Settings.Default.ListView_ColWidth_NetworkShares != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_NetworkShares)].Width = Settings.Default.ListView_ColWidth_NetworkShares; }
                if (Settings.Default.ListView_ColWidth_DNSName != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_DNSName)].Width = Settings.Default.ListView_ColWidth_DNSName; }
                if (Settings.Default.ListView_ColWidth_ContentLength != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_HTTPContentLength)].Width = Settings.Default.ListView_ColWidth_ContentLength; }
                if (Settings.Default.ListView_ColWidth_ContentType != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_HTTPContentType)].Width = Settings.Default.ListView_ColWidth_ContentType; }
                if (Settings.Default.ListView_ColWidth_Expires != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_HTTPExpires)].Width = Settings.Default.ListView_ColWidth_Expires; }
                if (Settings.Default.ListView_ColWidth_ETag != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_HTTPETag)].Width = Settings.Default.ListView_ColWidth_ETag; }

                // RESTORE COLUMNS DISPLAY INDEX [ORDER]
                if (Settings.Default.ListView_DisplayIndex_Service != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_EndpointName)].DisplayIndex = Settings.Default.ListView_DisplayIndex_Service; }
                if (Settings.Default.ListView_DisplayIndex_Protocol != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_Protocol)].DisplayIndex = Settings.Default.ListView_DisplayIndex_Protocol; }
                if (Settings.Default.ListView_DisplayIndex_Port != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_Port)].DisplayIndex = Settings.Default.ListView_DisplayIndex_Port; }
                if (Settings.Default.ListView_DisplayIndex_Endpoint != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_EndpointURL)].DisplayIndex = Settings.Default.ListView_DisplayIndex_Endpoint; }
                if (Settings.Default.ListView_DisplayIndex_IPAddress != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_IPAddress)].DisplayIndex = Settings.Default.ListView_DisplayIndex_IPAddress; }
                if (Settings.Default.ListView_DisplayIndex_ResponseTime != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_ResponseTime)].DisplayIndex = Settings.Default.ListView_DisplayIndex_ResponseTime; }
                if (Settings.Default.ListView_DisplayIndex_Code != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_Code)].DisplayIndex = Settings.Default.ListView_DisplayIndex_Code; }
                if (Settings.Default.ListView_DisplayIndex_Message != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_Message)].DisplayIndex = Settings.Default.ListView_DisplayIndex_Message; }
                if (Settings.Default.ListView_DisplayIndex_LastSeenOnline != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_LastSeenOnline)].DisplayIndex = Settings.Default.ListView_DisplayIndex_LastSeenOnline; }
                if (Settings.Default.ListView_DisplayIndex_MACAddress != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_MACAddress)].DisplayIndex = Settings.Default.ListView_DisplayIndex_MACAddress; }
                if (Settings.Default.ListView_DisplayIndex_PingTime != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_PingTime)].DisplayIndex = Settings.Default.ListView_DisplayIndex_PingTime; }
                if (Settings.Default.ListView_DisplayIndex_Server != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_Server)].DisplayIndex = Settings.Default.ListView_DisplayIndex_Server; }
                if (Settings.Default.ListView_DisplayIndex_UserName != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_UserName)].DisplayIndex = Settings.Default.ListView_DisplayIndex_UserName; }
                if (Settings.Default.ListView_DisplayIndex_NetworkShares != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_NetworkShares)].DisplayIndex = Settings.Default.ListView_DisplayIndex_NetworkShares; }
                if (Settings.Default.ListView_DisplayIndex_DNSName != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_DNSName)].DisplayIndex = Settings.Default.ListView_DisplayIndex_DNSName; }
                if (Settings.Default.ListView_DisplayIndex_ContentLength != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_HTTPContentLength)].DisplayIndex = Settings.Default.ListView_DisplayIndex_ContentLength; }
                if (Settings.Default.ListView_DisplayIndex_ContentType != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_HTTPContentType)].DisplayIndex = Settings.Default.ListView_DisplayIndex_ContentType; }
                if (Settings.Default.ListView_DisplayIndex_Expires != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_HTTPExpires)].DisplayIndex = Settings.Default.ListView_DisplayIndex_Expires; }
                if (Settings.Default.ListView_DisplayIndex_ETag != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_HTTPETag)].DisplayIndex = Settings.Default.ListView_DisplayIndex_ETag; }
            }
            catch
            {
                RestoreSavedSettingsError("Endpoints List Columns Setting");
            }
        }

        public void UpdateLastSeenOnlineList()
        {
            foreach (EndpointDefinition endpoint in endpointsList)
            {
                endpointsList_LastSeenOnline[endpoint.Name] = endpoint.LastSeenOnline;
            }
        }

        public void RestoreLastSeenOnlineList()
        {
            if (File.Exists(lastSeenOnlineJSONFile))
            {
                try
                {
                    Dictionary<string, string> _endpointsList_LastSeenOnline = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(lastSeenOnlineJSONFile));

                    if (_endpointsList_LastSeenOnline != null &&
                        _endpointsList_LastSeenOnline.Count > 0)
                    {
                        endpointsList_LastSeenOnline = _endpointsList_LastSeenOnline;
                    }
                }
                catch
                {
                }
            }
        }

        public void LoadEndpointReferences()
        {
            NewBackgroundThread(() =>
            {
                // CLEAR ENDPOINTS CHECK LIST
                endpointsList.Clear();

                // CHECK DEFINITIONS FILE EXISTENCE
                if (File.Exists(endpointDefinitionsFile))
                {
                    EndpointDefinitionParser endpointDefinitionParser = new EndpointDefinitionParser();
                    List<string> endpointDuplicityList = new List<string>();
                    List<string> invalidURLList = new List<string>();

                    // READ DEFINITIONS FILE LINE BY LINE
                    int lineNumber = 1;
                    string line;
                    StreamReader file = new StreamReader(endpointDefinitionsFile, Encoding.Default, true);
                    while ((line = file.ReadLine()) != null)
                    {
                        // REMOVE SPACES FROM LINE
                        line = line.Trim();

                        // CHECK LINE
                        if (!string.IsNullOrEmpty(line) &&
                            line != "|" &&
                            !line.StartsWith("#"))
                        {
                            // CHECK ITEMS COUNT LIMIT
                            if (lineNumber > Settings.Default.Config_MaximumEndpointReferencesCount)
                            {
                                MessageBox.Show(
                                  "Endpoints definitions file \"" + endpointDefinitionsFile +
                                  "\" contains more than " +
                                  Settings.Default.Config_MaximumEndpointReferencesCount +
                                  " items." +
                                  Environment.NewLine + Environment.NewLine +
                                  "Rest of definitions will be ignored",
                                  "Maximum Endpoints limit reached",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning);

                                break;
                            }

                            // SET PROGRESS STATUS INFORMATION
                            SetProgressStatus(0, 0,
                                "Loading Endpoints References [" + lineNumber + "] ...", Color.Blue);

                            EndpointDefinitionParseResult parseResult = endpointDefinitionParser.ParseLine(line, lineNumber);

                            if (parseResult.DuplicateError != null)
                            {
                                endpointDuplicityList.Add(parseResult.DuplicateError.ToDuplicateDisplayText());
                            }

                            if (parseResult.InvalidUrlError != null)
                            {
                                invalidURLList.Add(parseResult.InvalidUrlError.ToInvalidUrlDisplayText());
                            }
                            else if (parseResult.IsValid)
                            {
                                EndpointDefinition endpointStatusDefiniton = parseResult.EndpointDefinition;

                                // RESTORE 'LAST SEEN ONLINE' VALUE
                                if (endpointsList_LastSeenOnline.ContainsKey(endpointStatusDefiniton.Name))
                                {
                                    endpointStatusDefiniton.LastSeenOnline = endpointsList_LastSeenOnline[endpointStatusDefiniton.Name];
                                }
                                else
                                {
                                    endpointsList_LastSeenOnline.Add(endpointStatusDefiniton.Name, endpointStatusDefiniton.LastSeenOnline);
                                }

                                // ENDPOINT DEFINITION IS VALID, ADD TO ENDPOINTS CHECK LIST
                                endpointsList.Add(endpointStatusDefiniton);
                            }
                        }

                        lineNumber++;
                    }

                    // SET PROGRESS STATUS INFORMATION
                    SetProgressStatus(0, 0, endpointsList.Count + " Valid Endpoints References Loaded", Color.Gray);

                    file.Close();

                    // SORT ENDPOINTS LIST BY ENDPOINT NAME
                    endpointsList.Sort((s, t) => string.Compare(s.Name, t.Name));

                    // CHECK DUPLICITY LIST
                    if (endpointDuplicityList.Count > 0)
                    {
                        // CREATE AND SHOW MESSAGEBOX 
                        string duplicityMessage = "Endpoints definitions file \"" +
                        endpointDefinitionsFile + "\" contains " +
                        endpointDuplicityList.Count + " items with duplicity names.";
                        duplicityMessage += Environment.NewLine;
                        duplicityMessage += Environment.NewLine;
                        duplicityMessage += "This definitions will be ignored because Endpoint Name must be unique identifier.";
                        duplicityMessage += Environment.NewLine;
                        duplicityMessage += Environment.NewLine;

                        if (endpointDuplicityList.Count <= 5)
                        {
                            // LIST AFFECTED DEFINITIONS ITEMS
                            duplicityMessage += string.Join(string.Empty, endpointDuplicityList.ToArray());
                        }
                        else
                        {
                            try
                            {
                                using (StreamWriter sw = new StreamWriter(endpointsList_Duplicities))
                                {
                                    // WRITE AFFECTED DEFINITIONS ITEMS
                                    sw.WriteLine(duplicityMessage + string.Join(string.Empty, endpointDuplicityList.ToArray()));
                                }
                            }
                            catch
                            {
                            }

                            duplicityMessage += "See \"" + endpointsList_Duplicities + "\" for details.";
                        }

                        MessageBox.Show(
                                duplicityMessage,
                                "Invalid endpoint definitions - Name duplicity",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                    }

                    // CHECK INVALID FORMAT LIST
                    if (invalidURLList.Count > 0)
                    {
                        // CREATE AND SHOW MESSAGEBOX, LIST AFFECTED DEFINITIONS ITEMS
                        string invalidURLMessage = "Endpoints definitions file \"" +
                        endpointDefinitionsFile + "\" contains " +
                        invalidURLList.Count + " items with URL in invalid format.";
                        invalidURLMessage += Environment.NewLine;
                        invalidURLMessage += Environment.NewLine;
                        invalidURLMessage += "This definitions will be ignored.";
                        invalidURLMessage += Environment.NewLine;
                        invalidURLMessage += Environment.NewLine;

                        if (invalidURLList.Count <= 5)
                        {
                            // LIST AFFECTED DEFINITIONS ITEMS
                            invalidURLMessage += string.Join(string.Empty, invalidURLList.ToArray());
                        }
                        else
                        {
                            try
                            {
                                using (StreamWriter sw = new StreamWriter(endpointsList_InvalidDefinitions))
                                {
                                    // WRITE AFFECTED DEFINITIONS ITEMS
                                    sw.WriteLine(
                                        invalidURLMessage +
                                        string.Join(string.Empty, invalidURLList.ToArray()));
                                }
                            }
                            catch
                            {
                            }

                            invalidURLMessage += "See \"" + endpointsList_InvalidDefinitions + "\" for details.";
                        }

                        MessageBox.Show(invalidURLMessage, "Invalid endpoint definitions - Invalid URL format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    // RESTORE DISABLED ITEMS LIST
                    RestoreDisabledItemsListAndFilter();

                    // LIST ENDPOINTS
                    ThreadSafeInvoke(() =>
                    {
                        ListEndpoints(ListViewRefreshMethod.CurrentState);

                        RefreshTrayIcon();
                    });

                    // AUTOMATIC REFRESH
                    if (app_ScanOnStartup)
                    {
                        ThreadSafeInvoke(() =>
                        {
                            btn_RunCheck_Click(this, null);
                        });
                    }

                    // CONTINUOUS REFRESH
                    if (cb_ContinuousRefresh.Checked)
                    {
                        TIMER_ContinuousRefresh.Start();
                    }
                }
                else
                {
                    ThreadSafeInvoke(() =>
                    {
                        SetControls(false, true);
                        lbl_EndpointsListLoading.ForeColor = Color.Red;
                        lbl_EndpointsListLoading.Text = "Endpoints definitions file \"" + endpointDefinitionsFile + "\" doesn't exists in \"" + Directory.GetCurrentDirectory() + "\".";
                    });
                }
            });
        }

        public void SaveLastSeenOnlineList()
        {
            try
            {
                if (endpointsList_LastSeenOnline.Count > 0)
                {
                    string jsonString = JsonConvert.SerializeObject(endpointsList_LastSeenOnline, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText(lastSeenOnlineJSONFile, jsonString, Encoding.UTF8);
                }
            }
            catch
            {
            }
        }

        private int GetEndpointColumnWidthForSave(ColumnHeader column)
        {
            if (column.Width > 0)
            {
                return column.Width;
            }

            return endpointColumnWidths.TryGetValue(column, out int width)
                ? Math.Max(40, width)
                : 80;
        }

        public void RestoreDisabledItemsListAndFilter()
        {
            ThreadSafeInvoke(() =>
            {
                tb_ListFilter.Text = Settings.Default.ListView_Filter;
            });

            if (!string.IsNullOrEmpty(Settings.Default.DisabledItemsList))
            {
                foreach (string disabledItem in Settings.Default.DisabledItemsList.Split('|'))
                {
                    if (endpointsList.Where(endpointItem => endpointItem.Name == disabledItem).Count().Equals(1))
                    {
                        endpointsList_Disabled.Add(disabledItem);
                    }
                }
            }
        }

        public void SaveDisabledItemsListAndFilter()
        {
            Settings.Default.ListView_Filter = tb_ListFilter.Text;

            Settings.Default.DisabledItemsList = string.Join("|", endpointsList_Disabled.ToList());
            Settings.Default.Save();
        }

        public void cb_ExportEndpointsStatus_XLSX_CheckedChanged(object sender, EventArgs e)
        {
            if (!cb_ExportEndpointsStatus_XLSX.Checked)
            {
                // DISABLE HTML EXPORT TOO
                cb_ExportEndpointsStatus_HTML.Checked = false;

                CloseFileStream(definitionsStatusExport_XLSX_FileStream);
            }

            SaveConfiguration();
        }

        public void cb_ExportEndpointsStatus_HTML_CheckedChanged(object sender, EventArgs e)
        {
            if (!cb_ExportEndpointsStatus_HTML.Checked)
            {
                CloseFileStream(definitionsStatusExport_HTML_Info_FileStream);
                CloseFileStream(definitionsStatusExport_HTML_HTTP_FileStream);
                CloseFileStream(definitionsStatusExport_HTML_FTP_FileStream);
            }
            else
            {
                // ENABLE XLSX EXPORT TOO
                cb_ExportEndpointsStatus_XLSX.Checked = true;
            }

            SaveConfiguration();
        }

        public void SetTrayIcon(int firstFrameIndex, int lastFrameIndex, int animationInterval_mS = 50)
        {
            TIMER_TrayIconAnimation.Stop();
            trayAnimation_Icons.Clear();
            trayAnimation_Index = 0;

            if (firstFrameIndex == lastFrameIndex)
            {
                trayIcon.Icon = GetIconFromListByIndex(firstFrameIndex);
            }
            else if (lastFrameIndex > firstFrameIndex)
            {
                for (int index = firstFrameIndex; index <= lastFrameIndex; index++)
                {
                    trayAnimation_Icons.Add(GetIconFromListByIndex(index));
                }

                TIMER_TrayIconAnimation.Interval = animationInterval_mS;
                TIMER_TrayIconAnimation.Start();
            }
        }

        public void cb_ResolvePageMetaInfo_CheckedChanged(object sender, EventArgs e)
        {
            if (!cb_ResolvePageMetaInfo.Checked)
            {
                cb_ResolvePageLinks.Checked = false;
            }

            SaveConfiguration();
        }

        public void cb_SaveResponse_CheckedChanged(object sender, EventArgs e)
        {
            SaveConfiguration();
        }

        public void lv_Endpoints_KeyUp(object sender, KeyEventArgs e)
        {
            if (sender != lv_Endpoints)
            {
                return;
            }

            if (e.Control && e.KeyCode == Keys.C)
            {
                CopySelectedValuesToClipboard();
            }
        }

        public void CopySelectedValuesToClipboard()
        {
            StringBuilder builder = new StringBuilder();
            foreach (ListViewItem item in lv_Endpoints.SelectedItems)
            {
                string itemLineText = string.Empty;

                foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
                {
                    itemLineText += subItem.Name + ": " + subItem.Text + Environment.NewLine;
                }

                itemLineText += Environment.NewLine;

                builder.AppendLine(itemLineText);
            }

            if (!string.IsNullOrEmpty(builder.ToString()))
            {
                try
                {
                    Clipboard.SetText(builder.ToString());
                }
                catch
                {
                }
            }
        }

        public void RestoreSavedSettingsError(string settingName)
        {
            MessageBox.Show(
                            "An error occurred while trying to apply your user specific settings" +
                            Environment.NewLine + Environment.NewLine +
                            "The program will continue to run, however saved user preferences " +
                            "[" + settingName + "] will need to be reset",
                            Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
        }

        public void btn_BrowseExportDir_MouseClick(object sender, MouseEventArgs e)
        {
            if (Directory.Exists(statusExport_Directory))
            {
                folderBrowserExportDir.SelectedPath = statusExport_Directory;
            }

            if (folderBrowserExportDir.ShowDialog() == DialogResult.OK)
            {
                statusExport_Directory = folderBrowserExportDir.SelectedPath;

                try
                {
                    CloseFileStream(definitionsStatusExport_JSON_FileStream);
                    using (File.Create(Path.Combine(statusExport_Directory, statusExport_JSONFile))) { }
                    File.Delete(Path.Combine(statusExport_Directory, statusExport_JSONFile));

                    CloseFileStream(definitionsStatusExport_XML_FileStream);
                    using (File.Create(Path.Combine(statusExport_Directory, statusExport_XMLFile))) { }
                    File.Delete(Path.Combine(statusExport_Directory, statusExport_XMLFile));

                    CloseFileStream(definitionsStatusExport_XLSX_FileStream);
                    using (File.Create(Path.Combine(statusExport_Directory, statusExport_XLSFile))) { }
                    File.Delete(Path.Combine(statusExport_Directory, statusExport_XLSFile));

                    CloseFileStream(definitionsStatusExport_HTML_Info_FileStream);
                    using (File.Create(Path.Combine(statusExport_Directory, statusExport_HTMLFile_InfoPage))) { }
                    File.Delete(Path.Combine(statusExport_Directory, statusExport_HTMLFile_InfoPage));

                    CloseFileStream(definitionsStatusExport_HTML_HTTP_FileStream);
                    using (File.Create(Path.Combine(statusExport_Directory, statusExport_HTMLFile_HTTPPage))) { }
                    File.Delete(Path.Combine(statusExport_Directory, statusExport_HTMLFile_HTTPPage));

                    CloseFileStream(definitionsStatusExport_HTML_FTP_FileStream);
                    using (File.Create(Path.Combine(statusExport_Directory, statusExport_HTMLFile_FTPPage))) { }
                    File.Delete(Path.Combine(statusExport_Directory, statusExport_HTMLFile_FTPPage));
                }
                catch (Exception exception)
                {
                    ExceptionNotify(this, exception, string.Empty, true);
                }

                SaveConfiguration();
            }
        }

        public void CloseFileStream(FileStream fileStream)
        {
            if (fileStream != null)
            {
                // CLOSE
                fileStream.Close();
            }
        }

        public FileStream OpenFileStream(string fileName)
        {
            // OPEN
            return new FileStream(
                                  fileName,
                                  FileMode.OpenOrCreate,
                                  FileAccess.ReadWrite,
                                  FileShare.Read);
        }

        public void num_PingTimeout_ValueChanged(object sender, EventArgs e)
        {
            lbl_PingTimeoutSecondsText.Text = GetFormattedValueCountString((int)num_PingTimeout.Value, "second");

            SaveConfiguration();
        }

        public string GetFormattedValueCountString(int value, string valueName, bool appendValueToString = false)
        {
            if (value != 1)
            {
                valueName += "s";
            }

            if (appendValueToString)
            {
                valueName = value + " " + valueName;
            }

            return valueName;
        }

        public void comboBox_Validate_SelectedIndexChanged(object sender, EventArgs e)
        {
            validationMethod = (ValidationMethod)Enum.ToObject(typeof(ValidationMethod), comboBox_Validate.SelectedIndex);

            if (!onClose && lv_Endpoints.Visible)
            {
                SaveConfiguration();

                ListEndpoints(ListViewRefreshMethod.CurrentState);

                RefreshTrayIcon();
            }

            if (validationMethod == ValidationMethod.Protocol)
            {
                if (ch_Code.Tag != null) { ch_Code.Width = (int)ch_Code.Tag; }
                if (ch_HTTPContentLength.Tag != null) { ch_HTTPContentLength.Width = (int)ch_HTTPContentLength.Tag; }
                if (ch_HTTPContentType.Tag != null) { ch_HTTPContentType.Width = (int)ch_HTTPContentType.Tag; }
                if (ch_HTTPExpires.Tag != null) { ch_HTTPExpires.Width = (int)ch_HTTPExpires.Tag; }
                if (ch_HTTPETag.Tag != null) { ch_HTTPETag.Width = (int)ch_HTTPETag.Tag; }
                if (ch_Port.Tag != null) { ch_Port.Width = (int)ch_Port.Tag; }
                if (ch_Protocol.Tag != null) { ch_Protocol.Width = (int)ch_Protocol.Tag; }
                if (ch_ResponseTime.Tag != null) { ch_ResponseTime.Width = (int)ch_ResponseTime.Tag; }
                if (ch_Server.Tag != null) { ch_Server.Width = (int)ch_Server.Tag; }
                if (ch_UserName.Tag != null) { ch_UserName.Width = (int)ch_UserName.Tag; }
            }
            else
            {
                ch_Code.Tag = ch_Code.Width;
                ch_HTTPContentLength.Tag = ch_HTTPContentLength.Width;
                ch_HTTPExpires.Tag = ch_HTTPExpires.Width;
                ch_HTTPETag.Tag = ch_HTTPETag.Width;
                ch_HTTPContentType.Tag = ch_HTTPContentType.Width;
                ch_Port.Tag = ch_Port.Width;
                ch_Protocol.Tag = ch_Protocol.Width;
                ch_ResponseTime.Tag = ch_ResponseTime.Width;
                ch_Server.Tag = ch_Server.Width;
                ch_UserName.Tag = ch_UserName.Width;

                ch_Code.Width = 0;
                ch_HTTPContentLength.Width = 0;
                ch_HTTPExpires.Width = 0;
                ch_HTTPETag.Width = 0;
                ch_HTTPContentType.Width = 0;
                ch_Port.Width = 0;
                ch_Protocol.Width = 0;
                ch_ResponseTime.Width = 0;
                ch_Server.Width = 0;
                ch_UserName.Width = 0;

                cb_TestPing.Checked = true;
            }
        }

        public static bool IsLocalHost(string host)
        {
            try
            {
                // get host IP addresses
                IPAddress[] hostIPs = Dns.GetHostAddresses(host);

                // get local IP addresses
                IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

                // test if any host IP equals to any local IP or to localhost
                foreach (IPAddress hostIP in hostIPs)
                {
                    // is localhost
                    if (IPAddress.IsLoopback(hostIP))
                    {
                        return true;
                    }
                    // is local address
                    foreach (IPAddress localIP in localIPs)
                    {
                        if (hostIP.Equals(localIP))
                        {
                            return true;
                        }
                    }
                }
            }
            catch
            {
            }

            return false;
        }
                
        public static void GetLocalDNSAndGWAddresses(out List<string> localDNSAndGWipAddresses, out List<string> localDNSAndGWmacAddresses)
        {
            localDNSAndGWipAddresses = new List<string>();
            localDNSAndGWmacAddresses = new List<string>();

            try
            {
                ManagementScope oMs = new ManagementScope("\\\\localhost\\root\\cimv2");

                // DNS
                string strQueryDNS = "select DNSServerSearchOrder from Win32_NetworkAdapterConfiguration where IPEnabled='true'";
                ObjectQuery oQDNS = new ObjectQuery(strQueryDNS);
                ManagementObjectSearcher oSDNS = new ManagementObjectSearcher(oMs, oQDNS);
                ManagementObjectCollection oRcDNS = oSDNS.Get();

                foreach (ManagementBaseObject oRDNS in oRcDNS)
                {
                    foreach (PropertyData property in oRDNS.Properties)
                    {
                        if (property.Value != null)
                        {
                            foreach (string dnsIP in (Array)property.Value)
                            {
                                if (!string.IsNullOrEmpty(dnsIP) &&
                                    !localDNSAndGWipAddresses.Contains(dnsIP))
                                {
                                    localDNSAndGWipAddresses.Add(dnsIP);
                                    localDNSAndGWmacAddresses.Add(WindowsLookupService.Lookup(IPAddress.Parse(dnsIP)));
                                }
                            }
                        }
                    }
                }

                // GW
                string strQueryGW = "select DefaultIPGateway from Win32_NetworkAdapterConfiguration where IPEnabled='true'";
                ObjectQuery oQGW = new ObjectQuery(strQueryGW);
                ManagementObjectSearcher oSGW = new ManagementObjectSearcher(oMs, oQGW);
                ManagementObjectCollection oRcGW = oSGW.Get();

                foreach (ManagementBaseObject oRGW in oRcGW)
                {
                    foreach (PropertyData property in oRGW.Properties)
                    {
                        if (property.Value != null)
                        {
                            foreach (string gwIP in (Array)property.Value)
                            {
                                if (!string.IsNullOrEmpty(gwIP) &&
                                    !localDNSAndGWipAddresses.Contains(gwIP))
                                {
                                    localDNSAndGWipAddresses.Add(gwIP);
                                    localDNSAndGWmacAddresses.Add(WindowsLookupService.Lookup(IPAddress.Parse(gwIP)));
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }
        public static string GetEnumDescriptionString(Enum enumValue)
        {
            FieldInfo field = enumValue.GetType().GetField(enumValue.ToString());


            return !(Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute) ? enumValue.ToString() : attribute.Description;
        }

        public static int GetEnumByDescriptionString(string description, Type enumType)
        {
            foreach (FieldInfo field in enumType.GetFields())
            {
                if (!(Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute))
                {
                    continue;
                }

                if (attribute.Description == description)
                {
                    return (int)field.GetValue(null);
                }
            }
            return 0;
        }

        public void lv_Endpoints_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right &&
                lv_Endpoints.FocusedItem.Bounds.Contains(e.Location) &&
                lv_Endpoints_SelectedEndpointsList.Count > 0)
            {
                // IF ENDPOINT STATUS IN N/A, REMOVE 'DETAILS' FROM CONTEXT MENU
                if ((validationMethod == ValidationMethod.Protocol &&
                     lv_Endpoints_SelectedEndpointsList.First().ResponseCode != status_NotAvailable &&
                     lv_Endpoints_SelectedEndpointsList.First().ResponseCode != status_Error) ||
                    (validationMethod == ValidationMethod.Ping &&
                     lv_Endpoints_SelectedEndpointsList.First().PingRoundtripTime != status_NotAvailable))
                {
                    toolStripSeparator_2.Visible = true;
                    toolStripMenuItem_Details.Visible = true;
                }
                else
                {
                    toolStripSeparator_2.Visible = false;
                    toolStripMenuItem_Details.Visible = false;
                }


                lv_Endpoints_ContextMenuStrip.Show(Cursor.Position);
            }
        }

        public void toolStripMenuItem_Details_Click(object sender, EventArgs e)
        {
            tray_RunCheck.Visible = false;
            tray_SpeedTest.Visible = false;
            tray_Separator_1.Visible = false;

            dialog_EndpointDetails = new EndpointDetailsDialog(
                (int)num_PingTimeout.Value * 1000,
                lv_Endpoints_SelectedEndpointsList.First(),
                imageList_Icons_32pix
                        .Images[GetStatusImageIndex(
                            lv_Endpoints_SelectedEndpointsList.First().ResponseCode,
                            lv_Endpoints_SelectedEndpointsList.First().PingRoundtripTime,
                            lv_Endpoints_SelectedEndpointsList.First().ResponseMessage)]);

            dialog_EndpointDetails.ShowDialog();
            dialog_EndpointDetails = null;

            if (onClose)
            {
                Application.Exit();
            }

            tray_RunCheck.Visible = true;
            tray_SpeedTest.Visible = true;
            tray_Separator_1.Visible = true;

        }

        private void toolStripMenuItem_AdminBrowse_Click(object sender, EventArgs e)
        {
            foreach (EndpointDefinition selectedEndpointDefinition in lv_Endpoints_SelectedEndpointsList)
            {
                BrowseEndpoint_WindowsExplorer(
                    new Uri(selectedEndpointDefinition.ResponseAddress).Host + @"\C$",
                            selectedEndpointDefinition.LoginName,
                            selectedEndpointDefinition.LoginPass);

                WindowState = FormWindowState.Minimized;
            }
        }

        public void toolStripMenuItem_Browse_Click(object sender, EventArgs e)
        {
            foreach (EndpointDefinition selectedEndpointDefinition in lv_Endpoints_SelectedEndpointsList)
            {
                BrowseEndpoint_WindowsExplorer(
                    new Uri(selectedEndpointDefinition.ResponseAddress).Host,
                            selectedEndpointDefinition.LoginName,
                            selectedEndpointDefinition.LoginPass);

                WindowState = FormWindowState.Minimized;
            }            
        }

        public void toolStripMenuItem_HTTP_Click(object sender, EventArgs e)
        {
            foreach (EndpointDefinition selectedEndpointDefinition in lv_Endpoints_SelectedEndpointsList)
            {
                OpenEndpoint_HTTP(selectedEndpointDefinition);

                WindowState = FormWindowState.Minimized;
            }
        }

        public void toolStripMenuItem_FTP_Click(object sender, EventArgs e)
        {
            foreach (EndpointDefinition selectedEndpointDefinition in lv_Endpoints_SelectedEndpointsList)
            {
                OpenEndpoint_FTP(selectedEndpointDefinition);

                WindowState = FormWindowState.Minimized;

            }            
        }

        public void toolStripMenuItem_RDP_Click(object sender, EventArgs e)
        {
            foreach (EndpointDefinition selectedEndpointDefinition in lv_Endpoints_SelectedEndpointsList)
            {
                ConnectEndpoint_RDP(new Uri(selectedEndpointDefinition.ResponseAddress).Host);

                WindowState = FormWindowState.Minimized;
            }            
        }

        public void toolStripMenuItem_VNC_Click(object sender, EventArgs e)
        {
            foreach (EndpointDefinition selectedEndpointDefinition in lv_Endpoints_SelectedEndpointsList)
            {
                ConnectEndpoint_VNC(new Uri(selectedEndpointDefinition.ResponseAddress).Host);

                WindowState = FormWindowState.Minimized;
            }            
        }

        public void toolStripMenuItem_SSH_Click(object sender, EventArgs e)
        {
            foreach (EndpointDefinition selectedEndpointDefinition in lv_Endpoints_SelectedEndpointsList)
            {
                ConnectEndpoint_Putty(new Uri(selectedEndpointDefinition.ResponseAddress).Host);

                WindowState = FormWindowState.Minimized;
            }
        }

        public void BrowseEndpoint_WindowsExplorer(
                                                          string endpointAddress,
                                                          string userName,
                                                          string userPassword)
        {
            string link = @"\\" + endpointAddress;

            try
            {
                StartBackgroundProcess(
                             link,
                             string.Empty,
                             userName,
                             userPassword);
            }
            catch (Exception exception)
            {
                ExceptionNotify(null, exception, string.Empty, true);
            }
        }

        public void ConnectEndpoint_RDP(string endpointAddress)
        {
            try
            {
                StartBackgroundProcess(
                             "mstsc.exe",
                             "/v:" + endpointAddress + " /admin",
                             null,
                             null);
            }
            catch (Exception exception)
            {
                ExceptionNotify(null, exception, string.Empty, true);
            }
        }

        public void ConnectEndpoint_VNC(string endpointAddress)
        {
            if (!string.IsNullOrEmpty(appExecutable_VNC) &&
                File.Exists(appExecutable_VNC))
            {
                try
                {
                    StartBackgroundProcess(
                                 appExecutable_VNC,
                                 endpointAddress,
                                 null,
                                 null);
                }
                catch (Exception exception)
                {
                    ExceptionNotify(null, exception, string.Empty, true);
                }
            }
            else
            {
                CheckerMainForm mainFormInstance = new CheckerMainForm();

                DialogResult questionDialogResult = MessageBox.Show("Do you want to browse your computer for VNC Viewer executable ?", "VNC Viewer Executable", MessageBoxButtons.YesNo);
                if (questionDialogResult == DialogResult.Yes &&
                    mainFormInstance.openFileDialog_VNCExe.ShowDialog() == DialogResult.OK &&
                    File.Exists(mainFormInstance.openFileDialog_VNCExe.FileName))
                {
                    appExecutable_VNC = mainFormInstance.openFileDialog_VNCExe.FileName;

                    ConnectEndpoint_VNC(endpointAddress);
                }

                mainFormInstance.Close();
            }
        }

        public void ConnectEndpoint_Putty(string endpointAddress)
        {
            if (!string.IsNullOrEmpty(appExecutable_Putty) &&
                File.Exists(appExecutable_Putty))
            {
                try
                {
                    StartBackgroundProcess(
                                 appExecutable_Putty,
                                 "-ssh " + endpointAddress + " 22",
                                 null,
                                 null);
                }
                catch (Exception exception)
                {
                    ExceptionNotify(null, exception, string.Empty, true);
                }
            }
            else
            {
                CheckerMainForm mainFormInstance = new CheckerMainForm();

                DialogResult questionDialogResult = MessageBox.Show("Do you want to browse your computer for Putty executable ?", "Putty Executable", MessageBoxButtons.YesNo);
                if (questionDialogResult == DialogResult.Yes &&
                    mainFormInstance.openFileDialog_PuttyExe.ShowDialog() == DialogResult.OK &&
                    File.Exists(mainFormInstance.openFileDialog_PuttyExe.FileName))
                {
                    appExecutable_Putty = mainFormInstance.openFileDialog_PuttyExe.FileName;

                    ConnectEndpoint_Putty(endpointAddress);
                }

                mainFormInstance.Close();
            }
        }

        public void BrowseEndpoint(
            string endpointAddress,
            string arguments,
            string userName,
            string userPass)
        {
            try
            {
                StartBackgroundProcess(
                             endpointAddress,
                             arguments,
                             userName,
                             userPass);
            }
            catch (Exception exception)
            {
                ExceptionNotify(null, exception, string.Empty, true);
            }
        }

        public void OpenEndpoint_HTTP(EndpointDefinition endpoint)
        {
            // ENDPOINT URI
            Uri _endpointURI = new Uri(endpoint.Address);

            if (_endpointURI.Scheme != Uri.UriSchemeHttp &&
                _endpointURI.Scheme != Uri.UriSchemeHttps)
            {
                // ENDPOINT SCHEME IS OTHER THAN 'HTTP' OR 'HTTPS', PASS DEFAULT 'HTTP' PROTOCOL PREFIX
                _endpointURI = new Uri(
                    Uri.UriSchemeHttp +
                    Uri.SchemeDelimiter +
                    _endpointURI.Host +
                    _endpointURI.PathAndQuery +
                    _endpointURI.Fragment);
            }

            string _endpointAddress = _endpointURI.AbsoluteUri;

            if (!string.IsNullOrEmpty(endpoint.LoginName) &&
                endpoint.LoginName != status_NotAvailable)
            {
                // IF CREDENTIALS ARE SPECIFIED FOR THE ENDPOINT, PASS THEM INTO ADDRESS IN STANDARD WAY
                _endpointAddress =
                    _endpointURI.Scheme +
                    Uri.SchemeDelimiter +
                    endpoint.LoginName +
                    ":" +
                    endpoint.LoginPass +
                    "@" +
                    _endpointURI.Authority +
                    _endpointURI.AbsolutePath;
            }

            BrowseEndpoint(
                _endpointAddress,
                null,
                null,
                null);
        }

        public void OpenEndpoint_FTP(EndpointDefinition endpoint)
        {
            Uri _endpointURI = new Uri(endpoint.Address);

            if (_endpointURI.Scheme != Uri.UriSchemeFtp)
            {
                // IF ENDPOINT IS NOT AN FTP TYPE, PASS FTP PROTOCOL PREFIX
                _endpointURI = new Uri(
                    Uri.UriSchemeFtp +
                    Uri.SchemeDelimiter +
                    _endpointURI.Host +
                    _endpointURI.PathAndQuery +
                    _endpointURI.Fragment);
            }

            string _endpointAddress = _endpointURI.AbsoluteUri;

            if (!string.IsNullOrEmpty(endpoint.LoginName) &&
                endpoint.LoginName != status_NotAvailable)
            {
                // IF CREDENTIALS ARE SPECIFIED FOR THE ENDPOINT, PASS THEM INTO ADDRESS IN STANDARD WAY
                _endpointAddress =
                    Uri.UriSchemeFtp +
                    Uri.SchemeDelimiter +
                    endpoint.LoginName +
                    ":" +
                    endpoint.LoginPass +
                    "@" +
                    _endpointURI.Authority +
                    _endpointURI.AbsolutePath;
            }

            BrowseEndpoint(
                _endpointAddress,
                null,
                null,
                null);
        }

        public void lv_Endpoints_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            lv_Endpoints_SelectedEndpointsList.Clear();

            foreach (ListViewItem selectedItem in lv_Endpoints.SelectedItems)
            {
                lv_Endpoints_SelectedEndpointsList.Add(endpointsList.Where(item => item.Name == selectedItem.Text).First());
            }
        }

        public void StartBackgroundProcess(
                                        string fileName,
                                        string arguments,
                                        string userName,
                                        string userPassword)
        {
            NewBackgroundThread(() =>
            {
                try
                {
                    ProcessStartInfo psInfo = new ProcessStartInfo
                    {
                        FileName = fileName,
                        UseShellExecute = true,
                        ErrorDialog = true
                    };

                    if (!string.IsNullOrEmpty(arguments))
                    {
                        psInfo.Arguments = arguments;
                    }

                    if (!string.IsNullOrEmpty(userName) &&
                        !string.IsNullOrEmpty(userPassword) &&
                        userName != status_NotAvailable &&
                        userPassword != status_NotAvailable)
                    {
                        NetworkConnection netConnection = null;

                        try
                        {
                            netConnection = new NetworkConnection(fileName, new NetworkCredential(userName, userPassword));

                            using (netConnection)
                            {
                                Process.Start(psInfo);
                            }
                        }
                        catch
                        {
                        }
                        finally
                        {
                            if (netConnection != null)
                            {
                                netConnection.Dispose();
                            }
                        }
                    }
                    else
                    {
                        Process.Start(psInfo);
                    }
                }
                catch (Exception ex)
                {
                    ThreadSafeInvoke(() =>
                    {
                        MessageBox.Show(
                        ex.Message,
                        "Open Network Connection",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    });
                }
            });
        }

        public static Image ResizeImage(Image imgPhoto, int Width, int Height)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;
            float nPercentW = Width / (float)sourceWidth;
            float nPercentH = Height / (float)sourceHeight;


            float nPercent;
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = Convert.ToInt16((Width - (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = Convert.ToInt16((Height - (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(Width, Height);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();

            return bmPhoto;
        }

        public void NewBackgroundThread(Action action)
        {
            Application.DoEvents();

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                action();
            })
            .Start();
        }

        public void ThreadSafeInvoke(Action action)
        {
            try
            {
                Invoke(action);
            }
            catch
            {
            }
        }

        private int sortColumn = -1;
        private void lv_Endpoints_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            listUpdating = true;

            if (e.Column != sortColumn)
            {
                sortColumn = e.Column;
                lv_Endpoints.Sorting = SortOrder.Ascending;
            }
            else
            {
                lv_Endpoints.Sorting = lv_Endpoints.Sorting == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            }

            lv_Endpoints.Sort();

            lv_Endpoints.ListViewItemSorter = new ListViewItemComparer(e.Column, lv_Endpoints.Sorting);

            listUpdating = false;
        }

        private void cb_ExportEndpointsStatus_JSON_CheckedChanged(object sender, EventArgs e)
        {
            if (!cb_ExportEndpointsStatus_JSON.Checked)
            {
                CloseFileStream(definitionsStatusExport_JSON_FileStream);
            }

            SaveConfiguration();
        }

        public void cb_ExportEndpointsStatus_XML_CheckedChanged(object sender, EventArgs e)
        {
            if (!cb_ExportEndpointsStatus_XML.Checked)
            {
                CloseFileStream(definitionsStatusExport_XML_FileStream);
            }

            SaveConfiguration();
        }

        public void TIMER_TrayIconAnimation_Tick(object sender, EventArgs e)
        {
            trayIcon.Icon = trayAnimation_Icons[trayAnimation_Index];
            trayAnimation_Index++;
            if (trayAnimation_Index == trayAnimation_Icons.Count())
            {
                trayAnimation_Index = 0;
            }
        }

        public void Lv_Endpoints_MouseLeave(object sender, EventArgs e)
        {
            if (lv_Endpoints != null)
            {
                endpointToolTip.SetToolTip(lv_Endpoints, string.Empty);
            }
        }

        private enum MatchType
        {
            NoMatch,
            ExactMatch,
            ClosestMatch
        };

        public static string GetKnownColorNameString(Color color)
        {
            string colorName = status_NotAvailable;

            if (!color.IsEmpty)
            {
                FindColor(color, out string _colorName);
                colorName = _colorName + " (" + ColorTranslator.ToHtml(color) + ")";
            }

            return colorName;
        }

        private static MatchType FindColor(Color colour, out string name)
        {
            MatchType
              result = MatchType.NoMatch;

            int
              least_difference = 0;

            name = "";

            foreach (PropertyInfo system_colour in typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy))
            {
                Color
                  system_colour_value = (Color)system_colour.GetValue(null, null);

                if (system_colour_value == colour)
                {
                    name = system_colour.Name;
                    result = MatchType.ExactMatch;
                    break;
                }

                int
                  a = colour.A - system_colour_value.A,
                  r = colour.R - system_colour_value.R,
                  g = colour.G - system_colour_value.G,
                  b = colour.B - system_colour_value.B,
                  difference = (a * a) + (r * r) + (g * g) + (b * b);

                if (result == MatchType.NoMatch || difference < least_difference)
                {
                    result = MatchType.ClosestMatch;
                    name = system_colour.Name;
                    least_difference = difference;
                }
            }

            return result;
        }

        public void cb_RemoveURLParameters_CheckedChanged(object sender, EventArgs e)
        {
            SaveConfiguration();
        }

        public void cb_ResolvePageLinks_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_ResolvePageLinks.Checked)
            {
                cb_ResolvePageMetaInfo.Checked = true;
            }

            SaveConfiguration();
        }

        public static void TextBox_SetPasswordVisibilty(
            TextBox textBox,
            bool passVisible)
        {
            textBox.UseSystemPasswordChar = passVisible;
        }

        public static string BuildUpConnectionString(EndpointDefinition endpointItem)
        {
            // BUILD-UP CONNECTION STRING
            string[] connectionData = endpointItem.ResponseAddress.Split(new string[] { Uri.SchemeDelimiter }, StringSplitOptions.None);

            string connectionString = connectionData[1];

            if (!string.IsNullOrEmpty(endpointItem.LoginName) &&
                endpointItem.LoginName != status_NotAvailable)
            {
                connectionString =
                    endpointItem.LoginName +
                    "@" +
                    connectionString;
            }

            return connectionData[0] + Uri.SchemeDelimiter + connectionString;
        }

        public void TIMER_ListAndLogsFilesWatcher_Tick(object sender, EventArgs e)
        {
            // ENDPOINTS LIST FILE
            mainMenu_EndpointsList.Enabled = File.Exists(endpointDefinitionsFile);

            // APP CONFIG FILE
            mainMenu_ConfigFile.Enabled = File.Exists(appConfigFile);
        }

        public void tb_ListFilter_TextChanged(object sender, EventArgs e)
        {
            ListEndpoints(ListViewRefreshMethod.CurrentState);

            RefreshTrayIcon();
        }

        public void pb_ListFilterClear_Click(object sender, EventArgs e)
        {
            pb_ListFilterClear.Visible = false;
            tb_ListFilter.BackColor = Color.LightGray;
            tb_ListFilter.Text = string.Empty;
        }

        public void lv_Endpoints_SelectedIndexChanged(object sender, EventArgs e)
        {
            // IF LIST ACTUALLY NOT REFRESHING AND FILTER NOT USED  
            if (!listUpdating && string.IsNullOrEmpty(tb_ListFilter.Text))
            {
                // GET SELECTED ITEM(S) INDEXES
                lv_Endpoints_SelectedItems.Clear();
                foreach (ListViewItem selectedItem in lv_Endpoints.SelectedItems)
                {
                    lv_Endpoints_SelectedItems.Add(selectedItem.Index);
                }

                // GET TOPITEM INDEX [TO PRESERVE SCROLLED POSITION]
                if (lv_Endpoints.TopItem != null)
                {
                    lv_Endpoints_TopItemIndex = lv_Endpoints.TopItem.Index;
                }
            }
        }

        public void CheckerMainForm_Shown(object sender, EventArgs e)
        {
            // LOAD VALIDATION METHOD TYPES AND SELECT DEFAULT [PROTOCOL]
            comboBox_Validate.DataSource = Enum.GetValues(typeof(ValidationMethod));
            comboBox_Validate.SelectedIndex = 0;

            RestoreListViewColumnsWidthAndOrder();
            RestoreVisibleEndpointColumns();
            RestoreWindowSizeAndPosition();
            LoadConfiguration();
            LoadEndpointReferences();
            ApplyBottomPanelsLayout();
        }

        private void CheckerMainForm_Resize(object sender, EventArgs e)
        {
            ApplyBottomPanelsLayout();
        }

        public void tray_SpeedTest_Click(object sender, EventArgs e)
        {
            if (mainMenu_SpeedTest.Enabled)
            {
                mainMenu_SpeedTest_Click(this, null);
            }
        }

        public void cb_PingHost_CheckedChanged(object sender, EventArgs e)
        {
            if (!cb_TestPing.Checked)
            {
                comboBox_Validate.SelectedIndex = 0;
            }

            SaveConfiguration();
        }

        public void cb_DNSLookupOnHost_CheckedChanged(object sender, EventArgs e)
        {
            SaveConfiguration();
        }

        public void TIMER_ContinuousRefresh_Tick(object sender, EventArgs e)
        {
            if (btn_RunCheck.Enabled &&
                dialog_SpeedTest == null &&
                dialog_EndpointDetails == null)
            {
                TIMER_ContinuousRefresh.Enabled = false;
                btn_RunCheck_Click(this, null);
            }
        }

        public void tray_Notifications_Enable_Click(object sender, EventArgs e)
        {
            cb_TrayBalloonNotify.Checked = true;
        }

        public void tray_Notifications_Disable_Click(object sender, EventArgs e)
        {
            cb_TrayBalloonNotify.Checked = false;
        }

        public void tray_CheckForUpdate_Click(object sender, EventArgs e)
        {
            mainMenu_UpdateCheck_Click(this, null);
        }

        public void cb_Resolve_IPAddresses_CheckedChanged(object sender, EventArgs e)
        {
            SaveConfiguration();
        }

        public void cb_Resolve_NIC_MACs_CheckedChanged(object sender, EventArgs e)
        {
            SaveConfiguration();
        }

        public void mainMenu_UpdateCheck_Click(object sender, EventArgs e)
        {
            mainMenu_UpdateCheck.Enabled = false;
            tray_CheckForUpdate.Enabled = false;

            NewBackgroundThread(() =>
            {
                CheckForUpdate();

                ThreadSafeInvoke(() =>
                {
                    if (app_AutoUpdateNow)
                    {
                        Close();
                    }
                    else if (!app_UpdateAvailable)
                    {
                        if (app_Version == app_LatestPackageVersion)
                        {
                            MessageBox.Show(
                            "You are using latest application version.",
                            "Check for Update",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show(
                            "There is no new build package available at this time.",
                            "Check for Update",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        }

                        mainMenu_UpdateCheck.Enabled = true;
                        tray_CheckForUpdate.Enabled = true;
                    }
                });
            });
        }

        public void mainMenu_HomePage_Click(object sender, EventArgs e)
        {
            BrowseEndpoint(
               "https://endpoint-status-checker.webnode.page",
               null,
               null,
               null);
        }

        public void mainMenu_FeatureRequest_Click(object sender, EventArgs e)
        {
            FeatureRequestDialog frDialog = new FeatureRequestDialog(
               new List<MailAddress> { report_Recipient });

            frDialog.ShowDialog();
        }

        public void mainMenu_EndpointsList_Click(object sender, EventArgs e)
        {
            using (EndpointManagementDialog dlg = new EndpointManagementDialog(this))
            {
                dlg.ShowDialog(this);
                if (dlg.FileWasModified)
                {
                    SetControls(false, true);
                    lbl_EndpointsListLoading.Visible = true;
                    lbl_ProgressCount.Visible = true;
                    lv_Endpoints.Visible = false;
                    LoadEndpointReferences();
                }
            }
        }

        public void mainMenu_SpeedTest_Click(object sender, EventArgs e)
        {
            tray_RunCheck.Visible = false;
            tray_SpeedTest.Visible = false;
            tray_CheckForUpdate.Visible = false;
            tray_Separator_1.Visible = false;
            tray_Separator_3.Visible = false;

            dialog_SpeedTest = new SpeedTestDialog();
            dialog_SpeedTest.ShowDialog();
            dialog_SpeedTest = null;

            tray_RunCheck.Visible = true;
            tray_SpeedTest.Visible = true;
            tray_CheckForUpdate.Visible = true;
            tray_Separator_1.Visible = true;
            tray_Separator_3.Visible = true;
        }

        public void mainMenu_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public void mainMenu_ConfigFile_Click(object sender, EventArgs e)
        {
            using (ConfigDialog dlg = new ConfigDialog(this))
            {
                dlg.ShowDialog(this);
            }
        }

        public void mainMenu_CfBypass_Click(object sender, EventArgs e)
        {
            using (CloudflareBypassSettingsDialog dlg = new CloudflareBypassSettingsDialog())
            {
                dlg.ShowDialog(this);
            }
        }

        // ── Premium visual theme ──────────────────────────────────────────────────

        private void ApplyPremiumTheme()
        {
            // ── Palette ──────────────────────────────────────────────────────────
            Color bg          = Color.FromArgb(  8,  14,  27);
            Color surface     = Color.FromArgb( 16,  26,  43);
            Color surfaceAlt  = Color.FromArgb( 23,  36,  61);
            Color input       = Color.FromArgb( 12,  21,  36);
            Color accent      = Color.FromArgb( 70, 143, 255);
            Color accentAlt   = Color.FromArgb( 42,  96, 214);
            Color success     = Color.FromArgb( 59, 218, 128);
            Color warning     = Color.FromArgb(255, 183,  67);
            Color danger      = Color.FromArgb(255,  99, 107);
            Color textMain    = Color.FromArgb(230, 238, 252);
            Color textMute    = Color.FromArgb(142, 158, 190);
            Color menuBg      = Color.FromArgb(  7,  13,  25);
            Color cardBorder  = Color.FromArgb(55, 77, 122);

            // Form
            BackColor = bg;
            MinimumSize = new Size(Math.Max(MinimumSize.Width, 1150), Math.Max(MinimumSize.Height, 740));

            // Menu strip
            MainMenuStrip.BackColor = menuBg;
            MainMenuStrip.ForeColor = textMain;
            MainMenuStrip.Renderer  = new DarkMenuStripRenderer();
            MainMenuStrip.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            MainMenuStrip.ImageScalingSize = new Size(18, 18);
            MainMenuStrip.Padding = new Padding(8, 5, 8, 5);
            foreach (ToolStripItem item in MainMenuStrip.Items)
            {
                item.BackColor = menuBg;
                item.ForeColor = textMain;

                if (item is ToolStripDropDownItem dropDownItem)
                {
                    ApplyDropDownTheme(dropDownItem, menuBg, textMain);
                }
            }

            // Walk every descendant control
            foreach (Control ctrl in DescendantControls(this))
            {
                switch (ctrl)
                {
                    case TabPage tabPage:
                        tabPage.BackColor = surface;
                        tabPage.ForeColor = textMain;
                        break;

                    case TabControl tabControl:
                        tabControl.BackColor = surface;
                        tabControl.ForeColor = textMain;
                        break;

                    case Panel panel:
                        panel.BackColor = bg;
                        break;

                    case GroupBox gb:
                        gb.BackColor = surface;
                        gb.ForeColor = accent;
                        gb.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold, GraphicsUnit.Point);
                        gb.Padding = new Padding(10, 22, 10, 10);
                        break;

                    case Label lbl when lbl.Name != "lbl_EndpointsListLoading":
                        lbl.ForeColor = (lbl == lbl_Copyright || lbl == lbl_Version)
                            ? textMute
                            : textMain;
                        break;

                    case CheckBox cb:
                        cb.UseVisualStyleBackColor = false;  // must be false or Windows ignores explicit colors
                        cb.BackColor = surface;
                        cb.ForeColor = textMain;
                        break;

                    case TextBox tb:
                        tb.BackColor = input;
                        tb.ForeColor = textMain;
                        tb.BorderStyle = BorderStyle.FixedSingle;
                        break;

                    case NumericUpDown nud:
                        nud.BackColor = input;
                        nud.ForeColor = textMain;
                        break;

                    case ComboBox cbo:
                        cbo.BackColor = input;
                        cbo.ForeColor = textMain;
                        break;

                    case ListView lv:
                        ConfigurePremiumListView(lv, surface, textMain);
                        break;

                    case ProgressBar progressBar:
                        progressBar.BackColor = surfaceAlt;
                        progressBar.ForeColor = accent;
                        break;

                    case RichTextBox richTextBox:
                        richTextBox.BackColor = input;
                        richTextBox.ForeColor = textMain;
                        richTextBox.BorderStyle = BorderStyle.None;
                        break;

                    case StatusStrip statusStrip:
                        statusStrip.BackColor = menuBg;
                        statusStrip.ForeColor = textMain;
                        break;

                    case ToolStrip toolStrip:
                        toolStrip.BackColor = menuBg;
                        toolStrip.ForeColor = textMain;
                        break;

                    case Button btn when btn.Image == null && btn.BackgroundImage == null:
                        btn.FlatStyle = FlatStyle.Flat;
                        btn.BackColor = input;
                        btn.ForeColor = textMain;
                        btn.FlatAppearance.BorderColor = accent;
                        break;
                }
            }

            // Explicit overrides for the main list
            ConfigurePremiumListView(lv_Endpoints, Color.FromArgb(13, 29, 38), textMain);

            // Status / action specific treatment to match dashboard styling
            foreach (Button btn in new[]
            {
                btn_LoadList,
                btn_BrowseExportDir,
                btn_CheckAll,
                btn_UncheckAll,
                btn_CheckAllAvailable,
                btn_CheckAllErrors,
                btn_RunCheck,
                btn_Terminate,
                btn_ColumnsChooser
            })
            {
                if (btn == null)
                {
                    continue;
                }

                StyleInlineCommandButton(btn);
            }

            ApplyPremiumActionIcons(accent, success, danger, textMute);
            WireCommandLabelsAsButtons();

            lbl_CheckAll.ForeColor = textMain;
            lbl_UncheckAll.ForeColor = textMute;
            lbl_CheckAllAvailable.ForeColor = success;
            lbl_CheckAllErrors.ForeColor = danger;
            lbl_RunCheck.ForeColor = accent;
            lbl_Terminate.ForeColor = textMute;
            lbl_BrowseExportDir.ForeColor = textMute;
            if (lbl_ColumnsChooser != null)
            {
                lbl_ColumnsChooser.ForeColor = Color.FromArgb(80, 200, 255);
                lbl_ColumnsChooser.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            }
            lbl_LastUpdate_Label.ForeColor = success;
            lbl_LastUpdate.ForeColor = textMain;

            pb_LastUpdate.BackColor = surface;
            pb_RefreshProcess.BackColor = Color.FromArgb(15, 18, 38);

            // Right-click context menus — DescendantControls() does not reach components,
            // so they must be styled explicitly.
            foreach (ContextMenuStrip cms in new[] { lv_Endpoints_ContextMenuStrip, trayContextMenu })
            {
                cms.BackColor = menuBg;
                cms.ForeColor = textMain;
                cms.Renderer  = new DarkMenuStripRenderer();
                cms.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
                cms.ImageScalingSize = new Size(18, 18);
                foreach (ToolStripItem item in cms.Items)
                {
                    item.BackColor = menuBg;
                    item.ForeColor = textMain;

                    if (item is ToolStripDropDownItem dropDownItem)
                    {
                        ApplyDropDownTheme(dropDownItem, menuBg, textMain);
                    }
                }
            }

            lbl_EndpointsListLoading.BackColor = surfaceAlt;
            lbl_EndpointsListLoading.ForeColor = warning;
            lbl_ProgressCount.ForeColor = accent;
            lbl_ProgressCount.Font = new Font("Segoe UI Semibold", 15F, FontStyle.Bold, GraphicsUnit.Point);
            pb_LastUpdate.ForeColor = success;
            pb_RefreshProcess.ForeColor = accent;

            StyleGroupBoxAsCard(groupBox_Actions, surface, cardBorder, Color.FromArgb(96, 169, 255));
            StyleGroupBoxAsCard(groupBox_EndpointSelection, surface, cardBorder, Color.FromArgb(88, 225, 150));
            StyleGroupBoxAsCard(groupBox_Export, surface, cardBorder, Color.FromArgb(121, 151, 255));
            StyleGroupBoxAsCard(groupBox_ScanProgress, surface, cardBorder, Color.FromArgb(92, 219, 255));
            StyleGroupBoxAsCard(groupBox_ListOptions, surface, cardBorder, Color.FromArgb(80, 200, 255));
            StyleGroupBoxAsCard(groupBox_CommonOptions, surface, cardBorder, Color.FromArgb(122, 231, 166));
            StyleGroupBoxAsCard(groupBox_HTTPOptions, surface, cardBorder, Color.FromArgb(255, 197, 101));

            ApplyPremiumControlState(inProgress: false, locked: false);
        }

        private static void StyleInlineCommandButton(Button button)
        {
            if (button == null)
            {
                return;
            }

            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(24, 39, 64);
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(30, 52, 84);
            button.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
            button.ForeColor = Color.FromArgb(230, 238, 252);
            button.BackColor = Color.FromArgb(16, 26, 43);
        }

        private void InitializeEndpointListOptions()
        {
            if (groupBox_ListOptions == null)
            {
                groupBox_ListOptions = new GroupBox
                {
                    Name = "groupBox_ListOptions",
                    Text = "List Options",
                    TabStop = false
                };
                Controls.Add(groupBox_ListOptions);
                groupBox_ListOptions.BringToFront();

                foreach (Control control in new Control[]
                {
                    lbl_ListFilter,
                    tb_ListFilter,
                    pb_ListFilterClear,
                    lbl_AutomaticRefresh,
                    num_RefreshInterval,
                    lbl_TimerIntervalMinutesText,
                    lbl_PingTimeout,
                    num_PingTimeout,
                    lbl_PingTimeoutSecondsText,
                    lbl_RequestTimeout,
                    num_HTTPRequestTimeout,
                    lbl_RequestTimeoutSecondsText,
                    lbl_FTPRequestTimeout,
                    num_FTPRequestTimeout,
                    lbl_FTPRequestTimeoutSecondsText,
                    lbl_ParallelThreadsCount,
                    num_ParallelThreadsCount,
                    lbl_Validate,
                    comboBox_Validate
                })
                {
                    groupBox_ListOptions.Controls.Add(control);
                }

                StyleGroupBoxAsCard(
                    groupBox_ListOptions,
                    Color.FromArgb(16, 26, 43),
                    Color.FromArgb(61, 84, 125),
                    Color.FromArgb(80, 200, 255));
            }

            InitializeEndpointColumnChooser();
        }

        private void InitializeEndpointColumnChooser()
        {
            endpointColumnsContextMenu = new ContextMenuStrip
            {
                Name = "endpointColumnsContextMenu",
                BackColor = Color.FromArgb(10, 18, 31),
                ForeColor = Color.FromArgb(230, 238, 252),
                Renderer = new DarkMenuStripRenderer()
            };

            foreach (ColumnHeader column in GetUserEndpointColumns())
            {
                if (!endpointColumnWidths.ContainsKey(column))
                {
                    endpointColumnWidths[column] = Math.Max(40, column.Width);
                }

                ToolStripMenuItem menuItem = new ToolStripMenuItem
                {
                    Text = column.Text,
                    Checked = column.Width > 0,
                    CheckOnClick = true,
                    Tag = column
                };
                menuItem.CheckedChanged += EndpointColumnMenuItem_CheckedChanged;
                endpointColumnsContextMenu.Items.Add(menuItem);
            }

            if (btn_ColumnsChooser == null)
            {
                btn_ColumnsChooser = new Button
                {
                    Name = "btn_ColumnsChooser",
                    Cursor = Cursors.Hand,
                    Enabled = true,
                    TabStop = false
                };
                btn_ColumnsChooser.Click += (s, e) => ShowEndpointColumnChooser(btn_ColumnsChooser, new Point(0, btn_ColumnsChooser.Height + 2));
                groupBox_Export.Controls.Add(btn_ColumnsChooser);

                lbl_ColumnsChooser = new Label
                {
                    Name = "lbl_ColumnsChooser",
                    Text = "Columns",
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Cursor = Cursors.Hand,
                    Enabled = true
                };
                lbl_ColumnsChooser.Click += (s, e) => ShowEndpointColumnChooser(lbl_ColumnsChooser, new Point(0, lbl_ColumnsChooser.Height + 2));
                groupBox_Export.Controls.Add(lbl_ColumnsChooser);
                StyleInlineCommandButton(btn_ColumnsChooser);
                SetPremiumButtonIcon(btn_ColumnsChooser, CreateCommandIcon(CommandIcon.Columns, Color.FromArgb(80, 200, 255), 20), 20);
                lbl_ColumnsChooser.ForeColor = Color.FromArgb(80, 200, 255);
                lbl_ColumnsChooser.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);

                endpointHeaderCornerPatch = new Panel
                {
                    Name = "endpointHeaderCornerPatch",
                    BackColor = Color.FromArgb(13, 23, 38),
                    Visible = true
                };
                Controls.Add(endpointHeaderCornerPatch);
            }

            InitializeScanProgressSection();

            lv_Endpoints.MouseUp -= Lv_Endpoints_HeaderMouseUp;
            lv_Endpoints.MouseUp += Lv_Endpoints_HeaderMouseUp;
            lv_Endpoints.MouseDown -= Lv_Endpoints_HeaderMouseDown;
            lv_Endpoints.MouseDown += Lv_Endpoints_HeaderMouseDown;
            lv_Endpoints.ColumnWidthChanged -= Lv_Endpoints_ColumnWidthChanged;
            lv_Endpoints.ColumnWidthChanged += Lv_Endpoints_ColumnWidthChanged;
        }

        private void InitializeScanProgressSection()
        {
            if (groupBox_ScanProgress != null)
            {
                return;
            }

            groupBox_ScanProgress = new GroupBox
            {
                Name = "groupBox_ScanProgress",
                Text = "Scan Progress",
                TabStop = false
            };
            Controls.Add(groupBox_ScanProgress);

            groupBox_ScanProgress.Controls.Add(lbl_ProgressCount);
            groupBox_ScanProgress.Controls.Add(pb_RefreshProcess);
            groupBox_ScanProgress.Controls.Add(pb_LastUpdate);
            groupBox_ScanProgress.Controls.Add(lbl_LastUpdate_Label);
            groupBox_ScanProgress.Controls.Add(lbl_LastUpdate);

            StyleGroupBoxAsCard(
                groupBox_ScanProgress,
                Color.FromArgb(16, 26, 43),
                Color.FromArgb(61, 84, 125),
                Color.FromArgb(92, 219, 255));
        }

        private static void StyleGroupBoxAsCard(GroupBox groupBox, Color fill, Color border, Color accent)
        {
            if (groupBox == null)
            {
                return;
            }

            EnsureGroupBoxHeaderClearance(groupBox, 28);

            groupBox.BackColor = fill;
            groupBox.ForeColor = accent;
            groupBox.FlatStyle = FlatStyle.Flat;
            groupBox.Paint -= PremiumGroupBoxPaint;
            groupBox.Paint += PremiumGroupBoxPaint;
            groupBox.Tag = new GroupBoxPaintTheme(fill, border, accent);
            groupBox.Invalidate();
        }

        private static void ConfigurePremiumListView(
            ListView listView,
            Color backColor,
            Color textColor)
        {
            if (listView == null)
            {
                return;
            }

            listView.BackColor = backColor;
            listView.ForeColor = textColor;
            listView.BorderStyle = BorderStyle.FixedSingle;
            listView.HeaderStyle = ColumnHeaderStyle.Clickable;
            listView.GridLines = false;
            listView.HideSelection = false;
            listView.FullRowSelect = true;
            listView.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            listView.OwnerDraw = true;
            listView.DrawColumnHeader -= PremiumListView_DrawColumnHeader;
            listView.DrawItem -= PremiumListView_DrawItem;
            listView.DrawSubItem -= PremiumListView_DrawSubItem;
            listView.DrawColumnHeader += PremiumListView_DrawColumnHeader;
            listView.DrawItem += PremiumListView_DrawItem;
            listView.DrawSubItem += PremiumListView_DrawSubItem;

            try
            {
                typeof(Control)
                    .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
                    ?.SetValue(listView, true, null);
            }
            catch
            {
            }
        }

        private static void PremiumListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            Color headerBack = Color.FromArgb(13, 23, 38);
            Color headerLine = Color.FromArgb(67, 82, 106);
            Color headerText = Color.FromArgb(230, 238, 252);

            using (SolidBrush backBrush = new SolidBrush(headerBack))
            using (Pen linePen = new Pen(headerLine))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
                e.Graphics.DrawLine(linePen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
                e.Graphics.DrawLine(linePen, e.Bounds.Right - 1, e.Bounds.Top + 3, e.Bounds.Right - 1, e.Bounds.Bottom - 4);
            }

            Rectangle textBounds = new Rectangle(e.Bounds.Left + 7, e.Bounds.Top + 1, Math.Max(0, e.Bounds.Width - 10), e.Bounds.Height - 2);
            if (string.IsNullOrEmpty(e.Header.Text))
            {
                return;
            }

            using (Font headerFont = new Font("Segoe UI Semibold", 8.5F, FontStyle.Regular, GraphicsUnit.Point))
            {
                TextRenderer.DrawText(
                    e.Graphics,
                    e.Header.Text,
                    headerFont,
                    textBounds,
                    headerText,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            }
        }

        private static void PremiumListView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            // Subitems own all painting in Details view.
        }

        private static void PremiumListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            if (!(sender is ListView listView))
            {
                e.DrawDefault = true;
                return;
            }

            Color baseBack = e.Item.BackColor.IsEmpty ? listView.BackColor : e.Item.BackColor;
            Color selectedBack = Color.FromArgb(42, 82, 138);
            Color gridLine = Color.FromArgb(74, 88, 110);
            Color textColor = e.SubItem.ForeColor.IsEmpty || e.SubItem.ForeColor.ToArgb() == Color.Black.ToArgb()
                ? e.Item.ForeColor
                : e.SubItem.ForeColor;

            if (textColor.IsEmpty || textColor.ToArgb() == Color.Black.ToArgb())
            {
                textColor = listView.ForeColor;
            }

            Rectangle bounds = e.Bounds;
            bool selected = e.Item.Selected;

            using (SolidBrush backBrush = new SolidBrush(selected ? selectedBack : baseBack))
            using (Pen gridPen = new Pen(gridLine))
            {
                e.Graphics.FillRectangle(backBrush, bounds);
                e.Graphics.DrawLine(gridPen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
                e.Graphics.DrawLine(gridPen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom);
            }

            Rectangle textBounds = new Rectangle(bounds.Left + 7, bounds.Top + 1, Math.Max(0, bounds.Width - 10), bounds.Height - 2);

            if (e.ColumnIndex == 0)
            {
                int x = bounds.Left + 4;

                if (listView.CheckBoxes)
                {
                    System.Windows.Forms.VisualStyles.CheckBoxState state = e.Item.Checked
                        ? System.Windows.Forms.VisualStyles.CheckBoxState.CheckedNormal
                        : System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedNormal;
                    Size checkboxSize = CheckBoxRenderer.GetGlyphSize(e.Graphics, state);
                    Point checkboxPoint = new Point(x, bounds.Top + (bounds.Height - checkboxSize.Height) / 2);
                    CheckBoxRenderer.DrawCheckBox(e.Graphics, checkboxPoint, state);
                    x += checkboxSize.Width + 5;
                }

                if (listView.SmallImageList != null &&
                    e.Item.ImageIndex >= 0 &&
                    e.Item.ImageIndex < listView.SmallImageList.Images.Count)
                {
                    Image image = listView.SmallImageList.Images[e.Item.ImageIndex];
                    int imageY = bounds.Top + (bounds.Height - image.Height) / 2;
                    e.Graphics.DrawImage(image, new Rectangle(x, imageY, image.Width, image.Height));
                    x += image.Width + 5;
                }

                textBounds = new Rectangle(x, bounds.Top + 1, Math.Max(0, bounds.Right - x - 6), bounds.Height - 2);
            }

            TextRenderer.DrawText(
                e.Graphics,
                e.SubItem.Text,
                listView.Font,
                textBounds,
                selected ? Color.White : textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
        }

        private void ApplyPremiumControlState(bool inProgress, bool locked)
        {
            Color textMain = Color.FromArgb(230, 238, 252);
            Color textMute = Color.FromArgb(142, 158, 190);
            Color surface = Color.FromArgb(16, 26, 43);
            Color input = Color.FromArgb(12, 21, 36);
            Color accent = Color.FromArgb(70, 143, 255);
            Color success = Color.FromArgb(59, 218, 128);
            Color danger = Color.FromArgb(255, 99, 107);

            foreach (GroupBox groupBox in new[] { groupBox_CommonOptions, groupBox_HTTPOptions, groupBox_Export, groupBox_EndpointSelection, groupBox_Actions, groupBox_ListOptions, groupBox_ScanProgress })
            {
                if (groupBox == null)
                {
                    continue;
                }

                groupBox.Enabled = true;
            }

            foreach (Label label in new[]
            {
                lbl_ListFilter,
                lbl_AutomaticRefresh,
                lbl_PingTimeout,
                lbl_PingTimeoutSecondsText,
                lbl_RequestTimeout,
                lbl_RequestTimeoutSecondsText,
                lbl_FTPRequestTimeout,
                lbl_FTPRequestTimeoutSecondsText,
                lbl_TimerIntervalMinutesText,
                lbl_ParallelThreadsCount,
                lbl_Validate,
                lbl_BrowseExportDir
            })
            {
                label.Enabled = true;
                label.ForeColor = textMute;
            }

            lbl_CheckAll.Enabled = true;
            lbl_UncheckAll.Enabled = true;
            lbl_CheckAllAvailable.Enabled = true;
            lbl_CheckAllErrors.Enabled = true;
            lbl_LoadList.Enabled = true;
            lbl_RunCheck.Enabled = true;
            lbl_Terminate.Enabled = true;

            lbl_CheckAll.ForeColor = textMain;
            lbl_UncheckAll.ForeColor = textMute;
            lbl_CheckAllAvailable.ForeColor = success;
            lbl_CheckAllErrors.ForeColor = danger;
            lbl_LoadList.ForeColor = textMain;
            lbl_RunCheck.ForeColor = !inProgress && !locked ? accent : textMute;
            lbl_Terminate.ForeColor = inProgress && locked ? danger : textMute;

            foreach (CheckBox checkBox in new[]
            {
                cb_RefreshOnStartup,
                cb_AutomaticRefresh,
                cb_ContinuousRefresh,
                cb_TrayBalloonNotify,
                cb_RefreshAutoSet,
                cb_ResolveNetworkShares,
                cb_TestPing,
                cb_Resolve_IPAddresses,
                cb_AllowAutoRedirect,
                cb_ValidateSSLCertificate,
                cb_RemoveURLParameters,
                cb_ResolvePageMetaInfo,
                cb_ResolvePageLinks,
                cb_SaveResponse,
                cb_Resolve_DNS_Names,
                cb_Resolve_NIC_MACs,
                cb_ExportEndpointsStatus_XLSX,
                cb_ExportEndpointsStatus_XML,
                cb_ExportEndpointsStatus_HTML,
                cb_ExportEndpointsStatus_JSON
            })
            {
                checkBox.BackColor = surface;
                checkBox.ForeColor = checkBox.Enabled ? textMain : textMute;
            }

            foreach (Control inputControl in new Control[] { tb_ListFilter, num_RefreshInterval, num_PingTimeout, num_HTTPRequestTimeout, num_FTPRequestTimeout, num_ParallelThreadsCount, comboBox_Validate })
            {
                inputControl.BackColor = input;
                inputControl.ForeColor = inputControl.Enabled ? textMain : textMute;
            }
        }

        private void TryUseDarkTitleBar()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return;
            }

            try
            {
                int useDarkMode = 1;
                DwmSetWindowAttribute(Handle, 20, ref useDarkMode, sizeof(int));
                DwmSetWindowAttribute(Handle, 19, ref useDarkMode, sizeof(int));
            }
            catch
            {
            }
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private void ApplyPremiumActionIcons(Color accent, Color success, Color danger, Color muted)
        {
            SetPremiumButtonIcon(btn_LoadList, CreateCommandIcon(CommandIcon.Refresh, success, 22), 22);
            SetPremiumButtonIcon(btn_RunCheck, CreateCommandIcon(CommandIcon.Play, accent, 22), 22);
            SetPremiumButtonIcon(btn_Terminate, CreateCommandIcon(CommandIcon.Stop, danger, 22), 22);
            SetPremiumButtonIcon(btn_BrowseExportDir, CreateCommandIcon(CommandIcon.Folder, Color.FromArgb(255, 197, 101), 20), 20);
            SetPremiumButtonIcon(btn_CheckAll, CreateCommandIcon(CommandIcon.Plus, accent, 22), 22);
            SetPremiumButtonIcon(btn_UncheckAll, CreateCommandIcon(CommandIcon.Minus, muted, 22), 22);
            SetPremiumButtonIcon(btn_CheckAllAvailable, CreateCommandIcon(CommandIcon.Check, success, 22), 22);
            SetPremiumButtonIcon(btn_CheckAllErrors, CreateCommandIcon(CommandIcon.X, danger, 22), 22);
            SetPremiumButtonIcon(btn_ColumnsChooser, CreateCommandIcon(CommandIcon.Columns, Color.FromArgb(80, 200, 255), 20), 20);
        }

        private enum CommandIcon
        {
            Refresh,
            Play,
            Stop,
            Folder,
            Plus,
            Minus,
            Check,
            X,
            Columns
        }

        private static Bitmap CreateCommandIcon(CommandIcon icon, Color color, int size)
        {
            Bitmap bitmap = new Bitmap(size, size);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            using (Pen pen = new Pen(color, Math.Max(2F, size / 10F)))
            using (SolidBrush brush = new SolidBrush(color))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                float pad = size * 0.22F;
                RectangleF r = new RectangleF(pad, pad, size - pad * 2, size - pad * 2);

                switch (icon)
                {
                    case CommandIcon.Refresh:
                        graphics.DrawArc(pen, r, 35, 285);
                        PointF a = new PointF(size * 0.74F, size * 0.20F);
                        graphics.FillPolygon(brush, new[]
                        {
                            a,
                            new PointF(a.X - size * 0.03F, a.Y + size * 0.22F),
                            new PointF(a.X - size * 0.20F, a.Y + size * 0.09F)
                        });
                        break;
                    case CommandIcon.Play:
                        graphics.FillPolygon(brush, new[]
                        {
                            new PointF(size * 0.34F, size * 0.24F),
                            new PointF(size * 0.34F, size * 0.76F),
                            new PointF(size * 0.76F, size * 0.50F)
                        });
                        break;
                    case CommandIcon.Stop:
                        graphics.FillRectangle(brush, size * 0.30F, size * 0.30F, size * 0.40F, size * 0.40F);
                        break;
                    case CommandIcon.Folder:
                        graphics.DrawLines(pen, new[]
                        {
                            new PointF(size * 0.16F, size * 0.72F),
                            new PointF(size * 0.16F, size * 0.30F),
                            new PointF(size * 0.42F, size * 0.30F),
                            new PointF(size * 0.50F, size * 0.40F),
                            new PointF(size * 0.84F, size * 0.40F),
                            new PointF(size * 0.84F, size * 0.72F),
                            new PointF(size * 0.16F, size * 0.72F)
                        });
                        break;
                    case CommandIcon.Plus:
                        graphics.DrawLine(pen, size * 0.50F, size * 0.24F, size * 0.50F, size * 0.76F);
                        graphics.DrawLine(pen, size * 0.24F, size * 0.50F, size * 0.76F, size * 0.50F);
                        break;
                    case CommandIcon.Minus:
                        graphics.DrawLine(pen, size * 0.24F, size * 0.50F, size * 0.76F, size * 0.50F);
                        break;
                    case CommandIcon.Check:
                        graphics.DrawLines(pen, new[]
                        {
                            new PointF(size * 0.20F, size * 0.52F),
                            new PointF(size * 0.42F, size * 0.72F),
                            new PointF(size * 0.80F, size * 0.28F)
                        });
                        break;
                    case CommandIcon.X:
                        graphics.DrawLine(pen, size * 0.28F, size * 0.28F, size * 0.72F, size * 0.72F);
                        graphics.DrawLine(pen, size * 0.72F, size * 0.28F, size * 0.28F, size * 0.72F);
                        break;
                    case CommandIcon.Columns:
                        graphics.DrawRectangle(pen, size * 0.18F, size * 0.24F, size * 0.64F, size * 0.52F);
                        graphics.DrawLine(pen, size * 0.40F, size * 0.25F, size * 0.40F, size * 0.75F);
                        graphics.DrawLine(pen, size * 0.60F, size * 0.25F, size * 0.60F, size * 0.75F);
                        break;
                }
            }

            return bitmap;
        }

        private static void SetPremiumButtonIcon(Button button, Image sourceImage, int iconSize)
        {
            if (button == null || sourceImage == null)
            {
                return;
            }

            button.Image = null;
            button.BackgroundImage = ResizeImage(sourceImage, iconSize, iconSize);
            button.BackgroundImageLayout = ImageLayout.Center;
            button.EnabledChanged -= PremiumIconButton_EnabledChanged;
            button.EnabledChanged += PremiumIconButton_EnabledChanged;
            button.Tag = button.BackgroundImage;
        }

        private static void PremiumIconButton_EnabledChanged(object sender, EventArgs e)
        {
            if (sender is Button button && button.Tag is Image image)
            {
                button.BackgroundImage = image;
            }
        }

        private void WireCommandLabelsAsButtons()
        {
            WireLabelButton(lbl_LoadList, btn_LoadList);
            WireLabelButton(lbl_RunCheck, btn_RunCheck);
            WireLabelButton(lbl_Terminate, btn_Terminate);
            WireLabelButton(lbl_CheckAll, btn_CheckAll);
            WireLabelButton(lbl_UncheckAll, btn_UncheckAll);
            WireLabelButton(lbl_CheckAllAvailable, btn_CheckAllAvailable);
            WireLabelButton(lbl_CheckAllErrors, btn_CheckAllErrors);
            WireLabelButton(lbl_BrowseExportDir, btn_BrowseExportDir);
        }

        private static void WireLabelButton(Label label, Button button)
        {
            if (label == null || button == null || label.Tag == button)
            {
                return;
            }

            label.Tag = button;
            label.Cursor = Cursors.Hand;
            label.Click += (s, e) =>
            {
                if (button.Enabled)
                {
                    button.PerformClick();
                }
            };
        }

        private static void EnsureGroupBoxHeaderClearance(GroupBox groupBox, int minimumContentTop)
        {
            if (groupBox.Controls.Count == 0)
            {
                return;
            }

            int minTop = int.MaxValue;
            foreach (Control control in groupBox.Controls)
            {
                minTop = Math.Min(minTop, control.Top);
            }

            if (minTop >= minimumContentTop)
            {
                return;
            }

            int delta = minimumContentTop - minTop;
            foreach (Control control in groupBox.Controls)
            {
                control.Top += delta;
            }

            groupBox.Height += delta;
        }

        private void ApplyBottomPanelsLayout()
        {
            if (lv_Endpoints == null ||
                groupBox_ListOptions == null ||
                groupBox_CommonOptions == null ||
                groupBox_HTTPOptions == null ||
                groupBox_EndpointSelection == null ||
                groupBox_Actions == null ||
                groupBox_Export == null ||
                groupBox_ScanProgress == null ||
                pb_RefreshProcess == null)
            {
                return;
            }

            const int margin = 12;
            const int gap = 10;
            const int footerHeight = 28;
            const int commandHeight = 76;
            const int bottomBandHeight = 188;

            int contentTop = MainMenuStrip.Bottom + 8;
            int commandTop = contentTop;
            int bottomBandTop = Math.Max(commandTop + commandHeight + 260, ClientSize.Height - footerHeight - bottomBandHeight);
            int listTop = commandTop + commandHeight + gap;
            int listBottom = bottomBandTop - gap;

            LayoutTopCommandRow(margin, commandTop, commandHeight);

            lv_Endpoints.Location = new Point(margin, listTop);
            lv_Endpoints.Size = new Size(Math.Max(420, ClientSize.Width - margin * 2), Math.Max(220, listBottom - listTop));
            FillEndpointListHeaderWidth();
            LayoutEndpointListOverlays();

            lbl_EndpointsListLoading.Location = lv_Endpoints.Location;
            lbl_EndpointsListLoading.Size = lv_Endpoints.Size;

            int leftWidth = Math.Min(560, Math.Max(500, (ClientSize.Width - margin * 2 - gap * 2) / 3));
            int middleX = margin + leftWidth + gap;
            int middleWidth = Math.Max(560, ClientSize.Width - margin - middleX);
            int optionWidth = Math.Max(220, (middleWidth - gap) / 2);

            groupBox_ListOptions.Location = new Point(margin, bottomBandTop);
            groupBox_ListOptions.Size = new Size(leftWidth, 166);
            LayoutFilterAndTimeoutControls(groupBox_ListOptions);

            groupBox_CommonOptions.Location = new Point(middleX, bottomBandTop);
            groupBox_CommonOptions.Size = new Size(optionWidth, 166);
            groupBox_HTTPOptions.Location = new Point(groupBox_CommonOptions.Right + gap, bottomBandTop);
            groupBox_HTTPOptions.Size = new Size(optionWidth, 166);
            LayoutOptionsGroup(groupBox_CommonOptions, new Control[]
            {
                cb_RefreshOnStartup,
                cb_ContinuousRefresh,
                cb_AutomaticRefresh,
                cb_RefreshAutoSet,
                cb_TrayBalloonNotify,
                cb_ResolveNetworkShares,
                cb_TestPing,
                cb_Resolve_IPAddresses
            });
            LayoutOptionsGroup(groupBox_HTTPOptions, new Control[]
            {
                cb_AllowAutoRedirect,
                cb_ValidateSSLCertificate,
                cb_RemoveURLParameters,
                cb_ResolvePageMetaInfo,
                cb_ResolvePageLinks,
                cb_SaveResponse,
                cb_Resolve_DNS_Names,
                cb_Resolve_NIC_MACs
            });

            lbl_Copyright.Location = new Point(margin, ClientSize.Height - footerHeight + 2);
            lbl_Version.Location = new Point(Math.Max(margin, ClientSize.Width - lbl_Version.Width - margin), ClientSize.Height - footerHeight + 2);
        }

        private void LayoutTopCommandRow(int margin, int top, int height)
        {
            const int gap = 10;
            int commandWidth = 374;
            int selectionWidth = 400;
            int exportWidth = 500;

            groupBox_Actions.Size = new Size(commandWidth, height);
            groupBox_EndpointSelection.Size = new Size(selectionWidth, height);
            groupBox_Actions.Location = new Point(margin, top);
            groupBox_EndpointSelection.Location = new Point(groupBox_Actions.Right + gap, top);

            int exportX = groupBox_EndpointSelection.Right + gap;
            groupBox_Export.Location = new Point(exportX, top);
            groupBox_Export.Size = new Size(exportWidth, height);

            int progressX = groupBox_Export.Right + gap;
            int progressWidth = ClientSize.Width - margin - progressX;
            groupBox_ScanProgress.Location = new Point(progressX, top);
            groupBox_ScanProgress.Size = new Size(Math.Max(0, progressWidth), height);
            groupBox_ScanProgress.Visible = progressWidth >= 260;

            LayoutActionsGroup();
            LayoutEndpointSelectionGroup();
            LayoutExportGroup();
            LayoutScanProgressGroup();
        }

        private void LayoutFilterAndTimeoutControls(GroupBox container)
        {
            const int outer = 12;
            const int labelWidth = 122;
            const int inputWidth = 60;
            const int rowHeight = 24;
            const int rowGap = 3;
            const int columnGap = 16;
            int columnWidth = Math.Max(220, (container.ClientSize.Width - outer * 2 - columnGap) / 2);

            Control[,] rows =
            {
                { lbl_AutomaticRefresh, num_RefreshInterval, lbl_TimerIntervalMinutesText },
                { lbl_PingTimeout, num_PingTimeout, lbl_PingTimeoutSecondsText },
                { lbl_RequestTimeout, num_HTTPRequestTimeout, lbl_RequestTimeoutSecondsText },
                { lbl_FTPRequestTimeout, num_FTPRequestTimeout, lbl_FTPRequestTimeoutSecondsText },
                { lbl_ParallelThreadsCount, num_ParallelThreadsCount, null },
                { lbl_Validate, comboBox_Validate, null }
            };

            int filterY = 36;
            lbl_ListFilter.AutoSize = false;
            lbl_ListFilter.Location = new Point(outer, filterY + 4);
            lbl_ListFilter.Size = new Size(labelWidth, 18);

            tb_ListFilter.Location = new Point(lbl_ListFilter.Right + 8, filterY);
            tb_ListFilter.Size = new Size(Math.Max(120, container.ClientSize.Width - tb_ListFilter.Left - outer - 28), 24);

            pb_ListFilterClear.Location = new Point(tb_ListFilter.Right + 4, filterY + 3);
            pb_ListFilterClear.Size = new Size(18, 18);

            int gridTop = filterY + rowHeight + 18;
            for (int i = 0; i < rows.GetLength(0); i++)
            {
                int column = i / 3;
                int row = i % 3;
                int columnX = outer + column * (columnWidth + columnGap);
                int rowY = gridTop + row * (rowHeight + rowGap);
                Control label = rows[i, 0];
                Control input = rows[i, 1];
                Control suffix = rows[i, 2];

                label.AutoSize = false;
                label.Location = new Point(columnX, rowY + 4);
                label.Size = new Size(labelWidth, 18);

                input.Location = new Point(columnX + labelWidth + 6, rowY);
                input.Size = input == comboBox_Validate || input == tb_ListFilter
                    ? new Size(Math.Min(124, Math.Max(90, columnX + columnWidth - input.Left)), 24)
                    : new Size(inputWidth, 24);

                if (suffix != null)
                {
                    suffix.Location = new Point(input.Right + 6, rowY + 4);
                    suffix.Size = suffix == pb_ListFilterClear ? new Size(18, 18) : new Size(70, 18);
                }
            }
        }

        private void LayoutExportGroup()
        {
            int x = 12;
            int y = 34;
            CheckBox[] exports = { cb_ExportEndpointsStatus_XLSX, cb_ExportEndpointsStatus_XML, cb_ExportEndpointsStatus_HTML, cb_ExportEndpointsStatus_JSON };
            foreach (CheckBox export in exports)
            {
                export.AutoSize = false;
                export.Location = new Point(x, y);
                export.Size = new Size(56, 18);
                x += 58;
            }

            lbl_BrowseExportDir.AutoSize = false;
            int folderX = x + 10;
            btn_BrowseExportDir.Location = new Point(folderX, 30);
            btn_BrowseExportDir.Size = new Size(24, 24);
            lbl_BrowseExportDir.Location = new Point(btn_BrowseExportDir.Right + 6, 34);
            lbl_BrowseExportDir.Size = new Size(96, 18);

            if (btn_ColumnsChooser != null && lbl_ColumnsChooser != null)
            {
                int columnsX = lbl_BrowseExportDir.Right + 14;
                btn_ColumnsChooser.Location = new Point(columnsX, 30);
                btn_ColumnsChooser.Size = new Size(24, 24);
                lbl_ColumnsChooser.Location = new Point(btn_ColumnsChooser.Right + 6, 34);
                lbl_ColumnsChooser.Size = new Size(72, 18);
            }
        }

        private void LayoutEndpointListOverlays()
        {
            if (endpointHeaderCornerPatch != null)
            {
                int patchWidth = SystemInformation.VerticalScrollBarWidth + 2;
                int patchHeight = Math.Max(22, lv_Endpoints.Font.Height + 10);
                endpointHeaderCornerPatch.Location = new Point(lv_Endpoints.Right - patchWidth - 1, lv_Endpoints.Top + 1);
                endpointHeaderCornerPatch.Size = new Size(patchWidth, patchHeight);
                endpointHeaderCornerPatch.BringToFront();
            }
        }

        private void LayoutScanProgressGroup()
        {
            if (groupBox_ScanProgress == null)
            {
                return;
            }

            int innerX = 12;
            int innerWidth = Math.Max(80, groupBox_ScanProgress.ClientSize.Width - innerX * 2);

            lbl_ProgressCount.TextAlign = ContentAlignment.MiddleLeft;
            lbl_ProgressCount.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            lbl_ProgressCount.BackColor = Color.Transparent;

            pb_LastUpdate.Location = new Point(innerX, 24);
            pb_LastUpdate.Size = new Size(18, 18);
            lbl_LastUpdate_Label.Location = new Point(pb_LastUpdate.Right + 6, 24);
            lbl_LastUpdate_Label.Size = new Size(112, 18);
            lbl_LastUpdate.Location = new Point(lbl_LastUpdate_Label.Right + 4, 24);
            lbl_LastUpdate.Size = new Size(Math.Max(90, innerWidth - (lbl_LastUpdate.Left - innerX)), 18);

            lbl_ProgressCount.Location = new Point(innerX, 43);
            lbl_ProgressCount.Size = new Size(innerWidth, 16);

            pb_RefreshProcess.Location = new Point(innerX, 61);
            pb_RefreshProcess.Size = new Size(innerWidth, 9);
            pb_RefreshProcess.BackColor = Color.FromArgb(13, 29, 38);
        }

        private void FillEndpointListHeaderWidth()
        {
            if (lv_Endpoints == null || lv_Endpoints.Columns.Count == 0)
            {
                return;
            }

            ColumnHeader lastVisibleColumn = GetUserEndpointColumns().LastOrDefault(column => column.Width > 0);
            if (lastVisibleColumn == null)
            {
                return;
            }

            int currentWidth = lastVisibleColumn.Width;
            int usedWidth = 0;
            foreach (ColumnHeader column in GetUserEndpointColumns())
            {
                if (column != lastVisibleColumn)
                {
                    usedWidth += column.Width;
                }
            }

            int availableWidth = lv_Endpoints.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 2;
            int stretchedWidth = Math.Max(currentWidth, availableWidth - usedWidth);

            if (lastVisibleColumn.Width == stretchedWidth)
            {
                return;
            }

            adjustingEndpointFillerColumn = true;
            lastVisibleColumn.Width = stretchedWidth;
            adjustingEndpointFillerColumn = false;
            lv_Endpoints.Invalidate();
        }

        private IEnumerable<ColumnHeader> GetUserEndpointColumns()
        {
            foreach (ColumnHeader column in lv_Endpoints.Columns)
            {
                yield return column;
            }
        }

        private void RestoreVisibleEndpointColumns()
        {
            string visibleColumns = Settings.Default.Config_VisibleColumns;
            if (string.IsNullOrWhiteSpace(visibleColumns))
            {
                return;
            }

            HashSet<string> visibleNames = new HashSet<string>(
                visibleColumns.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(item => item.Trim()),
                StringComparer.OrdinalIgnoreCase);

            if (visibleNames.Count == 0)
            {
                return;
            }

            foreach (ColumnHeader column in GetUserEndpointColumns())
            {
                if (!endpointColumnWidths.ContainsKey(column) && column.Width > 0)
                {
                    endpointColumnWidths[column] = column.Width;
                }

                if (!visibleNames.Contains(column.Name))
                {
                    column.Width = 0;
                }
            }
        }

        private void SaveVisibleEndpointColumns()
        {
            Settings.Default.Config_VisibleColumns = string.Join(
                ",",
                GetUserEndpointColumns()
                    .Where(column => column.Width > 0)
                    .Select(column => column.Name));
            Settings.Default.Save();
        }

        private void Lv_Endpoints_HeaderMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || !IsEndpointHeaderPoint(e.Location))
            {
                return;
            }

            ShowEndpointColumnChooser(e.Location);
        }

        private void Lv_Endpoints_HeaderMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && IsEndpointHeaderPoint(e.Location))
            {
                ShowEndpointColumnChooser(e.Location);
            }
        }

        private void CheckerMainForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || endpointColumnsContextMenu == null || lv_Endpoints == null)
            {
                return;
            }

            Rectangle headerBounds = new Rectangle(lv_Endpoints.Left, lv_Endpoints.Top, lv_Endpoints.Width, Math.Max(24, lv_Endpoints.Font.Height + 10));
            if (headerBounds.Contains(e.Location))
            {
                ShowEndpointColumnChooser(new Point(e.X - lv_Endpoints.Left, e.Y - lv_Endpoints.Top));
            }
        }

        private bool IsEndpointHeaderPoint(Point location)
        {
            int headerHeight = Math.Max(22, lv_Endpoints.Font.Height + 10);
            return location.Y >= 0 && location.Y <= headerHeight;
        }

        private void ShowEndpointColumnChooser(Point location)
        {
            ShowEndpointColumnChooser(lv_Endpoints, location);
        }

        private void ShowEndpointColumnChooser(Control owner, Point location)
        {
            foreach (ToolStripMenuItem item in endpointColumnsContextMenu.Items.OfType<ToolStripMenuItem>())
            {
                if (item.Tag is ColumnHeader column)
                {
                    item.CheckedChanged -= EndpointColumnMenuItem_CheckedChanged;
                    item.Checked = column.Width > 0;
                    item.CheckedChanged += EndpointColumnMenuItem_CheckedChanged;
                }
            }

            endpointColumnsContextMenu.Show(owner, location);
        }

        private void Lv_Endpoints_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (adjustingEndpointFillerColumn)
            {
                return;
            }

            if (e.ColumnIndex < 0 || e.ColumnIndex >= lv_Endpoints.Columns.Count)
            {
                return;
            }

            ColumnHeader column = lv_Endpoints.Columns[e.ColumnIndex];
            if (column.Width > 0)
            {
                endpointColumnWidths[column] = column.Width;
            }

            FillEndpointListHeaderWidth();
        }

        private void EndpointColumnMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (!(sender is ToolStripMenuItem item) || !(item.Tag is ColumnHeader column))
            {
                return;
            }

            if (!item.Checked && GetUserEndpointColumns().Count(c => c.Width > 0) <= 1)
            {
                item.CheckedChanged -= EndpointColumnMenuItem_CheckedChanged;
                item.Checked = true;
                item.CheckedChanged += EndpointColumnMenuItem_CheckedChanged;
                return;
            }

            if (item.Checked)
            {
                column.Width = endpointColumnWidths.TryGetValue(column, out int width)
                    ? Math.Max(40, width)
                    : 80;
            }
            else
            {
                if (column.Width > 0)
                {
                    endpointColumnWidths[column] = column.Width;
                }
                column.Width = 0;
            }

            FillEndpointListHeaderWidth();
            SaveVisibleEndpointColumns();
        }

        private void LayoutOptionsGroup(GroupBox groupBox, Control[] controls)
        {
            if (groupBox == null || controls == null)
            {
                return;
            }

            int columnGap = 16;
            int columnWidth = Math.Max(150, (groupBox.ClientSize.Width - 24 - columnGap) / 2);
            int rowHeight = 21;
            int startY = 36;

            for (int i = 0; i < controls.Length; i++)
            {
                Control control = controls[i];
                int column = i / 4;
                int row = i % 4;
                int x = 12 + column * (columnWidth + columnGap);
                int y = startY + row * rowHeight;

                control.AutoSize = false;
                control.Location = new Point(x, y);
                control.Size = new Size(columnWidth, 19);
            }
        }

        private void LayoutEndpointSelectionGroup()
        {
            const int groupInnerMargin = 10;
            const int buttonSize = 24;
            const int topOffset = 19;

            int availableWidth = groupBox_EndpointSelection.ClientSize.Width - groupInnerMargin * 2;
            int slotWidth = Math.Max(buttonSize + 6, availableWidth / 4);

            Button[] buttons = { btn_CheckAll, btn_UncheckAll, btn_CheckAllAvailable, btn_CheckAllErrors };
            Label[] labels = { lbl_CheckAll, lbl_UncheckAll, lbl_CheckAllAvailable, lbl_CheckAllErrors };

            for (int i = 0; i < buttons.Length; i++)
            {
                int slotX = groupInnerMargin + i * slotWidth;
                int buttonX = slotX + 2;

                buttons[i].Location = new Point(buttonX, topOffset);
                buttons[i].Size = new Size(buttonSize, buttonSize);

                labels[i].AutoSize = false;
                labels[i].TextAlign = ContentAlignment.MiddleLeft;
                labels[i].Location = new Point(buttonX + buttonSize + 4, topOffset + 2);
                labels[i].Size = new Size(Math.Max(34, slotWidth - buttonSize - 6), 18);
            }
        }

        private void LayoutActionsGroup()
        {
            const int groupInnerMargin = 10;
            const int buttonSize = 24;
            const int topOffset = 19;

            int availableWidth = groupBox_Actions.ClientSize.Width - groupInnerMargin * 2;
            int slotWidth = Math.Max(buttonSize + 10, availableWidth / 3);

            Button[] buttons = { btn_LoadList, btn_RunCheck, btn_Terminate };
            Label[] labels = { lbl_LoadList, lbl_RunCheck, lbl_Terminate };

            for (int i = 0; i < buttons.Length; i++)
            {
                int slotX = groupInnerMargin + i * slotWidth;
                int buttonX = slotX + 2;

                buttons[i].Location = new Point(buttonX, topOffset);
                buttons[i].Size = new Size(buttonSize, buttonSize);

                labels[i].AutoSize = false;
                labels[i].TextAlign = ContentAlignment.MiddleLeft;
                labels[i].Location = new Point(buttonX + buttonSize + 4, topOffset + 2);
                labels[i].Size = new Size(Math.Max(56, slotWidth - buttonSize - 6), 18);
            }
        }

        private static void PremiumGroupBoxPaint(object sender, PaintEventArgs e)
        {
            if (!(sender is GroupBox groupBox) || !(groupBox.Tag is GroupBoxPaintTheme theme))
            {
                return;
            }

            Rectangle bounds = new Rectangle(0, 0, groupBox.Width - 1, groupBox.Height - 1);
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                return;
            }

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (GraphicsPath path = RoundedRect(bounds, 8))
            using (LinearGradientBrush fillBrush = new LinearGradientBrush(bounds, ControlPaint.Light(theme.Fill, 0.05F), theme.Fill, LinearGradientMode.Vertical))
            using (Pen borderPen = new Pen(theme.Border, 1.1F))
            {
                e.Graphics.FillPath(fillBrush, path);
                e.Graphics.DrawPath(borderPen, path);
            }

            using (SolidBrush textBrush = new SolidBrush(theme.Accent))
            {
                e.Graphics.DrawString(groupBox.Text, groupBox.Font, textBrush, 16, 2);
            }
        }

        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static void ApplyDropDownTheme(ToolStripDropDownItem rootItem, Color background, Color foreground)
        {
            if (rootItem == null)
            {
                return;
            }

            foreach (ToolStripItem dropDownItem in rootItem.DropDownItems)
            {
                dropDownItem.BackColor = background;
                dropDownItem.ForeColor = foreground;

                if (dropDownItem is ToolStripDropDownItem nestedDropDown)
                {
                    ApplyDropDownTheme(nestedDropDown, background, foreground);
                }
            }
        }

        private void InitializePremiumMotionEffects()
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return;
            }

            premiumUiPulseTimer.Interval = 95;
            premiumUiPulseTimer.Tick -= PremiumUiPulseTimer_Tick;
            premiumUiPulseTimer.Tick += PremiumUiPulseTimer_Tick;
            premiumUiPulseTimer.Start();
        }

        private void PremiumUiPulseTimer_Tick(object sender, EventArgs e)
        {
            premiumUiPulseTick++;
            double wave = (Math.Sin(premiumUiPulseTick * 0.15) + 1d) / 2d;

            Color accentA = Color.FromArgb(74, 132, 255);
            Color accentB = Color.FromArgb(121, 170, 255);
            Color pulseAccent = BlendColor(accentA, accentB, wave);

            if (btn_RunCheck != null)
            {
                btn_RunCheck.FlatAppearance.BorderColor = pulseAccent;
            }

            if (btn_LoadList != null)
            {
                btn_LoadList.FlatAppearance.BorderColor = BlendColor(Color.FromArgb(70, 84, 118), accentA, wave * 0.7);
            }

            if (lbl_RunCheck != null)
            {
                lbl_RunCheck.ForeColor = pulseAccent;
            }

            if (lbl_ProgressCount != null)
            {
                lbl_ProgressCount.ForeColor = BlendColor(Color.FromArgb(128, 150, 196), pulseAccent, 0.65);
            }

            if (pb_RefreshProcess != null)
            {
                pb_RefreshProcess.ForeColor = pulseAccent;
            }
        }

        private static Color BlendColor(Color from, Color to, double amount)
        {
            double clamped = Math.Max(0d, Math.Min(1d, amount));
            int a = from.A + (int)((to.A - from.A) * clamped);
            int r = from.R + (int)((to.R - from.R) * clamped);
            int g = from.G + (int)((to.G - from.G) * clamped);
            int b = from.B + (int)((to.B - from.B) * clamped);
            return Color.FromArgb(a, r, g, b);
        }

        public static IEnumerable<Control> DescendantControls(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                yield return c;
                foreach (Control child in DescendantControls(c))
                    yield return child;
            }
        }

        public static void ApplyDarkTheme(Control root)
        {
            Color bg       = Color.FromArgb( 10,  16,  31);
            Color surface  = Color.FromArgb( 18,  27,  45);
            Color input    = Color.FromArgb( 14,  23,  39);
            Color accent   = Color.FromArgb( 74, 132, 255);
            Color textMain = Color.FromArgb(224, 233, 248);

            root.BackColor = bg;

            foreach (Control ctrl in DescendantControls(root))
            {
                switch (ctrl)
                {
                    case TabPage tp:
                        tp.BackColor = surface;
                        break;
                    case Panel p:
                        p.BackColor = bg;
                        break;
                    case TabControl tc:
                        tc.BackColor = surface;
                        break;
                    case GroupBox gb:
                        gb.BackColor = surface;
                        gb.ForeColor = accent;
                        break;
                    case Label lbl:
                        lbl.ForeColor = textMain;
                        break;
                    case CheckBox cb:
                        cb.UseVisualStyleBackColor = false;
                        cb.BackColor = surface;
                        cb.ForeColor = textMain;
                        break;
                    case TextBox tb:
                        tb.BackColor = input;
                        tb.ForeColor = textMain;
                        tb.BorderStyle = BorderStyle.FixedSingle;
                        break;
                    case NumericUpDown nud:
                        nud.BackColor = input;
                        nud.ForeColor = textMain;
                        break;
                    case ComboBox cbo:
                        cbo.BackColor = input;
                        cbo.ForeColor = textMain;
                        break;
                    case ListView lv:
                        lv.BackColor = surface;
                        lv.ForeColor = textMain;
                        break;
                    case DataGridView dgv:
                        dgv.BackgroundColor = surface;
                        dgv.ForeColor = textMain;
                        dgv.GridColor = accent;
                        dgv.DefaultCellStyle.BackColor = input;
                        dgv.DefaultCellStyle.ForeColor = textMain;
                        dgv.ColumnHeadersDefaultCellStyle.BackColor = surface;
                        dgv.ColumnHeadersDefaultCellStyle.ForeColor = accent;
                        dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(55, 75, 145);
                        dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(32, 37, 62);
                        dgv.AlternatingRowsDefaultCellStyle.ForeColor = textMain;
                        break;
                    case Button btn when btn.Image == null && btn.BackgroundImage == null:
                        btn.FlatStyle = FlatStyle.Flat;
                        btn.BackColor = input;
                        btn.ForeColor = textMain;
                        btn.FlatAppearance.BorderColor = accent;
                        break;
                }
            }
        }

        private sealed class DarkMenuStripRenderer : ToolStripProfessionalRenderer
        {
            public DarkMenuStripRenderer() : base(new DarkMenuColorTable()) { }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                Color bg = e.Item.Selected
                    ? Color.FromArgb(55, 75, 145)
                    : Color.FromArgb(12, 14, 30);
                using (SolidBrush brush = new SolidBrush(bg))
                    e.Graphics.FillRectangle(brush, new Rectangle(Point.Empty, e.Item.Size));
            }
        }

        private sealed class DarkMenuColorTable : ProfessionalColorTable
        {
            private static readonly Color _dark   = Color.FromArgb(12,  14,  30);
            private static readonly Color _hover  = Color.FromArgb(55,  75, 145);
            private static readonly Color _border = Color.FromArgb(65,  90, 170);

            public override Color MenuStripGradientBegin        => _dark;
            public override Color MenuStripGradientEnd          => _dark;
            public override Color MenuItemSelectedGradientBegin => _hover;
            public override Color MenuItemSelectedGradientEnd   => _hover;
            public override Color MenuItemSelected              => _hover;
            public override Color MenuItemBorder                => _border;
            public override Color MenuBorder                    => _border;
            public override Color ToolStripDropDownBackground   => _dark;
            public override Color ImageMarginGradientBegin      => _dark;
            public override Color ImageMarginGradientMiddle     => _dark;
            public override Color ImageMarginGradientEnd        => _dark;
            public override Color SeparatorDark                 => _border;
            public override Color SeparatorLight                => _hover;
        }

        public void btn_LoadList_Click(object sender, EventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                SetControls(false, true);
                lbl_EndpointsListLoading.Visible = true;
                lbl_ProgressCount.Visible = true;
                lv_Endpoints.Visible = false;
                LoadEndpointReferences();
            }
        }
    }

    public sealed class PremiumProgressBar : Panel
    {
        private int   _value   = 0;
        private int   _maximum = 100;
        private float _sweep   = -0.3f;
        private readonly System.Windows.Forms.Timer _anim;

        private static readonly Color ColBg     = Color.FromArgb( 15,  18,  38);
        private static readonly Color ColFillA  = Color.FromArgb( 38,  65, 175);
        private static readonly Color ColFillB  = Color.FromArgb( 88, 126, 232);
        private static readonly Color ColBorder = Color.FromArgb( 55,  85, 200);

        public PremiumProgressBar()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint  |
                     ControlStyles.UserPaint             |
                     ControlStyles.ResizeRedraw, true);
            BackColor = ColBg;
            _anim = new System.Windows.Forms.Timer { Interval = 16 };
            _anim.Tick += (_, __) => { _sweep += 0.008f; if (_sweep > 1.3f) _sweep = -0.3f; Invalidate(); };
        }

        public int Value   { get => _value;   set { _value   = Math.Clamp(value, 0, _maximum); Invalidate(); } }
        public int Maximum { get => _maximum; set { _maximum = Math.Max(1, value); Invalidate(); } }

        public void StartAnimation() { _sweep = -0.3f; _anim.Start(); }
        public void StopAnimation()  { _anim.Stop(); _value = 0; Invalidate(); }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Guard against degenerate control size during layout/animation ticks
            if (Width <= 2 || Height <= 2) return;

            var g = e.Graphics;

            using (var bg = new SolidBrush(ColBg))
                g.FillRectangle(bg, ClientRectangle);

            int barH  = Height - 2;
            int fillW = _maximum > 0
                ? Math.Max(0, (int)((double)_value / _maximum * (Width - 2)))
                : 0;

            if (fillW >= 1 && barH >= 1)
            {
                var fillRect = new Rectangle(1, 1, fillW, barH);

                // Main gradient fill
                try
                {
                    using (var lg = new System.Drawing.Drawing2D.LinearGradientBrush(
                        fillRect, ColFillA, ColFillB,
                        System.Drawing.Drawing2D.LinearGradientMode.Horizontal))
                        g.FillRectangle(lg, fillRect);
                }
                catch (ArgumentException) { /* skip on degenerate rect */ }

                // Highlight strip
                int hlH = Math.Max(1, barH / 3);
                try
                {
                    using (var hl = new System.Drawing.Drawing2D.LinearGradientBrush(
                        new Rectangle(1, 1, fillW, hlH),
                        Color.FromArgb(55, 255, 255, 255), Color.FromArgb(0, 255, 255, 255),
                        System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                        g.FillRectangle(hl, 1, 1, fillW, hlH);
                }
                catch (ArgumentException) { /* skip */ }

                // Shimmer sweep
                float cx = 1f + _sweep * fillW;
                float bw = fillW * 0.20f + 8f;
                var   pt1 = new System.Drawing.PointF(cx - bw, 0f);
                var   pt2 = new System.Drawing.PointF(cx + bw, 0f);

                try
                {
                    using (var sb = new System.Drawing.Drawing2D.LinearGradientBrush(
                        pt1, pt2,
                        Color.FromArgb(0, 200, 220, 255),
                        Color.FromArgb(60, 200, 220, 255)))
                    {
                        sb.WrapMode = System.Drawing.Drawing2D.WrapMode.Clamp;
                        sb.InterpolationColors = new System.Drawing.Drawing2D.ColorBlend(3)
                        {
                            Colors    = new[] { Color.FromArgb(  0, 200, 220, 255),
                                                Color.FromArgb( 60, 200, 220, 255),
                                                Color.FromArgb(  0, 200, 220, 255) },
                            Positions = new[] { 0f, 0.5f, 1f }
                        };
                        var state = g.Save();
                        g.SetClip(fillRect);
                        g.FillRectangle(sb, cx - bw, 1, bw * 2, barH);
                        g.Restore(state);
                    }
                }
                catch (ArgumentException) { /* skip shimmer on degenerate gradient */ }
            }

            using (var pen = new Pen(ColBorder, 1))
                g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _anim?.Dispose();
            base.Dispose(disposing);
        }
    }

    public class ListViewItemComparer : IComparer
    {
        private readonly int col;
        private readonly SortOrder order;
        public ListViewItemComparer()
        {
            col = 0;
            order = SortOrder.Ascending;
        }

        public ListViewItemComparer(int column, SortOrder order)
        {
            col = column;
            this.order = order;
        }

        public int Compare(object x, object y)
        {
            int returnVal = string.Compare(((ListViewItem)x).SubItems[col].Text,
                ((ListViewItem)y).SubItems[col].Text);

            if (order == SortOrder.Descending)
            {
                returnVal *= -1;
            }

            return returnVal;
        }
    }

    public sealed class GroupBoxPaintTheme
    {
        public Color Fill { get; }
        public Color Border { get; }
        public Color Accent { get; }

        public GroupBoxPaintTheme(Color fill, Color border, Color accent)
        {
            Fill = fill;
            Border = border;
            Accent = accent;
        }
    }

    public class EndpointDefinition
    {
        public string Name { get; set; }
        public string Protocol { get; set; }
        public string Port { get; set; }
        public string Address { get; set; }
        public string ResponseAddress { get; set; }
        public string[] IPAddress { get; set; }
        public string ResponseTime { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string PingRoundtripTime { get; set; }
        public string ServerID { get; set; }
        public string LoginName { get; set; }
        public string LoginPass { get; set; }
        public string[] NetworkShare { get; set; }
        public string[] DNSName { get; set; }
        public PropertyItems HTMLMetaInfo { get; set; }
        public string HTTPautoRedirects { get; set; }
        public string HTTPcontentType { get; set; }
        public string HTTPcontentLength { get; set; }
        public string HTTPexpires { get; set; }
        public string HTTPetag { get; set; }
        public Encoding HTTPencoding { get; set; }
        public Encoding HTMLencoding { get; set; }
        public Encoding HTMLdefaultStreamEncoding { get; set; }
        public string HTMLTitle { get; set; }
        public string HTMLAuthor { get; set; }
        public string HTMLDescription { get; set; }
        public string HTMLContentLanguage { get; set; }
        public Color HTMLThemeColor { get; set; }
        public PropertyItems HTTPRequestHeaders { get; set; }
        public PropertyItems HTTPResponseHeaders { get; set; }
        public string[] MACAddress { get; set; }
        public string FTPBannerMessage { get; set; }
        public string FTPWelcomeMessage { get; set; }
        public string FTPExitMessage { get; set; }
        public string FTPStatusDescription { get; set; }
        public string LastSeenOnline { get; set; }
        public PropertyItems SSLCertificateProperties { get; set; }
        public PropertyItems HTMLPageLinks { get; set; }
    }

    public class Property
    {
        public string ItemName { get; set; }
        public string ItemValue { get; set; }
    }

    public class PropertyItems
    {
        public List<Property> PropertyItem { get; set; }
    }

    public class NetworkConnection : IDisposable
    {
        private readonly string _networkName;

        public NetworkConnection(string networkName, NetworkCredential credentials)
        {
            _networkName = networkName;

            NetResource netResource = new NetResource()
            {
                Scope = ResourceScope.GlobalNetwork,
                ResourceType = ResourceType.Disk,
                DisplayType = ResourceDisplaytype.Share,
                RemoteName = networkName
            };

            string userName = string.IsNullOrEmpty(credentials.Domain)
                ? credentials.UserName
                : string.Format(@"{0}\{1}", credentials.Domain, credentials.UserName);

            int result = WNetAddConnection2(
                netResource,
                credentials.Password,
                userName,
                0);

            if (result != 0)
            {
                throw new IOException("Error connecting to remote share", result);
            }
        }

        ~NetworkConnection()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            WNetCancelConnection2(_networkName, 0, true);
        }

        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2(NetResource netResource,
            string password, string username, int flags);

        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2(string name, int flags,
            bool force);
    }

    [StructLayout(LayoutKind.Sequential)]
    public class NetResource
    {
        public ResourceScope Scope;
        public ResourceType ResourceType;
        public ResourceDisplaytype DisplayType;
        public int Usage;
        public string LocalName;
        public string RemoteName;
        public string Comment;
        public string Provider;
    }

    public enum ResourceScope : int
    {
        Connected = 1,
        GlobalNetwork,
        Remembered,
        Recent,
        Context
    };

    public enum ResourceType : int
    {
        Any = 0,
        Disk = 1,
        Print = 2,
        Reserved = 8,
    }

    public enum ResourceDisplaytype : int
    {
        Generic = 0x0,
        Domain = 0x01,
        Server = 0x02,
        Share = 0x03,
        File = 0x04,
        Group = 0x05,
        Network = 0x06,
        Root = 0x07,
        Shareadmin = 0x08,
        Directory = 0x09,
        Tree = 0x0a,
        Ndscontainer = 0x0b
    }

    public class IP_API_JSON_Response
    {
        [JsonProperty("status")]
        public string Service_Status { get; set; }

        [JsonProperty("country")]
        public string Country_Name { get; set; }

        [JsonProperty("countryCode")]
        public string Country_Code { get; set; }

        [JsonProperty("region")]
        public string Region_Code { get; set; }

        [JsonProperty("regionName")]
        public string Region_Name { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("zip")]
        public string City_ZIP_Code { get; set; }

        [JsonProperty("lat")]
        public string Geo_Lat { get; set; }

        [JsonProperty("lon")]
        public string Geo_Lon { get; set; }

        [JsonProperty("timezone")]
        public string TimeZone { get; set; }

        [JsonProperty("isp")]
        public string ISP { get; set; }

        [JsonProperty("org")]
        public string ISP_ORG { get; set; }

        [JsonProperty("as")]
        public string ISP_AS { get; set; }

        [JsonProperty("query")]
        public string Service_Query { get; set; }
    }
}
