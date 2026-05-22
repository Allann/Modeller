using Modeller.Parser.Ast;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Modeller.Parser.Parsers;

/// <summary>
/// Parsers for discriminated union type definitions
/// </summary>
public static class UnionParsers
{
    /// <summary>
    /// Parses a single variant block within a union
    /// </summary>
    private static Parser<char, UnionVariantNode> Variant { get; } =
        Map(
            (_, __, name, ___, desc, ____, attrs) =>
            {
                var attrList = attrs.ToList();
                return new UnionVariantNode(
                    name,
                    desc.GetValueOrDefault(),
                    attrList.Count > 0 ? attrList : null);
            },
            TokenParsers.Keyword("variant"),
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.Identifier,
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.QuotedString.Optional(),
            TokenParsers.SkipWhitespaceAndComments,
            EntityParsers.Attribute.Before(TokenParsers.SkipWhitespaceAndComments).Until(TokenParsers.End_)
        );

    /// <summary>
    /// Parses a complete union type definition
    /// </summary>
    public static Parser<char, UnionNode> Union { get; } =
        Map(
            (_, __, name, ___, desc, ____, variants) =>
            {
                var variantList = variants.ToList();
                return new UnionNode(
                    name,
                    desc.GetValueOrDefault(),
                    variantList.Count > 0 ? variantList : null);
            },
            TokenParsers.Keyword("union"),
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.Identifier,
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.QuotedString.Optional(),
            TokenParsers.SkipWhitespaceAndComments,
            Variant.Before(TokenParsers.SkipWhitespaceAndComments).Until(TokenParsers.End_)
        );
}
