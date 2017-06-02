/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
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
 *  */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.Tools.Manipulator;
using System;
using System.IO;
using System.Linq;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for the PDBReader class.
    /// </summary>
    // @cdk.module test-pdb
    // @author      Edgar Luttmann <edgar@uni-paderborn.de>
    // @author      Martin Eklund <martin.eklund@farmbio.uu.se>
    // @cdk.created 2001-08-09
    [TestClass()]
    public class PDBReaderTest : SimpleChemObjectReaderTest
    {
        protected override string testFile => "NCDK.Data.PDB.coffeine.pdb";
        static readonly PDBReader simpleReader = new PDBReader();
        protected override IChemObjectIO ChemObjectIOToTest => simpleReader;

        /// <summary>
        /// Test to see if PDB files with CONECT records are handled properly.
        /// </summary>
        // @cdk.bug 2046633
        [TestMethod()]
        public void TestConnectRecords()
        {
            string data = "SEQRES    111111111111111111111111111111111111111111111111111111111111111     \n"
                    + "ATOM      1  N   SER A 326     103.777  74.304  20.170  1.00 21.58           N\n"
                    + "ATOM      2  CA  SER A 326     102.613  74.991  20.586  1.00 18.59           C\n"
                    + "ATOM      3  C   SER A 326     101.631  74.211  21.431  1.00 17.75           C\n"
                    + "ATOM      4  O   SER A 326     101.653  74.549  22.634  1.00 18.51           O\n"
                    + "CONECT    1    4\n" + "CONECT    4    1\n" + "END    \n";

            StringReader stringReader = new StringReader(data);
            PDBReader reader = new PDBReader(stringReader);
            reader.IOSettings["UseRebondTool"].Setting = "false"; // UseRebondTool
            reader.IOSettings["ReadConnectSection"].Setting = "true"; // ReadConnectSection

            var obj = new ChemFile();
            reader.Read(obj);
            reader.Close();
            stringReader.Close();
            Assert.IsNotNull(obj);
            int bondCount = ((IChemFile)obj)[0][0].MoleculeSet[0]
                    .Bonds.Count;
            // if ReadConnectSection=true and UseRebondTool=false then bondCount ==
            // 1 (from just the CONECT) else if ReadConnectSection=false and
            // UseRebondTool=true then bondCount == 3 (just atoms within bonding
            // distance)
            Assert.AreEqual(1, bondCount);
        }

        [TestMethod()]
        public void ReadCharge()
        {
            string data = "HETATM 3486 MG    MG A 302      24.885  14.008  59.194  1.00 29.42          MG+2\n" + "END";
            IChemFile chemFile = GetChemFileFromString(data);
            IAtomContainer atomContainer = GetFirstAtomContainer(chemFile, 1, 1, 1);
            Assert.AreEqual(2, atomContainer.Atoms[0].Charge);
        }

        [TestMethod()]
        public void OldFormatNewFormatTest()
        {
            string oldFormat = "ATOM      1 1HA  UNK A   1      20.662  36.632  23.475  1.00 10.00      114D  45\nEND";
            string newFormat = "ATOM      1 1HA  UNK A   1      20.662  36.632  23.475  1.00 10.00           H\nEND";

            IChemFile oldFormatFile = GetChemFileFromString(oldFormat);
            IChemFile newFormatFile = GetChemFileFromString(newFormat);
            IAtomContainer acOld = GetFirstAtomContainer(oldFormatFile, 1, 1, 1);
            IAtomContainer acNew = GetFirstAtomContainer(newFormatFile, 1, 1, 1);
            Assert.AreEqual("H", acOld.Atoms[0].Symbol);
            Assert.AreEqual("H", acNew.Atoms[0].Symbol);
        }

        [TestMethod()]
        public void TestAccepts()
        {
            PDBReader reader = new PDBReader();
            Assert.IsTrue(reader.Accepts(typeof(ChemFile)));
        }

        [TestMethod()]
        public void TestPDBFileCoffein()
        {
            string filename = "NCDK.Data.PDB.coffeine.pdb";
            var ins = ResourceLoader.GetAsStream(filename);

            ISimpleChemObjectReader oReader = new PDBReader(ins);
            Assert.IsNotNull(oReader);

            IChemFile oChemFile = (IChemFile)oReader.Read(new ChemFile());
            Assert.IsNotNull(oChemFile);
            Assert.AreEqual(oChemFile.Count, 1);

            IChemSequence oSeq = oChemFile[0];
            Assert.IsNotNull(oSeq);
            Assert.AreEqual(oSeq.Count, 1);

            IChemModel oModel = oSeq[0];
            Assert.IsNotNull(oModel);
            Assert.AreEqual(1, oModel.MoleculeSet.Count);

            IAtomContainer container = oModel.MoleculeSet[0];
            Assert.IsFalse(container is IBioPolymer);
            Assert.IsTrue(container is IAtomContainer);
            IAtomContainer oMol = (IAtomContainer)container;
            Assert.IsNotNull(oMol);
            Assert.AreEqual(oMol.Atoms.Count, 14);

            IAtom nAtom = oMol.Atoms[0];
            Assert.IsNotNull(nAtom);
            Assert.IsTrue(nAtom is PDBAtom);
            PDBAtom oAtom = (PDBAtom)nAtom;
            Assert.AreEqual("C", oAtom.Symbol);
            Assert.AreEqual(1, oAtom.Serial.Value);
            Assert.AreEqual("C1", oAtom.Name);
            Assert.AreEqual("MOL", oAtom.ResName);
            Assert.AreEqual("1", oAtom.ResSeq);
            Assert.AreEqual(1.0, oAtom.Occupancy.Value, 0);
            Assert.AreEqual(0.0, oAtom.TempFactor.Value, 0);

            nAtom = oMol.Atoms[3];
            Assert.IsNotNull(nAtom);
            Assert.IsTrue(nAtom is PDBAtom);
            oAtom = (PDBAtom)nAtom;
            Assert.AreEqual("O", oAtom.Symbol);
            Assert.AreEqual(4, oAtom.Serial.Value);
            Assert.AreEqual("O4", oAtom.Name);
            Assert.AreEqual("MOL", oAtom.ResName);
            Assert.AreEqual("1", oAtom.ResSeq);
            Assert.AreEqual(1.0, oAtom.Occupancy.Value, 0);
            Assert.AreEqual(0.0, oAtom.TempFactor.Value, 0);

            nAtom = oMol.Atoms[oMol.Atoms.Count - 1];
            Assert.IsNotNull(nAtom);
            Assert.IsTrue(nAtom is PDBAtom);
            oAtom = (PDBAtom)nAtom;
            Assert.AreEqual("N", oAtom.Symbol);
            Assert.AreEqual(14, oAtom.Serial.Value);
            Assert.AreEqual("N14", oAtom.Name);
            Assert.AreEqual("MOL", oAtom.ResName);
            Assert.AreEqual("1", oAtom.ResSeq);
            Assert.AreEqual(1.0, oAtom.Occupancy.Value, 0);
            Assert.AreEqual(0.0, oAtom.TempFactor.Value, 0);
        }

        /// <summary>
        /// Tests reading a protein PDB file.
        /// </summary>
        [TestMethod()]
        public void TestProtein()
        {
            string filename = "NCDK.Data.PDB.Test-1crn.pdb";
            var ins = ResourceLoader.GetAsStream(filename);

            ISimpleChemObjectReader reader = new PDBReader(ins);
            Assert.IsNotNull(reader);

            ChemFile chemFile = (ChemFile)reader.Read(new ChemFile());
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(1, chemFile.Count);

            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(1, seq.Count);

            IChemModel model = seq[0];
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.MoleculeSet.Count());

            IAtomContainer container = model.MoleculeSet[0];
            Assert.IsTrue(container is IBioPolymer);
            IBioPolymer mol = (IBioPolymer)container;
            Assert.IsNotNull(mol);
            Assert.AreEqual(327, mol.Atoms.Count);
            Assert.AreEqual(46, mol.GetMonomerMap().Count());
            Assert.IsNotNull(mol.GetMonomer("THRA1", "A"));
            Assert.AreEqual(7, mol.GetMonomer("THRA1", "A").Atoms.Count);
            Assert.IsNotNull(mol.GetMonomer("ILEA7", "A"));
            Assert.AreEqual(8, mol.GetMonomer("ILEA7", "A").Atoms.Count);

            IAtom nAtom = mol.Atoms[94];
            Assert.IsNotNull(nAtom);
            Assert.IsTrue(nAtom is PDBAtom);
            PDBAtom atom = (PDBAtom)nAtom;
            Assert.AreEqual("C", atom.Symbol);
            Assert.AreEqual(95, atom.Serial.Value);
            Assert.AreEqual("CZ", atom.Name);
            Assert.AreEqual("PHE", atom.ResName);
            Assert.AreEqual("13", atom.ResSeq);
            Assert.AreEqual(1.0, atom.Occupancy.Value, 0.001);
            Assert.AreEqual(6.84, atom.TempFactor.Value, 0.001);
        }

        public IChemFile GetChemFileFromString(string data)
        {
            StringReader stringReader = new StringReader(data);
            PDBReader reader = new PDBReader(stringReader);
            Assert.IsNotNull(reader);
            return GetChemFile(reader);
        }

        public IChemFile GetChemFile(string filename)
        {
            return GetChemFile(filename, false);
        }

        public IChemFile GetChemFile(ISimpleChemObjectReader reader)
        {
            return GetChemFile(reader, false);
        }

        public IChemFile GetChemFile(string filename, bool useRebond)
        {
            var ins = ResourceLoader.GetAsStream(filename);
            return GetChemFile(new PDBReader(ins), useRebond);
        }

        public IChemFile GetChemFile(ISimpleChemObjectReader reader, bool useRebond)
        {
            Assert.IsNotNull(reader);

            reader.IOSettings["UseRebondTool"].Setting = useRebond.ToString();

            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            Assert.IsNotNull(chemFile);
            return chemFile;
        }

        public IAtomContainer GetFirstAtomContainer(IChemFile chemFile, int chemSequenceCount, int chemModelCount,
                int moleculeCount)
        {
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(chemSequenceCount, chemFile.Count);

            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(chemModelCount, seq.Count);

            IChemModel model = seq[0];
            Assert.IsNotNull(model);
            Assert.AreEqual(moleculeCount, model.MoleculeSet.Count());
            return model.MoleculeSet[0];
        }

        public void TestObjectCountsChemFile(IChemFile chemFile, int chemSequenceCount, int chemModelCount,
                int moleculeCount, int atomCount, int strandCount, int monomerCount, int structureCount)
        {
            IAtomContainer container = GetFirstAtomContainer(chemFile, chemSequenceCount, chemModelCount, moleculeCount);
            Assert.IsTrue(container is IBioPolymer);
            IBioPolymer polymer = (IBioPolymer)container;

            // chemical validation
            Assert.AreEqual(atomCount, ChemFileManipulator.GetAtomCount(chemFile));
            Assert.AreEqual(strandCount, polymer.GetStrandMap().Count);
            Assert.AreEqual(monomerCount, polymer.GetMonomerMap().Count());

            Assert.IsTrue(polymer is PDBPolymer);
            PDBPolymer pdb = (PDBPolymer)polymer;

            // PDB validation
            Assert.AreEqual(structureCount, pdb.GetStructures().Count());
        }

        [TestMethod()]
        public void Test114D()
        {
            string filename = "NCDK.Data.PDB.114D.pdb";
            var ins = ResourceLoader.GetAsStream(filename);

            ISimpleChemObjectReader reader = new PDBReader(ins);
            Assert.IsNotNull(reader);

            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(1, chemFile.Count);

            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(1, seq.Count);

            IChemModel model = seq[0];
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.MoleculeSet.Count());

            IAtomContainer container = model.MoleculeSet[0];
            Assert.IsTrue(container is IBioPolymer);
            IBioPolymer polymer = (IBioPolymer)container;

            Assert.IsTrue(polymer.GetStrand("A") is PDBStrand, "Strand A is not a PDBStrand");
            PDBStrand strandA = (PDBStrand)polymer.GetStrand("A");
            var lst = strandA.GetMonomerNamesInSequentialOrder().GetEnumerator();
            lst.MoveNext();
            string monomer1 = lst.Current;
            IMonomer mono1 = strandA.GetMonomer(monomer1);
            Assert.IsNotNull(mono1);
            Assert.IsNotNull(mono1.MonomerName);
            Assert.IsTrue(mono1 is PDBMonomer, "Monomer is not a PDBMonomer");
            PDBMonomer pdbMonomer = (PDBMonomer)mono1;
            Assert.AreEqual(pdbMonomer.ResSeq, "1");

            lst.MoveNext();
            string monomer2 = lst.Current;
            IMonomer mono2 = strandA.GetMonomer(monomer2);
            Assert.IsTrue(mono2 is PDBMonomer, "Monomer is not a PDBMonomer");
            PDBMonomer pdbMonomer2 = (PDBMonomer)mono2;
            Assert.AreEqual(pdbMonomer2.ResSeq, "2");

            // chemical validation
            Assert.AreEqual(552, ChemFileManipulator.GetAtomCount(chemFile));
            Assert.AreEqual(2, polymer.GetStrandMap().Count());
            Assert.AreEqual(24, polymer.GetMonomerMap().Count());

            Assert.IsTrue(polymer.GetStrandNames().Contains("A"));
            Assert.IsTrue(polymer.GetStrandNames().Contains("B"));
            Assert.IsFalse(polymer.GetStrandNames().Contains("C"));
            Assert.AreEqual(24, polymer.GetMonomerMap().Count());

            Assert.IsTrue(polymer is PDBPolymer);
            PDBPolymer pdb = (PDBPolymer)polymer;

            // PDB validation
            Assert.AreEqual(0, pdb.GetStructures().Count());
        }

        [TestMethod()]
        public void TestUnk()
        {
            string filename = "NCDK.Data.PDB.unk.pdb";
            IChemFile chemFile = GetChemFile(filename);
            IAtomContainer atomContainer = GetFirstAtomContainer(chemFile, 1, 1, 1);
            Assert.AreEqual(5, atomContainer.Atoms.Count);
            foreach (var atom in atomContainer.Atoms)
            {
                Assert.IsFalse(string.Equals(atom.Symbol, "1h", StringComparison.OrdinalIgnoreCase), "Improper element symbol " + atom.Symbol);
            }
        }

        [TestMethod()]
        public void TestHetatmOnly()
        {
            string filename = "NCDK.Data.PDB.hetatm_only.pdb";
            IChemFile chemFile = GetChemFile(filename, true);
            IAtomContainer atomContainer = GetFirstAtomContainer(chemFile, 1, 1, 1);
            Assert.IsTrue(atomContainer is IAtomContainer);
            Assert.AreEqual(14, atomContainer.Atoms.Count);
            Assert.AreEqual(15, atomContainer.Bonds.Count);
        }

        [TestMethod()]
        public void Test1SPX()
        {
            string filename = "NCDK.Data.PDB.1SPX.pdb";
            IChemFile chemFile = GetChemFile(filename);
            TestObjectCountsChemFile(chemFile, 1, 1, 1, 1904, 1, 237, 19);
        }

        [TestMethod()]
        public void Test1XKQ()
        {
            string filename = "NCDK.Data.PDB.1XKQ.pdb";
            IChemFile chemFile = GetChemFile(filename);
            TestObjectCountsChemFile(chemFile, 1, 1, 1, 8955, 4, 1085, 90);
        }

        [TestMethod()]
        public void Test1A00()
        {
            string filename = "NCDK.Data.PDB.1A00.pdb";
            IChemFile chemFile = GetChemFile(filename);
            TestObjectCountsChemFile(chemFile, 1, 1, 1, 4770, 4, 574, 35);
        }

        [TestMethod()]
        public void Test1BOQ()
        {
            string filename = "NCDK.Data.PDB.1BOQ.pdb";
            IChemFile chemFile = GetChemFile(filename);
            TestObjectCountsChemFile(chemFile, 1, 1, 1, 1538, 1, 198, 21);
        }

        [TestMethod()]
        public void Test1TOH()
        {
            string filename = "NCDK.Data.PDB.1TOH.pdb";
            IChemFile chemFile = GetChemFile(filename);
            TestObjectCountsChemFile(chemFile, 1, 1, 1, 2804, 1, 325, 23);
        }

        [TestCategory("SlowTest")]
        [TestMethod()]
        public void Test1CKV()
        {
            string filename = "NCDK.Data.PDB.1CKV.pdb";
            IChemFile chemFile = GetChemFile(filename);
            TestObjectCountsChemFile(chemFile, 1, 14, 1, 31066, 1, 141, 9);
        }

        [TestMethod()]
        public void Test1D66()
        {
            string filename = "NCDK.Data.PDB.1D66.pdb";
            var ins = ResourceLoader.GetAsStream(filename);

            ISimpleChemObjectReader reader = new PDBReader(ins);
            Assert.IsNotNull(reader);

            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(1, chemFile.Count);

            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(1, seq.Count);

            IChemModel model = seq[0];
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.MoleculeSet.Count());

            IAtomContainer container = model.MoleculeSet[0];
            Assert.IsTrue(container is IBioPolymer);
            IBioPolymer polymer = (IBioPolymer)container;

            Assert.IsTrue(polymer is PDBPolymer);
            PDBPolymer pdb = (PDBPolymer)polymer;
            Assert.AreEqual(4, pdb.GetStrandCount());

            Assert.IsTrue(polymer.GetStrandNames().Contains("D"));
            Assert.IsTrue(polymer.GetStrand("D") is PDBStrand, "Strand D is not a PDBStrand");
            Assert.IsTrue(polymer.GetStrandNames().Contains("E"));
            Assert.IsTrue(polymer.GetStrand("E") is PDBStrand, "Strand E is not a PDBStrand");
            Assert.IsTrue(polymer.GetStrandNames().Contains("A"));
            Assert.IsTrue(polymer.GetStrand("A") is PDBStrand, "Strand A is not a PDBStrand");
            Assert.IsTrue(polymer.GetStrandNames().Contains("B"));
            Assert.IsTrue(polymer.GetStrand("B") is PDBStrand, "Strand B is not a PDBStrand");

            //Check to pick up all 4 strands
            Assert.AreEqual(polymer.GetStrandMap().Count, 4);

            //The following check is to see that the first monomers in a strand
            //can be accessed consecutively
            //i.e. their resSeq numbering follows that in the File

            //Strand A
            PDBStrand strandA = (PDBStrand)polymer.GetStrand("A");
            var lst = strandA.GetMonomerNamesInSequentialOrder();

            //Should be 57 monomers in strand A
            Assert.AreEqual(57, lst.Count);
            var lstIter = lst.GetEnumerator();

            lstIter.MoveNext();
            string monomer1 = lstIter.Current;
            IMonomer mono1 = strandA.GetMonomer(monomer1);
            Assert.IsNotNull(mono1);
            Assert.IsNotNull(mono1.MonomerName);
            Assert.IsTrue(mono1 is PDBMonomer, "Monomer is not a PDBMonomer");
            PDBMonomer pdbMonomer = (PDBMonomer)mono1;
            Assert.AreEqual("A", pdbMonomer.ChainID);
            Assert.AreEqual("8", pdbMonomer.ResSeq);

            lstIter.MoveNext();
            monomer1 = lstIter.Current;
            mono1 = strandA.GetMonomer(monomer1);
            pdbMonomer = (PDBMonomer)mono1;
            Assert.AreEqual("A", pdbMonomer.ChainID);
            Assert.AreEqual("9", pdbMonomer.ResSeq);

            lstIter.MoveNext();
            monomer1 = lstIter.Current;
            mono1 = strandA.GetMonomer(monomer1);
            pdbMonomer = (PDBMonomer)mono1;
            Assert.AreEqual("A", pdbMonomer.ChainID);
            Assert.AreEqual("10", pdbMonomer.ResSeq);

            //Strand B
            PDBStrand strandB = (PDBStrand)polymer.GetStrand("B");
            lst = strandB.GetMonomerNamesInSequentialOrder();

            //Should be 57 monomers in strand B
            Assert.AreEqual(57, lst.Count);
            lstIter = lst.GetEnumerator();

            lstIter.MoveNext();
            monomer1 = lstIter.Current;
            mono1 = strandB.GetMonomer(monomer1);
            Assert.IsNotNull(mono1);
            Assert.IsNotNull(mono1.MonomerName);
            Assert.IsTrue(mono1 is PDBMonomer, "Monomer is not a PDBMonomer");
            pdbMonomer = (PDBMonomer)mono1;
            Assert.AreEqual("B", pdbMonomer.ChainID);
            Assert.AreEqual("8", pdbMonomer.ResSeq);

            lstIter.MoveNext();
            monomer1 = lstIter.Current;
            mono1 = strandB.GetMonomer(monomer1);
            pdbMonomer = (PDBMonomer)mono1;
            Assert.AreEqual("B", pdbMonomer.ChainID);
            Assert.AreEqual("9", pdbMonomer.ResSeq);

            lstIter.MoveNext();
            monomer1 = lstIter.Current;
            mono1 = strandB.GetMonomer(monomer1);
            pdbMonomer = (PDBMonomer)mono1;
            Assert.AreEqual("B", pdbMonomer.ChainID);
            Assert.AreEqual("10", pdbMonomer.ResSeq);

            //Strand E
            PDBStrand strandE = (PDBStrand)polymer.GetStrand("E");
            lst = strandE.GetMonomerNamesInSequentialOrder();

            //Should be 19 monomers in strand E
            Assert.AreEqual(19, lst.Count);
            lstIter = lst.GetEnumerator();

            lstIter.MoveNext();
            monomer1 = lstIter.Current;
            mono1 = strandE.GetMonomer(monomer1);
            Assert.IsNotNull(mono1);
            Assert.IsNotNull(mono1.MonomerName);
            Assert.IsTrue(mono1 is PDBMonomer, "Monomer is not a PDBMonomer");
            pdbMonomer = (PDBMonomer)mono1;
            Assert.AreEqual("E", pdbMonomer.ChainID);
            Assert.AreEqual("20", pdbMonomer.ResSeq);

            lstIter.MoveNext();
            monomer1 = lstIter.Current;
            mono1 = strandE.GetMonomer(monomer1);
            pdbMonomer = (PDBMonomer)mono1;
            Assert.AreEqual("E", pdbMonomer.ChainID);
            Assert.AreEqual("21", pdbMonomer.ResSeq);

            lstIter.MoveNext();
            monomer1 = lstIter.Current;
            mono1 = strandE.GetMonomer(monomer1);
            pdbMonomer = (PDBMonomer)mono1;
            Assert.AreEqual("E", pdbMonomer.ChainID);
            Assert.AreEqual("22", pdbMonomer.ResSeq);

            //Chain D should be 1,2,3...19
            PDBStrand strandD = (PDBStrand)polymer.GetStrand("D");
            lst = strandD.GetMonomerNamesInSequentialOrder();

            //Should be 19 monomers in strand D
            Assert.AreEqual(19, lst.Count);
            lstIter = lst.GetEnumerator();

            lstIter.MoveNext();
            monomer1 = lstIter.Current;
            mono1 = strandD.GetMonomer(monomer1);
            Assert.IsNotNull(mono1);
            Assert.IsNotNull(mono1.MonomerName);
            Assert.IsTrue(mono1 is PDBMonomer, "Monomer is not a PDBMonomer");
            pdbMonomer = (PDBMonomer)mono1;
            Assert.AreEqual("D", pdbMonomer.ChainID);
            Assert.AreEqual("1", pdbMonomer.ResSeq);

            lstIter.MoveNext();
            monomer1 = lstIter.Current;
            mono1 = strandD.GetMonomer(monomer1);
            pdbMonomer = (PDBMonomer)mono1;
            Assert.AreEqual("D", pdbMonomer.ChainID);
            Assert.AreEqual("2", pdbMonomer.ResSeq);

            lstIter.MoveNext();
            monomer1 = lstIter.Current;
            mono1 = strandD.GetMonomer(monomer1);
            pdbMonomer = (PDBMonomer)mono1;
            Assert.AreEqual("D", pdbMonomer.ChainID);
            Assert.AreEqual("3", pdbMonomer.ResSeq);

            // PDB Structures validation
            //Should have 6 helices
            Assert.AreEqual(6, pdb.GetStructures().Count());
        }

        // @cdk.bug 489
        [TestCategory("SlowTest")]
        [TestMethod()]
        public void ReadFinalPump()
        {
            IChemFile chemFile = new PDBReader(ResourceLoader.GetAsStream(GetType(), "finalPump96.09.06.pdb")).Read(new ChemFile());
        }
    }
}
