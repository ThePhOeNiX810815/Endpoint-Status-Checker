using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace EndpointChecker
{
    internal static class EndpointDefinitionParserTests
    {
        private static int failed;

        private static void Main()
        {
            Run("Blank lines are ignored", BlankLinesAreIgnored);
            Run("Comment lines are ignored", CommentLinesAreIgnored);
            Run("Lone pipe is ignored", LonePipeIsIgnored);
            Run("Name URL splits on first pipe only", NameUrlSplitsOnFirstPipeOnly);
            Run("URL without protocol is rejected as missing protocol", UrlWithoutProtocolIsRejectedAsMissingProtocol);
            Run("Unsupported schemes are rejected", UnsupportedSchemesAreRejected);
            Run("Duplicate endpoint names use ordinal current comparison", DuplicateEndpointNamesUseOrdinalComparison);
            Run("Duplicate invalid lines report both legacy categories", DuplicateInvalidLinesReportBothLegacyCategories);
            Run("Credentials are extracted from URL", CredentialsAreExtractedFromUrl);
            Run("Credentials are removed from endpoint address", CredentialsAreRemovedFromEndpointAddress);
            Run("Credential endpoint name behavior is preserved", CredentialEndpointNameBehaviorIsPreserved);
            Run("URI escaping normalization is preserved", UriEscapingNormalizationIsPreserved);
            Run("Default endpoint values are preserved", DefaultEndpointValuesArePreserved);
            Run("Last seen restoration remains orchestration responsibility", LastSeenRestorationRemainsOrchestrationResponsibility);
            Run("Maximum endpoint count remains line number based orchestration", MaximumEndpointCountRemainsLineNumberBasedOrchestration);
            Run("Duplicate names differing by case whitespace normalization are preserved", DuplicateNamesDifferingByCaseWhitespaceNormalizationArePreserved);

            if (failed > 0)
            {
                Environment.Exit(1);
            }
        }

        private static void BlankLinesAreIgnored()
        {
            AssertEqual(EndpointDefinitionParseStatus.Ignored, new EndpointDefinitionParser().ParseLine("   ", 1).Status);
        }

        private static void CommentLinesAreIgnored()
        {
            AssertEqual(EndpointDefinitionParseStatus.Ignored, new EndpointDefinitionParser().ParseLine("  # comment", 1).Status);
        }

        private static void LonePipeIsIgnored()
        {
            AssertEqual(EndpointDefinitionParseStatus.Ignored, new EndpointDefinitionParser().ParseLine(" | ", 1).Status);
        }

        private static void NameUrlSplitsOnFirstPipeOnly()
        {
            EndpointDefinitionParseResult result = new EndpointDefinitionParser().ParseLine("Pipe Test|http://example.com/a|b", 7);

            AssertEqual(EndpointDefinitionParseStatus.Valid, result.Status);
            AssertEqual("Pipe Test", result.EndpointDefinition.Name);
            AssertEqual("http://example.com/a%7Cb", result.EndpointDefinition.Address);
            AssertEqual("http://example.com/a%7Cb", result.EndpointDefinition.ResponseAddress);
        }

        private static void UrlWithoutProtocolIsRejectedAsMissingProtocol()
        {
            EndpointDefinitionParseResult result = new EndpointDefinitionParser().ParseLine("Missing|example.com", 3);

            AssertEqual(EndpointDefinitionParseStatus.InvalidUrl, result.Status);
            AssertEqual(EndpointDefinitionParseErrorKind.MissingProtocol, result.Error.Kind);
            AssertContains("Missing protocol prefix", result.Error.ToInvalidUrlDisplayText());
        }

        private static void UnsupportedSchemesAreRejected()
        {
            EndpointDefinitionParseResult result = new EndpointDefinitionParser().ParseLine("SSH|ssh://example.com", 4);

            AssertEqual(EndpointDefinitionParseStatus.InvalidUrl, result.Status);
            AssertEqual(EndpointDefinitionParseErrorKind.UnsupportedProtocol, result.Error.Kind);
            AssertContains("Unsupported protocol type: SSH", result.Error.ToInvalidUrlDisplayText());
        }

        private static void DuplicateEndpointNamesUseOrdinalComparison()
        {
            EndpointDefinitionParser parser = new EndpointDefinitionParser();

            AssertEqual(EndpointDefinitionParseStatus.Valid, parser.ParseLine("Same|http://example.com", 1).Status);
            EndpointDefinitionParseResult duplicate = parser.ParseLine("Same|http://example.org", 2);

            AssertEqual(EndpointDefinitionParseStatus.Duplicate, duplicate.Status);
            AssertEqual(EndpointDefinitionParseErrorKind.DuplicateName, duplicate.Error.Kind);
        }

        private static void DuplicateInvalidLinesReportBothLegacyCategories()
        {
            EndpointDefinitionParser parser = new EndpointDefinitionParser();

            AssertEqual(EndpointDefinitionParseStatus.Valid, parser.ParseLine("Same|http://example.com", 1).Status);
            EndpointDefinitionParseResult duplicate = parser.ParseLine("Same|example.org", 2);

            AssertEqual(EndpointDefinitionParseStatus.Duplicate, duplicate.Status);
            AssertEqual(EndpointDefinitionParseErrorKind.DuplicateName, duplicate.DuplicateError.Kind);
            AssertEqual(EndpointDefinitionParseErrorKind.MissingProtocol, duplicate.InvalidUrlError.Kind);
        }

        private static void CredentialsAreExtractedFromUrl()
        {
            EndpointDefinition endpoint = new EndpointDefinitionParser()
                .ParseLine("Secure|http://user:pass@example.com/private", 1)
                .EndpointDefinition;

            AssertEqual("user", endpoint.LoginName);
            AssertEqual("pass", endpoint.LoginPass);
        }

        private static void CredentialsAreRemovedFromEndpointAddress()
        {
            EndpointDefinition endpoint = new EndpointDefinitionParser()
                .ParseLine("Secure|http://user:pass@example.com/private", 1)
                .EndpointDefinition;

            AssertEqual("http://example.com/private", endpoint.Address);
        }

        private static void CredentialEndpointNameBehaviorIsPreserved()
        {
            EndpointDefinition endpoint = new EndpointDefinitionParser()
                .ParseLine("http://user:pass@example.com|http://user:pass@example.com", 1)
                .EndpointDefinition;

            AssertEqual("http://example.com [as 'user']", endpoint.Name);
        }

        private static void UriEscapingNormalizationIsPreserved()
        {
            EndpointDefinition endpoint = new EndpointDefinitionParser()
                .ParseLine("Escaped|http://example.com/a path?q=hello world", 1)
                .EndpointDefinition;

            AssertEqual("http://example.com/a%20path?q=hello%20world", endpoint.Address);
            AssertEqual("http://example.com/a%20path?q=hello%20world", endpoint.ResponseAddress);
        }

        private static void DefaultEndpointValuesArePreserved()
        {
            EndpointDefinition endpoint = EndpointDefinitionParser.CreateDefaultEndpointDefinition("Defaults|http://example.com");

            AssertEqual("Defaults", endpoint.Name);
            AssertEqual("http://example.com", endpoint.Address);
            AssertEqual("http://example.com", endpoint.ResponseAddress);
            AssertEqual("N/A", endpoint.Protocol);
            AssertEqual("N/A", endpoint.Port);
            AssertArray(new[] { "N/A" }, endpoint.IPAddress);
            AssertEqual("N/A", endpoint.ResponseTime);
            AssertEqual("N/A", endpoint.ResponseCode);
            AssertEqual("Not Checked Yet", endpoint.ResponseMessage);
            AssertEqual("N/A", endpoint.LastSeenOnline);
            AssertEqual("N/A", endpoint.PingRoundtripTime);
            AssertEqual("N/A", endpoint.ServerID);
            AssertEqual("N/A", endpoint.LoginName);
            AssertEqual("N/A", endpoint.LoginPass);
            AssertArray(new[] { "N/A" }, endpoint.NetworkShare);
            AssertArray(new[] { "N/A" }, endpoint.DNSName);
            AssertEqual(0, endpoint.HTMLMetaInfo.PropertyItem.Count);
            AssertEqual("N/A", endpoint.HTTPautoRedirects);
            AssertEqual("N/A", endpoint.HTTPcontentType);
            AssertEqual(null, endpoint.HTTPencoding);
            AssertEqual(null, endpoint.HTMLdefaultStreamEncoding);
            AssertEqual(null, endpoint.HTMLencoding);
            AssertEqual("N/A", endpoint.HTMLTitle);
            AssertEqual("N/A", endpoint.HTMLAuthor);
            AssertEqual(0, endpoint.HTMLPageLinks.PropertyItem.Count);
            AssertEqual("N/A", endpoint.HTMLDescription);
            AssertEqual("N/A", endpoint.HTMLContentLanguage);
            AssertEqual(Color.Empty, endpoint.HTMLThemeColor);
            AssertEqual("N/A", endpoint.HTTPcontentLength);
            AssertEqual("N/A", endpoint.HTTPexpires);
            AssertEqual("N/A", endpoint.HTTPetag);
            AssertEqual(0, endpoint.HTTPRequestHeaders.PropertyItem.Count);
            AssertEqual(0, endpoint.HTTPResponseHeaders.PropertyItem.Count);
            AssertArray(new[] { "N/A" }, endpoint.MACAddress);
            AssertEqual("N/A", endpoint.FTPBannerMessage);
            AssertEqual("N/A", endpoint.FTPWelcomeMessage);
            AssertEqual("N/A", endpoint.FTPExitMessage);
            AssertEqual("N/A", endpoint.FTPStatusDescription);
            AssertEqual(0, endpoint.SSLCertificateProperties.PropertyItem.Count);
        }

        private static void LastSeenRestorationRemainsOrchestrationResponsibility()
        {
            Dictionary<string, string> lastSeen = new Dictionary<string, string>
            {
                { "Endpoint", "2026-08-02 10:15:00" }
            };

            EndpointDefinition endpoint = new EndpointDefinitionParser()
                .ParseLine("Endpoint|http://example.com", 1)
                .EndpointDefinition;

            if (lastSeen.ContainsKey(endpoint.Name))
            {
                endpoint.LastSeenOnline = lastSeen[endpoint.Name];
            }

            AssertEqual("2026-08-02 10:15:00", endpoint.LastSeenOnline);
        }

        private static void MaximumEndpointCountRemainsLineNumberBasedOrchestration()
        {
            int maximum = 2;
            int loaded = 0;

            string[] lines =
            {
                "# comment",
                "First|http://example.com",
                "Second|http://example.org"
            };

            EndpointDefinitionParser parser = new EndpointDefinitionParser();
            for (int i = 0; i < lines.Length; i++)
            {
                int lineNumber = i + 1;
                if (lineNumber > maximum)
                {
                    break;
                }

                if (parser.ParseLine(lines[i], lineNumber).IsValid)
                {
                    loaded++;
                }
            }

            AssertEqual(1, loaded);
        }

        private static void DuplicateNamesDifferingByCaseWhitespaceNormalizationArePreserved()
        {
            EndpointDefinitionParser parser = new EndpointDefinitionParser();

            AssertEqual(EndpointDefinitionParseStatus.Valid, parser.ParseLine("Name|http://example.com", 1).Status);
            AssertEqual(EndpointDefinitionParseStatus.Valid, parser.ParseLine("name|http://example.org", 2).Status);
            AssertEqual(EndpointDefinitionParseStatus.Duplicate, parser.ParseLine(" Name |http://example.net", 3).Status);
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

        private static void AssertArray(string[] expected, string[] actual)
        {
            if (!expected.SequenceEqual(actual))
            {
                throw new InvalidOperationException("Expected <" + string.Join(",", expected) + "> but was <" + string.Join(",", actual) + ">.");
            }
        }

        private static void AssertContains(string expected, string actual)
        {
            if (actual == null || !actual.Contains(expected))
            {
                throw new InvalidOperationException("Expected text containing <" + expected + "> but was <" + actual + ">.");
            }
        }
    }

    public class EndpointDefinition
    {
        public string Name { get; set; }
        public string Protocol { get; set; }
        public string Port { get; set; }
        public string Address { get; set; }
        public string ResponseAddress { get; set; }
        public string[] IPAddress { get; set; }
        public string ResponseTime { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string PingRoundtripTime { get; set; }
        public string ServerID { get; set; }
        public string LoginName { get; set; }
        public string LoginPass { get; set; }
        public string[] NetworkShare { get; set; }
        public string[] DNSName { get; set; }
        public PropertyItems HTMLMetaInfo { get; set; }
        public string HTTPautoRedirects { get; set; }
        public string HTTPcontentType { get; set; }
        public string HTTPcontentLength { get; set; }
        public string HTTPexpires { get; set; }
        public string HTTPetag { get; set; }
        public Encoding HTTPencoding { get; set; }
        public Encoding HTMLencoding { get; set; }
        public Encoding HTMLdefaultStreamEncoding { get; set; }
        public string HTMLTitle { get; set; }
        public string HTMLAuthor { get; set; }
        public string HTMLDescription { get; set; }
        public string HTMLContentLanguage { get; set; }
        public Color HTMLThemeColor { get; set; }
        public PropertyItems HTTPRequestHeaders { get; set; }
        public PropertyItems HTTPResponseHeaders { get; set; }
        public string[] MACAddress { get; set; }
        public string FTPBannerMessage { get; set; }
        public string FTPWelcomeMessage { get; set; }
        public string FTPExitMessage { get; set; }
        public string FTPStatusDescription { get; set; }
        public string LastSeenOnline { get; set; }
        public PropertyItems SSLCertificateProperties { get; set; }
        public PropertyItems HTMLPageLinks { get; set; }
    }

    public class Property
    {
        public string ItemName { get; set; }
        public string ItemValue { get; set; }
    }

    public class PropertyItems
    {
        public List<Property> PropertyItem { get; set; }
    }
}
