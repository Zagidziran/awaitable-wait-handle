namespace Tests.Unit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AwaitableWaitHandle;
    using FluentAssertions;
    using Xunit;

    public class AwaitWaitHandleTests
    {
        [Fact]
        public async Task ShouldAwaitWaitHandle()
        {
            var mre = new ManualResetEvent(false);
            var _ = Task.Delay(3).ContinueWith(_ => mre.Set());
            await mre;
        }

        [Fact]
        public async Task ShouldThrowTimeoutException()
        {
            var mre = new ManualResetEvent(false);
            Func<Task> action = async () => await mre.WithTimeout(TimeSpan.FromMilliseconds(10));
            await action.Should().ThrowAsync<TimeoutException>();
        }

        [Fact]
        public async Task ShouldAwaitWaitHandleEvenTimeoutSpecified()
        {
            var mre = new ManualResetEvent(false);
            var _ = Task.Delay(30).ContinueWith(_ => mre.Set());
            await mre.WithTimeout(TimeSpan.FromSeconds(10));
        }
    }
}
