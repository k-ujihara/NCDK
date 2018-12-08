using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace NCDK.FaulonSignatures.Chemistry
{
    [TestClass()]
    public class MoleculeSignatureTest
    {
        [TestMethod()]
        public void MinimalTest()
        {
            Molecule molecule = new Molecule();
            molecule.AddAtom(0, "C");
            molecule.AddAtom(1, "N");
            molecule.AddAtom(2, "Cl");
            molecule.AddAtom(3, "O");
            molecule.AddSingleBond(0, 1);
            molecule.AddSingleBond(1, 2);
            molecule.AddSingleBond(1, 3);
            MoleculeSignature signature = new MoleculeSignature(molecule);
            string moleculeSignature = signature.GetGraphSignature();
            //Console.Out.WriteLine("molsig : " + moleculeSignature);

            Molecule permutation = new Molecule();
            permutation.AddAtom(0, "C");
            permutation.AddAtom(1, "N");
            permutation.AddAtom(2, "O");
            permutation.AddAtom(3, "Cl");
            permutation.AddSingleBond(0, 1);
            permutation.AddSingleBond(1, 2);
            permutation.AddSingleBond(1, 3);
            MoleculeSignature permSignature = new MoleculeSignature(molecule);
            string permSignatureString = permSignature.GetGraphSignature();
            //Console.Out.WriteLine("molsig : " + permSignatureString);

            Assert.AreEqual(moleculeSignature, permSignatureString);
        }

        [TestMethod()]
        public void TestColoredTreeCreation()
        {
            string signatureString = "[C]([C]([C,1])[C]([C,1]))";
            ColoredTree tree = AtomSignature.Parse(signatureString);
            Assert.AreEqual(signatureString, tree.ToString());
        }

        [TestMethod()]
        public void TestOddCycleReadin()
        {
            string signatureString = "[C]([C]([C,2]([C,1]))[C]([C,1]))";
            ColoredTree tree = AtomSignature.Parse(signatureString);
            Assert.AreEqual(signatureString, tree.ToString());
            Molecule molecule = new MoleculeBuilder().FromTree(tree);
            Assert.AreEqual(5, molecule.GetAtomCount());
            Assert.AreEqual(5, molecule.GetBondCount());
        }

        [TestMethod()]
        public void TestCage()
        {
            string signatureString = "[C]([C]([C,2]([C]([C,3][C,4]))[C]([C,5]" +
                                     "[C,3]([C,6]([C,1]))))[C]([C]([C,7][C]" +
                                     "([C,1][C,8]))[C,5]([C,8]([C,6])))[C]([C,2]" +
                                     "[C,7]([C,4]([C,1]))))";
            ColoredTree tree = AtomSignature.Parse(signatureString);
            Assert.AreEqual(signatureString, tree.ToString());
            Molecule molecule = new MoleculeBuilder().FromTree(tree);
            Assert.AreEqual(16, molecule.GetAtomCount());
            Assert.AreEqual(24, molecule.GetBondCount());
        }

        [TestMethod()]
        public void TestRoundtrip()
        {
            Molecule molecule = new Molecule();
            molecule.AddAtom("C");
            molecule.AddAtom("C");
            molecule.AddAtom("C");
            molecule.AddAtom("C");
            molecule.AddSingleBond(0, 1);
            molecule.AddSingleBond(0, 3);
            molecule.AddSingleBond(1, 2);
            molecule.AddSingleBond(2, 3);

            AtomSignature atomSignature = new AtomSignature(molecule, 0);
            string signatureString = atomSignature.ToCanonicalString();

            ColoredTree tree = AtomSignature.Parse(signatureString);
            MoleculeBuilder builder = new MoleculeBuilder();
            Molecule builtMolecule = builder.FromTree(tree);
            Assert.AreEqual(molecule.ToString(), builtMolecule.ToString());

            // test that this can be done more than once
            builtMolecule = builder.FromTree(tree);
            Assert.AreEqual(molecule.ToString(), builtMolecule.ToString());
        }

        [TestMethod()]
        public void TestLargeExample()
        {
            var filename = "NCDK.FaulonSignatures.Data.large_example.sdf";
            foreach (var molecule in MoleculeReader.ReadSDFFile(filename))
            {
                MoleculeSignature signature = new MoleculeSignature(molecule);
                Console.Out.WriteLine(signature.GetGraphSignature());
            }
        }

        [TestMethod()]
        public void TestSDF()
        {
            var filename = "NCDK.FaulonSignatures.Data.test.sdf";
            int molNr = 0;
            foreach (var molecule in MoleculeReader.ReadSDFFile(filename))
            {
                Console.Out.WriteLine(++molNr);
                MoleculeSignature signature = new MoleculeSignature(molecule);
                Console.Out.WriteLine(signature.GetGraphSignature());
                //Console.Out.WriteLine(signature.GetVertexSignature(0));
            }

        }

        [TestMethod()]
        public void TestCanonicalLabelling()
        {
            var filename = "NCDK.FaulonSignatures.Data.multCycle.sdf";
            foreach (var molecule in MoleculeReader.ReadSDFFile(filename))
            {
                MoleculeSignature signature = new MoleculeSignature(molecule);
                Console.Out.WriteLine(molecule.GetAtomCount());
                Assert.AreEqual(false, signature.IsCanonicallyLabelled());
            }

            string filenameCanLabel = "NCDK.FaulonSignatures.Data.multCycleCanLabel.sdf";
            foreach (var molecule in MoleculeReader.ReadSDFFile(filenameCanLabel))
            {
                MoleculeSignature signatureCanLabel = new MoleculeSignature(molecule);
                Assert.AreEqual(true, signatureCanLabel.IsCanonicallyLabelled());
            }
        }

        public void CheckCanonicalIsUnique(Molecule molecule)
        {
            Console.Out.WriteLine("isUnique?" + molecule);
            AtomPermutor permutor = new AtomPermutor(molecule);
            string canonicalStringForm = null;
            bool isUnique = true;
            bool atLeastOneCanonicalExample = false;

            MoleculeSignature initialMolSig = new MoleculeSignature(molecule);
            if (initialMolSig.IsCanonicallyLabelled())
            {
                atLeastOneCanonicalExample = true;
                canonicalStringForm = initialMolSig.ReconstructCanonicalEdgeString();
            }

            foreach (Molecule permutation in permutor)
            {
                MoleculeSignature molSig = new MoleculeSignature(permutation);
                if (molSig.IsCanonicallyLabelled())
                {
                    string stringForm = molSig.ReconstructCanonicalEdgeString();
                    if (canonicalStringForm == null)
                    {
                        canonicalStringForm = stringForm;
                        atLeastOneCanonicalExample = true;
                    }
                    else
                    {
                        if (canonicalStringForm.Equals(stringForm))
                        {
                            continue;
                        }
                        else
                        {
                            // not unique if there is more than one string form
                            isUnique = false;
                            break;
                        }
                    }
                }
            }
            Assert.IsTrue(isUnique, "Canonical example is not unique");
            Assert.IsTrue(atLeastOneCanonicalExample, "No canonical example");
        }

        [TestMethod()]
        public void TestFourCycle()
        {
            Molecule molecule = MoleculeFactory.FiveCycle();
            AtomSignature atomSignature = new AtomSignature(molecule, 0);
            Console.Out.WriteLine(atomSignature.ToCanonicalString());
            //        Console.Out.WriteLine(atomSignature);
        }

        [TestMethod()]
        public void TestFiveCycle()
        {
            Molecule molecule = MoleculeFactory.FiveCycle();
            AtomSignature atomSignature = new AtomSignature(molecule, 0);
            Console.Out.WriteLine(atomSignature.ToCanonicalString());
            //        Console.Out.WriteLine(atomSignature);
        }

        [TestMethod()]
        public void TestThreeStarCanonicalUnique()
        {
            Molecule molecule = MoleculeFactory.ThreeStar();
            this.CheckCanonicalIsUnique(molecule);
        }

        [TestMethod()]
        public void TestFourStarCanonicalUnique()
        {
            Molecule molecule = MoleculeFactory.FourStar();
            this.CheckCanonicalIsUnique(molecule);
        }

        [TestMethod()]
        public void TestFiveStarCanonicalUnique()
        {
            Molecule molecule = MoleculeFactory.FiveStar();
            this.CheckCanonicalIsUnique(molecule);
        }

        [TestMethod()]
        public void TestTriangleCanonicalIsUnique()
        {
            Molecule molecule = MoleculeFactory.ThreeCycle();
            this.CheckCanonicalIsUnique(molecule);
        }

        [TestMethod()]
        public void TestSquareCanonicalIsUnique()
        {
            Molecule molecule = MoleculeFactory.FourCycle();
            this.CheckCanonicalIsUnique(molecule);
        }

        [TestMethod()]
        public void TestPentagonCanonicalIsUnique()
        {
            Molecule molecule = MoleculeFactory.FiveCycle();
            this.CheckCanonicalIsUnique(molecule);
        }

        [TestMethod()]
        public void TestHexagonCanonicalIsUnique()
        {
            Molecule molecule = MoleculeFactory.SixCycle();
            this.CheckCanonicalIsUnique(molecule);
        }

        [TestMethod()]
        public void TestPropellaneCanonicalIsUnique()
        {
            Molecule molecule = MoleculeFactory.Propellane();
            this.CheckCanonicalIsUnique(molecule);
        }

        [TestMethod()]
        public void TestPseudopropellaneCanonicalIsUnique()
        {
            Molecule molecule = MoleculeFactory.Pseudopropellane();
            this.CheckCanonicalIsUnique(molecule);
        }

        [TestMethod()]
        public void TestMethylCyclobutaneCanonicalIsUnique()
        {
            Molecule molecule = MoleculeFactory.MethylatedCyclobutane();
            this.CheckCanonicalIsUnique(molecule);
        }

        [TestMethod()]
        public void TestSixcageCanonicalIsUnique()
        {
            Molecule molecule = MoleculeFactory.SixCage();
            this.CheckCanonicalIsUnique(molecule);
        }

        [TestMethod()]
        public void TestCarbonChainUnique()
        {
            // single atom
            Molecule a = new Molecule("C", 1);
            this.CheckCanonicalIsUnique(a);

            // pair of connected atoms
            a.AddAtom("C");
            a.AddSingleBond(0, 1);
            this.CheckCanonicalIsUnique(a);

            // chain of three atoms
            a.AddAtom("C");
            a.AddSingleBond(0, 2);
            this.CheckCanonicalIsUnique(a);

            // copies for new level
            Molecule b = new Molecule(a);
            Molecule c = new Molecule(a);

            // 4-chain
            a.AddAtom("C");
            a.AddSingleBond(1, 3);
            this.CheckCanonicalIsUnique(a);

            // 3-star
            b.AddAtom("C");
            b.AddSingleBond(0, 3);
            this.CheckCanonicalIsUnique(b);

            // 3-cycle
            c.AddSingleBond(1, 2);
            this.CheckCanonicalIsUnique(c);
        }

        [TestMethod()]
        public void TestCarbonHydrogenCanonicalChain()
        {
            Molecule a = new Molecule("C", 1);
            this.CheckCanonicalIsUnique(a);

            a.AddAtom("H");
            a.AddSingleBond(0, 1);
            this.CheckCanonicalIsUnique(a);

            a.AddAtom("H");
            a.AddSingleBond(0, 2);
            this.CheckCanonicalIsUnique(a);

            a.AddAtom("H");
            a.AddSingleBond(0, 3);
            this.CheckCanonicalIsUnique(a);

            a.AddAtom("H");
            a.AddSingleBond(0, 4);
            this.CheckCanonicalIsUnique(a);
        }

        [TestMethod()]
        public void TestMetheneFragmentIsCanonicallyUnique()
        {
            Molecule molecule = new Molecule();
            molecule.AddAtom("C");
            molecule.AddAtom("H");
            molecule.AddAtom("H");
            molecule.AddSingleBond(0, 1);
            molecule.AddSingleBond(0, 2);
            this.CheckCanonicalIsUnique(molecule);
        }

        [TestMethod()]
        public void TestMethaneIsCanonicallyUnique()
        {
            Molecule molecule = MoleculeFactory.Methane();
            this.CheckCanonicalIsUnique(molecule);
        }

        [TestMethod()]
        public void TestMethaneSignatures()
        {
            Molecule molecule = MoleculeFactory.Methane();
            MoleculeSignature signature = new MoleculeSignature(molecule);
            var symmetryClasses = signature.GetSymmetryClasses();
            Assert.AreEqual(2, symmetryClasses.Count);
            foreach (var symmetryClass in symmetryClasses)
            {
                if (symmetryClass.GetSignatureString().StartsWith("[H"))
                {
                    Assert.AreEqual(4, symmetryClass.Count);
                }
                else
                {
                    Assert.AreEqual(1, symmetryClass.Count);
                }
            }
        }

        [TestMethod()]
        public void TestMetheneFragmentSignatures()
        {
            Molecule molecule = new Molecule();
            molecule.AddAtom("C");
            molecule.AddAtom("H");
            molecule.AddAtom("H");
            molecule.AddSingleBond(0, 1);
            molecule.AddSingleBond(0, 2);
            MoleculeSignature signature = new MoleculeSignature(molecule);
            var symmetryClasses = signature.GetSymmetryClasses();
            Assert.AreEqual(2, symmetryClasses.Count);
            foreach (var symmetryClass in symmetryClasses)
            {
                if (symmetryClass.GetSignatureString().StartsWith("[H"))
                {
                    Assert.AreEqual(2, symmetryClass.Count);
                }
                else
                {
                    Assert.AreEqual(1, symmetryClass.Count);
                }
            }
        }

        [TestMethod()]
        public void TestMethyneFragmentSignatures()
        {
            Molecule molecule = new Molecule();
            molecule.AddAtom("C");
            molecule.AddAtom("H");
            molecule.AddSingleBond(0, 1);
            MoleculeSignature signature = new MoleculeSignature(molecule);
            var symmetryClasses = signature.GetSymmetryClasses();
            Assert.AreEqual(2, symmetryClasses.Count);
            foreach (var symmetryClass in symmetryClasses)
            {
                Assert.AreEqual(1, symmetryClass.Count);
            }
        }

        [TestMethod()]
        public void TestCanonicalIsUnique()
        {
            Molecule molecule = new Molecule();
            molecule.AddAtom("C");
            molecule.AddAtom("C");
            molecule.AddAtom("C");
            molecule.AddAtom("C");
            molecule.AddAtom("C");
            molecule.AddAtom("C");
            molecule.AddSingleBond(0, 1);
            molecule.AddSingleBond(1, 2);
            molecule.AddSingleBond(1, 3);
            molecule.AddSingleBond(2, 4);
            molecule.AddSingleBond(2, 5);
            this.CheckCanonicalIsUnique(molecule);
        }
    }
}
