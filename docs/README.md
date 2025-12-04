# Modeller Documentation

Modeller is a **code generation tool** that transforms domain definitions into source code using pluggable templates.

## Overview

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   Definitions   │───>│    Templates     │───>│     Output      │
│  (Domain Model) │    │   (Generators)   │    │  (Files/Code)   │
└─────────────────┘    └──────────────────┘    └─────────────────┘
```

- **Definitions** describe your domain model (entities, services, fields, relationships)
- **Templates** are code generators that transform definitions into source code
- **Output** is the generated code (files, projects, solutions)

---

## 🚀 Future Direction (Recommended Reading)

This repository is focused on designing the **next generation** of Modeller using a natural language-oriented, YAML-based domain definition format.

| Document | Description |
|----------|-------------|
| **[Future Specification](architecture/draft/README.md)** | **Start here** - The new domain definition language |
| [Domain Concepts](architecture/draft/02-domain-concepts.md) | Entities, Value Objects, Shared Data |
| [Behaviours](architecture/draft/03-behaviours.md) | Commands, Queries, Workflows, Events |
| [Examples](architecture/draft/05-examples.md) | Concrete definition examples |
| [AI Integration](architecture/draft/06-ai-integration.md) | AI agent compatibility |

---

## 📦 Legacy Implementation Reference

The documents below describe the **existing C# fluent API implementation** (in a separate repository). These are preserved for reference but represent the approach being replaced.

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

## Quick Start (Legacy)

### Building Code from a Definition

```bash
model build <template> <definition> [options]
```

Example:
```bash
model build ApiSolution CaseDomain --definitions ../src/Modeller.Definitions --templates ../src/Modeller.Templates --output ./output
```

### Listing Available Templates

```bash
model list templates --folder <path-to-templates> --target net8.0
```

### Listing Available Definitions

```bash
model list definitions --folder <path-to-definitions>
```

## Key Concepts

These concepts apply to both legacy and future implementations:

### Enterprise
The top-level container representing your entire domain. Contains services, entities, and enumerations.

### Service
A bounded context or domain service containing related entities and business logic.

### Entity
A domain object with identity, fields, keys, and relationships.

### Behaviours (Future)
Commands, queries, and workflows that define what can be done in the domain. See [Behaviours](architecture/draft/03-behaviours.md).

### Template
A code generator (C# DLL) that consumes definitions and produces output files.

## Project Structure

```
Modeller/
├── src/
│   ├── Modeller/              # Core library
│   ├── Modeller.Definitions/  # Sample definitions
│   ├── Modeller.Templates/    # Template implementations
│   └── Modeller.Tool/         # CLI tool
├── tests/                     # Unit tests
├── docs/                      # Documentation
└── Templates/                 # Compiled template output
```

## License

See the repository root for license information.

