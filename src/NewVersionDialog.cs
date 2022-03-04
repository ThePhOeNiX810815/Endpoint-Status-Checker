using System;
using System.Diagnostics;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using static EndpointChecker.CheckerMainForm;
using static EndpointChecker.Program;

namespace EndpointChecker
{
    public partial class NewVersionDialog : Form
    {
        public bool updateNow { get; set; }
        public bool updateSkip { get; set; }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        public NewVersionDialog()
        {
            InitializeComponent();

            // COMMON EXCEPTION HANDLERS
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);
            Application.ThreadException += new ThreadExceptionEventHandler(ThreadExceptionHandler);

            // SET DOUBLE BUFFER
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            // SET INFORMATION TEXT
            lbl_NewVersion.Text = "A new version of " + app_ApplicationName + " is available !";

            lbl_NewVersionDetail.Text =
                app_ApplicationName +
                " " +
                GetVersionString(app_LatestPackageVersion, true, false) +
                " is now available. You have version " +
                GetVersionString(app_Version, true, false) +
                ". Would you like to download it now ?";

            // SET RELEASE NOTES
            rtb_ReleaseNotes.Rtf = app_LatestPackageReleaseNotes_RTF;
        }

        public void btn_SkipThisVersion_Click(object sender, EventArgs e)
        {
            updateNow = false;
            updateSkip = true;

            Close();
        }

        public void btn_RemindMeLater_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void btn_InstallUpdate_Click(object sender, EventArgs e)
        {
            updateNow = true;
            updateSkip = false;

            Close();
        }

        public void rtb_ReleaseNotes_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            try
            {
                Uri linkURI = new Uri(e.LinkText);

                ProcessStartInfo browseLINK = new ProcessStartInfo(linkURI.AbsoluteUri);
                Process.Start(browseLINK);
            }
            catch
            {
            }
        }
    }
}
