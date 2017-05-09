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
    public class Tetrahedral3DParityTest
    {
        private static int Clockwise = -1;
        private static int AntiClockwise = +1;

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstruction_Empty()
        {
            new Tetrahedral3DParity(new Vector3[0]);
        }

        [TestMethod()]
        public void TestParity_Three_Clockwise()
        {
            Vector3[] coords = new Vector3[]{new Vector3((float)1.70,(float)0.98, (float)-0.51), // -O
                new Vector3((float)2.65,(float)-0.83, (float)0.62), // -N
                new Vector3((float)0.26,(float)-0.33, (float)0.95), // -C
                new Vector3((float)1.44,(float)-0.33, (float)-0.03), // C (centre)
        };
            Assert.AreEqual(Clockwise, new Tetrahedral3DParity(coords).Parity);
        }

        [TestMethod()]
        public void TestParity_Three_Anticlockwise()
        {
            Vector3[] coords = new Vector3[]{new Vector3((float)1.70,(float)0.98, (float)-0.51), // -O
                new Vector3((float)0.26,(float)-0.33, (float)0.95), // -C
                new Vector3((float)2.65,(float)-0.83, (float)0.62), // -N
                new Vector3((float)1.44,(float)-0.33, (float)-0.03), // C (centre)
        };
            Assert.AreEqual(AntiClockwise, new Tetrahedral3DParity(coords).Parity);
        }

        [TestMethod()]
        public void TestParity_Four_Clockwise()
        {
            Vector3[] coords = new Vector3[]{new Vector3((float)1.70,(float)0.98, (float)-0.51), // -O
                new Vector3((float)2.65,(float)-0.83, (float)0.62), // -N
                new Vector3((float)0.26,(float)-0.33, (float)0.95), // -C
                new Vector3((float)1.21,(float)-0.97, (float)-0.89), // -H
        };
            Assert.AreEqual(Clockwise, new Tetrahedral3DParity(coords).Parity);
        }

        [TestMethod()]
        public void TestParity_Four_Anticlockwise()
        {
            Vector3[] coords = new Vector3[] {
                new Vector3((float)1.70,(float)0.98, (float)-0.51), // -O
                new Vector3((float)0.26,(float)-0.33, (float)0.95), // -C
                new Vector3((float)2.65,(float)-0.83, (float)0.62), // -N
                new Vector3((float)1.21,(float)-0.97, (float)-0.89), // -H
            };
            Assert.AreEqual(AntiClockwise, new Tetrahedral3DParity(coords).Parity);
        }
    }
}
