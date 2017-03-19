/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *  All we ask is that proper credit is given for our work, which includes
 *  - but is not limited to - adding the above copyright notice to the beginning
 *  of your source code files, and to any copyright notice that you may distribute
 *  with programs based on this work.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT Any WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 */
using NCDK.Config;
using NCDK.Tools.Manipulator;
using System;
using System.Diagnostics;
using System.Linq;

namespace NCDK.Tools
{
    /// <summary>
    /// Small customization of ValencyHybridChecker suggested by Todd Martin
    /// specially tuned for SMILES parsing.
    /// </summary>
    // @author       Egon Willighagen
    // @cdk.created  2004-06-12
    // @cdk.keyword  atom, valency
    // @cdk.module   valencycheck
    // @cdk.githash
    public class SmilesValencyChecker : IValencyChecker, IDeduceBondOrderTool
    {
        private string atomTypeList = null;
        protected AtomTypeFactory structgenATF;

        public SmilesValencyChecker()
            : this("NCDK.Dict.Data.cdk-atom-types.owl")
        { }

        public SmilesValencyChecker(string atomTypeList)
        {
            this.atomTypeList = atomTypeList;
            Trace.TraceInformation("Using configuration file: ", atomTypeList);
        }

        /// <summary>
        /// Saturates a molecule by setting appropriate bond orders.
        /// </summary>
        // @cdk.keyword            bond order, calculation
        // @cdk.created 2003-10-03
        public void Saturate(IAtomContainer atomContainer)
        {
            Trace.TraceInformation("Saturating atomContainer by adjusting bond orders...");
            bool allSaturated = AllSaturated(atomContainer);
            if (!allSaturated)
            {
                Trace.TraceInformation("Saturating bond orders is needed...");
                IBond[] bonds = new IBond[atomContainer.Bonds.Count];
                for (int i = 0; i < bonds.Length; i++)
                    bonds[i] = atomContainer.Bonds[i];
                bool succeeded = Saturate(bonds, atomContainer);
                if (!succeeded)
                {
                    throw new CDKException("Could not saturate this atomContainer!");
                }
            }
        }

        /// <summary>
        /// Saturates a set of Bonds in an AtomContainer.
        /// </summary>
        public bool Saturate(IBond[] bonds, IAtomContainer atomContainer)
        {
            Debug.WriteLine("Saturating bond set of size: ", bonds.Length);
            bool bondsAreFullySaturated = false;
            if (bonds.Length > 0)
            {
                IBond bond = bonds[0];

                // determine bonds left
                int leftBondCount = bonds.Length - 1;
                IBond[] leftBonds = new IBond[leftBondCount];
                Array.Copy(bonds, 1, leftBonds, 0, leftBondCount);

                // examine this bond
                Debug.WriteLine("Examining this bond: ", bond);
                if (IsSaturated(bond, atomContainer))
                {
                    Debug.WriteLine("OK, bond is saturated, now try to saturate remaining bonds (if needed)");
                    bondsAreFullySaturated = Saturate(leftBonds, atomContainer);
                }
                else if (IsUnsaturated(bond, atomContainer))
                {
                    Debug.WriteLine("Ok, this bond is unsaturated, and can be saturated");
                    // two options now:
                    // 1. saturate this one directly
                    // 2. saturate this one by saturating the rest
                    Debug.WriteLine("Option 1: Saturating this bond directly, then trying to saturate rest");
                    // considering organic bonds, the max order is 3, so increase twice
                    bool bondOrderIncreased = SaturateByIncreasingBondOrder(bond, atomContainer);
                    bondsAreFullySaturated = bondOrderIncreased && Saturate(bonds, atomContainer);
                    if (bondsAreFullySaturated)
                    {
                        Debug.WriteLine("Option 1: worked");
                    }
                    else
                    {
                        Debug.WriteLine("Option 1: failed. Trying option 2.");
                        Debug.WriteLine("Option 2: Saturing this bond by saturating the rest");
                        // revert the increase (if succeeded), then saturate the rest
                        if (bondOrderIncreased) UnsaturateByDecreasingBondOrder(bond);
                        bondsAreFullySaturated = Saturate(leftBonds, atomContainer) && IsSaturated(bond, atomContainer);
                        if (!bondsAreFullySaturated) Debug.WriteLine("Option 2: failed");
                    }
                }
                else
                {
                    Debug.WriteLine("Ok, this bond is unsaturated, but cannot be saturated");
                    // try recursing and see if that fixes things
                    bondsAreFullySaturated = Saturate(leftBonds, atomContainer) && IsSaturated(bond, atomContainer);
                }
            }
            else
            {
                bondsAreFullySaturated = true; // empty is saturated by default
            }
            return bondsAreFullySaturated;
        }

        public bool UnsaturateByDecreasingBondOrder(IBond bond)
        {
            if (bond.Order != BondOrder.Single)
            {
                bond.Order = BondManipulator.DecreaseBondOrder(bond.Order);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns whether a bond is unsaturated. A bond is unsaturated if
        /// <b>all</b> Atoms in the bond are unsaturated.
        /// </summary>
        public bool IsUnsaturated(IBond bond, IAtomContainer atomContainer)
        {
            Debug.WriteLine("isBondUnsaturated?: ", bond);
            IAtom[] atoms = BondManipulator.GetAtomArray(bond);
            bool isUnsaturated = true;
            for (int i = 0; i < atoms.Length && isUnsaturated; i++)
            {
                isUnsaturated = isUnsaturated && !IsSaturated(atoms[i], atomContainer);
            }
            Debug.WriteLine("Bond is unsaturated?: ", isUnsaturated);
            return isUnsaturated;
        }

        /// <summary>
        /// Tries to saturate a bond by increasing its bond orders by 1.0.
        /// </summary>
        /// <returns>true if the bond could be increased</returns>
        public bool SaturateByIncreasingBondOrder(IBond bond, IAtomContainer atomContainer)
        {
            IAtom[] atoms = BondManipulator.GetAtomArray(bond);
            IAtom atom = atoms[0];
            IAtom partner = atoms[1];
            Debug.WriteLine("  saturating bond: ", atom.Symbol, "-", partner.Symbol);
            var atomTypes1 = GetAtomTypeFactory(bond.Builder).GetAtomTypes(atom.Symbol);
            var atomTypes2 = GetAtomTypeFactory(bond.Builder).GetAtomTypes(partner.Symbol);
            foreach (var aType1 in atomTypes1)
            {
                Debug.WriteLine("  condidering atom type: ", aType1);
                if (CouldMatchAtomType(atomContainer, atom, aType1))
                {
                    Debug.WriteLine("  trying atom type: ", aType1);
                    foreach (var aType2 in atomTypes2)
                    {
                        Debug.WriteLine("  condidering partner type: ", aType1);
                        if (CouldMatchAtomType(atomContainer, partner, aType2))
                        {
                            Debug.WriteLine("    with atom type: ", aType2);
                            if (BondManipulator.IsLowerOrder(bond.Order, aType2.MaxBondOrder)
                                    && BondManipulator.IsLowerOrder(bond.Order, aType1.MaxBondOrder))
                            {
                                bond.Order = BondManipulator.IncreaseBondOrder(bond.Order);
                                Debug.WriteLine("Bond order now ", bond.Order);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Returns whether a bond is saturated. A bond is saturated if
        /// <b>both</b> Atoms in the bond are saturated.
        /// </summary>
        public bool IsSaturated(IBond bond, IAtomContainer atomContainer)
        {
            Debug.WriteLine("isBondSaturated?: ", bond);
            IAtom[] atoms = BondManipulator.GetAtomArray(bond);
            bool isSaturated = true;
            for (int i = 0; i < atoms.Length; i++)
            {
                Debug.WriteLine("IsSaturated(Bond, AC): atom I=", i);
                isSaturated = isSaturated && IsSaturated(atoms[i], atomContainer);
            }
            Debug.WriteLine("IsSaturated(Bond, AC): result=", isSaturated);
            return isSaturated;
        }

        /// <summary>
        /// Determines of all atoms on the AtomContainer are saturated.
        /// </summary>
        public bool IsSaturated(IAtomContainer container)
        {
            return AllSaturated(container);
        }

        public bool AllSaturated(IAtomContainer ac)
        {
            Debug.WriteLine("Are all atoms saturated?");
            for (int f = 0; f < ac.Atoms.Count; f++)
            {
                if (!IsSaturated(ac.Atoms[f], ac))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Determines if the atom can be of type AtomType. That is, it sees if this
        /// AtomType only differs in bond orders, or implicit hydrogen count.
        /// </summary>
        public bool CouldMatchAtomType(IAtom atom, double bondOrderSum, BondOrder maxBondOrder, IAtomType type)
        {
            Debug.WriteLine("couldMatchAtomType:   ... matching atom ", atom, " vs ", type);
            int hcount = atom.ImplicitHydrogenCount.Value;
            int charge = atom.FormalCharge.Value;
            if (charge == type.FormalCharge)
            {
                Debug.WriteLine("couldMatchAtomType:     formal charge matches...");
                //            if (atom.Hybridization == type.Hybridization) {
                //                Debug.WriteLine("couldMatchAtomType:     hybridization is OK...");
                if (bondOrderSum + hcount <= type.BondOrderSum)
                {
                    Debug.WriteLine("couldMatchAtomType:     bond order sum is OK...");
                    if (!BondManipulator.IsHigherOrder(maxBondOrder, type.MaxBondOrder))
                    {
                        Debug.WriteLine("couldMatchAtomType:     max bond order is OK... We have a match!");
                        return true;
                    }
                }
                else
                {
                    Debug.WriteLine("couldMatchAtomType:      no match", "" + (bondOrderSum + hcount), " > ",
                            "" + type.BondOrderSum);
                }
                //            }
            }
            else
            {
                Debug.WriteLine("couldMatchAtomType:     formal charge does NOT match...");
            }
            Debug.WriteLine("couldMatchAtomType:    No Match");
            return false;
        }

        /// <summary>
        /// Calculates the number of hydrogens that can be added to the given atom to fullfil
        /// the atom's valency. It will return 0 for PseudoAtoms, and for atoms for which it
        /// does not have an entry in the configuration file.
        /// </summary>
        public int CalculateNumberOfImplicitHydrogens(IAtom atom, double bondOrderSum, BondOrder maxBondOrder,
                int neighbourCount)
        {

            int missingHydrogens = 0;
            if (atom is IPseudoAtom)
            {
                Debug.WriteLine("don't figure it out... it simply does not lack H's");
                return 0;
            }

            Debug.WriteLine("Calculating number of missing hydrogen atoms");
            // get default atom
            var atomTypes = GetAtomTypeFactory(atom.Builder).GetAtomTypes(atom.Symbol);
            foreach (var type in atomTypes)
            {
                if (CouldMatchAtomType(atom, bondOrderSum, maxBondOrder, type))
                {
                    Debug.WriteLine("This type matches: ", type);
                    int formalNeighbourCount = type.FormalNeighbourCount.Value;
                    switch (type.Hybridization.Ordinal)
                    {
                        case Hybridization.O.Unset:
                            missingHydrogens = (int)(type.BondOrderSum - bondOrderSum);
                            break;
                        case Hybridization.O.SP3:
                            missingHydrogens = formalNeighbourCount - neighbourCount;
                            break;
                        case Hybridization.O.SP2:
                            missingHydrogens = formalNeighbourCount - neighbourCount;
                            break;
                        case Hybridization.O.SP1:
                            missingHydrogens = formalNeighbourCount - neighbourCount;
                            break;
                        default:
                            missingHydrogens = (int)(type.BondOrderSum - bondOrderSum);
                            break;
                    }
                    break;
                }
            }

            Debug.WriteLine("missing hydrogens: ", missingHydrogens);
            return missingHydrogens;
        }

        /// <summary>
        /// Checks whether an Atom is saturated by comparing it with known AtomTypes.
        /// It returns true if the atom is an PseudoAtom and when the element is not in the list.
        /// </summary>
        public bool IsSaturated(IAtom atom, IAtomContainer container)
        {
            if (atom is IPseudoAtom)
            {
                Debug.WriteLine("don't figure it out... it simply does not lack H's");
                return true;
            }

            var atomTypes = GetAtomTypeFactory(atom.Builder).GetAtomTypes(atom.Symbol);
            if (atomTypes.Any())
            {
                Trace.TraceWarning("Missing entry in atom type list for ", atom.Symbol);
                return true;
            }
            double bondOrderSum = container.GetBondOrderSum(atom);
            BondOrder maxBondOrder = container.GetMaximumBondOrder(atom);
            int hcount = atom.ImplicitHydrogenCount.Value;
            int charge = atom.FormalCharge.Value;

            Debug.WriteLine("Checking saturation of atom ", atom.Symbol);
            Debug.WriteLine("bondOrderSum: ", bondOrderSum);
            Debug.WriteLine("maxBondOrder: ", maxBondOrder);
            Debug.WriteLine("hcount: ", hcount);
            Debug.WriteLine("charge: ", charge);

            bool elementPlusChargeMatches = false;
            foreach (var type in atomTypes)
            {
                if (CouldMatchAtomType(atom, bondOrderSum, maxBondOrder, type))
                {
                    if (bondOrderSum + hcount == type.BondOrderSum
                            && !BondManipulator.IsHigherOrder(maxBondOrder, type.MaxBondOrder))
                    {
                        Debug.WriteLine("We have a match: ", type);
                        Debug.WriteLine("Atom is saturated: ", atom.Symbol);
                        return true;
                    }
                    else
                    {
                        // ok, the element and charge matche, but unfulfilled
                        elementPlusChargeMatches = true;
                    }
                } // else: formal charges don't match
            }

            if (elementPlusChargeMatches)
            {
                Debug.WriteLine("No, atom is not saturated.");
                return false;
            }

            // ok, the found atom was not in the list
            Trace.TraceError("Could not find atom type!");
            throw new CDKException("The atom with element " + atom.Symbol + " and charge " + charge + " is not found.");
        }

        public int CalculateNumberOfImplicitHydrogens(IAtom atom, IAtomContainer container)
        {
            return this.CalculateNumberOfImplicitHydrogens(atom, container.GetBondOrderSum(atom),
                    container.GetMaximumBondOrder(atom), container.GetConnectedAtoms(atom).Count());
        }

        protected AtomTypeFactory GetAtomTypeFactory(IChemObjectBuilder builder)
        {
            if (structgenATF == null)
            {
                try
                {
                    structgenATF = AtomTypeFactory.GetInstance(atomTypeList, builder);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception);
                    throw new CDKException("Could not instantiate AtomTypeFactory!", exception);
                }
            }
            return structgenATF;
        }

        /// <summary>
        /// Determines if the atom can be of type AtomType.
        /// </summary>
        public bool CouldMatchAtomType(IAtomContainer container, IAtom atom, IAtomType type)
        {
            double bondOrderSum = container.GetBondOrderSum(atom);
            BondOrder maxBondOrder = container.GetMaximumBondOrder(atom);
            return CouldMatchAtomType(atom, bondOrderSum, maxBondOrder, type);
        }
    }
}
