using System;
using System.Collections.Generic;
using System.IO;

namespace NCDK.FaulonSignatures.Chemistry
{
    public class MoleculeReader
    {
        public static Molecule ReadMolfile(string filename)
        {
            Molecule molecule = null;
            try
            {
                using (var reader = new StreamReader(filename))
                {
                    string line;
                    var block = new List<string>();
                    while ((line = reader.ReadLine()) != null)
                    {
                        block.Add(line);
                    }
                    molecule = MakeMolecule(block);
                }
            }
            catch (IOException ioe)
            {
                Console.Error.WriteLine(ioe.ToString());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
            }
            return molecule;
        }

        /// <summary>
        /// Read a list of Molecule from SDFile
        /// </summary>
        /// <param name="filename">path to SDFile</param>
        /// <returns>List of Molecules</returns>
        public static List<Molecule> ReadSDFFile(string filename)
        {
            try
            {
                using (var stream = new FileStream(filename, FileMode.Open))
                {
                    return ReadSDFfromStream(stream);
                }
            }
            catch (FileNotFoundException e)
            {
                Console.Error.WriteLine(e.ToString());
            }
            return null;
        }

        /// <summary>
        /// Read a list of Molecule from an InputStream, providing SDFile contents
        /// </summary>
        /// <param name="stream">InputStream to read from</param>
        /// <returns>List of Molecules</returns>
        public static List<Molecule> ReadSDFfromStream(Stream stream)
        {
            List<Molecule> molecules = new List<Molecule>();
            try
            {
                using (var reader = new StreamReader(stream))
                {
                    string line;
                    int i = 0;
                    List<string> block = new List<string>();
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("$$$$"))
                        {
                            Molecule molecule;
                            try
                            {
                                molecule = MoleculeReader.MakeMolecule(block);
                                molecules.Add(molecule);
                            }
                            catch (Exception e)
                            {
                                Console.Error.WriteLine("failed for block " + i + " " + e);
                                Console.Error.WriteLine(e.StackTrace);
                            }
                            block.Clear();
                            i++;
                        }
                        else
                        {
                            block.Add(line);
                        }
                    }
                }
            }
            catch (IOException ioe)
            {
                Console.Error.WriteLine(ioe.ToString());
            }
            return molecules;
        }

        private static Molecule MakeMolecule(List<string> block)
        {
            Molecule molecule = new Molecule();
            // counts are on the fourth line
            string countLine = block[3];
            int atomCount = int.Parse(countLine.Substring(0, 3).Trim());
            int bondCount = int.Parse(countLine.Substring(3, 6).Trim());

            // atom block starts on the fifth line (4th index)
            int atomLineStart = 4;
            int atomLineEnd = atomCount + atomLineStart;
            for (int i = atomLineStart; i < atomLineEnd; i++)
            {
                string symbol = block[i].Substring(30, 33).Trim();
                int atomIndex = i - atomLineStart;
                molecule.AddAtom(atomIndex, symbol);
            }

            if (atomCount > 1)
            {
                // bond block starts right after the atom block
                int bondLineStart = atomLineEnd;
                int bondLineEnd = bondLineStart + bondCount;
                for (int i = bondLineStart; i < bondLineEnd; i++)
                {
                    string bondLine = block[i];
                    int atomNumberA =
                        int.Parse(bondLine.Substring(0, 3).Trim());
                    int atomNumberB =
                        int.Parse(bondLine.Substring(3, 6).Trim());
                    int order = int.Parse(bondLine.Substring(7, 10).Trim());
                    var o = ConvertIntToBondOrder(order);
                    molecule.AddBond(atomNumberA - 1, atomNumberB - 1, o);
                }
            }
            return molecule;
        }

        private static Molecule.BondOrder ConvertIntToBondOrder(int o)
        {
            switch (o)
            {
                case 1: return Molecule.BondOrder.Single;
                case 2: return Molecule.BondOrder.Double;
                case 3: return Molecule.BondOrder.Triple;
                case 4: return Molecule.BondOrder.Aromatic;
                default: return Molecule.BondOrder.Single;
            }
        }
    }
}
