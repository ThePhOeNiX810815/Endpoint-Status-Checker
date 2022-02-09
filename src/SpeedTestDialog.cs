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
        public int maxTestServersCount = 50;

        public enum TestServerSelectionMode
        {
            AllServers = 0,
            AllServersExceptCurrentCountry = 1,
            OnlyServersFromCurrentCountry = 2
        }

        public TestServerSelectionMode testServerSelectionMode = TestServerSelectionMode.AllServers;

        public SpeedTestDialog()
        {
            InitializeComponent();

            // SET DOUBLE BUFFER
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        public void Btn_SpeedTest_Refresh_Click(object sender, EventArgs e)
        {
            SpeedTest();
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        public void SpeedTest()
        {
            // COMMON EXCEPTION HANDLERS
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CheckerMainForm.UnhandledExceptionHandler);
            Application.ThreadException += new ThreadExceptionEventHandler(CheckerMainForm.ThreadExceptionHandler);

            rb_AllServers.Enabled = false;
            rb_AllServersExceptCurrCountry.Enabled = false;
            rb_CurrentCountryServersOnly.Enabled = false;

            btn_SpeedTest_Refresh.Enabled = false;
            pb_SpeedTestProgress.Visible = true;

            rtb_SpeedTest_LogConsole.Text = string.Empty;
            rtb_SpeedTest_LogConsole.Visible = true;

            tb_SpeedTest_ExternalIP.Text = Program.status_NotAvailable;
            tb_SpeedTest_CurrentCountry.Text = Program.status_NotAvailable;
            tb_SpeedTest_TestServer.Text = Program.status_NotAvailable;
            tb_SpeedTest_HostedBy.Text = Program.status_NotAvailable;
            tb_SpeedTest_Distance.Text = Program.status_NotAvailable;
            tb_SpeedTest_Latency.Text = Program.status_NotAvailable;

            tb_SpeedTest_ExternalIP.BackColor = Color.Silver;
            tb_SpeedTest_CurrentCountry.BackColor = Color.Silver;
            tb_SpeedTest_TestServer.BackColor = Color.Silver;
            tb_SpeedTest_HostedBy.BackColor = Color.Silver;
            tb_SpeedTest_Distance.BackColor = Color.Silver;
            tb_SpeedTest_Latency.BackColor = Color.Silver;

            lbl_SpeedTest_Mbps_Download_Label.Visible = false;
            lbl_SpeedTest_Mbps_Upload_Label.Visible = false;
            lbl_SpeedTest_Download_Label.Visible = false;
            lbl_SpeedTest_Upload_Label.Visible = false;

            aGauge_DownloadSpeed.Visible = false;
            aGauge_UploadSpeed.Visible = false;

            aGauge_DownloadSpeed.MaxValue = 50;
            aGauge_UploadSpeed.MaxValue = 50;

            aGauge_DownloadSpeed.ScaleLinesMajorStepValue = 10;
            aGauge_UploadSpeed.ScaleLinesMajorStepValue = 10;

            NewBackgroundThread((Action)(() =>
            {
                // GET PUBLIC IP
                string ipAddress = SpeedTest_GetExternalPublicIPAddress();

                if (ipAddress != Program.status_NotAvailable)
                {
                    RegionInfo currentCountry_RegionInfo = SpeedTest_GetUserCountry(ipAddress);

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

                            IEnumerable<Server> servers = SelectServers(
                                                                        speedTestSettings,
                                                                        currentCountry_RegionInfo,
                                                                        speedTestClient,
                                                                        maxTestServersCount);

                            if (servers.Count() > 0)
                            {
                                SpeedTest_AppendTextToLogBox(
                                                             rtb_SpeedTest_LogConsole,
                                                                 Environment.NewLine +
                                                                 Environment.NewLine +
                                                                 "Selecting best server by latency ...",
                                                             Color.Green,
                                                             true);

                                Server bestServer = SelectBestServer(servers);

                                ThreadSafeInvoke((Action)(() =>
                                {
                                    tb_SpeedTest_TestServer.Text = bestServer.Host + " (" + bestServer.Country + ")";
                                    tb_SpeedTest_HostedBy.Text = bestServer.Sponsor;
                                    tb_SpeedTest_Distance.Text = (int)bestServer.Distance / 1000 + " km";
                                    tb_SpeedTest_Latency.Text = bestServer.Latency + " ms";

                                    tb_SpeedTest_TestServer.BackColor = Color.LightSkyBlue;
                                    tb_SpeedTest_HostedBy.BackColor = Color.LightSkyBlue;
                                    tb_SpeedTest_Distance.BackColor = Color.LightSkyBlue;

                                    if (bestServer.Latency <= 30)
                                    {
                                        tb_SpeedTest_Latency.BackColor = Color.Green;
                                    }
                                    else if (bestServer.Latency <= 60)
                                    {
                                        tb_SpeedTest_Latency.BackColor = Color.Orange;
                                    }
                                    else
                                    {
                                        tb_SpeedTest_Latency.BackColor = Color.Red;
                                    }
                                }));

                                SpeedTest_AppendTextToLogBox(
                                        rtb_SpeedTest_LogConsole,
                                            Environment.NewLine +
                                            Environment.NewLine +
                                            "Testing download speed ...",
                                        Color.Green,
                                        true);

                                double downloadSpeed = Math.Round(speedTestClient.TestDownloadSpeed(bestServer, speedTestSettings.Download.ThreadsPerUrl) / 1024);

                                SpeedTest_AppendTextToLogBox(
                                        rtb_SpeedTest_LogConsole,
                                            downloadSpeed + " Mbps",
                                        Color.Black,
                                        false);

                                SpeedTest_AppendTextToLogBox(
                                        rtb_SpeedTest_LogConsole,
                                            Environment.NewLine +
                                            Environment.NewLine +
                                            "Testing upload speed ..." +
                                            Environment.NewLine,
                                        Color.DarkOrange,
                                        true);

                                double uploadSpeed = Math.Round(speedTestClient.TestUploadSpeed(bestServer, speedTestSettings.Upload.ThreadsPerUrl) / 1024);

                                SpeedTest_AppendTextToLogBox(
                                                             rtb_SpeedTest_LogConsole,
                                                             uploadSpeed + " Mbps",
                                                             Color.Black,
                                                             false);

                                ThreadSafeInvoke((Action)(() =>
                                {
                                    rtb_SpeedTest_LogConsole.Visible = false;
                                    btn_SpeedTest_Refresh.Enabled = true;
                                    pb_SpeedTestProgress.Visible = false;

                                    lbl_SpeedTest_Mbps_Download_Label.Text = downloadSpeed.ToString() + " Mbps";
                                    lbl_SpeedTest_Mbps_Upload_Label.Text = uploadSpeed.ToString() + " Mbps";

                                    float downloadSpeed_FloatValue = (float)downloadSpeed;
                                    float uploadSpeed_FloatValue = (float)uploadSpeed;

                                    while (aGauge_DownloadSpeed.MaxValue <= downloadSpeed_FloatValue)
                                    {
                                        aGauge_DownloadSpeed.MaxValue += (float)50;
                                    }

                                    while (aGauge_UploadSpeed.MaxValue <= uploadSpeed_FloatValue)
                                    {
                                        aGauge_UploadSpeed.MaxValue += (float)50;
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

                                    aGauge_DownloadSpeed.Value = (float)downloadSpeed;
                                    aGauge_UploadSpeed.Value = (float)uploadSpeed;

                                    aGauge_DownloadSpeed.Visible = true;
                                    aGauge_UploadSpeed.Visible = true;

                                    lbl_SpeedTest_Mbps_Download_Label.Visible = true;
                                    lbl_SpeedTest_Mbps_Upload_Label.Visible = true;
                                    lbl_SpeedTest_Download_Label.Visible = true;
                                    lbl_SpeedTest_Upload_Label.Visible = true;
                                }));
                            }
                            else
                            {
                                SpeedTest_AppendTextToLogBox(
                                                             rtb_SpeedTest_LogConsole,
                                                                 Environment.NewLine +
                                                                 Environment.NewLine +
                                                                 "Not any test Server available",
                                                             Color.Red,
                                                             true);
                            }
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
                    }
                }

                ThreadSafeInvoke((Action)(() =>
                {
                    rb_AllServers.Enabled = true;
                    rb_AllServersExceptCurrCountry.Enabled = true;
                    rb_CurrentCountryServersOnly.Enabled = true;

                    btn_SpeedTest_Refresh.Enabled = true;
                    pb_SpeedTestProgress.Visible = false;
                }));
            }));
        }

        public Server SelectBestServer(IEnumerable<Server> servers)
        {
            var bestServer = servers.OrderBy(x => x.Latency).First();

            SpeedTest_AppendTextToLogBox(
                            rtb_SpeedTest_LogConsole,
                                "Hosted by '" + bestServer.Sponsor +
                                "' (" + bestServer.Name + "/" +
                                bestServer.Country +
                                "), distance: " + (int)bestServer.Distance / 1000 +
                                "km, latency: " + bestServer.Latency + "ms",
                            Color.SlateBlue,
                            false);

            return bestServer;
        }

        public IEnumerable<Server> SelectServers(
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
                        filteredServersList.Add(serverItem);
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
                        filteredServersList.Add(serverItem);
                    }
                }
            }
            else if (testServerSelectionMode == TestServerSelectionMode.AllServers)
            {
                filteredServersList = settings.Servers.ToList();
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
                            Color.SlateBlue,
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

                // IF NOT FIRST LINE, APPEND NEW LINE
                if (!string.IsNullOrEmpty(logBox.Text))
                {
                    logBox.AppendText("\r\n");
                }

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
                logBox.SelectionColor = logBox.ForeColor;

                // SCROLL TO END
                logBox.Select(logBox.Text.Length - 1, 0);
                logBox.ScrollToCaret();
            }));
        }

        public string GetStringCorrectEncoding(string content)
        {
            byte[] encodedBytes = Encoding.Default.GetBytes(content);
            return Encoding.UTF8.GetString(encodedBytes);
        }

        public string SpeedTest_GetExternalPublicIPAddress()
        {
            string ipAddress = Program.status_NotAvailable;

            try
            {
                SpeedTest_AppendTextToLogBox(
                            rtb_SpeedTest_LogConsole,
                                "Getting external IP address [by 'http://icanhazip.com'] ...",
                            Color.Black,
                            true);

                // GET IP ADDRESS
                ipAddress = new WebClient().DownloadString("http://icanhazip.com").TrimEnd('/').TrimEnd();

                ThreadSafeInvoke((Action)(() =>
                {
                    tb_SpeedTest_ExternalIP.BackColor = Color.LightBlue;
                    tb_SpeedTest_ExternalIP.Text = ipAddress;
                }));

                SpeedTest_AppendTextToLogBox(
                            rtb_SpeedTest_LogConsole,
                                "External IP address: " +
                                ipAddress +
                                Environment.NewLine,
                            Color.Green,
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
                    btn_SpeedTest_Refresh.Enabled = true;
                    pb_SpeedTestProgress.Visible = false;
                }));
            }

            return ipAddress;
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
                    tb_SpeedTest_CurrentCountry.BackColor = Color.LightBlue;
                    tb_SpeedTest_CurrentCountry.Text = regionInfo.DisplayName;
                }));

                SpeedTest_AppendTextToLogBox(
                            rtb_SpeedTest_LogConsole,
                                "Current country: " +
                                regionInfo.DisplayName +
                                Environment.NewLine,
                            Color.Green,
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
                    btn_SpeedTest_Refresh.Enabled = true;
                    pb_SpeedTestProgress.Visible = false;
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
            e.Cancel = !btn_SpeedTest_Refresh.Enabled;
        }

        public void SpeedTestDialog_Shown(object sender, EventArgs e)
        {
            Btn_SpeedTest_Refresh_Click(this, null);
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
    }
}