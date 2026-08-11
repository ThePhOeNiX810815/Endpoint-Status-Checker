using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;

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
            Run("HTTP handled web exception mapping preserves status description and message suffix rules", HttpHandledWebExceptionMappingPreservesStatusDescriptionAndMessageSuffixRules);
            Run("HTTP transport web exception mapping preserves inner exception chain", HttpTransportWebExceptionMappingPreservesInnerExceptionChain);
            Run("HTTP generic exception mapping preserves type and first inner message", HttpGenericExceptionMappingPreservesTypeAndFirstInnerMessage);
            Run("HTTP response interpreter preserves success metadata mapping", HttpResponseInterpreterPreservesSuccessMetadataMapping);
            Run("HTTP response interpreter preserves handled error and Cloudflare note mapping", HttpResponseInterpreterPreservesHandledErrorAndCloudflareNoteMapping);
            Run("HTTP response interpreter preserves content-length formatting", HttpResponseInterpreterPreservesContentLengthFormatting);
            Run("Cloudflare bypass interpreter preserves successful bypass override mapping", CloudflareBypassInterpreterPreservesSuccessfulBypassOverrideMapping);
            Run("Cloudflare bypass interpreter preserves failed bypass append mapping", CloudflareBypassInterpreterPreservesFailedBypassAppendMapping);
            Run("Cloudflare bypass interpreter preserves bypass exception append mapping", CloudflareBypassInterpreterPreservesBypassExceptionAppendMapping);
            Run("Cloudflare bypass executor preserves no-attempt behavior", CloudflareBypassExecutorPreservesNoAttemptBehavior);
            Run("Cloudflare bypass executor preserves success mapping behavior", CloudflareBypassExecutorPreservesSuccessMappingBehavior);
            Run("Cloudflare bypass executor preserves exception append behavior", CloudflareBypassExecutorPreservesExceptionAppendBehavior);
            Run("HTTP header collector preserves null and empty handling", HttpHeaderCollectorPreservesNullAndEmptyHandling);
            Run("HTTP header collector preserves request header name and value mapping", HttpHeaderCollectorPreservesHeaderNameAndValueMapping);
            Run("HTTP response body processor preserves read trigger conditions", HttpResponseBodyProcessorPreservesReadTriggerConditions);
            Run("HTTP response body processor preserves max byte cap behavior", HttpResponseBodyProcessorPreservesMaxByteCapBehavior);
            Run("HTTP response body processor preserves html meta gating", HttpResponseBodyProcessorPreservesHtmlMetaGating);
            Run("HTTP response body processor preserves encoding fallback gating", HttpResponseBodyProcessorPreservesEncodingFallbackGating);
            Run("HTTP html metadata resolver preserves title meta links and language extraction", HttpHtmlMetadataResolverPreservesTitleMetaLinksAndLanguageExtraction);
            Run("HTTP html metadata resolver preserves existing html encoding and theme parsing fallback", HttpHtmlMetadataResolverPreservesExistingHtmlEncodingAndThemeParsingFallback);

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
            AssertEqual(false, request.KeepAlive);
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

        private static void HttpHandledWebExceptionMappingPreservesStatusDescriptionAndMessageSuffixRules()
        {
            string withDescription = EndpointHttpStatusMapper.BuildHandledWebExceptionMessage(
                "Forbidden",
                "Forbidden",
                "403",
                "ProtocolError: request failed");

            AssertEqual("Forbidden -> ProtocolError: request failed", withDescription);

            string withoutDescription = EndpointHttpStatusMapper.BuildHandledWebExceptionMessage(
                string.Empty,
                "Forbidden",
                "403",
                "Remote server returned status 403");

            AssertEqual("Forbidden", withoutDescription);
        }

        private static void HttpTransportWebExceptionMappingPreservesInnerExceptionChain()
        {
            Exception root = new InvalidOperationException("root-cause");
            Exception middle = new ApplicationException("mid-layer", root);
            WebException transport = new WebException("transport-failure", middle, WebExceptionStatus.ConnectFailure, null);

            string message = EndpointHttpStatusMapper.BuildTransportWebExceptionMessage(transport);

            AssertEqual("ConnectFailure -> transport-failure -> mid-layer -> root-cause", message);
        }

        private static void HttpGenericExceptionMappingPreservesTypeAndFirstInnerMessage()
        {
            Exception exception = new InvalidOperationException("outer", new ApplicationException("inner"));
            string message = EndpointHttpStatusMapper.BuildGenericExceptionMessage(exception);

            AssertEqual("InvalidOperation -> outer -> inner", message);
        }

        private static void HttpResponseInterpreterPreservesSuccessMetadataMapping()
        {
            EndpointHttpSuccessInterpretResult result = EndpointHttpResponseInterpreter.InterpretSuccess(
                new EndpointHttpSuccessInterpretInput
                {
                    EndpointUri = new Uri("https://example.test/source"),
                    ResponseUri = new Uri("https://example.test/target"),
                    StatusCode = 200,
                    StatusCodeName = "OK",
                    StatusDescription = "Healthy",
                    Server = "nginx<private>",
                    AllowAutoRedirect = true,
                    AutoRedirectCount = 2,
                    AutoRedirectFollowed = true,
                    ContentTypeHeader = "text/html; charset=utf-8",
                    ExpiresHeader = "Wed, 21 Oct 2015 07:28:00 GMT",
                    ETagHeader = "\"abc123\"",
                    ContentLength = -1,
                    ContentLengthHeader = "321",
                    StatusNotAvailable = "N/A",
                });

            AssertEqual("443", result.Port);
            AssertEqual("HTTPS", result.Protocol);
            AssertEqual("200", result.ResponseCode);
            AssertEqual("Healthy (Redirected from \"https://example.test/source\")", result.ResponseMessage);
            AssertEqual("nginx", result.ServerId);
            AssertEqual("2", result.HttpAutoRedirects);
            AssertEqual("text/html", result.HttpContentType);
            AssertEqual("21.10.2015 07:28", result.HttpExpires);
            AssertEqual("abc123", result.HttpEtag);
            AssertEqual(321L, result.ContentLength);
        }

        private static void HttpResponseInterpreterPreservesHandledErrorAndCloudflareNoteMapping()
        {
            EndpointHttpHandledErrorResult result = EndpointHttpResponseInterpreter.InterpretHandledError(
                new EndpointHttpHandledErrorInput
                {
                    StatusCode = 403,
                    StatusCodeName = "Forbidden",
                    StatusDescription = "Forbidden",
                    WebExceptionMessage = "ProtocolError: blocked",
                    IsCloudflareProtected = true,
                    CloudflareRay = "abc123",
                });

            AssertEqual("403", result.ResponseCode);
            AssertEqual("Forbidden -> ProtocolError: blocked [Cloudflare Bot Protection | CF-RAY: abc123]", result.ResponseMessage);
        }

        private static void HttpResponseInterpreterPreservesContentLengthFormatting()
        {
            AssertEqual("N/A", EndpointHttpResponseInterpreter.FormatContentLength(-1, "N/A"));
            AssertEqual((1024d / 1024d).ToString("0.00") + " kB", EndpointHttpResponseInterpreter.FormatContentLength(1024, "N/A"));
            AssertEqual((1048576d / 1048576d).ToString("0.00") + " MB", EndpointHttpResponseInterpreter.FormatContentLength(1048576, "N/A"));
            AssertEqual((1073741824d / 1073741824d).ToString("0.00") + " GB", EndpointHttpResponseInterpreter.FormatContentLength(1073741824, "N/A"));
            AssertEqual("999 bytes", EndpointHttpResponseInterpreter.FormatContentLength(999, "N/A"));
        }

        private static void CloudflareBypassInterpreterPreservesSuccessfulBypassOverrideMapping()
        {
            EndpointCloudflareBypassInterpretResult result = EndpointCloudflareBypassInterpreter.Interpret(
                new EndpointCloudflareBypassInterpretInput
                {
                    ExistingResponseCode = "403",
                    ExistingResponseMessage = "Forbidden",
                    HasBypassResult = true,
                    BypassSuccess = true,
                    BypassStatusCode = 200,
                    BypassStatusMessage = "OK (FlareSolverr bypass)",
                    BypassMethodUsed = "FlareSolverr",
                });

            AssertEqual("200", result.ResponseCode);
            AssertEqual("OK (FlareSolverr bypass)", result.ResponseMessage);
        }

        private static void CloudflareBypassInterpreterPreservesFailedBypassAppendMapping()
        {
            EndpointCloudflareBypassInterpretResult result = EndpointCloudflareBypassInterpreter.Interpret(
                new EndpointCloudflareBypassInterpretInput
                {
                    ExistingResponseCode = "403",
                    ExistingResponseMessage = "Forbidden",
                    HasBypassResult = true,
                    BypassSuccess = false,
                    BypassStatusCode = 0,
                    BypassStatusMessage = "FlareSolverr not reachable",
                    BypassMethodUsed = "FlareSolverr",
                });

            AssertEqual("403", result.ResponseCode);
            AssertEqual("Forbidden | Bypass(FlareSolverr): FlareSolverr not reachable", result.ResponseMessage);
        }

        private static void CloudflareBypassInterpreterPreservesBypassExceptionAppendMapping()
        {
            EndpointCloudflareBypassInterpretResult result = EndpointCloudflareBypassInterpreter.Interpret(
                new EndpointCloudflareBypassInterpretInput
                {
                    ExistingResponseCode = "403",
                    ExistingResponseMessage = "Forbidden",
                    BypassExceptionMessage = "unexpected failure",
                });

            AssertEqual("403", result.ResponseCode);
            AssertEqual("Forbidden | Bypass error: unexpected failure", result.ResponseMessage);
        }

        private static void CloudflareBypassExecutorPreservesNoAttemptBehavior()
        {
            bool checkerCalled = false;

            EndpointCloudflareBypassInterpretResult result = EndpointCloudflareBypassExecutor.Execute(
                new EndpointCloudflareBypassExecuteInput
                {
                    ShouldAttemptBypass = false,
                    ExistingResponseCode = "403",
                    ExistingResponseMessage = "Forbidden",
                    EndpointAddress = "https://example.test",
                    ExecuteBypassCheck = _ =>
                    {
                        checkerCalled = true;
                        return null;
                    },
                });

            AssertEqual(false, checkerCalled);
            AssertEqual("403", result.ResponseCode);
            AssertEqual("Forbidden", result.ResponseMessage);
        }

        private static void CloudflareBypassExecutorPreservesSuccessMappingBehavior()
        {
            EndpointCloudflareBypassInterpretResult result = EndpointCloudflareBypassExecutor.Execute(
                new EndpointCloudflareBypassExecuteInput
                {
                    ShouldAttemptBypass = true,
                    ExistingResponseCode = "403",
                    ExistingResponseMessage = "Forbidden",
                    EndpointAddress = "https://example.test",
                    ExecuteBypassCheck = _ => new EndpointCloudflareBypassCheckResult
                    {
                        HasBypassResult = true,
                        BypassSuccess = true,
                        BypassStatusCode = 200,
                        BypassStatusMessage = "OK (FlareSolverr bypass)",
                        BypassMethodUsed = "FlareSolverr",
                    },
                });

            AssertEqual("200", result.ResponseCode);
            AssertEqual("OK (FlareSolverr bypass)", result.ResponseMessage);
        }

        private static void CloudflareBypassExecutorPreservesExceptionAppendBehavior()
        {
            EndpointCloudflareBypassInterpretResult result = EndpointCloudflareBypassExecutor.Execute(
                new EndpointCloudflareBypassExecuteInput
                {
                    ShouldAttemptBypass = true,
                    ExistingResponseCode = "403",
                    ExistingResponseMessage = "Forbidden",
                    EndpointAddress = "https://example.test",
                    ExecuteBypassCheck = _ => throw new InvalidOperationException("unexpected failure"),
                });

            AssertEqual("403", result.ResponseCode);
            AssertEqual("Forbidden | Bypass error: unexpected failure", result.ResponseMessage);
        }

        private static void HttpHeaderCollectorPreservesNullAndEmptyHandling()
        {
            AssertEqual(0, EndpointHttpHeaderCollector.Collect(null).Count);

            WebHeaderCollection emptyHeaders = new WebHeaderCollection();
            AssertEqual(0, EndpointHttpHeaderCollector.Collect(emptyHeaders).Count);
        }

        private static void HttpHeaderCollectorPreservesHeaderNameAndValueMapping()
        {
            WebHeaderCollection headers = new WebHeaderCollection
            {
                ["Server"] = "nginx",
                ["ETag"] = "abc123",
                ["X-Custom"] = "x-value"
            };

            List<Property> result = EndpointHttpHeaderCollector.Collect(headers);

            AssertEqual(3, result.Count);

            Dictionary<string, string> mapped = new Dictionary<string, string>();
            foreach (Property item in result)
            {
                mapped[item.ItemName] = item.ItemValue;
            }

            AssertEqual("nginx", mapped["Server"]);
            AssertEqual("abc123", mapped["ETag"]);
            AssertEqual("x-value", mapped["X-Custom"]);
        }

        private static void HttpResponseBodyProcessorPreservesReadTriggerConditions()
        {
            AssertEqual(false, EndpointHttpResponseBodyProcessor.ShouldReadResponseBody(false, false));
            AssertEqual(true, EndpointHttpResponseBodyProcessor.ShouldReadResponseBody(true, false));
            AssertEqual(true, EndpointHttpResponseBodyProcessor.ShouldReadResponseBody(false, true));
        }

        private static void HttpResponseBodyProcessorPreservesMaxByteCapBehavior()
        {
            byte[] sourceBytes = Encoding.ASCII.GetBytes("1234567890");

            byte[] noRoom = EndpointHttpResponseBodyProcessor.ReadResponseBytes(new MemoryStream(sourceBytes), maxBytes: 0, chunkSize: 4);
            AssertEqual(0, noRoom.Length);

            byte[] capped = EndpointHttpResponseBodyProcessor.ReadResponseBytes(new MemoryStream(sourceBytes), maxBytes: 8, chunkSize: 4);
            AssertEqual(8, capped.Length);
            AssertEqual("12345678", Encoding.ASCII.GetString(capped));

            byte[] all = EndpointHttpResponseBodyProcessor.ReadResponseBytes(new MemoryStream(sourceBytes), maxBytes: 20, chunkSize: 4);
            AssertEqual(10, all.Length);
            AssertEqual("1234567890", Encoding.ASCII.GetString(all));
        }

        private static void HttpResponseBodyProcessorPreservesHtmlMetaGating()
        {
            AssertEqual(true, EndpointHttpResponseBodyProcessor.ShouldResolveHtmlMetaInfo(true, "text/html"));
            AssertEqual(false, EndpointHttpResponseBodyProcessor.ShouldResolveHtmlMetaInfo(false, "text/html"));
            AssertEqual(false, EndpointHttpResponseBodyProcessor.ShouldResolveHtmlMetaInfo(true, "application/json"));
            AssertEqual(false, EndpointHttpResponseBodyProcessor.ShouldResolveHtmlMetaInfo(true, null));
        }

        private static void HttpResponseBodyProcessorPreservesEncodingFallbackGating()
        {
            Encoding utf8 = Encoding.UTF8;
            Encoding ascii = Encoding.ASCII;

            AssertEqual(true, EndpointHttpResponseBodyProcessor.ShouldResolveMetaWithHtmlEncodingFallback(null, utf8));
            AssertEqual(false, EndpointHttpResponseBodyProcessor.ShouldResolveMetaWithHtmlEncodingFallback(utf8, ascii));
            AssertEqual(false, EndpointHttpResponseBodyProcessor.ShouldResolveMetaWithHtmlEncodingFallback(null, null));

            AssertEqual(true, EndpointHttpResponseBodyProcessor.ShouldAssignDefaultHtmlEncoding(null, null));
            AssertEqual(false, EndpointHttpResponseBodyProcessor.ShouldAssignDefaultHtmlEncoding(null, utf8));
            AssertEqual(false, EndpointHttpResponseBodyProcessor.ShouldAssignDefaultHtmlEncoding(ascii, null));
        }

        private static void HttpHtmlMetadataResolverPreservesTitleMetaLinksAndLanguageExtraction()
        {
            string html = "<html lang='mul'><head>" +
                "<title>Header Title</title>" +
                "<meta name='description' content='Example description' />" +
                "<meta name='web_author' content='Author Name' />" +
                "<meta name='theme-color' content='#112233' />" +
                "<meta http-equiv='content-type' content='text/html; charset=utf-8' />" +
                "</head><body>" +
                "<a href='https://example.test/target'>self</a>" +
                "<a href='https://example.test/other/'>other1</a>" +
                "<a href='https://example.test/other'>other2</a>" +
                "<img src='ftp://files.example.test/a.bin'/>" +
                "</body></html>";

            EndpointHttpHtmlMetadataResolveOutput result = EndpointHttpHtmlMetadataResolver.Resolve(
                new EndpointHttpHtmlMetadataResolveInput
                {
                    HtmlResponseDocumentString = html,
                    ResponseUri = new Uri("https://example.test/target"),
                    ResolvePageLinks = true,
                    StatusNotAvailable = "N/A",
                    CurrentHtmlContentLanguage = "N/A",
                    CurrentHtmlEncoding = null,
                    ParseEncoding = value => value != null && value.ToLower().Contains("utf-8") ? Encoding.UTF8 : null,
                });

            AssertEqual("Header Title", result.HtmlTitle);
            AssertEqual("Example description", result.HtmlDescription);
            AssertEqual("Author Name", result.HtmlAuthor);
            AssertEqual("Multi-Language (mul)", result.HtmlContentLanguage);
            AssertEqual(true, result.HasHtmlThemeColor);
            AssertEqual(ColorTranslator.FromHtml("#112233"), result.HtmlThemeColor);
            AssertEqual(Encoding.UTF8.WebName, result.HtmlEncoding.WebName);

            AssertEqual(2, result.HtmlPageLinks.PropertyItem.Count);
            AssertEqual("https://example.test/other", result.HtmlPageLinks.PropertyItem[0].ItemValue);
            AssertEqual("ftp://files.example.test/a.bin", result.HtmlPageLinks.PropertyItem[1].ItemValue);
        }

        private static void HttpHtmlMetadataResolverPreservesExistingHtmlEncodingAndThemeParsingFallback()
        {
            Encoding existingEncoding = Encoding.ASCII;
            EndpointHttpHtmlMetadataResolveOutput result = EndpointHttpHtmlMetadataResolver.Resolve(
                new EndpointHttpHtmlMetadataResolveInput
                {
                    HtmlResponseDocumentString = "<html><head>" +
                        "<meta charset='utf-16'/>" +
                        "<meta property='og:title' content='Meta Title'/>" +
                        "<meta name='theme-color' content='not-a-color'/>" +
                        "</head></html>",
                    ResponseUri = new Uri("https://example.test/page"),
                    ResolvePageLinks = false,
                    StatusNotAvailable = "N/A",
                    CurrentHtmlContentLanguage = "N/A",
                    CurrentHtmlEncoding = existingEncoding,
                    ParseEncoding = _ => Encoding.Unicode,
                });

            AssertEqual("Meta Title", result.HtmlTitle);
            AssertEqual(existingEncoding.WebName, result.HtmlEncoding.WebName);
            AssertEqual(false, result.HasHtmlThemeColor);
            AssertEqual("N/A", result.HtmlAuthor);
            AssertEqual("N/A", result.HtmlDescription);
            AssertEqual(null, result.HtmlPageLinks);
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

    public class Property
    {
        public string ItemName { get; set; }
        public string ItemValue { get; set; }
    }

    public class PropertyItems
    {
        public List<Property> PropertyItem { get; set; }
    }
}
