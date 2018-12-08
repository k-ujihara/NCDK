using NCDK.Silent;
using NCDK.Smiles;
using System;

namespace NCDK.SMSD
{
    static class Isomorphism_Example
    {
        static void Main()
        {
            {
                #region 1
                SmilesParser sp = new SmilesParser();
                // Benzene
                IAtomContainer A1 = sp.ParseSmiles("C1=CC=CC=C1");
                // Napthalene
                IAtomContainer A2 = sp.ParseSmiles("C1=CC2=C(C=C1)C=CC=C2");
                //Turbo mode search
                //Bond Sensitive is set true
                Isomorphism comparison = new Isomorphism(Algorithm.SubStructure, true);
                // set molecules, remove hydrogens, clean and configure molecule
                comparison.Init(A1, A2, true, true);
                // set chemical filter true
                comparison.SetChemFilters(false, false, false);
                if (comparison.IsSubgraph())
                {
                    //Get similarity score
                    Console.Out.WriteLine("Tanimoto coefficient:  " + comparison.GetTanimotoSimilarity());
                    Console.Out.WriteLine("A1 is a subgraph of A2:  " + comparison.IsSubgraph());
                    //Get Modified AtomContainer
                    IAtomContainer Mol1 = comparison.ReactantMolecule;
                    IAtomContainer Mol2 = comparison.ProductMolecule;
                    // Print the mapping between molecules
                    Console.Out.WriteLine(" Mappings: ");
                    foreach (var mapping in comparison.GetFirstMapping())
                    {
                        Console.Out.WriteLine((mapping.Key + 1) + " " + (mapping.Value + 1));

                        IAtom eAtom = Mol1.Atoms[mapping.Key];
                        IAtom pAtom = Mol2.Atoms[mapping.Value];
                        Console.Out.WriteLine(eAtom.Symbol + " " + pAtom.Symbol);
                    }
                    Console.Out.WriteLine("");
                }
                #endregion
            }
            {
                #region 2
                SmilesParser sp = new SmilesParser();
                // Benzene
                IAtomContainer A1 = sp.ParseSmiles("C1=CC=CC=C1");
                // Napthalene
                IAtomContainer A2 = sp.ParseSmiles("C1=CC2=C(C=C1)C=CC=C2");
                //{ 0: Default Isomorphism Algorithm, 1: MCSPlus Algorithm, 2: VFLibMCS Algorithm, 3: CDKMCS Algorithm}
                //Bond Sensitive is set true
                Isomorphism comparison = new Isomorphism(Algorithm.Default, true);
                // set molecules, remove hydrogens, clean and configure molecule
                comparison.Init(A1, A2, true, true);
                // set chemical filter true
                comparison.SetChemFilters(true, true, true);

                //Get similarity score
                Console.Out.WriteLine("Tanimoto coefficient:  " + comparison.GetTanimotoSimilarity());
                Console.Out.WriteLine("A1 is a subgraph of A2:  " + comparison.IsSubgraph());
                //Get Modified AtomContainer
                IAtomContainer Mol1 = comparison.ReactantMolecule;
                IAtomContainer Mol2 = comparison.ProductMolecule;
                // Print the mapping between molecules
                Console.Out.WriteLine(" Mappings: ");
                foreach (var mapping in comparison.GetFirstMapping())
                {
                    Console.Out.WriteLine((mapping.Key + 1) + " " + (mapping.Value + 1));

                    IAtom eAtom = Mol1.Atoms[mapping.Key];
                    IAtom pAtom = Mol2.Atoms[mapping.Value];
                    Console.Out.WriteLine(eAtom.Symbol + " " + pAtom.Symbol);
                }
                Console.Out.WriteLine("");
                #endregion
            }
        }
    }
}
