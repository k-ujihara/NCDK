/* Copyright (C) 2004-2007  Matteo Floris <mfe4@users.sf.net>
 * Copyright (C) 2006-2007  Federico
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
using NCDK.QSAR.Results;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    /// This class calculates GHR proton descriptors used in neural networks for H1 NMR
    /// shift <token>cdk-cite-AiresDeSousa2002</token>. It only applies to (explicit) hydrogen atoms,
    /// requires aromaticity to be perceived (possibly done via a parameter), and
    /// needs 3D coordinates for all atoms.
    /// </summary>
    /// <remarks>
    ///  This descriptor uses these parameters:
    /// <list type="table">
    /// <listheader><term>Name</term><term>Default</term><term>Description</term></listheader>
    /// <item><term>checkAromaticity</term><term>false</term><term>True is the aromaticity has to be checked</term></item>
    /// </list>
    /// </remarks>
    // @author      Federico
    // @cdk.created 2006-12-11
    // @cdk.module  qsaratomic
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:rdfProtonCalculatedValues
    // @cdk.bug     1632419
    public class RDFProtonDescriptor_GHR : AbstractRDFProtonDescriptor
    {
        internal override int desc_length => 15;

        /// <summary>
        /// Constructor for the RDFProtonDescriptor object
        /// </summary>
        public RDFProtonDescriptor_GHR()
        {
            names = new string[desc_length];
            for (int i = 0; i < desc_length; i++)
            {
                names[i] = "RDF_GHR_" + i;
            }
        }

        private static string[] names;
        public override IReadOnlyList<string> DescriptorNames => names;

        /// <summary>
        /// The specification attribute of the RDFProtonDescriptor_GHR object
        /// </summary>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification __Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#rdfProtonCalculatedValues",
                typeof(RDFProtonDescriptor_GHR).FullName,
                "The Chemistry Development Kit");
        internal override DescriptorSpecification _Specification => __Specification;

        internal override bool MakeDescriptorLastStage(
            ArrayResult<double> rdfProtonCalculatedValues,
            IAtom atom,
            IAtom clonedAtom,
            IAtomContainer mol,
            IAtom neighbour0,
            List<int> singles,
            List<int> doubles,
            List<int> atoms,
            List<int> bondsInCycloex)
        {
            ///////////////////////THE FIRST CALCULATED DESCRIPTOR IS g(H)r     WITH Partial CHARGES:
            if (atoms.Any())
            {
                //Variables
                double distance;
                double sum;
                double smooth = -20;
                double partial;
                int position;
                double limitInf = 1.4;
                double limitSup = 4;
                double step = (limitSup - limitInf) / 15;
                IAtom atom2;

                for (double ghr = limitInf; ghr < limitSup; ghr = ghr + step)
                {
                    sum = 0;
                    foreach (var atom1 in atoms)
                    {
                        distance = 0;
                        partial = 0;
                        int thisAtom = (int)atom1;
                        position = thisAtom;
                        atom2 = mol.Atoms[position];
                        distance = CalculateDistanceBetweenTwoAtoms(atom, atom2);
                        partial = atom2.Charge.Value * Math.Exp(smooth * (Math.Pow((ghr - distance), 2)));
                        sum += partial;
                    }
                    rdfProtonCalculatedValues.Add(sum);
                    Debug.WriteLine("RDF gr distance prob.: " + sum + " at distance " + ghr);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
