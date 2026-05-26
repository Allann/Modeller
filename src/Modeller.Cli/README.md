# Modeller CLI

<p align="center">
  <img src="icon.svg" alt="Modeller CLI" width="128" height="128">
</p>

<p align="center">
  <a href="https://www.nuget.org/packages/Modeller.Cli/"><img src="https://img.shields.io/nuget/v/Modeller.Cli.svg?label=nuget" alt="NuGet"></a>
  <a href="https://github.com/CSharp-Catalyst/Modeller/actions/workflows/ci.yml"><img src="https://github.com/CSharp-Catalyst/Modeller/actions/workflows/ci.yml/badge.svg" alt="CI"></a>
  <a href="LICENSE"><img src="https://img.shields.io/badge/license-MIT-blue.svg" alt="License: MIT"></a>
  <a href="https://dotnet.microsoft.com"><img src="https://img.shields.io/badge/.NET-10.0-purple.svg" alt=".NET"></a>
</p>

**Modeller CLI** is a code-generation tool for .NET that transforms domain definitions into production-ready C# code. Define your domain once using an intuitive DSL, then let Modeller generate consistent, convention-following boilerplate across all layers of your application.

---

## 🚀 Quick Start

### Installation

Install as a global .NET tool:

```bash
dotnet tool install --global Modeller.Cli
```

Verify the installation:

```bash
modeller --version
```

### Basic Usage

1. **Initialize a project** with templates and configuration:

```bash
modeller init --template-source ./templates --pack csharp/plugin
```

2. **Create domain definitions** (e.g., `domain/entities/order.entity`):

```modeller
entity Order
  attributes
	id         guid        generated
	reference  text        unique
	status     OrderStatus
	created    datetime    generated
  end
end
```

3. **Generate code** from your definitions:

```bash
modeller generate
```

Generated files appear in your configured output directory as `.g.cs` files that you can freely regenerate without losing hand-written code.

---

## 📚 Commands

### `modeller init`

Initialize a new `.modeller` configuration folder in your project.

```bash
modeller init --template-source <path> --pack <pack-name> [options]
```

**Options:**
- `-t, --template-source <path>` — Source path or URL for templates (required)
- `-p, --pack <name>` — Template pack to use, e.g., `csharp/plugin` (required)
- `-d, --domain <path>` — Path to domain definitions folder (default: current directory)
- `-f, --force` — Overwrite existing configuration

**Example:**
```bash
modeller init --template-source ./templates --pack csharp/clean-architecture --domain ./domain
```

Creates:
- `.modeller/config.yaml` — Project configuration
- `.modeller/profiles/` — Generation profiles

---

### `modeller generate`

Generate code from domain definitions using configured templates and profiles.

```bash
modeller generate [options]
```

**Options:**
- `-p, --profile <name>` — Profile to use (default: from config.yaml)
- `-l, --layer <name>` — Generate only a specific layer
- `-v, --var <key=value>` — Override variables (can be used multiple times)
- `-n, --dry-run` — Preview what would be generated without writing files

**Examples:**
```bash
# Generate using default profile
modeller generate

# Generate only the API layer
modeller generate --layer api

# Generate with custom variables
modeller generate --var company=Acme --var product=InventorySystem

# Preview without writing files
modeller generate --dry-run
```

---

### `modeller validate`

Validate domain definitions and project configuration.

```bash
modeller validate
```

Checks:
- Configuration file syntax
- Template availability
- Domain definition syntax
- Profile configuration
- Output path accessibility

**Example output:**
```
✓ .modeller folder found
✓ config.yaml loaded (version 1.0)
✓ Templates found: 12 templates in 3 layers
✓ Domain definitions found: 5 entities, 2 commands
✓ Profile 'default' validated
✓ Output path accessible

Validation complete: 0 errors, 0 warnings
```

---

### `modeller templates`

Manage and view available template packs.

#### List available templates

```bash
modeller templates list [--source <path>]
```

**Example:**
```bash
modeller templates list --source ./my-templates
```

**Output:**
```
Available template packs:
  csharp/clean-architecture  - Clean Architecture with CQRS
  csharp/plugin              - Plugin-based architecture
  csharp/minimal-api         - Minimal API endpoints
```

#### View template pack details

```bash
modeller templates info <pack-name> [--source <path>]
```

**Example:**
```bash
modeller templates info csharp/plugin
```

---

### `modeller snippet`

Work with reusable template snippets.

#### List snippets

```bash
modeller snippet list [--source <path>]
```

#### Show snippet content

```bash
modeller snippet show <snippet-name> [--source <path>]
```

**Example:**
```bash
modeller snippet show entity-base
```

---

## 📁 Project Structure

After running `modeller init`, your project will have:

```
your-project/
├── .modeller/
│   ├── config.yaml           # Main configuration
│   └── profiles/
│       └── default.yaml      # Default generation profile
├── domain/                   # Domain definitions (customizable)
│   ├── domain.def
│   ├── entities/
│   │   ├── order.entity
│   │   └── customer.entity
│   ├── commands/
│   └── queries/
└── output/                   # Generated code (customizable)
	└── *.g.cs               # Generated files
```

---

## 🎨 DSL File Types

Modeller supports multiple file types for different domain concepts:

| Extension      | Description                       | Example                    |
|----------------|-----------------------------------|----------------------------|
| `.def`         | Domain definition                 | `domain OrderManagement`   |
| `.entity`      | Entity with attributes            | `entity Order`             |
| `.command`     | Command (write operation)         | `command PlaceOrder`       |
| `.query`       | Query (read operation)            | `query GetOrdersByCustomer`|
| `.enum`        | Enumeration                       | `enum OrderStatus`         |
| `.union`       | Discriminated union               | `union PaymentMethod`      |
| `.value`       | Value object                      | `value Money`              |
| `.aggregate`   | Aggregate root                    | `aggregate Order`          |
| `.service`     | Domain service                    | `service PricingService`   |
| `.event`       | Domain event                      | `event OrderPlaced`        |

---

## ⚙️ Configuration

### config.yaml

Located in `.modeller/config.yaml`, this file controls:

```yaml
version: "1.0"
templateSource:
  type: file
  path: ./templates
  pack: csharp/plugin

domain:
  path: ./domain
  namespace: MyCompany.MyProduct

output:
  path: ./output
  namespace: MyCompany.MyProduct.Generated

profiles:
  default: default

variables:
  company: MyCompany
  product: MyProduct
```

### Profiles

Profiles (in `.modeller/profiles/`) define what to generate:

```yaml
name: default
layers:
  - name: domain
	enabled: true
	templates:
	  - entity
	  - value-object
  - name: api
	enabled: true
	templates:
	  - controller
	  - dto
```

---

## 🔄 Workflow Integration

### Build-Time Generation

Add to your `.csproj` to regenerate on build:

```xml
<Target Name="ModellerGenerate" BeforeTargets="BeforeBuild">
  <Exec Command="dotnet tool restore" />
  <Exec Command="dotnet modeller generate" />
</Target>
```

### Watch Mode (Manual)

Use `dotnet watch` with a custom target:

```bash
dotnet watch --project YourProject.csproj msbuild /t:ModellerGenerate
```

### CI/CD Integration

#### GitHub Actions

```yaml
- name: Install Modeller
  run: dotnet tool install --global Modeller.Cli

- name: Generate code
  run: modeller generate

- name: Verify generated code
  run: dotnet build
```

#### Azure DevOps

```yaml
- task: DotNetCoreCLI@2
  displayName: 'Install Modeller'
  inputs:
	command: custom
	custom: tool
	arguments: 'install --global Modeller.Cli'

- script: modeller generate
  displayName: 'Generate code'
```

---

## 🎯 Key Benefits

### ✅ Consistency
Templates encode your team's conventions. Every generated file follows the same patterns.

### ✅ Productivity
Add a new entity with one `.entity` file. Modeller generates all layers automatically.

### ✅ Maintainability
`.g.cs` files are regenerated cleanly. Drift between layers is impossible.

### ✅ Safety
Only generated files (marked `.g.*`) are overwritten. Hand-written code is never touched.

### ✅ Flexibility
Templates are yours to customize. Change them without recompiling the tool.

### ✅ Zero Runtime Dependency
Modeller generates plain C# code. No framework, no magic, no runtime overhead.

---

## 🔧 Advanced Usage

### Custom Variables

Override variables at generation time:

```bash
modeller generate --var namespace=Acme.Inventory --var api-version=v2
```

Variables flow through:
- Template content (via Scriban)
- Output paths
- Generated namespaces

### Layer-Specific Generation

Generate only specific layers:

```bash
modeller generate --layer domain
modeller generate --layer api
```

### Multiple Profiles

Create specialized profiles for different scenarios:

```yaml
# .modeller/profiles/api-only.yaml
name: api-only
layers:
  - name: api
	enabled: true
```

```bash
modeller generate --profile api-only
```

---

## 🆚 Comparison

| Without Modeller | With Modeller |
|------------------|---------------|
| Write `DbSet<T>`, DTOs, controllers, endpoints by hand | Define entity once, regenerate all layers |
| Boilerplate drifts across layers | `.g.*` files always in sync |
| Conventions vary per developer | Templates enforce consistency |
| Touching 6+ files per entity | Add one `.entity` file, run generate |
| Manual updates prone to errors | Automated regeneration |

---

## 📖 Resources

- **Main Repository:** [github.com/CSharp-Catalyst/Modeller](https://github.com/CSharp-Catalyst/Modeller)
- **VS Code Extension:** [Modeller DSL](https://marketplace.visualstudio.com/items?itemName=catalyst.modeller-dsl)
- **Documentation:** [docs/](../../docs/)
- **Issues:** [GitHub Issues](https://github.com/CSharp-Catalyst/Modeller/issues)
- **Discussions:** [GitHub Discussions](https://github.com/CSharp-Catalyst/Modeller/discussions)

---

## 🤝 Contributing

Contributions are welcome! See [CONTRIBUTING.md](../../CONTRIBUTING.md) for guidelines.

---

## 📄 License

MIT License - see [LICENSE](../../LICENSE) for details.

---

## 💬 Support

- **Questions?** Open a [discussion](https://github.com/CSharp-Catalyst/Modeller/discussions)
- **Bug?** File an [issue](https://github.com/CSharp-Catalyst/Modeller/issues)
- **Need help?** Check the [documentation](../../docs/) or ask in discussions

---

<p align="center">
  Made with ❤️ by the Modeller Team
</p>
