using System;
using System.Collections.Generic;
using System.Drawing;

namespace EndpointChecker
{
    internal sealed class EndpointDefinitionParser
    {
        private const string StatusNotAvailable = "N/A";
        private const string ResponseMessageNotCheckedYet = "Not Checked Yet";

        private readonly List<string> endpointNames = new List<string>();

        public EndpointDefinitionParser(IEnumerable<string> existingEndpointNames = null)
        {
            if (existingEndpointNames == null)
            {
                return;
            }

            foreach (string endpointName in existingEndpointNames)
            {
                endpointNames.Add(endpointName);
            }
        }

        public EndpointDefinitionParseResult ParseLine(string rawLine, int lineNumber)
        {
            string line = rawLine == null ? string.Empty : rawLine.Trim();

            if (string.IsNullOrEmpty(line) ||
                line == "|" ||
                line.StartsWith("#"))
            {
                return EndpointDefinitionParseResult.Ignored(lineNumber);
            }

            EndpointDefinition endpointDefinition = CreateDefaultEndpointDefinition(line);
            EndpointDefinitionParseError invalidUrlError = ValidateAndNormalizeEndpoint(endpointDefinition, lineNumber);
            EndpointDefinitionParseError duplicateError = null;

            foreach (string endpointName in endpointNames)
            {
                if (endpointName == endpointDefinition.Name)
                {
                    duplicateError = EndpointDefinitionParseError.Duplicate(lineNumber, endpointDefinition);
                }
            }

            if (duplicateError != null)
            {
                return EndpointDefinitionParseResult.Duplicate(endpointDefinition, duplicateError, invalidUrlError);
            }

            if (invalidUrlError != null)
            {
                return EndpointDefinitionParseResult.Invalid(endpointDefinition, invalidUrlError);
            }

            endpointNames.Add(endpointDefinition.Name);
            return EndpointDefinitionParseResult.Valid(endpointDefinition);
        }

        internal static EndpointDefinition CreateDefaultEndpointDefinition(string line)
        {
            string[] lineParts = line.Split(new char[] { '|' }, 2);
            string lineAddress = lineParts.Length > 1 ? lineParts[1].Trim() : lineParts[0].Trim();
            string lineName = lineParts[0].Trim();

            return new EndpointDefinition()
            {
                Name = lineName,
                Protocol = StatusNotAvailable,
                Port = StatusNotAvailable,
                Address = lineAddress,
                ResponseAddress = lineAddress,
                IPAddress = new string[] { StatusNotAvailable },
                ResponseTime = StatusNotAvailable,
                ResponseCode = StatusNotAvailable,
                ResponseMessage = ResponseMessageNotCheckedYet,
                LastSeenOnline = StatusNotAvailable,
                PingRoundtripTime = StatusNotAvailable,
                ServerID = StatusNotAvailable,
                LoginName = StatusNotAvailable,
                LoginPass = StatusNotAvailable,
                NetworkShare = new string[] { StatusNotAvailable },
                DNSName = new string[] { StatusNotAvailable },
                HTMLMetaInfo = new PropertyItems() { PropertyItem = new List<Property>() },
                HTTPautoRedirects = StatusNotAvailable,
                HTTPcontentType = StatusNotAvailable,
                HTTPencoding = null,
                HTMLdefaultStreamEncoding = null,
                HTMLencoding = null,
                HTMLTitle = StatusNotAvailable,
                HTMLAuthor = StatusNotAvailable,
                HTMLPageLinks = new PropertyItems() { PropertyItem = new List<Property>() },
                HTMLDescription = StatusNotAvailable,
                HTMLContentLanguage = StatusNotAvailable,
                HTMLThemeColor = Color.Empty,
                HTTPcontentLength = StatusNotAvailable,
                HTTPexpires = StatusNotAvailable,
                HTTPetag = StatusNotAvailable,
                HTTPRequestHeaders = new PropertyItems() { PropertyItem = new List<Property>() },
                HTTPResponseHeaders = new PropertyItems() { PropertyItem = new List<Property>() },
                MACAddress = new string[] { StatusNotAvailable },
                FTPBannerMessage = StatusNotAvailable,
                FTPWelcomeMessage = StatusNotAvailable,
                FTPExitMessage = StatusNotAvailable,
                FTPStatusDescription = StatusNotAvailable,
                SSLCertificateProperties = new PropertyItems() { PropertyItem = new List<Property>() }
            };
        }

        private static EndpointDefinitionParseError ValidateAndNormalizeEndpoint(EndpointDefinition endpointDefinition, int lineNumber)
        {
            if (!endpointDefinition.Address.ToLower().Contains(Uri.SchemeDelimiter))
            {
                return EndpointDefinitionParseError.MissingProtocol(lineNumber, endpointDefinition);
            }

            ExtractCredentials(endpointDefinition);

            try
            {
                if (!Uri.IsWellFormedUriString(endpointDefinition.Address, UriKind.Absolute))
                {
                    string escaped = Uri.EscapeUriString(endpointDefinition.Address);
                    if (Uri.IsWellFormedUriString(escaped, UriKind.Absolute))
                    {
                        endpointDefinition.Address = escaped;
                        endpointDefinition.ResponseAddress = escaped;
                    }
                }

                Uri endpointURI = new Uri(endpointDefinition.Address, UriKind.Absolute);

                endpointDefinition.Port = endpointURI.Port.ToString();
                endpointDefinition.Protocol = endpointURI.Scheme.ToUpper();

                if (endpointDefinition.Protocol != Uri.UriSchemeHttp.ToUpper() &&
                    endpointDefinition.Protocol != Uri.UriSchemeHttps.ToUpper() &&
                    endpointDefinition.Protocol != Uri.UriSchemeFtp.ToUpper())
                {
                    return EndpointDefinitionParseError.UnsupportedProtocol(lineNumber, endpointDefinition);
                }
            }
            catch (Exception exception)
            {
                return EndpointDefinitionParseError.InvalidFormat(lineNumber, endpointDefinition, exception.Message);
            }

            return null;
        }

        private static void ExtractCredentials(EndpointDefinition endpointDefinition)
        {
            if (!endpointDefinition.Address.Contains("@"))
            {
                return;
            }

            string[] credentials = endpointDefinition.Address.Split('@')[0].Split('/')[2].Split(':');
            if (credentials.Length != 2)
            {
                return;
            }

            endpointDefinition.LoginName = credentials[0];
            endpointDefinition.LoginPass = credentials[1];

            endpointDefinition.Name = endpointDefinition.Name
                .Replace(
                endpointDefinition.LoginName + ":" +
                endpointDefinition.LoginPass + "@", string.Empty) +
                " [as '" + endpointDefinition.LoginName + "']";

            endpointDefinition.Address = endpointDefinition.Address
                .Replace(endpointDefinition.LoginName + ":" +
                endpointDefinition.LoginPass + "@", string.Empty);
        }
    }

    internal sealed class EndpointDefinitionParseResult
    {
        private EndpointDefinitionParseResult(
            EndpointDefinitionParseStatus status,
            EndpointDefinition endpointDefinition,
            EndpointDefinitionParseError invalidUrlError,
            EndpointDefinitionParseError duplicateError,
            int lineNumber)
        {
            Status = status;
            EndpointDefinition = endpointDefinition;
            InvalidUrlError = invalidUrlError;
            DuplicateError = duplicateError;
            LineNumber = lineNumber;
        }

        public EndpointDefinitionParseStatus Status { get; }
        public EndpointDefinition EndpointDefinition { get; }
        public EndpointDefinitionParseError Error => DuplicateError ?? InvalidUrlError;
        public EndpointDefinitionParseError InvalidUrlError { get; }
        public EndpointDefinitionParseError DuplicateError { get; }
        public int LineNumber { get; }
        public bool IsIgnored => Status == EndpointDefinitionParseStatus.Ignored;
        public bool IsValid => Status == EndpointDefinitionParseStatus.Valid;

        public static EndpointDefinitionParseResult Ignored(int lineNumber)
        {
            return new EndpointDefinitionParseResult(EndpointDefinitionParseStatus.Ignored, null, null, null, lineNumber);
        }

        public static EndpointDefinitionParseResult Valid(EndpointDefinition endpointDefinition)
        {
            return new EndpointDefinitionParseResult(EndpointDefinitionParseStatus.Valid, endpointDefinition, null, null, 0);
        }

        public static EndpointDefinitionParseResult Invalid(EndpointDefinition endpointDefinition, EndpointDefinitionParseError error)
        {
            return new EndpointDefinitionParseResult(EndpointDefinitionParseStatus.InvalidUrl, endpointDefinition, error, null, error.LineNumber);
        }

        public static EndpointDefinitionParseResult Duplicate(
            EndpointDefinition endpointDefinition,
            EndpointDefinitionParseError duplicateError,
            EndpointDefinitionParseError invalidUrlError)
        {
            return new EndpointDefinitionParseResult(EndpointDefinitionParseStatus.Duplicate, endpointDefinition, invalidUrlError, duplicateError, duplicateError.LineNumber);
        }
    }

    internal enum EndpointDefinitionParseStatus
    {
        Ignored,
        Valid,
        InvalidUrl,
        Duplicate
    }

    internal sealed class EndpointDefinitionParseError
    {
        private EndpointDefinitionParseError(
            EndpointDefinitionParseErrorKind kind,
            int lineNumber,
            string endpointName,
            string endpointAddress,
            string detail)
        {
            Kind = kind;
            LineNumber = lineNumber;
            EndpointName = endpointName;
            EndpointAddress = endpointAddress;
            Detail = detail;
        }

        public EndpointDefinitionParseErrorKind Kind { get; }
        public int LineNumber { get; }
        public string EndpointName { get; }
        public string EndpointAddress { get; }
        public string Detail { get; }

        public static EndpointDefinitionParseError MissingProtocol(int lineNumber, EndpointDefinition endpointDefinition)
        {
            return new EndpointDefinitionParseError(
                EndpointDefinitionParseErrorKind.MissingProtocol,
                lineNumber,
                endpointDefinition.Name,
                endpointDefinition.Address,
                "Missing protocol prefix");
        }

        public static EndpointDefinitionParseError UnsupportedProtocol(int lineNumber, EndpointDefinition endpointDefinition)
        {
            return new EndpointDefinitionParseError(
                EndpointDefinitionParseErrorKind.UnsupportedProtocol,
                lineNumber,
                endpointDefinition.Name,
                endpointDefinition.Address,
                "Unsupported protocol type: " + endpointDefinition.Protocol);
        }

        public static EndpointDefinitionParseError InvalidFormat(int lineNumber, EndpointDefinition endpointDefinition, string exceptionMessage)
        {
            return new EndpointDefinitionParseError(
                EndpointDefinitionParseErrorKind.InvalidFormat,
                lineNumber,
                endpointDefinition.Name,
                endpointDefinition.Address,
                "URL in invalid format" + Environment.NewLine + exceptionMessage);
        }

        public static EndpointDefinitionParseError Duplicate(int lineNumber, EndpointDefinition endpointDefinition)
        {
            return new EndpointDefinitionParseError(
                EndpointDefinitionParseErrorKind.DuplicateName,
                lineNumber,
                endpointDefinition.Name,
                endpointDefinition.Address,
                null);
        }

        public string ToInvalidUrlDisplayText()
        {
            string message =
                "Line:  " + LineNumber +
                Environment.NewLine +
                "Endpoint Name:  " + EndpointName +
                Environment.NewLine +
                "Endpoint Address:  " + EndpointAddress +
                Environment.NewLine +
                Detail +
                Environment.NewLine;

            if (Kind == EndpointDefinitionParseErrorKind.MissingProtocol ||
                Kind == EndpointDefinitionParseErrorKind.UnsupportedProtocol)
            {
                message +=
                    Uri.UriSchemeHttp.ToUpper() + ", " +
                    Uri.UriSchemeHttps.ToUpper() + " and " +
                    Uri.UriSchemeFtp.ToUpper() + " protocols are supported" +
                    Environment.NewLine;
            }

            return message + Environment.NewLine;
        }

        public string ToDuplicateDisplayText()
        {
            return
                "Line:  " + LineNumber +
                Environment.NewLine +
                "Endpoint Name:  " + EndpointName +
                Environment.NewLine +
                "Endpoint Address:  " + EndpointAddress +
                Environment.NewLine +
                Environment.NewLine;
        }
    }

    internal enum EndpointDefinitionParseErrorKind
    {
        MissingProtocol,
        UnsupportedProtocol,
        InvalidFormat,
        DuplicateName
    }
}
