using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace DbContext
{
    public class Generator : IGenerator
    {
        private readonly Module _module;

        public Generator(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var files = new FileGroup();
            files.AddFile((IFile)new DbContextUser(Settings, _module).Create());
            files.AddFile((IFile)new DbContextGenerated(Settings, _module).Create());
            return files;
        }
    }
}