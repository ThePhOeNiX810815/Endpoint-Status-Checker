using System;
using System.Collections.Generic;

namespace EndpointChecker
{
    internal static class EndpointNetworkShareAcquisitionTests
    {
        public static void Register()
        {
            EndpointCheckingCoreTestRunner.Run("Network share acquisition does not assign when disabled", NetworkShareAcquisitionDoesNotAssignWhenDisabled);
            EndpointCheckingCoreTestRunner.Run("Network share acquisition assigns sorted shares on success", NetworkShareAcquisitionAssignsSortedSharesOnSuccess);
            EndpointCheckingCoreTestRunner.Run("Network share acquisition does not assign on acquisition exception", NetworkShareAcquisitionDoesNotAssignOnAcquisitionException);
        }

        private static void NetworkShareAcquisitionDoesNotAssignWhenDisabled()
        {
            bool called = false;
            EndpointNetworkShareAcquireResult result = EndpointNetworkShareAcquisition.TryAcquire(
                resolveNetworkShares: false,
                hostName: "example.test",
                getNetShares: hostName =>
                {
                    called = true;
                    return new List<string> { "\\\\example.test\\Share" };
                });

            EndpointCheckingCoreTestRunner.AssertEqual(false, called);
            EndpointCheckingCoreTestRunner.AssertEqual(false, result.ShouldAssign);
            EndpointCheckingCoreTestRunner.AssertEqual(null, result.Shares);
        }

        private static void NetworkShareAcquisitionAssignsSortedSharesOnSuccess()
        {
            EndpointNetworkShareAcquireResult result = EndpointNetworkShareAcquisition.TryAcquire(
                resolveNetworkShares: true,
                hostName: "example.test",
                getNetShares: hostName => new List<string>
                {
                    "\\\\example.test\\zeta",
                    "\\\\example.test\\alpha",
                });

            EndpointCheckingCoreTestRunner.AssertEqual(true, result.ShouldAssign);
            EndpointCheckingCoreTestRunner.AssertArray(
                new[]
                {
                    "\\\\example.test\\alpha",
                    "\\\\example.test\\zeta",
                },
                result.Shares);
        }

        private static void NetworkShareAcquisitionDoesNotAssignOnAcquisitionException()
        {
            EndpointNetworkShareAcquireResult result = EndpointNetworkShareAcquisition.TryAcquire(
                resolveNetworkShares: true,
                hostName: "example.test",
                getNetShares: hostName => throw new InvalidOperationException("fail"));

            EndpointCheckingCoreTestRunner.AssertEqual(false, result.ShouldAssign);
            EndpointCheckingCoreTestRunner.AssertEqual(null, result.Shares);
        }
    }
}
