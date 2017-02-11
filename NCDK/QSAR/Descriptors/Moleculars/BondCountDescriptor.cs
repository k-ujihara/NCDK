/* Copyright (C) 2004-2007  Matteo Floris <mfe4@users.sf.net>
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
using NCDK.QSAR.Result;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /**
     *  IDescriptor based on the number of bonds of a certain bond order.
     *
     * <p>This descriptor uses these parameters:
     * <table border="1">
     *   <tr>
     *     <td>Name</td>
     *     <td>Default</td>
     *     <td>Description</td>
     *   </tr>
     *   <tr>
     *     <td>order</td>
     *     <td>""</td>
     *     <td>The bond order</td>
     *   </tr>
     * </table>
     *
     * Returns a single value with name <i>nBX</i> where <i>X</i> can be
     * <ul>
     * <li>s for single bonds
     * <li>d for double bonds
     * <li>t for triple bonds
     * <li>a for aromatic bonds
     * <li>"" for all bonds
     * </ul>
     *
     * Note that the descriptor does not consider bonds to H's.
     *
     * @author      mfe4
     * @cdk.created 2004-11-13
     * @cdk.module  qsarmolecular
     * @cdk.githash
     * @cdk.set     qsar-descriptors
     * @cdk.dictref qsar-descriptors:bondCount
     */
    public class BondCountDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {

        /// <summary> defaults to UNSET, which means: count all bonds **/
        private string order = "";

        /// <summary>
        ///  Constructor for the BondCountDescriptor object
        /// </summary>
        public BondCountDescriptor() { }

        /// <summary>
        ///  The specification attribute of the BondCountDescriptor object
        /// </summary>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
         new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#bondCount",
                typeof(BondCountDescriptor).FullName,
                "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the BondCountDescriptor object
        /// </summary>
        /// <exception cref="CDKException">Description of the Exception</exception>
		public override object[] Parameters
        {
            set
            {
                if (value.Length > 1)
                {
                    throw new CDKException("BondCount only expects one parameter");
                }
                if (!(value[0] is string))
                {
                    throw new CDKException("The parameter must be of type BondOrder");
                }
                string bondType = (string)value[0];
                if (bondType.Length > 1 || !"sdtq".Contains(bondType))
                {
                    throw new CDKException("The only allowed values for this parameter are 's', 'd', 't', 'q' and ''.");
                }
                // ok, all should be fine
                order = bondType;
            }
            get
            {
                // return the parameters as used for the descriptor calculation
                return new object[] { order };
            }
        }

        public override string[] DescriptorNames
        {
            get
            {
                if (order.Equals(""))
                    return new string[] { "nB" };
                else
                    return new string[] { "nB" + order };
            }
        }

        /// <summary>
        ///  This method calculate the number of bonds of a given type in an atomContainer
        /// </summary>
        /// <param name="container">AtomContainer</param>
        /// <returns>The number of bonds of a certain type.</returns>
        public override DescriptorValue Calculate(IAtomContainer container)
        {
            if (order.Equals(""))
            {
                int bondCount = 0;
                foreach (var bond in container.Bonds)
                {
                    bool hasHydrogen = false;
                    for (int i = 0; i < bond.Atoms.Count; i++)
                    {
                        if (bond.Atoms[i].Symbol.Equals("H"))
                        {
                            hasHydrogen = true;
                            break;
                        }
                    }
                    if (!hasHydrogen) bondCount++;
                }
                return new DescriptorValue(_Specification, ParameterNames, Parameters, new IntegerResult(bondCount), DescriptorNames, null);
            }
			else
            {
                int bondCount = 0;
                foreach (var bond in container.Bonds)
                {
                    if (BondMatch(bond.Order, order))
                    {
                        bondCount += 1;
                    }
                }
                return new DescriptorValue(_Specification, ParameterNames, Parameters, new IntegerResult(bondCount), DescriptorNames);
            }
        }

        private bool BondMatch(BondOrder order, string orderString)
        {
            if (order == BondOrder.Single && "s".Equals(orderString))
                return true;
            else if (order == BondOrder.Double && "d".Equals(orderString))
                return true;
            else if (order == BondOrder.Triple && "t".Equals(orderString))
                return true;
            else
                return (order == BondOrder.Quadruple && "q".Equals(orderString));
        }

        /// <summary>
        /// The specific type of the DescriptorResult object.
        /// <para>
        /// The return value from this method really indicates what type of result will
        /// be obtained from the <see cref="DescriptorValue"/> object. Note that the same result
        /// can be achieved by interrogating the <see cref="DescriptorValue"/> object; this method
        /// allows you to do the same thing, without actually calculating the descriptor.
        /// </para>
        /// </summary>
        public override IDescriptorResult DescriptorResultType { get; } = new IntegerResult(1);

        /// <summary>
        /// The parameterNames attribute of the BondCountDescriptor object
        /// </summary>
        public override string[] ParameterNames { get; } = new string[] { "order" };

        /// <summary>
        ///  Gets the parameterType attribute of the BondCountDescriptor object
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>The parameterType value</returns>
        public override object GetParameterType(string name)
        {
            if ("order".Equals(name)) return "";
            return null;
        }
    }
}
