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
using NCDK.Smiles;
using NCDK.Tools;

namespace NCDK.StructGen.Stochastic
{
    /// <summary>
    // @cdk.module test-structgen
    /// </summary>
    [TestClass()]
    public class PartialFilledStructureMergerTest : CDKTestCase
    {

        [TestMethod()]
        public void TestGenerate_IAtomContainerSet()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            var acs = Default.ChemObjectBuilder.Instance.CreateAtomContainerSet();
            acs.Add(sp.ParseSmiles("CCCCC"));
            acs.Add(sp.ParseSmiles("C1=C(C1)CC"));
            acs[0].Atoms[0].ImplicitHydrogenCount = 2;
            acs[0].Atoms[1].ImplicitHydrogenCount = 2;
            acs[0].Atoms[2].ImplicitHydrogenCount = 2;
            acs[0].Atoms[3].ImplicitHydrogenCount = 2;
            acs[0].Atoms[4].ImplicitHydrogenCount = 2;
            acs[1].Atoms[0].ImplicitHydrogenCount = 0;
            acs[1].Atoms[1].ImplicitHydrogenCount = 0;
            acs[1].Atoms[2].ImplicitHydrogenCount = 2;
            acs[1].Atoms[3].ImplicitHydrogenCount = 2;
            acs[1].Atoms[4].ImplicitHydrogenCount = 2;
            PartialFilledStructureMerger pfsm = new PartialFilledStructureMerger();
            IAtomContainer result = pfsm.Generate(acs);
            Assert.IsTrue(ConnectivityChecker.IsConnected(result));
            Assert.IsTrue(new SaturationChecker().AllSaturated(result));
        }

        [TestMethod()]
        public void TestPartialFilledStructureMerger2()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            var acs = Default.ChemObjectBuilder.Instance.CreateAtomContainerSet();
            acs.Add(sp.ParseSmiles("C=CCCC"));
            acs.Add(sp.ParseSmiles("C(C)=C1CC1"));
            acs[0].Atoms[0].ImplicitHydrogenCount = 0;
            acs[0].Atoms[1].ImplicitHydrogenCount = 0;
            acs[0].Atoms[2].ImplicitHydrogenCount = 2;
            acs[0].Atoms[3].ImplicitHydrogenCount = 2;
            acs[0].Atoms[4].ImplicitHydrogenCount = 2;
            acs[1].Atoms[0].ImplicitHydrogenCount = 0;
            acs[1].Atoms[1].ImplicitHydrogenCount = 2;
            acs[1].Atoms[2].ImplicitHydrogenCount = 0;
            acs[1].Atoms[3].ImplicitHydrogenCount = 2;
            acs[1].Atoms[4].ImplicitHydrogenCount = 2;
            PartialFilledStructureMerger pfsm = new PartialFilledStructureMerger();
            IAtomContainer result = pfsm.Generate(acs);
            Assert.IsTrue(ConnectivityChecker.IsConnected(result));
            Assert.IsTrue(new SaturationChecker().AllSaturated(result));
        }

        [TestMethod()]
        public void TestPartialFilledStructureMerger3()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            var acs = Default.ChemObjectBuilder.Instance.CreateAtomContainerSet();
            acs.Add(sp.ParseSmiles("CCCCC"));
            acs.Add(sp.ParseSmiles("CCC"));
            acs.Add(sp.ParseSmiles("CC"));
            acs[0].Atoms[0].ImplicitHydrogenCount = 2;
            acs[0].Atoms[1].ImplicitHydrogenCount = 2;
            acs[0].Atoms[2].ImplicitHydrogenCount = 2;
            acs[0].Atoms[3].ImplicitHydrogenCount = 2;
            acs[0].Atoms[4].ImplicitHydrogenCount = 2;
            acs[1].Atoms[0].ImplicitHydrogenCount = 2;
            acs[1].Atoms[1].ImplicitHydrogenCount = 2;
            acs[1].Atoms[2].ImplicitHydrogenCount = 2;
            acs[2].Atoms[0].ImplicitHydrogenCount = 2;
            acs[2].Atoms[1].ImplicitHydrogenCount = 2;
            PartialFilledStructureMerger pfsm = new PartialFilledStructureMerger();
            IAtomContainer result = pfsm.Generate(acs);
            Assert.IsTrue(ConnectivityChecker.IsConnected(result));
            Assert.IsTrue(new SaturationChecker().AllSaturated(result));
        }

        [TestMethod()]
        public void TestPartialFilledStructureMerger4()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            var acs = Default.ChemObjectBuilder.Instance.CreateAtomContainerSet();
            acs.Add(sp.ParseSmiles("CCCCC"));
            acs.Add(sp.ParseSmiles("CCCC"));
            acs.Add(sp.ParseSmiles("C"));
            acs[0].Atoms[0].ImplicitHydrogenCount = 0;
            acs[0].Atoms[1].ImplicitHydrogenCount = 2;
            acs[0].Atoms[2].ImplicitHydrogenCount = 2;
            acs[0].Atoms[3].ImplicitHydrogenCount = 2;
            acs[0].Atoms[4].ImplicitHydrogenCount = 2;
            acs[1].Atoms[0].ImplicitHydrogenCount = 0;
            acs[1].Atoms[1].ImplicitHydrogenCount = 2;
            acs[1].Atoms[2].ImplicitHydrogenCount = 2;
            acs[1].Atoms[3].ImplicitHydrogenCount = 2;
            acs[2].Atoms[0].ImplicitHydrogenCount = 2;
            PartialFilledStructureMerger pfsm = new PartialFilledStructureMerger();
            IAtomContainer result = pfsm.Generate(acs);
            Assert.IsTrue(ConnectivityChecker.IsConnected(result));
            Assert.IsTrue(new SaturationChecker().AllSaturated(result));
        }

        [TestMethod()]
        public void TestPartialFilledStructureMerger5()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            var acs = Default.ChemObjectBuilder.Instance.CreateAtomContainerSet();
            acs.Add(sp.ParseSmiles("C1CCC1"));
            acs.Add(sp.ParseSmiles("C(C)CCC"));
            acs.Add(sp.ParseSmiles("C"));
            acs[0].Atoms[0].ImplicitHydrogenCount = 0;
            acs[0].Atoms[1].ImplicitHydrogenCount = 2;
            acs[0].Atoms[2].ImplicitHydrogenCount = 2;
            acs[0].Atoms[3].ImplicitHydrogenCount = 2;
            acs[1].Atoms[0].ImplicitHydrogenCount = 0;
            acs[1].Atoms[1].ImplicitHydrogenCount = 2;
            acs[1].Atoms[2].ImplicitHydrogenCount = 2;
            acs[1].Atoms[3].ImplicitHydrogenCount = 2;
            acs[1].Atoms[4].ImplicitHydrogenCount = 2;
            acs[2].Atoms[0].ImplicitHydrogenCount = 2;
            PartialFilledStructureMerger pfsm = new PartialFilledStructureMerger();
            IAtomContainer result = pfsm.Generate(acs);
            Assert.IsTrue(ConnectivityChecker.IsConnected(result));
            Assert.IsTrue(new SaturationChecker().AllSaturated(result));
        }
    }
}
