using Newtonsoft.Json;
using NSpeedTest;
using NSpeedTest.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace EndpointChecker
{
    public partial class SpeedTestDialog : Form
    {
        // MAXIMUM NUMBER OF TESTS SERVERS
        public static int maxTestServersCount = 50;

        public static string clientIP = Program.status_NotAvailable;
        public static string clientISP = Program.status_NotAvailable;

        public static int testTakesCount = 10;
        public static int testRetryCount = 3;

        // SPEED TEST SERVER INSTANCE
        public static SpeedTestClient speedTestClient = new SpeedTestClient();
        public static Settings speedTestSettings = new Settings();

        public static List<Server> testServersList = new List<Server>();
        public enum TestServerSelectionMode
        {
            AllServers = 0,
            AllServersExceptCurrentCountry = 1,
            OnlyServersFromCurrentCountry = 2
        }

        public TestServerSelectionMode testServerSelectionMode = TestServerSelectionMode.AllServers;

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        public SpeedTestDialog()
        {
            InitializeComponent();

            // COMMON EXCEPTION HANDLERS
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CheckerMainForm.UnhandledExceptionHandler);
            Application.ThreadException += new ThreadExceptionEventHandler(CheckerMainForm.ThreadExceptionHandler);

            // SET DOUBLE BUFFER
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            SetControlsCleanState();

            lbl_SpeedTest_ExternalIP_Value.Text = Program.status_NotAvailable;

            NewBackgroundThread((Action)(() =>
            {
                try
                {
                    // SPEED TEST CLIENT SETTINGS
                    speedTestSettings = speedTestClient.GetSettings();

                    ThreadSafeInvoke((Action)(() =>
                    {
                        // GET CLIENT IP ADDRESS
                        clientIP = speedTestSettings.Client.Ip;

                        // GET CLIENT ISP
                        clientISP = speedTestSettings.Client.Isp;

                        // SET LABEL
                        lbl_SpeedTest_ExternalIP_Value.BackColor = Color.PaleGreen;
                        lbl_SpeedTest_ExternalIP_Value.Text = clientIP + " (" + clientISP + ")";

                        // GET SERVERS
                        btn_SpeedTest_GetServers_Click(this, null);
                    }));
                }
                catch (Exception exception)
                {
                    SetProgessState(false);

                    ThreadSafeInvoke((Action)(() =>
                    {
                        btn_SpeedTest_GetServers.Enabled = false;
                    }));

                    SpeedTest_AppendTextToLogBox(
                            rtb_SpeedTest_LogConsole,
                                "ERROR: " +
                                exception.Message +
                                Environment.NewLine,
                            Color.Red,
                            true);
                }
            }));
        }

        public Server GetSelectedServer()
        {
            SetAGaugeControlsCleanState();

            Server selectedServer;

            if (cb_SpeedTest_TestServer.SelectedIndex == 0)
            {
                // FIND BEST SERVER (BY LATENCY)
                selectedServer = BestServerByLatency();
            }
            else
            {
                // SPECIFIC SERVER
                selectedServer = testServersList[cb_SpeedTest_TestServer.SelectedIndex - 1];
            }

            return selectedServer;
        }

        public void SetServerDetails(Server targetServer)
        {
            lbl_SpeedTest_HostedBy_Value.Text = targetServer.Sponsor;
            lbl_SpeedTest_Distance_Value.Text = (int)targetServer.Distance / 1000 + " km";
            lbl_SpeedTest_Latency_Value.Text = targetServer.Latency + " ms";

            lbl_SpeedTest_HostedBy_Value.BackColor = Color.LightSkyBlue;
            lbl_SpeedTest_Distance_Value.BackColor = Color.LightSkyBlue;

            if (targetServer.Latency <= 30)
            {
                lbl_SpeedTest_Latency_Value.BackColor = Color.Green;
            }
            else if (targetServer.Latency <= 60)
            {
                lbl_SpeedTest_Latency_Value.BackColor = Color.Orange;
            }
            else
            {
                lbl_SpeedTest_Latency_Value.BackColor = Color.Red;
            }
        }

        public Server BestServerByLatency()
        {
            SpeedTest_AppendTextToLogBox(
                                         rtb_SpeedTest_LogConsole,
                                         Environment.NewLine +
                                         "Selecting best server by latency ...",
                                         Color.Black,
                                         true);

            Server bestServer = testServersList.OrderBy(x => x.Latency).First();

            SpeedTest_AppendTextToLogBox(
                                    rtb_SpeedTest_LogConsole,
                                        "Hosted by '" + bestServer.Sponsor +
                                        "' (" + bestServer.Name + "/" +
                                        bestServer.Country +
                                        "), distance: " + (int)bestServer.Distance / 1000 +
                                        "km, latency: " + bestServer.Latency + "ms" +
                                        Environment.NewLine,
                                    Color.White,
                                    false);

            return bestServer;
        }

        public void SpeedTestToServer(Server targetServer)
        {
            NewBackgroundThread((Action)(() =>
            {
                try
                {
                    SpeedTest_AppendTextToLogBox(
                                        rtb_SpeedTest_LogConsole,
                                             Environment.NewLine +
                                             "Testing download speed by " +
                                             GetStringCorrectEncoding(targetServer.Sponsor) +
                                             " (" +
                                             GetStringCorrectEncoding(targetServer.Name) +
                                             "/" +
                                             GetStringCorrectEncoding(targetServer.Country) +
                                             ")",
                                        Color.LimeGreen,
                                        true);

                    // TEST DOWNLOAD SPEED
                    int downloadSpeed = GetDownloadSpeed(targetServer);

                    SpeedTest_AppendTextToLogBox(
                            rtb_SpeedTest_LogConsole,
                                downloadSpeed + " Mbps (" + testTakesCount + " takes)",
                            Color.Black,
                            false);

                    // SET CONTROLS FOR DOWNLOAD
                    ThreadSafeInvoke((Action)(() =>
                    {
                        while (aGauge_DownloadSpeed.MaxValue <= (float)downloadSpeed)
                        {
                            aGauge_DownloadSpeed.MaxValue += (float)50;
                        }

                        if (aGauge_DownloadSpeed.MaxValue > 150)
                        {
                            aGauge_DownloadSpeed.ScaleLinesMajorStepValue = 25;
                        }
                        else if (aGauge_DownloadSpeed.MaxValue > 300)
                        {
                            aGauge_DownloadSpeed.ScaleLinesMajorStepValue = 75;
                        }
                        else if (aGauge_DownloadSpeed.MaxValue > 500)
                        {
                            aGauge_DownloadSpeed.ScaleLinesMajorStepValue = 125;
                        }
                        else if (aGauge_DownloadSpeed.MaxValue > 750)
                        {
                            aGauge_DownloadSpeed.ScaleLinesMajorStepValue = 150;
                        }
                        else if (aGauge_DownloadSpeed.MaxValue > 1000)
                        {
                            aGauge_DownloadSpeed.ScaleLinesMajorStepValue = 1175;
                        }

                        aGauge_DownloadSpeed.Value = (float)downloadSpeed;
                        lbl_SpeedTest_Mbps_Download_Label.BackColor = Color.PaleGreen;
                        lbl_SpeedTest_Download_Label.ForeColor = Color.Green;
                        aGauge_DownloadSpeed.NeedleColor1 = AGaugeNeedleColor.Green;
                        lbl_SpeedTest_Mbps_Download_Label.Text = downloadSpeed.ToString() + " Mbps";
                    }));

                    SpeedTest_AppendTextToLogBox(
                            rtb_SpeedTest_LogConsole,
                               Environment.NewLine +
                               "Testing upload speed by " +
                               GetStringCorrectEncoding(targetServer.Sponsor) +
                               " (" +
                               GetStringCorrectEncoding(targetServer.Name) +
                               "/" +
                               GetStringCorrectEncoding(targetServer.Country) +
                               ")",
                            Color.Red,
                            true);

                    // TEST UPLOAD SPEED
                    int uploadSpeed = GetUploadSpeed(targetServer);

                    SpeedTest_AppendTextToLogBox(
                                                 rtb_SpeedTest_LogConsole,
                                                 uploadSpeed + " Mbps (" + testTakesCount + " takes)",
                                                 Color.Black,
                                                 false);

                    // SET CONTROLS FOR UPLOAD
                    ThreadSafeInvoke((Action)(() =>
                    {
                        while (aGauge_UploadSpeed.MaxValue <= (float)uploadSpeed)
                        {
                            aGauge_UploadSpeed.MaxValue += (float)50;
                        }

                        if (aGauge_UploadSpeed.MaxValue > 150)
                        {
                            aGauge_UploadSpeed.ScaleLinesMajorStepValue = 25;
                        }
                        else if (aGauge_UploadSpeed.MaxValue > 300)
                        {
                            aGauge_UploadSpeed.ScaleLinesMajorStepValue = 50;
                        }
                        else if (aGauge_UploadSpeed.MaxValue > 500)
                        {
                            aGauge_UploadSpeed.ScaleLinesMajorStepValue = 75;
                        }
                        else if (aGauge_UploadSpeed.MaxValue > 750)
                        {
                            aGauge_UploadSpeed.ScaleLinesMajorStepValue = 100;
                        }
                        else if (aGauge_UploadSpeed.MaxValue > 1000)
                        {
                            aGauge_UploadSpeed.ScaleLinesMajorStepValue = 150;
                        }

                        aGauge_UploadSpeed.Value = (float)uploadSpeed;
                        lbl_SpeedTest_Mbps_Upload_Label.BackColor = Color.LightSalmon;
                        lbl_SpeedTest_Upload_Label.ForeColor = Color.Red;
                        aGauge_UploadSpeed.NeedleColor1 = AGaugeNeedleColor.Red;
                        lbl_SpeedTest_Mbps_Upload_Label.Text = uploadSpeed.ToString() + " Mbps";
                    }));

                    SpeedTest_AppendTextToLogBox(
                                                 rtb_SpeedTest_LogConsole,
                                                 Environment.NewLine,
                                                 Color.Black,
                                                 false);
                }
                catch (Exception exception)
                {
                    SpeedTest_AppendTextToLogBox(
                            rtb_SpeedTest_LogConsole,
                                Environment.NewLine +
                                "ERROR: " +
                                exception.Message +
                                Environment.NewLine,
                            Color.Red,
                            true);
                }

                ThreadSafeInvoke((Action)(() =>
                {
                    SetProgessState(false);
                }));
            }));
        }

        public int GetDownloadSpeed(Server targetServer)
        {
            int downloadSpeed = 0;

            for (int i = 1; i < testTakesCount; i++)
            {
                downloadSpeed += (int)Math.Round(speedTestClient.TestDownloadSpeed(targetServer, speedTestSettings.Download.ThreadsPerUrl) / 1024, testRetryCount);
            }

            return (downloadSpeed / testTakesCount);
        }

        public int GetUploadSpeed(Server targetServer)
        {
            int uploadSpeed = 0;

            for (int i = 1; i < testTakesCount; i++)
            {
                uploadSpeed += (int)Math.Round(speedTestClient.TestUploadSpeed(targetServer, speedTestSettings.Upload.ThreadsPerUrl) / 1024, testRetryCount);
            }

            return (uploadSpeed / testTakesCount);
        }

        public IEnumerable<Server> GetServers(
            Settings settings,
            RegionInfo regionInfo_CurrentCountry,
            SpeedTestClient client,
            int testServersCount)
        {
            List<Server> serversList = settings.Servers.ToList();
            List<Server> filteredServersList = new List<Server>();

            if (testServerSelectionMode == TestServerSelectionMode.AllServersExceptCurrentCountry)
            {
                foreach (Server serverItem in serversList)
                {
                    if (
                        !(serverItem.Country.ToLower() == regionInfo_CurrentCountry.DisplayName.ToLower()) &&
                        !(serverItem.Country.ToLower() == regionInfo_CurrentCountry.EnglishName.ToLower()) &&
                        !(serverItem.Country.ToLower() == regionInfo_CurrentCountry.Name.ToLower()) &&
                        !(serverItem.Country.ToLower() == regionInfo_CurrentCountry.NativeName.ToLower()) &&
                        !(serverItem.Country.ToLower() == regionInfo_CurrentCountry.TwoLetterISORegionName.ToLower()) &&
                        !(serverItem.Country.ToLower() == regionInfo_CurrentCountry.ThreeLetterISORegionName.ToLower()) &&
                        !(serverItem.Country.ToLower() == regionInfo_CurrentCountry.ThreeLetterWindowsRegionName.ToLower()))
                    {
                        if (filteredServersList.Count <= maxTestServersCount)
                        {
                            filteredServersList.Add(serverItem);
                        }
                    }
                }
            }
            else if (testServerSelectionMode == TestServerSelectionMode.OnlyServersFromCurrentCountry)
            {
                foreach (Server serverItem in serversList)
                {
                    if (
                        (serverItem.Country.ToLower() == regionInfo_CurrentCountry.DisplayName.ToLower()) ||
                        (serverItem.Country.ToLower() == regionInfo_CurrentCountry.EnglishName.ToLower()) ||
                        (serverItem.Country.ToLower() == regionInfo_CurrentCountry.Name.ToLower()) ||
                        (serverItem.Country.ToLower() == regionInfo_CurrentCountry.NativeName.ToLower()) ||
                        (serverItem.Country.ToLower() == regionInfo_CurrentCountry.TwoLetterISORegionName.ToLower()) ||
                        (serverItem.Country.ToLower() == regionInfo_CurrentCountry.ThreeLetterISORegionName.ToLower()) ||
                        (serverItem.Country.ToLower() == regionInfo_CurrentCountry.ThreeLetterWindowsRegionName.ToLower()))
                    {
                        if (filteredServersList.Count <= maxTestServersCount)
                        {
                            filteredServersList.Add(serverItem);
                        }
                    }
                }
            }
            else if (testServerSelectionMode == TestServerSelectionMode.AllServers)
            {
                filteredServersList = serversList.Take(maxTestServersCount).ToList();
            }

            foreach (var server in filteredServersList.Take(testServersCount))
            {
                server.Latency = client.TestServerLatency(server);

                SpeedTest_AppendTextToLogBox(
                            rtb_SpeedTest_LogConsole,
                                "Hosted by '" + GetStringCorrectEncoding(server.Sponsor) +
                                "' (" + GetStringCorrectEncoding(server.Name) + "/" +
                                GetStringCorrectEncoding(server.Country) +
                                "), distance: " + (int)server.Distance / 1000 +
                                "km, latency: " + server.Latency + "ms",
                            Color.White,
                            false);
            }

            return filteredServersList;
        }

        public void SpeedTest_AppendTextToLogBox(RichTextBox logBox, string resultLine, Color textColor, bool boldText)
        {
            // INVOKE DELEGATE ->> 
            ThreadSafeInvoke((Action)(() =>
            {
                // SELECT
                logBox.SelectionStart = logBox.TextLength;
                logBox.SelectionLength = 0;

                // APPEND TEXT
                logBox.SelectionColor = textColor;

                if (boldText)
                {
                    logBox.SelectionFont = new Font(logBox.Font, FontStyle.Bold);
                }
                else
                {
                    logBox.SelectionFont = new Font(logBox.Font, FontStyle.Regular);
                }

                logBox.AppendText(resultLine);

                logBox.AppendText(Environment.NewLine);

                logBox.SelectionColor = logBox.ForeColor;

                // SCROLL TO END
                logBox.SelectionStart = logBox.Text.Length;
                logBox.ScrollToCaret();
            }));
        }

        public string GetStringCorrectEncoding(string content)
        {
            byte[] encodedBytes = Encoding.Default.GetBytes(content);
            return Encoding.UTF8.GetString(encodedBytes);
        }

        public RegionInfo SpeedTest_GetUserCountry(string ipAddress)
        {
            RegionInfo regionInfo = null;

            try
            {
                SpeedTest_AppendTextToLogBox(
                            rtb_SpeedTest_LogConsole,
                                "Getting current country name [by 'http://ipinfo.io'] ...",
                            Color.Black,
                            true);

                // GET IP INFO / COUNTRY
                string info = new WebClient().DownloadString("http://ipinfo.io/" + ipAddress);
                IpInfo ipInfo = JsonConvert.DeserializeObject<IpInfo>(info);
                regionInfo = new RegionInfo(ipInfo.Country);

                ThreadSafeInvoke((Action)(() =>
                {
                    lbl_SpeedTest_CurrentCountry_Value.BackColor = Color.BlanchedAlmond;
                    lbl_SpeedTest_CurrentCountry_Value.Text = regionInfo.DisplayName;
                }));

                SpeedTest_AppendTextToLogBox(
                            rtb_SpeedTest_LogConsole,
                                "Current country: " +
                                regionInfo.DisplayName +
                                Environment.NewLine,
                            Color.Yellow,
                            true);
            }
            catch (Exception exception)
            {
                SpeedTest_AppendTextToLogBox(
                            rtb_SpeedTest_LogConsole,
                                Environment.NewLine +
                                "ERROR: " +
                                exception.Message +
                                Environment.NewLine,
                            Color.Red,
                            true);

                ThreadSafeInvoke((Action)(() =>
                {
                    SetProgessState(false);
                }));
            }

            return regionInfo;
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

        public void SpeedTestDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = pb_SpeedTestProgress.Visible;
        }

        public void rb_AllServers_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_AllServers.Checked)
            {
                rb_AllServersExceptCurrCountry.Checked = false;
                rb_CurrentCountryServersOnly.Checked = false;

                testServerSelectionMode = TestServerSelectionMode.AllServers;
            }
        }

        public void rb_AllServersExceptCurrCountry_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_AllServersExceptCurrCountry.Checked)
            {
                rb_AllServers.Checked = false;
                rb_CurrentCountryServersOnly.Checked = false;

                testServerSelectionMode = TestServerSelectionMode.AllServersExceptCurrentCountry;
            }
        }

        public void rb_CurrentCountryServersOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_CurrentCountryServersOnly.Checked)
            {
                rb_AllServers.Checked = false;
                rb_AllServersExceptCurrCountry.Checked = false;

                testServerSelectionMode = TestServerSelectionMode.OnlyServersFromCurrentCountry;
            }
        }

        public void comboBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            // By using Sender, one method could handle multiple ComboBoxes
            ComboBox cbx = sender as ComboBox;
            if (cbx != null)
            {
                // Always draw the background
                e.DrawBackground();

                // Drawing one of the items?
                if (e.Index >= 0)
                {
                    // Set the string alignment.  Choices are Center, Near and Far
                    StringFormat sf = new StringFormat();
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Center;

                    // Set the Brush to ComboBox ForeColor to maintain any ComboBox color settings
                    // Assumes Brush is solid
                    Brush brush = new SolidBrush(cbx.ForeColor);

                    // If drawing highlighted selection, change brush
                    if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                        brush = SystemBrushes.HighlightText;

                    // Draw the string
                    e.Graphics.DrawString(cbx.Items[e.Index].ToString(), lbl_SpeedTest_ExternalIP.Font, brush, e.Bounds, sf);
                }
            }
        }

        public void btn_SpeedTest_GetServers_Click(object sender, EventArgs e)
        {
            SetControlsCleanState();

            SetAGaugeControlsCleanState();

            SetProgessState(true);

            NewBackgroundThread((Action)(() =>
            {
                RegionInfo currentCountry_RegionInfo = SpeedTest_GetUserCountry(clientIP);

                if (currentCountry_RegionInfo != null)
                {
                    try
                    {
                        SpeedTest_AppendTextToLogBox(
                                rtb_SpeedTest_LogConsole,
                                    "Getting test servers list [from 'https://www.speedtest.net'] ...",
                                Color.Black,
                                true);

                        SpeedTestClient speedTestClient = new SpeedTestClient();
                        Settings speedTestSettings = speedTestClient.GetSettings();

                        IEnumerable<Server> servers = GetServers(
                                                                 speedTestSettings,
                                                                 currentCountry_RegionInfo,
                                                                 speedTestClient,
                                                                 maxTestServersCount);

                        foreach (Server testServer in servers)
                        {
                            testServersList.Add(testServer);

                            ThreadSafeInvoke((Action)(() =>
                            {
                                cb_SpeedTest_TestServer.Items.Add(GetStringCorrectEncoding(testServer.Sponsor) +
                                                                  " (" + GetStringCorrectEncoding(testServer.Name) + "/" +
                                                                  GetStringCorrectEncoding(testServer.Country) + ")");
                            }));
                        }

                        if (servers.Count() > 0)
                        {
                            ThreadSafeInvoke((Action)(() =>
                            {
                                cb_SpeedTest_TestServer.SelectedIndex = 0;

                                pb_GO.Visible = true;
                            }));
                        }
                        else
                        {
                            SpeedTest_AppendTextToLogBox(
                                                         rtb_SpeedTest_LogConsole,
                                                             "Not any test Server available" +
                                                             Environment.NewLine,
                                                         Color.Red,
                                                         true);
                        }
                    }
                    catch (Exception exception)
                    {
                        SetProgessState(false);

                        SpeedTest_AppendTextToLogBox(
                                rtb_SpeedTest_LogConsole,
                                    "ERROR: " +
                                    exception.Message +
                                    Environment.NewLine,
                                Color.Red,
                                true);
                    }
                }

                ThreadSafeInvoke((Action)(() =>
                {
                    SetProgessState(false);
                }));
            }));
        }

        public void SetControlsCleanState()
        {
            testServersList.Clear();

            rtb_SpeedTest_LogConsole.Text = string.Empty;

            lbl_SpeedTest_CurrentCountry_Value.Text = Program.status_NotAvailable;
            lbl_SpeedTest_HostedBy_Value.Text = Program.status_NotAvailable;
            lbl_SpeedTest_Distance_Value.Text = Program.status_NotAvailable;
            lbl_SpeedTest_Latency_Value.Text = Program.status_NotAvailable;

            cb_SpeedTest_TestServer.Items.Clear();
            cb_SpeedTest_TestServer.Items.Add("Using Best Server (by latency)");

            lbl_SpeedTest_CurrentCountry_Value.BackColor = Color.DimGray;
            cb_SpeedTest_TestServer.BackColor = Color.DimGray;
            lbl_SpeedTest_HostedBy_Value.BackColor = Color.DimGray;
            lbl_SpeedTest_Distance_Value.BackColor = Color.DimGray;
            lbl_SpeedTest_Latency_Value.BackColor = Color.DimGray;
        }

        public void SetAGaugeControlsCleanState()
        {
            lbl_SpeedTest_Mbps_Download_Label.Text = Program.status_NotAvailable;
            lbl_SpeedTest_Mbps_Upload_Label.Text = Program.status_NotAvailable;

            lbl_SpeedTest_Mbps_Download_Label.BackColor = Color.Silver;
            lbl_SpeedTest_Mbps_Upload_Label.BackColor = Color.Silver;
            lbl_SpeedTest_Download_Label.ForeColor = Color.Silver;
            lbl_SpeedTest_Upload_Label.ForeColor = Color.Silver;

            aGauge_DownloadSpeed.NeedleColor1 = AGaugeNeedleColor.Gray;
            aGauge_UploadSpeed.NeedleColor1 = AGaugeNeedleColor.Gray;

            aGauge_DownloadSpeed.Value = 0;
            aGauge_UploadSpeed.Value = 0;

            aGauge_DownloadSpeed.MaxValue = 50;
            aGauge_UploadSpeed.MaxValue = 50;

            aGauge_DownloadSpeed.ScaleLinesMajorStepValue = 10;
            aGauge_UploadSpeed.ScaleLinesMajorStepValue = 10;
        }

        public void SetProgessState(bool inProgress)
        {
            ThreadSafeInvoke((Action)(() =>
            {
                rb_AllServers.Enabled = !inProgress;
                rb_AllServersExceptCurrCountry.Enabled = !inProgress;
                rb_CurrentCountryServersOnly.Enabled = !inProgress;
                cb_SpeedTest_TestServer.Enabled = !inProgress;
                btn_SpeedTest_GetServers.Visible = !inProgress;
                pb_SpeedTestProgress.Visible = inProgress;

                pb_GO.Visible =
                    !inProgress &&
                    clientIP != Program.status_NotAvailable &&
                    cb_SpeedTest_TestServer.Items.Count > 1;
            }));
        }

        public void cb_SpeedTest_TestServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (testServersList.Count() > 0)
            {
                SetAGaugeControlsCleanState();
                SetServerDetails(GetSelectedServer());
            }
        }

        public void pb_GO_Click(object sender, EventArgs e)
        {
            SetProgessState(true);
            SpeedTestToServer(GetSelectedServer());
        }
    }
}