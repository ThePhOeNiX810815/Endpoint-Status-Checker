using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace EndpointChecker
{
    internal static class EndpointSslCertificateAcquisitionTests
    {
        public static void Register()
        {
            EndpointCheckingCoreTestRunner.Run("SSL certificate acquisition returns empty list when getter is null", SslCertificateAcquisitionReturnsEmptyListWhenGetterIsNull);
            EndpointCheckingCoreTestRunner.Run("SSL certificate acquisition returns empty list when certificate is null", SslCertificateAcquisitionReturnsEmptyListWhenCertificateIsNull);
            EndpointCheckingCoreTestRunner.Run("SSL certificate acquisition returns empty list when getter throws", SslCertificateAcquisitionReturnsEmptyListWhenGetterThrows);
            EndpointCheckingCoreTestRunner.Run("SSL certificate acquisition maps representative certificate properties", SslCertificateAcquisitionMapsRepresentativeCertificateProperties);
        }

        private static void SslCertificateAcquisitionReturnsEmptyListWhenGetterIsNull()
        {
            List<Property> properties = EndpointSslCertificateAcquisition.TryCollectProperties(null);

            EndpointCheckingCoreTestRunner.AssertEqual(0, properties.Count);
        }

        private static void SslCertificateAcquisitionReturnsEmptyListWhenCertificateIsNull()
        {
            List<Property> properties = EndpointSslCertificateAcquisition.TryCollectProperties(() => null);

            EndpointCheckingCoreTestRunner.AssertEqual(0, properties.Count);
        }

        private static void SslCertificateAcquisitionReturnsEmptyListWhenGetterThrows()
        {
            List<Property> properties = EndpointSslCertificateAcquisition.TryCollectProperties(
                () => throw new InvalidOperationException("failed"));

            EndpointCheckingCoreTestRunner.AssertEqual(0, properties.Count);
        }

        private static void SslCertificateAcquisitionMapsRepresentativeCertificateProperties()
        {
            using (RSA rsa = RSA.Create(2048))
            {
                CertificateRequest request = new CertificateRequest(
                    "CN=endpoint-checker-test",
                    rsa,
                    HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1);

                using (X509Certificate2 generated = request.CreateSelfSigned(
                    DateTimeOffset.UtcNow.AddDays(-1),
                    DateTimeOffset.UtcNow.AddDays(7)))
                {
                    List<Property> properties = EndpointSslCertificateAcquisition.TryCollectProperties(() => generated);
                    Dictionary<string, string> byName = new Dictionary<string, string>(StringComparer.Ordinal);
                    foreach (Property property in properties)
                    {
                        byName[property.ItemName] = property.ItemValue;
                    }

                    EndpointCheckingCoreTestRunner.AssertEqual(generated.Subject, byName["Subject"]);
                    EndpointCheckingCoreTestRunner.AssertEqual(generated.Issuer, byName["Issuer Name"]);
                    EndpointCheckingCoreTestRunner.AssertEqual(generated.Thumbprint, byName["Thumbprint"]);
                }
            }
        }
    }
}
