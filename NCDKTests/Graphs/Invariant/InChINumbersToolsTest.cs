/* Copyright (C) 2011  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Common.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.Smiles;
using NCDK.NInChI;
using Moq;

namespace NCDK.Graphs.Invariant
{
    // @cdk.module test-inchi
    [TestClass()]
    public class InChINumbersToolsTest : CDKTestCase
    {
        [TestMethod()]
        public void TestSimpleNumbering()
        {
            IAtomContainer container = new AtomContainer();
            container.Atoms.Add(new Atom("O"));
            container.Atoms.Add(new Atom("C"));
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Single);
            long[] numbers = InChINumbersTools.GetNumbers(container);
            Assert.AreEqual(2, numbers.Length);
            Assert.AreEqual(2, numbers[0]);
            Assert.AreEqual(1, numbers[1]);
        }

        [TestMethod()]
        public void TestHydrogens()
        {
            IAtomContainer container = new AtomContainer();
            container.Atoms.Add(new Atom("H"));
            container.Atoms.Add(new Atom("C"));
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Single);
            container.Atoms.Add(new Atom("H"));
            container.AddBond(container.Atoms[1], container.Atoms[2], BondOrder.Single);
            container.Atoms.Add(new Atom("H"));
            container.AddBond(container.Atoms[1], container.Atoms[3], BondOrder.Single);
            container.Atoms.Add(new Atom("H"));
            container.AddBond(container.Atoms[1], container.Atoms[4], BondOrder.Single);
            long[] numbers = InChINumbersTools.GetNumbers(container);
            Assert.AreEqual(5, numbers.Length);
            Assert.AreEqual(0, numbers[0]);
            Assert.AreEqual(1, numbers[1]);
            Assert.AreEqual(0, numbers[2]);
            Assert.AreEqual(0, numbers[3]);
            Assert.AreEqual(0, numbers[4]);
        }

        [TestMethod()]
        public void TestGlycine()
        {
            SmilesParser parser = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer atomContainer = parser.ParseSmiles("C(C(=O)O)N");
            long[] numbers = InChINumbersTools.GetNumbers(atomContainer);
            Assert.AreEqual(5, numbers.Length);
            Assert.AreEqual(1, numbers[0]);
            Assert.AreEqual(2, numbers[1]);
            Assert.AreEqual(4, numbers[2]);
            Assert.AreEqual(5, numbers[3]);
            Assert.AreEqual(3, numbers[4]);
        }

        [TestMethod()]
        public void TestGlycine_uSmiles()
        {
            SmilesParser parser = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer atomContainer = parser.ParseSmiles("C(C(=O)O)N");
            long[] numbers = InChINumbersTools.GetNumbers(atomContainer);
            Assert.AreEqual(5, numbers.Length);
            Assert.AreEqual(1, numbers[0]);
            Assert.AreEqual(2, numbers[1]);
            Assert.AreEqual(4, numbers[2]);
            Assert.AreEqual(5, numbers[3]);
            Assert.AreEqual(3, numbers[4]);
        }

        [TestMethod()]
        public void FixedH()
        {
            SmilesParser parser = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer atomContainer = parser.ParseSmiles("N1C=NC2=CC=CC=C12");
            string auxInfo = InChINumbersTools.AuxInfo(atomContainer, INCHI_OPTION.FixedH);
            string expected = "AuxInfo=1/1/" + "N:6,7,5,8,2,4,9,3,1/" + "E:(1,2)(3,4)(6,7)(8,9)/" + "F:7,6,8,5,2,9,4,1,3/"
                    + "rA:9NCNCCCCCC/" + "rB:s1;d2;s3;d4;s5;d6;s7;s1s4d8;/" + "rC:;;;;;;;;;";
            Assert.AreEqual(expected, auxInfo);
        }

        [TestMethod()]
        public void ParseStandard()
        {
            Assert.IsTrue(Compares.AreEqual(
                new long[] { 3, 2, 1 },
                InChINumbersTools.ParseUSmilesNumbers("AuxInfo=1/0/N:3,2,1/rA:3OCC/rB:s1;s2;/rC:;;;", Mock(3))));
        }

        [TestMethod()]
        public void ParseRecMet()
        {

            // C(=O)O[Pt](N)(N)Cl
            Assert.IsTrue(Compares.AreEqual(
                new long[] { 7, 6, 1, 5, 3, 4, 2 },
                InChINumbersTools.ParseUSmilesNumbers(
                    "AuxInfo=1/1/N:3,2,4;7;5;6;1/E:(2,3);;;;/F:5m/E:m;;;;/CRV:;;2*1-1;/rA:7PtOCONNCl/rB:s1;s2;d3;s1;s1;s1;/rC:;;;;;;;/R:/0/N:3,7,5,6,4,2,1/E:(3,4)",
                    Mock(7))));
        }

        [TestMethod()]
        public void ParseFixedH()
        {
            // N1C=NC=C1
            Assert.IsTrue(Compares.AreEqual(
                new long[] { 4, 3, 5, 2, 1 },

                InChINumbersTools.ParseUSmilesNumbers(
                    "AuxInfo=1/1/N:4,5,2,3,1/E:(1,2)(4,5)/F:5,4,2,1,3/rA:5NCNCC/rB:s1;d2;s3;s1d4;/rC:;;;;;",
                    Mock(5))));
        }

        [TestMethod()]
        public void ParseDisconnected()
        {
            // O.N1C=NC=C1
            Assert.IsTrue(Compares.AreEqual(
                new long[] { 6, 4, 3, 5, 2, 1 },
                InChINumbersTools.ParseUSmilesNumbers(
                    "AuxInfo=1/1/N:5,6,3,4,2;1/E:(1,2)(4,5);/F:6,5,3,2,4;m/rA:6ONCNCC/rB:;s2;d3;s4;s2d5;/rC:;;;;;;",
                    Mock(6))));
        }

        [TestMethod()]
        public void ParseMultipleDisconnected()
        {
            // O.N1C=NC=C1.O.O=O
            Assert.IsTrue(Compares.AreEqual(
                new long[] { 8, 4, 3, 5, 2, 1, 9, 6, 7 },
                InChINumbersTools.ParseUSmilesNumbers(
                        "AuxInfo=1/1/N:5,6,3,4,2;8,9;1;7/E:(1,2)(4,5);(1,2);;/F:6,5,3,2,4;3m/E:;m;;/rA:9ONCNCCOOO/rB:;s2;d3;s4;s2d5;;;d8;/rC:;;;;;;;;;",
                        Mock(9))));
        }

        // if '[O-]' is first start at '=O' instead
        [TestMethod()]
        public void FavorCarbonyl()
        {
            IAtomContainer container = new SmilesParser(Silent.ChemObjectBuilder.Instance).ParseSmiles("P([O-])=O");
            Assert.IsTrue(Compares.AreEqual(new long[] { 3, 2, 1 }, InChINumbersTools.GetUSmilesNumbers(container)));
        }

        [TestMethod()]
        public void UnlabelledHydrogens()
        {
            IAtomContainer container = new SmilesParser(Silent.ChemObjectBuilder.Instance)
                    .ParseSmiles("[H]C([H])([H])[H]");
            Assert.IsTrue(Compares.AreEqual(new long[] { 2, 1, 3, 4, 5 }, InChINumbersTools.GetUSmilesNumbers(container)));
        }

        [TestMethod()]
        public void Bug1370()
        {
            IAtomContainer container = new SmilesParser(Silent.ChemObjectBuilder.Instance)
                    .ParseSmiles("O=[Bi]Cl");
            Assert.IsTrue(Compares.AreEqual(new long[] { 3, 1, 2 }, InChINumbersTools.GetUSmilesNumbers(container)));
        }

        static IAtomContainer Mock(int nAtoms)
        {
            var mock_container = new Mock<IAtomContainer>();
            mock_container.Setup(n => n.Atoms.Count).Returns(nAtoms);
            for (int i = 0; i < nAtoms; i++)
            {
                var mock_atom = new Mock<IAtom>();
                mock_atom.SetupGet(n => n.Symbol).Returns("C");
                mock_atom.SetupGet(n => n.AtomicNumber).Returns(6);
                mock_container.SetupGet(n => n.Atoms[i]).Returns(mock_atom.Object);
            }
            return mock_container.Object;
        }
    }
}

