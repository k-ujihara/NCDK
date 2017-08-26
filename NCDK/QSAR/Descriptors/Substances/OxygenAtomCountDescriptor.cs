/* Copyright (C) 2011-2015  Egon Willighagen <egonw@users.sf.net>
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License as published by the Free
 * Software Foundation; either version 2.1 of the License, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more
 * details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation, Inc.,
 * 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.QSAR.Results;
using System;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Substances
{
    /// <summary>
    /// Descriptor that returns the number of oxygens in the chemical
    /// formula. Originally aimed at metal oxide nanoparticles <token>cdk-cite-Liu2011</token>.
    /// </summary>
    // @author      egonw
    // @cdk.githash
    public class OxygenAtomCountDescriptor : ISubstanceDescriptor
    {
        /// <inheritdoc/>
        public IReadOnlyList<string> DescriptorNames { get; } = new string[] { "NoMe" };

        /// <inheritdoc/>
        public IReadOnlyList<string> ParameterNames => Array.Empty<string>();

        /// <inheritdoc/>
        public object GetParameterType(string substance) => null;

        /// <inheritdoc/>
        public object[] Parameters
        {
            get { return Array.Empty<object>(); }
            set { }
        }

        /// <inheritdoc/>
        public IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://egonw.github.com/resource/NM_001002",
                typeof(OxygenAtomCountDescriptor).FullName,
                "The Chemistry Development Kit");

        /// <inheritdoc/>
        public DescriptorValue<Result<int>> Calculate(ISubstance substance)
        {
            int count = 0;
            if (substance != null)
            {
                foreach (var container in substance)
                {
                    foreach (var atom in container.Atoms)
                    {
                        if ("O".Equals(atom.Symbol) || 8 == atom.AtomicNumber)
                            count++;
                    }
                }
            }

            return new DescriptorValue<Result<int>>(
                _Specification, ParameterNames, Parameters,
                new Result<int>(count), DescriptorNames
            );
        }

        DescriptorValue ISubstanceDescriptor.Calculate(ISubstance substance)
            => Calculate(substance);

        /// <inheritdoc/>
        public IDescriptorResult DescriptorResultType { get; } = Result<int>.Instance;
    }
}
