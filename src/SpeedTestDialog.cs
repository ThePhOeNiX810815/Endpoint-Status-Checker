using Newtonsoft.Json;
using NSpeedTest;
using NSpeedTest.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static EndpointChecker.CheckerMainForm;
using static EndpointChecker.Program;

namespace EndpointChecker
{
    public partial class SpeedTestDialog : Form
    {
        // MAXIMUM NUMBER OF TESTS SERVERS [API HARD-LIMITED TO 10, BUT NO ONE KNOWS...]
        public static int maxTestServersCount = 50;
        public static int maxLatencyProbeCandidates = 12;

        public static string clientIP = status_NotAvailable;
        public static string clientISP = status_NotAvailable;

        public static int testTakesCount = 3;
        public static int testRetryCount = 5;
        public const int DownloadBenchmarkConcurrency = 18;
        public const int UploadBenchmarkConcurrency = 6;
        public const int ThroughputQualificationCandidates = 4;
        public const int ThroughputQualificationConcurrency = 16;
        private static readonly TimeSpan ThroughputQualificationWarmup = TimeSpan.FromMilliseconds(1200);
        private static readonly TimeSpan ThroughputQualificationDuration = TimeSpan.FromMilliseconds(4200);

        // SPEED TEST SERVER / SETTINGS
        public static SpeedTestClient speedTestClient = new SpeedTestClient();
        public static Settings speedTestSettings = new Settings();

        // TARGET TEST SERVER
        public static Server targetServer = new Server();

        // IP INFO API RESPONSE
        public static SpeedTestGeoInfo ipInfo;

        public static List<Server> testServersList = new List<Server>();
        private readonly ConcurrentDictionary<string, double> serverQualifiedDownloadMbps = new ConcurrentDictionary<string, double>();

        // SERVER SCOPE SETTING
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

        // VALUES CALCULATION SETTING
        public enum ValuesCalculationMode
        {
            [Description("Best Values")]
            BestValues = 0,
            [Description("Average Values")]
            AverageValues = 1,
        }

        public ValuesCalculationMode valuesCalculationMode = ValuesCalculationMode.BestValues;

        private readonly Color colorPageBackground = Color.FromArgb(10, 16, 31);
        private readonly Color colorSurface = Color.FromArgb(20, 28, 48);
        private readonly Color colorSurfaceAlt = Color.FromArgb(26, 35, 60);
        private readonly Color colorInput = Color.FromArgb(15, 22, 40);
        private readonly Color colorBorder = Color.FromArgb(52, 68, 110);
        private readonly Color colorTextPrimary = Color.FromArgb(236, 242, 255);
        private readonly Color colorTextSecondary = Color.FromArgb(148, 164, 196);
        private readonly Color colorAccent = Color.FromArgb(86, 154, 255);
        private readonly Color colorSuccess = Color.FromArgb(48, 196, 141);
        private readonly Color colorWarning = Color.FromArgb(255, 184, 77);
        private readonly Color colorDanger = Color.FromArgb(255, 107, 129);
        private readonly Color colorInfo = Color.FromArgb(91, 195, 255);
        private readonly Color colorSurfaceHighlight = Color.FromArgb(34, 46, 76);
        private readonly Color colorSuccessSurface = Color.FromArgb(24, 58, 50);
        private readonly Color colorWarningSurface = Color.FromArgb(71, 52, 19);
        private readonly Color colorDangerSurface = Color.FromArgb(73, 32, 43);

        private PremiumSurfacePanel panel_Header;
        private PremiumSurfacePanel panel_Details;
        private PremiumSurfacePanel panel_Log;
        private PremiumSurfacePanel panel_Download;
        private PremiumSurfacePanel panel_Upload;
        private Label lbl_HeaderCaption;
        private Label lbl_LogCaption;
        private Label lbl_LogHint;
        private Label lbl_DownloadHint;
        private Label lbl_UploadHint;
        private Label lbl_HeaderStatus;
        private Button btn_RunSpeedTest;
        private Label lbl_DownloadTrendCaption;
        private Label lbl_UploadTrendCaption;
        private SparklinePanel panel_DownloadTrend;
        private SparklinePanel panel_UploadTrend;
        private PublicIdentityResponse cachedPublicIdentity;
        private string resolvedPublicIp = string.Empty;
        private DateTime lastDownloadTrendUpdateUtc = DateTime.MinValue;
        private DateTime lastUploadTrendUpdateUtc = DateTime.MinValue;
        private int lastDownloadTrendValue = -1;
        private int lastUploadTrendValue = -1;
        private const int GaugeAnimationDelayMs = 8;
        private readonly List<int> downloadSpeedHistory = new List<int>();
        private readonly List<int> uploadSpeedHistory = new List<int>();
        private readonly System.Windows.Forms.Timer motionPulseTimer = new System.Windows.Forms.Timer();
        private readonly System.Windows.Forms.Timer revealTimer = new System.Windows.Forms.Timer();
        private readonly System.Windows.Forms.Timer startupGaugeSweepTimer = new System.Windows.Forms.Timer();
        private readonly List<Control> revealSequence = new List<Control>();
        private int revealSequenceIndex = 0;
        private int pulseTick = 0;
        private int gaugeSweepStep = 0;
        private bool gaugeSweepReturning = false;

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

            // ADD SETTINGS OPTIONS
            cb_SpeedTest_ServerScope.Items.Add("All Servers");
            cb_SpeedTest_ServerScope.Items.Add("All Except Current Country");
            cb_SpeedTest_ServerScope.Items.Add("Current Country Only");

            cb_SpeedTest_Calculation.Items.Add("Best Values");
            cb_SpeedTest_Calculation.Items.Add("Average Values");

            // RESTORE PREFERRED SETTINGS (IF SAVED)
            RestorePreferredSettings();

            InitializePremiumUi();
            InitializePremiumMotionEffects();
            HandleCreated += (s, e) => TryUseDarkTitleBar();

            NewBackgroundThread(() =>
            {
                try
                {
                    // GET SPEEDTEST SETTINGS
                    AppendTextToLogBox(
                                         rtb_SpeedTest_LogConsole,
                                         "Retrieving OOKLA's SpeedTest API Configuration ...",
                                         Color.Blue,
                                         true);

                    speedTestSettings = speedTestClient.GetSettings();
                    PublicIdentityResponse publicIdentity = ResolvePublicIdentity();
                    cachedPublicIdentity = publicIdentity;
                    resolvedPublicIp = publicIdentity?.Ip ?? string.Empty;

                    ThreadSafeInvoke(() =>
                    {
                        clientIP = string.IsNullOrWhiteSpace(publicIdentity?.Ip) ? status_NotAvailable : publicIdentity.Ip;
                        clientISP = ResolveBestIspProvider(publicIdentity);

                        UpdatePublicIdentityDisplay();

                        btn_SpeedTest_GetServers_Click(this, null);
                    });
                }
                catch (Exception exception)
                {
                    SetProgressState(false);

                    ThreadSafeInvoke(() =>
                    {
                        cb_SpeedTest_TestServer.Items.Add(status_NotAvailable);
                        cb_SpeedTest_TestServer.SelectedIndex = 0;

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

        private void InitializePremiumUi()
        {
            SuspendLayout();

            BackColor = colorPageBackground;
            ForeColor = colorTextPrimary;
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            TopMost = false;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(1180, 860);
            MinimumSize = new Size(1196, 899);
            MaximumSize = new Size(1196, 899);
            StartPosition = FormStartPosition.CenterParent;

            CreateOrUpdateSurfacePanel(ref panel_Header, "panel_Header", new Rectangle(18, 16, 1144, 92));
            CreateOrUpdateSurfacePanel(ref panel_Details, "panel_Details", new Rectangle(18, 124, 1144, 308));
            CreateOrUpdateSurfacePanel(ref panel_Log, "panel_Log", new Rectangle(18, 448, 1144, 158));
            CreateOrUpdateSurfacePanel(ref panel_Download, "panel_Download", new Rectangle(18, 622, 560, 220));
            CreateOrUpdateSurfacePanel(ref panel_Upload, "panel_Upload", new Rectangle(602, 622, 560, 220));

            if (lbl_HeaderCaption == null)
            {
                lbl_HeaderCaption = new Label();
                Controls.Add(lbl_HeaderCaption);
            }

            if (btn_RunSpeedTest == null)
            {
                btn_RunSpeedTest = new Button();
                btn_RunSpeedTest.Click += pb_GO_Click;
                Controls.Add(btn_RunSpeedTest);
            }

            EnsurePremiumLabel(ref lbl_LogCaption);
            EnsurePremiumLabel(ref lbl_LogHint);
            EnsurePremiumLabel(ref lbl_DownloadHint);
            EnsurePremiumLabel(ref lbl_UploadHint);
            EnsurePremiumLabel(ref lbl_HeaderStatus);
            EnsurePremiumLabel(ref lbl_DownloadTrendCaption);
            EnsurePremiumLabel(ref lbl_UploadTrendCaption);

            label1.BorderStyle = BorderStyle.None;
            label1.BackColor = colorSurfaceAlt;
            label1.ForeColor = colorTextPrimary;
            label1.Font = new Font("Segoe UI Semibold", 22F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new System.Drawing.Point(42, 24);
            label1.Size = new Size(520, 42);
            label1.Text = "SpeedTest Control Center";
            label1.TextAlign = ContentAlignment.MiddleLeft;

            lbl_HeaderCaption.BackColor = colorSurfaceAlt;
            lbl_HeaderCaption.ForeColor = colorTextSecondary;
            lbl_HeaderCaption.Font = new Font("Segoe UI", 10.25F, FontStyle.Regular, GraphicsUnit.Point);
            lbl_HeaderCaption.Location = new System.Drawing.Point(44, 64);
            lbl_HeaderCaption.Size = new Size(690, 20);
            lbl_HeaderCaption.Text = "High-visibility latency and throughput telemetry with curated server discovery, route selection, and a live benchmark console.";

            lbl_HeaderStatus.BackColor = colorInput;
            lbl_HeaderStatus.ForeColor = colorInfo;
            lbl_HeaderStatus.Font = new Font("Segoe UI Semibold", 9.25F, FontStyle.Bold, GraphicsUnit.Point);
            lbl_HeaderStatus.Location = new System.Drawing.Point(724, 40);
            lbl_HeaderStatus.Size = new Size(56, 38);
            lbl_HeaderStatus.TextAlign = ContentAlignment.MiddleCenter;
            lbl_HeaderStatus.Text = "INIT";

            StyleActionButton(btn_SpeedTest_GetServers, "Refresh Servers", new Rectangle(788, 40, 150, 38), colorInput, colorBorder, colorTextPrimary);
            StyleActionButton(btn_RunSpeedTest, "Run Benchmark", new Rectangle(952, 40, 164, 38), colorAccent, colorAccent, Color.White);
            btn_RunSpeedTest.Visible = false;
            btn_RunSpeedTest.Enabled = false;
            btn_RunSpeedTest.BringToFront();
            btn_SpeedTest_GetServers.BringToFront();

            pb_GO.Visible = false;
            pb_GO.Enabled = false;

            pb_SpeedTestProgress.BackColor = colorSurfaceAlt;
            pb_SpeedTestProgress.Location = new System.Drawing.Point(1082, 20);
            pb_SpeedTestProgress.Size = new Size(56, 56);
            pb_SpeedTestProgress.BringToFront();

            StyleCaptionLabel(lbl_SpeedTest_ExternalIP, "Public IP and ISP");
            StyleCaptionLabel(lbl_SpeedTest_CurrentCountry, "Approximate Location");
            StyleCaptionLabel(lbl_SpeedTest_TestServer, "Benchmark Server");
            StyleCaptionLabel(lbl_SpeedTest_HostedBy, "Hosted By");
            StyleCaptionLabel(lbl_SpeedTest_Distance, "Server Distance");
            StyleCaptionLabel(lbl_SpeedTest_Latency, "Latency");
            StyleCaptionLabel(lbl_SpeedTest_ServerScope, "Server Scope");
            StyleCaptionLabel(lbl_SpeedTest_Calculation, "Result Mode");

            StyleValueLabel(lbl_SpeedTest_ExternalIP_Value, ContentAlignment.MiddleLeft);
            StyleValueLabel(lbl_SpeedTest_CurrentCountry_Value, ContentAlignment.MiddleLeft);
            StyleValueLabel(lbl_SpeedTest_HostedBy_Value, ContentAlignment.MiddleLeft);
            StyleValueLabel(lbl_SpeedTest_Distance_Value, ContentAlignment.MiddleLeft);
            StyleValueLabel(lbl_SpeedTest_Latency_Value, ContentAlignment.MiddleCenter);

            StyleComboBox(cb_SpeedTest_TestServer);
            StyleComboBox(cb_SpeedTest_ServerScope);
            StyleComboBox(cb_SpeedTest_Calculation);

            lbl_SpeedTest_ExternalIP.Location = new System.Drawing.Point(42, 150);
            lbl_SpeedTest_ExternalIP.Size = new Size(180, 18);
            lbl_SpeedTest_ExternalIP_Value.Location = new System.Drawing.Point(42, 174);
            lbl_SpeedTest_ExternalIP_Value.Size = new Size(500, 34);

            lbl_SpeedTest_CurrentCountry.Location = new System.Drawing.Point(594, 150);
            lbl_SpeedTest_CurrentCountry.Size = new Size(180, 18);
            lbl_SpeedTest_CurrentCountry_Value.Location = new System.Drawing.Point(594, 174);
            lbl_SpeedTest_CurrentCountry_Value.Size = new Size(526, 34);

            lbl_SpeedTest_TestServer.Location = new System.Drawing.Point(42, 224);
            lbl_SpeedTest_TestServer.Size = new Size(220, 18);
            cb_SpeedTest_TestServer.Location = new System.Drawing.Point(42, 248);
            cb_SpeedTest_TestServer.Size = new Size(1078, 34);

            lbl_SpeedTest_HostedBy.Location = new System.Drawing.Point(42, 298);
            lbl_SpeedTest_HostedBy.Size = new Size(180, 18);
            lbl_SpeedTest_HostedBy_Value.Location = new System.Drawing.Point(42, 322);
            lbl_SpeedTest_HostedBy_Value.Size = new Size(1078, 34);

            lbl_SpeedTest_Distance.Location = new System.Drawing.Point(42, 372);
            lbl_SpeedTest_Distance.Size = new Size(180, 18);
            lbl_SpeedTest_Distance_Value.Location = new System.Drawing.Point(42, 396);
            lbl_SpeedTest_Distance_Value.Size = new Size(412, 34);

            lbl_SpeedTest_Latency.Location = new System.Drawing.Point(472, 372);
            lbl_SpeedTest_Latency.Size = new Size(120, 18);
            lbl_SpeedTest_Latency_Value.Location = new System.Drawing.Point(472, 396);
            lbl_SpeedTest_Latency_Value.Size = new Size(160, 34);

            lbl_SpeedTest_ServerScope.Location = new System.Drawing.Point(650, 372);
            lbl_SpeedTest_ServerScope.Size = new Size(150, 18);
            cb_SpeedTest_ServerScope.Location = new System.Drawing.Point(650, 396);
            cb_SpeedTest_ServerScope.Size = new Size(228, 34);

            lbl_SpeedTest_Calculation.Location = new System.Drawing.Point(896, 372);
            lbl_SpeedTest_Calculation.Size = new Size(140, 18);
            cb_SpeedTest_Calculation.Location = new System.Drawing.Point(896, 396);
            cb_SpeedTest_Calculation.Size = new Size(224, 34);

            lbl_LogCaption.BackColor = colorSurface;
            lbl_LogCaption.ForeColor = colorTextPrimary;
            lbl_LogCaption.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lbl_LogCaption.Location = new System.Drawing.Point(36, 458);
            lbl_LogCaption.Size = new Size(170, 22);
            lbl_LogCaption.Text = "Session Log";

            lbl_LogHint.BackColor = colorSurface;
            lbl_LogHint.ForeColor = colorTextSecondary;
            lbl_LogHint.Font = new Font("Segoe UI", 8.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbl_LogHint.Location = new System.Drawing.Point(36, 482);
            lbl_LogHint.Size = new Size(320, 18);
            lbl_LogHint.Text = "Live routing, latency, download, and upload traces";

            rtb_SpeedTest_LogConsole.BackColor = colorInput;
            rtb_SpeedTest_LogConsole.ForeColor = colorTextPrimary;
            rtb_SpeedTest_LogConsole.BorderStyle = BorderStyle.None;
            rtb_SpeedTest_LogConsole.Font = new Font("Consolas", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
            rtb_SpeedTest_LogConsole.Location = new System.Drawing.Point(32, 510);
            rtb_SpeedTest_LogConsole.Size = new Size(1112, 82);

            ConfigureGauge(aGauge_DownloadSpeed, new Rectangle(44, 682, 192, 144));
            ConfigureGauge(aGauge_UploadSpeed, new Rectangle(628, 682, 192, 144));

            lbl_SpeedTest_Download_Label.BackColor = colorSurface;
            lbl_SpeedTest_Download_Label.BorderStyle = BorderStyle.None;
            lbl_SpeedTest_Download_Label.ForeColor = colorTextPrimary;
            lbl_SpeedTest_Download_Label.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            lbl_SpeedTest_Download_Label.Location = new System.Drawing.Point(44, 642);
            lbl_SpeedTest_Download_Label.Size = new Size(300, 32);
            lbl_SpeedTest_Download_Label.Text = "Download Throughput";
            lbl_SpeedTest_Download_Label.TextAlign = ContentAlignment.MiddleLeft;

            lbl_DownloadHint.BackColor = colorSurface;
            lbl_DownloadHint.ForeColor = colorTextSecondary;
            lbl_DownloadHint.Font = new Font("Segoe UI", 8.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbl_DownloadHint.Location = new System.Drawing.Point(44, 672);
            lbl_DownloadHint.Size = new Size(270, 16);
            lbl_DownloadHint.Text = "Observed throughput across repeated test passes";

            lbl_SpeedTest_Upload_Label.BackColor = colorSurface;
            lbl_SpeedTest_Upload_Label.BorderStyle = BorderStyle.None;
            lbl_SpeedTest_Upload_Label.ForeColor = colorTextPrimary;
            lbl_SpeedTest_Upload_Label.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            lbl_SpeedTest_Upload_Label.Location = new System.Drawing.Point(628, 642);
            lbl_SpeedTest_Upload_Label.Size = new Size(300, 32);
            lbl_SpeedTest_Upload_Label.Text = "Upload Throughput";
            lbl_SpeedTest_Upload_Label.TextAlign = ContentAlignment.MiddleLeft;

            lbl_UploadHint.BackColor = colorSurface;
            lbl_UploadHint.ForeColor = colorTextSecondary;
            lbl_UploadHint.Font = new Font("Segoe UI", 8.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbl_UploadHint.Location = new System.Drawing.Point(628, 672);
            lbl_UploadHint.Size = new Size(274, 16);
            lbl_UploadHint.Text = "Measured against selected host using multi-pass upload";

            lbl_SpeedTest_Mbps_Download_Label.BackColor = colorSuccessSurface;
            lbl_SpeedTest_Mbps_Download_Label.ForeColor = colorTextPrimary;
            lbl_SpeedTest_Mbps_Download_Label.Font = new Font("Segoe UI Semibold", 25F, FontStyle.Bold, GraphicsUnit.Point);
            lbl_SpeedTest_Mbps_Download_Label.Location = new System.Drawing.Point(286, 700);
            lbl_SpeedTest_Mbps_Download_Label.Size = new Size(228, 48);
            lbl_SpeedTest_Mbps_Download_Label.BorderStyle = BorderStyle.None;

            lbl_SpeedTest_Mbps_Upload_Label.BackColor = colorDangerSurface;
            lbl_SpeedTest_Mbps_Upload_Label.ForeColor = colorTextPrimary;
            lbl_SpeedTest_Mbps_Upload_Label.Font = new Font("Segoe UI Semibold", 25F, FontStyle.Bold, GraphicsUnit.Point);
            lbl_SpeedTest_Mbps_Upload_Label.Location = new System.Drawing.Point(870, 700);
            lbl_SpeedTest_Mbps_Upload_Label.Size = new Size(228, 48);
            lbl_SpeedTest_Mbps_Upload_Label.BorderStyle = BorderStyle.None;

            pBar_Download.Location = new System.Drawing.Point(286, 817);
            pBar_Download.Size = new Size(228, 10);
            pBar_Download.BackColor = colorInput;

            pBar_Upload.Location = new System.Drawing.Point(870, 817);
            pBar_Upload.Size = new Size(228, 10);
            pBar_Upload.BackColor = colorInput;

            CreateOrUpdateSparklinePanel(ref panel_DownloadTrend, "panel_DownloadTrend", new Rectangle(286, 762, 228, 50), Color.FromArgb(76, 214, 145), Color.FromArgb(30, 88, 64));
            CreateOrUpdateSparklinePanel(ref panel_UploadTrend, "panel_UploadTrend", new Rectangle(870, 762, 228, 50), Color.FromArgb(255, 117, 136), Color.FromArgb(88, 43, 54));

            StyleTrendCaptionLabel(lbl_DownloadTrendCaption, "Live Trend");
            lbl_DownloadTrendCaption.Location = new System.Drawing.Point(286, 744);
            lbl_DownloadTrendCaption.Size = new Size(228, 16);

            StyleTrendCaptionLabel(lbl_UploadTrendCaption, "Live Trend");
            lbl_UploadTrendCaption.Location = new System.Drawing.Point(870, 744);
            lbl_UploadTrendCaption.Size = new Size(228, 16);

            UpdateActionButtonsAndStatus(inProgress: false);

            ResumeLayout(false);
        }

        private void InitializePremiumMotionEffects()
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return;
            }

            revealSequence.Clear();
            revealSequence.Add(panel_Header);
            revealSequence.Add(panel_Details);
            revealSequence.Add(panel_Log);
            revealSequence.Add(panel_Download);
            revealSequence.Add(panel_Upload);

            foreach (Control section in revealSequence)
            {
                if (section != null)
                {
                    section.Visible = false;
                }
            }

            revealSequenceIndex = 0;
            Opacity = 0.93;

            revealTimer.Interval = 70;
            revealTimer.Tick -= RevealTimer_Tick;
            revealTimer.Tick += RevealTimer_Tick;
            revealTimer.Start();

            motionPulseTimer.Interval = 90;
            motionPulseTimer.Tick -= MotionPulseTimer_Tick;
            motionPulseTimer.Tick += MotionPulseTimer_Tick;
            motionPulseTimer.Start();

            startupGaugeSweepTimer.Interval = 18;
            startupGaugeSweepTimer.Tick -= StartupGaugeSweepTimer_Tick;
            startupGaugeSweepTimer.Tick += StartupGaugeSweepTimer_Tick;
            startupGaugeSweepTimer.Start();
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

        private void RevealTimer_Tick(object sender, EventArgs e)
        {
            if (revealSequenceIndex < revealSequence.Count)
            {
                Control section = revealSequence[revealSequenceIndex];
                if (section != null)
                {
                    section.Visible = true;
                    section.Invalidate();
                }

                revealSequenceIndex++;
            }

            if (Opacity < 1)
            {
                Opacity = Math.Min(1, Opacity + 0.015);
            }

            if (revealSequenceIndex >= revealSequence.Count && Opacity >= 1)
            {
                revealTimer.Stop();
            }
        }

        private void MotionPulseTimer_Tick(object sender, EventArgs e)
        {
            pulseTick++;
            double wave = (Math.Sin(pulseTick * 0.22) + 1d) / 2d;
            int alpha = 110 + (int)(wave * 90);
            Color pulseBorder = Color.FromArgb(alpha, colorAccent);

            if (panel_Header != null)
            {
                panel_Header.BorderColor = pulseBorder;
            }

            if (btn_RunSpeedTest != null && btn_RunSpeedTest.Visible && btn_RunSpeedTest.Enabled)
            {
                int offset = 8 + (int)(wave * 22);
                btn_RunSpeedTest.FlatAppearance.BorderColor = Color.FromArgb(90 + offset, 136 + offset, 255);
            }
        }

        private void StartupGaugeSweepTimer_Tick(object sender, EventArgs e)
        {
            int previewCap = 32;
            int stepSize = 2;

            if (!gaugeSweepReturning)
            {
                gaugeSweepStep += stepSize;
                if (gaugeSweepStep >= previewCap)
                {
                    gaugeSweepStep = previewCap;
                    gaugeSweepReturning = true;
                }
            }
            else
            {
                gaugeSweepStep -= stepSize;
                if (gaugeSweepStep <= 0)
                {
                    aGauge_DownloadSpeed.Value = 0;
                    aGauge_UploadSpeed.Value = 0;
                    startupGaugeSweepTimer.Stop();
                    return;
                }
            }

            aGauge_DownloadSpeed.Value = gaugeSweepStep;
            aGauge_UploadSpeed.Value = Math.Max(0, gaugeSweepStep - 3);
        }

        private void CreateOrUpdateSurfacePanel(ref PremiumSurfacePanel panel, string name, Rectangle bounds)
        {
            if (panel == null)
            {
                panel = new PremiumSurfacePanel();
                panel.Name = name;
                Controls.Add(panel);
            }

            panel.Bounds = bounds;
            panel.FillColor = name == "panel_Header" ? colorSurfaceAlt : colorSurface;
            panel.BorderColor = colorBorder;
            panel.BackColor = panel.FillColor;
            panel.SendToBack();
        }

        private void CreateOrUpdateSparklinePanel(ref SparklinePanel panel, string name, Rectangle bounds, Color lineColor, Color fillColor)
        {
            if (panel == null)
            {
                panel = new SparklinePanel
                {
                    Name = name,
                    BorderStyle = BorderStyle.None,
                };
                Controls.Add(panel);
            }

            panel.Bounds = bounds;
            panel.LineColor = lineColor;
            panel.FillColor = fillColor;
            panel.BackColor = colorSurfaceAlt;
            panel.BringToFront();
        }

        private void StyleTrendCaptionLabel(Label label, string text)
        {
            label.Text = text;
            label.BackColor = colorSurface;
            label.ForeColor = colorTextSecondary;
            label.Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold, GraphicsUnit.Point);
            label.TextAlign = ContentAlignment.MiddleRight;
        }

        private void EnsurePremiumLabel(ref Label label)
        {
            if (label == null)
            {
                label = new Label();
                Controls.Add(label);
            }
        }

        private void StyleCaptionLabel(Label label, string text)
        {
            label.Text = text;
            label.BackColor = colorSurface;
            label.BorderStyle = BorderStyle.None;
            label.ForeColor = colorTextSecondary;
            label.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label.TextAlign = ContentAlignment.MiddleLeft;
        }

        private void StyleValueLabel(Label label, ContentAlignment textAlignment)
        {
            label.BackColor = colorSurfaceAlt;
            label.BorderStyle = BorderStyle.None;
            label.ForeColor = colorTextPrimary;
            label.Font = new Font("Segoe UI Semibold", 10.25F, FontStyle.Bold, GraphicsUnit.Point);
            label.TextAlign = textAlignment;
        }

        private void StyleComboBox(ComboBox comboBox)
        {
            comboBox.BackColor = colorInput;
            comboBox.ForeColor = colorTextPrimary;
            comboBox.FlatStyle = FlatStyle.Flat;
            comboBox.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold, GraphicsUnit.Point);
            comboBox.IntegralHeight = false;
        }

        private Color NormalizeLogColor(Color sourceColor)
        {
            if (sourceColor == Color.Black || sourceColor == Color.DarkGray)
            {
                return colorTextSecondary;
            }

            if (sourceColor == Color.White)
            {
                return colorTextPrimary;
            }

            if (sourceColor == Color.Red)
            {
                return colorDanger;
            }

            if (sourceColor == Color.Yellow || sourceColor == Color.Orange)
            {
                return colorWarning;
            }

            if (sourceColor == Color.LimeGreen || sourceColor == Color.LightGreen || sourceColor == Color.PaleGreen)
            {
                return colorSuccess;
            }

            if (sourceColor == Color.LightSkyBlue)
            {
                return colorInfo;
            }

            if (sourceColor == Color.LightPink || sourceColor == Color.LightSalmon)
            {
                return colorDanger;
            }

            return sourceColor;
        }

        private void StyleActionButton(Button button, string text, Rectangle bounds, Color backColor, Color borderColor, Color foreColor)
        {
            button.Text = text;
            button.Bounds = bounds;
            button.BackColor = backColor;
            button.ForeColor = foreColor;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = borderColor;
            button.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(backColor, 0.2F);
            button.FlatAppearance.MouseOverBackColor = ControlPaint.Light(backColor, 0.1F);
            button.Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold, GraphicsUnit.Point);
            button.Cursor = Cursors.Hand;
        }

        private void ConfigureGauge(System.Windows.Forms.AGauge gauge, Rectangle bounds)
        {
            gauge.BackColor = colorSurface;
            gauge.BaseArcColor = colorBorder;
            gauge.BaseArcRadius = 50;
            gauge.BaseArcStart = 135;
            gauge.BaseArcSweep = 270;
            gauge.BaseArcWidth = 3;
            gauge.Center = new System.Drawing.Point(96, 82);
            gauge.Font = new Font("Segoe UI", 8.5F, FontStyle.Regular, GraphicsUnit.Point);
            gauge.Location = bounds.Location;
            gauge.Size = bounds.Size;
            gauge.NeedleColor2 = colorSurfaceAlt;
            gauge.NeedleRadius = 50;
            gauge.NeedleWidth = 3;
            gauge.ScaleLinesInterColor = colorBorder;
            gauge.ScaleLinesInterInnerRadius = 49;
            gauge.ScaleLinesInterOuterRadius = 55;
            gauge.ScaleLinesInterWidth = 2;
            gauge.ScaleLinesMajorColor = colorTextSecondary;
            gauge.ScaleLinesMajorInnerRadius = 44;
            gauge.ScaleLinesMajorOuterRadius = 55;
            gauge.ScaleLinesMajorWidth = 2;
            gauge.ScaleLinesMinorColor = colorBorder;
            gauge.ScaleLinesMinorInnerRadius = 49;
            gauge.ScaleLinesMinorOuterRadius = 55;
            gauge.ScaleNumbersColor = colorTextSecondary;
            gauge.ScaleNumbersRadius = 68;
        }

        private void UpdateGaugeScale(System.Windows.Forms.AGauge gauge, int speed)
        {
            while (gauge.MaxValue <= speed)
            {
                gauge.MaxValue += 50;
            }

            if (gauge.MaxValue > 1000)
            {
                gauge.ScaleLinesMajorStepValue = 250;
            }
            else if (gauge.MaxValue > 750)
            {
                gauge.ScaleLinesMajorStepValue = 200;
            }
            else if (gauge.MaxValue > 500)
            {
                gauge.ScaleLinesMajorStepValue = 150;
            }
            else if (gauge.MaxValue > 300)
            {
                gauge.ScaleLinesMajorStepValue = 100;
            }
            else if (gauge.MaxValue > 150)
            {
                gauge.ScaleLinesMajorStepValue = 50;
            }
            else
            {
                gauge.ScaleLinesMajorStepValue = 10;
            }
        }

        private bool CanRunSpeedTest()
        {
            return clientIP != status_NotAvailable && cb_SpeedTest_TestServer.Items.Count > 1;
        }

        public void SavePreferredSettings()
        {
            Properties.Settings.Default.SpeedTest_ServerScope = cb_SpeedTest_ServerScope.SelectedIndex;
            Properties.Settings.Default.SpeedTest_ValuesCalculation = cb_SpeedTest_Calculation.SelectedIndex;
        }

        public void RestorePreferredSettings()
        {
            cb_SpeedTest_ServerScope.SelectedIndex = Math.Max(0, Math.Min(Properties.Settings.Default.SpeedTest_ServerScope, cb_SpeedTest_ServerScope.Items.Count - 1));
            cb_SpeedTest_Calculation.SelectedIndex = Math.Max(0, Math.Min(Properties.Settings.Default.SpeedTest_ValuesCalculation, cb_SpeedTest_Calculation.Items.Count - 1));
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

            lbl_SpeedTest_HostedBy_Value.BackColor = colorSurfaceHighlight;
            lbl_SpeedTest_Distance_Value.BackColor = colorSurfaceHighlight;
            lbl_SpeedTest_Latency_Value.BackColor = GetColorByLatencyTime(targetServer.Latency);
        }

        public Server GetBestServerByLatency()
        {
            AppendTextToLogBox(
                                         rtb_SpeedTest_LogConsole,
                                         Environment.NewLine +
                                         "Selecting Best Server by Latency + Throughput ...",
                                         Color.Black,
                                         true);

            Server bestServer = testServersList
                .Where(server => serverQualifiedDownloadMbps.ContainsKey(GetServerIdentityKey(server)))
                .OrderByDescending(server => serverQualifiedDownloadMbps[GetServerIdentityKey(server)])
                .ThenBy(server => server.Latency)
                .FirstOrDefault()
                ?? testServersList.OrderBy(server => server.Latency).First();

            string bestServerKey = GetServerIdentityKey(bestServer);
            if (serverQualifiedDownloadMbps.TryGetValue(bestServerKey, out double qualifiedMbps))
            {
                AppendTextToLogBox(
                    rtb_SpeedTest_LogConsole,
                    "Qualified Throughput: " + qualifiedMbps.ToString("0") + " Mbps",
                    Color.Black,
                    true);
            }

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
                                           "Server Latency (" +
                                           +testTakesCount + " probes): " +
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

                    ThreadSafeInvoke(() =>
                    {
                        lbl_DownloadHint.Text = "Sampling sustained transfer rate with staged payload streams";
                        lbl_DownloadHint.ForeColor = colorInfo;
                    });

                    // TEST DOWNLOAD SPEED
                    int downloadSpeed = TestServerDownloadSpeed();

                    AppendTextToLogBox(
                            rtb_SpeedTest_LogConsole,
                                "Download Speed (single benchmark): " +
                                downloadSpeed + " Mbps",
                            Color.Black,
                            true);

                    // SET CONTROLS FOR DOWNLOAD
                    ThreadSafeInvoke(() =>
                    {
                        UpdateGaugeScale(aGauge_DownloadSpeed, downloadSpeed);

                        pBar_Download.Visible = false;
                        pBar_Download.Value = 0;
                        aGauge_DownloadSpeed.Value = Math.Max(0, Math.Min(downloadSpeed, (int)aGauge_DownloadSpeed.MaxValue));
                        lbl_SpeedTest_Mbps_Download_Label.BackColor = Color.FromArgb(22, 65, 56);
                        lbl_SpeedTest_Download_Label.ForeColor = colorSuccess;
                        lbl_DownloadHint.ForeColor = colorSuccess;
                        lbl_DownloadHint.Text = "Peak qualified downstream throughput";
                        aGauge_DownloadSpeed.NeedleColor1 = AGaugeNeedleColor.Green;
                        lbl_SpeedTest_Mbps_Download_Label.Text = downloadSpeed.ToString() + " Mbps";
                        AddSpeedHistorySample(downloadSpeedHistory, downloadSpeed, panel_DownloadTrend);

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

                    ThreadSafeInvoke(() =>
                    {
                        lbl_UploadHint.Text = "Measuring uplink consistency under multi-stream pressure";
                        lbl_UploadHint.ForeColor = colorInfo;
                    });

                    // TEST UPLOAD SPEED
                    int uploadSpeed = TestServerUploadSpeed();

                    AppendTextToLogBox(
                           rtb_SpeedTest_LogConsole,
                               "Upload Speed (single benchmark): " +
                               uploadSpeed + " Mbps",
                           Color.Black,
                           true);

                    // SET CONTROLS FOR UPLOAD
                    ThreadSafeInvoke(() =>
                    {
                        UpdateGaugeScale(aGauge_UploadSpeed, uploadSpeed);

                        pBar_Upload.Visible = false;
                        pBar_Upload.Value = 0;
                        aGauge_UploadSpeed.Value = Math.Max(0, Math.Min(uploadSpeed, (int)aGauge_UploadSpeed.MaxValue));
                        lbl_SpeedTest_Mbps_Upload_Label.BackColor = Color.FromArgb(72, 34, 44);
                        lbl_SpeedTest_Upload_Label.ForeColor = colorDanger;
                        lbl_UploadHint.ForeColor = colorDanger;
                        lbl_UploadHint.Text = "Peak qualified upstream throughput";
                        aGauge_UploadSpeed.NeedleColor1 = AGaugeNeedleColor.Red;
                        lbl_SpeedTest_Mbps_Upload_Label.Text = uploadSpeed.ToString() + " Mbps";
                        AddSpeedHistorySample(uploadSpeedHistory, uploadSpeed, panel_UploadTrend);

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
                        SetProgressState(false);
                    });
                }
            });
        }

        public int TestServerLatency()
        {
            int currentRetryCount = 0;
            int totalLatencyTime = 0;
            int bestLatencyTime = 1000000;

            for (int i = 1; i <= testTakesCount; i++)
            {
                try
                {
                    int currentLatencyTime = speedTestClient.TestServerLatency(targetServer);
                    totalLatencyTime += currentLatencyTime;

                    if (currentLatencyTime < bestLatencyTime)
                    {
                        bestLatencyTime = currentLatencyTime;
                    }

                    currentRetryCount = 0;

                    AppendTextToLogBox(
                           rtb_SpeedTest_LogConsole,
                               "Take " +
                               +i + " -> Latency (Ping Response Time): " +
                               currentLatencyTime + " ms",
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
                        throw;
                    }
                }
            }

            if (valuesCalculationMode == ValuesCalculationMode.BestValues)
            {
                return bestLatencyTime;
            }
            else
            {
                return totalLatencyTime / testTakesCount;
            }
        }

        public int TestServerDownloadSpeed()
        {
            int currentRetryCount = 0;

            ThreadSafeInvoke(() =>
            {
                pBar_Download.Visible = true;
                pBar_Download.Value = Math.Max(1, pBar_Download.Maximum / 5);
            });

            while (true)
            {
                try
                {
                    int currentDownloadSpeed = (int)Math.Round(
                        speedTestClient.TestDownloadSpeed(
                            targetServer,
                            DownloadBenchmarkConcurrency,
                            retryCount: 2,
                            progressCallback: currentSpeedKbps =>
                            {
                                int liveSpeedMbps = Math.Max(0, (int)Math.Round(currentSpeedKbps / 1024d, MidpointRounding.AwayFromZero));
                                ThreadSafeInvoke(() => UpdateLiveDownloadTelemetry(liveSpeedMbps));
                            }) / 1024,
                        2);

                    currentRetryCount = 0;

                    ThreadSafeInvoke(() =>
                    {
                        pBar_Download.Value = pBar_Download.Maximum;
                        Application.DoEvents();
                    });

                    AppendTextToLogBox(
                           rtb_SpeedTest_LogConsole,
                               "Benchmark -> Speed: " +
                               currentDownloadSpeed + " Mbps",
                           Color.DarkGray,
                           false);

                    ThreadSafeInvoke(() =>
                    {
                        AddSpeedHistorySample(downloadSpeedHistory, currentDownloadSpeed, panel_DownloadTrend);
                    });

                    Application.DoEvents();
                    return currentDownloadSpeed;
                }
                catch (Exception eX)
                {
                    if (currentRetryCount < testRetryCount)
                    {
                        AppendTextToLogBox(
                          rtb_SpeedTest_LogConsole,
                              "ERROR: " +
                              BuildExceptionMessage(eX),
                          Color.Red,
                          false);

                        currentRetryCount++;
                        ThreadSafeInvoke(() =>
                        {
                            pBar_Download.Value = Math.Min(pBar_Download.Maximum - 1, Math.Max(1, pBar_Download.Maximum / 5 + currentRetryCount * 10));
                        });
                        continue;
                    }

                    throw;
                }
            }
        }

        public int TestServerUploadSpeed()
        {
            int currentRetryCount = 0;

            ThreadSafeInvoke(() =>
            {
                pBar_Upload.Visible = true;
                pBar_Upload.Value = Math.Max(1, pBar_Upload.Maximum / 5);
            });

            while (true)
            {
                try
                {
                    int currentUploadSpeed = (int)Math.Round(
                        speedTestClient.TestUploadSpeed(
                            targetServer,
                            UploadBenchmarkConcurrency,
                            retryCount: 2,
                            progressCallback: currentSpeedKbps =>
                            {
                                int liveSpeedMbps = Math.Max(0, (int)Math.Round(currentSpeedKbps / 1024d, MidpointRounding.AwayFromZero));
                                ThreadSafeInvoke(() => UpdateLiveUploadTelemetry(liveSpeedMbps));
                            }) / 1024,
                        2);

                    currentRetryCount = 0;

                    ThreadSafeInvoke(() =>
                    {
                        pBar_Upload.Value = pBar_Upload.Maximum;
                        Application.DoEvents();
                    });

                    AppendTextToLogBox(
                           rtb_SpeedTest_LogConsole,
                               "Benchmark -> Speed: " +
                               currentUploadSpeed + " Mbps",
                           Color.DarkGray,
                           false);

                    ThreadSafeInvoke(() =>
                    {
                        AddSpeedHistorySample(uploadSpeedHistory, currentUploadSpeed, panel_UploadTrend);
                    });

                    Application.DoEvents();
                    return currentUploadSpeed;
                }
                catch (Exception eX)
                {
                    if (currentRetryCount < testRetryCount)
                    {
                        AppendTextToLogBox(
                          rtb_SpeedTest_LogConsole,
                              "ERROR:" +
                              BuildExceptionMessage(eX),
                          Color.Red,
                          false);

                        currentRetryCount++;
                        ThreadSafeInvoke(() =>
                        {
                            pBar_Upload.Value = Math.Min(pBar_Upload.Maximum - 1, Math.Max(1, pBar_Upload.Maximum / 5 + currentRetryCount * 10));
                        });
                        continue;
                    }

                    throw;
                }
            }
        }

        public IEnumerable<Server> GetServers()
        {
            if (speedTestSettings == null || speedTestSettings.Servers == null || speedTestSettings.Servers.Count == 0)
            {
                speedTestSettings = speedTestClient.GetSettings();
            }

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
                        filteredServersList.Count < maxTestServersCount)
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
                        filteredServersList.Count < maxTestServersCount))
                    {
                        filteredServersList.Add(serverItem);
                    }
                }
            }
            else if (testServerSelectionMode == TestServerSelectionMode.AllServers)
            {
                filteredServersList = serversList.Take(maxTestServersCount).ToList();
            }

            List<Server> candidateServers = filteredServersList
                .OrderBy(server => server.Distance)
                .Take(maxLatencyProbeCandidates)
                .ToList();

            AppendTextToLogBox(
                        rtb_SpeedTest_LogConsole,
                            "Probing nearest " + candidateServers.Count + " server candidates for latency ...",
                        Color.Black,
                        true);

            Parallel.ForEach(candidateServers, new ParallelOptions { MaxDegreeOfParallelism = 4 }, server =>
            {
                int bestLatency = int.MaxValue;

                for (int i = 0; i < 2; i++)
                {
                    try
                    {
                        int currentLatency = speedTestClient.TestServerLatency(server, 1);
                        if (currentLatency < bestLatency)
                        {
                            bestLatency = currentLatency;
                        }
                    }
                    catch
                    {
                    }

                    if (i == 0)
                    {
                        Thread.Sleep(75);
                    }
                }

                server.Latency = bestLatency == int.MaxValue ? 9999 : bestLatency;
            });

            List<Server> orderedServers = candidateServers
                .Where(server => server.Latency < 9999)
                .OrderBy(server => server.Latency)
                .ToList();

            serverQualifiedDownloadMbps.Clear();

            List<Server> throughputCandidates = orderedServers
                .Take(ThroughputQualificationCandidates)
                .ToList();

            if (throughputCandidates.Count > 0)
            {
                AppendTextToLogBox(
                            rtb_SpeedTest_LogConsole,
                                Environment.NewLine +
                                "Running throughput qualification on top " + throughputCandidates.Count + " nearby servers ...",
                            Color.Black,
                            true);

                Parallel.ForEach(throughputCandidates, new ParallelOptions { MaxDegreeOfParallelism = 2 }, server =>
                {
                    try
                    {
                        double speedKbps = speedTestClient.TestDownloadSpeed(
                            server,
                            ThroughputQualificationConcurrency,
                            retryCount: 2,
                            warmupDuration: ThroughputQualificationWarmup,
                            benchmarkDuration: ThroughputQualificationDuration);

                        double speedMbps = Math.Round(speedKbps / 1024d, 2);
                        serverQualifiedDownloadMbps[GetServerIdentityKey(server)] = speedMbps;
                    }
                    catch
                    {
                        serverQualifiedDownloadMbps[GetServerIdentityKey(server)] = 0;
                    }
                });

                foreach (Server server in throughputCandidates.OrderByDescending(server => serverQualifiedDownloadMbps.GetValueOrDefault(GetServerIdentityKey(server))))
                {
                    string serverKey = GetServerIdentityKey(server);
                    if (serverQualifiedDownloadMbps.TryGetValue(serverKey, out double speedMbps) && speedMbps > 0)
                    {
                        AppendTextToLogBox(
                            rtb_SpeedTest_LogConsole,
                            "Qualification -> " +
                            GetStringCorrectEncoding(server.Sponsor) +
                            " (" +
                            GetStringCorrectEncoding(server.Name) +
                            "/" +
                            GetStringCorrectEncoding(server.Country) +
                            "): " + speedMbps.ToString("0") + " Mbps",
                            Color.DarkGray,
                            false);
                    }
                }

                List<Server> reorderedByQualification = orderedServers
                    .OrderByDescending(server => serverQualifiedDownloadMbps.GetValueOrDefault(GetServerIdentityKey(server)))
                    .ThenBy(server => server.Latency)
                    .ToList();

                if (reorderedByQualification.Count > 0 && serverQualifiedDownloadMbps.GetValueOrDefault(GetServerIdentityKey(reorderedByQualification[0])) > 0)
                {
                    orderedServers = reorderedByQualification;
                }
            }

            foreach (Server server in orderedServers)
            {
                ListSelectedServerDetails(server);
            }

            return orderedServers;
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
                logBox.SelectionColor = NormalizeLogColor(textColor);

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

        private static string FirstNonEmpty(params string[] values)
        {
            foreach (string value in values)
            {
                if (!string.IsNullOrWhiteSpace(value) &&
                    !value.Equals(status_NotAvailable, StringComparison.OrdinalIgnoreCase))
                {
                    return value.Trim();
                }
            }

            return status_NotAvailable;
        }

        private static string NormalizeOrganizationName(string organization)
        {
            if (string.IsNullOrWhiteSpace(organization))
            {
                return string.Empty;
            }

            string normalized = organization.Trim();
            if (!normalized.StartsWith("AS", StringComparison.OrdinalIgnoreCase))
            {
                return normalized;
            }

            int firstWhitespace = normalized.IndexOf(' ');
            if (firstWhitespace <= 2)
            {
                return normalized;
            }

            bool hasAsnPrefix = true;
            for (int i = 2; i < firstWhitespace; i++)
            {
                if (!char.IsDigit(normalized[i]))
                {
                    hasAsnPrefix = false;
                    break;
                }
            }

            return hasAsnPrefix ? normalized.Substring(firstWhitespace + 1).Trim() : normalized;
        }

        private string ResolveBestIspProvider(PublicIdentityResponse publicIdentity)
        {
            string speedTestIsp = speedTestSettings?.Client?.Isp;
            string ipWhoIsIsp = publicIdentity?.Isp;
            string ipWhoIsOrg = NormalizeOrganizationName(publicIdentity?.Organization);

            return FirstNonEmpty(speedTestIsp, ipWhoIsIsp, ipWhoIsOrg, clientISP);
        }

        private PublicIdentityResponse ResolvePublicIdentity()
        {
            PublicIdentityResponse fallbackIdentity = new PublicIdentityResponse
            {
                Ip = speedTestSettings?.Client?.Ip ?? status_NotAvailable,
                Isp = FirstNonEmpty(speedTestSettings?.Client?.Isp, status_NotAvailable),
            };

            string preferredIp = ResolvePublicIpAddress();

            try
            {
                string identityUrl = string.IsNullOrWhiteSpace(preferredIp)
                    ? "https://ipwho.is/"
                    : "https://ipwho.is/" + preferredIp;

                string response = new CustomWebClient().DownloadString(identityUrl);
                PublicIdentityResponse publicIdentityResponse = JsonConvert.DeserializeObject<PublicIdentityResponse>(response);
                if (!string.IsNullOrWhiteSpace(publicIdentityResponse?.Ip))
                {
                    publicIdentityResponse.Ip = string.IsNullOrWhiteSpace(preferredIp)
                        ? publicIdentityResponse.Ip.Trim()
                        : preferredIp;

                    publicIdentityResponse.Isp = FirstNonEmpty(publicIdentityResponse.Isp, NormalizeOrganizationName(publicIdentityResponse.Organization), fallbackIdentity.Isp);

                    return publicIdentityResponse;
                }
            }
            catch
            {
            }

            if (!string.IsNullOrWhiteSpace(preferredIp))
            {
                fallbackIdentity.Ip = preferredIp;
            }

            return fallbackIdentity;
        }

        private string ResolvePublicIpAddress()
        {
            if (!string.IsNullOrWhiteSpace(resolvedPublicIp))
            {
                return resolvedPublicIp;
            }

            try
            {
                string ipifyResponse = new CustomWebClient().DownloadString("https://api64.ipify.org?format=json");
                IpifyResponse ipInfoResponse = JsonConvert.DeserializeObject<IpifyResponse>(ipifyResponse);
                if (!string.IsNullOrWhiteSpace(ipInfoResponse?.Ip))
                {
                    resolvedPublicIp = ipInfoResponse.Ip.Trim();
                    return resolvedPublicIp;
                }
            }
            catch
            {
            }

            resolvedPublicIp = speedTestSettings?.Client?.Ip?.Trim() ?? string.Empty;
            return resolvedPublicIp;
        }

        private void SetUserLocationDisplay(Color backColor)
        {
            ThreadSafeInvoke(() =>
            {
                lbl_SpeedTest_CurrentCountry_Value.BackColor = backColor;
                lbl_SpeedTest_CurrentCountry_Value.Text = FormatGeoLocationChipText(ipInfo);
            });
        }

        private void UpdatePublicIdentityDisplay()
        {
            ThreadSafeInvoke(() =>
            {
                lbl_SpeedTest_ExternalIP_Value.BackColor = colorSuccessSurface;
                lbl_SpeedTest_ExternalIP_Value.Text = clientIP + " (" + clientISP + ")";
            });
        }

        private static string GetStringOrDefault(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? status_NotAvailable : value.Trim();
        }

        private string FormatGeoLocationChipText(SpeedTestGeoInfo geoInfo)
        {
            if (geoInfo == null)
            {
                return status_NotAvailable;
            }

            string city = GetStringOrDefault(geoInfo.City);
            string region = GetStringOrDefault(geoInfo.Region_Name);
            string country = GetStringOrDefault(geoInfo.Country_Name);

            if (region != status_NotAvailable && !region.Equals(city, StringComparison.OrdinalIgnoreCase))
            {
                return GetStringCorrectEncoding(city) + ", " + GetStringCorrectEncoding(region) + " / " + GetStringCorrectEncoding(country);
            }

            return GetStringCorrectEncoding(city) + " / " + GetStringCorrectEncoding(country);
        }

        private void LogResolvedGeoInfo(string sourceName)
        {
            AppendTextToLogBox(
                        rtb_SpeedTest_LogConsole,
                            "Geo Source: " + sourceName + Environment.NewLine +
                            "Country: " + GetStringCorrectEncoding(GetStringOrDefault(ipInfo.Country_Name)) + Environment.NewLine +
                            "Country Code: " + GetStringCorrectEncoding(GetStringOrDefault(ipInfo.Country_Code)) + Environment.NewLine +
                            "Region: " + GetStringCorrectEncoding(GetStringOrDefault(ipInfo.Region_Code)) + Environment.NewLine +
                            "Region Name: " + GetStringCorrectEncoding(GetStringOrDefault(ipInfo.Region_Name)) + Environment.NewLine +
                            "City: " + GetStringCorrectEncoding(GetStringOrDefault(ipInfo.City)) + Environment.NewLine +
                            "ZIP Code: " + GetStringCorrectEncoding(GetStringOrDefault(ipInfo.City_ZIP_Code)) + Environment.NewLine +
                            "GEO Latitude: " + GetStringCorrectEncoding(GetStringOrDefault(ipInfo.Geo_Lat)) + Environment.NewLine +
                            "GEO Longitude: " + GetStringCorrectEncoding(GetStringOrDefault(ipInfo.Geo_Lon)) + Environment.NewLine +
                            "Time Zone: " + GetStringCorrectEncoding(GetStringOrDefault(ipInfo.TimeZone)) + Environment.NewLine +
                            "ISP Organization: " + GetStringCorrectEncoding(GetStringOrDefault(ipInfo.ISP_ORG)) + Environment.NewLine +
                            "ASN: " + GetStringCorrectEncoding(GetStringOrDefault(ipInfo.ISP_AS)) + Environment.NewLine,
                        Color.Yellow,
                        true);
        }

        private bool TryResolveGeoInfoFromIpWhoIs(string ipAddress, out SpeedTestGeoInfo resolvedGeoInfo)
        {
            resolvedGeoInfo = null;

            PublicIdentityResponse publicIdentity = cachedPublicIdentity ?? ResolvePublicIdentity();
            if (publicIdentity == null ||
                string.IsNullOrWhiteSpace(publicIdentity.Country) ||
                string.IsNullOrWhiteSpace(publicIdentity.City))
            {
                try
                {
                    string lookupUrl = string.IsNullOrWhiteSpace(ipAddress)
                        ? "https://ipwho.is/"
                        : "https://ipwho.is/" + ipAddress;

                    string response = new CustomWebClient().DownloadString(lookupUrl);
                    publicIdentity = JsonConvert.DeserializeObject<PublicIdentityResponse>(response);
                }
                catch
                {
                    return false;
                }

                if (publicIdentity == null ||
                    string.IsNullOrWhiteSpace(publicIdentity.Country) ||
                    string.IsNullOrWhiteSpace(publicIdentity.City))
                {
                    return false;
                }
            }

            resolvedGeoInfo = new SpeedTestGeoInfo
            {
                Country_Name = publicIdentity.Country,
                Country_Code = publicIdentity.CountryCode,
                Region_Code = publicIdentity.RegionCode,
                Region_Name = publicIdentity.Region,
                City = publicIdentity.City,
                City_ZIP_Code = publicIdentity.Postal,
                Geo_Lat = publicIdentity.Latitude,
                Geo_Lon = publicIdentity.Longitude,
                TimeZone = publicIdentity.TimeZone,
                ISP_ORG = FirstNonEmpty(publicIdentity.Isp, NormalizeOrganizationName(publicIdentity.Organization)),
                ISP_AS = publicIdentity.Asn,
            };

            if (string.IsNullOrWhiteSpace(resolvedGeoInfo.ISP_ORG))
            {
                resolvedGeoInfo.ISP_ORG = ResolveBestIspProvider(publicIdentity);
            }

            return !string.IsNullOrWhiteSpace(resolvedGeoInfo.Country_Name);
        }

        private bool TryResolveGeoInfoFromIpApi(string ipAddress, out SpeedTestGeoInfo resolvedGeoInfo)
        {
            resolvedGeoInfo = null;

            string endpoint = string.IsNullOrWhiteSpace(ipAddress)
                ? "https://ipapi.co/json/"
                : "https://ipapi.co/" + ipAddress + "/json/";

            string info = new CustomWebClient().DownloadString(endpoint);
            SpeedTestGeoInfo ipApiGeoInfo = JsonConvert.DeserializeObject<SpeedTestGeoInfo>(info);

            if (ipApiGeoInfo == null)
            {
                return false;
            }

            if (ipApiGeoInfo.Error)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(ipApiGeoInfo.Country_Name))
            {
                return false;
            }

            resolvedGeoInfo = ipApiGeoInfo;
            if (string.IsNullOrWhiteSpace(resolvedGeoInfo.ISP_ORG))
            {
                resolvedGeoInfo.ISP_ORG = ResolveBestIspProvider(cachedPublicIdentity);
            }

            return true;
        }

        private bool TryResolveGeoInfoFromIpApiCom(string ipAddress, out SpeedTestGeoInfo resolvedGeoInfo)
        {
            resolvedGeoInfo = null;

            string endpoint = "http://ip-api.com/json/";
            if (!string.IsNullOrWhiteSpace(ipAddress))
            {
                endpoint += ipAddress;
            }

            endpoint += "?fields=status,country,countryCode,region,regionName,city,zip,lat,lon,timezone,isp,org,as,query";

            string info = new CustomWebClient().DownloadString(endpoint);
            IpApiComGeoResponse ipApiComGeoInfo = JsonConvert.DeserializeObject<IpApiComGeoResponse>(info);
            if (ipApiComGeoInfo == null ||
                !"success".Equals(ipApiComGeoInfo.Status, StringComparison.OrdinalIgnoreCase) ||
                string.IsNullOrWhiteSpace(ipApiComGeoInfo.Country))
            {
                return false;
            }

            resolvedGeoInfo = new SpeedTestGeoInfo
            {
                Country_Name = ipApiComGeoInfo.Country,
                Country_Code = ipApiComGeoInfo.CountryCode,
                Region_Code = ipApiComGeoInfo.Region,
                Region_Name = ipApiComGeoInfo.RegionName,
                City = ipApiComGeoInfo.City,
                City_ZIP_Code = ipApiComGeoInfo.Zip,
                Geo_Lat = ipApiComGeoInfo.Lat.ToString(System.Globalization.CultureInfo.InvariantCulture),
                Geo_Lon = ipApiComGeoInfo.Lon.ToString(System.Globalization.CultureInfo.InvariantCulture),
                TimeZone = ipApiComGeoInfo.Timezone,
                ISP_ORG = FirstNonEmpty(ipApiComGeoInfo.Isp, NormalizeOrganizationName(ipApiComGeoInfo.Org)),
                ISP_AS = ipApiComGeoInfo.As,
            };

            return true;
        }

        private void HarmonizeGeoInfoWithNearestServer()
        {
            if (ipInfo == null || speedTestSettings?.Servers == null || speedTestSettings.Servers.Count == 0)
            {
                return;
            }

            if (!TryParseGeoCoordinates(ipInfo, out double geoLat, out double geoLon))
            {
                return;
            }

            Server nearestServer = speedTestSettings.Servers
                .OrderBy(server => server.Distance)
                .FirstOrDefault();

            if (nearestServer == null ||
                string.IsNullOrWhiteSpace(nearestServer.Country) ||
                string.IsNullOrWhiteSpace(ipInfo.Country_Name) ||
                !nearestServer.Country.Equals(ipInfo.Country_Name, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            double distanceKm = CalculateDistanceKm(geoLat, geoLon, nearestServer.Latitude, nearestServer.Longitude);
            if (distanceKm <= 85 && !string.IsNullOrWhiteSpace(nearestServer.Name) &&
                !nearestServer.Name.Equals(ipInfo.City, StringComparison.OrdinalIgnoreCase))
            {
                AppendTextToLogBox(
                    rtb_SpeedTest_LogConsole,
                    "Geo harmonization: using nearby metro label '" + GetStringCorrectEncoding(nearestServer.Name) +
                    "' (distance " + distanceKm.ToString("0") + " km from IP geolocation).",
                    colorInfo,
                    true);

                ipInfo.City = nearestServer.Name;
            }
        }

        private static bool TryParseGeoCoordinates(SpeedTestGeoInfo geoInfo, out double latitude, out double longitude)
        {
            bool latOk = double.TryParse(geoInfo?.Geo_Lat, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out latitude);
            bool lonOk = double.TryParse(geoInfo?.Geo_Lon, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out longitude);
            return latOk && lonOk;
        }

        private static double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)
        {
            const double earthRadiusKm = 6371d;
            double dLat = DegreesToRadians(lat2 - lat1);
            double dLon = DegreesToRadians(lon2 - lon1);

            double a = Math.Sin(dLat / 2d) * Math.Sin(dLat / 2d) +
                       Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                       Math.Sin(dLon / 2d) * Math.Sin(dLon / 2d);
            double c = 2d * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1d - a));
            return earthRadiusKm * c;
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180d);
        }

        private SpeedTestGeoInfo BuildFallbackGeoInfoFromNearestServer()
        {
            if (speedTestSettings == null || speedTestSettings.Servers == null || speedTestSettings.Servers.Count == 0)
            {
                speedTestSettings = speedTestClient.GetSettings();
            }

            Server nearestServer = speedTestSettings.Servers
                .OrderBy(server => server.Distance)
                .FirstOrDefault();

            if (nearestServer == null)
            {
                return null;
            }

            return new SpeedTestGeoInfo
            {
                Country_Name = nearestServer.Country,
                Country_Code = string.Empty,
                Region_Code = string.Empty,
                Region_Name = string.Empty,
                City = nearestServer.Name,
                City_ZIP_Code = string.Empty,
                Geo_Lat = speedTestSettings.Client.Latitude.ToString(),
                Geo_Lon = speedTestSettings.Client.Longitude.ToString(),
                TimeZone = string.Empty,
                ISP_ORG = clientISP,
                ISP_AS = string.Empty,
            };
        }

        private void ApplyFallbackGeoInfo(Exception sourceException)
        {
            ipInfo = BuildFallbackGeoInfoFromNearestServer();

            if (ipInfo == null)
            {
                throw new InvalidOperationException("Unable to derive fallback location from SpeedTest server data.", sourceException);
            }

            AppendTextToLogBox(
                        rtb_SpeedTest_LogConsole,
                            "GeoLocation API unavailable. Falling back to nearest SpeedTest server metadata." +
                            Environment.NewLine +
                            "Approximate City: " + GetStringCorrectEncoding(ipInfo.City) + Environment.NewLine +
                            "Approximate Country: " + GetStringCorrectEncoding(ipInfo.Country_Name) + Environment.NewLine,
                        colorWarning,
                        true);

            SetUserLocationDisplay(colorSuccessSurface);
        }

        public void GetUserCountry()
        {
            try
            {
                string preferredIp = ResolvePublicIpAddress();

                AppendTextToLogBox(
                            rtb_SpeedTest_LogConsole,
                                "Resolving GeoLocation Info for IP '" + (string.IsNullOrWhiteSpace(preferredIp) ? status_NotAvailable : preferredIp) +
                                "' [ip-api.com primary, ipapi.co secondary, ipwho.is fallback] ...",
                            Color.Black,
                            true);

                if (TryResolveGeoInfoFromIpApiCom(preferredIp, out SpeedTestGeoInfo ipApiComGeoInfo))
                {
                    ipInfo = ipApiComGeoInfo;
                    clientISP = FirstNonEmpty(ipInfo.ISP_ORG, clientISP);
                    UpdatePublicIdentityDisplay();
                    SetUserLocationDisplay(colorSuccessSurface);
                    LogResolvedGeoInfo("ip-api.com");
                    return;
                }

                if (TryResolveGeoInfoFromIpApi(preferredIp, out SpeedTestGeoInfo ipApiGeoInfo))
                {
                    ipInfo = ipApiGeoInfo;
                    clientISP = FirstNonEmpty(ipInfo.ISP_ORG, clientISP);
                    UpdatePublicIdentityDisplay();
                    SetUserLocationDisplay(colorWarningSurface);
                    LogResolvedGeoInfo("ipapi.co");
                    return;
                }

                if (TryResolveGeoInfoFromIpWhoIs(preferredIp, out SpeedTestGeoInfo ipWhoIsGeoInfo))
                {
                    ipInfo = ipWhoIsGeoInfo;
                    clientISP = FirstNonEmpty(ipInfo.ISP_ORG, clientISP);
                    UpdatePublicIdentityDisplay();
                    SetUserLocationDisplay(colorWarningSurface);
                    LogResolvedGeoInfo("ipwho.is");
                    return;
                }

                throw new Exception("No GeoLocation provider returned a usable location payload.");
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

                try
                {
                    ApplyFallbackGeoInfo(exception);
                }
                catch (Exception fallbackException)
                {
                    AppendTextToLogBox(
                                rtb_SpeedTest_LogConsole,
                                    "Fallback location resolution failed:" +
                                    BuildExceptionMessage(fallbackException) +
                                    Environment.NewLine,
                                Color.Red,
                                true);

                    ThreadSafeInvoke(() =>
                    {
                        SetProgressState(false);
                    });
                }
            }
        }

        public void NewBackgroundThread(Action action)
        {
            UiThreadHelpers.StartBackgroundThread(action, Application.DoEvents);
        }

        public void ThreadSafeInvoke(Action action)
        {
            UiThreadHelpers.SafeInvoke(() => Invoke(action), Application.DoEvents);
        }

        public void SpeedTestDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = pb_SpeedTestProgress.Visible;

            motionPulseTimer.Stop();
            revealTimer.Stop();
            startupGaugeSweepTimer.Stop();

            SavePreferredSettings();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void btn_SpeedTest_GetServers_Click(object sender, EventArgs e)
        {
            SetControlsCleanState();

            SetAGaugeControlsCleanState();

            SetProgressState(true);

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
                            AppendTextToLogBox(
                                                         rtb_SpeedTest_LogConsole,
                                                             "There are " + servers.Count() +
                                                             " unique Test Server(s) available" +
                                                             Environment.NewLine +
                                                             "Server Scope: " +
                                                             GetEnumDescriptionString(testServerSelectionMode),
                                                         Color.LimeGreen,
                                                         true);

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
                                                             "Not any Test Server available" +
                                                             Environment.NewLine +
                                                             "Server Scope: " +
                                                             GetEnumDescriptionString(testServerSelectionMode) +
                                                             Environment.NewLine,
                                                         Color.Red,
                                                         true);
                        }
                    }
                    catch (Exception exception)
                    {
                        SetProgressState(false);

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
                    SetProgressState(false);
                });
            });
        }

        public void SetControlsCleanState()
        {
            testServersList.Clear();
            serverQualifiedDownloadMbps.Clear();
            downloadSpeedHistory.Clear();
            uploadSpeedHistory.Clear();

            panel_DownloadTrend?.SetSamples(Array.Empty<double>());
            panel_UploadTrend?.SetSamples(Array.Empty<double>());

            rtb_SpeedTest_LogConsole.Text = string.Empty;

            lbl_SpeedTest_CurrentCountry_Value.Text = status_NotAvailable;
            lbl_SpeedTest_ExternalIP_Value.Text = clientIP == status_NotAvailable ? status_NotAvailable : clientIP + " (" + clientISP + ")";
            lbl_SpeedTest_HostedBy_Value.Text = status_NotAvailable;
            lbl_SpeedTest_Distance_Value.Text = status_NotAvailable;
            lbl_SpeedTest_Latency_Value.Text = status_NotAvailable;

            cb_SpeedTest_TestServer.Items.Clear();
            cb_SpeedTest_TestServer.Items.Add("Using Best Server (latency + throughput)");

            lbl_SpeedTest_ExternalIP_Value.BackColor = colorSuccessSurface;
            lbl_SpeedTest_CurrentCountry_Value.BackColor = colorSurfaceAlt;
            cb_SpeedTest_TestServer.BackColor = colorInput;
            lbl_SpeedTest_HostedBy_Value.BackColor = colorSurfaceAlt;
            lbl_SpeedTest_Distance_Value.BackColor = colorSurfaceAlt;
            lbl_SpeedTest_Latency_Value.BackColor = colorSurfaceAlt;
        }

        public void SetAGaugeControlsCleanState()
        {
            lastDownloadTrendUpdateUtc = DateTime.MinValue;
            lastUploadTrendUpdateUtc = DateTime.MinValue;
            lastDownloadTrendValue = -1;
            lastUploadTrendValue = -1;

            pBar_Download.Visible = false;
            pBar_Download.Value = 0;

            pBar_Upload.Visible = false;
            pBar_Upload.Value = 0;

            lbl_SpeedTest_Mbps_Download_Label.Text = status_NotAvailable;
            lbl_SpeedTest_Mbps_Upload_Label.Text = status_NotAvailable;

            lbl_SpeedTest_Mbps_Download_Label.BackColor = colorSurfaceAlt;
            lbl_SpeedTest_Mbps_Upload_Label.BackColor = colorSurfaceAlt;
            lbl_SpeedTest_Download_Label.ForeColor = colorTextPrimary;
            lbl_SpeedTest_Upload_Label.ForeColor = colorTextPrimary;

            aGauge_DownloadSpeed.NeedleColor1 = AGaugeNeedleColor.Gray;
            aGauge_UploadSpeed.NeedleColor1 = AGaugeNeedleColor.Gray;

            aGauge_DownloadSpeed.Value = 0;
            aGauge_UploadSpeed.Value = 0;

            aGauge_DownloadSpeed.MaxValue = 50;
            aGauge_UploadSpeed.MaxValue = 50;

            aGauge_DownloadSpeed.ScaleLinesMajorStepValue = 10;
            aGauge_UploadSpeed.ScaleLinesMajorStepValue = 10;

            lbl_DownloadHint.ForeColor = colorTextSecondary;
            lbl_DownloadHint.Text = "Observed throughput across repeated test passes";
            lbl_UploadHint.ForeColor = colorTextSecondary;
            lbl_UploadHint.Text = "Measured against selected host using multi-pass upload";
        }

        private void UpdateLiveDownloadTelemetry(int liveSpeedMbps)
        {
            UpdateGaugeScale(aGauge_DownloadSpeed, liveSpeedMbps);
            aGauge_DownloadSpeed.NeedleColor1 = AGaugeNeedleColor.Green;
            aGauge_DownloadSpeed.Value = Math.Max(0, Math.Min(liveSpeedMbps, (int)aGauge_DownloadSpeed.MaxValue));

            lbl_SpeedTest_Mbps_Download_Label.BackColor = Color.FromArgb(22, 65, 56);
            lbl_SpeedTest_Mbps_Download_Label.Text = liveSpeedMbps + " Mbps";

            int nextProgress = pBar_Download.Value + 4;
            pBar_Download.Value = nextProgress >= pBar_Download.Maximum ? Math.Max(1, pBar_Download.Maximum / 4) : nextProgress;

            if (liveSpeedMbps <= 0)
            {
                return;
            }

            DateTime now = DateTime.UtcNow;
            if ((now - lastDownloadTrendUpdateUtc).TotalMilliseconds < 420 &&
                Math.Abs(lastDownloadTrendValue - liveSpeedMbps) < 3)
            {
                return;
            }

            lastDownloadTrendUpdateUtc = now;
            lastDownloadTrendValue = liveSpeedMbps;
            AddSpeedHistorySample(downloadSpeedHistory, liveSpeedMbps, panel_DownloadTrend);
        }

        private void UpdateLiveUploadTelemetry(int liveSpeedMbps)
        {
            UpdateGaugeScale(aGauge_UploadSpeed, liveSpeedMbps);
            aGauge_UploadSpeed.NeedleColor1 = AGaugeNeedleColor.Red;
            aGauge_UploadSpeed.Value = Math.Max(0, Math.Min(liveSpeedMbps, (int)aGauge_UploadSpeed.MaxValue));

            lbl_SpeedTest_Mbps_Upload_Label.BackColor = Color.FromArgb(72, 34, 44);
            lbl_SpeedTest_Mbps_Upload_Label.Text = liveSpeedMbps + " Mbps";

            int nextProgress = pBar_Upload.Value + 4;
            pBar_Upload.Value = nextProgress >= pBar_Upload.Maximum ? Math.Max(1, pBar_Upload.Maximum / 4) : nextProgress;

            if (liveSpeedMbps <= 0)
            {
                return;
            }

            DateTime now = DateTime.UtcNow;
            if ((now - lastUploadTrendUpdateUtc).TotalMilliseconds < 420 &&
                Math.Abs(lastUploadTrendValue - liveSpeedMbps) < 2)
            {
                return;
            }

            lastUploadTrendUpdateUtc = now;
            lastUploadTrendValue = liveSpeedMbps;
            AddSpeedHistorySample(uploadSpeedHistory, liveSpeedMbps, panel_UploadTrend);
        }

        private static void AddSpeedHistorySample(List<int> history, int value, SparklinePanel panel)
        {
            if (history == null || panel == null)
            {
                return;
            }

            int safeValue = Math.Max(0, value);
            history.Add(safeValue);

            while (history.Count > 18)
            {
                history.RemoveAt(0);
            }

            panel.SetSamples(BuildAnimatedTrendSeries(history));
        }

        private static IEnumerable<double> BuildAnimatedTrendSeries(IReadOnlyList<int> checkpoints)
        {
            if (checkpoints == null || checkpoints.Count == 0)
            {
                return Array.Empty<double>();
            }

            if (checkpoints.Count == 1)
            {
                double value = Math.Max(1, checkpoints[0]);
                return new[] { value * 0.22, value * 0.47, value * 0.73, value };
            }

            List<double> points = new List<double>(checkpoints.Count * 7);
            points.Add(Math.Max(1, checkpoints[0]) * 0.32);
            points.Add(Math.Max(1, checkpoints[0]));

            for (int i = 1; i < checkpoints.Count; i++)
            {
                double previous = checkpoints[i - 1];
                double current = checkpoints[i];
                const int segmentSteps = 5;

                for (int step = 1; step <= segmentSteps; step++)
                {
                    double t = step / (double)segmentSteps;
                    double eased = t * t * (3d - 2d * t);
                    double wave = Math.Sin((i * segmentSteps + step) * 0.9d) * Math.Max(0.7d, Math.Abs(current - previous) * 0.03d);
                    double point = previous + ((current - previous) * eased) + wave;
                    points.Add(Math.Max(0d, point));
                }
            }

            return points;
        }

        public void SetProgressState(bool inProgress)
        {
            ThreadSafeInvoke(() =>
            {
                cb_SpeedTest_ServerScope.Enabled = !inProgress;
                cb_SpeedTest_Calculation.Enabled = !inProgress;
                cb_SpeedTest_TestServer.Enabled = !inProgress && testServersList.Count > 1;
                UpdateActionButtonsAndStatus(inProgress);
                pb_SpeedTestProgress.Visible = inProgress;
                pb_SpeedTestProgress.BringToFront();
                btn_SpeedTest_GetServers.BringToFront();

                pb_GO.Visible = false;
            });
        }

        private void UpdateActionButtonsAndStatus(bool inProgress)
        {
            bool canRunSpeedTest = CanRunSpeedTest();

            btn_SpeedTest_GetServers.Visible = !inProgress;
            btn_SpeedTest_GetServers.Enabled = !inProgress;

            if (btn_RunSpeedTest != null)
            {
                btn_RunSpeedTest.Visible = canRunSpeedTest && !inProgress;
                btn_RunSpeedTest.Enabled = canRunSpeedTest && !inProgress;
                btn_RunSpeedTest.BackColor = canRunSpeedTest && !inProgress ? colorAccent : colorSurfaceAlt;
                btn_RunSpeedTest.FlatAppearance.BorderColor = canRunSpeedTest && !inProgress ? colorAccent : colorBorder;
                btn_RunSpeedTest.ForeColor = canRunSpeedTest && !inProgress ? Color.White : colorTextSecondary;
                btn_RunSpeedTest.BringToFront();
            }

            if (lbl_HeaderStatus != null)
            {
                if (inProgress)
                {
                    lbl_HeaderStatus.Text = "LIVE";
                    lbl_HeaderStatus.BackColor = colorWarningSurface;
                    lbl_HeaderStatus.ForeColor = colorWarning;
                }
                else if (canRunSpeedTest)
                {
                    lbl_HeaderStatus.Text = "READY";
                    lbl_HeaderStatus.BackColor = colorSuccessSurface;
                    lbl_HeaderStatus.ForeColor = colorSuccess;
                }
                else
                {
                    lbl_HeaderStatus.Text = "INIT";
                    lbl_HeaderStatus.BackColor = colorInput;
                    lbl_HeaderStatus.ForeColor = colorInfo;
                }
            }
        }

        private void AnimateGaugeToValue(System.Windows.Forms.AGauge gauge, int targetValue)
        {
            int startValue = (int)Math.Round(gauge.Value);
            int delta = targetValue - startValue;

            if (delta == 0)
            {
                return;
            }

            int steps = Math.Max(12, Math.Min(42, Math.Abs(delta) / 3));
            for (int i = 1; i <= steps; i++)
            {
                int nextValue = startValue + (delta * i / steps);
                gauge.Value = Math.Max(0, nextValue);
                Application.DoEvents();
                Thread.Sleep(GaugeAnimationDelayMs);
            }

            gauge.Value = Math.Max(0, targetValue);
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
            SetProgressState(true);
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

        public void Custom_ComboBox_DrawItem(object sender, DrawItemEventArgs e)
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
                    bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
                    using (Brush backgroundBrush = new SolidBrush(isSelected ? colorAccent : colorInput))
                    {
                        e.Graphics.FillRectangle(backgroundBrush, e.Bounds);
                    }

                    // Draw the string
                    using (Brush brush = new SolidBrush(colorTextPrimary))
                    {
                        e.Graphics.DrawString(cbx.Items[e.Index].ToString(), new Font("Segoe UI", 10, FontStyle.Regular), brush, e.Bounds, sf);
                    }
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
            return latencyTime <= 10
                ? Color.FromArgb(24, 82, 60)
                : latencyTime <= 20
                    ? Color.FromArgb(87, 66, 22)
                    : Color.FromArgb(92, 38, 50);
        }

        private static string GetServerIdentityKey(Server server)
        {
            if (server == null)
            {
                return string.Empty;
            }

            return (server.Url ?? string.Empty) + "|" +
                   (server.Sponsor ?? string.Empty) + "|" +
                   (server.Name ?? string.Empty) + "|" +
                   (server.Country ?? string.Empty);
        }

        public void cb_SpeedTest_ServerScope_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cb_SpeedTest_ServerScope.SelectedIndex == 0)
            {
                testServerSelectionMode = TestServerSelectionMode.AllServers;
            }
            else if (cb_SpeedTest_ServerScope.SelectedIndex == 1)
            {
                testServerSelectionMode = TestServerSelectionMode.AllServersExceptCurrentCountry;
            }
            else if (cb_SpeedTest_ServerScope.SelectedIndex == 2)
            {
                testServerSelectionMode = TestServerSelectionMode.OnlyServersFromCurrentCountry;
            }

            if (btn_SpeedTest_GetServers.Visible)
            {
                btn_SpeedTest_GetServers_Click(this, null);
            }
        }

        public void cb_SpeedTest_Calculation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cb_SpeedTest_Calculation.SelectedIndex == 0)
            {
                valuesCalculationMode = ValuesCalculationMode.BestValues;
            }
            else if (cb_SpeedTest_Calculation.SelectedIndex == 1)
            {
                valuesCalculationMode = ValuesCalculationMode.AverageValues;
            }
        }
    }

    public class SpeedTestGeoInfo
    {
        [JsonProperty("error")]
        public bool Error { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("country_name")]
        public string Country_Name { get; set; }

        [JsonProperty("country_code")]
        public string Country_Code { get; set; }

        [JsonProperty("region_code")]
        public string Region_Code { get; set; }

        [JsonProperty("region")]
        public string Region_Name { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("postal")]
        public string City_ZIP_Code { get; set; }

        [JsonProperty("latitude")]
        public string Geo_Lat { get; set; }

        [JsonProperty("longitude")]
        public string Geo_Lon { get; set; }

        [JsonProperty("timezone")]
        public string TimeZone { get; set; }

        [JsonProperty("org")]
        public string ISP_ORG { get; set; }

        [JsonProperty("asn")]
        public string ISP_AS { get; set; }
    }

    public class PublicIdentityResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; } = true;

        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty("connection")]
        public PublicConnectionResponse Connection { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("region_code")]
        public string RegionCode { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("postal")]
        public string Postal { get; set; }

        [JsonProperty("latitude")]
        public string Latitude { get; set; }

        [JsonProperty("longitude")]
        public string Longitude { get; set; }

        [JsonProperty("timezone")]
        public string TimeZone { get; set; }

        [JsonProperty("org")]
        public string Organization { get; set; }

        [JsonProperty("asn")]
        public string Asn { get; set; }

        public string Isp
        {
            get => Connection?.Isp;
            set
            {
                Connection ??= new PublicConnectionResponse();
                Connection.Isp = value;
            }
        }
    }

    public class PublicConnectionResponse
    {
        [JsonProperty("isp")]
        public string Isp { get; set; }
    }

    public class IpifyResponse
    {
        [JsonProperty("ip")]
        public string Ip { get; set; }
    }

    public class IpApiComGeoResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("regionName")]
        public string RegionName { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("zip")]
        public string Zip { get; set; }

        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lon")]
        public double Lon { get; set; }

        [JsonProperty("timezone")]
        public string Timezone { get; set; }

        [JsonProperty("isp")]
        public string Isp { get; set; }

        [JsonProperty("org")]
        public string Org { get; set; }

        [JsonProperty("as")]
        public string As { get; set; }
    }

    public class PremiumSurfacePanel : Panel
    {
        public Color FillColor { get; set; } = Color.FromArgb(20, 28, 48);
        public Color BorderColor { get; set; } = Color.FromArgb(52, 68, 110);

        public PremiumSurfacePanel()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Rectangle drawBounds = new Rectangle(0, 0, Width - 1, Height - 1);
            if (drawBounds.Width <= 0 || drawBounds.Height <= 0)
            {
                return;
            }

            using (LinearGradientBrush brush = new LinearGradientBrush(
                drawBounds,
                ControlPaint.Light(FillColor, 0.08F),
                FillColor,
                LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, drawBounds);
            }

            using (Pen borderPen = new Pen(BorderColor))
            {
                e.Graphics.DrawRectangle(borderPen, drawBounds);
            }
        }
    }

    public class ProgressBar_Green : ProgressBar
    {
        private readonly Color backgroundColor = Color.FromArgb(20, 33, 56);
        private readonly Color fillColor = Color.FromArgb(48, 196, 141);

        public ProgressBar_Green()
        {
            SetStyle(ControlStyles.UserPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle bounds = e.ClipRectangle;
            using (SolidBrush backgroundBrush = new SolidBrush(backgroundColor))
            {
                e.Graphics.FillRectangle(backgroundBrush, bounds);
            }

            int safeMaximum = Math.Max(1, Maximum);
            int fillWidth = Math.Max(0, (int)((bounds.Width - 4) * (double)Value / safeMaximum));
            Rectangle fillRect = new Rectangle(2, 2, fillWidth, Math.Max(1, bounds.Height - 4));

            using (SolidBrush fillBrush = new SolidBrush(fillColor))
            {
                e.Graphics.FillRectangle(fillBrush, fillRect);
            }
        }
    }

    public class ProgressBar_Red : ProgressBar
    {
        private readonly Color backgroundColor = Color.FromArgb(20, 33, 56);
        private readonly Color fillColor = Color.FromArgb(255, 107, 129);

        public ProgressBar_Red()
        {
            SetStyle(ControlStyles.UserPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle bounds = e.ClipRectangle;
            using (SolidBrush backgroundBrush = new SolidBrush(backgroundColor))
            {
                e.Graphics.FillRectangle(backgroundBrush, bounds);
            }

            int safeMaximum = Math.Max(1, Maximum);
            int fillWidth = Math.Max(0, (int)((bounds.Width - 4) * (double)Value / safeMaximum));
            Rectangle fillRect = new Rectangle(2, 2, fillWidth, Math.Max(1, bounds.Height - 4));

            using (SolidBrush fillBrush = new SolidBrush(fillColor))
            {
                e.Graphics.FillRectangle(fillBrush, fillRect);
            }
        }
    }

    public class SparklinePanel : Panel
    {
        private readonly List<double> samples = new List<double>();

        public Color LineColor { get; set; } = Color.FromArgb(76, 214, 145);
        public Color FillColor { get; set; } = Color.FromArgb(30, 88, 64);

        public SparklinePanel()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw, true);
        }

        public void SetSamples(IEnumerable<double> values)
        {
            samples.Clear();
            if (values != null)
            {
                samples.AddRange(values);
            }

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Rectangle canvas = new Rectangle(0, 0, Width - 1, Height - 1);
            if (canvas.Width <= 0 || canvas.Height <= 0)
            {
                return;
            }

            using (LinearGradientBrush backgroundBrush = new LinearGradientBrush(
                canvas,
                ControlPaint.Light(BackColor, 0.04F),
                BackColor,
                LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(backgroundBrush, canvas);
            }

            using (Pen border = new Pen(Color.FromArgb(58, 78, 122)))
            {
                e.Graphics.DrawRectangle(border, canvas);
            }

            if (samples.Count < 2)
            {
                return;
            }

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            double min = samples.Min();
            double max = samples.Max();
            double span = max - min;
            bool nearFlat = span < 0.5d;
            if (nearFlat)
            {
                double padding = Math.Max(1d, Math.Abs(max) * 0.08d);
                min -= padding;
                max += padding;
                span = Math.Max(1d, max - min);
            }
            else
            {
                span = Math.Max(1d, span);
            }

            PointF[] points = new PointF[samples.Count];
            for (int i = 0; i < samples.Count; i++)
            {
                float x = canvas.Left + 4 + (float)i * (canvas.Width - 8) / Math.Max(1, samples.Count - 1);
                float normalized = (float)((samples[i] - min) / span);
                if (nearFlat)
                {
                    normalized = 0.52F + (float)(Math.Sin((i + 1) * 0.62d) * 0.05d);
                }

                normalized = Math.Max(0.08F, Math.Min(0.92F, normalized));
                float y = canvas.Bottom - 4 - normalized * (canvas.Height - 10);
                points[i] = new PointF(x, y);
            }

            using (GraphicsPath fillPath = new GraphicsPath())
            {
                fillPath.AddLines(points);
                fillPath.AddLine(points[points.Length - 1], new PointF(points[points.Length - 1].X, canvas.Bottom - 2));
                fillPath.AddLine(new PointF(points[0].X, canvas.Bottom - 2), points[0]);
                fillPath.CloseFigure();

                using (SolidBrush fillBrush = new SolidBrush(Color.FromArgb(96, FillColor)))
                {
                    e.Graphics.FillPath(fillBrush, fillPath);
                }
            }

            using (Pen glowPen = new Pen(Color.FromArgb(92, LineColor), 4F))
            {
                e.Graphics.DrawLines(glowPen, points);
            }

            using (Pen linePen = new Pen(LineColor, 2F))
            {
                e.Graphics.DrawLines(linePen, points);
            }
        }
    }
}
