namespace EndpointChecker
{
    /// <summary>
    /// Captures check-run metadata written to the export summary worksheet.
    /// </summary>
    internal sealed class EndpointExportRunSummary
    {
        private EndpointExportRunSummary(
            string startDateTime,
            string endDateTime,
            int durationSeconds,
            int pingTimeoutSeconds,
            int httpRequestTimeoutSeconds,
            int ftpRequestTimeoutSeconds,
            string httpAutoRedirection,
            string sslCertificateValidation,
            string threadsCount,
            string resolveNetworkShares,
            string resolvePageMetaInfo,
            string saveResponse,
            string pingHost,
            string dnsLookupOnHost)
        {
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            DurationSeconds = durationSeconds;
            PingTimeoutSeconds = pingTimeoutSeconds;
            HttpRequestTimeoutSeconds = httpRequestTimeoutSeconds;
            FtpRequestTimeoutSeconds = ftpRequestTimeoutSeconds;
            HttpAutoRedirection = httpAutoRedirection;
            SslCertificateValidation = sslCertificateValidation;
            ThreadsCount = threadsCount;
            ResolveNetworkShares = resolveNetworkShares;
            ResolvePageMetaInfo = resolvePageMetaInfo;
            SaveResponse = saveResponse;
            PingHost = pingHost;
            DnsLookupOnHost = dnsLookupOnHost;
        }

        public string StartDateTime { get; }

        public string EndDateTime { get; }

        public int DurationSeconds { get; }

        public int PingTimeoutSeconds { get; }

        public int HttpRequestTimeoutSeconds { get; }

        public int FtpRequestTimeoutSeconds { get; }

        public string HttpAutoRedirection { get; }

        public string SslCertificateValidation { get; }

        public string ThreadsCount { get; }

        public string ResolveNetworkShares { get; }

        public string ResolvePageMetaInfo { get; }

        public string SaveResponse { get; }

        public string PingHost { get; }

        public string DnsLookupOnHost { get; }

        /// <summary>
        /// Creates a summary snapshot using the legacy string values already produced by the form.
        /// </summary>
        public static EndpointExportRunSummary Create(
            string startDateTime,
            string endDateTime,
            int durationSeconds,
            int pingTimeoutSeconds,
            int httpRequestTimeoutSeconds,
            int ftpRequestTimeoutSeconds,
            string httpAutoRedirection,
            string sslCertificateValidation,
            string threadsCount,
            string resolveNetworkShares,
            string resolvePageMetaInfo,
            string saveResponse,
            string pingHost,
            string dnsLookupOnHost)
        {
            return new EndpointExportRunSummary(
                startDateTime,
                endDateTime,
                durationSeconds,
                pingTimeoutSeconds,
                httpRequestTimeoutSeconds,
                ftpRequestTimeoutSeconds,
                httpAutoRedirection,
                sslCertificateValidation,
                threadsCount,
                resolveNetworkShares,
                resolvePageMetaInfo,
                saveResponse,
                pingHost,
                dnsLookupOnHost);
        }
    }
}
