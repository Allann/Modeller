# Data Types

This document defines the type system for domain definitions.

## Primitive Types

| Type | Description | Example |
|------|-------------|---------|
| `text` | Variable-length string | `text`, `text(100)` |
| `integer` | Whole number | `integer`, `integer(min=0)` |
| `decimal` | Fixed-point number | `decimal(10,2)` |
| `boolean` | True/false | `boolean` |
| `date` | Date without time | `date` |
| `time` | Time without date | `time` |
| `datetime` | Date and time | `datetime` |
| `guid` | Unique identifier | `guid` |
| `binary` | Binary data | `binary` |

## Type Modifiers

### Text Options

```yaml
Name:
  type: text(100)           # Max 100 characters
  
Code:
  type: text(2..10)         # Between 2 and 10 characters
  
Email:
  type: text
  format: email             # Semantic format hint
  
Phone:
  type: text
  pattern: "[0-9]{10}"      # Regex pattern
```

### Numeric Options

```yaml
Age:
  type: integer
  min: 0
  max: 150
  
Price:
  type: decimal(10,2)       # 10 digits, 2 decimal places
  min: 0
  
Percentage:
  type: decimal(5,2)
  min: 0
  max: 100
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

```yaml
Status:
  type: BookingStatus       # Reference to enum
```

### Entity Reference

```yaml
Session:
  type: Session             # Reference to entity
```

### Value Object Reference

```yaml
HomeAddress:
  type: Address             # Reference to value object
```

---

## Collections

```yaml
# List of primitives
Tags:
  type: list of text
  
# List of entities (relationship)
Attendances:
  has_many: Attendance
  
# List of value objects
Addresses:
  type: list of Address
```

---

## Optional and Defaults

```yaml
# Required (default)
Name:
  type: text

# Optional
MiddleName:
  type: text
  optional: true
  
# Default value
Country:
  type: text
  default: Australia
  
# Computed (read-only)
FullName:
  type: text
  computed: "{FirstName} {LastName}"
```

---

## Domain-Specific Types

Define reusable types for your domain:

```yaml
# types/australian-state.type.yaml
type: AustralianState
base: text(3)
description: Australian state or territory code

allowed:
  - NSW: New South Wales
  - VIC: Victoria
  - QLD: Queensland
  - WA: Western Australia
  - SA: South Australia
  - TAS: Tasmania
  - ACT: Australian Capital Territory
  - NT: Northern Territory
```

Usage:
```yaml
State:
  type: AustralianState
```

---

## Type Inference

For AI generation, types can often be inferred:

| Field Name Pattern | Inferred Type |
|-------------------|---------------|
| `*Id` | guid or integer |
| `*Date`, `*At`, `*On` | date or datetime |
| `*Time` | time |
| `*Name`, `*Title` | text |
| `*Email` | text (email format) |
| `*Phone` | text (phone format) |
| `*Amount`, `*Price`, `*Cost` | decimal |
| `*Count`, `*Number`, `*Quantity` | integer |
| `Is*`, `Has*`, `Can*` | boolean |
| `*Status`, `*Type`, `*Category` | enum reference |

---

## Mapping to Implementation

| Domain Type | C# | SQL Server | PostgreSQL |
|-------------|-----|------------|------------|
| `text` | `string` | `nvarchar` | `varchar` |
| `text(n)` | `string` | `nvarchar(n)` | `varchar(n)` |
| `integer` | `int` | `int` | `integer` |
| `decimal(p,s)` | `decimal` | `decimal(p,s)` | `numeric(p,s)` |
| `boolean` | `bool` | `bit` | `boolean` |
| `date` | `DateOnly` | `date` | `date` |
| `time` | `TimeOnly` | `time` | `time` |
| `datetime` | `DateTime` | `datetime2` | `timestamp` |
| `guid` | `Guid` | `uniqueidentifier` | `uuid` |

Mapping is handled by code generators, keeping domain definitions clean.

