using Modeller.Parser;

namespace Modeller.Parser.Tests;

public class UnionParserTests
{
    [Fact]
    public void ParsesUnionWithTwoVariants()
    {
        var input = """
            union ContactMethod
              "How to reach a person"

              variant Email
                "Contact via email"
                Address: email "Email address"
              end

              variant Phone
                "Contact via phone"
                Number: text(20) "Phone number"
                Extension: text(10), optional "Extension"
              end
            end
            """;

        var result = DslParser.ParseUnion(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal("ContactMethod", result.Value!.Name);
        Assert.Equal("How to reach a person", result.Value.Description);
        Assert.Equal(2, result.Value.Variants?.Count);
    }

    [Fact]
    public void ParsesVariantNames()
    {
        var input = """
            union StorageLocation
              variant Embedded
                Data: binary "Raw bytes"
              end

              variant Reference
                Key: text(500) "Storage key"
              end
            end
            """;

        var result = DslParser.ParseUnion(input);

        Assert.True(result.Success, result.Error);
        var variants = result.Value!.Variants!;
        Assert.Equal("Embedded", variants[0].Name);
        Assert.Equal("Reference", variants[1].Name);
    }

    [Fact]
    public void ParsesVariantAttributes()
    {
        var input = """
            union Image
              "An image stored inline or by reference"

              variant Embedded
                Data: binary "Raw image bytes"
                MimeType: text(100) "MIME type"
                FileName: text(255) "Original filename"
                SizeBytes: long "File size in bytes"
              end

              variant Reference
                StorageKey: text(500) "External storage key"
                MimeType: text(100) "MIME type"
                FileName: text(255) "Original filename"
                SizeBytes: long "File size in bytes"
              end
            end
            """;

        var result = DslParser.ParseUnion(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal(4, result.Value!.Variants![0].Attributes?.Count);
        Assert.Equal(4, result.Value.Variants[1].Attributes?.Count);

        var firstAttr = result.Value.Variants[0].Attributes![0];
        Assert.Equal("Data", firstAttr.Name);
        Assert.Equal("binary", firstAttr.DataType);
    }

    [Fact]
    public void ParsesVariantWithOptionalAttributes()
    {
        var input = """
            union ContactMethod
              variant Phone
                Number: text(20) "Phone number"
                Extension: text(10), optional "Extension"
              end
            end
            """;

        var result = DslParser.ParseUnion(input);

        Assert.True(result.Success, result.Error);
        var attrs = result.Value!.Variants![0].Attributes!;
        Assert.False(attrs[0].IsOptional);
        Assert.True(attrs[1].IsOptional);
    }

    [Fact]
    public void ParsesMinimalUnion()
    {
        var input = """
            union Simple
              variant A
                Value: text "A value"
              end
            end
            """;

        var result = DslParser.ParseUnion(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal("Simple", result.Value!.Name);
        Assert.Null(result.Value.Description);
        Assert.Single(result.Value.Variants!);
    }
}
