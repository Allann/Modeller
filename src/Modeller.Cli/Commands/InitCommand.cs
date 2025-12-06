using System.CommandLine;
using Modeller.Generator.Configuration;

namespace Modeller.Cli.Commands;

/// <summary>
/// modeller init - Initialize a new .modeller configuration
/// </summary>
public static class InitCommand
{
    public static Command Create()
    {
        var templateSourceOption = new Option<string>(
            aliases: ["--template-source", "-t"],
            description: "Source path/URL for templates (e.g., file://C:/templates)")
        {
            IsRequired = true
        };

        var domainOption = new Option<string>(
            aliases: ["--domain", "-d"],
            description: "Path to domain definitions folder",
            getDefaultValue: () => ".");

        var packOption = new Option<string>(
            aliases: ["--pack", "-p"],
            description: "Template pack to use (e.g., csharp/clean-architecture)")
        {
            IsRequired = true
        };

        var forceOption = new Option<bool>(
            aliases: ["--force", "-f"],
            description: "Overwrite existing configuration");

        var command = new Command("init", "Initialize a new .modeller configuration")
        {
            templateSourceOption,
            domainOption,
            packOption,
            forceOption
        };

        command.SetHandler(ExecuteAsync, templateSourceOption, domainOption, packOption, forceOption);

        return command;
    }

    private static async Task ExecuteAsync(string templateSource, string domain, string pack, bool force)
    {
        var projectRoot = Directory.GetCurrentDirectory();
        var modellerFolder = Path.Combine(projectRoot, ".modeller");

        // Check if already initialized
        if (Directory.Exists(modellerFolder) && !force)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Configuration already exists. Use --force to overwrite.");
            Console.ResetColor();
            return;
        }

        Console.WriteLine($"Initializing Modeller in {projectRoot}");
        Console.WriteLine($"  Template source: {templateSource}");
        Console.WriteLine($"  Pack: {pack}");
        Console.WriteLine($"  Domain: {domain}");

        // Create .modeller folder structure
        Directory.CreateDirectory(modellerFolder);
        Directory.CreateDirectory(Path.Combine(modellerFolder, "profiles"));
        Directory.CreateDirectory(Path.Combine(modellerFolder, "templates"));

        // Copy templates from source
        var sourcePath = templateSource.Replace("file://", "");
        var packSourcePath = Path.Combine(sourcePath, pack);

        if (!Directory.Exists(packSourcePath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Pack not found: {packSourcePath}");
            Console.ResetColor();
            return;
        }

        // Copy pack templates
        var packDestPath = Path.Combine(modellerFolder, "templates", pack);
        CopyDirectory(packSourcePath, packDestPath);
        Console.WriteLine($"  Copied pack: {pack}");

        // Copy snippets if they exist
        var snippetsSourcePath = Path.Combine(sourcePath, "_snippets");
        if (Directory.Exists(snippetsSourcePath))
        {
            var snippetsDestPath = Path.Combine(modellerFolder, "templates", "_snippets");
            CopyDirectory(snippetsSourcePath, snippetsDestPath);
            Console.WriteLine("  Copied snippets");
        }

        // Create default config.yaml
        var configContent = $$"""
            # Modeller Configuration
            version: 1

            domain: {{domain}}
            template_source: {{templateSource}}

            variables:
              company: MyCompany
              product: MyProduct
              copyright: "© {{DateTime.Now.Year}} MyCompany"
              root_namespace: MyCompany.MyProduct

            output:
              root: ./src
              project_pattern: "{variables.company}.{variables.product}.{layer}"

            default_profile: default

            files:
              generated_suffix: ".g"
              line_ending: auto
              encoding: utf-8
            """;

        await File.WriteAllTextAsync(Path.Combine(modellerFolder, "config.yaml"), configContent);
        Console.WriteLine("  Created config.yaml");

        // Create default profile
        var profileContent = $$"""
            name: Default
            description: Default generation profile
            pack: {{pack}}

            layers:
              - name: Domain
                template: domain
                output: "{variables.company}.{variables.product}.Domain"

            include:
              entities: all
              enums: all
              commands: all
              queries: all
              projections: all
            """;

        await File.WriteAllTextAsync(
            Path.Combine(modellerFolder, "profiles", "default.yaml"),
            profileContent);
        Console.WriteLine("  Created profiles/default.yaml");

        // Create README.md with project documentation
        var readmeContent = CreateReadmeContent();
        await File.WriteAllTextAsync(Path.Combine(modellerFolder, "README.md"), readmeContent);
        Console.WriteLine("  Created README.md");

        // Create LLM instructions file
        var llmInstructionsContent = CreateLlmInstructionsContent();
        await File.WriteAllTextAsync(Path.Combine(modellerFolder, "llm-instructions.md"), llmInstructionsContent);
        Console.WriteLine("  Created llm-instructions.md");

        // Create sample domain files if domain folder doesn't exist
        var domainPath = Path.IsPathRooted(domain) ? domain : Path.Combine(projectRoot, domain);
        if (!Directory.Exists(domainPath) || !Directory.EnumerateFiles(domainPath, "*.*", SearchOption.AllDirectories).Any())
        {
            await CreateSampleDomainFiles(domainPath);
            Console.WriteLine("  Created sample domain files");
        }

        // Create Augment rules for AI assistant context
        await CreateAugmentRules(projectRoot, llmInstructionsContent);
        Console.WriteLine("  Created .augment/rules for AI assistants");

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\nInitialization complete!");
        Console.ResetColor();

        Console.WriteLine("\nNext steps:");
        Console.WriteLine("  1. Edit .modeller/config.yaml to configure your project");
        Console.WriteLine("  2. See .modeller/README.md for DSL syntax reference");

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("VS Code Extension:");
        Console.ResetColor();
        Console.WriteLine("  For syntax highlighting and file icons, install the Modeller DSL extension:");
        Console.WriteLine("  - Open VS Code Extensions (Ctrl+Shift+X)");
        Console.WriteLine("  - Search for 'Modeller DSL'");
        Console.WriteLine("  - Or install from: editors/vscode-modeller/");
        Console.WriteLine("  - After install, enable icons: File > Preferences > File Icon Theme > Modeller DSL Icons");
    }

    private static void CopyDirectory(string sourceDir, string destDir)
    {
        Directory.CreateDirectory(destDir);

        foreach (var file in Directory.GetFiles(sourceDir))
        {
            var destFile = Path.Combine(destDir, Path.GetFileName(file));
            File.Copy(file, destFile, true);
        }

        foreach (var dir in Directory.GetDirectories(sourceDir))
        {
            var destSubDir = Path.Combine(destDir, Path.GetFileName(dir));
            CopyDirectory(dir, destSubDir);
        }
    }

    private static string CreateReadmeContent() => """
        # Modeller Project Configuration

        This folder contains Modeller configuration for code generation.

        ## Quick Start

        ```bash
        # Preview what will be generated
        modeller generate --dry-run

        # Generate code
        modeller generate

        # Validate configuration
        modeller validate
        ```

        ## Folder Structure

        ```
        .modeller/
          config.yaml           # Main configuration
          profiles/             # Generation profiles
            default.yaml
          templates/            # Template files
          README.md             # This file
          llm-instructions.md   # AI assistant context
        ```

        ## Domain Definition Files

        Domain definitions use a custom DSL format. Place them in your domain folder.

        ### File Extensions

        | Extension | Purpose |
        |-----------|---------|
        | `.entity` | Domain entities with attributes and keys |
        | `.enum` | Enumerations with named values |
        | `.command` | Commands (write operations) |
        | `.query` | Queries (read operations) |
        | `.projection` | DTOs and read models |

        See `llm-instructions.md` for complete DSL syntax reference.

        ## Configuration

        Edit `config.yaml` to set:
        - `domain` - Path to domain definition files
        - `variables` - Template variables (company, product, namespace)
        - `output` - Where to generate files

        Edit `profiles/default.yaml` to configure:
        - `layers` - Which layers to generate
        - `include/exclude` - Filter entities, commands, etc.

        ## Generated Files

        - Files with `.g.` in the name (e.g., `Customer.g.cs`) are regenerated on each run
        - Files without `.g.` are never overwritten - safe for your customizations
        - Use partial classes to extend generated code
        """;

    private static string CreateLlmInstructionsContent() => """
        # Modeller DSL Reference for AI Assistants

        This document provides the complete syntax reference for Modeller domain definition files.
        Use this as authoritative reference when helping users create or modify domain definitions.

        ## IMPORTANT: File Format

        Domain definitions use a **custom text-based DSL**, NOT YAML or JSON.
        Each definition type has its own file extension and syntax.

        ---

        ## Entity Definition (.entity)

        Entities represent domain objects with attributes, keys, and indexes.

        ### Syntax

        ```
        # Comments start with #

        entity <EntityName>
          "<description>"

          <AttributeName>: <type> "<description>"
          <AttributeName>: <type>, optional "<description>"
          <AttributeName>: <type>, default(<value>) "<description>"
          <AttributeName>: <EnumName>, optional "<description>"
        end

        key <EntityName>
          <KeyName>: guid, generated

          index [<Attribute>] unique
          index [<Attribute>]
          index [<Attr1>, <Attr2>]
        end
        ```

        ### Example

        ```
        entity Customer
          "A customer in the system"

          Name: text(100) "Customer name"
          Email: text(255) "Email address"
          Status: CustomerStatus "Account status"
          Balance: decimal, default(0) "Account balance"
          Notes: text(500), optional "Additional notes"
          Active: boolean, default(true) "Is active"
          CreatedAt: datetime "When created"
          UpdatedAt: datetime, optional "Last update"
        end

        key Customer
          CustomerId: guid, generated

          index [Email] unique
          index [Status]
          index [Active, Status]
        end
        ```

        ---

        ## Enumeration Definition (.enum)

        Enumerations define named constants with integer values.

        ### Syntax

        ```
        enum <EnumName>
          "<description>"

          <ValueName>: <integer> "<description>"
        end
        ```

        ### Example

        ```
        enum CustomerStatus
          "Customer account status"

          Pending: 0 "Awaiting verification"
          Active: 1 "Active customer"
          Suspended: 2 "Account suspended"
          Closed: 3 "Account closed"
        end
        ```

        ---

        ## Command Definition (.command)

        Commands represent write operations that change state.

        ### Syntax

        ```
        command <CommandName>
          "<description>"

          input
            <ParamName>: <type> "<description>"
            <ParamName>: <type>, optional "<description>"
          end

          output
            <EntityOrType> "<description>"
          end

          errors
            <ErrorName> "<description>"
          end
        end
        ```

        ### Example

        ```
        command CreateCustomer
          "Creates a new customer account"

          input
            Name: text(100) "Customer name"
            Email: text(255) "Email address"
            InitialBalance: decimal, optional "Starting balance"
          end

          output
            Customer "The created customer"
          end

          errors
            DuplicateEmail "A customer with this email already exists"
            ValidationError "Input validation failed"
          end
        end
        ```

        ---

        ## Query Definition (.query)

        Queries represent read operations that return data.

        ### Syntax

        ```
        query <QueryName>
          "<description>"

          input
            <ParamName>: <type> "<description>"
            <ParamName>: <type>, optional "<description>"
          end

          returns <EntityOrProjection>
        end
        ```

        ### Example

        ```
        query GetCustomerById
          "Retrieves a customer by their unique identifier"

          input
            CustomerId: guid "The customer's unique identifier"
          end

          returns Customer
        end

        query ListCustomers
          "Lists customers with optional filtering"

          input
            Status: CustomerStatus, optional "Filter by status"
            PageNumber: integer, optional "Page number (1-based)"
            PageSize: integer, optional "Items per page"
          end

          returns CustomerList
        end
        ```

        ---

        ## Projection Definition (.projection)

        Projections define DTOs, view models, and read-only data structures.

        ### Syntax

        ```
        projection <ProjectionName>
          "<description>"

          <AttributeName>: <type> "<description>"
          <AttributeName>: <type>, computed "<description>"
        end
        ```

        ### Example

        ```
        projection CustomerList
          "Paginated list of customers"

          Customers: list(Customer) "List of customers"
          TotalCount: integer "Total number of customers"
          PageNumber: integer "Current page number"
          PageSize: integer "Items per page"
          TotalPages: integer, computed "Total pages available"
          HasNextPage: boolean, computed "More pages available"
        end

        projection CustomerSummary
          "Summary view of a customer"

          CustomerId: guid "Unique identifier"
          Name: text(100) "Customer name"
          Email: text(255) "Email address"
          Status: CustomerStatus "Account status"
          DisplayName: text(200), computed "Formatted display name"
        end
        ```

        ---

        ## Data Types

        | Type | Description | Example |
        |------|-------------|---------|
        | `text(<length>)` | String with max length | `text(100)` |
        | `integer` | 32-bit integer | `integer` |
        | `long` | 64-bit integer | `long` |
        | `decimal` | Decimal number | `decimal` |
        | `boolean` | True/false | `boolean` |
        | `datetime` | Date and time | `datetime` |
        | `date` | Date only | `date` |
        | `guid` | UUID/GUID | `guid` |
        | `<EnumName>` | Reference to enum | `CustomerStatus` |
        | `list(<Type>)` | Collection | `list(Customer)` |

        ## Attribute Modifiers

        | Modifier | Description | Example |
        |----------|-------------|---------|
        | `optional` | Nullable/not required | `Email: text(255), optional` |
        | `default(<value>)` | Default value | `Active: boolean, default(true)` |
        | `computed` | Calculated property | `TotalPages: integer, computed` |
        | `generated` | Auto-generated (for keys) | `CustomerId: guid, generated` |

        ## Index Modifiers

        | Modifier | Description | Example |
        |----------|-------------|---------|
        | `unique` | Unique constraint | `index [Email] unique` |

        ---

        ## File Organization

        Recommended folder structure for domain definitions:

        ```
        domain/
          entities/
            customer.entity
            order.entity
          enums/
            customer-status.enum
            order-status.enum
          behaviours/
            create-customer.command
            update-customer.command
            get-customer.query
            list-customers.query
          projections/
            customer-list.projection
            customer-summary.projection
        ```

        ## Naming Conventions

        - **File names**: kebab-case (e.g., `create-customer.command`)
        - **Entity/Command/Query names**: PascalCase (e.g., `CreateCustomer`)
        - **Attribute names**: PascalCase (e.g., `CustomerName`)
        - **Enum values**: PascalCase (e.g., `Active`, `Pending`)

        ---

        ## Generated Code Conventions

        - Generated files have `.g.` suffix (e.g., `Customer.g.cs`)
        - Generated files are **always overwritten** on regeneration
        - Files without `.g.` are **never overwritten** - safe for customization
        - Use partial classes to extend generated code

        ### Example Extension Pattern

        ```csharp
        // Customer.g.cs - Generated, always overwritten
        public sealed partial record Customer
        {
            public Guid CustomerId { get; init; }
            public string Name { get; init; } = string.Empty;
            // ... generated properties
        }

        // Customer.cs - Your file, never touched
        public sealed partial record Customer
        {
            public string DisplayName => $"{Name} ({Email})";
        }
        ```
        """;

    private static async Task CreateSampleDomainFiles(string domainPath)
    {
        // Create folder structure
        var entitiesPath = Path.Combine(domainPath, "entities");
        var enumsPath = Path.Combine(domainPath, "enums");
        var behavioursPath = Path.Combine(domainPath, "behaviours");
        var projectionsPath = Path.Combine(domainPath, "projections");

        Directory.CreateDirectory(entitiesPath);
        Directory.CreateDirectory(enumsPath);
        Directory.CreateDirectory(behavioursPath);
        Directory.CreateDirectory(projectionsPath);

        // Sample entity
        await File.WriteAllTextAsync(Path.Combine(entitiesPath, "sample.entity"), """
            # Sample Entity
            # Replace this with your actual domain entity

            entity Sample
              "A sample entity to demonstrate the DSL syntax"

              Name: text(100) "The name"
              Description: text(500), optional "Optional description"
              Status: SampleStatus "Current status"
              Active: boolean, default(true) "Is active"
              CreatedAt: datetime "When created"
            end

            key Sample
              SampleId: guid, generated

              index [Name] unique
              index [Status]
              index [Active]
            end
            """);

        // Sample enum
        await File.WriteAllTextAsync(Path.Combine(enumsPath, "sample-status.enum"), """
            # SampleStatus Enum
            # Replace this with your actual enumeration

            enum SampleStatus
              "Status values for Sample entity"

              Draft: 0 "Initial draft state"
              Active: 1 "Currently active"
              Archived: 2 "Archived/inactive"
            end
            """);

        // Sample command
        await File.WriteAllTextAsync(Path.Combine(behavioursPath, "create-sample.command"), """
            # CreateSample Command
            # Replace this with your actual command

            command CreateSample
              "Creates a new sample"

              input
                Name: text(100) "The name"
                Description: text(500), optional "Optional description"
              end

              output
                Sample "The created sample"
              end

              errors
                DuplicateName "A sample with this name already exists"
                ValidationError "Input validation failed"
              end
            end
            """);

        // Sample query
        await File.WriteAllTextAsync(Path.Combine(behavioursPath, "get-sample.query"), """
            # GetSample Query
            # Replace this with your actual query

            query GetSample
              "Retrieves a sample by its unique identifier"

              input
                SampleId: guid "The sample's unique identifier"
              end

              returns Sample
            end
            """);

        // Sample projection
        await File.WriteAllTextAsync(Path.Combine(projectionsPath, "sample-list.projection"), """
            # SampleList Projection
            # Replace this with your actual projection

            projection SampleList
              "Paginated list of samples"

              Samples: list(Sample) "List of samples"
              TotalCount: integer "Total number available"
              PageNumber: integer "Current page (1-based)"
              PageSize: integer "Items per page"
              TotalPages: integer, computed "Total pages"
              HasNextPage: boolean, computed "More pages available"
            end
            """);
    }

    private static async Task CreateAugmentRules(string projectRoot, string dslReference)
    {
        var rulesPath = Path.Combine(projectRoot, ".augment", "rules");
        Directory.CreateDirectory(rulesPath);

        // Create the DSL reference rule (Auto type - detected when working with domain files)
        var dslRuleContent = $"""
            ---
            description: Reference for Modeller DSL syntax when creating or modifying domain definition files (.entity, .enum, .command, .query, .projection)
            globs:
              - "**/*.entity"
              - "**/*.enum"
              - "**/*.command"
              - "**/*.query"
              - "**/*.projection"
              - "**/domain/**"
              - "**/.modeller/**"
            ---

            {dslReference}
            """;

        await File.WriteAllTextAsync(Path.Combine(rulesPath, "modeller-dsl.md"), dslRuleContent);

        // Create the project conventions rule (Always type - always included)
        var conventionsContent = """
            ---
            description: Modeller project conventions and generated code patterns
            alwaysApply: true
            ---

            # Modeller Project Conventions

            This project uses Modeller for code generation from domain definitions.

            ## Key Conventions

            1. **Domain definitions** are in the `domain/` folder using a custom text-based DSL (NOT YAML)
            2. **File extensions**: `.entity`, `.enum`, `.command`, `.query`, `.projection`
            3. **Generated files** have `.g.` in the name (e.g., `Customer.g.cs`) and are always overwritten
            4. **Your customizations** go in files without `.g.` - these are never touched by the generator
            5. **Use partial classes** to extend generated code

            ## Commands

            ```bash
            modeller generate          # Generate code from domain definitions
            modeller generate --dry-run # Preview what will be generated
            modeller validate          # Validate configuration and domain files
            modeller --help            # Show all commands
            ```

            ## When Modifying Domain Definitions

            - Check `.modeller/llm-instructions.md` for complete DSL syntax reference
            - Domain files use a custom text format, not YAML or JSON
            - Each definition type has its own syntax (entity, enum, command, query, projection)
            """;

        await File.WriteAllTextAsync(Path.Combine(rulesPath, "modeller-conventions.md"), conventionsContent);
    }
}

