using System;

namespace EndpointChecker
{
    internal static class EndpointCheckingCoreTestRunner
    {
        private static int failed;

        private static void Main()
        {
            EndpointCheckOptionsTests.Register();
            EndpointCheckResultFactoryTests.Register();
            EndpointCheckProgressTests.Register();
            EndpointScanWorkflowRulesTests.Register();
            EndpointPingRetryExecutorTests.Register();
            EndpointFtpRequestFactoryTests.Register();
            EndpointFtpStatusMapperTests.Register();
            EndpointHttpRedirectResolverTests.Register();
            EndpointNetworkIdentityResolverTests.Register();
            EndpointNetworkShareAcquisitionTests.Register();
            EndpointNetworkShareEnumeratorTests.Register();
            EndpointNetworkShareResolverTests.Register();
            EndpointSslCertificateAcquisitionTests.Register();
            EndpointSslCertificatePropertyMapperTests.Register();
            EndpointTaskSyncBridgeTests.Register();
            EndpointVirusTotalScanRetryExecutorTests.Register();
            EndpointScanTerminalFinalizerTests.Register();
            UiThreadHelpersTests.Register();
            ReportMailTableBuilderTests.Register();

            if (failed > 0)
            {
                Environment.Exit(1);
            }
        }

        public static void Run(string name, Action test)
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

        public static void AssertEqual<T>(T expected, T actual)
        {
            if (!object.Equals(expected, actual))
            {
                throw new InvalidOperationException("Expected <" + expected + "> but was <" + actual + ">.");
            }
        }

        public static void AssertArray(string[] expected, string[] actual)
        {
            if (expected.Length != actual.Length)
            {
                throw new InvalidOperationException("Expected array length <" + expected.Length + "> but was <" + actual.Length + ">.");
            }

            for (int i = 0; i < expected.Length; i++)
            {
                AssertEqual(expected[i], actual[i]);
            }
        }
    }

    internal static class EndpointCheckOptionsTests
    {
        public static void Register()
        {
            Run("UI seconds are converted to millisecond timeouts", UiSecondsAreConvertedToMillisecondTimeouts);
            Run("Boolean options preserve UI values", BooleanOptionsPreserveUiValues);
            Run("Thread count is reduced to enabled endpoint count", ThreadCountIsReducedToEnabledEndpointCount);
            Run("Thread count is unchanged when enabled endpoint count is zero", ThreadCountIsUnchangedWhenEnabledEndpointCountIsZero);
            Run("Thread count is unchanged when enabled endpoint count is greater", ThreadCountIsUnchangedWhenEnabledEndpointCountIsGreater);
        }

        private static void UiSecondsAreConvertedToMillisecondTimeouts()
        {
            EndpointCheckOptions options = BuildOptions(
                pingTimeoutSeconds: 3,
                httpRequestTimeoutSeconds: 30,
                ftpRequestTimeoutSeconds: 45);

            EndpointCheckingCoreTestRunner.AssertEqual(3000, options.PingTimeout);
            EndpointCheckingCoreTestRunner.AssertEqual(30000, options.HttpRequestTimeout);
            EndpointCheckingCoreTestRunner.AssertEqual(45000, options.FtpRequestTimeout);
        }

        private static void BooleanOptionsPreserveUiValues()
        {
            EndpointCheckOptions options = EndpointCheckOptions.FromUiValues(
                allowAutoRedirect: true,
                validateSslCertificate: false,
                autoAdjustRefreshTimer: true,
                resolveNetworkShares: false,
                resolvePageMetaInfo: true,
                removeUrlParameters: false,
                resolvePageLinks: true,
                saveResponse: false,
                testPing: true,
                resolveDnsNames: false,
                resolveIpAddresses: true,
                resolveMacAddresses: false,
                threadsCount: 7,
                pingTimeoutSeconds: 1,
                httpRequestTimeoutSeconds: 2,
                ftpRequestTimeoutSeconds: 3);

            EndpointCheckingCoreTestRunner.AssertEqual(true, options.AllowAutoRedirect);
            EndpointCheckingCoreTestRunner.AssertEqual(false, options.ValidateSslCertificate);
            EndpointCheckingCoreTestRunner.AssertEqual(true, options.AutoAdjustRefreshTimer);
            EndpointCheckingCoreTestRunner.AssertEqual(false, options.ResolveNetworkShares);
            EndpointCheckingCoreTestRunner.AssertEqual(true, options.ResolvePageMetaInfo);
            EndpointCheckingCoreTestRunner.AssertEqual(false, options.RemoveUrlParameters);
            EndpointCheckingCoreTestRunner.AssertEqual(true, options.ResolvePageLinks);
            EndpointCheckingCoreTestRunner.AssertEqual(false, options.SaveResponse);
            EndpointCheckingCoreTestRunner.AssertEqual(true, options.TestPing);
            EndpointCheckingCoreTestRunner.AssertEqual(false, options.ResolveDnsNames);
            EndpointCheckingCoreTestRunner.AssertEqual(true, options.ResolveIpAddresses);
            EndpointCheckingCoreTestRunner.AssertEqual(false, options.ResolveMacAddresses);
            EndpointCheckingCoreTestRunner.AssertEqual(7, options.ThreadsCount);
        }

        private static void ThreadCountIsReducedToEnabledEndpointCount()
        {
            EndpointCheckOptions options = BuildOptions(threadsCount: 8)
                .WithThreadCountAdjustedForEnabledEndpoints(3);

            EndpointCheckingCoreTestRunner.AssertEqual(3, options.ThreadsCount);
        }

        private static void ThreadCountIsUnchangedWhenEnabledEndpointCountIsZero()
        {
            EndpointCheckOptions options = BuildOptions(threadsCount: 8)
                .WithThreadCountAdjustedForEnabledEndpoints(0);

            EndpointCheckingCoreTestRunner.AssertEqual(8, options.ThreadsCount);
        }

        private static void ThreadCountIsUnchangedWhenEnabledEndpointCountIsGreater()
        {
            EndpointCheckOptions options = BuildOptions(threadsCount: 4)
                .WithThreadCountAdjustedForEnabledEndpoints(8);

            EndpointCheckingCoreTestRunner.AssertEqual(4, options.ThreadsCount);
        }

        private static EndpointCheckOptions BuildOptions(
            int threadsCount = 1,
            int pingTimeoutSeconds = 1,
            int httpRequestTimeoutSeconds = 1,
            int ftpRequestTimeoutSeconds = 1)
        {
            return EndpointCheckOptions.FromUiValues(
                allowAutoRedirect: false,
                validateSslCertificate: false,
                autoAdjustRefreshTimer: false,
                resolveNetworkShares: false,
                resolvePageMetaInfo: false,
                removeUrlParameters: false,
                resolvePageLinks: false,
                saveResponse: false,
                testPing: false,
                resolveDnsNames: false,
                resolveIpAddresses: false,
                resolveMacAddresses: false,
                threadsCount: threadsCount,
                pingTimeoutSeconds: pingTimeoutSeconds,
                httpRequestTimeoutSeconds: httpRequestTimeoutSeconds,
                ftpRequestTimeoutSeconds: ftpRequestTimeoutSeconds);
        }

        private static void Run(string name, Action test) => EndpointCheckingCoreTestRunner.Run(name, test);
    }

    internal static class EndpointScanWorkflowRulesTests
    {
        public static void Register()
        {
            Run("HTTP protocol routing respects protocol validation and cancellation", HttpProtocolRoutingRespectsProtocolValidationAndCancellation);
            Run("FTP protocol routing respects protocol validation and cancellation", FtpProtocolRoutingRespectsProtocolValidationAndCancellation);
            Run("Protocol route resolver maps HTTP HTTPS FTP and None", ProtocolRouteResolverMapsHttpHttpsFtpAndNone);
            Run("Manual redirect follow requires 3xx and location header", ManualRedirectFollowRequires3xxAndLocationHeader);
            Run("Redirect annotation follows URI changes and explicit follow flag", RedirectAnnotationFollowsUriChangesAndExplicitFollowFlag);
            Run("Ping check message is set only in ping validation mode", PingCheckMessageIsSetOnlyInPingValidationMode);
            Run("Cancellation semantics mark endpoint terminated", CancellationSemanticsMarkEndpointTerminated);
            Run("Last seen online updates only for terminal success states", LastSeenOnlineUpdatesOnlyForTerminalSuccessStates);
            Run("Cloudflare bypass path is enabled only when configured", CloudflareBypassPathIsEnabledOnlyWhenConfigured);
        }

        private static void HttpProtocolRoutingRespectsProtocolValidationAndCancellation()
        {
            EndpointCheckingCoreTestRunner.AssertEqual(true, EndpointScanWorkflowRules.IsHttpProtocolCheck(true, false, "http"));
            EndpointCheckingCoreTestRunner.AssertEqual(true, EndpointScanWorkflowRules.IsHttpProtocolCheck(true, false, "HTTPS"));
            EndpointCheckingCoreTestRunner.AssertEqual(false, EndpointScanWorkflowRules.IsHttpProtocolCheck(false, false, "http"));
            EndpointCheckingCoreTestRunner.AssertEqual(false, EndpointScanWorkflowRules.IsHttpProtocolCheck(true, true, "http"));
            EndpointCheckingCoreTestRunner.AssertEqual(false, EndpointScanWorkflowRules.IsHttpProtocolCheck(true, false, "ftp"));
        }

        private static void FtpProtocolRoutingRespectsProtocolValidationAndCancellation()
        {
            EndpointCheckingCoreTestRunner.AssertEqual(true, EndpointScanWorkflowRules.IsFtpProtocolCheck(true, false, "ftp"));
            EndpointCheckingCoreTestRunner.AssertEqual(true, EndpointScanWorkflowRules.IsFtpProtocolCheck(true, false, "FTP"));
            EndpointCheckingCoreTestRunner.AssertEqual(false, EndpointScanWorkflowRules.IsFtpProtocolCheck(true, true, "ftp"));
            EndpointCheckingCoreTestRunner.AssertEqual(false, EndpointScanWorkflowRules.IsFtpProtocolCheck(false, false, "ftp"));
            EndpointCheckingCoreTestRunner.AssertEqual(false, EndpointScanWorkflowRules.IsFtpProtocolCheck(true, false, "http"));
        }

        private static void ProtocolRouteResolverMapsHttpHttpsFtpAndNone()
        {
            EndpointCheckingCoreTestRunner.AssertEqual(EndpointProtocolRoute.Http, EndpointProtocolRouteResolver.Resolve(true, false, "http"));
            EndpointCheckingCoreTestRunner.AssertEqual(EndpointProtocolRoute.Http, EndpointProtocolRouteResolver.Resolve(true, false, "HTTPS"));
            EndpointCheckingCoreTestRunner.AssertEqual(EndpointProtocolRoute.Ftp, EndpointProtocolRouteResolver.Resolve(true, false, "ftp"));
            EndpointCheckingCoreTestRunner.AssertEqual(EndpointProtocolRoute.None, EndpointProtocolRouteResolver.Resolve(true, false, "mailto"));
            EndpointCheckingCoreTestRunner.AssertEqual(EndpointProtocolRoute.None, EndpointProtocolRouteResolver.Resolve(false, false, "http"));
            EndpointCheckingCoreTestRunner.AssertEqual(EndpointProtocolRoute.None, EndpointProtocolRouteResolver.Resolve(true, true, "http"));
        }

        private static void ManualRedirectFollowRequires3xxAndLocationHeader()
        {
            EndpointCheckingCoreTestRunner.AssertEqual(true, EndpointScanWorkflowRules.ShouldFollowManualRedirect(true, 302, "https://example.com/new"));
            EndpointCheckingCoreTestRunner.AssertEqual(false, EndpointScanWorkflowRules.ShouldFollowManualRedirect(false, 302, "https://example.com/new"));
            EndpointCheckingCoreTestRunner.AssertEqual(false, EndpointScanWorkflowRules.ShouldFollowManualRedirect(true, 200, "https://example.com/new"));
            EndpointCheckingCoreTestRunner.AssertEqual(false, EndpointScanWorkflowRules.ShouldFollowManualRedirect(true, 301, null));
        }

        private static void RedirectAnnotationFollowsUriChangesAndExplicitFollowFlag()
        {
            Uri endpointUri = new Uri("http://example.com/path");
            Uri sameResponseUri = new Uri("http://example.com/path");
            Uri changedResponseUri = new Uri("https://example.com/path");

            EndpointCheckingCoreTestRunner.AssertEqual(false, EndpointScanWorkflowRules.ShouldAppendRedirectSource(endpointUri, sameResponseUri, false));
            EndpointCheckingCoreTestRunner.AssertEqual(true, EndpointScanWorkflowRules.ShouldAppendRedirectSource(endpointUri, changedResponseUri, false));
            EndpointCheckingCoreTestRunner.AssertEqual(true, EndpointScanWorkflowRules.ShouldAppendRedirectSource(endpointUri, sameResponseUri, true));
        }

        private static void PingCheckMessageIsSetOnlyInPingValidationMode()
        {
            EndpointCheckingCoreTestRunner.AssertEqual(true, EndpointScanWorkflowRules.ShouldMarkPingCheckMessage(true, false, true));
            EndpointCheckingCoreTestRunner.AssertEqual(false, EndpointScanWorkflowRules.ShouldMarkPingCheckMessage(false, false, true));
            EndpointCheckingCoreTestRunner.AssertEqual(false, EndpointScanWorkflowRules.ShouldMarkPingCheckMessage(true, true, true));
            EndpointCheckingCoreTestRunner.AssertEqual(false, EndpointScanWorkflowRules.ShouldMarkPingCheckMessage(true, false, false));
        }

        private static void CancellationSemanticsMarkEndpointTerminated()
        {
            EndpointCheckingCoreTestRunner.AssertEqual(true, EndpointScanWorkflowRules.ShouldMarkTerminated(true));
            EndpointCheckingCoreTestRunner.AssertEqual(false, EndpointScanWorkflowRules.ShouldMarkTerminated(false));
            EndpointCheckingCoreTestRunner.AssertEqual(false, EndpointScanWorkflowRules.ShouldRunPing(true, true));
            EndpointCheckingCoreTestRunner.AssertEqual(false, EndpointScanWorkflowRules.ShouldRecordProtocolDuration(true, true));
        }

        private static void LastSeenOnlineUpdatesOnlyForTerminalSuccessStates()
        {
            EndpointCheckingCoreTestRunner.AssertEqual(true, EndpointScanWorkflowRules.ShouldUpdateLastSeenOnline(true, "200", false, "N/A", "ERROR", "N/A"));
            EndpointCheckingCoreTestRunner.AssertEqual(false, EndpointScanWorkflowRules.ShouldUpdateLastSeenOnline(true, "ERROR", false, "N/A", "ERROR", "N/A"));
            EndpointCheckingCoreTestRunner.AssertEqual(false, EndpointScanWorkflowRules.ShouldUpdateLastSeenOnline(true, "N/A", false, "N/A", "ERROR", "N/A"));
            EndpointCheckingCoreTestRunner.AssertEqual(true, EndpointScanWorkflowRules.ShouldUpdateLastSeenOnline(false, "N/A", true, "12 ms", "ERROR", "N/A"));
            EndpointCheckingCoreTestRunner.AssertEqual(false, EndpointScanWorkflowRules.ShouldUpdateLastSeenOnline(false, "N/A", true, "N/A", "ERROR", "N/A"));
        }

        private static void CloudflareBypassPathIsEnabledOnlyWhenConfigured()
        {
            EndpointCheckingCoreTestRunner.AssertEqual(true, EndpointScanWorkflowRules.ShouldAttemptCloudflareBypass(true, true));
            EndpointCheckingCoreTestRunner.AssertEqual(false, EndpointScanWorkflowRules.ShouldAttemptCloudflareBypass(true, false));
            EndpointCheckingCoreTestRunner.AssertEqual(false, EndpointScanWorkflowRules.ShouldAttemptCloudflareBypass(false, true));
        }

        private static void Run(string name, Action test) => EndpointCheckingCoreTestRunner.Run(name, test);
    }
}
