/* Copyright (C) 2006-2010  Syed Asad Rahman <asad@ebi.ac.uk>
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
using NCDK.Common.Collections;
using NCDK.SMSD.Algorithms.McGregors;
using NCDK.SMSD.Global;
using NCDK.SMSD.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace NCDK.SMSD.Algorithms.MCSPluses
{
    /// <summary>
    /// This class handles MCS plus algorithm which is a combination of
    /// c-clique algorithm and McGregor algorithm.
    /// </summary>
    // @cdk.module smsd
    // @cdk.githash
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    [Obsolete("SMSD has been deprecated from the CDK with a newer, more recent version of SMSD is available at http://github.com/asad/smsd . ")]
    public class MCSPlus
    {
        /// <summary>
        /// Default constructor added
        /// </summary>
        public MCSPlus()
        { }

        private static TimeManager timeManager = null;

        /// <summary>
        /// the timeout
        /// </summary>
        protected static double GetTimeOut() => TimeOut.Instance.Time;

        /// <summary>
        /// the timeManager
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected static TimeManager GetTimeManager()
        {
            return timeManager;
        }

        /// <summary>
        /// </summary>
        /// <param name="aTimeManager">the timeManager to set</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected internal static void SetTimeManager(TimeManager aTimeManager)
        {
            TimeOut.Instance.Enabled = false;
            timeManager = aTimeManager;
        }

        internal IList<IList<int>> GetOverlaps(IAtomContainer ac1, IAtomContainer ac2, bool shouldMatchBonds)
        {
            Deque<IList<int>> maxCliqueSet = null;
            IList<IList<int>> mappings = new List<IList<int>>();
            try
            {
                GenerateCompatibilityGraph gcg = new GenerateCompatibilityGraph(ac1, ac2, shouldMatchBonds);
                var compGraphNodes = gcg.GetCompGraphNodes();

                var cEdges = gcg.GetCEgdes();
                var dEdges = gcg.GetDEgdes();

                //            Console.Error.WriteLine("**************************************************");
                //            Console.Error.WriteLine("CEdges: " + CEdges.Count);
                //            Console.Out.WriteLine("DEdges: " + DEdges.Count);

                BKKCKCF init = new BKKCKCF(compGraphNodes, cEdges, dEdges);
                maxCliqueSet = init.GetMaxCliqueSet();

                //            Console.Error.WriteLine("**************************************************");
                //            Console.Error.WriteLine("Max_Cliques_Set: " + maxCliqueSet.Count);
                //            Console.Out.WriteLine("Best Clique Size: " + init.GetBestCliqueSize());

                //clear all the compatibility graph content
                gcg.Clear();
                while (maxCliqueSet.Count != 0)
                {
                    var cliqueList = maxCliqueSet.Peek();
                    int cliqueSize = cliqueList.Count;
                    if (cliqueSize < ac1.Atoms.Count && cliqueSize < ac2.Atoms.Count)
                    {
                        McGregor mgit = new McGregor(ac1, ac2, mappings, shouldMatchBonds);
                        mgit.StartMcGregorIteration(mgit.MCSSize, cliqueList, compGraphNodes);
                        mappings = mgit.Mappings;
                        mgit = null;
                    }
                    else
                    {
                        mappings = ExactMapping.ExtractMapping(mappings, compGraphNodes, cliqueList);
                    }
                    maxCliqueSet.Pop();
                    if (IsTimeOut())
                        break;
                }
            }
            catch (IOException ex)
            {
                Trace.TraceError(ex.Message);
            }
            return mappings;
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
