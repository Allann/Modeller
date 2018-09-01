using Modeller.Interfaces;
using System;

namespace Modeller.Generator
{
    internal class SettingsValidator : ValidatorBase
    {
        private readonly ISettingsLoader _loader;

        public SettingsValidator(Context context, ISettingsLoader loader)
            : base(context)
        {
            _loader = loader ?? throw new ArgumentNullException(nameof(loader));
        }

        public override void Validate()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Context.SettingsFile))
                {
                    Context.SetSettings(new Settings());
                    return;
                }

                var settings = _loader.Load<Settings>(Context.SettingsFile);
                if (settings == null)
                {
                    Context.SetSettings(new Settings());
                }
                else
                {
                    Context.SetSettings(settings);
                }
            }
            catch (Exception ex)
            {
                AddIssue(ex.Message);
            }
        }
    }
}
