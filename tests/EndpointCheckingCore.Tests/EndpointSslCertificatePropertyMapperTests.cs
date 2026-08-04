using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace EndpointChecker
{
    internal static class EndpointSslCertificatePropertyMapperTests
    {
        public static void Register()
        {
            EndpointCheckingCoreTestRunner.Run("SSL certificate mapper returns empty list when certificate is null", SslCertificateMapperReturnsEmptyListWhenCertificateIsNull);
            EndpointCheckingCoreTestRunner.Run("SSL certificate mapper preserves required and optional property values", SslCertificateMapperPreservesRequiredAndOptionalPropertyValues);
        }

        private static void SslCertificateMapperReturnsEmptyListWhenCertificateIsNull()
        {
            IReadOnlyList<EndpointSslCertificatePropertyItem> properties = EndpointSslCertificatePropertyMapper.Build(null);

            EndpointCheckingCoreTestRunner.AssertEqual(0, properties.Count);
        }

        private static void SslCertificateMapperPreservesRequiredAndOptionalPropertyValues()
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
                    IReadOnlyList<EndpointSslCertificatePropertyItem> properties =
                        EndpointSslCertificatePropertyMapper.Build(generated);

                    Dictionary<string, string> byName = new Dictionary<string, string>(StringComparer.Ordinal);
                    foreach (EndpointSslCertificatePropertyItem property in properties)
                    {
                        byName[property.ItemName] = property.ItemValue;
                    }

                    EndpointCheckingCoreTestRunner.AssertEqual(generated.Archived.ToString(), byName["Archived"]);
                    EndpointCheckingCoreTestRunner.AssertEqual(generated.HasPrivateKey.ToString(), byName["Has Private Key"]);
                    EndpointCheckingCoreTestRunner.AssertEqual(generated.NotAfter.ToString(), byName["Valid To"]);
                    EndpointCheckingCoreTestRunner.AssertEqual(generated.NotBefore.ToString(), byName["Valid From"]);
                    EndpointCheckingCoreTestRunner.AssertEqual(generated.Version.ToString(), byName["Version"]);
                    EndpointCheckingCoreTestRunner.AssertEqual(generated.GetPublicKeyString(), byName["Public Key"]);
                    EndpointCheckingCoreTestRunner.AssertEqual(generated.SignatureAlgorithm.FriendlyName, byName["Signature Algorithm"]);
                    EndpointCheckingCoreTestRunner.AssertEqual(generated.Issuer, byName["Issuer Name"]);
                    EndpointCheckingCoreTestRunner.AssertEqual(generated.SerialNumber, byName["Serial Number"]);
                    EndpointCheckingCoreTestRunner.AssertEqual(generated.Subject, byName["Subject"]);
                    EndpointCheckingCoreTestRunner.AssertEqual(generated.Thumbprint, byName["Thumbprint"]);
                }
            }
        }
    }
}
