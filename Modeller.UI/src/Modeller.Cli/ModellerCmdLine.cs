using Core.CmdLine;

namespace Modeller.Cli
{
    [CmdLineOptions(Wait = true)]
    public class ModellerCmdLine : CmdLineObject
    {
        [CmdLineArg(Alias = "s", Usage = "file path to the source module definition", ShowInUsage = DefaultBoolean.True)]
        public string SourceModel { get; set; }

        [CmdLineArg(Alias = "o", Usage = "output folder", ShowInUsage = DefaultBoolean.True)]
        public string OutputPath { get; set; }

        [CmdLineArg(Alias = "t", Usage = "target framework", ShowInUsage = DefaultBoolean.True)]
        public string Target { get; set; } = "netstandard2.0";

        [CmdLineArg(Alias = "g", Usage = "generator to use", ShowInUsage = DefaultBoolean.True)]
        public string Generator { get; set; }

        [CmdLineArg(Alias = "v", Usage = "generator version", ShowInUsage = DefaultBoolean.True)]
        public string Version { get; set; }

        [CmdLineArg(Alias = "lf", Usage = "local folder where generator are stored", ShowInUsage = DefaultBoolean.True)]
        public string LocalFolder { get; set; }

        [CmdLineArg(Alias = "sf", Usage = "server folder where generator are stored", ShowInUsage = DefaultBoolean.True)]
        public string ServerFolder { get; set; }

        [CmdLineArg(Alias = "l", Usage = "list all available generators", ShowInUsage = DefaultBoolean.True)]
        public bool List { get; set; }

        [CmdLineArg(Usage = "outputs more details to the console")]
        public bool Verbose { get; set; }

        [CmdLineArg(Alias = "u", Usage = "update local generators", ShowInUsage = DefaultBoolean.True)]
        public bool UpdateLocal { get; set; }

        [CmdLineArg(Usage = "overwrite local files", ShowInUsage = DefaultBoolean.True)]
        public bool Overwrite { get; set; }

        [CmdLineArg(Alias = "m", Usage = "model name within the module", ShowInUsage = DefaultBoolean.True)]
        public string ModelName { get; internal set; }

        [CmdLineArg(Usage = "settings file", ShowInUsage = DefaultBoolean.True)]
        public string Settings { get; internal set; }
    }
}