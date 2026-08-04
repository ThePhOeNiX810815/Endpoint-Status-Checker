using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml;
using ClosedXML.Excel;

namespace EndpointChecker
{
    internal static class EndpointStructuredExportGeneratorTests
    {
        private static int failed;

        private static void Main()
        {
            Run("JSON export remains indented", JsonExportRemainsIndented);
            Run("XML export preserves root shape", XmlExportPreservesRootShape);
            Run("XML export preserves Encoding plus workaround", XmlExportPreservesEncodingPlusWorkaround);
            Run("HTML export adds auto refresh meta tag", HtmlExportAddsAutoRefreshMetaTag);
            Run("HTML export injects endpoint row hyperlink click handler", HtmlExportInjectsEndpointRowHyperlinkClickHandler);
            Run("HTML export injects refresh button CSS block", HtmlExportInjectsRefreshButtonCssBlock);
            Run("HTML summary export replaces placeholders with expected links", HtmlSummaryExportReplacesPlaceholdersWithExpectedLinks);
            Run("HTML hyperlink transformation preserves legacy ampersand escaping", HtmlHyperlinkTransformationPreservesLegacyAmpersandEscaping);
            Run("XLSX export workbook remains readable with expected sheets", XlsxExportWorkbookRemainsReadableWithExpectedSheets);
            Run("XLSX export preserves worksheet column order", XlsxExportPreservesWorksheetColumnOrder);
            Run("XLSX export preserves representative cell values", XlsxExportPreservesRepresentativeCellValues);
            Run("XLSX export preserves hidden column behavior", XlsxExportPreservesHiddenColumnBehavior);
            Run("XLSX export preserves key formatting", XlsxExportPreservesKeyFormatting);
            Run("XLSX export preserves worksheet deletion rules", XlsxExportPreservesWorksheetDeletionRules);

            if (failed > 0)
            {
                Environment.Exit(1);
            }
        }

        private static void JsonExportRemainsIndented()
        {
            string json = EndpointStructuredExportGenerator.CreateJson(new[]
            {
                new EndpointDefinition { Name = "Endpoint A", Address = "https://example.test" }
            });

            AssertContains(Environment.NewLine, json);
            AssertContains("  \"Name\": \"Endpoint A\"", json);
            AssertContains("  \"Address\": \"https://example.test\"", json);
        }

        private static void XmlExportPreservesRootShape()
        {
            string json = EndpointStructuredExportGenerator.CreateJson(new[]
            {
                new EndpointDefinition { Name = "Endpoint A", Address = "https://example.test" }
            });

            XmlDocument document = EndpointStructuredExportGenerator.CreateXmlDocument(json);

            AssertEqual("EndpointStatus", document.DocumentElement.Name);
            AssertEqual("Endpoint A", document.DocumentElement.SelectSingleNode("EndpointStatus/Name").InnerText);
            AssertEqual("https://example.test", document.DocumentElement.SelectSingleNode("EndpointStatus/Address").InnerText);
        }

        private static void XmlExportPreservesEncodingPlusWorkaround()
        {
            string json = "[{\"Encoding+Name\":\"utf-8\"}]";

            XmlDocument document = EndpointStructuredExportGenerator.CreateXmlDocument(json);

            AssertEqual("utf-8", document.DocumentElement.SelectSingleNode("EndpointStatus/Encoding_Name").InnerText);
        }

        private static void HtmlExportAddsAutoRefreshMetaTag()
        {
            string inputHtml = "<html><head><title>x</title></head><body></body></html>";
            string outputHtml = EndpointHtmlExportTransformer.AddAutoRefreshMetaTag(inputHtml, 30);

            AssertContains("<meta http-equiv=\"refresh\" content=\"30\">", outputHtml);
        }

        private static void HtmlExportInjectsEndpointRowHyperlinkClickHandler()
        {
            string inputHtml = @"<html>
<head>
<style type=""text/css"">
.X1{text-decoration:underline;color:#00f;}
</style>
</head>
<body>
<table>
<tr><td>Header</td><td>Header</td><td>Header</td><td><a>Address</a></td></tr>
<tr><td>Name</td><td>HTTP</td><td>443</td><td><a>https://example.test</a></td></tr>
</table>
</body>
</html>";

            string outputHtml = EndpointHtmlExportTransformer.CreateEndpointUrlHyperLinks(inputHtml);

            AssertContains("onclick=\"location.href = 'https://example.test'\"", outputHtml);
            AssertContains("cursor:pointer;", outputHtml);
        }

        private static void HtmlExportInjectsRefreshButtonCssBlock()
        {
            string inputHtml = "<html xmlns=\"http://www.w3.org/1999/xhtml\">" + Environment.NewLine +
                               "  <head>" + Environment.NewLine +
                               "    <style type=\"text/css\">table";

            string outputHtml = EndpointHtmlExportTransformer.AddRefreshCssButton(inputHtml);

            AssertContains("ID=\"refreshBTN\"", outputHtml);
            AssertContains("window.location.reload()", outputHtml);
            AssertContains("cursor:pointer;", outputHtml);
        }

        private static void HtmlSummaryExportReplacesPlaceholdersWithExpectedLinks()
        {
            string inputHtml = "<html><head><title>Summary</title></head><body>xHTML_XLSXx xHTML_JSONx xHTML_XMLx xHTML_HTTPx xHTML_FTPx</body></html>";

            string outputHtml = EndpointHtmlExportTransformer.ReplaceSummaryHyperLinkPlaceholders(
                inputHtml,
                "Endpoints_Status.xlsx",
                "Endpoints_Status.json",
                "Endpoints_Status.xml",
                "Endpoints_Status_HTTP.html",
                "Endpoints_Status_FTP.html");

            outputHtml = EndpointHtmlExportTransformer.AddAutoRefreshMetaTag(outputHtml, 30);

            AssertContains("<meta http-equiv=\"refresh\" content=\"30\">", outputHtml);
            AssertContains("<a href=\"Endpoints_Status.xlsx\" style=\"color:white;\">Endpoints Status XLSX Export</a>", outputHtml);
            AssertContains("<a href=\"Endpoints_Status.json\" style=\"color:white;\">Endpoints Status JSON Export</a>", outputHtml);
            AssertContains("<a href=\"Endpoints_Status.xml\" style=\"color:white;\">Endpoints Status XML Export</a>", outputHtml);
            AssertContains("<a href=\"Endpoints_Status_HTTP.html\" style=\"color:white;\">HTTP Endpoints Status List</a>", outputHtml);
            AssertContains("<a href=\"Endpoints_Status_FTP.html\" style=\"color:white;\">FTP Endpoints Status List</a>", outputHtml);
        }

        private static void HtmlHyperlinkTransformationPreservesLegacyAmpersandEscaping()
        {
            string inputHtml = @"<html>
<head>
<style type=""text/css"">
.X1{text-decoration:underline;color:#00f;}
</style>
</head>
<body>
<table>
<tr><td>Header</td><td>Header</td><td>Header</td><td><a>Address</a></td></tr>
<tr><td>Name</td><td>HTTP</td><td>443</td><td><a>https://example.test/path?x=1&amp;y=2</a></td></tr>
</table>
</body>
</html>";

            string outputHtml = EndpointHtmlExportTransformer.CreateEndpointUrlHyperLinks(inputHtml);

            AssertContains("onclick=\"location.href = 'https://example.test/path?x=1&amp;amp;amp;y=2'\"", outputHtml);
        }

        private static void XlsxExportWorkbookRemainsReadableWithExpectedSheets()
        {
            EndpointXlsxExportWorkbookInput input = CreateWorkbookInput();

            using (XLWorkbook workbook = EndpointXlsxExportWorkbookBuilder.Build(input))
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;

                    using (XLWorkbook reloadedWorkbook = new XLWorkbook(stream))
                    {
                        AssertEqual(3, reloadedWorkbook.Worksheets.Count);
                        AssertNotNull(reloadedWorkbook.Worksheet("Summary"));
                        AssertNotNull(reloadedWorkbook.Worksheet("HTTP Endpoints"));
                        AssertNotNull(reloadedWorkbook.Worksheet("FTP Endpoints"));
                    }
                }
            }
        }

        private static void XlsxExportPreservesWorksheetColumnOrder()
        {
            EndpointXlsxExportWorkbookInput input = CreateWorkbookInput();

            using (XLWorkbook workbook = EndpointXlsxExportWorkbookBuilder.Build(input))
            {
                IXLWorksheet httpWorksheet = workbook.Worksheet("HTTP Endpoints");
                IXLWorksheet ftpWorksheet = workbook.Worksheet("FTP Endpoints");

                string[] expectedHttpHeaders =
                {
                    "Endpoint Name",
                    "Protocol",
                    "Target Port",
                    "Endpoint Response URL",
                    "Endpoint IP Address(es)",
                    "Endpoint NIC MAC Address(es)",
                    "Endpoint DNS Name(s)",
                    "Response Time",
                    "Status Code",
                    "Status Message",
                    "Last Seen Online",
                    "Ping Roundtrip Time",
                    "UserName [Basic Auth]",
                    "Network Share(s)",
                    "HTTP Server ID",
                    "HTTP Auto Redirects",
                    "HTTP Content Type",
                    "HTTP Content Length",
                    "HTTP Expires",
                    "HTTP ETag",
                    "HTTP Encoding",
                    "HTML Encoding",
                    "HTML Page Title",
                    "HTML Page Author",
                    "HTML Page Description",
                    "HTML Content Language",
                    "HTML Theme Color",
                    "HTML Page Links Count",
                };

                for (int index = 0; index < expectedHttpHeaders.Length; index++)
                {
                    AssertEqual(expectedHttpHeaders[index], httpWorksheet.Cell(1, index + 1).GetString());
                }

                string[] expectedFtpHeaders =
                {
                    "Endpoint Name",
                    "Protocol",
                    "Target Port",
                    "Endpoint Response URL",
                    "Endpoint IP Address(es)",
                    "Endpoint NIC MAC Address(es)",
                    "Endpoint DNS Name(s)",
                    "Response Time",
                    "Status Code",
                    "Status Message",
                    "Last Seen Online",
                    "Ping Roundtrip Time",
                    "UserName",
                    "Network Share(s)",
                };

                for (int index = 0; index < expectedFtpHeaders.Length; index++)
                {
                    AssertEqual(expectedFtpHeaders[index], ftpWorksheet.Cell(1, index + 1).GetString());
                }
            }
        }

        private static void XlsxExportPreservesRepresentativeCellValues()
        {
            EndpointXlsxExportWorkbookInput input = CreateWorkbookInput();

            using (XLWorkbook workbook = EndpointXlsxExportWorkbookBuilder.Build(input))
            {
                IXLWorksheet summaryWorksheet = workbook.Worksheet("Summary");
                IXLWorksheet httpWorksheet = workbook.Worksheet("HTTP Endpoints");
                IXLWorksheet ftpWorksheet = workbook.Worksheet("FTP Endpoints");

                AssertEqual("Version 1.2.3 (built 2026-08-04)", summaryWorksheet.Cell("B1").GetString());
                AssertEqual("1", summaryWorksheet.Cell("E4").GetString());
                AssertEqual("1", summaryWorksheet.Cell("E5").GetString());

                AssertEqual("HTTP Endpoint", httpWorksheet.Cell("A2").GetString());
                AssertEqual("HTTPS", httpWorksheet.Cell("B2").GetString());
                AssertEqual("https://status.example.test/health", httpWorksheet.Cell("D2").GetString());
                AssertEqual("application/json", httpWorksheet.Cell("Q2").GetString());
                AssertEqual("2", httpWorksheet.Cell("AB2").GetString());

                Uri httpHyperlink = httpWorksheet.Cell("D2").GetHyperlink().ExternalAddress;
                AssertEqual(new Uri("https://status.example.test/health"), httpHyperlink);

                AssertEqual("FTP Endpoint", ftpWorksheet.Cell("A2").GetString());
                AssertEqual("FTP", ftpWorksheet.Cell("B2").GetString());
                AssertEqual("ftp://ftp.example.test/files", ftpWorksheet.Cell("D2").GetString());
            }
        }

        private static void XlsxExportPreservesHiddenColumnBehavior()
        {
            EndpointXlsxExportWorkbookInput input = CreateWorkbookInput();

            using (XLWorkbook workbook = EndpointXlsxExportWorkbookBuilder.Build(input))
            {
                IXLWorksheet httpWorksheet = workbook.Worksheet("HTTP Endpoints");
                IXLWorksheet ftpWorksheet = workbook.Worksheet("FTP Endpoints");

                AssertTrue(httpWorksheet.Column("X").IsHidden);
                AssertFalse(httpWorksheet.Column("Q").IsHidden);
                AssertTrue(ftpWorksheet.Column("N").IsHidden);
                AssertFalse(ftpWorksheet.Column("J").IsHidden);
            }
        }

        private static void XlsxExportPreservesKeyFormatting()
        {
            EndpointXlsxExportWorkbookInput input = CreateWorkbookInput();

            using (XLWorkbook workbook = EndpointXlsxExportWorkbookBuilder.Build(input))
            {
                IXLWorksheet summaryWorksheet = workbook.Worksheet("Summary");
                IXLWorksheet httpWorksheet = workbook.Worksheet("HTTP Endpoints");

                AssertEqual("@", httpWorksheet.Cell("A1").Style.NumberFormat.Format);
                AssertTrue(httpWorksheet.SheetView.SplitRow > 0);
                AssertTrue(httpWorksheet.SheetView.SplitColumn > 0);

                AssertColor(XLColor.CoolGrey.Color, httpWorksheet.Cell("A1").Style.Fill.BackgroundColor.Color);
                AssertColor(Color.LightGreen, httpWorksheet.Cell("A2").Style.Fill.BackgroundColor.Color);
                AssertColor(XLColor.CoolGrey.Color, summaryWorksheet.Cell("A1").Style.Fill.BackgroundColor.Color);
                AssertColor(XLColor.LightBlue.Color, summaryWorksheet.Cell("B1").Style.Fill.BackgroundColor.Color);
            }
        }

        private static void XlsxExportPreservesWorksheetDeletionRules()
        {
            EndpointXlsxExportWorkbookInput ftpOnlyInput = CreateWorkbookInput();
            ftpOnlyInput.HttpRows = new List<EndpointXlsxHttpRow>();

            using (XLWorkbook ftpOnlyWorkbook = EndpointXlsxExportWorkbookBuilder.Build(ftpOnlyInput))
            {
                AssertFalse(ftpOnlyWorkbook.Worksheets.Any(w => w.Name == "HTTP Endpoints"));
                AssertTrue(ftpOnlyWorkbook.Worksheets.Any(w => w.Name == "FTP Endpoints"));
            }

            EndpointXlsxExportWorkbookInput httpOnlyInput = CreateWorkbookInput();
            httpOnlyInput.FtpRows = new List<EndpointXlsxFtpRow>();

            using (XLWorkbook httpOnlyWorkbook = EndpointXlsxExportWorkbookBuilder.Build(httpOnlyInput))
            {
                AssertTrue(httpOnlyWorkbook.Worksheets.Any(w => w.Name == "HTTP Endpoints"));
                AssertFalse(httpOnlyWorkbook.Worksheets.Any(w => w.Name == "FTP Endpoints"));
            }
        }

        private static EndpointXlsxExportWorkbookInput CreateWorkbookInput()
        {
            return new EndpointXlsxExportWorkbookInput
            {
                Author = "Endpoint Checker",
                StatusNotAvailable = "N/A",
                Summary = new EndpointXlsxExportSummaryData
                {
                    AppVersion = "Version 1.2.3 (built 2026-08-04)",
                    OperatingSystem = "Windows 11",
                    TargetFramework = ".NET 10",
                    SystemMemory = "32 GB",
                    UserName = "test-user",
                    Domain = "TESTDOMAIN",
                    ComputerName = "TEST-PC",
                    StartDateTime = "2026-08-04 10:00:00",
                    EndDateTime = "2026-08-04 10:00:30",
                    Duration = "30 seconds",
                    ThreadsCount = "5",
                    PingTimeout = "2 seconds",
                    HttpRequestTimeout = "10 seconds",
                    FtpRequestTimeout = "10 seconds",
                    SslCertificateValidation = "Enabled",
                    HttpAutoRedirection = "Enabled",
                    ResolveNetworkShares = "Disabled",
                    ResolvePageMetaInfo = "Enabled",
                    SaveResponse = "Disabled",
                    PingHost = "Enabled",
                    DnsLookupOnHost = "Enabled",
                },
                HttpRows = new List<EndpointXlsxHttpRow>
                {
                    new EndpointXlsxHttpRow
                    {
                        EndpointName = "HTTP Endpoint",
                        Protocol = "HTTPS",
                        TargetPort = "443",
                        ResponseUrl = "https://status.example.test/health",
                        IpAddresses = "203.0.113.10",
                        MacAddresses = "00-11-22-33-44-55",
                        DnsNames = "status.example.test",
                        ResponseTime = "15 ms",
                        StatusCode = "200",
                        StatusMessage = "OK",
                        LastSeenOnline = "2026-08-04 10:00:30",
                        PingRoundtripTime = "11 ms",
                        UserName = "api-user",
                        NetworkShares = "N/A",
                        ServerId = "nginx",
                        HttpAutoRedirects = "0",
                        HttpContentType = "application/json",
                        HttpContentLength = "123",
                        HttpExpires = "N/A",
                        HttpETag = "abc123",
                        HttpEncoding = "utf-8",
                        HtmlEncoding = "utf-8",
                        HtmlPageTitle = "Status",
                        HtmlPageAuthor = "N/A",
                        HtmlPageDescription = "Service health",
                        HtmlContentLanguage = "English",
                        HtmlThemeColor = "Blue",
                        HtmlPageLinksCount = "2",
                        RowBackgroundColor = Color.LightGreen,
                    },
                },
                FtpRows = new List<EndpointXlsxFtpRow>
                {
                    new EndpointXlsxFtpRow
                    {
                        EndpointName = "FTP Endpoint",
                        Protocol = "FTP",
                        TargetPort = "21",
                        ResponseUrl = "ftp://ftp.example.test/files",
                        IpAddresses = "203.0.113.20",
                        MacAddresses = "66-77-88-99-AA-BB",
                        DnsNames = "ftp.example.test",
                        ResponseTime = "34 ms",
                        StatusCode = "230",
                        StatusMessage = "Logged in",
                        LastSeenOnline = "2026-08-04 10:00:30",
                        PingRoundtripTime = "18 ms",
                        UserName = "ftp-user",
                        NetworkShares = "N/A",
                        RowBackgroundColor = Color.LightGreen,
                    },
                },
            };
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

        private static void AssertContains(string expected, string actual)
        {
            if (actual == null || !actual.Contains(expected))
            {
                throw new InvalidOperationException("Expected text to contain <" + expected + "> but was <" + actual + ">.");
            }
        }

        private static void AssertEqual<T>(T expected, T actual)
        {
            if (!object.Equals(expected, actual))
            {
                throw new InvalidOperationException("Expected <" + expected + "> but was <" + actual + ">.");
            }
        }

        private static void AssertTrue(bool value)
        {
            if (!value)
            {
                throw new InvalidOperationException("Expected condition to be true.");
            }
        }

        private static void AssertFalse(bool value)
        {
            if (value)
            {
                throw new InvalidOperationException("Expected condition to be false.");
            }
        }

        private static void AssertNotNull(object value)
        {
            if (value == null)
            {
                throw new InvalidOperationException("Expected value not to be null.");
            }
        }

        private static void AssertColor(Color expected, Color actual)
        {
            if (expected.ToArgb() != actual.ToArgb())
            {
                throw new InvalidOperationException("Expected color <" + expected + "> but was <" + actual + ">.");
            }
        }
    }

    public class EndpointDefinition
    {
        public string Name { get; set; }

        public string Address { get; set; }
    }
}
