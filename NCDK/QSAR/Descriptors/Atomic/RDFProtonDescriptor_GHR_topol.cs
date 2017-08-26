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
using NCDK.Graphs;
using NCDK.QSAR.Results;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    /// This class calculates GHR topological proton descriptors used in neural networks for H1 NMR
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
    public partial class RDFProtonDescriptor_GHR_topol : IAtomicDescriptor
    {
        private int desc_length => 15;

        public RDFProtonDescriptor_GHR_topol()
        {
            names = new string[desc_length];
            for (int i = 0; i < desc_length; i++)
            {
                names[i] = "gHrTop_" + (i + 1);
            }
        }

        private static string[] names;
        public IReadOnlyList<string> DescriptorNames => names;

        /// <summary>
        /// The specification attribute of the RDFProtonDescriptor_GHR_topol object
        /// </summary>
        public IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification __Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#rdfProtonCalculatedValues",
                typeof(RDFProtonDescriptor_GHR_topol).FullName, "The Chemistry Development Kit");
        private DescriptorSpecification _Specification => __Specification;

        private bool MakeDescriptorLastStage(
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

            ///////////////////////THE SECOND CALCULATED DESCRIPTOR IS g(H)r TOPOLOGICAL WITH SUM OF BOND LENGTHS

            smooth = -20;
            IAtom startVertex = clonedAtom;
            IAtom endVertex;
            int thisAtom;
            limitInf = 1.4;
            limitSup = 4;
            step = (limitSup - limitInf) / 15;

            if (atoms.Count > 0)
            {
                //ArrayList gHr_topol_function = new ArrayList(15);
                ShortestPaths shortestPaths = new ShortestPaths(mol, startVertex);
                for (double ghrt = limitInf; ghrt < limitSup; ghrt = ghrt + step)
                {
                    sum = 0;
                    for (int at = 0; at < atoms.Count; at++)
                    {
                        distance = 0;
                        thisAtom = atoms[at];
                        position = thisAtom;
                        endVertex = mol.Atoms[position];
                        atom2 = mol.Atoms[position];
                        int[] path = shortestPaths.GetPathTo(endVertex);
                        for (int i = 1; i < path.Length; i++)
                        {
                            distance += CalculateDistanceBetweenTwoAtoms(mol.Atoms[path[i - 1]], mol.Atoms[path[i]]);
                        }
                        partial = atom2.Charge.Value * Math.Exp(smooth * (Math.Pow((ghrt - distance), 2)));
                        sum += partial;
                    }
                    //gHr_topol_function.Add(new Double(sum));
                    rdfProtonCalculatedValues.Add(sum);
                    Debug.WriteLine("RDF gr-topol distance prob.: " + sum + " at distance " + ghrt);
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
