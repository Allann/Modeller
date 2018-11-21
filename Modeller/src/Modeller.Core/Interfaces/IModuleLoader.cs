using Hy.Modeller.Models;

namespace Hy.Modeller.Interfaces
{
    internal interface IModuleLoader
    {
        Module Load(string filePath);
        bool TryLoad(string filePath, out Module module);
    }
}
