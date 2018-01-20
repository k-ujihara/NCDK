/* Copyright (C) 2007  Miguel Rojasch <miguelrojasch@users.sf.net>
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NCDK.Formula.Rules
{
    /// <summary>
    /// <para>Ring Double Bond Equivalents (RDBE) or
    /// Double Bond Equivalents (DBE) are calculated from valence values of
    /// elements contained in a formula and should tell the number of bonds - or rings.
    /// Since this formula will fail for MFs with higher valence states such as
    /// N(V), P(V), S(IV) or S(VI), this method will focus on the lowest valence state for these elements.</para>
    /// <para>The equation used is: D = 1 + [0.5 SUM_i(N_i(V_I-2))]</para>
    /// <para>where D is the unsaturation, i is the total number of different elements in the composition, N_i the number
    /// of atoms of element i, and Vi is the common valence of the atom i.</para>
    /// </summary>
    /// <remarks>
    /// Table 1: Parameters set by this rule.
    /// <list type="table">
    /// <item>
    ///   <term>Name</term>
    ///   <term>Default</term>
    ///   <term>Description</term>
    /// </item>
    /// <item>
    ///   <term>charge</term>
    ///   <term>0.0</term>
    ///   <term>The RDBE rule of MolecularFormula</term>
    /// </item>
    /// </list>
    /// </remarks>
    // @cdk.module  formula
    // @author      miguelrojasch
    // @cdk.created 2008-06-11
    // @cdk.githash
    public class RDBERule : IRule
    {
        private static IDictionary<string, int[]> oxidationStateTable = null;
        private double min = -0.5;
        private double max = 30;

        /// <summary>
        /// Constructor for the RDBE object.
        /// </summary>
        public RDBERule()
        {
            CreateTable();
        }
        
        /// <summary>
        /// The parameters attribute of the <see cref="RDBERule"/> object.
        /// </summary>
        public object[] Parameters
        {
            get
            {
                // return the parameters as used for the rule validation
                object[] parameters = new object[2];
                parameters[0] = min;
                parameters[1] = max;
                return parameters;
            }
            set
            {
                if (value.Length != 2) throw new CDKException("RDBERule expects two parameters");
                if (!(value[0] is double)) throw new CDKException("The 1 parameter must be of type Double");
                if (!(value[1] is double)) throw new CDKException("The 2 parameter must be of type Double");

                min = (double)value[0];
                max = (double)value[1];
            }
        }

        /// <summary>
        /// Validate the RDBRule of this IMolecularFormula.
        /// </summary>
        /// <param name="formula">Parameter is the IMolecularFormula</param>
        /// <returns>A double value meaning 1.0 True, 0.0 False</returns>
        public double Validate(IMolecularFormula formula)
        {
            Trace.TraceInformation("Start validation of ", formula);

            var RDBEList = GetRDBEValue(formula);
            foreach (var RDBE in RDBEList)
            {
                if (min <= RDBE && RDBE <= 30) if (Validate(formula, RDBE)) return 1.0;
            }

            return 0.0;

        }

        /// <summary>
        /// Validate the ion state. It takes into account that neutral, nonradical compounds
        /// always have an even-numbered pair-wiser arrangement of binding electrons signilizaded
        /// by an integer DBE value. Charged compounds due to soft ionzation techniques
        /// will give an odd number of binding electrons and a fractional DBE (X.05).
        /// </summary>
        /// <param name="formula">Parameter is the IMolecularFormula</param>
        /// <param name="value">The RDBE value</param>
        /// <returns>True, if corresponds with</returns>
        public bool Validate(IMolecularFormula formula, double value)
        {
            double charge = formula.Charge ?? 0;

            long iPart = (long)value;
            double fPart = value - iPart;

            if (fPart == 0.0 && charge == 0) return true;
            if (fPart != 0.0 && charge != 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Method to extract the Ring Double Bond Equivalents (RDB) value. It test all possible
        /// oxidation states.
        /// </summary>
        /// <param name="formula">The IMolecularFormula object</param>
        /// <returns>The RDBE value</returns>
        /// <seealso cref="CreateTable"/>
        public List<double> GetRDBEValue(IMolecularFormula formula)
        {
            var RDBEList = new List<double>();
            // The number of combinations with repetition
            // (v+n-1)!/[n!(v-1)!]
            int nE = 0; // number of elements to change
            var nV = new List<int>(); // number of valence changing
            foreach (var isotope in formula.Isotopes)
            {
                int[] valence = GetOxidationState(formula.Builder.NewAtom(isotope.Symbol));
                if (valence.Length != 1)
                {
                    for (int i = 0; i < valence.Length; i++)
                    {
                        nV.Add(valence[i]);
                    }
                    nE += MolecularFormulaManipulator.GetElementCount(formula,
                            formula.Builder.NewElement(isotope.Symbol));
                }
            }

            double RDBE = 0;
            if (nE == 0)
            {
                foreach (var isotope in formula.Isotopes)
                {
                    int[] valence = GetOxidationState(formula.Builder.NewAtom(isotope.Symbol));
                    double value = (valence[0] - 2) * formula.GetCount(isotope) / 2.0;
                    RDBE += value;
                }
                RDBE += 1;
                RDBEList.Add(RDBE);
            }
            else
            {
                double RDBE_1 = 0;
                foreach (var isotope in formula.Isotopes)
                {
                    int[] valence = GetOxidationState(formula.Builder.NewAtom(isotope.Symbol));
                    double value = (valence[0] - 2) * formula.GetCount(isotope) * 0.5;
                    RDBE_1 += value;
                }
                string[] valences = new string[nV.Count];
                for (int i = 0; i < valences.Length; i++)
                    valences[i] = nV[i].ToString();

                Combinations c = new Combinations(valences, nE);
                while (c.HasMoreElements())
                {
                    double RDBE_int = 0.0;
                    object[] combo = (object[])c.NextElement();
                    for (int i = 0; i < combo.Length; i++)
                    {
                        int value = (int.Parse((string)combo[i]) - 2) / 2;
                        RDBE_int += value;
                    }
                    RDBE = 1 + RDBE_1 + RDBE_int;
                    RDBEList.Add(RDBE);
                }
            }
            return RDBEList;
        }

        /// <summary>
        /// Get the common oxidation state given a atom.
        /// </summary>
        /// <param name="newAtom">The IAtom</param>
        /// <returns>The oxidation state value</returns>
        private int[] GetOxidationState(IAtom newAtom)
        {
            return oxidationStateTable[newAtom.Symbol];
        }

        /// <summary>
        /// Create the table with the common oxidation states
        /// </summary>
        private void CreateTable()
        {
            if (oxidationStateTable == null)
            {
                oxidationStateTable = new Dictionary<string, int[]>
                {
                    ["H"] = new int[] { 1 },
                    //            oxidationStateTable["Li"] = 1;
                    //            oxidationStateTable["Be"] = 2;
                    ["B"] = new int[] { 3 },
                    ["C"] = new int[] { 4 },
                    ["N"] = new int[] { 3 },
                    ["O"] = new int[] { 2 },
                    ["F"] = new int[] { 1 },
                    ["Na"] = new int[] { 1 },
                    ["Mg"] = new int[] { 2 },
                    ["Al"] = new int[] { 3 },
                    ["Si"] = new int[] { 4 },
                    ["P"] = new int[] { 3, 5 },
                    ["S"] = new int[] { 2, 4, 6 },
                    ["Cl"] = new int[] { 1 },
                    //            oxidationStateTable["K"] = 1;
                    //            oxidationStateTable["Ca"] = 2;
                    //            oxidationStateTable["Ga"] = 3;
                    //            oxidationStateTable["Ge"] = 4;
                    //            oxidationStateTable["As"] = 5;
                    //            oxidationStateTable["Se"] = 6;
                    //            oxidationStateTable["Br"] = 7;
                    //            oxidationStateTable["Rb"] = 1;
                    //            oxidationStateTable["Sr"] = 2;
                    //            oxidationStateTable["In"] = 3;
                    //            oxidationStateTable["Sn"] = 4;
                    //            oxidationStateTable["Sb"] = 5;
                    //            oxidationStateTable["Te"] = 6;
                    ["I"] = new int[] { 1 }
                };
                //            oxidationStateTable["Cs"] = 1;
                //            oxidationStateTable["Ba"] = 2;
                //            oxidationStateTable["Tl"] = 3;
                //            oxidationStateTable["Pb"] = 4;
                //            oxidationStateTable["Bi"] = 5;
                //            oxidationStateTable["Po"] = 6;
                //            oxidationStateTable["At"] = 7;
                //            oxidationStateTable["Fr"] = 1;
                //            oxidationStateTable["Ra"] = 2;
                //            oxidationStateTable["Cu"] = 2;
                //            oxidationStateTable["Mn"] = 2;
                //            oxidationStateTable["Co"] = 2;
            }
        }

        public class Combinations
        {
            private object[] inArray;
            private int n, m;
            private int[] index;
            private bool hasMore = true;

            /// <summary>
            /// Create a Combination to enumerate through all subsets of the
            /// supplied Object array, selecting m at a time.
            /// </summary>
            /// <param name="inArray">the group to choose from</param>
            /// <param name="m">int the number to select in each choice</param>
            public Combinations(object[] inArray, int m)
            {
                this.inArray = inArray;
                this.n = inArray.Length;
                this.m = m;

                // index is an array of ints that keep track of the next combination to return.
                // For example, an index on 5 things taken 3 at a time might contain {0 3 4}.
                // This index will be followed by {1 2 3}. Initially, the index is {0 ... m - 1}.
                index = new int[m];
                for (int i = 0; i < m; i++)
                    index[0] = 0;
            }

            /// <summary>
            /// </summary>
            /// <returns>true, unless we have already returned the last combination.</returns>
            public bool HasMoreElements()
            {
                return hasMore;
            }

            /// <summary>
            /// Move the index forward a notch. The algorithm finds the rightmost
            /// index element that can be incremented, increments it, and then
            /// changes the elements to the right to each be 1 plus the element on their left.
            /// <para>
            /// For example, if an index of 5 things taken 3 at a time is at {0 3 4}, only the 0 can
            /// be incremented without running out of room. The next index is {1, 1+1, 1+2) or
            /// {1, 2, 3}. This will be followed by {1, 2, 4}, {1, 3, 4}, and {2, 3, 4}.
            /// </para>
            /// <para>
            /// The algorithm is from Applied Combinatorics, by Alan Tucker.
            /// </para>
            /// </summary>
            private void MoveIndex()
            {
                int i = RightmostIndexBelowMax();
                if (i >= 0)
                {
                    index[i] = index[i] + 1;
                    for (int j = i + 1; j < m; j++)
                        index[j] = index[j - 1];
                }
                else
                    hasMore = false;
            }

            /// <summary>
            /// Actually, an array of Objects is returned. The declaration must say just Object,
            /// because the Combinations class implements Enumeration, which declares that the
            /// NextElement() returns a plain Object. Users must cast the returned object to (Object[]).
            /// </summary>
            /// <returns><see cref="Object"/>, the next combination from the supplied Object array.</returns>
            public object NextElement()
            {
                if (!hasMore) return null;

                object[] output = new object[m];
                for (int i = 0; i < m; i++)
                {
                    output[i] = inArray[index[i]];
                }
                MoveIndex();
                return output;
            }

            /// <summary>
            /// </summary>
            /// <returns>int, the index which can be bumped up.</returns>
            private int RightmostIndexBelowMax()
            {
                for (int i = m - 1; i >= 0; i--)
                {
                    int s = n - 1;
                    if (index[i] != s) return i;
                }
                return -1;
            }
        }
    }
}
