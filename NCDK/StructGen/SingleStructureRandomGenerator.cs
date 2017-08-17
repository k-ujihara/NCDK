/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *  All we ask is that proper credit is given for our work, which includes
 *  - but is not limited to - adding the above copyright notice to the beginning
 *  of your source code files, and to any copyright notice that you may distribute
 *  with programs based on this work.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 */
using NCDK.Graphs;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System;
using System.Diagnostics;
using System.Linq;

namespace NCDK.StructGen
{
    /// <summary>
    /// Randomly generates a single, connected, correctly bonded structure for
    /// a given molecular formula.
    /// To see it working run the graphical
    /// test org.openscience.cdk.test.SingleStructureRandomGeneratorTest
    /// and add more structures to the panel using the "More" button.
    /// In order to use this class, use MFAnalyser to get an AtomContainer from
    /// a molecular formula string.
    /// </summary>
    /// <remarks>
    /// <para>Assign hydrogen counts to each heavy atom. The hydrogens should not be
    /// in the atom pool but should be assigned implicitly to the heavy atoms in
    /// order to reduce computational cost.
    /// Assign this AtomContainer to the
    /// SingleStructureRandomGenerator and retrieve a randomly generated, but correctly bonded
    /// structure by using the Generate() method. You can then repeatedly call
    /// the Generate() method in order to retrieve further structures.</para>
    /// <para>
    /// Agenda:
    /// <list type="bullet">
    ///  <item>add a method for randomly adding hydrogens to the atoms</item>
    ///  <item>add a seed for random generator for reproducability</item>
    /// </list>
    /// </para>
    /// </remarks>
    // @author      steinbeck
    // @cdk.created 2001-09-04
    // @cdk.module  structgen
    // @cdk.githash
    public class SingleStructureRandomGenerator
    {
        IAtomContainer atomContainer;
        SaturationChecker satCheck;
        Maths.Random random = null;

        /// <summary>
        /// Constructor for the SingleStructureRandomGenerator object.
        /// </summary>
        public SingleStructureRandomGenerator(long seed)
        {
            satCheck = new SaturationChecker();
            random = new Maths.Random(seed);
        }

        /// <summary>
        /// Constructor for the SingleStructureRandomGenerator object.
        /// </summary>
        public SingleStructureRandomGenerator()
            : this((long)11000)
        { }

        /// <summary>
        /// Sets the AtomContainer attribute of the SingleStructureRandomGenerator object.
        ///
        /// <param name="ac">The new AtomContainer value</param>
        /// </summary>
        public void SetAtomContainer(IAtomContainer ac)
        {
            this.atomContainer = ac;
        }

        /// <summary>
        /// Generates a random structure based on the atoms in the given IAtomContainer.
        /// </summary>
        public IAtomContainer Generate()
        {
            bool structureFound = false;
            bool bondFormed;
            double order;
            double max, cmax1, cmax2;
            int iteration = 0;
            IAtom partner;
            IAtom atom;
            do
            {
                iteration++;
                atomContainer.RemoveAllElectronContainers();
                do
                {
                    bondFormed = false;
                    for (int f = 0; f < atomContainer.Atoms.Count; f++)
                    {
                        atom = atomContainer.Atoms[f];

                        if (!satCheck.IsSaturated(atom, atomContainer))
                        {
                            partner = GetAnotherUnsaturatedNode(atom);
                            if (partner != null)
                            {
                                cmax1 = satCheck.GetCurrentMaxBondOrder(atom, atomContainer);

                                cmax2 = satCheck.GetCurrentMaxBondOrder(partner, atomContainer);
                                max = Math.Min(cmax1, cmax2);
                                order = Math.Min(Math.Max(1.0, random.NextInt((int)Math.Round(max))), 3.0);
                                Debug.WriteLine("Forming bond of order ", order);
                                atomContainer.Bonds.Add(atomContainer.Builder.NewBond(atom, partner,
                                        BondManipulator.CreateBondOrder(order)));
                                bondFormed = true;
                            }
                        }
                    }
                } while (bondFormed);
                if (ConnectivityChecker.IsConnected(atomContainer) && satCheck.AllSaturated(atomContainer))
                {
                    structureFound = true;
                }
            } while (!structureFound && iteration < 20);
            Debug.WriteLine("Structure found after #iterations: ", iteration);
            return atomContainer.Builder.NewAtomContainer(atomContainer);
        }

        /// <summary>
        /// Gets the AnotherUnsaturatedNode attribute of the SingleStructureRandomGenerator object.
        /// </summary>
        /// <returns>The AnotherUnsaturatedNode value</returns>
        private IAtom GetAnotherUnsaturatedNode(IAtom exclusionAtom)
        {
            IAtom atom;
            int next = random.NextInt(atomContainer.Atoms.Count);

            for (int f = next; f < atomContainer.Atoms.Count; f++)
            {
                atom = atomContainer.Atoms[f];
                if (!satCheck.IsSaturated(atom, atomContainer) && exclusionAtom != atom
                        && !atomContainer.GetConnectedAtoms(exclusionAtom).Contains(atom))
                {
                    return atom;
                }
            }
            for (int f = 0; f < next; f++)
            {
                atom = atomContainer.Atoms[f];
                if (!satCheck.IsSaturated(atom, atomContainer) && exclusionAtom != atom
                        && !atomContainer.GetConnectedAtoms(exclusionAtom).Contains(atom))
                {
                    return atom;
                }
            }
            return null;
        }
    }
}
