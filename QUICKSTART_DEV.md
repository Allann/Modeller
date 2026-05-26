# Quick Start: Using Modeller in a New Project

This guide walks you through setting up and running Modeller in a new project using the globally installed CLI.

---

## Prerequisites

- Modeller CLI installed globally (`modeller --version` should work)
- Your template pack in a local folder (e.g. `.\template`)
- Your domain definitions in a local folder (e.g. `.\definition`)

If `modeller` is not found, see [Installation](#installation) below.

---

## Step 1: Create Your Project Folder

```powershell
mkdir C:\Projects\Acme
cd C:\Projects\Acme
```

Lay out your folders before initialising:

```
Acme/
  definition/    ← your domain definition files go here
  template/      ← your template pack goes here
```

---

## Step 2: Initialise the Project

Run `init` from the project root, pointing at your template folder and specifying the pack:

- `--template-source` = where template packs live
- `--pack` = which pack inside that source to use (for example `csharp/plugin`)

The pack name is not a folder in your project root. It is resolved under `--template-source`.

```powershell
modeller init --template-source ./template --pack csharp/plugin --domain ./definition
```

If you are unsure which packs are available in your template folder, list them first:

```powershell
modeller templates list --source ./template
```

This creates a `.modeller/` folder containing:
- `config.yaml` — main configuration
- `profiles/default.yaml` — default generation profile

> If you need to redo this later, add `--force` to overwrite existing configuration.

---

## Step 3: Validate the Configuration

Check that everything is wired up correctly before generating:

```powershell
modeller validate
```

A successful run shows all green ticks. Fix any red errors before proceeding — common causes are a missing `definition` folder or an incorrect pack name.

---

## Step 4: Preview Generation (Dry Run)

See what files would be created without writing anything:

```powershell
modeller generate --dry-run
```

Review the listed files and paths. If the output root or namespaces look wrong, edit `.modeller/config.yaml` before continuing.

---

## Step 5: Generate Code

```powershell
modeller generate
```

Generated files have `.g.` in their names (e.g. `Customer.g.cs`) and are safe to overwrite on every run. Files you create or edit without `.g.` in the name are never touched.

---

## Typical Workflow After Setup

```powershell
# Edit a definition file in .\definition\...

# Re-generate
modeller generate

# Or preview first
modeller generate --dry-run
```

---

## Common Commands

| Command | Description |
|---------|-------------|
| `modeller init` | Initialise a new project configuration |
| `modeller validate` | Validate configuration and definitions |
| `modeller generate` | Generate code |
| `modeller generate --dry-run` | Preview generation without writing files |
| `modeller --help` | List all commands |
| `modeller <command> --help` | Help for a specific command |

---

## Troubleshooting

### "Command not found"
```powershell
# Reload PATH in the current session
$env:PATH = [System.Environment]::GetEnvironmentVariable("PATH","Machine") + ";" + [System.Environment]::GetEnvironmentVariable("PATH","User")
```

### Wrong output paths or namespaces
Edit `.modeller/config.yaml` — look for the `variables` block:
```yaml
variables:
  company: Acme
  product: MyProduct
  root_namespace: Acme.MyProduct
```

### Init created no `.modeller` folder
Make sure the `--template-source` path exists and contains the expected pack subfolder (e.g. `template\csharp\plugin`).

### "Pack not found"
Your pack value must match one returned by `modeller templates list --source ./template`.
If list shows `csharp/plugin`, use `--pack csharp/plugin` exactly.

### What is `pack.yaml` and where is "backend"?
`pack.yaml` is the manifest for a template pack. It defines metadata and which template layers belong to that pack.

For example, `csharp/plugin/pack.yaml` includes layers such as:
- `infrastructure`
- `sdk`
- `api`
- `ui`

There is no required layer name called `backend`. In this pack, backend concerns are split across `api`, `infrastructure`, and `sdk`.

Also note: `modeller init` copies the selected pack into `.modeller/templates`. Generation uses that local copy first. If you change files in your source `./template` after init, re-run `modeller init --force ...` (or sync `.modeller/templates`) to pick up those changes.

### Validate reports a missing template source
Use a relative path (`./template`) or an absolute path (`file://C:/Projects/Acme/template`). Spaces in paths require the `file://` form.

---

## Installation

If `modeller` is not yet installed, install it as a global .NET tool:

```powershell
dotnet tool install --global Modeller.Cli
```

To update an existing installation:
```powershell
dotnet tool update --global Modeller.Cli
```

To check what version is installed:
```powershell
dotnet tool list -g | Select-String "modeller"
```

---

## See Also

- [docs/modeller-cli-reference.md](docs/modeller-cli-reference.md) — full CLI reference
- [docs/quick-start.md](docs/quick-start.md) — extended getting started guide
- [docs/data-types.md](docs/data-types.md) — definition file data types

---

## Working on the Modeller CLI Itself

> This section is only relevant if you are developing the Modeller CLI from source.

### Quick Run (no installation)

```powershell
.\dev-run.ps1 [command] [options]
```

Examples:
```powershell
.\dev-run.ps1 --version
.\dev-run.ps1 generate --dry-run
.\dev-run.ps1 validate
```

### Full Install (test the real experience)

```powershell
.\dev-install.ps1
```

After code changes, re-run `.\dev-install.ps1` to rebuild, repack, and reinstall.

If the version does not update:
```powershell
dotnet nuget locals all --clear
dotnet tool uninstall -g Modeller.Cli
.\dev-install.ps1
```

See **DEV_SCRIPTS.md** for full documentation of these scripts.
