/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
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
using System.Linq;

namespace NCDK.QSAR
{
    /// <summary>
    /// TestSuite that runs all tests for the <see cref="DescriptorEngine"/> DescriptorEngine.
    /// </summary>
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class DescriptorEngineTest : CDKTestCase
    {
        public DescriptorEngineTest() { }

        [TestMethod()]
        public void TestConstructor()
        {
            DescriptorEngine engine = DescriptorEngine.Create<IMolecularDescriptor>(Default.ChemObjectBuilder.Instance);
            Assert.IsNotNull(engine);
        }

        [TestMethod()]
        public void TestLoadingOfMolecularDescriptors()
        {
            DescriptorEngine engine = DescriptorEngine.Create<IMolecularDescriptor>(Default.ChemObjectBuilder.Instance);
            Assert.IsNotNull(engine);
            int loadedDescriptors = engine.GetDescriptorInstances().Count;
            Assert.IsTrue(0 != loadedDescriptors, "Could not load any descriptors");
            Assert.AreEqual(loadedDescriptors, engine.GetDescriptorClassNames().Count);
            Assert.AreEqual(loadedDescriptors, engine.GetDescriptorSpecifications().Count);
        }

        [TestMethod()]
        public void TestLoadingOfAtomicDescriptors()
        {
            DescriptorEngine engine = DescriptorEngine.Create<IAtomicDescriptor>(Default.ChemObjectBuilder.Instance);
            Assert.IsNotNull(engine);
            int loadedDescriptors = engine.GetDescriptorInstances().Count;
            Assert.AreNotSame(0, loadedDescriptors);
            Assert.AreEqual(loadedDescriptors, engine.GetDescriptorClassNames().Count);
            Assert.AreEqual(loadedDescriptors, engine.GetDescriptorSpecifications().Count);
        }

        [TestMethod()]
        public void TestLoadingOfBondDescriptors()
        {
            DescriptorEngine engine = DescriptorEngine.Create<IBondDescriptor>(Default.ChemObjectBuilder.Instance);
            Assert.IsNotNull(engine);
            int loadedDescriptors = engine.GetDescriptorInstances().Count;
            Assert.AreNotSame(0, loadedDescriptors);
            Assert.AreEqual(loadedDescriptors, engine.GetDescriptorClassNames().Count);
            Assert.AreEqual(loadedDescriptors, engine.GetDescriptorSpecifications().Count);
        }

        [TestMethod()]
        public void TestDictionaryType()
        {
            DescriptorEngine engine = DescriptorEngine.Create<IMolecularDescriptor>(Default.ChemObjectBuilder.Instance);

            string className = "NCDK.QSAR.Descriptors.Moleculars.ZagrebIndexDescriptor";
            DescriptorSpecification specRef = new DescriptorSpecification(
                    "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#zagrebIndex",
                    this.GetType().FullName,
                    "The Chemistry Development Kit");

            Assert.AreEqual("molecularDescriptor", engine.GetDictionaryType(className));
            Assert.AreEqual("molecularDescriptor", engine.GetDictionaryType(specRef));
        }

        [TestMethod()]
        public void TestDictionaryClass()
        {
            DescriptorEngine engine = DescriptorEngine.Create<IMolecularDescriptor>(Default.ChemObjectBuilder.Instance);

            string className = "NCDK.QSAR.Descriptors.Moleculars.TPSADescriptor";
            DescriptorSpecification specRef = new DescriptorSpecification(
                    "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#tpsa",
                    this.GetType().FullName,
                    "The Chemistry Development Kit");

            string[] dictClass = engine.GetDictionaryClass(className);
            Assert.AreEqual(2, dictClass.Length);
            Assert.AreEqual("topologicalDescriptor", dictClass[0]);
            Assert.AreEqual("electronicDescriptor", dictClass[1]);

            dictClass = engine.GetDictionaryClass(specRef);
            Assert.AreEqual(2, dictClass.Length);
            Assert.AreEqual("topologicalDescriptor", dictClass[0]);
            Assert.AreEqual("electronicDescriptor", dictClass[1]);
        }

        [TestMethod()]
        public void TestAvailableClass()
        {
            DescriptorEngine engine = DescriptorEngine.Create<IMolecularDescriptor>(Default.ChemObjectBuilder.Instance);
            var availClasses = engine.GetAvailableDictionaryClasses();
            Assert.AreEqual(5, availClasses.Count());
        }

        [TestMethod()]
        public void TestLoadingOfAtomPairDescriptors()
        {
            DescriptorEngine engine = DescriptorEngine.Create<IAtomicDescriptor>(Default.ChemObjectBuilder.Instance);
            Assert.IsNotNull(engine);
            int loadedDescriptors = engine.GetDescriptorInstances().Count;
            Assert.AreNotSame(0, loadedDescriptors);
            Assert.AreEqual(loadedDescriptors, engine.GetDescriptorClassNames().Count);
            Assert.AreEqual(loadedDescriptors, engine.GetDescriptorSpecifications().Count);
        }
    }
}
