using System;
using System.Diagnostics;
using System.IO;

namespace Core.Utilities
{
    internal static class FileUtility
    {
        /// <summary>
        /// Removes a directory as best as it can. Errors are ignored.
        /// </summary>
        /// <param name="dirPath"></param>
        public static void RemoveDirectory(string dirPath)
        {
            foreach (var childDirPath in Directory.GetDirectories(dirPath))
                RemoveDirectory(childDirPath);

            foreach (var filePath in Directory.GetFiles(dirPath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Unable to delete " + filePath + ": " + ex.Message);
                }
            }

            try
            {
                Directory.Delete(dirPath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to delete " + dirPath + ": " + ex.Message);
            }
        }
    }
}
