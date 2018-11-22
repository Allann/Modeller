using Hy.Modeller.Interfaces;
using Hy.Modeller.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Hy.Modeller.Generator
{
    public class Context
    {
        private IList<string> _issues = new List<string>();
        private readonly Action<string> _output;

        public Context(string moduleFile, string folder, string generator, string target, string version, string settingsFile, string modelName, string outputPath, Action<string> output = null)
        {
            SetFolder(folder);
            SetTarget(target);
            SetGeneratorName(generator, version);
            SetOutputPath(outputPath);

            ModuleFile = moduleFile;
            SettingsFile = settingsFile;
            ModelName = modelName;
            _output = output;
            Validate();
        }

        public string Target { get; private set; }

        public void SetTarget(string value) => Target = string.IsNullOrWhiteSpace(value) ? Defaults.Target : value;

        public string GeneratorName { get; private set; }

        public Version Version { get; private set; }

        public void SetGeneratorName(string generator, string version)
        {
            if (!Version.TryParse(version, out var vers))
            {
                Version = Defaults.Version;
            }
            GeneratorName = generator;
        }

        public string Folder { get; private set; }

        public void SetFolder(string value) => Folder = string.IsNullOrWhiteSpace(value) ? Defaults.LocalFolder : value;

        public void Validate()
        {
            // create validators
            var loader = new JsonModuleLoader();
            var mv = new ModuleValidator(this, loader);
            var gv = new GeneratorValidator(this);
            var settingsLoader = new JsonSettingsLoader();
            var sv = new SettingsValidator(this, settingsLoader);
            var cv = new ContextValidator(this);

            var validators = new List<IValidator> { mv, gv, sv, cv };

            // execute validation
            foreach (var validator in validators)
            {
                validator.Validate();
            }

            if (_output != null)
            {
                // output the errors
                foreach (var issue in _issues)
                {
                    _output.Invoke(issue);
                }
            }
        }

        internal void SetGenerator(GeneratorItem generator) => Generator = generator;

        internal void SetModule(Module module) => Module = module;

        internal void SetModel(Model model) => Model = model;

        internal void SetSettings(ISettings settings)
        {
            Settings = settings;
            if (Settings != null && !Settings.Context.Packages.Any())
            {
                var ps = new PackageService();
                Settings.Context.Packages = ps.Items.ToList();
            }
        }

        internal void SetOutputPath(string path) => OutputPath = path;

        internal void AddIssue(string issue)
        {
            if (!string.IsNullOrWhiteSpace(issue))
                _issues.Add(issue);
            else
                _issues.Add("Unknown issue added, but details were not included.");
        }

        public Module Module { get; private set; }
        public GeneratorItem Generator { get; private set; }
        public ISettings Settings { get; private set; }
        public Model Model { get; private set; }

        public bool IsValid => !_issues.Any();
        public IReadOnlyCollection<string> ValidationMessages => new ReadOnlyCollection<string>(_issues);

        public string OutputPath { get; private set; }

        public string ModuleFile { get; }

        public string ModelName { get; }

        public string SettingsFile { get; }
    }
}