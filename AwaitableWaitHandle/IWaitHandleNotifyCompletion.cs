namespace Zagidziran.Concurrent.AwaitableWaitHandle
{
    using System.Runtime.CompilerServices;
    using System.Threading;

    public interface IWaitHandleNotifyCompletion : INotifyCompletion
    {
        public WaitHandle WaitHandle { get; }

        public bool IsCompleted { get; }

        public void GetResult();
    }
}
