using System;
using System.Collections.Generic;

namespace EndpointChecker
{
    internal readonly struct EndpointWorkLoopRunResult
    {
        public EndpointWorkLoopRunResult(int processedCount, bool terminatedEarly)
        {
            ProcessedCount = processedCount;
            TerminatedEarly = terminatedEarly;
        }

        public int ProcessedCount { get; }

        public bool TerminatedEarly { get; }
    }

    internal static class EndpointWorkLoopRunner
    {
        public static EndpointWorkLoopRunResult Run<T>(
            IReadOnlyList<T> items,
            Func<bool> shouldStop,
            Action<T> processItem,
            Action<int, int> onItemProcessed = null)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (processItem == null)
            {
                throw new ArgumentNullException(nameof(processItem));
            }

            int totalCount = items.Count;
            int processedCount = 0;

            for (int index = 0; index < totalCount; index++)
            {
                if (shouldStop != null && shouldStop())
                {
                    return new EndpointWorkLoopRunResult(processedCount, true);
                }

                processItem(items[index]);
                processedCount++;

                if (onItemProcessed != null)
                {
                    onItemProcessed(processedCount, totalCount);
                }
            }

            return new EndpointWorkLoopRunResult(processedCount, false);
        }
    }
}