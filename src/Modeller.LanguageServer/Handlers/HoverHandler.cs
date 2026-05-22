using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Modeller.LanguageServer.Handlers;

public class HoverHandler(DocumentManager documentManager) : HoverHandlerBase
{
    private static readonly TextDocumentSelector Selector = LanguageIds.CreateSelector();

    private sealed record HoverDoc(string Summary, string? Syntax = null);

    private static readonly Dictionary<string, HoverDoc> Docs = new(StringComparer.OrdinalIgnoreCase)
    {
        ["domain"]         = new("Root aggregate for a bounded context.", "domain Sales\n  \"Domain description\"\n\n  company \"Contoso\"\n  version \"1.0.0\"\n\n  services\n    Orders\n    Customers\n  end\nend"),
        ["entity"]         = new("State-bearing model with fields and relationships.", "entity Customer\n  \"Domain entity\"\n\n  Name: text(100)\n  Email: email, optional\n\n  has_many Order\nend"),
        ["key"]            = new("Primary key and secondary index definition for an entity.", "key Customer\n  Id: id\n\n  index Email unique\nend"),
        ["enum"]           = new("Named constant values with explicit numeric mappings.", "enum Status\n  Active: 1\n  Suspended: 2\nend"),
        ["flags"]          = new("Bitwise enum values that can be combined.", "flags Permission\n  Read: 1\n  Write: 2\n  Execute: 4\nend"),
        ["service"]        = new("Boundary that owns entities and exposes operations.", "service Orders\n  \"Order management\"\n\n  entities\n    Order\n  end\nend"),
        ["command"]        = new("State-changing operation.", "command CreateOrder\n  input\n    CustomerId: guid\n  end\n\n  output\n    Order\n  end\nend"),
        ["query"]          = new("Read-only operation.", "query GetOrder\n  input\n    OrderId: guid\n  end\n\n  returns Order\nend"),
        ["value"]          = new("Immutable value object.", "value Address\n  Street: text(100)\n  City: text(100)\nend"),
        ["shared"]         = new("Shared lookup data reused across services.", "shared Country\n  Name: text(100)\n  Code: text(2)\nend"),
        ["event"]          = new("Domain event payload.", "event OrderCreated\n  OrderId: guid\n  CreatedAt: datetime\nend"),
        ["projection"]     = new("Read model optimized for queries/reporting.", "projection OrderSummary\n  OrderId: guid\n  Total: money\nend"),
        ["union"]          = new("Discriminated union with named variants.", "union PaymentMethod\n  variant Card\n    Last4: text(4)\n  end\n\n  variant Cash\n    Received: money\n  end\nend"),

        ["text"]           = new("Unicode string.", "Name: text(100)"),
        ["integer"]        = new("32-bit signed integer.", "Quantity: integer"),
        ["long"]           = new("64-bit signed integer.", "Sequence: long"),
        ["decimal"]        = new("Exact decimal number.", "Rate: decimal"),
        ["boolean"]        = new("Boolean value (`true` or `false`).", "IsActive: boolean"),
        ["date"]           = new("Calendar date (no time component).", "StartDate: date"),
        ["time"]           = new("Time of day (no date component).", "StartTime: time"),
        ["datetime"]       = new("Combined date and time.", "CreatedAt: datetime"),
        ["guid"]           = new("Globally unique identifier (UUID v4).", "OrderId: guid"),
        ["id"]             = new("Shorthand for `guid, generated`.", "Id: id"),
        ["name"]           = new("Human-readable name string.", "DisplayName: name"),
        ["binary"]         = new("Raw byte array.", "Payload: binary"),
        ["email"]          = new("Email address.", "Email: email"),
        ["url"]            = new("URL string.", "Website: url"),
        ["money"]          = new("Monetary value (amount + currency code).", "Total: money"),
        ["percentage"]     = new("Percentage value.", "Discount: percentage"),
        ["geospatial"]     = new("Latitude/longitude coordinate.", "Location: geospatial"),
        ["image"]          = new("Image binary content.", "Avatar: image"),
        ["document"]       = new("Document binary content.", "Attachment: document"),

        ["has_one"]        = new("Single entity reference (foreign key).", "has_one Customer"),
        ["has_many"]       = new("Collection of related entities.", "has_many Order"),
        ["belongs_to"]     = new("Inverse side of a `has_one` relationship.", "belongs_to Region"),
        ["many_to_many"]   = new("Bidirectional collection relationship.", "many_to_many Tag through ProductTag"),
        ["references"]     = new("Read-only reference block to external entities.", "references\n  Customer\nend"),

        ["input"]          = new("Block of input fields.", "input\n  Id: guid\nend"),
        ["output"]         = new("Success payload block for commands.", "output\n  Result\nend"),
        ["errors"]         = new("Error contract block for operations.", "errors\n  ValidationError\nend"),
        ["publishes"]      = new("Events emitted on successful command handling.", "publishes\n  OrderCreated\nend"),
        ["entities"]       = new("Entities owned by a service.", "entities\n  Order\nend"),
        ["enums"]          = new("Enums owned by a service.", "enums\n  OrderStatus\nend"),
        ["services"]       = new("Services declared in a domain.", "services\n  Orders\nend"),
        ["calls"]          = new("External commands this service invokes.", "calls\n  Billing.ChargeCard\nend"),
        ["implements"]     = new("Commands this service handles.", "implements\n  Orders.CreateOrder\nend"),
        ["variant"]        = new("A named case inside a union.", "variant Card\n  Last4: text(4)\nend"),

        ["company"]        = new("Organisation that owns this domain.", "company \"Contoso\""),
        ["version"]        = new("Semantic version of the domain model.", "version \"1.0.0\""),
        ["returns"]        = new("Query return type.", "returns many OrderSummary"),
        ["transport"]      = new("Operation transport protocol.", "transport http"),
        ["streaming"]      = new("Operation streaming mode.", "streaming server"),
        ["optional"]       = new("Field may be null/absent.", "Email: email, optional"),
        ["unique"]         = new("Field or index must be unique.", "index Email unique"),
        ["generated"]      = new("Field value is generated automatically.", "Id: guid, generated"),
        ["index"]          = new("Secondary index on one or more fields.", "index [Region, Name]"),
        ["end"]            = new("Closes the current block.", "end"),
    };

    protected override HoverRegistrationOptions CreateRegistrationOptions(
        HoverCapability capability,
        ClientCapabilities clientCapabilities) => new()
    {
        DocumentSelector = Selector
    };

    public override Task<Hover?> Handle(HoverParams request, CancellationToken cancellationToken)
    {
        var content = documentManager.GetContent(request.TextDocument.Uri);
        if (content is null)
            return Task.FromResult<Hover?>(null);

        var word = WordAt(content, request.Position);
        if (word is null || !Docs.TryGetValue(word, out var doc))
            return Task.FromResult<Hover?>(null);

        var markdown = $"**{word}**\n\n{doc.Summary}";
        if (!string.IsNullOrWhiteSpace(doc.Syntax))
            markdown += $"\n\n**Syntax**\n```modeller\n{doc.Syntax}\n```";

        return Task.FromResult<Hover?>(new Hover
        {
            Contents = new MarkedStringsOrMarkupContent(new MarkupContent
            {
                Kind  = MarkupKind.Markdown,
                Value = markdown
            })
        });
    }

    private static string? WordAt(string content, Position pos)
    {
        var lines = content.Split('\n');
        if ((int)pos.Line >= lines.Length) return null;

        var line = lines[(int)pos.Line];
        var col  = Math.Min((int)pos.Character, line.Length);

        // expand left
        var start = col;
        while (start > 0 && IsWordChar(line[start - 1])) start--;

        // expand right
        var end = col;
        while (end < line.Length && IsWordChar(line[end])) end++;

        return start == end ? null : line[start..end];
    }

    private static bool IsWordChar(char c) => char.IsLetterOrDigit(c) || c == '_';
}
