/*
 * Copyright (C) 2008 The Guava Authors
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF Any KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using static NCDK.Common.Base.Preconditions;

namespace NCDK.Common.Primitives
{
    public static class Ints
    {
        /// <summary>
        /// Compares the two specified <see cref="int"/> values. The sign of the value returned is the same as
        /// that of <c><paramref name="a"/>.CompareTo(<paramref name="b"/>)</c>.
        /// <para><b>Note for Java 7 and later:</b> this method should be treated as deprecated; use the
        /// equivalent <see cref="int.CompareTo(int)"/> method instead.</para>
        /// </summary>
        /// <param name="a">the first <see cref="int"/> to compare</param>
        /// <param name="b">the second <see cref="int"/> to compare</param>
        /// <returns>a negative value if <paramref name="a"/> is less than <paramref name="b"/>; a positive value if <paramref name="a"/> is greater than <paramref name="b"/>; or zero if they are equal</returns>
        public static int Compare(int a, int b)
        {
            return (a < b) ? -1 : ((a > b) ? 1 : 0);
        }

        /// <summary>
        /// Returns an array containing the same values as {@code array}, but
        /// guaranteed to be of a specified minimum length. If {@code array} already
        /// has a length of at least {@code minLength}, it is returned directly.
        /// Otherwise, a new array of size {@code minLength + padding} is returned,
        /// containing the values of {@code array}, and zeroes in the remaining places.
        /// </summary>
        /// <param name="array">the source array</param>
        /// <param name="minLength">the minimum length the returned array must guarantee</param>
        /// <param name="padding">an extra amount to "grow" the array by if growth is necessary</param>
        /// <returns>an array containing the values of {@code array}, with guaranteed minimum length {@code minLength}</returns>
        /// <exception cref="">if {@code minLength} or {@code padding} is negative</exception>
        public static int[] EnsureCapacity(
            int[] array, int minLength, int padding)
        {
            CheckArgument(minLength >= 0, "Invalid minLength: {0}", minLength);
            CheckArgument(padding >= 0, "Invalid padding: {0}", padding);
            return (array.Length < minLength)
                ? CopyOf(array, minLength + padding)
                : array;
        }

        private static int[] CopyOf(int[] original, int length)
        {
            int[] copy = new int[length];
            Array.Copy(original, 0, copy, 0, Math.Min(original.Length, length));
            return copy;
        }
    }
}
