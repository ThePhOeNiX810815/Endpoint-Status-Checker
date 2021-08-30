namespace EndpointChecker
{
    partial class FeatureRequestDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FeatureRequestDialog));
            this.lbl_Title = new System.Windows.Forms.Label();
            this.lbl_Status = new System.Windows.Forms.Label();
            this.pb_Status = new System.Windows.Forms.PictureBox();
            this.lbl_UserEMailAddress = new System.Windows.Forms.Label();
            this.tb_UserEMailAddress = new System.Windows.Forms.TextBox();
            this.tb_Information = new System.Windows.Forms.TextBox();
            this.lbl_Information = new System.Windows.Forms.Label();
            this.btn_Send = new System.Windows.Forms.Button();
            this.btn_Close = new System.Windows.Forms.Button();
            this.lv_AttachedFiles = new System.Windows.Forms.ListView();
            this.imageList_Attachment = new System.Windows.Forms.ImageList(this.components);
            this.btn_AttachFiles = new System.Windows.Forms.Button();
            this.openFileDialog_AttachFiles = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Status)).BeginInit();
            this.SuspendLayout();
            // 
            // lbl_Title
            // 
            this.lbl_Title.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Title.ForeColor = System.Drawing.Color.RoyalBlue;
            this.lbl_Title.Location = new System.Drawing.Point(22, 18);
            this.lbl_Title.Name = "lbl_Title";
            this.lbl_Title.Size = new System.Drawing.Size(396, 23);
            this.lbl_Title.TabIndex = 0;
            this.lbl_Title.Text = "New Feature Request or Improvement";
            this.lbl_Title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_Status
            // 
            this.lbl_Status.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Status.ForeColor = System.Drawing.Color.Black;
            this.lbl_Status.Location = new System.Drawing.Point(22, 46);
            this.lbl_Status.Name = "lbl_Status";
            this.lbl_Status.Size = new System.Drawing.Size(396, 23);
            this.lbl_Status.TabIndex = 1;
            this.lbl_Status.Text = "Send improvement details to development team";
            this.lbl_Status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pb_Status
            // 
            this.pb_Status.Image = global::EndpointChecker.Properties.Resources.http;
            this.pb_Status.Location = new System.Drawing.Point(185, 76);
            this.pb_Status.Name = "pb_Status";
            this.pb_Status.Size = new System.Drawing.Size(72, 72);
            this.pb_Status.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_Status.TabIndex = 2;
            this.pb_Status.TabStop = false;
            // 
            // lbl_UserEMailAddress
            // 
            this.lbl_UserEMailAddress.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_UserEMailAddress.Location = new System.Drawing.Point(23, 187);
            this.lbl_UserEMailAddress.Name = "lbl_UserEMailAddress";
            this.lbl_UserEMailAddress.Size = new System.Drawing.Size(395, 20);
            this.lbl_UserEMailAddress.TabIndex = 3;
            this.lbl_UserEMailAddress.Text = "You E-Mail Address where we can contact you (optional)";
            this.lbl_UserEMailAddress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tb_UserEMailAddress
            // 
            this.tb_UserEMailAddress.BackColor = System.Drawing.SystemColors.Info;
            this.tb_UserEMailAddress.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tb_UserEMailAddress.Location = new System.Drawing.Point(26, 206);
            this.tb_UserEMailAddress.Multiline = true;
            this.tb_UserEMailAddress.Name = "tb_UserEMailAddress";
            this.tb_UserEMailAddress.Size = new System.Drawing.Size(392, 20);
            this.tb_UserEMailAddress.TabIndex = 4;
            this.tb_UserEMailAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tb_UserEMailAddress.TextChanged += new System.EventHandler(this.tb_UserEMailAddress_TextChanged);
            // 
            // tb_Information
            // 
            this.tb_Information.BackColor = System.Drawing.SystemColors.Info;
            this.tb_Information.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tb_Information.Location = new System.Drawing.Point(26, 264);
            this.tb_Information.Multiline = true;
            this.tb_Information.Name = "tb_Information";
            this.tb_Information.Size = new System.Drawing.Size(392, 142);
            this.tb_Information.TabIndex = 6;
            this.tb_Information.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tb_Information.TextChanged += new System.EventHandler(this.tb_Information_TextChanged);
            // 
            // lbl_Information
            // 
            this.lbl_Information.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Information.Location = new System.Drawing.Point(23, 244);
            this.lbl_Information.Name = "lbl_Information";
            this.lbl_Information.Size = new System.Drawing.Size(395, 20);
            this.lbl_Information.TabIndex = 5;
            this.lbl_Information.Text = "Improvement Description";
            this.lbl_Information.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btn_Send
            // 
            this.btn_Send.BackColor = System.Drawing.Color.DarkGray;
            this.btn_Send.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Send.Enabled = false;
            this.btn_Send.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Send.Location = new System.Drawing.Point(26, 480);
            this.btn_Send.Name = "btn_Send";
            this.btn_Send.Size = new System.Drawing.Size(142, 27);
            this.btn_Send.TabIndex = 7;
            this.btn_Send.Text = "Send Message";
            this.btn_Send.UseVisualStyleBackColor = false;
            this.btn_Send.Click += new System.EventHandler(this.btn_Send_Click);
            // 
            // btn_Close
            // 
            this.btn_Close.BackColor = System.Drawing.Color.DarkGray;
            this.btn_Close.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Close.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Close.Location = new System.Drawing.Point(276, 480);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(142, 27);
            this.btn_Close.TabIndex = 8;
            this.btn_Close.Text = "Close";
            this.btn_Close.UseVisualStyleBackColor = false;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // lv_AttachedFiles
            // 
            this.lv_AttachedFiles.BackColor = System.Drawing.SystemColors.Info;
            this.lv_AttachedFiles.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lv_AttachedFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lv_AttachedFiles.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lv_AttachedFiles.HideSelection = false;
            this.lv_AttachedFiles.LargeImageList = this.imageList_Attachment;
            this.lv_AttachedFiles.Location = new System.Drawing.Point(131, 417);
            this.lv_AttachedFiles.Name = "lv_AttachedFiles";
            this.lv_AttachedFiles.ShowItemToolTips = true;
            this.lv_AttachedFiles.Size = new System.Drawing.Size(287, 53);
            this.lv_AttachedFiles.SmallImageList = this.imageList_Attachment;
            this.lv_AttachedFiles.TabIndex = 9;
            this.lv_AttachedFiles.UseCompatibleStateImageBehavior = false;
            this.lv_AttachedFiles.View = System.Windows.Forms.View.List;
            this.lv_AttachedFiles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lv_AttachedFiles_KeyDown);
            // 
            // imageList_Attachment
            // 
            this.imageList_Attachment.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_Attachment.ImageStream")));
            this.imageList_Attachment.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList_Attachment.Images.SetKeyName(0, "star.ico");
            // 
            // btn_AttachFiles
            // 
            this.btn_AttachFiles.BackColor = System.Drawing.Color.DarkGray;
            this.btn_AttachFiles.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_AttachFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.btn_AttachFiles.Location = new System.Drawing.Point(26, 417);
            this.btn_AttachFiles.Name = "btn_AttachFiles";
            this.btn_AttachFiles.Size = new System.Drawing.Size(99, 53);
            this.btn_AttachFiles.TabIndex = 10;
            this.btn_AttachFiles.Text = "Attach File(s) (optional)";
            this.btn_AttachFiles.UseVisualStyleBackColor = false;
            this.btn_AttachFiles.Click += new System.EventHandler(this.btn_AttachFiles_Click);
            // 
            // openFileDialog_AttachFiles
            // 
            this.openFileDialog_AttachFiles.Multiselect = true;
            this.openFileDialog_AttachFiles.SupportMultiDottedExtensions = true;
            this.openFileDialog_AttachFiles.Title = "Browse for file(s) to attach ...";
            // 
            // FeatureRequestDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(443, 532);
            this.Controls.Add(this.lv_AttachedFiles);
            this.Controls.Add(this.btn_AttachFiles);
            this.Controls.Add(this.btn_Close);
            this.Controls.Add(this.btn_Send);
            this.Controls.Add(this.tb_Information);
            this.Controls.Add(this.lbl_Information);
            this.Controls.Add(this.tb_UserEMailAddress);
            this.Controls.Add(this.lbl_UserEMailAddress);
            this.Controls.Add(this.pb_Status);
            this.Controls.Add(this.lbl_Status);
            this.Controls.Add(this.lbl_Title);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(443, 532);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(443, 170);
            this.Name = "FeatureRequestDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FeatureRequestDialog";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FeatureRequestDialog_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.pb_Status)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label lbl_Title;
        public System.Windows.Forms.Label lbl_Status;
        public System.Windows.Forms.Label lbl_UserEMailAddress;
        public System.Windows.Forms.TextBox tb_UserEMailAddress;
        public System.Windows.Forms.TextBox tb_Information;
        public System.Windows.Forms.Label lbl_Information;
        public System.Windows.Forms.Button btn_Send;
        public System.Windows.Forms.PictureBox pb_Status;
        public System.Windows.Forms.Button btn_Close;
        public System.Windows.Forms.ListView lv_AttachedFiles;
        public System.Windows.Forms.Button btn_AttachFiles;
        public System.Windows.Forms.ImageList imageList_Attachment;
        public System.Windows.Forms.OpenFileDialog openFileDialog_AttachFiles;
    }
}