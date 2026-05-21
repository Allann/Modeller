using Modeller.Generator.Generation;

namespace Modeller.Integration.Tests;

/// <summary>
/// Targeted tests for DomainLoader against the modeller-units sample domain.
/// </summary>
public class DomainLoaderTests
{
    private static readonly string UnitsPath = FindUnitsPath();

    private static string FindUnitsPath()
    {
        var dir = Directory.GetCurrentDirectory();
        while (dir != null)
        {
            var candidate = Path.Combine(dir, "samples", "modeller-units");
            if (Directory.Exists(candidate)) return candidate;
            dir = Directory.GetParent(dir)?.FullName;
        }
        throw new DirectoryNotFoundException("Could not find samples/modeller-units directory");
    }

    [Fact]
    public void LoadDomain_ThrowsWhenPathDoesNotExist()
    {
        var loader = new DomainLoader();
        Assert.Throws<DirectoryNotFoundException>(() =>
            loader.LoadDomain(Path.Combine(UnitsPath, "does-not-exist")));
    }

    [Fact]
    public void LoadDomain_ReturnsCorrectDomainName()
    {
        var loader = new DomainLoader();
        var domain = loader.LoadDomain(UnitsPath);
        Assert.Equal("UnitsManagement", domain.Name);
    }

    [Fact]
    public void LoadDomain_LoadsEntities()
    {
        var loader = new DomainLoader();
        var domain = loader.LoadDomain(UnitsPath);
        Assert.NotEmpty(domain.Entities);
    }

    [Fact]
    public void LoadDomain_EntityNamesAreNonEmpty()
    {
        var loader = new DomainLoader();
        var domain = loader.LoadDomain(UnitsPath);
        Assert.All(domain.Entities, e => Assert.False(string.IsNullOrWhiteSpace(e.Name)));
    }

    [Fact]
    public void LoadDomain_LoadsEnums()
    {
        var loader = new DomainLoader();
        var domain = loader.LoadDomain(UnitsPath);
        Assert.NotEmpty(domain.Enums);
    }

    [Fact]
    public void LoadDomain_EnumNamesAreNonEmpty()
    {
        var loader = new DomainLoader();
        var domain = loader.LoadDomain(UnitsPath);
        Assert.All(domain.Enums, e => Assert.False(string.IsNullOrWhiteSpace(e.Name)));
    }

    [Fact]
    public void LoadDomain_LoadsCommands()
    {
        var loader = new DomainLoader();
        var domain = loader.LoadDomain(UnitsPath);
        Assert.NotEmpty(domain.Commands);
    }

    [Fact]
    public void LoadDomain_LoadsQueries()
    {
        var loader = new DomainLoader();
        var domain = loader.LoadDomain(UnitsPath);
        Assert.NotEmpty(domain.Queries);
    }

    [Fact]
    public void GetStats_ReturnsMatchingCounts()
    {
        var loader = new DomainLoader();
        var domain = loader.LoadDomain(UnitsPath);
        var stats = loader.GetStats(domain);

        Assert.Equal(domain.Entities.Count, stats.Entities);
        Assert.Equal(domain.Enums.Count, stats.Enums);
        Assert.Equal(domain.Commands.Count, stats.Commands);
        Assert.Equal(domain.Queries.Count, stats.Queries);
    }

    [Fact]
    public void LoadDomain_ContainsUnitEntity()
    {
        var loader = new DomainLoader();
        var domain = loader.LoadDomain(UnitsPath);
        Assert.Contains(domain.Entities, e => e.Name == "Unit");
    }

    [Fact]
    public void LoadDomain_ContainsSyncJobEntity()
    {
        var loader = new DomainLoader();
        var domain = loader.LoadDomain(UnitsPath);
        Assert.Contains(domain.Entities, e => e.Name == "SyncJob");
    }
}
