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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Common.Base;
using NCDK.Silent;
using NCDK.Stereo;
using System.Linq;

namespace NCDK.Graphs.InChI
{
    /// <summary>
    /// TestCase for the <see cref="InChIToStructure"/> class.
    /// </summary>
    // @cdk.module test-inchi
    [TestClass()]
    public class InChIToStructureTest : CDKTestCase
    {
        [TestMethod()]
        public void TestConstructor_String_IChemObjectBuilder()
        {
            var parser = new InChIToStructure("InChI=1S/CH4/h1H4", ChemObjectBuilder.Instance);
            Assert.IsNotNull(parser);
        }

        [TestMethod()]
        public void TestGetAtomContainer()
        {
            var parser = new InChIToStructure("InChI=1S/CH4/h1H4", ChemObjectBuilder.Instance);
            var container = parser.AtomContainer;
            Assert.IsNotNull(container);
            Assert.AreEqual(1, container.Atoms.Count);
        }

        /// <summary>@cdk.bug 1293</summary>
        [TestMethod()]
        public void NonNullAtomicNumbers()
        {
            var parser = new InChIToStructure("InChI=1S/CH4/h1H4", ChemObjectBuilder.Instance);
            var container = parser.AtomContainer;
            foreach (var atom in container.Atoms)
            {
                Assert.IsNotNull(atom.AtomicNumber);
            }
            Assert.IsNotNull(container);
            Assert.AreEqual(1, container.Atoms.Count);
        }

        [TestMethod()]
        public void TestFixedHydrogens()
        {
            var parser = new InChIToStructure("InChI=1/CH2O2/c2-1-3/h1H,(H,2,3)/f/h2H", ChemObjectBuilder.Instance);
            var container = parser.AtomContainer;
            Assert.IsNotNull(container);
            Assert.AreEqual(3, container.Atoms.Count);
            Assert.AreEqual(2, container.Bonds.Count);
            Assert.IsTrue(container.Bonds[0].Order == BondOrder.Double || container.Bonds[1].Order == BondOrder.Double);
        }

        [TestMethod()]
        public void TestGetReturnStatus_EOF()
        {
            var parser = new InChIToStructure("InChI=1S", ChemObjectBuilder.Instance);
            var container = parser.AtomContainer;
            var returnStatus = parser.ReturnStatus;
            Assert.IsNotNull(returnStatus);
            Assert.AreEqual(InChIReturnCode.EOF, returnStatus);
        }

        [TestMethod()]
        public void TestGetMessage()
        {
            var parser = new InChIToStructure("InChI=1S/CH5/h1H4", ChemObjectBuilder.Instance);
            var container = parser.AtomContainer;
            string message = parser.Message;
            Assert.IsNotNull(message);
        }

        [TestMethod()]
        public void TestGetMessageNull()
        {
            var parser = new InChIToStructure("InChI=1S", ChemObjectBuilder.Instance);
            var container = parser.AtomContainer;
            string message = parser.Message;
            Assert.IsNull(message);
        }

        [TestMethod()]
        public void TestGetLog()
        {
            var parser = new InChIToStructure("InChI=1S/CH5/h1H4", ChemObjectBuilder.Instance);
            var container = parser.AtomContainer;
            string message = parser.Message;
            Assert.IsNotNull(message);
        }

        [TestMethod()]
        public void TestGetWarningFlags()
        {
            var parser = new InChIToStructure("InChI=1S/CH5/h1H4", ChemObjectBuilder.Instance);
            var container = parser.AtomContainer;
            var flags = parser.WarningFlags;
            Assert.IsNotNull(flags);
            Assert.AreEqual(4, flags.Count);
        }

        [TestMethod()]
        public void TestGetAtomContainer_IChemObjectBuilder()
        {
            var parser = new InChIToStructure("InChI=1S/CH5/h1H4", ChemObjectBuilder.Instance);
            var container = parser.AtomContainer;
            // test if the created IAtomContainer is done with the Silent module...
            // OK, this is not typical use, but maybe the above generate method should be private
            Assert.IsInstanceOfType(container, ChemObjectBuilder.Instance.NewAtomContainer().GetType());
        }

        [TestMethod()]
        public void AtomicOxygen()
        {
            var parser = new InChIToStructure("InChI=1S/O", ChemObjectBuilder.Instance);
            var container = parser.AtomContainer;
            Assert.IsInstanceOfType(container, ChemObjectBuilder.Instance.NewAtomContainer().GetType());
            Assert.IsNotNull(container.Atoms[0].ImplicitHydrogenCount);
            Assert.AreEqual(0, container.Atoms[0].ImplicitHydrogenCount);
        }

        [TestMethod()]
        public void HeavyOxygenWater()
        {
            var parser = new InChIToStructure("InChI=1S/H2O/h1H2/i1+2", ChemObjectBuilder.Instance);
            var container = parser.AtomContainer;
            Assert.IsNotNull(container.Atoms[0].ImplicitHydrogenCount);
            Assert.IsInstanceOfType(container, ChemObjectBuilder.Instance.NewAtomContainer().GetType());
            Assert.IsNotNull(container.Atoms[0].ImplicitHydrogenCount);
            Assert.AreEqual(2, container.Atoms[0].ImplicitHydrogenCount);
            Assert.AreEqual(18, container.Atoms[0].MassNumber);
        }

        [TestMethod()]
        public void E_bute_2_ene()
        {
            var parser = new InChIToStructure("InChI=1/C4H8/c1-3-4-2/h3-4H,1-2H3/b4-3+", ChemObjectBuilder.Instance);
            var container = parser.AtomContainer;
            var ses = container.StereoElements.GetEnumerator();
            Assert.IsInstanceOfType(container, ChemObjectBuilder.Instance.NewAtomContainer().GetType());
            Assert.IsTrue(ses.MoveNext());
            var se = ses.Current;
            Assert.IsInstanceOfType(se, typeof(IDoubleBondStereochemistry));
            Assert.AreEqual(DoubleBondConformation.Opposite, ((IDoubleBondStereochemistry)se).Stereo);
        }

        [TestMethod()]
        public void Z_bute_2_ene()
        {
            var parser = new InChIToStructure("InChI=1/C4H8/c1-3-4-2/h3-4H,1-2H3/b4-3-", ChemObjectBuilder.Instance);
            var container = parser.AtomContainer;
            var ses = container.StereoElements.GetEnumerator();
            Assert.IsInstanceOfType(container, ChemObjectBuilder.Instance.NewAtomContainer().GetType());
            Assert.IsTrue(ses.MoveNext());
            var se = ses.Current;
            Assert.IsInstanceOfType(se, typeof(IDoubleBondStereochemistry));
            Assert.AreEqual(DoubleBondConformation.Together, ((IDoubleBondStereochemistry)se).Stereo);
        }

        /// <summary>
        /// (R)-penta-2,3-diene
        /// </summary>
        [TestMethod()]
        public void R_penta_2_3_diene()
        {
            var parser = new InChIToStructure("InChI=1S/C5H8/c1-3-5-4-2/h3-4H,1-2H3/t5-/m0/s1", Silent.ChemObjectBuilder.Instance);
            var container = parser.AtomContainer;

            var ses = container.StereoElements.GetEnumerator();
            Assert.IsInstanceOfType(container, Silent.ChemObjectBuilder.Instance.NewAtomContainer().GetType());
            var f = ses.MoveNext();
            Assert.IsTrue(f);
            var se = ses.Current;
            Assert.IsInstanceOfType(se, typeof(ExtendedTetrahedral));
            ExtendedTetrahedral element = (ExtendedTetrahedral)se;
            Assert.IsTrue(
                Compares.AreEqual(
                    new IAtom[] {
                    container.Atoms[5], container.Atoms[0], container.Atoms[1], container.Atoms[6] },
                    element.Peripherals));
            Assert.AreEqual(container.Atoms[4], element.Focus);
            Assert.AreEqual(TetrahedralStereo.AntiClockwise, element.Winding);
        }

        /// <summary>
        /// (S)-penta-2,3-diene
        /// </summary>
        [TestMethod()]
        public void S_penta_2_3_diene()
        {
            var parser = new InChIToStructure("InChI=1S/C5H8/c1-3-5-4-2/h3-4H,1-2H3/t5-/m1/s1", Silent.ChemObjectBuilder.Instance);
            var container = parser.AtomContainer;

            var ses = container.StereoElements.GetEnumerator();
            Assert.IsInstanceOfType(container, Silent.ChemObjectBuilder.Instance.NewAtomContainer().GetType());
            Assert.IsTrue(ses.MoveNext());
            var se = ses.Current;
            Assert.IsInstanceOfType(se, typeof(ExtendedTetrahedral));
            ExtendedTetrahedral element = (ExtendedTetrahedral)se;

            Assert.IsTrue(
                Compares.AreEqual(
                    new IAtom[] { container.Atoms[5], container.Atoms[0], container.Atoms[1], container.Atoms[6] },
                     element.Peripherals));
            Assert.AreEqual(container.Atoms[4], element.Focus);
            Assert.AreEqual(TetrahedralStereo.Clockwise, element.Winding);
        }

        [TestMethod()]
        public void Diazene()
        {
            InChIToStructure parse = new InChIToStructure("InChI=1S/H2N2/c1-2/h1-2H/b2-1+",
                                                          Silent.ChemObjectBuilder.Instance);
            IAtomContainer mol = parse.AtomContainer;
            Assert.AreEqual(4, mol.Atoms.Count);
            Assert.AreEqual(true, mol.StereoElements.Any());
        }

        [TestMethod()]
        public void ReadImplicitDeuteriums()
        {
            var inchi = "InChI=1S/C22H32O2/c1-2-3-4-5-6-7-8-9-10-11-12-13-14-15-16-17-18-19-20-21-22(23)24/h3-4,6-7,9-10,12-13,15-16,18-19H,2,5,8,11,14,17,20-21H2,1H3,(H,23,24)/b4-3-,7-6-,10-9-,13-12-,16-15-,19-18-/i1D3,2D2";
            var intostruct = InChIGeneratorFactory.Instance.GetInChIToStructure(inchi, Silent.ChemObjectBuilder.Instance);
            var mol = intostruct.AtomContainer;
            int dCount = 0;
            foreach (var atom in mol.Atoms)
            {
                var mass = atom.MassNumber;
                if (mass != null && mass.Equals(2))
                    dCount++;
            }
            Assert.AreEqual(5, dCount);
        }
    }
}
