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

```
entity Booking
    """
    Planned attendance for a child at a centre for a session.
    Transitions through states as attendance is recorded.
    """

    attributes
        Date: date "When attendance is planned"
        Status: BookingStatus "Current state of the booking"
        AdjustedHours: decimal? "Total adjusted hours after modifications"

    has_one Session "The session being booked"
    has_many Attendance as Attendances "Recorded attendance instances"
    has_one Absence? "Absence record if child did not attend"

    belongs_to Child
end
```

### Key Concepts

| Keyword | Meaning |
|---------|---------|
| `attributes` | Data the entity holds |
| `has_one` | Single related entity |
| `has_many` | Collection of related entities |
| `belongs_to` | Parent entity (ownership) |
| `?` suffix | Optional (nullable) |

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

```
value Address
    "A physical or postal address"

    attributes
        Street: text "Street number and name"
        Suburb: text "Suburb or city"
        State: State "State or territory"
        PostCode: text(4) "Postal code"
        Country: text = "Australia" "Country name"
end
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

```
shared Organisation
    "Organisation data managed by the Organisation service"
    source OrganisationService

    attributes
        Name: text "Organisation name"
        Code: text "Short code identifier"
        Status: OrganisationStatus "Active/Inactive state"
end
```

### Lookup Data

Simple shared data used for selection (dropdowns, lists):

```
shared AbsenceReason
    "Standard reasons for child absences"
    lookup

    attributes
        Code: text "Short code"
        Name: text "Display name"
        IsChargeable: bool "Whether this reason incurs charges"
end
```

### Usage in Entities

```
entity Centre
    "A childcare centre"

    attributes
        Name: text "Centre name"

    uses Organisation from OrganisationService [Name, Code]
        "The organisation that owns this centre"
end
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

```
enum BookingStatus
    "Possible states of a booking"

    Planned "Booking is scheduled for the future"
    Attending "Child has signed in"
    Attended "Child has signed out"
    Absence "Child did not attend"
    Cancelled "Booking was cancelled"
end
```

### Flags

Flags are enumerations that can be combined (bitwise).

```
flags DaysOfWeek
    "Set of days, can select multiple"

    Monday = 1
    Tuesday = 2
    Wednesday = 4
    Thursday = 8
    Friday = 16
    Saturday = 32
    Sunday = 64
end
```
