using Modeller.Parser.Ast;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Modeller.Parser.Parsers;

/// <summary>
/// Parsers for value object definitions
/// </summary>
public static class ValueParsers
{
    /// <summary>
    /// Parses a value object body line (attribute only, no relationships)
    /// </summary>
    private static Parser<char, AttributeNode> ValueBodyLine { get; } =
        EntityParsers.Attribute;

    /// <summary>
    /// Parses a complete value object definition
    /// </summary>
    public static Parser<char, ValueNode> Value { get; } =
        Map(
            (_, __, name, ___, desc, ____, attrs) =>
            {
                var attrList = attrs.ToList();
                return new ValueNode(
                    name,
                    desc.GetValueOrDefault(),
                    attrList.Count > 0 ? attrList : null);
            },
            TokenParsers.Keyword("value"),
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.Identifier,
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.QuotedString.Optional(),
            TokenParsers.SkipWhitespaceAndComments,
            ValueBodyLine.Before(TokenParsers.SkipWhitespaceAndComments).Until(TokenParsers.End_)
        );
}

