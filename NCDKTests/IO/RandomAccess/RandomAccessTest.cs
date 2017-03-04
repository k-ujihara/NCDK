/* Copyright (C) 2005-2008  Nina Jeliazkova <nina@acad.bg>
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
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;

namespace NCDK.IO.RandomAccess
{
    /// <summary>
    /// Test for <see cref="RandomAccessSDFReader"/>
    /// </summary>
    // @author Nina Jeliazkova <nina@acad.bg>
    // @cdk.module test-extra
    [TestClass()]
    public class RandomAccessTest : CDKTestCase
    {
        [TestMethod()]
        public void Test()
        {
            string path = "NCDK.Data.MDL.test2.sdf";
            Trace.TraceInformation("Testing: " + path);
            using (var ins = ResourceLoader.GetAsStream(path))
            {
                string f = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".sdf");
                try
                {
                    // copy data to tmp file
                    using (var out_ = new FileStream(f, FileMode.Create))
                    {
                        var buf = new byte[ins.Length];
                        ins.Read(buf, 0, buf.Length);
                        out_.Write(buf, 0, buf.Length);
                    }


                    //System.Console.Out.WriteLine(System.GetProperty("user.dir"));
                    RandomAccessReader rf = new RandomAccessSDFReader(f, Default.ChemObjectBuilder.Instance);
                    try
                    {
                        Assert.AreEqual(6, rf.Count);

                        string[] mdlnumbers = {"MFCD00000387", "MFCD00000661", "MFCD00000662", "MFCD00000663", "MFCD00000664",
                        "MFCD03453215"};
                        //reading backwards - just for the test
                        for (int i = rf.Count - 1; i >= 0; i--)
                        {
                            IChemObject m = rf.ReadRecord(i);
                            Assert.AreEqual(mdlnumbers[i], m.GetProperty<string>("MDLNUMBER"));
                            Assert.IsTrue(m is IAtomContainer);
                            Assert.IsTrue(((IAtomContainer)m).Atoms.Count > 0);
                        }
                    }
                    finally
                    {
                        if (rf != null) rf.Close();
                    }
                }
                finally
                {
                    File.Delete(f);
                }
            }
        }
    }
}
