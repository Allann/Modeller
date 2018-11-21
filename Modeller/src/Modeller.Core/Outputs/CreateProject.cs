using Hy.Modeller.Interfaces;
using System;

namespace Hy.Modeller.Outputs
{
    internal class CreateProject
    {
        private readonly IProject _project;
        private readonly Action<string> _output;

        public CreateProject(IProject project, Action<string> output)
        {
            _project = project ?? throw new ArgumentNullException(nameof(project));
            _output = output;
        }

        internal void Create(string basePath)
        {
            var path = System.IO.Path.IsPathRooted(_project.Path) ? _project.Path : System.IO.Path.Combine(basePath, _project.Path);

            _output?.Invoke($"Path:    {_project.Path}");
            _output?.Invoke($"Project: {_project.Name}");
            _output?.Invoke($"     Id: {_project.Id}");

            foreach (var fg in _project.FileGroups)
            {
                var groupPath = string.IsNullOrWhiteSpace(fg.Path) ? path : System.IO.Path.Combine(path, fg.Path);
                foreach (var file in fg.Files)
                {
                    var filePath = (string.IsNullOrWhiteSpace(file.Path) || groupPath.Contains(file.Path)) ? groupPath : System.IO.Path.Combine(groupPath, file.Path);
                    file.Path = filePath;

                    var fileOutput = new CreateFile(file, _output);
                    fileOutput.Create(_project.Path);
                }
            }
        }
    }
}