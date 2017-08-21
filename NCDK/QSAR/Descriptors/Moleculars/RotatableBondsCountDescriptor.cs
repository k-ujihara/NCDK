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
using NCDK.Graphs;
using NCDK.QSAR.Results;
using NCDK.Tools.Manipulator;
using System.Linq;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    ///  The number of rotatable bonds is given by the SMARTS specified by Daylight on
    ///  <see href="http://www.daylight.com/dayhtml_tutorials/languages/smarts/smarts_examples.html#EXMPL">SMARTS tutorial</see>
    /// </summary>
    /// <remarks>
    /// <para>This descriptor uses these parameters:
    /// <list type="table">
    ///   <item>
    ///     <term>Name</term>
    ///     <term>Default</term>
    ///     <term>Description</term>
    ///   </item>
    ///   <item>
    ///     <term>includeTerminals</term>
    ///     <term>false</term>
    ///     <term>True if terminal bonds are included</term>
    ///   </item>
    ///   <item>
    ///     <term>excludeAmides</term>
    ///     <term>false</term>
    ///     <term>True if amide C-N bonds should be excluded</term>
    ///   </item>
    /// </list>
    /// </para>
    /// Returns a single value named <i>nRotB</i>
    /// </remarks>
    // @author      mfe4
    // @cdk.created 2004-11-03
    // @cdk.module  qsarmolecular
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:rotatableBondsCount
    // @cdk.keyword bond count, rotatable
    // @cdk.keyword descriptor
    public class RotatableBondsCountDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        private bool includeTerminals = false;
        private bool excludeAmides = false;

        /// <summary>
        ///  Constructor for the RotatableBondsCountDescriptor object
        /// </summary>
        public RotatableBondsCountDescriptor() { }

        /// <summary>
        /// The specification attribute of the RotatableBondsCountDescriptor object
        /// </summary>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#rotatableBondsCount",
                typeof(RotatableBondsCountDescriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the RotatableBondsCountDescriptor object
        /// </summary>
        /// <exception cref="CDKException"></exception>
        public override object[] Parameters
        {
            set
            {
                if (value.Length != 2)
                {
                    throw new CDKException("RotatableBondsCount expects two parameters");
                }
                if (!(value[0] is bool) || !(value[1] is bool))
                {
                    throw new CDKException("The parameters must be of type bool");
                }
                // ok, all should be fine
                includeTerminals = (bool)value[0];
                excludeAmides = (bool)value[1];
            }
            get
            {
                // return the parameters as used for the descriptor calculation
                return new object[] { includeTerminals, excludeAmides };
            }
        }

        public override string[] DescriptorNames => new string[] { includeTerminals ? "nRotBt" : "nRotB" };

        /// <summary>
        ///  The method calculates the number of rotatable bonds of an atom container.
        ///  If the bool parameter is set to true, terminal bonds are included.
        /// </summary>
        /// <param name="ac">AtomContainer</param>
        /// <returns>number of rotatable bonds</returns>
        public override DescriptorValue Calculate(IAtomContainer ac)
        {
            int rotatableBondsCount = 0;
            int degree0;
            int degree1;
            IRingSet ringSet;
            try
            {
                ringSet = new SpanningTree(ac).GetBasicRings();
            }
            catch (NoSuchAtomException e)
            {
                return new DescriptorValue(_Specification, ParameterNames, Parameters, new Result<int>(0), DescriptorNames, e);
            }
            foreach (var bond in ac.Bonds)
            {
                if (ringSet.GetRings(bond).Count() > 0)
                {
                    bond.IsInRing = true;
                }
            }
            foreach (var bond in ac.Bonds)
            {
                IAtom atom0 = bond.Atoms[0];
                IAtom atom1 = bond.Atoms[1];
                if (atom0.Symbol.Equals("H") || atom1.Symbol.Equals("H")) continue;
                if (bond.Order == BondOrder.Single)
                {
                    if ((BondManipulator.IsLowerOrder(ac.GetMaximumBondOrder(atom0), BondOrder.Triple))
                            && (BondManipulator.IsLowerOrder(ac.GetMaximumBondOrder(atom1), BondOrder.Triple)))
                    {
                        if (!bond.IsInRing)
                        {

                            if (excludeAmides && (IsAmide(atom0, atom1, ac) || IsAmide(atom1, atom0, ac)))
                            {
                                continue;
                            }

                            // if there are explicit H's we should ignore those bonds
                            degree0 = ac.GetConnectedBonds(atom0).Count() - GetConnectedHCount(ac, atom0);
                            degree1 = ac.GetConnectedBonds(atom1).Count() - GetConnectedHCount(ac, atom1);
                            if ((degree0 == 1) || (degree1 == 1))
                            {
                                if (includeTerminals)
                                {
                                    rotatableBondsCount += 1;
                                }
                            }
                            else
                            {
                                rotatableBondsCount += 1;
                            }
                        }
                    }
                }
            }
            return new DescriptorValue(_Specification, ParameterNames, Parameters, new Result<int>(rotatableBondsCount), DescriptorNames);
        }

        /// <summary>
        /// Checks whether both atoms are involved in an amide C-N bond: *N(*)C(*)=O.
        ///
        /// Only the most common constitution is considered. Tautomeric, O\C(*)=N\*,
        /// and charged forms, [O-]\C(*)=N\*, are ignored.
        ///
        /// <param name="atom0">the first bonding partner</param>
        /// <param name="atom1">the second bonding partner</param>
        /// <param name="ac">the parent container</param>
        ///
        /// <returns>if both partners are involved in an amide C-N bond</returns>
        /// </summary>
        private bool IsAmide(IAtom atom0, IAtom atom1, IAtomContainer ac)
        {

            if (atom0.Symbol.Equals("C") && atom1.Symbol.Equals("N"))
            {
                foreach (var neighbor in ac.GetConnectedAtoms(atom0))
                {
                    if (neighbor.Symbol.Equals("O")
                            && ac.GetBond(atom0, neighbor).Order == BondOrder.Double)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private int GetConnectedHCount(IAtomContainer atomContainer, IAtom atom)
        {
            var connectedAtoms = atomContainer.GetConnectedAtoms(atom);
            int n = 0;
            foreach (var anAtom in connectedAtoms)
                if (anAtom.Symbol.Equals("H")) n++;
            return n;
        }

        /// <inheritdoc/>
        public override IDescriptorResult DescriptorResultType { get; } = new Result<int>(1);

        /// <summary>
        /// The parameterNames attribute of the RotatableBondsCountDescriptor object
        /// </summary>
        public override string[] ParameterNames { get; } = new string[] { "includeTerminals", "excludeAmides" };

        /// <summary>
        ///  Gets the parameterType attribute of the RotatableBondsCountDescriptor object
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override object GetParameterType(string name) => true;
    }
}
