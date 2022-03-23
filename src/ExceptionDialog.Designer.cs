namespace EndpointChecker
{
    partial class ExceptionDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExceptionDialog));
            this.lbl_Title = new System.Windows.Forms.Label();
            this.lbl_Status = new System.Windows.Forms.Label();
            this.pb_Status = new System.Windows.Forms.PictureBox();
            this.lbl_UserEMailAddress = new System.Windows.Forms.Label();
            this.tb_UserEMailAddress = new System.Windows.Forms.TextBox();
            this.tb_OptionalComment = new System.Windows.Forms.TextBox();
            this.lbl_OptionalComment = new System.Windows.Forms.Label();
            this.btn_Send = new System.Windows.Forms.Button();
            this.cb_SystemInfo = new System.Windows.Forms.CheckBox();
            this.cb_AdditionalLogs = new System.Windows.Forms.CheckBox();
            this.cb_AttachScreenshot = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Status)).BeginInit();
            this.SuspendLayout();
            // 
            // lbl_Title
            // 
            this.lbl_Title.Font = new System.Drawing.Font("Segoe UI Semibold", 14.25F, System.Drawing.FontStyle.Bold);
            this.lbl_Title.ForeColor = System.Drawing.Color.Red;
            this.lbl_Title.Location = new System.Drawing.Point(11, 13);
            this.lbl_Title.Name = "lbl_Title";
            this.lbl_Title.Size = new System.Drawing.Size(422, 23);
            this.lbl_Title.TabIndex = 0;
            this.lbl_Title.Text = "Unexpected Exception occurred in application";
            this.lbl_Title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_Status
            // 
            this.lbl_Status.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.lbl_Status.ForeColor = System.Drawing.Color.Black;
            this.lbl_Status.Location = new System.Drawing.Point(22, 36);
            this.lbl_Status.Name = "lbl_Status";
            this.lbl_Status.Size = new System.Drawing.Size(396, 46);
            this.lbl_Status.TabIndex = 1;
            this.lbl_Status.Text = "Send error details report to development team";
            this.lbl_Status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pb_Status
            // 
            this.pb_Status.Image = global::EndpointChecker.Properties.Resources.ErrorReport;
            this.pb_Status.Location = new System.Drawing.Point(185, 92);
            this.pb_Status.Name = "pb_Status";
            this.pb_Status.Size = new System.Drawing.Size(72, 72);
            this.pb_Status.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_Status.TabIndex = 2;
            this.pb_Status.TabStop = false;
            // 
            // lbl_UserEMailAddress
            // 
            this.lbl_UserEMailAddress.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_UserEMailAddress.Location = new System.Drawing.Point(23, 193);
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
            this.tb_UserEMailAddress.Location = new System.Drawing.Point(26, 212);
            this.tb_UserEMailAddress.Multiline = true;
            this.tb_UserEMailAddress.Name = "tb_UserEMailAddress";
            this.tb_UserEMailAddress.Size = new System.Drawing.Size(392, 20);
            this.tb_UserEMailAddress.TabIndex = 4;
            this.tb_UserEMailAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tb_UserEMailAddress.TextChanged += new System.EventHandler(this.tb_UserEMailAddress_TextChanged);
            // 
            // tb_OptionalComment
            // 
            this.tb_OptionalComment.BackColor = System.Drawing.SystemColors.Info;
            this.tb_OptionalComment.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tb_OptionalComment.Location = new System.Drawing.Point(26, 269);
            this.tb_OptionalComment.Multiline = true;
            this.tb_OptionalComment.Name = "tb_OptionalComment";
            this.tb_OptionalComment.Size = new System.Drawing.Size(392, 120);
            this.tb_OptionalComment.TabIndex = 6;
            this.tb_OptionalComment.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tb_OptionalComment.TextChanged += new System.EventHandler(this.tb_OptionalComment_TextChanged);
            // 
            // lbl_OptionalComment
            // 
            this.lbl_OptionalComment.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_OptionalComment.Location = new System.Drawing.Point(23, 249);
            this.lbl_OptionalComment.Name = "lbl_OptionalComment";
            this.lbl_OptionalComment.Size = new System.Drawing.Size(395, 20);
            this.lbl_OptionalComment.TabIndex = 5;
            this.lbl_OptionalComment.Text = "Additional Comment (optional)";
            this.lbl_OptionalComment.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btn_Send
            // 
            this.btn_Send.BackColor = System.Drawing.Color.DarkGray;
            this.btn_Send.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Send.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Send.Location = new System.Drawing.Point(141, 482);
            this.btn_Send.Name = "btn_Send";
            this.btn_Send.Size = new System.Drawing.Size(160, 36);
            this.btn_Send.TabIndex = 7;
            this.btn_Send.Text = "Send Error Report";
            this.btn_Send.UseVisualStyleBackColor = false;
            this.btn_Send.Click += new System.EventHandler(this.btn_Send_Click);
            // 
            // cb_SystemInfo
            // 
            this.cb_SystemInfo.AutoSize = true;
            this.cb_SystemInfo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cb_SystemInfo.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_SystemInfo.Location = new System.Drawing.Point(26, 399);
            this.cb_SystemInfo.Name = "cb_SystemInfo";
            this.cb_SystemInfo.Size = new System.Drawing.Size(376, 24);
            this.cb_SystemInfo.TabIndex = 8;
            this.cb_SystemInfo.Text = "Send System Information (User, CPU, RAM, Disk, NIC)";
            this.cb_SystemInfo.UseVisualStyleBackColor = true;
            // 
            // cb_AdditionalLogs
            // 
            this.cb_AdditionalLogs.AutoSize = true;
            this.cb_AdditionalLogs.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cb_AdditionalLogs.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_AdditionalLogs.Location = new System.Drawing.Point(26, 418);
            this.cb_AdditionalLogs.Name = "cb_AdditionalLogs";
            this.cb_AdditionalLogs.Size = new System.Drawing.Size(352, 24);
            this.cb_AdditionalLogs.TabIndex = 9;
            this.cb_AdditionalLogs.Text = "Attach Additional LOG files (System, Application)";
            this.cb_AdditionalLogs.UseVisualStyleBackColor = true;
            // 
            // cb_AttachScreenshot
            // 
            this.cb_AttachScreenshot.AutoSize = true;
            this.cb_AttachScreenshot.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cb_AttachScreenshot.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_AttachScreenshot.Location = new System.Drawing.Point(26, 437);
            this.cb_AttachScreenshot.Name = "cb_AttachScreenshot";
            this.cb_AttachScreenshot.Size = new System.Drawing.Size(147, 24);
            this.cb_AttachScreenshot.TabIndex = 10;
            this.cb_AttachScreenshot.Text = "Attach Screenshot";
            this.cb_AttachScreenshot.UseVisualStyleBackColor = true;
            // 
            // ExceptionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(443, 532);
            this.Controls.Add(this.cb_AttachScreenshot);
            this.Controls.Add(this.cb_AdditionalLogs);
            this.Controls.Add(this.cb_SystemInfo);
            this.Controls.Add(this.btn_Send);
            this.Controls.Add(this.tb_OptionalComment);
            this.Controls.Add(this.lbl_OptionalComment);
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
            this.MinimumSize = new System.Drawing.Size(443, 190);
            this.Name = "ExceptionDialog";
            this.Opacity = 0D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ExceptionDialog";
            this.TopMost = true;
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ExceptionDialog_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.pb_Status)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label lbl_Title;
        public System.Windows.Forms.Label lbl_Status;
        public System.Windows.Forms.Label lbl_UserEMailAddress;
        public System.Windows.Forms.TextBox tb_UserEMailAddress;
        public System.Windows.Forms.TextBox tb_OptionalComment;
        public System.Windows.Forms.Label lbl_OptionalComment;
        public System.Windows.Forms.Button btn_Send;
        public System.Windows.Forms.PictureBox pb_Status;
        public System.Windows.Forms.CheckBox cb_SystemInfo;
        public System.Windows.Forms.CheckBox cb_AdditionalLogs;
        public System.Windows.Forms.CheckBox cb_AttachScreenshot;
    }
}