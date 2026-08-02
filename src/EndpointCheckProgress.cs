using System.Threading;

namespace EndpointChecker
{
    internal sealed class EndpointCheckProgress
    {
        private int completedCount;
        private int activeCount;

        public EndpointCheckProgress(int totalCount)
        {
            TotalCount = totalCount < 0 ? 0 : totalCount;
        }

        public int TotalCount { get; }

        public EndpointCheckProgressSnapshot Snapshot =>
            new EndpointCheckProgressSnapshot(
                TotalCount,
                Clamp(Volatile.Read(ref completedCount), 0, TotalCount),
                Clamp(Volatile.Read(ref activeCount), 0, TotalCount));

        public EndpointCheckProgressWorkItem StartEndpoint()
        {
            Interlocked.Increment(ref activeCount);
            return new EndpointCheckProgressWorkItem();
        }

        public EndpointCheckProgressSnapshot CompleteEndpoint(EndpointCheckProgressWorkItem workItem)
        {
            if (workItem == null ||
                !workItem.TryComplete())
            {
                return Snapshot;
            }

            int completed = Interlocked.Increment(ref completedCount);

            int active;
            do
            {
                active = Volatile.Read(ref activeCount);
                if (active <= 0)
                {
                    break;
                }
            }
            while (Interlocked.CompareExchange(ref activeCount, active - 1, active) != active);

            if (completed > TotalCount)
            {
                Interlocked.Exchange(ref completedCount, TotalCount);
            }

            return Snapshot;
        }

        private static int Clamp(int value, int minimum, int maximum)
        {
            if (value < minimum)
            {
                return minimum;
            }

            if (value > maximum)
            {
                return maximum;
            }

            return value;
        }
    }
}
