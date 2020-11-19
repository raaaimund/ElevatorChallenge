using ElevatorChallenge.Application;
using ElevatorChallenge.Application.Factories;
using ElevatorChallenge.Application.Services;
using ElevatorChallenge.Domain.Entities;

namespace ElevatorChallenge.Infrastructure.Factories
{
    public class ElevatorMoverFactory : IElevatorMoverFactory
    {
        private readonly ILogMovementService _logger;
        private readonly IWaiterService _waiterService;
        private readonly IRequestQueue<ElevatorRequest> _requestQueue;

        public ElevatorMoverFactory(
            ILogMovementService logger, 
            IWaiterService waiterService, 
            IRequestQueue<ElevatorRequest> requestQueue
        )
        {
            _logger = logger;
            _waiterService = waiterService;
            _requestQueue = requestQueue;
        }

        public ElevatorMover Create(Elevator elevator) =>
            new ElevatorMover(elevator, _logger, _waiterService, _requestQueue);
    }
}
