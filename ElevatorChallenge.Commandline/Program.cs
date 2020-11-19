using ElevatorChallenge.Application;
using ElevatorChallenge.Application.Services;
using ElevatorChallenge.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using ElevatorChallenge.Application.Factories;
using ElevatorChallenge.Domain.Entities;
using ElevatorChallenge.Infrastructure.Factories;

namespace ElevatorChallenge.Commandline
{
    class Program
    {
        private static readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        static async Task Main(string[] args)
        {
            Console.CancelKeyPress += Console_CancelKeyPress;

            try
            {
                await BuildCommandlineHost(args)
                    .RunConsoleAsync(CancellationTokenSource.Token);
            }
            catch (AggregateException aggregatedExceptions)
            {
                aggregatedExceptions.Handle((ex) =>
                {
                    if (ex is OperationCanceledException)
                    {
                        Console.WriteLine("Cancelled ... bye.");
                        return true;
                    }

                    return false;
                });
            }
        }

        private static IHostBuilder BuildCommandlineHost(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddScoped<ILogMovementService, NetCoreLogMovementService>();
                    services.AddScoped<IWaiterService, WaiterService>();
                    services.AddScoped<IElevatorMoverFactory, ElevatorMoverFactory>();
                    services.AddSingleton<IRequestQueue<ElevatorRequest>, ElevatorRequestQueueUsingChannel>();
                    services.AddSingleton<ElevatorSystem, ElevatorSystemWithTestData>();
                    services.AddHostedService<ElevatorSystemHostedService>();
                });

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs args)
        {
            CancellationTokenSource.Cancel();
            CancellationTokenSource?.Dispose();
        }
    }
}
