# Behaviours

Behaviours define **what can be done** within the domain. They are separated into:

- **Commands** - Actions that change things (e.g., "Cancel a booking")
- **Queries** - Questions that retrieve information (e.g., "Show me today's bookings")
- **Workflows** - Processes that orchestrate multiple commands

This separation reflects how business naturally thinks: doing things, asking questions, and following processes.

## Commands

Commands represent **business actions** that change state. They are named using action verbs that describe what the business does.

### Characteristics

- Changes state
- Has a single responsibility
- May publish events
- Named as business actions (not technical CRUD)
- Preconditions handled by rules engine (future)

### Definition Structure

```yaml
command: RecordAttendance
description: Records that a child has arrived for their booked session

involves:
  Booking: accessed through
  Attendance: creates
  Child: reads
  Adult: reads (the person signing in)

input:
  Booking:
    type: Booking
    description: The booking to record attendance for

  TimeIn:
    type: time
    description: When the child arrived

  SignedInBy:
    type: Adult
    description: Adult who signed the child in

outcome:
  creates:
    - Attendance

  changes:
    - Booking.Status: Attending

  publishes:
    - AttendanceRecorded
```

### The `involves` Keyword

The `involves` section defines which entities participate in a behaviour and how:

| Relationship | Meaning |
|--------------|---------|
| `accessed through` | This is the main entity; others are accessed via it |
| `reads` | Information is read from this entity |
| `creates` | A new instance of this entity is created |
| `updates` | This entity is modified |
| `removes` | This entity is deleted |

### Business Actions vs CRUD

Prefer business language over technical operations:

| Instead of | Use |
|------------|-----|
| CreateBooking | PlaceBooking, ScheduleAttendance |
| UpdateBooking | RescheduleBooking, AdjustSession |
| DeleteBooking | CancelBooking, WithdrawBooking |
| UpdateStatus | RecordAttendance, RecordAbsence |

### Simple CRUD Entities

For straightforward lookup/shared data where CRUD is appropriate:

```yaml
entity: AbsenceReason
description: Reasons why a child may be absent
crud: all  # Generates standard Create, Update, Delete commands

attributes:
  Name:
    type: text
    description: Display name of the reason
```

This generates standard commands: Create, Update, Delete.

---

## Queries

Queries retrieve data without modifying state. They return projections of domain data.

### Characteristics

- Read-only, no side effects
- Returns a projection (may differ from entity structure)
- Can aggregate data from multiple entities
- Defines what information is returned

### Definition Structure

```yaml
query: GetActualBookings
description: Retrieves bookings with attendance and absence data
owner: Booking

parameters:
  Centre:
    type: Centre
    description: The centre to query
    
  DateRange:
    type: DateRange
    description: Period to include
    
  IncludeAbsences:
    type: boolean
    default: true
    description: Whether to include absence records

returns:
  type: list
  shape: ActualBookingResult
  description: Bookings grouped by room
```

### Projections

Queries return projections, not raw entities:

```yaml
projection: ActualBookingResult
description: Booking with attendance details for reporting

shape:
  BookingDate:
    from: Booking.Date
    
  ChildName:
    from: Booking.Child.Name
    format: "{FirstName} {LastName}"
    
  SessionName:
    from: Booking.Session.Name
    
  Status:
    from: Booking.Status
    
  Attendances:
    from: Booking.Attendances
    shape: AttendanceResult
    
  HasAbsence:
    computed: Booking.Absence is not null
```

---

## Command vs Query Summary

| Aspect | Command | Query |
|--------|---------|-------|
| Purpose | Change state | Read state |
| Side effects | Yes | No |
| Returns | Success/failure | Data |
| Naming | Imperative verb | Get/Find/List |
| Caching | Not cacheable | Cacheable |
| Events | May publish | Never publishes |

---

## Workflows

Workflows are behaviours that orchestrate multiple commands in a sequence. They represent business processes.

```yaml
workflow: ProcessEnrolment
description: Complete enrolment of a child at a centre

involves:
  Child: reads, updates
  Centre: reads
  Arrangement: creates
  Family: reads, notifies

steps:
  - VerifyChildDetails:
      description: Ensure child information is complete
      command: ValidateChild

  - CreateArrangement:
      description: Set up the care arrangement
      command: CreateArrangement
      after: VerifyChildDetails

  - NotifyParties:
      description: Inform relevant parties
      commands:
        - NotifyCentre
        - NotifyFamily
      parallel: true
      after: CreateArrangement

outcome:
  publishes:
    - EnrolmentCompleted (when successful)
    - EnrolmentFailed (when unsuccessful)
```

---

## Events

Events signal that something has happened. They are always named in **past tense**.

### Event Types

| Type | Purpose | Subscribers |
|------|---------|-------------|
| **Domain Event** | Something happened in this domain | Other parts of same domain |
| **Integration Event** | Notify other systems | External systems/domains |
| **Notification Event** | Trigger communication | Notification services |

### Domain Event

```yaml
event: AttendanceRecorded
type: domain
description: A child's attendance has been recorded
source: RecordAttendance

data:
  Booking:
    type: Booking
    description: The booking attendance was recorded for

  Attendance:
    type: Attendance
    description: The attendance record created

  RecordedAt:
    type: datetime
    description: When the attendance was recorded
```

### Integration Event

```yaml
event: ChildEnrolmentChanged
type: integration
description: Notifies external systems of enrolment changes
source: ProcessEnrolment

data:
  ChildId:
    type: text
    description: External identifier for the child

  CentreId:
    type: text
    description: External identifier for the centre

  ChangeType:
    type: text
    description: Type of change (New, Modified, Ended)
```

### Event Lifecycle

1. **Published** - Command completes and publishes event
2. **Delivered** - Event is sent to subscribers
3. **Handled** - Subscriber processes the event
4. **Acknowledged** - Processing confirmed complete

### Subscribing to Events

Other behaviours can react to events:

```yaml
command: CalculateBookingCharges
description: Calculate charges when attendance is recorded
triggered_by: AttendanceRecorded

involves:
  Attendance: reads
  Booking: reads
  Charges: creates
```
