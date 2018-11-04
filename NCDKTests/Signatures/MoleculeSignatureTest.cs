/* Copyright (C) 2009-2010 maclean {gilleain.torrance@gmail.com}
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.FaulonSignatures;
using NCDK.Graphs;
using NCDK.Silent;
using NCDK.Smiles;
using NCDK.Templates;
using System.Collections.Generic;

namespace NCDK.Signatures
{
    // @cdk.module test-signature
    // @author maclean
    [TestClass()]
    public class MoleculeSignatureTest : CDKTestCase
    {
        private SmilesParser parser;
        private IChemObjectBuilder builder;
        private IAtomContainer mol;
        private MoleculeSignature molSig;

        public MoleculeSignatureTest()
        {
            this.parser = CDK.SmilesParser;
            this.builder = ChemObjectBuilder.Instance;
            mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            molSig = new MoleculeSignature(mol);
        }

        [TestMethod()]
        public void GetVertexCountTest()
        {
            Assert.AreEqual(mol.Atoms.Count, molSig.GetVertexCount());
        }

        [TestMethod()]
        public void GetSignatureStringForVertexTest()
        {
            Assert.AreEqual("[C]([C])", molSig.SignatureStringForVertex(0));
        }

        [TestMethod()]
        public void GetSignatureStringForVertexTest_height()
        {
            Assert.AreEqual("[C]", molSig.SignatureStringForVertex(0, 0));
        }

        [TestMethod()]
        public void GetSignatureForVertexTest()
        {
            Assert.IsNotNull(molSig.GetVertexSignatures());
        }

        [TestMethod()]
        public void CalculateOrbitsTest()
        {
            Assert.AreEqual(1, molSig.CalculateOrbits().Count);
        }

        [TestMethod()]
        public void FromSignatureStringTest()
        {
            string signatureString = molSig.ToCanonicalString();
            IAtomContainer reconstructed = MoleculeSignature.FromSignatureString(signatureString, builder);
            Assert.AreEqual(mol.Atoms.Count, reconstructed.Atoms.Count);
        }

        [TestMethod()]
        public void ToCanonicalSignatureStringTest()
        {
            Assert.AreEqual("[C]", molSig.ToCanonicalSignatureString(0));
        }

        public void FullPermutationTest(IAtomContainer mol)
        {
            AtomContainerAtomPermutor permutor = new AtomContainerAtomPermutor(mol);
            string expected = new MoleculeSignature(mol).ToCanonicalString();
            int numberOfPermutationsTried = 0;
            while (permutor.HasNext())
            {
                permutor.MoveNext();
                IAtomContainer permutation = permutor.Current;
                string actual = new MoleculeSignature(permutation).ToCanonicalString();
                numberOfPermutationsTried++;
                Assert.AreEqual(expected, actual, $"Failed on permutation{numberOfPermutationsTried}");
            }
        }

        public string CanonicalStringFromSmiles(string smiles)
        {
            var mol = parser.ParseSmiles(smiles);
            MoleculeSignature signature = new MoleculeSignature(mol);
            return signature.ToCanonicalString();
        }

        public string CanonicalStringFromMolecule(IAtomContainer molecule)
        {
            MoleculeSignature signature = new MoleculeSignature(molecule);
            return signature.GetGraphSignature();
        }

        public string FullStringFromMolecule(IAtomContainer molecule)
        {
            MoleculeSignature molSig = new MoleculeSignature(molecule);
            return molSig.ToFullString();
        }

        public List<string> GetAtomicSignatures(IAtomContainer molecule)
        {
            MoleculeSignature signature = new MoleculeSignature(molecule);
            return signature.GetVertexSignatureStrings();
        }

        public void AddHydrogens(IAtomContainer mol, IAtom atom, int n)
        {
            for (int i = 0; i < n; i++)
            {
                IAtom h = builder.NewAtom("H");
                mol.Atoms.Add(h);
                mol.Bonds.Add(builder.NewBond(atom, h));
            }
        }

        [TestMethod()]
        public void TestEmpty()
        {
            IAtomContainer mol = ChemObjectBuilder.Instance.NewAtomContainer();
            MoleculeSignature signature = new MoleculeSignature(mol);
            string signatureString = signature.ToCanonicalString();
            string expected = "";
            Assert.AreEqual(expected, signatureString);
        }

        [TestMethod()]
        public void TestSingleNode()
        {
            string singleChild = "C";
            string signatureString = this.CanonicalStringFromSmiles(singleChild);
            string expected = "[C]";
            Assert.AreEqual(expected, signatureString);
        }

        [TestMethod()]
        public void TestSingleChild()
        {
            string singleChild = "CC";
            string signatureString = this.CanonicalStringFromSmiles(singleChild);
            string expected = "[C]([C])";
            Assert.AreEqual(expected, signatureString);
        }

        [TestMethod()]
        public void TestMultipleChildren()
        {
            string multipleChildren = "C(C)C";
            string signatureString = this.CanonicalStringFromSmiles(multipleChildren);
            string expected = "[C]([C]([C]))";
            Assert.AreEqual(expected, signatureString);
        }

        [TestMethod()]
        public void TestThreeCycle()
        {
            string fourCycle = "C1CC1";
            string signatureString = this.CanonicalStringFromSmiles(fourCycle);
            string expected = "[C]([C]([C,0])[C,0])";
            Assert.AreEqual(expected, signatureString);
        }

        [TestMethod()]
        public void TestFourCycle()
        {
            string fourCycle = "C1CCC1";
            string signatureString = this.CanonicalStringFromSmiles(fourCycle);
            string expected = "[C]([C]([C,0])[C]([C,0]))";
            Assert.AreEqual(expected, signatureString);
        }

        [TestMethod()]
        public void TestMultipleFourCycles()
        {
            string bridgedRing = "C1C(C2)CC12";
            string signatureString = this.CanonicalStringFromSmiles(bridgedRing);
            string expected = "[C]([C]([C,0])[C]([C,0])[C]([C,0]))";
            Assert.AreEqual(expected, signatureString);
        }

        [TestMethod()]
        public void TestFiveCycle()
        {
            string fiveCycle = "C1CCCC1";
            string signatureString = this.CanonicalStringFromSmiles(fiveCycle);
            string expected = "[C]([C]([C]([C,0]))[C]([C,0]))";
            Assert.AreEqual(expected, signatureString);
        }

        [TestMethod()]
        public void TestMultipleFiveCycles()
        {
            string multipleFiveCycle = "C1C(CC2)CCC12";
            string signatureString = this.CanonicalStringFromSmiles(multipleFiveCycle);
            string expected = "[C]([C]([C]([C,0]))[C]([C]([C,0]))[C]([C,0]))";
            Assert.AreEqual(expected, signatureString);
        }

        [TestMethod()]
        public void TestCubane()
        {
            string expected = "[C]([C]([C,3]([C,2])[C,1]([C,2]))[C]([C,3][C,0]" + "([C,2]))[C]([C,0][C,1]))";
            IAtomContainer mol = AbstractSignatureTest.MakeCubane();
            Assert.AreEqual(expected, this.CanonicalStringFromMolecule(mol));
        }

        [TestMethod()]
        public void TestCage()
        {
            string expectedA = "[C]([C]([C]([C,4][C,3]([C,1]))[C]([C,5][C,3]))"
                    + "[C]([C,4]([C]([C,2][C,1]))[C]([C,2]([C,0])[C,6]))" + "[C]([C,5]([C]([C,0][C,1]))[C,6]([C,0])))";

            string expectedB = "[C]([C]([C]([C]([C,1]([C,0])[C,4])[C,5])[C,7]"
                    + "([C,4]([C,3])))[C]([C]([C,3]([C,0])[C,6])[C,7])" + "[C]([C,5]([C]([C,2][C,1]))[C,6]([C,2]([C,0]))))";
            IAtomContainer mol = AbstractSignatureTest.MakeCage();
            string signature = this.CanonicalStringFromMolecule(mol);
            Assert.AreEqual(expectedA, signature);
            string fullSignature = FullStringFromMolecule(mol);
            string fullExpected = "8" + expectedA + " + 8" + expectedB;
            Assert.AreEqual(fullExpected, fullSignature);
        }

        [TestMethod()]
        public void TestPropellane()
        {
            string expectedA = "[C]([C]([C,0])[C]([C,0])[C]([C,0])[C,0])";
            string expectedB = "[C]([C]([C,2][C,1][C,0])[C,2]([C,1][C,0]))";
            IAtomContainer mol = AbstractSignatureTest.MakePropellane();
            string signature = this.CanonicalStringFromMolecule(mol);
            Assert.AreEqual(expectedA, signature);
            string fullExpected = "2" + expectedA + " + 3" + expectedB;
            string fullSignature = FullStringFromMolecule(mol);
            Assert.AreEqual(fullExpected, fullSignature);
        }

        [TestMethod()]
        public void TestBridgedCycloButane()
        {
            string expected = "[C]([C]([C,0])[C]([C,0])[C,0])";
            IAtomContainer mol = AbstractSignatureTest.MakeBridgedCyclobutane();
            string signature = this.CanonicalStringFromMolecule(mol);
            Assert.AreEqual(expected, signature);
        }

        [TestMethod()]
        public void TestCyclohexaneWithHydrogens()
        {
            IAtomContainer cyclohexane = TestMoleculeFactory.MakeCyclohexane();
            for (int i = 0; i < 6; i++)
            {
                AddHydrogens(cyclohexane, cyclohexane.Atoms[i], 2);
            }
            string expected = "[C]([C]([C]([C,0]([H][H])[H][H])[H][H])" + "[C]([C]([C,0][H][H])[H][H])[H][H])";

            string actual = this.CanonicalStringFromMolecule(cyclohexane);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void TestBenzeneWithDoubleBonds()
        {
            IAtomContainer benzene = builder.NewAtomContainer();
            AbstractSignatureTest.AddCarbons(benzene, 6);
            for (int i = 0; i < 6; i++)
            {
                AbstractSignatureTest.AddHydrogens(benzene, i, 1);
            }
            benzene.AddBond(benzene.Atoms[0], benzene.Atoms[1], BondOrder.Single);
            benzene.AddBond(benzene.Atoms[1], benzene.Atoms[2], BondOrder.Double);
            benzene.AddBond(benzene.Atoms[2], benzene.Atoms[3], BondOrder.Single);
            benzene.AddBond(benzene.Atoms[3], benzene.Atoms[4], BondOrder.Double);
            benzene.AddBond(benzene.Atoms[4], benzene.Atoms[5], BondOrder.Single);
            benzene.AddBond(benzene.Atoms[5], benzene.Atoms[0], BondOrder.Double);

            MoleculeSignature signature = new MoleculeSignature(benzene);
            string carbonSignature = signature.SignatureStringForVertex(0);
            for (int i = 1; i < 6; i++)
            {
                string carbonSignatureI = signature.SignatureStringForVertex(i);
                Assert.AreEqual(carbonSignature, carbonSignatureI);
            }
        }

        [TestMethod()]
        public void GetAromaticEdgeLabelTest()
        {
            IAtomContainer benzeneRing = builder.NewAtomContainer();
            for (int i = 0; i < 6; i++)
            {
                benzeneRing.Atoms.Add(builder.NewAtom("C"));
            }
            for (int i = 0; i < 6; i++)
            {
                IAtom a = benzeneRing.Atoms[i];
                IAtom b = benzeneRing.Atoms[(i + 1) % 6];
                IBond bond = builder.NewBond(a, b);
                benzeneRing.Bonds.Add(bond);
                bond.IsAromatic = true;
            }

            MoleculeSignature molSignature = new MoleculeSignature(benzeneRing);
            List<AbstractVertexSignature> signatures = molSignature.GetVertexSignatures();
            foreach (var signature in signatures)
            {
                for (int i = 0; i < 6; i++)
                {
                    Assert.AreEqual("p", ((AtomSignature)signature).GetEdgeLabel(i, (i + 1) % 6), $"Failed for {i}");
                }
            }
        }

        [TestMethod()]
        public void CyclobuteneTest()
        {
            string expectedA = "[C]([C]([C,0])=[C]([C,0]))";
            string expectedB = "[C]([C]([C,0])[C](=[C,0]))";
            IAtomContainer cyclobutene = builder.NewAtomContainer();
            AbstractSignatureTest.AddCarbons(cyclobutene, 4);
            cyclobutene.AddBond(cyclobutene.Atoms[0], cyclobutene.Atoms[1], BondOrder.Single);
            cyclobutene.AddBond(cyclobutene.Atoms[0], cyclobutene.Atoms[2], BondOrder.Single);
            cyclobutene.AddBond(cyclobutene.Atoms[1], cyclobutene.Atoms[3], BondOrder.Double);
            cyclobutene.AddBond(cyclobutene.Atoms[2], cyclobutene.Atoms[3], BondOrder.Single);
            Assert.AreEqual(expectedA, CanonicalStringFromMolecule(cyclobutene));

            string expectedFullString = "2" + expectedA + " + 2" + expectedB;
            string actualFullString = FullStringFromMolecule(cyclobutene);
            Assert.AreEqual(expectedFullString, actualFullString);
        }

        [TestMethod()]
        public void MethyleneCyclopropeneTest()
        {
            IAtomContainer mol = builder.NewAtomContainer();
            AbstractSignatureTest.AddCarbons(mol, 4);
            AbstractSignatureTest.AddHydrogens(mol, 1, 2);
            AbstractSignatureTest.AddHydrogens(mol, 2, 1);
            AbstractSignatureTest.AddHydrogens(mol, 3, 1);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Double);
            MoleculeSignature molSig = new MoleculeSignature(mol);

            string sigFor2Height1 = molSig.SignatureStringForVertex(2, 1);
            string sigFor3Height1 = molSig.SignatureStringForVertex(3, 1);
            Assert.IsTrue(
                    sigFor2Height1.Equals(sigFor3Height1),
                    "Height 1 signatures for atoms 2 and 3" + " should be the same");

            string sigFor2Height2 = molSig.SignatureStringForVertex(2, 1);
            string sigFor3Height2 = molSig.SignatureStringForVertex(3, 1);
            Assert.IsTrue(
                    sigFor2Height2.Equals(sigFor3Height2),
                    "Height 2 signatures for atoms 2 and 3" + " should be the same");
        }

        [TestMethod()]
        public void FusedSquareMultipleBondTest()
        {
            IAtomContainer mol = builder.NewAtomContainer();
            string expected = "[C]([C]([C,1])[C]([C,0])[C](=[C,1])[C](=[C,0]))";
            AbstractSignatureTest.AddCarbons(mol, 7);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[5], BondOrder.Double);
            mol.AddBond(mol.Atoms[2], mol.Atoms[5], BondOrder.Single);
            mol.AddBond(mol.Atoms[3], mol.Atoms[6], BondOrder.Single);
            mol.AddBond(mol.Atoms[4], mol.Atoms[6], BondOrder.Double);
            MoleculeSignature molSig = new MoleculeSignature(mol);
            string sigFor0 = molSig.SignatureStringForVertex(0);
            Assert.AreEqual(expected, sigFor0);
        }

        public int FindFirstAtomIndexForSymbol(IAtomContainer container, string symbol)
        {
            for (int i = 0; i < container.Atoms.Count; i++)
            {
                if (container.Atoms[i].Symbol.Equals(symbol))
                {
                    return i;
                }
            }
            return -1;
        }

        [TestMethod()]
        public void MethylFerroceneTest()
        {
            string smiles = "CC12C3C4C5C1[Fe]23456789C%10C6C7C8C9%10";
            var mol = parser.ParseSmiles(smiles);
            MoleculeSignature molSig = new MoleculeSignature(mol);
            int feIndex = FindFirstAtomIndexForSymbol(mol, "Fe");
            string sigForIron = molSig.SignatureStringForVertex(feIndex);
            string expected = "[Fe]([C]([C,3][C,4])[C,3]([C,1])[C,4]([C,0])" + "[C]([C,5][C,6])[C,5]([C,2])[C,6]([C,7])"
                    + "[C,7]([C][C,2])[C,0]([C,1])[C,1][C,2])";
            Assert.AreEqual(expected, sigForIron);
        }

        [TestMethod()]
        public void ThreeMethylSulphanylPropanal()
        {
            string smiles = "O=CCCSC";
            FullPermutationTest(parser.ParseSmiles(smiles));
        }

        [TestMethod()]
        public void CycleWheelTest()
        {
            IAtomContainer mol = AbstractSignatureTest.MakeCycleWheel(3, 3);
            string expected = "[C]([C]([C]([C,2])[C,2])" + "[C]([C]([C,1])[C,1])" + "[C]([C]([C,0])[C,0]))";
            MoleculeSignature molSig = new MoleculeSignature(mol);
            string centralSignature = molSig.SignatureStringForVertex(0);
            Assert.AreEqual(expected, centralSignature);
        }

        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TtprTest()
        {
            string expected = "[Rh]([P]([C]([C]([C]([C,6]))" + "[C]([C]([C,6])))[C]([C]([C]([C,3]))"
                    + "[C]([C]([C,3])))[C]([C]([C]([C,2]))" + "[C]([C]([C,2]))))[P]([C]([C]([C]([C,7]))"
                    + "[C]([C]([C,7])))[C]([C]([C]([C,4]))" + "[C]([C]([C,4])))[C]([C]([C]([C,1]))"
                    + "[C]([C]([C,1]))))[P]([C]([C]([C]([C,8]))" + "[C]([C]([C,8])))[C]([C]([C]([C,5]))"
                    + "[C]([C]([C,5])))[C]([C]([C]([C,0]))" + "[C]([C]([C,0])))))";
            int phosphateCount = 3;
            int ringCount = 3;
            IAtomContainer ttpr = AbstractSignatureTest.MakeRhLikeStructure(phosphateCount, ringCount);
            MoleculeSignature molSig = new MoleculeSignature(ttpr);
            string centralSignature = molSig.SignatureStringForVertex(0);
            Assert.AreEqual(expected, centralSignature);
        }

        [TestMethod()]
        public void NapthaleneSkeletonHeightTest()
        {
            IAtomContainer napthalene = builder.NewAtomContainer();
            for (int i = 0; i < 10; i++)
            {
                napthalene.Atoms.Add(builder.NewAtom("C"));
            }
            napthalene.AddBond(napthalene.Atoms[0], napthalene.Atoms[1], BondOrder.Single);
            napthalene.AddBond(napthalene.Atoms[0], napthalene.Atoms[5], BondOrder.Single);
            napthalene.AddBond(napthalene.Atoms[1], napthalene.Atoms[2], BondOrder.Single);
            napthalene.AddBond(napthalene.Atoms[1], napthalene.Atoms[6], BondOrder.Single);
            napthalene.AddBond(napthalene.Atoms[2], napthalene.Atoms[3], BondOrder.Single);
            napthalene.AddBond(napthalene.Atoms[2], napthalene.Atoms[9], BondOrder.Single);
            napthalene.AddBond(napthalene.Atoms[3], napthalene.Atoms[4], BondOrder.Single);
            napthalene.AddBond(napthalene.Atoms[4], napthalene.Atoms[5], BondOrder.Single);
            napthalene.AddBond(napthalene.Atoms[6], napthalene.Atoms[7], BondOrder.Single);
            napthalene.AddBond(napthalene.Atoms[7], napthalene.Atoms[8], BondOrder.Single);
            napthalene.AddBond(napthalene.Atoms[8], napthalene.Atoms[9], BondOrder.Single);

            MoleculeSignature molSig = new MoleculeSignature(napthalene);
            int height = 2;
            IDictionary<string, Orbit> orbits = new Dictionary<string, Orbit>();
            for (int i = 0; i < napthalene.Atoms.Count; i++)
            {
                string signatureString = molSig.SignatureStringForVertex(i, height);
                Orbit orbit;
                if (orbits.ContainsKey(signatureString))
                {
                    orbit = orbits[signatureString];
                }
                else
                {
                    orbit = new Orbit(signatureString, height);
                    orbits[signatureString] = orbit;
                }
                orbit.AddAtomAt(i);
            }
            Assert.AreEqual(3, orbits.Count);
        }

        [TestMethod()]
        public void NapthaleneWithDoubleBondsAndHydrogenHeightTest()
        {
            IAtomContainer napthalene = builder.NewAtomContainer();
            for (int i = 0; i < 10; i++)
            {
                napthalene.Atoms.Add(builder.NewAtom("C"));
            }
            napthalene.AddBond(napthalene.Atoms[0], napthalene.Atoms[1], BondOrder.Single);
            napthalene.AddBond(napthalene.Atoms[0], napthalene.Atoms[5], BondOrder.Double);
            napthalene.AddBond(napthalene.Atoms[1], napthalene.Atoms[2], BondOrder.Double);
            napthalene.AddBond(napthalene.Atoms[1], napthalene.Atoms[6], BondOrder.Single);
            napthalene.AddBond(napthalene.Atoms[2], napthalene.Atoms[3], BondOrder.Single);
            napthalene.AddBond(napthalene.Atoms[2], napthalene.Atoms[9], BondOrder.Single);
            napthalene.AddBond(napthalene.Atoms[3], napthalene.Atoms[4], BondOrder.Double);
            napthalene.AddBond(napthalene.Atoms[4], napthalene.Atoms[5], BondOrder.Single);
            napthalene.AddBond(napthalene.Atoms[6], napthalene.Atoms[7], BondOrder.Double);
            napthalene.AddBond(napthalene.Atoms[7], napthalene.Atoms[8], BondOrder.Single);
            napthalene.AddBond(napthalene.Atoms[8], napthalene.Atoms[9], BondOrder.Double);

            napthalene.Atoms.Add(builder.NewAtom("H"));
            napthalene.AddBond(napthalene.Atoms[0], napthalene.Atoms[10], BondOrder.Single);
            napthalene.Atoms.Add(builder.NewAtom("H"));
            napthalene.AddBond(napthalene.Atoms[3], napthalene.Atoms[11], BondOrder.Single);
            napthalene.Atoms.Add(builder.NewAtom("H"));
            napthalene.AddBond(napthalene.Atoms[4], napthalene.Atoms[12], BondOrder.Single);
            napthalene.Atoms.Add(builder.NewAtom("H"));
            napthalene.AddBond(napthalene.Atoms[5], napthalene.Atoms[13], BondOrder.Single);
            napthalene.Atoms.Add(builder.NewAtom("H"));
            napthalene.AddBond(napthalene.Atoms[6], napthalene.Atoms[14], BondOrder.Single);
            napthalene.Atoms.Add(builder.NewAtom("H"));
            napthalene.AddBond(napthalene.Atoms[7], napthalene.Atoms[15], BondOrder.Single);
            napthalene.Atoms.Add(builder.NewAtom("H"));
            napthalene.AddBond(napthalene.Atoms[8], napthalene.Atoms[16], BondOrder.Single);
            napthalene.Atoms.Add(builder.NewAtom("H"));
            napthalene.AddBond(napthalene.Atoms[9], napthalene.Atoms[17], BondOrder.Single);

            int height = 2;
            SignatureQuotientGraph mqg = new SignatureQuotientGraph(napthalene, height);
            Assert.AreEqual(4, mqg.GetVertexCount());
            Assert.AreEqual(6, mqg.GetEdgeCount());
            Assert.AreEqual(2, mqg.NumberOfLoopEdges());
        }
    }
}

