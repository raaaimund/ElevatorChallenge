using ElevatorChallenge.Domain.Entities;

namespace ElevatorChallenge.Application.Factories
{
    public interface IElevatorMoverFactory
    {
        ElevatorMover Create(Elevator elevator);
    }
}