/* Copyright (C) 2007  Ola Spjuth <ospjuth@users.sf.net>
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
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.IO;
using NCDK.IO.CML;
using NCDK.LibIO.CML;
using NCDK.Tools.Manipulator;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace NCDK.LibIO.MD
{
    // @cdk.module test-libiomd
    [TestClass()]
    public class MDMoleculeTest
        : CDKTestCase
    {
        private static readonly IChemObjectBuilder builder = CDK.Builder;

        // @cdk.bug 1748257
        [TestMethod()]
        public void TestBug1748257()
        {
            MDMolecule mol = new MDMolecule();
            mol.Atoms.Add(builder.NewAtom("C")); // 0
            mol.Atoms.Add(builder.NewAtom("C")); // 1
            mol.Atoms.Add(builder.NewAtom("H")); // 2
            mol.Atoms.Add(builder.NewAtom("H")); // 3
            mol.Atoms.Add(builder.NewAtom("H")); // 4
            mol.Atoms.Add(builder.NewAtom("H")); // 5

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double); // 1
            mol.AddBond(mol.Atoms[2], mol.Atoms[0], BondOrder.Single); // 3
            mol.AddBond(mol.Atoms[3], mol.Atoms[0], BondOrder.Single); // 4
            mol.AddBond(mol.Atoms[4], mol.Atoms[1], BondOrder.Single); // 5
            mol.AddBond(mol.Atoms[5], mol.Atoms[1], BondOrder.Single); // 6

            Convertor convertor = new Convertor(false, "");
            CMLAtom cmlatom = convertor.CDKAtomToCMLAtom(mol, mol.Atoms[2]);
            Assert.AreEqual(cmlatom.HydrogenCount, 0);
        }

        /// <summary>
        /// Test an MDMolecule with residues and charge groups
        /// </summary>
        [TestMethod()]
        public void TestMDMolecule()
        {
            MDMolecule mol = new MDMolecule();
            mol.Atoms.Add(builder.NewAtom("C")); // 0
            mol.Atoms.Add(builder.NewAtom("C")); // 1
            mol.Atoms.Add(builder.NewAtom("C")); // 2
            mol.Atoms.Add(builder.NewAtom("C")); // 3
            mol.Atoms.Add(builder.NewAtom("C")); // 4
            mol.Atoms.Add(builder.NewAtom("C")); // 5

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single); // 1
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double); // 2
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single); // 3
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Double); // 4
            mol.AddBond(mol.Atoms[4], mol.Atoms[5], BondOrder.Single); // 5
            mol.AddBond(mol.Atoms[5], mol.Atoms[0], BondOrder.Double); // 6

            //Create 2 residues
            var ac = builder.NewAtomContainer();
            ac.Atoms.Add(mol.Atoms[0]);
            ac.Atoms.Add(mol.Atoms[1]);
            ac.Atoms.Add(mol.Atoms[2]);
            Residue res1 = new Residue(ac, 0, mol);
            res1.Name = "myResidue1";
            mol.AddResidue(res1);

            var ac2 = builder.NewAtomContainer();
            ac2.Atoms.Add(mol.Atoms[3]);
            ac2.Atoms.Add(mol.Atoms[4]);
            ac2.Atoms.Add(mol.Atoms[5]);
            Residue res2 = new Residue(ac2, 1, mol);
            res2.Name = "myResidue2";
            mol.AddResidue(res2);

            //Test residue creation
            Assert.AreEqual(res1.GetParentMolecule(), mol);
            Assert.AreEqual(res2.GetParentMolecule(), mol);
            Assert.AreEqual(res1.Atoms.Count, 3);
            Assert.AreEqual(res2.Atoms.Count, 3);
            Assert.AreEqual(res1.Name, "myResidue1");
            Assert.AreEqual(res2.Name, "myResidue2");
            Assert.IsNotNull(mol.GetResidues());
            Assert.AreEqual(mol.GetResidues().Count, 2);
            Assert.AreEqual(mol.GetResidues()[0], res1);
            Assert.AreEqual(mol.GetResidues()[1], res2);

            //Create 2 chargegroups
            var ac3 = builder.NewAtomContainer();
            ac3.Atoms.Add(mol.Atoms[0]);
            ac3.Atoms.Add(mol.Atoms[1]);
            ChargeGroup chg1 = new ChargeGroup(ac3, 0, mol);
            mol.AddChargeGroup(chg1);

            var ac4 = builder.NewAtomContainer();
            ac4.Atoms.Add(mol.Atoms[2]);
            ac4.Atoms.Add(mol.Atoms[3]);
            ac4.Atoms.Add(mol.Atoms[4]);
            ac4.Atoms.Add(mol.Atoms[5]);
            ChargeGroup chg2 = new ChargeGroup(ac4, 1, mol);
            mol.AddChargeGroup(chg2);

            //Test chargegroup creation
            Assert.AreEqual(chg1.GetParentMolecule(), mol);
            Assert.AreEqual(chg2.GetParentMolecule(), mol);
            Assert.AreEqual(chg1.Atoms.Count, 2);
            Assert.AreEqual(chg2.Atoms.Count, 4);

            Assert.IsNotNull(mol.GetChargeGroups());
            Assert.AreEqual(mol.GetChargeGroups().Count, 2);
            Assert.AreEqual(mol.GetChargeGroups()[0], chg1);
            Assert.AreEqual(mol.GetChargeGroups()[1], chg2);
        }

        [TestMethod()]
        public void TestMDMoleculeCustomizationRoundtripping()
        {
            StringWriter writer = new StringWriter();

            CMLWriter cmlWriter = new CMLWriter(writer);
            cmlWriter.RegisterCustomizer(new MDMoleculeCustomizer());
            MDMolecule molecule = MakeMDBenzene();
            cmlWriter.Write(molecule);
            cmlWriter.Close();

            string serializedMol = writer.ToString();
            Debug.WriteLine("****************************** TestMDMoleculeCustomizationRoundtripping()");
            Debug.WriteLine(serializedMol);
            Debug.WriteLine("******************************");
            Debug.WriteLine("****************************** testMDMoleculeCustomization Write first");
            Debug.WriteLine(serializedMol);
            Debug.WriteLine("******************************");

            CMLReader reader = new CMLReader(new MemoryStream(Encoding.UTF8.GetBytes(serializedMol)));
            reader.RegisterConvention("md:mdMolecule", new MDMoleculeConvention(builder.NewChemFile()));
            IChemFile file = (IChemFile)reader.Read(builder.NewChemFile());
            reader.Close();
            var containers = ChemFileManipulator.GetAllAtomContainers(file).ToReadOnlyList();
            Assert.AreEqual(1, containers.Count);

            var molecule2 = containers[0];
            Assert.IsTrue(molecule2 is MDMolecule);
            MDMolecule mdMol = (MDMolecule)molecule2;

            Assert.AreEqual(6, mdMol.Atoms.Count);
            Assert.AreEqual(6, mdMol.Bonds.Count);

            var residues = mdMol.GetResidues();
            Assert.AreEqual(2, residues.Count);
            Assert.AreEqual(3, ((Residue)residues[0]).Atoms.Count);
            Assert.AreEqual(3, ((Residue)residues[1]).Atoms.Count);
            Assert.AreEqual("myResidue1", ((Residue)residues[0]).Name);
            Assert.AreEqual("myResidue2", ((Residue)residues[1]).Name);
            Assert.AreEqual(0, ((Residue)residues[0]).GetNumber());
            Assert.AreEqual(1, ((Residue)residues[1]).GetNumber());

            var chargeGroup = mdMol.GetChargeGroups();
            Assert.AreEqual(2, chargeGroup.Count);
            Assert.AreEqual(2, ((ChargeGroup)chargeGroup[0]).Atoms.Count);
            Assert.AreEqual(4, ((ChargeGroup)chargeGroup[1]).Atoms.Count);
            Assert.IsNotNull(((ChargeGroup)chargeGroup[0]).GetSwitchingAtom());
            Assert.AreEqual("a2", ((ChargeGroup)chargeGroup[0]).GetSwitchingAtom().Id);
            Assert.IsNotNull(((ChargeGroup)chargeGroup[1]).GetSwitchingAtom());
            Assert.AreEqual("a5", ((ChargeGroup)chargeGroup[1]).GetSwitchingAtom().Id);

            Assert.AreEqual(2, ((ChargeGroup)chargeGroup[0]).GetNumber());
            Assert.AreEqual(3, ((ChargeGroup)chargeGroup[1]).GetNumber());

            writer = new StringWriter();

            cmlWriter = new CMLWriter(writer);
            cmlWriter.RegisterCustomizer(new MDMoleculeCustomizer());
            cmlWriter.Write(mdMol);
            cmlWriter.Close();

            string serializedMDMol = writer.ToString();
            Debug.WriteLine("****************************** TestMDMoleculeCustomizationRoundtripping()");
            Debug.WriteLine(serializedMol);
            Debug.WriteLine("******************************");
            Debug.WriteLine("****************************** testMDMoleculeCustomization Write second");
            Debug.WriteLine(serializedMDMol);
            Debug.WriteLine("******************************");

            Assert.AreEqual(serializedMol, serializedMDMol);
        }

        [TestMethod()]
        public void TestMDMoleculeCustomization()
        {
            StringWriter writer = new StringWriter();

            CMLWriter cmlWriter = new CMLWriter(writer);
            cmlWriter.RegisterCustomizer(new MDMoleculeCustomizer());
            try
            {
                IAtomContainer molecule = MakeMDBenzene();
                cmlWriter.Write(molecule);
                cmlWriter.Close();
            }
            catch (Exception exception)
            {
                if (!(exception is CDKException || exception is IOException))
                    throw;
                {
                    Trace.TraceError($"Error while creating an CML2 file: {exception.Message}");
                    Debug.WriteLine(exception);
                    Assert.Fail(exception.Message);
                }
            }
            string cmlContent = writer.ToString();
            Debug.WriteLine("****************************** TestMDMoleculeCustomization()");
            Debug.WriteLine(cmlContent);
            Debug.WriteLine("******************************");
            //        Console.Out.WriteLine("****************************** TestMDMoleculeCustomization()");
            //        Console.Out.WriteLine(cmlContent);
            //        Console.Out.WriteLine("******************************");
            Assert.IsTrue(cmlContent.IndexOf("xmlns:md") != -1);
            Assert.IsTrue(cmlContent.IndexOf("md:residue\"") != -1);
            Assert.IsTrue(cmlContent.IndexOf("md:resNumber\"") != -1);
            Assert.IsTrue(cmlContent.IndexOf("md:chargeGroup\"") != -1);
            Assert.IsTrue(cmlContent.IndexOf("md:cgNumber\"") != -1);
            Assert.IsTrue(cmlContent.IndexOf("md:switchingAtom\"") != -1);
        }

        /// <summary>
        /// Create a benzene molecule with 2 residues and 2 charge groups
        /// </summary>
        /// <returns></returns>
        public MDMolecule MakeMDBenzene()
        {
            MDMolecule mol = new MDMolecule();
            mol.Atoms.Add(builder.NewAtom("C")); // 0
            mol.Atoms.Add(builder.NewAtom("C")); // 1
            mol.Atoms.Add(builder.NewAtom("C")); // 2
            mol.Atoms.Add(builder.NewAtom("C")); // 3
            mol.Atoms.Add(builder.NewAtom("C")); // 4
            mol.Atoms.Add(builder.NewAtom("C")); // 5

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single); // 1
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double); // 2
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single); // 3
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Double); // 4
            mol.AddBond(mol.Atoms[4], mol.Atoms[5], BondOrder.Single); // 5
            mol.AddBond(mol.Atoms[5], mol.Atoms[0], BondOrder.Double); // 6

            //Create 2 residues
            var ac = builder.NewAtomContainer();
            ac.Atoms.Add(mol.Atoms[0]);
            ac.Atoms.Add(mol.Atoms[1]);
            ac.Atoms.Add(mol.Atoms[2]);
            Residue res1 = new Residue(ac, 0, mol);
            res1.Name = "myResidue1";
            mol.AddResidue(res1);

            var ac2 = builder.NewAtomContainer();
            ac2.Atoms.Add(mol.Atoms[3]);
            ac2.Atoms.Add(mol.Atoms[4]);
            ac2.Atoms.Add(mol.Atoms[5]);
            Residue res2 = new Residue(ac2, 1, mol);
            res2.Name = "myResidue2";
            mol.AddResidue(res2);

            //Create 2 chargegroups
            var ac3 = builder.NewAtomContainer();
            ac3.Atoms.Add(mol.Atoms[0]);
            ac3.Atoms.Add(mol.Atoms[1]);
            ChargeGroup chg1 = new ChargeGroup(ac3, 2, mol);
            chg1.SetSwitchingAtom(mol.Atoms[1]);
            mol.AddChargeGroup(chg1);

            var ac4 = builder.NewAtomContainer();
            ac4.Atoms.Add(mol.Atoms[2]);
            ac4.Atoms.Add(mol.Atoms[3]);
            ac4.Atoms.Add(mol.Atoms[4]);
            ac4.Atoms.Add(mol.Atoms[5]);
            ChargeGroup chg2 = new ChargeGroup(ac4, 3, mol);
            chg2.SetSwitchingAtom(mol.Atoms[4]);
            mol.AddChargeGroup(chg2);

            return mol;
        }
    }
}
