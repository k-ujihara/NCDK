/* Copyright (C) 2006-2007  Miguel Rojas <miguel.rojas@uni-koeln.de>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License along
 * with this program; if not, write to the Free Software Foundation, Inc.,
 * 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Common.Primitives;
using NCDK.QSAR.Result;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NCDK.QSAR.Descriptors.Atomic
{
    /**
     *  This class returns the ionization potential of an atom containing lone
     *  pair electrons. It is
     *  based on a decision tree which is extracted from Weka(J48) from
     *  experimental values. Up to now is only possible predict for
     *  Cl,Br,I,N,P,O,S Atoms and they are not belong to
     *  conjugated system or not adjacent to an double bond.
     *
     * <p>This descriptor uses these parameters:
     * <table border="1">
     *   <tr>
     *     <td>Name</td>
     *     <td>Default</td>
     *     <td>Description</td>
     *   </tr>
     *   <tr>
     *     <td></td>
     *     <td></td>
     *     <td>no parameters</td>
     *   </tr>
     * </table>
     *
     * @author       Miguel Rojas
     * @cdk.created  2006-05-26
     * @cdk.module   qsaratomic
     * @cdk.githash
     * @cdk.set      qsar-descriptors
     * @cdk.dictref  qsar-descriptors:ionizationPotential
     */
    public class IPAtomicHOSEDescriptor : AbstractAtomicDescriptor
    {
        private static readonly string[] NAMES = { "ipAtomicHOSE" };

        /// <summary> Maximum spheres to use by the HoseCode model.*/
        internal int maxSpheresToUse = 10;

        private IPdb db;

        /// <summary>
        ///  Constructor for the IPAtomicHOSEDescriptor object.
        /// </summary>
        public IPAtomicHOSEDescriptor()
        {
            db = new IPdb(this);
        }

        /// <summary>
        /// The specification attribute of the IPAtomicHOSEDescriptor object
        /// </summary>
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#ionizationPotential",
                typeof(IPAtomicHOSEDescriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the IPAtomicHOSEDescriptor object.
        /// </summary>
        public override object[] Parameters { get { return null; } set { } }

        public override string[] DescriptorNames => NAMES;

        /// <summary>
        ///  This method calculates the ionization potential of an atom.
        /// </summary>
        /// <param name="atom">The IAtom to ionize.</param>
        /// <param name="container">Parameter is the IAtomContainer.</param>
        /// <returns>The ionization potential. Not possible the ionization.</returns>
        public override DescriptorValue Calculate(IAtom atom, IAtomContainer container)
        {
            double value;
            // FIXME: for now I'll cache a few modified atomic properties, and restore them at the end of this method
            var originalAtomtypeName = atom.AtomTypeName;
            var originalNeighborCount = atom.FormalNeighbourCount;
            var originalValency = atom.Valency;
            var originalBondOrderSum = atom.BondOrderSum;
            var originalMaxBondOrder = atom.MaxBondOrder;
            var originalHybridization = atom.Hybridization;

            if (!IsCachedAtomContainer(container))
            {
                try
                {
                    AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(container);
                    LonePairElectronChecker lpcheck = new LonePairElectronChecker();
                    lpcheck.Saturate(container);
                }
                catch (CDKException e)
                {
                    return new DescriptorValue(_Specification, ParameterNames, Parameters, new DoubleResult(
                            double.NaN), NAMES, e);
                }

            }
            value = db.ExtractIP(container, atom);
            // restore original props
            atom.AtomTypeName = originalAtomtypeName;
            atom.FormalNeighbourCount = originalNeighborCount;
            atom.Valency = originalValency;
            atom.Hybridization = originalHybridization;
            atom.MaxBondOrder = originalMaxBondOrder;
            atom.BondOrderSum = originalBondOrderSum;

            return new DescriptorValue(_Specification, ParameterNames, Parameters, new DoubleResult(value),
                                       NAMES);

        }

        /// <summary>
        /// Looking if the Atom belongs to the halogen family.
        ///
        /// <param name="atom">The IAtom</param>
        /// <returns>True, if it belongs</returns>
        /// </summary>
        internal bool FamilyHalogen(IAtom atom)
        {
            string symbol = atom.Symbol;
            return symbol.Equals("F") || symbol.Equals("Cl") || symbol.Equals("Br") || symbol.Equals("I");
        }

        /// <summary>
        /// The parameterNames attribute of the IPAtomicHOSEDescriptor object.
        /// </summary>
        public override string[] ParameterNames { get; } = Array.Empty<string>();

        /// <summary>
        /// Gets the parameterType attribute of the IPAtomicHOSEDescriptor object.
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>An Object of class equal to that of the parameter being requested</returns>
        public override object GetParameterType(string name) => null;

        /// <summary>
        /// Class defining the database containing the relation between the energy for ionizing and the HOSEcode
        /// fingerprints
        /// </summary>
        // @author Miguel Rojas
        class IPdb
        {
            IPAtomicHOSEDescriptor parent;

            Dictionary<string, Dictionary<string, double>> listGroup = new Dictionary<string, Dictionary<string, double>>();
            Dictionary<string, Dictionary<string, double>> listGroupS = new Dictionary<string, Dictionary<string, double>>();

            /// <summary>
            /// The constructor of the IPdb.
            /// </summary>
            public IPdb(IPAtomicHOSEDescriptor parent)
            {
                this.parent = parent;
            }

            /// <summary>
            /// extract from the db the ionization energy.
            ///
            /// <param name="container">The IAtomContainer</param>
            /// <param name="atom">The IAtom</param>
            /// <returns>The energy value</returns>
            /// </summary>
            public double ExtractIP(IAtomContainer container, IAtom atom)
            {
                // loading the files if they are not done
                string name = "";
                string nameS = "";
                Dictionary<string, double> hoseVSenergy = new Dictionary<string, double>();
                Dictionary<string, double> hoseVSenergyS = new Dictionary<string, double>();
                if (parent.FamilyHalogen(atom))
                {
                    name = "X_IP_HOSE.db";
                    nameS = "X_IP_HOSE_S.db";
                    if (listGroup.ContainsKey(name))
                    {
                        hoseVSenergy = listGroup[name];
                        hoseVSenergyS = listGroupS[nameS];
                    }
                    else
                    {
                        string path = "NCDK.QSAR.Descriptors.Atomic.Data." + name;
                        string pathS = "NCDK.QSAR.Descriptors.Atomic.Data." + nameS;
                        var ins = this.GetType().Assembly.GetManifestResourceStream(path);
                        var insr = new StreamReader(ins);
                        hoseVSenergy = ExtractAttributes(insr);

                        ins = this.GetType().Assembly.GetManifestResourceStream(pathS);
                        insr = new StreamReader(ins);
                        hoseVSenergyS = ExtractAttributes(insr);
                    }
                }
                else
                    return 0;

                try
                {
                    HOSECodeGenerator hcg = new HOSECodeGenerator();
                    //Check starting from the exact sphere hose code and maximal a value of 10
                    int exactSphere = 0;
                    string hoseCode = "";
                    for (int spheres = parent.maxSpheresToUse; spheres > 0; spheres--)
                    {
                        hcg.GetSpheres(container, atom, spheres, true);
                        var atoms = hcg.GetNodesInSphere(spheres);
                        if (atoms.Count != 0)
                        {
                            exactSphere = spheres;
                            hoseCode = hcg.GetHOSECode(container, atom, spheres, true);
                            if (hoseVSenergy.ContainsKey(hoseCode))
                            {
                                return hoseVSenergy[hoseCode];
                            }
                            if (hoseVSenergyS.ContainsKey(hoseCode))
                            {
                                return hoseVSenergyS[hoseCode];
                            }
                            break;
                        }
                    }
                    //Check starting from the rings bigger and smaller
                    //TODO:IP: Better application
                    for (int i = 0; i < 3; i++)
                    { // two rings
                        for (int plusMinus = 0; plusMinus < 2; plusMinus++)
                        { // plus==bigger, minus==smaller
                            int sign = -1;
                            if (plusMinus == 1) sign = 1;

                            var st = Strings.Tokenize(hoseCode, '(', ')', '/').GetEnumerator();
                            StringBuilder hoseCodeBuffer = new StringBuilder();
                            int sum = exactSphere + sign * (i + 1);
                            for (int k = 0; k < sum; k++)
                            {
                                if (st.MoveNext())
                                {
                                    string partcode = st.Current;
                                    hoseCodeBuffer.Append(partcode);
                                }
                                if (k == 0)
                                {
                                    hoseCodeBuffer.Append('(');
                                }
                                else if (k == 3)
                                {
                                    hoseCodeBuffer.Append(')');
                                }
                                else
                                {
                                    hoseCodeBuffer.Append('/');
                                }
                            }
                            string hoseCodeBU = hoseCodeBuffer.ToString();

                            if (hoseVSenergyS.ContainsKey(hoseCodeBU))
                            {
                                return hoseVSenergyS[hoseCodeBU];
                            }
                        }
                    }
                }
                catch (CDKException e)
                {
                    Console.Error.WriteLine(e.StackTrace);
                }
                return 0;
            }

            /// <summary>
            /// Extract the Hose code and energy
            ///
            /// <param name="input">The BufferedReader</param>
            /// <returns>HashMap with the Hose vs energy attributes</returns>
            /// </summary>
            private Dictionary<string, double> ExtractAttributes(TextReader input)
            {
                Dictionary<string, double> hoseVSenergy = new Dictionary<string, double>();
                string line;

                try
                {
                    while ((line = input.ReadLine()) != null)
                    {
                        if (line.StartsWith("#")) continue;
                        List<string> values = ExtractInfo(line);
                        if (values[1].Equals("")) continue;
                        hoseVSenergy[values[0]] = double.Parse(values[1]);
                    }
                }
                catch (IOException e)
                {
                    Console.Error.WriteLine(e.StackTrace);
                }
                return hoseVSenergy;
            }
        }

        /// <summary>
        /// Extract the information from a line which contains HOSE_ID & energy.
        ///
        /// <param name="str">string with the information</param>
        /// <returns>List with string = HOSECode and string = energy</returns>
        /// </summary>
        private static List<string> ExtractInfo(string str)
        {

            StringBuilder idEdited = new StringBuilder();
            StringBuilder valEdited = new StringBuilder();

            int strlen = str.Length;

            bool foundSpace = false;
            int countSpace = 0;
            bool foundDigit = false;
            for (int i = 0; i < strlen; i++)
            {
                if (!foundDigit) if (char.IsLetter(str[i])) foundDigit = true;

                if (foundDigit)
                {
                    if (char.IsWhiteSpace(str[i]))
                    {
                        if (countSpace == 0)
                        {
                            foundSpace = true;
                        }
                        else
                            break;
                    }
                    else
                    {
                        if (foundSpace)
                        {
                            valEdited.Append(str[i]);
                        }
                        else
                        {
                            idEdited.Append(str[i]);
                        }
                    }
                }
            }
            List<string> objec = new List<string>();
            objec.Add(idEdited.ToString());
            objec.Add(valEdited.ToString());
            return objec;
        }
    }
}
