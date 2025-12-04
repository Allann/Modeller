using Modeller.Parser;

namespace Modeller.Parser.Tests;

public class SharedParserTests
{
    [Fact]
    public void ParsesSimpleShared()
    {
        var input = """
            shared Country
              "Reference data for countries"
              
              Code: text(2) "ISO country code"
              Name: text(100) "Country name"
            end
            """;

        var result = DslParser.ParseShared(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal("Country", result.Value!.Name);
        Assert.Equal("Reference data for countries", result.Value.Description);
        Assert.Equal(2, result.Value.Attributes?.Count);
        
        Assert.Equal("Code", result.Value.Attributes![0].Name);
        Assert.Equal("Name", result.Value.Attributes![1].Name);
    }

    [Fact]
    public void ParsesSharedWithoutDescription()
    {
        var input = """
            shared Currency
              Code: text(3)
              Symbol: text(5)
              Name: text(50)
            end
            """;

        var result = DslParser.ParseShared(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal("Currency", result.Value!.Name);
        Assert.Null(result.Value.Description);
        Assert.Equal(3, result.Value.Attributes?.Count);
    }

    [Fact]
    public void ParsesSharedWithOptionalFields()
    {
        var input = """
            shared Region
              Code: text(10)
              Name: text(100)
              Description: text(500), optional
            end
            """;

        var result = DslParser.ParseShared(input);

        Assert.True(result.Success, result.Error);
        Assert.False(result.Value!.Attributes![0].IsOptional);
        Assert.True(result.Value.Attributes![2].IsOptional);
    }
}

