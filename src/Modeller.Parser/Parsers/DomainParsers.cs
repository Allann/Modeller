using Modeller.Parser.Ast;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Modeller.Parser.Parsers;

/// <summary>
/// Parsers for domain definitions
/// </summary>
public static class DomainParsers
{
    /// <summary>
    /// Parses company "Name"
    /// </summary>
    private static Parser<char, string> CompanyProperty { get; } =
        Map(
            (_, __, value) => value,
            TokenParsers.Keyword("company"),
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.QuotedString
        );

    /// <summary>
    /// Parses version "1.0.0"
    /// </summary>
    private static Parser<char, string> VersionProperty { get; } =
        Map(
            (_, __, value) => value,
            TokenParsers.Keyword("version"),
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.QuotedString
        );

    /// <summary>
    /// Parses a services block like: services\n  Service1\n  Service2\nend
    /// </summary>
    private static Parser<char, IReadOnlyList<string>> ServicesBlock { get; } =
        Map(
            (_, __, services) => (IReadOnlyList<string>)services.ToList(),
            TokenParsers.Keyword("services"),
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.Identifier.Before(TokenParsers.SkipWhitespaceAndComments).Until(TokenParsers.End_)
        );

    /// <summary>
    /// Parses a domain property (company, version, or services block)
    /// </summary>
    private static Parser<char, object> DomainProperty { get; } =
        Try(CompanyProperty.Select(c => (object)("company", c)))
            .Or(Try(VersionProperty.Select(v => (object)("version", v))))
            .Or(ServicesBlock.Select(s => (object)("services", s)));

    /// <summary>
    /// Parses a complete domain definition
    /// </summary>
    public static Parser<char, DomainNode> Domain { get; } =
        Map(
            (_, __, name, ___, desc, ____, props) =>
            {
                var company = props.OfType<(string, string)>().FirstOrDefault(p => p.Item1 == "company").Item2;
                var version = props.OfType<(string, string)>().FirstOrDefault(p => p.Item1 == "version").Item2;
                var services = props.OfType<(string, IReadOnlyList<string>)>().FirstOrDefault(p => p.Item1 == "services").Item2;
                return new DomainNode(
                    name,
                    desc.GetValueOrDefault(),
                    company,
                    version,
                    services);
            },
            TokenParsers.Keyword("domain"),
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.Identifier,
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.QuotedString.Optional(),
            TokenParsers.SkipWhitespaceAndComments,
            DomainProperty.Before(TokenParsers.SkipWhitespaceAndComments).Until(TokenParsers.End_)
        );
}

