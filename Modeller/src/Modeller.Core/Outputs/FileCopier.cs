using System;
using Hy.Modeller.Interfaces;

namespace Hy.Modeller.Outputs
{
    internal class FileCopier
    {
        private readonly IFileCopy _fc;
        private readonly bool _overwrite;
        private readonly Action<string> _output;

        public FileCopier(IFileCopy fc, bool overwrite, Action<string> output)
        {
            _fc = fc ?? throw new ArgumentNullException(nameof(fc));
            _overwrite = overwrite;
            _output = output;
        }

        internal void Copy(string basePath)
        {
            if (!System.IO.Path.IsPathRooted(_fc.Destination))
            {
                _fc.Destination = !string.IsNullOrWhiteSpace(_fc.Destination) ? System.IO.Path.Combine(basePath, _fc.Destination) : basePath;
            }

            try
            {
                if (!_overwrite && System.IO.File.Exists(_fc.Destination))
                    _output.Invoke($"Copy: {_fc.Source} skipped.");
                else
                {
                    System.IO.File.Copy(_fc.Source, _fc.Destination, _overwrite);
                    _output.Invoke($"Copy: {_fc.Source} -> {_fc.Destination} - success");
                }
            }
            catch (Exception ex)
            {
                _output.Invoke($"Copy: {_fc.Source} -> {_fc.Destination} - failed. {ex.Message}");
            }
        }
    }
}