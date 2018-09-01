using Modeller.Extensions;
using Modeller.Interfaces;
using System.IO;

namespace Modeller.Generator
{
    internal class JsonSettingsLoader : ISettingsLoader
    {
        public ISettings Load<T>(string filePath)
            where T : ISettings
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Settings file '{filePath}' does not exist.");
            }

            string settings;
            using (var reader = File.OpenText(filePath))
            {
                settings = reader.ReadToEnd();
            }
            return settings.FromJson<T>();
        }

        public bool TryLoad<T>(string filePath, out ISettings settings)
            where T : ISettings
        {
            try
            {
                settings = Load<T>(filePath);
                return true;
            }
            catch
            {
                settings = null;
                return false;
            }
        }
    }
}
