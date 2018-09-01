using Core.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Core.CmdLine
{
    /// <summary>
    ///     A list of CmdLineProperty objects.
    /// </summary>
    public class CmdLinePropertyList : IEnumerable<CmdLineProperty>
    {
        /// <summary>
        ///     Creates an instance of CmdLinePropertyList.
        /// </summary>
        public CmdLinePropertyList(CmdLineObject obj)
        {
            _compare = obj.Options.Comparer;
            _propertyDictionary = CreateDictionary(obj.Options.Comparer);
            Object = obj;
            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(obj))
            {
                var claAtt = prop.GetAttribute<CmdLineArgAttribute>();
                if (claAtt == null) continue;
                var cmdLineProp = new CmdLineProperty(obj, prop, claAtt);
                _properties.Add(cmdLineProp);
                Add(prop.Name, cmdLineProp);
                foreach (var alias in cmdLineProp.Aliases)
                    Add(alias, cmdLineProp);
            }
        }

        private Dictionary<string, CmdLineProperty> CreateDictionary(StringComparison compare)
        {
            switch (compare)
            {
                case StringComparison.CurrentCulture:
                    return new Dictionary<string, CmdLineProperty>(StringComparer.CurrentCulture);
                case StringComparison.CurrentCultureIgnoreCase:
                    return new Dictionary<string, CmdLineProperty>(StringComparer.CurrentCultureIgnoreCase);
                case StringComparison.InvariantCulture:
                    return new Dictionary<string, CmdLineProperty>(StringComparer.InvariantCulture);
                case StringComparison.InvariantCultureIgnoreCase:
                    return new Dictionary<string, CmdLineProperty>(StringComparer.InvariantCultureIgnoreCase);
                case StringComparison.Ordinal:
                    return new Dictionary<string, CmdLineProperty>(StringComparer.Ordinal);
                case StringComparison.OrdinalIgnoreCase:
                    return new Dictionary<string, CmdLineProperty>(StringComparer.OrdinalIgnoreCase);
                default:
                    throw new ArgumentException($"The comparison type '{compare}' is not supported.");
            }
        }

        private readonly StringComparison _compare;
        private readonly List<CmdLineProperty> _properties = new List<CmdLineProperty>();
        private readonly Dictionary<string, CmdLineProperty> _propertyDictionary;

        /// <summary>
        ///     Gets the command-line property associated with this argument.
        /// </summary>
        /// <param name="argName">This can be the shortcut, full property name, or a partial property name that is unique.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Thrown when the command-line property cannot be found.</exception>
        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public CmdLineProperty this[string argName]
        {
            get
            {
                if (string.IsNullOrEmpty(argName)) return null;

                // Honor exact matches.
                if (_propertyDictionary.ContainsKey(argName))
                    return _propertyDictionary[argName];

                // Search for a command-line property that starts with this name.
                var foundProps = new List<CmdLineProperty>();
                foreach (var prop in this)
                {
                    if (prop.Name.StartsWith(argName, _compare))
                        if (!foundProps.Contains(prop)) foundProps.Add(prop);
                    foreach (var alias in prop.Aliases)
                        if (alias.StartsWith(argName, _compare))
                            if (!foundProps.Contains(prop)) foundProps.Add(prop);
                }

                if (foundProps.Count == 0) return null;
                if (foundProps.Count == 1) return foundProps[0];
                // Multiple properties were found. We cannot process this argument.
                throw new AmbiguousCmdLineNameException(argName, foundProps.ToArray());
            }
        }

        /// <summary>
        ///     Gets the number of properties in the list.
        /// </summary>
        public int Count
        {
            get { return _properties.Count; }
        }

        /// <summary>
        ///     Gets the command-line object for this list.
        /// </summary>
        public CmdLineObject Object { get; private set; }

        /// <summary>
        ///     Adds a command-line property to the list keyed to the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="prop"></param>
        public void Add(string name, CmdLineProperty prop)
        {
            if (_propertyDictionary.ContainsKey(name))
                throw new CmdLineArgumentException($"The command-line name '{name}' is already defined.");
            _propertyDictionary.Add(name, prop);
        }

        /// <summary>
        ///     Gets the enumerator for the list.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<CmdLineProperty> GetEnumerator()
        {
            return _properties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}