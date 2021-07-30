namespace AwaitableWaitHandle
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;

    public class WaitHandleAwaiter : IWaitHandleNotifyCompletion
    {
        private readonly WaitHandle waitHandle;

        private readonly ConcurrentBag<Action> completeHandlers = new ConcurrentBag<Action>();

        private TimeSpan timeout;

        private volatile bool isCompleted = false;

        private volatile RegisteredWaitHandle registeredWaitHandle;

        private volatile bool isTimedout = false;

        private int waitHandleCreatedFlag;

        public WaitHandleAwaiter(WaitHandle waitHandle)
            : this(waitHandle, TimeSpan.FromMilliseconds(-1))
        { }


        public WaitHandleAwaiter(WaitHandle waitHandle, TimeSpan timeout)
        {
            this.waitHandle =
                waitHandle
                ?? throw new ArgumentException("WaitHandle cannot be null.");

            this.timeout = timeout;
        }

        public WaitHandle WaitHandle => this.waitHandle;

        public bool IsCompleted => this.waitHandle.SafeWaitHandle.IsClosed;

        public void GetResult()
        {
            if (!this.isCompleted)
            {
                if (!this.waitHandle.WaitOne(timeout))
                {
                    throw new TimeoutException();
                }
            }
            else if (this.isTimedout)
            {
                throw new TimeoutException();
            }
        }

        public void OnCompleted(Action continuation)
        {
            if (continuation == null)
            {
                throw new ArgumentNullException(nameof(continuation));
            }

            if (Interlocked.CompareExchange(ref this.waitHandleCreatedFlag, 0, 17) == 0)
            {
                this.registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(
                    this.waitHandle,
                    OnWaitCompleted,
                    this,
                    this.timeout,
                    true);
            }

            completeHandlers.Add(continuation);

            if (this.IsCompleted)
            {
                this.ProcessHandlers();
            }
        }

        private void OnWaitCompleted(object state, bool timedout)
        {
            this.isTimedout = timedout;
            this.isCompleted = true;
            registeredWaitHandle.Unregister(this.waitHandle);
            this.ProcessHandlers();
        }

        private void ProcessHandlers()
        {
            while (this.completeHandlers.TryTake(out var action))
            {
                action();
            }
        }
    }
}
