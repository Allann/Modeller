using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Modeller
{
    internal class AssemblyLoader : AssemblyLoadContext
    {
        private readonly string _folderPath;

        internal AssemblyLoader(string folderPath)
        {
            _folderPath = folderPath;
        }

        internal Assembly Load(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            var assemblyName = new AssemblyName(fileInfo.Name.Replace(fileInfo.Extension, string.Empty));
            return Load(assemblyName);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            var fileInfo = LoadFileInfo(assemblyName.Name);
            return File.Exists(fileInfo.FullName)
                ? TryGetAssemblyFromAssemblyName(assemblyName, out var assembly) ? assembly : LoadFromAssemblyPath(fileInfo.FullName)
                : Assembly.Load(assemblyName);
        }

        private FileInfo LoadFileInfo(string assemblyName) => new FileInfo(Path.Combine(_folderPath, $"{assemblyName}.dll"));

        private static bool TryGetAssemblyFromAssemblyName(AssemblyName assemblyName, out Assembly assembly)
        {
            try
            {
                assembly = Default.LoadFromAssemblyName(assemblyName);
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