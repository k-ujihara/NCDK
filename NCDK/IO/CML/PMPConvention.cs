/* Copyright (C) 1997-2007  Egon Willighagen <egonw@users.sf.net>
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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using NCDK.Numerics;
using System.Diagnostics;
using System.Xml.Linq;

namespace NCDK.IO.CML
{
    /// <summary>
    ///  Implementation of the PMPMol Covention for CML.
    /// <para>PMP stands for PolyMorph Predictor and is a module
    ///  of Cerius2 (tm).</para>
    /// </summary>
    // @cdk.module io
    // @cdk.githash
    // @author Egon Willighagen <egonw@sci.kun.nl>
    public class PMPConvention : CMLCoreModule
    {
        public PMPConvention(IChemFile chemFile)
            : base(chemFile)
        {
        }

        public PMPConvention(ICMLModule conv)
            : base(conv)
        {
            Debug.WriteLine("New PMP Convention!");
        }

        public override void StartDocument()
        {
            base.StartDocument();
            //        cdo.StartObject("Frame");
            CurrentChemModel = CurrentChemFile.Builder.NewChemModel();
        }

        public override void StartElement(CMLStack xpath, XElement element)
        {
            Debug.WriteLine("PMP element: name");
            base.StartElement(xpath, element);
        }

        public override void CharacterData(CMLStack xpath, XElement element)
        {
            string s = element.Value.Trim();
            Debug.WriteLine($"Start PMP chardata ({CurrentElement}) :{s}");
            Debug.WriteLine($" ElTitle: {ElementTitle}");
            if (xpath.ToString().EndsWith("string/") && BUILTIN.Equals("spacegroup"))
            {
                string sg = "P1";
                // standardize space group names (see Crystal.java)
                if ("P 21 21 21 (1)".Equals(s))
                {
                    sg = "P 2_1 2_1 2_1";
                }
                //            cdo.SetObjectProperty("Crystal", "spacegroup", sg);
                ((ICrystal)CurrentMolecule).SpaceGroup = sg;
            }
            else if (xpath.ToString().EndsWith("floatArray/")
                  && (ElementTitle.Equals("a") || ElementTitle.Equals("b") || ElementTitle.Equals("c")))
            {
                var tokens = s.Split(' ');
                if (tokens.Length > 2)
                {
                    if (ElementTitle.Equals("a"))
                    {
                        ((ICrystal)CurrentMolecule).A = new Vector3(
                            double.Parse(tokens[0]),
                            double.Parse(tokens[1]),
                            double.Parse(tokens[2]));
                    }
                    else if (ElementTitle.Equals("b"))
                    {
                        ((ICrystal)CurrentMolecule).B = new Vector3(
                            double.Parse(tokens[0]),
                            double.Parse(tokens[1]),
                            double.Parse(tokens[2]));
                    }
                    else if (ElementTitle.Equals("c"))
                    {
                        ((ICrystal)CurrentMolecule).C = new Vector3(
                            double.Parse(tokens[0]),
                            double.Parse(tokens[1]),
                            double.Parse(tokens[2]));
                    }
                }
                else
                {
                    Debug.WriteLine("PMP Convention error: incorrect number of cell axis fractions!");
                }
                //            cdo.EndObject(axis);
            }
            else
            {
                base.CharacterData(xpath, element);
            }
            Debug.WriteLine("End PMP chardata");
        }
    }
}
