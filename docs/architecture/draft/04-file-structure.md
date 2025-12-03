# File Structure

Domain definitions are organized in a hierarchical file structure that separates concerns and enables modular composition.

## Directory Layout

```
domain/
├── domain.yaml                      # Domain metadata
│
├── services/                        # Bounded contexts
│   ├── scheduling.service.yaml
│   ├── billing.service.yaml
│   └── organisation.service.yaml
│
├── entities/                        # Domain entities
│   ├── booking/
│   │   ├── booking.entity.yaml      # Domain definition
│   │   └── booking.key.yaml         # Identity (persistence)
│   ├── attendance/
│   │   ├── attendance.entity.yaml
│   │   └── attendance.key.yaml
│   └── child/
│       ├── child.entity.yaml
│       └── child.key.yaml
│
├── values/                          # Value objects
│   ├── address.value.yaml
│   ├── date-range.value.yaml
│   └── person-name.value.yaml
│
├── shared/                          # Shared/lookup data (external)
│   ├── organisation.shared.yaml
│   └── absence-reason.shared.yaml
│
├── commands/                        # Business actions
│   ├── booking/
│   │   ├── place-booking.cmd.yaml
│   │   ├── cancel-booking.cmd.yaml
│   │   └── record-attendance.cmd.yaml
│   └── billing/
│       └── generate-invoice.cmd.yaml
│
├── queries/                         # Read operations
│   ├── booking/
│   │   ├── get-bookings.query.yaml
│   │   └── get-actual-bookings.query.yaml
│   └── billing/
│       └── get-invoice-summary.query.yaml
│
├── events/                          # Domain events
│   ├── booking-placed.event.yaml
│   ├── booking-cancelled.event.yaml
│   └── attendance-recorded.event.yaml
│
├── enums/                           # Enumerations
│   ├── booking-status.enum.yaml
│   └── care-type.enum.yaml
│
├── flags/                           # Bitwise flags
│   └── days-of-week.flags.yaml
│
└── projections/                     # Query return shapes
    ├── actual-booking-result.projection.yaml
    └── invoice-summary.projection.yaml
```

## File Types

| Extension | Purpose | Contains |
|-----------|---------|----------|
| `.entity.yaml` | Entity definition | Attributes, relationships, belongs_to |
| `.key.yaml` | Entity identity | Primary key, ownership chain |
| `.value.yaml` | Value object | Immutable attributes |
| `.shared.yaml` | Shared/lookup data | External data projection |
| `.service.yaml` | Bounded context | Groups entities, commands, queries |
| `.cmd.yaml` | Command | Input, outcome, events |
| `.query.yaml` | Query | Parameters, return shape |
| `.event.yaml` | Domain event | Event data |
| `.enum.yaml` | Enumeration | Named values |
| `.flags.yaml` | Flags | Bitwise values |
| `.projection.yaml` | Projection | Query return shape |

## Domain Metadata

```yaml
# domain.yaml
domain: ChildcareManagement
version: 2.0
description: |
  Domain model for childcare centre management including
  bookings, attendance, billing, and family management.

services:
  - scheduling
  - billing
  - organisation
  - family

defaults:
  text_max_length: 200
  date_format: ISO8601
```

## Service Definition

```yaml
# services/scheduling.service.yaml
service: Scheduling
description: Manages bookings, attendance, and sessions

entities:
  owned:
    - Booking
    - Attendance
    - Absence
    - Session
    - Room
    
  shared:
    - Child: [Name, DateOfBirth]
    - Centre: [Name, Token]
    - Adult: [Name, Contact]

enums:
  - BookingStatus
  - CareType
  - AbsenceType

commands:
  - PlaceBooking
  - CancelBooking
  - RecordAttendance
  - RecordAbsence

queries:
  - GetBookings
  - GetActualBookings
  - GetAttendanceReport
```

## Separation of Domain and Persistence

### Entity Definition (Domain)

```yaml
# entities/booking/booking.entity.yaml
entity: Booking
description: Planned attendance for a child at a session

attributes:
  Date:
    type: date
    description: When attendance is planned
    
  Status:
    type: BookingStatus
    
relationships:
  Session:
    has_one: Session
    
  Child:
    belongs_to: Child
```

### Key Definition (Persistence)

```yaml
# entities/booking/booking.key.yaml
key: Booking
description: Identity for Booking entity

identity:
  BookingId:
    type: integer
    generated: on_create
    
ownership:
  parent: Child
  through: ChildId
  
indexes:
  - fields: [Date, Session]
    unique: true
```

This separation allows:
- Domain stays clean of persistence concerns
- Keys can vary by storage technology
- Code generators combine as needed

