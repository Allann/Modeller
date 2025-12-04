# File Structure

Domain definitions are organized in a hierarchical file structure that separates concerns and enables modular composition.

## Directory Layout

```
domain/
├── domain.def                       # Domain metadata
│
├── services/                        # Bounded contexts
│   ├── scheduling.service
│   ├── billing.service
│   └── organisation.service
│
├── entities/                        # Domain entities
│   ├── booking.entity
│   ├── attendance.entity
│   └── child.entity
│
├── keys/                            # Identity definitions
│   ├── booking.key
│   ├── attendance.key
│   └── child.key
│
├── values/                          # Value objects
│   ├── address.value
│   ├── date-range.value
│   └── person-name.value
│
├── shared/                          # Shared/lookup data (external)
│   ├── organisation.shared
│   └── absence-reason.shared
│
├── behaviours/                      # Commands and queries
│   ├── place-booking.command
│   ├── cancel-booking.command
│   ├── record-attendance.command
│   ├── get-bookings.query
│   └── get-actual-bookings.query
│
├── events/                          # Domain events
│   ├── booking-placed.event
│   ├── booking-cancelled.event
│   └── attendance-recorded.event
│
├── enums/                           # Enumerations
│   ├── booking-status.enum
│   └── care-type.enum
│
├── flags/                           # Bitwise flags
│   └── days-of-week.flags
│
└── projections/                     # Query return shapes
    ├── actual-booking-result.projection
    └── invoice-summary.projection
```

## File Types

| Extension | Purpose | Contains |
|-----------|---------|----------|
| `.def` | Domain definition | Domain metadata, version, description |
| `.entity` | Entity definition | Attributes, relationships, belongs_to |
| `.key` | Entity identity | Primary key, ownership chain |
| `.value` | Value object | Immutable attributes |
| `.shared` | Shared/lookup data | External data projection |
| `.service` | Bounded context | Groups entities, commands, queries |
| `.command` | Command | Input, outcome, events |
| `.query` | Query | Parameters, return shape |
| `.event` | Domain event | Event data |
| `.enum` | Enumeration | Named values |
| `.flags` | Flags | Bitwise values |
| `.projection` | Projection | Query return shape |

## Domain Metadata

```
// domain.def
domain ChildcareManagement
    version "2.0"
    """
    Domain model for childcare centre management including
    bookings, attendance, billing, and family management.
    """

    services
        scheduling
        billing
        organisation
        family

    defaults
        text_max_length 200
        date_format ISO8601
end
```

## Service Definition

```
// services/scheduling.service
service Scheduling
    "Manages bookings, attendance, and sessions"

    owns
        Booking
        Attendance
        Absence
        Session
        Room

    uses
        Child [Name, DateOfBirth]
        Centre [Name, Token]
        Adult [Name, Contact]

    enums
        BookingStatus
        CareType
        AbsenceType

    commands
        PlaceBooking
        CancelBooking
        RecordAttendance
        RecordAbsence

    queries
        GetBookings
        GetActualBookings
        GetAttendanceReport
end
```

## Separation of Domain and Persistence

### Entity Definition (Domain)

```
// entities/booking.entity
entity Booking
    "Planned attendance for a child at a session"

    attributes
        Date: date "When attendance is planned"
        Status: BookingStatus

    has_one Session
    belongs_to Child
end
```

### Key Definition (Persistence)

```
// keys/booking.key
key Booking
    "Identity for Booking entity"

    identity
        BookingId: int generated

    ownership
        parent Child
        through ChildId

    indexes
        [Date, Session] unique
end
```

This separation allows:
- Domain stays clean of persistence concerns
- Keys can vary by storage technology
- Code generators combine as needed
