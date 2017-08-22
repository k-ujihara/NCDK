/* Copyright (C) 2002-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.QSAR.Results;
using System;
using System.Collections.Generic;

namespace NCDK.QSAR
{
    /// <summary>
    /// Class that is used to store descriptor values as <see cref="IChemObject"/> properties.
    /// </summary>
    // @cdk.module standard
    // @cdk.githash
    [Serializable]
    public class DescriptorValue
    {
        private DescriptorSpecification specification;
        private IReadOnlyList<string> parameterNames;
        private object[] parameterSettings;
        private IDescriptorResult value;
        private IReadOnlyList<string> descriptorNames;
        private Exception exception;

        /// <summary>
        /// Construct a descriptor value object, representing the numeric values as well as parameters and provenance.
        /// <para>
        /// This constructor should be used when there has been no error during the descriptor calculation
        /// </para>
        /// </summary>
        /// <param name="specification">The specification</param>
        /// <param name="parameterNames">The parameter names for the descriptors</param>
        /// <param name="parameterSettings">The parameter settings</param>
        /// <param name="value">The actual values</param>
        /// <param name="descriptorNames">The names of the values</param>
        public DescriptorValue(DescriptorSpecification specification, IReadOnlyList<string> parameterNames, object[] parameterSettings,
                IDescriptorResult value, IReadOnlyList<string> descriptorNames)
            : this(specification, parameterNames, parameterSettings, value, descriptorNames, null)
        { }

        /// <summary>
        /// Construct a descriptor value object, representing the numeric values as well as parameters and provenance.
        /// <para>
        /// This constructor should be used when there has been an error during the descriptor calculation
        /// </para>
        /// </summary>
        /// <param name="specification">The specification</param>
        /// <param name="parameterNames">The parameter names for the descriptors</param>
        /// <param name="parameterSettings">The parameter settings</param>
        /// <param name="value">The actual values</param>
        /// <param name="descriptorNames">The names of the values</param>
        /// <param name="exception">The exception object that should have been caught if an error occurred during descriptor calculation</param>
        public DescriptorValue(DescriptorSpecification specification, IReadOnlyList<string> parameterNames, object[] parameterSettings,
                IDescriptorResult value, IReadOnlyList<string> descriptorNames, Exception exception)
        {
            this.specification = specification;
            this.parameterNames = parameterNames;
            this.parameterSettings = parameterSettings;
            this.value = value;
            this.descriptorNames = descriptorNames;
            this.exception = exception;
        }

        public DescriptorSpecification Specification => this.specification;
        public object[] Parameters => this.parameterSettings;
        public IReadOnlyList<string> ParameterNames => this.parameterNames;
        public IDescriptorResult Value => this.value;
        public Exception Exception => exception;

        /// <summary>
        /// An array of names for each descriptor value calculated.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Many descriptors return multiple values. In general it is useful for the
        /// descriptor to indicate the names for each value. When a descriptor creates
        /// a <see cref="DescriptorValue"/> object, it should supply an array of names equal
        /// in length to the number of descriptor calculated.
        /// </para>
        /// <para>
        /// In many cases, these names can be as simple as X1, X2, ..., XN where X is a prefix
        /// and 1, 2, ..., N are the indices. On the other hand it is also possible to return
        /// other arbitrary names, which should be documented in the JavaDocs for the descriptor
        /// (e.g., the CPSA descriptor).
        /// </para>
        /// <para>
        /// Note that by default if a descriptor returns a single value (such as <see cref="NCDK.QSAR.Descriptors.Moleculars.ALOGPDescriptor"/>
        /// the return array will have a single element
        /// </para>
        /// <para>
        /// In case a descriptor creates a <see cref="DescriptorValue"/> object with no names, this
        /// method will generate a set of names based on the <see cref="DescriptorSpecification"/> object
        /// supplied at instantiation.
        /// </para>
        /// </remarks>
        public IReadOnlyList<string> Names
        {
            get
            {
                if (descriptorNames == null || descriptorNames.Count == 0)
                {
                    string title = specification.ImplementationTitle;
                    if (value is Result<bool> || value is Result<double> || value is Result<int>)
                    {
                        descriptorNames = new string[] { title };
                    }
                    else
                    {
                        int ndesc = 0;
                        if (value is ArrayResult<double>)
                        {
                            ndesc = value.Length;
                        }
                        else if (value is ArrayResult<int>)
                        {
                            ndesc = value.Length;
                        }
                        var names = new string[ndesc];
                        for (int i = 1; i < ndesc + 1; i++)
                            names[i] = title + i;
                        descriptorNames = names;
                    }
                }
                return descriptorNames;
            }
        }
    }
}
