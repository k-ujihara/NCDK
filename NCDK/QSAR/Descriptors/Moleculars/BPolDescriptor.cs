/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
 *
 *  Contact: cdk-devel@lists.sourceforge.net
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
using NCDK.Config;
using NCDK.QSAR.Results;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Sum of the absolute value of the difference between atomic polarizabilities
    ///  of all bonded atoms in the molecule (including implicit hydrogens) with polarizabilities taken from
    /// http://www.sunysccc.edu/academic/mst/ptable/p-table2.htm
    /// 
    /// This descriptor assumes 2-centered bonds.
    /// </summary>
    /// <remarks>
    /// <para>This descriptor uses these parameters:
    /// <list type="table">
    /// <listheader><term>Name</term><term>Default</term><term>Description</term></listheader>
    /// <item><term></term><term></term><term>no parameters</term></item>
    /// </list>
    /// </para>
    /// Returns a single value with name <i>bpol</i>.
    /// </remarks>
    // @author      mfe4
    // @cdk.created 2004-11-13
    // @cdk.module  qsarmolecular
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:bpol
    public class BPolDescriptor : IMolecularDescriptor
    {
        /* Atomic polarizabilities ordered by atomic number from 1 to 102. */
        private static double[] polarizabilities;
        private static readonly string[] NAMES = { "bpol" };

        /// <summary>
        ///  Constructor for the APolDescriptor object
        /// </summary>
        public BPolDescriptor()
        {
            // atomic polarizabilities ordered by atomic number from 1 to 102
            if (polarizabilities == null)
            {
                polarizabilities = new double[]{0, 0.666793, 0.204956, 24.3, 5.6, 3.03, 1.76, 1.1, 0.802, 0.557, 0.3956,
                    23.6, 10.6, 6.8, 5.38, 3.63, 2.9, 2.18, 1.6411, 43.4, 22.8, 17.8, 14.6, 12.4, 11.6, 9.4, 8.4, 7.5,
                    6.8, 6.1, 7.1, 8.12, 6.07, 4.31, 3.77, 3.05, 2.4844, 47.3, 27.6, 22.7, 17.9, 15.7, 12.8, 11.4, 9.6,
                    8.6, 4.8, 7.2, 7.2, 10.2, 7.7, 6.6, 5.5, 5.35, 4.044, 59.6, 39.7, 31.1, 29.6, 28.2, 31.4, 30.1,
                    28.8, 27.7, 23.5, 25.5, 24.5, 23.6, 22.7, 21.8, 21, 21.9, 16.2, 13.1, 11.1, 9.7, 8.5, 7.6, 6.5,
                    5.8, 5.7, 7.6, 6.8, 7.4, 6.8, 6, 5.3, 48.7, 38.3, 32.1, 32.1, 25.4, 27.4, 24.8, 24.5, 23.3, 23,
                    22.7, 20.5, 19.7, 23.8, 18.2, 17.5};
            }
        }

        public IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
         new DescriptorSpecification(
             "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#bpol",
             typeof(BPolDescriptor).FullName,
             "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the BPolDescriptor object
        /// </summary>
        /// <exception cref="CDKException">Description of the Exception</exception>
        public object[] Parameters
        {
            set
            {
                // no parameters for this descriptor
            }
            get
            {
                // no parameters for this descriptor
                return (null);
            }
        }

        public IReadOnlyList<string> DescriptorNames => NAMES;

        /// <summary>
        ///  This method calculate the sum of the absolute value of
        ///  the difference between atomic polarizabilities of all bonded atoms in the molecule
        /// </summary>
        /// <param name="container">Parameter is the atom container.</param>
        /// <returns>The sum of atomic polarizabilities</returns>
        public DescriptorValue<Result<double>> Calculate(IAtomContainer container)
        {
            double bpol = 0;
            int atomicNumber0;
            int atomicNumber1;
            double difference;
            try
            {
                IsotopeFactory ifac = Isotopes.Instance;
                IElement element0;
                IElement element1;

                string symbol0;
                string symbol1;
                foreach (var bond in container.Bonds)
                {
                    IAtom atom0 = bond.Atoms[0];
                    IAtom atom1 = bond.Atoms[1];

                    symbol0 = atom0.Symbol;
                    symbol1 = atom1.Symbol;
                    element0 = ifac.GetElement(symbol0);
                    element1 = ifac.GetElement(symbol1);
                    atomicNumber0 = element0.AtomicNumber.Value;
                    atomicNumber1 = element1.AtomicNumber.Value;
                    difference = polarizabilities[atomicNumber0] - polarizabilities[atomicNumber1];
                    bpol += Math.Abs(difference);
                }

                // after going through the bonds, we go through the atoms and see if they have
                // implicit H's and if so, consider the associated "implicit" bonds. Note that
                // if the count is UNSET, we assume it is 0
                foreach (var atom in container.Atoms)
                {
                    int impH = atom.ImplicitHydrogenCount ?? 0;
                    IElement elem = ifac.GetElement(atom.Symbol);
                    int atnum = elem.AtomicNumber.Value;
                    difference = Math.Abs(polarizabilities[atnum] - polarizabilities[1]) * impH;
                    bpol += difference;
                }
                return new DescriptorValue<Result<double>>(_Specification, ParameterNames, Parameters,
                        new Result<double>(bpol), DescriptorNames);
            }
            catch (Exception ex1)
            {
                Debug.WriteLine(ex1);
                return new DescriptorValue<Result<double>>(_Specification, ParameterNames, Parameters, new Result<double>(
                        double.NaN), DescriptorNames, new CDKException("Problems with IsotopeFactory due to "
                        + ex1.ToString(), ex1));
            }
        }

        /// <summary>
        /// Returns the specific type of the DescriptorResult object.
        /// <para>
        /// The return value from this method really indicates what type of result will
        /// be obtained from the <see cref="DescriptorValue"/> object. Note that the same result
        /// can be achieved by interrogating the <see cref="DescriptorValue"/> object; this method
        /// allows you to do the same thing, without actually calculating the descriptor.</para>
        /// </summary>
        public IDescriptorResult DescriptorResultType { get; } = new Result<double>(0.0);

        /// <summary>
        ///  The parameterNames attribute of the BPolDescriptor object
        /// </summary>
        public IReadOnlyList<string> ParameterNames => null; // no param names to return

        /// <summary>
        ///  Gets the parameterType attribute of the BPolDescriptor object
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>The parameterType value</returns>
        public object GetParameterType(string name) => null;

        IDescriptorValue IMolecularDescriptor.Calculate(IAtomContainer container) => Calculate(container);
    }
}

