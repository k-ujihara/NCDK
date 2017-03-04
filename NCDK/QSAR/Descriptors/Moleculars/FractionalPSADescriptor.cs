/* Copyright (c) 2014 Collaborative Drug Discovery, Inc. <alex@collaborativedrug.com>
 *
 * Implemented by Alex M. Clark, produced by Collaborative Drug Discovery, Inc.
 * Made available to the CDK community under the terms of the GNU LGPL.
 *
 *    http://collaborativedrug.com
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
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

using NCDK.AtomTypes;
using NCDK.Config;
using NCDK.QSAR.Result;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System;
using System.IO;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Polar surface area expressed as a ratio to molecular size. Calculates <b>tpsaEfficiency</b>, which is
    /// to <see cref="TPSADescriptor"/> / <b>molecular weight</b>, in units of square Angstroms per Dalton.
    ///
    /// Other related descriptors may also be useful to add, e.g. ratio of polar to hydrophobic surface area.
    ///
    // @cdk.module qsarmolecular
    // @cdk.githash
    ///
    // @cdk.set     qsar-descriptors
    // @cdk.dictref qsar-descriptors:fractionalPSA
    // @cdk.keyword volume
    // @cdk.keyword descriptor
    /// </summary>
    public class FractionalPSADescriptor : IMolecularDescriptor
    {
        public FractionalPSADescriptor() { }

        public void Initialise(IChemObjectBuilder builder) { }

        public IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
         new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#fractionalPSA", 
                typeof(FractionalPSADescriptor).FullName,
                "The Chemistry Development Kit");

        public object[] Parameters
        {
            set
            {
                if (value.Length != 0)
                {
                    throw new CDKException("The FractionalPSADescriptor expects zero parameters");
                }
            }
            get
            {
                return Array.Empty<object>();
            }
        }

        public string[] DescriptorNames { get; } = new string[] { "tpsaEfficiency" };

        private DescriptorValue GetDummyDescriptorValue(Exception e)
        {
            return new DescriptorValue(_Specification, ParameterNames, Parameters, new DoubleResult(double.NaN), DescriptorNames, e);
        }

        /// <summary>
        /// Calculates the topological polar surface area and expresses it as a ratio to molecule size.
        /// </summary>
        /// <param name="atomContainer">The <see cref="IAtomContainer"/> whose volume is to be calculated</param>
        /// <returns>Descriptor(s) retaining to polar surface area</returns>
        public DescriptorValue Calculate(IAtomContainer mol)
        {
            mol = (IAtomContainer)mol.Clone();
            double polar = 0, weight = 0;
            try
            {
                // type & assign implicit hydrogens
                IChemObjectBuilder builder = mol.Builder;
                CDKAtomTypeMatcher matcher = CDKAtomTypeMatcher.GetInstance(builder);
                foreach (var atom in mol.Atoms)
                {
                    IAtomType type = matcher.FindMatchingAtomType(mol, atom);
                    AtomTypeManipulator.Configure(atom, type);
                }
                CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(builder);
                adder.AddImplicitHydrogens(mol);

                // polar surface area: chain it off the TPSADescriptor
                TPSADescriptor tpsa = new TPSADescriptor();
                DescriptorValue value = tpsa.Calculate(mol);
                polar = ((DoubleResult)value.GetValue()).Value;

                //  molecular weight
                foreach (var atom in mol.Atoms)
                {
                    weight += Isotopes.Instance.GetMajorIsotope(atom.Symbol).ExactMass.Value;
                    weight += (atom.ImplicitHydrogenCount ?? 0) * 1.00782504;
                }
            }
            catch (Exception exception)
            {
                if (!(exception is CDKException || exception is IOException))
                    throw;
                return GetDummyDescriptorValue(exception);
            }
            return new DescriptorValue(_Specification, ParameterNames, Parameters, new DoubleResult(
                    weight == 0 ? 0 : polar / weight), DescriptorNames);
        }

        public IDescriptorResult DescriptorResultType { get; } = new DoubleResultType();
        public string[] ParameterNames { get; } = Array.Empty<string>();
        public object GetParameterType(string name) => null;
    }
}
