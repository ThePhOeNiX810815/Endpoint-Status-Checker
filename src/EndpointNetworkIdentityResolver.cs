using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EndpointChecker
{
    internal sealed class EndpointNetworkIdentitySeed
    {
        public string[] IpAddresses { get; set; }

        public string[] DnsNames { get; set; }
    }

    internal sealed class EndpointNetworkIdentityFinalizeOutput
    {
        public string[] IpAddresses { get; set; }

        public string[] DnsNames { get; set; }

        public string[] MacAddresses { get; set; }
    }

    internal static class EndpointNetworkIdentityResolver
    {
        public static EndpointNetworkIdentitySeed CreateSeed(string responseHost)
        {
            if (Regex.IsMatch(responseHost, @"^[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$"))
            {
                return new EndpointNetworkIdentitySeed
                {
                    IpAddresses = new[] { responseHost }
                };
            }

            return new EndpointNetworkIdentitySeed
            {
                DnsNames = new[] { responseHost }
            };
        }

        public static bool ShouldIncludeMacAddress(
            string macAddress,
            string ipAddress,
            IReadOnlyCollection<string> localDnsAndGatewayMacAddresses,
            IReadOnlyCollection<string> localDnsAndGatewayIpAddresses)
        {
            if (string.IsNullOrEmpty(macAddress))
            {
                return false;
            }

            return !localDnsAndGatewayMacAddresses.Contains(macAddress) ||
                   localDnsAndGatewayIpAddresses.Contains(ipAddress);
        }

        public static EndpointNetworkIdentityFinalizeOutput Finalize(
            EndpointNetworkIdentitySeed seed,
            IReadOnlyList<string> resolvedIpAddresses,
            IReadOnlyList<string> resolvedDnsNames,
            IReadOnlyList<string> resolvedMacAddresses)
        {
            return new EndpointNetworkIdentityFinalizeOutput
            {
                IpAddresses = resolvedIpAddresses.Count > 0 ? resolvedIpAddresses.ToArray() : seed.IpAddresses,
                DnsNames = resolvedDnsNames.Count > 0 ? resolvedDnsNames.ToArray() : seed.DnsNames,
                MacAddresses = resolvedMacAddresses.Count > 0 ? resolvedMacAddresses.ToArray() : null,
            };
        }
    }
}