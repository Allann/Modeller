using System;

namespace Hy.Modeller.Fluent
{
    public static class Module
    {
        public static ModuleBuilder Create(string project)
        {
            if (string.IsNullOrWhiteSpace(project))
            {
                throw new ArgumentNullException(project);
            }

            var module = new Models.Module();
            module.Project.SetName(project);
            return new ModuleBuilder(module);
        }
    }
}