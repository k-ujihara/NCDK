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
using System;
using System.Linq;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Evaluates the weight of atoms of a certain element type.
    /// </summary>
    // @author      mfe4
    // @cdk.created 2004-11-13
    // @cdk.module  qsarmolecular
    // @cdk.dictref qsar-descriptors:weight
    [DescriptorSpecification("http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#weight")]
    public class WeightDescriptor : AbstractDescriptor, IMolecularDescriptor
    {
        private readonly IAtomContainer container;

        /// <param name="container">The AtomContainer for which this descriptor is to be calculated. If 'H'
        /// is specified as the element symbol make sure that the AtomContainer has hydrogens.</param>
        public WeightDescriptor(IAtomContainer container)
        {
            this.container = container;
        }

        [DescriptorResult]
        public class Result : AbstractDescriptorResult
        {
            public Result(double value)
            {
                this.Weight = value;
            }

            public Result(Exception e) : base(e) { }

            [DescriptorResultProperty]
            public double Weight { get; private set; }

            public double Value => Weight;
        }

        /// <summary>
        /// Calculate the weight of specified element type in the supplied <see cref="IAtomContainer"/>.
        /// </summary>
        /// <returns>The total weight of atoms of the specified element type</returns>
        /// <param name="symbol">If *, returns the molecular weight, otherwise the weight for the given element</param>
        public Result Calculate(string symbol = "*")
        {
            var iso = CDK.IsotopeFactory;
            var h = iso.GetMajorIsotope(NaturalElements.H.AtomicNumber);

            double weight;
            switch (symbol)
            {
                case "*":
                    weight = container.Atoms
                        .Select(atom => iso.GetMajorIsotope(atom.AtomicNumber).ExactMass.Value
                                      + (atom.ImplicitHydrogenCount ?? 0) * h.ExactMass.Value)
                        .Sum();
                    break;
                case "H":
                    weight = container.Atoms
                        .Select(atom =>
                           (atom.AtomicNumber == NaturalElements.H.AtomicNumber
                            ? 1
                            : (atom.ImplicitHydrogenCount ?? 0)) * h.ExactMass.Value)
                        .Sum();
                    break;
                default:
                    var number = NaturalElement.ToAtomicNumber(symbol);
                    weight = container.Atoms.Count(atom => atom.AtomicNumber == number) * iso.GetMajorIsotope(number).ExactMass.Value;
                    break;
            }

            return new Result(weight);
        }

        IDescriptorResult IMolecularDescriptor.Calculate() => Calculate();
    }
}
