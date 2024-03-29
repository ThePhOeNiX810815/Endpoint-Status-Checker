﻿using ArpLookup;
using EndpointChecker.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Windows.Forms;
using static EndpointChecker.Program;

namespace EndpointChecker
{
    public partial class ExceptionDialog : Form
    {
        public static bool _autoCloseApp;
        public static Exception _exception;
        public static string _additionalInfo;
        public static string _callingMethod;
        public static List<MailAddress> _recipientsAddressesList = new List<MailAddress>();
        public static List<string> _attachmentsList = new List<string>();
        public static string _machineInfo_MachineName;
        public static string _machineInfo_UserName;
        public static string _machineInfo_RAM;
        public static string _machineInfo_DiskSpace;
        public static List<string> _machineInfo_IPList = new List<string>();
        public static List<string> _machineInfo_MACList = new List<string>();
        public static MemoryStream _screenshotStream = new MemoryStream();

        public ExceptionDialog(Exception exception, string callingMethod, string additionalInfo, List<MailAddress> recipientsAddressesList, List<string> attachmentsList, bool autoCloseApp)
        {
            InitializeComponent();

            CreateScreenshot();

            SetDialogSize(true);

            _autoCloseApp = autoCloseApp;
            _exception = exception;
            _additionalInfo = additionalInfo;
            _callingMethod = callingMethod;
            _recipientsAddressesList = recipientsAddressesList;
            _attachmentsList = attachmentsList;
        }

        public void SendNotificationMail(
            bool sendSystemInfo,
            bool attachLogs,
            bool attachScreenshot,
            string user_EMail,
            string user_Comment)
        {
            List<Property> reportItems = new List<Property>
            {
                new Property { ItemName = "Application Version", ItemValue = app_ApplicationName + " v" + app_VersionString },
                new Property { ItemName = "Date / Time", ItemValue = DateTime.Now.ToString("dd.MM.yyyy HH:mm") }
            };

            // OPTIONAL [SYSTEM INFO]
            if (sendSystemInfo)
            {
                reportItems.Add(new Property { ItemName = "Machine Name", ItemValue = _machineInfo_MachineName });
                reportItems.Add(new Property { ItemName = "IP Address(es)", ItemValue = string.Join(", ", _machineInfo_IPList) });
                reportItems.Add(new Property { ItemName = "MAC Address(es)", ItemValue = string.Join(", ", _machineInfo_MACList) });
                reportItems.Add(new Property { ItemName = "Operating System Version", ItemValue = os_VersionString });
                reportItems.Add(new Property { ItemName = "Operating System Login Name", ItemValue = _machineInfo_UserName });
                reportItems.Add(new Property { ItemName = "Operating Memory", ItemValue = _machineInfo_RAM });
                reportItems.Add(new Property { ItemName = "System Drive (C)", ItemValue = _machineInfo_DiskSpace });
            }

            reportItems.Add(new Property { ItemName = "Calling Method", ItemValue = _callingMethod });
            reportItems.Add(new Property { ItemName = "Exception HResult", ItemValue = _exception.HResult.ToString() });
            reportItems.Add(new Property { ItemName = "Exception Message", ItemValue = _exception.Message });
            reportItems.Add(new Property { ItemName = "Stacktrace", ItemValue = _exception.StackTrace });

            // OPTIONAL [USER E-MAIL]
            if (!string.IsNullOrEmpty(user_EMail))
            {
                reportItems.Add(new Property { ItemName = "Reporter E-Mail Address", ItemValue = user_EMail });
            }

            // OPTIONAL [USER COMMENT]
            if (!string.IsNullOrEmpty(user_Comment))
            {
                reportItems.Add(new Property { ItemName = "Reporter Comment", ItemValue = user_Comment });
            }

            // OPTIONAL [ADDITIONAL INFO]
            if (!string.IsNullOrEmpty(_additionalInfo))
            {
                reportItems.Add(new Property { ItemName = "Additional Information", ItemValue = _additionalInfo });
            }

            // E-MAIL SUBJECT AND CREATE BODY HTML TABLE
            string eMailMessageSubject = app_ApplicationName + " Unhandled Exception Report";
            string eMailMessageBody = string.Empty;

            HtmlTable table = new HtmlTable
            {
                Border = 3,
                BorderColor = "#FF0000",
                BgColor = "#FFC0CB",
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
                mailMessageString.AppendFormat(sw.ToString());
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

                    // ADD SCREENSHOT (OPTIONAL)
                    if (attachScreenshot &&
                        _screenshotStream != null)
                    {
                        mailMessage.Attachments.Add(new Attachment(_screenshotStream, "ScreenShot.png"));
                    }

                    // ADD ATTACHMENTS (OPTIONAL)
                    if (attachLogs)
                    {
                        foreach (string attachment in _attachmentsList)
                        {
                            if (File.Exists(attachment))
                            {
                                mailMessage.Attachments.Add(new Attachment(attachment));
                            }
                        }
                    }

                    // ADD USER MAIL ON RECIPIENTS LIST (OPTIONAL)
                    if (IsMailAddressValid(user_EMail))
                    {
                        _recipientsAddressesList.Add(new MailAddress(user_EMail));
                    }

                    SendMailMessage(mailMessage);
                }
            }
            catch (Exception ex)
            {
                ThreadSafeInvoke(() =>
                {
                    // SET STATUS CONTROLS
                    lbl_Status.Text = "There was an error opening Exception Report";
                    pb_Status.Image = Resources.Failed;
                });

                Thread.Sleep(3000);

                ThreadSafeInvoke(() =>
                {
                    Hide();

                    MessageBox.Show(
                            "Unable to send exception details e-mail due to following error:" +
                            Environment.NewLine +
                            Environment.NewLine +
                            ex.Message,
                        app_ApplicationName + " v" + app_VersionString,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                });
            }

            ThreadSafeInvoke(() =>
            {
                if (_autoCloseApp)
                {
                    // CLOSE PROGRAM
                    Environment.Exit(1);
                }
                else
                {
                    // CLOSE DIALOG
                    Close();
                }
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
                    lbl_Status.Text = "Exception report has been created and opened";
                    pb_Status.Image = Resources.Success;

                    Application.DoEvents();
                    Thread.Sleep(3000);
                });
            }
        }

        public void CreateScreenshot()
        {
            // GET SCREENSHOT STREAM
            try
            {
                Rectangle screenBounds = Screen.GetBounds(Point.Empty);

                using (Bitmap screenshotBitmap = new Bitmap(screenBounds.Width, screenBounds.Height, PixelFormat.Format32bppRgb))
                {
                    using (Graphics graphicsSurface = Graphics.FromImage(screenshotBitmap))
                    {
                        graphicsSurface.CopyFromScreen(Point.Empty, Point.Empty, screenBounds.Size);
                    }

                    screenshotBitmap.Save(_screenshotStream, ImageFormat.Png);
                    _screenshotStream.Position = 0;
                }
            }
            catch
            {
            }

            // SHOW DIALOG
            Opacity = 100;
        }

        public void LoadMachineInfo()
        {
            // MACHINE NAME
            _machineInfo_MachineName = Dns.GetHostName().ToUpper();

            // LOCAL IP(s) AND MAC(s)
            IPHostEntry hostEntry = Dns.GetHostEntry(_machineInfo_MachineName);
            foreach (IPAddress ip in hostEntry.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    _machineInfo_IPList.Add(ip.ToString());
                    string localMAC = WindowsLookupService.Lookup(ip);

                    if (!string.IsNullOrEmpty(localMAC))
                    {
                        _machineInfo_MACList.Add(localMAC);
                    }
                    else
                    {
                        _machineInfo_MACList.Add("N/A");
                    }
                }

                Application.DoEvents();
            }

            // USER NAME
            _machineInfo_UserName = WindowsIdentity.GetCurrent().Name;

            // OPERATING MEMORY / DISK SPACE
            long ram_Maximum = PerformanceInfo.GetTotalMemoryInMiB();
            long ram_Used = ram_Maximum - PerformanceInfo.GetPhysicalAvailableMemoryInMiB();

            DriveInfo driveInfo = new DriveInfo(@"C:");
            long disk_Total = driveInfo.TotalSize / 1024 / 1024 / 1024;
            long disk_Used = (driveInfo.TotalSize - driveInfo.AvailableFreeSpace) / 1024 / 1024 / 1024;

            _machineInfo_RAM = (int)ram_Used + " of " + (int)ram_Maximum + " MB Used";
            _machineInfo_DiskSpace = (int)disk_Used + " of " + (int)disk_Total + " GB Used";
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
            lbl_Status.Text = "Sending error details report to development team";
            pb_Status.Image = Resources.eMailEnvelope;

            // GET SYSTEM INFORMATION (OPTIONAL)            
            LoadMachineInfo();

            // GET REPORT OPTIONS
            bool sendSystemInfo = cb_SystemInfo.Checked;
            bool attachLogs = cb_AdditionalLogs.Checked;
            bool attachScreenshot = cb_AttachScreenshot.Checked;
            string user_EMail = tb_UserEMailAddress.Text.TrimStart().TrimEnd();
            string user_Comment = tb_OptionalComment.Text.TrimStart().TrimEnd();

            NewBackgroundThread(() =>
            {
                // SEND E-MAIL REPORT
                SendNotificationMail(
                    sendSystemInfo,
                    attachLogs,
                    attachScreenshot,
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
            tb_UserEMailAddress.BackColor = string.IsNullOrEmpty(tb_UserEMailAddress.Text)
                ? SystemColors.Info
                : !IsMailAddressValid(tb_UserEMailAddress.Text) ? System.Drawing.Color.Pink : System.Drawing.Color.Honeydew;
        }

        public void tb_OptionalComment_TextChanged(object sender, EventArgs e)
        {
            tb_OptionalComment.BackColor = string.IsNullOrEmpty(tb_OptionalComment.Text) ? SystemColors.Info : System.Drawing.Color.Honeydew;
        }

        public void SetDialogSize(bool allControls)
        {
            Size = allControls ? MaximumSize : MinimumSize;

            Invalidate();
            Update();
            Refresh();
            CenterToScreen();
            BringToFront();
        }

        public void ExceptionDialog_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphicsObj = CreateGraphics();
            Pen myPen = new Pen(System.Drawing.Color.Red, 20);
            Rectangle myRectangle = new Rectangle(0, 0, Width, Height);
            graphicsObj.DrawRectangle(myPen, myRectangle);
        }
    }

    public static class PerformanceInfo
    {
        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetPerformanceInfo([Out] out PerformanceInformation PerformanceInformation, [In] int Size);

        [StructLayout(LayoutKind.Sequential)]
        public struct PerformanceInformation
        {
            public int Size;
            public IntPtr CommitTotal;
            public IntPtr CommitLimit;
            public IntPtr CommitPeak;
            public IntPtr PhysicalTotal;
            public IntPtr PhysicalAvailable;
            public IntPtr SystemCache;
            public IntPtr KernelTotal;
            public IntPtr KernelPaged;
            public IntPtr KernelNonPaged;
            public IntPtr PageSize;
            public int HandlesCount;
            public int ProcessCount;
            public int ThreadCount;
        }

        public static long GetPhysicalAvailableMemoryInMiB()
        {
            PerformanceInformation pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToInt64((pi.PhysicalAvailable.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            }
            else
            {
                return -1;
            }
        }

        public static long GetTotalMemoryInMiB()
        {
            PerformanceInformation pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToInt64((pi.PhysicalTotal.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            }
            else
            {
                return -1;
            }
        }
    }
}