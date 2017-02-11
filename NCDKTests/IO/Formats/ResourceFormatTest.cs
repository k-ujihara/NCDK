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
using System;

namespace NCDK.IO.Formats
{
    // @cdk.module test-ioformats
    [TestClass()]
    abstract public class ResourceFormatTest
    {
        private IResourceFormat resourceFormat;

        public void SetResourceFormat(IResourceFormat format)
        {
            this.resourceFormat = format;
        }

        [TestMethod()]
        public void TestResourceFormatSet()
        {
            Assert.IsNotNull(resourceFormat, "You must use SetResourceFormatSet() to set the resourceFormat object.");
        }

        [TestMethod()]
        public void TestGetMIMEType()
        {
            if (resourceFormat.MIMEType == null)
            {
                // OK, that's fine
            }
            else
            {
                Assert.AreNotSame(0, resourceFormat.MIMEType.Length);
            }
        }

        [TestMethod()]
        public void TestGetFormatName()
        {
            Assert.IsNotNull(resourceFormat.FormatName);
            Assert.AreNotSame(0, resourceFormat.FormatName.Length);
        }

        [TestMethod()]
        public void TestGetPreferredNameExtension()
        {
            if (resourceFormat.PreferredNameExtension == null)
            {
                if (resourceFormat.NameExtensions == null || resourceFormat.NameExtensions.Length == 0)
                {
                    // Seems to be current practice
                    // FIXME: needs to be discussed
                }
                else
                {
                    Assert.Fail("This format define file name extensions (NameExtensions), but does not provide a prefered extension (PreferredNameExtension).");
                }
            }
            else
            {
                string prefExtension = resourceFormat.PreferredNameExtension;
                Assert.AreNotSame(0, prefExtension.Length);
                Assert.IsNotNull(
                        resourceFormat.NameExtensions,
                        "This format defines a preferred file name extension (PreferredNameExtension), but does not provide a full list of extensions (NameExtensions).");
                string[] allExtensions = resourceFormat.NameExtensions;
                bool prefExtInAllExtList = false;
                for (int i = 0; i < allExtensions.Length; i++)
                {
                    if (allExtensions[i].Equals(prefExtension)) prefExtInAllExtList = true;
                }
                Assert.IsTrue(prefExtInAllExtList, "The preferred extension is not found in the list of all extensions");
            }
        }

        [TestMethod()]
        public void TestGetNameExtensions()
        {
            if (resourceFormat.NameExtensions == null)
            {
                // Seems to be current practice
                // FIXME: needs to be discussed
            }
            else if (resourceFormat.NameExtensions.Length == 0)
            {
                // Seems to be current practice
                // FIXME: needs to be discussed
            }
            else
            {
                string[] exts = resourceFormat.NameExtensions;
                for (int i = 0; i < exts.Length; i++)
                {
                    string extension = exts[i];
                    Assert.IsNotNull(extension);
                    Assert.AreNotSame(0, extension.Length);
                    Assert.IsFalse(extension.Contains(","), "File name extensions should not contain ',' characters");
                    Assert.IsFalse(extension.Contains("."), "File name extensions should not contain '.' characters");
                }
            }
        }

        [TestMethod()]
        public void TestHashCode()
        {
            IResourceFormat a = (IResourceFormat)resourceFormat.GetType().GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
            IResourceFormat b = (IResourceFormat)resourceFormat.GetType().GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
            Assert.AreEqual(b.GetHashCode(), a.GetHashCode());
        }

        [TestMethod()]
        public void TestEquals()
        {
            IResourceFormat a = (IResourceFormat)resourceFormat.GetType().GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
            IResourceFormat b = (IResourceFormat)resourceFormat.GetType().GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
            Assert.AreEqual(b, a);
        }

        [TestMethod()]
        public void TestEquals_null()
        {
            IResourceFormat a = (IResourceFormat)resourceFormat.GetType().GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
            Assert.IsFalse(a.Equals(null));
        }

        [TestMethod()]
        public void TestIsXMLBased()
        {
            Assert.IsNotNull(resourceFormat.IsXmlBased);
        }
    }
}
