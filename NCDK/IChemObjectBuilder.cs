/* Copyright (C) 2009-2010  Egon Willighagen <egonw@users.sf.net>
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

using System.Collections.Generic;
using NCDK.Numerics;

namespace NCDK
{
    /// <summary>
    /// A helper class to instantiate a <see cref="ICDKObject"/> instance for a specific implementation.
    /// </summary>
    // @author        egonw
    // @cdk.module    interfaces
    // @cdk.githash
    public interface IChemObjectBuilder
    {
        IAdductFormula CreateAdductFormula();
        IAdductFormula CreateAdductFormula(IMolecularFormula formula);
        IAminoAcid CreateAminoAcid();
        IAtom CreateAtom();
        IAtom CreateAtom(IElement element);
        IAtom CreateAtom(int elem);
        IAtom CreateAtom(int elem, int hcnt);
        IAtom CreateAtom(int elem, int hcnt, int fchg);
        IAtom CreateAtom(string elementSymbol);
        IAtom CreateAtom(string elementSymbol, Vector2 point2d);
        IAtom CreateAtom(string elementSymbol, Vector3 point3d);
        IAtomContainerSet<T> CreateAtomContainerSet<T>() where T : IAtomContainer;
        IAtomContainer CreateAtomContainer();
        IAtomContainer CreateAtomContainer(IAtomContainer container);
        IAtomContainer CreateAtomContainer(IEnumerable<IAtom> atoms, IEnumerable<IBond> bonds);
        IAtomContainerSet<IAtomContainer> CreateAtomContainerSet();
        IAtomType CreateAtomType(IElement element);
        IAtomType CreateAtomType(string elementSymbol);
        IAtomType CreateAtomType(string identifier, string elementSymbol);
        IBioPolymer CreateBioPolymer();
        IBond CreateBond();
        IBond CreateBond(IAtom atom1, IAtom atom2);
        IBond CreateBond(IAtom atom1, IAtom atom2, BondOrder order);
        IBond CreateBond(IEnumerable<IAtom> atoms);
        IBond CreateBond(IEnumerable<IAtom> atoms, BondOrder order);
        IBond CreateBond(IAtom atom1, IAtom atom2, BondOrder order, BondStereo stereo);
        IBond CreateBond(IEnumerable<IAtom> atoms, BondOrder order, BondStereo stereo);
        IChemFile CreateChemFile();
        IChemModel CreateChemModel();
        IChemObject CreateChemObject();
        IChemObject CreateChemObject(IChemObject chemObject);
        IChemSequence CreateChemSequence();
        ICrystal CreateCrystal();
        ICrystal CreateCrystal(IAtomContainer container);
        IElectronContainer CreateElectronContainer();
        IElement CreateElement();
        IElement CreateElement(IElement element);
        IElement CreateElement(string symbol);
        IElement CreateElement(string symbol, int? atomicNumber);
        IFragmentAtom CreateFragmentAtom();
        ILonePair CreateLonePair();
        ILonePair CreateLonePair(IAtom atom);
        IIsotope CreateIsotope(string elementSymbol);
        IIsotope CreateIsotope(int atomicNumber, string elementSymbol, int massNumber, double exactMass, double abundance);
        IIsotope CreateIsotope(int atomicNumber, string elementSymbol, double exactMass, double abundance);
        IIsotope CreateIsotope(string elementSymbol, int massNumber);
        IIsotope CreateIsotope(IElement element);
        IMapping CreateMapping(IChemObject objectOne, IChemObject objectTwo);
        IMolecularFormula CreateMolecularFormula();
        IMolecularFormulaSet CreateMolecularFormulaSet();
        IMolecularFormulaSet CreateMolecularFormulaSet(IMolecularFormula formula);
        IMonomer CreateMonomer();
        IPseudoAtom CreatePseudoAtom();
        IPseudoAtom CreatePseudoAtom(string label);
        IPseudoAtom CreatePseudoAtom(IElement element);
        IPseudoAtom CreatePseudoAtom(string label, Vector2 point2d);
        IPseudoAtom CreatePseudoAtom(string label, Vector3 point3d);
        IReaction CreateReaction();
        IReactionSet CreateReactionSet();
        IReactionScheme CreateReactionScheme();
        IPDBAtom CreatePDBAtom(IElement element);
        IPDBAtom CreatePDBAtom(string symbol);
        IPDBAtom CreatePDBAtom(string symbol, Vector3 coordinate);
        IPDBMonomer CreatePDBMonomer();
        IPDBPolymer CreatePDBPolymer();
        IPDBStructure CreatePDBStructure();
        IPolymer CreatePolymer();
        IRing CreateRing();
        IRing CreateRing(int ringSize, string elementSymbol);
        IRing CreateRing(IAtomContainer atomContainer);
        IRing CreateRing(IEnumerable<IAtom> atoms, IEnumerable<IBond> bonds);
        IRingSet CreateRingSet();
        ISubstance CreateSubstance();
        ISingleElectron CreateSingleElectron();
        ISingleElectron CreateSingleElectron(IAtom atom);
        IStrand CreateStrand();
        ITetrahedralChirality CreateTetrahedralChirality(IAtom chiralAtom, IEnumerable<IAtom> ligandAtoms, TetrahedralStereo chirality);
    }
}
