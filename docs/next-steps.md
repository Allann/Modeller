# Modeller — Suggested Next Steps

_Generated: 21 May 2026_

These steps are ordered by value-to-effort ratio. The tool is functionally complete for its core workflow; the priorities below focus on hardening, distribution, and closing known gaps.

---

## 1. Commit the Pending Line-Ending Cleanup (5 min)

Five files have LF → CRLF differences that produce Git warnings on every diff. Options:

- **Preferred**: Add a `.gitattributes` rule to normalise line endings project-wide, then commit the five files. This removes noise permanently.
- **Alternative**: Commit the five files as-is to clear the dirty tree.

```
# .gitattributes
* text=auto eol=crlf
*.scriban text eol=lf
```

---

## 2. Update the Stale Implementation Status Doc (15 min)

[docs/architecture/draft/11-implementation-status.md](architecture/draft/11-implementation-status.md) still shows the CLI as "Not Started" and several generators as "In Progress". Update it to reflect the actual completed state so it is useful as reference rather than misleading.

---

## 3. Verify the End-to-End Generation Workflow (1–2 hrs)

The `csharp/plugin` template pack was the last significant feature added. Before extending further, manually run the full generation cycle against the `samples/modeller-units` domain and confirm:

- `modeller init` produces the correct `.modeller/` scaffolding
- `modeller generate` runs without errors
- Generated output in `output/` matches the expected structure for api, infrastructure, sdk, and ui layers
- Variable substitution in output paths resolves correctly (the last two bug fixes addressed this)

This also serves as a smoke test before writing more templates.

---

## 4. Expand Integration Test Coverage (2–4 hrs)

`Modeller.Integration.Tests` exists but likely has thin coverage of the newer plugin template pack. Add scenarios that:

- Run the full generate pipeline against `samples/modeller-units`
- Assert specific output files are created with the right content (spot-check a few key strings)
- Cover the `validate` command against both valid and intentionally invalid definitions

This gives confidence when refactoring templates or the generation engine.

---

## 5. Set Up a CI Pipeline (2–4 hrs)

A `.github/workflows/` directory exists but its content was not enumerated. At minimum, set up:

- **Build + test** on push to `main` and on pull requests (`dotnet build`, `dotnet test`)
- **Pack** step that produces the `modeller` global tool NuGet package as a build artefact
- Optionally: publish to NuGet.org on tagged releases

This is the prerequisite for distributing the tool to other developers.

---

## 6. Publish the NuGet Global Tool (1 hr, after CI)

The `.csproj` is already configured for global tool packaging. Once CI is in place, publish to NuGet.org so users can install via:

```
dotnet tool install --global modeller
```

Steps:
1. Confirm `PackageId`, `Version`, and `Description` in `Modeller.Cli.csproj`
2. Obtain a NuGet API key
3. Wire the publish step into the CI release workflow

---

## 7. Implement Workflow Support (4–8 hrs)

The architecture documentation describes `workflow` as a first-class DSL concept (multi-step processes), but there is no `.workflow` parser or generator. If workflows are needed for the target domains, this is the next DSL gap to close:

1. Add `WorkflowNode` to `Ast/BehaviourNodes.cs`
2. Add `WorkflowParsers.cs` following the pattern of `BehaviourParsers.cs`
3. Extend `DomainBuilder` to build workflow domain objects
4. Add Scriban templates for workflow scaffolding

---

## 8. Publish the VS Code Extension (2 hrs)

The extension under `editors/vscode-modeller/` provides syntax highlighting and file icons but is not distributed via the VS Code Marketplace. To publish:

1. Create a publisher account at [marketplace.visualstudio.com](https://marketplace.visualstudio.com)
2. Use `vsce package` and `vsce publish`
3. Consider whether to keep the extension in this repo or move it to a dedicated repository

---

## 9. Add a Second Language Template Pack (ongoing)

The architecture is language-agnostic but only C# templates exist. A TypeScript/Node.js pack (or SQL schema generator) would demonstrate the multi-language design and make the tool more broadly useful. The `_shared/` and `_snippets/` directories are already in place to hold cross-language fragments.

---

## 10. Document the Template Authoring Guide (2–3 hrs)

There is documentation covering the DSL and CLI but no guide for developers who want to write new template packs. A concise authoring guide covering:

- The structure of `pack.yaml` and `template.yaml`
- Available Scriban model variables (domain, entity, service, etc.)
- Custom template functions (`DomainTemplateFunctions`)
- How to test a template locally

...would lower the barrier for contributors and for internal teams building custom packs.

---

## Priority Summary

| # | Task | Effort | Impact |
|---|---|---|---|
| 1 | Commit line-ending cleanup | Trivial | Removes noise |
| 2 | Update stale status doc | 15 min | Keeps docs trustworthy |
| 3 | Verify end-to-end generation | 1–2 hrs | Validates last feature |
| 4 | Expand integration tests | 2–4 hrs | Confidence for future changes |
| 5 | Set up CI pipeline | 2–4 hrs | Enables safe iteration |
| 6 | Publish NuGet global tool | 1 hr | Distribution |
| 7 | Workflow DSL support | 4–8 hrs | DSL completeness |
| 8 | Publish VS Code extension | 2 hrs | Developer experience |
| 9 | Second language template pack | Ongoing | Breadth |
| 10 | Template authoring guide | 2–3 hrs | Contributor enablement |
