using Hy.Modeller.GeneratorBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ClassLibrary
{
    public class GeneratorDetails : MetadataBase
    {
        public override string Name => "Class";

        public override string Description => "Create a class file";

        public override Type EntryPoint => typeof(Generator);

        public override IEnumerable<Type> SubGenerators => new Collection<Type>() { };
    }
}