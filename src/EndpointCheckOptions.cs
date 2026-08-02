using System;

namespace EndpointChecker
{
    /// <summary>
    /// Captures endpoint-check options from the UI before background work starts.
    /// </summary>
    /// <remarks>
    /// Timeout values are stored in milliseconds because the legacy request APIs consume
    /// milliseconds, while the UI exposes seconds.
    /// </remarks>
    internal sealed class EndpointCheckOptions
    {
        private EndpointCheckOptions(
            bool allowAutoRedirect,
            bool validateSslCertificate,
            bool autoAdjustRefreshTimer,
            bool resolveNetworkShares,
            bool resolvePageMetaInfo,
            bool removeUrlParameters,
            bool resolvePageLinks,
            bool saveResponse,
            bool testPing,
            bool resolveDnsNames,
            bool resolveIpAddresses,
            bool resolveMacAddresses,
            int threadsCount,
            int pingTimeout,
            int httpRequestTimeout,
            int ftpRequestTimeout)
        {
            AllowAutoRedirect = allowAutoRedirect;
            ValidateSslCertificate = validateSslCertificate;
            AutoAdjustRefreshTimer = autoAdjustRefreshTimer;
            ResolveNetworkShares = resolveNetworkShares;
            ResolvePageMetaInfo = resolvePageMetaInfo;
            RemoveUrlParameters = removeUrlParameters;
            ResolvePageLinks = resolvePageLinks;
            SaveResponse = saveResponse;
            TestPing = testPing;
            ResolveDnsNames = resolveDnsNames;
            ResolveIpAddresses = resolveIpAddresses;
            ResolveMacAddresses = resolveMacAddresses;
            ThreadsCount = threadsCount;
            PingTimeout = pingTimeout;
            HttpRequestTimeout = httpRequestTimeout;
            FtpRequestTimeout = ftpRequestTimeout;
        }

        public bool AllowAutoRedirect { get; }
        public bool ValidateSslCertificate { get; }
        public bool AutoAdjustRefreshTimer { get; }
        public bool ResolveNetworkShares { get; }
        public bool ResolvePageMetaInfo { get; }
        public bool RemoveUrlParameters { get; }
        public bool ResolvePageLinks { get; }
        public bool SaveResponse { get; }
        public bool TestPing { get; }
        public bool ResolveDnsNames { get; }
        public bool ResolveIpAddresses { get; }
        public bool ResolveMacAddresses { get; }
        public int ThreadsCount { get; }
        public int PingTimeout { get; }
        public int HttpRequestTimeout { get; }
        public int FtpRequestTimeout { get; }

        /// <summary>
        /// Creates a snapshot from current UI values while preserving legacy seconds-to-milliseconds conversion.
        /// </summary>
        public static EndpointCheckOptions FromUiValues(
            bool allowAutoRedirect,
            bool validateSslCertificate,
            bool autoAdjustRefreshTimer,
            bool resolveNetworkShares,
            bool resolvePageMetaInfo,
            bool removeUrlParameters,
            bool resolvePageLinks,
            bool saveResponse,
            bool testPing,
            bool resolveDnsNames,
            bool resolveIpAddresses,
            bool resolveMacAddresses,
            int threadsCount,
            int pingTimeoutSeconds,
            int httpRequestTimeoutSeconds,
            int ftpRequestTimeoutSeconds)
        {
            return new EndpointCheckOptions(
                allowAutoRedirect,
                validateSslCertificate,
                autoAdjustRefreshTimer,
                resolveNetworkShares,
                resolvePageMetaInfo,
                removeUrlParameters,
                resolvePageLinks,
                saveResponse,
                testPing,
                resolveDnsNames,
                resolveIpAddresses,
                resolveMacAddresses,
                threadsCount,
                checked(pingTimeoutSeconds * 1000),
                checked(httpRequestTimeoutSeconds * 1000),
                checked(ftpRequestTimeoutSeconds * 1000));
        }

        /// <summary>
        /// Preserves the current behavior of reducing requested parallelism to the enabled endpoint count.
        /// </summary>
        public EndpointCheckOptions WithThreadCountAdjustedForEnabledEndpoints(int enabledEndpointsCount)
        {
            if (enabledEndpointsCount > 0 &&
                enabledEndpointsCount < ThreadsCount)
            {
                return WithThreadsCount(enabledEndpointsCount);
            }

            return this;
        }

        private EndpointCheckOptions WithThreadsCount(int threadsCount)
        {
            return new EndpointCheckOptions(
                AllowAutoRedirect,
                ValidateSslCertificate,
                AutoAdjustRefreshTimer,
                ResolveNetworkShares,
                ResolvePageMetaInfo,
                RemoveUrlParameters,
                ResolvePageLinks,
                SaveResponse,
                TestPing,
                ResolveDnsNames,
                ResolveIpAddresses,
                ResolveMacAddresses,
                threadsCount,
                PingTimeout,
                HttpRequestTimeout,
                FtpRequestTimeout);
        }
    }
}
