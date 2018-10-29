/*
 * Copyright (c) 2013 European Bioinformatics Institute (EMBL-EBI)
 *                    John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or (at
 * your option) any later version. All we ask is that proper credit is given
 * for our work, which includes - but is not limited to - adding the above
 * copyright notice to the beginning of your source code files, and to any
 * copyright notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * Any WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Common.Base;
using NCDK.IO;
using NCDK.Numerics;
using NCDK.Silent;
using NCDK.Smiles;
using NCDK.Tools.Manipulator;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Stereo
{
    // @author John May
    // @cdk.module test-standard
    [TestClass()]
    public class StereoElementFactoryTest
    {
        // don't create double bond configs in benzene
        [TestMethod()]
        public void Benzene()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(Atom("C", 1, 1.30, -0.75));
            mol.Atoms.Add(Atom("C", 1, -0.00, -1.50));
            mol.Atoms.Add(Atom("C", 1, -1.30, -0.75));
            mol.Atoms.Add(Atom("C", 1, -1.30, 0.75));
            mol.Atoms.Add(Atom("C", 1, 0.00, 1.50));
            mol.Atoms.Add(Atom("C", 1, 1.30, 0.75));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Double);
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[4], mol.Atoms[5], BondOrder.Double);
            mol.AddBond(mol.Atoms[0], mol.Atoms[5], BondOrder.Single);
            StereoElementFactory factory = StereoElementFactory.Using2DCoordinates(mol);
            Assert.AreEqual(0, factory.CreateAll().Count());
        }

        // >=8 is okay for db stereo (ala inchi)
        [TestMethod()]
        public void Cyclooctatetraene()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(Atom("C", 1, -10.46, 6.36));
            mol.Atoms.Add(Atom("C", 1, -11.34, 5.15));
            mol.Atoms.Add(Atom("C", 1, -10.46, 3.93));
            mol.Atoms.Add(Atom("C", 1, -9.03, 4.40));
            mol.Atoms.Add(Atom("C", 1, -7.60, 3.93));
            mol.Atoms.Add(Atom("C", 1, -6.72, 5.15));
            mol.Atoms.Add(Atom("C", 1, -7.60, 6.36));
            mol.Atoms.Add(Atom("C", 1, -9.03, 5.90));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Double);
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[4], mol.Atoms[5], BondOrder.Double);
            mol.AddBond(mol.Atoms[5], mol.Atoms[6], BondOrder.Single);
            mol.AddBond(mol.Atoms[6], mol.Atoms[7], BondOrder.Double);
            mol.AddBond(mol.Atoms[0], mol.Atoms[7], BondOrder.Single);
            StereoElementFactory factory = StereoElementFactory.Using2DCoordinates(mol);
            Assert.AreEqual(4, factory.CreateAll().Count());
        }

        // not okay... but technically the trans form exists
        [TestMethod()]
        public void DoubleBondInSevenMemberedRing()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(Atom("C", 1, -10.46, 6.36));
            mol.Atoms.Add(Atom("C", 1, -11.34, 5.15));
            mol.Atoms.Add(Atom("C", 1, -10.46, 3.93));
            mol.Atoms.Add(Atom("C", 1, -9.03, 4.40));
            mol.Atoms.Add(Atom("C", 1, -7.60, 3.93));
            mol.Atoms.Add(Atom("C", 1, -6.72, 5.15));
            mol.Atoms.Add(Atom("C", 1, -7.60, 6.36));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[4], mol.Atoms[5], BondOrder.Single);
            mol.AddBond(mol.Atoms[5], mol.Atoms[6], BondOrder.Single);
            mol.AddBond(mol.Atoms[6], mol.Atoms[0], BondOrder.Single);
            StereoElementFactory factory = StereoElementFactory.Using2DCoordinates(mol);
            Assert.AreEqual(0, factory.CreateAll().Count());
        }

        [TestMethod()]
        public void HydrogenIsotope()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(Atom("C", 3, 0.00, 0.00));
            mol.Atoms.Add(Atom("C", 1, 1.30, -0.75));
            mol.Atoms.Add(Atom("C", 1, 2.60, -0.00));
            mol.Atoms.Add(Atom("H", 0, 3.90, -0.75));
            mol.Atoms[3].MassNumber = 2;
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            StereoElementFactory factory = StereoElementFactory.Using2DCoordinates(mol);
            Assert.AreEqual(1, factory.CreateAll().Count());
        }

        [TestMethod()]
        public void BridgeHeadNitrogen()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(Atom("C", 2, 1.23, 0.75));
            mol.Atoms.Add(Atom("C", 2, 1.23, -0.75));
            mol.Atoms.Add(Atom("N", 0, -0.07, -1.50));
            mol.Atoms.Add(Atom("C", 2, -1.36, -0.75));
            mol.Atoms.Add(Atom("C", 2, -1.36, 0.75));
            mol.Atoms.Add(Atom("N", 0, -0.07, 1.50));
            mol.Atoms.Add(Atom("C", 2, 0.39, -0.00));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single, BondStereo.None);
            mol.AddBond(mol.Atoms[2], mol.Atoms[1], BondOrder.Single, BondStereo.Up);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single, BondStereo.None);
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single, BondStereo.None);
            mol.AddBond(mol.Atoms[4], mol.Atoms[5], BondOrder.Single, BondStereo.None);
            mol.AddBond(mol.Atoms[5], mol.Atoms[0], BondOrder.Single, BondStereo.Up);
            mol.AddBond(mol.Atoms[5], mol.Atoms[6], BondOrder.Single, BondStereo.None);
            mol.AddBond(mol.Atoms[2], mol.Atoms[6], BondOrder.Single, BondStereo.None);
            StereoElementFactory factory = StereoElementFactory.Using2DCoordinates(mol);
            Assert.AreEqual(2, factory.CreateAll().Count());
        }

        [TestMethod()]
        public void E_but2ene()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 1, -2.19d, 1.64d));
            m.Atoms.Add(Atom("C", 1, -1.36d, 1.64d));
            m.Atoms.Add(Atom("C", 3, -2.60d, 0.92d));
            m.Atoms.Add(Atom("C", 3, -0.95d, 2.35d));

            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Double);
            m.AddBond(m.Atoms[0], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[3], BondOrder.Single);

            StereoElementFactory factory = StereoElementFactory.Using2DCoordinates(m);
            IDoubleBondStereochemistry element = factory.CreateGeometric(m.Bonds[0], null);

            Assert.IsNotNull(element);
            Assert.AreEqual(DoubleBondConformation.Opposite, element.Stereo);
        }

        [TestMethod()]
        public void Z_but2ene()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 3, -2.46d, 1.99d));
            m.Atoms.Add(Atom("C", 1, -1.74d, 0.68d));
            m.Atoms.Add(Atom("C", 1, -0.24d, 0.65d));
            m.Atoms.Add(Atom("C", 3, 0.54d, 1.94d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Single);

            StereoElementFactory factory = StereoElementFactory.Using2DCoordinates(m);
            IDoubleBondStereochemistry element = factory.CreateGeometric(m.Bonds[1], null);

            Assert.IsNotNull(element);
            Assert.AreEqual(DoubleBondConformation.Together, element.Stereo);
        }

        /// <summary>
        /// (E)-hexa-2,3,4-triene
        /// </summary>
        // @cdk.smiles C/C=C=C=C/C
        [TestMethod()]
        public void E_hexa234triene()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(Atom("C", 1, 2.48d, 0.00d));
            mol.Atoms.Add(Atom("C", 0, 1.65d, 0.00d));
            mol.Atoms.Add(Atom("C", 0, 0.83d, 0.00d));
            mol.Atoms.Add(Atom("C", 1, 0.00d, 0.00d));
            mol.Atoms.Add(Atom("C", 3, -0.41d, -0.71d));
            mol.Atoms.Add(Atom("C", 3, 2.89d, 0.71d));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Double);
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[5], BondOrder.Single);
            StereoElementFactory factory = StereoElementFactory.Using2DCoordinates(mol);
            var dbs = new List<IBond>();
            dbs.Add(mol.Bonds[0]);
            dbs.Add(mol.Bonds[1]);
            dbs.Add(mol.Bonds[2]);
            ExtendedCisTrans element = factory.CreateExtendedCisTrans(dbs, Stereocenters.Of(mol));
            Assert.IsNotNull(element);
            Assert.AreEqual(StereoConfigurations.Opposite, element.Configure);
        }

        /// <summary>
        /// (Z)-hexa-2,3,4-triene
        /// </summary>
        // @cdk.smiles C/C=C=C=C\C
        [TestMethod()]
        public void z_hexa234triene()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(Atom("C", 1, 2.48d, 0.00d));
            mol.Atoms.Add(Atom("C", 0, 1.65d, 0.00d));
            mol.Atoms.Add(Atom("C", 0, 0.83d, 0.00d));
            mol.Atoms.Add(Atom("C", 1, 0.00d, 0.00d));
            mol.Atoms.Add(Atom("C", 3, -0.41d, -0.71d));
            mol.Atoms.Add(Atom("C", 3, 2.92d, -0.69d));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Double);
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[5], BondOrder.Single);
            StereoElementFactory factory = StereoElementFactory.Using2DCoordinates(mol);
            var dbs = new List<IBond>();
            dbs.Add(mol.Bonds[0]);
            dbs.Add(mol.Bonds[1]);
            dbs.Add(mol.Bonds[2]);
            ExtendedCisTrans element = factory.CreateExtendedCisTrans(dbs, Stereocenters.Of(mol));
            Assert.IsNotNull(element);
            Assert.AreEqual(StereoConfigurations.Together, element.Configure);
        }

        /// <summary>
        /// (E)-hexa-2,3,4-triene
        /// </summary>
        // @cdk.smiles C/C=C=C=C/C
        [TestMethod()]
        public void E_hexa234triene_3D()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(Atom("C", 1, 0.29d, 0.01d, 0.02d));
            mol.Atoms.Add(Atom("C", 0, -0.56d, -0.90d, 0.25d));
            mol.Atoms.Add(Atom("C", 0, -1.37d, -1.75d, 0.46d));
            mol.Atoms.Add(Atom("C", 1, -2.24d, -2.65d, 0.67d));
            mol.Atoms.Add(Atom("C", 3, -3.66d, -2.36d, 0.68d));
            mol.Atoms.Add(Atom("C", 3, 1.69d, -0.32d, -0.11d));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Double);
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[5], BondOrder.Single);
            StereoElementFactory factory = StereoElementFactory.Using3DCoordinates(mol);
            var dbs = new List<IBond>();
            dbs.Add(mol.Bonds[0]);
            dbs.Add(mol.Bonds[1]);
            dbs.Add(mol.Bonds[2]);
            ExtendedCisTrans element = factory.CreateExtendedCisTrans(dbs, Stereocenters.Of(mol));
            Assert.IsNotNull(element);
            Assert.AreEqual(StereoConfigurations.Opposite, element.Configure);
        }

        /// <summary>
        /// (Z)-hexa-2,3,4-triene
        /// </summary>
        // @cdk.smiles C/C=C=C=C\C
        [TestMethod()]
        public void Z_hexa234triene_3D()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(Atom("C", 1, -0.09d, -0.45d, -1.07d));
            mol.Atoms.Add(Atom("C", 0, -0.67d, -1.04d, -0.11d));
            mol.Atoms.Add(Atom("C", 0, -1.23d, -1.59d, 0.79d));
            mol.Atoms.Add(Atom("C", 1, -1.84d, -2.17d, 1.74d));
            mol.Atoms.Add(Atom("C", 3, -3.13d, -1.73d, 2.21d));
            mol.Atoms.Add(Atom("C", 3, -0.70d, 0.69d, -1.73d));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Double);
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[5], BondOrder.Single);
            StereoElementFactory factory = StereoElementFactory.Using3DCoordinates(mol);
            var dbs = new List<IBond>
            {
                mol.Bonds[0],
                mol.Bonds[1],
                mol.Bonds[2]
            };
            ExtendedCisTrans element = factory.CreateExtendedCisTrans(dbs, Stereocenters.Of(mol));
            Assert.IsNotNull(element);
            Assert.AreEqual(StereoConfigurations.Together, element.Configure);
        }

        [TestMethod()]
        public void UnspecBut2eneByCoordinates()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 1, -1.37d, 1.64d));
            m.Atoms.Add(Atom("C", 1, -2.19d, 1.63d));
            m.Atoms.Add(Atom("C", 3, -2.59d, 0.90d));
            m.Atoms.Add(Atom("C", 3, -0.52d, 1.73d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Double);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[3], BondOrder.Single);

            StereoElementFactory factory = StereoElementFactory.Using2DCoordinates(m);
            IDoubleBondStereochemistry element = factory.CreateGeometric(m.Bonds[0], null);

            Assert.IsNull(element);
        }

        [TestMethod()]
        public void Unspec_but2ene_wavyBond()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 1, -1.27d, 1.48d));
            m.Atoms.Add(Atom("C", 1, -2.10d, 1.46d));
            m.Atoms.Add(Atom("C", 3, -2.50d, 0.74d));
            m.Atoms.Add(Atom("C", 3, -0.87d, 2.20d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Double);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[3], BondOrder.Single, BondStereo.UpOrDown);

            StereoElementFactory factory = StereoElementFactory.Using2DCoordinates(m);
            IDoubleBondStereochemistry element = factory.CreateGeometric(m.Bonds[0], null);

            Assert.IsNull(element);
        }

        [TestMethod()]
        public void Unspec_but2ene_crossBond()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 1, -1.27d, 1.48d));
            m.Atoms.Add(Atom("C", 1, -2.10d, 1.46d));
            m.Atoms.Add(Atom("C", 3, -2.50d, 0.74d));
            m.Atoms.Add(Atom("C", 3, -0.87d, 2.20d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Double, BondStereo.EOrZ);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[3], BondOrder.Single);

            StereoElementFactory factory = StereoElementFactory.Using2DCoordinates(m);
            IDoubleBondStereochemistry element = factory.CreateGeometric(m.Bonds[0], null);

            Assert.IsNull(element);
        }

        [TestMethod()]
        public void R_butan2ol()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("O", 1, -0.46d, 1.98d));
            m.Atoms.Add(Atom("C", 1, -1.28d, 1.96d));
            m.Atoms.Add(Atom("C", 2, -1.71d, 2.67d));
            m.Atoms.Add(Atom("C", 3, -1.68d, 1.24d));
            m.Atoms.Add(Atom("C", 3, -2.53d, 2.66d));
            m.AddBond(m.Atoms[1], m.Atoms[0], BondOrder.Single, BondStereo.Down);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[4], BondOrder.Single);
            StereoElementFactory factory = StereoElementFactory.Using2DCoordinates(m);
            ITetrahedralChirality element = factory.CreateTetrahedral(m.Atoms[1], null);
            Assert.IsNotNull(element);
            Assert.AreEqual(TetrahedralStereo.Clockwise, element.Stereo);
        }

        [TestMethod()]
        public void S_butan2ol()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("O", 1, -0.46d, 1.98d));
            m.Atoms.Add(Atom("C", 1, -1.28d, 1.96d));
            m.Atoms.Add(Atom("C", 2, -1.71d, 2.67d));
            m.Atoms.Add(Atom("C", 3, -1.68d, 1.24d));
            m.Atoms.Add(Atom("C", 3, -2.53d, 2.66d));
            m.AddBond(m.Atoms[1], m.Atoms[0], BondOrder.Single, BondStereo.Up);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[4], BondOrder.Single);
            StereoElementFactory factory = StereoElementFactory.Using2DCoordinates(m);
            ITetrahedralChirality element = factory.CreateTetrahedral(m.Atoms[1], null);
            Assert.IsNotNull(element);
            Assert.AreEqual(TetrahedralStereo.AntiClockwise, element.Stereo);
        }

        [TestMethod()]
        public void R_butan2ol_3d()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 1, 0.56d, 0.05d, 0.71d));
            m.Atoms.Add(Atom("C", 2, -0.53d, 0.51d, -0.30d));
            m.Atoms.Add(Atom("C", 3, 1.81d, -0.53d, 0.02d));
            m.Atoms.Add(Atom("C", 3, -1.80d, 1.06d, 0.37d));
            m.Atoms.Add(Atom("O", 1, 0.95d, 1.15d, 1.54d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[4], BondOrder.Single);
            StereoElementFactory factory = StereoElementFactory.Using3DCoordinates(m);
            ITetrahedralChirality element = factory.CreateTetrahedral(m.Atoms[0], Stereocenters.Of(m));
            Assert.IsNotNull(element);
            Assert.AreEqual(TetrahedralStereo.Clockwise, element.Stereo);
        }

        [TestMethod()]
        public void S_butan2ol_3d()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 1, -0.17d, -0.12d, -0.89d));
            m.Atoms.Add(Atom("C", 2, 1.12d, -0.91d, -0.51d));
            m.Atoms.Add(Atom("C", 3, -0.10d, 0.46d, -2.32d));
            m.Atoms.Add(Atom("C", 3, 1.07d, -1.54d, 0.91d));
            m.Atoms.Add(Atom("O", 1, -0.38d, 0.96d, 0.02d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[4], BondOrder.Single);
            StereoElementFactory factory = StereoElementFactory.Using3DCoordinates(m);
            ITetrahedralChirality element = factory.CreateTetrahedral(m.Atoms[0], Stereocenters.Of(m));
            Assert.IsNotNull(element);
            Assert.AreEqual(TetrahedralStereo.AntiClockwise, element.Stereo);
        }

        [TestMethod()]
        public void R_butan2ol_3d_expH()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 0, -0.07d, -0.14d, 0.50d));
            m.Atoms.Add(Atom("C", 2, -0.05d, -1.20d, -0.65d));
            m.Atoms.Add(Atom("C", 3, 0.98d, -0.46d, 1.60d));
            m.Atoms.Add(Atom("C", 3, -1.11d, -0.94d, -1.75d));
            m.Atoms.Add(Atom("O", 1, 0.21d, 1.16d, -0.01d));
            m.Atoms.Add(Atom("H", 0, -1.06d, -0.13d, 0.96d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[5], BondOrder.Single);
            StereoElementFactory factory = StereoElementFactory.Using3DCoordinates(m);
            ITetrahedralChirality element = factory.CreateTetrahedral(m.Atoms[0], Stereocenters.Of(m));
            Assert.IsNotNull(element);
            Assert.AreEqual(TetrahedralStereo.Clockwise, element.Stereo);
        }

        [TestMethod()]
        public void S_butan2ol_3d_expH()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 0, -0.17d, -0.12d, -0.89d));
            m.Atoms.Add(Atom("C", 2, 1.12d, -0.91d, -0.51d));
            m.Atoms.Add(Atom("C", 3, -0.10d, 0.46d, -2.32d));
            m.Atoms.Add(Atom("C", 3, 1.07d, -1.54d, 0.91d));
            m.Atoms.Add(Atom("O", 1, -0.38d, 0.96d, 0.02d));
            m.Atoms.Add(Atom("H", 0, -1.03d, -0.79d, -0.83d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[5], BondOrder.Single);
            StereoElementFactory factory = StereoElementFactory.Using3DCoordinates(m);
            ITetrahedralChirality element = factory.CreateTetrahedral(m.Atoms[0], Stereocenters.Of(m));
            Assert.IsNotNull(element);
            Assert.AreEqual(TetrahedralStereo.AntiClockwise, element.Stereo);
        }

        [TestMethod()]
        public void Unspec_butan2ol()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("O", 1, -0.46d, 1.98d));
            m.Atoms.Add(Atom("C", 1, -1.28d, 1.96d));
            m.Atoms.Add(Atom("C", 2, -1.71d, 2.67d));
            m.Atoms.Add(Atom("C", 3, -1.68d, 1.24d));
            m.Atoms.Add(Atom("C", 3, -2.53d, 2.66d));
            m.AddBond(m.Atoms[1], m.Atoms[0], BondOrder.Single, BondStereo.UpOrDown);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[4], BondOrder.Single);

            StereoElementFactory factory = StereoElementFactory.Using2DCoordinates(m);
            ITetrahedralChirality element = factory.CreateTetrahedral(m.Atoms[1], null);
            Assert.IsNull(element);
        }

        // @cdk.inchi InChI=1S/C3H8OS/c1-3-5(2)4/h3H2,1-2H3/t5-/m1/s1
        [TestMethod()]
        public void R_methanesulfinylethane()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("S", 0, 0.01d, 1.50d));
            m.Atoms.Add(Atom("C", 3, 0.03d, 0.00d));
            m.Atoms.Add(Atom("C", 2, -1.30d, 2.23d));
            m.Atoms.Add(Atom("C", 3, -1.33d, 3.73d));
            m.Atoms.Add(Atom("O", 0, 1.29d, 2.28d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single, BondStereo.Down);
            m.AddBond(m.Atoms[0], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[4], BondOrder.Double);
            StereoElementFactory factory = StereoElementFactory.Using2DCoordinates(m);
            ITetrahedralChirality element = factory.CreateTetrahedral(m.Atoms[0], null);
            Assert.IsNotNull(element);
            Assert.AreEqual(TetrahedralStereo.AntiClockwise, element.Stereo);
        }

        // @cdk.inchi InChI=1S/C3H8OS/c1-3-5(2)4/h3H2,1-2H3/t5-/m0/s1
        [TestMethod()]
        public void S_methanesulfinylethane()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("S", 0, 0.01d, 1.50d));
            m.Atoms.Add(Atom("C", 3, 0.03d, 0.00d));
            m.Atoms.Add(Atom("C", 2, -1.30d, 2.23d));
            m.Atoms.Add(Atom("C", 3, -1.33d, 3.73d));
            m.Atoms.Add(Atom("O", 0, 1.29d, 2.28d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single, BondStereo.Up);
            m.AddBond(m.Atoms[0], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[4], BondOrder.Double);
            StereoElementFactory factory = StereoElementFactory.Using2DCoordinates(m);
            ITetrahedralChirality element = factory.CreateTetrahedral(m.Atoms[0], null);
            Assert.IsNotNull(element);
            Assert.AreEqual(TetrahedralStereo.Clockwise, element.Stereo);
        }

        [TestMethod()]
        public void E_but2ene_3d()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 1, -0.19d, 0.09d, -0.27d));
            m.Atoms.Add(Atom("C", 1, 0.22d, -1.15d, 0.05d));
            m.Atoms.Add(Atom("C", 3, 0.21d, 0.75d, -1.49d));
            m.Atoms.Add(Atom("C", 3, -0.17d, -1.82d, 1.27d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Double);
            m.AddBond(m.Atoms[0], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[3], BondOrder.Single);

            StereoElementFactory factory = StereoElementFactory.Using3DCoordinates(m);
            IDoubleBondStereochemistry element = factory.CreateGeometric(m.Bonds[0], null);

            Assert.IsNotNull(element);
            Assert.AreEqual(DoubleBondConformation.Opposite, element.Stereo);
        }

        [TestMethod()]
        public void Z_but2ene_3d()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 1, 0.05d, -1.28d, 0.13d));
            m.Atoms.Add(Atom("C", 1, -0.72d, -0.58d, -0.72d));
            m.Atoms.Add(Atom("C", 3, 1.11d, -0.74d, 0.95d));
            m.Atoms.Add(Atom("C", 3, -0.65d, 0.85d, -0.94d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Double);
            m.AddBond(m.Atoms[0], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[3], BondOrder.Single);

            StereoElementFactory factory = StereoElementFactory.Using3DCoordinates(m);
            IDoubleBondStereochemistry element = factory.CreateGeometric(m.Bonds[0], null);

            Assert.IsNotNull(element);
            Assert.AreEqual(DoubleBondConformation.Together, element.Stereo);
        }

        [TestMethod()]
        public void Inverse_style_Downbond()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("O", 1, -0.46d, 1.98d));
            m.Atoms.Add(Atom("C", 1, -1.28d, 1.96d));
            m.Atoms.Add(Atom("C", 2, -1.71d, 2.67d));
            m.Atoms.Add(Atom("C", 3, -1.68d, 1.24d));
            m.Atoms.Add(Atom("C", 3, -2.53d, 2.66d));
            m.AddBond(m.Atoms[1], m.Atoms[0], BondOrder.Single, BondStereo.DownInverted);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[4], BondOrder.Single);
            StereoElementFactory factory = StereoElementFactory.Using2DCoordinates(m);
            ITetrahedralChirality element = factory.CreateTetrahedral(m.Atoms[1], Stereocenters.Of(m));
            Assert.IsNotNull(element);
            Assert.AreEqual(TetrahedralStereo.Clockwise, element.Stereo);
        }

        // this example mocks a case where the down bond is inverse but is shared
        // between two stereo-centres - we can't create an element for atom 1 as
        // this bond is used to specify atom '2'
        [TestMethod()]
        public void Inverse_style_Downbond_ambiguous()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("O", 1, -0.46d, 1.98d));
            m.Atoms.Add(Atom("C", 1, -1.28d, 1.96d));
            m.Atoms.Add(Atom("C", 1, -1.71d, 2.67d));
            m.Atoms.Add(Atom("C", 3, -1.68d, 1.24d));
            m.Atoms.Add(Atom("C", 3, -2.53d, 2.66d));
            m.Atoms.Add(Atom("O", 1, -1.31d, 3.39d));
            m.AddBond(m.Atoms[1], m.Atoms[0], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Single, BondStereo.DownInverted);
            m.AddBond(m.Atoms[1], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[5], BondOrder.Single);

            StereoElementFactory factory = StereoElementFactory.Using2DCoordinates(m);
            ITetrahedralChirality element = factory.CreateTetrahedral(m.Atoms[1], Stereocenters.Of(m));
            Assert.IsNull(element);
        }

        /// <summary>
        /// MetaCyc CPD-7272 D-dopachrome
        /// http://metacyc.org/META/NEW-IMAGE?type=NIL&object=CPD-7272
        /// </summary>
        // @cdk.inchi InChI=1S/C9H7NO4/c11-7-2-4-1-6(9(13)14)10-5(4)3-8(7)12/h1,3,6,10H,2H2,(H,13,14)/p-1
        [TestMethod()]
        public void Inverse_style_Downbond_dopachrome()
        {
            MDLV2000Reader mdl = null;
            try
            {
                mdl = new MDLV2000Reader(ResourceLoader.GetAsStream("NCDK.Data.MDL.CPD-7272.mol"));
                IAtomContainer ac = mdl.Read(new AtomContainer());

                // MDL reader currently adds stereo automatically
                var ses = ac.StereoElements.ToArray();

                Assert.AreEqual(1, ses.Length);
                Assert.IsNotNull(ses[0]);
            }
            finally
            {
                if (mdl != null) mdl.Close();
            }
        }

        [TestMethod()]
        public void CreateExtendedTetrahedralFrom2DCoordinates_cw()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 3, -1.56d, 0.78d));
            m.Atoms.Add(Atom("C", 0, -1.13d, 1.49d));
            m.Atoms.Add(Atom("C", 0, -0.31d, 1.47d));
            m.Atoms.Add(Atom("C", 0, 0.52d, 1.46d));
            m.Atoms.Add(Atom("C", 3, 0.94d, 2.17d));
            m.Atoms.Add(Atom("H", 0, 0.92d, 0.74d));
            m.Atoms.Add(Atom("H", 0, -1.53d, 2.21d));
            m.AddBond(m.Atoms[1], m.Atoms[0], BondOrder.Single, BondStereo.Up);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double, BondStereo.None);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Double, BondStereo.None);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[6], BondOrder.Single, BondStereo.Down);
            m.AddBond(m.Atoms[3], m.Atoms[5], BondOrder.Single);

            ExtendedTetrahedral et = StereoElementFactory.Using2DCoordinates(m).CreateExtendedTetrahedral(2,
                    Stereocenters.Of(m));
            Assert.AreEqual(TetrahedralStereo.Clockwise, et.Winding);
            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[] { m.Atoms[0], m.Atoms[6], m.Atoms[4], m.Atoms[5] }, et.Peripherals));
            Assert.AreEqual(m.Atoms[2], et.Focus);
        }

        [TestMethod()]
        public void CreateExtendedTetrahedralFrom2DCoordinates_ccw()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 3, -1.56d, 0.78d));
            m.Atoms.Add(Atom("C", 0, -1.13d, 1.49d));
            m.Atoms.Add(Atom("C", 0, -0.31d, 1.47d));
            m.Atoms.Add(Atom("C", 0, 0.52d, 1.46d));
            m.Atoms.Add(Atom("C", 3, 0.94d, 2.17d));
            m.Atoms.Add(Atom("H", 0, 0.92d, 0.74d));
            m.Atoms.Add(Atom("H", 0, -1.53d, 2.21d));
            m.AddBond(m.Atoms[1], m.Atoms[0], BondOrder.Single, BondStereo.Down);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double, BondStereo.None);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Double, BondStereo.None);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[6], BondOrder.Single, BondStereo.Up);
            m.AddBond(m.Atoms[3], m.Atoms[5], BondOrder.Single);

            ExtendedTetrahedral et = StereoElementFactory.Using2DCoordinates(m).CreateExtendedTetrahedral(2,
                    Stereocenters.Of(m));
            Assert.AreEqual(TetrahedralStereo.AntiClockwise, et.Winding);
            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[] { m.Atoms[0], m.Atoms[6], m.Atoms[4], m.Atoms[5] }, et.Peripherals));
            Assert.AreEqual(m.Atoms[2], et.Focus);
        }

        [TestMethod()]
        public void CreateExtendedTetrahedralFrom2DCoordinatesImplicitHydrogens_cw()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 3, -1.56d, 0.78d));
            m.Atoms.Add(Atom("C", 1, -1.13d, 1.49d));
            m.Atoms.Add(Atom("C", 0, -0.31d, 1.47d));
            m.Atoms.Add(Atom("C", 1, 0.52d, 1.46d));
            m.Atoms.Add(Atom("C", 3, 0.94d, 2.17d));
            m.AddBond(m.Atoms[1], m.Atoms[0], BondOrder.Single, BondStereo.Up);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double, BondStereo.None);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Double, BondStereo.None);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);

            ExtendedTetrahedral et = StereoElementFactory.Using2DCoordinates(m).CreateExtendedTetrahedral(2,
                    Stereocenters.Of(m));
            Assert.AreEqual(TetrahedralStereo.Clockwise, et.Winding);
            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[] { m.Atoms[0], m.Atoms[1], m.Atoms[4], m.Atoms[3] }, et.Peripherals));
            Assert.AreEqual(m.Atoms[2], et.Focus);
        }

        [TestMethod()]
        public void CreateExtendedTetrahedralFrom2DCoordinatesImplicitHydrogens_ccw()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 3, -1.56d, 0.78d));
            m.Atoms.Add(Atom("C", 1, -1.13d, 1.49d));
            m.Atoms.Add(Atom("C", 0, -0.31d, 1.47d));
            m.Atoms.Add(Atom("C", 1, 0.52d, 1.46d));
            m.Atoms.Add(Atom("C", 3, 0.94d, 2.17d));
            m.AddBond(m.Atoms[1], m.Atoms[0], BondOrder.Single, BondStereo.Down);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double, BondStereo.None);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Double, BondStereo.None);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);

            ExtendedTetrahedral et = StereoElementFactory.Using2DCoordinates(m).CreateExtendedTetrahedral(2,
                    Stereocenters.Of(m));
            Assert.AreEqual(TetrahedralStereo.AntiClockwise, et.Winding);
            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[] { m.Atoms[0], m.Atoms[1], m.Atoms[4], m.Atoms[3] }, et.Peripherals));
            Assert.AreEqual(m.Atoms[2], et.Focus);
        }

        [TestMethod()]
        public void CreateExtendedTetrahedralFrom2DCoordinatesNoNonplanarBonds()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 3, -1.56d, 0.78d));
            m.Atoms.Add(Atom("C", 0, -1.13d, 1.49d));
            m.Atoms.Add(Atom("C", 0, -0.31d, 1.47d));
            m.Atoms.Add(Atom("C", 0, 0.52d, 1.46d));
            m.Atoms.Add(Atom("C", 3, 0.94d, 2.17d));
            m.Atoms.Add(Atom("H", 0, 0.92d, 0.74d));
            m.Atoms.Add(Atom("H", 0, -1.53d, 2.21d));
            m.AddBond(m.Atoms[1], m.Atoms[0], BondOrder.Single, BondStereo.None);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double, BondStereo.None);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Double, BondStereo.None);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[6], BondOrder.Single, BondStereo.None);
            m.AddBond(m.Atoms[3], m.Atoms[5], BondOrder.Single);

            ExtendedTetrahedral et = StereoElementFactory.Using2DCoordinates(m).CreateExtendedTetrahedral(2,
                    Stereocenters.Of(m));
            Assert.IsNull(et);
        }

        [TestMethod()]
        public void CreateExtendedTetrahedralFrom3DCoordinates_cw()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 3, 0.1925, -2.7911, 1.8739));
            m.Atoms.Add(Atom("C", 0, -0.4383, -2.0366, 0.8166));
            m.Atoms.Add(Atom("C", 0, 0.2349, -1.2464, 0.0943));
            m.Atoms.Add(Atom("C", 0, 0.9377, -0.4327, -0.5715));
            m.Atoms.Add(Atom("C", 3, 1.0851, 0.9388, -0.1444));
            m.Atoms.Add(Atom("H", 0, 1.3810, -0.7495, -1.4012));
            m.Atoms.Add(Atom("H", 0, -1.4096, -2.1383, 0.6392));
            m.AddBond(m.Atoms[1], m.Atoms[0], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Double);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[5], BondOrder.Single);

            var stereos = StereoElementFactory.Using3DCoordinates(m).CreateAll().ToList();
            Assert.AreEqual(1, stereos.Count);
            Assert.IsInstanceOfType(stereos[0], typeof(ExtendedTetrahedral));
            ExtendedTetrahedral et = (ExtendedTetrahedral)stereos[0];
            Assert.AreEqual(TetrahedralStereo.Clockwise, et.Winding);
            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[] { m.Atoms[0], m.Atoms[6], m.Atoms[4], m.Atoms[5] }, et.Peripherals));
            Assert.AreEqual(m.Atoms[2], et.Focus);
        }

        [TestMethod()]
        public void CreateExtendedTetrahedralFrom3DCoordinates_ccw()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 3, -1.4096, -2.1383, 0.6392));
            m.Atoms.Add(Atom("C", 0, -0.4383, -2.0366, 0.8166));
            m.Atoms.Add(Atom("C", 0, 0.2349, -1.2464, 0.0943));
            m.Atoms.Add(Atom("C", 0, 0.9377, -0.4327, -0.5715));
            m.Atoms.Add(Atom("C", 3, 1.0851, 0.9388, -0.1444));
            m.Atoms.Add(Atom("H", 0, 1.3810, -0.7495, -1.4012));
            m.Atoms.Add(Atom("H", 0, 0.1925, -2.7911, 1.8739));
            m.AddBond(m.Atoms[1], m.Atoms[0], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Double);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[5], BondOrder.Single);

            var stereos = StereoElementFactory.Using3DCoordinates(m).CreateAll().ToList();
            Assert.AreEqual(1, stereos.Count);
            Assert.IsInstanceOfType(stereos[0], typeof(ExtendedTetrahedral));
            ExtendedTetrahedral et = (ExtendedTetrahedral)stereos[0];
            Assert.AreEqual(TetrahedralStereo.AntiClockwise, et.Winding);
            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[] { m.Atoms[0], m.Atoms[6], m.Atoms[4], m.Atoms[5] }, et.Peripherals));
            Assert.AreEqual(m.Atoms[2], et.Focus);
        }

        [TestMethod()]
        public void CreateExtendedTetrahedral()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 3, -1.56d, 0.78d));
            m.Atoms.Add(Atom("C", 1, -1.13d, 1.49d));
            m.Atoms.Add(Atom("C", 0, -0.31d, 1.47d));
            m.Atoms.Add(Atom("C", 1, 0.52d, 1.46d));
            m.Atoms.Add(Atom("C", 3, 0.94d, 2.17d));
            m.AddBond(m.Atoms[1], m.Atoms[0], BondOrder.Single, BondStereo.Up);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double, BondStereo.None);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Double, BondStereo.None);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);
            m.SetStereoElements(StereoElementFactory.Using2DCoordinates(m).CreateAll());
            Assert.IsTrue(m.StereoElements.GetEnumerator().MoveNext());
            Assert.IsInstanceOfType(m.StereoElements.First(), typeof(ExtendedTetrahedral));
        }

        [TestMethod()]
        public void DoNotCreateNonStereogenicExtendedTetrahedral()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 3, -1.56d, 0.78d));
            m.Atoms.Add(Atom("C", 1, -1.13d, 1.49d));
            m.Atoms.Add(Atom("C", 0, -0.31d, 1.47d));
            m.Atoms.Add(Atom("C", 0, 0.52d, 1.46d));
            m.Atoms.Add(Atom("C", 3, 0.94d, 2.17d));
            m.Atoms.Add(Atom("C", 3, 0.92d, 0.74d));
            m.AddBond(m.Atoms[1], m.Atoms[0], BondOrder.Single, BondStereo.Up);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double, BondStereo.None);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Double, BondStereo.None);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[5], BondOrder.Single);
            m.SetStereoElements(StereoElementFactory.Using2DCoordinates(m).CheckSymmetry(true).CreateAll());
            Assert.IsFalse(m.StereoElements.GetEnumerator().MoveNext());
        }

        /// <summary>
        /// The embedding of 3D depictions may cause bonds of abnormal length
        /// (e.g. CHEBI:7621). The parity computation should consider this, here
        /// we check we get the correct (anti-clockwise) configuration.
        /// </summary>
        [TestMethod()]
        public void DifferentBondLengthsDoNotAffectWinding()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("O", 1, 14.50d, -8.72d));
            m.Atoms.Add(Atom("N", 2, 14.50d, -11.15d));
            m.Atoms.Add(Atom("C", 0, 15.28d, -7.81d));
            m.Atoms.Add(Atom("C", 3, 12.91d, -7.81d));
            m.Atoms.Add(Atom("H", 0, 16.00d, -7.39d));
            m.AddBond(m.Atoms[2], m.Atoms[0], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[4], BondOrder.Single, BondStereo.Down);

            StereoElementFactory sef = StereoElementFactory.Using2DCoordinates(m);
            ITetrahedralChirality element = sef.CreateTetrahedral(2, Stereocenters.Of(m));

            Assert.AreEqual(m.Atoms[2], element.ChiralAtom);

            var ligands = element.Ligands;
            Assert.AreEqual(m.Atoms[0], ligands[0]);
            Assert.AreEqual(m.Atoms[1], ligands[1]);
            Assert.AreEqual(m.Atoms[3], ligands[2]);
            Assert.AreEqual(m.Atoms[4], ligands[3]);

            Assert.AreEqual(TetrahedralStereo.AntiClockwise, element.Stereo);
        }

        [TestMethod()]
        public void Always2DTetrahedralElements()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 1, 0.34d, 2.28d));
            m.Atoms.Add(Atom("O", 1, 1.17d, 2.28d));
            m.Atoms.Add(Atom("C", 1, -0.07d, 2.99d));
            m.Atoms.Add(Atom("C", 1, -0.07d, 1.56d));
            m.Atoms.Add(Atom("O", 1, 0.34d, 3.70d));
            m.Atoms.Add(Atom("O", 1, 0.34d, 0.85d));
            m.Atoms.Add(Atom("C", 3, -0.90d, 2.99d));
            m.Atoms.Add(Atom("C", 3, -0.90d, 1.56d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single, BondStereo.Up);
            m.AddBond(m.Atoms[0], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[4], BondOrder.Single, BondStereo.Down);
            m.AddBond(m.Atoms[3], m.Atoms[5], BondOrder.Single, BondStereo.Down);
            m.AddBond(m.Atoms[2], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[7], BondOrder.Single);

            var elements = StereoElementFactory.Using2DCoordinates(m).CreateAll();
            Assert.AreEqual(3, elements.Count());
        }

        /// <summary>
        /// Watch out for cumulated bonds with a kink in 3d. The generation program
        /// has not understood the chemistry completely.
        /// </summary>
        // @cdk.smiles CC=[C@]=CC 
        [TestMethod()]
        public void BadlyOptimizedAllene()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 1, -4.02, 3.96, -1.09));
            m.Atoms.Add(Atom("C", 0, -4.96, 3.82, 0.13));
            m.Atoms.Add(Atom("C", 3, -3.70, 5.35, -1.67));
            m.Atoms.Add(Atom("C", 1, -5.27, 2.44, 0.71));
            m.Atoms.Add(Atom("C", 3, -6.21, 2.30, 1.92));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Double);
            m.AddBond(m.Atoms[0], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[3], BondOrder.Double);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);
            var elements = StereoElementFactory.Using3DCoordinates(m).CreateAll().ToList();
            Assert.AreEqual(0, elements.Count);
        }

        [TestMethod()]
        public void OnlyCreateStereoForConsitionalDifferencesIn3D()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 1, -1.00d, -0.25d, 1.22d));
            m.Atoms.Add(Atom("O", 1, -1.82d, 0.20d, 2.30d));
            m.Atoms.Add(Atom("C", 1, -0.04d, -1.38d, 1.71d));
            m.Atoms.Add(Atom("C", 1, -0.24d, 0.95d, 0.57d));
            m.Atoms.Add(Atom("O", 1, 0.82d, -0.90d, 2.75d));
            m.Atoms.Add(Atom("O", 1, 0.63d, 1.58d, 1.51d));
            m.Atoms.Add(Atom("C", 3, -0.81d, -2.61d, 2.25d));
            m.Atoms.Add(Atom("C", 3, -1.19d, 2.03d, -0.01d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[7], BondOrder.Single);

            var elements = StereoElementFactory.Using3DCoordinates(m).CreateAll().ToList();
            // XXX: really 3 but we can't tell the middle centre is one ATM, see
            //      'dontCreateStereoForNonStereogenicIn3D'
            Assert.AreEqual(2, elements.Count);
        }

        [TestMethod()]
        public void DontCreateStereoForNonStereogenicIn3D()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 0, 0.00d, 0.00d, 0.00d));
            m.Atoms.Add(Atom("H", 0, -0.36d, -0.51d, 0.89d));
            m.Atoms.Add(Atom("H", 0, 1.09d, 0.00d, 0.00d));
            m.Atoms.Add(Atom("H", 0, -0.36d, 1.03d, 0.00d));
            m.Atoms.Add(Atom("H", 0, -0.36d, -0.51d, -0.89d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[4], BondOrder.Single);

            var elements = StereoElementFactory.Using3DCoordinates(m).CreateAll().ToList();

            // methane carbon is of course non-stereogenic
            Assert.AreEqual(0, elements.Count);
        }

        /// <summary>
        /// glyceraldehyde
        /// </summary>
        // @cdk.inchi InChI=1/C3H6O3/c4-1-3(6)2-5/h1,3,5-6H,2H2/t3-/s2
        [TestMethod()]
        public void OnlyInterpretFischerProjectionsWhenAsked()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 0, 0.80d, 1.24d));
            m.Atoms.Add(Atom("C", 0, 0.80d, 0.42d));
            m.Atoms.Add(Atom("O", 1, 0.09d, 1.66d));
            m.Atoms.Add(Atom("O", 0, 1.52d, 1.66d));
            m.Atoms.Add(Atom("O", 1, 1.63d, 0.42d));
            m.Atoms.Add(Atom("C", 2, 0.80d, -0.41d));
            m.Atoms.Add(Atom("H", 0, -0.02d, 0.42d));
            m.Atoms.Add(Atom("O", 1, 1.52d, -0.82d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[3], BondOrder.Double, BondStereo.EZByCoordinates);
            m.AddBond(m.Atoms[1], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[5], m.Atoms[7], BondOrder.Single);

            Assert.IsTrue(StereoElementFactory.Using2DCoordinates(m)
                                           .CreateAll()
                                           .Count() == 0);
            Assert.IsTrue(StereoElementFactory.Using2DCoordinates(m)
                                           .InterpretProjections(Projection.Haworth)
                                           .CreateAll()
                                           .Count() == 0);
            Assert.IsTrue(StereoElementFactory.Using2DCoordinates(m)
                                           .InterpretProjections(Projection.Chair)
                                           .CreateAll()
                                           .Count() == 0);
            Assert.IsFalse(StereoElementFactory.Using2DCoordinates(m)
                                            .InterpretProjections(Projection.Fischer)
                                            .CreateAll()
                                            .Count() == 0);
        }

        /// <summary>
        /// beta-D-glucose
        /// </summary>
        // @cdk.inchi InChI=1/C6H12O6/c7-1-2-3(8)4(9)5(10)6(11)12-2/h2-11H,1H2/t2-,3-,4+,5-,6-/s2
        [TestMethod()]
        public void OnlyInterpretHaworthProjectionsWhenAsked()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 1, 4.16d, 1.66d));
            m.Atoms.Add(Atom("C", 1, 3.75d, 0.94d));
            m.Atoms.Add(Atom("C", 1, 4.16d, 0.23d));
            m.Atoms.Add(Atom("C", 1, 5.05d, 0.23d));
            m.Atoms.Add(Atom("C", 1, 5.46d, 0.94d));
            m.Atoms.Add(Atom("O", 0, 5.05d, 1.66d));
            m.Atoms.Add(Atom("O", 1, 5.46d, 1.77d));
            m.Atoms.Add(Atom("C", 2, 4.16d, 2.48d));
            m.Atoms.Add(Atom("O", 1, 3.45d, 2.89d));
            m.Atoms.Add(Atom("O", 1, 3.75d, 0.12d));
            m.Atoms.Add(Atom("O", 1, 4.16d, 1.05d));
            m.Atoms.Add(Atom("O", 1, 5.05d, -0.60d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[7], BondOrder.Single);
            m.AddBond(m.Atoms[7], m.Atoms[8], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[9], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[10], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[11], BondOrder.Single);

            Assert.IsTrue(StereoElementFactory.Using2DCoordinates(m)
                                           .CreateAll()
                                           .Count() == 0);
            Assert.IsTrue(StereoElementFactory.Using2DCoordinates(m)
                                           .InterpretProjections(Projection.Fischer)
                                           .CreateAll()
                                           .Count() == 0);
            Assert.IsTrue(StereoElementFactory.Using2DCoordinates(m)
                                           .InterpretProjections(Projection.Chair)
                                           .CreateAll()
                                           .Count() == 0);
            Assert.IsFalse(StereoElementFactory.Using2DCoordinates(m)
                                            .InterpretProjections(Projection.Haworth)
                                            .CreateAll()
                                            .Count() == 0);
        }

        /// <summary>
        /// beta-D-glucose
        /// </summary>
        // @cdk.inchi InChI=1/C6H12O6/c7-1-2-3(8)4(9)5(10)6(11)12-2/h2-11H,1H2/t2-,3-,4+,5-,6-/s2
        [TestMethod()]
        public void OnlyInterpretChairProjectionsWhenAsked()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 1, -0.77d, 10.34d));
            m.Atoms.Add(Atom("C", 1, 0.03d, 10.13d));
            m.Atoms.Add(Atom("O", 0, 0.83d, 10.34d));
            m.Atoms.Add(Atom("C", 1, 1.24d, 9.63d));
            m.Atoms.Add(Atom("C", 1, 0.44d, 9.84d));
            m.Atoms.Add(Atom("C", 1, -0.35d, 9.63d));
            m.Atoms.Add(Atom("O", 1, 0.86d, 9.13d));
            m.Atoms.Add(Atom("O", 1, 2.04d, 9.84d));
            m.Atoms.Add(Atom("C", 2, -0.68d, 10.54d));
            m.Atoms.Add(Atom("O", 1, -0.68d, 11.37d));
            m.Atoms.Add(Atom("O", 1, -1.48d, 9.93d));
            m.Atoms.Add(Atom("O", 1, -1.15d, 9.84d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[5], m.Atoms[0], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[7], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[8], BondOrder.Single);
            m.AddBond(m.Atoms[8], m.Atoms[9], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[10], BondOrder.Single);
            m.AddBond(m.Atoms[5], m.Atoms[11], BondOrder.Single);

            Assert.IsTrue(StereoElementFactory.Using2DCoordinates(m)
                                           .CreateAll()
                                           .Count() == 0);
            Assert.IsTrue(StereoElementFactory.Using2DCoordinates(m)
                                           .InterpretProjections(Projection.Fischer)
                                           .CreateAll()
                                           .Count() == 0);
            Assert.IsTrue(StereoElementFactory.Using2DCoordinates(m)
                                           .InterpretProjections(Projection.Haworth)
                                           .CreateAll()
                                           .Count() == 0);
            Assert.IsFalse(StereoElementFactory.Using2DCoordinates(m)
                                            .InterpretProjections(Projection.Chair)
                                            .CreateAll()
                                            .Count() == 0);
        }

        /// <summary>
        /// Pass through non-stereo configurations if check symmetry is disabled
        /// </summary>
        [TestMethod()]
        public void KeepNonStereoConfiguration()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 0, 0.07, 1.19));
            m.Atoms.Add(Atom("H", 0, 0.56, 2.02));
            m.Atoms.Add(Atom("C", 3, -0.29, 2.04));
            m.Atoms.Add(Atom("C", 3, -0.66, 0.82));
            m.Atoms.Add(Atom("C", 2, 0.76, 0.74));
            m.Atoms.Add(Atom("C", 3, 1.50, 1.12));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single, BondStereo.Up);
            m.AddBond(m.Atoms[0], m.Atoms[2], BondOrder.Single, BondStereo.Down);
            m.AddBond(m.Atoms[0], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[5], BondOrder.Single);
            var elements = StereoElementFactory.Using2DCoordinates(m).CreateAll();
            Assert.AreEqual(1, elements.Count());
            m.SetStereoElements(elements);
            SmilesGenerator smigen = new SmilesGenerator(SmiFlavors.Stereo);
            Assert.AreEqual("[C@]([H])(C)(C)CC", smigen.Create(m));
        }

        [TestMethod()]
        public void KeepNonStereoConfigurationPhosphorusTautomer()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("P", 0, 0.07, 1.19));
            m.Atoms.Add(Atom("O", 0, 0.56, 2.02));
            m.Atoms.Add(Atom("O", 1, -0.29, 2.04));
            m.Atoms.Add(Atom("C", 3, -0.66, 0.82));
            m.Atoms.Add(Atom("C", 2, 0.76, 0.74));
            m.Atoms.Add(Atom("C", 3, 1.50, 1.12));
            m.AddBond(m.Atoms[0], m.Atoms[2], BondOrder.Single, BondStereo.Down);
            m.AddBond(m.Atoms[0], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Double);
            var elements = StereoElementFactory.Using2DCoordinates(m).CreateAll();
            Assert.AreEqual(1, elements.Count());
            m.SetStereoElements(elements);
            SmilesGenerator smigen = new SmilesGenerator(SmiFlavors.Stereo);
            Assert.AreEqual("[P@@](O)(C)(CC)=O", smigen.Create(m));
        }

        [TestMethod()]
        public void DoNotkeepNonStereoConfigurationPhosphorusTautomer()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("P", 0, 0.07, 1.19));
            m.Atoms.Add(Atom("O", 0, 0.56, 2.02));
            m.Atoms.Add(Atom("O", 1, -0.29, 2.04));
            m.Atoms.Add(Atom("C", 3, -0.66, 0.82));
            m.Atoms.Add(Atom("C", 2, 0.76, 0.74));
            m.Atoms.Add(Atom("C", 3, 1.50, 1.12));
            m.AddBond(m.Atoms[0], m.Atoms[2], BondOrder.Single, BondStereo.Down);
            m.AddBond(m.Atoms[0], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Double);
            var elements = StereoElementFactory.Using2DCoordinates(m).CheckSymmetry(true).CreateAll();
            Assert.AreEqual(0, elements.Count());
            m.SetStereoElements(elements);
            SmilesGenerator smigen = new SmilesGenerator(SmiFlavors.Stereo);
            Assert.AreEqual("P(O)(C)(CC)=O", smigen.Create(m));
        }

        /// <summary>
        /// Do not pass through non-stereo configurations if check symmetry is enabled
        /// </summary>
        [TestMethod()]
        public void DoNotKeepNonStereoConfiguration()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 0, 0.07, 1.19));
            m.Atoms.Add(Atom("H", 0, 0.56, 2.02));
            m.Atoms.Add(Atom("C", 3, -0.29, 2.04));
            m.Atoms.Add(Atom("C", 3, -0.66, 0.82));
            m.Atoms.Add(Atom("C", 2, 0.76, 0.74));
            m.Atoms.Add(Atom("C", 3, 1.50, 1.12));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single, BondStereo.Up);
            m.AddBond(m.Atoms[0], m.Atoms[2], BondOrder.Single, BondStereo.Down);
            m.AddBond(m.Atoms[0], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[5], BondOrder.Single);
            var elements = StereoElementFactory.Using2DCoordinates(m).CheckSymmetry(true).CreateAll();
            Assert.AreEqual(0, elements.Count());
            m.SetStereoElements(elements);
            SmilesGenerator smigen = new SmilesGenerator(SmiFlavors.Stereo);
            Assert.AreEqual("C([H])(C)(C)CC", smigen.Create(m));
        }

        /// <summary>
        /// Pass through non-stereo configurations if check symmetry is disabled
        /// </summary>
        [TestMethod()]
        public void KeepNonStereoConfigurationH2()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 0, 0.07, 1.19));
            m.Atoms.Add(Atom("H", 0, 0.56, 2.02));
            m.Atoms.Add(Atom("H", 0, -0.29, 2.04));
            m.Atoms.Add(Atom("C", 3, -0.66, 0.82));
            m.Atoms.Add(Atom("C", 2, 0.76, 0.74));
            m.Atoms.Add(Atom("C", 3, 1.50, 1.12));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single, BondStereo.Up);
            m.AddBond(m.Atoms[0], m.Atoms[2], BondOrder.Single, BondStereo.Down);
            m.AddBond(m.Atoms[0], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[5], BondOrder.Single);
            var elements = StereoElementFactory.Using2DCoordinates(m).CreateAll();
            Assert.AreEqual(1, elements.Count());
            m.SetStereoElements(elements);
            SmilesGenerator smigen = new SmilesGenerator(SmiFlavors.Stereo);
            Assert.AreEqual("[C@]([H])([H])(C)CC", smigen.Create(m));
            AtomContainerManipulator.SuppressHydrogens(m);
            Assert.AreEqual("[C@H2](C)CC", smigen.Create(m));
            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(m);
            Assert.AreEqual("[C@](C([H])([H])[H])(C(C([H])([H])[H])([H])[H])([H])[H]", smigen.Create(m));
        }

        /// <summary>
        /// BiNOL - SMILES/InChI can't represent the atropoisomerism but the single
        ///         bond rotation is restricted.
        /// </summary>
        // @cdk.smiles OC1=CC=C2C=CC=CC2=C1C1=C(O)C=CC2=C1C=CC=C2 
        [TestMethod()]
        public void Binol2D()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 0, -0.83, -0.01));
            m.Atoms.Add(Atom("C", 0, -1.55, -0.42));
            m.Atoms.Add(Atom("C", 1, -1.55, -1.25));
            m.Atoms.Add(Atom("C", 1, -0.83, -1.66));
            m.Atoms.Add(Atom("C", 0, -0.12, -1.25));
            m.Atoms.Add(Atom("C", 0, -0.12, -0.42));
            m.Atoms.Add(Atom("C", 0, -0.83, 0.82));
            m.Atoms.Add(Atom("C", 1, -0.83, 2.47));
            m.Atoms.Add(Atom("C", 1, -1.55, 2.05));
            m.Atoms.Add(Atom("C", 0, -1.55, 1.23));
            m.Atoms.Add(Atom("C", 0, -0.12, 1.23));
            m.Atoms.Add(Atom("C", 0, -0.12, 2.05));
            m.Atoms.Add(Atom("O", 1, -2.26, 0.82));
            m.Atoms.Add(Atom("O", 1, -2.26, -0.01));
            m.Atoms.Add(Atom("C", 1, 0.60, 2.47));
            m.Atoms.Add(Atom("C", 1, 0.60, 0.82));
            m.Atoms.Add(Atom("C", 1, 1.31, 1.23));
            m.Atoms.Add(Atom("C", 1, 1.31, 2.05));
            m.Atoms.Add(Atom("C", 1, 0.60, -0.01));
            m.Atoms.Add(Atom("C", 1, 0.60, -1.66));
            m.Atoms.Add(Atom("C", 1, 1.31, -1.25));
            m.Atoms.Add(Atom("C", 1, 1.31, -0.42));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Double);
            m.AddBond(m.Atoms[4], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[5], BondOrder.Double);
            m.AddBond(m.Atoms[0], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[7], m.Atoms[8], BondOrder.Double);
            m.AddBond(m.Atoms[8], m.Atoms[9], BondOrder.Single);
            m.AddBond(m.Atoms[10], m.Atoms[11], BondOrder.Double);
            m.AddBond(m.Atoms[7], m.Atoms[11], BondOrder.Single);
            m.AddBond(m.Atoms[9], m.Atoms[6], BondOrder.Double);
            m.AddBond(m.Atoms[6], m.Atoms[10], BondOrder.Single);
            m.AddBond(m.Atoms[9], m.Atoms[12], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[13], BondOrder.Single);
            m.AddBond(m.Atoms[15], m.Atoms[16], BondOrder.Double);
            m.AddBond(m.Atoms[16], m.Atoms[17], BondOrder.Single);
            m.AddBond(m.Atoms[14], m.Atoms[17], BondOrder.Double);
            m.AddBond(m.Atoms[10], m.Atoms[15], BondOrder.Single);
            m.AddBond(m.Atoms[14], m.Atoms[11], BondOrder.Single);
            m.AddBond(m.Atoms[19], m.Atoms[20], BondOrder.Double);
            m.AddBond(m.Atoms[20], m.Atoms[21], BondOrder.Single);
            m.AddBond(m.Atoms[18], m.Atoms[21], BondOrder.Double);
            m.AddBond(m.Atoms[4], m.Atoms[19], BondOrder.Single);
            m.AddBond(m.Atoms[18], m.Atoms[5], BondOrder.Single);
            var stereo = StereoElementFactory.Using2DCoordinates(m).CreateAll().ToList();
            Assert.AreEqual(0, stereo.Count);
            m.Bonds[12].Stereo = BondStereo.Up;
            var stereoUp = StereoElementFactory.Using2DCoordinates(m).CreateAll().ToList();
            Assert.AreEqual(1, stereoUp.Count);
            m.Bonds[12].Stereo = BondStereo.Down;
            var stereoDown = StereoElementFactory.Using2DCoordinates(m).CreateAll().ToList();
            Assert.AreEqual(1, stereoDown.Count);
            var s1 = stereoUp[0];
            var s2 = stereoDown[0];
            Assert.AreEqual(s2.Focus, s1.Focus);
            Assert.IsTrue(Compares.AreDeepEqual(s2.Carriers, s1.Carriers));
            Assert.AreEqual(StereoConfigurations.Right, s1.Configure);
            Assert.AreEqual(StereoConfigurations.Left, s2.Configure);

            // now test placement of wedges else where
            m.Bonds[12].Stereo = BondStereo.None;
            m.GetBond(m.Atoms[9], m.Atoms[12]).Stereo = BondStereo.Up;
            var stereoUpOther = StereoElementFactory.Using2DCoordinates(m).CreateAll().ToList();
            Assert.AreEqual(1, stereoUpOther.Count);
            var s3 = stereoUpOther[0];
            Assert.AreEqual(s2.Focus, s3.Focus);
            Assert.IsTrue(Compares.AreDeepEqual(s2.Carriers, s3.Carriers));
            Assert.AreEqual(s2.Configure, s3.Configure);

            m.GetBond(m.Atoms[9], m.Atoms[12]).Stereo = BondStereo.Down;
            var stereoDownOther = StereoElementFactory.Using2DCoordinates(m).CreateAll().ToList();
            Assert.AreEqual(1, stereoDownOther.Count);
            var s4 = stereoDownOther[0];
            Assert.AreEqual(s1.Focus, s4.Focus);
            Assert.IsTrue(Compares.AreDeepEqual(s1.Carriers, s4.Carriers));
            Assert.AreEqual(s1.Configure, s4.Configure);
        }

        // @cdk.smiles CC1=C(C=CC=C1)C1=C(C)C=CC=C1O 
        [TestMethod()]
        public void Atropisomer1()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 0, -7.53, -2.12));
            m.Atoms.Add(Atom("C", 1, -8.24, -2.53));
            m.Atoms.Add(Atom("C", 1, -8.24, -3.36));
            m.Atoms.Add(Atom("C", 1, -7.53, -3.77));
            m.Atoms.Add(Atom("C", 1, -6.82, -3.36));
            m.Atoms.Add(Atom("C", 0, -6.82, -2.53));
            m.Atoms.Add(Atom("C", 0, -7.53, -1.30));
            m.Atoms.Add(Atom("C", 0, -6.82, -0.88));
            m.Atoms.Add(Atom("C", 1, -6.82, -0.06));
            m.Atoms.Add(Atom("C", 1, -7.53, 0.35));
            m.Atoms.Add(Atom("C", 1, -8.24, -0.06));
            m.Atoms.Add(Atom("C", 0, -8.24, -0.88));
            m.Atoms.Add(Atom("C", 3, -8.96, -1.30));
            m.Atoms.Add(Atom("O", 1, -6.10, -1.30));
            m.Atoms.Add(Atom("C", 3, -6.10, -2.12));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single, BondStereo.Up);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Double);
            m.AddBond(m.Atoms[4], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[5], BondOrder.Double);
            m.AddBond(m.Atoms[7], m.Atoms[8], BondOrder.Double);
            m.AddBond(m.Atoms[8], m.Atoms[9], BondOrder.Single);
            m.AddBond(m.Atoms[9], m.Atoms[10], BondOrder.Double);
            m.AddBond(m.Atoms[10], m.Atoms[11], BondOrder.Single);
            m.AddBond(m.Atoms[6], m.Atoms[7], BondOrder.Single);
            m.AddBond(m.Atoms[6], m.Atoms[11], BondOrder.Double);
            m.AddBond(m.Atoms[0], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[11], m.Atoms[12], BondOrder.Single);
            m.AddBond(m.Atoms[7], m.Atoms[13], BondOrder.Single);
            m.AddBond(m.Atoms[5], m.Atoms[14], BondOrder.Single);
            var stereo = StereoElementFactory.Using2DCoordinates(m).CreateAll();
            Assert.AreEqual(1, stereo.Count());
        }

        // @cdk.smiles CC1=C(C(O)=CC=C1)C1=CC=CC=C1 
        [TestMethod()]
        public void NonAtropisomer2()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 0, -7.53, -2.12));
            m.Atoms.Add(Atom("C", 1, -8.24, -2.53));
            m.Atoms.Add(Atom("C", 1, -8.24, -3.36));
            m.Atoms.Add(Atom("C", 1, -7.53, -3.77));
            m.Atoms.Add(Atom("C", 1, -6.82, -3.36));
            m.Atoms.Add(Atom("C", 1, -6.82, -2.53));
            m.Atoms.Add(Atom("C", 0, -7.53, -1.30));
            m.Atoms.Add(Atom("C", 0, -6.82, -0.88));
            m.Atoms.Add(Atom("C", 1, -6.82, -0.06));
            m.Atoms.Add(Atom("C", 1, -7.53, 0.35));
            m.Atoms.Add(Atom("C", 1, -8.24, -0.06));
            m.Atoms.Add(Atom("C", 0, -8.24, -0.88));
            m.Atoms.Add(Atom("C", 3, -8.96, -1.30));
            m.Atoms.Add(Atom("O", 1, -6.10, -1.30));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single, BondStereo.Up);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Double);
            m.AddBond(m.Atoms[4], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[5], BondOrder.Double);
            m.AddBond(m.Atoms[7], m.Atoms[8], BondOrder.Double);
            m.AddBond(m.Atoms[8], m.Atoms[9], BondOrder.Single);
            m.AddBond(m.Atoms[9], m.Atoms[10], BondOrder.Double);
            m.AddBond(m.Atoms[10], m.Atoms[11], BondOrder.Single);
            m.AddBond(m.Atoms[6], m.Atoms[7], BondOrder.Single);
            m.AddBond(m.Atoms[6], m.Atoms[11], BondOrder.Double);
            m.AddBond(m.Atoms[0], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[11], m.Atoms[12], BondOrder.Single);
            m.AddBond(m.Atoms[7], m.Atoms[13], BondOrder.Single);
            var stereo = StereoElementFactory.Using2DCoordinates(m).CreateAll();
            Assert.AreEqual(0, stereo.Count());
        }

        // @cdk.smiles CC1=C(C=CC=C1)C1=C(C)C=CC=C1 
        [TestMethod()]
        public void NonAtropisomer3()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 0, -7.53, -2.12));
            m.Atoms.Add(Atom("C", 1, -8.24, -2.53));
            m.Atoms.Add(Atom("C", 1, -8.24, -3.36));
            m.Atoms.Add(Atom("C", 1, -7.53, -3.77));
            m.Atoms.Add(Atom("C", 1, -6.82, -3.36));
            m.Atoms.Add(Atom("C", 0, -6.82, -2.53));
            m.Atoms.Add(Atom("C", 0, -7.53, -1.30));
            m.Atoms.Add(Atom("C", 1, -6.82, -0.88));
            m.Atoms.Add(Atom("C", 1, -6.82, -0.06));
            m.Atoms.Add(Atom("C", 1, -7.53, 0.35));
            m.Atoms.Add(Atom("C", 1, -8.24, -0.06));
            m.Atoms.Add(Atom("C", 0, -8.24, -0.88));
            m.Atoms.Add(Atom("C", 3, -8.96, -1.30));
            m.Atoms.Add(Atom("C", 3, -6.10, -2.12));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single, BondStereo.Up);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Double);
            m.AddBond(m.Atoms[4], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[5], BondOrder.Double);
            m.AddBond(m.Atoms[7], m.Atoms[8], BondOrder.Double);
            m.AddBond(m.Atoms[8], m.Atoms[9], BondOrder.Single);
            m.AddBond(m.Atoms[9], m.Atoms[10], BondOrder.Double);
            m.AddBond(m.Atoms[10], m.Atoms[11], BondOrder.Single);
            m.AddBond(m.Atoms[6], m.Atoms[7], BondOrder.Single);
            m.AddBond(m.Atoms[6], m.Atoms[11], BondOrder.Double);
            m.AddBond(m.Atoms[0], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[11], m.Atoms[12], BondOrder.Single);
            m.AddBond(m.Atoms[5], m.Atoms[13], BondOrder.Single);
            var stereo = StereoElementFactory.Using2DCoordinates(m).CreateAll();
            Assert.AreEqual(0, stereo.Count());
        }

        // @cdk.smiles [H]C1=CC=C2C=CC=CC2=C1C1=C([H])C=CC2=C1C=CC=C2 
        [TestMethod()]
        public void NonAtropisomerExplHydrogens()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("H", 0, -1.43, 0.83));
            m.Atoms.Add(Atom("C", 0, -0.71, 1.24));
            m.Atoms.Add(Atom("C", 1, -0.71, 2.06));
            m.Atoms.Add(Atom("C", 1, 0.00, 2.48));
            m.Atoms.Add(Atom("C", 0, 0.71, 2.06));
            m.Atoms.Add(Atom("C", 1, 1.43, 2.48));
            m.Atoms.Add(Atom("C", 1, 2.14, 2.06));
            m.Atoms.Add(Atom("C", 1, 2.14, 1.24));
            m.Atoms.Add(Atom("C", 1, 1.43, 0.83));
            m.Atoms.Add(Atom("C", 0, 0.71, 1.24));
            m.Atoms.Add(Atom("C", 0, 0.00, 0.83));
            m.Atoms.Add(Atom("C", 0, 0.00, 0.00));
            m.Atoms.Add(Atom("C", 0, -0.71, -0.41));
            m.Atoms.Add(Atom("H", 0, -1.43, 0.00));
            m.Atoms.Add(Atom("C", 1, -0.71, -1.24));
            m.Atoms.Add(Atom("C", 1, 0.00, -1.65));
            m.Atoms.Add(Atom("C", 0, 0.71, -1.24));
            m.Atoms.Add(Atom("C", 0, 0.71, -0.41));
            m.Atoms.Add(Atom("C", 1, 1.43, 0.00));
            m.Atoms.Add(Atom("C", 1, 2.14, -0.41));
            m.Atoms.Add(Atom("C", 1, 2.14, -1.24));
            m.Atoms.Add(Atom("C", 1, 1.43, -1.65));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Double);
            m.AddBond(m.Atoms[4], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[5], m.Atoms[6], BondOrder.Double);
            m.AddBond(m.Atoms[6], m.Atoms[7], BondOrder.Single);
            m.AddBond(m.Atoms[7], m.Atoms[8], BondOrder.Double);
            m.AddBond(m.Atoms[8], m.Atoms[9], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[9], BondOrder.Single);
            m.AddBond(m.Atoms[9], m.Atoms[10], BondOrder.Double);
            m.AddBond(m.Atoms[1], m.Atoms[10], BondOrder.Single);
            m.AddBond(m.Atoms[10], m.Atoms[11], BondOrder.Single);
            m.AddBond(m.Atoms[11], m.Atoms[12], BondOrder.Double);
            m.AddBond(m.Atoms[12], m.Atoms[14], BondOrder.Single);
            m.AddBond(m.Atoms[14], m.Atoms[15], BondOrder.Double);
            m.AddBond(m.Atoms[15], m.Atoms[16], BondOrder.Single);
            m.AddBond(m.Atoms[16], m.Atoms[17], BondOrder.Double);
            m.AddBond(m.Atoms[11], m.Atoms[17], BondOrder.Single, BondStereo.Up);
            m.AddBond(m.Atoms[17], m.Atoms[18], BondOrder.Single);
            m.AddBond(m.Atoms[18], m.Atoms[19], BondOrder.Double);
            m.AddBond(m.Atoms[19], m.Atoms[20], BondOrder.Single);
            m.AddBond(m.Atoms[20], m.Atoms[21], BondOrder.Double);
            m.AddBond(m.Atoms[16], m.Atoms[21], BondOrder.Single);
            m.AddBond(m.Atoms[12], m.Atoms[13], BondOrder.Single);
            var stereo = StereoElementFactory.Using2DCoordinates(m).CreateAll();
            Assert.AreEqual(0, stereo.Count());
        }

        // @cdk.smiles CC1=CC=CC(Cl)=C1C1=C(C)C=CC=C1 
        [TestMethod()]
        public void Atropisomer3D()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 1, -4.95, 1.27));
            m.Atoms.Add(Atom("C", 1, -4.26, 1.73));
            m.Atoms.Add(Atom("C", 0, -2.85, 1.74));
            m.Atoms.Add(Atom("C", 0, -2.12, 1.25));
            m.Atoms.Add(Atom("C", 0, -2.83, 0.80));
            m.Atoms.Add(Atom("C", 1, -4.23, 0.82));
            m.Atoms.Add(Atom("C", 3, -2.16, 2.26));
            m.Atoms.Add(Atom("Cl", 0, -2.04, 0.24));
            m.Atoms.Add(Atom("C", 0, -0.70, 1.20));
            m.Atoms.Add(Atom("C", 1, 0.00, 2.39));
            m.Atoms.Add(Atom("C", 1, 1.41, 2.39));
            m.Atoms.Add(Atom("C", 1, 2.12, 1.22));
            m.Atoms.Add(Atom("C", 1, 1.44, 0.04));
            m.Atoms.Add(Atom("C", 0, 0.02, 0.01));
            m.Atoms.Add(Atom("C", 3, -0.62, -1.29));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double);
            m.AddBond(m.Atoms[3], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Double);
            m.AddBond(m.Atoms[4], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[5], BondOrder.Double);
            m.AddBond(m.Atoms[2], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[7], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[8], BondOrder.Single);
            m.AddBond(m.Atoms[9], m.Atoms[10], BondOrder.Double);
            m.AddBond(m.Atoms[10], m.Atoms[11], BondOrder.Single);
            m.AddBond(m.Atoms[11], m.Atoms[12], BondOrder.Double);
            m.AddBond(m.Atoms[12], m.Atoms[13], BondOrder.Single);
            m.AddBond(m.Atoms[8], m.Atoms[9], BondOrder.Single);
            m.AddBond(m.Atoms[8], m.Atoms[13], BondOrder.Double);
            m.AddBond(m.Atoms[13], m.Atoms[14], BondOrder.Single);
            var stereo = StereoElementFactory.Using3DCoordinates(m).CreateAll();
            Assert.AreEqual(1, stereo.Count());
        }

        static IAtom Atom(string symbol, int h, double x, double y)
        {
            IAtom a = new Atom(symbol)
            {
                ImplicitHydrogenCount = h,
                Point2D = new Vector2(x, y)
            };
            return a;
        }

        static IAtom Atom(string symbol, int h, double x, double y, double z)
        {
            IAtom a = new Atom(symbol)
            {
                ImplicitHydrogenCount = h,
                Point3D = new Vector3(x, y, z)
            };
            return a;
        }
    }
}
