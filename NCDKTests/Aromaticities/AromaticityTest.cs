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
using NCDK.Graphs;
using NCDK.Smiles;
using NCDK.Tools.Diff;
using NCDK.Tools.Manipulator;
using System.Linq;

namespace NCDK.Aromaticities
{
    // @author John May
    // @cdk.module test-standard
    [TestClass()]
    public class AromaticityTest
    {
        private readonly Aromaticity cdk = new Aromaticity(ElectronDonation.CDKModel, Cycles.AllSimpleFinder);
        private readonly Aromaticity cdkExo = new Aromaticity(ElectronDonation.CDKAllowingExocyclicModel, Cycles.AllSimpleFinder);
        private readonly Aromaticity daylight = new Aromaticity(ElectronDonation.DaylightModel, Cycles.AllSimpleFinder);

        [TestMethod()]
        public void Benzene()
        {
            Assert.AreEqual(6, cdk.FindBonds(Percieve(CreateFromSmiles("C1=CC=CC=C1"))).Count());
            Assert.AreEqual(6, daylight.FindBonds(CreateFromSmiles("C1=CC=CC=C1")).Count());
        }

        [TestMethod()]
        public void Furan()
        {
            Assert.AreEqual(5, cdk.FindBonds(Percieve(CreateFromSmiles("C1=CC=CO1"))).Count());
            Assert.AreEqual(5, daylight.FindBonds(CreateFromSmiles("C1=CC=CO1")).Count());
        }

        [TestMethod()]
        public void Quinone()
        {
            Assert.AreEqual(0, cdk.FindBonds(Percieve(CreateFromSmiles("O=C1C=CC(=O)C=C1"))).Count());
            Assert.AreEqual(6, cdkExo.FindBonds(Percieve(CreateFromSmiles("O=C1C=CC(=O)C=C1"))).Count());
            Assert.AreEqual(0, daylight.FindBonds(CreateFromSmiles("O=C1C=CC(=O)C=C1")).Count());
        }

        [TestMethod()]
        public void Azulene()
        {
            Assert.AreEqual(10, cdk.FindBonds(Percieve(CreateFromSmiles("C1=CC2=CC=CC=CC2=C1"))).Count());
            Assert.AreEqual(10, daylight.FindBonds(CreateFromSmiles("C1=CC2=CC=CC=CC2=C1")).Count());
        }

        // 4-oxo-1H-pyridin-1-ide
        [TestMethod()]
        public void Oxypyridinide()
        {
            Assert.AreEqual(0, cdk.FindBonds(Percieve(CreateFromSmiles("O=C1C=C[N-]C=C1"))).Count());
            Assert.AreEqual(0, cdkExo.FindBonds(Percieve(CreateFromSmiles("O=C1C=C[N-]C=C1"))).Count());
            Assert.AreEqual(6, daylight.FindBonds(CreateFromSmiles("O=C1C=C[N-]C=C1")).Count());
        }

        // 2-Pyridone
        [TestMethod()]
        public void Pyridinone()
        {
            Assert.AreEqual(0, cdk.FindBonds(Percieve(CreateFromSmiles("O=C1NC=CC=C1"))).Count());
            Assert.AreEqual(0, cdkExo.FindBonds(Percieve(CreateFromSmiles("O=C1C=C[N-]C=C1"))).Count());
            Assert.AreEqual(6, daylight.FindBonds(CreateFromSmiles("O=C1NC=CC=C1")).Count());
        }

        [TestMethod()]
        public void Subset()
        {
            Assert.AreEqual(5, daylight.FindBonds(CreateFromSmiles("[O-][Cu++]123([O-])CN4C=NC5=C4C(N=CN5)=[O+]1.O=S(=O)([OH+]2)[OH+]3")).Count());
        }

        [TestMethod()]
        public void ClearFlags_cyclobutadiene()
        {
            IAtomContainer cyclobutadiene = CreateFromSmiles("c1ccc1");
            daylight.Apply(cyclobutadiene);
            foreach (var bond in cyclobutadiene.Bonds)
                Assert.IsFalse(bond.IsAromatic);
            foreach (var atom in cyclobutadiene.Atoms)
                Assert.IsFalse(atom.IsAromatic);
        }

        [TestMethod()]
        public void ClearFlags_quinone()
        {
            IAtomContainer quinone = CreateFromSmiles("O=c1ccc(=O)cc1");
            daylight.Apply(quinone);
            foreach (var bond in quinone.Bonds)
                Assert.IsFalse(bond.IsAromatic);
            foreach (var atom in quinone.Atoms)
                Assert.IsFalse(atom.IsAromatic);
        }

        [TestMethod()]
        public void ValidSum()
        {
            // aromatic
            Assert.IsTrue(Aromaticity.ValidSum(2));
            Assert.IsTrue(Aromaticity.ValidSum(6));
            Assert.IsTrue(Aromaticity.ValidSum(10));
            Assert.IsTrue(Aromaticity.ValidSum(14));
            Assert.IsTrue(Aromaticity.ValidSum(18));

            // anti-aromatic
            Assert.IsFalse(Aromaticity.ValidSum(4));
            Assert.IsFalse(Aromaticity.ValidSum(8));
            Assert.IsFalse(Aromaticity.ValidSum(12));
            Assert.IsFalse(Aromaticity.ValidSum(16));
            Assert.IsFalse(Aromaticity.ValidSum(20));

            // other numbers
            Assert.IsFalse(Aromaticity.ValidSum(0));
            Assert.IsFalse(Aromaticity.ValidSum(1));
            Assert.IsFalse(Aromaticity.ValidSum(3));
            Assert.IsFalse(Aromaticity.ValidSum(5));
            Assert.IsFalse(Aromaticity.ValidSum(7));
            Assert.IsFalse(Aromaticity.ValidSum(9));
            Assert.IsFalse(Aromaticity.ValidSum(11));
            Assert.IsFalse(Aromaticity.ValidSum(13));
            Assert.IsFalse(Aromaticity.ValidSum(15));
        }

        [TestMethod()]
        public void ElectronSum()
        {
            Assert.AreEqual(4, Aromaticity.ElectronSum(new int[] { 0, 1, 2, 3, 0 }, new int[] { 1, 1, 1, 1 }, new int[] { 0, 1, 2, 3 }));
        }

        // @cdk.bug 736
        [TestMethod()]
        public void EnsureConsistentRepresentation()
        {
            IAtomContainer a = CreateFromSmiles("C1=CC2=CC3=CC4=C(C=CC=C4)C=C3C=C2C=C1");
            IAtomContainer b = CreateFromSmiles("c1cc2cc3cc4c(cccc4)cc3cc2cc1");
            Aromaticity arom = new Aromaticity(ElectronDonation.DaylightModel, Cycles.AllSimpleFinder);
            arom.Apply(a);
            arom.Apply(b);
            Assert.IsTrue(AtomContainerDiff.Diff(a, b).Count() == 0);
        }

        static IAtomContainer CreateFromSmiles(string smi)
        {
            return new SmilesParser(Silent.ChemObjectBuilder.Instance).ParseSmiles(smi);
        }

        static IAtomContainer Percieve(IAtomContainer molecule)
        {
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            return molecule;
        }
    }
}
