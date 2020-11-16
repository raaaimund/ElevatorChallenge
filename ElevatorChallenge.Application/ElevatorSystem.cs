using ElevatorChallenge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ElevatorChallenge.Application
{
    public class ElevatorSystem : IDisposable
    {
        private readonly Channel<ElevatorRequest> _requestChannel;
        private readonly ICollection<Task> _workers;
        private readonly ICollection<ElevatorMover> _elevatorMovers;
        private readonly ICollection<ElevatorRequest> _requests;
        private readonly Func<Elevator, ChannelReader<ElevatorRequest>, ElevatorMover> _createElevatorMover;

        public ElevatorSystem(Func<Elevator, ChannelReader<ElevatorRequest>, ElevatorMover> createElevatorMover)
        {
            _requestChannel = Channel.CreateUnbounded<ElevatorRequest>();
            _workers = new List<Task>();
            _elevatorMovers = new List<ElevatorMover>();
            _requests = new List<ElevatorRequest>();
            _createElevatorMover = createElevatorMover;
        }

        public void AddRequest(ElevatorRequest request) =>
            _requests.Add(request);

        public void AddElevator(Elevator elevator) =>
            _elevatorMovers.Add(_createElevatorMover(elevator, _requestChannel.Reader));

        private async Task WriteRequestsToChannelAsync(CancellationToken cancellationToken)
        {
            foreach (var request in _requests)
                await _requestChannel.Writer.WriteAsync(request, cancellationToken);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _workers.Add(WriteRequestsToChannelAsync(cancellationToken));
            foreach (var elevatorMover in _elevatorMovers)
                _workers.Add(elevatorMover.CollectAndHandleRequestsAsync(cancellationToken));

            await Task.WhenAll(_workers.ToArray());
        }

        public void Dispose()
        {
            _requestChannel.Writer.Complete();
        }
    }
}
