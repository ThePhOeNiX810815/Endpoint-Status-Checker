using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace EndpointChecker
{
    internal sealed class EndpointHttpSuccessInterpretInput
    {
        public Uri EndpointUri { get; set; }

        public Uri ResponseUri { get; set; }

        public int StatusCode { get; set; }

        public string StatusCodeName { get; set; }

        public string StatusDescription { get; set; }

        public string Server { get; set; }

        public bool AllowAutoRedirect { get; set; }

        public int AutoRedirectCount { get; set; }

        public bool AutoRedirectFollowed { get; set; }

        public string ContentTypeHeader { get; set; }

        public string ExpiresHeader { get; set; }

        public string ETagHeader { get; set; }

        public long ContentLength { get; set; }

        public string ContentLengthHeader { get; set; }

        public string StatusNotAvailable { get; set; }
    }

    internal sealed class EndpointHttpSuccessInterpretResult
    {
        public string Port { get; set; }

        public string Protocol { get; set; }

        public string ResponseCode { get; set; }

        public string ResponseMessage { get; set; }

        public string ServerId { get; set; }

        public string HttpAutoRedirects { get; set; }

        public string HttpContentType { get; set; }

        public string HttpExpires { get; set; }

        public string HttpEtag { get; set; }

        public long ContentLength { get; set; }
    }

    internal sealed class EndpointHttpHandledErrorInput
    {
        public int StatusCode { get; set; }

        public string StatusCodeName { get; set; }

        public string StatusDescription { get; set; }

        public string WebExceptionMessage { get; set; }

        public bool IsCloudflareProtected { get; set; }

        public string CloudflareRay { get; set; }
    }

    internal sealed class EndpointHttpHandledErrorResult
    {
        public string ResponseCode { get; set; }

        public string ResponseMessage { get; set; }
    }

    internal static class EndpointHttpResponseInterpreter
    {
        public static EndpointHttpSuccessInterpretResult InterpretSuccess(EndpointHttpSuccessInterpretInput input)
        {
            string responseCode = input.StatusCode.ToString();
            string responseMessage = !string.IsNullOrEmpty(input.StatusDescription)
                ? input.StatusDescription
                : input.StatusCodeName;

            if (input.AllowAutoRedirect)
            {
                if (input.EndpointUri != null && input.ResponseUri != null)
                {
                    if (!string.Equals(input.EndpointUri.AbsoluteUri, input.ResponseUri.AbsoluteUri, StringComparison.Ordinal) || input.AutoRedirectFollowed)
                    {
                        responseMessage += " (Redirected from \"" + input.EndpointUri.OriginalString + "\")";
                    }
                }
            }

            return new EndpointHttpSuccessInterpretResult
            {
                Port = input.ResponseUri.Port.ToString(),
                Protocol = input.ResponseUri.Scheme.ToUpperInvariant(),
                ResponseCode = responseCode,
                ResponseMessage = responseMessage,
                ServerId = SanitizeServerId(input.Server),
                HttpAutoRedirects = input.AllowAutoRedirect ? input.AutoRedirectCount.ToString() : null,
                HttpContentType = ParseContentType(input.ContentTypeHeader, input.StatusNotAvailable),
                HttpExpires = ParseHttpExpires(input.ExpiresHeader),
                HttpEtag = ParseEtag(input.ETagHeader),
                ContentLength = ParseContentLength(input.ContentLength, input.ContentLengthHeader),
            };
        }

        public static EndpointHttpHandledErrorResult InterpretHandledError(EndpointHttpHandledErrorInput input)
        {
            string responseCode = input.StatusCode.ToString();
            string responseMessage = EndpointHttpStatusMapper.BuildHandledWebExceptionMessage(
                input.StatusDescription,
                input.StatusCodeName,
                responseCode,
                input.WebExceptionMessage);

            if (input.IsCloudflareProtected)
            {
                responseMessage += " [Cloudflare Bot Protection" +
                    (!string.IsNullOrEmpty(input.CloudflareRay) ? " | CF-RAY: " + input.CloudflareRay : string.Empty) +
                    "]";
            }

            return new EndpointHttpHandledErrorResult
            {
                ResponseCode = responseCode,
                ResponseMessage = responseMessage,
            };
        }

        public static string FormatContentLength(long contentLength, string statusNotAvailable)
        {
            if (contentLength == -1)
            {
                return statusNotAvailable;
            }

            if (contentLength >= 1073741824)
            {
                return (contentLength / 1073741824d).ToString("0.00") + " GB";
            }

            if (contentLength >= 1048576)
            {
                return (contentLength / 1048576d).ToString("0.00") + " MB";
            }

            if (contentLength >= 1024)
            {
                return (contentLength / 1024d).ToString("0.00") + " kB";
            }

            return contentLength + " bytes";
        }

        private static string SanitizeServerId(string server)
        {
            if (string.IsNullOrEmpty(server))
            {
                return null;
            }

            return Regex.Replace(server, "<.*?>", string.Empty);
        }

        private static string ParseContentType(string valueString, string statusNotAvailable)
        {
            string contentType = statusNotAvailable;

            if (!string.IsNullOrEmpty(valueString))
            {
                string[] valueStringArray = valueString.Split(new[] { ' ', ';', ',' });

                foreach (string value in valueStringArray)
                {
                    if (value.ToLowerInvariant().Contains("/"))
                    {
                        contentType = value
                            .ToLowerInvariant()
                            .Replace("<", string.Empty)
                            .Replace(">", string.Empty)
                            .Replace("\"", string.Empty)
                            .Replace("'", string.Empty)
                            .Replace("\\", string.Empty)
                            .Trim();
                    }
                }
            }

            return contentType;
        }

        private static string ParseHttpExpires(string expiresHeader)
        {
            if (string.IsNullOrEmpty(expiresHeader))
            {
                return null;
            }

            DateTime expiresDateTime;
            if (!TryParseHttpDate(expiresHeader, out expiresDateTime) || expiresDateTime <= DateTime.MinValue)
            {
                return null;
            }

            return expiresDateTime.ToString("dd.MM.yyyy HH:mm");
        }

        private static string ParseEtag(string eTagHeader)
        {
            if (string.IsNullOrEmpty(eTagHeader))
            {
                return null;
            }

            return eTagHeader
                .ToString()
                .TrimStart()
                .TrimEnd()
                .TrimStart('"')
                .TrimEnd('"');
        }

        private static long ParseContentLength(long contentLength, string contentLengthHeader)
        {
            if (!string.IsNullOrEmpty(contentLengthHeader))
            {
                long.TryParse(contentLengthHeader, out contentLength);
            }

            return contentLength;
        }

        private static bool TryParseHttpDate(string httpDate, out DateTime parsedDate)
        {
            string[] formats =
            {
                "r",
                "dddd, dd-MMM-yy HH:mm:ss GMT",
                "ddd MMM  d HH:mm:ss yyyy"
            };

            return DateTime.TryParseExact(
                httpDate,
                formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out parsedDate);
        }
    }
}
