/* Copyright (C) 2004-2007  Miguel Rojas <miguel.rojas@uni-koeln.de>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 */
using NCDK.Common.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Numerics;

namespace NCDK.Default
{
    /// <summary>
    /// Checks the functionality of the <see cref="PDBAtom"/> class.
    /// </summary>
    // @cdk.module test-data
    [TestClass()]
    public class PDBAtomTest : AbstractPDBAtomTest
    {
        public override IChemObject NewChemObject()
        {
            return new PDBAtom("C");
        }

        [TestMethod()]
        public void TestPDBAtom_IElement()
        {
            IElement element = NewChemObject().Builder.CreateElement();
            IAtom a = new PDBAtom(element);
            Assert.IsNotNull(a);
        }

        /// <summary>
        /// Method to test the Atom(string symbol) method.
        /// </summary>
        [TestMethod()]
        public void TestPDBAtom_String()
        {
            IPDBAtom a = new PDBAtom("C");
            Assert.AreEqual("C", a.Symbol);
            Assert.IsNull(a.Point2D);
            Assert.IsNull(a.Point3D);
            Assert.IsNull(a.FractionalPoint3D);
        }

        /// <summary>
        /// Method to test the Atom(string symbol, javax.vecmath.Vector3 point3D) method.
        /// </summary>
        [TestMethod()]
        public void TestPDBAtom_String_Point3d()
        {
            Vector3 point3d = new Vector3(1, 2, 3);

            IPDBAtom a = new PDBAtom("C", point3d);
            Assert.AreEqual("C", a.Symbol);
            Assert.AreEqual(point3d, a.Point3D);
            Assert.IsNull(a.Point2D);
            Assert.IsNull(a.FractionalPoint3D);
        }

        /// <summary>
        /// Method to test the Compare() method.
        /// </summary>
        [TestMethod()]
        public override void TestCompare_Object()
        {
            IPDBAtom someAtom = new PDBAtom("C");
            if (someAtom is Atom)
            {
                Atom atom = (Atom)someAtom;
                Assert.IsTrue(atom.Compare(atom));
                IAtom hydrogen = atom.Builder.CreateAtom("H");
                Assert.IsFalse(atom.Compare(hydrogen));
                Assert.IsFalse(atom.Compare("C"));
            }
        }
    }
}
