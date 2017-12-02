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
using Moq;
using NCDK.Default;
using System;

namespace NCDK.Stereo
{
    // @author John May
    [TestClass()]
    public sealed class ExtendedTetrahedralTest
    {
        [TestMethod()]
        public void PeripheralsAreCopied()
        {
            IAtom focus = new Mock<IAtom>().Object;
            IAtom[] peripherals = new IAtom[] { new Mock<IAtom>().Object, new Mock<IAtom>().Object, new Mock<IAtom>().Object, new Mock<IAtom>().Object };
            ExtendedTetrahedral element = new ExtendedTetrahedral(focus, peripherals, TetrahedralStereo.Clockwise);

            // modifying this array does not change the one in the structure
            peripherals[0] = peripherals[1] = peripherals[2] = peripherals[3] = null;
            Assert.IsNotNull(element.Peripherals[0]);
            Assert.IsNotNull(element.Peripherals[1]);
            Assert.IsNotNull(element.Peripherals[2]);
            Assert.IsNotNull(element.Peripherals[3]);
        }

        [TestMethod()]
        public void PeripheralsAreNotModifable()
        {
            IAtom focus = new Mock<IAtom>().Object;
            IAtom[] peripherals = new IAtom[] { new Mock<IAtom>().Object, new Mock<IAtom>().Object, new Mock<IAtom>().Object, new Mock<IAtom>().Object };
            ExtendedTetrahedral element = new ExtendedTetrahedral(focus, peripherals, TetrahedralStereo.Clockwise);

            // modifying this array does not change the one in the structure
            peripherals = element.Peripherals;
            peripherals[0] = peripherals[1] = peripherals[2] = peripherals[3] = null;
            Assert.IsNotNull(element.Peripherals[0]);
            Assert.IsNotNull(element.Peripherals[1]);
            Assert.IsNotNull(element.Peripherals[2]);
            Assert.IsNotNull(element.Peripherals[3]);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void NonCumulatedAtomThrowsException()
        {
            IAtomContainer ac = new AtomContainer();
            ac.Atoms.Add(new Atom("C"));
            ac.Atoms.Add(new Atom("C"));
            ac.Atoms.Add(new Atom("C"));
            ac.AddBond(ac.Atoms[0], ac.Atoms[1], BondOrder.Single);
            ac.AddBond(ac.Atoms[1], ac.Atoms[2], BondOrder.Single);
            ExtendedTetrahedral.FindTerminalAtoms(ac, ac.Atoms[0]);
        }

        [TestMethod()]
        public void TerminalAtomsAreFoundUnordered()
        {
            IAtomContainer ac = new AtomContainer();
            ac.Atoms.Add(new Atom("C"));
            ac.Atoms.Add(new Atom("C"));
            ac.Atoms.Add(new Atom("C"));
            ac.AddBond(ac.Atoms[0], ac.Atoms[1], BondOrder.Double);
            ac.AddBond(ac.Atoms[1], ac.Atoms[2], BondOrder.Double);
            IAtom[] terminals = ExtendedTetrahedral.FindTerminalAtoms(ac, ac.Atoms[1]);
            // note order may change
            Assert.AreEqual(ac.Atoms[0], terminals[0]);
            Assert.AreEqual(ac.Atoms[2], terminals[1]);
        }

        [TestMethod()]
        public void TerminalAtomsAreFoundOrdered()
        {
            IAtomContainer ac = new AtomContainer();
            ac.Atoms.Add(new Atom("C"));
            ac.Atoms.Add(new Atom("C"));
            ac.Atoms.Add(new Atom("C"));
            ac.Atoms.Add(new Atom("C"));
            ac.Atoms.Add(new Atom("C"));
            ac.AddBond(ac.Atoms[0], ac.Atoms[1], BondOrder.Single);
            ac.AddBond(ac.Atoms[1], ac.Atoms[2], BondOrder.Double);
            ac.AddBond(ac.Atoms[2], ac.Atoms[3], BondOrder.Double);
            ac.AddBond(ac.Atoms[3], ac.Atoms[4], BondOrder.Single);

            ExtendedTetrahedral element = new ExtendedTetrahedral(ac.Atoms[2], new IAtom[]{ac.Atoms[4], ac.Atoms[3],
                ac.Atoms[1], ac.Atoms[0]}, TetrahedralStereo.Clockwise);

            IAtom[] terminals = element.FindTerminalAtoms(ac);
            Assert.AreEqual(ac.Atoms[3], terminals[0]);
            Assert.AreEqual(ac.Atoms[1], terminals[1]);
        }

        [TestMethod()]
        [ExpectedException(typeof(NotSupportedException))]
        public void NoBuilder()
        {
            IAtom focus = new Mock<IAtom>().Object;
            IAtom[] peripherals = new IAtom[] { new Mock<IAtom>().Object, new Mock<IAtom>().Object, new Mock<IAtom>().Object, new Mock<IAtom>().Object };
            ExtendedTetrahedral element = new ExtendedTetrahedral(focus, peripherals, TetrahedralStereo.Clockwise);
            var dummy = element.Builder;
        }

        [TestMethod()]
        public void ContainsAnAtom()
        {
            IAtom focus = new Mock<IAtom>().Object;
            IAtom[] peripherals = new IAtom[] { new Mock<IAtom>().Object, new Mock<IAtom>().Object, new Mock<IAtom>().Object, new Mock<IAtom>().Object };
            ExtendedTetrahedral element = new ExtendedTetrahedral(focus, peripherals, TetrahedralStereo.Clockwise);
            Assert.IsTrue(element.Contains(focus));
            Assert.IsTrue(element.Contains(peripherals[0]));
            Assert.IsTrue(element.Contains(peripherals[1]));
            Assert.IsTrue(element.Contains(peripherals[2]));
            Assert.IsTrue(element.Contains(peripherals[3]));

            Assert.IsFalse(element.Contains(new Mock<IAtom>().Object));
        }

        // trival access
        [TestMethod()]
        public void NoOperation() { }
    }
}
