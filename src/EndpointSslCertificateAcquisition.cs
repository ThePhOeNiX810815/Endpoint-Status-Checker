using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace EndpointChecker
{
    internal static class EndpointSslCertificateAcquisition
    {
        public static List<Property> TryCollectProperties(Func<X509Certificate> getCertificate)
        {
            List<Property> properties = new List<Property>();

            if (getCertificate == null)
            {
                return properties;
            }

            try
            {
                X509Certificate certificate = getCertificate();
                if (certificate == null)
                {
                    return properties;
                }

                X509Certificate2 certificate2 = new X509Certificate2(certificate);
                foreach (EndpointSslCertificatePropertyItem item in EndpointSslCertificatePropertyMapper.Build(certificate2))
                {
                    properties.Add(new Property
                    {
                        ItemName = item.ItemName,
                        ItemValue = item.ItemValue,
                    });
                }
            }
            catch
            {
            }

            return properties;
        }
    }
}
