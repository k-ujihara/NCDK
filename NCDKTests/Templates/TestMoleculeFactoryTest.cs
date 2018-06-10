/*
 * Copyright (c) 2013. John May <jwmay@users.sf.net>
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
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace NCDK.Templates
{
    /// <summary>
    /// This class contains methods for generating simple organic molecules and is
    /// copy of <see cref="FaulonSignatures.Chemistry.MoleculeFactory"/> for use in tests.
    /// </summary>
    // @cdk.module test-data
    [TestClass()]
    public class TestMoleculeFactoryTest
    {
        [TestMethod()]
        public void Test()
        {
            foreach (var method in typeof(TestMoleculeFactory).GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                var parameters = method.GetParameters();
                switch (parameters.Length)
                {
                    case 0:
                        method.Invoke(null, new object[] { });
                        break;
                    case 1:
                        method.Invoke(null, new object[] { 10 });
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
