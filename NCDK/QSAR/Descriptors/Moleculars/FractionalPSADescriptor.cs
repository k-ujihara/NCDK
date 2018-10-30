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
using NCDK.QSAR.Results;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.IO;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Polar surface area expressed as a ratio to molecular size. 
    /// </summary>
    /// <remarks>
    /// Calculates <b>tpsaEfficiency</b>, which is
    /// to <see cref="TPSADescriptor"/> / <b>molecular weight</b>, in units of square Angstroms per Dalton.
    /// Other related descriptors may also be useful to add, e.g. ratio of polar to hydrophobic surface area.
    /// </remarks>
    // @cdk.module qsarmolecular
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:fractionalPSA
    // @cdk.keyword volume
    // @cdk.keyword descriptor
    public class FractionalPSADescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        public FractionalPSADescriptor() { }

        public override IImplementationSpecification Specification => specification;
        private static readonly DescriptorSpecification specification =
         new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#fractionalPSA", 
                typeof(FractionalPSADescriptor).FullName,
                "The Chemistry Development Kit");

        public override IReadOnlyList<object> Parameters
        {
            set
            {
                if (value.Count != 0)
                {
                    throw new CDKException("The FractionalPSADescriptor expects zero parameters");
                }
            }
            get
            {
                return Array.Empty<object>();
            }
        }

        public override IReadOnlyList<string> DescriptorNames { get; } = new string[] { "tpsaEfficiency" };

        private DescriptorValue<Result<double>> GetDummyDescriptorValue(Exception e)
        {
            return new DescriptorValue<Result<double>>(specification, ParameterNames, Parameters, new Result<double>(double.NaN), DescriptorNames, e);
        }

        /// <summary>
        /// Calculates the topological polar surface area and expresses it as a ratio to molecule size.
        /// </summary>
        /// <param name="mol">The <see cref="IAtomContainer"/> whose volume is to be calculated</param>
        /// <returns>Descriptor(s) retaining to polar surface area</returns>
        public DescriptorValue<Result<double>> Calculate(IAtomContainer mol)
        {
            mol = (IAtomContainer)mol.Clone();
            double polar = 0, weight = 0;
            try
            {
                // type & assign implicit hydrogens
                var builder = mol.Builder;
                var matcher = CDK.AtomTypeMatcher;
                foreach (var atom in mol.Atoms)
                {
                    var type = matcher.FindMatchingAtomType(mol, atom);
                    AtomTypeManipulator.Configure(atom, type);
                }
                var adder = CDK.HydrogenAdder;
                adder.AddImplicitHydrogens(mol);

                // polar surface area: chain it off the TPSADescriptor
                TPSADescriptor tpsa = new TPSADescriptor();
                var value = tpsa.Calculate(mol);
                polar = value.Value.Value;

                //  molecular weight
                foreach (var atom in mol.Atoms)
                {
                    weight += BODRIsotopeFactory.Instance.GetMajorIsotope(atom.Symbol).ExactMass.Value;
                    weight += (atom.ImplicitHydrogenCount ?? 0) * 1.00782504;
                }
            }
            catch (Exception exception)
            {
                if (!(exception is CDKException || exception is IOException))
                    throw;
                return GetDummyDescriptorValue(exception);
            }
            return new DescriptorValue<Result<double>>(specification, ParameterNames, Parameters, new Result<double>(
                    weight == 0 ? 0 : polar / weight), DescriptorNames);
        }

        public override IDescriptorResult DescriptorResultType { get; } = new Result<double>();
        public override IReadOnlyList<string> ParameterNames { get; } = Array.Empty<string>();
        public override object GetParameterType(string name) => null;

        IDescriptorValue IMolecularDescriptor.Calculate(IAtomContainer container) => Calculate(container);
    }
}
