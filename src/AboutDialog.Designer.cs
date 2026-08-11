namespace EndpointChecker
{
    partial class AboutDialog
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pb_Icon = new System.Windows.Forms.PictureBox();
            this.lbl_Title = new System.Windows.Forms.Label();
            this.lbl_ReleaseChannel = new System.Windows.Forms.Label();
            this.lbl_Developer = new System.Windows.Forms.Label();
            this.lbl_VersionBuilt = new System.Windows.Forms.Label();
            this.lbl_Copyright = new System.Windows.Forms.Label();
            this.linkLabel_HomePage = new System.Windows.Forms.LinkLabel();
            this.linkLabel_GitHub = new System.Windows.Forms.LinkLabel();
            this.cb_KeepPlayingMusic = new System.Windows.Forms.CheckBox();
            this.btn_Close = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Icon)).BeginInit();
            this.SuspendLayout();

            // pb_Icon
            this.pb_Icon.Location = new System.Drawing.Point(16, 16);
            this.pb_Icon.Size = new System.Drawing.Size(48, 48);
            this.pb_Icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_Icon.TabStop = false;

            // lbl_Title
            this.lbl_Title.AutoSize = true;
            this.lbl_Title.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lbl_Title.Location = new System.Drawing.Point(76, 20);
            this.lbl_Title.Text = "Endpoint Status Checker";

            // lbl_ReleaseChannel
            // Hidden and skipped over (see AboutDialog.cs) once app_ReleaseChannelLabel is empty,
            // i.e. once this is a stable release rather than a release-candidate build.
            this.lbl_ReleaseChannel.AutoSize = true;
            this.lbl_ReleaseChannel.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lbl_ReleaseChannel.ForeColor = System.Drawing.Color.DarkOrange;
            this.lbl_ReleaseChannel.Location = new System.Drawing.Point(76, 44);
            this.lbl_ReleaseChannel.Text = string.Empty;

            // lbl_Developer
            this.lbl_Developer.AutoSize = true;
            this.lbl_Developer.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lbl_Developer.Location = new System.Drawing.Point(76, 64);
            this.lbl_Developer.Text = "Developer: ";

            // lbl_VersionBuilt
            this.lbl_VersionBuilt.AutoSize = true;
            this.lbl_VersionBuilt.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lbl_VersionBuilt.Location = new System.Drawing.Point(16, 92);
            this.lbl_VersionBuilt.Text = "Version: , Built: ";

            // lbl_Copyright
            this.lbl_Copyright.AutoSize = true;
            this.lbl_Copyright.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lbl_Copyright.Location = new System.Drawing.Point(16, 114);
            this.lbl_Copyright.Text = string.Empty;

            // linkLabel_HomePage
            this.linkLabel_HomePage.AutoSize = true;
            this.linkLabel_HomePage.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.linkLabel_HomePage.Location = new System.Drawing.Point(16, 138);
            this.linkLabel_HomePage.Text = string.Empty;
            this.linkLabel_HomePage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_HomePage_LinkClicked);

            // linkLabel_GitHub
            this.linkLabel_GitHub.AutoSize = true;
            this.linkLabel_GitHub.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.linkLabel_GitHub.Location = new System.Drawing.Point(16, 160);
            this.linkLabel_GitHub.Text = "GitHub (v3)";
            this.linkLabel_GitHub.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_GitHub_LinkClicked);

            // cb_KeepPlayingMusic
            this.cb_KeepPlayingMusic.AutoSize = true;
            this.cb_KeepPlayingMusic.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cb_KeepPlayingMusic.Location = new System.Drawing.Point(16, 190);
            this.cb_KeepPlayingMusic.Size = new System.Drawing.Size(300, 20);
            this.cb_KeepPlayingMusic.Text = "Keep playing music after closing this screen";
            this.cb_KeepPlayingMusic.UseVisualStyleBackColor = true;

            // btn_Close
            this.btn_Close.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btn_Close.Location = new System.Drawing.Point(296, 222);
            this.btn_Close.Size = new System.Drawing.Size(80, 28);
            this.btn_Close.Text = "Close";
            this.btn_Close.UseVisualStyleBackColor = true;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);

            // AboutDialog
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(392, 262);
            this.Controls.Add(this.pb_Icon);
            this.Controls.Add(this.lbl_Title);
            this.Controls.Add(this.lbl_ReleaseChannel);
            this.Controls.Add(this.lbl_Developer);
            this.Controls.Add(this.lbl_VersionBuilt);
            this.Controls.Add(this.lbl_Copyright);
            this.Controls.Add(this.linkLabel_HomePage);
            this.Controls.Add(this.linkLabel_GitHub);
            this.Controls.Add(this.cb_KeepPlayingMusic);
            this.Controls.Add(this.btn_Close);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AboutDialog_FormClosing);

            ((System.ComponentModel.ISupportInitialize)(this.pb_Icon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.PictureBox pb_Icon;
        private System.Windows.Forms.Label lbl_Title;
        private System.Windows.Forms.Label lbl_ReleaseChannel;
        private System.Windows.Forms.Label lbl_Developer;
        private System.Windows.Forms.Label lbl_VersionBuilt;
        private System.Windows.Forms.Label lbl_Copyright;
        private System.Windows.Forms.LinkLabel linkLabel_HomePage;
        private System.Windows.Forms.LinkLabel linkLabel_GitHub;
        private System.Windows.Forms.CheckBox cb_KeepPlayingMusic;
        private System.Windows.Forms.Button btn_Close;
    }
}
