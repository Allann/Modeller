using Hy.Modeller.Interfaces;
using System;

namespace Hy.Modeller.Generator
{
    internal class ModuleValidator : ValidatorBase
    {
        private readonly IModuleLoader _loader;

        public ModuleValidator(Context context, IModuleLoader loader) : base(context)
        {
            _loader = loader ?? throw new ArgumentNullException(nameof(loader));
        }

        public override void Validate()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Context.ModuleFile))
                {
                    AddIssue("Missing the model file path and name.");
                    return;
                }

                var module = _loader.Load(Context.ModuleFile);

                // check to see if we actually did get a module, even though no errors occured.
                if (module == null)
                {
                    AddIssue($"Unable to load the module from file {Context.ModuleFile}");
                }
                else
                {
                    Context.SetModule(module);
                }
            }
            catch (Exception ex)
            {
                AddIssue(ex.Message);
            }
        }
    }
}
