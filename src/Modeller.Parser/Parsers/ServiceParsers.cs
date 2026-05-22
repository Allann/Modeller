using Modeller.Parser.Ast;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Modeller.Parser.Parsers;

/// <summary>
/// Parsers for service definitions
/// </summary>
public static class ServiceParsers
{
    private static Parser<char, IReadOnlyList<string>> NameListBlock(string keyword) =>
        Map(
            (_, __, items) => (IReadOnlyList<string>)items.ToList(),
            TokenParsers.Keyword(keyword),
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.Identifier.Before(TokenParsers.SkipWhitespaceAndComments).Until(TokenParsers.End_)
        );

    private static Parser<char, IReadOnlyList<string>> EntitiesBlock { get; } = NameListBlock("entities");
    private static Parser<char, IReadOnlyList<string>> EnumsBlock { get; } = NameListBlock("enums");

    /// <summary>
    /// Parses a references block — per-consumer shared type names this service reads from other contexts
    /// </summary>
    private static Parser<char, IReadOnlyList<string>> ReferencesBlock { get; } = NameListBlock("references");

    /// <summary>
    /// Parses a calls block — RPC command names this service invokes on other services
    /// </summary>
    private static Parser<char, IReadOnlyList<string>> CallsBlock { get; } = NameListBlock("calls");

    /// <summary>
    /// Parses an implements block — RPC command names this service handles (is the provider for)
    /// </summary>
    private static Parser<char, IReadOnlyList<string>> ImplementsBlock { get; } = NameListBlock("implements");

    /// <summary>
    /// Parses a service body element
    /// </summary>
    private static Parser<char, object> ServiceBodyElement { get; } =
        Try(EntitiesBlock.Select(e => (object)("entities", e)))
            .Or(Try(EnumsBlock.Select(e => (object)("enums", e))))
            .Or(Try(ReferencesBlock.Select(r => (object)("references", r))))
            .Or(Try(CallsBlock.Select(c => (object)("calls", c))))
            .Or(Try(ImplementsBlock.Select(i => (object)("implements", i))));

    /// <summary>
    /// Parses a complete service definition
    /// </summary>
    public static Parser<char, ServiceNode> Service { get; } =
        Map(
            (_, __, name, ___, desc, ____, elements) =>
            {
                var lists = elements.OfType<(string, IReadOnlyList<string>)>().ToList();
                var entities   = lists.FirstOrDefault(e => e.Item1 == "entities").Item2;
                var enums      = lists.FirstOrDefault(e => e.Item1 == "enums").Item2;
                var refs       = lists.FirstOrDefault(e => e.Item1 == "references").Item2;
                var calls      = lists.FirstOrDefault(e => e.Item1 == "calls").Item2;
                var implements = lists.FirstOrDefault(e => e.Item1 == "implements").Item2;
                return new ServiceNode(
                    name,
                    desc.GetValueOrDefault(),
                    entities,
                    enums,
                    refs,
                    calls,
                    implements);
            },
            TokenParsers.Keyword("service"),
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.Identifier,
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.QuotedString.Optional(),
            TokenParsers.SkipWhitespaceAndComments,
            ServiceBodyElement.Before(TokenParsers.SkipWhitespaceAndComments).Until(TokenParsers.End_)
        );
}

