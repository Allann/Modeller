using System.Collections.Generic;
using System.Linq;

namespace Hy.Modeller
{
    public static class Targets
    {
        public static string Default => Supported.First();

        public static IEnumerable<string> Supported => new List<string> { "netstandard2.0" };
    }
}