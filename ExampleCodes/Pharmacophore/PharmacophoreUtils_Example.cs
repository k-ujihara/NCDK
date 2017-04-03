using System;
using System.Collections.Generic;
using System.IO;

namespace NCDK.Pharmacophore
{
    class PharmacophoreUtils_Example
    {
        void Main()
        {
            #region ReadPharmacophoreDefinitions
            using (var srm = new FileStream("mydefs.xml", FileMode.Open))
            {
                IList<PharmacophoreQuery> defs = PharmacophoreUtils.ReadPharmacophoreDefinitions(srm);
                Console.Out.WriteLine("Number of definitions = " + defs.Count);
                for (int i = 0; i < defs.Count; i++) {
                    Console.Out.WriteLine($"Desc: {defs[i].GetProperty<string>("description")}");
                }
            }
            #endregion
        }
    }
}
