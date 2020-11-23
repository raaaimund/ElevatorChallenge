using System;
using System.Threading;
using System.Threading.Tasks;
using ElevatorChallenge.Application;
using ElevatorChallenge.Application.Services;
using ElevatorChallenge.Infrastructure.Services;
using Moq;
using Xunit;

namespace ElevatorChallenge.Commandline.Tests
{
    public class ElevatorSystemHostedServiceTests
    {
        private readonly IWaiterService _waiterService = new WaiterService();

        [Fact]
        public async Task StartAsyncShouldAbortIfCanceled()
        {
            var cts = new CancellationTokenSource();
            var elevatorSystem = new Mock<IElevatorSystem>();
            var hostedService = new ElevatorSystemHostedService(elevatorSystem.Object, _waiterService);
            elevatorSystem.Setup(system =>
                    system.StartAsync(It.IsAny<CancellationToken>()))
                .Throws<TaskCanceledException>();

            cts.Cancel();

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                hostedService.StartAsync(cts.Token)
            );
        }

        [Fact]
        public async Task StartAsyncShouldReturnCanceledIfCanceled()
        {
            var cts = new CancellationTokenSource();
            var elevatorSystem = new Mock<IElevatorSystem>();
            var hostedService = new ElevatorSystemHostedService(elevatorSystem.Object, _waiterService);

            cts.Cancel();
            elevatorSystem.Setup(system =>
                    system.StartAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromCanceled(cts.Token));

            await hostedService.StartAsync(cts.Token)
                .ContinueWith(t =>
                    Assert.Equal(TaskStatus.Canceled, t.Status)
                );
        }

        [Fact]
        public async Task StartAsyncShouldReturnCompletedTaskIfCompleted()
        {
            var elevatorSystem = new Mock<IElevatorSystem>();
            elevatorSystem.Setup(system =>
                system.StartAsync(It.IsAny<CancellationToken>()));
            var hostedService = new ElevatorSystemHostedService(elevatorSystem.Object, _waiterService);

            await hostedService.StartAsync()
                .ContinueWith(t =>
                    Assert.Equal(TaskStatus.RanToCompletion, t.Status)
                );
        }

        [Fact]
        public async Task StopAsyncShouldAbortIfCanceled()
        {
            var cts = new CancellationTokenSource();
            var elevatorSystem = new Mock<IElevatorSystem>();
            elevatorSystem.Setup(system =>
                system.StartAsync(It.IsAny<CancellationToken>()));
            var hostedService = new ElevatorSystemHostedService(elevatorSystem.Object, _waiterService);

            cts.Cancel();
            await hostedService.StartAsync();

            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                hostedService.StopAsync(cts.Token)
            );
        }
    }
}
