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
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        private readonly IChemObjectBuilder builder = CDK.Builder;
        protected override string TestFile => "NCDK.Data.PDB.coffeine.pdb";
        protected override Type ChemObjectIOToTestType => typeof(PDBReader);

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

            IChemFile obj;
            using (var reader = new PDBReader(new StringReader(data)))
            {
                reader.IOSettings["UseRebondTool"].Setting = "false"; // UseRebondTool
                reader.IOSettings["ReadConnectSection"].Setting = "true"; // ReadConnectSection

                obj = builder.NewChemFile();
                reader.Read(obj);
            }
            Assert.IsNotNull(obj);
            var bondCount = obj[0][0].MoleculeSet[0].Bonds.Count;
            Assert.AreEqual(1, bondCount);
        }

        [TestMethod()]
        public void ReadCharge()
        {
            var data = "HETATM 3486 MG    MG A 302      24.885  14.008  59.194  1.00 29.42          MG+2\n" + "END";
            var chemFile = GetChemFileFromString(data);
            var atomContainer = GetFirstAtomContainer(chemFile, 1, 1, 1);
            Assert.AreEqual(2, atomContainer.Atoms[0].Charge);
        }

        [TestMethod()]
        public void OldFormatNewFormatTest()
        {
            var oldFormat = "ATOM      1 1HA  UNK A   1      20.662  36.632  23.475  1.00 10.00      114D  45\nEND";
            var newFormat = "ATOM      1 1HA  UNK A   1      20.662  36.632  23.475  1.00 10.00           H\nEND";

            var oldFormatFile = GetChemFileFromString(oldFormat);
            var newFormatFile = GetChemFileFromString(newFormat);
            var acOld = GetFirstAtomContainer(oldFormatFile, 1, 1, 1);
            var acNew = GetFirstAtomContainer(newFormatFile, 1, 1, 1);
            Assert.AreEqual("H", acOld.Atoms[0].Symbol);
            Assert.AreEqual("H", acNew.Atoms[0].Symbol);
        }

        [TestMethod()]
        public void TestAccepts()
        {
            var reader = new PDBReader(new StringReader(""));
            Assert.IsTrue(reader.Accepts(typeof(IChemFile)));
        }

        [TestMethod()]
        public void TestPDBFileCoffein()
        {
            var filename = "NCDK.Data.PDB.coffeine.pdb";
            var ins = ResourceLoader.GetAsStream(filename);

            var oReader = new PDBReader(ins);
            Assert.IsNotNull(oReader);

            var oChemFile = (IChemFile)oReader.Read(builder.NewChemFile());
            Assert.IsNotNull(oChemFile);
            Assert.AreEqual(oChemFile.Count, 1);

            var oSeq = oChemFile[0];
            Assert.IsNotNull(oSeq);
            Assert.AreEqual(oSeq.Count, 1);

            var oModel = oSeq[0];
            Assert.IsNotNull(oModel);
            Assert.AreEqual(1, oModel.MoleculeSet.Count);

            var container = oModel.MoleculeSet[0];
            Assert.IsFalse(container is IBioPolymer);
            Assert.IsTrue(container is IAtomContainer);
            var oMol = (IAtomContainer)container;
            Assert.IsNotNull(oMol);
            Assert.AreEqual(oMol.Atoms.Count, 14);

            var nAtom = oMol.Atoms[0];
            Assert.IsNotNull(nAtom);
            Assert.IsTrue(nAtom is IPDBAtom);
            var oAtom = (IPDBAtom)nAtom;
            Assert.AreEqual("C", oAtom.Symbol);
            Assert.AreEqual(1, oAtom.Serial.Value);
            Assert.AreEqual("C1", oAtom.Name);
            Assert.AreEqual("MOL", oAtom.ResName);
            Assert.AreEqual("1", oAtom.ResSeq);
            Assert.AreEqual(1.0, oAtom.Occupancy.Value, 0);
            Assert.AreEqual(0.0, oAtom.TempFactor.Value, 0);

            nAtom = oMol.Atoms[3];
            Assert.IsNotNull(nAtom);
            Assert.IsTrue(nAtom is IPDBAtom);
            oAtom = (IPDBAtom)nAtom;
            Assert.AreEqual("O", oAtom.Symbol);
            Assert.AreEqual(4, oAtom.Serial.Value);
            Assert.AreEqual("O4", oAtom.Name);
            Assert.AreEqual("MOL", oAtom.ResName);
            Assert.AreEqual("1", oAtom.ResSeq);
            Assert.AreEqual(1.0, oAtom.Occupancy.Value, 0);
            Assert.AreEqual(0.0, oAtom.TempFactor.Value, 0);

            nAtom = oMol.Atoms[oMol.Atoms.Count - 1];
            Assert.IsNotNull(nAtom);
            Assert.IsTrue(nAtom is IPDBAtom);
            oAtom = (IPDBAtom)nAtom;
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
            var filename = "NCDK.Data.PDB.Test-1crn.pdb";
            var ins = ResourceLoader.GetAsStream(filename);

            var reader = new PDBReader(ins);
            Assert.IsNotNull(reader);

            var chemFile = (IChemFile)reader.Read(builder.NewChemFile());
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(1, chemFile.Count);

            var seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(1, seq.Count);

            var model = seq[0];
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.MoleculeSet.Count());

            var container = model.MoleculeSet[0];
            Assert.IsTrue(container is IBioPolymer);
            var mol = (IBioPolymer)container;
            Assert.IsNotNull(mol);
            Assert.AreEqual(327, mol.Atoms.Count);
            Assert.AreEqual(46, mol.GetMonomerMap().Count());
            Assert.IsNotNull(mol.GetMonomer("THRA1", "A"));
            Assert.AreEqual(7, mol.GetMonomer("THRA1", "A").Atoms.Count);
            Assert.IsNotNull(mol.GetMonomer("ILEA7", "A"));
            Assert.AreEqual(8, mol.GetMonomer("ILEA7", "A").Atoms.Count);

            var nAtom = mol.Atoms[94];
            Assert.IsNotNull(nAtom);
            Assert.IsTrue(nAtom is IPDBAtom);
            var atom = (IPDBAtom)nAtom;
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
            var stringReader = new StringReader(data);
            var reader = new PDBReader(stringReader);
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
            using (var reader = new PDBReader(ResourceLoader.GetAsStream(filename)))
            {
                return GetChemFile(reader, useRebond);
            }
        }

        public IChemFile GetChemFile(ISimpleChemObjectReader reader, bool useRebond)
        {
            Assert.IsNotNull(reader);

            reader.IOSettings["UseRebondTool"].Setting = useRebond.ToString();

            var chemFile = reader.Read(builder.NewChemFile());
            Assert.IsNotNull(chemFile);
            return chemFile;
        }

        public IAtomContainer GetFirstAtomContainer(IChemFile chemFile, int chemSequenceCount, int chemModelCount, int moleculeCount)
        {
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(chemSequenceCount, chemFile.Count);

            var seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(chemModelCount, seq.Count);

            var model = seq[0];
            Assert.IsNotNull(model);
            Assert.AreEqual(moleculeCount, model.MoleculeSet.Count());
            return model.MoleculeSet[0];
        }

        public void TestObjectCountsChemFile(IChemFile chemFile, int chemSequenceCount, int chemModelCount,
                int moleculeCount, int atomCount, int strandCount, int monomerCount, int structureCount)
        {
            var container = GetFirstAtomContainer(chemFile, chemSequenceCount, chemModelCount, moleculeCount);
            Assert.IsTrue(container is IBioPolymer);
            var polymer = (IBioPolymer)container;

            // chemical validation
            Assert.AreEqual(atomCount, ChemFileManipulator.GetAtomCount(chemFile));
            Assert.AreEqual(strandCount, polymer.GetStrandMap().Count);
            Assert.AreEqual(monomerCount, polymer.GetMonomerMap().Count());

            Assert.IsTrue(polymer is IPDBPolymer);
            var pdb = (IPDBPolymer)polymer;

            // PDB validation
            Assert.AreEqual(structureCount, pdb.GetStructures().Count());
        }

        [TestMethod()]
        public void Test114D()
        {
            var filename = "NCDK.Data.PDB.114D.pdb";
            var ins = ResourceLoader.GetAsStream(filename);

            var reader = new PDBReader(ins);
            Assert.IsNotNull(reader);

            var chemFile = (IChemFile)reader.Read(builder.NewChemFile());
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(1, chemFile.Count);

            var seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(1, seq.Count);

            var model = seq[0];
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.MoleculeSet.Count());

            var container = model.MoleculeSet[0];
            Assert.IsTrue(container is IBioPolymer);
            var polymer = (IBioPolymer)container;

            Assert.IsTrue(polymer.GetStrand("A") is IPDBStrand, "Strand A is not a PDBStrand");
            var strandA = (IPDBStrand)polymer.GetStrand("A");
            var lst = strandA.GetMonomerNamesInSequentialOrder().GetEnumerator();
            lst.MoveNext();
            var monomer1 = lst.Current;
            var mono1 = strandA.GetMonomer(monomer1);
            Assert.IsNotNull(mono1);
            Assert.IsNotNull(mono1.MonomerName);
            Assert.IsTrue(mono1 is IPDBMonomer, "Monomer is not a PDBMonomer");
            var pdbMonomer = (IPDBMonomer)mono1;
            Assert.AreEqual(pdbMonomer.ResSeq, "1");

            lst.MoveNext();
            var monomer2 = lst.Current;
            var mono2 = strandA.GetMonomer(monomer2);
            Assert.IsTrue(mono2 is IPDBMonomer, "Monomer is not a PDBMonomer");
            var pdbMonomer2 = (IPDBMonomer)mono2;
            Assert.AreEqual(pdbMonomer2.ResSeq, "2");

            // chemical validation
            Assert.AreEqual(552, ChemFileManipulator.GetAtomCount(chemFile));
            Assert.AreEqual(2, polymer.GetStrandMap().Count());
            Assert.AreEqual(24, polymer.GetMonomerMap().Count());

            Assert.IsTrue(polymer.GetStrandNames().Contains("A"));
            Assert.IsTrue(polymer.GetStrandNames().Contains("B"));
            Assert.IsFalse(polymer.GetStrandNames().Contains("C"));
            Assert.AreEqual(24, polymer.GetMonomerMap().Count());

            Assert.IsTrue(polymer is IPDBPolymer);
            var pdb = (IPDBPolymer)polymer;

            // PDB validation
            Assert.AreEqual(0, pdb.GetStructures().Count());
        }

        [TestMethod()]
        public void TestUnk()
        {
            var filename = "NCDK.Data.PDB.unk.pdb";
            var chemFile = GetChemFile(filename);
            var atomContainer = GetFirstAtomContainer(chemFile, 1, 1, 1);
            Assert.AreEqual(5, atomContainer.Atoms.Count);
            foreach (var atom in atomContainer.Atoms)
            {
                Assert.IsFalse(string.Equals(atom.Symbol, "1h", StringComparison.OrdinalIgnoreCase), "Improper element symbol " + atom.Symbol);
            }
        }

        [TestMethod()]
        public void TestHetatmOnly()
        {
            var filename = "NCDK.Data.PDB.hetatm_only.pdb";
            var chemFile = GetChemFile(filename, true);
            var atomContainer = GetFirstAtomContainer(chemFile, 1, 1, 1);
            Assert.IsTrue(atomContainer is IAtomContainer);
            Assert.AreEqual(14, atomContainer.Atoms.Count);
            Assert.AreEqual(15, atomContainer.Bonds.Count);
        }

        [TestMethod()]
        public void Test1SPX()
        {
            var filename = "NCDK.Data.PDB.1SPX.pdb";
            var chemFile = GetChemFile(filename);
            TestObjectCountsChemFile(chemFile, 1, 1, 1, 1904, 1, 237, 19);
        }

        [TestMethod()]
        public void Test1XKQ()
        {
            var filename = "NCDK.Data.PDB.1XKQ.pdb";
            var chemFile = GetChemFile(filename);
            TestObjectCountsChemFile(chemFile, 1, 1, 1, 8955, 4, 1085, 90);
        }

        [TestMethod()]
        public void Test1A00()
        {
            var filename = "NCDK.Data.PDB.1A00.pdb";
            var chemFile = GetChemFile(filename);
            TestObjectCountsChemFile(chemFile, 1, 1, 1, 4770, 4, 574, 35);
        }

        [TestMethod()]
        public void Test1BOQ()
        {
            var filename = "NCDK.Data.PDB.1BOQ.pdb";
            var chemFile = GetChemFile(filename);
            TestObjectCountsChemFile(chemFile, 1, 1, 1, 1538, 1, 198, 21);
        }

        [TestMethod()]
        public void Test1TOH()
        {
            var filename = "NCDK.Data.PDB.1TOH.pdb";
            var chemFile = GetChemFile(filename);
            TestObjectCountsChemFile(chemFile, 1, 1, 1, 2804, 1, 325, 23);
        }

        [TestCategory("SlowTest")]
        [TestMethod()]
        public void Test1CKV()
        {
            var filename = "NCDK.Data.PDB.1CKV.pdb";
            var chemFile = GetChemFile(filename);
            TestObjectCountsChemFile(chemFile, 1, 14, 1, 31066, 1, 141, 9);
        }

        [TestMethod()]
        public void Test1D66()
        {
            var filename = "NCDK.Data.PDB.1D66.pdb";
            var ins = ResourceLoader.GetAsStream(filename);

            var reader = new PDBReader(ins);
            Assert.IsNotNull(reader);

            var chemFile = (IChemFile)reader.Read(builder.NewChemFile());
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(1, chemFile.Count);

            var seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(1, seq.Count);

            var model = seq[0];
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.MoleculeSet.Count());

            var container = model.MoleculeSet[0];
            Assert.IsTrue(container is IBioPolymer);
            var polymer = (IBioPolymer)container;

            Assert.IsTrue(polymer is IPDBPolymer);
            var pdb = (IPDBPolymer)polymer;
            Assert.AreEqual(4, pdb.GetStrandMap().Count);

            Assert.IsTrue(polymer.GetStrandNames().Contains("D"));
            Assert.IsTrue(polymer.GetStrand("D") is IPDBStrand, "Strand D is not a PDBStrand");
            Assert.IsTrue(polymer.GetStrandNames().Contains("E"));
            Assert.IsTrue(polymer.GetStrand("E") is IPDBStrand, "Strand E is not a PDBStrand");
            Assert.IsTrue(polymer.GetStrandNames().Contains("A"));
            Assert.IsTrue(polymer.GetStrand("A") is IPDBStrand, "Strand A is not a PDBStrand");
            Assert.IsTrue(polymer.GetStrandNames().Contains("B"));
            Assert.IsTrue(polymer.GetStrand("B") is IPDBStrand, "Strand B is not a PDBStrand");

            //Check to pick up all 4 strands
            Assert.AreEqual(polymer.GetStrandMap().Count, 4);

            //The following check is to see that the first monomers in a strand
            //can be accessed consecutively
            //i.e. their resSeq numbering follows that in the File

            //Strand A
            var strandA = (IPDBStrand)polymer.GetStrand("A");
            var lst = strandA.GetMonomerNamesInSequentialOrder();

            //Should be 57 monomers in strand A
            Assert.AreEqual(57, lst.Count);
            var lstIter = lst.GetEnumerator();

            lstIter.MoveNext();
            var monomer1 = lstIter.Current;
            var mono1 = strandA.GetMonomer(monomer1);
            Assert.IsNotNull(mono1);
            Assert.IsNotNull(mono1.MonomerName);
            Assert.IsTrue(mono1 is IPDBMonomer, "Monomer is not a PDBMonomer");
            var pdbMonomer = (IPDBMonomer)mono1;
            Assert.AreEqual("A", pdbMonomer.ChainID);
            Assert.AreEqual("8", pdbMonomer.ResSeq);

            lstIter.MoveNext();
            monomer1 = lstIter.Current;
            mono1 = strandA.GetMonomer(monomer1);
            pdbMonomer = (IPDBMonomer)mono1;
            Assert.AreEqual("A", pdbMonomer.ChainID);
            Assert.AreEqual("9", pdbMonomer.ResSeq);

            lstIter.MoveNext();
            monomer1 = lstIter.Current;
            mono1 = strandA.GetMonomer(monomer1);
            pdbMonomer = (IPDBMonomer)mono1;
            Assert.AreEqual("A", pdbMonomer.ChainID);
            Assert.AreEqual("10", pdbMonomer.ResSeq);

            //Strand B
            var strandB = (IPDBStrand)polymer.GetStrand("B");
            lst = strandB.GetMonomerNamesInSequentialOrder();

            //Should be 57 monomers in strand B
            Assert.AreEqual(57, lst.Count);
            lstIter = lst.GetEnumerator();

            lstIter.MoveNext();
            monomer1 = lstIter.Current;
            mono1 = strandB.GetMonomer(monomer1);
            Assert.IsNotNull(mono1);
            Assert.IsNotNull(mono1.MonomerName);
            Assert.IsTrue(mono1 is IPDBMonomer, "Monomer is not a PDBMonomer");
            pdbMonomer = (IPDBMonomer)mono1;
            Assert.AreEqual("B", pdbMonomer.ChainID);
            Assert.AreEqual("8", pdbMonomer.ResSeq);

            lstIter.MoveNext();
            monomer1 = lstIter.Current;
            mono1 = strandB.GetMonomer(monomer1);
            pdbMonomer = (IPDBMonomer)mono1;
            Assert.AreEqual("B", pdbMonomer.ChainID);
            Assert.AreEqual("9", pdbMonomer.ResSeq);

            lstIter.MoveNext();
            monomer1 = lstIter.Current;
            mono1 = strandB.GetMonomer(monomer1);
            pdbMonomer = (IPDBMonomer)mono1;
            Assert.AreEqual("B", pdbMonomer.ChainID);
            Assert.AreEqual("10", pdbMonomer.ResSeq);

            //Strand E
            var strandE = (IPDBStrand)polymer.GetStrand("E");
            lst = strandE.GetMonomerNamesInSequentialOrder();

            //Should be 19 monomers in strand E
            Assert.AreEqual(19, lst.Count);
            lstIter = lst.GetEnumerator();

            lstIter.MoveNext();
            monomer1 = lstIter.Current;
            mono1 = strandE.GetMonomer(monomer1);
            Assert.IsNotNull(mono1);
            Assert.IsNotNull(mono1.MonomerName);
            Assert.IsTrue(mono1 is IPDBMonomer, "Monomer is not a PDBMonomer");
            pdbMonomer = (IPDBMonomer)mono1;
            Assert.AreEqual("E", pdbMonomer.ChainID);
            Assert.AreEqual("20", pdbMonomer.ResSeq);

            lstIter.MoveNext();
            monomer1 = lstIter.Current;
            mono1 = strandE.GetMonomer(monomer1);
            pdbMonomer = (IPDBMonomer)mono1;
            Assert.AreEqual("E", pdbMonomer.ChainID);
            Assert.AreEqual("21", pdbMonomer.ResSeq);

            lstIter.MoveNext();
            monomer1 = lstIter.Current;
            mono1 = strandE.GetMonomer(monomer1);
            pdbMonomer = (IPDBMonomer)mono1;
            Assert.AreEqual("E", pdbMonomer.ChainID);
            Assert.AreEqual("22", pdbMonomer.ResSeq);

            //Chain D should be 1,2,3...19
            var strandD = (IPDBStrand)polymer.GetStrand("D");
            lst = strandD.GetMonomerNamesInSequentialOrder();

            //Should be 19 monomers in strand D
            Assert.AreEqual(19, lst.Count);
            lstIter = lst.GetEnumerator();

            lstIter.MoveNext();
            monomer1 = lstIter.Current;
            mono1 = strandD.GetMonomer(monomer1);
            Assert.IsNotNull(mono1);
            Assert.IsNotNull(mono1.MonomerName);
            Assert.IsTrue(mono1 is IPDBMonomer, "Monomer is not a PDBMonomer");
            pdbMonomer = (IPDBMonomer)mono1;
            Assert.AreEqual("D", pdbMonomer.ChainID);
            Assert.AreEqual("1", pdbMonomer.ResSeq);

            lstIter.MoveNext();
            monomer1 = lstIter.Current;
            mono1 = strandD.GetMonomer(monomer1);
            pdbMonomer = (IPDBMonomer)mono1;
            Assert.AreEqual("D", pdbMonomer.ChainID);
            Assert.AreEqual("2", pdbMonomer.ResSeq);

            lstIter.MoveNext();
            monomer1 = lstIter.Current;
            mono1 = strandD.GetMonomer(monomer1);
            pdbMonomer = (IPDBMonomer)mono1;
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
            using (var reader = new PDBReader(ResourceLoader.GetAsStream(GetType(), "finalPump96.09.06.pdb")))
            {
                var chemFile = reader.Read(builder.NewChemFile());
            }
        }
    }
}
