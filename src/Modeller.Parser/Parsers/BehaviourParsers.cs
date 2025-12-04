using Modeller.Parser.Ast;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Modeller.Parser.Parsers;

/// <summary>
/// Parsers for command and query definitions
/// </summary>
public static class BehaviourParsers
{
    /// <summary>
    /// Parses an input block
    /// </summary>
    private static Parser<char, IReadOnlyList<AttributeNode>> InputBlock { get; } =
        Map(
            (_, __, attrs) => (IReadOnlyList<AttributeNode>)attrs.ToList(),
            TokenParsers.Keyword("input"),
            TokenParsers.SkipWhitespaceAndComments,
            EntityParsers.Attribute.Before(TokenParsers.SkipWhitespaceAndComments).Until(TokenParsers.End_)
        );

    /// <summary>
    /// Parses an output block like: output\n  Domain "description"\nend
    /// </summary>
    private static Parser<char, (string Type, string? Description)> OutputBlockWithEnd { get; } =
        Try(
            Map(
                (_, __, type, ___, desc, ____, _____) => (type, (string?)desc.GetValueOrDefault()),
                TokenParsers.Keyword("output"),
                TokenParsers.SkipWhitespaceAndComments,
                TokenParsers.Identifier,
                TokenParsers.SkipWhitespaceAndComments,
                TokenParsers.QuotedString.Optional(),
                TokenParsers.SkipWhitespaceAndComments,
                TokenParsers.End_
            )
        );

    /// <summary>
    /// Parses a simple output declaration like: output Domain
    /// </summary>
    private static Parser<char, (string Type, string? Description)> OutputSimple { get; } =
        Map(
            (_, __, type) => (type, (string?)null),
            TokenParsers.Keyword("output"),
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.Identifier
        );

    /// <summary>
    /// Parses either output format
    /// </summary>
    private static Parser<char, (string Type, string? Description)> OutputDeclaration { get; } =
        OutputBlockWithEnd.Or(OutputSimple);

    /// <summary>
    /// Parses an error like: FileNotFound "Description"
    /// </summary>
    private static Parser<char, ErrorNode> Error { get; } =
        Map(
            (name, _, desc) => new ErrorNode(name, desc.GetValueOrDefault()),
            TokenParsers.Identifier,
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.QuotedString.Optional()
        );

    /// <summary>
    /// Parses an errors block
    /// </summary>
    private static Parser<char, IReadOnlyList<ErrorNode>> ErrorsBlock { get; } =
        Map(
            (_, __, errors) => (IReadOnlyList<ErrorNode>)errors.ToList(),
            TokenParsers.Keyword("errors"),
            TokenParsers.SkipWhitespaceAndComments,
            Error.Before(TokenParsers.SkipWhitespaceAndComments).Until(TokenParsers.End_)
        );

    /// <summary>
    /// Parses a publishes block
    /// </summary>
    private static Parser<char, IReadOnlyList<string>> PublishesBlock { get; } =
        Map(
            (_, __, events) => (IReadOnlyList<string>)events.ToList(),
            TokenParsers.Keyword("publishes"),
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.Identifier.Before(TokenParsers.SkipWhitespaceAndComments).Until(TokenParsers.End_)
        );

    /// <summary>
    /// Parses a command body element
    /// </summary>
    private static Parser<char, object> CommandBodyElement { get; } =
        Try(InputBlock.Select(i => (object)("input", i)))
            .Or(Try(OutputDeclaration.Select(o => (object)("output", o))))
            .Or(Try(ErrorsBlock.Select(e => (object)("errors", e))))
            .Or(PublishesBlock.Select(p => (object)("publishes", p)));

    /// <summary>
    /// Parses a command definition
    /// </summary>
    public static Parser<char, CommandNode> Command { get; } =
        Map(
            (_, __, name, ___, desc, ____, elements) =>
            {
                var inputs = elements.OfType<(string, IReadOnlyList<AttributeNode>)>().FirstOrDefault(e => e.Item1 == "input").Item2;
                var outputBlock = elements.OfType<(string, (string Type, string? Description))>().FirstOrDefault(e => e.Item1 == "output").Item2;
                var errors = elements.OfType<(string, IReadOnlyList<ErrorNode>)>().FirstOrDefault(e => e.Item1 == "errors").Item2;
                var events = elements.OfType<(string, IReadOnlyList<string>)>().FirstOrDefault(e => e.Item1 == "publishes").Item2;
                return new CommandNode(name, desc.GetValueOrDefault(), inputs, outputBlock.Type, errors, events);
            },
            TokenParsers.Keyword("command"),
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.Identifier,
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.QuotedString.Optional(),
            TokenParsers.SkipWhitespaceAndComments,
            CommandBodyElement.Before(TokenParsers.SkipWhitespaceAndComments).Until(TokenParsers.End_)
        );

    /// <summary>
    /// Parses returns Type or returns many Type
    /// </summary>
    private static Parser<char, (string Type, bool Many)> ReturnsDeclaration { get; } =
        Map(
            (_, __, many, type) => (type, many.HasValue),
            TokenParsers.Keyword("returns"),
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.Keyword("many").Before(TokenParsers.SkipWhitespaceAndComments).Optional(),
            TokenParsers.Identifier
        );

    /// <summary>
    /// Parses a query body element
    /// </summary>
    private static Parser<char, object> QueryBodyElement { get; } =
        Try(InputBlock.Select(i => (object)("input", i)))
            .Or(ReturnsDeclaration.Select(r => (object)("returns", r)));

    /// <summary>
    /// Parses a query definition
    /// </summary>
    public static Parser<char, QueryNode> Query { get; } =
        Map(
            (_, __, name, ___, desc, ____, elements) =>
            {
                var inputs = elements.OfType<(string, IReadOnlyList<AttributeNode>)>().FirstOrDefault(e => e.Item1 == "input").Item2;
                var returns = elements.OfType<(string, (string Type, bool Many))>().FirstOrDefault(e => e.Item1 == "returns").Item2;
                return new QueryNode(
                    name,
                    desc.GetValueOrDefault(),
                    inputs,
                    returns.Type,
                    returns.Many);
            },
            TokenParsers.Keyword("query"),
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.Identifier,
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.QuotedString.Optional(),
            TokenParsers.SkipWhitespaceAndComments,
            QueryBodyElement.Before(TokenParsers.SkipWhitespaceAndComments).Until(TokenParsers.End_)
        );
}

