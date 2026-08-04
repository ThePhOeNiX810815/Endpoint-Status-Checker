using System;

namespace EndpointChecker
{
    internal static class EndpointHttpRedirectResolverTests
    {
        public static void Register()
        {
            EndpointCheckingCoreTestRunner.Run("Redirect resolver ignores non-redirect status", RedirectResolverIgnoresNonRedirectStatus);
            EndpointCheckingCoreTestRunner.Run("Redirect resolver requires location header", RedirectResolverRequiresLocationHeader);
            EndpointCheckingCoreTestRunner.Run("Redirect resolver returns absolute location URI", RedirectResolverReturnsAbsoluteLocationUri);
            EndpointCheckingCoreTestRunner.Run("Redirect resolver combines relative location with response URI", RedirectResolverCombinesRelativeLocationWithResponseUri);
        }

        private static void RedirectResolverIgnoresNonRedirectStatus()
        {
            Uri redirected;
            bool result = EndpointHttpRedirectResolver.TryResolveRedirectUri(true, 200, "https://example.test/new", new Uri("https://example.test/old"), out redirected);

            EndpointCheckingCoreTestRunner.AssertEqual(false, result);
            EndpointCheckingCoreTestRunner.AssertEqual(null, redirected);
        }

        private static void RedirectResolverRequiresLocationHeader()
        {
            Uri redirected;
            bool result = EndpointHttpRedirectResolver.TryResolveRedirectUri(true, 302, string.Empty, new Uri("https://example.test/old"), out redirected);

            EndpointCheckingCoreTestRunner.AssertEqual(false, result);
            EndpointCheckingCoreTestRunner.AssertEqual(null, redirected);
        }

        private static void RedirectResolverReturnsAbsoluteLocationUri()
        {
            Uri redirected;
            bool result = EndpointHttpRedirectResolver.TryResolveRedirectUri(true, 301, "https://example.test/new", new Uri("https://example.test/old"), out redirected);

            EndpointCheckingCoreTestRunner.AssertEqual(true, result);
            EndpointCheckingCoreTestRunner.AssertEqual("https://example.test/new", redirected.AbsoluteUri.TrimEnd('/'));
        }

        private static void RedirectResolverCombinesRelativeLocationWithResponseUri()
        {
            Uri redirected;
            bool result = EndpointHttpRedirectResolver.TryResolveRedirectUri(true, 302, "/new/path", new Uri("https://example.test/old/path"), out redirected);

            EndpointCheckingCoreTestRunner.AssertEqual(true, result);
            EndpointCheckingCoreTestRunner.AssertEqual("https://example.test/new/path", redirected.AbsoluteUri.TrimEnd('/'));
        }
    }

    internal static class EndpointScanTerminalFinalizerTests
    {
        public static void Register()
        {
            EndpointCheckingCoreTestRunner.Run("Terminal finalizer always stamps address and response time", TerminalFinalizerAlwaysStampsAddressAndResponseTime);
            EndpointCheckingCoreTestRunner.Run("Terminal finalizer cancellation overrides code and message", TerminalFinalizerCancellationOverridesCodeAndMessage);
            EndpointCheckingCoreTestRunner.Run("Terminal finalizer updates last seen for protocol success", TerminalFinalizerUpdatesLastSeenForProtocolSuccess);
            EndpointCheckingCoreTestRunner.Run("Terminal finalizer preserves last seen for protocol error", TerminalFinalizerPreservesLastSeenForProtocolError);
            EndpointCheckingCoreTestRunner.Run("Terminal finalizer updates last seen for ping success", TerminalFinalizerUpdatesLastSeenForPingSuccess);
            EndpointCheckingCoreTestRunner.Run("Terminal finalizer preserves last seen for ping not available", TerminalFinalizerPreservesLastSeenForPingNotAvailable);
        }

        private static void TerminalFinalizerAlwaysStampsAddressAndResponseTime()
        {
            EndpointScanFinalizeOutput output = Finalize(
                responseCode: "200",
                responseMessage: "OK",
                pingRoundtrip: "N/A",
                cancellationPending: false,
                isProtocolValidation: true,
                isPingValidation: false);

            EndpointCheckingCoreTestRunner.AssertEqual("http://example.test/source", output.Address);
            EndpointCheckingCoreTestRunner.AssertEqual("http://example.test/response", output.ResponseAddress);
            EndpointCheckingCoreTestRunner.AssertEqual("123 ms", output.ResponseTime);
        }

        private static void TerminalFinalizerCancellationOverridesCodeAndMessage()
        {
            EndpointScanFinalizeOutput output = Finalize(
                responseCode: "200",
                responseMessage: "OK",
                pingRoundtrip: "12 ms",
                cancellationPending: true,
                isProtocolValidation: true,
                isPingValidation: false);

            EndpointCheckingCoreTestRunner.AssertEqual("N/A", output.ResponseCode);
            EndpointCheckingCoreTestRunner.AssertEqual("Terminated", output.ResponseMessage);
            EndpointCheckingCoreTestRunner.AssertEqual("2020-01-01 00:00:00", output.LastSeenOnline);
        }

        private static void TerminalFinalizerUpdatesLastSeenForProtocolSuccess()
        {
            EndpointScanFinalizeOutput output = Finalize(
                responseCode: "200",
                responseMessage: "OK",
                pingRoundtrip: "N/A",
                cancellationPending: false,
                isProtocolValidation: true,
                isPingValidation: false);

            EndpointCheckingCoreTestRunner.AssertEqual("2026-08-04 10:20:30", output.LastSeenOnline);
        }

        private static void TerminalFinalizerPreservesLastSeenForProtocolError()
        {
            EndpointScanFinalizeOutput output = Finalize(
                responseCode: "ERROR",
                responseMessage: "ConnectFailure",
                pingRoundtrip: "N/A",
                cancellationPending: false,
                isProtocolValidation: true,
                isPingValidation: false);

            EndpointCheckingCoreTestRunner.AssertEqual("2020-01-01 00:00:00", output.LastSeenOnline);
        }

        private static void TerminalFinalizerUpdatesLastSeenForPingSuccess()
        {
            EndpointScanFinalizeOutput output = Finalize(
                responseCode: "N/A",
                responseMessage: "Ping Check Only",
                pingRoundtrip: "7 ms",
                cancellationPending: false,
                isProtocolValidation: false,
                isPingValidation: true);

            EndpointCheckingCoreTestRunner.AssertEqual("2026-08-04 10:20:30", output.LastSeenOnline);
        }

        private static void TerminalFinalizerPreservesLastSeenForPingNotAvailable()
        {
            EndpointScanFinalizeOutput output = Finalize(
                responseCode: "N/A",
                responseMessage: "Ping Check Only",
                pingRoundtrip: "N/A",
                cancellationPending: false,
                isProtocolValidation: false,
                isPingValidation: true);

            EndpointCheckingCoreTestRunner.AssertEqual("2020-01-01 00:00:00", output.LastSeenOnline);
        }

        private static EndpointScanFinalizeOutput Finalize(
            string responseCode,
            string responseMessage,
            string pingRoundtrip,
            bool cancellationPending,
            bool isProtocolValidation,
            bool isPingValidation)
        {
            return EndpointScanTerminalFinalizer.Finalize(new EndpointScanFinalizeInput
            {
                EndpointAddress = "http://example.test/source",
                ResponseAddress = "http://example.test/response",
                DurationTime = "123 ms",
                ResponseCode = responseCode,
                ResponseMessage = responseMessage,
                PingRoundtripTime = pingRoundtrip,
                LastSeenOnline = "2020-01-01 00:00:00",
                CancellationPending = cancellationPending,
                IsProtocolValidation = isProtocolValidation,
                IsPingValidation = isPingValidation,
                StatusError = "ERROR",
                StatusNotAvailable = "N/A",
                TerminatedMessage = "Terminated",
                LastSeenNow = "2026-08-04 10:20:30"
            });
        }
    }
}
