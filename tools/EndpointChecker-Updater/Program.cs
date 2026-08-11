using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EndpointCheckerUpdater
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new UpdaterForm());
        }
    }

    internal sealed class UpdaterForm : Form
    {
        // ── Constants ────────────────────────────────────────────────────────────

        const string DownloadUrl = "";
        const string TargetVer = "development build";
        const string AppExe = "EndpointChecker.exe";

        // Set this to false only for an official, stable (non-RC) release build.
        // While true, the user is always asked to confirm before a test build is installed.
        const bool IsReleaseCandidateBuild = true;

        // User data files that must survive the update
        static readonly string[] UserDataFiles =
        {
            "EndpointChecker_EndpointsList.txt",
            "EndpointChecker_LastSeenOnline.json"
        };

        // ── Controls ─────────────────────────────────────────────────────────────

        private TextBox _txtDir;
        private Label _lblVersion, _lblStatus;
        private Button _btnBrowse, _btnUpdate, _btnClose;
        private ProgressBar _progress;
        private RichTextBox _log;

        // ── Constructor ──────────────────────────────────────────────────────────

        public UpdaterForm()
        {
            BuildUI();
            DetectInstallDir();
        }

        // ── UI ───────────────────────────────────────────────────────────────────

        private void BuildUI()
        {
            Text = "Endpoint Status Checker — Updater unavailable";
            Size = new Size(580, 500);
            MinimumSize = Size;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9f);
            BackColor = Color.FromArgb(22, 19, 25);
            ForeColor = Color.WhiteSmoke;

            // Header
            var lblTitle = MakeLabel(
                "Endpoint Status Checker — Updater",
                16, 14, 544, 28,
                Color.FromArgb(100, 160, 255),
                new Font("Segoe UI", 13f, FontStyle.Bold));

            var lblSub = MakeLabel(
                "Official v3 updates are not available yet. Use a manually provided test build only.",
                16, 44, 544, 18, Color.Silver);

            var sep1 = MakeSep(70);

            // Install directory
            var lblDir = MakeLabel(
                "Install directory  (folder that contains EndpointChecker.exe):",
                14, 82, 544, 18);

            _txtDir = new TextBox
            {
                Location = new Point(14, 102),
                Size = new Size(446, 22),
                BackColor = Color.FromArgb(30, 35, 60),
                ForeColor = Color.WhiteSmoke,
                BorderStyle = BorderStyle.FixedSingle
            };

            _btnBrowse = MakeButton("Browse…", 466, 100, 96, 26);
            _btnBrowse.Click += BtnBrowse_Click;

            _lblVersion = MakeLabel("Detected version: —", 14, 130, 544, 18);

            var sep2 = MakeSep(152);

            // Progress
            _progress = new ProgressBar
            {
                Location = new Point(14, 162),
                Size = new Size(548, 16),
                Minimum = 0,
                Maximum = 100,
                Style = ProgressBarStyle.Continuous
            };

            _lblStatus = MakeLabel(
                "Updater disabled until an official v3 release is published.",
                14, 182, 548, 18);

            // Log
            _log = new RichTextBox
            {
                Location = new Point(14, 206),
                Size = new Size(548, 212),
                ReadOnly = true,
                BackColor = Color.FromArgb(15, 15, 20),
                ForeColor = Color.FromArgb(160, 200, 160),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 8.5f),
                ScrollBars = RichTextBoxScrollBars.Vertical
            };

            // Buttons
            _btnUpdate = MakeButton("Start Update", 358, 428, 108, 30,
                Color.FromArgb(25, 90, 45), Color.FromArgb(45, 150, 75));
            _btnUpdate.Enabled = false;
            _btnUpdate.Click += BtnUpdate_Click;

            _btnClose = MakeButton("Close", 474, 428, 88, 30,
                Color.FromArgb(60, 30, 30), Color.FromArgb(120, 50, 50));
            _btnClose.Click += (s, e) => Close();

            Controls.AddRange(new Control[]
            {
                lblTitle, lblSub, sep1,
                lblDir, _txtDir, _btnBrowse, _lblVersion,
                sep2, _progress, _lblStatus, _log,
                _btnUpdate, _btnClose
            });
        }

        private static Label MakeLabel(string text, int x, int y, int w, int h,
            Color? color = null, Font font = null) =>
            new Label
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, h),
                AutoSize = false,
                ForeColor = color ?? Color.Silver,
                Font = font
            };

        private static Label MakeSep(int y) =>
            new Label
            {
                Location = new Point(0, y),
                Size = new Size(580, 1),
                BackColor = Color.FromArgb(50, 50, 70)
            };

        private static Button MakeButton(string text, int x, int y, int w, int h,
            Color? bg = null, Color? border = null)
        {
            var b = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, h),
                FlatStyle = FlatStyle.Flat,
                BackColor = bg ?? Color.FromArgb(40, 50, 90),
                ForeColor = Color.WhiteSmoke
            };
            b.FlatAppearance.BorderColor = border ?? Color.FromArgb(60, 80, 140);
            return b;
        }

        // ── Auto-detection ───────────────────────────────────────────────────────

        private void DetectInstallDir()
        {
            // 1. Running process — most reliable
            foreach (var proc in Process.GetProcessesByName("EndpointChecker"))
            {
                try
                {
                    SetDir(Path.GetDirectoryName(proc.MainModule.FileName));
                    return;
                }
                catch { }
            }

            // 2. Common candidate locations
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string profile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string progFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string progFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            foreach (string candidate in new[]
            {
                Path.Combine(desktop,       AppExe),
                Path.Combine(profile,       "Downloads", AppExe),
                Path.Combine(progFiles,     "EndpointChecker", AppExe),
                Path.Combine(progFilesX86,  "EndpointChecker", AppExe),
                @"C:\EndpointChecker\"       + AppExe,
                @"C:\Tools\EndpointChecker\" + AppExe,
            })
            {
                if (File.Exists(candidate))
                {
                    SetDir(Path.GetDirectoryName(candidate));
                    return;
                }
            }
        }

        private void SetDir(string dir)
        {
            _txtDir.Text = dir ?? string.Empty;
            string exePath = Path.Combine(dir ?? string.Empty, AppExe);

            bool downloadAvailable = !string.IsNullOrWhiteSpace(DownloadUrl);

            if (File.Exists(exePath))
            {
                string availabilitySuffix = downloadAvailable
                    ? "  →  a test build is available"
                    : "  →  no official v3 update available";

                try
                {
                    var vi = FileVersionInfo.GetVersionInfo(exePath);
                    _lblVersion.Text = $"Detected:  v{vi.FileVersion}{availabilitySuffix}";
                    _lblVersion.ForeColor = Color.FromArgb(100, 200, 100);
                }
                catch
                {
                    _lblVersion.Text = $"Detected: EndpointChecker.exe found (version unknown){availabilitySuffix}";
                    _lblVersion.ForeColor = Color.FromArgb(100, 200, 100);
                }

                _btnUpdate.Enabled = downloadAvailable;
            }
            else
            {
                _lblVersion.Text = "EndpointChecker.exe not found in this folder.";
                _lblVersion.ForeColor = Color.Orange;
                _btnUpdate.Enabled = false;
            }
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog
            {
                Description = "Select the folder that contains EndpointChecker.exe",
                SelectedPath = _txtDir.Text
            })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                    SetDir(dlg.SelectedPath);
            }
        }

        // ── Update logic ─────────────────────────────────────────────────────────

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(DownloadUrl))
            {
                MessageBox.Show(
                    "Official v3 updates are not available yet.",
                    Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            if (IsReleaseCandidateBuild)
            {
                DialogResult confirm = MessageBox.Show(
                    $"v{TargetVer} is a test (release candidate) build, not an official stable release.\r\n\r\n" +
                    "Do you want to install it to help test it?",
                    "Test Build Confirmation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirm != DialogResult.Yes)
                {
                    Log("Update cancelled by user (test build declined).", Color.Yellow);
                    return;
                }
            }

            string installDir = _txtDir.Text.Trim();
            if (string.IsNullOrEmpty(installDir) || !File.Exists(Path.Combine(installDir, AppExe)))
            {
                MessageBox.Show(
                    "Select a valid install directory containing EndpointChecker.exe first.",
                    Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            SetControlsEnabled(false);
            _ = RunUpdateAsync(installDir);
        }

        private void SetControlsEnabled(bool enabled)
        {
            _btnUpdate.Enabled = enabled;
            _btnBrowse.Enabled = enabled;
            _txtDir.Enabled = enabled;
            _btnClose.Enabled = enabled;
        }

        private async Task RunUpdateAsync(string installDir)
        {
            try
            {
                await RunUpdateStepsAsync(installDir);
            }
            catch (Exception ex)
            {
                Log($"Update failed: {ex.Message}", Color.OrangeRed);
                Status("Update failed. See log for details.", Color.OrangeRed);
                SetControlsEnabled(true);
                MessageBox.Show(
                    $"The update could not be completed:\r\n\r\n{ex.Message}\r\n\r\n" +
                    "Nothing further was changed after this point — you can try again.",
                    Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private async Task RunUpdateStepsAsync(string installDir)
        {
            string tempZip = Path.Combine(Path.GetTempPath(), "EndpointChecker-update-download.zip");
            string tempExtract = Path.Combine(Path.GetTempPath(), "EndpointChecker_Updater_Extract");

            // ── Step 1: Stop running instance ────────────────────────────────────
            Status("Stopping running application...");
            Log("Checking for running EndpointChecker processes...");

            foreach (var proc in Process.GetProcessesByName("EndpointChecker"))
            {
                try
                {
                    Log($"  Stopping PID {proc.Id}...");
                    proc.CloseMainWindow();
                    if (!proc.WaitForExit(3000))
                        proc.Kill();
                    await Task.Delay(500);
                    Log("  Stopped.");
                }
                catch (Exception ex)
                {
                    Log($"  Warning: {ex.Message}", Color.Yellow);
                }
            }

            Progress(3);

            // ── Step 2: Download ─────────────────────────────────────────────────
            Status("Downloading official v3 package from GitHub...");
            Log($"Downloading: {DownloadUrl}");

            using (var wc = new WebClient())
            {
                wc.DownloadProgressChanged += (s, ev) =>
                {
                    Progress(3 + (int)(ev.ProgressPercentage * 0.57));
                    Status($"Downloading...  {ev.ProgressPercentage}%  " +
                           $"({ev.BytesReceived / 1_048_576} MB / {ev.TotalBytesToReceive / 1_048_576} MB)");
                };
                await wc.DownloadFileTaskAsync(DownloadUrl, tempZip);
            }

            long sizeMB = new FileInfo(tempZip).Length / 1_048_576;
            Log($"Download complete: {sizeMB} MB");
            Progress(60);

            // ── Step 3: Extract ──────────────────────────────────────────────────
            Status("Extracting package...");
            Log($"Extracting to: {tempExtract}");

            await Task.Run(() =>
            {
                if (Directory.Exists(tempExtract))
                    Directory.Delete(tempExtract, true);
                ZipFile.ExtractToDirectory(tempZip, tempExtract);
            });

            Log("Extraction complete.");
            Progress(70);

            // Some release zips wrap everything in a single top-level folder
            // (e.g. "EndpointChecker-v3.1.1-rc2-test/EndpointChecker.exe") instead of a flat
            // layout. If that's what we got, treat that folder as the real root so files land
            // directly in installDir rather than in an unexpected subfolder. tempExtract itself
            // is left alone so step 7 still cleans up the whole extraction, not just the subfolder.
            string copySourceRoot = tempExtract;
            string[] rootEntries = Directory.GetFileSystemEntries(tempExtract);
            if (rootEntries.Length == 1 && Directory.Exists(rootEntries[0]))
            {
                copySourceRoot = rootEntries[0];
                Log($"Zip wraps a single folder — using it as the update root: {Path.GetFileName(copySourceRoot)}");
            }

            // ── Step 4: Backup user data ─────────────────────────────────────────
            Status("Backing up user data...");
            var backups = new Dictionary<string, byte[]>();

            foreach (string uf in UserDataFiles)
            {
                string path = Path.Combine(installDir, uf);
                if (File.Exists(path))
                {
                    backups[uf] = File.ReadAllBytes(path);
                    Log($"Backed up: {uf}");
                }
            }

            Progress(75);

            // ── Step 5: Copy new files ───────────────────────────────────────────
            Status("Installing new files...");
            Log($"Copying to: {installDir}");
            int filesCopied = 0;

            await Task.Run(() =>
            {
                foreach (string src in Directory.GetFiles(copySourceRoot, "*", SearchOption.AllDirectories))
                {
                    string rel = src.Substring(copySourceRoot.Length).TrimStart(Path.DirectorySeparatorChar);
                    string dest = Path.Combine(installDir, rel);
                    Directory.CreateDirectory(Path.GetDirectoryName(dest));
                    CopyFileWithRetry(src, dest);
                    filesCopied++;
                }
            });

            Log($"Installed {filesCopied} files.");
            Progress(92);

            // ── Step 6: Restore user data ────────────────────────────────────────
            Status("Restoring user data...");

            foreach (var kv in backups)
            {
                File.WriteAllBytes(Path.Combine(installDir, kv.Key), kv.Value);
                Log($"Restored: {kv.Key}");
            }

            Progress(96);

            // ── Step 7: Cleanup ──────────────────────────────────────────────────
            Status("Cleaning up temporary files...");

            await Task.Run(() =>
            {
                try { File.Delete(tempZip); } catch { }
                try { Directory.Delete(tempExtract, true); } catch { }
            });

            Progress(100);

            // ── Step 8: Done ─────────────────────────────────────────────────────
            Log($"Update to v{TargetVer} complete!", Color.FromArgb(100, 220, 100));
            Status($"Update complete!  Launching Endpoint Status Checker v{TargetVer}...",
                Color.FromArgb(100, 220, 100));

            _btnClose.Enabled = true;
            await Task.Delay(1500);

            string newExe = Path.Combine(installDir, AppExe);
            if (File.Exists(newExe))
                Process.Start(new ProcessStartInfo(newExe) { UseShellExecute = true });

            Close();
        }

        // The exe we just stopped can stay briefly locked by the OS/AV after process exit —
        // retry a few times with a short delay rather than failing the whole update over it.
        private static void CopyFileWithRetry(string src, string dest, int maxAttempts = 5)
        {
            for (int attempt = 1; ; attempt++)
            {
                try
                {
                    File.Copy(src, dest, overwrite: true);
                    return;
                }
                catch (IOException) when (attempt < maxAttempts)
                {
                    Thread.Sleep(300);
                }
            }
        }

        // ── Thread-safe UI helpers ───────────────────────────────────────────────

        private void Status(string text, Color color = default)
        {
            if (InvokeRequired) { Invoke(new Action(() => Status(text, color))); return; }
            _lblStatus.ForeColor = color == default ? Color.Silver : color;
            _lblStatus.Text = text;
        }

        private void Progress(int value)
        {
            if (InvokeRequired) { Invoke(new Action(() => Progress(value))); return; }
            _progress.Value = Math.Min(value, _progress.Maximum);
        }

        private void Log(string text, Color color = default)
        {
            if (InvokeRequired) { Invoke(new Action(() => Log(text, color))); return; }
            _log.SelectionStart = _log.TextLength;
            _log.SelectionLength = 0;
            _log.SelectionColor = color == default ? Color.FromArgb(160, 200, 160) : color;
            _log.AppendText(text + "\n");
            _log.ScrollToCaret();
        }
    }
}
