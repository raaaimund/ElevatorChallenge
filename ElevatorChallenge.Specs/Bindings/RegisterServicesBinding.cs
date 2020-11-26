using BoDi;
using ElevatorChallenge.Application;
using ElevatorChallenge.Application.Factories;
using ElevatorChallenge.Application.Services;
using ElevatorChallenge.Domain.Entities;
using ElevatorChallenge.Infrastructure.Factories;
using ElevatorChallenge.Infrastructure.Services;
using ElevatorChallenge.Specs.Fakes;
using TechTalk.SpecFlow;

namespace ElevatorChallenge.Specs.Bindings
{
    [Binding]
    public class RegisterServicesBinding
    {
        private readonly IObjectContainer _container;

        public RegisterServicesBinding(IObjectContainer container)
        {
            _container = container;
        }

        [BeforeScenario]
        public void RegisterServices()
        {
            IRequestQueue<ElevatorRequest> queue = new ElevatorRequestQueueUsingChannel();
            IWaiterService waiterService = new FakeWaiterService();
            ILogMovementService logger = new ConsoleWriteLineLogMovementService();
            IElevatorRequestHandlerFactory requestHandlerFactory = new ElevatorRequestHandlerFactory(logger, waiterService, queue);
            IElevatorSystem elevatorSystem = new ElevatorSystem(requestHandlerFactory, waiterService, queue);
            _container.RegisterInstanceAs(elevatorSystem);
        }
    }
}