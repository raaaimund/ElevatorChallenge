using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using ElevatorChallenge.Application.Services;
using ElevatorChallenge.Domain.Entities;

namespace ElevatorChallenge.Infrastructure.Services
{
    public class ElevatorRequestQueueUsingChannel : IRequestQueue<ElevatorRequest>
    {
        private readonly Channel<ElevatorRequest> _channel;

        public ElevatorRequestQueueUsingChannel()
        {
            _channel = Channel.CreateUnbounded<ElevatorRequest>();
        }

        public Task Completion => 
            _channel.Reader.Completion;

        public ValueTask WriteAsync(ElevatorRequest request, CancellationToken cancellationToken = default) =>
            _channel.Writer.WriteAsync(request, cancellationToken);

        public IAsyncEnumerable<ElevatorRequest> ReadAllAsync(CancellationToken cancellationToken = default) =>
            _channel.Reader.ReadAllAsync(cancellationToken);

        public void Complete() =>
            _channel.Writer.Complete();
    }
}
