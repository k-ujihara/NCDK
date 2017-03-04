/* Copyright (C) 2007  Egon Willighagen <egonw@users.sf.net>
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
 */
using NCDK.Common.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace NCDK.Maths
{
    /// <summary>
    // @cdk.module test-standard
    /// </summary>
    [TestClass()]
    public class RandomNumbersToolTest : CDKTestCase
    {

        public RandomNumbersToolTest()
            : base()
        { }

        [TestMethod()]
        public void TestGetRandomSeed()
        {
            TestSetRandomSeed_long();
        }

        [TestMethod()]
        public void TestSetRandomSeed_long()
        {
            long seed = System.DateTime.Now.Ticks;
            RandomNumbersTool.RandomSeed = seed;
            Assert.AreEqual(seed, RandomNumbersTool.RandomSeed);
        }

        [TestMethod()]
        public void TestSetRandom()
        {
            MersenneTwister rng = new MersenneTwister();
            RandomNumbersTool.Random = rng;
            Assert.AreEqual(rng, RandomNumbersTool.Random);
        }

        [TestMethod()]
        public void TestRandomInt()
        {
            int random = RandomNumbersTool.RandomInt();
            Assert.IsTrue(random == 0 || random == 1);
        }

        [TestMethod()]
        public void TestRandomBoolean()
        {
            bool random = RandomNumbersTool.RandomBoolean();
            Assert.IsTrue(random || !random);
        }

        [TestMethod()]
        public void TestRandomLong()
        {
            long random = RandomNumbersTool.RandomLong();
            Assert.IsTrue(random >= 0L);
            Assert.IsTrue(random <= 1L);
        }

        [TestMethod()]
        public void TestRandomLong_long_long()
        {
            long lower_limit = 2L;
            long upper_limit = 4L;
            long random = RandomNumbersTool.RandomLong(lower_limit, upper_limit);
            Assert.IsTrue(random >= lower_limit);
            Assert.IsTrue(random <= upper_limit);
        }

        [TestMethod()]
        public void TestRandomDouble()
        {
            double random = RandomNumbersTool.RandomDouble();
            Assert.IsTrue(random >= 0.0);
            Assert.IsTrue(random <= 1.0);
        }

        [TestMethod()]
        public void TestRandomDouble_double_double()
        {
            double lower_limit = 2.0;
            double upper_limit = 4.0;
            double random = RandomNumbersTool.RandomDouble(lower_limit, upper_limit);
            Assert.IsTrue(random >= lower_limit);
            Assert.IsTrue(random <= upper_limit);
        }

        [TestMethod()]
        public void TestRandomFloat()
        {
            float random = RandomNumbersTool.RandomFloat();
            Assert.IsTrue(random >= 0.0);
            Assert.IsTrue(random <= 1.0);
        }

        [TestMethod()]
        public void TestRandomFloat_float_float()
        {
            float lower_limit = (float)2.0;
            float upper_limit = (float)4.0;
            float random = RandomNumbersTool.RandomFloat(lower_limit, upper_limit);
            Assert.IsTrue(random >= lower_limit);
            Assert.IsTrue(random <= upper_limit);
        }

        [TestMethod()]
        public void TestRandomBit()
        {
            int random = RandomNumbersTool.RandomBit();
            Assert.IsTrue(random == 0 || random == 1);
        }

        [TestMethod()]
        public void TestRandomInt_int_int()
        {
            int random = RandomNumbersTool.RandomInt(0, 5);
            Assert.IsTrue(random == 0 || random == 1 || random == 2 || random == 3 || random == 4 || random == 5);
        }

        [TestCategory("SlowTest")]
        [TestMethod()]
        public void TestFlipCoin()
        {
            int ntry = 1000000;
            double p = 0.5;
            int ntrue = 0;
            int nfalse = 0;
            for (int i = 0; i < ntry; i++)
            {
                if (RandomNumbersTool.FlipCoin(p))
                    ntrue += 1;
                else
                    nfalse += 1;
            }
            Assert.AreEqual(0.5, (double)ntrue / ntry, 0.01);
            Assert.AreEqual(0.5, (double)nfalse / ntry, 0.01);
        }

        [TestMethod()]
        public void TestGaussianFloat()
        {
            float dev = (float)1.0;
            float epsilon = 0.01f;

            int ntry = 100000;
            float[] values = new float[ntry];
            for (int i = 0; i < ntry; i++)
                values[i] = RandomNumbersTool.GaussianFloat(dev);

            // no get the sd of the values
            float mean = 0.0f;
            for (int i = 0; i < ntry; i++)
                mean += values[i];
            mean = mean / ntry;

            float sd = 0.0f;
            for (int i = 0; i < ntry; i++)
                sd += (values[i] - mean) * (values[i] - mean);
            sd = (float)Math.Sqrt(sd / (ntry - 1));
            Assert.IsTrue(sd >= (dev - epsilon) && sd <= (dev + epsilon), "Estimated SD does not match to 2 decimal places");
        }

        [TestMethod()]
        public void TestGaussianDouble()
        {
            double dev = 2.0;
            double epsilon = 0.01;
            int ntry = 100000;
            double[] values = new double[ntry];
            for (int i = 0; i < ntry; i++)
                values[i] = RandomNumbersTool.GaussianDouble(dev);

            // no get the sd of the values
            double mean = 0;
            for (int i = 0; i < ntry; i++)
                mean += values[i];
            mean = mean / ntry;

            double sd = 0;
            for (int i = 0; i < ntry; i++)
                sd += (values[i] - mean) * (values[i] - mean);
            sd = Math.Sqrt(sd / (ntry - 1));
            Assert.IsTrue(sd >= (dev - epsilon) && sd <= (dev + epsilon), "Estimated SD does not match to 2 decimal places");
        }

        [TestMethod()]
        public void TestExponentialDouble()
        {
            double mean = 1.0f;
            double epsilon = 0.01f;
            int ntry = 100000;
            double[] values = new double[ntry];

            for (int i = 0; i < ntry; i++)
                values[i] = RandomNumbersTool.ExponentialDouble(mean);

            // no get the mean of the values
            double m = 0.0f;
            for (int i = 0; i < ntry; i++)
                m += values[i];
            m = m / ntry;

            Assert.IsTrue(m >= (mean - epsilon) && m <= (mean + epsilon), "Estimated mean does not match to 2 decimal places " + m);
        }
    }
}
