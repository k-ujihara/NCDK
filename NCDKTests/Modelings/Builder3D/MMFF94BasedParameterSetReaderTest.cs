/* Copyright (C) 2012 Daniel Szisz
 *
 * Contact: orlando@caesar.elte.hu
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
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Common.Base;
using System.Collections.Generic;
using System.Globalization;


namespace NCDK.Modelings.Builder3D
{
    /// <summary>
    /// This class is for testing the MMFF94 based parameter reading in CDK.
    /// </summary>
    // @author danielszisz
    // @version 04/16/2012
    // @cdk.module test-forcefield
    [TestClass()]
    public class MMFF94BasedParameterSetReaderTest
    {
        [TestMethod()]
        public void TestReadParameterSets()
        {
            MMFF94BasedParameterSetReader mmff94bpsr = new MMFF94BasedParameterSetReader();
            mmff94bpsr.ReadParameterSets();
            var parameterSet = mmff94bpsr.GetParamterSet();

            //test atom type
            var atomtypes = mmff94bpsr.AtomTypes;
            IAtomType atomtype = atomtypes[0];
            string sid = "C";
            Assert.AreEqual(sid, atomtype.AtomTypeName);
            string rootType = "C";
            Assert.AreEqual(rootType, atomtype.Symbol);
            string smaxbond = "4";
            Assert.AreEqual(int.Parse(smaxbond, NumberFormatInfo.InvariantInfo), (int)atomtype.FormalNeighbourCount);
            string satomNr = "6";
            Assert.AreEqual(int.Parse(satomNr, NumberFormatInfo.InvariantInfo), (int)atomtype.AtomicNumber);

            //atom
            //TODO testing

            //bond
            //        string scode = "0";
            string sid1 = "C";
            string sid2 = "C";
            string slen = "1.508";
            string sk2 = "306.432";
            string sk3 = "-612.865";
            string sk4 = "715.009";
            string sbci = "0.0000";
            string bondkey = "bond" + sid1 + ";" + sid2;
            var bonddata = new List<double>
            {
                (double)(double.Parse(slen, NumberFormatInfo.InvariantInfo)),
                (double)(double.Parse(sk2, NumberFormatInfo.InvariantInfo)),
                (double)(double.Parse(sk3, NumberFormatInfo.InvariantInfo)),
                (double)(double.Parse(sk4, NumberFormatInfo.InvariantInfo)),
                (double)(double.Parse(sbci, NumberFormatInfo.InvariantInfo))
            };

            //strbnd
            //        scode = "0";
            sid1 = "C";
            sid2 = "C";
            string sid3 = "C";
            string value1 = "14.82507";
            string value2 = "14.82507";
            string strbndkey = "strbnd" + sid1 + ";" + sid2 + ";" + sid3;
            var strbnddata = new List<double>
            {
                (double)(double.Parse(value1, NumberFormatInfo.InvariantInfo)),
                (double)(double.Parse(value2, NumberFormatInfo.InvariantInfo))
            };

            //angle
            //      scode = "0";
            sid1 = "C=";
            sid2 = "C";
            sid3 = "N";
            value1 = "105.837";
            value2 = "86.1429";
            string value3 = "-34.5494";
            string value4 = "0";
            string anglekey = "angle" + sid1 + ";" + sid2 + ";" + sid3;
            var angledata = new List<double>
            {
                (double)(double.Parse(value1, NumberFormatInfo.InvariantInfo)),
                (double)(double.Parse(value2, NumberFormatInfo.InvariantInfo)),
                (double)(double.Parse(value3, NumberFormatInfo.InvariantInfo)),
                (double)(double.Parse(value4, NumberFormatInfo.InvariantInfo))
            };

            //torsion
            //        scode = "0";
            sid1 = "HC";
            sid2 = "C";
            sid3 = "C";
            string sid4 = "HC";
            value1 = "0.142";
            value2 = "0.693";
            value3 = "0.157";
            value4 = "0.000";
            string value5 = "0.000";
            string torsionkey = "torsion" + ";" + sid1 + ";" + sid2 + ";" + sid3 + ";" + sid4;
            var torsiondata = new List<double>
            {
                (double)(double.Parse(value1, NumberFormatInfo.InvariantInfo)),
                (double)(double.Parse(value2, NumberFormatInfo.InvariantInfo)),
                (double)(double.Parse(value3, NumberFormatInfo.InvariantInfo)),
                (double)(double.Parse(value4, NumberFormatInfo.InvariantInfo)),
                (double)(double.Parse(value5, NumberFormatInfo.InvariantInfo))
            };

            //opbend
            //      scode = "0";
            sid1 = "O=";
            sid2 = "C=";
            sid3 = "CR4R";
            sid4 = "CR4R";
            value1 = "10.86681780";
            string opbendkey = "opbend" + ";" + sid1 + ";" + sid2 + ";" + sid3 + ";" + sid4;
            var opbenddata = new List<double>
            {
                (double)(double.Parse(value1, NumberFormatInfo.InvariantInfo))
            };

            //TODO data lines testing

            foreach (var e in parameterSet)
            {
                if (e.Key.Equals(bondkey))
                    Assert.IsTrue(Compares.AreDeepEqual(bonddata, e.Value));
                else if (e.Key.Equals(strbndkey))
                    Assert.IsTrue(Compares.AreDeepEqual(strbnddata, e.Value));
                else if (e.Key.Equals(anglekey))
                    Assert.IsTrue(Compares.AreDeepEqual(angledata, e.Value));
                else if (e.Key.Equals(torsionkey))
                    Assert.IsTrue(Compares.AreDeepEqual(torsiondata, e.Value));
                else if (e.Key.Equals(opbendkey))
                    Assert.IsTrue(Compares.AreDeepEqual(opbenddata, e.Value));
            }
        }
    }
}
