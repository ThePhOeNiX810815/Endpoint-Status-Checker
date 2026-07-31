namespace EndpointChecker
{
    partial class CloudflareBypassSettingsDialog
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.label_Method = new System.Windows.Forms.Label();
            this.comboBox_BypassMethod = new System.Windows.Forms.ComboBox();
            this.panel_FlareSolverr = new System.Windows.Forms.Panel();
            this.lbl_FsInfo = new System.Windows.Forms.Label();
            this.label_FsUrl = new System.Windows.Forms.Label();
            this.textBox_FlareSolverrUrl = new System.Windows.Forms.TextBox();
            this.lbl_PlaywrightStatus = new System.Windows.Forms.Label();
            this.btn_OK = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.panel_FlareSolverr.SuspendLayout();
            this.SuspendLayout();

            // label_Method
            this.label_Method.AutoSize = true;
            this.label_Method.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label_Method.Location = new System.Drawing.Point(12, 18);
            this.label_Method.Text = "Bypass Method:";

            // comboBox_BypassMethod
            this.comboBox_BypassMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_BypassMethod.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.comboBox_BypassMethod.Location = new System.Drawing.Point(130, 14);
            this.comboBox_BypassMethod.Size = new System.Drawing.Size(305, 23);
            this.comboBox_BypassMethod.SelectedIndexChanged += new System.EventHandler(this.comboBox_BypassMethod_SelectedIndexChanged);

            // panel_FlareSolverr
            this.panel_FlareSolverr.BackColor = System.Drawing.Color.FromArgb(245, 255, 248);
            this.panel_FlareSolverr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_FlareSolverr.Controls.Add(this.lbl_FsInfo);
            this.panel_FlareSolverr.Controls.Add(this.label_FsUrl);
            this.panel_FlareSolverr.Controls.Add(this.textBox_FlareSolverrUrl);
            this.panel_FlareSolverr.Location = new System.Drawing.Point(12, 50);
            this.panel_FlareSolverr.Size = new System.Drawing.Size(423, 110);
            this.panel_FlareSolverr.Visible = false;

            // lbl_FsInfo
            this.lbl_FsInfo.AutoSize = false;
            this.lbl_FsInfo.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lbl_FsInfo.Location = new System.Drawing.Point(8, 8);
            this.lbl_FsInfo.Size = new System.Drawing.Size(405, 50);
            this.lbl_FsInfo.Text =
                "FlareSolverr is a standalone proxy that solves Cloudflare challenges externally.\r\n" +
                "Download & run it from: github.com/FlareSolverr/FlareSolverr\r\n" +
                "Default port: 8191. Must be running before scans start.";

            // label_FsUrl
            this.label_FsUrl.AutoSize = true;
            this.label_FsUrl.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.label_FsUrl.Location = new System.Drawing.Point(8, 72);
            this.label_FsUrl.Text = "FlareSolverr URL:";

            // textBox_FlareSolverrUrl
            this.textBox_FlareSolverrUrl.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.textBox_FlareSolverrUrl.Location = new System.Drawing.Point(130, 69);
            this.textBox_FlareSolverrUrl.Size = new System.Drawing.Size(285, 23);
            this.textBox_FlareSolverrUrl.Text = "http://localhost:8191";

            // lbl_PlaywrightStatus
            this.lbl_PlaywrightStatus.AutoSize = false;
            this.lbl_PlaywrightStatus.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lbl_PlaywrightStatus.Location = new System.Drawing.Point(12, 50);
            this.lbl_PlaywrightStatus.Size = new System.Drawing.Size(423, 40);
            this.lbl_PlaywrightStatus.Text = string.Empty;
            this.lbl_PlaywrightStatus.Visible = false;

            // btn_OK
            this.btn_OK.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btn_OK.Location = new System.Drawing.Point(268, 175);
            this.btn_OK.Size = new System.Drawing.Size(80, 30);
            this.btn_OK.Text = "OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);

            // btn_Cancel
            this.btn_Cancel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btn_Cancel.Location = new System.Drawing.Point(355, 175);
            this.btn_Cancel.Size = new System.Drawing.Size(80, 30);
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);

            // CloudflareBypassSettingsDialog
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(447, 218);
            this.Controls.Add(this.label_Method);
            this.Controls.Add(this.comboBox_BypassMethod);
            this.Controls.Add(this.panel_FlareSolverr);
            this.Controls.Add(this.lbl_PlaywrightStatus);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.btn_Cancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cloudflare Bypass Settings";

            this.panel_FlareSolverr.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label label_Method;
        private System.Windows.Forms.ComboBox comboBox_BypassMethod;
        private System.Windows.Forms.Panel panel_FlareSolverr;
        private System.Windows.Forms.Label lbl_FsInfo;
        private System.Windows.Forms.Label label_FsUrl;
        private System.Windows.Forms.TextBox textBox_FlareSolverrUrl;
        private System.Windows.Forms.Label lbl_PlaywrightStatus;
        private System.Windows.Forms.Button btn_OK;
        private System.Windows.Forms.Button btn_Cancel;
    }
}
