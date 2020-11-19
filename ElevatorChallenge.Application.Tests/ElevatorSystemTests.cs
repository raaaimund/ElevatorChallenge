using ElevatorChallenge.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ElevatorChallenge.Domain.Entities;
using ElevatorChallenge.Infrastructure.Factories;
using Xunit;

namespace ElevatorChallenge.Application.Tests
{
    public class ElevatorSystemTests
    {
        [Fact]
        public async void StartAsyncWithoutRequestsOrElevatorsShouldAbortIfCancelled()
        {
            var elevatorSystem = new ElevatorSystem(
                null,
                new WaiterService(),
                new ElevatorRequestQueueUsingChannel()
            );
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
            var elevatorSystem = new ElevatorSystem(
                null,
                new WaiterService(),
                new ElevatorRequestQueueUsingChannel()
            );
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
            var queue = new ElevatorRequestQueueUsingChannel();
            var elevatorSystem = new ElevatorSystem(
                new ElevatorMoverFactory(null, null, queue),
                new WaiterService(),
                queue
                );
            var cancellationTokenSource = new CancellationTokenSource();

            elevatorSystem.AddElevator(new Domain.Entities.Elevator());
            cancellationTokenSource.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(
                () =>
                    elevatorSystem.StartAsync(cancellationTokenSource.Token)
            );
        }

        [Fact]
        public async void RequestShouldBeWrittenToQueue()
        {
            var queue = new ElevatorRequestQueueUsingChannel();
            var actualRequests = new List<ElevatorRequest>();
            var elevatorSystem = new ElevatorSystem(
                new ElevatorMoverFactory(null, null, queue),
                new WaiterService(),
                queue
            );
            elevatorSystem.AddRequest(new ElevatorRequest());

            await Task.WhenAny(
                elevatorSystem.StartAsync(),
                Task.Run(async () =>
                {
                    await Task.Delay(10);
                    queue.Complete();
                })
            );
            await foreach (var request in queue.ReadAllAsync())
                actualRequests.Add(request);

            Assert.Single(actualRequests);
        }
    }
}
