using System;
using System.IO;
using System.Linq;

namespace Hy.Modeller.Generator
{
    internal class GeneratorValidator : ValidatorBase
    {
        public GeneratorItem Generator { get; private set; }

        public GeneratorValidator(Context context)
            : base(context)
        {
        }

        public override void Validate()
        {
            try
            {
                if (!ValidateGeneratorFolder())
                    return;
                ValidateGenerator();

                if (Generator != null)
                {
                    Context.SetGenerator(Generator);
                }
            }
            catch (Exception ex)
            {
                AddIssue(ex.Message);
            }
        }

        private bool ValidateGeneratorFolder()
        {
            if (!Directory.Exists(Context.Folder))
            {
                AddIssue($"Local folder not found '{Context.Folder}'");
                return false;
            }
            return true;
        }

        private void ValidateGenerator()
        {
            if (Context.GeneratorName == null)
                return;

            var folder = Path.Combine(Context.Folder, Context.Target);
            if (!Directory.Exists(folder))
            {
                AddIssue($"No target folder named '{Context.Target}' was found in '{Context.Folder}'.");
                return;
            }

            var name = Context.GeneratorName.ToLowerInvariant();

            var generators = FileHelper.GetAvailableGenerators(folder);
            var matches = generators.Where(g => g.Metadata.Name.ToLowerInvariant() == name || g.AbbreviatedFileName.ToLowerInvariant() == name);
            if (!matches.Any())
            {
                AddIssue($"No matching generator exists for the name: '{Context.GeneratorName}'.");
                return;
            }

            var exact = matches.SingleOrDefault(m => m.Metadata.Version == Context.Version);
            Generator = exact ?? matches.OrderByDescending(k => k.Metadata.Version).First();

            if (Generator == null)
                AddIssue($"Failed to return the requested generator '{Context.Target}.{Context.GeneratorName}'");
        }
    }
}
