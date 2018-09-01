using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class SharedViewComponentVersion : IGenerator
    {
        private readonly Module _module;

        public SharedViewComponentVersion(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("@model ApplicationModel");
            sb.AppendLine("<small>@Model.Version</small>");

            return new File
            {
                Name = "Default.cshtml",
                Path = System.IO.Path.Combine("Shared","Components", "Version"),
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}