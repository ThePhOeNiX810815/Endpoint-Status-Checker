using System;
using System.Net.NetworkInformation;

namespace EndpointChecker
{
    internal readonly struct EndpointPingAttemptResult
    {
        public EndpointPingAttemptResult(bool isSuccess, bool isTimeout, long roundtripMilliseconds)
        {
            IsSuccess = isSuccess;
            IsTimeout = isTimeout;
            RoundtripMilliseconds = roundtripMilliseconds;
        }

        public bool IsSuccess { get; }

        public bool IsTimeout { get; }

        public long RoundtripMilliseconds { get; }

        public static EndpointPingAttemptResult FromReply(PingReply pingReply)
        {
            return new EndpointPingAttemptResult(
                pingReply != null && pingReply.Status == IPStatus.Success,
                pingReply != null && pingReply.Status == IPStatus.TimedOut,
                pingReply != null ? pingReply.RoundtripTime : 0);
        }
    }

    internal static class EndpointPingRetryExecutor
    {
        public static string Execute(Func<EndpointPingAttemptResult> sendPingAttempt, int maxRetryCount, int retryCount = 0)
        {
            EndpointPingAttemptResult attempt = sendPingAttempt();

            if (attempt.IsSuccess)
            {
                return attempt.RoundtripMilliseconds + " ms";
            }

            if (attempt.IsTimeout && retryCount < maxRetryCount)
            {
                return Execute(sendPingAttempt, maxRetryCount, retryCount + 1);
            }

            return string.Empty;
        }
    }
}
