/*  Copyright (C) 2008  Miguel Rojas <miguelrojasch@yahoo.es>
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
using System;
using System.Linq;

namespace NCDK.Charges
{
    /**
    /// Calculation of the electronegativity of orbitals of a molecule
    /// by the method Gasteiger based on electronegativity is given by X = a + bq + c(q*q).
     *
    /// @author       Miguel Rojas Cherto
    /// @cdk.created  2008-104-31
    /// @cdk.module   charges
    /// @cdk.keyword  electronegativity
    /// @cdk.githash
     */
    public class PiElectronegativity
    {

        private GasteigerMarsiliPartialCharges peoe = null;
        private GasteigerPEPEPartialCharges pepe = null;

        /// <summary>
        /// The maximum number of Iterations.
        /// </summary>
        public int MaxIterations { get; set; } = 6;

        /**Number of maximum resonance structures*/
        /// <summary>
        /// The maximum number of resonance structures.
        /// </summary>
        public int MaxResonanceStructures { get; set; } = 50;

        private IAtomContainer molPi;
        private IAtomContainer acOldP;
        private double[][] gasteigerFactors;

        /**
        /// Constructor for the PiElectronegativity object.
         */
        public PiElectronegativity()
            : this(6, 50)
        { }

        /**
        /// Constructor for the Electronegativity object.
         *
        /// @param maxIterations         The maximal number of Iteration
        /// @param maxResonStruc         The maximal number of Resonance Structures
         */
        public PiElectronegativity(int maxIterations, int maxResonStruc)
        {
            peoe = new GasteigerMarsiliPartialCharges();
            pepe = new GasteigerPEPEPartialCharges();
            MaxIterations = maxIterations;
            MaxResonanceStructures = maxResonStruc;
        }

        /**
        /// calculate the electronegativity of orbitals pi.
         *
        /// @param ac                    IAtomContainer
        /// @param atom                  atom for which effective atom electronegativity should be calculated
         *
        /// @return piElectronegativity
         */
        public double CalculatePiElectronegativity(IAtomContainer ac, IAtom atom)
        {

            return CalculatePiElectronegativity(ac, atom, MaxIterations, MaxResonanceStructures);
        }

        /**
        /// calculate the electronegativity of orbitals pi.
         *
        /// @param ac                    IAtomContainer
        /// @param atom                  atom for which effective atom electronegativity should be calculated
        /// @param maxIterations         The maximal number of Iteration
        /// @param maxResonStruc         The maximal number of Resonance Structures
         *
        /// @return piElectronegativity
         */
        public double CalculatePiElectronegativity(IAtomContainer ac, IAtom atom, int maxIterations, int maxResonStruc)
        {
            MaxIterations = maxIterations;
            MaxResonanceStructures = maxResonStruc;

            double electronegativity = 0;

            try
            {
                if (!ac.Equals(acOldP))
                {
                    molPi = ac.Builder.CreateAtomContainer(ac);

                    peoe = new GasteigerMarsiliPartialCharges();
                    peoe.AssignGasteigerMarsiliSigmaPartialCharges(molPi, true);
                    var iSet = ac.Builder.CreateAtomContainerSet();
                    iSet.Add(molPi);
                    iSet.Add(molPi);

                    gasteigerFactors = pepe.AssignrPiMarsilliFactors(iSet);

                    acOldP = ac;
                }
                IAtom atomi = molPi.Atoms[ac.Atoms.IndexOf(atom)];
                int atomPosition = molPi.Atoms.IndexOf(atomi);
                int stepSize = pepe.StepSize;
                int start = (stepSize * (atomPosition) + atomPosition);
                double q = atomi.Charge.Value;
                if (molPi.GetConnectedLonePairs(molPi.Atoms[atomPosition]).Any()
                        || molPi.GetMaximumBondOrder(atomi) != BondOrder.Single || atomi.FormalCharge != 0)
                {
                    return ((gasteigerFactors[1][start]) + (q * gasteigerFactors[1][start + 1]) + (gasteigerFactors[1][start + 2] * (q * q)));
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.StackTrace);
            }

            return electronegativity;
        }
    }
}
