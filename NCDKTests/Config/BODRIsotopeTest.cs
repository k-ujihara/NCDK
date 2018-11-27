/* Copyright (C) 2013  Egon Willighagen <egonw@users.sf.net>
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

namespace NCDK.Config
{
    /// <summary>
    /// Checks the functionality of the <see cref="BODRIsotope"/>.
    /// </summary>
    [TestClass()]
    public class BODRIsotopeTest
    {
        [TestMethod()]
        public virtual void TestConstructor()
        {
            IIsotope isotope = new BODRIsotope("C", 6, 12, 12.0, 99.0);
            Assert.AreEqual("C", isotope.Symbol);
            Assert.AreEqual(6, isotope.AtomicNumber);
            Assert.AreEqual(12, isotope.MassNumber.Value);
            Assert.AreEqual(12.0, isotope.ExactMass.Value, 0.001);
            Assert.AreEqual(99.0, isotope.NaturalAbundance.Value, 0.001);
        }

        [TestMethod()]
        public virtual void TestNonclonable()
        {
            var isotope = new BODRIsotope("C", 6, 12, 12.0, 99.0);
            IIsotope clone = (IIsotope)isotope.Clone();
            Assert.AreEqual(isotope, clone);
        }

        [TestMethod()]
        public void TestImmutable()
        {
            IIsotope isotope = new BODRIsotope("C", 6, 12, 12.0, 99.0)
            {
                // try mutations
                Symbol = "N",
                AtomicNumber = 5,
                MassNumber = 15,
                ExactMass = 15.000,
                NaturalAbundance = 0.364
            };
            // check if original
            Assert.AreEqual(6, isotope.AtomicNumber);
            Assert.AreEqual(12, isotope.MassNumber.Value);
            Assert.AreEqual(12.0, isotope.ExactMass.Value, 0.001);
            Assert.AreEqual(99.0, isotope.NaturalAbundance.Value, 0.001);
        }

        [TestMethod()]
        public void Untested()
        {
            Assert.IsTrue(true); // keep PMD from complaining
        }
    }
}