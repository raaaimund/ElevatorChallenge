using ElevatorChallenge.Application;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ElevatorChallenge.Commandline
{
    public class ElevatorSystemHostedService : IHostedService, IDisposable
    {
        private readonly ElevatorSystem _elevatorSystem;

        private CancellationTokenSource _cancellationTokenSource;
        private Task _currentlyExecutingTasks;

        public ElevatorSystemHostedService(ElevatorSystem elevatorSystem)
        {
            _elevatorSystem = elevatorSystem;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _currentlyExecutingTasks = ExecuteAsync(_cancellationTokenSource.Token);
            return _currentlyExecutingTasks.IsCompleted 
                ? _currentlyExecutingTasks 
                : Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_currentlyExecutingTasks == null) return;
            _cancellationTokenSource.Cancel();
            await Task.WhenAny(
                _currentlyExecutingTasks,
                InfiniteRunningTaskUntilCancelledWith(cancellationToken)
            );
            cancellationToken.ThrowIfCancellationRequested();
        }

        private async Task ExecuteAsync(CancellationToken cancellationToken) =>
            await _elevatorSystem.StartAsync(cancellationToken);

        private Task InfiniteRunningTaskUntilCancelledWith(CancellationToken cancellationToken) =>
            Task.Delay(-1, cancellationToken);

        public void Dispose()
        {
            _cancellationTokenSource.Dispose();
        }
    }
}
