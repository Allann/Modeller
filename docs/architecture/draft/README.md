# Domain Definition Language - Draft Specification

> **Status**: Draft - Work in Progress

This folder contains the draft specification for a natural language-oriented domain definition format designed for:

1. **Business readability** - Definitions read like documentation that business stakeholders can review
2. **Technology agnostic** - Describes *what* the business does, not *how* systems implement it
3. **AI agent friendly** - Structured for AI understanding and generation
4. **Code generation ready** - Translatable to multiple output formats

## Core Principles

### Separation of Concerns

| Concern | Belongs In | Does NOT Belong In |
|---------|------------|-------------------|
| **Domain Model** | Entity/Value Object definitions | Database IDs, technical keys |
| **Persistence** | Separate storage definitions | Domain definitions |
| **Interfaces** | Command/Query definitions | Entity internals |
| **Business Rules** | Rules engine (future) | Scattered in definitions |

### Domain Building Blocks

```
┌─────────────────────────────────────────────────────────────┐
│                         SERVICE                              │
│  (Bounded Context)                                          │
│                                                             │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐ │
│  │  ENTITIES   │  │   VALUE     │  │    REFERENCE        │ │
│  │             │  │   OBJECTS   │  │    DATA             │ │
│  │ - Identity  │  │ - No ID     │  │ - External          │ │
│  │ - Lifecycle │  │ - Immutable │  │ - Read-only         │ │
│  │ - Mutable   │  │ - Equality  │  │ - Not owned         │ │
│  └─────────────┘  └─────────────┘  └─────────────────────┘ │
│                                                             │
│  ┌─────────────────────────┐  ┌───────────────────────────┐│
│  │       COMMANDS          │  │         QUERIES           ││
│  │ (Business Actions)      │  │ (Business Questions)      ││
│  │ - Change things         │  │ - Retrieve information    ││
│  │ - Notify when done      │  │ - No changes made         ││
│  └─────────────────────────┘  └───────────────────────────┘│
└─────────────────────────────────────────────────────────────┘
```

## Documents

| Document | Description |
|----------|-------------|
| [Overview](01-overview.md) | Philosophy and design goals |
| [Domain Concepts](02-domain-concepts.md) | Entities, Value Objects, Shared Data |
| [Behaviours](03-behaviours.md) | Commands, Queries, Workflows, and Events |
| [File Structure](04-file-structure.md) | Proposed file organization |
| [Examples](05-examples.md) | Concrete definition examples |
| [AI Integration](06-ai-integration.md) | How AI agents consume and generate definitions |
| [Data Types](07-data-types.md) | Type system reference |
| [Glossary](08-glossary.md) | Key terms and definitions |
| [Format Options](09-format-options.md) | DSL vs YAML vs JSON trade-offs |
| [Templates](10-templates.md) | Code generation templates and engines |

## Design Goals

1. **Business-first** - Technical concerns (storage, interfaces) are separate
2. **Natural language** - Reads like English, understandable by non-technical stakeholders
3. **AI-friendly** - Clear semantics for AI understanding and generation
4. **Version-aware** - Support for evolution over time
5. **Composable** - Services can reference other services' data

