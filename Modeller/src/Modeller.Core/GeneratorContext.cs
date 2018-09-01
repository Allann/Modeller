using System.Collections.Generic;

namespace Modeller
{
    public class GeneratorContext
    {
        public string SourceModel { get; set; }
        public string OutputPath { get; set; }
        public string Target { get; set; }
        public string Generator { get; set; }
        public string Version { get; set; }
        public string LocalFolder { get; set; }
        public string ServerFolder { get; set; }
        public bool Verbose { get; set; }
        public string ModelName { get; set; }
        public string Settings { get; set; }

        public List<Package> Packages { get; set; } = new List<Package>();
    }
}