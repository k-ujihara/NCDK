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
using NCDK.QSAR.Results;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Calculates 29 Charged Partial Surface Area (CPSA) descriptors.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The CPSA's were developed by Stanton et al. (<token>cdk-cite-STA90</token>) and
    /// are related to the Polar Surface Area descriptors. The original
    /// implementation was in the ADAPT software package and the definitions
    /// of the individual descriptors are presented in the following table. This class
    /// returns a <see cref="ArrayResult{Double}"/> containing the 29 descriptors in the order
    /// described in the table.
    /// </para>
    /// <para>
    /// <a name="cpsa">A Summary of the 29 CPSA Descriptors</a>
    /// <list type="table"> 
    /// <listheader><term>IDescriptor</term><term>Meaning</term></listheader>
    /// <item>
    /// <term>PPSA-1</term><term> partial positive surface area -- sum of surface area on positive parts of molecule</term></item><item>
    /// <term>PPSA-2</term><term> partial positive surface area * total positive charge on the molecule </term></item><item>
    /// <term>PPSA-3</term><term> charge weighted partial positive surface area</term></item><item>
    /// <term>PNSA-1</term><term> partial negative surface area -- sum of surface area on negative parts of molecule</term></item><item>
    /// <term>PNSA-2</term><term> partial negative surface area * total negative charge on the molecule</term></item><item>
    /// <term>PNSA-3</term><term> charge weighted partial negative surface area</term></item><item>
    /// <term>    DPSA-1</term><term> difference of PPSA-1 and PNSA-1</term></item><item>
    /// <term>    DPSA-2</term><term> difference of FPSA-2 and PNSA-2</term></item><item>
    /// <term>    DPSA-3</term><term> difference of PPSA-3 and PNSA-3</term></item><item>
    /// <term>    FPSA-1</term><term> PPSA-1 / total molecular surface area</term></item><item>
    /// <term>    FFSA-2  </term><term>PPSA-2 / total molecular surface area</term></item><item>
    /// <term>    FPSA-3</term><term> PPSA-3 / total molecular surface area</term></item><item>
    /// <term>    FNSA-1</term><term> PNSA-1 / total molecular surface area</term></item><item>
    /// <term>    FNSA-2</term><term> PNSA-2 / total molecular surface area</term></item><item>
    /// <term>    FNSA-3</term><term> PNSA-3 / total molecular surface area</term></item><item>
    /// <term>    WPSA-1</term><term> PPSA-1 *  total molecular surface area / 1000</term></item><item>
    /// <term>WPSA-2</term><term>    PPSA-2 * total molecular surface area /1000</term></item><item>
    /// <term>WPSA-3</term><term>  PPSA-3 * total molecular surface area / 1000</term></item><item>
    /// <term>WNSA-1</term><term>  PNSA-1 *  total molecular surface area /1000</term></item><item>
    /// <term>WNSA-2</term><term> PNSA-2 * total molecular surface area / 1000</term></item><item>
    /// <term>WNSA-3</term><term> PNSA-3 * total molecular surface area / 1000</term></item><item>
    /// <term>RPCG</term><term> relative positive charge --  most positive charge / total positive charge</term></item><item>
    /// <term>    RNCG    </term><term>relative negative charge -- most negative charge / total negative charge</term></item><item>
    /// <term>    RPCS    </term><term>relative positive charge surface area -- most positive surface area * RPCG</term></item><item>
    /// <term>    RNCS    </term><term>relative negative charge surface area -- most negative surface area * RNCG</term></item>
    /// <item>
    /// <term>THSA</term>
    /// <term>sum of solvent accessible surface areas of
    /// atoms with absolute value of partial charges
    /// less than 0.2
    /// </term>
    /// </item>
    /// <item>
    /// <term>TPSA</term>
    /// <term>sum of solvent accessible surface areas of
    /// atoms with absolute value of partial charges
    /// greater than or equal 0.2
    /// </term>
    /// </item>
    /// <item>
    /// <term>RHSA</term>
    /// <term>THSA / total molecular surface area
    /// </term>
    /// </item>
    /// <item>
    /// <term>RPSA</term>
    /// <term>TPSA / total molecular surface area
    /// </term>
    /// </item>
    /// </list>
    /// </para>
    /// <para>
    /// <b>NOTE</b>: The values calculated by this implementation will differ from those
    /// calculated by the original ADAPT implementation of the CPSA descriptors. This
    /// is because the original implementation used an analytical surface area algorithm
    /// and used partial charges obtained from MOPAC using the AM1 Hamiltonian.
    /// This implementation uses a numerical
    /// algorithm to obtain surface areas (see <see cref="NumericalSurface"/>) and obtains partial
    /// charges using the Gasteiger-Marsilli algorithm (see <see cref="GasteigerMarsiliPartialCharges"/>).
    /// </para>
    /// <para>
    /// However, a comparison of the values calculated by the two implementations indicates
    /// that they are qualitatively the same.
    /// </para>
    /// <para>This descriptor uses these parameters:
    /// <list type="table">
    /// <listheader><term>Name</term><term>Default</term><term>Description</term></listheader>
    /// <item><term></term><term></term><term>no parameters</term></item>
    /// </list>
    /// </para>
    /// </remarks>
    // @author Rajarshi Guha
    // @cdk.created 2005-05-16
    // @cdk.module qsarmolecular
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:CPSA
    public class CPSADescriptor : IMolecularDescriptor
    {
        private static readonly string[] NAMES = {"PPSA-1", "PPSA-2", "PPSA-3", "PNSA-1", "PNSA-2", "PNSA-3", "DPSA-1",
            "DPSA-2", "DPSA-3", "FPSA-1", "FPSA-2", "FPSA-3", "FNSA-1", "FNSA-2", "FNSA-3", "WPSA-1", "WPSA-2",
            "WPSA-3", "WNSA-1", "WNSA-2", "WNSA-3", "RPCG", "RNCG", "RPCS", "RNCS", "THSA", "TPSA", "RHSA", "RPSA"};

        public CPSADescriptor() { }

        public IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
         new DescriptorSpecification(
             "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#CPSA",
             typeof(CPSADescriptor).FullName,
             "The Chemistry Development Kit");

        /// <summary>
        /// The parameterNames attribute of the CPSADescriptor object.
        /// </summary>
        public IReadOnlyList<string> ParameterNames => null;
        public IReadOnlyList<string> DescriptorNames => NAMES;
        /// <summary>
        /// The parameters attribute of the CPSADescriptor object.
        /// </summary>
        public object[] Parameters { get { return null; } set { } }

        /// <summary>
        /// Gets the parameterType attribute of the CPSADescriptor object.
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>The parameterType value</returns>
        public object GetParameterType(string name) => null;

        /// <summary>
        /// Evaluates the 29 CPSA descriptors using Gasteiger-Marsilli charges.
        /// </summary>
        /// <param name="atomContainer">Parameter is the atom container.</param>
        /// <returns>An ArrayList containing 29 elements in the order described above</returns>
        public DescriptorValue<ArrayResult<double>> Calculate(IAtomContainer atomContainer)
        {
            ArrayResult<double> retval = new ArrayResult<double>();

            if (!GeometryUtil.Has3DCoordinates(atomContainer))
            {
                for (int i = 0; i < 29; i++)
                    retval.Add(double.NaN);
                return new DescriptorValue<ArrayResult<double>>(_Specification, ParameterNames, Parameters, retval,
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
                return new DescriptorValue<ArrayResult<double>>(_Specification, ParameterNames, Parameters, retval,
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
                return new DescriptorValue<ArrayResult<double>>(_Specification, ParameterNames, Parameters, retval,
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

            return new DescriptorValue<ArrayResult<double>>(_Specification, ParameterNames, Parameters, retval, DescriptorNames);
        }

        /// <inheritdoc/>
        public IDescriptorResult DescriptorResultType { get; } = new ArrayResult<double>(29);

        IDescriptorValue IMolecularDescriptor.Calculate(IAtomContainer container) => Calculate(container);
    }
}
