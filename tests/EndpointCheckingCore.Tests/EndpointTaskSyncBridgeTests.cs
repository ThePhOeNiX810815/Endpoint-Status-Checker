using System;
using System.Threading.Tasks;

namespace EndpointChecker
{
    internal static class EndpointTaskSyncBridgeTests
    {
        public static void Register()
        {
            EndpointCheckingCoreTestRunner.Run("Task sync bridge returns completed task result", TaskSyncBridgeReturnsCompletedTaskResult);
            EndpointCheckingCoreTestRunner.Run("Task sync bridge unwraps inner exception type", TaskSyncBridgeUnwrapsInnerExceptionType);
            EndpointCheckingCoreTestRunner.Run("Task sync bridge throws for null task", TaskSyncBridgeThrowsForNullTask);
        }

        private static void TaskSyncBridgeReturnsCompletedTaskResult()
        {
            int value = EndpointTaskSyncBridge.AwaitResult(Task.FromResult(42));
            EndpointCheckingCoreTestRunner.AssertEqual(42, value);
        }

        private static void TaskSyncBridgeUnwrapsInnerExceptionType()
        {
            Task<int> faulted = Task.FromException<int>(new InvalidOperationException("boom"));

            try
            {
                EndpointTaskSyncBridge.AwaitResult(faulted);
                throw new InvalidOperationException("Expected exception was not thrown.");
            }
            catch (InvalidOperationException ex)
            {
                EndpointCheckingCoreTestRunner.AssertEqual("boom", ex.Message);
            }
        }

        private static void TaskSyncBridgeThrowsForNullTask()
        {
            try
            {
                EndpointTaskSyncBridge.AwaitResult<int>(null);
                throw new InvalidOperationException("Expected ArgumentNullException was not thrown.");
            }
            catch (ArgumentNullException ex)
            {
                EndpointCheckingCoreTestRunner.AssertEqual("task", ex.ParamName);
            }
        }
    }
}
