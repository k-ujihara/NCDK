/* Copyright (C) 2009  Stefan Kuhn <shk3@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
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
using NCDK.Graphs;
using NCDK.IO;
using NCDK.Maths;
using NCDK.Tools.Manipulator;
using System.IO;

namespace NCDK.StructGen.Stochastic.Operators
{
    // @cdk.module test-structgen
    [TestClass()]
    public class CrossoverMachineTest : CDKTestCase
    {
        [TestMethod()]
        [TestCategory("VerySlowTest")] // structgen is slow... a single method here currently takes ~6 seconds
        public void TestDoCrossover_IAtomContainer()
        {
            IChemObjectSet<IAtomContainer> som;

            string filename = "NCDK.Data.Smiles.c10h16isomers.smi";
            using (var ins = ResourceLoader.GetAsStream(filename))
            using (SMILESReader reader = new SMILESReader(ins))
            {
                som = reader.Read(CDK.Builder.NewAtomContainerSet());
                Assert.AreEqual(99, som.Count, "We must have read 99 structures");
            }

            // Comment out next line to enable time seed random.
            RandomNumbersTool.RandomSeed = 0L;

            CrossoverMachine cm = new CrossoverMachine();
            string correctFormula = "C10H16";
            int errorcount = 0;
            for (int i = 0; i < som.Count; i++)
            {
                int[] hydrogencount1 = new int[4];
                foreach (var atom in som[i].Atoms)
                {
                    hydrogencount1[atom.ImplicitHydrogenCount.Value]++;
                }
                for (int k = i + 1; k < som.Count; k++)
                {
                    try
                    {
                        var result = cm.DoCrossover(som[i], som[k]);
                        int[] hydrogencount2 = new int[4];
                        foreach (var atom in som[k].Atoms)
                        {
                            hydrogencount2[atom.ImplicitHydrogenCount.Value]++;
                        }
                        Assert.AreEqual(2, result.Count, "Result size must be 2");
                        for (int l = 0; l < 2; l++)
                        {
                            IAtomContainer ac = result[l];
                            Assert.IsTrue(ConnectivityChecker.IsConnected(ac), "Result must be connected");
                            Assert.AreEqual(
                                MolecularFormulaManipulator.GetString(MolecularFormulaManipulator.GetMolecularFormula(ac)),
                                correctFormula,
                                "Molecular formula must be the same as" + "of the input");
                            int[] hydrogencountresult = new int[4];
                            int hcounttotal = 0;
                            foreach (var atom in result[l].Atoms)
                            {
                                hydrogencountresult[atom.ImplicitHydrogenCount.Value]++;
                                hcounttotal += atom.ImplicitHydrogenCount.Value;
                            }
                            if (hydrogencount1[0] == hydrogencount2[0])
                                Assert.AreEqual(
                                        hydrogencount1[0], hydrogencountresult[0],
                                        "Hydrogen count of the result must" + " be same as of input");
                            if (hydrogencount1[1] == hydrogencount2[1])
                                Assert.AreEqual(
                                        hydrogencount1[1], hydrogencountresult[1],
                                        "Hydrogen count of the result must" + " be same as of input");
                            if (hydrogencount1[2] == hydrogencount2[2])
                                Assert.AreEqual(
                                        hydrogencount1[2], hydrogencountresult[2],
                                        "Hydrogen count of the result must" + " be same as of input");
                            if (hydrogencount1[3] == hydrogencount2[3])
                                Assert.AreEqual(
                                        hydrogencount1[3], hydrogencountresult[3],
                                        "Hydrogen count of the result must" + " be same as of input");
                            Assert.AreEqual(16, hcounttotal);
                        }
                    }
                    catch (CDKException)
                    {
                        errorcount++;
                    }
                }
            }
            Assert.IsTrue(errorcount < 300, "We tolerate up to 300 errors");
        }
    }
}
