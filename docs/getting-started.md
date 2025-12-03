# Getting Started

This guide walks you through using Modeller to generate code from a definition.

## Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022 or VS Code (optional)

## Building the Tool

First, build the Modeller solution:

```bash
cd Modeller
dotnet build
```

This compiles:
- The core library (`Modeller.dll`)
- The CLI tool (`Modeller.Tool`)
- All definitions and templates

## Exploring Available Templates

List the templates that are available:

```bash
cd src/Modeller.Tool/bin/Debug/net8.0
./model list templates --folder ../../../../Modeller.Templates --target net8.0
```

You'll see output like:

| Usage | Name | Description | Version |
|-------|------|-------------|---------|
| ApiSolution | API Solution | Build a complete API solution | 1.0.0 |
| ApiProject | WebApi Project | Build a WebApi project | 1.0.0 |
| DatabaseProject | Database Project | Generate database schema | 1.0.0 |

## Exploring Available Definitions

List the definitions:

```bash
./model list definitions --folder ../../../../Modeller.Definitions
```

## Your First Build

Generate code using the `ApiSolution` template with the `NewBranch` definition:

```bash
./model build ApiSolution NewBranch \
  --definitions ../../../../Modeller.Definitions \
  --templates ../../../../Modeller.Templates \
  --output ~/generated \
  --target net8.0
```

This generates a complete API solution structure in the `~/generated` folder.

## Understanding the Output

After generation, you'll have:

```
generated/
├── src/
│   ├── YourProject.Api/           # Web API project
│   ├── YourProject.Domain/        # Domain layer
│   ├── YourProject.Infrastructure/# Infrastructure layer
│   ├── YourProject.Sdk/           # Client SDK
│   └── Shared/                    # Shared code
├── Directory.Build.props
├── Directory.Build.targets
├── Directory.Packages.props
├── .editorconfig
├── .gitignore
├── global.json
└── YourProject.sln
```

## Creating a Custom Definition

1. Create a new class library project:

```bash
dotnet new classlib -n MyDomain
```

2. Add a reference to Modeller:

```xml
<ItemGroup>
  <ProjectReference Include="..\Modeller\Modeller.csproj" />
</ItemGroup>
```

3. Create your definition:

```csharp
// Definition.cs
public class MyDomainDefinition : DefinitionBase
{
    protected override EnterpriseBuilder Define()
    {
        return new EnterpriseBuilder()
            .For("MyCompany")
            .Project(new("MyProject"))
            .AddDocumentation("My domain model")
            .WithVersion(new("1.0"))
            .AddEntity(new Name("Customer"))
                .AddDocumentation("A customer entity")
                .IsRoot()
                .AddKey(new Name("Customer"), 1)
                    .AddField(new("CustomerId"))
                        .WithDataType(DataType.Int32)
                        .Generated(ValueGeneratedTypes.Add)
                    .And
                .And
                .AddField(new("Name"))
                    .WithDataType(DataType.String.WithMaxLength(100))
                    .AsBusinessKey()
                .And
            .And
            .AddService(new("CustomerManagement", true))
                .AddDocumentation("Customer management service")
                .AddEntities([new Name("Customer")])
            .And;
    }
}

// DefinitionData.cs
public class DefinitionData : IDefinitionMetaData
{
    public FileVersion Version => new("1.0");
    public string Name => "MyDomain";
    public string Description => "My custom domain.";
    public Type EntryPoint => typeof(MyDomainDefinition);
}
```

4. Build and use:

```bash
dotnet build
./model build ApiSolution MyDomain --definitions ./bin/Debug/net8.0 ...
```

## Next Steps

- Read [Definitions](definitions.md) to learn the full fluent API
- Read [Templates](templates.md) to understand how templates work
- Read [Data Types](data-types.md) for field type reference
- Read [CLI Reference](cli-reference.md) for all command options

