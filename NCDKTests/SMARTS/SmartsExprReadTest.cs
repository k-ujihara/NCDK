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
using NCDK.Silent;
using System;
using static NCDK.Isomorphisms.Matchers.ExprType;

namespace NCDK.SMARTS
{
    [TestClass()]
    public class SmartsExprReadTest
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
            return new Expr(And, a, b);
        }

        static Expr GetOr(Expr a, Expr b)
        {
            return new Expr(Or, a, b);
        }

        private static Expr GetAtomExpr(IAtom atom)
        {
            return ((QueryAtom)AtomRef.Deref(atom)).Expression;
        }

        private static Expr GetBondExpr(IBond bond)
        {
            return ((QueryBond)BondRef.Deref(bond)).Expression;
        }

        static Expr GetAtomExpr(string sma, SmartsFlaver flav)
        {
            var mol = new AtomContainer();
            Assert.IsTrue(Smarts.Parse(mol, sma, flav));
            return GetAtomExpr(mol.Atoms[0]);
        }

        static Expr GetAtomExpr(string sma)
        {
            return GetAtomExpr(sma, SmartsFlaver.Loose);
        }

        static Expr GetBondExpr(string sma, SmartsFlaver flav)
        {
            var mol = new AtomContainer();
            Assert.IsTrue(Smarts.Parse(mol, sma, flav));
            return GetBondExpr(mol.Bonds[0]);
        }

        static Expr GetBondExpr(string sma)
        {
            return GetBondExpr(sma, SmartsFlaver.Loose);
        }

        [TestMethod()]
        public void TrailingOperator()
        {
            var mol = new AtomContainer();
            Assert.IsFalse(Smarts.Parse(mol, "[a#6,]"));
            Assert.IsFalse(Smarts.Parse(mol, "[a#6;]"));
            Assert.IsFalse(Smarts.Parse(mol, "[a#6&]"));
            Assert.IsFalse(Smarts.Parse(mol, "[a#6!]"));
        }

        [TestMethod()]
        public void LeadingOperator()
        {
            var mol = new AtomContainer();
            Assert.IsFalse(Smarts.Parse(mol, "[,a#6]"));
            Assert.IsFalse(Smarts.Parse(mol, "[;a#6]"));
            Assert.IsFalse(Smarts.Parse(mol, "[&a#6]"));
            Assert.IsTrue(Smarts.Parse(mol, "[!a#6]"));
        }

        [TestMethod()]
        public void TrailingBondOperator()
        {
            var mol = new AtomContainer();
            Assert.IsFalse(Smarts.Parse(mol, "*-,*"));
            Assert.IsFalse(Smarts.Parse(mol, "*-;*"));
            Assert.IsFalse(Smarts.Parse(mol, "*-&*"));
            Assert.IsFalse(Smarts.Parse(mol, "*-!*"));
        }

        [TestMethod()]
        public void LeadingBondOperator()
        {
            var mol = new AtomContainer();
            Assert.IsFalse(Smarts.Parse(mol, "*,-*"));
            Assert.IsFalse(Smarts.Parse(mol, "*;-*"));
            Assert.IsFalse(Smarts.Parse(mol, "*&-*"));
            Assert.IsTrue(Smarts.Parse(mol, "*!-*"));
        }

        [TestMethod()]
        public void OpPrecedence1()
        {
            var mol = new AtomContainer();
            Assert.IsTrue(Smarts.Parse(mol, "[a#6,a#7]"));
            Expr actual = GetAtomExpr(mol.Atoms[0]);
            Expr expected = GetOr(GetAnd(GetExpr(IsAromatic), GetExpr(ExprType.Element, 6)),
                               GetAnd(GetExpr(IsAromatic), GetExpr(ExprType.Element, 7)));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void OpPrecedence2()
        {
            var mol = new AtomContainer();
            Assert.IsTrue(Smarts.Parse(mol, "[a;#6,#7]"));
            Expr actual = GetAtomExpr(mol.Atoms[0]);
            Expr expected = GetAnd(GetExpr(IsAromatic),
                                GetOr(GetExpr(ExprType.Element, 6), GetExpr(ExprType.Element, 7)));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void OpPrecedence3()
        {
            var mol = new AtomContainer();
            Assert.IsTrue(Smarts.Parse(mol, "[#6,#7;a]"));
            Expr actual = GetAtomExpr(mol.Atoms[0]);
            Expr expected = GetAnd(GetExpr(IsAromatic),
                                GetOr(GetExpr(ExprType.Element, 6), GetExpr(ExprType.Element, 7)));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void OpPrecedence4()
        {
            var mol = new AtomContainer();
            Assert.IsTrue(Smarts.Parse(mol, "[#6,#7a]"));
            Expr actual = GetAtomExpr(mol.Atoms[0]);
            Expr expected = GetOr(GetExpr(ExprType.Element, 6),
                               GetAnd(GetExpr(ExprType.Element, 7), GetExpr(IsAromatic)));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void OpPrecedence5()
        {
            var mol = new AtomContainer();
            Assert.IsTrue(Smarts.Parse(mol, "[#6&a,#7]"));
            Expr actual = GetAtomExpr(mol.Atoms[0]);
            Expr expected = GetOr(GetExpr(ExprType.Element, 7),
                               GetAnd(GetExpr(ExprType.Element, 6), GetExpr(IsAromatic)));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void OrList()
        {
            var mol = new AtomContainer();
            Assert.IsTrue(Smarts.Parse(mol, "[F,Cl,Br,I]"));
            Expr actual = GetAtomExpr(mol.Atoms[0]);
            Expr expected = GetOr(GetExpr(ExprType.Element, 9),
                               GetOr(GetExpr(ExprType.Element, 17),
                                  GetOr(GetExpr(ExprType.Element, 35),
                                     GetExpr(ExprType.Element, 53))));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ExplicitHydrogen()
        {
            var mol = new AtomContainer();
            Assert.IsTrue(Smarts.Parse(mol, "[2H+]"));
            Expr actual = GetAtomExpr(mol.Atoms[0]);
            Expr expected = GetAnd(GetExpr(ExprType.Isotope, 2),
                                GetAnd(GetExpr(ExprType.Element, 1), GetExpr(FormalCharge, 1)));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ExplicitHydrogenNeg()
        {
            var mol = new AtomContainer();
            Assert.IsTrue(Smarts.Parse(mol, "[H-]"));
            Expr actual = GetAtomExpr(mol.Atoms[0]);
            Expr expected = GetAnd(GetExpr(ExprType.Element, 1),
                                GetExpr(FormalCharge, -1));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ExplicitHydrogenWithAtomMap()
        {
            var mol = new AtomContainer();
            Assert.IsTrue(Smarts.Parse(mol, "[2H+:2]"));
            Expr actual = GetAtomExpr(mol.Atoms[0]);
            Expr expected = GetAnd(GetExpr(ExprType.Isotope, 2),
                                GetAnd(GetExpr(ExprType.Element, 1),
                                    GetExpr(FormalCharge, 1)));
            Assert.AreEqual(2, mol.Atoms[0].GetProperty<int>(CDKPropertyName.AtomAtomMapping));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ExplicitHydrogenWithBadAtomMap()
        {
            var mol = new AtomContainer();
            Assert.IsFalse(Smarts.Parse(mol, "[2H+:]"));
        }

        [TestMethod()]
        public void NonExplicitHydrogen()
        {
            var mol = new AtomContainer();
            Assert.IsTrue(Smarts.Parse(mol, "[2&H+]"));
            Expr actual = GetAtomExpr(mol.Atoms[0]);
            Expr expected = GetAnd(GetExpr(ExprType.Isotope, 2),
                                GetAnd(GetExpr(ExprType.TotalHCount, 1),
                                    GetExpr(FormalCharge, +1)));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void NonExplicitHydrogen2()
        {
            var mol = new AtomContainer();
            Assert.IsTrue(Smarts.Parse(mol, "[2,H+]"));
            Expr actual = GetAtomExpr(mol.Atoms[0]);
            Expr expected = GetOr(GetExpr(ExprType.Isotope, 2),
                               GetAnd(GetExpr(ExprType.TotalHCount, 1),
                                   GetExpr(FormalCharge, +1)));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void NonExplicitHydrogen3()
        {
            var mol = new AtomContainer();
            Assert.IsTrue(Smarts.Parse(mol, "[2H1+]"));
            Expr actual = GetAtomExpr(mol.Atoms[0]);
            Expr expected = GetAnd(GetExpr(ExprType.Isotope, 2),
                                GetAnd(GetExpr(ExprType.TotalHCount, 1),
                                    GetExpr(FormalCharge, 1)));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void SpecifiedIsotope()
        {
            Expr actual = GetAtomExpr("[!0]");
            Expr expected = GetExpr(HasIsotope);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void NnspecifiedIsotope()
        {
            Expr actual = GetAtomExpr("[0]");
            Expr expected = GetExpr(HasUnspecifiedIsotope);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void RingMembership()
        {
            Expr actual = GetAtomExpr("[R]");
            Expr expected = GetExpr(IsInRing);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void RingMembership2()
        {
            Expr actual = GetAtomExpr("[!R0]");
            Expr expected = GetExpr(IsInRing);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ChainMembership()
        {
            Expr actual = GetAtomExpr("[R0]");
            Expr expected = GetExpr(IsInChain);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ChainMembership2()
        {
            Expr actual = GetAtomExpr("[!R]");
            Expr expected = GetExpr(IsInChain);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ChainMembership3()
        {
            Expr actual = GetAtomExpr("[r0]");
            Expr expected = GetExpr(IsInChain);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ChainMembership4()
        {
            Expr actual = GetAtomExpr("[x0]");
            Expr expected = GetExpr(IsInChain);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void Aromatic()
        {
            Expr actual = GetAtomExpr("[a]");
            Expr expected = GetExpr(IsAromatic);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void Aromatic2()
        {
            Expr actual = GetAtomExpr("[!A]");
            Expr expected = GetExpr(IsAromatic);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void Aliphatic()
        {
            Expr actual = GetAtomExpr("[A]");
            Expr expected = GetExpr(IsAliphatic);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void Aliphatic2()
        {
            Expr actual = GetAtomExpr("[!a]");
            Expr expected = GetExpr(IsAliphatic);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void NotTrue()
        {
            Expr actual = GetAtomExpr("[!*]");
            Expr expected = GetExpr(False);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void NotNotTrue()
        {
            Expr actual = GetAtomExpr("[!!*]");
            Expr expected = GetExpr(True);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void RingCountDefault()
        {
            Expr actual = GetAtomExpr("[R]");
            Expr expected = GetExpr(IsInRing);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void RingCount0()
        {
            Expr actual = GetAtomExpr("[R0]");
            Expr expected = GetExpr(IsInChain);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void RingCount()
        {
            Expr actual = GetAtomExpr("[R1]");
            Expr expected = GetExpr(ExprType.RingCount, 1);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void RingCountOEChem()
        {
            Expr actual = GetAtomExpr("[R2]", SmartsFlaver.OEChem);
            Expr expected = GetExpr(ExprType.RingBondCount, 2);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void RingSmallest()
        {
            Expr actual = GetAtomExpr("[r5]");
            Expr expected = GetExpr(ExprType.RingSmallest, 5);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void RingSmallestDefault()
        {
            Expr actual = GetAtomExpr("[r]");
            Expr expected = GetExpr(IsInRing);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void RingSmallestInvalid()
        {
            var mol = new AtomContainer();
            Assert.IsTrue(Smarts.Parse(mol, "[r0]")); // not in ring
            Assert.IsFalse(Smarts.Parse(mol, "[r1]"));
            Assert.IsFalse(Smarts.Parse(mol, "[r2]"));
            Assert.IsTrue(Smarts.Parse(mol, "[r3]"));
        }

        // make sure not read as C & r
        [TestMethod()]
        public void Chromium()
        {
            Expr actual = GetAtomExpr("[Cr]");
            Expr expected = GetExpr(ExprType.Element, NaturalElements.Chromium.AtomicNumber);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void Hetero()
        {
            Expr actual = GetAtomExpr("[#X]");
            Expr expected = GetExpr(IsHetero);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void RingSize()
        {
            var mol = new AtomContainer();
            Assert.IsTrue(Smarts.Parse(mol, "[Z8]", SmartsFlaver.Daylight));
            Expr actual = GetAtomExpr(mol.Atoms[0]);
            Expr expected = GetExpr(ExprType.RingSize, 8);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void RingSize0()
        {
            var mol = new AtomContainer();
            Assert.IsTrue(Smarts.Parse(mol, "[Z0]", SmartsFlaver.Daylight));
            Expr actual = GetAtomExpr(mol.Atoms[0]);
            Expr expected = GetExpr(IsInChain);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void RingSizeDefault()
        {
            var mol = new AtomContainer();
            Assert.IsTrue(Smarts.Parse(mol, "[Z]", SmartsFlaver.Daylight));
            Expr actual = GetAtomExpr(mol.Atoms[0]);
            Expr expected = GetExpr(IsInRing);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void AdjacentHeteroCount()
        {
            var mol = new AtomContainer();
            Assert.IsTrue(Smarts.Parse(mol, "[Z2]", SmartsFlaver.CACTVS));
            Expr actual = GetAtomExpr(mol.Atoms[0]);
            Expr expected = GetExpr(AliphaticHeteroSubstituentCount, 2);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void AdjacentHetero()
        {
            var mol = new AtomContainer();
            Assert.IsTrue(Smarts.Parse(mol, "[Z]", SmartsFlaver.CACTVS));
            Expr actual = GetAtomExpr(mol.Atoms[0]);
            Expr expected = GetExpr(HasAliphaticHeteroSubstituent);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void AdjacentHetero0()
        {
            var mol = new AtomContainer();
            Assert.IsTrue(Smarts.Parse(mol, "[Z0]", SmartsFlaver.CACTVS));
            Expr actual = GetAtomExpr(mol.Atoms[0]);
            Expr expected = GetExpr(HasAliphaticHeteroSubstituent).Negate();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void Valence()
        {
            Expr actual = GetAtomExpr("[v4]");
            Expr expected = GetExpr(ExprType.Valence, 4);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ValenceDefault()
        {
            Expr actual = GetAtomExpr("[v]");
            Expr expected = GetExpr(ExprType.Valence, 1);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void Degree()
        {
            Expr actual = GetAtomExpr("[D4]");
            Expr expected = GetExpr(ExprType.Degree, 4);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void DegreeDefault()
        {
            Expr actual = GetAtomExpr("[D]");
            Expr expected = GetExpr(ExprType.Degree, 1);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void DegreeCDKLegacy()
        {
            Expr actual = GetAtomExpr("[D4]", SmartsFlaver.CdkLegacy);
            Expr expected = GetExpr(HeavyDegree, 4);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void DegreeCDKLegacyDefault()
        {
            Expr actual = GetAtomExpr("[D]", SmartsFlaver.CdkLegacy);
            Expr expected = GetExpr(HeavyDegree, 1);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void Connectivity()
        {
            Expr actual = GetAtomExpr("[X4]");
            Expr expected = GetExpr(TotalDegree, 4);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ConnectivityDefault()
        {
            Expr actual = GetAtomExpr("[X]");
            Expr expected = GetExpr(TotalDegree, 1);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void TotalHCount()
        {
            Expr actual = GetAtomExpr("[H2]");
            Expr expected = GetExpr(ExprType.TotalHCount, 2);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ImplHCount()
        {
            Expr actual = GetAtomExpr("[h2]");
            Expr expected = GetExpr(ExprType.ImplicitHCount, 2);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void HasImplHCount()
        {
            Expr actual = GetAtomExpr("[h]");
            Expr expected = GetExpr(HasImplicitHydrogen);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void RingBondCount()
        {
            Expr actual = GetAtomExpr("[x2]");
            Expr expected = GetExpr(ExprType.RingBondCount, 2);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void RingBondCount0()
        {
            Expr actual = GetAtomExpr("[x0]");
            Expr expected = GetExpr(IsInChain);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void RingBondCount1()
        {
            Assert.IsFalse(Smarts.Parse(new QueryAtomContainer(null), "[x1]"));
        }

        [TestMethod()]
        public void RingBondCountDefault()
        {
            Expr actual = GetAtomExpr("[x]");
            Expr expected = GetExpr(IsInRing);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void FormalChargeNeg()
        {
            Expr actual = GetAtomExpr("[-1]");
            Expr expected = GetExpr(FormalCharge, -1);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void FormalChargeNegNeg()
        {
            Expr actual = GetAtomExpr("[--]");
            Expr expected = GetExpr(FormalCharge, -2);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void FormalChargePos()
        {
            Expr actual = GetAtomExpr("[+]");
            Expr expected = GetExpr(FormalCharge, +1);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void FormalChargePosPos()
        {
            Expr actual = GetAtomExpr("[++]");
            Expr expected = GetExpr(FormalCharge, +2);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void AtomMaps()
        {
            var mol = new QueryAtomContainer(null);
            Assert.IsFalse(Smarts.Parse(mol, "[:10]"));
            Assert.IsTrue(Smarts.Parse(mol, "[*:10]"));
            Assert.AreEqual(10, mol.Atoms[0].GetProperty<int>(CDKPropertyName.AtomAtomMapping));
        }

        [TestMethod()]
        public void PeriodicTableGroup()
        {
            Expr actual = GetAtomExpr("[#G16]", SmartsFlaver.MOE);
            Expr expected = GetExpr(PeriodicGroup, 16);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void PeriodicTableGroupCDKLegacy()
        {
            Expr actual = GetAtomExpr("[G16]", SmartsFlaver.CdkLegacy);
            Expr expected = GetExpr(PeriodicGroup, 16);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void InsaturationCactvs()
        {
            Expr actual = GetAtomExpr("[G1]", SmartsFlaver.CACTVS);
            Expr expected = GetExpr(Insaturation, 1);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void InsaturationCactvsOrMoe()
        {
            Assert.AreEqual(GetExpr(Insaturation, 1), GetAtomExpr("[i1]", SmartsFlaver.CACTVS));
            Assert.AreEqual(GetExpr(Insaturation, 1), GetAtomExpr("[i1]", SmartsFlaver.MOE));
        }

        [TestMethod()]
        public void HeteroSubCountCactvs()
        {
            Assert.AreEqual(GetExpr(HasHeteroSubstituent), GetAtomExpr("[z]", SmartsFlaver.CACTVS));
            Assert.AreEqual(GetExpr(HeteroSubstituentCount, 1), GetAtomExpr("[z1]", SmartsFlaver.CACTVS));
        }

        [TestMethod()]
        public void HybridisationNumber()
        {
            Expr actual = GetAtomExpr("[^2]", SmartsFlaver.OEChem);
            Expr expected = GetExpr(ExprType.HybridisationNumber, 2);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void HybridisationNumberDaylight()
        {
            Assert.IsFalse(Smarts.Parse(new QueryAtomContainer(null), "[^2]", SmartsFlaver.Daylight));
        }

        [TestMethod()]
        public void AtomStereoLeft()
        {
            Expr actual = GetAtomExpr("[@]");
            Expr expected = GetExpr(Stereochemistry, (int)StereoConfigurations.Left);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void AtomStereoRight()
        {
            Expr actual = GetAtomExpr("[@@]");
            Expr expected = GetExpr(Stereochemistry, (int)StereoConfigurations.Right);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void AtomStereoLeftOrUnspec()
        {
            Expr actual = GetAtomExpr("[@?]");
            Expr expected = GetOr(GetExpr(Stereochemistry, (int)StereoConfigurations.Left),
                GetExpr(Stereochemistry, 0));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void AtomStereoSimpleLeft()
        {
            Expr actual = GetAtomExpr("[C@H]");
            Assert.AreEqual(new Expr(AliphaticElement, 6)
                .And(new Expr(Stereochemistry, 1))
                .And(new Expr(ExprType.TotalHCount, 1)), actual);
        }

        [TestMethod()]
        public void BadExprs()
        {
            var mol = new AtomContainer();
            Assert.IsFalse(Smarts.Parse(mol, "*-,*"));
            Assert.IsFalse(Smarts.Parse(mol, "*-;*"));
            Assert.IsFalse(Smarts.Parse(mol, "*-!*"));
            Assert.IsFalse(Smarts.Parse(mol, "*-&*"));
            Assert.IsFalse(Smarts.Parse(mol, "*!*"));
            Assert.IsFalse(Smarts.Parse(mol, "*,*"));
            Assert.IsFalse(Smarts.Parse(mol, "*;*"));
            Assert.IsFalse(Smarts.Parse(mol, "*&*"));
            Assert.IsFalse(Smarts.Parse(mol, "*,-*"));
        }

        [TestMethod()]
        public void SingleOrAromatic()
        {
            Expr actual = GetBondExpr("**");
            Expr expected = GetExpr(ExprType.SingleOrAromatic);
            Assert.AreEqual(actual, expected);
        }

        [TestMethod()]
        public void SingleBond()
        {
            Expr actual = GetBondExpr("*-*");
            Expr expected = GetExpr(AliphaticOrder, 1);
            Assert.AreEqual(actual, expected);
        }

        [TestMethod()]
        public void DoubleBond()
        {
            Expr actual = GetBondExpr("*=*");
            Expr expected = GetExpr(AliphaticOrder, 2);
            Assert.AreEqual(actual, expected);
        }

        [TestMethod()]
        public void TripleBond()
        {
            Expr actual = GetBondExpr("*#*");
            Expr expected = GetExpr(AliphaticOrder, 3);
            Assert.AreEqual(actual, expected);
        }

        [TestMethod()]
        public void QuadBond()
        {
            Expr actual = GetBondExpr("*$*");
            Expr expected = GetExpr(AliphaticOrder, 4);
            Assert.AreEqual(actual, expected);
        }

        [TestMethod()]
        public void AromaticBond()
        {
            Expr actual = GetBondExpr("*:*");
            Expr expected = GetExpr(IsAromatic);
            Assert.AreEqual(actual, expected);
        }

        [TestMethod()]
        public void AliphaticBond()
        {
            Expr actual = GetBondExpr("*!:*");
            Expr expected = GetExpr(IsAliphatic);
            Assert.AreEqual(actual, expected);
        }

        [TestMethod()]
        public void ChainBond()
        {
            Expr actual = GetBondExpr("*!@*");
            Expr expected = GetExpr(IsInChain);
            Assert.AreEqual(actual, expected);
        }

        [TestMethod()]
        public void AnyBond()
        {
            Expr actual = GetBondExpr("*~*");
            Expr expected = GetExpr(True);
            Assert.AreEqual(actual, expected);
        }

        [TestMethod()]
        public void SingleOrDouble()
        {
            Expr actual = GetBondExpr("*-,=*");
            Expr expected = GetOr(GetExpr(AliphaticOrder, 1),
                               GetExpr(AliphaticOrder, 2));
            Assert.AreEqual(actual, expected);
        }

        [TestMethod()]
        public void OperatorPrecedence()
        {
            Expr actual = GetBondExpr("*@;-,=*");
            Expr expected = GetAnd(GetExpr(IsInRing),
                                GetOr(GetExpr(AliphaticOrder, 1),
                                   GetExpr(AliphaticOrder, 2)));
            Assert.AreEqual(actual, expected);
        }

        [TestMethod()]
        public void NotInRing()
        {
            Expr actual = GetBondExpr("*!@*");
            Expr expected = GetExpr(IsInChain);
            Assert.AreEqual(actual, expected);
        }

        [TestMethod()]
        public void NotAromatic()
        {
            Expr actual = GetBondExpr("*!:*");
            Expr expected = GetExpr(IsAliphatic);
            Assert.AreEqual(actual, expected);
        }

        [TestMethod()]
        public void NotNotWildcard()
        {
            Expr actual = GetBondExpr("*!!~*");
            Expr expected = GetExpr(True);
            Assert.AreEqual(actual, expected);
        }

        [TestMethod()]
        public void TestAliphaticSymbols()
        {
            foreach (var e in NaturalElement.Elements)
            {
                int len = e.Symbol.Length;
                if (len == 1 || len == 2)
                {
                    string smarts = $"[{e.Symbol}]";
                    var mol = new QueryAtomContainer(null);
                    Assert.IsTrue(Smarts.Parse(mol, smarts), smarts);
                    var expr = GetAtomExpr(mol.Atoms[0]);
                    var ee = new Expr(ExprType.Element, e.AtomicNumber);
                    var ea = new Expr(AliphaticElement, e.AtomicNumber);
                    Assert.IsTrue(expr.Equals(ee) || expr.Equals(ea));
                }
            }
        }

        [TestMethod()]
        public void TestAromaticSymbols()
        {
            Assert.AreEqual(new Expr(AromaticElement, 5), GetAtomExpr("[b]"));
            Assert.AreEqual(new Expr(AromaticElement, 6), GetAtomExpr("[c]"));
            Assert.AreEqual(new Expr(AromaticElement, 7), GetAtomExpr("[n]"));
            Assert.AreEqual(new Expr(AromaticElement, 8), GetAtomExpr("[o]"));
            Assert.AreEqual(new Expr(AromaticElement, 13), GetAtomExpr("[al]"));
            Assert.AreEqual(new Expr(AromaticElement, 14), GetAtomExpr("[si]"));
            Assert.AreEqual(new Expr(AromaticElement, 15), GetAtomExpr("[p]"));
            Assert.AreEqual(new Expr(AromaticElement, 16), GetAtomExpr("[s]"));
            Assert.AreEqual(new Expr(AromaticElement, 33), GetAtomExpr("[as]"));
            Assert.AreEqual(new Expr(AromaticElement, 34), GetAtomExpr("[se]"));
            Assert.AreEqual(new Expr(AromaticElement, 51), GetAtomExpr("[sb]"));
            Assert.AreEqual(new Expr(AromaticElement, 52), GetAtomExpr("[te]"));
        }

        [TestMethod()]
        public void TestBadSymbols()
        {
            Assert.IsFalse(Smarts.Parse(new QueryAtomContainer(null), "[L]"));
            Assert.IsFalse(Smarts.Parse(new QueryAtomContainer(null), "[J]"));
            Assert.IsFalse(Smarts.Parse(new QueryAtomContainer(null), "[Q]"));
            Assert.IsFalse(Smarts.Parse(new QueryAtomContainer(null), "[G]"));
            Assert.IsFalse(Smarts.Parse(new QueryAtomContainer(null), "[T]"));
            Assert.IsFalse(Smarts.Parse(new QueryAtomContainer(null), "[M]"));
            Assert.IsFalse(Smarts.Parse(new QueryAtomContainer(null), "[E]"));
            Assert.IsFalse(Smarts.Parse(new QueryAtomContainer(null), "[t]"));
            Assert.IsFalse(Smarts.Parse(new QueryAtomContainer(null), "[?]"));
        }

        [TestMethod()]
        public void TestRecursive()
        {
            Assert.IsTrue(Smarts.Parse(new QueryAtomContainer(null), "[$(*OC)]"));
            Assert.IsFalse(Smarts.Parse(new QueryAtomContainer(null), "[$*OC)]"));
            Assert.IsFalse(Smarts.Parse(new QueryAtomContainer(null), "[$(*OC]"));
            Assert.IsTrue(Smarts.Parse(new QueryAtomContainer(null), "[$((*[O-].[Na+]))]"));
            Assert.IsFalse(Smarts.Parse(new QueryAtomContainer(null), "[$([J])]"));
        }

        // recursive SMARTS with single atoms should be 'lifted' up to a single
        // non-recursive expression
        [TestMethod()]
        public void TestTrivialRecursive()
        {
            Expr expr = GetAtomExpr("[$(F),$(Cl),$(Br)]");
            Assert.AreEqual(GetOr(GetExpr(ExprType.Element, 9), GetOr(GetExpr(ExprType.Element, 17), GetExpr(ExprType.Element, 35))), expr);
        }

        // must always be read/written in SMARTS as recursive but we can lift
        // the expression up to the top level
        [TestMethod()]
        public void TestTrivialRecursive2()
        {
            Expr expr = GetAtomExpr("[!$([F,Cl,Br])]");
            Assert.AreEqual(GetOr(GetExpr(ExprType.Element, 9), GetOr(GetExpr(ExprType.Element, 17), GetExpr(ExprType.Element, 35))).Negate(), expr);
        }

        [TestMethod()]
        public void RingOpenCloseInconsistency()
        {
            Assert.IsFalse(Smarts.Parse(new QueryAtomContainer(null), "C=1CC-,=1"));
            Assert.IsFalse(Smarts.Parse(new QueryAtomContainer(null), "C=1CC-1")); ;
        }

        [TestMethod()]
        public void RingOpenCloseConsistency()
        {
            Assert.IsTrue(Smarts.Parse(new QueryAtomContainer(null), "C-,=1CC-,=1"));
            Assert.IsTrue(Smarts.Parse(new QueryAtomContainer(null), "C!~1CC!~1"));
        }

        [TestMethod()]
    public void DegreeRange()
        {
            Expr expr = GetAtomExpr("[D{1-3}]");
            Assert.AreEqual(GetOr(GetExpr(ExprType.Degree, 1),
                                   GetOr(GetExpr(ExprType.Degree, 2),
                                      GetExpr(ExprType.Degree, 3))), expr);
        }

        [TestMethod()]
        public void ImplHRange()
        {
            Expr expr = GetAtomExpr("[h{1-3}]");
            Assert.AreEqual(GetOr(GetExpr(ExprType.ImplicitHCount, 1),
                                   GetOr(GetExpr(ExprType.ImplicitHCount, 2),
                                      GetExpr(ExprType.ImplicitHCount, 3))), expr);
        }

        [TestMethod()]
    public void TotalHCountRange()
        {
            Expr expr = GetAtomExpr("[H{1-3}]");
            Assert.AreEqual(GetOr(GetExpr(ExprType.TotalHCount, 1),
                                   GetOr(GetExpr(ExprType.TotalHCount, 2),
                                      GetExpr(ExprType.TotalHCount, 3))), expr);
        }
        [TestMethod()]
    public void ValenceRange()
        {
            Expr expr = GetAtomExpr("[v{1-3}]");
            Assert.AreEqual(GetOr(GetExpr(ExprType.Valence, 1),
                                   GetOr(GetExpr(ExprType.Valence, 2),
                                      GetExpr(ExprType.Valence, 3))), expr);
        }
        [TestMethod()]
    public void RingBondCountRange()
        {
            Expr expr = GetAtomExpr("[x{2-4}]");
            Assert.AreEqual(GetOr(GetExpr(ExprType.RingBondCount, 2),
                                   GetOr(GetExpr(ExprType.RingBondCount, 3),
                                      GetExpr(ExprType.RingBondCount, 4))), expr);
        }

        [TestMethod()]
    public void RingSmallestSizeCountRange()
        {
            Expr expr = GetAtomExpr("[r{5-7}]");
            Assert.AreEqual(GetOr(GetExpr(ExprType.RingSmallest, 5),
                                   GetOr(GetExpr(ExprType.RingSmallest, 6),
                                      GetExpr(ExprType.RingSmallest, 7))), expr);
        }
    }
}
