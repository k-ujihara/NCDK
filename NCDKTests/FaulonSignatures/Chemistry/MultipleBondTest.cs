using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace NCDK.FaulonSignatures.Chemistry
{
    [TestClass()]
    public class MultipleBondTest
    {
        [TestMethod()]
        public void AromaticTest()
        {
            Molecule benzene = new Molecule();
            benzene.AddMultipleAtoms(6, "C");
            benzene.AddBond(0, 1, Molecule.BondOrder.Aromatic);
            benzene.AddBond(1, 2, Molecule.BondOrder.Aromatic);
            benzene.AddBond(2, 3, Molecule.BondOrder.Aromatic);
            benzene.AddBond(3, 4, Molecule.BondOrder.Aromatic);
            benzene.AddBond(4, 5, Molecule.BondOrder.Aromatic);
            benzene.AddBond(5, 0, Molecule.BondOrder.Aromatic);
            MoleculeSignature molSig = new MoleculeSignature(benzene);
            Console.Out.WriteLine(molSig.ToCanonicalString());
        }

        [TestMethod()]
        public void CocoTest()
        {
            Molecule moleculeA = new Molecule();
            int chainLength = 6;
            moleculeA.AddAtom("O");
            moleculeA.AddAtom("C");
            moleculeA.AddAtom("C");
            moleculeA.AddAtom("O");
            moleculeA.AddAtom("C");
            moleculeA.AddAtom("O");
            for (int i = 0; i < chainLength - 2; i++)
            {
                moleculeA.AddSingleBond(i, i + 1);
            }
            moleculeA.AddBond(chainLength - 2, chainLength - 1, Molecule.BondOrder.Double);
            //        moleculeA.AddBond(chainLength - 2, chainLength - 1, 1);

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
            //        moleculeB.AddBond(2, 5, 1);
            moleculeB.AddSingleBond(3, 4);

            MoleculeSignature molSigA = new MoleculeSignature(moleculeA);
            string sigA = molSigA.SignatureStringForVertex(3);
            Console.Out.WriteLine(sigA);
            Console.Out.WriteLine("--------------------------------------");
            MoleculeSignature molSigB = new MoleculeSignature(moleculeB);
            string sigB = molSigB.SignatureStringForVertex(4);
            Console.Out.WriteLine(sigB);
            Assert.AreEqual(sigA, sigB);
        }

        [TestMethod()]
        public void MultipleBondedFragmentTest()
        {
            Molecule molA = new Molecule();
            molA.AddAtom("C");
            molA.AddAtom("C");
            molA.AddAtom("C");
            molA.AddAtom("H");
            molA.AddBond(0, 1, Molecule.BondOrder.Double);
            molA.AddSingleBond(0, 2);
            molA.AddSingleBond(0, 3);

            MoleculeSignature molSig = new MoleculeSignature(molA);
            string signatureFor0A = molSig.SignatureStringForVertex(0);
            Console.Out.WriteLine(signatureFor0A);

            Molecule molB = new Molecule();
            molB.AddAtom("C");
            molB.AddAtom("C");
            molB.AddAtom("C");
            molB.AddAtom("H");
            molB.AddSingleBond(0, 1);
            molB.AddBond(0, 2, Molecule.BondOrder.Double);
            molB.AddSingleBond(0, 3);

            molSig = new MoleculeSignature(molB);
            string signatureFor0B = molSig.SignatureStringForVertex(0);
            Console.Out.WriteLine(signatureFor0B);

            Assert.AreEqual(signatureFor0A, signatureFor0B);
        }

        [TestMethod()]
        public void Cyclobut_1_ene()
        {
            Molecule mol = new Molecule();
            for (int i = 0; i < 4; i++)
            {
                mol.AddAtom("C");
            }
            mol.AddSingleBond(0, 1);
            mol.AddBond(0, 2, Molecule.BondOrder.Double);
            mol.AddSingleBond(1, 3);
            mol.AddSingleBond(2, 3);
            MoleculeSignature molSignature = new MoleculeSignature(mol);
            Console.Out.WriteLine(molSignature.ToCanonicalString());
        }


        [TestMethod()]
        public void Cyclobut_2_ene()
        {
            Molecule mol = new Molecule();
            for (int i = 0; i < 4; i++)
            {
                mol.AddAtom("C");
            }
            mol.AddSingleBond(0, 1);
            mol.AddBond(0, 2, Molecule.BondOrder.Double);
            mol.AddBond(1, 3, Molecule.BondOrder.Double);
            mol.AddSingleBond(2, 3);
            MoleculeSignature molSignature = new MoleculeSignature(mol);
            Console.Out.WriteLine(molSignature.ToCanonicalString());
        }

        [TestMethod()]
        public void BenzeneTest()
        {
            Molecule mol = new Molecule();
            for (int i = 0; i < 6; i++)
            {
                mol.AddAtom("C");
            }
            mol.AddSingleBond(0, 1);
            mol.AddBond(1, 2, Molecule.BondOrder.Double);
            mol.AddSingleBond(2, 3);
            mol.AddBond(3, 4, Molecule.BondOrder.Double);
            mol.AddSingleBond(4, 5);
            mol.AddBond(5, 0, Molecule.BondOrder.Double);
            MoleculeSignature molSig = new MoleculeSignature(mol);
            string sigString0 = molSig.SignatureStringForVertex(0);
            for (int i = 1; i < 6; i++)
            {
                string sigStringI = molSig.SignatureStringForVertex(i);
                Assert.AreEqual(sigString0, sigStringI);
            }
        }
    }
}
