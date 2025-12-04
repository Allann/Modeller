using Modeller.Domain;
using Modeller.Generator;
using Modeller.Parser;

namespace Modeller.Integration.Tests;

/// <summary>
/// Phase 3: Tests generating C# records from the domain model
/// </summary>
public class CodeGenerationTests
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
        
        var domainPath = Path.Combine(SamplesPath, "domain.def");
        if (File.Exists(domainPath))
        {
            var result = DslParser.ParseDomain(File.ReadAllText(domainPath));
            if (result.Success) builder.AddDomain(result.Value!);
        }
        
        var entitiesDir = Path.Combine(SamplesPath, "entities");
        if (Directory.Exists(entitiesDir))
        {
            foreach (var file in Directory.GetFiles(entitiesDir, "*.entity"))
            {
                var result = DslParser.ParseEntityFile(File.ReadAllText(file));
                if (result.Success) builder.AddEntity(result.Value!);
            }
        }
        
        var enumsDir = Path.Combine(SamplesPath, "enums");
        if (Directory.Exists(enumsDir))
        {
            foreach (var file in Directory.GetFiles(enumsDir, "*.enum"))
            {
                var result = DslParser.ParseEnum(File.ReadAllText(file));
                if (result.Success) builder.AddEnum(result.Value!);
            }
        }
        
        return builder.Build();
    }

    [Fact]
    public void GeneratesFilesForAllEntities()
    {
        var domain = BuildDomainFromSamples();
        var generator = new CSharpRecordGenerator();
        
        var files = generator.Generate(domain).ToList();
        
        Assert.NotEmpty(files);
        
        // Should have one file per entity plus one per enum
        var expectedCount = domain.Entities.Count + domain.Enums.Count;
        Assert.Equal(expectedCount, files.Count);
    }

    [Fact]
    public void GeneratedEntityFileContainsRecordDeclaration()
    {
        var domain = BuildDomainFromSamples();
        var generator = new CSharpRecordGenerator();
        
        var files = generator.Generate(domain).ToList();
        var entityFile = files.FirstOrDefault(f => f.Path == "Entity.cs");
        
        Assert.NotNull(entityFile);
        Assert.Contains("public sealed record Entity", entityFile.Content);
    }

    [Fact]
    public void GeneratedEnumFileContainsEnumDeclaration()
    {
        var domain = BuildDomainFromSamples();
        var generator = new CSharpRecordGenerator();
        
        var files = generator.Generate(domain).ToList();
        var enumFile = files.FirstOrDefault(f => f.Path == "DataType.cs");
        
        Assert.NotNull(enumFile);
        Assert.Contains("public enum DataType", enumFile.Content);
    }

    [Fact]
    public void GeneratedRecordsIncludeNamespace()
    {
        var domain = BuildDomainFromSamples();
        var generator = new CSharpRecordGenerator();
        
        var files = generator.Generate(domain).ToList();
        
        foreach (var file in files)
        {
            Assert.Contains("namespace Generated;", file.Content);
        }
    }

    [Fact]
    public void GeneratedRecordIncludesAttributes()
    {
        var domain = BuildDomainFromSamples();
        var generator = new CSharpRecordGenerator();
        
        var files = generator.Generate(domain).ToList();
        var entityFile = files.FirstOrDefault(f => f.Path == "Entity.cs");
        
        Assert.NotNull(entityFile);
        // Entity should have a Name attribute
        Assert.Contains("Name", entityFile.Content);
    }

    [Fact]
    public void GeneratedCodeIsValidCSharpSyntax()
    {
        var domain = BuildDomainFromSamples();
        var generator = new CSharpRecordGenerator();
        
        var files = generator.Generate(domain).ToList();
        
        foreach (var file in files)
        {
            // Basic syntax checks
            Assert.Contains("namespace", file.Content);
            
            if (file.Path.EndsWith(".cs"))
            {
                // Either record or enum
                Assert.True(
                    file.Content.Contains("record") || file.Content.Contains("enum"),
                    $"File {file.Path} should contain either 'record' or 'enum'");
            }
        }
    }
}

