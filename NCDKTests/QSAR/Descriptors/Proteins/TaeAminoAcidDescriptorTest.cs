/* Copyright (C) 2004-2007  Rajarshi Guha <rajarshi@users.sourceforge.net>
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Tools;

namespace NCDK.QSAR.Descriptors.Proteins
{
    // @cdk.module test-qsarprotein
    [TestClass()]
    public class TaeAminoAcidDescriptorTest : DescriptorTest<TaeAminoAcidDescriptor>
    {
        protected override IAtomContainer DefaultMolecular { get; } = ProteinBuilderTool.CreateProtein("A", CDK.Builder);

        [TestMethod()]
        public void TestTaeAminoAcidDescriptor()
        {
            var pepseq = ProteinBuilderTool.CreateProtein("ACDEFGH", CDK.Builder);
            var result = CreateDescriptor(pepseq).Calculate();

            Assert.AreEqual(147, result.Values.Count);
        }
    }
}
