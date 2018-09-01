using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class SharedViewError : IGenerator
    {
        private readonly Module _module;
        
        public SharedViewError(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("@using Microsoft.AspNetCore.Hosting");
            sb.AppendLine($"@using {_module.Namespace}.Web.ViewModels");
            sb.AppendLine();
            sb.AppendLine("@model ErrorViewModel");
            sb.AppendLine("@inject IHostingEnvironment host");
            sb.AppendLine();
            sb.AppendLine("@{");
            sb.AppendLine("    var error = Model?.Error?.Error;");
            sb.AppendLine("    var errorDescription = host.IsDevelopment() ? Model?.Error?.ErrorDescription : null;");
            sb.AppendLine("    var request_id = Model?.Error?.RequestId;");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("<div class=\"error-page\">");
            sb.AppendLine("    <div class=\"page-header\">");
            sb.AppendLine("        <h1>Error</h1>");
            sb.AppendLine("    </div>");
            sb.AppendLine();
            sb.AppendLine("    <div class=\"row\">");
            sb.AppendLine("        <div class=\"col-sm-6\">");
            sb.AppendLine("            <div class=\"alert alert-danger\">");
            sb.AppendLine("                Sorry, there was an error");
            sb.AppendLine();
            sb.AppendLine("                @if (error != null)");
            sb.AppendLine("                {");
            sb.AppendLine("                    <strong>");
            sb.AppendLine("                        <em>");
            sb.AppendLine("                            : @error");
            sb.AppendLine("                        </em>");
            sb.AppendLine("                    </strong>");
            sb.AppendLine();
            sb.AppendLine("                    if (errorDescription != null)");
            sb.AppendLine("                    {");
            sb.AppendLine("                        <div>@errorDescription</div>");
            sb.AppendLine("                    }");
            sb.AppendLine("                }");
            sb.AppendLine("            </div>");
            sb.AppendLine();
            sb.AppendLine("            @if (request_id != null)");
            sb.AppendLine("            {");
            sb.AppendLine("                <div class=\"request-id\">Request Id: @request_id</div>");
            sb.AppendLine("            }");
            sb.AppendLine("        </div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</div>");

            return new File
            {
                Name = "Error.cshtml",
                Path = "Shared",
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}