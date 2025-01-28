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

