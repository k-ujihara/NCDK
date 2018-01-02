/* Copyright (C) 2004-2007  Matteo Floris <mfe4@users.sf.net>
 *                    2010  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Graphs.Matrix;
using NCDK.QSAR.Results;
using NCDK.Tools.Manipulator;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// This descriptor calculates the Wiener numbers. This includes the Wiener Path number
    /// and the Wiener Polarity Number.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Further information is given in
    /// Wiener path number: half the sum of all the distance matrix entries; Wiener
    /// polarity number: half the sum of all the distance matrix entries with a
    /// value of 3. For more information see <token>cdk-cite-Wiener1947</token>; <token>cdk-cite-TOD2000</token>.
    /// </para>
    /// <para>
    /// This descriptor uses no parameters.
    /// </para>
    /// <para>
    /// This descriptor works properly with AtomContainers whose atoms contain <b>implicit hydrogens</b>
    /// or <b>explicit hydrogens</b>.
    /// </para>
    /// <para>
    /// Returns the following values
    /// <list type="bullet">
    /// <item>WPATH - weiner path number</item>
    /// <item>WPOL - weiner polarity number</item>
    /// </list>
    /// </para>
    /// <para>This descriptor does not have any parameters.</para>
    /// </remarks>
    // @author         mfe4
    // @cdk.created        December 7, 2004
    // @cdk.created    2004-11-03
    // @cdk.module     qsarmolecular
    // @cdk.githash
    // @cdk.dictref    qsar-descriptors:wienerNumbers
    // @cdk.keyword    Wiener number
    public class WienerNumbersDescriptor : IMolecularDescriptor
    {
        private static readonly string[] NAMES = { "WPATH", "WPOL" };

        double[][] matr = null;
        ArrayResult<double> wienerNumbers = null;
        ConnectionMatrix connectionMatrix = new ConnectionMatrix();
        AtomContainerManipulator atm = new AtomContainerManipulator();

        /// <summary>
        ///  Constructor for the WienerNumbersDescriptor object.
        /// </summary>
        public WienerNumbersDescriptor()
        { }

        /// <inheritdoc/>
        public IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#wienerNumbers",
                typeof(WienerNumbersDescriptor).FullName,
                "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the WienerNumbersDescriptor object.
        /// </summary>
        public object[] Parameters { get { return null; } set { } }

        public IReadOnlyList<string> DescriptorNames => NAMES;

        /// <summary>
        /// Calculate the Wiener numbers.
        /// </summary>
        /// <param name="atomContainer">The <see cref="IAtomContainer"/> for which this descriptor is to be calculated</param>
        /// <returns>wiener numbers as array of 2 doubles</returns>
        public DescriptorValue<ArrayResult<double>> Calculate(IAtomContainer atomContainer)
        {
            wienerNumbers = new ArrayResult<double>(2);
            double wienerPathNumber = 0; //wienerPath
            double wienerPolarityNumber = 0; //wienerPol

            matr = ConnectionMatrix.GetMatrix(AtomContainerManipulator.RemoveHydrogens(atomContainer));
            int[][] distances = PathTools.ComputeFloydAPSP(matr);

            int partial;
            for (int i = 0; i < distances.Length; i++)
            {
                for (int j = 0; j < distances.Length; j++)
                {
                    partial = distances[i][j];
                    wienerPathNumber += partial;
                    if (partial == 3)
                    {
                        wienerPolarityNumber += 1;
                    }
                }
            }
            wienerPathNumber = wienerPathNumber / 2;
            wienerPolarityNumber = wienerPolarityNumber / 2;

            wienerNumbers.Add(wienerPathNumber);
            wienerNumbers.Add(wienerPolarityNumber);
            return new DescriptorValue<ArrayResult<double>>(_Specification, ParameterNames, Parameters, wienerNumbers,
                    DescriptorNames);
        }

        /// <summary>
        /// Returns the specific type of the DescriptorResult object.
        /// <para>
        /// The return value from this method really indicates what type of result will
        /// be obtained from the <see cref="DescriptorValue"/> object. Note that the same result
        /// can be achieved by interrogating the <see cref="DescriptorValue"/> object; this method
        /// allows you to do the same thing, without actually calculating the descriptor.
        /// </para>
        /// </summary>
        public IDescriptorResult DescriptorResultType { get; } = new ArrayResult<double>(2);

        /// <summary>
        /// The parameterNames attribute of the WienerNumbersDescriptor object.
        /// This descriptor does not return any parameters
        /// </summary>
        public IReadOnlyList<string> ParameterNames => null;

        /// <summary>
        /// Gets the parameterType attribute of the WienerNumbersDescriptor object.
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>An Object of class equal to that of the parameter being requested</returns>
        public object GetParameterType(string name) => null;

        IDescriptorValue IMolecularDescriptor.Calculate(IAtomContainer container) => Calculate(container);
    }
}
