
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
            this.SuspendLayout();
            // 
            // TIMER_FadeOutAndClose
            // 
            this.TIMER_FadeOutAndClose.Interval = 10;
            this.TIMER_FadeOutAndClose.Tick += new System.EventHandler(this.TIMER_FadeOutAndClose_Tick);
            // 
            // TIMER_StartFading
            // 
            this.TIMER_StartFading.Enabled = true;
            this.TIMER_StartFading.Interval = 5000;
            this.TIMER_StartFading.Tick += new System.EventHandler(this.TIMER_StartFading_Tick);
            // 
            // lbl_Name
            // 
            this.lbl_Name.BackColor = System.Drawing.Color.Transparent;
            this.lbl_Name.Font = new System.Drawing.Font("Arial Rounded MT Bold", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Name.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lbl_Name.Location = new System.Drawing.Point(12, 44);
            this.lbl_Name.Name = "lbl_Name";
            this.lbl_Name.Size = new System.Drawing.Size(631, 41);
            this.lbl_Name.TabIndex = 0;
            this.lbl_Name.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_Copyright
            // 
            this.lbl_Copyright.BackColor = System.Drawing.Color.Transparent;
            this.lbl_Copyright.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Copyright.ForeColor = System.Drawing.Color.Black;
            this.lbl_Copyright.Location = new System.Drawing.Point(4, 203);
            this.lbl_Copyright.Name = "lbl_Copyright";
            this.lbl_Copyright.Size = new System.Drawing.Size(299, 17);
            this.lbl_Copyright.TabIndex = 1;
            this.lbl_Copyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_Version
            // 
            this.lbl_Version.BackColor = System.Drawing.Color.Transparent;
            this.lbl_Version.Font = new System.Drawing.Font("Century", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Version.ForeColor = System.Drawing.Color.Black;
            this.lbl_Version.Location = new System.Drawing.Point(12, 95);
            this.lbl_Version.Name = "lbl_Version";
            this.lbl_Version.Size = new System.Drawing.Size(631, 26);
            this.lbl_Version.TabIndex = 2;
            this.lbl_Version.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_Build
            // 
            this.lbl_Build.BackColor = System.Drawing.Color.Transparent;
            this.lbl_Build.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Build.ForeColor = System.Drawing.Color.Black;
            this.lbl_Build.Location = new System.Drawing.Point(12, 123);
            this.lbl_Build.Name = "lbl_Build";
            this.lbl_Build.Size = new System.Drawing.Size(631, 30);
            this.lbl_Build.TabIndex = 4;
            this.lbl_Build.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SplashScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(655, 223);
            this.Controls.Add(this.lbl_Build);
            this.Controls.Add(this.lbl_Version);
            this.Controls.Add(this.lbl_Copyright);
            this.Controls.Add(this.lbl_Name);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Transparent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SplashScreen";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Timer TIMER_FadeOutAndClose;
        public System.Windows.Forms.Timer TIMER_StartFading;
        public System.Windows.Forms.Label lbl_Name;
        public System.Windows.Forms.Label lbl_Copyright;
        public System.Windows.Forms.Label lbl_Version;
        public System.Windows.Forms.Label lbl_Build;
    }
}