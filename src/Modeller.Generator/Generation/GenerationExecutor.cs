using System.Text;
using Modeller.Generator.Configuration;

namespace Modeller.Generator.Generation;

/// <summary>
/// Executes generation plans by rendering templates and writing files
/// </summary>
public sealed class GenerationExecutor
{
    private readonly TemplateEngine _templateEngine;
    private readonly FileConfig _fileConfig;

    public GenerationExecutor(string templatesPath, FileConfig? fileConfig = null)
    {
        _templateEngine = new TemplateEngine(templatesPath);
        _fileConfig = fileConfig ?? new FileConfig();
    }

    /// <summary>
    /// Execute a generation plan
    /// </summary>
    /// <param name="plan">The plan to execute</param>
    /// <param name="dryRun">If true, don't actually write files</param>
    /// <param name="onProgress">Optional callback for progress updates</param>
    /// <returns>Results for each file</returns>
    public async Task<IReadOnlyList<GenerationResult>> ExecuteAsync(
        GenerationPlan plan,
        bool dryRun = false,
        Action<GenerationResult>? onProgress = null)
    {
        var results = new List<GenerationResult>();

        foreach (var file in plan.Files)
        {
            var result = await GenerateFileAsync(file, dryRun);
            results.Add(result);
            onProgress?.Invoke(result);
        }

        return results;
    }

    private async Task<GenerationResult> GenerateFileAsync(PlannedFile file, bool dryRun)
    {
        try
        {
            // Check if file exists and determine action
            var fileExists = File.Exists(file.OutputPath);
            var canOverwrite = file.IsGenerated;

            if (dryRun)
            {
                return new GenerationResult
                {
                    File = file,
                    Success = true,
                    Action = GenerationAction.DryRun
                };
            }

            // Skip non-generated files that exist
            if (fileExists && !canOverwrite)
            {
                return new GenerationResult
                {
                    File = file,
                    Success = true,
                    Action = GenerationAction.Skipped
                };
            }

            // Load and render template
            var templateContent = await File.ReadAllTextAsync(file.TemplatePath);
            var rendered = _templateEngine.RenderFromString(templateContent, file.Context);

            // Apply line ending normalization
            rendered = NormalizeLineEndings(rendered);

            // Ensure directory exists
            var directory = Path.GetDirectoryName(file.OutputPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Write file
            var encoding = GetEncoding();
            await File.WriteAllTextAsync(file.OutputPath, rendered, encoding);

            return new GenerationResult
            {
                File = file,
                Success = true,
                Action = fileExists ? GenerationAction.Overwritten : GenerationAction.Created
            };
        }
        catch (Exception ex)
        {
            return new GenerationResult
            {
                File = file,
                Success = false,
                Action = GenerationAction.Failed,
                Error = ex.Message
            };
        }
    }

    private string NormalizeLineEndings(string content)
    {
        return _fileConfig.LineEnding.ToLowerInvariant() switch
        {
            "lf" => content.Replace("\r\n", "\n"),
            "crlf" => content.Replace("\r\n", "\n").Replace("\n", "\r\n"),
            _ => content // "auto" - keep as is
        };
    }

    private Encoding GetEncoding()
    {
        return _fileConfig.Encoding.ToLowerInvariant() switch
        {
            "utf-8" => new UTF8Encoding(false), // No BOM
            "utf-8-bom" => new UTF8Encoding(true),
            "utf-16" => Encoding.Unicode,
            _ => new UTF8Encoding(false)
        };
    }
}

