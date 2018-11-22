using Hy.Modeller.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Hy.Modeller
{
    public static class FileHelper
    {
        public static string GetAbbreviatedFilename(string filePath)
        {
            var filename = Path.GetFileNameWithoutExtension(filePath);
            var idx = filename.IndexOf('.');
            if (idx > -1)
            {
                filename = filename.Substring(0, idx);
            }
            return filename;
        }

        public static void Write(this IFile file, bool overwrite = false)
        {
            var dir = new DirectoryInfo(file.Path);
            var filename = Path.Combine(dir.FullName, file.Name);
            if (!dir.Exists)
            {
                Directory.CreateDirectory(dir.FullName);
                File.WriteAllText(filename, file.Content);
            }
            else
            {
                var existing = new FileInfo(filename);
                if (existing.Exists)
                {
                    if (overwrite)
                    {
                        File.WriteAllText(filename, file.Content);
                    }
                }
                else
                {
                    File.WriteAllText(filename, file.Content);
                }
            }
        }

        public static IEnumerable<GeneratorItem> GetAvailableGenerators(string localFolder = null)
        {
            if (string.IsNullOrWhiteSpace(localFolder))
            {
                localFolder = Defaults.LocalFolder;
            }
            var local = new DirectoryInfo(localFolder);

            var list = new List<GeneratorItem>();
            if (!local.Exists)
            {
                return list;
            }

            AddFiles(list, local);
            return list;
        }

        private static void AddFiles(List<GeneratorItem> list, DirectoryInfo folder)
        {
            foreach (var subFolder in folder.GetDirectories())
            {
                AddFiles(list, subFolder);
            }

            var asmLoader = new AssemblyLoader(folder.FullName);
            foreach (var file in folder.GetFiles("*.dll"))
            {
                var asm = asmLoader.Load(file.FullName);
                var types = asm.GetExportedTypes().Where(t => t.IsClass);

                var md = types.FirstOrDefault(t => t.GetInterface("IMetadata") != null);
                if (md == null)
                {
                    continue;
                }

                if (Activator.CreateInstance(md) is IMetadata i)
                {
                    if (i.EntryPoint == null)
                    {
                        continue;
                    }

                    list.Add(new GeneratorItem(i, file.FullName, i.EntryPoint));
                }
            }
        }

        public static bool UpdateLocalGenerators(string serverFolder = null, string localFolder = null, bool overwrite = false, Action<string> output = null)
        {
            if (string.IsNullOrWhiteSpace(localFolder))
            {
                localFolder = Defaults.LocalFolder;
            }
            //if (string.IsNullOrWhiteSpace(serverFolder))
            //{
            //    serverFolder = Defaults.ServerFolder;
            //}

            var server = new DirectoryInfo(serverFolder);
            var local = new DirectoryInfo(localFolder);

            if (!server.Exists)
            {
                return false;
            }

            DirectoryCopy(server, local, true, overwrite, output);
            return true;
        }

        private static void DirectoryCopy(DirectoryInfo sourceDirName, DirectoryInfo destDirName, bool copySubDirs, bool overwrite, Action<string> output)
        {
            if (!sourceDirName.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
            }

            var dirs = sourceDirName.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!destDirName.Exists)
            {
                output?.Invoke($"creating {destDirName.FullName}");
                destDirName.Create();
            }

            // Get the files in the directory and copy them to the new location.
            var files = sourceDirName.GetFiles();
            foreach (var file in files)
            {
                var temppath = Path.Combine(destDirName.FullName, file.Name);
                if (File.Exists(temppath) && !overwrite)
                {
                    continue;
                }

                output?.Invoke($"copying {file.Name} to {destDirName.Name}");
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (var subdir in dirs)
                {
                    var temppath = new DirectoryInfo(Path.Combine(destDirName.FullName, subdir.Name));
                    DirectoryCopy(subdir, temppath, copySubDirs, overwrite, output);
                }
            }
        }
    }
}