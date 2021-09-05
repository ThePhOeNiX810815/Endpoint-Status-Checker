using DomainPublicSuffix;
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
using System.Xml;
using VirusTotalNET;
using VirusTotalNET.Objects;
using VirusTotalNET.Results;
using Whois.NET;
using Whois.NET.Internal;

namespace EndpointChecker
{
    public partial class EndpointDetailsDialog : Form
    {
        // WMI CONNECTION SCOPE
        ManagementScope wmiConnectionScope = new ManagementScope();

        // MAC VENDOR WEBPAGE URL
        string macVendorWebPage = string.Empty;

        // IP ADDRESS COMBOBOX TOOLTIP
        ToolTip ipAddress_ComboboxTooltip = new ToolTip();

        // LIVE PING ICON TOOLTIP
        ToolTip livePingIconTooltip = new ToolTip();

        // MAC VENDOR ICON TOOLTIP
        ToolTip macVendorIconTooltip = new ToolTip();

        // USER PASSWORD VISIBILITY ICON TOOLTIP
        ToolTip userPasswordVisibilityIconTooltip = new ToolTip();

        // VIRUSTOTAL PERMALINK TEXTBOX TOOLTIP
        ToolTip virusTotal_PermalinkTextboxTooltip = new ToolTip();

        // WEAK ETAG ICON TOOLTIP
        ToolTip httpInfo_WeakETagIconTooltip = new ToolTip();

        // VIRUSTOTAL RESULT
        UrlScanResult virusTotal_ScanResult = null;

        // WMI MANAGEMENT CLASSES ENUM
        enum WMIManagementClass
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
        Dictionary<int, string> portsList = new Dictionary<int, string>
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
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CheckerMainForm.UnhandledExceptionHandler);
            Application.ThreadException += new ThreadExceptionEventHandler(CheckerMainForm.ThreadExceptionHandler);

            // HIDE PROTOCOL SPECIFIC FIELDS FROM 'MAIN INFO' TAB, IF VALIDATION METHOD IS 'PING ONLY'
            if (CheckerMainForm.validationMethod == CheckerMainForm.ValidationMethod.Ping)
            {
                lbl_StatusCode.Visible = false;
                pb_StatusIcon.Visible = false;
                tb_StatusCode.Visible = false;
                lbl_StatusMessage.Visible = false;
                tb_StatusMessage.Visible = false;
            }

            // ADD RESIZED [16x16] IMAGES TO TAB CONTROL IMAGE LIST
            imageList_Tabs.Images.Add(CheckerMainForm.ResizeImage(Properties.Resources.browse_FTP, 16, 16));
            imageList_Tabs.Images.Add(CheckerMainForm.ResizeImage(Properties.Resources.browse_HTTP, 16, 16));
            imageList_Tabs.Images.Add(CheckerMainForm.ResizeImage(Properties.Resources.browse_Share, 16, 16));
            imageList_Tabs.Images.Add(CheckerMainForm.ResizeImage(Properties.Resources.pingHop, 16, 16));
            imageList_Tabs.Images.Add(CheckerMainForm.ResizeImage(Properties.Resources.requestHeaderProperty, 16, 16));
            imageList_Tabs.Images.Add(CheckerMainForm.ResizeImage(Properties.Resources.responseHeaderProperty, 16, 16));
            imageList_Tabs.Images.Add(CheckerMainForm.ResizeImage(Properties.Resources.information, 16, 16));
            imageList_Tabs.Images.Add(CheckerMainForm.ResizeImage(Properties.Resources.wmi, 16, 16));
            imageList_Tabs.Images.Add(CheckerMainForm.ResizeImage(Properties.Resources.port, 16, 16));
            imageList_Tabs.Images.Add(CheckerMainForm.ResizeImage(Properties.Resources.whoIs, 16, 16));
            imageList_Tabs.Images.Add(CheckerMainForm.ResizeImage(Properties.Resources.geoLocation, 16, 16));
            imageList_Tabs.Images.Add(CheckerMainForm.ResizeImage(Properties.Resources.ssl, 16, 16));
            imageList_Tabs.Images.Add(CheckerMainForm.ResizeImage(Properties.Resources.virusTotal, 16, 16));
            imageList_Tabs.Images.Add(CheckerMainForm.ResizeImage(Properties.Resources.category, 16, 16));
            imageList_Tabs.Images.Add(CheckerMainForm.ResizeImage(Properties.Resources.link_16x16, 16, 16));
            imageList_Tabs.Images.Add(CheckerMainForm.ResizeImage(Properties.Resources.main, 16, 16));

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
            pb_Browse_WindowsExplorer.Image = CheckerMainForm.ResizeImage(Properties.Resources.browse_Share, pb_Browse_WindowsExplorer.Width, pb_Browse_WindowsExplorer.Height);
            pb_AdminBrowse_WindowsExplorer.Image = CheckerMainForm.ResizeImage(Properties.Resources.browse_Admin_Share, pb_AdminBrowse_WindowsExplorer.Width, pb_AdminBrowse_WindowsExplorer.Height);
            pb_FTP.Image = CheckerMainForm.ResizeImage(Properties.Resources.browse_FTP, pb_FTP.Width, pb_FTP.Height);
            pb_HTTP.Image = CheckerMainForm.ResizeImage(Properties.Resources.browse_HTTP, pb_HTTP.Width, pb_HTTP.Height);
            pb_RDP.Image = CheckerMainForm.ResizeImage(Properties.Resources.connect_RDP, pb_RDP.Width, pb_RDP.Height);
            pb_VNC.Image = CheckerMainForm.ResizeImage(Properties.Resources.connect_VNC, pb_VNC.Width, pb_VNC.Height);

            _selectedEndpoint = selectedEndpoint;
            _selectedEndpointIcon = selectedEndpointImage;
            _pingTimeout = pingTimeout;

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

            TIMER_LoadProperties.Start();
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
                CheckerMainForm.validationMethod != CheckerMainForm.ValidationMethod.Ping)
            {
                // ADD FTP TAB PAGE
                if (_selectedEndpoint.FTPBannerMessage != CheckerMainForm.status_NotAvailable ||
                    _selectedEndpoint.FTPExitMessage != CheckerMainForm.status_NotAvailable ||
                    _selectedEndpoint.FTPStatusDescription != CheckerMainForm.status_NotAvailable ||
                    _selectedEndpoint.FTPWelcomeMessage != CheckerMainForm.status_NotAvailable)
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
                if (CheckerMainForm.validationMethod != CheckerMainForm.ValidationMethod.Ping)
                {
                    // HTTP REQUEST HEADERS
                    if (_selectedEndpoint.HTTPRequestHeaders.PropertyItem.Count > 0)
                    {
                        foreach (Property header in _selectedEndpoint.HTTPRequestHeaders.PropertyItem)
                        {
                            ListViewItem headerItem = new ListViewItem(header.ItemName, 4);
                            headerItem.SubItems.Add(header.ItemValue);
                            lv_HTTP_RequestHeaders.Items.Add(headerItem);
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
                            headerItem.SubItems.Add(header.ItemValue);
                            lv_HTTP_ResponseHeaders.Items.Add(headerItem);
                        }

                        // AUTO SIZE 'KEY' COLUMN BY LONGEST ITEM
                        ch_HTTPresponse_Headers_Key.Width = -1;

                        // ADD HTTP RESPONSE HEADERS TAB PAGE
                        tabControl.TabPages.Add(tabPage_HTTPResponseHeaders);
                    }

                    tb_PageInfo_HTMLTitle.Text = _selectedEndpoint.HTMLTitle;
                    tb_PageInfo_HTMLDescription.Text = _selectedEndpoint.HTMLDescription;
                    tb_PageInfo_Author.Text = _selectedEndpoint.HTMLAuthor;
                    tb_PageInfo_HTMLEncoding.Text = CheckerMainForm.GetEncodingName(_selectedEndpoint.HTMLencoding);
                    tb_PageInfo_ContentLanguage.Text = _selectedEndpoint.HTMLContentLanguage;

                    // SET THEME COLOR CONTROLS
                    if (_selectedEndpoint.HTMLThemeColor != Color.Empty)
                    {
                        tb_PageInfo_ThemeColor.Text = CheckerMainForm.GetKnownColorNameString(_selectedEndpoint.HTMLThemeColor);
                        pb_PageInfo_ThemeColor.BackColor = _selectedEndpoint.HTMLThemeColor;
                        pb_PageInfo_ThemeColor.Visible = true;
                    }
                    else
                    {
                        tb_PageInfo_ThemeColor.Text = CheckerMainForm.status_NotAvailable;
                    }

                    // HTML INFO
                    if (tb_PageInfo_HTMLTitle.Text != CheckerMainForm.status_NotAvailable ||
                        tb_PageInfo_Author.Text != CheckerMainForm.status_NotAvailable ||
                        tb_PageInfo_HTMLDescription.Text != CheckerMainForm.status_NotAvailable ||
                        tb_PageInfo_ContentLanguage.Text != CheckerMainForm.status_NotAvailable ||
                        tb_PageInfo_ThemeColor.Text != CheckerMainForm.status_NotAvailable ||
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

                            metaItem.SubItems.Add(meta.ItemValue);
                            lv_HTML_MetaInfo.Items.Add(metaItem);
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
                    tb_HTTPInfo_Encoding.Text = CheckerMainForm.GetEncodingName(_selectedEndpoint.HTTPencoding);
                    tb_HTTPInfo_ContentLenght.Text = _selectedEndpoint.HTTPcontentLenght;
                    tb_HTTPInfo_Expires.Text = _selectedEndpoint.HTTPexpires;
                    tb_HTTPInfo_ETag.Text = _selectedEndpoint.HTTPetag.TrimStart('W').TrimStart('/').TrimStart('"').TrimEnd('"');
                    pb_HTTPInfo_WeakETag.Visible = _selectedEndpoint.HTTPetag.ToLower().StartsWith("w/");

                    if (tb_HTTPInfo_ServerName.Text != CheckerMainForm.status_NotAvailable ||
                        tb_HTTPInfo_AutoRedirects.Text != CheckerMainForm.status_NotAvailable ||
                        tb_HTTPInfo_ContentType.Text != CheckerMainForm.status_NotAvailable ||
                        tb_HTTPInfo_Encoding.Text != CheckerMainForm.status_NotAvailable ||
                        tb_HTTPInfo_ContentLenght.Text != CheckerMainForm.status_NotAvailable ||
                        tb_HTTPInfo_Expires.Text != CheckerMainForm.status_NotAvailable ||
                        tb_HTTPInfo_ETag.Text != CheckerMainForm.status_NotAvailable)
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
                            sslItem.SubItems.Add(sslProperty.ItemValue);
                            lv_SSLCertificate.Items.Add(sslItem);
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
                            pageLinkItem.SubItems.Add(pageLink.ItemName);
                            pageLinkItem.SubItems.Add(pageLink.ItemValue);
                            lv_PageLinks.Items.Add(pageLinkItem);
                        }

                        // ADD HTML PAGE LINKS PAGE
                        tabControl.TabPages.Add(tabPage_PageLinks);

                        // SET PAGE LINKS COUNT MESSAGE
                        lbl_LinksCountMessage.Text = lv_PageLinks.Items.Count + " links has been found within this webpage. Doubleclick any to open in web browser.";
                    }
                }

                // VIRUSTOTAL SCAN
                tabControl.TabPages.Add(tabPage_VirusTotal);
                btn_VirusTotal_Refresh_Click(this, null);
            }

            // SET TABS [COMMON]
            // NETWORK SHARES
            if (!_selectedEndpoint.NetworkShare[0].Contains(CheckerMainForm.status_NotAvailable))
            {
                foreach (string netShare in _selectedEndpoint.NetworkShare)
                {
                    string[] valueStringArray = netShare.Split(new Char[]
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

                    netShareItem.SubItems.Add(valueStringArray[1].TrimStart().TrimEnd());

                    if (valueStringArray.Length > 2)
                    {
                        netShareItem.SubItems.Add(valueStringArray[2].Replace(")", string.Empty).TrimStart().TrimEnd());
                    }

                    netShareItem.ToolTipText = "Mouse doubleclick to browse shared resource";

                    lv_NetworkShares.Items.Add(netShareItem);
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

            NewBackgroundThread((Action)(() =>
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(websiteURL + "/favicon.ico");
                httpWebRequest.Method = WebRequestMethods.Http.Get;
                httpWebRequest.UserAgent = CheckerMainForm.httpUserAgent;
                httpWebRequest.Timeout = 5000;
                httpWebRequest.ReadWriteTimeout = 5000;

                HttpWebResponse httpWebResponse = null;

                try
                {
                    httpWebResponse = CheckerMainForm.GetHTTPWebResponse(httpWebRequest, 5);
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
                    }
                }
            }));
        }

        public void traceRoute_RouteNodeFound(object sender, VRK.Net.RouteNodeFoundEventArgs e)
        {
            if (e.Node.Address.ToString() != "0.0.0.0")
            {
                ListViewItem item = new ListViewItem((lv_RouteList.Items.Count + 1).ToString(), 3);
                item.SubItems.Add(e.Node.Address.ToString());
                item.SubItems.Add(e.Node.DNSName);
                item.SubItems.Add(e.Node.RoundTripTime);
                item.ToolTipText = "Mouse doubleclick to open in browser [HTTP]";

                ThreadSafeInvoke((Action)(() =>
                {
                    lv_RouteList.Items.Add(item);
                }));
            }
        }

        public void btn_TraceRoute_Refresh_Click(object sender, EventArgs e)
        {
            btn_TraceRoute_Refresh.Enabled = false;
            pb_TraceRouteProgress.Visible = true;
            traceRoute.HostNameOrAddress = cb_IPAddress.GetItemText(cb_IPAddress.SelectedItem);
            lv_RouteList.Items.Clear();

            NewBackgroundThread((Action)(() =>
            {
                try
                {
                    traceRoute.Trace();
                }
                catch
                {
                    traceRoute_Done(this, null);
                }
            }));
        }

        public void traceRoute_Done(object sender, EventArgs e)
        {
            ThreadSafeInvoke((Action)(() =>
            {
                btn_TraceRoute_Refresh.Enabled = true;
                pb_TraceRouteProgress.Visible = false;
            }));
        }

        public void lv_RouteList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lv_RouteList.SelectedItems.Count == 1)
            {
                string address = Uri.UriSchemeHttp + Uri.SchemeDelimiter + lv_RouteList.SelectedItems[0].SubItems[2].Text;

                try
                {
                    CheckerMainForm.StartBackgroundProcess(
                                                 address,
                                                 null,
                                                 null,
                                                 null);
                }
                catch (Exception exception)
                {
                    CheckerMainForm.ExceptionNotifier(exception);
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
                    CheckerMainForm.StartBackgroundProcess(
                                                 share,
                                                 null,
                                                 _selectedEndpoint.LoginName,
                                                 _selectedEndpoint.LoginPass);
                }
                catch (Exception exception)
                {
                    CheckerMainForm.ExceptionNotifier(exception);
                }
            }
        }

        public void pb_Browse_WindowsExplorer_MouseClick(object sender, MouseEventArgs e)
        {
            CheckerMainForm.BrowseEndpoint_WindowsExplorer(
                new Uri(_selectedEndpoint.ResponseAddress).Host,
                _selectedEndpoint.LoginName,
                _selectedEndpoint.LoginPass);
        }

        public void pb_AdminBrowse_WindowsExplorer_MouseClick(object sender, MouseEventArgs e)
        {
            CheckerMainForm.BrowseEndpoint_WindowsExplorer(
                new Uri(_selectedEndpoint.ResponseAddress).Host + @"\C$",
                _selectedEndpoint.LoginName,
                _selectedEndpoint.LoginPass);
        }

        public void pb_RDP_MouseClick(object sender, MouseEventArgs e)
        {
            CheckerMainForm.ConnectEndpoint_RDP(
                new Uri(_selectedEndpoint.ResponseAddress).Host);
        }

        public void pb_VNC_MouseClick(object sender, MouseEventArgs e)
        {
            CheckerMainForm.ConnectEndpoint_VNC(
                new Uri(_selectedEndpoint.ResponseAddress).Host);
        }

        public void pb_HTTP_MouseClick(object sender, MouseEventArgs e)
        {
            Uri endpointURI = new Uri(_selectedEndpoint.ResponseAddress);

            string connectionString =
                Uri.UriSchemeHttp +
                Uri.SchemeDelimiter +
                endpointURI.Authority +
                endpointURI.AbsolutePath;

            CheckerMainForm.BrowseEndpoint(
                connectionString,
                null,
                _selectedEndpoint.LoginName,
                _selectedEndpoint.LoginPass);
        }

        public void pb_FTP_MouseClick(object sender, MouseEventArgs e)
        {
            Uri endpointURI = new Uri(_selectedEndpoint.ResponseAddress);

            string connectionString =
                Uri.UriSchemeFtp +
                Uri.SchemeDelimiter +
                endpointURI.Authority +
                endpointURI.AbsolutePath;

            if (!string.IsNullOrEmpty(_selectedEndpoint.LoginName) &&
                !string.IsNullOrEmpty(_selectedEndpoint.LoginPass) &&
                _selectedEndpoint.LoginName != CheckerMainForm.status_NotAvailable &&
                _selectedEndpoint.LoginPass != CheckerMainForm.status_NotAvailable)
            {
                connectionString =
                    Uri.UriSchemeFtp +
                    Uri.SchemeDelimiter +
                    _selectedEndpoint.LoginName +
                    ":" +
                    _selectedEndpoint.LoginPass +
                    "@" +
                    endpointURI.Authority +
                    endpointURI.AbsolutePath;
            }

            CheckerMainForm.BrowseEndpoint(
                connectionString,
                null,
                null,
                null);
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
                pb_PingRefresh.Image = Properties.Resources.refresh;
                tb_PingTime.Text = _selectedEndpoint.PingRoundtripTime;
            }
        }

        public void TIMER_PingRefresh_Tick(object sender, EventArgs e)
        {
            NewBackgroundThread((Action)(() =>
            {
                try
                {
                    string pingRoundtripTime = CheckerMainForm.GetPingTime(new Uri(_selectedEndpoint.ResponseAddress).Host, _pingTimeout, 3);

                    if (!string.IsNullOrEmpty(pingRoundtripTime))
                    {
                        ThreadSafeInvoke((Action)(() =>
                        {
                            tb_PingTime.Text = pingRoundtripTime;
                            tb_PingTime.Text += " (1s refresh)";
                        }));
                    }
                }
                catch
                {
                }
            }));
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

                        if (managementObject.Properties["Name"].Value.ToString() ==
                            managementObject.Properties["Caption"].Value.ToString())
                        {
                            managementObjectNodeText =
                                managementObject.Properties["Name"].Value.ToString();
                        }
                        else
                        {
                            managementObjectNodeText =
                                managementObject.Properties["Name"].Value.ToString() +
                                " (" +
                                managementObject.Properties["Caption"].Value.ToString() +
                                ")";
                        }

                        TreeNode managementObjectNode = new TreeNode(managementObjectNodeText, 8, 8);

                        foreach (PropertyData propertyData in managementObject.Properties)
                        {
                            if (propertyData.Value != null &&
                                !string.IsNullOrEmpty(propertyData.Value.ToString()))
                            {
                                TreeNode dataNode = new TreeNode(propertyData.Name);
                                dataNode.ImageIndex = 11;
                                dataNode.SelectedImageIndex = 11;

                                switch (propertyData.Value.GetType().ToString())
                                {
                                    case "System.String[]":
                                        string[] str = (string[])propertyData.Value;

                                        foreach (string st in str)
                                            dataNode.Nodes.Add(new TreeNode(st.ToString()));
                                        break;

                                    case "System.UInt16[]":
                                        ushort[] shortData = (ushort[])propertyData.Value;

                                        foreach (ushort st in shortData)
                                            dataNode.Nodes.Add(new TreeNode(st.ToString()));
                                        break;

                                    default:
                                        dataNode.Nodes.Add(propertyData.Value.ToString());
                                        break;
                                }

                                managementObjectNode.Nodes.Add(dataNode);
                            }
                        }

                        classNode.Nodes.Add(managementObjectNode);
                    }

                    if (classNode.Nodes.Count > 0)
                    {
                        ThreadSafeInvoke((Action)(() =>
                        {
                            treeView_ComputerInfo.Nodes.Add(classNode);
                        }));
                    }
                }
                catch
                {
                }
            }

            ThreadSafeInvoke((Action)(() =>
            {
                btn_WMIInfo_Refresh.Enabled = true;
                pb_WMIInfoProgress.Visible = false;
            }));
        }

        public void btn_WMIInfo_Refresh_Click(object sender, EventArgs e)
        {
            string ipAddress = cb_IPAddress.GetItemText(cb_IPAddress.SelectedItem);

            lbl_WMI_Exception.Visible = false;
            treeView_ComputerInfo.Nodes.Clear();
            btn_WMIInfo_Refresh.Enabled = false;
            pb_WMIInfoProgress.Visible = true;

            NewBackgroundThread((Action)(() =>
            {
                wmiConnectionScope.Path = new ManagementPath(@"\\" + ipAddress + @"\root\CIMV2");

                wmiConnectionScope.Options = new ConnectionOptions();
                wmiConnectionScope.Options.Impersonation = ImpersonationLevel.Impersonate;
                wmiConnectionScope.Options.Authentication = AuthenticationLevel.Default;
                wmiConnectionScope.Options.EnablePrivileges = true;

                if (!CheckerMainForm.IsLocalHost(new Uri(_selectedEndpoint.ResponseAddress).Host) &&
                    _selectedEndpoint.LoginName != CheckerMainForm.status_NotAvailable &&
                    _selectedEndpoint.LoginPass != CheckerMainForm.status_NotAvailable)
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
                    ThreadSafeInvoke((Action)(() =>
                    {
                        lbl_WMI_Exception.Text = ex.Message.Split('(')[0].TrimStart().TrimEnd();
                    }));
                }

                if (wmiConnectionScope.IsConnected)
                {
                    // CONNECTED - GET COMPUTER INFORMATION
                    GetComputerInformationByWMI();
                }
                else
                {
                    // NOT CONNECTED - SHOW ERROR MESSAGE
                    ThreadSafeInvoke((Action)(() =>
                    {
                        lbl_WMI_Exception.Visible = true;
                        btn_WMIInfo_Refresh.Enabled = true;
                        pb_WMIInfoProgress.Visible = false;
                    }));
                }
            }));
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

            ThreadSafeInvoke((Action)(() =>
            {
                ipAddress = cb_IPAddress.GetItemText(cb_IPAddress.SelectedItem);
            }));

            Parallel.ForEach(portsList, portItem =>
            {
                bool portOpen = true;

                // CHECK PORT
                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
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

                ThreadSafeInvoke((Action)(() =>
                {
                    lv_Ports.BeginUpdate();

                    lv_Ports.Items.Add(PortListViewItem(
                        portOpen,
                        portItem.Key.ToString(),
                        portItem.Value));

                    lv_Ports.EndUpdate();
                }));
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

            portItem.SubItems.Add(portNumber);
            portItem.SubItems.Add(portDescription);

            return portItem;
        }

        public void BW_PortCheck_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            btn_Ports_Refresh.Enabled = true;
            pb_PortsProgress.Visible = false;
        }

        public void GetWhoIsInfo()
        {
            NewBackgroundThread((Action)(() =>
            {
                // WHOIS DATA LIST
                List<Property> whoisData = new List<Property>();

                // DOMAIN INFO
                DomainName domainInfo = null;

                try
                {
                    TLDRulesCache.Init(CheckerMainForm.tldRulesCacheFile);

                    // TRY TO GET REGISTRABLE DOMAIN NAME
                    if (DomainName.TryParse(
                         new Uri(
                             _selectedEndpoint.ResponseAddress)
                             .Host
                             .Replace(
                                 "www.",
                                 string.Empty),
                             out domainInfo))
                    {
                        string registrableDomainName = domainInfo.RegistrableDomain;
                        ThreadSafeInvoke((Action)(() =>
                        {
                            tb_WhoIs_RegistrableDomain.Text = registrableDomainName;
                        }));

                        // GET WHOIS SERVER
                        string whoisServer = DomainToWhoisServerList.Default.FindWhoisServer(registrableDomainName);
                        ThreadSafeInvoke((Action)(() =>
                        {
                            tb_WhoIs_Server.Text = whoisServer;
                        }));

                        // GET RESPONSE FROM WHOIS SERVER
                        string whoisRawResponse = WhoisClient.RawQuery(
                                                                registrableDomainName,
                                                                whoisServer,
                                                                43,
                                                                Encoding.UTF8)
                                                                    .TrimStart()
                                                                    .TrimEnd();

                        // PARSE PROPERTIES FROM RESPONSE
                        string previousKey = string.Empty;

                        foreach (string whoisDataItem in whoisRawResponse.Split(new char[] { '\r', '\n' }))
                        {
                            // FILTER EMPTY LINES AND COMMENTS
                            if (string.IsNullOrEmpty(whoisDataItem) ||
                                string.IsNullOrEmpty(whoisDataItem.Trim()))
                            {
                                previousKey = string.Empty;
                            }
                            else if (whoisDataItem.Length > 0 && whoisDataItem.TrimStart()[0] != '%')
                            {
                                string[] whoisDataItemValues = whoisDataItem.Split(new char[] { ':' }, 2);
                                string key = whoisDataItemValues[0].TrimStart().TrimEnd();

                                if (whoisDataItemValues.Length == 2)
                                {
                                    string value = whoisDataItemValues[1].TrimStart().TrimEnd();

                                    if (string.IsNullOrEmpty(value))
                                    {
                                        previousKey = key;
                                    }
                                    else if (!string.IsNullOrEmpty(previousKey))
                                    {
                                        whoisData.Add(new Property { ItemName = previousKey + "/" + key, ItemValue = value });
                                    }
                                    else
                                    {
                                        whoisData.Add(new Property { ItemName = key, ItemValue = value });
                                    }
                                }
                                else if (!string.IsNullOrEmpty(previousKey))
                                {
                                    whoisData.Add(new Property { ItemName = key, ItemValue = key });
                                }
                            }
                        }
                    }
                }
                catch
                {
                }

                ThreadSafeInvoke((Action)(() =>
                {
                    if (domainInfo != null &&
                        whoisData.Count > 0)
                    {
                        foreach (Property whoisDataItem in whoisData)
                        {
                            ListViewItem lvItem = new ListViewItem();

                            if (!string.IsNullOrEmpty(whoisDataItem.ItemName) &&
                                !string.IsNullOrEmpty(whoisDataItem.ItemValue))
                            {
                                lvItem.ImageIndex = 6;
                            }

                            lvItem.Text = whoisDataItem.ItemName;
                            lvItem.SubItems.Add(whoisDataItem.ItemValue);

                            lv_WhoIs.Items.Add(lvItem);
                        }

                        pb_WhoIsProgress.Visible = false;
                        lv_WhoIs.Visible = true;

                        // ADD WHOIS TAB PAGE
                        tabControl.TabPages.Add(tabPage_WhoIs);
                    }
                }));
            }));
        }

        public void GetPageCategoryListFromHTMLMeta()
        {
            List<Property> keywordTagList = _selectedEndpoint.HTMLMetaInfo.PropertyItem.Where(metaInfo => metaInfo.ItemName.ToLower() == "keywords").ToList();

            foreach (Property keywordTag in keywordTagList)
            {
                foreach (string keywordItem in keywordTag.ItemValue.Split(new Char[] { ',', ';' }))
                {
                    string keywordValue = keywordItem.TrimStart().TrimEnd();

                    if (!string.IsNullOrEmpty(keywordValue) &&
                        !lv_CategoryList.Items.ContainsKey(keywordValue))
                    {
                        ListViewItem categoryItem = new ListViewItem();
                        categoryItem.Text = keywordValue.First().ToString().ToUpper() + keywordValue.Substring(1).ToLower();
                        categoryItem.ToolTipText = "Doubleclick category item to browse search";
                        categoryItem.ImageIndex = 27;

                        lv_CategoryList.Items.Add(categoryItem);
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

            string macAddressVendor = CheckerMainForm.status_NotAvailable;

            NewBackgroundThread((Action)(() =>
            {
                if (macAddress != CheckerMainForm.status_NotAvailable)
                {
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(macLookupAPI + macAddress);
                    httpWebRequest.Method = WebRequestMethods.Http.Get;
                    httpWebRequest.UserAgent = CheckerMainForm.httpUserAgent;
                    httpWebRequest.Timeout = 5000;
                    httpWebRequest.ReadWriteTimeout = 5000;

                    HttpWebResponse httpWebResponse = null;

                    try
                    {
                        httpWebResponse = CheckerMainForm.GetHTTPWebResponse(httpWebRequest, 5);

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
                                    httpWebRequest.UserAgent = CheckerMainForm.httpUserAgent;
                                    httpWebRequest.Timeout = 5000;
                                    httpWebRequest.ReadWriteTimeout = 5000;

                                    httpWebResponse = null;

                                    try
                                    {
                                        httpWebResponse = CheckerMainForm.GetHTTPWebResponse(httpWebRequest, 5);

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
                                                            ThreadSafeInvoke((Action)(() =>
                                                            {
                                                                pb_Vendor.Image = CheckerMainForm.ResizeImage(vendorImage, 23, 23);

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
                                                            }));

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
                        }
                    }
                }

                ThreadSafeInvoke((Action)(() =>
                {
                    tb_MACVendor.Text = macAddressVendor;
                    pb_MACVendorProgress.Visible = false;
                }));
            }));
        }

        public void GetIPGeoInfo(string ipAddress)
        {
            NewBackgroundThread((Action)(() =>
            {
                ThreadSafeInvoke((Action)(() =>
                {
                    bool showTab = false;

                    if (ipAddress != CheckerMainForm.status_NotAvailable)
                    {
                        try
                        {
                            XmlDocument doc = new XmlDocument();
                            doc.Load("http://ip-api.com/xml/" + ipAddress);

                            if (!string.IsNullOrEmpty(doc.DocumentElement.ChildNodes[0].InnerText) &&
                                doc.DocumentElement.ChildNodes[0].InnerText == "success" &&
                                !string.IsNullOrEmpty(doc.DocumentElement.ChildNodes[7].InnerText) &&
                                !string.IsNullOrEmpty(doc.DocumentElement.ChildNodes[8].InnerText) &&
                                doc.DocumentElement.ChildNodes[7].InnerText != "0" &&
                                doc.DocumentElement.ChildNodes[8].InnerText != "0")
                            {
                                Bitmap mapTile = GetImageFromGoogleMapsAPI(
                                    doc.DocumentElement.ChildNodes[7].InnerText,
                                    doc.DocumentElement.ChildNodes[8].InnerText);

                                if (mapTile != null)
                                {
                                    // LAT, LON
                                    tb_GeoLocation_Latitude.Text = doc.DocumentElement.ChildNodes[7].InnerText.TrimStart().TrimEnd();
                                    tb_GeoLocation_Longitude.Text = doc.DocumentElement.ChildNodes[8].InnerText.TrimStart().TrimEnd();

                                    // MAP TILE
                                    pb_GeoLocation_Map.Image = mapTile;

                                    // GET COUNTRY FLAG
                                    Bitmap countryFlag = GetCountryFlagByCode(
                                    doc.DocumentElement.ChildNodes[2].InnerText);

                                    if (countryFlag != null)
                                    {
                                        pb_GeoLocation_CountryFlag.Image = countryFlag;
                                        pb_GeoLocation_CountryFlag.Visible = true;
                                    }
                                    else
                                    {
                                        pb_GeoLocation_CountryFlag.Visible = false;
                                    }

                                    // OTHER PROPERTIES
                                    tb_GeoLocation_ISP.Text = DefaultIfNullorEmpty(doc.DocumentElement.ChildNodes[10].InnerText.TrimStart().TrimEnd());
                                    tb_GeoLocation_AS.Text = DefaultIfNullorEmpty(doc.DocumentElement.ChildNodes[12].InnerText.TrimStart().TrimEnd());

                                    if (!string.IsNullOrEmpty(tb_GeoLocation_ORG.Text))
                                    {
                                        tb_GeoLocation_AS.Text = tb_GeoLocation_AS.Text.Replace(tb_GeoLocation_ORG.Text, string.Empty).TrimStart().TrimEnd();
                                    }

                                    tb_GeoLocation_RegionName.Text = DefaultIfNullorEmpty(doc.DocumentElement.ChildNodes[4].InnerText);
                                    tb_GeoLocation_TimeZone.Text = DefaultIfNullorEmpty(doc.DocumentElement.ChildNodes[9].InnerText);
                                    tb_GeoLocation_ZipCode.Text = DefaultIfNullorEmpty(doc.DocumentElement.ChildNodes[6].InnerText);
                                    tb_GeoLocation_City.Text = DefaultIfNullorEmpty(doc.DocumentElement.ChildNodes[5].InnerText);
                                    tb_GeoLocation_ORG.Text = DefaultIfNullorEmpty(doc.DocumentElement.ChildNodes[11].InnerText);
                                    tb_GeoLocation_CountryName.Text = DefaultIfNullorEmpty(doc.DocumentElement.ChildNodes[1].InnerText);
                                    tb_GeoLocation_RegionName.Text = DefaultIfNullorEmpty(doc.DocumentElement.ChildNodes[4].InnerText);

                                    if (!string.IsNullOrEmpty(doc.DocumentElement.ChildNodes[2].InnerText))
                                    {
                                        tb_GeoLocation_CountryName.Text += " (" + doc.DocumentElement.ChildNodes[2].InnerText + ")";
                                    }

                                    if (!string.IsNullOrEmpty(doc.DocumentElement.ChildNodes[3].InnerText))
                                    {
                                        tb_GeoLocation_RegionName.Text += " (" + doc.DocumentElement.ChildNodes[3].InnerText + ")";
                                    }

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
                }));
            }));
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
                httpWebRequest.UserAgent = CheckerMainForm.httpUserAgent;
                httpWebRequest.Timeout = 5000;
                httpWebRequest.ReadWriteTimeout = 5000;

                httpWebResponse = CheckerMainForm.GetHTTPWebResponse(httpWebRequest, 5);

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
                CheckerMainForm.googleMapsZoomFactor +
                "&size=400x400" +
                "&maptype=hybrid" +
                "&markers=color:green%7Clabel:A%7C" +
                latlng +
                "&key=" + CheckerMainForm.apiKey_GoogleMaps;

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(path);
                httpWebRequest.Method = WebRequestMethods.Http.Get;
                httpWebRequest.UserAgent = CheckerMainForm.httpUserAgent;
                httpWebRequest.Timeout = 5000;
                httpWebRequest.ReadWriteTimeout = 5000;

                httpWebResponse = CheckerMainForm.GetHTTPWebResponse(httpWebRequest, 5);

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

                Invoke(action);
            }
            catch
            {
            }
        }

        public static string DefaultIfNullorEmpty(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return CheckerMainForm.status_NotAvailable;
            }
            else
            {
                return input;
            }
        }

        public void pb_GeoLocation_Map_Click(object sender, EventArgs e)
        {
            try
            {
                CheckerMainForm.StartBackgroundProcess(
                    "https://www.google.com/maps/@?api=1&map_action=map&center=" +
                    tb_GeoLocation_Latitude.Text +
                    "," +
                    tb_GeoLocation_Longitude.Text +
                    "&zoom=" +
                    CheckerMainForm.googleMapsZoomFactor +
                    "&basemap=satellite",
                    null,
                    null,
                    null);
            }
            catch (Exception exception)
            {
                CheckerMainForm.ExceptionNotifier(exception);
            }
        }

        public void TIMER_LoadProperties_Tick(object sender, EventArgs e)
        {
            TIMER_LoadProperties.Stop();
            TIMER_LoadProperties.Enabled = false;

            LoadProperties();
        }

        public void cb_IPAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            // DNS NAME
            if (_selectedEndpoint.DNSName.Length == _selectedEndpoint.IPAddress.Length)
            {
                tb_DNSName.Text = _selectedEndpoint.DNSName[cb_IPAddress.SelectedIndex];
            }
            else
            {
                tb_DNSName.Text = string.Join(", ", _selectedEndpoint.DNSName);
            }

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
                tb_MACAddress.Text = CheckerMainForm.status_NotAvailable;
                tb_MACVendor.Text = CheckerMainForm.status_NotAvailable;
            }

            // GET IP ADDRESS GEO INFO
            GetIPGeoInfo(_selectedEndpoint.IPAddress[cb_IPAddress.SelectedIndex]);

            // IP ADDRESS PRESENT
            if (cb_IPAddress.GetItemText(cb_IPAddress.SelectedItem) != CheckerMainForm.status_NotAvailable)
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
            if (btn_VirusTotal_Refresh.Enabled)
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
                httpWebRequest.UserAgent = CheckerMainForm.httpUserAgent;
                httpWebRequest.Timeout = 5000;
                httpWebRequest.ReadWriteTimeout = 5000;

                httpWebResponse = CheckerMainForm.GetHTTPWebResponse(httpWebRequest, 5);

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
                    CheckerMainForm.StartBackgroundProcess(
                                                 macVendorWebPage,
                                                 null,
                                                 null,
                                                 null);
                }
                catch (Exception exception)
                {
                    CheckerMainForm.ExceptionNotifier(exception);
                }
            }
        }

        public void btn_VirusTotal_Refresh_Click(object sender, EventArgs e)
        {
            // SET CONTROLS
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

            GetVirusTotalScanReport(tb_ResponseURL.Text, 0);
        }

        public void GetVirusTotalScanReport(string urlToScan, int retry)
        {
            virusTotal_ScanResult = null;

            lbl_VirusTotal_Status.ForeColor = Color.DarkGreen;

            NewBackgroundThread((Action)(() =>
            {
                try
                {
                    // REQUEST SCAN REPORT
                    VirusTotal virusTotal = new VirusTotal(CheckerMainForm.apiKey_VirusTotal);
                    virusTotal.UseTLS = true;
                    virusTotal.UserAgent = CheckerMainForm.httpUserAgent;
                    Task<UrlScanResult> virusTotal_ScanResultTask = virusTotal.ScanUrlAsync(urlToScan);
                    virusTotal_ScanResult = virusTotal_ScanResultTask.Result;

                    if (virusTotal_ScanResult.ResponseCode == VirusTotalNET.ResponseCodes.UrlScanResponseCode.Queued)
                    {
                        ThreadSafeInvoke((Action)(() =>
                        {
                            lbl_VirusTotal_Status.Text = virusTotal_ScanResultTask.Result.VerboseMsg;

                            if (retry > 0)
                            {
                                lbl_VirusTotal_Status.Text += " [Retry " + retry + "]";
                            }
                        }));
                    }
                    else
                    {
                        throw new VirusTotalNET.Exceptions.InvalidResourceException(virusTotal_ScanResult.VerboseMsg);
                    }
                }
                catch (Exception vtException)
                {
                    if (!vtException.GetType().IsAssignableFrom(typeof(VirusTotalNET.Exceptions.InvalidResourceException)))
                    {
                        ThreadSafeInvoke((Action)(() =>
                        {
                            lbl_VirusTotal_Status.ForeColor = Color.Firebrick;

                            if (vtException.InnerException != null)
                            {
                                if (vtException.InnerException.Message != null)
                                {
                                    lbl_VirusTotal_Status.Text = vtException.InnerException.Message;
                                }
                                else
                                {
                                    lbl_VirusTotal_Status.Text = vtException.InnerException.ToString();
                                }
                            }
                            else
                            {
                                lbl_VirusTotal_Status.Text = vtException.ToString();
                            }

                            if (retry > 0)
                            {
                                lbl_VirusTotal_Status.Text += " [Retry " + retry + "]";
                            }
                        }));

                        Thread.Sleep(5000);

                        retry++;

                        GetVirusTotalScanReport(urlToScan, retry);
                    }
                    else
                    {
                        ThreadSafeInvoke((Action)(() =>
                        {
                            tabControl.TabPages.Remove(tabPage_VirusTotal);
                        }));
                    }
                }
            }));
        }

        public void tb_VirusTotal_Permalink_MouseClick(object sender, MouseEventArgs e)
        {
            CheckerMainForm.BrowseEndpoint(
                tb_VirusTotal_Permalink.Text,
                null,
                null,
                null);
        }

        public void TIMER_VirusTotalResult_Tick(object sender, EventArgs e)
        {
            if (virusTotal_ScanResult != null &&
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
                VirusTotal virusTotal = new VirusTotal(CheckerMainForm.apiKey_VirusTotal);
                virusTotal.UseTLS = true;
                virusTotal.UserAgent = CheckerMainForm.httpUserAgent;
                Task<UrlReport> virusTotalReportTask = virusTotal.GetUrlReportAsync(virusTotal_ScanResult.Url);
                UrlReport virusTotalReport = virusTotalReportTask.Result;
                virusTotalReportTask.Dispose();

                if (virusTotalReport.ResponseCode == VirusTotalNET.ResponseCodes.UrlReportResponseCode.Present)
                {
                    virusTotal_ScanResult = null;

                    ThreadSafeInvoke((Action)(() =>
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
                                scanResultItem.SubItems.Add(urlScan.Result);

                                if (urlScan.Result == "clean site")
                                {
                                    scanResultItem.ImageIndex = 10;
                                }
                                else if (urlScan.Result == "unrated site")
                                {
                                    scanResultItem.ImageIndex = 14;
                                }
                                else
                                {
                                    scanResultItem.ImageIndex = 13;
                                }

                                lv_VirusTotal.Items.Add(scanResultItem);
                            }
                        }
                    }));
                }
                else
                {
                    ThreadSafeInvoke((Action)(() =>
                    {
                        lbl_VirusTotal_Status.Text = "Waiting for Scan Report ...";
                    }));
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
                    CheckerMainForm.StartBackgroundProcess(
                                                 address,
                                                 null,
                                                 null,
                                                 null);
                }
                catch (Exception exception)
                {
                    CheckerMainForm.ExceptionNotifier(exception);
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

            NewBackgroundThread((Action)(() =>
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
                            httpWebRequest.UserAgent = CheckerMainForm.httpUserAgent;
                            httpWebRequest.Timeout = 10000;
                            httpWebRequest.ReadWriteTimeout = 10000;
                            httpWebRequest.AllowAutoRedirect = true;
                            httpWebRequest.ProtocolVersion = HttpVersion.Version11;

                            httpWebResponse = CheckerMainForm.GetHTTPWebResponse(httpWebRequest, 5);

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
                            }
                        }

                        checkedLinksList.Add(new Tuple<string, string, string>(linkStatus, pageLink.ItemName, pageLink.ItemValue));
                    }

                    Application.DoEvents();
                }

                ThreadSafeInvoke((Action)(() =>
                {
                    // UPDATE LIST
                    lv_PageLinks.BeginUpdate();
                    lv_PageLinks.Items.Clear();

                    foreach (Tuple<string, string, string> checkedLinkStatus in checkedLinksList)
                    {
                        ListViewItem linkItem = new ListViewItem();
                        linkItem.Text = checkedLinkStatus.Item1;

                        if (linkItem.Text == "VALID")
                        {
                            linkItem.ImageIndex = 10;
                        }
                        else if (linkItem.Text == "INVALID")
                        {
                            linkItem.ImageIndex = 9;
                        }

                        linkItem.SubItems.Add(checkedLinkStatus.Item2);
                        linkItem.SubItems.Add(checkedLinkStatus.Item3);

                        lv_PageLinks.Items.Add(linkItem);
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

                        if (isAnyInvalid)
                        {
                            pb_PageLinks_CommonLinksStatus.Image = Properties.Resources.linkStatus_Invalid;
                        }
                        else
                        {
                            pb_PageLinks_CommonLinksStatus.Image = Properties.Resources.linkStatus_Valid;
                        }

                        pb_PageLinks_CommonLinksStatus.Visible = true;
                    }
                }));
            }));
        }

        public void lv_CategoryList_ItemActivate(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection selectedItems = ((ListView)sender).SelectedItems;

            foreach (ListViewItem selectedItem in selectedItems)
            {
                CheckerMainForm.BrowseEndpoint(
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
            CheckerMainForm.TextBox_SetPasswordVisibilty(
                tb_UserPassword,
                true);
        }

        public void pb_ShowPassword_MouseUp(object sender, MouseEventArgs e)
        {
            CheckerMainForm.TextBox_SetPasswordVisibilty(
                tb_UserPassword,
                false);
        }

        public void tb_UserPassword_TextChanged(object sender, EventArgs e)
        {
            CheckerMainForm.TextBox_SetPasswordVisibilty(
                tb_UserPassword,
                tb_UserPassword.Text == CheckerMainForm.status_NotAvailable);

            pb_ShowPassword.Visible =
                !(tb_UserPassword.Text == CheckerMainForm.status_NotAvailable);
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

    public class IpInfo
    {

        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty("hostname")]
        public string Hostname { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("loc")]
        public string Loc { get; set; }

        [JsonProperty("org")]
        public string Org { get; set; }

        [JsonProperty("postal")]
        public string Postal { get; set; }
    }
}
