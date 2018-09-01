using System;
using System.IO;

namespace Modeller
{
    public static class Defaults
    {
        public static string CompanyName => "MyCompany";

        public static string LocalFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Defaults.CompanyName, "Generators");

        public static string ServerFolder => @"\Generators";

        public static string Target => "netstandard2.0";

        public static Version Version => new Version();
    }
}