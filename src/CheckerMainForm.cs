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
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
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

        // CUSTOM .NET FRAMEWORK VERSION ENUM
        public enum DotNetFramework_Version
        {
            v4_5,
            v4_5_1,
            v4_5_2,
            v4_6,
            v4_6_1,
            v4_6_2,
            v4_7,
            v4_7_1,
            v4_7_2,
            v4_8
        };

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

        // FOR MAC ADDRESS RESOLVER FEATURE PURPOSE
        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        public static extern int SendARP(int destIp, int srcIP, byte[] macAddr, ref uint physicalAddrLen);

        // THIS SWITCH INDICATES THAT TRAY ICON BALLOON TOOLTIP IS ACTUALLY DISPLAYED
        bool balloonVisible = false;

        // VARIABLES FOR TRAY ICON ANIMATION PURPOSES
        int trayAnimation_Index = 0;
        List<Icon> trayAnimation_Icons = new List<Icon>();

        // THIS SWITCH INDICATES THAT ENDPOINTS LISTVIEW IS ACTUALLY UPDATING
        bool listUpdating = false;

        // THIS SWITCH INDICATES ATTEMPT TO CLOSE APPLICATION DURING PROGRESS
        bool onClose = false;

        // VNC VIEWER EXECUTABLE [FOR 'FTP' CONNECTION PURPOSE]
        public static string appExecutable_VNC = string.Empty;

        // PUTTY EXECUTABLE [FOR 'SSH' CONNECTION PURPOSE]
        public static string appExecutable_Putty = string.Empty;

        // ENDPOINTS STATUS EXPORT FILES STREAMS [FOR EXCLUSIVE LOCK PURPOSE]
        FileStream definitonsStatusExport_JSON_FileStream = null;
        FileStream definitonsStatusExport_XLSX_FileStream = null;
        FileStream definitonsStatusExport_XML_FileStream = null;
        FileStream definitonsStatusExport_HTML_Info_FileStream = null;
        FileStream definitonsStatusExport_HTML_HTTP_FileStream = null;
        FileStream definitonsStatusExport_HTML_FTP_FileStream = null;

        // WORKING LIST OF ENDPOINTS
        List<EndpointDefinition> endpointsList = new List<EndpointDefinition>();

        // WORKING LIST OF DISABLED ENDPOINTS
        List<string> endpointsList_Disabled = new List<string>();

        // WORKING LIST OF 'LAST SEEN ONLINE' VALUES OF ENDPOINTS
        Dictionary<string, string> endpointsList_LastSeenOnline = new Dictionary<string, string>();

        // ENDPOINTS LISTVIEW TOPITEM INDEX [FOR PRESERVING SCROLLED POSITION AFTER LIST UPDATE]
        int lv_Endpoints_TopItemIndex = 0;

        // ENDPOINTS LISTVIEW SELECTED ITEM(S) INDEXES [FOR PRESERVING ITEM(S) SELECTION AFTER LIST UPDATE]
        List<int> lv_Endpoints_SelectedItems = new List<int>();

        // ENDPOINTS LISTVIEW SELECTED ITEM
        public static EndpointDefinition lv_Endpoints_SelectedEndpoint = null;

        // GET LOCAL GATEWAY IP AND MAC ADDRESSES
        public static List<string> localDNSAndGWIPAddresses;
        public static List<string> localDNSAndGWMACAddresses;

        // SELECTED VALIDATION METHOD
        public static ValidationMethod validationMethod;

        // INSTALLED .NET FRAMEWORK VERSION
        DotNetFramework_Version dotNetFramework_LatestInstalledVersion;

        // ENDPOINT DETAILS DIALOG INSTANCE
        EndpointDetailsDialog dialog_EndpointDetails = null;

        // SPEEDTEST DIALOG INSTANCE
        SpeedTestDialog dialog_SpeedTest = null;

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
            int minWorker, minIOC;
            ThreadPool.GetMinThreads(out minWorker, out minIOC);
            ThreadPool.SetMinThreads(100, minIOC);

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
            tray_Refresh.Image = ResizeImage(Resources.refresh.ToBitmap(), trayContextMenu.ImageScalingSize.Width, trayContextMenu.ImageScalingSize.Height);
            tray_SpeedTest.Image = ResizeImage(Resources.speedTest.ToBitmap(), trayContextMenu.ImageScalingSize.Width, trayContextMenu.ImageScalingSize.Height);
            tray_Exit.Image = ResizeImage(Resources.error.ToBitmap(), trayContextMenu.ImageScalingSize.Width, trayContextMenu.ImageScalingSize.Height);

            // SET VERSION / BUILD LABELS
            Text = app_Title;

            lbl_Copyright.Text = app_Copyright;
            lbl_Version.Text += "Version: " + app_VersionString +
                                ", Built: " + app_BuiltDate;

            // SET CONTROLS TOOLTIPS
            SetControlsTooltips();

            // CHECK .NET FRAMEWORK INSTALLED VERSION [FOR EXPORT INFORMATION PURPOSE]
            CheckDotNetFWKInstalledVersion();

            // LOAD 'LAST SEEN ONLINE' LIST
            RestoreLastSeenOnlineList();
        }

        public void SetControlsTooltips()
        {
            // SET TOOLTIP FOR APPLICATION WEB PAGE [WEBNODE] LINK LABEL
            ToolTip toolTip_AppWebPage = new ToolTip();
            toolTip_AppWebPage.ToolTipIcon = ToolTipIcon.Info;
            toolTip_AppWebPage.IsBalloon = true;
            toolTip_AppWebPage.ToolTipTitle = "WebNode";
            toolTip_AppWebPage.SetToolTip(pb_AppWebPage, "Open application web page.");

            // SET TOOLTIP FOR ITNETWORK LINK LABEL
            ToolTip toolTip_ITNetwork = new ToolTip();
            toolTip_ITNetwork.ToolTipIcon = ToolTipIcon.Info;
            toolTip_ITNetwork.IsBalloon = true;
            toolTip_ITNetwork.ToolTipTitle = "IT Network CZ";
            toolTip_ITNetwork.SetToolTip(pb_ITNetwork, "Open project page on IT Network CZ portal.");

            // SET TOOLTIP FOR GITHUB LINK LABEL
            ToolTip toolTip_GitHub = new ToolTip();
            toolTip_GitHub.ToolTipIcon = ToolTipIcon.Info;
            toolTip_GitHub.IsBalloon = true;
            toolTip_GitHub.ToolTipTitle = "GitHub";
            toolTip_GitHub.SetToolTip(pb_GitHub, "Open project repository on GitHub portal. Entire source code and releases are open for public.");

            // SET TOOLTIP FOR GITLAB LINK LABEL
            ToolTip toolTip_GitLab = new ToolTip();
            toolTip_GitLab.ToolTipIcon = ToolTipIcon.Info;
            toolTip_GitLab.IsBalloon = true;
            toolTip_GitLab.ToolTipTitle = "GitLab";
            toolTip_GitLab.SetToolTip(pb_GitLab, "Open project repository on GitLab portal. Entire source code and releases are open for public.");

            // SET TOOLTIP FOR ITNETWORK LINK LABEL
            ToolTip toolTip_SoftPedia = new ToolTip();
            toolTip_SoftPedia.ToolTipIcon = ToolTipIcon.Info;
            toolTip_SoftPedia.IsBalloon = true;
            toolTip_SoftPedia.ToolTipTitle = "SoftPedia";
            toolTip_SoftPedia.SetToolTip(pb_SoftPedia, "Open project page on SoftPedia portal.");

            // SET TOOLTIP FOR FEATURE REQUEST BUTTON
            ToolTip toolTip_FeatureRequest = new ToolTip();
            toolTip_FeatureRequest.ToolTipIcon = ToolTipIcon.Info;
            toolTip_FeatureRequest.IsBalloon = true;
            toolTip_FeatureRequest.ToolTipTitle = "Feature Request or Improvement";
            toolTip_FeatureRequest.SetToolTip(pb_FeatureRequest, "Send new Feature or Improvement description to development team");

            // SET TOOLTIP FOR 'ALL' ENDPOINTS SELECTION BUTTON
            ToolTip toolTip_EndpointSelection_All = new ToolTip();
            toolTip_EndpointSelection_All.ToolTipIcon = ToolTipIcon.Info;
            toolTip_EndpointSelection_All.IsBalloon = true;
            toolTip_EndpointSelection_All.ToolTipTitle = "Endpoints Selection";
            toolTip_EndpointSelection_All.SetToolTip(btn_CheckAll, "Select ALL EndPoints on list");

            // SET TOOLTIP FOR 'PASSED' ENDPOINTS SELECTION BUTTON
            ToolTip toolTip_EndpointSelection_Passed = new ToolTip();
            toolTip_EndpointSelection_Passed.ToolTipIcon = ToolTipIcon.Info;
            toolTip_EndpointSelection_Passed.IsBalloon = true;
            toolTip_EndpointSelection_Passed.ToolTipTitle = "Endpoints Selection";
            toolTip_EndpointSelection_Passed.SetToolTip(btn_CheckAllAvailable, "Select only previously PASSED EndPoints on list");

            // SET TOOLTIP FOR 'NONE' ENDPOINTS SELECTION BUTTON
            ToolTip toolTip_EndpointSelection_None = new ToolTip();
            toolTip_EndpointSelection_None.ToolTipIcon = ToolTipIcon.Info;
            toolTip_EndpointSelection_None.IsBalloon = true;
            toolTip_EndpointSelection_None.ToolTipTitle = "Endpoints Selection";
            toolTip_EndpointSelection_None.SetToolTip(btn_UncheckAll, "Deselect ALL EndPoints on list");

            // SET TOOLTIP FOR 'FAILED' ENDPOINTS SELECTION BUTTON
            ToolTip toolTip_EndpointSelection_Failed = new ToolTip();
            toolTip_EndpointSelection_Failed.ToolTipIcon = ToolTipIcon.Info;
            toolTip_EndpointSelection_Failed.IsBalloon = true;
            toolTip_EndpointSelection_Failed.ToolTipTitle = "Endpoints Selection";
            toolTip_EndpointSelection_Failed.SetToolTip(btn_CheckAllErrors, "Select only previously FAILED EndPoints on list");

            // SET TOOLTIP FOR 'REPORT OUTPUT FOLDER' BROWSE BUTTON
            ToolTip toolTip_BrowseReportOutputFolder = new ToolTip();
            toolTip_BrowseReportOutputFolder.IsBalloon = true;
            toolTip_BrowseReportOutputFolder.SetToolTip(btn_BrowseExportDir, "Browse for Report(s) output folder");

            // SET TOOLTIP FOR 'REFRESH' BUTTON
            ToolTip toolTip_Refresh = new ToolTip();
            toolTip_Refresh.IsBalloon = true;
            toolTip_Refresh.SetToolTip(btn_Refresh, "Refresh EndPoints list status");

            // SET TOOLTIP FOR 'TERMINATE' BUTTON
            ToolTip toolTip_Terminate = new ToolTip();
            toolTip_Terminate.IsBalloon = true;
            toolTip_Terminate.SetToolTip(btn_Terminate, "Terminate EndPoint status check process");

            // SET TOOLTIP FOR 'SPEEDTEST' BUTTON
            ToolTip toolTip_SpeedTest = new ToolTip();
            toolTip_SpeedTest.ToolTipIcon = ToolTipIcon.Info;
            toolTip_SpeedTest.IsBalloon = true;
            toolTip_SpeedTest.ToolTipTitle = "SpeedTest";
            toolTip_SpeedTest.SetToolTip(btn_SpeedTest, "Benchmark network Download / Upload speeds via OOKLA SpeedTest API");

            // SET TOOLTIP FOR 'ENDPOINTS LIST' FILE OPEN BUTTON
            ToolTip toolTip_OpenEndpointsListFile = new ToolTip();
            toolTip_OpenEndpointsListFile.ToolTipIcon = ToolTipIcon.Info;
            toolTip_OpenEndpointsListFile.IsBalloon = true;
            toolTip_OpenEndpointsListFile.ToolTipTitle = "Open EndPoints list file";
            toolTip_OpenEndpointsListFile.SetToolTip(btn_EndpointsList, "Open EndPoints list file (" + endpointDefinitonsFile + ") in default editor");

            // SET TOOLTIP FOR 'CONFIG' FILE OPEN BUTTON
            ToolTip toolTip_OpenAppConfigFile = new ToolTip();
            toolTip_OpenAppConfigFile.ToolTipIcon = ToolTipIcon.Info;
            toolTip_OpenAppConfigFile.IsBalloon = true;
            toolTip_OpenAppConfigFile.ToolTipTitle = "Open App config file";
            toolTip_OpenAppConfigFile.SetToolTip(btn_ConfigFile, "Open App configuration file (" + Path.GetFileName(appConfigFile) + ") in default editor");
        }

        public void CheckDotNetFWKInstalledVersion()
        {
            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
            {
                if (ndpKey != null && ndpKey.GetValue("Release") != null)
                {
                    int releaseKey = Convert.ToInt32(ndpKey.GetValue("Release"));

                    if (releaseKey >= 527934)
                    {
                        dotNetFramework_LatestInstalledVersion = DotNetFramework_Version.v4_8;
                    }
                    else if (releaseKey >= 461808)
                    {
                        dotNetFramework_LatestInstalledVersion = DotNetFramework_Version.v4_7_2;
                    }
                    else if (releaseKey >= 461308)
                    {
                        dotNetFramework_LatestInstalledVersion = DotNetFramework_Version.v4_7_1;
                    }
                    else if (releaseKey >= 460798)
                    {
                        dotNetFramework_LatestInstalledVersion = DotNetFramework_Version.v4_7;
                    }
                    else if (releaseKey >= 394802)
                    {
                        dotNetFramework_LatestInstalledVersion = DotNetFramework_Version.v4_6_2;
                    }
                    else if (releaseKey >= 394254)
                    {
                        dotNetFramework_LatestInstalledVersion = DotNetFramework_Version.v4_6_1;
                    }
                    else if (releaseKey >= 393295)
                    {
                        dotNetFramework_LatestInstalledVersion = DotNetFramework_Version.v4_6;
                    }
                    else if (releaseKey >= 379893)
                    {
                        dotNetFramework_LatestInstalledVersion = DotNetFramework_Version.v4_5_2;
                    }
                    else if (releaseKey >= 378675)
                    {
                        dotNetFramework_LatestInstalledVersion = DotNetFramework_Version.v4_5_1;
                    }
                    else
                    {
                        dotNetFramework_LatestInstalledVersion = DotNetFramework_Version.v4_5;
                    }
                }
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
                    cb_DNSAndMACLookupOnHost.Checked = Settings.Default.Config_DNSAndMACLookupOnHost;
                    cb_PingHost.Checked = Settings.Default.Config_PingHost;

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
            TIMER_AutomaticRefresh.Interval = ((int)num_RefreshInterval.Value * 60000);
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
                    Settings.Default.Config_DNSAndMACLookupOnHost = cb_DNSAndMACLookupOnHost.Checked;
                    Settings.Default.Config_PingHost = cb_PingHost.Checked;
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

                refreshedItem.SubItems.Add(
                    BuildUpConnectionString(
                        endpointItem,
                        new Uri(endpointItem.ResponseAddress).Scheme));

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
                refreshedItem.SubItems.Add(endpointItem.HTTPcontentLenght);
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
                refreshedItem.SubItems[16].Name = "HTTP Content Lenght";
                refreshedItem.SubItems[17].Name = "HTTP Expires";
                refreshedItem.SubItems[18].Name = "HTTP ETag";

                // SET BACKGROUND COLOR BY STATUS CODE
                refreshedItem.BackColor = GetColorByStatus(endpointItem.ResponseCode, endpointItem.PingRoundtripTime, endpointItem.ResponseMessage);

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

                if (refreshedItem.Text.ToLower().Contains(tb_ListFilter.Text.ToLower()))
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

                SetControls(false, false);

                btn_Refresh.Enabled = (lv_Endpoints.CheckedItems.Count > 0);
                lbl_NoEndpoints.Visible = false;
                lv_Endpoints.Visible = true;
            }
            else
            {
                SetControls(false, true);

                lbl_NoEndpoints.Visible = true;
                lv_Endpoints.Visible = false;
            }

            pb_Progress_Init.Visible = false;
            listUpdating = false;
        }

        public void bw_GetStatus_DoWork(object sender, DoWorkEventArgs e)
        {
            SetProgressStatus(0, 0, "Initializing Endpoints Status refresh ...", Color.DarkOrchid);

            // WORKING VARIABLES
            ConcurrentBag<EndpointDefinition> updatedEndpointsList = new ConcurrentBag<EndpointDefinition>();
            bool autoRedirect_Enable = cb_AllowAutoRedirect.Checked;
            bool validateSSLCertificate = cb_ValidateSSLCertificate.Checked;
            bool autoAdjustRefreshTimer = cb_RefreshAutoSet.Checked;
            bool resolveNetworkShares = cb_ResolveNetworkShares.Checked;
            bool resolvePageMetaInfo = cb_ResolvePageMetaInfo.Checked;
            bool removeURLParameters = cb_RemoveURLParameters.Checked;
            bool resolvePageLinks = cb_ResolvePageLinks.Checked;
            bool saveResponse = cb_SaveResponse.Checked;
            bool pingHost = cb_PingHost.Checked;
            bool dnsLookupOnHost = cb_DNSAndMACLookupOnHost.Checked;
            int threadsCount = (int)num_ParallelThreadsCount.Value;
            int pingTimeout = (int)num_PingTimeout.Value * 1000;
            int httpRequestTimeout = (int)num_HTTPRequestTimeout.Value * 1000;
            int ftpRequestTimeout = (int)num_FTPRequestTimeout.Value * 1000;

            int endpointsCount_Current = 0;
            int endpointsCount_Enabled =
                endpointsList.Where(
                    eItem =>
                             !endpointsList_Disabled.Contains(eItem.Name) &&
                             eItem.Name.ToLower().Contains(tb_ListFilter.Text.ToLower())).Count();

            // FLUSH LOCAL DNS CACHE
            DnsFlushResolverCache();

            // ALLOWED SECURITY PROTOCOLS
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)0x30 |
                                                   (SecurityProtocolType)0xC0 |
                                                   (SecurityProtocolType)0x300 |
                                                   (SecurityProtocolType)0xC00 |
                                                   (SecurityProtocolType)0x3000;

            ServicePointManager.Expect100Continue = true;

            if (validateSSLCertificate)
            {   // VALIDATE SERVER CERTIFICATE [HTTPS]
                ServicePointManager.ServerCertificateValidationCallback = null;
            }
            else
            {
                // BYPASS SERVER CERTIFICATE VALIDATION
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            }

            // ADJUST THREADS COUNT SETTING BY ENABLED ITEMS COUNT [IF LESS]
            if (endpointsCount_Enabled > 0 &&
                endpointsCount_Enabled < threadsCount)
            {
                threadsCount = endpointsCount_Enabled;
            }

            // STORE PROGRESS START DATE/TIME [FOR 'EXPORT' AND 'AUTO ADJUST REFRESH INTERVAL' PURPOSES]
            DateTime startDT_List = DateTime.Now;

            // EXECUTE PARALLEL PROCESS 
            Parallel.ForEach(
                endpointsList,
                new ParallelOptions { MaxDegreeOfParallelism = threadsCount },
                endpointItem =>
                {
                    if (!endpointItem.Name.ToLower().Contains(tb_ListFilter.Text.ToLower()))
                    {
                        // ADD CURRENT STATUS DEFINITION TO LIST
                        // [NOT VISIBLE ON THE LIST, IS FILTERED OUT]
                        updatedEndpointsList.Add(endpointItem);
                    }
                    else
                    {
                        // RESCAN ENDPOINT STATUS
                        Uri endpointURI = new Uri(endpointItem.Address);
                        Uri responseURI = new Uri(endpointItem.ResponseAddress);
                        string durationTime_Item = status_NotAvailable;

                        EndpointDefinition endpoint = new EndpointDefinition()
                        {
                            Name = endpointItem.Name,
                            Address = endpointItem.Address,
                            Protocol = endpointItem.Protocol,
                            Port = endpointItem.Port,
                            IPAddress = new string[] { status_NotAvailable },
                            DNSName = new string[] { status_NotAvailable },
                            ResponseTime = status_NotAvailable,
                            ResponseCode = status_NotAvailable,
                            ResponseMessage = GetEnumDescriptionString(EndpointStatus.NOTCHECKED),
                            LastSeenOnline = endpointItem.LastSeenOnline,
                            PingRoundtripTime = status_NotAvailable,
                            ServerID = status_NotAvailable,
                            LoginName = endpointItem.LoginName,
                            LoginPass = endpointItem.LoginPass,
                            NetworkShare = new string[] { status_NotAvailable },
                            HTMLMetaInfo = new PropertyItems() { PropertyItem = new List<Property>() },
                            HTTPautoRedirects = status_NotAvailable,
                            HTTPcontentType = status_NotAvailable,
                            HTTPencoding = null,
                            HTMLencoding = null,
                            HTMLTitle = status_NotAvailable,
                            HTMLAuthor = status_NotAvailable,
                            HTMLDescription = status_NotAvailable,
                            HTMLContentLanguage = status_NotAvailable,
                            HTMLThemeColor = Color.Empty,
                            HTMLPageLinks = new PropertyItems() { PropertyItem = new List<Property>() },
                            HTTPcontentLenght = status_NotAvailable,
                            HTTPexpires = status_NotAvailable,
                            HTTPetag = status_NotAvailable,
                            HTTPRequestHeaders = new PropertyItems() { PropertyItem = new List<Property>() },
                            HTTPResponseHeaders = new PropertyItems() { PropertyItem = new List<Property>() },
                            MACAddress = new string[] { status_NotAvailable },
                            FTPBannerMessage = status_NotAvailable,
                            FTPWelcomeMessage = status_NotAvailable,
                            FTPExitMessage = status_NotAvailable,
                            FTPStatusDescription = status_NotAvailable,
                            SSLCertificateProperties = new PropertyItems() { PropertyItem = new List<Property>() }
                        };

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
                                            httpRequestTimeout,
                                            autoRedirect_Enable,
                                            removeURLParameters);

                                        try
                                        {
                                            // TRY TO GET RESPONSE
                                            httpWebResponse = GetHTTPWebResponse(httpWebRequest, 3);

                                            // HANDLE POSSIBLE REDIRECT [3xx]
                                            if (autoRedirect_Enable && ((int)httpWebResponse.StatusCode).ToString().StartsWith("3"))
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
                                            HttpWebResponse _httpWebResponse = wEX.Response as HttpWebResponse;

                                            // IF RESULT CODE IS '3xx', DO A SECOND CALL ON 'LOCATION'
                                            if (autoRedirect_Enable &&
                                                _httpWebResponse != null &&
                                                ((int)_httpWebResponse.StatusCode).ToString().StartsWith("3") &&
                                                _httpWebResponse.Headers.AllKeys.Contains("Location") &&
                                                !string.IsNullOrEmpty(_httpWebResponse.GetResponseHeader("Location")))
                                            {
                                                // GET 'LOCATION' HEADER VALUE
                                                string locationHeaderValue = _httpWebResponse.GetResponseHeader("Location");

                                                // IF IS RELATIVE PATH
                                                if (Uri.IsWellFormedUriString(locationHeaderValue, UriKind.Relative))
                                                {
                                                    locationHeaderValue = Url.Combine(_httpWebResponse.ResponseUri.AbsoluteUri, locationHeaderValue);
                                                }

                                                // PREPARE WEBREQUEST
                                                HttpWebRequest httpWebRequest_Redirected = PrepareHTTPWebRequest(
                                                    endpoint,
                                                    new Uri(locationHeaderValue),
                                                    httpRequestTimeout,
                                                    autoRedirect_Enable,
                                                    removeURLParameters,
                                                    _httpWebResponse.Cookies);

                                                autoRedirect_Followed = true;

                                                // GET RESPONSE FROM 'LOCATION'
                                                httpWebResponse = GetHTTPWebResponse(httpWebRequest_Redirected, 3);
                                            }
                                            else
                                            {
                                                throw (wEX);
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
                                        if (autoRedirect_Enable)
                                        {
                                            FieldInfo fieldInfo = httpWebRequest.GetType().GetField("_AutoRedirects", BindingFlags.NonPublic | BindingFlags.Instance);
                                            int httpAutoRedirects = (int)fieldInfo.GetValue(httpWebRequest);
                                            endpoint.HTTPautoRedirects = httpAutoRedirects.ToString();

                                            // CHECK AUTO REDIRECT URL [COMPARE REQUEST AND RESPONSE ENDPOINT URIs]
                                            if (endpointURI.Scheme != responseURI.Scheme ||
                                                endpointURI.Port != responseURI.Port ||
                                                endpointURI.Host != responseURI.Host ||
                                                autoRedirect_Followed)
                                            {
                                                endpoint.ResponseMessage += " (Redirected from \"" + endpointURI.AbsoluteUri + "\")";
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

                                        // GET CONTENT LENGHT FROM RESPONSE HEADER
                                        long contentLength = httpWebResponse.ContentLength;

                                        if (!string.IsNullOrEmpty(httpWebResponse.Headers["Content-Length"]))
                                        {
                                            long.TryParse(httpWebResponse.Headers["Content-Length"], out contentLength);
                                        }

                                        GetWebResponseContentLenghtString(endpoint, contentLength);

                                        // TRY TO GET HEADER ENCODING FROM RESPONSE HEADER
                                        endpoint.HTTPencoding = GetEncoding(httpWebResponse.ContentType);

                                        if (saveResponse || resolvePageMetaInfo)
                                        {
                                            // GET RESPONSE STREAM
                                            using (BinaryReader httpWebResponseBinaryReader = new BinaryReader(httpWebResponse.GetResponseStream()))
                                            {
                                                MemoryStream httpWebResponseMemoryStream = new MemoryStream();

                                                byte[] httpWebResponseByteArray;
                                                byte[] httpWebResponseBuffer = httpWebResponseBinaryReader.ReadBytes(1024);
                                                while (httpWebResponseBuffer.Length > 0 && httpWebResponseMemoryStream.Length < (http_SaveResponse_MaxLenght_Bytes + 1024))
                                                {
                                                    httpWebResponseMemoryStream.Write(httpWebResponseBuffer, 0, httpWebResponseBuffer.Length);
                                                    httpWebResponseBuffer = httpWebResponseBinaryReader.ReadBytes(1024);
                                                }

                                                httpWebResponseByteArray = new byte[(int)httpWebResponseMemoryStream.Length];
                                                httpWebResponseMemoryStream.Position = 0;
                                                httpWebResponseMemoryStream.Read(httpWebResponseByteArray, 0, httpWebResponseByteArray.Length);

                                                // GET CONTENT LENGHT FROM FULL RESPONSE
                                                contentLength = httpWebResponseMemoryStream.Length;
                                                GetWebResponseContentLenghtString(endpoint, contentLength);

                                                if (saveResponse &&
                                                    !string.IsNullOrEmpty(endpoint.HTTPcontentType) &&
                                                    CheckWebResponseContentLenght(endpoint, httpWebResponse, contentLength))
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
                                                    if (resolvePageMetaInfo)
                                                    {
                                                        // READ RESPONSE STREAM [HTTP HEADER ENCODING] AND GET HTML META INFO
                                                        ResolvePageMetaInfo(ReadHTTPResponseStream(httpWebResponseMemoryStream, endpoint.HTTPencoding), endpoint, responseURI, resolvePageLinks);

                                                        // IF ENCODING NOT PRESENT IN HTTP HEADER, READ AGAIN WITH ENCODING FROM HTML META
                                                        if (endpoint.HTTPencoding == null)
                                                        {
                                                            // READ AGAIN WITH ENCODING FROM HTML META [IF PRESENT]
                                                            if (endpoint.HTMLencoding != null)
                                                            {
                                                                // READ RESPONSE STREAM [HML META ENCODING] AND GET HTML META INFO
                                                                ResolvePageMetaInfo(ReadHTTPResponseStream(httpWebResponseMemoryStream, endpoint.HTMLencoding), endpoint, responseURI, resolvePageLinks);
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
                                        }
                                        else
                                        {
                                            // EXCEPTION CODE
                                            endpoint.ResponseCode = status_Error;

                                            // EXCEPTION STATUS
                                            endpoint.ResponseMessage = webException.Status.ToString();
                                            endpoint.ResponseMessage += " -> " + webException.Message;

                                            // INNER EXCEPTION MESSAGE
                                            if (webException.InnerException != null &&
                                                !string.IsNullOrEmpty(webException.InnerException.Message) &&
                                                !endpoint.ResponseMessage.Contains(webException.InnerException.Message))
                                            {
                                                endpoint.ResponseMessage += " -> " + webException.InnerException.Message;
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
                                        ftpWebRequest = (FtpWebRequest)WebRequest.Create(endpointURI.AbsoluteUri);
                                        ftpWebRequest.Method = WebRequestMethods.Ftp.PrintWorkingDirectory;
                                        ftpWebRequest.Timeout = ftpRequestTimeout;
                                        ftpWebRequest.ReadWriteTimeout = ftpRequestTimeout;
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

                                // GET ITEM CHECK DURATION TIME [FOR 'EXPORT' PURPOSE]
                                if (validationMethod == ValidationMethod.Protocol &&
                                    !BW_GetStatus.CancellationPending)
                                {
                                    durationTime_Item = sw_ItemProgress.ElapsedMilliseconds.ToString() + " ms";
                                }

                                if (!BW_GetStatus.CancellationPending)
                                {
                                    try
                                    {
                                        // RESOLVE NETWORK SHARES
                                        if (resolveNetworkShares)
                                        {
                                            List<string> netSharesList = GetNetShares(responseURI.Host);
                                            if (netSharesList.Count > 0)
                                            {
                                                netSharesList.Sort();
                                                endpoint.NetworkShare = netSharesList.ToArray();
                                            }
                                        }

                                        // FILL UP 'IP ADDRESS' / 'DNS NAME' [FAST, BY REGEX]
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

                                        // DNS / MAC LOOKUP
                                        if (dnsLookupOnHost)
                                        {
                                            List<string> endpointIPAddressesStringList = new List<string>();
                                            List<string> endpointDNSNamesStringList = new List<string>();
                                            List<string> endpointMACAddressStringList = new List<string>();

                                            foreach (IPAddress endpointIPAddress in Dns.GetHostAddresses(responseURI.Host))
                                            {
                                                if (endpointIPAddress.AddressFamily == AddressFamily.InterNetwork)
                                                {
                                                    endpointIPAddressesStringList.Add(endpointIPAddress.ToString());

                                                    try
                                                    {
                                                        // RESOLVE DNS NAME(S)
                                                        IPHostEntry hostEntry = Dns.GetHostEntry(endpointIPAddress);
                                                        endpointDNSNamesStringList.Add(hostEntry.HostName);
                                                    }
                                                    catch
                                                    {
                                                    }

                                                    try
                                                    {
                                                        // RESOLVE MAC ADDRESS(ES)
                                                        string macAddress = GetMACAddress(endpointIPAddress);

                                                        // IF ENDPOINT IP ADDRESS IS NOT LOCAL OR
                                                        // RESOLVED MAC IS NOT MAC ADDRESS OF ANY DNS SERVER OR DEFAULT GATEWAY
                                                        if (!string.IsNullOrEmpty(macAddress) &&
                                                           (!localDNSAndGWMACAddresses.Contains(macAddress) ||
                                                            localDNSAndGWIPAddresses.Contains(endpointIPAddress.ToString())))
                                                        {
                                                            endpointMACAddressStringList.Add(macAddress);
                                                        }
                                                    }
                                                    catch
                                                    {
                                                    }
                                                }
                                            }

                                            // SORT IP ADDRESS(ES) LIST
                                            if (endpointIPAddressesStringList.Count > 0)
                                            {
                                                endpoint.IPAddress = endpointIPAddressesStringList.ToArray();
                                            }

                                            // SORT DNS NAME(S) LIST
                                            if (endpointDNSNamesStringList.Count > 0)
                                            {
                                                endpoint.DNSName = endpointDNSNamesStringList.ToArray();
                                            }
                                            else if (!Regex.IsMatch(responseURI.Host, @"^[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$"))
                                            {
                                                // ORIGINAL URL IS NOT IP ADDRESS, GET HOSTNAME
                                                endpoint.DNSName = new string[] { responseURI.Host };
                                            }

                                            // SORT MAC ADDRESS(ES) LIST
                                            if (endpointMACAddressStringList.Count > 0)
                                            {
                                                endpoint.MACAddress = endpointMACAddressStringList.ToArray();
                                            }
                                        }
                                    }
                                    catch
                                    {
                                    }
                                }

                                if (!BW_GetStatus.CancellationPending)
                                {
                                    // PING HOST
                                    try
                                    {
                                        if (validationMethod == ValidationMethod.Ping || pingHost)
                                        {
                                            if (validationMethod == ValidationMethod.Ping)
                                            {
                                                endpoint.ResponseMessage = GetEnumDescriptionString(EndpointStatus.PINGCHECK);
                                            }

                                            string pingRoundtripTime = GetPingTime(responseURI.Host, pingTimeout, 1);

                                            if (!string.IsNullOrEmpty(pingRoundtripTime))
                                            {
                                                endpoint.PingRoundtripTime = pingRoundtripTime;
                                            }
                                        }
                                    }
                                    catch
                                    {
                                    }
                                }

                                // SET PROGRESS STATUS LABEL
                                SetProgressStatus(endpointsCount_Enabled, endpointsCount_Current);
                            }
                        }

                        // UPDATE ADDRESSES
                        endpoint.Address = endpointURI.AbsoluteUri;
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
                                 !endpoint.ResponseCode.StartsWith("4") &&
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
                });

            // GET PROGRESS DURATION TIME [FOR 'EXPORT' AND 'AUTO ADJUST REFRESH INTERVAL' PURPOSES]
            DateTime endDT_List = DateTime.Now;
            int durationTime_List = (int)(endDT_List - startDT_List).TotalSeconds;

            if (autoAdjustRefreshTimer)
            {
                // ADJUST AUTO REFRESH INTERVAL BY LAST PROGRESS DURATION TIME (+ 1 MINUTE RESERVE]
                decimal durationTime_List_Minutes = (durationTime_List / 60000);
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
                                  (durationTime_List),
                                  (pingTimeout / 1000),
                                  (httpRequestTimeout / 1000),
                                  (ftpRequestTimeout / 1000),
                                  autoRedirect_Enable.ToString(),
                                  validateSSLCertificate.ToString(),
                                  threadsCount.ToString(),
                                  resolveNetworkShares.ToString(),
                                  resolvePageMetaInfo.ToString(),
                                  saveResponse.ToString(),
                                  pingHost.ToString(),
                                  dnsLookupOnHost.ToString()
                                  );

            // GARBAGE COLLECTOR
            GC.Collect();
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
            // REQUEST PARAMETERS
            string httpWebRequest_Method = WebRequestMethods.Http.Get;
            Version httpWebRequest_ProtocolVersion = HttpVersion.Version11;
            Guid httpWebRequest_ApplicationGUID = Guid.NewGuid();
            RequestCachePolicy httpWebRequest_CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
            DecompressionMethods httpWebRequest_DecompressionMethods =
                DecompressionMethods.GZip |
                DecompressionMethods.Deflate |
                DecompressionMethods.None;

            if (removeURLParameters)
            {
                // REMOVE URL PARAMETERS [IF ANY PRESENT]
                endpointURI.RemoveQuery();
            }

            // COOKIE CONTAINER
            CookieContainer httpWebRequest_CookieContainer = new CookieContainer(300);
            if (cookies != null)
            {
                httpWebRequest_CookieContainer.Add(cookies);
            }

            // CREATE REQUEST
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(endpointURI.AbsoluteUri);
            httpWebRequest.Method = httpWebRequest_Method;
            httpWebRequest.UserAgent = http_UserAgent;
            httpWebRequest.Accept = "*/*";
            httpWebRequest.Timeout = httpRequestTimeout;
            httpWebRequest.ReadWriteTimeout = httpRequestTimeout;
            httpWebRequest.AllowAutoRedirect = allowAutoRedirect;
            httpWebRequest.KeepAlive = true;
            httpWebRequest.CachePolicy = httpWebRequest_CachePolicy;
            httpWebRequest.CookieContainer = httpWebRequest_CookieContainer;
            httpWebRequest.AutomaticDecompression = httpWebRequest_DecompressionMethods;
            httpWebRequest.ProtocolVersion = httpWebRequest_ProtocolVersion;
            httpWebRequest.Date = DateTime.Now;
            httpWebRequest.MaximumAutomaticRedirections = 20;
            httpWebRequest.Referer = endpointURI.Host;

            // CUSTOM HEADERS
            WebHeaderCollection requestHeadersCollection = new WebHeaderCollection();
            requestHeadersCollection.Add("EndpointStatusChecker-Token", httpWebRequest_ApplicationGUID.ToString());
            requestHeadersCollection.Add("Accept-Encoding", "gzip, deflate, br");
            requestHeadersCollection.Add("Accept-Language", "*;*");
            requestHeadersCollection.Add("Cache-Control", "max-age=0");
            requestHeadersCollection.Add("DNT", "1");
            requestHeadersCollection.Add("Authority", endpointURI.Authority);
            requestHeadersCollection.Add("Path", endpointURI.AbsolutePath);
            requestHeadersCollection.Add("Scheme", endpointURI.Scheme);

            // SECURITY HEADERS
            requestHeadersCollection.Add("Upgrade-Insecure-Requests", "1");
            requestHeadersCollection.Add("Sec-Fetch-User", "?1");
            requestHeadersCollection.Add("Sec-Fetch-Site", "same-origin");
            requestHeadersCollection.Add("Sec-Fetch-Node", "navigate");
            requestHeadersCollection.Add("Sec-Fetch-Dest", "empty");
            requestHeadersCollection.Add("Sec-CH-UA", http_Sec_CH_UserAgent);
            requestHeadersCollection.Add("Sec-CH-UA-Mobile", "?0");
            requestHeadersCollection.Add("Sec-CH-UA-Platform", "\"Windows\"");
            httpWebRequest.Headers.Add(requestHeadersCollection);

            // SET CREDENTIALS
            httpWebRequest.PreAuthenticate = true;
            httpWebRequest.AuthenticationLevel = System.Net.Security.AuthenticationLevel.MutualAuthRequested;

            if (endpoint.LoginName != status_NotAvailable &&
                !string.IsNullOrEmpty(endpoint.LoginName))
            {
                // SPECIFIED CREDENTIALS
                httpWebRequest.Credentials = new NetworkCredential(endpoint.LoginName, endpoint.LoginPass);
            }
            else
            {
                // CURRENT WINDOWS USER CREDENTIALS
                httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
            }

            // GET REQUEST HEADERS
            GetHTTPWebHeaders(endpoint.HTTPRequestHeaders.PropertyItem, httpWebRequest.Headers);

            return httpWebRequest;
        }

        public void GetHTTPWebHeaders(List<Property> propertyItemCollection, WebHeaderCollection headerCollection)
        {
            propertyItemCollection.Clear();

            if (headerCollection != null &&
                headerCollection.Count > 0)
            {
                propertyItemCollection.Clear();

                foreach (string headerName in headerCollection.AllKeys)
                {
                    propertyItemCollection.Add(new Property { ItemName = headerName, ItemValue = headerCollection[headerName] });
                }
            }
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
            var formats = new[] {
                "r",							// preferred
                "dddd, dd-MMM-yy HH:mm:ss GMT",	// obsolete RFC 850 format
                "ddd MMM  d HH:mm:ss yyyy"		// ANSI C's asctime() format
            };

            return DateTime.TryParseExact(httpDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out parsedDate);
        }
        public bool CheckWebResponseContentLenght(EndpointDefinition endpoint, HttpWebResponse httpWebResponse, long contentLenght)
        {
            if (contentLenght > http_SaveResponse_MaxLenght_Bytes)
            {
                MessageBox.Show(
                    "Response content is too big for download (" + http_SaveResponse_MaxLenght_Bytes + " bytes limit)" +
                    Environment.NewLine +
                    Environment.NewLine +
                    "Endpoint Name:  " + endpoint.Name +
                    Environment.NewLine +
                    "Endpoint URL:  " + httpWebResponse.ResponseUri.AbsoluteUri +
                    Environment.NewLine +
                    "Response Content Type:  " + endpoint.HTTPcontentType +
                    Environment.NewLine +
                    "Response Content Lenght:  " + endpoint.HTTPcontentLenght,
                    "Download Response",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            return (contentLenght <= http_SaveResponse_MaxLenght_Bytes);
        }
        public void GetWebResponseContentLenghtString(EndpointDefinition endpoint, long contentLenght)
        {
            if (contentLenght == -1)
            {
                endpoint.HTTPcontentLenght = status_NotAvailable;
            }
            else if (contentLenght >= 1073741824)
            {
                endpoint.HTTPcontentLenght = (contentLenght / 1073741824).ToString("0.00") + " GB";
            }
            else if (contentLenght >= 1048576)
            {
                endpoint.HTTPcontentLenght = (contentLenght / 1048576).ToString("0.00") + " MB";
            }
            else if (contentLenght >= 1024)
            {
                endpoint.HTTPcontentLenght = (contentLenght / 1024).ToString("0.00") + " kB";
            }
            else
            {
                endpoint.HTTPcontentLenght = contentLenght + " bytes";
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
            if (encoding != null &&
                !string.IsNullOrEmpty(encoding.EncodingName))
            {
                return encoding.EncodingName;
            }
            else
            {
                return status_NotAvailable;
            }
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
            object value = key != null ? key.GetValue("Extension", null) : null;
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
                int threadCountMax_WT;
                int threadCountAvailable_WT;
                int threadCountMax_CPT;
                int threadCountAvailable_CPT;
                ThreadPool.GetMaxThreads(out threadCountMax_WT, out threadCountMax_CPT);
                ThreadPool.GetAvailableThreads(out threadCountAvailable_WT, out threadCountAvailable_CPT);
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
                    var taskBarInstance = Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance;

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
            }

            // SET STATUS LABEL TEXT AND COLOR
            ThreadSafeInvoke(() =>
            {
                Application.DoEvents();
                lbl_ProgressCount.ForeColor = statusColor;
                lbl_ProgressCount.Text = statusMessage;
                Application.DoEvents();
            });
        }

        public void btn_Refresh_Click(object sender, EventArgs e)
        {
            if (IsHandleCreated &&
                !BW_GetStatus.IsBusy &&
                !onClose &&
                dialog_EndpointDetails == null &&
                dialog_SpeedTest == null)
            {
                btn_Refresh.Enabled = false;

                SetControls(true, true);

                BW_GetStatus.RunWorkerAsync();
            }
        }

        public void SetControls(bool inProgress, bool locked)
        {
            if (inProgress)
            {
                // SET TRAY ICON [ANIMATION]
                SetTrayIcon(12, 20);

                // SET TRAY TOOLTIP MESSAGE
                SetTrayTooltipText(Environment.NewLine + "Endpoints check in progress ...");

                // CLEAR PROGRESS COUNTER INFORMATION LABEL
                SetProgressStatus(0, 0, string.Empty);
            }
            else if (Visible &&
                     Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.IsPlatformSupported)
            {
                // SET TASKBAR PROGRESS TO 'NO PROGRESS'
                Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance
                    .SetProgressState(
                    Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.NoProgress);
            }

            // VISIBLE OR ENABLED DURING PROGRESS
            btn_Terminate.Enabled = inProgress && locked;
            lbl_Terminate.Enabled = inProgress && locked;
            lbl_ProgressCount.Visible = inProgress && locked;
            pb_RefreshProcess.Visible = inProgress && locked;

            // NOT VISIBLE OR ENABLED DURING PROGRESS
            SetCheckButtons(!inProgress && !locked);
            groupBox_Export.Enabled = !inProgress && !locked;
            groupBox_CommonOptions.Enabled = !inProgress && !locked;
            groupBox_HTTPOptions.Enabled = !inProgress && !locked;
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
            cb_PingHost.Enabled = !inProgress && !locked;
            cb_DNSAndMACLookupOnHost.Enabled = !inProgress && !locked;
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
            tray_Refresh.Visible = !inProgress && !locked && dialog_SpeedTest == null && dialog_EndpointDetails == null;
            tray_SpeedTest.Visible = !inProgress && !locked && dialog_SpeedTest == null && dialog_EndpointDetails == null;
            btn_BrowseExportDir.Enabled = !inProgress && !locked;
            btn_SpeedTest.Enabled = !inProgress && !locked;
            lbl_Refresh.Enabled = !inProgress && !locked;
            lbl_BrowseExportDir.Enabled = !inProgress && !locked;
            lbl_SpeedTest.Enabled = !inProgress && !locked;

            lbl_ListFilter.Enabled = !inProgress && !locked && endpointsList.Count > 0;
            tb_ListFilter.Enabled = !inProgress && !locked && endpointsList.Count > 0;

            pb_ListFilterClear.Visible = !inProgress && !locked && endpointsList.Count > 0 && !string.IsNullOrEmpty(tb_ListFilter.Text);

            if (string.IsNullOrEmpty(tb_ListFilter.Text))
            {
                tb_ListFilter.BackColor = Color.LightGray;
            }
            else if (lv_Endpoints.Items.Count > 0)
            {
                tb_ListFilter.BackColor = Color.Honeydew;
            }
            else
            {
                tb_ListFilter.BackColor = Color.MistyRose;
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

                // LAST UPDATE LABEL
                lbl_LastUpdate.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
                lbl_LastUpdate.Visible = true;
                lbl_LastUpdate_Label.Visible = true;

                // TRAY ICON
                RefreshTrayIcon();

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
            if (btn_Refresh.Enabled)
            {
                btn_Refresh_Click(this, null);
            }
        }

        public void num_RefreshInterval_ValueChanged(object sender, EventArgs e)
        {
            lbl_TimerIntervalMinutesText.Text = GetFormattedValueCountString((int)num_RefreshInterval.Value, "minute");

            SaveConfiguration();
            TIMER_AutomaticRefresh.Interval = ((int)num_RefreshInterval.Value * 60000);
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
                btn_Refresh.Enabled = (lv_Endpoints.CheckedItems.Count > 0);
            }
        }

        public void SetTrayTooltipText(string text)
        {
            Type t = typeof(NotifyIcon);
            BindingFlags hidden = BindingFlags.NonPublic | BindingFlags.Instance;
            t.GetField("text", hidden).SetValue(trayIcon, Text + Environment.NewLine + text);
            if ((bool)t.GetField("added", hidden).GetValue(trayIcon))
            {
                t.GetMethod("UpdateIcon", hidden).Invoke(trayIcon, new object[] { true });
            }
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
                btn_Refresh_Click(this, null);
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
            btn_Refresh.Enabled = (lv_Endpoints.CheckedItems.Count > 0);
        }

        public void btn_UncheckAll_Click(object sender, EventArgs e)
        {
            SetCheckButtons(false);
            ListEndpoints(ListViewRefreshMethod.UncheckAll);
            SetCheckButtons(true);

            RefreshTrayIcon();
            btn_Refresh.Enabled = (lv_Endpoints.CheckedItems.Count > 0);
        }

        public void btn_CheckAllAvailable_Click(object sender, EventArgs e)
        {
            SetCheckButtons(false);
            ListEndpoints(ListViewRefreshMethod.CheckAllPassed);
            SetCheckButtons(true);

            RefreshTrayIcon();
            btn_Refresh.Enabled = (lv_Endpoints.CheckedItems.Count > 0);
        }

        public void btn_CheckAllErrors_Click(object sender, EventArgs e)
        {
            SetCheckButtons(false);
            ListEndpoints(ListViewRefreshMethod.CheckAllFailed);
            SetCheckButtons(true);

            RefreshTrayIcon();
            btn_Refresh.Enabled = (lv_Endpoints.CheckedItems.Count > 0);
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
                            CloseFileStream(definitonsStatusExport_JSON_FileStream);
                            File.WriteAllText(Path.Combine(statusExport_Directory, statusExport_JSONFile), jsonString, Encoding.UTF8);
                            definitonsStatusExport_JSON_FileStream = OpenFileStream(Path.Combine(statusExport_Directory, statusExport_JSONFile));
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
                            CloseFileStream(definitonsStatusExport_XML_FileStream);
                            xmlExport.Save(Path.Combine(statusExport_Directory, statusExport_XMLFile));
                            definitonsStatusExport_XML_FileStream = OpenFileStream(Path.Combine(statusExport_Directory, statusExport_XMLFile));
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
                        endpointsStatusExport_HTTP_WorkSheet.Cell("E" + httpWorkSheetLineNumber).SetValue("Endpoint IP Address");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("F" + httpWorkSheetLineNumber).SetValue("Response Time");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("G" + httpWorkSheetLineNumber).SetValue("Status Code");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("H" + httpWorkSheetLineNumber).SetValue("Status Message");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("I" + httpWorkSheetLineNumber).SetValue("Last Seen Online");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("J" + httpWorkSheetLineNumber).SetValue("Endpoint Host MAC Address");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("K" + httpWorkSheetLineNumber).SetValue("Ping Time");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("L" + httpWorkSheetLineNumber).SetValue("UserName [Basic Auth]");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("M" + httpWorkSheetLineNumber).SetValue("Server ID");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("N" + httpWorkSheetLineNumber).SetValue("Endpoint DNS Name");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("O" + httpWorkSheetLineNumber).SetValue("Network Shares");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("P" + httpWorkSheetLineNumber).SetValue("HTTP Auto Redirects");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("Q" + httpWorkSheetLineNumber).SetValue("HTTP Content Type");
                        endpointsStatusExport_HTTP_WorkSheet.Cell("R" + httpWorkSheetLineNumber).SetValue("HTTP Content Lenght");
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
                        endpointsStatusExport_FTP_WorkSheet.Cell("E" + ftpWorkSheetLineNumber).SetValue("Endpoint IP Address");
                        endpointsStatusExport_FTP_WorkSheet.Cell("F" + ftpWorkSheetLineNumber).SetValue("Response Time");
                        endpointsStatusExport_FTP_WorkSheet.Cell("G" + ftpWorkSheetLineNumber).SetValue("Status Code");
                        endpointsStatusExport_FTP_WorkSheet.Cell("H" + ftpWorkSheetLineNumber).SetValue("Status Message");
                        endpointsStatusExport_FTP_WorkSheet.Cell("I" + ftpWorkSheetLineNumber).SetValue("Last Seen Online");
                        endpointsStatusExport_FTP_WorkSheet.Cell("J" + ftpWorkSheetLineNumber).SetValue("Endpoint Host MAC Address");
                        endpointsStatusExport_FTP_WorkSheet.Cell("K" + ftpWorkSheetLineNumber).SetValue("Ping Time");
                        endpointsStatusExport_FTP_WorkSheet.Cell("L" + ftpWorkSheetLineNumber).SetValue("UserName");
                        endpointsStatusExport_FTP_WorkSheet.Cell("M" + ftpWorkSheetLineNumber).SetValue("Endpoint DNS Name");
                        endpointsStatusExport_FTP_WorkSheet.Cell("N" + ftpWorkSheetLineNumber).SetValue("Network Shares");
                        ftpWorkSheetLineNumber++;

                        // ADD ENDPOINTS ITEMS TO SHEETS 
                        foreach (EndpointDefinition endpointItem in exportList)
                        {
                            if (endpointItem.Protocol == Uri.UriSchemeHttp.ToUpper() ||
                                endpointItem.Protocol == Uri.UriSchemeHttps.ToUpper())
                            {
                                // ADD ENDPOINT ITEM TO HTTP SHEET
                                endpointsStatusExport_HTTP_WorkSheet.Cell("A" + httpWorkSheetLineNumber).SetValue(endpointItem.Name);
                                endpointsStatusExport_HTTP_WorkSheet.Cell("B" + httpWorkSheetLineNumber).SetValue(endpointItem.Protocol);
                                endpointsStatusExport_HTTP_WorkSheet.Cell("C" + httpWorkSheetLineNumber).SetValue(endpointItem.Port);
                                endpointsStatusExport_HTTP_WorkSheet.Cell("D" + httpWorkSheetLineNumber).SetValue(endpointItem.ResponseAddress.Split(';')[0]);

                                // CREATE RESPONSE ADDRESS HYPERLINK
                                endpointsStatusExport_HTTP_WorkSheet.Cell("D" + httpWorkSheetLineNumber).Hyperlink = new XLHyperlink(endpointItem.ResponseAddress.Split(';')[0]);
                                endpointsStatusExport_HTTP_WorkSheet.Cell("E" + httpWorkSheetLineNumber).SetValue(string.Join(Environment.NewLine, endpointItem.IPAddress));
                                endpointsStatusExport_HTTP_WorkSheet.Cell("F" + httpWorkSheetLineNumber).SetValue(endpointItem.ResponseTime);
                                endpointsStatusExport_HTTP_WorkSheet.Cell("G" + httpWorkSheetLineNumber).SetValue(endpointItem.ResponseCode);
                                endpointsStatusExport_HTTP_WorkSheet.Cell("H" + httpWorkSheetLineNumber).SetValue(endpointItem.ResponseMessage);
                                endpointsStatusExport_HTTP_WorkSheet.Cell("I" + httpWorkSheetLineNumber).SetValue(endpointItem.LastSeenOnline);
                                endpointsStatusExport_HTTP_WorkSheet.Cell("J" + httpWorkSheetLineNumber).SetValue(string.Join(Environment.NewLine, endpointItem.MACAddress));
                                endpointsStatusExport_HTTP_WorkSheet.Cell("K" + httpWorkSheetLineNumber).SetValue(endpointItem.PingRoundtripTime);
                                endpointsStatusExport_HTTP_WorkSheet.Cell("L" + httpWorkSheetLineNumber).SetValue(endpointItem.LoginName);
                                endpointsStatusExport_HTTP_WorkSheet.Cell("M" + httpWorkSheetLineNumber).SetValue(endpointItem.ServerID);
                                endpointsStatusExport_HTTP_WorkSheet.Cell("N" + httpWorkSheetLineNumber).SetValue(string.Join(Environment.NewLine, endpointItem.DNSName));
                                endpointsStatusExport_HTTP_WorkSheet.Cell("O" + httpWorkSheetLineNumber).SetValue(string.Join(Environment.NewLine, endpointItem.NetworkShare));
                                endpointsStatusExport_HTTP_WorkSheet.Cell("P" + httpWorkSheetLineNumber).SetValue(endpointItem.HTTPautoRedirects);
                                endpointsStatusExport_HTTP_WorkSheet.Cell("Q" + httpWorkSheetLineNumber).SetValue(endpointItem.HTTPcontentType);
                                endpointsStatusExport_HTTP_WorkSheet.Cell("R" + httpWorkSheetLineNumber).SetValue(endpointItem.HTTPcontentLenght);
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
                                string connectionString =
                                    BuildUpConnectionString(
                                        endpointItem,
                                        Uri.UriSchemeFtp);

                                // ADD ENDPOINT ITEM TO FTP SHEET
                                endpointsStatusExport_FTP_WorkSheet.Cell("A" + ftpWorkSheetLineNumber).SetValue(endpointItem.Name);
                                endpointsStatusExport_FTP_WorkSheet.Cell("B" + ftpWorkSheetLineNumber).SetValue(endpointItem.Protocol);
                                endpointsStatusExport_FTP_WorkSheet.Cell("C" + ftpWorkSheetLineNumber).SetValue(endpointItem.Port);
                                endpointsStatusExport_FTP_WorkSheet.Cell("D" + ftpWorkSheetLineNumber).SetValue(connectionString);
                                endpointsStatusExport_FTP_WorkSheet.Cell("D" + ftpWorkSheetLineNumber).Hyperlink = new XLHyperlink(connectionString);
                                endpointsStatusExport_FTP_WorkSheet.Cell("E" + ftpWorkSheetLineNumber).SetValue(string.Join(Environment.NewLine, endpointItem.IPAddress));
                                endpointsStatusExport_FTP_WorkSheet.Cell("F" + ftpWorkSheetLineNumber).SetValue(endpointItem.ResponseTime);
                                endpointsStatusExport_FTP_WorkSheet.Cell("G" + ftpWorkSheetLineNumber).SetValue(endpointItem.ResponseCode);
                                endpointsStatusExport_FTP_WorkSheet.Cell("H" + ftpWorkSheetLineNumber).SetValue(endpointItem.ResponseMessage);
                                endpointsStatusExport_FTP_WorkSheet.Cell("I" + ftpWorkSheetLineNumber).SetValue(endpointItem.LastSeenOnline);
                                endpointsStatusExport_FTP_WorkSheet.Cell("J" + ftpWorkSheetLineNumber).SetValue(string.Join(Environment.NewLine, endpointItem.MACAddress));
                                endpointsStatusExport_FTP_WorkSheet.Cell("K" + ftpWorkSheetLineNumber).SetValue(endpointItem.PingRoundtripTime);
                                endpointsStatusExport_FTP_WorkSheet.Cell("L" + ftpWorkSheetLineNumber).SetValue(endpointItem.LoginName);
                                endpointsStatusExport_FTP_WorkSheet.Cell("M" + ftpWorkSheetLineNumber).SetValue(string.Join(Environment.NewLine, endpointItem.DNSName));
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
                        endpointsStatusExport_Summary_WorkSheet.Cell("B1").SetValue("Version " + app_VersionString + " (built " + app_BuiltDate + ")");
                        endpointsStatusExport_Summary_WorkSheet.Cell("A2").SetValue("Operating System");
                        endpointsStatusExport_Summary_WorkSheet.Cell("B2").SetValue(os_VersionString);
                        endpointsStatusExport_Summary_WorkSheet.Cell("A3").SetValue("Latest Installed .NET FrameWork Runtime");
                        endpointsStatusExport_Summary_WorkSheet.Cell("B3").SetValue(dotNetFramework_LatestInstalledVersion.ToString().Replace("v", "Version ").Replace("_", "."));
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
                        endpointsStatusExport_Summary_WorkSheet.Cell("D10").SetValue("Supported Security Protocols [HTTPS]");
                        endpointsStatusExport_Summary_WorkSheet.Cell("E10").SetValue("SSL 3.0, TLS 1.0, TLS 1.1, TLS 1.2, TLS 1.3");
                        endpointsStatusExport_Summary_WorkSheet.Cell("D11").SetValue("Server Certificate Validation [HTTPS]");
                        endpointsStatusExport_Summary_WorkSheet.Cell("E11").SetValue(sslCertificateValidation);
                        endpointsStatusExport_Summary_WorkSheet.Cell("D12").SetValue("Auto Redirection [HTTP]");
                        endpointsStatusExport_Summary_WorkSheet.Cell("E12").SetValue(httpAutoRedirection);
                        endpointsStatusExport_Summary_WorkSheet.Cell("D13").SetValue("Resolve Network Shares");
                        endpointsStatusExport_Summary_WorkSheet.Cell("E13").SetValue(resolveNetworkShares);
                        endpointsStatusExport_Summary_WorkSheet.Cell("D14").SetValue("Resolve Page Meta Info [HTTP/HTML]");
                        endpointsStatusExport_Summary_WorkSheet.Cell("E14").SetValue(resolvePageMetaInfo);
                        endpointsStatusExport_Summary_WorkSheet.Cell("D15").SetValue("Save Response [HTTP]");
                        endpointsStatusExport_Summary_WorkSheet.Cell("E15").SetValue(saveResponse);
                        endpointsStatusExport_Summary_WorkSheet.Cell("D16").SetValue("Ping Host");
                        endpointsStatusExport_Summary_WorkSheet.Cell("E16").SetValue(pingHost);
                        endpointsStatusExport_Summary_WorkSheet.Cell("D17").SetValue("DNS / MAC Lookup on Host");
                        endpointsStatusExport_Summary_WorkSheet.Cell("E17").SetValue(dnsLookupOnHost);

                        // SETTINGS FOR HTTP ENDPOINTS WORKSHEET
                        endpointsStatusExport_HTTP_WorkSheet.Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                            .Alignment.SetVertical(XLAlignmentVerticalValues.Top);
                        endpointsStatusExport_HTTP_WorkSheet.SheetView.FreezeRows(1);
                        endpointsStatusExport_HTTP_WorkSheet.SheetView.FreezeColumns(1);
                        endpointsStatusExport_HTTP_WorkSheet.RangeUsed().SetAutoFilter();
                        endpointsStatusExport_HTTP_WorkSheet.Rows().AdjustToContents();
                        endpointsStatusExport_HTTP_WorkSheet.Columns().AdjustToContents(10, (double)70);
                        endpointsStatusExport_HTTP_WorkSheet.CellsUsed().SetDataType(XLDataType.Text);
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
                        endpointsStatusExport_FTP_WorkSheet.CellsUsed().SetDataType(XLDataType.Text);
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
                            CloseFileStream(definitonsStatusExport_XLSX_FileStream);
                            endpointsStatusExport_WorkBook.SaveAs(Path.Combine(statusExport_Directory, statusExport_XLSFile), new SaveOptions { ValidatePackage = true });
                            definitonsStatusExport_XLSX_FileStream = OpenFileStream(Path.Combine(statusExport_Directory, statusExport_XLSFile));
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
                                jsonPageHyperlink.Text = "JSON_Endpoints_Status_List_Hyperlink";
                                summaryWorkSheet["A10"].Style.Color = Color.DarkOrange;
                                summaryWorkSheet["A10"].Style.Font.IsBold = true;
                                summaryWorkSheet["A10"].Style.HorizontalAlignment = HorizontalAlignType.Center;
                            }

                            if (cb_ExportEndpointsStatus_XML.Checked)
                            {
                                // ADD 'XML' HYPERLINK PLACEHOLDER TO 'SUMMARY' PAGE           
                                RichText xmlPageHyperlink = summaryWorkSheet["A11"].RichText;
                                xmlPageHyperlink.Text = "XML_Endpoints_Status_List_Hyperlink";
                                summaryWorkSheet["A11"].Style.Color = Color.Red;
                                summaryWorkSheet["A11"].Style.Font.IsBold = true;
                                summaryWorkSheet["A11"].Style.HorizontalAlignment = HorizontalAlignType.Center;
                            }

                            // ADD 'XLSX' HYPERLINK PLACEHOLDER TO 'SUMMARY' PAGE           
                            RichText xlsxPageHyperlink = summaryWorkSheet["A12"].RichText;
                            xlsxPageHyperlink.Text = "XLSX_Endpoints_Status_List_Hyperlink";
                            summaryWorkSheet["A12"].Style.Color = Color.DarkViolet;
                            summaryWorkSheet["A12"].Style.Font.IsBold = true;
                            summaryWorkSheet["A12"].Style.HorizontalAlignment = HorizontalAlignType.Center;

                            if (xlsxWorkBook.Worksheets.Where(w => w.Name == "HTTP Endpoints").Count() == 1)
                            {
                                try
                                {
                                    CloseFileStream(definitonsStatusExport_HTML_Info_FileStream);
                                    CloseFileStream(definitonsStatusExport_HTML_HTTP_FileStream);
                                    CloseFileStream(definitonsStatusExport_HTML_FTP_FileStream);

                                    // SAVE HTML [HTTP PAGE]
                                    Application.DoEvents();
                                    xlsxWorkBook.Worksheets["HTTP Endpoints"].SaveToHtml(Path.Combine(statusExport_Directory, statusExport_HTMLFile_HTTPPage));
                                    Application.DoEvents();

                                    // ADD 'HTTP' HTML FIXED REFRESH BUTTON
                                    string httpHTMLstring = File.ReadAllText(Path.Combine(statusExport_Directory, statusExport_HTMLFile_HTTPPage));
                                    httpHTMLstring = CreateEndpointURLHyperLink(httpHTMLstring);
                                    httpHTMLstring = AddRefreshCSSButtonToHTMLString(httpHTMLstring);

                                    // SAVE HTML STRING [HTTP PAGE]
                                    Application.DoEvents();
                                    File.WriteAllText(Path.Combine(statusExport_Directory, statusExport_HTMLFile_HTTPPage), httpHTMLstring, Encoding.UTF8);
                                    Application.DoEvents();

                                    // LOCK 'HTTP' HTML
                                    definitonsStatusExport_HTML_HTTP_FileStream = OpenFileStream(Path.Combine(statusExport_Directory, statusExport_HTMLFile_HTTPPage));

                                    // ADD 'HTTP' HYPERLINK PLACEHOLDER TO 'SUMMARY' PAGE           
                                    RichText httpPageHyperlink = summaryWorkSheet["A14"].RichText;
                                    httpPageHyperlink.Text = "HTTP_Endpoints_Status_List_Hyperlink";
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
                                    string ftpHTMLstring = File.ReadAllText(Path.Combine(statusExport_Directory, statusExport_HTMLFile_FTPPage));
                                    ftpHTMLstring = CreateEndpointURLHyperLink(ftpHTMLstring);
                                    ftpHTMLstring = AddRefreshCSSButtonToHTMLString(ftpHTMLstring);

                                    // SAVE HTML STRING [FTP PAGE]
                                    Application.DoEvents();
                                    File.WriteAllText(Path.Combine(statusExport_Directory, statusExport_HTMLFile_FTPPage), ftpHTMLstring, Encoding.UTF8);
                                    Application.DoEvents();

                                    // LOCK 'FTP' HTML
                                    definitonsStatusExport_HTML_FTP_FileStream = OpenFileStream(Path.Combine(statusExport_Directory, statusExport_HTMLFile_FTPPage));

                                    // ADD 'FTP' HYPERLINK PLACEHOLDER TO 'SUMMARY' PAGE           
                                    RichText ftpPageHyperlink = summaryWorkSheet["A15"].RichText;
                                    ftpPageHyperlink.Text = "FTP_Endpoints_Status_List_Hyperlink";
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
                                    .Replace("XLSX_Endpoints_Status_List_Hyperlink", "<a href=\"" + statusExport_XLSFile + "\" style=\"color:white;\">Endpoints Status XLSX Export</a>")
                                    .Replace("JSON_Endpoints_Status_List_Hyperlink", "<a href=\"" + statusExport_JSONFile + "\" style=\"color:white;\">Endpoints Status JSON Export </a>")
                                    .Replace("XML_Endpoints_Status_List_Hyperlink", "<a href=\"" + statusExport_XMLFile + "\" style=\"color:white;\">Endpoints Status XML Export</a>")
                                    .Replace("HTTP_Endpoints_Status_List_Hyperlink", "<a href=\"" + statusExport_HTMLFile_HTTPPage + "\" style=\"color:white;\">HTTP Endpoints Status List</a>")
                                    .Replace("FTP_Endpoints_Status_List_Hyperlink", "<a href=\"" + statusExport_HTMLFile_FTPPage + "\" style=\"color:white;\">FTP Endpoints Status List</a>");

                                // ADD HTML AUTO REFRESH
                                summaryHTMLstring = AddAutoRefreshToHTMLString(summaryHTMLstring, 30);

                                // SAVE HTML STRING [SUMMARY PAGE]
                                Application.DoEvents();
                                File.WriteAllText(Path.Combine(statusExport_Directory, statusExport_HTMLFile_InfoPage), summaryHTMLstring, Encoding.UTF8);
                                Application.DoEvents();

                                // LOCK 'SUMMARY' HTML
                                definitonsStatusExport_HTML_Info_FileStream = OpenFileStream(Path.Combine(statusExport_Directory, statusExport_HTMLFile_InfoPage));
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
                            ExceptionNotifier(this, exception, true);
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

                    using (var stringWriter = new StringWriter())
                    using (var xmlTextWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true, NewLineOnAttributes = false, OmitXmlDeclaration = true }))
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
                {
                    // TERMINATED
                    return Color.LightSkyBlue;
                }
                else if (statusMessage == GetEnumDescriptionString(EndpointStatus.NOTCHECKED) ||
                         statusMessage == GetEnumDescriptionString(EndpointStatus.DISABLED))
                {
                    // DISABLED / NOT CHECKED
                    return Color.Gray;
                }
            }

            if (validationMethod == ValidationMethod.Protocol)
            {
                if (statusMessage == GetEnumDescriptionString(EndpointStatus.PINGCHECK))
                {
                    // NOT CHECKED
                    return Color.LightGray;
                }
                else if (statusCode == status_Error)
                {
                    // ERROR
                    return Color.Crimson;
                }
                else if (statusCode[0].ToString() == "2")
                {
                    // SUCCESS
                    return Color.PaleGreen;
                }
                else if (statusCode[0].ToString() == "4")
                {
                    // PROTOCOL ERROR
                    return Color.HotPink;
                }
                else
                {
                    // WARNING
                    return Color.SandyBrown;
                }
            }
            else
            {
                if (pingTime == status_NotAvailable)
                {
                    // ERROR
                    return Color.Crimson;
                }
                else
                {
                    // SUCCESS
                    return Color.PaleGreen;
                }
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

            // DISABLE ITSELF
            btn_Terminate.Enabled = false;
            lbl_Terminate.Enabled = false;

            // TERMINATE WORKER
            BW_GetStatus.CancelAsync();

            SetProgressStatus(0, 0);
        }

        public void num_ParallelThreadsCount_ValueChanged(object sender, EventArgs e)
        {
            SaveConfiguration();
        }

        public void CheckerMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            onClose = true;

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

            // SAVE SETTINGS
            SaveListViewColumnsWidthAndOrder();
            SaveWindowSizeAndPosition();
            SaveDisabledItemsListAndFilter();
            SaveConfiguration();
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
        static extern int NetApiBufferFree(IntPtr Buffer);

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
            Dictionary<int, string> codeList = new Dictionary<int, string>();
            codeList.Add(0, "OK");
            codeList.Add(5, "The user has insufficient privilege for this operation");
            codeList.Add(8, "Not enough memory");
            codeList.Add(65, "Network access is denied");
            codeList.Add(87, "Invalid parameter specified");
            codeList.Add(53, "The network path was not found");
            codeList.Add(123, "Invalid name");
            codeList.Add(124, "Invalid level parameter");
            codeList.Add(234, "More data available, buffer too small");
            codeList.Add(2102, "Device driver not installed");
            codeList.Add(2106, "This operation can be performed only on a server");
            codeList.Add(2114, "Server service not installed");
            codeList.Add(2123, "Buffer too small for fixed-length data");
            codeList.Add(2127, "Error encountered while executing function remotely");
            codeList.Add(2138, "The Workstation service is not started");
            codeList.Add(2141, "The server is not configured for this transaction (IPC$ is not shared)");
            codeList.Add(2351, "Invalid computername specified");

            if (codeList.ContainsKey(code))
            {
                return codeList[code];
            }
            else
            {
                return "Result Code: " + code.ToString();
            }
        }

        public string NetShareType(uint code)
        {
            Dictionary<uint, string> codeList = new Dictionary<uint, string>();
            codeList.Add(0, "Folder");
            codeList.Add(1, "Printer");
            codeList.Add(2, "Device");
            codeList.Add(3, "IPC");
            codeList.Add(2147483648, "Admin/Folder");
            codeList.Add(2147483649, "Admin/Printer");
            codeList.Add(2147483650, "Admin/Device");
            codeList.Add(2147483651, "Admin/IPC");

            if (codeList.ContainsKey(code))
            {
                return codeList[code];
            }
            else
            {
                return "Type Code: " + code.ToString();
            }
        }

        ToolTip endpointToolTip;
        Point endpointToolTipLastPosition = new Point(-1, -1);
        public void lv_Endpoints_MouseMove(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo hitTestItem = lv_Endpoints.HitTest(e.X, e.Y);

            if (endpointToolTip == null)
                endpointToolTip = new ToolTip();

            if (endpointToolTipLastPosition != e.Location)
            {
                if (hitTestItem.Item != null &&
                    hitTestItem.SubItem != null &&
                    hitTestItem.SubItem.Name == "Endpoint Name")
                {
                    // TEXT
                    string infoText = Environment.NewLine;
                    infoText += "Left mouse button doubleclick for select / unselect EndPoint";
                    infoText += Environment.NewLine;
                    infoText += "Right mouse button click for more EndPoint options context menu";
                    infoText += Environment.NewLine + Environment.NewLine;
                    infoText += "Select single or more EndPoints and press CTRL+C to copy details to clipboard";
                    endpointToolTip.ToolTipTitle = hitTestItem.Item.Text;
                    endpointToolTip.Show(infoText, hitTestItem.Item.ListView, (e.X + 20), (e.Y + 25), 20000);
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
                    Settings.Default.ListView_ColWidth_Service = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_EndpointName)].Width;
                    Settings.Default.ListView_ColWidth_Protocol = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_Protocol)].Width;
                    Settings.Default.ListView_ColWidth_Port = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_Port)].Width;
                    Settings.Default.ListView_ColWidth_Endpoint = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_EndpointURL)].Width;
                    Settings.Default.ListView_ColWidth_IPAddress = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_IPAddress)].Width;
                    Settings.Default.ListView_ColWidth_ResponseTime = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_ResponseTime)].Width;
                    Settings.Default.ListView_ColWidth_Code = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_Code)].Width;
                    Settings.Default.ListView_ColWidth_Message = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_Message)].Width;
                    Settings.Default.ListView_ColWidth_LastSeenOnline = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_LastSeenOnline)].Width;
                    Settings.Default.ListView_ColWidth_MACAddress = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_MACAddress)].Width;
                    Settings.Default.ListView_ColWidth_PingTime = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_PingTime)].Width;
                    Settings.Default.ListView_ColWidth_Server = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_Server)].Width;
                    Settings.Default.ListView_ColWidth_UserName = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_UserName)].Width;
                    Settings.Default.ListView_ColWidth_NetworkShares = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_NetworkShares)].Width;
                    Settings.Default.ListView_ColWidth_DNSName = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_DNSName)].Width;
                    Settings.Default.ListView_ColWidth_ContentLenght = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_HTTPContentLenght)].Width;
                    Settings.Default.ListView_ColWidth_ContentType = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_HTTPContentType)].Width;
                    Settings.Default.ListView_ColWidth_Expires = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_HTTPExpires)].Width;
                    Settings.Default.ListView_ColWidth_ETag = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_HTTPETag)].Width;

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
                    Settings.Default.ListView_DisplayIndex_ContentLenght = lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_HTTPContentLenght)].DisplayIndex;
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
                if (Settings.Default.ListView_ColWidth_ContentLenght != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_HTTPContentLenght)].Width = Settings.Default.ListView_ColWidth_ContentLenght; }
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
                if (Settings.Default.ListView_DisplayIndex_ContentLenght != -1) { lv_Endpoints.Columns[lv_Endpoints.Columns.IndexOf(ch_HTTPContentLenght)].DisplayIndex = Settings.Default.ListView_DisplayIndex_ContentLenght; }
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
                    endpointsList_LastSeenOnline = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(lastSeenOnlineJSONFile));
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
                if (File.Exists(endpointDefinitonsFile))
                {
                    List<string> endpointDuplicityList = new List<string>();
                    List<string> invalidURLList = new List<string>();

                    // READ DEFINITIONS FILE LINE BY LINE
                    int lineNumber = 1;
                    string line;
                    StreamReader file = new StreamReader(endpointDefinitonsFile, Encoding.Default, true);
                    while ((line = file.ReadLine()) != null)
                    {
                        // REMOVE SPACES FROM LINE
                        line = line.Trim();

                        // CHECK LINE
                        if (!string.IsNullOrEmpty(line) && line != "|")
                        {
                            // CHECK ITEMS COUNT LIMIT
                            if (lineNumber > Settings.Default.Config_MaximumEndpointReferencesCount)
                            {
                                MessageBox.Show(
                                  "Endpoints definitions file \"" + endpointDefinitonsFile +
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

                            // CHECK DEFINITION FOR NAME PARAMETER
                            if (!line.Contains("|"))
                            {
                                // CREATE DEFAULT DEFINITION WITH DEFAULT NAME [ENDPOINT URL]
                                line = line + "|" + line;
                            }

                            // CREATE ENDPOINT STATUS DEFINITION
                            EndpointDefinition endpointStatusDefiniton = new EndpointDefinition()
                            {
                                Name = line.Split('|')[0].TrimEnd('/'),
                                Protocol = status_NotAvailable,
                                Port = status_NotAvailable,
                                Address = line.Split('|')[1].TrimEnd('/'),
                                ResponseAddress = line.Split('|')[1].TrimEnd('/'),
                                IPAddress = new string[] { status_NotAvailable },
                                ResponseTime = status_NotAvailable,
                                ResponseCode = status_NotAvailable,
                                ResponseMessage = GetEnumDescriptionString(EndpointStatus.NOTCHECKED),
                                LastSeenOnline = status_NotAvailable,
                                PingRoundtripTime = status_NotAvailable,
                                ServerID = status_NotAvailable,
                                LoginName = status_NotAvailable,
                                LoginPass = status_NotAvailable,
                                NetworkShare = new string[] { status_NotAvailable },
                                DNSName = new string[] { status_NotAvailable },
                                HTMLMetaInfo = new PropertyItems() { PropertyItem = new List<Property>() },
                                HTTPautoRedirects = status_NotAvailable,
                                HTTPcontentType = status_NotAvailable,
                                HTTPencoding = null,
                                HTMLdefaultStreamEncoding = null,
                                HTMLencoding = null,
                                HTMLTitle = status_NotAvailable,
                                HTMLAuthor = status_NotAvailable,
                                HTMLPageLinks = new PropertyItems() { PropertyItem = new List<Property>() },
                                HTMLDescription = status_NotAvailable,
                                HTMLContentLanguage = status_NotAvailable,
                                HTMLThemeColor = Color.Empty,
                                HTTPcontentLenght = status_NotAvailable,
                                HTTPexpires = status_NotAvailable,
                                HTTPetag = status_NotAvailable,
                                HTTPRequestHeaders = new PropertyItems() { PropertyItem = new List<Property>() },
                                HTTPResponseHeaders = new PropertyItems() { PropertyItem = new List<Property>() },
                                MACAddress = new string[] { status_NotAvailable },
                                FTPBannerMessage = status_NotAvailable,
                                FTPWelcomeMessage = status_NotAvailable,
                                FTPExitMessage = status_NotAvailable,
                                FTPStatusDescription = status_NotAvailable,
                                SSLCertificateProperties = new PropertyItems() { PropertyItem = new List<Property>() }
                            };

                            bool duplicityDefinition = false;
                            bool invalidURL = false;

                            // CHECK URL
                            if (!endpointStatusDefiniton.Address.ToLower().Contains(Uri.SchemeDelimiter))
                            {
                                // URL FORMAT IS INVALID, ADD TO ERRORS LIST [MISSING PROTOCOL PREFIX]
                                invalidURL = true;
                                invalidURLList.Add(
                                        "Line:  " + lineNumber +
                                        Environment.NewLine +
                                        "Endpoint Name:  " + endpointStatusDefiniton.Name +
                                        Environment.NewLine +
                                        "Endpoint Address:  " + endpointStatusDefiniton.Address +
                                        Environment.NewLine +
                                        "Missing protocol prefix" +
                                        Environment.NewLine +
                                        Uri.UriSchemeHttp.ToUpper() + ", " +
                                        Uri.UriSchemeHttps.ToUpper() + " and " +
                                        Uri.UriSchemeFtp.ToUpper() + " protocols are supported" +
                                        Environment.NewLine +
                                        Environment.NewLine
                                    );
                            }
                            else
                            {
                                // GET CREDENTIALS FROM URL [IF PRESENT]
                                if (endpointStatusDefiniton.Address.Contains("@"))
                                {
                                    string[] credentials = endpointStatusDefiniton.Address.Split('@')[0].Split('/')[2].Split(':');
                                    if (credentials.Length == 2)
                                    {
                                        // EXTRACT
                                        endpointStatusDefiniton.LoginName = credentials[0];
                                        endpointStatusDefiniton.LoginPass = credentials[1];

                                        // REMOVE CREDENTIALS FROM ENDPOINT NAME
                                        // ADD USERNAME IDENTIFIER
                                        endpointStatusDefiniton.Name = endpointStatusDefiniton.Name
                                            .Replace(
                                            endpointStatusDefiniton.LoginName + ":" +
                                            endpointStatusDefiniton.LoginPass + "@", string.Empty) +
                                            " [as '" + endpointStatusDefiniton.LoginName + "']";

                                        // REMOVE CREDENTIALS FROM ENDPOINT ADDRESS
                                        endpointStatusDefiniton.Address = endpointStatusDefiniton.Address
                                            .Replace(endpointStatusDefiniton.LoginName + ":" +
                                            endpointStatusDefiniton.LoginPass + "@", string.Empty);
                                    }
                                }

                                try
                                {
                                    // CHECK URL FORMAT [TRY TO CREATE ENDPOINT URI]
                                    Uri endpointURI = new Uri(endpointStatusDefiniton.Address, UriKind.Absolute);

                                    // GET URI ATTRIBUTES
                                    endpointStatusDefiniton.Port = endpointURI.Port.ToString();
                                    endpointStatusDefiniton.Protocol = endpointURI.Scheme.ToUpper();

                                    // CHECK SUPPORTED PROTOCOLS TYPES
                                    if (endpointStatusDefiniton.Protocol != Uri.UriSchemeHttp.ToUpper() &&
                                        endpointStatusDefiniton.Protocol != Uri.UriSchemeHttps.ToUpper() &&
                                        endpointStatusDefiniton.Protocol != Uri.UriSchemeFtp.ToUpper())
                                    {
                                        // UNSUPPORTED PROTOCOL TYPE, ADD TO ERRORS LIST [UNSUPPORTED PROTOCOL TYPE]
                                        invalidURL = true;
                                        invalidURLList.Add(
                                            "Line:  " + lineNumber +
                                            Environment.NewLine +
                                            "Endpoint Name:  " + endpointStatusDefiniton.Name +
                                            Environment.NewLine +
                                            "Endpoint Address:  " + endpointStatusDefiniton.Address +
                                            Environment.NewLine +
                                            "Unsupported protocol type: " + endpointStatusDefiniton.Protocol +
                                            Environment.NewLine +
                                            Uri.UriSchemeHttp.ToUpper() + ", " +
                                            Uri.UriSchemeHttps.ToUpper() + " and " +
                                            Uri.UriSchemeFtp.ToUpper() + " protocols are supported" +
                                            Environment.NewLine +
                                            Environment.NewLine
                                        );
                                    }
                                }
                                catch (Exception exception)
                                {
                                    // URL FORMAT IS INVALID, ADD TO ERRORS LIST [INVALID URL FORMAT]
                                    invalidURL = true;
                                    invalidURLList.Add(
                                            "Line:  " + lineNumber +
                                            Environment.NewLine +
                                            "Endpoint Name:  " + endpointStatusDefiniton.Name +
                                            Environment.NewLine +
                                            "Endpoint Address:  " + endpointStatusDefiniton.Address +
                                            Environment.NewLine +
                                            "URL in invalid format" +
                                            Environment.NewLine +
                                            exception.Message +
                                            Environment.NewLine +
                                            Environment.NewLine
                                        );
                                }
                            }

                            foreach (EndpointDefinition endpointDefinition in endpointsList)
                            {
                                // CHECK ALL EXISTING DEFINITIONS FOR NAME DUPLICITY
                                if (endpointDefinition.Name == endpointStatusDefiniton.Name)
                                {
                                    // DUPLICITY EXISTS, ADD TO ERRORS LIST [DUPLICITIES]
                                    duplicityDefinition = true;
                                    endpointDuplicityList.Add(
                                        "Line:  " + lineNumber +
                                        Environment.NewLine +
                                        "Endpoint Name:  " + endpointStatusDefiniton.Name +
                                        Environment.NewLine +
                                        "Endpoint Address:  " + endpointStatusDefiniton.Address +
                                        Environment.NewLine +
                                        Environment.NewLine
                                    );
                                }
                            }

                            if (!duplicityDefinition &&
                                !invalidURL)
                            {
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
                        endpointDefinitonsFile + "\" contains " +
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
                        endpointDefinitonsFile + "\" contains " +
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
                                using (StreamWriter sw = new StreamWriter(endpointsList_InvalidDefs))
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

                            invalidURLMessage += "See \"" + endpointsList_InvalidDefs + "\" for details.";
                        }

                        MessageBox.Show(invalidURLMessage, "Invalid endpoint definitions - Invalid URL format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    SetControls(false, true);

                    lbl_NoEndpoints.ForeColor = Color.Red;
                    lbl_NoEndpoints.Text = "Endpoints definitions file \"" + endpointDefinitonsFile + "\" doesn't exists in \"" + Directory.GetCurrentDirectory() + "\".";
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
                        btn_Refresh_Click(this, null);
                    });
                }

                // CONTINUOUS REFRESH
                if (cb_ContinuousRefresh.Checked)
                {
                    TIMER_ContinuousRefresh.Start();
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

                CloseFileStream(definitonsStatusExport_XLSX_FileStream);
            }

            SaveConfiguration();
        }

        public void cb_ExportEndpointsStatus_HTML_CheckedChanged(object sender, EventArgs e)
        {
            if (!cb_ExportEndpointsStatus_HTML.Checked)
            {
                CloseFileStream(definitonsStatusExport_HTML_Info_FileStream);
                CloseFileStream(definitonsStatusExport_HTML_HTTP_FileStream);
                CloseFileStream(definitonsStatusExport_HTML_FTP_FileStream);
            }
            else
            {
                // ENABLE XLSX EXPORT TOO
                cb_ExportEndpointsStatus_XLSX.Checked = true;
            }

            SaveConfiguration();
        }

        public void SetTrayIcon(int firstFrameIndex, int lastFrameIndex)
        {
            if (firstFrameIndex == lastFrameIndex)
            {
                TIMER_TrayIconAnimation.Stop();
                trayAnimation_Icons.Clear();
                trayAnimation_Index = 0;

                trayIcon.Icon = GetIconFromListByIndex(firstFrameIndex);
            }
            else if (lastFrameIndex > firstFrameIndex)
            {
                for (int index = firstFrameIndex; index <= lastFrameIndex; index++)
                {
                    trayAnimation_Icons.Add(GetIconFromListByIndex(index));
                }

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
            if (sender != lv_Endpoints) return;

            if (e.Control && e.KeyCode == Keys.C)
                CopySelectedValuesToClipboard();
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
            folderBrowserExportDir.SelectedPath = statusExport_Directory;
            if (folderBrowserExportDir.ShowDialog() == DialogResult.OK)
            {
                statusExport_Directory = folderBrowserExportDir.SelectedPath;

                try
                {
                    CloseFileStream(definitonsStatusExport_JSON_FileStream);
                    using (File.Create(Path.Combine(statusExport_Directory, statusExport_JSONFile))) { }
                    File.Delete(Path.Combine(statusExport_Directory, statusExport_JSONFile));

                    CloseFileStream(definitonsStatusExport_XML_FileStream);
                    using (File.Create(Path.Combine(statusExport_Directory, statusExport_XMLFile))) { }
                    File.Delete(Path.Combine(statusExport_Directory, statusExport_XMLFile));

                    CloseFileStream(definitonsStatusExport_XLSX_FileStream);
                    using (File.Create(Path.Combine(statusExport_Directory, statusExport_XLSFile))) { }
                    File.Delete(Path.Combine(statusExport_Directory, statusExport_XLSFile));

                    CloseFileStream(definitonsStatusExport_HTML_Info_FileStream);
                    using (File.Create(Path.Combine(statusExport_Directory, statusExport_HTMLFile_InfoPage))) { }
                    File.Delete(Path.Combine(statusExport_Directory, statusExport_HTMLFile_InfoPage));

                    CloseFileStream(definitonsStatusExport_HTML_HTTP_FileStream);
                    using (File.Create(Path.Combine(statusExport_Directory, statusExport_HTMLFile_HTTPPage))) { }
                    File.Delete(Path.Combine(statusExport_Directory, statusExport_HTMLFile_HTTPPage));

                    CloseFileStream(definitonsStatusExport_HTML_FTP_FileStream);
                    using (File.Create(Path.Combine(statusExport_Directory, statusExport_HTMLFile_FTPPage))) { }
                    File.Delete(Path.Combine(statusExport_Directory, statusExport_HTMLFile_FTPPage));
                }
                catch (Exception exception)
                {
                    ExceptionNotifier(this, exception, true);
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
                if (ch_HTTPContentLenght.Tag != null) { ch_HTTPContentLenght.Width = (int)ch_HTTPContentLenght.Tag; }
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
                ch_HTTPContentLenght.Tag = ch_HTTPContentLenght.Width;
                ch_HTTPExpires.Tag = ch_HTTPExpires.Width;
                ch_HTTPETag.Tag = ch_HTTPETag.Width;
                ch_HTTPContentType.Tag = ch_HTTPContentType.Width;
                ch_Port.Tag = ch_Port.Width;
                ch_Protocol.Tag = ch_Protocol.Width;
                ch_ResponseTime.Tag = ch_ResponseTime.Width;
                ch_Server.Tag = ch_Server.Width;
                ch_UserName.Tag = ch_UserName.Width;

                ch_Code.Width = 0;
                ch_HTTPContentLenght.Width = 0;
                ch_HTTPExpires.Width = 0;
                ch_HTTPETag.Width = 0;
                ch_HTTPContentType.Width = 0;
                ch_Port.Width = 0;
                ch_Protocol.Width = 0;
                ch_ResponseTime.Width = 0;
                ch_Server.Width = 0;
                ch_UserName.Width = 0;

                cb_PingHost.Checked = true;
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
                    if (IPAddress.IsLoopback(hostIP)) return true;
                    // is local address
                    foreach (IPAddress localIP in localIPs)
                    {
                        if (hostIP.Equals(localIP)) return true;
                    }
                }
            }
            catch
            {
            }

            return false;
        }

        public static string GetMACAddress(IPAddress ipAddress)
        {
            byte[] macAddr = new byte[6];
            uint macAddrLen = (uint)macAddr.Length;

            if (SendARP(BitConverter.ToInt32(ipAddress.GetAddressBytes(), 0), 0, macAddr, ref macAddrLen) == 0)
            {
                string[] macStrArr = new string[(int)macAddrLen];

                for (int i = 0; i < macAddrLen; i++)
                {
                    macStrArr[i] = macAddr[i].ToString("x2");
                }

                return (string.Join(":", macStrArr)).ToUpper();
            }
            else
            {
                return null;
            }
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

                foreach (ManagementObject oRDNS in oRcDNS)
                {
                    foreach (PropertyData property in oRDNS.Properties)
                    {
                        if (property.Value != null)
                        {
                            foreach (string dnsIP in (Array)property.Value)
                            {
                                if (!string.IsNullOrEmpty(dnsIP))
                                {
                                    localDNSAndGWipAddresses.Add(dnsIP);
                                    localDNSAndGWmacAddresses.Add(GetMACAddress(IPAddress.Parse(dnsIP)));
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

                foreach (ManagementObject oRGW in oRcGW)
                {
                    foreach (PropertyData property in oRGW.Properties)
                    {
                        if (property.Value != null)
                        {
                            foreach (string gwIP in (Array)property.Value)
                            {
                                if (!string.IsNullOrEmpty(gwIP))
                                {
                                    localDNSAndGWipAddresses.Add(gwIP);
                                    localDNSAndGWmacAddresses.Add(GetMACAddress(IPAddress.Parse(gwIP)));
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

            DescriptionAttribute attribute
                    = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))
                        as DescriptionAttribute;

            return attribute == null ? enumValue.ToString() : attribute.Description;
        }

        public static int GetEnumByDescriptionString(string description, Type enumType)
        {
            foreach (var field in enumType.GetFields())
            {
                DescriptionAttribute attribute
                    = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute == null)
                    continue;
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
                lv_Endpoints_SelectedEndpoint != null)
            {
                // IF ENDPOINT STATUS IN N/A, REMOVE 'DETAILS' FROM CONTEXT MENU
                if ((validationMethod == ValidationMethod.Protocol &&
                     lv_Endpoints_SelectedEndpoint.ResponseCode != status_NotAvailable &&
                     lv_Endpoints_SelectedEndpoint.ResponseCode != status_Error) ||
                    (validationMethod == ValidationMethod.Ping &&
                     lv_Endpoints_SelectedEndpoint.PingRoundtripTime != status_NotAvailable))
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
            tray_Refresh.Visible = false;
            tray_SpeedTest.Visible = false;
            tray_Separator_1.Visible = false;

            dialog_EndpointDetails = new EndpointDetailsDialog(
                (int)num_PingTimeout.Value * 1000,
                lv_Endpoints_SelectedEndpoint,
                imageList_Icons_32pix
                        .Images[GetStatusImageIndex(
                            lv_Endpoints_SelectedEndpoint.ResponseCode,
                            lv_Endpoints_SelectedEndpoint.PingRoundtripTime,
                            lv_Endpoints_SelectedEndpoint.ResponseMessage)]);

            dialog_EndpointDetails.ShowDialog();
            dialog_EndpointDetails = null;

            if (onClose)
            {
                Application.Exit();
            }

            tray_Refresh.Visible = true;
            tray_SpeedTest.Visible = true;
            tray_Separator_1.Visible = true;

        }

        private void toolStripMenuItem_AdminBrowse_Click(object sender, EventArgs e)
        {
            BrowseEndpoint_WindowsExplorer(
                new Uri(lv_Endpoints_SelectedEndpoint.ResponseAddress).Host + @"\C$",
                lv_Endpoints_SelectedEndpoint.LoginName,
                lv_Endpoints_SelectedEndpoint.LoginPass);
        }

        public void toolStripMenuItem_Browse_Click(object sender, EventArgs e)
        {
            BrowseEndpoint_WindowsExplorer(
                new Uri(lv_Endpoints_SelectedEndpoint.ResponseAddress).Host,
                lv_Endpoints_SelectedEndpoint.LoginName,
                lv_Endpoints_SelectedEndpoint.LoginPass);
        }

        public void toolStripMenuItem_HTTP_Click(object sender, EventArgs e)
        {
            OpenEndpoint_HTTP(lv_Endpoints_SelectedEndpoint);
        }

        public void toolStripMenuItem_FTP_Click(object sender, EventArgs e)
        {
            OpenEndpoint_FTP(lv_Endpoints_SelectedEndpoint);
        }

        public void toolStripMenuItem_RDP_Click(object sender, EventArgs e)
        {
            ConnectEndpoint_RDP(new Uri(lv_Endpoints_SelectedEndpoint.ResponseAddress).Host);
        }

        public void toolStripMenuItem_VNC_Click(object sender, EventArgs e)
        {
            ConnectEndpoint_VNC(new Uri(lv_Endpoints_SelectedEndpoint.ResponseAddress).Host);
        }

        public static void BrowseEndpoint_WindowsExplorer(
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
                ExceptionNotifier(null, exception, true);
            }
        }

        public static void ConnectEndpoint_RDP(string endpointAddress)
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
                ExceptionNotifier(null, exception, true);
            }
        }

        public static void ConnectEndpoint_VNC(string endpointAddress)
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
                    ExceptionNotifier(null, exception, true);
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

        public static void ConnectEndpoint_Putty(string endpointAddress)
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
                    ExceptionNotifier(null, exception, true);
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

        public static void BrowseEndpoint(
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
                ExceptionNotifier(null, exception, true);
            }
        }

        public static void OpenEndpoint_HTTP(EndpointDefinition endpoint)
        {
            NetworkCredential credentials = new NetworkCredential();

            Uri _endpointURI = new Uri(endpoint.Address);

            if (_endpointURI.Scheme != Uri.UriSchemeHttp ||
                _endpointURI.Scheme != Uri.UriSchemeHttps)            
            {
                // IF ENDPOINT IS NOT AN HTTP/HTTPS TYPE, PASS HTTP PROTOCOL PREFIX
                _endpointURI = new Uri(
                    Uri.UriSchemeHttp +
                    Uri.SchemeDelimiter +
                    _endpointURI.Host +
                    _endpointURI.PathAndQuery +
                    _endpointURI.Fragment);
            }

            if ((endpoint.Protocol.ToLower() == Uri.UriSchemeHttp ||
                 endpoint.Protocol.ToLower() == Uri.UriSchemeHttps) &&
                !string.IsNullOrEmpty(endpoint.LoginName) &&
                endpoint.LoginName != status_NotAvailable)
            {
                credentials = new NetworkCredential(endpoint.LoginName, endpoint.LoginPass);
            }

            BrowseEndpoint(
                _endpointURI.AbsoluteUri,
                null,
                credentials.UserName,
                credentials.Password);
        }

        public static void OpenEndpoint_FTP(EndpointDefinition endpoint)
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
            if (lv_Endpoints.SelectedItems.Count == 1)
            {
                if (e.IsSelected)
                {
                    lv_Endpoints_SelectedEndpoint = endpointsList.Where(endpointItem => endpointItem.Name == e.Item.Text).First();
                }
                else
                {
                    lv_Endpoints_SelectedEndpoint = null;
                }
            }
        }

        public static void StartBackgroundProcess(
                                        string fileName,
                                        string arguments,
                                        string userName,
                                        string userPassword)
        {
            try
            {
                ProcessStartInfo psInfo = new ProcessStartInfo();
                psInfo.FileName = fileName;
                psInfo.UseShellExecute = true;
                psInfo.ErrorDialog = true;

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
                MessageBox.Show(
                    ex.Message,
                    "Open Network Connection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        public static Image ResizeImage(Image imgPhoto, int Width, int Height)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = (Width / (float)sourceWidth);
            float nPercentH = (Height / (float)sourceHeight);

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
                if (lv_Endpoints.Sorting == SortOrder.Ascending)
                    lv_Endpoints.Sorting = SortOrder.Descending;
                else
                    lv_Endpoints.Sorting = SortOrder.Ascending;
            }

            lv_Endpoints.Sort();

            lv_Endpoints.ListViewItemSorter = new ListViewItemComparer(e.Column, lv_Endpoints.Sorting);

            listUpdating = false;
        }

        private void cb_ExportEndpointsStatus_JSON_CheckedChanged(object sender, EventArgs e)
        {
            if (!cb_ExportEndpointsStatus_JSON.Checked)
            {
                CloseFileStream(definitonsStatusExport_JSON_FileStream);
            }

            SaveConfiguration();
        }

        public void cb_ExportEndpointsStatus_XML_CheckedChanged(object sender, EventArgs e)
        {
            if (!cb_ExportEndpointsStatus_XML.Checked)
            {
                CloseFileStream(definitonsStatusExport_XML_FileStream);
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

        enum MatchType
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
                string _colorName;
                FindColor(color, out _colorName);
                colorName = _colorName + " (" + ColorTranslator.ToHtml(color) + ")";
            }

            return colorName;
        }

        static MatchType FindColor(Color colour, out string name)
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
                  difference = a * a + r * r + g * g + b * b;

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

        public void pb_ITNetwork_Click(object sender, EventArgs e)
        {
            BrowseEndpoint(
                "https://www.itnetwork.cz/csharp/winforms/csharp-windows-forms-zdrojove-kody/endpoint-status-checker",
                null,
                null,
                null);
        }

        public static void TextBox_SetPasswordVisibilty(
            TextBox textBox,
            bool passVisible)
        {
            textBox.UseSystemPasswordChar = passVisible;
        }

        public static string BuildUpConnectionString(
            EndpointDefinition endpointItem,
            string protocol)
        {
            // BUILD-UP CONNECTION STRING
            Uri endpointURI = new Uri(endpointItem.ResponseAddress);

            string connectionString =
                endpointURI.Authority +
                endpointURI.AbsolutePath;

            if (
                !string.IsNullOrEmpty(endpointItem.LoginName) &&
                endpointItem.LoginName != status_NotAvailable)
            {
                connectionString =
                    endpointItem.LoginName +
                    "@" +
                    connectionString;
            }

            return protocol + Uri.SchemeDelimiter + connectionString;
        }

        public void pb_FeatureRequest_Click(object sender, EventArgs e)
        {
            FeatureRequestDialog frDialog = new FeatureRequestDialog(
                featureRequest_senderEMailAddress,
                new List<string> { authorEmailAddress });

            frDialog.ShowDialog();
        }

        public void btn_EndpointsList_Click(object sender, EventArgs e)
        {
            if (File.Exists(endpointDefinitonsFile))
            {
                BrowseEndpoint(
                endpointDefinitonsFile,
                null,
                null,
                null);
            }
        }

        public void TIMER_ListAndLogsFilesWatcher_Tick(object sender, EventArgs e)
        {
            // ENDPOINTS LIST FILE
            btn_EndpointsList.Enabled = File.Exists(endpointDefinitonsFile);
            lbl_EndpointsList.Enabled = File.Exists(endpointDefinitonsFile);

            // ERROR(S) LOG(S) FILES
            btn_ConfigFile.Enabled = File.Exists(appConfigFile);
            lbl_ConfigFile.Enabled = File.Exists(appConfigFile);
        }

        public void btn_ConfigFile_Click(object sender, EventArgs e)
        {
            if (File.Exists(appConfigFile))
            {
                BrowseEndpoint(
                appConfigFile,
                null,
                null,
                null);
            }
        }

        public void pb_GitHub_Click(object sender, EventArgs e)
        {
            BrowseEndpoint(
                "https://github.com/ThePhOeNiX810815/Endpoint-Status-Checker/",
                null,
                null,
                null);
        }

        public void toolStripMenuItem_SSH_Click(object sender, EventArgs e)
        {
            ConnectEndpoint_Putty(new Uri(lv_Endpoints_SelectedEndpoint.ResponseAddress).Host);
        }

        public void pb_GitLab_Click(object sender, EventArgs e)
        {
            BrowseEndpoint(
                "https://gitlab.com/ThePhOeNiX810815/Endpoint-Status-Checker/",
                null,
                null,
                null);
        }

        public void pb_AppWebPage_Click(object sender, EventArgs e)
        {
            BrowseEndpoint(
                "https://endpoint-status-checker.webnode.com",
                null,
                null,
                null);
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

        public void pb_SoftPedia_Click(object sender, EventArgs e)
        {
            BrowseEndpoint(
                "https://www.softpedia.com/get/PORTABLE-SOFTWARE/Network/Endpoint-Status-Checker.shtml",
                null,
                null,
                null);
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

        public static string NotAvailable_IfNullorEmpty(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return status_NotAvailable;
            }
            else
            {
                return input;
            }
        }

        public void CheckerMainForm_Shown(object sender, EventArgs e)
        {
            // LOAD VALIDATION METHOD TYPES AND SELECT DEFAULT [PROTOCOL]
            comboBox_Validate.DataSource = Enum.GetValues(typeof(ValidationMethod));
            comboBox_Validate.SelectedIndex = 0;

            RestoreListViewColumnsWidthAndOrder();
            RestoreWindowSizeAndPosition();
            LoadConfiguration();
            LoadEndpointReferences();
        }

        public void tray_SpeedTest_Click(object sender, EventArgs e)
        {
            if (btn_SpeedTest.Enabled)
            {
                btn_SpeedTest_Click(this, null);
            }
        }

        public void cb_PingHost_CheckedChanged(object sender, EventArgs e)
        {
            if (!cb_PingHost.Checked)
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
            if (btn_Refresh.Enabled &&
                dialog_SpeedTest == null &&
                dialog_EndpointDetails == null)
            {
                TIMER_ContinuousRefresh.Enabled = false;
                btn_Refresh_Click(this, null);
            }
        }

        public void btn_SpeedTest_Click(object sender, EventArgs e)
        {
            tray_Refresh.Visible = false;
            tray_SpeedTest.Visible = false;
            tray_Separator_1.Visible = false;

            dialog_SpeedTest = new SpeedTestDialog();
            dialog_SpeedTest.ShowDialog();
            dialog_SpeedTest = null;

            if (onClose)
            {
                Application.Exit();
            }

            tray_Refresh.Visible = true;
            tray_SpeedTest.Visible = true;
            tray_Separator_1.Visible = true;
        }

        public void tray_Notifications_Enable_Click(object sender, EventArgs e)
        {
            cb_TrayBalloonNotify.Checked = true;
        }

        public void tray_Notifications_Disable_Click(object sender, EventArgs e)
        {
            cb_TrayBalloonNotify.Checked = false;
        }
    }

    public class ListViewItemComparer : IComparer
    {
        private int col;
        private SortOrder order;
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
            int returnVal = -1;
            returnVal = string.Compare(((ListViewItem)x).SubItems[col].Text,
                            ((ListViewItem)y).SubItems[col].Text);

            if (order == SortOrder.Descending)
                returnVal *= -1;

            return returnVal;
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
        public string HTTPcontentLenght { get; set; }
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
        string _networkName;

        public NetworkConnection(string networkName, NetworkCredential credentials)
        {
            _networkName = networkName;

            var netResource = new NetResource()
            {
                Scope = ResourceScope.GlobalNetwork,
                ResourceType = ResourceType.Disk,
                DisplayType = ResourceDisplaytype.Share,
                RemoteName = networkName
            };

            var userName = string.IsNullOrEmpty(credentials.Domain)
                ? credentials.UserName
                : string.Format(@"{0}\{1}", credentials.Domain, credentials.UserName);

            var result = WNetAddConnection2(
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