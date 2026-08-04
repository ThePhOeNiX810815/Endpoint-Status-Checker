namespace EndpointChecker
{
    internal static class EndpointVirusTotalCancellationGate
    {
        public static bool IsCancellationRequested(
            bool isCancelled,
            bool isDisposed,
            bool isDisposing,
            bool hasCheckerMainForm)
        {
            return isCancelled ||
                isDisposed ||
                isDisposing ||
                !hasCheckerMainForm;
        }
    }
}