
namespace EndpointChecker
{
    partial class SplashScreen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashScreen));
            this.TIMER_FadeOutAndClose = new System.Windows.Forms.Timer(this.components);
            this.TIMER_StartFading = new System.Windows.Forms.Timer(this.components);
            this.lbl_Name = new System.Windows.Forms.Label();
            this.lbl_Copyright = new System.Windows.Forms.Label();
            this.lbl_Version = new System.Windows.Forms.Label();
            this.lbl_Build = new System.Windows.Forms.Label();
            this.lbl_ReleaseType = new System.Windows.Forms.Label();
            this.pb_CloseDialog = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pb_CloseDialog)).BeginInit();
            this.SuspendLayout();
            // 
            // TIMER_FadeOutAndClose
            // 
            this.TIMER_FadeOutAndClose.Interval = 10;
            this.TIMER_FadeOutAndClose.Tick += new System.EventHandler(this.TIMER_FadeOutAndClose_Tick);
            // 
            // TIMER_StartFading
            // 
            this.TIMER_StartFading.Interval = 5000;
            this.TIMER_StartFading.Tick += new System.EventHandler(this.TIMER_StartFading_Tick);
            // 
            // lbl_Name
            // 
            this.lbl_Name.BackColor = System.Drawing.Color.Transparent;
            this.lbl_Name.Font = new System.Drawing.Font("Arial Rounded MT Bold", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Name.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lbl_Name.Location = new System.Drawing.Point(12, 33);
            this.lbl_Name.Name = "lbl_Name";
            this.lbl_Name.Size = new System.Drawing.Size(631, 41);
            this.lbl_Name.TabIndex = 0;
            this.lbl_Name.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_Copyright
            // 
            this.lbl_Copyright.BackColor = System.Drawing.Color.Transparent;
            this.lbl_Copyright.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Copyright.ForeColor = System.Drawing.Color.Black;
            this.lbl_Copyright.Location = new System.Drawing.Point(7, 192);
            this.lbl_Copyright.Name = "lbl_Copyright";
            this.lbl_Copyright.Size = new System.Drawing.Size(299, 25);
            this.lbl_Copyright.TabIndex = 1;
            this.lbl_Copyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_Version
            // 
            this.lbl_Version.BackColor = System.Drawing.Color.Transparent;
            this.lbl_Version.Font = new System.Drawing.Font("Century Gothic", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Version.ForeColor = System.Drawing.Color.Black;
            this.lbl_Version.Location = new System.Drawing.Point(12, 84);
            this.lbl_Version.Name = "lbl_Version";
            this.lbl_Version.Size = new System.Drawing.Size(631, 34);
            this.lbl_Version.TabIndex = 2;
            this.lbl_Version.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_Build
            // 
            this.lbl_Build.BackColor = System.Drawing.Color.Transparent;
            this.lbl_Build.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Build.ForeColor = System.Drawing.Color.Black;
            this.lbl_Build.Location = new System.Drawing.Point(12, 120);
            this.lbl_Build.Name = "lbl_Build";
            this.lbl_Build.Size = new System.Drawing.Size(631, 30);
            this.lbl_Build.TabIndex = 4;
            this.lbl_Build.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_ReleaseType
            // 
            this.lbl_ReleaseType.BackColor = System.Drawing.Color.LightSteelBlue;
            this.lbl_ReleaseType.Font = new System.Drawing.Font("Segoe UI Semibold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_ReleaseType.ForeColor = System.Drawing.Color.Black;
            this.lbl_ReleaseType.Location = new System.Drawing.Point(176, 155);
            this.lbl_ReleaseType.Name = "lbl_ReleaseType";
            this.lbl_ReleaseType.Size = new System.Drawing.Size(302, 25);
            this.lbl_ReleaseType.TabIndex = 5;
            this.lbl_ReleaseType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_ReleaseType.Visible = false;
            // 
            // pb_CloseDialog
            // 
            this.pb_CloseDialog.BackColor = System.Drawing.Color.Transparent;
            this.pb_CloseDialog.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pb_CloseDialog.Image = global::EndpointChecker.Properties.Resources.close;
            this.pb_CloseDialog.Location = new System.Drawing.Point(618, 5);
            this.pb_CloseDialog.Name = "pb_CloseDialog";
            this.pb_CloseDialog.Size = new System.Drawing.Size(32, 32);
            this.pb_CloseDialog.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_CloseDialog.TabIndex = 6;
            this.pb_CloseDialog.TabStop = false;
            this.pb_CloseDialog.Click += new System.EventHandler(this.pb_CloseDialog_Click);
            // 
            // SplashScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(655, 223);
            this.Controls.Add(this.pb_CloseDialog);
            this.Controls.Add(this.lbl_ReleaseType);
            this.Controls.Add(this.lbl_Build);
            this.Controls.Add(this.lbl_Version);
            this.Controls.Add(this.lbl_Copyright);
            this.Controls.Add(this.lbl_Name);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Transparent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SplashScreen";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pb_CloseDialog)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Timer TIMER_FadeOutAndClose;
        public System.Windows.Forms.Timer TIMER_StartFading;
        public System.Windows.Forms.Label lbl_Name;
        public System.Windows.Forms.Label lbl_Copyright;
        public System.Windows.Forms.Label lbl_Version;
        public System.Windows.Forms.Label lbl_Build;
        public System.Windows.Forms.Label lbl_ReleaseType;
        public System.Windows.Forms.PictureBox pb_CloseDialog;
    }
}