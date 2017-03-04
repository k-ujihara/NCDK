/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *  All I ask is that proper credit is given for my work, which includes
 *  - but is not limited to - adding the above copyright notice to the beginning
 *  of your source code files, and to any copyright notice that you may distribute
 *  with programs based on this work.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.IO;

using System;

namespace NCDK.Tools
{
    /// <summary>
    /// Tests the HOSECode genertor.
    /// </summary>
    // @cdk.module test-extra
    // @author     steinbeck
    // @cdk.created    2002-11-16
    [TestClass()]
    public class BremserPredictorTest : CDKTestCase
    {
        static bool standAlone = false;

        [TestMethod()]
        public void TestConstructor()
        {
            BremserOneSphereHOSECodePredictor bp = new BremserOneSphereHOSECodePredictor();
            Assert.IsNotNull(bp);
        }

        /// <summary>
        ///  A unit test for JUnit
        ///
        /// <returns>Description of the Return Value</returns>
        /// </summary>
        [TestMethod()]
        public void TestPrediction()
        {
            var data = new[] {"=C(//)", "=OCC(//)", "CC(//)", "CC(//)", "CCC(//)", "CC(//)", "CC(//)", "CCC(//)", "CCC(//)",
                "CC(//)", "CC(//)", "CC(//)", "CC(//)", "CCO(//)", "CC(//)", "CCO(//)", "CCO(//)", "CC(//)", "O(//)",
                "CC(//)", "CCC(//)", "CCC(//)", "CCC(//)"};

            double[] result = {112.6, 198.6, 29.6, 29.6, 40.1, 29.6, 29.6, 40.1, 40.1, 29.6, 29.6, 29.6, 29.6, 73.1, 29.6,
                73.1, 73.1, 29.6, 54.7, 29.6, 40.1, 40.1, 40.1};

            double prediction;
            BremserOneSphereHOSECodePredictor bp = new BremserOneSphereHOSECodePredictor();
            for (int f = 0; f < data.Length; f++)
            {
                prediction = bp.Predict(data[f]);
                //logger.debug("\"" + prediction + "\",");
                Assert.AreEqual(result[f], prediction, 0.001);
            }

        }

        /// <summary>
        ///  A unit test for JUnit
        ///
        /// <returns>Description of the Return Value</returns>
        /// </summary>
        [TestMethod()]
        public void TestGetConfidenceLimit()
        {
            double[] result = {28.5, 25.7, 28.5, 34.9, 28.5, 25.7, 25.4, 28.5, 28.5, 14.8, 13.3, 23.0, 34.9, 25.7, 25.7,
                28.5, 25.7, 25.7, 13.3, 14.4, 14.4, 8.9, 14.8, 14.8, 13.3, 13.3, 13.3, 14.4, 14.4, 13.3, 14.4, 14.4,
                8.9, 14.8, 14.8, 13.3, 13.3, 13.3, 14.4, 14.4, 13.3};
            IAtomContainer molecule = null;
            string filename = "NCDK.Data.MDL.BremserPredictionTest.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            molecule = reader.Read(new AtomContainer());
            double prediction;
            BremserOneSphereHOSECodePredictor bp = new BremserOneSphereHOSECodePredictor();
            HOSECodeGenerator hcg = new HOSECodeGenerator();
            string s = null;
            RemoveHydrogens(molecule);
            //logger.debug("Molecule has " + molecule.Atoms.Count + " atoms.");
            for (int f = 0; f < molecule.Atoms.Count; f++)
            {
                s = hcg.GetHOSECode(molecule, molecule.Atoms[f], 1);
                prediction = bp.GetConfidenceLimit(hcg.MakeBremserCompliant(s));
                //logger.debug("\"" + prediction + "\",");
                Assert.AreEqual(result[f], prediction, 0.001);
            }

        }

        [TestMethod()]
        public void TestFailure1()
        {
            bool correct = false;
            BremserOneSphereHOSECodePredictor bp = new BremserOneSphereHOSECodePredictor();
            try
            {
                bp.Predict("dumb code");
            }
            catch (Exception exc)
            {
                if (exc is CDKException)
                {
                    correct = true;
                }
            }
            Assert.IsTrue(correct);
        }

        [TestMethod()]
        public void TestFailure2()
        {
            bool correct = false;
            BremserOneSphereHOSECodePredictor bp = new BremserOneSphereHOSECodePredictor();
            try
            {
                bp.GetConfidenceLimit("dumb code");
            }
            catch (Exception exc)
            {
                if (exc is CDKException)
                {
                    correct = true;
                }
            }
            Assert.IsTrue(correct);
        }

        [TestMethod()]
        public void TestFailure3()
        {
            bool correct = false;
            string test = null;
            BremserOneSphereHOSECodePredictor bp = new BremserOneSphereHOSECodePredictor();
            try
            {
                bp.Predict(test);
            }
            catch (Exception exc)
            {
                if (exc is CDKException)
                {
                    correct = true;
                }
            }
            Assert.IsTrue(correct);
        }

        private void RemoveHydrogens(IAtomContainer ac)
        {
            IAtom atom = null;
            int f = ac.Atoms.Count - 1;

            do
            {
                atom = ac.Atoms[f];
                if (atom.Symbol.Equals("H"))
                {
                    ac.RemoveAtomAndConnectedElectronContainers(atom);
                }
                f--;
            } while (f >= 0);
        }
    }
}
