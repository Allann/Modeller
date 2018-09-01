using Core.Extensions;
using Core.Utilities;
using System;
using System.IO;
using System.Reflection;

namespace Core
{
    /// <summary>
    /// Primary class for application information and plugin support.
    /// </summary>
    public sealed class Application
    {
        static Application()
        {
            var asm = Assembly.GetEntryAssembly();

            if (asm == null)
                return;

            var titleAtt = asm.GetAttribute<AssemblyTitleAttribute>();
            if (titleAtt != null)
                Title = titleAtt.Title;

            var descAtt = asm.GetAttribute<AssemblyDescriptionAttribute>();
            if (descAtt != null)
                Description = descAtt.Description;

            var companyAtt = asm.GetAttribute<AssemblyCompanyAttribute>();
            if (companyAtt != null)
                Company = companyAtt.Company;

            var productAtt = asm.GetAttribute<AssemblyProductAttribute>();
            if (productAtt != null)
                Product = productAtt.Product;

            var copyrightAtt = asm.GetAttribute<AssemblyCopyrightAttribute>();
            if (copyrightAtt != null)
                Copyright = copyrightAtt.Copyright;

            var trademarkAtt = asm.GetAttribute<AssemblyTrademarkAttribute>();
            if (trademarkAtt != null)
                Trademark = trademarkAtt.Trademark;

            Version = asm.GetName().Version;

            var uri = new Uri(asm.EscapedCodeBase);
            ExePath = uri.Scheme == "file" ? uri.LocalPath + uri.Fragment : uri.ToString();

            ExeName = Path.GetFileName(ExePath);
        }

        /// <summary>
        /// Private to enforce the Singleton pattern.
        /// </summary>
        private Application() { }

        /// <summary>
        /// Gets the title of the executing assembly from AssemblyTitleAttribute.
        /// </summary>
        public static string Title { get; private set; }

        /// <summary>
        /// Gets the version of the executing assembly.
        /// </summary>
        public static Version Version { get; private set; }

        /// <summary>
        /// Gets the description of the executing assembly from AssemblyDescriptionAttribute.
        /// </summary>
        public static string Description { get; private set; }

        /// <summary>
        /// Gets the company name of the executing assembly from AssemblyCompanyAttribute.
        /// </summary>
        public static string Company { get; private set; }

        /// <summary>
        /// Gets the product name of the executing assembly from AssemblyProductAttribute.
        /// </summary>
        public static string Product { get; private set; }

        /// <summary>
        /// Gets the copyright of the executing assembly from AssemblyCopyrightAttribute.
        /// </summary>
        public static string Copyright { get; private set; }

        /// <summary>
        /// Gets the trademark of the executing assembly from AssemblyTrademarkAttribute.
        /// </summary>
        public static string Trademark { get; private set; }

        /// <summary>
        /// Gets the path the the executing assembly.
        /// </summary>
        public static string ExePath { get; private set; }

        /// <summary>
        /// Gets the just the name of the exe (without the extension).
        /// </summary>
        public static string ExeName { get; private set; }

        /// <summary>
        /// Returns an absolute path relative to the ExePath.
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public static string GetPath(string relativePath)
        {
            var dirPath = Path.GetDirectoryName(ExePath);
            return Path.Combine(dirPath, relativePath);
        }

        /// <summary>
        /// Gets the path to the temporary directory for this application. This is a subdirectory off of the system temp directory.
        /// </summary>
        /// <returns></returns>
        public static string GetTempPath()
        {
            var tempPath = Path.GetTempPath();
            if (!string.IsNullOrEmpty(ExeName))
                tempPath = Path.Combine(tempPath, ExeName);

            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);

            return tempPath;
        }

        /// <summary>
        /// Removes the temp directory for this application.
        /// </summary>
        public static void CleanTempDirectory()
        {
            var tempPath = GetTempPath();
            if (!Directory.Exists(tempPath))
                return;
            FileUtility.RemoveDirectory(tempPath);
        }
    }
}
