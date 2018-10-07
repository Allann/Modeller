using System.Linq;

namespace Modeller.Generator
{
    internal class ContextValidator : ValidatorBase
    {
        public ContextValidator(Context context)
            : base(context)
        {
        }

        public override void Validate()
        {
            if (Context.Module == null)
            {
                AddIssue("Context Module not defined.");
            }
            if (Context.Generator == null)
            {
                AddIssue("Context Generator not defined.");
            }
            if (Context.Settings == null)
            {
                AddIssue("Context Settings not defined.");
            }

            if (!string.IsNullOrWhiteSpace(Context.ModelName))
            {
                var model = Context.Module.Models.FirstOrDefault(m => m.Name.Singular.Value.ToLowerInvariant() == Context.ModelName.ToLowerInvariant());
                if (model == null)
                {
                    AddIssue($"Unable to locate model {Context.ModelName}");
                }
                else
                {
                    Context.SetModel(model);
                }
            }

            if (string.IsNullOrWhiteSpace(Context.OutputPath))
            {
                Context.SetOutputPath(Defaults.OutputFolder);
            }
            else if (!System.IO.Path.IsPathRooted(Context.OutputPath))
            {
                AddIssue($"Output path '{Context.OutputPath}' is not an absolute path.");
            }
            else
                Context.SetOutputPath(Context.OutputPath);
        }
    }
}
