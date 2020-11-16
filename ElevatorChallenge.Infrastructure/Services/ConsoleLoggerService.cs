using ElevatorChallenge.Application.Services;
using ElevatorChallenge.Domain.Entities;
using System;

namespace ElevatorChallenge.Infrastructure.Services
{
    public class ConsoleLoggerService : ILoggerService
    {
        public void PrintCurrentMovement(Elevator elevator, ElevatorRequest request) =>
            Console.WriteLine(
                $"{elevator.Name}: {CreateSymbolForDirection(request)} Current: {elevator.CurrentFloor} [Request: from {request.FromFloor} -> to {request.ToFloor}]"
            );

        private string CreateSymbolForDirection(ElevatorRequest request) =>
            request.Direction == Domain.Enums.Direction.Up ? "↑" : "↓";
    }
}
