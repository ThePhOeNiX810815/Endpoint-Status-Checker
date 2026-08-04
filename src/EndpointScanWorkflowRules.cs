using System;

namespace EndpointChecker
{
    internal static class EndpointScanWorkflowRules
    {
        public static bool IsHttpProtocolCheck(bool isProtocolValidation, bool cancellationPending, string protocol)
        {
            return isProtocolValidation &&
                   !cancellationPending &&
                   (string.Equals(protocol, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(protocol, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsFtpProtocolCheck(bool isProtocolValidation, bool cancellationPending, string protocol)
        {
            return isProtocolValidation &&
                   !cancellationPending &&
                   string.Equals(protocol, Uri.UriSchemeFtp, StringComparison.OrdinalIgnoreCase);
        }

        public static bool ShouldFollowManualRedirect(bool allowAutoRedirect, int statusCode, string locationHeader)
        {
            return allowAutoRedirect &&
                   statusCode >= 300 &&
                   statusCode <= 399 &&
                   !string.IsNullOrEmpty(locationHeader);
        }

        public static bool ShouldAppendRedirectSource(Uri endpointUri, Uri responseUri, bool autoRedirectFollowed)
        {
            if (endpointUri == null ||
                responseUri == null)
            {
                return autoRedirectFollowed;
            }

            return endpointUri.Scheme != responseUri.Scheme ||
                   endpointUri.Port != responseUri.Port ||
                   endpointUri.Host != responseUri.Host ||
                   autoRedirectFollowed;
        }

        public static bool ShouldRecordProtocolDuration(bool isProtocolValidation, bool cancellationPending)
        {
            return isProtocolValidation && !cancellationPending;
        }

        public static bool ShouldRunPing(bool cancellationPending, bool testPing)
        {
            return !cancellationPending && testPing;
        }

        public static bool ShouldMarkPingCheckMessage(bool isPingValidation, bool cancellationPending, bool testPing)
        {
            return isPingValidation && ShouldRunPing(cancellationPending, testPing);
        }

        public static bool ShouldMarkTerminated(bool cancellationPending)
        {
            return cancellationPending;
        }

        public static bool ShouldUpdateLastSeenOnline(
            bool isProtocolValidation,
            string responseCode,
            bool isPingValidation,
            string pingRoundtripTime,
            string statusError,
            string statusNotAvailable)
        {
            return (isProtocolValidation &&
                    responseCode != statusError &&
                    responseCode != statusNotAvailable) ||
                   (isPingValidation &&
                    pingRoundtripTime != statusNotAvailable);
        }

        public static bool ShouldAttemptCloudflareBypass(bool cloudflareProtected, bool cloudflareBypassEnabled)
        {
            return cloudflareProtected && cloudflareBypassEnabled;
        }
    }
}