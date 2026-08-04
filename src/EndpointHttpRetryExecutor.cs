using System;
using System.Net;

namespace EndpointChecker
{
    /// <summary>
    /// Preserves the legacy timeout-retry behavior used by endpoint HTTP checks.
    /// </summary>
    internal static class EndpointHttpRetryExecutor
    {
        public static HttpWebResponse Execute(HttpWebRequest httpWebRequest, int maxRetryCount, int retryCount = 0)
        {
            return ExecuteWithRetry(() => (HttpWebResponse)httpWebRequest.GetResponse(), maxRetryCount, retryCount);
        }

        internal static HttpWebResponse ExecuteWithRetry(Func<HttpWebResponse> getResponse, int maxRetryCount, int retryCount = 0)
        {
            try
            {
                return getResponse();
            }
            catch (WebException webException)
            {
                if (webException.Status == WebExceptionStatus.Timeout &&
                    retryCount < maxRetryCount)
                {
                    retryCount++;
                    return ExecuteWithRetry(getResponse, maxRetryCount, retryCount);
                }

                throw;
            }
        }
    }
}
