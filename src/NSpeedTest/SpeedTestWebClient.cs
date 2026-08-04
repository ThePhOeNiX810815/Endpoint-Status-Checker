using EndpointChecker;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Web;
using System.Xml.Serialization;

namespace NSpeedTest
{
    internal class SpeedTestWebClient : WebClient
    {
        private readonly bool bypassTlsCertificateValidation;

        public int ConnectionLimit { get; set; }

        public SpeedTestWebClient(bool bypassTlsCertificateValidation = false)
        {
            ConnectionLimit = 32;
            this.bypassTlsCertificateValidation = bypassTlsCertificateValidation;
        }

        public T GetConfig<T>(string url)
        {
            using (CustomWebClient webClient = new CustomWebClient())
            {
                var data = webClient.DownloadString(url);
                var xmlSerializer = new XmlSerializer(typeof(T));
                using (var reader = new StringReader(data))
                {
                    return (T)xmlSerializer.Deserialize(reader);
                }
            }
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(AddTimeStamp(address)) as HttpWebRequest;

            if (request == null)
            {
                return base.GetWebRequest(AddTimeStamp(address));
            }

            request.Timeout = 5000;
            request.ReadWriteTimeout = 5000;
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.UserAgent = Program.http_UserAgent;
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
            request.ServicePoint.ConnectionLimit = ConnectionLimit;

            if (bypassTlsCertificateValidation)
            {
                request.ServerCertificateValidationCallback = delegate { return true; };
            }

            return request;
        }

        private static Uri AddTimeStamp(Uri address)
        {
            var uriBuilder = new UriBuilder(address);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["x"] = DateTime.Now.ToFileTime().ToString(CultureInfo.InvariantCulture);
            uriBuilder.Query = query.ToString();
            return uriBuilder.Uri;
        }
    }
}
