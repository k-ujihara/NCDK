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
using NCDK.QSAR.Result;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    /// This class calculates GDR proton descriptors used in neural networks for H1 NMR
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
    public class RDFProtonDescriptor_GSR : AbstractRDFProtonDescriptor
    {
        internal override int desc_length => 7;

        public RDFProtonDescriptor_GSR()
        {
            names = new string[desc_length];
            for (int i = 0; i < desc_length; i++)
            {
                names[i] = "gSr_" + (i + 1);
            }
        }

        private static string[] names;
        public override string[] DescriptorNames => names;

        /// <summary>
        /// The specification attribute of the RDFProtonDescriptor_GSR object
        /// </summary>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification __Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#rdfProtonCalculatedValues",
                typeof(RDFProtonDescriptor_GSR).FullName,
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
            //Variables
            double[] values; // for storage of results of other methods
            double sum;
            double smooth = -20;
            double partial;
            int position;
            double limitInf;
            double limitSup;
            double step;

            ////////////////////////THE FOUTH DESCRIPTOR IS gS(r), WHICH TAKES INTO ACCOUNT Single BONDS IN RIGID SYSTEMS

            Vector3 middlePoint = new Vector3();
            double angle = 0;

            if (singles.Count > 0)
            {
                double dist0;
                double dist1;
                IAtom singleBondAtom0;
                IAtom singleBondAtom1;
                position = 0;
                IBond theSingleBond = null;
                limitInf = 0;
                limitSup = Math.PI / 2;
                step = (limitSup - limitInf) / 7;
                smooth = -1.15;
                for (double ghs = 0; ghs < limitSup; ghs = ghs + step)
                {
                    sum = 0;
                    for (int sing = 0; sing < singles.Count; sing++)
                    {
                        angle = 0;
                        partial = 0;
                        int thisSingleBond = singles[sing];
                        position = thisSingleBond;
                        theSingleBond = mol.Bonds[position];
                        middlePoint = theSingleBond.Geometric3DCenter;
                        singleBondAtom0 = theSingleBond.Atoms[0];
                        singleBondAtom1 = theSingleBond.Atoms[1];
                        dist0 = CalculateDistanceBetweenTwoAtoms(singleBondAtom0, atom);
                        dist1 = CalculateDistanceBetweenTwoAtoms(singleBondAtom1, atom);

                        var aA = middlePoint;
                        var aB = dist1 > dist0 ? singleBondAtom0.Point3D.Value : singleBondAtom1.Point3D.Value;
                        var bA = middlePoint;
                        var bB = atom.Point3D.Value;

                        values = CalculateDistanceBetweenAtomAndBond(atom, theSingleBond);

                        angle = CalculateAngleBetweenTwoLines(aA, aB, bA, bB);
                        //Console.Out.WriteLine("ANGLe: "+angle+ " "+ mol.Atoms.IndexOf(atomsInSingleBond[0]) +" " +mol.Atoms.IndexOf(atomsInSingleBond[1]));

                        partial = (1 / (Math.Pow(values[0], 2))) * Math.Exp(smooth * (Math.Pow((ghs - angle), 2)));
                        sum += partial;
                    }
                    //gSr_function.Add(new Double(sum));
                    rdfProtonCalculatedValues.Add(sum);
                    Debug.WriteLine("RDF gSr prob.: " + sum + " at distance " + ghs);
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
