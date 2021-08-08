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
        public async Task ShouldAwaitWaitHandle()
        {
            using var mre = new ManualResetEvent(false);
            var _ = Task.Delay(3).ContinueWith(_ => mre.Set());
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
            var _ = Task.Delay(30).ContinueWith(_ => mre.Set());
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
    }
}
