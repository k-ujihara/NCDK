/* Copyright (C) 2011  Jonathan Alvarsson <jonalv@users.sf.net>
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
using System;

namespace NCDK.Fingerprints
{
    [TestClass()]
    public abstract class AbstractBitFingerprintTest : CDKTestCase
    {
        protected IBitFingerprint bitsetFP;
        private Type C; // : IBitFingerprint

        public AbstractBitFingerprintTest(Type C)
        {
            this.C = C;
            bitsetFP = (IBitFingerprint)C.GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
        }

        [TestMethod()]
        public void TestCreate()
        {
            Assert.IsFalse(bitsetFP[0]);
        }

        [TestMethod()]
        public void TestGetAndSet()
        {
            TestCreate();
            bitsetFP[1] = true;
            Assert.IsTrue(bitsetFP[1]);
            Assert.IsFalse(bitsetFP[2]);
            bitsetFP[3] = true;
            Assert.IsTrue(bitsetFP[3]);
        }

        private IBitFingerprint CreateFP2()
        {
            IBitFingerprint fp = (IBitFingerprint)C.GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
            fp[2] = true;
            fp[3] = true;
            return fp;
        }

        [TestMethod()]
        public void TestAnd()
        {
            TestGetAndSet();
            bitsetFP.And(CreateFP2());
            Assert.IsFalse(bitsetFP[0]);
            Assert.IsFalse(bitsetFP[1]);
            Assert.IsFalse(bitsetFP[2]);
            Assert.IsTrue(bitsetFP[3]);
        }

        [TestMethod()]
        public void TestOr()
        {
            TestGetAndSet();
            bitsetFP.Or(CreateFP2());
            Assert.IsFalse(bitsetFP[0]);
            Assert.IsTrue(bitsetFP[1]);
            Assert.IsTrue(bitsetFP[2]);
            Assert.IsTrue(bitsetFP[3]);
        }

        [TestMethod()]
        public void TestEquals()
        {
            IBitFingerprint fp1 = (IBitFingerprint)C.GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
            IBitFingerprint fp2 = (IBitFingerprint)C.GetConstructor(Type.EmptyTypes).Invoke(new object[] { });

            foreach (var fp in new IBitFingerprint[] { fp1, fp2 })
            {
                fp[0] = true;
                fp[1] = false;
                fp[2] = true;
            }
            Assert.IsTrue(fp1.Equals(fp2), "identical fingerprints should be equal");
            Assert.IsFalse(bitsetFP.Equals(fp1), "different fingerprints should not be equal");
            Assert.IsTrue(fp1.GetHashCode() == fp2.GetHashCode(), "equal fingerprints must have same hashcode");
        }
    }
}
