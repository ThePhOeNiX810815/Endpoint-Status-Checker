using System;

namespace EndpointChecker
{
    internal sealed class EndpointCloudflareBypassCheckResult
    {
        public bool HasBypassResult { get; set; }

        public bool BypassSuccess { get; set; }

        public int BypassStatusCode { get; set; }

        public string BypassStatusMessage { get; set; }

        public string BypassMethodUsed { get; set; }

        public string BypassExceptionMessage { get; set; }
    }

    internal sealed class EndpointCloudflareBypassExecuteInput
    {
        public bool ShouldAttemptBypass { get; set; }

        public string ExistingResponseCode { get; set; }

        public string ExistingResponseMessage { get; set; }

        public string EndpointAddress { get; set; }

        public Func<string, EndpointCloudflareBypassCheckResult> ExecuteBypassCheck { get; set; }
    }

    internal static class EndpointCloudflareBypassExecutor
    {
        public static EndpointCloudflareBypassInterpretResult Execute(EndpointCloudflareBypassExecuteInput input)
        {
            if (input == null)
            {
                return null;
            }

            if (!input.ShouldAttemptBypass)
            {
                return new EndpointCloudflareBypassInterpretResult
                {
                    ResponseCode = input.ExistingResponseCode,
                    ResponseMessage = input.ExistingResponseMessage,
                };
            }

            try
            {
                EndpointCloudflareBypassCheckResult bypassResult = input.ExecuteBypassCheck(input.EndpointAddress);

                return EndpointCloudflareBypassInterpreter.Interpret(
                    new EndpointCloudflareBypassInterpretInput
                    {
                        ExistingResponseCode = input.ExistingResponseCode,
                        ExistingResponseMessage = input.ExistingResponseMessage,
                        HasBypassResult = bypassResult != null && bypassResult.HasBypassResult,
                        BypassSuccess = bypassResult != null && bypassResult.BypassSuccess,
                        BypassStatusCode = bypassResult != null ? bypassResult.BypassStatusCode : 0,
                        BypassStatusMessage = bypassResult != null ? bypassResult.BypassStatusMessage : null,
                        BypassMethodUsed = bypassResult != null ? bypassResult.BypassMethodUsed : null,
                    });
            }
            catch (Exception bypassEx)
            {
                return EndpointCloudflareBypassInterpreter.Interpret(
                    new EndpointCloudflareBypassInterpretInput
                    {
                        ExistingResponseCode = input.ExistingResponseCode,
                        ExistingResponseMessage = input.ExistingResponseMessage,
                        BypassExceptionMessage = bypassEx.Message,
                    });
            }
        }
    }
}
