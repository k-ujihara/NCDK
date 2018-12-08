using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Silent;
using NCDK.IO;
using System;

namespace NCDK.Graphs.Matrix
{
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class TopologicalMatrixTest 
        : CDKTestCase
    {
        private static readonly IChemObjectBuilder builder = CDK.Builder;

        [Ignore()] // not actually asserting anything
        [TestMethod()]
        public void TestTopologicalMatrix_IAtomContainer()
        {
            var filename = "NCDK.Data.MDL.chlorobenzene.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new MDLV2000Reader(ins, ChemObjectReaderMode.Strict);
            IAtomContainer container = (IAtomContainer)reader.Read(builder.NewAtomContainer());
            int[][] matrix = TopologicalMatrix.GetMatrix(container);
            Assert.AreEqual(12, matrix.Length);
            for (int i = 0; i < matrix.Length; i++)
            {
                Console.Out.WriteLine("");

                for (int j = 0; j < matrix.Length; j++)
                {
                    Console.Out.Write(matrix[i][j] + " ");
                }
            }
            Console.Out.WriteLine();
        }
    }
}
