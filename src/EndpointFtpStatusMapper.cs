using System;

namespace EndpointChecker
{
    internal sealed class EndpointFtpStatusMapInput
    {
        public string StatusError { get; set; }

        public string StatusNotAvailable { get; set; }

        public int? FtpStatusCode { get; set; }

        public string FtpStatusDescription { get; set; }

        public string FtpBannerMessage { get; set; }

        public string FtpWelcomeMessage { get; set; }

        public string FtpExitMessage { get; set; }

        public string WebExceptionStatus { get; set; }

        public string WebExceptionMessage { get; set; }

        public string WebExceptionInnerMessage { get; set; }
    }

    internal sealed class EndpointFtpStatusMapResult
    {
        public string ResponseCode { get; set; }

        public string ResponseMessage { get; set; }

        public string BannerMessage { get; set; }

        public string WelcomeMessage { get; set; }

        public string ExitMessage { get; set; }

        public string StatusDescription { get; set; }
    }

    internal static class EndpointFtpStatusMapper
    {
        public static EndpointFtpStatusMapResult Map(EndpointFtpStatusMapInput input)
        {
            EndpointFtpStatusMapResult result = new EndpointFtpStatusMapResult
            {
                ResponseCode = input.StatusError,
                ResponseMessage = input.StatusNotAvailable
            };

            if (input.FtpStatusCode.HasValue && input.FtpStatusCode.Value != 0)
            {
                result.ResponseCode = input.FtpStatusCode.Value.ToString();

                if (!string.IsNullOrEmpty(input.FtpStatusDescription) &&
                    !result.ResponseCode.StartsWith("2", StringComparison.Ordinal))
                {
                    result.ResponseMessage = SanitizeFtpStatusText(input.FtpStatusDescription, result.ResponseCode);
                }
                else if (!string.IsNullOrEmpty(input.FtpBannerMessage) &&
                         input.FtpBannerMessage.StartsWith("220", StringComparison.Ordinal))
                {
                    result.ResponseCode = "220";
                    result.ResponseMessage = SanitizeFtpStatusText(input.FtpBannerMessage, result.ResponseCode);
                }
                else if (!string.IsNullOrEmpty(input.FtpWelcomeMessage) &&
                         input.FtpWelcomeMessage.StartsWith("230", StringComparison.Ordinal))
                {
                    result.ResponseCode = "230";
                    result.ResponseMessage = SanitizeFtpStatusText(input.FtpWelcomeMessage, result.ResponseCode);
                }
                else if (!string.IsNullOrEmpty(input.FtpStatusDescription))
                {
                    result.ResponseMessage = SanitizeFtpStatusText(input.FtpStatusDescription, result.ResponseCode);
                }

                if (!string.IsNullOrEmpty(input.FtpBannerMessage))
                {
                    result.BannerMessage = input.FtpBannerMessage.Trim();
                }

                if (!string.IsNullOrEmpty(input.FtpWelcomeMessage))
                {
                    result.WelcomeMessage = input.FtpWelcomeMessage.Trim();
                }

                if (!string.IsNullOrEmpty(input.FtpExitMessage))
                {
                    result.ExitMessage = input.FtpExitMessage.Trim();
                }

                if (!string.IsNullOrEmpty(input.FtpStatusDescription))
                {
                    result.StatusDescription = input.FtpStatusDescription.Trim();
                }
            }
            else if (!string.IsNullOrEmpty(input.WebExceptionStatus))
            {
                result.ResponseMessage = input.WebExceptionStatus;

                if (!string.IsNullOrEmpty(input.WebExceptionMessage))
                {
                    result.ResponseMessage += " -> " + input.WebExceptionMessage;
                }

                if (!string.IsNullOrEmpty(input.WebExceptionInnerMessage) &&
                    !result.ResponseMessage.Contains(input.WebExceptionInnerMessage, StringComparison.Ordinal))
                {
                    result.ResponseMessage += " -> " + input.WebExceptionInnerMessage;
                }
            }

            return result;
        }

        private static string SanitizeFtpStatusText(string statusText, string statusCode)
        {
            return statusText
                .Replace(statusCode, string.Empty)
                .TrimStart('-')
                .Trim();
        }
    }
}
