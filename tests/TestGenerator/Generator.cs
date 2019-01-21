using Hy.Modeller.Interfaces;
using Hy.Modeller.Outputs;

namespace TestGenerator
{
    public class Generator : IGenerator
    {
        public Generator(ISettings settings)
        {
            Settings = settings;
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            return new Snippet("Test", "Test Content");
        }
    }
}
