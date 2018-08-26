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
using NCDK.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Silent;
using NCDK.Templates;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for the PDBWriter class.
    /// </summary>
    // @cdk.module test-pdb
    // @author      Egon Willighagen
    // @cdk.created 2001-08-09
    [TestClass()]
    public class PDBWriterTest : ChemObjectIOTest
    {
        protected override Type ChemObjectIOToTestType => typeof(MDLRXNWriter);
        private static IChemObjectBuilder builder = ChemObjectBuilder.Instance;

        [TestMethod()]
        public void TestRoundTrip()
        {
            StringWriter sWriter = new StringWriter();
            PDBWriter writer = new PDBWriter(sWriter);

            ICrystal crystal = builder.NewCrystal();
            crystal.A = new Vector3(0, 1, 0);
            crystal.B = new Vector3(1, 0, 0);
            crystal.C = new Vector3(0, 0, 2);

            IAtom atom = builder.NewAtom("C");
            atom.Point3D = new Vector3(0.1, 0.1, 0.3);
            crystal.Atoms.Add(atom);

            writer.Write(crystal);
            writer.Close();

            string output = sWriter.ToString();
            Assert.IsNotNull(output);
            Assert.IsTrue(output.Length > 0);

            PDBReader reader = new PDBReader(new StringReader(""));
            ChemFile chemFile = (ChemFile)reader.Read(new ChemFile());
            reader.Close();

            Assert.IsNotNull(chemFile);
            Assert.AreEqual(1, chemFile.Count);
            IChemSequence sequence = chemFile[0];
            Assert.AreEqual(1, sequence.Count);
            IChemModel chemModel = sequence[0];
            Assert.IsNotNull(chemModel);

            // can't do further testing as the PDBReader does not read
            // Crystal structures :(
        }

        [TestMethod()]
        public void TestRoundTrip_fractionalCoordinates()
        {
            StringWriter sWriter = new StringWriter();
            PDBWriter writer = new PDBWriter(sWriter);

            Crystal crystal = new Crystal();
            crystal.A = new Vector3(0, 1, 0);
            crystal.B = new Vector3(1, 0, 0);
            crystal.C = new Vector3(0, 0, 2);

            IAtom atom = new Atom("C");
            atom.FractionalPoint3D = new Vector3(0.1, 0.1, 0.3);
            crystal.Atoms.Add(atom);

            writer.Write(crystal);
            writer.Close();

            string output = sWriter.ToString();
            Assert.IsNotNull(output);
            Assert.IsTrue(output.Length > 0);

            PDBReader reader = new PDBReader(new StringReader(""));
            ChemFile chemFile = (ChemFile)reader.Read(new ChemFile());
            reader.Close();

            Assert.IsNotNull(chemFile);
            Assert.AreEqual(1, chemFile.Count);
            IChemSequence sequence = chemFile[0];
            Assert.AreEqual(1, sequence.Count);
            IChemModel chemModel = sequence[0];
            Assert.IsNotNull(chemModel);

            // can't do further testing as the PDBReader does not read
            // Crystal structures :(
        }

        private IAtomContainer SingleAtomMolecule()
        {
            return SingleAtomMolecule("");
        }

        private IAtomContainer SingleAtomMolecule(string id)
        {
            return SingleAtomMolecule(id, null);
        }

        private IAtomContainer SingleAtomMolecule(string id, int? formalCharge)
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("C", new Vector3(0.0, 0.0, 0.0));
            mol.Atoms.Add(atom);
            mol.Id = id;
            if (formalCharge != null)
            {
                atom.FormalCharge = formalCharge;
            }
            return mol;
        }

        private IAtomContainer SingleBondMolecule()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(new Atom("C", new Vector3(0.0, 0.0, 0.0)));
            mol.Atoms.Add(new Atom("O", new Vector3(1.0, 1.0, 1.0)));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            return mol;
        }

        private string GetAsString(IAtomContainer mol)
        {
            StringWriter stringWriter = new StringWriter();
            PDBWriter writer = new PDBWriter(stringWriter);
            writer.WriteMolecule(mol);
            writer.Close();
            return stringWriter.ToString();
        }

        private IEnumerable<string> GetAsStringArray(IAtomContainer mol)
        {
            var s = GetAsString(mol);
            using (var reader = new StringReader(s))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                    yield return line;
            }
            yield break;
        }

        [TestMethod()]
        public void WriteAsHET()
        {
            IAtomContainer mol = SingleAtomMolecule();
            StringWriter stringWriter = new StringWriter();
            PDBWriter writer = new PDBWriter(stringWriter);
            writer.IOSettings["WriteAsHET"].Setting = "true";
            writer.WriteMolecule(mol);
            writer.Close();
            string asString = stringWriter.ToString();
            Assert.IsTrue(asString.IndexOf("HETATM") != -1);
        }

        [TestMethod()]
        public void WriteAsATOM()
        {
            IAtomContainer mol = SingleAtomMolecule();
            StringWriter stringWriter = new StringWriter();
            PDBWriter writer = new PDBWriter(stringWriter);
            writer.IOSettings["WriteAsHET"].Setting = "false";
            writer.WriteMolecule(mol);
            writer.Close();
            string asString = stringWriter.ToString();
            Assert.IsTrue(asString.IndexOf("ATOM") != -1);
        }

        [TestMethod()]
        public void WriteMolID()
        {
            IAtomContainer mol = SingleAtomMolecule("ZZZ");
            Assert.IsTrue(GetAsString(mol).IndexOf("ZZZ") != -1);
        }

        [TestMethod()]
        public void WriteNullMolID()
        {
            IAtomContainer mol = SingleAtomMolecule(null);
            Assert.IsTrue(GetAsString(mol).IndexOf("MOL") != -1);
        }

        [TestMethod()]
        public void WriteEmptyStringMolID()
        {
            IAtomContainer mol = SingleAtomMolecule("");
            Assert.IsTrue(GetAsString(mol).IndexOf("MOL") != -1);
        }

        [TestMethod()]
        public void WriteChargedAtom()
        {
            IAtomContainer mol = SingleAtomMolecule("", 1);
            var lines = GetAsStringArray(mol).ToArray();
            Assert.IsTrue(lines[lines.Length - 2].EndsWith("+1", StringComparison.Ordinal));
        }

        [TestMethod()]
        public void WriteMoleculeWithBond()
        {
            IAtomContainer mol = SingleBondMolecule();
            string[] lines = GetAsStringArray(mol).ToArray();
            string lastLineButTwo = lines[lines.Length - 3];
            string lastLineButOne = lines[lines.Length - 2];
            Assert.AreEqual("CONECT    1    2", lastLineButTwo);
            Assert.AreEqual("CONECT    2    1", lastLineButOne);
        }

        private void SetCoordinatesToZero(IAtomContainer mol)
        {
            foreach (var atom in mol.Atoms)
            {
                atom.Point3D = new Vector3(0.0, 0.0, 0.0);
            }
        }

        [TestMethod()]
        public void MolfactoryRoundtripTest()
        {
            IAtomContainer original = TestMoleculeFactory.MakePyrrole();
            SetCoordinatesToZero(original);
            StringWriter stringWriter = new StringWriter();
            PDBWriter writer = new PDBWriter(stringWriter);
            writer.WriteMolecule(original);
            writer.Close();
            string output = stringWriter.ToString();
            PDBReader reader = new PDBReader(new StringReader(output));
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();
            IAtomContainer reconstructed = chemFile[0][0].MoleculeSet[0];
            Assert.AreEqual(original.Atoms.Count, reconstructed.Atoms.Count);
            Assert.AreEqual(original.Bonds.Count, reconstructed.Bonds.Count);
        }
    }
}
