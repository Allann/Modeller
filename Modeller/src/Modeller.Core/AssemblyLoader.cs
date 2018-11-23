using Hy.Modeller.Interfaces;
using McMaster.NETCore.Plugins;
using System.IO;
using System.Reflection;

namespace Hy.Modeller
{
    internal class GeneratorLoader
    {
        private readonly string _folderPath;

        public GeneratorLoader(string folderPath)
        {
            _folderPath = folderPath;
        }

        private FileInfo LoadFileInfo(string assemblyName) => new FileInfo(Path.Combine(_folderPath, $"{assemblyName}.dll"));

        internal Assembly Load(string filePath)
        {
            var loader = PluginLoader.CreateFromAssemblyFile(filePath, sharedTypes: new[] { typeof(ISettings) });
            return loader.LoadDefaultAssembly();
        }

        internal Assembly Load(AssemblyName assemblyName)
        {
            var fileInfo = LoadFileInfo(assemblyName.Name);
            return File.Exists(fileInfo.FullName)
                ? TryGetAssemblyFromAssemblyName(assemblyName, out var assembly) ? assembly : Load(fileInfo.FullName)
                : Assembly.Load(assemblyName);
        }

        private bool TryGetAssemblyFromAssemblyName(AssemblyName assemblyName, out Assembly assembly)
        {
            try
            {
                var fileInfo = LoadFileInfo(assemblyName.Name);

                var loader = PluginLoader.CreateFromAssemblyFile(fileInfo.FullName, sharedTypes: new[] { typeof(ISettings), typeof(IMetadata), typeof(IGenerator), typeof(IOutput) });
                assembly = loader.LoadDefaultAssembly();
                return true;
            }
            catch
            {
                assembly = null;
                return false;
            }
        }
    }
}