using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace EndpointChecker
{
    /// <summary>
    /// Determines whether an endpoint address points at a local/internal network location,
    /// where a VirusTotal URL scan is meaningless (VirusTotal can only reach public addresses).
    /// </summary>
    internal enum EndpointLocalAddressClassification
    {
        /// <summary>Address is a routable public host — VirusTotal scanning is applicable.</summary>
        Public,

        /// <summary>Address is loopback, private-range, link-local, or a bare/.local hostname.</summary>
        Local,

        /// <summary>Could not be determined automatically (e.g. an unresolved public-looking DNS name).</summary>
        Unknown
    }

    internal static class EndpointLocalAddressClassifier
    {
        private const string StatusNotAvailable = "N/A";

        public static EndpointLocalAddressClassification Classify(EndpointDefinition endpoint)
        {
            if (endpoint == null)
            {
                return EndpointLocalAddressClassification.Unknown;
            }

            string host = TryGetHost(endpoint.Address) ?? TryGetHost(endpoint.ResponseAddress);
            if (string.IsNullOrEmpty(host))
            {
                return EndpointLocalAddressClassification.Unknown;
            }

            if (host.Equals("localhost", StringComparison.OrdinalIgnoreCase) ||
                host.EndsWith(".local", StringComparison.OrdinalIgnoreCase))
            {
                return EndpointLocalAddressClassification.Local;
            }

            if (IPAddress.TryParse(host, out IPAddress hostAddress))
            {
                return IsPrivateOrLoopback(hostAddress)
                    ? EndpointLocalAddressClassification.Local
                    : EndpointLocalAddressClassification.Public;
            }

            List<IPAddress> resolvedAddresses = (endpoint.IPAddress ?? Array.Empty<string>())
                .Where(candidate => !string.IsNullOrWhiteSpace(candidate) && candidate != StatusNotAvailable)
                .Select(candidate => IPAddress.TryParse(candidate, out IPAddress parsed) ? parsed : null)
                .Where(parsed => parsed != null)
                .ToList();

            if (resolvedAddresses.Count > 0)
            {
                return resolvedAddresses.All(IsPrivateOrLoopback)
                    ? EndpointLocalAddressClassification.Local
                    : EndpointLocalAddressClassification.Public;
            }

            // Bare hostnames (no dot) are, in practice, always intranet device names.
            if (!host.Contains('.'))
            {
                return EndpointLocalAddressClassification.Local;
            }

            return EndpointLocalAddressClassification.Unknown;
        }

        private static string TryGetHost(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                return null;
            }

            try
            {
                return new Uri(address, UriKind.Absolute).Host;
            }
            catch (UriFormatException)
            {
                return null;
            }
        }

        private static bool IsPrivateOrLoopback(IPAddress address)
        {
            if (IPAddress.IsLoopback(address))
            {
                return true;
            }

            if (address.AddressFamily == AddressFamily.InterNetwork)
            {
                byte[] bytes = address.GetAddressBytes();

                return bytes[0] == 10 ||
                       (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) ||
                       (bytes[0] == 192 && bytes[1] == 168) ||
                       (bytes[0] == 169 && bytes[1] == 254);
            }

            if (address.AddressFamily == AddressFamily.InterNetworkV6)
            {
                if (address.IsIPv6LinkLocal)
                {
                    return true;
                }

                byte[] bytes = address.GetAddressBytes();
                return (bytes[0] & 0xFE) == 0xFC; // fc00::/7 — unique local address
            }

            return false;
        }
    }
}
