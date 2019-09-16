/*
 * Copyright (C) 2019  The Chemistry Development Kit (CDK) project
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
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.IO;
using NCDK.Tools.Manipulator;
using System.Linq;

namespace NCDK.Geometries.Surface
{
    [TestClass()]
    public class NumericalSurfaceTest {

        [TestMethod()]
        public void TestCranbinSurface()
        {
            var bldr = CDK.Builder;
            IChemFile chemFile;
            var path = "NCDK.Data.PDB.1CRN.pdb";
            using (var pdbr = new PDBReader(ResourceLoader.GetAsStream(path)))
            {
                chemFile = pdbr.Read(bldr.NewChemFile());
                var mol = ChemFileManipulator.GetAllAtomContainers(chemFile).ElementAt(0);
                var surface = new NumericalSurface(mol);
                var map = surface.GetAtomSurfaceMap();
                Assert.AreEqual(222, map.Count);
                Assert.AreEqual(327, mol.Atoms.Count);
            }
        }
    }
}
