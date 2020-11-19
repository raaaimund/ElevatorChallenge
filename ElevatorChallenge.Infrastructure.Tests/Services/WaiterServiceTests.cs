using ElevatorChallenge.Infrastructure.Services;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ElevatorChallenge.Infrastructure.Tests.Services
{
    public class WaiterServiceTests
    {
        [Fact]
        public async Task WaitForSecondsAsyncShouldAbortIfCancelled()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var waiterService = new WaiterService();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsAsync<TaskCanceledException>(
                () => 
                waiterService.WaitForSecondsAsync(1, cancellationTokenSource.Token)
            );

        }
    }
}
