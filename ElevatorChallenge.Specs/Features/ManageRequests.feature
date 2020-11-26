Feature: Manage requests
	In order to get to or leave my office
	as an employee
	I want the system to bring me to my destination

Background: 
	Given there is 1 elevator

Scenario: I want to get to or leave the office
	Given I am at floor <currentFloor>
	And my destination is on floor <destinationFloor>
	When I scan my card
	Then an elevator should bring me to floor <destinationFloor>

Examples: 
	| currentFloor | destinationFloor |
	| 0            | 35               |
	| 35           | 0                |

Scenario: Multiple employees are leaving office
	When the following requests were made:
		| FromFloor | ToFloor |
		| 35        | 0       |
		| 4         | 0       |
		| 54        | 0       |
		| 23        | 0       |
	Then all employees should arrive on their destination

Scenario: Multiple employees are coming and leaving office
	When the following requests were made:
         | FromFloor | ToFloor |
         | 0         | 3       |
         | 45        | 0       |
         | 0         | 34      |
         | 0         | 45      |
         | 43        | 0       |
	Then all employees should arrive on their destination