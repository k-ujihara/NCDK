/*
 * Copyright (C) 2010  Mark Rijnbeek <mark_rynbeek@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may
 * distribute with programs based on this work.
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
using NCDK.IO.Formats;
using NCDK.Isomorphisms.Matchers;
using NCDK.Silent;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace NCDK.IO
{
    /// <summary>
    /// Unit tests for  <see cref="RGroupQueryReader"/>.
    /// </summary>
    // @cdk.module test-io
    // @author Mark Rijnbeek
    [TestClass()]
    public class RGroupQueryReaderTest : SimpleChemObjectReaderTest
    {
        protected override string TestFile => "NCDK.Data.MDL.rgfile.1.mol";
        protected override Type ChemObjectIOToTestType => typeof(RGroupQueryReader);

        public RGroupQueryReaderTest() { }

        [TestMethod()]
        public void TestAccepts()
        {
            RGroupQueryReader reader = new RGroupQueryReader(new StringReader(""));
            Assert.IsFalse(reader.Accepts(typeof(AtomContainer)));
            Assert.IsTrue(reader.Accepts(typeof(RGroupQuery)));
        }

        [TestMethod()]
        public override void TestAcceptsAtLeastOneChemObjectClass()
        {
            RGroupQueryReader reader = new RGroupQueryReader(new StringReader(""));
            Assert.IsTrue(reader.Accepts(typeof(RGroupQuery)));
        }

        public override void TestAcceptsAtLeastOneNonotifyObject() { }

        /// <summary>
        /// Test that the format factory guesses the correct IChemFormat based on the file content.
        /// </summary>
        [TestMethod()]
        public void TestRGFileFormat()
        {
            var filename = "NCDK.Data.MDL.rgfile.1.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            IChemFormat format = new FormatFactory().GuessFormat(ins);
            Assert.AreEqual(format.GetType(), typeof(RGroupQueryFormat));
        }

        /// <summary>
        /// Test parsing of RGFile rgfile.1.mol.
        /// Simple R-group query file.
        /// </summary>
        [TestMethod()]
        public void TestRgroupQueryFile1()
        {
            var filename = "NCDK.Data.MDL.rgfile.1.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            RGroupQueryReader reader = new RGroupQueryReader(ins);
            RGroupQuery rGroupQuery = (RGroupQuery)reader.Read(new RGroupQuery());
            reader.Close();
            Assert.IsNotNull(rGroupQuery);
            Assert.AreEqual(rGroupQuery.RGroupDefinitions.Count, 1);
            Assert.AreEqual(rGroupQuery.RootStructure.Atoms.Count, 7);

            foreach (var at in rGroupQuery.GetAllRgroupQueryAtoms())
            {
                if (at is PseudoAtom)
                {
                    Assert.AreEqual(((PseudoAtom)at).Label, "R1");
                    var rootApo = rGroupQuery.RootAttachmentPoints;
                    var apoBonds = rootApo[at];
                    Assert.AreEqual(apoBonds.Count, 1);
                    // Assert that the root attachment is the bond between R1 and P
                    foreach (var bond in rGroupQuery.RootStructure.Bonds)
                    {
                        if (bond.Contains(at))
                        {
                            Assert.AreEqual(bond, apoBonds[1]);
                            foreach (var atInApo in bond.Atoms)
                            {
                                Assert.IsTrue(atInApo.Symbol.Equals("R") || atInApo.Symbol.Equals("P"));
                            }
                        }
                    }
                }
            }

            int val_1 = rGroupQuery.RGroupDefinitions.Keys.First();
            Assert.AreEqual(val_1, 1);
            RGroupList rList = rGroupQuery.RGroupDefinitions[val_1];
            Assert.AreEqual(rList.Occurrence, "0,1-3");

            var rGroups = rList.RGroups;
            Assert.AreEqual(rGroups[0].FirstAttachmentPoint.Symbol, "N");
            Assert.AreEqual(rGroups[1].FirstAttachmentPoint.Symbol, "O");
            Assert.AreEqual(rGroups[2].FirstAttachmentPoint.Symbol, "S");

            Assert.IsNull(rGroups[0].SecondAttachmentPoint);
            Assert.IsNull(rGroups[1].SecondAttachmentPoint);
            Assert.IsNull(rGroups[2].SecondAttachmentPoint);

            var configurations = rGroupQuery.GetAllConfigurations();
            Assert.AreEqual(configurations.Count(), 4);

            //IsRestH is set to true for R1, so with zero substitutes, the phosphor should get the restH flag set to true.
            bool restH_Identified = false;
            foreach (var atc in configurations)
            {
                if (atc.Atoms.Count == 6)
                {
                    foreach (var atom in atc.Atoms)
                    {
                        if (atom.Symbol.Equals("P"))
                        {
                            Assert.IsNotNull(atom.GetProperty<bool?>(CDKPropertyName.RestH));
                            Assert.AreEqual(atom.GetProperty<bool>(CDKPropertyName.RestH), true);
                            restH_Identified = true;
                        }
                    }
                }
            }
            Assert.IsTrue(restH_Identified);
        }

        /// <summary>
        /// Test parsing of RGFile rgfile.2.mol.
        /// More elaborate R-group query file.
        /// </summary>
        [TestMethod()]
        public void TestRgroupQueryFile2()
        {
            var filename = "NCDK.Data.MDL.rgfile.2.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new RGroupQueryReader(ins);
            var rGroupQuery = reader.Read(new RGroupQuery());
            reader.Close();
            Assert.IsNotNull(rGroupQuery);
            Assert.AreEqual(rGroupQuery.RGroupDefinitions.Count, 3);
            Assert.AreEqual(rGroupQuery.RootStructure.Atoms.Count, 14);
            Assert.AreEqual(rGroupQuery.RootAttachmentPoints.Count, 4);

            var rGroupQueryAtoms = rGroupQuery.GetAllRgroupQueryAtoms();
            Assert.AreEqual(rGroupQueryAtoms.Count, 4);

            rGroupQueryAtoms = rGroupQuery.GetRgroupQueryAtoms(1);
            Assert.AreEqual(rGroupQueryAtoms.Count, 1);

            foreach (var at in rGroupQuery.GetAllRgroupQueryAtoms())
            {
                if (at is PseudoAtom)
                {
                    Assert.IsTrue(RGroupQuery.IsValidRgroupQueryLabel(((PseudoAtom)at).Label));

                    var rgroupNum = int.Parse(((PseudoAtom)at).Label.Substring(1));
                    Assert.IsTrue(rgroupNum == 1 || rgroupNum == 2 || rgroupNum == 11);
                    switch (rgroupNum)
                    {
                        case 1:
                            {
                                //Test: R1 has two attachment points, defined by AAL
                                var rootApo = rGroupQuery.RootAttachmentPoints;
                                var apoBonds = rootApo[at];
                                Assert.AreEqual(apoBonds.Count, 2);
                                Assert.AreEqual(apoBonds[1].GetOther(at).Symbol, "N");
                                Assert.IsTrue(apoBonds[2].GetOther(at).Symbol.Equals("C"));
                                //Test: Oxygens are the 2nd APO's for R1
                                var rList = rGroupQuery.RGroupDefinitions[1];
                                Assert.AreEqual(rList.RGroups.Count, 2);
                                var rGroups = rList.RGroups;
                                Assert.AreEqual(rGroups[0].SecondAttachmentPoint.Symbol, "O");
                                Assert.AreEqual(rGroups[1].SecondAttachmentPoint.Symbol, "O");
                                Assert.IsFalse(rList.IsRestH);
                            }
                            break;
                        case 2:
                            {
                                RGroupList rList = rGroupQuery.RGroupDefinitions[2];
                                Assert.AreEqual(rList.RGroups.Count, 2);
                                Assert.AreEqual(rList.Occurrence, "0,2");
                                Assert.AreEqual(rList.RequiredRGroupNumber, 11);
                                Assert.IsFalse(rList.IsRestH);
                            }
                            break;
                        case 11:
                            {
                                RGroupList rList = rGroupQuery.RGroupDefinitions[11];
                                Assert.AreEqual(rList.RGroups.Count, 1);
                                Assert.AreEqual(rList.RequiredRGroupNumber, 0);
                                Assert.IsTrue(rList.IsRestH);

                                var rGroups = rList.RGroups;
                                Assert.AreEqual(rGroups[0].FirstAttachmentPoint.Symbol, "Pt");
                                Assert.AreEqual(rGroups[0].SecondAttachmentPoint, null);
                            }
                            break;
                    }
                }
            }

            var configurations = rGroupQuery.GetAllConfigurations();
            Assert.AreEqual(12, configurations.Count());

            //Test restH values
            int countRestHForSmallestConfigurations = 0;
            foreach (var atc in configurations)
            {
                if (atc.Atoms.Count == 13)
                { // smallest configuration
                    foreach (var atom in atc.Atoms)
                    {
                        if (atom.GetProperty<bool?>(CDKPropertyName.RestH) != null)
                        {
                            countRestHForSmallestConfigurations++;
                            if (atom.Symbol.Equals("P"))
                                Assert.AreEqual(atom.GetProperty<bool>(CDKPropertyName.RestH), true);
                        }
                    }
                }
            }
            Assert.AreEqual(countRestHForSmallestConfigurations, 6);

        }

        /// <summary>
        /// Test parsing of RGFile rgfile.3.mol.
        /// This R-group query has R1 bound double twice, and has AAL lines to parse.
        /// </summary>
        [TestMethod()]
        public void TestRgroupQueryFile3()
        {
            var filename = "NCDK.Data.MDL.rgfile.3.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            RGroupQueryReader reader = new RGroupQueryReader(ins);
            RGroupQuery rGroupQuery = reader.Read(new RGroupQuery());
            reader.Close();
            Assert.IsNotNull(rGroupQuery);
            Assert.AreEqual(rGroupQuery.RGroupDefinitions.Count, 1);
            Assert.AreEqual(rGroupQuery.RootStructure.Atoms.Count, 10);
            Assert.AreEqual(rGroupQuery.RootAttachmentPoints.Count, 2);

            Assert.AreEqual(rGroupQuery.GetAllConfigurations().Count(), 8);

            //Test correctness AAL lines
            foreach (var at in rGroupQuery.GetRgroupQueryAtoms(1))
            {
                if (at is PseudoAtom)
                {
                    Assert.AreEqual(((PseudoAtom)at).Label, "R1");

                    var apoBonds = rGroupQuery.RootAttachmentPoints[at];
                    Assert.AreEqual(apoBonds.Count, 2);

                    var boundAtom1 = apoBonds[1].GetOther(at);
                    Assert.IsTrue(boundAtom1.Symbol.Equals("Te") || boundAtom1.Symbol.Equals("S"));

                    var boundAtom2 = apoBonds[2].GetOther(at);
                    Assert.IsTrue(boundAtom2.Symbol.Equals("Po") || boundAtom2.Symbol.Equals("O"));
                }
            }

            // Test that there only two Rgroup query atoms (R#). The third R is a
            // pseudo atom, but because it is not numbered it is not part of any
            // query condition.
            var allrGroupQueryAtoms = rGroupQuery.GetAllRgroupQueryAtoms();
            Assert.AreEqual(allrGroupQueryAtoms.Count, 2);
        }

        /// <summary>
        /// Test parsing of RGFile rgfile.4.mol.
        /// This R-group query has its R# atom detached, no bounds.
        /// </summary>
        [TestMethod()]
        public void TestRgroupQueryFile4()
        {
            var filename = "NCDK.Data.MDL.rgfile.4.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new RGroupQueryReader(ins);
            var rGroupQuery = reader.Read(new RGroupQuery());
            reader.Close();
            Assert.IsNotNull(rGroupQuery);
            Assert.AreEqual(rGroupQuery.RGroupDefinitions.Count, 1);
            Assert.AreEqual(rGroupQuery.RootStructure.Atoms.Count, 6);

            var allrGroupQueryAtoms = rGroupQuery.GetAllRgroupQueryAtoms();
            Assert.AreEqual(allrGroupQueryAtoms.Count, 1);
            var rList = rGroupQuery.RGroupDefinitions[1];
            Assert.AreEqual(rList.RGroups.Count, 2);
            Assert.AreEqual(rList.RequiredRGroupNumber, 0);
            Assert.IsFalse(rList.IsRestH);
            Assert.AreEqual(rGroupQuery.RootAttachmentPoints.Count, 0);
            Assert.IsTrue(rGroupQuery.AreSubstituentsDefined());

            Assert.AreEqual(rGroupQuery.GetAllConfigurations().Count(), 2);

            // This query has a detached R-group, test for empty attachment points
            var rGroups = rList.RGroups;
            Assert.AreEqual(rGroups[0].FirstAttachmentPoint, null);
            Assert.AreEqual(rGroups[0].SecondAttachmentPoint, null);
            Assert.AreEqual(rGroups[1].FirstAttachmentPoint, null);
            Assert.AreEqual(rGroups[1].SecondAttachmentPoint, null);
        }

        /// <summary>
        /// Test parsing of RGFile rgfile.5.mol.
        /// This exotic R-group query files has many R# groups and subsitutes,
        /// to test mainly for getting all valid configurations.
        /// </summary>
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestRgroupQueryFile5()
        {
            var filename = "NCDK.Data.MDL.rgfile.5.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new RGroupQueryReader(ins);
            var rGroupQuery = reader.Read(new RGroupQuery());
            reader.Close();
            Assert.IsNotNull(rGroupQuery);
            Assert.AreEqual(rGroupQuery.RGroupDefinitions.Count, 4);

            //Test combinatorial explosion: R5 has many different configurations
            Assert.AreEqual(rGroupQuery.GetAllConfigurations().Count(), 17820);
        }

        /// <summary>
        /// Test parsing of RGFile rgfile.6.mol.
        /// This RGFile is incomplete, RGP lines are missing. We still want to
        /// accept it (Symyx/ChemAxon software accepts it too).
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(CDKException))]
        public void TestRgroupQueryFile6()
        {
            var filename = "NCDK.Data.MDL.rgfile.6.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new RGroupQueryReader(ins);
            var rGroupQuery = reader.Read(new RGroupQuery());
            reader.Close();
            Assert.IsNotNull(rGroupQuery);
            Assert.AreEqual(rGroupQuery.RGroupDefinitions.Count, 3);
            Assert.AreEqual(rGroupQuery.RootStructure.Atoms.Count, 14);

            // This file has missing $RGP blocks. You could argue that this is
            // thus not a legal query (ie missing query specifications)
            Assert.IsFalse(rGroupQuery.AreSubstituentsDefined());

            //Getting for all configurations won't happen, because not all groups were set
            rGroupQuery.GetAllConfigurations(); // Will raise exception
        }

        /// <summary>
        /// Test parsing of RGFile rgfile.7.mol.
        /// This RGFile has APO lines with value 3: both attachment points.
        /// <para>
        /// Also, R32 appears twice, but with different numbers of attachment.
        /// The parser should not trip over this, and make nice configurations.
        /// </para>
        /// </summary>
        [TestMethod()]
        public void TestRgroupQueryFile7()
        {
            var filename = "NCDK.Data.MDL.rgfile.7.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new RGroupQueryReader(ins);
            var rGroupQuery = reader.Read(new RGroupQuery());
            reader.Close();
            Assert.IsNotNull(rGroupQuery);
            Assert.AreEqual(rGroupQuery.RGroupDefinitions.Count, 1);
            Assert.AreEqual(rGroupQuery.RootStructure.Atoms.Count, 9);
            Assert.AreEqual(rGroupQuery.GetAllConfigurations().Count(), 20);
        }
    }
}
