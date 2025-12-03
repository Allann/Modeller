# AI Agent Integration

This document describes how the domain definition format is designed for AI agent consumption and generation.

## Design for AI

### Predictable Schema

AI agents work best with consistent, predictable structures:

```yaml
# Every entity follows the same pattern
entity: [Name]
description: [Text]

attributes:
  [FieldName]:
    type: [DataType]
    description: [Text]
    optional: [boolean]
    
relationships:
  [RelationshipName]:
    has_one|has_many: [EntityName]
    description: [Text]
    
belongs_to: [ParentEntity]

can:
  - [action verb phrase]
```

### Semantic Keys

Keywords convey meaning that AI can understand:

| Keyword | Semantic Meaning |
|---------|------------------|
| `is` | Definition/description |
| `has` | Owns attributes |
| `belongs_to` | Parent relationship |
| `has_one` | Single relationship |
| `has_many` | Collection relationship |
| `can` | Available behaviours |
| `uses` | Reference data fields |
| `publishes` | Events produced |
| `changes` | State mutations |

### Self-Documenting

Every element requires a description, enabling AI to:
- Explain what concepts mean
- Generate appropriate implementations
- Answer questions about the domain

---

## AI Capabilities

### 1. Domain Understanding

AI can answer questions about the domain:

```
Q: "What is a Booking?"
A: Parse entity.description → "Planned attendance for a child..."

Q: "What can you do with a Booking?"
A: Parse entity.can → "place, cancel, record attendance, record absence"

Q: "What does a Booking contain?"
A: Parse entity.attributes → "Date, Status, Session times..."
```

### 2. Definition Generation

AI can generate definitions from natural language:

```
User: "I need to track vehicle maintenance. A vehicle belongs to a 
       depot and can have scheduled services. Each service has a 
       date, type, and cost."

AI generates:
- Vehicle.entity.yaml
- MaintenanceService.entity.yaml  
- ServiceType.enum.yaml
- ScheduleService.cmd.yaml
- GetVehicleServices.query.yaml
```

### 3. Validation

AI can validate definitions for:
- Required fields present
- Relationship consistency
- Naming conventions
- Description completeness

### 4. Code Generation Guidance

AI understands what code to generate:

```yaml
entity: Booking
can:
  - be cancelled  # → CancelBooking command
                  # → BookingCancelled event
                  # → Booking.Cancel() method
```

---

## Query Interface

Structured queries AI can execute against definitions:

```yaml
# List all entities in a service
query:
  type: entities
  service: Scheduling
  
# Find commands that affect an entity
query:
  type: commands
  affects: Booking
  
# Get relationships for an entity
query:
  type: relationships
  entity: Booking
  
# Find all events in domain
query:
  type: events
  
# Search by description
query:
  type: search
  text: "attendance"
```

---

## Conversation Patterns

### Exploring the Domain

```
User: "Describe the scheduling domain"
AI: [Reads scheduling.service.yaml]
    "The Scheduling service manages bookings, attendance, and sessions.
     It owns entities: Booking, Attendance, Absence, Session, Room.
     Key operations include placing bookings and recording attendance."

User: "How does attendance work?"
AI: [Reads Booking.entity.yaml, RecordAttendance.cmd.yaml]
    "When a child arrives, the RecordAttendance command is used.
     It requires the booking, arrival time, and the adult who signed in.
     This changes the booking status to 'Attending' and creates an
     Attendance record."
```

### Modifying the Domain

```
User: "Add a notes field to Booking"
AI: [Modifies booking.entity.yaml]
    "I've added an optional Notes attribute to Booking:
     Notes:
       type: text(500)
       optional: true
       description: Additional notes about the booking"

User: "Bookings should be able to be rescheduled"
AI: [Creates reschedule-booking.cmd.yaml]
    "I've created a RescheduleBooking command with:
     - Input: Booking, NewDate, NewSession, Reason
     - Changes: Booking.Date, Booking.Session
     - Publishes: BookingRescheduled"
```

---

## JSON Schema

For tooling and validation, provide JSON Schema:

```json
{
  "$schema": "http://json-schema.org/draft-07/schema#",
  "$id": "https://modeller.dev/schema/entity.json",
  "type": "object",
  "required": ["entity", "description", "attributes"],
  "properties": {
    "entity": { "type": "string" },
    "description": { "type": "string" },
    "attributes": {
      "type": "object",
      "additionalProperties": {
        "$ref": "#/definitions/attribute"
      }
    },
    "relationships": {
      "type": "object",
      "additionalProperties": {
        "$ref": "#/definitions/relationship"  
      }
    },
    "belongs_to": { "type": "string" },
    "can": {
      "type": "array",
      "items": { "type": "string" }
    }
  }
}
```

This enables:
- IDE autocompletion
- Validation tooling
- AI-assisted editing

