using System.Text;

namespace Hy.Modeller.Outputs
{
    static class StringBuildExtensions
    {
        internal static StringBuilder Indent(this StringBuilder sb, int indent = 1)
        {
            sb.Append(new string(' ', indent * 4));
            return sb;
        }
    }
}
