using System.Collections.Generic;

namespace EndpointChecker
{
    internal static class EndpointNetworkShareResolver
    {
        public static string[] BuildSortedShareArray(IReadOnlyList<string> networkShares)
        {
            if (networkShares == null || networkShares.Count <= 0)
            {
                return null;
            }

            List<string> sortedShares = new List<string>(networkShares);
            sortedShares.Sort();

            return sortedShares.ToArray();
        }
    }
}
