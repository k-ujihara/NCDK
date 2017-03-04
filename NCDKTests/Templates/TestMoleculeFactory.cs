/*
 * Copyright (c) 2013. John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */
using NCDK.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Config;
using NCDK.Default;
using System;
using System.Diagnostics;

namespace NCDK.Templates
{
    /// <summary>
    /// This class contains methods for generating simple organic molecules and is
    /// copy of {@link MoleculeFactory} for use in tests.
    ///
    // @cdk.module test-data
    /// </summary>
    public class TestMoleculeFactory
    {
        private static void MolAddBond(IAtomContainer mol, int a, int b, BondOrder order)
        {
            mol.AddBond(mol.Atoms[a], mol.Atoms[b], order);
        }

        public static IAtomContainer MakeAlphaPinene()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("C")); // 4
            mol.Add(new Atom("C")); // 5
            mol.Add(new Atom("C")); // 6
            mol.Add(new Atom("C")); // 7
            mol.Add(new Atom("C")); // 8
            mol.Add(new Atom("C")); // 9
            mol.Add(new Atom("C")); // 10

            MolAddBond(mol, 0, 1, BondOrder.Double); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Single); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 5, BondOrder.Single); // 5
            MolAddBond(mol, 5, 0, BondOrder.Single); // 6
            MolAddBond(mol, 0, 6, BondOrder.Single); // 7
            MolAddBond(mol, 3, 7, BondOrder.Single); // 8
            MolAddBond(mol, 5, 7, BondOrder.Single); // 9
            MolAddBond(mol, 7, 8, BondOrder.Single); // 10
            MolAddBond(mol, 7, 9, BondOrder.Single); // 11
            ConfigureAtoms(mol);
            return mol;
        }

        /// <summary>
        /// Generate an Alkane (chain of carbons with no hydrogens) of a given length.
        ///
        /// <p>This method was written by Stephen Tomkinson.
        ///
        /// <param name="chainLength">The number of carbon atoms to have in the chain.</param>
        /// <returns>A molecule containing a bonded chain of carbons.</returns>
        // @cdk.created 2003-08-15
        /// </summary>
        public static IAtomContainer MakeAlkane(int chainLength)
        {
            IAtomContainer currentChain = new AtomContainer();

            //Add the initial atom
            currentChain.Add(new Atom("C"));

            //Add further atoms and bonds as needed, a pair at a time.
            for (int atomCount = 1; atomCount < chainLength; atomCount++)
            {
                currentChain.Add(new Atom("C"));
                MolAddBond(currentChain, atomCount, atomCount - 1, BondOrder.Single);
            }

            return currentChain;
        }

        public static IAtomContainer MakeEthylCyclohexane()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("C")); // 4
            mol.Add(new Atom("C")); // 5
            mol.Add(new Atom("C")); // 6
            mol.Add(new Atom("C")); // 7
            mol.Add(new Atom("C")); // 8

            MolAddBond(mol, 0, 1, BondOrder.Single); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Single); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 5, BondOrder.Single); // 5
            MolAddBond(mol, 5, 0, BondOrder.Single); // 6
            MolAddBond(mol, 0, 6, BondOrder.Single); // 7
            MolAddBond(mol, 6, 7, BondOrder.Single); // 8
            return mol;
        }

        /// <summary>
        /// Returns cyclohexene without explicit hydrogens.
        ///
        // @cdk.inchi InChI=1/C6H10/c1-2-4-6-5-3-1/h1-2H,3-6H2
        /// </summary>
        public static IAtomContainer MakeCyclohexene()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("C")); // 4
            mol.Add(new Atom("C")); // 5
            mol.Add(new Atom("C")); // 6

            MolAddBond(mol, 0, 1, BondOrder.Single); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Single); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 5, BondOrder.Single); // 5
            MolAddBond(mol, 5, 0, BondOrder.Double); // 6
            return mol;
        }

        /// <summary>
        /// Returns cyclohexane without explicit hydrogens.
        ///
        // @cdk.inchi InChI=1/C6H12/c1-2-4-6-5-3-1/h1-6H2
        /// </summary>
        public static IAtomContainer MakeCyclohexane()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("C")); // 4
            mol.Add(new Atom("C")); // 5
            mol.Add(new Atom("C")); // 6

            MolAddBond(mol, 0, 1, BondOrder.Single); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Single); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 5, BondOrder.Single); // 5
            MolAddBond(mol, 5, 0, BondOrder.Single); // 6
            return mol;
        }

        /// <summary>
        /// Returns cyclopentane without explicit hydrogens.
        ///
        // @cdk.inchi InChI=1/C5H10/c1-2-4-5-3-1/h1-5H2
        /// </summary>
        public static IAtomContainer MakeCyclopentane()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("C")); // 4
            mol.Add(new Atom("C")); // 5

            MolAddBond(mol, 0, 1, BondOrder.Single); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Single); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 0, BondOrder.Single); // 5
            return mol;
        }

        /// <summary>
        /// Returns cyclobutane without explicit hydrogens.
        ///
        // @cdk.inchi InChI=1/C4H8/c1-2-4-3-1/h1-4H2
        /// </summary>
        public static IAtomContainer MakeCyclobutane()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("C")); // 4

            MolAddBond(mol, 0, 1, BondOrder.Single); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Single); // 3
            MolAddBond(mol, 3, 0, BondOrder.Single); // 4
            return mol;
        }

        /// <summary>
        /// Returns cyclobutadiene without explicit hydrogens.
        ///
        // @cdk.inchi InChI=1/C4H4/c1-2-4-3-1/h1-4H
        /// </summary>
        public static IAtomContainer MakeCyclobutadiene()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("C")); // 4

            MolAddBond(mol, 0, 1, BondOrder.Single); // 1
            MolAddBond(mol, 1, 2, BondOrder.Double); // 2
            MolAddBond(mol, 2, 3, BondOrder.Single); // 3
            MolAddBond(mol, 3, 0, BondOrder.Double); // 4
            return mol;
        }

        public static IAtomContainer MakePropylCycloPropane()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 0
            mol.Add(new Atom("C")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("C")); // 4
            mol.Add(new Atom("C")); // 4
            MolAddBond(mol, 0, 1, BondOrder.Single); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 0, BondOrder.Single); // 3
            MolAddBond(mol, 2, 3, BondOrder.Single); // 4
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 5, BondOrder.Single); // 4

            return mol;
        }

        /// <summary>
        /// Returns biphenyl without explicit hydrogens.
        ///
        // @cdk.inchi InChI=1/C12H10/c1-3-7-11(8-4-1)12-9-5-2-6-10-12/h1-10H
        /// </summary>
        public static IAtomContainer MakeBiphenyl()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 0
            mol.Add(new Atom("C")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("C")); // 4
            mol.Add(new Atom("C")); // 5
            mol.Add(new Atom("C")); // 6
            mol.Add(new Atom("C")); // 7
            mol.Add(new Atom("C")); // 8
            mol.Add(new Atom("C")); // 9
            mol.Add(new Atom("C")); // 10
            mol.Add(new Atom("C")); // 11

            MolAddBond(mol, 0, 1, BondOrder.Double); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Double); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 5, BondOrder.Double); // 5
            MolAddBond(mol, 5, 0, BondOrder.Single); // 6

            MolAddBond(mol, 0, 6, BondOrder.Single); // 7
            MolAddBond(mol, 6, 7, BondOrder.Single); // 8
            MolAddBond(mol, 7, 8, BondOrder.Double); // 5
            MolAddBond(mol, 8, 9, BondOrder.Single); // 6
            MolAddBond(mol, 9, 10, BondOrder.Double); // 7
            MolAddBond(mol, 10, 11, BondOrder.Single); // 8
            MolAddBond(mol, 11, 6, BondOrder.Double); // 5
            return mol;
        }

        public static IAtomContainer MakePhenylEthylBenzene()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 0
            mol.Add(new Atom("C")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("C")); // 4
            mol.Add(new Atom("C")); // 5
            mol.Add(new Atom("C")); // 6
            mol.Add(new Atom("C")); // 7
            mol.Add(new Atom("C")); // 8
            mol.Add(new Atom("C")); // 9
            mol.Add(new Atom("C")); // 10
            mol.Add(new Atom("C")); // 11
            mol.Add(new Atom("C")); // 12
            mol.Add(new Atom("C")); // 13

            MolAddBond(mol, 0, 1, BondOrder.Double); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Double); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 5, BondOrder.Double); // 5
            MolAddBond(mol, 5, 0, BondOrder.Single); // 6

            MolAddBond(mol, 0, 6, BondOrder.Single); // 7
            MolAddBond(mol, 6, 7, BondOrder.Single); // 8
            MolAddBond(mol, 7, 8, BondOrder.Single); // 5
            MolAddBond(mol, 8, 9, BondOrder.Single); // 6
            MolAddBond(mol, 9, 10, BondOrder.Double); // 7
            MolAddBond(mol, 10, 11, BondOrder.Single); // 8
            MolAddBond(mol, 11, 12, BondOrder.Double); // 5
            MolAddBond(mol, 12, 13, BondOrder.Single);
            MolAddBond(mol, 13, 8, BondOrder.Double); // 5
            return mol;
        }

        public static IAtomContainer MakePhenylAmine()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 0
            mol.Add(new Atom("C")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("C")); // 4
            mol.Add(new Atom("C")); // 5
            mol.Add(new Atom("N")); // 6

            MolAddBond(mol, 0, 1, BondOrder.Double); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Double); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 5, BondOrder.Double); // 5
            MolAddBond(mol, 5, 0, BondOrder.Single); // 6

            MolAddBond(mol, 0, 6, BondOrder.Single); // 7
            return mol;
        }

        /* build a molecule from 4 condensed triangles */
        public static IAtomContainer Make4x3CondensedRings()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("C")); // 4
            mol.Add(new Atom("C")); // 5
            mol.Add(new Atom("C")); // 6

            MolAddBond(mol, 0, 1, BondOrder.Single); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 0, BondOrder.Single); // 3
            MolAddBond(mol, 2, 3, BondOrder.Single); // 4
            MolAddBond(mol, 1, 3, BondOrder.Single); // 5
            MolAddBond(mol, 3, 4, BondOrder.Single); // 6
            MolAddBond(mol, 4, 2, BondOrder.Single); // 7
            MolAddBond(mol, 4, 5, BondOrder.Single); // 8
            MolAddBond(mol, 5, 3, BondOrder.Single); // 9

            return mol;
        }

        public static IAtomContainer MakeSpiroRings()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 0
            mol.Add(new Atom("C")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("C")); // 4
            mol.Add(new Atom("C")); // 5
            mol.Add(new Atom("C")); // 6
            mol.Add(new Atom("C")); // 7
            mol.Add(new Atom("C")); // 8
            mol.Add(new Atom("C")); // 9

            MolAddBond(mol, 0, 1, BondOrder.Single); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Single); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 5, BondOrder.Single); // 5
            MolAddBond(mol, 5, 6, BondOrder.Single); // 6
            MolAddBond(mol, 6, 0, BondOrder.Single); // 7
            MolAddBond(mol, 6, 7, BondOrder.Single); // 8
            MolAddBond(mol, 7, 8, BondOrder.Single); // 9
            MolAddBond(mol, 8, 9, BondOrder.Single); // 10
            MolAddBond(mol, 9, 6, BondOrder.Single); // 11
            return mol;
        }

        public static IAtomContainer MakeBicycloRings()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 0
            mol.Add(new Atom("C")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("C")); // 4
            mol.Add(new Atom("C")); // 5
            mol.Add(new Atom("C")); // 6
            mol.Add(new Atom("C")); // 7

            MolAddBond(mol, 0, 1, BondOrder.Single); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Single); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 5, BondOrder.Single); // 5
            MolAddBond(mol, 5, 0, BondOrder.Single); // 6
            MolAddBond(mol, 6, 0, BondOrder.Single); // 7
            MolAddBond(mol, 6, 7, BondOrder.Single); // 8
            MolAddBond(mol, 7, 3, BondOrder.Single); // 9
            return mol;
        }

        public static IAtomContainer MakeFUsedRings()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 0
            mol.Add(new Atom("C")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("C")); // 4
            mol.Add(new Atom("C")); // 5
            mol.Add(new Atom("C")); // 6
            mol.Add(new Atom("C")); // 7
            mol.Add(new Atom("C")); // 8
            mol.Add(new Atom("C")); // 9

            MolAddBond(mol, 0, 1, BondOrder.Single); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Single); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 5, BondOrder.Single); // 5
            MolAddBond(mol, 5, 0, BondOrder.Single); // 6
            MolAddBond(mol, 5, 6, BondOrder.Single); // 7
            MolAddBond(mol, 6, 7, BondOrder.Single); // 8
            MolAddBond(mol, 7, 4, BondOrder.Single); // 9
            MolAddBond(mol, 8, 0, BondOrder.Single); // 10
            MolAddBond(mol, 9, 1, BondOrder.Single); // 11
            MolAddBond(mol, 9, 8, BondOrder.Single); // 11
            return mol;
        }

        public static IAtomContainer MakeMethylDecaline()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 0
            mol.Add(new Atom("C")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("C")); // 4
            mol.Add(new Atom("C")); // 5
            mol.Add(new Atom("C")); // 6
            mol.Add(new Atom("C")); // 7
            mol.Add(new Atom("C")); // 8
            mol.Add(new Atom("C")); // 9
            mol.Add(new Atom("C")); // 10

            MolAddBond(mol, 0, 1, BondOrder.Single); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Single); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 5, BondOrder.Single); // 5
            MolAddBond(mol, 5, 0, BondOrder.Single); // 6
            MolAddBond(mol, 5, 6, BondOrder.Single); // 7
            MolAddBond(mol, 6, 7, BondOrder.Single); // 8RingSet
            MolAddBond(mol, 7, 8, BondOrder.Single); // 9
            MolAddBond(mol, 8, 9, BondOrder.Single); // 10
            MolAddBond(mol, 9, 0, BondOrder.Single); // 11
            MolAddBond(mol, 5, 10, BondOrder.Single); // 12
            return mol;

        }

        public static IAtomContainer MakeEthylPropylPhenantren()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 0
            mol.Add(new Atom("C")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("C")); // 4
            mol.Add(new Atom("C")); // 5
            mol.Add(new Atom("C")); // 6
            mol.Add(new Atom("C")); // 7
            mol.Add(new Atom("C")); // 8
            mol.Add(new Atom("C")); // 9
            mol.Add(new Atom("C")); // 10
            mol.Add(new Atom("C")); // 11
            mol.Add(new Atom("C")); // 12
            mol.Add(new Atom("C")); // 13
            mol.Add(new Atom("C")); // 14
            mol.Add(new Atom("C")); // 15
            mol.Add(new Atom("C")); // 16
            mol.Add(new Atom("C")); // 17
            mol.Add(new Atom("C")); // 18

            MolAddBond(mol, 0, 1, BondOrder.Single); // 1
            MolAddBond(mol, 1, 2, BondOrder.Double); // 2
            MolAddBond(mol, 2, 3, BondOrder.Single); // 3
            MolAddBond(mol, 3, 4, BondOrder.Double); // 4
            MolAddBond(mol, 4, 5, BondOrder.Single); // 5
            MolAddBond(mol, 5, 6, BondOrder.Double); // 6
            MolAddBond(mol, 6, 7, BondOrder.Single); // 8
            MolAddBond(mol, 7, 8, BondOrder.Double); // 9
            MolAddBond(mol, 8, 9, BondOrder.Single); // 10
            MolAddBond(mol, 9, 0, BondOrder.Double); // 11
            MolAddBond(mol, 9, 4, BondOrder.Single); // 12
            MolAddBond(mol, 8, 10, BondOrder.Single); // 12
            MolAddBond(mol, 10, 11, BondOrder.Double); // 12
            MolAddBond(mol, 11, 12, BondOrder.Single); // 12
            MolAddBond(mol, 12, 13, BondOrder.Double); // 12
            MolAddBond(mol, 13, 7, BondOrder.Single); // 12
            MolAddBond(mol, 3, 14, BondOrder.Single); // 12
            MolAddBond(mol, 14, 15, BondOrder.Single); // 12
            MolAddBond(mol, 12, 16, BondOrder.Single); // 12
            MolAddBond(mol, 16, 17, BondOrder.Single); // 12
            MolAddBond(mol, 17, 18, BondOrder.Single); // 12
            ConfigureAtoms(mol);
            return mol;
        }

        public static IAtomContainer MakeSteran()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 0
            mol.Add(new Atom("C")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("C")); // 4
            mol.Add(new Atom("C")); // 5
            mol.Add(new Atom("C")); // 6
            mol.Add(new Atom("C")); // 7
            mol.Add(new Atom("C")); // 8
            mol.Add(new Atom("C")); // 9
            mol.Add(new Atom("C")); // 10
            mol.Add(new Atom("C")); // 11
            mol.Add(new Atom("C")); // 12
            mol.Add(new Atom("C")); // 13
            mol.Add(new Atom("C")); // 14
            mol.Add(new Atom("C")); // 15
            mol.Add(new Atom("C")); // 16

            MolAddBond(mol, 0, 1, BondOrder.Single); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Single); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 5, BondOrder.Single); // 5
            MolAddBond(mol, 5, 6, BondOrder.Single); // 6
            MolAddBond(mol, 6, 7, BondOrder.Single); // 8
            MolAddBond(mol, 7, 8, BondOrder.Single); // 9
            MolAddBond(mol, 8, 9, BondOrder.Single); // 10
            MolAddBond(mol, 9, 0, BondOrder.Single); // 11
            MolAddBond(mol, 9, 4, BondOrder.Single); // 12
            MolAddBond(mol, 8, 10, BondOrder.Single); // 13
            MolAddBond(mol, 10, 11, BondOrder.Single); // 14
            MolAddBond(mol, 11, 12, BondOrder.Single); // 15
            MolAddBond(mol, 12, 13, BondOrder.Single); // 16
            MolAddBond(mol, 13, 7, BondOrder.Single); // 17
            MolAddBond(mol, 13, 14, BondOrder.Single); // 18
            MolAddBond(mol, 14, 15, BondOrder.Single); // 19
            MolAddBond(mol, 15, 16, BondOrder.Single); // 20
            MolAddBond(mol, 16, 12, BondOrder.Single); // 21

            ConfigureAtoms(mol);
            return mol;
        }

        /// <summary>
        /// Returns azulene without explicit hydrogens.
        ///
        // @cdk.inchi InChI=1/C10H8/c1-2-5-9-7-4-8-10(9)6-3-1/h1-8H
        /// </summary>
        public static IAtomContainer MakeAzulene()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 0
            mol.Add(new Atom("C")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("C")); // 4
            mol.Add(new Atom("C")); // 5
            mol.Add(new Atom("C")); // 6
            mol.Add(new Atom("C")); // 7
            mol.Add(new Atom("C")); // 8
            mol.Add(new Atom("C")); // 9

            MolAddBond(mol, 0, 1, BondOrder.Double); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Double); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 5, BondOrder.Double); // 5
            MolAddBond(mol, 5, 6, BondOrder.Single); // 6
            MolAddBond(mol, 6, 7, BondOrder.Double); // 8
            MolAddBond(mol, 7, 8, BondOrder.Single); // 9
            MolAddBond(mol, 8, 9, BondOrder.Double); // 10
            MolAddBond(mol, 9, 5, BondOrder.Single); // 11
            MolAddBond(mol, 9, 0, BondOrder.Single); // 12

            return mol;
        }

        /// <summary>
        /// Returns indole without explicit hydrogens.
        ///
        // @cdk.inchi InChI=1/C8H7N/c1-2-4-8-7(3-1)5-6-9-8/h1-6,9H
        /// </summary>
        public static IAtomContainer MakeIndole()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 0
            mol.Add(new Atom("C")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("C")); // 4
            mol.Add(new Atom("C")); // 5
            mol.Add(new Atom("C")); // 6
            mol.Add(new Atom("C")); // 7
            mol.Add(new Atom("N")); // 8

            MolAddBond(mol, 0, 1, BondOrder.Double); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Double); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 5, BondOrder.Double); // 5
            MolAddBond(mol, 5, 6, BondOrder.Single); // 6
            MolAddBond(mol, 6, 7, BondOrder.Double); // 8
            MolAddBond(mol, 7, 8, BondOrder.Single); // 9
            MolAddBond(mol, 0, 5, BondOrder.Single); // 11
            MolAddBond(mol, 8, 0, BondOrder.Single); // 12

            return mol;
        }

        /// <summary>
        /// Returns pyrrole without explicit hydrogens.
        ///
        // @cdk.inchi InChI=1/C4H5N/c1-2-4-5-3-1/h1-5H
        /// </summary>
        public static IAtomContainer MakePyrrole()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 0
            mol.Add(new Atom("N")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("C")); // 4

            MolAddBond(mol, 0, 1, BondOrder.Single); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Double); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 0, BondOrder.Double); // 5

            return mol;
        }

        /// <summary>
        /// Returns pyrrole anion without explicit hydrogens.
        ///
        // @cdk.inchi InChI=1/C4H4N/c1-2-4-5-3-1/h1-4H/q-1
        /// </summary>
        public static IAtomContainer MakePyrroleAnion()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom nitrogenAnion = new Atom("N");
            nitrogenAnion.FormalCharge = -1;
            mol.Add(new Atom("C")); // 0
            mol.Add(nitrogenAnion); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("C")); // 4

            MolAddBond(mol, 0, 1, BondOrder.Single); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Double); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 0, BondOrder.Double); // 5

            return mol;
        }

        /// <summary>
        /// Returns imidazole without explicit hydrogens.
        ///
        // @cdk.inchi InChI=1/C3H4N2/c1-2-5-3-4-1/h1-3H,(H,4,5)/f/h4H
        /// </summary>
        public static IAtomContainer MakeImidazole()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 0
            mol.Add(new Atom("N")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("N")); // 3
            mol.Add(new Atom("C")); // 4

            MolAddBond(mol, 0, 1, BondOrder.Single); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Double); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 0, BondOrder.Double); // 5

            return mol;
        }

        /// <summary>
        /// Returns pyrazole without explicit hydrogens.
        ///
        // @cdk.inchi InChI=1/C3H4N2/c1-2-4-5-3-1/h1-3H,(H,4,5)/f/h4H
        /// </summary>
        public static IAtomContainer MakePyrazole()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 0
            mol.Add(new Atom("N")); // 1
            mol.Add(new Atom("N")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("C")); // 4

            MolAddBond(mol, 0, 1, BondOrder.Single); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Double); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 0, BondOrder.Double); // 5

            return mol;
        }

        /// <summary>
        /// Returns 1,2,4-triazole without explicit hydrogens.
        ///
        // @cdk.inchi InChI=1/C3H4N2/c1-2-4-5-3-1/h1-3H,(H,4,5)/f/h4H
        /// </summary>
        public static IAtomContainer Make124Triazole()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 0
            mol.Add(new Atom("N")); // 1
            mol.Add(new Atom("N")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("N")); // 4

            MolAddBond(mol, 0, 1, BondOrder.Single); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Double); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 0, BondOrder.Double); // 5

            return mol;
        }

        /// <summary>
        /// Returns 1,2,3-triazole without explicit hydrogens.
        ///
        // @cdk.inchi InChI=1/C2H3N3/c1-2-4-5-3-1/h1-2H,(H,3,4,5)/f/h5H
        /// </summary>
        public static IAtomContainer Make123Triazole()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 0
            mol.Add(new Atom("N")); // 1
            mol.Add(new Atom("N")); // 2
            mol.Add(new Atom("N")); // 3
            mol.Add(new Atom("C")); // 4

            MolAddBond(mol, 0, 1, BondOrder.Single); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Double); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 0, BondOrder.Double); // 5

            return mol;
        }

        /// <summary>
        /// Returns tetrazole without explicit hydrogens.
        ///
        // @cdk.inchi InChI=1/CH2N4/c1-2-4-5-3-1/h1H,(H,2,3,4,5)/f/h4H
        /// </summary>
        public static IAtomContainer MakeTetrazole()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("N")); // 0
            mol.Add(new Atom("N")); // 1
            mol.Add(new Atom("N")); // 2
            mol.Add(new Atom("N")); // 3
            mol.Add(new Atom("C")); // 4

            MolAddBond(mol, 0, 1, BondOrder.Single); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Double); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 0, BondOrder.Double); // 5

            return mol;
        }

        /// <summary>
        /// Returns Oxazole without explicit hydrogens.
        ///
        // @cdk.inchi InChI=1/C3H3NO/c1-2-5-3-4-1/h1-3H
        /// </summary>
        public static IAtomContainer MakeOxazole()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 0
            mol.Add(new Atom("O")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("N")); // 3
            mol.Add(new Atom("C")); // 4

            MolAddBond(mol, 0, 1, BondOrder.Single); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Double); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 0, BondOrder.Double); // 5

            return mol;
        }

        /// <summary>
        /// Returns Isoxazole without explicit hydrogens.
        ///
        // @cdk.inchi InChI=1/C3H3NO/c1-2-4-5-3-1/h1-3H
        /// </summary>
        public static IAtomContainer MakeIsoxazole()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 0
            mol.Add(new Atom("O")); // 1
            mol.Add(new Atom("N")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("C")); // 4

            MolAddBond(mol, 0, 1, BondOrder.Single); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Double); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 0, BondOrder.Double); // 5

            return mol;
        }

        /// <summary>
        /// Returns isothiazole without explicit hydrogens.
        ///
        // @cdk.inchi InChI=1/C3H3NS/c1-2-4-5-3-1/h1-3H
        /// </summary>
        public static IAtomContainer MakeIsothiazole()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 0
            mol.Add(new Atom("S")); // 1
            mol.Add(new Atom("N")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("C")); // 4

            MolAddBond(mol, 0, 1, BondOrder.Single); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Double); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 0, BondOrder.Double); // 5

            return mol;
        }

        /// <summary>
        /// Returns thiadiazole without explicit hydrogens.
        ///
        // @cdk.inchi InChI=1/C2H2N2S/c1-3-4-2-5-1/h1-2H
        /// </summary>
        public static IAtomContainer MakeThiadiazole()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 0
            mol.Add(new Atom("S")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("N")); // 3
            mol.Add(new Atom("N")); // 4

            MolAddBond(mol, 0, 1, BondOrder.Single); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Double); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 0, BondOrder.Double); // 5

            return mol;
        }

        /// <summary>
        /// Returns oxadiazole without explicit hydrogens.
        ///
        // @cdk.inchi InChI=1/C2H2N2O/c1-3-4-2-5-1/h1-2H
        /// </summary>
        public static IAtomContainer MakeOxadiazole()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 0
            mol.Add(new Atom("O")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("N")); // 3
            mol.Add(new Atom("N")); // 4

            MolAddBond(mol, 0, 1, BondOrder.Single); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Double); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 0, BondOrder.Double); // 5

            return mol;
        }

        /// <summary>
        /// Returns pyridine without explicit hydrogens.
        ///
        // @cdk.inchi InChI=1/C3H3NO/c1-2-4-5-3-1/h1-3H
        /// </summary>
        public static IAtomContainer MakePyridine()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 0
            mol.Add(new Atom("N")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("C")); // 4
            mol.Add(new Atom("C")); // 5

            MolAddBond(mol, 0, 1, BondOrder.Double); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Double); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 5, BondOrder.Double); // 5
            MolAddBond(mol, 5, 0, BondOrder.Single); // 6

            return mol;
        }

        /// <summary>
        /// Returns pyridine oxide without explicit hydrogens.
        ///
        // @cdk.inchi InChI=1/C5H5NO/c7-6-4-2-1-3-5-6/h1-5H
        /// </summary>
        public static IAtomContainer MakePyridineOxide()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 0
            mol.Add(new Atom("N")); // 1
            mol.Atoms[1].FormalCharge = 1;
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("C")); // 4
            mol.Add(new Atom("C")); // 5
            mol.Add(new Atom("O")); // 6
            mol.Atoms[6].FormalCharge = -1;

            MolAddBond(mol, 0, 1, BondOrder.Double); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Double); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 5, BondOrder.Double); // 5
            MolAddBond(mol, 5, 0, BondOrder.Single); // 6
            MolAddBond(mol, 1, 6, BondOrder.Single); // 7

            return mol;
        }

        /// <summary>
        /// Returns pyrimidine without explicit hydrogens.
        ///
        // @cdk.inchi InChI=1/C4H4N2/c1-2-5-4-6-3-1/h1-4H
        /// </summary>
        public static IAtomContainer MakePyrimidine()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 0
            mol.Add(new Atom("N")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("N")); // 3
            mol.Add(new Atom("C")); // 4
            mol.Add(new Atom("C")); // 5

            MolAddBond(mol, 0, 1, BondOrder.Double); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Double); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 5, BondOrder.Double); // 5
            MolAddBond(mol, 5, 0, BondOrder.Single); // 6

            return mol;
        }

        /// <summary>
        /// Returns pyridazine without explicit hydrogens.
        ///
        // @cdk.inchi InChI=1/C4H4N2/c1-2-4-6-5-3-1/h1-4H
        /// </summary>
        public static IAtomContainer MakePyridazine()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 0
            mol.Add(new Atom("N")); // 1
            mol.Add(new Atom("N")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("C")); // 4
            mol.Add(new Atom("C")); // 5

            MolAddBond(mol, 0, 1, BondOrder.Double); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Double); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 5, BondOrder.Double); // 5
            MolAddBond(mol, 5, 0, BondOrder.Single); // 6

            return mol;
        }

        /// <summary>
        /// Returns triazine without explicit hydrogens.
        ///
        // @cdk.inchi InChI=1/C4H4N2/c1-2-4-6-5-3-1/h1-4H
        /// </summary>
        public static IAtomContainer MakeTriazine()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 0
            mol.Add(new Atom("N")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("N")); // 3
            mol.Add(new Atom("C")); // 4
            mol.Add(new Atom("N")); // 5

            MolAddBond(mol, 0, 1, BondOrder.Double); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Double); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 5, BondOrder.Double); // 5
            MolAddBond(mol, 5, 0, BondOrder.Single); // 6

            return mol;
        }

        /// <summary>
        /// Returns thiazole without explicit hydrogens.
        ///
        // @cdk.inchi InChI=1/C3H3NS/c1-2-5-3-4-1/h1-3H
        /// </summary>
        public static IAtomContainer MakeThiazole()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 0
            mol.Add(new Atom("N")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("S")); // 3
            mol.Add(new Atom("C")); // 4

            MolAddBond(mol, 0, 1, BondOrder.Single); // 1
            MolAddBond(mol, 1, 2, BondOrder.Double); // 2
            MolAddBond(mol, 2, 3, BondOrder.Single); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 0, BondOrder.Double); // 5

            return mol;
        }

        public static IAtomContainer MakeSingleRing()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 0
            mol.Add(new Atom("C")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("C")); // 4
            mol.Add(new Atom("C")); // 5
                                    //        mol.Add(new Atom("C")); // 6
                                    //        mol.Add(new Atom("C")); // 7
                                    //        mol.Add(new Atom("C")); // 8
                                    //        mol.Add(new Atom("C")); // 9

            MolAddBond(mol, 0, 1, BondOrder.Single); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Single); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 5, BondOrder.Single); // 5
            MolAddBond(mol, 5, 0, BondOrder.Single); // 6
                                                     //        MolAddBond(mol, 5, 6, BondOrder.Single); // 7
                                                     //        MolAddBond(mol, 6, 7, BondOrder.Single); // 8
                                                     //        MolAddBond(mol, 7, 4, BondOrder.Single); // 9
                                                     //        MolAddBond(mol, 8, 0, BondOrder.Single); // 10
                                                     //        MolAddBond(mol, 9, 1, BondOrder.Single); // 11

            return mol;
        }

        public static IAtomContainer MakeDiamantane()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 0
            mol.Add(new Atom("C")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("C")); // 4
            mol.Add(new Atom("C")); // 5
            mol.Add(new Atom("C")); // 6
            mol.Add(new Atom("C")); // 7
            mol.Add(new Atom("C")); // 8
            mol.Add(new Atom("C")); // 9
            mol.Add(new Atom("C")); // 10
            mol.Add(new Atom("C")); // 11
            mol.Add(new Atom("C")); // 12
            mol.Add(new Atom("C")); // 13

            MolAddBond(mol, 0, 1, BondOrder.Single); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Single); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 5, BondOrder.Single); // 5
            MolAddBond(mol, 5, 0, BondOrder.Single); // 6
            MolAddBond(mol, 5, 6, BondOrder.Single); // 7
            MolAddBond(mol, 6, 9, BondOrder.Single); // 8
            MolAddBond(mol, 1, 7, BondOrder.Single); // 9
            MolAddBond(mol, 7, 9, BondOrder.Single); // 10
            MolAddBond(mol, 3, 8, BondOrder.Single); // 11
            MolAddBond(mol, 8, 9, BondOrder.Single); // 12
            MolAddBond(mol, 0, 10, BondOrder.Single); // 13
            MolAddBond(mol, 10, 13, BondOrder.Single); // 14
            MolAddBond(mol, 2, 11, BondOrder.Single); // 15
            MolAddBond(mol, 11, 13, BondOrder.Single); // 16
            MolAddBond(mol, 4, 12, BondOrder.Single); // 17
            MolAddBond(mol, 12, 13, BondOrder.Single); // 18

            return mol;
        }

        public static IAtomContainer MakeBranchedAliphatic()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 0
            mol.Add(new Atom("C")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("C")); // 4
            mol.Add(new Atom("C")); // 5
            mol.Add(new Atom("C")); // 6
            mol.Add(new Atom("C")); // 7
            mol.Add(new Atom("C")); // 8
            mol.Add(new Atom("C")); // 9
            mol.Add(new Atom("C")); // 10
            mol.Add(new Atom("C")); // 11
            mol.Add(new Atom("C")); // 12
            mol.Add(new Atom("C")); // 13
            mol.Add(new Atom("C")); // 14
            mol.Add(new Atom("C")); // 15
            mol.Add(new Atom("C")); // 16
            mol.Add(new Atom("C")); // 17
            mol.Add(new Atom("C")); // 18

            MolAddBond(mol, 0, 1, BondOrder.Single); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Double); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 5, BondOrder.Single); // 5
            MolAddBond(mol, 2, 6, BondOrder.Single); // 6
            MolAddBond(mol, 6, 7, BondOrder.Single); // 7
            MolAddBond(mol, 7, 8, BondOrder.Single); // 8
            MolAddBond(mol, 6, 9, BondOrder.Single); // 9
            MolAddBond(mol, 6, 10, BondOrder.Single); // 10
            MolAddBond(mol, 10, 11, BondOrder.Single); // 11
            MolAddBond(mol, 8, 12, BondOrder.Triple); // 12
            MolAddBond(mol, 12, 13, BondOrder.Single); // 13
            MolAddBond(mol, 11, 14, BondOrder.Single); // 14
            MolAddBond(mol, 9, 15, BondOrder.Single);
            MolAddBond(mol, 15, 16, BondOrder.Double);
            MolAddBond(mol, 16, 17, BondOrder.Double);
            MolAddBond(mol, 17, 18, BondOrder.Single);

            return mol;
        }

        public static IAtomContainer MakeBenzene()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("C")); // 0
            mol.Add(new Atom("C")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("C")); // 4
            mol.Add(new Atom("C")); // 5

            MolAddBond(mol, 0, 1, BondOrder.Single); // 1
            MolAddBond(mol, 1, 2, BondOrder.Double); // 2
            MolAddBond(mol, 2, 3, BondOrder.Single); // 3
            MolAddBond(mol, 3, 4, BondOrder.Double); // 4
            MolAddBond(mol, 4, 5, BondOrder.Single); // 5
            MolAddBond(mol, 5, 0, BondOrder.Double); // 6
            return mol;
        }

        public static IAtomContainer MakeQuinone()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("O")); // 0
            mol.Add(new Atom("C")); // 1
            mol.Add(new Atom("C")); // 2
            mol.Add(new Atom("C")); // 3
            mol.Add(new Atom("C")); // 4
            mol.Add(new Atom("C")); // 5
            mol.Add(new Atom("C")); // 6
            mol.Add(new Atom("O")); // 7

            MolAddBond(mol, 0, 1, BondOrder.Double); // 1
            MolAddBond(mol, 1, 2, BondOrder.Single); // 2
            MolAddBond(mol, 2, 3, BondOrder.Double); // 3
            MolAddBond(mol, 3, 4, BondOrder.Single); // 4
            MolAddBond(mol, 4, 5, BondOrder.Single); // 5
            MolAddBond(mol, 5, 6, BondOrder.Double); // 6
            MolAddBond(mol, 6, 1, BondOrder.Single); // 7
            MolAddBond(mol, 4, 7, BondOrder.Double); // 8
            return mol;
        }

        public static IAtomContainer MakePiperidine()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("N"));
            mol.Add(new Atom("C"));
            mol.Add(new Atom("C"));
            mol.Add(new Atom("C"));
            mol.Add(new Atom("C"));
            mol.Add(new Atom("C"));
            mol.Add(new Atom("H"));

            MolAddBond(mol, 0, 1, BondOrder.Single);
            MolAddBond(mol, 1, 2, BondOrder.Single);
            MolAddBond(mol, 2, 3, BondOrder.Single);
            MolAddBond(mol, 3, 4, BondOrder.Single);
            MolAddBond(mol, 4, 5, BondOrder.Single);
            MolAddBond(mol, 5, 0, BondOrder.Single);

            MolAddBond(mol, 0, 6, BondOrder.Single);

            return mol;

        }

        public static IAtomContainer MakeTetrahydropyran()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Add(new Atom("O"));
            mol.Add(new Atom("C"));
            mol.Add(new Atom("C"));
            mol.Add(new Atom("C"));
            mol.Add(new Atom("C"));
            mol.Add(new Atom("C"));

            MolAddBond(mol, 0, 1, BondOrder.Single);
            MolAddBond(mol, 1, 2, BondOrder.Single);
            MolAddBond(mol, 2, 3, BondOrder.Single);
            MolAddBond(mol, 3, 4, BondOrder.Single);
            MolAddBond(mol, 4, 5, BondOrder.Single);
            MolAddBond(mol, 5, 0, BondOrder.Single);

            return mol;

        }

        /// <summary>
        // @cdk.inchi InChI=1/C5H5N5/c6-4-3-5(9-1-7-3)10-2-8-4/h1-2H,(H3,6,7,8,9,10)/f/h7H,6H2
        /// </summary>
        public static IAtomContainer MakeAdenine()
        {
            IAtomContainer mol = new AtomContainer(); // Adenine
            IAtom a1 = mol.Builder.CreateAtom("C");
            a1.Point2D = new Vector2(21.0223, -17.2946);
            mol.Add(a1);
            IAtom a2 = mol.Builder.CreateAtom("C");
            a2.Point2D = new Vector2(21.0223, -18.8093);
            mol.Add(a2);
            IAtom a3 = mol.Builder.CreateAtom("C");
            a3.Point2D = new Vector2(22.1861, -16.6103);
            mol.Add(a3);
            IAtom a4 = mol.Builder.CreateAtom("N");
            a4.Point2D = new Vector2(19.8294, -16.8677);
            mol.Add(a4);
            IAtom a5 = mol.Builder.CreateAtom("N");
            a5.Point2D = new Vector2(22.2212, -19.5285);
            mol.Add(a5);
            IAtom a6 = mol.Builder.CreateAtom("N");
            a6.Point2D = new Vector2(19.8177, -19.2187);
            mol.Add(a6);
            IAtom a7 = mol.Builder.CreateAtom("N");
            a7.Point2D = new Vector2(23.4669, -17.3531);
            mol.Add(a7);
            IAtom a8 = mol.Builder.CreateAtom("N");
            a8.Point2D = new Vector2(22.1861, -15.2769);
            mol.Add(a8);
            IAtom a9 = mol.Builder.CreateAtom("C");
            a9.Point2D = new Vector2(18.9871, -18.0139);
            mol.Add(a9);
            IAtom a10 = mol.Builder.CreateAtom("C");
            a10.Point2D = new Vector2(23.4609, -18.8267);
            mol.Add(a10);
            IBond b1 = mol.Builder.CreateBond(a1, a2, BondOrder.Double);
            mol.Add(b1);
            IBond b2 = mol.Builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Add(b2);
            IBond b3 = mol.Builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Add(b3);
            IBond b4 = mol.Builder.CreateBond(a2, a5, BondOrder.Single);
            mol.Add(b4);
            IBond b5 = mol.Builder.CreateBond(a2, a6, BondOrder.Single);
            mol.Add(b5);
            IBond b6 = mol.Builder.CreateBond(a3, a7, BondOrder.Double);
            mol.Add(b6);
            IBond b7 = mol.Builder.CreateBond(a3, a8, BondOrder.Single);
            mol.Add(b7);
            IBond b8 = mol.Builder.CreateBond(a4, a9, BondOrder.Double);
            mol.Add(b8);
            IBond b9 = mol.Builder.CreateBond(a5, a10, BondOrder.Double);
            mol.Add(b9);
            IBond b10 = mol.Builder.CreateBond(a6, a9, BondOrder.Single);
            mol.Add(b10);
            IBond b11 = mol.Builder.CreateBond(a7, a10, BondOrder.Single);
            mol.Add(b11);

            return mol;
        }

        /// <summary>
        /// InChI=1/C10H8/c1-2-6-10-8-4-3-7-9(10)5-1/h1-8H
        /// </summary>
        [TestMethod()]
        public static IAtomContainer MakeNaphthalene()
        {
            IChemObjectBuilder builder = ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("C");
            a1.FormalCharge = 0;
            mol.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Add(a5);
            IAtom a6 = builder.CreateAtom("C");
            a6.FormalCharge = 0;
            mol.Add(a6);
            IAtom a7 = builder.CreateAtom("C");
            a7.FormalCharge = 0;
            mol.Add(a7);
            IAtom a8 = builder.CreateAtom("C");
            a8.FormalCharge = 0;
            mol.Add(a8);
            IAtom a9 = builder.CreateAtom("C");
            a9.FormalCharge = 0;
            mol.Add(a9);
            IAtom a10 = builder.CreateAtom("C");
            a10.FormalCharge = 0;
            mol.Add(a10);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Double);
            mol.Add(b1);
            IBond b2 = builder.CreateBond(a2, a3, BondOrder.Single);
            mol.Add(b2);
            IBond b3 = builder.CreateBond(a3, a4, BondOrder.Double);
            mol.Add(b3);
            IBond b4 = builder.CreateBond(a4, a5, BondOrder.Single);
            mol.Add(b4);
            IBond b5 = builder.CreateBond(a5, a6, BondOrder.Double);
            mol.Add(b5);
            IBond b6 = builder.CreateBond(a6, a7, BondOrder.Single);
            mol.Add(b6);
            IBond b7 = builder.CreateBond(a7, a8, BondOrder.Double);
            mol.Add(b7);
            IBond b8 = builder.CreateBond(a3, a8, BondOrder.Single);
            mol.Add(b8);
            IBond b9 = builder.CreateBond(a8, a9, BondOrder.Single);
            mol.Add(b9);
            IBond b10 = builder.CreateBond(a9, a10, BondOrder.Double);
            mol.Add(b10);
            IBond b11 = builder.CreateBond(a1, a10, BondOrder.Single);
            mol.Add(b11);
            return mol;
        }

        /// <summary>
        // @cdk.inchi InChI=1/C14H10/c1-2-6-12-10-14-8-4-3-7-13(14)9-11(12)5-1/h1-10H
        /// </summary>
        [TestMethod()]
        public static IAtomContainer MakeAnthracene()
        {
            IChemObjectBuilder builder = ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("C");
            a1.FormalCharge = 0;
            mol.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Add(a5);
            IAtom a6 = builder.CreateAtom("C");
            a6.FormalCharge = 0;
            mol.Add(a6);
            IAtom a7 = builder.CreateAtom("C");
            a7.FormalCharge = 0;
            mol.Add(a7);
            IAtom a8 = builder.CreateAtom("C");
            a8.FormalCharge = 0;
            mol.Add(a8);
            IAtom a9 = builder.CreateAtom("C");
            a9.FormalCharge = 0;
            mol.Add(a9);
            IAtom a10 = builder.CreateAtom("C");
            a10.FormalCharge = 0;
            mol.Add(a10);
            IAtom a11 = builder.CreateAtom("C");
            a11.FormalCharge = 0;
            mol.Add(a11);
            IAtom a12 = builder.CreateAtom("C");
            a12.FormalCharge = 0;
            mol.Add(a12);
            IAtom a13 = builder.CreateAtom("C");
            a13.FormalCharge = 0;
            mol.Add(a13);
            IAtom a14 = builder.CreateAtom("C");
            a14.FormalCharge = 0;
            mol.Add(a14);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Double);
            mol.Add(b1);
            IBond b2 = builder.CreateBond(a2, a3, BondOrder.Single);
            mol.Add(b2);
            IBond b3 = builder.CreateBond(a3, a4, BondOrder.Double);
            mol.Add(b3);
            IBond b4 = builder.CreateBond(a4, a5, BondOrder.Single);
            mol.Add(b4);
            IBond b5 = builder.CreateBond(a5, a6, BondOrder.Double);
            mol.Add(b5);
            IBond b6 = builder.CreateBond(a6, a7, BondOrder.Single);
            mol.Add(b6);
            IBond b7 = builder.CreateBond(a7, a8, BondOrder.Double);
            mol.Add(b7);
            IBond b8 = builder.CreateBond(a8, a9, BondOrder.Single);
            mol.Add(b8);
            IBond b9 = builder.CreateBond(a9, a10, BondOrder.Double);
            mol.Add(b9);
            IBond b10 = builder.CreateBond(a5, a10, BondOrder.Single);
            mol.Add(b10);
            IBond b11 = builder.CreateBond(a10, a11, BondOrder.Single);
            mol.Add(b11);
            IBond b12 = builder.CreateBond(a11, a12, BondOrder.Double);
            mol.Add(b12);
            IBond b13 = builder.CreateBond(a3, a12, BondOrder.Single);
            mol.Add(b13);
            IBond b14 = builder.CreateBond(a12, a13, BondOrder.Single);
            mol.Add(b14);
            IBond b15 = builder.CreateBond(a13, a14, BondOrder.Double);
            mol.Add(b15);
            IBond b16 = builder.CreateBond(a1, a14, BondOrder.Single);
            mol.Add(b16);
            return mol;
        }

        /// <summary>
        /// octacyclo[17.2.2.2¹,⁴.2⁴,⁷.2⁷,¹⁰.2¹⁰,¹³.2¹³,¹⁶.2¹⁶,¹⁹]pentatriacontane
        // @cdk.inchi InChI=1/C35H56/c1-2-30-6-3-29(1)4-7-31(8-5-29)13-15-33(16-14-31)21-23-35(24-22-33)27-25-34(26-28-35)19-17-32(11-9-30,12-10-30)18-20-34/h1-28H2
        /// </summary>
        [TestMethod()]
        public static IAtomContainer MakeCyclophaneLike()
        {
            IChemObjectBuilder builder = ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("C");
            a1.FormalCharge = 0;
            mol.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Add(a5);
            IAtom a6 = builder.CreateAtom("C");
            a6.FormalCharge = 0;
            mol.Add(a6);
            IAtom a7 = builder.CreateAtom("C");
            a7.FormalCharge = 0;
            mol.Add(a7);
            IAtom a8 = builder.CreateAtom("C");
            a8.FormalCharge = 0;
            mol.Add(a8);
            IAtom a9 = builder.CreateAtom("C");
            a9.FormalCharge = 0;
            mol.Add(a9);
            IAtom a10 = builder.CreateAtom("C");
            a10.FormalCharge = 0;
            mol.Add(a10);
            IAtom a11 = builder.CreateAtom("C");
            a11.FormalCharge = 0;
            mol.Add(a11);
            IAtom a12 = builder.CreateAtom("C");
            a12.FormalCharge = 0;
            mol.Add(a12);
            IAtom a13 = builder.CreateAtom("C");
            a13.FormalCharge = 0;
            mol.Add(a13);
            IAtom a14 = builder.CreateAtom("C");
            a14.FormalCharge = 0;
            mol.Add(a14);
            IAtom a15 = builder.CreateAtom("C");
            a15.FormalCharge = 0;
            mol.Add(a15);
            IAtom a16 = builder.CreateAtom("C");
            a16.FormalCharge = 0;
            mol.Add(a16);
            IAtom a17 = builder.CreateAtom("C");
            a17.FormalCharge = 0;
            mol.Add(a17);
            IAtom a18 = builder.CreateAtom("C");
            a18.FormalCharge = 0;
            mol.Add(a18);
            IAtom a19 = builder.CreateAtom("C");
            a19.FormalCharge = 0;
            mol.Add(a19);
            IAtom a20 = builder.CreateAtom("C");
            a20.FormalCharge = 0;
            mol.Add(a20);
            IAtom a21 = builder.CreateAtom("C");
            a21.FormalCharge = 0;
            mol.Add(a21);
            IAtom a22 = builder.CreateAtom("C");
            a22.FormalCharge = 0;
            mol.Add(a22);
            IAtom a23 = builder.CreateAtom("C");
            a23.FormalCharge = 0;
            mol.Add(a23);
            IAtom a24 = builder.CreateAtom("C");
            a24.FormalCharge = 0;
            mol.Add(a24);
            IAtom a25 = builder.CreateAtom("C");
            a25.FormalCharge = 0;
            mol.Add(a25);
            IAtom a26 = builder.CreateAtom("C");
            a26.FormalCharge = 0;
            mol.Add(a26);
            IAtom a27 = builder.CreateAtom("C");
            a27.FormalCharge = 0;
            mol.Add(a27);
            IAtom a28 = builder.CreateAtom("C");
            a28.FormalCharge = 0;
            mol.Add(a28);
            IAtom a29 = builder.CreateAtom("C");
            a29.FormalCharge = 0;
            mol.Add(a29);
            IAtom a30 = builder.CreateAtom("C");
            a30.FormalCharge = 0;
            mol.Add(a30);
            IAtom a31 = builder.CreateAtom("C");
            a31.FormalCharge = 0;
            mol.Add(a31);
            IAtom a32 = builder.CreateAtom("C");
            a32.FormalCharge = 0;
            mol.Add(a32);
            IAtom a33 = builder.CreateAtom("C");
            a33.FormalCharge = 0;
            mol.Add(a33);
            IAtom a34 = builder.CreateAtom("C");
            a34.FormalCharge = 0;
            mol.Add(a34);
            IAtom a35 = builder.CreateAtom("C");
            a35.FormalCharge = 0;
            mol.Add(a35);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Add(b1);
            IBond b2 = builder.CreateBond(a2, a3, BondOrder.Single);
            mol.Add(b2);
            IBond b3 = builder.CreateBond(a3, a4, BondOrder.Single);
            mol.Add(b3);
            IBond b4 = builder.CreateBond(a4, a5, BondOrder.Single);
            mol.Add(b4);
            IBond b5 = builder.CreateBond(a5, a6, BondOrder.Single);
            mol.Add(b5);
            IBond b6 = builder.CreateBond(a1, a6, BondOrder.Single);
            mol.Add(b6);
            IBond b7 = builder.CreateBond(a6, a7, BondOrder.Single);
            mol.Add(b7);
            IBond b8 = builder.CreateBond(a7, a8, BondOrder.Single);
            mol.Add(b8);
            IBond b9 = builder.CreateBond(a8, a9, BondOrder.Single);
            mol.Add(b9);
            IBond b10 = builder.CreateBond(a9, a10, BondOrder.Single);
            mol.Add(b10);
            IBond b11 = builder.CreateBond(a10, a11, BondOrder.Single);
            mol.Add(b11);
            IBond b12 = builder.CreateBond(a6, a11, BondOrder.Single);
            mol.Add(b12);
            IBond b13 = builder.CreateBond(a9, a12, BondOrder.Single);
            mol.Add(b13);
            IBond b14 = builder.CreateBond(a12, a13, BondOrder.Single);
            mol.Add(b14);
            IBond b15 = builder.CreateBond(a13, a14, BondOrder.Single);
            mol.Add(b15);
            IBond b16 = builder.CreateBond(a14, a15, BondOrder.Single);
            mol.Add(b16);
            IBond b17 = builder.CreateBond(a15, a16, BondOrder.Single);
            mol.Add(b17);
            IBond b18 = builder.CreateBond(a9, a16, BondOrder.Single);
            mol.Add(b18);
            IBond b19 = builder.CreateBond(a14, a17, BondOrder.Single);
            mol.Add(b19);
            IBond b20 = builder.CreateBond(a17, a18, BondOrder.Single);
            mol.Add(b20);
            IBond b21 = builder.CreateBond(a18, a19, BondOrder.Single);
            mol.Add(b21);
            IBond b22 = builder.CreateBond(a19, a20, BondOrder.Single);
            mol.Add(b22);
            IBond b23 = builder.CreateBond(a20, a21, BondOrder.Single);
            mol.Add(b23);
            IBond b24 = builder.CreateBond(a14, a21, BondOrder.Single);
            mol.Add(b24);
            IBond b25 = builder.CreateBond(a19, a22, BondOrder.Single);
            mol.Add(b25);
            IBond b26 = builder.CreateBond(a22, a23, BondOrder.Single);
            mol.Add(b26);
            IBond b27 = builder.CreateBond(a23, a24, BondOrder.Single);
            mol.Add(b27);
            IBond b28 = builder.CreateBond(a24, a25, BondOrder.Single);
            mol.Add(b28);
            IBond b29 = builder.CreateBond(a25, a26, BondOrder.Single);
            mol.Add(b29);
            IBond b30 = builder.CreateBond(a26, a27, BondOrder.Single);
            mol.Add(b30);
            IBond b31 = builder.CreateBond(a27, a28, BondOrder.Single);
            mol.Add(b31);
            IBond b32 = builder.CreateBond(a28, a29, BondOrder.Single);
            mol.Add(b32);
            IBond b33 = builder.CreateBond(a3, a29, BondOrder.Single);
            mol.Add(b33);
            IBond b34 = builder.CreateBond(a27, a30, BondOrder.Single);
            mol.Add(b34);
            IBond b35 = builder.CreateBond(a30, a31, BondOrder.Single);
            mol.Add(b35);
            IBond b36 = builder.CreateBond(a3, a31, BondOrder.Single);
            mol.Add(b36);
            IBond b37 = builder.CreateBond(a27, a32, BondOrder.Single);
            mol.Add(b37);
            IBond b38 = builder.CreateBond(a32, a33, BondOrder.Single);
            mol.Add(b38);
            IBond b39 = builder.CreateBond(a24, a33, BondOrder.Single);
            mol.Add(b39);
            IBond b40 = builder.CreateBond(a24, a34, BondOrder.Single);
            mol.Add(b40);
            IBond b41 = builder.CreateBond(a34, a35, BondOrder.Single);
            mol.Add(b41);
            IBond b42 = builder.CreateBond(a19, a35, BondOrder.Single);
            mol.Add(b42);
            return mol;
        }

        /// <summary>
        /// octacyclo[24.2.2.2²,⁵.2⁶,⁹.2¹⁰,¹³.2¹⁴,¹⁷.2¹⁸,²¹.2²²,²⁵]dotetracontane
        // @cdk.inchi InChI=1/C42H70/c1-2-30-4-3-29(1)31-5-7-33(8-6-31)35-13-15-37(16-14-35)39-21-23-41(24-22-39)42-27-25-40(26-28-42)38-19-17-36(18-20-38)34-11-9-32(30)10-12-34/h29-42H,1-28H2
        /// </summary>
        [TestMethod()]
        public static IAtomContainer MakeGappedCyclophaneLike()
        {
            IChemObjectBuilder builder = ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("C");
            a1.FormalCharge = 0;
            mol.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Add(a5);
            IAtom a6 = builder.CreateAtom("C");
            a6.FormalCharge = 0;
            mol.Add(a6);
            IAtom a7 = builder.CreateAtom("C");
            a7.FormalCharge = 0;
            mol.Add(a7);
            IAtom a8 = builder.CreateAtom("C");
            a8.FormalCharge = 0;
            mol.Add(a8);
            IAtom a9 = builder.CreateAtom("C");
            a9.FormalCharge = 0;
            mol.Add(a9);
            IAtom a10 = builder.CreateAtom("C");
            a10.FormalCharge = 0;
            mol.Add(a10);
            IAtom a11 = builder.CreateAtom("C");
            a11.FormalCharge = 0;
            mol.Add(a11);
            IAtom a12 = builder.CreateAtom("C");
            a12.FormalCharge = 0;
            mol.Add(a12);
            IAtom a13 = builder.CreateAtom("C");
            a13.FormalCharge = 0;
            mol.Add(a13);
            IAtom a14 = builder.CreateAtom("C");
            a14.FormalCharge = 0;
            mol.Add(a14);
            IAtom a15 = builder.CreateAtom("C");
            a15.FormalCharge = 0;
            mol.Add(a15);
            IAtom a16 = builder.CreateAtom("C");
            a16.FormalCharge = 0;
            mol.Add(a16);
            IAtom a17 = builder.CreateAtom("C");
            a17.FormalCharge = 0;
            mol.Add(a17);
            IAtom a18 = builder.CreateAtom("C");
            a18.FormalCharge = 0;
            mol.Add(a18);
            IAtom a19 = builder.CreateAtom("C");
            a19.FormalCharge = 0;
            mol.Add(a19);
            IAtom a20 = builder.CreateAtom("C");
            a20.FormalCharge = 0;
            mol.Add(a20);
            IAtom a21 = builder.CreateAtom("C");
            a21.FormalCharge = 0;
            mol.Add(a21);
            IAtom a22 = builder.CreateAtom("C");
            a22.FormalCharge = 0;
            mol.Add(a22);
            IAtom a23 = builder.CreateAtom("C");
            a23.FormalCharge = 0;
            mol.Add(a23);
            IAtom a24 = builder.CreateAtom("C");
            a24.FormalCharge = 0;
            mol.Add(a24);
            IAtom a25 = builder.CreateAtom("C");
            a25.FormalCharge = 0;
            mol.Add(a25);
            IAtom a26 = builder.CreateAtom("C");
            a26.FormalCharge = 0;
            mol.Add(a26);
            IAtom a27 = builder.CreateAtom("C");
            a27.FormalCharge = 0;
            mol.Add(a27);
            IAtom a28 = builder.CreateAtom("C");
            a28.FormalCharge = 0;
            mol.Add(a28);
            IAtom a29 = builder.CreateAtom("C");
            a29.FormalCharge = 0;
            mol.Add(a29);
            IAtom a30 = builder.CreateAtom("C");
            a30.FormalCharge = 0;
            mol.Add(a30);
            IAtom a31 = builder.CreateAtom("C");
            a31.FormalCharge = 0;
            mol.Add(a31);
            IAtom a32 = builder.CreateAtom("C");
            a32.FormalCharge = 0;
            mol.Add(a32);
            IAtom a33 = builder.CreateAtom("C");
            a33.FormalCharge = 0;
            mol.Add(a33);
            IAtom a34 = builder.CreateAtom("C");
            a34.FormalCharge = 0;
            mol.Add(a34);
            IAtom a35 = builder.CreateAtom("C");
            a35.FormalCharge = 0;
            mol.Add(a35);
            IAtom a36 = builder.CreateAtom("C");
            a36.FormalCharge = 0;
            mol.Add(a36);
            IAtom a37 = builder.CreateAtom("C");
            a37.FormalCharge = 0;
            mol.Add(a37);
            IAtom a38 = builder.CreateAtom("C");
            a38.FormalCharge = 0;
            mol.Add(a38);
            IAtom a39 = builder.CreateAtom("C");
            a39.FormalCharge = 0;
            mol.Add(a39);
            IAtom a40 = builder.CreateAtom("C");
            a40.FormalCharge = 0;
            mol.Add(a40);
            IAtom a41 = builder.CreateAtom("C");
            a41.FormalCharge = 0;
            mol.Add(a41);
            IAtom a42 = builder.CreateAtom("C");
            a42.FormalCharge = 0;
            mol.Add(a42);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Add(b1);
            IBond b2 = builder.CreateBond(a2, a3, BondOrder.Single);
            mol.Add(b2);
            IBond b3 = builder.CreateBond(a3, a4, BondOrder.Single);
            mol.Add(b3);
            IBond b4 = builder.CreateBond(a4, a5, BondOrder.Single);
            mol.Add(b4);
            IBond b5 = builder.CreateBond(a5, a6, BondOrder.Single);
            mol.Add(b5);
            IBond b6 = builder.CreateBond(a1, a6, BondOrder.Single);
            mol.Add(b6);
            IBond b7 = builder.CreateBond(a6, a7, BondOrder.Single);
            mol.Add(b7);
            IBond b8 = builder.CreateBond(a7, a8, BondOrder.Single);
            mol.Add(b8);
            IBond b9 = builder.CreateBond(a8, a9, BondOrder.Single);
            mol.Add(b9);
            IBond b10 = builder.CreateBond(a9, a10, BondOrder.Single);
            mol.Add(b10);
            IBond b11 = builder.CreateBond(a10, a11, BondOrder.Single);
            mol.Add(b11);
            IBond b12 = builder.CreateBond(a11, a12, BondOrder.Single);
            mol.Add(b12);
            IBond b13 = builder.CreateBond(a7, a12, BondOrder.Single);
            mol.Add(b13);
            IBond b14 = builder.CreateBond(a10, a13, BondOrder.Single);
            mol.Add(b14);
            IBond b15 = builder.CreateBond(a13, a14, BondOrder.Single);
            mol.Add(b15);
            IBond b16 = builder.CreateBond(a14, a15, BondOrder.Single);
            mol.Add(b16);
            IBond b17 = builder.CreateBond(a15, a16, BondOrder.Single);
            mol.Add(b17);
            IBond b18 = builder.CreateBond(a16, a17, BondOrder.Single);
            mol.Add(b18);
            IBond b19 = builder.CreateBond(a17, a18, BondOrder.Single);
            mol.Add(b19);
            IBond b20 = builder.CreateBond(a13, a18, BondOrder.Single);
            mol.Add(b20);
            IBond b21 = builder.CreateBond(a16, a19, BondOrder.Single);
            mol.Add(b21);
            IBond b22 = builder.CreateBond(a19, a20, BondOrder.Single);
            mol.Add(b22);
            IBond b23 = builder.CreateBond(a20, a21, BondOrder.Single);
            mol.Add(b23);
            IBond b24 = builder.CreateBond(a21, a22, BondOrder.Single);
            mol.Add(b24);
            IBond b25 = builder.CreateBond(a22, a23, BondOrder.Single);
            mol.Add(b25);
            IBond b26 = builder.CreateBond(a23, a24, BondOrder.Single);
            mol.Add(b26);
            IBond b27 = builder.CreateBond(a19, a24, BondOrder.Single);
            mol.Add(b27);
            IBond b28 = builder.CreateBond(a22, a25, BondOrder.Single);
            mol.Add(b28);
            IBond b29 = builder.CreateBond(a25, a26, BondOrder.Single);
            mol.Add(b29);
            IBond b30 = builder.CreateBond(a26, a27, BondOrder.Single);
            mol.Add(b30);
            IBond b31 = builder.CreateBond(a27, a28, BondOrder.Single);
            mol.Add(b31);
            IBond b32 = builder.CreateBond(a28, a29, BondOrder.Single);
            mol.Add(b32);
            IBond b33 = builder.CreateBond(a29, a30, BondOrder.Single);
            mol.Add(b33);
            IBond b34 = builder.CreateBond(a25, a30, BondOrder.Single);
            mol.Add(b34);
            IBond b35 = builder.CreateBond(a28, a31, BondOrder.Single);
            mol.Add(b35);
            IBond b36 = builder.CreateBond(a31, a32, BondOrder.Single);
            mol.Add(b36);
            IBond b37 = builder.CreateBond(a32, a33, BondOrder.Single);
            mol.Add(b37);
            IBond b38 = builder.CreateBond(a33, a34, BondOrder.Single);
            mol.Add(b38);
            IBond b39 = builder.CreateBond(a34, a35, BondOrder.Single);
            mol.Add(b39);
            IBond b40 = builder.CreateBond(a35, a36, BondOrder.Single);
            mol.Add(b40);
            IBond b41 = builder.CreateBond(a31, a36, BondOrder.Single);
            mol.Add(b41);
            IBond b42 = builder.CreateBond(a34, a37, BondOrder.Single);
            mol.Add(b42);
            IBond b43 = builder.CreateBond(a37, a38, BondOrder.Single);
            mol.Add(b43);
            IBond b44 = builder.CreateBond(a38, a39, BondOrder.Single);
            mol.Add(b44);
            IBond b45 = builder.CreateBond(a39, a40, BondOrder.Single);
            mol.Add(b45);
            IBond b46 = builder.CreateBond(a3, a40, BondOrder.Single);
            mol.Add(b46);
            IBond b47 = builder.CreateBond(a40, a41, BondOrder.Single);
            mol.Add(b47);
            IBond b48 = builder.CreateBond(a41, a42, BondOrder.Single);
            mol.Add(b48);
            IBond b49 = builder.CreateBond(a37, a42, BondOrder.Single);
            mol.Add(b49);
            return mol;
        }

        private static void ConfigureAtoms(IAtomContainer mol)
        {
            try
            {
                Isotopes.Instance.ConfigureAtoms(mol);
            }
            catch (Exception exc)
            {
                Trace.TraceError($"Could not configure molecule! {exc.Message}");
            }
        }

    }
}
