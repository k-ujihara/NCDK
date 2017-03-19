/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.Numerics;
using NCDK.Config;
using NCDK.Tools.Manipulator;
using System;
using System.Diagnostics;

namespace NCDK.Charges
{
    /// <summary>
    /// The calculation of the inductive partial atomic charges and equalization of
    /// effective electronegativities is based on {@cdk.cite CHE03}.
    /// </summary>
    // @author      mfe4
    // @cdk.module  charges
    // @cdk.githash
    // @cdk.created 2004-11-03
    // @cdk.keyword partial atomic charges
    // @cdk.keyword charge distribution
    // @cdk.keyword electronegativity
    public class InductivePartialCharges : IChargeCalculator
    {
        private static double[] pauling;
        private IsotopeFactory ifac = null;
        private AtomTypeFactory factory = null;

        /// <summary>
        ///  Constructor for the InductivePartialCharges object.
        /// </summary>
        public InductivePartialCharges()
        {
            if (pauling == null)
            {
                // pauling ElEn :
                // second position is H, last is Ac
                pauling = new double[]{0, 2.1, 0, 1.0, 1.5, 2.0, 2.5, 3.0, 3.5, 4.0, 0, 0.9, 1.2, 1.5, 1.8, 2.1, 2.5, 3.0,
                    0, 0.8, 1.0, 1.3, 1.5, 1.6, 1.6, 1.5, 1.8, 1.8, 1.8, 1.9, 1.6, 1.6, 1.8, 2.0, 2.4, 2.8, 0, 0.8,
                    1.0, 1.3, 1.4, 1.6, 1.8, 1.9, 2.2, 2.2, 2.2, 1.9, 1.7, 1.7, 1.8, 1.9, 2.1, 2.5, 0.7, 0.9, 1.1, 1.3,
                    1.5, 1.7, 1.9, 2.2, 2.2, 2.2, 2.4, 1.9, 1.8, 1.8, 1.9, 2.0, 2.2, 0, 0.7, 0.9, 1.1};
            }
        }

        /// <summary>
        /// Main method, set charge as atom properties.
        /// </summary>
        /// <param name="ac">AtomContainer</param>
        /// <returns>AtomContainer</returns>
        public IAtomContainer AssignInductivePartialCharges(IAtomContainer ac)
        {
            if (factory == null)
            {
                factory = AtomTypeFactory
                        .GetInstance("NCDK.Config.Data.jmol_atomtypes.txt", ac.Builder);
            }

            int stepsLimit = 9;
            IAtom[] atoms = AtomContainerManipulator.GetAtomArray(ac);
            double[] pChInch = new double[atoms.Length * (stepsLimit + 1)];
            double[] ElEn = new double[atoms.Length * (stepsLimit + 1)];
            double[] pCh = new double[atoms.Length * (stepsLimit + 1)];
            double[] startEE = GetPaulingElectronegativities(ac, true);
            for (int e = 0; e < atoms.Length; e++)
            {
                ElEn[e] = startEE[e];
                //Debug.WriteLine("INDU: initial EE "+startEE[e]);
            }
            //double tmp1 = 0;
            //double tmp2 = 0;
            for (int s = 1; s < 10; s++)
            {
                for (int a = 0; a < atoms.Length; a++)
                {
                    pChInch[a + (s * atoms.Length)] = GetAtomicChargeIncrement(ac, a, ElEn, s);
                    pCh[a + (s * atoms.Length)] = pChInch[a + (s * atoms.Length)] + pCh[a + ((s - 1) * atoms.Length)];
                    ElEn[a + (s * atoms.Length)] = ElEn[a + ((s - 1) * atoms.Length)]
                            + (pChInch[a + (s * atoms.Length)] / GetAtomicSoftnessCore(ac, a));
                    if (s == 9)
                    {
                        atoms[a].SetProperty("InductivePartialCharge", pCh[a + (s * atoms.Length)]);
                        atoms[a].SetProperty("EffectiveAtomicElectronegativity", ElEn[a + (s * atoms.Length)]);
                    }
                    //tmp1 = pCh[a + (s * atoms.Length)];
                    //tmp2 = ElEn[a + (s * atoms.Length)];
                    //Debug.WriteLine("DONE step " + s + ", atom " + atoms[a].Symbol + ", ch " + tmp1 + ", ee " + tmp2);
                }
            }
            return ac;
        }

        public void CalculateCharges(IAtomContainer container)
        {
            try
            {
                this.AssignInductivePartialCharges(container);
            }
            catch (Exception exception)
            {
                throw new CDKException("Could not calculate inductive partial charges: " + exception.Message,
                        exception);
            }
        }

        /// <summary>
        ///  Gets the paulingElectronegativities attribute of the
        ///  InductivePartialCharges object.
        /// </summary>
        /// <param name="ac">AtomContainer</param>
        /// <param name="modified">if true, some values are modified by following the reference</param>
        /// <returns>The pauling electronegativities</returns>
        public double[] GetPaulingElectronegativities(IAtomContainer ac, bool modified)
        {
            double[] paulingElectronegativities = new double[ac.Atoms.Count];
            IElement element = null;
            string symbol = null;
            int atomicNumber = 0;
            try
            {
                ifac = Isotopes.Instance;
                for (int i = 0; i < ac.Atoms.Count; i++)
                {
                    IAtom atom = ac.Atoms[i];
                    symbol = ac.Atoms[i].Symbol;
                    element = ifac.GetElement(symbol);
                    atomicNumber = element.AtomicNumber.Value;
                    if (modified)
                    {
                        if (symbol.Equals("Cl"))
                        {
                            paulingElectronegativities[i] = 3.28;
                        }
                        else if (symbol.Equals("Br"))
                        {
                            paulingElectronegativities[i] = 3.13;
                        }
                        else if (symbol.Equals("I"))
                        {
                            paulingElectronegativities[i] = 2.93;
                        }
                        else if (symbol.Equals("H"))
                        {
                            paulingElectronegativities[i] = 2.10;
                        }
                        else if (symbol.Equals("C"))
                        {
                            if (ac.GetMaximumBondOrder(atom) == BondOrder.Single)
                            {
                                // Csp3
                                paulingElectronegativities[i] = 2.20;
                            }
                            else if (ac.GetMaximumBondOrder(atom) == BondOrder.Double)
                            {
                                paulingElectronegativities[i] = 2.31;
                            }
                            else
                            {
                                paulingElectronegativities[i] = 3.15;
                            }
                        }
                        else if (symbol.Equals("O"))
                        {
                            if (ac.GetMaximumBondOrder(atom) == BondOrder.Single)
                            {
                                // Osp3
                                paulingElectronegativities[i] = 3.20;
                            }
                            else if (ac.GetMaximumBondOrder(atom) != BondOrder.Single)
                            {
                                paulingElectronegativities[i] = 4.34;
                            }
                        }
                        else if (symbol.Equals("Si"))
                        {
                            paulingElectronegativities[i] = 1.99;
                        }
                        else if (symbol.Equals("S"))
                        {
                            paulingElectronegativities[i] = 2.74;
                        }
                        else if (symbol.Equals("N"))
                        {
                            paulingElectronegativities[i] = 2.59;
                        }
                        else
                        {
                            paulingElectronegativities[i] = pauling[atomicNumber];
                        }
                    }
                    else
                    {
                        paulingElectronegativities[i] = pauling[atomicNumber];
                    }
                }
                return paulingElectronegativities;
            }
            catch (Exception ex1)
            {
                Debug.WriteLine(ex1);
                throw new CDKException("Problems with IsotopeFactory due to " + ex1.ToString(), ex1);
            }
        }

        /// <summary>
        ///  Gets the atomicSoftnessCore attribute of the InductivePartialCharges object.
        /// </summary>
        /// <remarks>
        /// this method returns the result of the core of the equation of atomic softness
        /// that can be used for qsar descriptors and during the iterative calculation
        /// of effective electronegativity
        /// </remarks>
        /// <param name="ac">AtomContainer</param>
        /// <param name="atomPosition">position of target atom</param>
        /// <returns>The atomicSoftnessCore value</returns>
        /// <exception cref="CDKException"></exception>
        public double GetAtomicSoftnessCore(IAtomContainer ac, int atomPosition)
        {
            if (factory == null)
            {
                factory = AtomTypeFactory
                        .GetInstance("NCDK.Config.Data.jmol_atomtypes.txt", ac.Builder);
            }
            IAtom target = null;
            double core = 0;
            double radiusTarget = 0;
            target = ac.Atoms[atomPosition];
            double partial = 0;
            double radius = 0;
            string symbol = null;
            IAtomType type = null;
            try
            {
                symbol = ac.Atoms[atomPosition].Symbol;
                type = factory.GetAtomType(symbol);
                if (GetCovalentRadius(symbol, ac.GetMaximumBondOrder(target)) > 0)
                {
                    radiusTarget = GetCovalentRadius(symbol, ac.GetMaximumBondOrder(target));
                }
                else
                {
                    radiusTarget = type.CovalentRadius.Value;
                }

            }
            catch (Exception ex1)
            {
                Debug.WriteLine(ex1);
                throw new CDKException("Problems with AtomTypeFactory due to " + ex1.Message, ex1);
            }

            foreach (var atom in ac.Atoms)
            {
                if (!target.Equals(atom))
                {
                    symbol = atom.Symbol;
                    partial = 0;
                    try
                    {
                        type = factory.GetAtomType(symbol);
                    }
                    catch (Exception ex1)
                    {
                        Debug.WriteLine(ex1);
                        throw new CDKException("Problems with AtomTypeFactory due to " + ex1.Message, ex1);
                    }
                    if (GetCovalentRadius(symbol, ac.GetMaximumBondOrder(atom)) > 0)
                    {
                        radius = GetCovalentRadius(symbol, ac.GetMaximumBondOrder(atom));
                    }
                    else
                    {
                        radius = type.CovalentRadius.Value;
                    }
                    partial += radius * radius;
                    partial += (radiusTarget * radiusTarget);
                    partial = partial / (CalculateSquaredDistanceBetweenTwoAtoms(target, atom));
                    core += partial;
                }
            }
            core = 2 * core;
            core = 0.172 * core;
            return core;
        }

        // this method returns the partial charge increment for a given atom
        /// <summary>
        ///  Gets the atomicChargeIncrement attribute of the InductivePartialCharges object.
        /// </summary>
        /// <param name="ac">AtomContainer</param>
        /// <param name="atomPosition">position of target atom</param>
        /// <param name="ElEn">electronegativity of target atom</param>
        /// <param name="step">step in iteration</param>
        /// <returns>The atomic charge increment for the target atom</returns>
        /// <exception cref="CDKException"></exception>
        private double GetAtomicChargeIncrement(IAtomContainer ac, int atomPosition, double[] ElEn, int step)
        {
            IAtom[] allAtoms = null;
            IAtom target = null;
            double incrementedCharge = 0;
            double radiusTarget = 0;
            target = ac.Atoms[atomPosition];
            //Debug.WriteLine("ATOM "+target.Symbol+" AT POSITION "+atomPosition);
            allAtoms = AtomContainerManipulator.GetAtomArray(ac);
            double tmp = 0;
            double radius = 0;
            string symbol = null;
            IAtomType type = null;
            try
            {
                symbol = target.Symbol;
                type = factory.GetAtomType(symbol);
                if (GetCovalentRadius(symbol, ac.GetMaximumBondOrder(target)) > 0)
                {
                    radiusTarget = GetCovalentRadius(symbol, ac.GetMaximumBondOrder(target));
                }
                else
                {
                    radiusTarget = type.CovalentRadius.Value;
                }
            }
            catch (Exception ex1)
            {
                Debug.WriteLine(ex1);
                throw new CDKException("Problems with AtomTypeFactory due to " + ex1.Message, ex1);
            }

            for (int a = 0; a < allAtoms.Length; a++)
            {
                if (!target.Equals(allAtoms[a]))
                {
                    tmp = 0;
                    symbol = allAtoms[a].Symbol;
                    try
                    {
                        type = factory.GetAtomType(symbol);
                    }
                    catch (Exception ex1)
                    {
                        Debug.WriteLine(ex1);
                        throw new CDKException("Problems with AtomTypeFactory due to " + ex1.Message, ex1);
                    }
                    if (GetCovalentRadius(symbol, ac.GetMaximumBondOrder(allAtoms[a])) > 0)
                    {
                        radius = GetCovalentRadius(symbol, ac.GetMaximumBondOrder(allAtoms[a]));
                    }
                    else
                    {
                        radius = type.CovalentRadius.Value;
                    }
                    tmp = (ElEn[a + ((step - 1) * allAtoms.Length)] - ElEn[atomPosition + ((step - 1) * allAtoms.Length)]);
                    tmp = tmp * ((radius * radius) + (radiusTarget * radiusTarget));
                    tmp = tmp / (CalculateSquaredDistanceBetweenTwoAtoms(target, allAtoms[a]));
                    incrementedCharge += tmp;
                    //if(actualStep==1)
                    //Debug.WriteLine("INDU: particular atom "+symbol+ ", radii: "+ radius+ " - " + radiusTarget+", dist: "+CalculateSquaredDistanceBetweenTwoAtoms(target, allAtoms[a]));
                }
            }
            incrementedCharge = 0.172 * incrementedCharge;
            //Debug.WriteLine("Increment: " +incrementedCharge);
            return incrementedCharge;
        }

        /// <summary>
        /// Gets the covalentRadius attribute of the InductivePartialCharges object.
        /// </summary>
        /// <param name="symbol">symbol of the atom</param>
        /// <param name="maxBondOrder">its max bond order</param>
        /// <returns>The covalentRadius value given by the reference</returns>
        private double GetCovalentRadius(string symbol, BondOrder maxBondOrder)
        {
            double radiusTarget = 0;
            if (symbol.Equals("F"))
            {
                radiusTarget = 0.64;
            }
            else if (symbol.Equals("Cl"))
            {
                radiusTarget = 0.99;
            }
            else if (symbol.Equals("Br"))
            {
                radiusTarget = 1.14;
            }
            else if (symbol.Equals("I"))
            {
                radiusTarget = 1.33;
            }
            else if (symbol.Equals("H"))
            {
                radiusTarget = 0.30;
            }
            else if (symbol.Equals("C"))
            {
                if (maxBondOrder == BondOrder.Single)
                {
                    // Csp3
                    radiusTarget = 0.77;
                }
                else if (maxBondOrder == BondOrder.Double)
                {
                    radiusTarget = 0.67;
                }
                else
                {
                    radiusTarget = 0.60;
                }
            }
            else if (symbol.Equals("O"))
            {
                if (maxBondOrder == BondOrder.Single)
                {
                    // Csp3
                    radiusTarget = 0.66;
                }
                else if (maxBondOrder != BondOrder.Single)
                {
                    radiusTarget = 0.60;
                }
            }
            else if (symbol.Equals("Si"))
            {
                radiusTarget = 1.11;
            }
            else if (symbol.Equals("S"))
            {
                radiusTarget = 1.04;
            }
            else if (symbol.Equals("N"))
            {
                radiusTarget = 0.70;
            }
            else
            {
                radiusTarget = 0;
            }
            return radiusTarget;
        }

        /// <summary>
        /// Evaluate the square of the Euclidean distance between two atoms.
        /// </summary>
        /// <param name="atom1">first atom</param>
        /// <param name="atom2">second atom</param>
        /// <returns>squared distance between the 2 atoms</returns>
        private double CalculateSquaredDistanceBetweenTwoAtoms(IAtom atom1, IAtom atom2)
        {
            double distance = 0;
            double tmp = 0;
            Vector3 firstPoint = atom1.Point3D.Value;
            Vector3 secondPoint = atom2.Point3D.Value;
            tmp = Vector3.Distance(firstPoint, secondPoint);
            distance = tmp * tmp;
            return distance;
        }
    }
}
