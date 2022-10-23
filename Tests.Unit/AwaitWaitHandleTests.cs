using System.Collections.Generic;

namespace Tests.Unit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xunit;
    using Zagidziran.Concurrent.AwaitableWaitHandle;

    public class AwaitWaitHandleTests
    {
        [Fact]
        public async Task ShouldResetOnWait()
        {
            var are = new AutoResetEvent(true);
            var wrapper = are.WithTimeout(TimeSpan.FromMilliseconds(10));
            await wrapper;
            Func<Task> action = async () => await wrapper;
            await action.Should().ThrowAsync<TimeoutException>();
        }

        [Fact]
        public async Task ShouldAwaitWaitHandle()
        {
            using var mre = new ManualResetEvent(false);
            var _ = Task.Delay(1).ContinueWith(_ => mre.Set());
            await mre;
        }

        [Fact]
        public async Task ShouldThrowTimeoutException()
        {
            using var mre = new ManualResetEvent(false);
            Func<Task> action = async () => await mre.WithTimeout(TimeSpan.FromMilliseconds(10));
            await action.Should().ThrowAsync<TimeoutException>();
        }

        [Fact]
        public async Task ShouldAwaitWaitHandleEvenTimeoutSpecified()
        {
            using var mre = new ManualResetEvent(false);
            var _ = Task.Delay(1).ContinueWith(_ => mre.Set());
            await mre.WithTimeout(TimeSpan.FromSeconds(10));
        }

        [Fact]
        public async Task ShouldAwaitSignaledWaitHandle()
        {
            using var mre = new ManualResetEvent(true);
            await mre;
        }

        [Fact]
        public async Task ShouldResetAutoResetEventWaitHandle()
        {
            using var are = new AutoResetEvent(true);
            await are;
            are.WaitOne(0).Should().BeFalse();
         }

        [Fact]
        public async Task ShouldThrowOperationCancelledException()
        {
            using var cts = new CancellationTokenSource();
            using var mre = new ManualResetEvent(false);
            cts.CancelAfter(1);
            Func<Task> action = async () => await mre.WithCancellation(cts.Token);
            await action.Should().ThrowAsync<OperationCanceledException>();
        }

        [Fact]
        public async Task ShouldThrowOperationCancelledExceptionWhenTimeoutSpecified()
        {
            using var cts = new CancellationTokenSource();
            using var mre = new ManualResetEvent(false);
            cts.CancelAfter(1);
            Func<Task> action = async () => await mre.WithCancellation(cts.Token).WithTimeout(TimeSpan.FromSeconds(10));
            await action.Should().ThrowAsync<OperationCanceledException>();
        }

        [Fact]
        public async Task ShouldThrowOperationCancelledOnAlreadyCancelledToken()
        {
            using var cts = new CancellationTokenSource();
            using var mre = new ManualResetEvent(false);
            cts.Cancel();
            Func<Task> action = async () => await mre.WithCancellation(cts.Token);
            await action.Should().ThrowAsync<OperationCanceledException>();
        }

        [Fact]
        public async Task ShouldThrowTimeoutExceptionWhenCancellationSpecified()
        {
            using var cts = new CancellationTokenSource();
            using var mre = new ManualResetEvent(false);
            Func<Task> action = async () => await mre.WithCancellation(cts.Token).WithTimeout(TimeSpan.FromMilliseconds(1));
            await action.Should().ThrowAsync<TimeoutException>();
        }

        [Fact]
        public async Task ShouldThrowObjectDisposedExceptionOnAlreadyDisposed()
        {
            using var mre = new ManualResetEvent(false);
            mre.Dispose();
            Func<Task> action = async () => await mre;
            await action.Should().ThrowAsync<ObjectDisposedException>();
        }
    }
}