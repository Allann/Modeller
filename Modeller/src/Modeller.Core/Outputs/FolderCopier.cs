using System;
using Modeller.Interfaces;

namespace Modeller.Outputs
{
    internal class FolderCopier
    {
        private readonly IFolderCopy _fc;
        private readonly bool _overwrite;
        private readonly Action<string> _output;

        public FolderCopier(IFolderCopy fc, bool overwrite, Action<string> output)
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
                var s = new System.IO.DirectoryInfo(_fc.Source);
                if (!s.Exists)
                    return;

                var d = new System.IO.DirectoryInfo(_fc.Destination);
                if (!_overwrite && d.Exists)
                {
                    _output.Invoke($"Copy: {_fc.Source} skipped.");
                }
                else
                {
                    if (!d.Exists)
                        d.Create();
                    foreach (var file in s.GetFiles())
                    {
                        file.CopyTo(System.IO.Path.Combine(d.FullName, file.Name), _overwrite);
                    }
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