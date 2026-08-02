using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static EndpointChecker.Program;

namespace EndpointChecker
{
    public partial class AutoUpdaterDialog : Form
    {
        // Derived lazily so the class can be referenced before CheckForUpdate() sets the link.
        private static string _zipFileName => Path.GetFileName(new Uri(app_LatestPackageLink).AbsolutePath);
        private static string _extractFolder => "EndpointChecker_Update";
        private static string _updateScript = string.Empty;
        private static bool _updateSuccess = false;

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")] public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")] public static extern bool ReleaseCapture();

        public AutoUpdaterDialog()
        {
            InitializeComponent();

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);
            Application.ThreadException += new ThreadExceptionEventHandler(ThreadExceptionHandler);

            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            lbl_Name.Text = app_ApplicationName;
            lbl_Copyright.Text = app_Copyright;

            lbl_UpdateVersion.Text =
                "Version " +
                GetVersionString(app_LatestPackageVersion, app_LatestPackageVersion.Build != 0, false);

            lbl_ReleaseDate.Text = "Released " + app_LatestPackageDate;

            BW_Update.RunWorkerAsync();
        }

        // ── Background worker ────────────────────────────────────────────────────

        public void bw_Update_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Thread.Sleep(1000);

                // 1. DOWNLOAD
                ThreadSafeInvoke(() => lbl_Progress.Text = "Downloading Package from GitHub ...");
                Thread.Sleep(1000);

                int maxAttempts = 20;
                int attempt = 0;
                bool downloaded = false;

                while (!downloaded && attempt < maxAttempts)
                {
                    attempt++;
                    try
                    {
                        using (CustomWebClient wc = new CustomWebClient())
                            wc.DownloadFile(new Uri(app_LatestPackageLink),
                                            Path.Combine(app_TempDir, _zipFileName));
                        downloaded = true;
                    }
                    catch (Exception)
                    {
                        CleanTempPackageArchive();
                        Thread.Sleep(1000);
                        if (attempt == maxAttempts) throw;
                    }
                }

                // 2. EXTRACT
                ThreadSafeInvoke(() => lbl_Progress.Text = "Extracting Package ...");
                Thread.Sleep(1000);
                UnzipUpdatePackage();

                // 3. WRITE UPDATE SCRIPT
                ThreadSafeInvoke(() => lbl_Progress.Text = "Preparing Update ...");
                Thread.Sleep(1000);
                WriteUpdateScript();

                // 4. CLEANUP ZIP
                ThreadSafeInvoke(() => lbl_Progress.Text = "Cleaning Up ...");
                Thread.Sleep(1000);
                CleanTempPackageArchive();

                // 5. SUCCESS
                ThreadSafeInvoke(() =>
                {
                    lbl_Progress.Visible = false;
                    lbl_UpdateStatus_Wait.Visible = false;
                    lbl_UpdateStatus.ForeColor = Color.Lime;
                    lbl_UpdateStatus.Text = "SUCCESSFULLY UPDATED";
                });

                _updateSuccess = true;
                Thread.Sleep(3000);
            }
            catch (Exception exception)
            {
                ThreadSafeInvoke(() =>
                {
                    lbl_Progress.Visible = false;
                    lbl_UpdateStatus_Wait.Visible = false;
                    lbl_UpdateStatus.ForeColor = Color.Red;
                    lbl_UpdateStatus.Text = "UPDATE FAILED";
                });

                Thread.Sleep(2000);
                ExceptionNotify(this, exception, "Package Link: " + app_LatestPackageLink, true);
            }
        }

        public void bw_Update_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            TIMER_FadeOutAndClose.Start();
        }

        // ── Fade-out / close ─────────────────────────────────────────────────────

        public void TIMER_FadeOutAndClose_Tick(object sender, EventArgs e)
        {
            if (Opacity > 0)
            {
                Opacity -= 0.01;
            }
            else
            {
                if (_updateSuccess && File.Exists(_updateScript))
                {
                    // Launch the update script minimised; it waits 3 s for this process to exit,
                    // then xcopy all new files, restores user data, and relaunches the app.
                    Process.Start(new ProcessStartInfo(_updateScript)
                    {
                        UseShellExecute = true,
                        WindowStyle = ProcessWindowStyle.Minimized
                    });
                }

                Environment.Exit(0);
            }
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        public static void UnzipUpdatePackage()
        {
            string extractDir = Path.Combine(app_TempDir, _extractFolder);
            CleanTempPackageDirectory();
            Directory.CreateDirectory(extractDir);
            ZipFile.ExtractToDirectory(Path.Combine(app_TempDir, _zipFileName), extractDir);
        }

        private static void WriteUpdateScript()
        {
            string extractDir = Path.Combine(app_TempDir, _extractFolder);
            string appDir = app_CurrentWorkingDir;
            string appExe = Path.Combine(appDir, "EndpointChecker.exe");
            string backupDir = Path.Combine(app_TempDir, "EndpointChecker_UserData");
            _updateScript = Path.Combine(app_TempDir, "EndpointChecker_Update.cmd");

            var sb = new StringBuilder();
            sb.AppendLine("@echo off");
            // Wait for the main process to release file locks
            sb.AppendLine("timeout /t 3 /nobreak > nul");
            // Backup user data files that should not be overwritten
            sb.AppendLine($"if not exist \"{backupDir}\" mkdir \"{backupDir}\"");
            sb.AppendLine($"if exist \"{appDir}\\EndpointChecker_EndpointsList.txt\" copy /y \"{appDir}\\EndpointChecker_EndpointsList.txt\" \"{backupDir}\\\" >nul 2>&1");
            sb.AppendLine($"if exist \"{appDir}\\EndpointChecker_LastSeenOnline.json\" copy /y \"{appDir}\\EndpointChecker_LastSeenOnline.json\" \"{backupDir}\\\" >nul 2>&1");
            // Copy all new files (overwrites everything including runtime DLLs)
            sb.AppendLine($"xcopy /s /y /e \"{extractDir}\\*.*\" \"{appDir}\\\"");
            // Restore user data
            sb.AppendLine($"if exist \"{backupDir}\\EndpointChecker_EndpointsList.txt\" copy /y \"{backupDir}\\EndpointChecker_EndpointsList.txt\" \"{appDir}\\\" >nul 2>&1");
            sb.AppendLine($"if exist \"{backupDir}\\EndpointChecker_LastSeenOnline.json\" copy /y \"{backupDir}\\EndpointChecker_LastSeenOnline.json\" \"{appDir}\\\" >nul 2>&1");
            // Relaunch the updated app
            sb.AppendLine($"start \"\" \"{appExe}\"");
            // Cleanup
            sb.AppendLine($"rmdir /s /q \"{extractDir}\" >nul 2>&1");
            sb.AppendLine($"rmdir /s /q \"{backupDir}\" >nul 2>&1");
            sb.AppendLine("(goto) 2>nul & del /f \"%~f0\"");

            File.WriteAllText(_updateScript, sb.ToString(), Encoding.ASCII);
        }

        public static void CleanTempPackageDirectory()
        {
            string dir = Path.Combine(app_TempDir, _extractFolder);
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
        }

        public static void CleanTempPackageArchive()
        {
            string zip = Path.Combine(app_TempDir, _zipFileName);
            if (File.Exists(zip))
                File.Delete(zip);
        }

        public void Controls_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        public void ThreadSafeInvoke(Action action)
        {
            try
            {
                Application.DoEvents();
                Invoke(action);
            }
            catch { }
        }
    }
}
