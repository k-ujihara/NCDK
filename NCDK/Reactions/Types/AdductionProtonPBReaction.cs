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
using NCDK.Reactions.Mechanisms;
using NCDK.Reactions.Types.Parameters;
using NCDK.Tools.Manipulator;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NCDK.Reactions.Types
{
    /// <summary>
    /// IReactionProcess which produces a protonation to double bond.
    /// As most commonly encountered, this reaction results in the formal migration
    /// of a hydrogen atom or proton, accompanied by a switch of a single bond and adjacent double bond
    /// <para>A=B + [H+] => [A+]-B-H</para>
    /// </summary>
    /// <seealso cref="AdductionPBMechanism"/>
    // @author         Miguel Rojas
    // @cdk.created    2008-02-11
    // @cdk.module     reaction
    // @cdk.set        reaction-types
    // @cdk.githash
    public partial class AdductionProtonPBReaction : ReactionEngine, IReactionProcess
    {
        /// <summary>
        /// Constructor of the AdductionProtonPBReaction object.
        /// </summary>
        public AdductionProtonPBReaction() { }

        /// <summary>
        /// Gets the specification attribute of the AdductionProtonPBReaction object.
        /// </summary>
        /// <returns>The specification value</returns>
        public ReactionSpecification Specification => new ReactionSpecification(
                    "http://almost.cubic.uni-koeln.de/jrg/Members/mrc/reactionDict/reactionDict#AdductionProtonPB", 
                    this.GetType().Name, "$Id$", "The Chemistry Development Kit");

        /// <summary>
        ///  Initiate process.
        ///  It is needed to call the addExplicitHydrogensToSatisfyValency
        ///  from the class tools.HydrogenAdder.
        /// </summary>
        /// <exception cref="CDKException"> Description of the Exception</exception>
        /// <param name="reactants">reactants of the reaction</param>
        /// <param name="agents">agents of the reaction (Must be in this case null)</param>
        public IReactionSet Initiate(IAtomContainerSet<IAtomContainer> reactants, IAtomContainerSet<IAtomContainer> agents)
        {
            Debug.WriteLine("initiate reaction: " + GetType().Name);

            if (reactants.Count != 1)
            {
                throw new CDKException(GetType().Name + " only expects one reactant");
            }
            if (agents != null)
            {
                throw new CDKException(GetType().Name + " don't expects agents");
            }

            IReactionSet setOfReactions = reactants.Builder.CreateReactionSet();
            IAtomContainer reactant = reactants[0];

            // if the parameter hasActiveCenter is not fixed yet, set the active centers
            IParameterReaction ipr = base.GetParameterClass(typeof(SetReactionCenter));
            if (ipr != null && !ipr.IsSetParameter) SetActiveCenters(reactant);

            if (AtomContainerManipulator.GetTotalCharge(reactant) != 0) return setOfReactions;

            foreach (var bondi in reactant.Bonds)
            {
                if (bondi.IsReactiveCenter
                        && ((bondi.Order == BondOrder.Double) || (bondi.Order == BondOrder.Triple))
                        && bondi.Atoms[0].IsReactiveCenter
                        && bondi.Atoms[1].IsReactiveCenter)
                {
                    int chargeAtom0 = bondi.Atoms[0].FormalCharge ?? 0;
                    int chargeAtom1 = bondi.Atoms[1].FormalCharge ?? 0;
                    if (chargeAtom0 >= 0 && chargeAtom1 >= 0
                            && !reactant.GetConnectedSingleElectrons(bondi.Atoms[0]).Any()
                            && !reactant.GetConnectedSingleElectrons(bondi.Atoms[1]).Any()
                            && !reactant.GetConnectedLonePairs(bondi.Atoms[0]).Any()
                            && !reactant.GetConnectedLonePairs(bondi.Atoms[1]).Any())
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            var atomList = new List<IAtom>();
                            if (j == 0)
                            {
                                atomList.Add(bondi.Atoms[0]);
                                atomList.Add(bondi.Atoms[1]);
                            }
                            else
                            {
                                atomList.Add(bondi.Atoms[1]);
                                atomList.Add(bondi.Atoms[0]);
                            }
                            IAtom atomH = reactant.Builder.CreateAtom("H");
                            atomH.FormalCharge = 1;
                            atomList.Add(atomH);

                            var bondList = new List<IBond>();
                            bondList.Add(bondi);

                            IAtomContainerSet<IAtomContainer> moleculeSet = reactant.Builder.CreateAtomContainerSet();
                            moleculeSet.Add(reactant);
                            IAtomContainer adduct = reactant.Builder.CreateAtomContainer();
                            adduct.Atoms.Add(atomH);
                            moleculeSet.Add(adduct);

                            IReaction reaction = Mechanism.Initiate(moleculeSet, atomList, bondList);
                            if (reaction == null)
                                continue;
                            else
                                setOfReactions.Add(reaction);
                        }
                    }
                }
            }

            return setOfReactions;
        }

        /// <summary>
        /// set the active center for this molecule.
        /// The active center will be those which correspond with X=Y.
        /// </summary>
        /// <param name="reactant">The molecule to set the activity</param>
        private void SetActiveCenters(IAtomContainer reactant)
        {
            if (AtomContainerManipulator.GetTotalCharge(reactant) != 0) return;

            foreach (var bondi in reactant.Bonds)
            {
                if (((bondi.Order == BondOrder.Double) || (bondi.Order == BondOrder.Triple)))
                {
                    int chargeAtom0 = bondi.Atoms[0].FormalCharge ?? 0;
                    int chargeAtom1 = bondi.Atoms[1].FormalCharge ?? 0;
                    if (chargeAtom0 >= 0 && chargeAtom1 >= 0
                            && !reactant.GetConnectedSingleElectrons(bondi.Atoms[0]).Any()
                            && !reactant.GetConnectedSingleElectrons(bondi.Atoms[1]).Any()
                            && !reactant.GetConnectedLonePairs(bondi.Atoms[0]).Any()
                            && !reactant.GetConnectedLonePairs(bondi.Atoms[1]).Any())
                    {
                        bondi.IsReactiveCenter = true;
                        bondi.Atoms[0].IsReactiveCenter = true;
                        bondi.Atoms[1].IsReactiveCenter = true;
                    }
                }
            }
        }
    }
}
