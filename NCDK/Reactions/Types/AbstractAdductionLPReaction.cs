/* Copyright (C) 2008  Miguel Rojas <miguelrojasch@users.sf.net>
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
using NCDK.Tools.Manipulator;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Reactions.Types
{
    // @author         Miguel Rojas
    // @cdk.created    2008-02-11
    // @cdk.module     reaction
    // @cdk.set        reaction-types
    // @cdk.githash
    public abstract class AbstractAdductionLPReaction : ReactionEngine, IReactionProcess
    {
        public AbstractAdductionLPReaction() { }

        /// <inheritdoc/>
        public abstract ReactionSpecification Specification { get; }

        /// <inheritdoc/>
        public abstract IReactionSet Initiate(IAtomContainerSet<IAtomContainer> reactants, IAtomContainerSet<IAtomContainer> agents);
        
        internal IReactionSet Initiate(IAtomContainerSet<IAtomContainer> reactants, IAtomContainerSet<IAtomContainer> agents, string atomSymbol)
        {
            CheckInitiateParams(reactants, agents);

            IReactionSet setOfReactions = reactants.Builder.CreateReactionSet();
            IAtomContainer reactant = reactants[0];

            IParameterReaction ipr = base.GetParameterClass(typeof(SetReactionCenter));
            if (ipr != null && !ipr.IsSetParameter) SetActiveCenters(reactant);

            if (AtomContainerManipulator.GetTotalCharge(reactant) > 0) return setOfReactions;

            foreach (var atomi in reactant.Atoms)
            {
                if (atomi.IsReactiveCenter
                   && (atomi.FormalCharge ?? 0) <= 0
                   && reactant.GetConnectedLonePairs(atomi).Any()
                   && !reactant.GetConnectedSingleElectrons(atomi).Any())
                {
                    var atomList = new List<IAtom>();
                    atomList.Add(atomi);
                    IAtom atomH = reactant.Builder.CreateAtom(atomSymbol);
                    atomH.FormalCharge = 1;
                    atomList.Add(atomH);

                    var moleculeSet = reactant.Builder.CreateAtomContainerSet();
                    moleculeSet.Add(reactant);
                    IAtomContainer adduct = reactant.Builder.CreateAtomContainer();
                    adduct.Atoms.Add(atomH);
                    moleculeSet.Add(adduct);

                    IReaction reaction = Mechanism.Initiate(moleculeSet, atomList, null);
                    if (reaction == null)
                        continue;
                    else
                        setOfReactions.Add(reaction);
                }
            }

            return setOfReactions;
        }

        private void SetActiveCenters(IAtomContainer reactant)
        {
            if (AtomContainerManipulator.GetTotalCharge(reactant) > 0) return;

            foreach (var atomi in reactant.Atoms)
            {
                if ((atomi.FormalCharge ?? 0) <= 0
                        && reactant.GetConnectedLonePairs(atomi).Any()
                        && !reactant.GetConnectedSingleElectrons(atomi).Any())
                {
                    atomi.IsReactiveCenter = true;
                }
            }
        }
    }
}
