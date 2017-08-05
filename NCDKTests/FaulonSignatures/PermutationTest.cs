using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Common.Collections;
using NCDK.FaulonSignatures.Chemistry;
using System;
using System.Linq;

namespace NCDK.FaulonSignatures
{
    [TestClass()]
    public class PermutationTest
    {
        public string ToSignatureString(Molecule molecule)
        {
            Console.Out.WriteLine(molecule);
            MoleculeSignature molSig = new MoleculeSignature(molecule);
            int i = 0;
            foreach (var signatureString in molSig.GetVertexSignatureStrings())
            {
                Console.Out.WriteLine(i + " " + signatureString);
                i++;
            }
            return molSig.GetMolecularSignature();
        }

        public void PrintIdentity(Molecule molecule, string signature)
        {
            int[] identity = new int[molecule.GetAtomCount()];
            for (int i = 0; i < molecule.GetAtomCount(); i++) { identity[i] = i; }
            Console.Out.WriteLine(molecule + "\t"
                    + Arrays.ToJavaString(identity) + "\t" + signature);
        }

        public void TestSpecificPermutation(Molecule molecule, int[] permutation)
        {
            string signature =
                new MoleculeSignature(molecule).GetMolecularSignature();
            PrintIdentity(molecule, signature);
            AtomPermutor permutor = new AtomPermutor(molecule);
            permutor.SetPermutation(permutation);
            Molecule permutedMolecule = permutor.First();
            string permutedSignature =
                new MoleculeSignature(permutedMolecule).GetMolecularSignature();
            Console.Out.WriteLine(permutedMolecule
                    + "\t" + Arrays.ToJavaString(permutor.GetCurrentPermutation())
                    + "\t" + permutedSignature
                    + "\t" + signature.Equals(permutedSignature));
            Assert.AreEqual(signature, permutedSignature);
        }

        public void PermuteCompletely(Molecule molecule)
        {
            string signature =
                new MoleculeSignature(molecule).GetMolecularSignature();
            PrintIdentity(molecule, signature);

            AtomPermutor permutor = new AtomPermutor(molecule);
            foreach (var permutedMolecule in permutor)
            {
                string permutedSignature =
                    new MoleculeSignature(permutedMolecule).GetMolecularSignature();
                Console.Out.WriteLine(permutedMolecule
                        + "\t" + Arrays.ToJavaString(permutor.GetCurrentPermutation())
                        + "\t" + permutedSignature
                        + "\t" + signature.Equals(permutedSignature));
                Assert.AreEqual(signature, permutedSignature);
            }
        }

        [TestMethod()]
        public void PermuteCNOMolecule()
        {
            Molecule molecule = new Molecule();
            molecule.AddAtom("C");
            molecule.AddAtom("N");
            molecule.AddAtom("O");
            molecule.AddSingleBond(0, 1);
            molecule.AddSingleBond(1, 2);
            PermuteCompletely(molecule);
        }

        [TestMethod()]
        public void PermuteOCCCSC()
        {
            Molecule molecule = new Molecule();
            molecule.AddAtom("O");
            molecule.AddAtom("C");
            molecule.AddAtom("C");
            molecule.AddAtom("C");
            molecule.AddAtom("S");
            molecule.AddAtom("C");
            molecule.AddBond(0, 1, Molecule.BondOrder.Double);
            molecule.AddSingleBond(1, 2);
            molecule.AddSingleBond(2, 3);
            molecule.AddSingleBond(3, 4);
            molecule.AddSingleBond(4, 5);
            PermuteCompletely(molecule);
        }

        [TestMethod()]
        public void PermuteOCCOCO()
        {
            Molecule molecule = new Molecule();
            molecule.AddAtom("O");
            molecule.AddAtom("C");
            molecule.AddAtom("C");
            molecule.AddAtom("O");
            molecule.AddAtom("C");
            molecule.AddAtom("O");
            molecule.AddSingleBond(0, 1);
            molecule.AddSingleBond(1, 2);
            molecule.AddSingleBond(2, 3);
            molecule.AddSingleBond(3, 4);
            molecule.AddBond(4, 5, Molecule.BondOrder.Double);
            //        molecule.AddBond(4, 5, 1);
            PermuteCompletely(molecule);
        }

        [TestMethod()]
        public void DoubleBondChainTest()
        {
            Molecule molecule = new Molecule();
            int chainLength = 6;
            molecule.AddAtom("O");
            molecule.AddAtom("C");
            molecule.AddAtom("C");
            molecule.AddAtom("O");
            molecule.AddAtom("C");
            molecule.AddAtom("O");
            for (int i = 0; i < chainLength - 2; i++)
            {
                molecule.AddSingleBond(i, i + 1);
            }
            molecule.AddBond(chainLength - 2, chainLength - 1, Molecule.BondOrder.Double);
            string sigA = ToSignatureString(molecule);
            //        PermuteCompletely(molecule);
            //        TestSpecificPermutation(molecule, new int[] {0,1,3,4,2,5});
            Molecule moleculeB = new Molecule();
            moleculeB.AddAtom("O");
            moleculeB.AddAtom("C");
            moleculeB.AddAtom("C");
            moleculeB.AddAtom("C");
            moleculeB.AddAtom("O");
            moleculeB.AddAtom("O");
            moleculeB.AddSingleBond(0, 1);
            moleculeB.AddSingleBond(1, 3);
            moleculeB.AddSingleBond(2, 4);
            moleculeB.AddBond(2, 5, Molecule.BondOrder.Double);
            moleculeB.AddSingleBond(3, 4);

            string sigB = ToSignatureString(moleculeB);
            //        MoleculeSignature molSig = new MoleculeSignature(moleculeB);
            //        molSig.SignatureStringForVertex(1);
            Assert.AreEqual(sigA, sigB);
        }
    }
}
