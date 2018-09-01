using System;
using System.Text;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class Constants : IGenerator
    {
        private readonly Module _module;

        public Constants(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new StringBuilder();

            sb.AppendLine("using System.Globalization;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Web");
            sb.AppendLine("{");
            sb.AppendLine("    public static class Constants");
            sb.AppendLine("    {");
            sb.AppendLine("        public static string LongDateFormatPattern = CultureInfo.CurrentCulture.DateTimeFormat.LongDatePattern;");
            sb.AppendLine("        public static string DateFormatPattern = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;");
            sb.AppendLine("        public static string LongDateTimeFormatPattern = $\"{CultureInfo.CurrentCulture.DateTimeFormat.LongDatePattern} {CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern}\";");
            sb.AppendLine("        public static string DateTimeFormatPattern = $\"{CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern} {CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern}\";");
            sb.AppendLine("        public const string DateFormatVm = \"{0:d}\";");
            sb.AppendLine("        public const string DateFormatSpecifierVm = \"d\";");
            sb.AppendLine("        public const string LongDateFormatVm = \"{0:D}\";");
            sb.AppendLine("        public const string LongDateFormatSpecifierVm = \"D\";");
            sb.AppendLine("        public const string ShortDateTimeFormatVm = \"{0:g}\";");
            sb.AppendLine("        public const string ShortDateTimeFormatSpecifierVm = \"g\";");
            sb.AppendLine("        public const string LongDateTimeFormatVm = \"{0:G}\";");
            sb.AppendLine("        public const string PercentFormatVm = \"{0:p0}\";");
            sb.AppendLine("        public const string IntegerFormatVm = \"{0:N0}\";");
            sb.AppendLine("        public const string DecimalFormatVm = \"{0:N2}\";");
            sb.AppendLine("        public const string IsActiveTemplate = \"#if (data.IsActive) { # <i class='fa fa-check' aria-hidden='true'></i> # } #\";");
            sb.AppendLine("        public const string IsUserValueTemplate = \"# if (data.IsUserValue) { # <i class='fa fa-check' aria-hidden='true'></i> # } #\";");
            sb.AppendLine("        public const string IsRoleValueTemplate = \"# if (data.IsRoleValue) { # <i class='fa fa-check' aria-hidden='true'></i> # } #\";");
            sb.AppendLine("        public const string IsResourceValueTemplate = \"# if (data.IsResourceValue) { # <i class='fa fa-check' aria-hidden='true'></i> # } #\";");
            sb.AppendLine("        public const string EnabledTemplate = \"# if (data.Enabled) { # <i class='fa fa-check' aria-hidden='true'></i> # } #\";");
            sb.AppendLine("        public const string ExpiredTemplate = \"# if (data.To<DateTimeOffset.Now) { # <i class='fa fa-check' aria-hidden='true'></i> # } #\";");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    public static class KnownApiRoutes");
            sb.AppendLine("    {");
            foreach (var model in _module.Models)
            {
                sb.AppendLine($"        public static class {model.Name.Plural.Value}");
                sb.AppendLine("        {");
                sb.AppendLine($"            public static string Name = \"{model.Name.Plural.Value}\";");
                sb.AppendLine("            public static string Lookup = \"Lookup\";");
                sb.AppendLine("            public static string Create = \"Create\";");
                sb.AppendLine("        }");
                sb.AppendLine();
            }
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    public static class KnownEntities");
            sb.AppendLine("    {");
            foreach (var model in _module.Models)
            {
                sb.AppendLine($"        public static class {model.Name.Singular.Value}");
                sb.AppendLine("        {");
                sb.AppendLine($"            public static string DisplayName = \"{model.Name.Singular.Display}\";");
                sb.AppendLine("        }");
                sb.AppendLine();
            }
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    public static class KnownMessageFor");
            sb.AppendLine("    {");
            sb.AppendLine("        public static class Exceptions");
            sb.AppendLine("        {");
            sb.AppendLine("            public const string MissingBaseUri = \"BaseUri has not been set in configuration.\";");
            sb.AppendLine("            public static string NotFound(string displayName, string id) => $\"{displayName} '{id}' was not found\";");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public static class WebController");
            sb.AppendLine("        {");
            sb.AppendLine("            public static string Creating(string displayName) => $\"Creating a new {displayName}\";");
            sb.AppendLine("            public static string Created(string displayName) => $\"{displayName} created\";");
            sb.AppendLine("            public static string Editing(string displayName, string id) => $\"Editing {displayName} '{id}'\";");
            sb.AppendLine("            public static string Edited(string displayName) => $\"{displayName} updated\";");
            sb.AppendLine("            public static string Fetching(string displayName, string id) => $\"Fetching {displayName} '{id}'\";");
            sb.AppendLine("            public static string Deleting(string displayName, string id) => $\"Deleting {displayName} '{id}'\";");
            sb.AppendLine("            public static string Deleted(string displayName) => $\"{displayName} deleted\";");
            sb.AppendLine();
            sb.AppendLine("            public static string CreateFailed(string displayName) => $\"Failed to create the {displayName}\";");
            sb.AppendLine("            public static string EditFailed(string displayName, string id) => $\"Failed to edit the {displayName} '{id}'\";");
            sb.AppendLine("            public static string DeleteFailed(string displayName, string id) => $\"Failed to delete the {displayName} '{id}'\";");
            sb.AppendLine("            public static string FindFailed(string displayName, string id) => $\"Failed to find the {displayName} '{id}'\";");
            sb.AppendLine("            public static string FindFailed(string displayName, string id, string message) => $\"Failed to find the {displayName} '{id}'. {message}\";");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return new File() { Name = "Constants.cs", Content = sb.ToString() };
        }
    }
}