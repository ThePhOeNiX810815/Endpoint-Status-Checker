using System;
using System.Collections.Generic;

namespace EndpointChecker
{
    internal static class EndpointNetworkShareResolverTests
    {
        public static void Register()
        {
            EndpointCheckingCoreTestRunner.Run("Network share resolver returns null for null input", NetworkShareResolverReturnsNullForNullInput);
            EndpointCheckingCoreTestRunner.Run("Network share resolver returns null for empty input", NetworkShareResolverReturnsNullForEmptyInput);
            EndpointCheckingCoreTestRunner.Run("Network share resolver returns sorted share array", NetworkShareResolverReturnsSortedShareArray);
        }

        private static void NetworkShareResolverReturnsNullForNullInput()
        {
            string[] shares = EndpointNetworkShareResolver.BuildSortedShareArray(null);

            EndpointCheckingCoreTestRunner.AssertEqual(null, shares);
        }

        private static void NetworkShareResolverReturnsNullForEmptyInput()
        {
            string[] shares = EndpointNetworkShareResolver.BuildSortedShareArray(Array.Empty<string>());

            EndpointCheckingCoreTestRunner.AssertEqual(null, shares);
        }

        private static void NetworkShareResolverReturnsSortedShareArray()
        {
            string[] shares = EndpointNetworkShareResolver.BuildSortedShareArray(new List<string>
            {
                "\\\\host\\ZETA",
                "\\\\host\\alpha",
                "\\\\host\\Beta"
            });

            EndpointCheckingCoreTestRunner.AssertArray(new[]
            {
                "\\\\host\\alpha",
                "\\\\host\\Beta",
                "\\\\host\\ZETA"
            }, shares);
        }
    }
}
