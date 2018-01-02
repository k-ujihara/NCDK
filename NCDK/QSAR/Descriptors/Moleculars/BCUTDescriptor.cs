/* Copyright (C) 2004-2007  Rajarshi Guha <rajarshi@users.sourceforge.net>
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
using NCDK.Common.Collections;
using MathNet.Numerics.LinearAlgebra;
using NCDK.Aromaticities;
using NCDK.Charges;
using NCDK.Config;
using NCDK.Graphs;
using NCDK.Graphs.Matrix;
using NCDK.QSAR.Results;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System;
using System.Linq;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Eigenvalue based descriptor noted for its utility in chemical diversity.
    /// Described by Pearlman et al. <token>cdk-cite-PEA99</token>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The descriptor is based on a weighted version of the Burden matrix <token>cdk-cite-BUR89</token>; <token>cdk-cite-BUR97</token>
    /// which takes into account both the connectivity as well as atomic
    /// properties of a molecule. The weights are a variety of atom properties placed along the
    /// diagonal of the Burden matrix. Currently three weighting schemes are employed
    /// <list type="bullet">
    /// <item>atomic weight</item>
    /// <item>partial charge (Gasteiger Marsilli)</item>
    /// <item>polarizability <token>cdk-cite-KJ81</token></item>
    /// </list> 
    /// </para>
    /// <para>By default, the descriptor will return the highest and lowest eigenvalues for the three
    /// classes of descriptor in a single ArrayList (in the order shown above). However it is also
    /// possible to supply a parameter list indicating how many of the highest and lowest eigenvalues
    /// (for each class of descriptor) are required. The descriptor works with the hydrogen depleted molecule.
    /// </para>
    /// <para>
    /// A side effect of specifying the number of highest and lowest eigenvalues is that it is possible
    /// to get two copies of all the eigenvalues. That is, if a molecule has 5 heavy atoms, then specifying
    /// the 5 highest eigenvalues returns all of them, and specifying the 5 lowest eigenvalues returns
    /// all of them, resulting in two copies of all the eigenvalues.
    /// </para>
    /// <para>
    /// Note that it is possible to
    /// specify an arbitrarily large number of eigenvalues to be returned. However if the number
    /// (i.e., nhigh or nlow) is larger than the number of heavy atoms, the remaining eignevalues
    /// will be NaN.
    /// </para>
    /// <para>
    /// Given the above description, if the aim is to gt all the eigenvalues for a molecule, you should
    /// set nlow to 0 and specify the number of heavy atoms (or some large number) for nhigh (or vice versa).
    /// </para>
    /// <para>
    /// This descriptor uses these parameters:
    /// <list type="table">
    /// <item>
    /// <term>Name</term>
    /// <term>Default</term>
    /// <term>Description</term>
    /// </item>
    /// <item>
    /// <term>nhigh</term>
    /// <term>1</term>
    /// <term>The number of highest eigenvalue</term>
    /// </item>
    /// <item>
    /// <term>nlow</term>
    /// <term>1</term>
    /// <term>The number of lowest eigenvalue</term>
    /// </item>
    /// <item>
    /// <term>checkAromaticity</term>
    /// <term>true</term>
    /// <term>Whether aromaticity should be checked</term>
    /// </item>
    /// </list>
    /// </para>
    /// <para>
    /// Returns an array of values in the following order
    /// <list type="bullet">
    /// <item>BCUTw-1l, BCUTw-2l ... - <i>nhigh</i> lowest atom weighted BCUTS</item>
    /// <item>BCUTw-1h, BCUTw-2h ... - <i>nlow</i> highest atom weighted BCUTS</item>
    /// <item>BCUTc-1l, BCUTc-2l ... - <i>nhigh</i> lowest partial charge weighted BCUTS</item>
    /// <item>BCUTc-1h, BCUTc-2h ... - <i>nlow</i> highest partial charge weighted BCUTS</item>
    /// <item>BCUTp-1l, BCUTp-2l ... - <i>nhigh</i> lowest polarizability weighted BCUTS</item>
    /// <item>BCUTp-1h, BCUTp-2h ... - <i>nlow</i> highest polarizability weighted BCUTS</item>
    /// </list> 
    /// </para>
    /// </remarks>
    // @author Rajarshi Guha
    // @cdk.created 2004-11-30
    // @cdk.module qsarmolecular
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:BCUT
    // @cdk.keyword BCUT
    // @cdk.keyword descriptor
    public class BCUTDescriptor : IMolecularDescriptor
    {
        // the number of negative & positive eigenvalues
        // to return for each class of BCUT descriptor
        private int nhigh;
        private int nlow;
        private bool checkAromaticity;

        public BCUTDescriptor()
        {
            // set the default number of BCUT's
            this.nhigh = 1;
            this.nlow = 1;
            this.checkAromaticity = true;
        }

        public IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
         new DescriptorSpecification("http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#BCUT",
               typeof(BCUTDescriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the BCUTDescriptor object.
        /// Three element array of Integer and one bool representing number of highest and lowest eigenvalues and the checkAromaticity flag to return respectively
        /// The new parameter values. This descriptor takes 3 parameters: number of highest
        ///               eigenvalues and number of lowest eigenvalues. If 0 is specified for either (the default)
        ///               then all calculated eigenvalues are returned. The third parameter checkAromaticity is a bool.
        ///               If checkAromaticity is true, the method check the aromaticity, if false, means that the aromaticity has
        ///               already been checked.
        /// </summary>
        /// <exception cref="CDKException">if the parameters are of the wrong type</exception>
        /// <seealso cref="Parameters"/>
        public object[] Parameters
        {
            set
            {
                // we expect 3 parameters
                if (value.Length != 3)
                {
                    throw new CDKException("BCUTDescriptor requires 3 parameters");
                }
                if (!(value[0] is int) || !(value[1] is int))
                {
                    throw new CDKException("Parameters must be of type Integer");
                }
                else if (!(value[2] is bool))
                {
                    throw new CDKException("The third parameter must be of type bool");
                }
                // ok, all should be fine

                this.nhigh = (int)value[0];
                this.nlow = (int)value[1];
                this.checkAromaticity = (bool)value[2];

                if (this.nhigh < 0 || this.nlow < 0)
                {
                    throw new CDKException("Number of eigenvalues to return must be zero or more");
                }
            }
            get
            {
                return new object[]
                {
                    this.nhigh,
                    this.nlow,
                    this.checkAromaticity,
                };
            }
        }

        public IReadOnlyList<string> DescriptorNames
        {
            get
            {
                string[] names;
                string[] suffix = { "w", "c", "p" };
                names = new string[3 * nhigh + 3 * nlow];
                int counter = 0;
                foreach (var aSuffix in suffix)
                {
                    for (int i = 0; i < nhigh; i++)
                    {
                        names[counter++] = "BCUT" + aSuffix + "-" + (i + 1) + "l";
                    }
                    for (int i = 0; i < nlow; i++)
                    {
                        names[counter++] = "BCUT" + aSuffix + "-" + (i + 1) + "h";
                    }
                }
                return names;
            }
        }

        /// <summary>
        /// The parameterNames attribute of the BCUTDescriptor object.
        /// </summary>
        public IReadOnlyList<string> ParameterNames { get; } = new string[] { "nhigh", "nlow", "checkAromaticity" };

        /// <summary>
        /// Gets the parameterType attribute of the BCUTDescriptor object.
        /// </summary>
        /// <param name="name">Description of the Parameter (can be either 'nhigh' or 'nlow' or checkAromaticity)</param>
        /// <returns>The parameterType value</returns>
        public object GetParameterType(string name)
        {
            switch (name)
            {
                case "nhigh":
                case "nlow":
                    return 1;
                case "checkAromaticity":
                    return true;
                default:
                    return null;
            }
        }

        private bool HasUndefined(double[][] m)
        {
            foreach (var aM in m)
            {
                foreach (var mm in aM)
                {
                    if (double.IsNaN(mm) || double.IsInfinity(mm))
                        return true;
                }
            }
            return false;
        }

        private static class BurdenMatrix
        {
            public static double[][] EvalMatrix(IAtomContainer atomContainer, double[] vsd)
            {
                IAtomContainer local = AtomContainerManipulator.RemoveHydrogens(atomContainer);

                int natom = local.Atoms.Count;
                double[][] matrix = Arrays.CreateJagged<double>(natom, natom);
                for (int i = 0; i < natom; i++)
                {
                    for (int j = 0; j < natom; j++)
                    {
                        matrix[i][j] = 0.0;
                    }
                }

                /* set the off diagonal entries */
                for (int i = 0; i < natom - 1; i++)
                {
                    for (int j = i + 1; j < natom; j++)
                    {
                        for (int k = 0; k < local.Bonds.Count; k++)
                        {
                            IBond bond = local.Bonds[k];
                            if (bond.Contains(local.Atoms[i]) && bond.Contains(local.Atoms[j]))
                            {
                                if (bond.IsAromatic)
                                    matrix[i][j] = 0.15;
                                else if (bond.Order == BondOrder.Single)
                                    matrix[i][j] = 0.1;
                                else if (bond.Order == BondOrder.Double)
                                    matrix[i][j] = 0.2;
                                else if (bond.Order == BondOrder.Triple) matrix[i][j] = 0.3;

                                if (local.GetConnectedBonds(local.Atoms[i]).Count() == 1 || local.GetConnectedBonds(local.Atoms[j]).Count() == 1)
                                {
                                    matrix[i][j] += 0.01;
                                }
                                matrix[j][i] = matrix[i][j];
                            }
                            else
                            {
                                matrix[i][j] = 0.001;
                                matrix[j][i] = 0.001;
                            }
                        }
                    }
                }

                /* set the diagonal entries */
                for (int i = 0; i < natom; i++)
                {
                    if (vsd != null)
                        matrix[i][i] = vsd[i];
                    else
                        matrix[i][i] = 0.0;
                }
                return (matrix);
            }
        }

        /// <summary>
        /// Calculates the three classes of BCUT descriptors.
        /// </summary>
        /// <param name="container">Parameter is the atom container.</param>
        /// <returns>An ArrayList containing the descriptors. The default is to return
        ///         all calculated eigenvalues of the Burden matrices in the order described
        ///         above. If a parameter list was supplied, then only the specified number
        ///         of highest and lowest eigenvalues (for each class of BCUT) will be returned.
        ///         </returns>
        public DescriptorValue<ArrayResult<double>> Calculate(IAtomContainer container)
        {
            int counter;
            IAtomContainer molecule = (IAtomContainer)container.Clone();

            // add H's in case they're not present
            try
            {
                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
                CDKHydrogenAdder hAdder = CDKHydrogenAdder.GetInstance(molecule.Builder);
                hAdder.AddImplicitHydrogens(molecule);
                AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(molecule);
            }
            catch (Exception e)
            {
                return GetDummyDescriptorValue(new CDKException($"Could not add hydrogens: {e.Message}", e));
            }

            // do aromaticity detecttion for calculating polarizability later on
            if (this.checkAromaticity)
            {
                try
                {
                    AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
                }
                catch (CDKException e)
                {
                    return GetDummyDescriptorValue(new CDKException($"Error in atom typing: {e.Message}", e));
                }
                try
                {
                    Aromaticity.CDKLegacy.Apply(molecule);
                }
                catch (CDKException e)
                {
                    return GetDummyDescriptorValue(new CDKException($"Error in aromaticity perception: {e.Message}"));
                }
            }

            // find number of heavy atoms
            int nheavy = 0;
            for (int i = 0; i < molecule.Atoms.Count; i++)
            {
                if (!molecule.Atoms[i].Symbol.Equals("H")) nheavy++;
            }

            if (nheavy == 0) return GetDummyDescriptorValue(new CDKException("No heavy atoms in the molecule"));

            double[] diagvalue = new double[nheavy];

            // get atomic mass weighted BCUT
            counter = 0;
            try
            {
                for (int i = 0; i < molecule.Atoms.Count; i++)
                {
                    if (molecule.Atoms[i].Symbol.Equals("H")) continue;
                    diagvalue[counter] = Isotopes.Instance.GetMajorIsotope(molecule.Atoms[i].Symbol).ExactMass.Value;
                    counter++;
                }
            }
            catch (Exception e)
            {
                return GetDummyDescriptorValue(new CDKException($"Could not calculate weight: {e.Message}", e));
            }

            double[][] burdenMatrix = BurdenMatrix.EvalMatrix(molecule, diagvalue);
            if (HasUndefined(burdenMatrix))
                return GetDummyDescriptorValue(new CDKException("Burden matrix has undefined values"));
            Matrix<double> matrix;
            matrix = Matrix<double>.Build.DenseOfColumnArrays(burdenMatrix);
            var eigenDecomposition = matrix.Evd().EigenValues;
            double[] eval1 = eigenDecomposition.Select(n => n.Real).ToArray();

            // get charge weighted BCUT
            LonePairElectronChecker lpcheck = new LonePairElectronChecker();
            GasteigerMarsiliPartialCharges peoe;
            try
            {
                lpcheck.Saturate(molecule);
                double[] charges = new double[molecule.Atoms.Count];
                //            pepe = new GasteigerPEPEPartialCharges();
                //            pepe.CalculateCharges(molecule);
                //            for (int i = 0; i < molecule.Atoms.Count; i++) charges[i] = molecule.Atoms[i].Charge;
                peoe = new GasteigerMarsiliPartialCharges();
                peoe.AssignGasteigerMarsiliSigmaPartialCharges(molecule, true);
                for (int i = 0; i < molecule.Atoms.Count; i++)
                    charges[i] += molecule.Atoms[i].Charge.Value;
                for (int i = 0; i < molecule.Atoms.Count; i++)
                {
                    molecule.Atoms[i].Charge = charges[i];
                }
            }
            catch (Exception e)
            {
                return GetDummyDescriptorValue(new CDKException("Could not calculate partial charges: " + e.Message, e));
            }
            counter = 0;
            for (int i = 0; i < molecule.Atoms.Count; i++)
            {
                if (molecule.Atoms[i].Symbol.Equals("H")) continue;
                diagvalue[counter] = molecule.Atoms[i].Charge.Value;
                counter++;
            }
            burdenMatrix = BurdenMatrix.EvalMatrix(molecule, diagvalue);
            if (HasUndefined(burdenMatrix))
                return GetDummyDescriptorValue(new CDKException("Burden matrix has undefined values"));
            matrix = Matrix<double>.Build.DenseOfColumnArrays(burdenMatrix);
            eigenDecomposition = matrix.Evd().EigenValues;
            double[] eval2 = eigenDecomposition.Select(n => n.Real).ToArray();

            int[][] topoDistance = PathTools.ComputeFloydAPSP(AdjacencyMatrix.GetMatrix(molecule));

            // get polarizability weighted BCUT
            Polarizability pol = new Polarizability();
            counter = 0;
            for (int i = 0; i < molecule.Atoms.Count; i++)
            {
                if (molecule.Atoms[i].Symbol.Equals("H")) continue;
                diagvalue[counter] = pol.CalculateGHEffectiveAtomPolarizability(molecule, molecule.Atoms[i], false,
                        topoDistance);
                counter++;
            }
            burdenMatrix = BurdenMatrix.EvalMatrix(molecule, diagvalue);
            if (HasUndefined(burdenMatrix))
                return GetDummyDescriptorValue(new CDKException("Burden matrix has undefined values"));
            matrix = Matrix<double>.Build.DenseOfColumnArrays(burdenMatrix);
            eigenDecomposition = matrix.Evd().EigenValues;
            double[] eval3 = eigenDecomposition.Select(n => n.Real).ToArray();

            string[] names;
            string[] suffix = { "w", "c", "p" };

            // return only the n highest & lowest eigenvalues
            int lnlow, lnhigh, enlow, enhigh;
            if (nlow > nheavy)
            {
                lnlow = nheavy;
                enlow = nlow - nheavy;
            }
            else
            {
                lnlow = nlow;
                enlow = 0;
            }

            if (nhigh > nheavy)
            {
                lnhigh = nheavy;
                enhigh = nhigh - nheavy;
            }
            else
            {
                lnhigh = nhigh;
                enhigh = 0;
            }

            ArrayResult<double> retval = new ArrayResult<double>((lnlow + enlow + lnhigh + enhigh) * 3);

            for (int i = 0; i < lnlow; i++)
                retval.Add(eval1[i]);
            for (int i = 0; i < enlow; i++)
                retval.Add(double.NaN);
            for (int i = 0; i < lnhigh; i++)
                retval.Add(eval1[eval1.Length - i - 1]);
            for (int i = 0; i < enhigh; i++)
                retval.Add(double.NaN);

            for (int i = 0; i < lnlow; i++)
                retval.Add(eval2[i]);
            for (int i = 0; i < enlow; i++)
                retval.Add(double.NaN);
            for (int i = 0; i < lnhigh; i++)
                retval.Add(eval2[eval2.Length - i - 1]);
            for (int i = 0; i < enhigh; i++)
                retval.Add(double.NaN);

            for (int i = 0; i < lnlow; i++)
                retval.Add(eval3[i]);
            for (int i = 0; i < enlow; i++)
                retval.Add(double.NaN);
            for (int i = 0; i < lnhigh; i++)
                retval.Add(eval3[eval3.Length - i - 1]);
            for (int i = 0; i < enhigh; i++)
                retval.Add(double.NaN);

            names = new string[3 * nhigh + 3 * nlow];
            counter = 0;
            foreach (var aSuffix in suffix)
            {
                for (int i = 0; i < nhigh; i++)
                {
                    names[counter++] = "BCUT" + aSuffix + "-" + (i + 1) + "l";
                }
                for (int i = 0; i < nlow; i++)
                {
                    names[counter++] = "BCUT" + aSuffix + "-" + (i + 1) + "h";
                }
            }

            return new DescriptorValue<ArrayResult<double>>(_Specification, ParameterNames, Parameters, retval,
                    DescriptorNames);
        }

        /// <inheritdoc/>
        public IDescriptorResult DescriptorResultType { get; } = new ArrayResult<double>(6);

        private DescriptorValue<ArrayResult<double>> GetDummyDescriptorValue(Exception e)
        {
            ArrayResult<double> results = new ArrayResult<double>(6);
            for (int i = 0; i < 6; i++)
                results.Add(double.NaN);
            return new DescriptorValue<ArrayResult<double>>(_Specification, ParameterNames, Parameters, results,
                    DescriptorNames, e);
        }

        IDescriptorValue IMolecularDescriptor.Calculate(IAtomContainer container) => Calculate(container);
    }
}
