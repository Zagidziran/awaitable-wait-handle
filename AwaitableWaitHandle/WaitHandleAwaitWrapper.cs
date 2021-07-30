namespace AwaitableWaitHandle
{
    using System;
    using System.Threading;

    public class WaitHandleAwaitWrapper
    {
        internal WaitHandleAwaitWrapper(WaitHandle waitHandle, TimeSpan timeout)
        {
            this.WaitHandle = waitHandle;
            this.Timeout = timeout;
        }

        internal WaitHandle WaitHandle { get; }

        internal TimeSpan Timeout { get; }
    }
}
