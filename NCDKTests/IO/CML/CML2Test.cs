/* Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.Common.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.Geometries;
using NCDK.Tools.Manipulator;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NCDK.IO.CML
{
    /// <summary>
    /// TestCase for the reading CML 2 files using a few test files in data/cmltest.
    /// </summary>
    // @cdk.module test-libiocml
    // @cdk.require java1.5+
    [TestClass()]
    public class CML2Test : CDKTestCase
    {
        [TestMethod()]
        public void TestFile3()
        {
            string filename = "NCDK.Data.CML.3.cml";
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            IAtomContainer mol = ChemFileManipulator.GetAllAtomContainers(chemFile).First();

            for (int i = 0; i <= 3; i++)
            {
                Assert.IsFalse(mol.Bonds[i].IsAromatic, "Bond " + (i + 1) + " is not aromatic in the file");
            }
            for (int i = 4; i <= 9; i++)
            {
                Assert.IsTrue(mol.Bonds[i].IsAromatic, "Bond " + (i + 1) + " is aromatic in the file");
            }
        }

        /**
         * @cdk.bug 2114987
         */
        [TestMethod()]
        public void TestCMLTestCase()
        {
            string filename = "NCDK.Data.CML.olaCmlAtomType.cml";
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = new ChemFile();
            chemFile = (IChemFile)reader.Read(chemFile);
            reader.Close();
            IAtomContainer container = ChemFileManipulator.GetAllAtomContainers(chemFile).First();
            foreach (var atom in container.Atoms)
            {
                Assert.AreEqual(null, atom.ImplicitHydrogenCount);
            }
        }

        [TestMethod()]
        public void TestCOONa()
        {
            string filename = "NCDK.Data.CML.COONa.cml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(chemFile.Count, 1);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(seq.Count, 1);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);
            Assert.AreEqual(model.MoleculeSet.Count(), 1);

            // test the molecule
            IAtomContainer mol = model.MoleculeSet[0];
            Assert.IsNotNull(mol);
            Assert.AreEqual(4, mol.Atoms.Count);
            Assert.AreEqual(2, mol.Bonds.Count);
            Assert.IsTrue(GeometryUtil.Has3DCoordinates(mol));
            Assert.IsTrue(!GeometryUtil.Has2DCoordinates(mol));

            foreach (var atom in mol.Atoms)
            {
                if (atom.Symbol.Equals("Na")) Assert.AreEqual(+1, atom.FormalCharge.Value);
            }
        }

        [TestMethod()]
        public void TestNitrate()
        {
            string filename = "NCDK.Data.CML.nitrate.cml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(chemFile.Count, 1);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(seq.Count, 1);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);
            Assert.AreEqual(model.MoleculeSet.Count(), 1);

            // test the molecule
            IAtomContainer mol = model.MoleculeSet[0];
            Assert.IsNotNull(mol);
            Assert.AreEqual(4, mol.Atoms.Count);
            Assert.AreEqual(3, mol.Bonds.Count);
            Assert.IsTrue(GeometryUtil.Has3DCoordinates(mol));
            Assert.IsTrue(!GeometryUtil.Has2DCoordinates(mol));

            foreach (var atom in mol.Atoms)
            {
                if (atom.Symbol.Equals("N")) Assert.AreEqual(+1, atom.FormalCharge.Value);
            }
        }

        [TestMethod()]
        public void TestCMLOK1()
        {
            string filename = "NCDK.Data.CML.cs2a.cml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(chemFile.Count, 1);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(seq.Count, 1);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);
            Assert.AreEqual(model.MoleculeSet.Count(), 1);

            // test the molecule
            IAtomContainer mol = model.MoleculeSet[0];
            Assert.IsNotNull(mol);
            Assert.AreEqual(38, mol.Atoms.Count);
            Assert.AreEqual(48, mol.Bonds.Count);
            Assert.IsTrue(GeometryUtil.Has3DCoordinates(mol));
            Assert.IsFalse(GeometryUtil.Has2DCoordinates(mol));
        }

        [TestMethod()]
        public void TestCMLOK2()
        {
            string filename = "NCDK.Data.CML.cs2a.mol.cml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(chemFile.Count, 1);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(seq.Count, 1);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);
            Assert.AreEqual(model.MoleculeSet.Count(), 1);

            // test the molecule
            IAtomContainer mol = model.MoleculeSet[0];
            Assert.IsNotNull(mol);
            Assert.AreEqual(38, mol.Atoms.Count);
            Assert.AreEqual(29, mol.Bonds.Count);
            Assert.IsTrue(GeometryUtil.Has3DCoordinates(mol));
            Assert.IsFalse(GeometryUtil.Has2DCoordinates(mol));
        }

        [TestMethod()]
        public void TestCMLOK3()
        {
            string filename = "NCDK.Data.CML.nsc2dmol.1.cml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(chemFile.Count, 1);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(seq.Count, 1);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);
            Assert.AreEqual(model.MoleculeSet.Count(), 1);

            // test the molecule
            IAtomContainer mol = model.MoleculeSet[0];
            Assert.IsNotNull(mol);
            Assert.AreEqual(13, mol.Atoms.Count);
            Assert.AreEqual(12, mol.Bonds.Count);
            Assert.IsFalse(GeometryUtil.Has3DCoordinates(mol));
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(mol));
        }

        [TestMethod()]
        public void TestCMLOK4()
        {
            string filename = "NCDK.Data.CML.nsc2dmol.2.cml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(chemFile.Count, 1);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(seq.Count, 1);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);
            Assert.AreEqual(model.MoleculeSet.Count(), 1);

            // test the molecule
            IAtomContainer mol = model.MoleculeSet[0];
            Assert.IsNotNull(mol);
            Assert.AreEqual(13, mol.Atoms.Count);
            Assert.AreEqual(12, mol.Bonds.Count);
            Assert.IsFalse(GeometryUtil.Has3DCoordinates(mol));
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(mol));
        }

        [TestMethod()]
        public void TestCMLOK5()
        {
            string filename = "NCDK.Data.CML.nsc2dmol.a1.cml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(chemFile.Count, 1);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(seq.Count, 1);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);
            Assert.AreEqual(model.MoleculeSet.Count(), 1);

            // test the molecule
            IAtomContainer mol = model.MoleculeSet[0];
            Assert.IsNotNull(mol);
            Assert.AreEqual(13, mol.Atoms.Count);
            Assert.AreEqual(12, mol.Bonds.Count);
            Assert.IsFalse(GeometryUtil.Has3DCoordinates(mol));
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(mol));
        }

        [TestMethod()]
        public void TestCMLOK6()
        {
            string filename = "NCDK.Data.CML.nsc2dmol.a2.cml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(chemFile.Count, 1);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(seq.Count, 1);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);
            Assert.AreEqual(model.MoleculeSet.Count(), 1);

            // test the molecule
            IAtomContainer mol = model.MoleculeSet[0];
            Assert.IsNotNull(mol);
            Assert.AreEqual(13, mol.Atoms.Count);
            Assert.AreEqual(12, mol.Bonds.Count);
            Assert.IsFalse(GeometryUtil.Has3DCoordinates(mol));
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(mol));
        }

        [TestMethod()]
        public void TestCMLOK7()
        {
            string filename = "NCDK.Data.CML.nsc3dcml.xml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(chemFile.Count, 1);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(seq.Count, 1);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);
            Assert.AreEqual(model.MoleculeSet.Count(), 1);

            // test the molecule
            IAtomContainer mol = model.MoleculeSet[0];
            Assert.IsNotNull(mol);
            Assert.AreEqual(27, mol.Atoms.Count);
            Assert.AreEqual(27, mol.Bonds.Count);
            Assert.IsTrue(GeometryUtil.Has3DCoordinates(mol));
            Assert.IsFalse(GeometryUtil.Has2DCoordinates(mol));
        }

        [TestMethod()]
        public void TestCMLOK8()
        {
            string filename = "NCDK.Data.CML.nsc2dcml.xml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(chemFile.Count, 1);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(seq.Count, 1);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);
            Assert.AreEqual(model.MoleculeSet.Count(), 1);

            // test the molecule
            IAtomContainer mol = model.MoleculeSet[0];
            Assert.IsNotNull(mol);
            Assert.AreEqual(15, mol.Atoms.Count);
            Assert.AreEqual(14, mol.Bonds.Count);
            Assert.IsFalse(GeometryUtil.Has3DCoordinates(mol));
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(mol));
        }

        [TestMethod()]
        public void TestCMLOK9()
        {
            string filename = "NCDK.Data.CML.nsc3dmol.1.cml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(chemFile.Count, 1);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(seq.Count, 1);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);
            Assert.AreEqual(model.MoleculeSet.Count(), 1);

            // test the molecule
            IAtomContainer mol = model.MoleculeSet[0];
            Assert.IsNotNull(mol);
            Assert.AreEqual(15, mol.Atoms.Count);
            Assert.AreEqual(15, mol.Bonds.Count);
            Assert.IsTrue(GeometryUtil.Has3DCoordinates(mol));
            Assert.IsFalse(GeometryUtil.Has2DCoordinates(mol));
        }

        [TestMethod()]
        public void TestCMLOK10()
        {
            string filename = "NCDK.Data.CML.nsc3dmol.2.cml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(chemFile.Count, 1);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(seq.Count, 1);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);
            Assert.AreEqual(model.MoleculeSet.Count(), 1);

            // test the molecule
            IAtomContainer mol = model.MoleculeSet[0];
            Assert.IsNotNull(mol);
            Assert.AreEqual(15, mol.Atoms.Count);
            Assert.AreEqual(15, mol.Bonds.Count);
            Assert.IsTrue(GeometryUtil.Has3DCoordinates(mol));
            Assert.IsFalse(GeometryUtil.Has2DCoordinates(mol));
        }

        [TestMethod()]
        public void TestCMLOK11()
        {
            string filename = "NCDK.Data.CML.nsc3dmol.a1.cml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(chemFile.Count, 1);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(seq.Count, 1);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);
            Assert.AreEqual(model.MoleculeSet.Count(), 1);

            // test the molecule
            IAtomContainer mol = model.MoleculeSet[0];
            Assert.IsNotNull(mol);
            Assert.AreEqual(15, mol.Atoms.Count);
            Assert.AreEqual(15, mol.Bonds.Count);
            Assert.IsTrue(GeometryUtil.Has3DCoordinates(mol));
            Assert.IsFalse(GeometryUtil.Has2DCoordinates(mol));
        }

        [TestMethod()]
        public void TestCMLOK12()
        {
            string filename = "NCDK.Data.CML.nsc3dmol.a2.cml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(chemFile.Count, 1);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(seq.Count, 1);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);
            Assert.AreEqual(model.MoleculeSet.Count(), 1);

            // test the molecule
            IAtomContainer mol = model.MoleculeSet[0];
            Assert.IsNotNull(mol);
            Assert.AreEqual(15, mol.Atoms.Count);
            Assert.AreEqual(15, mol.Bonds.Count);
            Assert.IsTrue(GeometryUtil.Has3DCoordinates(mol));
            Assert.IsFalse(GeometryUtil.Has2DCoordinates(mol));
        }

        /**
         * This test tests whether the CMLReader is able to ignore the CMLSpect part
         * of a CML file, while extracting the molecule.
         */
        [TestMethod()]
        public void TestCMLSpectMolExtraction()
        {
            string filename = "NCDK.Data.CML.molAndspect.cml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(chemFile.Count, 1);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(seq.Count, 1);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);
            Assert.AreEqual(model.MoleculeSet.Count(), 1);

            // test the molecule
            IAtomContainer mol = model.MoleculeSet[0];
            Assert.IsNotNull(mol);
            Assert.AreEqual(17, mol.Atoms.Count);
            Assert.AreEqual(18, mol.Bonds.Count);
            Assert.IsFalse(GeometryUtil.Has3DCoordinates(mol));
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(mol));
        }

        /**
         * This test tests whether the CMLReader is able to ignore the CMLReaction part
         * of a CML file, while extracting the reaction.
         */
        [TestMethod()]
        public void TestCMLReaction()
        {
            string filename = "NCDK.Data.CML.reaction.2.cml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(chemFile.Count, 1);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(seq.Count, 1);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);
            Assert.AreEqual(model.ReactionSet.Count, 1);

            // test the reaction
            IReaction reaction = model.ReactionSet[0];
            Assert.IsNotNull(reaction);
            Assert.AreEqual("react", reaction.Reactants[0].Id);
            Assert.AreEqual("product", reaction.Products[0].Id);
            Assert.AreEqual("a14293164", reaction.Reactants[0].Atoms[0].Id);
            Assert.AreEqual(6, reaction.Products[0].Atoms.Count);
            Assert.AreEqual(6, reaction.Reactants[0].Atoms.Count);
        }

        /**
         * This test tests whether the CMLReader is able to ignore the CMLReaction part
         * of a CML file, while extracting the reaction.
         */
        [TestMethod()]
        public void TestCMLReactionWithAgents()
        {
            string filename = "NCDK.Data.CML.reaction.1.cml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(chemFile.Count, 1);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(seq.Count, 1);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);
            Assert.AreEqual(model.ReactionSet.Count, 1);

            // test the reaction
            IReaction reaction = model.ReactionSet[0];
            Assert.IsNotNull(reaction);
            Assert.AreEqual("react", reaction.Reactants[0].Id);
            Assert.AreEqual("product", reaction.Products[0].Id);
            Assert.AreEqual("water", reaction.Agents[0].Id);
            Assert.AreEqual("H+", reaction.Agents[1].Id);
            Assert.AreEqual(6, reaction.Products[0].Atoms.Count);
            Assert.AreEqual(6, reaction.Reactants[0].Atoms.Count);
        }

        /**
         * This test tests whether the CMLReader is able to ignore the CMLReaction part
         * of a CML file, while extracting the reaction.
         */
        [TestMethod()]
        public void TestCMLReactionList()
        {
            string filename = "NCDK.Data.CML.reactionList.1.cml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(chemFile.Count, 1);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(1, seq.Count);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);
            Assert.AreEqual(2, model.ReactionSet.Count);
            Assert.AreEqual("1.3.2", model.ReactionSet[0].Id);

            // test the reaction
            IReaction reaction = model.ReactionSet[0];
            Assert.IsNotNull(reaction);
            Assert.AreEqual("actey", reaction.Reactants[0].Id);
            Assert.AreEqual("a14293164", reaction.Reactants[0].Atoms[0].Id);
            Assert.AreEqual(6, reaction.Products[0].Atoms.Count);
            Assert.AreEqual(6, reaction.Reactants[0].Atoms.Count);
        }

        /**
         * @cdk.bug 1560486
         */
        [TestMethod()]
        public void TestCMLWithFormula()
        {
            string filename = "NCDK.Data.CML.cmlWithFormula.cml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(chemFile.Count, 1);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(seq.Count, 1);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);

            IAtomContainer mol = model.MoleculeSet[0];
            Assert.IsNotNull(mol);
            Assert.AreEqual("a", mol.Id);
            Assert.AreEqual("a1", mol.Atoms[0].Id);
            Assert.AreEqual(27, mol.Atoms.Count);
            Assert.AreEqual(32, mol.Bonds.Count);
        }

        /**
         * Only Molecule with concise MolecularFormula
         */
        [TestMethod()]
        public void TestCMLConciseFormula()
        {
            string filename = "NCDK.Data.CML.cmlConciseFormula.cml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(chemFile.Count, 1);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(seq.Count, 1);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);

            IAtomContainer mol = model.MoleculeSet[0];
            Assert.IsNotNull(mol);

            // FIXME: REACT: It should return two different formulas
            Assert.AreEqual("[C 18 H 21 Cl 2 Mn 1 N 5 O 1]",
				Strings.ToJavaString(mol.GetProperty<IList<string>>(CDKPropertyName.FORMULA)));
        }

        /**
         * Only Molecule with concise MolecularFormula
         */
        [TestMethod()]
        public void TestCMLConciseFormula2()
        {
            string filename = "NCDK.Data.CML.cmlConciseFormula2.cml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(chemFile.Count, 1);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(seq.Count, 1);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);

            IAtomContainer mol = model.MoleculeSet[0];
            Assert.IsNotNull(mol);

            // FIXME: REACT: It should return two different formulas
            Assert.AreEqual("[C 18 H 21 Cl 2 Mn 1 N 5 O 1, C 4 H 10]", 
				Strings.ToJavaString(mol.GetProperty<IList<string>>(CDKPropertyName.FORMULA)));
        }

        /**
         * This test tests whether the CMLReader is able to ignore the CMLReaction part
         * of a CML file, while extracting the reaction.
         */
        [TestMethod()]
        public void TestCMLScheme1()
        {
            string filename = "NCDK.Data.CML.reactionScheme.1.cml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(chemFile.Count, 1);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(1, seq.Count);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);

            // test reaction
            Assert.AreEqual(4, model.ReactionSet.Count);
            string[] idReaction = { "r1", "r2", "r3", "r4" };
            string[] idReactants = { "A", "B", "A", "F" };
            string[] idProducts = { "B", "C", "F", "G" };
            for (int i = 0; i < idReaction.Length; i++)
            {
                IReaction reaction = model.ReactionSet[i];
                Assert.AreEqual(idReaction[i], reaction.Id);
                // test molecule
                Assert.AreEqual(1, reaction.Products.Count());
                Assert.AreEqual(idProducts[i], reaction.Products[0].Id);

                Assert.AreEqual(1, reaction.Reactants.Count());
                Assert.AreEqual(idReactants[i], reaction.Reactants[0].Id);
            }
        }

        /**
         * This test tests whether the CMLReader is able to ignore the CMLReaction part
         * of a CML file, while extracting the reaction.
         */
        [TestMethod()]
        public void TestCMLScheme2()
        {
            string filename = "NCDK.Data.CML.reactionScheme.2.cml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(chemFile.Count, 1);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(1, seq.Count);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);

            // test reaction
            Assert.AreEqual(2, model.ReactionSet.Count);
            string[] idReaction = { "r1", "r2" };
            string[] idReactants = { "A", "B" };
            string[] idProducts = { "B", "C" };
            for (int i = 0; i < idReaction.Length; i++)
            {
                IReaction reaction = model.ReactionSet[i];
                Assert.AreEqual(idReaction[i], reaction.Id);
                // test molecule
                Assert.AreEqual(1, reaction.Products.Count());
                Assert.AreEqual(idProducts[i], reaction.Products[0].Id);

                Assert.AreEqual(1, reaction.Reactants.Count());
                Assert.AreEqual(idReactants[i], reaction.Reactants[0].Id);
            }
        }

        /**
         * This test tests whether the CMLReader is able to ignore the CMLReaction part
         * of a CML file, while extracting the reaction.
         */
        [TestMethod()]
        public void TestCMLSchemeStepList1()
        {
            string filename = "NCDK.Data.CML.reactionSchemeStepList.1.cml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(chemFile.Count, 1);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(1, seq.Count);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);

            // test reaction
            Assert.AreEqual(4, model.ReactionSet.Count);
            string[] idReaction = { "r1.1", "r1.2", "r2.1", "r2.2" };
            string[] idReactants = { "A", "B", "A", "D" };
            string[] idProducts = { "B", "C", "D", "E" };
            for (int i = 0; i < idReaction.Length; i++)
            {
                IReaction reaction = model.ReactionSet[i];
                Assert.AreEqual(idReaction[i], reaction.Id);
                // test molecule
                Assert.AreEqual(1, reaction.Products.Count());
                Assert.AreEqual(idProducts[i], reaction.Products[0].Id);

                Assert.AreEqual(1, reaction.Reactants.Count());
                Assert.AreEqual(idReactants[i], reaction.Reactants[0].Id);
            }

        }

        /**
         * This test tests whether the CMLReader is able to ignore the CMLReaction part
         * of a CML file, while extracting the reaction.
         */
        [TestMethod()]
        public void TestCMLStepList()
        {
            string filename = "NCDK.Data.CML.reactionStepList.1.cml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(chemFile.Count, 1);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(1, seq.Count);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);

            // test reaction
            Assert.AreEqual(3, model.ReactionSet.Count);
            string[] idReaction = { "r1", "r2", "r3" };
            string[] idReactants = { "A", "B", "C" };
            string[] idProducts = { "B", "C", "D" };
            for (int i = 0; i < idReaction.Length; i++)
            {
                IReaction reaction = model.ReactionSet[i];
                Assert.AreEqual(idReaction[i], reaction.Id);
                // test molecule
                Assert.AreEqual(1, reaction.Products.Count());
                Assert.AreEqual(idProducts[i], reaction.Products[0].Id);

                Assert.AreEqual(1, reaction.Reactants.Count());
                Assert.AreEqual(idReactants[i], reaction.Reactants[0].Id);
            }

        }

        /**
         * This test tests whether the CMLReader is able to read a reactionscheme object with
         * references to list of molecules.
         */
        [TestMethod()]
        public void TestCMLSchemeMoleculeSet()
        {
            string filename = "NCDK.Data.CML.reactionSchemeMoleculeSet.cml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(chemFile.Count, 1);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(1, seq.Count);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);

            // test reaction
            Assert.AreEqual(1, model.ReactionSet.Count);
            string[] idReaction = { "react_1" };
            string[] idReactants = { "A" };
            string[] idProducts = { "B", "C" };

            IReaction reaction = model.ReactionSet[0];
            Assert.AreEqual(idReaction[0], reaction.Id);
            // test molecule
            Assert.AreEqual(2, reaction.Products.Count());
            Assert.AreEqual(idProducts[0], reaction.Products[0].Id);
            Assert.AreEqual("C 9 H 20 N 1",
                reaction.Products[0].GetProperty<IList<string>>(CDKPropertyName.FORMULA)[0]);
            Assert.AreEqual(idProducts[1], reaction.Products[1].Id);

            Assert.AreEqual(1, reaction.Reactants.Count());
            Assert.AreEqual(idReactants[0], reaction.Reactants[0].Id);
            Assert.AreEqual("C 28 H 60 N 1",
                reaction.Reactants[0].GetProperty<IList<string>>(CDKPropertyName.FORMULA)[0]);
        }

        /**
         * @cdk.bug 2697568
         */
        [TestMethod()]
        public void TestReadReactionWithPointersToMoleculeSet()
        {
            string filename = "NCDK.Data.CML.AlanineTree.cml";
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = new ChemFile();
            chemFile = (IChemFile)reader.Read(chemFile);
            reader.Close();
            Assert.AreSame(chemFile[0][0].MoleculeSet[0], chemFile
                    [0][0].ReactionSet[0].Reactants[0]);
        }

        /**
         * @cdk.bug 2697568
         */
        [TestMethod()]
        public void TestBug2697568()
        {
            string filename = "NCDK.Data.CML.AlanineTreeReverse.cml";
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = new ChemFile();
            chemFile = (IChemFile)reader.Read(chemFile);
            reader.Close();
            Assert.AreSame(chemFile[0][0].MoleculeSet[0], chemFile
                    [0][0].ReactionSet[0].Reactants[0]);
        }

        /**
         */
        [TestMethod()]
        public void TestReactionProperties()
        {
            string filename = "NCDK.Data.CML.reaction.2.cml";
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = new ChemFile();
            chemFile = (IChemFile)reader.Read(chemFile);
            reader.Close();
            IReaction reaction = chemFile[0][0].ReactionSet[0];

            Assert.AreEqual("3", reaction.GetProperty<string>("Ka"));
        }
    }
}
