using Hy.Modeller.Interfaces;
using System;

namespace Hy.Modeller.Outputs
{
    internal class CreateSolution
    {
        private readonly ISolution _solution;
        private readonly Action<string> _output;

        public CreateSolution(ISolution solution, Action<string> output)
        {
            _solution = solution ?? throw new ArgumentNullException(nameof(solution));
            _output = output;
        }

        internal void Create(string basePath)
        {
            if (!System.IO.Path.IsPathRooted(_solution.Directory))
                _solution.Directory = System.IO.Path.Combine(basePath, _solution.Directory);

            _output?.Invoke($"Project: {_solution.Name}");

            foreach (var file in _solution.Files)
            {
                var fileOutput = new CreateFile(file, _output);
                fileOutput.Create(_solution.Directory);
            }

            foreach (var project in _solution.Projects)
            {
                var projectOutput = new CreateProject(project, _output);
                projectOutput.Create(_solution.Directory);
            }
        }
    }

}
