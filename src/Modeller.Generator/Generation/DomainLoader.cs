using Modeller.Domain;
using Modeller.Parser;

namespace Modeller.Generator.Generation;

/// <summary>
/// Loads domain definitions from a folder containing definition files
/// </summary>
public sealed class DomainLoader
{
    /// <summary>
    /// Load a domain from a folder containing definition files
    /// </summary>
    /// <param name="domainPath">Path to the domain folder</param>
    /// <returns>The loaded domain</returns>
    public Domain.Domain LoadDomain(string domainPath)
    {
        if (!Directory.Exists(domainPath))
            throw new DirectoryNotFoundException($"Domain folder not found: {domainPath}");

        var builder = new DomainBuilder();

        // Load domain definition (.domain file)
        var domainFiles = Directory.GetFiles(domainPath, "*.def", SearchOption.AllDirectories);
        foreach (var file in domainFiles)
        {
            var content = File.ReadAllText(file);
            var result = DslParser.ParseDomain(content);
            if (result.Success && result.Value != null)
            {
                builder.AddDomain(result.Value);
            }
            else
            {
                throw new InvalidOperationException($"Failed to parse domain file {file}: {result.Error}");
            }
        }

        // Load entity definitions (.entity files)
        var entityFiles = Directory.GetFiles(domainPath, "*.entity", SearchOption.AllDirectories);
        foreach (var file in entityFiles)
        {
            var content = File.ReadAllText(file);
            var result = DslParser.ParseEntityFile(content);
            if (result.Success && result.Value != null)
            {
                builder.AddEntity(result.Value);
            }
            else
            {
                throw new InvalidOperationException($"Failed to parse entity file {file}: {result.Error}");
            }
        }

        // Load enum definitions (.enum files)
        var enumFiles = Directory.GetFiles(domainPath, "*.enum", SearchOption.AllDirectories);
        foreach (var file in enumFiles)
        {
            var content = File.ReadAllText(file);
            var result = DslParser.ParseEnum(content);
            if (result.Success && result.Value != null)
            {
                builder.AddEnum(result.Value);
            }
            else
            {
                throw new InvalidOperationException($"Failed to parse enum file {file}: {result.Error}");
            }
        }

        // Load command definitions (.command files)
        var commandFiles = Directory.GetFiles(domainPath, "*.command", SearchOption.AllDirectories);
        foreach (var file in commandFiles)
        {
            var content = File.ReadAllText(file);
            var result = DslParser.ParseCommand(content);
            if (result.Success && result.Value != null)
            {
                builder.AddCommand(result.Value);
            }
            else
            {
                throw new InvalidOperationException($"Failed to parse command file {file}: {result.Error}");
            }
        }

        // Load query definitions (.query files)
        var queryFiles = Directory.GetFiles(domainPath, "*.query", SearchOption.AllDirectories);
        foreach (var file in queryFiles)
        {
            var content = File.ReadAllText(file);
            var result = DslParser.ParseQuery(content);
            if (result.Success && result.Value != null)
            {
                builder.AddQuery(result.Value);
            }
            else
            {
                throw new InvalidOperationException($"Failed to parse query file {file}: {result.Error}");
            }
        }

        return builder.Build();
    }

    /// <summary>
    /// Get statistics about what was loaded
    /// </summary>
    public DomainStats GetStats(Domain.Domain domain) => new(
        Entities: domain.Entities.Count,
        Enums: domain.Enums.Count,
        Commands: domain.Commands.Count,
        Queries: domain.Queries.Count,
        Services: domain.Services.Count
    );
}

/// <summary>
/// Statistics about a loaded domain
/// </summary>
public sealed record DomainStats(
    int Entities,
    int Enums,
    int Commands,
    int Queries,
    int Services
);

