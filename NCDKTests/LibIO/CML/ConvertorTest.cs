/* Copyright (C) 2007  Stefan Kuhn <shk3@users.sf.net>
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
 *
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Xml.Linq;

namespace NCDK.LibIO.CML
{
    // @cdk.module test-libiocml
    [TestClass()]
    public class ConvertorTest : CDKTestCase
    {
        [TestMethod()]
        public void TestCdkBondToCMLBond_Wedge()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IBond bond = builder.CreateBond();
            bond.Order = BondOrder.Single;
            bond.Stereo = BondStereo.Up;

            Convertor convertor = new Convertor(true, null);
            CMLBond cmlBond = convertor.CDKJBondToCMLBond(bond);

            var writer = new StringWriter();
            var d = new XDocument(cmlBond);
            d.Save(writer);

            string expected = "<bondStereo dictRef=\"cml:W\">W</bondStereo>";
            string actual = writer.ToString();

            Assert.IsTrue(actual.Contains(expected));
        }

        [TestMethod()]
        public void TestCdkBondToCMLBond_Hatch()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IBond bond = builder.CreateBond();
            bond.Order = BondOrder.Single;
            bond.Stereo = BondStereo.Down;

            Convertor convertor = new Convertor(true, null);
            CMLBond cmlBond = convertor.CDKJBondToCMLBond(bond);

            var writer = new StringWriter();
            var d = new XDocument(cmlBond);
            d.Save(writer);

            string expected = "<bondStereo dictRef=\"cml:H\">H</bondStereo>";
            string actual = writer.ToString();

            Assert.IsTrue(actual.Contains(expected));
        }
    }
}
