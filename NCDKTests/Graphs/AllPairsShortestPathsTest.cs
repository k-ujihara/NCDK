/*
 * Copyright (C) 2012 John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
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
using NCDK.Common.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Silent;
using NCDK.Templates;
using System;

namespace NCDK.Graphs
{
    // @author John May
    // @cdk.module test-core
    [TestClass()]
    public class AllPairsShortestPathsTest
    {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void TestConstruction_Null()
        {
            new AllPairsShortestPaths(null);
        }

        [TestMethod()]
        public virtual void TestConstruction_Empty()
        {
            AllPairsShortestPaths asp = new AllPairsShortestPaths(new AtomContainer());

            // all vs all fro -10 -> 10
            for (int i = -10; i < 10; i++)
            {
                for (int j = -10; j < 10; j++)
                {

                    Assert.IsTrue(Compares.AreEqual(Array.Empty<int[]>(), asp.From(i).GetPathsTo(0)));
                    Assert.IsTrue(Compares.AreEqual(Array.Empty<int>(), asp.From(i).GetPathTo(0)));
                    Assert.IsTrue(Compares.AreEqual(Array.Empty<IAtom>(), asp.From(i).GetAtomsTo(0)));

                    Assert.AreEqual(0, asp.From(i).GetNPathsTo(j));
                    Assert.AreEqual(int.MaxValue, asp.From(i).GetDistanceTo(j));
                }
            }
        }

        [TestMethod()]
        public virtual void TestFrom_Atom_Benzene()
        {
            IAtomContainer benzene = TestMoleculeFactory.MakeBenzene();
            AllPairsShortestPaths asp = new AllPairsShortestPaths(benzene);

            IAtom c1 = benzene.Atoms[0];
            IAtom c2 = benzene.Atoms[1];
            IAtom c3 = benzene.Atoms[2];
            IAtom c4 = benzene.Atoms[3];
            IAtom c5 = benzene.Atoms[4];
            IAtom c6 = benzene.Atoms[5];

            //    c2 - c3
            //  /        \
            // c1         c4
            //  \        /
            //    c6 - c5

            Assert.IsNotNull(asp.From(c1));
            Assert.IsNotNull(asp.From(c2));
            Assert.IsNotNull(asp.From(c3));
            Assert.IsNotNull(asp.From(c4));
            Assert.IsNotNull(asp.From(c5));
            Assert.IsNotNull(asp.From(c6));

            {
                IAtom[] expected = new IAtom[] { c1, c2, c3 };
                IAtom[] actual = asp.From(c1).GetAtomsTo(c3);
                Assert.IsTrue(Compares.AreEqual(expected, actual));
            }

            {
                IAtom[] expected = new IAtom[] { c3, c2, c1 };
                IAtom[] actual = asp.From(c3).GetAtomsTo(c1);
                Assert.IsTrue(Compares.AreEqual(expected, actual));
            }

            {
                IAtom[] expected = new IAtom[] { c1, c6, c5 };
                IAtom[] actual = asp.From(c1).GetAtomsTo(c5);
                Assert.IsTrue(Compares.AreEqual(expected, actual));
            }

            {
                IAtom[] expected = new IAtom[] { c5, c6, c1 };
                IAtom[] actual = asp.From(c5).GetAtomsTo(c1);
                Assert.IsTrue(Compares.AreEqual(expected, actual));
            }
        }

        [TestMethod()]
        public virtual void TestFrom_Int_Benzene()
        {
            IAtomContainer benzene = TestMoleculeFactory.MakeBenzene();
            AllPairsShortestPaths asp = new AllPairsShortestPaths(benzene);

            //    1 - 2
            //  /       \
            // 0         3
            //  \       /
            //    5 - 4

            Assert.IsNotNull(asp.From(0));
            Assert.IsNotNull(asp.From(1));
            Assert.IsNotNull(asp.From(2));
            Assert.IsNotNull(asp.From(3));
            Assert.IsNotNull(asp.From(4));
            Assert.IsNotNull(asp.From(5));

            {
                int[] expected = new int[] { 0, 1, 2 };
                int[] actual = asp.From(0).GetPathTo(2);
                Assert.IsTrue(Compares.AreEqual(expected, actual));
            }

            {
                int[] expected = new int[] { 2, 1, 0 };
                int[] actual = asp.From(2).GetPathTo(0);
                Assert.IsTrue(Compares.AreEqual(expected, actual));
            }

            {
                int[] expected = new int[] { 0, 5, 4 };
                int[] actual = asp.From(0).GetPathTo(4);
                Assert.IsTrue(Compares.AreEqual(expected, actual));
            }

            {
                int[] expected = new int[] { 4, 5, 0 };
                int[] actual = asp.From(4).GetPathTo(0);
                Assert.IsTrue(Compares.AreEqual(expected, actual));
            }
        }
    }
}
