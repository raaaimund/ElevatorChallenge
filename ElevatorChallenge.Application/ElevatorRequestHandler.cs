using System.Threading;
using System.Threading.Tasks;
using ElevatorChallenge.Application.Services;
using ElevatorChallenge.Domain.Entities;

namespace ElevatorChallenge.Application
{
    public class ElevatorRequestHandler
    {
        private readonly IRequestQueue<ElevatorRequest> _queue;
        private readonly ElevatorMover _elevatorMover;

        public ElevatorRequestHandler(IRequestQueue<ElevatorRequest> queue, ElevatorMover elevatorMover)
        {
            _queue = queue;
            _elevatorMover = elevatorMover;
        }

        public async Task HandleRequestsAsync(CancellationToken cancellationToken = default)
        {
            await foreach (var request in _queue.ReadAllAsync(cancellationToken))
            {
                if (_elevatorMover.HasToMoveToAnotherFloorToPickUpPassengerFor(request))
                    await _elevatorMover.MoveToFloorAsync(
                        _elevatorMover.CreateRequestForMovingToFloor(_elevatorMover.Elevator.CurrentFloor, request.FromFloor),
                        cancellationToken
                    );

                await _elevatorMover.MoveToFloorAsync(request, cancellationToken);
            }

            await _queue.Completion;
        }
    }
}
