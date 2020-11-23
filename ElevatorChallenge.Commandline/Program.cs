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
    internal class Program
    {
        private static readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        private static async Task Main(string[] args)
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
                    if (ex is not OperationCanceledException) return false;
                    Console.WriteLine("Cancelled ... bye.");
                    return true;

                });
            }
        }

        private static IHostBuilder BuildCommandlineHost(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddTransient<ILogMovementService, NetCoreLogMovementService>();
                    services.AddTransient<IWaiterService, WaiterService>();
                    services.AddTransient<IElevatorRequestHandlerFactory, ElevatorRequestHandlerFactory>();
                    services.AddSingleton<IRequestQueue<ElevatorRequest>, ElevatorRequestQueueUsingChannel>();
                    services.AddSingleton<IElevatorSystem, ElevatorSystemWithTestData>();
                    services.AddHostedService<ElevatorSystemHostedService>();
                });

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs args)
        {
            CancellationTokenSource.Cancel();
        }
    }
}
