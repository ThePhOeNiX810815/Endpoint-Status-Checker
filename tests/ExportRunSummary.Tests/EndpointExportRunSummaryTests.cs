using System;

namespace EndpointChecker
{
    internal static class EndpointExportRunSummaryTests
    {
        private static int failed;

        private static void Main()
        {
            Run("Snapshot preserves export summary values", SnapshotPreservesExportSummaryValues);

            if (failed > 0)
            {
                Environment.Exit(1);
            }
        }

        private static void SnapshotPreservesExportSummaryValues()
        {
            EndpointExportRunSummary summary = EndpointExportRunSummary.Create(
                "01.08.2026 10:00:00",
                "01.08.2026 10:00:05",
                5,
                1,
                2,
                3,
                "Enabled",
                "Disabled",
                "4",
                "Enabled",
                "Disabled",
                "Enabled",
                "Disabled",
                "Enabled");

            AssertEqual("01.08.2026 10:00:00", summary.StartDateTime);
            AssertEqual("01.08.2026 10:00:05", summary.EndDateTime);
            AssertEqual(5, summary.DurationSeconds);
            AssertEqual(1, summary.PingTimeoutSeconds);
            AssertEqual(2, summary.HttpRequestTimeoutSeconds);
            AssertEqual(3, summary.FtpRequestTimeoutSeconds);
            AssertEqual("Enabled", summary.HttpAutoRedirection);
            AssertEqual("Disabled", summary.SslCertificateValidation);
            AssertEqual("4", summary.ThreadsCount);
            AssertEqual("Enabled", summary.ResolveNetworkShares);
            AssertEqual("Disabled", summary.ResolvePageMetaInfo);
            AssertEqual("Enabled", summary.SaveResponse);
            AssertEqual("Disabled", summary.PingHost);
            AssertEqual("Enabled", summary.DnsLookupOnHost);
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
