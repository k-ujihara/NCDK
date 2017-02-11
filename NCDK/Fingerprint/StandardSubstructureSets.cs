using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NCDK.Fingerprint
{
    /**
    // Default sets of atom containers aimed for use with the substructure.
     *
    // @author egonw
     *
    // @cdk.module fingerprint
    // @cdk.githash
     */
#if TEST
    public
#endif
    class StandardSubstructureSets
    {
        private static string[] smarts = null;

        /**
        // The functional groups.
         *
        // @return A set of the functional groups.
        // @ if there is an error parsing SMILES for the functional groups
         */
        public static string[] GetFunctionalGroupSMARTS()
        {
            if (smarts != null) return smarts;

            string filename = "NCDK.Fingerprint.Data.SMARTS_InteLigand.txt";
            Stream ins = typeof(StandardSubstructureSets).Assembly.GetManifestResourceStream(filename);
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
