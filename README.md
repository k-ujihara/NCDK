NCDK: The Chemistry Development Kit ported to C#
===============================================

NCDK is .NET port of [the Chemistry Development Project (CDK)](https://github.com/cdk/cdk). Functionality is provided for many areas in cheminformatics.

NCDK is covered under LGPL v2.1. The modules are free and open-source and is easily integrated with other open-source or in-house projects.

The current release is based on [cdk 2019-04-15](https://github.com/cdk/cdk/commit/c3d0f16502bf08df50365fee392e11d7c9856657) snapshot except JLogP module.

Getting Started
---------------

Most of codes are written in C\#. You can learn NCDK from CDK documents, NCDKTests and NCDKDisplayTests project.

* NCDK -- Core module of NCDK
* NCDK.Display -- Depiction module of NCDK. It is based on WPF.
* NCDK.Tests -- Unit tests for NCDK
* NCDK.DisplayTests -- Unit tests for NCDK.Display
* MolViewer -- MOL file viewer, which can depict SMILES. It includes WPF control to show molecular/reaction.
* Documentation -- Sandcastle Help File Builder project

Chem object contains utility methods for quick access to frequently used functions. The methods' name are called after RDKit. They should be work within C# interactive.

```
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;

    class Chem_Example
    {
        static void Main(string[] args)
        {
            var toluene = Chem.MolFromSmiles("Cc1ccccc1");
            var mol1 = Chem.MolFromFile("Data/input.mol");
            var stringWithMolData = new StreamReader("Data/input.mol").ReadToEnd();
            var mol2 = Chem.MolFromMolBlock(stringWithMolData);

            using (var suppl = Chem.SDMolSupplier("Data/5ht3ligs.sdf"))
            {
                Console.WriteLine(suppl.Count);
                foreach (var mol in suppl)
                {
                    if (mol == null)
                        continue;
                    Console.WriteLine(mol.Atoms.Count);
                }
                Console.WriteLine(suppl[0].Atoms.Count);
            }

            using (var fsuppl = Chem.ForwardSDMolSupplier(new FileStream("Data/5ht3ligs.sdf", FileMode.Open)))
            {
                foreach (var mol in fsuppl)
                {
                    if (mol == null)
                        continue;
                    Console.WriteLine(mol.Atoms.Count);
                }
            }

            using (var gzsuppl = Chem.ForwardSDMolSupplier(
                new GZipStream(
                    new FileStream("Data/actives_5ht3.sdf.gz", FileMode.Open), 
                    CompressionMode.Decompress)))
            {
                Console.WriteLine(gzsuppl.Count(x => x != null));
            }
        }
    }

```

Build from Command Line
-----------------------

Windows: Open Developer Command Prompt for VS 2017, and launch "BuildAll.bat".

Linux (.NET Core): To build NCDK.dll, launch "dotnet build --framework netstandard2.0" in NCDK directory.

Test from Command Line
---------------------

Windows: Launch "vstest.console.exe NCDK.Tests\bin\Release\netcoreapp2.1\NCDK.Tests.dll" and "vstest.console.exe NCDK.DisplayTests\bin\Release\NCDK.DisplayTests.dll".

Linux: Launch "dotnet build" to build assemblies and copy libinchi.so.#.## to output directory of NCDK.Tests, and make a link of libinchi.so to the libinchi.so.#.##.
And then, launch "dotnet test" in NCDK.Tests directory to test it.

NuGet Packages
--------------

* [NCDK](https://www.nuget.org/packages/NCDK/) -- for .NET Standard 2.0 and .NET Framework 4.6.1. InChI features work only on Intel-based Windows or Intel-based Linux system.
* [NCDK.Display](https://www.nuget.org/packages/NCDK.Display/) -- for .NET Framework 4.6.1.

Copyright (c) 2016-2019 Kazuya Ujihara
