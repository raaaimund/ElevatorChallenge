using ElevatorChallenge.Application.Services;
using ElevatorChallenge.Domain.Entities;

namespace ElevatorChallenge.Infrastructure.Services
{
    public abstract class BaseLogMovementService : ILogMovementService
    {
        public abstract void LogMovement(Elevator elevator, ElevatorRequest request);
        protected string CreateSymbolForDirection(ElevatorRequest request) =>
            request.Direction == Domain.Enums.Direction.Up ? "↑" : "↓";
        protected string CreateMessage(Elevator elevator, ElevatorRequest request) =>
            $"{elevator.Name}: {CreateSymbolForDirection(request)} Current: {elevator.CurrentFloor} [Request: from {request.FromFloor} -> to {request.ToFloor}]";
    }
}
