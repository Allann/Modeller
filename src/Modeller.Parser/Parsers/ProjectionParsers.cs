using Modeller.Parser.Ast;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Modeller.Parser.Parsers;

/// <summary>
/// Parsers for projection definitions (query return shapes)
/// </summary>
public static class ProjectionParsers
{
    /// <summary>
    /// Parses a projection body line (attribute only)
    /// </summary>
    private static Parser<char, AttributeNode> ProjectionBodyLine { get; } =
        EntityParsers.Attribute;

    /// <summary>
    /// Parses a complete projection definition
    /// </summary>
    public static Parser<char, ProjectionNode> Projection { get; } =
        Map(
            (_, __, name, ___, desc, ____, attrs) =>
            {
                var attrList = attrs.ToList();
                return new ProjectionNode(
                    name,
                    desc.GetValueOrDefault(),
                    attrList.Count > 0 ? attrList : null);
            },
            TokenParsers.Keyword("projection"),
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.Identifier,
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.QuotedString.Optional(),
            TokenParsers.SkipWhitespaceAndComments,
            ProjectionBodyLine.Before(TokenParsers.SkipWhitespaceAndComments).Until(TokenParsers.End_)
        );
}

