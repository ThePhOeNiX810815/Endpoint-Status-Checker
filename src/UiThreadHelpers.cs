using System;
using System.Threading;

namespace EndpointChecker
{
    internal static class UiThreadHelpers
    {
        public static void StartBackgroundThread(Action action, Action beforeStart = null)
        {
            if (beforeStart != null)
            {
                beforeStart();
            }

            Thread thread = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                action();
            });

            thread.Start();
        }

        public static void SafeInvoke(Action invokeAction, Action beforeInvoke = null)
        {
            try
            {
                if (beforeInvoke != null)
                {
                    beforeInvoke();
                }

                invokeAction();
            }
            catch
            {
            }
        }
    }
}
