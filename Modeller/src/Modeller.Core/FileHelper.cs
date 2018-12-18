using Hy.Modeller.GeneratorBase;
using Hy.Modeller.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Hy.Modeller
{
    public static class FileHelper
    {
        public static string GetAbbreviatedFilename(string filePath)
        {
            if (filePath == null)
                return null;

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
            Trace.Indent();
            foreach (var subFolder in folder.GetDirectories())
            {
                AddFiles(list, subFolder);
            }

            var asmLoader = new GeneratorLoader(folder.FullName);
            foreach (var file in folder.GetFiles("*.dll"))
            {
                Trace.WriteLine($"Filename: {file.FullName}");
                var ass = asmLoader.Load(file.FullName);
                Trace.WriteLine($"Assembly: {ass.FullName}");
                var dt = ass.DefinedTypes;
                Trace.WriteLine($"Defined Types: {dt.Count()}");
                Trace.Indent();
                foreach (var t in dt)
                {
                    Trace.WriteLine($"Type: {t.FullName}");
                    Trace.Indent();
                    foreach (var i in t.ImplementedInterfaces)
                    {
                        Trace.WriteLine($"Interface: {i.FullName}");
                    }
                    Trace.Unindent();
                }
                Trace.Unindent();
                var metaDataTypes = dt.Where(t => t.ImplementedInterfaces.Any(it => it.FullName == "Hy.Modeller.Interfaces.IMetadata"));
                Trace.WriteLine($"Matched Types: {metaDataTypes.Count()}");
                Trace.Indent();
                foreach (var t in metaDataTypes)
                {
                    Trace.WriteLine($"Type: {t.FullName}");
                }
                Trace.Unindent();

                foreach (var type in metaDataTypes)
                {
                    var obj = Activator.CreateInstance(type);
                    if (obj == null)
                        continue;

                    var instance = obj as IMetadata;
                    if (instance != null)
                    {
                        var entryPoint = instance.EntryPoint;
                        if (entryPoint == null)
                        {
                            continue;
                        }
                        list.Add(new GeneratorItem(instance, file.FullName, entryPoint));
                    }
                    else
                    {
                        var name = type.GetProperty("Name").GetValue(obj).ToString();
                        var description = type.GetProperty("Description").GetValue(obj).ToString();
                        var entryPoint = type.GetProperty("EntryPoint").GetValue(obj) as Type;
                        var subGenerators = type.GetProperty("SubGenerators").GetValue(obj) as IEnumerable<Type>;
                        var version = type.GetProperty("Version").GetValue(obj) as Version;

                        var md = new TempGeneratorDetail(name, description, entryPoint, subGenerators, version);
                        list.Add(new GeneratorItem(md, file.FullName, entryPoint));
                    }
                }
            }
            Trace.Unindent();
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

        private class TempGeneratorDetail : IMetadata
        {
            public TempGeneratorDetail(string name, string description, Type entryPoint, IEnumerable<Type> subGenerators, Version version)
            {
                Name = name;
                Description = description;
                EntryPoint = entryPoint;
                SubGenerators = subGenerators;
                Version = version;
            }
            public Version Version { get; }
            public string Name { get; }
            public string Description { get; }
            public Type EntryPoint { get; }
            public IEnumerable<Type> SubGenerators { get; }
        }
    }
}