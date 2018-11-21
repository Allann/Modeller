using Hy.Modeller.Extensions;
using Hy.Modeller.Interfaces;
using System.IO;

namespace Hy.Modeller.Generator
{
    internal class JsonModuleLoader : IModuleLoader
    {
        public Models.Module Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Module file '{filePath}' does not exist.");
            }

            string model;
            using (var reader = File.OpenText(filePath))
            {
                model = reader.ReadToEnd();
            }
            return model.FromJson<Models.Module>();
        }

        public bool TryLoad(string filePath, out Models.Module module)
        {
            try
            {
                module = Load(filePath);
                return true;
            }
            catch
            {
                module = null;
                return false;
            }
        }
    }
}
