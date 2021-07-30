namespace AwaitableWaitHandle
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public static class WaitHandleExtensions
    {
        public static IWaitHandleNotifyCompletion GetAwaiter(this WaitHandle waitHandle) => new WaitHandleAwaiter(waitHandle);

        public static WaitHandleAwaitWrapper WithTimeout(this WaitHandle waitHandle, TimeSpan timeout)
            => new WaitHandleAwaitWrapper(waitHandle, timeout);

        public static IWaitHandleNotifyCompletion GetAwaiter(this WaitHandleAwaitWrapper waitHandleWrapper) => new WaitHandleAwaiter(waitHandleWrapper.WaitHandle, waitHandleWrapper.Timeout);
    }
}
