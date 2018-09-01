using System;
using System.Linq;
using Modeller;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebViews
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

        public IOutput Create()
        {
            var output = new FileGroup
            {
                Path = System.IO.Path.Combine("Views", _model.Name.Plural.Value)
            };

            if (_model.IsEntity())
            {
                output.AddFile((IFile)new IndexView(Settings, _module, _model).Create());
                output.AddFile((IFile)new DetailView(Settings, _module, _model).Create());
                output.AddFile((IFile)new CreateView(Settings, _module, _model).Create());
                output.AddFile((IFile)new EditView(Settings, _module, _model).Create());
            }

            foreach (var model in _module.Models)
            {
                if (model == _model)
                    continue;

                var relationships = model.Relationships.Where(r => r.RightModel.Singular.Value == _model.Name.Singular.Value || r.LeftModel.Singular.Value == _model.Name.Singular.Value);
                foreach (var relationship in relationships)
                {
                    relationship.GetOther(_model.Name, out var otherType, out var otherModel, out var otherField);
                    if (otherType != RelationShipTypes.Many)
                        continue;

                    var om = _module.Models.SingleOrDefault(m => m.Name.Equals(otherModel));
                    if (!om.IsEntity())
                        continue;

                    relationship.GetMatch(_model.Name, out var matchType, out var matchField);
                    var file = (IFile)new ChildView(Settings, _module, _model, om, otherField).Create();
                    output.AddFile(file);

                    var content = (IFile)new ContentPartialView(Settings, _module, _model).Create();
                    output.AddFile(content);
                    var info = (IFile)new InfoPartialView(Settings, _module, _model).Create();
                    output.AddFile(info);
                }
            }

            return output;
        }

        public ISettings Settings { get; }
    }
}