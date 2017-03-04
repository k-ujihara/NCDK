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
using NCDK.Default;

namespace NCDK.Validate
{
    /// <summary>
    // @cdk.module test-standard
    /// </summary>
    [TestClass()]
    public class ProblemMarkerTest : CDKTestCase
    {
        public ProblemMarkerTest()
            : base()
        { }

        [TestMethod()]
        public void TestUnMarkWithError_IChemObject()
        {
            IChemObject obj = new ChemObject();
            Assert.IsNull(obj.GetProperty<bool?>(ProblemMarker.ErrorMarker));
            ProblemMarker.MarkWithError(obj);
            Assert.IsNotNull(obj.GetProperty<bool?>(ProblemMarker.ErrorMarker));
            ProblemMarker.UnMarkWithError(obj);
            Assert.IsNull(obj.GetProperty<bool?>(ProblemMarker.ErrorMarker));
        }

        [TestMethod()]
        public void TestUnMarkWithWarning_IChemObject()
        {
            IChemObject obj = new ChemObject();
            Assert.IsNull(obj.GetProperty<bool?>(ProblemMarker.WarningMarker));
            ProblemMarker.MarkWithWarning(obj);
            Assert.IsNotNull(obj.GetProperty<bool?>(ProblemMarker.WarningMarker));
            ProblemMarker.UnMarkWithWarning(obj);
            Assert.IsNull(obj.GetProperty<bool?>(ProblemMarker.WarningMarker));
        }

        [TestMethod()]
        public void TestUnMark_IChemObject()
        {
            IChemObject obj = new ChemObject();
            Assert.IsNull(obj.GetProperty<bool?>(ProblemMarker.WarningMarker));
            ProblemMarker.MarkWithWarning(obj);
            Assert.IsNotNull(obj.GetProperty<bool?>(ProblemMarker.WarningMarker));
            ProblemMarker.MarkWithError(obj);
            Assert.IsNotNull(obj.GetProperty<bool?>(ProblemMarker.ErrorMarker));
            ProblemMarker.Unmark(obj);
            Assert.IsNull(obj.GetProperty<bool?>(ProblemMarker.WarningMarker));
            Assert.IsNull(obj.GetProperty<bool?>(ProblemMarker.ErrorMarker));
        }

        [TestMethod()]
        public void TestMarkWithError_IChemObject()
        {
            IChemObject obj = new ChemObject();
            Assert.IsNull(obj.GetProperty<bool?>(ProblemMarker.ErrorMarker));
            ProblemMarker.MarkWithError(obj);
            Assert.IsNotNull(obj.GetProperty<bool?>(ProblemMarker.ErrorMarker));
        }

        [TestMethod()]
        public void TestMarkWithWarning_IChemObject()
        {
            IChemObject obj = new ChemObject();
            Assert.IsNull(obj.GetProperty<bool?>(ProblemMarker.WarningMarker));
            ProblemMarker.MarkWithWarning(obj);
            Assert.IsNotNull(obj.GetProperty<bool?>(ProblemMarker.WarningMarker));
        }
    }
}
