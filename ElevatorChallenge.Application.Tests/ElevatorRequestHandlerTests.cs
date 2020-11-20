using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ElevatorChallenge.Application.Services;
using ElevatorChallenge.Domain.Entities;
using Moq;
using Xunit;

namespace ElevatorChallenge.Application.Tests
{
    public class ElevatorRequestHandlerTests
    {
        private readonly Mock<ILogMovementService> _movementLogger;
        private readonly Mock<IWaiterService> _waiterService;
        private readonly Mock<IRequestQueue<ElevatorRequest>> _queue;

        public static async IAsyncEnumerable<ElevatorRequest> GetRequestAsAsyncEnumerable(ElevatorRequest request)
        {
            yield return request;
            await Task.CompletedTask;
        }

        public ElevatorRequestHandlerTests()
        {
            _movementLogger = new Mock<ILogMovementService>();
            _movementLogger.Setup(logger => logger.LogMovement(
                It.IsAny<Elevator>(),
                It.IsAny<ElevatorRequest>()
            ));
            _waiterService = new Mock<IWaiterService>();
            _waiterService.Setup(waiter => waiter.WaitForSecondsAsync(
                It.IsAny<int>(),
                CancellationToken.None)
            );
            _queue = new Mock<IRequestQueue<ElevatorRequest>>();
        }

        [Fact]
        public async Task CollectAndHandleRequestsAsyncShouldAbortIfCanceled()
        {
            var cts = new CancellationTokenSource();
            _queue.Setup(q => q.ReadAllAsync(cts.Token))
                .Throws<TaskCanceledException>();
            var elevatorMover = new ElevatorMover(
                new Elevator(), _movementLogger.Object, _waiterService.Object
            );
            var requestHandler = new ElevatorRequestHandler(
                _queue.Object,
                elevatorMover
            );

            cts.Cancel();

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                requestHandler.HandleRequestsAsync(cts.Token)
            );
        }

        public static IEnumerable<object[]> HandleRequestsAsyncTestData =>
            new List<object[]>
            {

                new object[]
                {
                    new ElevatorRequest {FromFloor = 0, ToFloor = 5},
                },
                new object[]
                {
                    new ElevatorRequest {FromFloor = 10, ToFloor = 0}
                }
            };

        [Theory]
        [MemberData(nameof(HandleRequestsAsyncTestData))]
        public async Task HandleRequestsAsyncShouldMoveElevatorToCorrectFloor(ElevatorRequest request)
        {
            _queue.Setup(q => q.ReadAllAsync(It.IsAny<CancellationToken>()))
                .Returns(GetRequestAsAsyncEnumerable(request));
            var elevatorMover = new ElevatorMover(
                new Elevator(), _movementLogger.Object, _waiterService.Object
            );
            var requestHandler = new ElevatorRequestHandler(
                _queue.Object,
                elevatorMover
            );

            await requestHandler.HandleRequestsAsync();

            Assert.Equal(request.ToFloor, elevatorMover.Elevator.CurrentFloor);
        }
    }
}
