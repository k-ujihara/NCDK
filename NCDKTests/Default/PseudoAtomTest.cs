/* Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
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
    /// Checks the functionality of the AtomTypeFactory
    ///
    // @cdk.module test-data
    /// </summary>
    [TestClass()]
    public class PseudoAtomTest
        : AbstractPseudoAtomTest
    {
        public override IChemObject NewChemObject()
        {
            return new PseudoAtom();
        }

        [TestMethod()]
        public virtual void TestPseudoAtom()
        {
            IPseudoAtom a = new PseudoAtom();
            Assert.AreEqual("R", a.Symbol);
            Assert.IsNull(a.Point3D);
            Assert.IsNull(a.Point2D);
            Assert.IsNull(a.FractionalPoint3D);
        }

        [TestMethod()]
        public virtual void TestPseudoAtom_IElement()
        {
            IElement element = NewChemObject().Builder.CreateElement();
            IPseudoAtom a = new PseudoAtom(element);
            Assert.AreEqual("R", a.Symbol);
            Assert.IsNull(a.Point3D);
            Assert.IsNull(a.Point2D);
            Assert.IsNull(a.FractionalPoint3D);
        }

        [TestMethod()]
        public virtual void TestPseudoAtom_String()
        {
            string label = "Arg255";
            IPseudoAtom a = new PseudoAtom(label);
            Assert.AreEqual("R", a.Symbol);
            Assert.AreEqual(label, a.Label);
            Assert.IsNull(a.Point3D);
            Assert.IsNull(a.Point2D);
            Assert.IsNull(a.FractionalPoint3D);
        }

        [TestMethod()]
        public virtual void TestPseudoAtom_String_Point2d()
        {
            Vector2 point = new Vector2(1, 2);
            string label = "Arg255";
            IPseudoAtom a = new PseudoAtom(label, point);
            Assert.AreEqual("R", a.Symbol);
            Assert.AreEqual(label, a.Label);
            Assert.AreEqual(point, a.Point2D);
            Assert.IsNull(a.Point3D);
            Assert.IsNull(a.FractionalPoint3D);
        }

        [TestMethod()]
        public virtual void TestPseudoAtom_String_Point3d()
        {
            Vector3 point = new Vector3(1, 2, 3);
            string label = "Arg255";
            IPseudoAtom a = new PseudoAtom(label, point);
            Assert.AreEqual("R", a.Symbol);
            Assert.AreEqual(label, a.Label);
            Assert.AreEqual(point, a.Point3D);
            Assert.IsNull(a.Point2D);
            Assert.IsNull(a.FractionalPoint3D);
        }
    }
}