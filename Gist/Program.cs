using NCDK.Depict;
using NCDK.Renderers.Colors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using NCDK;
using NCDK.Tools.Manipulator;

namespace SmilesToPng
{
    class Program
    {
        const string SMILES_Pomalidomide = "C1CNCCC1";
        static string defaultSmiles = SMILES_Pomalidomide;

        static void Main(string[] args)
        {
            var molsInSmiles = new List<string>();
            switch (args.Length)
            {
                case 0:
                    molsInSmiles.Add(defaultSmiles);
                    break;
                default:
                    molsInSmiles.AddRange(args);
                    break;
            }

            var generator = new DepictionGenerator
            {
                AtomColorer = new CDK2DAtomColors(),
                BackgroundColor = System.Windows.Media.Colors.Transparent,
            };
            int i = 0;
            foreach (var smiles in molsInSmiles)
            {
                try
                {
                    var mol = CDK.SmilesParser.ParseSmiles(smiles);
                    AtomContainerManipulator.PerceiveDativeBonds(mol);
                    AtomContainerManipulator.PerceiveRadicals(mol);
                    var depict = generator.Depict(mol);
                    depict.WriteTo($"{i}.png");
                    depict.WriteTo($"{i}.svg");
                }
                catch (Exception e)
                {
                    Trace.TraceWarning(e.Message);
                }
                i++;
            }
        }
    }
}
