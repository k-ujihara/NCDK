/*
 * MX - Essential Cheminformatics
 *
 * Copyright (c) 2007-2009 Metamolecular, LLC
 *
 * http://metamolecular.com/mx
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.

 *Copyright (C) 2009-2010 Syed Asad Rahman <asad@ebi.ac.uk>
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
using NCDK.SMSD.Algorithms.VFLib.Map;

namespace NCDK.SMSD.Algorithms.VFLib
{
    // @author Richard L. Apodaca <rapodaca at metamolecular.com>
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    // @cdk.module test-smsd
    [TestClass()]
    public class VFMapperTest
    {
        private IAtomContainer hexane;
        private IAtomContainer benzene;
        private IAtomContainer pyridine;
        private IAtomContainer toluene4;
        private IAtomContainer pyridazine;
        private IAtomContainer naphthalene;
        private IAtomContainer chlorobenzene;
        private IAtomContainer chloroisoquinoline4;
        private IAtomContainer toluene;
        private IAtomContainer phenol;
        private IAtomContainer acetone;
        private IAtomContainer propane;
        private IAtomContainer cyclopropane;

        public VFMapperTest()
        {
        }

        [TestInitialize()]
        public void Setup()
        {
            hexane = Molecules.CreateHexane();
            benzene = Molecules.CreateBenzene();
            pyridine = Molecules.CreatePyridine();
            toluene4 = Molecules.Create4Toluene();
            pyridazine = Molecules.CreatePyridazine();
            chloroisoquinoline4 = Molecules.CreateChloroIsoquinoline4();
            chlorobenzene = Molecules.CreateChlorobenzene();
            naphthalene = Molecules.CreateNaphthalene();
            toluene = Molecules.CreateToluene();
            phenol = Molecules.CreatePhenol();
            acetone = Molecules.CreateAcetone();
            propane = Molecules.CreatePropane();
            cyclopropane = Molecules.CreateCyclopropane();
        }

        [TestMethod()]
        public void TestItShouldMatchHexaneToHexane()
        {
            IMapper mapper = new VFMapper(hexane, true);

            Assert.IsTrue(mapper.HasMap(hexane));
        }

        [TestMethod()]
        public void TestItShouldMatchHexaneToHexaneWhenUsingMolecule()
        {
            IMapper mapper = new VFMapper(hexane, true);

            Assert.IsTrue(mapper.HasMap(hexane));
        }

        [TestMethod()]
        public void TestItShouldMatchBenzeneToBenzene()
        {
            IMapper mapper = new VFMapper(benzene, true);

            Assert.IsTrue(mapper.HasMap(benzene));
        }

        [TestMethod()]
        public void TestItShouldNotMatchHexaneToBenzene()
        {
            IMapper mapper = new VFMapper(hexane, true);

            Assert.IsFalse(mapper.HasMap(benzene));
        }

        [TestMethod()]
        public void TestItShouldNotMatchPyridazineToNaphthalene()
        {
            IMapper mapper = new VFMapper(pyridazine, true);

            Assert.IsFalse(mapper.HasMap(naphthalene));
        }

        [TestMethod()]
        public void TestItShouldNotMatchChlorobenzeneTo4ChloroIsoquinoline()
        {
            IMapper mapper = new VFMapper(chlorobenzene, true);

            Assert.IsFalse(mapper.HasMap(chloroisoquinoline4));
        }

        [TestMethod()]
        public void TestItShouldNotMatchBenzeneToPyridine()
        {
            IMapper mapper = new VFMapper(benzene, true);

            Assert.IsFalse(mapper.HasMap(pyridine));

            mapper = new VFMapper(pyridine, true);

            Assert.IsFalse(mapper.HasMap(benzene));
        }

        [TestMethod()]
        public void TestItShouldNotMatchTolueneToBenzene()
        {
            IMapper mapper = new VFMapper(toluene, true);

            Assert.IsFalse(mapper.HasMap(benzene));
        }

        [TestMethod()]
        public void TestItShouldMatchAcetoneToAcetone()
        {
            IMapper mapper = new VFMapper(acetone, true);

            Assert.IsTrue(mapper.HasMap(acetone));
        }

        [TestMethod()]
        public void TestItShouldMatchPropaneToCyclopropane()
        {
            IMapper mapper = new VFMapper(propane, true);

            Assert.IsTrue(mapper.HasMap(cyclopropane));
        }

        [TestMethod()]
        public void TestItShouldFindTwoMapsFromHexaneToHexane()
        {
            IMapper mapper = new VFMapper(hexane, true);

            var maps = mapper.GetMaps(hexane);

            Assert.AreEqual(2, maps.Count);
        }

        [TestMethod()]
        public void TestItShouldNotMatchTolueneToPhenol()
        {
            IMapper mapper = new VFMapper(toluene, true);

            Assert.IsFalse(mapper.HasMap(phenol));
        }

        [TestMethod()]
        public void TestItShouldMapSixAtomsOfBenzeneOntoBenzene()
        {
            IMapper mapper = new VFMapper(benzene, true);
            var map = mapper.GetFirstMap(benzene);

            Assert.AreEqual(6, map.Count);
        }

        [TestMethod()]
        public void TestItShouldCountTwelveMapsForBenzeneOntoBenzene()
        {
            IMapper mapper = new VFMapper(benzene, true);

            Assert.AreEqual(12, mapper.CountMaps(benzene));
        }

        [TestMethod()]
        public void TestItShouldCountTwoMapsForTolueneOntoToluene()
        {
            IMapper mapper = new VFMapper(toluene, true);

            Assert.AreEqual(2, mapper.CountMaps(toluene));
        }

        [TestMethod()]
        public void TestItShouldFindTwelveMapsForBenzeneOntoBenzene()
        {
            IMapper mapper = new VFMapper(benzene, true);
            var maps = mapper.GetMaps(benzene);

            Assert.AreEqual(12, maps.Count);
        }

        [TestMethod()]
        public void TestItShouldFindTwentyFourMapsForBenzeneOntoNaphthalene()
        {
            IMapper mapper = new VFMapper(benzene, true);
            var maps = mapper.GetMaps(naphthalene);

            Assert.AreEqual(24, maps.Count);
        }

        [TestMethod()]
        public void TestItShouldFindAMapForEquivalentFormsOfToluene()
        {
            IMapper mapper = new VFMapper(toluene, true);
            var map = mapper.GetFirstMap(toluene4);

            Assert.AreEqual(7, map.Count);
        }

        [TestMethod()]
        public void TestItShouldFindTwoMapsForEquivalentFormsOfToluene()
        {
            IMapper mapper = new VFMapper(toluene, true);
            var maps = mapper.GetMaps(toluene4);

            Assert.AreEqual(2, maps.Count);
        }
        //    [TestMethod()]
        //    public void TestItMapsBlockedPropaneOntoPropane() {
        //        IAtomContainer blockedPropane = Molecules.CreatePropane();
        //        IAtom atom = blockedPropane.Builder.NewAtom("H");
        //        blockedPropane.Atoms.Add(atom);
        //        IBond bond = blockedPropane.Builder.NewBond(atom, blockedPropane.Atoms[1], BondOrder.Single);
        //
        //        blockedPropane.Bonds.Add(bond);
        //
        //        IMapper mapper = new VFMapper(blockedPropane, true);
        //
        //        Assert.IsTrue(mapper.HasMap(propane));
        //    }
        //    public void TestItMapsBlockedBenzaldehydeOntoBenzaldehyde() {
        //        Molecule blockedBenzaldehyde = this.CreateBlockedBenzaldehyde();
        //        IMapper mapper = new VFMapper(blockedBenzaldehyde, true);
        //
        //        Assert.IsTrue(mapper.HasMap(CreateBenzaldehyde()));
        //    }
        //
        //    public void TestItDoesntMapBlockedBenzaldehydeOntoBenzoicAcid() {
        //        Molecule blockedBenzaldehyde = this.CreateBlockedBenzaldehyde();
        //        IMapper mapper = new VFMapper(blockedBenzaldehyde, true);
        //
        //        Assert.IsFalse(mapper.HasMap(CreateBenzoicAcid()));
        //    }
        //
        //    public void TestItMapsDimethylsulfideToChargelessDMSO() {
        //        IMapper mapper = new VFMapper(Molecules.CreateDimethylsulfide(), true);
        //
        //        Assert.IsTrue(mapper.HasMap(Molecules.CreateChargelessDMSO()));
        //    }
        //
        //    public void TestItMapsDimethylsulfideToChargedDMSO() {
        //        IMapper mapper = new VFMapper(Molecules.CreateDimethylsulfide());
        //
        //        Assert.IsTrue(mapper.HasMap(Molecules.CreateChargedDMSO()));
        //    }
        //  public void TestItMapsChargelessDMSOToChargeledDMSO()
        //  {
        //    Mapper mapper = new DefaultMapper(Molecules.CreateChargelessDMSO());
        //
        //    Assert.IsTrue(mapper.HasMap(Molecules.CreateChargedDMSO()));
        //  }
        //    [TestMethod()]
        //    public void TestItMapsPropaneToAcetone() {
        //        IAtomContainer mol = Molecules.CreatePropane();
        //        IQuery querComp = QueryCompiler.Compile(mol, true);
        //        IMapper mapper = new VFMapper(querComp);
        //        Assert.IsTrue(mapper.HasMap(Molecules.CreateAcetone()));
        //    }
        //
        //    [TestMethod()]
        //    public void TestDoesntMapImineToAmine() {
        //        IAtomContainer mol = Molecules.CreateSimpleImine();
        //        IQuery querComp = QueryCompiler.Compile(mol, true);
        //        IMapper mapper = new VFMapper(querComp);
        //        IDictionary<INode, IAtom> map = mapper.GetFirstMap(Molecules.CreateSimpleAmine());
        //        Assert.AreEqual(0, map.Count);
        //    }
        //
        //    public void TestItMapsBigmolToItself() {
        //        IMapper mapper = new VFMapper(bigmol, true);
        //
        //        Assert.AreEqual(bigmol.Atoms.Count, mapper.GetFirstMap(bigmol).Count);
        //    }
        //
        //    public void TestBigmolHasOneMap() {
        //        IMapper mapper = new VFMapper(bigmol, true);
        //
        //        Assert.AreEqual(1, mapper.CountMaps(bigmol));
        //    }
    }
}

