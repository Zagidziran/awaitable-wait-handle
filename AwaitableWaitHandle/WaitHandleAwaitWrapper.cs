namespace Zagidziran.Concurrent.AwaitableWaitHandle
{
    using System;
    using System.Threading;

    public class WaitHandleAwaitWrapper
    {
        internal WaitHandleAwaitWrapper(WaitHandle waitHandle, TimeSpan timeout, CancellationToken cancellationToken)
        {
            this.WaitHandle = waitHandle;
            this.Timeout = timeout;
            this.CancellationToken = cancellationToken;
        }

        internal WaitHandle WaitHandle { get; }

        internal TimeSpan Timeout { get; }

        internal CancellationToken CancellationToken { get; }
    }
}
