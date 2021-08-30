using System;
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

        public static string systemMemorySize;
        public static string assembly_ApplicationName = "Endpoint Status Checker";
        public static string assembly_CurrentWorkingDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string assembly_BuiltDate = RetrieveLinkerTimestamp();
        public static string assembly_Copyright = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location).LegalCopyright;
        public static string assembly_Version = Assembly.GetExecutingAssembly().GetName().Version.Major.ToString() + "." +
                                       Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString();

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
                         RequiredLibraryExists("DomainPublicSuffix.dll") &&
                         RequiredLibraryExists("tracert.dll") &&
                         RequiredLibraryExists("Spire.Common.dll") &&
                         RequiredLibraryExists("Spire.License.dll") &&
                         RequiredLibraryExists("Spire.XLS.dll") &&
                         RequiredLibraryExists("Spire.Pdf.dll") &&
                         RequiredLibraryExists("IPAddressRange.dll") &&
                         RequiredLibraryExists("WhoisClient.dll") &&
                         RequiredLibraryExists("Newtonsoft.Json.dll") &&
                         RequiredLibraryExists("Microsoft.WindowsAPICodePack.dll") &&
                         RequiredLibraryExists("Microsoft.WindowsAPICodePack.Shell.dll") &&
                         RequiredLibraryExists("VirusTotal.NET.dll") &&
                         RequiredLibraryExists("NSpeedTest.dll"))
                {
                    // RUN NEW APPLICATION INSTANCE
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                    Application.Run(new CheckerMainForm());
                }
            }
        }

        public static bool RequiredLibraryExists(string fileName)
        {
            if (!File.Exists(Path.Combine(assembly_CurrentWorkingDir, fileName)))
            {
                MessageBox.Show("Required library \"" + fileName + "\" not found in current working directory \""
                    + assembly_CurrentWorkingDir + "\".", "Endpoint Status Checker v" + assembly_Version,
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
    }
}
