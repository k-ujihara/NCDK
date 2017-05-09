
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NCDK.Fingerprint
{
    /// <summary>
    /// Default sets of atom containers aimed for use with the substructure.
    /// </summary>
    // @author egonw
    // @cdk.module fingerprint
    // @cdk.githash
    internal class StandardSubstructureSets
    {
        private static string[] smarts = null;

        /// <summary>
        /// The functional groups.
        /// </summary>
        /// <returns>A set of the functional groups.</returns>
        /// <exception cref="Exception">if there is an error parsing SMILES for the functional groups</exception>
        public static string[] GetFunctionalGroupSMARTS()
        {
            if (smarts != null) return smarts;

            string filename = "NCDK.Fingerprint.Data.SMARTS_InteLigand.txt";
            Stream ins = ResourceLoader.GetAsStream(filename);
            TextReader reader = new StreamReader(ins);

            List<string> tmp = new List<string>();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("#") || line.Trim().Length == 0) continue;
                string[] toks = line.Split(':');
                StringBuilder s = new StringBuilder();
                for (int i = 1; i < toks.Length - 1; i++)
                    s.Append(toks[i] + ":");
                s.Append(toks[toks.Length - 1]);
                tmp.Add(s.ToString().Trim());
            }
            smarts = tmp.ToArray();
            return smarts;
        }
    }
}
