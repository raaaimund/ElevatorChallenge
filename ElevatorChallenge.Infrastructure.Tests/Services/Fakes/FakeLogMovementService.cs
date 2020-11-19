using ElevatorChallenge.Domain.Entities;
using ElevatorChallenge.Domain.Enums;
using ElevatorChallenge.Infrastructure.Services;
using System;

namespace ElevatorChallenge.Infrastructure.Tests.Services.Fakes
{
    public class FakeLogMovementService : BaseLogMovementService
    {
        public override void LogMovement(Elevator elevator, ElevatorRequest request)
        {
            throw new System.NotImplementedException();
        }

        internal string FakeCreateSymbolForDirection(Direction direction) =>
            CreateSymbolForDirection(direction);

        internal string FakeCreateMessage(Elevator elevator, ElevatorRequest request) =>
            CreateMessage(elevator, request);
    }
}
