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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.IO.Formats;
using NCDK.Tools.Manipulator;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for the instantiation and functionality of the <see cref="ReaderFactory"/>.
    /// </summary>
    // @cdk.module test-io
    [TestClass()]
    public class AbstractReaderFactoryTest
    {
        private ReaderFactory factory = new ReaderFactory();

        internal void ExpectReader(string filename, IResourceFormat expectedFormat, int expectedAtomCount, int expectedBondCount)
        {
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            Assert.IsNotNull(ins, "Cannot find file: " + filename);
            if (expectedFormat is IChemFormatMatcher)
            {
                factory.RegisterFormat((IChemFormatMatcher)expectedFormat);
            }
            ISimpleChemObjectReader reader = factory.CreateReader(ins);
            Assert.IsNotNull(reader);
            Assert.AreEqual(((IChemFormat)expectedFormat).ReaderClassName, reader.GetType().FullName);
            // now try reading something from it
            IChemObject[] objects = { new ChemFile(), new ChemModel(), new AtomContainer(), new Reaction() };
            bool read = false;
            for (int i = 0; (i < objects.Length && !read); i++)
            {
                if (reader.Accepts(objects[i].GetType()))
                {
                    IChemObject chemObject = reader.Read(objects[i]);
                    Assert.IsNotNull(chemObject, "Reader accepted a " + objects[i].GetType().Name + " but failed to read it");
                    AssertAtomCount(expectedAtomCount, chemObject);
                    AssertBondCount(expectedBondCount, chemObject);
                    read = true;
                }
            }
            if (read)
            {
                // ok, reseting worked
            }
            else
            {
                Assert.Fail("Reading an IChemObject from the Reader did not work properly.");
            }
        }

        void AssertBondCount(int expectedBondCount, IChemObject chemObject)
        {
            if (expectedBondCount != -1)
            {
                if (chemObject is IChemFile)
                {
                    Assert.AreEqual(expectedBondCount, ChemFileManipulator.GetBondCount((IChemFile)chemObject));
                }
                else if (chemObject is IChemModel)
                {
                    Assert.AreEqual(expectedBondCount, ChemModelManipulator.GetBondCount((IChemModel)chemObject));
                }
                else if (chemObject is IAtomContainer)
                {
                    Assert.AreEqual(expectedBondCount, ((IAtomContainer)chemObject).Bonds.Count);
                }
                else if (chemObject is IReaction)
                {
                    Assert.AreEqual(expectedBondCount, ReactionManipulator.GetBondCount((IReaction)chemObject));
                }
            }
        }

        void AssertAtomCount(int expectedAtomCount, IChemObject chemObject)
        {
            if (expectedAtomCount != -1)
            {
                if (chemObject is IChemFile)
                {
                    Assert.AreEqual(expectedAtomCount, ChemFileManipulator.GetAtomCount((IChemFile)chemObject));
                }
                else if (chemObject is IChemModel)
                {
                    Assert.AreEqual(expectedAtomCount, ChemModelManipulator.GetAtomCount((IChemModel)chemObject));
                }
                else if (chemObject is IAtomContainer)
                {
                    Assert.AreEqual(expectedAtomCount, ((IAtomContainer)chemObject).Atoms.Count);
                }
                else if (chemObject is IReaction)
                {
                    Assert.AreEqual(expectedAtomCount, ReactionManipulator.GetAtomCount((IReaction)chemObject));
                }
            }
        }
    }
}
