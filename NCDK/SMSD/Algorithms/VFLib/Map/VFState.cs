/*
 *
 *
 * Copyright (C) 2009-2010  Syed Asad Rahman <asad@ebi.ac.uk>
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
 *
 * MX Cheminformatics Tools for Java
 *
 * Copyright (c) 2007-2009 Metamolecular, LLC
 *
 * http://metamolecular.com
 *
 * Permission is hereby granted, free of charge, to any person obtaining atom copy
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
 *
 */
using NCDK.SMSD.Algorithms.VFLib.Builder;

using System.Collections.Generic;

namespace NCDK.SMSD.Algorithms.VFLib.Map
{
    /// <summary>
    /// This class finds mapping states between query and target
    /// molecules.
    ///
    // @cdk.module smsd
    // @cdk.githash
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    /// </summary>
    public class VFState : IState
    {

        private List<Match> candidates;
        private IQuery query;
        private TargetProperties target;
        private List<INode> queryPath;
        private List<IAtom> targetPath;
        private IDictionary<INode, IAtom> map;

        /// <summary>
        /// Initialise the VFState with query and target
        /// <param name="query">/// @param target</param>
        /// </summary>
        public VFState(IQuery query, TargetProperties target)
        {
            this.map = new Dictionary<INode, IAtom>();
            this.queryPath = new List<INode>();
            this.targetPath = new List<IAtom>();

            this.query = query;
            this.target = target;
            this.candidates = new List<Match>();
            LoadRootCandidates();

        }

        private VFState(VFState state, Match match)
        {
            this.candidates = new List<Match>();
            this.queryPath = new List<INode>(state.queryPath);
            this.targetPath = new List<IAtom>(state.targetPath);

            this.map = state.map;
            this.query = state.query;
            this.target = state.target;

            map[match.QueryNode] = match.TargetAtom;
            queryPath.Add(match.QueryNode);
            targetPath.Add(match.TargetAtom);
            LoadCandidates(match);
        }

        public void BackTrack()
        {
            if (queryPath.Count == 0 || IsGoal)
            {
                map.Clear();
                return;
            }
            if (IsHeadMapped())
            {
                return;
            }
            map.Clear();
            for (int i = 0; i < queryPath.Count - 1; i++)
            {
                map[queryPath[i]] = targetPath[i];
            }
        }

        public IDictionary<INode, IAtom> GetMap()
        {
            return new Dictionary<INode, IAtom>(map);
        }

        public bool HasNextCandidate()
        {
            return candidates.Count != 0;
        }

        public bool IsDead => query.CountNodes() > target.AtomCount;

        public bool IsGoal => map.Count == query.CountNodes();

        public bool IsMatchFeasible(Match match)
        {
            if (map.ContainsKey(match.QueryNode) || map.Values.Contains(match.TargetAtom))
            {
                return false;
            }
            if (!MatchAtoms(match))
            {
                return false;
            }
            if (!MatchBonds(match))
            {
                return false;
            }
            return true;
        }

        public Match NextCandidate()
        {
            var ret = candidates[candidates.Count - 1];
            candidates.RemoveAt(candidates.Count - 1);
            return ret;
        }

        public IState NextState(Match match)
        {
            return new VFState(this, match);
        }

        private void LoadRootCandidates()
        {
            for (int i = 0; i < query.CountNodes(); i++)
            {
                for (int j = 0; j < target.AtomCount; j++)
                {
                    Match match = new Match(query.GetNode(i), target.GetAtom(j));
                    candidates.Add(match);
                }
            }
        }

        //@TODO Asad Check the Neighbour count
        private void LoadCandidates(Match lastMatch)
        {
            IAtom atom = lastMatch.TargetAtom;
            var targetNeighbors = target.GetNeighbors(atom);
            foreach (var q in lastMatch.QueryNode.Neighbors())
            {
                foreach (var t in targetNeighbors)
                {
                    Match match = new Match(q, t);
                    if (CandidateFeasible(match))
                    {
                        candidates.Add(match);
                    }
                }
            }
        }

        private bool CandidateFeasible(Match candidate)
        {
            foreach (var queryAtom in map.Keys)
            {
                if (queryAtom.Equals(candidate.QueryNode) || map[queryAtom].Equals(candidate.TargetAtom))
                {
                    return false;
                }
            }
            return true;
        }

        //This function is updated by Asad to include more matches

        private bool MatchAtoms(Match match)
        {
            IAtom atom = match.TargetAtom;
            if (match.QueryNode.CountNeighbors() > target.CountNeighbors(atom))
            {
                return false;
            }
            return match.QueryNode.AtomMatcher.Matches(target, atom);
        }

        private bool MatchBonds(Match match)
        {
            if (queryPath.Count == 0)
            {
                return true;
            }

            if (!MatchBondsToHead(match))
            {
                return false;
            }

            for (int i = 0; i < queryPath.Count - 1; i++)
            {
                IEdge queryBond = query.GetEdge(queryPath[i], match.QueryNode);
                IBond targetBond = target.GetBond(targetPath[i], match.TargetAtom);
                if (queryBond == null)
                {
                    continue;
                }

                if (targetBond == null)
                {
                    return false;
                }
                if (!MatchBond(queryBond, targetBond))
                {
                    return false;
                }
            }
            return true;
        }

        private bool MatchBond(IEdge edge, IBond targetBond)
        {
            return edge.BondMatcher.Matches(target, targetBond);
        }

        private bool IsHeadMapped()
        {
            INode head = queryPath[queryPath.Count - 1];
            foreach (var neighbor in head.Neighbors())
            {
                if (!map.ContainsKey(neighbor))
                {
                    return false;
                }
            }
            return true;
        }

        private bool MatchBondsToHead(Match match)
        {
            INode queryHead = GetQueryPathHead();
            IAtom targetHead = GetTargetPathHead();

            IEdge queryBond = query.GetEdge(queryHead, match.QueryNode);
            IBond targetBond = target.GetBond(targetHead, match.TargetAtom);

            if (queryBond == null || targetBond == null)
            {
                return false;
            }
            return MatchBond(queryBond, targetBond);
        }

        private INode GetQueryPathHead()
        {
            return queryPath[queryPath.Count - 1];
        }

        private IAtom GetTargetPathHead()
        {
            return targetPath[targetPath.Count - 1];
        }
    }
}
