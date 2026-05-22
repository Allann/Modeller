using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Modeller.LanguageServer.Handlers;

public class HoverHandler(DocumentManager documentManager) : HoverHandlerBase
{
    private static readonly TextDocumentSelector Selector = LanguageIds.CreateSelector();

    private static readonly Dictionary<string, string> Docs = new()
    {
        ["text"]           = "Unicode string. Optional max length: `text(100)` or `text(max)`.",
        ["integer"]        = "32-bit signed integer.",
        ["long"]           = "64-bit signed integer.",
        ["decimal"]        = "Exact decimal number.",
        ["boolean"]        = "`true` or `false`.",
        ["date"]           = "Calendar date (no time component).",
        ["time"]           = "Time of day (no date component).",
        ["datetime"]       = "Combined date and time.",
        ["guid"]           = "Globally unique identifier (UUID v4).",
        ["id"]             = "Auto-generated GUID primary key — shorthand for `guid, generated`.",
        ["name"]           = "Human-readable name string.",
        ["binary"]         = "Raw byte array.",
        ["email"]          = "Email address.",
        ["url"]            = "URL string.",
        ["money"]          = "Monetary value (amount + currency code).",
        ["percentage"]     = "Percentage value.",
        ["geospatial"]     = "Latitude/longitude coordinate.",
        ["image"]          = "Image binary content.",
        ["document"]       = "Document binary content.",
        ["has_one"]        = "Single entity reference (foreign key).",
        ["has_many"]       = "Ordered collection of related entities.",
        ["belongs_to"]     = "Inverse side of a `has_one` relationship.",
        ["many_to_many"]   = "Bidirectional collection relationship.",
        ["references"]     = "Read-only reference to an entity owned by another service.",
        ["optional"]       = "Field may be null/absent.",
        ["unique"]         = "Field value must be unique within the entity.",
        ["transport"]      = "Communication protocol: `http` (default) or `grpc`.",
        ["streaming"]      = "Streaming mode: `none` (default), `server`, `client`, or `bidirectional`.",
        ["returns"]        = "Query return type. Use `returns many TypeName` for a collection.",
        ["publishes"]      = "Domain events emitted when this command succeeds.",
        ["variant"]        = "A named case in a discriminated union.",
        ["input"]          = "Block of input parameters.",
        ["output"]         = "Output type returned on success.",
        ["errors"]         = "Error types this operation can raise.",
        ["entities"]       = "Entities owned by this service.",
        ["enums"]          = "Enums owned by this service.",
        ["services"]       = "Services belonging to this domain.",
        ["calls"]          = "Commands in other services this service invokes.",
        ["implements"]     = "Commands that this service handles.",
        ["company"]        = "Organisation that owns this domain.",
        ["version"]        = "Semantic version of this domain model.",
        ["generated"]      = "Key field value is auto-generated.",
        ["index"]          = "Secondary index on one or more key fields. Add `unique` for a unique constraint.",
    };

    protected override HoverRegistrationOptions CreateRegistrationOptions(
        HoverCapability capability,
        ClientCapabilities clientCapabilities) => new()
    {
        DocumentSelector = Selector
    };

    public override Task<Hover?> Handle(HoverParams request, CancellationToken cancellationToken)
    {
        var content = documentManager.GetContent(request.TextDocument.Uri);
        if (content is null)
            return Task.FromResult<Hover?>(null);

        var word = WordAt(content, request.Position);
        if (word is null || !Docs.TryGetValue(word, out var doc))
            return Task.FromResult<Hover?>(null);

        return Task.FromResult<Hover?>(new Hover
        {
            Contents = new MarkedStringsOrMarkupContent(new MarkupContent
            {
                Kind  = MarkupKind.Markdown,
                Value = $"**{word}** — {doc}"
            })
        });
    }

    private static string? WordAt(string content, Position pos)
    {
        var lines = content.Split('\n');
        if ((int)pos.Line >= lines.Length) return null;

        var line = lines[(int)pos.Line];
        var col  = Math.Min((int)pos.Character, line.Length);

        // expand left
        var start = col;
        while (start > 0 && IsWordChar(line[start - 1])) start--;

        // expand right
        var end = col;
        while (end < line.Length && IsWordChar(line[end])) end++;

        return start == end ? null : line[start..end];
    }

    private static bool IsWordChar(char c) => char.IsLetterOrDigit(c) || c == '_';
}
