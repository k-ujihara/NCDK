/*
 * Copyright (C) 2010 Rajarshi Guha <rajarshi.guha@gmail.com>
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
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Fragments
{
    /// <summary>
    /// Test fragment utils
    /// </summary>
    // @cdk.module test-fragment
    [TestClass()]
    public class FragmentUtilsTest : CDKTestCase
    {
        static SmilesParser smilesParser;

        static FragmentUtilsTest()
        {
            smilesParser = CDK.SmilesParser;
        }

        [TestMethod()]
        public void TestSplit()
        {
            var mol = smilesParser.ParseSmiles("C1CC1C2CCC2");
            SpanningTree st = new SpanningTree(mol);
            IRingSet rings = st.GetAllRings();
            IBond splitBond = null;
            for (int i = 0; i < mol.Bonds.Count; i++)
            {
                if (rings.GetRings(mol.Bonds[i]).Count() == 0)
                {
                    splitBond = mol.Bonds[i];
                    break;
                }
            }
            var frags = FragmentUtils.SplitMolecule(mol, splitBond);
            SmilesGenerator sg = new SmilesGenerator();
            var uniqueFrags = new HashSet<string>();
            foreach (var frag in frags)
            {
                uniqueFrags.Add(sg.Create(frag));
            }
            Assert.AreEqual(2, uniqueFrags.Count);
            // You can put the fragments back together with a ring closure and dot
            // [CH]12CC1.[CH]12CCC1
            Assert.IsTrue(uniqueFrags.IsSupersetOf(new[] { "[CH]1CC1", "[CH]1CCC1" }));
        }

        [TestMethod()]
        public void TestMakeAtomContainer()
        {
            var builder = CDK.Builder;

            IAtom atom = builder.NewAtom("C");
            IAtom exclude = builder.NewAtom("C");

            IAtom a1 = builder.NewAtom("C");
            IAtom a2 = builder.NewAtom("C");

            IBond[] bonds = new IBond[]{builder.NewBond(atom, exclude),
                builder.NewBond(a1, a2), builder.NewBond(a1, atom),
                builder.NewBond(a2, exclude)};

            IAtomContainer part = FragmentUtils.MakeAtomContainer(atom, bonds, exclude);

            Assert.AreEqual(3, part.Atoms.Count);
            Assert.AreEqual(2, part.Bonds.Count);

            Assert.IsTrue(part.Contains(atom));
            Assert.IsTrue(part.Contains(a1));
            Assert.IsTrue(part.Contains(a2));
            Assert.IsFalse(part.Contains(exclude));

            Assert.IsTrue(part.Contains(bonds[1]));
            Assert.IsTrue(part.Contains(bonds[2]));
        }

        [TestMethod()]
        public void TestTraversal_Chain()
        {
            var builder = CDK.Builder;

            IAtom[] atoms = new IAtom[]{builder.NewAtom("C"), builder.NewAtom("C"),
                builder.NewAtom("C"), builder.NewAtom("C"),
                builder.NewAtom("C"), builder.NewAtom("C")};
            IBond[] bonds = new IBond[]{builder.NewBond(atoms[0], atoms[1]),
                builder.NewBond(atoms[1], atoms[2]),
                builder.NewBond(atoms[2], atoms[3]),
                builder.NewBond(atoms[3], atoms[4]),
                builder.NewBond(atoms[4], atoms[5])};

            IAtomContainer m = builder.NewAtomContainer();
            m.SetAtoms(atoms);
            m.SetBonds(bonds);

            List<IBond> accumulator = new List<IBond>();

            // traverse from one end
            FragmentUtils.Traverse(m, atoms[0], accumulator);

            Assert.AreEqual(5, accumulator.Count);
            Assert.AreEqual(bonds[0], accumulator[0]);
            Assert.AreEqual(bonds[1], accumulator[1]);
            Assert.AreEqual(bonds[2], accumulator[2]);
            Assert.AreEqual(bonds[3], accumulator[3]);
            Assert.AreEqual(bonds[4], accumulator[4]);

            // traverse from the middle
            accumulator.Clear();
            FragmentUtils.Traverse(m, atoms[3], accumulator);

            Assert.AreEqual(5, accumulator.Count);

            Assert.AreEqual(bonds[2], accumulator[0]);
            Assert.AreEqual(bonds[1], accumulator[1]);
            Assert.AreEqual(bonds[0], accumulator[2]);
            Assert.AreEqual(bonds[3], accumulator[3]);
            Assert.AreEqual(bonds[4], accumulator[4]);
        }
    }
}
