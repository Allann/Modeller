# Modeller
Code generation via a model is made easier with the Modeller tool.  There are 3 distinct parts to this tool.

- Modeller (this project) - provides the core functionality, the business logic so to speak.
- Modeller.Generators - this is a solution of many projects that are code generators for a specific output type.
- Modeller.UI - a group of projects each providing a client for a user.
  - Modeller.CLI - Command Line interface
  - Modeller.Wpf - Various controls to be used in the VSIX package
  - Modeller.Vsix - Intergration components for Vsiual Studio 2017
