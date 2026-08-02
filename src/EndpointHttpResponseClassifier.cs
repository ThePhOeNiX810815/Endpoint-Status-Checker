using System;
using System.Net;

namespace EndpointChecker
{
    /// <summary>
    /// Classifies HTTP response metadata that affects endpoint status compatibility behavior.
    /// </summary>
    internal static class EndpointHttpResponseClassifier
    {
        /// <summary>
        /// Preserves Cloudflare detection based on either CF-RAY or a cloudflare server value.
        /// </summary>
        public static bool IsCloudflareProtected(WebHeaderCollection headers, string server)
        {
            bool hasCfRay = headers != null && !string.IsNullOrEmpty(headers["CF-RAY"]);
            bool hasCfServer = !string.IsNullOrEmpty(server) &&
                               server.IndexOf("cloudflare", StringComparison.OrdinalIgnoreCase) >= 0;

            return hasCfRay || hasCfServer;
        }
    }
}
