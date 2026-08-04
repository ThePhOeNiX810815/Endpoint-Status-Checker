using System;
using System.Net;
using System.Net.Cache;

namespace EndpointChecker
{
    internal sealed class EndpointFtpRequestBuildInput
    {
        public Uri EndpointUri { get; set; }

        public int TimeoutMilliseconds { get; set; }

        public string LoginName { get; set; }

        public string LoginPass { get; set; }

        public string StatusNotAvailable { get; set; }

        public string AnonymousFtpPassword { get; set; }
    }

    internal sealed class EndpointFtpRequestBuildResult
    {
        public FtpWebRequest Request { get; set; }

        public string LoginName { get; set; }

        public string LoginPass { get; set; }
    }

    internal static class EndpointFtpRequestFactory
    {
        public static EndpointFtpRequestBuildResult Build(EndpointFtpRequestBuildInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(input.EndpointUri.OriginalString);
            request.Method = WebRequestMethods.Ftp.PrintWorkingDirectory;
            request.Timeout = input.TimeoutMilliseconds;
            request.ReadWriteTimeout = input.TimeoutMilliseconds;
            request.UsePassive = false;
            request.UseBinary = false;
            request.KeepAlive = false;
            request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
            request.Proxy = null;

            string loginName = input.LoginName;
            string loginPass = input.LoginPass;

            if (loginName == input.StatusNotAvailable ||
                loginPass == input.StatusNotAvailable)
            {
                loginName = request.Credentials.GetCredential(input.EndpointUri, string.Empty).UserName;
                loginPass = input.AnonymousFtpPassword;
            }

            request.Credentials = new NetworkCredential(loginName, loginPass);

            return new EndpointFtpRequestBuildResult
            {
                Request = request,
                LoginName = loginName,
                LoginPass = loginPass,
            };
        }
    }
}