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
}
