using System;
using System.Linq;
using System.Threading.Tasks;
using ElevatorChallenge.Application;
using ElevatorChallenge.Domain.Entities;
using ElevatorChallenge.Specs.Context;
using FluentAssertions;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace ElevatorChallenge.Specs.Steps
{
    [Binding]
    public class ManageRequestsSteps
    {
        private readonly IElevatorSystem _elevatorSystem;
        private readonly EmployeeRequestContext _employeeRequestContext;
        private readonly AddedElevatorsContext _addedElevatorsContext;

        public ManageRequestsSteps(IElevatorSystem elevatorSystem, EmployeeRequestContext employeeRequestContext, AddedElevatorsContext addedElevatorsContext)
        {
            _elevatorSystem = elevatorSystem;
            _employeeRequestContext = employeeRequestContext;
            _addedElevatorsContext = addedElevatorsContext;
        }

        [Given(@"there is (.*) elevator")]
        public void GivenThereIsElevator(int elevators)
        {
            for (var i = 0; i < elevators; i++)
            {
                var elevator = new Elevator();
                _addedElevatorsContext.Elevators.Add(elevator);
                _elevatorSystem.AddElevator(elevator);
            }
        }

        [Given(@"I am at floor (.*)")]
        public void GivenIAmAtFloor(int currentFloor)
        {
            _employeeRequestContext.CurrentFloor = currentFloor;
        }

        [Given(@"my destination is on floor (.*)")]
        public void GivenMyOfficeIsOnFloor(int destinationFloor)
        {
            _employeeRequestContext.DestinationFloor = destinationFloor;
        }

        [When(@"I press the button")]
        [When(@"I scan my card")]
        public async Task WhenIPressTheButton()
        {
            _elevatorSystem.AddRequest(new ElevatorRequest()
            {
                FromFloor = _employeeRequestContext.CurrentFloor,
                ToFloor = _employeeRequestContext.DestinationFloor
            });

            await Task.WhenAny(
                _elevatorSystem.StartAsync(),
                Task.Delay(TimeSpan.FromSeconds(1))
            );
        }

        [When(@"the following requests were made:")]
        public async Task WhenTheFollowingRequestsWereMade(Table table)
        {
            var requests = table.CreateSet<ElevatorRequest>();
            foreach (var request in requests)
            {
                _elevatorSystem.AddRequest(request);
            }

            await Task.WhenAny(
                _elevatorSystem.StartAsync(),
                Task.Delay(TimeSpan.FromSeconds(1))
            );
        }

        [Then(@"an elevator should bring me to floor (.*)")]
        public void ThenAnElevatorShouldBringMeToFloor(int destinationFloor)
        {
            var elevatorsOnDestinationFloor = _addedElevatorsContext.Elevators.Any(a => a.CurrentFloor == destinationFloor);
            elevatorsOnDestinationFloor
                .Should()
                .BeTrue("at least one elevator arrived on this floor.");
        }

        [Then(@"all employees should arrive on their destination")]
        public void ThenAllEmployeesShouldArriveOnTheirDestination()
        {
            ScenarioContext.Current.Pending();
        }

        [AfterScenario]
        public void AfterScenario()
        {
            _elevatorSystem.Dispose();
        }
    }
}
