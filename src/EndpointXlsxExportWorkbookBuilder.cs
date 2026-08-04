using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace EndpointChecker
{
    internal sealed class EndpointXlsxExportWorkbookInput
    {
        public string Author { get; set; }

        public string StatusNotAvailable { get; set; }

        public EndpointXlsxExportSummaryData Summary { get; set; }

        public IReadOnlyList<EndpointXlsxHttpRow> HttpRows { get; set; }

        public IReadOnlyList<EndpointXlsxFtpRow> FtpRows { get; set; }
    }

    internal sealed class EndpointXlsxExportSummaryData
    {
        public string AppVersion { get; set; }
        public string OperatingSystem { get; set; }
        public string TargetFramework { get; set; }
        public string SystemMemory { get; set; }
        public string UserName { get; set; }
        public string Domain { get; set; }
        public string ComputerName { get; set; }
        public string StartDateTime { get; set; }
        public string EndDateTime { get; set; }
        public string Duration { get; set; }
        public string ThreadsCount { get; set; }
        public string PingTimeout { get; set; }
        public string HttpRequestTimeout { get; set; }
        public string FtpRequestTimeout { get; set; }
        public string SslCertificateValidation { get; set; }
        public string HttpAutoRedirection { get; set; }
        public string ResolveNetworkShares { get; set; }
        public string ResolvePageMetaInfo { get; set; }
        public string SaveResponse { get; set; }
        public string PingHost { get; set; }
        public string DnsLookupOnHost { get; set; }
    }

    internal sealed class EndpointXlsxHttpRow
    {
        public string EndpointName { get; set; }
        public string Protocol { get; set; }
        public string TargetPort { get; set; }
        public string ResponseUrl { get; set; }
        public string IpAddresses { get; set; }
        public string MacAddresses { get; set; }
        public string DnsNames { get; set; }
        public string ResponseTime { get; set; }
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public string LastSeenOnline { get; set; }
        public string PingRoundtripTime { get; set; }
        public string UserName { get; set; }
        public string NetworkShares { get; set; }
        public string ServerId { get; set; }
        public string HttpAutoRedirects { get; set; }
        public string HttpContentType { get; set; }
        public string HttpContentLength { get; set; }
        public string HttpExpires { get; set; }
        public string HttpETag { get; set; }
        public string HttpEncoding { get; set; }
        public string HtmlEncoding { get; set; }
        public string HtmlPageTitle { get; set; }
        public string HtmlPageAuthor { get; set; }
        public string HtmlPageDescription { get; set; }
        public string HtmlContentLanguage { get; set; }
        public string HtmlThemeColor { get; set; }
        public string HtmlPageLinksCount { get; set; }
        public Color RowBackgroundColor { get; set; }
    }

    internal sealed class EndpointXlsxFtpRow
    {
        public string EndpointName { get; set; }
        public string Protocol { get; set; }
        public string TargetPort { get; set; }
        public string ResponseUrl { get; set; }
        public string IpAddresses { get; set; }
        public string MacAddresses { get; set; }
        public string DnsNames { get; set; }
        public string ResponseTime { get; set; }
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public string LastSeenOnline { get; set; }
        public string PingRoundtripTime { get; set; }
        public string UserName { get; set; }
        public string NetworkShares { get; set; }
        public Color RowBackgroundColor { get; set; }
    }

    internal static class EndpointXlsxExportWorkbookBuilder
    {
        private static readonly string[] HttpHeaders =
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

        private static readonly string[] FtpHeaders =
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

        public static XLWorkbook Build(EndpointXlsxExportWorkbookInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (input.Summary == null)
            {
                throw new ArgumentNullException(nameof(input.Summary));
            }

            IReadOnlyList<EndpointXlsxHttpRow> httpRows = input.HttpRows ?? Array.Empty<EndpointXlsxHttpRow>();
            IReadOnlyList<EndpointXlsxFtpRow> ftpRows = input.FtpRows ?? Array.Empty<EndpointXlsxFtpRow>();

            XLWorkbook workbook = new XLWorkbook();
            IXLWorksheet summaryWorksheet = workbook.Worksheets.Add("Summary");
            IXLWorksheet httpWorksheet = workbook.Worksheets.Add("HTTP Endpoints");
            IXLWorksheet ftpWorksheet = workbook.Worksheets.Add("FTP Endpoints");

            foreach (IXLWorksheet worksheet in workbook.Worksheets)
            {
                worksheet.Author = input.Author;
            }

            WriteHeaders(httpWorksheet, HttpHeaders);
            WriteHeaders(ftpWorksheet, FtpHeaders);

            int httpRowNumber = 2;
            foreach (EndpointXlsxHttpRow row in httpRows)
            {
                httpWorksheet.Cell("A" + httpRowNumber).SetValue(row.EndpointName);
                httpWorksheet.Cell("B" + httpRowNumber).SetValue(row.Protocol);
                httpWorksheet.Cell("C" + httpRowNumber).SetValue(row.TargetPort);
                httpWorksheet.Cell("D" + httpRowNumber).SetValue(row.ResponseUrl);
                httpWorksheet.Cell("D" + httpRowNumber).SetHyperlink(new XLHyperlink(row.ResponseUrl));
                httpWorksheet.Cell("E" + httpRowNumber).SetValue(row.IpAddresses);
                httpWorksheet.Cell("F" + httpRowNumber).SetValue(row.MacAddresses);
                httpWorksheet.Cell("G" + httpRowNumber).SetValue(row.DnsNames);
                httpWorksheet.Cell("H" + httpRowNumber).SetValue(row.ResponseTime);
                httpWorksheet.Cell("I" + httpRowNumber).SetValue(row.StatusCode);
                httpWorksheet.Cell("J" + httpRowNumber).SetValue(row.StatusMessage);
                httpWorksheet.Cell("K" + httpRowNumber).SetValue(row.LastSeenOnline);
                httpWorksheet.Cell("L" + httpRowNumber).SetValue(row.PingRoundtripTime);
                httpWorksheet.Cell("M" + httpRowNumber).SetValue(row.UserName);
                httpWorksheet.Cell("N" + httpRowNumber).SetValue(row.NetworkShares);
                httpWorksheet.Cell("O" + httpRowNumber).SetValue(row.ServerId);
                httpWorksheet.Cell("P" + httpRowNumber).SetValue(row.HttpAutoRedirects);
                httpWorksheet.Cell("Q" + httpRowNumber).SetValue(row.HttpContentType);
                httpWorksheet.Cell("R" + httpRowNumber).SetValue(row.HttpContentLength);
                httpWorksheet.Cell("S" + httpRowNumber).SetValue(row.HttpExpires);
                httpWorksheet.Cell("T" + httpRowNumber).SetValue(row.HttpETag);
                httpWorksheet.Cell("U" + httpRowNumber).SetValue(row.HttpEncoding);
                httpWorksheet.Cell("V" + httpRowNumber).SetValue(row.HtmlEncoding);
                httpWorksheet.Cell("W" + httpRowNumber).SetValue(row.HtmlPageTitle);
                httpWorksheet.Cell("X" + httpRowNumber).SetValue(row.HtmlPageAuthor);
                httpWorksheet.Cell("Y" + httpRowNumber).SetValue(row.HtmlPageDescription);
                httpWorksheet.Cell("Z" + httpRowNumber).SetValue(row.HtmlContentLanguage);
                httpWorksheet.Cell("AA" + httpRowNumber).SetValue(row.HtmlThemeColor);
                httpWorksheet.Cell("AB" + httpRowNumber).SetValue(row.HtmlPageLinksCount);

                httpWorksheet.Row(httpRowNumber)
                    .CellsUsed()
                    .Style.Fill.BackgroundColor = XLColor.FromColor(row.RowBackgroundColor);

                httpRowNumber++;
            }

            int ftpRowNumber = 2;
            foreach (EndpointXlsxFtpRow row in ftpRows)
            {
                ftpWorksheet.Cell("A" + ftpRowNumber).SetValue(row.EndpointName);
                ftpWorksheet.Cell("B" + ftpRowNumber).SetValue(row.Protocol);
                ftpWorksheet.Cell("C" + ftpRowNumber).SetValue(row.TargetPort);
                ftpWorksheet.Cell("D" + ftpRowNumber).SetValue(row.ResponseUrl);
                ftpWorksheet.Cell("D" + ftpRowNumber).SetHyperlink(new XLHyperlink(row.ResponseUrl));
                ftpWorksheet.Cell("E" + ftpRowNumber).SetValue(row.IpAddresses);
                ftpWorksheet.Cell("F" + ftpRowNumber).SetValue(row.MacAddresses);
                ftpWorksheet.Cell("G" + ftpRowNumber).SetValue(row.DnsNames);
                ftpWorksheet.Cell("H" + ftpRowNumber).SetValue(row.ResponseTime);
                ftpWorksheet.Cell("I" + ftpRowNumber).SetValue(row.StatusCode);
                ftpWorksheet.Cell("J" + ftpRowNumber).SetValue(row.StatusMessage);
                ftpWorksheet.Cell("K" + ftpRowNumber).SetValue(row.LastSeenOnline);
                ftpWorksheet.Cell("L" + ftpRowNumber).SetValue(row.PingRoundtripTime);
                ftpWorksheet.Cell("M" + ftpRowNumber).SetValue(row.UserName);
                ftpWorksheet.Cell("N" + ftpRowNumber).SetValue(row.NetworkShares);

                ftpWorksheet.Row(ftpRowNumber)
                    .CellsUsed()
                    .Style.Fill.BackgroundColor = XLColor.FromColor(row.RowBackgroundColor);

                ftpRowNumber++;
            }

            WriteSummary(summaryWorksheet, input.Summary, httpRows.Count, ftpRows.Count);

            ApplyEndpointSheetSettings(httpWorksheet);
            ApplyEndpointSheetSettings(ftpWorksheet);
            ApplySummarySheetSettings(summaryWorksheet);

            HideNotAvailableOnlyColumns(httpWorksheet, input.StatusNotAvailable);
            HideNotAvailableOnlyColumns(ftpWorksheet, input.StatusNotAvailable);

            // Keep the historical worksheet deletion sequence exactly as before.
            if (httpWorksheet.RowsUsed().Count() < 2)
            {
                httpWorksheet.Delete();
            }
            else if (ftpWorksheet.RowsUsed().Count() < 2)
            {
                ftpWorksheet.Delete();
            }

            return workbook;
        }

        private static void WriteHeaders(IXLWorksheet worksheet, IReadOnlyList<string> headers)
        {
            for (int index = 0; index < headers.Count; index++)
            {
                worksheet.Cell(1, index + 1).SetValue(headers[index]);
            }
        }

        private static void WriteSummary(IXLWorksheet summaryWorksheet, EndpointXlsxExportSummaryData summary, int httpCount, int ftpCount)
        {
            summaryWorksheet.Cell("A1").SetValue("Endpoint Checker Application");
            summaryWorksheet.Cell("B1").SetValue(summary.AppVersion);
            summaryWorksheet.Cell("A2").SetValue("Operating System");
            summaryWorksheet.Cell("B2").SetValue(summary.OperatingSystem);
            summaryWorksheet.Cell("A3").SetValue("Target Framework Version");
            summaryWorksheet.Cell("B3").SetValue(summary.TargetFramework);
            summaryWorksheet.Cell("A4").SetValue("System Memory (RAM)");
            summaryWorksheet.Cell("B4").SetValue(summary.SystemMemory);
            summaryWorksheet.Cell("A5").SetValue("User Name");
            summaryWorksheet.Cell("B5").SetValue(summary.UserName);
            summaryWorksheet.Cell("A6").SetValue("Domain");
            summaryWorksheet.Cell("B6").SetValue(summary.Domain);
            summaryWorksheet.Cell("A7").SetValue("Computer Name");
            summaryWorksheet.Cell("B7").SetValue(summary.ComputerName);

            summaryWorksheet.Cell("D1").SetValue("Check Started");
            summaryWorksheet.Cell("E1").SetValue(summary.StartDateTime);
            summaryWorksheet.Cell("D2").SetValue("Check Ended");
            summaryWorksheet.Cell("E2").SetValue(summary.EndDateTime);
            summaryWorksheet.Cell("D3").SetValue("Check Duration");
            summaryWorksheet.Cell("E3").SetValue(summary.Duration);
            summaryWorksheet.Cell("D4").SetValue("HTTP Endpoints Count");
            summaryWorksheet.Cell("E4").SetValue(httpCount.ToString());
            summaryWorksheet.Cell("D5").SetValue("FTP Endpoints Count");
            summaryWorksheet.Cell("E5").SetValue(ftpCount.ToString());
            summaryWorksheet.Cell("D6").SetValue("Parallel Threads Count");
            summaryWorksheet.Cell("E6").SetValue(summary.ThreadsCount);
            summaryWorksheet.Cell("D7").SetValue("Ping Timeout");
            summaryWorksheet.Cell("E7").SetValue(summary.PingTimeout);
            summaryWorksheet.Cell("D8").SetValue("HTTP Request Timeout");
            summaryWorksheet.Cell("E8").SetValue(summary.HttpRequestTimeout);
            summaryWorksheet.Cell("D9").SetValue("FTP Request Timeout");
            summaryWorksheet.Cell("E9").SetValue(summary.FtpRequestTimeout);
            summaryWorksheet.Cell("D10").SetValue("Server Certificate Validation [HTTPS]");
            summaryWorksheet.Cell("E10").SetValue(summary.SslCertificateValidation);
            summaryWorksheet.Cell("D11").SetValue("Auto Redirection [HTTP]");
            summaryWorksheet.Cell("E11").SetValue(summary.HttpAutoRedirection);
            summaryWorksheet.Cell("D12").SetValue("Resolve Network Shares");
            summaryWorksheet.Cell("E12").SetValue(summary.ResolveNetworkShares);
            summaryWorksheet.Cell("D13").SetValue("Resolve Page Meta Info [HTTP/HTML]");
            summaryWorksheet.Cell("E13").SetValue(summary.ResolvePageMetaInfo);
            summaryWorksheet.Cell("D14").SetValue("Save Response [HTTP]");
            summaryWorksheet.Cell("E14").SetValue(summary.SaveResponse);
            summaryWorksheet.Cell("D15").SetValue("Ping Host");
            summaryWorksheet.Cell("E15").SetValue(summary.PingHost);
            summaryWorksheet.Cell("D16").SetValue("DNS / MAC Lookup on Host");
            summaryWorksheet.Cell("E16").SetValue(summary.DnsLookupOnHost);
        }

        private static void ApplyEndpointSheetSettings(IXLWorksheet worksheet)
        {
            worksheet.Style
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Top);
            worksheet.SheetView.FreezeRows(1);
            worksheet.SheetView.FreezeColumns(1);
            worksheet.RangeUsed().SetAutoFilter();
            worksheet.Rows().AdjustToContents();
            worksheet.Columns().AdjustToContents(10, (double)70);
            worksheet.CellsUsed().Style.NumberFormat.Format = "@";
            worksheet.Row(1).CellsUsed().Style.Fill.BackgroundColor = XLColor.CoolGrey;
            worksheet.CellsUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        }

        private static void ApplySummarySheetSettings(IXLWorksheet summaryWorksheet)
        {
            summaryWorksheet.SheetView.FreezeColumns(1);
            summaryWorksheet.Style
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Top);
            summaryWorksheet.Rows().AdjustToContents();
            summaryWorksheet.Columns().AdjustToContents();
            summaryWorksheet.Column(1).CellsUsed().Style.Fill.BackgroundColor = XLColor.CoolGrey;
            summaryWorksheet.Column(2).CellsUsed().Style.Fill.BackgroundColor = XLColor.LightBlue;
            summaryWorksheet.Column(4).CellsUsed().Style.Fill.BackgroundColor = XLColor.CoolGrey;
            summaryWorksheet.Column(5).CellsUsed().Style.Fill.BackgroundColor = XLColor.LightBlue;
            summaryWorksheet.CellsUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        }

        private static void HideNotAvailableOnlyColumns(IXLWorksheet worksheet, string statusNotAvailable)
        {
            foreach (IXLColumn column in worksheet.Columns())
            {
                if (column.CellsUsed().Where(c => c.Value.ToString() != statusNotAvailable).Count() == 1)
                {
                    column.Hide();
                }
            }
        }
    }
}