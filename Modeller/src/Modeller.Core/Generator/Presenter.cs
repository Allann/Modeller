using Modeller.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Modeller.Generator
{
    public class Presenter
    {
        private readonly Action<string, bool> _output;

        public Presenter(string folder, string target, Action<string, bool> output)
        {
            _output = output ?? throw new ArgumentNullException(nameof(output));
            if (string.IsNullOrWhiteSpace(folder))
            {
                folder = Defaults.LocalFolder;
            }
            Folder = folder;

            if (string.IsNullOrWhiteSpace(target))
            {
                target = Defaults.Target;
            }
            Target = target;
        }

        public string Folder { get; }
        public string Target { get; }

        private IEnumerable<IEnumerable<string>> Process(string folder, bool verbose)
        {
            var rows = new List<List<string>>();

            var generators = FileHelper.GetAvailableGenerators(folder);
            foreach (var generator in generators)
            {
                var abbr = FileHelper.GetAbbreviatedFilename(generator.FilePath);
                var m = generator.Metadata;
                if (verbose)
                {
                    rows.Add(new List<string> { abbr, m.Name, m.Version.ToString(), m.Description });
                }
                else
                {
                    rows.Add(new List<string> { abbr, m.Name, m.Version.ToString() });
                }
            }

            if (rows.Any())
            {
                var cols = verbose ? 3 : 2;
                var widths = new List<int>(cols);
                for (var col = 0; col < cols; col++)
                {
                    widths.Add(rows.Max(l => l[col].Length));
                }
                foreach (var row in rows)
                {
                    for (var col = 0; col < cols; col++)
                    {
                        row[col] = row[col].PadRight(widths[col]) + " | ";
                    }
                }
            }

            return rows;
        }

        public void Display(bool verbose = false)
        {
            var folder = System.IO.Path.Combine(Folder, Target);
            _output?.Invoke("Available generators", true);
            _output?.Invoke($"  location: {folder}", true);

            var table = Process(folder, verbose);
            foreach (var row in table)
            {
                foreach (var cell in row)
                {
                    _output?.Invoke(cell, false);
                }
                _output?.Invoke("", true);
            }
        }

    }
}
