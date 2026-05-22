using Modeller.Parser;

namespace Modeller.Parser.Tests;

public class KeyParserTests
{
    [Fact]
    public void ParsesKeyWithIdShorthand()
    {
        var input = """
            key Booking
              BookingId: id
            end
            """;

        var result = DslParser.ParseKey(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal("Booking", result.Value!.EntityName);
        Assert.Single(result.Value.Fields);

        var field = result.Value.Fields[0];
        Assert.Equal("BookingId", field.Name);
        Assert.Equal("guid", field.DataType);
        Assert.True(field.IsGenerated);
    }

    [Fact]
    public void ParsesKeyWithGuidGenerated()
    {
        var input = """
            key Entity
              EntityId: guid, generated
            end
            """;

        var result = DslParser.ParseKey(input);

        Assert.True(result.Success, result.Error);
        var field = result.Value!.Fields[0];
        Assert.Equal("EntityId", field.Name);
        Assert.Equal("guid", field.DataType);
        Assert.True(field.IsGenerated);
    }

    [Fact]
    public void IdAndGuidGeneratedProduceSameAst()
    {
        var idInput = """
            key Thing
              ThingId: id
            end
            """;

        var guidInput = """
            key Thing
              ThingId: guid, generated
            end
            """;

        var idResult = DslParser.ParseKey(idInput);
        var guidResult = DslParser.ParseKey(guidInput);

        Assert.True(idResult.Success, idResult.Error);
        Assert.True(guidResult.Success, guidResult.Error);

        var idField = idResult.Value!.Fields[0];
        var guidField = guidResult.Value!.Fields[0];

        Assert.Equal(guidField.DataType, idField.DataType);
        Assert.Equal(guidField.IsGenerated, idField.IsGenerated);
    }

    [Fact]
    public void ParsesKeyWithUniqueIndex()
    {
        var input = """
            key Booking
              BookingId: id

              index [Child, Date, Session] unique
            end
            """;

        var result = DslParser.ParseKey(input);

        Assert.True(result.Success, result.Error);
        Assert.Single(result.Value!.Indexes!);

        var index = result.Value.Indexes![0];
        Assert.Equal(3, index.Fields.Count);
        Assert.True(index.IsUnique);
    }

    [Fact]
    public void ParsesKeyWithMultipleIndexes()
    {
        var input = """
            key Child
              ChildId: id

              index [Organisation, CRN] unique
              index [DateOfBirth]
            end
            """;

        var result = DslParser.ParseKey(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal(2, result.Value!.Indexes!.Count);
        Assert.True(result.Value.Indexes[0].IsUnique);
        Assert.False(result.Value.Indexes[1].IsUnique);
    }
}
