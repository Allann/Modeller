using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary
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
