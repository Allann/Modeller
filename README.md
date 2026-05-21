# Modeller

[![CI](https://github.com/CSharp-Catalyst/Modeller/actions/workflows/ci.yml/badge.svg)](https://github.com/CSharp-Catalyst/Modeller/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Modeller.Cli.svg?label=nuget)](https://www.nuget.org/packages/Modeller.Cli/)
[![VS Code Marketplace](https://img.shields.io/visual-studio-marketplace/v/catalyst.modeller-dsl.svg?label=vs%20code)](https://marketplace.visualstudio.com/items?itemName=catalyst.modeller-dsl)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-purple.svg)](https://dotnet.microsoft.com)

**Modeller** is a code-generation CLI for .NET that turns lightweight domain definitions into production-ready C# source code. Write a brief description of your domain once — Modeller handles the boilerplate.

```
modeller init --template-source ./templates --pack csharp/plugin
modeller generate
```

---

## Why Modeller?

| Without Modeller | With Modeller |
|---|---|
| Write `DbSet<T>`, `IEntityTypeConfiguration<T>`, `CreateRequest`, `UpdateRequest`, endpoint wiring, ... by hand for every entity | Define your entity once; regenerate all layers on change |
| Boilerplate drifts out of sync across layers | `.g.*` files are always regenerated — drift is impossible |
| Conventions vary between developers | Templates encode your conventions; the whole team follows them |
| Adding a new entity means touching six files | Add one `.entity` file, run `modeller generate` |

Modeller is **not** a framework or runtime dependency. It generates plain C# files you own and can modify freely. The `.g.*` naming convention separates generated files from hand-written ones so they can coexist without conflict.

---

## Key Features

- **Custom DSL** — human-readable definition files (`.entity`, `.command`, `.query`, `.enum`, ...) that describe *what the business cares about*, not how it is stored
- **Pluggable Scriban templates** — templates live outside the tool; edit them without recompiling
- **Profile-based generation** — generate one layer, all layers, or a custom subset via YAML profiles
- **Per-domain and per-entity generation** — single file for the whole domain or one file per entity, controlled per template
- **Variable substitution** — `{variables.company}`, `{variables.product}` flow through output paths and template context
- **Safe by default** — only `.g.*` files are overwritten; hand-written code is never touched
- **VS Code extension** — syntax highlighting and custom file icons for all definition types

---

## Quick Start

### 1. Install the CLI

```bash
dotnet tool install --global Modeller.Cli
```

> **Note:** The package is not yet published to NuGet. To run from source:
> ```bash
> git clone https://github.com/CSharp-Catalyst/Modeller.git
> cd Modeller
> dotnet run --project src/Modeller.Cli -- --help
> ```

### 2. Initialise a project

```bash
# From your project root (where your .sln lives)
modeller init --template-source /path/to/templates --pack csharp/plugin
```

This creates a `.modeller/` folder with `config.yaml` and starter profiles.

### 3. Define your domain

Create a `domain/` folder and describe your domain in plain text:

```
# domain/domain.def
domain OrderManagement

# domain/entities/order.entity
entity Order
  attributes
    id         guid        generated
    reference  text        unique
    status     OrderStatus
    created    datetime    generated
  end
end
```

### 4. Generate

```bash
modeller generate
```

Generated files appear in the output folder configured in `.modeller/config.yaml`.

---

## DSL File Types

| Extension | What it describes |
|-----------|-------------------|
| `.def` | Domain name and top-level metadata |
| `.entity` | Domain entity with attributes and relationships |
| `.value` | Immutable value object |
| `.shared` | Shared/reference lookup data |
| `.enum` | Enumeration |
| `.flags` | Bit-flag enumeration |
| `.command` | Write operation (input, output, errors) |
| `.query` | Read operation (input, output) |
| `.event` | Domain event |
| `.key` | Entity key / persistence identity (kept separate from domain concern) |
| `.service` | Bounded context / service boundary |
| `.projection` | Read model projection |

---

## Template Packs

Templates ship separately from the tool. The `templates/` folder in this repo contains:

| Pack | What it generates |
|------|-------------------|
| `csharp/plugin` | Full plugin-architecture application — API endpoints, DbContext, SDK types, UI scaffolding |
| `csharp/clean-architecture` | Clean architecture layers — Application, Domain, Infrastructure, Presentation |
| `csharp/domain` | Bare C# records for entities, enums, commands, and queries |

### csharp/plugin layers

| Layer | Key outputs |
|-------|-------------|
| `API` | `Program.g.cs`, DI extensions, Minimal API endpoints per entity, `.csproj` |
| `Infrastructure` | `DbContext`, `IEntityTypeConfiguration<T>` per entity, mapping extensions |
| `SDK` | `ApiResult`, `CreateRequest`, `UpdateRequest`, validators, client types |
| `UI` | UI layer scaffolding |

---

## VS Code Extension

The **Modeller DSL** extension provides syntax highlighting and custom file icons for all 12 definition types.

**Install from source:**
```bash
cd editors/vscode-modeller
npm install && npm run package
# Then install the generated .vsix in VS Code
```

> The extension will be available on the VS Code Marketplace once a publisher account is configured. See [`editors/vscode-modeller/INSTALL.md`](editors/vscode-modeller/INSTALL.md).

---

## Project Structure

```
Modeller/
├── src/
│   ├── Modeller.Domain/        # Immutable domain model (Entity, Command, Query, ...)
│   ├── Modeller.Parser/        # Pidgin-based DSL parsers for all 12 file types
│   ├── Modeller.Generator/     # Scriban template engine, planner, executor, config
│   └── Modeller.Cli/           # System.CommandLine CLI (init, generate, validate, ...)
├── tests/
│   ├── Modeller.Parser.Tests/
│   ├── Modeller.Generator.Tests/
│   └── Modeller.Integration.Tests/
├── templates/
│   └── csharp/
│       ├── domain/
│       ├── clean-architecture/
│       └── plugin/             # api | infrastructure | sdk | ui
├── samples/
│   ├── modeller/               # Modeller's own domain (self-describing)
│   └── modeller-units/         # Real-world example — JJs Waste units management
├── editors/
│   └── vscode-modeller/        # VS Code extension
└── docs/
    └── architecture/           # Design decisions, DSL spec, implementation status
```

---

## Development

```bash
# Build
dotnet build

# Test
dotnet test

# Run CLI from source
dotnet run --project src/Modeller.Cli -- generate

# Package the NuGet global tool
dotnet pack src/Modeller.Cli/Modeller.Cli.csproj --configuration Release
```

### Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Node.js 22+ (VS Code extension only)

---

## Architecture

Detailed design documentation lives in [`docs/architecture/`](docs/architecture/draft/README.md):

| Doc | Topic |
|-----|-------|
| [01 — Overview](docs/architecture/draft/01-overview.md) | Design philosophy |
| [02 — Domain Concepts](docs/architecture/draft/02-domain-concepts.md) | Entities, value objects, shared data |
| [03 — Behaviours](docs/architecture/draft/03-behaviours.md) | Commands, queries, events, workflows |
| [04 — File Structure](docs/architecture/draft/04-file-structure.md) | Folder layout and file conventions |
| [05 — Examples](docs/architecture/draft/05-examples.md) | Concrete DSL syntax examples |
| [07 — Data Types](docs/architecture/draft/07-data-types.md) | Type system and language mappings |
| [10 — Templates](docs/architecture/draft/10-templates.md) | Template design and Scriban usage |
| [12 — Orchestration](docs/architecture/draft/12-template-orchestration.md) | Generation pipeline and configuration |

---

## License

[MIT](LICENSE)
