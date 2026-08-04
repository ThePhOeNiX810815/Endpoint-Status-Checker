using System;
using System.Threading.Tasks;

namespace EndpointChecker
{
    internal static class EndpointTaskSyncBridge
    {
        public static T AwaitResult<T>(Task<T> task)
        {
            if (task == null)
            {
                throw new ArgumentNullException("task");
            }

            // Avoid Task.Result AggregateException wrapping and avoid syncing to a captured context.
            return task.ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
