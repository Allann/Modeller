namespace Modeller.LanguageServer.Completions;

public static class CompletionData
{
    public static readonly string[] PrimitiveTypes =
    [
        "text", "integer", "long", "decimal", "boolean",
        "date", "time", "datetime", "guid", "name",
        "binary", "id", "email", "url",
        "money", "percentage", "geospatial", "image", "document"
    ];

    public static readonly string[] TransportValues = ["http", "grpc"];

    public static readonly string[] StreamingValues = ["none", "server", "client", "bidirectional"];

    // (label, detail, insert-snippet)
    public static IEnumerable<(string Label, string Detail, string Snippet)> TopLevelSnippets(string ext) =>
        ext.ToLowerInvariant() switch
        {
            ".def" =>
            [
                ("domain", "Domain definition",
                    "domain ${1:Name}\n\t\"${2:Description}\"\n\n\tcompany \"${3:Company}\"\n\tversion \"${4:1.0.0}\"\n\n\tservices\n\t\t${5:ServiceName}\n\tend\nend")
            ],
            ".entity" =>
            [
                ("entity", "Entity definition",
                    "entity ${1:Name}\n\t\"${2:Description}\"\n\n\t${3:Field}: ${4:text}\nend")
            ],
            ".key" =>
            [
                ("key", "Key definition",
                    "key ${1:EntityName}\n\tId: id\nend")
            ],
            ".enum" =>
            [
                ("enum", "Enum definition",
                    "enum ${1:Name}\n\t\"${2:Description}\"\n\n\t${3:Value}: 1\nend")
            ],
            ".flags" =>
            [
                ("flags", "Flags definition",
                    "flags ${1:Name}\n\t\"${2:Description}\"\n\n\t${3:Value}: 1\nend")
            ],
            ".service" =>
            [
                ("service", "Service definition",
                    "service ${1:Name}\n\t\"${2:Description}\"\n\n\tentities\n\t\t${3:Entity}\n\tend\nend")
            ],
            ".command" =>
            [
                ("command", "Command definition",
                    "command ${1:Name}\n\t\"${2:Description}\"\n\n\tinput\n\t\t${3:Field}: ${4:text}\n\tend\nend")
            ],
            ".query" =>
            [
                ("query", "Query definition",
                    "query ${1:Name}\n\t\"${2:Description}\"\n\n\tinput\n\t\t${3:Field}: ${4:text}\n\tend\n\n\treturns ${5:ResultType}\nend")
            ],
            ".value" =>
            [
                ("value", "Value object definition",
                    "value ${1:Name}\n\t\"${2:Description}\"\n\n\t${3:Field}: ${4:text}\nend")
            ],
            ".shared" =>
            [
                ("shared", "Shared data definition",
                    "shared ${1:Name}\n\t\"${2:Description}\"\n\n\t${3:Field}: ${4:text}\nend")
            ],
            ".event" =>
            [
                ("event", "Domain event definition",
                    "event ${1:Name}\n\t\"${2:Description}\"\n\n\t${3:Field}: ${4:guid}\nend")
            ],
            ".projection" =>
            [
                ("projection", "Projection definition",
                    "projection ${1:Name}\n\t\"${2:Description}\"\n\n\t${3:Field}: ${4:text}\nend")
            ],
            ".union" =>
            [
                ("union", "Discriminated union definition",
                    "union ${1:Name}\n\t\"${2:Description}\"\n\n\tvariant ${3:Variant}\n\t\t${4:Field}: ${5:text}\n\tend\nend")
            ],
            _ => []
        };

    public static IEnumerable<string> BlockKeywords(string block) =>
        block switch
        {
            "entity"     => ["has_one", "has_many", "belongs_to", "many_to_many", "references", "end"],
            "value"      => ["end"],
            "shared"     => ["end"],
            "event"      => ["end"],
            "projection" => ["end"],
            "key"        => ["index", "end"],
            "enum"       => ["end"],
            "flags"      => ["end"],
            "service"    => ["entities", "enums", "references", "calls", "implements", "end"],
            "command"    => ["input", "output", "errors", "publishes", "transport", "streaming", "end"],
            "query"      => ["input", "returns", "transport", "streaming", "end"],
            "domain"     => ["company", "version", "services", "end"],
            "union"      => ["variant", "end"],
            // sub-blocks
            "input" or "errors" or "publishes"
                or "entities" or "enums" or "services"
                or "references" or "calls" or "implements" => ["end"],
            "variant"    => ["end"],
            _            => ["end"]
        };
}
