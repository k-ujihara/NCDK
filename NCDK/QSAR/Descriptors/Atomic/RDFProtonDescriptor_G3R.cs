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
using NCDK.Numerics;
using NCDK.QSAR.Results;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    /// This class calculates G3R proton descriptors used in neural networks for H1
    /// NMR shift <token>cdk-cite-AiresDeSousa2002</token>. It only applies to (explicit) hydrogen atoms,
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
    public class RDFProtonDescriptor_G3R : AbstractRDFProtonDescriptor
    {
        internal override int desc_length => 13;

        /// <summary>
        /// Constructor for the RDFProtonDescriptor object
        /// </summary>
        public RDFProtonDescriptor_G3R()
        {
            names = new string[desc_length];
            for (int i = 0; i < desc_length; i++)
            {
                names[i] = "g3r_" + (i + 1);
            }
        }

        private static string[] names;
        public override string[] DescriptorNames => names;

        /// <summary>
        /// Gets the specification attribute of the RDFProtonDescriptor_G3R object
        /// </summary>
        /// <returns>The specification value</returns>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification __Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#rdfProtonCalculatedValues",
                typeof(RDFProtonDescriptor_G3R).FullName,
                "The Chemistry Development Kit");
        internal override DescriptorSpecification _Specification => __Specification;

        internal override bool MakeDescriptorLastStage(
            DoubleArrayResult rdfProtonCalculatedValues,
            IAtom atom,
            IAtom clonedAtom,
            IAtomContainer mol,
            IAtom neighbour0,
            List<int> singles,
            List<int> doubles,
            List<int> atoms,
            List<int> bondsInCycloex)
        {
            // ////////////////////////LAST DESCRIPTOR IS g3(r), FOR PROTONS BONDED TO LIKE-CYCLOEXANE RINGS:
            if (bondsInCycloex.Count > 0)
            {
                // Variables
                double sum;
                double smooth = -20;
                double partial;
                int position;
                double limitInf;
                double limitSup;
                double step;

                Vector3 aA;
                Vector3 aB;
                Vector3 bA;
                Vector3 bB;
                double angle;

                IAtom cycloexBondAtom0;
                IAtom cycloexBondAtom1;
                IBond theInCycloexBond;
                limitInf = 0;
                limitSup = Math.PI;
                step = (limitSup - limitInf) / 13;
                position = 0;
                smooth = -2.86;
                angle = 0;
                int yaCounter = 0;
                for (double g3r = 0; g3r < limitSup; g3r = g3r + step)
                {
                    sum = 0;
                    foreach (var aBondsInCycloex in bondsInCycloex)
                    {
                        yaCounter = 0;
                        angle = 0;
                        partial = 0;
                        position = aBondsInCycloex;
                        theInCycloexBond = mol.Bonds[position];
                        cycloexBondAtom0 = theInCycloexBond.Atoms[0];
                        cycloexBondAtom1 = theInCycloexBond.Atoms[1];

                        var connAtoms = mol.GetConnectedAtoms(cycloexBondAtom0);
                        foreach (var connAtom in connAtoms)
                        {
                            if (connAtom.Equals(neighbour0)) yaCounter += 1;
                        }

                        if (yaCounter > 0)
                        {
                            aA = new Vector3(cycloexBondAtom1.Point3D.Value.X, cycloexBondAtom1.Point3D.Value.Y,
                                    cycloexBondAtom1.Point3D.Value.Z);
                            aB = new Vector3(cycloexBondAtom0.Point3D.Value.X, cycloexBondAtom0.Point3D.Value.Y,
                                    cycloexBondAtom0.Point3D.Value.Z);
                        }
                        else
                        {
                            aA = new Vector3(cycloexBondAtom0.Point3D.Value.X, cycloexBondAtom0.Point3D.Value.Y,
                                    cycloexBondAtom0.Point3D.Value.Z);
                            aB = new Vector3(cycloexBondAtom1.Point3D.Value.X, cycloexBondAtom1.Point3D.Value.Y,
                                    cycloexBondAtom1.Point3D.Value.Z);
                        }
                        bA = new Vector3(neighbour0.Point3D.Value.X, neighbour0.Point3D.Value.Y, neighbour0.Point3D.Value.Z);
                        bB = new Vector3(atom.Point3D.Value.X, atom.Point3D.Value.Y, atom.Point3D.Value.Z);

                        angle = CalculateAngleBetweenTwoLines(aA, aB, bA, bB);

                        // Debug.WriteLine("gcycr ANGLE: " + angle + " "
                        // +mol.Atoms.IndexOf(cycloexBondAtom0) + "
                        // "+mol.Atoms.IndexOf(cycloexBondAtom1));

                        partial = Math.Exp(smooth * (Math.Pow((g3r - angle), 2)));
                        sum += partial;
                    }
                    // g3r_function.Add(new Double(sum));
                    rdfProtonCalculatedValues.Add(sum);
                    Debug.WriteLine("RDF g3r prob.: " + sum + " at distance " + g3r);
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
