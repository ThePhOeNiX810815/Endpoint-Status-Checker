using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using static EndpointChecker.Program;

namespace EndpointChecker
{
    public partial class SplashScreen : Form
    {
        private const int LegacySplashWidth = 1000;
        private const int LegacySplashHeight = 378;

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public SplashScreen()
        {
            InitializeComponent();

            ApplyCustomSplashBackground();
            ApplySplashLabelStyling();

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
                lbl_ReleaseType.ForeColor = Color.FromArgb(245, 180, 120);
            }
            else if (!app_IsOriginalSignedExecutable)
            {
                lbl_ReleaseType.Text = "CUSTOM UNSIGNED BUILD";
                lbl_ReleaseType.ForeColor = Color.FromArgb(244, 130, 130);
            }
            else
            {
                if (app_LatestPackageVersion > new Version(0, 0, 0, 0) &&
                    app_LatestPackageVersion < app_Version)
                {
                    lbl_ReleaseType.Text = "UNRELEASED VERSION";
                    lbl_ReleaseType.ForeColor = Color.FromArgb(196, 166, 244);
                }
                else if (app_Version.Build != 0)
                {
                    lbl_ReleaseType.Text = "PRE-RELEASE VERSION";
                    lbl_ReleaseType.ForeColor = Color.FromArgb(140, 196, 244);
                }
                else
                {
                    lbl_ReleaseType.Text = "RELEASE VERSION";
                    lbl_ReleaseType.ForeColor = Color.FromArgb(136, 212, 168);
                }
            }

            Opacity = 1;

            TIMER_StartFading.Start();
        }

        private void ApplyCustomSplashBackground()
        {
            Image customSplash = TryLoadCustomSplashBackground();
            if (customSplash == null)
            {
                return;
            }

            BackgroundImage = customSplash;
            BackgroundImageLayout = ImageLayout.Stretch;
            ResizeSplashForBackground(customSplash.Size);
        }

        private static Image TryLoadCustomSplashBackground()
        {
            string imagesPath = Path.Combine(AppContext.BaseDirectory, "Images");
            string[] candidates =
            {
                Path.Combine(imagesPath, "Splash_Screen.jpg"),
                Path.Combine(imagesPath, "Splash_Screen.jpeg"),
                Path.Combine(imagesPath, "Splash_Screen.png"),
            };

            foreach (string candidate in candidates)
            {
                try
                {
                    if (File.Exists(candidate))
                    {
                        using (FileStream fileStream = File.OpenRead(candidate))
                        using (Image loaded = Image.FromStream(fileStream))
                        {
                            return new Bitmap(loaded);
                        }
                    }
                }
                catch
                {
                }
            }

            return null;
        }

        private void ResizeSplashForBackground(Size splashSize)
        {
            if (splashSize.Width <= 0 || splashSize.Height <= 0)
            {
                return;
            }

            float scaleX = splashSize.Width / (float)LegacySplashWidth;
            float scaleY = splashSize.Height / (float)LegacySplashHeight;

            ClientSize = splashSize;
            MinimumSize = splashSize;
            MaximumSize = splashSize;

            ScaleControlBounds(lbl_Name, scaleX, scaleY);
            ScaleControlBounds(lbl_Version_Date, scaleX, scaleY);
            ScaleControlBounds(lbl_Copyright, scaleX, scaleY);
            ScaleControlBounds(lbl_ReleaseType, scaleX, scaleY);
            ScaleControlBounds(pb_CloseDialog, scaleX, scaleY);

            ScaleLabelFont(lbl_Name, scaleY);
            ScaleLabelFont(lbl_Version_Date, scaleY);
            ScaleLabelFont(lbl_Copyright, scaleY);
            ScaleLabelFont(lbl_ReleaseType, scaleY);
        }

        private static void ScaleControlBounds(Control control, float scaleX, float scaleY)
        {
            control.Location = new Point(
                (int)Math.Round(control.Location.X * scaleX),
                (int)Math.Round(control.Location.Y * scaleY));

            control.Size = new Size(
                Math.Max(1, (int)Math.Round(control.Size.Width * scaleX)),
                Math.Max(1, (int)Math.Round(control.Size.Height * scaleY)));
        }

        private static void ScaleLabelFont(Label label, float scaleY)
        {
            float scaledSize = Math.Max(8f, label.Font.Size * scaleY);
            label.Font = new Font(label.Font.FontFamily, scaledSize, label.Font.Style);
        }

        private void ApplySplashLabelStyling()
        {
            lbl_Name.ForeColor = Color.FromArgb(232, 241, 245);
            lbl_Version_Date.ForeColor = Color.FromArgb(190, 210, 224);
            lbl_Copyright.ForeColor = Color.FromArgb(214, 214, 214);
            lbl_ReleaseType.ForeColor = Color.FromArgb(220, 220, 220);
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
