using System;
using System.Collections.Generic;
using System.Drawing;

namespace EndpointChecker
{
    /// <summary>
    /// Builds initial and fallback endpoint check results using the legacy status strings and defaults.
    /// </summary>
    internal static class EndpointCheckResultFactory
    {
        private const string StatusNotAvailable = "N/A";
        private const string StatusError = "ERROR";
        private const string ResponseMessageNotCheckedYet = "Not Checked Yet";

        /// <summary>
        /// Creates the pending result shown while an enabled endpoint is being checked.
        /// </summary>
        public static EndpointDefinition CreatePendingResult(EndpointDefinition source)
        {
            return new EndpointDefinition()
            {
                Name = source.Name,
                Address = source.Address,
                Protocol = source.Protocol,
                Port = source.Port,
                IPAddress = new string[] { StatusNotAvailable },
                DNSName = new string[] { StatusNotAvailable },
                ResponseTime = StatusNotAvailable,
                ResponseCode = StatusNotAvailable,
                ResponseMessage = ResponseMessageNotCheckedYet,
                LastSeenOnline = source.LastSeenOnline,
                PingRoundtripTime = StatusNotAvailable,
                ServerID = StatusNotAvailable,
                LoginName = source.LoginName,
                LoginPass = source.LoginPass,
                NetworkShare = new string[] { StatusNotAvailable },
                HTMLMetaInfo = new PropertyItems() { PropertyItem = new List<Property>() },
                HTTPautoRedirects = StatusNotAvailable,
                HTTPcontentType = StatusNotAvailable,
                HTTPencoding = null,
                HTMLencoding = null,
                HTMLTitle = StatusNotAvailable,
                HTMLAuthor = StatusNotAvailable,
                HTMLDescription = StatusNotAvailable,
                HTMLContentLanguage = StatusNotAvailable,
                HTMLThemeColor = Color.Empty,
                HTMLPageLinks = new PropertyItems() { PropertyItem = new List<Property>() },
                HTTPcontentLength = StatusNotAvailable,
                HTTPexpires = StatusNotAvailable,
                HTTPetag = StatusNotAvailable,
                HTTPRequestHeaders = new PropertyItems() { PropertyItem = new List<Property>() },
                HTTPResponseHeaders = new PropertyItems() { PropertyItem = new List<Property>() },
                MACAddress = new string[] { StatusNotAvailable },
                FTPBannerMessage = StatusNotAvailable,
                FTPWelcomeMessage = StatusNotAvailable,
                FTPExitMessage = StatusNotAvailable,
                FTPStatusDescription = StatusNotAvailable,
                SSLCertificateProperties = new PropertyItems() { PropertyItem = new List<Property>() }
            };
        }

        /// <summary>
        /// Creates the top-level safety-net result used when endpoint checking throws unexpectedly.
        /// </summary>
        public static EndpointDefinition CreateUnhandledExceptionResult(EndpointDefinition source, Exception exception)
        {
            return new EndpointDefinition
            {
                Name = source.Name,
                Address = source.Address,
                ResponseAddress = source.ResponseAddress ?? source.Address,
                Protocol = source.Protocol ?? StatusNotAvailable,
                Port = source.Port ?? StatusNotAvailable,
                ResponseCode = StatusError,
                ResponseMessage = exception.GetType().Name + " -> " + exception.Message,
                ResponseTime = StatusNotAvailable,
                LastSeenOnline = source.LastSeenOnline ?? StatusNotAvailable,
                PingRoundtripTime = StatusNotAvailable,
                ServerID = StatusNotAvailable,
                LoginName = source.LoginName ?? StatusNotAvailable,
                LoginPass = source.LoginPass ?? StatusNotAvailable,
                IPAddress = source.IPAddress ?? new string[] { StatusNotAvailable },
                DNSName = source.DNSName ?? new string[] { StatusNotAvailable },
                NetworkShare = source.NetworkShare ?? new string[] { StatusNotAvailable },
                MACAddress = source.MACAddress ?? new string[] { StatusNotAvailable },
                HTMLMetaInfo = new PropertyItems { PropertyItem = new List<Property>() },
                HTMLPageLinks = new PropertyItems { PropertyItem = new List<Property>() },
                HTTPRequestHeaders = new PropertyItems { PropertyItem = new List<Property>() },
                HTTPResponseHeaders = new PropertyItems { PropertyItem = new List<Property>() },
                SSLCertificateProperties = new PropertyItems { PropertyItem = new List<Property>() },
                HTTPautoRedirects = StatusNotAvailable,
                HTTPcontentType = StatusNotAvailable,
                HTTPcontentLength = StatusNotAvailable,
                HTTPexpires = StatusNotAvailable,
                HTTPetag = StatusNotAvailable,
                FTPBannerMessage = StatusNotAvailable,
                FTPWelcomeMessage = StatusNotAvailable,
                FTPExitMessage = StatusNotAvailable,
                FTPStatusDescription = StatusNotAvailable
            };
        }
    }
}
