using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;

namespace NCDK.IO.Iterator
{
    /// <summary>
    /// TestCase for the reading MDL mol files using one test file.
    /// </summary>
    /// <seealso cref="MDLReader"/>
    // @cdk.module test-extra
    [TestClass()]
    public class IteratingMDLConformerReaderTest : CDKTestCase
    {
        [TestMethod()]
        public void TestSDF()
        {
            string filename = "NCDK.Data.MDL.iterconftest.sdf";
            Trace.TraceInformation("Testing: " + filename);
            Stream ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            IteratingMDLConformerReader reader = new IteratingMDLConformerReader(ins,
                    Default.ChemObjectBuilder.Instance);

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
            string filename = "NCDK.Data.MDL.iterconftest.sdf";
            Trace.TraceInformation("Testing: " + filename);
            Stream ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            var reader = new IteratingMDLConformerReader(ins, Default.ChemObjectBuilder.Instance).GetEnumerator();

            reader.MoveNext();
            var dummy = reader.Current;
            reader.Reset();
        }
    }
}
