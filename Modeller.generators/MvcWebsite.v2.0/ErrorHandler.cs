using System;
using System.Text;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class ErrorHandler : IGenerator
    {
        private readonly Module _module;

        public ErrorHandler(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new StringBuilder();

            sb.AppendLine("using System;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using Flurl.Http;");
            sb.AppendLine("using Jbssa.Core.ExceptionTranslation;");
            sb.AppendLine("using Jbssa.Core.Extensions;");
            sb.AppendLine("using Jbssa.Core.Mvc;");
            sb.AppendLine("using Jbssa.Core.Mvc.Extensions;");
            sb.AppendLine("using Jbssa.Core.Mvc.Services.Toastr;");
            sb.AppendLine("using Microsoft.AspNetCore.Mvc.ModelBinding;");
            sb.AppendLine("using Microsoft.Extensions.Logging;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Web");
            sb.AppendLine("{");
            sb.AppendLine("    public static class ErrorHandler");
            sb.AppendLine("    {");
            sb.AppendLine("        public static async Task<string> LogError(this ControllerBase controller, FlurlHttpException ex, ModelStateDictionary modelState, ILogger logger, string message, string header)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (ex == null)");
            sb.AppendLine("                throw new ArgumentNullException(nameof(ex));");
            sb.AppendLine("            if (modelState == null)");
            sb.AppendLine("                throw new ArgumentNullException(nameof(modelState));");
            sb.AppendLine();
            sb.AppendLine("            // assume a translated exception first");
            sb.AppendLine("            try");
            sb.AppendLine("            {");
            sb.AppendLine("                var model = await ex.GetResponseJsonAsync<ExceptionModel>();");
            sb.AppendLine("                if (model != null)");
            sb.AppendLine("                {");
            sb.AppendLine("                    var longMessage = model.Description ?? ex.Message;");
            sb.AppendLine("                    var shortMessage = model.Message ?? ex.Message;");
            sb.AppendLine("                    if (message.IsNullOrWhiteSpace())");
            sb.AppendLine("                        message = shortMessage;");
            sb.AppendLine();
            sb.AppendLine("                    if (logger != null)");
            sb.AppendLine("                        logger.LogError(ex, $\"{message}. {longMessage}\");");
            sb.AppendLine("                    modelState.AddModelError(\"\", shortMessage);");
            sb.AppendLine("                    if (!header.IsNullOrWhiteSpace())");
            sb.AppendLine("                        controller.AddToastMessage(header, shortMessage, ToastType.Error);");
            sb.AppendLine("                    return shortMessage;");
            sb.AppendLine("                }");
            sb.AppendLine("            }");
            sb.AppendLine("            catch");
            sb.AppendLine("            { }");
            sb.AppendLine();
            sb.AppendLine("            var msg = await ex.GetResponseStringAsync();");
            sb.AppendLine();
            sb.AppendLine("            if (logger != null)");
            sb.AppendLine("                logger.LogError(ex, $\"{message}. {msg}\");");
            sb.AppendLine("            modelState.AddModelError(\"\", msg);");
            sb.AppendLine("            if (!header.IsNullOrWhiteSpace())");
            sb.AppendLine("                controller.AddToastMessage(header, msg, ToastType.Error);");
            sb.AppendLine();
            sb.AppendLine("            return msg;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return new File() { Name = "ErrorHandler.cs", Content = sb.ToString() };
        }
    }
}