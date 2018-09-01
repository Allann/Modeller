using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace UnitTests
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
            var project = (IProject)new ProjectFile(Settings, _module).Create();

            var files = new FileGroup();
            project.AddFileGroup(files);

            return project;
        }
    }
}