using System.Collections.Generic;

namespace EndpointChecker
{
    internal static class EndpointFaviconLookupPlanBuilder
    {
        public static string[] BuildRequestUrls(string websiteUrl, string fallbackGoogleResolveUrl)
        {
            List<string> urls = new List<string>();

            if (string.IsNullOrEmpty(websiteUrl))
            {
                return urls.ToArray();
            }

            urls.Add(websiteUrl + "/favicon.ico");

            if (!string.IsNullOrEmpty(fallbackGoogleResolveUrl) &&
                !websiteUrl.Contains(fallbackGoogleResolveUrl))
            {
                urls.Add(fallbackGoogleResolveUrl + websiteUrl + "/favicon.ico");
            }

            return urls.ToArray();
        }
    }
}
