using ElevatorChallenge.Application.Services;
using ElevatorChallenge.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElevatorChallenge.Application.Factories;

namespace ElevatorChallenge.Application
{
    public class ElevatorSystem : IElevatorSystem
    {
        private readonly ICollection<Task> _workers;
        private readonly ICollection<ElevatorRequestHandler> _requestsHandler;
        private readonly ICollection<ElevatorRequest> _requests;
        private readonly IElevatorRequestHandlerFactory _requestHandlerFactory;
        private readonly IWaiterService _waiterService;
        private readonly IRequestQueue<ElevatorRequest> _queue;

        public ElevatorSystem(
            IElevatorRequestHandlerFactory requestHandlerFactory, 
            IWaiterService waiterService, 
            IRequestQueue<ElevatorRequest> queue
        )
        {
            _workers = new List<Task>();
            _requestsHandler = new List<ElevatorRequestHandler>();
            _requests = new List<ElevatorRequest>();
            _requestHandlerFactory = requestHandlerFactory;
            _waiterService = waiterService;
            _queue = queue;
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            _workers.Add(WriteRequestsToChannelAsync(cancellationToken));

            foreach (var elevatorMover in _requestsHandler)
                _workers.Add(elevatorMover.HandleRequestsAsync(cancellationToken));

            await Task.WhenAny(
                Task.WhenAll(_workers.ToArray()),
                _waiterService.WaitUntilCanceled(cancellationToken)
            );

            cancellationToken.ThrowIfCancellationRequested();
        }

        public void AddRequest(ElevatorRequest request) =>
            _requests.Add(request);

        public void AddElevator(Elevator elevator) =>
            _requestsHandler.Add(_requestHandlerFactory.Create(elevator));

        private async Task WriteRequestsToChannelAsync(CancellationToken cancellationToken)
        {
            foreach (var request in _requests)
                await _queue.WriteAsync(request, cancellationToken);
        }

        public void Dispose()
        {
            _queue.Complete();
        }
    }
}
