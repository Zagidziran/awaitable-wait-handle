namespace Zagidziran.Concurrent.SpinMonitor
{
    using System;

    internal class LockHandle : IDisposable
    {
        private readonly object lockObject;

        public LockHandle(object lockObject)
        {
            this.lockObject = lockObject;
        }

        public object LockObject => this.lockObject;

        public void Dispose()
        {
            SpinMonitor.Exit(this);
        }
    }
}
