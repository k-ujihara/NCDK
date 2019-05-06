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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Common.Base;
using NCDK.Config;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.AtomTypes
{
    /// <summary>
    /// Helper class that all atom type matcher test classes must implement.
    /// It keeps track of the atom types which have been tested, to ensure
    /// that all atom types are tested.
    /// </summary>
    // @cdk.module test-core
    // @cdk.bug    1890702
    [TestClass()]
    abstract public class AbstractAtomTypeTest : CDKTestCase, IAtomTypeTest
    {
        public abstract string AtomTypeListName { get; }
        public abstract AtomTypeFactory GetFactory();
        public abstract IAtomTypeMatcher GetAtomTypeMatcher(IChemObjectBuilder builder);

        /// <summary>
        /// Helper method to test if atom types are correctly perceived. Meanwhile, it maintains a list
        /// of atom types that have been tested so far, which allows testing afterwards that all atom
        /// types are at least tested once.
        /// </summary>
        /// <param name="testedAtomTypes">List of atom types tested so far.</param>
        /// <param name="expectedTypes">Expected atom types for the atoms given in <paramref name="mol"/>.</param>
        /// <param name="mol">The <see cref="IAtomContainer"/> with <see cref="IAtom"/>s for which atom types should be perceived.</param>
        /// <exception cref="System.Exception">Thrown if something went wrong during the atom type perception.</exception>
        public virtual void AssertAtomTypes(IDictionary<string, int> testedAtomTypes, string[] expectedTypes, IAtomContainer mol)
        {
            Assert.AreEqual(expectedTypes.Length, mol.Atoms.Count, "The number of expected atom types is unequal to the number of atoms");
            var atm = GetAtomTypeMatcher(mol.Builder);
            for (int i = 0; i < expectedTypes.Length; i++)
            {
                var testedAtom = mol.Atoms[i];
                var foundType = atm.FindMatchingAtomType(mol, testedAtom);
                AssertAtomType(testedAtomTypes, "Incorrect perception for atom " + i, expectedTypes[i], foundType);
                AssertConsistentProperties(mol, testedAtom, foundType);
                // test for bug #1890702: configure, and then make sure the same atom type is perceived
                AtomTypeManipulator.Configure(testedAtom, foundType);
                var secondType = atm.FindMatchingAtomType(mol, testedAtom);
                AssertAtomType(
                    testedAtomTypes,
                    $"Incorrect perception *after* assigning atom type properties for atom {i}", 
                    expectedTypes[i],
                    secondType);
            }
        }

        public void AssertAtomTypeNames(IDictionary<string, int> testedAtomTypes, string[] expectedTypes, IAtomContainer mol)
        {
            Assert.AreEqual(
                    expectedTypes.Length, mol.Atoms.Count,
                    "The number of expected atom types is unequal to the number of atoms");
            IAtomTypeMatcher atm = GetAtomTypeMatcher(mol.Builder);
            for (int i = 0; i < expectedTypes.Length; i++)
            {
                IAtom testedAtom = mol.Atoms[i];
                IAtomType foundType = atm.FindMatchingAtomType(mol, testedAtom);
                AssertAtomType(testedAtomTypes, "Incorrect perception for atom " + i, expectedTypes[i], foundType);
            }
        }

        /// <summary>
        /// Method that tests if the matched <see cref="IAtomType"/> and the <see cref="IAtom"/> are
        /// consistent. For example, it tests if hybridization states and formal charges are equal.
        ///
        // @cdk.bug 1897589
        /// </summary>
        private void AssertConsistentProperties(IAtomContainer mol, IAtom atom, IAtomType matched)
        {
            // X has no properties; nothing to match
            if (string.Equals("X", matched.AtomTypeName, StringComparison.Ordinal))
            {
                return;
            }

            if (!atom.Hybridization.IsUnset() && !matched.Hybridization.IsUnset())
            {
                Assert.AreEqual(atom.Hybridization, matched.Hybridization, "Hybridization does not match");
            }
            if (atom.FormalCharge != null && matched.FormalCharge != null)
            {
                Assert.AreEqual(atom.FormalCharge, matched.FormalCharge, "Formal charge does not match");
            }
            var connections = mol.GetConnectedBonds(atom);
            int connectionCount = connections.Count();
            if (matched.FormalNeighbourCount != null)
            {
                Assert.IsFalse(connectionCount > matched.FormalNeighbourCount, "Number of neighbors is too high");
            }
            if (!matched.MaxBondOrder.IsUnset())
            {
                BondOrder expectedMax = matched.MaxBondOrder;
                foreach (var bond in connections)
                {
                    BondOrder order = bond.Order;
                    if (!order.IsUnset())
                    {
                        if (BondManipulator.IsHigherOrder(order, expectedMax))
                        {
                            Assert.Fail("At least one bond order exceeds the maximum for the atom type");
                        }
                    }
                    else if (bond.IsSingleOrDouble)
                    {
                        if (expectedMax != BondOrder.Single && expectedMax != BondOrder.Double)
                        {
                            Assert.Fail("A single or double flagged bond does not match the bond order of the atom type");
                        }
                    }
                }
            }
        }

        public void AssertAtomType(IDictionary<string, int> testedAtomTypes, string expectedID, IAtomType foundAtomType)
        {
            this.AssertAtomType(testedAtomTypes, "", expectedID, foundAtomType);
        }

        public void AssertAtomType(IDictionary<string, int> testedAtomTypes, string error, string expectedID,
                IAtomType foundAtomType)
        {
            AddTestedAtomType(testedAtomTypes, expectedID);

            Assert.IsNotNull(foundAtomType, "No atom type was recognized, but expected: " + expectedID);
            Assert.AreEqual(expectedID, foundAtomType.AtomTypeName, error);
        }

        private void AddTestedAtomType(IDictionary<string, int> testedAtomTypes, string expectedID)
        {
            if (testedAtomTypes == null)
            {
                testedAtomTypes = new Dictionary<string, int>();
            }

            try
            {
                var type = GetFactory().GetAtomType(expectedID);
                Assert.IsNotNull(type, $"Attempt to test atom type which is not defined in the {AtomTypeListName}: {expectedID}");
            }
            catch (NoSuchAtomTypeException exception)
            {
                Console.Error.WriteLine($"Attempt to test atom type which is not defined in the {AtomTypeListName}: {exception.Message}");
            }
            if (testedAtomTypes.ContainsKey(expectedID))
            {
                // increase the count, so that redundancy can be calculated
                testedAtomTypes[expectedID] = 1 + testedAtomTypes[expectedID];
            }
            else
            {
                testedAtomTypes[expectedID] = 1;
            }
        }

        public virtual void TestForDuplicateDefinitions()
        {
            var expectedTypesArray = GetFactory().GetAllAtomTypes();
            var alreadyDefinedTypes = new HashSet<string>();
            foreach (var ee in expectedTypesArray)
            {
                string definedType = ee.AtomTypeName;
                if (alreadyDefinedTypes.Contains(definedType))
                {
                    Assert.Fail("Duplicate atom type definition in XML: " + definedType);
                }
                alreadyDefinedTypes.Add(definedType);
            }
        }

        //public static void CountTestedAtomTypes(IDictionary<string, int> testedAtomTypesMap, AtomTypeFactory factory)
        //{
        //    var testedAtomTypes = new HashSet<string>();
        //    foreach (var key in testedAtomTypesMap.Keys)
        //        testedAtomTypes.Add(key);

        //    var definedTypes = new HashSet<string>();
        //    var expectedTypesArray = factory.GetAllAtomTypes();
        //    foreach (var ee in expectedTypesArray)
        //    {
        //        definedTypes.Add(ee.AtomTypeName);
        //    }

        //    if (Compares.AreOrderLessDeepEqual(definedTypes, testedAtomTypes))
        //    {
        //        // all is fine
        //    }
        //    else if (definedTypes.Count > testedAtomTypes.Count)
        //    {
        //        // more atom types defined than tested
        //        int expectedTypeCount = definedTypes.Count;
        //        foreach (var e in testedAtomTypes)
        //            definedTypes.Remove(e);
        //        string errorMessage = "Atom types defined but not tested:";
        //        foreach (var notTestedType in definedTypes)
        //        {
        //            errorMessage += " " + notTestedType;
        //        }
        //        if (expectedTypeCount != testedAtomTypes.Count)
        //        {
        //            Assert.Fail(errorMessage);
        //        }
        //    }
        //    else
        //    { // testedAtomTypes.Count > definedTypes.Count
        //      // more atom types tested than defined
        //        int testedTypeCount = testedAtomTypes.Count;
        //        foreach (var e in definedTypes)
        //            testedAtomTypes.Remove(e);
        //        string errorMessage = "Atom types tested but not defined:";
        //        foreach (var notDefined in testedAtomTypes)
        //        {
        //            errorMessage += " " + notDefined;
        //        }
        //        if (testedTypeCount != testedAtomTypes.Count)
        //        {
        //            Assert.Fail(errorMessage);
        //        }
        //    }
        //}
    }
}
