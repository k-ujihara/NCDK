/*
 * Copyright (c) 2013, European Bioinformatics Institute (EMBL-EBI)
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met: 
 *
 * 1. Redistributions of source code must retain the above copyright notice, this
 *    list of conditions and the following disclaimer. 
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution. 
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 * The views and conclusions contained in the software and documentation are those
 * of the authors and should not be interpreted as representing official policies, 
 * either expressed or implied, of the FreeBSD Project.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Beam
{
   /// <summary> <author>John May </author></summary>
    [TestClass()]
    public class ElectronAssignmentTest
    {
        [TestMethod()]
        public void BiphenylLike()
        {
            AssertAssignable("c1ccccc1-c1ccccc1");
            AssertUnassignable("c1ccccc1=c1ccccc1"); // n.b. daylight will assign this - for now we say it's invalid
            AssertAssignable("c1ccccc1c1ccccc1");
        }

        [TestMethod()]
        public void FulvaleneLike()
        {
            AssertAssignable("c1cccc1-c1cccc1");
            AssertAssignable("c1cccc1=c1cccc1");
            AssertAssignable("c1cccc1c1cccc1");
        }

        [TestMethod()]
        public void Cyclopentadiene()
        {
            AssertUnassignable("c1cccc1");
            AssertAssignable("c1cCcc1");
        }

        static void AssertAssignable(string smi)
        {
            Assert.IsTrue(ElectronAssignment.Verify(Graph.FromSmiles(smi)));
        }

        static void AssertUnassignable(string smi)
        {
            Assert.IsFalse(ElectronAssignment.Verify(Graph.FromSmiles(smi)));
        }
    }
}
