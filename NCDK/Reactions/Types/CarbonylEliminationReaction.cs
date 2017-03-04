/* Copyright (C) 2008 Miguel Rojas <miguelrojasch@users.sf.net>
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT Any WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Reactions.Types.Parameters;
using System.Collections.Generic;
using System.Diagnostics;

namespace NCDK.Reactions.Types
{
    /// <summary>
    /// <para>IReactionProcess which participate mass spectrum process.
    /// This reaction could be represented as RC-C#[O+] => R[C] + |C#[O+]</para>
    /// </summary>
    /// <example>
    /// Make sure that the molecule has the correspond lone pair electrons
    /// for each atom. You can use the method: 
    /// <code>LonePairElectronChecker </code>
    /// <para>It is processed by the HeterolyticCleavageMechanism class</para>
    ///
    /// <code>
    ///  IAtomContainerSet setOfReactants = Default.ChemObjectBuilder.Instance.NewAtomContainerSet();
    ///  setOfReactants.Add(new AtomContainer());
    ///  IReactionProcess type = new CarbonylEliminationReaction();
    ///  object[] parameters = {bool.FALSE};
    ///  type.Parameters = parameters;
    ///  IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].SetFlag(CDKConstants.REACTIVE_CENTER,true);</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    /// <seealso cref="HeterolyticCleavageMechanism"/>
    // @author         Miguel Rojas
    // @cdk.created    2006-10-16
    // @cdk.module     reaction
    // @cdk.githash
    // @cdk.set        reaction-types
    public class CarbonylEliminationReaction : ReactionEngine, IReactionProcess
    {
        /// <summary>
        /// Constructor of the CarbonylEliminationReaction object.
        /// </summary>
        public CarbonylEliminationReaction() { }

        /// <summary>
        ///  Gets the specification attribute of the CarbonylEliminationReaction object.
        /// </summary>
        /// <returns>The specification value</returns>
        public ReactionSpecification Specification =>
            new ReactionSpecification(
                    "http://almost.cubic.uni-koeln.de/jrg/Members/mrc/reactionDict/reactionDict#CarbonylElimination", this
                            .GetType().Name, "$Id$", "The Chemistry Development Kit");

        /// <summary>
        ///  Initiate process.
        /// </summary>
        /// <exception cref="CDKException"> Description of the Exception</exception>
        /// <param name="reactants">reactants of the reaction</param>
        /// <param name="agents">agents of the reaction (Must be in this case null)</param>
        public IReactionSet Initiate(IAtomContainerSet<IAtomContainer> reactants, IAtomContainerSet<IAtomContainer> agents)
        {
            Debug.WriteLine("initiate reaction: CarbonylEliminationReaction");

            if (reactants.Count != 1)
            {
                throw new CDKException("CarbonylEliminationReaction only expects one reactant");
            }
            if (agents != null)
            {
                throw new CDKException("CarbonylEliminationReaction don't expects agents");
            }

            IReactionSet setOfReactions = reactants.Builder.CreateReactionSet();
            IAtomContainer reactant = reactants[0];

            // if the parameter hasActiveCenter is not fixed yet, set the active
            // centers
            IParameterReact ipr = base.GetParameterClass(typeof(SetReactionCenter));
            if (ipr != null && !ipr.IsSetParameter) SetActiveCenters(reactant);
            foreach (var atomi in reactant.Atoms)
            {
                if (atomi.IsReactiveCenter && atomi.Symbol.Equals("O")
                    && atomi.FormalCharge == 1)
                {

                    foreach (var bondi in reactant.GetConnectedBonds(atomi))
                    {
                        if (bondi.IsReactiveCenter && bondi.Order == BondOrder.Triple)
                        {
                            IAtom atomj = bondi.GetConnectedAtom(atomi);
                            if (atomj.IsReactiveCenter)
                            {
                                foreach (var bondj in reactant.GetConnectedBonds(atomj))
                                {
                                    if (bondj.Equals(bondi)) continue;

                                    if (bondj.IsReactiveCenter
                                            && bondj.Order == BondOrder.Single)
                                    {

                                        IAtom atomk = bondj.GetConnectedAtom(atomj);
                                        if (atomk.IsReactiveCenter && atomk.FormalCharge == 0)
                                        {

                                            var atomList = new List<IAtom>();
                                            atomList.Add(atomk);
                                            atomList.Add(atomj);
                                            var bondList = new List<IBond>();
                                            bondList.Add(bondj);

                                            IAtomContainerSet<IAtomContainer> moleculeSet = reactant.Builder.CreateAtomContainerSet<IAtomContainer>();
                                            moleculeSet.Add(reactant);
                                            IReaction reaction = Mechanism.Initiate(moleculeSet, atomList, bondList);
                                            if (reaction == null)
                                                continue;
                                            else
                                                setOfReactions.Add(reaction);

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return setOfReactions;

        }

        /// <summary>
        /// set the active center for this molecule.
        /// The active center will be those which correspond with RC-C#[O+].
        /// <code>
        /// C: Atom
        /// -: single bond
        /// C: Atom
        /// #: triple bond
        /// O: Atom with formal charge = 1
        ///  </code>
        /// </summary>
        /// <param name="reactant">The molecule to set the activity</param>
        private void SetActiveCenters(IAtomContainer reactant)
        {
            foreach (var atomi in reactant.Atoms)
            {
                if (atomi.Symbol.Equals("O") && atomi.FormalCharge == 1)
                {
                    foreach (var bondi in reactant.GetConnectedBonds(atomi))
                    {
                        if (bondi.Order == BondOrder.Triple)
                        {
                            IAtom atomj = bondi.GetConnectedAtom(atomi);
                            foreach (var bondj in reactant.GetConnectedBonds(atomj))
                            {
                                if (bondj.Equals(bondi)) continue;

                                if (bondj.Order == BondOrder.Single)
                                {

                                    IAtom atomk = bondj.GetConnectedAtom(atomj);
                                    if (atomk.FormalCharge == 0)
                                    {
                                        atomi.IsReactiveCenter = true;
                                        bondi.IsReactiveCenter = true;
                                        atomj.IsReactiveCenter = true;
                                        bondj.IsReactiveCenter = true;
                                        atomk.IsReactiveCenter = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
