/* Copyright (C) 2007  Federico
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using NCDK.Aromaticities;
using NCDK.AtomTypes;
using NCDK.Charges;
using NCDK.Graphs;
using NCDK.Graphs.Matrix;
using NCDK.QSAR.Results;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// This class calculates ATS autocorrelation descriptor, where the weight equal to the charges.
    /// </summary>
    // @author Federico
    // @cdk.created 2007-03-01
    // @cdk.module qsarmolecular
    // @cdk.githash
    public class AutocorrelationDescriptorPolarizability : IMolecularDescriptor
    {
        private static readonly string[] NAMES = { "ATSp1", "ATSp2", "ATSp3", "ATSp4", "ATSp5" };

        private static double[] Listpolarizability(IAtomContainer container, int[][] dmat)
        {
            int natom = container.Atoms.Count;
            double[] polars = new double[natom];

            Polarizability polar = new Polarizability();
            for (int i = 0; i < natom; i++)
            {
                IAtom atom = container.Atoms[i];
                try
                {
                    polars[i] = polar.CalculateGHEffectiveAtomPolarizability(container, atom, false, dmat);
                }
                catch (Exception ex1)
                {
                    throw new CDKException("Problems with assign Polarizability due to " + ex1.ToString(), ex1);
                }
            }

            return polars;
        }

        /// <summary>
        /// This method calculate the ATS Autocorrelation descriptor.
        /// </summary>
        public DescriptorValue<ArrayResult<double>> Calculate(IAtomContainer container)
        {
            IAtomContainer molecule;
            molecule = (IAtomContainer)container.Clone();

            // add H's in case they're not present
            try
            {
                CDKAtomTypeMatcher matcher = CDKAtomTypeMatcher.GetInstance(molecule.Builder);
                foreach (var atom in molecule.Atoms)
                {
                    IAtomType type = matcher.FindMatchingAtomType(molecule, atom);
                    AtomTypeManipulator.Configure(atom, type);
                }
                CDKHydrogenAdder hAdder = CDKHydrogenAdder.GetInstance(molecule.Builder);
                hAdder.AddImplicitHydrogens(molecule);
                AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(molecule);
            }
            catch (Exception e)
            {
                return GetDummyDescriptorValue(new CDKException("Could not add hydrogens: " + e.Message, e));
            }

            // do aromaticity detecttion for calculating polarizability later on
            try
            {
                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            }
            catch (CDKException e)
            {
                return GetDummyDescriptorValue(new CDKException("Could not percieve atom types: " + e.Message, e));
            }
            try
            {
                Aromaticity.CDKLegacy.Apply(molecule);
            }
            catch (CDKException e)
            {
                return GetDummyDescriptorValue(new CDKException("Could not percieve aromaticity: " + e.Message, e));
            }

            // get the distance matrix for pol calcs as well as for later on
            int[][] distancematrix = PathTools.ComputeFloydAPSP(AdjacencyMatrix.GetMatrix(molecule));

            try
            {
                double[] w = Listpolarizability(molecule, distancematrix);
                int natom = molecule.Atoms.Count;
                double[] polarizabilitySum = new double[5];

                for (int k = 0; k < 5; k++)
                {
                    for (int i = 0; i < natom; i++)
                    {
                        if (molecule.Atoms[i].Symbol.Equals("H")) continue;
                        for (int j = 0; j < natom; j++)
                        {
                            if (molecule.Atoms[j].Symbol.Equals("H")) continue;
                            if (distancematrix[i][j] == k)
                            {
                                polarizabilitySum[k] += w[i] * w[j];
                            }
                            else
                                polarizabilitySum[k] += 0.0;
                        }
                    }
                    if (k > 0) polarizabilitySum[k] = polarizabilitySum[k] / 2;

                }
                ArrayResult<double> result = new ArrayResult<double>(5);
                foreach (var aPolarizabilitySum in polarizabilitySum)
                {
                    result.Add(aPolarizabilitySum);

                }

                return new DescriptorValue<ArrayResult<double>>(_Specification, ParameterNames, Parameters, result,
                        DescriptorNames);

            }
            catch (Exception ex)
            {
                return GetDummyDescriptorValue(new CDKException("Error while calculating the ATSpolarizabilty descriptor: "
                        + ex.Message, ex));
            }
        }

        private DescriptorValue<ArrayResult<double>> GetDummyDescriptorValue(Exception e)
        {
            ArrayResult<double> results = new ArrayResult<double>(5);
            for (int i = 0; i < 5; i++)
                results.Add(double.NaN);
            return new DescriptorValue<ArrayResult<double>>(_Specification, ParameterNames, Parameters, results,
                    DescriptorNames, e);
        }

        public IReadOnlyList<string> ParameterNames => null;
        public object GetParameterType(string name) => null;

        public object[] Parameters
        {
            get { return null; }
            set { }
        }

        public IReadOnlyList<string> DescriptorNames => NAMES;

        public IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
         new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#autoCorrelationPolarizability",
               typeof(AutocorrelationDescriptorPolarizability).FullName, 
               "The Chemistry Development Kit");

        public IDescriptorResult DescriptorResultType { get; } = new ArrayResult<double>(5);

        DescriptorValue IMolecularDescriptor.Calculate(IAtomContainer container) => Calculate(container);
    }
}
