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
    /// <summary>
    /// Parses an entities block like: entities\n  Entity1\n  Entity2\nend
    /// </summary>
    private static Parser<char, IReadOnlyList<string>> EntitiesBlock { get; } =
        Map(
            (_, __, entities) => (IReadOnlyList<string>)entities.ToList(),
            TokenParsers.Keyword("entities"),
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.Identifier.Before(TokenParsers.SkipWhitespaceAndComments).Until(TokenParsers.End_)
        );

    /// <summary>
    /// Parses an enums block like: enums\n  Enum1\n  Enum2\nend
    /// </summary>
    private static Parser<char, IReadOnlyList<string>> EnumsBlock { get; } =
        Map(
            (_, __, enums) => (IReadOnlyList<string>)enums.ToList(),
            TokenParsers.Keyword("enums"),
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.Identifier.Before(TokenParsers.SkipWhitespaceAndComments).Until(TokenParsers.End_)
        );

    /// <summary>
    /// Parses a reference like: Definition: [Domain, Entity, Enum]
    /// </summary>
    private static Parser<char, ReferenceNode> Reference { get; } =
        Map(
            (service, entities) => new ReferenceNode(service, entities),
            TokenParsers.Identifier.Before(TokenParsers.Colon),
            TokenParsers.IdentifierList
        );

    /// <summary>
    /// Parses a references block
    /// </summary>
    private static Parser<char, IReadOnlyList<ReferenceNode>> ReferencesBlock { get; } =
        Map(
            (_, __, refs) => (IReadOnlyList<ReferenceNode>)refs.ToList(),
            TokenParsers.Keyword("references"),
            TokenParsers.SkipWhitespaceAndComments,
            Reference.Before(TokenParsers.SkipWhitespaceAndComments).Until(TokenParsers.End_)
        );

    /// <summary>
    /// Parses a service body element
    /// </summary>
    private static Parser<char, object> ServiceBodyElement { get; } =
        Try(EntitiesBlock.Select(e => (object)("entities", e)))
            .Or(Try(EnumsBlock.Select(e => (object)("enums", e))))
            .Or(ReferencesBlock.Select(r => (object)("references", r)));

    /// <summary>
    /// Parses a complete service definition
    /// </summary>
    public static Parser<char, ServiceNode> Service { get; } =
        Map(
            (_, __, name, ___, desc, ____, elements) =>
            {
                var entities = elements.OfType<(string, IReadOnlyList<string>)>().FirstOrDefault(e => e.Item1 == "entities").Item2;
                var enums = elements.OfType<(string, IReadOnlyList<string>)>().FirstOrDefault(e => e.Item1 == "enums").Item2;
                var refs = elements.OfType<(string, IReadOnlyList<ReferenceNode>)>().FirstOrDefault(e => e.Item1 == "references").Item2;
                return new ServiceNode(
                    name,
                    desc.GetValueOrDefault(),
                    entities,
                    enums,
                    refs);
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

