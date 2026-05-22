using Modeller.Parser;
using Modeller.Parser.Ast;

namespace Modeller.LanguageServer;

public static class DocumentParser
{
    public record DocumentParseResult(bool Success, string? Error, int? Line, int? Column);

    public static DocumentParseResult Parse(string fileExtension, string content)
    {
        return fileExtension.ToLowerInvariant() switch
        {
            ".def"        => From(DslParser.ParseDomain(content)),
            ".entity"     => From(DslParser.ParseEntityFile(content)),
            ".service"    => From(DslParser.ParseService(content)),
            ".key"        => From(DslParser.ParseKey(content)),
            ".enum"       => From(DslParser.ParseEnum(content)),
            ".flags"      => From(DslParser.ParseEnum(content)),
            ".command"    => From(DslParser.ParseCommand(content)),
            ".query"      => From(DslParser.ParseQuery(content)),
            ".value"      => From(DslParser.ParseValue(content)),
            ".shared"     => From(DslParser.ParseShared(content)),
            ".event"      => From(DslParser.ParseEvent(content)),
            ".projection" => From(DslParser.ParseProjection(content)),
            ".union"      => From(DslParser.ParseUnion(content)),
            _             => new DocumentParseResult(true, null, null, null)
        };
    }

    private static DocumentParseResult From<T>(ParseResult<T> r) where T : AstNode =>
        new(r.Success, r.Error, r.ErrorLine, r.ErrorColumn);
}
