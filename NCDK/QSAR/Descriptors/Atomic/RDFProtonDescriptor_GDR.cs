/* Copyright (C) 2004-2007  Matteo Floris <mfe4@users.sf.net>
 * Copyright (C) 2006-2007  Federico
 *                    2014  Mark B Vine (orcid:0000-0002-7794-0426)
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
    public class RDFProtonDescriptor_GDR : AbstractRDFProtonDescriptor
    {
        internal override int desc_length => 7;

        /// <summary>
        /// Constructor for the RDFProtonDescriptor object
        /// </summary>
        public RDFProtonDescriptor_GDR()
        {
            names = new string[desc_length];
            for (int i = 0; i < desc_length; i++)
            {
                names[i] = "gDr_" + (i + 1);
            }
        }

        private static string[] names;
        public override string[] DescriptorNames => names;

        /// <summary>
        /// The specification attribute of the RDFProtonDescriptor_GDR object
        /// </summary>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification __Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#rdfProtonCalculatedValues",
                typeof(RDFProtonDescriptor_GDR).FullName, "The Chemistry Development Kit");
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
            //Variables
            double sum;
            double smooth = -20;
            double partial;
            int position;
            double limitInf;
            double limitSup;
            double step;

            ////////////////////////THE THIRD DESCRIPTOR IS gD(r) WITH DISTANCE AND RADIAN ANGLE BTW THE PROTON AND THE MIDDLE POINT OF Double BOND

            Vector3 aA;
            Vector3 aB;
            Vector3 bA;
            Vector3 bB;
            Vector3 middlePoint;
            double angle;

            if (doubles.Count > -0.0001)
            {
                IAtom goodAtom0;
                IAtom goodAtom1;
                limitInf = 0;
                limitSup = Math.PI / 2;
                step = (limitSup - limitInf) / 7;
                position = 0;
                partial = 0;
                smooth = -1.15;
                int goodPosition = 0;
                IBond goodBond;
                for (double ghd = limitInf; ghd < limitSup; ghd = ghd + step)
                {
                    sum = 0;
                    for (int dou = 0; dou < doubles.Count; dou++)
                    {
                        partial = 0;
                        position = doubles[dou];
                        var theDoubleBond = mol.Bonds[position];
                        goodPosition = GetNearestBondtoAGivenAtom(mol, atom, theDoubleBond);
                        goodBond = mol.Bonds[goodPosition];
                        goodAtom0 = goodBond.Atoms[0];
                        goodAtom1 = goodBond.Atoms[1];
                        var atomP = atom.Point3D.Value;
                        var goodAtom0P = goodAtom0.Point3D.Value;
                        var goodAtom1P = goodAtom1.Point3D.Value;

                        //Console.Out.WriteLine("GOOD POS IS "+mol.Atoms.IndexOf(goodAtoms[0])+" "+mol.Atoms.IndexOf(goodAtoms[1]));

                        middlePoint = theDoubleBond.Geometric3DCenter;
                        var values = CalculateDistanceBetweenAtomAndBond(atom, theDoubleBond);

                        if (theDoubleBond.Contains(goodAtom0))
                        {
                            aA = new Vector3(goodAtom0P.X, goodAtom0P.Y, goodAtom0P.Z);
                            aB = new Vector3(goodAtom1P.X, goodAtom1P.Y, goodAtom1P.Z);
                        }
                        else
                        {
                            aA = new Vector3(goodAtom1P.X, goodAtom1P.Y, goodAtom1P.Z);
                            aB = new Vector3(goodAtom0P.X, goodAtom0P.Y, goodAtom0P.Z);
                        }
                        bA = new Vector3(middlePoint.X, middlePoint.Y, middlePoint.Z);
                        bB = new Vector3(atomP.X, atomP.Y, atomP.Z);
                        angle = CalculateAngleBetweenTwoLines(aA, aB, bA, bB);
                        partial = ((1 / (Math.Pow(values[0], 2))) * Math.Exp(smooth * (Math.Pow((ghd - angle), 2))));
                        sum += partial;
                    }
                    //gDr_function.Add(new Double(sum));
                    rdfProtonCalculatedValues.Add(sum);
                    Debug.WriteLine("GDR prob dist.: " + sum + " at distance " + ghd);
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
