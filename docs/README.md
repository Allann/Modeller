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

## Documentation Index

| Document | Description |
|----------|-------------|
| [Architecture](architecture.md) | System architecture and component overview |
| [Definitions](definitions.md) | How to create and structure domain definitions |
| [Templates](templates.md) | How templates work and how to create them |
| [CLI Reference](cli-reference.md) | Command-line tool usage |
| [API Reference](api-reference.md) | Core interfaces and classes |

## Quick Start

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

### Enterprise
The top-level container representing your entire domain. Contains services, entities, and enumerations.

### Service
A bounded context or domain service containing related entities and business logic.

### Entity
A domain object with identity, fields, keys, and relationships.

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

