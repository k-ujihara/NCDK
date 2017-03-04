/* Copyright (C) 2006-2007  Miguel Rojas <miguel.rojas@uni-koeln.de>
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
using NCDK.QSAR.Result;
using NCDK.Smiles;
using NCDK.Tools;
using NCDK.Tools.Manipulator;

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    /// TestSuite that runs all QSAR tests.
    /// </summary>
    // @cdk.module test-qsaratomic
    [TestClass()]
    public class IPAtomicHOSEDescriptorTest : AtomicDescriptorTest
    {
        private SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
        LonePairElectronChecker lpcheck = new LonePairElectronChecker();

        /// <summary>
        /// Constructor for the IPAtomicHOSEDescriptorTest object
        /// </summary>
        public IPAtomicHOSEDescriptorTest()
        {
            descriptor = new IPAtomicHOSEDescriptor();
            SetDescriptor(typeof(IPAtomicHOSEDescriptor));
        }

        [TestMethod()]
        public void TestIPAtomicHOSEDescriptor()
        {
            IAtomicDescriptor descriptor = new IPAtomicHOSEDescriptor();
            Assert.IsNotNull(descriptor);
        }

        /// <summary>
        /// A unit test for JUnit with CCCCl
        /// </summary>
        // @cdk.inchi InChI=1/C3H7Cl/c1-2-3-4/h2-3H2,1H3
        [TestMethod()]
        public void TestIPDescriptor1()
        {
            IAtomContainer mol = sp.ParseSmiles("CCCCl");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            lpcheck.Saturate(mol);

            double result = ((DoubleResult)descriptor.Calculate(mol.Atoms[3], mol).GetValue()).Value;
            double resultAccordingNIST = 10.8;

            Assert.AreEqual(resultAccordingNIST, result, 0.00001);
        }

        /// <summary>
        /// A unit test for JUnit with CC(C)Cl
        /// </summary>
        // @cdk.inchi InChI=1/C3H7Cl/c1-3(2)4/h3H,1-2H3
        [TestMethod()]
        public void TestIPDescriptor2()
        {
            IAtomContainer mol = sp.ParseSmiles("CC(CC)Cl"); // not in db
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            lpcheck.Saturate(mol);

            double result = ((DoubleResult)descriptor.Calculate(mol.Atoms[4], mol).GetValue()).Value;
            double resultAccordingNIST = 10.57; //value for CC(C)Cl

            Assert.AreEqual(resultAccordingNIST, result, 0.00001);
        }

        /// <summary>
        /// A unit test for JUnit with C=CCCl
        /// </summary>
        // @cdk.inchi InChI=1/C3H5Cl/c1-2-3-4/h2H,1,3H2
        [TestMethod()]
        public void TestNotDB()
        {
            IAtomContainer mol = sp.ParseSmiles("C=CCCl"); // not in db
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            lpcheck.Saturate(mol);

            double result = ((DoubleResult)descriptor.Calculate(mol.Atoms[3], mol).GetValue()).Value;
            double resultAccordingNIST = 10.8; //value for CCCCl aprox.

            Assert.AreEqual(resultAccordingNIST, result, 0.00001);
        }

        /// <summary>
        /// A unit test for JUnit with C-Cl
        /// </summary>
        // @cdk.inchi InChI=1/CH3F/c1-2/h1H3
        [TestMethod()]
        public void TestIPDescriptor_1()
        {
            IAtomContainer mol = sp.ParseSmiles("C-Cl");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            lpcheck.Saturate(mol);

            double result = ((DoubleResult)descriptor.Calculate(mol.Atoms[1], mol).GetValue()).Value;
            double resultAccordingNIST = 11.26;

            Assert.AreEqual(resultAccordingNIST, result, 0.42);
        }

        /// <summary>
        /// A unit test for JUnit with C-C-Br
        /// </summary>
        [TestMethod()]
        public void TestIPDescriptor_2()
        {

            IAtomContainer mol = sp.ParseSmiles("C-C-Br");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            lpcheck.Saturate(mol);

            double result = ((DoubleResult)descriptor.Calculate(mol.Atoms[2], mol).GetValue()).Value;
            double resultAccordingNIST = 11.29;

            Assert.AreEqual(resultAccordingNIST, result, 1.95);
        }

        /// <summary>
        /// A unit test for JUnit with C-C-C-I
        /// </summary>
        [TestMethod()]
        public void TestIPDescriptor_3()
        {
            IAtomContainer mol = sp.ParseSmiles("C-C-C-I");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            lpcheck.Saturate(mol);

            double result = ((DoubleResult)descriptor.Calculate(mol.Atoms[3], mol).GetValue()).Value;
            double resultAccordingNIST = 9.27;

            Assert.AreEqual(resultAccordingNIST, result, 0.02);
        }
        //    /// <summary>
        //    ///  A unit test for JUnit with C-C-O
        //    ///
        //    ///  @cdk.inchi InChI=1/C2H6O/c1-2-3/h3H,2H2,1H3
        //    /// </summary>
        //    [TestMethod()]
        //    public void TestIPDescriptor_4() throws Exception{
        //
        //        IAtomContainer mol = sp.ParseSmiles("C-C-O");
        //
        //        AddExplicitHydrogens(mol);
        //
        //        LonePairElectronChecker lpcheck = new LonePairElectronChecker();
        //        lpcheck.Saturate(mol);
        //
        //        double result= ((DoubleResult)descriptor.Calculate(mol.Atoms[2], mol).GetValue()).Value;
        //        double resultAccordingNIST = 10.48;
        //
        //        Assert.AreEqual(resultAccordingNIST, result, 1.24);
        //    }
        //
        //    /// <summary>
        //    ///  A unit test for JUnit with N1(C)CCC(C)(C)CC1
        //    ///
        //    /// </summary>
        //    [TestMethod()]
        //    public void TestIPDescriptor_5() throws Exception{
        //
        //        IAtomContainer mol = sp.ParseSmiles("N1(C)CCC(C)(C)CC1");
        //
        //        AddExplicitHydrogens(mol);
        //
        //        LonePairElectronChecker lpcheck = new LonePairElectronChecker();
        //        lpcheck.Saturate(mol);
        //
        //        double result= ((DoubleResult)descriptor.Calculate(mol.Atoms[0], mol).GetValue()).Value;
        //        double resultAccordingNIST = 7.77;
        //
        //        Assert.AreEqual(resultAccordingNIST, result, 0.02);
        //    }
        //    /// <summary>
        //    ///  A unit test for JUnit with C-N-C
        //    ///
        //    ///  @cdk.inchi InChI=1/C2H7N/c1-3-2/h3H,1-2H3
        //    /// </summary>
        //    [TestMethod()]
        //    public void TestIPDescriptor_6() throws Exception{
        //
        //        IAtomContainer mol = sp.ParseSmiles("C-N-C");
        //
        //        AddExplicitHydrogens(mol);
        //
        //        LonePairElectronChecker lpcheck = new LonePairElectronChecker();
        //        lpcheck.Saturate(mol);
        //
        //        double result= ((DoubleResult)descriptor.Calculate(mol.Atoms[1],mol).GetValue()).Value;
        //        double resultAccordingNIST = 8.24;
        //
        //        Assert.AreEqual(resultAccordingNIST, result, 0.09);
        //    }
        //    /// <summary>
        //    ///  A unit test for JUnit with C-C-N
        //    ///
        //    ///  @cdk.inchi InChI=1/C2H7N/c1-2-3/h2-3H2,1H3
        //    /// </summary>
        //    [TestMethod()]
        //    public void TestIPDescriptor_7() throws Exception{
        //
        //        IAtomContainer mol = sp.ParseSmiles("C-C-N");
        //
        //        AddExplicitHydrogens(mol);
        //
        //        LonePairElectronChecker lpcheck = new LonePairElectronChecker();
        //        lpcheck.Saturate(mol);
        //
        //        double result= ((DoubleResult)descriptor.Calculate(mol.Atoms[2],mol).GetValue()).Value;
        //        double resultAccordingNIST = 8.9;
        //
        //        Assert.AreEqual(resultAccordingNIST, result, 0.35);
        //    }
        //    /// <summary>
        //    ///  A unit test for JUnit with C-C-P-C-C
        //    ///
        //    ///  @cdk.inchi InChI=1/C4H11P/c1-3-5-4-2/h5H,3-4H2,1-2H3
        //    /// </summary>
        //    [TestMethod()]
        //    public void TestIPDescriptor_8() throws Exception{
        //
        //        IAtomContainer mol = sp.ParseSmiles("C-C-P-C-C");
        //
        //        AddExplicitHydrogens(mol);
        //
        //        LonePairElectronChecker lpcheck = new LonePairElectronChecker();
        //        lpcheck.Saturate(mol);
        //
        //        double result= ((DoubleResult)descriptor.Calculate(mol.Atoms[2], mol).GetValue()).Value;
        //        double resultAccordingNIST = 8.5;
        //
        //        Assert.AreEqual(resultAccordingNIST, result, 0.051);
        //    }
        //
        //    /// <summary>
        //    ///  A unit test for JUnit with O=C(C)CC(C)C
        //    ///
        //    ///  @cdk.inchi InChI=1/C6H12O/c1-5(2)4-6(3)7/h5H,4H2,1-3H3
        //    /// </summary>
        //    [TestMethod()]
        //    public void TestIPDescriptor_9() throws Exception{
        //
        //        IAtomContainer mol = sp.ParseSmiles("O=C(C)CC(C)C");
        //
        //        AddExplicitHydrogens(mol);
        //
        //        LonePairElectronChecker lpcheck = new LonePairElectronChecker();
        //        lpcheck.Saturate(mol);
        //
        //        double result= ((DoubleResult)descriptor.Calculate(mol.Atoms[0], mol).GetValue()).Value;
        //        double resultAccordingNIST = 9.3;
        //
        //        Assert.AreEqual(resultAccordingNIST, result, 0.051);
        //    }
        //    /// <summary>
        //    ///  A unit test for JUnit with O=C1C2CCC1CC2
        //    ///
        //    ///  @cdk.inchi InChI=1/C7H10O/c8-7-5-1-2-6(7)4-3-5/h5-6H,1-4H2
        //    /// </summary>
        //    [TestMethod()]
        //    public void TestIPDescriptor_10() throws Exception{
        //
        //        IAtomContainer mol = sp.ParseSmiles("O=C1C2CCC1CC2");
        //
        //        AddExplicitHydrogens(mol);
        //
        //        LonePairElectronChecker lpcheck = new LonePairElectronChecker();
        //        lpcheck.Saturate(mol);
        //
        //        double result= ((DoubleResult)descriptor.Calculate(mol.Atoms[0],mol).GetValue()).Value;
        //        double resultAccordingNIST = 9.01;
        //
        //        Assert.AreEqual(resultAccordingNIST, result, 0.06);
        //    }
        //
        //    /// <summary>
        //    ///  A unit test for JUnit with CCOCCCO
        //    ///
        //    ///  @cdk.inchi InChI=1/C5H12O2/c1-2-7-5-3-4-6/h6H,2-5H2,1H3
        //    /// </summary>
        //    [TestMethod()]
        //    public void TestIPDescriptor_14() throws Exception{
        //
        //        IAtomContainer mol = sp.ParseSmiles("CCOCCCO");
        //
        //        AddExplicitHydrogens(mol);
        //
        //        LonePairElectronChecker lpcheck = new LonePairElectronChecker();
        //        lpcheck.Saturate(mol);
        //
        //        double result= ((DoubleResult)descriptor.Calculate(mol.Atoms[2], mol).GetValue()).Value;
        ////        Assert.IsNotNull(result);
        //
        //        result= ((DoubleResult)descriptor.Calculate(mol.Atoms[7], mol).GetValue()).Value;
        ////        Assert.IsNotNull(result);
        //
        //    }
        //    /// <summary>
        //    /// A unit test for JUnit with C-C-N
        //    ///
        //    ///  @cdk.inchi  InChI=1/C2H7N/c1-2-3/h2-3H2,1H3
        //    ///
        //    // @throws ClassNotFoundException
        //    // @throws CDKException
        //    // @throws java.lang.Exception
        //    /// </summary>
        //    [TestMethod()]
        //    public void TestIPDescriptorReaction() throws Exception{
        //
        //        IAtomContainer mol = sp.ParseSmiles("C-C-N");
        //        AreEqual(3, mol.Atoms.Count);
        //        AddExplicitHydrogens(mol);
        //        AreEqual(10, mol.Atoms.Count);
        //
        //        LonePairElectronChecker lpcheck = new LonePairElectronChecker();
        //        lpcheck.Saturate(mol);
        //        AreEqual("Unexpected number of lone pairs", 1, mol.GetLonePairCount());
        //
        //        AreEqual("N", mol.Atoms[2].Symbol);
        //        descriptor.Calculate(mol.Atoms[2], mol);
        //        IReactionSet reactionSet = descriptor.ReactionSet;
        //
        //        IsNotNull("No reaction was found", reactionSet.GetReaction(0));
        //        IsNotNull("The ionization energy was not set for the reaction", reactionSet.GetReaction(0).GetProperty<>("IonizationEnergy"));
        //        double result = ((Double) reactionSet.GetReaction(0).GetProperty<>("IonizationEnergy")).Value;
        //        double resultAccordingNIST = 8.9;
        //
        //        Assert.AreEqual(1, reactionSet.GetReactionCount());
        //        Assert.AreEqual(resultAccordingNIST, result, 0.5);
        //    }
        //    /// <summary>
        //    /// A unit test for JUnit with CCCCCC
        //    ///
        //    ///  @cdk.inchi InChI=1/C6H14/c1-3-5-6-4-2/h3-6H2,1-2H3
        //    ///
        //    // @throws ClassNotFoundException
        //    // @throws CDKException
        //    // @throws java.lang.Exception
        //    /// </summary>
        //    [TestMethod()]
        //    public void TestIPDescriptorReaction2() throws Exception{
        //
        //        SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
        //        IAtomContainer mol = sp.ParseSmiles("CCCCCC");
        //
        //        AddExplicitHydrogens(mol);
        //
        //        LonePairElectronChecker lpcheck = new LonePairElectronChecker();
        //        lpcheck.Saturate(mol);
        //
        //        descriptor.Calculate(mol.Atoms[0], mol);
        //        IReactionSet reactionSet = descriptor.ReactionSet;
        //
        //        Assert.AreEqual(0, reactionSet.GetReactionCount());
        //    }
        //
        //    /// <summary>
        //    /// A unit test for JUnit with O(C=CC=C)C
        //    ///
        //    ///  @cdk.inchi InChI=1/C5H8O/c1-3-4-5-6-2/h3-5H,1H2,2H3
        //    ///
        //    // @throws ClassNotFoundException
        //    // @throws CDKException
        //    // @throws java.lang.Exception
        //    /// </summary>
        //    [TestMethod()]
        //    public void TestIPPySystemWithHeteroatomDescriptor3() throws Exception{
        //
        //        SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
        //        IAtomContainer mol = sp.ParseSmiles("O(C=CC=C)C");
        //
        //        AddExplicitHydrogens(mol);
        //
        //        LonePairElectronChecker lpcheck = new LonePairElectronChecker();
        //        lpcheck.Saturate(mol);
        //
        //        double result= ((DoubleResult)descriptor.Calculate(mol.Atoms[0],mol).GetValue()).Value;
        //        double resultAccordingNIST = 8.03;
        //        Assert.AreEqual(resultAccordingNIST, result, 0.11);
        //
        //        IReactionSet reactionSet = descriptor.ReactionSet;
        //        AreEqual(5, reactionSet.GetReactionCount());
        //
        //    }
        //    /// <summary>
        //    /// A unit test for JUnit with OC=CC
        //    ///
        //    ///  @cdk.inchi InChI=1/C3H6O/c1-2-3-4/h2-4H,1H3
        //    ///
        //    // @throws ClassNotFoundException
        //    // @throws CDKException
        //    // @throws java.lang.Exception
        //    /// </summary>
        //    [TestMethod()]
        //    public void TestIPPySystemWithHeteroatomDescriptor2() throws Exception{
        //
        //        SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
        //        IAtomContainer mol = sp.ParseSmiles("OC=CC");
        //
        //        AddExplicitHydrogens(mol);
        //
        //        LonePairElectronChecker lpcheck = new LonePairElectronChecker();
        //        lpcheck.Saturate(mol);
        //
        //        double result= ((DoubleResult)descriptor.Calculate(mol.Atoms[0],mol).GetValue()).Value;
        //        double resultAccordingNIST = 8.64;
        //        Assert.AreEqual(resultAccordingNIST, result, 0.21);
        //
        //        IReactionSet reactionSet = descriptor.ReactionSet;
        //        AreEqual(3, reactionSet.GetReactionCount());
        //
        //    }
        //    /// <summary>
        //    /// A unit test for JUnit with C1=C(C)CCS1
        //    ///
        //    ///  @cdk.inchi  InChI=1/C5H8S/c1-5-2-3-6-4-5/h4H,2-3H2,1H3
        //    ///
        //    // @throws ClassNotFoundException
        //    // @throws CDKException
        //    // @throws java.lang.Exception
        //    /// </summary>
        //    [TestMethod()]
        //    public void TestIPPySystemWithHeteroatomDescriptor1() throws Exception{
        //
        //        SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
        //        IAtomContainer mol = sp.ParseSmiles("C1=C(C)CCS1");
        //
        //        AddExplicitHydrogens(mol);
        //
        //        LonePairElectronChecker lpcheck = new LonePairElectronChecker();
        //        lpcheck.Saturate(mol);
        //
        //        double result= ((DoubleResult)descriptor.Calculate(mol.Atoms[5],mol).GetValue()).Value;
        //        double resultAccordingNIST = 7.77;
        //        Assert.AreEqual(resultAccordingNIST, result, 0.3);
        //
        //        IReactionSet reactionSet = descriptor.ReactionSet;
        //        AreEqual(3, reactionSet.GetReactionCount());
        //
        //    }
        //
        //    /// <summary>
        //    /// A unit test for JUnit with OC(C#CC)(C)C
        //    ///
        //    ///  @cdk.inchi InChI=1/C6H10O/c1-4-5-6(2,3)7/h7H,1-3H3
        //    ///
        //    // @throws ClassNotFoundException
        //    // @throws CDKException
        //    // @throws java.lang.Exception
        //    /// </summary>
        //    [TestMethod()]
        //    public void TestIDescriptor5() throws Exception{
        //
        //        SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
        //        IAtomContainer mol = sp.ParseSmiles("OC(C#CC)(C)C");
        //
        //        AddExplicitHydrogens(mol);
        //
        //        LonePairElectronChecker lpcheck = new LonePairElectronChecker();
        //        lpcheck.Saturate(mol);
        //
        //        descriptor.Calculate(mol.Atoms[0],mol);
        //
        //        IReactionSet reactionSet = descriptor.ReactionSet;
        //        AreEqual(1, reactionSet.GetReactionCount());
        //
        //    }
        //
    }
}
