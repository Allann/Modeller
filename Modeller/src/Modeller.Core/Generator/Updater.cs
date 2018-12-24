using System;
using System.IO;

namespace Hy.Modeller.Generator
{
    public class Updater
    {
        private readonly Action<string> _output;
        private int _affected;

        public Updater(string server = null, string local = null, string target = null, bool overwrite = false, Action<string> output = null, bool verbose = false)
        {
            if (string.IsNullOrWhiteSpace(local))
            {
                local = Defaults.LocalFolder;
            }
            LocalFolder = local;

            if (string.IsNullOrWhiteSpace(server))
            {
                server = Defaults.ServerFolder;
            }
            ServerFolder = server;

            if (string.IsNullOrWhiteSpace(target))
            {
                target = Defaults.Target;
            }
            Target = target;
            Overwrite = overwrite;
            _output = output;
            Verbose = verbose;
        }

        public string LocalFolder { get; }
        public string ServerFolder { get; }
        public string Target { get; }
        public bool Overwrite { get; }
        public bool Verbose { get; }

        public void Refresh()
        {
            _affected = 0;
            _output?.Invoke($"Updating generator files for target: {Target}");
            _output?.Invoke($"Server Folder: {ServerFolder}");
            _output?.Invoke($"Local Folder: {LocalFolder}");
            _output?.Invoke($"Overwrite: {Overwrite}");

            var server = Path.Combine(ServerFolder + "", Target);
            var local = Path.Combine(LocalFolder + "", Target);

            if (UpdateLocalGenerators())
            {
                _output?.Invoke($"Update completed successfully. Files affected: {_affected}");
            }
            else
            {
                _output?.Invoke($"Update failed. Files affected: {_affected}");
            }
        }

        private bool UpdateLocalGenerators()
        {
            var server = new DirectoryInfo(ServerFolder);
            var local = new DirectoryInfo(LocalFolder);

            if (!server.Exists)
            {
                _output?.Invoke($"Server Folder '{server.FullName}' not found.");
                return false;
            }

            DirectoryCopy(server, local);
            return true;
        }

        private void DirectoryCopy(DirectoryInfo sourceDirName, DirectoryInfo destDirName)
        {
            if (!sourceDirName.Exists)
            {
                return;
            }

            var dirs = sourceDirName.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!destDirName.Exists)
            {
                if (Verbose)
                    _output?.Invoke($"creating {destDirName.FullName}");
                destDirName.Create();
            }

            // Get the files in the directory and copy them to the new location.
            var files = sourceDirName.GetFiles();
            foreach (var file in files)
            {
                var temppath = Path.Combine(destDirName.FullName, file.Name);
                if (File.Exists(temppath) && !Overwrite)
                {
                    if (Verbose)
                        _output?.Invoke($"skipping {file.Name}");
                    continue;
                }

                if (Verbose)
                    _output?.Invoke($"copying {file.Name} to {destDirName.Name}");
                file.CopyTo(temppath, Overwrite);
                _affected++;
            }

            // copy Sub-directories and their contents to new location.
            foreach (var subdir in dirs)
            {
                var temppath = new DirectoryInfo(Path.Combine(destDirName.FullName, subdir.Name));
                DirectoryCopy(subdir, temppath);
            }
        }
    }
}