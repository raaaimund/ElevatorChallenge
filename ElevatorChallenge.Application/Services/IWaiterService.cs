using System.Threading;
using System.Threading.Tasks;

namespace ElevatorChallenge.Application.Services
{
    public interface IWaiterService
    {
        Task WaitForSecondsAsync(int seconds, CancellationToken cancellationToken);
        Task WaitUntilCanceled(CancellationToken cancellationToken);
    }
}
