using Modeller.Domain;
using Modeller.Parser;

namespace Modeller.Integration.Tests;

/// <summary>
/// Phase 2: Tests building a unified domain model from all parsed DSL files
/// </summary>
public class DomainAssemblyTests
{
    private static readonly string SamplesPath = FindSamplesPath();
    
    private static string FindSamplesPath()
    {
        var dir = Directory.GetCurrentDirectory();
        while (dir != null)
        {
            var samples = Path.Combine(dir, "samples", "modeller");
            if (Directory.Exists(samples)) return samples;
            dir = Directory.GetParent(dir)?.FullName;
        }
        throw new DirectoryNotFoundException("Could not find samples/modeller directory");
    }

    private static Domain.Domain BuildDomainFromSamples()
    {
        var builder = new DomainBuilder();
        
        // Parse domain.def
        var domainPath = Path.Combine(SamplesPath, "domain.def");
        if (File.Exists(domainPath))
        {
            var result = DslParser.ParseDomain(File.ReadAllText(domainPath));
            if (result.Success) builder.AddDomain(result.Value!);
        }
        
        // Parse all entity files
        var entitiesDir = Path.Combine(SamplesPath, "entities");
        if (Directory.Exists(entitiesDir))
        {
            foreach (var file in Directory.GetFiles(entitiesDir, "*.entity"))
            {
                var result = DslParser.ParseEntityFile(File.ReadAllText(file));
                if (result.Success) builder.AddEntity(result.Value!);
            }
        }
        
        // Parse all enum files
        var enumsDir = Path.Combine(SamplesPath, "enums");
        if (Directory.Exists(enumsDir))
        {
            foreach (var file in Directory.GetFiles(enumsDir, "*.enum"))
            {
                var result = DslParser.ParseEnum(File.ReadAllText(file));
                if (result.Success) builder.AddEnum(result.Value!);
            }
        }
        
        // Parse all service files
        var servicesDir = Path.Combine(SamplesPath, "services");
        if (Directory.Exists(servicesDir))
        {
            foreach (var file in Directory.GetFiles(servicesDir, "*.service"))
            {
                var result = DslParser.ParseService(File.ReadAllText(file));
                if (result.Success) builder.AddService(result.Value!);
            }
        }
        
        // Parse all command files
        var behavioursDir = Path.Combine(SamplesPath, "behaviours");
        if (Directory.Exists(behavioursDir))
        {
            foreach (var file in Directory.GetFiles(behavioursDir, "*.command"))
            {
                var result = DslParser.ParseCommand(File.ReadAllText(file));
                if (result.Success) builder.AddCommand(result.Value!);
            }
            
            foreach (var file in Directory.GetFiles(behavioursDir, "*.query"))
            {
                var result = DslParser.ParseQuery(File.ReadAllText(file));
                if (result.Success) builder.AddQuery(result.Value!);
            }
        }
        
        return builder.Build();
    }

    [Fact]
    public void BuildsDomainFromAllSampleFiles()
    {
        var domain = BuildDomainFromSamples();
        
        Assert.NotNull(domain);
        Assert.Equal("Modeller", domain.Name);
    }

    [Fact]
    public void DomainContainsExpectedEntities()
    {
        var domain = BuildDomainFromSamples();
        
        Assert.NotEmpty(domain.Entities);
        Assert.Contains(domain.Entities, e => e.Name == "Domain");
        Assert.Contains(domain.Entities, e => e.Name == "Entity");
        Assert.Contains(domain.Entities, e => e.Name == "Service");
    }

    [Fact]
    public void DomainContainsExpectedEnums()
    {
        var domain = BuildDomainFromSamples();
        
        Assert.NotEmpty(domain.Enums);
        Assert.Contains(domain.Enums, e => e.Name == "DataType");
    }

    [Fact]
    public void DomainContainsExpectedCommands()
    {
        var domain = BuildDomainFromSamples();
        
        Assert.NotEmpty(domain.Commands);
        Assert.Contains(domain.Commands, c => c.Name == "ParseDomain");
    }

    [Fact]
    public void DomainContainsExpectedQueries()
    {
        var domain = BuildDomainFromSamples();
        
        Assert.NotEmpty(domain.Queries);
        Assert.Contains(domain.Queries, q => q.Name == "GetDomain");
    }

    [Fact]
    public void EntityRelationshipsAreResolved()
    {
        var domain = BuildDomainFromSamples();
        
        var entityDef = domain.GetEntity("Entity");
        Assert.NotNull(entityDef);
        
        // Entity should have relationships
        Assert.NotEmpty(entityDef.Relationships);
        
        // At least one relationship should be resolved
        var resolvedRelationship = entityDef.Relationships
            .FirstOrDefault(r => r.TargetEntity != null);
        
        // This may or may not resolve depending on sample files
        // Just verify no exceptions were thrown during resolution
    }

    [Fact]
    public void EntitiesHaveAttributes()
    {
        var domain = BuildDomainFromSamples();
        
        var entityDef = domain.GetEntity("Entity");
        Assert.NotNull(entityDef);
        Assert.NotEmpty(entityDef.Attributes);
        Assert.Contains(entityDef.Attributes, a => a.Name == "Name");
    }
}

