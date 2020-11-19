using ElevatorChallenge.Application.Services;
using ElevatorChallenge.Domain.Entities;
using ElevatorChallenge.Domain.Enums;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ElevatorChallenge.Application
{
    public class ElevatorMover
    {
        private readonly Elevator _elevator;
        private readonly ChannelReader<ElevatorRequest> _requestReader;
        private readonly ILogMovementService _logger;
        private readonly IWaiterService _waiterService;

        public ElevatorMover(
            Elevator elevator, 
            ChannelReader<ElevatorRequest> requestReader, 
            ILogMovementService logger, 
            IWaiterService waiterService
        )
        {
            _elevator = elevator;
            _requestReader = requestReader;
            _logger = logger;
            _waiterService = waiterService;
        }

        public async Task CollectAndHandleRequestsAsync(CancellationToken cancellationToken)
        {
            await foreach (var request in _requestReader.ReadAllAsync())
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

        private Task MoveToFloorAsync(ElevatorRequest request, CancellationToken cancellationToken) =>
            request.Direction == Direction.Up
                ? MoveUpAsync(request, cancellationToken)
                : MoveDownAsync(request, cancellationToken);

        private async Task MoveUpAsync(ElevatorRequest request, CancellationToken cancellationToken)
        {
            for (int currentFloor = request.FromFloor; currentFloor <= request.ToFloor; currentFloor++)
            {
                SetElevatorCurrentFloorTo(currentFloor);
                _logger.LogMovement(_elevator, request);
                await _waiterService.WaitForSecondsAsync(1, cancellationToken);
            }
        }

        private async Task MoveDownAsync(ElevatorRequest request, CancellationToken cancellationToken)
        {
            for (int currentFloor = request.FromFloor; currentFloor >= request.ToFloor; currentFloor--)
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
