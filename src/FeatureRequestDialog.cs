using EndpointChecker.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Windows.Forms;
using static EndpointChecker.Program;

namespace EndpointChecker
{
    public partial class FeatureRequestDialog : Form
    {
        public static List<MailAddress> _recipientsAddressesList = new List<MailAddress>();

        public FeatureRequestDialog(List<MailAddress> recipientsAddressesList)
        {
            InitializeComponent();

            _recipientsAddressesList = recipientsAddressesList;
        }

        public void SendNotificationMail(
            string user_ReporterEMail,
            string user_Comment)
        {
            List<Property> reportItems = new List<Property>
            {
                new Property { ItemName = "Application Version", ItemValue = app_ApplicationName + " v" + app_VersionString },
                new Property { ItemName = "Date / Time", ItemValue = DateTime.Now.ToString("dd.MM.yyyy HH:mm") },
                new Property { ItemName = "Improvement Description", ItemValue = user_Comment }
            };

            // OPTIONAL [USER E-MAIL]
            if (!string.IsNullOrEmpty(user_ReporterEMail))
            {
                reportItems.Add(new Property { ItemName = "Reporter E-Mail Address", ItemValue = user_ReporterEMail });
            }

            // E-MAIL SUBJECT AND CREATE BODY HTML TABLE
            string eMailMessageSubject = app_ApplicationName + " Feature Request";
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

            using (StringWriter sw = new StringWriter())
            {
                table.RenderControl(new HtmlTextWriter(sw));
                _ = mailMessageString.AppendFormat(sw.ToString());
                eMailMessageBody = mailMessageString.ToString();
            }

            try
            {
                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.Subject = eMailMessageSubject;
                    mailMessage.Body = eMailMessageBody;
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Priority = MailPriority.High;

                    // ADD ATTACHMENTS (OPTIONAL)
                    ThreadSafeInvoke(() =>
                    {
                        foreach (ListViewItem attachedFileItem in lv_AttachedFiles.Items)
                        {
                            mailMessage.Attachments.Add(new Attachment(attachedFileItem.Tag.ToString()));
                        }
                    });

                    // TRY TO SEND THE E-MAIL(S) BY HOST E-MAIL CLIENT
                    SendMailMessage(mailMessage);

                    Thread.Sleep(2000);
                }
            }
            catch (Exception ex)
            {
                ThreadSafeInvoke(() =>
                {
                    // SET STATUS CONTROLS
                    lbl_Status.Text = "There was an error opening Feature Request";
                    pb_Status.Image = Resources.Failed;
                });

                Thread.Sleep(3000);

                ThreadSafeInvoke(() =>
                {
                    Hide();

                    _ = MessageBox.Show(
                            "Unable to send Feature Request e-mail due to following error:" +
                            Environment.NewLine +
                            Environment.NewLine +
                            ex.Message,
                        app_ApplicationName + " v" + app_Version,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                });
            }

            ThreadSafeInvoke(() =>
            {
                GC.Collect();

                // CLOSE DIALOG
                Close();
            });
        }

        public void SendMailMessage(MailMessage mailMessage)
        {
            mailMessage.From = report_Recipient;

            foreach (MailAddress _recipientAddress in _recipientsAddressesList)
            {
                mailMessage.To.Add(_recipientAddress);
            }

            using (SmtpClient smtpClient = new SmtpClient())
            {
                string mail_TempFolder = Path.Combine(app_TempDir, Guid.NewGuid().ToString());

                // CREATE TEMP MAIL FOLDER
                if (!Directory.Exists(mail_TempFolder))
                {
                    Directory.CreateDirectory(mail_TempFolder);
                }

                // SAVE MESSAGE TO LIST
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                smtpClient.PickupDirectoryLocation = mail_TempFolder;
                smtpClient.Send(mailMessage);

                // SHOW INFORMATION TO USER
                MessageBox.Show(
                    "Report E-Mail Message will be opened in your E-Mail application." +
                    Environment.NewLine +
                    Environment.NewLine +
                    "Please, resend the report message to the author in such a way that you reply on it." +
                    Environment.NewLine +
                    Environment.NewLine +
                    "Thanks a lot :-)",
                    app_ApplicationName + " v" + app_VersionString,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // OPEN SAVED MESSAGE BY HOST DEFAULT ASSOCIATED E-MAIL CLIENT
                Process.Start(Directory.GetFiles(mail_TempFolder).Single());

                ThreadSafeInvoke(() =>
                {
                    // SET STATUS CONTROLS
                    lbl_Status.Text = "Feature Request has been created and opened";
                    pb_Status.Image = Resources.Success;

                    Application.DoEvents();
                    Thread.Sleep(3000);
                });
            }
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

                _ = Invoke(action);
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

            NewBackgroundThread(() =>
            {
                // SEND E-MAIL REPORT
                SendNotificationMail(
                    user_EMail,
                    user_Comment);
            });
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

            tb_UserEMailAddress.BackColor = string.IsNullOrEmpty(tb_UserEMailAddress.Text)
                ? SystemColors.Info
                : !IsMailAddressValid(tb_UserEMailAddress.Text) ? Color.Pink : Color.Honeydew;
        }

        public void SetDialogSize(bool allControls)
        {
            Size = allControls ? MaximumSize : MinimumSize;

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

            tb_Information.BackColor = string.IsNullOrEmpty(tb_Information.Text) ? SystemColors.Info : Color.Honeydew;
        }

        public void btn_Close_Click(object sender, EventArgs e)
        {
            Close();

            GC.Collect();
        }

        public void btn_AttachFiles_Click(object sender, EventArgs e)
        {
            openFileDialog_AttachFiles.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (openFileDialog_AttachFiles.ShowDialog() == DialogResult.OK)
            {
                foreach (string fileName in openFileDialog_AttachFiles.FileNames)
                {
                    ListViewItem fileItem = new ListViewItem
                    {
                        ImageIndex = 0,
                        Text = Path.GetFileName(fileName),
                        Tag = fileName,
                        ToolTipText = fileName + " (press DEL to remove from list)"
                    };

                    _ = lv_AttachedFiles.Items.Add(fileItem);
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
