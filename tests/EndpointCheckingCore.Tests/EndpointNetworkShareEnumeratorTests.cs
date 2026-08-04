using System;
using System.Collections.Generic;

namespace EndpointChecker
{
    internal static class EndpointNetworkShareEnumeratorTests
    {
        public static void Register()
        {
            EndpointCheckingCoreTestRunner.Run("Network share enumerator maps known share types", NetworkShareEnumeratorMapsKnownShareTypes);
            EndpointCheckingCoreTestRunner.Run("Network share enumerator maps unknown share types", NetworkShareEnumeratorMapsUnknownShareTypes);
            EndpointCheckingCoreTestRunner.Run("Network share enumerator preserves optional share remarks", NetworkShareEnumeratorPreservesOptionalShareRemarks);
            EndpointCheckingCoreTestRunner.Run("Network share enumerator maps known error codes", NetworkShareEnumeratorMapsKnownErrorCodes);
            EndpointCheckingCoreTestRunner.Run("Network share enumerator maps unknown error codes", NetworkShareEnumeratorMapsUnknownErrorCodes);
        }

        private static void NetworkShareEnumeratorMapsKnownShareTypes()
        {
            List<string> shares = EndpointNetworkShareEnumerator.BuildShareItems(new[]
            {
                new EndpointNetworkShareNativeItem { Name = "Docs", TypeCode = 0, Remark = null },
                new EndpointNetworkShareNativeItem { Name = "Print", TypeCode = 1, Remark = null },
                new EndpointNetworkShareNativeItem { Name = "IPC$", TypeCode = 3, Remark = null },
            });

            EndpointCheckingCoreTestRunner.AssertEqual(3, shares.Count);
            EndpointCheckingCoreTestRunner.AssertEqual("[Folder] Docs", shares[0]);
            EndpointCheckingCoreTestRunner.AssertEqual("[Printer] Print", shares[1]);
            EndpointCheckingCoreTestRunner.AssertEqual("[IPC] IPC$", shares[2]);
        }

        private static void NetworkShareEnumeratorMapsUnknownShareTypes()
        {
            List<string> shares = EndpointNetworkShareEnumerator.BuildShareItems(new[]
            {
                new EndpointNetworkShareNativeItem { Name = "Mystery", TypeCode = 999, Remark = null }
            });

            EndpointCheckingCoreTestRunner.AssertEqual(1, shares.Count);
            EndpointCheckingCoreTestRunner.AssertEqual("[Type Code: 999] Mystery", shares[0]);
        }

        private static void NetworkShareEnumeratorPreservesOptionalShareRemarks()
        {
            List<string> shares = EndpointNetworkShareEnumerator.BuildShareItems(new[]
            {
                new EndpointNetworkShareNativeItem { Name = "Media", TypeCode = 0, Remark = "Read only" }
            });

            EndpointCheckingCoreTestRunner.AssertEqual(1, shares.Count);
            EndpointCheckingCoreTestRunner.AssertEqual("[Folder] Media (Read only)", shares[0]);
        }

        private static void NetworkShareEnumeratorMapsKnownErrorCodes()
        {
            EndpointCheckingCoreTestRunner.AssertEqual(
                "The network path was not found",
                EndpointNetworkShareEnumerator.GetErrorMessage(53));
        }

        private static void NetworkShareEnumeratorMapsUnknownErrorCodes()
        {
            EndpointCheckingCoreTestRunner.AssertEqual(
                "Result Code: 7777",
                EndpointNetworkShareEnumerator.GetErrorMessage(7777));
        }
    }
}
