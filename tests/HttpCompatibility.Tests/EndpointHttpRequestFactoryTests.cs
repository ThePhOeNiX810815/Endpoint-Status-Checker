using System;
using System.Net;

namespace EndpointChecker
{
    internal static class EndpointHttpRequestFactoryTests
    {
        private static int failed;

        private static void Main()
        {
            Run("Factory preserves request method headers and browser-like settings", FactoryPreservesRequestMethodHeadersAndBrowserLikeSettings);
            Run("Factory preserves timeout and redirect settings", FactoryPreservesTimeoutAndRedirectSettings);
            Run("Factory preserves default credential behavior", FactoryPreservesDefaultCredentialBehavior);
            Run("Factory preserves specified credential behavior", FactoryPreservesSpecifiedCredentialBehavior);
            Run("Factory preserves legacy GDPR cookie injection", FactoryPreservesLegacyGdprCookieInjection);
            Run("Remove URL parameters flag remains non-mutating", RemoveUrlParametersFlagRemainsNonMutating);
            Run("Cloudflare classifier preserves CF-RAY detection", CloudflareClassifierPreservesCfRayDetection);
            Run("Cloudflare classifier preserves server detection", CloudflareClassifierPreservesServerDetection);
            Run("Cloudflare classifier ignores unrelated responses", CloudflareClassifierIgnoresUnrelatedResponses);
            Run("HTTP retry executor retries timeout exceptions until success", HttpRetryExecutorRetriesTimeoutExceptionsUntilSuccess);
            Run("HTTP retry executor stops retrying after max timeout retries", HttpRetryExecutorStopsRetryingAfterMaxTimeoutRetries);
            Run("HTTP retry executor does not retry non-timeout web exceptions", HttpRetryExecutorDoesNotRetryNonTimeoutWebExceptions);

            if (failed > 0)
            {
                Environment.Exit(1);
            }
        }

        private static void FactoryPreservesRequestMethodHeadersAndBrowserLikeSettings()
        {
            HttpWebRequest request = CreateRequest();

            AssertEqual(WebRequestMethods.Http.Get, request.Method);
            AssertEqual("EndpointCheckerTest/1.0", request.UserAgent);
            AssertEqual(@"text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7", request.Accept);
            AssertEqual(HttpVersion.Version11, request.ProtocolVersion);
            AssertEqual(true, request.KeepAlive);
            AssertEqual(100, request.MaximumAutomaticRedirections);
            AssertEqual(DecompressionMethods.GZip | DecompressionMethods.Deflate, request.AutomaticDecompression);
            AssertEqual(@"*;*", request.Headers["accept-language"]);
            AssertEqual("max-age=0", request.Headers["cache-control"]);
            AssertEqual("1", request.Headers["dnt"]);
            AssertEqual("1", request.Headers["upgrade-insecure-requests"]);
            AssertEqual("?1", request.Headers["Sec-Fetch-User"]);
            AssertEqual("none", request.Headers["Sec-Fetch-Site"]);
            AssertEqual("navigate", request.Headers["Sec-Fetch-Mode"]);
            AssertEqual("document", request.Headers["Sec-Fetch-Dest"]);
            AssertEqual("?0", request.Headers["Sec-CH-UA-Mobile"]);
            AssertEqual("\"Windows\"", request.Headers["Sec-CH-UA-Platform"]);
        }

        private static void FactoryPreservesTimeoutAndRedirectSettings()
        {
            HttpWebRequest request = CreateRequest(timeout: 12345, allowAutoRedirect: false);

            AssertEqual(12345, request.Timeout);
            AssertEqual(12345, request.ReadWriteTimeout);
            AssertEqual(false, request.AllowAutoRedirect);
        }

        private static void FactoryPreservesDefaultCredentialBehavior()
        {
            HttpWebRequest request = EndpointHttpRequestFactory.Create(
                new EndpointDefinition { LoginName = "N/A", LoginPass = "N/A" },
                new Uri("http://example.com/path"),
                1000,
                true,
                false,
                "EndpointCheckerTest/1.0");

            AssertEqual(CredentialCache.DefaultCredentials, request.Credentials);
            AssertEqual(true, request.PreAuthenticate);
            AssertEqual(System.Net.Security.AuthenticationLevel.MutualAuthRequested, request.AuthenticationLevel);
        }

        private static void FactoryPreservesSpecifiedCredentialBehavior()
        {
            HttpWebRequest request = EndpointHttpRequestFactory.Create(
                new EndpointDefinition { LoginName = "user", LoginPass = "pass" },
                new Uri("http://example.com/path"),
                1000,
                true,
                false,
                "EndpointCheckerTest/1.0");

            NetworkCredential credential = request.Credentials.GetCredential(new Uri("http://example.com/path"), "Basic");

            AssertEqual("user", credential.UserName);
            AssertEqual("pass", credential.Password);
        }

        private static void FactoryPreservesLegacyGdprCookieInjection()
        {
            Uri uri = new Uri("http://example.com/path");
            HttpWebRequest request = CreateRequest(uri: uri);
            CookieCollection cookies = request.CookieContainer.GetCookies(uri);

            AssertEqual("yes", cookies["viewed_cookie_policy"].Value);
            AssertEqual("yes", cookies["cookielawinfo-checkbox-necessary"].Value);
            AssertEqual("yes", cookies["cookielawinfo-checkbox-functional"].Value);
            AssertEqual("yes", cookies["cookielawinfo-checkbox-performance"].Value);
            AssertEqual("yes", cookies["cookielawinfo-checkbox-analytics"].Value);
            AssertEqual("yes", cookies["cookielawinfo-checkbox-advertisement"].Value);
            AssertEqual("yes", cookies["cookielawinfo-checkbox-others"].Value);
        }

        private static void RemoveUrlParametersFlagRemainsNonMutating()
        {
            HttpWebRequest request = CreateRequest(uri: new Uri("http://example.com/path?a=1"), removeUrlParameters: true);

            AssertEqual("http://example.com/path?a=1", request.RequestUri.AbsoluteUri);
        }

        private static HttpWebRequest CreateRequest(
            Uri uri = null,
            int timeout = 1000,
            bool allowAutoRedirect = true,
            bool removeUrlParameters = false)
        {
            return EndpointHttpRequestFactory.Create(
                new EndpointDefinition { LoginName = "N/A", LoginPass = "N/A" },
                uri ?? new Uri("http://example.com/path"),
                timeout,
                allowAutoRedirect,
                removeUrlParameters,
                "EndpointCheckerTest/1.0");
        }

        private static void CloudflareClassifierPreservesCfRayDetection()
        {
            WebHeaderCollection headers = new WebHeaderCollection();
            headers["CF-RAY"] = "abc123";

            AssertEqual(true, EndpointHttpResponseClassifier.IsCloudflareProtected(headers, null));
        }

        private static void CloudflareClassifierPreservesServerDetection()
        {
            AssertEqual(true, EndpointHttpResponseClassifier.IsCloudflareProtected(new WebHeaderCollection(), "cloudflare"));
            AssertEqual(true, EndpointHttpResponseClassifier.IsCloudflareProtected(new WebHeaderCollection(), "CloudFlare"));
        }

        private static void CloudflareClassifierIgnoresUnrelatedResponses()
        {
            AssertEqual(false, EndpointHttpResponseClassifier.IsCloudflareProtected(null, null));
            AssertEqual(false, EndpointHttpResponseClassifier.IsCloudflareProtected(new WebHeaderCollection(), "nginx"));
        }

        private static void HttpRetryExecutorRetriesTimeoutExceptionsUntilSuccess()
        {
            int attempts = 0;

            HttpWebResponse response = EndpointHttpRetryExecutor.ExecuteWithRetry(
                () =>
                {
                    attempts++;
                    if (attempts < 3)
                    {
                        throw new WebException("timeout", WebExceptionStatus.Timeout);
                    }

                    return null;
                },
                maxRetryCount: 3);

            AssertEqual(3, attempts);
            AssertEqual(null, response);
        }

        private static void HttpRetryExecutorStopsRetryingAfterMaxTimeoutRetries()
        {
            int attempts = 0;

            try
            {
                EndpointHttpRetryExecutor.ExecuteWithRetry(
                    () =>
                    {
                        attempts++;
                        throw new WebException("timeout", WebExceptionStatus.Timeout);
                    },
                    maxRetryCount: 2);

                throw new InvalidOperationException("Expected timeout exception was not thrown.");
            }
            catch (WebException webException)
            {
                AssertEqual(WebExceptionStatus.Timeout, webException.Status);
            }

            AssertEqual(3, attempts);
        }

        private static void HttpRetryExecutorDoesNotRetryNonTimeoutWebExceptions()
        {
            int attempts = 0;

            try
            {
                EndpointHttpRetryExecutor.ExecuteWithRetry(
                    () =>
                    {
                        attempts++;
                        throw new WebException("connect failure", WebExceptionStatus.ConnectFailure);
                    },
                    maxRetryCount: 5);

                throw new InvalidOperationException("Expected non-timeout web exception was not thrown.");
            }
            catch (WebException webException)
            {
                AssertEqual(WebExceptionStatus.ConnectFailure, webException.Status);
            }

            AssertEqual(1, attempts);
        }

        private static void Run(string name, Action test)
        {
            try
            {
                test();
                Console.WriteLine("PASS " + name);
            }
            catch (Exception exception)
            {
                failed++;
                Console.WriteLine("FAIL " + name + ": " + exception.Message);
            }
        }

        private static void AssertEqual<T>(T expected, T actual)
        {
            if (!object.Equals(expected, actual))
            {
                throw new InvalidOperationException("Expected <" + expected + "> but was <" + actual + ">.");
            }
        }
    }

    public class EndpointDefinition
    {
        public string LoginName { get; set; }
        public string LoginPass { get; set; }
    }
}
