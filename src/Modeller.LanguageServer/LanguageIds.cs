using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Modeller.LanguageServer;

public static class LanguageIds
{
    public static readonly string[] All =
    [
        "modeller",
        "modeller-def", "modeller-entity", "modeller-key", "modeller-enum",
        "modeller-flags", "modeller-service", "modeller-command", "modeller-query",
        "modeller-value", "modeller-shared", "modeller-event", "modeller-projection",
        "modeller-union"
    ];

    public static TextDocumentSelector CreateSelector() =>
        new(All.Select(lang => new TextDocumentFilter { Language = lang }).ToArray());
}
