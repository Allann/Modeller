using System;
using System.Text;
using Modeller;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace ApiController
{
    public class Controller : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;

        public Controller(ISettings settings, Module module, Model model)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            if (!Settings.SupportRegen || !_model.IsEntity())
                return null;

            var i1 = h.Indent(1);
            var i2 = h.Indent(2);
            var i3 = h.Indent(3);
            var i4 = h.Indent(4);

            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine($"using {_module.Namespace}.Interfaces;");
            sb.AppendLine("using Jbssa.Core.Interfaces;");
            sb.AppendLine("using Jbssa.Core.Mvc;");
            sb.AppendLine("using Microsoft.AspNetCore.Authorization;");
            sb.AppendLine("using Microsoft.AspNetCore.Mvc;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Api.Controllers");
            sb.AppendLine("{");
            sb.AppendLine($"{i1}// todo: fix api security");
            sb.AppendLine($"{i1}//[Authorize(Security.SupportedPolicies.{_model.Name.Plural.Value})]");
            sb.AppendLine($"{i1}[Route(\"api/{_model.Name.Plural.Value}\")]");
            sb.AppendLine($"{i1}public partial class {_model.Name.Plural.Value}ApiController : ApiBaseAsync<Domain.{_model.Name.Singular.Value}>");
            sb.AppendLine($"{i1}{{");
            sb.AppendLine($"{i2}public {_model.Name.Plural.Value}ApiController(IEntityOptionsAsync<Domain.{_model.Name.Singular.Value}> options)");
            sb.AppendLine($"{i3}: base(options)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}if (options.ReadService != null)");
            sb.AppendLine($"{i4}_controllerOptions += \",GET\";");
            sb.AppendLine($"{i3}if (options.EditService != null)");
            sb.AppendLine($"{i4}_controllerOptions += \",POST,PUT,PATCH,DELETE\";");
            sb.AppendLine($"{i3}if (_options is IHasMapper o)");
            sb.AppendLine($"{i4}_mapper = o.GetMapper();");
            sb.AppendLine($"{i3}if (_mapper == null)");
            sb.AppendLine($"{i4}throw new ApplicationException($\"Missing required Mapping component in {{DisplayName}}\");");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}[HttpOptions]");
            sb.AppendLine($"{i2}public IActionResult Get{_model.Name.Singular.Value}Options()");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}Response.Headers.Add(\"Allow\", _controllerOptions);");
            sb.AppendLine($"{i3}return Ok();");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine($"{i1}}}");
            sb.AppendLine("}");

            return new File { Name = $"{_model.Name.Plural.Value}Controller.cs", Content = sb.ToString(), CanOverwrite = false };
        }
    }
}
