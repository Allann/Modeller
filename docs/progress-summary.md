# Modeller — Progress Summary

_Last updated: 22 May 2026_

---

## What Is Modeller?

Modeller is a code-generation CLI tool that turns domain definitions written in a custom DSL into production-ready source code via pluggable Scriban templates. The target output language is C#, and the design follows Domain-Driven Design conventions (entities, value objects, commands, queries, events, projections).

---

## Overall Status

| Layer | Status | Notes |
|---|---|---|
| DSL Parser | ✅ Complete | All 12 file types parsed via Pidgin combinators |
| Domain Model | ✅ Complete | Immutable records, fluent builder, factory validation |
| Configuration System | ✅ Complete | YAML `config.yaml` + profile YAMLs; YamlDotNet v17.1.0 |
| CLI Tool | ✅ Complete | `init`, `generate`, `validate`, `templates`, `snippet`; System.CommandLine v2.0.8 |
| Code Generation Engine | ✅ Complete | Scriban v7.2.0 templates, variable substitution, per-domain/per-entity generation |
| Template Packs | ✅ Complete | `csharp/domain`, `csharp/clean-architecture`, `csharp/plugin` (api, infrastructure, sdk, ui) |
| VS Code Extension | ✅ Complete | v1.4.0; syntax highlighting + file icons for all 12 DSL types |
| Test Suite | ✅ Complete | 169 tests across 3 projects (43 parser, 42 generator, 84 integration) |
| CI Pipeline | ✅ Complete | GitHub Actions: build/test/pack on push/PR; release on `v*.*.*` tags |
| Root README | ✅ Complete | GitHub landing page with badges and quick start |
| Documentation | ✅ Updated | Architecture docs reviewed and corrected |
| NuGet packaging | Wired up | `.csproj` configured as global tool `Modeller.Cli`; not yet published |
| VS Code Marketplace | Ready | Extension packaged; publisher account needed to publish |

---

## Development History

The project was built in five explicit phases, all committed:

| Phase | Scope |
|---|---|
| 1 | Template orchestration foundation |
| 2 | Configuration system (YAML config + profiles) |
| 3 | CLI commands (`init`, `generate`) |
| 4 | Code generation engine (Scriban + domain loader) |
| 5 | `validate` and `snippet` commands |

Post-phase additions:
- User docs and global tool install support
- LLM context and sample files bundled into `init`
- Version display in CLI help
- `csharp/plugin` template pack — api, infrastructure, sdk, ui layers
- Bug fixes: `init` reading `pack.yaml` correctly, variable substitution in output paths, `DomainLoader` `.def` extension fix

Session work (May 2026):
- `.gitattributes` for line-ending normalisation
- Updated `11-implementation-status.md` to reflect actual state
- Verified end-to-end generation against `samples/modeller-units`
- Updated all NuGet dependencies to latest; migrated to Central Package Management (`Directory.Packages.props`)
- Upgraded Scriban 6.5.2 → 7.2.0 (resolves critical/high CVEs)
- Migrated System.CommandLine beta4 → 2.0.8 stable (breaking API changes resolved)
- Added 48 new integration tests (84 total); coverage includes all plugin layers, domain loader, DSL parsing
- GitHub Actions CI: build/test/pack (`ci.yml`) and release (`release.yml`)
- VS Code extension v1.4.0: updated devDeps, added `@vscode/vsce`, added `vscode-extension.yml` workflow
- Added MIT `LICENSE` file
- Full docs review and correction pass

---

## Key Architectural Decisions

- **Parser**: Pidgin v3.5.1 combinator library — each DSL concept has its own parser file
- **Templates**: Scriban v7.2.0 — templates stored as `.scriban` files alongside `template.yaml` manifests
- **Config**: YamlDotNet v17.1.0 — `config.yaml` (project-wide) + per-profile YAML files under `.modeller/profiles/`
- **Output control**: `per: domain` or `per: entity` in template manifest drives generation cardinality
- **Variable substitution**: `{variables.company}`, `{variables.product}` etc. resolved at generation time
- **CLI**: System.CommandLine v2.0.8 — packaged as a .NET 10.0 global tool
- **Package management**: Central Package Management via `Directory.Packages.props`

---

## Template Pack Coverage

### `csharp/domain`
Basic entity, enum, command, query record output.

### `csharp/clean-architecture`
Multi-template pack following clean architecture layering.

### `csharp/plugin` (newest)
| Sub-pack | Generates |
|---|---|
| `api` | Program.cs, DI extensions, endpoint extensions, per-entity Minimal API endpoints, .csproj |
| `infrastructure` | DbContext, entity configurations, mapping extensions |
| `sdk` | ApiResult, CreateRequest, UpdateRequest, and other client types |
| `ui` | UI layer scaffolding |

End-to-end test (84 integration tests) verifies 39 files generated across all four layers against the `samples/modeller-units` domain with zero failures.

---

## Known Gaps

1. **Workflow DSL** — `workflow` is described in architecture docs but has no parser or generator support.
2. **NuGet publish** — the global tool package is not yet published to NuGet.org.
3. **VS Code Marketplace** — the extension is packaged and ready; a publisher account (`catalyst`) is needed.
4. **Non-C# templates** — only C# template packs exist; the engine is language-agnostic.
