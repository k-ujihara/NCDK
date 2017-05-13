/* Copyright (C) 2011  Egon Willighagen <egonw@users.sourceforge.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License as published by the Free
 * Software Foundation; either version 2.1 of the License, or (at your option)
 * any later version. All we ask is that proper credit is given for our work,
 * which includes - but is not limited to - adding the above copyright notice to
 * the beginning of your source code files, and to any copyright notice that you
 * may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more
 * details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation, Inc.,
 * 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using System;

namespace NCDK.Renderers
{
    // @cdk.module test-renderbasic
    [TestClass()]
    public class BoundsCalculatorTest
    {
        /// <summary>
        /// Test if we get the expected <see cref="ArgumentException"/> when we pass
        /// an <see cref="IAtomContainer"/> without 2D coordinates.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCalculateBounds_IAtomContainer_SingleAtom()
        {
            IAtomContainer container = new AtomContainer();
            container.Atoms.Add(container.Builder.CreateAtom("C"));
            BoundsCalculator.CalculateBounds(container);
        }

        /// <summary>
        /// Test if we get the expected <see cref="ArgumentException"/> when we pass
        /// an <see cref="IAtomContainer"/> without 2D coordinates.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCalculateBounds_IAtomContainer()
        {
            IAtomContainer container = new AtomContainer();
            container.Atoms.Add(container.Builder.CreateAtom("C"));
            container.Atoms.Add(container.Builder.CreateAtom("C"));
            BoundsCalculator.CalculateBounds(container);
        }

        /// <summary>
        /// Test if we get the expected <see cref="ArgumentException"/> when we pass
        /// an <see cref="IAtomContainer"/> without 2D coordinates.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCalculateBounds_IAtomContainerSet_SingleAtom()
        {
            IAtomContainer container = new AtomContainer();
            container.Atoms.Add(container.Builder.CreateAtom("C"));
            var set = container.Builder.CreateAtomContainerSet();
            set.Add(container);
            BoundsCalculator.CalculateBounds(set);
        }

        /// <summary>
        /// Test if we get the expected <see cref="ArgumentException"/> when we pass
        /// an <see cref="IAtomContainer"/> without 2D coordinates.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCalculateBounds_IAtomContainerSet()
        {
            IAtomContainer container = new AtomContainer();
            container.Atoms.Add(container.Builder.CreateAtom("C"));
            container.Atoms.Add(container.Builder.CreateAtom("C"));
            var set = container.Builder.CreateAtomContainerSet();
            set.Add(container);
            BoundsCalculator.CalculateBounds(set);
        }

        /// <summary>
        /// Test if we get the expected <see cref="ArgumentException"/> when we pass
        /// an <see cref="IAtomContainer"/> without 2D coordinates.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCalculateBounds_IReactionSet_SingleAtom()
        {
            IAtomContainer container = new AtomContainer();
            container.Atoms.Add(container.Builder.CreateAtom("C"));
            IReaction reaction = container.Builder.CreateReaction();
            reaction.Reactants.Add(container.Builder.CreateAtomContainer(container));
            IReactionSet set = container.Builder.CreateReactionSet();
            set.Add(reaction);
            BoundsCalculator.CalculateBounds(set);
        }

        /// <summary>
        /// Test if we get the expected <see cref="ArgumentException"/> when we pass
        /// an <see cref="IAtomContainer"/> without 2D coordinates.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCalculateBounds_IReactionSet()
        {
            IAtomContainer container = new AtomContainer();
            container.Atoms.Add(container.Builder.CreateAtom("C"));
            container.Atoms.Add(container.Builder.CreateAtom("C"));
            IReaction reaction = container.Builder.CreateReaction();
            reaction.Reactants.Add(container.Builder.CreateAtomContainer(container));
            IReactionSet set = container.Builder.CreateReactionSet();
            set.Add(reaction);
            BoundsCalculator.CalculateBounds(set);
        }

        /// <summary>
        /// Test if we get the expected <see cref="ArgumentException"/> when we pass
        /// an <see cref="IAtomContainer"/> without 2D coordinates.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCalculateBounds_IChemModel_SingleAtom()
        {
            IAtomContainer container = new AtomContainer();
            container.Atoms.Add(container.Builder.CreateAtom("C"));
            var set = container.Builder.CreateAtomContainerSet();
            set.Add(container);
            IChemModel model = container.Builder.CreateChemModel();
            model.MoleculeSet = set;
            BoundsCalculator.CalculateBounds(model);
        }

        /// <summary>
        /// Test if we get the expected <see cref="ArgumentException"/> when we pass
        /// an <see cref="IAtomContainer"/> without 2D coordinates.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCalculateBounds_IChemModel()
        {
            IAtomContainer container = new AtomContainer();
            container.Atoms.Add(container.Builder.CreateAtom("C"));
            container.Atoms.Add(container.Builder.CreateAtom("C"));
            var set = container.Builder.CreateAtomContainerSet();
            set.Add(container);
            IChemModel model = container.Builder.CreateChemModel();
            model.MoleculeSet = set;
            BoundsCalculator.CalculateBounds(model);
        }

        /// <summary>
        /// Test if we get the expected <see cref="ArgumentException"/> when we pass
        /// an <see cref="IAtomContainer"/> without 2D coordinates.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCalculateBounds_IReaction_SingleAtom()
        {
            IAtomContainer container = new AtomContainer();
            container.Atoms.Add(container.Builder.CreateAtom("C"));
            IReaction reaction = container.Builder.CreateReaction();
            reaction.Reactants.Add(container.Builder.CreateAtomContainer(container));
            BoundsCalculator.CalculateBounds(reaction);
        }

        /// <summary>
        /// Test if we get the expected <see cref="ArgumentException"/> when we pass
        /// an <see cref="IAtomContainer"/> without 2D coordinates.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCalculateBounds_IReaction()
        {
            IAtomContainer container = new AtomContainer();
            container.Atoms.Add(container.Builder.CreateAtom("C"));
            container.Atoms.Add(container.Builder.CreateAtom("C"));
            IReaction reaction = container.Builder.CreateReaction();
            reaction.Reactants.Add(container.Builder.CreateAtomContainer(container));
            BoundsCalculator.CalculateBounds(reaction);
        }
    }
}
