# Examples

Concrete examples of domain definitions using the proposed format.

## Complete Entity Example

### Domain Definition

```yaml
# entities/booking/booking.entity.yaml
entity: Booking
description: |
  Planned attendance for a child at a centre for a session.
  A booking represents the intention for a child to attend.
  It transitions through states as attendance is recorded.

attributes:
  Date:
    type: date
    description: Date when attendance is planned
    
  SessionStartTime:
    type: time
    description: Start time taken from session
    
  SessionEndTime:
    type: time
    description: End time taken from session
    
  AdjustedStartTime:
    type: time
    optional: true
    description: Adjusted start accounting for closures
    
  AdjustedEndTime:
    type: time
    optional: true
    description: Adjusted end accounting for closures
    
  AdjustedHours:
    type: decimal(5,2)
    optional: true
    description: Total adjusted hours
    
  Status:
    type: BookingStatus
    description: Current state of the booking
    
  AdjustedReason:
    type: BookingAdjustedReason
    optional: true
    description: Why hours were adjusted

relationships:
  Session:
    has_one: Session
    description: The session being booked
    
  Room:
    has_one: Room
    description: Room where attendance occurs
    
  Attendances:
    has_many: Attendance
    description: Recorded attendance instances
    
  Absence:
    has_one: Absence
    optional: true
    description: Absence record if applicable

belongs_to: Child
```

> Note: What can be done with a Booking is defined in behaviours, not in the entity itself.

### Key Definition

```yaml
# entities/booking/booking.key.yaml
key: Booking

identity:
  BookingId:
    type: integer
    generated: on_create

ownership:
  parent: Child
  
indexes:
  - fields: [Date, Session, Child]
    unique: true
    name: UX_Booking_DateSessionChild
```

---

## Value Object Example

```yaml
# values/address.value.yaml
value: Address
description: A physical or postal address

attributes:
  Line1:
    type: text(100)
    description: Street address line 1
    
  Line2:
    type: text(100)
    optional: true
    description: Street address line 2
    
  Suburb:
    type: text(50)
    description: Suburb or city
    
  State:
    type: AustralianState
    description: State or territory
    
  PostCode:
    type: text(4)
    pattern: "[0-9]{4}"
    description: Postal code
    
  Country:
    type: text(50)
    default: Australia
```

---

## Reference Data Example

```yaml
# references/organisation.ref.yaml
reference: Organisation
description: Organisation data from OrganisationService
source: OrganisationService

uses:
  Name:
    type: text
    description: Organisation display name
    
  Code:
    type: text
    description: Short identifier
    
  ABN:
    type: text
    description: Australian Business Number
    
  Status:
    type: OrganisationStatus
    description: Active/Inactive
```

---

## Command Example

```yaml
# commands/booking/record-attendance.cmd.yaml
command: RecordAttendance
description: Records that a child has arrived for their booking
owner: Booking

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
    
  AuthorisedBy:
    type: Adult
    optional: true
    description: Adult authorising if different from signer

outcome:
  creates:
    - Attendance
    
  changes:
    - Booking.Status: Attending
    
  publishes:
    - AttendanceRecorded
```

---

## Query Example

```yaml
# queries/booking/get-actual-bookings.query.yaml
query: GetActualBookings
description: Retrieves bookings with attendance and absence data
owner: Booking

parameters:
  Centre:
    type: Centre
    description: Centre to query bookings for
    
  FromDate:
    type: date
    description: Start of date range
    
  ToDate:
    type: date
    description: End of date range
    
  IncludeCancelled:
    type: boolean
    default: false
    description: Include cancelled bookings

returns:
  type: list of ActualBookingResult
```

---

## Enumeration Example

```yaml
# enums/booking-status.enum.yaml
enum: BookingStatus
description: Possible states of a booking

values:
  Planned:
    description: Booking is scheduled for the future
    
  Attending:
    description: Child has signed in, not yet signed out
    
  Attended:
    description: Child has signed in and out
    
  Absence:
    description: Child did not attend
    
  Cancelled:
    description: Booking was cancelled before attendance
```

---

## Service Example

```yaml
# services/scheduling.service.yaml
service: Scheduling
description: |
  Manages the scheduling of child attendance including
  bookings, sessions, and attendance recording.

entities:
  - Booking
  - Attendance
  - Absence
  - Session
  - Room
  - RoutineBookingSession
  - CasualBookingSession

values:
  - TimeRange

references:
  Child:
    uses: [Name, DateOfBirth, ChildNumber]
  Centre:
    uses: [Name, Token]
  Adult:
    uses: [Name, Contact]

enums:
  - BookingStatus
  - BookingAdjustedReason
  - AbsenceType
  - CareType

commands:
  - PlaceBooking
  - CancelBooking
  - RecordAttendance
  - RecordAbsence
  - AdjustBooking

queries:
  - GetBookings
  - GetActualBookings
  - GetAttendanceReport
  - GetAbsenceSummary
```

