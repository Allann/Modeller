namespace Modeller.Parser.Ast;

/// <summary>
/// A command definition - a write operation
/// </summary>
/// <param name="Name">The command name</param>
/// <param name="Description">Optional description</param>
/// <param name="Inputs">Input parameters</param>
/// <param name="Output">Output type</param>
/// <param name="Errors">Possible error types</param>
/// <param name="Events">Events published</param>
/// <param name="Transport">Wire protocol (default: Http)</param>
/// <param name="Streaming">Streaming mode (default: None)</param>
/// <param name="Span">Source location</param>
public sealed record CommandNode(
    string Name,
    string? Description = null,
    IReadOnlyList<AttributeNode>? Inputs = null,
    string? Output = null,
    IReadOnlyList<ErrorNode>? Errors = null,
    IReadOnlyList<string>? Events = null,
    TransportType Transport = TransportType.Http,
    StreamingMode Streaming = StreamingMode.None,
    SourceSpan Span = default) : AstNode(Span);

/// <summary>
/// A query definition - a read operation
/// </summary>
/// <param name="Name">The query name</param>
/// <param name="Description">Optional description</param>
/// <param name="Inputs">Input parameters</param>
/// <param name="Returns">Return type</param>
/// <param name="ReturnsMany">Whether returns a collection</param>
/// <param name="Transport">Wire protocol (default: Http)</param>
/// <param name="Streaming">Streaming mode (default: None)</param>
/// <param name="Span">Source location</param>
public sealed record QueryNode(
    string Name,
    string? Description = null,
    IReadOnlyList<AttributeNode>? Inputs = null,
    string? Returns = null,
    bool ReturnsMany = false,
    TransportType Transport = TransportType.Http,
    StreamingMode Streaming = StreamingMode.None,
    SourceSpan Span = default) : AstNode(Span);

/// <summary>
/// An error that a command can produce
/// </summary>
/// <param name="Name">The error name</param>
/// <param name="Description">Optional description</param>
/// <param name="Span">Source location</param>
public sealed record ErrorNode(
    string Name,
    string? Description = null,
    SourceSpan Span = default) : AstNode(Span);

