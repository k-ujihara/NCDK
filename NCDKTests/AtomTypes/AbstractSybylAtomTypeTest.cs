/* Copyright (C) 2007-2008  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Config;
using System.Collections.Generic;

namespace NCDK.AtomTypes
{
    /**
     * Helper class that all atom type matcher test classes must implement.
     * It keeps track of the atom types which have been tested, to ensure
     * that all atom types are tested.
     *
     * @cdk.module test-core
     * @cdk.bug    1890702
     */
    [TestClass()]
    abstract public class AbstractSybylAtomTypeTest : AbstractAtomTypeTest
    {
        private const string ATOMTYPE_LIST = "sybyl-atom-types.owl";

        protected static readonly AtomTypeFactory factory = AtomTypeFactory
                                                                     .GetInstance("NCDK.Dict.Data."
                                                                             + ATOMTYPE_LIST,
                                                                             Silent.ChemObjectBuilder.Instance);


        public override string AtomTypeListName => ATOMTYPE_LIST;

        public override AtomTypeFactory GetFactory()
        {
            return factory;
        }

        public override IAtomTypeMatcher GetAtomTypeMatcher(IChemObjectBuilder builder)
        {
            return SybylAtomTypeMatcher.GetInstance(builder);
        }

        public override void AssertAtomTypes(IDictionary<string, int> testedAtomTypes, string[] expectedTypes, IAtomContainer mol)
        {
            Assert.AreEqual(expectedTypes.Length, mol.Atoms.Count,
                "The number of expected atom types is unequal to the number of atoms");
            IAtomTypeMatcher atm = GetAtomTypeMatcher(mol.Builder);
            for (int i = 0; i < expectedTypes.Length; i++)
            {
                IAtom testedAtom = mol.Atoms[i];
                IAtomType foundType = atm.FindMatchingAtomType(mol, testedAtom);
                AssertAtomType(testedAtomTypes, "Incorrect perception for atom " + i, expectedTypes[i], foundType);
            }
        }
    }
}
