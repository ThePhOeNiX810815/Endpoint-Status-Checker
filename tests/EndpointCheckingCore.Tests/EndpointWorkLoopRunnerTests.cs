using System;
using System.Collections.Generic;

namespace EndpointChecker
{
    internal static class EndpointWorkLoopRunnerTests
    {
        public static void Register()
        {
            EndpointCheckingCoreTestRunner.Run("Work loop runner processes all items when no stop signal", ProcessesAllItemsWhenStopSignalIsFalse);
            EndpointCheckingCoreTestRunner.Run("Work loop runner stops before next item when stop signal turns true", StopsBeforeNextItemWhenStopSignalTurnsTrue);
            EndpointCheckingCoreTestRunner.Run("Work loop runner reports per-item progress without duplication", ReportsPerItemProgressWithoutDuplication);
            EndpointCheckingCoreTestRunner.Run("Work loop runner throws when process callback is missing", ThrowsWhenProcessCallbackIsMissing);
            EndpointCheckingCoreTestRunner.Run("Work loop runner propagates process exceptions", PropagatesProcessExceptions);
        }

        private static void ProcessesAllItemsWhenStopSignalIsFalse()
        {
            List<int> processed = new List<int>();

            EndpointWorkLoopRunResult result = EndpointWorkLoopRunner.Run(
                new[] { 1, 2, 3 },
                () => false,
                value => processed.Add(value));

            EndpointCheckingCoreTestRunner.AssertEqual(3, result.ProcessedCount);
            EndpointCheckingCoreTestRunner.AssertEqual(false, result.TerminatedEarly);
            EndpointCheckingCoreTestRunner.AssertArray(new[] { "1", "2", "3" }, new[]
            {
                processed[0].ToString(),
                processed[1].ToString(),
                processed[2].ToString()
            });
        }

        private static void StopsBeforeNextItemWhenStopSignalTurnsTrue()
        {
            int processedCount = 0;

            EndpointWorkLoopRunResult result = EndpointWorkLoopRunner.Run(
                new[] { 10, 20, 30, 40 },
                () => processedCount >= 2,
                _ => processedCount++);

            EndpointCheckingCoreTestRunner.AssertEqual(2, result.ProcessedCount);
            EndpointCheckingCoreTestRunner.AssertEqual(true, result.TerminatedEarly);
            EndpointCheckingCoreTestRunner.AssertEqual(2, processedCount);
        }

        private static void ReportsPerItemProgressWithoutDuplication()
        {
            int callbackCount = 0;
            int lastProcessed = 0;
            int lastTotal = 0;

            EndpointWorkLoopRunResult result = EndpointWorkLoopRunner.Run(
                new[] { "a", "b", "c" },
                () => false,
                _ => { },
                (processed, total) =>
                {
                    callbackCount++;
                    lastProcessed = processed;
                    lastTotal = total;
                });

            EndpointCheckingCoreTestRunner.AssertEqual(3, result.ProcessedCount);
            EndpointCheckingCoreTestRunner.AssertEqual(false, result.TerminatedEarly);
            EndpointCheckingCoreTestRunner.AssertEqual(3, callbackCount);
            EndpointCheckingCoreTestRunner.AssertEqual(3, lastProcessed);
            EndpointCheckingCoreTestRunner.AssertEqual(3, lastTotal);
        }

        private static void ThrowsWhenProcessCallbackIsMissing()
        {
            bool threw = false;

            try
            {
                EndpointWorkLoopRunner.Run(new[] { 1 }, () => false, null);
            }
            catch (ArgumentNullException ex)
            {
                threw = ex.ParamName == "processItem";
            }

            EndpointCheckingCoreTestRunner.AssertEqual(true, threw);
        }

        private static void PropagatesProcessExceptions()
        {
            bool threw = false;

            try
            {
                EndpointWorkLoopRunner.Run(
                    new[] { 1, 2 },
                    () => false,
                    value =>
                    {
                        if (value == 2)
                        {
                            throw new InvalidOperationException("expected");
                        }
                    });
            }
            catch (InvalidOperationException ex)
            {
                threw = ex.Message == "expected";
            }

            EndpointCheckingCoreTestRunner.AssertEqual(true, threw);
        }
    }
}
