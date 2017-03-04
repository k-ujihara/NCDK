/* Copyright (C) 2006-2007  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Config;
using NCDK.Default;

namespace NCDK.Tools
{
    /// <summary>
    /// Test suite for testing deduce-bond-order implementations.
    /// This suite tests deduction from hybridization rich starting
    /// points, excluding, but optional, implicit or explicit
    /// hydrogen counts.
    ///
    // @author      egonw
    // @cdk.module  test-valencycheck
    // @cdk.created 2006-08-16
    /// </summary>
    [TestClass()]
    public class DeduceBondOrderTestFromExplicitHydrogens : CDKTestCase
    {

        private IDeduceBondOrderTool dboTool = new SaturationChecker();

        /// <summary>
        /// Test <div class="inchi">InChI=1/C2H2/c1-2/h1-2H</div>.
        /// </summary>
        [TestMethod()]
        public void TestAcetylene()
        {
            IAtomContainer keto = new AtomContainer();

            // atom block
            IAtom atom1 = new Atom(Elements.Carbon.ToIElement());
            AddHydrogens(keto, atom1, 1);
            IAtom atom2 = new Atom(Elements.Carbon.ToIElement());
            AddHydrogens(keto, atom2, 1);

            // bond block
            IBond bond1 = new Bond(atom1, atom2);

            keto.Atoms.Add(atom1);
            keto.Atoms.Add(atom2);
            keto.Bonds.Add(bond1);

            // now have the algorithm have a go at it
            dboTool.Saturate(keto);

            // now check whether it did the right thing
            Assert.AreEqual(BondOrder.Triple, bond1.Order);
        }

        /// <summary>
        /// Test <div class="inchi">InChI=1/C2H4O/c1-2-3/h2H,1H3</div>.
        /// </summary>
        [TestMethod()]
        public void TestKeto()
        {
            IAtomContainer keto = new AtomContainer();

            // atom block
            IAtom atom1 = new Atom(Elements.Carbon.ToIElement());
            AddHydrogens(keto, atom1, 3);
            IAtom atom2 = new Atom(Elements.Carbon.ToIElement());
            AddHydrogens(keto, atom2, 1);
            IAtom atom3 = new Atom(Elements.Oxygen.ToIElement());

            // bond block
            IBond bond1 = new Bond(atom1, atom2);
            IBond bond2 = new Bond(atom2, atom3);

            keto.Atoms.Add(atom1);
            keto.Atoms.Add(atom2);
            keto.Atoms.Add(atom3);
            keto.Bonds.Add(bond1);
            keto.Bonds.Add(bond2);

            // now have the algorithm have a go at it
            dboTool.Saturate(keto);

            // now check whether it did the right thing
            Assert.AreEqual(BondOrder.Single, bond1.Order);
            Assert.AreEqual(BondOrder.Double, bond2.Order);
        }

        /// <summary>
        /// Test <div class="inchi">InChI=1/C2H6O/c1-2-3/h3H,2H2,1H3</div>.
        /// </summary>
        [TestMethod()]
        public void TestEnol()
        {
            IAtomContainer enol = new AtomContainer();

            // atom block
            IAtom atom1 = new Atom(Elements.Carbon.ToIElement());
            AddHydrogens(enol, atom1, 2);
            IAtom atom2 = new Atom(Elements.Carbon.ToIElement());
            AddHydrogens(enol, atom2, 1);
            IAtom atom3 = new Atom(Elements.Oxygen.ToIElement());
            AddHydrogens(enol, atom3, 1);

            // bond block
            IBond bond1 = new Bond(atom1, atom2);
            IBond bond2 = new Bond(atom2, atom3);

            enol.Atoms.Add(atom1);
            enol.Atoms.Add(atom2);
            enol.Atoms.Add(atom3);
            enol.Bonds.Add(bond1);
            enol.Bonds.Add(bond2);

            // now have the algorithm have a go at it
            dboTool.Saturate(enol);

            // now check whether it did the right thing
            Assert.AreEqual(BondOrder.Double, bond1.Order);
            Assert.AreEqual(BondOrder.Single, bond2.Order);
        }

        /// <summary>
        /// Test <div class="inchi">InChI=1/C4H6/c1-3-4-2/h3-4H,1-2H2</div>.
        /// </summary>
        [TestMethod()]
        public void XtestButadiene()
        {
            IAtomContainer enol = new AtomContainer();

            // atom block
            IAtom atom1 = new Atom(Elements.Carbon.ToIElement());
            AddHydrogens(enol, atom1, 2);
            IAtom atom2 = new Atom(Elements.Carbon.ToIElement());
            AddHydrogens(enol, atom2, 1);
            IAtom atom3 = new Atom(Elements.Carbon.ToIElement());
            AddHydrogens(enol, atom3, 1);
            IAtom atom4 = new Atom(Elements.Carbon.ToIElement());
            AddHydrogens(enol, atom4, 2);

            // bond block
            IBond bond1 = new Bond(atom1, atom2);
            IBond bond2 = new Bond(atom2, atom3);
            IBond bond3 = new Bond(atom3, atom4);

            enol.Atoms.Add(atom1);
            enol.Atoms.Add(atom2);
            enol.Atoms.Add(atom3);
            enol.Atoms.Add(atom4);
            enol.Bonds.Add(bond1);
            enol.Bonds.Add(bond2);
            enol.Bonds.Add(bond3);

            // now have the algorithm have a go at it
            dboTool.Saturate(enol);

            // now check whether it did the right thing
            Assert.AreEqual(BondOrder.Double, bond1.Order);
            Assert.AreEqual(BondOrder.Single, bond2.Order);
            Assert.AreEqual(BondOrder.Double, bond3.Order);
        }

        /// <summary>
        /// Test <div class="inchi">InChI=1/C6H4O2/c7-5-1-2-6(8)4-3-5/h1-4H</div>.
        /// </summary>
        [TestMethod()]
        public void TestQuinone()
        {
            IAtomContainer enol = new AtomContainer();

            // atom block
            IAtom atom1 = new Atom(Elements.Carbon.ToIElement());
            IAtom atom2 = new Atom(Elements.Carbon.ToIElement());
            AddHydrogens(enol, atom2, 1);
            IAtom atom3 = new Atom(Elements.Carbon.ToIElement());
            AddHydrogens(enol, atom3, 1);
            IAtom atom4 = new Atom(Elements.Carbon.ToIElement());
            IAtom atom5 = new Atom(Elements.Carbon.ToIElement());
            AddHydrogens(enol, atom5, 1);
            IAtom atom6 = new Atom(Elements.Carbon.ToIElement());
            AddHydrogens(enol, atom6, 1);
            IAtom atom7 = new Atom(Elements.Oxygen.ToIElement());
            IAtom atom8 = new Atom(Elements.Oxygen.ToIElement());

            // bond block
            IBond bond1 = new Bond(atom1, atom2);
            IBond bond2 = new Bond(atom2, atom3);
            IBond bond3 = new Bond(atom3, atom4);
            IBond bond4 = new Bond(atom4, atom5);
            IBond bond5 = new Bond(atom5, atom6);
            IBond bond6 = new Bond(atom6, atom1);
            IBond bond7 = new Bond(atom7, atom1);
            IBond bond8 = new Bond(atom8, atom4);

            enol.Atoms.Add(atom1);
            enol.Atoms.Add(atom2);
            enol.Atoms.Add(atom3);
            enol.Atoms.Add(atom4);
            enol.Atoms.Add(atom5);
            enol.Atoms.Add(atom6);
            enol.Atoms.Add(atom7);
            enol.Atoms.Add(atom8);
            enol.Bonds.Add(bond1);
            enol.Bonds.Add(bond2);
            enol.Bonds.Add(bond3);
            enol.Bonds.Add(bond4);
            enol.Bonds.Add(bond5);
            enol.Bonds.Add(bond6);
            enol.Bonds.Add(bond7);
            enol.Bonds.Add(bond8);

            // now have the algorithm have a go at it
            dboTool.Saturate(enol);

            // now check whether it did the right thing
            Assert.AreEqual(BondOrder.Single, bond1.Order);
            Assert.AreEqual(BondOrder.Double, bond2.Order);
            Assert.AreEqual(BondOrder.Single, bond3.Order);
            Assert.AreEqual(BondOrder.Single, bond4.Order);
            Assert.AreEqual(BondOrder.Double, bond5.Order);
            Assert.AreEqual(BondOrder.Single, bond6.Order);
            Assert.AreEqual(BondOrder.Double, bond7.Order);
            Assert.AreEqual(BondOrder.Double, bond8.Order);
        }

        /// <summary>
        /// Test <div class="inchi">InChI=1/C6H6/c1-2-4-6-5-3-1/h1-6H</div>.
        /// </summary>
        [TestMethod()]
        public void TestBenzene()
        {
            IAtomContainer enol = new AtomContainer();

            // atom block
            IAtom atom1 = new Atom(Elements.Carbon.ToIElement());
            AddHydrogens(enol, atom1, 1);
            IAtom atom2 = new Atom(Elements.Carbon.ToIElement());
            AddHydrogens(enol, atom2, 1);
            IAtom atom3 = new Atom(Elements.Carbon.ToIElement());
            AddHydrogens(enol, atom3, 1);
            IAtom atom4 = new Atom(Elements.Carbon.ToIElement());
            AddHydrogens(enol, atom4, 1);
            IAtom atom5 = new Atom(Elements.Carbon.ToIElement());
            AddHydrogens(enol, atom5, 1);
            IAtom atom6 = new Atom(Elements.Carbon.ToIElement());
            AddHydrogens(enol, atom6, 1);

            // bond block
            IBond bond1 = new Bond(atom1, atom2);
            IBond bond2 = new Bond(atom2, atom3);
            IBond bond3 = new Bond(atom3, atom4);
            IBond bond4 = new Bond(atom4, atom5);
            IBond bond5 = new Bond(atom5, atom6);
            IBond bond6 = new Bond(atom6, atom1);

            enol.Atoms.Add(atom1);
            enol.Atoms.Add(atom2);
            enol.Atoms.Add(atom3);
            enol.Atoms.Add(atom4);
            enol.Atoms.Add(atom5);
            enol.Atoms.Add(atom6);
            enol.Bonds.Add(bond1);
            enol.Bonds.Add(bond2);
            enol.Bonds.Add(bond3);
            enol.Bonds.Add(bond4);
            enol.Bonds.Add(bond5);
            enol.Bonds.Add(bond6);

            // now have the algorithm have a go at it
            dboTool.Saturate(enol);

            // now check whether it did the right thing
            Assert.AreEqual(BondOrder.Single.Numeric + BondOrder.Double.Numeric, bond1
                    .Order.Numeric + bond6.Order.Numeric); // around atom1
            Assert.AreEqual(BondOrder.Single.Numeric + BondOrder.Double.Numeric, bond1
                    .Order.Numeric + bond2.Order.Numeric); // around atom2
            Assert.AreEqual(BondOrder.Single.Numeric + BondOrder.Double.Numeric, bond2
                    .Order.Numeric + bond3.Order.Numeric); // around atom3
            Assert.AreEqual(BondOrder.Single.Numeric + BondOrder.Double.Numeric, bond3
                    .Order.Numeric + bond4.Order.Numeric); // around atom4
            Assert.AreEqual(BondOrder.Single.Numeric + BondOrder.Double.Numeric, bond4
                    .Order.Numeric + bond5.Order.Numeric); // around atom5
            Assert.AreEqual(BondOrder.Single.Numeric + BondOrder.Double.Numeric, bond5
                    .Order.Numeric + bond6.Order.Numeric); // around atom6
        }

        /// <summary>
        /// Test <div class="inchi">InChI=1/C4H5N/c1-2-4-5-3-1/h1-5H</div>.
        /// </summary>
        [TestMethod()]
        public void TestPyrrole()
        {
            IAtomContainer enol = new AtomContainer();

            // atom block
            IAtom atom1 = new Atom(Elements.Carbon.ToIElement());
            AddHydrogens(enol, atom1, 1);
            IAtom atom2 = new Atom(Elements.Carbon.ToIElement());
            AddHydrogens(enol, atom2, 1);
            IAtom atom3 = new Atom(Elements.Carbon.ToIElement());
            AddHydrogens(enol, atom3, 1);
            IAtom atom4 = new Atom(Elements.Carbon.ToIElement());
            AddHydrogens(enol, atom4, 1);
            IAtom atom5 = new Atom(Elements.Nitrogen.ToIElement());
            AddHydrogens(enol, atom5, 1);

            // bond block
            IBond bond1 = new Bond(atom1, atom2);
            IBond bond2 = new Bond(atom2, atom3);
            IBond bond3 = new Bond(atom3, atom4);
            IBond bond4 = new Bond(atom4, atom5);
            IBond bond5 = new Bond(atom5, atom1);

            enol.Atoms.Add(atom1);
            enol.Atoms.Add(atom2);
            enol.Atoms.Add(atom3);
            enol.Atoms.Add(atom4);
            enol.Atoms.Add(atom5);
            enol.Bonds.Add(bond1);
            enol.Bonds.Add(bond2);
            enol.Bonds.Add(bond3);
            enol.Bonds.Add(bond4);
            enol.Bonds.Add(bond5);

            // now have the algorithm have a go at it
            dboTool.Saturate(enol);

            // now check whether it did the right thing
            Assert.AreEqual(BondOrder.Double, bond1.Order);
            Assert.AreEqual(BondOrder.Single, bond2.Order);
            Assert.AreEqual(BondOrder.Double, bond3.Order);
            Assert.AreEqual(BondOrder.Single, bond4.Order);
            Assert.AreEqual(BondOrder.Single, bond5.Order);
        }

        /// <summary>
        /// Test <div class="inchi">InChI=1/C5H5N/c1-2-4-6-5-3-1/h1-5H</div>.
        /// </summary>
        //@Ignore("previously disabled 'xtest'")
        public void XtestPyridine()
        {
            IAtomContainer enol = new AtomContainer();

            // atom block
            IAtom atom1 = new Atom(Elements.Carbon.ToIElement());
            AddHydrogens(enol, atom1, 1);
            IAtom atom2 = new Atom(Elements.Carbon.ToIElement());
            AddHydrogens(enol, atom2, 1);
            IAtom atom3 = new Atom(Elements.Carbon.ToIElement());
            AddHydrogens(enol, atom3, 1);
            IAtom atom4 = new Atom(Elements.Carbon.ToIElement());
            AddHydrogens(enol, atom4, 1);
            IAtom atom5 = new Atom(Elements.Nitrogen.ToIElement());
            IAtom atom6 = new Atom(Elements.Carbon.ToIElement());
            AddHydrogens(enol, atom6, 1);

            // bond block
            IBond bond1 = new Bond(atom1, atom2);
            IBond bond2 = new Bond(atom2, atom3);
            IBond bond3 = new Bond(atom3, atom4);
            IBond bond4 = new Bond(atom4, atom5);
            IBond bond5 = new Bond(atom5, atom6);
            IBond bond6 = new Bond(atom6, atom1);

            enol.Atoms.Add(atom1);
            enol.Atoms.Add(atom2);
            enol.Atoms.Add(atom3);
            enol.Atoms.Add(atom4);
            enol.Atoms.Add(atom5);
            enol.Atoms.Add(atom6);
            enol.Bonds.Add(bond1);
            enol.Bonds.Add(bond2);
            enol.Bonds.Add(bond3);
            enol.Bonds.Add(bond4);
            enol.Bonds.Add(bond5);
            enol.Bonds.Add(bond6);

            // now have the algorithm have a go at it
            dboTool.Saturate(enol);

            // now check whether it did the right thing
            Assert.AreEqual(BondOrder.Single.Numeric + BondOrder.Double.Numeric, bond1
                    .Order.Numeric + bond6.Order.Numeric); // around atom1
            Assert.AreEqual(BondOrder.Single.Numeric + BondOrder.Double.Numeric, bond1
                    .Order.Numeric + bond2.Order.Numeric); // around atom2
            Assert.AreEqual(BondOrder.Single.Numeric + BondOrder.Double.Numeric, bond2
                    .Order.Numeric + bond3.Order.Numeric); // around atom3
            Assert.AreEqual(BondOrder.Single.Numeric + BondOrder.Double.Numeric, bond3
                    .Order.Numeric + bond4.Order.Numeric); // around atom4
            Assert.AreEqual(BondOrder.Single.Numeric + BondOrder.Double.Numeric, bond4
                    .Order.Numeric + bond5.Order.Numeric); // around atom5
            Assert.AreEqual(BondOrder.Single.Numeric + BondOrder.Double.Numeric, bond5
                    .Order.Numeric + bond6.Order.Numeric); // around atom6
        }

        private void AddHydrogens(IAtomContainer container, IAtom atom, int numberOfHydrogens)
        {
            for (int i = 0; i < numberOfHydrogens; i++)
                container.Bonds.Add(atom.Builder.CreateBond(atom,
                        atom.Builder.CreateAtom("H")));
        }
    }
}
