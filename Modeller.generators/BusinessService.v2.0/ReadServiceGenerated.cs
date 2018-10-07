using System;
using System.Text;
using Modeller;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace BusinessService
{
    internal class ReadServiceGenerated : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;

        public ReadServiceGenerated(ISettings settings, Module module, Model model)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var i1 = h.Indent(1);
            var i2 = h.Indent(2);
            var i3 = h.Indent(3);
            var i4 = h.Indent(4);

            var sb = new StringBuilder();
            sb.AppendLine(((ISnippet)new Header.Generator(Settings, new GeneratorDetails()).Create()).Content);
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine($"using {_module.Namespace}.Business.Extensions;");
            sb.AppendLine($"using {_module.Namespace}.Data;");
            sb.AppendLine("using Microsoft.AspNetCore.JsonPatch;");
            sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb.AppendLine("using Jbssa.Core.Interfaces;");
            sb.AppendLine("using Jbssa.Core.Extensions;");
            sb.AppendLine("using Jbssa.Core.BusinessLogic;");
            sb.AppendLine("using Kendo.Mvc.UI;");
            sb.AppendLine("using Kendo.Mvc.Extensions;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Business.Services");
            sb.AppendLine("{");
            sb.Append($"{i1}");
            if (!Settings.SupportRegen)
                sb.Append("public ");
            sb.Append($"partial class {_model.Name.Singular.Value}ReadService");
            if (!Settings.SupportRegen)
                sb.AppendLine($" : ReadonlyServiceAsyncBase<Domain.{_model.Name.Singular.Value}>");
            else
                sb.AppendLine();
            sb.AppendLine($"{i1}{{");
            sb.AppendLine($"{i2}private readonly {_module.Project.Singular.Value}DbContext _context;");
            if (Settings.SupportRegen)
            {
                sb.AppendLine();
                sb.AppendLine($"{i2}partial void GetAllQuery(ref IQueryable<Domain.{_model.Name.Singular.Value}> query, ref DataSourceRequest request);");
                sb.AppendLine();
                sb.AppendLine($"{i2}partial void GetItemQuery(ref IQueryable<Domain.{_model.Name.Singular.Value}> query);");
            }
            else
            {
                sb.AppendLine();
                sb.AppendLine($"{i2}public {_model.Name.Singular.Value}ReadService({_module.Project.Singular.Value}DbContext context)");
                sb.AppendLine($"{i2}{{");
                sb.AppendLine($"{i3}_context = context ?? throw new ArgumentNullException(nameof(context));");
                sb.AppendLine($"{i2}}}");
            }
            sb.AppendLine();
            sb.AppendLine($"{i2}protected async override Task<DataSourceResult> GetAllAsync(DataSourceRequest request)");
            sb.AppendLine($"{i2}{{");
            if (Settings.SupportRegen)
            {
                sb.AppendLine($"{i3}IQueryable<Domain.{_model.Name.Singular.Value}> query = _context.{_model.Name.Plural.Value};");
                sb.AppendLine($"{i3}GetAllQuery(ref query, ref request);");
            }
            else
            {
                sb.Append($"{i3}var query = _context.{_model.Name.Plural.Value}");
                foreach (var item in _model.Relationships)
                {
                    item.GetOther(_model.Name, out var type, out var model, out var field);
                    switch (type)
                    {
                        case RelationShipTypes.Zero:
                        case RelationShipTypes.One:
                            sb.Append($".Include(a => a.{model.Singular.Value})");
                            break;
                        case RelationShipTypes.Many:
                            sb.Append($".Include(a => a.{model.Plural.Value})");
                            break;
                        default:
                            break;
                    }
                }
                sb.AppendLine(";");
            }
            sb.AppendLine();
            sb.AppendLine($"{i3}return await query.ToDataSourceResultAsync(request, s => s.ToListResponse());");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}protected async override Task<Domain.{_model.Name.Singular.Value}> GetItemAsync(string businessKey)");
            sb.AppendLine($"{i2}{{");
            if (Settings.SupportRegen)
            {
                sb.AppendLine($"{i3}IQueryable<Domain.{_model.Name.Singular.Value}> query = _context.{_model.Name.Plural.Value};");
                sb.AppendLine($"{i3}GetItemQuery(ref query);");
            }
            else
            {
                sb.Append($"{i3}var query = _context.{_model.Name.Plural.Value}");
                foreach (var item in _model.Relationships)
                {
                    item.GetOther(_model.Name, out var type, out var model, out var field);
                    switch (type)
                    {
                        case RelationShipTypes.Zero:
                        case RelationShipTypes.One:
                            sb.Append($".Include(a => a.{model.Singular.Value})");
                            break;
                        case RelationShipTypes.Many:
                            sb.Append($".Include(a => a.{model.Plural.Value})");
                            break;
                        default:
                            break;
                    }
                }
                sb.AppendLine(";");
            }
            sb.AppendLine();
            sb.AppendLine($"{i3}return TryConvertBusinessKey(businessKey, out var id)");
            sb.AppendLine($"{i4}? await query.SingleOrDefaultAsync(e => e.Id == id)");
            sb.Append($"{i4}: ");
            var bk = _model.HasBusinessKey();
            if (bk != null)
                sb.AppendLine($"await query.FirstOrDefaultAsync(s => s.{bk.Name.Singular.Value} == businessKey);");
            else
                sb.AppendLine("throw new NotSupportedException(\"Business Key must be a Unique Identifier.\");");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}protected async override Task<bool> ExistsAsync(string businessKey)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}return TryConvertBusinessKey(businessKey, out var id)");
            sb.AppendLine($"{i4}? await _context.{_model.Name.Plural.Value}.CountAsync(e => e.Id == id) > 0");
            sb.Append($"{i4}: ");
            if (bk != null)
                sb.AppendLine($"await _context.{_model.Name.Plural.Value}.CountAsync(s => s.{bk.Name.Singular.Value} == businessKey) > 0;");
            else
                sb.AppendLine("throw new NotSupportedException(\"Business Key must be a Unique Identifier.\");");
            sb.AppendLine($"{i2}}}");
            if (!Settings.SupportRegen)
            {
                sb.AppendLine();
                sb.AppendLine($"{i2}protected override Task<IEnumerable<Domain.{_model.Name.Singular.Value}>> GetAllAsync()");
                sb.AppendLine($"{i2}{{");
                sb.AppendLine($"{i3}throw new NotImplementedException();");
                sb.AppendLine($"{i2}}}");
            }
            sb.AppendLine($"{i1}}}");
            sb.AppendLine("}");

            var name = $"{_model.Name.Singular.Value}ReadService";
            if (Settings.SupportRegen)
                name += ".generated";
            return new File { Name = $"{name}.cs", Content = sb.ToString(), CanOverwrite = Settings.SupportRegen };
        }
    }
}