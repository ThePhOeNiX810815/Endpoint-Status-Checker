using EndpointChecker.Properties;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows.Forms;

namespace EndpointChecker
{
    internal static class Program
    {
        // TEST MODE SWITCH
        public static bool app_TestMode = false;

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string className, string windowTitle);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);

        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowPlacement(IntPtr hWnd, ref Windowplacement lpwndpl);

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
        public static bool app_ScanOnStartup;
        public static bool app_ShowSplashScreen;
        public static string app_ApplicationName = FileVersionInfo.GetVersionInfo(app_Assembly.Location).ProductName;
        public static string app_ApplicationExecutableName = AppDomain.CurrentDomain.FriendlyName;
        public static string app_CurrentWorkingDir = Path.GetDirectoryName(app_Assembly.Location);
        public static string app_TempDir = Path.GetTempPath();
        public static string app_Built_Date = RetrieveLinkerTimestamp(false);
        public static string app_Built_DateTime = RetrieveLinkerTimestamp(true);
        public static string app_Copyright = FileVersionInfo.GetVersionInfo(app_Assembly.Location).LegalCopyright;
        public static string app_Title = app_ApplicationName + " v" + app_VersionString;

        // FEEDBACK AND EXCEPTION E-MAIL REPORT SENDER AND RECIPIENT ADDRESS 
        public static MailAddress report_Recipient = new MailAddress("petermachaj@e.email");

        // ANONYMOUS FTP DEFAULT PASSWORD
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

        // HTTP CLIENT USER-AGENT STRING
        public static string http_UserAgent;

        // STRING FORMAT FOR 'NOT AVAILABLE' STATUS
        public static string status_NotAvailable = "N/A";

        // STRING FORMAT FOR 'ERROR' STATUS
        public static string status_Error = "ERROR";

        // OS ID STRING
        public static string os_VersionString = status_NotAvailable;

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
        public static bool app_AutoUpdateNow = false;
        public static bool app_UpdateAvailable = false;
        public static Version app_LatestPackageVersion = app_Version;
        public static string app_LatestPackageLink = string.Empty;
        public static string app_LatestPackageDate = string.Empty;
        public static string app_LatestPackageReleaseNotes_RTF = string.Empty;

        // SIGNING CERTIFICATE
        public static bool app_IsOriginalSignedExecutable = IsOriginalSignedExecutable();

        // REQUIRED LIBRARIES LIST
        public static string[] app_RequiredLibsList = new string[]
        {
            "AGauge.dll",
            "ClosedXML.dll",
            "DocumentFormat.OpenXml.dll",
            "ExcelNumberFormat.dll",
            "FastMember.dll",
            "Flurl.dll",
            "HtmlAgilityPack.dll",
            "IPAddressRange.dll",
            "Microsoft.WindowsAPICodePack.dll",
            "Microsoft.WindowsAPICodePack.Shell.dll",
            "Nager.PublicSuffix.dll",
            "Newtonsoft.Json.dll",
            "Spire.XLS.dll",
            "tracert.dll",
            "VirusTotal.NET.dll",
            "WhoisClient.dll"
        };

        // EMBEDDED CUSTOM FONTS LIST
        public static string[] app_CustomFontsUsed = new string[]
        {
            "AGENCYB.TTF",
            "AGENCYR.TTF",
            "segoeui.ttf",
            "segoeuib.ttf",
            "segoeuil.ttf",
            "segoeuisl.ttf",
            "seguibl.ttf",
            "seguisb.ttf"
        };

        // .NET FRAMEWORK VERSION ENUM
        public enum DotNETFrameworkVersion
        {
            v4_8_1,
            v4_8,
            v4_7_2,
            v4_7_1,
            v4_7,
            v4_6_2,
            v4_6_1,
            v4_6,
            v4_5_2,
            v4_5_1,
            v4_5,
            NONE
        };

        // MINIMUM REQUIRED OS VERSION
        // MIN OS VERSION: 6.1 [Windows 7 / Server 2008 R2]
        public static Version app_Minimum_OS_Version = new Version(6, 1);

        // MINIMUM REQUIRED .NET FRAMEWORK VERSION
        // IN.NET FWK VERSION: 4.5
        public static DotNETFrameworkVersion app_Minimum_DotNETFWK_Version = DotNETFrameworkVersion.v4_5;

        // LATEST .NET FRAMEWORK VERSION INSTALLED
        public static DotNETFrameworkVersion app_Latest_DotNETFWK_Version_Installed = DotNETFrameworkVersion.NONE;

        [STAThread]
        public static void Main(string[] args)
        {
            // CHECK FOR MINIMAL REQUIRED OS AND .NET FRAMEWORK VERSIOMS
            // 
            // 
            if (Environment.OSVersion.Version < app_Minimum_OS_Version)
            {
                MessageBox.Show(
                        "This application requires Windows 7 / Server 2008 R2 or later to run.",
                        "Endpoint Status Checker v" +
                        app_VersionString,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (!IsRequiredNetFwkVersionInstalled(app_Minimum_DotNETFWK_Version))
            {
                MessageBox.Show(
                         "This application requires .NET Framework 4.5 or later to run.",
                         "Endpoint Status Checker v" +
                         app_VersionString,
                         MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                // SET LATEST SUPPORTED SECURITY PROTOCOL FOR HTTP (TLS)
                SetSecurityProtocol();

                // PUBLISH EMBEDDED FONTS
                PublishEmbeddedFonts();

                if (Settings.Default.UpgradeRequired)
                {
                    // UPGRADE SETTINGS FROM PREVIOUS VERSION
                    Settings.Default.Upgrade();
                    Settings.Default.UpgradeRequired = false;
                    Settings.Default.Save();
                }

                // GET OS VERSION STRING
                if (GetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName", out object reg_osVersion_ProductName))
                {
                    os_VersionString =
                        (string)reg_osVersion_ProductName +
                        " " +
                        (Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit") +
                        ", Build " +
                        Environment.OSVersion.Version.Build.ToString();

                    if (GetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows NT\CurrentVersion", "DisplayVersion", out object reg_osVersion_DisplayVersion))
                    {
                        os_VersionString += " (" + reg_osVersion_DisplayVersion + ")";
                    }
                }

                // GET SETTINGS
                LoadApplicationConfig();

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
                    GetPhysicallyInstalledSystemMemory(out long systemMemorySize_KB);
                    systemMemorySize = (systemMemorySize_KB / 1024 / 1024).ToString() + " GB";

                    // CHECK APPLICATION INSTANCE
                    using (Mutex mainMutex = new Mutex(true, app_ApplicationName, out bool createdNew))
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
                        else if (RequiredLibrariesExists(app_RequiredLibsList))
                        {
                            CheckForUpdate();

                            if (app_AutoUpdateNow)
                            {
                                ExecuteUpdater();

                                Environment.Exit(0);
                            }
                            else
                            {
                                if (app_ShowSplashScreen ||
                                    app_TestMode)
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
        }

        public static bool RequiredLibrariesExists(string[] librariesList)
        {
            bool libabriesPresent = true;

            foreach (string library in librariesList)
            {
                if (!File.Exists(Path.Combine(app_CurrentWorkingDir, library)))
                {
                    MessageBox.Show(
                        "Referenced library \"" +
                        library +
                        "\" not found in \"" +
                        app_CurrentWorkingDir +
                        "\"." +
                        Environment.NewLine +
                        Environment.NewLine +
                        "Check all required libraries are present in application directory.",
                        "Endpoint Status Checker v" +
                        app_VersionString,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    libabriesPresent = false;
                    break;
                }
            }

            return libabriesPresent;
        }

        // If assemblyName is not fully qualified, a random matching may be 
        public static string QueryAssemblyInfo(string assemblyName)
        {
            ASSEMBLY_INFO assembyInfo = new ASSEMBLY_INFO
            {
                cchBuf = 512
            };
            assembyInfo.currentAssemblyPath = new string('\0',
                assembyInfo.cchBuf);


            // Get IAssemblyCache pointer
            IntPtr hr = GacApi.CreateAssemblyCache(out IAssemblyCache assemblyCache, 0);
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

        private static string RetrieveLinkerTimestamp(bool appendTime)
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
                }
            }

            int i = BitConverter.ToInt32(b, c_PeHeaderOffset);
            int secondsSince1970 = BitConverter.ToInt32(b, i + c_LinkerTimestampOffset);
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0);
            dt = dt.AddSeconds(secondsSince1970);
            dt = dt.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(dt).Hours);

            if (appendTime)
            {
                return dt.ToString("dd.MM.yyyy HH:mm");
            }
            else
            {
                return dt.ToString("dd.MM.yyyy");
            }
        }

        public static string GetVersionString(Version version, bool addBuildNumber, bool addRevisionNumber)
        {
            string versionString = version.Major.ToString() +
                                   "." +
                                   version.Minor.ToString();

            if (addBuildNumber)
            {
                versionString += "." + version.Build.ToString();

                if (addRevisionNumber)
                {
                    versionString += "." + version.Revision.ToString();
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

                    if ((app_LatestPackageVersion > app_Version &&
                         app_LatestPackageVersion > app_AutoUpdate_SkipVersion) || 
                        app_TestMode)
                    {
                        app_UpdateAvailable = true;

                        if (app_AutoUpdate_AutoUpdateInFuture)
                        {
                            // AUTO UPDATE
                            app_AutoUpdateNow = true;
                        }
                        else
                        {
                            // SHOW NEW VERSION DIALOG
                            NewVersionDialog newVersionDialog = new NewVersionDialog();
                            newVersionDialog.ShowDialog();

                            if (newVersionDialog.AutoUpdateInFuture)
                            {
                                Settings.Default.AutoUpdate_AutoUpdateInFuture = true;
                                Settings.Default.Save();
                                LoadApplicationConfig();
                            }

                            if (newVersionDialog.UpdateNow)
                            {
                                app_AutoUpdateNow = true;
                            }
                            else if (newVersionDialog.UpdateSkip)
                            {
                                Settings.Default.AutoUpdate_SkipVersion = app_LatestPackageVersion.ToString();
                                Settings.Default.Save();
                                LoadApplicationConfig();
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

        public static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            ExceptionNotifier(null, (Exception)args.ExceptionObject, "Raised by UnhandledExceptionHandler", true);
        }

        public static void ThreadExceptionHandler(object sender, ThreadExceptionEventArgs args)
        {
            ExceptionNotifier(null, args.Exception, "Raised by ThreadExceptionHandler", true);
        }

        public static bool GetRegistryValue(string fullKeyName, string valueName, out object value)
        {
            value = Registry.GetValue(fullKeyName, valueName, null);

            return value != null;
        }

        public static bool SetRegistryValue(string fullKeyName, string valueName, object value, RegistryValueKind valueKind)
        {
            try
            {
                Registry.SetValue(fullKeyName, valueName, value, valueKind);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void ExceptionNotifier(object sender, Exception exception, string additionalInfo, bool autoCloseApp, string callerName = "")
        {
            Form senderForm = null;

            if (sender != null &&
                sender is Form)
            {
                senderForm = sender as Form;
                senderForm.Hide();
            }

            StackFrame[] stackFrames = new StackTrace().GetFrames();

            string callingMethod =
                stackFrames[2].GetMethod().Name +
                "() ->> " +
                stackFrames[1].GetMethod().Name +
                "()";

            if (!string.IsNullOrEmpty(callerName))
            {
                callingMethod += " [" + callerName + "]";
            }

            ExceptionDialog exDialog = new ExceptionDialog(
                exception,
                callingMethod,
                additionalInfo,
                new List<MailAddress> { report_Recipient },
                new List<string> { endpointDefinitonsFile },
                autoCloseApp);

            exDialog.ShowDialog();

            if (!autoCloseApp &&
                senderForm != null)
            {
                senderForm.Show();
            }
        }

        public static string NotAvailable_IfNullorEmpty(string input)
        {
            return string.IsNullOrEmpty(input) ? status_NotAvailable : input;
        }

        public static void ExecuteUpdater()
        {
            string currentExecutable = app_Assembly.Location;
            string updaterExecutable = Path.Combine(app_TempDir, "EndpointChecker_AutoUpdater.exe");

            // COPY UPDATER TO TEMP DIRECORY
            File.Copy(currentExecutable, updaterExecutable, true);

            // UPDATER ARGUMENTS
            List<string> updaterArgs = new List<string>
                            {
                                "/AutoUpdate",
                                "\"" + Path.GetDirectoryName(currentExecutable) + "\"",
                                app_ApplicationExecutableName,
                                app_LatestPackageVersion.ToString(),
                                app_LatestPackageLink,
                                app_LatestPackageDate
                            };

            // PREPARE UPDATER
            ProcessStartInfo startUpdater = new ProcessStartInfo(updaterExecutable)
            {
                Arguments = string.Join(" ", updaterArgs),
                UseShellExecute = true
            };

            // EXECUTE UPDATE PROCESS
            Process.Start(startUpdater);
        }

        public static void LoadApplicationConfig()
        {
            app_ScanOnStartup = Settings.Default.Config_ScanOnStartup;
            app_ShowSplashScreen = Settings.Default.Config_ShowSplashScreen;
            apiKey_GoogleMaps = Settings.Default.GoogleMaps_API_Key;
            googleMapsZoomFactor = Settings.Default.GoogleMaps_API_ZoomFactor;
            apiKey_VirusTotal = Settings.Default.VirusTotal_API_Key;
            http_UserAgent = Settings.Default.Config_HTTP_UserAgent;
            http_SaveResponse_MaxLenght_Bytes = Settings.Default.Config_HTTP_SaveResponse_MaxLenght_Bytes;
            app_AutoUpdate_SkipVersion = new Version(Settings.Default.AutoUpdate_SkipVersion);
            app_AutoUpdate_AutoUpdateInFuture = Settings.Default.AutoUpdate_AutoUpdateInFuture;
        }

        public static void PublishEmbeddedFonts()
        {
            foreach (string fontFile in app_CustomFontsUsed)
            {
                FontPublisher.SaveAndPublishFont(fontFile);
            }
        }

        public static bool IsRequiredNetFwkVersionInstalled(DotNETFrameworkVersion requiredVersion)
        {
            bool isInstalled = false;

            try
            {
                using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
                {
                    int releaseKey = Convert.ToInt32(ndpKey.GetValue("Release"));

                    if (releaseKey >= 533320)
                        app_Latest_DotNETFWK_Version_Installed = DotNETFrameworkVersion.v4_8_1;
                    if (releaseKey >= 528040)
                        app_Latest_DotNETFWK_Version_Installed = DotNETFrameworkVersion.v4_8;
                    if (releaseKey >= 461808)
                        app_Latest_DotNETFWK_Version_Installed = DotNETFrameworkVersion.v4_7_2;
                    if (releaseKey >= 461308)
                        app_Latest_DotNETFWK_Version_Installed = DotNETFrameworkVersion.v4_7_1;
                    if (releaseKey >= 460798)
                        app_Latest_DotNETFWK_Version_Installed = DotNETFrameworkVersion.v4_7;
                    if (releaseKey >= 394802)
                        app_Latest_DotNETFWK_Version_Installed = DotNETFrameworkVersion.v4_6_2;
                    if (releaseKey >= 394254)
                        app_Latest_DotNETFWK_Version_Installed = DotNETFrameworkVersion.v4_6_1;
                    if (releaseKey >= 393295)
                        app_Latest_DotNETFWK_Version_Installed = DotNETFrameworkVersion.v4_6;
                    if (releaseKey >= 379893)
                        app_Latest_DotNETFWK_Version_Installed = DotNETFrameworkVersion.v4_5_2;
                    if (releaseKey >= 378675)
                        app_Latest_DotNETFWK_Version_Installed = DotNETFrameworkVersion.v4_5_1;
                    if (releaseKey >= 378389)
                        app_Latest_DotNETFWK_Version_Installed = DotNETFrameworkVersion.v4_5;

                    isInstalled = (app_Latest_DotNETFWK_Version_Installed >= requiredVersion);
                }
            }
            catch
            {
            }         

            return isInstalled;
        }

        public static void SetSecurityProtocol()
        {
            try
            {   // try TLS 1.3
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)12288
                                                     | (SecurityProtocolType)3072
                                                     | (SecurityProtocolType)768
                                                     | SecurityProtocolType.Tls;
            }
            catch (NotSupportedException)
            {
                try
                {   // try TLS 1.2
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072
                                                         | (SecurityProtocolType)768
                                                         | SecurityProtocolType.Tls;
                }
                catch (NotSupportedException)
                {
                    try
                    {   // try TLS 1.1
                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)768
                                                             | SecurityProtocolType.Tls;
                    }
                    catch (NotSupportedException)
                    {   // set TLS 1.0
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                    }
                }
            }
        }
    }

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
}