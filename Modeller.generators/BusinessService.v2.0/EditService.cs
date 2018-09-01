using System;
using System.Text;
using Modeller;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace BusinessClass
{
    internal class EditService : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;

        public EditService(ISettings settings, Module module, Model model)
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

            var sb = new StringBuilder();
            sb.AppendLine("using Microsoft.AspNetCore.JsonPatch;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Threading;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine($"using {_module.Namespace}.Business.Extensions;");
            sb.AppendLine($"using {_module.Namespace}.Data;");
            sb.AppendLine("using Jbssa.Core.Interfaces;");
            sb.AppendLine("using Jbssa.Core.Extensions;");
            sb.AppendLine("using Jbssa.Core.BusinessLogic;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Business.Services");
            sb.AppendLine("{");
            sb.AppendLine($"{i1}public class {_model.Name.Singular.Value}EditService : EditableServiceAsyncBase<Domain.{_model.Name.Singular.Value}>");
            sb.AppendLine($"{i1}{{");

            sb.AppendLine($"{i2}private readonly IReadableAsync<Domain.{_model.Name.Singular.Value}> _readService;");
            sb.AppendLine($"{i2}private readonly {_module.Project.Singular.Value}DbContext _context;");
            sb.AppendLine();
            sb.AppendLine($"{i2}public {_model.Name.Singular.Value}EditService({_module.Project.Singular.Value}DbContext context, IReadableAsync<Domain.{_model.Name.Singular.Value}> readService)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}_context = context ?? throw new ArgumentNullException(nameof(context));");
            sb.AppendLine($"{i3}_readService = readService ?? throw new ArgumentNullException(nameof(readService));");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}protected async override Task<Domain.{_model.Name.Singular.Value}> AddAsync(Domain.{_model.Name.Singular.Value} entity)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i2}    if (entity == null)");
            sb.AppendLine($"{i2}        throw new ArgumentNullException(nameof(entity));");
            sb.AppendLine($"{i2}    var tracker = await _context.AddAsync(entity);");
            sb.AppendLine($"{i2}    return tracker.Entity;");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}protected override Task<Domain.{_model.Name.Singular.Value}> CreateAsync()");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i2}    return Task.FromResult(Domain.{_model.Name.Singular.Value}.Create());");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}protected async override Task DeleteAsync(Domain.{_model.Name.Singular.Value} entity)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i2}    if (entity == null)");
            sb.AppendLine($"{i2}        throw new ArgumentNullException(nameof(entity));");
            sb.AppendLine($"{i2}    var item = await _readService.GetItem(entity.Id.ToString());");
            sb.AppendLine($"{i2}    if (item == null)");
            sb.AppendLine($"{i2}        return;");
            sb.AppendLine($"{i2}    item.Version = entity.Version;");
            sb.AppendLine($"{i2}    _context.Remove(item);");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}protected async override Task<Domain.{_model.Name.Singular.Value}> PatchAsync(string businessKey, JsonPatchDocument<Domain.{_model.Name.Singular.Value}> document)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i2}    if (businessKey.IsNullOrWhiteSpace())");
            sb.AppendLine($"{i2}        throw new ArgumentNullException(nameof(businessKey));");
            sb.AppendLine($"{i2}    if (document == null)");
            sb.AppendLine($"{i2}        throw new ArgumentNullException(nameof(document));");
            sb.AppendLine($"{i2}    var originalModel = await _readService.GetItem(businessKey);");
            sb.AppendLine($"{i2}    if (originalModel == null)");
            sb.AppendLine($"{i2}        return null;");
            sb.AppendLine();
            sb.AppendLine($"{i2}    document.ApplyTo(originalModel);");
            sb.AppendLine();
            sb.AppendLine($"{i2}    return originalModel;");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}protected async override Task<int> SaveAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i2}    return await _context.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}protected async override Task<Domain.{_model.Name.Singular.Value}> UpdateAsync(string businessKey, Domain.{_model.Name.Singular.Value} entity)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i2}    if (businessKey.IsNullOrWhiteSpace())");
            sb.AppendLine($"{i2}        throw new ArgumentNullException(nameof(businessKey));");
            sb.AppendLine($"{i2}    if (entity == null)");
            sb.AppendLine($"{i2}        throw new ArgumentNullException(nameof(entity));");
            sb.AppendLine($"{i2}    var originalModel = await _readService.GetItem(businessKey);");
            sb.AppendLine($"{i2}    if (originalModel == null)");
            sb.AppendLine($"{i2}        return null;");
            sb.AppendLine();
            sb.AppendLine($"{i2}originalModel.UpdateUsing(entity);");
            sb.AppendLine($"{i2}");
            sb.AppendLine($"{i2}return originalModel;");
            sb.AppendLine("}");
            sb.AppendLine($"{i1}}}");
            sb.AppendLine("}");

            return new File { Name = $"{_model.Name.Singular.Value}EditService.cs", Content = sb.ToString() };
        }
    }
}