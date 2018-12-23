namespace NCDK
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;

    class Chem_Example
    {
        static void Main(string[] args)
        {
            var toluene = Chem.MolFromSmiles("Cc1ccccc1");
            var mol1 = Chem.MolFromFile("Data/input.mol");
            var stringWithMolData = new StreamReader("Data/input.mol").ReadToEnd();
            var mol2 = Chem.MolFromMolBlock(stringWithMolData);

            using (var suppl = Chem.SDMolSupplier("Data/5ht3ligs.sdf"))
            {
                Console.WriteLine(suppl.Count);
                foreach (var mol in suppl)
                {
                    if (mol == null)
                        continue;
                    Console.WriteLine(mol.Atoms.Count);
                }
                Console.WriteLine(suppl[0].Atoms.Count);
            }

            using (var fsuppl = Chem.ForwardSDMolSupplier(new FileStream("Data/5ht3ligs.sdf", FileMode.Open)))
            {
                foreach (var mol in fsuppl)
                {
                    if (mol == null)
                        continue;
                    Console.WriteLine(mol.Atoms.Count);
                }
            }

            using (var gzsuppl = Chem.ForwardSDMolSupplier(
                new GZipStream(
                    new FileStream("Data/actives_5ht3.sdf.gz", FileMode.Open), 
                    CompressionMode.Decompress)))
            {
                Console.WriteLine(gzsuppl.Count(x => x != null));
            }
        }
    }
}
