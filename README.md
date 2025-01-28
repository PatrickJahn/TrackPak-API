# TrackPak-API


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

