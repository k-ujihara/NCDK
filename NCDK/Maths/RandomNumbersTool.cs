/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using System;

namespace NCDK.Maths
{
    public interface IRandomNumbersTool
    {

    }

    [Serializable]
    public class Random
    {
        public Random()
            : this(DateTime.Now.Ticks)
        { }

        public Random(long seed)
            : this((UInt64)seed)
        { }

        public Random(UInt64 seed)
        {
            this.seed = (seed ^ 0x5DEECE66DUL) & ((1UL << 48) - 1);
        }

        public int NextInt(int n)
        {
            if (n <= 0) throw new ArgumentException("n must be positive");

            if ((n & -n) == n)  // i.e., n is a power of 2
                return (int)((n * (long)Next(31)) >> 31);

            long bits, val;
            do
            {
                bits = Next(31);
                val = bits % (UInt32)n;
            }
            while (bits - val + (n - 1) < 0);

            return (int)val;
        }

        protected UInt32 Next(int bits)
        {
            seed = (seed * 0x5DEECE66DL + 0xBL) & ((1L << 48) - 1);

            return (UInt32)(seed >> (48 - bits));
        }

        private UInt64 seed;
    }

    /**
     * Class supplying useful methods to generate random numbers.
     * This class isn't supposed to be instantiated. You should use it by calling
     * its static methods.
     *
     * @cdk.module standard
     * @cdk.githash
     */
    public class RandomNumbersTool
    {
        private static BitsStreamGenerator random;
        private static long randomSeed;

        static RandomNumbersTool()
        {
            randomSeed = System.DateTime.Now.Ticks;
            random = new MersenneTwister(randomSeed);
        }

        /// <summary>
        /// The instance of Random used by this class.
        /// </summary>
        public static BitsStreamGenerator Random
        {
            get { return random; }
            set { random = value; }
        }

        /// <summary>
        /// The seed being used by this random number generator.
        /// </summary>
        public static long RandomSeed
        {
            get
            {
                return randomSeed;
            }
            set
            {
                randomSeed = value;
                random.SetSeed(randomSeed);
            }
        }

        /// <summary>
        ///  Generates a random integer between <code>0</code> and <code>1</code>.
        /// </summary>
        /// <returns> a random integer between <code>0</code> and <code>1</code>.</returns>
        public static int RandomInt() => RandomInt(0, 1);

        /// <summary>
        /// Generates a random integer between the specified values.
        /// </summary>
        /// <param name="lo">the lower bound for the generated integer.</param>
        /// <param name="hi">the upper bound for the generated integer.</param>
        /// <returns> a random integer between <code>lo</code> and <code>hi</code>.</returns>
        public static int RandomInt(int lo, int hi)
        {
            return (Math.Abs(random.Next()) % (hi - lo + 1)) + lo;
        }

        /// <summary>
        /// Generates a random long between <code>0</code> and <code>1</code>.
        /// </summary>
        /// <returns>a random long between <code>0</code> and <code>1</code>.</returns>
        public static long RandomLong()
        {
            return RandomLong(0, 1);
        }

        /**
         * Generates a random long between the specified values.
         * <p/>
         *
         * @param lo the lower bound for the generated long.
         * @param hi the upper bound for the generated long.
         * @return a random long between <code>lo</code> and <code>hi</code>.
         */
        public static long RandomLong(long lo, long hi)
        {
            return NextLong(random, hi - lo + 1L) + lo;
        }

        /// <summary>
        /// Access the next long random number between 0 and n.
        /// </summary>
        /// <param name="rng">random number generator</param>
        /// <param name="n">max value</param>
        /// <returns>a long random number between 0 and n</returns>
        /// <remarks><a href="http://stackoverflow.com/questions/2546078/java-random-long-number-in-0-x-n-range">Random Long Number in range, Stack Overflow</a></remarks>
        private static long NextLong(BitsStreamGenerator rng, long n)
        {
            if (n <= 0) throw new ArgumentException("n must be greater than 0");
            long bits, val;
            do
            {
                bits = (long)((ulong)(rng.NextLong() << 1) >> 1);
                val = bits % n;
            } while (bits - val + (n - 1) < 0L);
            return val;
        }

        /// <summary>
        /// Generates a random float between <code>0</code> and <code>1</code>.
        /// </summary>
        /// <returns>a random float between <code>0</code> and <code>1</code>.</returns>
        public static float RandomFloat()
        {
            return random.NextFloat();
        }

        /// <summary>
        /// Generates a random float between the specified values.
        /// </summary>
        /// <param name="lo">the lower bound for the generated float.</param>
        /// <param name="hi">the upper bound for the generated float.</param>
        /// <returns>a random float between <code>lo</code> and <code>hi</code>.</returns>
        public static float RandomFloat(float lo, float hi)
        {
            return (hi - lo) * random.NextFloat() + lo;
        }

        /// <summary>
        /// Generates a random double between <code>0</code> and <code>1</code>.
        /// </summary>
        /// <returns>a random double between <code>0</code> and <code>1</code>.</returns>
        public static double RandomDouble()
        {
            return random.NextDouble();
        }

        /**
         * Generates a random double between the specified values.
         * <p/>
         *
         * @param lo the lower bound for the generated double.
         * @param hi the upper bound for the generated double.
         * @return a random double between <code>lo</code> and <code>hi</code>.
         */
        public static double RandomDouble(double lo, double hi)
        {
            return (hi - lo) * random.NextDouble() + lo;
        }

        /**
         * Generates a random bool.
         * <p/>
         *
         * @return a random bool.
         */
        public static bool RandomBoolean()
        {
            return (RandomInt() == 1);
        }

        /**
         * Generates a random bit: either <code>0</code> or <code>1</code>.
         * <p/>
         *
         * @return a random bit.
         */
        public static int RandomBit()
        {
            return RandomInt();
        }

        /**
         * Returns a bool value based on a biased coin toss.
         * <p/>
         *
         * @param p the probability of success.
         * @return <see langword="true"/> if a success was found; <see langword="false"/>
         *         otherwise.
         */
        public static bool FlipCoin(double p)
        {
            return (RandomDouble() < p ? true : false);
        }

        /**
         * Generates a random float from a Gaussian distribution with the specified
         * deviation.
         * <p/>
         *
         * @param dev the desired deviation.
         * @return a random float from a Gaussian distribution with deviation
         *         <code>dev</code>.
         */
        public static float GaussianFloat(float dev)
        {
            return (float)random.NextGaussian() * dev;
        }

        /**
         * Generates a random double from a Gaussian distribution with the specified
         * deviation.
         * <p/>
         *
         * @param dev the desired deviation.
         * @return a random double from a Gaussian distribution with deviation
         *         <code>dev</code>.
         */
        public static double GaussianDouble(double dev)
        {
            return random.NextGaussian() * dev;
        }

        /**
         * Generates a random double from an Exponential distribution with the specified
         * mean value.
         * <p/>
         *
         * @param mean the desired mean value.
         * @return a random double from an Exponential distribution with mean value
         *         <code>mean</code>.
         */
        public static double ExponentialDouble(double mean)
        {
            return -mean * Math.Log(RandomDouble());
        }
    }

    /// <summary>
    /// Base class for random number generators that generates bits streams.
    /// </summary>
    // @version $Id: BitsStreamGenerator.java 1244107 2012-02-14 16:17:55Z erans $
    // @since 2.0
    public abstract class BitsStreamGenerator
    {
        /// <summary>Next gaussian.</summary>
        private double nextGaussian;

        /** Creates a new random number generator.
         */
        public BitsStreamGenerator()
        {
            nextGaussian = double.NaN;
        }

        /// <inheritdoc/>
        public abstract void SetSeed(int seed);

        /// <inheritdoc/>
        public abstract void SetSeed(int[] seed);

        /// <inheritdoc/>
        public abstract void SetSeed(long seed);

        /** Generate next pseudorandom number.
         * <p>This method is the core generation algorithm. It is used by all the
         * public generation methods for the various primitive types {@link
         * #NextBoolean()}, {@link #NextBytes(byte[])}, {@link #NextDouble()},
         * {@link #NextFloat()}, {@link #NextGaussian()}, {@link #NextInt()},
         * {@link #Next(int)} and {@link #NextLong()}.</p>
         * @param bits number of random bits to produce
         * @return random bits generated
         */
        protected abstract uint Next(int bits);

        /// <inheritdoc/>
        public bool NextBool()
        {
            return Next(1) != 0;
        }

        /// <inheritdoc/>
        public void NextBytes(byte[] bytes)
        {
            int i = 0;
            int iEnd = bytes.Length - 3;
            while (i < iEnd)
            {
                uint random = Next(32);
                bytes[i] = (byte)(random & 0xff);
                bytes[i + 1] = (byte)((random >> 8) & 0xff);
                bytes[i + 2] = (byte)((random >> 16) & 0xff);
                bytes[i + 3] = (byte)((random >> 24) & 0xff);
                i += 4;
            }
            {
                uint random = Next(32);
                while (i < bytes.Length)
                {
                    bytes[i++] = (byte)(random & 0xff);
                    random = random >> 8;
                }
            }
        }

        private const double Double_1_10000000000000 = 1.0d / 0x10000000000000;

        /// <inheritdoc/>
        public double NextDouble()
        {
            ulong high = ((ulong)Next(26)) << 26;
            ulong low = (ulong)Next(26);
            ulong s = high | low;
            var ret = Double_1_10000000000000 * s;
            return ret;
        }

        private const double Double_1_800000 = 1.0d / 0x800000;

        /// <inheritdoc/>
        public float NextFloat()
        {
            var ret = Double_1_800000 * Next(23);
            return (float)ret;
        }

        /// <inheritdoc/>
        public double NextGaussian()
        {
            double random;
            if (double.IsNaN(nextGaussian))
            {
                // generate a new pair of gaussian numbers
                double x = NextDouble();
                double y = NextDouble();
                double alpha = 2 * Math.PI * x;
                double r = Math.Sqrt(-2 * Math.Log(y));
                random = r * Math.Cos(alpha);
                nextGaussian = r * Math.Sin(alpha);
            }
            else
            {
                // use the second element of the pair already generated
                random = nextGaussian;
                nextGaussian = double.NaN;
            }

            return random;
        }

        /// <inheritdoc/>
        public int Next()
        {
            return (int)Next(32);
        }

        /**
         * {@inheritDoc}
         * <p>This default implementation is copied from Apache Harmony
         * java.util.Random (r929253).</p>
         *
         * <p>Implementation notes: <ul>
         * <li>If n is a power of 2, this method returns
         * {@code (int) ((n * (long) Next(31)) >> 31)}.</li>
         *
         * <li>If n is not a power of 2, what is returned is {@code Next(31) % n}
         * with {@code Next(31)} values rejected (i.e. regenerated) until a
         * value that is larger than the remainder of {@code int.MaxValue / n}
         * is generated. Rejection of this initial segment is necessary to ensure
         * a uniform distribution.</li></ul></p>
         */
        public int NextInt(int n)
        {
            if (n > 0)
            {
                if ((n & -n) == n)
                {
                    var nn = (ulong)Next(31);
                    return (int)(((ulong)n * nn) >> 31);
                }
                int bits;
                int val;
                do
                {
                    bits = (int)Next(31);
                    val = bits % n;
                } while (bits - val + (n - 1) < 0);
                return val;
            }
            throw new ArgumentOutOfRangeException(nameof(n));
        }

        /// <inheritdoc/>
        public long NextLong()
        {
            long high = ((long)Next(32)) << 32;
            long low = ((long)Next(32)) & 0xffffffffL;
            return high | low;
        }

        /**
         * Clears the cache used by the default implementation of
         * {@link #nextGaussian}.
         */
        public void Clear()
        {
            nextGaussian = double.NaN;
        }
    }

    /** This class implements a powerful pseudo-random number generator
        * developed by Makoto Matsumoto and Takuji Nishimura during
        * 1996-1997.

        * <p>This generator features an extremely long period
        * (2<sup>19937</sup>-1) and 623-dimensional equidistribution up to 32
        * bits accuracy. The home page for this generator is located at <a
        * href="http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/emt.html">
        * http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/emt.html</a>.</p>

        * <p>This generator is described in a paper by Makoto Matsumoto and
        * Takuji Nishimura in 1998: <a
        * href="http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/ARTICLES/mt.pdf">Mersenne
        * Twister: A 623-Dimensionally Equidistributed Uniform Pseudo-Random
        * Number Generator</a>, ACM Transactions on Modeling and Computer
        * Simulation, Vol. 8, No. 1, January 1998, pp 3--30</p>

        * <p>This class is mainly a Java port of the 2002-01-26 version of
        * the generator written in C by Makoto Matsumoto and Takuji
        * Nishimura. Here is their original copyright:</p>

        * <table border="0" width="80%" cellpadding="10" align="center" bgcolor="#E0E0E0">
        * <tr><td>Copyright (C) 1997 - 2002, Makoto Matsumoto and Takuji Nishimura,
        *     All rights reserved.</td></tr>

        * <tr><td>Redistribution and use in source and binary forms, with or without
        * modification, are permitted provided that the following conditions
        * are met:
        * <ol>
        *   <li>Redistributions of source code must retain the above copyright
        *       notice, this list of conditions and the following disclaimer.</li>
        *   <li>Redistributions in binary form must reproduce the above copyright
        *       notice, this list of conditions and the following disclaimer in the
        *       documentation and/or other materials provided with the distribution.</li>
        *   <li>The names of its contributors may not be used to endorse or promote
        *       products derived from this software without specific prior written
        *       permission.</li>
        * </ol></td></tr>

        * <tr><td><strong>THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
        * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
        * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
        * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
        * DISCLAIMED.  IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS
        * BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY,
        * OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
        * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
        * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY
        * OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
        * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE
        * USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
        * DAMAGE.</strong></td></tr>
        * </table>

        * @version $Id: MersenneTwister.java 1244107 2012-02-14 16:17:55Z erans $
        * @since 2.0

        */
    public class MersenneTwister : BitsStreamGenerator
    {
        /// <summary>Size of the bytes pool.</summary>
        private const int N = 624;

        /// <summary>Period second parameter.</summary>
        private const int M = 397;

        /// <summary>X * MATRIX_A for X = {0, 1}.</summary>
        private static readonly uint[] MAG01 = { 0x0, 0x9908b0df };

        /// <summary>Bytes pool.</summary>
        private uint[] mt;

        /// <summary>Current index in the bytes pool.</summary>
        private uint mti;

        /** Creates a new random number generator.
         * <p>The instance is initialized using the current time plus the
         * system identity hash code of this instance as the seed.</p>
         */
        public MersenneTwister()
        {
            mt = new uint[N];
            SetSeed(System.DateTime.Now.Ticks);
        }

        /** Creates a new random number generator using a single int seed.
         * @param seed the initial seed (32 bits integer)
         */
        public MersenneTwister(int seed)
        {
            mt = new uint[N];
            SetSeed(seed);
        }

        /** Creates a new random number generator using an int array seed.
         * @param seed the initial seed (32 bits integers array), if null
         * the seed of the generator will be related to the current time
         */
        public MersenneTwister(int[] seed)
        {
            mt = new uint[N];
            SetSeed(seed);
        }

        /** Creates a new random number generator using a single long seed.
         * @param seed the initial seed (64 bits integer)
         */
        public MersenneTwister(long seed)
        {
            mt = new uint[N];
            SetSeed(seed);
        }

        /** Reinitialize the generator as if just built with the given int seed.
         * <p>The state of the generator is exactly the same as a new
         * generator built with the same seed.</p>
         * @param seed the initial seed (32 bits integer)
         */
        public override void SetSeed(int seed)
        {
            // we use a long masked by 0xffffffffL as a poor man unsigned int
            long longMT = seed;
            // NB: unlike original C code, we are working with java longs, the cast below makes masking unnecessary
            mt[0] = (uint)longMT;
            for (mti = 1; mti < N; ++mti)
            {
                // See Knuth TAOCP Vol2. 3rd Ed. P.106 for multiplier.
                // initializer from the 2002-01-09 C version by Makoto Matsumoto
                longMT = (1812433253L * (longMT ^ (longMT >> 30)) + mti) & 0xffffffffL;
                mt[mti] = (uint)longMT;
            }

            Clear(); // Clear normal deviate cache
        }

        /** Reinitialize the generator as if just built with the given int array seed.
         * <p>The state of the generator is exactly the same as a new
         * generator built with the same seed.</p>
         * @param seed the initial seed (32 bits integers array), if null
         * the seed of the generator will be the current system time plus the
         * system identity hash code of this instance
         */
        public override void SetSeed(int[] seed)
        {
            if (seed == null)
            {
                SetSeed(System.DateTime.Now.Ticks);
                return;
            }

            SetSeed(19650218);
            int i = 1;
            int j = 0;

            for (int k = Math.Max(N, seed.Length); k != 0; k--)
            {
                long l0 = (mt[i] & 0x7fffffffL) | (((int)mt[i] < 0) ? 0x80000000L : 0x0L);
                long l1 = (mt[i - 1] & 0x7fffffffL) | (((int)mt[i - 1] < 0) ? 0x80000000L : 0x0L);
                long l = (l0 ^ ((l1 ^ (l1 >> 30)) * 1664525L)) + seed[j] + j; // non linear
                mt[i] = (uint)(l & 0xffffffffL);
                i++; j++;
                if (i >= N)
                {
                    mt[0] = mt[N - 1];
                    i = 1;
                }
                if (j >= seed.Length)
                {
                    j = 0;
                }
            }

            for (int k = N - 1; k != 0; k--)
            {
                long l0 = (mt[i] & 0x7fffffffL) | (((int)mt[i] < 0) ? 0x80000000L : 0x0L);
                long l1 = (mt[i - 1] & 0x7fffffffL) | (((int)mt[i - 1] < 0) ? 0x80000000L : 0x0L);
                long l = (l0 ^ ((l1 ^ (l1 >> 30)) * 1566083941L)) - i; // non linear
                mt[i] = (uint)(l & 0xffffffffL);
                i++;
                if (i >= N)
                {
                    mt[0] = mt[N - 1];
                    i = 1;
                }
            }

            mt[0] = 0x80000000; // MSB is 1; assuring non-zero initial array

            Clear(); // Clear normal deviate cache

        }

        /** Reinitialize the generator as if just built with the given long seed.
         * <p>The state of the generator is exactly the same as a new
         * generator built with the same seed.</p>
         * @param seed the initial seed (64 bits integer)
         */
        public override void SetSeed(long seed)
        {
            SetSeed(new int[] { (int)(((ulong)seed) >> 32), (int)(seed & 0xffffffffL) });
        }

        /** Generate next pseudorandom number.
         * <p>This method is the core generation algorithm. It is used by all the
         * public generation methods for the various primitive types {@link
         * #NextBoolean()}, {@link #NextBytes(byte[])}, {@link #NextDouble()},
         * {@link #NextFloat()}, {@link #NextGaussian()}, {@link #NextInt()},
         * {@link #Next(int)} and {@link #NextLong()}.</p>
         * @param bits number of random bits to produce
         * @return random bits generated
         */
        protected override uint Next(int bits)
        {
            uint y;

            if (mti >= N)
            { // generate N words at one time
                uint mtNext = mt[0];
                for (int k = 0; k < N - M; ++k)
                {
                    uint mtCurr = mtNext;
                    mtNext = mt[k + 1];
                    y = (mtCurr & 0x80000000) | (mtNext & 0x7fffffff);
                    mt[k] = mt[k + M] ^ (y >> 1) ^ MAG01[y & 0x1];
                }
                for (int k = N - M; k < N - 1; ++k)
                {
                    uint mtCurr = mtNext;
                    mtNext = mt[k + 1];
                    y = (mtCurr & 0x80000000) | (mtNext & 0x7fffffff);
                    mt[k] = mt[k + (M - N)] ^ (y >> 1) ^ MAG01[y & 0x1];
                }
                y = (mtNext & 0x80000000) | (mt[0] & 0x7fffffff);
                mt[N - 1] = mt[M - 1] ^ (y >> 1) ^ MAG01[y & 0x1];

                mti = 0;
            }

            y = mt[mti++];

            // tempering
            y ^= y >> 11;
            y ^= (y << 7) & 0x9d2c5680;
            y ^= (y << 15) & 0xefc60000;
            y ^= y >> 18;

            return y >> (32 - bits);
        }
    }
}
