



// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2016-2017  Kazuya Ujihara <ujihara.kazuya@gmail.com>

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
using NCDK.Numerics;
using NCDK.Stereo;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NCDK.Default
{
    /// <summary>
    /// A factory class to provide implementation independent <see cref="ICDKObject"/>s.
    /// </summary>
    /// <example>
    /// <code>    
    ///     IChemObjectBuilder builder = ChemObjectBuilder.Instance;
    /// 
    ///     IAtom a = builder.NewAtom();
    ///     IAtom c12 = builder.NewAtom("C");
    ///     IAtom c13 = builder.NewAtom(builder.NewIsotope("C", 13));
    /// </code>
    /// </example>
    // @author        egonw
    // @author        john may
    // @cdk.module    data
    // @cdk.githash 
    public sealed class ChemObjectBuilder
        : IChemObjectBuilder
    {
        public static IChemObjectBuilder Instance { get; } = new ChemObjectBuilder();

        private bool LegacyAtomContainer { get; set; }

        internal ChemObjectBuilder()
        {
            var val = System.Environment.GetEnvironmentVariable("NCDKUseLegacyAtomContainer");
            if (string.IsNullOrWhiteSpace(val))
                LegacyAtomContainer = false;
            else
            {
                val = val.Trim();
                switch (val.ToUpperInvariant())
                {
                    case "T":
                    case "TRUE":
                    case "1":
                        LegacyAtomContainer = true;
                        break;
                    case "F":
                    case "FALSE":
                    case "0":
                        LegacyAtomContainer = false;
                        break;
                    default:
                        throw new InvalidOperationException("Invalid value, expected true/false: " + val);
                }
            }

			if (LegacyAtomContainer)
				Trace.TraceError("[WARN] Using the old AtomContainer implementation.");
        }

        internal ChemObjectBuilder(bool legacyAtomContainer)
        {
            this.LegacyAtomContainer = legacyAtomContainer;
        }

#pragma warning disable CA1822
        public T New<T>() where T : IAtomContainer, new() => new T();
#pragma warning restore

        // elements
        public IAtom NewAtom() => new Atom();
        public IAtom NewAtom(IElement element) => new Atom(element);
        public IAtom NewAtom(int elem) => new Atom(elem);
        public IAtom NewAtom(int elem, int hcnt) => new Atom(elem, hcnt);
        public IAtom NewAtom(int elem, int hcnt, int fchg) => new Atom(elem, hcnt, fchg);
        public IAtom NewAtom(string elementSymbol) => new Atom(elementSymbol);
        public IAtom NewAtom(string elementSymbol, Vector2 point2d) => new Atom(elementSymbol, point2d);
        public IAtom NewAtom(string elementSymbol, Vector3 point3d) => new Atom(elementSymbol, point3d);
        public IPseudoAtom NewPseudoAtom() => new PseudoAtom();
        public IPseudoAtom NewPseudoAtom(string label) => new PseudoAtom(label);
        public IPseudoAtom NewPseudoAtom(IElement element) => new PseudoAtom(element);
        public IPseudoAtom NewPseudoAtom(string label, Vector2 point2d) => new PseudoAtom(label, point2d);
        public IPseudoAtom NewPseudoAtom(string label, Vector3 point3d) => new PseudoAtom(label, point3d);
        public IElement NewElement() => new Element();
        public IElement NewElement(IElement element) => new Element(element);
        public IElement NewElement(string symbol) => new Element(symbol);
        public IElement NewElement(string symbol, int? atomicNumber) => new Element(symbol, atomicNumber);
        public IAtomType NewAtomType(IElement element) => new AtomType(element);
        public IAtomType NewAtomType(string elementSymbol) => new AtomType(elementSymbol);
        public IAtomType NewAtomType(string identifier, string elementSymbol) => new AtomType(identifier, elementSymbol);
        public IFragmentAtom NewFragmentAtom() => new FragmentAtom();
        public IPDBAtom NewPDBAtom(IElement element) => new PDBAtom(element);
        public IPDBAtom NewPDBAtom(string symbol) => new PDBAtom(symbol);
        public IPDBAtom NewPDBAtom(string symbol, Vector3 coordinate) => new PDBAtom(symbol, coordinate);
        public IIsotope NewIsotope(string elementSymbol) => new Isotope(elementSymbol);
        public IIsotope NewIsotope(int atomicNumber, string elementSymbol, int massNumber, double exactMass, double abundance) => new Isotope(atomicNumber, elementSymbol, massNumber, exactMass, abundance);
        public IIsotope NewIsotope(int atomicNumber, string elementSymbol, double exactMass, double abundance) => new Isotope(atomicNumber, elementSymbol, exactMass, abundance);
        public IIsotope NewIsotope(string elementSymbol, int massNumber) => new Isotope(elementSymbol, massNumber);
        public IIsotope NewIsotope(IElement element) => new Isotope(element);

        // electron containers
        public IBond NewBond() => new Bond();
        public IBond NewBond(IAtom atom1, IAtom atom2) => new Bond(atom1, atom2);
        public IBond NewBond(IAtom atom1, IAtom atom2, BondOrder order) => new Bond(atom1, atom2, order);
        public IBond NewBond(IAtom atom1, IAtom atom2, BondOrder order, BondStereo stereo) => new Bond(atom1, atom2, order, stereo);
        public IBond NewBond(IEnumerable<IAtom> atoms) => new Bond(atoms);
        public IBond NewBond(IEnumerable<IAtom> atoms, BondOrder order) => new Bond(atoms, order);
        public IBond NewBond(IEnumerable<IAtom> atoms, BondOrder order, BondStereo stereo) => new Bond(atoms, order, stereo);
        public IElectronContainer NewElectronContainer() => new ElectronContainer();
        public ISingleElectron NewSingleElectron() => new SingleElectron();
        public ISingleElectron NewSingleElectron(IAtom atom) => new SingleElectron(atom);
        public ILonePair NewLonePair() => new LonePair();
        public ILonePair NewLonePair(IAtom atom) => new LonePair(atom);
        
        // atom containers
        public IAtomContainer NewAtomContainer() => LegacyAtomContainer ? (IAtomContainer)new AtomContainer() : (IAtomContainer)new AtomContainer2();
        public IAtomContainer NewAtomContainer(IAtomContainer container) => LegacyAtomContainer ? (IAtomContainer) new AtomContainer(container) : (IAtomContainer)new AtomContainer2(container);
        public IAtomContainer NewAtomContainer(IEnumerable<IAtom> atoms, IEnumerable<IBond> bonds) => LegacyAtomContainer ? (IAtomContainer) new AtomContainer(atoms, bonds) : (IAtomContainer)new AtomContainer2(atoms, bonds);
        public IRing NewRing() => new Ring();
        public IRing NewRing(int ringSize, string elementSymbol) => new Ring(ringSize, elementSymbol);
        public IRing NewRing(IAtomContainer atomContainer) => new Ring(atomContainer);
        public IRing NewRing(IEnumerable<IAtom> atoms, IEnumerable<IBond> bonds) => new Ring(atoms, bonds); 
        public ICrystal NewCrystal() => new Crystal();
        public ICrystal NewCrystal(IAtomContainer container) => new Crystal(container);
        public IPolymer NewPolymer() => new Polymer();
        public IPDBPolymer NewPDBPolymer() => new PDBPolymer();
        public IMonomer NewMonomer() => new Monomer();
        public IPDBMonomer NewPDBMonomer() => new PDBMonomer();
        public IBioPolymer NewBioPolymer() => new BioPolymer();
        public IPDBStructure NewPDBStructure() => new PDBStructure();
        public IAminoAcid NewAminoAcid() => new AminoAcid();
        public IStrand NewStrand() => new Strand();

        // reactions
        public IReaction NewReaction() => new Reaction();
        public IReactionScheme NewReactionScheme() => new ReactionScheme();

        // formula
        public IMolecularFormula NewMolecularFormula() => new MolecularFormula();
        public IAdductFormula NewAdductFormula() => new AdductFormula();
        public IAdductFormula NewAdductFormula(IMolecularFormula formula) => new AdductFormula(formula);

        // chem object sets
        public IChemObjectSet<T> NewChemObjectSet<T>() where T : IAtomContainer => new ChemObjectSet<T>();
        public IAtomContainerSet NewAtomContainerSet() => new AtomContainerSet();
        public IMolecularFormulaSet NewMolecularFormulaSet() => new MolecularFormulaSet();
        public IMolecularFormulaSet NewMolecularFormulaSet(IMolecularFormula formula) => new MolecularFormulaSet(formula);
        public IReactionSet NewReactionSet() => new ReactionSet();
        public IRingSet NewRingSet() => new RingSet();
        public IChemModel NewChemModel() => new ChemModel();
        public IChemFile NewChemFile() => new ChemFile();
        public IChemSequence NewChemSequence() => new ChemSequence();
        public ISubstance NewSubstance() => new Substance();
        
        // stereo components (requires some modification after instantiation)
        public ITetrahedralChirality NewTetrahedralChirality(IAtom chiralAtom, IReadOnlyList<IAtom> ligandAtoms, TetrahedralStereo chirality)
        {
            var o = new TetrahedralChirality(chiralAtom, ligandAtoms, chirality)
            {
                Builder = this,
            };
            return o;
        }

        public IDoubleBondStereochemistry CreateDoubleBondStereochemistry(IBond stereoBond, IEnumerable<IBond> ligandBonds, DoubleBondConformation stereo)
        {
            var o = new DoubleBondStereochemistry(stereoBond, ligandBonds, stereo)
            {
                Builder = this,
            };
            return o;
        }

        // miscellaneous
        public IMapping NewMapping(IChemObject objectOne, IChemObject objectTwo) => new Mapping(objectOne, objectTwo);
        public IChemObject NewChemObject() => new ChemObject();
        public IChemObject NewChemObject(IChemObject chemObject) => new ChemObject(chemObject);
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
    ///     IAtom a = builder.NewAtom();
    ///     IAtom c12 = builder.NewAtom("C");
    ///     IAtom c13 = builder.NewAtom(builder.NewIsotope("C", 13));
    /// </code>
    /// </example>
    // @author        egonw
    // @author        john may
    // @cdk.module    data
    // @cdk.githash 
    public sealed class ChemObjectBuilder
        : IChemObjectBuilder
    {
        public static IChemObjectBuilder Instance { get; } = new ChemObjectBuilder();

        private bool LegacyAtomContainer { get; set; }

        internal ChemObjectBuilder()
        {
            var val = System.Environment.GetEnvironmentVariable("NCDKUseLegacyAtomContainer");
            if (string.IsNullOrWhiteSpace(val))
                LegacyAtomContainer = false;
            else
            {
                val = val.Trim();
                switch (val.ToUpperInvariant())
                {
                    case "T":
                    case "TRUE":
                    case "1":
                        LegacyAtomContainer = true;
                        break;
                    case "F":
                    case "FALSE":
                    case "0":
                        LegacyAtomContainer = false;
                        break;
                    default:
                        throw new InvalidOperationException("Invalid value, expected true/false: " + val);
                }
            }

			if (LegacyAtomContainer)
				Trace.TraceError("[WARN] Using the old AtomContainer implementation.");
        }

        internal ChemObjectBuilder(bool legacyAtomContainer)
        {
            this.LegacyAtomContainer = legacyAtomContainer;
        }

#pragma warning disable CA1822
        public T New<T>() where T : IAtomContainer, new() => new T();
#pragma warning restore

        // elements
        public IAtom NewAtom() => new Atom();
        public IAtom NewAtom(IElement element) => new Atom(element);
        public IAtom NewAtom(int elem) => new Atom(elem);
        public IAtom NewAtom(int elem, int hcnt) => new Atom(elem, hcnt);
        public IAtom NewAtom(int elem, int hcnt, int fchg) => new Atom(elem, hcnt, fchg);
        public IAtom NewAtom(string elementSymbol) => new Atom(elementSymbol);
        public IAtom NewAtom(string elementSymbol, Vector2 point2d) => new Atom(elementSymbol, point2d);
        public IAtom NewAtom(string elementSymbol, Vector3 point3d) => new Atom(elementSymbol, point3d);
        public IPseudoAtom NewPseudoAtom() => new PseudoAtom();
        public IPseudoAtom NewPseudoAtom(string label) => new PseudoAtom(label);
        public IPseudoAtom NewPseudoAtom(IElement element) => new PseudoAtom(element);
        public IPseudoAtom NewPseudoAtom(string label, Vector2 point2d) => new PseudoAtom(label, point2d);
        public IPseudoAtom NewPseudoAtom(string label, Vector3 point3d) => new PseudoAtom(label, point3d);
        public IElement NewElement() => new Element();
        public IElement NewElement(IElement element) => new Element(element);
        public IElement NewElement(string symbol) => new Element(symbol);
        public IElement NewElement(string symbol, int? atomicNumber) => new Element(symbol, atomicNumber);
        public IAtomType NewAtomType(IElement element) => new AtomType(element);
        public IAtomType NewAtomType(string elementSymbol) => new AtomType(elementSymbol);
        public IAtomType NewAtomType(string identifier, string elementSymbol) => new AtomType(identifier, elementSymbol);
        public IFragmentAtom NewFragmentAtom() => new FragmentAtom();
        public IPDBAtom NewPDBAtom(IElement element) => new PDBAtom(element);
        public IPDBAtom NewPDBAtom(string symbol) => new PDBAtom(symbol);
        public IPDBAtom NewPDBAtom(string symbol, Vector3 coordinate) => new PDBAtom(symbol, coordinate);
        public IIsotope NewIsotope(string elementSymbol) => new Isotope(elementSymbol);
        public IIsotope NewIsotope(int atomicNumber, string elementSymbol, int massNumber, double exactMass, double abundance) => new Isotope(atomicNumber, elementSymbol, massNumber, exactMass, abundance);
        public IIsotope NewIsotope(int atomicNumber, string elementSymbol, double exactMass, double abundance) => new Isotope(atomicNumber, elementSymbol, exactMass, abundance);
        public IIsotope NewIsotope(string elementSymbol, int massNumber) => new Isotope(elementSymbol, massNumber);
        public IIsotope NewIsotope(IElement element) => new Isotope(element);

        // electron containers
        public IBond NewBond() => new Bond();
        public IBond NewBond(IAtom atom1, IAtom atom2) => new Bond(atom1, atom2);
        public IBond NewBond(IAtom atom1, IAtom atom2, BondOrder order) => new Bond(atom1, atom2, order);
        public IBond NewBond(IAtom atom1, IAtom atom2, BondOrder order, BondStereo stereo) => new Bond(atom1, atom2, order, stereo);
        public IBond NewBond(IEnumerable<IAtom> atoms) => new Bond(atoms);
        public IBond NewBond(IEnumerable<IAtom> atoms, BondOrder order) => new Bond(atoms, order);
        public IBond NewBond(IEnumerable<IAtom> atoms, BondOrder order, BondStereo stereo) => new Bond(atoms, order, stereo);
        public IElectronContainer NewElectronContainer() => new ElectronContainer();
        public ISingleElectron NewSingleElectron() => new SingleElectron();
        public ISingleElectron NewSingleElectron(IAtom atom) => new SingleElectron(atom);
        public ILonePair NewLonePair() => new LonePair();
        public ILonePair NewLonePair(IAtom atom) => new LonePair(atom);
        
        // atom containers
        public IAtomContainer NewAtomContainer() => LegacyAtomContainer ? (IAtomContainer)new AtomContainer() : (IAtomContainer)new AtomContainer2();
        public IAtomContainer NewAtomContainer(IAtomContainer container) => LegacyAtomContainer ? (IAtomContainer) new AtomContainer(container) : (IAtomContainer)new AtomContainer2(container);
        public IAtomContainer NewAtomContainer(IEnumerable<IAtom> atoms, IEnumerable<IBond> bonds) => LegacyAtomContainer ? (IAtomContainer) new AtomContainer(atoms, bonds) : (IAtomContainer)new AtomContainer2(atoms, bonds);
        public IRing NewRing() => new Ring();
        public IRing NewRing(int ringSize, string elementSymbol) => new Ring(ringSize, elementSymbol);
        public IRing NewRing(IAtomContainer atomContainer) => new Ring(atomContainer);
        public IRing NewRing(IEnumerable<IAtom> atoms, IEnumerable<IBond> bonds) => new Ring(atoms, bonds); 
        public ICrystal NewCrystal() => new Crystal();
        public ICrystal NewCrystal(IAtomContainer container) => new Crystal(container);
        public IPolymer NewPolymer() => new Polymer();
        public IPDBPolymer NewPDBPolymer() => new PDBPolymer();
        public IMonomer NewMonomer() => new Monomer();
        public IPDBMonomer NewPDBMonomer() => new PDBMonomer();
        public IBioPolymer NewBioPolymer() => new BioPolymer();
        public IPDBStructure NewPDBStructure() => new PDBStructure();
        public IAminoAcid NewAminoAcid() => new AminoAcid();
        public IStrand NewStrand() => new Strand();

        // reactions
        public IReaction NewReaction() => new Reaction();
        public IReactionScheme NewReactionScheme() => new ReactionScheme();

        // formula
        public IMolecularFormula NewMolecularFormula() => new MolecularFormula();
        public IAdductFormula NewAdductFormula() => new AdductFormula();
        public IAdductFormula NewAdductFormula(IMolecularFormula formula) => new AdductFormula(formula);

        // chem object sets
        public IChemObjectSet<T> NewChemObjectSet<T>() where T : IAtomContainer => new ChemObjectSet<T>();
        public IAtomContainerSet NewAtomContainerSet() => new AtomContainerSet();
        public IMolecularFormulaSet NewMolecularFormulaSet() => new MolecularFormulaSet();
        public IMolecularFormulaSet NewMolecularFormulaSet(IMolecularFormula formula) => new MolecularFormulaSet(formula);
        public IReactionSet NewReactionSet() => new ReactionSet();
        public IRingSet NewRingSet() => new RingSet();
        public IChemModel NewChemModel() => new ChemModel();
        public IChemFile NewChemFile() => new ChemFile();
        public IChemSequence NewChemSequence() => new ChemSequence();
        public ISubstance NewSubstance() => new Substance();
        
        // stereo components (requires some modification after instantiation)
        public ITetrahedralChirality NewTetrahedralChirality(IAtom chiralAtom, IReadOnlyList<IAtom> ligandAtoms, TetrahedralStereo chirality)
        {
            var o = new TetrahedralChirality(chiralAtom, ligandAtoms, chirality)
            {
                Builder = this,
            };
            return o;
        }

        public IDoubleBondStereochemistry CreateDoubleBondStereochemistry(IBond stereoBond, IEnumerable<IBond> ligandBonds, DoubleBondConformation stereo)
        {
            var o = new DoubleBondStereochemistry(stereoBond, ligandBonds, stereo)
            {
                Builder = this,
            };
            return o;
        }

        // miscellaneous
        public IMapping NewMapping(IChemObject objectOne, IChemObject objectTwo) => new Mapping(objectOne, objectTwo);
        public IChemObject NewChemObject() => new ChemObject();
        public IChemObject NewChemObject(IChemObject chemObject) => new ChemObject(chemObject);
    }
}
