/* Copyright (C) 2007  Miguel Rojasch <miguelrojasch@users.sf.net>
 *               2014  Mark B Vine (orcid:0000-0002-7794-0426)
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

using NCDK.Config;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace NCDK.Tools.Manipulator
{
    /// <summary>
    /// Class with convenience methods that provide methods to manipulate <see cref="IMolecularFormula"/>'s.
    /// </summary>
    // @cdk.module  formula
    // @author      miguelrojasch
    // @cdk.created 2007-11-20
    // @cdk.githash
    public static class MolecularFormulaManipulator
    {
        /// <summary>
        /// Checks a set of Nodes for the occurrence of each isotopes
        /// instance in the molecular formula. In short number of atoms.
        /// </summary>
        /// <param name="formula">The MolecularFormula to check</param>
        /// <returns>The occurrence total</returns>
        public static int GetAtomCount(IMolecularFormula formula)
        {
            int count = 0;
            foreach (var isotope in formula.Isotopes)
            {
                count += formula.GetCount(isotope);
            }
            return count;
        }

        /// <summary>
        /// Checks a set of Nodes for the occurrence of the isotopes in the
        /// molecular formula from a particular IElement. It returns 0 if the
        /// element does not exist. The search is based only on the IElement.
        /// </summary>
        /// <param name="formula">The MolecularFormula to check</param>
        /// <param name="element">The IElement object</param>
        /// <returns>The occurrence of this element in this molecular formula</returns>
        public static int GetElementCount(IMolecularFormula formula, IElement element)
        {
            int count = 0;
            foreach (var isotope in formula.Isotopes)
            {
                if (isotope.Symbol.Equals(element.Symbol, StringComparison.Ordinal))
                    count += formula.GetCount(isotope);
            }
            return count;
        }

        /// <summary>
        /// Occurrences of a given element from an isotope in a molecular formula.
        /// </summary>
        /// <param name="formula">the formula</param>
        /// <param name="isotope">isotope of an element</param>
        /// <returns>number of the times the element occurs</returns>
        /// <seealso cref="GetElementCount(IMolecularFormula, IElement)"/>
        public static int GetElementCount(IMolecularFormula formula, IIsotope isotope)
        {
            return GetElementCount(formula, formula.Builder.NewElement(isotope));
        }

        /// <summary>
        /// Occurrences of a given element in a molecular formula.
        /// </summary>
        /// <param name="formula">the formula</param>
        /// <param name="symbol">element symbol (e.g. C for carbon)</param>
        /// <returns>number of the times the element occurs</returns>
        /// <seealso cref="GetElementCount(IMolecularFormula, IElement)"/>
        public static int GetElementCount(IMolecularFormula formula, string symbol)
        {
            return GetElementCount(formula, formula.Builder.NewElement(symbol));
        }

        /// <summary>
        /// Get a list of <see cref="IIsotope"/> from a given <paramref name="element"/> which is contained
        /// molecular. The search is based only on the <see cref="IElement"/>.
        /// </summary>
        /// <param name="formula">The <see cref="IMolecularFormula"/> to check</param>
        /// <param name="element">The <see cref="IElement"/> object</param>
        /// <returns>The list with the IIsotopes in this molecular formula</returns>
        public static IEnumerable<IIsotope> GetIsotopes(IMolecularFormula formula, IElement element)
        {
            foreach (var isotope in formula.Isotopes)
            {
                if (isotope.Symbol.Equals(element.Symbol, StringComparison.Ordinal))
                    yield return isotope;
            }
            yield break;
        }

        /// <summary>
        ///  Get a list of all <see cref="IElement"/>s which are contained molecular.
        /// </summary>
        /// <param name="formula">The molecular formula to check</param>
        /// <returns>The list with the elements in this molecular formula</returns>
        public static IEnumerable<IElement> Elements(IMolecularFormula formula)
        {
            List<string> stringList = new List<string>();
            foreach (var isotope in formula.Isotopes)
            {
                if (!stringList.Contains(isotope.Symbol))
                {
                    yield return isotope;
                    stringList.Add(isotope.Symbol);
                }
            }
            yield break;
        }

        /// <summary>
        /// True, if the <paramref name="formula"/> contains the given element as <see cref="IIsotope"/> object.
        /// </summary>
        /// <param name="formula">molecular formula</param>
        /// <param name="element">The element this <see cref="IMolecularFormula"/> is searched for</param>
        /// <returns>True, if the MolecularFormula contains the given element object</returns>
        public static bool ContainsElement(IMolecularFormula formula, IElement element)
        {
            foreach (var isotope in formula.Isotopes)
            {
                if (element.Symbol.Equals(isotope.Symbol, StringComparison.Ordinal))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Removes all isotopes from a given element in the molecular formula.
        /// </summary>
        /// <param name="formula">molecular formula</param>
        /// <param name="element">The <see cref="IElement"/> of the IIsotopes to be removed</param>
        /// <returns>The molecularFormula with the isotopes removed</returns>
        public static IMolecularFormula RemoveElement(IMolecularFormula formula, IElement element)
        {
            foreach (var isotope in GetIsotopes(formula, element).ToList())
            {
                formula.Remove(isotope);
            }
            return formula;
        }

        /// <summary>
        /// Returns the string representation of the molecule formula.
        /// </summary>
        /// <param name="formula">The IMolecularFormula Object</param>
        /// <param name="orderElements">The order of Elements</param>
        /// <param name="setOne">True, when must be set the value 1 for elements with one atom</param>
        /// <returns>A string containing the molecular formula</returns>
        /// <seealso cref="GetHTML(IMolecularFormula)"/>
        /// <seealso cref="OrderEle"/>
        /// <seealso cref="OrderEleHillNoCarbons"/>
        /// <seealso cref="OrderEleHillWithCarbons"/>
        public static string GetString(IMolecularFormula formula, IReadOnlyList<string> orderElements, bool setOne)
        {
            var stringMF = new StringBuilder();
            var isotopesList = PutInOrder(orderElements, formula);

            // collect elements in a map - since different isotopes of the
            // same element will get repeated in the formula
            var elemSet = new List<string>();
            foreach (var isotope in isotopesList)
            {
                var symbol = isotope.Symbol;
                if (!elemSet.Contains(symbol))
                    elemSet.Add(symbol);
            }

            foreach (var elem in elemSet)
            {
                int count = 0;
                foreach (var isotope in formula.Isotopes)
                {
                    if (isotope.Symbol.Equals(elem, StringComparison.Ordinal))
                        count += formula.GetCount(isotope);
                }
                stringMF.Append(elem);
                if (!(count == 1 && !setOne))
                    stringMF.Append(count);
            }
            return stringMF.ToString();
        }

        /// <summary>
        /// Returns the string representation of the molecule formula.
        /// Based on Hill System. The Hill system is a system of writing
        /// chemical formulas such that the number of carbon atoms in a
        /// molecule is indicated first, the number of hydrogen atoms next,
        /// and then the number of all other chemical elements subsequently,
        /// in alphabetical order. When the formula contains no carbon, all
        /// the elements, including hydrogen, are listed alphabetically.
        /// </summary>
        /// <param name="formula">The IMolecularFormula Object</param>
        /// <returns>A string containing the molecular formula</returns>
        /// <seealso cref="GetHTML(IMolecularFormula)"/>
        public static string GetString(IMolecularFormula formula)
        {
            return GetString(formula, false);
        }

        /// <summary>
        /// Returns the string representation of the molecule formula.
        /// Based on Hill System. The Hill system is a system of writing
        /// chemical formulas such that the number of carbon atoms in a
        /// molecule is indicated first, the number of hydrogen atoms next,
        /// and then the number of all other chemical elements subsequently,
        /// in alphabetical order. When the formula contains no carbon, all
        /// the elements, including hydrogen, are listed alphabetically.
        /// </summary>
        /// <param name="formula">The IMolecularFormula Object</param>
        /// <param name="setOne">True, when must be set the value 1 for elements with one atom</param>
        /// <returns>A string containing the molecular formula</returns>
        /// <seealso cref="GetHTML(IMolecularFormula)"/>
        public static string GetString(IMolecularFormula formula, bool setOne)
        {
            if (ContainsElement(formula, formula.Builder.NewElement("C")))
                return GetString(formula, OrderEleHillWithCarbons, setOne);
            else
                return GetString(formula, OrderEleHillNoCarbons, setOne);
        }

        public static IReadOnlyList<IIsotope> PutInOrder(IReadOnlyList<string> orderElements, IMolecularFormula formula)
        {
            var isotopesList = new List<IIsotope>();
            foreach (var orderElement in orderElements)
            {
                var element = formula.Builder.NewElement(orderElement);
                if (ContainsElement(formula, element))
                {
                    var isotopes = GetIsotopes(formula, element);
                    foreach (var isotope in isotopes)
                    {
                        isotopesList.Add(isotope);
                    }
                }
            }
            return isotopesList;
        }

        [Obsolete("Use" + nameof(GetString))]
        public static string GetHillString(IMolecularFormula formula)
        {
            var hillString = new StringBuilder();

            var hillMap = new SortedDictionary<string, int>();
            foreach (var isotope in formula.Isotopes)
            {
                string symbol = isotope.Symbol;
                if (hillMap.ContainsKey(symbol))
                    hillMap[symbol] = hillMap[symbol] + formula.GetCount(isotope);
                else
                    hillMap[symbol] = formula.GetCount(isotope);
            }

            // if we have a C append it and also add in the H
            // and then remove these elements
            int count;
            if (hillMap.ContainsKey("C"))
            {
                hillString.Append('C');
                count = hillMap["C"];
                if (count > 1) hillString.Append(count);
                hillMap.Remove("C");
                if (hillMap.ContainsKey("H"))
                {
                    hillString.Append('H');
                    count = hillMap["H"];
                    if (count > 1) hillString.Append(count);
                    hillMap.Remove("H");
                }
            }

            // now take all the rest in alphabetical order
            foreach (var key in hillMap.Keys)
            {
                hillString.Append(key);
                count = hillMap[key];
                if (count > 1)
                    hillString.Append(count);
            }
            return hillString.ToString();
        }

        /// <summary>
        /// Returns the string representation of the molecule formula based on Hill
        /// System with numbers wrapped in &lt;sub&gt;&lt;/sub&gt; tags. Useful for
        /// displaying formulae in Swing components or on the web.
        /// </summary>
        /// <param name="formula">The IMolecularFormula object</param>
        /// <returns>A HTML representation of the molecular formula</returns>
        /// <seealso cref="GetHTML(IMolecularFormula, bool, bool)"/>
        public static string GetHTML(IMolecularFormula formula)
        {
            return GetHTML(formula, true, true);
        }

        /// <summary>
        /// Returns the string representation of the molecule formula based on Hill
        /// System with numbers wrapped in &lt;sub&gt;&lt;/sub&gt; tags and the
        /// isotope of each Element in &lt;sup&gt;&lt;/sup&gt; tags and the total
        /// charge of IMolecularFormula in &lt;sup&gt;&lt;/sup&gt; tags. Useful for
        /// displaying formulae in Swing components or on the web.
        /// </summary>
        /// <param name="formula">The IMolecularFormula object</param>
        /// <param name="chargeB">True, If it has to show the charge</param>
        /// <param name="isotopeB">True, If it has to show the Isotope mass</param>
        /// <returns>A HTML representation of the molecular formula</returns>
        /// <seealso cref="GetHTML(IMolecularFormula)"/>
        public static string GetHTML(IMolecularFormula formula, bool chargeB, bool isotopeB)
        {
            var orderElements = 
                ContainsElement(formula, formula.Builder.NewElement("C"))
              ? OrderEleHillWithCarbons
              : OrderEleHillNoCarbons;
            return GetHTML(formula, orderElements, chargeB, isotopeB);
        }

        /// <summary>
        /// Returns the string representation of the molecule formula with numbers
        /// wrapped in &lt;sub&gt;&lt;/sub&gt; tags and the isotope of each Element
        /// in &lt;sup&gt;&lt;/sup&gt; tags and the total showCharge of IMolecularFormula
        /// in &lt;sup&gt;&lt;/sup&gt; tags. Useful for displaying formulae in Swing
        /// components or on the web.
        /// </summary>
        /// <param name="formula">The IMolecularFormula object</param>
        /// <param name="orderElements">The order of Elements</param>
        /// <param name="showCharge">True, If it has to show the showCharge</param>
        /// <param name="showIsotopes">True, If it has to show the Isotope mass</param>
        /// <returns>A HTML representation of the molecular formula</returns>
        /// <seealso cref="GetHTML(IMolecularFormula)"/>
        public static string GetHTML(IMolecularFormula formula, IReadOnlyList<string> orderElements, bool showCharge, bool showIsotopes)
        {
            var sb = new StringBuilder();
            foreach (var orderElement in orderElements)
            {
                var element = formula.Builder.NewElement(orderElement);
                if (ContainsElement(formula, element))
                {
                    if (!showIsotopes)
                    {
                        sb.Append(element.Symbol);
                        var n = GetElementCount(formula, element);
                        if (n > 1)
                        {
                            sb.Append("<sub>").Append(n).Append("</sub>");
                        }
                    }
                    else
                    {
                        foreach (var isotope in GetIsotopes(formula, element))
                        {
                            var massNumber = isotope.MassNumber;
                            if (massNumber != null)
                                sb.Append("<sup>").Append(massNumber).Append("</sup>");
                            sb.Append(isotope.Symbol);
                            var n = formula.GetCount(isotope);
                            if (n > 1)
                            {
                                sb.Append("<sub>").Append(n).Append("</sub>");
                            }
                        }
                    }
                }
            }

            if (showCharge)
            {
                var charge = formula.Charge;
                if (charge == null || charge == 0)
                {
                    return sb.ToString();
                }
                else
                {
                    sb.Append("<sup>");
                    if (charge > 1 || charge < -1)
                        sb.Append(Math.Abs(charge.Value));
                    if (charge > 0)
                        sb.Append('+');
                    else
                        sb.Append(MINUS); // note, not a hyphen!
                    sb.Append("</sup>");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Construct an instance of IMolecularFormula, initialized with a molecular
        /// formula string. The string is immediately analyzed and a set of Nodes
        /// is built based on this analysis
        /// <para>The hydrogens must be implicit.</para>
        /// </summary>
        /// <param name="stringMF">The molecularFormula string</param>
        /// <param name="builder">a IChemObjectBuilder which is used to construct atoms</param>
        /// <returns>The filled IMolecularFormula</returns>
        /// <seealso cref="GetMolecularFormula(string, IMolecularFormula)"/>
        public static IMolecularFormula GetMolecularFormula(string stringMF, IChemObjectBuilder builder)
        {
            return GetMolecularFormula(stringMF, false, builder);
        }

        /// <summary>
        /// Construct an instance of IMolecularFormula, initialized with a molecular
        /// formula string. The string is immediately analyzed and a set of Nodes
        /// is built based on this analysis. The hydrogens must be implicit. Major
        /// isotopes are being used.
        /// </summary>
        /// <param name="stringMF">The molecularFormula string</param>
        /// <param name="builder">a IChemObjectBuilder which is used to construct atoms</param>
        /// <returns>The filled IMolecularFormula</returns>
        /// <seealso cref="GetMolecularFormula(string, IMolecularFormula)"/>
        public static IMolecularFormula GetMajorIsotopeMolecularFormula(string stringMF, IChemObjectBuilder builder)
        {
            return GetMolecularFormula(stringMF, true, builder);
        }

        private static IMolecularFormula GetMolecularFormula(string stringMF, bool assumeMajorIsotope, IChemObjectBuilder builder)
        {
            IMolecularFormula formula = builder.NewMolecularFormula();

            return GetMolecularFormula(stringMF, formula, assumeMajorIsotope);
        }

        private const char HYPHEN = '-';
        private const char MINUS = '–';
        private const string HYPHEN_STR = "-";
        private const string MINUS_STR = "–";

        /// <summary>
        /// add in a instance of IMolecularFormula the elements extracts form
        /// molecular formula string. The string is immediately analyzed and a set of Nodes
        /// is built based on this analysis
        /// <para> The hydrogens must be implicit.</para>
        /// </summary>
        /// <param name="stringMF">The molecularFormula string</param>
        /// <returns>The filled IMolecularFormula</returns>
        /// <seealso cref="GetMolecularFormula(string, IChemObjectBuilder)"/>
        public static IMolecularFormula GetMolecularFormula(string stringMF, IMolecularFormula formula)
        {
            return GetMolecularFormula(stringMF, formula, false);
        }

        /// <summary>
        /// Add to an instance of IMolecularFormula the elements extracts form
        /// molecular formula string. The string is immediately analyzed and a set of Nodes
        /// is built based on this analysis. The hydrogens are assumed to be implicit.
        /// The bool indicates if the major isotope is to be assumed, or if no
        /// assumption is to be made.
        /// </summary>
        /// <param name="stringMF">The molecularFormula string</param>
        /// <param name="assumeMajorIsotope">If true, it will take the major isotope for each element</param>
        /// <returns>The filled IMolecularFormula</returns>
        /// <seealso cref="GetMolecularFormula(string, IChemObjectBuilder)"/>
        /// <seealso cref="GetMolecularFormula(string, bool, IChemObjectBuilder)"/>
        private static IMolecularFormula GetMolecularFormula(string stringMF, IMolecularFormula formula, bool assumeMajorIsotope)
        {
            if (stringMF.Contains(".") || stringMF.Contains("(") || (stringMF.Length > 0 && stringMF[0] >= '0' && stringMF[0] <= '9'))
                stringMF = SimplifyMolecularFormula(stringMF);

            // Extract charge from string when contains []X- format
            int? charge = null;
            if ((stringMF.Contains("[") && stringMF.Contains("]")) && (stringMF.Contains("+") || stringMF.Contains(HYPHEN_STR) || stringMF.Contains(MINUS_STR)))
            {
                charge = ExtractCharge(stringMF);
                stringMF = CleanMFfromCharge(stringMF);
            }

            // FIXME: MF: variables with lower case first char
            char ThisChar;

            // Buffer for
            var RecentElementSymbol = "";
            var RecentElementCountString = "0";
            // string to be converted to an integer

            int RecentElementCount;

            if (stringMF.Length == 0)
            {
                return null;
            }

            for (int f = 0; f < stringMF.Length; f++)
            {
                ThisChar = stringMF[f];
                if (f < stringMF.Length)
                {
                    if (ThisChar >= 'A' && ThisChar <= 'Z')
                    {
                        //
                        // New Element begins
                        //
                        RecentElementSymbol = new string(new[] { ThisChar });
                        RecentElementCountString = "0";
                    }
                    if (ThisChar >= 'a' && ThisChar <= 'z')
                    {
                        // Two-letter Element continued
                        RecentElementSymbol += ThisChar;
                    }
                    if (ThisChar >= '0' && ThisChar <= '9')
                    {
                        // Two-letter Element continued
                        RecentElementCountString += ThisChar;
                    }
                }
                if (f == stringMF.Length - 1 || (stringMF[f + 1] >= 'A' && stringMF[f + 1] <= 'Z'))
                {
                    // Here an element symbol as well as its number should have been read completely
                    RecentElementCount = int.Parse(RecentElementCountString, NumberFormatInfo.InvariantInfo);
                    if (RecentElementCount == 0)
                    {
                        RecentElementCount = 1;
                    }

                    var isotope = formula.Builder.NewIsotope(RecentElementSymbol);
                    if (assumeMajorIsotope)
                    {
                        try
                        {
                            isotope = BODRIsotopeFactory.Instance.GetMajorIsotope(RecentElementSymbol);
                        }
                        catch (IOException)
                        {
                            throw new ApplicationException("Cannot load the IsotopeFactory");
                        }
                    }
                    formula.Add(isotope, RecentElementCount);

                }
            }
            if (charge != null)
                formula.Charge = charge;
            return formula;
        }

        /// <summary>
        /// Extract the molecular formula when it is defined with charge. e.g. [O3S]2-.
        /// </summary>
        /// <param name="formula">The formula to inspect</param>
        /// <returns>The corrected formula</returns>
        private static string CleanMFfromCharge(string formula)
        {
            if (!(formula.Contains("[") && formula.Contains("]")))
                return formula;
            bool startBreak = false;
            string finalFormula = "";
            for (int f = 0; f < formula.Length; f++)
            {
                char thisChar = formula[f];
                if (thisChar == '[')
                {
                    // start
                    startBreak = true;
                }
                else if (thisChar == ']')
                {
                    break;
                }
                else if (startBreak)
                    finalFormula += thisChar;
            }
            return finalFormula;
        }

        /// <summary>
        /// Extract the charge given a molecular formula format [O3S]2-.
        /// </summary>
        /// <param name="formula">The formula to inspect</param>
        /// <returns>The charge</returns>
        private static int ExtractCharge(string formula)
        {
            if (!(formula.Contains("[") && formula.Contains("]") && (formula.Contains("+") || formula.Contains(HYPHEN_STR) || formula.Contains(MINUS_STR))))
                return 0;

            bool finishBreak = false;
            string multiple = "";
            for (int f = 0; f < formula.Length; f++)
            {
                char thisChar = formula[f];
                switch (thisChar)
                {
                    case ']':
                        // finish
                        finishBreak = true;
                        break;
                    case HYPHEN:
                    case MINUS:
                        multiple = HYPHEN + multiple;
                        goto Exit_For;
                    case '+':
                        goto Exit_For;
                    default:
                        if (finishBreak)
                        {
                            multiple += thisChar;
                        }
                        break;
                }
            }
        Exit_For:
            switch (multiple)
            {
                case "":
                case HYPHEN_STR:
                case MINUS_STR:
                    multiple += 1;
                    break;
            }
            return int.Parse(multiple, NumberFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Get the summed exact mass of all isotopes from an MolecularFormula. It
        /// assumes isotope masses to be preset, and returns 0.0 if not.
        /// </summary>
        /// <param name="formula">The IMolecularFormula to calculate</param>
        /// <returns>The summed exact mass of all atoms in this MolecularFormula</returns>
        public static double GetTotalExactMass(IMolecularFormula formula)
        {
            double mass = 0.0;

            foreach (var isotope in formula.Isotopes)
            {
                if (isotope.ExactMass == null)
                {
                    try
                    {
                        var majorIsotope = BODRIsotopeFactory.Instance.GetMajorIsotope(isotope.Symbol);
                        if (majorIsotope != null)
                        {
                            mass += majorIsotope.ExactMass.Value * formula.GetCount(isotope);
                        }
                    }
                    catch (IOException)
                    {
                        throw new ApplicationException("Could not instantiate the IsotopeFactory.");
                    }
                }
                else
                    mass += isotope.ExactMass.Value * formula.GetCount(isotope);
            }
            if (formula.Charge != null)
                mass = CorrectMass(mass, formula.Charge.Value);
            return mass;
        }

        /// <summary>
        /// Correct the mass according the charge of the IMmoleculeFormula.
        /// Negative charge will add the mass of one electron to the mass.
        /// </summary>
        /// <param name="mass">The mass to correct</param>
        /// <param name="charge">The charge</param>
        /// <returns>The mass with the correction</returns>
        private static double CorrectMass(double mass, int charge)
        {
            const double massE = 0.00054857990927;
            if (charge > 0)
                mass -= massE * charge;
            else if (charge < 0)
                mass += massE * Math.Abs(charge);
            return mass;
        }

        /// <summary>
        /// Get the summed mass number of all isotopes from an MolecularFormula. It
        /// assumes isotope masses to be preset, and returns 0.0 if not.
        /// </summary>
        /// <param name="formula">The IMolecularFormula to calculate</param>
        /// <returns>The summed nominal mass of all atoms in this MolecularFormula</returns>
        public static double GetTotalMassNumber(IMolecularFormula formula)
        {
            double mass = 0.0;
            foreach (var isotope in formula.Isotopes)
            {
                try
                {
                    var isotope2 = BODRIsotopeFactory.Instance.GetMajorIsotope(isotope.Symbol);
                    if (isotope2 != null)
                    {
                        mass += isotope2.MassNumber.Value * formula.GetCount(isotope);
                    }
                }
                catch (IOException e)
                {
                    Console.Error.WriteLine(e.StackTrace);
                }
            }
            return mass;
        }

        /// <summary>
        /// Get the summed natural mass of all elements from an MolecularFormula.
        /// </summary>
        /// <param name="formula">The IMolecularFormula to calculate</param>
        /// <returns>The summed exact mass of all atoms in this MolecularFormula</returns>
        public static double GetNaturalExactMass(IMolecularFormula formula)
        {
            double mass = 0.0;
            var factory = BODRIsotopeFactory.Instance;
            foreach (var isotope in formula.Isotopes)
            {
                var isotopesElement = formula.Builder.NewElement(isotope);
                mass += factory.GetNaturalMass(isotopesElement) * formula.GetCount(isotope);
            }
            return mass;
        }

        /// <summary>
        /// Get the summed major isotopic mass of all elements from an MolecularFormula.
        /// </summary>
        /// <param name="formula">The IMolecularFormula to calculate</param>
        /// <returns>The summed exact major isotope masses of all atoms in this MolecularFormula</returns>
        public static double GetMajorIsotopeMass(IMolecularFormula formula)
        {
            double mass = 0.0;
            var factory = BODRIsotopeFactory.Instance;
            foreach (var isotope in formula.Isotopes)
            {
                var major = factory.GetMajorIsotope(isotope.Symbol);
                if (major != null)
                {
                    mass += major.ExactMass.Value * formula.GetCount(isotope);
                }
            }
            return mass;
        }

        /// <summary>
        /// Get the summed natural abundance of all isotopes from an MolecularFormula. Assumes
        /// abundances to be preset, and will return 0.0 if not.
        /// </summary>
        /// <param name="formula">The IMolecularFormula to calculate</param>
        /// <returns>The summed natural abundance of all isotopes in this MolecularFormula</returns>
        public static double GetTotalNaturalAbundance(IMolecularFormula formula)
        {
            double abundance = 1.0;
            foreach (var isotope in formula.Isotopes)
            {
                if (isotope.NaturalAbundance == null)
                    return 0.0;
                abundance = abundance * Math.Pow(isotope.NaturalAbundance.Value, formula.GetCount(isotope));
            }
            return abundance / Math.Pow(100, GetAtomCount(formula));
        }

        /// <summary>
        /// Returns the number of double bond equivalents in this molecule.
        /// </summary>
        /// <param name="formula">The IMolecularFormula to calculate</param>
        /// <returns>The number of DBEs</returns>
        /// <exception cref="CDKException">if DBE cannot be evaluated</exception>
        // @cdk.keyword DBE
        // @cdk.keyword double bond equivalent
        public static double GetDBE(IMolecularFormula formula)
        {
            var valencies = new int[5];
            var ac = GetAtomContainer(formula);
            var factory = CDK.StructgenAtomTypeFactory;

            for (int f = 0; f < ac.Atoms.Count; f++)
            {
                var types = factory.GetAtomTypes(ac.Atoms[f].Symbol);
                if (!types.Any())
                    throw new CDKException($"Calculation of double bond equivalents not possible due to problems with element {ac.Atoms[f].Symbol}");
                valencies[(int)types.First().BondOrderSum.Value]++;
            }
            return 1 + (valencies[4]) + (valencies[3] / 2) - (valencies[1] / 2);
        }

        /// <summary>
        /// Method that actually does the work of convert the atomContainer
        /// to IMolecularFormula.
        /// <para>The hydrogens must be implicit.</para>
        /// </summary>
        /// <param name="atomContainer">IAtomContainer object</param>
        /// <returns>a molecular formula object</returns>
        /// <seealso cref="GetMolecularFormula(IAtomContainer, IMolecularFormula)"/>
        public static IMolecularFormula GetMolecularFormula(IAtomContainer atomContainer)
        {
            var formula = atomContainer.Builder.NewMolecularFormula();

            return GetMolecularFormula(atomContainer, formula);
        }

        /// <summary>
        /// Method that actually does the work of convert the atomContainer
        /// to IMolecularFormula given a <see cref="IMolecularFormula"/>.
        /// <para>The hydrogens must be implicit.</para>
        /// </summary>
        /// <param name="atomContainer">IAtomContainer object</param>
        /// <param name="formula">IMolecularFormula molecularFormula to put the new Isotopes</param>
        /// <returns>the filled AtomContainer</returns>
        /// <seealso cref="GetMolecularFormula(IAtomContainer)"/>
        public static IMolecularFormula GetMolecularFormula(IAtomContainer atomContainer, IMolecularFormula formula)
        {
            int charge = 0;
            IAtom hAtom = null;
            foreach (var iAtom in atomContainer.Atoms)
            {
                formula.Add(iAtom);
                if (iAtom.FormalCharge != null)
                    charge += iAtom.FormalCharge.Value;

                if (iAtom.ImplicitHydrogenCount != null && (iAtom.ImplicitHydrogenCount.Value > 0))
                {
                    if (hAtom == null)
                        hAtom = atomContainer.Builder.NewAtom("H");
                    formula.Add(hAtom, iAtom.ImplicitHydrogenCount.Value);
                }
            }
            formula.Charge = charge;
            return formula;
        }

        /// <summary>
        /// Method that actually does the work of convert the <see cref="IMolecularFormula"/>
        /// to <see cref="IAtomContainer"/>.
        /// <para>The hydrogens must be implicit.</para>
        /// </summary>
        /// <returns>the filled AtomContainer</returns>
        /// <seealso cref="GetAtomContainer(IMolecularFormula, IAtomContainer)"/>
        public static IAtomContainer GetAtomContainer(IMolecularFormula formula)
        {
            var atomContainer = formula.Builder.NewAtomContainer();
            return GetAtomContainer(formula, atomContainer);
        }

        /// <summary>
        /// Method that actually does the work of convert the <see cref="IMolecularFormula"/>
        /// to <see cref="IAtomContainer"/> given a <see cref="IAtomContainer"/>.
        /// <para>The hydrogens must be implicit.</para>
        /// </summary>
        /// <param name="atomContainer">IAtomContainer to put the new Elements</param>
        /// <returns>the filled AtomContainer</returns>
        /// <see cref="GetAtomContainer(IMolecularFormula)"/>
        public static IAtomContainer GetAtomContainer(IMolecularFormula formula, IAtomContainer atomContainer)
        {
            foreach (var isotope in formula.Isotopes)
            {
                var occur = formula.GetCount(isotope);
                for (int i = 0; i < occur; i++)
                {
                    var atom = formula.Builder.NewAtom(isotope);
                    atomContainer.Atoms.Add(atom);
                }
            }
            return atomContainer;
        }

        /// <summary>
        /// Converts a formula string (like "C2H4") into an atom container with atoms
        /// but no bonds.
        /// </summary>
        /// <param name="formulaString">the formula to convert</param>
        /// <param name="builder">a chem object builder</param>
        /// <returns>atoms wrapped in an atom container</returns>
        public static IAtomContainer GetAtomContainer(string formulaString, IChemObjectBuilder builder)
        {
            return MolecularFormulaManipulator.GetAtomContainer(MolecularFormulaManipulator.GetMolecularFormula(formulaString, builder));
        }

        /// <summary>
        /// The Elements ordered according to (approximate) probability of occurrence.
        /// </summary>
        /// <remarks>This begins with the "elements of life" C, H, O, N, (Si, P, S, F, Cl),
        /// then continues with the "common" chemical synthesis ingredients, closing off
        /// with the tail-end of the periodic table in atom-number order and finally
        /// the generic R-group.
        /// </remarks>
        public static IReadOnlyList<string> OrderEle { get; } = new string[]
        {
            // Elements of life
            "C", "H", "O", "N", "Si", "P", "S", "F", "Cl",

            "Br", "I", "Sn", "B", "Pb", "Tl", "Ba", "In", "Pd", "Pt", "Os", "Ag", "Zr", "Se", "Zn", "Cu", "Ni",
            "Co", "Fe", "Cr", "Ti", "Ca", "K", "Al", "Mg", "Na", "Ce", "Hg", "Au", "Ir", "Re", "W", "Ta", "Hf",
            "Lu", "Yb", "Tm", "Er", "Ho", "Dy", "Tb", "Gd", "Eu", "Sm", "Pm", "Nd", "Pr", "La", "Cs", "Xe", "Te",
            "Sb", "Cd", "Rh", "Ru", "Tc", "Mo", "Nb", "Y", "Sr", "Rb", "Kr", "As", "Ge", "Ga", "Mn", "V", "Sc",
            "Ar", "Ne", "He", "Be", "Li",

            // rest of periodic table, in atom-number order.
            "Bi", "Po", "At", "Rn",
            // row-7 elements (including f-block)
            "Fr", "Ra", "Ac", "Th", "Pa", "U", "Np", "Pu", "Am", "Cm", "Bk", "Cf", "Es", "Fm", "Md", "No", "Lr",
            "Rf", "Db", "Sg", "Bh", "Hs", "Mt", "Ds", "Rg", "Cn",

            // The "odd one out": an unspecified R-group
            "R",
        };

        /// <summary>
        /// Returns the Elements in Hill system order for non-carbon-containing formulas
        /// (i.e. strict alphabetical order, with one-letter elements preceding two-letter elements.)
        /// The generic R-group is treated specially and comes last.
        /// </summary>
        private static IReadOnlyList<string> OrderEleHillNoCarbons { get; } = new string[]
        {
            "Ac", "Ag", "Al", "Am", "Ar", "As", "At", "Au", "B", "Ba", "Be", "Bh", "Bi", "Bk", "Br",
            "C", "Ca", "Cd", "Ce", "Cf", "Cl", "Cm", "Cn", "Co", "Cr", "Cs", "Cu", "Db", "Ds", "Dy", "Er", "Es",
            "Eu", "F", "Fe", "Fm", "Fr", "Ga", "Gd", "Ge", "H", "He", "Hf", "Hg", "Ho", "Hs", "I", "In", "Ir", "K",
            "Kr", "La", "Li", "Lr", "Lu", "Md", "Mg", "Mn", "Mo", "Mt", "N", "Na", "Nb", "Nd", "Ne", "Ni", "No",
            "Np", "O", "Os", "P", "Pa", "Pb", "Pd", "Pm", "Po", "Pr", "Pt", "Pu", "Ra", "Rb", "Re", "Rf", "Rg",
            "Rh", "Rn", "Ru", "S", "Sb", "Sc", "Se", "Sg", "Si", "Sm", "Sn", "Sr", "Ta", "Tb", "Tc", "Te", "Th",
            "Ti", "Tl", "Tm", "U", "V", "W", "Xe", "Y", "Yb", "Zn", "Zr",
            // The "odd one out": an unspecified R-group
            "R"
        };

        /// <summary>
        /// Returns the Elements in Hill system order for carbon-containing formulas
        /// (i.e. first carbon and hydrogen, and then the rest of the elements in strict
        /// alphabetical order, with one-letter elements preceding two-letter elements.)
        /// The generic R-group is treated specially and comes last.
        /// </summary>
        private static IReadOnlyList<string> OrderEleHillWithCarbons { get; } = new string[]
        {
            "C", "H", "Ac", "Ag", "Al", "Am", "Ar", "As", "At", "Au", "B", "Ba", "Be", "Bh", "Bi",
            "Bk", "Br", "Ca", "Cd", "Ce", "Cf", "Cl", "Cm", "Cn", "Co", "Cr", "Cs", "Cu", "Db", "Ds", "Dy", "Er",
            "Es", "Eu", "F", "Fe", "Fm", "Fr", "Ga", "Gd", "Ge", "He", "Hf", "Hg", "Ho", "Hs", "I", "In", "Ir",
            "K", "Kr", "La", "Li", "Lr", "Lu", "Md", "Mg", "Mn", "Mo", "Mt", "N", "Na", "Nb", "Nd", "Ne", "Ni",
            "No", "Np", "O", "Os", "P", "Pa", "Pb", "Pd", "Pm", "Po", "Pr", "Pt", "Pu", "Ra", "Rb", "Re", "Rf",
            "Rg", "Rh", "Rn", "Ru", "S", "Sb", "Sc", "Se", "Sg", "Si", "Sm", "Sn", "Sr", "Ta", "Tb", "Tc", "Te",
            "Th", "Ti", "Tl", "Tm", "U", "V", "W", "Xe", "Y", "Yb", "Zn", "Zr",
            // The "odd one out": an unspecified R-group
            "R"
        };

        /// <summary>
        /// Compare two IMolecularFormula looking at type and number of IIsotope and
        /// charge of the formula.
        /// </summary>
        /// <param name="formula1">The first IMolecularFormula</param>
        /// <param name="formula2">The second IMolecularFormula</param>
        /// <returns>True, if the both IMolecularFormula are the same</returns>
        public static bool Compare(IMolecularFormula formula1, IMolecularFormula formula2)
        {
            if (formula1.Charge != formula2.Charge)
                return false;

            if (formula1.IsotopesCount != formula2.IsotopesCount)
                return false;

            foreach (var isotope in formula1.Isotopes)
            {
                if (!formula2.Contains(isotope))
                    return false;

                if (formula1.GetCount(isotope) != formula2.GetCount(isotope))
                    return false;
            }

            foreach (var isotope in formula2.Isotopes)
            {
                if (!formula1.Contains(isotope))
                    return false;
                if (formula2.GetCount(isotope) != formula1.GetCount(isotope))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns a set of nodes excluding all the hydrogens.
        /// </summary>
        /// <param name="formula">The IMolecularFormula</param>
        /// <returns>The heavyElements value into a List</returns>
        // @cdk.keyword    hydrogen, removal
        public static IEnumerable<IElement> GetHeavyElements(IMolecularFormula formula)
        {
            foreach (var element in Elements(formula))
            {
                if (!string.Equals(element.Symbol, "H", StringComparison.Ordinal))
                {
                    yield return element;
                }
            }
            yield break;
        }

        /// <summary>
        /// Simplify the molecular formula. E.g the dot '.' character convention is
        /// used when dividing a formula into parts. In this case any numeral following a dot refers
        /// to all the elements within that part of the formula that follow it.
        /// </summary>
        /// <param name="formula">The molecular formula</param>
        /// <returns>The simplified molecular formula</returns>
        public static string SimplifyMolecularFormula(string formula)
        {
            string newFormula = formula;
            char thisChar;

            if (formula.Contains(" "))
            {
                newFormula = newFormula.Replace(" ", "");
            }
            if (!formula.Contains("."))
                return BreakExtractor(formula);

            List<string> listMF = new List<string>();
            while (newFormula.Contains("."))
            {
                int pos = newFormula.IndexOf('.');
                string thisFormula = newFormula.Substring(0, pos);
                if (thisFormula[0] >= '0' && thisFormula[0] <= '9')
                    thisFormula = MultipleExtractor(thisFormula);

                if (thisFormula.Contains("("))
                    thisFormula = BreakExtractor(thisFormula);

                listMF.Add(thisFormula);
                thisFormula = newFormula.Substring(pos + 1, newFormula.Length - (pos + 1));
                if (!thisFormula.Contains("."))
                {
                    if (thisFormula[0] >= '0' && thisFormula[0] <= '9')
                        thisFormula = MultipleExtractor(thisFormula);

                    if (thisFormula.Contains("("))
                        thisFormula = BreakExtractor(thisFormula);

                    listMF.Add(thisFormula);
                }
                newFormula = thisFormula;
            }
            if (newFormula.Contains("("))
                newFormula = BreakExtractor(newFormula);

            string recentElementSymbol = "";
            string recentElementCountString = "0";

            var eleSymb = new List<string>();
            var eleCount = new List<int>();
            for (int i = 0; i < listMF.Count; i++)
            {
                string thisFormula = listMF[i];
                for (int f = 0; f < thisFormula.Length; f++)
                {
                    thisChar = thisFormula[f];
                    if (f < thisFormula.Length)
                    {
                        if (thisChar >= 'A' && thisChar <= 'Z')
                        {
                            recentElementSymbol = new string(new[] { thisChar });
                            recentElementCountString = "0";
                        }
                        if (thisChar >= 'a' && thisChar <= 'z')
                        {
                            recentElementSymbol += thisChar;
                        }
                        if (thisChar >= '0' && thisChar <= '9')
                        {
                            recentElementCountString += thisChar;
                        }
                    }
                    if (f == thisFormula.Length - 1 || (thisFormula[f + 1] >= 'A' && thisFormula[f + 1] <= 'Z'))
                    {
                        int posit = eleSymb.IndexOf(recentElementSymbol);
                        int count = int.Parse(recentElementCountString, NumberFormatInfo.InvariantInfo);
                        if (posit == -1)
                        {
                            eleSymb.Add(recentElementSymbol);
                            eleCount.Add(count);
                        }
                        else
                        {
                            int countP = int.Parse(recentElementCountString, NumberFormatInfo.InvariantInfo);
                            if (countP == 0)
                                countP = 1;
                            int countA = eleCount[posit];
                            if (countA == 0)
                                countA = 1;
                            int value = countP + countA;
                            eleCount.RemoveAt(posit);
                            eleCount.Insert(posit, value);
                        }
                    }
                }
            }
            string newF = "";
            for (int i = 0; i < eleCount.Count; i++)
            {
                var element = eleSymb[i];
                var num = eleCount[i];
                if (num == 0)
                    newF += element;
                else
                    newF += element + num;
            }
            return newF;
        }

        /// <summary>
        /// The parenthesis convention is used to show a quantity by which a formula is multiplied.
        /// For example: (C12H20O11)2 really means that a C24H40O22 unit.
        /// </summary>
        /// <param name="formula">Formula to correct</param>
        /// <returns>Formula with the correction</returns>
        private static string BreakExtractor(string formula)
        {
            bool finalBreak = false;
            string recentformula = "";
            string multiple = "0";
            for (int f = 0; f < formula.Length; f++)
            {
                var thisChar = formula[f];
                if (thisChar == '(')
                {
                    // start
                }
                else if (thisChar == ')')
                {
                    // final
                    finalBreak = true;
                }
                else if (!finalBreak)
                {
                    recentformula += thisChar;
                }
                else
                {
                    multiple += thisChar;
                }
            }

            var finalformula = Muliplier(recentformula, int.Parse(multiple, NumberFormatInfo.InvariantInfo));
            return finalformula;
        }

        /// <summary>
        /// The starting with numeric value is used to show a quantity by which a formula is multiplied.
        /// For example: 2H2O really means that a H4O2 unit.
        /// </summary>
        /// <param name="formula">Formula to correct</param>
        /// <returns>Formula with the correction</returns>
        private static string MultipleExtractor(string formula)
        {
            var recentCompoundCount = new StringBuilder("0");
            var recentCompound = new StringBuilder();

            bool found = false;
            for (int f = 0; f < formula.Length; f++)
            {
                char thisChar = formula[f];
                if (thisChar >= '0' && thisChar <= '9')
                {
                    if (!found)
                        recentCompoundCount.Append(thisChar);
                    else
                        recentCompound.Append(thisChar);
                }
                else
                {
                    found = true;
                    recentCompound.Append(thisChar);
                }
            }
            return Muliplier(recentCompound.ToString(), int.Parse(recentCompoundCount.ToString(), NumberFormatInfo.InvariantInfo));
        }

        /// <summary>
        /// This method multiply all the element over a value.
        /// </summary>
        /// <param name="formula">Formula to correct</param>
        /// <param name="factor">Factor to multiply</param>
        /// <returns>Formula with the correction</returns>
        private static string Muliplier(string formula, int factor)
        {
            var finalformula = new StringBuilder();
            var recentElementSymbol = new StringBuilder();
            var recentElementCountString = new StringBuilder("0");
            for (int f = 0; f < formula.Length; f++)
            {
                char thisChar = formula[f];
                if (f < formula.Length)
                {
                    if (thisChar >= 'A' && thisChar <= 'Z')
                    {
                        recentElementSymbol = new StringBuilder().Append(thisChar);
                        recentElementCountString = new StringBuilder("0");
                    }
                    if (thisChar >= 'a' && thisChar <= 'z')
                    {
                        recentElementSymbol.Append(thisChar);
                    }
                    if (thisChar >= '0' && thisChar <= '9')
                    {
                        recentElementCountString.Append(thisChar);
                    }
                }
                if (f == formula.Length - 1 || (formula[f + 1] >= 'A' && formula[f + 1] <= 'Z'))
                {
                    int recentElementCount = int.Parse(recentElementCountString.ToString(), NumberFormatInfo.InvariantInfo);
                    if (recentElementCount == 0)
                        finalformula.Append(recentElementSymbol).Append(factor);
                    else
                        finalformula.Append(recentElementSymbol).Append(recentElementCount * factor);
                }
            }
            return finalformula.ToString();
        }

        /// <summary>
        /// Adjust the protonation of a molecular formula. This utility method adjusts the hydrogen isotope count
        /// and charge at the same time.
        /// </summary>
        /// <example>
        /// <code>
        /// IMolecularFormula mf = MolecularFormulaManipulator.GetMolecularFormula("[C6H5O]-", bldr);
        /// MolecularFormulaManipulator.AdjustProtonation(mf, +1); // now "C6H6O"
        /// MolecularFormulaManipulator.AdjustProtonation(mf, -1); // now "C6H5O-"
        /// </code>
        /// 
        /// The return value indicates whether the protonation could be adjusted:
        /// 
        /// <code>
        /// IMolecularFormula mf = MolecularFormulaManipulator.GetMolecularFormula("[Cl]-", bldr);
        /// MolecularFormulaManipulator.AdjustProtonation(mf, +0); // false still "[Cl]-"
        /// MolecularFormulaManipulator.AdjustProtonation(mf, +1); // true now "HCl"
        /// MolecularFormulaManipulator.AdjustProtonation(mf, -1); // true now "[Cl]-" (again)
        /// MolecularFormulaManipulator.AdjustProtonation(mf, -1); // false still "[Cl]-" (no H to remove!)
        /// </code>
        /// 
        /// The method tries to select an existing hydrogen isotope to augment. If no hydrogen isotopes are found
        /// a new major isotope (<sup>1</sup>H) is created.
        /// </example>
        /// <param name="mf">molecular formula</param>
        /// <param name="hcnt">the number of hydrogens to add/remove, (&gt;0 protonate:, &lt;0: deprotonate)</param>
        /// <returns>the protonation was be adjusted</returns>
        public static bool AdjustProtonation(IMolecularFormula mf, int hcnt)
        {
            if (mf == null)
                throw new ArgumentNullException(nameof(mf), "No formula provided");
            if (hcnt == 0)
                return false; // no protons to add

            IChemObjectBuilder bldr = mf.Builder;
            int chg = mf.Charge ?? 0;

            IIsotope proton = null;
            int pcount = 0;

            foreach (IIsotope iso in mf.Isotopes)
            {
                if (string.Equals("H", iso.Symbol, StringComparison.Ordinal))
                {
                    int count = mf.GetCount(iso);
                    if (count < hcnt)
                        continue;
                    // acceptable
                    if (proton == null &&
                        (iso.MassNumber == null || iso.MassNumber == 1))
                    {
                        proton = iso;
                        pcount = count;
                    }
                    // better
                    else if (proton != null &&
                               iso.MassNumber != null && iso.MassNumber == 1 &&
                               proton.MassNumber == null)
                    {
                        proton = iso;
                        pcount = count;
                    }
                }
            }

            if (proton == null && hcnt < 0)
            {
                return false;
            }
            else if (proton == null && hcnt > 0)
            {
                proton = bldr.NewIsotope("H");
                proton.MassNumber = 1;
            }

            mf.Remove(proton);
            if (pcount + hcnt > 0)
                mf.Add(proton, pcount + hcnt);
            mf.Charge = chg + hcnt;

            return true;
        }
    }
}
