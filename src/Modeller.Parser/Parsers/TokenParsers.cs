using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Modeller.Parser.Parsers;

/// <summary>
/// Core token parsers for the DSL
/// </summary>
public static class TokenParsers
{
    /// <summary>
    /// Matches a single line comment starting with #
    /// </summary>
    public static Parser<char, Unit> LineComment { get; } =
        Char('#')
            .Then(Token(c => c != '\n' && c != '\r').SkipMany())
            .Then(EndOfLine.IgnoreResult().Or(End))
            .Labelled("comment");

    /// <summary>
    /// Skips whitespace and comments
    /// </summary>
    public static Parser<char, Unit> SkipWhitespaceAndComments { get; } =
        SkipWhitespaces
            .Then(LineComment.Then(SkipWhitespaces).SkipMany());

    /// <summary>
    /// A valid identifier (starts with letter, contains letters/digits/underscores)
    /// </summary>
    public static Parser<char, string> Identifier { get; } =
        Token(c => char.IsLetter(c) || c == '_')
            .Then(Token(c => char.IsLetterOrDigit(c) || c == '_').ManyString(), (first, rest) => first + rest)
            .Labelled("identifier");

    /// <summary>
    /// A quoted string (double quotes)
    /// </summary>
    public static Parser<char, string> QuotedString { get; } =
        Token(c => c != '"')
            .ManyString()
            .Between(Char('"'), Char('"'))
            .Labelled("quoted string");

    /// <summary>
    /// An integer value
    /// </summary>
    public static Parser<char, int> Integer { get; } =
        Digit.AtLeastOnceString()
            .Select(int.Parse)
            .Labelled("integer");

    /// <summary>
    /// A boolean value (true/false)
    /// </summary>
    public static Parser<char, bool> Boolean { get; } =
        String("true").Select(_ => true)
            .Or(String("false").Select(_ => false))
            .Labelled("boolean");

    /// <summary>
    /// Creates a keyword parser that must be followed by whitespace or end
    /// </summary>
    public static Parser<char, string> Keyword(string keyword) =>
        Try(String(keyword)
            .Before(Lookahead(
                Token(c => !char.IsLetterOrDigit(c) && c != '_')
                    .IgnoreResult()
                    .Or(End))))
            .Select(_ => keyword)
            .Labelled($"keyword '{keyword}'");

    /// <summary>
    /// Parses 'end' keyword
    /// </summary>
    public static Parser<char, Unit> End_ { get; } =
        Keyword("end").IgnoreResult();

    /// <summary>
    /// Parses a comma separator with optional surrounding whitespace
    /// </summary>
    public static Parser<char, Unit> Comma { get; } =
        SkipWhitespaceAndComments
            .Then(Char(','))
            .Then(SkipWhitespaceAndComments);

    /// <summary>
    /// Parses a colon with optional surrounding whitespace
    /// </summary>
    public static Parser<char, Unit> Colon { get; } =
        SkipWhitespaceAndComments
            .Then(Char(':'))
            .Then(SkipWhitespaceAndComments);

    /// <summary>
    /// Parses content between parentheses
    /// </summary>
    public static Parser<char, T> InParens<T>(Parser<char, T> parser) =>
        parser.Between(
            Char('(').Before(SkipWhitespaceAndComments),
            SkipWhitespaceAndComments.Then(Char(')')));

    /// <summary>
    /// Parses content between square brackets
    /// </summary>
    public static Parser<char, T> InBrackets<T>(Parser<char, T> parser) =>
        parser.Between(
            Char('[').Before(SkipWhitespaceAndComments),
            SkipWhitespaceAndComments.Then(Char(']')));

    /// <summary>
    /// Parses a list of identifiers separated by commas in brackets: [A, B, C]
    /// </summary>
    public static Parser<char, IReadOnlyList<string>> IdentifierList { get; } =
        InBrackets(Identifier.Separated(Comma))
            .Select(x => (IReadOnlyList<string>)x.ToList());

}

