using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Modeller.Parser.Parsers;

/// <summary>
/// Parsed data type with optional parameters
/// </summary>
/// <param name="TypeName">The type name</param>
/// <param name="MaxLength">Optional max length for text</param>
/// <param name="Precision">Optional precision for decimal types</param>
/// <param name="Scale">Optional scale for decimal types</param>
public sealed record ParsedDataType(
    string TypeName,
    int? MaxLength = null,
    int? Precision = null,
    int? Scale = null);

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
        "binary", "id", "email", "url",
        // standard library value types
        "money", "percentage", "geospatial",
        // standard library union types
        "image", "document"
    ];

    /// <summary>
    /// Parses 'max' as a special length indicator
    /// </summary>
    private static Parser<char, int?> MaxLength { get; } =
        Try(TokenParsers.Keyword("max").Select(_ => (int?)int.MaxValue))
            .Or(TokenParsers.Integer.Select(i => (int?)i));

    /// <summary>
    /// Parses decimal precision/scale like: decimal(18,2)
    /// </summary>
    private static Parser<char, (int Precision, int Scale)> DecimalPrecisionScale { get; } =
        Map(
            (precision, _, scale) => (precision, scale),
            TokenParsers.Integer,
            TokenParsers.Comma,
            TokenParsers.Integer);

    /// <summary>
    /// Parses a data type like: text, text(100), text(max), decimal(18,2), integer, CustomType
    /// </summary>
    public static Parser<char, ParsedDataType> DataType { get; } =
        TokenParsers.Identifier.SelectMany(
            name =>
            {
                var lengthParser = TokenParsers.InParens(MaxLength).Optional();

                if (name.Equals("decimal", StringComparison.OrdinalIgnoreCase))
                {
                    return Try(TokenParsers.InParens(DecimalPrecisionScale))
                        .Select(ps => new ParsedDataType(
                            name,
                            MaxLength: null,
                            Precision: ps.Precision,
                            Scale: ps.Scale))
                        .Or(lengthParser.Select(length => new ParsedDataType(name, MaxLength: length.GetValueOrDefault())));
                }

                return lengthParser.Select(length => new ParsedDataType(name, MaxLength: length.GetValueOrDefault()));
            },
            (_, parsed) => parsed);
}

