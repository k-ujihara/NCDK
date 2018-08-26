/* Copyright (C) 2009-2010 Syed Asad Rahman <asad@ebi.ac.uk>
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
using NCDK.Isomorphisms.Matchers;
using System.Collections.Generic;

namespace NCDK.SMSD
{
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    // @cdk.module test-smsd
    [TestClass()]
    public abstract class AbstractMCSTest
    {
        public class AbstractMCSImpl : AbstractMCS
        {
            public override void Init(IAtomContainer source, IAtomContainer target, bool removeHydrogen, bool cleanMol) { }

            public override void Init(IQueryAtomContainer source, IAtomContainer target) { }

            public override void SetChemFilters(bool stereoFilter, bool fragmentFilter, bool energyFilter) { }

            public override double? GetEnergyScore(int Key) => null;

            public override int? GetFragmentSize(int Key) => null;

            public override IAtomContainer ProductMolecule => null;

            public override IAtomContainer ReactantMolecule => null;

            public override int? GetStereoScore(int Key) => null;

            public override bool IsStereoMisMatch() => false;

            public override bool IsSubgraph() => false;

            public override double GetTanimotoSimilarity() => 0.0;

            public override double GetEuclideanDistance() => 0.0;

            public override IReadOnlyList<IReadOnlyDictionary<IAtom, IAtom>> GetAllAtomMapping() => null;

            public override IReadOnlyList<IReadOnlyDictionary<int, int>> GetAllMapping() => null;

            public override IReadOnlyDictionary<IAtom, IAtom> GetFirstAtomMapping() => null;

            public override IReadOnlyDictionary<int, int> GetFirstMapping() => null;

            public override double BondSensitiveTimeOut
            {
                get { throw new System.NotSupportedException("Not supported yet."); }
                set { throw new System.NotSupportedException("Not supported yet."); }
            }

            public override double BondInSensitiveTimeOut
            {
                get { throw new System.NotSupportedException("Not supported yet."); }
                set { throw new System.NotSupportedException("Not supported yet."); }
            }
        }
    }
}
