using Modeller.Parser.Ast;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Modeller.Parser.Parsers;

/// <summary>
/// Parsers for domain event definitions
/// </summary>
public static class EventParsers
{
    /// <summary>
    /// Parses an event body line (attribute only)
    /// </summary>
    private static Parser<char, AttributeNode> EventBodyLine { get; } =
        EntityParsers.Attribute;

    /// <summary>
    /// Parses a complete event definition
    /// </summary>
    public static Parser<char, EventNode> Event { get; } =
        Map(
            (_, __, name, ___, desc, ____, attrs) =>
            {
                var attrList = attrs.ToList();
                return new EventNode(
                    name,
                    desc.GetValueOrDefault(),
                    attrList.Count > 0 ? attrList : null);
            },
            TokenParsers.Keyword("event"),
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.Identifier,
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.QuotedString.Optional(),
            TokenParsers.SkipWhitespaceAndComments,
            EventBodyLine.Before(TokenParsers.SkipWhitespaceAndComments).Until(TokenParsers.End_)
        );
}

