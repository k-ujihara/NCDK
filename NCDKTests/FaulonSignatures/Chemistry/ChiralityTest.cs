using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace NCDK.FaulonSignatures.Chemistry
{
    [TestClass()]
    public class ChiralityTest
    {
        [TestMethod()]
        public void SpiranTest()
        {
            Molecule mol = new Molecule();
            mol.AddAtom("N");
            mol.AddMultipleAtoms(12, "C");
            mol.AddMultipleAtoms(24, "H");
            mol.AddMultipleSingleBonds(0, 1, 4, 5, 8);  // central N
            mol.AddMultipleSingleBonds(1, 2, 33, 34);   // ring 1 - CA
            mol.AddMultipleSingleBonds(2, 3, 11, 15);   // ring 1 - CB
            mol.AddMultipleSingleBonds(3, 4, 12, 16);   // ring 1 - CB
            mol.AddMultipleSingleBonds(4, 35, 36);      // ring 1 - CA
            mol.AddMultipleSingleBonds(5, 6, 29, 30);   // ring 2 - CA
            mol.AddMultipleSingleBonds(6, 7, 9, 13);    // ring 2 - CB
            mol.AddMultipleSingleBonds(7, 8, 10, 14);   // ring 2 - CB
            mol.AddMultipleSingleBonds(8, 31, 32);      // ring 2 - CA
            mol.AddMultipleSingleBonds(9, 17, 18, 19);  // CH3 A
            mol.AddMultipleSingleBonds(10, 20, 21, 22); // CH3 B
            mol.AddMultipleSingleBonds(11, 23, 24, 25); // CH3 C
            mol.AddMultipleSingleBonds(12, 26, 27, 28); // CH3 D

            // 7 symmetry classes - central N, CA, CB, CH3-C, HA, HB, CH3-H 
            MoleculeSignature molSig = new MoleculeSignature(mol);
            var symmetryClasses = molSig.GetSymmetryClasses();
            Assert.AreEqual(7, symmetryClasses.Count);

            var tetraChiralCenters =
                        ChiralCenterFinder.FindTetrahedralChiralCenters(mol);
            Assert.AreEqual(4, tetraChiralCenters.Count);
            Assert.AreEqual(2, (int)tetraChiralCenters[0]);
            Assert.AreEqual(3, (int)tetraChiralCenters[1]);
            Assert.AreEqual(6, (int)tetraChiralCenters[2]);
            Assert.AreEqual(7, (int)tetraChiralCenters[3]);
        }

        [TestMethod()]
        public void DichlorocyclopropaneTest()
        {
            Molecule mol = new Molecule();
            mol.AddAtom("C");
            mol.AddAtom("C");
            mol.AddAtom("C");
            mol.AddAtom("Cl");
            mol.AddAtom("Cl");
            mol.AddAtom("H");
            mol.AddAtom("H");
            mol.AddAtom("H");
            mol.AddAtom("H");
            mol.AddMultipleSingleBonds(0, 1, 2, 3, 5);
            mol.AddMultipleSingleBonds(1, 2, 4, 6);
            mol.AddMultipleSingleBonds(2, 7, 8);

            // 5 symmetry classes - the non-chiral carbon, its hydrogens, the chiral
            // carbons, their hydrogens, and the chlorines.
            MoleculeSignature molSig = new MoleculeSignature(mol);
            var symmetryClasses = molSig.GetSymmetryClasses();
            //        Console.Out.WriteLine(symmetryClasses);
            Assert.AreEqual(5, symmetryClasses.Count);

            // only two possible chiral centers
            var tetraChiralCenters =
                        ChiralCenterFinder.FindTetrahedralChiralCenters(mol);
            Assert.AreEqual(2, tetraChiralCenters.Count);
            Assert.AreEqual(0, (int)tetraChiralCenters[0]);
            Assert.AreEqual(1, (int)tetraChiralCenters[1]);
        }

        [TestMethod()]
        public void DihydroxyCyclohexane()
        {
            Molecule mol = new Molecule();
            mol.AddMultipleAtoms(6, "C");
            mol.AddMultipleAtoms(2, "O");
            mol.AddMultipleAtoms(12, "H");
            mol.AddMultipleSingleBonds(0, 1, 5, 6, 11);
            mol.AddMultipleSingleBonds(1, 2, 12, 13);
            mol.AddMultipleSingleBonds(2, 3, 14, 15);
            mol.AddMultipleSingleBonds(3, 4, 7, 10);
            mol.AddMultipleSingleBonds(4, 5, 16, 17);
            mol.AddMultipleSingleBonds(5, 18, 19);
            mol.AddMultipleSingleBonds(6, 8);
            mol.AddMultipleSingleBonds(7, 9);
            MoleculeSignature molSig = new MoleculeSignature(mol);
            var symmetryClasses = molSig.GetSymmetryClasses();
            Console.Out.WriteLine(symmetryClasses);
            Assert.AreEqual(6, symmetryClasses.Count);

            // this method cannot find the linked chiral centers 
            var tetraChiralCenters =
                        ChiralCenterFinder.FindTetrahedralChiralCenters(mol);
            Assert.IsTrue(!tetraChiralCenters.Any());
        }
    }
}
