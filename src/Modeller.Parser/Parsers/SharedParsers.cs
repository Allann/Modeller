using Modeller.Parser.Ast;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Modeller.Parser.Parsers;

/// <summary>
/// Parsers for shared/lookup data definitions
/// </summary>
public static class SharedParsers
{
    /// <summary>
    /// Parses a shared data body line (attribute only)
    /// </summary>
    private static Parser<char, AttributeNode> SharedBodyLine { get; } =
        EntityParsers.Attribute;

    /// <summary>
    /// Parses a complete shared data definition
    /// </summary>
    public static Parser<char, SharedNode> Shared { get; } =
        Map(
            (_, __, name, ___, desc, ____, attrs) =>
            {
                var attrList = attrs.ToList();
                return new SharedNode(
                    name,
                    desc.GetValueOrDefault(),
                    attrList.Count > 0 ? attrList : null);
            },
            TokenParsers.Keyword("shared"),
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.Identifier,
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.QuotedString.Optional(),
            TokenParsers.SkipWhitespaceAndComments,
            SharedBodyLine.Before(TokenParsers.SkipWhitespaceAndComments).Until(TokenParsers.End_)
        );
}

