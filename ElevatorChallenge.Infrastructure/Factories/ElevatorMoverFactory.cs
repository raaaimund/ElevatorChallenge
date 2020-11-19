using System.Threading.Channels;
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

        public ElevatorMoverFactory(ILogMovementService logger, IWaiterService waiterService)
        {
            _logger = logger;
            _waiterService = waiterService;
        }

        public ElevatorMover Create(Elevator elevator, ChannelReader<ElevatorRequest> requestReader) =>
            new ElevatorMover(elevator, requestReader, _logger, _waiterService);
    }
}
