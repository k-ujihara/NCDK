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
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.LibIO.CML;
using NCDK.QSAR;
using NCDK.QSAR.Descriptors.Moleculars;
using NCDK.Templates;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace NCDK.IO.CML
{
    // @author John May
    // @cdk.module test-qsarcml
    [TestClass()]
    public class QSARCMLRoundTripTest
    {
        private static Convertor convertor;

        static QSARCMLRoundTripTest()
        {
            convertor = new Convertor(false, "");
            convertor.RegisterCustomizer(new QSARCustomizer());
        }

        [TestMethod()]
        public void TestDescriptorValue_QSAR()
        {
            IAtomContainer molecule = TestMoleculeFactory.MakeBenzene();
            IMolecularDescriptor descriptor = new WeightDescriptor();

            DescriptorValue originalValue = null;
            originalValue = descriptor.Calculate(molecule);
            molecule.SetProperty(originalValue.Specification, originalValue);
            IAtomContainer roundTrippedMol = CMLRoundTripTool.RoundTripMolecule(convertor, molecule);

            Assert.AreEqual(1, roundTrippedMol.GetProperties().Count);
            Console.Out.WriteLine("" + roundTrippedMol.GetProperties().Keys);
            var obj = roundTrippedMol.GetProperties().Keys.ToArray()[0];
            Console.Out.WriteLine("" + obj);
            Assert.IsTrue(obj is DescriptorSpecification);
            DescriptorSpecification spec = (DescriptorSpecification)obj;
            Assert.AreEqual(descriptor.Specification.SpecificationReference, spec.SpecificationReference);
            Assert.AreEqual(descriptor.Specification.ImplementationIdentifier, spec.ImplementationIdentifier);
            Assert.AreEqual(descriptor.Specification.ImplementationTitle, spec.ImplementationTitle);
            Assert.AreEqual(descriptor.Specification.ImplementationVendor, spec.ImplementationVendor);

            var value = roundTrippedMol.GetProperty<object>(spec);
            Assert.IsNotNull(value);
            Assert.IsTrue(value is DescriptorValue);
            DescriptorValue descriptorResult = (DescriptorValue)value;
            Assert.AreEqual(originalValue.Value.ToString(), descriptorResult.Value.ToString());
        }

        [TestMethod()]
        public void TestQSARCustomization()
        {
            StringWriter writer = new StringWriter();
            IAtomContainer molecule = TestMoleculeFactory.MakeBenzene();
            IMolecularDescriptor descriptor = new WeightDescriptor();

            CMLWriter cmlWriter = new CMLWriter(writer);
            cmlWriter.RegisterCustomizer(new QSARCustomizer());
            DescriptorValue value = descriptor.Calculate(molecule);
            molecule.SetProperty(value.Specification, value);

            cmlWriter.Write(molecule);
            string cmlContent = writer.ToString();
            Debug.WriteLine("****************************** TestQSARCustomization()");
            Debug.WriteLine(cmlContent);
            Debug.WriteLine("******************************");
            Assert.IsTrue(cmlContent.IndexOf("<property") != -1 && cmlContent.IndexOf("xmlns:qsar") != -1);
            Assert.IsTrue(cmlContent.IndexOf("#weight\"") != -1);
        }
    }
}
