# TrackPak-API

## Running a service locally 
To run a service locally you have to setup the DB first. 
To do this you can run the script in the service Infrastructure: 
- dbSetupScript.sh

This will create the services DB(s) and a DB user.

## Repository Structure
```
TrackPakAPI/
│ 
├── UserService/
│   ├── UserService.API/             👈 API project (ASP.NET Core Web API)
│   ├── UserService.Application/     👈 Application logic (business rules)
│   ├── UserService.Domain/          👈 Domain models (entities, aggregates)
│   ├── UserService.Infrastructure/  👈 Infrastructure (DB, external APIs)
│   └── UserService.Tests/           👈 Unit & Integration Tests
│
│
├── Shared/                   👈 Shared libraries across microservices
│   ├── Shared.Common/        👈 Common utilities and helper functions
│   ├── Shared.Logging/       👈 Centralized logging functionality
│   ├── Shared.Messaging/     👈 Messaging utilities for inter-service communication (e.g., queues)
```



## Testing Tracking Service 
To test Tracking service you can use wscat (install via npm). 
The following commands can be used to test.
- Subscribe as an employee: 
````
wscat -c "ws://localhost:5057/tracking/ws?employeeId=6286f2e0-f19a-4d93-b70e-10af78642f62"
````

- Subscribe to a specific employee as a user
````
wscat -c "ws://localhost:5057/tracking/ws?userId=409a2b0c-0bfa-459c-b782-82a914bec178&employeeId=6286f2e0-f19a-4d93-b70e-10af78642f62"
````

If employee has not started subscribing, a user won't be able to subscribe to him. 