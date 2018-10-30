/* Copyright (C) 1997-2009,2011  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All I ask is that proper credit is given for my work, which includes
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Aromaticities;
using NCDK.Common.Collections;
using NCDK.IO;
using NCDK.Silent;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System.Collections;

namespace NCDK.Fingerprints
{
    // @cdk.module test-standard
    [TestClass()]
    public abstract class AbstractFixedLengthFingerprinterTest : AbstractFingerprinterTest
    {
        // logical 'AND' or two bit sets (orginals are not modified)
        static BitArray And(BitArray a, BitArray b)
        {
            BitArray c = (BitArray)a.Clone();
            c.And(b);
            return c;
        }

        // @cdk.bug 706786
        [TestMethod()]
        public virtual void TestBug706786()
        {
            // inlined molecules - note this test fails if implicit hydrogens are
            // included. generally MACCS and ESTATE can't be used for substructure filter
            // check those subclasses which check the bits are set
            IAtomContainer superStructure = Bug706786_1();
            IAtomContainer subStructure = Bug706786_2();

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(superStructure);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(subStructure);
            AddImplicitHydrogens(superStructure);
            AddImplicitHydrogens(subStructure);

            IFingerprinter fingerprinter = GetBitFingerprinter();
            BitArray superBS = fingerprinter.GetBitFingerprint(superStructure).AsBitSet();
            BitArray subBS = fingerprinter.GetBitFingerprint(subStructure).AsBitSet();

            Assert.IsTrue(BitArrays.Equals(subBS, And(superBS, subBS)));
        }

        // @cdk.bug 853254
        [TestMethod()]
        public void TestBug853254()
        {
            var builder = Silent.ChemObjectBuilder.Instance;
            string filename = "NCDK.Data.MDL.bug853254-2.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderMode.Strict);
            IAtomContainer superstructure = (IAtomContainer)reader.Read(builder.NewAtomContainer());

            filename = "NCDK.Data.MDL.bug853254-1.mol";
            ins = ResourceLoader.GetAsStream(filename);
            reader = new MDLV2000Reader(ins, ChemObjectReaderMode.Strict);
            IAtomContainer substructure = (IAtomContainer)reader.Read(builder.NewAtomContainer());

            // these molecules are different resonance forms of the same molecule
            // make sure aromaticity is detected. although some fingerprinters do this
            // one should not expected all implementations to do so.
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(superstructure);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(substructure);
            Aromaticity.CDKLegacy.Apply(superstructure);
            Aromaticity.CDKLegacy.Apply(substructure);

            IFingerprinter fingerprinter = GetBitFingerprinter();
            BitArray superBS = fingerprinter.GetBitFingerprint(superstructure).AsBitSet();
            BitArray subBS = fingerprinter.GetBitFingerprint(substructure).AsBitSet();
            bool isSubset = FingerprinterTool.IsSubset(superBS, subBS);
            Assert.IsTrue(isSubset);
        }

        /// <summary>
        /// Fingerprint not subset.
        /// </summary>
        // @cdk.bug 934819
        [TestMethod()]
        public virtual void TestBug934819()
        {
            // inlined molecules - note this test fails if implicit hydrogens are
            // included. generally PubCheMFingerprint can't be used for substructure filter
            IAtomContainer superStructure = Bug934819_2();
            IAtomContainer subStructure = Bug934819_1();

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(superStructure);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(subStructure);
            AddImplicitHydrogens(superStructure);
            AddImplicitHydrogens(subStructure);

            IFingerprinter fingerprinter = GetBitFingerprinter();
            BitArray superBS = fingerprinter.GetBitFingerprint(superStructure).AsBitSet();
            BitArray subBS = fingerprinter.GetBitFingerprint(subStructure).AsBitSet();

            Assert.IsTrue(BitArrays.Equals(subBS, And(superBS, subBS)));
        }

        /// <summary>
        /// Problems with different aromaticity concepts.
        /// </summary>
        // @cdk.bug 771485
        [TestMethod()]
        public void TestBug771485()
        {
            var builder = Silent.ChemObjectBuilder.Instance;
            string filename = "NCDK.Data.MDL.bug771485-1.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderMode.Strict);
            IAtomContainer structure1 = (IAtomContainer)reader.Read(builder.NewAtomContainer());

            filename = "NCDK.Data.MDL.bug771485-2.mol";
            ins = ResourceLoader.GetAsStream(filename);
            reader = new MDLV2000Reader(ins, ChemObjectReaderMode.Strict);
            IAtomContainer structure2 = (IAtomContainer)reader.Read(builder.NewAtomContainer());

            // these molecules are different resonance forms of the same molecule
            // make sure aromaticity is detected. although some fingerprinters do this
            // one should not expected all implementations to do so.
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(structure1);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(structure2);
            Aromaticity.CDKLegacy.Apply(structure1);
            Aromaticity.CDKLegacy.Apply(structure2);
            AddImplicitHydrogens(structure1);
            AddImplicitHydrogens(structure2);

            Kekulization.Kekulize(structure1);
            Kekulization.Kekulize(structure2);

            // hydrogens loaded from MDL mol files if non-query. Structure 2 has
            // query aromatic bonds and the hydrogen counts are not assigned - ensure
            // this is done here.
            CDK.HydrogenAdder.AddImplicitHydrogens(structure1);
            CDK.HydrogenAdder.AddImplicitHydrogens(structure2);

            IFingerprinter fingerprinter = GetBitFingerprinter();
            BitArray superBS = fingerprinter.GetBitFingerprint(structure2).AsBitSet();
            BitArray subBS = fingerprinter.GetBitFingerprint(structure1).AsBitSet();
            bool isSubset = FingerprinterTool.IsSubset(superBS, subBS);
            Assert.IsTrue(isSubset);
        }

        /// <summary>
        /// Fingerprinter gives different fingerprints for same molecule.
        /// </summary>
        // @cdk.bug 931608
        // @cdk.bug 934819
        [TestMethod()]
        public void TestBug931608()
        {
            var builder = Silent.ChemObjectBuilder.Instance;
            string filename = "NCDK.Data.MDL.bug931608-1.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderMode.Strict);
            IAtomContainer structure1 = (IAtomContainer)reader.Read(builder.NewAtomContainer());

            filename = "NCDK.Data.MDL.bug931608-2.mol";
            ins = ResourceLoader.GetAsStream(filename);
            reader = new MDLV2000Reader(ins, ChemObjectReaderMode.Strict);
            IAtomContainer structure2 = (IAtomContainer)reader.Read(builder.NewAtomContainer());

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(structure1);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(structure2);

            IFingerprinter fingerprinter = GetBitFingerprinter();
            BitArray bs1 = fingerprinter.GetBitFingerprint(structure1).AsBitSet();
            BitArray bs2 = fingerprinter.GetBitFingerprint(structure2).AsBitSet();
            // now we do the bool XOR on the two bitsets, leading
            // to a bitset that has all the bits set to "true" which differ
            // between the two original bitsets
            bs1.Xor(bs2);
            // cardinality gives us the number of "true" bits in the
            // result of the XOR operation.
            int cardinality = BitArrays.Cardinality(bs1);
            Assert.AreEqual(0, cardinality);
        }

        /// <summary>
        /// NCDK.Data.MDL.bug70786-1.mol
        /// CC(=O)C1=CC2=C(OC(C)(C)[C@@H](O)[C@@H]2O)C=C1
        /// </summary>
        // @cdk.inchi InChI=1/C13H16O4/c1-7(14)8-4-5-10-9(6-8)11(15)12(16)13(2,3)17-10/h4-6,11-12,15-16H,1-3H3/t11-,12+/s2
        public static IAtomContainer Bug706786_1()
        {
            IChemObjectBuilder builder = ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("C");
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("O");
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.IsAromatic = true;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            a5.IsAromatic = true;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.NewAtom("C");
            a6.IsAromatic = true;
            mol.Atoms.Add(a6);
            IAtom a7 = builder.NewAtom("C");
            a7.IsAromatic = true;
            mol.Atoms.Add(a7);
            IAtom a8 = builder.NewAtom("O");
            mol.Atoms.Add(a8);
            IAtom a9 = builder.NewAtom("C");
            mol.Atoms.Add(a9);
            IAtom a10 = builder.NewAtom("C");
            mol.Atoms.Add(a10);
            IAtom a11 = builder.NewAtom("C");
            mol.Atoms.Add(a11);
            IAtom a12 = builder.NewAtom("H");
            mol.Atoms.Add(a12);
            IAtom a13 = builder.NewAtom("C");
            mol.Atoms.Add(a13);
            IAtom a14 = builder.NewAtom("O");
            mol.Atoms.Add(a14);
            IAtom a15 = builder.NewAtom("H");
            mol.Atoms.Add(a15);
            IAtom a16 = builder.NewAtom("C");
            mol.Atoms.Add(a16);
            IAtom a17 = builder.NewAtom("O");
            mol.Atoms.Add(a17);
            IAtom a18 = builder.NewAtom("C");
            a18.IsAromatic = true;
            mol.Atoms.Add(a18);
            IAtom a19 = builder.NewAtom("C");
            a19.IsAromatic = true;
            mol.Atoms.Add(a19);
            IBond b1 = builder.NewBond(a2, a1, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a3, a2, BondOrder.Double);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a4, a2, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a5, a4, BondOrder.Double);
            b4.IsAromatic = true;
            mol.Bonds.Add(b4);
            IBond b5 = builder.NewBond(a6, a5, BondOrder.Single);
            b5.IsAromatic = true;
            mol.Bonds.Add(b5);
            IBond b6 = builder.NewBond(a7, a6, BondOrder.Double);
            b6.IsAromatic = true;
            mol.Bonds.Add(b6);
            IBond b7 = builder.NewBond(a8, a7, BondOrder.Single);
            mol.Bonds.Add(b7);
            IBond b8 = builder.NewBond(a9, a8, BondOrder.Single);
            mol.Bonds.Add(b8);
            IBond b9 = builder.NewBond(a10, a9, BondOrder.Single);
            mol.Bonds.Add(b9);
            IBond b10 = builder.NewBond(a11, a9, BondOrder.Single);
            mol.Bonds.Add(b10);
            IBond b11 = builder.NewBond(a13, a12, BondOrder.Single);
            mol.Bonds.Add(b11);
            IBond b12 = builder.NewBond(a13, a9, BondOrder.Single);
            mol.Bonds.Add(b12);
            IBond b13 = builder.NewBond(a14, a13, BondOrder.Single);
            mol.Bonds.Add(b13);
            IBond b14 = builder.NewBond(a16, a15, BondOrder.Single);
            mol.Bonds.Add(b14);
            IBond b15 = builder.NewBond(a16, a13, BondOrder.Single);
            mol.Bonds.Add(b15);
            IBond b16 = builder.NewBond(a16, a6, BondOrder.Single);
            mol.Bonds.Add(b16);
            IBond b17 = builder.NewBond(a17, a16, BondOrder.Single);
            mol.Bonds.Add(b17);
            IBond b18 = builder.NewBond(a18, a7, BondOrder.Single);
            b18.IsAromatic = true;
            mol.Bonds.Add(b18);
            IBond b19 = builder.NewBond(a19, a18, BondOrder.Double);
            b19.IsAromatic = true;
            mol.Bonds.Add(b19);
            IBond b20 = builder.NewBond(a19, a4, BondOrder.Single);
            b20.IsAromatic = true;
            mol.Bonds.Add(b20);
            return mol;
        }

        /// <summary>
        /// NCDK.Data.MDL.bug706786-2.mol
        /// C1COC2=CC=CC=C2C1
        /// </summary>
        // @cdk.inchi InChI=1/C9H10O/c1-2-6-9-8(4-1)5-3-7-10-9/h1-2,4,6H,3,5,7H2
        public static IAtomContainer Bug706786_2()
        {
            IChemObjectBuilder builder = ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("C");
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("O");
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.IsAromatic = true;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            a5.IsAromatic = true;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.NewAtom("C");
            a6.IsAromatic = true;
            mol.Atoms.Add(a6);
            IAtom a7 = builder.NewAtom("C");
            a7.IsAromatic = true;
            mol.Atoms.Add(a7);
            IAtom a8 = builder.NewAtom("C");
            a8.IsAromatic = true;
            mol.Atoms.Add(a8);
            IAtom a9 = builder.NewAtom("C");
            a9.IsAromatic = true;
            mol.Atoms.Add(a9);
            IAtom a10 = builder.NewAtom("C");
            mol.Atoms.Add(a10);
            IBond b1 = builder.NewBond(a2, a1, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a3, a2, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a4, a3, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a5, a4, BondOrder.Double);
            b4.IsAromatic = true;
            mol.Bonds.Add(b4);
            IBond b5 = builder.NewBond(a6, a5, BondOrder.Single);
            b5.IsAromatic = true;
            mol.Bonds.Add(b5);
            IBond b6 = builder.NewBond(a7, a6, BondOrder.Double);
            b6.IsAromatic = true;
            mol.Bonds.Add(b6);
            IBond b7 = builder.NewBond(a8, a7, BondOrder.Single);
            b7.IsAromatic = true;
            mol.Bonds.Add(b7);
            IBond b8 = builder.NewBond(a9, a8, BondOrder.Double);
            b8.IsAromatic = true;
            mol.Bonds.Add(b8);
            IBond b9 = builder.NewBond(a9, a4, BondOrder.Single);
            b9.IsAromatic = true;
            mol.Bonds.Add(b9);
            IBond b10 = builder.NewBond(a10, a9, BondOrder.Single);
            mol.Bonds.Add(b10);
            IBond b11 = builder.NewBond(a10, a1, BondOrder.Single);
            mol.Bonds.Add(b11);
            return mol;
        }

        /// <summary>
        /// /data/mdl/bug934819_1.mol
        /// [O-][N+](=O)C1=CC=CS1
        /// </summary>
        // @cdk.inchi InChI=1/C4H3NO2S/c6-5(7)4-2-1-3-8-4/h1-3H
        public static IAtomContainer Bug934819_1()
        {
            IChemObjectBuilder builder = ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("O");
            a1.FormalCharge = -1;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("N");
            a2.FormalCharge = 1;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("O");
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.IsAromatic = true;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            a5.IsAromatic = true;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.NewAtom("C");
            a6.IsAromatic = true;
            mol.Atoms.Add(a6);
            IAtom a7 = builder.NewAtom("C");
            a7.IsAromatic = true;
            mol.Atoms.Add(a7);
            IAtom a8 = builder.NewAtom("S");
            a8.IsAromatic = true;
            mol.Atoms.Add(a8);
            IBond b1 = builder.NewBond(a2, a1, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a3, a2, BondOrder.Double);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a4, a2, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a5, a4, BondOrder.Double);
            b4.IsAromatic = true;
            mol.Bonds.Add(b4);
            IBond b5 = builder.NewBond(a6, a5, BondOrder.Single);
            b5.IsAromatic = true;
            mol.Bonds.Add(b5);
            IBond b6 = builder.NewBond(a7, a6, BondOrder.Double);
            b6.IsAromatic = true;
            mol.Bonds.Add(b6);
            IBond b7 = builder.NewBond(a8, a7, BondOrder.Single);
            b7.IsAromatic = true;
            mol.Bonds.Add(b7);
            IBond b8 = builder.NewBond(a8, a4, BondOrder.Single);
            b8.IsAromatic = true;
            mol.Bonds.Add(b8);
            return mol;
        }

        /// <summary>
        /// /data/mdl/bug934819-2.mol
        /// CCCCSC1=CC=C(S1)C#CC1=CC=C(S1)[N+]([O-])=O
        /// </summary>
        // @cdk.inchi InChI=1/C14H13NO2S3/c1-2-3-10-18-14-9-7-12(20-14)5-4-11-6-8-13(19-11)15(16)17/h6-9H,2-3,10H2,1H3
        public static IAtomContainer Bug934819_2()
        {
            IChemObjectBuilder builder = ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("C");
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("S");
            mol.Atoms.Add(a5);
            IAtom a6 = builder.NewAtom("C");
            a6.IsAromatic = true;
            mol.Atoms.Add(a6);
            IAtom a7 = builder.NewAtom("C");
            a7.IsAromatic = true;
            mol.Atoms.Add(a7);
            IAtom a8 = builder.NewAtom("C");
            a8.IsAromatic = true;
            mol.Atoms.Add(a8);
            IAtom a9 = builder.NewAtom("C");
            a9.IsAromatic = true;
            mol.Atoms.Add(a9);
            IAtom a10 = builder.NewAtom("S");
            a10.IsAromatic = true;
            mol.Atoms.Add(a10);
            IAtom a11 = builder.NewAtom("C");
            mol.Atoms.Add(a11);
            IAtom a12 = builder.NewAtom("C");
            mol.Atoms.Add(a12);
            IAtom a13 = builder.NewAtom("C");
            a13.IsAromatic = true;
            mol.Atoms.Add(a13);
            IAtom a14 = builder.NewAtom("C");
            a14.IsAromatic = true;
            mol.Atoms.Add(a14);
            IAtom a15 = builder.NewAtom("C");
            a15.IsAromatic = true;
            mol.Atoms.Add(a15);
            IAtom a16 = builder.NewAtom("C");
            a16.IsAromatic = true;
            mol.Atoms.Add(a16);
            IAtom a17 = builder.NewAtom("S");
            a17.IsAromatic = true;
            mol.Atoms.Add(a17);
            IAtom a18 = builder.NewAtom("N");
            a18.FormalCharge = 1;
            mol.Atoms.Add(a18);
            IAtom a19 = builder.NewAtom("O");
            a19.FormalCharge = -1;
            mol.Atoms.Add(a19);
            IAtom a20 = builder.NewAtom("O");
            mol.Atoms.Add(a20);
            IBond b1 = builder.NewBond(a2, a1, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a3, a2, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a4, a3, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a5, a4, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.NewBond(a6, a5, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.NewBond(a7, a6, BondOrder.Double);
            b6.IsAromatic = true;
            mol.Bonds.Add(b6);
            IBond b7 = builder.NewBond(a8, a7, BondOrder.Single);
            b7.IsAromatic = true;
            mol.Bonds.Add(b7);
            IBond b8 = builder.NewBond(a9, a8, BondOrder.Double);
            b8.IsAromatic = true;
            mol.Bonds.Add(b8);
            IBond b9 = builder.NewBond(a10, a9, BondOrder.Single);
            b9.IsAromatic = true;
            mol.Bonds.Add(b9);
            IBond b10 = builder.NewBond(a10, a6, BondOrder.Single);
            b10.IsAromatic = true;
            mol.Bonds.Add(b10);
            IBond b11 = builder.NewBond(a11, a9, BondOrder.Single);
            mol.Bonds.Add(b11);
            IBond b12 = builder.NewBond(a12, a11, BondOrder.Triple);
            mol.Bonds.Add(b12);
            IBond b13 = builder.NewBond(a13, a12, BondOrder.Single);
            mol.Bonds.Add(b13);
            IBond b14 = builder.NewBond(a14, a13, BondOrder.Double);
            b14.IsAromatic = true;
            mol.Bonds.Add(b14);
            IBond b15 = builder.NewBond(a15, a14, BondOrder.Single);
            b15.IsAromatic = true;
            mol.Bonds.Add(b15);
            IBond b16 = builder.NewBond(a16, a15, BondOrder.Double);
            b16.IsAromatic = true;
            mol.Bonds.Add(b16);
            IBond b17 = builder.NewBond(a17, a16, BondOrder.Single);
            b17.IsAromatic = true;
            mol.Bonds.Add(b17);
            IBond b18 = builder.NewBond(a17, a13, BondOrder.Single);
            b18.IsAromatic = true;
            mol.Bonds.Add(b18);
            IBond b19 = builder.NewBond(a18, a16, BondOrder.Single);
            mol.Bonds.Add(b19);
            IBond b20 = builder.NewBond(a19, a18, BondOrder.Single);
            mol.Bonds.Add(b20);
            IBond b21 = builder.NewBond(a20, a18, BondOrder.Double);
            mol.Bonds.Add(b21);
            return mol;
        }

        public static BitArray AsBitSet(params int[] xs)
        {
            BitArray bs = new BitArray(0);
            foreach (var x in xs)
                BitArrays.SetValue(bs, x, true);
            return bs;
        }
    }
}
