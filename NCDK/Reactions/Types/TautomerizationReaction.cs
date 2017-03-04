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
using System.Linq;
using NCDK.Reactions.Mechanisms;

namespace NCDK.Reactions.Types
{
    /// <summary>
    /// <para>IReactionProcess which produces a tautomerization chemical reaction.
    /// As most commonly encountered, this reaction results in the formal migration
    /// of a hydrogen atom or proton, accompanied by a switch of a single bond and adjacent double bond</para>
    ///
    /// <code>X=Y-Z-H => X(H)-Y=Z</code>
    ///
    /// <para>Below you have an example how to initiate the mechanism.</para>
    /// <para>It is processed by the HeterolyticCleavageMechanism class</para>
    /// <code>
    ///  IAtomContainerSet setOfReactants = Default.ChemObjectBuilder.Instance.NewAtomContainerSet();
    ///  setOfReactants.Add(new AtomContainer());
    ///  IReactionProcess type = new TautomerizationReaction();
    ///  object[] parameters = {bool.FALSE};
    ///  type.Parameters = parameters;
    ///  IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to specify the reaction in a fixed point.</para>
    /// <code>atoms[0].SetFlag(CDKConstants.REACTIVE_CENTER,true);</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not specified then the reaction process will
    /// try to find automatically the possible reaction centers.</para>
    ///
    ///
    // @author         Miguel Rojas
    ///
    // @cdk.created    2008-02-11
    // @cdk.module     reaction
    // @cdk.set        reaction-types
    // @cdk.githash
    ///
    /// <seealso cref="TautomerizationMechanism"/>
    ///*/
    public class TautomerizationReaction : ReactionEngine, IReactionProcess
    {

        /// <summary>
        /// Constructor of the TautomerizationReaction object.
        ///
        /// </summary>
        public TautomerizationReaction() { }

        /// <summary>
        ///  Gets the specification attribute of the TautomerizationReaction object.
        ///
        /// <returns>The specification value</returns>
        /// </summary>

        public ReactionSpecification Specification =>
            new ReactionSpecification(
                    "http://almost.cubic.uni-koeln.de/jrg/Members/mrc/reactionDict/reactionDict#Tautomerization", this
                            .GetType().Name, "$Id$", "The Chemistry Development Kit");

        /// <summary>
        ///  Initiate process.
        ///  It is needed to call the addExplicitHydrogensToSatisfyValency
        ///  from the class tools.HydrogenAdder.
        ///
        /// <param name="reactants">reactants of the reaction</param>
        /// <param name="agents">agents of the reaction (Must be in this case null)</param>
        ///
        /// <exception cref="CDKException"> Description of the Exception</exception>
        /// </summary>

        public IReactionSet Initiate(IAtomContainerSet<IAtomContainer> reactants, IAtomContainerSet<IAtomContainer> agents)
        {

            Debug.WriteLine("initiate reaction: TautomerizationReaction");

            if (reactants.Count != 1)
            {
                throw new CDKException("TautomerizationReaction only expects one reactant");
            }
            if (agents != null)
            {
                throw new CDKException("TautomerizationReaction don't expects agents");
            }

            IReactionSet setOfReactions = reactants.Builder.CreateReactionSet();
            IAtomContainer reactant = reactants[0];

            /// if the parameter hasActiveCenter is not fixed yet, set the active centers
            IParameterReact ipr = base.GetParameterClass(typeof(SetReactionCenter));
            if (ipr != null && !ipr.IsSetParameter) SetActiveCenters(reactant);

            foreach (var atomi in reactant.Atoms)
            {
                if (atomi.IsReactiveCenter
                        && (atomi.FormalCharge ?? 0) == 0
                        && !reactant.GetConnectedSingleElectrons(atomi).Any())
                {
                    foreach (var bondi in reactant.GetConnectedBonds(atomi))
                    {
                        if (bondi.IsReactiveCenter && bondi.Order == BondOrder.Double)
                        {
                            IAtom atomj = bondi.GetConnectedAtom(atomi); // Atom pos 2
                            if (atomj.IsReactiveCenter
                                    && (atomj.FormalCharge ?? 0) == 0
                                    && !reactant.GetConnectedSingleElectrons(atomj).Any())
                            {
                                foreach (var bondj in reactant.GetConnectedBonds(atomj))
                                {
                                    if (bondj.Equals(bondi)) continue;
                                    if (bondj.IsReactiveCenter
                                            && bondj.Order == BondOrder.Single)
                                    {
                                        IAtom atomk = bondj.GetConnectedAtom(atomj); // Atom pos 3
                                        if (atomk.IsReactiveCenter
                                                && (atomk.FormalCharge ?? 0) == 0
                                                && !reactant.GetConnectedSingleElectrons(atomk).Any())
                                        {
                                            foreach (var bondk in reactant.GetConnectedBonds(atomk))
                                            {
                                                if (bondk.Equals(bondj)) continue;
                                                if (bondk.IsReactiveCenter
                                                        && bondk.Order == BondOrder.Single)
                                                {
                                                    IAtom atoml = bondk.GetConnectedAtom(atomk); // Atom pos 4
                                                    if (atoml.IsReactiveCenter
                                                            && atoml.Symbol.Equals("H"))
                                                    {

                                                        var atomList = new List<IAtom>();
                                                        atomList.Add(atomi);
                                                        atomList.Add(atomj);
                                                        atomList.Add(atomk);
                                                        atomList.Add(atoml);
                                                        var bondList = new List<IBond>();
                                                        bondList.Add(bondi);
                                                        bondList.Add(bondj);
                                                        bondList.Add(bondk);

                                                        IAtomContainerSet<IAtomContainer> moleculeSet = reactant.Builder.CreateAtomContainerSet<IAtomContainer>();

                                                        moleculeSet.Add(reactant);
                                                        IReaction reaction = Mechanism.Initiate(moleculeSet, atomList,
                                                                bondList);
                                                        if (reaction == null)
                                                            continue;
                                                        else
                                                            setOfReactions.Add(reaction);

                                                        break; // because of the others atoms are hydrogen too.
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
            }

            return setOfReactions;
        }

        /// <summary>
        /// set the active center for this molecule.
        /// The active center will be those which correspond with X=Y-Z-H.
        /// <code>
        /// X: Atom
        /// =: bond
        /// Y: Atom
        /// -: bond
        /// Z: Atom
        /// -: bond
        /// H: Atom
        ///  </code>
        ///
        /// <param name="reactant">The molecule to set the activity</param>
        // @
        /// </summary>
        private void SetActiveCenters(IAtomContainer reactant)
        {
            foreach (var atomi in reactant.Atoms)
            {
                if ((atomi.FormalCharge ?? 0) == 0
                        && !reactant.GetConnectedSingleElectrons(atomi).Any())
                {
                    foreach (var bondi in reactant.GetConnectedBonds(atomi))
                    {
                        if (bondi.Order == BondOrder.Double)
                        {
                            IAtom atomj = bondi.GetConnectedAtom(atomi); // Atom pos 2
                            if ((atomj.FormalCharge ?? 0) == 0
                                    && !reactant.GetConnectedSingleElectrons(atomj).Any())
                            {
                                foreach (var bondj in reactant.GetConnectedBonds(atomj))
                                {
                                    if (bondj.Equals(bondi)) continue;
                                    if (bondj.Order == BondOrder.Single)
                                    {
                                        IAtom atomk = bondj.GetConnectedAtom(atomj); // Atom pos 3
                                        if ((atomk.FormalCharge ?? 0) == 0
                                                && !reactant.GetConnectedSingleElectrons(atomk).Any())
                                        {
                                            foreach (var bondk in reactant.GetConnectedBonds(atomk))
                                            {
                                                if (bondk.Equals(bondj)) continue;
                                                if (bondk.Order == BondOrder.Single)
                                                {
                                                    IAtom atoml = bondk.GetConnectedAtom(atomk); // Atom pos 4
                                                    if (atoml.Symbol.Equals("H"))
                                                    {
                                                        atomi.IsReactiveCenter = true;
                                                        atomj.IsReactiveCenter = true;
                                                        atomk.IsReactiveCenter = true;
                                                        atoml.IsReactiveCenter = true;
                                                        bondi.IsReactiveCenter = true;
                                                        bondj.IsReactiveCenter = true;
                                                        bondk.IsReactiveCenter = true;
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
            }
        }
    }
}
