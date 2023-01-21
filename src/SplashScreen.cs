using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using static EndpointChecker.Program;

namespace EndpointChecker
{
    public partial class SplashScreen : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        public SplashScreen()
        {
            InitializeComponent();

            // COMMON EXCEPTION HANDLERS
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);
            Application.ThreadException += new ThreadExceptionEventHandler(ThreadExceptionHandler);

            // SET DOUBLE BUFFER
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            // SET INFORMATION LABELS
            lbl_Name.Text = app_ApplicationName;
            lbl_Version_Date.Text = "v" + GetVersionString(app_Version, app_Version.Build != 0, false) + " ~ " + app_Built_Date;
            lbl_Copyright.Text = app_Copyright;


            // SET RELEASE TYPE LABEL
            if (app_TestMode)
            {
                lbl_ReleaseType.Text = "BUILD IN TEST MODE";
                lbl_ReleaseType.ForeColor = Color.OrangeRed;
            }
            else if (!app_IsOriginalSignedExecutable)
            {
                lbl_ReleaseType.Text = "CUSTOM UNSIGNED BUILD";
                lbl_ReleaseType.ForeColor = Color.Red;
            }
            else
            {
                if (app_LatestPackageVersion > new Version(0, 0, 0, 0) &&
                    app_LatestPackageVersion < app_Version)
                {
                    lbl_ReleaseType.Text = "UNRELEASED VERSION";
                    lbl_ReleaseType.ForeColor = Color.DarkViolet;
                }
                else if (app_Version.Build != 0)
                {
                    lbl_ReleaseType.Text = "PRE-RELEASE VERSION";
                    lbl_ReleaseType.ForeColor = Color.Blue;
                }
                else
                {
                    lbl_ReleaseType.Text = "RELEASE VERSION";
                    lbl_ReleaseType.ForeColor = Color.Green;
                }
            }

            Opacity = 1;

            TIMER_StartFading.Start();
        }

        public void Controls_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        public void TIMER_FadeOutAndClose_Tick(object sender, EventArgs e)
        {
            if (Opacity > 0)
            {
                Opacity -= 0.01;
            }
            else
            {
                Close();
            }
        }

        public void TIMER_StartFading_Tick(object sender, EventArgs e)
        {
            TIMER_StartFading.Stop();
            TIMER_FadeOutAndClose.Start();
        }

        public void pb_CloseDialog_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
