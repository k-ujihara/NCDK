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
using System.Collections.Generic;
using System.Diagnostics;

namespace NCDK.Formula.Rules
{
    /// <summary>
    /// This class validate if the occurrence of the IElements in the IMolecularFormula, for
    /// metabolites, are into a maximal limit according paper: . The study is from 2 different mass spectral
    /// databases and according different mass of the metabolites. The analysis don't
    /// take account if the IElement is not contained in the matrix. It will be jumped. 
    /// <para>
    /// The rules is based from Tobias Kind paper with the title "Seven Golden Rules for heuristic
    /// filtering of molecular formula" <token>cdk-cite-kind2007</token>.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Table 1: Parameters set by this rule.
    /// <list type="table">
    /// <listheader>
    ///   <term>Name</term>
    ///   <term>Default</term>
    ///   <term>Description</term>
    /// </listheader>
    /// <item>
    ///   <term>database</term>
    ///   <term>willey</term>
    ///   <term>Mass spectral databases extraction</term>
    /// </item>
    /// <item>
    ///   <term>massRange</term>
    ///   <term>&lt; 500</term>
    ///   <term>Mass to take account</term>
    /// </item>
    /// </list>
    /// </remarks>
    // @cdk.module  formula
    // @author      miguelrojasch
    // @cdk.created 2007-11-20
    // @cdk.githash
    public class MMElementRule : IRule
    {
        /// <summary> Database used. As default Willey.</summary>
        private Database databaseUsed = Database.WILEY;

        /// <summary> Mass range used. As default lower than 500.</summary>
        private RangeMass rangeMassUsed = RangeMass.Minus500;

        private Dictionary<string, int> hashMap;

        /// <summary> A enumeration of the possible mass range according the rules. </summary>
        public class RangeMass
        {
            /// <summary>IMolecularFormula from a metabolite with a mass lower than 500 Da.</summary>
            public static readonly RangeMass Minus500 = new RangeMass();
            /// <summary>IMolecularFormula from a metabolite with a mass lower than 1000 Da.</summary>
            public static readonly RangeMass Minus1000 = new RangeMass();
            /// <summary>IMolecularFormula from a metabolite with a mass lower than 2000 Da.</summary>
            public static readonly RangeMass Minus2000 = new RangeMass();
            /// <summary>IMolecularFormula from a metabolite with a mass lower than 3000 Da.</summary>
            public static readonly RangeMass Minus3000 = new RangeMass();
        }

        /// <summary> A enumeration of the possible databases according the rules.</summary>
        public class Database
        {
            /// <summary>Wiley mass spectral database.</summary>
            public static readonly Database WILEY = new Database();
            /// <summary>Dictionary of Natural Products Online mass spectral database.</summary>
            public static readonly Database DNP = new Database();
        }

        /// <summary>
        /// Constructor for the MMElementRule object.
        /// </summary>
        public MMElementRule()
        {
            // initiate Hashmap default
            this.hashMap = GetWisley_500();
        }

        /// <summary>
        /// The parameters attribute of the MMElementRule object.
        /// </summary>
        public object[] Parameters
        {
            get
            {
                // return the parameters as used for the rule validation
                object[] params_ = new object[2];
                params_[0] = databaseUsed;
                params_[1] = rangeMassUsed;
                return params_;
            }
            set
            {
                if (value.Length > 2) throw new CDKException("MMElementRule only expects maximal two parameters");

                if (value[0] != null)
                {
                    if (!(value[0] is Database))
                        throw new CDKException("The parameter must be of type Database enum");
                    databaseUsed = (Database)value[0];
                }

                if (value.Length > 1 && value[1] != null)
                {
                    if (!(value[1] is RangeMass))
                        throw new CDKException("The parameter must be of type RangeMass enum");
                    rangeMassUsed = (RangeMass)value[1];
                }

                if ((databaseUsed == Database.DNP) && (rangeMassUsed == RangeMass.Minus500))
                    this.hashMap = GetDNP_500();
                else if ((databaseUsed == Database.DNP) && (rangeMassUsed == RangeMass.Minus1000))
                    this.hashMap = GetDNP_1000();
                else if ((databaseUsed == Database.DNP) && (rangeMassUsed == RangeMass.Minus2000))
                    this.hashMap = GetDNP_2000();
                else if ((databaseUsed == Database.DNP) && (rangeMassUsed == RangeMass.Minus3000))
                    this.hashMap = GetDNP_3000();
                else if ((databaseUsed == Database.WILEY) && (rangeMassUsed == RangeMass.Minus500))
                    this.hashMap = GetWisley_500();
                else if ((databaseUsed == Database.WILEY) && (rangeMassUsed == RangeMass.Minus1000))
                    this.hashMap = GetWisley_1000();
                else if ((databaseUsed == Database.WILEY) && (rangeMassUsed == RangeMass.Minus2000))
                    this.hashMap = GetWisley_2000();
            }
        }
        
        /// <summary>
        /// Validate the occurrence of this IMolecularFormula.
        /// </summary>
        /// <param name="formula">Parameter is the IMolecularFormula</param>
        /// <returns>An ArrayList containing 9 elements in the order described above</returns>
        public double Validate(IMolecularFormula formula)
        {
            Trace.TraceInformation("Start validation of ", formula);
            double isValid = 1.0;
            foreach (var element in MolecularFormulaManipulator.Elements(formula))
            {
                int occur = MolecularFormulaManipulator.GetElementCount(formula, element);
                if (occur > hashMap[element.Symbol])
                {
                    isValid = 0.0;
                    break;
                }
            }

            return isValid;
        }

        /// <summary>
        /// Get the map linking the symbol of the element and number maximum of occurrence.
        /// For the analysis with the DNP database and mass lower than 500 Da.
        /// </summary>
        /// <returns>The HashMap of the symbol linked with the maximum occurrence</returns>
        private Dictionary<string, int> GetDNP_500()
        {
            Dictionary<string, int> map = new Dictionary<string, int>();

            map["C"] = 29;
            map["H"] = 72;
            map["N"] = 10;
            map["O"] = 18;
            map["P"] = 4;
            map["S"] = 7;
            map["F"] = 15;
            map["Cl"] = 8;
            map["Br"] = 5;

            return map;
        }

        /// <summary>
        /// Get the map linking the symbol of the element and number maximum of occurrence.
        /// For the analysis with the DNP database and mass lower than 1000 Da.
        /// </summary>
        /// <returns>The HashMap of the symbol linked with the maximum occurrence</returns>
        private Dictionary<string, int> GetDNP_1000()
        {
            Dictionary<string, int> map = new Dictionary<string, int>();

            map["C"] = 66;
            map["H"] = 126;
            map["N"] = 25;
            map["O"] = 27;
            map["P"] = 6;
            map["S"] = 8;
            map["F"] = 16;
            map["Cl"] = 11;
            map["Br"] = 8;

            return map;
        }

        /// <summary>
        /// Get the map linking the symbol of the element and number maximum of occurrence.
        /// For the analysis with the DNP database and mass lower than 2000 Da.
        /// </summary>
        /// <returns>The HashMap of the symbol linked with the maximum occurrence</returns>
        private Dictionary<string, int> GetDNP_2000()
        {
            Dictionary<string, int> map = new Dictionary<string, int>();

            map["C"] = 115;
            map["H"] = 236;
            map["N"] = 32;
            map["O"] = 63;
            map["P"] = 6;
            map["S"] = 8;
            map["F"] = 16;
            map["Cl"] = 11;
            map["Br"] = 8;

            return map;
        }

        /// <summary>
        /// Get the map linking the symbol of the element and number maximum of occurrence.
        /// For the analysis with the DNP database and mass lower than 3000 Da.
        /// </summary>
        /// <returns>The HashMap of the symbol linked with the maximum occurrence</returns>
        private Dictionary<string, int> GetDNP_3000()
        {
            Dictionary<string, int> map = new Dictionary<string, int>();

            map["C"] = 162;
            map["H"] = 208;
            map["N"] = 48;
            map["O"] = 78;
            map["P"] = 6;
            map["S"] = 9;
            map["F"] = 16;
            map["Cl"] = 11;
            map["Br"] = 8;

            return map;
        }

        /// <summary>
        /// Get the map linking the symbol of the element and number maximum of occurrence.
        /// For the analysis with the Wisley database and mass lower than 500 Da.
        /// </summary>
        /// <returns>The HashMap of the symbol linked with the maximum occurrence</returns>
        private Dictionary<string, int> GetWisley_500()
        {
            Dictionary<string, int> map = new Dictionary<string, int>();

            map["C"] = 39;
            map["H"] = 72;
            map["N"] = 20;
            map["O"] = 20;
            map["P"] = 9;
            map["S"] = 10;
            map["F"] = 16;
            map["Cl"] = 10;
            map["Br"] = 4;
            map["Br"] = 8;

            return map;
        }

        /// <summary>
        /// Get the map linking the symbol of the element and number maximum of occurrence.
        /// For the analysis with the Wisley database and mass lower than 1000 Da.
        /// </summary>
        /// <returns>The HashMap of the symbol linked with the maximum occurrence</returns>
        private Dictionary<string, int> GetWisley_1000()
        {
            Dictionary<string, int> map = new Dictionary<string, int>();

            map["C"] = 78;
            map["H"] = 126;
            map["N"] = 20;
            map["O"] = 27;
            map["P"] = 9;
            map["S"] = 14;
            map["F"] = 34;
            map["Cl"] = 12;
            map["Br"] = 8;
            map["Si"] = 14;

            return map;
        }

        /// <summary>
        /// Get the map linking the symbol of the element and number maximum of occurrence.
        /// For the analysis with the Wisley database and mass lower than 2000 Da.
        /// </summary>
        /// <returns>The HashMap of the symbol linked with the maximum occurrence</returns>
        private Dictionary<string, int> GetWisley_2000()
        {
            Dictionary<string, int> map = new Dictionary<string, int>();

            map["C"] = 156;
            map["H"] = 180;
            map["N"] = 20;
            map["O"] = 40;
            map["P"] = 9;
            map["S"] = 14;
            map["F"] = 48;
            map["Cl"] = 12;
            map["Br"] = 10;
            map["Si"] = 15;

            return map;
        }
    }
}

