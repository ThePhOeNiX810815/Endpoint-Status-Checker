using System;
using System.Drawing;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using static EndpointChecker.CheckerMainForm;
using static EndpointChecker.Program;

namespace EndpointChecker
{
    public partial class SplashScreen : Form
    {
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
            lbl_Version.Text = "Version ";
            lbl_Build.Text = "Built " + app_BuiltDate;
            lbl_Copyright.Text = app_Copyright;

            // SET VERSION LABEL
            if (app_Version.Build == 0)
            {
                lbl_Version.Text += app_Version.Major + "." + app_Version.Minor;
            }
            else
            {
                lbl_Version.Text += app_VersionString;
            }

            // SET RELEASE TYPE LABEL
            if (!app_IsOriginalSignedExecutable)
            {
                lbl_ReleaseType.Text = "CUSTOM BUILD";
                lbl_ReleaseType.ForeColor = Color.Red;
                lbl_ReleaseType.Visible = true;
            }
            else if (app_LatestPackageVersion > new Version(0, 0, 0, 0))
            {
                if (app_LatestPackageVersion < app_Version)
                {
                    lbl_ReleaseType.Text = "UNRELEASED BUILD";
                    lbl_ReleaseType.ForeColor = Color.DarkViolet;
                }
                else if (app_Version.Build != 0)
                {
                    lbl_ReleaseType.Text = "Pre-Release Version";
                    lbl_ReleaseType.ForeColor = Color.Blue;
                }
                else
                {
                    lbl_ReleaseType.Text = "Release Version";
                    lbl_ReleaseType.ForeColor = Color.Green;
                }

                lbl_ReleaseType.Visible = true;
            }

            Opacity = 1;

            TIMER_StartFading.Start();
        }
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x84:
                    base.WndProc(ref m);
                    if ((int)m.Result == 0x1)
                        m.Result = (IntPtr)0x2;
                    return;
            }

            base.WndProc(ref m);
        }

        public void TIMER_FadeOutAndClose_Tick(object sender, EventArgs e)
        {
            if (Opacity > 0)
            {
                Opacity = Opacity - 0.01;
            }
            else
            {
                Close();

                GC.Collect();
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

            GC.Collect();
        }
    }
}
