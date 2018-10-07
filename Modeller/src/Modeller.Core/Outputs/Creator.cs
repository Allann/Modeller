using Modeller.Generator;
using Modeller.Interfaces;
using System;

namespace Modeller.Outputs
{
    public class Creator
    {
        private readonly Action<string> _output;
        private readonly bool _verbose;

        public Creator(Context context, Action<string> output = null, bool verbose = false)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            _output = output;
            _verbose = verbose;
        }

        public Context Context { get; }

        public void Create(IOutput output)
        {
            if (output == null)
                return;

            var outputPath = Context.OutputPath;
            if (string.IsNullOrWhiteSpace(outputPath))
                outputPath = Defaults.OutputFolder;

            _output?.Invoke("Generation finished, writing output.");
            _output?.Invoke($"Output: {outputPath}");

            if (output is ISnippet x)
            {
                var snippet = new CreateSnippet(x, _output);
                snippet.Create(outputPath);
            }
            else if (output is IFile f)
            {
                var file = new CreateFile(f, _output);
                file.Create(outputPath);
            }
            else if (output is IFileGroup fg)
            {
                var fileGroup = new CreateFileGroup(fg, _output);
                fileGroup.Create(outputPath);
            }
            else if (output is IProject p)
            {
                var project = new CreateProject(p, _output);
                project.Create(outputPath);
            }
            else if (output is ISolution s)
            {
                var solution = new CreateSolution(s, _output);
                solution.Create(outputPath);
            }
            else if (output is IFileCopy fc)
            {
                var copier = new FileCopier(fc, false, _output);
                copier.Copy(outputPath);
            }
            else if (output is IFolderCopy fo)
            {
                var copier = new FolderCopier(fo, false, _output);
                copier.Copy(outputPath);
            }
            else
                _output?.Invoke($"Output type '{output?.GetType().FullName}' is not supported.");

            _output?.Invoke("Generation complete.");
        }
    }
}