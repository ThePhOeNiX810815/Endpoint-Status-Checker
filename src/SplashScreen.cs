using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;

namespace EndpointChecker
{
    public partial class SplashScreen : Form
    {
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        public SplashScreen()
        {
            InitializeComponent();

            // SET DOUBLE BUFFER
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            // SET INFORMATION LABELS
            lbl_Name.Text = Program.app_ApplicationName;
            lbl_Version.Text = "Version ";
            lbl_Build.Text = "Build " + Program.app_BuiltDate;
            lbl_Copyright.Text = Program.app_Copyright;

            if (Program.app_Version.Build == 0)
            {
                lbl_Version.Text += Program.app_Version.Major + "." + Program.app_Version.Minor;
            }
            else
            {
                lbl_Version.Text += Program.app_VersionString;
                lbl_PreRelease.Visible = true;
            }

            Opacity = 1;
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
