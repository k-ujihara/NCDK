/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace NCDK
{
    /// <summary>
    /// Checks the functionality of <see cref="IMapping"/> implementations.
    /// </summary>
    // @cdk.module test-interfaces
    public abstract class AbstractMappingTest
        : AbstractChemObjectTest
    {
        /// <summary>
        /// Method to test whether the class complies with RFC #9.
        /// </summary>
        public virtual void TestToString()
        {
            IMapping mapping = (IMapping)NewChemObject();
            string description = mapping.ToString();
            for (int i = 0; i < description.Length; i++)
            {
                Assert.IsTrue(description[i] != '\n');
                Assert.IsTrue(description[i] != '\r');
            }
        }

        public override void TestClone()
        {
            IMapping mapping = (IMapping)NewChemObject();
            object clone = mapping.Clone();
            Assert.IsTrue(clone is IMapping);
        }

        public virtual void TestGetChemObject_int()
        {
            IChemObject obj = NewChemObject();
            IAtom atom0 = obj.Builder.NewAtom();
            IAtom atom1 = obj.Builder.NewAtom();
            IMapping mapping = obj.Builder.NewMapping(atom0, atom1);
            Assert.AreEqual(atom0, mapping[0]);
            Assert.AreEqual(atom1, mapping[1]);
        }

        public virtual void TestRelatedChemObjects()
        {
            IChemObject obj = NewChemObject();
            IAtom atom0 = obj.Builder.NewAtom();
            IAtom atom1 = obj.Builder.NewAtom();
            IMapping mapping = obj.Builder.NewMapping(atom0, atom1);

            IEnumerator<IChemObject> iter = mapping.GetRelatedChemObjects().GetEnumerator();
            Assert.IsTrue(iter.MoveNext());
            Assert.AreEqual(atom0, (IAtom)iter.Current);
            Assert.IsTrue(iter.MoveNext());
            Assert.AreEqual(atom1, (IAtom)iter.Current);
            Assert.IsFalse(iter.MoveNext());
        }

        public virtual void TestClone_ChemObject()
        {
            IMapping mapping = (IMapping)NewChemObject();

            IMapping clone = (IMapping)mapping.Clone();
            //IChemObject[] map = mapping.GetRelatedChemObjects();
            //IChemObject[] mapClone = clone.GetRelatedChemObjects();
            //AreEqual(map.Length, mapClone.Length);
            for (int f = 0; f < 2; f++)
            {
                for (int g = 0; g < 2; g++)
                {
                    Assert.IsNotNull(mapping[f]);
                    Assert.IsNotNull(clone[g]);
                    Assert.AreNotSame(mapping[f], clone[g]);
                }
            }
        }
    }
}
