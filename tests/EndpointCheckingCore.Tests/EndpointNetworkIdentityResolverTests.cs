using System;
using System.Collections.Generic;

namespace EndpointChecker
{
    internal static class EndpointNetworkIdentityResolverTests
    {
        public static void Register()
        {
            Run("Network identity seed maps IPv4 host to IP address", NetworkIdentitySeedMapsIpv4HostToIpAddress);
            Run("Network identity seed maps host name to DNS name", NetworkIdentitySeedMapsHostNameToDnsName);
            Run("Network identity resolver keeps only allowed MAC addresses", NetworkIdentityResolverKeepsOnlyAllowedMacAddresses);
            Run("Network identity finalization prefers resolved values", NetworkIdentityFinalizationPrefersResolvedValues);
            Run("Network identity finalization preserves seed values when unresolved", NetworkIdentityFinalizationPreservesSeedValuesWhenUnresolved);
        }

        private static void NetworkIdentitySeedMapsIpv4HostToIpAddress()
        {
            EndpointNetworkIdentitySeed seed = EndpointNetworkIdentityResolver.CreateSeed("203.0.113.9");

            EndpointCheckingCoreTestRunner.AssertArray(new[] { "203.0.113.9" }, seed.IpAddresses);
            EndpointCheckingCoreTestRunner.AssertEqual(null, seed.DnsNames);
        }

        private static void NetworkIdentitySeedMapsHostNameToDnsName()
        {
            EndpointNetworkIdentitySeed seed = EndpointNetworkIdentityResolver.CreateSeed("status.example.test");

            EndpointCheckingCoreTestRunner.AssertEqual(null, seed.IpAddresses);
            EndpointCheckingCoreTestRunner.AssertArray(new[] { "status.example.test" }, seed.DnsNames);
        }

        private static void NetworkIdentityResolverKeepsOnlyAllowedMacAddresses()
        {
            bool includeRegularMac = EndpointNetworkIdentityResolver.ShouldIncludeMacAddress(
                "AA-BB-CC-DD-EE-FF",
                "203.0.113.10",
                new[] { "11-22-33-44-55-66" },
                new[] { "192.0.2.1" });

            bool includeGatewayMacWithGatewayIp = EndpointNetworkIdentityResolver.ShouldIncludeMacAddress(
                "11-22-33-44-55-66",
                "192.0.2.1",
                new[] { "11-22-33-44-55-66" },
                new[] { "192.0.2.1" });

            bool includeGatewayMacWithNonGatewayIp = EndpointNetworkIdentityResolver.ShouldIncludeMacAddress(
                "11-22-33-44-55-66",
                "203.0.113.10",
                new[] { "11-22-33-44-55-66" },
                new[] { "192.0.2.1" });

            EndpointCheckingCoreTestRunner.AssertEqual(true, includeRegularMac);
            EndpointCheckingCoreTestRunner.AssertEqual(true, includeGatewayMacWithGatewayIp);
            EndpointCheckingCoreTestRunner.AssertEqual(false, includeGatewayMacWithNonGatewayIp);
        }

        private static void NetworkIdentityFinalizationPrefersResolvedValues()
        {
            EndpointNetworkIdentitySeed seed = EndpointNetworkIdentityResolver.CreateSeed("status.example.test");

            EndpointNetworkIdentityFinalizeOutput output = EndpointNetworkIdentityResolver.Finalize(
                seed,
                new List<string> { "203.0.113.10", "203.0.113.11" },
                new List<string> { "status.example.test", "status2.example.test" },
                new List<string> { "AA-BB-CC-DD-EE-FF" });

            EndpointCheckingCoreTestRunner.AssertArray(new[] { "203.0.113.10", "203.0.113.11" }, output.IpAddresses);
            EndpointCheckingCoreTestRunner.AssertArray(new[] { "status.example.test", "status2.example.test" }, output.DnsNames);
            EndpointCheckingCoreTestRunner.AssertArray(new[] { "AA-BB-CC-DD-EE-FF" }, output.MacAddresses);
        }

        private static void NetworkIdentityFinalizationPreservesSeedValuesWhenUnresolved()
        {
            EndpointNetworkIdentitySeed seed = EndpointNetworkIdentityResolver.CreateSeed("203.0.113.9");

            EndpointNetworkIdentityFinalizeOutput output = EndpointNetworkIdentityResolver.Finalize(
                seed,
                new List<string>(),
                new List<string>(),
                new List<string>());

            EndpointCheckingCoreTestRunner.AssertArray(new[] { "203.0.113.9" }, output.IpAddresses);
            EndpointCheckingCoreTestRunner.AssertEqual(null, output.DnsNames);
            EndpointCheckingCoreTestRunner.AssertEqual(null, output.MacAddresses);
        }

        private static void Run(string name, Action test) => EndpointCheckingCoreTestRunner.Run(name, test);
    }
}
