using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Modeller.LanguageServer.Completions;

public static class CompletionProvider
{
    // Block keywords that consume an "end" — sub-blocks that stand alone on their line
    private static readonly HashSet<string> UnnamedBlocks =
    [
        "input", "output", "errors", "publishes",
        "entities", "enums", "services",
        "references", "calls", "implements"
    ];

    // Top-level block keywords that are always followed by a name
    private static readonly HashSet<string> NamedBlocks =
    [
        "entity", "key", "enum", "flags", "service", "command", "query",
        "value", "shared", "event", "projection", "union", "domain", "variant"
    ];

    public static IEnumerable<CompletionItem> GetCompletions(
        string content, Position cursor, string fileExtension)
    {
        var lines = content.Split('\n');
        var ctx   = AnalyseContext(lines, cursor);

        if (ctx.IsTopLevel)
            return TopLevelCompletions(fileExtension);

        if (ctx.IsAfterColon)
            return TypeCompletions();

        if (ctx.IsAfterType)
            return ModifierCompletions();

        if (ctx.IsAfterTransport)
            return CompletionData.TransportValues.Select(Keyword);

        if (ctx.IsAfterStreaming)
            return CompletionData.StreamingValues.Select(Keyword);

        if (ctx.IsAfterReturns)
            return [Keyword("many"), Snippet("many <Type>", "many ${1:TypeName}", "many <TypeName>")];

        return BlockCompletions(ctx.InnermostBlock, fileExtension);
    }

    // ── context analysis ──────────────────────────────────────────────────────

    private static Context AnalyseContext(string[] lines, Position cursor)
    {
        var stack = new Stack<string>();

        for (var i = 0; i <= (int)cursor.Line && i < lines.Length; i++)
        {
            var raw     = i == (int)cursor.Line
                            ? lines[i][..Math.Min((int)cursor.Character, lines[i].Length)]
                            : lines[i];
            var trimmed = raw.TrimStart();

            if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith('#'))
                continue;

            // block close
            if (trimmed == "end" || trimmed.StartsWith("end ") || trimmed.StartsWith("end#"))
            {
                if (stack.Count > 0) stack.Pop();
                continue;
            }

            // block open
            var opener = DetectBlockOpen(trimmed);
            if (opener is not null)
            {
                stack.Push(opener);
                continue;
            }
        }

        var cursorLine   = cursor.Line < lines.Length ? lines[(int)cursor.Line] : "";
        var upToCursor   = cursorLine[..Math.Min((int)cursor.Character, cursorLine.Length)].TrimStart();
        var ctx          = new Context { BlockStack = stack, CurrentLine = upToCursor };

        ctx.IsTopLevel = stack.Count == 0;

        // "FieldName: " — colon with no space before it (not a keyword line)
        var colonIdx = upToCursor.IndexOf(':');
        if (colonIdx > 0 && !upToCursor[..colonIdx].Contains(' '))
        {
            ctx.IsAfterColon = true;
            var afterColon = upToCursor[(colonIdx + 1)..].TrimStart();

            // if a type word already present and there's trailing whitespace → modifier context
            var typeWord = afterColon.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            if (typeWord is not null &&
                CompletionData.PrimitiveTypes.Contains(typeWord) &&
                afterColon.Length > typeWord.Length)
            {
                ctx.IsAfterType  = true;
                ctx.IsAfterColon = false;
            }
        }

        if (upToCursor.TrimStart().StartsWith("transport "))  ctx.IsAfterTransport = true;
        if (upToCursor.TrimStart().StartsWith("streaming "))  ctx.IsAfterStreaming  = true;
        if (upToCursor.TrimStart().StartsWith("returns "))    ctx.IsAfterReturns    = true;

        return ctx;
    }

    private static string? DetectBlockOpen(string trimmed)
    {
        foreach (var kw in NamedBlocks)
            if (trimmed == kw || trimmed.StartsWith(kw + " ") || trimmed.StartsWith(kw + "\t"))
                return kw;

        foreach (var kw in UnnamedBlocks)
            if (trimmed == kw)
                return kw;

        return null;
    }

    // ── completion builders ───────────────────────────────────────────────────

    private static IEnumerable<CompletionItem> TopLevelCompletions(string ext)
    {
        foreach (var (label, detail, snippet) in CompletionData.TopLevelSnippets(ext))
            yield return new CompletionItem
            {
                Label            = label,
                Kind             = CompletionItemKind.Snippet,
                Detail           = detail,
                InsertTextFormat = InsertTextFormat.Snippet,
                InsertText       = snippet
            };
    }

    private static IEnumerable<CompletionItem> TypeCompletions() =>
        CompletionData.PrimitiveTypes.Select(t => new CompletionItem
        {
            Label  = t,
            Kind   = CompletionItemKind.TypeParameter,
            Detail = "Built-in type"
        });

    private static IEnumerable<CompletionItem> ModifierCompletions()
    {
        yield return Keyword("optional");
        yield return Keyword("unique");
        yield return new CompletionItem
        {
            Label            = "default",
            Kind             = CompletionItemKind.Keyword,
            InsertTextFormat = InsertTextFormat.Snippet,
            InsertText       = "default(${1:value})"
        };
    }

    private static IEnumerable<CompletionItem> BlockCompletions(string? block, string ext)
    {
        if (block is null) yield break;

        foreach (var kw in CompletionData.BlockKeywords(block))
            yield return Keyword(kw);

        // attribute-line snippet in field-bearing blocks
        if (block is "entity" or "value" or "shared" or "event" or "projection" or "variant"
                  or "input")
            yield return Snippet("field", "${1:FieldName}: ${2:text}", "FieldName: type");
    }

    // ── helpers ───────────────────────────────────────────────────────────────

    private static CompletionItem Keyword(string label) => new()
    {
        Label = label,
        Kind  = CompletionItemKind.Keyword
    };

    private static CompletionItem Snippet(string label, string insert, string detail) => new()
    {
        Label            = label,
        Kind             = CompletionItemKind.Snippet,
        Detail           = detail,
        InsertTextFormat = InsertTextFormat.Snippet,
        InsertText       = insert
    };

    private class Context
    {
        public Stack<string> BlockStack   { get; set; } = new();
        public string        CurrentLine  { get; set; } = "";
        public bool          IsTopLevel   { get; set; }
        public bool          IsAfterColon { get; set; }
        public bool          IsAfterType  { get; set; }
        public bool          IsAfterTransport { get; set; }
        public bool          IsAfterStreaming  { get; set; }
        public bool          IsAfterReturns   { get; set; }
        public string?       InnermostBlock   => BlockStack.Count > 0 ? BlockStack.Peek() : null;
    }
}
