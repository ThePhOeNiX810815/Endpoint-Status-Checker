using System;
using System.Threading;

namespace EndpointChecker
{
    internal sealed class EndpointVirusTotalRetryAttemptResult
    {
        public object Payload { get; set; }

        public string StatusMessage { get; set; }
    }

    internal sealed class EndpointVirusTotalRetryExecuteInput
    {
        public int MaxRetryCount { get; set; }

        public int InitialRetry { get; set; }

        public int RetryDelayMilliseconds { get; set; }

        public Func<EndpointVirusTotalRetryAttemptResult> ExecuteAttempt { get; set; }

        public Func<Exception, bool> ShouldRetryException { get; set; }

        public Action<Exception, int> OnRetrying { get; set; }

        public Action<EndpointVirusTotalRetryAttemptResult> OnSuccess { get; set; }

        public Action<Exception> OnFailed { get; set; }

        public Func<bool> IsCancellationRequested { get; set; }

        public Func<int, Func<bool>, bool> Delay { get; set; }
    }

    internal sealed class EndpointVirusTotalRetryExecutionResult
    {
        public bool Completed { get; set; }

        public bool Failed { get; set; }

        public bool Cancelled { get; set; }

        public int AttemptCount { get; set; }
    }

    internal static class EndpointVirusTotalScanRetryExecutor
    {
        public static EndpointVirusTotalRetryExecutionResult Execute(EndpointVirusTotalRetryExecuteInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            if (input.ExecuteAttempt == null)
            {
                throw new ArgumentNullException("ExecuteAttempt");
            }

            EndpointVirusTotalRetryExecutionResult result = new EndpointVirusTotalRetryExecutionResult();
            int retry = input.InitialRetry;

            while (true)
            {
                if (IsCancellationRequested(input))
                {
                    result.Cancelled = true;
                    return result;
                }

                try
                {
                    result.AttemptCount++;
                    EndpointVirusTotalRetryAttemptResult attemptResult = input.ExecuteAttempt();
                    if (attemptResult == null)
                    {
                        throw new InvalidOperationException("ExecuteAttempt returned null result.");
                    }

                    input.OnSuccess?.Invoke(attemptResult);

                    result.Completed = true;
                    return result;
                }
                catch (Exception exception)
                {
                    bool shouldRetry = input.ShouldRetryException != null &&
                        input.ShouldRetryException(exception) &&
                        retry <= input.MaxRetryCount;

                    if (!shouldRetry)
                    {
                        if (!IsCancellationRequested(input))
                        {
                            input.OnFailed?.Invoke(exception);
                            result.Failed = true;
                        }
                        else
                        {
                            result.Cancelled = true;
                        }

                        return result;
                    }

                    if (!IsCancellationRequested(input))
                    {
                        input.OnRetrying?.Invoke(exception, retry);
                    }

                    retry++;

                    if (!Delay(input, IsCancellationRequested))
                    {
                        result.Cancelled = true;
                        return result;
                    }
                }
            }
        }

        public static string BuildLegacyRetryStatusMessage(Exception exception, int retry)
        {
            if (exception == null)
            {
                return string.Empty;
            }

            string statusMessage = exception.InnerException != null
                ? exception.InnerException.Message ?? exception.InnerException.ToString()
                : exception.ToString();

            if (retry > 0)
            {
                statusMessage += " [Retry " + retry + "]";
            }

            return statusMessage;
        }

        private static bool IsCancellationRequested(EndpointVirusTotalRetryExecuteInput input)
        {
            return input.IsCancellationRequested != null && input.IsCancellationRequested();
        }

        private static bool Delay(EndpointVirusTotalRetryExecuteInput input, Func<EndpointVirusTotalRetryExecuteInput, bool> isCancellationRequested)
        {
            Func<int, Func<bool>, bool> delay = input.Delay ?? DefaultDelay;

            int delayMs = input.RetryDelayMilliseconds;
            if (delayMs < 0)
            {
                delayMs = 0;
            }

            return delay(delayMs, () => isCancellationRequested(input));
        }

        private static bool DefaultDelay(int delayMilliseconds, Func<bool> isCancellationRequested)
        {
            if (delayMilliseconds <= 0)
            {
                return !isCancellationRequested();
            }

            int elapsed = 0;
            while (elapsed < delayMilliseconds)
            {
                if (isCancellationRequested())
                {
                    return false;
                }

                int chunk = Math.Min(100, delayMilliseconds - elapsed);
                Thread.Sleep(chunk);
                elapsed += chunk;
            }

            return !isCancellationRequested();
        }
    }
}
