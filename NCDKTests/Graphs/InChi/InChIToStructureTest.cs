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
using NCDK.Stereo;
using NCDK.NInChI;
using System.Collections.Generic;

namespace NCDK.Graphs.InChi
{
    /// <summary>
    /// TestCase for the {@link InChIToStructure} class.
    ///
    // @cdk.module test-inchi
    /// </summary>
    [TestClass()]
    public class InChIToStructureTest : CDKTestCase
    {

        [TestMethod()]
        public void TestConstructor_String_IChemObjectBuilder()
        {
            InChIToStructure parser = new InChIToStructure("InChI=1S/CH4/h1H4", Default.ChemObjectBuilder.Instance);
            Assert.IsNotNull(parser);
        }

        [TestMethod()]
        public void TestGetAtomContainer()
        {
            InChIToStructure parser = new InChIToStructure("InChI=1S/CH4/h1H4", Default.ChemObjectBuilder.Instance);
            IAtomContainer container = parser.AtomContainer;
            Assert.IsNotNull(container);
            Assert.AreEqual(1, container.Atoms.Count);
        }

        /// <summary>@cdk.bug 1293</summary>
        [TestMethod()]
        public void NonNullAtomicNumbers()
        {
            InChIToStructure parser = new InChIToStructure("InChI=1S/CH4/h1H4", Default.ChemObjectBuilder.Instance);
            IAtomContainer container = parser.AtomContainer;
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
            InChIToStructure parser = new InChIToStructure("InChI=1/CH2O2/c2-1-3/h1H,(H,2,3)/f/h2H",
                    Default.ChemObjectBuilder.Instance);
            IAtomContainer container = parser.AtomContainer;
            Assert.IsNotNull(container);
            Assert.AreEqual(3, container.Atoms.Count);
            Assert.AreEqual(2, container.Bonds.Count);
            Assert.IsTrue(container.Bonds[0].Order == BondOrder.Double || container.Bonds[1].Order == BondOrder.Double);
        }

        [TestMethod()]
        public void TestGetReturnStatus_EOF()
        {
            InChIToStructure parser = new InChIToStructure("InChI=1S", Default.ChemObjectBuilder.Instance);
            var container = parser.AtomContainer;
            INCHI_RET returnStatus = parser.ReturnStatus;
            Assert.IsNotNull(returnStatus);
            Assert.AreEqual(INCHI_RET.EOF, returnStatus);
        }

        [TestMethod()]
        public void TestGetMessage()
        {
            InChIToStructure parser = new InChIToStructure("InChI=1S/CH5/h1H4", Default.ChemObjectBuilder.Instance);
            var container = parser.AtomContainer;
            string message = parser.Message;
            Assert.IsNotNull(message);
        }

        [TestMethod()]
        public void TestGetMessageNull()
        {
            InChIToStructure parser = new InChIToStructure("InChI=1S", Default.ChemObjectBuilder.Instance);
            var container = parser.AtomContainer;
            string message = parser.Message;
            Assert.IsNull(message);
        }

        [TestMethod()]
        public void TestGetLog()
        {
            InChIToStructure parser = new InChIToStructure("InChI=1S/CH5/h1H4", Default.ChemObjectBuilder.Instance);
            var container = parser.AtomContainer;
            string message = parser.Message;
            Assert.IsNotNull(message);
        }

        [TestMethod()]
        public void TestGetWarningFlags()
        {
            InChIToStructure parser = new InChIToStructure("InChI=1S/CH5/h1H4", Default.ChemObjectBuilder.Instance);
            var container = parser.AtomContainer;
            ulong[,] flags = parser.WarningFlags;
            Assert.IsNotNull(flags);
            Assert.AreEqual(4, flags.Length);
        }

        [TestMethod()]
        public void TestGetAtomContainer_IChemObjectBuilder()
        {
            InChIToStructure parser = new InChIToStructure("InChI=1S/CH5/h1H4", Default.ChemObjectBuilder.Instance);
            parser.GenerateAtomContainerFromInChI(Silent.ChemObjectBuilder.Instance);
            IAtomContainer container = parser.AtomContainer;
            // test if the created IAtomContainer is done with the Silent module...
            // OK, this is not typical use, but maybe the above generate method should be private
            Assert.IsTrue(container is Silent.AtomContainer);
        }

        [TestMethod()]
        public void AtomicOxygen()
        {
            InChIToStructure parser = new InChIToStructure("InChI=1S/O", Default.ChemObjectBuilder.Instance);
            parser.GenerateAtomContainerFromInChI(Silent.ChemObjectBuilder.Instance);
            IAtomContainer container = parser.AtomContainer;
            Assert.IsInstanceOfType(container, typeof(Silent.AtomContainer));
            Assert.IsNotNull(container.Atoms[0].ImplicitHydrogenCount);
            Assert.AreEqual(0, container.Atoms[0].ImplicitHydrogenCount);
        }

        [TestMethod()]
        public void HeavyOxygenWater()
        {
            InChIToStructure parser = new InChIToStructure("InChI=1S/H2O/h1H2/i1+2", Default.ChemObjectBuilder.Instance);
            parser.GenerateAtomContainerFromInChI(Silent.ChemObjectBuilder.Instance);
            IAtomContainer container = parser.AtomContainer;
            Assert.IsInstanceOfType(container, typeof(Silent.AtomContainer));
            Assert.IsNotNull(container.Atoms[0].ImplicitHydrogenCount);
            Assert.AreEqual(2, container.Atoms[0].ImplicitHydrogenCount);
            Assert.AreEqual(18, container.Atoms[0].MassNumber);
        }

        [TestMethod()]
        public void E_bute_2_ene()
        {
            InChIToStructure parser = new InChIToStructure("InChI=1/C4H8/c1-3-4-2/h3-4H,1-2H3/b4-3+",
                    Default.ChemObjectBuilder.Instance);
            parser.GenerateAtomContainerFromInChI(Silent.ChemObjectBuilder.Instance);
            IAtomContainer container = parser.AtomContainer;
            Assert.IsInstanceOfType(container, typeof(Silent.AtomContainer));
            IEnumerator<IStereoElement> ses = container.StereoElements.GetEnumerator();
            Assert.IsTrue(ses.MoveNext());
            IStereoElement se = ses.Current;
            Assert.IsInstanceOfType(se, typeof(IDoubleBondStereochemistry));
            Assert.AreEqual(DoubleBondConformation.Opposite, ((IDoubleBondStereochemistry)se).Stereo);
        }

        [TestMethod()]
        public void Z_bute_2_ene()
        {
            InChIToStructure parser = new InChIToStructure("InChI=1/C4H8/c1-3-4-2/h3-4H,1-2H3/b4-3-",
                    Default.ChemObjectBuilder.Instance);
            parser.GenerateAtomContainerFromInChI(Silent.ChemObjectBuilder.Instance);
            IAtomContainer container = parser.AtomContainer;
            Assert.IsInstanceOfType(container, typeof(Silent.AtomContainer));
            IEnumerator<IStereoElement> ses = container.StereoElements.GetEnumerator();
            Assert.IsTrue(ses.MoveNext());
            IStereoElement se = ses.Current;
            Assert.IsInstanceOfType(se, typeof(IDoubleBondStereochemistry));
            Assert.AreEqual(DoubleBondConformation.Together, ((IDoubleBondStereochemistry)se).Stereo);
        }

        /// <summary>
        /// (R)-penta-2,3-diene
        /// </summary>
        [TestMethod()]
        public void R_penta_2_3_diene()
        {
            InChIToStructure parser = new InChIToStructure("InChI=1S/C5H8/c1-3-5-4-2/h3-4H,1-2H3/t5-/m0/s1",
                    Silent.ChemObjectBuilder.Instance);
            IAtomContainer container = parser.AtomContainer;

            Assert.IsInstanceOfType(container, typeof(Silent.AtomContainer));
            IEnumerator<IStereoElement> ses = container.StereoElements.GetEnumerator();
            Assert.IsTrue(ses.MoveNext());
            IStereoElement se = ses.Current;
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
            InChIToStructure parser = new InChIToStructure("InChI=1S/C5H8/c1-3-5-4-2/h3-4H,1-2H3/t5-/m1/s1",
                    Silent.ChemObjectBuilder.Instance);
            IAtomContainer container = parser.AtomContainer;

            Assert.IsInstanceOfType(container, typeof(Silent.AtomContainer));
            IEnumerator<IStereoElement> ses = container.StereoElements.GetEnumerator();
            Assert.IsTrue(ses.MoveNext());
            IStereoElement se = ses.Current;
            Assert.IsInstanceOfType(se, typeof(ExtendedTetrahedral));
            ExtendedTetrahedral element = (ExtendedTetrahedral)se;

            Assert.IsTrue(
                Compares.AreEqual(
                    new IAtom[] { container.Atoms[5], container.Atoms[0], container.Atoms[1], container.Atoms[6] },
                     element.Peripherals));
            Assert.AreEqual(container.Atoms[4], element.Focus);
            Assert.AreEqual(TetrahedralStereo.Clockwise, element.Winding);
        }
    }
}
