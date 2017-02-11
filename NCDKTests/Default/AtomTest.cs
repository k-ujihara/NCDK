/* Copyright (C) 2002-2007  The Chemistry Development Kit (CDK) project
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
using System;
using System.Collections.Generic;
using System.Linq;
using NCDK.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NCDK.Default
{
    /**
     * Checks the functionality of the AtomTypeFactory
     *
     * @cdk.module test-data
     */
	[TestClass()]
    public class AtomTest
            : AbstractAtomTest
    {
        public override IChemObject NewChemObject()
        {
            return new Atom();
        }

        /**
         * Method to test the Atom(string symbol) method.
         */
        [TestMethod()]
        public virtual void TestAtom()
        {
            IAtom a = new Atom();
            Assert.IsNotNull(a);
        }

        [TestMethod()]
        public virtual void TestAtom_IElement()
        {
            IElement element = NewChemObject().Builder.CreateElement();
            IAtom a = new Atom(element);
            Assert.IsNotNull(a);
        }

        /**
         * Method to test the Atom(string symbol) method.
         */
        [TestMethod()]
        public virtual void TestAtom_String()
        {
            IAtom a = new Atom("C");
            Assert.AreEqual("C", a.Symbol);
            Assert.IsNull(a.Point2D);
            Assert.IsNull(a.Point3D);
            Assert.IsNull(a.FractionalPoint3D);
        }

        /**
         * Method to test the Atom(string symbol, javax.vecmath.Vector3 point3D) method.
         */
        [TestMethod()]
        public virtual void TestAtom_String_Point3d()
        {
            Vector3 point3d = new Vector3(1, 2, 3);

            IAtom a = new Atom("C", point3d);
            Assert.AreEqual("C", a.Symbol);
            Assert.AreEqual(point3d, a.Point3D);
            Assert.IsNull(a.Point2D);
            Assert.IsNull(a.FractionalPoint3D);
        }

        /**
         * Method to test the Atom(string symbol, javax.vecmath.Vector3 point3D) method.
         */
        [TestMethod()]
        public virtual void TestAtom_String_Point2d()
        {
            Vector2 point2d = new Vector2(1, 2);

            IAtom a = new Atom("C", point2d);
            Assert.AreEqual("C", a.Symbol);
            Assert.AreEqual(point2d, a.Point2D);
            Assert.IsNull(a.Point3D);
            Assert.IsNull(a.FractionalPoint3D);
        }

        /**
         * Method to test the Compare() method.
         */
        [TestMethod()]
        public override void TestCompare_Object()
        {
            IAtom someAtom = new Atom("C");
            if (someAtom is Atom)
            {
                Atom atom = (Atom)someAtom;
                Assert.IsTrue(atom.Compare(atom));
                IAtom hydrogen = someAtom.Builder.CreateAtom("H");
                Assert.IsFalse(atom.Compare(hydrogen));
                Assert.IsFalse(atom.Compare("C"));
            }
        }
    }
}
