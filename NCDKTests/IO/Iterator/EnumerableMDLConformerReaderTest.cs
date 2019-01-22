using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace NCDK.IO.Iterator
{
    /// <summary>
    /// TestCase for the reading MDL mol files using one test file.
    /// </summary>
    /// <seealso cref="MDLReader"/>
    // @cdk.module test-extra
    [TestClass()]
    public class EnumerableMDLConformerReaderTest : CDKTestCase
    {
        [TestMethod()]
        public void TestSDF()
        {
            var filename = "NCDK.Data.MDL.iterconftest.sdf";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new EnumerableMDLConformerReader(ins);
            
            int molCount = 0;
            int[] nconfs = new int[3];

            int i = 0;
            foreach (var confContainer in reader)
            {
                Assert.IsNotNull(confContainer);
                nconfs[i++] = confContainer.Count;
                molCount++;
            }

            Assert.AreEqual(3, molCount);
            Assert.AreEqual(3, nconfs[0]);
            Assert.AreEqual(18, nconfs[1]);
            Assert.AreEqual(18, nconfs[2]);
        }

        [TestMethod()]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestRemove()
        {
            var filename = "NCDK.Data.MDL.iterconftest.sdf";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new EnumerableMDLConformerReader(ins).GetEnumerator();

            reader.MoveNext();
            var dummy = reader.Current;
            reader.Reset();
        }
    }
}
