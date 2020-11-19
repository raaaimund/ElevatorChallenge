using System.Threading;
using System.Threading.Tasks;

namespace ElevatorChallenge.Application.Services
{
    public interface IWaiterService
    {
        Task WaitForSecondsAsync(int seconds, CancellationToken cancellationToken = default);
        Task WaitUntilCanceled(CancellationToken cancellationToken = default);
    }
}
