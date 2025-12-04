using Modeller.Parser.Ast;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Modeller.Parser.Parsers;

/// <summary>
/// Parsers for enum and flags definitions
/// </summary>
public static class EnumParsers
{
    /// <summary>
    /// Parses an enum value line like: Active: 1 "Description"
    /// </summary>
    public static Parser<char, EnumValueNode> EnumValue { get; } =
        Map(
            (name, value, _, desc) => new EnumValueNode(name, value, desc.GetValueOrDefault()),
            TokenParsers.Identifier.Before(TokenParsers.Colon),
            TokenParsers.Integer,
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.QuotedString.Optional()
        );

    /// <summary>
    /// Parses an enum definition
    /// </summary>
    public static Parser<char, EnumNode> Enum { get; } =
        Map(
            (_, __, name, ___, desc, ____, values) =>
            {
                var valueList = values.ToList();
                return new EnumNode(
                    name,
                    desc.GetValueOrDefault(),
                    valueList.Count > 0 ? valueList : null);
            },
            TokenParsers.Keyword("enum"),
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.Identifier,
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.QuotedString.Optional(),
            TokenParsers.SkipWhitespaceAndComments,
            EnumValue.Before(TokenParsers.SkipWhitespaceAndComments).Until(TokenParsers.End_)
        );

    /// <summary>
    /// Parses a flags definition (same structure as enum but with 'flags' keyword)
    /// </summary>
    public static Parser<char, EnumNode> Flags { get; } =
        Map(
            (_, __, name, ___, desc, ____, values) =>
            {
                var valueList = values.ToList();
                return new EnumNode(
                    name,
                    desc.GetValueOrDefault(),
                    valueList.Count > 0 ? valueList : null);
            },
            TokenParsers.Keyword("flags"),
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.Identifier,
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.QuotedString.Optional(),
            TokenParsers.SkipWhitespaceAndComments,
            EnumValue.Before(TokenParsers.SkipWhitespaceAndComments).Until(TokenParsers.End_)
        );

    /// <summary>
    /// Parses either an enum or flags definition
    /// </summary>
    public static Parser<char, EnumNode> EnumOrFlags { get; } =
        Try(Enum).Or(Flags);
}

