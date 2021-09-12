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

            // COMMON EXCEPTION HANDLERS
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);
            Application.ThreadException += new ThreadExceptionEventHandler(ThreadExceptionHandler);

            // SET DOUBLE BUFFER
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            // SET INFORMATION LABELS
            lbl_Name.Text = Program.assembly_ApplicationName;
            lbl_Version.Text = "Version " + Program.assembly_Version;
            lbl_Build.Text = "Build " + Program.assembly_BuiltDate;
            lbl_Copyright.Text = Program.assembly_Copyright;

            if (!Program.assembly_Version.EndsWith(".0"))
            {
                lbl_Version.Text += " Pre-Release";
            }
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
            }
        }

        public void TIMER_StartFading_Tick(object sender, EventArgs e)
        {
            TIMER_StartFading.Stop();
            TIMER_FadeOutAndClose.Start();
        }

        public static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            ExceptionNotifier((Exception)args.ExceptionObject);
        }

        public static void ThreadExceptionHandler(object sender, ThreadExceptionEventArgs args)
        {
            ExceptionNotifier(args.Exception);
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
                Program.exceptionReport_senderEMailAddress,
                new List<string> { Program.authorEmailAddress },
                new List<string> { Program.endpointDefinitonsFile });

            exDialog.ShowDialog();
        }
    }
}
