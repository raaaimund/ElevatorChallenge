using ElevatorChallenge.Application.Services;
using ElevatorChallenge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using ElevatorChallenge.Application.Factories;

namespace ElevatorChallenge.Application
{
    public class ElevatorSystem : IDisposable
    {
        private readonly ICollection<Task> _workers;
        private readonly ICollection<ElevatorMover> _elevatorMovers;
        private readonly ICollection<ElevatorRequest> _requests;
        private readonly IElevatorMoverFactory _elevatorMoverFactory;
        private readonly IWaiterService _waiterService;
        private readonly IRequestQueue<ElevatorRequest> _requestChannel;

        public ElevatorSystem(IElevatorMoverFactory elevatorMoverFactory, IWaiterService waiterService, IRequestQueue<ElevatorRequest> requestChannel)
        {
            _workers = new List<Task>();
            _elevatorMovers = new List<ElevatorMover>();
            _requests = new List<ElevatorRequest>();
            _elevatorMoverFactory = elevatorMoverFactory;
            _waiterService = waiterService;
            _requestChannel = requestChannel;
        }

        public void AddRequest(ElevatorRequest request) =>
            _requests.Add(request);

        public void AddElevator(Elevator elevator) =>
            _elevatorMovers.Add(_elevatorMoverFactory.Create(elevator));

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            _workers.Add(WriteRequestsToChannelAsync(cancellationToken));

            foreach (var elevatorMover in _elevatorMovers)
                _workers.Add(elevatorMover.CollectAndHandleRequestsAsync(cancellationToken));

            await Task.WhenAny(
                Task.WhenAll(_workers.ToArray()),
                _waiterService.WaitUntilCanceled(cancellationToken)
            );

            cancellationToken.ThrowIfCancellationRequested();
        }

        private async Task WriteRequestsToChannelAsync(CancellationToken cancellationToken)
        {
            foreach (var request in _requests)
                await _requestChannel.WriteAsync(request, cancellationToken);
        }

        public void Dispose()
        {
            _requestChannel.Complete();
        }
    }
}
