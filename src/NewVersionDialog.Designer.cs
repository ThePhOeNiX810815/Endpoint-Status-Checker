namespace EndpointChecker
{
    partial class NewVersionDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewVersionDialog));
            this.pb_UpdateLogo = new System.Windows.Forms.PictureBox();
            this.lbl_NewVersion = new System.Windows.Forms.Label();
            this.lbl_NewVersionDetail = new System.Windows.Forms.Label();
            this.lbl_ReleaseNotes = new System.Windows.Forms.Label();
            this.btn_SkipThisVersion = new System.Windows.Forms.Button();
            this.btn_InstallUpdate = new System.Windows.Forms.Button();
            this.btn_RemindMeLater = new System.Windows.Forms.Button();
            this.lbl_ReleaseNotesBackground = new System.Windows.Forms.Label();
            this.rtb_ReleaseNotes = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pb_UpdateLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // pb_UpdateLogo
            // 
            this.pb_UpdateLogo.Image = global::EndpointChecker.Properties.Resources.newUpdateLogo;
            this.pb_UpdateLogo.Location = new System.Drawing.Point(2, 1);
            this.pb_UpdateLogo.Name = "pb_UpdateLogo";
            this.pb_UpdateLogo.Size = new System.Drawing.Size(120, 120);
            this.pb_UpdateLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_UpdateLogo.TabIndex = 0;
            this.pb_UpdateLogo.TabStop = false;
            // 
            // lbl_NewVersion
            // 
            this.lbl_NewVersion.BackColor = System.Drawing.Color.Transparent;
            this.lbl_NewVersion.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_NewVersion.ForeColor = System.Drawing.Color.White;
            this.lbl_NewVersion.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbl_NewVersion.Location = new System.Drawing.Point(123, 11);
            this.lbl_NewVersion.Name = "lbl_NewVersion";
            this.lbl_NewVersion.Size = new System.Drawing.Size(708, 31);
            this.lbl_NewVersion.TabIndex = 1;
            this.lbl_NewVersion.Text = "A new version of <AppName> is available!";
            this.lbl_NewVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_NewVersionDetail
            // 
            this.lbl_NewVersionDetail.BackColor = System.Drawing.Color.Transparent;
            this.lbl_NewVersionDetail.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_NewVersionDetail.ForeColor = System.Drawing.Color.White;
            this.lbl_NewVersionDetail.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbl_NewVersionDetail.Location = new System.Drawing.Point(123, 45);
            this.lbl_NewVersionDetail.Name = "lbl_NewVersionDetail";
            this.lbl_NewVersionDetail.Size = new System.Drawing.Size(708, 31);
            this.lbl_NewVersionDetail.TabIndex = 2;
            this.lbl_NewVersionDetail.Text = "<AppName> <UpdateVersion> is now available. You have version <AppVersion>. Would " +
    "you like to download it now ?";
            this.lbl_NewVersionDetail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_ReleaseNotes
            // 
            this.lbl_ReleaseNotes.BackColor = System.Drawing.Color.Transparent;
            this.lbl_ReleaseNotes.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_ReleaseNotes.ForeColor = System.Drawing.Color.White;
            this.lbl_ReleaseNotes.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbl_ReleaseNotes.Location = new System.Drawing.Point(123, 79);
            this.lbl_ReleaseNotes.Name = "lbl_ReleaseNotes";
            this.lbl_ReleaseNotes.Size = new System.Drawing.Size(708, 31);
            this.lbl_ReleaseNotes.TabIndex = 3;
            this.lbl_ReleaseNotes.Text = "Release Notes:";
            this.lbl_ReleaseNotes.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btn_SkipThisVersion
            // 
            this.btn_SkipThisVersion.BackColor = System.Drawing.Color.Gray;
            this.btn_SkipThisVersion.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_SkipThisVersion.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_SkipThisVersion.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_SkipThisVersion.Location = new System.Drawing.Point(126, 490);
            this.btn_SkipThisVersion.Name = "btn_SkipThisVersion";
            this.btn_SkipThisVersion.Size = new System.Drawing.Size(180, 29);
            this.btn_SkipThisVersion.TabIndex = 5;
            this.btn_SkipThisVersion.Text = "Skip This Version";
            this.btn_SkipThisVersion.UseVisualStyleBackColor = false;
            this.btn_SkipThisVersion.Click += new System.EventHandler(this.btn_SkipThisVersion_Click);
            // 
            // btn_InstallUpdate
            // 
            this.btn_InstallUpdate.BackColor = System.Drawing.Color.Gray;
            this.btn_InstallUpdate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_InstallUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_InstallUpdate.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_InstallUpdate.Location = new System.Drawing.Point(651, 490);
            this.btn_InstallUpdate.Name = "btn_InstallUpdate";
            this.btn_InstallUpdate.Size = new System.Drawing.Size(180, 29);
            this.btn_InstallUpdate.TabIndex = 6;
            this.btn_InstallUpdate.Text = "Install Update";
            this.btn_InstallUpdate.UseVisualStyleBackColor = false;
            this.btn_InstallUpdate.Click += new System.EventHandler(this.btn_InstallUpdate_Click);
            // 
            // btn_RemindMeLater
            // 
            this.btn_RemindMeLater.BackColor = System.Drawing.Color.Gray;
            this.btn_RemindMeLater.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_RemindMeLater.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_RemindMeLater.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_RemindMeLater.Location = new System.Drawing.Point(455, 490);
            this.btn_RemindMeLater.Name = "btn_RemindMeLater";
            this.btn_RemindMeLater.Size = new System.Drawing.Size(180, 29);
            this.btn_RemindMeLater.TabIndex = 7;
            this.btn_RemindMeLater.Text = "Remind Me Later";
            this.btn_RemindMeLater.UseVisualStyleBackColor = false;
            this.btn_RemindMeLater.Click += new System.EventHandler(this.btn_RemindMeLater_Click);
            // 
            // lbl_ReleaseNotesBackground
            // 
            this.lbl_ReleaseNotesBackground.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.lbl_ReleaseNotesBackground.Cursor = System.Windows.Forms.Cursors.Default;
            this.lbl_ReleaseNotesBackground.Location = new System.Drawing.Point(126, 111);
            this.lbl_ReleaseNotesBackground.Name = "lbl_ReleaseNotesBackground";
            this.lbl_ReleaseNotesBackground.Size = new System.Drawing.Size(705, 366);
            this.lbl_ReleaseNotesBackground.TabIndex = 8;
            // 
            // rtb_ReleaseNotes
            // 
            this.rtb_ReleaseNotes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.rtb_ReleaseNotes.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtb_ReleaseNotes.Cursor = System.Windows.Forms.Cursors.Default;
            this.rtb_ReleaseNotes.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtb_ReleaseNotes.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.rtb_ReleaseNotes.Location = new System.Drawing.Point(156, 136);
            this.rtb_ReleaseNotes.Name = "rtb_ReleaseNotes";
            this.rtb_ReleaseNotes.ReadOnly = true;
            this.rtb_ReleaseNotes.Size = new System.Drawing.Size(648, 316);
            this.rtb_ReleaseNotes.TabIndex = 9;
            this.rtb_ReleaseNotes.Text = "";
            this.rtb_ReleaseNotes.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtb_ReleaseNotes_LinkClicked);
            // 
            // NewVersionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(843, 534);
            this.Controls.Add(this.rtb_ReleaseNotes);
            this.Controls.Add(this.btn_RemindMeLater);
            this.Controls.Add(this.btn_InstallUpdate);
            this.Controls.Add(this.btn_SkipThisVersion);
            this.Controls.Add(this.lbl_ReleaseNotes);
            this.Controls.Add(this.lbl_NewVersionDetail);
            this.Controls.Add(this.lbl_NewVersion);
            this.Controls.Add(this.pb_UpdateLogo);
            this.Controls.Add(this.lbl_ReleaseNotesBackground);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewVersionDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Software Update";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pb_UpdateLogo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.PictureBox pb_UpdateLogo;
        public System.Windows.Forms.Label lbl_NewVersion;
        public System.Windows.Forms.Label lbl_NewVersionDetail;
        public System.Windows.Forms.Label lbl_ReleaseNotes;
        public System.Windows.Forms.Button btn_SkipThisVersion;
        public System.Windows.Forms.Button btn_InstallUpdate;
        public System.Windows.Forms.Button btn_RemindMeLater;
        public System.Windows.Forms.Label lbl_ReleaseNotesBackground;
        public System.Windows.Forms.RichTextBox rtb_ReleaseNotes;
    }
}