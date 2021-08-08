namespace Zagidziran.Concurrent.AwaitableWaitHandle
{
    using System;
    using System.Threading;

    public static class WaitHandleExtensions
    {
        public static IWaitHandleNotifyCompletion GetAwaiter(this WaitHandle waitHandle)
            => new WaitHandleAwaiter(waitHandle, TimeSpan.FromMilliseconds(-1), CancellationToken.None);

        public static IWaitHandleNotifyCompletion GetAwaiter(this WaitHandleAwaitWrapper waitHandleWrapper) =>
            new WaitHandleAwaiter(waitHandleWrapper.WaitHandle, waitHandleWrapper.Timeout, waitHandleWrapper.CancellationToken);

        public static WaitHandleAwaitWrapper WithTimeout(this WaitHandle waitHandle, TimeSpan timeout)
            => new WaitHandleAwaitWrapper(waitHandle, timeout, CancellationToken.None);
        public static WaitHandleAwaitWrapper WithCancellation(this WaitHandle waitHandle, CancellationToken cancellationToken)
            => new WaitHandleAwaitWrapper(waitHandle, TimeSpan.FromMilliseconds(-1), cancellationToken);

        public static WaitHandleAwaitWrapper WithTimeout(this WaitHandleAwaitWrapper waitHandleWrapper, TimeSpan timeout)
            => new WaitHandleAwaitWrapper(
                waitHandleWrapper.WaitHandle,
                waitHandleWrapper.Timeout == TimeSpan.FromMilliseconds(-1) 
                ? timeout
                : throw new ArgumentException("Timeout already configured.", nameof(timeout)),
                waitHandleWrapper.CancellationToken);


        public static WaitHandleAwaitWrapper WithCancellation(this WaitHandleAwaitWrapper waitHandleWrapper, CancellationToken cancellationToken)
            => new WaitHandleAwaitWrapper(
                waitHandleWrapper.WaitHandle,
                waitHandleWrapper.Timeout, 
                waitHandleWrapper.CancellationToken == CancellationToken.None ? cancellationToken : throw new ArgumentException("Cancellation already configured.", nameof(cancellationToken)));

    }
}
