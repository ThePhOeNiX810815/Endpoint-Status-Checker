using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
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
            String assemblyName,
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
        public String currentAssemblyPath;

        public int cchBuf;
    }

    static class Program
    {
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);

        // APPLICATION SPECIFICATION
        public static string systemMemorySize;
        public static string assembly_ApplicationName = "Endpoint Status Checker";
        public static string assembly_CurrentWorkingDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string assembly_BuiltDate = RetrieveLinkerTimestamp();
        public static string assembly_Copyright = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location).LegalCopyright;

        public static Version assembly_Version = Assembly.GetExecutingAssembly().GetName().Version;
        public static string assembly_VersionString = GetVersionString(assembly_Version, true, false); 

        // FEEDBACK AND EXCEPTION HANDLING E-MAIL ADDRESSES
        public static string exceptionReport_senderEMailAddress = "ExceptionReport@EndpointStatusChecker";
        public static string featureRequest_senderEMailAddress = "FeatureRequest@EndpointStatusChecker";
        public static string authorEmailAddress = "phoenixvm@gmail.com";
        public static string anonymousFTPPassword = "anonymous";

        // ENDPOINTS DEFINITIONS FILE NAME
        public static string endpointDefinitonsFile = "EndpointChecker_EndpointsList.txt";

        // GOOGLE MAPS API KEY
        public static string apiKey_GoogleMaps = string.Empty;
        public static int googleMapsZoomFactor = 0;

        // VIRUSTOTAL API KEY
        public static string apiKey_VirusTotal = string.Empty;

        // HTTP CLIENT USER-AGENT STRING
        public static string http_UserAgent = string.Empty;

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
        public static string statusExport_Directory = Program.assembly_CurrentWorkingDir;
        public static string statusExport_XLSFile = "EndpointsStatus.xlsx";
        public static string statusExport_JSONFile = "EndpointsStatus.json";
        public static string statusExport_XMLFile = "EndpointsStatus.xml";
        public static string statusExport_HTMLFile_InfoPage = "EndpointsStatus_Info.html";
        public static string statusExport_HTMLFile_HTTPPage = "EndpointsStatus_HTTP.html";
        public static string statusExport_HTMLFile_FTPPage = "EndpointsStatus_FTP.html";

        // MAXIMUM LENGHT OF HTTP RESPONSE TO READ
        public static long http_SaveResponse_MaxLenght_Bytes = 0;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // GET SYSTEM MEMORY SIZE
            long systemMemorySize_KB;
            GetPhysicallyInstalledSystemMemory(out systemMemorySize_KB);
            systemMemorySize = (systemMemorySize_KB / 1024 / 1024).ToString() + " GB";

            // CHECK APPLICATION INSTANCE
            bool createdNew = true;
            using (Mutex mainMutex = new Mutex(true, "Endpoint Checker Instance", out createdNew))
            {
                if (!createdNew)
                {
                    // ANOTHER APPLICATION INSTANCE IS ALREADY RUNNING, CREATE CHECK FILE
                    try
                    {
                        if (File.Exists(Path.GetTempPath() + @"\EndpointCheckerInstanceRunning"))
                        {
                            File.Delete(Path.GetTempPath() + @"\EndpointCheckerInstanceRunning");
                        }

                        File.Create(Path.GetTempPath() + @"\EndpointCheckerInstanceRunning").Dispose();
                    }
                    catch
                    {
                    }
                }
                else if (RequiredLibraryExists("ClosedXML.dll") &&
                         RequiredLibraryExists("DocumentFormat.OpenXml.dll") &&
                         RequiredLibraryExists("ExcelNumberFormat.dll") &&
                         RequiredLibraryExists("FastMember.dll") &&
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
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

                    // SHOW SPLASH SCREEN
                    Application.Run(new SplashScreen());

                    // RUN NEW APPLICATION INSTANCE
                    Application.Run(new CheckerMainForm());
                }
            }
        }

        public static bool RequiredLibraryExists(string fileName)
        {
            if (!File.Exists(Path.Combine(assembly_CurrentWorkingDir, fileName)))
            {
                MessageBox.Show("Required library \"" + fileName + "\" not found in current working directory \""
                    + assembly_CurrentWorkingDir + "\".", "Endpoint Status Checker v" + assembly_VersionString,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }
            else
            {
                return true;
            }
        }

        // If assemblyName is not fully qualified, a random matching may be 
        public static String QueryAssemblyInfo(String assemblyName)
        {
            ASSEMBLY_INFO assembyInfo = new ASSEMBLY_INFO();
            assembyInfo.cchBuf = 512;
            assembyInfo.currentAssemblyPath = new String('\0',
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
    }
}
