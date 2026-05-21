# Modeller — Progress Summary

_Generated: 21 May 2026_

---

## What Is Modeller?

Modeller is a code-generation CLI tool that turns domain definitions written in a custom DSL into production-ready source code via pluggable Scriban templates. The target output language is C#, and the design follows Domain-Driven Design conventions (entities, value objects, commands, queries, events, projections).

---

## Overall Status

| Layer | Status | Notes |
|---|---|---|
| DSL Parser | Complete | All 12 file types parsed via Pidgin combinators |
| Domain Model | Complete | Immutable records, fluent builder, factory validation |
| Configuration System | Complete | YAML `config.yaml` + profile YAMLs |
| CLI Tool | Complete | `init`, `generate`, `validate`, `templates`, `snippet` |
| Code Generation Engine | Complete | Scriban templates, variable substitution, per-domain/per-entity generation |
| Template Packs | Mostly complete | `csharp/domain`, `csharp/clean-architecture`, `csharp/plugin` (api, infrastructure, sdk, ui) |
| VS Code Extension | Complete | Syntax highlighting + file icons for all DSL types |
| Test Suite | Partial | Parser and generator unit tests present; integration tests exist but coverage gap likely |
| Documentation | Mixed | User-facing docs updated; `11-implementation-status.md` is significantly stale |
| NuGet packaging | Wired up | `.csproj` configured as global tool `modeller`; no evidence of published packages |

---

## Development Phases Completed

The project was built in five explicit phases, all now committed:

| Phase | Commit | Scope |
|---|---|---|
| 1 | `f1bb055` | Template orchestration foundation |
| 2 | `99153f1` | Configuration system (YAML config + profiles) |
| 3 | `2268848` | CLI commands (`init`, `generate`) |
| 4 | `c948ab9` | Code generation engine (Scriban + domain loader) |
| 5 | `790536b` | `validate` and `snippet` commands |

Post-phase additions:
- User docs and global tool install support (`cb02456`)
- LLM context and sample files bundled into `init` (`ebe58ea`)
- Version display in CLI help (`1748e4a`)
- `csharp/plugin` template pack — api, infrastructure, sdk, ui layers (`f599a08`)
- Bug fixes: `init` reading `pack.yaml` correctly, variable substitution in output paths

---

## Key Architectural Decisions

- **Parser**: Pidgin v3.5.1 combinator library — each DSL concept has its own parser file
- **Templates**: Scriban v6.5.2 — templates stored as `.scriban` files alongside `template.yaml` manifests
- **Config**: YAML with `config.yaml` (project-wide) + per-profile YAML files under `.modeller/profiles/`
- **Output control**: `per: domain` or `per: entity` in template manifest drives generation cardinality
- **Variable substitution**: `{variables.company}`, `{variables.product}` etc. resolved at generation time
- **CLI**: `System.CommandLine` v2.0.0-beta — packaged as a .NET global tool

---

## Template Pack Coverage

### `csharp/domain`
Basic entity, enum, command, query record output.

### `csharp/clean-architecture`
Multi-template pack following clean architecture layering.

### `csharp/plugin` (newest — added last commit before work paused)
| Sub-pack | Generates |
|---|---|
| `api` | Program.cs, DI extensions, endpoint extensions, per-entity Minimal API endpoints, .csproj |
| `infrastructure` | DbContext, entity configurations, mapping extensions |
| `sdk` | ApiResult, CreateRequest, UpdateRequest, and other client types |
| `ui` | UI layer scaffolding |

---

## Uncommitted Changes

All five modified files in the working tree are **line-ending normalisation only** (LF → CRLF on Windows). No logic changes are pending:

- `src/Modeller.Generator/Configuration/ConfigLoader.cs`
- `templates/csharp/plugin/api/template.yaml`
- `templates/csharp/plugin/infrastructure/template.yaml`
- `templates/csharp/plugin/sdk/template.yaml`
- `templates/csharp/plugin/ui/template.yaml`

These can be committed or added to `.gitattributes` to suppress the warnings.

---

## Known Gaps / Issues

1. **`docs/architecture/draft/11-implementation-status.md` is stale** — still shows CLI as "Not Started" and `ScribanDomainGenerator` as "In Progress", which are both done.
2. **No published NuGet package** — the tool is packaged as a global .NET tool but there is no evidence of a CI publish pipeline or release on NuGet.
3. **Integration test coverage** — `Modeller.Integration.Tests` exists but the degree of end-to-end scenario coverage is unclear.
4. **`workflow` DSL concept** — referenced in architecture docs but parser/generator support is not evident.
5. **Non-C# template packs** — the architecture is language-agnostic but only C# templates exist.
6. **VS Code extension not published** — the extension is built but not distributed via the VS Code Marketplace.

---

## Repository Statistics

| Item | Count |
|---|---|
| C# source files | ~50 |
| Test files | ~20 |
| Scriban templates | ~40 |
| Documentation files | ~24 |
| Sample DSL definition files | ~30 |
| Target framework | .NET 10.0 |
