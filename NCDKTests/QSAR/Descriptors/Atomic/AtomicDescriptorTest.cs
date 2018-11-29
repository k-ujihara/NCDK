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
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Tools.Diff;
using System;
using System.Diagnostics;
using System.Linq;

namespace NCDK.QSAR.Descriptors.Atomic
{
    // @cdk.module test-qsaratomic
    public abstract class AtomicDescriptorTest<T> 
        : DescriptorTest<T> where T: IAtomicDescriptor
    {
        public AtomicDescriptorTest() { }

        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestCalculate_IAtomContainer()
        {
            var v = Descriptor.Calculate(Water.Atoms[1]);
            Assert.IsNotNull(v);
            Trace.Assert(v != null);
            if (v.Exception == null)
                Assert.AreNotEqual(0, v.Count, $"{typeof(T).FullName}: The descriptor did not calculate any value.");
        }

        /// <summary>
        /// Checks if the given labels are consistent.
        /// </summary>
        /// <exception cref="Exception">Passed on from calculate.</exception>
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestLabels()
        {
            var typeName = typeof(T).FullName;
            var v = Descriptor.Calculate(Water.Atoms[1]);
            Assert.IsNotNull(v);
            if (v.Exception == null)
            {
                var names = v.Keys;
                Assert.IsNotNull(names, $"{typeName}: The descriptor must return labels using the Names method.");
                Assert.AreNotEqual(0, names.Count(), $"{typeName}: At least one label must be given.");
                foreach (var name in names)
                {
                    Assert.IsNotNull(name, $"{typeName}: A descriptor label may not be null.");
                    Assert.AreNotEqual(0, name.Length, $"{typeName}: The label string must not be empty.");
                }
                Assert.IsNotNull(v.Values, $"{typeName}");
                Assert.AreEqual(names.Count(), v.Values.Count(), $"{typeName}: The number of labels must equals the number of values.");
            }
        }

        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestCalculate_NoModifications()
        {
            var atom = Water.Atoms[1];
            var clone = (IAtom)Water.Atoms[1].Clone();
            Descriptor.Calculate(atom);
            var diff = AtomDiff.Diff(clone, atom);
            Assert.AreEqual(0, diff.Length, $"The descriptor must not change the passed atom in any respect, but found this diff: {diff}");
        }
    }
}
