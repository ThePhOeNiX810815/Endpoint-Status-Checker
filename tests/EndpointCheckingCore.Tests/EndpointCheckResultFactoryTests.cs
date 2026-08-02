using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace EndpointChecker
{
    internal static class EndpointCheckResultFactoryTests
    {
        public static void Register()
        {
            EndpointCheckingCoreTestRunner.Run("Pending check result preserves source identity fields", PendingCheckResultPreservesSourceIdentityFields);
            EndpointCheckingCoreTestRunner.Run("Pending check result initializes current legacy defaults", PendingCheckResultInitializesCurrentLegacyDefaults);
            EndpointCheckingCoreTestRunner.Run("Unhandled exception result preserves current error mapping", UnhandledExceptionResultPreservesCurrentErrorMapping);
        }

        private static void PendingCheckResultPreservesSourceIdentityFields()
        {
            EndpointDefinition source = BuildSourceEndpoint();
            EndpointDefinition result = EndpointCheckResultFactory.CreatePendingResult(source);

            EndpointCheckingCoreTestRunner.AssertEqual("Endpoint", result.Name);
            EndpointCheckingCoreTestRunner.AssertEqual("http://example.com", result.Address);
            EndpointCheckingCoreTestRunner.AssertEqual("HTTP", result.Protocol);
            EndpointCheckingCoreTestRunner.AssertEqual("80", result.Port);
            EndpointCheckingCoreTestRunner.AssertEqual("2026-08-02 10:15:00", result.LastSeenOnline);
            EndpointCheckingCoreTestRunner.AssertEqual("user", result.LoginName);
            EndpointCheckingCoreTestRunner.AssertEqual("pass", result.LoginPass);
        }

        private static void PendingCheckResultInitializesCurrentLegacyDefaults()
        {
            EndpointDefinition result = EndpointCheckResultFactory.CreatePendingResult(BuildSourceEndpoint());

            EndpointCheckingCoreTestRunner.AssertArray(new[] { "N/A" }, result.IPAddress);
            EndpointCheckingCoreTestRunner.AssertArray(new[] { "N/A" }, result.DNSName);
            EndpointCheckingCoreTestRunner.AssertEqual("N/A", result.ResponseTime);
            EndpointCheckingCoreTestRunner.AssertEqual("N/A", result.ResponseCode);
            EndpointCheckingCoreTestRunner.AssertEqual("Not Checked Yet", result.ResponseMessage);
            EndpointCheckingCoreTestRunner.AssertEqual("N/A", result.PingRoundtripTime);
            EndpointCheckingCoreTestRunner.AssertEqual("N/A", result.ServerID);
            EndpointCheckingCoreTestRunner.AssertArray(new[] { "N/A" }, result.NetworkShare);
            EndpointCheckingCoreTestRunner.AssertEqual(0, result.HTMLMetaInfo.PropertyItem.Count);
            EndpointCheckingCoreTestRunner.AssertEqual("N/A", result.HTTPautoRedirects);
            EndpointCheckingCoreTestRunner.AssertEqual("N/A", result.HTTPcontentType);
            EndpointCheckingCoreTestRunner.AssertEqual(null, result.HTTPencoding);
            EndpointCheckingCoreTestRunner.AssertEqual(null, result.HTMLencoding);
            EndpointCheckingCoreTestRunner.AssertEqual("N/A", result.HTMLTitle);
            EndpointCheckingCoreTestRunner.AssertEqual("N/A", result.HTMLAuthor);
            EndpointCheckingCoreTestRunner.AssertEqual("N/A", result.HTMLDescription);
            EndpointCheckingCoreTestRunner.AssertEqual("N/A", result.HTMLContentLanguage);
            EndpointCheckingCoreTestRunner.AssertEqual(Color.Empty, result.HTMLThemeColor);
            EndpointCheckingCoreTestRunner.AssertEqual(0, result.HTMLPageLinks.PropertyItem.Count);
            EndpointCheckingCoreTestRunner.AssertEqual("N/A", result.HTTPcontentLength);
            EndpointCheckingCoreTestRunner.AssertEqual("N/A", result.HTTPexpires);
            EndpointCheckingCoreTestRunner.AssertEqual("N/A", result.HTTPetag);
            EndpointCheckingCoreTestRunner.AssertEqual(0, result.HTTPRequestHeaders.PropertyItem.Count);
            EndpointCheckingCoreTestRunner.AssertEqual(0, result.HTTPResponseHeaders.PropertyItem.Count);
            EndpointCheckingCoreTestRunner.AssertArray(new[] { "N/A" }, result.MACAddress);
            EndpointCheckingCoreTestRunner.AssertEqual("N/A", result.FTPBannerMessage);
            EndpointCheckingCoreTestRunner.AssertEqual("N/A", result.FTPWelcomeMessage);
            EndpointCheckingCoreTestRunner.AssertEqual("N/A", result.FTPExitMessage);
            EndpointCheckingCoreTestRunner.AssertEqual("N/A", result.FTPStatusDescription);
            EndpointCheckingCoreTestRunner.AssertEqual(0, result.SSLCertificateProperties.PropertyItem.Count);
        }

        private static void UnhandledExceptionResultPreservesCurrentErrorMapping()
        {
            EndpointDefinition source = BuildSourceEndpoint();
            EndpointDefinition result = EndpointCheckResultFactory.CreateUnhandledExceptionResult(source, new InvalidOperationException("boom"));

            EndpointCheckingCoreTestRunner.AssertEqual("ERROR", result.ResponseCode);
            EndpointCheckingCoreTestRunner.AssertEqual("InvalidOperationException -> boom", result.ResponseMessage);
            EndpointCheckingCoreTestRunner.AssertEqual("http://example.com/response", result.ResponseAddress);
            EndpointCheckingCoreTestRunner.AssertEqual("N/A", result.ResponseTime);
            EndpointCheckingCoreTestRunner.AssertEqual("2026-08-02 10:15:00", result.LastSeenOnline);
        }

        private static EndpointDefinition BuildSourceEndpoint()
        {
            return new EndpointDefinition
            {
                Name = "Endpoint",
                Address = "http://example.com",
                ResponseAddress = "http://example.com/response",
                Protocol = "HTTP",
                Port = "80",
                LoginName = "user",
                LoginPass = "pass",
                LastSeenOnline = "2026-08-02 10:15:00",
                IPAddress = new[] { "192.0.2.10" },
                DNSName = new[] { "example.com" },
                NetworkShare = new[] { "Share" },
                MACAddress = new[] { "00-00-00-00-00-00" }
            };
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
