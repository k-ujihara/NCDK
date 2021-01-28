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
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NCDK.Aromaticities;
using NCDK.Formula;
using NCDK.Numerics;
using NCDK.Templates;
using NCDK.Tools.Manipulator;
using System.Diagnostics;
using System.IO;

namespace NCDK.IO.CML
{
    /// <summary>
    /// TestCase for the reading CML 2 files using a few test files in data/cmltest.
    /// </summary>
    // @cdk.module test-libiocml
    [TestClass()]
    public class CML2WriterTest : CDKTestCase
    {
        private readonly IChemObjectBuilder builder = CDK.Builder;

        [TestMethod()]
        public void TestCMLWriterBenzene()
        {
            var molecule = TestMoleculeFactory.MakeBenzene();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            Aromaticity.CDKLegacy.Apply(molecule);
            StringWriter writer;
            using (var cmlWriter = new CMLWriter(writer = new StringWriter()))
            {
                cmlWriter.Write(molecule);
            }
            Debug.WriteLine("****************************** TestCMLWriterBenzene()");
            Debug.WriteLine(writer.ToString());
            Debug.WriteLine("******************************");
            Assert.IsTrue(writer.ToString().IndexOf("</molecule>") != -1);
        }

        /// <summary>
        /// Test example with one explicit carbon, and four implicit hydrogens.
        /// </summary>
        // @cdk.bug 1655045
        [TestMethod()]
        public void TestHydrogenCount()
        {
            var molecule = builder.NewAtomContainer(); // methane
            molecule.Atoms.Add(molecule.Builder.NewAtom(ChemicalElement.C));
            molecule.Atoms[0].ImplicitHydrogenCount = 4;
            StringWriter writer;
            using (var cmlWriter = new CMLWriter(writer = new StringWriter()))
            {
                cmlWriter.Write(molecule);
            }
            Debug.WriteLine("****************************** TestHydrogenCount()");
            Debug.WriteLine(writer.ToString());
            Debug.WriteLine("******************************");
            Assert.IsTrue(writer.ToString().IndexOf("hydrogenCount=\"4\"") != -1);
        }

        [TestMethod()]
        public void TestNullFormalCharge()
        {
            StringWriter writer;
            using (writer = new StringWriter())
            {
                var molecule = builder.NewAtomContainer(); // methane
                molecule.Atoms.Add(molecule.Builder.NewAtom(ChemicalElement.C));
                molecule.Atoms[0].FormalCharge = null;
                var cmlWriter = new CMLWriter(writer);
                cmlWriter.Write(molecule);
            }
            Debug.WriteLine("****************************** TestNullFormalCharge()");
            Debug.WriteLine(writer.ToString());
            Debug.WriteLine("******************************");
            Assert.IsFalse(writer.ToString().Contains("formalCharge"));
        }

        /// <summary>
        /// Test example with one explicit carbon, writing of MassNumber
        /// </summary>
        [TestMethod()]
        public void TestMassNumber()
        {
            StringWriter writer;
            using (writer = new StringWriter())
            { 
                var mol = builder.NewAtomContainer();
                var atom = builder.NewAtom("C");
                atom.MassNumber = 12;
                mol.Atoms.Add(atom);
                var cmlWriter = new CMLWriter(writer);
                cmlWriter.Write(mol);
            }
            Debug.WriteLine("****************************** TestMAssNumber()");
            Debug.WriteLine(writer.ToString());
            Debug.WriteLine("******************************");
            Assert.IsTrue(writer.ToString().IndexOf("isotopeNumber=\"12\"") != -1);
        }

        /// <summary>
        /// Test example with one explicit carbon, and one implicit hydrogen, and three implicit hydrogens.
        /// </summary>
        // @cdk.bug 1655045
        [TestMethod()]
        public void TestHydrogenCount_2()
        {
            var molecule = builder.NewAtomContainer(); // methane
            molecule.Atoms.Add(molecule.Builder.NewAtom(ChemicalElement.C));
            molecule.Atoms.Add(molecule.Builder.NewAtom(ChemicalElement.H));
            molecule.Atoms[0].ImplicitHydrogenCount = 3;
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            StringWriter writer;
            using (var cmlWriter = new CMLWriter((writer = new StringWriter())))
            {
                cmlWriter.Write(molecule);
            }
            Debug.WriteLine("****************************** TestHydrogenCount_2()");
            Debug.WriteLine(writer.ToString());
            Debug.WriteLine("******************************");
            Assert.IsTrue(writer.ToString().IndexOf("hydrogenCount=\"4\"") != -1);
        }

        [TestMethod()]
        public void TestCMLCrystal()
        {
            var crystal = builder.NewCrystal();
            var silicon = builder.NewAtom("Si");
            silicon.FractionalPoint3D = new Vector3(0, 0, 0);
            crystal.Atoms.Add(silicon);
            crystal.A = (new Vector3(1.5, 0.0, 0.0));
            crystal.B = (new Vector3(0.0, 2.0, 0.0));
            crystal.C = (new Vector3(0.0, 0.0, 1.5));
            StringWriter writer;
            using (var cmlWriter = new CMLWriter(writer = new StringWriter()))
            {
                cmlWriter.Write(crystal);
            }
            var cmlContent = writer.ToString();
            Debug.WriteLine("****************************** TestCMLCrystal()");
            Debug.WriteLine(cmlContent);
            Debug.WriteLine("******************************");
            Assert.IsTrue(cmlContent.IndexOf("</crystal>") != -1); // the cystal info has to be present
            Assert.IsTrue(cmlContent.IndexOf("<atom") != -1); // an Atom has to be present
        }

        [TestMethod()]
        public void TestReactionCustomization()
        {
            var reaction = builder.NewReaction();
            reaction.Id = "reaction1";
            var reactant = reaction.Builder.NewAtomContainer();
            reactant.Id = "react";
            var product = reaction.Builder.NewAtomContainer();
            product.Id = "product";
            var agent = reaction.Builder.NewAtomContainer();
            agent.Id = "agent";

            reaction.Reactants.Add(reactant);
            reaction.Products.Add(product);
            reaction.Agents.Add(agent);

            StringWriter writer;
            using (var cmlWriter = new CMLWriter(writer = new StringWriter()))
            {
                cmlWriter.Write(reaction);
            }
            var cmlContent = writer.ToString();
            Debug.WriteLine("****************************** TestReactionCustomization()");
            Debug.WriteLine(cmlContent);
            Debug.WriteLine("******************************");
            Assert.IsTrue(cmlContent.IndexOf("<reaction id=\"reaction1") != -1);
            Assert.IsTrue(cmlContent.IndexOf("<molecule id=\"react") != -1);
            Assert.IsTrue(cmlContent.IndexOf("<molecule id=\"product") != -1);
            Assert.IsTrue(cmlContent.IndexOf("<molecule id=\"agent") != -1);
        }

        [TestMethod()]
        public void TestReactionScheme1()
        {
            var scheme1 = builder.NewReactionScheme();
            scheme1.Id = "rs0";
            var scheme2 = scheme1.Builder.NewReactionScheme();
            scheme2.Id = "rs1";
            scheme1.Add(scheme2);

            var reaction = scheme1.Builder.NewReaction();
            reaction.Id = "r1";
            var moleculeA = reaction.Builder.NewAtomContainer();
            moleculeA.Id = "A";
            var moleculeB = reaction.Builder.NewAtomContainer();
            moleculeB.Id = "B";
            reaction.Reactants.Add(moleculeA);
            reaction.Products.Add(moleculeB);

            scheme2.Add(reaction);

            var reaction2 = reaction.Builder.NewReaction();
            reaction2.Id = "r2";
            var moleculeC = reaction.Builder.NewAtomContainer();
            moleculeC.Id = "C";
            reaction2.Reactants.Add(moleculeB);
            reaction2.Products.Add(moleculeC);

            scheme1.Add(reaction2);

            StringWriter writer;
            using (var cmlWriter = new CMLWriter(writer = new StringWriter()))
            {
                cmlWriter.Write(scheme1);
            }
            var cmlContent = writer.ToString();
            Debug.WriteLine("****************************** TestReactionCustomization()");
            Debug.WriteLine(cmlContent);
            Debug.WriteLine("******************************");
            Assert.IsTrue(cmlContent.IndexOf("<reactionScheme id=\"rs0") != -1);
            Assert.IsTrue(cmlContent.IndexOf("<reactionScheme id=\"rs1") != -1);
            Assert.IsTrue(cmlContent.IndexOf("<reaction id=\"r1") != -1);
            Assert.IsTrue(cmlContent.IndexOf("<reaction id=\"r2") != -1);
            Assert.IsTrue(cmlContent.IndexOf("<molecule id=\"A") != -1);
            Assert.IsTrue(cmlContent.IndexOf("<molecule id=\"B") != -1);
            Assert.IsTrue(cmlContent.IndexOf("<molecule id=\"C") != -1);
        }

        [TestMethod()]
        public void TestReactionScheme2()
        {
            var scheme1 = builder.NewReactionScheme();
            scheme1.Id = "rs0";

            var reaction = builder.NewReaction();
            reaction.Id = "r1";
            var moleculeA = reaction.Builder.NewAtomContainer();
            moleculeA.Id = "A";
            var moleculeB = reaction.Builder.NewAtomContainer();
            moleculeB.Id = "B";
            reaction.Reactants.Add(moleculeA);
            reaction.Products.Add(moleculeB);

            scheme1.Add(reaction);

            var reaction2 = reaction.Builder.NewReaction();
            reaction2.Id = "r2";
            var moleculeC = reaction.Builder.NewAtomContainer();
            moleculeC.Id = "C";
            reaction2.Reactants.Add(moleculeB);
            reaction2.Products.Add(moleculeC);

            scheme1.Add(reaction2);

            StringWriter writer;
            using (var cmlWriter = new CMLWriter(writer = new StringWriter()))
            {
                cmlWriter.Write(scheme1);
            }
            var cmlContent = writer.ToString();
            Debug.WriteLine("****************************** TestReactionCustomization()");
            Debug.WriteLine(cmlContent);
            Debug.WriteLine("******************************");
            Assert.IsTrue(cmlContent.IndexOf("<reactionScheme id=\"rs0") != -1);
            Assert.IsTrue(cmlContent.IndexOf("<reaction id=\"r1") != -1);
            Assert.IsTrue(cmlContent.IndexOf("<reaction id=\"r2") != -1);
            Assert.IsTrue(cmlContent.IndexOf("<molecule id=\"A") != -1);
            Assert.IsTrue(cmlContent.IndexOf("<molecule id=\"B") != -1);
            Assert.IsTrue(cmlContent.IndexOf("<molecule id=\"C") != -1);
        }

        [TestMethod()]
        public void TestReactionSchemeWithFormula()
        {
            var scheme1 = builder.NewReactionScheme();
            scheme1.Id = "rs0";

            var reaction = builder.NewReaction();
            reaction.Id = "r1";
            var moleculeA = reaction.Builder.NewAtomContainer();
            moleculeA.Id = "A";
            var formula = new MolecularFormula();
            formula.Add(reaction.Builder.NewIsotope("C"), 10);
            formula.Add(reaction.Builder.NewIsotope("H"), 15);
            formula.Add(reaction.Builder.NewIsotope("N"), 2);
            formula.Add(reaction.Builder.NewIsotope("O"), 1);
            moleculeA.SetProperty(CDKPropertyName.Formula, formula);
            var moleculeB = reaction.Builder.NewAtomContainer();
            moleculeB.Id = "B";
            reaction.Reactants.Add(moleculeA);
            reaction.Products.Add(moleculeB);

            scheme1.Add(reaction);

            var reaction2 = reaction.Builder.NewReaction();
            reaction2.Id = "r2";
            var moleculeC = reaction.Builder.NewAtomContainer();
            moleculeC.Id = "C";
            reaction2.Reactants.Add(moleculeB);
            reaction2.Products.Add(moleculeC);

            scheme1.Add(reaction2);

            StringWriter writer;
            using (var cmlWriter = new CMLWriter(writer = new StringWriter()))
            {
                cmlWriter.Write(scheme1);
            }
            var cmlContent = writer.ToString();
            Debug.WriteLine("****************************** TestReactionCustomization()");
            Debug.WriteLine(cmlContent);
            Debug.WriteLine("******************************");
            Assert.IsTrue(cmlContent.IndexOf("<reactionScheme id=\"rs0") != -1);
            Assert.IsTrue(cmlContent.IndexOf("<reaction id=\"r1") != -1);
            Assert.IsTrue(cmlContent.IndexOf("<reaction id=\"r2") != -1);
            Assert.IsTrue(cmlContent.IndexOf("<molecule id=\"A") != -1);
            Assert.IsTrue(cmlContent.IndexOf("<formula concise=") != -1);
            Assert.IsTrue(cmlContent.IndexOf("<molecule id=\"B") != -1);
            Assert.IsTrue(cmlContent.IndexOf("<molecule id=\"C") != -1);
        }

        [TestMethod()]
        public void TestReactionSchemeWithFormula2()
        {
            var scheme1 = builder.NewReactionScheme();
            scheme1.Id = "rs0";

            var reaction = builder.NewReaction();
            reaction.Id = "r1";
            var moleculeA = reaction.Builder.NewAtomContainer();
            moleculeA.Id = "A";
            moleculeA.SetProperty(CDKPropertyName.Formula, "C 10 H 15 N 2 O 1");
            var moleculeB = reaction.Builder.NewAtomContainer();
            moleculeB.Id = "B";
            reaction.Reactants.Add(moleculeA);
            reaction.Products.Add(moleculeB);

            scheme1.Add(reaction);

            var reaction2 = reaction.Builder.NewReaction();
            reaction2.Id = "r2";
            var moleculeC = reaction.Builder.NewAtomContainer();
            moleculeC.Id = "C";
            reaction2.Reactants.Add(moleculeB);
            reaction2.Products.Add(moleculeC);

            scheme1.Add(reaction2);

            StringWriter writer;
            using (var cmlWriter = new CMLWriter(writer = new StringWriter()))
            {
                cmlWriter.Write(scheme1);
            }
            var cmlContent = writer.ToString();
            Debug.WriteLine("****************************** TestReactionCustomization()");
            Debug.WriteLine(cmlContent);
            Debug.WriteLine("******************************");
            Assert.IsTrue(cmlContent.IndexOf("<reactionScheme id=\"rs0") != -1);
            Assert.IsTrue(cmlContent.IndexOf("<reaction id=\"r1") != -1);
            Assert.IsTrue(cmlContent.IndexOf("<reaction id=\"r2") != -1);
            Assert.IsTrue(cmlContent.IndexOf("<molecule id=\"A") != -1);
            Assert.IsTrue(cmlContent.IndexOf("<scalar dictRef=\"cdk:molecularProperty") != -1);
            Assert.IsTrue(cmlContent.IndexOf("<molecule id=\"B") != -1);
            Assert.IsTrue(cmlContent.IndexOf("<molecule id=\"C") != -1);
        }

        [TestMethod()]
        public void TestChemModeID()
        {
            var chemModel = builder.NewChemModel();
            chemModel.Id = "cm0";

            StringWriter writer;
            using (var cmlWriter = new CMLWriter(writer = new StringWriter()))
            {
                cmlWriter.Write(chemModel);
            }
            var cmlContent = writer.ToString();
            Debug.WriteLine("****************************** TestReactionCustomization()");
            Debug.WriteLine(cmlContent);
            Debug.WriteLine("******************************");
            Assert.IsTrue(cmlContent.IndexOf("<list convention=\"cdk:model\" id=\"cm0") != -1);
        }

        [TestMethod()]
        public void TestMoleculeSetID()
        {
            var moleculeSet = builder.NewChemObjectSet<IAtomContainer>();
            moleculeSet.Id = "ms0";

            StringWriter writer;
            using (var cmlWriter = new CMLWriter(writer = new StringWriter()))
            {
                cmlWriter.Write(moleculeSet);
            }
            var cmlContent = writer.ToString();
            Debug.WriteLine("****************************** TestReactionCustomization()");
            Debug.WriteLine(cmlContent);
            Debug.WriteLine("******************************");
            Assert.IsTrue(cmlContent.IndexOf("<moleculeList convention=\"cdk:moleculeSet\" id=\"ms0") != -1);
        }

        [TestMethod()]
        public void TestReactionProperty()
        {
            var reaction = builder.NewReaction();
            reaction.Id = "r1";
            reaction.SetProperty("blabla", "blabla2");
            StringWriter writer;
            using (var cmlWriter = new CMLWriter(writer = new StringWriter()))
            {
                cmlWriter.Write(reaction);
            }
            var cmlContent = writer.ToString();
            Debug.WriteLine("****************************** TestReactionCustomization()");
            Debug.WriteLine(cmlContent);
            Debug.WriteLine("******************************");
            Assert.IsTrue(cmlContent.IndexOf("<scalar dictRef=\"cdk:reactionProperty") != -1);
        }

        [TestMethod()]
        public void WriteIsClosed()
        {
            var mock = new Mock<TextWriter>();
            new CMLWriter(mock.Object).Close();
            mock.Verify(n => n.Close());
        }
    }
}
