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
using System;
using System.IO;

namespace NCDK.IO
{
    // @author John May
    // @cdk.module test-io
    [TestClass()]
    public class MDLV2000AtomBlockTest
    {
        private readonly MDLV2000Reader reader = new MDLV2000Reader(new StringReader(""));
        private static readonly IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

        [TestMethod()]
        public void LineLength_excessSpace()
        {
            IAtom a1 = reader.ReadAtomFast("    7.8089   -1.3194    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0 ",
                    builder, 1);
            IAtom a2 = reader.ReadAtomFast("    7.8089   -1.3194    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0    ",
                    builder, 1);
        }

        [TestMethod()]
        public void LineLength_exact()
        {
            IAtom atom = reader.ReadAtomFast("    7.8089   -1.3194    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0",
                    builder, 1);
        }

        [TestMethod()]
        public void LineLength_truncated()
        {
            IAtom atom = reader.ReadAtomFast("    7.8089   -1.3194    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  ",
                    builder, 1);
        }

        [TestMethod()]
        public void Symbol_C()
        {
            IAtom atom = reader.ReadAtomFast("    7.8089   -1.3194    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0",
                    builder, 1);
            Assert.AreEqual("C", atom.Symbol);
        }

        [TestMethod()]
        public void Symbol_N()
        {
            IAtom atom = reader.ReadAtomFast("    7.8089   -1.3194    0.0000 N   0  0  0  0  0  0  0  0  0  0  0  0",
                    builder, 1);
            Assert.AreEqual("N", atom.Symbol);
        }

        [TestMethod()]
        public void ReadCoordinates()
        {
            IAtom atom = reader.ReadAtomFast("    7.8089   -1.3194    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0",
                    builder, 1);
            Assert.AreEqual(7.8089, atom.Point3D.Value.X, 0.5);
            Assert.AreEqual(-1.3194, atom.Point3D.Value.Y, 0.5);
            Assert.AreEqual(0, atom.Point3D.Value.Z, 0.5);
        }

        [TestMethod()]
        public void MassDiff_c13()
        {
            IAtom atom = reader.ReadAtomFast("    7.8089   -1.3194    0.0000 C   1  0  0  0  0  0  0  0  0  0  0  0",
                    builder, 1);
            Assert.AreEqual(13, atom.MassNumber);
        }

        [TestMethod()]
        public void MassDiff_c14()
        {
            IAtom atom = reader.ReadAtomFast("    7.8089   -1.3194    0.0000 C   2  0  0  0  0  0  0  0  0  0  0  0",
                    builder, 1);
            Assert.AreEqual(14, atom.MassNumber);
        }

        [TestMethod()]
        public void MassDiff_c11()
        {
            IAtom atom = reader.ReadAtomFast("    7.8089   -1.3194    0.0000 C  -1  0  0  0  0  0  0  0  0  0  0  0",
                    builder, 1);
            Assert.AreEqual(11, atom.MassNumber);
        }

        [TestMethod()]
        public void Charge_cation()
        {
            IAtom atom = reader.ReadAtomFast("    7.8089   -1.3194    0.0000 C   0  1  0  0  0  0  0  0  0  0  0  0",
                    builder, 1);
            Assert.AreEqual(3, atom.FormalCharge);
        }

        [TestMethod()]
        public void Charge_dication()
        {
            IAtom atom = reader.ReadAtomFast("    7.8089   -1.3194    0.0000 C   0  2  0  0  0  0  0  0  0  0  0  0",
                    builder, 1);
            Assert.AreEqual(2, atom.FormalCharge);
        }

        [TestMethod()]
        public void Charge_trication()
        {
            IAtom atom = reader.ReadAtomFast("    7.8089   -1.3194    0.0000 C   0  3  0  0  0  0  0  0  0  0  0  0",
                    builder, 1);
            Assert.AreEqual(1, atom.FormalCharge);
        }

        // SingleElectronContainer created by M  RAD
        [TestMethod()]
        public void Charge_Doubletradical()
        {
            IAtom atom = reader.ReadAtomFast("    7.8089   -1.3194    0.0000 C   0  4  0  0  0  0  0  0  0  0  0  0",
                    builder, 1);
            Assert.AreEqual(0, atom.FormalCharge);
        }

        [TestMethod()]
        public void Charge_anion()
        {
            IAtom atom = reader.ReadAtomFast("    7.8089   -1.3194    0.0000 C   0  5  0  0  0  0  0  0  0  0  0  0",
                    builder, 1);
            Assert.AreEqual(-1, atom.FormalCharge);
        }

        [TestMethod()]
        public void Charge_dianion()
        {
            IAtom atom = reader.ReadAtomFast("    7.8089   -1.3194    0.0000 C   0  6  0  0  0  0  0  0  0  0  0  0",
                    builder, 1);
            Assert.AreEqual(-2, atom.FormalCharge);
        }

        [TestMethod()]
        public void Charge_trianion()
        {
            IAtom atom = reader.ReadAtomFast("    7.8089   -1.3194    0.0000 C   0  7  0  0  0  0  0  0  0  0  0  0",
                    builder, 1);
            Assert.AreEqual(-3, atom.FormalCharge);
        }

        [TestMethod()]
        public void Charge_invalid()
        {
            IAtom atom = reader.ReadAtomFast("    7.8089   -1.3194    0.0000 C   0  8  0  0  0  0  0  0  0  0  0  0",
                    builder, 1);
            Assert.AreEqual(0, atom.FormalCharge);
        }

        [TestMethod()]
        public void Valence_0()
        {
            IAtom atom = reader.ReadAtomFast("    7.8089   -1.3194    0.0000 C   0  0  0  0  0 15  0  0  0  0  0  0",
                    builder, 1);
            Assert.AreEqual(0, atom.Valency);
        }

        [TestMethod()]
        public void Valence_unset()
        {
            IAtom atom = reader.ReadAtomFast("    7.8089   -1.3194    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0",
                    builder, 1);
            Assert.AreEqual(null, atom.Valency);
        }

        [TestMethod()]
        public void Valence_1()
        {
            IAtom atom = reader.ReadAtomFast("    7.8089   -1.3194    0.0000 C   0  0  0  0  0  1  0  0  0  0  0  0",
                    builder, 1);
            Assert.AreEqual(1, atom.Valency);
        }

        [TestMethod()]
        public void Valence_14()
        {
            IAtom atom = reader.ReadAtomFast("    7.8089   -1.3194    0.0000 C   0  0  0  0  0 14  0  0  0  0  0  0",
                    builder, 1);
            Assert.AreEqual(14, atom.Valency);
        }

        [TestMethod()]
        public void Valence_invalid()
        {
            IAtom atom = reader.ReadAtomFast("    7.8089   -1.3194    0.0000 C   0  0  0  0  0 16  0  0  0  0  0  0",
                    builder, 1);
            Assert.AreEqual(null, atom.Valency);
        }

        [TestMethod()]
        public void Mapping()
        {
            IAtom atom = reader.ReadAtomFast("    7.8089   -1.3194    0.0000 C   0  0  0  0  0  0  0  0  0  1  0  0",
                    builder, 1);
            Assert.AreEqual(1, atom.GetProperty<int>(CDKPropertyName.AtomAtomMapping));
        }

        [TestMethod()]
        public void Mapping_42()
        {
            IAtom atom = reader.ReadAtomFast("    7.8089   -1.3194    0.0000 C   0  0  0  0  0  0  0  0  0 42  0  0",
                    builder, 1);
            Assert.AreEqual(42, atom.GetProperty<int>(CDKPropertyName.AtomAtomMapping));
        }

        [TestMethod()]
        public void Mapping_999()
        {
            IAtom atom = reader.ReadAtomFast("    7.8089   -1.3194    0.0000 C   0  0  0  0  0  0  0  0  0999  0  0",
                    builder, 1);
            Assert.AreEqual(999, atom.GetProperty<int>(CDKPropertyName.AtomAtomMapping));
        }

        [TestMethod()]
        public void LonePairAtomSymbol()
        {
            Assert.IsTrue(MDLV2000Reader.IsPseudoElement("LP"));
        }

        [TestMethod()]
        public void AtomListAtomSymbol()
        {
            Assert.IsTrue(MDLV2000Reader.IsPseudoElement("L"));
        }

        [TestMethod()]
        public void HeavyAtomSymbol()
        {
            Assert.IsTrue(MDLV2000Reader.IsPseudoElement("A"));
        }

        [TestMethod()]
        public void HeteroAtomSymbol()
        {
            Assert.IsTrue(MDLV2000Reader.IsPseudoElement("Q"));
        }

        [TestMethod()]
        public void UnspecifiedAtomSymbol()
        {
            Assert.IsTrue(MDLV2000Reader.IsPseudoElement("*"));
        }

        [TestMethod()]
        public void RGroupAtomSymbol()
        {
            Assert.IsTrue(MDLV2000Reader.IsPseudoElement("R"));
        }

        [TestMethod()]
        public void RGroupAtomSymbol_Hash()
        {
            Assert.IsTrue(MDLV2000Reader.IsPseudoElement("R#"));
        }

        [TestMethod()]
        public void InvalidAtomSymbol()
        {
            Assert.IsFalse(MDLV2000Reader.IsPseudoElement("RNA"));
            Assert.IsFalse(MDLV2000Reader.IsPseudoElement("DNA"));
            Assert.IsFalse(MDLV2000Reader.IsPseudoElement("ACP"));
        }

        [TestMethod()]
        public void ReadMDLCoordinate()
        {
            Assert.AreEqual(7.8089, new MDLV2000Reader(new StringReader("")).ReadMDLCoordinate("    7.8089", 0), 0.1);
        }

        [TestMethod()]
        public void ReadMDLCoordinate_negative()
        {
            Assert.AreEqual(-2.0012, new MDLV2000Reader(new StringReader("")).ReadMDLCoordinate("   -2.0012", 0), 0.1);
        }

        [TestMethod()]
        public void ReadMDLCoordinate_offset()
        {
            Assert.AreEqual(7.8089, new MDLV2000Reader(new StringReader("")).ReadMDLCoordinate("   -2.0012    7.8089", 10), 0.1);
        }

        [TestMethod()]
        public void ReadOldJmolCoords()
        {
            MDLV2000Reader reader = new MDLV2000Reader(new StringReader("")) { ReaderMode = ChemObjectReaderMode.Relaxed };
            Assert.IsTrue(Math.Abs(reader.ReadMDLCoordinate("  -2.00120    7.8089", 0) - (-2.00120)) < 0.1);
        }

        [TestMethod()]
        [ExpectedException(typeof(CDKException), AllowDerivedTypes = true)]
        public void ReadOldJmolCoordsFailOnStrictRead()
        {
            MDLV2000Reader reader = new MDLV2000Reader(new StringReader("")) { ReaderMode = ChemObjectReaderMode.Strict };
            reader.ReadMDLCoordinate("  -2.00120    7.8089", 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(CDKException), AllowDerivedTypes = true)]
        public void ReadMDLCoordinates_wrong_decimal_position_strict()
        {
            MDLV2000Reader reader = new MDLV2000Reader(new StringReader("")) { ReaderMode = ChemObjectReaderMode.Strict };
            Assert.IsTrue(Math.Abs(reader.ReadMDLCoordinate("   -2.0012   7.8089 ", 10) - 7.8089) < 0.1);
        }

        [TestMethod()]
        public void ReadMDLCoordinates_wrong_decimal_position_relaxed()
        {
            MDLV2000Reader reader = new MDLV2000Reader(new StringReader("")) { ReaderMode = ChemObjectReaderMode.Relaxed };
            Assert.IsTrue(Math.Abs(reader.ReadMDLCoordinate("   -2.0012   7.8089 ", 10) - 7.8089) < 0.1);
        }

        [TestMethod()]
        public void ReadMDLCoordinates_no_value_relaxed()
        {
            MDLV2000Reader reader = new MDLV2000Reader(new StringReader("")) { ReaderMode = ChemObjectReaderMode.Relaxed };
            Assert.IsTrue(Math.Abs(reader.ReadMDLCoordinate("   -2.0012          ", 10) - 0) < 0.1);
        }

        [TestMethod()]
        public void ReadMDLCoordinates_no_decimal_relaxed()
        {
            MDLV2000Reader reader = new MDLV2000Reader(new StringReader("")) { ReaderMode = ChemObjectReaderMode.Relaxed };
            Assert.IsTrue(Math.Abs(reader.ReadMDLCoordinate("   -2.0012   708089 ", 10) - 708089) < 0.1);
        }
    }
}
