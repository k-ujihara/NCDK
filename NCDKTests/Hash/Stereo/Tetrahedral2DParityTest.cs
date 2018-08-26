/*
 * Copyright (c) 2013. John May <jwmay@users.sf.net>
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
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using NCDK.Common.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using NCDK.Numerics;

namespace NCDK.Hash.Stereo
{
    // @author John May
    // @cdk.module test-hash
    [TestClass()]
    public class Tetrahedral2DParityTest
    {
        private const int Clockwise = -1;
        private const int AntiClockwise = +1;
        private const int None = 0;

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstruction_InvalidCoords()
        {
            new Tetrahedral2DParity(Array.Empty<Vector2>(), new int[4]);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstruction_InvalidElev()
        {
            new Tetrahedral2DParity(new Vector2[4], Array.Empty<int>());
        }

        /// <summary>
        /// aminoethanol (explicit H) hatch bond on hydrogen (none,none,none,down)
        /// </summary>
        // @cdk.inchi InChI=1S/C2H7NO/c1-2(3)4/h2,4H,3H2,1H3/t2-/m1/s1
        [TestMethod()]
        public void TestParity_Four_NNND()
        {
            Vector2[] coords = new Vector2[]{new Vector2((float)-7.75,(float)5.79), // -O
                new Vector2((float)-6.42,(float)3.48), // -N
                new Vector2((float)-9.09,(float)3.48), // -C
                new Vector2(-9.09,(float)5.02), // -H (down)
        };
            int[] elev = new int[] { 0, 0, 0, -1 };
            GeometricParity parity = new Tetrahedral2DParity(coords, elev);
            Assert.AreEqual(Clockwise, parity.Parity);
        }

        /// <summary>
        /// aminoethanol (explicit H) wedge on hydrogen (none,none,none,up)
        /// </summary>
        // @cdk.inchi InChI=1S/C2H7NO/c1-2(3)4/h2,4H,3H2,1H3/t2-/m1/s1
        [TestMethod()]
        public void TestParity_Four_NNNU()
        {
            Vector2[] coords = new Vector2[]{new Vector2((float)-7.75,(float)5.79), // -O
                new Vector2((float)-6.42,(float)3.48), // -N
                new Vector2((float)-9.09,(float)3.48), // -C
                new Vector2(-9.09,(float)5.02), // -H (up)
        };
            int[] elev = new int[] { 0, 0, 0, 1 };
            GeometricParity parity = new Tetrahedral2DParity(coords, elev);
            Assert.AreEqual(AntiClockwise, parity.Parity);
        }

        /// <summary>
        /// aminoethanol (explicit H) with no wedge/hatch bonds
        /// (none,none,none,none)
        /// </summary>
        // @cdk.inchi InChI=1S/C2H7NO/c1-2(3)4/h2,4H,3H2,1H3/t2-/m1/s1
        [TestMethod()]
        public void TestParity_Four_NNNN()
        {
            Vector2[] coords = new Vector2[]{new Vector2((float)-7.75,(float)5.79), // -O
                new Vector2((float)-6.42,(float)3.48), // -N
                new Vector2((float)-9.09,(float)3.48), // -C
                new Vector2((float)-9.09,(float)5.02), // -H
        };
            int[] elev = new int[] { 0, 0, 0, 0 }; // no wedge/hatch bonds
            GeometricParity parity = new Tetrahedral2DParity(coords, elev);
            Assert.AreEqual(None, parity.Parity);
        }

        /// <summary>
        /// aminoethanol (explicit H) with a wedge bond on non hydrogens
        /// (up,up,up,none)
        /// </summary>
        // @cdk.inchi InChI=1S/C2H7NO/c1-2(3)4/h2,4H,3H2,1H3/t2-/m1/s1
        [TestMethod()]
        public void TestParity_Four_UUUN()
        {
            Vector2[] coords = new Vector2[]{new Vector2((float)-7.75,(float)5.79), // -O
                new Vector2((float)-6.42,(float)3.48), // -N
                new Vector2((float)-9.09,(float)3.48), // -C
                new Vector2((float)-9.09,(float)5.02), // -H
        };
            int[] elev = new int[] { 1, 1, 1, 0 }; // no wedge/hatch bonds
            GeometricParity parity = new Tetrahedral2DParity(coords, elev);
            Assert.AreEqual(Clockwise, parity.Parity);
        }

        /// <summary>
        /// aminoethanol (explicit H) with a wedge bond on non hydrogens
        /// (down,down,down,none)
        /// </summary>
        // @cdk.inchi InChI=1S/C2H7NO/c1-2(3)4/h2,4H,3H2,1H3/t2-/m1/s1
        [TestMethod()]
        public void TestParity_Four_DDDN()
        {
            Vector2[] coords = new Vector2[]{new Vector2((float)-7.75,(float)5.79), // -O
                new Vector2((float)-6.42,(float)3.48), // -N
                new Vector2((float)-9.09,(float)3.48), // -C
                new Vector2((float)-9.09,(float)5.02), // -H
        };
            int[] elev = new int[] { -1, -1, -1, 0 }; // no wedge/hatch bonds
            GeometricParity parity = new Tetrahedral2DParity(coords, elev);
            Assert.AreEqual(AntiClockwise, parity.Parity);
        }

        /// <summary>
        /// aminoethanol (explicit H) with a wedge bond on all atoms (up,up,up,up) -
        /// makes no sense
        /// </summary>
        // @cdk.inchi InChI=1S/C2H7NO/c1-2(3)4/h2,4H,3H2,1H3/t2-/m1/s1
        [TestMethod()]
        public void TestParity_Four_UUUU()
        {
            Vector2[] coords = new Vector2[]{new Vector2((float)-7.75,(float)5.79), // -O
                new Vector2((float)-6.42,(float)3.48), // -N
                new Vector2((float)-9.09,(float)3.48), // -C
                new Vector2((float)-9.09,(float)5.02), // -H
        };
            int[] elev = new int[] { 1, 1, 1, 1 }; // no wedge/hatch bonds
            GeometricParity parity = new Tetrahedral2DParity(coords, elev);
            Assert.AreEqual(None, parity.Parity);
        }

        /// <summary>
        /// aminoethanol (explicit H) with a hatch bond on all atoms
        /// (down,down,down,down) - makes no sense
        /// </summary>
        // @cdk.inchi InChI=1S/C2H7NO/c1-2(3)4/h2,4H,3H2,1H3/t2-/m1/s1
        [TestMethod()]
        public void TestParity_Four_DDDD()
        {
            Vector2[] coords = new Vector2[]{new Vector2((float)-7.75,(float)5.79), // -O
                new Vector2((float)-6.42,(float)3.48), // -N
                new Vector2((float)-9.09,(float)3.48), // -C
                new Vector2((float)-9.09,(float)5.02), // -H
        };
            int[] elev = new int[] { -1, -1, -1, -1 }; // no wedge/hatch bonds
            GeometricParity parity = new Tetrahedral2DParity(coords, elev);
            Assert.AreEqual(None, parity.Parity);
        }

        /// <summary>
        /// aminoethanol (implicit H) (up,none,none)
        /// </summary>
        // @cdk.inchi InChI=1S/C2H7NO/c1-2(3)4/h2,4H,3H2,1H3/t2-/m1/s1
        [TestMethod()]
        public void TestParity_Three_UNN()
        {
            Vector2[] coords = new Vector2[]{new Vector2(-7.75,(float)5.79), // -O (up)
                new Vector2((float)-6.42,(float)3.48), // -N
                new Vector2((float)-9.09,(float)3.48), // -C
                new Vector2(-7.75,(float)4.25), //  C (centre)
        };
            int[] elev = new int[] { 1, 0, 0, 0 };
            GeometricParity parity = new Tetrahedral2DParity(coords, elev);
            Assert.AreEqual(Clockwise, parity.Parity);
        }

        /// <summary>
        /// aminoethanol (implicit H) (up,up,up)
        /// </summary>
        // @cdk.inchi InChI=1S/C2H7NO/c1-2(3)4/h2,4H,3H2,1H3/t2-/m1/s1
        [TestMethod()]
        public void TestParity_Three_UUU()
        {
            Vector2[] coords = new Vector2[]{new Vector2(-7.75,(float)5.79), // -O (up)
                new Vector2((float)-6.42,(float)3.48), // -N
                new Vector2((float)-9.09,(float)3.48), // -C
                new Vector2(-7.75,(float)4.25), //  C (centre)
        };
            int[] elev = new int[] { 1, 1, 1, 0 };
            GeometricParity parity = new Tetrahedral2DParity(coords, elev);
            Assert.AreEqual(Clockwise, parity.Parity);
        }

        /// <summary>
        /// aminoethanol (implicit H) (down, none, none)
        /// </summary>
        // @cdk.inchi InChI=1S/C2H7NO/c1-2(3)4/h2,4H,3H2,1H3/t2-/m1/s1
        [TestMethod()]
        public void TestParity_Three_DNN()
        {
            Vector2[] coords = new Vector2[]{new Vector2(-7.75,(float)5.79), // -O (down)
                new Vector2((float)-6.42,(float)3.48), // -N
                new Vector2((float)-9.09,(float)3.48), // -C
                new Vector2(-7.75,(float)4.25), //  C (centre)
        };
            int[] elev = new int[] { -1, 0, 0, 0 };
            GeometricParity parity = new Tetrahedral2DParity(coords, elev);
            Assert.AreEqual(AntiClockwise, parity.Parity);
        }

        /// <summary>
        /// aminoethanol (implicit H) (down, none, none)
        /// </summary>
        // @cdk.inchi InChI=1S/C2H7NO/c1-2(3)4/h2,4H,3H2,1H3/t2-/m1/s1
        [TestMethod()]
        public void TestParity_Three_DDD()
        {
            Vector2[] coords = new Vector2[]{new Vector2(-7.75,(float)5.79), // -O (down)
                new Vector2((float)-6.42,(float)3.48), // -N
                new Vector2((float)-9.09,(float)3.48), // -C
                new Vector2(-7.75,(float)4.25), //  C (centre)
        };
            int[] elev = new int[] { -1, -1, -1, 0 };
            GeometricParity parity = new Tetrahedral2DParity(coords, elev);
            Assert.AreEqual(AntiClockwise, parity.Parity);
        }
    }
}
