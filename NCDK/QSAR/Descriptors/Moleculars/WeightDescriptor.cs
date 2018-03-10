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

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    ///  IDescriptor based on the weight of atoms of a certain element type.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///  If the wild-card symbol *
    ///  is specified, the returned value is the molecular weight.
    ///  If an invalid element symbol is specified, the return value is
    ///  0 and no exception is thrown
    ///  </para>
    /// <para>This descriptor uses these parameters:
    /// <list type="table">
    ///   <item>
    ///     <term>Name</term>
    ///     <term>Default</term>
    ///     <term>Description</term>
    ///   </item>
    ///   <item>
    ///     <term>elementSymbol</term>
    ///     <term>*</term>
    ///     <term>If *, returns the molecular weight, otherwise the weight for the given element</term>
    ///   </item>
    /// </list>
    /// </para>
    /// <para>
    /// Returns a single value named <i>wX</i> where <i>X</i> is the chemical symbol
    /// or <i>MW</i> if * is specified as a parameter.
    /// </para>
    /// </remarks>
    // @author      mfe4
    // @cdk.created 2004-11-13
    // @cdk.module  qsarmolecular
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:weight
    public class WeightDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        private string elementName = "*";

        /// <summary>
        ///  Constructor for the WeightDescriptor object.
        /// </summary>
        public WeightDescriptor() { }

        /// <inheritdoc/>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification("http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#weight",
                typeof(WeightDescriptor).FullName,
                "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the WeightDescriptor object.
        /// </summary>
        /// <exception cref="CDKException">if more than 1 parameter is specified or if the parameter is not of type string</exception>
        public override object[] Parameters
        {
            set
            {
                if (value.Length > 1)
                {
                    throw new CDKException("weight only expects one parameter");
                }
                if (!(value[0] is string))
                {
                    throw new CDKException("The parameter must be of type string");
                }
                // ok, all should be fine
                elementName = (string)value[0];
            }
            get
            {
                // return the parameters as used for the descriptor calculation
                return new object[] { elementName };
            }
        }

        public override IReadOnlyList<string> DescriptorNames
        {
            get
            {
                string name = "w";
                if (elementName.Equals("*"))
                    name = "MW";
                else
                    name += elementName;
                return new string[] { name };
            }
        }

        private DescriptorValue<Result<double>> GetDummyDescriptorValue(Exception e)
        {
            return new DescriptorValue<Result<double>>(_Specification, ParameterNames, Parameters, new Result<double>(double.NaN), DescriptorNames, e);
        }

        /// <summary>
        /// Calculate the weight of specified element type in the supplied <see cref="IAtomContainer"/>.
        /// </summary>
        /// <param name="container">The AtomContainer for which this descriptor is to be calculated. If 'H'
        /// is specified as the element symbol make sure that the AtomContainer has hydrogens.</param>
        /// <returns>The total weight of atoms of the specified element type</returns>
        public DescriptorValue<Result<double>> Calculate(IAtomContainer container)
        {
            double weight = 0;
            if (elementName.Equals("*"))
            {
                try
                {
                    for (int i = 0; i < container.Atoms.Count; i++)
                    {
                        //Debug.WriteLine("WEIGHT: "+container.GetAtomAt(i).Symbol +" " +IsotopeFactory.Instance.GetMajorIsotope( container.GetAtomAt(i).Symbol ).ExactMass);
                        weight += Isotopes.Instance.GetMajorIsotope(container.Atoms[i].Symbol).ExactMass.Value;
                        int hcount = container.Atoms[i].ImplicitHydrogenCount ?? 0;
                        weight += (hcount * 1.00782504);
                    }
                }
                catch (Exception e)
                {
                    return GetDummyDescriptorValue(e);
                }
            }
            else if (elementName.Equals("H"))
            {
                try
                {
                    IIsotope h = Isotopes.Instance.GetMajorIsotope("H");
                    for (int i = 0; i < container.Atoms.Count; i++)
                    {
                        if (container.Atoms[i].Symbol.Equals(elementName))
                        {
                            weight += Isotopes.Instance.GetMajorIsotope(container.Atoms[i].Symbol).ExactMass.Value;
                        }
                        else
                        {
                            weight += (container.Atoms[i].ImplicitHydrogenCount.Value * h.ExactMass.Value);
                        }
                    }
                }
                catch (Exception e)
                {
                    return GetDummyDescriptorValue(e);
                }
            }
            else
            {
                try
                {
                    for (int i = 0; i < container.Atoms.Count; i++)
                    {
                        if (container.Atoms[i].Symbol.Equals(elementName))
                        {
                            weight += Isotopes.Instance.GetMajorIsotope(container.Atoms[i].Symbol).ExactMass.Value;
                        }
                    }
                }
                catch (Exception e)
                {
                    return GetDummyDescriptorValue(e);
                }
            }

            return new DescriptorValue<Result<double>>(_Specification, ParameterNames, Parameters, new Result<double>(weight),
                    DescriptorNames);

        }

        /// <inheritdoc/>
        public override IDescriptorResult DescriptorResultType { get; } = new Result<double>(0.0);

        /// <summary>
        /// The parameterNames attribute of the WeightDescriptor object.
        /// </summary>
        public override IReadOnlyList<string> ParameterNames { get; } = new string[] { "elementSymbol" };

        /// <summary>
        ///  Gets the parameterType attribute of the WeightDescriptor object.
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>An Object whose class is that of the parameter requested</returns>
        public override object GetParameterType(string name) => "";

        IDescriptorValue IMolecularDescriptor.Calculate(IAtomContainer container) => Calculate(container);
    }
}
