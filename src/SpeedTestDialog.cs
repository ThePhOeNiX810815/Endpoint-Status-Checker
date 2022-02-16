﻿using Newtonsoft.Json;
using NSpeedTest;
using NSpeedTest.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static EndpointChecker.Program;

namespace EndpointChecker
{
    public partial class SpeedTestDialog : Form
    {
        // 

        // MAXIMUM NUMBER OF TESTS SERVERS
        public static int maxTestServersCount = 50;

        public static string clientIP = status_NotAvailable;
        public static string clientISP = status_NotAvailable;

        public static int testTakesCount = 10;
        public static int testRetryCount = 5;

        // SPEED TEST SERVER / SETTINGS
        public static SpeedTestClient speedTestClient = new SpeedTestClient();
        public static Settings speedTestSettings = new Settings();

        public static List<Server> testServersList = new List<Server>();
        public enum TestServerSelectionMode
        {
            [Description("AllServers")]
            AllServers = 0,
            [Description("AllServersExceptCurrentCountry")]
            AllServersExceptCurrentCountry = 1,
            [Description("OnlyServersFromCurrentCountry")]
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

                    SpeedTest_AppendTextToLogBox(
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
                                        "Hosted by '" +
                                        GetStringCorrectEncoding(bestServer.Sponsor) +
                                        "' (" +
                                        GetStringCorrectEncoding(bestServer.Name) +
                                        "/" +
                                        GetStringCorrectEncoding(bestServer.Country) +
                                        "), distance: " + (int)bestServer.Distance / 1000 +
                                        "km, latency: " + bestServer.Latency +
                                        "ms" +
                                        Environment.NewLine,
                                    Color.DeepSkyBlue,
                                    true);

            return bestServer;
        }

        public void SpeedTestToServer(Server targetServer)
        {
            NewBackgroundThread(() =>
            {
                try
                {
                    SpeedTest_AppendTextToLogBox(
                                        rtb_SpeedTest_LogConsole,
                                             Environment.NewLine +
                                             "Testing Download Speed by " +
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

                        pBar_Download.Visible = false;
                        pBar_Download.Value = 0;
                        aGauge_DownloadSpeed.Value = downloadSpeed;
                        lbl_SpeedTest_Mbps_Download_Label.BackColor = Color.PaleGreen;
                        lbl_SpeedTest_Download_Label.ForeColor = Color.Green;
                        aGauge_DownloadSpeed.NeedleColor1 = AGaugeNeedleColor.Green;
                        lbl_SpeedTest_Mbps_Download_Label.Text = downloadSpeed.ToString() + " Mbps";

                        Application.DoEvents();
                    });

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

                        pBar_Upload.Visible = false;
                        pBar_Upload.Value = 0;
                        aGauge_UploadSpeed.Value = uploadSpeed;
                        lbl_SpeedTest_Mbps_Upload_Label.BackColor = Color.LightSalmon;
                        lbl_SpeedTest_Upload_Label.ForeColor = Color.Red;
                        aGauge_UploadSpeed.NeedleColor1 = AGaugeNeedleColor.Red;
                        lbl_SpeedTest_Mbps_Upload_Label.Text = uploadSpeed.ToString() + " Mbps";

                        Application.DoEvents();
                    });

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

        public int GetDownloadSpeed(Server targetServer)
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

                    SpeedTest_AppendTextToLogBox(
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
                        SpeedTest_AppendTextToLogBox(
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

            return (downloadSpeed / testTakesCount);
        }

        public int GetUploadSpeed(Server targetServer)
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

                    SpeedTest_AppendTextToLogBox(
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
                        SpeedTest_AppendTextToLogBox(
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
                        !(serverItem.Country.ToLower() == GetStringCorrectEncoding(regionInfo_CurrentCountry.DisplayName.ToLower())) &&
                        !(serverItem.Country.ToLower() == GetStringCorrectEncoding(regionInfo_CurrentCountry.EnglishName.ToLower())) &&
                        !(serverItem.Country.ToLower() == GetStringCorrectEncoding(regionInfo_CurrentCountry.Name.ToLower())) &&
                        !(serverItem.Country.ToLower() == GetStringCorrectEncoding(regionInfo_CurrentCountry.NativeName.ToLower())) &&
                        !(serverItem.Country.ToLower() == GetStringCorrectEncoding(regionInfo_CurrentCountry.TwoLetterISORegionName.ToLower())) &&
                        !(serverItem.Country.ToLower() == GetStringCorrectEncoding(regionInfo_CurrentCountry.ThreeLetterISORegionName.ToLower())) &&
                        !(serverItem.Country.ToLower() == GetStringCorrectEncoding(regionInfo_CurrentCountry.ThreeLetterWindowsRegionName.ToLower())))
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
                        (serverItem.Country.ToLower() == GetStringCorrectEncoding(regionInfo_CurrentCountry.DisplayName.ToLower())) ||
                        (serverItem.Country.ToLower() == GetStringCorrectEncoding(regionInfo_CurrentCountry.EnglishName.ToLower())) ||
                        (serverItem.Country.ToLower() == GetStringCorrectEncoding(regionInfo_CurrentCountry.Name.ToLower())) ||
                        (serverItem.Country.ToLower() == GetStringCorrectEncoding(regionInfo_CurrentCountry.NativeName.ToLower())) ||
                        (serverItem.Country.ToLower() == GetStringCorrectEncoding(regionInfo_CurrentCountry.TwoLetterISORegionName.ToLower())) ||
                        (serverItem.Country.ToLower() == GetStringCorrectEncoding(regionInfo_CurrentCountry.ThreeLetterISORegionName.ToLower())) ||
                        (serverItem.Country.ToLower() == GetStringCorrectEncoding(regionInfo_CurrentCountry.ThreeLetterWindowsRegionName.ToLower())))
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
                                "Hosted by '" +
                                GetStringCorrectEncoding(server.Sponsor) +
                                "' (" +
                                GetStringCorrectEncoding(server.Name) +
                                "/" +
                                GetStringCorrectEncoding(server.Country) +
                                "), distance: " +
                                (int)server.Distance / 1000 +
                                "km, latency: " +
                                server.Latency + "ms",
                            Color.White,
                            true);
            }

            return filteredServersList;
        }

        public void SpeedTest_AppendTextToLogBox(RichTextBox logBox, string resultLine, Color textColor, bool boldText)
        {
            // INVOKE DELEGATE ->> 
            ThreadSafeInvoke(() =>
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

                Application.DoEvents();
            });
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

                ThreadSafeInvoke(() =>
                {
                    lbl_SpeedTest_CurrentCountry_Value.BackColor = Color.BlanchedAlmond;
                    lbl_SpeedTest_CurrentCountry_Value.Text =
                        regionInfo.DisplayName +
                        " (" +
                        regionInfo.TwoLetterISORegionName +
                        ")";
                });

                SpeedTest_AppendTextToLogBox(
                            rtb_SpeedTest_LogConsole,
                                "Current Country: " +
                                regionInfo.DisplayName +
                                Environment.NewLine +
                                "Country Code: " +
                                regionInfo.TwoLetterISORegionName +
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
                                BuildExceptionMessage(exception) +
                                Environment.NewLine,
                            Color.Red,
                            true);

                ThreadSafeInvoke(() =>
                {
                    SetProgessState(false);
                });
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

            SaveServerScopePreferredSetting();
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

        public void comboBox1_DrawItem(object sender, DrawItemEventArgs e)
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

            NewBackgroundThread(() =>
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

                        IEnumerable<Server> servers = GetServers(
                                                                 speedTestSettings,
                                                                 currentCountry_RegionInfo,
                                                                 speedTestClient,
                                                                 maxTestServersCount);

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
                cb_SpeedTest_TestServer.Enabled = !inProgress;
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

        public static string BuildExceptionMessage(Exception eX)
        {
            string exceptionMessage =
                Environment.NewLine +
                eX.Message;

            if (eX.InnerException != null &&
                !eX.InnerException.Message.Contains(eX.Message))
            {
                exceptionMessage +=
                    Environment.NewLine +
                    eX.InnerException.Message;
            }

            return exceptionMessage;
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
                ProgressBarRenderer.DrawHorizontalBar(e.Graphics, e.ClipRectangle);
            rec.Height = rec.Height - 4;
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
                ProgressBarRenderer.DrawHorizontalBar(e.Graphics, e.ClipRectangle);
            rec.Height = rec.Height - 4;
            e.Graphics.FillRectangle(Brushes.Red, 2, 2, rec.Width, rec.Height);
        }
    }
}