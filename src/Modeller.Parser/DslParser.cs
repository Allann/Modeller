using Modeller.Parser.Ast;
using Modeller.Parser.Parsers;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Modeller.Parser;

/// <summary>
/// Result of a parse operation
/// </summary>
/// <typeparam name="T">The AST node type</typeparam>
public readonly record struct ParseResult<T>(
    bool Success,
    T? Value,
    string? Error,
    int? ErrorLine,
    int? ErrorColumn) where T : AstNode
{
    public static ParseResult<T> Ok(T value) => new(true, value, null, null, null);
    
    public static ParseResult<T> Fail(string error, int? line = null, int? column = null) =>
        new(false, default, error, line, column);
}

/// <summary>
/// Main DSL parser - parses definition files into AST nodes
/// </summary>
public static class DslParser
{
    /// <summary>
    /// Parses a domain definition file
    /// </summary>
    public static ParseResult<DomainNode> ParseDomain(string input) =>
        Parse(input, DomainParsers.Domain);

    /// <summary>
    /// Parses a service definition file
    /// </summary>
    public static ParseResult<ServiceNode> ParseService(string input) =>
        Parse(input, ServiceParsers.Service);

    /// <summary>
    /// Parses an entity definition (entity block only)
    /// </summary>
    public static ParseResult<EntityNode> ParseEntity(string input) =>
        Parse(input, EntityParsers.Entity);

    /// <summary>
    /// Parses an entity file (entity + optional key)
    /// </summary>
    public static ParseResult<EntityNode> ParseEntityFile(string input) =>
        Parse(input, EntityParsers.EntityFile);

    /// <summary>
    /// Parses a key definition
    /// </summary>
    public static ParseResult<KeyNode> ParseKey(string input) =>
        Parse(input, KeyParsers.Key);

    /// <summary>
    /// Parses an enum definition file
    /// </summary>
    public static ParseResult<EnumNode> ParseEnum(string input) =>
        Parse(input, EnumParsers.EnumOrFlags);

    /// <summary>
    /// Parses a command definition file
    /// </summary>
    public static ParseResult<CommandNode> ParseCommand(string input) =>
        Parse(input, BehaviourParsers.Command);

    /// <summary>
    /// Parses a query definition file
    /// </summary>
    public static ParseResult<QueryNode> ParseQuery(string input) =>
        Parse(input, BehaviourParsers.Query);

    /// <summary>
    /// Core parse function
    /// </summary>
    private static ParseResult<T> Parse<T>(string input, Parser<char, T> parser) where T : AstNode
    {
        var fullParser = TokenParsers.SkipWhitespaceAndComments
            .Then(parser)
            .Before(TokenParsers.SkipWhitespaceAndComments)
            .Before(End);

        var result = fullParser.Parse(input);

        if (result.Success)
        {
            return ParseResult<T>.Ok(result.Value);
        }

        var error = result.Error;
        var errorMessage = FormatError(error, input);
        return ParseResult<T>.Fail(
            errorMessage,
            error?.ErrorPos.Line,
            error?.ErrorPos.Col);
    }

    private static string FormatError(ParseError<char>? error, string input)
    {
        if (error is null) return "Unknown parse error";

        var pos = error.ErrorPos;
        var lines = input.Split('\n');
        var lineIndex = pos.Line - 1;
        var context = lineIndex >= 0 && lineIndex < lines.Length
            ? $" near: '{lines[lineIndex].Trim()}'"
            : "";

        var expected = error.Expected?.Any() == true
            ? $"Expected: {string.Join(" or ", error.Expected.Select(e => e.ToString()))}"
            : "";

        var unexpected = error.Unexpected.HasValue
            ? $"Unexpected: '{error.Unexpected.Value}'"
            : "";

        return $"Parse error at line {pos.Line}, col {pos.Col}.{context} {expected} {unexpected}".Trim();
    }
}

