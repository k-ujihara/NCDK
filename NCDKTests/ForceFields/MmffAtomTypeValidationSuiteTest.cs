/*
 * Copyright (c) 2014 European Bioinformatics Institute (EMBL-EBI)
 *                    John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or (at
 * your option) any later version. All we ask is that proper credit is given
 * for our work, which includes - but is not limited to - adding the above
 * copyright notice to the beginning of your source code files, and to any
 * copyright notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Common.Base;
using NCDK.Common.Collections;
using NCDK.Smiles;
using System;

namespace NCDK.ForceFields
{
    /// <summary>
    /// Ensure the atom types of the validation suite (http://server.ccl.net/cca/data/MMFF94/) are
    /// correctly assigned.
    /// </summary>
    // @author John May
    // [TestCategory("SlowTest")] // waiting on SlowTest patch
    [TestClass()]
    public class MmffAtomTypeValidationSuiteTest : AbstractMmffAtomTypeValidationSuiteTest
    {
        static readonly MmffAtomTypeMatcher Instance = new MmffAtomTypeMatcher();

        public override string[] Assign(IAtomContainer container)
        {
            return Instance.SymbolicTypes(container);
        }

        public override void AssertMatchingTypes(IAtomContainer container, string[] actual, string[] expected)
        {
            // create a useful failure message that displays a SMILES and tags incorrectly typed atoms
            string mesg = "";
            if (!Arrays.AreEqual(actual, expected))
            {
                for (int i = 0; i < actual.Length; i++)
                {
                    if (!expected[i].Equals(actual[i]))
                    {
                        container.Atoms[i].SetProperty(CDKPropertyName.AtomAtomMapping, 1);
                    }
                }
                try
                {
                    mesg = SmilesGenerator.Generic.Aromatic().WithAtomClasses().Create(container);
                }
                catch (CDKException e)
                {
                    Console.Error.WriteLine(e.Message);
                }
            }

            Assert.IsTrue(Compares.AreDeepEqual(expected, actual), mesg);
        }
    }
}
