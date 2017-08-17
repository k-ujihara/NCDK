/* Copyright (C) 2009-2010  Syed Asad Rahman <asad@ebi.ac.uk>
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
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 * MX Cheminformatics Tools for Java
 *
 * Copyright (c) 2007-2009 Metamolecular, LLC
 *
 * http://metamolecular.com
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
using NCDK.Aromaticities;
using NCDK.Default;
using NCDK.Smiles;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System;

namespace NCDK.SMSD.Algorithms.VFLib
{
    /// <summary>
    /// query and target molecules.
    /// </summary>
    // @cdk.module test-smsd
    // @cdk.githash
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    public class Molecules
    {
        public static IAtomContainer Create4Toluene()
        {
            IAtomContainer result = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            IAtom c1 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c1.Id = "1";
            IAtom c2 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c2.Id = "2";
            IAtom c3 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c3.Id = "3";
            IAtom c4 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c4.Id = "4";
            IAtom c5 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c5.Id = "5";
            IAtom c6 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c6.Id = "6";
            IAtom c7 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c7.Id = "7";

            result.Atoms.Add(c1);
            result.Atoms.Add(c2);
            result.Atoms.Add(c3);
            result.Atoms.Add(c4);
            result.Atoms.Add(c5);
            result.Atoms.Add(c6);
            result.Atoms.Add(c7);

            IBond bond1 = new Bond(c1, c2, BondOrder.Single);
            IBond bond2 = new Bond(c2, c3, BondOrder.Double);
            IBond bond3 = new Bond(c3, c4, BondOrder.Single);
            IBond bond4 = new Bond(c4, c5, BondOrder.Double);
            IBond bond5 = new Bond(c5, c6, BondOrder.Single);
            IBond bond6 = new Bond(c6, c1, BondOrder.Double);
            IBond bond7 = new Bond(c7, c4, BondOrder.Single);

            result.Bonds.Add(bond1);
            result.Bonds.Add(bond2);
            result.Bonds.Add(bond3);
            result.Bonds.Add(bond4);
            result.Bonds.Add(bond5);
            result.Bonds.Add(bond6);
            result.Bonds.Add(bond7);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(result);
            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(result.Builder);
            adder.AddImplicitHydrogens(result);
            Aromaticity.CDKLegacy.Apply(result);

            return result;
        }

        public static IAtomContainer CreateMethane()
        {
            IAtomContainer result = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            IAtom c1 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            result.Atoms.Add(c1);
            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(result.Builder);
            adder.AddImplicitHydrogens(result);

            return result;
        }

        public static IAtomContainer CreatePropane()
        {
            IAtomContainer result = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            IAtom c1 = result.Builder.NewAtom("C");
            IAtom c2 = result.Builder.NewAtom("C");
            IAtom c3 = result.Builder.NewAtom("C");

            result.Atoms.Add(c1);
            result.Atoms.Add(c2);
            result.Atoms.Add(c3);

            IBond bond1 = new Bond(c1, c2, BondOrder.Single);
            IBond bond2 = new Bond(c2, c3, BondOrder.Single);

            result.Bonds.Add(bond1);
            result.Bonds.Add(bond2);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(result);
            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(result.Builder);
            adder.AddImplicitHydrogens(result);
            Aromaticity.CDKLegacy.Apply(result);

            SmilesGenerator sg = new SmilesGenerator();
            string oldSmiles = sg.Create(result);
            Console.Out.WriteLine("Propane " + oldSmiles);

            return result;
        }

        public static IAtomContainer CreateHexane()
        {
            IAtomContainer result = Default.ChemObjectBuilder.Instance.NewAtomContainer();

            IAtom c1 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c1.Id = "1";
            IAtom c2 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c2.Id = "2";
            IAtom c3 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c3.Id = "3";
            IAtom c4 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c4.Id = "4";
            IAtom c5 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c5.Id = "5";
            IAtom c6 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c6.Id = "6";

            result.Atoms.Add(c1);
            result.Atoms.Add(c2);
            result.Atoms.Add(c3);
            result.Atoms.Add(c4);
            result.Atoms.Add(c5);
            result.Atoms.Add(c6);

            IBond bond1 = new Bond(c1, c2, BondOrder.Single);
            IBond bond2 = new Bond(c2, c3, BondOrder.Single);
            IBond bond3 = new Bond(c3, c4, BondOrder.Single);
            IBond bond4 = new Bond(c4, c5, BondOrder.Single);
            IBond bond5 = new Bond(c5, c6, BondOrder.Single);

            result.Bonds.Add(bond1);
            result.Bonds.Add(bond2);
            result.Bonds.Add(bond3);
            result.Bonds.Add(bond4);
            result.Bonds.Add(bond5);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(result);
            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(result.Builder);
            adder.AddImplicitHydrogens(result);
            Aromaticity.CDKLegacy.Apply(result);

            SmilesGenerator sg = new SmilesGenerator();
            string oldSmiles = sg.Create(result);
            Console.Out.WriteLine("Hexane " + oldSmiles);

            return result;
        }

        public static IAtomContainer CreateBenzene()
        {
            IAtomContainer result = Default.ChemObjectBuilder.Instance.NewAtomContainer();

            IAtom c1 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c1.Id = "1";
            IAtom c2 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c2.Id = "2";
            IAtom c3 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c3.Id = "3";
            IAtom c4 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c4.Id = "4";
            IAtom c5 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c5.Id = "5";
            IAtom c6 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c6.Id = "6";

            result.Atoms.Add(c1);
            result.Atoms.Add(c2);
            result.Atoms.Add(c3);
            result.Atoms.Add(c4);
            result.Atoms.Add(c5);
            result.Atoms.Add(c6);

            IBond bond1 = new Bond(c1, c2, BondOrder.Single);
            IBond bond2 = new Bond(c2, c3, BondOrder.Double);
            IBond bond3 = new Bond(c3, c4, BondOrder.Single);
            IBond bond4 = new Bond(c4, c5, BondOrder.Double);
            IBond bond5 = new Bond(c5, c6, BondOrder.Single);
            IBond bond6 = new Bond(c6, c1, BondOrder.Double);

            result.Bonds.Add(bond1);
            result.Bonds.Add(bond2);
            result.Bonds.Add(bond3);
            result.Bonds.Add(bond4);
            result.Bonds.Add(bond5);
            result.Bonds.Add(bond6);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(result);
            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(result.Builder);
            adder.AddImplicitHydrogens(result);
            Aromaticity.CDKLegacy.Apply(result);

            SmilesGenerator sg = new SmilesGenerator();
            string oldSmiles = sg.Create(result);
            Console.Out.WriteLine("Benzene " + oldSmiles);

            return result;
        }

        public static IAtomContainer CreateNaphthalene()
        {
            IAtomContainer result = Default.ChemObjectBuilder.Instance.NewAtomContainer();

            IAtom c1 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c1.Id = "1";
            IAtom c2 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c2.Id = "2";
            IAtom c3 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c3.Id = "3";
            IAtom c4 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c4.Id = "4";
            IAtom c5 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c5.Id = "5";
            IAtom c6 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c6.Id = "6";
            IAtom c7 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c4.Id = "7";
            IAtom c8 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c5.Id = "8";
            IAtom c9 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c6.Id = "9";
            IAtom c10 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c6.Id = "10";

            result.Atoms.Add(c1);
            result.Atoms.Add(c2);
            result.Atoms.Add(c3);
            result.Atoms.Add(c4);
            result.Atoms.Add(c5);
            result.Atoms.Add(c6);
            result.Atoms.Add(c7);
            result.Atoms.Add(c8);
            result.Atoms.Add(c9);
            result.Atoms.Add(c10);

            IBond bond1 = new Bond(c1, c2, BondOrder.Single);
            IBond bond2 = new Bond(c2, c3, BondOrder.Double);
            IBond bond3 = new Bond(c3, c4, BondOrder.Single);
            IBond bond4 = new Bond(c4, c5, BondOrder.Double);
            IBond bond5 = new Bond(c5, c6, BondOrder.Single);
            IBond bond6 = new Bond(c6, c1, BondOrder.Double);
            IBond bond7 = new Bond(c5, c7, BondOrder.Single);
            IBond bond8 = new Bond(c7, c8, BondOrder.Double);
            IBond bond9 = new Bond(c8, c9, BondOrder.Single);
            IBond bond10 = new Bond(c9, c10, BondOrder.Double);
            IBond bond11 = new Bond(c10, c6, BondOrder.Single);

            result.Bonds.Add(bond1);
            result.Bonds.Add(bond2);
            result.Bonds.Add(bond3);
            result.Bonds.Add(bond4);
            result.Bonds.Add(bond5);
            result.Bonds.Add(bond6);
            result.Bonds.Add(bond7);
            result.Bonds.Add(bond8);
            result.Bonds.Add(bond9);
            result.Bonds.Add(bond10);
            result.Bonds.Add(bond11);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(result);
            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(result.Builder);
            adder.AddImplicitHydrogens(result);
            Aromaticity.CDKLegacy.Apply(result);

            SmilesGenerator sg = new SmilesGenerator();
            string oldSmiles = sg.Create(result);
            Console.Out.WriteLine("Naphthalene " + oldSmiles);

            return result;
        }

        public static IAtomContainer CreatePyridazine()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            string smiles = "C1=CN=NC=C1";
            IAtomContainer result = sp.ParseSmiles(smiles);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(result);
            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(result.Builder);
            adder.AddImplicitHydrogens(result);
            Aromaticity.CDKLegacy.Apply(result);
            return result;
        }

        public static IAtomContainer CreateChloroIsoquinoline4()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            string smiles = "ClC1=CC=NC2=C1C=CC=C2";
            IAtomContainer result = sp.ParseSmiles(smiles);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(result);
            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(result.Builder);
            adder.AddImplicitHydrogens(result);
            Aromaticity.CDKLegacy.Apply(result);
            return result;
        }

        public static IAtomContainer CreateChlorobenzene()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            string smiles = "Clc1ccccc1";
            IAtomContainer result = sp.ParseSmiles(smiles);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(result);
            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(result.Builder);
            adder.AddImplicitHydrogens(result);
            Aromaticity.CDKLegacy.Apply(result);
            return result;
        }

        public static IAtomContainer CreateAcetone()
        {
            IAtomContainer result = Default.ChemObjectBuilder.Instance.NewAtomContainer();

            IAtom c1 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c1.Id = "1";
            IAtom c2 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c2.Id = "2";
            IAtom c3 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c3.Id = "3";
            IAtom c4 = Default.ChemObjectBuilder.Instance.NewAtom("O");
            c4.Id = "4";

            result.Atoms.Add(c1);
            result.Atoms.Add(c2);
            result.Atoms.Add(c3);
            result.Atoms.Add(c4);

            IBond bond1 = new Bond(c1, c2, BondOrder.Single);
            IBond bond2 = new Bond(c2, c3, BondOrder.Single);
            IBond bond3 = new Bond(c3, c4, BondOrder.Double);

            result.Bonds.Add(bond1);
            result.Bonds.Add(bond2);
            result.Bonds.Add(bond3);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(result);
            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(result.Builder);
            adder.AddImplicitHydrogens(result);
            Aromaticity.CDKLegacy.Apply(result);

            return result;
        }

        //
        //    public static Molecule CreateNeopentane() {
        //        Molecule result = new DefaultMolecule();
        //        Atom c0 = result.Atoms.Add("C");
        //        Atom c1 = result.Atoms.Add("C");
        //        Atom c2 = result.Atoms.Add("C");
        //        Atom c3 = result.Atoms.Add("C");
        //        Atom c4 = result.Atoms.Add("C");
        //
        //        result.Connect(c0, c1, 1);
        //        result.Connect(c0, c2, 1);
        //        result.Connect(c0, c3, 1);
        //        result.Connect(c0, c4, 1);
        //
        //        return result;
        //    }
        //

        public static IAtomContainer CreateCubane()
        {
            IAtomContainer result = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            IAtom c1 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c1.Id = "1";
            IAtom c2 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c2.Id = "2";
            IAtom c3 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c3.Id = "3";
            IAtom c4 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c4.Id = "4";
            IAtom c5 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c5.Id = "5";
            IAtom c6 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c6.Id = "6";
            IAtom c7 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c7.Id = "7";
            IAtom c8 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c8.Id = "8";

            result.Atoms.Add(c1);
            result.Atoms.Add(c2);
            result.Atoms.Add(c3);
            result.Atoms.Add(c4);
            result.Atoms.Add(c5);
            result.Atoms.Add(c6);
            result.Atoms.Add(c7);
            result.Atoms.Add(c8);

            IBond bond1 = new Bond(c1, c2, BondOrder.Single);
            IBond bond2 = new Bond(c2, c3, BondOrder.Single);
            IBond bond3 = new Bond(c3, c4, BondOrder.Single);
            IBond bond4 = new Bond(c4, c1, BondOrder.Single);

            IBond bond5 = new Bond(c5, c6, BondOrder.Single);
            IBond bond6 = new Bond(c6, c7, BondOrder.Single);
            IBond bond7 = new Bond(c7, c8, BondOrder.Single);
            IBond bond8 = new Bond(c8, c5, BondOrder.Single);

            IBond bond9 = new Bond(c1, c5, BondOrder.Single);
            IBond bond10 = new Bond(c2, c6, BondOrder.Single);
            IBond bond11 = new Bond(c3, c7, BondOrder.Single);
            IBond bond12 = new Bond(c4, c8, BondOrder.Single);

            result.Bonds.Add(bond1);
            result.Bonds.Add(bond2);
            result.Bonds.Add(bond3);
            result.Bonds.Add(bond4);
            result.Bonds.Add(bond5);
            result.Bonds.Add(bond6);
            result.Bonds.Add(bond7);
            result.Bonds.Add(bond8);
            result.Bonds.Add(bond9);
            result.Bonds.Add(bond10);
            result.Bonds.Add(bond11);
            result.Bonds.Add(bond12);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(result);
            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(result.Builder);
            adder.AddImplicitHydrogens(result);
            Aromaticity.CDKLegacy.Apply(result);

            return result;
        }

        //
        //    public static Molecule CreateBicyclo220hexane() {
        //        Molecule result = new DefaultMolecule();
        //        Atom c0 = result.Atoms.Add("C");
        //        Atom c1 = result.Atoms.Add("C");
        //        Atom c2 = result.Atoms.Add("C");
        //        Atom c3 = result.Atoms.Add("C");
        //        Atom c4 = result.Atoms.Add("C");
        //        Atom c5 = result.Atoms.Add("C");
        //
        //        result.Connect(c0, c1, 1);
        //        result.Connect(c1, c2, 1);
        //        result.Connect(c2, c3, 1);
        //        result.Connect(c3, c4, 1);
        //        result.Connect(c4, c5, 1);
        //        result.Connect(c5, c0, 1);
        //        result.Connect(c2, c5, 1);
        //
        //        return result;
        //    }
        //
        //    public static Molecule CreateEthylbenzeneWithSuperatom() {
        //        Molecule result = Molecules.CreateBenzene();
        //        Atom carbon1 = result.Atoms.Add("C");
        //        Atom carbon2 = result.Atoms.Add("C");
        //        Bond crossingBond = result.Connect(result.Atoms[0], carbon1, 1);
        //        result.Connect(carbon1, carbon2, 1);
        //
        //        Superatom substructure = result.AddSuperatom();
        //        substructure.Atoms.Add(carbon1);
        //        substructure.Atoms.Add(carbon2);
        //        substructure.AddCrossingBond(crossingBond);
        //        substructure.SetCrossingVector(crossingBond, 0.1, 0.1);
        //        substructure.Label = "Ethyl";
        //
        //        return result;
        //    }
        //

        public static IAtomContainer CreatePyridine()
        {
            IAtomContainer result = Default.ChemObjectBuilder.Instance.NewAtomContainer();

            IAtom c1 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c1.Id = "1";
            IAtom c2 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c2.Id = "2";
            IAtom c3 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c3.Id = "3";
            IAtom c4 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c4.Id = "4";
            IAtom c5 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c5.Id = "5";
            IAtom c6 = Default.ChemObjectBuilder.Instance.NewAtom("N");
            c6.Id = "6";

            result.Atoms.Add(c1);
            result.Atoms.Add(c2);
            result.Atoms.Add(c3);
            result.Atoms.Add(c4);
            result.Atoms.Add(c5);
            result.Atoms.Add(c6);

            IBond bond1 = new Bond(c1, c2, BondOrder.Single);
            IBond bond2 = new Bond(c2, c3, BondOrder.Double);
            IBond bond3 = new Bond(c3, c4, BondOrder.Single);
            IBond bond4 = new Bond(c4, c5, BondOrder.Double);
            IBond bond5 = new Bond(c5, c6, BondOrder.Single);
            IBond bond6 = new Bond(c6, c1, BondOrder.Double);

            result.Bonds.Add(bond1);
            result.Bonds.Add(bond2);
            result.Bonds.Add(bond3);
            result.Bonds.Add(bond4);
            result.Bonds.Add(bond5);
            result.Bonds.Add(bond6);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(result);
            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(result.Builder);
            adder.AddImplicitHydrogens(result);
            Aromaticity.CDKLegacy.Apply(result);

            return result;
        }

        public static IAtomContainer CreateToluene()
        {
            IAtomContainer result = Default.ChemObjectBuilder.Instance.NewAtomContainer();

            IAtom c1 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c1.Id = "1";
            IAtom c2 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c2.Id = "2";
            IAtom c3 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c3.Id = "3";
            IAtom c4 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c4.Id = "4";
            IAtom c5 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c5.Id = "5";
            IAtom c6 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c6.Id = "6";
            IAtom c7 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c6.Id = "7";

            result.Atoms.Add(c1);
            result.Atoms.Add(c2);
            result.Atoms.Add(c3);
            result.Atoms.Add(c4);
            result.Atoms.Add(c5);
            result.Atoms.Add(c6);
            result.Atoms.Add(c7);

            IBond bond1 = new Bond(c1, c2, BondOrder.Single);
            IBond bond2 = new Bond(c2, c3, BondOrder.Double);
            IBond bond3 = new Bond(c3, c4, BondOrder.Single);
            IBond bond4 = new Bond(c4, c5, BondOrder.Double);
            IBond bond5 = new Bond(c5, c6, BondOrder.Single);
            IBond bond6 = new Bond(c6, c1, BondOrder.Double);
            IBond bond7 = new Bond(c7, c1, BondOrder.Single);

            result.Bonds.Add(bond1);
            result.Bonds.Add(bond2);
            result.Bonds.Add(bond3);
            result.Bonds.Add(bond4);
            result.Bonds.Add(bond5);
            result.Bonds.Add(bond6);
            result.Bonds.Add(bond7);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(result);
            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(result.Builder);
            adder.AddImplicitHydrogens(result);
            Aromaticity.CDKLegacy.Apply(result);

            return result;
        }

        public static IAtomContainer CreatePhenol()
        {

            IAtomContainer result = Default.ChemObjectBuilder.Instance.NewAtomContainer();

            IAtom c1 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c1.Id = "1";
            IAtom c2 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c2.Id = "2";
            IAtom c3 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c3.Id = "3";
            IAtom c4 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c4.Id = "4";
            IAtom c5 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c5.Id = "5";
            IAtom c6 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c6.Id = "6";
            IAtom c7 = Default.ChemObjectBuilder.Instance.NewAtom("O");
            c6.Id = "7";

            result.Atoms.Add(c1);
            result.Atoms.Add(c2);
            result.Atoms.Add(c3);
            result.Atoms.Add(c4);
            result.Atoms.Add(c5);
            result.Atoms.Add(c6);
            result.Atoms.Add(c7);

            IBond bond1 = new Bond(c1, c2, BondOrder.Single);
            IBond bond2 = new Bond(c2, c3, BondOrder.Double);
            IBond bond3 = new Bond(c3, c4, BondOrder.Single);
            IBond bond4 = new Bond(c4, c5, BondOrder.Double);
            IBond bond5 = new Bond(c5, c6, BondOrder.Single);
            IBond bond6 = new Bond(c6, c1, BondOrder.Double);
            IBond bond7 = new Bond(c7, c1, BondOrder.Single);

            result.Bonds.Add(bond1);
            result.Bonds.Add(bond2);
            result.Bonds.Add(bond3);
            result.Bonds.Add(bond4);
            result.Bonds.Add(bond5);
            result.Bonds.Add(bond6);
            result.Bonds.Add(bond7);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(result);
            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(result.Builder);
            adder.AddImplicitHydrogens(result);
            Aromaticity.CDKLegacy.Apply(result);

            return result;
        }

        public static IAtomContainer CreateCyclohexane()
        {

            IAtomContainer result = Default.ChemObjectBuilder.Instance.NewAtomContainer();

            IAtom c1 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c1.Id = "1";
            IAtom c2 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c2.Id = "2";
            IAtom c3 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c3.Id = "3";
            IAtom c4 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c4.Id = "4";
            IAtom c5 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c5.Id = "5";
            IAtom c6 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            c6.Id = "6";

            result.Atoms.Add(c1);
            result.Atoms.Add(c2);
            result.Atoms.Add(c3);
            result.Atoms.Add(c4);
            result.Atoms.Add(c5);
            result.Atoms.Add(c6);

            IBond bond1 = new Bond(c1, c2, BondOrder.Single);
            IBond bond2 = new Bond(c2, c3, BondOrder.Single);
            IBond bond3 = new Bond(c3, c4, BondOrder.Single);
            IBond bond4 = new Bond(c4, c5, BondOrder.Single);
            IBond bond5 = new Bond(c5, c6, BondOrder.Single);
            IBond bond6 = new Bond(c6, c1, BondOrder.Single);

            result.Bonds.Add(bond1);
            result.Bonds.Add(bond2);
            result.Bonds.Add(bond3);
            result.Bonds.Add(bond4);
            result.Bonds.Add(bond5);
            result.Bonds.Add(bond6);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(result);
            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(result.Builder);
            adder.AddImplicitHydrogens(result);
            Aromaticity.CDKLegacy.Apply(result);

            return result;
        }

        public static IAtomContainer CreateCyclopropane()
        {
            IAtomContainer result = Default.ChemObjectBuilder.Instance.NewAtomContainer();

            IAtom c1 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            IAtom c2 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            IAtom c3 = Default.ChemObjectBuilder.Instance.NewAtom("C");

            result.Atoms.Add(c1);
            result.Atoms.Add(c2);
            result.Atoms.Add(c3);

            IBond bond1 = new Bond(c1, c2, BondOrder.Single);
            IBond bond2 = new Bond(c2, c3, BondOrder.Single);
            IBond bond3 = new Bond(c3, c1, BondOrder.Single);

            result.Bonds.Add(bond1);
            result.Bonds.Add(bond2);
            result.Bonds.Add(bond3);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(result);
            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(result.Builder);
            adder.AddImplicitHydrogens(result);
            Aromaticity.CDKLegacy.Apply(result);

            return result;
        }

        public static IAtomContainer CreateIsobutane()
        {
            IAtomContainer result = Default.ChemObjectBuilder.Instance.NewAtomContainer();

            IAtom c1 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            IAtom c2 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            IAtom c3 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            IAtom c4 = Default.ChemObjectBuilder.Instance.NewAtom("C");

            result.Atoms.Add(c1);
            result.Atoms.Add(c2);
            result.Atoms.Add(c3);
            result.Atoms.Add(c4);

            IBond bond1 = new Bond(c1, c2, BondOrder.Single);
            IBond bond2 = new Bond(c2, c3, BondOrder.Single);
            IBond bond3 = new Bond(c2, c4, BondOrder.Single);

            result.Bonds.Add(bond1);
            result.Bonds.Add(bond2);
            result.Bonds.Add(bond3);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(result);
            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(result.Builder);
            adder.AddImplicitHydrogens(result);
            Aromaticity.CDKLegacy.Apply(result);

            return result;
        }

        //    private IAtomContainer CreateBenzaldehyde() {
        //        Molecule result = new DefaultMolecule();
        //        Atom c1 = result.Atoms.Add("C");
        //        Atom c2 = result.Atoms.Add("C");
        //        Atom c3 = result.Atoms.Add("C");
        //        Atom c4 = result.Atoms.Add("C");
        //        Atom c5 = result.Atoms.Add("C");
        //        Atom c6 = result.Atoms.Add("C");
        //        Atom c7 = result.Atoms.Add("C");
        //        Atom o8 = result.Atoms.Add("O");
        //
        //        result.Connect(c1, c2, 1);
        //        result.Connect(c2, c3, 2);
        //        result.Connect(c3, c4, 1);
        //        result.Connect(c4, c5, 2);
        //        result.Connect(c5, c6, 1);
        //        result.Connect(c6, c1, 2);
        //        result.Connect(c7, c1, 1);
        //        result.Connect(c7, o8, 2);
        //
        //        return result;
        //    }
        //
        //    private IAtomContainer CreateBenzoicAcid() {
        //        Molecule result = CreateBenzaldehyde();
        //
        //        result.Connect(result.Atoms[6], result.Atoms.Add("O"), 1);
        //
        //        return result;
        //    }
        //
        //    private IAtomContainer CreateBlockedBenzaldehyde() {
        //        Molecule result = CreateBenzaldehyde();
        //
        //        result.Connect(result.Atoms[6], result.Atoms.Add("H"), 1);
        //
        //        return result;
        //    }
        //    private Molecule Create4Toluene() {
        //        Molecule result = new DefaultMolecule();
        //        Atom c1 = result.Atoms.Add("C");
        //        Atom c2 = result.Atoms.Add("C");
        //        Atom c3 = result.Atoms.Add("C");
        //        Atom c4 = result.Atoms.Add("C");
        //        Atom c5 = result.Atoms.Add("C");
        //        Atom c6 = result.Atoms.Add("C");
        //        Atom c7 = result.Atoms.Add("C");
        //
        //        result.Connect(c1, c2, 1);
        //        result.Connect(c2, c3, 2);
        //        result.Connect(c3, c4, 1);
        //        result.Connect(c4, c5, 2);
        //        result.Connect(c5, c6, 1);
        //        result.Connect(c6, c1, 2);
        //        result.Connect(c7, c4, 1);
        //
        //        return result;
        //    }
        public static IAtomContainer CreateSimpleImine()
        {
            IAtomContainer result = Default.ChemObjectBuilder.Instance.NewAtomContainer();

            IAtom c1 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            IAtom c2 = Default.ChemObjectBuilder.Instance.NewAtom("N");

            result.Atoms.Add(c1);
            result.Atoms.Add(c2);

            IBond bond = new Bond(c1, c2, BondOrder.Double);
            result.Bonds.Add(bond);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(result);
            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(result.Builder);
            adder.AddImplicitHydrogens(result);
            Aromaticity.CDKLegacy.Apply(result);

            SmilesGenerator sg = new SmilesGenerator();
            string oldSmiles = sg.Create(result);
            Console.Out.WriteLine("SimpleImine " + oldSmiles);

            return result;
        }

        public static IAtomContainer CreateSimpleAmine()
        {
            IAtomContainer result = Default.ChemObjectBuilder.Instance.NewAtomContainer();

            IAtom c1 = Default.ChemObjectBuilder.Instance.NewAtom("C");
            IAtom c2 = Default.ChemObjectBuilder.Instance.NewAtom("N");

            result.Atoms.Add(c1);
            result.Atoms.Add(c2);

            IBond bond = new Bond(c1, c2, BondOrder.Single);
            result.Bonds.Add(bond);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(result);
            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(result.Builder);
            adder.AddImplicitHydrogens(result);
            Aromaticity.CDKLegacy.Apply(result);

            SmilesGenerator sg = new SmilesGenerator();
            string oldSmiles = sg.Create(result);
            Console.Out.WriteLine("SimpleAmine " + oldSmiles);

            return result;
        }
    }
}
