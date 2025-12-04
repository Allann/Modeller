# Overview

## Philosophy

A domain definition should describe **what the business cares about**, not how it's stored or transmitted. Technical concerns like database keys, serialization formats, and interface versioning are important but belong in separate layers.

```
┌─────────────────────────────────────────┐
│           DOMAIN DEFINITION             │  ← What the business understands
│  "A Booking is planned attendance..."   │
├─────────────────────────────────────────┤
│           PERSISTENCE LAYER             │  ← How it's stored
│  Keys, indexes, storage relationships   │
├─────────────────────────────────────────┤
│           INTEGRATION LAYER             │  ← How it's exposed
│  Interfaces, versions, contracts        │
└─────────────────────────────────────────┘
```

## Design Goals

### 1. Domain-First Modeling

Define concepts in business terms:

```yaml
# Good - describes the business concept
Adult:
  is: A person responsible for children at the centre
  has:
    Name: person's full name
    Contact: how to reach them
    
# Avoid - leaks persistence concerns
Adult:
  AdultId: auto-increment integer
  FK_OrganisationId: foreign key to Organisation
```

### 2. Natural Language Orientation

Definitions should read like documentation:

```yaml
Booking:
  is: |
    Planned attendance for a child at a centre for a session.
    Transitions through: planned → attending → attended (or absence).
    
  has:
    Date: when attendance is planned
    Session: the session being booked
    Status: current state of the booking
    
  belongs to: Child
```

> Note: What can be done (behaviours) is defined separately - see [Behaviours](03-behaviours.md).

### 3. AI Agent Compatibility

Structured for machine understanding:

- **Predictable schema** - AI knows what fields to expect
- **Semantic keys** - `has`, `belongs to`, `involves` convey meaning
- **Self-documenting** - Every element has a description
- **Queryable** - "What can be done with a Booking?" → parse related commands

### 4. Separation of Concerns

| File Type | Contains | Example |
|-----------|----------|---------|
| `.entity` | Domain structure | `Booking.entity.yaml` |
| `.key` | Identity/persistence | `Booking.key.yaml` |
| `.cmd` | Commands (mutations) | `CancelBooking.cmd.yaml` |
| `.query` | Queries (reads) | `GetBookings.query.yaml` |
| `.enum` | Enumerations | `BookingStatus.enum.yaml` |
| `.service` | Bounded context | `Scheduling.service.yaml` |

### 5. Version Support

Interface evolution without domain pollution:

```yaml
# Domain stays clean
Booking:
  has:
    Status: current state

# Versioning is separate - for when systems need different views
Booking.v1:
  version: v1
  maps_to: Booking
  shape:
    status: Status.name  # v1 returns name only

Booking.v2:
  version: v2
  maps_to: Booking
  shape:
    status: Status  # v2 returns full status
```

## What This Enables

1. **Generate multiple outputs** from single source of truth
2. **AI can read and generate** valid definitions
3. **Business stakeholders** can review and discuss definitions
4. **Versioned interfaces** without domain model changes
5. **Rules engine integration** for business logic (future)

