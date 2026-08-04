namespace EndpointChecker
{
    internal sealed class EndpointCloudflareBypassInterpretInput
    {
        public string ExistingResponseCode { get; set; }

        public string ExistingResponseMessage { get; set; }

        public bool HasBypassResult { get; set; }

        public bool BypassSuccess { get; set; }

        public int BypassStatusCode { get; set; }

        public string BypassStatusMessage { get; set; }

        public string BypassMethodUsed { get; set; }

        public string BypassExceptionMessage { get; set; }
    }

    internal sealed class EndpointCloudflareBypassInterpretResult
    {
        public string ResponseCode { get; set; }

        public string ResponseMessage { get; set; }
    }

    internal static class EndpointCloudflareBypassInterpreter
    {
        public static EndpointCloudflareBypassInterpretResult Interpret(EndpointCloudflareBypassInterpretInput input)
        {
            string responseCode = input.ExistingResponseCode;
            string responseMessage = input.ExistingResponseMessage;

            if (input.HasBypassResult)
            {
                if (input.BypassSuccess)
                {
                    responseCode = input.BypassStatusCode.ToString();
                    responseMessage = input.BypassStatusMessage;
                }
                else
                {
                    responseMessage +=
                        " | Bypass(" + input.BypassMethodUsed + "): " +
                        input.BypassStatusMessage;
                }
            }

            if (!string.IsNullOrEmpty(input.BypassExceptionMessage))
            {
                responseMessage += " | Bypass error: " + input.BypassExceptionMessage;
            }

            return new EndpointCloudflareBypassInterpretResult
            {
                ResponseCode = responseCode,
                ResponseMessage = responseMessage,
            };
        }
    }
}
