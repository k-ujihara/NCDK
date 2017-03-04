/*
 * MX Cheminformatics Tools for Java
 *
 * Copyright (c) 2007-2009 Metamolecular, LLC
 *
 * http://metamolecular.com
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
 */
using NCDK.SMSD.Algorithms.VFLib.Builder;

using NCDK.SMSD.Algorithms.VFLib.Query;
using NCDK.SMSD.Global;
using NCDK.SMSD.Tools;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NCDK.SMSD.Algorithms.VFLib.Map
{
    /// <summary>
    /// This class finds MCS between query and target molecules
    /// using VF2 algorithm.
    ///
    // @cdk.module smsd
    // @cdk.githash
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    /// </summary>
    public class VFMapper : IMapper
    {

        private IQuery query;
        private List<IDictionary<INode, IAtom>> maps;
        private int currentMCSSize = -1;
        private static TimeManager timeManager = null;

        /// <summary>
        /// <returns>the timeout</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected static double GetTimeOut() => TimeOut.Instance.Time;

        /// <summary>
        /// <returns>the timeManager</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected static TimeManager GetTimeManager()
        {
            return timeManager;
        }

        /// <summary>
        /// <param name="aTimeManager">the timeManager to set</param>
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected static void SetTimeManager(TimeManager aTimeManager)
        {
            TimeOut.Instance.Enabled = false;
            timeManager = aTimeManager;
        }

        /// <summary>
        ///
        /// <param name="query">/// </summary></param>
        public VFMapper(IQuery query)
        {
            SetTimeManager(new TimeManager());
            this.query = query;
            this.maps = new List<IDictionary<INode, IAtom>>();
        }

        /// <summary>
        ///
        /// <param name="queryMolecule">/// @param bondMatcher</param>
        /// </summary>
        public VFMapper(IAtomContainer queryMolecule, bool bondMatcher)
        {
            SetTimeManager(new TimeManager());
            this.query = new QueryCompiler(queryMolecule, bondMatcher).Compile();
            this.maps = new List<IDictionary<INode, IAtom>>();
        }

        /// <summary> {@inheritDoc}
        /// <param name="targetMolecule">targetMolecule graph</param>
        /// </summary>
        public bool HasMap(IAtomContainer targetMolecule)
        {
            IState state = new VFState(query, new TargetProperties(targetMolecule));
            maps.Clear();
            return MapFirst(state);
        }

        public IList<IDictionary<INode, IAtom>> GetMaps(IAtomContainer target)
        {
            IState state = new VFState(query, new TargetProperties(target));
            maps.Clear();
            MapAll(state);
            return new List<IDictionary<INode, IAtom>>(maps);
        }

        public IDictionary<INode, IAtom> GetFirstMap(IAtomContainer target)
        {
            IState state = new VFState(query, new TargetProperties(target));
            maps.Clear();
            MapFirst(state);
            return maps.Count == 0 ? new Dictionary<INode, IAtom>() : maps[0];
        }

        public int CountMaps(IAtomContainer target)
        {
            IState state = new VFState(query, new TargetProperties(target));
            maps.Clear();
            MapAll(state);
            return maps.Count;
        }

        /// <summary> {@inheritDoc}
        /// <param name="targetMolecule">targetMolecule graph</param>
        /// </summary>
        public bool HasMap(TargetProperties targetMolecule)
        {
            IState state = new VFState(query, targetMolecule);
            maps.Clear();
            return MapFirst(state);
        }

        public IList<IDictionary<INode, IAtom>> GetMaps(TargetProperties targetMolecule)
        {
            IState state = new VFState(query, targetMolecule);
            maps.Clear();
            MapAll(state);
            return new List<IDictionary<INode, IAtom>>(maps);
        }

        public IDictionary<INode, IAtom> GetFirstMap(TargetProperties targetMolecule)
        {
            IState state = new VFState(query, targetMolecule);
            maps.Clear();
            MapFirst(state);
            return maps.Count == 0 ? new Dictionary<INode, IAtom>() : maps[0];
        }

        public int CountMaps(TargetProperties targetMolecule)
        {
            IState state = new VFState(query, targetMolecule);
            maps.Clear();
            MapAll(state);
            return maps.Count;
        }

        private void AddMapping(IState state)
        {
            IDictionary<INode, IAtom> map = state.GetMap();
            if (!HasMap(map) && map.Count > currentMCSSize)
            {
                maps.Add(map);
                currentMCSSize = map.Count;
            }
            else if (!HasMap(map) && map.Count == currentMCSSize)
            {
                maps.Add(map);
            }
        }

        private void MapAll(IState state)
        {
            if (state.IsDead)
            {
                return;
            }

            if (HasMap(state.GetMap()))
            {
                state.BackTrack();
            }

            if (state.IsGoal)
            {
                IDictionary<INode, IAtom> map = state.GetMap();
                if (!HasMap(map))
                {
                    maps.Add(state.GetMap());
                }
                else
                {
                    state.BackTrack();
                }
            }

            while (state.HasNextCandidate())
            {
                Match candidate = state.NextCandidate();
                if (state.IsMatchFeasible(candidate))
                {
                    IState nextState = state.NextState(candidate);
                    MapAll(nextState);
                    nextState.BackTrack();
                }
            }
        }

        private bool MapFirst(IState state)
        {
            if (state.IsDead)
            {
                return false;
            }

            if (state.IsGoal)
            {
                maps.Add(state.GetMap());
                return true;
            }

            bool found = false;
            while (!found && state.HasNextCandidate())
            {
                Match candidate = state.NextCandidate();
                if (state.IsMatchFeasible(candidate))
                {
                    IState nextState = state.NextState(candidate);
                    found = MapFirst(nextState);
                    nextState.BackTrack();
                }
            }
            return found;
        }

        private bool HasMap(IDictionary<INode, IAtom> map)
        {
            foreach (var storedMap in maps)
            {
                if (Mapper.Comparer_INode_IAtom.Equals(storedMap, map))
                {
                    return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static bool IsTimeOut()
        {
            if (GetTimeOut() > -1 && GetTimeManager().GetElapsedTimeInMinutes() > GetTimeOut())
            {
                TimeOut.Instance.Enabled = true;
                return true;
            }
            return false;
        }
    }
}
