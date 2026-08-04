using System;

namespace EndpointChecker
{
    internal sealed class EndpointScanFinalizeInput
    {
        public string EndpointAddress { get; set; }

        public string ResponseAddress { get; set; }

        public string DurationTime { get; set; }

        public string ResponseCode { get; set; }

        public string ResponseMessage { get; set; }

        public string PingRoundtripTime { get; set; }

        public string LastSeenOnline { get; set; }

        public bool CancellationPending { get; set; }

        public bool IsProtocolValidation { get; set; }

        public bool IsPingValidation { get; set; }

        public string StatusError { get; set; }

        public string StatusNotAvailable { get; set; }

        public string TerminatedMessage { get; set; }

        public string LastSeenNow { get; set; }
    }

    internal sealed class EndpointScanFinalizeOutput
    {
        public string Address { get; set; }

        public string ResponseAddress { get; set; }

        public string ResponseTime { get; set; }

        public string ResponseCode { get; set; }

        public string ResponseMessage { get; set; }

        public string LastSeenOnline { get; set; }
    }

    internal static class EndpointScanTerminalFinalizer
    {
        public static EndpointScanFinalizeOutput Finalize(EndpointScanFinalizeInput input)
        {
            EndpointScanFinalizeOutput output = new EndpointScanFinalizeOutput
            {
                Address = input.EndpointAddress,
                ResponseAddress = input.ResponseAddress,
                ResponseTime = input.DurationTime,
                ResponseCode = input.ResponseCode,
                ResponseMessage = input.ResponseMessage,
                LastSeenOnline = input.LastSeenOnline
            };

            if (EndpointScanWorkflowRules.ShouldMarkTerminated(input.CancellationPending))
            {
                output.ResponseCode = input.StatusNotAvailable;
                output.ResponseMessage = input.TerminatedMessage;
                return output;
            }

            if (EndpointScanWorkflowRules.ShouldUpdateLastSeenOnline(
                input.IsProtocolValidation,
                output.ResponseCode,
                input.IsPingValidation,
                input.PingRoundtripTime,
                input.StatusError,
                input.StatusNotAvailable))
            {
                output.LastSeenOnline = input.LastSeenNow;
            }

            return output;
        }
    }
}
