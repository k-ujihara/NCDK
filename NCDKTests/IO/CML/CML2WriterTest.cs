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
using NCDK.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NCDK.Aromaticities;
using NCDK.Config;
using NCDK.Default;
using NCDK.Formula;
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
        [TestMethod()]
        public void TestCMLWriterBenzene()
        {
            StringWriter writer = new StringWriter();
            IAtomContainer molecule = TestMoleculeFactory.MakeBenzene();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            Aromaticity.CDKLegacy.Apply(molecule);
            CMLWriter cmlWriter = new CMLWriter(writer);

            cmlWriter.Write(molecule);
            cmlWriter.Close();
            Debug.WriteLine("****************************** TestCMLWriterBenzene()");
            Debug.WriteLine(writer.ToString());
            Debug.WriteLine("******************************");
            Assert.IsTrue(writer.ToString().IndexOf("</molecule>") != -1);
        }

        /// <summary>
        /// Test example with one explicit carbon, and four implicit hydrogens.
        ///
        // @cdk.bug 1655045
        /// </summary>
        [TestMethod()]
        public void TestHydrogenCount()
        {
            StringWriter writer = new StringWriter();
            IAtomContainer molecule = new AtomContainer(); // methane
            molecule.Atoms.Add(molecule.Builder.NewAtom(ChemicalElements.Carbon.ToIElement()));
            molecule.Atoms[0].ImplicitHydrogenCount = 4;
            CMLWriter cmlWriter = new CMLWriter(writer);

            cmlWriter.Write(molecule);
            cmlWriter.Close();
            Debug.WriteLine("****************************** TestHydrogenCount()");
            Debug.WriteLine(writer.ToString());
            Debug.WriteLine("******************************");
            Assert.IsTrue(writer.ToString().IndexOf("hydrogenCount=\"4\"") != -1);
        }

        [TestMethod()]
        public void TestNullFormalCharge()
        {
            StringWriter writer = new StringWriter();
            IAtomContainer molecule = new AtomContainer(); // methane
            molecule.Atoms.Add(molecule.Builder.NewAtom(ChemicalElements.Carbon.ToIElement()));
            molecule.Atoms[0].FormalCharge = null;
            CMLWriter cmlWriter = new CMLWriter(writer);

            cmlWriter.Write(molecule);
            cmlWriter.Close();
            Debug.WriteLine("****************************** TestNullFormalCharge()");
            Debug.WriteLine(writer.ToString());
            Debug.WriteLine("******************************");
            Assert.IsFalse(writer.ToString().Contains("formalCharge"));
        }

        /// <summary>
       /// Test example with one explicit carbon, writing of MassNumber
       ///
       /// </summary>
        [TestMethod()]
        public void TestMassNumber()
        {
            StringWriter writer = new StringWriter();
            IAtomContainer mol = new AtomContainer();
            Atom atom = new Atom("C");
            atom.MassNumber = 12;
            mol.Atoms.Add(atom);
            CMLWriter cmlWriter = new CMLWriter(writer);

            cmlWriter.Write(mol);
            cmlWriter.Close();
            Debug.WriteLine("****************************** TestMAssNumber()");
            Debug.WriteLine(writer.ToString());
            Debug.WriteLine("******************************");
            Assert.IsTrue(writer.ToString().IndexOf("isotopeNumber=\"12\"") != -1);
        }

        /// <summary>
        /// Test example with one explicit carbon, and one implicit hydrogen, and three implicit hydrogens.
        ///
        // @cdk.bug 1655045
        /// </summary>
        [TestMethod()]
        public void TestHydrogenCount_2()
        {
            StringWriter writer = new StringWriter();
            IAtomContainer molecule = new AtomContainer(); // methane
            molecule.Atoms.Add(molecule.Builder.NewAtom(ChemicalElements.Carbon.ToIElement()));
            molecule.Atoms.Add(molecule.Builder.NewAtom(ChemicalElements.Hydrogen.ToIElement()));
            molecule.Atoms[0].ImplicitHydrogenCount = 3;
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            CMLWriter cmlWriter = new CMLWriter(writer);

            cmlWriter.Write(molecule);
            cmlWriter.Close();
            Debug.WriteLine("****************************** TestHydrogenCount_2()");
            Debug.WriteLine(writer.ToString());
            Debug.WriteLine("******************************");
            Assert.IsTrue(writer.ToString().IndexOf("hydrogenCount=\"4\"") != -1);
        }

        [TestMethod()]
        public void TestCMLCrystal()
        {
            StringWriter writer = new StringWriter();
            ICrystal crystal = new Crystal();
            IAtom silicon = new Atom("Si");
            silicon.FractionalPoint3D = new Vector3(0, 0, 0);
            crystal.Atoms.Add(silicon);
            crystal.A = (new Vector3(1.5, 0.0, 0.0));
            crystal.B = (new Vector3(0.0, 2.0, 0.0));
            crystal.C = (new Vector3(0.0, 0.0, 1.5));
            CMLWriter cmlWriter = new CMLWriter(writer);

            cmlWriter.Write(crystal);
            cmlWriter.Close();
            string cmlContent = writer.ToString();
            Debug.WriteLine("****************************** TestCMLCrystal()");
            Debug.WriteLine(cmlContent);
            Debug.WriteLine("******************************");
            Assert.IsTrue(cmlContent.IndexOf("</crystal>") != -1); // the cystal info has to be present
            Assert.IsTrue(cmlContent.IndexOf("<atom") != -1); // an Atom has to be present
        }

        [TestMethod()]
        public void TestReactionCustomization()
        {
            StringWriter writer = new StringWriter();
            IReaction reaction = new Reaction();
            reaction.Id = "reaction1";
            IAtomContainer reactant = reaction.Builder.NewAtomContainer();
            reactant.Id = "react";
            IAtomContainer product = reaction.Builder.NewAtomContainer();
            product.Id = "product";
            IAtomContainer agent = reaction.Builder.NewAtomContainer();
            agent.Id = "agent";

            reaction.Reactants.Add(reactant);
            reaction.Products.Add(product);
            reaction.Agents.Add(agent);

            CMLWriter cmlWriter = new CMLWriter(writer);
            cmlWriter.Write(reaction);
            cmlWriter.Close();
            string cmlContent = writer.ToString();
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
            StringWriter writer = new StringWriter();
            IReactionScheme scheme1 = Default.ChemObjectBuilder.Instance.NewReactionScheme();
            scheme1.Id = "rs0";
            IReactionScheme scheme2 = scheme1.Builder.NewReactionScheme();
            scheme2.Id = "rs1";
            scheme1.Add(scheme2);

            IReaction reaction = scheme1.Builder.NewReaction();
            reaction.Id = "r1";
            IAtomContainer moleculeA = reaction.Builder.NewAtomContainer();
            moleculeA.Id = "A";
            IAtomContainer moleculeB = reaction.Builder.NewAtomContainer();
            moleculeB.Id = "B";
            reaction.Reactants.Add(moleculeA);
            reaction.Products.Add(moleculeB);

            scheme2.Add(reaction);

            IReaction reaction2 = reaction.Builder.NewReaction();
            reaction2.Id = "r2";
            IAtomContainer moleculeC = reaction.Builder.NewAtomContainer();
            moleculeC.Id = "C";
            reaction2.Reactants.Add(moleculeB);
            reaction2.Products.Add(moleculeC);

            scheme1.Add(reaction2);

            CMLWriter cmlWriter = new CMLWriter(writer);
            cmlWriter.Write(scheme1);
            cmlWriter.Close();
            string cmlContent = writer.ToString();
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
            StringWriter writer = new StringWriter();
            ReactionScheme scheme1 = new ReactionScheme();
            scheme1.Id = "rs0";

            IReaction reaction = Default.ChemObjectBuilder.Instance.NewReaction();
            reaction.Id = "r1";
            IAtomContainer moleculeA = reaction.Builder.NewAtomContainer();
            moleculeA.Id = "A";
            IAtomContainer moleculeB = reaction.Builder.NewAtomContainer();
            moleculeB.Id = "B";
            reaction.Reactants.Add(moleculeA);
            reaction.Products.Add(moleculeB);

            scheme1.Add(reaction);

            IReaction reaction2 = reaction.Builder.NewReaction();
            reaction2.Id = "r2";
            IAtomContainer moleculeC = reaction.Builder.NewAtomContainer();
            moleculeC.Id = "C";
            reaction2.Reactants.Add(moleculeB);
            reaction2.Products.Add(moleculeC);

            scheme1.Add(reaction2);

            CMLWriter cmlWriter = new CMLWriter(writer);
            cmlWriter.Write(scheme1);
            cmlWriter.Close();
            string cmlContent = writer.ToString();
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
            StringWriter writer = new StringWriter();
            ReactionScheme scheme1 = new ReactionScheme();
            scheme1.Id = "rs0";

            IReaction reaction = Default.ChemObjectBuilder.Instance.NewReaction();
            reaction.Id = "r1";
            IAtomContainer moleculeA = reaction.Builder.NewAtomContainer();
            moleculeA.Id = "A";
            IMolecularFormula formula = new MolecularFormula();
            formula.Add(reaction.Builder.NewIsotope("C"), 10);
            formula.Add(reaction.Builder.NewIsotope("H"), 15);
            formula.Add(reaction.Builder.NewIsotope("N"), 2);
            formula.Add(reaction.Builder.NewIsotope("O"), 1);
            moleculeA.SetProperty(CDKPropertyName.Formula, formula);
            IAtomContainer moleculeB = reaction.Builder.NewAtomContainer();
            moleculeB.Id = "B";
            reaction.Reactants.Add(moleculeA);
            reaction.Products.Add(moleculeB);

            scheme1.Add(reaction);

            IReaction reaction2 = reaction.Builder.NewReaction();
            reaction2.Id = "r2";
            IAtomContainer moleculeC = reaction.Builder.NewAtomContainer();
            moleculeC.Id = "C";
            reaction2.Reactants.Add(moleculeB);
            reaction2.Products.Add(moleculeC);

            scheme1.Add(reaction2);

            CMLWriter cmlWriter = new CMLWriter(writer);
            cmlWriter.Write(scheme1);
            cmlWriter.Close();
            string cmlContent = writer.ToString();

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
            StringWriter writer = new StringWriter();
            ReactionScheme scheme1 = new ReactionScheme();
            scheme1.Id = "rs0";

            IReaction reaction = Default.ChemObjectBuilder.Instance.NewReaction();
            reaction.Id = "r1";
            IAtomContainer moleculeA = reaction.Builder.NewAtomContainer();
            moleculeA.Id = "A";
            moleculeA.SetProperty(CDKPropertyName.Formula, "C 10 H 15 N 2 O 1");
            IAtomContainer moleculeB = reaction.Builder.NewAtomContainer();
            moleculeB.Id = "B";
            reaction.Reactants.Add(moleculeA);
            reaction.Products.Add(moleculeB);

            scheme1.Add(reaction);

            IReaction reaction2 = reaction.Builder.NewReaction();
            reaction2.Id = "r2";
            IAtomContainer moleculeC = reaction.Builder.NewAtomContainer();
            moleculeC.Id = "C";
            reaction2.Reactants.Add(moleculeB);
            reaction2.Products.Add(moleculeC);

            scheme1.Add(reaction2);

            CMLWriter cmlWriter = new CMLWriter(writer);
            cmlWriter.Write(scheme1);
            cmlWriter.Close();
            string cmlContent = writer.ToString();
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
            StringWriter writer = new StringWriter();
            IChemModel chemModel = new ChemModel();
            chemModel.Id = "cm0";

            CMLWriter cmlWriter = new CMLWriter(writer);
            cmlWriter.Write(chemModel);
            cmlWriter.Close();
            string cmlContent = writer.ToString();
            Debug.WriteLine("****************************** TestReactionCustomization()");
            Debug.WriteLine(cmlContent);
            Debug.WriteLine("******************************");
            Assert.IsTrue(cmlContent.IndexOf("<list convention=\"cdk:model\" id=\"cm0") != -1);
        }

        [TestMethod()]
        public void TestMoleculeSetID()
        {
            StringWriter writer = new StringWriter();
            var moleculeSet = new ChemObjectSet<IAtomContainer>();
            moleculeSet.Id = "ms0";

            CMLWriter cmlWriter = new CMLWriter(writer);
            cmlWriter.Write(moleculeSet);
            cmlWriter.Close();
            string cmlContent = writer.ToString();
            Debug.WriteLine("****************************** TestReactionCustomization()");
            Debug.WriteLine(cmlContent);
            Debug.WriteLine("******************************");
            Assert.IsTrue(cmlContent.IndexOf("<moleculeList convention=\"cdk:moleculeSet\" id=\"ms0") != -1);
        }

        [TestMethod()]
        public void TestReactionProperty()
        {
            StringWriter writer = new StringWriter();
            IReaction reaction = Default.ChemObjectBuilder.Instance.NewReaction();
            reaction.Id = "r1";
            reaction.SetProperty("blabla", "blabla2");
            CMLWriter cmlWriter = new CMLWriter(writer);
            cmlWriter.Write(reaction);
            cmlWriter.Close();
            string cmlContent = writer.ToString();
            Debug.WriteLine("****************************** TestReactionCustomization()");
            Debug.WriteLine(cmlContent);
            Debug.WriteLine("******************************");
            Assert.IsTrue(cmlContent.IndexOf("<scalar dictRef=\"cdk:reactionProperty") != -1);
        }

        /// <summary>
        /// TODO: introduce concept for ReactionStepList and ReactionStep.
        /// </summary>
        //    [TestMethod()] public void TestReactionStepList()  {
        //        StringWriter writer = new StringWriter();
        //        ReactionChain chain = new ReactionChain();
        //        chain.Id = "rsl1";
        //
        //
        //        IReaction reaction = Default.ChemObjectBuilder.Instance.NewReaction();
        //        reaction.Id = "r1";
        //        IAtomContainer moleculeA = reaction.GetNewBuilder().NewAtomContainer();
        //        moleculeA.Id = "A";
        //        IAtomContainer moleculeB = reaction.GetNewBuilder().NewAtomContainer();
        //        moleculeB.Id = "B";
        //        reaction.Reactants.Add(moleculeA);
        //        reaction.Products.Add(moleculeB);
        //
        //        chain.AddReaction(reaction);
        //
        //        IReaction reaction2 = reaction.GetNewBuilder().NewReaction();
        //        reaction2.Id = "r2";
        //        IAtomContainer moleculeC = reaction.GetNewBuilder().NewAtomContainer();
        //        moleculeC.Id = "C";
        //        reaction2.Reactants.Add(moleculeB);
        //        reaction2.Products.Add(moleculeC);
        //
        //        chain.AddReaction(reaction2);
        //
        //        CMLWriter cmlWriter = new CMLWriter(writer);
        //        cmlWriter.Write(chain);
        //        string cmlContent = writer.ToString();
        //        Debug.WriteLine("****************************** TestReactionCustomization()");
        //        Debug.WriteLine(cmlContent);
        //        Debug.WriteLine("******************************");
        //        Assert.IsTrue(cmlContent.IndexOf("<reactionStepList id=\"rsl1") != -1);
        //        Assert.IsTrue(cmlContent.IndexOf("<reaction id=\"r1") != -1);
        //        Assert.IsTrue(cmlContent.IndexOf("<reaction id=\"r2") != -1);
        //        Assert.IsTrue(cmlContent.IndexOf("<molecule id=\"A") != -1);
        //        Assert.IsTrue(cmlContent.IndexOf("<molecule id=\"B") != -1);
        //        Assert.IsTrue(cmlContent.IndexOf("<molecule id=\"C") != -1);
        //    }
        //
        //    [TestMethod()] public void TestReactionSchemeStepList1()  {
        //        StringWriter writer = new StringWriter();
        //        ReactionScheme scheme1 = new ReactionScheme();
        //        scheme1.Id = "rs0";
        //        ReactionScheme scheme2 = new ReactionScheme();
        //        scheme2.Id = "rs1";
        //        scheme1.Add(scheme2);
        //
        //
        //        IReaction reaction1 = Default.ChemObjectBuilder.Instance.NewReaction();
        //        reaction1.Id = "r1.1";
        //        IAtomContainer moleculeA = reaction1.GetNewBuilder().NewAtomContainer();
        //        moleculeA.Id = "A";
        //        IAtomContainer moleculeB = reaction1.GetNewBuilder().NewAtomContainer();
        //        moleculeB.Id = "B";
        //        reaction1.Reactants.Add(moleculeA);
        //        reaction1.Products.Add(moleculeB);
        //
        //        scheme2.AddReaction(reaction1);
        //
        //        IReaction reaction2 = reaction1.GetNewBuilder().NewReaction();
        //        reaction2.Id = "r1.2";
        //        IAtomContainer moleculeC = reaction1.GetNewBuilder().NewAtomContainer();
        //        moleculeC.Id = "C";
        //        reaction2.Reactants.Add(moleculeB);
        //        reaction2.Products.Add(moleculeC);
        //
        //        scheme2.AddReaction(reaction2);
        //
        //        ReactionChain chain = new ReactionChain();
        //        chain.Id = "rsl1";
        //
        //        IReaction reaction3 = reaction1.GetNewBuilder().NewReaction();
        //        reaction3.Id = "r2.1";
        //        IAtomContainer moleculeD = reaction1.GetNewBuilder().NewAtomContainer();
        //        moleculeD.Id = "D";
        //        reaction3.Reactants.Add(moleculeA);
        //        reaction3.Products.Add(moleculeD);
        //
        //        chain.AddReaction(reaction3,0);
        //
        //        IReaction reaction4 = reaction1.GetNewBuilder().NewReaction();
        //        reaction4.Id = "r2.2";
        //        IAtomContainer moleculeE = reaction1.GetNewBuilder().NewAtomContainer();
        //        moleculeE.Id = "E";
        //        reaction4.Reactants.Add(moleculeD);
        //        reaction4.Products.Add(moleculeE);
        //
        //        chain.AddReaction(reaction4,1);
        //
        ////        scheme1.Add((IReactionSet)chain);
        //
        //        CMLWriter cmlWriter = new CMLWriter(writer);
        //        cmlWriter.Write(scheme1);
        //        string cmlContent = writer.ToString();
        //        Debug.WriteLine("****************************** TestReactionCustomization()");
        //        Debug.WriteLine(cmlContent);
        //        Debug.WriteLine("******************************");
        //        Assert.IsTrue(cmlContent.IndexOf("<reactionScheme id=\"rs0") != -1);
        //        Assert.IsTrue(cmlContent.IndexOf("<reactionScheme id=\"rs1") != -1);
        //        Assert.IsTrue(cmlContent.IndexOf("<reaction id=\"r1") != -1);
        //        Assert.IsTrue(cmlContent.IndexOf("<reaction id=\"r2") != -1);
        //        Assert.IsTrue(cmlContent.IndexOf("<molecule id=\"A") != -1);
        //        Assert.IsTrue(cmlContent.IndexOf("<molecule id=\"B") != -1);
        //        Assert.IsTrue(cmlContent.IndexOf("<molecule id=\"C") != -1);
        //        Assert.IsTrue(cmlContent.IndexOf("<reactionStepList id=\"rsl1") != -1);
        //        Assert.IsTrue(cmlContent.IndexOf("<molecule id=\"D") != -1);
        //        Assert.IsTrue(cmlContent.IndexOf("<molecule id=\"E") != -1);
        //    }

        [TestMethod()]
        public void WriteIsClosed()
        {
            var mock = new Mock<TextWriter>();
            new CMLWriter(mock.Object).Close();
            mock.Verify(n => n.Close());
        }
    }
}
