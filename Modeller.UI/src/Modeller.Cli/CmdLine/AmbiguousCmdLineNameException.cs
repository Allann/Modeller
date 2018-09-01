using System.Collections.Generic;

namespace Core.CmdLine
{
    /// <summary>
    /// Exception thrown when multiple command-line properties match a given argument name.
    /// </summary>
    public class AmbiguousCmdLineNameException
        : CmdLineException
    {

        /// <summary>
        /// Creates an instance of AmbiguousCmdLineNameException.
        /// </summary>
        /// <param name="argName"></param>
        /// <param name="props"></param>
        public AmbiguousCmdLineNameException(string argName, params CmdLineProperty[] props)
            : base($"The command-line argument name '{argName}' matches the following command-line properties: {string.Join(", ", GetPropertyNames(props))}. You must disambiguate the argument name by using either the shortcut or include additional characters in the name.")
        {
            ArgName = argName;
            AmbiguousProperties = props;
        }

        /// <summary>
        /// Gets the name of the invalid argument.
        /// </summary>
        public string ArgName { get; private set; }

        /// <summary>
        /// Gets the conflicting properties.
        /// </summary>
        public CmdLineProperty[] AmbiguousProperties { get; private set; }

        private static string[] GetPropertyNames(params CmdLineProperty[] props)
        {
            var names = new List<string>();
            foreach (var prop in props)
                names.Add(prop.Name);
            return names.ToArray();
        }
    }
}
