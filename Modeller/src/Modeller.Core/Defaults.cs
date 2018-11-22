using System;
using System.IO;

namespace Hy.Modeller
{
    public static class Defaults
    {
        public static string OutputFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Visual Studio 2017", "Projects");

        public static string LocalFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Jbssa", "Generators");

        //public static string ServerFolder => @"\\auqldrv00fil1bi\work\IT\Development\JBSFramework\Generators";

        public static string Target => Targets.Default;

        public static Version Version => new Version();
    }
}