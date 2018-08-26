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

using NCDK.SMSD.Helper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NCDK.SMSD.Tools
{
    /// <summary>
    /// Class that stores bond breaking/formation energy between two atoms.
    /// </summary>
    /// <remarks>
    /// Reference: Huheey, pps. A-21 to A-34; T.L. Cottrell,
    /// "The Strengths of Chemical Bonds," 2nd ed., Butterworths, London, 1958;
    /// B. deB. Darwent, "National Standard Reference Data Series,
    /// "National Bureau of Standards, No. 31, Washington, DC, 1970;
    /// S.W. Benson, J. Chem. Educ., 42, 502 (1965).
    ///
    /// Common Bond Energies (D) and Bond Lengths (r)
    ///
    /// Hydrogen
    /// Bond    D(kJ/mol) r(pm)
    ///
    /// H-H    432    74
    /// H-B    389    119
    /// H-C    411    109
    /// H-Si    318    148
    /// H-Ge    288    153
    /// H-Sn    251    170
    /// H-N    386    101
    /// H-P    322    144
    /// H-As    247    152
    /// H-O    459    96
    /// H-S    363    134
    /// H-Se    276    146
    /// H-Te    238    170
    /// H-F    565    92
    /// H-Cl    428    127
    /// H-Br    362    141
    /// H-I    295    161
    ///
    ///
    /// Group 13
    /// Bond    D(kJ/mol) r(pm)
    /// B-B    293
    /// B-O    536
    /// B-F    613
    /// B-Cl    456    175
    /// B-Br    377
    ///
    ///
    /// Group 14
    /// Bond    D(kJ/mol) r(pm)
    /// C-C    346    154
    /// C=C    602    134
    /// C#C    835    120
    /// C-Si    318    185
    /// C-Ge    238    195
    /// C-Sn    192    216
    /// C-Pb    130    230
    /// C-N    305    147
    /// C=N    615    129
    /// C#N    887    116
    /// C-P    264    184
    /// C-O    358    143
    /// C=O    799    120
    /// C#O    1072    113
    /// C-B    356
    /// C-S    272    182
    /// C=S    573    160
    /// C-F    485    135
    /// C-Cl    327    177
    /// C-Br    285    194
    /// C-I    213    214
    ///
    ///
    /// Group 14
    /// Bond    D(kJ/mol) r(pm)
    /// Si-Si    222    233
    /// Si-N    355
    /// Si-O    452    163
    /// Si-S    293    200
    /// Si-F    565    160
    /// Si-Cl    381    202
    /// Si-Br    310    215
    /// Si-I    234    243
    /// Ge-Ge    188    241
    /// Ge-N    257
    /// Ge-F    470    168
    /// Ge-Cl    349    210
    /// Ge-Br    276    230
    /// Ge-I    212
    /// Sn-F    414
    /// Sn-Cl    323    233
    /// Sn-Br    273    250
    /// Sn-I    205    270
    /// Pb-F    331
    /// Pb-Cl    243    242
    /// Pb-Br    201
    /// Pb-I    142    279
    ///
    ///
    /// Group 15
    /// Bond    D(kJ/mol) r(pm)
    /// N-N    167    145
    /// N=N    418    125
    /// N#N    942    110
    /// N-O    201    140
    /// N=O    607    121
    /// N-F    283    136
    /// N-Cl    313    175
    /// P-P    201    221
    /// P-O    335    163
    /// P=O    544    150
    /// P=S    335    186
    /// P-F    490    154
    /// P-Cl    326    203
    /// P-Br    264
    /// P-I    184
    /// As-As    146    243
    /// As-O    301    178
    /// As-F    484    171
    /// As-Cl    322    216
    /// As-Br    458    233
    /// As-I    200    254
    /// Sb-Sb    121
    /// Sb-F    440
    /// Sb-Cl (SbCl5)    248
    /// Sb-Cl (SbCl3)    315    232
    ///
    /// Group 16
    /// Bond    D(kJ/mol) r(pm)
    /// O-O    142    148
    /// O=O    494    121
    /// O-F    190    142
    /// S=O    522    143
    /// S-S (S8)    226    205
    /// S=S    425    149
    /// S-F    284    156
    /// S-Cl    255    207
    /// Se-Se    172
    /// Se=Se    272    215
    ///
    /// Group 17
    /// Bond    D(kJ/mol) r(pm))
    /// F-F    155    142
    /// Cl-Cl    240    199
    /// Br-Br    190    228
    /// I-I    148    267
    /// At-At    116
    /// I-O    201
    /// I-F    273    191
    /// I-Cl    208    232
    /// I-Br    175
    ///
    /// Group 18
    /// Bond    D(kJ/mol) r(pm)
    /// Kr-F (KrF2)    50    190
    /// Xe-O    84    175
    /// Xe-F    130    195
    /// </remarks>
    // @cdk.githash
    // @cdk.module smsd
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    public class BondEnergies
    {
        private static IDictionary<int, BondEnergy> bondEngergies = null;
        /// <summary>
        /// Singleton pattern instance for the Bond Energy class
        /// </summary>
        public static BondEnergies Instance { get; } = new BondEnergies();

        protected internal BondEnergies()
        {
            int key = 1;
            bondEngergies = new ConcurrentDictionary<int, BondEnergy>();

            //      =========Hydrogen Block==============
            key = SetHydrogenBlock(key);
            //       ==================Group 13=================
            key = SetGroup13(key);
            //      ===================Group 14 Part 1=================
            key = SetGroup14Part1(key);
            //      ===================Group 14 Part 2=================
            key = SetGroup14Part2(key);
            //      ===================Group 15=================
            key = SetGroup15(key);
            //      ===================Group 16=================
            key = SetGroup16(key);
            //      ===================Group 17=================
            key = SetGroup17(key);
            //      ===================Group 18=================
            key = SetGroup18(key);
        }

        /// <summary>
        /// Returns bond energy for a bond type, given atoms and bond type
        /// <param name="sourceAtom">First bondEnergy</param>
        /// <param name="targetAtom">Second bondEnergy</param>
        /// <param name="bondOrder">(single, double etc)</param>
        /// <returns>bond energy</returns>
        /// </summary>
        public virtual int GetEnergies(IAtom sourceAtom, IAtom targetAtom, BondOrder bondOrder)
        {
            int dKJPerMol = -1;

            foreach (var entry in bondEngergies)
            {
                BondEnergy bondEnergy = entry.Value;
                string atom1 = bondEnergy.SymbolFirstAtom;
                string atom2 = bondEnergy.SymbolSecondAtom;
                if ((string.Equals(atom1, sourceAtom.Symbol, StringComparison.OrdinalIgnoreCase) &&
                     string.Equals(atom2, targetAtom.Symbol, StringComparison.OrdinalIgnoreCase))
                 || (string.Equals(atom2, sourceAtom.Symbol, StringComparison.OrdinalIgnoreCase) &&
                     string.Equals(atom1, targetAtom.Symbol, StringComparison.OrdinalIgnoreCase)))
                {
                    BondOrder order = bondEnergy.BondOrder;
                    if (order.CompareTo(bondOrder) == 0)
                    {
                        dKJPerMol = bondEnergy.Energy;
                    }
                }
            }
            return dKJPerMol;
        }

        /// <summary>
        /// Returns bond energy for a bond type, given atoms and bond type
        /// <param name="sourceAtom">First bondEnergy</param>
        /// <param name="targetAtom">Second bondEnergy</param>
        /// <param name="bondOrder">(single, double etc)</param>
        /// <returns>bond energy</returns>
        /// </summary>
        public static int GetEnergies(string sourceAtom, string targetAtom, BondOrder bondOrder)
        {
            int dKJPerMol = -1;

            foreach (var entry in bondEngergies)
            {
                BondEnergy bondEnergy = entry.Value;
                string atom1 = bondEnergy.SymbolFirstAtom;
                string atom2 = bondEnergy.SymbolSecondAtom;
                if ((string.Equals(atom1, sourceAtom, StringComparison.OrdinalIgnoreCase) &&
                     string.Equals(atom2, targetAtom, StringComparison.OrdinalIgnoreCase))
                 || (string.Equals(atom2, sourceAtom, StringComparison.OrdinalIgnoreCase) &&
                     string.Equals(atom1, targetAtom, StringComparison.OrdinalIgnoreCase)))
                {

                    BondOrder order = bondEnergy.BondOrder;
                    if (order.CompareTo(bondOrder) == 0)
                    {
                        dKJPerMol = bondEnergy.Energy;
                    }
                }
            }
            return dKJPerMol;
        }

        /// <summary>
        /// Returns bond energy for a bond type, given atoms and bond type
        /// <param name="bond">(single, double etc)</param>
        /// <returns>bond energy</returns>
        /// </summary>
        public static int GetEnergies(IBond bond)
        {
            int dKJPerMol = -1;
            foreach (var entry in bondEngergies)
            {
                BondEnergy bondEnergy = entry.Value;
                if (bondEnergy.Matches(bond))
                {
                    dKJPerMol = bondEnergy.Energy;
                }
            }
            return dKJPerMol;
        }

        private static int SetHydrogenBlock(int key)
        {
            bondEngergies[key++] = new BondEnergy("H", "H", BondOrder.Single, 432);
            bondEngergies[key++] = new BondEnergy("H", "B", BondOrder.Single, 389);
            bondEngergies[key++] = new BondEnergy("H", "C", BondOrder.Single, 411);
            bondEngergies[key++] = new BondEnergy("H", "Si", BondOrder.Single, 318);
            bondEngergies[key++] = new BondEnergy("H", "Ge", BondOrder.Single, 288);
            bondEngergies[key++] = new BondEnergy("H", "Sn", BondOrder.Single, 251);
            bondEngergies[key++] = new BondEnergy("H", "N", BondOrder.Single, 386);
            bondEngergies[key++] = new BondEnergy("H", "P", BondOrder.Single, 322);
            bondEngergies[key++] = new BondEnergy("H", "As", BondOrder.Single, 247);
            bondEngergies[key++] = new BondEnergy("H", "O", BondOrder.Single, 459);
            bondEngergies[key++] = new BondEnergy("H", "S", BondOrder.Single, 363);
            bondEngergies[key++] = new BondEnergy("H", "Se", BondOrder.Single, 276);
            bondEngergies[key++] = new BondEnergy("H", "Te", BondOrder.Single, 238);
            bondEngergies[key++] = new BondEnergy("H", "F", BondOrder.Single, 565);
            bondEngergies[key++] = new BondEnergy("H", "Cl", BondOrder.Single, 428);
            bondEngergies[key++] = new BondEnergy("H", "Br", BondOrder.Single, 362);
            bondEngergies[key++] = new BondEnergy("H", "I", BondOrder.Single, 295);
            return key;
        }

        private static int SetGroup13(int key)
        {
            bondEngergies[key++] = new BondEnergy("B", "B", BondOrder.Single, 293);
            bondEngergies[key++] = new BondEnergy("B", "O", BondOrder.Single, 536);
            bondEngergies[key++] = new BondEnergy("B", "F", BondOrder.Single, 613);
            bondEngergies[key++] = new BondEnergy("B", "Cl", BondOrder.Single, 456);
            bondEngergies[key++] = new BondEnergy("B", "Br", BondOrder.Single, 377);
            return key;
        }

        private static int SetGroup14Part1(int key)
        {
            bondEngergies[key++] = new BondEnergy("C", "C", BondOrder.Single, 346);
            bondEngergies[key++] = new BondEnergy("C", "C", BondOrder.Double, 602);
            bondEngergies[key++] = new BondEnergy("C", "C", BondOrder.Triple, 835);
            bondEngergies[key++] = new BondEnergy("C", "Si", BondOrder.Single, 318);
            bondEngergies[key++] = new BondEnergy("C", "Ge", BondOrder.Single, 238);
            bondEngergies[key++] = new BondEnergy("C", "Sn", BondOrder.Single, 192);
            bondEngergies[key++] = new BondEnergy("C", "Pb", BondOrder.Single, 130);
            bondEngergies[key++] = new BondEnergy("C", "N", BondOrder.Single, 305);
            bondEngergies[key++] = new BondEnergy("C", "N", BondOrder.Double, 615);
            bondEngergies[key++] = new BondEnergy("C", "N", BondOrder.Triple, 887);
            bondEngergies[key++] = new BondEnergy("C", "P", BondOrder.Single, 264);
            bondEngergies[key++] = new BondEnergy("C", "O", BondOrder.Single, 358);
            bondEngergies[key++] = new BondEnergy("C", "O", BondOrder.Double, 799);
            bondEngergies[key++] = new BondEnergy("C", "O", BondOrder.Triple, 1072);
            bondEngergies[key++] = new BondEnergy("C", "B", BondOrder.Single, 356);
            bondEngergies[key++] = new BondEnergy("C", "S", BondOrder.Single, 272);
            bondEngergies[key++] = new BondEnergy("C", "S", BondOrder.Double, 573);
            bondEngergies[key++] = new BondEnergy("C", "F", BondOrder.Single, 485);
            bondEngergies[key++] = new BondEnergy("C", "Cl", BondOrder.Single, 327);
            bondEngergies[key++] = new BondEnergy("C", "Br", BondOrder.Single, 285);
            bondEngergies[key++] = new BondEnergy("C", "I", BondOrder.Single, 213);
            return key;
        }

        private static int SetGroup14Part2(int key)
        {

            bondEngergies[key++] = new BondEnergy("Si", "Si", BondOrder.Single, 222);
            bondEngergies[key++] = new BondEnergy("Si", "N", BondOrder.Single, 355);
            bondEngergies[key++] = new BondEnergy("Si", "O", BondOrder.Single, 452);
            bondEngergies[key++] = new BondEnergy("Si", "S", BondOrder.Single, 293);
            bondEngergies[key++] = new BondEnergy("Si", "F", BondOrder.Single, 565);
            bondEngergies[key++] = new BondEnergy("Si", "Cl", BondOrder.Single, 381);
            bondEngergies[key++] = new BondEnergy("Si", "Br", BondOrder.Single, 310);
            bondEngergies[key++] = new BondEnergy("Si", "I", BondOrder.Single, 234);

            bondEngergies[key++] = new BondEnergy("Ge", "Ge", BondOrder.Single, 188);
            bondEngergies[key++] = new BondEnergy("Ge", "N", BondOrder.Single, 257);
            bondEngergies[key++] = new BondEnergy("Ge", "F", BondOrder.Single, 470);
            bondEngergies[key++] = new BondEnergy("Ge", "Cl", BondOrder.Single, 349);
            bondEngergies[key++] = new BondEnergy("Ge", "Br", BondOrder.Single, 276);
            bondEngergies[key++] = new BondEnergy("Ge", "I", BondOrder.Single, 212);

            bondEngergies[key++] = new BondEnergy("Sn", "F", BondOrder.Single, 414);
            bondEngergies[key++] = new BondEnergy("Sn", "Cl", BondOrder.Single, 323);
            bondEngergies[key++] = new BondEnergy("Sn", "Br", BondOrder.Single, 273);
            bondEngergies[key++] = new BondEnergy("Sn", "I", BondOrder.Single, 205);

            bondEngergies[key++] = new BondEnergy("Pb", "F", BondOrder.Single, 313);
            bondEngergies[key++] = new BondEnergy("Pb", "Cl", BondOrder.Single, 243);
            bondEngergies[key++] = new BondEnergy("Pb", "Br", BondOrder.Single, 201);
            bondEngergies[key++] = new BondEnergy("Pb", "I", BondOrder.Single, 142);
            return key;
        }

        private static int SetGroup15(int key)
        {
            bondEngergies[key++] = new BondEnergy("N", "N", BondOrder.Single, 167);
            bondEngergies[key++] = new BondEnergy("N", "N", BondOrder.Double, 418);
            bondEngergies[key++] = new BondEnergy("N", "N", BondOrder.Triple, 942);
            bondEngergies[key++] = new BondEnergy("N", "O", BondOrder.Single, 201);
            bondEngergies[key++] = new BondEnergy("N", "O", BondOrder.Double, 607);
            bondEngergies[key++] = new BondEnergy("N", "F", BondOrder.Single, 283);
            bondEngergies[key++] = new BondEnergy("N", "Cl", BondOrder.Single, 313);

            bondEngergies[key++] = new BondEnergy("P", "P", BondOrder.Single, 201);
            bondEngergies[key++] = new BondEnergy("P", "O", BondOrder.Single, 335);
            bondEngergies[key++] = new BondEnergy("P", "O", BondOrder.Double, 544);
            bondEngergies[key++] = new BondEnergy("P", "S", BondOrder.Double, 335);
            bondEngergies[key++] = new BondEnergy("P", "F", BondOrder.Single, 490);
            bondEngergies[key++] = new BondEnergy("P", "Cl", BondOrder.Single, 326);
            bondEngergies[key++] = new BondEnergy("P", "Br", BondOrder.Single, 264);
            bondEngergies[key++] = new BondEnergy("P", "I", BondOrder.Single, 184);

            bondEngergies[key++] = new BondEnergy("As", "As", BondOrder.Single, 146);
            bondEngergies[key++] = new BondEnergy("As", "O", BondOrder.Single, 301);
            bondEngergies[key++] = new BondEnergy("As", "F", BondOrder.Single, 484);
            bondEngergies[key++] = new BondEnergy("As", "Cl", BondOrder.Single, 322);
            bondEngergies[key++] = new BondEnergy("As", "Br", BondOrder.Single, 458);
            bondEngergies[key++] = new BondEnergy("As", "I", BondOrder.Single, 200);

            bondEngergies[key++] = new BondEnergy("Sb", "Sb", BondOrder.Single, 121);
            bondEngergies[key++] = new BondEnergy("Sb", "F", BondOrder.Single, 440);
            //          Sb-Cl (SbCl 5)
            bondEngergies[key++] = new BondEnergy("Sb", "Cl", BondOrder.Single, 248);
            //          Sb-Cl (SbCl 3)
            bondEngergies[key++] = new BondEnergy("Sb", "Cl", BondOrder.Single, 315);
            return key;

        }

        private static int SetGroup16(int key)
        {

            bondEngergies[key++] = new BondEnergy("O", "O", BondOrder.Single, 142);
            bondEngergies[key++] = new BondEnergy("O", "O", BondOrder.Double, 494);
            bondEngergies[key++] = new BondEnergy("O", "F", BondOrder.Single, 190);
            bondEngergies[key++] = new BondEnergy("O", "S", BondOrder.Single, 365);
            bondEngergies[key++] = new BondEnergy("S", "O", BondOrder.Double, 522);
            bondEngergies[key++] = new BondEnergy("S", "S", BondOrder.Single, 226);
            bondEngergies[key++] = new BondEnergy("S", "S", BondOrder.Double, 425);
            bondEngergies[key++] = new BondEnergy("S", "F", BondOrder.Single, 284);
            bondEngergies[key++] = new BondEnergy("S", "Cl", BondOrder.Single, 255);
            bondEngergies[key++] = new BondEnergy("Se", "Se", BondOrder.Single, 172);
            bondEngergies[key++] = new BondEnergy("Se", "Se", BondOrder.Double, 272);
            return key;

        }

        private static int SetGroup17(int key)
        {
            bondEngergies[key++] = new BondEnergy("F", "F", BondOrder.Single, 155);
            bondEngergies[key++] = new BondEnergy("Cl", "Cl", BondOrder.Single, 240);
            bondEngergies[key++] = new BondEnergy("Br", "Br", BondOrder.Single, 190);
            bondEngergies[key++] = new BondEnergy("I", "I", BondOrder.Single, 148);
            bondEngergies[key++] = new BondEnergy("At", "At", BondOrder.Single, 116);

            bondEngergies[key++] = new BondEnergy("I", "O", BondOrder.Single, 201);
            bondEngergies[key++] = new BondEnergy("I", "F", BondOrder.Single, 273);
            bondEngergies[key++] = new BondEnergy("I", "Cl", BondOrder.Single, 208);
            bondEngergies[key++] = new BondEnergy("I", "Br", BondOrder.Single, 175);
            return key;

        }

        private static int SetGroup18(int key)
        {
            bondEngergies[key++] = new BondEnergy("Kr", "F", BondOrder.Single, 50);
            bondEngergies[key++] = new BondEnergy("Xe", "O", BondOrder.Single, 84);
            bondEngergies[key++] = new BondEnergy("Xe", "F", BondOrder.Single, 130);
            return key;
        }
    }
}
