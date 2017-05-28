/* Copyright (C) 2007-2015  Egon Willighagen <egonw@users.sf.net>
 *                    2011  Nimish Gopal <nimishg@ebi.ac.uk>
 *                    2011  Syed Asad Rahman <asad@ebi.ac.uk>
 *                    2011  Gilleain Torrance <gilleain.torrance@gmail.com>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation, version 2.1.
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
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NCDK.Config;
using NCDK.RingSearches;
using NCDK.Tools.Manipulator;

namespace NCDK.AtomTypes
{
    /// <summary>
    /// Atom Type matcher that perceives atom types as defined in the CDK atom type list
    /// <c>NCDK.Dict.Data.cdk-atom-types.owl</c>.
    ///  If there is not an atom type defined for the tested atom, then <see langword="null"/> is returned.
    /// </summary>
    // @author         egonw
    // @cdk.created    2007-07-20
    // @cdk.module     core
    // @cdk.githash
    public class CDKAtomTypeMatcher
        : IAtomTypeMatcher
    {
        public const int RequireNothing = 1;
        public const int RequireExplicitHydrogens = 2;

        private AtomTypeFactory factory;
        private int mode;

        private static readonly object syncLock = new object();

        private static IDictionary<int, IDictionary<IChemObjectBuilder, CDKAtomTypeMatcher>> factories = new ConcurrentDictionary<int, IDictionary<IChemObjectBuilder, CDKAtomTypeMatcher>>();

        private CDKAtomTypeMatcher(IChemObjectBuilder builder, int mode)
        {
            factory = AtomTypeFactory.GetInstance("NCDK.Dict.Data.cdk-atom-types.owl", builder);
            this.mode = mode;
        }

        public static CDKAtomTypeMatcher GetInstance(IChemObjectBuilder builder)
        {
            return GetInstance(builder, RequireNothing);
        }

        public static CDKAtomTypeMatcher GetInstance(IChemObjectBuilder builder, int mode)
        {
            lock (syncLock)
            {
                if (!factories.ContainsKey(mode))
                    factories.Add(mode, new Dictionary<IChemObjectBuilder, CDKAtomTypeMatcher>(1));
                if (!factories[mode].ContainsKey(builder))
                    factories[mode].Add(builder, new CDKAtomTypeMatcher(builder, mode));
                return factories[mode][builder];
            }
        }

        public IAtomType[] FindMatchingAtomTypes(IAtomContainer atomContainer)
        {
            return FindMatchingAtomTypes(atomContainer, null);
        }

        private IAtomType[] FindMatchingAtomTypes(IAtomContainer atomContainer, RingSearch searcher)
        {
            // cache the ring information
            if (searcher == null) searcher = new RingSearch(atomContainer);
            // cache atom bonds
            var connectedBonds = new Dictionary<IAtom, IList<IBond>>(atomContainer.Atoms.Count);
            foreach (var bond in atomContainer.Bonds)
            {
                foreach (var atom in bond.Atoms)
                {
                    IList<IBond> atomBonds;
                    if (!connectedBonds.TryGetValue(atom, out atomBonds))
                    {
                        atomBonds = new List<IBond>();
                        connectedBonds.Add(atom, atomBonds);
                    }
                    atomBonds.Add(bond);
                }
            }

            IAtomType[] types = new IAtomType[atomContainer.Atoms.Count];
            int typeCounter = 0;
            foreach (var atom in atomContainer.Atoms)
            {
                types[typeCounter] = FindMatchingAtomType(atomContainer, atom, searcher, connectedBonds.ContainsKey(atom) ? connectedBonds[atom] : null);
                typeCounter++;
            }
            return types;
        }

        public IAtomType FindMatchingAtomType(IAtomContainer atomContainer, IAtom atom)
        {
            return FindMatchingAtomType(atomContainer, atom, null, null);
        }

        private IAtomType FindMatchingAtomType(IAtomContainer atomContainer, IAtom atom, RingSearch searcher, IList<IBond> connectedBonds)
        {
            IAtomType type = null;
            if (atom is IPseudoAtom)
            {
                return factory.GetAtomType("X");
            }
            if ("C".Equals(atom.Symbol))
            {
                type = PerceiveCarbons(atomContainer, atom, searcher, connectedBonds);
            }
            else if ("H".Equals(atom.Symbol))
            {
                type = PerceiveHydrogens(atomContainer, atom, connectedBonds);
            }
            else if ("O".Equals(atom.Symbol))
            {
                type = PerceiveOxygens(atomContainer, atom, searcher, connectedBonds);
            }
            else if ("N".Equals(atom.Symbol))
            {
                type = PerceiveNitrogens(atomContainer, atom, searcher, connectedBonds);
            }
            else if ("S".Equals(atom.Symbol))
            {
                type = PerceiveSulphurs(atomContainer, atom, searcher, connectedBonds);
            }
            else if ("P".Equals(atom.Symbol))
            {
                type = PerceivePhosphors(atomContainer, atom, connectedBonds);
            }
            else if ("Si".Equals(atom.Symbol))
            {
                type = PerceiveSilicon(atomContainer, atom);
            }
            else if ("Li".Equals(atom.Symbol))
            {
                type = PerceiveLithium(atomContainer, atom);
            }
            else if ("B".Equals(atom.Symbol))
            {
                type = PerceiveBorons(atomContainer, atom);
            }
            else if ("Be".Equals(atom.Symbol))
            {
                type = PerceiveBeryllium(atomContainer, atom);
            }
            else if ("Cr".Equals(atom.Symbol))
            {
                type = PerceiveChromium(atomContainer, atom);
            }
            else if ("Se".Equals(atom.Symbol))
            {
                type = PerceiveSelenium(atomContainer, atom, connectedBonds);
            }
            else if ("Mo".Equals(atom.Symbol))
            {
                type = PerceiveMolybdenum(atomContainer, atom);
            }
            else if ("Rb".Equals(atom.Symbol))
            {
                type = PerceiveRubidium(atomContainer, atom);
            }
            else if ("Te".Equals(atom.Symbol))
            {
                type = PerceiveTellurium(atomContainer, atom);
            }
            else if ("Cu".Equals(atom.Symbol))
            {
                type = PerceiveCopper(atomContainer, atom);
            }
            else if ("Ba".Equals(atom.Symbol))
            {
                type = PerceiveBarium(atomContainer, atom);
            }
            else if ("Ga".Equals(atom.Symbol))
            {
                type = PerceiveGallium(atomContainer, atom);
            }
            else if ("Ru".Equals(atom.Symbol))
            {
                type = PerceiveRuthenium(atomContainer, atom);
            }
            else if ("Zn".Equals(atom.Symbol))
            {
                type = PerceiveZinc(atomContainer, atom);
            }
            else if ("Al".Equals(atom.Symbol))
            {
                type = PerceiveAluminium(atomContainer, atom);
            }
            else if ("Ni".Equals(atom.Symbol))
            {
                type = PerceiveNickel(atomContainer, atom);
            }
            else if ("Gd".Equals(atom.Symbol))
            {
                type = PerceiveGadolinum(atomContainer, atom);
            }
            else if ("Ge".Equals(atom.Symbol))
            {
                type = PerceiveGermanium(atomContainer, atom);
            }
            else if ("Co".Equals(atom.Symbol))
            {
                type = PerceiveCobalt(atomContainer, atom);
            }
            else if ("Br".Equals(atom.Symbol))
            {
                type = PerceiveBromine(atomContainer, atom);
            }
            else if ("V".Equals(atom.Symbol))
            {
                type = PerceiveVanadium(atomContainer, atom);
            }
            else if ("Ti".Equals(atom.Symbol))
            {
                type = PerceiveTitanium(atomContainer, atom);
            }
            else if ("Sr".Equals(atom.Symbol))
            {
                type = PerceiveStrontium(atomContainer, atom);
            }
            else if ("Pb".Equals(atom.Symbol))
            {
                type = PerceiveLead(atomContainer, atom);
            }
            else if ("Tl".Equals(atom.Symbol))
            {
                type = PerceiveThallium(atomContainer, atom);
            }
            else if ("Sb".Equals(atom.Symbol))
            {
                type = PerceiveAntimony(atomContainer, atom);
            }
            else if ("Pt".Equals(atom.Symbol))
            {
                type = PerceivePlatinum(atomContainer, atom);
            }
            else if ("Hg".Equals(atom.Symbol))
            {
                type = PerceiveMercury(atomContainer, atom);
            }
            else if ("Fe".Equals(atom.Symbol))
            {
                type = PerceiveIron(atomContainer, atom);
            }
            else if ("Ra".Equals(atom.Symbol))
            {
                type = PerceiveRadium(atomContainer, atom);
            }
            else if ("Au".Equals(atom.Symbol))
            {
                type = PerceiveGold(atomContainer, atom);
            }
            else if ("Ag".Equals(atom.Symbol))
            {
                type = PerceiveSilver(atomContainer, atom);
            }
            else if ("Cl".Equals(atom.Symbol))
            {
                type = PerceiveChlorine(atomContainer, atom, connectedBonds);
            }
            else if ("In".Equals(atom.Symbol))
            {
                type = PerceiveIndium(atomContainer, atom);
            }
            else if ("Pu".Equals(atom.Symbol))
            {
                type = PerceivePlutonium(atomContainer, atom);
            }
            else if ("Th".Equals(atom.Symbol))
            {
                type = PerceiveThorium(atomContainer, atom);
            }
            else if ("K".Equals(atom.Symbol))
            {
                type = PerceivePotassium(atomContainer, atom);
            }
            else if ("Mn".Equals(atom.Symbol))
            {
                type = PerceiveManganese(atomContainer, atom);
            }
            else if ("Mg".Equals(atom.Symbol))
            {
                type = PerceiveMagnesium(atomContainer, atom);
            }
            else if ("Na".Equals(atom.Symbol))
            {
                type = PerceiveSodium(atomContainer, atom);
            }
            else if ("As".Equals(atom.Symbol))
            {
                type = PerceiveArsenic(atomContainer, atom);
            }
            else if ("Cd".Equals(atom.Symbol))
            {
                type = PerceiveCadmium(atomContainer, atom);
            }
            else if ("Ca".Equals(atom.Symbol))
            {
                type = PerceiveCalcium(atomContainer, atom);
            }
            else
            {
                if (type == null) type = PerceiveHalogens(atomContainer, atom, connectedBonds);
                if (type == null) type = PerceiveCommonSalts(atomContainer, atom);
                if (type == null) type = PerceiveOrganometallicCenters(atomContainer, atom);
                if (type == null) type = PerceiveNobelGases(atomContainer, atom);
            }

            // if no atom type can be assigned we set the atom type to 'X', this flags
            // to other methods that atom typing was performed but did not yield a match
            if (type == null)
            {
                type = GetAtomType("X");
            }

            return type;
        }

        private IAtomType PerceiveGallium(IAtomContainer atomContainer, IAtom atom)
        {
            BondOrder maxBondOrder = atomContainer.GetMaximumBondOrder(atom);
            if (!IsCharged(atom) && maxBondOrder == BondOrder.Single && atomContainer.GetConnectedBonds(atom).Count() <= 3)
            {
                IAtomType type = GetAtomType("Ga");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            else if (atom.FormalCharge == 3)
            {
                IAtomType type = GetAtomType("Ga.3plus");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            return null;
        }

        private IAtomType PerceiveGermanium(IAtomContainer atomContainer, IAtom atom)
        {
            BondOrder maxBondOrder = atomContainer.GetMaximumBondOrder(atom);
            if (!IsCharged(atom) && maxBondOrder == BondOrder.Single && atomContainer.GetConnectedBonds(atom).Count() <= 4)
            {
                IAtomType type = GetAtomType("Ge");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            if (atom.FormalCharge == 0 && atomContainer.GetConnectedBonds(atom).Count() == 3)
            {
                IAtomType type = GetAtomType("Ge.3");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            return null;
        }

        private IAtomType PerceiveSelenium(IAtomContainer atomContainer, IAtom atom, IList<IBond> connectedBonds)
        {
            if ("Se".Equals(atom.Symbol))
            {
                if (connectedBonds == null) connectedBonds = atomContainer.GetConnectedBonds(atom).ToList();
                int doublebondcount = CountAttachedDoubleBonds(connectedBonds, atom);
                if (atom.FormalCharge != null && atom.FormalCharge == 0)
                {
                    if (atomContainer.GetConnectedBonds(atom).Count() == 0)
                    {
                        if (atom.ImplicitHydrogenCount != null && atom.ImplicitHydrogenCount == 0)
                        {
                            IAtomType type = GetAtomType("Se.2");
                            if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                        }
                        else
                        {
                            IAtomType type = GetAtomType("Se.3");
                            if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                        }
                    }
                    else if (atomContainer.GetConnectedBonds(atom).Count() == 1)
                    {
                        if (doublebondcount == 1)
                        {
                            IAtomType type = GetAtomType("Se.1");
                            if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                        }
                        else if (doublebondcount == 0)
                        {
                            IAtomType type = GetAtomType("Se.3");
                            if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                        }
                    }
                    else if (atomContainer.GetConnectedBonds(atom).Count() == 2)
                    {
                        if (doublebondcount == 0)
                        {
                            IAtomType type = GetAtomType("Se.3");
                            if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                        }
                        else if (doublebondcount == 2)
                        {
                            IAtomType type = GetAtomType("Se.sp2.2");
                            if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                        }
                    }
                    else if (atomContainer.GetConnectedBonds(atom).Count() == 3)
                    {
                        IAtomType type = GetAtomType("Se.sp3.3");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                    else if (atomContainer.GetConnectedBonds(atom).Count() == 4)
                    {
                        if (doublebondcount == 2)
                        {
                            IAtomType type = GetAtomType("Se.sp3.4");
                            if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                        }
                        else if (doublebondcount == 0)
                        {
                            IAtomType type = GetAtomType("Se.sp3d1.4");
                            if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                        }
                    }
                    else if (atomContainer.GetConnectedBonds(atom).Count() == 5)
                    {
                        IAtomType type = GetAtomType("Se.5");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                }
                else if ((atom.FormalCharge != null && atom.FormalCharge == 4)
                      && atomContainer.GetConnectedBonds(atom).Count() == 0)
                {
                    IAtomType type = GetAtomType("Se.4plus");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                else if ((atom.FormalCharge != null && atom.FormalCharge == 1)
                      && atomContainer.GetConnectedBonds(atom).Count() == 3)
                {
                    IAtomType type = GetAtomType("Se.plus.3");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                else if ((atom.FormalCharge != null && atom.FormalCharge == -2)
                      && atomContainer.GetConnectedBonds(atom).Count() == 0)
                {
                    IAtomType type = GetAtomType("Se.2minus");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
            }
            return null;
        }

        private IAtomType PerceiveTellurium(IAtomContainer atomContainer, IAtom atom)
        {
            BondOrder maxBondOrder = atomContainer.GetMaximumBondOrder(atom);
            if (!IsCharged(atom) && maxBondOrder == BondOrder.Single && atomContainer.GetConnectedBonds(atom).Count() <= 2)
            {
                IAtomType type = GetAtomType("Te.3");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            else if (atom.FormalCharge == 4)
            {
                if (atomContainer.GetConnectedBonds(atom).Count() == 0)
                {
                    IAtomType type = GetAtomType("Te.4plus");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
            }
            return null;
        }

        private IAtomType PerceiveBorons(IAtomContainer atomContainer, IAtom atom)
        {
            BondOrder maxBondOrder = atomContainer.GetMaximumBondOrder(atom);
            if (atom.FormalCharge == -1 && maxBondOrder == BondOrder.Single
                    && atomContainer.GetConnectedBonds(atom).Count() <= 4)
            {
                IAtomType type = GetAtomType("B.minus");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            else if (atom.FormalCharge == +3 && atomContainer.GetConnectedBonds(atom).Count() == 4)
            {
                IAtomType type = GetAtomType("B.3plus");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            else if (atomContainer.GetConnectedBonds(atom).Count() <= 3)
            {
                IAtomType type = GetAtomType("B");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            return null;
        }

        private IAtomType PerceiveBeryllium(IAtomContainer atomContainer, IAtom atom)
        {
            if (atom.FormalCharge == -2 && atomContainer.GetMaximumBondOrder(atom) == BondOrder.Single
                    && atomContainer.GetConnectedBonds(atom).Count() <= 4)
            {
                IAtomType type = GetAtomType("Be.2minus");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            else if (atom.FormalCharge == 0 && atomContainer.GetConnectedBonds(atom).Count() == 0)
            {
                IAtomType type = GetAtomType("Be.neutral");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            return null;
        }

        private IAtomType PerceiveCarbonRadicals(IAtomContainer atomContainer, IAtom atom)
        {
            if (atomContainer.GetConnectedBonds(atom).Count() == 0)
            {
                IAtomType type = GetAtomType("C.radical.planar");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            else if (atomContainer.GetConnectedBonds(atom).Count() <= 3)
            {
                BondOrder maxBondOrder = atomContainer.GetMaximumBondOrder(atom);
                if (maxBondOrder == BondOrder.Single)
                {
                    IAtomType type = GetAtomType("C.radical.planar");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
                else if (maxBondOrder == BondOrder.Double)
                {
                    IAtomType type = GetAtomType("C.radical.sp2");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
                else if (maxBondOrder == BondOrder.Triple)
                {
                    IAtomType type = GetAtomType("C.radical.sp1");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
            }
            return null;
        }

        private IAtomType PerceiveCarbons(IAtomContainer atomContainer, IAtom atom,
                                          RingSearch searcher, IList<IBond> connectedBonds)
        {
            if (HasOneSingleElectron(atomContainer, atom))
            {
                return PerceiveCarbonRadicals(atomContainer, atom);
            }
            if (connectedBonds == null) connectedBonds = atomContainer.GetConnectedBonds(atom).ToList();
            // if hybridization is given, use that
            if (HasHybridization(atom) && !IsCharged(atom))
            {
                if (atom.Hybridization == Hybridization.SP2)
                {
                    IAtomType type = GetAtomType("C.sp2");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                else if (atom.Hybridization == Hybridization.SP3)
                {
                    IAtomType type = GetAtomType("C.sp3");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                else if (atom.Hybridization == Hybridization.SP1)
                {
                    BondOrder maxBondOrder = GetMaximumBondOrder(connectedBonds);
                    if (maxBondOrder == BondOrder.Triple)
                    {
                        IAtomType type = GetAtomType("C.sp");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                    else
                    {
                        IAtomType type = GetAtomType("C.allene");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                }
            }
            else if (IsCharged(atom))
            {
                if (atom.FormalCharge == 1)
                {
                    if (connectedBonds.Count == 0)
                    {
                        IAtomType type = GetAtomType("C.plus.sp2");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                    else
                    {
                        BondOrder maxBondOrder = GetMaximumBondOrder(connectedBonds);
                        if (maxBondOrder == BondOrder.Triple)
                        {
                            IAtomType type = GetAtomType("C.plus.sp1");
                            if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                        }
                        else if (maxBondOrder == BondOrder.Double)
                        {
                            IAtomType type = GetAtomType("C.plus.sp2");
                            if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                        }
                        else if (maxBondOrder == BondOrder.Single)
                        {
                            IAtomType type = GetAtomType("C.plus.planar");
                            if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                        }
                    }
                }
                else if (atom.FormalCharge == -1)
                {
                    BondOrder maxBondOrder = GetMaximumBondOrder(connectedBonds);
                    if (maxBondOrder == BondOrder.Single && connectedBonds.Count <= 3)
                    {
                        if (BothNeighborsAreSp2(atom, atomContainer, connectedBonds) && IsRingAtom(atom, atomContainer, searcher))
                        {
                            IAtomType type = GetAtomType("C.minus.planar");
                            if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                        }
                        IAtomType typee = GetAtomType("C.minus.sp3");
                        if (IsAcceptable(atom, atomContainer, typee, connectedBonds)) return typee;
                    }
                    else if (maxBondOrder == BondOrder.Double
                          && connectedBonds.Count <= 3)
                    {
                        IAtomType type = GetAtomType("C.minus.sp2");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                    else if (maxBondOrder == BondOrder.Triple
                          && connectedBonds.Count <= 1)
                    {
                        IAtomType type = GetAtomType("C.minus.sp1");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                }
                return null;
            }
            else if (atom.IsAromatic)
            {
                IAtomType type = GetAtomType("C.sp2");
                if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
            }
            else if (HasOneOrMoreSingleOrDoubleBonds(connectedBonds))
            {
                IAtomType type = GetAtomType("C.sp2");
                if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
            }
            else if (connectedBonds.Count > 4)
            {
                // FIXME: I don't perceive carbons with more than 4 connections yet
                return null;
            }
            else
            { // OK, use bond order info
                BondOrder maxBondOrder = GetMaximumBondOrder(connectedBonds);
                if (maxBondOrder == BondOrder.Quadruple)
                {
                    // WTF??
                    return null;
                }
                else if (maxBondOrder == BondOrder.Triple)
                {
                    IAtomType type = GetAtomType("C.sp");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                else if (maxBondOrder == BondOrder.Double)
                {
                    // OK, one or two double bonds?
                    int doubleBondCount = CountAttachedDoubleBonds(connectedBonds, atom);
                    if (doubleBondCount == 2)
                    {
                        IAtomType type = GetAtomType("C.allene");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                    else if (doubleBondCount == 1)
                    {
                        IAtomType type = GetAtomType("C.sp2");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                }
                else
                {
                    if (HasAromaticBond(connectedBonds))
                    {
                        IAtomType type = GetAtomType("C.sp2");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                    IAtomType typee = GetAtomType("C.sp3");
                    if (IsAcceptable(atom, atomContainer, typee, connectedBonds)) return typee;
                }
            }
            return null;
        }

        private BondOrder GetMaximumBondOrder(IList<IBond> connectedBonds)
        {
            BondOrder max = BondOrder.Single;
            foreach (var bond in connectedBonds)
            {
                if (bond.Order.Numeric > max.Numeric)
                    max = bond.Order;
            }
            return max;
        }

        private bool HasOneOrMoreSingleOrDoubleBonds(IList<IBond> bonds)
        {
            foreach (var bond in bonds)
            {
                if (bond.IsSingleOrDouble) return true;
            }
            return false;
        }

        private bool HasOneSingleElectron(IAtomContainer atomContainer, IAtom atom)
        {
            return atomContainer.SingleElectrons.Any(n => n.Contains(atom));
        }

        private int CountSingleElectrons(IAtomContainer atomContainer, IAtom atom)
        {
            return atomContainer.SingleElectrons.Count(n => n.Contains(atom));
        }

        private IAtomType PerceiveOxygenRadicals(IAtomContainer atomContainer, IAtom atom)
        {
            if (atom.FormalCharge == 0)
            {
                if (atomContainer.GetConnectedBonds(atom).Count() <= 1)
                {
                    IAtomType type = GetAtomType("O.sp3.radical");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
            }
            else if (atom.FormalCharge == +1)
            {
                if (atomContainer.GetConnectedBonds(atom).Count() == 0)
                {
                    IAtomType type = GetAtomType("O.plus.radical");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
                else if (atomContainer.GetConnectedBonds(atom).Count() <= 2)
                {
                    BondOrder maxBondOrder = atomContainer.GetMaximumBondOrder(atom);
                    if (maxBondOrder == BondOrder.Single)
                    {
                        IAtomType type = GetAtomType("O.plus.radical");
                        if (IsAcceptable(atom, atomContainer, type)) return type;
                    }
                    else if (maxBondOrder == BondOrder.Double)
                    {
                        IAtomType type = GetAtomType("O.plus.sp2.radical");
                        if (IsAcceptable(atom, atomContainer, type)) return type;
                    }
                }
            }
            return null;
        }

        private bool IsCharged(IAtom atom)
        {
            return (atom.FormalCharge != null && atom.FormalCharge != 0);
        }

        private bool HasHybridization(IAtom atom)
        {
            return !atom.Hybridization.IsUnset;
        }

        private IAtomType PerceiveOxygens(IAtomContainer atomContainer, IAtom atom,
                                          RingSearch searcher, IList<IBond> connectedBonds)
        {
            if (HasOneSingleElectron(atomContainer, atom))
            {
                return PerceiveOxygenRadicals(atomContainer, atom);
            }

            // if hybridization is given, use that
            if (connectedBonds == null) connectedBonds = atomContainer.GetConnectedBonds(atom).ToList();
            if (HasHybridization(atom) && !IsCharged(atom))
            {
                if (atom.Hybridization == Hybridization.SP2)
                {
                    int connectedAtomsCount = connectedBonds.Count;
                    if (connectedAtomsCount == 1)
                    {
                        if (IsCarboxylate(atomContainer, atom, connectedBonds))
                        {
                            IAtomType type = GetAtomType("O.sp2.co2");
                            if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                        }
                        else
                        {
                            IAtomType type = GetAtomType("O.sp2");
                            if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                        }
                    }
                    else if (connectedAtomsCount == 2)
                    {
                        IAtomType type = GetAtomType("O.planar3");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                }
                else if (atom.Hybridization == Hybridization.SP3)
                {
                    IAtomType type = GetAtomType("O.sp3");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                else if (atom.Hybridization == Hybridization.Planar3)
                {
                    IAtomType type = GetAtomType("O.planar3");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
            }
            else if (IsCharged(atom))
            {
                if (atom.FormalCharge == -1 && connectedBonds.Count <= 1)
                {
                    if (IsCarboxylate(atomContainer, atom, connectedBonds))
                    {
                        IAtomType type = GetAtomType("O.minus.co2");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                    else
                    {
                        IAtomType type = GetAtomType("O.minus");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                }
                else if (atom.FormalCharge == -2 && connectedBonds.Count == 0)
                {
                    IAtomType type = GetAtomType("O.minus2");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                else if (atom.FormalCharge == +1)
                {
                    if (connectedBonds.Count == 0)
                    {
                        IAtomType type = GetAtomType("O.plus");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                    BondOrder maxBondOrder = GetMaximumBondOrder(connectedBonds);
                    if (maxBondOrder == BondOrder.Double)
                    {
                        IAtomType type = GetAtomType("O.plus.sp2");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                    else if (maxBondOrder == BondOrder.Triple)
                    {
                        IAtomType type = GetAtomType("O.plus.sp1");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                    else
                    {
                        IAtomType type = GetAtomType("O.plus");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                }
                return null;
            }
            else if (connectedBonds.Count > 2)
            {
                // FIXME: I don't perceive carbons with more than 4 connections yet
                return null;
            }
            else if (connectedBonds.Count == 0)
            {
                IAtomType type = GetAtomType("O.sp3");
                if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
            }
            else
            { // OK, use bond order info
                BondOrder maxBondOrder = GetMaximumBondOrder(connectedBonds);
                if (maxBondOrder == BondOrder.Double)
                {
                    if (IsCarboxylate(atomContainer, atom, connectedBonds))
                    {
                        IAtomType type = GetAtomType("O.sp2.co2");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                    else
                    {
                        IAtomType type = GetAtomType("O.sp2");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                }
                else if (maxBondOrder == BondOrder.Single)
                {
                    int explicitHydrogens = CountExplicitHydrogens(atom, connectedBonds);
                    int connectedHeavyAtoms = connectedBonds.Count - explicitHydrogens;
                    if (connectedHeavyAtoms == 2)
                    {
                        // a O.sp3 which is expected to take part in an aromatic system
                        if (BothNeighborsAreSp2(atom, atomContainer, connectedBonds) && IsRingAtom(atom, atomContainer, searcher))
                        {
                            IAtomType type = GetAtomType("O.planar3");
                            if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                        }
                        IAtomType typee = GetAtomType("O.sp3");
                        if (IsAcceptable(atom, atomContainer, typee, connectedBonds)) return typee;
                    }
                    else
                    {
                        IAtomType type = GetAtomType("O.sp3");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                }
            }
            return null;
        }

        private bool IsCarboxylate(IAtomContainer container, IAtom atom, IList<IBond> connectedBonds)
        {
            // assumes that the oxygen only has one neighbor (C=O, or C-[O-])
            if (connectedBonds.Count != 1) return false;
            IAtom carbon = connectedBonds.First().GetOther(atom);
            if (!"C".Equals(carbon.Symbol)) return false;

            var carbonBonds = container.GetConnectedBonds(carbon).ToList();
            if (carbonBonds.Count < 2) return false;
            int oxygenCount = 0;
            int singleBondedNegativeOxygenCount = 0;
            int doubleBondedOxygenCount = 0;
            foreach (var cBond in carbonBonds)
            {
                IAtom neighbor = cBond.GetOther(carbon);
                if ("O".Equals(neighbor.Symbol))
                {
                    oxygenCount++;
                    BondOrder order = cBond.Order;
                    int? charge = neighbor.FormalCharge;
                    if (order == BondOrder.Single && charge.HasValue && charge.Value == -1)
                    {
                        singleBondedNegativeOxygenCount++;
                    }
                    else if (order == BondOrder.Double)
                    {
                        doubleBondedOxygenCount++;
                    }
                }
            }
            return (oxygenCount == 2) && (singleBondedNegativeOxygenCount == 1) && (doubleBondedOxygenCount == 1);
        }

        private bool AtLeastTwoNeighborsAreSp2(IAtom atom, IAtomContainer atomContainer, IList<IBond> connectedBonds)
        {
            int count = 0;
            foreach (var bond in connectedBonds)
            {
                if (bond.Order == BondOrder.Double || bond.IsAromatic)
                {
                    count++;
                }
                else
                {
                    IAtom nextAtom = bond.GetOther(atom);
                    if (nextAtom.Hybridization == Hybridization.SP2)
                    {
                        // OK, it's SP2
                        count++;
                    }
                    else
                    {
                        var nextConnectBonds = atomContainer.GetConnectedBonds(nextAtom).ToList();
                        if (CountAttachedDoubleBonds(nextConnectBonds, nextAtom) > 0)
                        {
                            // OK, it's SP2
                            count++;
                        }
                    }
                }
                if (count >= 2) return true;
            }
            return false;
        }

        private bool BothNeighborsAreSp2(IAtom atom, IAtomContainer atomContainer, IList<IBond> connectedBonds)
        {
            return AtLeastTwoNeighborsAreSp2(atom, atomContainer, connectedBonds);
        }

        private IAtomType PerceiveNitrogenRadicals(IAtomContainer atomContainer, IAtom atom)
        {
            if (atomContainer.GetConnectedBonds(atom).Count() >= 1 && atomContainer.GetConnectedBonds(atom).Count() <= 2)
            {
                BondOrder maxBondOrder = atomContainer.GetMaximumBondOrder(atom);
                if (atom.FormalCharge != null && atom.FormalCharge == +1)
                {
                    if (maxBondOrder == BondOrder.Double)
                    {
                        IAtomType type = GetAtomType("N.plus.sp2.radical");
                        if (IsAcceptable(atom, atomContainer, type)) return type;
                    }
                    else if (maxBondOrder == BondOrder.Single)
                    {
                        IAtomType type = GetAtomType("N.plus.sp3.radical");
                        if (IsAcceptable(atom, atomContainer, type)) return type;
                    }
                }
                else if (atom.FormalCharge == null || atom.FormalCharge == 0)
                {
                    if (maxBondOrder == BondOrder.Single)
                    {
                        IAtomType type = GetAtomType("N.sp3.radical");
                        if (IsAcceptable(atom, atomContainer, type)) return type;
                    }
                    else if (maxBondOrder == BondOrder.Double)
                    {
                        IAtomType type = GetAtomType("N.sp2.radical");
                        if (IsAcceptable(atom, atomContainer, type)) return type;
                    }
                }
            }
            else
            {
                BondOrder maxBondOrder = atomContainer.GetMaximumBondOrder(atom);
                if (atom.FormalCharge != null && atom.FormalCharge == +1
                        && maxBondOrder == BondOrder.Single)
                {
                    IAtomType type = GetAtomType("N.plus.sp3.radical");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
            }
            return null;
        }

        private IAtomType PerceiveMolybdenum(IAtomContainer atomContainer, IAtom atom)
        {
            if (atom.FormalCharge != null && atom.FormalCharge == 0)
            {
                int neighbors = atomContainer.GetConnectedBonds(atom).Count();
                if (neighbors == 4)
                {
                    IAtomType type = GetAtomType("Mo.4");
                    if (IsAcceptable(atom, atomContainer, type))
                    {
                        return type;
                    }
                }
                IAtomType type1 = GetAtomType("Mo.metallic");
                if (IsAcceptable(atom, atomContainer, type1))
                {
                    return type1;
                }
            }
            return null;
        }

        private IAtomType PerceiveNitrogens(IAtomContainer atomContainer, IAtom atom, RingSearch searcher, IList<IBond> connectedBonds)
        {
            // if hybridization is given, use that
            if (HasOneSingleElectron(atomContainer, atom))
            {
                return PerceiveNitrogenRadicals(atomContainer, atom);
            }

            if (connectedBonds == null) connectedBonds = atomContainer.GetConnectedBonds(atom).ToList();
            if (HasHybridization(atom) && !IsCharged(atom))
            {
                if (atom.Hybridization == Hybridization.SP1)
                {
                    int neighborCount = connectedBonds.Count;
                    if (neighborCount > 1)
                    {
                        IAtomType type = GetAtomType("N.sp1.2");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                    else
                    {
                        IAtomType type = GetAtomType("N.sp1");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                }
                else if (atom.Hybridization == Hybridization.SP2)
                {
                    if (IsAmide(atom, atomContainer, connectedBonds))
                    {
                        IAtomType type = GetAtomType("N.amide");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                    else if (IsThioAmide(atom, atomContainer, connectedBonds))
                    {
                        IAtomType type = GetAtomType("N.thioamide");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                    // but an sp2 hyb N might N.sp2 or N.planar3 (pyrrole), so check for the latter
                    int neighborCount = connectedBonds.Count;
                    if (neighborCount == 4 && BondOrder.Double == GetMaximumBondOrder(connectedBonds))
                    {
                        IAtomType type = GetAtomType("N.oxide");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                    else if (neighborCount > 1 && BothNeighborsAreSp2(atom, atomContainer, connectedBonds))
                    {
                        if (IsRingAtom(atom, atomContainer, searcher))
                        {
                            if (neighborCount == 3)
                            {
                                BondOrder maxOrder = GetMaximumBondOrder(connectedBonds);
                                if (maxOrder == BondOrder.Double)
                                {
                                    IAtomType type = GetAtomType("N.sp2.3");
                                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                                }
                                else if (maxOrder == BondOrder.Single)
                                {
                                    IAtomType type = GetAtomType("N.planar3");
                                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                                }
                            }
                            else if (neighborCount == 2)
                            {
                                BondOrder maxOrder = GetMaximumBondOrder(connectedBonds);
                                if (maxOrder == BondOrder.Single)
                                {
                                    if (atom.ImplicitHydrogenCount != null
                                            && atom.ImplicitHydrogenCount == 1)
                                    {
                                        IAtomType type = GetAtomType("N.planar3");
                                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                                    }
                                    else
                                    {
                                        IAtomType type = GetAtomType("N.sp2");
                                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                                    }
                                }
                                else if (maxOrder == BondOrder.Double)
                                {
                                    IAtomType type = GetAtomType("N.sp2");
                                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                                }
                            }
                        }
                    }
                    IAtomType typee = GetAtomType("N.sp2");
                    if (IsAcceptable(atom, atomContainer, typee, connectedBonds)) return typee;
                }
                else if (atom.Hybridization == Hybridization.SP3)
                {
                    IAtomType type = GetAtomType("N.sp3");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                else if (atom.Hybridization == Hybridization.Planar3)
                {
                    BondOrder maxBondOrder = GetMaximumBondOrder(connectedBonds);
                    if (connectedBonds.Count == 3 && maxBondOrder == BondOrder.Double
                            && CountAttachedDoubleBonds(connectedBonds, atom, "O") == 2)
                    {
                        IAtomType type = GetAtomType("N.nitro");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                    IAtomType typee = GetAtomType("N.planar3");
                    if (IsAcceptable(atom, atomContainer, typee, connectedBonds)) return typee;
                }
            }
            else if (IsCharged(atom))
            {
                if (atom.FormalCharge == 1)
                {
                    BondOrder maxBondOrder = GetMaximumBondOrder(connectedBonds);
                    if (maxBondOrder == BondOrder.Single || connectedBonds.Count == 0)
                    {
                        if (atom.Hybridization == Hybridization.SP2)
                        {
                            IAtomType type = GetAtomType("N.plus.sp2");
                            if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                        }
                        IAtomType typee = GetAtomType("N.plus");
                        if (IsAcceptable(atom, atomContainer, typee, connectedBonds)) return typee;
                    }
                    else if (maxBondOrder == BondOrder.Double)
                    {
                        int doubleBonds = CountAttachedDoubleBonds(connectedBonds, atom);
                        if (doubleBonds == 1)
                        {
                            IAtomType type = GetAtomType("N.plus.sp2");
                            if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                        }
                        else if (doubleBonds == 2)
                        {
                            IAtomType type = GetAtomType("N.plus.sp1");
                            if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                        }
                    }
                    else if (maxBondOrder == BondOrder.Triple)
                    {
                        if (connectedBonds.Count == 2)
                        {
                            IAtomType type = GetAtomType("N.plus.sp1");
                            if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                        }
                    }
                }
                else if (atom.FormalCharge == -1)
                {
                    BondOrder maxBondOrder = GetMaximumBondOrder(connectedBonds);
                    if (maxBondOrder == BondOrder.Single)
                    {
                        if (connectedBonds.Count >= 2 && BothNeighborsAreSp2(atom, atomContainer, connectedBonds)
                                && IsRingAtom(atom, atomContainer, searcher))
                        {
                            IAtomType type = GetAtomType("N.minus.planar3");
                            if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                        }
                        else if (connectedBonds.Count <= 2)
                        {
                            IAtomType type = GetAtomType("N.minus.sp3");
                            if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                        }
                    }
                    else if (maxBondOrder == BondOrder.Double)
                    {
                        if (connectedBonds.Count <= 1)
                        {
                            IAtomType type = GetAtomType("N.minus.sp2");
                            if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                        }
                    }
                }
            }
            else if (connectedBonds.Count > 3)
            {
                if (connectedBonds.Count == 4 && CountAttachedDoubleBonds(connectedBonds, atom) == 1)
                {
                    IAtomType type = GetAtomType("N.oxide");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                return null;
            }
            else if (!connectedBonds.Any())
            {
                IAtomType type = GetAtomType("N.sp3");
                if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
            }
            else if (HasOneOrMoreSingleOrDoubleBonds(connectedBonds))
            {
                int connectedAtoms = connectedBonds.Count
                        + atom.ImplicitHydrogenCount ?? 0;
                if (connectedAtoms == 3)
                {
                    IAtomType type = GetAtomType("N.planar3");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                IAtomType typee = GetAtomType("N.sp2");
                if (IsAcceptable(atom, atomContainer, typee, connectedBonds)) return typee;
            }
            else
            { // OK, use bond order info
                BondOrder maxBondOrder = GetMaximumBondOrder(connectedBonds);
                if (maxBondOrder == BondOrder.Single)
                {
                    if (IsAmide(atom, atomContainer, connectedBonds))
                    {
                        IAtomType type = GetAtomType("N.amide");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                    else if (IsThioAmide(atom, atomContainer, connectedBonds))
                    {
                        IAtomType type = GetAtomType("N.thioamide");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }

                    IList<IBond> heavy = HeavyBonds(connectedBonds);

                    int expHCount = heavy.Count() - connectedBonds.Count;

                    if (heavy.Count() == 2)
                    {

                        if (heavy[0].IsAromatic && heavy[1].IsAromatic)
                        {

                            int hCount = atom.ImplicitHydrogenCount ?? 0 + expHCount;
                            if (hCount == 0)
                            {
                                if (maxBondOrder == BondOrder.Single
                                        && IsSingleHeteroAtom(atom, atomContainer))
                                {
                                    IAtomType type = GetAtomType("N.planar3");
                                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                                }
                                else
                                {
                                    IAtomType type = GetAtomType("N.sp2");
                                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                                }
                            }
                            else if (hCount == 1)
                            {
                                IAtomType type = GetAtomType("N.planar3");
                                if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                            }
                        }
                        else if (BothNeighborsAreSp2(atom, atomContainer, connectedBonds) && IsRingAtom(atom, atomContainer, searcher))
                        {
                            // a N.sp3 which is expected to take part in an aromatic system
                            IAtomType type = GetAtomType("N.planar3");
                            if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                        }
                        else
                        {
                            IAtomType type = GetAtomType("N.sp3");
                            if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                        }
                    }
                    else if (heavy.Count() == 3)
                    {
                        if (BothNeighborsAreSp2(atom, atomContainer, connectedBonds) && IsRingAtom(atom, atomContainer, searcher))
                        {
                            IAtomType type = GetAtomType("N.planar3");
                            if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                        }
                        IAtomType typee = GetAtomType("N.sp3");
                        if (IsAcceptable(atom, atomContainer, typee, connectedBonds)) return typee;
                    }
                    else if (heavy.Count() == 1)
                    {
                        IAtomType type = GetAtomType("N.sp3");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                    else if (heavy.Count() == 0)
                    {
                        IAtomType type = GetAtomType("N.sp3");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                }
                else if (maxBondOrder == BondOrder.Double)
                {
                    if (connectedBonds.Count == 3
                            && CountAttachedDoubleBonds(connectedBonds, atom, "O") == 2)
                    {
                        IAtomType type = GetAtomType("N.nitro");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                    else if (connectedBonds.Count == 3
                          && CountAttachedDoubleBonds(connectedBonds, atom) > 0)
                    {
                        IAtomType type = GetAtomType("N.sp2.3");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                    IAtomType typee = GetAtomType("N.sp2");
                    if (IsAcceptable(atom, atomContainer, typee, connectedBonds)) return typee;
                }
                else if (maxBondOrder == BondOrder.Triple)
                {
                    int neighborCount = connectedBonds.Count;
                    if (neighborCount > 1)
                    {
                        IAtomType type = GetAtomType("N.sp1.2");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                    else
                    {
                        IAtomType type = GetAtomType("N.sp1");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Determines whether the bonds (up to two spheres away) are only to non
        /// hetroatoms. Currently used in N.planar3 perception of (e.g. pyrrole).
        /// </summary>
        /// <param name="atom">an atom to test</param>
        /// <param name="container">container of the atom</param>
        /// <returns>whether the atom's only bonds are to heteroatoms</returns>
        /// <seealso cref="PerceiveNitrogens(IAtomContainer, IAtom, RingSearch, IList{IBond})"/>
        private bool IsSingleHeteroAtom(IAtom atom, IAtomContainer container)
        {
            var connected = container.GetConnectedAtoms(atom);

            foreach (var atom1 in connected)
            {
                bool aromatic = container.GetBond(atom, atom1).IsAromatic;

                // ignoring non-aromatic bonds
                if (!aromatic) continue;

                // found a hetroatom - we're not a single hetroatom
                if (!"C".Equals(atom1.Symbol)) return false;

                // check the second sphere
                foreach (var atom2 in container.GetConnectedAtoms(atom1))
                {
                    if (atom2.Equals(atom) && container.GetBond(atom1, atom2).IsAromatic
                            && !"C".Equals(atom2.Symbol))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool IsRingAtom(IAtom atom, IAtomContainer atomContainer, RingSearch searcher)
        {
            if (searcher == null) searcher = new RingSearch(atomContainer);
            return searcher.Cyclic(atom);
        }

        private bool IsAmide(IAtom atom, IAtomContainer atomContainer, IList<IBond> connectedBonds)
        {
            if (connectedBonds.Count < 1) return false;
            foreach (var bond in connectedBonds)
            {
                IAtom neighbor = bond.GetOther(atom);
                if (neighbor.Symbol.Equals("C"))
                {
                    if (CountAttachedDoubleBonds(atomContainer.GetConnectedBonds(neighbor).ToList(), neighbor, "O") == 1) return true;
                }
            }
            return false;
        }

        private bool IsThioAmide(IAtom atom, IAtomContainer atomContainer, IList<IBond> connectedBonds)
        {
            if (connectedBonds.Count < 1) return false;
            foreach (var bond in connectedBonds)
            {
                IAtom neighbor = bond.GetOther(atom);
                if (neighbor.Symbol.Equals("C"))
                {
                    if (CountAttachedDoubleBonds(atomContainer.GetConnectedBonds(neighbor).ToList(), neighbor, "S") == 1) return true;
                }
            }
            return false;
        }

        private int CountExplicitHydrogens(IAtom atom, IList<IBond> connectedBonds)
        {
            int count = 0;
            foreach (var bond in connectedBonds)
            {
                IAtom aAtom = bond.GetOther(atom);
                if (aAtom.Symbol.Equals("H"))
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Filter a bond list keeping only bonds between heavy atoms.
        /// </summary>
        /// <param name="bonds">a list of bond</param>
        /// <returns>the bond list only with heavy bonds</returns>
        private IList<IBond> HeavyBonds(IList<IBond> bonds)
        {
            IList<IBond> heavy = new List<IBond>(bonds.Count);
            foreach (var bond in bonds)
            {
                if (!(bond.Begin.Symbol.Equals("H") && bond.End.Symbol.Equals("H")))
                {
                    heavy.Add(bond);
                }
            }
            return heavy;
        }

        private IAtomType PerceiveIron(IAtomContainer atomContainer, IAtom atom)
        {
            if ("Fe".Equals(atom.Symbol))
            {
                if (HasOneSingleElectron(atomContainer, atom))
                {
                    // no idea how to deal with this yet
                    return null;
                }
                else if ((atom.FormalCharge != null && atom.FormalCharge == 0))
                {
                    int neighbors = atomContainer.GetConnectedBonds(atom).Count();
                    if (neighbors == 0)
                    {
                        IAtomType type = GetAtomType("Fe.metallic");
                        if (IsAcceptable(atom, atomContainer, type))
                        {
                            return type;
                        }
                    }
                    else if (neighbors == 2)
                    {
                        IAtomType type5 = GetAtomType("Fe.2");
                        if (IsAcceptable(atom, atomContainer, type5))
                        {
                            return type5;
                        }
                    }
                    else if (neighbors == 3)
                    {
                        IAtomType type6 = GetAtomType("Fe.3");
                        if (IsAcceptable(atom, atomContainer, type6))
                        {
                            return type6;
                        }
                    }
                    else if (neighbors == 4)
                    {
                        IAtomType type7 = GetAtomType("Fe.4");
                        if (IsAcceptable(atom, atomContainer, type7))
                        {
                            return type7;
                        }
                    }
                    else if (neighbors == 5)
                    {
                        IAtomType type8 = GetAtomType("Fe.5");
                        if (IsAcceptable(atom, atomContainer, type8))
                        {
                            return type8;
                        }
                    }
                    else if (neighbors == 6)
                    {
                        IAtomType type9 = GetAtomType("Fe.6");
                        if (IsAcceptable(atom, atomContainer, type9))
                        {
                            return type9;
                        }
                    }
                }
                else if ((atom.FormalCharge != null && atom.FormalCharge == 2))
                {
                    int neighbors = atomContainer.GetConnectedBonds(atom).Count();
                    if (neighbors <= 1)
                    {
                        IAtomType type = GetAtomType("Fe.2plus");
                        if (IsAcceptable(atom, atomContainer, type))
                        {
                            return type;
                        }
                    }
                }
                else if ((atom.FormalCharge != null && atom.FormalCharge == 1))
                {
                    int neighbors = atomContainer.GetConnectedBonds(atom).Count();

                    if (neighbors == 2)
                    {
                        IAtomType type0 = GetAtomType("Fe.plus");
                        if (IsAcceptable(atom, atomContainer, type0))
                        {
                            return type0;
                        }
                    }
                }
                else if ((atom.FormalCharge != null && atom.FormalCharge == 3))
                {
                    IAtomType type1 = GetAtomType("Fe.3plus");
                    if (IsAcceptable(atom, atomContainer, type1))
                    {
                        return type1;
                    }
                }
                else if ((atom.FormalCharge != null && atom.FormalCharge == -2))
                {
                    IAtomType type2 = GetAtomType("Fe.2minus");
                    if (IsAcceptable(atom, atomContainer, type2))
                    {
                        return type2;
                    }
                }
                else if ((atom.FormalCharge != null && atom.FormalCharge == -3))
                {
                    IAtomType type3 = GetAtomType("Fe.3minus");
                    if (IsAcceptable(atom, atomContainer, type3))
                    {
                        return type3;
                    }
                }
                else if ((atom.FormalCharge != null && atom.FormalCharge == -4))
                {
                    IAtomType type4 = GetAtomType("Fe.4minus");
                    if (IsAcceptable(atom, atomContainer, type4))
                    {
                        return type4;
                    }
                }
            }
            return null;
        }

        private IAtomType PerceiveMercury(IAtomContainer atomContainer, IAtom atom)
        {
            if ("Hg".Equals(atom.Symbol))
            {
                if (HasOneSingleElectron(atomContainer, atom))
                {
                    // no idea how to deal with this yet
                    return null;
                }
                else if ((atom.FormalCharge != null && atom.FormalCharge == -1))
                {
                    IAtomType type = GetAtomType("Hg.minus");
                    if (IsAcceptable(atom, atomContainer, type))
                    {
                        return type;
                    }
                }
                else if ((atom.FormalCharge != null && atom.FormalCharge == 2))
                {
                    IAtomType type = GetAtomType("Hg.2plus");
                    if (IsAcceptable(atom, atomContainer, type))
                    {
                        return type;
                    }
                }
                else if ((atom.FormalCharge != null && atom.FormalCharge == +1))
                {
                    int neighbors = atomContainer.GetConnectedBonds(atom).Count();
                    if (neighbors <= 1)
                    {
                        IAtomType type = GetAtomType("Hg.plus");
                        if (IsAcceptable(atom, atomContainer, type))
                        {
                            return type;
                        }
                    }
                }
                else if ((atom.FormalCharge != null && atom.FormalCharge == 0))
                {
                    int neighbors = atomContainer.GetConnectedBonds(atom).Count();
                    if (neighbors == 2)
                    {
                        IAtomType type = GetAtomType("Hg.2");
                        if (IsAcceptable(atom, atomContainer, type))
                        {
                            return type;
                        }
                    }
                    else if (neighbors == 1)
                    {
                        IAtomType type = GetAtomType("Hg.1");
                        if (IsAcceptable(atom, atomContainer, type))
                        {
                            return type;
                        }
                    }
                    else if (neighbors == 0)
                    {
                        IAtomType type = GetAtomType("Hg.metallic");
                        if (IsAcceptable(atom, atomContainer, type))
                        {
                            return type;
                        }
                    }
                }
            }
            return null;
        }

        private IAtomType PerceiveSulphurs(IAtomContainer atomContainer, IAtom atom,
                                           RingSearch searcher, IList<IBond> connectedBonds)
        {
            if (connectedBonds == null) connectedBonds = atomContainer.GetConnectedBonds(atom).ToList();
            BondOrder maxBondOrder = GetMaximumBondOrder(connectedBonds);
            int neighborcount = connectedBonds.Count;
            if (HasOneSingleElectron(atomContainer, atom))
            {
                // no idea how to deal with this yet
                return null;
            }
            else if (atom.Hybridization == Hybridization.SP2
                  && atom.FormalCharge != null && atom.FormalCharge == +1)
            {
                if (neighborcount == 3)
                {
                    IAtomType type = GetAtomType("S.inyl.charged");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                else
                {
                    IAtomType type = GetAtomType("S.plus");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
            }
            else if (atom.FormalCharge != null && atom.FormalCharge != 0)
            {

                if (atom.FormalCharge == -1 && neighborcount == 1)
                {
                    IAtomType type = GetAtomType("S.minus");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                else if (atom.FormalCharge == +1 && neighborcount == 2)
                {
                    IAtomType type = GetAtomType("S.plus");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                else if (atom.FormalCharge == +1 && neighborcount == 3)
                {
                    IAtomType type = GetAtomType("S.inyl.charged");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                else if (atom.FormalCharge == +2 && neighborcount == 4)
                {
                    IAtomType type = GetAtomType("S.onyl.charged");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                else if (atom.FormalCharge == -2 && neighborcount == 0)
                {
                    IAtomType type = GetAtomType("S.2minus");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
            }
            else if (neighborcount == 0)
            {
                if (atom.FormalCharge != null && atom.FormalCharge == 0)
                {
                    IAtomType type = GetAtomType("S.3");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
            }
            else if (neighborcount == 1)
            {
                if (connectedBonds.First().Order == BondOrder.Double)
                {
                    IAtomType type = GetAtomType("S.2");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                else if (connectedBonds.First().Order == BondOrder.Single)
                {
                    IAtomType type = GetAtomType("S.3");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
            }
            else if (neighborcount == 2)
            {
                if (BothNeighborsAreSp2(atom, atomContainer, connectedBonds) && IsRingAtom(atom, atomContainer, searcher))
                {
                    if (CountAttachedDoubleBonds(connectedBonds, atom) == 2)
                    {
                        IAtomType type = GetAtomType("S.inyl.2");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                    else
                    {
                        IAtomType type = GetAtomType("S.planar3");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                }
                else if (CountAttachedDoubleBonds(connectedBonds, atom, "O") == 2)
                {
                    IAtomType type = GetAtomType("S.oxide");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                else if (CountAttachedDoubleBonds(connectedBonds, atom) == 2)
                {
                    IAtomType type = GetAtomType("S.inyl.2");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                else if (CountAttachedDoubleBonds(connectedBonds, atom) <= 1)
                {
                    IAtomType type = GetAtomType("S.3");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                else if (CountAttachedDoubleBonds(connectedBonds, atom) == 0
                      && CountAttachedSingleBonds(connectedBonds, atom) == 2)
                {
                    IAtomType type = GetAtomType("S.octahedral");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
            }
            else if (neighborcount == 3)
            {
                int doubleBondedAtoms = CountAttachedDoubleBonds(connectedBonds, atom);
                if (doubleBondedAtoms == 1)
                {
                    IAtomType type = GetAtomType("S.inyl");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                else if (doubleBondedAtoms == 3)
                {
                    IAtomType type = GetAtomType("S.trioxide");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                else if (doubleBondedAtoms == 0)
                {
                    IAtomType type = GetAtomType("S.anyl");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
            }
            else if (neighborcount == 4)
            {
                // count the number of double bonded oxygens
                int doubleBondedOxygens = CountAttachedDoubleBonds(connectedBonds, atom, "O");
                int doubleBondedNitrogens = CountAttachedDoubleBonds(connectedBonds, atom, "N");
                int doubleBondedSulphurs = CountAttachedDoubleBonds(connectedBonds, atom, "S");
                int countAttachedDoubleBonds = CountAttachedDoubleBonds(connectedBonds, atom);

                if (doubleBondedOxygens + doubleBondedNitrogens == 2)
                {
                    IAtomType type = GetAtomType("S.onyl");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                else if (doubleBondedSulphurs == 1 && doubleBondedOxygens == 1)
                {
                    IAtomType type = GetAtomType("S.thionyl");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                else if (maxBondOrder == BondOrder.Single)
                {
                    IAtomType type = GetAtomType("S.anyl");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                else if (doubleBondedOxygens == 1)
                {
                    IAtomType type = GetAtomType("S.sp3d1");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                else if (countAttachedDoubleBonds == 2 && maxBondOrder == BondOrder.Double)
                {
                    IAtomType type = GetAtomType("S.sp3.4");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }

            }
            else if (neighborcount == 5)
            {

                if (maxBondOrder == BondOrder.Double)
                {

                    IAtomType type = GetAtomType("S.sp3d1");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                else if (maxBondOrder == BondOrder.Single)
                {
                    IAtomType type = GetAtomType("S.octahedral");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
            }
            else if (neighborcount == 6)
            {
                if (maxBondOrder == BondOrder.Single)
                {
                    IAtomType type = GetAtomType("S.octahedral");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
            }
            return null;
        }

        private IAtomType PerceivePhosphors(IAtomContainer atomContainer, IAtom atom, IList<IBond> connectedBonds)
        {
            if (connectedBonds == null) connectedBonds = atomContainer.GetConnectedBonds(atom).ToList();
            int neighborcount = connectedBonds.Count;
            BondOrder maxBondOrder = GetMaximumBondOrder(connectedBonds);
            if (CountSingleElectrons(atomContainer, atom) == 3)
            {
                IAtomType type = GetAtomType("P.se.3");
                if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
            }
            else if (HasOneSingleElectron(atomContainer, atom))
            {
                // no idea how to deal with this yet
                return null;
            }
            else if (neighborcount == 0)
            {
                if (atom.FormalCharge == null || atom.FormalCharge.Value == 0)
                {
                    IAtomType type = GetAtomType("P.ine");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
            }
            else if (neighborcount == 1)
            {
                if (atom.FormalCharge == null || atom.FormalCharge.Value == 0)
                {
                    IAtomType type = GetAtomType("P.ide");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
            }
            else if (neighborcount == 3)
            {
                int doubleBonds = CountAttachedDoubleBonds(connectedBonds, atom);
                if (atom.FormalCharge != null && atom.FormalCharge.Value == 1)
                {
                    IAtomType type = GetAtomType("P.anium");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                else if (doubleBonds == 1)
                {
                    IAtomType type = GetAtomType("P.ate");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                else
                {
                    IAtomType type = GetAtomType("P.ine");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
            }
            else if (neighborcount == 2)
            {
                if (maxBondOrder == BondOrder.Double)
                {
                    if (atom.FormalCharge != null && atom.FormalCharge.Value == 1)
                    {
                        IAtomType type = GetAtomType("P.sp1.plus");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                    else
                    {
                        IAtomType type = GetAtomType("P.irane");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                }
                else if (maxBondOrder == BondOrder.Single)
                {
                    IAtomType type = GetAtomType("P.ine");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
            }
            else if (neighborcount == 4)
            {
                // count the number of double bonded oxygens
                int doubleBonds = CountAttachedDoubleBonds(connectedBonds, atom);
                if (atom.FormalCharge == 1 && doubleBonds == 0)
                {
                    IAtomType type = GetAtomType("P.ate.charged");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                else if (doubleBonds == 1)
                {
                    IAtomType type = GetAtomType("P.ate");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
            }
            else if (neighborcount == 5)
            {
                if (atom.FormalCharge == null || atom.FormalCharge.Value == 0)
                {
                    IAtomType type = GetAtomType("P.ane");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
            }
            return null;
        }

        private IAtomType PerceiveHydrogens(IAtomContainer atomContainer, IAtom atom, IList<IBond> connectedBonds)
        {
            if (connectedBonds == null) connectedBonds = atomContainer.GetConnectedBonds(atom).ToList();
            int neighborcount = connectedBonds.Count;
            if (HasOneSingleElectron(atomContainer, atom))
            {
                if ((atom.FormalCharge == null || atom.FormalCharge == 0) && neighborcount == 0)
                {
                    IAtomType type = GetAtomType("H.radical");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                return null;
            }
            else if (neighborcount == 2)
            {
                // FIXME: bridging hydrogen as in B2H6
                return null;
            }
            else if (neighborcount == 1)
            {
                if (atom.FormalCharge == null || atom.FormalCharge == 0)
                {
                    IAtomType type = GetAtomType("H");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
            }
            else if (neighborcount == 0)
            {
                if (atom.FormalCharge == null || atom.FormalCharge == 0)
                {
                    IAtomType type = GetAtomType("H");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                else if (atom.FormalCharge == 1)
                {
                    IAtomType type = GetAtomType("H.plus");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                else if (atom.FormalCharge == -1)
                {
                    IAtomType type = GetAtomType("H.minus");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
            }
            return null;
        }

        private IAtomType PerceiveLithium(IAtomContainer atomContainer, IAtom atom)
        {
            int neighborcount = atomContainer.GetConnectedBonds(atom).Count();
            if (neighborcount == 1)
            {
                if (atom.FormalCharge == null || atom.FormalCharge == 0)
                {
                    IAtomType type = GetAtomType("Li");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
            }
            else if (neighborcount == 0)
            {
                if (atom.FormalCharge == null || atom.FormalCharge == 0)
                {
                    IAtomType type = GetAtomType("Li.neutral");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
                if (atom.FormalCharge == null || atom.FormalCharge == +1)
                {
                    IAtomType type = GetAtomType("Li.plus");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
            }
            return null;
        }

        private IAtomType PerceiveHalogens(IAtomContainer atomContainer, IAtom atom, IList<IBond> connectedBonds)
        {
            if (connectedBonds == null) connectedBonds = atomContainer.GetConnectedBonds(atom).ToList();
            if ("F".Equals(atom.Symbol))
            {
                if (HasOneSingleElectron(atomContainer, atom))
                {
                    if (connectedBonds.Count == 0)
                    {
                        if (atom.FormalCharge != null && atom.FormalCharge == +1)
                        {
                            IAtomType type = GetAtomType("F.plus.radical");
                            if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                        }
                        else if (atom.FormalCharge == null || atom.FormalCharge == 0)
                        {
                            IAtomType type = GetAtomType("F.radical");
                            if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                        }
                    }
                    else if (connectedBonds.Count <= 1)
                    {
                        BondOrder maxBondOrder = GetMaximumBondOrder(connectedBonds);
                        if (maxBondOrder == BondOrder.Single)
                        {
                            IAtomType type = GetAtomType("F.plus.radical");
                            if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                        }
                    }
                    return null;
                }
                else if (atom.FormalCharge != null && atom.FormalCharge != 0)
                {
                    if (atom.FormalCharge == -1)
                    {
                        IAtomType type = GetAtomType("F.minus");
                        if (IsAcceptable(atom, atomContainer, type)) return type;
                    }
                    else if (atom.FormalCharge == 1)
                    {
                        BondOrder maxBondOrder = GetMaximumBondOrder(connectedBonds);
                        if (maxBondOrder == BondOrder.Double)
                        {
                            IAtomType type = GetAtomType("F.plus.sp2");
                            if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                        }
                        else if (maxBondOrder == BondOrder.Single)
                        {
                            IAtomType type = GetAtomType("F.plus.sp3");
                            if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                        }
                    }
                }
                else if (connectedBonds.Count == 1 || connectedBonds.Count == 0)
                {
                    IAtomType type = GetAtomType("F");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
            }
            else if ("I".Equals(atom.Symbol))
            {
                return PerceiveIodine(atomContainer, atom, connectedBonds);
            }

            return null;
        }

        private IAtomType PerceiveArsenic(IAtomContainer atomContainer, IAtom atom)
        {
            if (HasOneSingleElectron(atomContainer, atom))
            {
                // no idea how to deal with this yet
                return null;
            }
            else if ((atom.FormalCharge != null && atom.FormalCharge == +1 && atomContainer
                  .GetConnectedBonds(atom).Count() <= 4))
            {
                IAtomType type = GetAtomType("As.plus");
                if (IsAcceptable(atom, atomContainer, type))
                {
                    return type;
                }
            }
            else if ((atom.FormalCharge != null && atom.FormalCharge == 0))
            {
                int neighbors = atomContainer.GetConnectedBonds(atom).Count();
                if (neighbors == 4)
                {
                    IAtomType type = GetAtomType("As.5");
                    if (IsAcceptable(atom, atomContainer, type))
                    {
                        return type;
                    }
                }
                if (neighbors == 2)
                {
                    IAtomType type = GetAtomType("As.2");
                    if (IsAcceptable(atom, atomContainer, type))
                    {
                        return type;
                    }
                }
                IAtomType typee = GetAtomType("As");
                if (IsAcceptable(atom, atomContainer, typee))
                {
                    return typee;
                }
            }
            else if ((atom.FormalCharge != null && atom.FormalCharge == +3))
            {
                IAtomType type = GetAtomType("As.3plus");
                if (IsAcceptable(atom, atomContainer, type))
                {
                    return type;
                }
            }
            else if ((atom.FormalCharge != null && atom.FormalCharge == -1))
            {
                IAtomType type = GetAtomType("As.minus");
                if (IsAcceptable(atom, atomContainer, type))
                {
                    return type;
                }
            }
            return null;
        }

        private IAtomType PerceiveThorium(IAtomContainer atomContainer, IAtom atom)
        {
            if ("Th".Equals(atom.Symbol))
            {
                if (atom.FormalCharge == 0 && atomContainer.GetConnectedBonds(atom).Count() == 0)
                {
                    IAtomType type = GetAtomType("Th");
                    if (IsAcceptable(atom, atomContainer, type))
                    {
                        return type;
                    }
                }
            }
            return null;
        }

        private IAtomType PerceiveRubidium(IAtomContainer atomContainer, IAtom atom)
        {
            if (HasOneSingleElectron(atomContainer, atom))
            {
                return null;
            }
            else if (atom.FormalCharge != null && atom.FormalCharge == +1)
            {
                IAtomType type = GetAtomType("Rb.plus");
                if (IsAcceptable(atom, atomContainer, type))
                {
                    return type;
                }
            }
            else if (atom.FormalCharge != null && atom.FormalCharge == 0)
            {
                IAtomType type = GetAtomType("Rb.neutral");
                if (IsAcceptable(atom, atomContainer, type))
                {
                    return type;
                }
            }
            return null;
        }

        private IAtomType PerceiveCommonSalts(IAtomContainer atomContainer, IAtom atom)
        {
            if ("Mg".Equals(atom.Symbol))
            {
                if (HasOneSingleElectron(atomContainer, atom))
                {
                    // no idea how to deal with this yet
                    return null;
                }
                else if ((atom.FormalCharge != null && atom.FormalCharge == +2))
                {
                    IAtomType type = GetAtomType("Mg.2plus");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
            }
            else if ("Co".Equals(atom.Symbol))
            {
                if (HasOneSingleElectron(atomContainer, atom))
                {
                    // no idea how to deal with this yet
                    return null;
                }
                else if ((atom.FormalCharge != null && atom.FormalCharge == +2))
                {
                    IAtomType type = GetAtomType("Co.2plus");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
                else if ((atom.FormalCharge != null && atom.FormalCharge == +3))
                {
                    IAtomType type = GetAtomType("Co.3plus");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
                else if ((atom.FormalCharge == null || atom.FormalCharge == 0))
                {
                    IAtomType type = GetAtomType("Co.metallic");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
            }
            else if ("W".Equals(atom.Symbol))
            {
                if (HasOneSingleElectron(atomContainer, atom))
                {
                    // no idea how to deal with this yet
                    return null;
                }
                else if ((atom.FormalCharge == null || atom.FormalCharge == 0))
                {
                    IAtomType type = GetAtomType("W.metallic");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
            }
            return null;
        }

        private IAtomType PerceiveCopper(IAtomContainer atomContainer, IAtom atom)
        {
            if (HasOneSingleElectron(atomContainer, atom))
            {
                // no idea how to deal with this yet
                return null;
            }
            else if ((atom.FormalCharge != null && atom.FormalCharge == +2))
            {
                IAtomType type = GetAtomType("Cu.2plus");
                if (IsAcceptable(atom, atomContainer, type))
                {
                    return type;
                }
            }
            else if (atom.FormalCharge != null && atom.FormalCharge == 0)
            {
                int neighbors = atomContainer.GetConnectedBonds(atom).Count();
                if (neighbors == 1)
                {
                    IAtomType type = GetAtomType("Cu.1");
                    if (IsAcceptable(atom, atomContainer, type))
                    {
                        return type;
                    }
                }
                else
                {
                    IAtomType type01 = GetAtomType("Cu.metallic");
                    if (IsAcceptable(atom, atomContainer, type01))
                    {
                        return type01;
                    }
                }
            }
            else if (atom.FormalCharge != null && atom.FormalCharge == +1)
            {
                IAtomType type02 = GetAtomType("Cu.plus");
                if (IsAcceptable(atom, atomContainer, type02))
                {
                    return type02;
                }
            }
            return null;
        }

        private IAtomType PerceiveBarium(IAtomContainer atomContainer, IAtom atom)
        {
            if (HasOneSingleElectron(atomContainer, atom))
            {
                return null;
            }
            else if ((atom.FormalCharge != null && atom.FormalCharge == 2))
            {
                IAtomType type = GetAtomType("Ba.2plus");
                if (IsAcceptable(atom, atomContainer, type))
                {
                    return type;
                }
            }
            return null;
        }

        private IAtomType PerceiveAluminium(IAtomContainer atomContainer, IAtom atom)
        {
            if (atom.FormalCharge != null && atom.FormalCharge == 3)
            {
                int connectedBondsCount = atomContainer.GetConnectedBonds(atom).Count();
                if (connectedBondsCount == 0)
                {
                    IAtomType type = GetAtomType("Al.3plus");
                    if (IsAcceptable(atom, atomContainer, type))
                    {
                        return type;
                    }
                }
            }
            else if (atom.FormalCharge != null && atom.FormalCharge == 0
                  && atomContainer.GetConnectedBonds(atom).Count() == 3)
            {
                IAtomType type = GetAtomType("Al");
                if (IsAcceptable(atom, atomContainer, type))
                {
                    return type;
                }
            }
            else if (atom.FormalCharge != null && atom.FormalCharge == -3
                  && atomContainer.GetConnectedBonds(atom).Count() == 6)
            {
                IAtomType type = GetAtomType("Al.3minus");
                if (IsAcceptable(atom, atomContainer, type))
                {
                    return type;
                }
            }
            return null;
        }

        private IAtomType PerceiveZinc(IAtomContainer atomContainer, IAtom atom)
        {
            if (HasOneSingleElectron(atomContainer, atom))
            {
                // no idea how to deal with this yet
                return null;
            }
            else if (atomContainer.GetConnectedBonds(atom).Count() == 0
                  && (atom.FormalCharge != null && atom.FormalCharge == 0))
            {
                IAtomType type = GetAtomType("Zn.metallic");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            else if (atomContainer.GetConnectedBonds(atom).Count() == 0
                  && (atom.FormalCharge != null && atom.FormalCharge == 2))
            {
                IAtomType type = GetAtomType("Zn.2plus");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            else if (atomContainer.GetConnectedBonds(atom).Count() == 1
                  && (atom.FormalCharge != null && atom.FormalCharge == 0))
            {
                IAtomType type = GetAtomType("Zn.1");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            else if (atomContainer.GetConnectedBonds(atom).Count() == 2
                  && (atom.FormalCharge != null && atom.FormalCharge == 0))
            {
                IAtomType type = GetAtomType("Zn");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            return null;
        }

        private IAtomType PerceiveChromium(IAtomContainer atomContainer, IAtom atom)
        {
            if (atom.FormalCharge != null && atom.FormalCharge == 0
                    && atomContainer.GetConnectedBonds(atom).Count() == 6)
            {
                IAtomType type = GetAtomType("Cr");
                if (IsAcceptable(atom, atomContainer, type))
                {
                    return type;
                }
            }
            else if (atom.FormalCharge != null && atom.FormalCharge == 0
                  && atomContainer.GetConnectedBonds(atom).Count() == 4)
            {
                IAtomType type = GetAtomType("Cr.4");
                if (IsAcceptable(atom, atomContainer, type))
                {
                    return type;
                }
            }
            else if (atom.FormalCharge != null && atom.FormalCharge == 6
                  && atomContainer.GetConnectedBonds(atom).Count() == 0)
            {
                IAtomType type = GetAtomType("Cr.6plus");
                if (IsAcceptable(atom, atomContainer, type))
                {
                    return type;
                }
            }
            else if (atom.FormalCharge != null && atom.FormalCharge == 0
                  && atomContainer.GetConnectedBonds(atom).Count() == 0)
            {
                IAtomType type = GetAtomType("Cr.neutral");
                if (IsAcceptable(atom, atomContainer, type))
                {
                    return type;
                }
            }
            else if ("Cr".Equals(atom.Symbol))
            {
                if (atom.FormalCharge != null && atom.FormalCharge == 3
                        && atomContainer.GetConnectedBonds(atom).Count() == 0)
                {
                    IAtomType type = GetAtomType("Cr.3plus");
                    if (IsAcceptable(atom, atomContainer, type))
                    {
                        return type;
                    }
                }
            }
            return null;
        }

        private IAtomType PerceiveOrganometallicCenters(IAtomContainer atomContainer, IAtom atom)
        {
            if ("Po".Equals(atom.Symbol))
            {
                if (HasOneSingleElectron(atomContainer, atom))
                {
                    // no idea how to deal with this yet
                    return null;
                }
                else if (atomContainer.GetConnectedBonds(atom).Count() == 2)
                {
                    IAtomType type = GetAtomType("Po");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
            }
            else if ("Sn".Equals(atom.Symbol))
            {
                if (HasOneSingleElectron(atomContainer, atom))
                {
                    // no idea how to deal with this yet
                    return null;
                }
                else if ((atom.FormalCharge != null && atom.FormalCharge == 0 && atomContainer
                      .GetConnectedBonds(atom).Count() <= 4))
                {
                    IAtomType type = GetAtomType("Sn.sp3");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
            }
            else if ("Sc".Equals(atom.Symbol))
            {
                if (atom.FormalCharge != null && atom.FormalCharge == -3
                        && atomContainer.GetConnectedBonds(atom).Count() == 6)
                {
                    IAtomType type = GetAtomType("Sc.3minus");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
            }
            return null;
        }

        private IAtomType PerceiveNickel(IAtomContainer atomContainer, IAtom atom)
        {
            if (HasOneSingleElectron(atomContainer, atom))
            {
                // no idea how to deal with this yet
                return null;
            }
            else if ((atom.FormalCharge != null && atom.FormalCharge == +2))
            {
                IAtomType type = GetAtomType("Ni.2plus");
                if (IsAcceptable(atom, atomContainer, type))
                {
                    return type;
                }
            }
            else if ((atom.FormalCharge != null && atom.FormalCharge == 0)
                  && atomContainer.GetConnectedBonds(atom).Count() == 2)
            {
                IAtomType type = GetAtomType("Ni");
                if (IsAcceptable(atom, atomContainer, type))
                {
                    return type;
                }
            }
            else if ((atom.FormalCharge != null && atom.FormalCharge == 0)
                  && atomContainer.GetConnectedBonds(atom).Count() == 0)
            {
                IAtomType type = GetAtomType("Ni.metallic");
                if (IsAcceptable(atom, atomContainer, type))
                {
                    return type;
                }
            }
            else if ((atom.FormalCharge != null && atom.FormalCharge == 1)
                  && atomContainer.GetConnectedBonds(atom).Count() == 1)
            {
                IAtomType type = GetAtomType("Ni.plus");
                if (IsAcceptable(atom, atomContainer, type))
                {
                    return type;
                }
            }
            return null;
        }

        private IAtomType PerceiveNobelGases(IAtomContainer atomContainer, IAtom atom)
        {
            if ("He".Equals(atom.Symbol))
            {
                if (HasOneSingleElectron(atomContainer, atom))
                {
                    // no idea how to deal with this yet
                    return null;
                }
                else if ((atom.FormalCharge == null || atom.FormalCharge == 0))
                {
                    IAtomType type = GetAtomType("He");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
            }
            else if ("Ne".Equals(atom.Symbol))
            {
                if (HasOneSingleElectron(atomContainer, atom))
                {
                    // no idea how to deal with this yet
                    return null;
                }
                else if ((atom.FormalCharge == null || atom.FormalCharge == 0))
                {
                    IAtomType type = GetAtomType("Ne");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
            }
            else if ("Ar".Equals(atom.Symbol))
            {
                if (HasOneSingleElectron(atomContainer, atom))
                {
                    // no idea how to deal with this yet
                    return null;
                }
                else if ((atom.FormalCharge == null || atom.FormalCharge == 0))
                {
                    IAtomType type = GetAtomType("Ar");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
            }
            else if ("Kr".Equals(atom.Symbol))
            {
                if (HasOneSingleElectron(atomContainer, atom))
                {
                    // no idea how to deal with this yet
                    return null;
                }
                else if ((atom.FormalCharge == null || atom.FormalCharge == 0))
                {
                    IAtomType type = GetAtomType("Kr");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
            }
            else if ("Xe".Equals(atom.Symbol))
            {
                if (HasOneSingleElectron(atomContainer, atom))
                {
                    // no idea how to deal with this yet
                    return null;
                }
                else if ((atom.FormalCharge == null || atom.FormalCharge == 0))
                {
                    if (atomContainer.GetConnectedBonds(atom).Count() == 0)
                    {
                        IAtomType type = GetAtomType("Xe");
                        if (IsAcceptable(atom, atomContainer, type)) return type;
                    }
                    else
                    {
                        IAtomType type = GetAtomType("Xe.3");
                        if (IsAcceptable(atom, atomContainer, type)) return type;
                    }
                }
            }
            else if ("Rn".Equals(atom.Symbol))
            {
                if (HasOneSingleElectron(atomContainer, atom))
                {
                    // no idea how to deal with this yet
                    return null;
                }
                else if ((atom.FormalCharge == null || atom.FormalCharge == 0))
                {
                    IAtomType type = GetAtomType("Rn");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
            }
            return null;
        }

        private IAtomType PerceiveSilicon(IAtomContainer atomContainer, IAtom atom)
        {
            if (HasOneSingleElectron(atomContainer, atom))
            {
                // no idea how to deal with this yet
                return null;
            }
            else if (atom.FormalCharge != null && atom.FormalCharge == 0)
            {
                if (atomContainer.GetConnectedBonds(atom).Count() == 2)
                {
                    IAtomType type = GetAtomType("Si.2");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
                else if (atomContainer.GetConnectedBonds(atom).Count() == 3)
                {
                    IAtomType type = GetAtomType("Si.3");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
                else if (atomContainer.GetConnectedBonds(atom).Count() == 4)
                {
                    IAtomType type = GetAtomType("Si.sp3");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
            }
            else if (atom.FormalCharge != null && atom.FormalCharge == -2)
            {
                IAtomType type = GetAtomType("Si.2minus.6");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            return null;
        }

        private IAtomType PerceiveManganese(IAtomContainer atomContainer, IAtom atom)
        {
            if (HasOneSingleElectron(atomContainer, atom))
            {
                // no idea how to deal with this yet
                return null;
            }
            else if ((atom.FormalCharge != null && atom.FormalCharge == 0))
            {
                int neighbors = atomContainer.GetConnectedBonds(atom).Count();
                if (neighbors == 2)
                {
                    IAtomType type02 = GetAtomType("Mn.2");
                    if (IsAcceptable(atom, atomContainer, type02)) return type02;
                }
                else if (neighbors == 0)
                {
                    IAtomType type03 = GetAtomType("Mn.metallic");
                    if (IsAcceptable(atom, atomContainer, type03)) return type03;
                }
            }
            else if ((atom.FormalCharge != null && atom.FormalCharge == +2))
            {
                IAtomType type = GetAtomType("Mn.2plus");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            else if ((atom.FormalCharge != null && atom.FormalCharge == +3))
            {
                IAtomType type = GetAtomType("Mn.3plus");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            return null;
        }

        private IAtomType PerceiveSodium(IAtomContainer atomContainer, IAtom atom)
        {
            if (HasOneSingleElectron(atomContainer, atom))
            {
                // no idea how to deal with this yet
                return null;
            }
            else if ((atom.FormalCharge != null && atom.FormalCharge == 1))
            {
                IAtomType type = GetAtomType("Na.plus");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            else if ((atom.FormalCharge == null || atom.FormalCharge == 0)
                  && atomContainer.GetConnectedBonds(atom).Count() == 1)
            {
                IAtomType type = GetAtomType("Na");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            else if ((atom.FormalCharge != null && atom.FormalCharge == 0)
                  && atomContainer.GetConnectedBonds(atom).Count() == 0)
            {
                IAtomType type = GetAtomType("Na.neutral");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            return null;
        }

        private IAtomType PerceiveIodine(IAtomContainer atomContainer, IAtom atom, IList<IBond> connectedBonds)
        {
            if (connectedBonds == null) connectedBonds = atomContainer.GetConnectedBonds(atom).ToList();
            if (HasOneSingleElectron(atomContainer, atom))
            {
                if (connectedBonds.Count == 0)
                {
                    if (atom.FormalCharge != null && atom.FormalCharge == +1)
                    {
                        IAtomType type = GetAtomType("I.plus.radical");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                    else if (atom.FormalCharge == null || atom.FormalCharge == 0)
                    {
                        IAtomType type = GetAtomType("I.radical");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                }
                else if (connectedBonds.Count <= 1)
                {
                    BondOrder maxBondOrder = GetMaximumBondOrder(connectedBonds);
                    if (maxBondOrder == BondOrder.Single)
                    {
                        IAtomType type = GetAtomType("I.plus.radical");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                }
                return null;
            }
            else if (atom.FormalCharge != null && atom.FormalCharge != 0)
            {
                if (atom.FormalCharge == -1)
                {
                    if (connectedBonds.Count == 0)
                    {
                        IAtomType type = GetAtomType("I.minus");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                    else
                    {
                        IAtomType type = GetAtomType("I.minus.5");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                }
                else if (atom.FormalCharge == 1)
                {
                    BondOrder maxBondOrder = GetMaximumBondOrder(connectedBonds);
                    if (maxBondOrder == BondOrder.Double)
                    {
                        IAtomType type = GetAtomType("I.plus.sp2");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                    else if (maxBondOrder == BondOrder.Single)
                    {
                        IAtomType type = GetAtomType("I.plus.sp3");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                }
            }
            else if (connectedBonds.Count == 3)
            {
                int doubleBondCount = CountAttachedDoubleBonds(connectedBonds, atom);
                if (doubleBondCount == 2)
                {
                    IAtomType type = GetAtomType("I.5");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                else if (atom.FormalCharge != null && atom.FormalCharge == 0)
                {
                    IAtomType type = GetAtomType("I.sp3d2.3");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
            }
            else if (connectedBonds.Count == 2)
            {
                BondOrder maxBondOrder = GetMaximumBondOrder(connectedBonds);
                if (maxBondOrder == BondOrder.Double)
                {
                    IAtomType type = GetAtomType("I.3");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
            }
            else if (connectedBonds.Count <= 1)
            {
                IAtomType type = GetAtomType("I");
                if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
            }
            return null;
        }

        private IAtomType PerceiveRuthenium(IAtomContainer atomContainer, IAtom atom)
        {
            if (atom.FormalCharge != null && atom.FormalCharge == 0)
            {
                IAtomType type = GetAtomType("Ru.6");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            else if (atom.FormalCharge != null && atom.FormalCharge == -2)
            {
                IAtomType type = GetAtomType("Ru.2minus.6");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            else if (atom.FormalCharge != null && atom.FormalCharge == -3)
            {
                IAtomType type = GetAtomType("Ru.3minus.6");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            return null;
        }

        private IAtomType PerceivePotassium(IAtomContainer atomContainer, IAtom atom)
        {
            if (HasOneSingleElectron(atomContainer, atom))
            {
                // no idea how to deal with this yet
                return null;
            }
            else if ((atom.FormalCharge != null && atom.FormalCharge == +1))
            {
                IAtomType type = GetAtomType("K.plus");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            else if (atom.FormalCharge == null || atom.FormalCharge == 0)
            {
                int neighbors = atomContainer.GetConnectedBonds(atom).Count();
                if (neighbors == 1)
                {
                    IAtomType type = GetAtomType("K.neutral");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
                IAtomType typee = GetAtomType("K.metallic");
                if (IsAcceptable(atom, atomContainer, typee)) return typee;
            }
            return null;
        }

        private IAtomType PerceivePlutonium(IAtomContainer atomContainer, IAtom atom)
        {
            if (atom.FormalCharge == 0 && atomContainer.GetConnectedBonds(atom).Count() == 0)
            {
                IAtomType type = GetAtomType("Pu");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            return null;
        }

        private IAtomType PerceiveCadmium(IAtomContainer atomContainer, IAtom atom)
        {
            if (HasOneSingleElectron(atomContainer, atom))
            {
                // no idea how to deal with this yet
                return null;
            }
            else if ((atom.FormalCharge != null && atom.FormalCharge == +2))
            {
                IAtomType type = GetAtomType("Cd.2plus");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            else if ((atom.FormalCharge != null && atom.FormalCharge == 0))
            {
                if (atomContainer.GetConnectedBonds(atom).Count() == 0)
                {
                    IAtomType type = GetAtomType("Cd.metallic");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
                else if (atomContainer.GetConnectedBonds(atom).Count() == 2)
                {
                    IAtomType type = GetAtomType("Cd.2");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
            }
            return null;
        }

        private IAtomType PerceiveIndium(IAtomContainer atomContainer, IAtom atom)
        {
            if (atom.FormalCharge == 0 && atomContainer.GetConnectedBonds(atom).Count() == 3)
            {
                IAtomType type = GetAtomType("In.3");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            else if (atom.FormalCharge == 3 && atomContainer.GetConnectedBonds(atom).Count() == 0)
            {
                IAtomType type = GetAtomType("In.3plus");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            else if (atom.FormalCharge == 0 && atomContainer.GetConnectedBonds(atom).Count() == 1)
            {
                IAtomType type = GetAtomType("In.1");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            else
            {
                IAtomType type = GetAtomType("In");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            return null;
        }

        private IAtomType PerceiveChlorine(IAtomContainer atomContainer, IAtom atom, IList<IBond> connectedBonds)
        {
            if (connectedBonds == null) connectedBonds = atomContainer.GetConnectedBonds(atom).ToList();
            if (HasOneSingleElectron(atomContainer, atom))
            {
                if (connectedBonds.Count > 1)
                {
                    if (atom.FormalCharge != null && atom.FormalCharge == +1)
                    {
                        IAtomType type = GetAtomType("Cl.plus.radical");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                }
                else if (connectedBonds.Count == 1)
                {
                    BondOrder maxBondOrder = atomContainer.GetMaximumBondOrder(atom);
                    if (maxBondOrder == BondOrder.Single)
                    {
                        IAtomType type = GetAtomType("Cl.plus.radical");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                }
                else if (connectedBonds.Count == 0
                      && (atom.FormalCharge == null || atom.FormalCharge == 0))
                {
                    IAtomType type = GetAtomType("Cl.radical");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
            }
            else if (atom.FormalCharge == null || atom.FormalCharge == 0)
            {
                int neighborcount = connectedBonds.Count;
                BondOrder maxBondOrder = GetMaximumBondOrder(connectedBonds);

                if (maxBondOrder == BondOrder.Double)
                {
                    if (neighborcount == 2)
                    {
                        IAtomType type = GetAtomType("Cl.2");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                    else if (neighborcount == 3)
                    {
                        IAtomType type = GetAtomType("Cl.chlorate");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                    else if (neighborcount == 4)
                    {
                        IAtomType type = GetAtomType("Cl.perchlorate");
                        if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                    }
                }
                else if (neighborcount <= 1)
                {
                    IAtomType type = GetAtomType("Cl");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
            }
            else if ((atom.FormalCharge != null && atom.FormalCharge == -1))
            {
                IAtomType type = GetAtomType("Cl.minus");
                if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
            }
            else if (atom.FormalCharge != null && atom.FormalCharge == 1)
            {
                BondOrder maxBondOrder = GetMaximumBondOrder(connectedBonds);
                if (maxBondOrder == BondOrder.Double)
                {
                    IAtomType type = GetAtomType("Cl.plus.sp2");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                else if (maxBondOrder == BondOrder.Single)
                {
                    IAtomType type = GetAtomType("Cl.plus.sp3");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
            }
            else if ((atom.FormalCharge != null && atom.FormalCharge == +3)
                  && connectedBonds.Count == 4)
            {
                IAtomType type = GetAtomType("Cl.perchlorate.charged");
                if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
            }
            else
            {
                int doubleBonds = CountAttachedDoubleBonds(connectedBonds, atom);
                if (connectedBonds.Count == 3 && doubleBonds == 2)
                {
                    IAtomType type = GetAtomType("Cl.chlorate");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
                else if (connectedBonds.Count == 4 && doubleBonds == 3)
                {
                    IAtomType type = GetAtomType("Cl.perchlorate");
                    if (IsAcceptable(atom, atomContainer, type, connectedBonds)) return type;
                }
            }
            return null;
        }

        private IAtomType PerceiveSilver(IAtomContainer atomContainer, IAtom atom)
        {
            if (HasOneSingleElectron(atomContainer, atom))
            {
                return null;
            }
            else if ((atom.FormalCharge != null && atom.FormalCharge == 0))
            {
                int neighbors = atomContainer.GetConnectedBonds(atom).Count();
                if (neighbors == 1)
                {
                    IAtomType type = GetAtomType("Ag.1");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
                IAtomType typee = GetAtomType("Ag.neutral");
                if (IsAcceptable(atom, atomContainer, typee)) return typee;
            }
            else if ((atom.FormalCharge != null && atom.FormalCharge == 1))
            {
                IAtomType type = GetAtomType("Ag.plus");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            return null;
        }

        private IAtomType PerceiveGold(IAtomContainer atomContainer, IAtom atom)
        {
            if (HasOneSingleElectron(atomContainer, atom))
            {
                return null;
            }
            int neighbors = atomContainer.GetConnectedBonds(atom).Count();
            if ((atom.FormalCharge != null && atom.FormalCharge == 0) && neighbors == 1)
            {
                IAtomType type = GetAtomType("Au.1");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            return null;
        }

        private IAtomType PerceiveRadium(IAtomContainer atomContainer, IAtom atom)
        {
            if (HasOneSingleElectron(atomContainer, atom))
            {
                return null;
            }
            else if ((atom.FormalCharge != null && atom.FormalCharge == 0))
            {
                IAtomType type = GetAtomType("Ra.neutral");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            return null;
        }

        private IAtomType PerceiveCalcium(IAtomContainer atomContainer, IAtom atom)
        {
            if ("Ca".Equals(atom.Symbol))
            {
                if (HasOneSingleElectron(atomContainer, atom))
                {
                    // no idea how to deal with this yet
                    return null;
                }
                else if ((atom.FormalCharge != null && atom.FormalCharge == 2 && atomContainer
                      .GetConnectedBonds(atom).Count() == 0))
                {
                    IAtomType type = GetAtomType("Ca.2plus");
                    if (IsAcceptable(atom, atomContainer, type))
                    {
                        return type;
                    }
                }
                else if ((atom.FormalCharge != null && atom.FormalCharge == 0 && atomContainer
                      .GetConnectedBonds(atom).Count() == 2))
                {
                    IAtomType type = GetAtomType("Ca.2");
                    if (IsAcceptable(atom, atomContainer, type))
                    {
                        return type;
                    }
                }
                else if ((atom.FormalCharge != null && atom.FormalCharge == 0 && atomContainer
                      .GetConnectedBonds(atom).Count() == 1))
                {
                    IAtomType type = GetAtomType("Ca.1");
                    if (IsAcceptable(atom, atomContainer, type))
                    {
                        return type;
                    }
                }
            }
            return null;
        }

        private IAtomType PerceivePlatinum(IAtomContainer atomContainer, IAtom atom)
        {
            if (HasOneSingleElectron(atomContainer, atom))
            {
                // no idea how to deal with this yet
                return null;
            }
            else if ((atom.FormalCharge != null && atom.FormalCharge == +2))
            {
                int neighbors = atomContainer.GetConnectedBonds(atom).Count();
                if (neighbors == 4)
                {
                    IAtomType type = GetAtomType("Pt.2plus.4");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
                else
                {
                    IAtomType type = GetAtomType("Pt.2plus");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
            }
            else if ((atom.FormalCharge == null || atom.FormalCharge == 0))
            {
                int neighbors = atomContainer.GetConnectedBonds(atom).Count();
                if (neighbors == 2)
                {
                    IAtomType type = GetAtomType("Pt.2");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
                else if (neighbors == 4)
                {
                    IAtomType type = GetAtomType("Pt.4");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
                else if (neighbors == 6)
                {
                    IAtomType type = GetAtomType("Pt.6");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
            }
            return null;
        }

        private IAtomType PerceiveAntimony(IAtomContainer atomContainer, IAtom atom)
        {
            if (HasOneSingleElectron(atomContainer, atom))
            {
                return null;
            }
            else if ((atom.FormalCharge != null && atom.FormalCharge == 0 && atomContainer
                  .GetConnectedBonds(atom).Count() == 3))
            {
                IAtomType type = GetAtomType("Sb.3");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            else if ((atom.FormalCharge != null && atom.FormalCharge == 0 && atomContainer
                  .GetConnectedBonds(atom).Count() == 4))
            {
                IAtomType type = GetAtomType("Sb.4");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            return null;
        }

        private IAtomType PerceiveGadolinum(IAtomContainer atomContainer, IAtom atom)
        {
            if (atom.FormalCharge != null && atom.FormalCharge == +3
                    && atomContainer.GetConnectedBonds(atom).Count() == 0)
            {
                IAtomType type = GetAtomType("Gd.3plus");
                if (IsAcceptable(atom, atomContainer, type))
                {
                    return type;
                }
            }
            return null;
        }

        private IAtomType PerceiveMagnesium(IAtomContainer atomContainer, IAtom atom)
        {
            if (HasOneSingleElectron(atomContainer, atom))
            {
                // no idea how to deal with this yet
                return null;
            }
            else if ((atom.FormalCharge != null && atom.FormalCharge == 0))
            {
                int neighbors = atomContainer.GetConnectedBonds(atom).Count();
                if (neighbors == 4)
                {
                    IAtomType type = GetAtomType("Mg.neutral");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
                else if (neighbors == 2)
                {
                    IAtomType type = GetAtomType("Mg.neutral.2");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
                else if (neighbors == 1)
                {
                    IAtomType type = GetAtomType("Mg.neutral.1");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
                else
                {
                    IAtomType type = GetAtomType("Mg.neutral");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
            }
            else if ((atom.FormalCharge != null && atom.FormalCharge == +2))
            {
                IAtomType type = GetAtomType("Mg.2plus");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            return null;
        }

        private IAtomType PerceiveThallium(IAtomContainer atomContainer, IAtom atom)
        {
            if (atom.FormalCharge != null && atom.FormalCharge == +1
                    && atomContainer.GetConnectedBonds(atom).Count() == 0)
            {
                IAtomType type = GetAtomType("Tl.plus");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            else if (atom.FormalCharge != null && atom.FormalCharge == 0
                  && atomContainer.GetConnectedBonds(atom).Count() == 0)
            {
                IAtomType type = GetAtomType("Tl");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            else if (atom.FormalCharge != null && atom.FormalCharge == 0
                  && atomContainer.GetConnectedBonds(atom).Count() == 1)
            {
                IAtomType type = GetAtomType("Tl.1");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            return null;
        }

        private IAtomType PerceiveLead(IAtomContainer atomContainer, IAtom atom)
        {
            if (atom.FormalCharge != null && atom.FormalCharge == 0
                    && atomContainer.GetConnectedBonds(atom).Count() == 0)
            {
                IAtomType type = GetAtomType("Pb.neutral");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            else if (atom.FormalCharge != null && atom.FormalCharge == 2
                  && atomContainer.GetConnectedBonds(atom).Count() == 0)
            {
                IAtomType type = GetAtomType("Pb.2plus");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            else if (atom.FormalCharge != null && atom.FormalCharge == 0
                  && atomContainer.GetConnectedBonds(atom).Count() == 1)
            {
                IAtomType type = GetAtomType("Pb.1");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            return null;
        }

        private IAtomType PerceiveStrontium(IAtomContainer atomContainer, IAtom atom)
        {
            if (HasOneSingleElectron(atomContainer, atom))
            {
                return null;
            }
            else if ((atom.FormalCharge != null && atom.FormalCharge == 2))
            {
                IAtomType type = GetAtomType("Sr.2plus");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            return null;
        }

        private IAtomType PerceiveTitanium(IAtomContainer atomContainer, IAtom atom)
        {
            if (atom.FormalCharge != null && atom.FormalCharge == -3
                    && atomContainer.GetConnectedBonds(atom).Count() == 6)
            {
                IAtomType type = GetAtomType("Ti.3minus");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            else if ((atom.FormalCharge == null || atom.FormalCharge == 0)
                  && atomContainer.GetConnectedBonds(atom).Count() == 4)
            {
                IAtomType type = GetAtomType("Ti.sp3");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            else if ((atom.FormalCharge != null && atom.FormalCharge == 0)
                  && atomContainer.GetConnectedBonds(atom).Count() == 2)
            {
                IAtomType type = GetAtomType("Ti.2");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            return null;
        }

        private IAtomType PerceiveVanadium(IAtomContainer atomContainer, IAtom atom)
        {
            if (atom.FormalCharge != null && atom.FormalCharge == -3
                    && atomContainer.GetConnectedBonds(atom).Count() == 6)
            {
                IAtomType type = GetAtomType("V.3minus");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            else if (atom.FormalCharge != null && atom.FormalCharge == -3
                  && atomContainer.GetConnectedBonds(atom).Count() == 4)
            {
                IAtomType type = GetAtomType("V.3minus.4");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            return null;
        }

        private IAtomType PerceiveBromine(IAtomContainer atomContainer, IAtom atom)
        {
            if (HasOneSingleElectron(atomContainer, atom))
            {
                if (atomContainer.GetConnectedBonds(atom).Count() == 0)
                {
                    if (atom.FormalCharge != null && atom.FormalCharge == +1)
                    {
                        IAtomType type = GetAtomType("Br.plus.radical");
                        if (IsAcceptable(atom, atomContainer, type)) return type;
                    }
                    else if (atom.FormalCharge == null || atom.FormalCharge == 0)
                    {
                        IAtomType type = GetAtomType("Br.radical");
                        if (IsAcceptable(atom, atomContainer, type)) return type;
                    }
                }
                else if (atomContainer.GetConnectedBonds(atom).Count() <= 1)
                {
                    BondOrder maxBondOrder = atomContainer.GetMaximumBondOrder(atom);
                    if (maxBondOrder == BondOrder.Single)
                    {
                        IAtomType type = GetAtomType("Br.plus.radical");
                        if (IsAcceptable(atom, atomContainer, type)) return type;
                    }
                }
                return null;
            }
            else if ((atom.FormalCharge != null && atom.FormalCharge == -1))
            {
                IAtomType type = GetAtomType("Br.minus");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            else if (atom.FormalCharge == 1)
            {
                BondOrder maxBondOrder = atomContainer.GetMaximumBondOrder(atom);
                if (maxBondOrder == BondOrder.Double)
                {
                    IAtomType type = GetAtomType("Br.plus.sp2");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
                else if (maxBondOrder == BondOrder.Single)
                {
                    IAtomType type = GetAtomType("Br.plus.sp3");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
            }
            else if (atomContainer.GetConnectedBonds(atom).Count() == 1 || atomContainer.GetConnectedBonds(atom).Count() == 0)
            {
                IAtomType type = GetAtomType("Br");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            else if (atomContainer.GetConnectedBonds(atom).Count() == 3)
            {
                IAtomType type = GetAtomType("Br.3");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            return null;
        }

        private int CountAttachedDoubleBonds(IList<IBond> connectedAtoms, IAtom atom, string symbol)
        {
            return CountAttachedBonds(connectedAtoms, atom, BondOrder.Double, symbol);
        }

        private IAtomType PerceiveCobalt(IAtomContainer atomContainer, IAtom atom)
        {
            if (HasOneSingleElectron(atomContainer, atom))
            {
                // no idea how to deal with this yet
                return null;
            }
            else if ((atom.FormalCharge != null && atom.FormalCharge == +2))
            {
                IAtomType type = GetAtomType("Co.2plus");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            else if ((atom.FormalCharge != null && atom.FormalCharge == +3))
            {
                IAtomType type = GetAtomType("Co.3plus");
                if (IsAcceptable(atom, atomContainer, type)) return type;
            }
            else if ((atom.FormalCharge == null || atom.FormalCharge == 0))
            {
                int neighbors = atomContainer.GetConnectedBonds(atom).Count();
                if (neighbors == 2)
                {
                    IAtomType type = GetAtomType("Co.2");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
                else if (neighbors == 4)
                {
                    IAtomType type = GetAtomType("Co.4");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
                else if (neighbors == 6)
                {
                    IAtomType type = GetAtomType("Co.6");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
                else if (neighbors == 1)
                {
                    IAtomType type = GetAtomType("Co.1");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
                else
                {
                    IAtomType type = GetAtomType("Co.metallic");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
            }
            else if ((atom.FormalCharge != null && atom.FormalCharge == +1))
            {
                int neighbors = atomContainer.GetConnectedBonds(atom).Count();
                if (neighbors == 2)
                {
                    IAtomType type = GetAtomType("Co.plus.2");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
                else if (neighbors == 4)
                {
                    IAtomType type = GetAtomType("Co.plus.4");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
                else if (neighbors == 1)
                {
                    IAtomType type = GetAtomType("Co.plus.1");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
                else if (neighbors == 6)
                {
                    IAtomType type = GetAtomType("Co.plus.6");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
                else if (neighbors == 5)
                {
                    IAtomType type = GetAtomType("Co.plus.5");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
                else
                {
                    IAtomType type = GetAtomType("Co.plus");
                    if (IsAcceptable(atom, atomContainer, type)) return type;
                }
            }
            return null;
        }

        private int CountAttachedDoubleBonds(IList<IBond> connectedBonds, IAtom atom)
        {
            return CountAttachedBonds(connectedBonds, atom, BondOrder.Double, null);
        }

        private int CountAttachedSingleBonds(IList<IBond> connectedBonds, IAtom atom)
        {
            return CountAttachedBonds(connectedBonds, atom, BondOrder.Single, null);
        }

        private bool HasAromaticBond(IList<IBond> connectedBonds)
        {
            foreach (var bond in connectedBonds)
            {
                if (bond.IsAromatic) return true;
            }
            return false;
        }

        /// <summary>
        /// Count the number of doubly bonded atoms.
        /// </summary>
        /// <param name="connectedBonds"></param>
        /// <param name="atom">the atom being looked at</param>
        /// <param name="order">the desired bond order of the attached bonds</param>
        /// <param name="symbol">If not null, then it only counts the double bonded atoms which match the given symbol.</param>
        /// <returns>the number of doubly bonded atoms</returns>
        private int CountAttachedBonds(IList<IBond> connectedBonds, IAtom atom, BondOrder order, string symbol)
        {
            // count the number of double bonded oxygens
            int neighborcount = connectedBonds.Count;
            int doubleBondedAtoms = 0;
            for (int i = neighborcount - 1; i >= 0; i--)
            {
                IBond bond = connectedBonds[i];
                if (bond.Order == order)
                {
                    if (bond.Atoms.Count == 2)
                    {
                        if (symbol != null)
                        {
                            // if other atom is of the given element (by its symbol)
                            if (bond.GetOther(atom).Symbol.Equals(symbol))
                            {
                                doubleBondedAtoms++;
                            }
                        }
                        else
                        {
                            doubleBondedAtoms++;
                        }
                    }
                }
            }
            return doubleBondedAtoms;
        }

        private IAtomType GetAtomType(string identifier)
        {
            IAtomType type = factory.GetAtomType(identifier);
            return type;
        }

        private bool IsAcceptable(IAtom atom, IAtomContainer container, IAtomType type)
        {
            return IsAcceptable(atom, container, type, null);
        }

        private bool IsAcceptable(IAtom atom, IAtomContainer container, IAtomType type, IList<IBond> connectedBonds)
        {
            if (connectedBonds == null) connectedBonds = container.GetConnectedBonds(atom).ToList();
            if (mode == RequireExplicitHydrogens)
            {
                // make sure no implicit hydrogens were assumed
                int actualContainerCount = connectedBonds.Count;
                int requiredContainerCount = type.FormalNeighbourCount.Value; // TODO: this can throw exception? 
                if (actualContainerCount != requiredContainerCount) return false;
            }
            else if (atom.ImplicitHydrogenCount.HasValue)
            {
                // confirm correct neighbour count
                int connectedAtoms = connectedBonds.Count;
                int hCount = atom.ImplicitHydrogenCount.Value;
                int actualNeighbourCount = connectedAtoms + hCount;
                int requiredNeighbourCount = type.FormalNeighbourCount.Value; // TODO: this can throw exception? 
                if (actualNeighbourCount > requiredNeighbourCount) return false;
            }

            // confirm correct bond orders
            BondOrder typeOrder = type.MaxBondOrder;
            if (!typeOrder.IsUnset)
            {
                foreach (var bond in connectedBonds)
                {
                    BondOrder order = bond.Order;
                    if (!order.IsUnset)
                    {
                        if (BondManipulator.IsHigherOrder(order, typeOrder)) return false;
                    }
                    else if (bond.IsSingleOrDouble)
                    {
                        if (typeOrder != BondOrder.Single && typeOrder != BondOrder.Double) return false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            // confirm correct valency
            if (type.Valency != null)
            {
                double valence = container.GetBondOrderSum(atom);
                valence += atom.ImplicitHydrogenCount ?? 0;
                if (valence > type.Valency)
                    return false;
            }

            // confirm correct formal charge
            if (atom.FormalCharge != null && !atom.FormalCharge.Equals(type.FormalCharge))
                return false;

            // confirm single electron count
            if (type.GetProperty<int?>(CDKPropertyName.SingleElectronCount) != null)
            {
                int count = CountSingleElectrons(container, atom);
                if (count != type.GetProperty<int>(CDKPropertyName.SingleElectronCount))
                    return false;
            }

            return true;
        }
    }
}
