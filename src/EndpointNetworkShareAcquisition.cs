using System;
using System.Collections.Generic;

namespace EndpointChecker
{
    internal sealed class EndpointNetworkShareAcquireResult
    {
        public bool ShouldAssign { get; set; }

        public string[] Shares { get; set; }
    }

    internal static class EndpointNetworkShareAcquisition
    {
        public static EndpointNetworkShareAcquireResult TryAcquire(
            bool resolveNetworkShares,
            string hostName,
            Func<string, List<string>> getNetShares)
        {
            if (!resolveNetworkShares ||
                string.IsNullOrEmpty(hostName) ||
                getNetShares == null)
            {
                return new EndpointNetworkShareAcquireResult { ShouldAssign = false };
            }

            try
            {
                List<string> networkShares = getNetShares(hostName);

                return new EndpointNetworkShareAcquireResult
                {
                    ShouldAssign = true,
                    Shares = EndpointNetworkShareResolver.BuildSortedShareArray(networkShares),
                };
            }
            catch
            {
                return new EndpointNetworkShareAcquireResult { ShouldAssign = false };
            }
        }
    }
}
