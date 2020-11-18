using ElevatorChallenge.Domain.Entities;

namespace ElevatorChallenge.Application.Services
{
    public interface ILogMovementService
    {
        void LogMovement(Elevator elevator, ElevatorRequest request);
    }
}
