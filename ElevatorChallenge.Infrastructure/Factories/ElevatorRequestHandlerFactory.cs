using ElevatorChallenge.Application;
using ElevatorChallenge.Application.Factories;
using ElevatorChallenge.Application.Services;
using ElevatorChallenge.Domain.Entities;

namespace ElevatorChallenge.Infrastructure.Factories
{
    public class ElevatorRequestHandlerFactory : IElevatorRequestHandlerFactory
    {
        private readonly ILogMovementService _logger;
        private readonly IWaiterService _waiterService;
        private readonly IRequestQueue<ElevatorRequest> _requestQueue;

        public ElevatorRequestHandlerFactory(
            ILogMovementService logger,
            IWaiterService waiterService,
            IRequestQueue<ElevatorRequest> requestQueue
        )
        {
            _logger = logger;
            _waiterService = waiterService;
            _requestQueue = requestQueue;
        }

        public ElevatorRequestHandler Create(Elevator elevator)
        {
            var elevatorMover = new ElevatorMover(elevator, _logger, _waiterService);
            var elevatorRequestHandler = new ElevatorRequestHandler(_requestQueue, elevatorMover);
            return elevatorRequestHandler;
        }
    }
}
