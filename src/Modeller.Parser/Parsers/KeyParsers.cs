using Modeller.Parser.Ast;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Modeller.Parser.Parsers;

/// <summary>
/// Parsers for key definitions
/// </summary>
public static class KeyParsers
{
    /// <summary>
    /// Parses the generated modifier: , generated
    /// </summary>
    private static Parser<char, bool> GeneratedModifier { get; } =
        Try(TokenParsers.Comma.Then(TokenParsers.Keyword("generated")))
            .Optional()
            .Select(opt => opt.HasValue);

    /// <summary>
    /// Parses a key field like: EntityId: guid, generated  or  EntityId: id
    /// The 'id' type is shorthand for 'guid, generated'
    /// </summary>
    public static Parser<char, KeyFieldNode> KeyField { get; } =
        Try(
            Map(
                (name, dataType, generated) =>
                    dataType.TypeName == "id"
                        ? new KeyFieldNode(name, "guid", IsGenerated: true)
                        : new KeyFieldNode(name, dataType.TypeName, generated),
                TokenParsers.Identifier.Before(TokenParsers.Colon),
                DataTypeParsers.DataType,
                GeneratedModifier
            )
        );

    /// <summary>
    /// Parses an index like: index Name unique
    /// or: index [Service, Name] unique
    /// </summary>
    public static Parser<char, IndexNode> Index { get; } =
        Map(
            (_, __, fields, ___, unique) => new IndexNode(fields, unique.HasValue),
            TokenParsers.Keyword("index"),
            TokenParsers.SkipWhitespaceAndComments,
            Try(TokenParsers.IdentifierList)
                .Or(TokenParsers.Identifier.Select(id => (IReadOnlyList<string>)[id])),
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.Keyword("unique").Optional()
        );

    /// <summary>
    /// Parses a key body element (field or index)
    /// </summary>
    private static Parser<char, object> KeyBodyElement { get; } =
        Index.Select(i => (object)i)
            .Or(KeyField.Select(f => (object)f));

    /// <summary>
    /// Parses a complete key definition
    /// </summary>
    public static Parser<char, KeyNode> Key { get; } =
        Map(
            (_, __, entityName, ___, elements) =>
            {
                var fields = elements.OfType<KeyFieldNode>().ToList();
                var indexes = elements.OfType<IndexNode>().ToList();
                return new KeyNode(
                    entityName,
                    fields,
                    indexes.Count > 0 ? indexes : null);
            },
            TokenParsers.Keyword("key"),
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.Identifier,
            TokenParsers.SkipWhitespaceAndComments,
            KeyBodyElement.Before(TokenParsers.SkipWhitespaceAndComments).Until(TokenParsers.End_)
        );
}

