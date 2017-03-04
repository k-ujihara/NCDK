/* Copyright (C) 2007  Miguel Rojasch <miguelrojasch@users.sf.net>
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
 *
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK
{
    /// <summary>
    /// Checks the functionality of {@link IReactionScheme} implementations.
    ///
    // @cdk.module test-interfaces
    /// </summary>
    [TestClass()]
    public abstract class AbstractReactionSchemeTest
           : AbstractReactionSetTest
    {
        [TestMethod()]
        public virtual void TestGetReactionSchemeCount()
        {
            IReactionScheme scheme = (IReactionScheme)NewChemObject();
            scheme.Add(scheme.Builder.CreateReactionScheme());
            Assert.AreEqual(1, scheme.Schemes.Count);
        }

        [TestMethod()]
        public override void TestGetReactionCount()
        {
            IReactionScheme scheme = (IReactionScheme)NewChemObject();
            scheme.Add(scheme.Builder.CreateReaction());
            scheme.Add(scheme.Builder.CreateReaction());
            Assert.AreEqual(2, scheme.Count);
        }

        [TestMethod()]
        public virtual void TestReactionSchemes()
        {
            IReactionScheme scheme = (IReactionScheme)NewChemObject();
            scheme.Add(scheme.Builder.CreateReactionScheme());
            scheme.Add(scheme.Builder.CreateReactionScheme());
            scheme.Add(scheme.Builder.CreateReactionScheme());

            Assert.AreEqual(3, scheme.Schemes.Count);
            int count = 0;
            foreach (var sch in scheme.Schemes)
            {
                sch.GetType();
                ++count;
            }
            Assert.AreEqual(3, count);
        }

        [TestMethod()]
        public override void TestReactions()
        {
            IReactionScheme scheme = (IReactionScheme)NewChemObject();
            scheme.Add(scheme.Builder.CreateReaction());
            scheme.Add(scheme.Builder.CreateReaction());
            scheme.Add(scheme.Builder.CreateReaction());

            Assert.AreEqual(3, scheme.Count);
            int count = 0;
            foreach (var reaction in scheme.Reactions)
            {
                ++count;
            }
            Assert.AreEqual(3, count);
        }

        [TestMethod()]
        public virtual void TestAdd_IReactionScheme()
        {
            IReactionScheme scheme = (IReactionScheme)NewChemObject();
            scheme.Add(scheme.Builder.CreateReactionScheme());
            scheme.Add(scheme.Builder.CreateReactionScheme());
            scheme.Add(scheme.Builder.CreateReactionScheme());

            IReactionScheme tested = scheme.Builder.CreateReactionScheme();
            Assert.AreEqual(0, tested.Schemes.Count);
            tested.Add(scheme);
            Assert.AreEqual(1, tested.Schemes.Count);
        }

        [TestMethod()]
        public virtual void TestAdd_IReaction()
        {
            IReactionScheme scheme = (IReactionScheme)NewChemObject();
            scheme.Add(scheme.Builder.CreateReactionScheme());
            scheme.Add(scheme.Builder.CreateReactionScheme());
            scheme.Add(scheme.Builder.CreateReactionScheme());

            IReactionScheme tested = scheme.Builder.CreateReactionScheme();
            Assert.AreEqual(0, tested.Schemes.Count);
            tested.Add(scheme);
            Assert.AreEqual(1, tested.Schemes.Count);
            Assert.AreEqual(0, tested.Count);
        }

        [TestMethod()]
        public override void TestClone()
        {
            IReactionScheme scheme = (IReactionScheme)NewChemObject();
            object clone = scheme.Clone();
            Assert.IsTrue(clone is IReactionScheme);
            Assert.AreNotSame(scheme, clone);
        }

        [TestMethod()]
        public virtual void TestRemoveReactionScheme_IReactionScheme()
        {
            IReactionScheme scheme = (IReactionScheme)NewChemObject();
            IReactionScheme scheme1 = (IReactionScheme)NewChemObject();
            IReactionScheme scheme2 = (IReactionScheme)NewChemObject();
            scheme.Add(scheme1);
            scheme.Add(scheme2);
            scheme.Remove(scheme1);
            Assert.AreEqual(1, scheme.Schemes.Count);
        }

        [TestMethod()]
        public virtual void TestRemoveAllReactionSchemes()
        {
            IReactionScheme scheme = (IReactionScheme)NewChemObject();
            IReactionScheme scheme1 = (IReactionScheme)NewChemObject();
            IReactionScheme scheme2 = (IReactionScheme)NewChemObject();
            scheme.Add(scheme1);
            scheme.Add(scheme2);

            Assert.AreEqual(2, scheme.Schemes.Count);
            scheme.Schemes.Clear();
            Assert.AreEqual(0, scheme.Schemes.Count);
        }
    }
}
