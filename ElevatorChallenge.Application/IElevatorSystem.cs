using System;
using System.Threading;
using System.Threading.Tasks;
using ElevatorChallenge.Domain.Entities;

namespace ElevatorChallenge.Application
{
    public interface IElevatorSystem : IDisposable
    {
        Task StartAsync(CancellationToken cancellationToken = default);
        void AddRequest(ElevatorRequest request);
        void AddElevator(Elevator elevator);
    }
}