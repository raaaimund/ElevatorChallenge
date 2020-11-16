using ElevatorChallenge.Domain.Entities;
using ElevatorChallenge.Domain.Enums;
using Xunit;

namespace ElevatorChallenge.Domain.Tests.Entities
{
    public class ElevatorRequestTests
    {
        [Theory]
        [InlineData(0, 15, Direction.Up)]
        [InlineData(15, 0, Direction.Down)]
        public void DirectionShouldBeCorrect(int fromFloor, int toFloor, Direction expected)
        {
            var actual = new ElevatorRequest() { FromFloor = fromFloor, ToFloor = toFloor };

            Assert.Equal(expected, actual.Direction);
        }
    }
}
