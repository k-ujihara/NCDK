/*
 * Copyright (c) 2018 John Mayfield <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or (at
 * your option) any later version. All we ask is that proper credit is given
 * for our work, which includes - but is not limited to - adding the above
 * copyright notice to the beginning of your source code files, and to any
 * copyright notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Config;
using NCDK.Isomorphisms.Matchers;
using NCDK.SMARTS;
using System.Linq;

namespace NCDK.Isomorphisms
{
    [TestClass()]
    public class SmartsExprWriteTest
    {
        static Expr GetExpr(ExprType type)
        {
            return new Expr(type);
        }

        static Expr GetExpr(ExprType type, int val)
        {
            return new Expr(type, val);
        }

        static Expr GetAnd(Expr a, Expr b)
        {
            return new Expr(ExprType.And, a, b);
        }

        static Expr GetOr(Expr a, Expr b)
        {
            return new Expr(ExprType.Or, a, b);
        }

        // C&r6 not Cr
        [TestMethod()]
        public void UseExplAnd1()
        {
            var expr = new Expr(ExprType.AliphaticElement, 6).And(new Expr(ExprType.RingSmallest, 6));
            Assert.AreEqual("[C&r6]", Smarts.GenerateAtom(expr));
        }

        // D&2 not D2
        [TestMethod()]
        public void UseExplAnd2()
        {
            var expr = new Expr(ExprType.Degree, 1).And(new Expr(ExprType.Isotope, 2));
            Assert.AreEqual("[D&2]", Smarts.GenerateAtom(expr));
        }

        // aromatic or aliphatic
        [TestMethod()]
        public void Carbon()
        {
            var expr = new Expr(ExprType.Element, 6);
            Assert.AreEqual("[#6]", Smarts.GenerateAtom(expr));
        }

        // helium can't be aromatic so we can always use the symbol
        [TestMethod()]
        public void Helium()
        {
            var expr = new Expr(ExprType.Element, 2);
            Assert.AreEqual("[He]", Smarts.GenerateAtom(expr));
        }

        [TestMethod()]
        public void Degree()
        {
            Assert.AreEqual("[D]", Smarts.GenerateAtom(GetExpr(ExprType.Degree, 1)));
            Assert.AreEqual("[D2]", Smarts.GenerateAtom(GetExpr(ExprType.Degree, 2)));
        }

        // can sometimes write just 'H' but a lot of effort to figure out when
        [TestMethod()]
        public void TotalHCount()
        {
            Assert.AreEqual("[H1]", Smarts.GenerateAtom(GetExpr(ExprType.TotalHCount, 1)));
            Assert.AreEqual("[H2]", Smarts.GenerateAtom(GetExpr(ExprType.TotalHCount, 2)));
        }

        [TestMethod()]
        public void Connectivity()
        {
            Assert.AreEqual("[X]", Smarts.GenerateAtom(GetExpr(ExprType.TotalDegree, 1)));
            Assert.AreEqual("[X2]", Smarts.GenerateAtom(GetExpr(ExprType.TotalDegree, 2)));
        }

        [TestMethod()]
        public void RingMembership()
        {
            Assert.AreEqual("[R]", Smarts.GenerateAtom(GetExpr(ExprType.IsInRing)));
            Assert.AreEqual("[!R]", Smarts.GenerateAtom(GetExpr(ExprType.IsInChain)));
        }

        [TestMethod()]
        public void RingCount()
        {
            Assert.AreEqual("[R2]", Smarts.GenerateAtom(GetExpr(ExprType.RingCount, 2)));
        }

        [TestMethod()]
        public void RingSmallest()
        {
            Assert.AreEqual("[r4]", Smarts.GenerateAtom(GetExpr(ExprType.RingSmallest, 4)));
        }

        [TestMethod()]
        public void Isotopes()
        {
            Assert.AreEqual("[13]", Smarts.GenerateAtom(GetExpr(ExprType.Isotope, 13)));
            Assert.AreEqual("[0]", Smarts.GenerateAtom(GetExpr(ExprType.HasUnspecifiedIsotope)));
            Assert.AreEqual("[!0]", Smarts.GenerateAtom(GetExpr(ExprType.HasIsotope)));
        }

        [TestMethod()]
        public void FormalCharges()
        {
            Assert.AreEqual("[-2]", Smarts.GenerateAtom(GetExpr(ExprType.FormalCharge, -2)));
            Assert.AreEqual("[-]", Smarts.GenerateAtom(GetExpr(ExprType.FormalCharge, -1)));
            Assert.AreEqual("[+0]", Smarts.GenerateAtom(GetExpr(ExprType.FormalCharge, 0)));
            Assert.AreEqual("[+]", Smarts.GenerateAtom(GetExpr(ExprType.FormalCharge, 1)));
            Assert.AreEqual("[+2]", Smarts.GenerateAtom(GetExpr(ExprType.FormalCharge, 2)));
        }

        [TestMethod()]
        public void Valence()
        {
            Assert.AreEqual("[v]", Smarts.GenerateAtom(GetExpr(ExprType.Valence, 1)));
            Assert.AreEqual("[v2]", Smarts.GenerateAtom(GetExpr(ExprType.Valence, 2)));
        }

        [TestMethod()]
        public void AtomicNum()
        {
            Assert.AreEqual("[#0]", Smarts.GenerateAtom(GetExpr(ExprType.Element, 0)));
            Assert.AreEqual("[#0]", Smarts.GenerateAtom(GetExpr(ExprType.AliphaticElement, 0)));
            Assert.AreEqual("[#0]", Smarts.GenerateAtom(GetExpr(ExprType.AromaticElement, 0)));
            Assert.AreEqual("[#1]", Smarts.GenerateAtom(GetExpr(ExprType.Element, 1)));
            Assert.AreEqual("[#1]", Smarts.GenerateAtom(GetExpr(ExprType.AliphaticElement, 1)));
            Assert.AreEqual("[#1]", Smarts.GenerateAtom(GetExpr(ExprType.AromaticElement, 1)));
            Assert.AreEqual("[He]", Smarts.GenerateAtom(GetExpr(ExprType.Element, 2)));
            Assert.AreEqual("[Li]", Smarts.GenerateAtom(GetExpr(ExprType.Element, 3)));
            Assert.AreEqual("[#6]", Smarts.GenerateAtom(GetExpr(ExprType.Element, 6)));
            Assert.AreEqual("[#7]", Smarts.GenerateAtom(GetExpr(ExprType.Element, 7)));
            Assert.AreEqual("[#8]", Smarts.GenerateAtom(GetExpr(ExprType.Element, 8)));
            Assert.AreEqual("F", Smarts.GenerateAtom(GetExpr(ExprType.Element, 9)));
            Assert.AreEqual("[Ne]", Smarts.GenerateAtom(GetExpr(ExprType.Element, 10)));
            Assert.AreEqual("[Na]", Smarts.GenerateAtom(GetExpr(ExprType.Element, 11)));
            Assert.AreEqual("[Mg]", Smarts.GenerateAtom(GetExpr(ExprType.Element, 12)));
            // Ds, Ts and Nh etc write as #<num>
            Assert.AreEqual("[#110]", Smarts.GenerateAtom(GetExpr(ExprType.Element, AtomicNumbers.Darmstadtium)));
            Assert.AreEqual("[#117]", Smarts.GenerateAtom(GetExpr(ExprType.Element, AtomicNumbers.Tennessine)));
            Assert.AreEqual("[#113]", Smarts.GenerateAtom(GetExpr(ExprType.Element, AtomicNumbers.Nihonium)));
        }

        // Ds, Ts and Nh etc can be ambiguous - we write anything above radon as
        // '#<num>'
        [TestMethod()]
        public void AtomicNumHighWeightElements()
        {
            Assert.AreEqual("[#110]", Smarts.GenerateAtom(GetExpr(ExprType.Element, AtomicNumbers.Darmstadtium)));
            Assert.AreEqual("[#117]", Smarts.GenerateAtom(GetExpr(ExprType.Element, AtomicNumbers.Tennessine)));
            Assert.AreEqual("[#113]", Smarts.GenerateAtom(GetExpr(ExprType.Element, AtomicNumbers.Nihonium)));
            Assert.AreEqual("[#110]", Smarts.GenerateAtom(GetExpr(ExprType.AliphaticElement, AtomicNumbers.Darmstadtium)));
            Assert.AreEqual("[#117]", Smarts.GenerateAtom(GetExpr(ExprType.AliphaticElement, AtomicNumbers.Tennessine)));
            Assert.AreEqual("[#113]", Smarts.GenerateAtom(GetExpr(ExprType.AliphaticElement, AtomicNumbers.Nihonium)));
        }

        [TestMethod()]
        public void AromaticElement()
        {
            Assert.AreEqual("c", Smarts.GenerateAtom(GetExpr(ExprType.AromaticElement, 6)));
            Assert.AreEqual("n", Smarts.GenerateAtom(GetExpr(ExprType.AromaticElement, 7)));
        }

        [TestMethod()]
        public void UseLowPrecedenceAnd()
        {
            var expr = new Expr(ExprType.Element, 8).And(new Expr(ExprType.Degree, 2).Or(new Expr(ExprType.Degree, 1)));
            Assert.AreEqual("[#8;D2,D]", Smarts.GenerateAtom(expr));
        }

        [TestMethod()]
        public void UseImplAnd()
        {
            var expr = new Expr(ExprType.AromaticElement, 7).And(new Expr(ExprType.Degree, 2).And(new Expr(ExprType.HasImplicitHydrogen)));
            Assert.AreEqual("[nD2h]", Smarts.GenerateAtom(expr));
        }

        // logical under a negate needs to be recursive
        [TestMethod()]
        public void UsrRecrNot()
        {
            var expr = new Expr(ExprType.Element, 9)
                .Or(new Expr(ExprType.Element, 17))
                .Or(new Expr(ExprType.Element, 35))
                .Negate();
            Assert.AreEqual("[!$([F,Cl,Br])]", Smarts.GenerateAtom(expr));
        }

        // or -> and -> or needs to be recursive
        [TestMethod()]
        public void UsrRecrOr()
        {
            var expr = GetOr(
                GetAnd(
                    GetOr(
                        GetExpr(ExprType.Element, 6),
                        GetExpr(ExprType.Element, 7)),
                    GetExpr(ExprType.IsInRing)),
                GetExpr(ExprType.IsAromatic));
            Assert.AreEqual("[$([#6,#7])R,a]", Smarts.GenerateAtom(expr));
        }

        [TestMethod()]
        public void SingleOrDouble()
        {
            var expr = new Expr(ExprType.AliphaticOrder, 1)
                   .Or(new Expr(ExprType.AliphaticOrder, 2));
            Assert.AreEqual("-,=", Smarts.GenerateBond(expr));
        }

        [TestMethod()]
        public void SingleOrDoubleInRing()
        {
            var expr = new Expr(ExprType.IsInRing)
                  .And(new Expr(ExprType.AliphaticOrder, 1)
                   .Or(new Expr(ExprType.AliphaticOrder, 2)));
            Assert.AreEqual("@;-,=", Smarts.GenerateBond(expr));
        }

        [TestMethod()]
        public void SingleOrDoubleInRing2()
        {
            var expr = new Expr(ExprType.IsInRing)
                  .And(new Expr(ExprType.SingleOrDouble));
            Assert.AreEqual("@;-,=", Smarts.GenerateBond(expr));
        }

        [TestMethod()]
        public void IndoleRoundTrip()
        {
            var mol = new QueryAtomContainer(null);
            Assert.IsTrue(Smarts.Parse(mol, "n1cnc2c1cccc2"));
            // CDK choice of data structures lose local arrangement but
            // output is still indole
            Assert.AreEqual("n1c2c(nc1)cccc2", Smarts.Generate(mol));
        }

        [TestMethod()]
        public void IndoleWithExprRoundTrip()
        {
            var mol = new QueryAtomContainer(null);
            Assert.IsTrue(Smarts.Parse(mol, "[n;$(*C),$(*OC)]1ccc2c1cccc2"));
            // CDK choice of data structures lose local arrangement but
            // output is still indole
            Assert.AreEqual("[n;$(*C),$(*OC)]1c2c(cc1)cccc2", Smarts.Generate(mol));
        }

        [TestMethod()]
        public void BondTrue()
        {
            var mol = new QueryAtomContainer(null);
            Assert.IsTrue(Smarts.Parse(mol, "C~C~N(~O)~O"));
            Assert.AreEqual("C~C~N(~O)~O", Smarts.Generate(mol));
        }

        [TestMethod()]
        public void BondFalse()
        {
            var mol = new QueryAtomContainer(null);
            Assert.IsTrue(Smarts.Parse(mol, "C!~C"));
            Assert.AreEqual("C!~C", Smarts.Generate(mol));
        }

        [TestMethod()]
        public void BondInChain()
        {
            var mol = new QueryAtomContainer(null);
            Assert.IsTrue(Smarts.Parse(mol, "C!@C"));
            Assert.AreEqual("C!@C", Smarts.Generate(mol));
        }

        [TestMethod()]
        public void BondInRing()
        {
            var mol = new QueryAtomContainer(null);
            Assert.IsTrue(Smarts.Parse(mol, "C@C"));
            Assert.AreEqual("C@C", Smarts.Generate(mol));
        }

        [TestMethod()]
        public void TripleBond()
        {
            var mol = new QueryAtomContainer(null);
            Assert.IsTrue(Smarts.Parse(mol, "C#C"));
            Assert.AreEqual("C#C", Smarts.Generate(mol));
        }

        [TestMethod()]
        public void NotTripleBond()
        {
            var mol = new QueryAtomContainer(null);
            Assert.IsTrue(Smarts.Parse(mol, "C!#C"));
            Assert.AreEqual("C!#C", Smarts.Generate(mol));
        }

        [TestMethod()]
        public void AromaticBond()
        {
            var mol = new QueryAtomContainer(null);
            Assert.IsTrue(Smarts.Parse(mol, "[#6]:[#6]"));
            Assert.AreEqual("[#6]:[#6]", Smarts.Generate(mol));
        }

        [TestMethod()]
        public void RingClosureExprs()
        {
            var mol = new QueryAtomContainer(null);
            Assert.IsTrue(Smarts.Parse(mol, "C1CCCC-,=1"));
            Assert.AreEqual("C1-,=CCCC1", Smarts.Generate(mol));
        }

        [TestMethod()]
        public void RingClosureExprs2()
        {
            var mol = new QueryAtomContainer(null);
            Assert.IsTrue(Smarts.Parse(mol, "C-,=1CCCC1"));
            Assert.AreEqual("C1-,=CCCC1", Smarts.Generate(mol));
        }

        [TestMethod()]
        public void RingClosureExprs3()
        {
            var mol = new QueryAtomContainer(null);
            Assert.IsTrue(Smarts.Parse(mol, "C1-,=CCCC1"));
            Assert.AreEqual("C1CCCC-,=1", Smarts.Generate(mol));
        }

        [TestMethod()]
        public void Reaction()
        {
            var mol = new QueryAtomContainer(null);
            Assert.IsTrue(Smarts.Parse(mol, "c1ccccc1[NH2]>>c1ccccc1N(~O)~O"));
            Assert.AreEqual("c1c(cccc1)[NH2]>>c1c(cccc1)N(~O)~O", Smarts.Generate(mol));
        }

        [TestMethod()]
        public void ReactionWithMaps()
        {
            var mol = new QueryAtomContainer(null);
            Assert.IsTrue(Smarts.Parse(mol, "c1cccc[c:1]1[NH2:2]>>c1cccc[c:1]1[N:2](~O)~O"));
            Assert.AreEqual("c1[c:1](cccc1)[NH2:2]>>c1[c:1](cccc1)[N:2](~O)~O", Smarts.Generate(mol));
        }

        [TestMethod()]
        public void CompGrouping()
        {
            var mol = new QueryAtomContainer(null);
            Assert.IsTrue(Smarts.Parse(mol, "([Na+].[Cl-]).c1ccccc1"));
            Assert.AreEqual("c1ccccc1.([Na+].[Cl-])", Smarts.Generate(mol));
        }

        [TestMethod()]
        public void CompGroupingOnAgent()
        {
            var mol = new QueryAtomContainer(null);
            Assert.IsTrue(Smarts.Parse(mol, ">(c1ccccc1[O-].[Na+])>"));
            Assert.AreEqual(">(c1c(cccc1)[O-].[Na+])>", Smarts.Generate(mol));
        }

        [TestMethod()]
        public void AtomStereo()
        {
            var mol = new QueryAtomContainer(null);
            Assert.IsTrue(Smarts.Parse(mol, "C[C@H](O)CC"));
            Assert.AreEqual("C[C@H1](O)CC", Smarts.Generate(mol));
        }

        private static void Swap(object[] a, int i, int j)
        {
            var tmp = a[i];
            a[i] = a[j];
            a[j] = tmp;
        }

        [TestMethod()]
        public void AtomStereoReordered()
        {
            var mol = new QueryAtomContainer(null);
            Assert.IsTrue(Smarts.Parse(mol, "C[C@H](O)CC"));
            var bonds = mol.Bonds.ToArray();
            Swap(bonds, 1, 2);
            mol.SetBonds(bonds);
            Assert.AreEqual("C[C@@H1](CC)O", Smarts.Generate(mol));
        }

        [TestMethod()]
        public void AtomStereoReordered2()
        {
            var mol = new QueryAtomContainer(null);
            Assert.IsTrue(Smarts.Parse(mol, "C[C@H](O)CC"));
            var atoms = mol.Atoms.ToArray();
            Swap(atoms, 0, 1);
            mol.SetAtoms(atoms);
            Assert.AreEqual("[C@@H1](C)(O)CC", Smarts.Generate(mol));
        }

        [TestMethod()]
        public void AtomStereoReordered3()
        {
            var mol = new QueryAtomContainer(null);
            Assert.IsTrue(Smarts.Parse(mol, "[C@H](C)(O)CC"));
            var atoms = mol.Atoms.ToArray();
            Swap(atoms, 0, 1);
            mol.SetAtoms(atoms);
            Assert.AreEqual("C[C@@H1](O)CC", Smarts.Generate(mol));
        }

        [TestMethod()]
        public void AtomStereoOrUnspec()
        {
            var mol = new QueryAtomContainer(null);
            Assert.IsTrue(Smarts.Parse(mol, "C[C@?H](O)CC"));
            Assert.AreEqual("C[CH1@?](O)CC", Smarts.Generate(mol));
        }

        [TestMethod()]
        public void BondStereoTrans()
        {
            var mol = new QueryAtomContainer(null);
            Assert.IsTrue(Smarts.Parse(mol, "C/C=C/C"));
            Assert.AreEqual("C/C=C/C", Smarts.Generate(mol));
            var atoms = mol.Atoms.ToArray();
            Swap(atoms, 0, 1);
            mol.SetAtoms(atoms);
            Assert.AreEqual("C(\\C)=C/C", Smarts.Generate(mol));
        }

        [TestMethod()]
        public void BondStereoCisTrans()
        {
            var mol = new QueryAtomContainer(null);
            Assert.IsTrue(Smarts.Parse(mol, "C/C=C/,\\C"));
            Assert.AreEqual("C/C=C/,\\C", Smarts.Generate(mol));
        }

        [TestMethod()]
        public void BondStereoCisUnspec()
        {
            var mol = new QueryAtomContainer(null);
            Assert.IsTrue(Smarts.Parse(mol, "C/C=C\\?C"));
            Assert.AreEqual("C/C=C\\?C", Smarts.Generate(mol));
            // not trans same as cis/unspec
            mol.RemoveAllElements();
            Assert.IsTrue(Smarts.Parse(mol, "C/C=C!/C"));
            Assert.AreEqual("C/C=C\\?C", Smarts.Generate(mol));
        }

        [TestMethod()]
        public void BondStereoTransUnspec()
        {
            var mol = new QueryAtomContainer(null);
            Assert.IsTrue(Smarts.Parse(mol, "C/?C=C/C"));
            Assert.AreEqual("C/C=C/?C", Smarts.Generate(mol));
            // not cis same as trans/unspec
            mol.RemoveAllElements();
            Assert.IsTrue(Smarts.Parse(mol, "C/C=C!\\C"));
            Assert.AreEqual("C/C=C/?C", Smarts.Generate(mol));
        }

        // unspecified db can be written as either /?\\? or !/!\\
        [TestMethod()]
        public void BondStereoUnspec()
        {
            var mol = new QueryAtomContainer(null);
            Assert.IsTrue(Smarts.Parse(mol, "C/C=C/?\\?C"));
            Assert.AreEqual("C/C=C!/!\\C", Smarts.Generate(mol));
        }

        // here we have one bond symbol shared between two stereo
        // bonds, changing it's affects both stereos
        [TestMethod()]
        public void BondStereoCisThenTrans()
        {
            var mol = new QueryAtomContainer(null);
            Assert.IsTrue(Smarts.Parse(mol, "C/C=C\\C=C\\C"));
            Assert.AreEqual("C/C=C\\C=C\\C", Smarts.Generate(mol));
        }

        // make sure we set the bond direction on the correct neighbor
        [TestMethod()]
        public void BondStereoCisThenTransWithNbr()
        {
            var mol = new QueryAtomContainer(null);
            Assert.IsTrue(Smarts.Parse(mol, "C/C=C(C)\\C=C\\C"));
            Assert.AreEqual("C/C=C(C)\\C=C\\C", Smarts.Generate(mol));
        }

        [TestMethod()]
        public void BondStereoCisThenTransUnspecWithNbr()
        {
            var mol = new QueryAtomContainer(null);
            Assert.IsTrue(Smarts.Parse(mol, "C/C=C(C)\\C=C\\?O"));
            Assert.AreEqual("C/C=C(C)\\C=C\\?O", Smarts.Generate(mol));
            var atoms = mol.Atoms.ToArray();
            Swap(atoms, 0, atoms.Length - 1);
            mol.SetAtoms(atoms);
            Assert.AreEqual("O/?C=C/C(=C\\C)C", Smarts.Generate(mol));
        }

        // this case is tricky, we need to set the non-query bond stereo
        // first then the 'or unspecified' one otherwise we would initially
        // set 'C/C=C(C)/?C=CO' and there is no way to set the other bond
        [TestMethod()]
        public void BondStereoCisThenTransUnspecWithNbrComplex()
        {
            var mol = new QueryAtomContainer(null);
            Assert.IsTrue(Smarts.Parse(mol, "C/?C=C(C)\\C=C\\O"));
            Assert.AreEqual("C/?C=C(C)/C=C/O", Smarts.Generate(mol));
        }

        // multiple calls to parse should set the stereo correctly and
        // put the queries in a single atom container
        [TestMethod()]
        public void MultipleReads()
        {
            var mol = new QueryAtomContainer(null);
            Assert.IsTrue(Smarts.Parse(mol, "C/C=C/C"));
            Assert.IsTrue(Smarts.Parse(mol, "C/C=C\\C"));
            Assert.AreEqual("C/C=C/C.C/C=C\\C", Smarts.Generate(mol));
        }

        [TestMethod()]
        public void RoundTripStereo()
        {
            var mol = new QueryAtomContainer(null);
            Smarts.Parse(mol, "O1.[S@]=1(C)CC");
            Assert.AreEqual("O=[S@@](C)CC", Smarts.Generate(mol));
        }
    }
}
