using System;

namespace EndpointChecker
{
    internal static class EndpointPingRetryExecutorTests
    {
        public static void Register()
        {
            EndpointCheckingCoreTestRunner.Run("Ping retry executor retries timeout until success", PingRetryExecutorRetriesTimeoutUntilSuccess);
            EndpointCheckingCoreTestRunner.Run("Ping retry executor stops after max timeout retries", PingRetryExecutorStopsAfterMaxTimeoutRetries);
            EndpointCheckingCoreTestRunner.Run("Ping retry executor does not retry non-timeout failures", PingRetryExecutorDoesNotRetryNonTimeoutFailures);
            EndpointCheckingCoreTestRunner.Run("Ping retry executor formats immediate success roundtrip", PingRetryExecutorFormatsImmediateSuccessRoundtrip);
        }

        private static void PingRetryExecutorRetriesTimeoutUntilSuccess()
        {
            int attempts = 0;

            string pingTime = EndpointPingRetryExecutor.Execute(
                () =>
                {
                    attempts++;
                    if (attempts < 3)
                    {
                        return new EndpointPingAttemptResult(false, true, 0);
                    }

                    return new EndpointPingAttemptResult(true, false, 42);
                },
                maxRetryCount: 2);

            EndpointCheckingCoreTestRunner.AssertEqual(3, attempts);
            EndpointCheckingCoreTestRunner.AssertEqual("42 ms", pingTime);
        }

        private static void PingRetryExecutorStopsAfterMaxTimeoutRetries()
        {
            int attempts = 0;

            string pingTime = EndpointPingRetryExecutor.Execute(
                () =>
                {
                    attempts++;
                    return new EndpointPingAttemptResult(false, true, 0);
                },
                maxRetryCount: 2);

            EndpointCheckingCoreTestRunner.AssertEqual(3, attempts);
            EndpointCheckingCoreTestRunner.AssertEqual(string.Empty, pingTime);
        }

        private static void PingRetryExecutorDoesNotRetryNonTimeoutFailures()
        {
            int attempts = 0;

            string pingTime = EndpointPingRetryExecutor.Execute(
                () =>
                {
                    attempts++;
                    return new EndpointPingAttemptResult(false, false, 0);
                },
                maxRetryCount: 5);

            EndpointCheckingCoreTestRunner.AssertEqual(1, attempts);
            EndpointCheckingCoreTestRunner.AssertEqual(string.Empty, pingTime);
        }

        private static void PingRetryExecutorFormatsImmediateSuccessRoundtrip()
        {
            string pingTime = EndpointPingRetryExecutor.Execute(
                () => new EndpointPingAttemptResult(true, false, 7),
                maxRetryCount: 3);

            EndpointCheckingCoreTestRunner.AssertEqual("7 ms", pingTime);
        }
    }

    internal static class EndpointFtpStatusMapperTests
    {
        public static void Register()
        {
            EndpointCheckingCoreTestRunner.Run("FTP mapper sanitizes non-2xx status descriptions", FtpMapperSanitizesNon2xxStatusDescriptions);
            EndpointCheckingCoreTestRunner.Run("FTP mapper falls back to banner message for 220 responses", FtpMapperFallsBackToBannerMessageFor220Responses);
            EndpointCheckingCoreTestRunner.Run("FTP mapper falls back to welcome message for 230 responses", FtpMapperFallsBackToWelcomeMessageFor230Responses);
            EndpointCheckingCoreTestRunner.Run("FTP mapper preserves web exception status message chain", FtpMapperPreservesWebExceptionStatusMessageChain);
            EndpointCheckingCoreTestRunner.Run("FTP mapper avoids duplicate inner exception messages", FtpMapperAvoidsDuplicateInnerExceptionMessages);
        }

        private static void FtpMapperSanitizesNon2xxStatusDescriptions()
        {
            EndpointFtpStatusMapResult result = EndpointFtpStatusMapper.Map(new EndpointFtpStatusMapInput
            {
                StatusError = "ERROR",
                StatusNotAvailable = "N/A",
                FtpStatusCode = 530,
                FtpStatusDescription = "530- Login incorrect",
                FtpBannerMessage = "220 Ready"
            });

            EndpointCheckingCoreTestRunner.AssertEqual("530", result.ResponseCode);
            EndpointCheckingCoreTestRunner.AssertEqual("Login incorrect", result.ResponseMessage);
            EndpointCheckingCoreTestRunner.AssertEqual("220 Ready", result.BannerMessage);
            EndpointCheckingCoreTestRunner.AssertEqual("530- Login incorrect", result.StatusDescription);
        }

        private static void FtpMapperFallsBackToBannerMessageFor220Responses()
        {
            EndpointFtpStatusMapResult result = EndpointFtpStatusMapper.Map(new EndpointFtpStatusMapInput
            {
                StatusError = "ERROR",
                StatusNotAvailable = "N/A",
                FtpStatusCode = 200,
                FtpBannerMessage = "220- FTP Service Ready"
            });

            EndpointCheckingCoreTestRunner.AssertEqual("220", result.ResponseCode);
            EndpointCheckingCoreTestRunner.AssertEqual("FTP Service Ready", result.ResponseMessage);
            EndpointCheckingCoreTestRunner.AssertEqual("220- FTP Service Ready", result.BannerMessage);
        }

        private static void FtpMapperFallsBackToWelcomeMessageFor230Responses()
        {
            EndpointFtpStatusMapResult result = EndpointFtpStatusMapper.Map(new EndpointFtpStatusMapInput
            {
                StatusError = "ERROR",
                StatusNotAvailable = "N/A",
                FtpStatusCode = 200,
                FtpWelcomeMessage = "230 User logged in"
            });

            EndpointCheckingCoreTestRunner.AssertEqual("230", result.ResponseCode);
            EndpointCheckingCoreTestRunner.AssertEqual("User logged in", result.ResponseMessage);
            EndpointCheckingCoreTestRunner.AssertEqual("230 User logged in", result.WelcomeMessage);
        }

        private static void FtpMapperPreservesWebExceptionStatusMessageChain()
        {
            EndpointFtpStatusMapResult result = EndpointFtpStatusMapper.Map(new EndpointFtpStatusMapInput
            {
                StatusError = "ERROR",
                StatusNotAvailable = "N/A",
                WebExceptionStatus = "Timeout",
                WebExceptionMessage = "The operation timed out.",
                WebExceptionInnerMessage = "No response from server."
            });

            EndpointCheckingCoreTestRunner.AssertEqual("ERROR", result.ResponseCode);
            EndpointCheckingCoreTestRunner.AssertEqual("Timeout -> The operation timed out. -> No response from server.", result.ResponseMessage);
        }

        private static void FtpMapperAvoidsDuplicateInnerExceptionMessages()
        {
            EndpointFtpStatusMapResult result = EndpointFtpStatusMapper.Map(new EndpointFtpStatusMapInput
            {
                StatusError = "ERROR",
                StatusNotAvailable = "N/A",
                WebExceptionStatus = "ConnectFailure",
                WebExceptionMessage = "Unable to connect.",
                WebExceptionInnerMessage = "Unable to connect."
            });

            EndpointCheckingCoreTestRunner.AssertEqual("ConnectFailure -> Unable to connect.", result.ResponseMessage);
        }
    }
}
