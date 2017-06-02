/* Copyright (C) 2008  Egon Willighagen <egonw@users.sf.net>
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
using Moq;
using NCDK.Tools.Diff.Tree;

namespace NCDK.Tools.Diff
{
    // @cdk.module test-diff 
    public class AtomDiffTest : CDKTestCase
    {
        [TestMethod()]
        public void TestMatchAgainstItself()
        {
            var m_atom1 = new Mock<IAtom>(); IAtom atom1 = m_atom1.Object;
            string result = AtomDiff.Diff(atom1, atom1);
            AssertZeroLength(result);
        }

        [TestMethod()]
        public void TestDiff()
        {
            var m_atom1 = new Mock<IAtom>(); IAtom atom1 = m_atom1.Object;
            var m_atom2 = new Mock<IAtom>(); IAtom atom2 = m_atom2.Object;
            m_atom1.SetupGet(n => n.Symbol).Returns("H");
            m_atom2.SetupGet(n => n.Symbol).Returns("C");

            string result = AtomDiff.Diff(atom1, atom2);
            Assert.IsNotNull(result);
            Assert.AreNotSame(0, result.Length);
            AssertContains(result, "AtomDiff");
            AssertContains(result, "H/C");
        }

        [TestMethod()]
        public void TestDifference()
        {
            var m_atom1 = new Mock<IAtom>(); IAtom atom1 = m_atom1.Object;
            var m_atom2 = new Mock<IAtom>(); IAtom atom2 = m_atom2.Object;
            m_atom1.SetupGet(n => n.Symbol).Returns("H");
            m_atom2.SetupGet(n => n.Symbol).Returns("C");

            IDifference difference = AtomDiff.Difference(atom1, atom2);
            Assert.IsNotNull(difference);
        }

        //@Ignore("unit test did not test AtomDiff but rather the ability of AtomContainer"
        //        + "to be serialized. This is already tested in each respective domain module")
        public void TestDiffFromSerialized()
        {
            //        IAtom atom = new Atom("C");
            //
            //        File tmpFile = File.createTempFile("serialized", ".dat");
            //        tmpFile.deleteOnExit();
            //        String objFilename = tmpFile.getAbsolutePath();
            //
            //        FileOutputStream fout = new FileOutputStream(objFilename);
            //        ObjectOutputStream ostream = new ObjectOutputStream(fout);
            //        ostream.writeObject(atom);
            //
            //        ostream.close();
            //        fout.close();
            //
            //        // now read the serialized atom in
            //        FileInputStream fin = new FileInputStream(objFilename);
            //        ObjectInputStream istream  = new ObjectInputStream(fin);
            //        Object obj = istream.readObject();
            //
            //        Assert.assertTrue(obj instanceof IAtom);
            //
            //        IAtom newAtom = (IAtom) obj;
            //        String diff = AtomDiff.diff(atom, newAtom);
            //
            //        Assert.assertTrue("There were differences between original and deserialized version!", diff.equals(""));
        }
    }
}
