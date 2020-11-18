using ElevatorChallenge.Domain.Entities;
using System;

namespace ElevatorChallenge.Infrastructure.Services
{
    public class ConsoleWritelineLogMovementService : BaseLogMovementService
    {
        public override void LogMovement(Elevator elevator, ElevatorRequest request) =>
            Console.WriteLine(CreateMessage(elevator, request));
    }
}
