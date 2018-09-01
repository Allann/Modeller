using FluentAssertions;
using Modeller.Extensions;
using Xunit;

namespace Modeller.Tests
{
    public static class ModuleFacts
    {
        private static string _testModule = $"{{\"company\":\"{Defaults.CompanyName}\",\"project\":\"\",\"models\":[]}}";

        [Fact]
        public static void CanSerialiseToJson()
        {
            var x = new Models.Module();

            var json = x.ToJson();

            json.Should().Be(_testModule);
        }

        [Fact]
        public static void CanDeserialiseFromJson()
        {
            var obj = _testModule.FromJson<Models.Module>();

            obj.Should().BeOfType<Models.Module>().And.NotBeNull();
        }
    }
}