/* Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace NCDK
{
    /// <summary>
    /// TestCase for <see cref="IReaction"/> implementations.
    /// </summary>
    // @cdk.module test-interfaces
    [TestClass()]
    public abstract class AbstractReactionTest
        : AbstractChemObjectTest
    {
        [TestMethod()]
        public virtual void TestGetReactantCount()
        {
            IReaction reaction = (IReaction)NewChemObject();
            Assert.AreEqual(0, reaction.Reactants.Count);
            reaction.Reactants.Add(reaction.Builder.NewAtomContainer());
            Assert.AreEqual(1, reaction.Reactants.Count);
        }

        [TestMethod()]
        public virtual void TestGetProductCount()
        {
            IReaction reaction = (IReaction)NewChemObject();
            Assert.AreEqual(0, reaction.Products.Count);
            reaction.Products.Add(reaction.Builder.NewAtomContainer());
            Assert.AreEqual(1, reaction.Products.Count);
        }

        [TestMethod()]
        public virtual void TestAddReactant_IAtomContainer()
        {
            IReaction reaction = (IReaction)NewChemObject();
            IAtomContainer sodiumhydroxide = reaction.Builder.NewAtomContainer();
            IAtomContainer aceticAcid = reaction.Builder.NewAtomContainer();
            IAtomContainer water = reaction.Builder.NewAtomContainer();
            IAtomContainer acetate = reaction.Builder.NewAtomContainer();
            reaction.Reactants.Add(sodiumhydroxide);
            reaction.Reactants.Add(aceticAcid);
            reaction.Reactants.Add(water);
            Assert.AreEqual(3, reaction.Reactants.Count);
            // next one should trigger a growArray, if the grow
            // size is still 3.
            reaction.Reactants.Add(acetate);
            Assert.AreEqual(4, reaction.Reactants.Count);

            Assert.AreEqual(1.0, reaction.Reactants.GetMultiplier(aceticAcid).Value, 0.00001);
        }

        [TestMethod()]
        public virtual void TestSetReactants_IAtomContainerSet()
        {
            IReaction reaction = (IReaction)NewChemObject();
            IAtomContainer sodiumhydroxide = reaction.Builder.NewAtomContainer();
            IAtomContainer aceticAcid = reaction.Builder.NewAtomContainer();
            IAtomContainer water = reaction.Builder.NewAtomContainer();
            IChemObjectSet<IAtomContainer> reactants = reaction.Builder.NewAtomContainerSet();
            reactants.Add(sodiumhydroxide);
            reactants.Add(aceticAcid);
            reactants.Add(water);
            reaction.Reactants.AddRange(reactants);
            Assert.AreEqual(3, reaction.Reactants.Count);

            Assert.AreEqual(1.0, reaction.Reactants.GetMultiplier(aceticAcid).Value, 0.00001);
        }

        [TestMethod()]
        public virtual void TestAddReactant_IAtomContainer_Double()
        {
            IReaction reaction = (IReaction)NewChemObject();
            IAtomContainer proton = reaction.Builder.NewAtomContainer();
            IAtomContainer sulfate = reaction.Builder.NewAtomContainer();
            reaction.Reactants.Add(proton, 2d);
            reaction.Reactants.Add(sulfate, 1d);
            Assert.AreEqual(2.0, reaction.Reactants.GetMultiplier(proton).Value, 0.00001);
            Assert.AreEqual(1.0, reaction.Reactants.GetMultiplier(sulfate).Value, 0.00001);
        }

        [TestMethod()]
        public virtual void TestAddProduct_IAtomContainer()
        {
            IReaction reaction = (IReaction)NewChemObject();
            IAtomContainer sodiumhydroxide = reaction.Builder.NewAtomContainer();
            IAtomContainer aceticAcid = reaction.Builder.NewAtomContainer();
            IAtomContainer water = reaction.Builder.NewAtomContainer();
            IAtomContainer acetate = reaction.Builder.NewAtomContainer();
            reaction.Products.Add(sodiumhydroxide);
            reaction.Products.Add(aceticAcid);
            reaction.Products.Add(water);
            Assert.AreEqual(3, reaction.Products.Count);
            // next one should trigger a growArray, if the grow
            // size is still 3.
            reaction.Products.Add(acetate);
            Assert.AreEqual(4, reaction.Products.Count);

            Assert.AreEqual(1.0, reaction.Products.GetMultiplier(aceticAcid).Value, 0.00001);
        }

        [TestMethod()]
        public virtual void TestSetProducts_IAtomContainerSet()
        {
            IReaction reaction = (IReaction)NewChemObject();
            IAtomContainer sodiumhydroxide = reaction.Builder.NewAtomContainer();
            IAtomContainer aceticAcid = reaction.Builder.NewAtomContainer();
            IAtomContainer water = reaction.Builder.NewAtomContainer();
            IChemObjectSet<IAtomContainer> products = reaction.Builder.NewAtomContainerSet();
            products.Add(sodiumhydroxide);
            products.Add(aceticAcid);
            products.Add(water);
            reaction.Products.AddRange(products);
            Assert.AreEqual(3, reaction.Products.Count);

            Assert.AreEqual(1.0, reaction.Products.GetMultiplier(aceticAcid).Value, 0.00001);
        }

        [TestMethod()]
        public virtual void TestAddProduct_IAtomContainer_Double()
        {
            IReaction reaction = (IReaction)NewChemObject();
            IAtomContainer proton = reaction.Builder.NewAtomContainer();
            IAtomContainer sulfate = reaction.Builder.NewAtomContainer();
            reaction.Products.Add(proton, 2.0);
            reaction.Products.Add(sulfate, 1.0);
            Assert.AreEqual(2.0, reaction.Products.GetMultiplier(proton).Value, 0.00001);
            Assert.AreEqual(1.0, reaction.Products.GetMultiplier(sulfate).Value, 0.00001);
        }

        [TestMethod()]
        public virtual void TestAddAgent_IAtomContainer()
        {
            IReaction reaction = (IReaction)NewChemObject();
            IAtomContainer proton = reaction.Builder.NewAtomContainer();
            reaction.Agents.Add(proton);
            Assert.AreEqual(1, reaction.Agents.Count);
        }

        [TestMethod()]
        public virtual void TestGetReactantCoefficient_IAtomContainer()
        {
            IReaction reaction = (IReaction)NewChemObject();
            IAtomContainer proton = reaction.Builder.NewAtomContainer();
            reaction.Reactants.Add(proton, 2.0);
            Assert.AreEqual(2.0, reaction.Reactants.GetMultiplier(proton).Value, 0.00001);

            Assert.AreEqual(-1.0,
                    reaction.Reactants.GetMultiplier(reaction.Builder.NewAtomContainer()).Value, 0.00001);
        }

        [TestMethod()]
        public virtual void TestGetProductCoefficient_IAtomContainer()
        {
            IReaction reaction = (IReaction)NewChemObject();
            IAtomContainer proton = reaction.Builder.NewAtomContainer();
            reaction.Products.Add(proton, 2.0);
            Assert.AreEqual(2.0, reaction.Products.GetMultiplier(proton).Value, 0.00001);

            Assert.AreEqual(-1.0,
                    reaction.Products.GetMultiplier(reaction.Builder.NewAtomContainer()).Value, 0.00001);
        }

        [TestMethod()]
        public virtual void TestSetReactantCoefficient_IAtomContainer_Double()
        {
            IReaction reaction = (IReaction)NewChemObject();
            IAtomContainer proton = reaction.Builder.NewAtomContainer();
            reaction.Reactants.Add(proton, 2.0);
            reaction.Reactants.SetMultiplier(proton, 3.0);
            Assert.AreEqual(3.0, reaction.Reactants.GetMultiplier(proton).Value, 0.00001);
        }

        [TestMethod()]
        public virtual void TestSetProductCoefficient_IAtomContainer_Double()
        {
            IReaction reaction = (IReaction)NewChemObject();
            IAtomContainer proton = reaction.Builder.NewAtomContainer();
            reaction.Products.Add(proton, 2.0);
            reaction.Products.SetMultiplier(proton, 1.0);
            Assert.AreEqual(1.0, reaction.Products.GetMultiplier(proton).Value, 0.00001);
        }

        [TestMethod()]
        public virtual void TestGetReactantCoefficients()
        {
            IReaction reaction = (IReaction)NewChemObject();
            IAtomContainer ed1 = reaction.Builder.NewAtomContainer();
            IAtomContainer ed2 = reaction.Builder.NewAtomContainer();
            reaction.Reactants.Add(ed1, 2d);
            reaction.Reactants.Add(ed2, 3d);
            var ec = reaction.Reactants.GetMultipliers();
            Assert.AreEqual(2.0, ec.Count, 0.00001);
            Assert.AreEqual(reaction.Reactants.GetMultiplier(ed1).Value, ec[0].Value, 0.00001);
            Assert.AreEqual(3.0, ec[1].Value, 0.00001);
        }

        [TestMethod()]
        public virtual void TestGetProductCoefficients()
        {
            IReaction reaction = (IReaction)NewChemObject();
            IAtomContainer pr1 = reaction.Builder.NewAtomContainer();
            IAtomContainer pr2 = reaction.Builder.NewAtomContainer();
            reaction.Products.Add(pr1, 1d);
            reaction.Products.Add(pr2, 2d);
            var pc = reaction.Products.GetMultipliers();
            Assert.AreEqual(2.0, pc.Count, 0.00001);
            Assert.AreEqual(reaction.Products.GetMultiplier(pr1).Value, pc[0].Value, 0.00001);
            Assert.AreEqual(2.0, pc[1].Value, 0.00001);
        }

        [TestMethod()]
        public virtual void TestSetReactantCoefficients_arrayDouble()
        {
            IReaction reaction = (IReaction)NewChemObject();
            IAtomContainer ed1 = reaction.Builder.NewAtomContainer();
            IAtomContainer ed2 = reaction.Builder.NewAtomContainer();
            reaction.Reactants.Add(ed1, 2d);
            reaction.Reactants.Add(ed2, 3d);
            double?[] ec = { 1.0, 2.0 };
            bool coeffSet = reaction.Reactants.SetMultipliers(ec);
            Assert.IsTrue(coeffSet);
            Assert.AreEqual(1.0, reaction.Reactants.GetMultiplier(ed1).Value, 0.00001);
            Assert.AreEqual(2.0, reaction.Reactants.GetMultiplier(ed2).Value, 0.00001);
            double?[] ecFalse = { 1.0 };
            Assert.IsFalse(reaction.Reactants.SetMultipliers(ecFalse));
        }

        [TestMethod()]
        public virtual void TestSetProductCoefficients_arrayDouble()
        {
            IReaction reaction = (IReaction)NewChemObject();
            IAtomContainer pr1 = reaction.Builder.NewAtomContainer();
            reaction.Products.Add(pr1, 1d);
            double?[] pc = { 2.0 };
            bool coeffSet = reaction.Products.SetMultipliers(pc);
            Assert.IsTrue(coeffSet);
            Assert.AreEqual(2.0, reaction.Products.GetMultiplier(pr1).Value, 0.00001);
            double?[] pcFalse = { 1.0, 2.0 };
            Assert.IsFalse(reaction.Products.SetMultipliers(pcFalse));
        }

        [TestMethod()]
        public virtual void TestGetReactants()
        {
            IReaction reaction = (IReaction)NewChemObject();
            IAtomContainer sodiumhydroxide = reaction.Builder.NewAtomContainer();
            IAtomContainer aceticAcid = reaction.Builder.NewAtomContainer();
            IAtomContainer water = reaction.Builder.NewAtomContainer();
            reaction.Reactants.Add(sodiumhydroxide);
            reaction.Reactants.Add(aceticAcid);
            reaction.Reactants.Add(water);
            Assert.AreEqual(3, reaction.Reactants.Count);
        }

        [TestMethod()]
        public virtual void TestGetProducts()
        {
            IReaction reaction = (IReaction)NewChemObject();
            IAtomContainer sodiumhydroxide = reaction.Builder.NewAtomContainer();
            IAtomContainer aceticAcid = reaction.Builder.NewAtomContainer();
            IAtomContainer water = reaction.Builder.NewAtomContainer();
            reaction.Products.Add(sodiumhydroxide);
            reaction.Products.Add(aceticAcid);
            reaction.Products.Add(water);
            Assert.AreEqual(3, reaction.Products.Count);
        }

        [TestMethod()]
        public virtual void TestGetAgents()
        {
            IReaction reaction = (IReaction)NewChemObject();
            IAtomContainer water = reaction.Builder.NewAtomContainer();
            reaction.Agents.Add(water);
            Assert.AreEqual(1, reaction.Agents.Count);
        }

        [TestMethod()]
        public virtual void TestSetDirection_IReaction_Direction()
        {
            IReaction reaction = (IReaction)NewChemObject();
            ReactionDirection direction = ReactionDirection.Bidirectional;
            reaction.Direction = direction;
            Assert.AreEqual(direction, reaction.Direction);
        }

        [TestMethod()]
        public virtual void TestGetDirection()
        {
            IReaction reaction = (IReaction)NewChemObject();
            Assert.AreEqual(ReactionDirection.Forward, reaction.Direction);
        }

        /// <summary>
        /// Method to test whether the class complies with RFC #9.
        /// </summary>
        [TestMethod()]
        public virtual void TestToString()
        {
            IReaction reaction = (IReaction)NewChemObject();
            string description = reaction.ToString();
            for (int i = 0; i < description.Length; i++)
            {
                Assert.IsTrue(description[i] != '\n');
                Assert.IsTrue(description[i] != '\r');
            }
        }

        [TestMethod()]
        public override void TestClone()
        {
            IReaction reaction = (IReaction)NewChemObject();
            object clone = reaction.Clone();
            Assert.IsNotNull(clone);
            Assert.IsTrue(clone is IReaction);
        }

        [TestMethod()]
        public virtual void TestClone_Mapping()
        {
            IReaction reaction = (IReaction)NewChemObject();
            IMapping mapping = reaction.Builder.NewMapping(
                    reaction.Builder.NewAtom("C"),
                    reaction.Builder.NewAtom("C"));
            reaction.Mappings.Add(mapping);
            IReaction clonedReaction = (IReaction)reaction.Clone();
            IEnumerator<IMapping> mappings = reaction.Mappings.GetEnumerator();
            IEnumerator<IMapping> clonedMappings = clonedReaction.Mappings.GetEnumerator();
            Assert.IsNotNull(mappings);
            Assert.IsTrue(mappings.MoveNext());
            Assert.IsNotNull(clonedMappings);
            Assert.IsTrue(clonedMappings.MoveNext());
        }

        [TestMethod()]
        public virtual void TestAddMapping_IMapping()
        {
            IReaction reaction = (IReaction)NewChemObject();
            IMapping mapping = reaction.Builder.NewMapping(reaction.Builder.NewAtom("C"),
                    reaction.Builder.NewAtom("C"));
            reaction.Mappings.Add(mapping);
            IEnumerator<IMapping> mappings = reaction.Mappings.GetEnumerator();
            Assert.IsNotNull(mappings);
            Assert.IsTrue(mappings.MoveNext());
            Assert.AreEqual(mapping, (IMapping)mappings.Current);
        }

        [TestMethod()]
        public virtual void TestRemoveMapping_int()
        {
            IReaction reaction = (IReaction)NewChemObject();
            IMapping mapping = reaction.Builder.NewMapping(reaction.Builder.NewAtom("C"),
                    reaction.Builder.NewAtom("C"));
            reaction.Mappings.Add(mapping);
            Assert.AreEqual(1, reaction.Mappings.Count);
            reaction.Mappings.RemoveAt(0);
            Assert.AreEqual(0, reaction.Mappings.Count);
        }

        [TestMethod()]
        public virtual void TestGetMapping_int()
        {
            IReaction reaction = (IReaction)NewChemObject();
            IMapping mapping = reaction.Builder.NewMapping(reaction.Builder.NewAtom("C"),
                    reaction.Builder.NewAtom("C"));
            reaction.Mappings.Add(mapping);
            IMapping gotIt = reaction.Mappings[0];
            Assert.AreEqual(mapping, gotIt);
        }

        [TestMethod()]
        public virtual void TestGetMappingCount()
        {
            IReaction reaction = (IReaction)NewChemObject();
            IMapping mapping = reaction.Builder.NewMapping(reaction.Builder.NewAtom("C"),
                    reaction.Builder.NewAtom("C"));
            reaction.Mappings.Add(mapping);
            Assert.AreEqual(1, reaction.Mappings.Count);
        }

        [TestMethod()]
        public virtual void TestMappings()
        {
            IReaction reaction = (IReaction)NewChemObject();
            IMapping mapping = reaction.Builder.NewMapping(reaction.Builder.NewAtom("C"),
                    reaction.Builder.NewAtom("C"));
            reaction.Mappings.Add(mapping);
            Assert.AreEqual(1, reaction.Mappings.Count);
        }
    }
}