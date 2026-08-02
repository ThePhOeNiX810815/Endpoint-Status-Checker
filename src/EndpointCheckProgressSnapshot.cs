namespace EndpointChecker
{
    internal sealed class EndpointCheckProgressSnapshot
    {
        public EndpointCheckProgressSnapshot(int totalCount, int completedCount, int activeCount)
        {
            TotalCount = totalCount;
            CompletedCount = completedCount;
            ActiveCount = activeCount;
        }

        public int TotalCount { get; }

        public int CompletedCount { get; }

        public int ActiveCount { get; }
    }
}
