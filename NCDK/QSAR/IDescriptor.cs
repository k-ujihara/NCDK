/* Copyright (C) 2004-2007  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.QSAR.Result;

namespace NCDK.QSAR
{
    /// <summary>
    /// Classes that implement this interface are QSAR descriptor calculators.
    /// The architecture provides a few subinterfaces such as the
    /// <see cref="IMolecularDescriptor"/>, 
    /// <see cref="IAtomicDescriptor"/> and
    /// <see cref="IBondDescriptor"/>.
    /// </summary>
    /// <remarks>
    /// <para><b>Calculated results</b></para>
    /// <para>The results calculated by the descriptor can have various types, which
    /// extend the <see cref="IDescriptorResult"/>, and is embedded in a
    /// <see cref="DescriptorValue"/>. Currently, there are five result types:
    /// <list type="bullet">
    ///   <item><see cref="BooleanResultType"/></item>
    ///   <item><see cref="DoubleResultType"/> </item>
    ///   <item><see cref="IntegerResultType"/></item>
    ///   <item><see cref="DoubleArrayResultType"/></item>
    ///   <item><see cref="IntegerArrayResultType"/></item>
    /// </list>
    /// But the DescriptorValue will hold an actual value using one of the
    /// following five classes:
    /// <list type="bullet">
    ///   <item><see cref="BooleanResult"/></item>
    ///   <item><see cref="DoubleResult"/> </item>
    ///   <item><see cref="IntegerResult"/></item>
    ///   <item><see cref="DoubleArrayResult"/></item>
    ///   <item><see cref="IntegerArrayResult"/></item>
    /// </list>
    /// </para>
    /// <para>
    ///  The length of the first of these three result types is fixed at
    /// 1. However, the length of the array result types varies, depending
    /// on the used descriptor parameters. The length must not depend on the
    /// IAtomContainer, but only on the parameters.
    /// </para>
    /// <para><b>Parameters</b></para>
    /// <para>
    /// A descriptor may have parameters that specify how the descriptor
    /// is calculated, or to what level of detail. For example, the atom
    /// count descriptor may calculate counts for all elements, or just
    /// the specified element. As an effect, the DescriptorValue results
    /// may vary in length too.
    /// </para>
    /// <para>Each descriptor <b>must</b> provide default parameters, which
    /// allow descriptors to be calculated without having to set parameter
    /// values.
    /// </para>
    /// <para>To interactively query which parameters are available, one can
    /// use the methods <see cref="ParameterNames"/> to see how many
    /// and which parameters are available. To determine what object is
    /// used to set the parameter, the method <see cref="GetParameterType(string)"/>
    /// is used, where the parameter name is used as identifier.
    /// </para>
    /// <para>The default values are retrieved using the <see cref="Parameters"/>
    /// method of a freshly instantiated <see cref="IDescriptor"/>. After use
    /// of <see cref="Parameters"/>, the current parameter values are
    /// returned.</para>
    /// </remarks>
    /// <seealso cref="DescriptorValue"/>
    /// <seealso cref="IDescriptorResult"/>
    // @cdk.module qsar
    // @cdk.githash
    public interface IDescriptor
    {
        /// <summary>
        /// Initialise the descriptor with the specified chem object builder. This
        /// allows descriptors that required domain objects, such as for SMARTS queries
        /// to initialise correctly. If you do not need domain objects then this method
        /// does not need to be implemented.
        /// </summary>
        /// <param name="builder">chem object builder to use with this descriptor</param>
        void Initialise(IChemObjectBuilder builder);

        /// <summary>
        /// A <see cref="IImplementationSpecification"/> which specifies which descriptor is implemented by this class.
        /// </summary>
        /// <remarks>
        /// These fields are used in the map:
        /// <list type="bullet">
        /// <item>
        /// <term>Specification-Reference</term>
        /// <description>refers to an entry in a unique dictionary</description>
        /// </item>
        /// <item>
        /// <term>Implementation-Title</term>
        /// <description>anything</description>
        /// </item>       
        /// <item>
        /// <term>Implementation-Identifier</term>
        /// <description>a unique identifier for this version of this class</description>
        /// </item>
        /// <item>
        /// <term>Implementation-Vendor</term>
        /// <description>CDK, JOELib, or anything else</description>
        /// </item>
        /// </list>
        /// </remarks>
        /// <returns>An object containing the descriptor specification</returns>
        IImplementationSpecification Specification { get; }

        /// <summary>
        /// The names of the parameters for this descriptor. The method
        /// returns <see langword="null"/> or a zero-length <c>object[]</c> array if the descriptor
        ///  does not have any parameters.
        /// </summary>
        /// <returns>An array of string containing the names of the parameters</returns>
        ///         that this descriptor can accept.
        string[] ParameterNames { get; }

        /// <summary>
        /// Returns a class matching that of the parameter with the given name. May
        /// only return null for when 'name' does not match any parameters returned
        /// by the <see cref="Parameters"/> method.
        /// </summary>
        /// <param name="name">The name of the parameter whose type is requested</param>
        /// <returns>An Object of the class corresponding to the parameter with the supplied name</returns>
        object GetParameterType(string name);

        /// <summary>
        /// The current parameter values. If not parameters have been set,
        /// it must return the default parameters. The method returns <see langword="null"/> or a
        /// zero-length <c>object[]</c> array if the descriptor does not have any
        /// parameters.
        /// </summary>
        /// <remarks>
        /// Must be done before callin calculate as the parameters influence the calculation outcome before set.
        /// </remarks>
        object[] Parameters { get; set; }

        /// <summary>
        /// Returns an array of names for each descriptor value calculated.
        /// <para>
        /// Many descriptors return multiple values. In general it is useful for the
        /// descriptor to indicate the names for each value.
        /// </para>
        /// <para>
        /// In many cases, these names can be as simple as X1, X2, ..., XN where X is a prefix
        /// and 1, 2, ..., N are the indices. On the other hand it is also possible to return
        /// other arbitrary names, which should be documented in the Javadocs for the descriptor
        /// (e.g., the CPSA descriptor).
        /// </para>
        /// <para>
        /// Note that by default if a descriptor returns a single value
        /// (such as <see cref="NCDK.QSAR.Descriptors.Moleculars.ALOGPDescriptor"/>
        /// the return array will have a single element
        /// </para>
        /// </summary>
        /// <returns>An array of descriptor names, equal in length to the number of descriptor calculated.</returns>
        string[] DescriptorNames { get; }
    }
}
