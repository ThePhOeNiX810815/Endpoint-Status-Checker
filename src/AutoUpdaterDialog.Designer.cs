namespace EndpointChecker
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AutoUpdaterDialog));
            this.lbl_Title = new System.Windows.Forms.Label();
            this.lbl_Version = new System.Windows.Forms.Label();
            this.pb_Progress = new System.Windows.Forms.PictureBox();
            this.lbl_Progress = new System.Windows.Forms.Label();
            this.BW_Update = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Progress)).BeginInit();
            this.SuspendLayout();
            // 
            // lbl_Title
            // 
            this.lbl_Title.BackColor = System.Drawing.Color.Transparent;
            this.lbl_Title.Font = new System.Drawing.Font("Century Gothic", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Title.ForeColor = System.Drawing.Color.White;
            this.lbl_Title.Location = new System.Drawing.Point(12, 6);
            this.lbl_Title.Name = "lbl_Title";
            this.lbl_Title.Size = new System.Drawing.Size(439, 41);
            this.lbl_Title.TabIndex = 0;
            this.lbl_Title.Text = "Endpoint Status Checker AutoUpdate";
            this.lbl_Title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_Version
            // 
            this.lbl_Version.BackColor = System.Drawing.Color.Transparent;
            this.lbl_Version.Font = new System.Drawing.Font("Century Gothic", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Version.ForeColor = System.Drawing.Color.DodgerBlue;
            this.lbl_Version.Location = new System.Drawing.Point(12, 47);
            this.lbl_Version.Name = "lbl_Version";
            this.lbl_Version.Size = new System.Drawing.Size(439, 41);
            this.lbl_Version.TabIndex = 1;
            this.lbl_Version.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pb_Progress
            // 
            this.pb_Progress.BackColor = System.Drawing.Color.Transparent;
            this.pb_Progress.Image = global::EndpointChecker.Properties.Resources.updater_Progress;
            this.pb_Progress.Location = new System.Drawing.Point(181, 93);
            this.pb_Progress.Name = "pb_Progress";
            this.pb_Progress.Size = new System.Drawing.Size(100, 94);
            this.pb_Progress.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_Progress.TabIndex = 2;
            this.pb_Progress.TabStop = false;
            // 
            // lbl_Progress
            // 
            this.lbl_Progress.BackColor = System.Drawing.Color.Transparent;
            this.lbl_Progress.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Progress.ForeColor = System.Drawing.Color.PaleGreen;
            this.lbl_Progress.Location = new System.Drawing.Point(12, 203);
            this.lbl_Progress.Name = "lbl_Progress";
            this.lbl_Progress.Size = new System.Drawing.Size(439, 31);
            this.lbl_Progress.TabIndex = 3;
            this.lbl_Progress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BW_Update
            // 
            this.BW_Update.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bw_Update_DoWork);
            this.BW_Update.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bw_Update_RunWorkerCompleted);
            // 
            // AutoUpdaterDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::EndpointChecker.Properties.Resources.updater_Background;
            this.ClientSize = new System.Drawing.Size(463, 243);
            this.Controls.Add(this.lbl_Progress);
            this.Controls.Add(this.pb_Progress);
            this.Controls.Add(this.lbl_Version);
            this.Controls.Add(this.lbl_Title);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(463, 243);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(463, 243);
            this.Name = "AutoUpdaterDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AutoUpdaterDialog";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pb_Progress)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label lbl_Title;
        public System.Windows.Forms.Label lbl_Version;
        public System.Windows.Forms.Label lbl_Progress;
        public System.Windows.Forms.PictureBox pb_Progress;
        public System.ComponentModel.BackgroundWorker BW_Update;
    }
}