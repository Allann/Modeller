using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace Modeller.LanguageServer.Handlers;

public class TextDocumentSyncHandler(DocumentManager documentManager) : TextDocumentSyncHandlerBase
{
    private static readonly TextDocumentSelector Selector = LanguageIds.CreateSelector();

    protected override TextDocumentSyncRegistrationOptions CreateRegistrationOptions(
        TextSynchronizationCapability capability,
        ClientCapabilities clientCapabilities) => new()
    {
        DocumentSelector = Selector,
        Change = TextDocumentSyncKind.Full
    };

    public override TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri) =>
        new(uri, "modeller");

    public override Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken)
    {
        documentManager.Update(request.TextDocument.Uri, request.TextDocument.Text);
        return Unit.Task;
    }

    public override Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken cancellationToken)
    {
        var text = request.ContentChanges.FirstOrDefault()?.Text ?? "";
        documentManager.Update(request.TextDocument.Uri, text);
        return Unit.Task;
    }

    public override Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken) =>
        Unit.Task;

    public override Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken)
    {
        documentManager.Remove(request.TextDocument.Uri);
        return Unit.Task;
    }
}
