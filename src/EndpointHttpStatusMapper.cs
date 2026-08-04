using System;
using System.Net;

namespace EndpointChecker
{
    internal static class EndpointHttpStatusMapper
    {
        public static string BuildHandledWebExceptionMessage(
            string statusDescription,
            string statusCodeName,
            string responseCode,
            string webExceptionMessage)
        {
            string responseMessage = !string.IsNullOrEmpty(statusDescription)
                ? statusDescription
                : statusCodeName;

            if (!string.IsNullOrEmpty(webExceptionMessage) &&
                !string.IsNullOrEmpty(responseCode) &&
                !webExceptionMessage.Contains(responseCode))
            {
                responseMessage += " -> " + webExceptionMessage;
            }

            return responseMessage;
        }

        public static string BuildTransportWebExceptionMessage(WebException webException)
        {
            string responseMessage = webException.Status + " -> " + webException.Message;

            Exception innerException = webException.InnerException;
            while (innerException != null)
            {
                if (!string.IsNullOrEmpty(innerException.Message) &&
                    !responseMessage.Contains(innerException.Message))
                {
                    responseMessage += " -> " + innerException.Message;
                }

                innerException = innerException.InnerException;
            }

            return responseMessage;
        }

        public static string BuildGenericExceptionMessage(Exception exception)
        {
            string responseMessage = exception.GetType().Name.Replace("Exception", string.Empty) + " -> " + exception.Message;

            if (exception.InnerException != null &&
                !string.IsNullOrEmpty(exception.InnerException.Message) &&
                !responseMessage.Contains(exception.InnerException.Message))
            {
                responseMessage += " -> " + exception.InnerException.Message;
            }

            return responseMessage;
        }
    }
}