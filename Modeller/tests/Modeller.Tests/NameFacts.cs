using FluentAssertions;
using Modeller.Models;
using Xunit;

namespace Modeller.Tests
{
    public static class NameFacts
    {
        [Theory]
        [InlineData("Test", "Test", "test", "_test", "Test", "Test")]
        [InlineData("freight rates", "FreightRate", "freightRate", "_freightRate", "FreightRate", "Freight Rate")]
        [InlineData(null, "", "", "", "", "")]
        [InlineData("", "", "", "", "", "")]
        [InlineData("  ", "", "", "", "", "")]
        public static void ValueSingular(string value, string expected, string local, string module, string global, string display)
        {
            var n = new Name(value);
            n.Singular.Value.Should().Be(expected);
            n.Singular.LocalVariable.Should().Be(local);
            n.Singular.ModuleVariable.Should().Be(module);
            n.Singular.StaticVariable.Should().Be(global);
            n.Singular.Display.Should().Be(display);
        }

        [Theory]
        [InlineData("Test", "Tests", "tests", "_tests", "Tests", "Tests")]
        [InlineData("freight rates", "FreightRates", "freightRates", "_freightRates", "FreightRates", "Freight Rates")]
        [InlineData(null, "", "", "", "", "")]
        [InlineData("", "", "", "", "", "")]
        [InlineData("  ", "", "", "", "", "")]
        public static void ValuePlural(string value, string expected, string local, string module, string global, string display)
        {
            var n = new Name(value);
            n.Plural.Value.Should().Be(expected);
            n.Plural.LocalVariable.Should().Be(local);
            n.Plural.ModuleVariable.Should().Be(module);
            n.Plural.StaticVariable.Should().Be(global);
            n.Plural.Display.Should().Be(display);
        }
    }
}