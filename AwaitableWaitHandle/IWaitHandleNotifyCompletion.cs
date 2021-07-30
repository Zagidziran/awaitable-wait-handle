namespace AwaitableWaitHandle
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IWaitHandleNotifyCompletion : INotifyCompletion
    {
        public WaitHandle WaitHandle { get; }

        public bool IsCompleted { get; }

        public void GetResult();
    }
}
