/* Copyright (C) 2010  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Common.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Numerics;

namespace NCDK
{
    /// <summary>
    /// Checks the functionality of <see cref="IChemObjectBuilder"/> implementations.
    ///
    // @cdk.module test-interfaces
    /// </summary>
    [TestClass()]
    public abstract class AbstractChemObjectBuilderTest : CDKTestCase
    {
        public virtual IChemObject RootObject { get; }

        //[TestMethod()]
        //[ExpectedException(typeof(ArgumentException))]
        //public void TestNewInstance_Class_arrayObject()
        //{
        //    // throw random stuff; it should fail
        //    IChemObjectBuilder builder = RootObject.Builder;
        //    builder.CreateAtom(new object[2]);
        //}

        //[TestMethod()]
        //[ExpectedException(typeof(ArgumentException))]
        //public void TestIncorrectNumberOf()
        //{
        //    IChemObjectBuilder builder = RootObject.Builder;
        //    builder.CreateAtom(builder.CreateAtomContainer());
        //}

        [TestMethod()]
        public void TestNewAtom()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IAtom atom = builder.CreateAtom();
            Assert.IsNotNull(atom);
            Assert.IsNull(atom.Symbol);
        }

        [TestMethod()]
        public void TestNewAtom_IElement()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IAtom atom = builder.CreateAtom(builder.CreateElement("N"));
            Assert.IsNotNull(atom);
            Assert.AreEqual("N", atom.Symbol);
        }

        [TestMethod()]
        public void TestNewAminoAcid()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IAminoAcid aa = builder.CreateAminoAcid();
            Assert.IsNotNull(aa);
        }

        [TestMethod()]
        public void TestNewAtom_String()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IAtom atom = builder.CreateAtom("C");
            Assert.IsNotNull(atom);
            Assert.AreEqual("C", atom.Symbol);
        }

        [TestMethod()]
        public void TestNewAtom_String_Point2d()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            Vector2 coord = new Vector2(1, 2);
            IAtom atom = builder.CreateAtom("C", coord);
            Assert.IsNotNull(atom);
            Assert.AreEqual("C", atom.Symbol);
            AssertAreEqual(coord, atom.Point2D, 0.0);
        }

        [TestMethod()]
        public void TestNewAtom_String_Point3d()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            Vector3 coord = new Vector3(1, 2, 3);
            IAtom atom = builder.CreateAtom("C", coord);
            Assert.IsNotNull(atom);
            Assert.AreEqual("C", atom.Symbol);
            AssertAreEqual(coord, atom.Point3D, 0.0);
        }

        [TestMethod()]
        public void TestNewAtomContainer()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IAtomContainer container = builder.CreateAtomContainer();
            Assert.IsNotNull(container);
        }

        //[TestMethod()]
        //public void TestNewAtomContainer_int_int_int_int()
        //{
        //    IChemObjectBuilder builder = RootObject.Builder;
        //    IAtomContainer container = builder.CreateAtomContainer(1, 2, 3, 4);
        //    Assert.IsNotNull(container);
        //}

        [TestMethod()]
        public void TestNewAtomContainer_IAtomContainer()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IAtomContainer container = builder.CreateAtomContainer();
            Assert.IsNotNull(container);
            IAtomContainer second = builder.CreateAtomContainer(container);
            Assert.IsNotNull(second);
        }

        [TestMethod()]
        public void TestNewAtomType_String()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IAtomType type = builder.CreateAtomType("C");
            Assert.IsNotNull(type);
        }

        [TestMethod()]
        public void TestNewAtomType_IElement()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IAtomType type = builder.CreateAtomType(builder.CreateElement("C"));
            Assert.IsNotNull(type);
        }

        [TestMethod()]
        public void TestNewAtomType_String_String()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IAtomType type = builder.CreateAtomType("C", "C.sp2");
            Assert.IsNotNull(type);
        }

        [TestMethod()]
        public void TestNewBioPolymer()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IBioPolymer polymer = builder.CreateBioPolymer();
            Assert.IsNotNull(polymer);
        }

        [TestMethod()]
        public void TestNewBond()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IBond bond = builder.CreateBond();
            Assert.IsNotNull(bond);
        }

        [TestMethod()]
        public void TestNewBond_IAtom_IAtom()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IBond bond = builder.CreateBond(builder.CreateAtom(),
                    builder.CreateAtom());
            Assert.IsNotNull(bond);
        }

        ///**
        // * @cdk.bug 3526870
        // */
        //[TestMethod()]
        //[ExpectedException(typeof(ArgumentException))]
        //public void TestNewBond_IAtom_IAtomContainer()
        //{
        //    IChemObjectBuilder builder = RootObject.Builder;
        //    builder.CreateBond(builder.CreateAtom(), builder.CreateAtomContainer());
        //}

        [TestMethod()]
        public void TestNewBond_IAtom_IAtom_BondOrder()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IBond bond = builder.CreateBond(builder.CreateAtom(),
                    builder.CreateAtom(), BondOrder.Single);
            Assert.IsNotNull(bond);
        }

        [TestMethod()]
        public void TestNewBond_IAtom_IAtom_BondOrder_IBond_Stereo()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IBond bond = builder.CreateBond(builder.CreateAtom(),
                    builder.CreateAtom(), BondOrder.Single, BondStereo.EOrZ);
            Assert.IsNotNull(bond);
        }

        [TestMethod()]
        public void TestNewBond_arrayIAtom()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IBond bond = builder.CreateBond(new IAtom[] { builder.CreateAtom(), builder.CreateAtom() });
            Assert.IsNotNull(bond);
        }

        [TestMethod()]
        public void TestNewBond_arrayIAtom_BondOrder()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IBond bond = builder.CreateBond(new IAtom[] { builder.CreateAtom(), builder.CreateAtom() },
                    BondOrder.Double);
            Assert.IsNotNull(bond);
        }

        [TestMethod()]
        public void TestNewChemFile()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IChemFile file = builder.CreateChemFile();
            Assert.IsNotNull(file);
        }

        [TestMethod()]
        public void TestNewChemModel()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IChemModel model = builder.CreateChemModel();
            Assert.IsNotNull(model);
        }

        [TestMethod()]
        public void TestNewChemObject()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IChemObject model = builder.CreateChemObject();
            Assert.IsNotNull(model);
        }

        [TestMethod()]
        public void TestNewChemObject_IChemObject()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IChemObject model = builder.CreateChemObject(builder.CreateChemObject());
            Assert.IsNotNull(model);
        }

        [TestMethod()]
        public void TestNewChemSequence()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IChemSequence sequence = builder.CreateChemSequence();
            Assert.IsNotNull(sequence);
        }

        [TestMethod()]
        public void TestNewCrystal()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            ICrystal crystal = builder.CreateCrystal();
            Assert.IsNotNull(crystal);
        }

        [TestMethod()]
        public void TestNewCrystal_IAtomContainer()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            ICrystal crystal = builder.CreateCrystal(builder.CreateAtomContainer());
            Assert.IsNotNull(crystal);
        }

        [TestMethod()]
        public void TestNewElectronContainer()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IElectronContainer container = builder.CreateElectronContainer();
            Assert.IsNotNull(container);
        }

        [TestMethod()]
        public void TestNewElement()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IElement element = builder.CreateElement();
            Assert.IsNotNull(element);
        }

        [TestMethod()]
        public void TestNewElement_IElement()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IElement element = builder.CreateElement(builder.CreateElement());
            Assert.IsNotNull(element);
        }

        [TestMethod()]
        public void TestNewElement_String()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IElement element = builder.CreateElement("C");
            Assert.IsNotNull(element);
        }

        [TestMethod()]
        public void TestNewElement_String_int()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IElement element = builder.CreateElement("C", 13);
            Assert.IsNotNull(element);
        }

        [TestMethod()]
        public void TestNewIsotope_int_String_Double_double()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IIsotope isotope = builder.CreateIsotope(6, "C", 1.0, 1.0);
            Assert.IsNotNull(isotope);
        }

        [TestMethod()]
        public void TestNewIsotope_int_String_int_Double_double()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IIsotope isotope = builder.CreateIsotope(6, "C", 13, 1.0, 1.0);
            Assert.IsNotNull(isotope);
        }

        [TestMethod()]
        public void TestNewIsotope_IElement()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IIsotope isotope = builder.CreateIsotope(builder.CreateElement());
            Assert.IsNotNull(isotope);
        }

        [TestMethod()]
        public void TestNewIsotope_String()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IIsotope isotope = builder.CreateIsotope("C");
            Assert.IsNotNull(isotope);
        }

        [TestMethod()]
        public void TestNewIsotope_String_int()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IIsotope isotope = builder.CreateIsotope("C", 13);
            Assert.IsNotNull(isotope);
        }

        [TestMethod()]
        public void TestNewLonePair()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            ILonePair lonePair = builder.CreateLonePair();
            Assert.IsNotNull(lonePair);
        }

        [TestMethod()]
        public void TestNewLonePair_IAtom()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            ILonePair lonePair = builder.CreateLonePair(builder.CreateAtom());
            Assert.IsNotNull(lonePair);
        }

        [TestMethod()]
        public void TestNewMapping_IChemObject_IChemObject()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IMapping mapping = builder.CreateMapping(builder.CreateChemObject(),
                    builder.CreateChemObject());
            Assert.IsNotNull(mapping);
        }

        [TestMethod()]
        public void TestNewMonomer()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IMonomer monomer = builder.CreateMonomer();
            Assert.IsNotNull(monomer);
        }

        [TestMethod()]
        public void TestNewPolymer()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IPolymer polymer = builder.CreatePolymer();
            Assert.IsNotNull(polymer);
        }

        [TestMethod()]
        public void TestNewPDBAtom_IElement()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IPDBAtom atom = builder.CreatePDBAtom(builder.CreateElement());
            Assert.IsNotNull(atom);
        }

        [TestMethod()]
        public void TestNewPDBAtom_String()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IPDBAtom atom = builder.CreatePDBAtom("O.3");
            Assert.IsNotNull(atom);
        }

        [TestMethod()]
        public void TestNewPDBAtom_String_Point3D()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IPDBAtom atom = builder.CreatePDBAtom("O.3", new Vector3(1, 2, 3));
            Assert.IsNotNull(atom);
        }

        [TestMethod()]
        public void TestNewPDBPolymer()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IPDBPolymer polymer = builder.CreatePDBPolymer();
            Assert.IsNotNull(polymer);
        }

        [TestMethod()]
        public void TestNewPDBStructure()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IPDBStructure structure = builder.CreatePDBStructure();
            Assert.IsNotNull(structure);
        }

        [TestMethod()]
        public void TestNewPDBMonomer()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IPDBMonomer monomer = builder.CreatePDBMonomer();
            Assert.IsNotNull(monomer);
        }

        [TestMethod()]
        public void TestNewPseudoAtom()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IPseudoAtom atom = builder.CreatePseudoAtom();
            Assert.IsNotNull(atom);
        }

        [TestMethod()]
        public void TestNewPseudoAtom_IElement()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IPseudoAtom atom = builder.CreatePseudoAtom(builder.CreateElement());
            Assert.IsNotNull(atom);
        }

        [TestMethod()]
        public void TestNewPseudoAtom_IAtom()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IPseudoAtom atom = builder.CreatePseudoAtom(builder.CreateAtom());
            Assert.IsNotNull(atom);
        }

        [TestMethod()]
        public void TestNewPseudoAtom_String()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IPseudoAtom atom = builder.CreatePseudoAtom("Foo");
            Assert.IsNotNull(atom);
        }

        [TestMethod()]
        public void TestNewPseudoAtom_String_Point2d()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IPseudoAtom atom = builder.CreatePseudoAtom("Foo", new Vector2(1, 2));
            Assert.IsNotNull(atom);
        }

        [TestMethod()]
        public void TestNewPseudoAtom_String_Point3d()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IPseudoAtom atom = builder.CreatePseudoAtom("Foo", new Vector3(1, 2, 3));
            Assert.IsNotNull(atom);
        }

        [TestMethod()]
        public void TestNewPDBAtom_String_Point3d()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IPDBAtom atom = builder.CreatePDBAtom("Foo", new Vector3(1, 2, 3));
            Assert.IsNotNull(atom);
        }

        [TestMethod()]
        public void TestNewReaction()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IReaction reaction = builder.CreateReaction();
            Assert.IsNotNull(reaction);
        }

        [TestMethod()]
        public void TestNewRing()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IRing ring = builder.CreateRing();
            Assert.IsNotNull(ring);
        }

        //[TestMethod()]
        //public void TestNewRing_int()
        //{
        //    IChemObjectBuilder builder = RootObject.Builder;
        //    IRing ring = builder.CreateRing(4);
        //    Assert.IsNotNull(ring);
        //}

        [TestMethod()]
        public void TestNewRing_int_String()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IRing ring = builder.CreateRing(5, "C");
            Assert.IsNotNull(ring);
        }

        [TestMethod()]
        public void TestNewRing_IAtomContainer()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IRing ring = builder.CreateRing(builder.CreateAtomContainer());
            Assert.IsNotNull(ring);
        }

        [TestMethod()]
        public void TestNewRingSet()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IRingSet set = builder.CreateRingSet();
            Assert.IsNotNull(set);
        }

        [TestMethod()]
        public void TestNewAtomContainerSet()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IAtomContainerSet set = builder.CreateAtomContainerSet();
            Assert.IsNotNull(set);
        }

        [TestMethod()]
        public void TestNewMoleculeSet()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IAtomContainerSet set = builder.CreateAtomContainerSet();
            Assert.IsNotNull(set);
        }

        [TestMethod()]
        public void TestNewReactionSet()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IReactionSet set = builder.CreateReactionSet();
            Assert.IsNotNull(set);
        }

        [TestMethod()]
        public void TestNewReactionScheme()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IReactionScheme scheme = builder.CreateReactionScheme();
            Assert.IsNotNull(scheme);
        }

        [TestMethod()]
        public void TestNewSingleElectron()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            ISingleElectron electron = builder.CreateSingleElectron();
            Assert.IsNotNull(electron);
        }

        [TestMethod()]
        public void TestNewSingleElectron_IAtom()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            ISingleElectron electron = builder.CreateSingleElectron(builder.CreateAtom());
            Assert.IsNotNull(electron);
        }

        [TestMethod()]
        public void TestNewStrand()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IStrand strand = builder.CreateStrand();
            Assert.IsNotNull(strand);
        }

        [TestMethod()]
        public void TestNewFragmentAtom()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IFragmentAtom fragAtom = builder.CreateFragmentAtom();
            Assert.IsNotNull(fragAtom);
        }

        [TestMethod()]
        public void TestNewMolecularFormula()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IMolecularFormula mf = builder.CreateMolecularFormula();
            Assert.IsNotNull(mf);
        }

        [TestMethod()]
        public void TestNewMolecularFormulaSet()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IMolecularFormulaSet mfSet = builder.CreateMolecularFormulaSet();
            Assert.IsNotNull(mfSet);
        }

        [TestMethod()]
        public void TestNewMolecularFormulaSet_IMolecularFormula()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IMolecularFormulaSet mfSet = builder.CreateMolecularFormulaSet(builder.CreateMolecularFormula());
            Assert.IsNotNull(mfSet);
        }

        [TestMethod()]
        public void TestNewAdductFormula()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IAdductFormula af = builder.CreateAdductFormula();
            Assert.IsNotNull(af);
        }

        [TestMethod()]
        public void TestNewAdductFormula_IMolecularFormula()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IAdductFormula af = builder.CreateAdductFormula(builder.CreateMolecularFormula());
            Assert.IsNotNull(af);
        }

        [TestMethod()]
        public void TestNewTetrahedralChirality()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            IAtomContainer molecule = builder.CreateAtomContainer();
            molecule.Add(builder.CreateAtom("Cl"));
            molecule.Add(builder.CreateAtom("C"));
            molecule.Add(builder.CreateAtom("Br"));
            molecule.Add(builder.CreateAtom("I"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[4], BondOrder.Single);
            IAtom[] ligands = new IAtom[]{molecule.Atoms[4], molecule.Atoms[3], molecule.Atoms[2],
                molecule.Atoms[0]};
            ITetrahedralChirality chirality = builder.CreateTetrahedralChirality(molecule.Atoms[1],
                    ligands, TetrahedralStereo.Clockwise);
            Assert.IsNotNull(chirality);
            Assert.AreEqual(builder, chirality.Builder);
        }

        //[TestMethod()]
        //public void TestSugggestion()
        //{
        //    IChemObjectBuilder builder = RootObject.Builder;
        //    try
        //    {
        //        builder.CreateAtom(true);
        //        Assert.Fail("I expected an exception, because this constructor does not exist.");
        //    }
        //    catch (Exception exception)
        //    {
        //        string message = exception.Message;
        //        Assert.IsTrue("But got this message instead: " + message, message.Contains("candidates are"));
        //    }
        //}

        [TestMethod()]
        public void TestSubstance()
        {
            IChemObjectBuilder builder = RootObject.Builder;
            ISubstance substance = builder.CreateSubstance();
            Assert.IsNotNull(substance);
        }
    }
}
