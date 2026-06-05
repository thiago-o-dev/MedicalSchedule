# WHAT EACH FOLDER MEANS

| Folder         | Purpose                  |
| -------------- | ------------------------ |
| Api            | HTTP layer               |
| Contracts      | external messages        |
| Domain         | business rules           |
| Features       | application use cases    |
| Infrastructure | external implementations |

# Api/
Transport only.

📁 Api
│
├─ 📁 Endpoints
│  │
│  ├── 📄 CreateAppointmentEndpoint.cs
│  └── 📄 CancelAppointmentEndpoint.cs
├── 📁 Middleware
└── 📁 Mapping

## Endpoints/
Minimal APIs.

## Middleware/
Only service-specific middleware.

Example:
- tenant resolution
- correlation IDs

## Mapping/
DTO ↔ command mapping.

# Features/
MOST IMPORTANT FOLDER.

Vertical Slice Architecture.

📁 Features
│
├── 📁 CreateAppointment
├── 📁 CancelAppointment
├── 📁 RescheduleAppointment
└── 📁 GetAppointment

Each folder is:

ONE USE CASE
EXAMPLE
📁 CreateAppointment
│
├── 📄 CreateAppointmentCommand.cs
├── 📄 CreateAppointmentHandler.cs
├── 📄 CreateAppointmentValidator.cs
├── 📄 CreateAppointmentRequest.cs
├── 📄 CreateAppointmentResponse.cs
└── 📄 CreateAppointmentMapper.cs

Everything related stays together.

This scales MUCH better than:
- Application/
- Validators/
- DTOs/
- Handlers/

# Domain/
Pure business rules.

📁 Domain
│
├── 📁 Entities
├── 📁 ValueObjects
├── 📁 Events
├── 📁 Policies
├── 📁 Services
└── 📁 Exceptions

## Policies/
Business invariants.

Example:
- SchedulingPolicy.cs

Contains rules like:
- doctor unavailable
- overlapping appointments
- clinic operating hours

This keeps entities clean.

## Services/?
Domain services.

Used when logic:
- belongs to domain,
- but doesn’t fit one entity.

Example:
- schedule conflict resolution

# Infrastructure/
External world implementations.

📁 Infrastructure
│
├── 📁 Persistence
├── 📁 Messaging
├── 📁 Redis
└── 📁 External

## WHAT IS External/?
Adapters to OTHER services.

Example:

📁 External
│
├── 📁 Payments
├── 📁 Notifications
└── 📁 Licensing

Hexagonal adapters.

# Contracts/

Messages exposed externally.

📁 Contracts
│
├── 📁 Events
├── 📁 Requests
└── 📁 Responses

Important:
- NEVER expose entities
- NEVER publish domain models

Only contracts.