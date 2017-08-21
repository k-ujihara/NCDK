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
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace NCDK.QSAR.Descriptors
{
    /// <summary>
    /// Tests for molecular descriptors.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    // @cdk.module test-qsar
    [TestClass()]
    public abstract class DescriptorTest<T> : CDKTestCase where T : IDescriptor
    {
        protected virtual T Descriptor { get; set; }

        public DescriptorTest() { }

        public virtual void SetDescriptor(Type descriptorClass)
        {
            if (Descriptor == null)
            {
                var defaultConstructor = descriptorClass.GetConstructor(Type.EmptyTypes);
                this.Descriptor = (T)defaultConstructor.Invoke(Array.Empty<object>());
            }
        }

        /// <summary>
        /// Makes sure that the extending class has set the super.descriptor.
        /// Each extending class should have this bit of code (JUnit3 formalism):
        /// <pre>
        /// [TestMethod()] public void SetUp() {
        ///   // Pass a Class, not an Object!
        ///   SetDescriptor(typeof(SomeDescriptor));
        /// }
        ///
        /// <p>The unit tests in the extending class may use this instance, but
        /// are not required.
        ///
        /// </pre>
        /// </summary>
        [TestMethod()]
        public void TestHasSetSuperDotDescriptor()
        {
            Assert.IsNotNull(Descriptor, "The extending class must set the super.descriptor in its SetUp() method.");
        }

        /// <summary>
        /// Checks if the parameterization is consistent.
        ///
        // @throws Exception
        /// </summary>
        [TestMethod()]
        public void TestGetParameterNames()
        {
            string[] paramNames = Descriptor.ParameterNames;
            if (paramNames == null) paramNames = new string[0];
            foreach (var paramName in paramNames)
            {
                Assert.IsNotNull("A parameter name must not be null.", paramName);
                Assert.AreNotEqual(0, paramName.Length, "A parameter name string must not be empty.");
            }
        }

        /// <summary>
        // @cdk.bug 1862137
        /// </summary>
        [TestMethod()]
        public void TestGetParameters()
        {
            var parameters = Descriptor.Parameters;
            if (parameters == null)
            {
                Assert.AreEqual(0,
                        Descriptor.ParameterNames == null ? 0 : Descriptor.ParameterNames.Length,
                        "For all parameters a default or actual value must be returned.");
                parameters = new object[0];
            }
            foreach (var param in parameters)
            {
                Assert.IsNotNull(param, "A parameter default must not be null.");
            }
        }

        /// <summary>
        // @cdk.bug 1862137
        /// </summary>
        [TestMethod()]
        public void TestGetParameterType_String()
        {
            string[] paramNames = Descriptor.ParameterNames;
            if (paramNames == null) paramNames = new string[0];
            var parameters = Descriptor.Parameters;
            if (parameters == null) parameters = new object[0];

            for (int i = 0; i < paramNames.Length; i++)
            {
                object type = Descriptor.GetParameterType(paramNames[i]);
                Assert.IsNotNull(type,
                    "The GetParameterType(string) return type is null for the " + "parameter: "
                        + paramNames[i]);
                Assert.AreEqual(
                    type.GetType().FullName, parameters[i].GetType().FullName,
                    "The GetParameterType(string) return type is not consistent "
                        + "with the Parameters types for parameter " + i);
            }
        }

        [TestMethod()]
        public void TestParameterConsistency()
        {
            string[] paramNames = Descriptor.ParameterNames;
            //      FIXME: see TestGetParameterNames() comment on the same line
            if (paramNames == null) paramNames = new string[0];
            var parameters = Descriptor.Parameters;
            //      FIXME: see TestGetParameters() comment on the same line
            if (parameters == null) parameters = new object[0];

            Assert.AreEqual(
                paramNames.Length, parameters.Length,
                "The number of returned parameter names must equate the number of returned parameters");
        }

        [TestMethod()]
        public void TestGetSpecification()
        {
            IImplementationSpecification spec = Descriptor.Specification;
            Assert.IsNotNull(spec, "The descriptor specification returned must not be null.");

            Assert.IsNotNull(spec.ImplementationIdentifier, "The specification identifier must not be null.");
            Assert.AreNotEqual(0, spec.ImplementationIdentifier.Length,
                "The specification identifier must not be empty.");

            Assert.IsNotNull(spec.ImplementationTitle, "specification title must not be null.");
            Assert.AreNotEqual(0, spec.ImplementationTitle.Length, "The specification title must not be empty.");

            Assert.IsNotNull(spec.ImplementationVendor, "The specification vendor must not be null.");
            Assert.AreNotEqual(0, spec.ImplementationVendor.Length, "The specification vendor must not be empty.");

            Assert.IsNotNull(spec.SpecificationReference, "The specification reference must not be null.");
            Assert.AreNotEqual(0, spec.SpecificationReference.Length, "The specification reference must not be empty.");
        }

        /// <summary>
        /// Tests that the specification no longer gives an empty CVS identifier,
        /// but one based on a repository blob or commit.
        /// </summary>
        [TestMethod()]
        public void TestGetSpecification_IdentifierNonDefault()
        {
            IImplementationSpecification spec = Descriptor.Specification;
            Assert.AreNotSame("$Id$", spec.ImplementationIdentifier);
        }

        [TestMethod()]
        public void TestSetParameters_arrayObject()
        {
            Object[] defaultParams = Descriptor.Parameters;
            Descriptor.Parameters = defaultParams;
        }

        [TestMethod()]
        public void TestGetDescriptorNames()
        {
            string[] descNames = Descriptor.DescriptorNames;
            Assert.IsNotNull(descNames);
            Assert.IsTrue(descNames.Length >= 1, "One or more descriptor names must be provided");
            foreach (var s in descNames)
            {
                Assert.IsTrue(s.Length != 0, "Descriptor name must be non-zero length");
            }
        }
    }
}
