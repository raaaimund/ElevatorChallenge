using ElevatorChallenge.Application;
using ElevatorChallenge.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Channels;

namespace ElevatorChallenge.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddElevatorMoverFactory(this IServiceCollection services)
        {
            services.AddScoped<Func<Elevator, ChannelReader<ElevatorRequest>, ElevatorMover>>(
                        services =>
                        (elevator, requestReader) =>
                        ActivatorUtilities.CreateInstance<ElevatorMover>(services, elevator, requestReader));
        }
    }
}
