using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using static EndpointChecker.Program;

namespace EndpointChecker
{
    public partial class AboutDialog : Form
    {
        public AboutDialog()
        {
            InitializeComponent();

            try
            {
                string iconPath = Path.Combine(AppContext.BaseDirectory, "app.ico");
                if (File.Exists(iconPath))
                {
                    Icon appIcon = new Icon(iconPath);
                    Icon = appIcon;
                    pb_Icon.Image = appIcon.ToBitmap();
                }
            }
            catch
            {
            }

            lbl_Title.Text = app_Title;
            lbl_Developer.Text = "Developer: " + app_Developer;
            lbl_VersionBuilt.Text = "Version: " + GetVersionString(app_Version, true, true) + ", Built: " + app_Built_DateTime;
            lbl_Copyright.Text = app_Copyright;
            linkLabel_HomePage.Text = app_HomePage;
            linkLabel_GitHub.Text = "GitHub (v3)";

            if (string.IsNullOrEmpty(app_ReleaseChannelLabel))
            {
                // Stable release — no badge to show, and everything below the title
                // shifts up to close the gap left by the hidden label.
                lbl_ReleaseChannel.Visible = false;

                int shiftUp = lbl_Developer.Top - lbl_ReleaseChannel.Top;
                foreach (Control control in new Control[]
                {
                    lbl_Developer, lbl_VersionBuilt, lbl_Copyright,
                    linkLabel_HomePage, linkLabel_GitHub, cb_KeepPlayingMusic
                })
                {
                    control.Top -= shiftUp;
                }
            }
            else
            {
                lbl_ReleaseChannel.Text = app_ReleaseChannelLabel;
            }

            // If a track from a previous About screen is still playing, leave it alone (never
            // overlap two tracks) and reflect that "keep playing" is effectively already on.
            bool alreadyPlaying = AboutDialogMusicPlayer.IsPlaying;
            AboutDialogMusicPlayer.EnsureStarted();
            cb_KeepPlayingMusic.Checked = alreadyPlaying;
        }

        private void linkLabel_HomePage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(app_HomePage) { UseShellExecute = true });
            }
            catch
            {
            }
        }

        private void linkLabel_GitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(app_GitHubPage) { UseShellExecute = true });
            }
            catch
            {
            }
        }

        private void AboutDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Music only ever stops here, on close — never mid-session just from toggling the checkbox.
            if (!cb_KeepPlayingMusic.Checked)
            {
                AboutDialogMusicPlayer.Stop();
            }
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
