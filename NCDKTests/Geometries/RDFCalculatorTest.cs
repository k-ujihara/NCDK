/* Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.Default;
using NCDK.Numerics;

namespace NCDK.Geometries
{
    /// <summary>
    /// This class defines regression tests that should ensure that the source code of the geometry.RDFCalculator is not broken.
    /// </summary>
    /// <seealso cref="RDFCalculator"/>
    // @cdk.module    test-extra
    // @author        Egon Willighagen
    // @cdk.created   2005-01-12
    [TestClass()]
    public class RDFCalculatorTest : CDKTestCase
    {
        [TestMethod()]
        public void TestRDFCalculator_double_double_double_double()
        {
            RDFCalculator calculator = new RDFCalculator(0.0, 5.0, 0.1, 0.0);

            Assert.IsNotNull(calculator);
        }

        class RDFWeightFunction : IRDFWeightFunction
        {
            public double Calculate(IAtom atom, IAtom atom2) => 1;
        }

        [TestMethod()]
        public void TestRDFCalculator_double_double_double_double_RDFWeightFunction()
        {
            RDFCalculator calculator = new RDFCalculator(0.0, 5.0, 0.1, 0.0, new RDFWeightFunction());
            Assert.IsNotNull(calculator);
        }

        [TestMethod()]
        public void TestCalculate()
        {
            RDFCalculator calculator = new RDFCalculator(0.0, 5.0, 0.1, 0.0);
            AtomContainer h2mol = new AtomContainer();
            Atom h1 = new Atom("H");
            h1.Point3D = new Vector3(-0.5, 0.0, 0.0);
            Atom h2 = new Atom("H");
            h2.Point3D = new Vector3(0.5, 0.0, 0.0);
            h2mol.Atoms.Add(h1);
            h2mol.Atoms.Add(h2);

            double[] rdf1 = calculator.Calculate(h2mol, h1);
            double[] rdf2 = calculator.Calculate(h2mol, h2);

            // test whether the double array length is ok
            Assert.AreEqual(51, rdf1.Length);

            // test whether the RDFs are identical
            Assert.AreEqual(rdf1.Length, rdf2.Length);
            for (int i = 0; i < rdf1.Length; i++)
            {
                Assert.AreEqual(rdf1[i], rdf2[i], 0.00001);
            }
        }

        [TestMethod()]
        public void TestCalculate_RDFWeightFunction()
        {
            RDFCalculator calculator = new RDFCalculator(0.0, 5.0, 0.1, 0.0, new RDFWeightFunction());
            AtomContainer h2mol = new AtomContainer();
            Atom h1 = new Atom("H");
            h1.Point3D = new Vector3(-0.5, 0.0, 0.0);
            Atom h2 = new Atom("H");
            h2.Point3D = new Vector3(0.5, 0.0, 0.0);
            h2mol.Atoms.Add(h1);
            h2mol.Atoms.Add(h2);

            double[] rdf1 = calculator.Calculate(h2mol, h1);
            double[] rdf2 = calculator.Calculate(h2mol, h2);

            // test whether the double array length is ok
            Assert.AreEqual(51, rdf1.Length);

            // test whether the RDFs are identical
            Assert.AreEqual(rdf1.Length, rdf2.Length);
            for (int i = 0; i < rdf1.Length; i++)
            {
                Assert.AreEqual(rdf1[i], rdf2[i], 0.00001);
            }
        }

        class RDFWeightFunctionCmulC : IRDFWeightFunction
        {
            public double Calculate(IAtom atom, IAtom atom2) => atom.Charge.Value * atom2.Charge.Value;
        }

        [TestMethod()]
        public void TestCalculate_RDFWeightFunction2()
        {
            RDFCalculator calculator = new RDFCalculator(0.0, 5.0, 0.1, 0.0, new RDFWeightFunctionCmulC());
            AtomContainer h2mol = new AtomContainer();
            Atom h1 = new Atom("H");
            h1.Point3D = new Vector3(-0.5, 0.0, 0.0);
            h1.Charge = +1.0;
            Atom h2 = new Atom("H");
            h2.Point3D = new Vector3(0.5, 0.0, 0.0);
            h2.Charge = -1.0;
            h2mol.Atoms.Add(h1);
            h2mol.Atoms.Add(h2);

            double[] rdf1 = calculator.Calculate(h2mol, h1);
            double[] rdf2 = calculator.Calculate(h2mol, h2);

            // test whether the double array length is ok
            Assert.AreEqual(51, rdf1.Length);

            // test whether the RDFs are identical
            Assert.AreEqual(rdf1.Length, rdf2.Length);
            for (int i = 0; i < rdf1.Length; i++)
            {
                Assert.AreEqual(rdf1[i], rdf2[i], 0.00001);
            }
        }

        [TestMethod()]
        public void TestCalculate_With_Gauss()
        {
            RDFCalculator calculator = new RDFCalculator(0.0, 5.0, 0.1, 0.3, new RDFWeightFunctionCmulC());
            AtomContainer h2mol = new AtomContainer();
            Atom h1 = new Atom("H");
            h1.Point3D = new Vector3(-0.5, 0.0, 0.0);
            h1.Charge = +1.0;
            Atom h2 = new Atom("H");
            h2.Point3D = new Vector3(0.5, 0.0, 0.0);
            h2.Charge = -1.0;
            h2mol.Atoms.Add(h1);
            h2mol.Atoms.Add(h2);

            double[] rdf1 = calculator.Calculate(h2mol, h1);
            double[] rdf2 = calculator.Calculate(h2mol, h2);

            // test whether the double array length is ok
            Assert.AreEqual(51, rdf1.Length);

            // test whether the RDFs are identical
            Assert.AreEqual(rdf1.Length, rdf2.Length);
            for (int i = 0; i < rdf1.Length; i++)
            {
                Assert.AreEqual(rdf1[i], rdf2[i], 0.00001);
            }
        }
    }
}
