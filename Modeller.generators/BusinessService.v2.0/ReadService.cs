using System;
using System.Text;
using Modeller;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace BusinessClass
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
            var i4 = h.Indent(4);

            var sb = new StringBuilder();
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
            sb.AppendLine($"{i1}public class {_model.Name.Singular.Value}ReadService : ReadonlyServiceAsyncBase<Domain.{_model.Name.Singular.Value}>");
            sb.AppendLine($"{i1}{{");
            sb.AppendLine($"{i2}private readonly {_module.Project.Singular.Value}DbContext _context;");
            sb.AppendLine();
            sb.AppendLine($"{i2}public {_model.Name.Singular.Value}ReadService({_module.Project.Singular.Value}DbContext context)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}_context = context ?? throw new ArgumentNullException(nameof(context));");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}private IQueryable<Domain.{_model.Name.Singular.Value}> Query()");
            sb.AppendLine($"{i2}{{");
            sb.Append($"{i3}return _context.{_model.Name.Plural.Value}");
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
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}protected async override Task<DataSourceResult> GetAllAsync(DataSourceRequest request)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}return await Query().ToDataSourceResultAsync(request, s => s.ToListResponse());");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}protected async override Task<Domain.{_model.Name.Singular.Value}> GetItemAsync(string businessKey)");
            sb.AppendLine($"{i2}{{");
            sb.Append($"{i4}var query = _context.{_model.Name.Plural.Value}");
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
            sb.AppendLine($"{i3}if(TryConvertBusinessKey(businessKey, out Guid id))");
            sb.AppendLine($"{i4}return await query.SingleOrDefaultAsync(e => e.Id == id);");
            sb.AppendLine($"{i3}else");
            var bk = _model.HasBusinessKey();
            if (bk != null)
            {
                sb.AppendLine($"{i4}return await query.FirstOrDefaultAsync(s => s.{bk.Name.Singular.Value} == businessKey);");
            }
            else
            {
                sb.AppendLine($"{i4}throw new NotSupportedException(\"Business Key must be a Unique Identifier.\");");
            }
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}protected async override Task<bool> ExistsAsync(string businessKey)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}if(TryConvertBusinessKey(businessKey, out Guid id))");
            sb.AppendLine($"{i4}return await _context.{_model.Name.Plural.Value}.CountAsync(e => e.Id == id) > 0;");
            sb.AppendLine($"{i3}else");
            if (bk != null)
            {
                sb.AppendLine($"{i4}return await _context.{_model.Name.Plural.Value}.CountAsync(s => s.{bk.Name.Singular.Value} == businessKey) > 0;");
            }
            else
            {
                sb.AppendLine($"{i4}throw new NotSupportedException(\"Business Key must be a Unique Identifier.\");");
            }
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}protected override Task<IEnumerable<Domain.{_model.Name.Singular.Value}>> GetAllAsync()");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}throw new NotImplementedException();");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine($"{i1}}}");
            sb.AppendLine("}");

            return new File { Name = $"{_model.Name.Singular.Value}ReadService.cs", Content = sb.ToString() };
        }
    }
}