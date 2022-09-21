﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using static EndpointChecker.Program;

namespace EndpointChecker
{
    public partial class AutoUpdaterDialog : Form
    {
        // TEMPORARY PACKAGE FILE
        private static readonly string tempPackageZIPfileName = Path.GetFileName(new Uri(app_LatestPackageLink).AbsolutePath);
        private static string tempPackageFolderName = string.Empty;

        // SUCCESS SWITCH
        private static bool updateSucess = false;

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        public AutoUpdaterDialog()
        {
            InitializeComponent();

            // COMMON EXCEPTION HANDLERS
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);
            Application.ThreadException += new ThreadExceptionEventHandler(ThreadExceptionHandler);

            // SET DOUBLE BUFFER
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            // SET TITLE
            lbl_Title.Text = app_ApplicationName + " AutoUpdate";

            // SET VERSION LABEL
            lbl_Version.Text =
                "Version " +
                GetVersionString(app_LatestPackageVersion, true, false) +
                " from " +
                app_LatestPackageDate;

            BW_Update.RunWorkerAsync();
        }

        public void bw_Update_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                // DOWNLOAD PACKAGE                
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                lbl_Progress.Text = "Downloading Package from GitHub ...";

                int downloadPackage_MaxAttemptCount = 5;
                int downloadPackage_CurrentAttempt = 0;
                bool downloadPackage_Success = false;

                while (!downloadPackage_Success &&
                       downloadPackage_CurrentAttempt <= downloadPackage_MaxAttemptCount)
                {
                    // INCREASE ATTEMP COUNTER
                    downloadPackage_CurrentAttempt++;

                    try
                    {
                        // TRY TO DOWNLOAD PACKAGE
                        using (WebClient webClient = new WebClient())
                        {
                            webClient.DownloadFile(new Uri(app_LatestPackageLink), Path.Combine(Path.GetTempPath(), tempPackageZIPfileName));

                            // SUCCESS, JUMP OUT OF WHILE CYCLE AND CONTINUE CODE
                            downloadPackage_Success = true;
                        }
                    }
                    catch (Exception webClientEX)
                    {
                        // CLEAN TEMPORARY PACKAGE FILE
                        CleanTempPackageArchive();

                        // WAIT FOR 1 SECOND
                        Thread.Sleep(1000);

                        // IF CURRENT ATTEMPT COUNT EQUALS MAX ATTEMPTS COUNT, THROW EXCEPTION 
                        if (downloadPackage_CurrentAttempt == downloadPackage_MaxAttemptCount)
                        {
                            throw (webClientEX);
                        }
                    }
                }

                // UNZIP PACKAGE
                using (ZipArchive zipArchive = ZipFile.OpenRead(Path.Combine(Path.GetTempPath(), tempPackageZIPfileName)))
                {
                    tempPackageFolderName = zipArchive.Entries.First().FullName;

                    CleanTempPackageDirectory();

                    zipArchive.ExtractToDirectory(Path.GetTempPath());
                }

                // UPDATE
                lbl_Progress.Text = "Copying Files ...";
                foreach (string appFile in Directory.GetFiles(Path.Combine(Path.GetTempPath(), tempPackageFolderName)))
                {
                    if (Path.GetExtension(appFile) == ".exe" ||
                        Path.GetExtension(appFile) == ".dll" ||
                        Path.GetExtension(appFile) == ".pdb" ||
                        Path.GetExtension(appFile) == ".config")
                    {
                        File.Copy(appFile, Path.Combine(app_CurrentWorkingDir, Path.GetFileName(appFile)), true);
                    }
                }

                // CLEANUP
                lbl_Progress.Text = "Cleaning Up ...";
                CleanTempPackageArchive();
                CleanTempPackageDirectory();

                // COMPLETE
                pb_Progress.Image = Properties.Resources.Success;
                lbl_Progress.ForeColor = Color.Chartreuse;
                lbl_Progress.Text = "Update Complete";
                updateSucess = true;

                Thread.Sleep(3000);
            }
            catch (Exception exception)
            {
                // FAILED
                pb_Progress.Image = Properties.Resources.Failed;
                lbl_Progress.ForeColor = Color.Red;
                lbl_Progress.Text = "Update Failed";

                Thread.Sleep(2000);

                ExceptionNotifier(this, exception, "Package Link: " + app_LatestPackageLink, true);
            }
        }

        public void bw_Update_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (updateSucess)
            {
                // EXECUTE UPDATED APPLICATION
                ProcessStartInfo startApp = new ProcessStartInfo(Path.Combine(app_CurrentWorkingDir, app_ApplicationExecutableName));
                _ = Process.Start(startApp);
            }

            // CLOSE
            Environment.Exit(0);
        }

        public static void CleanTempPackageDirectory()
        {
            if (Directory.Exists(Path.Combine(Path.GetTempPath(), tempPackageFolderName)))
            {
                Directory.Delete(Path.Combine(Path.GetTempPath(), tempPackageFolderName), true);
            }
        }

        public static void CleanTempPackageArchive()
        {
            if (File.Exists(Path.Combine(Path.GetTempPath(), Path.Combine(Path.GetTempPath(), tempPackageZIPfileName))))
            {
                File.Delete(Path.Combine(Path.GetTempPath(), Path.Combine(Path.GetTempPath(), tempPackageZIPfileName)));
            }
        }
    }
}
