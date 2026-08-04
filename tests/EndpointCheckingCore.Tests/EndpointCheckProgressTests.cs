using System;

namespace EndpointChecker
{
    internal static class EndpointCheckProgressTests
    {
        public static void Register()
        {
            Run("Queued endpoint does not increment completed progress", QueuedEndpointDoesNotIncrementCompletedProgress);
            Run("Starting endpoint does not increment completed progress", StartingEndpointDoesNotIncrementCompletedProgress);
            Run("Completing endpoint increments completed progress once", CompletingEndpointIncrementsCompletedProgressOnce);
            Run("Terminal failure increments completed progress once", TerminalFailureIncrementsCompletedProgressOnce);
            Run("Progress never exceeds total endpoints", ProgressNeverExceedsTotalEndpoints);
            Run("Completed count remains below total while checks remain active", CompletedCountRemainsBelowTotalWhileChecksRemainActive);
            Run("Progress reaches total only after final endpoint terminates", ProgressReachesTotalOnlyAfterFinalEndpointTerminates);
            Run("Cancellation terminal state increments completed progress once", CancellationTerminalStateIncrementsCompletedProgressOnce);
            Run("Mixed fast and slow endpoint completion keeps active worker count accurate", MixedFastAndSlowEndpointCompletionKeepsActiveWorkerCountAccurate);
            Run("Final active worker drain reaches zero only after last completion", FinalActiveWorkerDrainReachesZeroOnlyAfterLastCompletion);
        }

        private static void QueuedEndpointDoesNotIncrementCompletedProgress()
        {
            EndpointCheckProgress progress = new EndpointCheckProgress(1);

            EndpointCheckProgressSnapshot snapshot = progress.Snapshot;

            EndpointCheckingCoreTestRunner.AssertEqual(1, snapshot.TotalCount);
            EndpointCheckingCoreTestRunner.AssertEqual(0, snapshot.CompletedCount);
            EndpointCheckingCoreTestRunner.AssertEqual(0, snapshot.ActiveCount);
        }

        private static void StartingEndpointDoesNotIncrementCompletedProgress()
        {
            EndpointCheckProgress progress = new EndpointCheckProgress(1);

            progress.StartEndpoint();
            EndpointCheckProgressSnapshot snapshot = progress.Snapshot;

            EndpointCheckingCoreTestRunner.AssertEqual(0, snapshot.CompletedCount);
            EndpointCheckingCoreTestRunner.AssertEqual(1, snapshot.ActiveCount);
        }

        private static void CompletingEndpointIncrementsCompletedProgressOnce()
        {
            EndpointCheckProgress progress = new EndpointCheckProgress(1);
            EndpointCheckProgressWorkItem workItem = progress.StartEndpoint();

            EndpointCheckProgressSnapshot snapshot = progress.CompleteEndpoint(workItem);

            EndpointCheckingCoreTestRunner.AssertEqual(1, snapshot.CompletedCount);
            EndpointCheckingCoreTestRunner.AssertEqual(0, snapshot.ActiveCount);

            snapshot = progress.CompleteEndpoint(workItem);

            EndpointCheckingCoreTestRunner.AssertEqual(1, snapshot.CompletedCount);
            EndpointCheckingCoreTestRunner.AssertEqual(0, snapshot.ActiveCount);
        }

        private static void TerminalFailureIncrementsCompletedProgressOnce()
        {
            EndpointCheckProgress progress = new EndpointCheckProgress(2);
            EndpointCheckProgressWorkItem successful = progress.StartEndpoint();
            EndpointCheckProgressWorkItem failed = progress.StartEndpoint();

            progress.CompleteEndpoint(successful);
            EndpointCheckProgressSnapshot snapshot = progress.CompleteEndpoint(failed);

            EndpointCheckingCoreTestRunner.AssertEqual(2, snapshot.CompletedCount);
            EndpointCheckingCoreTestRunner.AssertEqual(0, snapshot.ActiveCount);
        }

        private static void ProgressNeverExceedsTotalEndpoints()
        {
            EndpointCheckProgress progress = new EndpointCheckProgress(1);
            EndpointCheckProgressWorkItem first = progress.StartEndpoint();
            EndpointCheckProgressWorkItem second = progress.StartEndpoint();

            progress.CompleteEndpoint(first);
            EndpointCheckProgressSnapshot snapshot = progress.CompleteEndpoint(second);

            EndpointCheckingCoreTestRunner.AssertEqual(1, snapshot.CompletedCount);
            EndpointCheckingCoreTestRunner.AssertEqual(0, snapshot.ActiveCount);
        }

        private static void CompletedCountRemainsBelowTotalWhileChecksRemainActive()
        {
            EndpointCheckProgress progress = new EndpointCheckProgress(3);
            EndpointCheckProgressWorkItem first = progress.StartEndpoint();
            progress.StartEndpoint();
            progress.StartEndpoint();

            EndpointCheckProgressSnapshot snapshot = progress.CompleteEndpoint(first);

            EndpointCheckingCoreTestRunner.AssertEqual(1, snapshot.CompletedCount);
            EndpointCheckingCoreTestRunner.AssertEqual(2, snapshot.ActiveCount);
            if (snapshot.CompletedCount >= snapshot.TotalCount)
            {
                throw new InvalidOperationException("Completed count reached total while checks were still active.");
            }
        }

        private static void ProgressReachesTotalOnlyAfterFinalEndpointTerminates()
        {
            EndpointCheckProgress progress = new EndpointCheckProgress(2);
            EndpointCheckProgressWorkItem first = progress.StartEndpoint();
            EndpointCheckProgressWorkItem second = progress.StartEndpoint();

            EndpointCheckProgressSnapshot snapshot = progress.CompleteEndpoint(first);
            EndpointCheckingCoreTestRunner.AssertEqual(1, snapshot.CompletedCount);

            snapshot = progress.CompleteEndpoint(second);
            EndpointCheckingCoreTestRunner.AssertEqual(2, snapshot.CompletedCount);
            EndpointCheckingCoreTestRunner.AssertEqual(0, snapshot.ActiveCount);
        }

        private static void CancellationTerminalStateIncrementsCompletedProgressOnce()
        {
            EndpointCheckProgress progress = new EndpointCheckProgress(1);
            EndpointCheckProgressWorkItem cancelled = progress.StartEndpoint();

            EndpointCheckProgressSnapshot snapshot = progress.CompleteEndpoint(cancelled);

            EndpointCheckingCoreTestRunner.AssertEqual(1, snapshot.CompletedCount);
            EndpointCheckingCoreTestRunner.AssertEqual(0, snapshot.ActiveCount);
        }

        private static void MixedFastAndSlowEndpointCompletionKeepsActiveWorkerCountAccurate()
        {
            EndpointCheckProgress progress = new EndpointCheckProgress(3);
            EndpointCheckProgressWorkItem fast = progress.StartEndpoint();
            EndpointCheckProgressWorkItem slow = progress.StartEndpoint();
            EndpointCheckProgressWorkItem medium = progress.StartEndpoint();

            EndpointCheckProgressSnapshot snapshot = progress.CompleteEndpoint(fast);
            EndpointCheckingCoreTestRunner.AssertEqual(1, snapshot.CompletedCount);
            EndpointCheckingCoreTestRunner.AssertEqual(2, snapshot.ActiveCount);

            snapshot = progress.CompleteEndpoint(medium);
            EndpointCheckingCoreTestRunner.AssertEqual(2, snapshot.CompletedCount);
            EndpointCheckingCoreTestRunner.AssertEqual(1, snapshot.ActiveCount);

            snapshot = progress.CompleteEndpoint(slow);
            EndpointCheckingCoreTestRunner.AssertEqual(3, snapshot.CompletedCount);
            EndpointCheckingCoreTestRunner.AssertEqual(0, snapshot.ActiveCount);
        }

        private static void FinalActiveWorkerDrainReachesZeroOnlyAfterLastCompletion()
        {
            EndpointCheckProgress progress = new EndpointCheckProgress(2);
            EndpointCheckProgressWorkItem first = progress.StartEndpoint();
            EndpointCheckProgressWorkItem second = progress.StartEndpoint();

            EndpointCheckProgressSnapshot snapshot = progress.CompleteEndpoint(first);
            EndpointCheckingCoreTestRunner.AssertEqual(1, snapshot.ActiveCount);

            snapshot = progress.CompleteEndpoint(second);
            EndpointCheckingCoreTestRunner.AssertEqual(0, snapshot.ActiveCount);
            EndpointCheckingCoreTestRunner.AssertEqual(2, snapshot.CompletedCount);
        }

        private static void Run(string name, Action test) => EndpointCheckingCoreTestRunner.Run(name, test);
    }
}
