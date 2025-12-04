# Modeller Documentation

Modeller is a **code generation tool** that transforms domain definitions into source code using pluggable templates.

## Overview

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   Definitions   │───>│    Semantic      │───>│    Templates    │───>│   Output    │
│   (.entity,     │    │    Domain Model  │    │   (Generators)  │    │  (C#, SQL,  │
│    .command,    │    │                  │    │                 │    │   etc.)     │
│    .query...)   │    │                  │    │                 │    │             │
└─────────────────┘    └──────────────────┘    └─────────────────┘    └─────────────┘
        │                      │
        │   Pidgin Parser      │
        └──────────────────────┘
```

---

## 🚀 Current Implementation

The next generation of Modeller uses a **custom Domain-Specific Language (DSL)** parsed with [Pidgin](https://github.com/benjamin-hodgson/Pidgin) parser combinators.

### What's Built

| Component | Status | Description |
|-----------|--------|-------------|
| **DSL Parser** | ✅ Complete | Pidgin-based parser for all definition types |
| **Domain Models** | ✅ Complete | Immutable, validated record types with factory methods |
| **VS Code Extension** | ✅ Complete | Syntax highlighting, icon theme, language support |
| **Sample Definitions** | ✅ Complete | Full example domain in `samples/modeller/` |
| **Code Generators** | 🔄 In Progress | C# record generator started |

### DSL File Types

| Extension | Purpose | Example |
|-----------|---------|---------|
| `.def` | Domain definition | `domain.def` |
| `.entity` | Entity definitions | `booking.entity` |
| `.value` | Value objects | `address.value` |
| `.shared` | Lookup/reference data | `country.shared` |
| `.enum` | Enumerations | `status.enum` |
| `.flags` | Flag enumerations | `permissions.flags` |
| `.command` | Commands (write operations) | `create-booking.command` |
| `.query` | Queries (read operations) | `get-bookings.query` |
| `.event` | Domain events | `booking-created.event` |
| `.key` | Key definitions | `booking.key` |
| `.service` | Service/bounded context | `scheduling.service` |
| `.projection` | Read model projections | `booking-summary.projection` |

### Example DSL Syntax

```
entity Booking
    "Planned attendance for a child at a centre for a session"

    attributes
        Date: date "When attendance is planned"
        Status: BookingStatus

    belongs_to Child
end
```

### Project Structure

```
Modeller/
├── src/
│   ├── Modeller.Parser/       # Pidgin-based DSL parser
│   ├── Modeller.Domain/       # Semantic domain models
│   └── Modeller.Generator/    # Code generators
├── editors/
│   └── vscode-modeller/       # VS Code extension
├── samples/
│   └── modeller/              # Example domain definitions
├── tests/
│   ├── Modeller.Parser.Tests/
│   └── Modeller.Integration.Tests/
└── docs/
    └── architecture/draft/    # Design specifications
```

---

## 📖 Specification Documents

| Document | Description |
|----------|-------------|
| **[Language Specification](architecture/draft/README.md)** | DSL design and core concepts |
| [Domain Concepts](architecture/draft/02-domain-concepts.md) | Entities, Value Objects, Shared Data |
| [Behaviours](architecture/draft/03-behaviours.md) | Commands, Queries, Workflows, Events |
| [File Structure](architecture/draft/04-file-structure.md) | Project organization |
| [Examples](architecture/draft/05-examples.md) | Concrete definition examples |
| [AI Integration](architecture/draft/06-ai-integration.md) | AI agent compatibility |
| [Data Types](architecture/draft/07-data-types.md) | Type system reference |
| [Format Options](architecture/draft/09-format-options.md) | Why we chose custom DSL over YAML |
| [Templates](architecture/draft/10-templates.md) | Code generation templates |

---

## 🛠️ VS Code Extension

The VS Code extension provides:
- **Syntax highlighting** for all `.entity`, `.command`, `.query`, etc. files
- **Custom file icons** for each definition type
- **Language configuration** for comments, brackets, etc.

See [editors/vscode-modeller/README.md](../editors/vscode-modeller/README.md) for installation.

---

## 📦 Legacy Implementation Reference

The documents below describe the **previous C# fluent API implementation**. These are preserved for reference.

| Document | Description |
|----------|-------------|
| [Architecture](architecture.md) | Legacy system architecture |
| [Definitions](definitions.md) | Legacy C# fluent API for definitions |
| [Templates](templates.md) | Legacy C# template system |
| [CLI Reference](cli-reference.md) | Legacy command-line tool |
| [API Reference](api-reference.md) | Legacy interfaces and classes |
| [Data Types](data-types.md) | Data type reference |
| [Getting Started](getting-started.md) | Legacy quick start guide |

---

## Key Concepts

### Domain
The top-level container representing your entire domain. Contains services, entities, enumerations, commands, and queries.

### Service
A bounded context grouping related entities and behaviours.

### Entity
A domain object with identity, attributes, keys, and relationships.

### Value Object
An immutable object defined by its attributes, not identity (e.g., Address, Money).

### Shared Data
Lookup or reference data shared across the domain (e.g., Countries, Currencies).

### Behaviours
Commands (write operations), queries (read operations), and events that define what can be done in the domain.

---

## License

See the repository root for license information.
