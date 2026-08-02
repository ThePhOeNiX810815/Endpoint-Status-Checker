using EndpointChecker.Properties;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace EndpointChecker
{
    public class ConfigDialog : Form
    {
        private readonly CheckerMainForm _owner;

        // Tab 1 — Refresh & Notifications
        private CheckBox cb_AutoRefresh;
        private CheckBox cb_ContinuousRefresh;
        private CheckBox cb_AutoAdjustInterval;
        private CheckBox cb_ScanOnStartup;
        private CheckBox cb_TrayNotify;
        private NumericUpDown num_RefreshInterval;

        // Tab 2 — Scan
        private ComboBox cmb_ValidationMethod;
        private CheckBox cb_TestPing;
        private CheckBox cb_AllowRedirect;
        private CheckBox cb_ValidateSSL;
        private NumericUpDown num_PingTimeout;
        private NumericUpDown num_HttpTimeout;
        private NumericUpDown num_FtpTimeout;
        private NumericUpDown num_ParallelThreads;

        // Tab 3 — Resolution
        private CheckBox cb_ResolveDns;
        private CheckBox cb_ResolveIp;
        private CheckBox cb_ResolveMac;
        private CheckBox cb_ResolveShares;
        private CheckBox cb_PageMetaInfo;
        private CheckBox cb_RemoveUrlParams;
        private CheckBox cb_PageLinks;
        private CheckBox cb_SaveResponse;

        // Tab 4 — Export
        private CheckBox cb_ExportXlsx;
        private CheckBox cb_ExportJson;
        private CheckBox cb_ExportXml;
        private CheckBox cb_ExportHtml;
        private TextBox txt_ExportDir;

        // Tab 5 — Tools & API Keys
        private TextBox txt_VncPath;
        private TextBox txt_PuttyPath;
        private TextBox txt_VirusTotal;
        private TextBox txt_GoogleMaps;

        public ConfigDialog(CheckerMainForm owner)
        {
            _owner = owner;
            BuildUI();
            LoadSettings();
            CheckerMainForm.ApplyDarkTheme(this);
        }

        // ── UI construction ────────────────────────────────────────────────────

        private void BuildUI()
        {
            Text = "Configuration";
            Size = new Size(600, 510);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9f);

            var tabs = new TabControl { Dock = DockStyle.Fill };
            tabs.TabPages.Add(BuildRefreshTab());
            tabs.TabPages.Add(BuildScanTab());
            tabs.TabPages.Add(BuildResolutionTab());
            tabs.TabPages.Add(BuildExportTab());
            tabs.TabPages.Add(BuildToolsTab());

            var footer = new Panel { Dock = DockStyle.Bottom, Height = 46 };
            var btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Size = new Size(88, 28), Location = new Point(392, 9) };
            var btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Size = new Size(88, 28), Location = new Point(488, 9) };
            btnOk.Click += BtnOk_Click;
            footer.Controls.AddRange(new Control[] { btnOk, btnCancel });

            Controls.Add(tabs);
            Controls.Add(footer);
            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }

        private TabPage BuildRefreshTab()
        {
            var tab = new TabPage("Refresh & Notifications");
            int y = 16;

            cb_AutoRefresh = AddCheck(tab, "Enable automatic refresh", ref y);
            cb_ContinuousRefresh = AddCheck(tab, "Continuous refresh (loop)", ref y);
            cb_AutoAdjustInterval = AddCheck(tab, "Auto-adjust refresh interval", ref y);
            cb_ScanOnStartup = AddCheck(tab, "Scan on startup", ref y);
            cb_TrayNotify = AddCheck(tab, "Tray balloon notification on error", ref y);

            y += 10;
            AddLabel(tab, "Refresh interval (minutes):", 14, y);
            num_RefreshInterval = new NumericUpDown
            {
                Location = new Point(230, y - 2),
                Width = 80,
                Minimum = 0,
                Maximum = 1440,
                DecimalPlaces = 0
            };
            tab.Controls.Add(num_RefreshInterval);

            return tab;
        }

        private TabPage BuildScanTab()
        {
            var tab = new TabPage("Scan");
            int y = 16;

            AddLabel(tab, "Validation method:", 14, y);
            cmb_ValidationMethod = new ComboBox
            {
                Location = new Point(190, y - 2),
                Width = 180,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmb_ValidationMethod.Items.Add("Protocol");
            cmb_ValidationMethod.Items.Add("Ping");
            tab.Controls.Add(cmb_ValidationMethod);
            y += 32;

            cb_TestPing = AddCheck(tab, "Test Ping (ICMP)", ref y);
            cb_AllowRedirect = AddCheck(tab, "Allow automatic HTTP redirects", ref y);
            cb_ValidateSSL = AddCheck(tab, "Validate SSL certificate", ref y);
            y += 10;

            AddLabel(tab, "Ping timeout (seconds):", 14, y);
            num_PingTimeout = new NumericUpDown { Location = new Point(230, y - 2), Width = 80, Minimum = 0, Maximum = 300 };
            tab.Controls.Add(num_PingTimeout);
            y += 28;

            AddLabel(tab, "HTTP request timeout (sec):", 14, y);
            num_HttpTimeout = new NumericUpDown { Location = new Point(230, y - 2), Width = 80, Minimum = 0, Maximum = 300 };
            tab.Controls.Add(num_HttpTimeout);
            y += 28;

            AddLabel(tab, "FTP request timeout (sec):", 14, y);
            num_FtpTimeout = new NumericUpDown { Location = new Point(230, y - 2), Width = 80, Minimum = 0, Maximum = 300 };
            tab.Controls.Add(num_FtpTimeout);
            y += 28;

            AddLabel(tab, "Parallel scan threads:", 14, y);
            num_ParallelThreads = new NumericUpDown { Location = new Point(230, y - 2), Width = 80, Minimum = 1, Maximum = 64 };
            tab.Controls.Add(num_ParallelThreads);

            return tab;
        }

        private TabPage BuildResolutionTab()
        {
            var tab = new TabPage("Resolution");
            int y = 16;

            cb_ResolveDns = AddCheck(tab, "Resolve DNS names", ref y);
            cb_ResolveIp = AddCheck(tab, "Resolve IP addresses", ref y);
            cb_ResolveMac = AddCheck(tab, "Resolve MAC addresses", ref y);
            cb_ResolveShares = AddCheck(tab, "Resolve network shares", ref y);
            cb_PageMetaInfo = AddCheck(tab, "Resolve page meta info (HTML)", ref y);
            cb_RemoveUrlParams = AddCheck(tab, "Remove URL parameters", ref y);
            cb_PageLinks = AddCheck(tab, "Resolve page links (HTML)", ref y);
            cb_SaveResponse = AddCheck(tab, "Save HTTP response to disk", ref y);

            return tab;
        }

        private TabPage BuildExportTab()
        {
            var tab = new TabPage("Export");
            int y = 16;

            cb_ExportXlsx = AddCheck(tab, "Export to XLSX (Excel)", ref y);
            cb_ExportJson = AddCheck(tab, "Export to JSON", ref y);
            cb_ExportXml = AddCheck(tab, "Export to XML", ref y);
            cb_ExportHtml = AddCheck(tab, "Export to HTML", ref y);
            y += 10;

            AddLabel(tab, "Export directory:", 14, y);
            txt_ExportDir = new TextBox { Location = new Point(14, y + 22), Width = 440, ReadOnly = true };
            var btnBrowse = new Button { Text = "Browse...", Location = new Point(460, y + 20), Size = new Size(88, 24) };
            btnBrowse.Click += (s, e) =>
            {
                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    fbd.Description = "Select export directory";
                    if (!string.IsNullOrEmpty(txt_ExportDir.Text) && Directory.Exists(txt_ExportDir.Text))
                        fbd.SelectedPath = txt_ExportDir.Text;
                    if (fbd.ShowDialog() == DialogResult.OK)
                        txt_ExportDir.Text = fbd.SelectedPath;
                }
            };
            tab.Controls.AddRange(new Control[] { txt_ExportDir, btnBrowse });

            return tab;
        }

        private TabPage BuildToolsTab()
        {
            var tab = new TabPage("Tools & API Keys");
            int y = 16;

            AddLabel(tab, "VNC Viewer executable:", 14, y);
            txt_VncPath = new TextBox { Location = new Point(14, y + 22), Width = 440, ReadOnly = true };
            var btnVnc = new Button { Text = "Browse...", Location = new Point(460, y + 20), Size = new Size(88, 24) };
            btnVnc.Click += (s, e) => BrowseExe(txt_VncPath);
            tab.Controls.AddRange(new Control[] { txt_VncPath, btnVnc });
            y += 58;

            AddLabel(tab, "PuTTY executable:", 14, y);
            txt_PuttyPath = new TextBox { Location = new Point(14, y + 22), Width = 440, ReadOnly = true };
            var btnPutty = new Button { Text = "Browse...", Location = new Point(460, y + 20), Size = new Size(88, 24) };
            btnPutty.Click += (s, e) => BrowseExe(txt_PuttyPath);
            tab.Controls.AddRange(new Control[] { txt_PuttyPath, btnPutty });
            y += 58;

            AddLabel(tab, "VirusTotal API key:", 14, y);
            txt_VirusTotal = new TextBox { Location = new Point(14, y + 22), Width = 534 };
            tab.Controls.Add(txt_VirusTotal);
            y += 58;

            AddLabel(tab, "Google Maps API key:", 14, y);
            txt_GoogleMaps = new TextBox { Location = new Point(14, y + 22), Width = 534 };
            tab.Controls.Add(txt_GoogleMaps);

            return tab;
        }

        // ── Settings I/O ───────────────────────────────────────────────────────

        private void LoadSettings()
        {
            var s = Settings.Default;

            cb_AutoRefresh.Checked = s.Config_EnableAutomaticRefresh;
            cb_ContinuousRefresh.Checked = s.Config_EnableContinuousRefresh;
            cb_AutoAdjustInterval.Checked = s.Config_AutoAdjustRefreshInterval;
            cb_ScanOnStartup.Checked = s.Config_ScanOnStartup;
            cb_TrayNotify.Checked = s.Config_EnableTrayNotificationsOnError;
            num_RefreshInterval.Value = Math.Max(num_RefreshInterval.Minimum,
                                            Math.Min(num_RefreshInterval.Maximum, s.Config_AutomaticRefreshIntervalSeconds));

            int vIdx = s.Config_ValidationMethod;
            cmb_ValidationMethod.SelectedIndex = (vIdx >= 0 && vIdx < cmb_ValidationMethod.Items.Count) ? vIdx : 0;
            cb_TestPing.Checked = s.Config_TestPing;
            cb_AllowRedirect.Checked = s.Config_AllowAutoRedirect;
            cb_ValidateSSL.Checked = s.Config_ValidateSSLCertificate;
            num_PingTimeout.Value = Clamp(s.Config_PingTimeoutSeconds, num_PingTimeout.Minimum, num_PingTimeout.Maximum);
            num_HttpTimeout.Value = Clamp(s.Config_HTTP_RequestTimeoutSeconds, num_HttpTimeout.Minimum, num_HttpTimeout.Maximum);
            num_FtpTimeout.Value = Clamp(s.Config_FTP_RequestTimeoutSeconds, num_FtpTimeout.Minimum, num_FtpTimeout.Maximum);
            num_ParallelThreads.Value = Clamp(s.Config_ParallelThreadsCount, num_ParallelThreads.Minimum, num_ParallelThreads.Maximum);

            cb_ResolveDns.Checked = s.Config_Resolve_DNS_Names;
            cb_ResolveIp.Checked = s.Config_Resolve_IP_Addresses;
            cb_ResolveMac.Checked = s.Config_Resolve_MAC_Addresses;
            cb_ResolveShares.Checked = s.Config_ResolveNetworkShares;
            cb_PageMetaInfo.Checked = s.Config_ResolvePageMetaInfo;
            cb_RemoveUrlParams.Checked = s.Config_RemoveURLParameters;
            cb_PageLinks.Checked = s.Config_ResolvePageLinks;
            cb_SaveResponse.Checked = s.Config_SaveResponse;

            cb_ExportXlsx.Checked = s.Config_ExportEndpointsStatus_XLSX;
            cb_ExportJson.Checked = s.Config_ExportEndpointsStatus_JSON;
            cb_ExportXml.Checked = s.Config_ExportEndpointsStatus_XML;
            cb_ExportHtml.Checked = s.Config_ExportEndpointsStatus_HTML;
            txt_ExportDir.Text = s.Config_EndpointsStatusExportDirectory;

            txt_VncPath.Text = s.Config_Executable_VNCViewer;
            txt_PuttyPath.Text = s.Config_Executable_Putty;
            txt_VirusTotal.Text = s.VirusTotal_API_Key;
            txt_GoogleMaps.Text = s.GoogleMaps_API_Key;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            var s = Settings.Default;

            s.Config_EnableAutomaticRefresh = cb_AutoRefresh.Checked;
            s.Config_EnableContinuousRefresh = cb_ContinuousRefresh.Checked;
            s.Config_AutoAdjustRefreshInterval = cb_AutoAdjustInterval.Checked;
            s.Config_ScanOnStartup = cb_ScanOnStartup.Checked;
            s.Config_EnableTrayNotificationsOnError = cb_TrayNotify.Checked;
            s.Config_AutomaticRefreshIntervalSeconds = num_RefreshInterval.Value;

            s.Config_ValidationMethod = cmb_ValidationMethod.SelectedIndex;
            s.Config_TestPing = cb_TestPing.Checked;
            s.Config_AllowAutoRedirect = cb_AllowRedirect.Checked;
            s.Config_ValidateSSLCertificate = cb_ValidateSSL.Checked;
            s.Config_PingTimeoutSeconds = num_PingTimeout.Value;
            s.Config_HTTP_RequestTimeoutSeconds = num_HttpTimeout.Value;
            s.Config_FTP_RequestTimeoutSeconds = num_FtpTimeout.Value;
            s.Config_ParallelThreadsCount = num_ParallelThreads.Value;

            s.Config_Resolve_DNS_Names = cb_ResolveDns.Checked;
            s.Config_Resolve_IP_Addresses = cb_ResolveIp.Checked;
            s.Config_Resolve_MAC_Addresses = cb_ResolveMac.Checked;
            s.Config_ResolveNetworkShares = cb_ResolveShares.Checked;
            s.Config_ResolvePageMetaInfo = cb_PageMetaInfo.Checked;
            s.Config_RemoveURLParameters = cb_RemoveUrlParams.Checked;
            s.Config_ResolvePageLinks = cb_PageLinks.Checked;
            s.Config_SaveResponse = cb_SaveResponse.Checked;

            s.Config_ExportEndpointsStatus_XLSX = cb_ExportXlsx.Checked;
            s.Config_ExportEndpointsStatus_JSON = cb_ExportJson.Checked;
            s.Config_ExportEndpointsStatus_XML = cb_ExportXml.Checked;
            s.Config_ExportEndpointsStatus_HTML = cb_ExportHtml.Checked;
            s.Config_EndpointsStatusExportDirectory = txt_ExportDir.Text.Trim();

            s.Config_Executable_VNCViewer = txt_VncPath.Text.Trim();
            s.Config_Executable_Putty = txt_PuttyPath.Text.Trim();
            s.VirusTotal_API_Key = txt_VirusTotal.Text.Trim();
            s.GoogleMaps_API_Key = txt_GoogleMaps.Text.Trim();

            s.HasSavedConfiguration = true;
            s.Save();

            // Sync the Program-level static fields that LoadConfiguration reads from
            Program.app_ScanOnStartup = cb_ScanOnStartup.Checked;
            Program.apiKey_VirusTotal = txt_VirusTotal.Text.Trim();
            Program.apiKey_GoogleMaps = txt_GoogleMaps.Text.Trim();
            CheckerMainForm.appExecutable_VNC = txt_VncPath.Text.Trim();
            CheckerMainForm.appExecutable_Putty = txt_PuttyPath.Text.Trim();
            if (Directory.Exists(txt_ExportDir.Text.Trim()))
                Program.statusExport_Directory = txt_ExportDir.Text.Trim();

            _owner.LoadConfiguration();
        }

        // ── Helpers ────────────────────────────────────────────────────────────

        private static CheckBox AddCheck(Control parent, string text, ref int y)
        {
            var cb = new CheckBox { Text = text, Location = new Point(14, y), AutoSize = true };
            parent.Controls.Add(cb);
            y += 24;
            return cb;
        }

        private static void AddLabel(Control parent, string text, int x, int y)
        {
            parent.Controls.Add(new Label { Text = text, Location = new Point(x, y + 2), AutoSize = true });
        }

        private static void BrowseExe(TextBox target)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
                ofd.Title = "Select executable";
                if (ofd.ShowDialog() == DialogResult.OK)
                    target.Text = ofd.FileName;
            }
        }

        private static decimal Clamp(decimal value, decimal min, decimal max)
            => value < min ? min : value > max ? max : value;
    }
}
