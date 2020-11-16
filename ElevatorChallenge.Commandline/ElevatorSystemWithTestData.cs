using ElevatorChallenge.Application;
using ElevatorChallenge.Domain.Entities;
using System;
using System.Threading.Channels;

namespace ElevatorChallenge.Commandline
{
    public class ElevatorSystemWithTestData : ElevatorSystem
    {
        public ElevatorSystemWithTestData(Func<Elevator, ChannelReader<ElevatorRequest>, ElevatorMover> createElevatorMover) : base(createElevatorMover)
        {
            AddElevator(new Elevator() { Name = "Aufzug 1" });
            AddElevator(new Elevator() { Name = "Aufzug 2" });
            AddRequest(new ElevatorRequest() { FromFloor = 0, ToFloor = 5 });
            AddRequest(new ElevatorRequest() { FromFloor = 10, ToFloor = 0 });
            AddRequest(new ElevatorRequest() { FromFloor = 5, ToFloor = 0 });
        }
    }
}
