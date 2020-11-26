using System.Collections.Generic;
using ElevatorChallenge.Domain.Entities;

namespace ElevatorChallenge.Specs.Context
{
    public class AddedElevatorsContext
    {
        public ICollection<Elevator> Elevators { get; set; }

        public AddedElevatorsContext()
        {
            Elevators = new List<Elevator>();
        }
    }
}