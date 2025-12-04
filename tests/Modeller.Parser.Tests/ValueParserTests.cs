using Modeller.Parser;

namespace Modeller.Parser.Tests;

public class ValueParserTests
{
    [Fact]
    public void ParsesSimpleValue()
    {
        var input = """
            value Address
              "A physical address"
              
              Street: text(200) "Street address line"
              City: text(100) "City name"
              PostalCode: text(20) "Postal code"
            end
            """;

        var result = DslParser.ParseValue(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal("Address", result.Value!.Name);
        Assert.Equal("A physical address", result.Value.Description);
        Assert.Equal(3, result.Value.Attributes?.Count);
        
        Assert.Equal("Street", result.Value.Attributes![0].Name);
        Assert.Equal("text", result.Value.Attributes[0].DataType);
        Assert.Equal(200, result.Value.Attributes[0].MaxLength);
    }

    [Fact]
    public void ParsesValueWithDefaults()
    {
        var input = """
            value Money
              Amount: decimal
              Currency: text(3), default("AUD")
            end
            """;

        var result = DslParser.ParseValue(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal("Money", result.Value!.Name);
        Assert.Equal(2, result.Value.Attributes?.Count);
        Assert.Equal("AUD", result.Value.Attributes![1].DefaultValue);
    }

    [Fact]
    public void ParsesValueWithOptionalFields()
    {
        var input = """
            value ContactInfo
              Email: email
              Phone: text(20), optional
            end
            """;

        var result = DslParser.ParseValue(input);

        Assert.True(result.Success, result.Error);
        Assert.False(result.Value!.Attributes![0].IsOptional);
        Assert.True(result.Value.Attributes![1].IsOptional);
    }
}

