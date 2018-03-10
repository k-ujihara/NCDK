/* Copyright (C) 2010  Egon Willighagen <egonw@users.sf.net>
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
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Numerics;
using System.Collections.Generic;

namespace NCDK.Geometries.CIP
{
    // @cdk.module test-cip
    [TestClass()]
    public class ImmutableHydrogenTest : CDKTestCase
    {
        // FIXME: think about how to cover all other IAtom methods that are not implemented...
        [TestMethod()]
        public void TestExpectedValues()
        {
            IAtom hydrogen = new ImmutableHydrogen();
            Assert.AreEqual("H", hydrogen.Symbol);
            Assert.AreEqual(1, hydrogen.AtomicNumber);
            Assert.AreEqual(1, hydrogen.MassNumber);
        }

        [TestMethod()]
        public void TestOverwriteStaticValues()
        {
            IAtom hydrogen = new ImmutableHydrogen
            {
                Symbol = "C",
                AtomicNumber = 12,
                MassNumber = 13
            };
            Assert.AreEqual("H", hydrogen.Symbol);
            Assert.AreEqual(1, hydrogen.AtomicNumber);
            Assert.AreEqual(1, hydrogen.MassNumber);
        }

        class DummyListenerAdd : IChemObjectListener
        {
            public void OnStateChanged(ChemObjectChangeEventArgs @event)
            {
            }
        }

        class DummyListenerRemove : IChemObjectListener
        {
            public void OnStateChanged(ChemObjectChangeEventArgs @event)
            {
            }
        }

        [TestMethod()]
        public void TestListenerStuff()
        {
            IAtom hydrogen = new ImmutableHydrogen();
            Assert.AreEqual(0, hydrogen.Listeners.Count);
            hydrogen.Listeners.Add(new DummyListenerAdd());
            Assert.AreEqual(0, hydrogen.Listeners.Count);
            hydrogen.Listeners.Remove(new DummyListenerRemove());
            Assert.AreEqual(0, hydrogen.Listeners.Count);
            hydrogen.NotifyChanged();

            Assert.IsFalse(hydrogen.Notification);
            hydrogen.Notification = true;
            Assert.IsFalse(hydrogen.Notification);
        }

        [TestMethod()]
        public void TestReturnsNull()
        {
            IAtom hydrogen = new ImmutableHydrogen();
            Assert.IsNull(hydrogen.Charge);
            Assert.IsNull(hydrogen.ImplicitHydrogenCount);
            Assert.IsNull(hydrogen.Point2D);
            Assert.IsNull(hydrogen.Point3D);
            Assert.AreEqual(0, hydrogen.StereoParity);
            Assert.IsNull(hydrogen.AtomTypeName);
            Assert.IsNull(hydrogen.BondOrderSum);
            Assert.IsNull(hydrogen.CovalentRadius);
            Assert.IsNull(hydrogen.FormalCharge);
            Assert.IsNull(hydrogen.FormalNeighbourCount);
            Assert.AreEqual((Hybridization)0, hydrogen.Hybridization);
            Assert.AreEqual((BondOrder)0, hydrogen.MaxBondOrder);
            Assert.IsNull(hydrogen.Valency);
            Assert.IsNull(hydrogen.ExactMass);
            Assert.IsNull(hydrogen.NaturalAbundance);
            Assert.IsFalse(hydrogen.IsAromatic);
            Assert.IsFalse(hydrogen.IsVisited);
            Assert.IsFalse(hydrogen.IsPlaced);
            Assert.IsNull(hydrogen.Id);
            Assert.IsNull(hydrogen.GetProperties());
            Assert.IsNull(hydrogen.GetProperty<string>(""));
            Assert.IsNull(hydrogen.Builder);
        }

        [TestMethod()]
        public void TestSetIsSilent()
        {
            // because we already test that the matching get methods
            // return null, we only test that set does not throw
            // exceptions
            IAtom hydrogen = new ImmutableHydrogen
            {
                Charge = 2.0,
                ImplicitHydrogenCount = 1,
                Point2D = new Vector2(1, 2),
                Point3D = new Vector3(2, 3, 4),
                StereoParity = 1,
                AtomTypeName = "foo",
                BondOrderSum = 4.0,
                CovalentRadius = 1.4,
                FormalCharge = 1,
                FormalNeighbourCount = 2,
                Hybridization = Hybridization.Planar3,
                MaxBondOrder = BondOrder.Quadruple,
                Valency = 4,
                ExactMass = 12.0,
                NaturalAbundance = 100.0,
                IsAromatic = true,
                IsInRing = true,
                Id = "Me"
            };
            hydrogen.AddProperties(new Dictionary<object, object>());
            hydrogen.SetProperties(new Dictionary<object, object>());
            hydrogen.SetProperty("", "");
            hydrogen.RemoveProperty("");
            Assert.IsTrue(true); // to indicate we made it
        }

        [TestMethod()]
        public void TestClone()
        {
            IAtom hydrogen = new ImmutableHydrogen();
            Assert.AreEqual(hydrogen, (IAtom)hydrogen.Clone());
        }
    }
}
