# Implementation Status

This document tracks the current implementation status of the Modeller domain definition system.

_Last updated: 22 May 2026_

---

## Overview

| Component | Status | Location |
|-----------|--------|----------|
| **DSL Parser** | ✅ Complete | `src/Modeller.Parser/` |
| **Domain Models** | ✅ Complete | `src/Modeller.Domain/` |
| **Configuration System** | ✅ Complete | `src/Modeller.Generator/Configuration/` |
| **Code Generation Engine** | ✅ Complete | `src/Modeller.Generator/` |
| **CLI Tool** | ✅ Complete | `src/Modeller.Cli/` |
| **Template Packs (C#)** | ✅ Complete | `templates/csharp/` |
| **VS Code Extension** | ✅ Complete | `editors/vscode-modeller/` |
| **Sample Definitions** | ✅ Complete | `samples/` |
| **Integration Tests** | ✅ Complete | `tests/Modeller.Integration.Tests/` |
| **CI Pipeline** | ✅ Complete | `.github/workflows/` |
| **NuGet Publish** | ⏳ Not Started | Package configured; not yet published |
| **VS Code Marketplace Publish** | ⏳ Not Started | Extension packaged; publisher account needed |
| **Workflow DSL Support** | ⏳ Not Started | — |

---

## DSL Parser

**Technology**: [Pidgin](https://github.com/benjamin-hodgson/Pidgin) v3.5.1 parser combinators

**Supported File Types**:

| Extension | Parser | Description |
|-----------|--------|-------------|
| `.def` | `DslParser.ParseDomain()` | Domain definition |
| `.entity` | `DslParser.ParseEntity()` | Entity definitions |
| `.value` | `DslParser.ParseValue()` | Value objects |
| `.shared` | `DslParser.ParseShared()` | Lookup/reference data |
| `.enum` | `DslParser.ParseEnum()` | Enumerations |
| `.flags` | `DslParser.ParseEnum()` | Flag enumerations |
| `.command` | `DslParser.ParseCommand()` | Commands (write ops) |
| `.query` | `DslParser.ParseQuery()` | Queries (read ops) |
| `.event` | `DslParser.ParseEvent()` | Domain events |
| `.key` | `DslParser.ParseKey()` | Key definitions |
| `.service` | `DslParser.ParseService()` | Service/bounded context |
| `.projection` | `DslParser.ParseProjection()` | Read model projections |

---

## Domain Models

**Pattern**: Immutable records with factory method validation

**Key Design Decisions**:
1. Internal constructors prevent invalid object creation
2. Factory methods (`New`, `CreateValid`) validate before construction
3. `{ get; }` properties ensure true immutability
4. `Guid.IsVersion7()` extension validates public IDs

**Core Types**:

| Type | Description |
|------|-------------|
| `Domain` | Top-level container for entire domain |
| `Service` | Bounded context grouping |
| `Entity` | Domain object with identity |
| `Attribute` | Property/field on an entity |
| `Relationship` | Link between entities |
| `Enumeration` | Enum type definition |
| `Command` | Write operation |
| `Query` | Read operation |

**Builder**: `DomainBuilder` converts parsed AST nodes into semantic domain model.

---

## Configuration System

**Technology**: YamlDotNet v17.1.0

**Structure**:
```
.modeller/
├── config.yaml          # Project-wide config (domain path, output root, variables)
└── profiles/
    ├── default.yaml     # Default generation profile
    ├── infrastructure.yaml
    └── plugin.yaml
```

**Key Classes**:
- `ConfigLoader` — loads `config.yaml` and profile YAMLs
- `ManifestLoader` — loads `pack.yaml` and `template.yaml` manifests
- `VariableMerger` — resolves `{variables.x}` substitutions in output paths
- `ProjectConfig`, `ProfileConfig` — typed configuration records

---

## Code Generation Engine

**Technology**: Scriban v7.2.0

**Generation Cardinality** (set in `template.yaml`):
- `per: domain` — one output file for the entire domain
- `per: entity` — one output file per entity in the domain

**Key Classes**:
- `GenerationPlanner` — reads profiles and builds a `GenerationPlan`
- `GenerationExecutor` — executes the plan, calls `TemplateEngine` per file
- `TemplateEngine` — wraps Scriban, passes domain model as context
- `DomainTemplateFunctions` — custom functions available inside templates (`pascal_case`, etc.)
- `ScribanDomainGenerator` — high-level facade used by the CLI

---

## CLI Tool

**Technology**: System.CommandLine v2.0.8  
**Distribution**: .NET global tool (`dotnet tool install --global Modeller.Cli`)

| Command | Description |
|---------|-------------|
| `init` | Scaffold a `.modeller/` folder, copy sample definitions and LLM context |
| `generate` | Run the full generation pipeline from domain definitions to output files |
| `validate` | Parse and validate domain definitions without generating output |
| `templates` | List available template packs |
| `snippet` | Work with reusable code snippets |

---

## Template Packs

All packs live under `templates/csharp/`.

### `domain`
Generates bare C# records for entities, enums, commands, and queries.

### `clean-architecture`
Multi-template pack organised by clean architecture layers.

### `plugin`
Newest pack — generates a full plugin-architecture application:

| Sub-pack | Key outputs |
|----------|-------------|
| `api` | `Program.g.cs`, DI extensions, endpoint extensions, per-entity Minimal API endpoints, `.csproj` |
| `infrastructure` | `DbContext`, entity configurations, mapping extensions |
| `sdk` | `ApiResult`, `CreateRequest`, `UpdateRequest`, and other client types |
| `ui` | UI layer scaffolding |

---

## VS Code Extension

**Location**: `editors/vscode-modeller/`

**Features**:
- ✅ Syntax highlighting (TextMate grammar)
- ✅ Custom file icons for all 12 definition types
- ✅ Language configuration (comments, brackets, auto-close pairs)
- ✅ Auto-prompt to activate icon theme on first install

**Installation**: See `editors/vscode-modeller/INSTALL.md`

**Status**: Built and installable locally; not yet published to the VS Code Marketplace.

---

## Sample Definitions

**`samples/modeller/`** — The Modeller tool's own domain, demonstrating all definition types.

**`samples/modeller-units/`** — Real-world example (JJs Waste units management domain) with a complete `.modeller/` config including default, infrastructure, and plugin profiles.

---

## Tests

| Project | Tests | Coverage |
|---------|-------|----------|
| `Modeller.Parser.Tests` | 43 | Unit tests for all 12 DSL parsers |
| `Modeller.Generator.Tests` | 42 | Unit tests for config loading, manifest loading, variable merging, template discovery, snippet loading |
| `Modeller.Integration.Tests` | 84 | End-to-end generation (all plugin layers), domain assembly, DSL parsing for both sample domains |

---

## Known Gaps

1. **Workflow DSL** — `workflow` is described in architecture docs but has no parser or generator support.
2. **NuGet publish** — the global tool package is not yet published to NuGet.org.
3. **VS Code Marketplace** — the extension is packaged and ready; a publisher account (`catalyst`) is needed to publish.
4. **Non-C# templates** — only C# template packs exist; the engine is language-agnostic.
