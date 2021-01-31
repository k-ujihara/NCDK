NCDK: The Chemistry Development Kit ported to C&#35;
===============================================

NCDK is .NET port of [the Chemistry Development Project (CDK)](https://github.com/cdk/cdk). Functionality is provided for many areas in cheminformatics.

NCDK is covered under LGPL v2.1. The modules are free and open-source and is easily integrated with other open-source or in-house projects.

The current release is based on [CDK 2021-01-31](https://github.com/cdk/cdk/tree/a027f6689a34d9ede3bb9befad2d0f1c5b2bb711) snapshot.

Getting Started
---------------

Most of codes are written in C&#35;. You can learn NCDK from CDK documents, NCDKTests and NCDKDisplayTests project.

* NCDK -- Core module of NCDK.
* NCDK.Display -- Depiction module of NCDK. It is based on WPF.
* NCDK.RDF --- RDF functions.
* NCDK.Legacy --- Legacy modules.
* NCDK.Tests -- Unit tests for NCDK.
* NCDK.DisplayTests -- Unit tests for NCDK.Display.
* NCDK.RDFTests --- Unit test for NCDK.RDF.
* NCDK.LegacyTests --- Unit test for NCDK.Legacy.
* MolViewer -- MOL file viewer, which can depict SMILES. It includes WPF control to show molecular/reaction.
* Documentation -- Sandcastle Help File Builder project.

Build from Command Line
-----------------------

Windows: Open Developer Command Prompt for VS 2017, and launch `BuildAll.bat`.

Linux (.NET Core): To build NCDK.dll, launch `dotnet build --framework netstandard2.0` in NCDK directory.

Test from Command Line
---------------------

Windows: Launch `vstest.console.exe NCDK.Tests\bin\Release\netcoreapp2.1\NCDK.Tests.dll` and `vstest.console.exe NCDK.DisplayTests\bin\Release\NCDK.DisplayTests.dll`.

Linux: Launch `dotnet build` to build assemblies and copy `libinchi.so.#.##` to output directory of NCDK.Tests, and make a link of libinchi.so to the `libinchi.so.#.##`.
And then, launch `dotnet test` in NCDK.Tests directory to test it.

NuGet Packages
--------------

* [NCDK](https://www.nuget.org/packages/NCDK/) -- for .NET Standard 2.0 and .NET Framework 4.6.1. InChI features work only on Intel-based Windows or Intel-based Linux system.
* [NCDK.Display](https://www.nuget.org/packages/NCDK.Display/) -- for .NET Framework 4.6.1.

Copyright (c) 2016-2019 Kazuya Ujihara
