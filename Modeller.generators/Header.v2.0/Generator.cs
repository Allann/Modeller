using System;
using Modeller.Interfaces;
using Modeller.Outputs;

namespace Header
{
    public class Generator : IGenerator
    {
        private readonly IMetadata _metadata;

        public Generator(ISettings settings, IMetadata metadata)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"// Auto-generated using Modeller template '{_metadata.Name}' version {_metadata.Version}");
            return new Snippet(sb.ToString());
        }
    }
}
