using Modeller.Generator.Configuration;
using Modeller.Generator.Generation;

namespace Modeller.Integration.Tests;

/// <summary>
/// End-to-end tests for the plugin template pack against the modeller-units sample domain.
/// Generation runs to a temp directory so the real output folder is never touched.
/// </summary>
public sealed class PluginTemplateGenerationTests : IDisposable
{
    private static readonly string UnitsProjectRoot = FindUnitsProjectRoot();

    private readonly string _tempOutput;
    private readonly LoadedConfiguration _config;
    private readonly Domain.Domain _domain;
    private readonly ProfileConfig _pluginProfile;
    private readonly GenerationPlanner _planner;
    private readonly GenerationExecutor _executor;

    public PluginTemplateGenerationTests()
    {
        _tempOutput = Path.Combine(Path.GetTempPath(), $"modeller-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempOutput);

        var loader = new ConfigLoader();
        _config = loader.LoadConfiguration(UnitsProjectRoot);
        _pluginProfile = loader.LoadProfile(_config.ModellerFolder, "plugin");

        var domainLoader = new DomainLoader();
        _domain = domainLoader.LoadDomain(_config.GetDomainPath());

        var templatesPath = _config.GetTemplatesPath();
        _planner = new GenerationPlanner(templatesPath);
        _executor = new GenerationExecutor(templatesPath, _config.ProjectConfig.Files);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempOutput))
            Directory.Delete(_tempOutput, recursive: true);
    }

    // ── Domain loading ────────────────────────────────────────────────────────

    [Fact]
    public void LoadedDomainHasCorrectName()
    {
        Assert.Equal("UnitsManagement", _domain.Name);
    }

    [Fact]
    public void LoadedDomainHasTwoEntities()
    {
        Assert.Equal(2, _domain.Entities.Count);
    }

    [Fact]
    public void LoadedDomainHasTwoEnums()
    {
        Assert.Equal(2, _domain.Enums.Count);
    }

    [Fact]
    public void LoadedDomainHasFourCommands()
    {
        Assert.Equal(4, _domain.Commands.Count);
    }

    [Fact]
    public void LoadedDomainHasFiveQueries()
    {
        Assert.Equal(5, _domain.Queries.Count);
    }

    // ── Generation plan ───────────────────────────────────────────────────────

    [Fact]
    public void PluginProfileHasFourLayers()
    {
        Assert.Equal(4, _pluginProfile.Layers.Count);
    }

    [Fact]
    public void PluginProfileLayerNamesAreCorrect()
    {
        var names = _pluginProfile.Layers.Select(l => l.Name).ToList();
        Assert.Contains("Infrastructure", names);
        Assert.Contains("SDK", names);
        Assert.Contains("API", names);
        Assert.Contains("UI", names);
    }

    [Fact]
    public void InfrastructureLayerPlanHasEightFiles()
    {
        var layer = _pluginProfile.Layers.Single(l => l.Name == "Infrastructure");
        var plan = CreatePlan(layer);
        Assert.Equal(8, plan.Files.Count);
    }

    [Fact]
    public void ApiLayerPlanHasSixFiles()
    {
        var layer = _pluginProfile.Layers.Single(l => l.Name == "API");
        var plan = CreatePlan(layer);
        Assert.Equal(6, plan.Files.Count);
    }

    [Fact]
    public void SdkLayerPlanHasTwentyOneFiles()
    {
        var layer = _pluginProfile.Layers.Single(l => l.Name == "SDK");
        var plan = CreatePlan(layer);
        Assert.Equal(21, plan.Files.Count);
    }

    [Fact]
    public void ApiLayerPlanHasOneEndpointsFilePerEntity()
    {
        var layer = _pluginProfile.Layers.Single(l => l.Name == "API");
        var plan = CreatePlan(layer);
        var endpointFiles = plan.Files.Where(f => f.RelativePath.EndsWith("Endpoints.g.cs")).ToList();
        Assert.Equal(_domain.Entities.Count, endpointFiles.Count);
    }

    // ── File generation ───────────────────────────────────────────────────────

    [Fact]
    public async Task InfrastructureLayerGeneratesAllFiles()
    {
        var layer = _pluginProfile.Layers.Single(l => l.Name == "Infrastructure");
        var results = await RunLayerAsync(layer);

        Assert.All(results, r => Assert.NotEqual(GenerationAction.Failed, r.Action));
        Assert.Equal(8, results.Count(r => r.Action == GenerationAction.Created));
    }

    [Fact]
    public async Task ApiLayerGeneratesAllFiles()
    {
        var layer = _pluginProfile.Layers.Single(l => l.Name == "API");
        var results = await RunLayerAsync(layer);

        Assert.All(results, r => Assert.NotEqual(GenerationAction.Failed, r.Action));
        Assert.Equal(6, results.Count(r => r.Action == GenerationAction.Created));
    }

    [Fact]
    public async Task SdkLayerGeneratesAllFiles()
    {
        var layer = _pluginProfile.Layers.Single(l => l.Name == "SDK");
        var results = await RunLayerAsync(layer);

        Assert.All(results, r => Assert.NotEqual(GenerationAction.Failed, r.Action));
        Assert.Equal(21, results.Count(r => r.Action == GenerationAction.Created));
    }

    [Fact]
    public async Task AllLayersGenerateTotalOfThirtyNineFiles()
    {
        var allResults = new List<GenerationResult>();
        foreach (var layer in _pluginProfile.Layers)
            allResults.AddRange(await RunLayerAsync(layer));

        Assert.Equal(39, allResults.Count);
        Assert.DoesNotContain(allResults, r => r.Action == GenerationAction.Failed);
    }

    [Fact]
    public async Task DryRunCreatesNoFilesOnDisk()
    {
        var layer = _pluginProfile.Layers.Single(l => l.Name == "Infrastructure");
        var plan = CreatePlan(layer);
        var results = await _executor.ExecuteAsync(plan, dryRun: true);

        Assert.All(results, r => Assert.Equal(GenerationAction.DryRun, r.Action));
        Assert.Empty(Directory.GetFiles(_tempOutput, "*", SearchOption.AllDirectories));
    }

    // ── Output path variable substitution ────────────────────────────────────

    [Fact]
    public async Task OutputFolderNamesContainCompanyAndProduct()
    {
        foreach (var layer in _pluginProfile.Layers)
            await RunLayerAsync(layer);

        var subdirs = Directory.GetDirectories(_tempOutput).Select(Path.GetFileName).ToList();
        Assert.All(subdirs, d => Assert.StartsWith("JJs.UnitsManagement.", d));
    }

    // ── Generated content spot-checks ────────────────────────────────────────

    [Fact]
    public async Task GeneratedDbContextContainsEntityDbSets()
    {
        var layer = _pluginProfile.Layers.Single(l => l.Name == "Infrastructure");
        await RunLayerAsync(layer);

        var dbContextFile = Directory.GetFiles(_tempOutput, "UnitsManagementDbContext.g.cs", SearchOption.AllDirectories)
            .SingleOrDefault();
        Assert.NotNull(dbContextFile);

        var content = await File.ReadAllTextAsync(dbContextFile);
        Assert.Contains("DbSet<", content);
        Assert.Contains("Unit", content);
        Assert.Contains("SyncJob", content);
    }

    [Fact]
    public async Task GeneratedApiEndpointsContainEntityNamespace()
    {
        var layer = _pluginProfile.Layers.Single(l => l.Name == "API");
        await RunLayerAsync(layer);

        var unitsEndpoints = Directory.GetFiles(_tempOutput, "UnitsEndpoints.g.cs", SearchOption.AllDirectories)
            .SingleOrDefault();
        Assert.NotNull(unitsEndpoints);

        var content = await File.ReadAllTextAsync(unitsEndpoints);
        Assert.Contains("namespace", content);
        Assert.Contains("UnitsEndpoints", content);
        Assert.Contains("MapGroup", content);
    }

    [Fact]
    public async Task GeneratedSdkCreateRequestContainsValidatorClass()
    {
        var layer = _pluginProfile.Layers.Single(l => l.Name == "SDK");
        await RunLayerAsync(layer);

        var validatorFile = Directory.GetFiles(_tempOutput, "CreateUnitValidator.g.cs", SearchOption.AllDirectories)
            .SingleOrDefault();
        Assert.NotNull(validatorFile);

        var content = await File.ReadAllTextAsync(validatorFile);
        Assert.Contains("Validator", content);
    }

    [Fact]
    public async Task GeneratedInfrastructureEntityConfigurationExistsPerEntity()
    {
        var layer = _pluginProfile.Layers.Single(l => l.Name == "Infrastructure");
        await RunLayerAsync(layer);

        var configFiles = Directory.GetFiles(_tempOutput, "*Configuration.g.cs", SearchOption.AllDirectories);
        Assert.Equal(_domain.Entities.Count, configFiles.Length);
    }

    [Fact]
    public async Task GeneratedApiCsprojUsesExpectedProjectName()
    {
        var layer = _pluginProfile.Layers.Single(l => l.Name == "API");
        await RunLayerAsync(layer);

        var csprojFile = Directory.GetFiles(_tempOutput, "*.g.csproj", SearchOption.AllDirectories)
            .SingleOrDefault(f => f.Contains("Api"));
        Assert.NotNull(csprojFile);
        Assert.Contains("JJs.UnitsManagement.Api", csprojFile);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private GenerationPlan CreatePlan(LayerConfig layer) =>
        _planner.CreatePlan(_domain, layer, _pluginProfile, _config.ProjectConfig, _tempOutput, []);

    private async Task<IReadOnlyList<GenerationResult>> RunLayerAsync(LayerConfig layer)
    {
        var plan = CreatePlan(layer);
        return await _executor.ExecuteAsync(plan);
    }

    private static string FindUnitsProjectRoot()
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
}
