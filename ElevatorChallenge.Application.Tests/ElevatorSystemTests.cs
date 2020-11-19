using ElevatorChallenge.Infrastructure.Services;
using System;
using System.Threading;
using ElevatorChallenge.Infrastructure.Factories;
using Xunit;

namespace ElevatorChallenge.Application.Tests
{
    public class ElevatorSystemTests
    {
        [Fact]
        public async void StartAsyncWithoutRequestsOrElevatorsShouldAbortIfCancelled()
        {
            var elevatorSystem = new ElevatorSystem(null, new WaiterService());
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(
                () =>
                elevatorSystem.StartAsync(cancellationTokenSource.Token)
            );
        }

        [Fact]
        public async void StartAsyncWithRequestsShouldAbortIfCancelled()
        {
            var elevatorSystem = new ElevatorSystem(null, new WaiterService());
            var cancellationTokenSource = new CancellationTokenSource();

            elevatorSystem.AddRequest(new Domain.Entities.ElevatorRequest());
            cancellationTokenSource.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(
               () =>
               elevatorSystem.StartAsync(cancellationTokenSource.Token)
           );
        }

        [Fact]
        public async void StartAsyncWithElevatorsShouldAbortIfCancelled()
        {
            var elevatorSystem = new ElevatorSystem(
                new ElevatorMoverFactory(null, null),
                new WaiterService()
            );
            var cancellationTokenSource = new CancellationTokenSource();

            elevatorSystem.AddElevator(new Domain.Entities.Elevator());
            cancellationTokenSource.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(
                () =>
                    elevatorSystem.StartAsync(cancellationTokenSource.Token)
            );
        }
    }
}
