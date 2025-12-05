using Modeller.Generator.Templates;
using Scriban;
using Scriban.Parsing;

namespace Modeller.Generator.Tests.TemplatesTests;

public class SnippetLoaderTests : IDisposable
{
    private readonly string _tempDir;
    private readonly SnippetLoader _loader;

    public SnippetLoaderTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"SnippetLoaderTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
        _loader = new SnippetLoader(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, recursive: true);
        }
    }

    [Fact]
    public void GetPath_ReturnsAbsolutePath()
    {
        // Arrange
        var context = new TemplateContext();
        var span = new SourceSpan();

        // Act
        var path = _loader.GetPath(context, span, "_snippets/csharp/header");

        // Assert
        Assert.True(Path.IsPathRooted(path));
        Assert.EndsWith(".scriban", path);
    }

    [Fact]
    public void GetPath_AddsScribanExtension()
    {
        // Arrange
        var context = new TemplateContext();
        var span = new SourceSpan();

        // Act
        var path = _loader.GetPath(context, span, "_snippets/csharp/header");

        // Assert
        Assert.EndsWith("header.scriban", path);
    }

    [Fact]
    public void GetPath_DoesNotDuplicateExtension()
    {
        // Arrange
        var context = new TemplateContext();
        var span = new SourceSpan();

        // Act
        var path = _loader.GetPath(context, span, "_snippets/csharp/header.scriban");

        // Assert
        Assert.EndsWith("header.scriban", path);
        Assert.DoesNotContain(".scriban.scriban", path);
    }

    [Fact]
    public void GetPath_ThrowsForPathOutsideRoot()
    {
        // Arrange
        var context = new TemplateContext();
        var span = new SourceSpan();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            _loader.GetPath(context, span, "../../../etc/passwd"));
    }

    [Fact]
    public void Load_ReturnsFileContent()
    {
        // Arrange
        var snippetDir = Path.Combine(_tempDir, "_snippets", "csharp");
        Directory.CreateDirectory(snippetDir);
        var snippetPath = Path.Combine(snippetDir, "test.scriban");
        File.WriteAllText(snippetPath, "Hello {{ name }}!");

        var context = new TemplateContext();
        var span = new SourceSpan();
        var path = _loader.GetPath(context, span, "_snippets/csharp/test");

        // Act
        var content = _loader.Load(context, span, path);

        // Assert
        Assert.Equal("Hello {{ name }}!", content);
    }

    [Fact]
    public void Load_CachesContent()
    {
        // Arrange
        var snippetDir = Path.Combine(_tempDir, "_snippets", "csharp");
        Directory.CreateDirectory(snippetDir);
        var snippetPath = Path.Combine(snippetDir, "cached.scriban");
        File.WriteAllText(snippetPath, "Original content");

        var context = new TemplateContext();
        var span = new SourceSpan();
        var path = _loader.GetPath(context, span, "_snippets/csharp/cached");

        // Act
        var content1 = _loader.Load(context, span, path);
        File.WriteAllText(snippetPath, "Modified content");
        var content2 = _loader.Load(context, span, path);

        // Assert
        Assert.Equal("Original content", content1);
        Assert.Equal("Original content", content2); // Still cached
        Assert.Equal(1, _loader.CacheCount);
    }

    [Fact]
    public void Load_ThrowsForMissingFile()
    {
        // Arrange
        var context = new TemplateContext();
        var span = new SourceSpan();
        var path = _loader.GetPath(context, span, "_snippets/csharp/nonexistent");

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() =>
            _loader.Load(context, span, path));
    }

    [Fact]
    public void ClearCache_RemovesCachedContent()
    {
        // Arrange
        var snippetDir = Path.Combine(_tempDir, "_snippets", "csharp");
        Directory.CreateDirectory(snippetDir);
        var snippetPath = Path.Combine(snippetDir, "clearable.scriban");
        File.WriteAllText(snippetPath, "Original");

        var context = new TemplateContext();
        var span = new SourceSpan();
        var path = _loader.GetPath(context, span, "_snippets/csharp/clearable");
        _loader.Load(context, span, path);
        Assert.Equal(1, _loader.CacheCount);

        // Act
        _loader.ClearCache();

        // Assert
        Assert.Equal(0, _loader.CacheCount);
    }
}

