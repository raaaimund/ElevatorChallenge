using ElevatorChallenge.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace ElevatorChallenge.Infrastructure.Services
{
    public class NetCoreLogMovementService : BaseLogMovementService
    {
        private readonly ILogger<NetCoreLogMovementService> _logger;

        public NetCoreLogMovementService(ILogger<NetCoreLogMovementService> logger)
        {
            _logger = logger;
        }

        public override void LogMovement(Elevator elevator, ElevatorRequest request) =>
            _logger.LogInformation(CreateMessage(elevator, request));
    }
}
