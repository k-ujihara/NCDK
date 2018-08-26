/*
 * Copyright (c) 2013 European Bioinformatics Institute (EMBL-EBI)
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
using NCDK.Silent;

namespace NCDK.IO
{
    // @author John May
    // @cdk.module test-io
    [TestClass()]
    public class MDLValenceTest
    {
        [TestMethod()]
        public void Sodium_metal()
        {
            IAtomContainer container = new AtomContainer();
            IAtom atom = new Atom("Na");
            atom.Valency = 0;
            container.Atoms.Add(atom);
            MDLValence.Apply(container);
            Assert.AreEqual(0, atom.Valency);
            Assert.AreEqual(0, atom.ImplicitHydrogenCount);
        }

        [TestMethod()]
        public void Sodium_hydride()
        {
            IAtomContainer container = new AtomContainer();
            IAtom atom = new Atom("Na");
            atom.Valency = 1;
            container.Atoms.Add(atom);
            MDLValence.Apply(container);
            Assert.AreEqual(1, atom.Valency);
            Assert.AreEqual(1, atom.ImplicitHydrogenCount);
        }

        [TestMethod()]
        public void Sodium_implicit()
        {
            IAtomContainer container = new AtomContainer();
            IAtom atom = new Atom("Na");
            container.Atoms.Add(atom);
            MDLValence.Apply(container);
            Assert.AreEqual(1, atom.Valency);
            Assert.AreEqual(1, atom.ImplicitHydrogenCount);
        }

        [TestMethod()]
        public void Bismuth()
        {
            IAtomContainer container = new AtomContainer();
            IAtom bi1 = new Atom("Bi");
            IAtom h2 = new Atom("H");
            bi1.FormalCharge = +2;
            container.Atoms.Add(bi1);
            container.Atoms.Add(h2);
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Single);
            MDLValence.Apply(container);
            Assert.AreEqual(3, bi1.Valency);
            Assert.AreEqual(1, h2.Valency);
            Assert.AreEqual(2, bi1.ImplicitHydrogenCount);
            Assert.AreEqual(0, h2.ImplicitHydrogenCount);
        }

        [TestMethod()]
        public void Tin_ii()
        {
            IAtomContainer container = new AtomContainer();
            IAtom atom = new Atom("Sn");
            atom.Valency = 2;
            container.Atoms.Add(atom);
            MDLValence.Apply(container);
            Assert.AreEqual(2, atom.Valency);
            Assert.AreEqual(2, atom.ImplicitHydrogenCount);
        }

        [TestMethod()]
        public void Tin_iv()
        {
            IAtomContainer container = new AtomContainer();
            IAtom atom = new Atom("Sn");
            atom.Valency = 4;
            IAtom hydrogen = new Atom("H");
            container.Atoms.Add(atom);
            container.Atoms.Add(hydrogen);
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Single);
            MDLValence.Apply(container);
            Assert.AreEqual(4, atom.Valency);
            Assert.AreEqual(3, atom.ImplicitHydrogenCount); // 4 - explicit H
        }

        [TestMethod()]
        public void Carbon_neutral()
        {
            IAtomContainer container = new AtomContainer();
            IAtom atom = new Atom("C");
            container.Atoms.Add(atom);
            MDLValence.Apply(container);
            Assert.AreEqual(4, atom.Valency);
            Assert.AreEqual(4, atom.ImplicitHydrogenCount);
        }

        [TestMethod()]
        public void Carbon_cation()
        {
            IAtomContainer container = new AtomContainer();
            IAtom atom = new Atom("C");
            atom.FormalCharge = -1;
            container.Atoms.Add(atom);
            MDLValence.Apply(container);
            Assert.AreEqual(3, atom.Valency);
            Assert.AreEqual(3, atom.ImplicitHydrogenCount);
        }

        [TestMethod()]
        public void Carbon_cation_DoubleBonded()
        {
            IAtomContainer container = new AtomContainer();
            IAtom c1 = new Atom("C");
            IAtom c2 = new Atom("C");
            c1.FormalCharge = -1;
            container.Atoms.Add(c1);
            container.Atoms.Add(c2);
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Double);
            MDLValence.Apply(container);
            Assert.AreEqual(3, c1.Valency);
            Assert.AreEqual(1, c1.ImplicitHydrogenCount);
            Assert.AreEqual(4, c2.Valency);
            Assert.AreEqual(2, c2.ImplicitHydrogenCount);
        }

        [TestMethod()]
        public void Carbon_anion()
        {
            IAtomContainer container = new AtomContainer();
            IAtom atom = new Atom("C");
            atom.FormalCharge = +1;
            container.Atoms.Add(atom);
            MDLValence.Apply(container);
            Assert.AreEqual(3, atom.Valency);
            Assert.AreEqual(3, atom.ImplicitHydrogenCount);
        }

        [TestMethod()]
        public void Bismuth_isImplicit()
        {
            IAtomContainer container = new AtomContainer();
            IAtom bi1 = new Atom("Bi");
            IAtom h2 = new Atom("H");
            bi1.FormalCharge = +2;
            container.Atoms.Add(bi1);
            container.Atoms.Add(h2);
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Single);
            MDLValence.Apply(container);
            Assert.AreEqual(3, bi1.Valency);
            Assert.AreEqual(1, h2.Valency);
            Assert.AreEqual(2, bi1.ImplicitHydrogenCount);
            Assert.AreEqual(0, h2.ImplicitHydrogenCount);
        }

        [TestMethod()]
        public void Nitrogen_neutral()
        {
            Assert.AreEqual(3, MDLValence.ImplicitValence(7, 0, 0));
            Assert.AreEqual(3, MDLValence.ImplicitValence(7, 0, 1));
            Assert.AreEqual(3, MDLValence.ImplicitValence(7, 0, 2));
            Assert.AreEqual(3, MDLValence.ImplicitValence(7, 0, 3));
            Assert.AreEqual(5, MDLValence.ImplicitValence(7, 0, 4));
            Assert.AreEqual(5, MDLValence.ImplicitValence(7, 0, 5));
            Assert.AreEqual(6, MDLValence.ImplicitValence(7, 0, 6));
        }

        [TestMethod()]
        public void Nitrogen_cation()
        {
            Assert.AreEqual(4, MDLValence.ImplicitValence(7, +1, 0));
            Assert.AreEqual(4, MDLValence.ImplicitValence(7, +1, 1));
            Assert.AreEqual(4, MDLValence.ImplicitValence(7, +1, 2));
            Assert.AreEqual(4, MDLValence.ImplicitValence(7, +1, 3));
            Assert.AreEqual(4, MDLValence.ImplicitValence(7, +1, 4));
            Assert.AreEqual(5, MDLValence.ImplicitValence(7, +1, 5));
            Assert.AreEqual(6, MDLValence.ImplicitValence(7, +1, 6));
        }

        [TestMethod()]
        public void Nitrogen_anion()
        {
            Assert.AreEqual(2, MDLValence.ImplicitValence(7, -1, 0));
            Assert.AreEqual(2, MDLValence.ImplicitValence(7, -1, 1));
            Assert.AreEqual(2, MDLValence.ImplicitValence(7, -1, 2));
            Assert.AreEqual(3, MDLValence.ImplicitValence(7, -1, 3));
            Assert.AreEqual(4, MDLValence.ImplicitValence(7, -1, 4));
            Assert.AreEqual(5, MDLValence.ImplicitValence(7, -1, 5));
            Assert.AreEqual(6, MDLValence.ImplicitValence(7, -1, 6));
        }
    }
}
