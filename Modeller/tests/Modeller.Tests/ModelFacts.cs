using FluentAssertions;
using Hy.Modeller.Extensions;
using Xunit;

namespace Hy.Modeller.Tests
{
    public static class ModelFacts
    {
        private const string _testModel = @"{""name"":"""",""key"":{""fields"":[]},""fields"":[],""indexes"":[],""relationships"":[]}";

        [Fact]
        public static void CanSerialiseToJson()
        {
            var x = new Models.Model();

            var json = x.ToJson();

            json.Should().Be(_testModel);
        }

        [Fact]
        public static void CanDeserialiseFromJson()
        {
            var obj = _testModel.FromJson<Models.Model>();

            obj.Should().BeOfType<Models.Model>().And.NotBeNull();
        }
    }
}