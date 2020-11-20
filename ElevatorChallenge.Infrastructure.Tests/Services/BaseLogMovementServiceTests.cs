using ElevatorChallenge.Domain.Entities;
using ElevatorChallenge.Domain.Enums;
using ElevatorChallenge.Infrastructure.Tests.Services.Fakes;
using System.Collections.Generic;
using Xunit;

namespace ElevatorChallenge.Infrastructure.Tests.Services
{
    public class BaseLogMovementServiceTests
    {
        [Theory]
        [InlineData(Direction.Up, "↑")]
        [InlineData(Direction.Down, "↓")]
        public void CreateSymbolForDirectionShouldReturnCorrectSymbol(Direction direction, string expectedSymbol)
        {
            var logger = new FakeLogMovementService();
            var actualSymbol = logger.FakeCreateSymbolForDirection(direction);

            Assert.Equal(expectedSymbol, actualSymbol);
        }

        public static IEnumerable<object[]> CreateMessageData =>
       new List<object[]>
       {
            new object[] {
                new Elevator { Name = "Elevator 1", CurrentFloor = 0 },
                new ElevatorRequest { FromFloor = 0, ToFloor = 5 },
                "Elevator 1: ↑ Current: 0 [Request: from 0 -> to 5]"
            },
            new object[] {
                new Elevator { Name = "Elevator 2", CurrentFloor = 0 },
                new ElevatorRequest { FromFloor = 5, ToFloor = 0 },
                "Elevator 2: ↓ Current: 0 [Request: from 5 -> to 0]"
            },
       };

        [Theory]
        [MemberData(nameof(CreateMessageData))]
        public void CreateMessageShouldCreateCorrectMessage(Elevator elevator, ElevatorRequest request, string expectedMessage)
        {
            var logger = new FakeLogMovementService();

            var actualMessage = logger.FakeCreateMessage(elevator, request);

            Assert.Equal(expectedMessage, actualMessage);
        }
    }
}
