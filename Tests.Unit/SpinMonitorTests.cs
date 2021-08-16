namespace Tests.Unit
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture.Xunit2;
    using FluentAssertions;
    using Xunit;
    using Zagidziran.Concurrent.SpinMonitor;
    using Zagidziran.Concurrent.SpinMonitor.Exceptions;

    public class SpinMonitorTests
    {
        [Theory]
        [AutoData]
        public void ShouldAcquireAndReleaseLock(object lockObject)
        {
            using var _ = SpinMonitor.Enter(lockObject);
        }

        [Theory]
        [AutoData]
        public void ShouldThrowLockNotOwnedException(object lockObject)
        {
            using var _ = SpinMonitor.Enter(lockObject);
            var anotherLockHandle = new LockHandle(lockObject);
            Action dispose = anotherLockHandle.Dispose;
            dispose.Should().Throw<LockNotOwnedException>();
        }

        [Theory]
        [AutoData]
        public async Task ShouldDispatchResourceAcrossTasks(object lockObject)
        {
            await Task.WhenAll(
                Enumerable
                    .Range(0, 10000)
                    .Select(_ => Task.Factory.StartNew(SpinMonitor.Enter(lockObject).Dispose)));
        }

        [Theory]
        [AutoData]
        public void ShouldWaitUntilResourceFree(object lockObject)
        {
            var handle = SpinMonitor.Enter(lockObject);
            var _ = Task.Delay(1).ContinueWith(_ => handle.Dispose());
            using var wait = SpinMonitor.Enter(lockObject);
        }

        [Theory]
        [AutoData]
        public void ShouldThrowOperationCancelledException(object lockObject)
        {
            using var _ = SpinMonitor.Enter(lockObject);
            var cts = new CancellationTokenSource(1);
            Action action = () => SpinMonitor.Enter(lockObject, cts.Token);
            action.Should().Throw<OperationCanceledException>();
        }

        [Theory]
        [AutoData]
        public void ShouldThrowTimeoutException(object lockObject)
        {
            using var _ = SpinMonitor.Enter(lockObject);
            Action action = () => SpinMonitor.Enter(lockObject, TimeSpan.FromMilliseconds(1));
            action.Should().Throw<TimeoutException>();
        }

        [Theory]
        [AutoData]
        public void ShouldReleaseLockInOtherThread(object lockObject)
        {
            var lockHandle = SpinMonitor.Enter(lockObject);
            var thread = new Thread(lockHandle.Dispose);
            thread.Start();
            thread.Join();
        }

        [Theory]
        [AutoData]
        public void ShouldNotReleaseNonOwnedLockInOtherThread(object lockObject)
        {
            using var lockHandle = SpinMonitor.Enter(lockObject);
            Exception releaseException = null;
            var thread = new Thread(() =>
            {
                try
                {
                    new LockHandle(lockObject).Dispose();
                }
                catch (Exception ex)
                {
                    releaseException = ex;
                }
            });
            thread.Start();
            thread.Join();
            releaseException.Should().BeOfType<LockNotOwnedException>();
        }
    }
}
