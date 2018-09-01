using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class ControllerErrorExtensions : IGenerator
    {
        private readonly Module _module;

        public ControllerErrorExtensions(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("using Flurl.Http;");
            sb.AppendLine($"using {_module.Namespace}.Web.ViewModels.Shared;");
            sb.AppendLine("using Jbssa.Core.Extensions;");
            sb.AppendLine("using Jbssa.Core.Mvc.Extensions;");
            sb.AppendLine("using Jbssa.Core.Mvc.Services.Toastr;");
            sb.AppendLine("using Microsoft.AspNetCore.Diagnostics;");
            sb.AppendLine("using Microsoft.AspNetCore.Mvc;");
            sb.AppendLine("using Microsoft.Extensions.Logging;");
            sb.AppendLine("");
            sb.AppendLine($"namespace {_module.Namespace}.Web.Extensions");
            sb.AppendLine("{");
            sb.AppendLine("    public static class ControllerErrorExtensions");
            sb.AppendLine("    {");
            sb.AppendLine("        public static ViewResult GetViewResult(this Controller controller, int statusCode, AppSettings settings, ApplicationModel model)");
            sb.AppendLine("        {");
            sb.AppendLine("            switch (statusCode)");
            sb.AppendLine("            {");
            sb.AppendLine("                case 404:");
            sb.AppendLine("                    return Get404(controller, settings, model);");
            sb.AppendLine("                default:");
            sb.AppendLine("                    return controller.View(\"Error\");");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private static ViewResult Get404(Controller controller, AppSettings settings, ApplicationModel model)");
            sb.AppendLine("        {");
            sb.AppendLine("            var feature = controller.HttpContext.Features.Get<IStatusCodeReExecuteFeature>();");
            sb.AppendLine("            var reExecuteFeature = feature as StatusCodeReExecuteFeature;");
            sb.AppendLine("            var previousUrl = $\"http://{controller.HttpContext.Request.Host.Value}{reExecuteFeature?.OriginalPath}{reExecuteFeature?.OriginalQueryString}\";");
            sb.AppendLine();
            sb.AppendLine("            var viewModel = new NotFoundViewModel");
            sb.AppendLine("            {");
            sb.AppendLine("                PreviousUrl = previousUrl");
            sb.AppendLine("            };");
            sb.AppendLine();
            sb.AppendLine("            if (!settings.HelpdeskEmail.IsNullOrWhiteSpace())");
            sb.AppendLine("            {");
            sb.AppendLine("                var subject = $\"{model.ApplicationName} - Page Not Found ({previousUrl})\";");
            sb.AppendLine("                var body = $\"A request was made in the {model.ApplicationName} website, but the page cannot be found: {previousUrl}.\\r\\n Environment details: {model.EnvironmentName} environment,\\r\\n Site: {model.SiteName} ({model.SiteCode}),\\r\\n Application version: {model.Version}.\";");
            sb.AppendLine("                viewModel.MailTo = $\"mailto:{settings.HelpdeskEmail}?subject={subject}&body={body}\";");
            sb.AppendLine("                viewModel.AllowEmail = true;");
            sb.AppendLine("            }");
            sb.AppendLine("            return controller.View(\"404\", viewModel);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public static void LogFailedCall<T>(this HttpCall call, ILogger<T> logger, string message)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (call.Succeeded) return;");
            sb.AppendLine();
            sb.AppendLine("            var sb = new System.Text.StringBuilder();");
            sb.AppendLine("            sb.AppendLine(call.ToString());");
            sb.AppendLine("            sb.AppendLine($\"Started: {call.StartedUtc}\");");
            sb.AppendLine("            sb.AppendLine($\"Duration: {call.Duration}\");");
            sb.AppendLine("            sb.AppendLine($\"Finished: {call.EndedUtc}\");");
            sb.AppendLine("            sb.AppendLine($\"StatusCode: {call.Response.StatusCode}\");");
            sb.AppendLine("            sb.AppendLine($\"Message: {message}\");");
            sb.AppendLine("            logger.LogError(call.Exception, sb.ToString());");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public static RedirectToActionResult RedirectOrThrow(this Controller controller, HttpCall call, string displayName=null)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (call.Completed)");
            sb.AppendLine("            {");
            sb.AppendLine("                if (call.Response.StatusCode == System.Net.HttpStatusCode.NotFound)");
            sb.AppendLine("                {");
            sb.AppendLine("                    call.ExceptionHandled = true;");
            sb.AppendLine("                    if (displayName.IsNullOrWhiteSpace()) displayName = \"Item\";");
            sb.AppendLine("                    controller.AddToastMessage($\"{displayName} not found\", \"\", ToastType.Info);");
            sb.AppendLine("                    return controller.RedirectToAction(\"Index\");");
            sb.AppendLine("                }");
            sb.AppendLine("            }");
            sb.AppendLine("            return controller.RedirectToAction(\"Error\", \"Home\",new { statusCode=call.HttpStatus.GetValueOrDefault()});");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return new File
            {
                Name = "ControllerErrorExtensions.cs",
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}