namespace Zagidziran.Concurrent.AwaitableWaitHandle
{
    using System;
    using System.Threading;

    public static class WaitHandleExtensions
    {
        public static IWaitHandleNotifyCompletion GetAwaiter(this WaitHandle waitHandle) => new WaitHandleAwaiter(waitHandle);

        public static WaitHandleAwaitWrapper WithTimeout(this WaitHandle waitHandle, TimeSpan timeout)
            => new WaitHandleAwaitWrapper(waitHandle, timeout);

        public static IWaitHandleNotifyCompletion GetAwaiter(this WaitHandleAwaitWrapper waitHandleWrapper) => new WaitHandleAwaiter(waitHandleWrapper.WaitHandle, waitHandleWrapper.Timeout);
    }
}
