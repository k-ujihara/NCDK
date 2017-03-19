/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
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
using System.Linq;

namespace NCDK.Default
{
    /// <summary>
    /// Checks the functionality of the ChemModel class.
    ///
    // @cdk.module test-data
    ///
    // @see org.openscience.cdk.ChemModel
    /// </summary>
    [TestClass()]
    public class ChemModelTest : AbstractChemModelTest
    {
        public override IChemObject NewChemObject()
        {
            return new ChemModel();
        }

        [TestMethod()]
        public virtual void TestChemModel()
        {
            IChemModel chemModel = new ChemModel();
            Assert.IsNotNull(chemModel);
            Assert.IsTrue(chemModel.IsEmpty());

            IAtom atom = new Atom("N");
            IRing mol = new Ring();   // NCDK does not allow to add AtomContainer to RingSet
            IAtomContainerSet<IAtomContainer> mset = new AtomContainerSet<IAtomContainer>();
            mol.Atoms.Add(atom);
            mset.Add(mol);    
            chemModel.MoleculeSet = mset;
            Assert.IsFalse(chemModel.IsEmpty());
            mol.Atoms.Remove(atom);
            Assert.IsFalse(chemModel.IsEmpty());
            chemModel.MoleculeSet = null;
            Assert.IsTrue(chemModel.IsEmpty());

            IChemModel model1 = new ChemModel();
            mol.Atoms.Add(atom);
            IReaction react = new Reaction();
            react.Reactants.Add(mol);
            IReactionSet rset = new ReactionSet();
            rset.Add(react);
            model1.ReactionSet = rset;
            Assert.IsFalse(model1.IsEmpty());
            mol.Atoms.Remove(atom);
            Assert.IsFalse(model1.IsEmpty());
            model1.ReactionSet = null;
            Assert.IsTrue(model1.IsEmpty());

            IChemModel model2 = new ChemModel();
            mol.Atoms.Add(atom);
            IRingSet ringset = new RingSet();
            ringset.AddRange(mset.Cast<IRing>());  // NCDK does not allow to add AtomContainer to Ring directly
            model2.RingSet = ringset;
            Assert.IsFalse(model2.IsEmpty());
            mol.Atoms.Remove(atom);
            Assert.IsFalse(model2.IsEmpty());
            model2.RingSet = null;
            Assert.IsTrue(model2.IsEmpty());

            IChemModel model3 = new ChemModel();
            mol.Atoms.Add(atom);
            ICrystal cry = new Crystal(mol);
            model3.Crystal = cry;
            Assert.IsFalse(model3.IsEmpty());
            mol.Atoms.Remove(atom);
            Assert.IsFalse(model3.IsEmpty());
            model3.Crystal = null;
            Assert.IsTrue(model3.IsEmpty());
        }
    }
}
