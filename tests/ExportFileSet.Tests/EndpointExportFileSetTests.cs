using System;
using System.IO;

namespace EndpointChecker
{
    internal static class EndpointExportFileSetTests
    {
        private static int failed;

        private static void Main()
        {
            Run("File set preserves export file names", FileSetPreservesExportFileNames);
            Run("File set combines directory and file names", FileSetCombinesDirectoryAndFileNames);

            if (failed > 0)
            {
                Environment.Exit(1);
            }
        }

        private static void FileSetPreservesExportFileNames()
        {
            EndpointExportFileSet files = CreateFileSet();

            AssertEqual("EndpointsStatus.xlsx", files.XlsxFileName);
            AssertEqual("EndpointsStatus.json", files.JsonFileName);
            AssertEqual("EndpointsStatus.xml", files.XmlFileName);
            AssertEqual("EndpointsStatus_Info.html", files.HtmlInfoFileName);
            AssertEqual("EndpointsStatus_HTTP.html", files.HtmlHttpFileName);
            AssertEqual("EndpointsStatus_FTP.html", files.HtmlFtpFileName);
        }

        private static void FileSetCombinesDirectoryAndFileNames()
        {
            EndpointExportFileSet files = CreateFileSet();

            AssertEqual(Path.Combine("/tmp/export", "EndpointsStatus.xlsx"), files.XlsxPath);
            AssertEqual(Path.Combine("/tmp/export", "EndpointsStatus.json"), files.JsonPath);
            AssertEqual(Path.Combine("/tmp/export", "EndpointsStatus.xml"), files.XmlPath);
            AssertEqual(Path.Combine("/tmp/export", "EndpointsStatus_Info.html"), files.HtmlInfoPath);
            AssertEqual(Path.Combine("/tmp/export", "EndpointsStatus_HTTP.html"), files.HtmlHttpPath);
            AssertEqual(Path.Combine("/tmp/export", "EndpointsStatus_FTP.html"), files.HtmlFtpPath);
        }

        private static EndpointExportFileSet CreateFileSet()
        {
            return EndpointExportFileSet.Create(
                "/tmp/export",
                "EndpointsStatus.xlsx",
                "EndpointsStatus.json",
                "EndpointsStatus.xml",
                "EndpointsStatus_Info.html",
                "EndpointsStatus_HTTP.html",
                "EndpointsStatus_FTP.html");
        }

        private static void Run(string name, Action test)
        {
            try
            {
                test();
                Console.WriteLine("PASS " + name);
            }
            catch (Exception exception)
            {
                failed++;
                Console.WriteLine("FAIL " + name + ": " + exception.Message);
            }
        }

        private static void AssertEqual<T>(T expected, T actual)
        {
            if (!object.Equals(expected, actual))
            {
                throw new InvalidOperationException("Expected <" + expected + "> but was <" + actual + ">.");
            }
        }
    }
}
