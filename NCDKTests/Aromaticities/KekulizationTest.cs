/*
 * Copyright (c) 2014 European Bioinformatics Institute (EMBL-EBI)
 *                    John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or (at
 * your option) any later version. All we ask is that proper credit is given
 * for our work, which includes - but is not limited to - adding the above
 * copyright notice to the beginning of your source code files, and to any
 * copyright notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * Any WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Common.Base;
using NCDK.Default;
using NCDK.Tools.Manipulator;

namespace NCDK.Aromaticities
{
    // @author John May
    [TestClass()]
    public class KekulizationTest
    {
        [TestMethod()]
        public void Benzene()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Bonds.Add(Bond(m, 0, 1, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 1, 2, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 2, 3, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 3, 4, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 4, 5, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 5, 0, BondOrder.Unset, true));
            AssertBondOrders(m, BondOrder.Double, BondOrder.Single, BondOrder.Double, BondOrder.Single, BondOrder.Double, BondOrder.Single);
        }

        // when a double bond is already set, it is not moved
        [TestMethod()]
        public void BenzeneWithExistingDoubleBond()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Bonds.Add(Bond(m, 0, 1, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 1, 2, BondOrder.Double, true)); // <-- already set
            m.Bonds.Add(Bond(m, 2, 3, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 3, 4, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 4, 5, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 5, 0, BondOrder.Unset, true));
            AssertBondOrders(m, BondOrder.Single, BondOrder.Double, BondOrder.Single, BondOrder.Double, BondOrder.Single, BondOrder.Double);
        }

        [TestMethod()]
        public void Pyrrole()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("N", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Bonds.Add(Bond(m, 0, 1, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 1, 2, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 2, 3, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 3, 4, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 4, 0, BondOrder.Unset, true));
            AssertBondOrders(m, BondOrder.Single, BondOrder.Double, BondOrder.Single, BondOrder.Double, BondOrder.Single);
        }

        [TestMethod()]
        public void PyrroleWithExplicitHydrogen()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("N", 0, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("H", 0, false));
            m.Bonds.Add(Bond(m, 0, 1, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 1, 2, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 2, 3, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 3, 4, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 4, 0, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 0, 5, BondOrder.Single, false));
            AssertBondOrders(m, BondOrder.Single, BondOrder.Double, BondOrder.Single, BondOrder.Double, BondOrder.Single, BondOrder.Single);
        }

        /// <summary>Hydrogens must be present - otherwise the kekulisation is ambiguous.</summary>
        [TestMethod()]
        [ExpectedException(typeof(CDKException))]
        public void PyrroleWithMissingHydrogen()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("N", 0, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Bonds.Add(Bond(m, 0, 1, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 1, 2, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 2, 3, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 3, 4, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 4, 0, BondOrder.Unset, true));
            AssertBondOrders(m);
        }

        /// @cdk.inchi InChI=1S/C10H8/c1-2-5-9-7-4-8-10(9)6-3-1/h1-8H
        [TestMethod()]
        public void Azulene()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 0, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 0, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Bonds.Add(Bond(m, 0, 1, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 1, 2, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 2, 3, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 3, 4, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 4, 5, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 5, 6, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 2, 6, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 6, 7, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 7, 8, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 8, 9, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 0, 9, BondOrder.Unset, true));
            AssertBondOrders(m, BondOrder.Double, BondOrder.Single, BondOrder.Double, BondOrder.Single, BondOrder.Double, BondOrder.Single, BondOrder.Single, BondOrder.Double, BondOrder.Single, BondOrder.Double, BondOrder.Single);
        }

        /// @cdk.inchi InChI=1S/C5H5NO/c7-6-4-2-1-3-5-6/h1-5H
        [TestMethod()]
        public void PyridineOxide()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("O", 0, false));
            m.Atoms.Add(Atom("N", 0, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms[0].FormalCharge = -1;
            m.Atoms[1].FormalCharge = +1;
            m.Bonds.Add(Bond(m, 0, 1, BondOrder.Single, false));
            m.Bonds.Add(Bond(m, 1, 2, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 2, 3, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 3, 4, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 4, 5, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 5, 6, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 1, 6, BondOrder.Unset, true));
            AssertBondOrders(m, BondOrder.Single, BondOrder.Double, BondOrder.Single, BondOrder.Double, BondOrder.Single, BondOrder.Double, BondOrder.Single);
        }

        /// @cdk.inchi InChI=1S/C5H5NO/c7-6-4-2-1-3-5-6/h1-5H
        [TestMethod()]
        public void PyridineOxideNonChargeSeparated()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("O", 0, false));
            m.Atoms.Add(Atom("N", 0, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Bonds.Add(Bond(m, 0, 1, BondOrder.Double, false));
            m.Bonds.Add(Bond(m, 1, 2, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 2, 3, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 3, 4, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 4, 5, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 5, 6, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 1, 6, BondOrder.Unset, true));
            AssertBondOrders(m, BondOrder.Double, BondOrder.Double, BondOrder.Single, BondOrder.Double, BondOrder.Single, BondOrder.Double, BondOrder.Single);
        }

        [TestMethod()]
        public void Furane()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("O", 0, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Bonds.Add(Bond(m, 0, 1, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 1, 2, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 2, 3, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 3, 4, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 4, 0, BondOrder.Unset, true));
            AssertBondOrders(m, BondOrder.Single, BondOrder.Double, BondOrder.Single, BondOrder.Double, BondOrder.Single);
        }

        /// <summary>
        /// As seen in: CHEBI:30858
        /// </summary>
        // @cdk.inchi InChI=1S/C4H4Te/c1-2-4-5-3-1/h1-4H
        [TestMethod()]
        public void Tellurophene()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("Te", 0, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Bonds.Add(Bond(m, 0, 1, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 1, 2, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 2, 3, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 3, 4, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 4, 0, BondOrder.Unset, true));
            AssertBondOrders(m, BondOrder.Single, BondOrder.Double, BondOrder.Single, BondOrder.Double, BondOrder.Single);
        }

        // @cdk.inchi InChI=1S/C5H5/c1-2-4-5-3-1/h1-5H/q-1
        [TestMethod()]
        public void CarbonAnion()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms[0].FormalCharge = -1;
            m.Bonds.Add(Bond(m, 0, 1, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 1, 2, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 2, 3, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 3, 4, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 4, 0, BondOrder.Unset, true));
            AssertBondOrders(m, BondOrder.Single, BondOrder.Double, BondOrder.Single, BondOrder.Double, BondOrder.Single);
        }

        // @cdk.inchi InChI=1S/C7H7/c1-2-4-6-7-5-3-1/h1-7H/q+1
        [TestMethod()]
        public void Tropylium()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms[0].FormalCharge = +1;
            m.Bonds.Add(Bond(m, 0, 1, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 1, 2, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 2, 3, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 3, 4, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 4, 5, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 5, 6, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 6, 0, BondOrder.Unset, true));
            AssertBondOrders(m, BondOrder.Single, BondOrder.Double, BondOrder.Single, BondOrder.Double, BondOrder.Single, BondOrder.Double, BondOrder.Single);
        }

        /// <summary>
        /// example seen in: CHEMBL141536
        /// </summary>
        // @cdk.inchi InChI=1S/C5H5Se/c1-2-4-6-5-3-1/h1-5H/q+1
        [TestMethod()]
        public void SeleniumCation()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("Se", 0, true));
            m.Atoms[5].FormalCharge = +1;
            m.Bonds.Add(Bond(m, 0, 1, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 1, 2, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 2, 3, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 3, 4, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 4, 5, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 5, 0, BondOrder.Unset, true));
            AssertBondOrders(m, BondOrder.Double, BondOrder.Single, BondOrder.Double, BondOrder.Single, BondOrder.Double, BondOrder.Single);
        }

        /// <summary>
        /// example seen in: CHEMBL13520
        /// </summary>
        // @cdk.inchi InChI=1/C11H9N3O3S/c1-18(17)9-5-3-2-4-8(9)14-6-7(10(15)16)12-11(14)13-18/h2-6H,1H3,(H,15,16)
        [TestMethod()]
        public void SixValentSulphur()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 3, false));
            m.Atoms.Add(Atom("S", 0, true));
            m.Atoms.Add(Atom("O", 0, false));
            m.Atoms.Add(Atom("N", 0, true));
            m.Atoms.Add(Atom("C", 0, true));
            m.Atoms.Add(Atom("N", 0, true));
            m.Atoms.Add(Atom("C", 0, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("N", 0, true));
            m.Atoms.Add(Atom("C", 0, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 0, true));
            m.Atoms.Add(Atom("C", 0, false));
            m.Atoms.Add(Atom("O", 0, false));
            m.Atoms.Add(Atom("O", 1, false));
            m.Bonds.Add(Bond(m, 0, 1, BondOrder.Single, false));
            m.Bonds.Add(Bond(m, 1, 2, BondOrder.Double, false));
            m.Bonds.Add(Bond(m, 1, 3, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 3, 4, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 4, 5, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 5, 6, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 6, 7, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 7, 8, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 4, 8, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 8, 9, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 9, 10, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 10, 11, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 11, 12, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 12, 13, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 13, 14, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 1, 14, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 9, 14, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 6, 15, BondOrder.Single, false));
            m.Bonds.Add(Bond(m, 15, 16, BondOrder.Double, false));
            m.Bonds.Add(Bond(m, 15, 17, BondOrder.Single, false));
            AssertBondOrders(m, BondOrder.Single, BondOrder.Double, BondOrder.Double, BondOrder.Single, BondOrder.Double, BondOrder.Single, BondOrder.Double, BondOrder.Single, BondOrder.Single, BondOrder.Single, BondOrder.Double,
                    BondOrder.Single, BondOrder.Double, BondOrder.Single, BondOrder.Double, BondOrder.Single, BondOrder.Single, BondOrder.Single, BondOrder.Double, BondOrder.Single);
        }

        // @cdk.inchi InChI=1S/C12H10/c1-3-7-11(8-4-1)12-9-5-2-6-10-12/h1-10H
        [TestMethod()]
        public void Biphenyl()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 0, true));
            m.Atoms.Add(Atom("C", 0, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Bonds.Add(Bond(m, 0, 1, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 1, 2, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 2, 3, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 3, 4, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 4, 5, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 0, 5, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 5, 6, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 6, 7, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 7, 8, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 8, 9, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 9, 10, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 10, 11, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 6, 11, BondOrder.Unset, true));
            AssertBondOrders(m, BondOrder.Double, BondOrder.Single, BondOrder.Double, BondOrder.Single, BondOrder.Double, BondOrder.Single, BondOrder.Single, BondOrder.Double, BondOrder.Single, BondOrder.Double, BondOrder.Single,
                    BondOrder.Double, BondOrder.Single);
        }

        // @cdk.inchi InChI=1S/C6H4O2/c7-5-1-2-6(8)4-3-5/h1-4H
        [TestMethod()]
        public void Quinone()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("O", 0, true));
            m.Atoms.Add(Atom("C", 0, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 0, true));
            m.Atoms.Add(Atom("O", 0, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Bonds.Add(Bond(m, 0, 1, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 1, 2, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 2, 3, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 3, 4, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 4, 5, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 4, 6, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 6, 7, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 1, 7, BondOrder.Unset, true));
            AssertBondOrders(m, BondOrder.Double, BondOrder.Single, BondOrder.Double, BondOrder.Single, BondOrder.Double, BondOrder.Single, BondOrder.Double, BondOrder.Single);
        }

        /// <summary>
        /// InChI=1S/C13H10/c1-3-7-12-10(5-1)9-11-6-2-4-8-13(11)12/h1-8H,9H2
        /// </summary>
        [TestMethod()]
        public void Fluorene()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 2, false));
            m.Atoms.Add(Atom("C", 0, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 0, true));
            m.Atoms.Add(Atom("C", 0, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 0, true));
            m.Bonds.Add(Bond(m, 0, 1, BondOrder.Single, false));
            m.Bonds.Add(Bond(m, 1, 2, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 2, 3, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 3, 4, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 4, 5, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 5, 6, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 1, 6, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 6, 7, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 7, 8, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 8, 9, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 9, 10, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 10, 11, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 11, 12, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 0, 12, BondOrder.Single, false));
            m.Bonds.Add(Bond(m, 7, 12, BondOrder.Unset, true));
            AssertBondOrders(m, BondOrder.Single, BondOrder.Double, BondOrder.Single, BondOrder.Double, BondOrder.Single, BondOrder.Double, BondOrder.Single, BondOrder.Single, BondOrder.Double, BondOrder.Single, BondOrder.Double,
                    BondOrder.Single, BondOrder.Double, BondOrder.Single, BondOrder.Single);
        }

        /// @cdk.inchi InChI=1S/C5H5B/c1-2-4-6-5-3-1/h1-5H
        [TestMethod()]
        public void Borinine()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("B", 0, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Bonds.Add(Bond(m, 0, 1, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 1, 2, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 2, 3, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 3, 4, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 4, 5, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 5, 0, BondOrder.Unset, true));
            AssertBondOrders(m, BondOrder.Double, BondOrder.Single, BondOrder.Double, BondOrder.Single, BondOrder.Double, BondOrder.Single);
        }

        // e.g. CHEMBL422679
        [TestMethod()]
        public void SulfurCation()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("S", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms.Add(Atom("C", 1, true));
            m.Atoms[0].FormalCharge = +1;
            m.Bonds.Add(Bond(m, 0, 1, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 1, 2, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 2, 3, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 3, 4, BondOrder.Unset, true));
            m.Bonds.Add(Bond(m, 4, 0, BondOrder.Unset, true));
            AssertBondOrders(m, BondOrder.Single, BondOrder.Double, BondOrder.Single, BondOrder.Double, BondOrder.Single);
        }

        void AssertBondOrders(IAtomContainer ac, params BondOrder[] expected)
        {
            Kekulization.Kekulize(ac);
            IBond[] bonds = AtomContainerManipulator.GetBondArray(ac);
            BondOrder[] actual = new BondOrder[bonds.Length];
            for (int i = 0; i < bonds.Length; i++)
                actual[i] = bonds[i].Order;
            Assert.IsTrue(Compares.AreEqual(expected, actual));
        }

        static IAtom Atom(string symbol, int h, bool arom)
        {
            IAtom a = new Atom(symbol)
            {
                ImplicitHydrogenCount = h,
                IsAromatic = arom
            };
            return a;
        }

        static IBond Bond(IAtomContainer m, int v, int w, BondOrder ord, bool arom)
        {
            IBond b = new Bond(m.Atoms[v], m.Atoms[w])
            {
                Order = ord,
                IsAromatic = arom
            };
            return b;
        }
    }
}
