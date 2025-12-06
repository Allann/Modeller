# Deep Dive: Template Orchestration Architecture

This document provides in-depth technical details about how Modeller's template orchestration system works.

## System Overview

Modeller transforms domain definitions (DSL files) into C# code using Scriban templates. The architecture follows a layered approach:

```
┌─────────────────────────────────────────────────────────────────────┐
│                         CLI Commands                                 │
│  modeller init | modeller generate | modeller validate              │
└─────────────────────────────────────────────────────────────────────┘
                                   │
                                   ▼
┌─────────────────────────────────────────────────────────────────────┐
│                      Orchestration Layer                             │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐                  │
│  │ConfigLoader │  │   Profile   │  │  Template   │                  │
│  │             │  │   Resolver  │  │  Discovery  │                  │
│  └─────────────┘  └─────────────┘  └─────────────┘                  │
└─────────────────────────────────────────────────────────────────────┘
                                   │
                                   ▼
┌─────────────────────────────────────────────────────────────────────┐
│                       Generation Engine                              │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐                  │
│  │DomainLoader │  │  Template   │  │ Generation  │                  │
│  │             │  │   Engine    │  │  Executor   │                  │
│  └─────────────┘  └─────────────┘  └─────────────┘                  │
└─────────────────────────────────────────────────────────────────────┘
                                   │
                                   ▼
┌─────────────────────────────────────────────────────────────────────┐
│                         Output Files                                 │
│  *.g.cs (regeneratable)  |  *.cs (user-owned, never overwritten)    │
└─────────────────────────────────────────────────────────────────────┘
```

## Core Components

### 1. Configuration System

**Location:** `src/Modeller.Generator/Configuration/`

#### ConfigLoader

Loads project configuration from the `.modeller/` folder:

```csharp
public class ConfigLoader
{
    public ProjectConfiguration LoadConfiguration(string projectRoot);
    public ProfileConfig LoadProfile(string modellerFolder, string profileName);
    public IEnumerable<string> ListProfiles(string modellerFolder);
    public bool HasConfiguration(string projectRoot);
}
```

#### ProjectConfig

Model for `.modeller/config.yaml`:
- `Version` - Schema version (currently 1)
- `Domain` - Path to domain definitions
- `TemplateSource` - Original template source for updates
- `Variables` - Global template variables
- `Output` - Output configuration (root, project_pattern)
- `DefaultProfile` - Profile to use when none specified
- `Files` - File handling (suffix, line ending, encoding)

#### ProfileConfig

Model for `.modeller/profiles/*.yaml`:
- `Name` - Profile display name
- `Pack` - Template pack to use
- `Layers` - List of layers to generate
- `Include`/`Exclude` - Filter which entities/enums to process
- `LayerVariables` - Per-layer variable overrides

#### VariableMerger

Merges variables with proper precedence:
```
config.yaml → profile → layer_variables → CLI overrides
```

### 2. Template System

**Location:** `src/Modeller.Generator/Templates/`

#### TemplateDiscovery

Scans the templates folder and discovers available packs:

```csharp
public class TemplateDiscovery
{
    public IEnumerable<PackInfo> DiscoverPacks();
    public PackManifest LoadPackManifest(string packPath);
    public TemplateManifest LoadTemplateManifest(string templatePath);
}
```

#### SnippetLoader

Loads reusable template fragments from `_snippets/`. Implements Scriban's `ITemplateLoader` interface for `include` support:

```csharp
public class SnippetLoader : ITemplateLoader
{
    public string GetPath(TemplateContext context, SourceSpan span, string templateName);
    public string Load(TemplateContext context, SourceSpan span, string templatePath);
    public IReadOnlyList<SnippetInfo> DiscoverSnippets();
}
```

#### TemplateEngine

Renders Scriban templates with full context:

```csharp
public class TemplateEngine
{
    public string Render(string templatePath, ScriptObject context);
    public string RenderString(string templateContent, ScriptObject context);
}
```

Registers custom functions:
- `pascal_case` - Convert to PascalCase
- `camel_case` - Convert to camelCase
- `snake_case` - Convert to snake_case
- `to_csharp_type` - Convert DSL type to C# type
- `pluralize` - Pluralize a word

### 3. Generation Engine

**Location:** `src/Modeller.Generator/Generation/`

#### DomainLoader

Parses domain definition files (`.entity`, `.enum`, `.command`, `.query`, `.projection`):

```csharp
public class DomainLoader
{
    public Domain LoadDomain(string domainPath);
    public DomainStats GetStats(Domain domain);
}
```

Domain model structure:
- `Domain` - Root containing all definitions
- `Entity` - Entity with attributes and key
- `Enumeration` - Enum with values
- `Command` - Command with parameters
- `Query` - Query with parameters
- `Projection` - Read model/DTO definition

#### GenerationPlanner

Creates a plan of files to generate based on configuration:

```csharp
public class GenerationPlanner
{
    public GenerationPlan CreatePlan(
        Domain domain,
        LayerConfig layer,
        ProfileConfig profile,
        ProjectConfig project,
        string outputRoot,
        Dictionary<string, object> cliOverrides);
}
```

The plan contains `PlannedFile` entries:
- `TemplatePath` - Source template file
- `OutputPath` - Where to write the file
- `Context` - Variables and data for rendering
- `CanOverwrite` - True if file has `.g.` suffix

#### GenerationExecutor

Executes the plan - renders templates and writes files:

```csharp
public class GenerationExecutor
{
    public Task<IReadOnlyList<GenerationResult>> ExecuteAsync(
        GenerationPlan plan,
        bool dryRun,
        Action<GenerationResult>? onProgress);
}
```

Generation rules:
- Files with `.g.` in name are **always overwritten**
- Files without `.g.` are **skipped if they exist**
- Directories are created as needed

## Folder Structure

### Template Server

Templates are stored in a configurable location:

```
{template-source}/
  _snippets/                          # Shared reusable snippets
    csharp/
      header.scriban                  # File header with namespace
      property.scriban                # Single property generation
      factory-methods.scriban         # New() and CreateValid()

  csharp/
    clean-architecture/               # Template pack
      pack.yaml                       # Pack metadata
      domain/
        template.yaml                 # What this template generates
        entity.scriban                # Entity template
        enum.scriban                  # Enum template
        command.scriban               # Command template
      infrastructure/
        template.yaml
        configuration.scriban
        db-context.scriban
      sdk/
        template.yaml
        request.scriban
        response.scriban
```

### Project `.modeller/` Folder

Created by `modeller init`:

```
{project-root}/
  .modeller/
    config.yaml                       # Main configuration
    profiles/
      default.yaml                    # Default profile
      infrastructure.yaml             # Infrastructure-only profile
    templates/                        # Copied from source
      _snippets/
        csharp/
          ...
      csharp/
        clean-architecture/
          ...
```

## Template Manifests

### Pack Manifest (`pack.yaml`)

Describes a template pack:

```yaml
name: clean-architecture
version: 1.0.0
description: Templates for Clean Architecture pattern
author: Modeller Team
language: csharp

templates:
  - domain
  - infrastructure
  - sdk
  - api

variables:
  use_nullable: true
  use_file_scoped_namespaces: true
```

### Template Manifest (`template.yaml`)

Describes what a single template generates:

```yaml
name: Domain Layer
description: Domain entities, enums, commands, and queries

generates:
  # Per-entity files
  - template: entity.scriban
    per: entity
    output: "Entities/{entity.name | pascal_case}.g.cs"

  # Per-enum files
  - template: enum.scriban
    per: enum
    output: "Enums/{enum.name | pascal_case}.g.cs"

  # Per-command files
  - template: command.scriban
    per: command
    output: "Commands/{command.name | pascal_case}.g.cs"

  # Per-domain files (runs once)
  - template: extensions.scriban
    per: domain
    output: "GuidExtensions.g.cs"

snippets:
  - csharp/header
  - csharp/property
  - csharp/factory-methods
```

## Snippet System

Snippets are reusable Scriban template fragments:

**`_snippets/csharp/header.scriban`**:
```scriban
{{~ if copyright ~}}
// {{ copyright }}
// Auto-generated - do not modify directly
{{~ end ~}}

{{~ for using in usings ~}}
using {{ using }};
{{~ end ~}}

namespace {{ namespace }};
```

**Using snippets in templates:**
```scriban
{{ include '_snippets/csharp/header'
     namespace: namespace
     copyright: variables.copyright
     usings: ['System', 'System.ComponentModel.DataAnnotations'] }}

public sealed record {{ entity.name | pascal_case }}
{
{{~ for attr in entity.attributes ~}}
{{ include '_snippets/csharp/property' attr: attr }}
{{~ end ~}}
}
```

## Generation Flow

1. **Load Configuration**
   - Read `.modeller/config.yaml`
   - Load profile (default or specified)
   - Merge variables (config → profile → layer → CLI)

2. **Parse Domain**
   - Load all `.entity`, `.enum`, `.command`, `.query`, `.projection` files
   - Build domain model
   - Validate references

3. **Resolve Templates**
   - Load `pack.yaml` for selected pack
   - Load `template.yaml` for each layer
   - Resolve snippet dependencies

4. **Plan Generation**
   - For each layer in profile:
     - For each `generates` entry in `template.yaml`:
       - Determine iterations (per: entity/enum/domain)
       - Calculate output paths
       - Check if file is `.g.*` (can overwrite)

5. **Execute Generation**
   - If `--dry-run`: display plan and exit
   - For each planned file:
     - Skip if not `.g.*` and file exists
     - Render template with Scriban
     - Write to output path

6. **Report Results**
   - Files created
   - Files overwritten
   - Files skipped (user-owned)
   - Errors encountered

## The `.g.` Convention

Files follow a naming convention that determines overwrite behavior:

| Pattern | Example | Behavior |
|---------|---------|----------|
| `*.g.cs` | `Unit.g.cs` | Always overwritten |
| `*.cs` | `Unit.cs` | Never overwritten |

This enables the **partial class pattern**:

```csharp
// Unit.g.cs - Generated, always overwritten
public sealed partial record Unit
{
    public Guid UnitId { get; init; }
    public string TruckNumber { get; init; } = string.Empty;
    // ... generated properties
}

// Unit.cs - User file, never touched
public sealed partial record Unit
{
    // User's custom methods
    public string DisplayName => $"{TruckNumber} - {Description}";
}
```

## See Also

- [Quick Start Guide](quick-start.md)
- [CLI Reference](modeller-cli-reference.md)
- [Definitions DSL](definitions.md)
- [Template Orchestration Spec](architecture/draft/12-template-orchestration.md)

