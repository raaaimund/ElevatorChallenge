using ElevatorChallenge.Application.Services;
using ElevatorChallenge.Domain.Entities;
using ElevatorChallenge.Domain.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace ElevatorChallenge.Application
{
    public class ElevatorMover
    {
        private readonly ILogMovementService _logger;
        private readonly IWaiterService _waiterService;

        public Elevator Elevator { get; }

        public ElevatorMover(
            Elevator elevator,
            ILogMovementService logger,
            IWaiterService waiterService)
        {
            Elevator = elevator;
            _logger = logger;
            _waiterService = waiterService;
        }

        public Task MoveToFloorAsync(ElevatorRequest request, CancellationToken cancellationToken = default) =>
            request.Direction == Direction.Up
                ? MoveUpAsync(request, cancellationToken)
                : MoveDownAsync(request, cancellationToken);

        public async Task MoveUpAsync(ElevatorRequest request, CancellationToken cancellationToken = default)
        {
            for (var currentFloor = request.FromFloor; currentFloor <= request.ToFloor; currentFloor++)
            {
                SetElevatorCurrentFloorTo(currentFloor);
                _logger.LogMovement(Elevator, request);
                await _waiterService.WaitForSecondsAsync(1, cancellationToken);
            }
        }

        public async Task MoveDownAsync(ElevatorRequest request, CancellationToken cancellationToken = default)
        {
            for (var currentFloor = request.FromFloor; currentFloor >= request.ToFloor; currentFloor--)
            {
                SetElevatorCurrentFloorTo(currentFloor);
                _logger.LogMovement(Elevator, request);
                await _waiterService.WaitForSecondsAsync(1, cancellationToken);
            }
        }

        private void SetElevatorCurrentFloorTo(int floor) =>
            Elevator.CurrentFloor = floor;

        public bool HasToMoveToAnotherFloorToPickUpPassengerFor(ElevatorRequest request) =>
            Elevator.CurrentFloor != request.FromFloor;

        public ElevatorRequest CreateRequestForMovingToFloor(int fromFloor, int toFloor) =>
            new ElevatorRequest { FromFloor = fromFloor, ToFloor = toFloor };
    }
}
