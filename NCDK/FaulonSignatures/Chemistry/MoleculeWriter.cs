using System;
using System.IO;

namespace NCDK.FaulonSignatures.Chemistry
{
    public class MoleculeWriter
    {
        public static void WriteMolfile(string filename, Molecule molecule)
        {
            try
            {
                using (var stream = new FileStream(filename, FileMode.Create))
                {
                    WriteToStream(stream, molecule);
                }
            }
            catch (FileNotFoundException e)
            {
                Console.Error.WriteLine(e.StackTrace);
            }
        }

        public static void WriteToStream(Stream stream, Molecule molecule)
        {
            try
            {
                using (var writer = new StreamWriter(stream))
                {
                    WriteHeader(writer, molecule);
                    for (int i = 0; i < molecule.GetAtomCount(); i++)
                    {
                        WriteAtom(writer, molecule, i);
                    }
                    for (int i = 0; i < molecule.GetBondCount(); i++)
                    {
                        WriteBond(writer, molecule, i);
                    }
                    writer.Write("M  END");
                    writer.WriteLine();
                }
            }
            catch (IOException)
            {
            }
        }

        private static void WriteHeader(
                TextWriter writer, Molecule molecule)
        {
            writer.WriteLine();
            writer.Write(" Written by signature package");
            writer.WriteLine();
            writer.WriteLine();
            int a = molecule.GetAtomCount();
            int b = molecule.GetBondCount();
            writer.Write(
                    string.Format("{0, 3D}{1, 3D}  0  0  0  0  0  0  0  0999 V2000", a, b));
            writer.WriteLine();
        }

        private static void WriteAtom(
                TextWriter writer, Molecule molecule, int i)
        {
            string empty3DCoords = "    0.0000    0.0000    0.0000 ";
            string emptyTail = " 0  0  0  0  0  0  0  0  0  0  0  0";
            string symbol = molecule.GetSymbolFor(i);
            writer.Write(empty3DCoords + string.Format("{0, -3}", symbol) + emptyTail);
            writer.WriteLine();
        }

        private static void WriteBond(
                TextWriter writer, Molecule molecule, int i)
        {
            int f = molecule.GetFirstInBond(i) + 1;
            int s = molecule.GetSecondInBond(i) + 1;
            int o = molecule.GetBondOrderAsInt(i);
            writer.Write(string.Format("{0, 3D}{1, 3D}{2, 3D} 0  0  0  0", f, s, o));
            writer.WriteLine();
        }
    }
}
