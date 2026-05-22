using Modeller.Parser;
using Modeller.Parser.Ast;

namespace Modeller.Parser.Tests;

public class BehaviourParserTests
{
    [Fact]
    public void ParsesCommand()
    {
        var input = """
            command ParseDomain
              "Parses DSL files into domain model"
              
              input
                SourcePath: text(500) "Path to files"
                Recursive: boolean, default(true)
              end
              
              output Domain
              
              errors
                FileNotFound "Source path does not exist"
                ParseError "Syntax error in file"
              end
              
              publishes
                DomainParsed
              end
            end
            """;

        var result = DslParser.ParseCommand(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal("ParseDomain", result.Value!.Name);
        Assert.Equal("Parses DSL files into domain model", result.Value.Description);
        Assert.Equal(2, result.Value.Inputs?.Count);
        Assert.Equal("Domain", result.Value.Output);
        Assert.Equal(2, result.Value.Errors?.Count);
        Assert.Single(result.Value.Events!);
    }

    [Fact]
    public void ParsesMinimalCommand()
    {
        var input = """
            command DoSomething
            end
            """;

        var result = DslParser.ParseCommand(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal("DoSomething", result.Value!.Name);
        Assert.Null(result.Value.Inputs);
        Assert.Null(result.Value.Output);
    }

    [Fact]
    public void ParsesQuery()
    {
        var input = """
            query GetDomain
              "Retrieves a domain by name"
              
              input
                Name: name "The domain name"
              end
              
              returns Domain
            end
            """;

        var result = DslParser.ParseQuery(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal("GetDomain", result.Value!.Name);
        Assert.Single(result.Value.Inputs!);
        Assert.Equal("Domain", result.Value.Returns);
        Assert.False(result.Value.ReturnsMany);
    }

    [Fact]
    public void ParsesQueryReturningMany()
    {
        var input = """
            query ListGenerators
              "Lists all generators"

              returns many Generator
            end
            """;

        var result = DslParser.ParseQuery(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal("Generator", result.Value!.Returns);
        Assert.True(result.Value.ReturnsMany);
    }

    [Fact]
    public void CommandDefaultsToHttpTransport()
    {
        var input = """
            command DoSomething
            end
            """;

        var result = DslParser.ParseCommand(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal(TransportType.Http, result.Value!.Transport);
        Assert.Equal(StreamingMode.None, result.Value.Streaming);
    }

    [Fact]
    public void ParsesCommandWithGrpcTransport()
    {
        var input = """
            command GovSubmitReport
              "Submits a report to government"
              transport grpc

              input
                Request: ReportRequest "The request"
              end

              output
                ReportResponse "The response"
              end
            end
            """;

        var result = DslParser.ParseCommand(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal("GovSubmitReport", result.Value!.Name);
        Assert.Equal(TransportType.Grpc, result.Value.Transport);
        Assert.Equal(StreamingMode.None, result.Value.Streaming);
        Assert.Single(result.Value.Inputs!);
        Assert.Equal("ReportResponse", result.Value.Output);
    }

    [Fact]
    public void ParsesCommandWithGrpcAndServerStreaming()
    {
        var input = """
            command StreamUpdates
              "Stream live updates"
              transport grpc
              streaming server

              input
                Filter: text "Filter expression"
              end

              output
                UpdateEvent "An individual update"
              end
            end
            """;

        var result = DslParser.ParseCommand(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal(TransportType.Grpc, result.Value!.Transport);
        Assert.Equal(StreamingMode.Server, result.Value.Streaming);
    }

    [Theory]
    [InlineData("server", StreamingMode.Server)]
    [InlineData("client", StreamingMode.Client)]
    [InlineData("bidirectional", StreamingMode.Bidirectional)]
    [InlineData("none", StreamingMode.None)]
    public void ParsesAllStreamingModes(string keyword, StreamingMode expected)
    {
        var input = $"""
            command StreamTest
              transport grpc
              streaming {keyword}
            end
            """;

        var result = DslParser.ParseCommand(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal(expected, result.Value!.Streaming);
    }

    [Fact]
    public void ParsesQueryWithGrpcTransport()
    {
        var input = """
            query StreamAttendance
              "Stream attendance events"
              transport grpc
              streaming server

              input
                CentreId: id "Centre to watch"
              end

              returns AttendanceEvent
            end
            """;

        var result = DslParser.ParseQuery(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal(TransportType.Grpc, result.Value!.Transport);
        Assert.Equal(StreamingMode.Server, result.Value.Streaming);
        Assert.Equal("AttendanceEvent", result.Value.Returns);
    }

    [Fact]
    public void QueryDefaultsToHttpTransport()
    {
        var input = """
            query GetSomething
              returns SomeType
            end
            """;

        var result = DslParser.ParseQuery(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal(TransportType.Http, result.Value!.Transport);
        Assert.Equal(StreamingMode.None, result.Value.Streaming);
    }
}

