﻿namespace EndpointChecker
{
    partial class AutoUpdaterDialog
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AutoUpdaterDialog));
            this.BW_Update = new System.ComponentModel.BackgroundWorker();
            this.lbl_Name = new System.Windows.Forms.Label();
            this.lbl_Copyright = new System.Windows.Forms.Label();
            this.lbl_Progress = new System.Windows.Forms.Label();
            this.lbl_UpdateVersion = new System.Windows.Forms.Label();
            this.TIMER_FadeOutAndClose = new System.Windows.Forms.Timer(this.components);
            this.lbl_UpdateStatus = new System.Windows.Forms.Label();
            this.lbl_UpdateStatus_Wait = new System.Windows.Forms.Label();
            this.lbl_ReleaseDate = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // BW_Update
            // 
            this.BW_Update.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bw_Update_DoWork);
            this.BW_Update.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bw_Update_RunWorkerCompleted);
            // 
            // lbl_Name
            // 
            this.lbl_Name.BackColor = System.Drawing.Color.Transparent;
            this.lbl_Name.Font = new System.Drawing.Font("Agency FB", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Name.ForeColor = System.Drawing.Color.LightGreen;
            this.lbl_Name.Location = new System.Drawing.Point(48, 274);
            this.lbl_Name.Name = "lbl_Name";
            this.lbl_Name.Size = new System.Drawing.Size(509, 36);
            this.lbl_Name.TabIndex = 4;
            this.lbl_Name.Text = "<< APP NAME LABEL >>";
            this.lbl_Name.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_Name.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Controls_MouseDown);
            // 
            // lbl_Copyright
            // 
            this.lbl_Copyright.BackColor = System.Drawing.Color.Transparent;
            this.lbl_Copyright.Font = new System.Drawing.Font("Agency FB", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Copyright.ForeColor = System.Drawing.Color.Orange;
            this.lbl_Copyright.Location = new System.Drawing.Point(694, 349);
            this.lbl_Copyright.Name = "lbl_Copyright";
            this.lbl_Copyright.Size = new System.Drawing.Size(299, 25);
            this.lbl_Copyright.TabIndex = 5;
            this.lbl_Copyright.Text = "<< COPYRIGHT LABEL >>";
            this.lbl_Copyright.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lbl_Copyright.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Controls_MouseDown);
            // 
            // lbl_Progress
            // 
            this.lbl_Progress.BackColor = System.Drawing.Color.Transparent;
            this.lbl_Progress.Font = new System.Drawing.Font("Agency FB", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Progress.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.lbl_Progress.Location = new System.Drawing.Point(11, 313);
            this.lbl_Progress.Name = "lbl_Progress";
            this.lbl_Progress.Size = new System.Drawing.Size(583, 34);
            this.lbl_Progress.TabIndex = 6;
            this.lbl_Progress.Text = "Initializing Update ...";
            this.lbl_Progress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_Progress.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Controls_MouseDown);
            // 
            // lbl_UpdateVersion
            // 
            this.lbl_UpdateVersion.BackColor = System.Drawing.Color.Transparent;
            this.lbl_UpdateVersion.Font = new System.Drawing.Font("Agency FB", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_UpdateVersion.ForeColor = System.Drawing.Color.PaleGoldenrod;
            this.lbl_UpdateVersion.Location = new System.Drawing.Point(394, 19);
            this.lbl_UpdateVersion.Name = "lbl_UpdateVersion";
            this.lbl_UpdateVersion.Size = new System.Drawing.Size(339, 35);
            this.lbl_UpdateVersion.TabIndex = 7;
            this.lbl_UpdateVersion.Text = "<< UPDATE VERSION >>";
            this.lbl_UpdateVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_UpdateVersion.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Controls_MouseDown);
            // 
            // TIMER_FadeOutAndClose
            // 
            this.TIMER_FadeOutAndClose.Interval = 10;
            this.TIMER_FadeOutAndClose.Tick += new System.EventHandler(this.TIMER_FadeOutAndClose_Tick);
            // 
            // lbl_UpdateStatus
            // 
            this.lbl_UpdateStatus.BackColor = System.Drawing.Color.Transparent;
            this.lbl_UpdateStatus.Font = new System.Drawing.Font("Agency FB", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_UpdateStatus.ForeColor = System.Drawing.Color.OrangeRed;
            this.lbl_UpdateStatus.Location = new System.Drawing.Point(0, 20);
            this.lbl_UpdateStatus.Name = "lbl_UpdateStatus";
            this.lbl_UpdateStatus.Size = new System.Drawing.Size(326, 33);
            this.lbl_UpdateStatus.TabIndex = 8;
            this.lbl_UpdateStatus.Text = "UPDATE IN PROGRESS";
            this.lbl_UpdateStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_UpdateStatus_Wait
            // 
            this.lbl_UpdateStatus_Wait.BackColor = System.Drawing.Color.Transparent;
            this.lbl_UpdateStatus_Wait.Font = new System.Drawing.Font("Agency FB", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_UpdateStatus_Wait.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.lbl_UpdateStatus_Wait.Location = new System.Drawing.Point(121, 51);
            this.lbl_UpdateStatus_Wait.Name = "lbl_UpdateStatus_Wait";
            this.lbl_UpdateStatus_Wait.Size = new System.Drawing.Size(85, 21);
            this.lbl_UpdateStatus_Wait.TabIndex = 9;
            this.lbl_UpdateStatus_Wait.Text = "plaese wait...";
            this.lbl_UpdateStatus_Wait.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_ReleaseDate
            // 
            this.lbl_ReleaseDate.BackColor = System.Drawing.Color.Transparent;
            this.lbl_ReleaseDate.Font = new System.Drawing.Font("Agency FB", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_ReleaseDate.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.lbl_ReleaseDate.Location = new System.Drawing.Point(476, 49);
            this.lbl_ReleaseDate.Name = "lbl_ReleaseDate";
            this.lbl_ReleaseDate.Size = new System.Drawing.Size(175, 23);
            this.lbl_ReleaseDate.TabIndex = 10;
            this.lbl_ReleaseDate.Text = "<< RELEASE DATE >>";
            this.lbl_ReleaseDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AutoUpdaterDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::EndpointChecker.Properties.Resources.splash_Background_3;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1000, 378);
            this.Controls.Add(this.lbl_UpdateStatus_Wait);
            this.Controls.Add(this.lbl_ReleaseDate);
            this.Controls.Add(this.lbl_UpdateVersion);
            this.Controls.Add(this.lbl_UpdateStatus);
            this.Controls.Add(this.lbl_Progress);
            this.Controls.Add(this.lbl_Copyright);
            this.Controls.Add(this.lbl_Name);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1000, 378);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1000, 378);
            this.Name = "AutoUpdaterDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AutoUpdaterDialog";
            this.TopMost = true;
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Controls_MouseDown);
            this.ResumeLayout(false);

        }

        #endregion
        public System.ComponentModel.BackgroundWorker BW_Update;
        public System.Windows.Forms.Label lbl_Name;
        public System.Windows.Forms.Label lbl_Copyright;
        public System.Windows.Forms.Label lbl_Progress;
        public System.Windows.Forms.Label lbl_UpdateVersion;
        public System.Windows.Forms.Timer TIMER_FadeOutAndClose;
        public System.Windows.Forms.Label lbl_UpdateStatus;
        public System.Windows.Forms.Label lbl_UpdateStatus_Wait;
        public System.Windows.Forms.Label lbl_ReleaseDate;
    }
}