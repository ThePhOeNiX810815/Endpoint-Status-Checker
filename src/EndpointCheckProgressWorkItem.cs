using System.Threading;

namespace EndpointChecker
{
    internal sealed class EndpointCheckProgressWorkItem
    {
        private int completed;

        public bool TryComplete()
        {
            return Interlocked.Exchange(ref completed, 1) == 0;
        }
    }
}
