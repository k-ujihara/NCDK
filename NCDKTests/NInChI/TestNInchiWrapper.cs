/*
 * Copyright 2006-2011 Sam Adams <sea36 at users.sourceforge.net>
 *
 * This file is part of JNI-InChI.
 *
 * JNI-InChI is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * JNI-InChI is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with JNI-InChI.  If not, see <http://www.gnu.org/licenses/>.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;

namespace NCDK.NInChI
{
    [TestClass()]
    public class TestNInchiWrapper
    {

        // Test molecules

        /// <summary>
        /// Generates input for a chlorine atom.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static NInchiInput getChlorineAtom(string options)
        {
            NInchiInput input = new NInchiInput(options);

            // Generate atoms
            input.Add(new NInchiAtom(0.000, 0.000, 0.000, "Cl"));
            input.Atoms[0].ImplicitH = 0;
            return input;
        }

        /// <summary>
        /// Generates input for a chlorine atom.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static NInchiInput getChlorineIon(string options)
        {
            NInchiInput input = new NInchiInput(options);

            // Generate atoms
            NInchiAtom a1 = input.Add(new NInchiAtom(0.000, 0.000, 0.000,
                    "Cl"));
            a1.Charge = -1;
            input.Atoms[0].Radical = INCHI_RADICAL.Singlet;

            return input;
        }

        /// <summary>
        /// Generates input for hydrogen chloride, with implicit H atom.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static NInchiInput getHydrogenChlorideImplicitH(string options)
        {
            NInchiInput input = new NInchiInput(options);

            // Generate atoms
            NInchiAtom a1 = input.Add(new NInchiAtom(0.000, 0.000, 0.000,
                    "Cl"));
            a1.ImplicitH = 1;

            return input;
        }

        /// <summary>
        /// Generates input for hydrogen chloride, with implicit protium atom.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static NInchiInput getHydrogenChlorideImplicitP(
                 string options)
        {
            NInchiInput input = new NInchiInput(options);

            // Generate atoms
            NInchiAtom a1 = input.Add(new NInchiAtom(0.000, 0.000, 0.000,
                    "Cl"));
            a1.ImplicitProtium = 1;

            return input;
        }

        /// <summary>
        /// Generates input for hydrogen chloride, with implicit deuterium atom.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static NInchiInput getHydrogenChlorideImplicitD(
                 string options)
        {
            NInchiInput input = new NInchiInput(options);

            // Generate atoms
            NInchiAtom a1 = input.Add(new NInchiAtom(0.000, 0.000, 0.000, "Cl"));
            a1.ImplicitDeuterium = 1;

            return input;
        }

        /// <summary>
        /// Generates input for hydrogen chloride, with implicit tritium atom.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static NInchiInput getHydrogenChlorideImplicitT(string options)
        {
            NInchiInput input = new NInchiInput(options);

            // Generate atoms
            NInchiAtom a1 = input.Add(new NInchiAtom(0.000, 0.000, 0.000, "Cl"));
            a1.ImplicitTritium = 1;

            return input;
        }

        /// <summary>
        /// Generates input for a 37Cl atom by isotopic mass.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static NInchiInput getChlorine37Atom(string options)
        {
            NInchiInput input = new NInchiInput(options);

            // Generate atoms
            NInchiAtom a1 = input.Add(new NInchiAtom(0.000, 0.000, 0.000, "Cl"));
            a1.IsotopicMass = 37;
            a1.ImplicitH = 0;

            return input;
        }

        /// <summary>
        /// Generates input for a 37Cl atom by isotopic mass shift.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static NInchiInput getChlorine37ByIsotopicMassShiftAtom(
                 string options)
        {
            NInchiInput input = new NInchiInput(options);

            // Generate atoms
            NInchiAtom a1 = input.Add(new NInchiAtom(0.000, 0.000, 0.000,
                    "Cl"));
            a1.SetIsotopicMassShift(+2);
            input.Atoms[0].ImplicitH = 0;

            return input;
        }

        /// <summary>
        /// Generates input for a methyl radical, with implicit hydrogens.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static NInchiInput getMethylRadical(string options)
        {
            NInchiInput input = new NInchiInput(options);

            // Generate atoms
            NInchiAtom a1 = input.Add(new NInchiAtom(0.000, 0.000, 0.000,
                    "C"));
            a1.ImplicitH = 3;
            a1.Radical = INCHI_RADICAL.Doublet;

            return input;
        }


        private static NInchiInput getSodiumHydroxide(string options)
        {
            NInchiInput input = new NInchiInput(options);

            // Generate atoms
            NInchiAtom a0 = new NInchiAtom(0.000, 0.000, 0.000, "Na");
            input.Add(a0);
            NInchiAtom a1 = new NInchiAtom(0.000, 0.000, 0.000, "O");
            a1.ImplicitH = 1;
            input.Add(a1);

            input.Bonds.Add(new NInchiBond(a0, a1, INCHI_BOND_TYPE.Single));

            return input;
        }

        /// <summary>
        /// Generates input for an ethane molecule, with no coordinates and implicit
        /// hydrogens.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static NInchiInput getEthane(string options)
        {
            NInchiInput input = new NInchiInput(options);

            // Generate atoms
            NInchiAtom a1 = input.Add(new NInchiAtom(0.000, 0.000, 0.000,
                    "C"));
            NInchiAtom a2 = input.Add(new NInchiAtom(0.000, 0.000, 0.000,
                    "C"));
            a1.ImplicitH = 3;
            a2.ImplicitH = 3;

            // Add bond
            input.Bonds.Add(new NInchiBond(a1, a2, INCHI_BOND_TYPE.Single));

            return input;
        }

        /// <summary>
        /// Generates input for an ethene molecule, with no coordinates and implicit
        /// hydrogens.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static NInchiInput getEthene(string options)
        {
            NInchiInput input = new NInchiInput(options);

            // Generate atoms
            NInchiAtom a1 = input.Add(new NInchiAtom(0.000, 0.000, 0.000,
                    "C"));
            NInchiAtom a2 = input.Add(new NInchiAtom(0.000, 0.000, 0.000,
                    "C"));
            a1.ImplicitH = 2;
            a2.ImplicitH = 2;

            // Add bond
            input.Bonds.Add(new NInchiBond(a1, a2, INCHI_BOND_TYPE.Double));

            return input;
        }

        /// <summary>
        /// Generates input for an ethyne molecule, with no coordinates and implicit
        /// hydrogens.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static NInchiInput getEthyne(string options)
        {
            NInchiInput input = new NInchiInput(options);

            // Generate atoms
            NInchiAtom a1 = input.Add(new NInchiAtom(0.000, 0.000, 0.000,
                    "C"));
            NInchiAtom a2 = input.Add(new NInchiAtom(0.000, 0.000, 0.000,
                    "C"));
            a1.ImplicitH = 1;
            a2.ImplicitH = 1;

            // Add bond
            input.Bonds.Add(new NInchiBond(a1, a2, INCHI_BOND_TYPE.Triple));

            return input;
        }

        /// <summary>
        /// Generates input for an (E)-1,2-dichloroethene molecule, with 2D
        /// coordinates and implicit hydrogens.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static NInchiInput getE12dichloroethene2D(string options)
        {
            NInchiInput input = new NInchiInput(options);

            // Generate atoms
            NInchiAtom a1 = input.Add(new NInchiAtom(2.866, -0.250, 0.000,
                    "C"));
            NInchiAtom a2 = input.Add(new NInchiAtom(3.732, 0.250, 0.000,
                    "C"));
            NInchiAtom a3 = input.Add(new NInchiAtom(2.000, 2.500, 0.000,
                    "Cl"));
            NInchiAtom a4 = input.Add(new NInchiAtom(4.598, -0.250, 0.000,
                    "Cl"));
            a1.ImplicitH = 1;
            a2.ImplicitH = 1;

            // Add bond
            input.Bonds.Add(new NInchiBond(a1, a2, INCHI_BOND_TYPE.Double));
            input.Bonds.Add(new NInchiBond(a1, a3, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a2, a4, INCHI_BOND_TYPE.Single));

            return input;
        }

        /// <summary>
        /// Generates input for an (E)-1,2-dichloroethene molecule, with 2D
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static NInchiInput getZ12dichloroethene2D(string options)
        {
            NInchiInput input = new NInchiInput(options);

            // Generate atoms
            NInchiAtom a1 = input.Add(new NInchiAtom(2.866, -0.440, 0.000,
                    "C"));
            NInchiAtom a2 = input.Add(new NInchiAtom(3.732, 0.060, 0.000,
                    "C"));
            NInchiAtom a3 = input.Add(new NInchiAtom(2.000, 0.060, 0.000,
                    "Cl"));
            NInchiAtom a4 = input.Add(new NInchiAtom(3.732, 1.060, 0.000,
                    "Cl"));
            a1.ImplicitH = 1;
            a2.ImplicitH = 1;

            // Add bond
            input.Bonds.Add(new NInchiBond(a1, a2, INCHI_BOND_TYPE.Double));
            input.Bonds.Add(new NInchiBond(a1, a3, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a2, a4, INCHI_BOND_TYPE.Single));

            return input;
        }

        /// <summary>
        /// Generates input for an (E)-1,2-dichloroethene molecule, with 0D
        /// coordinates.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static NInchiInput get12dichloroethene0D(string options)
        {
            NInchiInput input = new NInchiInput(options);

            // Generate atoms
            NInchiAtom a1 = input.Add(new NInchiAtom(0.000, 0.000, 0.000,
                    "C"));
            NInchiAtom a2 = input.Add(new NInchiAtom(0.000, 0.000, 0.000,
                    "C"));
            NInchiAtom a3 = input.Add(new NInchiAtom(0.000, 0.000, 0.000,
                    "Cl"));
            NInchiAtom a4 = input.Add(new NInchiAtom(0.000, 0.000, 0.000,
                    "Cl"));
            a1.ImplicitH = 1;
            a2.ImplicitH = 1;

            // Add bond
            input.Bonds.Add(new NInchiBond(a1, a2, INCHI_BOND_TYPE.Double));
            input.Bonds.Add(new NInchiBond(a1, a3, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a2, a4, INCHI_BOND_TYPE.Single));

            return input;
        }

        /// <summary>
        /// Generates input for an (E)-1,2-dichloroethene molecule, with 0D
        /// coordinates and stereo parities.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static NInchiInput getE12dichloroethene0D(string options)
        {
            NInchiInput input = new NInchiInput(options);

            // Generate atoms
            NInchiAtom a1 = input.Add(new NInchiAtom(0.000, 0.000, 0.000,
                    "C"));
            NInchiAtom a2 = input.Add(new NInchiAtom(0.000, 0.000, 0.000,
                    "C"));
            NInchiAtom a3 = input.Add(new NInchiAtom(0.000, 0.000, 0.000,
                    "Cl"));
            NInchiAtom a4 = input.Add(new NInchiAtom(0.000, 0.000, 0.000,
                    "Cl"));
            a1.ImplicitH = 1;
            a2.ImplicitH = 1;

            // Add bond
            input.Bonds.Add(new NInchiBond(a1, a2, INCHI_BOND_TYPE.Double));
            input.Bonds.Add(new NInchiBond(a1, a3, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a2, a4, INCHI_BOND_TYPE.Single));

            // Add stereo parities
            input.Stereos.Add(NInchiStereo0D.CreateNewDoublebondStereo0D(a3, a1,
                    a2, a4, INCHI_PARITY.Even));

            return input;
        }

        /// <summary>
        /// Generates input for an (E)-1,2-dichloroethene molecule, with 2D
        /// coordinates and stereo parities.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static NInchiInput getZ12dichloroethene0D(string options)
        {
            NInchiInput input = new NInchiInput(options);

            // Generate atoms
            NInchiAtom a1 = input.Add(new NInchiAtom(0.000, 0.000, 0.000,
                    "C"));
            NInchiAtom a2 = input.Add(new NInchiAtom(0.000, 0.000, 0.000,
                    "C"));
            NInchiAtom a3 = input.Add(new NInchiAtom(0.000, 0.000, 0.000,
                    "Cl"));
            NInchiAtom a4 = input.Add(new NInchiAtom(0.000, 0.000, 0.000,
                    "Cl"));
            a1.ImplicitH = 1;
            a2.ImplicitH = 1;

            // Add bond
            input.Bonds.Add(new NInchiBond(a1, a2, INCHI_BOND_TYPE.Double));
            input.Bonds.Add(new NInchiBond(a1, a3, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a2, a4, INCHI_BOND_TYPE.Single));

            // Add stereo parities
            input.Stereos.Add(NInchiStereo0D.CreateNewDoublebondStereo0D(a3, a1,
                    a2, a4, INCHI_PARITY.Odd));

            return input;
        }

        /// <summary>
        /// Generates input for L-alanine molecule, with 3D coordinates and implicit
        /// hydrogens.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static NInchiInput getLAlanine3D(string options)
        {
            NInchiInput input = new NInchiInput(options);

            // Generate atoms
            NInchiAtom a1 = input.Add(new NInchiAtom(-0.358, 0.819, 20.655,
                    "C"));
            NInchiAtom a2 = input.Add(new NInchiAtom(-1.598, -0.032,
                    20.905, "C"));
            NInchiAtom a3 = input.Add(new NInchiAtom(-0.275, 2.014, 21.574,
                    "N"));
            NInchiAtom a4 = input.Add(new NInchiAtom(0.952, 0.043, 20.838,
                    "C"));
            NInchiAtom a5 = input.Add(new NInchiAtom(-2.678, 0.479, 21.093,
                    "O"));
            NInchiAtom a6 = input.Add(new NInchiAtom(-1.596, -1.239,
                    20.958, "O"));

            a1.ImplicitH = 1;
            a3.ImplicitH = 2;
            a4.ImplicitH = 3;
            a5.ImplicitH = 1;

            // Add bonds
            input.Bonds.Add(new NInchiBond(a1, a2, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a1, a3, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a1, a4, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a2, a5, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a2, a6, INCHI_BOND_TYPE.Double));

            return input;
        }

        /// <summary>
        /// Generates input for D-alanine molecule, with 3D coordinates and implicit
        /// hydrogens.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static NInchiInput getDAlanine3D(string options)
        {
            NInchiInput input = new NInchiInput(options);

            // Generate atoms
            NInchiAtom a1 = input.Add(new NInchiAtom(0.358, 0.819, 20.655,
                    "C"));
            NInchiAtom a2 = input.Add(new NInchiAtom(1.598, -0.032, 20.905,
                    "C"));
            NInchiAtom a3 = input.Add(new NInchiAtom(0.275, 2.014, 21.574,
                    "N"));
            NInchiAtom a4 = input.Add(new NInchiAtom(-0.952, 0.043, 20.838,
                    "C"));
            NInchiAtom a5 = input.Add(new NInchiAtom(2.678, 0.479, 21.093,
                    "O"));
            NInchiAtom a6 = input.Add(new NInchiAtom(1.596, -1.239, 20.958,
                    "O"));

            a1.ImplicitH = 1;
            a3.ImplicitH = 2;
            a4.ImplicitH = 3;
            a5.ImplicitH = 1;

            // Add bonds
            input.Bonds.Add(new NInchiBond(a1, a2, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a1, a3, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a1, a4, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a2, a5, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a2, a6, INCHI_BOND_TYPE.Double));

            return input;
        }

        /// <summary>
        /// Generates input for alanine molecule, with 2D coordinates.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static NInchiInput getAlanine2D(string options)
        {
            NInchiInput input = new NInchiInput(options);

            // Generate atoms
            NInchiAtom a1 = input
                    .Add(new NInchiAtom(264.0, 968.0, 0.0, "C"));
            NInchiAtom a2 = input
                    .Add(new NInchiAtom(295.0, 985.0, 0.0, "C"));
            NInchiAtom a3 = input
                    .Add(new NInchiAtom(233.0, 986.0, 0.0, "N"));
            NInchiAtom a4 = input
                    .Add(new NInchiAtom(264.0, 932.0, 0.0, "C"));
            NInchiAtom a5 = input
                    .Add(new NInchiAtom(326.0, 967.0, 0.0, "O"));
            NInchiAtom a6 = input.Add(new NInchiAtom(295.0, 1021.0, 0.0,
                    "O"));

            a1.ImplicitH = 1;
            a3.ImplicitH = 2;
            a4.ImplicitH = 3;
            a5.ImplicitH = 1;

            // Add bonds
            input.Bonds.Add(new NInchiBond(a1, a2, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a1, a3, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a1, a4, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a2, a5, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a2, a6, INCHI_BOND_TYPE.Double));

            return input;
        }

        /// <summary>
        /// Generates input for L-alanine molecule, with 2D coordinates and bond
        /// stereo definitions.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static NInchiInput getLAlanine2Da(string options)
        {
            NInchiInput input = new NInchiInput(options);

            // Generate atoms
            NInchiAtom a1 = input
                    .Add(new NInchiAtom(264.0, 968.0, 0.0, "C"));
            NInchiAtom a2 = input
                    .Add(new NInchiAtom(295.0, 985.0, 0.0, "C"));
            NInchiAtom a3 = input
                    .Add(new NInchiAtom(233.0, 986.0, 0.0, "N"));
            NInchiAtom a4 = input
                    .Add(new NInchiAtom(264.0, 932.0, 0.0, "C"));
            NInchiAtom a5 = input
                    .Add(new NInchiAtom(326.0, 967.0, 0.0, "O"));
            NInchiAtom a6 = input.Add(new NInchiAtom(295.0, 1021.0, 0.0,
                    "O"));

            a1.ImplicitH = 1;
            a3.ImplicitH = 2;
            a4.ImplicitH = 3;
            a5.ImplicitH = 1;

            // Add bonds
            input.Bonds.Add(new NInchiBond(a1, a2, INCHI_BOND_TYPE.Single));
            input.Add(new NInchiBond(a1, a3, INCHI_BOND_TYPE.Single))
                    .BondStereo = INCHI_BOND_STEREO.Single1Down;
            input.Bonds.Add(new NInchiBond(a1, a4, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a2, a5, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a2, a6, INCHI_BOND_TYPE.Double));

            return input;
        }


        private static NInchiInput getLAlanine2Db(string options)
        {
            NInchiInput input = new NInchiInput(options);

            // Generate atoms
            NInchiAtom a1 = input
                    .Add(new NInchiAtom(264.0, 968.0, 0.0, "C"));
            NInchiAtom a2 = input
                    .Add(new NInchiAtom(295.0, 985.0, 0.0, "C"));
            NInchiAtom a3 = input
                    .Add(new NInchiAtom(233.0, 986.0, 0.0, "N"));
            NInchiAtom a4 = input
                    .Add(new NInchiAtom(264.0, 932.0, 0.0, "C"));
            NInchiAtom a5 = input
                    .Add(new NInchiAtom(326.0, 967.0, 0.0, "O"));
            NInchiAtom a6 = input.Add(new NInchiAtom(295.0, 1021.0, 0.0,
                    "O"));

            a1.ImplicitH = 1;
            a3.ImplicitH = 2;
            a4.ImplicitH = 3;
            a5.ImplicitH = 1;

            // Add bonds
            input.Bonds.Add(new NInchiBond(a1, a2, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a3, a1, INCHI_BOND_TYPE.Single));
            input.Add(new NInchiBond(a1, a4, INCHI_BOND_TYPE.Single))
                    .BondStereo = INCHI_BOND_STEREO.Single1Down;
            input.Bonds.Add(new NInchiBond(a2, a5, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a2, a6, INCHI_BOND_TYPE.Double));

            return input;
        }

        /// <summary>
        /// Generates input for D-alanine molecule, with 2D coordinates and bond
        /// stereo definitions.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static NInchiInput getDAlanine2D(string options)
        {
            NInchiInput input = new NInchiInput(options);

            // Generate atoms
            NInchiAtom a1 = input
                    .Add(new NInchiAtom(264.0, 968.0, 0.0, "C"));
            NInchiAtom a2 = input
                    .Add(new NInchiAtom(295.0, 985.0, 0.0, "C"));
            NInchiAtom a3 = input
                    .Add(new NInchiAtom(233.0, 986.0, 0.0, "N"));
            NInchiAtom a4 = input
                    .Add(new NInchiAtom(264.0, 932.0, 0.0, "C"));
            NInchiAtom a5 = input
                    .Add(new NInchiAtom(326.0, 967.0, 0.0, "O"));
            NInchiAtom a6 = input.Add(new NInchiAtom(295.0, 1021.0, 0.0,
                    "O"));

            a1.ImplicitH = 1;
            a3.ImplicitH = 2;
            a4.ImplicitH = 3;
            a5.ImplicitH = 1;

            // Add bonds
            input.Bonds.Add(new NInchiBond(a1, a2, INCHI_BOND_TYPE.Single));
            input.Add(new NInchiBond(a1, a3, INCHI_BOND_TYPE.Single))
                    .BondStereo = INCHI_BOND_STEREO.Single1Up;
            input.Bonds.Add(new NInchiBond(a1, a4, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a2, a5, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a2, a6, INCHI_BOND_TYPE.Double));

            return input;
        }

        /// <summary>
        /// Generates input for alanine molecule with no coordinates.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static NInchiInput getAlanine0D(string options)
        {
            NInchiInput input = new NInchiInput(options);

            // Generate atoms
            NInchiAtom a0 = input.Add(new NInchiAtom(0.0, 0.0, 0.0, "C"));
            NInchiAtom a1 = input.Add(new NInchiAtom(0.0, 0.0, 0.0, "C"));
            NInchiAtom a2 = input.Add(new NInchiAtom(0.0, 0.0, 0.0, "N"));
            NInchiAtom a3 = input.Add(new NInchiAtom(0.0, 0.0, 0.0, "C"));
            NInchiAtom a4 = input.Add(new NInchiAtom(0.0, 0.0, 0.0, "O"));
            NInchiAtom a5 = input.Add(new NInchiAtom(0.0, 0.0, 0.0, "O"));
            NInchiAtom a6 = input.Add(new NInchiAtom(0.0, 0.0, 0.0, "H"));
            a2.ImplicitH = 2;
            a3.ImplicitH = 3;
            a4.ImplicitH = 1;

            // Add bonds
            input.Bonds.Add(new NInchiBond(a0, a1, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a0, a2, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a0, a3, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a1, a4, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a1, a5, INCHI_BOND_TYPE.Double));
            input.Bonds.Add(new NInchiBond(a0, a6, INCHI_BOND_TYPE.Single));

            return input;
        }

        /// <summary>
        /// Generates input for L-alanine molecule with no coordinates but 0D stereo
        /// parities.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static NInchiInput getLAlanine0D(string options)
        {
            NInchiInput input = new NInchiInput(options);

            // Generate atoms
            NInchiAtom a1 = input.Add(new NInchiAtom(0.0, 0.0, 0.0, "C"));
            NInchiAtom a2 = input.Add(new NInchiAtom(0.0, 0.0, 0.0, "C"));
            NInchiAtom a3 = input.Add(new NInchiAtom(0.0, 0.0, 0.0, "N"));
            NInchiAtom a4 = input.Add(new NInchiAtom(0.0, 0.0, 0.0, "C"));
            NInchiAtom a5 = input.Add(new NInchiAtom(0.0, 0.0, 0.0, "O"));
            NInchiAtom a6 = input.Add(new NInchiAtom(0.0, 0.0, 0.0, "O"));
            NInchiAtom a7 = input.Add(new NInchiAtom(0.0, 0.0, 0.0, "H"));
            a3.ImplicitH = 2;
            a4.ImplicitH = 3;
            a5.ImplicitH = 1;

            // Add bonds
            input.Bonds.Add(new NInchiBond(a1, a2, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a1, a3, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a1, a4, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a2, a5, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a2, a6, INCHI_BOND_TYPE.Double));
            input.Bonds.Add(new NInchiBond(a1, a7, INCHI_BOND_TYPE.Single));

            // Add stereo parities
            input.Stereos.Add(NInchiStereo0D.CreateNewTetrahedralStereo0D(a1, a3,
                    a4, a7, a2, INCHI_PARITY.Odd));

            return input;
        }

        /// <summary>
        /// Generates input for D-alanine molecule with no coordinates but 0D stereo
        /// parities.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static NInchiInput getDAlanine0D(string options)
        {
            NInchiInput input = new NInchiInput(options);

            // Generate atoms
            NInchiAtom a1 = input.Add(new NInchiAtom(0.0, 0.0, 0.0, "C"));
            NInchiAtom a2 = input.Add(new NInchiAtom(0.0, 0.0, 0.0, "C"));
            NInchiAtom a3 = input.Add(new NInchiAtom(0.0, 0.0, 0.0, "N"));
            NInchiAtom a4 = input.Add(new NInchiAtom(0.0, 0.0, 0.0, "C"));
            NInchiAtom a5 = input.Add(new NInchiAtom(0.0, 0.0, 0.0, "O"));
            NInchiAtom a6 = input.Add(new NInchiAtom(0.0, 0.0, 0.0, "O"));
            NInchiAtom a7 = input.Add(new NInchiAtom(0.0, 0.0, 0.0, "H"));
            a3.ImplicitH = 2;
            a4.ImplicitH = 3;
            a5.ImplicitH = 1;

            // Add bonds
            input.Bonds.Add(new NInchiBond(a1, a2, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a1, a3, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a1, a4, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a2, a5, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a2, a6, INCHI_BOND_TYPE.Double));
            input.Bonds.Add(new NInchiBond(a1, a7, INCHI_BOND_TYPE.Single));

            // Add stereo parities
            input.Stereos.Add(NInchiStereo0D.CreateNewTetrahedralStereo0D(a1, a3,
                    a4, a7, a2, INCHI_PARITY.Even));
            return input;
        }


        private NInchiInput getNSC7414a(string options)
        {

            NInchiInput input = new NInchiInput(options);

            // Generate atoms
            NInchiAtom a1 = input.Add(new NInchiAtom(-1.1292, -0.5292, 0.0, "C"));
            NInchiAtom a2 = input.Add(new NInchiAtom(-1.1333, -1.5917, 0.0, "C"));
            NInchiAtom a3 = input.Add(new NInchiAtom(-1.1333, 0.5333, 0.0, "C"));
            NInchiAtom a4 = input.Add(new NInchiAtom(-1.1375, -2.6542, 0.0, "C"));
            NInchiAtom a5 = input.Add(new NInchiAtom(0.8375, 0.5625, 0.0, "C"));
            NInchiAtom a6 = input.Add(new NInchiAtom(0.9917, -2.4667, 0.0, "C"));
            NInchiAtom a7 = input.Add(new NInchiAtom(2.2417, -0.6542, 0.0, "C"));
            NInchiAtom a8 = input.Add(new NInchiAtom(4.3000, -0.5000, 0.0, "C"));
            NInchiAtom a9 = input.Add(new NInchiAtom(5.8583, 0.9667, 0.0, "C"));
            NInchiAtom a10 = input.Add(new NInchiAtom(6.0167, -1.7500, 0.0, "C"));
            NInchiAtom a11 = input.Add(new NInchiAtom(6.2042, -3.3417, 0.0, "C"));
            a1.ImplicitH = -1;
            a2.ImplicitH = -1;
            a3.ImplicitH = -1;
            a4.ImplicitH = -1;
            a5.ImplicitH = -1;
            a6.ImplicitH = -1;
            a7.ImplicitH = -1;
            a8.ImplicitH = -1;
            a9.ImplicitH = -1;
            a10.ImplicitH = -1;
            a11.ImplicitH = -1;

            input.Bonds.Add(new NInchiBond(a6, a4, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a1, a2, INCHI_BOND_TYPE.Double));
            input.Bonds.Add(new NInchiBond(a7, a5, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a7, a6, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a2, a4, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a8, a7, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a9, a8, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a5, a3, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a8, a10, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a1, a3, INCHI_BOND_TYPE.Single));
            input.Bonds.Add(new NInchiBond(a11, a10, INCHI_BOND_TYPE.Single));

            //        input.Stereos.Add(new JniInchiStereo0D(a7, a7, a5, a6, a8, INCHI_STEREOTYPE.Tetrahedral, INCHI_PARITY.Even));
            //        input.Stereos.Add(new JniInchiStereo0D(a8, a8, a7, a9, a10, INCHI_STEREOTYPE.Tetrahedral, INCHI_PARITY.Odd));

            return input;
        }

        /*

      -ClnMol-06180618052D

    11 11  0  0  0  0  0  0  0  0999 V2000
       -1.1292   -0.5292    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0	1
       -1.1333   -1.5917    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0	2
       -1.1333    0.5333    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0	3
       -1.1375   -2.6542    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0	4
        0.8375    0.5625    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0	5
        0.9917   -2.4667    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0	6
        2.2417   -0.6542    0.0000 C   0  0  2  0  0  0  0  0  0  0  0  0	7
        4.3000   -0.5000    0.0000 C   0  0  1  0  0  0  0  0  0  0  0  0	8
        5.8583    0.9667    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0	9
        6.0167   -1.7500    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0	10
        6.2042   -3.3417    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0	11
      6  4  1  0  0  0  0
      1  2  2  0  0  0  0
      7  5  1  1  0  0  0
      7  6  1  1  0  0  0
      2  4  1  0  0  0  0
      8  7  1  0  0  0  0
      9  8  1  1  0  0  0
      5  3  1  0  0  0  0
      8 10  1  1  0  0  0
      1  3  1  0  0  0  0
    11 10  1  0  0  0  0
    M  END
    >  <ID>
    NSC-7414a

        */

        // Test atom handling

        /// <summary>
        /// Tests element name is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInchiFromChlorineAtom()
        {
            NInchiInput input = getChlorineAtom("");
            NInchiOutput output = NInchiWrapper.GetInchi(input);
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
            Assert.AreEqual("InChI=1S/Cl", output.Inchi);
        }

        /// <summary>
        /// Tests charge is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInchiFromChlorineIon()
        {
            NInchiInput input = getChlorineIon("");
            NInchiOutput output = NInchiWrapper.GetInchi(input);
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
            Assert.AreEqual("InChI=1S/Cl/q-1", output.Inchi);
        }

        /// <summary>
        /// Tests isotopic mass is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInchiFromChlorine37Atom()
        {
            NInchiInput input = getChlorine37Atom("");
            NInchiOutput output = NInchiWrapper.GetInchi(input);
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
            Assert.AreEqual("InChI=1S/Cl/i1+2", output.Inchi);
        }

        /// <summary>
        /// Tests isotopic mass shift is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInchiFromChlorine37ByIstopicMassShiftAtom()
        {
            NInchiInput input = getChlorine37ByIsotopicMassShiftAtom("");
            NInchiOutput output = NInchiWrapper.GetInchi(input);
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
            Assert.AreEqual("InChI=1S/Cl/i1+2", output.Inchi);
        }


        [TestMethod()]
        public void TestGetInchiFromSodiumHydroxide()
        {
            NInchiInput input = getSodiumHydroxide("");
            NInchiOutput output = NInchiWrapper.GetInchi(input);
            Assert.AreEqual("InChI=1S/Na.H2O/h;1H2/q+1;/p-1", output.Inchi);
            Assert.AreEqual(INCHI_RET.WARNING, output.ReturnStatus);
            Assert.AreEqual("Metal was disconnected; Proton(s) added/removed", output.Message);
        }

        /// <summary>
        /// Tests implicit hydrogen count is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInchiFromHydrogenChlorideImplicitH()
        {
            NInchiInput input = getHydrogenChlorideImplicitH("");
            NInchiOutput output = NInchiWrapper.GetInchi(input);
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
            Assert.AreEqual("InChI=1S/ClH/h1H", output.Inchi);
        }

        /// <summary>
        /// Tests implicit protium count is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInchiFromHydrogenChlorideImplicitP()
        {
            NInchiInput input = getHydrogenChlorideImplicitP("");
            NInchiOutput output = NInchiWrapper.GetInchi(input);
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
            Assert.AreEqual("InChI=1S/ClH/h1H/i/hH", output.Inchi);
        }

        /// <summary>
        /// Tests implicit deuterium count is correctly passed to InChi.
        /// </summary>
        [TestMethod()]
        public void TestGetInchiFromHydrogenChlorideImplicitD()
        {
            NInchiInput input = getHydrogenChlorideImplicitD("");
            NInchiOutput output = NInchiWrapper.GetInchi(input);
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
            Assert.AreEqual("InChI=1S/ClH/h1H/i/hD", output.Inchi);
        }

        /// <summary>
        /// Tests implicit tritium count is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInchiFromHydrogenChlorideImplicitT()
        {
            NInchiInput input = getHydrogenChlorideImplicitT("");
            NInchiOutput output = NInchiWrapper.GetInchi(input);
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
            Assert.AreEqual("InChI=1S/ClH/h1H/i/hT", output.Inchi);
        }

        /// <summary>
        /// Tests radical state is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInchiFromMethylRadical()
        {
            NInchiInput input = getMethylRadical("");
            NInchiOutput output = NInchiWrapper.GetInchi(input);
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
            Assert.AreEqual("InChI=1S/CH3/h1H3", output.Inchi);
        }

        // Test bond handling

        /// <summary>
        /// Tests single bond is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInchiFromEthane()
        {
            NInchiInput input = getEthane("");
            NInchiOutput output = NInchiWrapper.GetInchi(input);
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
            Assert.AreEqual("InChI=1S/C2H6/c1-2/h1-2H3", output.Inchi);
        }

        /// <summary>
        /// Tests double bond is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInchiFromEthene()
        {
            NInchiInput input = getEthene("");
            NInchiOutput output = NInchiWrapper.GetInchi(input);
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
            Assert.AreEqual("InChI=1S/C2H4/c1-2/h1-2H2", output.Inchi);
        }

        /// <summary>
        /// Tests triple bond is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInchiFromEthyne()
        {
            NInchiInput input = getEthyne("");
            NInchiOutput output = NInchiWrapper.GetInchi(input);
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
            Assert.AreEqual("InChI=1S/C2H2/c1-2/h1-2H", output.Inchi);
        }

        // Test 2D coordinate handling

        /// <summary>
        /// Tests 2D coordinates are correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInchiEandZ12Dichloroethene2D()
        {
            NInchiInput inputE = getE12dichloroethene2D("");
            NInchiOutput outputE = NInchiWrapper.GetInchi(inputE);
            Assert.AreEqual(INCHI_RET.OKAY, outputE.ReturnStatus);
            Assert.AreEqual("InChI=1S/C2H2Cl2/c3-1-2-4/h1-2H/b2-1+", outputE
                    .Inchi);

            NInchiInput inputZ = getZ12dichloroethene2D("");
            NInchiOutput outputZ = NInchiWrapper.GetInchi(inputZ);
            Assert.AreEqual(INCHI_RET.OKAY, outputZ.ReturnStatus);
            Assert.AreEqual("InChI=1S/C2H2Cl2/c3-1-2-4/h1-2H/b2-1-", outputZ
                    .Inchi);
        }

        // Test 3D coordinate handling

        /// <summary>
        /// Tests InChI generation from L and D-Alanine molecules, with 3D
        /// coordinates.
        /// </summary>
        [TestMethod()]
        public void TestGetInchiFromLandDAlanine3D()
        {
            NInchiInput inputL = getLAlanine3D("");
            NInchiOutput outputL = NInchiWrapper.GetInchi(inputL);
            Assert.AreEqual(INCHI_RET.OKAY, outputL.ReturnStatus);
            Assert.AreEqual(
                    "InChI=1S/C3H7NO2/c1-2(4)3(5)6/h2H,4H2,1H3,(H,5,6)/t2-/m0/s1",
                    outputL.Inchi);

            NInchiInput inputD = getDAlanine3D("");
            NInchiOutput outputD = NInchiWrapper.GetInchi(inputD);
            Assert.AreEqual(INCHI_RET.OKAY, outputD.ReturnStatus);
            Assert.AreEqual(
                    "InChI=1S/C3H7NO2/c1-2(4)3(5)6/h2H,4H2,1H3,(H,5,6)/t2-/m1/s1",
                    outputD.Inchi);
        }

        // Test handling of 2D coordinates with bond stereo types

        /// <summary>
        /// Tests InChI generation from L and D-Alanine molecules, with 3D
        /// coordinates, using FixSp3Bug option from InChI software v1.01
        /// </summary>
        [TestMethod()]
        public void TestGetInchiFromAlanine2D()
        {
            NInchiInput input = getAlanine2D("");
            NInchiOutput output = NInchiWrapper.GetInchi(input);
            Assert.AreEqual(INCHI_RET.WARNING, output.ReturnStatus);
            Assert.AreEqual("Omitted undefined stereo", output.Message);
            Assert.AreEqual("InChI=1S/C3H7NO2/c1-2(4)3(5)6/h2H,4H2,1H3,(H,5,6)",
                    output.Inchi);
        }

        [TestMethod()]
        public void TestGetInchiFromLAlanine2D()
        {
            NInchiInput inputL = getLAlanine2Da("");
            NInchiOutput outputL = NInchiWrapper.GetInchi(inputL);
            Assert.AreEqual(INCHI_RET.OKAY, outputL.ReturnStatus);
            Assert.AreEqual(
                    "InChI=1S/C3H7NO2/c1-2(4)3(5)6/h2H,4H2,1H3,(H,5,6)/t2-/m0/s1",
                    outputL.Inchi);
        }

        [TestMethod()]
        public void TestGetInchiFromDAlanine2D()
        {
            NInchiInput inputD = getDAlanine2D("");
            NInchiOutput outputD = NInchiWrapper.GetInchi(inputD);
            Assert.AreEqual(INCHI_RET.OKAY, outputD.ReturnStatus);
            Assert.AreEqual(
                    "InChI=1S/C3H7NO2/c1-2(4)3(5)6/h2H,4H2,1H3,(H,5,6)/t2-/m1/s1",
                    outputD.Inchi);
        }


        /// <summary>
        /// Tests InChI generation from L-Alanine molecules, with 2D coordinates
        /// and wedge/hatch bonds, with bond drawn in opposite directions.
        /// </summary>
        [TestMethod()]
        public void TestGetInchiStereoBondDirection1()
        {
            NInchiInput input = getLAlanine2Da("");
            NInchiOutput output = NInchiWrapper.GetInchi(input);
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
            Assert.AreEqual(
                    "InChI=1S/C3H7NO2/c1-2(4)3(5)6/h2H,4H2,1H3,(H,5,6)/t2-/m0/s1",
                    output.Inchi);
        }

        [TestMethod()]
        public void TestGetInchiStereoBondDirection2()
        {
            NInchiInput inputL = getLAlanine2Db("");
            NInchiOutput outputL = NInchiWrapper.GetInchi(inputL);
            Assert.AreEqual(INCHI_RET.OKAY, outputL.ReturnStatus);
            Assert.AreEqual(
                    "InChI=1S/C3H7NO2/c1-2(4)3(5)6/h2H,4H2,1H3,(H,5,6)/t2-/m0/s1",
                    outputL.Inchi);
        }

        // Test handling of no coordinates, with stereo parities

        /// <summary>
        /// Tests InChI generation from L and D-Alanine molecules, with no
        /// coordinates but tetrahedral stereo parities.
        /// </summary>
        [TestMethod()]
        public void TestGetInchiFromLandDAlanine0D()
        {
            NInchiInput input = getAlanine0D("");
            NInchiOutput output = NInchiWrapper.GetInchi(input);
            Assert.AreEqual(INCHI_RET.WARNING, output.ReturnStatus);
            Assert.AreEqual("Omitted undefined stereo", output.Message);
            Assert.AreEqual("InChI=1S/C3H7NO2/c1-2(4)3(5)6/h2H,4H2,1H3,(H,5,6)",
                    output.Inchi);

            NInchiInput inputL = getLAlanine0D("");
            NInchiOutput outputL = NInchiWrapper.GetInchi(inputL);
            Assert.AreEqual(INCHI_RET.OKAY, outputL.ReturnStatus);
            Assert.AreEqual(
                    "InChI=1S/C3H7NO2/c1-2(4)3(5)6/h2H,4H2,1H3,(H,5,6)/t2-/m0/s1",
                    outputL.Inchi);

            NInchiInput inputD = getDAlanine0D("");
            NInchiOutput outputD = NInchiWrapper.GetInchi(inputD);
            Assert.AreEqual(INCHI_RET.OKAY, outputD.ReturnStatus);
            Assert.AreEqual(
                    "InChI=1S/C3H7NO2/c1-2(4)3(5)6/h2H,4H2,1H3,(H,5,6)/t2-/m1/s1",
                    outputD.Inchi);
        }

        /// <summary>
        /// Tests InChI generation from E and Z 1,2-dichloroethene molecules, with no
        /// coordinates but doublebond stereo parities.
        /// </summary>
        [TestMethod()]
        public void TestGetInchiEandZ12Dichloroethene0D()
        {
            NInchiInput input = get12dichloroethene0D("");
            NInchiOutput output = NInchiWrapper.GetInchi(input);
            Assert.AreEqual(INCHI_RET.WARNING, output.ReturnStatus);
            Assert.AreEqual("Omitted undefined stereo", output.Message);
            Assert.AreEqual("InChI=1S/C2H2Cl2/c3-1-2-4/h1-2H", output
                            .Inchi);

            NInchiInput inputE = getE12dichloroethene0D("");
            NInchiOutput outputE = NInchiWrapper.GetInchi(inputE);
            Assert.AreEqual(INCHI_RET.OKAY, outputE.ReturnStatus);
            Assert.AreEqual("InChI=1S/C2H2Cl2/c3-1-2-4/h1-2H/b2-1+", outputE
                    .Inchi);

            NInchiInput inputZ = getZ12dichloroethene0D("");
            NInchiOutput outputZ = NInchiWrapper.GetInchi(inputZ);
            Assert.AreEqual(INCHI_RET.OKAY, outputZ.ReturnStatus);
            Assert.AreEqual("InChI=1S/C2H2Cl2/c3-1-2-4/h1-2H/b2-1-", outputZ
                    .Inchi);
        }



        [TestMethod()]
        public void TestGetInchiFromNSC7414a()
        {
            NInchiInput input = getNSC7414a("");
            NInchiOutput output = NInchiWrapper.GetInchi(input);
            Assert.AreEqual(INCHI_RET.WARNING, output.ReturnStatus);
            Assert.AreEqual("Omitted undefined stereo", output.Message);
            Assert.AreEqual(
                    "InChI=1S/C11H20/c1-3-10(2)11-8-6-4-5-7-9-11/h4-5,10-11H,3,6-9H2,1-2H3",
                    output.Inchi);
        }



        // Test option checking

        /// <summary>
        /// Tests option lists are canonicalised correctly.
        /// </summary>
        [TestMethod()]
        public void TestCheckOptionsList()
        {
            var opList = new List<INCHI_OPTION>();
            opList.Add(INCHI_OPTION.Compress);
            opList.Add(INCHI_OPTION.SNon);
            string options = NInchiWrapper.CheckOptions(opList);
            string flag = NInchiWrapper.FlagChar;
            Assert.AreEqual(flag + "Compress " + flag + "SNon ", options);
        }

        /// <summary>
        /// </summary>
        [TestMethod()]
        public void TestCheckOptionsString()
        {
            string options = NInchiWrapper.CheckOptions("  -ComPreSS  /SNon");
            string flag = NInchiWrapper.FlagChar;
            Assert.AreEqual(flag + "Compress " + flag + "SNon", options);
        }

        // Test option handling

        /// <summary>
        /// Tests passing options to inchi.
        /// </summary>
        // [TestMethod()]
        public void TestGetInchiWithOptions()
        {
            NInchiInput input = getLAlanine3D("");
            NInchiOutput output = NInchiWrapper.GetInchi(input);
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
            Assert.AreEqual(
                    "InChI=1/C3H7NO2/c1-2(4)3(5)6/h2H,4H2,1H3,(H,5,6)/t2-/m0/s1",
                    output.Inchi);

            input = getLAlanine3D("-compress");
            output = NInchiWrapper.GetInchi(input);
            // debug(output);
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
            Assert.AreEqual("InChI=1/C3H7NO2/cABBCC/hB1D2A3,1EF/tB1/m0/s1",
                    output.Inchi);

            input = getLAlanine3D("/compress");
            output = NInchiWrapper.GetInchi(input);
            // debug(output);
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
            Assert.AreEqual("InChI=1/C3H7NO2/cABBCC/hB1D2A3,1EF/tB1/m0/s1",
                    output.Inchi);

            input = getLAlanine3D("-cOMprEsS");
            output = NInchiWrapper.GetInchi(input);
            // debug(output);
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
            Assert.AreEqual("InChI=1/C3H7NO2/cABBCC/hB1D2A3,1EF/tB1/m0/s1",
                    output.Inchi);
        }

        // Test structure generation from InChI strings

        /// <summary>
        /// Tests element name is correctly read from InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetChlorineAtomFromInchi()
        {
            NInchiInputInchi input = new NInchiInputInchi("InChI=1/Cl");
            NInchiOutputStructure output = NInchiWrapper
                    .GetStructureFromInchi(input);
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
            Assert.AreEqual(1, output.Atoms.Count);
            Assert.AreEqual(0, output.Bonds.Count);
            Assert.AreEqual(0, output.Stereos.Count);
            Assert.AreEqual(
                            "InChI Atom: Cl [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:0 P:0 D:0 T:0 // Radical: None",
                            output.Atoms[0].ToDebugString());
        }

        /// <summary>
        /// Tests charge is correctly read from InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetChargeFromSodiumHydroxide()
        {

            NInchiInputInchi input = new NInchiInputInchi("InChI=1S/Na.H2O/h;1H2/q+1;/p-1");
            NInchiOutputStructure output = NInchiWrapper.GetStructureFromInchi(input);
            Assert.AreEqual(2, output.Atoms.Count);
            Assert.AreEqual(0, output.Bonds.Count);
            Assert.AreEqual(0, output.Stereos.Count);
            Assert.AreEqual("InChI Atom: Na [0.0,0.0,0.0] Charge:1 // Iso Mass:0 // Implicit H:0 P:0 D:0 T:0 // Radical: None", output.Atoms[0].ToDebugString());
            Assert.AreEqual("InChI Atom: O [0.0,0.0,0.0] Charge:-1 // Iso Mass:0 // Implicit H:1 P:0 D:0 T:0 // Radical: None", output.Atoms[1].ToDebugString());
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
        }

        /// <summary>
        /// Tests isotopic mass is correctly read from InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetChlorine37AtomFromInchi()
        {
            NInchiInputInchi input = new NInchiInputInchi("InChI=1/Cl/i1+2");
            NInchiOutputStructure output = NInchiWrapper
                    .GetStructureFromInchi(input);
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
            Assert.AreEqual(1, output.Atoms.Count);
            Assert.AreEqual(0, output.Bonds.Count);
            Assert.AreEqual(0, output.Stereos.Count);
            Assert.AreEqual(
                            "InChI Atom: Cl [0.0,0.0,0.0] Charge:0 // Iso Mass:10002 // Implicit H:0 P:0 D:0 T:0 // Radical: None",
                            output.Atoms[0].ToDebugString());
        }

        /// <summary>
        /// Tests implicit hydrogen count is correctly read from InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetHydrogenChlorideImplicitHFromInchi()
        {
            NInchiInputInchi input = new NInchiInputInchi("InChI=1/ClH/h1H");
            NInchiOutputStructure output = NInchiWrapper
                    .GetStructureFromInchi(input);
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
            Assert.AreEqual(1, output.Atoms.Count);
            Assert.AreEqual(0, output.Bonds.Count);
            Assert.AreEqual(0, output.Stereos.Count);
            Assert.AreEqual(
                            "InChI Atom: Cl [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:1 P:0 D:0 T:0 // Radical: None",
                            output.Atoms[0].ToDebugString());
        }

        /// <summary>
        /// Tests implicit protium count is correctly read from InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetHydrogenChlorideImplicitPFromInchi()
        {
            NInchiInputInchi input = new NInchiInputInchi(
                    "InChI=1/ClH/h1H/i/hH");
            NInchiOutputStructure output = NInchiWrapper
                    .GetStructureFromInchi(input);
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
            Assert.AreEqual(1, output.Atoms.Count);
            Assert.AreEqual(0, output.Bonds.Count);
            Assert.AreEqual(0, output.Stereos.Count);
            Assert.AreEqual(
                            "InChI Atom: Cl [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:0 P:1 D:0 T:0 // Radical: None",
                            output.Atoms[0].ToDebugString());
        }

        /// <summary>
        /// Tests implicit deuterium count is correctly read from InChi.
        /// </summary>
        [TestMethod()]
        public void TestGetHydrogenChlorideImplicitDFromInchi()
        {
            NInchiInputInchi input = new NInchiInputInchi(
                    "InChI=1/ClH/h1H/i/hD");
            NInchiOutputStructure output = NInchiWrapper
                    .GetStructureFromInchi(input);
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
            Assert.AreEqual(1, output.Atoms.Count);
            Assert.AreEqual(0, output.Bonds.Count);
            Assert.AreEqual(0, output.Stereos.Count);
            Assert.AreEqual(
                            "InChI Atom: Cl [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:0 P:0 D:1 T:0 // Radical: None",
                            output.Atoms[0].ToDebugString());
        }

        /// <summary>
        /// Tests implicit tritium count is correctly read from InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetHydrogenChlorideImplicitTFromInchi()
        {
            NInchiInputInchi input = new NInchiInputInchi(
                    "InChI=1/ClH/h1H/i/hT");
            NInchiOutputStructure output = NInchiWrapper
                    .GetStructureFromInchi(input);
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
            Assert.AreEqual(1, output.Atoms.Count);
            Assert.AreEqual(0, output.Bonds.Count);
            Assert.AreEqual(0, output.Stereos.Count);
            Assert.AreEqual(
                            "InChI Atom: Cl [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:0 P:0 D:0 T:1 // Radical: None",
                            output.Atoms[0].ToDebugString());
        }

        /// <summary>
        /// Tests radical state is correctly read from InChI.
        /// </summary>
        //@Ignore
        // Test fails due to problem with InChI library
        public void TestGetMethylRadicalFromInchi()
        {

            NInchiInputInchi input = new NInchiInputInchi("InChI=1/CH3/h1H3");
            NInchiOutputStructure output = NInchiWrapper.GetStructureFromInchi(input);
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
            Assert.AreEqual(1, output.Atoms.Count);
            Assert.AreEqual(0, output.Bonds.Count);
            Assert.AreEqual(0, output.Stereos.Count);
            Assert.AreEqual("InChI Atom: C [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:3 P:0 D:0 T:0 // Radical: Doublet", output.Atoms[0].ToDebugString());
        }



        // Test bond handling

        /// <summary>
        /// Tests single bond is correctly read from InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetEthaneFromInchi()
        {
            NInchiInputInchi input = new NInchiInputInchi(
                    "InChI=1/C2H6/c1-2/h1-2H3");
            NInchiOutputStructure output = NInchiWrapper
                    .GetStructureFromInchi(input);
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
            Assert.AreEqual(2, output.Atoms.Count);
            Assert.AreEqual(1, output.Bonds.Count);
            Assert.AreEqual(0, output.Stereos.Count);
            Assert.AreEqual(
                            "InChI Atom: C [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:3 P:0 D:0 T:0 // Radical: None",
                            output.Atoms[0].ToDebugString());
            Assert.AreEqual(
                            "InChI Atom: C [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:3 P:0 D:0 T:0 // Radical: None",
                            output.Atoms[1].ToDebugString());
            Assert.AreEqual("InChI Bond: C-C // Type: Single // Stereo: None",
                    output.Bonds[0].ToDebugString());
        }

        /// <summary>
        /// Tests double bond is correctly read from InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetEtheneFromInchi()
        {
            NInchiInputInchi input = new NInchiInputInchi(
                    "InChI=1/C2H4/c1-2/h1-2H2");
            NInchiOutputStructure output = NInchiWrapper
                    .GetStructureFromInchi(input);
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
            Assert.AreEqual(2, output.Atoms.Count);
            Assert.AreEqual(1, output.Bonds.Count);
            Assert.AreEqual(0, output.Stereos.Count);
            Assert.AreEqual(
                            "InChI Atom: C [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:2 P:0 D:0 T:0 // Radical: None",
                            output.Atoms[0].ToDebugString());
            Assert.AreEqual(
                            "InChI Atom: C [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:2 P:0 D:0 T:0 // Radical: None",
                            output.Atoms[1].ToDebugString());
            Assert.AreEqual("InChI Bond: C-C // Type: Double // Stereo: None",
                    output.Bonds[0].ToDebugString());
        }

        /// <summary>
        /// Tests triple bond is correctly read from InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetEthyneFromInchi()
        {
            NInchiInputInchi input = new NInchiInputInchi(
                    "InChI=1/C2H2/c1-2/h1-2H");
            NInchiOutputStructure output = NInchiWrapper
                    .GetStructureFromInchi(input);
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
            Assert.AreEqual(2, output.Atoms.Count);
            Assert.AreEqual(1, output.Bonds.Count);
            Assert.AreEqual(0, output.Stereos.Count);
            Assert.AreEqual(
                            "InChI Atom: C [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:1 P:0 D:0 T:0 // Radical: None",
                            output.Atoms[0].ToDebugString());
            Assert.AreEqual(
                            "InChI Atom: C [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:1 P:0 D:0 T:0 // Radical: None",
                            output.Atoms[1].ToDebugString());
            Assert.AreEqual("InChI Bond: C-C // Type: Triple // Stereo: None",
                    output.Bonds[0].ToDebugString());
        }

        // Test handling of no coordinates, with stereo parities

        /// <summary>
        /// Tests generation of L and D-Alanine molecules, from InChIs with
        /// tetrahedral stereo parities.
        /// </summary>
        [TestMethod()]
        public void TestGetLandDAlanine0DFromInchi()
        {
            NInchiInputInchi input = new NInchiInputInchi(
                    "InChI=1/C3H7NO2/c1-2(4)3(5)6/h2H,4H2,1H3,(H,5,6)");
            NInchiOutputStructure output = NInchiWrapper
                    .GetStructureFromInchi(input);
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
            Assert.AreEqual(6, output.Atoms.Count);
            Assert.AreEqual(5, output.Bonds.Count);
            Assert.AreEqual(0, output.Stereos.Count);
            Assert.AreEqual(
                            "InChI Atom: C [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:3 P:0 D:0 T:0 // Radical: None",
                            output.Atoms[0].ToDebugString());
            Assert.AreEqual(
                            "InChI Atom: C [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:1 P:0 D:0 T:0 // Radical: None",
                            output.Atoms[1].ToDebugString());
            Assert.AreEqual(
                            "InChI Atom: C [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:0 P:0 D:0 T:0 // Radical: None",
                            output.Atoms[2].ToDebugString());
            Assert.AreEqual(
                            "InChI Atom: N [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:2 P:0 D:0 T:0 // Radical: None",
                            output.Atoms[3].ToDebugString());
            Assert.AreEqual(
                            "InChI Atom: O [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:0 P:0 D:0 T:0 // Radical: None",
                            output.Atoms[4].ToDebugString());
            Assert.AreEqual(
                            "InChI Atom: O [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:1 P:0 D:0 T:0 // Radical: None",
                            output.Atoms[5].ToDebugString());
            Assert.AreEqual("InChI Bond: C-C // Type: Single // Stereo: None",
                    output.Bonds[0].ToDebugString());
            Assert.AreEqual("InChI Bond: C-C // Type: Single // Stereo: None",
                    output.Bonds[1].ToDebugString());
            Assert.AreEqual("InChI Bond: N-C // Type: Single // Stereo: None",
                    output.Bonds[2].ToDebugString());
            Assert.AreEqual("InChI Bond: O-C // Type: Double // Stereo: None",
                    output.Bonds[3].ToDebugString());
            Assert.AreEqual("InChI Bond: O-C // Type: Single // Stereo: None",
                    output.Bonds[4].ToDebugString());

            NInchiInputInchi inputL = new NInchiInputInchi(
                    "InChI=1/C3H7NO2/c1-2(4)3(5)6/h2H,4H2,1H3,(H,5,6)/t2-/m0/s1");
            NInchiOutputStructure outputL = NInchiWrapper
                    .GetStructureFromInchi(inputL);
            Assert.AreEqual(INCHI_RET.OKAY, outputL.ReturnStatus);
            Assert.AreEqual(7, outputL.Atoms.Count);
            Assert.AreEqual(6, outputL.Bonds.Count);
            Assert.AreEqual(1, outputL.Stereos.Count);
            Assert.AreEqual(
                            "InChI Atom: C [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:3 P:0 D:0 T:0 // Radical: None",
                            outputL.Atoms[0].ToDebugString());
            Assert.AreEqual(
                            "InChI Atom: C [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:0 P:0 D:0 T:0 // Radical: None",
                            outputL.Atoms[1].ToDebugString());
            Assert.AreEqual(
                            "InChI Atom: C [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:0 P:0 D:0 T:0 // Radical: None",
                            outputL.Atoms[2].ToDebugString());
            Assert.AreEqual(
                            "InChI Atom: N [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:2 P:0 D:0 T:0 // Radical: None",
                            outputL.Atoms[3].ToDebugString());
            Assert.AreEqual(
                            "InChI Atom: O [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:0 P:0 D:0 T:0 // Radical: None",
                            outputL.Atoms[4].ToDebugString());
            Assert.AreEqual(
                            "InChI Atom: O [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:1 P:0 D:0 T:0 // Radical: None",
                            outputL.Atoms[5].ToDebugString());
            Assert.AreEqual("InChI Bond: C-C // Type: Single // Stereo: None",
                    outputL.Bonds[0].ToDebugString());
            Assert.AreEqual("InChI Bond: C-C // Type: Single // Stereo: None",
                    outputL.Bonds[1].ToDebugString());
            Assert.AreEqual("InChI Bond: N-C // Type: Single // Stereo: None",
                    outputL.Bonds[2].ToDebugString());
            Assert.AreEqual("InChI Bond: O-C // Type: Double // Stereo: None",
                    outputL.Bonds[3].ToDebugString());
            Assert.AreEqual("InChI Bond: O-C // Type: Single // Stereo: None",
                    outputL.Bonds[4].ToDebugString());
            Assert.AreEqual(
                    "InChI Stereo0D: C [H,C,C,N] Type::Tetrahedral // Parity:Odd",
                    outputL.Stereos[0].ToDebugString());

            NInchiInputInchi inputD = new NInchiInputInchi(
                    "InChI=1/C3H7NO2/c1-2(4)3(5)6/h2H,4H2,1H3,(H,5,6)/t2-/m1/s1");
            NInchiOutputStructure outputD = NInchiWrapper
                    .GetStructureFromInchi(inputD);
            Assert.AreEqual(INCHI_RET.OKAY, outputD.ReturnStatus);
            Assert.AreEqual(7, outputD.Atoms.Count);
            Assert.AreEqual(6, outputD.Bonds.Count);
            Assert.AreEqual(1, outputD.Stereos.Count);
            Assert.AreEqual(
                            "InChI Atom: C [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:3 P:0 D:0 T:0 // Radical: None",
                            outputD.Atoms[0].ToDebugString());
            Assert.AreEqual(
                            "InChI Atom: C [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:0 P:0 D:0 T:0 // Radical: None",
                            outputD.Atoms[1].ToDebugString());
            Assert.AreEqual(
                            "InChI Atom: C [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:0 P:0 D:0 T:0 // Radical: None",
                            outputD.Atoms[2].ToDebugString());
            Assert.AreEqual(
                            "InChI Atom: N [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:2 P:0 D:0 T:0 // Radical: None",
                            outputD.Atoms[3].ToDebugString());
            Assert.AreEqual(
                            "InChI Atom: O [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:0 P:0 D:0 T:0 // Radical: None",
                            outputD.Atoms[4].ToDebugString());
            Assert.AreEqual(
                            "InChI Atom: O [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:1 P:0 D:0 T:0 // Radical: None",
                            outputD.Atoms[5].ToDebugString());
            Assert.AreEqual("InChI Bond: C-C // Type: Single // Stereo: None",
                    outputD.Bonds[0].ToDebugString());
            Assert.AreEqual("InChI Bond: C-C // Type: Single // Stereo: None",
                    outputD.Bonds[1].ToDebugString());
            Assert.AreEqual("InChI Bond: N-C // Type: Single // Stereo: None",
                    outputD.Bonds[2].ToDebugString());
            Assert.AreEqual("InChI Bond: O-C // Type: Double // Stereo: None",
                    outputD.Bonds[3].ToDebugString());
            Assert.AreEqual("InChI Bond: O-C // Type: Single // Stereo: None",
                    outputD.Bonds[4].ToDebugString());
            Assert.AreEqual(
                    "InChI Stereo0D: C [H,C,C,N] Type::Tetrahedral // Parity:Even",
                    outputD.Stereos[0].ToDebugString());
        }

        /// <summary>
        /// Tests generation of E and Z 1,2-dichloroethene molecules, from InChIs
        /// with doublebond stereo parities.
        /// </summary>
        [TestMethod()]
        public void TestGetEandZ12Dichloroethene0DFromInchi()
        {
            NInchiInputInchi input = new NInchiInputInchi(
                    "InChI=1/C2H2Cl2/c3-1-2-4/h1-2H");
            NInchiOutputStructure output = NInchiWrapper
                    .GetStructureFromInchi(input);
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
            Assert.AreEqual(4, output.Atoms.Count);
            Assert.AreEqual(3, output.Bonds.Count);
            Assert.AreEqual(0, output.Stereos.Count);
            Assert.AreEqual(
                            "InChI Atom: C [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:1 P:0 D:0 T:0 // Radical: None",
                            output.Atoms[0].ToDebugString());
            Assert.AreEqual(
                            "InChI Atom: C [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:1 P:0 D:0 T:0 // Radical: None",
                            output.Atoms[1].ToDebugString());
            Assert.AreEqual(
                            "InChI Atom: Cl [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:0 P:0 D:0 T:0 // Radical: None",
                            output.Atoms[2].ToDebugString());
            Assert.AreEqual(
                            "InChI Atom: Cl [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:0 P:0 D:0 T:0 // Radical: None",
                            output.Atoms[3].ToDebugString());
            Assert.AreEqual("InChI Bond: C-C // Type: Double // Stereo: None",
                    output.Bonds[0].ToDebugString());
            Assert.AreEqual("InChI Bond: Cl-C // Type: Single // Stereo: None",
                    output.Bonds[1].ToDebugString());
            Assert.AreEqual("InChI Bond: Cl-C // Type: Single // Stereo: None",
                    output.Bonds[2].ToDebugString());

            NInchiInputInchi inputE = new NInchiInputInchi(
                    "InChI=1/C2H2Cl2/c3-1-2-4/h1-2H/b2-1+");
            NInchiOutputStructure outputE = NInchiWrapper
                    .GetStructureFromInchi(inputE);
            Assert.AreEqual(INCHI_RET.OKAY, outputE.ReturnStatus);
            Assert.AreEqual(6, outputE.Atoms.Count);
            Assert.AreEqual(5, outputE.Bonds.Count);
            Assert.AreEqual(1, outputE.Stereos.Count);
            Assert.AreEqual(
                            "InChI Atom: C [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:0 P:0 D:0 T:0 // Radical: None",
                            outputE.Atoms[0].ToDebugString());
            Assert.AreEqual(
                            "InChI Atom: C [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:0 P:0 D:0 T:0 // Radical: None",
                            outputE.Atoms[1].ToDebugString());
            Assert.AreEqual(
                            "InChI Atom: Cl [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:0 P:0 D:0 T:0 // Radical: None",
                            outputE.Atoms[2].ToDebugString());
            Assert.AreEqual(
                            "InChI Atom: Cl [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:0 P:0 D:0 T:0 // Radical: None",
                            outputE.Atoms[3].ToDebugString());
            Assert.AreEqual(
                            "InChI Atom: H [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:0 P:0 D:0 T:0 // Radical: None",
                            outputE.Atoms[4].ToDebugString());
            Assert.AreEqual(
                            "InChI Atom: H [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:0 P:0 D:0 T:0 // Radical: None",
                            outputE.Atoms[5].ToDebugString());
            Assert.AreEqual("InChI Bond: C-C // Type: Double // Stereo: None",
                    outputE.Bonds[0].ToDebugString());
            Assert.AreEqual("InChI Bond: Cl-C // Type: Single // Stereo: None",
                    outputE.Bonds[1].ToDebugString());
            Assert.AreEqual("InChI Bond: Cl-C // Type: Single // Stereo: None",
                    outputE.Bonds[2].ToDebugString());
            Assert.AreEqual(
                    "InChI Stereo0D: - [H,C,C,H] Type::DoubleBond // Parity:Even",
                    outputE.Stereos[0].ToDebugString());

            NInchiInputInchi inputZ = new NInchiInputInchi(
                    "InChI=1/C2H2Cl2/c3-1-2-4/h1-2H/b2-1-");
            NInchiOutputStructure outputZ = NInchiWrapper
                    .GetStructureFromInchi(inputZ);
            Assert.AreEqual(INCHI_RET.OKAY, outputZ.ReturnStatus);
            Assert.AreEqual(6, outputZ.Atoms.Count);
            Assert.AreEqual(5, outputZ.Bonds.Count);
            Assert.AreEqual(1, outputZ.Stereos.Count);
            Assert.AreEqual(
                            "InChI Atom: C [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:0 P:0 D:0 T:0 // Radical: None",
                            outputZ.Atoms[0].ToDebugString());
            Assert.AreEqual(
                            "InChI Atom: C [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:0 P:0 D:0 T:0 // Radical: None",
                            outputZ.Atoms[1].ToDebugString());
            Assert.AreEqual(
                            "InChI Atom: Cl [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:0 P:0 D:0 T:0 // Radical: None",
                            outputZ.Atoms[2].ToDebugString());
            Assert.AreEqual(
                            "InChI Atom: Cl [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:0 P:0 D:0 T:0 // Radical: None",
                            outputZ.Atoms[3].ToDebugString());
            Assert.AreEqual(
                            "InChI Atom: H [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:0 P:0 D:0 T:0 // Radical: None",
                            outputZ.Atoms[4].ToDebugString());
            Assert.AreEqual(
                            "InChI Atom: H [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:0 P:0 D:0 T:0 // Radical: None",
                            outputZ.Atoms[5].ToDebugString());
            Assert.AreEqual("InChI Bond: C-C // Type: Double // Stereo: None",
                    outputZ.Bonds[0].ToDebugString());
            Assert.AreEqual("InChI Bond: Cl-C // Type: Single // Stereo: None",
                    outputZ.Bonds[1].ToDebugString());
            Assert.AreEqual("InChI Bond: Cl-C // Type: Single // Stereo: None",
                    outputZ.Bonds[2].ToDebugString());
            Assert.AreEqual(
                    "InChI Stereo0D: - [H,C,C,H] Type::DoubleBond // Parity:Odd",
                    outputZ.Stereos[0].ToDebugString());
        }


        /// <summary>
        /// Tests InchiKey generation.
        /// </summary>
        [TestMethod()]
        public void TestGetInchiKeyForCafeine()
        {
            string inchi = "InChI=1/C8H10N4O2/c1-10-4-9-6-5(10)7(13)12(3)8(14)11(6)2/h4H,1-3H3";
            NInchiOutputKey output = NInchiWrapper.GetInchiKey(inchi);
            Assert.AreEqual(INCHI_KEY.OK, output.ReturnStatus);
            Assert.AreEqual("RYYVLZVUVIJVGH-UHFFFAOYNA-N", output.Key);
        }

        /// <summary>
        /// Tests InchiKey generation.
        /// </summary>
        [TestMethod()]
        public void TestGetStdInchiKeyForCafeine()
        {
            string inchi = "InChI=1S/C8H10N4O2/c1-10-4-9-6-5(10)7(13)12(3)8(14)11(6)2/h4H,1-3H3";
            NInchiOutputKey output = NInchiWrapper.GetInchiKey(inchi);
            Assert.AreEqual(INCHI_KEY.OK, output.ReturnStatus);
            Assert.AreEqual("RYYVLZVUVIJVGH-UHFFFAOYSA-N", output.Key);
        }

        [TestMethod()]
        public void TestGetInchiKeyEmptyInput()
        {
            string inchi = "";
            NInchiOutputKey output = NInchiWrapper.GetInchiKey(inchi);
            Assert.AreEqual(INCHI_KEY.INVALID_INCHI_PREFIX, output.ReturnStatus);
        }

        [TestMethod()]
        public void TestGetInchiKeyInputInvalidPrefix()
        {
            string inchi = "foo=1/C8H10N4O2/c1-10-4-9-6-5(10)7(13)12(3)8(14)11(6)2/h4H,1-3H3";
            NInchiOutputKey output = NInchiWrapper.GetInchiKey(inchi);
            Assert.AreEqual(INCHI_KEY.INVALID_INCHI_PREFIX, output.ReturnStatus);
        }

        [TestMethod()]
        public void TestGetInchiKeyInputInvalidInchi()
        {
            string inchi = "InChI=1/-";
            NInchiOutputKey output = NInchiWrapper.GetInchiKey(inchi);
            Assert.AreEqual(INCHI_KEY.INVALID_INCHI, output.ReturnStatus);
        }

        //@Ignore
        // InChI library does very little checking of input,
        // many invalid InChIs will produce InChI keys...
        public void TestGetInchiKeyInputInvalidInchi1()
        {
            string inchi = "InChI=1/C8H10N4O2/x1-9-4-9-6-5(10)7(13)12(3)8(14)11(6)2/h4H,1-3H3";
            NInchiOutputKey output = NInchiWrapper.GetInchiKey(inchi);
            Assert.AreEqual(INCHI_KEY.INVALID_INCHI, output.ReturnStatus);
        }

        [TestMethod()]
        public void TestCheckInchiKeyValid()
        {
            string key = "RYYVLZVUVIJVGH-UHFFFAOYNA-N";
            INCHI_KEY_STATUS status = NInchiWrapper.CheckInchiKey(key);
            Assert.AreEqual(INCHI_KEY_STATUS.VALID_NON_STANDARD, status);
        }

        [TestMethod()]
        public void TestCheckInchiKeyInvalidLengthLong()
        {
            string key = "RYYVLZVUVIJVGH-UHFFFAOYNA-NX";
            INCHI_KEY_STATUS status1 = NInchiWrapper.CheckInchiKey(key);
            Assert.AreEqual(INCHI_KEY_STATUS.INVALID_LENGTH, status1);
        }

        [TestMethod()]
        public void TestCheckInchiKeyInvalidLengthShort()
        {
            string key = "RYYVLZVUVIJVGH-UHFFFAOYNA-";
            INCHI_KEY_STATUS status2 = NInchiWrapper.CheckInchiKey(key);
            Assert.AreEqual(INCHI_KEY_STATUS.INVALID_LENGTH, status2);
        }

        [TestMethod()]
        public void TestCheckInchiKeyInvalidLayout()
        {
            string key = "RYYVLZVUVIJVGHXUHFFFAOYNAXN";
            INCHI_KEY_STATUS status = NInchiWrapper.CheckInchiKey(key);
            Assert.AreEqual(INCHI_KEY_STATUS.INVALID_LAYOUT, status);
        }

        [TestMethod()]
        public void TestCheckInchiKeyInvalidVersion()
        {
            string key = "RYYVLZVUVIJVGH-UHFFFAOYNX-N";
            INCHI_KEY_STATUS status = NInchiWrapper.CheckInchiKey(key);
            Assert.AreEqual(INCHI_KEY_STATUS.INVALID_VERSION, status);
        }

        /* Option doesn't work yet
        [TestMethod()]
        @Ignore
        public void TestGenerateInchiKeyViaOptions()  {
            JniInchiInput input = getLAlanine3D("-key");
            JniInchiOutput output = JniInchiWrapper.GetInchi(input);
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
    //        Assert.AreEqual(null, output.Inchi);
    //        Assert.AreEqual(null, output.AuxInfo);
    //        Assert.AreEqual(null, output.Message);
    //        Assert.AreEqual(null, output.Log);
        }
       */

        /// <summary>
        /// Tests thread safety - starts ten threads, and sets them generating InChIs
        /// for randomly picked elements. Checks generated InChIs are as expected.
        /// </summary>
        [TestMethod()]
        public void Multithreading()
        {
            string[] ELS = { "H", "He", "Li", "Be", "B", "C", "N", "O",
                "F", "Ne", "Na", "Mg", "Al", "Si", "P", "S", "Cl", "Ar" };
            
            bool stop = false;
            bool done = false;

            int failCount = 0;
            int runCount = 0;

            var tts = new Thread[5];
            {
                for (int i = 0; i < tts.Length; i++)
                {
                    var ts = new ThreadStart(() =>
                    {
                        //            Console.Error.WriteLine("Thread "+threadIndex+" starting");
                        Random rand = new Random(0);
                        while (!stop)
                        {
                            Thread.Yield();
                            runCount++;
                            NInchiInput input = new NInchiInput();
                            string element = ELS[rand.Next(ELS.Length)];
                            input.Add(new NInchiAtom(0, 0, 0, element));
                            input.Atoms[0].ImplicitH = 0;
                            try
                            {
                                NInchiOutput output = NInchiWrapper.GetInchi(input);

                                if (INCHI_RET.OKAY != output.ReturnStatus)
                                {
                                    if (INCHI_RET.BUSY != output.ReturnStatus)
                                    {
                                        failCount++;
                                    }
                                }
                                else if (!("InChI=1S/" + element)
                                        .Equals(output.Inchi))
                                {
                                    failCount++;
                                }
                            }
                            catch (Exception e)
                            {
                                failCount++;
                                Console.Error.WriteLine("Thread " + i + " error: " + e.Message);
                                break;
                            }

                            Thread.Yield();
                        }
                        done = true;
                    });
                    tts[i] = new Thread(ts);
                }
            }

            for (int i = 0; i < tts.Length; i++)
                tts[i].Start();

            Thread.Sleep(50);
            stop = true;

            for (int i = 0; i < tts.Length; i++)
                tts[i].Join();

            Assert.AreEqual(0, failCount, "Fail count");
        }


        [TestMethod()]
        public void TestTooManyAtoms()
        {
            NInchiInput input = new NInchiInput();
            for (int i = 0; i < 2000; i++)
            {
                input.Add(new NInchiAtom(0, 0, 0, "C"));
            }
            try
            {
                NInchiWrapper.GetInchi(input);
                Assert.Fail("too many atoms");
            }
            catch (ArgumentException)
            {
                ; // pass
            }
        }

        [TestMethod()]
        public void TestStructureToInchiBug2085031a()
        {
            NInchiInputInchi input = new NInchiInputInchi("InChI=1/C24H33N3O5/c1-23(2,3)26-21(29)20(17-11-8-7-9-12-17)27(16-18-13-10-14-31-18)19(28)15-25-22(30)32-24(4,5)6/h7-14,20H,15-16H2,1-6H3,(H,25,30)(H,26,29)");
            NInchiOutputStructure output = NInchiWrapper
                    .GetStructureFromInchi(input);
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
        }

        [TestMethod()]
        public void TestStructureToInchiBug2085031b()
        {
            NInchiInputInchi input = new NInchiInputInchi("InChI=1/C24H33N3O5/c1-23(2,3)26-21(29)20(17-11-8-7-9-12-17)27(16-18-13-10-14-31-18)19(28)15-25-22(30)32-24(4,5)6/h7-14,20H,15-16H2,1-6H3,(H,25,30)(H,26,29) ");
            NInchiOutputStructure output = NInchiWrapper
                    .GetStructureFromInchi(input);
            Assert.AreEqual(INCHI_RET.EOF, output.ReturnStatus);
        }


        [TestMethod()]
        public void TestGetStdInchi()
        {
            NInchiInput input = getLAlanine0D("");
            NInchiOutput output = NInchiWrapper.GetStdInchi(input);
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
            Assert.AreEqual(
                    "InChI=1S/C3H7NO2/c1-2(4)3(5)6/h2H,4H2,1H3,(H,5,6)/t2-/m0/s1",
                    output.Inchi);
        }


        // Test null inputs

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCheckInchiKeyNull()
        {
            NInchiWrapper.CheckInchiKey(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCheckOptionsListNull()
        {
            NInchiWrapper.CheckOptions((List<INCHI_OPTION>)null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCheckOptionsStringNull()
        {
            NInchiWrapper.CheckOptions((string)null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetInchiNull()
        {
            NInchiWrapper.GetInchi(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetInchiFromInchiNull()
        {
            NInchiWrapper.GetInchiFromInchi(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetInchiKeyNull()
        {
            NInchiWrapper.GetInchiKey(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetStdInchiNull()
        {
            NInchiWrapper.GetStdInchi(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetStructureFromInchiNull()
        {
            NInchiWrapper.GetStructureFromInchi(null);
        }

        [TestMethod()]
        public void TestGetInputFromAuxInfoLAlanine3D()
        {
            NInchiInputData data = NInchiWrapper.GetInputFromAuxInfo("AuxInfo=1/1/N:4,1,2,3,5,6/E:(5,6)/it:im/rA:6CCNCOO/rB:s1;s1;s1;s2;d2;/rC:-.358,.819,20.655;-1.598,-.032,20.905;-.275,2.014,21.574;.952,.043,20.838;-2.678,.479,21.093;-1.596,-1.239,20.958;");
            Assert.AreEqual(INCHI_RET.OKAY, data.ReturnStatus);
            NInchiInput input = data.Input;
            Assert.AreEqual(6, input.Atoms.Count);
            Assert.AreEqual("InChI Atom: C [-0.358,0.819,20.655] Charge:0 // Iso Mass:0 // Implicit H:-1 P:0 D:0 T:0 // Radical: None", input.Atoms[0].ToDebugString());
            Assert.AreEqual("InChI Atom: C [-1.598,-0.032,20.905] Charge:0 // Iso Mass:0 // Implicit H:-1 P:0 D:0 T:0 // Radical: None", input.Atoms[1].ToDebugString());
            Assert.AreEqual("InChI Atom: N [-0.275,2.014,21.574] Charge:0 // Iso Mass:0 // Implicit H:-1 P:0 D:0 T:0 // Radical: None", input.Atoms[2].ToDebugString());
            Assert.AreEqual("InChI Atom: C [0.952,0.043,20.838] Charge:0 // Iso Mass:0 // Implicit H:-1 P:0 D:0 T:0 // Radical: None", input.Atoms[3].ToDebugString());
            Assert.AreEqual("InChI Atom: O [-2.678,0.479,21.093] Charge:0 // Iso Mass:0 // Implicit H:-1 P:0 D:0 T:0 // Radical: None", input.Atoms[4].ToDebugString());
            Assert.AreEqual("InChI Atom: O [-1.596,-1.239,20.958] Charge:0 // Iso Mass:0 // Implicit H:-1 P:0 D:0 T:0 // Radical: None", input.Atoms[5].ToDebugString());
            Assert.AreEqual(5, input.Bonds.Count);
            Assert.AreEqual("InChI Bond: C-C // Type: Single // Stereo: None", input.Bonds[0].ToDebugString());
            Assert.AreEqual("InChI Bond: N-C // Type: Single // Stereo: None", input.Bonds[1].ToDebugString());
            Assert.AreEqual("InChI Bond: C-C // Type: Single // Stereo: None", input.Bonds[2].ToDebugString());
            Assert.AreEqual("InChI Bond: O-C // Type: Single // Stereo: None", input.Bonds[3].ToDebugString());
            Assert.AreEqual("InChI Bond: O-C // Type: Double // Stereo: None", input.Bonds[4].ToDebugString());
            Assert.AreEqual(0, input.Stereos.Count);
        }

        [TestMethod()]
        public void TestGetInputFromAuxInfoE12DiChloroEthane()
        {
            NInchiInputData data = NInchiWrapper.GetInputFromAuxInfo("AuxInfo=1/0/N:1,2,3,4/E:(1,2)(3,4)/rA:4CCClCl/rB:d+1;s1;s2;/rC:;;;;");
            Assert.AreEqual(INCHI_RET.OKAY, data.ReturnStatus);
            NInchiInput input = data.Input;
            Assert.AreEqual(4, input.Atoms.Count);
            Assert.AreEqual("InChI Atom: C [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:-1 P:0 D:0 T:0 // Radical: None", input.Atoms[0].ToDebugString());
            Assert.AreEqual("InChI Atom: C [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:-1 P:0 D:0 T:0 // Radical: None", input.Atoms[1].ToDebugString());
            Assert.AreEqual("InChI Atom: Cl [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:-1 P:0 D:0 T:0 // Radical: None", input.Atoms[2].ToDebugString());
            Assert.AreEqual("InChI Atom: Cl [0.0,0.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:-1 P:0 D:0 T:0 // Radical: None", input.Atoms[3].ToDebugString());
            Assert.AreEqual(3, input.Bonds.Count);
            Assert.AreEqual("InChI Bond: C-C // Type: Double // Stereo: None", input.Bonds[0].ToDebugString());
            Assert.AreEqual("InChI Bond: Cl-C // Type: Single // Stereo: None", input.Bonds[1].ToDebugString());
            Assert.AreEqual("InChI Bond: Cl-C // Type: Single // Stereo: None", input.Bonds[2].ToDebugString());
            Assert.AreEqual(1, input.Stereos.Count);
            Assert.AreEqual("InChI Stereo0D: - [Cl,C,C,Cl] Type::DoubleBond // Parity:Even", input.Stereos[0].ToDebugString());
        }

        [TestMethod()]
        public void TestGetInputFromLAlanine2D()
        {
            NInchiInputData data = NInchiWrapper.GetInputFromAuxInfo("AuxInfo=1/1/N:4,1,2,3,5,6/E:(5,6)/it:im/rA:6CCNCOO/rB:s1;N1;s1;s2;d2;/rC:264,968,0;295,985,0;233,986,0;264,932,0;326,967,0;295,1021,0;");
            Assert.AreEqual(INCHI_RET.OKAY, data.ReturnStatus);
            NInchiInput input = data.Input;
            Assert.AreEqual(6, input.Atoms.Count);
            Assert.AreEqual("InChI Atom: C [264.0,968.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:-1 P:0 D:0 T:0 // Radical: None", input.Atoms[0].ToDebugString());
            Assert.AreEqual("InChI Atom: C [295.0,985.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:-1 P:0 D:0 T:0 // Radical: None", input.Atoms[1].ToDebugString());
            Assert.AreEqual("InChI Atom: N [233.0,986.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:-1 P:0 D:0 T:0 // Radical: None", input.Atoms[2].ToDebugString());
            Assert.AreEqual("InChI Atom: C [264.0,932.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:-1 P:0 D:0 T:0 // Radical: None", input.Atoms[3].ToDebugString());
            Assert.AreEqual("InChI Atom: O [326.0,967.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:-1 P:0 D:0 T:0 // Radical: None", input.Atoms[4].ToDebugString());
            Assert.AreEqual("InChI Atom: O [295.0,1021.0,0.0] Charge:0 // Iso Mass:0 // Implicit H:-1 P:0 D:0 T:0 // Radical: None", input.Atoms[5].ToDebugString());
            Assert.AreEqual(5, input.Bonds.Count);
            Assert.AreEqual("InChI Bond: C-C // Type: Single // Stereo: None", input.Bonds[0].ToDebugString());
            Assert.AreEqual("InChI Bond: N-C // Type: Single // Stereo: Single2Down", input.Bonds[1].ToDebugString());
            Assert.AreEqual("InChI Bond: C-C // Type: Single // Stereo: None", input.Bonds[2].ToDebugString());
            Assert.AreEqual("InChI Bond: O-C // Type: Single // Stereo: None", input.Bonds[3].ToDebugString());
            Assert.AreEqual("InChI Bond: O-C // Type: Double // Stereo: None", input.Bonds[4].ToDebugString());
            Assert.AreEqual(0, input.Stereos.Count);
        }

        [TestMethod()]
        public void TestCheckInchiValidStd()
        {
            string inchi = "InChI=1S/C3H7NO2/c1-2(4)3(5)6/h2H,4H2,1H3,(H,5,6)/t2-/m0/s1";
            INCHI_STATUS ret = NInchiWrapper.CheckInchi(inchi, false);
            Assert.AreEqual(INCHI_STATUS.VALID_STANDARD, ret);
        }

        //@Ignore     // TODO -- this seems to be an InChI bug
        public void TestCheckInchiValidStdStrict()
        {
            string inchi = "InChI=1S/C4H6/c1-3-4-2/h3-4H,1-2H2";
            INCHI_STATUS ret = NInchiWrapper.CheckInchi(inchi, true);
            Assert.AreEqual(INCHI_STATUS.VALID_STANDARD, ret);
        }

        [TestMethod()]
        public void TestCheckInchiValidNonStd()
        {
            string inchi = "InChI=1/C8H10N4O2/c1-10-4-9-6-5(10)7(13)12(3)8(14)11(6)2/h4H,1-3H3";
            INCHI_STATUS ret = NInchiWrapper.CheckInchi(inchi, false);
            Assert.AreEqual(INCHI_STATUS.VALID_NON_STANDARD, ret);
        }

        [TestMethod()]
        public void TestCheckInchiInvalidVersion()
        {
            string inchi = "InChI=2/C8H10N4O2/c1-10-4-9-6-5(10)7(13)12(3)8(14)11(6)2/h4H,1-3H3";
            INCHI_STATUS ret = NInchiWrapper.CheckInchi(inchi, false);
            Assert.AreEqual(INCHI_STATUS.INVALID_VERSION, ret);
        }

        [TestMethod()]
        public void TestCheckInchiInvalidPrefix()
        {
            string inchi = "foo=1/C8H10N4O2/c1-10-4-9-6-5(10)7(13)12(3)8(14)11(6)2/h4H,1-3H3";
            INCHI_STATUS ret = NInchiWrapper.CheckInchi(inchi, false);
            Assert.AreEqual(INCHI_STATUS.INVALID_PREFIX, ret);
        }


        [TestMethod()]
        public void TestGetInchiFromInchi()
        {
            string inchi = "InChI=1S/C4H6/c1-3-4-2/h3-4H,1-2H2";
            NInchiInputInchi input = new NInchiInputInchi(inchi);
            NInchiOutput output = NInchiWrapper.GetInchiFromInchi(input);
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
            Assert.AreEqual(inchi, output.Inchi);
        }
    }
}
