/* Co.Yright (C) 2011 Gilleain Torrance <gilleain.torrance@gmail.com>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software;.You can redistribute it and/or
 * modi.Y it under the terms of the GNU Lesser General Public License
 * as published .Y the Free Software Foundation; either version 2.1
 * of the License, or (at.Your option) a.Y later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above co.Yright notice to the beginning
 * of.Your source code files, and to a.Y co.Yright notice that.You m.Y distribute
 * with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warran.Y of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a co.Y of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Silent;
using System;
using System.Collections.Generic;
using NCDK.Numerics;

namespace NCDK.Layout
{
    // @author maclean
    // @cdk.module test-sdg
    [TestClass()]
    public class AtomPlacerTest : CDKTestCase
    {
        [TestMethod()]
        public void EmptyAtomsListTest()
        {
            List<IAtom> atoms = new List<IAtom>();
            // switch on debugging, to see if NPE is thrown
            AtomPlacer placer = new AtomPlacer();
            bool npeThrown = false;
            try
            {
                AtomPlacer.PopulatePolygonCorners(atoms, Vector2.Zero, 0, 10, 10);
            }
            catch (NullReferenceException)
            {
                npeThrown = true;
            }
            Assert.IsFalse(npeThrown, "Null pointer for emp.Y atoms list");
        }

        [TestMethod()]
        public void TriangleTest()
        {
            List<IAtom> atoms = new List<IAtom>
            {
                new Atom("C"),
                new Atom("C"),
                new Atom("C")
            };
            AtomPlacer placer = new AtomPlacer();
            AtomPlacer.PopulatePolygonCorners(atoms, Vector2.Zero, 0, 10, 10);
            foreach (var atom in atoms)
            {
                Assert.IsNotNull(atom.Point2D);
            }
        }

        [TestMethod()]
        public void Cumulated_x2()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 3));
            m.Atoms.Add(Atom("C", 1));
            m.Atoms.Add(Atom("C", 0));
            m.Atoms.Add(Atom("C", 1));
            m.Atoms.Add(Atom("C", 3));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Double);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);
            m.Atoms[0].Point2D = Vector2.Zero;
            m.Atoms[0].IsPlaced = true;

            AtomPlacer atomPlacer = new AtomPlacer { Molecule = m };
            var v = new Vector2(0, 1.5);
            atomPlacer.PlaceLinearChain(m, ref v, 1.5);

            Vector2 p1 = m.Atoms[1].Point2D.Value;
            Vector2 p2 = m.Atoms[2].Point2D.Value;
            Vector2 p3 = m.Atoms[3].Point2D.Value;

            Vector2 p2p1 = new Vector2(p1.X - p2.X, p1.Y - p2.Y);
            Vector2 p2p3 = new Vector2(p3.X - p2.X, p3.Y - p2.Y);

            p2p1 = Vector2.Normalize(p2p1);
            p2p3 = Vector2.Normalize(p2p3);

            double round = (float)(p2p1.X * p2p3.X + p2p1.Y * p2p3.Y); // rounding the value to avoid over 1 value. 
            double theta = Math.Acos(round);

            Assert.AreEqual(Math.PI, theta, 0.05);
        }

        [TestMethod()]
        public void Cumulated_x3()
        {
            var m = new AtomContainer();
            m.Atoms.Add(Atom("C", 3));
            m.Atoms.Add(Atom("C", 1));
            m.Atoms.Add(Atom("C", 0));
            m.Atoms.Add(Atom("C", 0));
            m.Atoms.Add(Atom("C", 1));
            m.Atoms.Add(Atom("C", 3));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Double);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Double);
            m.AddBond(m.Atoms[4], m.Atoms[5], BondOrder.Single);
            m.Atoms[0].Point2D = Vector2.Zero;
            m.Atoms[0].IsPlaced = true;

            AtomPlacer atomPlacer = new AtomPlacer { Molecule = m };
            var v = new Vector2(0, 1.5);
            atomPlacer.PlaceLinearChain(m, ref v, 1.5);

            Vector2 p1 = m.Atoms[1].Point2D.Value;
            Vector2 p2 = m.Atoms[2].Point2D.Value;
            Vector2 p3 = m.Atoms[3].Point2D.Value;
            Vector2 p4 = m.Atoms[4].Point2D.Value;

            Vector2 p2p1 = new Vector2(p1.X - p2.X, p1.Y - p2.Y);
            Vector2 p2p3 = new Vector2(p3.X - p2.X, p3.Y - p2.Y);
            Vector2 p3p2 = new Vector2(p2.X - p3.X, p2.Y - p3.Y);
            Vector2 p3p4 = new Vector2(p4.X - p3.X, p4.Y - p3.Y);

            p2p1 = Vector2.Normalize(p2p1);
            p2p3 = Vector2.Normalize(p2p3);
            p3p2 = Vector2.Normalize(p3p2);
            p3p4 = Vector2.Normalize(p3p4);

            //Assert.AreEqual(Math.PI, Math.Acos(p2p1.X * p2p3.X + p2p1.Y * p2p3.Y), 0.05);
            //Assert.AreEqual(Math.PI, Math.Acos(p3p2.X * p3p4.X + p3p2.Y * p3p4.Y), 0.05);
            Assert.AreEqual(-1, p2p1.X * p2p3.X + p2p1.Y * p2p3.Y, 0.001);
            Assert.AreEqual(-1, p3p2.X * p3p4.X + p3p2.Y * p3p4.Y, 0.001);
        }

        static IAtom Atom(string symbol, int hCount)
        {
            IAtom a = new Atom(symbol) { ImplicitHydrogenCount = hCount };
            return a;
        }
    }
}
