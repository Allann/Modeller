using Core.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;

namespace Core.CmdLine
{
    /// <summary>
    ///     Represents a property that can be set via the command-line.
    /// </summary>
    public class CmdLineProperty
    {
        /// <summary>
        ///     Creates an instance of a CmdLineProperty.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="prop"></param>
        public CmdLineProperty(CmdLineObject obj, PropertyDescriptor prop)
            : this(obj, prop, null)
        {
        }

        /// <summary>
        ///     Creates an instance of a CmdLineProperty.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="prop"></param>
        /// <param name="claAtt"></param>
        internal CmdLineProperty(CmdLineObject obj, PropertyDescriptor prop, CmdLineArgAttribute claAtt)
        {
            Object = obj;
            _property = prop;
            DefaultValue = Value;
            Validators = new List<ICustomValidator>();
            ShowDefaultValue = prop.PropertyType == typeof (string) ? !string.IsNullOrEmpty((string) Value) : true;

            if (claAtt == null)
            {
                Required = false;
                Usage = "";
                ShowInUsage = DefaultBoolean.Default;
                AllowSave = true;
                Aliases = new string[] {};
            }
            else
            {
                Required = claAtt.Required;
                Usage = claAtt.Usage;
                ShowInUsage = claAtt.ShowInUsage;
                AllowSave = claAtt.AllowSave;
                Aliases = claAtt.Aliases;
            }

            var reqAtt = prop.GetAttribute<RequiredAttribute>();
            if (reqAtt != null) Required = true;
        }

        private readonly PropertyDescriptor _property;

        /// <summary>
        ///     Gets the command-line object associated with this property.
        /// </summary>
        public CmdLineObject Object { get; private set; }

        /// <summary>
        ///     The name of the command-line property.
        /// </summary>
        public string Name
        {
            get { return _property.Name; }
        }

        /// <summary>
        ///     Gets the description associated with the property.
        /// </summary>
        public string Description
        {
            get { return _property.Description; }
        }

        /// <summary>
        ///     Gets the type of the property.
        /// </summary>
        public Type PropertyType
        {
            get { return _property.PropertyType; }
        }

        /// <summary>
        ///     Gets the aliases associated with this property.
        /// </summary>
        public string[] Aliases { get; private set; }

        /// <summary>
        ///     Gets or sets a value that determines if this command-line argument is required.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        ///     Gets or sets the short description that should be used in the usage description.
        /// </summary>
        public string Usage { get; set; }

        /// <summary>
        ///     Gets or sets a value that determines if the argument should be displayed in the usage. By default, only required
        ///     arguments and help are displayed in the usage in order to save space when printing the usage.
        /// </summary>
        public DefaultBoolean ShowInUsage { get; set; }

        /// <summary>
        ///     Gets a value that determines if this property was set through the command-line or not.
        /// </summary>
        public bool PropertySet { get; private set; }

        /// <summary>
        ///     Gets the default value for this property. Used in the command-line help description.
        /// </summary>
        public object DefaultValue { get; private set; }

        /// <summary>
        ///     Gets or sets the current value for this property.
        /// </summary>
        public object Value
        {
            get => _property.GetValue(Object);
            set
            {
                if (value is string[] strs)
                {
                    if (_property.PropertyType.IsArray)
                    {
                        try
                        {
                            value = strs.Convert(PropertyType.GetElementType());
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.GetDetails());
                            Error = $"[{string.Join(", ", strs)}] is not valid. The value must be able to convert to an array of {PropertyType.GetElementType().GetCSharpName()}.";
                            return;
                        }
                    }
                    else
                    {
                        try
                        {
                            value = ConvertEx.ChangeType(strs[0], PropertyType);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.GetDetails());
                            if (PropertyType.IsEnum)
                            {
                                Error =
                                    $"'{strs[0]}' is not valid. The argument must be one of these values: [{string.Join(", ", Enum.GetValues(PropertyType))}].";
                            }
                            else
                            {
                                var typeName = PropertyType.GetCSharpName();
                                Error = $"'{strs[0]}' is not valid. The argument must be a {typeName}.";
                            }
                            return;
                        }
                    }
                }
                _property.SetValue(Object, ConvertEx.ChangeType(value, PropertyType));
                PropertySet = true;
            }
        }

        /// <summary>
        ///     Gets or sets a value that determines if the default value should be displayed to the user in the usage.
        /// </summary>
        public bool ShowDefaultValue { get; set; }

        /// <summary>
        ///     Gets or sets a value that determines if the property should be saved.
        /// </summary>
        public bool AllowSave { get; set; }

        /// <summary>
        ///     Gets any errors associated with this property.
        /// </summary>
        public string Error { get; private set; }

        /// <summary>
        ///     Additional custom validators. Useful for adding validation outside of attributes.
        /// </summary>
        public IList<ICustomValidator> Validators { get; private set; }

        /// <summary>
        ///     Gets the textual representation of this command-line object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var value = new StringBuilder();
            if (Value is IEnumerable vals)
            {
                var first = true;
                foreach (var val in vals)
                {
                    if (!first)
                        value.Append(", ");
                    first = false;
                    value.Append(ConvertEx.ToString(val));
                }
            }
            else
                value.Append(ConvertEx.ToString(Value));
            return $"{Name}=[{value}]";
        }

        internal ValidationAttribute[] GetValidationAtts()
        {
            return _property.GetAttributes<ValidationAttribute>();
        }
    }
}