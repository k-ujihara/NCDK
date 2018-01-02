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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Numerics;
using Moq;
using static NCDK.Renderers.Generators.Standards.HydrogenPosition;
using System;
using NCDK.Common.Mathematics;
using NCDK.Common.Base;

namespace NCDK.Renderers.Generators.Standards
{
    [TestClass()]
    public class HydrogenPositionTest
    {
        [TestMethod()]
        public void CardinalDirectionForNorthIsBelow()
        {
            Assert.AreEqual(Below, HydrogenPositionTools.UsingCardinalDirection(new Vector2(0, 1)));
        }

        [TestMethod()]
        public void CardinalDirectionForNorthEastIsLeft()
        {
            Assert.AreEqual(Left, HydrogenPositionTools.UsingCardinalDirection(new Vector2(1, 1)));
        }

        [TestMethod()]
        public void CardinalDirectionForEastIsLeft()
        {
            Assert.AreEqual(Left, HydrogenPositionTools.UsingCardinalDirection(new Vector2(1, 0)));
        }

        [TestMethod()]
        public void CardinalDirectionForSouthEastIsLeft()
        {
            Assert.AreEqual(Left, HydrogenPositionTools.UsingCardinalDirection(new Vector2(1, -1)));
        }

        [TestMethod()]
        public void CardinalDirectionForSouthIsAbove()
        {
            Assert.AreEqual(Above, HydrogenPositionTools.UsingCardinalDirection(new Vector2(0, -1)));
        }

        [TestMethod()]
        public void CardinalDirectionForSouthWestIsRight()
        {
            Assert.AreEqual(Right, HydrogenPositionTools.UsingCardinalDirection(new Vector2(-1, -1)));
        }

        [TestMethod()]
        public void CardinalDirectionForWestIsRight()
        {
            Assert.AreEqual(Right, HydrogenPositionTools.UsingCardinalDirection(new Vector2(-1, 0)));
        }

        [TestMethod()]
        public void CardinalDirectionForNorthWestIsRight()
        {
            Assert.AreEqual(Right, HydrogenPositionTools.UsingCardinalDirection(new Vector2(-1, 0)));
        }

        [TestMethod()]
        public void HydrogensAppearBeforeOxygen()
        {
            var mock_atom = new Mock<IAtom>(); var atom = mock_atom.Object;
            mock_atom.Setup(n => n.AtomicNumber).Returns(8);
            Assert.AreEqual(Left, HydrogenPositionTools.UsingDefaultPlacement(atom));
        }

        [TestMethod()]
        public void HydrogensAppearBeforeSulfur()
        {
            var mock_atom = new Mock<IAtom>(); var atom = mock_atom.Object;
            mock_atom.Setup(n => n.AtomicNumber).Returns(16);
            Assert.AreEqual(Left, HydrogenPositionTools.UsingDefaultPlacement(atom));
        }

        [TestMethod()]
        public void HydrogensAppearAfterNitrogen()
        {
            var mock_atom = new Mock<IAtom>(); var atom = mock_atom.Object;
            mock_atom.Setup(n => n.AtomicNumber).Returns(7);
            Assert.AreEqual(Right, HydrogenPositionTools.UsingDefaultPlacement(atom));
        }

        [TestMethod()]
        public void HydrogensAppearAfterCarbon()
        {
            var mock_atom = new Mock<IAtom>(); var atom = mock_atom.Object;
            mock_atom.Setup(n => n.AtomicNumber).Returns(6);
            Assert.AreEqual(Right, HydrogenPositionTools.UsingDefaultPlacement(atom));
        }

        [TestMethod()]
        public void HydrogensAppearAfterWhenBondIsFromLeft()
        {
            var mock_atom1 = new Mock<IAtom>(); var atom1 = mock_atom1.Object;
            var mock_atom2 = new Mock<IAtom>(); var atom2 = mock_atom2.Object;
            mock_atom1.Setup(n => n.Point2D).Returns(new Vector2(0, 0));
            mock_atom2.Setup(n => n.Point2D).Returns(new Vector2(-1, 0));
            Assert.AreEqual(Right, HydrogenPositionTools.Position(atom1, new[] { atom2 }));
        }

        [TestMethod()]
        public void HydrogensAppearBeforeWhenBondIsFromRight()
        {
            var mock_atom1 = new Mock<IAtom>(); var atom1 = mock_atom1.Object;
            var mock_atom2 = new Mock<IAtom>(); var atom2 = mock_atom2.Object;
            mock_atom1.Setup(n => n.Point2D).Returns(new Vector2(0, 0));
            mock_atom2.Setup(n => n.Point2D).Returns(new Vector2(1, 0));
            Assert.AreEqual(Left, HydrogenPositionTools.Position(atom1, new[] { atom2 }));
        }

        [TestMethod()]
        public void UsingCardinalDirection()
        {
            var mock_atom1 = new Mock<IAtom>(); var atom1 = mock_atom1.Object;
            var mock_atom2 = new Mock<IAtom>(); var atom2 = mock_atom2.Object;
            var mock_atom3 = new Mock<IAtom>(); var atom3 = mock_atom2.Object;
            mock_atom1.Setup(n => n.Point2D).Returns(new Vector2(0, 0));
            mock_atom2.Setup(n => n.Point2D).Returns(new Vector2(1, 1));
            mock_atom3.Setup(n => n.Point2D).Returns(new Vector2(1, -1));
            Assert.AreEqual(Left, HydrogenPositionTools.Position(atom1, new[] { atom2, atom3 }));
        }

        [TestMethod()]
        public void useDefaultPlacementWithNoBonds()
        {
            var mock_atom = new Mock<IAtom>(); var atom = mock_atom.Object;
            mock_atom.Setup(n => n.AtomicNumber).Returns(8);
            Assert.AreEqual(Left, HydrogenPositionTools.Position(atom, Array.Empty<IAtom>()));
        }

        [TestMethod()]
        public void Values()
        {
            Assert.IsTrue(Compares.AreDeepEqual(
                new HydrogenPosition[] { Right, Left, Above, Below }, 
                HydrogenPositionTools.Values));
        }

        [TestMethod()]
        public void ValueOf()
        {
            Assert.AreEqual(HydrogenPosition.Above, HydrogenPositionTools.Parse("Above"));
        }

        [TestMethod()]
        public void AngularExtentRight()
        {
            double theta = Vectors.DegreeToRadian(60);
            var vectors = new[]
            {
                new Vector2(-1, 0),
                new Vector2(Math.Cos(theta), Math.Sin(theta)),
                new Vector2(Math.Cos(-theta), Math.Sin(-theta)),
            };
            Assert.AreEqual(Right, HydrogenPositionTools.UsingAngularExtent(vectors));
        }

        [TestMethod()]
        public void AngularExtentLeft()
        {
            double theta = Vectors.DegreeToRadian(120);
            var vectors = new[]
            {
                new Vector2(1, 0),
                new Vector2(Math.Cos(theta), Math.Sin(theta)),
                new Vector2(Math.Cos(-theta), Math.Sin(-theta)),
            };
            Assert.AreEqual(Left, HydrogenPositionTools.UsingAngularExtent(vectors));
        }

        [TestMethod()]
        public void AngularExtentBelow()
        {
            double theta1 = Vectors.DegreeToRadian(210);
            double theta2 = Vectors.DegreeToRadian(330);
            var vectors = new[]
            {
                new Vector2(0, 1),
                new Vector2(Math.Cos(theta1), Math.Sin(theta1)),
                new Vector2(Math.Cos(theta2), Math.Sin(theta2)),
            };
            Assert.AreEqual(Below, HydrogenPositionTools.UsingAngularExtent(vectors));
        }

        [TestMethod()]
        public void AngularExtentAbove()
        {
            double theta1 = Vectors.DegreeToRadian(30);
            double theta2 = Vectors.DegreeToRadian(150);
            var vectors = new[]
            {
                new Vector2(0, -1),
                new Vector2(Math.Cos(theta1), Math.Sin(theta1)),
                new Vector2(Math.Cos(theta2), Math.Sin(theta2)),
            };
            Assert.AreEqual(Above, HydrogenPositionTools.UsingAngularExtent(vectors));
        }

        [TestMethod()]
        public void Symmetric()
        {
            // all extents are the same so 'Right' is chosen in preference
            var vectors = new[]
            {
                new Vector2(1, 1), new Vector2(1, -1), new Vector2(-1, 1), new Vector2(-1, -1)
            };
            Assert.AreEqual(Right, HydrogenPositionTools.UsingAngularExtent(vectors));
        }

        [TestMethod()]
        public void LargestExtent()
        {
            // the largest extents here are above and below
            var vectors = new[]
            {
                new Vector2(Math.Cos(Vectors.DegreeToRadian(30)), Math.Sin(Vectors.DegreeToRadian(30))),
                new Vector2(Math.Cos(Vectors.DegreeToRadian(-30)), Math.Sin(Vectors.DegreeToRadian(-30))),
                new Vector2(Math.Cos(Vectors.DegreeToRadian(150)), Math.Sin(Vectors.DegreeToRadian(150))),
                new Vector2(Math.Cos(Vectors.DegreeToRadian(-150)), Math.Sin(Vectors.DegreeToRadian(-150)))
            };
            Assert.AreEqual(Above, HydrogenPositionTools.UsingAngularExtent(vectors));
        }
    }
}
