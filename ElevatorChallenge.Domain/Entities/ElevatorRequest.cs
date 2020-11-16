using ElevatorChallenge.Domain.Enums;

namespace ElevatorChallenge.Domain.Entities
{
    public class ElevatorRequest
    {
        public int FromFloor { get; set; }
        public int ToFloor { get; set; }
        public Direction Direction =>
            FromFloor > ToFloor ? Direction.Down : Direction.Up;
    }
}
