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

using System;

namespace NCDK.QSAR
{
    /// <summary>
    /// Class that is used to distribute descriptor specifications.
    /// </summary>
    // @cdk.module standard
    // @cdk.githash
    [Serializable]
    public class DescriptorSpecification : IImplementationSpecification
    {
        public string SpecificationReference { get; private set; }
        public string ImplementationTitle { get; private set; }
        public string ImplementationIdentifier { get; private set; }
        public string ImplementationVendor { get; private set; }

        /// <summary>
        /// Container for specifying the type of descriptor.
        /// </summary>
        /// <param name="specificationReference">
        /// Reference to a formal definition in a
        /// dictionary (e.g. in STMML format) of the descriptor, preferably
        /// referring to the original article. The format of the content is
        /// expected to be &lt;dictionaryNameSpace&gt;:&lt;entryID&gt;.</param>
        /// <param name="implementationTitle">
        /// Title for the implementation of the descriptor
        /// for which the algorithm is given by the specification.</param>
        /// <param name="implementationIdentifier">Unique identifier for the actual
        /// implementation, preferably including the exact version number of
        /// the source code. E.g. $Id$ can be used when the source code is
        /// in a CVS repository.</param>
        /// <param name="implementationVendor">
        /// Name of the organisation/person/program/whatever who wrote/packaged the implementation.</param>
        public DescriptorSpecification(string specificationReference, string implementationTitle, string implementationIdentifier, string implementationVendor)
        {
            SpecificationReference = specificationReference;
            ImplementationTitle = implementationTitle;
            ImplementationIdentifier = implementationIdentifier;
            ImplementationVendor = implementationVendor;
        }

        /// <summary>
        /// Container for specifying the type of descriptor. The specificationIdentifier is
        /// defined by the CDK version.
        /// </summary>
        /// <param name="specificationReference">Reference to a formal definition in a
        /// dictionary (e.g. in STMML format) of the descriptor, preferably
        /// referring to the original article. The format of the content is
        /// expected to be &lt;dictionaryNameSpace&gt;:&lt;entryID&gt;.</param>
        /// <param name="implementationTitle">Title for the implementation of the descriptor
        /// for which the algorithm is given by the specification.</param>
        /// <param name="implementationVendor">
        /// Name of the organisation/person/program/whatever who wrote/packaged the implementation.</param>
        public DescriptorSpecification(string specificationReference, string implementationTitle,  string implementationVendor)
        {
            SpecificationReference = specificationReference;
            ImplementationTitle = implementationTitle;
            ImplementationIdentifier = CDK.Version;
            ImplementationVendor = implementationVendor;
        }

        public override string ToString()
        {
            return nameof(DescriptorSpecification) + "{"
                + "SpecificationReference=" + (this.SpecificationReference ?? "") + ", "
                + "ImplementationTitle=" + (this.ImplementationTitle ?? "") + ", "
                + "ImplementationIdentifier=" + (this.ImplementationIdentifier ?? "") + ", "
                + "ImplementationVendor=" + (this.ImplementationVendor ?? "") + "}";
        }
    }
}
