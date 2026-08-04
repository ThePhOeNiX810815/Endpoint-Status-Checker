using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace EndpointChecker
{
    internal sealed class EndpointSslCertificatePropertyItem
    {
        public string ItemName { get; set; }

        public string ItemValue { get; set; }
    }

    internal static class EndpointSslCertificatePropertyMapper
    {
        public static IReadOnlyList<EndpointSslCertificatePropertyItem> Build(X509Certificate2 certificate)
        {
            List<EndpointSslCertificatePropertyItem> properties = new List<EndpointSslCertificatePropertyItem>();
            if (certificate == null)
            {
                return properties;
            }

            properties.Add(new EndpointSslCertificatePropertyItem { ItemName = "Archived", ItemValue = certificate.Archived.ToString() });
            properties.Add(new EndpointSslCertificatePropertyItem { ItemName = "Has Private Key", ItemValue = certificate.HasPrivateKey.ToString() });
            properties.Add(new EndpointSslCertificatePropertyItem { ItemName = "Valid To", ItemValue = certificate.NotAfter.ToString() });
            properties.Add(new EndpointSslCertificatePropertyItem { ItemName = "Valid From", ItemValue = certificate.NotBefore.ToString() });
            properties.Add(new EndpointSslCertificatePropertyItem { ItemName = "Version", ItemValue = certificate.Version.ToString() });
            properties.Add(new EndpointSslCertificatePropertyItem { ItemName = "Public Key", ItemValue = certificate.GetPublicKeyString() });

            AddIfNonEmpty(properties, "Signature Algorithm", certificate.SignatureAlgorithm.FriendlyName);
            AddIfNonEmpty(properties, "Friendly Name", certificate.FriendlyName);
            AddIfNonEmpty(properties, "Issuer Name", certificate.Issuer);
            AddIfNonEmpty(properties, "Serial Number", certificate.SerialNumber);
            AddIfNonEmpty(properties, "Subject", certificate.Subject);
            AddIfNonEmpty(properties, "Thumbprint", certificate.Thumbprint);

            return properties;
        }

        private static void AddIfNonEmpty(List<EndpointSslCertificatePropertyItem> properties, string name, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                properties.Add(new EndpointSslCertificatePropertyItem { ItemName = name, ItemValue = value });
            }
        }
    }
}
