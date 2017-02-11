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

namespace NCDK.Reactions.Types
{
    /**
     * <p>IReactionProcess which make an electron impact for for Non-Bonding Electron Lost.
     * This reaction type is a representation of the processes which occurs in the mass spectrometer.</p>
     * <p>It is processed by the RemovingSEofNBMechanism class</p>
     *
     *<pre>
     *  IAtomContainerSet setOfReactants = Default.ChemObjectBuilder.Instance.NewAtomContainerSet();
     *  setOfReactants.Add(new AtomContainer());
     *  IReactionProcess type = new ElectronImpactNBEReaction();
     *  object[] parameters = {bool.FALSE};
        type.Parameters = parameters;
     *  IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
     *  </pre>
     *
     * <p>We have the possibility to localize the reactive center. Good method if you
     * want to localize the reaction in a fixed point</p>
     * <pre>atoms[0].SetFlag(CDKConstants.REACTIVE_CENTER,true);</pre>
     * <p>Moreover you must put the parameter true</p>
     * <p>If the reactive center is not localized then the reaction process will
     * try to find automatically the possible reactive center.</p>
     *
     *
     * @author         Miguel Rojas
     *
     * @cdk.created    2006-04-01
     * @cdk.module     reaction
     * @cdk.githash
     * @cdk.set        reaction-types
     * @cdk.dictref    reaction-types:electronImpact
     *
     * @see RemovingSEofNBMechanism
     *
     **/
    public class ElectronImpactNBEReaction : ReactionEngine, IReactionProcess
    {

        /**
         * Constructor of the ElectronImpactNBEReaction object.
         *
         */
        public ElectronImpactNBEReaction()
            : base()
        { }

        /**
         * Gets the specification attribute of the ElectronImpactNBEReaction object.
         *
         * @return    The specification value
         */

        public ReactionSpecification Specification =>
            new ReactionSpecification(
                    "http://almost.cubic.uni-koeln.de/jrg/Members/mrc/reactionDict/reactionDict#ElectronImpactNBE", this
                            .GetType().Name, "$Id$", "The Chemistry Development Kit");

        /**
         *  Initiate process.
         *  It is needed to call the addExplicitHydrogensToSatisfyValency
         *  from the class tools.HydrogenAdder.
         *
         *
         * @param  reactants         Reactants of the reaction
         * @param  agents            Agents of the reaction (Must be in this case null)
         *
         * @exception  CDKException  Description of the Exception
         */

        public IReactionSet Initiate(IAtomContainerSet<IAtomContainer> reactants, IAtomContainerSet<IAtomContainer> agents)
        {

            Debug.WriteLine("initiate reaction: ElectronImpactNBEReaction");

            if (reactants.Count != 1)
            {
                throw new CDKException("ElectronImpactNBEReaction only expects one reactant");
            }
            if (agents != null)
            {
                throw new CDKException("ElectronImpactNBEReaction don't expects agents");
            }

            IReactionSet setOfReactions = reactants.Builder.CreateReactionSet();
            IAtomContainer reactant = reactants[0];

            /*
             * if the parameter hasActiveCenter is not fixed yet, set the active
             * centers
             */
            IParameterReact ipr = base.GetParameterClass(typeof(SetReactionCenter));
            if (ipr != null && !ipr.IsSetParameter) SetActiveCenters(reactant);
            foreach (var atom in reactant.Atoms)
            {
                if (atom.IsReactiveCenter && reactant.GetConnectedLonePairs(atom).Any()
                        && !reactant.GetConnectedSingleElectrons(atom).Any())
                {

                    var atomList = new List<IAtom>();
                    atomList.Add(atom);
                    IAtomContainerSet<IAtomContainer> moleculeSet = reactant.Builder.CreateAtomContainerSet();
                    moleculeSet.Add(reactant);
                    IReaction reaction = Mechanism.Initiate(moleculeSet, atomList, null);
                    if (reaction == null)
                        continue;
                    else
                        setOfReactions.Add(reaction);
                }
            }
            return setOfReactions;

        }

        /**
         * set the active center for this molecule. The active center
         * will be heteroatoms which contain at least one group of
         * lone pair electrons.
         *
         * @param reactant The molecule to set the activity
         * @
         */
        private void SetActiveCenters(IAtomContainer reactant)
        {
            foreach (var atom in reactant.Atoms)
            {
                if (reactant.GetConnectedLonePairs(atom).Any() && !reactant.GetConnectedSingleElectrons(atom).Any())
                    atom.IsReactiveCenter = true;
            }
        }
    }
}
