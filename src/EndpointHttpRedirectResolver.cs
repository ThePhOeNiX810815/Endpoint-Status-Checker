using System;

namespace EndpointChecker
{
    internal static class EndpointHttpRedirectResolver
    {
        public static bool TryResolveRedirectUri(
            bool allowAutoRedirect,
            int statusCode,
            string locationHeader,
            Uri responseUri,
            out Uri resolvedRedirectUri)
        {
            resolvedRedirectUri = null;

            if (!EndpointScanWorkflowRules.ShouldFollowManualRedirect(allowAutoRedirect, statusCode, locationHeader))
            {
                return false;
            }

            string locationHeaderValue = locationHeader;
            if (Uri.IsWellFormedUriString(locationHeaderValue, UriKind.Relative) && responseUri != null)
            {
                resolvedRedirectUri = new Uri(responseUri, locationHeaderValue);
                return true;
            }

            resolvedRedirectUri = new Uri(locationHeaderValue);
            return true;
        }
    }
}
