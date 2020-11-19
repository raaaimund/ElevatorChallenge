using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ElevatorChallenge.Application.Services;
using ElevatorChallenge.Domain.Entities;
using ElevatorChallenge.Infrastructure.Services;
using Moq;
using Xunit;

namespace ElevatorChallenge.Application.Tests
{
    public class ElevatorManagerTests
    {
        private readonly Mock<ILogMovementService> _movementLogger;
        private readonly Mock<IWaiterService> _waiterService;

        public ElevatorManagerTests()
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
        }

        [Fact]
        public async Task MoveDownAsyncShouldMoveElevatorDownwards()
        {
            var request = new ElevatorRequest() { FromFloor = 5, ToFloor = 0 };
            var elevator = new Elevator()
            {
                CurrentFloor = 5,
                Name = nameof(MoveDownAsyncShouldMoveElevatorDownwards)
            };
            var elevatorMover = new ElevatorMover(
                elevator,
                _movementLogger.Object,
                _waiterService.Object,
                null
            );

            await elevatorMover.MoveDownAsync(request);

            Assert.Equal(request.ToFloor, elevator.CurrentFloor);
        }

        [Fact]
        public async Task MoveDownAsyncShouldAbortIfCanceled()
        {
            var cts = new CancellationTokenSource();
            var request = new ElevatorRequest() { FromFloor = 5, ToFloor = 0 };
            var elevator = new Elevator()
            {
                CurrentFloor = 5,
                Name = nameof(MoveDownAsyncShouldMoveElevatorDownwards)
            };
            var elevatorMover = new ElevatorMover(
                elevator,
                _movementLogger.Object,
                new WaiterService(),
                null
            );

            cts.Cancel();

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                elevatorMover.MoveDownAsync(request, cts.Token)
            );
        }
        [Fact]
        public async Task MoveUpAsyncShouldMoveElevatorUpwards()
        {
            var request = new ElevatorRequest() { FromFloor = 0, ToFloor = 5 };
            var elevator = new Elevator()
            {
                CurrentFloor = 0,
                Name = nameof(MoveDownAsyncShouldMoveElevatorDownwards)
            };
            var elevatorMover = new ElevatorMover(
                elevator,
                _movementLogger.Object,
                _waiterService.Object,
                null
            );

            await elevatorMover.MoveUpAsync(request);

            Assert.Equal(request.ToFloor, elevator.CurrentFloor);
        }

        [Fact]
        public async Task MoveUpAsyncShouldAbortIfCanceled()
        {
            var cts = new CancellationTokenSource();
            var request = new ElevatorRequest() { FromFloor = 0, ToFloor = 5 };
            var elevator = new Elevator()
            {
                CurrentFloor = 0,
                Name = nameof(MoveDownAsyncShouldMoveElevatorDownwards)
            };
            var elevatorMover = new ElevatorMover(
                elevator,
                _movementLogger.Object,
                new WaiterService(),
                null
            );

            cts.Cancel();

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                elevatorMover.MoveUpAsync(request, cts.Token)
            );
        }

        public static IEnumerable<object[]> CreateMoveToFloorData =>
            new List<object[]>()
            {
                new object[]
                {
                    new Elevator(),
                    new ElevatorRequest() { FromFloor = 0, ToFloor = 5 },

                },
                new object[]
                {
                    new Elevator(){CurrentFloor = 5},
                    new ElevatorRequest() { FromFloor = 5, ToFloor = 0 },
                }
            };

        [Theory]
        [MemberData(nameof(CreateMoveToFloorData))]
        public async Task MoveToFloorAsyncShouldMoveUpIfDirectionIsUp(Elevator elevator, ElevatorRequest request)
        {
            var elevatorMover = new ElevatorMover(
                elevator,
                _movementLogger.Object,
                _waiterService.Object,
                null
            );

            await elevatorMover.MoveToFloorAsync(request);

            Assert.Equal(request.ToFloor, elevator.CurrentFloor);
        }
    }
}
