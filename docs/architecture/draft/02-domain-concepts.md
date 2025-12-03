# Domain Concepts

The domain layer consists of three primary building blocks: **Entities**, **Value Objects**, and **Reference Data**.

## Entities

An entity has **identity** and a **lifecycle**. Two entities with the same attributes are still different if they have different identities.

### Characteristics

- Has a unique identity (but identity is defined separately from domain)
- Has a lifecycle (created, modified, potentially deleted)
- Mutable - state can change over time
- Equality based on identity, not attributes

### Definition Structure

```yaml
entity: Booking
description: |
  Planned attendance for a child at a centre for a session.
  Transitions through states as attendance is recorded.

attributes:
  Date:
    type: date
    description: When attendance is planned
    
  Status:
    type: BookingStatus
    description: Current state of the booking
    
  AdjustedHours:
    type: decimal
    optional: true
    description: Total adjusted hours after modifications

relationships:
  Session:
    has_one: Session
    description: The session being booked
    
  Attendances:
    has_many: Attendance
    description: Recorded attendance instances
    
  Absence:
    has_one: Absence
    optional: true
    description: Absence record if child did not attend

belongs_to: Child
```

### Key Concepts

| Keyword | Meaning |
|---------|---------|
| `attributes` | Data the entity holds |
| `relationships` | Connections to other entities |
| `belongs_to` | Parent entity (ownership) |
| `has_one` | Single related entity |
| `has_many` | Collection of related entities |

---

## Value Objects

A value object has **no identity**. It is defined entirely by its attributes. Two value objects with the same attributes are equal.

### Characteristics

- No unique identity
- Immutable - once created, cannot change
- Equality based on all attributes
- Often embedded within entities
- Replaceable, not updatable

### Definition Structure

```yaml
value: Address
description: A physical or postal address

attributes:
  Street:
    type: text
    description: Street number and name
    
  Suburb:
    type: text
    description: Suburb or city
    
  State:
    type: State
    description: State or territory
    
  PostCode:
    type: text(4)
    description: Postal code
    
  Country:
    type: text
    default: Australia
    description: Country name
```

### Common Value Objects

| Value Object | Use Case |
|--------------|----------|
| Address | Physical locations |
| Money | Currency amounts |
| DateRange | Period of time |
| PersonName | First/Last/Title |
| PhoneNumber | Contact numbers |
| EmailAddress | Email with validation |

---

## Shared Data

Shared data is **owned by another domain** but used within this domain. It is read-only from this domain's perspective. This often includes lookup data used for selection or validation.

### Characteristics

- Not owned by this service/domain
- Read-only - cannot be modified here
- May be cached locally
- Only the fields you need (projection)
- Updated through external sync or refresh

### Definition Structure

```yaml
shared: Organisation
description: Organisation data managed by the Organisation service
source: OrganisationService

uses:
  Name:
    type: text
    description: Organisation name

  Code:
    type: text
    description: Short code identifier

  Status:
    type: OrganisationStatus
    description: Active/Inactive state
```

### Lookup Data

Simple shared data used for selection (dropdowns, lists):

```yaml
shared: AbsenceReason
description: Standard reasons for child absences
lookup: true

uses:
  Code:
    type: text
    description: Short code

  Name:
    type: text
    description: Display name

  IsChargeable:
    type: boolean
    description: Whether this reason incurs charges
```

### Usage in Entities

```yaml
entity: Centre
description: A childcare centre

attributes:
  Name:
    type: text
    description: Centre name

shared:
  Organisation:
    from: OrganisationService
    uses: [Name, Code]
    description: The organisation that owns this centre
```

---

## Comparison

| Aspect | Entity | Value Object | Shared Data |
|--------|--------|--------------|-------------|
| Identity | Has unique ID | No ID | External ID |
| Mutability | Mutable | Immutable | Read-only |
| Lifecycle | Managed here | Created/replaced | Managed elsewhere |
| Ownership | Owned by domain | Embedded | External |
| Equality | By identity | By attributes | By external ID |

---

## Enumerations

Enumerations define a fixed set of named values.

```yaml
enum: BookingStatus
description: Possible states of a booking

values:
  Planned: Booking is scheduled for the future
  Attending: Child has signed in
  Attended: Child has signed out
  Absence: Child did not attend
  Cancelled: Booking was cancelled
```

### Flags

Flags are enumerations that can be combined (bitwise).

```yaml
flags: DaysOfWeek
description: Set of days, can select multiple

values:
  Monday: 1
  Tuesday: 2
  Wednesday: 4
  Thursday: 8
  Friday: 16
  Saturday: 32
  Sunday: 64
```

