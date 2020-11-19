using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ElevatorChallenge.Application.Services
{
    public interface IRequestQueue<TRequest>
    {
        Task Completion { get; }

        ValueTask WriteAsync(TRequest request, CancellationToken cancellationToken = default);
        IAsyncEnumerable<TRequest> ReadAllAsync(CancellationToken cancellationToken = default);
        void Complete();
    }
}