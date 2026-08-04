using System;
using System.Threading;
using EndpointChecker;
using Xunit;

namespace EndpointCheckingCore.Tests
{
    public class UiThreadHelpersTests
    {
        [Fact]
        public void StartBackgroundThread_InvokesBeforeStartAndRunsAction()
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

                Assert.True(done.Wait(TimeSpan.FromSeconds(5)), "Background action did not complete in time.");
            }

            Assert.True(beforeStartCalled);
            Assert.True(actionCalled);
        }

        [Fact]
        public void StartBackgroundThread_RunsOnBackgroundThread()
        {
            bool isBackground = false;
            using (ManualResetEventSlim done = new ManualResetEventSlim(false))
            {
                UiThreadHelpers.StartBackgroundThread(() =>
                {
                    isBackground = Thread.CurrentThread.IsBackground;
                    done.Set();
                });

                Assert.True(done.Wait(TimeSpan.FromSeconds(5)), "Background action did not complete in time.");
            }

            Assert.True(isBackground);
        }

        [Fact]
        public void SafeInvoke_InvokesBeforeInvokeAndAction()
        {
            bool beforeInvokeCalled = false;
            bool actionCalled = false;

            UiThreadHelpers.SafeInvoke(
                () => actionCalled = true,
                () => beforeInvokeCalled = true);

            Assert.True(beforeInvokeCalled);
            Assert.True(actionCalled);
        }

        [Fact]
        public void SafeInvoke_SwallowsExceptionFromInvokeAction()
        {
            Exception exception = Record.Exception(() =>
                UiThreadHelpers.SafeInvoke(() => throw new InvalidOperationException("expected")));

            Assert.Null(exception);
        }

        [Fact]
        public void SafeInvoke_SwallowsExceptionFromBeforeInvoke()
        {
            bool actionCalled = false;

            Exception exception = Record.Exception(() =>
                UiThreadHelpers.SafeInvoke(
                    () => actionCalled = true,
                    () => throw new InvalidOperationException("expected")));

            Assert.Null(exception);
            Assert.False(actionCalled);
        }
    }
}
