using ElevatorChallenge.Domain.Entities;

namespace ElevatorChallenge.Application.Services
{
    public interface ILoggerService
    {
        void PrintCurrentMovement(Elevator elevator, ElevatorRequest request);
    }
}
