using ElevatorChallenge.Application.Services;
using ElevatorChallenge.Domain.Entities;
using ElevatorChallenge.Domain.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace ElevatorChallenge.Application
{
    public class ElevatorMover
    {
        private readonly Elevator _elevator;
        private readonly ILogMovementService _logger;
        private readonly IWaiterService _waiterService;
        private readonly IRequestQueue<ElevatorRequest> _requestReader;

        public ElevatorMover(
            Elevator elevator,
            ILogMovementService logger,
            IWaiterService waiterService,
            IRequestQueue<ElevatorRequest> requestReader)
        {
            _elevator = elevator;
            _logger = logger;
            _waiterService = waiterService;
            _requestReader = requestReader;
        }

        public async Task CollectAndHandleRequestsAsync(CancellationToken cancellationToken = default)
        {
            await foreach (var request in _requestReader.ReadAllAsync(cancellationToken))
            {
                if (HasToMoveToAnotherFloorToPickUpPassengerFor(request))
                    await MoveToFloorAsync(
                        CreateRequestForMovingToFloor(_elevator.CurrentFloor, request.FromFloor),
                        cancellationToken
                    );

                await MoveToFloorAsync(request, cancellationToken);
            }

            await _requestReader.Completion;
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
                _logger.LogMovement(_elevator, request);
                await _waiterService.WaitForSecondsAsync(1, cancellationToken);
            }
        }

        public async Task MoveDownAsync(ElevatorRequest request, CancellationToken cancellationToken = default)
        {
            for (var currentFloor = request.FromFloor; currentFloor >= request.ToFloor; currentFloor--)
            {
                SetElevatorCurrentFloorTo(currentFloor);
                _logger.LogMovement(_elevator, request);
                await _waiterService.WaitForSecondsAsync(1, cancellationToken);
            }
        }

        private void SetElevatorCurrentFloorTo(int floor) =>
            _elevator.CurrentFloor = floor;

        private bool HasToMoveToAnotherFloorToPickUpPassengerFor(ElevatorRequest request) =>
            _elevator.CurrentFloor != request.FromFloor;

        private ElevatorRequest CreateRequestForMovingToFloor(int fromFloor, int toFloor) =>
            new ElevatorRequest() { FromFloor = fromFloor, ToFloor = toFloor };
    }
}
