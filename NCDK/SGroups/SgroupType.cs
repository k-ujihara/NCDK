/*
 * Copyright (c) 2015 John May <jwmay@users.sf.net>
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
 * Any WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using System;
using System.Collections.Generic;

namespace NCDK.SGroups
{
    /// <summary>
    /// Enumeration of Ctab Sgroup types.
    /// <p/>
    /// <br/>
    /// <b>Display shortcuts</b>
    /// <list type="bullet">
    /// <item>SUP, abbreviation Sgroup (formerly called superatom)</item>
    /// <item>MUL, multiple group</item>
    /// <item>GEN, generic</item>
    /// </list>
    /// <b>Polymers</b>
    /// <list type="bullet">
    /// <item>SRU, SRU type</item>
    /// <item>MON, monomer</item>
    /// <item>MER, Mer type</item>
    /// <item>COP, copolymer</item>
    /// <item>CRO, crosslink</item>
    /// <item>MOD, modification</item>
    /// <item>GRA, graft</item>
    /// <item>Any, any polymer</item> 
    /// </list>
    /// <b>Components, Mixtures, and formulations</b>
    /// <list type="bullet">
    /// <item>COM, component</item>
    /// <item>MIX, mixture</item>
    /// <item>FOR, formulation</item>
    /// </list>
    /// <b>Non-chemical</b>
    /// <list type="bullet">
    /// <item>DAT, data Sgroup</item>
    /// </list>
    /// </summary>
    public class SgroupType
    {
        public enum O
        {
            CtabAbbreviation,
            CtabMultipleGroup,
            CtabStructureRepeatUnit,
            CtabMonomer,
            CtabModified,
            CtabCopolymer,
            CtabMer,
            CtabCrossLink,
            CtabGraft,
            CtabAnyPolymer,
            CtabComponent,
            CtabMixture,
            CtabFormulation,
            CtabData,
            CtabGeneric,
            ExtMulticenter,
        };

        public O Ordinal { get; private set; }

        public static readonly SgroupType CtabAbbreviation = new SgroupType("SUP", O.CtabAbbreviation);
        public static readonly SgroupType CtabMultipleGroup = new SgroupType("MUL", O.CtabMultipleGroup);
        public static readonly SgroupType CtabStructureRepeatUnit = new SgroupType("SRU", O.CtabStructureRepeatUnit);
        public static readonly SgroupType CtabMonomer = new SgroupType("MON", O.CtabMonomer);
        public static readonly SgroupType CtabModified = new SgroupType("MOD", O.CtabModified);
        public static readonly SgroupType CtabCopolymer = new SgroupType("COP", O.CtabCopolymer);
        public static readonly SgroupType CtabMer = new SgroupType("MER", O.CtabMer);
        public static readonly SgroupType CtabCrossLink = new SgroupType("CRO", O.CtabCrossLink);
        public static readonly SgroupType CtabGraft = new SgroupType("GRA", O.CtabGraft);
        public static readonly SgroupType CtabAnyPolymer = new SgroupType("Any", O.CtabAnyPolymer);
        public static readonly SgroupType CtabComponent = new SgroupType("COM", O.CtabComponent);
        public static readonly SgroupType CtabMixture = new SgroupType("MIX", O.CtabMixture);
        public static readonly SgroupType CtabFormulation = new SgroupType("FOR", O.CtabFormulation);
        public static readonly SgroupType CtabData = new SgroupType("DAT", O.CtabData);
        public static readonly SgroupType CtabGeneric = new SgroupType("GEN", O.CtabGeneric);

        // extension for handling positional variation and distributed coordination bonds
        public static readonly SgroupType ExtMulticenter = new SgroupType("N/A", O.ExtMulticenter);

        public static readonly IEnumerable<SgroupType> Values = new[]
        {
            CtabAbbreviation,
            CtabMultipleGroup,
            CtabStructureRepeatUnit,
            CtabMonomer,
            CtabModified,
            CtabCopolymer,
            CtabMer,
            CtabCrossLink,
            CtabGraft,
            CtabAnyPolymer,
            CtabComponent,
            CtabMixture,
            CtabFormulation,
            CtabData,
            CtabGeneric,
            ExtMulticenter,
        };

        static readonly IDictionary<string, SgroupType> map = new Dictionary<string, SgroupType>();

        static SgroupType()
        {
            foreach (var t in Values)
                map.Add(t.ctabKey, t);
        }

        private readonly string ctabKey;

        private SgroupType(string ctabKey, O Ordinal)
        {
            this.ctabKey = ctabKey;
            this.Ordinal = Ordinal;
        }

        public string Key => ctabKey;

        public static SgroupType ParseCtabKey(string str)
        {
            SgroupType type;
            if (!map.TryGetValue(str, out type))
            {
                type = SgroupType.CtabGeneric;
            }
            return type;
        }
    }
}
