using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class SharedViewToastrBuilder : IGenerator
    {
        private readonly Module _module;

        public SharedViewToastrBuilder(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("@using Jbssa.Core.Mvc.Services.Toastr");
            sb.AppendLine("@using Newtonsoft.Json;");
            sb.AppendLine();
            sb.AppendLine("@if (TempData.ContainsKey(\"Toastr\"))");
            sb.AppendLine("{");
            sb.AppendLine("    var jsonToast = TempData[\"Toastr\"] as string;");
            sb.AppendLine("    Toastr toastr = JsonConvert.DeserializeObject<Toastr>(jsonToast);");
            sb.AppendLine("    if (toastr != null)");
            sb.AppendLine("    {");
            sb.AppendLine("        <script type=\"text/javascript\">");
            sb.AppendLine("            $(document).ready(function () {");
            sb.AppendLine("                toastr.options.closeButton = '@toastr.ShowCloseButton';");
            sb.AppendLine("                toastr.options.newestOnTop = '@toastr.ShowNewestOnTop';");
            sb.AppendLine();
            sb.AppendLine("                @foreach (ToastMessage message in toastr.ToastMessages)");
            sb.AppendLine("                {");
            sb.AppendLine("                    string toastTypeValue = message.ToastType.ToString(\"F\").ToLower();");
            sb.AppendLine("                    @: var optionsOverride = { positionClass: \"toast-top-center\" };");
            sb.AppendLine("                    if (message.IsSticky) {");
            sb.AppendLine("                        @:optionsOverride.timeOut = 0;");
            sb.AppendLine("                        @:optionsOverride.extendedTimeout = 0;");
            sb.AppendLine("                    }");
            sb.AppendLine("                    if (!string.IsNullOrEmpty(message.Width)) {");
            sb.AppendLine("                        @:toastr['@toastTypeValue']('@message.Message', '@message.Title', optionsOverride).attr(\"style\", \"width:\" + @message.Width + \"px !important\");");
            sb.AppendLine("                    }");
            sb.AppendLine("                    else {");
            sb.AppendLine("                        @:toastr['@toastTypeValue']('@message.Message', '@message.Title', optionsOverride);");
            sb.AppendLine("                    }");
            sb.AppendLine("                }");
            sb.AppendLine("            });");
            sb.AppendLine("        </script>");
            sb.AppendLine("    }");
            sb.AppendLine("    TempData.Remove(\"Toastr\");");
            sb.AppendLine("}");

            return new File
            {
                Name = "_ToastrBuilder.cshtml",
                Path = "Shared",
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}
