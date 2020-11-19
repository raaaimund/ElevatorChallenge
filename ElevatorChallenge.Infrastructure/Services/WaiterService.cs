using ElevatorChallenge.Application.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ElevatorChallenge.Infrastructure.Services
{
    public class WaiterService : IWaiterService
    {
        public Task WaitForSecondsAsync(int seconds, CancellationToken cancellationToken) =>
            Task.Delay(TimeSpan.FromSeconds(seconds), cancellationToken);

        public Task WaitUntilCanceled(CancellationToken cancellationToken) =>
            Task.Delay(Timeout.Infinite, cancellationToken);
    }
}
