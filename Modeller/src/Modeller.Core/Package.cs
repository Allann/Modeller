namespace Hy.Modeller
{
    public class Package
    {
        public Package()
        {

        }

        public Package(string name, string version)
        {
            Name = name;
            Version = version;
        }

        public string Name { get; set; }

        public string Version { get; set; }
    }
}