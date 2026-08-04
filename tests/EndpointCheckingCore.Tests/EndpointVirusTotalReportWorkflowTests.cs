namespace EndpointChecker
{
    internal static class EndpointVirusTotalReportWorkflowTests
    {
        public static void Register()
        {
            EndpointCheckingCoreTestRunner.Run("VirusTotal report workflow keeps waiting status for non-present responses", VirusTotalReportWorkflowKeepsWaitingStatusForNonPresentResponses);
            EndpointCheckingCoreTestRunner.Run("VirusTotal report workflow clears pending scan and applies clean UI for present clean reports", VirusTotalReportWorkflowClearsPendingScanAndAppliesCleanUiForPresentCleanReports);
            EndpointCheckingCoreTestRunner.Run("VirusTotal report workflow clears pending scan and applies infected UI for present infected reports", VirusTotalReportWorkflowClearsPendingScanAndAppliesInfectedUiForPresentInfectedReports);
            EndpointCheckingCoreTestRunner.Run("VirusTotal report workflow preserves legacy no-ui transition when present report has no scans", VirusTotalReportWorkflowPreservesLegacyNoUiTransitionWhenPresentReportHasNoScans);
        }

        private static void VirusTotalReportWorkflowKeepsWaitingStatusForNonPresentResponses()
        {
            EndpointVirusTotalReportTransitionModel model = EndpointVirusTotalReportWorkflow.Build(new EndpointVirusTotalReportTransitionInput
            {
                IsReportPresent = false,
                ScanCount = 0,
                Positives = 0,
            });

            EndpointCheckingCoreTestRunner.AssertEqual(true, model.ShouldShowWaitingStatus);
            EndpointCheckingCoreTestRunner.AssertEqual(false, model.ShouldClearPendingScanResult);
            EndpointCheckingCoreTestRunner.AssertEqual(false, model.ShouldApplyReportUi);
        }

        private static void VirusTotalReportWorkflowClearsPendingScanAndAppliesCleanUiForPresentCleanReports()
        {
            EndpointVirusTotalReportTransitionModel model = EndpointVirusTotalReportWorkflow.Build(new EndpointVirusTotalReportTransitionInput
            {
                IsReportPresent = true,
                ScanCount = 42,
                Positives = 0,
            });

            EndpointCheckingCoreTestRunner.AssertEqual(true, model.ShouldClearPendingScanResult);
            EndpointCheckingCoreTestRunner.AssertEqual(true, model.ShouldApplyReportUi);
            EndpointCheckingCoreTestRunner.AssertEqual(true, model.IsCleanResult);
            EndpointCheckingCoreTestRunner.AssertEqual(false, model.ShouldShowWaitingStatus);
        }

        private static void VirusTotalReportWorkflowClearsPendingScanAndAppliesInfectedUiForPresentInfectedReports()
        {
            EndpointVirusTotalReportTransitionModel model = EndpointVirusTotalReportWorkflow.Build(new EndpointVirusTotalReportTransitionInput
            {
                IsReportPresent = true,
                ScanCount = 10,
                Positives = 2,
            });

            EndpointCheckingCoreTestRunner.AssertEqual(true, model.ShouldClearPendingScanResult);
            EndpointCheckingCoreTestRunner.AssertEqual(true, model.ShouldApplyReportUi);
            EndpointCheckingCoreTestRunner.AssertEqual(false, model.IsCleanResult);
        }

        private static void VirusTotalReportWorkflowPreservesLegacyNoUiTransitionWhenPresentReportHasNoScans()
        {
            EndpointVirusTotalReportTransitionModel model = EndpointVirusTotalReportWorkflow.Build(new EndpointVirusTotalReportTransitionInput
            {
                IsReportPresent = true,
                ScanCount = 0,
                Positives = 0,
            });

            EndpointCheckingCoreTestRunner.AssertEqual(true, model.ShouldClearPendingScanResult);
            EndpointCheckingCoreTestRunner.AssertEqual(false, model.ShouldApplyReportUi);
            EndpointCheckingCoreTestRunner.AssertEqual(false, model.ShouldShowWaitingStatus);
        }
    }

    internal static class EndpointVirusTotalCancellationGateTests
    {
        public static void Register()
        {
            EndpointCheckingCoreTestRunner.Run("VirusTotal cancellation gate returns false when no cancellation signals are present", VirusTotalCancellationGateReturnsFalseWhenNoCancellationSignalsArePresent);
            EndpointCheckingCoreTestRunner.Run("VirusTotal cancellation gate returns true for explicit cancellation flag", VirusTotalCancellationGateReturnsTrueForExplicitCancellationFlag);
            EndpointCheckingCoreTestRunner.Run("VirusTotal cancellation gate returns true when dialog is disposed or disposing", VirusTotalCancellationGateReturnsTrueWhenDialogIsDisposedOrDisposing);
            EndpointCheckingCoreTestRunner.Run("VirusTotal cancellation gate returns true when main form reference is missing", VirusTotalCancellationGateReturnsTrueWhenMainFormReferenceIsMissing);
        }

        private static void VirusTotalCancellationGateReturnsFalseWhenNoCancellationSignalsArePresent()
        {
            bool cancelled = EndpointVirusTotalCancellationGate.IsCancellationRequested(
                isCancelled: false,
                isDisposed: false,
                isDisposing: false,
                hasCheckerMainForm: true);

            EndpointCheckingCoreTestRunner.AssertEqual(false, cancelled);
        }

        private static void VirusTotalCancellationGateReturnsTrueForExplicitCancellationFlag()
        {
            bool cancelled = EndpointVirusTotalCancellationGate.IsCancellationRequested(
                isCancelled: true,
                isDisposed: false,
                isDisposing: false,
                hasCheckerMainForm: true);

            EndpointCheckingCoreTestRunner.AssertEqual(true, cancelled);
        }

        private static void VirusTotalCancellationGateReturnsTrueWhenDialogIsDisposedOrDisposing()
        {
            bool disposedCancelled = EndpointVirusTotalCancellationGate.IsCancellationRequested(
                isCancelled: false,
                isDisposed: true,
                isDisposing: false,
                hasCheckerMainForm: true);

            bool disposingCancelled = EndpointVirusTotalCancellationGate.IsCancellationRequested(
                isCancelled: false,
                isDisposed: false,
                isDisposing: true,
                hasCheckerMainForm: true);

            EndpointCheckingCoreTestRunner.AssertEqual(true, disposedCancelled);
            EndpointCheckingCoreTestRunner.AssertEqual(true, disposingCancelled);
        }

        private static void VirusTotalCancellationGateReturnsTrueWhenMainFormReferenceIsMissing()
        {
            bool cancelled = EndpointVirusTotalCancellationGate.IsCancellationRequested(
                isCancelled: false,
                isDisposed: false,
                isDisposing: false,
                hasCheckerMainForm: false);

            EndpointCheckingCoreTestRunner.AssertEqual(true, cancelled);
        }
    }
}
