using System;
using System.Text;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace Property
{
    public enum PropertyScope
    {
        @public,
        @protected,
        @internal,
        @private,
        notAvalable
    }

    public class Generator : IGenerator
    {
        private readonly Field _field;
        private readonly int _indent;
        private readonly bool _useDataAnnotations;
        private readonly bool _useBackingField;
        private readonly PropertyScope _getScope;
        private readonly PropertyScope _setScope;

        public Generator(Field field, int indent = 2, bool useDataAnnotations = true, bool useBackingField = false, PropertyScope getScope = PropertyScope.@public, PropertyScope setScope = PropertyScope.@public)
        {
            _field = field ?? throw new ArgumentNullException(nameof(field));
            if (indent < 1)
            {
                indent = 1;
            }
            _indent = indent;
            _useDataAnnotations = useDataAnnotations;
            _useBackingField = useBackingField;
            _getScope = getScope;
            _setScope = setScope;
        }

        public bool ShowNullable { get; set; } = true;

        public bool GuidNullable { get; set; } = false;

        public ISettings Settings => throw new NotImplementedException();

        public IOutput Create()
        {
            var dt = h.DataType(_field, ShowNullable, GuidNullable);
            var sb = new StringBuilder();

            if (_useBackingField)
            {
                sb.AppendLine($"{h.Indent(_indent)}private {dt} {_field.Name.Singular.ModuleVariable}");
            }

            var display = _field.Name.Singular.Display;
            if (_field.Name.Singular.Value != display)
            {
                if (display.EndsWith(" Id"))
                    display = display.Substring(0, display.Length - 3);
                if(_useDataAnnotations)
                    sb.AppendLine($"{ h.Indent(_indent)}[Display(Name = \"{display}\")]");
            }
            if (_field.DataType == DataTypes.String)
            {
                if (!_field.Nullable && _useDataAnnotations)
                {
                    sb.AppendLine(h.Indent(_indent) + "[Required]");
                }
                if (_field.MaxLength.HasValue && _useDataAnnotations)
                {
                    sb.Append(h.Indent(_indent) + $"[StringLength({_field.MaxLength.Value}");
                    if (_field.MinLength.HasValue)
                    {
                        sb.Append($", MinimumLength={_field.MinLength.Value}");
                    }
                    sb.AppendLine(")]");
                }
            }

            var ga = _getScope != PropertyScope.notAvalable;
            var sa = _setScope != PropertyScope.notAvalable;
            var gs = _getScope.ToString() + " ";
            var ss = _setScope.ToString() + " ";
            var rs = "";
            if (_getScope == _setScope || !ga || !sa)
            {
                rs = gs;
                gs = "";
                ss = "";
            }
            if (rs.Trim() == PropertyScope.notAvalable.ToString())
                return new Snippet(string.Empty);

            sb.Append($"{h.Indent(_indent)}{rs}{dt} {_field.Name.Singular.Value}");
            if (_useBackingField)
            {
                sb.AppendLine();
                sb.AppendLine(h.Indent(_indent) + "{");
                if (ga)
                {
                    sb.AppendLine($"{h.Indent(_indent + 1)}{gs}get");
                    sb.AppendLine(h.Indent(_indent + 1) + "{");
                    sb.AppendLine($"{h.Indent(_indent + 2)}return {_field.Name.Singular.ModuleVariable};");
                    sb.AppendLine(h.Indent(_indent + 1) + "}");
                }
                if (sa)
                {
                    sb.AppendLine($"{h.Indent(_indent + 1)}{ss}set");
                    sb.AppendLine(h.Indent(_indent + 1) + "{");
                    sb.AppendLine($"{h.Indent(_indent + 2)}{_field.Name.Singular.ModuleVariable} = value;");
                    sb.AppendLine(h.Indent(_indent + 1) + "}");
                }
                sb.AppendLine(h.Indent(_indent) + "}");
            }
            else
            {
                sb.Append(" { ");
                if (ga)
                {
                    sb.Append($"{gs}get; ");
                }
                if (sa)
                {
                    sb.Append($"{ss}set; ");
                }
                sb.AppendLine("}");
            }
            return new Snippet(sb.ToString());
        }
    }
}