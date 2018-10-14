/* Copyright (c) 2018 Kazuya Ujihara <ujihara.kazuya@gmail.com>
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
using NCDK.QSAR.Results;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    [TestClass()]
    public class FractionalCSP3DescriptorTest : MolecularDescriptorTest
    {
        public FractionalCSP3DescriptorTest()
        {
            SetDescriptor(typeof(FractionalCSP3Descriptor));
        }

        struct E
        {
            public string Smiles;
            public double Value;
            public string Name;

            public E(string smiles, double value, string name = "")
            {
                this.Smiles = smiles;
                this.Value = value;
                this.Name = name;
            }
        };

        static readonly E[] table = new E[]
        {
            new E("HH", 0, "hydrogen"),
            new E("HOH", 0, "water"),
            new E("C1=CC=CC=C1", 0, "benzene"),
            new E("C1=CN=CC=C1", 0, "pyridine"),
            new E("CC1=CC=CC(C)=N1", 0.29, "dimethylpyridine"),
            new E("CC1CCCC(C)N1", 1, "methylpiperidine"),
            new E("CC1=NC(NC(NC2CN(C3=CC=CC(F)=C3)C(C2)=O)=O)=CC=C1", 0.24),
        };

        [TestMethod()]
        public void TestFractionalCSP3Descriptor()
        {
            var sp = CDK.SilentSmilesParser;
            foreach (var e in table)
            {
                var mol = sp.ParseSmiles(e.Smiles);
                Assert.AreEqual(e.Value, ((Result<double>)Descriptor.Calculate(mol).Value).Value, 0.01);
            }
        }
    }
}
