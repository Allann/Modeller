using System;
using System.Text;
using Modeller;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace ApiOption
{
    public class Generator : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;

        public Generator(ISettings settings, Module module, Model model)
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

            var sb = new StringBuilder();
            sb.AppendLine("using Jbssa.Core.Interfaces;");
            sb.AppendLine("using Jbssa.Core.Mvc;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Api.Options");
            sb.AppendLine("{");
            sb.AppendLine($"{i1}public class {_model.Name.Singular.Value}Options : EntityOptionsAsync<Domain.{_model.Name.Singular.Value}>, IHasListProvider<Domain.{_model.Name.Singular.Value}>");
            sb.AppendLine($"{i1}{{");

            sb.AppendLine($"{i2}private readonly IListProvider<Domain.{_model.Name.Singular.Value}> _listProvider;");
            sb.AppendLine($"{i2}public {_model.Name.Singular.Value}Options(IReadableAsync<Domain.{_model.Name.Singular.Value}> readService, IEditableAsync<Domain.{_model.Name.Singular.Value}> editService, IListProvider<Domain.{_model.Name.Singular.Value}> listProvider)");
            sb.AppendLine($"{i3}: base(readService, editService)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}_listProvider = listProvider;");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}IListProvider<Domain.{_model.Name.Singular.Value}> IHasListProvider<Domain.{_model.Name.Singular.Value}>.GetListProvider()");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}return _listProvider;");
            sb.AppendLine($"{i2}}}");

            sb.AppendLine($"{i1}}}");
            sb.AppendLine("}");

            return new File { Name = $"{_model.Name.Singular.Value}Options.cs", Content = sb.ToString(), CanOverwrite = Settings.SupportRegen };
        }
    }
}