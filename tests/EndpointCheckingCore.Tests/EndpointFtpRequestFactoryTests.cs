using System;
using System.Net;

namespace EndpointChecker
{
    internal static class EndpointFtpRequestFactoryTests
    {
        public static void Register()
        {
            Run("FTP request factory preserves legacy request settings", FtpRequestFactoryPreservesLegacyRequestSettings);
            Run("FTP request factory preserves explicit credentials", FtpRequestFactoryPreservesExplicitCredentials);
            Run("FTP request factory applies anonymous password fallback for missing credentials", FtpRequestFactoryAppliesAnonymousPasswordFallbackForMissingCredentials);
        }

        private static void FtpRequestFactoryPreservesLegacyRequestSettings()
        {
            EndpointFtpRequestBuildResult result = EndpointFtpRequestFactory.Build(new EndpointFtpRequestBuildInput
            {
                EndpointUri = new Uri("ftp://example.test/files"),
                TimeoutMilliseconds = 15000,
                LoginName = "ftp-user",
                LoginPass = "ftp-pass",
                StatusNotAvailable = "N/A",
                AnonymousFtpPassword = "anonymous@example.test",
            });

            FtpWebRequest request = result.Request;

            EndpointCheckingCoreTestRunner.AssertEqual(WebRequestMethods.Ftp.PrintWorkingDirectory, request.Method);
            EndpointCheckingCoreTestRunner.AssertEqual(15000, request.Timeout);
            EndpointCheckingCoreTestRunner.AssertEqual(15000, request.ReadWriteTimeout);
        }

        private static void FtpRequestFactoryPreservesExplicitCredentials()
        {
            EndpointFtpRequestBuildResult result = EndpointFtpRequestFactory.Build(new EndpointFtpRequestBuildInput
            {
                EndpointUri = new Uri("ftp://example.test/files"),
                TimeoutMilliseconds = 10000,
                LoginName = "explicit-user",
                LoginPass = "explicit-pass",
                StatusNotAvailable = "N/A",
                AnonymousFtpPassword = "anonymous@example.test",
            });

            NetworkCredential credential = (NetworkCredential)result.Request.Credentials.GetCredential(new Uri("ftp://example.test/files"), string.Empty);

            EndpointCheckingCoreTestRunner.AssertEqual("explicit-user", result.LoginName);
            EndpointCheckingCoreTestRunner.AssertEqual("explicit-pass", result.LoginPass);
            EndpointCheckingCoreTestRunner.AssertEqual("explicit-user", credential.UserName);
            EndpointCheckingCoreTestRunner.AssertEqual("explicit-pass", credential.Password);
        }

        private static void FtpRequestFactoryAppliesAnonymousPasswordFallbackForMissingCredentials()
        {
            EndpointFtpRequestBuildResult result = EndpointFtpRequestFactory.Build(new EndpointFtpRequestBuildInput
            {
                EndpointUri = new Uri("ftp://example.test/files"),
                TimeoutMilliseconds = 10000,
                LoginName = "N/A",
                LoginPass = "N/A",
                StatusNotAvailable = "N/A",
                AnonymousFtpPassword = "anonymous@example.test",
            });

            EndpointCheckingCoreTestRunner.AssertEqual("anonymous@example.test", result.LoginPass);

            if (string.IsNullOrEmpty(result.LoginName))
            {
                throw new InvalidOperationException("Expected fallback FTP login name to be populated.");
            }

            NetworkCredential credential = (NetworkCredential)result.Request.Credentials.GetCredential(new Uri("ftp://example.test/files"), string.Empty);
            EndpointCheckingCoreTestRunner.AssertEqual(result.LoginName, credential.UserName);
            EndpointCheckingCoreTestRunner.AssertEqual("anonymous@example.test", credential.Password);
        }

        private static void Run(string name, Action test) => EndpointCheckingCoreTestRunner.Run(name, test);
    }
}
