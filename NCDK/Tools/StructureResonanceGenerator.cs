/* Copyright (C) 2006-2007  Miguel Rojas <miguel.rojas@uni-koeln.de>
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
using NCDK.Aromaticities;
using NCDK.Isomorphisms;
using NCDK.Isomorphisms.Matchers;
using NCDK.Reactions;
using NCDK.Reactions.Types;
using NCDK.Reactions.Types.Parameters;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NCDK.Tools
{
    /// <summary>
    /// <p>This class try to generate resonance structure for a determinate molecule.</p>
    /// <p>Make sure that the molecule has the corresponding lone pair electrons
    /// for each atom. You can use the method: <code> LonePairElectronChecker </code>
    /// <p>It is needed to call the addExplicitHydrogensToSatisfyValency
    ///  from the class tools.HydrogenAdder.</p>
    /// <p>It is based on rearrangements of electrons and charge</p>
    /// <p>The method is based on call by reactions which occur in a resonance.</p>
    ///
    /// <code>
    /// StructureResonanceGenerator srG = new StructureReseonanceGenerator(true,true,true,true,false);
    /// MoleculeSet setOf = srG.GetResonances(new Molecule());
    /// </code>
    ///
    /// <p>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</p>
    /// <code>atoms[0].SetFlag(CDKConstants.REACTIVE_CENTER,true);</code>
    /// <p>Moreover you must put the parameter as true</p>
    /// <p>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</p>
    ///
    // @author       Miguel Rojas
    // @cdk.created  2006-5-05
    // @cdk.module   reaction
    // @cdk.githash
    ///
    // @see NCDK.Reactions.IReactionProcess
    /// </summary>
    public class StructureResonanceGenerator
    {
        /// <summary>Generate resonance structure without looking at the symmetry*/
        private bool lookingSymmetry;

        /// <summary>
        /// Construct an instance of StructureResonanceGenerator. Default restrictions
        /// are initiated.
        ///
        /// <seealso cref="SetDefaultReactions"/>
        /// </summary>
        public StructureResonanceGenerator()
                : this(false)
        { }

        /// <summary>
        /// Construct an instance of StructureResonanceGenerator. Default restrictions
        /// are initiated.
        ///
        /// <param name="lookingSymmetry">Specify if the resonance generation is based looking at the symmetry</param>
        /// <seealso cref="SetDefaultReactions"/>
        /// </summary>
        public StructureResonanceGenerator(bool lookingSymmetry)
        {
            Trace.TraceInformation("Initiate StructureResonanceGenerator");
            this.lookingSymmetry = lookingSymmetry;
            SetDefaultReactions();

        }

        /// <summary>
        ///  The reactions that must be used in the generation of the resonance.
        /// </summary>
        /// <seealso cref="IReactionProcess"/>
        public IList<IReactionProcess> Reactions { get; set; } = new List<IReactionProcess>();

        /// <summary>
        /// The number maximal of resonance structures to be found. The
        /// algorithm breaks the process when is came to this number.
        /// </summary>
        public int MaximalStructures { get; set; } = 50; /* TODO: REACT: some time takes too much time. At the moment fixed to 50 structures*/

        /// <summary>
        /// Set the default reactions that must be presents to generate the resonance.
        ///
        /// <seealso cref="Reactions"/>
        /// </summary>
        public void SetDefaultReactions()
        {
            CallDefaultReactions();
        }

        private void CallDefaultReactions()
        {
            List<IParameterReact> paramList = new List<IParameterReact>();
            IParameterReact param = new SetReactionCenter();
            param.IsSetParameter = false;
            paramList.Add(param);

            IReactionProcess type = new SharingLonePairReaction();
            try
            {
                type.ParameterList = paramList;
            }
            catch (CDKException e)
            {
                Console.Error.WriteLine(e.StackTrace);
            }
            Reactions.Add(type);

            type = new PiBondingMovementReaction();
            List<IParameterReact> paramList2 = new List<IParameterReact>();
            IParameterReact param2 = new SetReactionCenter();
            param2.IsSetParameter = false;
            paramList2.Add(param2);
            try
            {
                type.ParameterList = paramList2;
            }
            catch (CDKException e)
            {
                Console.Error.WriteLine(e.StackTrace);
            }
            Reactions.Add(type);

            type = new RearrangementAnionReaction();
            try
            {
                type.ParameterList = paramList;
            }
            catch (CDKException e)
            {
                Console.Error.WriteLine(e.StackTrace);
            }
            Reactions.Add(type);

            type = new RearrangementCationReaction();
            try
            {
                type.ParameterList = paramList;
            }
            catch (CDKException e)
            {
                Console.Error.WriteLine(e.StackTrace);
            }
            Reactions.Add(type);

            type = new RearrangementLonePairReaction();
            try
            {
                type.ParameterList = paramList;
            }
            catch (CDKException e)
            {
                Console.Error.WriteLine(e.StackTrace);
            }
            Reactions.Add(type);

            type = new RearrangementRadicalReaction();
            try
            {
                type.ParameterList = paramList;
            }
            catch (CDKException e)
            {
                Console.Error.WriteLine(e.StackTrace);
            }
            Reactions.Add(type);

        }

        /// <summary>
        /// Get the resonance structures from an <see cref="IAtomContainer"/>.
        ///
        /// <param name="molecule">The IAtomContainer to analyze</param>
        /// <returns>The different resonance structures</returns>
        /// </summary>
        public IAtomContainerSet<IAtomContainer> GetStructures(IAtomContainer molecule)
        {
            int countStructure = 0;
            var setOfMol = molecule.Builder.CreateAtomContainerSet();
            setOfMol.Add(molecule);

            for (int i = 0; i < setOfMol.Count; i++)
            {
                IAtomContainer mol = setOfMol[i];
                foreach (var aReactionsList in Reactions)
                {
                    IReactionProcess reaction = aReactionsList;
                    var setOfReactants = molecule.Builder.CreateAtomContainerSet();
                    setOfReactants.Add(mol);
                    try
                    {
                        IReactionSet setOfReactions = reaction.Initiate(setOfReactants, null);
                        if (setOfReactions.Count != 0)
                            for (int k = 0; k < setOfReactions.Count; k++)
                                for (int j = 0; j < setOfReactions[k].Products.Count; j++)
                                {
                                    IAtomContainer product = setOfReactions[k].Products[j];
                                    if (!ExistAC(setOfMol, product))
                                    {
                                        setOfMol.Add(product);
                                        countStructure++;
                                        if (countStructure > MaximalStructures) return setOfMol;
                                    }
                                }
                    }
                    catch (CDKException e)
                    {
                        Console.Error.WriteLine(e.StackTrace);
                    }
                }
            }
            return setOfMol;
        }

        /// <summary>
        /// Get the container which is found resonance from a <see cref="IAtomContainer"/>.
        /// It is based on looking if the order of the bond changes.
        ///
        /// <param name="molecule">The IAtomContainer to analyze</param>
        /// <returns>The different containers</returns>
        /// </summary>
        public IAtomContainerSet<IAtomContainer> GetContainers(IAtomContainer molecule)
        {
            var setOfCont = molecule.Builder.CreateAtomContainerSet();
            var setOfMol = GetStructures(molecule);

            if (setOfMol.Count == 0) return setOfCont;

            /* extraction of all bonds which has been produced a changes of order */
            List<IBond> bondList = new List<IBond>();
            for (int i = 1; i < setOfMol.Count; i++)
            {
                IAtomContainer mol = setOfMol[i];
                for (int j = 0; j < mol.Bonds.Count; j++)
                {
                    IBond bond = molecule.Bonds[j];
                    if (!mol.Bonds[j].Order.Equals(bond.Order))
                    {
                        if (!bondList.Contains(bond)) bondList.Add(bond);
                    }
                }
            }

            if (bondList.Count == 0) return null;

            int[] flagBelonging = new int[bondList.Count];
            for (int i = 0; i < flagBelonging.Length; i++)
                flagBelonging[i] = 0;
            int[] position = new int[bondList.Count];
            int maxGroup = 1;

            /* Analysis if the bond are linked together */
            List<IBond> newBondList = new List<IBond>();
            newBondList.Add(bondList[0]);

            int pos = 0;
            for (int i = 0; i < newBondList.Count; i++)
            {

                if (i == 0)
                    flagBelonging[i] = maxGroup;
                else
                {
                    if (flagBelonging[position[i]] == 0)
                    {
                        maxGroup++;
                        flagBelonging[position[i]] = maxGroup;
                    }
                }

                IBond bondA = newBondList[i];
                for (int ato = 0; ato < 2; ato++)
                {
                    IAtom atomA1 = bondA.Atoms[ato];
                    var bondA1s = molecule.GetConnectedBonds(atomA1);
                    foreach (var bondB in bondA1s)
                    {
                        if (!newBondList.Contains(bondB)) for (int k = 0; k < bondList.Count; k++)
                                if (bondList[k].Equals(bondB)) if (flagBelonging[k] == 0)
                                    {
                                        flagBelonging[k] = maxGroup;
                                        pos++;
                                        newBondList.Add(bondB);
                                        position[pos] = k;

                                    }
                    }
                }
                //if it is final size and not all are added
                if (newBondList.Count - 1 == i) for (int k = 0; k < bondList.Count; k++)
                        if (!newBondList.Contains(bondList[k]))
                        {
                            newBondList.Add(bondList[k]);
                            position[i + 1] = k;
                            break;
                        }
            }
            /* creating containers according groups */
            for (int i = 0; i < maxGroup; i++)
            {
                IAtomContainer container = molecule.Builder.CreateAtomContainer();
                for (int j = 0; j < bondList.Count; j++)
                {
                    if (flagBelonging[j] != i + 1) continue;
                    IBond bond = bondList[j];
                    IAtom atomA1 = bond.Atoms[0];
                    IAtom atomA2 = bond.Atoms[1];
                    if (!container.Contains(atomA1)) container.Atoms.Add(atomA1);
                    if (!container.Contains(atomA2)) container.Atoms.Add(atomA2);
                    container.Bonds.Add(bond);
                }
                setOfCont.Add(container);
            }
            return setOfCont;
        }

        /// <summary>
        /// Get the container which the atom is found on resonance from a <see cref="IAtomContainer"/>.
        /// It is based on looking if the order of the bond changes. Return null
        /// is any is found.
        ///
        /// <param name="molecule">The IAtomContainer to analyze</param>
        /// <param name="atom">The IAtom</param>
        /// <returns>The container with the atom</returns>
        /// </summary>
        public IAtomContainer GetContainer(IAtomContainer molecule, IAtom atom)
        {
            var setOfCont = GetContainers(molecule);
            if (setOfCont == null) return null;

            foreach (var container in setOfCont)
            {
                if (container.Contains(atom)) return container;
            }

            return null;
        }

        /// <summary>
        /// Get the container which the bond is found on resonance from a <see cref="IAtomContainer"/>.
        /// It is based on looking if the order of the bond changes. Return null
        /// is any is found.
        ///
        /// <param name="molecule">The IAtomContainer to analyze</param>
        /// <param name="bond">The IBond</param>
        /// <returns>The container with the bond</returns>
        /// </summary>
        public IAtomContainer GetContainer(IAtomContainer molecule, IBond bond)
        {
            var setOfCont = GetContainers(molecule);
            if (setOfCont == null) return null;

            foreach (var container in setOfCont)
            {
                if (container.Contains(bond)) return container;
            }

            return null;
        }

        /// <summary>
        /// Search if the setOfAtomContainer contains the atomContainer
        ///
        ///
        /// <param name="set">ISetOfAtomContainer object where to search</param>
        /// <param name="atomContainer">IAtomContainer to search</param>
        /// <returns>True, if the atomContainer is contained</returns>
        /// </summary>
        private bool ExistAC(IAtomContainerSet<IAtomContainer> set, IAtomContainer atomContainer)
        {

            IAtomContainer acClone = null;
            acClone = (IAtomContainer)atomContainer.Clone();
            if (!lookingSymmetry)
            { /* remove all aromatic flags */
                foreach (var atom in acClone.Atoms)
                    atom.IsAromatic = false;
                foreach (var bond in acClone.Bonds)
                    bond.IsAromatic = false;
            }

            for (int i = 0; i < acClone.Atoms.Count; i++)
                //            if(acClone.Atoms[i].Id == null)
                acClone.Atoms[i].Id = "" + acClone.Atoms.IndexOf(acClone.Atoms[i]);

            if (lookingSymmetry)
            {
                try
                {
                    Aromaticity.CDKLegacy.Apply(acClone);
                }
                catch (CDKException e)
                {
                    Console.Error.WriteLine(e.StackTrace);
                }
            }
            else
            {
                if (!lookingSymmetry)
                { /* remove all aromatic flags */
                    foreach (var atom in acClone.Atoms)
                        atom.IsAromatic = false;
                    foreach (var bond in acClone.Bonds)
                        bond.IsAromatic = false;
                }
            }
            for (int i = 0; i < set.Count; i++)
            {
                IAtomContainer ss = set[i];
                for (int j = 0; j < ss.Atoms.Count; j++)
                    //                if(ss.Atoms[j].Id == null)
                    ss.Atoms[j].Id = "" + ss.Atoms.IndexOf(ss.Atoms[j]);

                try
                {

                    if (!lookingSymmetry)
                    {
                        QueryAtomContainer qAC = QueryAtomContainerCreator.CreateSymbolChargeIDQueryContainer(acClone);
                        if (new UniversalIsomorphismTester().IsIsomorph(ss, qAC))
                        {
                            QueryAtomContainer qAC2 = QueryAtomContainerCreator
                                    .CreateSymbolAndBondOrderQueryContainer(acClone);
                            if (new UniversalIsomorphismTester().IsIsomorph(ss, qAC2)) return true;
                        }
                    }
                    else
                    {
                        QueryAtomContainer qAC = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(acClone);
                        Aromaticity.CDKLegacy.Apply(ss);
                        if (new UniversalIsomorphismTester().IsIsomorph(ss, qAC)) return true;
                    }

                }
                catch (CDKException e1)
                {
                    Console.Error.WriteLine(e1);
                    Trace.TraceError(e1.Message);
                    Debug.WriteLine(e1);
                }
            }
            return false;
        }
    }
}
