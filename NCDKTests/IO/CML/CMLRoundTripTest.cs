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
using NCDK.Default;
using NCDK.LibIO.CML;
using NCDK.Templates;
using NCDK.Tools.Manipulator;
using System.Diagnostics;
using System.Linq;

namespace NCDK.IO.CML {
    /// <summary>
    /// TestCase for reading CML 2 files using a few test files in data/cmltest.
    /// </summary>
    // @cdk.module  test-libiocml
    [TestClass()]
    public class CMLRoundTripTest : CDKTestCase
    {
        private static Convertor convertor;

        static CMLRoundTripTest()
        {
            convertor = new Convertor(false, "");
        }

        [TestMethod()]
        public void TestAtom()
        {
            IAtomContainer mol = new AtomContainer();
            Atom atom = new Atom("N");
            mol.Atoms.Add(atom);

            IAtomContainer roundTrippedMol = CMLRoundTripTool.RoundTripMolecule(convertor, mol);

            Assert.AreEqual(1, roundTrippedMol.Atoms.Count);
            IAtom roundTrippedAtom = roundTrippedMol.Atoms[0];
            Assert.AreEqual(atom.Symbol, roundTrippedAtom.Symbol);
        }

        [TestMethod()]
        public void TestAtomId()
        {
            IAtomContainer mol = new AtomContainer();
            Atom atom = new Atom("N");
            atom.Id = "N1";
            mol.Atoms.Add(atom);

            IAtomContainer roundTrippedMol = CMLRoundTripTool.RoundTripMolecule(convertor, mol);

            Assert.AreEqual(1, roundTrippedMol.Atoms.Count);
            IAtom roundTrippedAtom = roundTrippedMol.Atoms[0];
            Assert.AreEqual(atom.Id, roundTrippedAtom.Id);
        }

        [TestMethod()]
        public void TestAtom2D()
        {
            IAtomContainer mol = new AtomContainer();
            Atom atom = new Atom("N");
            Vector2 p2d = new Vector2(1.3, 1.4);
            atom.Point2D = p2d;
            mol.Atoms.Add(atom);

            IAtomContainer roundTrippedMol = CMLRoundTripTool.RoundTripMolecule(convertor, mol);

            Assert.AreEqual(1, roundTrippedMol.Atoms.Count);
            IAtom roundTrippedAtom = roundTrippedMol.Atoms[0];
            base.AssertAreEqual(atom.Point2D, roundTrippedAtom.Point2D, 0.00001);
        }

        [TestMethod()]
        public void TestAtom3D()
        {
            IAtomContainer mol = new AtomContainer();
            Atom atom = new Atom("N");
            Vector3 p3d = new Vector3(1.3, 1.4, 0.9);
            atom.Point3D = p3d;
            mol.Atoms.Add(atom);

            IAtomContainer roundTrippedMol = CMLRoundTripTool.RoundTripMolecule(convertor, mol);

            Assert.AreEqual(1, roundTrippedMol.Atoms.Count);
            IAtom roundTrippedAtom = roundTrippedMol.Atoms[0];
            base.AssertAreEqual(atom.Point3D, roundTrippedAtom.Point3D, 0.00001);
        }

        [TestMethod()]
        public void TestAtom2DAnd3D()
        {
            IAtomContainer mol = new AtomContainer();
            Atom atom = new Atom("N");
            Vector2 p2d = new Vector2(1.3, 1.4);
            atom.Point2D = p2d;
            Vector3 p3d = new Vector3(1.3, 1.4, 0.9);
            atom.Point3D = p3d;
            mol.Atoms.Add(atom);

            IAtomContainer roundTrippedMol = CMLRoundTripTool.RoundTripMolecule(convertor, mol);

            Assert.AreEqual(1, roundTrippedMol.Atoms.Count);
            IAtom roundTrippedAtom = roundTrippedMol.Atoms[0];
            base.AssertAreEqual(atom.Point2D, roundTrippedAtom.Point2D, 0.00001);
            base.AssertAreEqual(atom.Point3D, roundTrippedAtom.Point3D, 0.00001);
        }

        [TestMethod()]
        public void TestAtomFract3D()
        {
            IAtomContainer mol = new AtomContainer();
            Atom atom = new Atom("N");
            Vector3 p3d = new Vector3(0.3, 0.4, 0.9);
            atom.FractionalPoint3D = p3d;
            mol.Atoms.Add(atom);

            IAtomContainer roundTrippedMol = CMLRoundTripTool.RoundTripMolecule(convertor, mol);

            Assert.AreEqual(1, roundTrippedMol.Atoms.Count);
            IAtom roundTrippedAtom = roundTrippedMol.Atoms[0];
            base.AssertAreEqual(atom.Point3D, roundTrippedAtom.Point3D, 0.00001);
        }

        [TestMethod()]
        public void TestPseudoAtom()
        {
            IAtomContainer mol = new AtomContainer();
            PseudoAtom atom = new PseudoAtom("N");
            atom.Label = "Glu55";
            mol.Atoms.Add(atom);

            IAtomContainer roundTrippedMol = CMLRoundTripTool.RoundTripMolecule(convertor, mol);

            Assert.AreEqual(1, roundTrippedMol.Atoms.Count);
            IAtom roundTrippedAtom = roundTrippedMol.Atoms[0];
            Assert.IsNotNull(roundTrippedAtom);
            Assert.IsTrue(roundTrippedAtom is IPseudoAtom);
            Assert.AreEqual("Glu55", ((IPseudoAtom)roundTrippedAtom).Label);
        }

        /// <summary>
        // @cdk.bug 1455346
        /// </summary>
        [TestMethod()]
        public void TestChemModel()
        {
            ChemModel model = new ChemModel();
            var moleculeSet = new ChemObjectSet<IAtomContainer>();
            IAtomContainer mol = new AtomContainer();
            PseudoAtom atom = new PseudoAtom("N");
            mol.Atoms.Add(atom);
            moleculeSet.Add(mol);
            model.MoleculeSet = moleculeSet;

            IChemModel roundTrippedModel = CMLRoundTripTool.RoundTripChemModel(convertor, model);

            var roundTrippedMolSet = roundTrippedModel.MoleculeSet;
            Assert.IsNotNull(roundTrippedMolSet);
            Assert.AreEqual(1, roundTrippedMolSet.Count);
            IAtomContainer roundTrippedMolecule = roundTrippedMolSet[0];
            Assert.IsNotNull(roundTrippedMolecule);
            Assert.AreEqual(1, roundTrippedMolecule.Atoms.Count);
        }

        [TestMethod()]
        public void TestAtomFormalCharge()
        {
            IAtomContainer mol = new AtomContainer();
            Atom atom = new Atom("N");
            int formalCharge = +1;
            atom.FormalCharge = formalCharge;
            mol.Atoms.Add(atom);

            IAtomContainer roundTrippedMol = CMLRoundTripTool.RoundTripMolecule(convertor, mol);

            Assert.AreEqual(1, roundTrippedMol.Atoms.Count);
            IAtom roundTrippedAtom = roundTrippedMol.Atoms[0];
            Assert.AreEqual(atom.FormalCharge, roundTrippedAtom.FormalCharge);
        }

        /// <summary>
        // @cdk.bug 1713398
        /// </summary>
        [TestMethod()]
        public void TestHydrogenCount()
        {
            IAtomContainer mol = new AtomContainer();
            Atom atom = new Atom("N");
            atom.ImplicitHydrogenCount = 3;
            mol.Atoms.Add(atom);

            IAtomContainer roundTrippedMol = CMLRoundTripTool.RoundTripMolecule(convertor, mol);

            Assert.AreEqual(1, roundTrippedMol.Atoms.Count);
            IAtom roundTrippedAtom = roundTrippedMol.Atoms[0];
            Assert.AreEqual(atom.ImplicitHydrogenCount, roundTrippedAtom.ImplicitHydrogenCount);
        }

        /// <summary>
        // @cdk.bug 1713398
        /// </summary>
        [TestMethod()]
        public void TestHydrogenCount_UNSET()
        {
            IAtomContainer mol = new AtomContainer();
            Atom atom = new Atom("N");
            atom.ImplicitHydrogenCount = null;
            mol.Atoms.Add(atom);

            IAtomContainer roundTrippedMol = CMLRoundTripTool.RoundTripMolecule(convertor, mol);

            Assert.AreEqual(1, roundTrippedMol.Atoms.Count);
            IAtom roundTrippedAtom = roundTrippedMol.Atoms[0];
            Assert.AreEqual(null, roundTrippedAtom.ImplicitHydrogenCount);
        }

        //@Ignore("Have to figure out how to store partial charges in CML2")
        public void TestAtomPartialCharge()
        {
            IAtomContainer mol = new AtomContainer();
            Atom atom = new Atom("N");
            double partialCharge = 0.5;
            atom.Charge = partialCharge;
            mol.Atoms.Add(atom);

            IAtomContainer roundTrippedMol = CMLRoundTripTool.RoundTripMolecule(convertor, mol);

            Assert.AreEqual(1, roundTrippedMol.Atoms.Count);
            IAtom roundTrippedAtom = roundTrippedMol.Atoms[0];
            Assert.AreEqual(atom.Charge.Value, roundTrippedAtom.Charge.Value, 0.0001);
        }

        //@Ignore("Have to figure out how to store atom parity in CML2")
        public void TestAtomStereoParity()
        {
            IAtomContainer mol = new AtomContainer();
            Atom atom = new Atom("C");
            int stereo = CDKConstants.STEREO_ATOM_PARITY_PLUS;
            atom.StereoParity = stereo;
            mol.Atoms.Add(atom);

            IAtomContainer roundTrippedMol = CMLRoundTripTool.RoundTripMolecule(convertor, mol);

            Assert.AreEqual(1, roundTrippedMol.Atoms.Count);
            IAtom roundTrippedAtom = roundTrippedMol.Atoms[0];
            Assert.AreEqual(atom.StereoParity, roundTrippedAtom.StereoParity);
        }

        [TestMethod()]
        public void TestIsotope()
        {
            IAtomContainer mol = new AtomContainer();
            Atom atom = new Atom("C");
            atom.MassNumber = 13;
            mol.Atoms.Add(atom);
            IAtomContainer roundTrippedMol = CMLRoundTripTool.RoundTripMolecule(convertor, mol);

            Assert.AreEqual(1, roundTrippedMol.Atoms.Count);
            IAtom roundTrippedAtom = roundTrippedMol.Atoms[0];
            Assert.AreEqual(atom.MassNumber, roundTrippedAtom.MassNumber);
        }

        /// <summary>
        // @cdk.bug 1014344
        /// </summary>
        //@Ignore("Functionality not yet implemented - exact mass can not be written/read")
        public void TestIsotope_ExactMass()
        {
            IAtomContainer mol = new AtomContainer();
            Atom atom = new Atom("C");
            atom.ExactMass = 13.0;
            mol.Atoms.Add(atom);
            IAtomContainer roundTrippedMol = CMLRoundTripTool.RoundTripMolecule(convertor, mol);

            Assert.AreEqual(1, roundTrippedMol.Atoms.Count);
            IAtom roundTrippedAtom = roundTrippedMol.Atoms[0];
            Assert.IsNotNull(atom.ExactMass);
            Assert.IsNotNull(roundTrippedAtom.ExactMass);
            Assert.AreEqual(atom.ExactMass.Value, roundTrippedAtom.ExactMass.Value, 0.01);
        }

        /// <summary>
        // @cdk.bug 1014344
        /// </summary>
        //@Ignore("Functionality not yet implemented - natural abundance can not be written/read")
        public void TestIsotope_Abundance()
        {
            IAtomContainer mol = new AtomContainer();
            Atom atom = new Atom("C");
            atom.NaturalAbundance = 1.0;
            mol.Atoms.Add(atom);
            IAtomContainer roundTrippedMol = CMLRoundTripTool.RoundTripMolecule(convertor, mol);

            Assert.AreEqual(1, roundTrippedMol.Atoms.Count);
            IAtom roundTrippedAtom = roundTrippedMol.Atoms[0];
            Assert.IsNotNull(atom.NaturalAbundance);
            Assert.IsNotNull(roundTrippedAtom.NaturalAbundance);
            Assert.AreEqual(atom.NaturalAbundance.Value, roundTrippedAtom.NaturalAbundance.Value, 0.01);
        }

        /// <summary>
        /// Test roundtripping of MassNumber.
        // @
        /// </summary>
        [TestMethod()]
        public void TestMassNumber()
        {
            IAtomContainer mol = new AtomContainer();
            Atom atom = new Atom("C");
            atom.MassNumber = 12;
            mol.Atoms.Add(atom);
            Assert.AreEqual(12, atom.MassNumber.Value);

            IAtomContainer roundTrippedMol = CMLRoundTripTool.RoundTripMolecule(convertor, mol);

            Assert.AreEqual(1, roundTrippedMol.Atoms.Count);
            IAtom roundTrippedAtom = roundTrippedMol.Atoms[0];
            Assert.AreEqual(atom.MassNumber, roundTrippedAtom.MassNumber);
        }

        [TestMethod()]
        public void TestBond()
        {
            IAtomContainer mol = new AtomContainer();
            Atom atom = new Atom("C");
            Atom atom2 = new Atom("O");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            Bond bond = new Bond(atom, atom2, BondOrder.Single);
            mol.Bonds.Add(bond);

            IAtomContainer roundTrippedMol = CMLRoundTripTool.RoundTripMolecule(convertor, mol);

            Assert.AreEqual(2, roundTrippedMol.Atoms.Count);
            Assert.AreEqual(1, roundTrippedMol.Bonds.Count);
            IBond roundTrippedBond = roundTrippedMol.Bonds[0];
            Assert.AreEqual(2, roundTrippedBond.Atoms.Count);
            Assert.AreEqual("C", roundTrippedBond.Begin.Symbol); // preserved direction?
            Assert.AreEqual("O", roundTrippedBond.End.Symbol);
            Assert.AreEqual(bond.Order, roundTrippedBond.Order);
        }

        [TestMethod()]
        public void TestBondID()
        {
            IAtomContainer mol = new AtomContainer();
            Atom atom = new Atom("C");
            Atom atom2 = new Atom("O");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            Bond bond = new Bond(atom, atom2, BondOrder.Single);
            bond.Id = "b1";
            mol.Bonds.Add(bond);

            IAtomContainer roundTrippedMol = CMLRoundTripTool.RoundTripMolecule(convertor, mol);
            IBond roundTrippedBond = roundTrippedMol.Bonds[0];
            Assert.AreEqual(bond.Id, roundTrippedBond.Id);
        }

        [TestMethod()]
        public void TestBondStereo()
        {
            IAtomContainer mol = new AtomContainer();
            Atom atom = new Atom("C");
            Atom atom2 = new Atom("O");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            Bond bond = new Bond(atom, atom2, BondOrder.Single);
            BondStereo stereo = BondStereo.Down;
            bond.Stereo = stereo;
            mol.Bonds.Add(bond);

            IAtomContainer roundTrippedMol = CMLRoundTripTool.RoundTripMolecule(convertor, mol);

            Assert.AreEqual(2, roundTrippedMol.Atoms.Count);
            Assert.AreEqual(1, roundTrippedMol.Bonds.Count);
            IBond roundTrippedBond = roundTrippedMol.Bonds[0];
            Assert.AreEqual(bond.Stereo, roundTrippedBond.Stereo);
        }

        [TestMethod()]
        public void TestBondAromatic()
        {
            IAtomContainer mol = new AtomContainer();
            // surely, this bond is not aromatic... but fortunately, file formats do not care about chemistry
            Atom atom = new Atom("C");
            Atom atom2 = new Atom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            Bond bond = new Bond(atom, atom2, BondOrder.Single);
            bond.IsAromatic = true;
            mol.Bonds.Add(bond);

            IAtomContainer roundTrippedMol = CMLRoundTripTool.RoundTripMolecule(convertor, mol);

            Assert.AreEqual(2, roundTrippedMol.Atoms.Count);
            Assert.AreEqual(1, roundTrippedMol.Bonds.Count);
            IBond roundTrippedBond = roundTrippedMol.Bonds[0];
            Assert.AreEqual(bond.IsAromatic, roundTrippedBond.IsAromatic);
            Assert.AreEqual(bond.Order, roundTrippedBond.Order);
        }

        /// <summary>
        // @cdk.bug 1713398
        /// </summary>
        [TestMethod()]
        public void TestBondAromatic_Double()
        {
            IAtomContainer mol = new AtomContainer();
            // surely, this bond is not aromatic... but fortunately, file formats do not care about chemistry
            Atom atom = new Atom("C");
            Atom atom2 = new Atom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            Bond bond = new Bond(atom, atom2, BondOrder.Double);
            bond.IsAromatic = true;
            mol.Bonds.Add(bond);

            IAtomContainer roundTrippedMol = CMLRoundTripTool.RoundTripMolecule(convertor, mol);

            Assert.AreEqual(2, roundTrippedMol.Atoms.Count);
            Assert.AreEqual(1, roundTrippedMol.Bonds.Count);
            IBond roundTrippedBond = roundTrippedMol.Bonds[0];
            Assert.AreEqual(bond.IsAromatic, roundTrippedBond.IsAromatic);
            Assert.AreEqual(bond.Order, roundTrippedBond.Order);
        }

        [TestMethod()]
        public void TestPartialCharge()
        {
            IAtomContainer mol = new AtomContainer();
            Atom atom = new Atom("C");
            mol.Atoms.Add(atom);
            double charge = -0.267;
            atom.Charge = charge;

            IAtomContainer roundTrippedMol = CMLRoundTripTool.RoundTripMolecule(convertor, mol);

            Assert.AreEqual(1, roundTrippedMol.Atoms.Count);
            IAtom roundTrippedAtom = roundTrippedMol.Atoms[0];
            Assert.AreEqual(charge, roundTrippedAtom.Charge.Value, 0.0001);
        }

        [TestMethod()]
        public void TestInChI()
        {
            IAtomContainer mol = new AtomContainer();
            string inchi = "InChI=1/CH2O2/c2-1-3/h1H,(H,2,3)";
            mol.SetProperty(CDKPropertyName.InChI, inchi);

            IAtomContainer roundTrippedMol = CMLRoundTripTool.RoundTripMolecule(convertor, mol);
            Assert.IsNotNull(roundTrippedMol);

            Assert.AreEqual(inchi, roundTrippedMol.GetProperty<string>(CDKPropertyName.InChI));
        }

        [TestMethod()]
        public void TestSpinMultiplicity()
        {
            IAtomContainer mol = new AtomContainer();
            Atom atom = new Atom("C");
            mol.Atoms.Add(atom);
            mol.SingleElectrons.Add(new SingleElectron(atom));

            IAtomContainer roundTrippedMol = CMLRoundTripTool.RoundTripMolecule(convertor, mol);

            Assert.AreEqual(1, roundTrippedMol.Atoms.Count);
            Assert.AreEqual(1, roundTrippedMol.GetElectronContainers().Count());
            IAtom roundTrippedAtom = roundTrippedMol.Atoms[0];
            Assert.AreEqual(1, roundTrippedMol.GetConnectedSingleElectrons(roundTrippedAtom).Count());
        }

        [TestMethod()]
        public void TestReaction()
        {
            Debug.WriteLine("********** TEST REACTION **********");
            IReaction reaction = new Reaction();
            reaction.Id = "reaction.1";
            IAtomContainer reactant = reaction.Builder.NewAtomContainer();
            reactant.Id = "react";
            IAtom atom = reaction.Builder.NewAtom("C");
            reactant.Atoms.Add(atom);
            reaction.Reactants.Add(reactant);

            IAtomContainer product = reaction.Builder.NewAtomContainer();
            product.Id = "product";
            atom = reaction.Builder.NewAtom("R");
            product.Atoms.Add(atom);
            reaction.Products.Add(product);

            IAtomContainer agent = reaction.Builder.NewAtomContainer();
            agent.Id = "water";
            atom = reaction.Builder.NewAtom("H");
            agent.Atoms.Add(atom);
            reaction.Agents.Add(agent);

            IReaction roundTrippedReaction = CMLRoundTripTool.RoundTripReaction(convertor, reaction);
            Assert.IsNotNull(roundTrippedReaction);
            Assert.AreEqual("reaction.1", roundTrippedReaction.Id);

            Assert.IsNotNull(roundTrippedReaction);
            var reactants = roundTrippedReaction.Reactants;
            Assert.IsNotNull(reactants);
            Assert.AreEqual(1, reactants.Count);
            IAtomContainer roundTrippedReactant = reactants[0];
            Assert.AreEqual("react", roundTrippedReactant.Id);
            Assert.AreEqual(1, roundTrippedReactant.Atoms.Count);

            var products = roundTrippedReaction.Products;
            Assert.IsNotNull(products);
            Assert.AreEqual(1, products.Count);
            IAtomContainer roundTrippedProduct = products[0];
            Assert.AreEqual("product", roundTrippedProduct.Id);
            Assert.AreEqual(1, roundTrippedProduct.Atoms.Count);

            var agents = roundTrippedReaction.Agents;
            Assert.IsNotNull(agents);
            Assert.AreEqual(1, agents.Count);
            IAtomContainer roundTrippedAgent = agents[0];
            Assert.AreEqual("water", roundTrippedAgent.Id);
            Assert.AreEqual(1, roundTrippedAgent.Atoms.Count);
        }

        [TestMethod()]
        public void TestDescriptorValue()
        {
            IAtomContainer molecule = TestMoleculeFactory.MakeBenzene();

            string[] propertyName = { "testKey1", "testKey2" };
            string[] propertyValue = { "testValue1", "testValue2" };

            for (int i = 0; i < propertyName.Length; i++)
                molecule.SetProperty(propertyName[i], propertyValue[i]);
            IAtomContainer roundTrippedMol = CMLRoundTripTool.RoundTripMolecule(convertor, molecule);

            for (int i = 0; i < propertyName.Length; i++)
            {
                Assert.IsNotNull(roundTrippedMol.GetProperty<string>(propertyName[i]));
                Assert.AreEqual(propertyValue[i], roundTrippedMol.GetProperty<string>(propertyName[i]));
            }
        }

        /// <summary>
        /// Tests of bond order information is stored even when aromaticity is given.
        ///
        // @
        /// </summary>
        [TestMethod()]
        public void TestAromaticity()
        {
            IAtomContainer molecule = TestMoleculeFactory.MakeBenzene();
            foreach (var bond in molecule.Bonds)
            {
                bond.IsAromatic = true;
            }

            IAtomContainer roundTrippedMol = CMLRoundTripTool.RoundTripMolecule(convertor, molecule);
            var bonds = roundTrippedMol.Bonds;
            double orderSum = BondManipulator.GetSingleBondEquivalentSum(bonds);
            foreach (var bond in bonds)
            {
                Assert.IsTrue(bond.IsAromatic);
            }
            Assert.AreEqual(9.0, orderSum, 0.001);
        }

        [TestMethod()]
        public void TestAtomAromaticity()
        {
            IAtomContainer molecule = TestMoleculeFactory.MakeBenzene();
            foreach (var atom in molecule.Atoms)
            {
                atom.IsAromatic = true;
            }

            IAtomContainer roundTrippedMol = CMLRoundTripTool.RoundTripMolecule(convertor, molecule);
            foreach (var atom in roundTrippedMol.Atoms)
            {
                Assert.IsTrue(atom.IsAromatic);
            }
        }

        /// <summary>
        /// Tests whether the custom atom properties survive the CML round-trip
        // @
        ///
        // @cdk.bug 1930029
        /// </summary>
        [TestMethod()]
        public void TestAtomProperty()
        {
            string[] key = { "customAtomProperty1", "customAtomProperty2" };
            string[] value = { "true", "false" };

            IAtomContainer mol = TestMoleculeFactory.MakeBenzene();
            foreach (var a in mol.Atoms)
            {
                for (int i = 0; i < key.Length; i++)
                    a.SetProperty(key[i], value[i]);
            }

            IAtomContainer roundTrippedMol = CMLRoundTripTool.RoundTripMolecule(convertor, mol);
            //Assert.AreEqual(convertor.CDKMoleculeToCMLMolecule(mol).ToXML(),
            //       convertor.CDKMoleculeToCMLMolecule(roundTrippedMol).ToXML());

            foreach (var a in roundTrippedMol.Atoms)
            {
                for (int i = 0; i < key.Length; i++)
                {
                    var actual = a.GetProperty<object>(key[i]);
                    Assert.IsNotNull(actual);
                    Assert.AreEqual(value[i], actual);
                }
            }
        }

        /// <summary>
        /// Tests whether the custom bond properties survive the CML round-trip
        // @
        ///
        // @cdk.bug 1930029
        /// </summary>
        [TestMethod()]
        public void TestBondProperty()
        {
            string[] key = { "customBondProperty1", "customBondProperty2" };
            string[] value = { "true", "false" };
            IAtomContainer mol = TestMoleculeFactory.MakeBenzene();
            foreach (var b in mol.Bonds)
            {
                for (int i = 0; i < key.Length; i++)
                    b.SetProperty(key[i], value[i]);
            }

            IAtomContainer roundTrippedMol = CMLRoundTripTool.RoundTripMolecule(convertor, mol);
            //Assert.AreEqual(convertor.CDKMoleculeToCMLMolecule(mol).ToXML(),
            //       convertor.CDKMoleculeToCMLMolecule(roundTrippedMol).ToXML());

            foreach (var b in roundTrippedMol.Bonds)
            {
                for (int i = 0; i < key.Length; i++)
                {
                    var actual = b.GetProperty<object>(key[i]);
                    Assert.IsNotNull(actual);
                    Assert.AreEqual(value[i], actual);
                }
            }
        }

        /// <summary>
        /// Tests whether the custom molecule properties survive the CML round-trip
        // @
        ///
        // @cdk.bug 1930029
        /// </summary>
        [TestMethod()]
        public void TestMoleculeProperty()
        {
            string[] key = { "customMoleculeProperty1", "customMoleculeProperty2" };
            string[] value = { "true", "false" };

            IAtomContainer mol = TestMoleculeFactory.MakeAdenine();
            for (int i = 0; i < key.Length; i++)
            {
                mol.SetProperty(key[i], value[i]);
            }
            IAtomContainer roundTrippedMol = CMLRoundTripTool.RoundTripMolecule(convertor, mol);
            //Assert.AreEqual(convertor.CDKMoleculeToCMLMolecule(mol).ToXML(),
            //       convertor.CDKMoleculeToCMLMolecule(roundTrippedMol).ToXML());
            for (int i = 0; i < key.Length; i++)
            {
                var actual = roundTrippedMol.GetProperty<object>(key[i]);
                Assert.IsNotNull(actual);
                Assert.AreEqual(value[i], actual);
            }
        }

        [TestMethod()]
        public void TestMoleculeSet()
        {
            var list = new ChemObjectSet<IAtomContainer>();
            list.Add(new AtomContainer());
            list.Add(new AtomContainer());
            IChemModel model = new ChemModel();
            model.MoleculeSet = list;

            IChemModel roundTripped = CMLRoundTripTool.RoundTripChemModel(convertor, model);
            var newList = roundTripped.MoleculeSet;
            Assert.IsNotNull(newList);
            Assert.AreEqual(2, newList.Count());
            Assert.IsNotNull(newList[0]);
            Assert.IsNotNull(newList[1]);
        }

        /// <summary>
        // @cdk.bug 1930029
        /// </summary>
        [TestMethod()]
        public void TestAtomProperties()
        {
            string filename = "NCDK.Data.CML.custompropertiestest.cml";
            var ins = ResourceLoader.GetAsStream(filename);
            CMLReader reader = new CMLReader(ins);
            ChemFile chemFile = (ChemFile)reader.Read((ChemFile)new ChemFile());
            reader.Close();
            Assert.IsNotNull(chemFile);
            IAtomContainer container = ChemFileManipulator.GetAllAtomContainers(chemFile).First();
            for (int i = 0; i < container.Atoms.Count; i++)
            {
                Assert.AreEqual(2, container.Atoms[i].GetProperties().Count);
            }
        }

        /// <summary>
        /// Test roundtripping of Unset property (Hydrogencount).
        // @
        /// </summary>
        [TestMethod()]
        public void TestUnSetHydrogenCount()
        {
            IAtomContainer mol = new AtomContainer();
            Atom atom = new Atom("C");
            atom.ImplicitHydrogenCount = null;
            Assert.IsNull(atom.ImplicitHydrogenCount);
            mol.Atoms.Add(atom);

            IAtomContainer roundTrippedMol = CMLRoundTripTool.RoundTripMolecule(convertor, mol);

            Assert.AreEqual(1, roundTrippedMol.Atoms.Count);
            IAtom roundTrippedAtom = roundTrippedMol.Atoms[0];
            Assert.IsNull(roundTrippedAtom.ImplicitHydrogenCount);
        }
    }
}
