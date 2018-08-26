/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
 *                    2008  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Silent;
using System.Linq;

namespace NCDK.Config
{
    /// <summary>
    /// Checks the functionality of the <see cref="NaturalElement"/>.
    /// </summary>
    // @cdk.module test-core
    [TestClass()]
    public class OWLBasedAtomTypeConfiguratorTest
    {
        [TestMethod()]
        public virtual void TestCDKBasedAtomTypeConfigurator()
        {
            OWLBasedAtomTypeConfigurator configurator = new OWLBasedAtomTypeConfigurator();
            Assert.IsNotNull(configurator);
        }

        [TestMethod()]
        public virtual void TestReadAtomTypes_IChemObjectBuilder()
        {
            var configFile = "NCDK.Dict.Data.cdk-atom-types.owl";
            var ins = ResourceLoader.GetAsStream(configFile);
            var configurator = new OWLBasedAtomTypeConfigurator();
            configurator.SetStream(ins);
            var atomTypes = configurator.ReadAtomTypes(new ChemObject().Builder);
            Assert.AreNotSame(0, atomTypes.Count());
        }

        [TestMethod()]
        public void TestSetInputStream_InputStream()
        {
            TestReadAtomTypes_IChemObjectBuilder();
        }
    }
}