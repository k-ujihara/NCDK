/*
 *  Copyright (C) 2004-2007  Rajarshi Guha <rajarshi@users.sourceforge.net>
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
using NCDK.Charges;
using NCDK.Geometries;
using NCDK.Geometries.Surface;
using NCDK.QSAR.Result;
using System;
using System.Diagnostics;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Calculates 29 Charged Partial Surface Area (CPSA) descriptors.
    /// <p/>
    /// The CPSA's were developed by Stanton et al. ({@cdk.cite STA90}) and
    /// are related to the Polar Surface Area descriptors. The original
    /// implementation was in the ADAPT software package and the the definitions
    /// of the individual descriptors are presented in the following table. This class
    /// returns a <code>DoubleArrayResult</code> containing the 29 descriptors in the order
    /// described in the table.
    /// <table border=1 cellpadding=2>
    /// <caption><a name="cpsa">A Summary of the 29 CPSA Descriptors</a></caption>
    /// <thead>
    /// <tr>
    /// <th>IDescriptor</th><th>Meaning</th>
    /// </tr>
    /// </thead>
    /// <tbody>
    /// <tr>
    /// <td>PPSA-1</td><td> partial positive surface area -- sum of surface area on positive parts of molecule</td></tr><tr>
    /// <td>PPSA-2</td><td> partial positive surface area * total positive charge on the molecule </td></tr><tr>
    /// <td>PPSA-3</td><td> charge weighted partial positive surface area</td></tr><tr>
    /// <td>PNSA-1</td><td> partial negative surface area -- sum of surface area on negative parts of molecule</td></tr><tr>
    /// <td>PNSA-2</td><td> partial negative surface area * total negative charge on the molecule</td></tr><tr>
    /// <td>PNSA-3</td><td> charge weighted partial negative surface area</td></tr><tr>
    /// <td>    DPSA-1</td><td> difference of PPSA-1 and PNSA-1</td></tr><tr>
    /// <td>    DPSA-2</td><td> difference of FPSA-2 and PNSA-2</td></tr><tr>
    /// <td>    DPSA-3</td><td> difference of PPSA-3 and PNSA-3</td></tr><tr>
    /// <td>    FPSA-1</td><td> PPSA-1 / total molecular surface area</td></tr><tr>
    /// <td>    FFSA-2  </td><td>PPSA-2 / total molecular surface area</td></tr><tr>
    /// <td>    FPSA-3</td><td> PPSA-3 / total molecular surface area</td></tr><tr>
    /// <td>    FNSA-1</td><td> PNSA-1 / total molecular surface area</td></tr><tr>
    /// <td>    FNSA-2</td><td> PNSA-2 / total molecular surface area</td></tr><tr>
    /// <td>    FNSA-3</td><td> PNSA-3 / total molecular surface area</td></tr><tr>
    /// <td>    WPSA-1</td><td> PPSA-1 *  total molecular surface area / 1000</td></tr><tr>
    /// <td>WPSA-2</td><td>    PPSA-2 * total molecular surface area /1000</td></tr><tr>
    /// <td>WPSA-3</td><td>  PPSA-3 * total molecular surface area / 1000</td></tr><tr>
    /// <td>WNSA-1</td><td>  PNSA-1 *  total molecular surface area /1000</td></tr><tr>
    /// <td>WNSA-2</td><td> PNSA-2 * total molecular surface area / 1000</td></tr><tr>
    /// <td>WNSA-3</td><td> PNSA-3 * total molecular surface area / 1000</td></tr><tr>
    /// <td>RPCG</td><td> relative positive charge --  most positive charge / total positive charge</td></tr><tr>
    /// <td>    RNCG    </td><td>relative negative charge -- most negative charge / total negative charge</td></tr><tr>
    /// <td>    RPCS    </td><td>relative positive charge surface area -- most positive surface area * RPCG</td></tr><tr>
    /// <td>    RNCS    </td><td>relative negative charge surface area -- most negative surface area * RNCG</td></tr>
    /// <tr>
    /// <td>THSA</td>
    /// <td>sum of solvent accessible surface areas of
    /// atoms with absolute value of partial charges
    /// less than 0.2
    /// </td>
    /// </tr>
    /// <tr>
    /// <td>TPSA</td>
    /// <td>sum of solvent accessible surface areas of
    /// atoms with absolute value of partial charges
    /// greater than or equal 0.2
    /// </td>
    /// </tr>
    /// <tr>
    /// <td>RHSA</td>
    /// <td>THSA / total molecular surface area
    /// </td>
    /// </tr>
    /// <tr>
    /// <td>RPSA</td>
    /// <td>TPSA / total molecular  surface area
    /// </td>
    /// </tr>
    /// </tbody>
    /// </table>
    /// <p/>
    /// <b>NOTE</b>: The values calculated by this implementation will differ from those
    /// calculated by the original ADAPT implementation of the CPSA descriptors. This
    /// is because the original implementation used an analytical surface area algorithm
    /// and used partial charges obtained from MOPAC using the AM1 Hamiltonian.
    /// This implementation uses a numerical
    /// algorithm to obtain surface areas (see <see cref="NumericalSurface"/>) and obtains partial
    /// charges using the Gasteiger-Marsilli algorithm (see <see cref="GasteigerMarsiliPartialCharges"/>).
    /// <p/>
    /// However, a comparison of the values calculated by the two implementations indicates
    /// that they are qualitatively the same.
    /// <p/>
    /// <p>This descriptor uses these parameters:
    /// <table border="1">
    /// <tr>
    /// <td>Name</td>
    /// <td>Default</td>
    /// <td>Description</td>
    /// </tr>
    /// <tr>
    /// <td></td>
    /// <td></td>
    /// <td>no parameters</td>
    /// </tr>
    /// </table>
    ///
    // @author Rajarshi Guha
    // @cdk.created 2005-05-16
    // @cdk.module qsarmolecular
    // @cdk.githash
    // @cdk.set qsar-descriptors
    // @cdk.dictref qsar-descriptors:CPSA
    /// </summary>
    public class CPSADescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        private static readonly string[] NAMES = {"PPSA-1", "PPSA-2", "PPSA-3", "PNSA-1", "PNSA-2", "PNSA-3", "DPSA-1",
            "DPSA-2", "DPSA-3", "FPSA-1", "FPSA-2", "FPSA-3", "FNSA-1", "FNSA-2", "FNSA-3", "WPSA-1", "WPSA-2",
            "WPSA-3", "WNSA-1", "WNSA-2", "WNSA-3", "RPCG", "RNCG", "RPCS", "RNCS", "THSA", "TPSA", "RHSA", "RPSA"};

        public CPSADescriptor() { }

        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
         new DescriptorSpecification(
             "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#CPSA",
             typeof(CPSADescriptor).FullName,
             "The Chemistry Development Kit");

        /// <summary>
        /// The parameterNames attribute of the CPSADescriptor object.
        /// </summary>
        public override string[] ParameterNames => null;
        public override string[] DescriptorNames => NAMES;
        /// <summary>
        /// The parameters attribute of the CPSADescriptor object.
        /// </summary>
        public override object[] Parameters { get { return null; } set { } }

        /// <summary>
        /// Gets the parameterType attribute of the CPSADescriptor object.
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>The parameterType value</returns>
        public override object GetParameterType(string name) => null;

        /// <summary>
        /// Evaluates the 29 CPSA descriptors using Gasteiger-Marsilli charges.
        /// </summary>
        /// <param name="atomContainer">Parameter is the atom container.</param>
        /// <returns>An ArrayList containing 29 elements in the order described above</returns>
        public override DescriptorValue Calculate(IAtomContainer atomContainer)
        {
            DoubleArrayResult retval = new DoubleArrayResult();

            if (!GeometryUtil.Has3DCoordinates(atomContainer))
            {
                for (int i = 0; i < 29; i++)
                    retval.Add(double.NaN);
                return new DescriptorValue(_Specification, ParameterNames, Parameters, retval,
                        DescriptorNames, new CDKException("Molecule must have 3D coordinates"));
            }

            IAtomContainer container;
            container = (IAtomContainer)atomContainer.Clone();

            //        IsotopeFactory factory = null;
            //        try {
            //            factory = IsotopeFactory.GetInstance(container.GetNewBuilder());
            //        } catch (Exception e) {
            //            Debug.WriteLine(e);
            //        }

            GasteigerMarsiliPartialCharges peoe;
            try
            {
                peoe = new GasteigerMarsiliPartialCharges();
                peoe.AssignGasteigerMarsiliSigmaPartialCharges(container, true);
            }
            catch (Exception)
            {
                Debug.WriteLine("Error in assigning Gasteiger-Marsilli charges");
                for (int i = 0; i < 29; i++)
                    retval.Add(double.NaN);
                return new DescriptorValue(_Specification, ParameterNames, Parameters, retval,
                        DescriptorNames, new CDKException("Error in getting G-M charges"));
            }

            NumericalSurface surface;
            try
            {
                surface = new NumericalSurface(container);
                surface.CalculateSurface();
            }
            catch (NullReferenceException)
            {
                Debug.WriteLine("Error in surface area calculation");
                for (int i = 0; i < 29; i++)
                    retval.Add(double.NaN);
                return new DescriptorValue(_Specification, ParameterNames, Parameters, retval,
                        DescriptorNames, new CDKException("Error in surface area calculation"));
            }

            //double molecularWeight = mfa.GetMass();
            double[] atomSurfaces = surface.GetAllSurfaceAreas();
            double totalSA = surface.GetTotalSurfaceArea();

            double ppsa1 = 0.0;
            double ppsa3 = 0.0;
            double pnsa1 = 0.0;
            double pnsa3 = 0.0;
            double totpcharge = 0.0;
            double totncharge = 0.0;
            for (int i = 0; i < container.Atoms.Count; i++)
            {
                if (container.Atoms[i].Charge > 0)
                {
                    ppsa1 += atomSurfaces[i];
                    ppsa3 += container.Atoms[i].Charge.Value * atomSurfaces[i];
                    totpcharge += container.Atoms[i].Charge.Value;
                }
                else
                {
                    pnsa1 += atomSurfaces[i];
                    pnsa3 += container.Atoms[i].Charge.Value * atomSurfaces[i];
                    totncharge += container.Atoms[i].Charge.Value;
                }
            }

            double ppsa2 = ppsa1 * totpcharge;
            double pnsa2 = pnsa1 * totncharge;

            // fractional +ve & -ve SA
            double fpsa1 = ppsa1 / totalSA;
            double fpsa2 = ppsa2 / totalSA;
            double fpsa3 = ppsa3 / totalSA;
            double fnsa1 = pnsa1 / totalSA;
            double fnsa2 = pnsa2 / totalSA;
            double fnsa3 = pnsa3 / totalSA;

            // surface wtd +ve & -ve SA
            double wpsa1 = ppsa1 * totalSA / 1000;
            double wpsa2 = ppsa2 * totalSA / 1000;
            double wpsa3 = ppsa3 * totalSA / 1000;
            double wnsa1 = pnsa1 * totalSA / 1000;
            double wnsa2 = pnsa2 * totalSA / 1000;
            double wnsa3 = pnsa3 * totalSA / 1000;

            // hydrophobic and poalr surface area
            double phobic = 0.0;
            double polar = 0.0;
            for (int i = 0; i < container.Atoms.Count; i++)
            {
                if (Math.Abs(container.Atoms[i].Charge.Value) < 0.2)
                {
                    phobic += atomSurfaces[i];
                }
                else
                {
                    polar += atomSurfaces[i];
                }
            }
            double thsa = phobic;
            double tpsa = polar;
            double rhsa = phobic / totalSA;
            double rpsa = polar / totalSA;

            // differential +ve & -ve SA
            double dpsa1 = ppsa1 - pnsa1;
            double dpsa2 = ppsa2 - pnsa2;
            double dpsa3 = ppsa3 - pnsa3;

            double maxpcharge = 0.0;
            double maxncharge = 0.0;
            int pidx = 0;
            int nidx = 0;
            for (int i = 0; i < container.Atoms.Count; i++)
            {
                double charge = container.Atoms[i].Charge.Value;
                if (charge > maxpcharge)
                {
                    maxpcharge = charge;
                    pidx = i;
                }
                if (charge < maxncharge)
                {
                    maxncharge = charge;
                    nidx = i;
                }
            }

            // relative descriptors
            double rpcg = maxpcharge / totpcharge;
            double rncg = maxncharge / totncharge;
            double rpcs = atomSurfaces[pidx] * rpcg;
            double rncs = atomSurfaces[nidx] * rncg;

            // fill in the values
            retval.Add(ppsa1);
            retval.Add(ppsa2);
            retval.Add(ppsa3);
            retval.Add(pnsa1);
            retval.Add(pnsa2);
            retval.Add(pnsa3);

            retval.Add(dpsa1);
            retval.Add(dpsa2);
            retval.Add(dpsa3);

            retval.Add(fpsa1);
            retval.Add(fpsa2);
            retval.Add(fpsa3);
            retval.Add(fnsa1);
            retval.Add(fnsa2);
            retval.Add(fnsa3);

            retval.Add(wpsa1);
            retval.Add(wpsa2);
            retval.Add(wpsa3);
            retval.Add(wnsa1);
            retval.Add(wnsa2);
            retval.Add(wnsa3);

            retval.Add(rpcg);
            retval.Add(rncg);
            retval.Add(rpcs);
            retval.Add(rncs);

            retval.Add(thsa);
            retval.Add(tpsa);
            retval.Add(rhsa);
            retval.Add(rpsa);

            return new DescriptorValue(_Specification, ParameterNames, Parameters, retval, DescriptorNames);
        }

        /// <summary>
        /// Returns the specific type of the DescriptorResult object.
        /// <p/>
        /// The return value from this method really indicates what type of result will
        /// be obtained from the <see cref="DescriptorValue"/> object. Note that the same result
        /// can be achieved by interrogating the <see cref="DescriptorValue"/> object; this method
        /// allows you to do the same thing, without actually calculating the descriptor.
        ///
        /// <returns>an object that implements the <see cref="IDescriptorResult"/> interface indicating</returns>
        ///         the actual type of values returned by the descriptor in the <see cref="DescriptorValue"/> object
        /// </summary>
        public override IDescriptorResult DescriptorResultType { get; } = new DoubleArrayResultType(29);
    }
}
