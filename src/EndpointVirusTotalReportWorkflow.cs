namespace EndpointChecker
{
    internal sealed class EndpointVirusTotalReportTransitionInput
    {
        public bool IsReportPresent { get; set; }

        public int ScanCount { get; set; }

        public int Positives { get; set; }
    }

    internal sealed class EndpointVirusTotalReportTransitionModel
    {
        public bool ShouldClearPendingScanResult { get; set; }

        public bool ShouldApplyReportUi { get; set; }

        public bool ShouldShowWaitingStatus { get; set; }

        public bool IsCleanResult { get; set; }
    }

    internal static class EndpointVirusTotalReportWorkflow
    {
        public static EndpointVirusTotalReportTransitionModel Build(EndpointVirusTotalReportTransitionInput input)
        {
            EndpointVirusTotalReportTransitionModel model = new EndpointVirusTotalReportTransitionModel();

            if (input == null)
            {
                return model;
            }

            if (!input.IsReportPresent)
            {
                model.ShouldShowWaitingStatus = true;
                return model;
            }

            model.ShouldClearPendingScanResult = true;

            if (input.ScanCount > 0)
            {
                model.ShouldApplyReportUi = true;
                model.IsCleanResult = input.Positives == 0;
            }

            return model;
        }
    }
}