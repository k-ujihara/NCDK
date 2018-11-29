/* Copyright (C) 2005-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.Numerics;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.IO;

namespace NCDK
{
    /// <summary>
    /// Super class for <b>all</b> CDK TestCase implementations that ensures that
    /// the LoggingTool is configured. This is the JUnit4 version of CDKTestCase.
    /// </summary>
    /// <seealso cref="CDKTestCase"/>
    // @cdk.module test
    public class CDKTestCase
    {
        public virtual void AssertAreEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual)
            where T : IChemObject
        {
            var ae = expected.GetEnumerator();
            var be = actual.GetEnumerator();
            while (ae.MoveNext())
            {
                if (!be.MoveNext())
                    Assert.Fail();
                if (ae.Current == null && be.Current == null)
                    continue;
                if (!ae.Current.Compare(be.Current))
                    Assert.Fail();
            }
            if (be.MoveNext())
                Assert.Fail(); 
        }

        public virtual void AssertAreOrderLessEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual)
            where T : IChemObject
        {
            var ae = expected.GetEnumerator();
            var be = actual.GetEnumerator();
            while (ae.MoveNext())
            {
                if (!be.MoveNext())
                    Assert.Fail();
                if (ae.Current == null && be.Current == null)
                    continue;
                if (!ae.Current.Compare(be.Current))
                    Assert.Fail();
            }
            if (be.MoveNext())
                Assert.Fail();
        }

        /// <summary>
        /// Compares two Vector2 objects, and asserts that the XY coordinates
        /// are identical within the given error.
        /// </summary>
        /// <param name="p1">first Vector2</param>
        /// <param name="p2">second Vector2</param>
        /// <param name="error">maximal allowed error</param>
        public virtual void AssertAreEqual(Vector2? p1, Vector2? p2, double error)
        {
            if (p1 == p2)
                return;
            Assert.IsNotNull(p1, "The expected Vector2 is null");
            Assert.IsNotNull(p2, "The tested Vector2 is null");
            Assert.AreEqual(p1.Value.X, p2.Value.X, error);
            Assert.AreEqual(p1.Value.Y, p2.Value.Y, error);
        }

        /// <summary>
        /// Compares two Vector3 objects, and asserts that the XY coordinates
        /// are identical within the given error.
        /// </summary>
        /// <param name="p1">first Vector3</param>
        /// <param name="p2">second Vector3</param>
        /// <param name="error">maximal allowed error</param>
        public virtual void AssertAreEqual(Vector3? p1, Vector3? p2, double error)
        {
            if (p1 == p2)
                return;
            Assert.IsNotNull(p1, "The expected Vector3 is null");
            Assert.IsNotNull(p2, "The tested Vector3 is null");
            Assert.AreEqual(p1.Value.X, p2.Value.X, error);
            Assert.AreEqual(p1.Value.Y, p2.Value.Y, error);
            Assert.AreEqual(p1.Value.Z, p2.Value.Z, error);
        }

        /// <summary>
        /// Tests method that asserts that for all atoms an reasonable CDK atom
        /// type can be perceived.
        /// </summary>
        /// <param name="container">IAtomContainer to test atom types of</param>
        public virtual void AssertAtomTypesPerceived(IAtomContainer container)
        {
            var matcher = CDK.AtomTypeMatcher;
            foreach (var atom in container.Atoms)
            {
                var type = matcher.FindMatchingAtomType(container, atom);
                Assert.IsNotNull(type, $"Could not perceive atom type for: {atom}");
            }
        }

        /// <summary>
        /// Convenience method that perceives atom types (CDK scheme) and
        /// adds explicit hydrogens accordingly. It does not create 2D or 3D
        /// coordinates for the new hydrogens.
        /// </summary>
        /// <param name="container">to which explicit hydrogens are added.</param>
        protected void AddExplicitHydrogens(IAtomContainer container)
        {
            AddImplicitHydrogens(container);
            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(container);
        }

        /// <summary>
        /// Convenience method that perceives atom types (CDK scheme) and
        /// adds implicit hydrogens accordingly. It does not create 2D or 3D
        /// coordinates for the new hydrogens.
        /// </summary>
        /// <param name="container">to which implicit hydrogens are added.</param>
        protected virtual void AddImplicitHydrogens(IAtomContainer container)
        {
            var matcher = CDK.AtomTypeMatcher;
            var atomCount = container.Atoms.Count;
            var originalAtomTypeNames = new string[atomCount];
            for (int i = 0; i < atomCount; i++)
            {
                var atom = container.Atoms[i];
                var type = matcher.FindMatchingAtomType(container, atom);
                originalAtomTypeNames[i] = atom.AtomTypeName;
                atom.AtomTypeName = type.AtomTypeName;
            }
            var hAdder = CDK.HydrogenAdder;
            hAdder.AddImplicitHydrogens(container);
            // reset to the original atom types
            for (int i = 0; i < atomCount; i++)
            {
                var atom = container.Atoms[i];
                atom.AtomTypeName = originalAtomTypeNames[i];
            }
        }

        /// <summary>
        /// Convenience method to check that all bond orders are single
        /// and all heavy atoms are aromatic (and that all explicit
        /// hydrogens are not aromatic).
        /// </summary>
        /// <param name="container">the atom container to check</param>
        protected void AssertAllSingleOrAromatic(IAtomContainer container)
        {
            foreach (var bond in container.Bonds)
            {
                if (!bond.IsAromatic)
                    Assert.AreEqual(BondOrder.Single, bond.Order);
            }

            foreach (var atom in container.Atoms)
            {
                if (atom.Symbol.Equals("H"))
                    Assert.IsFalse(atom.IsAromatic, atom.Symbol + container.Atoms.IndexOf(atom) + " was aromatic");
                else
                    Assert.IsTrue(atom.IsAromatic, atom.Symbol + container.Atoms.IndexOf(atom) + " was not aromatic");
            }
        }

        /// <summary>
        /// Convenience method to check the atom symbols
        /// of a molecule.
        /// </summary>
        /// <param name="symbols">an array of the expected atom symbols</param>
        /// <param name="container">the atom container to check</param>
        protected void AssertAtomSymbols(string[] symbols, IAtomContainer container)
        {
            for (int i = 0; i < container.Atoms.Count; i++)
                Assert.AreEqual(symbols[i], container.Atoms[i].Symbol);
        }

        /// <summary>
        /// Convenience method to check the hybridization states
        /// of a molecule.
        /// </summary>
        /// <param name="hybridizations">an array of the expected hybridization states</param>
        /// <param name="container">the atom container to check</param>
        protected void AssertHybridizations(Hybridization[] hybridizations, IAtomContainer container)
        {
            for (int i = 0; i < container.Atoms.Count; i++)
                Assert.AreEqual(hybridizations[i], container.Atoms[i].Hybridization);
        }

        /// <summary>
        /// Convenience method to check the hydrogen counts
        /// of a molecule.
        /// </summary>
        /// <param name="hydrogenCounts">an array of the expected hydrogenCounts</param>
        /// <param name="container">the atom container to check</param>
        protected void AssertHydrogenCounts(int[] hydrogenCounts, IAtomContainer container)
        {
            for (int i = 0; i < container.Atoms.Count; i++)
                Assert.AreEqual(hydrogenCounts[i], container.Atoms[i].ImplicitHydrogenCount.Value);
        }

        /// <summary>
        /// Asserts that the given string has zero .Length.
        ///
        /// <param name="string">string to test the .Length of.</param>
        /// </summary>
        public virtual void AssertZeroLength(string testString)
        {
            Assert.IsNotNull(testString, "Expected a non-null string.");
            Assert.AreEqual(0, testString.Length, $"Expected a zero-.Length string, but found '{testString}'");
        }

        /// <summary>
        /// Asserts that the given string consists of a single line, and thus
        /// does not contain any '\r' and/or '\n' characters.
        /// </summary>
        /// <param name="string">string to test.</param>
        public virtual void AssertOneLiner(string testString)
        {
            Assert.IsNotNull(testString, "Expected a non-null string.");
            for (int i = 0; i < testString.Length; i++)
            {
                char c = testString[i];
                Assert.AreNotEqual('\n', c, "The string must not contain newline characters");
                Assert.AreNotEqual('\r', c, "The string must not contain newline characters");
            }
        }

        /// <summary>
        /// This test allows people to use the <see cref="TestMethodAttribute"/> annotation for
        /// methods that are testing in other classes than identified with <see cref="TestClass"/>.
        /// Bit of a workaround for the current set up, but useful in situations where
        /// a methods is rather untestable, such as <see cref="NCDK.Utils.Xml.XContentHandler"/>'s <see cref="NCDK.Utils.Xml.XContentHandler.EndElement(System.Xml.Linq.XElement)"/> methods.
        /// <para>
        /// Should be used only in these rare cases.
        /// </para>
        /// </summary>
        [TestMethod()]
        public virtual void TestedByOtherClass()
        {
            // several methods, like EndElement() are not directly tested
            Assert.IsTrue(true);
        }

        /// <summary>
        /// Asserts that the given subString is present in the fullString.
        /// </summary>
        /// <param name="fullString">string which should contain the subString</param>
        /// <param name="subString">string that must be present in the fullString</param>
        public virtual void AssertContains(string fullString, string subString)
        {
            Assert.IsNotNull(fullString, "Expected a non-null string to test contains against.");
            Assert.IsNotNull(subString, "Expected a non-null substring in contains test.");
            Assert.IsTrue(fullString.Contains(subString), $"Expected the full string '{fullString}' to contain '{subString}'.");
        }

        protected static string CopyFileToTmp(string prefix, string suffix, Stream ins, string toReplace, string replaceWith)
        {
            var tmpFile = Path.Combine(Path.GetTempPath(), prefix ?? "" + Guid.NewGuid().ToString() + suffix ?? "");
            string all;
            using (var rs = new StreamReader(ins))
            {
                all = rs.ReadToEnd();
                if (toReplace != null && replaceWith != null)
                    all = all.Replace(toReplace, replaceWith);
            }
            using (var ws = new StreamWriter(tmpFile))
            {
                ws.Write(all);
            }
            return tmpFile;
        }
    }
}
