using System;
using Modeller.Interfaces;

namespace Modeller.Outputs
{
    internal class CreateFileGroup
    {
        private readonly IFileGroup _fileGroup;
        private readonly Action<string> _output;

        public CreateFileGroup(IFileGroup fileGroup, Action<string> output)
        {
            _fileGroup = fileGroup ?? throw new ArgumentNullException(nameof(fileGroup));
            _output = output;
        }

        internal void Create(string basePath)
        {
            if (string.IsNullOrWhiteSpace(_fileGroup.Path))
                _fileGroup.Path = basePath;
            else if (!System.IO.Path.IsPathRooted(_fileGroup.Path))
                _fileGroup.Path = System.IO.Path.Combine(basePath, _fileGroup.Path);

            _output?.Invoke($"Filegroup: {_fileGroup.Name}");
            foreach (var file in _fileGroup.Files)
            {
                var fileOutput = new CreateFile(file, _output);
                fileOutput.Create(_fileGroup.Path);
            }
        }
    }
}