using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ElevatorChallenge.Domain.Entities;
using ElevatorChallenge.Infrastructure.Services;
using Xunit;

namespace ElevatorChallenge.Infrastructure.Tests.Services
{
    public class ElevatorRequestQueueUsingChannelTests
    {
        [Fact]
        public async Task WriteAsyncShouldWriteToQueue()
        {
            var queue = new ElevatorRequestQueueUsingChannel();
            var actualRequests = new List<ElevatorRequest>();

            await queue.WriteAsync(new ElevatorRequest());
            queue.Complete();
            await foreach (var request in queue.ReadAllAsync())
                actualRequests.Add(request);

            Assert.Single(actualRequests);
        }

        [Fact(Timeout = 10)]
        public async Task CompletionShouldWaitUntilComplete()
        {
            var queue = new ElevatorRequestQueueUsingChannel();
            var tasks = new[]
            {
                queue.Completion,
                Task.Delay(Timeout.Infinite, CancellationToken.None)
            };
            
            queue.Complete();
            
            await Task.Factory.ContinueWhenAny(tasks, t =>
                Assert.Equal(Task.CompletedTask.Status, t.Status)
            );
        }
    }
}
