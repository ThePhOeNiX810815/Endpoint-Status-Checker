using EndpointChecker.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Windows.Forms;

namespace EndpointChecker
{
    public partial class FeatureRequestDialog : Form
    {
        public static string _senderAddress;
        public static List<string> _recipientsAddressesList = new List<string>();

        public FeatureRequestDialog(string senderAddress, List<string> recipientsAddressesList)
        {
            InitializeComponent();

            _senderAddress = senderAddress;
            _recipientsAddressesList = recipientsAddressesList;
        }

        public void SendNotificationMail(
            string user_EMail,
            string user_Comment)
        {
            List<Property> reportItems = new List<Property>();

            reportItems.Add(new Property { ItemName = "Application Version", ItemValue = Program.assembly_ApplicationName + " v" + Program.assembly_Version });
            reportItems.Add(new Property { ItemName = "Date / Time", ItemValue = DateTime.Now.ToString("dd.MM.yyyy HH:mm") });
            reportItems.Add(new Property { ItemName = "Improvement Description", ItemValue = user_Comment });

            // OPTIONAL [USER E-MAIL]
            if (!string.IsNullOrEmpty(user_EMail))
            {
                reportItems.Add(new Property { ItemName = "Reporter E-Mail Address", ItemValue = user_EMail });
            }

            // E-MAIL SUBJECT AND CREATE BODY HTML TABLE
            string eMailMessageSubject = Program.assembly_ApplicationName + " Feature Request";
            string eMailMessageBody = string.Empty;

            HtmlTable table = new HtmlTable
            {
                Border = 3,
                BorderColor = "#003CFF",
                BgColor = "#91ABFF",
                CellPadding = 5,
                CellSpacing = 10
            };

            StringBuilder mailMessageString = new StringBuilder();

            foreach (Property reportItem in reportItems)
            {
                HtmlTableRow row = new HtmlTableRow();
                row.Cells.Add(new HtmlTableCell { InnerText = reportItem.ItemName });
                row.Cells.Add(new HtmlTableCell { InnerText = reportItem.ItemValue });
                table.Rows.Add(row);
            }

            using (var sw = new StringWriter())
            {
                table.RenderControl(new HtmlTextWriter(sw));
                mailMessageString.AppendFormat(sw.ToString());
                eMailMessageBody = mailMessageString.ToString();
            }

            try
            {
                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(_senderAddress);
                    mailMessage.Subject = eMailMessageSubject;
                    mailMessage.Body = eMailMessageBody;
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Priority = MailPriority.High;

                    // ADD RECIPIENTS FROM LIST
                    foreach (string recipientAddress in _recipientsAddressesList)
                    {
                        if (IsMailAddressValid(recipientAddress))
                        {
                            mailMessage.To.Add(new MailAddress(recipientAddress));
                        }
                    }

                    // ADD ATTACHMENTS (OPTIONAL)
                    ThreadSafeInvoke((Action)(() =>
                    {
                        foreach (ListViewItem attachedFileItem in lv_AttachedFiles.Items)
                        {
                            mailMessage.Attachments.Add(new Attachment(attachedFileItem.Tag.ToString()));
                        }
                    }));

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "gmail-smtp-in.l.google.com";
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Port = 25;
                    smtp.Send(mailMessage);

                    ThreadSafeInvoke((Action)(() =>
                    {
                        // SET STATUS CONTROLS
                        lbl_Status.Text = "Feature Request has been successfully sent";
                        pb_Status.Image = Resources.Success;
                    }));
                }
            }
            catch (Exception ex)
            {
                ThreadSafeInvoke((Action)(() =>
                {
                    // SET STATUS CONTROLS
                    lbl_Status.Text = "There was an error sending Feature Request";
                    pb_Status.Image = Resources.Failed;

                    BringToFront();

                    MessageBox.Show(
                            "Unable to send Feature Request e-mail due to following error:" +
                            Environment.NewLine +
                            Environment.NewLine +
                            ex.Message,
                        Program.assembly_ApplicationName + " v" + Program.assembly_Version,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }));
            }

            Thread.Sleep(3000);

            ThreadSafeInvoke((Action)(() =>
            {
                // CLOSE DIALOG
                Close();
            }));
        }

        public void NewBackgroundThread(Action action)
        {
            Application.DoEvents();

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                action();
            })
            .Start();
        }

        public void ThreadSafeInvoke(Action action)
        {
            try
            {
                Application.DoEvents();

                Invoke(action);
            }
            catch
            {
            }
        }

        public void btn_Send_Click(object sender, EventArgs e)
        {
            // SET MINIMAL DIALOG VIEW
            SetDialogSize(false);

            // SET STATUS CONTROLS
            lbl_Status.Text = "Sending Feature Request to development team";
            pb_Status.Image = Resources.eMailEnvelope;

            // GET REPORT OPTIONS
            string user_EMail = tb_UserEMailAddress.Text.TrimStart().TrimEnd();
            string user_Comment = tb_Information.Text.TrimStart().TrimEnd();

            NewBackgroundThread((Action)(() =>
            {
                // SEND E-MAIL REPORT
                SendNotificationMail(
                    user_EMail,
                    user_Comment);
            }));
        }

        public static bool IsMailAddressValid(string mailAddress)
        {
            try
            {
                MailAddress eMailValidator = new MailAddress(mailAddress);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void tb_UserEMailAddress_TextChanged(object sender, EventArgs e)
        {
            btn_Send.Enabled = !string.IsNullOrEmpty(tb_Information.Text) &&
                (string.IsNullOrEmpty(tb_UserEMailAddress.Text) ||
                IsMailAddressValid(tb_UserEMailAddress.Text));

            if (string.IsNullOrEmpty(tb_UserEMailAddress.Text))
            {
                tb_UserEMailAddress.BackColor = SystemColors.Info;
            }
            else if (!IsMailAddressValid(tb_UserEMailAddress.Text))
            {
                tb_UserEMailAddress.BackColor = Color.Pink;
            }
            else
            {
                tb_UserEMailAddress.BackColor = Color.Honeydew;
            }
        }

        public void SetDialogSize(bool allControls)
        {
            if (allControls)
            {
                Size = MaximumSize;
            }
            else
            {
                Size = MinimumSize;
            }

            Invalidate();
            Update();
            Refresh();
            CenterToScreen();
        }

        public void FeatureRequestDialog_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphicsObj = CreateGraphics();
            Pen myPen = new Pen(Color.BlueViolet, 20);
            Rectangle myRectangle = new Rectangle(0, 0, Width, Height);
            graphicsObj.DrawRectangle(myPen, myRectangle);
        }

        public void tb_Information_TextChanged(object sender, EventArgs e)
        {
            btn_Send.Enabled = !string.IsNullOrEmpty(tb_Information.Text) &&
                (string.IsNullOrEmpty(tb_UserEMailAddress.Text) ||
                IsMailAddressValid(tb_UserEMailAddress.Text));

            if (string.IsNullOrEmpty(tb_Information.Text))
            {
                tb_Information.BackColor = SystemColors.Info;
            }
            else
            {
                tb_Information.BackColor = Color.Honeydew;
            }
        }

        public void btn_Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void btn_AttachFiles_Click(object sender, EventArgs e)
        {
            openFileDialog_AttachFiles.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (openFileDialog_AttachFiles.ShowDialog() == DialogResult.OK)
            {
                foreach (string fileName in openFileDialog_AttachFiles.FileNames)
                {
                    ListViewItem fileItem = new ListViewItem();
                    fileItem.ImageIndex = 0;
                    fileItem.Text = Path.GetFileName(fileName);
                    fileItem.Tag = fileName;
                    fileItem.ToolTipText = fileName + " (press DEL to remove from list)";

                    lv_AttachedFiles.Items.Add(fileItem);
                }
            }
        }

        public void lv_AttachedFiles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete &&
                lv_AttachedFiles.SelectedItems.Count > 0)
            {
                foreach (ListViewItem selectedItem in lv_AttachedFiles.SelectedItems)
                {
                    lv_AttachedFiles.Items.Remove(selectedItem);
                }
            }
        }
    }
}
