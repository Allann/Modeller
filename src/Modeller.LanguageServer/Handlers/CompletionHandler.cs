using Modeller.LanguageServer.Completions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Modeller.LanguageServer.Handlers;

public class CompletionHandler(DocumentManager documentManager) : CompletionHandlerBase
{
    private static readonly TextDocumentSelector Selector = LanguageIds.CreateSelector();

    protected override CompletionRegistrationOptions CreateRegistrationOptions(
        CompletionCapability capability,
        ClientCapabilities clientCapabilities) => new()
    {
        DocumentSelector   = Selector,
        TriggerCharacters  = new Container<string>(":", " ")
    };

    public override Task<CompletionList> Handle(CompletionParams request, CancellationToken cancellationToken)
    {
        var uri     = request.TextDocument.Uri;
        var content = documentManager.GetContent(uri) ?? "";
        var ext     = Path.GetExtension(uri.Path);
        var items   = CompletionProvider.GetCompletions(content, request.Position, ext);

        return Task.FromResult(new CompletionList(items));
    }

    public override Task<CompletionItem> Handle(CompletionItem request, CancellationToken cancellationToken) =>
        Task.FromResult(request);
}
