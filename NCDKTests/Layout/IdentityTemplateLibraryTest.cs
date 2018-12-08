/*
 * Copyright (c) 2014 European Bioinformatics Institute (EMBL-EBI)
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
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Common.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Silent;
using System.Collections.Generic;
using System.IO;
using NCDK.Numerics;
using System.Text;

namespace NCDK.Layout
{
    [TestClass()]
    public class IdentityTemplateLibraryTest
    {
        [TestMethod()]
        public void DecodeCoordinates()
        {
            Vector2[] points = IdentityTemplateLibrary.DecodeCoordinates("12.5, 5.5, 4, 2");
            Assert.AreEqual(2, points.Length);
            Assert.AreEqual(12.5, points[0].X, 0.01);
            Assert.AreEqual(5.5, points[0].Y, 0.01);
            Assert.AreEqual(4, points[1].X, 0.01);
            Assert.AreEqual(2, points[1].Y, 0.01);
        }

        [TestMethod()]
        public void EncodeCoordinates()
        {
            Vector2[] points = new Vector2[] { new Vector2(12.5, 5.5), new Vector2(4, 2) };
            string str = IdentityTemplateLibrary.EncodeCoordinates(points);
            Assert.AreEqual("|(12.5,5.5,;4,2,)|", str);
        }

        [TestMethod()]
        public void EncodeEntry()
        {
            var smiles = "CO";
            Vector2[] points = new Vector2[] { new Vector2(12.5f, 5.5f), new Vector2(4f, 2f) };
            string encoded = IdentityTemplateLibrary.EncodeEntry(new KeyValuePair<string, Vector2[]>(smiles, points));
            var entry = IdentityTemplateLibrary.DecodeEntry(encoded);
            Assert.AreEqual("CO |(12.5,5.5,;4,2,)|", encoded);
        }

        [TestMethod()]
        public void DecodeEntry()
        {
            string encode = "CO 12.500, 5.500, 4.000, 2.000";
            var entry = IdentityTemplateLibrary.DecodeEntry(encode);
            Assert.AreEqual("CO", entry.Key);
            Assert.IsTrue(Compares.AreDeepEqual(new Vector2[] { new Vector2(12.5, 5.5), new Vector2(4, 2) }, entry.Value));
        }

        [TestMethod()]
        public void AssignEthanolNoEntry()
        {
            IAtomContainer container = new AtomContainer();
            container.Atoms.Add(new Atom("O"));
            container.Atoms.Add(new Atom("C"));
            container.Atoms.Add(new Atom("C"));
            container.Atoms[0].ImplicitHydrogenCount = 0;
            container.Atoms[1].ImplicitHydrogenCount = 0;
            container.Atoms[2].ImplicitHydrogenCount = 0;
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Single);
            container.AddBond(container.Atoms[1], container.Atoms[2], BondOrder.Single);

            Assert.IsFalse(IdentityTemplateLibrary.Empty().AssignLayout(container));
        }

        [TestMethod()]
        public void AssignEthanol()
        {
            IAtomContainer container = new AtomContainer();
            container.Atoms.Add(new Atom("O"));
            container.Atoms.Add(new Atom("C"));
            container.Atoms.Add(new Atom("C"));
            container.Atoms[0].ImplicitHydrogenCount = 0;
            container.Atoms[1].ImplicitHydrogenCount = 0;
            container.Atoms[2].ImplicitHydrogenCount = 0;
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Single);
            container.AddBond(container.Atoms[1], container.Atoms[2], BondOrder.Single);

            IdentityTemplateLibrary lib = IdentityTemplateLibrary.Empty();
            lib.Add(IdentityTemplateLibrary.DecodeEntry("OCC 4, 5, 2, 3, 0, 1"));
            Assert.IsTrue(lib.AssignLayout(container));
            Assert.AreEqual(4, container.Atoms[0].Point2D.Value.X, 0.01);
            Assert.AreEqual(5, container.Atoms[0].Point2D.Value.Y, 0.01);
            Assert.AreEqual(2, container.Atoms[1].Point2D.Value.X, 0.01);
            Assert.AreEqual(3, container.Atoms[1].Point2D.Value.Y, 0.01);
            Assert.AreEqual(0, container.Atoms[2].Point2D.Value.X, 0.01);
            Assert.AreEqual(1, container.Atoms[2].Point2D.Value.Y, 0.01);
        }

        [TestMethod()]
        public void Store()
        {
            IdentityTemplateLibrary lib = IdentityTemplateLibrary.Empty();
            lib.Add(IdentityTemplateLibrary.DecodeEntry("[C][C][O] 0, 1, 2, 3, 4, 5"));
            lib.Add(IdentityTemplateLibrary.DecodeEntry("[C][C] 0, 1, 2, 3"));
            using (var baos = new MemoryStream())
            {
                lib.Store(baos);
                Assert.AreEqual(
                    "[C][C][O] |(0,1,;2,3,;4,5,)|\n[C][C] |(0,1,;2,3,)|\n",
                    Encoding.UTF8.GetString(baos.ToArray()));
            }
        }
    }
}
