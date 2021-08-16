namespace Zagidziran.Concurrent.SpinMonitor
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using Zagidziran.Concurrent.SpinMonitor.Exceptions;

    public static class SpinMonitor
    {
        private static ConcurrentDictionary<object, LockHandle> lockOwners =
            new ConcurrentDictionary<object, LockHandle>();

        public static IDisposable Enter(object lockObject) =>
            Enter(lockObject, TimeSpan.FromMilliseconds(-1), CancellationToken.None);

        public static IDisposable Enter(object lockObject, TimeSpan timeout) =>
            Enter(lockObject, timeout, CancellationToken.None);

        public static IDisposable Enter(object lockObject, CancellationToken cancellationToken) =>
            Enter(lockObject, TimeSpan.FromMilliseconds(-1), cancellationToken);

        public static IDisposable Enter(object lockObject, TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (lockObject == null)
            {
                throw new ArgumentNullException(nameof(lockObject));
            }

            var spinWait = new SpinWait();
            var lockHandle = new LockHandle(lockObject);
            var sw = Stopwatch.StartNew();
            while (!lockOwners.TryAdd(lockObject, lockHandle))
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (timeout.TotalMilliseconds >= 0 && timeout <= sw.Elapsed)
                {
                    throw new TimeoutException();
                }

                spinWait.SpinOnce();
            }

            return lockHandle;
        }

        internal static void Exit(LockHandle handle)
        {
            if (!lockOwners.TryRemove(new KeyValuePair<object, LockHandle>(handle.LockObject, handle)))
            {
                throw new LockNotOwnedException();
            }
        }
    }
}
