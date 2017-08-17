/* Copyright (C) 2009-2010 maclean {gilleain.torrance@gmail.com}
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
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace NCDK.Signatures
{
    // @cdk.module test-signature
    // @author maclean
    [TestClass()]
    public class AtomSignatureTest : AbstractSignatureTest
    {
        private IAtomContainer atomContainer;
        private AtomSignature atomSignature;

        public AtomSignatureTest()
        {
            atomContainer = builder.NewAtomContainer();
            atomContainer.Atoms.Add(builder.NewAtom("C"));
            atomContainer.Atoms.Add(builder.NewAtom("C"));
            atomContainer.AddBond(atomContainer.Atoms[0], atomContainer.Atoms[1], BondOrder.Double);
            atomSignature = new AtomSignature(0, atomContainer);
        }

        [TestMethod()]
        public void GetIntLabelTest()
        {
            atomContainer.Atoms[0].MassNumber = 12;
            Assert.AreEqual(12, atomSignature.GetIntLabel(0));
        }

        [TestMethod()]
        public void GetConnectedTest()
        {
            Assert.AreEqual(1, atomSignature.GetConnected(0)[0]);
        }

        [TestMethod()]
        public void GetEdgeLabelTest()
        {
            Assert.AreEqual("=", atomSignature.GetEdgeLabel(0, 1));
        }

        [TestMethod()]
        public void GetAromaticEdgeLabelTest()
        {
            IAtomContainer benzeneRing = builder.NewAtomContainer();
            for (int i = 0; i < 6; i++)
            {
                benzeneRing.Atoms.Add(builder.NewAtom("C"));
            }
            for (int i = 0; i < 6; i++)
            {
                IAtom a = benzeneRing.Atoms[i];
                IAtom b = benzeneRing.Atoms[(i + 1) % 6];
                IBond bond = builder.NewBond(a, b);
                benzeneRing.Bonds.Add(bond);
                bond.IsAromatic = true;
            }

            AtomSignature signature = new AtomSignature(0, benzeneRing);
            for (int i = 0; i < 6; i++)
            {
                Assert.AreEqual("p", signature.GetEdgeLabel(i, (i + 1) % 6), "Failed for " + i);
            }
        }

        [TestMethod()]
        public void GetVertexSymbolTest()
        {
            Assert.AreEqual("C", atomSignature.GetVertexSymbol(0));
        }

        //    [TestMethod()]
        //    public void IntegerInvariantsTest() {
        //        IAtomContainer isotopeChiralMol = builder.NewAtomContainer();
        //        isotopeChiralMol.Atoms.Add(builder.NewAtom("C"));
        //
        //        IAtom s32 = builder.NewAtom("S");
        //        s32.MassNumber = 32;
        //
        //        IAtom s33 = builder.NewAtom("S");
        //        s33.MassNumber = 33;
        //
        //        IAtom s34 = builder.NewAtom("S");
        //        s34.MassNumber = 34;
        //
        //        IAtom s36 = builder.NewAtom("S");
        //        s36.MassNumber = 36;
        //
        //        isotopeChiralMol.Atoms.Add(s36);
        //        isotopeChiralMol.Atoms.Add(s34);
        //        isotopeChiralMol.Atoms.Add(s33);
        //        isotopeChiralMol.Atoms.Add(s32);
        //
        //        isotopeChiralMol.AddBond(isotopeChiralMol.Atoms[0], isotopeChiralMol.Atoms[1], BondOrder.Single);
        //        isotopeChiralMol.AddBond(isotopeChiralMol.Atoms[0], isotopeChiralMol.Atoms[2], BondOrder.Single);
        //        isotopeChiralMol.AddBond(isotopeChiralMol.Atoms[0], isotopeChiralMol.Atoms[3], BondOrder.Single);
        //        isotopeChiralMol.AddBond(isotopeChiralMol.Atoms[0], isotopeChiralMol.Atoms[4], BondOrder.Single);
        //
        //        MoleculeSignature molSig = new MoleculeSignature(isotopeChiralMol);
        //        Console.Out.WriteLine(molSig.ToCanonicalString());
        //    }

        [TestMethod()]
        public void CuneaneCubaneHeightTest()
        {
            IAtomContainer cuneane = AbstractSignatureTest.MakeCuneane();
            IAtomContainer cubane = AbstractSignatureTest.MakeCubane();
            int height = 1;
            AtomSignature cuneaneSignature = new AtomSignature(0, height, cuneane);
            AtomSignature cubaneSignature = new AtomSignature(0, height, cubane);
            string cuneaneSigString = cuneaneSignature.ToCanonicalString();
            string cubaneSigString = cubaneSignature.ToCanonicalString();
            Assert.AreEqual(cuneaneSigString, cubaneSigString);
        }

        public void MoleculeIsCarbon3Regular(IAtomContainer molecule)
        {
            int i = 0;
            foreach (var a in molecule.Atoms)
            {
                int count = 0;
                foreach (var connected in molecule.GetConnectedAtoms(a))
                {
                    if (connected.Symbol.Equals("C"))
                    {
                        count++;
                    }
                }
                Assert.AreEqual(3, count, "Failed for atom " + i);
                i++;
            }
        }

        [TestMethod()]
        public void DodecahedraneHeightTest()
        {
            IAtomContainer dodecahedrane = AbstractSignatureTest.MakeDodecahedrane();
            MoleculeIsCarbon3Regular(dodecahedrane);
            int diameter = 5;
            for (int height = 0; height <= diameter; height++)
            {
                AllEqualAtHeightTest(dodecahedrane, height);
            }
        }

        [TestMethod()]
        public void AllHeightsOfASymmetricGraphAreEqualTest()
        {
            IAtomContainer cubane = MakeCubane();
            int diameter = 3;
            for (int height = 0; height <= diameter; height++)
            {
                AllEqualAtHeightTest(cubane, height);
            }
        }

        public void AllEqualAtHeightTest(IAtomContainer molecule, int height)
        {
            IDictionary<string, int> sigfreq = new Dictionary<string, int>();
            for (int i = 0; i < molecule.Atoms.Count; i++)
            {
                AtomSignature atomSignature = new AtomSignature(i, height, molecule);
                string canonicalSignature = atomSignature.ToCanonicalString();
                if (sigfreq.ContainsKey(canonicalSignature))
                {
                    sigfreq[canonicalSignature] = sigfreq[canonicalSignature] + 1;
                }
                else
                {
                    sigfreq[canonicalSignature] = 1;
                }
            }
            Assert.AreEqual(1, sigfreq.Keys.Count);
        }

        [TestMethod()]
        public void ConvertEdgeLabelToColorTest()
        {
            IAtomContainer ac = MakeBenzene(); // doesn't really matter
            AtomSignature atomSignature = new AtomSignature(0, ac);
            int aromaticColor = atomSignature.ConvertEdgeLabelToColor("p");
            Assert.IsTrue(aromaticColor > 0);
            int singleColor = atomSignature.ConvertEdgeLabelToColor("");
            int doubleColor = atomSignature.ConvertEdgeLabelToColor("=");
            int tripleColor = atomSignature.ConvertEdgeLabelToColor("#");
            Assert.IsTrue(singleColor < doubleColor);
            Assert.IsTrue(doubleColor < tripleColor);
        }
    }
}
