using System;
using System.Text;
using Modeller;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace BusinessService
{
    internal class EditServiceGenerated : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;

        public EditServiceGenerated(ISettings settings, Module module, Model model)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            if (!_model.IsEntity())
                return null;

            var i1 = h.Indent(1);
            var i2 = h.Indent(2);
            var i3 = h.Indent(3);
            var i4 = h.Indent(4);

            var sb = new StringBuilder();
            sb.AppendLine(((ISnippet)new Header.Generator(Settings, new GeneratorDetails()).Create()).Content);
            sb.AppendLine("using Microsoft.AspNetCore.JsonPatch;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Threading;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine($"using {_module.Namespace}.Business.Extensions;");
            sb.AppendLine($"using {_module.Namespace}.Data;");
            sb.AppendLine("using Jbssa.Core.Interfaces;");
            sb.AppendLine("using Jbssa.Core.Extensions;");
            sb.AppendLine("using Jbssa.Core.BusinessLogic;");
            if(Settings.SupportRegen)
            {
                sb.AppendLine("using Microsoft.EntityFrameworkCore.ChangeTracking;");
            }
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Business.Services");
            sb.AppendLine("{");
            sb.Append($"{i1}");
            if (Settings.SupportRegen)
                sb.Append("partial ");
            else
                sb.Append("public ");
            sb.Append($"class {_model.Name.Singular.Value}EditService");
            if (!Settings.SupportRegen)
                sb.Append($" : EditableServiceAsyncBase<Domain.{_model.Name.Singular.Value}>");
            sb.AppendLine();
            sb.AppendLine($"{i1}{{");
            sb.AppendLine($"{i2}private readonly IReadableAsync<Domain.{_model.Name.Singular.Value}> _readService;");
            sb.AppendLine($"{i2}private readonly {_module.Project.Singular.Value}DbContext _context;");
            if (Settings.SupportRegen)
            {
                sb.AppendLine();
                sb.AppendLine($"{i2}partial void Add(ref EntityEntry<Domain.{_model.Name.Singular.Value}> entity);");
                sb.AppendLine($"{i2}partial void Create(ref Domain.{_model.Name.Singular.Value} entity);");
                sb.AppendLine($"{i2}partial void Delete(ref Domain.{_model.Name.Singular.Value} entity);");
                sb.AppendLine($"{i2}partial void Patch(ref Domain.{_model.Name.Singular.Value} entity);");
                sb.AppendLine($"{i2}partial void Update(ref Domain.{_model.Name.Singular.Value} entity);");
            }
            else
            {
                sb.AppendLine();
                sb.AppendLine($"{i2}public {_model.Name.Singular.Value}EditService({_module.Project.Singular.Value}DbContext context, IReadableAsync<Domain.{_model.Name.Singular.Value}> readService)");
                sb.AppendLine($"{i2}{{");
                sb.AppendLine($"{i3}_context = context ?? throw new ArgumentNullException(nameof(context));");
                sb.AppendLine($"{i3}_readService = readService ?? throw new ArgumentNullException(nameof(readService));");
                sb.AppendLine($"{i2}}}");
            }
            sb.AppendLine();
            sb.AppendLine($"{i2}protected async override Task<Domain.{_model.Name.Singular.Value}> AddAsync(Domain.{_model.Name.Singular.Value} entity)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}if (entity == null)");
            sb.AppendLine($"{i4}throw new ArgumentNullException(nameof(entity));");
            sb.AppendLine($"{i3}var tracker = await _context.AddAsync(entity);");
            if (Settings.SupportRegen)
            {
                sb.AppendLine($"{i3}Add(ref tracker);");
            }
            sb.AppendLine($"{i3}return tracker.Entity;");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}protected override Task<Domain.{_model.Name.Singular.Value}> CreateAsync()");
            sb.AppendLine($"{i2}{{");
            if (Settings.SupportRegen)
            {
                sb.AppendLine($"{i3}var entity = Domain.{_model.Name.Singular.Value}.Create();");
                sb.AppendLine($"{i3}Create(ref entity);");
            }
            sb.AppendLine($"{i3}return Task.FromResult(entity);");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}protected async override Task DeleteAsync(Domain.{_model.Name.Singular.Value} entity)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}if (entity == null)");
            sb.AppendLine($"{i4}throw new ArgumentNullException(nameof(entity));");
            sb.AppendLine($"{i3}var item = await _readService.GetItem(entity.Id.ToString());");
            sb.AppendLine($"{i3}if (item == null)");
            sb.AppendLine($"{i4}return;");
            sb.AppendLine($"{i3}item.Version = entity.Version;");
            if (Settings.SupportRegen)
            {
                sb.AppendLine($"{i3}Delete(ref item);");
            }
            sb.AppendLine($"{i3}_context.Remove(item);");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}protected async override Task<Domain.{_model.Name.Singular.Value}> PatchAsync(string businessKey, JsonPatchDocument<Domain.{_model.Name.Singular.Value}> document)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}if (businessKey.IsNullOrWhiteSpace())");
            sb.AppendLine($"{i4}throw new ArgumentNullException(nameof(businessKey));");
            sb.AppendLine($"{i3}if (document == null)");
            sb.AppendLine($"{i4}throw new ArgumentNullException(nameof(document));");
            sb.AppendLine($"{i3}var originalModel = await _readService.GetItem(businessKey);");
            sb.AppendLine($"{i3}if (originalModel == null)");
            sb.AppendLine($"{i4}return null;");
            sb.AppendLine();
            sb.AppendLine($"{i3}document.ApplyTo(originalModel);");
            if (Settings.SupportRegen)
            {
                sb.AppendLine($"{i3}Patch(ref originalModel);");
            }
            sb.AppendLine();
            sb.AppendLine($"{i3}return originalModel;");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}protected async override Task<int> SaveAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}return await _context.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}protected async override Task<Domain.{_model.Name.Singular.Value}> UpdateAsync(string businessKey, Domain.{_model.Name.Singular.Value} entity)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}if (businessKey.IsNullOrWhiteSpace())");
            sb.AppendLine($"{i4}throw new ArgumentNullException(nameof(businessKey));");
            sb.AppendLine($"{i3}if (entity == null)");
            sb.AppendLine($"{i4}throw new ArgumentNullException(nameof(entity));");
            sb.AppendLine($"{i3}var originalModel = await _readService.GetItem(businessKey);");
            sb.AppendLine($"{i3}if (originalModel == null)");
            sb.AppendLine($"{i4}return null;");
            sb.AppendLine();
            sb.AppendLine($"{i3}originalModel.UpdateUsing(entity);");
            if (Settings.SupportRegen)
            {
                sb.AppendLine($"{i3}Update(ref originalModel);");
            }
            sb.AppendLine();
            sb.AppendLine($"{i3}return originalModel;");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine($"{i1}}}");
            sb.AppendLine("}");

            var name = $"{_model.Name.Singular.Value}EditService";
            if (Settings.SupportRegen)
                name += ".generated";
            return new File { Name = $"{name}.cs", Content = sb.ToString(), CanOverwrite = Settings.SupportRegen };
        }
    }
}