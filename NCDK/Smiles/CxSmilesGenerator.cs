/*
 * Copyright (c) 2016 John May <jwmay@users.sf.net>
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
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using NCDK.Common.Primitives;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using static NCDK.Smiles.CxSmilesState;

namespace NCDK.Smiles
{
    internal static class CxSmilesGenerator
    {
        // calculate the inverse of a permutation
        private static int[] Inverse(int[] perm)
        {
            var inv = new int[perm.Length];
            for (int i = 0, len = perm.Length; i < len; i++)
                inv[perm[i]] = i;
            return inv;
        }

        private static string Encode_alias(string label)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < label.Length; i++)
            {
                char c = label[i];
                if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c == '_')
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append("&#").Append(new string(new[] { c })).Append(";");
                }
            }
            return sb.ToString();
        }

        private static int Compare(Comparison<int> comp, List<int> a, List<int> b)
        {
            var alen = a.Count;
            var blen = b.Count;
            var len = Math.Min(alen, blen);
            for (int i = 0; i < len; i++)
            {
                var cmp = comp(a[i], b[i]);
                if (cmp != 0)
                    return cmp;
            }
            return alen.CompareTo(blen);
        }

        class CxPolymerSgroup_Comparer : IComparer<CxPolymerSgroup>
        {
            readonly Comparison<int> comp;

            public CxPolymerSgroup_Comparer(Comparison<int> comp)
            {
                this.comp = comp;
            }

            public int Compare(CxPolymerSgroup a, CxPolymerSgroup b)
            {
                int cmp = 0;
                cmp = a.type.CompareTo(b.type);
                if (cmp != 0) 
                    return cmp;
                cmp = CxSmilesGenerator.Compare(comp, a.atoms, b.atoms);
                return cmp;
            }
        }

        class CxDataSgroup_Comparer : IComparer<CxDataSgroup>
        {
            readonly Comparison<int> comp;

            public CxDataSgroup_Comparer(Comparison<int> comp)
            {
                this.comp = comp;
            }

            public int Compare(CxDataSgroup a, CxDataSgroup b)
            {
                int cmp = 0;
                cmp = a.field.CompareTo(b.field);
                if (cmp != 0) 
                    return cmp;
                cmp = a.value.CompareTo(b.value);
                if (cmp != 0) 
                    return cmp;
                cmp = CxSmilesGenerator.Compare(comp, a.atoms, b.atoms);
                return cmp;
            }
        }

        internal static string Generate(CxSmilesState state, SmiFlavors opts, int[] components, int[] ordering)
        {
            if (!SmiFlavorTool.IsSet(opts, SmiFlavors.CxSmilesWithCoords))
                return "";

            var invorder = Inverse(ordering);

            var sb = new StringBuilder();
            sb.Append(' ');
            sb.Append('|');

            //int invComp(int a, int b) => invorder[a].CompareTo(invorder[b]);
            int comp(int a, int b) => ordering[a].CompareTo(ordering[b]);

            // Fragment Grouping
            if (SmiFlavorTool.IsSet(opts, SmiFlavors.CxFragmentGroup)
             && state.fragGroups != null
             && state.fragGroups.Any())
            {
                int maxCompId = 0;
                foreach (int compId_ in components)
                {
                    if (compId_ > maxCompId)
                        maxCompId = compId_;
                }

                // get the output component order
                var compMap = new int[maxCompId + 1];
                int compId = 1;
                foreach (int idx in invorder)
                {
                    var component = components[idx];
                    if (compMap[component] == 0)
                        compMap[component] = compId++;
                }
                // index vs number, we need to output index
                for (int i = 0; i < compMap.Length; i++)
                    compMap[i]--;

                int compComp(int a, int b) => compMap[a].CompareTo(compMap[b]);

                var fragGroupCpy = new List<List<int>>(state.fragGroups);
                foreach (var idxs in fragGroupCpy)
                    idxs.Sort(compComp);
                fragGroupCpy.Sort((a, b) => CxSmilesGenerator.Compare(compComp, a, b));

                // C1=CC=CC=C1.C1=CC=CC=C1.[OH-].[Na+]>> |f:0.1,2.3,c:0,2,4,6,8,10|
                sb.Append('f');
                sb.Append(':');
                for (int i = 0; i < fragGroupCpy.Count; i++)
                {
                    if (i > 0)
                        sb.Append(',');
                    AppendIntegers(compMap, '.', sb, fragGroupCpy[i]);
                }
            }

            // Atom Labels
            if (SmiFlavorTool.IsSet(opts, SmiFlavors.CxAtomLabel)
             && state.atomLabels != null && state.atomLabels.Any())
            {
                if (sb.Length > 2)
                    sb.Append(',');
                sb.Append('$');
                int nonempty_cnt = 0;
                foreach (int idx in invorder)
                {
                    if (!state.atomLabels.TryGetValue(idx, out string label))
                        label = "";
                    else nonempty_cnt++;
                    sb.Append(Encode_alias(label));
                    // don't need to write anymore more ';'
                    if (nonempty_cnt == state.atomLabels.Count)
                        break;
                    sb.Append(";");
                }
                sb.Append('$');
            }

            // Atom Values
            if (SmiFlavorTool.IsSet(opts, SmiFlavors.CxAtomValue)
             && state.atomValues != null && state.atomValues.Any())
            {

                if (sb.Length > 2)
                    sb.Append(',');
                sb.Append("$_AV:");
                int nonempty_cnt = 0;
                foreach (int idx in invorder)
                {
                    var label = state.atomValues[idx];
                    if (string.IsNullOrEmpty(label))
                        label = "";
                    else nonempty_cnt++;
                    sb.Append(Encode_alias(label));
                    // don't need to write anymore more ';'
                    if (nonempty_cnt == state.atomValues.Count)
                        break;
                    sb.Append(";");
                }
                sb.Append('$');
            }

            // 2D/3D Coordinates
            if (SmiFlavorTool.IsSet(opts, SmiFlavors.CxCoordinates)
             && state.atomCoords != null && state.atomCoords.Any())
            {
                if (sb.Length > 2)
                    sb.Append(',');
                sb.Append('(');
                for (int i = 0; i < ordering.Length; i++)
                {
                    var xyz = state.atomCoords[invorder[i]];
                    if (i != 0)
                        sb.Append(';');
                    if (xyz[0] != 0)
                        sb.Append(Strings.ToSimpleString(xyz[0], 2));
                    sb.Append(',');
                    if (xyz[1] != 0)
                        sb.Append(Strings.ToSimpleString(xyz[1], 2));
                    sb.Append(',');
                    if (xyz[2] != 0)
                        sb.Append(Strings.ToSimpleString(xyz[2], 2));
                }
                sb.Append(')');
            }

            // Multi-center/Positional variation bonds
            if (SmiFlavorTool.IsSet(opts, SmiFlavors.CxMulticenter)
             && state.positionVar != null
             && state.positionVar.Any())
            {
                if (sb.Length > 2)
                    sb.Append(',');
                sb.Append('m');
                sb.Append(':');

                var multicenters = new List<KeyValuePair<int, IList<int>>>(state.positionVar);

                // consistent output order
                multicenters.Sort((a, b) => comp(a.Key, b.Key));

                for (int i = 0; i < multicenters.Count; i++)
                {
                    if (i != 0) sb.Append(',');
                    var e = multicenters[i];
                    sb.Append(ordering[e.Key]);
                    sb.Append(':');
                    var vals = new List<int>(e.Value);
                    vals.Sort(comp);
                    AppendIntegers(ordering, '.', sb, vals);
                }
            }

            if (SmiFlavorTool.IsSet(opts, SmiFlavors.CxLigandOrder) &&
                state.ligandOrdering != null && state.ligandOrdering.Any())
            {
                if (sb.Length > 2)
                    sb.Append(',');
                sb.Append("LO");
                sb.Append(':');

                var ligandorderings = new List<KeyValuePair<int, IList<int>>>(state.ligandOrdering);

                // consistent output order
                ligandorderings.Sort((a, b) => comp(a.Key, b.Key));

                for (int i = 0; i < ligandorderings.Count; i++)
                {
                    if (i != 0)
                        sb.Append(',');
                    var e = ligandorderings[i];
                    sb.Append(ordering[e.Key]);
                    sb.Append(':');
                    AppendIntegers(ordering, '.', sb, e.Value);
                }
            }

            int numSgroups = 0;
            // *CCO* |$_AP1;;;;_AP2$,Sg:n:1,2,3::ht|
            if (SmiFlavorTool.IsSet(opts, SmiFlavors.CxPolymer)
             && state.mysgroups != null && state.mysgroups.Any())
            {
                var polysgroups = new List<CxPolymerSgroup>();
                foreach (var polysgroup in state.mysgroups)
                {
                    if (polysgroup is CxPolymerSgroup)
                    {
                        polysgroups.Add((CxPolymerSgroup)polysgroup);
                        polysgroup.atoms.Sort(comp);
                    }
                }

                polysgroups.Sort(new CxPolymerSgroup_Comparer(comp));

                foreach (var cxPolymerSgroup in polysgroups) {
                    cxPolymerSgroup.id = numSgroups++;
                    if (sb.Length > 2) sb.Append(',');
                    sb.Append("Sg:");
                    sb.Append(cxPolymerSgroup.type);
                    sb.Append(':');
                    AppendIntegers(ordering, ',', sb, cxPolymerSgroup.atoms);
                    sb.Append(':');
                    if (cxPolymerSgroup.subscript != null)
                        sb.Append(cxPolymerSgroup.subscript);
                    sb.Append(':');
                    if (cxPolymerSgroup.supscript != null)
                        sb.Append(cxPolymerSgroup.supscript.ToLowerInvariant());
                }
            }
            if (SmiFlavorTool.IsSet(opts, SmiFlavors.CxDataSgroups)
             && state.mysgroups != null && state.mysgroups.Any())
            {
                var datasgroups = new List<CxDataSgroup>();
                foreach (var datasgroup in state.mysgroups)
                {
                    if (datasgroup is CxDataSgroup sgroup)
                    {
                        datasgroups.Add(sgroup);
                        datasgroup.atoms.Sort(comp);
                    }
                }
                datasgroups.Sort(new CxDataSgroup_Comparer(comp));

                foreach (var cxDataSgroup in datasgroups)
                {
                    cxDataSgroup.id = numSgroups++;
                    if (sb.Length > 2) 
                        sb.Append(',');
                    sb.Append("SgD:");
                    AppendIntegers(ordering, ',', sb, cxDataSgroup.atoms);
                    sb.Append(':');
                    if (cxDataSgroup.field != null)
                        sb.Append(cxDataSgroup.field);
                    sb.Append(':');
                    if (cxDataSgroup.value != null)
                        sb.Append(cxDataSgroup.value);
                    sb.Append(':');
                    if (cxDataSgroup.operator_ != null)
                        sb.Append(cxDataSgroup.operator_);
                    sb.Append(':');
                    if (cxDataSgroup.unit != null)
                        sb.Append(cxDataSgroup.unit);
                    // fmt (t/f/n) + coords?
                }
            }

            // hierarchy information
            if (numSgroups > 0)
            {
                bool firstSgH = true;
                if (state.mysgroups != null)
                {
                    state.mysgroups.Sort((a, b) => a.id.CompareTo(b.id));
                    foreach (var sgroup in state.mysgroups)
                    {
                        if (!sgroup.children.Any())
                            continue;
                        if (sb.Length > 2) 
                            sb.Append(',');
                        if (firstSgH)
                        {
                            sb.Append("SgH:");
                            firstSgH = false;
                        }
                        sb.Append(sgroup.id).Append(':');
                        bool first = true;
                        var children = new List<CxSgroup>(sgroup.children);
                        children.Sort((a, b) => a.id.CompareTo(b.id));
                        foreach (var child in children)
                        {
                            if (child.id < 0)
                                continue;
                            if (!first)
                                sb.Append('.');
                            first = false;
                            sb.Append(child.id);
                        }
                    }
                }
            }

            // [C]1[CH][CH]CCC1 |^1:1,2,^3:0|
            if (SmiFlavorTool.IsSet(opts, SmiFlavors.CxRadical)
             && state.atomRads != null
             && state.atomRads.Any())
            {
                var radinv = new SortedDictionary<CxSmilesState.Radical, List<int>>();
                foreach (var e in state.atomRads)
                {
                    if (!radinv.TryGetValue(e.Value, out List<int> idxs))
                        radinv[e.Value] = idxs = new List<int>();
                    idxs.Add(e.Key);
                }
                foreach (var e in radinv)
                {
                    if (sb.Length > 2)
                        sb.Append(',');
                    sb.Append('^');
                    sb.Append((int)e.Key + 1);
                    sb.Append(':');
                    e.Value.Sort(comp);
                    AppendIntegers(ordering, ',', sb, e.Value);
                }
            }

            sb.Append('|');
            if (sb.Length <= 3)
            {
                return "";
            }
            else
            {
                return sb.ToString();
            }
        }

        private static void AppendIntegers(int[] invorder, char sep, StringBuilder sb, IList<int> vals)
        {
            if (vals.Any())
                sb.Append(string.Join(new string(new[] { sep }), vals.Select(v => invorder[v].ToString(NumberFormatInfo.InvariantInfo))));
        }
    }
}
