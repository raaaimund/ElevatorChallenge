using ElevatorChallenge.Domain.Entities;

namespace ElevatorChallenge.Application.Factories
{
    public interface IElevatorRequestHandlerFactory
    {
        ElevatorRequestHandler Create(Elevator elevator);
    }
}