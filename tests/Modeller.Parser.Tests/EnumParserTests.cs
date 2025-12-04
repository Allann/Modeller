using Modeller.Parser;

namespace Modeller.Parser.Tests;

public class EnumParserTests
{
    [Fact]
    public void ParsesSimpleEnum()
    {
        var input = """
            enum DataType
              "The primitive data types"
              
              Text: 1 "Variable length text"
              Integer: 2 "Whole number"
              Boolean: 3 "True or false"
            end
            """;

        var result = DslParser.ParseEnum(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal("DataType", result.Value!.Name);
        Assert.Equal("The primitive data types", result.Value.Description);
        Assert.Equal(3, result.Value.Values?.Count);
        
        Assert.Equal("Text", result.Value.Values![0].Name);
        Assert.Equal(1, result.Value.Values[0].Value);
        Assert.Equal("Variable length text", result.Value.Values[0].Description);
    }

    [Fact]
    public void ParsesFlags()
    {
        var input = """
            flags DaysOfWeek
              "Days of the week as bit flags"
              
              None: 0 "No days"
              Monday: 1 "Monday"
              Tuesday: 2 "Tuesday"
              Wednesday: 4 "Wednesday"
              Thursday: 8 "Thursday"
              Friday: 16 "Friday"
              Saturday: 32 "Saturday"
              Sunday: 64 "Sunday"
            end
            """;

        var result = DslParser.ParseEnum(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal("DaysOfWeek", result.Value!.Name);
        Assert.Equal(8, result.Value.Values?.Count);
        Assert.Equal(64, result.Value.Values![7].Value);
    }

    [Fact]
    public void ParsesEnumWithoutDescriptions()
    {
        var input = """
            enum Status
              Active: 1
              Inactive: 2
              Deleted: 3
            end
            """;

        var result = DslParser.ParseEnum(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal(3, result.Value!.Values?.Count);
        Assert.Null(result.Value.Values![0].Description);
    }
}

