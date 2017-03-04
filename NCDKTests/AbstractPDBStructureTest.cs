/* Copyright (C) 2004-2007  Miguel Rojas <miguel.rojas@uni-koeln.de>
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
 *
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK
{
    /// <summary>
    /// Checks the functionality of {@link IPDBStructure} implementations.
    ///
    // @cdk.module test-interfaces
    /// </summary>
    [TestClass()]
    public abstract class AbstractPDBStructureTest : CDKTestCase
    {
        public IChemObjectBuilder Builder => ChemObject.Builder;
        public IChemObject ChemObject { get; set; }

        public IChemObject NewChemObject()
        {
            return (IChemObject)ChemObject.Clone();
        }

        [TestMethod()]
        public virtual void TestGetEndChainID() {
            IPDBStructure structure = Builder.CreatePDBStructure();
            Assert.IsNull(structure.EndChainID);
        }

    [TestMethod()]
    public virtual void TestSetEndChainID_Character() {
        IPDBStructure structure = Builder.CreatePDBStructure();
        char endChainID = 'x';
        structure.EndChainID = endChainID;
        Assert.AreEqual(endChainID, structure.EndChainID.Value);
    }

    [TestMethod()]
    public virtual void TestGetEndInsertionCode() {
        IPDBStructure structure = Builder.CreatePDBStructure();
        Assert.IsNull(structure.EndInsertionCode);
    }

    [TestMethod()]
    public virtual void TestSetEndInsertionCode_Character() {
        IPDBStructure structure = Builder.CreatePDBStructure();
        char endInsertionCode = 'x';
        structure.EndInsertionCode = endInsertionCode;
        Assert.AreEqual(endInsertionCode, structure.EndInsertionCode.Value);
    }

    [TestMethod()]
    public virtual void TestGetEndSequenceNumber() {
        IPDBStructure structure = Builder.CreatePDBStructure();
        Assert.IsNull(structure.EndSequenceNumber);
    }

    [TestMethod()]
    public virtual void TestSetEndSequenceNumber_Integer() {
        IPDBStructure structure = Builder.CreatePDBStructure();
        int endSequenceNumber = 5;
        structure.EndSequenceNumber = endSequenceNumber;
        Assert.AreEqual(endSequenceNumber, structure.EndSequenceNumber.Value);
    }

    [TestMethod()]
    public virtual void TestGetStartChainID() {
        IPDBStructure structure = Builder.CreatePDBStructure();
        Assert.IsNull(structure.StartChainID);
    }

    [TestMethod()]
    public virtual void TestSetStartChainID_Character() {
        IPDBStructure structure = Builder.CreatePDBStructure();
        char startChainID = 'x';
        structure.StartChainID = startChainID;
        Assert.AreEqual(startChainID, structure.StartChainID.Value);
    }

    [TestMethod()]
    public virtual void TestGetStartInsertionCode() {
        IPDBStructure structure = Builder.CreatePDBStructure();
        Assert.IsNull(structure.StartInsertionCode);
    }

    [TestMethod()]
    public virtual void TestSetStartInsertionCode_Character() {
        IPDBStructure structure = Builder.CreatePDBStructure();
        char startInsertionCode = 'x';
        structure.StartInsertionCode = startInsertionCode;
        Assert.AreEqual(startInsertionCode, structure.StartInsertionCode.Value);
    }

    [TestMethod()]
    public virtual void TestGetStartSequenceNumber() {
        IPDBStructure structure = Builder.CreatePDBStructure();
        Assert.IsNull(structure.StartSequenceNumber);
    }

    [TestMethod()]
    public virtual void TestSetStartSequenceNumber_Integer() {
        IPDBStructure structure = Builder.CreatePDBStructure();
        int startSequenceNumber = 5;
        structure.StartSequenceNumber = startSequenceNumber;
        Assert.AreEqual(startSequenceNumber, structure.StartSequenceNumber.Value);
    }

    [TestMethod()]
    public virtual void TestGetStructureType() {
        IPDBStructure structure = Builder.CreatePDBStructure();
        string type = structure.StructureType;
        Assert.IsNull(type);
    }

    [TestMethod()]
    public virtual void TestSetStructureType_String() {
        IPDBStructure structure = Builder.CreatePDBStructure();
        string type = "alpha-barrel";
        structure.StructureType = type;
        Assert.AreEqual(type, structure.StructureType);
    }

    /// <summary>
    /// Method to test whether the class complies with RFC #9.
    /// </summary>
    [TestMethod()]
    public virtual void TestToString() {
        IPDBStructure structure = Builder.CreatePDBStructure();
        string description = structure.ToString();
        for (int i = 0; i < description.Length; i++) {
            Assert.IsTrue('\n' != description[i]);
            Assert.IsTrue('\r' != description[i]);
        }
    }
}
}
