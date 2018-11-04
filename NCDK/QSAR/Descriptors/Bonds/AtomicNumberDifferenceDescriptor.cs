/* Copyright (C) 2007-2008  Egon Willighagen <egonw@users.sf.net>
 *                    2014  Mark B Vine (orcid:0000-0002-7794-0426)
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

using NCDK.Config;
using NCDK.QSAR.Results;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Bonds
{
    /// <summary>
    /// Describes the imbalance in atomic number of the <see cref="IBond"/> .
    /// </summary>
    // @author      Egon Willighagen
    // @cdk.created 2007-12-29
    // @cdk.module  qsarbond
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:bondAtomicNumberImbalance
    public partial class AtomicNumberDifferenceDescriptor
        : IBondDescriptor
    {
        private static IsotopeFactory factory = CDK.IsotopeFactory;

        private readonly static string[] NAMES = { "MNDiff" };

        public AtomicNumberDifferenceDescriptor()
        {
        }

        public IImplementationSpecification Specification => specification;
        private static readonly DescriptorSpecification specification =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#bondAtomicNumberImbalance",
                typeof(AtomicNumberDifferenceDescriptor).FullName, "The Chemistry Development Kit");

        public IReadOnlyList<object> Parameters { get { return null; } set { } }

        public IReadOnlyList<string> DescriptorNames => NAMES;

        public DescriptorValue<Result<double>> Calculate(IBond bond, IAtomContainer ac)
        {
            if (bond.Atoms.Count != 2)
            {
                return new DescriptorValue<Result<double>>(specification, ParameterNames, Parameters, new Result<double>(
                        double.NaN), NAMES, new CDKException("Only 2-center bonds are considered"));
            }

            var atoms = BondManipulator.GetAtomArray(bond);

            return new DescriptorValue<Result<double>>(specification, ParameterNames, Parameters, new Result<double>(
                    Math.Abs(factory.GetElement(atoms[0].Symbol).AtomicNumber.Value - factory.GetElement(atoms[1].Symbol).AtomicNumber.Value)), NAMES);
        }

        public IReadOnlyList<string> ParameterNames => Array.Empty<string>();

        public object GetParameterType(string name) => null;
    }
}
