# Data Types

This document defines the type system for domain definitions.

## Primitive Types

| Type | Description | Example |
|------|-------------|---------|
| `text` | Variable-length string | `text`, `text(100)` |
| `int` | Whole number | `int`, `int(min=0)` |
| `decimal` | Fixed-point number | `decimal(10,2)` |
| `bool` | True/false | `bool` |
| `date` | Date without time | `date` |
| `time` | Time without date | `time` |
| `datetime` | Date and time | `datetime` |
| `guid` | Unique identifier | `guid` |
| `binary` | Binary data | `binary` |

## Type Modifiers

### Text Options

```
attributes
    Name: text(100)                    // Max 100 characters
    Code: text(2..10)                  // Between 2 and 10 characters
    Email: text format email           // Semantic format hint
    Phone: text pattern "[0-9]{10}"    // Regex pattern
```

### Numeric Options

```
attributes
    Age: int min 0 max 150
    Price: decimal(10,2) min 0         // 10 digits, 2 decimal places
    Percentage: decimal(5,2) min 0 max 100
```

### Common Formats

| Format | Meaning |
|--------|---------|
| `email` | Email address |
| `url` | Web URL |
| `phone` | Phone number |
| `currency` | Money amount |
| `percentage` | 0-100 value |

---

## Complex Types

### Enumeration Reference

```
attributes
    Status: BookingStatus              // Reference to enum
```

### Entity Reference

attributes
    Session: Session                   // Reference to entity
```

### Value Object Reference

```
attributes
    HomeAddress: Address               // Reference to value object
```

---

## Collections

```
// List of primitives
attributes
    Tags: list of text

// List of entities (relationship)
has_many Attendance as Attendances

// List of value objects
attributes
    Addresses: list of Address
```

---

## Optional and Defaults

```
attributes
    // Required (default)
    Name: text

    // Optional (use ? suffix)
    MiddleName: text?

    // Default value
    Country: text = "Australia"

    // Computed (read-only)
    FullName: text computed "{FirstName} {LastName}"
```

---

## Domain-Specific Types

Define reusable types for your domain:

```
// enums/australian-state.enum
enum AustralianState
    "Australian state or territory code"

    NSW "New South Wales"
    VIC "Victoria"
    QLD "Queensland"
    WA "Western Australia"
    SA "South Australia"
    TAS "Tasmania"
    ACT "Australian Capital Territory"
    NT "Northern Territory"
end
```

Usage:
```
attributes
    State: AustralianState
```

---

## Type Inference

For AI generation, types can often be inferred:

| Field Name Pattern | Inferred Type |
|-------------------|---------------|
| `*Id` | guid or int |
| `*Date`, `*At`, `*On` | date or datetime |
| `*Time` | time |
| `*Name`, `*Title` | text |
| `*Email` | text (email format) |
| `*Phone` | text (phone format) |
| `*Amount`, `*Price`, `*Cost` | decimal |
| `*Count`, `*Number`, `*Quantity` | int |
| `Is*`, `Has*`, `Can*` | bool |
| `*Status`, `*Type`, `*Category` | enum reference |

---

## Mapping to Implementation

| Domain Type | C# | SQL Server | PostgreSQL |
|-------------|-----|------------|------------|
| `text` | `string` | `nvarchar` | `varchar` |
| `text(n)` | `string` | `nvarchar(n)` | `varchar(n)` |
| `int` | `int` | `int` | `integer` |
| `decimal(p,s)` | `decimal` | `decimal(p,s)` | `numeric(p,s)` |
| `bool` | `bool` | `bit` | `boolean` |
| `date` | `DateOnly` | `date` | `date` |
| `time` | `TimeOnly` | `time` | `time` |
| `datetime` | `DateTime` | `datetime2` | `timestamp` |
| `guid` | `Guid` | `uniqueidentifier` | `uuid` |

Mapping is handled by code generators, keeping domain definitions clean.
