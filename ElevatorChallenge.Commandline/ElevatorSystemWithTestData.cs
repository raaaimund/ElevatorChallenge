using ElevatorChallenge.Application;
using ElevatorChallenge.Domain.Entities;
using ElevatorChallenge.Application.Factories;
using ElevatorChallenge.Application.Services;

namespace ElevatorChallenge.Commandline
{
    public class ElevatorSystemWithTestData : ElevatorSystem
    {
        public ElevatorSystemWithTestData(
            IElevatorMoverFactory elevatorMoverFactory, 
            IWaiterService waiterService, 
            IRequestQueue<ElevatorRequest> requestChannel
        ) : base(elevatorMoverFactory, waiterService, requestChannel)
        {
            AddElevator(new Elevator { Name = "Aufzug 1" });
            AddElevator(new Elevator { Name = "Aufzug 2" });
            AddRequest(new ElevatorRequest { FromFloor = 0, ToFloor = 5 });
            AddRequest(new ElevatorRequest { FromFloor = 10, ToFloor = 0 });
            AddRequest(new ElevatorRequest { FromFloor = 5, ToFloor = 0 });
        }
    }
}
