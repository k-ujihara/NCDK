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
using NCDK.Silent;
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
            var engine = DescriptorEngine.Create<IMolecularDescriptor>(ChemObjectBuilder.Instance);
            Assert.IsNotNull(engine);
        }

        [TestMethod()]
        public void TestLoadingOfMolecularDescriptors()
        {
            var engine = DescriptorEngine.Create<IMolecularDescriptor>(ChemObjectBuilder.Instance);
            Assert.IsNotNull(engine);
            int loadedDescriptors = engine.GetDescriptorInstances().Count();
            Assert.IsTrue(0 != loadedDescriptors, "Could not load any descriptors");
            Assert.AreEqual(loadedDescriptors, engine.GetDescriptorClassNames().Count());
            Assert.AreEqual(loadedDescriptors, engine.GetDescriptorSpecifications().Count());
        }

        [TestMethod()]
        public void TestLoadingOfAtomicDescriptors()
        {
            var engine = DescriptorEngine.Create<IAtomicDescriptor>(ChemObjectBuilder.Instance);
            Assert.IsNotNull(engine);
            int loadedDescriptors = engine.GetDescriptorInstances().Count();
            Assert.AreNotSame(0, loadedDescriptors);
            Assert.AreEqual(loadedDescriptors, engine.GetDescriptorClassNames().Count());
            Assert.AreEqual(loadedDescriptors, engine.GetDescriptorSpecifications().Count());
        }

        [TestMethod()]
        public void TestLoadingOfBondDescriptors()
        {
            var engine = DescriptorEngine.Create<IBondDescriptor>(ChemObjectBuilder.Instance);
            Assert.IsNotNull(engine);
            int loadedDescriptors = engine.GetDescriptorInstances().Count();
            Assert.AreNotSame(0, loadedDescriptors);
            Assert.AreEqual(loadedDescriptors, engine.GetDescriptorClassNames().Count());
            Assert.AreEqual(loadedDescriptors, engine.GetDescriptorSpecifications().Count());
        }

        [TestMethod()]
        public void TestDictionaryType()
        {
            var engine = DescriptorEngine.Create<IMolecularDescriptor>(ChemObjectBuilder.Instance);

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
            var engine = DescriptorEngine.Create<IMolecularDescriptor>(ChemObjectBuilder.Instance);

            string className = "NCDK.QSAR.Descriptors.Moleculars.TPSADescriptor";
            DescriptorSpecification specRef = new DescriptorSpecification(
                    "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#tpsa",
                    this.GetType().FullName,
                    "The Chemistry Development Kit");

            var dictClass = engine.GetDictionaryClass(className).ToReadOnlyList();
            Assert.AreEqual(2, dictClass.Count);
            Assert.AreEqual("topologicalDescriptor", dictClass[0]);
            Assert.AreEqual("electronicDescriptor", dictClass[1]);

            dictClass = engine.GetDictionaryClass(specRef).ToReadOnlyList();
            Assert.AreEqual(2, dictClass.Count);
            Assert.AreEqual("topologicalDescriptor", dictClass[0]);
            Assert.AreEqual("electronicDescriptor", dictClass[1]);
        }

        [TestMethod()]
        public void TestAvailableClass()
        {
            var engine = DescriptorEngine.Create<IMolecularDescriptor>(ChemObjectBuilder.Instance);
            var availClasses = engine.GetAvailableDictionaryClasses();
            Assert.AreEqual(5, availClasses.Count());
        }

        [TestMethod()]
        public void TestLoadingOfAtomPairDescriptors()
        {
            var engine = DescriptorEngine.Create<IAtomicDescriptor>(ChemObjectBuilder.Instance);
            Assert.IsNotNull(engine);
            int loadedDescriptors = engine.GetDescriptorInstances().Count();
            Assert.AreNotSame(0, loadedDescriptors);
            Assert.AreEqual(loadedDescriptors, engine.GetDescriptorClassNames().Count());
            Assert.AreEqual(loadedDescriptors, engine.GetDescriptorSpecifications().Count());
        }
    }
}
