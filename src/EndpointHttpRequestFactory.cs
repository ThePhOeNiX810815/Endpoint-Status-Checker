using System;
using System.Net;
using System.Net.Cache;

namespace EndpointChecker
{
    /// <summary>
    /// Builds the legacy HttpWebRequest used for endpoint status checks.
    /// </summary>
    /// <remarks>
    /// This factory intentionally keeps HttpWebRequest settings, headers, credentials, cookies,
    /// and the non-mutating removeUrlParameters behavior stable for compatibility.
    /// </remarks>
    internal static class EndpointHttpRequestFactory
    {
        private const string StatusNotAvailable = "N/A";

        /// <summary>
        /// Creates a configured request without applying user-defined endpoint headers.
        /// </summary>
        public static HttpWebRequest Create(
            EndpointDefinition endpoint,
            Uri endpointUri,
            int httpRequestTimeout,
            bool allowAutoRedirect,
            bool removeUrlParameters,
            string userAgent,
            CookieCollection cookies = null)
        {
            // The legacy Flurl RemoveQuery call did not assign its return value, so this flag is
            // intentionally non-mutating here to preserve observed request URI behavior.
            _ = removeUrlParameters;

            CookieContainer cookieContainer = new CookieContainer(300);
            if (cookies != null)
            {
                cookieContainer.Add(cookies);
            }

            cookieContainer.Add(new Cookie("viewed_cookie_policy", "yes", endpointUri.AbsolutePath, endpointUri.Host));
            cookieContainer.Add(new Cookie("cookielawinfo-checkbox-necessary", "yes", endpointUri.AbsolutePath, endpointUri.Host));
            cookieContainer.Add(new Cookie("cookielawinfo-checkbox-functional", "yes", endpointUri.AbsolutePath, endpointUri.Host));
            cookieContainer.Add(new Cookie("cookielawinfo-checkbox-performance", "yes", endpointUri.AbsolutePath, endpointUri.Host));
            cookieContainer.Add(new Cookie("cookielawinfo-checkbox-analytics", "yes", endpointUri.AbsolutePath, endpointUri.Host));
            cookieContainer.Add(new Cookie("cookielawinfo-checkbox-advertisement", "yes", endpointUri.AbsolutePath, endpointUri.Host));
            cookieContainer.Add(new Cookie("cookielawinfo-checkbox-others", "yes", endpointUri.AbsolutePath, endpointUri.Host));

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(endpointUri.AbsoluteUri);
            httpWebRequest.Method = WebRequestMethods.Http.Get;
            httpWebRequest.UserAgent = userAgent;
            httpWebRequest.Accept = @"text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7";
            httpWebRequest.Timeout = httpRequestTimeout;
            httpWebRequest.ReadWriteTimeout = httpRequestTimeout;
            httpWebRequest.AllowAutoRedirect = allowAutoRedirect;
            httpWebRequest.KeepAlive = true;
            httpWebRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
            httpWebRequest.CookieContainer = cookieContainer;
            httpWebRequest.AutomaticDecompression =
                DecompressionMethods.GZip |
                DecompressionMethods.Deflate |
                DecompressionMethods.None;
            httpWebRequest.ProtocolVersion = HttpVersion.Version11;
            httpWebRequest.MaximumAutomaticRedirections = 100;

            WebHeaderCollection requestHeadersCollection = new WebHeaderCollection
            {
                { "accept-language", @"*;*" },
                { "cache-control", "max-age=0" },
                { "dnt", "1" },
                { "upgrade-insecure-requests", "1" },
                { "Sec-Fetch-User", "?1" },
                { "Sec-Fetch-Site", "none" },
                { "Sec-Fetch-Mode", "navigate" },
                { "Sec-Fetch-Dest", "document" },
                { "Sec-CH-UA-Mobile", "?0" },
                { "Sec-CH-UA-Platform", "\"Windows\"" }
            };
            httpWebRequest.Headers.Add(requestHeadersCollection);

            httpWebRequest.PreAuthenticate = true;
            httpWebRequest.AuthenticationLevel = System.Net.Security.AuthenticationLevel.MutualAuthRequested;

            if (endpoint.LoginName != StatusNotAvailable &&
                !string.IsNullOrEmpty(endpoint.LoginName))
            {
                httpWebRequest.Credentials = new NetworkCredential(endpoint.LoginName, endpoint.LoginPass);
            }
            else
            {
                httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
            }

            return httpWebRequest;
        }
    }
}
