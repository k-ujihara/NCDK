/* Copyright (C) 2007  Andreas Schueller <archvile18@users.sf.net>
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
using NCDK.Default;
using NCDK.Tools.Manipulator;
using System.Collections.Generic;

namespace NCDK.Tools.Manipulator
{
    /// <summary>
    // @cdk.module test-standard
    /// </summary>
    [TestClass()]
    public class AtomContainerComparatorTest : CDKTestCase
    {

        public AtomContainerComparatorTest()
            : base()
        { }

        [TestMethod()]
        public void TestCompare_Null_IAtomContainer()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IRing cycloPentane = builder.CreateRing(5, "C");

            // Instantiate the comparator
            IComparer<IAtomContainer> comparator = new AtomContainerComparator<IAtomContainer>();

            // Assert.assert correct comparison
            Assert.AreEqual(1, comparator.Compare(null, cycloPentane), "null <-> cycloPentane");
        }

        [TestMethod()]
        public void TestCompare_Null_Null()
        {
            // Instantiate the comparator
            IComparer<IAtomContainer> comparator = new AtomContainerComparator<IAtomContainer>();

            // Assert.assert correct comparison
            Assert.AreEqual(0, comparator.Compare(null, null), "null <-> null");
        }

        [TestMethod()]
        public void TestCompare_Atom_PseudoAtom()
        {
            // Instantiate the comparator
            IComparer<IAtomContainer> comparator = new AtomContainerComparator<IAtomContainer>();

            IAtomContainer atomContainer1 = new AtomContainer();

            atomContainer1.Atoms.Add(new Atom("C"));

            IAtomContainer atomContainer2 = new AtomContainer();

            atomContainer2.Atoms.Add(new PseudoAtom("*"));

            Assert.AreEqual(1,
                    comparator.Compare(atomContainer1, atomContainer2),
                    atomContainer1 + " <-> " + atomContainer2);
        }

        [TestMethod()]
        public void TestCompare_IAtomContainer_Null()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IRing cycloPentane = builder.CreateRing(5, "C");

            // Instantiate the comparator
            IComparer<IAtomContainer> comparator = new AtomContainerComparator<IAtomContainer>();

            // Assert.assert correct comparison
            Assert.AreEqual(-1, comparator.Compare(cycloPentane, null), "cycloPentane <-> null");
        }

        [TestMethod()]
        public void TestCompare_RingSize()
        {
            // Create some IAtomContainers
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IRing cycloPentane = builder.CreateRing(5, "C");
            IRing cycloHexane = builder.CreateRing(6, "C");

            // Instantiate the comparator
            IComparer<IAtomContainer> comparator = new AtomContainerComparator<IAtomContainer>();

            Assert.AreEqual(-1, comparator.Compare(cycloPentane, cycloHexane), "cycloPentane <-> cycloHexane");
            Assert.AreEqual(0, comparator.Compare(cycloPentane, cycloPentane), "cycloPentane <-> cycloPentane");
            Assert.AreEqual(1, comparator.Compare(cycloHexane, cycloPentane), "cycloHexane <-> cycloPentane");
        }

        [TestMethod()]
        public void TestCompare_Ring_NonRing()
        {
            // Create some IAtomContainers
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IRing cycloHexane = builder.CreateRing(6, "C");
            IAtomContainer hexaneNitrogen = builder.CreateRing(6, "N");
            hexaneNitrogen.Bonds.RemoveAt(0);

            // Instantiate the comparator
            IComparer<IAtomContainer> comparator = new AtomContainerComparator<IAtomContainer>();

            Assert.AreEqual(-1, comparator.Compare(cycloHexane, hexaneNitrogen), "cycloHexane <-> hexaneNitrogen");
            Assert.AreEqual(0, comparator.Compare(cycloHexane, cycloHexane), "cycloHexane <-> cycloHexane");
            Assert.AreEqual(1, comparator.Compare(hexaneNitrogen, cycloHexane), "hexaneNitrogen <-> cycloHexane");
        }

        [TestMethod()]
        public void TestCompare_Ring_NonRing2()
        {
            // Create some IAtomContainers
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer hexaneNitrogen = builder.CreateRing(6, "N");
            hexaneNitrogen.Bonds.RemoveAt(0);
            IRing cycloHexaneNitrogen = builder.CreateRing(6, "N");

            // Instantiate the comparator
            IComparer<IAtomContainer> comparator = new AtomContainerComparator<IAtomContainer>();

            Assert.AreEqual(-1, comparator.Compare(hexaneNitrogen, cycloHexaneNitrogen), "hexaneNitrogen <-> cycloHexaneNitrogen");
            Assert.AreEqual(0, comparator.Compare(hexaneNitrogen, hexaneNitrogen), "hexaneNitrogen <-> hexaneNitrogen");
            Assert.AreEqual(1, comparator.Compare(cycloHexaneNitrogen, hexaneNitrogen), "cycloHexaneNitrogen <-> hexaneNitrogen");
        }

        [TestMethod()]
        public void TestCompare_BondOrder()
        {
            // Create some IAtomContainers
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IRing cycloHexaneNitrogen = builder.CreateRing(6, "N");
            IRing cycloHexeneNitrogen = builder.CreateRing(6, "N");
            cycloHexeneNitrogen.Bonds[0].Order = BondOrder.Double;

            // Instantiate the comparator
            IComparer<IAtomContainer> comparator = new AtomContainerComparator<IAtomContainer>();

            Assert.AreEqual(-1,
                    comparator.Compare(cycloHexaneNitrogen, cycloHexeneNitrogen), "cycloHexaneNitrogen <-> cycloHexeneNitrogen");
            Assert.AreEqual(0,
                    comparator.Compare(cycloHexaneNitrogen, cycloHexaneNitrogen), "cycloHexaneNitrogen <-> cycloHexaneNitrogen");
            Assert.AreEqual(0,
                    comparator.Compare(cycloHexeneNitrogen, cycloHexeneNitrogen), "cycloHexeneNitrogen <-> cycloHexeneNitrogen");
            Assert.AreEqual(1,
                    comparator.Compare(cycloHexeneNitrogen, cycloHexaneNitrogen), "cycloHexeneNitrogen <-> cycloHexaneNitrogen");
        }
    }
}
