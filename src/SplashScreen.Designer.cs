
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
            this.lbl_Version_Date = new System.Windows.Forms.Label();
            this.pb_CloseDialog = new System.Windows.Forms.PictureBox();
            this.lbl_ReleaseType = new System.Windows.Forms.Label();
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
            this.lbl_Name.Font = new System.Drawing.Font("Agency FB", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Name.ForeColor = System.Drawing.Color.LightGreen;
            this.lbl_Name.Location = new System.Drawing.Point(48, 274);
            this.lbl_Name.Name = "lbl_Name";
            this.lbl_Name.Size = new System.Drawing.Size(509, 36);
            this.lbl_Name.TabIndex = 0;
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
            this.lbl_Copyright.TabIndex = 1;
            this.lbl_Copyright.Text = "<< COPYRIGHT LABEL >>";
            this.lbl_Copyright.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lbl_Copyright.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Controls_MouseDown);
            // 
            // lbl_Version_Date
            // 
            this.lbl_Version_Date.BackColor = System.Drawing.Color.Transparent;
            this.lbl_Version_Date.Font = new System.Drawing.Font("Agency FB", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Version_Date.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.lbl_Version_Date.Location = new System.Drawing.Point(50, 313);
            this.lbl_Version_Date.Name = "lbl_Version_Date";
            this.lbl_Version_Date.Size = new System.Drawing.Size(507, 34);
            this.lbl_Version_Date.TabIndex = 2;
            this.lbl_Version_Date.Text = "<< VERSION / DATE >>";
            this.lbl_Version_Date.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_Version_Date.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Controls_MouseDown);
            // 
            // pb_CloseDialog
            // 
            this.pb_CloseDialog.BackColor = System.Drawing.Color.Transparent;
            this.pb_CloseDialog.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pb_CloseDialog.Image = global::EndpointChecker.Properties.Resources.glass_transparent_cross;
            this.pb_CloseDialog.Location = new System.Drawing.Point(948, 3);
            this.pb_CloseDialog.Name = "pb_CloseDialog";
            this.pb_CloseDialog.Size = new System.Drawing.Size(48, 48);
            this.pb_CloseDialog.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_CloseDialog.TabIndex = 6;
            this.pb_CloseDialog.TabStop = false;
            this.pb_CloseDialog.Click += new System.EventHandler(this.pb_CloseDialog_Click);
            // 
            // lbl_ReleaseType
            // 
            this.lbl_ReleaseType.BackColor = System.Drawing.Color.Transparent;
            this.lbl_ReleaseType.Font = new System.Drawing.Font("Agency FB", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_ReleaseType.ForeColor = System.Drawing.Color.Black;
            this.lbl_ReleaseType.Location = new System.Drawing.Point(0, 20);
            this.lbl_ReleaseType.Name = "lbl_ReleaseType";
            this.lbl_ReleaseType.Size = new System.Drawing.Size(326, 33);
            this.lbl_ReleaseType.TabIndex = 9;
            this.lbl_ReleaseType.Text = "<<PACKAGE TYPE>>";
            this.lbl_ReleaseType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SplashScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = global::EndpointChecker.Properties.Resources.splash_Background_3;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1000, 378);
            this.Controls.Add(this.lbl_ReleaseType);
            this.Controls.Add(this.pb_CloseDialog);
            this.Controls.Add(this.lbl_Version_Date);
            this.Controls.Add(this.lbl_Copyright);
            this.Controls.Add(this.lbl_Name);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Transparent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(1000, 378);
            this.MinimumSize = new System.Drawing.Size(1000, 378);
            this.Name = "SplashScreen";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Controls_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.pb_CloseDialog)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Timer TIMER_FadeOutAndClose;
        public System.Windows.Forms.Timer TIMER_StartFading;
        public System.Windows.Forms.Label lbl_Name;
        public System.Windows.Forms.Label lbl_Copyright;
        public System.Windows.Forms.Label lbl_Version_Date;
        public System.Windows.Forms.PictureBox pb_CloseDialog;
        public System.Windows.Forms.Label lbl_ReleaseType;
    }
}