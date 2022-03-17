using EndpointChecker.Properties;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows.Forms;

namespace EndpointChecker
{
    internal class GacApi
    {
        [DllImport("fusion.dll")]
        internal static extern IntPtr CreateAssemblyCache(
            out IAssemblyCache ppAsmCache, int reserved);
    }

    // GAC Interfaces - IAssemblyCache. As a sample, non used vtable entries     
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("e707dcde-d1cd-11d2-bab9-00c04f8eceae")]
    internal interface IAssemblyCache
    {
        int Dummy1();
        [PreserveSig()]
        IntPtr QueryAssemblyInfo(
            int flags,
            [MarshalAs(UnmanagedType.LPWStr)]
            string assemblyName,
            ref ASSEMBLY_INFO assemblyInfo);

        int Dummy2();
        int Dummy3();
        int Dummy4();
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ASSEMBLY_INFO
    {
        public int cbAssemblyInfo;
        public int assemblyFlags;
        public long assemblySizeInKB;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string currentAssemblyPath;

        public int cchBuf;
    }

    static class Program
    {
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string className, string windowTitle);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);

        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowPlacement(IntPtr hWnd, ref Windowplacement lpwndpl);

        private enum ShowWindowEnum
        {
            Hide = 0,
            ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
            Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
            Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
            Restore = 9, ShowDefault = 10, ForceMinimized = 11
        };

        private struct Windowplacement
        {
            public int length;
            public int flags;
            public int showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }

        // APPLICATION SPECIFICATION
        public static Assembly app_Assembly = Assembly.GetExecutingAssembly();
        public static Version app_Version = app_Assembly.GetName().Version;
        public static string app_VersionString = GetVersionString(app_Version, true, false);
        
        public static string os_VersionString =
            (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows NT\CurrentVersion", "ProductName", null) +
            " " +
            (Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit") +
            ", Build " +
            Environment.OSVersion.Version.Build.ToString();

        public static bool app_ScanOnStartup;
        public static bool app_ShowSplashScreen;
        public static string app_ApplicationName = FileVersionInfo.GetVersionInfo(app_Assembly.Location).ProductName;
        public static string app_ApplicationExecutableName = AppDomain.CurrentDomain.FriendlyName;
        public static string app_CurrentWorkingDir = Path.GetDirectoryName(app_Assembly.Location);
        public static string app_BuiltDate = RetrieveLinkerTimestamp();
        public static string app_Copyright = FileVersionInfo.GetVersionInfo(app_Assembly.Location).LegalCopyright;
        public static string app_Title = app_ApplicationName + " v" + app_VersionString;

        // FEEDBACK AND EXCEPTION HANDLING E-MAIL ADDRESSES
        public static string exceptionReport_senderEMailAddress = "ExceptionReport@EndpointStatusChecker";
        public static string featureRequest_senderEMailAddress = "FeatureRequest@EndpointStatusChecker";
        public static string authorEmailAddress = "phoenixvm@gmail.com";
        public static string anonymousFTPPassword = "anonymous";

        // ENDPOINTS DEFINITIONS FILE NAME
        public static string endpointDefinitonsFile = "EndpointChecker_EndpointsList.txt";

        // GOOGLE MAPS API KEY & ZOOM FACTOR
        public static string apiKey_GoogleMaps;
        public static int googleMapsZoomFactor;

        // SYSTEM MEMORY SIZE STRING
        public static string systemMemorySize;

        // VIRUSTOTAL API KEY
        public static string apiKey_VirusTotal;

        // HTTP CLIENT USER-AGENT STRINGS
        public static string http_UserAgent;
        public static string http_Sec_CH_UserAgent;

        // STRING FORMAT FOR 'NOT AVAILABLE' STATUS
        public static string status_NotAvailable = "N/A";

        // STRING FORMAT FOR 'ERROR' STATUS
        public static string status_Error = "ERROR";

        // APPLICATION CONFIGURATION FILE
        public static string appConfigFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;

        // ENDPOINTS 'LAST SEEN ONLINE' LIST FILE NAME
        public static string lastSeenOnlineJSONFile = "EndpointChecker_LastSeenOnline.json";

        // ERRORS [ENDPOINT DUPLICITIES] LIST FILE NAME
        public static string endpointsList_Duplicities = "_ERRORS - Invalid Endpoint Definitions - Duplicities.txt";

        // ERRORS [INVALID ENDPOINT DEFINITIONS] LIST FILE NAME
        public static string endpointsList_InvalidDefs = "_ERRORS - Invalid Endpoint Definitions - Invalid URL format.txt";

        // DEFAULT STATUS EXPORT DIRECTORY AND FILENAMES
        public static string statusExport_Directory = app_CurrentWorkingDir;
        public static string statusExport_XLSFile = "EndpointsStatus.xlsx";
        public static string statusExport_JSONFile = "EndpointsStatus.json";
        public static string statusExport_XMLFile = "EndpointsStatus.xml";
        public static string statusExport_HTMLFile_InfoPage = "EndpointsStatus_Info.html";
        public static string statusExport_HTMLFile_HTTPPage = "EndpointsStatus_HTTP.html";
        public static string statusExport_HTMLFile_FTPPage = "EndpointsStatus_FTP.html";

        // MAXIMUM LENGHT OF HTTP RESPONSE TO READ
        public static long http_SaveResponse_MaxLenght_Bytes;

        // AUTO UPDATE VARIABLES
        public static bool app_AutoUpdate_AutoUpdateInFuture;
        public static Version app_AutoUpdate_SkipVersion;
        public static bool app_AutoUpdate = false;
        public static Version app_LatestPackageVersion = new Version(0, 0, 0, 0);
        public static string app_LatestPackageLink = string.Empty;
        public static string app_LatestPackageDate = string.Empty;
        public static string app_LatestPackageReleaseNotes_RTF = string.Empty;

        // SIGNING CERTIFICATE
        public static bool app_IsOriginalSignedExecutable = IsOriginalSignedExecutable();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (Settings.Default.UpgradeRequired)
            {
                // UPGRADE SETTINGS FROM PREVIOUS VERSION
                Settings.Default.Upgrade();
                Settings.Default.UpgradeRequired = false;
                Settings.Default.Save();
            }

            // GET SETTINGS
            app_ScanOnStartup = Settings.Default.Config_ScanOnStartup;
            app_ShowSplashScreen = Settings.Default.Config_ShowSplashScreen;
            apiKey_GoogleMaps = Settings.Default.GoogleMaps_API_Key;
            googleMapsZoomFactor = Settings.Default.GoogleMaps_API_ZoomFactor;
            apiKey_VirusTotal = Settings.Default.VirusTotal_API_Key;
            http_UserAgent = Settings.Default.Config_HTTP_UserAgent;
            http_Sec_CH_UserAgent = Settings.Default.Config_HTTP_Sec_CH_UserAgent;
            http_SaveResponse_MaxLenght_Bytes = Settings.Default.Config_HTTP_SaveResponse_MaxLenght_Bytes;
            app_AutoUpdate_SkipVersion = new Version(Settings.Default.AutoUpdate_SkipVersion);
            app_AutoUpdate_AutoUpdateInFuture = Settings.Default.AutoUpdate_AutoUpdateInFuture;

            // APP SETTINGS
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            if (args.Length == 6 &&
                args[0] == "/AutoUpdate" &&
                Directory.Exists(args[1]) &&
                !string.IsNullOrEmpty(args[2]) &&
                !string.IsNullOrEmpty(args[3]) &&
                !string.IsNullOrEmpty(args[4]) &&
                !string.IsNullOrEmpty(args[5]))
            {
                // PASS ARGUMENTS
                app_CurrentWorkingDir = args[1];
                app_ApplicationExecutableName = args[2];
                app_LatestPackageVersion = new Version(args[3]);
                app_LatestPackageLink = args[4];
                app_LatestPackageDate = args[5];

                // AUTO UPDATE FROM GITHUB PACKAGE
                Application.Run(new AutoUpdaterDialog());
            }
            else
            {
                // GET SYSTEM MEMORY SIZE
                long systemMemorySize_KB;
                GetPhysicallyInstalledSystemMemory(out systemMemorySize_KB);
                systemMemorySize = (systemMemorySize_KB / 1024 / 1024).ToString() + " GB";

                // CHECK APPLICATION INSTANCE
                bool createdNew = true;
                using (Mutex mainMutex = new Mutex(true, app_ApplicationName, out createdNew))
                {
                    if (!createdNew)
                    {
                        // ANOTHER APPLICATION INSTANCE IS ALREADY RUNNING, RESTORE WINDOW
                        IntPtr wdwIntPtr = FindWindow(null, app_Title);
                        Windowplacement placement = new Windowplacement();

                        GetWindowPlacement(wdwIntPtr, ref placement);
                        ShowWindow(wdwIntPtr, ShowWindowEnum.Show);
                        SetForegroundWindow(wdwIntPtr);
                    }
                    else if (RequiredLibraryExists("AGauge.dll") &&
                             RequiredLibraryExists("ClosedXML.dll") &&
                             RequiredLibraryExists("DocumentFormat.OpenXml.dll") &&
                             RequiredLibraryExists("ExcelNumberFormat.dll") &&
                             RequiredLibraryExists("FastMember.dll") &&
                             RequiredLibraryExists("Flurl.dll") &&
                             RequiredLibraryExists("HtmlAgilityPack.dll") &&
                             RequiredLibraryExists("IPAddressRange.dll") &&
                             RequiredLibraryExists("Microsoft.WindowsAPICodePack.dll") &&
                             RequiredLibraryExists("Microsoft.WindowsAPICodePack.Shell.dll") &&
                             RequiredLibraryExists("Nager.PublicSuffix.dll") &&
                             RequiredLibraryExists("Newtonsoft.Json.dll") &&
                             RequiredLibraryExists("NSpeedTest.dll") &&
                             RequiredLibraryExists("Spire.License.dll") &&
                             RequiredLibraryExists("Spire.XLS.dll") &&
                             RequiredLibraryExists("Spire.Pdf.dll") &&
                             RequiredLibraryExists("tracert.dll") &&
                             RequiredLibraryExists("VirusTotal.NET.dll") &&
                             RequiredLibraryExists("WhoisClient.dll"))
                    {
                        CheckForUpdate();

                        if (app_AutoUpdate)
                        {
                            string currentExecutable = app_Assembly.Location;
                            string updaterExecutable = Path.Combine(Path.GetTempPath(), "EndpointChecker_AutoUpdater.exe");

                            // COPY UPDATER TO TEMP DIRECORY
                            File.Copy(currentExecutable, updaterExecutable, true);

                            // UPDATER ARGUMENTS
                            List<string> updaterArgs = new List<string>();
                            updaterArgs.Add("/AutoUpdate");
                            updaterArgs.Add("\"" + Path.GetDirectoryName(currentExecutable) + "\"");
                            updaterArgs.Add(app_ApplicationExecutableName);
                            updaterArgs.Add(app_LatestPackageVersion.ToString());
                            updaterArgs.Add(app_LatestPackageLink);
                            updaterArgs.Add(app_LatestPackageDate);

                            // EXECUTE UPDATER
                            ProcessStartInfo startUpdater = new ProcessStartInfo(updaterExecutable);
                            startUpdater.Arguments = string.Join(" ", updaterArgs);
                            startUpdater.UseShellExecute = false;
                            Process.Start(startUpdater);

                            // CLOSE
                            Application.Exit();
                        }
                        else
                        {
                            if (app_ShowSplashScreen)
                            {
                                // SHOW SPLASH SCREEN
                                Application.Run(new SplashScreen());
                            }

                            // RUN NEW APPLICATION INSTANCE
                            Application.Run(new CheckerMainForm());
                        }
                    }
                }
            }
        }

        public static bool RequiredLibraryExists(string fileName)
        {
            if (!File.Exists(Path.Combine(app_CurrentWorkingDir, fileName)))
            {
                MessageBox.Show("Required library \"" + fileName + "\" not found in current working directory \""
                    + app_CurrentWorkingDir + "\".", "Endpoint Status Checker v" + app_VersionString,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }
            else
            {
                return true;
            }
        }

        // If assemblyName is not fully qualified, a random matching may be 
        public static string QueryAssemblyInfo(string assemblyName)
        {
            ASSEMBLY_INFO assembyInfo = new ASSEMBLY_INFO();
            assembyInfo.cchBuf = 512;
            assembyInfo.currentAssemblyPath = new string('\0',
                assembyInfo.cchBuf);

            IAssemblyCache assemblyCache = null;

            // Get IAssemblyCache pointer
            IntPtr hr = GacApi.CreateAssemblyCache(out assemblyCache, 0);
            if (hr == IntPtr.Zero)
            {
                hr = assemblyCache.QueryAssemblyInfo(1, assemblyName, ref assembyInfo);
                if (hr != IntPtr.Zero)
                {
                    Marshal.ThrowExceptionForHR(hr.ToInt32());
                }
            }
            else
            {
                Marshal.ThrowExceptionForHR(hr.ToInt32());
            }
            return assembyInfo.currentAssemblyPath;
        }

        static string RetrieveLinkerTimestamp()
        {
            string filePath = Assembly.GetCallingAssembly().Location;
            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;
            byte[] b = new byte[2048];
            Stream s = null;

            try
            {
                s = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                s.Read(b, 0, 2048);
            }
            finally
            {
                if (s != null)
                {
                    s.Close();

                    GC.Collect();
                }
            }

            int i = BitConverter.ToInt32(b, c_PeHeaderOffset);
            int secondsSince1970 = BitConverter.ToInt32(b, i + c_LinkerTimestampOffset);
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0);
            dt = dt.AddSeconds(secondsSince1970);
            dt = dt.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(dt).Hours);
            return dt.ToString("dd.MM.yyyy HH:mm");
        }

        public static string GetVersionString(Version version, bool addBuildNumber, bool addRevisionNumber)
        {
            string versionString = version.Major.ToString() +
                                   "." +
                                   version.Minor.ToString();

            if (addBuildNumber)
            {
                versionString += (".") + version.Build.ToString();

                if (addRevisionNumber)
                {
                    versionString += (".") + version.Revision.ToString();
                }
            }

            return versionString;
        }

        public static void CheckForUpdate()
        {
            try
            {
                using (WebClient updateWC = new WebClient())
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    // GET LATEST VERSION NUMBER
                    app_LatestPackageVersion = new Version(
                        updateWC
                        .DownloadString(
                            "https://raw.githubusercontent.com/ThePhOeNiX810815/Endpoint-Status-Checker/Main-Dev-Branch/version.txt")
                        .TrimEnd());

                    // GET LATEST PACKAGE INFO
                    string[] app_LatestPackageInfo =
                        updateWC
                        .DownloadString(
                            "https://raw.githubusercontent.com/ThePhOeNiX810815/Endpoint-Status-Checker/Main-Dev-Branch/package.txt")
                        .TrimEnd()
                        .Split(
                            new string[] { "\n" },
                            StringSplitOptions.None);

                    app_LatestPackageLink = app_LatestPackageInfo[0];
                    app_LatestPackageDate = app_LatestPackageInfo[1];

                    // GET LATEST VERSION RELEASE NOTES
                    app_LatestPackageReleaseNotes_RTF =
                        updateWC
                        .DownloadString(
                            "https://raw.githubusercontent.com/ThePhOeNiX810815/Endpoint-Status-Checker/Main-Dev-Branch/release_notes.rtf");

                    if (app_LatestPackageVersion > app_Version &&
                        app_LatestPackageVersion > app_AutoUpdate_SkipVersion)
                    {
                        if (app_AutoUpdate_AutoUpdateInFuture)
                        {
                            // AUTO UPDATE
                            app_AutoUpdate = true;
                        }
                        else
                        {
                            // SHOW NEW VERSION DIALOG
                            NewVersionDialog newVersionDialog = new NewVersionDialog();
                            newVersionDialog.ShowDialog();

                            if (newVersionDialog.updateInFuture)
                            {
                                Settings.Default.AutoUpdate_AutoUpdateInFuture = true;
                                Settings.Default.Save();
                            }

                            if (newVersionDialog.updateNow)
                            {
                                app_AutoUpdate = true;
                            }
                            else if (newVersionDialog.updateSkip)
                            {
                                Settings.Default.AutoUpdate_SkipVersion = app_LatestPackageVersion.ToString();
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public static bool IsOriginalSignedExecutable()
        {
            bool isOriginalSignedExecutable = false;

            try
            {
                // GET SIGNING CERT
                X509Certificate2 app_SigningAuthCertificate = new X509Certificate2(X509Certificate.CreateFromSignedFile(app_Assembly.Location));

                // VALIDATE SIGNING CERT
                isOriginalSignedExecutable =
                    app_SigningAuthCertificate.GetSerialNumberString().Equals("4C0D5A65225EE4A0") &&
                    app_SigningAuthCertificate.Issuer.Equals("CN=Peter Machaj Root CA") &&
                    app_SigningAuthCertificate.Subject.Equals("CN=Peter Machaj");
            }
            catch
            {
            }

            return isOriginalSignedExecutable;
        }
    }
}
