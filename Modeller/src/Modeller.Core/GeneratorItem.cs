using Hy.Modeller.Interfaces;
using System;
using System.IO;

namespace Hy.Modeller
{
    public class GeneratorItem
    {
        private IGenerator _instance;

        public GeneratorItem(IMetadata metadata, string filePath, Type generatorType)
        {
            Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            Type = generatorType ?? throw new ArgumentNullException(nameof(generatorType));
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("File path must be provided", nameof(filePath));
            }
            if(!File.Exists(filePath))
            {
                throw new FileNotFoundException("Generator not found.", filePath);
            }
            FilePath = filePath;
            AbbreviatedFileName = FileHelper.GetAbbreviatedFilename(filePath);
        }

        public string AbbreviatedFileName { get; }

        public IMetadata Metadata { get; }

        public Type Type { get; }

        public string FilePath { get; }

        public IGenerator Instance(params object[] args)
        {
            if (_instance == null)
            {
                if (Activator.CreateInstance(Type, args) is IGenerator g)
                    _instance = g;
            }
            return _instance;
        }
    }
}
