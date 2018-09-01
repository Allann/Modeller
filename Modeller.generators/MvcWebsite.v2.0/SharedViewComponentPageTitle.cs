using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class SharedViewComponentPageTitle : IGenerator
    {
        private readonly Module _module;
        
        public SharedViewComponentPageTitle(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("@model ApplicationModel");
            sb.AppendLine("@Model.SiteCode - @ViewBag.Title - @Model.ApplicationName - @Model.Version");

            return new File
            {
                Name = "Default.cshtml",
                Path = System.IO.Path.Combine("Shared","Components", "PageTitle"),
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}