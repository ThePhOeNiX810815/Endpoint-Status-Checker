using System;

namespace EndpointChecker
{
    internal enum EndpointProtocolRoute
    {
        None = 0,
        Http = 1,
        Ftp = 2
    }

    internal static class EndpointProtocolRouteResolver
    {
        public static EndpointProtocolRoute Resolve(bool isProtocolValidation, bool cancellationPending, string protocol)
        {
            if (!isProtocolValidation || cancellationPending)
            {
                return EndpointProtocolRoute.None;
            }

            if (string.Equals(protocol, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(protocol, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
            {
                return EndpointProtocolRoute.Http;
            }

            if (string.Equals(protocol, Uri.UriSchemeFtp, StringComparison.OrdinalIgnoreCase))
            {
                return EndpointProtocolRoute.Ftp;
            }

            return EndpointProtocolRoute.None;
        }
    }
}