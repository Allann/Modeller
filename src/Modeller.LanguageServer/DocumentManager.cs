using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using LspRange = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Modeller.LanguageServer;

public class DocumentManager(ILanguageServerFacade server)
{
    private readonly Dictionary<DocumentUri, string> _documents = new();

    public void Update(DocumentUri uri, string content)
    {
        _documents[uri] = content;
        PublishDiagnostics(uri, content);
    }

    public void Remove(DocumentUri uri)
    {
        _documents.Remove(uri);
        server.TextDocument.PublishDiagnostics(new PublishDiagnosticsParams
        {
            Uri         = uri,
            Diagnostics = new Container<Diagnostic>()
        });
    }

    public string? GetContent(DocumentUri uri) =>
        _documents.TryGetValue(uri, out var content) ? content : null;

    private void PublishDiagnostics(DocumentUri uri, string content)
    {
        var ext    = Path.GetExtension(uri.Path);
        var result = DocumentParser.Parse(ext, content);

        var diagnostics = new List<Diagnostic>();
        if (!result.Success && result.Error is not null)
        {
            var line = Math.Max(0, (result.Line ?? 1) - 1);
            var col  = Math.Max(0, (result.Column ?? 1) - 1);
            diagnostics.Add(new Diagnostic
            {
                Severity = DiagnosticSeverity.Error,
                Range    = new LspRange(new Position(line, col), new Position(line, col + 1)),
                Message  = result.Error,
                Source   = "modeller"
            });
        }

        server.TextDocument.PublishDiagnostics(new PublishDiagnosticsParams
        {
            Uri         = uri,
            Diagnostics = new Container<Diagnostic>(diagnostics)
        });
    }
}
