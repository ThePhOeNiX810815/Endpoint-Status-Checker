using System;
using System.Threading;

namespace EndpointChecker
{
    internal static class UiThreadHelpersTests
    {
        public static void Register()
        {
            EndpointCheckingCoreTestRunner.Run("UI thread helper start invokes before-start callback and background action", StartBackgroundThreadInvokesBeforeStartAndRunsAction);
            EndpointCheckingCoreTestRunner.Run("UI thread helper start runs action on background thread", StartBackgroundThreadRunsOnBackgroundThread);
            EndpointCheckingCoreTestRunner.Run("UI thread helper safe invoke runs before-invoke callback and action", SafeInvokeInvokesBeforeInvokeAndAction);
            EndpointCheckingCoreTestRunner.Run("UI thread helper safe invoke swallows invoke exceptions", SafeInvokeSwallowsExceptionFromInvokeAction);
            EndpointCheckingCoreTestRunner.Run("UI thread helper safe invoke swallows before-invoke exceptions", SafeInvokeSwallowsExceptionFromBeforeInvoke);
        }

        private static void StartBackgroundThreadInvokesBeforeStartAndRunsAction()
        {
            bool beforeStartCalled = false;
            bool actionCalled = false;
            using (ManualResetEventSlim done = new ManualResetEventSlim(false))
            {
                UiThreadHelpers.StartBackgroundThread(
                    () =>
                    {
                        actionCalled = true;
                        done.Set();
                    },
                    () => beforeStartCalled = true);

                if (!done.Wait(TimeSpan.FromSeconds(5)))
                {
                    throw new InvalidOperationException("Background action did not complete in time.");
                }
            }

            EndpointCheckingCoreTestRunner.AssertEqual(true, beforeStartCalled);
            EndpointCheckingCoreTestRunner.AssertEqual(true, actionCalled);
        }

        private static void StartBackgroundThreadRunsOnBackgroundThread()
        {
            bool isBackground = false;
            using (ManualResetEventSlim done = new ManualResetEventSlim(false))
            {
                UiThreadHelpers.StartBackgroundThread(() =>
                {
                    isBackground = Thread.CurrentThread.IsBackground;
                    done.Set();
                });

                if (!done.Wait(TimeSpan.FromSeconds(5)))
                {
                    throw new InvalidOperationException("Background action did not complete in time.");
                }
            }

            EndpointCheckingCoreTestRunner.AssertEqual(true, isBackground);
        }

        private static void SafeInvokeInvokesBeforeInvokeAndAction()
        {
            bool beforeInvokeCalled = false;
            bool actionCalled = false;

            UiThreadHelpers.SafeInvoke(
                () => actionCalled = true,
                () => beforeInvokeCalled = true);

            EndpointCheckingCoreTestRunner.AssertEqual(true, beforeInvokeCalled);
            EndpointCheckingCoreTestRunner.AssertEqual(true, actionCalled);
        }

        private static void SafeInvokeSwallowsExceptionFromInvokeAction()
        {
            try
            {
                UiThreadHelpers.SafeInvoke(() => throw new InvalidOperationException("expected"));
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException("SafeInvoke should swallow invoke exceptions.", exception);
            }
        }

        private static void SafeInvokeSwallowsExceptionFromBeforeInvoke()
        {
            bool actionCalled = false;

            try
            {
                UiThreadHelpers.SafeInvoke(
                    () => actionCalled = true,
                    () => throw new InvalidOperationException("expected"));
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException("SafeInvoke should swallow before-invoke exceptions.", exception);
            }

            EndpointCheckingCoreTestRunner.AssertEqual(false, actionCalled);
        }
    }
}
