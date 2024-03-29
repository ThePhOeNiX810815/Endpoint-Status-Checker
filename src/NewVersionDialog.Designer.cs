﻿namespace EndpointChecker
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
            this.lbl_NewVersion = new System.Windows.Forms.Label();
            this.lbl_NewVersionDetail = new System.Windows.Forms.Label();
            this.lbl_ReleaseNotes = new System.Windows.Forms.Label();
            this.btn_SkipThisVersion = new System.Windows.Forms.Button();
            this.btn_InstallUpdate = new System.Windows.Forms.Button();
            this.btn_RemindMeLater = new System.Windows.Forms.Button();
            this.rtb_ReleaseNotes = new System.Windows.Forms.RichTextBox();
            this.cb_FutureAutoUpdate = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lbl_NewVersion
            // 
            this.lbl_NewVersion.BackColor = System.Drawing.Color.Transparent;
            this.lbl_NewVersion.Font = new System.Drawing.Font("Segoe UI Semibold", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_NewVersion.ForeColor = System.Drawing.Color.LightGreen;
            this.lbl_NewVersion.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbl_NewVersion.Location = new System.Drawing.Point(9, 5);
            this.lbl_NewVersion.Name = "lbl_NewVersion";
            this.lbl_NewVersion.Size = new System.Drawing.Size(708, 31);
            this.lbl_NewVersion.TabIndex = 1;
            this.lbl_NewVersion.Text = "A new version of <AppName> is available!";
            this.lbl_NewVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_NewVersion.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Controls_MouseDown);
            // 
            // lbl_NewVersionDetail
            // 
            this.lbl_NewVersionDetail.BackColor = System.Drawing.Color.Transparent;
            this.lbl_NewVersionDetail.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_NewVersionDetail.ForeColor = System.Drawing.Color.SandyBrown;
            this.lbl_NewVersionDetail.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbl_NewVersionDetail.Location = new System.Drawing.Point(9, 39);
            this.lbl_NewVersionDetail.Name = "lbl_NewVersionDetail";
            this.lbl_NewVersionDetail.Size = new System.Drawing.Size(708, 31);
            this.lbl_NewVersionDetail.TabIndex = 2;
            this.lbl_NewVersionDetail.Text = "<AppName> <UpdateVersion> is now available. You have version <AppVersion>.";
            this.lbl_NewVersionDetail.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_NewVersionDetail.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Controls_MouseDown);
            // 
            // lbl_ReleaseNotes
            // 
            this.lbl_ReleaseNotes.BackColor = System.Drawing.Color.Transparent;
            this.lbl_ReleaseNotes.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_ReleaseNotes.ForeColor = System.Drawing.Color.White;
            this.lbl_ReleaseNotes.Location = new System.Drawing.Point(9, 101);
            this.lbl_ReleaseNotes.Name = "lbl_ReleaseNotes";
            this.lbl_ReleaseNotes.Size = new System.Drawing.Size(708, 22);
            this.lbl_ReleaseNotes.TabIndex = 3;
            this.lbl_ReleaseNotes.Text = "Release Notes";
            this.lbl_ReleaseNotes.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_ReleaseNotes.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Controls_MouseDown);
            // 
            // btn_SkipThisVersion
            // 
            this.btn_SkipThisVersion.BackColor = System.Drawing.Color.Gray;
            this.btn_SkipThisVersion.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_SkipThisVersion.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_SkipThisVersion.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_SkipThisVersion.Location = new System.Drawing.Point(12, 621);
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
            this.btn_InstallUpdate.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_InstallUpdate.Location = new System.Drawing.Point(537, 621);
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
            this.btn_RemindMeLater.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_RemindMeLater.Location = new System.Drawing.Point(274, 621);
            this.btn_RemindMeLater.Name = "btn_RemindMeLater";
            this.btn_RemindMeLater.Size = new System.Drawing.Size(180, 29);
            this.btn_RemindMeLater.TabIndex = 7;
            this.btn_RemindMeLater.Text = "Remind Me Later";
            this.btn_RemindMeLater.UseVisualStyleBackColor = false;
            this.btn_RemindMeLater.Click += new System.EventHandler(this.btn_RemindMeLater_Click);
            // 
            // rtb_ReleaseNotes
            // 
            this.rtb_ReleaseNotes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.rtb_ReleaseNotes.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtb_ReleaseNotes.Cursor = System.Windows.Forms.Cursors.Default;
            this.rtb_ReleaseNotes.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtb_ReleaseNotes.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.rtb_ReleaseNotes.Location = new System.Drawing.Point(12, 126);
            this.rtb_ReleaseNotes.Name = "rtb_ReleaseNotes";
            this.rtb_ReleaseNotes.ReadOnly = true;
            this.rtb_ReleaseNotes.Size = new System.Drawing.Size(705, 435);
            this.rtb_ReleaseNotes.TabIndex = 9;
            this.rtb_ReleaseNotes.Text = "";
            this.rtb_ReleaseNotes.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtb_ReleaseNotes_LinkClicked);
            // 
            // cb_FutureAutoUpdate
            // 
            this.cb_FutureAutoUpdate.AutoSize = true;
            this.cb_FutureAutoUpdate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cb_FutureAutoUpdate.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_FutureAutoUpdate.ForeColor = System.Drawing.Color.White;
            this.cb_FutureAutoUpdate.Location = new System.Drawing.Point(12, 567);
            this.cb_FutureAutoUpdate.Name = "cb_FutureAutoUpdate";
            this.cb_FutureAutoUpdate.Size = new System.Drawing.Size(427, 24);
            this.cb_FutureAutoUpdate.TabIndex = 10;
            this.cb_FutureAutoUpdate.Text = "Automatically download and install updates in the future";
            this.cb_FutureAutoUpdate.UseVisualStyleBackColor = true;
            // 
            // NewVersionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(729, 662);
            this.Controls.Add(this.cb_FutureAutoUpdate);
            this.Controls.Add(this.rtb_ReleaseNotes);
            this.Controls.Add(this.btn_RemindMeLater);
            this.Controls.Add(this.btn_InstallUpdate);
            this.Controls.Add(this.btn_SkipThisVersion);
            this.Controls.Add(this.lbl_ReleaseNotes);
            this.Controls.Add(this.lbl_NewVersionDetail);
            this.Controls.Add(this.lbl_NewVersion);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(729, 662);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(729, 662);
            this.Name = "NewVersionDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Software Update";
            this.TopMost = true;
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Controls_MouseDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.Label lbl_NewVersion;
        public System.Windows.Forms.Label lbl_NewVersionDetail;
        public System.Windows.Forms.Label lbl_ReleaseNotes;
        public System.Windows.Forms.Button btn_SkipThisVersion;
        public System.Windows.Forms.Button btn_InstallUpdate;
        public System.Windows.Forms.Button btn_RemindMeLater;
        public System.Windows.Forms.RichTextBox rtb_ReleaseNotes;
        public System.Windows.Forms.CheckBox cb_FutureAutoUpdate;
    }
}