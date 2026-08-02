using System;
using System.Net;

namespace EndpointChecker
{
    internal static class EndpointHttpResponseClassifier
    {
        public static bool IsCloudflareProtected(WebHeaderCollection headers, string server)
        {
            bool hasCfRay = headers != null && !string.IsNullOrEmpty(headers["CF-RAY"]);
            bool hasCfServer = !string.IsNullOrEmpty(server) &&
                               server.IndexOf("cloudflare", StringComparison.OrdinalIgnoreCase) >= 0;

            return hasCfRay || hasCfServer;
        }
    }
}
