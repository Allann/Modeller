using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class SharedViewComponentUser : IGenerator
    {
        private readonly Module _module;
        
        public SharedViewComponentUser(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("@model UserViewModel");
            sb.AppendLine("<h4 class=\"pull-right\" style=\"text-align:right;\"><small>@Model.UserId</small></h4>");

            return new File
            {
                Name = "Default.cshtml",
                Path = System.IO.Path.Combine("Shared","Components", "User"),
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}