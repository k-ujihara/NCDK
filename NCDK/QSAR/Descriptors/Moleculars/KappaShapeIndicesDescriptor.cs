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
using NCDK.QSAR.Result;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Kier and Hall kappa molecular shape indices compare the molecular graph with minimal and maximal molecular graphs;
    /// a description is given at: http://www.chemcomp.com/Journal_of_CCG/Features/descr.htm#KH :
    /// "they are intended to capture different aspects of molecular shape.  Note that hydrogens are ignored.
    /// In the following description, n denotes the number of atoms in the hydrogen suppressed graph,
    /// m is the number of bonds in the hydrogen suppressed graph. Also, let p2 denote the number of paths of length 2
    /// and let p3 denote the number of paths of length 3".
    /// </summary>
    /// <remarks>
    /// Returns three values in the order
    /// <list type="bullet"> 
    /// <item>Kier1 -  First kappa shape index</item>
    /// <item>Kier2 - Second kappa shape index</item>
    /// <item>Kier3 -  Third kappa (É») shape index</item>
    /// </list>
    /// <para>This descriptor does not have any parameters.</para>
    /// </remarks>
    // @author mfe4
    // @cdk.created 2004-11-03
    // @cdk.module qsarmolecular
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:kierValues
    // @cdk.keyword Kappe shape index
    // @cdk.keyword descriptor
    public class KappaShapeIndicesDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        private static readonly string[] NAMES = { "Kier1", "Kier2", "Kier3" };

        /// <summary>
        /// Constructor for the KappaShapeIndicesDescriptor object
        /// </summary>
        public KappaShapeIndicesDescriptor() { }

        /// <summary>
        /// The specification attribute of the KappaShapeIndicesDescriptor object
        /// </summary>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
         new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#kierValues",
                typeof(KappaShapeIndicesDescriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the
        /// KappaShapeIndicesDescriptor object
        /// </summary>
        public override object[] Parameters { get { return null; } set { } }

        public override string[] DescriptorNames => NAMES;

        /// <summary>
        /// Calculates the kier shape indices for an atom container
        /// </summary>
        /// <param name="container">AtomContainer</param>
        /// <returns>kier1, kier2 and kier3 are returned as arrayList of doubles</returns>
        /// <exception cref="CDKException">Possible Exceptions</exception>
        public override DescriptorValue Calculate(IAtomContainer container)
        {
            IAtomContainer atomContainer;
            atomContainer = (IAtomContainer)container.Clone();
            atomContainer = AtomContainerManipulator.RemoveHydrogens(atomContainer);

            //IAtom[] atoms = atomContainer.GetAtoms();
            DoubleArrayResult kierValues = new DoubleArrayResult(3);
            double bond1;
            double bond2;
            double bond3;
            double kier1;
            double kier2;
            double kier3;
            double atomsCount = atomContainer.Atoms.Count;
            var singlePaths = new List<double>();
            var doublePaths = new List<string>();
            var triplePaths = new List<string>();
            double[] sorterFirst = new double[2];
            double[] sorterSecond = new double[3];
            string tmpbond2;
            string tmpbond3;

            for (int a1 = 0; a1 < atomsCount; a1++)
            {
                bond1 = 0;
                var firstAtomNeighboors = atomContainer.GetConnectedAtoms(atomContainer.Atoms[a1]);
                foreach (var firstAtomNeighboor in firstAtomNeighboors)
                {
                    bond1 = atomContainer.Bonds.IndexOf(atomContainer.GetBond(atomContainer.Atoms[a1], firstAtomNeighboor));
                    if (!singlePaths.Contains(bond1))
                    {
                        singlePaths.Add(bond1);
                        singlePaths.Sort();
                    }
                    var secondAtomNeighboors = atomContainer.GetConnectedAtoms(firstAtomNeighboor);
                    foreach (var secondAtomNeighboor in secondAtomNeighboors)
                    {
                        bond2 = atomContainer.Bonds.IndexOf(atomContainer.GetBond(firstAtomNeighboor, secondAtomNeighboor));
                        if (!singlePaths.Contains(bond2))
                        {
                            singlePaths.Add(bond2);
                        }
                        sorterFirst[0] = bond1;
                        sorterFirst[1] = bond2;
                        Array.Sort(sorterFirst);

                        tmpbond2 = sorterFirst[0] + "+" + sorterFirst[1];

                        if (!doublePaths.Contains(tmpbond2) && (bond1 != bond2))
                        {
                            doublePaths.Add(tmpbond2);
                        }
                        var thirdAtomNeighboors = atomContainer.GetConnectedAtoms(secondAtomNeighboor);
                        foreach (var thirdAtomNeighboor in thirdAtomNeighboors)
                        {
                            bond3 = atomContainer.Bonds.IndexOf(atomContainer.GetBond(secondAtomNeighboor, thirdAtomNeighboor));
                            if (!singlePaths.Contains(bond3))
                            {
                                singlePaths.Add(bond3);
                            }
                            sorterSecond[0] = bond1;
                            sorterSecond[1] = bond2;
                            sorterSecond[2] = bond3;
                            Array.Sort(sorterSecond);

                            tmpbond3 = sorterSecond[0] + "+" + sorterSecond[1] + "+" + sorterSecond[2];
                            if (!triplePaths.Contains(tmpbond3))
                            {
                                if ((bond1 != bond2) && (bond1 != bond3) && (bond2 != bond3))
                                {
                                    triplePaths.Add(tmpbond3);
                                }
                            }
                        }
                    }
                }
            }

            if (atomsCount == 1)
            {
                kier1 = 0;
                kier2 = 0;
                kier3 = 0;
            }
            else
            {
                kier1 = (((atomsCount) * ((atomsCount - 1) * (atomsCount - 1))) / (singlePaths.Count * singlePaths.Count));
                if (atomsCount == 2)
                {
                    kier2 = 0;
                    kier3 = 0;
                }
                else
                {
                    if (doublePaths.Count == 0)
                        kier2 = double.NaN;
                    else
                        kier2 = (((atomsCount - 1) * ((atomsCount - 2) * (atomsCount - 2))) / (doublePaths.Count * doublePaths.Count));
                    if (atomsCount == 3)
                    {
                        kier3 = 0;
                    }
                    else
                    {
                        if (atomsCount % 2 != 0)
                        {
                            if (triplePaths.Count == 0)
                                kier3 = double.NaN;
                            else
                                kier3 = (((atomsCount - 1) * ((atomsCount - 3) * (atomsCount - 3))) / (triplePaths.Count * triplePaths.Count));
                        }
                        else
                        {
                            if (triplePaths.Count == 0)
                                kier3 = double.NaN;
                            else
                                kier3 = (((atomsCount - 3) * ((atomsCount - 2) * (atomsCount - 2))) / (triplePaths.Count * triplePaths.Count));
                        }
                    }
                }
            }

            kierValues.Add(kier1);
            kierValues.Add(kier2);
            kierValues.Add(kier3);
            return new DescriptorValue(_Specification, ParameterNames, Parameters, kierValues,
                    DescriptorNames);
        }

        /// <inheritdoc/>
        public override IDescriptorResult DescriptorResultType { get; } = new DoubleArrayResultType(3);

        /// <summary>
        /// The parameterNames attribute of the
        /// KappaShapeIndicesDescriptor object
        /// </summary>
        public override string[] ParameterNames => null; // no param names to return

        /// <summary>
        /// Gets the parameterType attribute of the
        /// KappaShapeIndicesDescriptor object
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>The parameterType value</returns>
        public override object GetParameterType(string name) => null;
    }
}
