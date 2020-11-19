using ElevatorChallenge.Application.Services;
using ElevatorChallenge.Domain.Entities;
using ElevatorChallenge.Domain.Enums;

namespace ElevatorChallenge.Infrastructure.Services
{
    public abstract class BaseLogMovementService : ILogMovementService
    {
        public abstract void LogMovement(Elevator elevator, ElevatorRequest request);
        protected string CreateSymbolForDirection(Direction direction) =>
            direction == Direction.Up ? "↑" : "↓";
        protected string CreateMessage(Elevator elevator, ElevatorRequest request) =>
            $"{elevator.Name}: {CreateSymbolForDirection(request.Direction)} Current: {elevator.CurrentFloor} [Request: from {request.FromFloor} -> to {request.ToFloor}]";
    }
}
