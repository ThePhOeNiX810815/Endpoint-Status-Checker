using System;
using System.Collections.Generic;

namespace EndpointChecker
{
    internal static class EndpointVirusTotalScanRetryExecutorTests
    {
        public static void Register()
        {
            EndpointCheckingCoreTestRunner.Run("VirusTotal retry executor immediate success completes in one attempt", VirusTotalRetryExecutorImmediateSuccessCompletesInOneAttempt);
            EndpointCheckingCoreTestRunner.Run("VirusTotal retry executor retries then succeeds", VirusTotalRetryExecutorRetriesThenSucceeds);
            EndpointCheckingCoreTestRunner.Run("VirusTotal retry executor retry exhaustion triggers terminal failure", VirusTotalRetryExecutorRetryExhaustionTriggersTerminalFailure);
            EndpointCheckingCoreTestRunner.Run("VirusTotal retry executor supports cancellation before first attempt", VirusTotalRetryExecutorSupportsCancellationBeforeFirstAttempt);
            EndpointCheckingCoreTestRunner.Run("VirusTotal retry executor supports cancellation during delay", VirusTotalRetryExecutorSupportsCancellationDuringDelay);
            EndpointCheckingCoreTestRunner.Run("VirusTotal retry status message preserves legacy exception mapping", VirusTotalRetryStatusMessagePreservesLegacyExceptionMapping);
            EndpointCheckingCoreTestRunner.Run("VirusTotal retry executor preserves exact attempt count semantics", VirusTotalRetryExecutorPreservesExactAttemptCountSemantics);
            EndpointCheckingCoreTestRunner.Run("VirusTotal retry executor preserves retry status ordering", VirusTotalRetryExecutorPreservesRetryStatusOrdering);
        }

        private static void VirusTotalRetryExecutorImmediateSuccessCompletesInOneAttempt()
        {
            int attempts = 0;

            EndpointVirusTotalRetryExecutionResult result = EndpointVirusTotalScanRetryExecutor.Execute(new EndpointVirusTotalRetryExecuteInput
            {
                MaxRetryCount = 24,
                InitialRetry = 0,
                RetryDelayMilliseconds = 0,
                ExecuteAttempt = () =>
                {
                    attempts++;
                    return new EndpointVirusTotalRetryAttemptResult { Payload = "ok", StatusMessage = "queued" };
                },
                ShouldRetryException = _ => true,
            });

            EndpointCheckingCoreTestRunner.AssertEqual(1, attempts);
            EndpointCheckingCoreTestRunner.AssertEqual(1, result.AttemptCount);
            EndpointCheckingCoreTestRunner.AssertEqual(true, result.Completed);
            EndpointCheckingCoreTestRunner.AssertEqual(false, result.Failed);
            EndpointCheckingCoreTestRunner.AssertEqual(false, result.Cancelled);
        }

        private static void VirusTotalRetryExecutorRetriesThenSucceeds()
        {
            int attempts = 0;
            int retryCallbacks = 0;
            string lastStatus = null;

            EndpointVirusTotalRetryExecutionResult result = EndpointVirusTotalScanRetryExecutor.Execute(new EndpointVirusTotalRetryExecuteInput
            {
                MaxRetryCount = 24,
                InitialRetry = 0,
                RetryDelayMilliseconds = 0,
                ExecuteAttempt = () =>
                {
                    attempts++;
                    if (attempts == 1)
                    {
                        throw new InvalidOperationException("retry-me");
                    }

                    return new EndpointVirusTotalRetryAttemptResult { Payload = "ok", StatusMessage = "queued" };
                },
                ShouldRetryException = _ => true,
                OnRetrying = (exception, retry) =>
                {
                    retryCallbacks++;
                    lastStatus = EndpointVirusTotalScanRetryExecutor.BuildLegacyRetryStatusMessage(exception, retry);
                },
            });

            EndpointCheckingCoreTestRunner.AssertEqual(2, attempts);
            EndpointCheckingCoreTestRunner.AssertEqual(2, result.AttemptCount);
            EndpointCheckingCoreTestRunner.AssertEqual(1, retryCallbacks);
            EndpointCheckingCoreTestRunner.AssertEqual(true, lastStatus.Contains("retry-me"));
            EndpointCheckingCoreTestRunner.AssertEqual(true, result.Completed);
        }

        private static void VirusTotalRetryExecutorRetryExhaustionTriggersTerminalFailure()
        {
            int attempts = 0;
            int failures = 0;

            EndpointVirusTotalRetryExecutionResult result = EndpointVirusTotalScanRetryExecutor.Execute(new EndpointVirusTotalRetryExecuteInput
            {
                MaxRetryCount = 2,
                InitialRetry = 0,
                RetryDelayMilliseconds = 0,
                ExecuteAttempt = () =>
                {
                    attempts++;
                    throw new InvalidOperationException("always-fails");
                },
                ShouldRetryException = _ => true,
                OnFailed = _ => failures++,
            });

            EndpointCheckingCoreTestRunner.AssertEqual(4, attempts);
            EndpointCheckingCoreTestRunner.AssertEqual(4, result.AttemptCount);
            EndpointCheckingCoreTestRunner.AssertEqual(1, failures);
            EndpointCheckingCoreTestRunner.AssertEqual(false, result.Completed);
            EndpointCheckingCoreTestRunner.AssertEqual(true, result.Failed);
            EndpointCheckingCoreTestRunner.AssertEqual(false, result.Cancelled);
        }

        private static void VirusTotalRetryExecutorSupportsCancellationBeforeFirstAttempt()
        {
            int attempts = 0;

            EndpointVirusTotalRetryExecutionResult result = EndpointVirusTotalScanRetryExecutor.Execute(new EndpointVirusTotalRetryExecuteInput
            {
                MaxRetryCount = 10,
                InitialRetry = 0,
                RetryDelayMilliseconds = 0,
                ExecuteAttempt = () =>
                {
                    attempts++;
                    return new EndpointVirusTotalRetryAttemptResult { Payload = "never" };
                },
                ShouldRetryException = _ => true,
                IsCancellationRequested = () => true,
            });

            EndpointCheckingCoreTestRunner.AssertEqual(0, attempts);
            EndpointCheckingCoreTestRunner.AssertEqual(0, result.AttemptCount);
            EndpointCheckingCoreTestRunner.AssertEqual(true, result.Cancelled);
            EndpointCheckingCoreTestRunner.AssertEqual(false, result.Completed);
        }

        private static void VirusTotalRetryExecutorSupportsCancellationDuringDelay()
        {
            int attempts = 0;
            bool cancelled = false;
            int retryCallbacks = 0;

            EndpointVirusTotalRetryExecutionResult result = EndpointVirusTotalScanRetryExecutor.Execute(new EndpointVirusTotalRetryExecuteInput
            {
                MaxRetryCount = 10,
                InitialRetry = 0,
                RetryDelayMilliseconds = 5000,
                ExecuteAttempt = () =>
                {
                    attempts++;
                    throw new InvalidOperationException("delay-cancel");
                },
                ShouldRetryException = _ => true,
                OnRetrying = (_, __) => retryCallbacks++,
                IsCancellationRequested = () => cancelled,
                Delay = (_, __) =>
                {
                    cancelled = true;
                    return false;
                },
            });

            EndpointCheckingCoreTestRunner.AssertEqual(1, attempts);
            EndpointCheckingCoreTestRunner.AssertEqual(1, result.AttemptCount);
            EndpointCheckingCoreTestRunner.AssertEqual(1, retryCallbacks);
            EndpointCheckingCoreTestRunner.AssertEqual(true, result.Cancelled);
        }

        private static void VirusTotalRetryStatusMessagePreservesLegacyExceptionMapping()
        {
            Exception wrapped = new Exception("outer", new InvalidOperationException("inner-message"));

            string statusWithoutRetry = EndpointVirusTotalScanRetryExecutor.BuildLegacyRetryStatusMessage(wrapped, 0);
            string statusWithRetry = EndpointVirusTotalScanRetryExecutor.BuildLegacyRetryStatusMessage(wrapped, 3);

            EndpointCheckingCoreTestRunner.AssertEqual("inner-message", statusWithoutRetry);
            EndpointCheckingCoreTestRunner.AssertEqual("inner-message [Retry 3]", statusWithRetry);
        }

        private static void VirusTotalRetryExecutorPreservesExactAttemptCountSemantics()
        {
            int attempts = 0;

            EndpointVirusTotalRetryExecutionResult result = EndpointVirusTotalScanRetryExecutor.Execute(new EndpointVirusTotalRetryExecuteInput
            {
                MaxRetryCount = 3,
                InitialRetry = 2,
                RetryDelayMilliseconds = 0,
                ExecuteAttempt = () =>
                {
                    attempts++;
                    throw new InvalidOperationException("always-fails");
                },
                ShouldRetryException = _ => true,
            });

            EndpointCheckingCoreTestRunner.AssertEqual(3, attempts);
            EndpointCheckingCoreTestRunner.AssertEqual(3, result.AttemptCount);
            EndpointCheckingCoreTestRunner.AssertEqual(true, result.Failed);
        }

        private static void VirusTotalRetryExecutorPreservesRetryStatusOrdering()
        {
            int attempts = 0;
            List<string> transitions = new List<string>();

            EndpointVirusTotalRetryExecutionResult result = EndpointVirusTotalScanRetryExecutor.Execute(new EndpointVirusTotalRetryExecuteInput
            {
                MaxRetryCount = 5,
                InitialRetry = 0,
                RetryDelayMilliseconds = 0,
                ExecuteAttempt = () =>
                {
                    attempts++;
                    if (attempts <= 2)
                    {
                        throw new InvalidOperationException("retry-" + attempts);
                    }

                    return new EndpointVirusTotalRetryAttemptResult { Payload = "ok", StatusMessage = "queued" };
                },
                ShouldRetryException = _ => true,
                OnRetrying = (_, retry) => transitions.Add("retry:" + retry),
                OnSuccess = _ => transitions.Add("success"),
            });

            EndpointCheckingCoreTestRunner.AssertEqual(true, result.Completed);
            EndpointCheckingCoreTestRunner.AssertEqual(3, result.AttemptCount);
            EndpointCheckingCoreTestRunner.AssertEqual(3, transitions.Count);
            EndpointCheckingCoreTestRunner.AssertEqual("retry:0", transitions[0]);
            EndpointCheckingCoreTestRunner.AssertEqual("retry:1", transitions[1]);
            EndpointCheckingCoreTestRunner.AssertEqual("success", transitions[2]);
        }
    }
}
