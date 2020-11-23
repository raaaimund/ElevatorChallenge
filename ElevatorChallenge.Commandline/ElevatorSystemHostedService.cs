using ElevatorChallenge.Application;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using ElevatorChallenge.Application.Services;

namespace ElevatorChallenge.Commandline
{
    public class ElevatorSystemHostedService : IHostedService, IDisposable
    {
        private readonly IElevatorSystem _elevatorSystem;
        private readonly IWaiterService _waiterService;

        private CancellationTokenSource _cancellationTokenSource;
        private Task _currentlyExecutingTasks;

        public ElevatorSystemHostedService(IElevatorSystem elevatorSystem, IWaiterService waiterService)
        {
            _elevatorSystem = elevatorSystem;
            _waiterService = waiterService;
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _currentlyExecutingTasks = _elevatorSystem.StartAsync(_cancellationTokenSource.Token);
            return _currentlyExecutingTasks.IsCompleted
                ? _currentlyExecutingTasks
                : Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (_currentlyExecutingTasks == null) return;
            _cancellationTokenSource.Cancel();
            await Task.WhenAny(
                _currentlyExecutingTasks,
                _waiterService.WaitUntilCanceled(_cancellationTokenSource.Token)
            );
            cancellationToken.ThrowIfCancellationRequested();
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
