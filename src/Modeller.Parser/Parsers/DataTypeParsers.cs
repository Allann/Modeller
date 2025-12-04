using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Modeller.Parser.Parsers;

/// <summary>
/// Parsed data type with optional parameters
/// </summary>
/// <param name="TypeName">The type name</param>
/// <param name="MaxLength">Optional max length for text</param>
public sealed record ParsedDataType(string TypeName, int? MaxLength = null);

/// <summary>
/// Parsers for data types
/// </summary>
public static class DataTypeParsers
{
    /// <summary>
    /// Built-in primitive types
    /// </summary>
    private static readonly string[] PrimitiveTypes =
    [
        "text", "integer", "long", "decimal", "boolean",
        "date", "time", "datetime", "guid", "name",
        "image", "email", "url"
    ];

    /// <summary>
    /// Parses 'max' as a special length indicator
    /// </summary>
    private static Parser<char, int?> MaxLength { get; } =
        Try(TokenParsers.Keyword("max").Select(_ => (int?)int.MaxValue))
            .Or(TokenParsers.Integer.Select(i => (int?)i));

    /// <summary>
    /// Parses a data type like: text, text(100), text(max), integer, CustomType
    /// </summary>
    public static Parser<char, ParsedDataType> DataType { get; } =
        TokenParsers.Identifier
            .Then(
                TokenParsers.InParens(MaxLength).Optional(),
                (name, length) => new ParsedDataType(name, length.GetValueOrDefault()));
}

