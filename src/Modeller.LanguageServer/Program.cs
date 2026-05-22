using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Modeller.LanguageServer;
using Modeller.LanguageServer.Handlers;
using OmniSharp.Extensions.LanguageServer.Server;

// Redirect our own stderr so it doesn't pollute the LSP stdio channel
Console.InputEncoding  = System.Text.Encoding.UTF8;
Console.OutputEncoding = System.Text.Encoding.UTF8;

var server = await LanguageServer.From(options =>
    options
        .WithInput(Console.OpenStandardInput())
        .WithOutput(Console.OpenStandardOutput())
        .WithHandler<TextDocumentSyncHandler>()
        .WithHandler<CompletionHandler>()
        .WithHandler<HoverHandler>()
        .WithServices(services =>
        {
            services.AddSingleton<DocumentManager>();
        })
        .WithLoggerFactory(LoggerFactory.Create(builder =>
            builder
                .AddFilter("OmniSharp", LogLevel.Warning)
                .AddFilter("Microsoft", LogLevel.Warning)))
);

await server.WaitForExit;
