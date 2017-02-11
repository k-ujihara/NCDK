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
using NCDK.Aromaticities;
using NCDK.Reactions.Types.Parameters;
using NCDK.RingSearches;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NCDK.Reactions.Types
{
    /**
     * <p>
     * This reaction could be represented as [A*]-(C)_4-C5[R] => A([R])-(C_4)-[C5*]. Due to
     * the single electron of atom A the R is moved.</p>
     * <p>It is processed by the RadicalSiteRearrangementMechanism class</p>
     *
     * <pre>
     *  IAtomContainerSet setOfReactants = Default.ChemObjectBuilder.Instance.NewAtomContainerSet();
     *  setOfReactants.Add(new AtomContainer());
     *  IReactionProcess type = new RadicalSiteRrDeltaReaction();
     *  object[] parameters = {bool.FALSE};
     *  type.Parameters = parameters;
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
     * @cdk.created    2006-10-20
     * @cdk.module     reaction
     * @cdk.githash
     * @cdk.set        reaction-types
     *
     * @see RadicalSiteRearrangementMechanism
     **/
    public class RadicalSiteRrDeltaReaction : ReactionEngine, IReactionProcess
    {

        /**
         * Constructor of the RadicalSiteRrDeltaReaction object
         *
         */
        public RadicalSiteRrDeltaReaction() { }

        /**
         *  Gets the specification attribute of the RadicalSiteRrDeltaReaction object
         *
         *@return    The specification value
         */

        public ReactionSpecification Specification => new ReactionSpecification(
                    "http://almost.cubic.uni-koeln.de/jrg/Members/mrc/reactionDict/reactionDict#RadicalSiteRrDelta", this
                            .GetType().Name, "$Id$", "The Chemistry Development Kit");

        /**
         *  Initiate process.
         *  It is needed to call the addExplicitHydrogensToSatisfyValency
         *  from the class tools.HydrogenAdder.
         *
         *
         *@exception  CDKException  Description of the Exception

         * @param  reactants         reactants of the reaction.
        * @param  agents            agents of the reaction (Must be in this case null).
         */

        public IReactionSet Initiate(IAtomContainerSet<IAtomContainer> reactants, IAtomContainerSet<IAtomContainer> agents)
        {

            Debug.WriteLine("initiate reaction: RadicalSiteRrDeltaReaction");

            if (reactants.Count != 1)
            {
                throw new CDKException("RadicalSiteRrDeltaReaction only expects one reactant");
            }
            if (agents != null)
            {
                throw new CDKException("RadicalSiteRrDeltaReaction don't expects agents");
            }

            IReactionSet setOfReactions = reactants.Builder.CreateReactionSet();
            IAtomContainer reactant = reactants[0];

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(reactant);
            Aromaticity.CDKLegacy.Apply(reactant);
            AllRingsFinder arf = new AllRingsFinder();
            IRingSet ringSet = arf.FindAllRings(reactant);
            for (int ir = 0; ir < ringSet.Count; ir++)
            {
                IRing ring = (IRing)ringSet[ir];
                for (int jr = 0; jr < ring.Atoms.Count; jr++)
                {
                    IAtom aring = ring.Atoms[jr];
                    aring.IsInRing = true;
                }
            }
            /*
             * if the parameter hasActiveCenter is not fixed yet, set the active
             * centers
             */
            IParameterReact ipr = base.GetParameterClass(typeof(SetReactionCenter));
            if (ipr != null && !ipr.IsSetParameter) SetActiveCenters(reactant);

            HOSECodeGenerator hcg = new HOSECodeGenerator();
            foreach (var atomi in reactant.Atoms)
            {
                if (atomi.IsReactiveCenter && reactant.GetConnectedSingleElectrons(atomi).Count() == 1)
                {

                    hcg.GetSpheres(reactant, atomi, 3, true);
                    var atom1s = hcg.GetNodesInSphere(3);

                    hcg.GetSpheres(reactant, atomi, 4, true);
                    foreach (var atoml in hcg.GetNodesInSphere(4))
                    {
                        if (atoml != null && atoml.IsReactiveCenter
                                && !atoml.IsInRing
                                && (atoml.FormalCharge ?? 0) == 0
                                && !atoml.Equals("H") && reactant.GetMaximumBondOrder(atoml) == BondOrder.Single)
                        {

                            foreach (var atomR in reactant.GetConnectedAtoms(atoml))
                            {
                                if (atom1s.Contains(atomR)) continue;
                                if (reactant.GetBond(atomR, atoml).IsReactiveCenter
                                        && atomR.IsReactiveCenter
                                        && (atomR.FormalCharge ?? 0) == 0)
                                {

                                    var atomList = new List<IAtom>();
                                    atomList.Add(atomR);
                                    atomList.Add(atomi);
                                    atomList.Add(atoml);
                                    var bondList = new List<IBond>();
                                    bondList.Add(reactant.GetBond(atomR, atoml));

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
            return setOfReactions;
        }

        /**
         * set the active center for this molecule.
         * The active center will be those which correspond with [A*]-(C)_2-C3[R]
         * <pre>
         * C: Atom with single electron
         * C5: Atom with the R to move
         *  </pre>
         *
         * @param reactant The molecule to set the activity
         * @
         */
        private void SetActiveCenters(IAtomContainer reactant)
        {
            HOSECodeGenerator hcg = new HOSECodeGenerator();
            foreach (var atomi in reactant.Atoms)
            {
                if (reactant.GetConnectedSingleElectrons(atomi).Count() == 1)
                {

                    hcg.GetSpheres(reactant, atomi, 3, true);
                    var atom1s = hcg.GetNodesInSphere(3);

                    hcg.GetSpheres(reactant, atomi, 4, true);
                    foreach (var atoml in hcg.GetNodesInSphere(4))
                    {
                        if (atoml != null && !atoml.IsInRing
                                && (atoml.FormalCharge ?? 0) == 0
                                && !atoml.Equals("H") && reactant.GetMaximumBondOrder(atoml) == BondOrder.Single)
                        {

                            foreach (var atomR in reactant.GetConnectedAtoms(atoml))
                            {
                                if (atom1s.Contains(atomR)) continue;
                                if ((atomR.FormalCharge ?? 0) == 0)
                                {

                                    atomi.IsReactiveCenter = true;
                                    atoml.IsReactiveCenter = true;
                                    atomR.IsReactiveCenter = true;
                                    reactant.GetBond(atomR, atoml).IsReactiveCenter = true; ;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
