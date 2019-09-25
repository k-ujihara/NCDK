/*
 * Copyright (c) 2018 Saulius Gražulis <grazulis@ibt.lt>
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace NCDK.Depict
{
    [TestClass()]
    public class DepictionTest
    {
        static readonly Dictionary<string, string> name2Smiles = new Dictionary<string, string>
        {
            ["cis-platin"] = "Cl[Pt@SP1](Cl)([NH3])[NH3]",
            ["trans-[Co(NH3)4(NO2)2]"] = "O=[N+]([O-])[Co@]([NH3])([NH3])([NH3])([NH3])[N+]([O-])(=O)",
        };

        [TestMethod()]
        public void DepictAsSvg()
        {
            var dg = new DepictionGenerator();
            foreach (var e in name2Smiles)
            {
                var ac = CDK.SmilesParser.ParseSmiles(e.Value);
                var svg = dg.Depict(ac).ToSvgString();
                //using (var w = new System.IO.StreamWriter(System.IO.Path.Combine("C:\\Users\\Public\\Documents", $"{e.Key}.svg")))
                //{
                //    w.Write(svg);
                //}
            }
        }
    }
}
