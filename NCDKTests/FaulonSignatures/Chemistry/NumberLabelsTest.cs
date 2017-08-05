using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static NCDK.FaulonSignatures.AbstractVertexSignature;

namespace NCDK.FaulonSignatures.Chemistry
{
    [TestClass()]
    public class NumberLabelsTest
    {
        [TestMethod()]
        public void ChoTest()
        {
            Molecule mol = new Molecule();
            mol.AddAtom("C");
            mol.AddAtom("H");
            mol.AddAtom("O");
            mol.AddSingleBond(0, 1);
            mol.AddSingleBond(0, 2);
            MoleculeSignature firstSig = new MoleculeSignature(mol, InvariantType.INTEGER);
            string firstSigString = firstSig.ToCanonicalString();
            Console.Out.WriteLine(firstSigString);
            AtomPermutor permutor = new AtomPermutor(mol);
            foreach (Molecule pMol in permutor)
            {
                MoleculeSignature pSig = new MoleculeSignature(pMol, InvariantType.INTEGER);
                string pSigString = pSig.ToCanonicalString();
                Console.Out.WriteLine(pSigString);
            }
        }
    }
}
