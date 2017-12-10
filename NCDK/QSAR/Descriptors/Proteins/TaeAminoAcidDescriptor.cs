/* Copyright (C) 2004-2007  Rajarshi Guha <rajarshi@users.sourceforge.net>
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
using NCDK.QSAR.Results;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace NCDK.QSAR.Descriptors.Proteins
{
    /// <summary>
    /// An implementation of the TAE descriptors for amino acids.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The TAE descriptors (<token>cdk-cite-BREN1995</token> <token>cdk-cite-BREN1997</token> <token>cdk-cite-WHITE2003</token>)
    /// are derived from pre-calculated quantum mechanical parameters. This class
    /// uses the parameters for amino acids and thus evaluates a set of 147 descriptors for peptide
    /// sequences.
    /// </para>
    /// <para>
    /// The class expects that it will be supplied an object which implements the <see cref="IBioPolymer"/>. Thus ordinary
    /// AtomContainer objects will result in an exception.
    /// </para>
    /// <para>
    /// The descriptors are returned in the following order (see
    /// <see href="http://www.chem.rpi.edu/chemweb/recondoc/TAE.doc">here</see>
    /// for a detailed description of the individual descriptors):
    /// <pre>
    /// Energy Population VOLTAE SurfArea
    /// SIDel.Rho.N Del.Rho.NMin Del.Rho.NMax Del.Rho.NIA Del.Rho.NA1
    /// Del.Rho.NA2 Del.Rho.NA3 Del.Rho.NA4 Del.Rho.NA5 Del.Rho.NA6
    /// Del.Rho.NA7 Del.Rho.NA8 Del.Rho.NA9 Del.Rho.NA10 SIDel.K.N
    /// Del.K.Min Del.K.Max Del.K.IA Del.K.NA1 Del.K.NA2
    /// Del.K.NA3 Del.K.NA4 Del.K.NA5 Del.K.NA6 Del.K.NA7
    /// Del.K.NA8 Del.K.NA9 Del.K.NA10 SIK SIKMin
    /// SIKMax SIKIA SIKA1 SIKA2 SIKA3
    /// SIKA4 SIKA5 SIKA6 SIKA7 SIKA8
    /// SIKA9 SIKA10 SIDel.G.N Del.G.NMin Del.G.NMax
    /// Del.G.NIA Del.G.NA1 Del.G.NA2 Del.G.NA3 Del.G.NA4
    /// Del.G.NA5 Del.G.NA6 Del.G.NA7 Del.G.NA8 Del.G.NA9
    /// Del.G.NA10 SIG SIGMin SIGMax SIGIA
    /// SIGA1 SIGA2 SIGA3 SIGA4 SIGA5
    /// SIGA6 SIGA7 SIGA8 SIGA9 SIGA10
    /// SIEP SIEPMin SIEPMax SIEPIA SIEPA1
    /// SIEPA2 SIEPA3 SIEPA4 SIEPA5 SIEPA6
    /// SIEPA7 SIEPA8 SIEPA9 SIEPA10 EP1
    /// EP2 EP3 EP4 EP5 EP6
    /// EP7 EP8 EP9 EP10 PIPMin
    /// PIPMax PIPAvg PIP1 PIP2 PIP3
    /// PIP4 PIP5 PIP6 PIP7 PIP8
    /// PIP9 PIP10 PIP11 PIP12 PIP13
    /// PIP14 PIP15 PIP16 PIP17 PIP18
    /// PIP19 PIP20 Fuk FukMin FukMax
    /// Fuk1 Fuk2 Fuk3 Fuk4 Fuk5
    /// Fuk6 Fuk7 Fuk8 Fuk9 Fuk10
    /// Lapl LaplMin LaplMax Lapl1 Lapl2
    /// Lapl3 Lapl4 Lapl5 Lapl6 Lapl7
    /// Lapl8 Lapl9 Lapl10
    /// </pre>
    /// </para>
    /// <para>This descriptor uses these parameters:
    /// <list type="table">
    /// <item>
    /// <term>Name</term>
    /// <term>Default</term>
    /// <term>Description</term>
    /// </item>
    /// <item>
    /// <term></term>
    /// <term></term>
    /// <term>no parameters</term>
    /// </item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <seealso cref="IBioPolymer"/>
    // @author      Rajarshi Guha
    // @cdk.created 2006-08-23
    // @cdk.module  qsarprotein
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:taeAminoAcid
    public partial class TaeAminoAcidDescriptor : IMolecularDescriptor
    {
        private Dictionary<string, double[]> taeParams = new Dictionary<string, double[]>();
        private int ndesc = 147;

        private Dictionary<string, string> nametrans = new Dictionary<string, string>();

        private List<IMonomer> GetMonomers(IBioPolymer iBioPolymer)
        {
            List<IMonomer> monomList = new List<IMonomer>();

            var strands = iBioPolymer.GetStrandMap();
            var strandKeys = strands.Keys;
            foreach (var key in strandKeys)
            {
                IStrand aStrand = strands[key];
                var tmp = aStrand.GetMonomerMap();
                var keys = tmp.Keys;
                foreach (var o1 in keys)
                {
                    monomList.Add(tmp[o1]);
                }
            }

            return monomList;
        }

        private void LoadTAEParams()
        {
            string filename = "NCDK.QSAR.Descriptors.Data.taepeptides.txt";
            var ins = ResourceLoader.GetAsStream(this.GetType(), filename);
            if (ins == null)
            {
                Debug.WriteLine("Could not load the TAE peptide parameter data file");
                taeParams = null;
                return;
            }
            try
            {
                var breader = new StreamReader(ins);
                breader.ReadLine(); // throw away the header
                for (int i = 0; i < 60; i++)
                {
                    string line = breader.ReadLine();
                    string[] components = line.Split(',');
                    if (components.Length != (ndesc + 1))
                        throw new CDKException("TAE peptide data table seems to be corrupt");
                    string key = components[0].ToLowerInvariant().Trim();

                    double[] data = new double[ndesc];
                    for (int j = 1; j < components.Length; j++)
                        data[j - 1] = double.Parse(components[j]);

                    taeParams[key] = data;
                }
            }
            catch (IOException ioe)
            {
                Console.Error.WriteLine(ioe.StackTrace);
                taeParams = null;
                return;
            }
            catch (CDKException e)
            {
                Console.Error.WriteLine(e.StackTrace);
                taeParams = null;
                return;
            }

            Debug.WriteLine($"Loaded {taeParams.Count} TAE parameters for amino acids");
        }

        public TaeAminoAcidDescriptor()
        {
            nametrans["a"] = "ala";
            nametrans["c"] = "cys";
            nametrans["d"] = "asp";
            nametrans["e"] = "glu";
            nametrans["f"] = "phe";
            nametrans["g"] = "gly";
            nametrans["h"] = "his";
            nametrans["i"] = "ile";
            nametrans["k"] = "lys";
            nametrans["l"] = "leu";
            nametrans["m"] = "met";
            nametrans["n"] = "asn";
            nametrans["p"] = "pro";
            nametrans["q"] = "gln";
            nametrans["r"] = "arg";
            nametrans["s"] = "ser";
            nametrans["t"] = "thr";
            nametrans["v"] = "val";
            nametrans["w"] = "trp";
            nametrans["y"] = "tyr";

            LoadTAEParams();
        }

        public IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#taeAminoAcid",
                typeof(TaeAminoAcidDescriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        /// The parameters attribute of the TaeAminoAcidDescriptor object.
        /// </summary>
        public object[] Parameters { get { return null; } set { } }

        public IReadOnlyList<string> DescriptorNames
        {
            get
            {
                string[] names = new string[ndesc];
                for (int i = 0; i < names.Length; i++)
                    names[i] = "TAE" + i;
                return names;
            }
        }

        /// <summary>
        /// Tthe parameterNames attribute of the TaeAminOAcidDescriptor object.
        /// </summary>
        public IReadOnlyList<string> ParameterNames => null;

        /// <summary>
        /// Gets the parameterType attribute of the TaeAminoAcidDescriptor object.
        /// </summary>
        /// <param name="name">Description of the Parameter</param>
        /// <returns>The parameterType value</returns>
        public object GetParameterType(string name) => null;

        private DescriptorValue<ArrayResult<double>> GetDummyDescriptorValue(Exception e)
        {
            int ndesc = DescriptorNames.Count;
            ArrayResult<double> results = new ArrayResult<double>(ndesc);
            for (int i = 0; i < ndesc; i++)
                results.Add(double.NaN);
            return new DescriptorValue<ArrayResult<double>>(_Specification, ParameterNames, Parameters, results, DescriptorNames, e);
        }

        /// <summary>
        /// Calculates the 147 TAE descriptors for amino acids.
        /// </summary>
        /// <param name="container">Parameter is the atom container which should implement <see cref="IBioPolymer"/>.</param>
        /// <returns>A <see cref="ArrayResult{Double}"/> value representing the 147 TAE descriptors</returns>
        public DescriptorValue<ArrayResult<double>> Calculate(IAtomContainer container)
        {
            if (taeParams == null) return GetDummyDescriptorValue(new CDKException("TAE parameters were not initialized"));
            if (!(container is IBioPolymer))
                return GetDummyDescriptorValue(new CDKException("The molecule should be of type IBioPolymer"));

            IBioPolymer peptide = (IBioPolymer)container;

            // I assume that we get single letter names
            //Collection aas = peptide.GetMonomerNames();

            double[] desc = new double[ndesc];
            for (int i = 0; i < ndesc; i++)
                desc[i] = 0.0;

            List<IMonomer> monomers = GetMonomers(peptide);

            foreach (var monomer in monomers)
            {
                string o = monomer.MonomerName;

                if (o.Length == 0) continue;

                string olc = o.ToLowerInvariant()[0].ToString();
                string tlc = (string)nametrans[olc];

                Debug.WriteLine("Converted " + olc + " to " + tlc);

                // get the params for this AA
                Double[] parameters = (Double[])taeParams[tlc];

                for (int i = 0; i < ndesc; i++)
                    desc[i] += parameters[i];
            }

            ArrayResult<double> retval = new ArrayResult<double>(ndesc);
            for (int i = 0; i < ndesc; i++)
                retval.Add(desc[i]);

            return new DescriptorValue<ArrayResult<double>>(_Specification, ParameterNames, Parameters, retval, DescriptorNames);
        }

        DescriptorValue IMolecularDescriptor.Calculate(IAtomContainer container)
            => Calculate(container);

        /// <inheritdoc/>
        public IDescriptorResult DescriptorResultType { get; } = new ArrayResult<double>(147);
    }
}
