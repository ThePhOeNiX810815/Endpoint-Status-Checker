using Newtonsoft.Json;
using NSpeedTest;
using NSpeedTest.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static EndpointChecker.CheckerMainForm;
using static EndpointChecker.Program;

namespace EndpointChecker
{
    public partial class SpeedTestDialog : Form
    {
        // MAXIMUM NUMBER OF TESTS SERVERS [API HARD-LIMITED TO 10, BUT NO ONE KNOWS...]
        public static int maxTestServersCount = 50;

        public static string clientIP = status_NotAvailable;
        public static string clientISP = status_NotAvailable;

        public static int testTakesCount = 10;
        public static int testRetryCount = 5;

        // SPEED TEST SERVER / SETTINGS
        public static SpeedTestClient speedTestClient = new SpeedTestClient();
        public static Settings speedTestSettings = new Settings();

        // TARGET TEST SERVER
        public static Server targetServer = new Server();

        // IP INFO API RESPONSE
        public static IP_API_JSON_Response ipInfo;

        public static List<Server> testServersList = new List<Server>();
        public enum TestServerSelectionMode
        {
            [Description("All Servers")]
            AllServers = 0,
            [Description("All Servers except Current Country")]
            AllServersExceptCurrentCountry = 1,
            [Description("Only Servers from Current Country")]
            OnlyServersFromCurrentCountry = 2
        }

        public TestServerSelectionMode testServerSelectionMode = TestServerSelectionMode.AllServers;

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        public SpeedTestDialog()
        {
            InitializeComponent();

            // COMMON EXCEPTION HANDLERS
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);
            Application.ThreadException += new ThreadExceptionEventHandler(ThreadExceptionHandler);

            // SET DOUBLE BUFFER
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            // SET CONTROLS CLEAN STATE
            SetControlsCleanState();

            lbl_SpeedTest_ExternalIP_Value.Text = status_NotAvailable;

            NewBackgroundThread(() =>
            {
                try
                {
                    // GET SPEEDTEST SETTINGS
                    AppendTextToLogBox(
                                         rtb_SpeedTest_LogConsole,
                                         "Retreiving OOKLA's SpeedTest API Configuration ...",
                                         Color.Blue,
                                         true);

                    speedTestSettings = speedTestClient.GetSettings();

                    ThreadSafeInvoke(() =>
                    {
                        // GET CLIENT IP ADDRESS
                        clientIP = speedTestSettings.Client.Ip;

                        // GET CLIENT ISP
                        clientISP = speedTestSettings.Client.Isp;

                        // SET IP / ISP LABEL
                        lbl_SpeedTest_ExternalIP_Value.BackColor = Color.PaleGreen;
                        lbl_SpeedTest_ExternalIP_Value.Text = clientIP + " (" + clientISP + ")";

                        // RESTORE PREFERRED SERVER SCOPE (IF SAVED)
                        RestoreServerScopePreferredSetting();
                    });
                }
                catch (Exception exception)
                {
                    SetProgessState(false);

                    ThreadSafeInvoke(() =>
                    {
                        btn_SpeedTest_GetServers.Enabled = false;
                    });

                    AppendTextToLogBox(
                            rtb_SpeedTest_LogConsole,
                                "ERROR: " +
                                BuildExceptionMessage(exception) +
                                Environment.NewLine,
                            Color.Red,
                            true);
                }
            });
        }

        public void SaveServerScopePreferredSetting()
        {
            if (rb_AllServers.Checked == true)
            {
                Properties.Settings.Default.SpeedTest_ServerScope = 0;
            }
            else if (rb_AllServersExceptCurrCountry.Checked == true)
            {
                Properties.Settings.Default.SpeedTest_ServerScope = 1;
            }
            else if (rb_CurrentCountryServersOnly.Checked == true)
            {
                Properties.Settings.Default.SpeedTest_ServerScope = 2;
            }
        }

        public void RestoreServerScopePreferredSetting()
        {
            if (Properties.Settings.Default.SpeedTest_ServerScope == 0)
            {
                rb_AllServers.Checked = true;
            }
            else if (Properties.Settings.Default.SpeedTest_ServerScope == 1)
            {
                rb_AllServersExceptCurrCountry.Checked = true;
            }
            else if (Properties.Settings.Default.SpeedTest_ServerScope == 2)
            {
                rb_CurrentCountryServersOnly.Checked = true;
            }
        }

        public void SelectServer()
        {
            SetAGaugeControlsCleanState();

            if (cb_SpeedTest_TestServer.SelectedIndex == 0)
            {
                // FIND BEST SERVER (BY LATENCY)
                targetServer = GetBestServerByLatency();
            }
            else
            {
                // SPECIFIC SERVER
                targetServer = testServersList[cb_SpeedTest_TestServer.SelectedIndex - 1];
                ListSelectedServer(targetServer);
            }
        }

        public void SetServerDetails()
        {
            lbl_SpeedTest_Latency_Value.Text = targetServer.Latency + " ms";
            lbl_SpeedTest_HostedBy_Value.Text = GetStringCorrectEncoding(
                targetServer.Sponsor +
                " (" +
                targetServer.Country +
                "/" +
                targetServer.Name +
                ")");

            lbl_SpeedTest_Distance_Value.Text =
                    FormatLocationsDistanceString(
                        GetStringCorrectEncoding(ipInfo.Country_Name),
                        GetStringCorrectEncoding(targetServer.Country),
                        GetStringCorrectEncoding(ipInfo.City),
                        GetStringCorrectEncoding(targetServer.Name),
                        (int)targetServer.Distance / 1000);

            lbl_SpeedTest_HostedBy_Value.BackColor = Color.LightSkyBlue;
            lbl_SpeedTest_Distance_Value.BackColor = Color.LightSkyBlue;
            lbl_SpeedTest_Latency_Value.BackColor = GetColorByLatencyTime(targetServer.Latency);
        }

        public Server GetBestServerByLatency()
        {
            AppendTextToLogBox(
                                         rtb_SpeedTest_LogConsole,
                                         Environment.NewLine +
                                         "Selecting Best Server by Latency ...",
                                         Color.Black,
                                         true);

            Server bestServer = testServersList.OrderBy(x => x.Latency).First();

            ListSelectedServerDetails(bestServer);

            return bestServer;
        }

        public void ListSelectedServer(Server selectedServer)
        {
            AppendTextToLogBox(
                                         rtb_SpeedTest_LogConsole,
                                         Environment.NewLine +
                                         "User selected specific Server ...",
                                         Color.Black,
                                         true);

            ListSelectedServerDetails(selectedServer);
        }

        public void ListSelectedServerDetails(Server server)
        {
            AppendTextToLogBox(
                                    rtb_SpeedTest_LogConsole,
                                        "Hosting: " +
                                        GetStringCorrectEncoding(server.Sponsor) +
                                        Environment.NewLine +
                                        "Distance: " +
                                        FormatLocationsDistanceString(
                                            GetStringCorrectEncoding(ipInfo.Country_Name),
                                            GetStringCorrectEncoding(server.Country),
                                            GetStringCorrectEncoding(ipInfo.City),
                                            GetStringCorrectEncoding(server.Name),
                                            (int)server.Distance / 1000),
                                    Color.White,
                                    true);

            AppendTextToLogBox(
                                rtb_SpeedTest_LogConsole,
                                    "Latency: " +
                                    server.Latency +
                                    "ms" +
                                    Environment.NewLine,
                                GetColorByLatencyTime(server.Latency),
                                true);
        }

        public void SpeedTestToServer()
        {
            NewBackgroundThread(() =>
            {
                try
                {
                    AppendTextToLogBox(
                            rtb_SpeedTest_LogConsole,
                               Environment.NewLine +
                               "Testing Server Latency Time: " +
                               GetStringCorrectEncoding(targetServer.Sponsor) +
                               " (" +
                               GetStringCorrectEncoding(targetServer.Name) +
                               "/" +
                               GetStringCorrectEncoding(targetServer.Country) +
                               ")",
                            Color.LightSkyBlue,
                            true);

                    int latencyTime = TestServerLatency();

                    AppendTextToLogBox(
                                       rtb_SpeedTest_LogConsole,
                                           "Average Server Latency (" +
                                           +testTakesCount + " takes): " +
                                           latencyTime + " ms",
                                       Color.Black,
                                       true);

                    // SET CONTROLS FOR LATENCY
                    ThreadSafeInvoke(() =>
                    {
                        lbl_SpeedTest_Latency_Value.Text = latencyTime + " ms";

                        Application.DoEvents();
                    });

                    AppendTextToLogBox(
                                        rtb_SpeedTest_LogConsole,
                                             Environment.NewLine +
                                             "Testing Download Speed by " +
                                             GetStringCorrectEncoding(targetServer.Sponsor) +
                                             " (" +
                                             GetStringCorrectEncoding(targetServer.Name) +
                                             "/" +
                                             GetStringCorrectEncoding(targetServer.Country) +
                                             ")",
                                        Color.LightGreen,
                                        true);

                    // TEST DOWNLOAD SPEED
                    int downloadSpeed = TestServerDownloadSpeed();

                    AppendTextToLogBox(
                            rtb_SpeedTest_LogConsole,
                                "Average Download Speed (" +
                                +testTakesCount + " takes): " +
                                downloadSpeed + " Mbps",
                            Color.Black,
                            true);

                    // SET CONTROLS FOR DOWNLOAD
                    ThreadSafeInvoke(() =>
                    {
                        while (aGauge_DownloadSpeed.MaxValue <= downloadSpeed)
                        {
                            aGauge_DownloadSpeed.MaxValue += 50;
                        }

                        if (aGauge_DownloadSpeed.MaxValue > 150)
                        {
                            aGauge_DownloadSpeed.ScaleLinesMajorStepValue = 50;
                        }
                        else if (aGauge_DownloadSpeed.MaxValue > 300)
                        {
                            aGauge_DownloadSpeed.ScaleLinesMajorStepValue = 100;
                        }
                        else if (aGauge_DownloadSpeed.MaxValue > 500)
                        {
                            aGauge_DownloadSpeed.ScaleLinesMajorStepValue = 150;
                        }
                        else if (aGauge_DownloadSpeed.MaxValue > 750)
                        {
                            aGauge_DownloadSpeed.ScaleLinesMajorStepValue = 200;
                        }
                        else if (aGauge_DownloadSpeed.MaxValue > 1000)
                        {
                            aGauge_DownloadSpeed.ScaleLinesMajorStepValue = 250;
                        }

                        pBar_Download.Visible = false;
                        pBar_Download.Value = 0;
                        aGauge_DownloadSpeed.Value = downloadSpeed;
                        lbl_SpeedTest_Mbps_Download_Label.BackColor = Color.PaleGreen;
                        lbl_SpeedTest_Download_Label.ForeColor = Color.Green;
                        aGauge_DownloadSpeed.NeedleColor1 = AGaugeNeedleColor.Green;
                        lbl_SpeedTest_Mbps_Download_Label.Text = downloadSpeed.ToString() + " Mbps";

                        Application.DoEvents();
                    });

                    AppendTextToLogBox(
                            rtb_SpeedTest_LogConsole,
                               Environment.NewLine +
                               "Testing Upload Speed by " +
                               GetStringCorrectEncoding(targetServer.Sponsor) +
                               " (" +
                               GetStringCorrectEncoding(targetServer.Name) +
                               "/" +
                               GetStringCorrectEncoding(targetServer.Country) +
                               ")",
                            Color.LightPink,
                            true);

                    // TEST UPLOAD SPEED
                    int uploadSpeed = TestServerUploadSpeed();

                    AppendTextToLogBox(
                           rtb_SpeedTest_LogConsole,
                               "Average Upload Speed (" +
                               +testTakesCount + " takes): " +
                               uploadSpeed + " Mbps",
                           Color.Black,
                           true);

                    // SET CONTROLS FOR UPLOAD
                    ThreadSafeInvoke(() =>
                    {
                        while (aGauge_UploadSpeed.MaxValue <= uploadSpeed)
                        {
                            aGauge_UploadSpeed.MaxValue += 50;
                        }

                        if (aGauge_UploadSpeed.MaxValue > 150)
                        {
                            aGauge_UploadSpeed.ScaleLinesMajorStepValue = 50;
                        }
                        else if (aGauge_UploadSpeed.MaxValue > 300)
                        {
                            aGauge_UploadSpeed.ScaleLinesMajorStepValue = 100;
                        }
                        else if (aGauge_UploadSpeed.MaxValue > 500)
                        {
                            aGauge_UploadSpeed.ScaleLinesMajorStepValue = 150;
                        }
                        else if (aGauge_UploadSpeed.MaxValue > 750)
                        {
                            aGauge_UploadSpeed.ScaleLinesMajorStepValue = 200;
                        }
                        else if (aGauge_UploadSpeed.MaxValue > 1000)
                        {
                            aGauge_UploadSpeed.ScaleLinesMajorStepValue = 250;
                        }

                        pBar_Upload.Visible = false;
                        pBar_Upload.Value = 0;
                        aGauge_UploadSpeed.Value = uploadSpeed;
                        lbl_SpeedTest_Mbps_Upload_Label.BackColor = Color.LightSalmon;
                        lbl_SpeedTest_Upload_Label.ForeColor = Color.Red;
                        aGauge_UploadSpeed.NeedleColor1 = AGaugeNeedleColor.Red;
                        lbl_SpeedTest_Mbps_Upload_Label.Text = uploadSpeed.ToString() + " Mbps";

                        Application.DoEvents();
                    });

                    AppendTextToLogBox(
                                                 rtb_SpeedTest_LogConsole,
                                                 Environment.NewLine,
                                                 Color.Black,
                                                 false);
                }
                catch (Exception exception)
                {
                    AppendTextToLogBox(
                            rtb_SpeedTest_LogConsole,
                                Environment.NewLine +
                                "ERROR: " +
                                BuildExceptionMessage(exception) +
                                Environment.NewLine,
                            Color.Red,
                            true);

                    ThreadSafeInvoke(() =>
                    {
                        SetAGaugeControlsCleanState();
                    });
                }
                finally
                {
                    ThreadSafeInvoke(() =>
                    {
                        SetProgessState(false);
                    });
                }
            });
        }

        public int TestServerLatency()
        {
            int currentRetryCount = 0;
            int latencyTime = 0;

            for (int i = 1; i <= testTakesCount; i++)
            {
                try
                {
                    int currentlatencyTime = speedTestClient.TestServerLatency(targetServer);
                    latencyTime += currentlatencyTime;
                    currentRetryCount = 0;

                    AppendTextToLogBox(
                           rtb_SpeedTest_LogConsole,
                               "Take " +
                               +i + " -> Latency (Ping Response Time): " +
                               currentlatencyTime + " ms",
                           Color.DarkGray,
                           false);

                    Application.DoEvents();
                }
                catch (Exception eX)
                {
                    if (currentRetryCount <= testRetryCount)
                    {
                        AppendTextToLogBox(
                          rtb_SpeedTest_LogConsole,
                              "ERROR: " +
                              BuildExceptionMessage(eX),
                          Color.Red,
                          false);

                        currentRetryCount++;
                        i--;
                    }
                    else
                    {
                        throw eX;
                    }
                }
            }

            return latencyTime / testTakesCount;
        }

        public int TestServerDownloadSpeed()
        {
            int currentRetryCount = 0;
            int downloadSpeed = 0;
            int progressStepValue = pBar_Download.Maximum / testTakesCount;

            ThreadSafeInvoke(() =>
            {
                pBar_Download.Visible = true;
            });

            for (int i = 1; i <= testTakesCount; i++)
            {
                try
                {
                    int currentDownloadSpeed = (int)Math.Round(speedTestClient.TestDownloadSpeed(targetServer, speedTestSettings.Download.ThreadsPerUrl) / 1024, 2);
                    downloadSpeed += currentDownloadSpeed;
                    currentRetryCount = 0;

                    ThreadSafeInvoke(() =>
                    {
                        pBar_Download.Value += progressStepValue;
                        Application.DoEvents();
                    });

                    AppendTextToLogBox(
                           rtb_SpeedTest_LogConsole,
                               "Take " +
                               +i + " -> Speed: " +
                               currentDownloadSpeed + " Mbps",
                           Color.DarkGray,
                           false);

                    Application.DoEvents();
                }
                catch (Exception eX)
                {
                    if (currentRetryCount <= testRetryCount)
                    {
                        AppendTextToLogBox(
                          rtb_SpeedTest_LogConsole,
                              "ERROR: " +
                              BuildExceptionMessage(eX),
                          Color.Red,
                          false);

                        currentRetryCount++;
                        i--;
                    }
                    else
                    {
                        throw eX;
                    }
                }
            }

            return downloadSpeed / testTakesCount;
        }

        public int TestServerUploadSpeed()
        {
            int currentRetryCount = 0;
            int uploadSpeed = 0;
            int progressStepValue = pBar_Upload.Maximum / testTakesCount;

            ThreadSafeInvoke(() =>
            {
                pBar_Upload.Visible = true;
            });

            for (int i = 1; i <= testTakesCount; i++)
            {
                try
                {
                    int currentUploadSpeed = (int)Math.Round(speedTestClient.TestUploadSpeed(targetServer, speedTestSettings.Upload.ThreadsPerUrl) / 1024, 2);
                    uploadSpeed += currentUploadSpeed;
                    currentRetryCount = 0;

                    ThreadSafeInvoke(() =>
                    {
                        pBar_Upload.Value += progressStepValue;
                        Application.DoEvents();
                    });

                    AppendTextToLogBox(
                           rtb_SpeedTest_LogConsole,
                               "Take " +
                               +i + " -> Speed: " +
                               currentUploadSpeed + " Mbps",
                           Color.DarkGray,
                           false);

                    Application.DoEvents();
                }
                catch (Exception eX)
                {
                    if (currentRetryCount <= testRetryCount)
                    {
                        AppendTextToLogBox(
                          rtb_SpeedTest_LogConsole,
                              "ERROR:" +
                              BuildExceptionMessage(eX),
                          Color.Red,
                          false);

                        currentRetryCount++;
                        i--;
                    }
                    else
                    {
                        throw eX;
                    }
                }
            }

            return uploadSpeed / testTakesCount;
        }

        public IEnumerable<Server> GetServers()
        {
            // GET SPEEDTEST SETTINGS
            speedTestSettings = speedTestClient.GetSettings();

            // GET SERVERS LIST
            List<Server> serversList = speedTestSettings.Servers.ToList();

            // TEMPORARY WORKING LIST
            List<Server> filteredServersList = new List<Server>();

            if (testServerSelectionMode == TestServerSelectionMode.AllServersExceptCurrentCountry)
            {
                foreach (Server serverItem in serversList)
                {
                    if (
                        !(serverItem.Country.ToLower() == GetStringCorrectEncoding(ipInfo.Country_Name.ToLower())) &&
                        !(serverItem.Country.ToLower() == GetStringCorrectEncoding(ipInfo.Country_Code.ToLower())) &&
                        filteredServersList.Count <= maxTestServersCount)
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
                        (serverItem.Country.ToLower() == GetStringCorrectEncoding(ipInfo.Country_Name.ToLower())) ||
                        ((serverItem.Country.ToLower() == GetStringCorrectEncoding(ipInfo.Country_Code.ToLower())) &&
                        filteredServersList.Count <= maxTestServersCount))
                    {
                        filteredServersList.Add(serverItem);
                    }
                }
            }
            else if (testServerSelectionMode == TestServerSelectionMode.AllServers)
            {
                filteredServersList = serversList.Take(maxTestServersCount).ToList();
            }

            foreach (Server server in filteredServersList.Take(maxTestServersCount))
            {
                for (int i = 0; i < 3; i++)
                {
                    int _serverLatency = speedTestClient.TestServerLatency(server);

                    if (i == 0 || server.Latency > _serverLatency)
                    {
                        server.Latency = _serverLatency;
                    }

                    Thread.Sleep(333);
                }

                ListSelectedServerDetails(server);
            }

            return filteredServersList;
        }

        public void AppendTextToLogBox(RichTextBox logBox, string resultLine, Color textColor, bool boldText)
        {
            // INVOKE DELEGATE ->> 
            ThreadSafeInvoke(() =>
            {
                // SELECT
                logBox.SelectionStart = logBox.TextLength;
                logBox.SelectionLength = 0;

                // APPEND TEXT
                logBox.SelectionColor = textColor;

                logBox.SelectionFont = boldText ? new Font(logBox.Font, FontStyle.Bold) : new Font(logBox.Font, FontStyle.Regular);

                logBox.AppendText(resultLine);
                logBox.AppendText(Environment.NewLine);

                logBox.SelectionColor = logBox.ForeColor;

                // SCROLL TO END
                logBox.SelectionStart = logBox.Text.Length;
                logBox.ScrollToCaret();

                Application.DoEvents();
            });
        }

        public string GetStringCorrectEncoding(string content)
        {
            byte[] encodedBytes = Encoding.Default.GetBytes(content);
            return Encoding.UTF8.GetString(encodedBytes);
        }

        public void GetUserCountry()
        {
            try
            {
                AppendTextToLogBox(
                            rtb_SpeedTest_LogConsole,
                                "Getting GeoLocation IP Info [by 'http://ip-api.com'] ...",
                            Color.Black,
                            true);

                // GET IP INFO / COUNTRY
                string info = new WebClient().DownloadString("http://ip-api.com/json");
                ipInfo = JsonConvert.DeserializeObject<IP_API_JSON_Response>(info);

                if (ipInfo.Service_Status != "success")
                {
                    throw new Exception("GeoLocation IP Info API returned status: " + ipInfo.Service_Status);
                }

                ThreadSafeInvoke(() =>
                {
                    lbl_SpeedTest_CurrentCountry_Value.BackColor = Color.BlanchedAlmond;
                    lbl_SpeedTest_CurrentCountry_Value.Text =
                        ipInfo.City +
                        "/" +
                        ipInfo.Country_Name;
                });

                AppendTextToLogBox(
                            rtb_SpeedTest_LogConsole,
                                "Country: " +
                                ipInfo.Country_Name +
                                Environment.NewLine +
                                "Country Code: " +
                                ipInfo.Country_Code +
                                Environment.NewLine +
                                "Region: " +
                                ipInfo.Region_Code +
                                Environment.NewLine +
                                "Region Name: " +
                                ipInfo.Region_Name +
                                Environment.NewLine +
                                "City: " +
                                ipInfo.City +
                                Environment.NewLine +
                                "ZIP Code: " +
                                ipInfo.City_ZIP_Code +
                                Environment.NewLine +
                                "GEO Latitude: " +
                                ipInfo.Geo_Lat +
                                Environment.NewLine +
                                "GEO Longitude: " +
                                ipInfo.Geo_Lon +
                                Environment.NewLine +
                                "Time Zone: " +
                                ipInfo.TimeZone +
                                Environment.NewLine +
                                "ISP: " +
                                ipInfo.ISP +
                                Environment.NewLine +
                                "ISP Organization: " +
                                ipInfo.ISP_ORG +
                                Environment.NewLine +
                                "ISP AS: " +
                                ipInfo.ISP_AS +
                                Environment.NewLine,
                            Color.Yellow,
                            true);
            }
            catch (Exception exception)
            {
                AppendTextToLogBox(
                            rtb_SpeedTest_LogConsole,
                                Environment.NewLine +
                                "ERROR: " +
                                BuildExceptionMessage(exception) +
                                Environment.NewLine,
                            Color.Red,
                            true);

                ThreadSafeInvoke(() =>
                {
                    SetProgessState(false);
                });
            }
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

            SaveServerScopePreferredSetting();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void rb_AllServers_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_AllServers.Checked)
            {
                rb_AllServersExceptCurrCountry.Checked = false;
                rb_CurrentCountryServersOnly.Checked = false;

                testServerSelectionMode = TestServerSelectionMode.AllServers;

                if (btn_SpeedTest_GetServers.Enabled)
                {
                    btn_SpeedTest_GetServers_Click(this, null);
                }
            }
        }

        public void rb_AllServersExceptCurrCountry_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_AllServersExceptCurrCountry.Checked)
            {
                rb_AllServers.Checked = false;
                rb_CurrentCountryServersOnly.Checked = false;

                testServerSelectionMode = TestServerSelectionMode.AllServersExceptCurrentCountry;

                if (btn_SpeedTest_GetServers.Enabled)
                {
                    btn_SpeedTest_GetServers_Click(this, null);
                }
            }
        }

        public void rb_CurrentCountryServersOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_CurrentCountryServersOnly.Checked)
            {
                rb_AllServers.Checked = false;
                rb_AllServersExceptCurrCountry.Checked = false;

                testServerSelectionMode = TestServerSelectionMode.OnlyServersFromCurrentCountry;

                if (btn_SpeedTest_GetServers.Enabled)
                {
                    btn_SpeedTest_GetServers_Click(this, null);
                }
            }
        }

        public void btn_SpeedTest_GetServers_Click(object sender, EventArgs e)
        {
            SetControlsCleanState();

            SetAGaugeControlsCleanState();

            SetProgessState(true);

            NewBackgroundThread(() =>
            {
                GetUserCountry();

                if (ipInfo != null)
                {
                    try
                    {
                        AppendTextToLogBox(
                                rtb_SpeedTest_LogConsole,
                                    "Getting test servers list [from 'https://www.speedtest.net'] ..." +
                                    Environment.NewLine,
                                Color.Black,
                                true);

                        IEnumerable<Server> servers = GetServers();

                        foreach (Server testServer in servers)
                        {
                            testServersList.Add(testServer);

                            ThreadSafeInvoke(() =>
                            {
                                cb_SpeedTest_TestServer.Items.Add(GetStringCorrectEncoding(testServer.Sponsor) +
                                                                  " (" +
                                                                  GetStringCorrectEncoding(testServer.Name) +
                                                                  "/" +
                                                                  GetStringCorrectEncoding(testServer.Country) +
                                                                  ")");
                            });
                        }

                        if (servers.Count() > 0)
                        {
                            ThreadSafeInvoke(() =>
                            {
                                cb_SpeedTest_TestServer.SelectedIndex = 0;

                                pb_GO.Visible = true;
                            });
                        }
                        else
                        {
                            AppendTextToLogBox(
                                                         rtb_SpeedTest_LogConsole,
                                                             "Not any test Server available (" +
                                                             GetEnumDescriptionString(testServerSelectionMode) +
                                                             ")" +
                                                             Environment.NewLine,
                                                         Color.Red,
                                                         true);
                        }
                    }
                    catch (Exception exception)
                    {
                        SetProgessState(false);

                        AppendTextToLogBox(
                                rtb_SpeedTest_LogConsole,
                                    "ERROR: " +
                                    BuildExceptionMessage(exception) +
                                    Environment.NewLine,
                                Color.Red,
                                true);
                    }
                }

                ThreadSafeInvoke(() =>
                {
                    SetProgessState(false);
                });
            });
        }

        public void SetControlsCleanState()
        {
            testServersList.Clear();

            rtb_SpeedTest_LogConsole.Text = string.Empty;

            lbl_SpeedTest_CurrentCountry_Value.Text = status_NotAvailable;
            lbl_SpeedTest_HostedBy_Value.Text = status_NotAvailable;
            lbl_SpeedTest_Distance_Value.Text = status_NotAvailable;
            lbl_SpeedTest_Latency_Value.Text = status_NotAvailable;

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
            pBar_Download.Visible = false;
            pBar_Download.Value = 0;

            pBar_Upload.Visible = false;
            pBar_Upload.Value = 0;

            lbl_SpeedTest_Mbps_Download_Label.Text = status_NotAvailable;
            lbl_SpeedTest_Mbps_Upload_Label.Text = status_NotAvailable;

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
            ThreadSafeInvoke(() =>
            {
                rb_AllServers.Enabled = !inProgress;
                rb_AllServersExceptCurrCountry.Enabled = !inProgress;
                rb_CurrentCountryServersOnly.Enabled = !inProgress;
                cb_SpeedTest_TestServer.Enabled = !inProgress && testServersList.Count > 1;
                btn_SpeedTest_GetServers.Visible = !inProgress;
                pb_SpeedTestProgress.Visible = inProgress;

                pb_GO.Visible =
                    !inProgress &&
                    clientIP != status_NotAvailable &&
                    cb_SpeedTest_TestServer.Items.Count > 1;
            });
        }

        public void cb_SpeedTest_TestServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetAGaugeControlsCleanState();

            if (testServersList.Count() > 1)
            {
                SelectServer();
                SetServerDetails();
            }
        }

        public void pb_GO_Click(object sender, EventArgs e)
        {
            SetProgessState(true);
            SelectServer();
            SpeedTestToServer();
        }

        public static string BuildExceptionMessage(Exception eX)
        {
            string exceptionMessage = Environment.NewLine;

            exceptionMessage += "==================";
            exceptionMessage += Environment.NewLine;

            // EX 'MESSAGE'
            exceptionMessage += eX.Message;

            // EX 'INNER EXCEPTION MESSAGE' [IF NOT THE SAME AS 'MESSAGE']
            if (eX.InnerException != null &&
                !eX.InnerException.Message.Contains(eX.Message))
            {
                exceptionMessage += Environment.NewLine;
                exceptionMessage += eX.InnerException.Message;
            }

            exceptionMessage += Environment.NewLine;
            exceptionMessage += "==================";

            return exceptionMessage;
        }

        public void cb_SpeedTest_TestServer_DrawItem(object sender, DrawItemEventArgs e)
        {
            // By using Sender, one method could handle multiple ComboBoxes
            ComboBox cbx = (ComboBox)sender;
            if (cbx != null)
            {
                // Always draw the background
                e.DrawBackground();

                // Drawing one of the items?
                if (e.Index >= 0)
                {
                    // Set the string alignment.  Choices are Center, Near and Far
                    StringFormat sf = new StringFormat
                    {
                        LineAlignment = StringAlignment.Center,
                        Alignment = StringAlignment.Center
                    };

                    // Set the Brush to ComboBox ForeColor to maintain any ComboBox color settings
                    // Assumes Brush is solid
                    Brush brush = new SolidBrush(Color.Yellow);

                    // If drawing highlighted selection, change brush
                    if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                    {
                        brush = SystemBrushes.HighlightText;
                    }

                    // Draw the string
                    e.Graphics.DrawString(cbx.Items[e.Index].ToString(), new Font("Segoe UI", 10, FontStyle.Regular), brush, e.Bounds, sf);
                }
            }
        }

        public string FormatLocationsDistanceString(
            string firstCountryName,
            string secondCountryName,
            string firstCityName,
            string secondCityName,
            int distanceKMs)
        {
            string locationString;

            if ((firstCountryName.ToLower() == secondCountryName.ToLower() &&
                 firstCityName.ToLower() == secondCityName.ToLower()) ||
                distanceKMs == 0)
            {
                // NO DISTANCE, SAME CITY/COUNTRY
                locationString =
                    "Right Here (" +
                    firstCityName +
                    "/" +
                    firstCountryName +
                    ")";
            }
            else
            {
                // CONCRETE DISTANCE
                locationString =
                    distanceKMs +
                    " km (from '" +
                    firstCityName +
                    "/" +
                    firstCountryName +
                    "' to '" +
                    secondCityName +
                    "/" +
                    secondCountryName +
                    "')";
            }

            return locationString;
        }

        public static Color GetColorByLatencyTime(int latencyTime)
        {
            return latencyTime <= 10 ? Color.LimeGreen : latencyTime <= 20 ? Color.Orange : Color.Red;
        }
    }

    public class ProgressBar_Green : ProgressBar
    {
        public ProgressBar_Green()
        {
            SetStyle(ControlStyles.UserPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rec = e.ClipRectangle;

            rec.Width = (int)(rec.Width * ((double)Value / Maximum)) - 4;
            if (ProgressBarRenderer.IsSupported)
            {
                ProgressBarRenderer.DrawHorizontalBar(e.Graphics, e.ClipRectangle);
            }

            rec.Height -= 4;
            e.Graphics.FillRectangle(Brushes.Green, 2, 2, rec.Width, rec.Height);
        }
    }

    public class ProgressBar_Red : ProgressBar
    {
        public ProgressBar_Red()
        {
            SetStyle(ControlStyles.UserPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rec = e.ClipRectangle;

            rec.Width = (int)(rec.Width * ((double)Value / Maximum)) - 4;
            if (ProgressBarRenderer.IsSupported)
            {
                ProgressBarRenderer.DrawHorizontalBar(e.Graphics, e.ClipRectangle);
            }

            rec.Height -= 4;
            e.Graphics.FillRectangle(Brushes.Red, 2, 2, rec.Width, rec.Height);
        }
    }
}