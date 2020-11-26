using System.Threading;
using System.Threading.Tasks;
using ElevatorChallenge.Application.Services;

namespace ElevatorChallenge.Specs.Fakes
{
    public class FakeWaiterService : IWaiterService
    {
        public Task WaitForSecondsAsync(int seconds, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task WaitUntilCanceled(CancellationToken cancellationToken = default) =>
            Task.Delay(Timeout.Infinite, cancellationToken);
    }
}