using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using static EndpointChecker.Program;

namespace EndpointChecker
{
    public partial class AutoUpdaterDialog : Form
    {
        // TEMPORARY PACKAGE FILE
        static string tempPackageZIPfileName = Path.GetFileName(new Uri(app_LatestPackageLink).AbsolutePath);
        static string tempPackageFolderName = string.Empty;

        public AutoUpdaterDialog()
        {
            InitializeComponent();

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
                lbl_Progress.Text = "Downloading Package from GitHub ...";
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                WebClient webClient = new WebClient();
                webClient.DownloadFile(new Uri(app_LatestPackageLink), Path.Combine(Path.GetTempPath(), tempPackageZIPfileName));

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

                Thread.Sleep(3000);
            }
            catch (Exception exception)
            {
                // FAILED
                pb_Progress.Image = Properties.Resources.Failed;
                lbl_Progress.ForeColor = Color.Red;
                lbl_Progress.Text = "Update Failed";

                Thread.Sleep(2000);

                ExceptionNotifier(exception);
            }
        }

        public void bw_Update_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // EXECUTE APPLICATION
            ProcessStartInfo startApp = new ProcessStartInfo(Path.Combine(app_CurrentWorkingDir, app_ApplicationExecutableName));
            Process.Start(startApp);

            // CLOSE
            Application.Exit();
        }
        public static void ExceptionNotifier(Exception exception, string callingMethod = "")
        {
            if (string.IsNullOrEmpty(callingMethod))
            {
                callingMethod = new StackTrace().GetFrame(1).GetMethod().Name;
            }

            ExceptionDialog exDialog = new ExceptionDialog(
                exception,
                callingMethod,
                exceptionReport_senderEMailAddress,
                new List<string> { authorEmailAddress },
                new List<string> { endpointDefinitonsFile });

            exDialog.ShowDialog();
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
