/* Copyright (C) 2007  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.IO;
using NCDK.Tools.Manipulator;
using System;
using System.Diagnostics;
using System.Linq;

namespace NCDK.AtomTypes
{
    /// <summary>
    /// This class tests the matching of atom types defined in the
    /// cdk atom type list, using the test files in <code>src/test/data</code>.
    ///
    // @cdk.module test-core
    ///
    // @see org.openscience.cdk.atomtype.CDKAtomTypeMatcher
    /// </summary>
    [TestClass()]
    public class CDKAtomTypeMatcherTestFileReposTest : CDKTestCase
    {

        //[TestMethod()]
        //private static int TestMethod()
        //{
        //    throw new NotImplementedException();
        //}

        public void TestPDBfiles()
        {
            string DIRNAME = "NCDK.Data.PDB.";
            string[] testFiles = { "114D.pdb", "1CRN.pdb", "1D66.pdb", "1IHA.pdb", "1PN8.pdb", };
            int tested = 0;
            int failed = 0;
            ISimpleChemObjectReader reader = new PDBReader();
            foreach (var testFile in testFiles)
            {
                TestResults results = TestFile(DIRNAME, testFile, reader);
                tested += results.tested;
                failed += results.failed;
            }
            Assert.AreEqual(tested, (tested - failed), "Could not match all atom types!");
        }

        [TestMethod()]
        public void TestMOL2files()
        {
            string DIRNAME = "NCDK.Data.Mol2.";
            string[] testFiles = { "fromWebsite.mol2", };
            int tested = 0;
            int failed = 0;
            ISimpleChemObjectReader reader = new PDBReader();
            foreach (var testFile in testFiles)
            {
                TestResults results = TestFile(DIRNAME, testFile, reader);
                tested += results.tested;
                failed += results.failed;
            }
            Assert.AreEqual(tested, (tested - failed), "Could not match all atom types!");
        }

        [TestMethod()]
        public void TestASNfiles()
        {
            string DIRNAME = "NCDK.Data.ASN.PubChem.";
            string[] testFiles = { "cid1.asn", };
            int tested = 0;
            int failed = 0;
            ISimpleChemObjectReader reader = new PDBReader();
            foreach (var testFile in testFiles)
            {
                TestResults results = TestFile(DIRNAME, testFile, reader);
                tested += results.tested;
                failed += results.failed;
            }
            Assert.AreEqual(tested, (tested - failed), "Could not match all atom types!");
        }

        [TestMethod()]
        public void TestMDLMolfiles()
        {
            string DIRNAME = "NCDK.Data.MDL.";
            string[] testFiles = {"2,5-dimethyl-furan.mol", "5SD.mol", "9553.mol", "9554.mol", "ADN.mol", "allmol231.mol",
                "allmol232.mol", "a-pinene.mol", "azulene.mol", "big.mol", "BremserPredictionTest.mol",
                "bug1014344-1.mol", "bug1089770-1.mol", "bug1089770-2.mol", "bug1328739.mol", "bug_1750968.mol",
                "bug1772609.mol", "bug682233.mol", "bug698152.mol", "bug706786-1.mol", "bug706786-2.mol",
                "bug716259.mol", "bug771485-1.mol", "bug771485-2.mol", "bug853254-1.mol", "bug853254-2.mol",
                "bug931608-1.mol", "bug931608-2.mol", "bug934819-1.mol", "bug934819-2.mol", "Butane-TestFF.mol",
                "Butane-TestFF-output.mol", "butanoic_acid.mol", "C12308.mol", "carbocations.mol", "choloylcoa.mol",
                "clorobenzene.mol", "cyclooctadien.mol", "cyclooctan.mol", "cycloocten.mol", "cyclopropane.mol",
                "d-ala.mol", "decalin.mol", "D+-glucose.mol", "D-mannose.mol", "Ethane-TestFF.mol",
                "Ethane-TestFF-output.mol", "figueras-test-buried.mol", "figueras-test-inring.mol",
                "figueras-test-sep3D.mol", "four-ring-5x10.mol", "heptane_almost_cyclic.mol",
                "heptane_almost_cyclic-output.mol",
                "heptane-modelbuilder.mol",
                "heptane-modelbuilder-output.mol",
                "heptane.mol",
                "Heptane-TestFF.mol",
                "Heptane-TestFF-output.mol",
                "hydroxyamino.mol",
                "isopropylacetate.mol",
                "l-ala.mol",
                "methylbenzol.mol",
                // "molV3000.mol", // can't be read with MDLV2000Reader!
                "murckoTest10.mol", "murckoTest11.mol", "murckoTest1.mol", "murckoTest2.mol", "murckoTest3.mol",
                "murckoTest4.mol", "murckoTest5.mol", "murckoTest6_3d_2.mol", "murckoTest6_3d.mol", "murckoTest6.mol",
                "murckoTest7.mol", "murckoTest8.mol", "murckoTest9.mol", "NN_dimethylaniline.mol",
                "nonConnectedPiSystems.mol", "Ooporphyrin.mol", "piSystemCumulative.mol",
                "piSystemWithCarbokation.mol", "polycarpol.mol", "porphyrin.mol", "prediction-test.mol",
                "reserpine.mol", "ring_03419.mol", "saturationcheckertest.mol", "sdg_test.mol",
                "shortest_path_test.mol", "six-ring-4x4.mol", "sulfurCompound.mol", "superspiro.mol",
                "testdoublebondconfig.mol", "testisopropylacetate.mol", "thiamin.mol", "withcharges.mol",
                "zinc_1309609.sdf", "noxide.sdf", "noxide2.sdf", "noxide3.sdf"};
            int tested = 0;
            int failed = 0;
            ISimpleChemObjectReader reader = new MDLV2000Reader();
            foreach (var testFile in testFiles)
            {
                try
                {
                    TestResults results = TestFile(DIRNAME, testFile, reader);
                    tested += results.tested;
                    failed += results.failed;
                }
                catch (Exception e)
                {
                    Assert.Fail(testFile + " caused an error: " + e);
                }
            }
            Assert.AreEqual(tested, (tested - failed), "Could not match all atom types!");
        }

        private TestResults TestFile(string dir, string filename, ISimpleChemObjectReader reader)
        {
            CDKAtomTypeMatcher matcher = CDKAtomTypeMatcher.GetInstance(Default.ChemObjectBuilder.Instance);
            var ins = ResourceLoader.GetAsStream(dir + filename);
            reader.SetReader(ins);
            IAtomContainer mol = null;
            if (reader.Accepts(typeof(IAtomContainer)))
            {
                mol = reader.Read(new Silent.AtomContainer());
            }
            else if (reader.Accepts(typeof(IChemFile)))
            {
                IChemFile cf = reader.Read(new ChemFile());
                mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
                var containers = ChemFileManipulator.GetAllAtomContainers(cf);
                foreach (var container in containers)
                    mol.Add(container);
            }

            Assert.IsNotNull(mol, "Could not read the file into a IAtomContainer: " + filename);

            TestResults results = new TestResults();
            Trace.Assert(mol != null);
            foreach (var atom in mol.Atoms)
            {
                results.tested++;
                IAtomType matched = matcher.FindMatchingAtomType(mol, atom);
                if (matched == null)
                {
                    results.failed++;
                    Console.Out.WriteLine("Could not match atom: " + results.tested + " in file " + filename);
                }
                else
                // OK, the matcher did find something. Now, let's see of the
                // atom type properties are consistent with those of the atom
                if (!atom.Symbol.Equals(matched.Symbol))
                {
                    // OK, OK, that's very basic indeed, but why not
                    results.failed++;
                    Console.Out.WriteLine("Symbol does not match: " + results.tested + " in file " + filename);
                    Console.Out.WriteLine("Found: " + atom.Symbol + ", expected: " + matched.Symbol);
                }
                else if (!atom.Hybridization.IsUnset
                      && atom.Hybridization != matched.Hybridization)
                {
                    results.failed++;
                    Console.Out.WriteLine("Hybridization does not match: " + results.tested + " in file " + filename);
                    Console.Out.WriteLine("Found: " + atom.Hybridization + ", expected: " + matched.Hybridization
                            + " (" + matched.AtomTypeName + ")");
                }
                else if (atom.FormalCharge.Value != matched.FormalCharge.Value)
                {
                    results.failed++;
                    Console.Out.WriteLine("Formal charge does not match: " + results.tested + " in file " + filename);
                    Console.Out.WriteLine("Found: " + atom.FormalCharge + ", expected: " + matched.FormalCharge
                            + " (" + matched.AtomTypeName + ")");
                }
                else
                {
                    var connections = mol.GetConnectedBonds(atom);
                    int connectionCount = connections.Count();
                    //                int piBondsFound = (int)mol.GetBondOrderSum(atom) - connectionCount;
                    // there might be missing hydrogens, so: found <= expected
                    if (matched.FormalNeighbourCount != null
                            && connectionCount > matched.FormalNeighbourCount
                            && !"X".Equals(matched.AtomTypeName))
                    {
                        results.failed++;
                        Console.Out.WriteLine("Number of neighbors is too high: " + results.tested + " in file " + filename);
                        Console.Out.WriteLine("Found: " + connectionCount + ", expected (max): "
                                + matched.FormalNeighbourCount + " (" + matched.AtomTypeName + ")");
                    }
                    // there might be missing double bonds, so: found <= expected
                    //                if (piBondsFound > matched.GetXXXX()) {
                    //                    results.failed++;
                    //                    Console.Out.WriteLine("Number of neighbors is too high: " + results.tested + " in file " + filename);
                    //                    Console.Out.WriteLine("Found: " + atom.FormalNeighbourCount +
                    //                                       ", expected (max): " + matched.FormalNeighbourCount);
                    //                }
                }
            }
            return results;
        }

        class TestResults
        {

            public int tested;
            public int failed;

            public TestResults()
            {
                tested = 0;
                failed = 0;
            }
        }
    }
}
