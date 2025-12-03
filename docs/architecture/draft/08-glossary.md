# Glossary

Key terms used throughout the domain definition language.

## Domain Building Blocks

### Entity
A thing with identity and a lifecycle. Two entities with the same attributes but different identities are different things.

> *Example: Two children named "John Smith" born on the same day are still different children.*

### Value Object
A thing defined entirely by its attributes. Has no identity. Immutable - if you need to change it, you replace it.

> *Example: An address. If you move, you don't update the address - you have a new address.*

### Shared Data
Data that is used by this domain but owned and managed elsewhere. Read-only from this domain's perspective. Often includes lookup data like countries, status codes, or reference tables.

> *Example: Organisation details used by the Booking domain but managed by the Organisation domain.*

### Service
A collection of related entities, behaviours, and shared data that together represent a business capability. Sometimes called a "bounded context".

> *Example: The Scheduling service manages bookings, attendance, and sessions.*

---

## Behaviours

### Command
A business action that changes something. Named using action verbs.

> *Examples: "Place a booking", "Cancel an enrolment", "Record attendance"*

### Query
A business question that retrieves information without changing anything.

> *Examples: "Show today's bookings", "List children at centre", "Get attendance report"*

### Workflow
A behaviour that orchestrates multiple commands in a defined sequence or pattern. Represents a business process.

> *Example: "Process Enrolment" involves verifying details, creating arrangement, and notifying parties.*

---

## Events

Events signal that something has happened. They are always named in past tense.

### Domain Event
Something significant that happened within this domain. Other parts of the same domain may react to it.

> *Example: `AttendanceRecorded` - the Billing component might react to calculate charges.*

### Integration Event
A notification sent to other domains or external systems. Represents a contract with the outside world.

> *Example: `ChildEnrolmentChanged` - sent to government reporting systems.*

### Notification Event
A trigger for user-facing communication (email, SMS, push notification).

> *Example: `BookingReminderDue` - triggers a reminder to be sent to parents.*

---

## Relationships

### Belongs To
Indicates ownership. The child entity's lifecycle is tied to the parent.

> *Example: Attendance belongs to Booking - if the booking is removed, so are its attendances.*

### Has One / Has Many
Indicates association. The entity has a relationship with another entity.

> *Example: Booking has many Attendances, Booking has one Session.*

### Involves
Used in behaviours to indicate which entities participate in an action.

> *Example: "Transfer Child" involves Child, Source Centre, Destination Centre, and Enrolment.*

### Accessed Through
Used in behaviours to indicate the context in which an entity is accessed.

> *Example: When recording attendance, Child is accessed through Booking.*

---

## Data Concepts

### Projection
A view of data shaped for a specific purpose. May combine data from multiple entities.

> *Example: "Attendance Report" projection combines Booking, Child, and Attendance data.*

### Lookup
Simple shared data used for selection or validation. Often presented as dropdowns or lists.

> *Example: List of absence reasons, care types, or states.*

---

## Technical Terms (for implementers)

These terms appear in technical documentation but should not be in business-facing definitions:

| Technical Term | Business-Friendly Alternative |
|----------------|------------------------------|
| Aggregate Root | Entry point / Main entity |
| DTO | Data shape / Response |
| API | Interface / Integration |
| CRUD | Create, Update, Delete (or specific actions) |
| Foreign Key | Relationship / Link |
| Persistence | Storage |

