

// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2016-2017  Kazuya Ujihara <uzzy@users.sourceforge.net>

/* Copyright (C) 2010  Egon Willighagen <egonw@users.sf.net>
 *               2012  John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Formula;
using NCDK.Stereo;
using System.Collections.Generic;
using NCDK.Numerics;

namespace NCDK.Default
{
	/// <summary>
	/// A factory class to provide implementation independent <see cref="ICDKObject"/>s.
	/// </summary>
	/// <example>
	/// <code>    
	///     IChemObjectBuilder builder = ChemObjectBuilder.Instance;
	/// 
	///     IAtom a = builder.CreateAtom();
	///     IAtom c12 = builder.CreateAtom("C");
	///     IAtom c13 = builder.CreateAtom(builder.CreateIsotope("C", 13));
	/// </code>
	/// </example>
	// @author        egonw
	// @author        john may
	// @cdk.module    data
	// @cdk.githash 
    public sealed class ChemObjectBuilder
        : IChemObjectBuilder
    {
        public static readonly IChemObjectBuilder Instance = new ChemObjectBuilder();

        public T Create<T>() where T : IAtomContainer, new() => new T();

        // elements
        public IAtom CreateAtom() => new Atom();
        public IAtom CreateAtom(IElement element) => new Atom(element);
        public IAtom CreateAtom(string elementSymbol) => new Atom(elementSymbol);
        public IAtom CreateAtom(string elementSymbol, Vector2 point2d) => new Atom(elementSymbol, point2d);
        public IAtom CreateAtom(string elementSymbol, Vector3 point3d) => new Atom(elementSymbol, point3d);
        public IPseudoAtom CreatePseudoAtom() => new PseudoAtom();
        public IPseudoAtom CreatePseudoAtom(string label) => new PseudoAtom(label);
        public IPseudoAtom CreatePseudoAtom(IElement element) => new PseudoAtom(element);
        public IPseudoAtom CreatePseudoAtom(string label, Vector2 point2d) => new PseudoAtom(label, point2d);
        public IPseudoAtom CreatePseudoAtom(string label, Vector3 point3d) => new PseudoAtom(label, point3d);
        public IElement CreateElement() => new Element();
        public IElement CreateElement(IElement element) => new Element(element);
        public IElement CreateElement(string symbol) => new Element(symbol);
        public IElement CreateElement(string symbol, int? atomicNumber) => new Element(symbol, atomicNumber);
        public IAtomType CreateAtomType(IElement element) => new AtomType(element);
        public IAtomType CreateAtomType(string elementSymbol) => new AtomType(elementSymbol);
        public IAtomType CreateAtomType(string identifier, string elementSymbol) => new AtomType(identifier, elementSymbol);
        public IFragmentAtom CreateFragmentAtom() => new FragmentAtom();
        public IPDBAtom CreatePDBAtom(IElement element) => new PDBAtom(element);
        public IPDBAtom CreatePDBAtom(string symbol) => new PDBAtom(symbol);
        public IPDBAtom CreatePDBAtom(string symbol, Vector3 coordinate) => new PDBAtom(symbol, coordinate);
        public IIsotope CreateIsotope(string elementSymbol) => new Isotope(elementSymbol);
        public IIsotope CreateIsotope(int atomicNumber, string elementSymbol, int massNumber, double exactMass, double abundance) => new Isotope(atomicNumber, elementSymbol, massNumber, exactMass, abundance);
        public IIsotope CreateIsotope(int atomicNumber, string elementSymbol, double exactMass, double abundance) => new Isotope(atomicNumber, elementSymbol, exactMass, abundance);
        public IIsotope CreateIsotope(string elementSymbol, int massNumber) => new Isotope(elementSymbol, massNumber);
        public IIsotope CreateIsotope(IElement element) => new Isotope(element);

		// electron containers
        public IBond CreateBond() => new Bond();
        public IBond CreateBond(IAtom atom1, IAtom atom2) => new Bond(atom1, atom2);
        public IBond CreateBond(IAtom atom1, IAtom atom2, BondOrder order) => new Bond(atom1, atom2, order);
        public IBond CreateBond(IAtom atom1, IAtom atom2, BondOrder order, BondStereo stereo) => new Bond(atom1, atom2, order, stereo);
        public IBond CreateBond(IEnumerable<IAtom> atoms) => new Bond(atoms);
        public IBond CreateBond(IEnumerable<IAtom> atoms, BondOrder order) => new Bond(atoms, order);
        public IBond CreateBond(IEnumerable<IAtom> atoms, BondOrder order, BondStereo stereo) => new Bond(atoms, order, stereo);
        public IElectronContainer CreateElectronContainer() => new ElectronContainer();
        public ISingleElectron CreateSingleElectron() => new SingleElectron();
        public ISingleElectron CreateSingleElectron(IAtom atom) => new SingleElectron(atom);
        public ILonePair CreateLonePair() => new LonePair();
        public ILonePair CreateLonePair(IAtom atom) => new LonePair(atom);
		
        // atom containers
        public IAtomContainer CreateAtomContainer() => new AtomContainer();
		public IAtomContainer CreateAtomContainer(IAtomContainer container) => new AtomContainer(container);
        public IAtomContainer CreateAtomContainer(IEnumerable<IAtom> atoms, IEnumerable<IBond> bonds) => new AtomContainer(atoms, bonds);
        public IRing CreateRing() => new Ring();
        public IRing CreateRing(int ringSize, string elementSymbol) => new Ring(ringSize, elementSymbol);
        public IRing CreateRing(IAtomContainer atomContainer) => new Ring(atomContainer);
        public IRing CreateRing(IEnumerable<IAtom> atoms, IEnumerable<IBond> bonds) => new Ring(atoms, bonds); 
        public ICrystal CreateCrystal() => new Crystal();
        public ICrystal CreateCrystal(IAtomContainer container) => new Crystal(container);
        public IPolymer CreatePolymer() => new Polymer();
        public IPDBPolymer CreatePDBPolymer() => new PDBPolymer();
        public IMonomer CreateMonomer() => new Monomer();
        public IPDBMonomer CreatePDBMonomer() => new PDBMonomer();
        public IBioPolymer CreateBioPolymer() => new BioPolymer();
        public IPDBStructure CreatePDBStructure() => new PDBStructure();
        public IAminoAcid CreateAminoAcid() => new AminoAcid();
        public IStrand CreateStrand() => new Strand();

        // reactions
        public IReaction CreateReaction() => new Reaction();
        public IReactionScheme CreateReactionScheme() => new ReactionScheme();

        // formula
        public IMolecularFormula CreateMolecularFormula() => new MolecularFormula();
        public IAdductFormula CreateAdductFormula() => new AdductFormula();
        public IAdductFormula CreateAdductFormula(IMolecularFormula formula) => new AdductFormula(formula);

        // chem object sets
        public IAtomContainerSet<T> CreateAtomContainerSet<T>() where T : IAtomContainer => new AtomContainerSet<T>();
        public IAtomContainerSet<IAtomContainer> CreateAtomContainerSet() => new AtomContainerSet<IAtomContainer>();
        public IMolecularFormulaSet CreateMolecularFormulaSet() => new MolecularFormulaSet();
        public IMolecularFormulaSet CreateMolecularFormulaSet(IMolecularFormula formula) => new MolecularFormulaSet(formula);
        public IReactionSet CreateReactionSet() => new ReactionSet();
        public IRingSet CreateRingSet() => new RingSet();
        public IChemModel CreateChemModel() => new ChemModel();
        public IChemFile CreateChemFile() => new ChemFile();
        public IChemSequence CreateChemSequence() => new ChemSequence();
        public ISubstance CreateSubstance() => new Substance();
		
		// stereo components (requires some modification after instantiation)
        public ITetrahedralChirality CreateTetrahedralChirality(IAtom chiralAtom, IEnumerable<IAtom> ligandAtoms, TetrahedralStereo chirality)
        {
            var o = new TetrahedralChirality(chiralAtom, ligandAtoms, chirality);
            o.Builder = this;
            return o;
        }

		public IDoubleBondStereochemistry CreateDoubleBondStereochemistry(IBond stereoBond, IEnumerable<IBond> ligandBonds, DoubleBondConformation stereo)
        {
            var o = new DoubleBondStereochemistry(stereoBond, ligandBonds, stereo);
            o.Builder = this;
            return o;
        }

		// miscellaneous
        public IMapping CreateMapping(IChemObject objectOne, IChemObject objectTwo) => new Mapping(objectOne, objectTwo);
        public IChemObject CreateChemObject() => new ChemObject();
        public IChemObject CreateChemObject(IChemObject chemObject) => new ChemObject(chemObject);
    }
}
namespace NCDK.Silent
{
	/// <summary>
	/// A factory class to provide implementation independent <see cref="ICDKObject"/>s.
	/// </summary>
	/// <example>
	/// <code>    
	///     IChemObjectBuilder builder = ChemObjectBuilder.Instance;
	/// 
	///     IAtom a = builder.CreateAtom();
	///     IAtom c12 = builder.CreateAtom("C");
	///     IAtom c13 = builder.CreateAtom(builder.CreateIsotope("C", 13));
	/// </code>
	/// </example>
	// @author        egonw
	// @author        john may
	// @cdk.module    data
	// @cdk.githash 
    public sealed class ChemObjectBuilder
        : IChemObjectBuilder
    {
        public static readonly IChemObjectBuilder Instance = new ChemObjectBuilder();

        public T Create<T>() where T : IAtomContainer, new() => new T();

        // elements
        public IAtom CreateAtom() => new Atom();
        public IAtom CreateAtom(IElement element) => new Atom(element);
        public IAtom CreateAtom(string elementSymbol) => new Atom(elementSymbol);
        public IAtom CreateAtom(string elementSymbol, Vector2 point2d) => new Atom(elementSymbol, point2d);
        public IAtom CreateAtom(string elementSymbol, Vector3 point3d) => new Atom(elementSymbol, point3d);
        public IPseudoAtom CreatePseudoAtom() => new PseudoAtom();
        public IPseudoAtom CreatePseudoAtom(string label) => new PseudoAtom(label);
        public IPseudoAtom CreatePseudoAtom(IElement element) => new PseudoAtom(element);
        public IPseudoAtom CreatePseudoAtom(string label, Vector2 point2d) => new PseudoAtom(label, point2d);
        public IPseudoAtom CreatePseudoAtom(string label, Vector3 point3d) => new PseudoAtom(label, point3d);
        public IElement CreateElement() => new Element();
        public IElement CreateElement(IElement element) => new Element(element);
        public IElement CreateElement(string symbol) => new Element(symbol);
        public IElement CreateElement(string symbol, int? atomicNumber) => new Element(symbol, atomicNumber);
        public IAtomType CreateAtomType(IElement element) => new AtomType(element);
        public IAtomType CreateAtomType(string elementSymbol) => new AtomType(elementSymbol);
        public IAtomType CreateAtomType(string identifier, string elementSymbol) => new AtomType(identifier, elementSymbol);
        public IFragmentAtom CreateFragmentAtom() => new FragmentAtom();
        public IPDBAtom CreatePDBAtom(IElement element) => new PDBAtom(element);
        public IPDBAtom CreatePDBAtom(string symbol) => new PDBAtom(symbol);
        public IPDBAtom CreatePDBAtom(string symbol, Vector3 coordinate) => new PDBAtom(symbol, coordinate);
        public IIsotope CreateIsotope(string elementSymbol) => new Isotope(elementSymbol);
        public IIsotope CreateIsotope(int atomicNumber, string elementSymbol, int massNumber, double exactMass, double abundance) => new Isotope(atomicNumber, elementSymbol, massNumber, exactMass, abundance);
        public IIsotope CreateIsotope(int atomicNumber, string elementSymbol, double exactMass, double abundance) => new Isotope(atomicNumber, elementSymbol, exactMass, abundance);
        public IIsotope CreateIsotope(string elementSymbol, int massNumber) => new Isotope(elementSymbol, massNumber);
        public IIsotope CreateIsotope(IElement element) => new Isotope(element);

		// electron containers
        public IBond CreateBond() => new Bond();
        public IBond CreateBond(IAtom atom1, IAtom atom2) => new Bond(atom1, atom2);
        public IBond CreateBond(IAtom atom1, IAtom atom2, BondOrder order) => new Bond(atom1, atom2, order);
        public IBond CreateBond(IAtom atom1, IAtom atom2, BondOrder order, BondStereo stereo) => new Bond(atom1, atom2, order, stereo);
        public IBond CreateBond(IEnumerable<IAtom> atoms) => new Bond(atoms);
        public IBond CreateBond(IEnumerable<IAtom> atoms, BondOrder order) => new Bond(atoms, order);
        public IBond CreateBond(IEnumerable<IAtom> atoms, BondOrder order, BondStereo stereo) => new Bond(atoms, order, stereo);
        public IElectronContainer CreateElectronContainer() => new ElectronContainer();
        public ISingleElectron CreateSingleElectron() => new SingleElectron();
        public ISingleElectron CreateSingleElectron(IAtom atom) => new SingleElectron(atom);
        public ILonePair CreateLonePair() => new LonePair();
        public ILonePair CreateLonePair(IAtom atom) => new LonePair(atom);
		
        // atom containers
        public IAtomContainer CreateAtomContainer() => new AtomContainer();
		public IAtomContainer CreateAtomContainer(IAtomContainer container) => new AtomContainer(container);
        public IAtomContainer CreateAtomContainer(IEnumerable<IAtom> atoms, IEnumerable<IBond> bonds) => new AtomContainer(atoms, bonds);
        public IRing CreateRing() => new Ring();
        public IRing CreateRing(int ringSize, string elementSymbol) => new Ring(ringSize, elementSymbol);
        public IRing CreateRing(IAtomContainer atomContainer) => new Ring(atomContainer);
        public IRing CreateRing(IEnumerable<IAtom> atoms, IEnumerable<IBond> bonds) => new Ring(atoms, bonds); 
        public ICrystal CreateCrystal() => new Crystal();
        public ICrystal CreateCrystal(IAtomContainer container) => new Crystal(container);
        public IPolymer CreatePolymer() => new Polymer();
        public IPDBPolymer CreatePDBPolymer() => new PDBPolymer();
        public IMonomer CreateMonomer() => new Monomer();
        public IPDBMonomer CreatePDBMonomer() => new PDBMonomer();
        public IBioPolymer CreateBioPolymer() => new BioPolymer();
        public IPDBStructure CreatePDBStructure() => new PDBStructure();
        public IAminoAcid CreateAminoAcid() => new AminoAcid();
        public IStrand CreateStrand() => new Strand();

        // reactions
        public IReaction CreateReaction() => new Reaction();
        public IReactionScheme CreateReactionScheme() => new ReactionScheme();

        // formula
        public IMolecularFormula CreateMolecularFormula() => new MolecularFormula();
        public IAdductFormula CreateAdductFormula() => new AdductFormula();
        public IAdductFormula CreateAdductFormula(IMolecularFormula formula) => new AdductFormula(formula);

        // chem object sets
        public IAtomContainerSet<T> CreateAtomContainerSet<T>() where T : IAtomContainer => new AtomContainerSet<T>();
        public IAtomContainerSet<IAtomContainer> CreateAtomContainerSet() => new AtomContainerSet<IAtomContainer>();
        public IMolecularFormulaSet CreateMolecularFormulaSet() => new MolecularFormulaSet();
        public IMolecularFormulaSet CreateMolecularFormulaSet(IMolecularFormula formula) => new MolecularFormulaSet(formula);
        public IReactionSet CreateReactionSet() => new ReactionSet();
        public IRingSet CreateRingSet() => new RingSet();
        public IChemModel CreateChemModel() => new ChemModel();
        public IChemFile CreateChemFile() => new ChemFile();
        public IChemSequence CreateChemSequence() => new ChemSequence();
        public ISubstance CreateSubstance() => new Substance();
		
		// stereo components (requires some modification after instantiation)
        public ITetrahedralChirality CreateTetrahedralChirality(IAtom chiralAtom, IEnumerable<IAtom> ligandAtoms, TetrahedralStereo chirality)
        {
            var o = new TetrahedralChirality(chiralAtom, ligandAtoms, chirality);
            o.Builder = this;
            return o;
        }

		public IDoubleBondStereochemistry CreateDoubleBondStereochemistry(IBond stereoBond, IEnumerable<IBond> ligandBonds, DoubleBondConformation stereo)
        {
            var o = new DoubleBondStereochemistry(stereoBond, ligandBonds, stereo);
            o.Builder = this;
            return o;
        }

		// miscellaneous
        public IMapping CreateMapping(IChemObject objectOne, IChemObject objectTwo) => new Mapping(objectOne, objectTwo);
        public IChemObject CreateChemObject() => new ChemObject();
        public IChemObject CreateChemObject(IChemObject chemObject) => new ChemObject(chemObject);
    }
}
