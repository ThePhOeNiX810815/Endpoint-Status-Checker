namespace EndpointChecker
{
    internal static class EndpointDetailsDialogCancellationGate
    {
        public static bool IsCancellationRequested(
            bool isDisposed,
            bool isDisposing,
            bool hasCheckerMainForm)
        {
            return isDisposed ||
                isDisposing ||
                !hasCheckerMainForm;
        }
    }
}
