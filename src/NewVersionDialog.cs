using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using static EndpointChecker.Program;

namespace EndpointChecker
{
    public partial class NewVersionDialog : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        public bool UpdateNow { get; set; }
        public bool UpdateSkip { get; set; }
        public bool AutoUpdateInFuture { get; set; }

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
            UpdateNow = false;
            UpdateSkip = true;
            AutoUpdateInFuture = cb_FutureAutoUpdate.Checked;

            Close();
        }

        public void btn_RemindMeLater_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void btn_InstallUpdate_Click(object sender, EventArgs e)
        {
            UpdateNow = true;
            UpdateSkip = false;
            AutoUpdateInFuture = cb_FutureAutoUpdate.Checked;

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

        public void Controls_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}
