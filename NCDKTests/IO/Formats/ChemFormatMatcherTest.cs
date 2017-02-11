/* Copyright (C) 2008  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@slists.sourceforge.net
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
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace NCDK.IO.Formats
{
    // @cdk.module test-ioformats
    [TestClass()]
    abstract public class ChemFormatMatcherTest : ChemFormatTest
    {
        private IChemFormatMatcher matcher;

        public virtual void SetChemFormatMatcher(IChemFormatMatcher matcher)
        {
            base.SetChemFormat(matcher);
            this.matcher = matcher;
        }

        [TestMethod()]
        public void TestChemFormatMatcherSet()
        {
            Assert.IsNotNull(matcher, "You must use SetChemFormatMatcher() to set the IChemFormatMatcher object.");
        }

        protected bool Matches(string header)
        {
            TextReader reader = new StringReader(header);
            string line;
            var lines = new List<string>();
            while ((line = reader.ReadLine()) != null)
                lines.Add(line);
            return matcher.Matches(lines).IsMatched;
        }

        [TestMethod()]
        public virtual void TestMatches()
        {
            Assert.IsTrue(true);
            // positive testing is done by the ReaderFactoryTest, and
            // negative tests are given below
        }

        [TestMethod()]
        public void TestNoLines()
        {
            Assert.IsFalse(matcher.Matches(new List<string>()).IsMatched);
        }

        [TestMethod()]
        public void TestMatchesEmptyString()
        {
            Assert.IsFalse(matcher.Matches(new List<string>(new[] { "" })).IsMatched);
        }

        [TestMethod()]
        public void TestMatchesLoremIpsum()
        {
            Assert.IsFalse(matcher
                    .Matches(
                            new[] { "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Etiam accumsan metus ut nulla." })
                    .IsMatched);
        }
    }
}
