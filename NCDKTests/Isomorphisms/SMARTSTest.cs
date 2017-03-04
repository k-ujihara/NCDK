/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All I ask is that proper credit is given for my work, which includes
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
using NCDK.Default;
using NCDK.Isomorphisms.Matchers;
using NCDK.Smiles;
using NCDK.Templates;

namespace NCDK.Isomorphisms
{
    /// <summary>
    // @cdk.module  test-smarts
    // @cdk.require java1.4+
    /// </summary>
    [TestClass()]
    public class SMARTSTest : CDKTestCase
    {
        private UniversalIsomorphismTester uiTester = new UniversalIsomorphismTester();

        [TestMethod()]
        public void TestStrictSMARTS()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;

            SmilesParser sp = new SmilesParser(builder);
            IAtomContainer atomContainer = sp.ParseSmiles("CC(=O)OC(=O)C"); // acetic acid anhydride
            var query = new QueryAtomContainer(builder);
            SymbolQueryAtom atom1 = new SymbolQueryAtom(builder);
            atom1.Symbol = "N";
            SymbolQueryAtom atom2 = new SymbolQueryAtom(builder);
            atom2.Symbol = "C";
            query.Atoms.Add(atom1);
            query.Atoms.Add(atom2);
            query.Bonds.Add(new OrderQueryBond(atom1, atom2, BondOrder.Double, builder));

            Assert.IsFalse(uiTester.IsSubgraph(atomContainer, query));
        }

        [TestMethod()]
        public void TestSMARTS()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            SmilesParser sp = new SmilesParser(builder);
            IAtomContainer atomContainer = sp.ParseSmiles("CC(=O)OC(=O)C"); // acetic acid anhydride
            var query = new QueryAtomContainer(builder);
            var atom1 = new Matchers.SMARTS.AnyAtom(builder);
            SymbolQueryAtom atom2 = new SymbolQueryAtom(builder);
            atom2.Symbol = "C";
            query.Atoms.Add(atom1);
            query.Atoms.Add(atom2);
            query.Bonds.Add(new OrderQueryBond(atom1, atom2, BondOrder.Double, builder));

            Assert.IsTrue(uiTester.IsSubgraph(atomContainer, query));
        }

        private IAtomContainer CreateEthane()
        {
            IAtomContainer container = new AtomContainer(); // SMILES "CC"
            IAtom carbon = new Atom("C");
            IAtom carbon2 = carbon.Builder.CreateAtom("C");
            carbon.ImplicitHydrogenCount = 3;
            carbon2.ImplicitHydrogenCount = 3;
            container.Atoms.Add(carbon);
            container.Atoms.Add(carbon2);
            container.Bonds.Add(carbon.Builder.CreateBond(carbon, carbon2, BondOrder.Single));
            return container;
        }

        [TestMethod()]
        public void TestImplicitHCountAtom()
        {
            IAtomContainer container = CreateEthane();

            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;

            QueryAtomContainer query1 = new QueryAtomContainer(builder); // SMARTS [h3][h3]
            var atom1 = new Matchers.SMARTS.ImplicitHCountAtom(3, builder);
            var atom2 = new Matchers.SMARTS.ImplicitHCountAtom(3, builder);
            query1.Atoms.Add(atom1);
            query1.Atoms.Add(atom2);
            query1.Bonds.Add(new OrderQueryBond(atom1, atom2, BondOrder.Single, builder));
            Assert.IsTrue(uiTester.IsSubgraph(container, query1));
        }

        [TestMethod()]
        public void TestImplicitHCountAtom2()
        {
            IAtomContainer container = CreateEthane();

            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;

            QueryAtomContainer query1 = new QueryAtomContainer(builder); // SMARTS [h3][h2]
            var atom1 = new Matchers.SMARTS.ImplicitHCountAtom(3, builder);
            var atom2 = new Matchers.SMARTS.ImplicitHCountAtom(2, builder);
            query1.Atoms.Add(atom1);
            query1.Atoms.Add(atom2);
            query1.Bonds.Add(new OrderQueryBond(atom1, atom2, BondOrder.Single, builder));
            Assert.IsFalse(uiTester.IsSubgraph(container, query1));
        }

        [TestMethod()]
        public void TestMatchInherited()
        {
            try
            {
                IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;

                SymbolQueryAtom c1 = new SymbolQueryAtom(new Atom("C"));
                SymbolAndChargeQueryAtom c2 = new SymbolAndChargeQueryAtom(new Atom("C"));

                IAtomContainer c = TestMoleculeFactory.MakeAlkane(2);

                QueryAtomContainer query1 = new QueryAtomContainer(builder);
                query1.Atoms.Add(c1);
                query1.Atoms.Add(c2);
                query1.Bonds.Add(new OrderQueryBond(c1, c2, BondOrder.Single, builder));
                Assert.IsTrue(uiTester.IsSubgraph(c, query1));

                var query = new QueryAtomContainer(builder);
                query.Atoms.Add(c1);
                query.Atoms.Add(c2);
                query.Bonds.Add(new Matchers.SMARTS.AnyOrderQueryBond(c1, c2, BondOrder.Single, builder));
                Assert.IsTrue(uiTester.IsSubgraph(c, query));

            }
            catch (CDKException exception)
            {
                Assert.Fail(exception.Message);
            }
        }
    }
}
