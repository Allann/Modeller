using Hy.Modeller.Interfaces;
using System;

namespace Hy.Modeller.Outputs
{
    internal class CreateSnippet
    {
        private readonly ISnippet _snippet;
        private readonly Action<string> _output;

        public CreateSnippet(ISnippet snippet, Action<string> output = null)
        {
            _snippet = snippet ?? throw new ArgumentNullException(nameof(snippet));
            _output = output;
        }

        public void Create(string basePath)
        {
            var outputPath = basePath;

            string filename;
            if (string.IsNullOrWhiteSpace(_snippet.Name))
            {
                filename = System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetTempFileName());
            }
            else
                filename = _snippet.Name;
            var ext = System.IO.Path.GetExtension(filename);
            if (string.IsNullOrEmpty(ext) || ext != ".txt")
                filename += ".txt";

            var f = new Outputs.File
            {
                Path = System.IO.Path.Combine(outputPath, "Snippets"),
                Name = filename,
                Content = _snippet.Content
            };

            var fileOutput = new CreateFile(f, _output);
            fileOutput.Create(basePath);
        }
    }

}
