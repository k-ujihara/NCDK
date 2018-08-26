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
using NCDK.Silent;
using NCDK.Smiles;
using NCDK.Tools;

namespace NCDK.StructGen.Stochastic
{
    // @cdk.module test-structgen
    [TestClass()]
    public class PartialFilledStructureMergerTest : CDKTestCase
    {
        [TestMethod()]
        public void TestGenerate_IAtomContainerSet()
        {
            var sp = new SmilesParser(ChemObjectBuilder.Instance);
            var acs = ChemObjectBuilder.Instance.NewAtomContainerSet();
            acs.Add(sp.ParseSmiles("[CH2]CCC[CH2]"));
            acs.Add(sp.ParseSmiles("[C]1=C(C1)C[CH2]"));
            var pfsm = new PartialFilledStructureMerger();
            var result = pfsm.Generate(acs);
            Assert.IsTrue(ConnectivityChecker.IsConnected(result));
            Assert.IsTrue(new SaturationChecker().AllSaturated(result));
        }

        [TestMethod()]
        public void TestPartialFilledStructureMerger2()
        {
            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            var acs = ChemObjectBuilder.Instance.NewAtomContainerSet();
            acs.Add(sp.ParseSmiles("[C]=[C]CC[CH2]"));
            acs.Add(sp.ParseSmiles("[C]([CH2])=C1CC1"));
            PartialFilledStructureMerger pfsm = new PartialFilledStructureMerger();
            IAtomContainer result = pfsm.Generate(acs);
            Assert.IsTrue(ConnectivityChecker.IsConnected(result));
            Assert.IsTrue(new SaturationChecker().AllSaturated(result));
        }

        [TestMethod()]
        public void TestPartialFilledStructureMerger3()
        {
            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            var acs = ChemObjectBuilder.Instance.NewAtomContainerSet();
            acs.Add(sp.ParseSmiles("[CH2]CCC[CH2]"));
            acs.Add(sp.ParseSmiles("[CH2]C[CH2]"));
            acs.Add(sp.ParseSmiles("[CH2][CH2]"));
            PartialFilledStructureMerger pfsm = new PartialFilledStructureMerger();
            IAtomContainer result = pfsm.Generate(acs);
            Assert.IsTrue(ConnectivityChecker.IsConnected(result));
            Assert.IsTrue(new SaturationChecker().AllSaturated(result));
        }

        [TestMethod()]
        public void TestPartialFilledStructureMerger4()
        {
            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            var acs = ChemObjectBuilder.Instance.NewAtomContainerSet();
            acs.Add(sp.ParseSmiles("[C]CCC[CH2]"));
            acs.Add(sp.ParseSmiles("[C]CC[CH2]"));
            acs.Add(sp.ParseSmiles("[CH2]"));
            PartialFilledStructureMerger pfsm = new PartialFilledStructureMerger();
            IAtomContainer result = pfsm.Generate(acs);
            Assert.IsTrue(ConnectivityChecker.IsConnected(result));
            Assert.IsTrue(new SaturationChecker().AllSaturated(result));
        }

        [TestMethod()]
        public void TestPartialFilledStructureMerger5()
        {
            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            var acs = ChemObjectBuilder.Instance.NewAtomContainerSet();
            acs.Add(sp.ParseSmiles("[C]1CCC1"));
            acs.Add(sp.ParseSmiles("[C]([CH2])CC[CH2]"));
            acs.Add(sp.ParseSmiles("[CH2]"));
            PartialFilledStructureMerger pfsm = new PartialFilledStructureMerger();
            IAtomContainer result = pfsm.Generate(acs);
            Assert.IsTrue(ConnectivityChecker.IsConnected(result));
            Assert.IsTrue(new SaturationChecker().AllSaturated(result));
        }
    }
}
