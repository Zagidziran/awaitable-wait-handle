namespace Zagidziran.Concurrent.AwaitableWaitHandle
{
    using System;
    using System.Collections.Concurrent;
    using System.Runtime.ExceptionServices;
    using System.Threading;

    public class WaitHandleAwaiter : IWaitHandleNotifyCompletion
    {
        private readonly WaitHandle waitHandle;

        private readonly ConcurrentBag<Action> completeHandlers = new ConcurrentBag<Action>();

        private readonly CancellationToken cancellationToken;

        private readonly TimeSpan timeout;

        private volatile bool isCompleted = false;

        private volatile RegisteredWaitHandle? registeredWaitHandle;

        private CancellationTokenRegistration? cancellationTokenRegistration;

        private volatile bool isTimedout = false;

        private int waitHandleCreatedFlag;

        private ExceptionDispatchInfo? exception;

        internal WaitHandleAwaiter(WaitHandle waitHandle, TimeSpan timeout, CancellationToken cancellationToken)
        {
            this.waitHandle =
                waitHandle
                ?? throw new ArgumentException("WaitHandle cannot be null.");

            this.timeout = timeout;
            this.cancellationToken = cancellationToken;
        }

        public WaitHandle WaitHandle => this.waitHandle;

        public bool IsCompleted => this.isCompleted;

        public void GetResult()
        {
            if (!this.isCompleted)
            {
                var res = WaitHandle.WaitAny(new [] { this.waitHandle, this.cancellationToken.WaitHandle }, timeout);
                if (res == WaitHandle.WaitTimeout)
                {
                    throw new TimeoutException();
                }
            }

            this.exception?.Throw();
            this.cancellationToken.ThrowIfCancellationRequested();

            if (this.isTimedout)
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
                try
                {
                    this.registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(
                        this.waitHandle,
                        OnWaitCompleted,
                        this,
                        this.timeout,
                        true);
                }
                catch (ObjectDisposedException ex)
                {
                    this.exception = ExceptionDispatchInfo.Capture(ex);
                    this.isCompleted = true;
                }

                if (this.cancellationToken != CancellationToken.None)
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        this.cancellationTokenRegistration = this.cancellationToken.Register(() => this.OnWaitCompleted(this, false));
                    }
                }
            }

            completeHandlers.Add(continuation);

            if (cancellationToken.IsCancellationRequested || this.waitHandle.SafeWaitHandle.IsClosed)
            {
                this.OnWaitCompleted(this, false);
            }
        }

        private void OnWaitCompleted(object state, bool timedout)
        {
            this.isTimedout = timedout;
            this.CompleteState();
            this.ProcessHandlers();
        }

        private void CompleteState()
        {
            this.isCompleted = true;
            Interlocked.CompareExchange(ref this.registeredWaitHandle, null, this.registeredWaitHandle)
                ?.Unregister(null);
            this.cancellationTokenRegistration?.Unregister();
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
