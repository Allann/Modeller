using System;
using System.Text;
using Modeller;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace BusinessService
{
    internal class ReadService : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;

        public ReadService(ISettings settings, Module module, Model model)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            if (!Settings.SupportRegen)
                return null;

            var i1 = h.Indent(1);
            var i2 = h.Indent(2);
            var i3 = h.Indent(3);

            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine($"using {_module.Namespace}.Data;");
            sb.AppendLine("using Jbssa.Core.BusinessLogic;");
            sb.AppendLine("using Kendo.Mvc.Extensions;");
            sb.AppendLine("using Kendo.Mvc.UI;");
            sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Business.Services");
            sb.AppendLine("{");
            sb.AppendLine($"{i1}public partial class {_model.Name.Singular.Value}ReadService : ReadonlyServiceAsyncBase<Domain.{_model.Name.Singular.Value}>");
            sb.AppendLine($"{i1}{{");
            sb.AppendLine($"{i2}public {_model.Name.Singular.Value}ReadService({_module.Project.Singular.Value}DbContext context)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}_context = context ?? throw new ArgumentNullException(nameof(context));");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();

            var first = true;
            foreach (var item in _model.Relationships)
            {
                item.GetOther(_model.Name, out var type, out var model, out var field);
                if (first)
                {
                    sb.AppendLine($"{i2}partial void GetAllQuery(ref IQueryable<Domain.{_model.Name.Singular.Value}> query, ref DataSourceRequest request)");
                    sb.AppendLine($"{i2}{{");
                    sb.Append($"{i3}query = query");
                    first = false;
                }
                if (type == RelationShipTypes.Many)
                    sb.Append($".Include(a => a.{model.Plural.Value})");
                else
                    sb.Append($".Include(a => a.{model.Singular.Value})");
            }
            if (!first)
            {
                sb.AppendLine(";");
                sb.AppendLine($"{i2}}}");
                sb.AppendLine();
            }

            first = true;
            foreach (var item in _model.Relationships)
            {
                item.GetOther(_model.Name, out var type, out var model, out var field);
                if (first)
                {
                    sb.AppendLine($"{i2}partial void GetItemQuery(ref IQueryable<Domain.{_model.Name.Singular.Value}> query)");
                    sb.AppendLine($"{i2}{{");
                    sb.Append($"{i3}query = query");
                    first = false;
                }
                if(type==RelationShipTypes.Many)
                    sb.Append($".Include(a => a.{model.Plural.Value})");
                else
                    sb.Append($".Include(a => a.{model.Singular.Value})");
            }
            if (!first)
            {
                sb.AppendLine(";");
                sb.AppendLine($"{i2}}}");
                sb.AppendLine();
            }

            sb.AppendLine($"{i2}protected override Task<IEnumerable<Domain.{_model.Name.Singular.Value}>> GetAllAsync()");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}throw new NotImplementedException();");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine($"{i1}}}");
            sb.AppendLine("}");

            return new File { Name = $"{_model.Name.Singular.Value}ReadService.cs", Content = sb.ToString(), CanOverwrite = false };
        }
    }
}