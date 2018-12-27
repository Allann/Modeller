# Modeller
Code generation via a model is made easy with the Modeller global tool and a few generator components.

## Benefits:
- No need to have or learn/use yeoman, node or js to generate code.
- Code First generation, i.e. no need to create a database first.
- Versioned templates. 

Packages available on [NuGet.org](https://www.nuget.org/packages?q=hy.modeller) include:
- [Hy.Modeller.Base](https://www.nuget.org/packages/Hy.Modeller.Base/) - defines the models, structure and rules for the module definition. 
- [Hy.Modeller.Core](https://www.nuget.org/packages/Hy.Modeller.Core/) - defines the components that make up the code generator.
- [Hy.Modeller.Fluent](https://www.nuget.org/packages/Hy.Modeller.Fluent/) - a fluent library allowing developers to create a module definition through a fluent language construct (see example below).
- [Hy.Modeller.Tool](https://www.nuget.org/packages/Hy.Modeller.Tool/) - a [dotnet global tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) that can generate the code using an existing module definition file.

## Generators
The modeller tool can't do much without generator packages.  These packages must be installed locally on the developers computer to be able to generate code.  