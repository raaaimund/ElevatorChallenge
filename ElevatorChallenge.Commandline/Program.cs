using ElevatorChallenge.Application;
using ElevatorChallenge.Application.Services;
using ElevatorChallenge.Infrastructure.Extensions;
using ElevatorChallenge.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ElevatorChallenge.Commandline
{
    class Program
    {
        private static readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        static async Task Main(string[] args)
        {
            Console.CancelKeyPress += Console_CancelKeyPress;

            try
            {
                await BuildCommandlineHost().RunConsoleAsync(_cancellationTokenSource.Token);
            }
            catch (AggregateException aggregatedExceptions)
            {
                aggregatedExceptions.Handle((ex) =>
                {
                    if(ex is OperationCanceledException)
                    {
                        Console.WriteLine("Cancelled ... bye.");
                        return true;
                    }

                    return false;
                });
            }
        }

        private static IHostBuilder BuildCommandlineHost() =>
            new HostBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddScoped<ILogMovementService, NetCoreLogMovementService>();
                    services.AddScoped<IWaiterService, WaiterService>();
                    services.AddElevatorMoverFactory();
                    services.AddSingleton<ElevatorSystem, ElevatorSystemWithTestData>();
                    services.AddHostedService<ElevatorSystemHostedService>();
                });

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs args)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}
