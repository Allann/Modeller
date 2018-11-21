using Hy.Modeller.Models;
using System;
using System.Collections.Generic;

namespace Hy.Modeller.Generator
{
    public class CodeGenerator
    {
        private readonly Context _context;
        private readonly Action<string> _output;
        private readonly bool _verbose;

        public CodeGenerator(Context context, Action<string> output, bool verbose = false)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _output = output ?? throw new ArgumentNullException(nameof(output));
            _verbose = verbose;
        }

        public Interfaces.IOutput Create()
        {
            if (_verbose)
            {
                _output($"Module: {_context.Module.Namespace}");
                _output($"Supports regen: {_context.Settings.SupportRegen}");
                _output($"Generator: {_context.Generator.Metadata.Name}");
            }

            var type = _context.Generator.Metadata.EntryPoint;
            var cis = type.GetConstructors();
            if (cis.Length != 1)
            {
                _output("Modeller only supports single public constructors for a generator.");
                return null;
            }
            var ci = cis[0];
            var args = new List<object>();
            foreach (var p in ci.GetParameters())
            {
                if (p.ParameterType == typeof(Interfaces.ISettings))
                {
                    args.Add(_context.Settings);
                }
                else if (p.ParameterType == typeof(Module))
                {
                    args.Add(_context.Module);
                }
                else if (p.ParameterType == typeof(Model))
                {
                    args.Add(_context.Model);
                }
                else
                {
                    _output($"{p.ParameterType.ToString()} is not a supported argument type on Generator constructors.");
                    return null;
                }
            }

            var generator = ci.Invoke(args.ToArray()) as Interfaces.IGenerator;
            return generator.Create();
        }
    }
}
