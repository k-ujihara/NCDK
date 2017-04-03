/* Copyright (C) 2005-2007  Christian Hoppe <chhoppe@users.sf.net>
 *
 *  Contact: cdk-devel@list.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.AtomTypes;
using NCDK.Graphs;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NCDK.Charges
{
    /// <summary>
    /// Calculation of the polarizability of a molecule by the method of Kang and
    /// Jhon and Gasteiger based on <token>cdk-cite-KJ81</token> and <token>cdk-cite-GH82</token>
    /// Limitations in parameterization of atoms:
    /// H, Csp3, Csp2, Csp2arom, Csp3, Nsp3, Nsp2, Nsp3,
    /// P, Osp3 and Osp2. Aromaticity must be calculated beforehand.
    /// </summary>
    // @author         chhoppe
    // @cdk.githash
    // @cdk.created    2004-11-03
    // @cdk.keyword polarizability
    // @cdk.module     charges
    public class Polarizability
    {
        /// <summary>
        /// Constructor for the Polarizability object.
        /// </summary>
        public Polarizability() { }

        private void AddExplicitHydrogens(IAtomContainer container)
        {
            try
            {
                CDKAtomTypeMatcher matcher = CDKAtomTypeMatcher.GetInstance(container.Builder);
                foreach (var atom in container.Atoms)
                {
                    IAtomType type = matcher.FindMatchingAtomType(container, atom);
                    AtomTypeManipulator.Configure(atom, type);
                }
                CDKHydrogenAdder hAdder = CDKHydrogenAdder.GetInstance(container.Builder);
                hAdder.AddImplicitHydrogens(container);
                AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(container);
            }
            catch (Exception)
            {
                Debug.WriteLine("Error in hydrogen addition");
            }
        }

        /// <summary>
        ///  Gets the polarizabilitiyFactorForAtom.
        /// </summary>
        /// <param name="atomContainer">AtomContainer</param>
        /// <param name="atom">atom for which the factor should become known</param>
        /// <returns>The polarizabilitiyFactorForAtom value</returns>
        public double GetPolarizabilitiyFactorForAtom(IAtomContainer atomContainer, IAtom atom)
        {
            IAtomContainer acH = atomContainer.Builder.CreateAtomContainer(atomContainer);
            AddExplicitHydrogens(acH);
            return GetKJPolarizabilityFactor(acH, atom);
        }

        /// <summary>
        ///  calculates the mean molecular polarizability as described in paper of Kang and Jhorn.
        /// </summary>
        /// <param name="atomContainer">AtomContainer</param>
        /// <returns>polarizabilitiy</returns>
        public double CalculateKJMeanMolecularPolarizability(IAtomContainer atomContainer)
        {
            double polarizabilitiy = 0;
            IAtomContainer acH = atomContainer.Builder.CreateAtomContainer(atomContainer);
            AddExplicitHydrogens(acH);
            for (int i = 0; i < acH.Atoms.Count; i++)
            {
                polarizabilitiy += GetKJPolarizabilityFactor(acH, acH.Atoms[i]);
            }
            return polarizabilitiy;
        }

        /// <summary>
        ///  calculate effective atom polarizability.
        /// </summary>
        /// <param name="atomContainer">IAtomContainer</param>
        /// <param name="atom">atom for which effective atom polarizability should be calculated</param>
        /// <param name="influenceSphereCutOff">cut off for spheres which should taken into account for calculation</param>
        /// <param name="addExplicitH">if set to true, then explicit H's will be added, otherwise it assumes that they have
        ///  been added to the molecule before being called</param>
        /// <returns>polarizabilitiy</returns>
        public double CalculateGHEffectiveAtomPolarizability(IAtomContainer atomContainer, IAtom atom,
                int influenceSphereCutOff, bool addExplicitH)
        {
            double polarizabilitiy = 0;

            IAtomContainer acH;
            if (addExplicitH)
            {
                acH = atomContainer.Builder.CreateAtomContainer(atomContainer);
                AddExplicitHydrogens(acH);
            }
            else
            {
                acH = atomContainer;
            }

            var startAtom = new List<IAtom>(1);
            startAtom.Insert(0, atom);
            double bond;

            polarizabilitiy += GetKJPolarizabilityFactor(acH, atom);
            for (int i = 0; i < acH.Atoms.Count; i++)
            {
                if (acH.Atoms[i] != atom)
                {
                    bond = PathTools.BreadthFirstTargetSearch(acH, startAtom, acH.Atoms[i], 0, influenceSphereCutOff);
                    if (bond == 1)
                    {
                        polarizabilitiy += GetKJPolarizabilityFactor(acH, acH.Atoms[i]);
                    }
                    else
                    {
                        polarizabilitiy += (Math.Pow(0.5, bond - 1) * GetKJPolarizabilityFactor(acH, acH.Atoms[i]));
                    }//if bond==0
                }//if !=atom
            }//for
            return polarizabilitiy;
        }

        /// <summary>
        /// calculate effective atom polarizability.
        /// </summary>
        /// <param name="atomContainer">IAtomContainer</param>
        /// <param name="atom">atom for which effective atom polarizability should be calculated</param>
        /// <param name="addExplicitH">if set to true, then explicit H's will be added, otherwise it assumes that they have
        ///                              been added to the molecule before being called</param>
        /// <param name="distanceMatrix">an n x n matrix of topological distances between all the atoms in the molecule.
        ///                              if this argument is non-null, then BFS will not be used and instead path lengths will be looked up. This
        ///                              form of the method is useful, if it is being called for multiple atoms in the same molecule</param>
        /// <returns>polarizabilitiy</returns>
        public double CalculateGHEffectiveAtomPolarizability(IAtomContainer atomContainer, IAtom atom,
                bool addExplicitH, int[][] distanceMatrix)
        {
            double polarizabilitiy = 0;

            IAtomContainer acH;
            if (addExplicitH)
            {
                acH = atomContainer.Builder.CreateAtomContainer(atomContainer);
                AddExplicitHydrogens(acH);
            }
            else
            {
                acH = atomContainer;
            }

            List<IAtom> startAtom = new List<IAtom>(1);
            startAtom.Insert(0, atom);
            double bond;

            polarizabilitiy += GetKJPolarizabilityFactor(acH, atom);
            for (int i = 0; i < acH.Atoms.Count; i++)
            {
                if (acH.Atoms[i] != atom)
                {
                    int atomIndex = atomContainer.Atoms.IndexOf(atom);
                    bond = distanceMatrix[atomIndex][i];
                    if (bond == 1)
                    {
                        polarizabilitiy += GetKJPolarizabilityFactor(acH, acH.Atoms[i]);
                    }
                    else
                    {
                        polarizabilitiy += (Math.Pow(0.5, bond - 1) * GetKJPolarizabilityFactor(acH, acH.Atoms[i]));
                    }//if bond==0
                }//if !=atom
            }//for
            return polarizabilitiy;
        }

        /// <summary>
        ///  calculate bond polarizability.
        /// </summary>
        /// <param name="atomContainer">AtomContainer</param>
        /// <param name="bond">Bond bond for which the polarizabilitiy should be calculated</param>
        /// <returns>polarizabilitiy</returns>
        public double CalculateBondPolarizability(IAtomContainer atomContainer, IBond bond)
        {
            double polarizabilitiy = 0;
            IAtomContainer acH = atomContainer.Builder.CreateAtomContainer(atomContainer);
            AddExplicitHydrogens(acH);
            if (bond.Atoms.Count == 2)
            {
                polarizabilitiy += GetKJPolarizabilityFactor(acH, bond.Atoms[0]);
                polarizabilitiy += GetKJPolarizabilityFactor(acH, bond.Atoms[1]);
            }
            return (polarizabilitiy / 2);
        }

        /// <summary>
        ///  Method which assigns the polarizabilitiyFactors.
        /// </summary>
        /// <param name="atomContainer">AtomContainer</param>
        /// <param name="atom">Atom</param>
        /// <returns>double polarizabilitiyFactor</returns>
        private double GetKJPolarizabilityFactor(IAtomContainer atomContainer, IAtom atom)
        {
            double polarizabilitiyFactor = 0;
            string AtomSymbol;
            AtomSymbol = atom.Symbol;
            if (AtomSymbol.Equals("H"))
            {
                polarizabilitiyFactor = 0.387;
            }
            else if (AtomSymbol.Equals("C"))
            {
                if (atom.IsAromatic)
                {
                    polarizabilitiyFactor = 1.230;
                }
                else if (atomContainer.GetMaximumBondOrder(atom) == BondOrder.Single)
                {
                    polarizabilitiyFactor = 1.064;/* 1.064 */
                }
                else if (atomContainer.GetMaximumBondOrder(atom) == BondOrder.Double)
                {
                    if (GetNumberOfHydrogen(atomContainer, atom) == 0)
                    {
                        polarizabilitiyFactor = 1.382;
                    }
                    else
                    {
                        polarizabilitiyFactor = 1.37;
                    }
                }
                else if (atomContainer.GetMaximumBondOrder(atom) == BondOrder.Triple
                      || atomContainer.GetMaximumBondOrder(atom) == BondOrder.Quadruple)
                {
                    polarizabilitiyFactor = 1.279;
                }
            }
            else if (AtomSymbol.Equals("N"))
            {
                if (atom.Charge != null && atom.Charge < 0)
                {
                    polarizabilitiyFactor = 1.090;
                }
                else if (atomContainer.GetMaximumBondOrder(atom) == BondOrder.Single)
                {
                    polarizabilitiyFactor = 1.094;
                }
                else if (atomContainer.GetMaximumBondOrder(atom) == BondOrder.Double)
                {
                    polarizabilitiyFactor = 1.030;
                }
                else
                {
                    polarizabilitiyFactor = 0.852;
                }
            }
            else if (AtomSymbol.Equals("O"))
            {
                if (atom.Charge != null && atom.Charge == -1)
                {
                    polarizabilitiyFactor = 1.791;
                }
                else if (atom.Charge != null && atom.Charge == 1)
                {
                    polarizabilitiyFactor = 0.422;
                }
                else if (atomContainer.GetMaximumBondOrder(atom) == BondOrder.Single)
                {
                    polarizabilitiyFactor = 0.664;
                }
                else if (atomContainer.GetMaximumBondOrder(atom) == BondOrder.Double)
                {
                    polarizabilitiyFactor = 0.460;
                }
            }
            else if (AtomSymbol.Equals("P"))
            {
                if (atomContainer.GetConnectedBonds(atom).Count() == 4
                        && atomContainer.GetMaximumBondOrder(atom) == BondOrder.Double)
                {
                    polarizabilitiyFactor = 0;
                }
            }
            else if (AtomSymbol.Equals("S"))
            {
                if (atom.IsAromatic)
                {
                    polarizabilitiyFactor = 3.38;
                }
                else if (atomContainer.GetMaximumBondOrder(atom) == BondOrder.Single)
                {
                    polarizabilitiyFactor = 3.20;/* 3.19 */
                }
                else if (atomContainer.GetMaximumBondOrder(atom) == BondOrder.Double)
                {
                    if (GetNumberOfHydrogen(atomContainer, atom) == 0)
                    {
                        polarizabilitiyFactor = 3.51;
                    }
                    else
                    {
                        polarizabilitiyFactor = 3.50;
                    }
                }
                else
                {
                    polarizabilitiyFactor = 3.42;
                }
            }
            else if (AtomSymbol.Equals("F"))
            {
                polarizabilitiyFactor = 0.296;
            }
            else if (AtomSymbol.Equals("Cl"))
            {
                polarizabilitiyFactor = 2.343;
            }
            else if (AtomSymbol.Equals("Br"))
            {
                polarizabilitiyFactor = 3.5;
            }
            else if (AtomSymbol.Equals("I"))
            {
                polarizabilitiyFactor = 5.79;
            }
            return polarizabilitiyFactor;
        }

        /// <summary>
        ///  Gets the numberOfHydrogen attribute of the Polarizability object.
        /// </summary>
        /// <param name="atomContainer">Description of the Parameter</param>
        /// <param name="atom">Description of the Parameter</param>
        /// <returns>The numberOfHydrogen value</returns>
        private int GetNumberOfHydrogen(IAtomContainer atomContainer, IAtom atom)
        {
            var bonds = atomContainer.GetConnectedBonds(atom);
            IAtom connectedAtom;
            int hCounter = 0;
            foreach (var bond in bonds)
            {
                connectedAtom = bond.GetConnectedAtom(atom);
                if (connectedAtom.Symbol.Equals("H"))
                {
                    hCounter += 1;
                }
            }
            return hCounter;
        }
    }
}
