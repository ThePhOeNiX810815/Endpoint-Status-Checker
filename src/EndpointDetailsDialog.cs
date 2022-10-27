using Nager.PublicSuffix;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VirusTotalNET;
using VirusTotalNET.Objects;
using VirusTotalNET.Results;
using Whois.NET;
using Whois.NET.Internal;
using static EndpointChecker.CheckerMainForm;
using static EndpointChecker.Program;

namespace EndpointChecker
{
    public partial class EndpointDetailsDialog : Form
    {
        // WMI CONNECTION SCOPE
        private readonly ManagementScope wmiConnectionScope = new ManagementScope();

        // IP ADDRESS COMBOBOX TOOLTIP
        private readonly ToolTip ipAddress_ComboboxTooltip = new ToolTip();

        // LIVE PING ICON TOOLTIP
        private readonly ToolTip livePingIconTooltip = new ToolTip();

        // MAC VENDOR ICON TOOLTIP
        private readonly ToolTip macVendorIconTooltip = new ToolTip();

        // USER PASSWORD VISIBILITY ICON TOOLTIP
        private readonly ToolTip userPasswordVisibilityIconTooltip = new ToolTip();

        // VIRUSTOTAL PERMALINK TEXTBOX TOOLTIP
        private readonly ToolTip virusTotal_PermalinkTextboxTooltip = new ToolTip();

        // WEAK ETAG ICON TOOLTIP
        private readonly ToolTip httpInfo_WeakETagIconTooltip = new ToolTip();

        // VIRUSTOTAL RESULT
        private UrlScanResult virusTotal_ScanResult = null;

        // MAC VENDOR WEBPAGE URL
        private string macVendorWebPage = string.Empty;

        // WMI MANAGEMENT CLASSES ENUM
        private enum WMIManagementClass
        {
            BaseBoard,
            Battery,
            BIOS,
            Bus,
            CDROMDrive,
            DiskDrive,
            DMAChannel,
            Fan,
            FloppyController,
            FloppyDrive,
            IDEController,
            Keyboard,
            MemoryDevice,
            NetworkAdapter,
            OperatingSystem,
            OnBoardDevice,
            ParallelPort,
            PhysicalMemory,
            PortConnector,
            Process,
            Printer,
            POTSModem,
            Processor,
            SCSIController,
            SerialPort,
            SerialPortConfiguration,
            SoundDevice,
            SystemEnclosure,
            TapeDrive,
            TemperatureProbe,
            USBController,
            USBHub,
            VideoController,
            VoltageProbe
        }

        public EndpointDefinition _selectedEndpoint;
        public Image _selectedEndpointIcon;
        public int _pingTimeout;

        // STANDARD PORTS LIST DICTIONARY
        private readonly Dictionary<int, string> portsList = new Dictionary<int, string>
        {
            { 20, "File Transfer Protocol (FTP) data" },
            { 21, "File Transfer Protocol (FTP) control" },
            { 22, "Secure Shell (SSH), SCP, SFTP" },
            { 23, "Telnet" },
            { 25, "Simple Mail Transfer Protocol (SMTP)" },
            { 53, "Domain Name System (DNS)" },
            { 69, "Trivial File Transfer Protocol (TFTP)" },
            { 70, "Gopher" },
            { 80, "Hypertext Transfer Protocol (HTTP)" },
            { 88, "Kerberos" },
            { 109, "Post Office Protocol, version 2 (POP2)" },
            { 110, "Post Office Protocol, version 3 (POP3)" },
            { 115, "Simple File Transfer Protocol (SFTP)" },
            { 123, "Network Time Protocol (NTP)" },
            { 135, "Microsoft RPC Locator, DCOM" },
            { 143, "Internet Message Access Protocol (IMAP)" },
            { 161, "Simple Network Management Protocol (SNMP)" },
            { 170, "Print server" },
            { 220, "Internet Message Access Protocol (IMAP) Version 3" },
            { 389, "Lightweight Directory Access Protocol (LDAP)" },
            { 401, "Uninterruptible power supply (UPS)" },
            { 443, "Hypertext Transfer Protocol over TLS/SSL (HTTPS)" },
            { 445, "Microsoft-DS Active Directory, Windows shares, SMB" },
            { 465, "Authenticated SMTP over TLS/SSL (SMTPS)" },
            { 512, "Rexec, Remote Process Execution" },
            { 530, "Remote procedure call (RPC)" },
            { 636, "Lightweight Directory Access Protocol over TLS/SSL (LDAPS)" },
            { 989, "FTPS Protocol (data), FTP over TLS/SSL" },
            { 990, "FTPS Protocol (control), FTP over TLS/SSL" },
            { 992, "Telnet protocol over TLS/SSL" },
            { 993, "Internet Message Access Protocol over TLS/SSL (IMAPS)" },
            { 995, "Post Office Protocol 3 over TLS/SSL (POP3S)" },
            { 1080, "SOCKS proxy" },
            { 1194, "OpenVPN" },
            { 1433, "Microsoft SQL Server" },
            { 1434, "Microsoft SQL Monitor" },
            { 1512, "Microsoft Windows Internet Name Service (WINS)" },
            { 1688, "Microsoft Key Management Service (KMS)" },
            { 1723, "Point-to-Point Tunneling Protocol (PPTP)" },
            { 1900, "Simple Service Discovery Protocol (SSDP), UPnP" },
            { 2049, "Network File System (NFS)" },
            { 2483, "Oracle database listener (insecure)" },
            { 2484, "Oracle database listener (over SSL)" },
            { 3020, "Common Internet File System (CIFS)" },
            { 3306, "MySQL" },
            { 3389, "Microsoft Remote Desktop" },
            { 3690, "Subversion (SVN) version control system" },
            { 3702, "Web Services Dynamic Discovery (WS-Discovery)" },
            { 5432, "PostgreSQL" },
            { 5800, "VNC HTTP"},
            { 5900, "VNC"},
            { 5938, "TeamViewer remote desktop" },
            { 6600, "Microsoft Hyper-V Live" },
            { 8080, "HTTP Alternative, Apache Tomcat, JIRA" },
            { 8090, "Confluence" },
            { 8883, "Secure MQTT (MQTT over TLS)" },
            { 9001, "ETL Service Manager, SharePoint, Cisco xRemote, TOR" },
            { 9800, "WebDAV Source" },
            { 10000, "Webmin" }
        };

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        public EndpointDetailsDialog(int pingTimeout, EndpointDefinition selectedEndpoint, Image selectedEndpointImage)
        {
            InitializeComponent();

            // COMMON EXCEPTION HANDLERS
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);
            Application.ThreadException += new ThreadExceptionEventHandler(ThreadExceptionHandler);

            // SET LOCAL VARIABLES
            _selectedEndpoint = selectedEndpoint;
            _selectedEndpointIcon = selectedEndpointImage;
            _pingTimeout = pingTimeout;
        }

        public void EndpointDetailsDialog_Shown(object sender, EventArgs e)
        {
            InitializeControls();
            LoadProperties();
        }

        public void InitializeControls()
        {
            // HIDE PROTOCOL SPECIFIC FIELDS FROM 'MAIN INFO' TAB, IF VALIDATION METHOD IS 'PING ONLY'
            if (validationMethod == ValidationMethod.Ping)
            {
                lbl_StatusCode.Visible = false;
                pb_StatusIcon.Visible = false;
                tb_StatusCode.Visible = false;
                lbl_StatusMessage.Visible = false;
                tb_StatusMessage.Visible = false;
            }

            // ADD RESIZED [16x16] IMAGES TO TAB CONTROL IMAGE LIST
            imageList_Tabs.Images.Add(ResizeImage(Properties.Resources.browse_FTP, 16, 16));
            imageList_Tabs.Images.Add(ResizeImage(Properties.Resources.browse_HTTP, 16, 16));
            imageList_Tabs.Images.Add(ResizeImage(Properties.Resources.browse_Share, 16, 16));
            imageList_Tabs.Images.Add(ResizeImage(Properties.Resources.pingHop, 16, 16));
            imageList_Tabs.Images.Add(ResizeImage(Properties.Resources.requestHeaderProperty, 16, 16));
            imageList_Tabs.Images.Add(ResizeImage(Properties.Resources.responseHeaderProperty, 16, 16));
            imageList_Tabs.Images.Add(ResizeImage(Properties.Resources.information.ToBitmap(), 16, 16));
            imageList_Tabs.Images.Add(ResizeImage(Properties.Resources.wmi, 16, 16));
            imageList_Tabs.Images.Add(ResizeImage(Properties.Resources.port, 16, 16));
            imageList_Tabs.Images.Add(ResizeImage(Properties.Resources.whoIs, 16, 16));
            imageList_Tabs.Images.Add(ResizeImage(Properties.Resources.geoLocation, 16, 16));
            imageList_Tabs.Images.Add(ResizeImage(Properties.Resources.ssl, 16, 16));
            imageList_Tabs.Images.Add(ResizeImage(Properties.Resources.virusTotal, 16, 16));
            imageList_Tabs.Images.Add(ResizeImage(Properties.Resources.category, 16, 16));
            imageList_Tabs.Images.Add(ResizeImage(Properties.Resources.link_16x16, 16, 16));
            imageList_Tabs.Images.Add(ResizeImage(Properties.Resources.main, 16, 16));

            // ASSIGN IMAGES TO TABS
            tabPage_FTPInfo.ImageIndex = 0;
            tabPage_HTTPInfo.ImageIndex = 1;
            tabPage_NetworkShares.ImageIndex = 2;
            tabPage_TraceRoute.ImageIndex = 3;
            tabPage_HTTPRequestHeaders.ImageIndex = 4;
            tabPage_HTTPResponseHeaders.ImageIndex = 5;
            tabPage_HTMLInfo.ImageIndex = 6;
            tabPage_WMI_ComputerInfo.ImageIndex = 7;
            tabPage_Ports.ImageIndex = 8;
            tabPage_WhoIs.ImageIndex = 9;
            tabPage_GeoLocation.ImageIndex = 10;
            tabPage_SSLCertificate.ImageIndex = 11;
            tabPage_VirusTotal.ImageIndex = 12;
            tabPage_PageCategory.ImageIndex = 13;
            tabPage_PageLinks.ImageIndex = 14;
            tabPage_MainInfo.ImageIndex = 15;

            // ASSIGN RESIZED IMAGES TO BUTTONS PICTURE BOX CONTROLS
            pb_Browse_WindowsExplorer.Image = ResizeImage(Properties.Resources.browse_Share, pb_Browse_WindowsExplorer.Width, pb_Browse_WindowsExplorer.Height);
            pb_AdminBrowse_WindowsExplorer.Image = ResizeImage(Properties.Resources.browse_Admin_Share, pb_AdminBrowse_WindowsExplorer.Width, pb_AdminBrowse_WindowsExplorer.Height);
            pb_FTP.Image = ResizeImage(Properties.Resources.browse_FTP, pb_FTP.Width, pb_FTP.Height);
            pb_HTTP.Image = ResizeImage(Properties.Resources.browse_HTTP, pb_HTTP.Width, pb_HTTP.Height);
            pb_RDP.Image = ResizeImage(Properties.Resources.connect_RDP, pb_RDP.Width, pb_RDP.Height);
            pb_VNC.Image = ResizeImage(Properties.Resources.connect_VNC, pb_VNC.Width, pb_VNC.Height);
            pb_SSH.Image = ResizeImage(Properties.Resources.ssh_2, pb_SSH.Width, pb_SSH.Height);

            ipAddress_ComboboxTooltip.SetToolTip(cb_IPAddress, "Click to select desired IP Address");
            livePingIconTooltip.SetToolTip(pb_PingRefresh, "Click for Live 1 second Ping Refresh");
            virusTotal_PermalinkTextboxTooltip.SetToolTip(tb_VirusTotal_Permalink, "Click to open Scan Result in browser");
            userPasswordVisibilityIconTooltip.SetToolTip(pb_ShowPassword, "Click to show User Password");
            httpInfo_WeakETagIconTooltip.SetToolTip(
                pb_HTTPInfo_WeakETag,
                "Weak validator is used." +
                Environment.NewLine +
                "Weak eTags are easy to generate," +
                Environment.NewLine +
                "but are far less useful for comparisons");
        }

        public void LoadProperties()
        {
            // STATUS IMAGE
            pb_StatusIcon.Image = _selectedEndpointIcon;

            // FAVICON
            if (_selectedEndpoint.Protocol.ToLower() == Uri.UriSchemeHttp ||
                _selectedEndpoint.Protocol.ToLower() == Uri.UriSchemeHttps)
            {
                GetWebsiteFavicon(_selectedEndpoint.Address);
            }

            // DIALOG WINDOW TITLE
            Text += " - " + _selectedEndpoint.Name;

            // MAIN PROPERTIES
            tb_EndpointName.Text = _selectedEndpoint.Name;
            tb_EndpointURL.Text = _selectedEndpoint.Address;
            cb_IPAddress.Items.AddRange(_selectedEndpoint.IPAddress);
            cb_IPAddress.SelectedIndex = 0;
            tb_PingTime.Text = _selectedEndpoint.PingRoundtripTime;
            tb_Port.Text = _selectedEndpoint.Port;
            tb_Protocol.Text = _selectedEndpoint.Protocol;
            tb_ResponseTime.Text = _selectedEndpoint.ResponseTime;

            tb_ResponseURL.Text = _selectedEndpoint.ResponseAddress.Replace(
                                            _selectedEndpoint.LoginName + ":" +
                                            _selectedEndpoint.LoginPass + "@",
                                            string.Empty);

            tb_StatusCode.Text = _selectedEndpoint.ResponseCode;
            tb_StatusMessage.Text = _selectedEndpoint.ResponseMessage;
            tb_UserName.Text = _selectedEndpoint.LoginName;
            tb_UserPassword.Text = _selectedEndpoint.LoginPass;

            // SET TABS [PROTOCOL SPECIFIC]
            if (_selectedEndpoint.Protocol.ToLower() == Uri.UriSchemeFtp &&
                validationMethod != ValidationMethod.Ping)
            {
                // ADD FTP TAB PAGE
                if (_selectedEndpoint.FTPBannerMessage != status_NotAvailable ||
                    _selectedEndpoint.FTPExitMessage != status_NotAvailable ||
                    _selectedEndpoint.FTPStatusDescription != status_NotAvailable ||
                    _selectedEndpoint.FTPWelcomeMessage != status_NotAvailable)
                {
                    tb_FTPInfo_WelcomeMessage.Text = _selectedEndpoint.FTPWelcomeMessage;
                    tb_FTPInfo_BannerMessage.Text = _selectedEndpoint.FTPBannerMessage;
                    tb_FTPInfo_ExitMessage.Text = _selectedEndpoint.FTPExitMessage;
                    tb_FTPInfo_StatusDescription.Text = _selectedEndpoint.FTPStatusDescription;

                    tabControl.TabPages.Add(tabPage_FTPInfo);
                }
            }
            else if (_selectedEndpoint.Protocol.ToLower() == Uri.UriSchemeHttp ||
                     _selectedEndpoint.Protocol.ToLower() == Uri.UriSchemeHttps)
            {
                if (validationMethod != ValidationMethod.Ping)
                {
                    // HTTP REQUEST HEADERS
                    if (_selectedEndpoint.HTTPRequestHeaders.PropertyItem.Count > 0)
                    {
                        foreach (Property header in _selectedEndpoint.HTTPRequestHeaders.PropertyItem)
                        {
                            ListViewItem headerItem = new ListViewItem(header.ItemName, 4);
                            _ = headerItem.SubItems.Add(header.ItemValue);
                            _ = lv_HTTP_RequestHeaders.Items.Add(headerItem);
                        }

                        // AUTO SIZE 'KEY' COLUMN BY LONGEST ITEM
                        ch_HTTPrequest_Headers_Key.Width = -1;

                        // ADD HTTP REQUEST HEADERS TAB PAGE
                        tabControl.TabPages.Add(tabPage_HTTPRequestHeaders);
                    }

                    // HTTP RESPONSE HEADERS
                    if (_selectedEndpoint.HTTPResponseHeaders.PropertyItem.Count > 0)
                    {
                        foreach (Property header in _selectedEndpoint.HTTPResponseHeaders.PropertyItem)
                        {
                            ListViewItem headerItem = new ListViewItem(header.ItemName, 5);
                            _ = headerItem.SubItems.Add(header.ItemValue);
                            _ = lv_HTTP_ResponseHeaders.Items.Add(headerItem);
                        }

                        // AUTO SIZE 'KEY' COLUMN BY LONGEST ITEM
                        ch_HTTPresponse_Headers_Key.Width = -1;

                        // ADD HTTP RESPONSE HEADERS TAB PAGE
                        tabControl.TabPages.Add(tabPage_HTTPResponseHeaders);
                    }

                    tb_PageInfo_HTMLTitle.Text = _selectedEndpoint.HTMLTitle;
                    tb_PageInfo_HTMLDescription.Text = _selectedEndpoint.HTMLDescription;
                    tb_PageInfo_Author.Text = _selectedEndpoint.HTMLAuthor;
                    tb_PageInfo_HTMLEncoding.Text = GetEncodingName(_selectedEndpoint.HTMLencoding);
                    tb_PageInfo_ContentLanguage.Text = _selectedEndpoint.HTMLContentLanguage;

                    // SET THEME COLOR CONTROLS
                    if (_selectedEndpoint.HTMLThemeColor != Color.Empty)
                    {
                        tb_PageInfo_ThemeColor.Text = GetKnownColorNameString(_selectedEndpoint.HTMLThemeColor);
                        pb_PageInfo_ThemeColor.BackColor = _selectedEndpoint.HTMLThemeColor;
                        pb_PageInfo_ThemeColor.Visible = true;
                    }
                    else
                    {
                        tb_PageInfo_ThemeColor.Text = status_NotAvailable;
                    }

                    // HTML INFO
                    if (tb_PageInfo_HTMLTitle.Text != status_NotAvailable ||
                        tb_PageInfo_Author.Text != status_NotAvailable ||
                        tb_PageInfo_HTMLDescription.Text != status_NotAvailable ||
                        tb_PageInfo_ContentLanguage.Text != status_NotAvailable ||
                        tb_PageInfo_ThemeColor.Text != status_NotAvailable ||
                        _selectedEndpoint.HTMLMetaInfo.PropertyItem.Count > 0)
                    {
                        foreach (Property meta in _selectedEndpoint.HTMLMetaInfo.PropertyItem)
                        {
                            ListViewItem metaItem = new ListViewItem(meta.ItemName);

                            // SET TAG-SPECIFIC ICON
                            if (meta.ItemName.ToLower().StartsWith("fb:"))
                            {
                                // FACEBOOK
                                metaItem.ImageIndex = 15;
                            }
                            else if (meta.ItemName.ToLower().StartsWith("twitter:"))
                            {
                                // TWITTER
                                metaItem.ImageIndex = 16;
                            }
                            else if (meta.ItemName.ToLower().StartsWith("og:"))
                            {
                                // OPEN GRAPH
                                metaItem.ImageIndex = 17;
                            }
                            else if (meta.ItemName.ToLower().StartsWith("al:"))
                            {
                                // APPLICATION LINK
                                metaItem.ImageIndex = 18;
                            }
                            else if (meta.ItemName.ToLower().StartsWith("article:"))
                            {
                                // ARTICLE
                                metaItem.ImageIndex = 20;
                            }
                            else if (meta.ItemName.ToLower().StartsWith("csrf-"))
                            {
                                // RAILS
                                metaItem.ImageIndex = 21;
                            }
                            else if (meta.ItemName.ToLower().StartsWith("msapplication-"))
                            {
                                // MICROSOFT
                                metaItem.ImageIndex = 22;
                            }
                            else if (meta.ItemName.ToLower().StartsWith("geo.") ||
                                     meta.ItemName.ToLower() == "icbm")
                            {
                                // LOCATION
                                metaItem.ImageIndex = 23;
                            }
                            else if (meta.ItemName.ToLower().StartsWith("apple"))
                            {
                                // APPLE
                                metaItem.ImageIndex = 24;
                            }
                            else if (meta.ItemName.ToLower().StartsWith("google"))
                            {
                                // GOOGLE
                                metaItem.ImageIndex = 25;
                            }
                            else if (meta.ItemName.ToLower() == "generator")
                            {
                                // PAGE GENERATOR TAG
                                metaItem.ImageIndex = 19;
                            }
                            else if (meta.ItemName.ToLower() == "robots")
                            {
                                // ROBOTS
                                metaItem.ImageIndex = 26;
                            }
                            else if (meta.ItemName.ToLower().StartsWith("x-"))
                            {
                                // X TAG
                                metaItem.ImageIndex = 29;
                            }
                            else if (meta.ItemName.ToLower().StartsWith("msvalidate"))
                            {
                                // BING TAG
                                metaItem.ImageIndex = 30;
                            }
                            else if (meta.ItemName.ToLower() == "author" ||
                                     meta.ItemName.ToLower() == "web_author" ||
                                     meta.ItemName.ToLower() == "autor")
                            {
                                // AUTHOR TAG
                                metaItem.ImageIndex = 31;
                            }
                            else if (meta.ItemName.ToLower() == "description")
                            {
                                // DESCRIPTION TAG
                                metaItem.ImageIndex = 32;
                            }
                            else if (meta.ItemName.ToLower().StartsWith("norton"))
                            {
                                // NORTON TAG
                                metaItem.ImageIndex = 33;
                            }
                            else
                            {
                                // DEFAULT
                                metaItem.ImageIndex = 6;
                            }

                            _ = metaItem.SubItems.Add(meta.ItemValue);
                            _ = lv_HTML_MetaInfo.Items.Add(metaItem);
                        }

                        // AUTO SIZE 'META TAG' COLUMN BY LONGEST ITEM
                        ch_HTMLMeta_MetaTag.Width = -1;

                        // ADD HTML TAB PAGE
                        tabControl.TabPages.Add(tabPage_HTMLInfo);
                    }

                    // HTTP INFO
                    tb_HTTPInfo_ServerName.Text = _selectedEndpoint.ServerID;
                    tb_HTTPInfo_AutoRedirects.Text = _selectedEndpoint.HTTPautoRedirects;
                    tb_HTTPInfo_ContentType.Text = _selectedEndpoint.HTTPcontentType;
                    tb_HTTPInfo_Encoding.Text = GetEncodingName(_selectedEndpoint.HTTPencoding);
                    tb_HTTPInfo_ContentLenght.Text = _selectedEndpoint.HTTPcontentLenght;
                    tb_HTTPInfo_Expires.Text = _selectedEndpoint.HTTPexpires;
                    tb_HTTPInfo_ETag.Text = _selectedEndpoint.HTTPetag.TrimStart('W').TrimStart('/').TrimStart('"').TrimEnd('"');
                    pb_HTTPInfo_WeakETag.Visible = _selectedEndpoint.HTTPetag.ToLower().StartsWith("w/");

                    if (tb_HTTPInfo_ServerName.Text != status_NotAvailable ||
                        tb_HTTPInfo_AutoRedirects.Text != status_NotAvailable ||
                        tb_HTTPInfo_ContentType.Text != status_NotAvailable ||
                        tb_HTTPInfo_Encoding.Text != status_NotAvailable ||
                        tb_HTTPInfo_ContentLenght.Text != status_NotAvailable ||
                        tb_HTTPInfo_Expires.Text != status_NotAvailable ||
                        tb_HTTPInfo_ETag.Text != status_NotAvailable)
                    {
                        // ADD HTTP TAB PAGE
                        tabControl.TabPages.Add(tabPage_HTTPInfo);
                    }

                    // SSL CERTIFICATE INFO
                    if (_selectedEndpoint.SSLCertificateProperties.PropertyItem.Count > 0)
                    {
                        foreach (Property sslProperty in _selectedEndpoint.SSLCertificateProperties.PropertyItem)
                        {
                            ListViewItem sslItem = new ListViewItem(sslProperty.ItemName, 12);
                            _ = sslItem.SubItems.Add(sslProperty.ItemValue);
                            _ = lv_SSLCertificate.Items.Add(sslItem);
                        }

                        // ADD SSL CERTIFICATE INFO PAGE
                        tabControl.TabPages.Add(tabPage_SSLCertificate);
                    }

                    // HTML PAGE LINKS
                    if (_selectedEndpoint.HTMLPageLinks.PropertyItem.Count > 0)
                    {
                        foreach (Property pageLink in _selectedEndpoint.HTMLPageLinks.PropertyItem)
                        {
                            ListViewItem pageLinkItem = new ListViewItem("UNKNOWN", 14);
                            _ = pageLinkItem.SubItems.Add(pageLink.ItemName);
                            _ = pageLinkItem.SubItems.Add(pageLink.ItemValue);
                            _ = lv_PageLinks.Items.Add(pageLinkItem);
                        }

                        // ADD HTML PAGE LINKS PAGE
                        tabControl.TabPages.Add(tabPage_PageLinks);

                        // SET PAGE LINKS COUNT MESSAGE
                        lbl_LinksCountMessage.Text = lv_PageLinks.Items.Count + " links has been found within this webpage. Doubleclick any to open in web browser.";
                    }
                }

                // VIRUSTOTAL SCAN
                tabControl.TabPages.Add(tabPage_VirusTotal);

                if (!string.IsNullOrEmpty(apiKey_VirusTotal))
                {
                    btn_VirusTotal_Refresh_Click(this, null);
                }
            }

            // SET TABS [COMMON]
            // NETWORK SHARES
            if (!_selectedEndpoint.NetworkShare[0].Contains(status_NotAvailable))
            {
                foreach (string netShare in _selectedEndpoint.NetworkShare)
                {
                    string[] valueStringArray = netShare.Split(new char[]
                                        {
                                            ']',
                                            '('
                                        });

                    string shareType = valueStringArray[0]
                        .Replace("[", string.Empty)
                        .TrimStart()
                        .TrimEnd();

                    // SHARED FOLDER
                    int typeImageIndex = 1;

                    if (shareType.ToLower().Contains("admin"))
                    {
                        // ADMIN SHARE
                        typeImageIndex = 0;
                    }
                    else if (shareType.ToLower().Contains("printer"))
                    {
                        // SHARED PRINTER
                        typeImageIndex = 2;
                    }

                    ListViewItem netShareItem = new ListViewItem(shareType, typeImageIndex);

                    _ = netShareItem.SubItems.Add(valueStringArray[1].TrimStart().TrimEnd());

                    if (valueStringArray.Length > 2)
                    {
                        _ = netShareItem.SubItems.Add(valueStringArray[2].Replace(")", string.Empty).TrimStart().TrimEnd());
                    }

                    netShareItem.ToolTipText = "Mouse doubleclick to browse shared resource";

                    _ = lv_NetworkShares.Items.Add(netShareItem);
                }

                // ADD NETWORK SHARES TAB PAGE
                tabControl.TabPages.Add(tabPage_NetworkShares);
            }

            // GET WHOIS INFORMATION
            GetWhoIsInfo();

            // GET PAGE CATEGORY LIST
            GetPageCategoryListFromHTMLMeta();
        }

        public void GetWebsiteFavicon(string websiteURL)
        {
            string fallbackGoogleResolveURL = "http://www.google.com/s2/favicons?domain=";

            NewBackgroundThread(() =>
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(websiteURL + "/favicon.ico");
                httpWebRequest.Method = WebRequestMethods.Http.Get;
                httpWebRequest.UserAgent = http_UserAgent;
                httpWebRequest.Timeout = 5000;
                httpWebRequest.ReadWriteTimeout = 5000;

                HttpWebResponse httpWebResponse = null;

                try
                {
                    httpWebResponse = GetHTTPWebResponse(httpWebRequest, 5);
                    Stream stream = httpWebResponse.GetResponseStream();
                    pb_Favicon.Image = new Bitmap(stream);
                }
                catch
                {
                    if (!websiteURL.Contains(fallbackGoogleResolveURL))
                    {
                        GetWebsiteFavicon(fallbackGoogleResolveURL + websiteURL);
                    }
                }
                finally
                {
                    if (httpWebResponse != null)
                    {
                        httpWebResponse.Close();

                        GC.Collect();
                    }
                }
            });
        }

        public void traceRoute_RouteNodeFound(object sender, VRK.Net.RouteNodeFoundEventArgs e)
        {
            if (e.Node.Address.ToString() != "0.0.0.0")
            {
                ListViewItem item = new ListViewItem((lv_RouteList.Items.Count + 1).ToString(), 3);
                _ = item.SubItems.Add(e.Node.Address.ToString());
                _ = item.SubItems.Add(e.Node.DNSName);
                _ = item.SubItems.Add(e.Node.RoundTripTime);
                item.ToolTipText = "Mouse doubleclick to open in browser [HTTP]";

                ThreadSafeInvoke(() =>
                {
                    _ = lv_RouteList.Items.Add(item);
                });
            }
        }

        public void btn_TraceRoute_Refresh_Click(object sender, EventArgs e)
        {
            btn_TraceRoute_Refresh.Enabled = false;
            pb_TraceRouteProgress.Visible = true;
            traceRoute.HostNameOrAddress = cb_IPAddress.GetItemText(cb_IPAddress.SelectedItem);
            lv_RouteList.Items.Clear();

            NewBackgroundThread(() =>
            {
                try
                {
                    traceRoute.Trace();
                }
                catch
                {
                    traceRoute_Done(this, null);
                }
            });
        }

        public void traceRoute_Done(object sender, EventArgs e)
        {
            ThreadSafeInvoke(() =>
            {
                btn_TraceRoute_Refresh.Enabled = true;
                pb_TraceRouteProgress.Visible = false;
            });
        }

        public void lv_RouteList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lv_RouteList.SelectedItems.Count == 1)
            {
                string address = Uri.UriSchemeHttp + Uri.SchemeDelimiter + lv_RouteList.SelectedItems[0].SubItems[2].Text;

                try
                {
                    StartBackgroundProcess(
                                                 address,
                                                 null,
                                                 null,
                                                 null);
                }
                catch (Exception exception)
                {
                    ExceptionNotifier(this, exception, string.Empty, true);
                }
            }
        }

        public void lv_NetworkShares_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lv_NetworkShares.SelectedItems.Count == 1)
            {
                string share = @"\\" + _selectedEndpoint.IPAddress[0] + @"\" + lv_NetworkShares.SelectedItems[0].SubItems[1].Text;

                try
                {
                    StartBackgroundProcess(
                                                 share,
                                                 null,
                                                 _selectedEndpoint.LoginName,
                                                 _selectedEndpoint.LoginPass);
                }
                catch (Exception exception)
                {
                    ExceptionNotifier(this, exception, string.Empty, true);
                }
            }
        }

        public void pb_Browse_WindowsExplorer_MouseClick(object sender, MouseEventArgs e)
        {
            BrowseEndpoint_WindowsExplorer(
                new Uri(_selectedEndpoint.ResponseAddress).Host,
                _selectedEndpoint.LoginName,
                _selectedEndpoint.LoginPass);
        }

        public void pb_AdminBrowse_WindowsExplorer_MouseClick(object sender, MouseEventArgs e)
        {
            BrowseEndpoint_WindowsExplorer(
                new Uri(_selectedEndpoint.ResponseAddress).Host + @"\C$",
                _selectedEndpoint.LoginName,
                _selectedEndpoint.LoginPass);
        }

        public void pb_RDP_MouseClick(object sender, MouseEventArgs e)
        {
            ConnectEndpoint_RDP(
                new Uri(_selectedEndpoint.ResponseAddress).Host);
        }

        public void pb_VNC_MouseClick(object sender, MouseEventArgs e)
        {
            ConnectEndpoint_VNC(
                new Uri(_selectedEndpoint.ResponseAddress).Host);
        }

        public void pb_HTTP_MouseClick(object sender, MouseEventArgs e)
        {
            OpenEndpoint_HTTP(_selectedEndpoint);
        }

        public void pb_FTP_MouseClick(object sender, MouseEventArgs e)
        {
            OpenEndpoint_FTP(_selectedEndpoint);
        }

        public void pb_PingRefresh_Click(object sender, EventArgs e)
        {
            if (!TIMER_PingRefresh.Enabled)
            {
                // START
                TIMER_PingRefresh.Enabled = true;
                TIMER_PingRefresh.Start();
                pb_PingRefresh.Image = Properties.Resources.loadingProgressWheel;
            }
            else
            {
                // STOP
                TIMER_PingRefresh.Stop();
                TIMER_PingRefresh.Enabled = false;
                pb_PingRefresh.Image = Properties.Resources.refresh.ToBitmap();
                tb_PingTime.Text = _selectedEndpoint.PingRoundtripTime;
            }
        }

        public void TIMER_PingRefresh_Tick(object sender, EventArgs e)
        {
            NewBackgroundThread(() =>
            {
                try
                {
                    string pingRoundtripTime = GetPingTime(new Uri(_selectedEndpoint.ResponseAddress).Host, _pingTimeout, 3);

                    if (!string.IsNullOrEmpty(pingRoundtripTime))
                    {
                        ThreadSafeInvoke(() =>
                        {
                            tb_PingTime.Text = pingRoundtripTime;
                            tb_PingTime.Text += " (1s refresh)";
                        });
                    }
                }
                catch
                {
                }
            });
        }

        public void GetComputerInformationByWMI()
        {
            foreach (WMIManagementClass managementClass in Enum.GetValues(typeof(WMIManagementClass)))
            {
                try
                {
                    TreeNode classNode = new TreeNode(managementClass.ToString(), 7, 7);
                    SelectQuery wmiQuery = new SelectQuery("SELECT * FROM Win32_" + managementClass.ToString());
                    ManagementObjectSearcher wmiSearchProcedure = new ManagementObjectSearcher(wmiConnectionScope, wmiQuery);

                    foreach (ManagementObject managementObject in wmiSearchProcedure.Get())
                    {
                        string managementObjectNodeText = string.Empty;

                        managementObjectNodeText = managementObject.Properties["Name"].Value.ToString() ==
                            managementObject.Properties["Caption"].Value.ToString()
                            ? managementObject.Properties["Name"].Value.ToString()
                            : managementObject.Properties["Name"].Value.ToString() +
                                " (" +
                                managementObject.Properties["Caption"].Value.ToString() +
                                ")";

                        TreeNode managementObjectNode = new TreeNode(managementObjectNodeText, 8, 8);

                        foreach (PropertyData propertyData in managementObject.Properties)
                        {
                            if (propertyData.Value != null &&
                                !string.IsNullOrEmpty(propertyData.Value.ToString()))
                            {
                                TreeNode dataNode = new TreeNode(propertyData.Name)
                                {
                                    ImageIndex = 11,
                                    SelectedImageIndex = 11
                                };

                                switch (propertyData.Value.GetType().ToString())
                                {
                                    case "System.String[]":
                                        string[] str = (string[])propertyData.Value;

                                        foreach (string st in str)
                                        {
                                            _ = dataNode.Nodes.Add(new TreeNode(st.ToString()));
                                        }

                                        break;

                                    case "System.UInt16[]":
                                        ushort[] shortData = (ushort[])propertyData.Value;

                                        foreach (ushort st in shortData)
                                        {
                                            _ = dataNode.Nodes.Add(new TreeNode(st.ToString()));
                                        }

                                        break;

                                    default:
                                        _ = dataNode.Nodes.Add(propertyData.Value.ToString());
                                        break;
                                }

                                _ = managementObjectNode.Nodes.Add(dataNode);
                            }
                        }

                        _ = classNode.Nodes.Add(managementObjectNode);
                    }

                    if (classNode.Nodes.Count > 0)
                    {
                        ThreadSafeInvoke(() =>
                        {
                            _ = treeView_ComputerInfo.Nodes.Add(classNode);
                        });
                    }
                }
                catch
                {
                }
            }

            ThreadSafeInvoke(() =>
            {
                btn_WMIInfo_Refresh.Enabled = true;
                pb_WMIInfoProgress.Visible = false;
            });
        }

        public void btn_WMIInfo_Refresh_Click(object sender, EventArgs e)
        {
            string ipAddress = cb_IPAddress.GetItemText(cb_IPAddress.SelectedItem);

            lbl_WMI_Exception.Visible = false;
            treeView_ComputerInfo.Nodes.Clear();
            btn_WMIInfo_Refresh.Enabled = false;
            pb_WMIInfoProgress.Visible = true;

            NewBackgroundThread(() =>
            {
                wmiConnectionScope.Path = new ManagementPath(@"\\" + ipAddress + @"\root\CIMV2");

                wmiConnectionScope.Options = new ConnectionOptions
                {
                    Impersonation = ImpersonationLevel.Impersonate,
                    Authentication = AuthenticationLevel.Default,
                    EnablePrivileges = true
                };

                if (!IsLocalHost(new Uri(_selectedEndpoint.ResponseAddress).Host) &&
                    _selectedEndpoint.LoginName != status_NotAvailable &&
                    _selectedEndpoint.LoginPass != status_NotAvailable)
                {
                    // PASS CREDENTIALS IF SPECIFIED
                    wmiConnectionScope.Options.Username = _selectedEndpoint.LoginName;
                    wmiConnectionScope.Options.Password = _selectedEndpoint.LoginPass;
                }

                try
                {
                    // TRY TO CONNECT
                    wmiConnectionScope.Connect();
                }
                catch (Exception ex)
                {
                    ThreadSafeInvoke(() =>
                    {
                        lbl_WMI_Exception.Text = ex.Message.Split('(')[0].TrimStart().TrimEnd();
                    });
                }

                if (wmiConnectionScope.IsConnected)
                {
                    // CONNECTED - GET COMPUTER INFORMATION
                    GetComputerInformationByWMI();
                }
                else
                {
                    // NOT CONNECTED - SHOW ERROR MESSAGE
                    ThreadSafeInvoke(() =>
                    {
                        lbl_WMI_Exception.Visible = true;
                        btn_WMIInfo_Refresh.Enabled = true;
                        pb_WMIInfoProgress.Visible = false;
                    });
                }
            });
        }

        public void btn_Ports_Refresh_Click(object sender, EventArgs e)
        {
            btn_Ports_Refresh.Enabled = false;
            pb_PortsProgress.Visible = true;
            lv_Ports.Items.Clear();

            BW_PortCheck.RunWorkerAsync();
        }

        public void BW_PortCheck_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            string ipAddress = string.Empty;

            ThreadSafeInvoke(() =>
            {
                ipAddress = cb_IPAddress.GetItemText(cb_IPAddress.SelectedItem);
            });

            _ = Parallel.ForEach(portsList, portItem =>
            {
                bool portOpen = true;

                // CHECK PORT
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    socket.SendTimeout = 5000;
                    socket.ReceiveTimeout = 5000;

                    try
                    {
                        socket.Connect(ipAddress, portItem.Key);
                    }
                    catch (SocketException ex)
                    {
                        if (ex.SocketErrorCode == SocketError.ConnectionRefused ||
                            ex.SocketErrorCode == SocketError.TimedOut)
                        {
                            portOpen = false;
                        }
                    }
                }

                ThreadSafeInvoke(() =>
                {
                    lv_Ports.BeginUpdate();

                    _ = lv_Ports.Items.Add(PortListViewItem(
                        portOpen,
                        portItem.Key.ToString(),
                        portItem.Value));

                    lv_Ports.EndUpdate();
                });
            });
        }

        public ListViewItem PortListViewItem(
                                             bool portOpened,
                                             string portNumber,
                                             string portDescription)
        {
            ListViewItem portItem = new ListViewItem();

            if (portOpened)
            {
                portItem.ImageIndex = 10;
                portItem.Text = "OPEN";
            }
            else
            {
                portItem.ImageIndex = 9;
                portItem.Text = "CLOSED";
            }

            _ = portItem.SubItems.Add(portNumber);
            _ = portItem.SubItems.Add(portDescription);

            return portItem;
        }

        public void BW_PortCheck_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            btn_Ports_Refresh.Enabled = true;
            pb_PortsProgress.Visible = false;
        }

        public void GetWhoIsInfo()
        {
            NewBackgroundThread(() =>
            {
                string registrableDomainName = new Uri(_selectedEndpoint.ResponseAddress).Host;
                string whoISserver_ServerAddress = string.Empty;
                string whoISserver_RAWresponse = string.Empty;

                try
                {
                    // TRY TO GET TLD REGISTRABLE DOMAIN NAME
                    DomainParser domainParser = new DomainParser(new WebTldRuleProvider());
                    registrableDomainName = domainParser.Parse(registrableDomainName).RegistrableDomain;

                    // FIND WHOIS SERVER
                    whoISserver_ServerAddress = DomainToWhoisServerList.Default.FindWhoisServer(registrableDomainName);

                    if (!string.IsNullOrEmpty(whoISserver_ServerAddress))
                    {
                        // GET RESPONSE FROM WHOIS SERVER
                        whoISserver_RAWresponse = WhoisClient.RawQuery(
                                                                registrableDomainName,
                                                                whoISserver_ServerAddress,
                                                                43,
                                                                Encoding.UTF8)
                                                                    .TrimStart()
                                                                    .TrimEnd();

                        if (!string.IsNullOrEmpty(whoISserver_RAWresponse))
                        {
                            ThreadSafeInvoke(() =>
                            {
                                tb_WhoIs_RegistrableDomain.Text = registrableDomainName;
                                tb_WhoIs_Server.Text = whoISserver_ServerAddress;

                                rtb_WhoIsInfo.Text = whoISserver_RAWresponse;
                                rtb_WhoIsInfo.Visible = true;
                                pb_WhoIsProgress.Visible = false;

                                tabControl.TabPages.Add(tabPage_WhoIs);
                            });
                        }
                    }
                }
                catch
                {
                }
            });
        }

        public void GetPageCategoryListFromHTMLMeta()
        {
            List<Property> keywordTagList = _selectedEndpoint.HTMLMetaInfo.PropertyItem.Where(metaInfo => metaInfo.ItemName.ToLower() == "keywords").ToList();

            foreach (Property keywordTag in keywordTagList)
            {
                foreach (string keywordItem in keywordTag.ItemValue.Split(new char[] { ',', ';' }))
                {
                    string keywordValue = keywordItem.TrimStart().TrimEnd();

                    if (!string.IsNullOrEmpty(keywordValue) &&
                        !lv_CategoryList.Items.ContainsKey(keywordValue))
                    {
                        ListViewItem categoryItem = new ListViewItem
                        {
                            Text = keywordValue.First().ToString().ToUpper() + keywordValue.Substring(1).ToLower(),
                            ToolTipText = "Doubleclick category item to browse search",
                            ImageIndex = 27
                        };

                        _ = lv_CategoryList.Items.Add(categoryItem);
                    }
                }
            }

            if (lv_CategoryList.Items.Count > 0)
            {
                tabControl.TabPages.Add(tabPage_PageCategory);
            }
        }

        public void GetMACVendor(string macAddress)
        {
            pb_MACVendorProgress.Visible = true;

            string macLookupAPI = "http://macvendors.co/api/";
            string vendorAutoCompleteAPI = "https://autocomplete.clearbit.com/v1/companies/suggest?query=";

            string macAddressVendor = status_NotAvailable;

            NewBackgroundThread(() =>
            {
                if (macAddress != status_NotAvailable)
                {
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(macLookupAPI + macAddress);
                    httpWebRequest.Method = WebRequestMethods.Http.Get;
                    httpWebRequest.UserAgent = http_UserAgent;
                    httpWebRequest.Timeout = 5000;
                    httpWebRequest.ReadWriteTimeout = 5000;

                    HttpWebResponse httpWebResponse = null;

                    try
                    {
                        httpWebResponse = GetHTTPWebResponse(httpWebRequest, 5);

                        if (httpWebResponse != null &&
                            httpWebResponse.StatusCode == HttpStatusCode.OK)
                        {
                            using (Stream stream = httpWebResponse.GetResponseStream())
                            {
                                // GET RAW RESPONSE
                                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                                string macAddressVendorRAWResponse = reader.ReadToEnd();

                                // DESERIALIZE JSON
                                MACVendorsAPIResponse macAddressVendorResponse = JsonConvert.DeserializeObject<MACVendorsAPIResponse>(macAddressVendorRAWResponse);

                                // GET VALUES
                                if (!string.IsNullOrEmpty(macAddressVendorResponse.result.company))
                                {
                                    macAddressVendor = macAddressVendorResponse.result.company;

                                    if (!string.IsNullOrEmpty(macAddressVendorResponse.result.address))
                                    {
                                        // REGEX TO REPLACE MULTIPLE SPACES WITH SINGLE SPACE
                                        RegexOptions options = RegexOptions.None;
                                        Regex regex = new Regex("[ ]{2,}", options);
                                        macAddressVendor += " (" + regex.Replace(macAddressVendorResponse.result.address, " ") + ")";
                                    }

                                    // GET VENDOR WEBSITE
                                    httpWebRequest = (HttpWebRequest)WebRequest.Create(vendorAutoCompleteAPI + macAddressVendorResponse.result.company.Split(' ')[0]);
                                    httpWebRequest.Method = WebRequestMethods.Http.Get;
                                    httpWebRequest.UserAgent = http_UserAgent;
                                    httpWebRequest.Timeout = 5000;
                                    httpWebRequest.ReadWriteTimeout = 5000;

                                    httpWebResponse = null;

                                    try
                                    {
                                        httpWebResponse = GetHTTPWebResponse(httpWebRequest, 5);

                                        if (httpWebResponse != null &&
                                            httpWebResponse.StatusCode == HttpStatusCode.OK)
                                        {
                                            using (Stream stream2 = httpWebResponse.GetResponseStream())
                                            {
                                                // GET RAW RESPONSE
                                                StreamReader reader2 = new StreamReader(stream2, Encoding.UTF8);
                                                string vendorAutoCompleteAPIRAWResponse = reader2.ReadToEnd();

                                                // DESERIALIZE JSON
                                                ClearBitAutocompleteAPIResult[] clearbitAutoCompleteAPIResponse = JsonConvert.DeserializeObject<ClearBitAutocompleteAPIResult[]>(vendorAutoCompleteAPIRAWResponse);

                                                foreach (ClearBitAutocompleteAPIResult result in clearbitAutoCompleteAPIResponse)
                                                {
                                                    // GET VENDOR IMAGE
                                                    if (!string.IsNullOrEmpty(result.logo))
                                                    {
                                                        Bitmap vendorImage = GetImageFromURL(result.logo);

                                                        if (vendorImage != null)
                                                        {
                                                            ThreadSafeInvoke(() =>
                                                            {
                                                                pb_Vendor.Image = ResizeImage(vendorImage, 23, 23);

                                                                // CLICK HANDLER FOR VENDOR WEB PAGE
                                                                if (!string.IsNullOrEmpty(result.domain))
                                                                {
                                                                    macVendorWebPage = "http://" + result.domain;
                                                                    pb_Vendor.Cursor = Cursors.Hand;
                                                                    macVendorIconTooltip
                                                                        .SetToolTip(
                                                                            pb_Vendor,
                                                                                "Click to open \"" +
                                                                                macAddressVendorResponse.result.company +
                                                                                "\" web page (" +
                                                                                result.domain +
                                                                                ")");
                                                                }
                                                            });

                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    catch
                                    {
                                    }
                                    finally
                                    {
                                        if (httpWebResponse != null)
                                        {
                                            httpWebResponse.Close();

                                            GC.Collect();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                    finally
                    {
                        if (httpWebResponse != null)
                        {
                            httpWebResponse.Close();

                            GC.Collect();
                        }
                    }
                }

                ThreadSafeInvoke(() =>
                {
                    tb_MACVendor.Text = macAddressVendor;
                    pb_MACVendorProgress.Visible = false;
                });
            });
        }

        public void GetIPGeoInfo(string ipAddress)
        {
            NewBackgroundThread(() =>
            {
                ThreadSafeInvoke(() =>
                {
                    bool showTab = false;

                    if (ipAddress != status_NotAvailable)
                    {
                        try
                        {
                            string info = new WebClient().DownloadString("http://ip-api.com/json/" + ipAddress);
                            IP_API_JSON_Response ipInfo = JsonConvert.DeserializeObject<IP_API_JSON_Response>(info);

                            if (ipInfo.Service_Status == "success")
                            {
                                Bitmap mapTile = GetImageFromGoogleMapsAPI(ipInfo.Geo_Lat, ipInfo.Geo_Lon);

                                if (mapTile != null)
                                {
                                    // MAP TILE
                                    pb_GeoLocation_Map.Image = mapTile;

                                    // GET COUNTRY FLAG
                                    Bitmap countryFlag = GetCountryFlagByCode(ipInfo.Country_Code);

                                    if (countryFlag != null)
                                    {
                                        pb_GeoLocation_CountryFlag.Image = countryFlag;
                                        pb_GeoLocation_CountryFlag.Visible = true;
                                    }
                                    else
                                    {
                                        pb_GeoLocation_CountryFlag.Visible = false;
                                    }

                                    tb_GeoLocation_Latitude.Text = ipInfo.Geo_Lat;
                                    tb_GeoLocation_Longitude.Text = ipInfo.Geo_Lon;
                                    tb_GeoLocation_ISP.Text = NotAvailable_IfNullorEmpty(ipInfo.ISP);
                                    tb_GeoLocation_AS.Text = NotAvailable_IfNullorEmpty(ipInfo.ISP_AS.Replace(ipInfo.ISP, string.Empty).TrimEnd());
                                    tb_GeoLocation_RegionName.Text = NotAvailable_IfNullorEmpty(ipInfo.Region_Name);
                                    tb_GeoLocation_TimeZone.Text = NotAvailable_IfNullorEmpty(ipInfo.TimeZone);
                                    tb_GeoLocation_ZipCode.Text = NotAvailable_IfNullorEmpty(ipInfo.City_ZIP_Code);
                                    tb_GeoLocation_City.Text = NotAvailable_IfNullorEmpty(ipInfo.City);
                                    tb_GeoLocation_ORG.Text = NotAvailable_IfNullorEmpty(ipInfo.ISP_ORG);
                                    tb_GeoLocation_CountryName.Text = NotAvailable_IfNullorEmpty(ipInfo.Country_Name + " (" + ipInfo.Country_Code + ")");
                                    tb_GeoLocation_RegionName.Text = NotAvailable_IfNullorEmpty(ipInfo.Region_Name + " (" + ipInfo.Region_Code + ")");

                                    tb_GeoLocation_IP.Text = cb_IPAddress.GetItemText(cb_IPAddress.SelectedItem);
                                    pb_GeoLocationProgress.Visible = false;
                                    pb_GeoLocation_Map.Visible = true;

                                    showTab = true;
                                }
                            }
                        }
                        catch
                        {
                        }
                    }

                    if (showTab)
                    {
                        if (!tabControl.TabPages.Contains(tabPage_GeoLocation))
                        {
                            // ADD GEO INFO TAB PAGE [IF NOT PRESENT]
                            tabControl.TabPages.Add(tabPage_GeoLocation);
                        }
                    }
                    else if (tabControl.TabPages.Contains(tabPage_GeoLocation))
                    {
                        // REMOVE GEO INFO TAB PAGE [IF PRESENT]
                        tabControl.TabPages.Remove(tabPage_GeoLocation);
                    }
                });
            });
        }

        public Bitmap GetCountryFlagByCode(string countryCode)
        {
            Bitmap flag = null;
            HttpWebResponse httpWebResponse = null;

            try
            {
                string path = "http://www.geognos.com/api/en/countries/flag/" + countryCode + ".png";

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(path);
                httpWebRequest.Method = WebRequestMethods.Http.Get;
                httpWebRequest.UserAgent = http_UserAgent;
                httpWebRequest.Timeout = 5000;
                httpWebRequest.ReadWriteTimeout = 5000;

                httpWebResponse = GetHTTPWebResponse(httpWebRequest, 5);

                if (httpWebResponse != null &&
                    httpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream stream = httpWebResponse.GetResponseStream())
                    {
                        flag = new Bitmap(stream);
                    }
                }
            }
            catch
            {
            }
            finally
            {
                if (httpWebResponse != null)
                {
                    httpWebResponse.Close();

                    GC.Collect();
                }
            }

            return flag;
        }

        public Bitmap GetImageFromGoogleMapsAPI(string latitude, string longitude)
        {
            Bitmap mapTile = null;
            HttpWebResponse httpWebResponse = null;

            try
            {
                string latlng = latitude + "," + longitude;
                string path = "http://maps.googleapis.com/maps/api/staticmap?center=" +
                latlng +
                "&zoom=" +
                googleMapsZoomFactor +
                "&size=400x400" +
                "&maptype=hybrid" +
                "&markers=color:green%7Clabel:A%7C" +
                latlng +
                "&key=" + apiKey_GoogleMaps;

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(path);
                httpWebRequest.Method = WebRequestMethods.Http.Get;
                httpWebRequest.UserAgent = http_UserAgent;
                httpWebRequest.Timeout = 5000;
                httpWebRequest.ReadWriteTimeout = 5000;

                httpWebResponse = GetHTTPWebResponse(httpWebRequest, 5);

                if (httpWebResponse != null &&
                    httpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream stream = httpWebResponse.GetResponseStream())
                    {
                        mapTile = new Bitmap(stream);
                    }
                }
            }
            catch
            {
            }
            finally
            {
                if (httpWebResponse != null)
                {
                    httpWebResponse.Close();

                    GC.Collect();
                }
            }

            return mapTile;
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
                Application.DoEvents();

                _ = Invoke(action);
            }
            catch
            {
            }
        }

        public void pb_GeoLocation_Map_Click(object sender, EventArgs e)
        {
            try
            {
                StartBackgroundProcess(
                    "https://www.google.com/maps/@?api=1&map_action=map&center=" +
                    tb_GeoLocation_Latitude.Text +
                    "," +
                    tb_GeoLocation_Longitude.Text +
                    "&zoom=" +
                    googleMapsZoomFactor +
                    "&basemap=satellite",
                    null,
                    null,
                    null);
            }
            catch (Exception exception)
            {
                ExceptionNotifier(this, exception, string.Empty, true);
            }
        }

        public void cb_IPAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            // DNS NAME
            tb_DNSName.Text = _selectedEndpoint.DNSName.Length == _selectedEndpoint.IPAddress.Length
                ? _selectedEndpoint.DNSName[cb_IPAddress.SelectedIndex]
                : string.Join(", ", _selectedEndpoint.DNSName);

            // MAC ADDRESS
            pb_Vendor.Image = Properties.Resources.vendor;
            pb_Vendor.Cursor = Cursors.Default;
            macVendorIconTooltip.SetToolTip(pb_Vendor, string.Empty);
            macVendorWebPage = string.Empty;

            if (_selectedEndpoint.MACAddress.Length > cb_IPAddress.SelectedIndex)
            {
                tb_MACAddress.Text = _selectedEndpoint.MACAddress[cb_IPAddress.SelectedIndex];

                // GET MAC VENDOR
                GetMACVendor(_selectedEndpoint.MACAddress[cb_IPAddress.SelectedIndex]);
            }
            else
            {
                tb_MACAddress.Text = status_NotAvailable;
                tb_MACVendor.Text = status_NotAvailable;
            }

            // GET IP ADDRESS GEO INFO
            GetIPGeoInfo(_selectedEndpoint.IPAddress[cb_IPAddress.SelectedIndex]);

            // IP ADDRESS PRESENT
            if (cb_IPAddress.GetItemText(cb_IPAddress.SelectedItem) != status_NotAvailable)
            {
                // PORT STATUS
                if (btn_Ports_Refresh.Enabled)
                {
                    if (!tabControl.TabPages.Contains(tabPage_Ports))
                    {
                        tabControl.TabPages.Add(tabPage_Ports);
                    }

                    btn_Ports_Refresh_Click(this, null);
                }

                // TRACE ROUTE
                if (btn_TraceRoute_Refresh.Enabled)
                {
                    if (!tabControl.TabPages.Contains(tabPage_TraceRoute))
                    {
                        tabControl.TabPages.Add(tabPage_TraceRoute);
                    }

                    btn_TraceRoute_Refresh_Click(this, null);
                }

                // WMI INFO
                if (btn_WMIInfo_Refresh.Enabled)
                {
                    if (!tabControl.TabPages.Contains(tabPage_WMI_ComputerInfo))
                    {
                        tabControl.TabPages.Add(tabPage_WMI_ComputerInfo);
                    }

                    btn_WMIInfo_Refresh_Click(this, null);
                }
            }

            // VIRUS TOTAL
            if (string.IsNullOrEmpty(apiKey_VirusTotal) &&
                tabControl.Controls.Contains(tabPage_VirusTotal))
            {
                btn_VirusTotal_Refresh_Click(this, null);
            }
        }

        public Bitmap GetImageFromURL(string imageURL)
        {
            Bitmap image = null;
            HttpWebResponse httpWebResponse = null;

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(imageURL);
                httpWebRequest.Method = WebRequestMethods.Http.Get;
                httpWebRequest.UserAgent = http_UserAgent;
                httpWebRequest.Timeout = 5000;
                httpWebRequest.ReadWriteTimeout = 5000;

                httpWebResponse = GetHTTPWebResponse(httpWebRequest, 5);

                if (httpWebResponse != null &&
                    httpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream stream = httpWebResponse.GetResponseStream())
                    {
                        image = new Bitmap(stream);
                    }
                }
            }
            catch
            {
            }
            finally
            {
                if (httpWebResponse != null)
                {
                    httpWebResponse.Close();

                    GC.Collect();
                }
            }

            return image;
        }

        private void pb_Vendor_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(macVendorWebPage))
            {
                try
                {
                    StartBackgroundProcess(
                                                 macVendorWebPage,
                                                 null,
                                                 null,
                                                 null);
                }
                catch (Exception exception)
                {
                    ExceptionNotifier(this, exception, string.Empty, true);
                }
            }
        }

        public void btn_VirusTotal_Refresh_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(apiKey_VirusTotal))
            {
                // ASK FOR API KEY
                DialogResult questionDialogResult = MessageBox.Show("API key required for VirusTotal scan option is not set. Do you want to set your own API key in application config file ?", "VirusTotal API Key", MessageBoxButtons.YesNo);
                if (questionDialogResult == DialogResult.Yes)
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
                else
                {
                    tabControl.TabPages.Remove(tabPage_VirusTotal);
                }
            }
            else
            {
                // SET CONTROLS / RUN SCAN
                lv_VirusTotal.Visible = false;

                lbl_VirusTotal_Permalink.Visible = false;
                lbl_VirusTotal_ScanDateTime.Visible = false;

                tb_VirusTotal_Permalink.Visible = false;
                tb_VirusTotal_ScanDateTime.Visible = false;

                lbl_VirusTotal_Status.Visible = true;
                lbl_VirusTotal_Status.Text = "Website scan in progress ...";

                btn_VirusTotal_Refresh.Enabled = false;
                pb_VirusTotalRefresh.Visible = true;
                pb_VirusTotal_Status.Visible = false;
                lv_VirusTotal.Items.Clear();

                GetVirusTotalScanReport(tb_ResponseURL.Text, 24, 0);
            }
        }

        public void GetVirusTotalScanReport(string urlToScan, int maxRetryCount, int retry)
        {
            virusTotal_ScanResult = null;

            NewBackgroundThread(() =>
            {
                try
                {
                    // REQUEST SCAN REPORT
                    VirusTotal virusTotal = new VirusTotal(apiKey_VirusTotal)
                    {
                        UseTLS = true,
                        UserAgent = http_UserAgent
                    };
                    Task<UrlScanResult> virusTotal_ScanResultTask = virusTotal.ScanUrlAsync(urlToScan);
                    virusTotal_ScanResult = virusTotal_ScanResultTask.Result;

                    if (virusTotal_ScanResult.ResponseCode == VirusTotalNET.ResponseCodes.UrlScanResponseCode.Queued)
                    {
                        ThreadSafeInvoke(() =>
                        {
                            lbl_VirusTotal_Status.ForeColor = Color.DarkGreen;
                            lbl_VirusTotal_Status.Text = virusTotal_ScanResultTask.Result.VerboseMsg;
                        });
                    }
                    else
                    {
                        throw new VirusTotalNET.Exceptions.InvalidResourceException(virusTotal_ScanResult.VerboseMsg);
                    }
                }
                catch (Exception vtException)
                {
                    if (!vtException.GetType().IsAssignableFrom(typeof(VirusTotalNET.Exceptions.InvalidResourceException)) && retry <= maxRetryCount)
                    {
                        ThreadSafeInvoke(() =>
                        {
                            lbl_VirusTotal_Status.ForeColor = Color.MediumVioletRed;

                            lbl_VirusTotal_Status.Text = vtException.InnerException != null
                                ? vtException.InnerException.Message ?? vtException.InnerException.ToString()
                                : vtException.ToString();

                            if (retry > 0)
                            {
                                lbl_VirusTotal_Status.Text += " [Retry " + retry + "]";
                            }
                        });

                        retry++;

                        Thread.Sleep(5000);
                        GetVirusTotalScanReport(urlToScan, maxRetryCount, retry);
                    }
                    else
                    {
                        ThreadSafeInvoke(() =>
                        {
                            tabControl.TabPages.Remove(tabPage_VirusTotal);
                        });
                    }
                }
            });
        }

        public void tb_VirusTotal_Permalink_MouseClick(object sender, MouseEventArgs e)
        {
            BrowseEndpoint(
                tb_VirusTotal_Permalink.Text,
                null,
                null,
                null);
        }

        public void TIMER_VirusTotalResult_Tick(object sender, EventArgs e)
        {
            if (virusTotal_ScanResult != null &&
                !string.IsNullOrEmpty(virusTotal_ScanResult.Url) &&
                !BW_VirusTotal_Report.IsBusy)
            {
                BW_VirusTotal_Report.RunWorkerAsync();
            }
        }

        private void BW_VirusTotal_Report_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                // REQUEST SCAN RESULT
                VirusTotal virusTotal = new VirusTotal(apiKey_VirusTotal)
                {
                    UseTLS = true,
                    UserAgent = http_UserAgent
                };
                Task<UrlReport> virusTotalReportTask = virusTotal.GetUrlReportAsync(virusTotal_ScanResult.Url);
                UrlReport virusTotalReport = virusTotalReportTask.Result;
                virusTotalReportTask.Dispose();
                GC.Collect();

                if (virusTotalReport.ResponseCode == VirusTotalNET.ResponseCodes.UrlReportResponseCode.Present)
                {
                    virusTotal_ScanResult = null;

                    ThreadSafeInvoke(() =>
                    {
                        if (virusTotalReport.Scans.Count > 0)
                        {
                            if (virusTotalReport.Positives == 0)
                            {
                                // CLEAN
                                pb_VirusTotal_Status.Image = Properties.Resources.virusClean;
                            }
                            else
                            {
                                // INFECTED
                                pb_VirusTotal_Status.Image = Properties.Resources.virusIcon;

                            }

                            // SET CONTROLS
                            pb_VirusTotal_Status.Visible = true;
                            lv_VirusTotal.Visible = true;
                            btn_VirusTotal_Refresh.Enabled = true;
                            lbl_VirusTotal_Status.Visible = false;
                            pb_VirusTotalRefresh.Visible = false;
                            lbl_VirusTotal_Permalink.Visible = true;
                            tb_VirusTotal_Permalink.Visible = true;
                            lbl_VirusTotal_ScanDateTime.Visible = true;
                            tb_VirusTotal_ScanDateTime.Visible = true;
                            tb_VirusTotal_Permalink.Text = virusTotalReport.Permalink;
                            tb_VirusTotal_ScanDateTime.Text = virusTotalReport.ScanDate.ToLocalTime().ToString("dd.MM.yyyy HH:mm");

                            // LIST RESULTS
                            foreach (string scanEngine in virusTotalReport.Scans.Keys)
                            {
                                ListViewItem scanResultItem = new ListViewItem();
                                UrlScanEngine urlScan = virusTotalReport.Scans[scanEngine];
                                scanResultItem.Text = scanEngine;
                                _ = scanResultItem.SubItems.Add(urlScan.Result);

                                scanResultItem.ImageIndex = urlScan.Result == "clean site" ? 10 : urlScan.Result == "unrated site" ? 14 : 13;

                                _ = lv_VirusTotal.Items.Add(scanResultItem);
                            }
                        }
                    });
                }
                else
                {
                    ThreadSafeInvoke(() =>
                    {
                        lbl_VirusTotal_Status.Text = "Waiting for Scan Report ...";
                    });
                }
            }
            catch
            {
            }
        }

        public void lv_PageLinks_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lv_PageLinks.SelectedItems.Count == 1)
            {
                string address = lv_PageLinks.SelectedItems[0].SubItems[2].Text;

                try
                {
                    StartBackgroundProcess(
                                                 address,
                                                 null,
                                                 null,
                                                 null);
                }
                catch (Exception exception)
                {
                    ExceptionNotifier(this, exception, string.Empty, true);
                }
            }
        }

        public void btn_PageLinks_Validate_Click(object sender, EventArgs e)
        {
            ValidatePageLinks();
        }

        public void ValidatePageLinks()
        {
            // SET CONTROLS
            btn_PageLinks_Validate.Enabled = false;
            pb_PageLinks_ValidatingProgress.Visible = true;
            pb_PageLinks_CommonLinksStatus.Visible = false;

            NewBackgroundThread(() =>
            {
                // CHEKED LINKS LIST [ITEM1: STATUS, ITEM2: LINK TYPE, ITEM3: LINK ADDRESS
                List<Tuple<string, string, string>> checkedLinksList = new List<Tuple<string, string, string>>();

                foreach (Property pageLink in _selectedEndpoint.HTMLPageLinks.PropertyItem)
                {
                    // ADD PAGE LINK ITEMS
                    if (pageLink.ItemValue.TrimEnd('/') != tb_ResponseURL.Text.TrimEnd('/'))
                    {
                        string linkStatus = "INVALID";

                        HttpWebResponse httpWebResponse = null;

                        try
                        {
                            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(pageLink.ItemValue);
                            httpWebRequest.Method = WebRequestMethods.Http.Get;
                            httpWebRequest.UserAgent = http_UserAgent;
                            httpWebRequest.Timeout = 10000;
                            httpWebRequest.ReadWriteTimeout = 10000;
                            httpWebRequest.AllowAutoRedirect = true;
                            httpWebRequest.ProtocolVersion = HttpVersion.Version11;

                            httpWebResponse = GetHTTPWebResponse(httpWebRequest, 5);

                            linkStatus = "VALID";
                        }
                        catch (WebException webEX)
                        {
                            httpWebResponse = webEX.Response as HttpWebResponse;

                            if (httpWebResponse != null &&
                                httpWebResponse.StatusCode != HttpStatusCode.NotFound &&
                                httpWebResponse.StatusCode != HttpStatusCode.BadGateway &&
                                httpWebResponse.StatusCode != HttpStatusCode.GatewayTimeout &&
                                httpWebResponse.StatusCode != HttpStatusCode.InternalServerError &&
                                httpWebResponse.StatusCode != HttpStatusCode.NotImplemented &&
                                httpWebResponse.StatusCode != HttpStatusCode.RequestTimeout &&
                                httpWebResponse.StatusCode != HttpStatusCode.Conflict &&
                                httpWebResponse.StatusCode != HttpStatusCode.Gone &&
                                httpWebResponse.StatusCode != HttpStatusCode.ServiceUnavailable)
                            {
                                linkStatus = "VALID";
                            }
                        }
                        catch
                        {
                        }
                        finally
                        {
                            if (httpWebResponse != null)
                            {
                                httpWebResponse.Close();

                                GC.Collect();
                            }
                        }

                        checkedLinksList.Add(new Tuple<string, string, string>(linkStatus, pageLink.ItemName, pageLink.ItemValue));
                    }

                    Application.DoEvents();
                }

                ThreadSafeInvoke(() =>
                {
                    // UPDATE LIST
                    lv_PageLinks.BeginUpdate();
                    lv_PageLinks.Items.Clear();

                    foreach (Tuple<string, string, string> checkedLinkStatus in checkedLinksList)
                    {
                        ListViewItem linkItem = new ListViewItem
                        {
                            Text = checkedLinkStatus.Item1
                        };

                        if (linkItem.Text == "VALID")
                        {
                            linkItem.ImageIndex = 10;
                        }
                        else if (linkItem.Text == "INVALID")
                        {
                            linkItem.ImageIndex = 9;
                        }

                        _ = linkItem.SubItems.Add(checkedLinkStatus.Item2);
                        _ = linkItem.SubItems.Add(checkedLinkStatus.Item3);

                        _ = lv_PageLinks.Items.Add(linkItem);
                    }

                    lv_PageLinks.EndUpdate();

                    // SET CONTROLS
                    pb_PageLinks_ValidatingProgress.Visible = false;
                    btn_PageLinks_Validate.Enabled = true;

                    // COMMON LINKS STATUS
                    if (lv_PageLinks.Items.Count > 0)
                    {
                        bool isAnyInvalid = false;

                        foreach (ListViewItem linkItem in lv_PageLinks.Items)
                        {
                            if (linkItem.Text == "INVALID")
                            {
                                isAnyInvalid = true;
                            }
                        }

                        pb_PageLinks_CommonLinksStatus.Image = isAnyInvalid ? Properties.Resources.linkStatus_Invalid : (Image)Properties.Resources.linkStatus_Valid;

                        pb_PageLinks_CommonLinksStatus.Visible = true;
                    }
                });
            });
        }

        public void lv_CategoryList_ItemActivate(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection selectedItems = ((ListView)sender).SelectedItems;

            foreach (ListViewItem selectedItem in selectedItems)
            {
                BrowseEndpoint(
                    "https://www.google.com/search?source=hp&q=" +
                    selectedItem.Text + "&oq=" +
                    selectedItem.Text,
                    null,
                    null,
                    null);
            }
        }

        public void pb_ShowPassword_MouseDown(object sender, MouseEventArgs e)
        {
            TextBox_SetPasswordVisibilty(
                tb_UserPassword,
                true);
        }

        public void pb_ShowPassword_MouseUp(object sender, MouseEventArgs e)
        {
            TextBox_SetPasswordVisibilty(
                tb_UserPassword,
                false);
        }

        public void tb_UserPassword_TextChanged(object sender, EventArgs e)
        {
            TextBox_SetPasswordVisibilty(
                tb_UserPassword,
                tb_UserPassword.Text == status_NotAvailable);

            pb_ShowPassword.Visible =
                !(tb_UserPassword.Text == status_NotAvailable);
        }

        public void pb_SSH_MouseClick(object sender, MouseEventArgs e)
        {
            ConnectEndpoint_Putty(
                new Uri(_selectedEndpoint.ResponseAddress).Host);
        }

        public void rtb_WhoIsInfo_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            BrowseEndpoint(
                    e.LinkText,
                    null,
                    null,
                    null);
        }
    }

    public class MACVendorsAPIResponse
    {
        public MACVendorsAPIResult result { get; set; }
    }

    public class MACVendorsAPIResult
    {
        public string company { get; set; }
        public string mac_prefix { get; set; }
        public string address { get; set; }
        public string start_hex { get; set; }
        public string end_hex { get; set; }
        public string country { get; set; }
        public string type { get; set; }
    }

    public class ClearBitAutocompleteAPIResult
    {
        public string name { get; set; }
        public string domain { get; set; }
        public string logo { get; set; }
    }
}
