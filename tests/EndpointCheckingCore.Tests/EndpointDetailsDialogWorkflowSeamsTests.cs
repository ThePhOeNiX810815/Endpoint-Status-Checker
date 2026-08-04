namespace EndpointChecker
{
    internal static class EndpointDetailsDialogWorkflowSeamsTests
    {
        public static void Register()
        {
            EndpointCheckingCoreTestRunner.Run("Favicon lookup plan includes direct and fallback request URLs", FaviconLookupPlanIncludesDirectAndFallbackRequestUrls);
            EndpointCheckingCoreTestRunner.Run("Favicon lookup plan avoids recursive fallback expansion", FaviconLookupPlanAvoidsRecursiveFallbackExpansion);
            EndpointCheckingCoreTestRunner.Run("Details dialog cancellation gate returns false when form is alive", DetailsDialogCancellationGateReturnsFalseWhenFormIsAlive);
            EndpointCheckingCoreTestRunner.Run("Details dialog cancellation gate returns true when disposing markers are present", DetailsDialogCancellationGateReturnsTrueWhenDisposingMarkersArePresent);
            EndpointCheckingCoreTestRunner.Run("MAC vendor presentation maps company address and vendor link", MacVendorPresentationMapsCompanyAddressAndVendorLink);
            EndpointCheckingCoreTestRunner.Run("MAC vendor presentation preserves not available fallback", MacVendorPresentationPreservesNotAvailableFallback);
        }

        private static void FaviconLookupPlanIncludesDirectAndFallbackRequestUrls()
        {
            string[] urls = EndpointFaviconLookupPlanBuilder.BuildRequestUrls(
                "http://example.com",
                "http://www.google.com/s2/favicons?domain=");

            EndpointCheckingCoreTestRunner.AssertEqual(2, urls.Length);
            EndpointCheckingCoreTestRunner.AssertEqual("http://example.com/favicon.ico", urls[0]);
            EndpointCheckingCoreTestRunner.AssertEqual("http://www.google.com/s2/favicons?domain=http://example.com/favicon.ico", urls[1]);
        }

        private static void FaviconLookupPlanAvoidsRecursiveFallbackExpansion()
        {
            string[] urls = EndpointFaviconLookupPlanBuilder.BuildRequestUrls(
                "http://www.google.com/s2/favicons?domain=http://example.com",
                "http://www.google.com/s2/favicons?domain=");

            EndpointCheckingCoreTestRunner.AssertEqual(1, urls.Length);
            EndpointCheckingCoreTestRunner.AssertEqual("http://www.google.com/s2/favicons?domain=http://example.com/favicon.ico", urls[0]);
        }

        private static void DetailsDialogCancellationGateReturnsFalseWhenFormIsAlive()
        {
            bool cancelled = EndpointDetailsDialogCancellationGate.IsCancellationRequested(
                isDisposed: false,
                isDisposing: false,
                hasCheckerMainForm: true);

            EndpointCheckingCoreTestRunner.AssertEqual(false, cancelled);
        }

        private static void DetailsDialogCancellationGateReturnsTrueWhenDisposingMarkersArePresent()
        {
            bool disposedCancelled = EndpointDetailsDialogCancellationGate.IsCancellationRequested(
                isDisposed: true,
                isDisposing: false,
                hasCheckerMainForm: true);

            bool disposingCancelled = EndpointDetailsDialogCancellationGate.IsCancellationRequested(
                isDisposed: false,
                isDisposing: true,
                hasCheckerMainForm: true);

            bool missingFormCancelled = EndpointDetailsDialogCancellationGate.IsCancellationRequested(
                isDisposed: false,
                isDisposing: false,
                hasCheckerMainForm: false);

            EndpointCheckingCoreTestRunner.AssertEqual(true, disposedCancelled);
            EndpointCheckingCoreTestRunner.AssertEqual(true, disposingCancelled);
            EndpointCheckingCoreTestRunner.AssertEqual(true, missingFormCancelled);
        }

        private static void MacVendorPresentationMapsCompanyAddressAndVendorLink()
        {
            EndpointMacVendorPresentationModel model = EndpointMacVendorPresentationBuilder.Build(new EndpointMacVendorPresentationInput
            {
                StatusNotAvailable = "N/A",
                Company = "Mega Corp",
                Address = "Road   1",
                Domain = "mega.example",
                HasVendorImage = true,
            });

            EndpointCheckingCoreTestRunner.AssertEqual("Mega Corp (Road 1)", model.VendorText);
            EndpointCheckingCoreTestRunner.AssertEqual(true, model.ShouldApplyVendorImage);
            EndpointCheckingCoreTestRunner.AssertEqual(true, model.ShouldEnableVendorLink);
            EndpointCheckingCoreTestRunner.AssertEqual("http://mega.example", model.VendorWebPage);
            EndpointCheckingCoreTestRunner.AssertEqual("Click to open \"Mega Corp\" web page (mega.example)", model.VendorTooltipText);
        }

        private static void MacVendorPresentationPreservesNotAvailableFallback()
        {
            EndpointMacVendorPresentationModel model = EndpointMacVendorPresentationBuilder.Build(new EndpointMacVendorPresentationInput
            {
                StatusNotAvailable = "N/A",
                Company = null,
                Address = null,
                Domain = null,
                HasVendorImage = false,
            });

            EndpointCheckingCoreTestRunner.AssertEqual("N/A", model.VendorText);
            EndpointCheckingCoreTestRunner.AssertEqual(false, model.ShouldApplyVendorImage);
            EndpointCheckingCoreTestRunner.AssertEqual(false, model.ShouldEnableVendorLink);
        }
    }
}
