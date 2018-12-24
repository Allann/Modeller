using System;
using System.Collections.Generic;
using Hy.Modeller.Interfaces;
using Hy.Modeller.Models;

namespace Hy.Modeller.Generator
{
    public interface IContext
    {
        string Folder { get; }
        string GeneratorName { get; }
        string Target { get; }
        Version Version { get; }

        GeneratorItem Generator { get; }
        bool IsValid { get; }
        Model Model { get; }
        string ModelName { get; }
        Module Module { get; }
        string ModuleFile { get; }
        string OutputPath { get; }
        ISettings Settings { get; }
        string SettingsFile { get; }
        IReadOnlyCollection<string> ValidationMessages { get; }

        void Validate();
    }
}