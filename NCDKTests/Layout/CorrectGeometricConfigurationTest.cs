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
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using NCDK.Common.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Silent;
using NCDK.Stereo;
using NCDK.Numerics;

namespace NCDK.Layout
{
    // @author John May
    // @cdk.module test-sdg
    [TestClass()]
    public class CorrectGeometricConfigurationTest
    {
        // C/C=C/CCC
        [TestMethod()]
        public void Cis()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 3, -0.74d, 5.00d));
            m.Atoms.Add(Atom("C", 1, -1.49d, 3.70d));
            m.Atoms.Add(Atom("C", 1, -0.74d, 2.40d));
            m.Atoms.Add(Atom("C", 2, -1.49d, 1.10d));
            m.Atoms.Add(Atom("C", 3, -0.74d, -0.20d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);
            m.StereoElements.Add(new DoubleBondStereochemistry(m.Bonds[1], new IBond[] { m.Bonds[0], m.Bonds[2] },
                    DoubleBondConformation.Together));
            CorrectGeometricConfiguration.Correct(m);
            AssertPoint(m.Atoms[0], -0.74, 5.0, 0.1);
            AssertPoint(m.Atoms[1], -1.49, 3.7, 0.1);
            AssertPoint(m.Atoms[2], -0.74, 2.4, 0.1);
            AssertPoint(m.Atoms[3], 0.76, 2.4, 0.1);
            AssertPoint(m.Atoms[4], 1.51, 1.10, 0.1);
        }

        // C/C=C\CCC
        [TestMethod()]
        public void Trans()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 3, -0.74d, 5.00d));
            m.Atoms.Add(Atom("C", 1, -1.49d, 3.70d));
            m.Atoms.Add(Atom("C", 1, -0.74d, 2.40d));
            m.Atoms.Add(Atom("C", 2, 0.76d, 2.40d));
            m.Atoms.Add(Atom("C", 3, 1.51d, 1.10d));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);
            m.StereoElements.Add(new DoubleBondStereochemistry(m.Bonds[1], new IBond[] { m.Bonds[0], m.Bonds[2] },
                    DoubleBondConformation.Opposite));
            CorrectGeometricConfiguration.Correct(m);
            AssertPoint(m.Atoms[0], -0.74, 5.0, 0.1);
            AssertPoint(m.Atoms[1], -1.49, 3.7, 0.1);
            AssertPoint(m.Atoms[2], -0.74d, 2.40d, 0.1);
            AssertPoint(m.Atoms[3], -1.49d, 1.10d, 0.1);
            AssertPoint(m.Atoms[4], -0.74d, -0.20d, 0.1);
        }

        static void AssertPoint(IAtom a, double x, double y, double epsilon)
        {
            Vector2 p = a.Point2D.Value;
            Assert.AreEqual(p.X, x, epsilon);
            Assert.AreEqual(p.Y, y, epsilon);
        }

        static IAtom Atom(string symbol, int hCount, double x, double y)
        {
            IAtom a = new Atom(symbol);
            a.ImplicitHydrogenCount = hCount;
            a.Point2D = new Vector2(x, y);
            return a;
        }
    }
}
