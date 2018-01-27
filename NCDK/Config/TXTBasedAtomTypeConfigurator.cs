using System;
using System.Collections.Generic;
using System.IO;

namespace NCDK.Config
{
    public class TXTBasedAtomTypeConfigurator
        : IAtomTypeConfigurator
    {
        private const string configFile = "NCDK.Config.Data.jmol_atomtypes.txt";

        public Stream Stream { get; set; }

        public TXTBasedAtomTypeConfigurator() { }

        public IEnumerable<IAtomType> ReadAtomTypes(IChemObjectBuilder builder)
        {
            if (Stream == null)
            {
                Stream = ResourceLoader.GetAsStream(configFile);
            }

            using (var reader = new StreamReader(Stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("#"))
                        continue;
                    var tokens = line.Split("\t ,;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length != 9)
                        throw new IOException("AtomTypeTable.ReadAtypes: " + "Wrong Number of fields");

                    IAtomType atomType;
                    try
                    {
                        var name = tokens[0];
                        var rootType = tokens[1];
                        var san = tokens[2];
                        var sam = tokens[3];
                        // skip the vdw radius value
                        var scovalent = tokens[5];
                        var sColorR = tokens[6];
                        var sColorG = tokens[7];
                        var sColorB = tokens[8];

                        var mass = double.Parse(sam);
                        var covalent = double.Parse(scovalent);
                        var atomicNumber = int.Parse(san);
                        var colorR = int.Parse(sColorR);
                        var colorG = int.Parse(sColorG);
                        var colorB = int.Parse(sColorB);

                        atomType = builder.NewAtomType(name, rootType);
                        atomType.AtomicNumber = atomicNumber;
                        atomType.ExactMass = mass;
                        atomType.CovalentRadius = covalent;
                        atomType.SetProperty(CDKPropertyName.Color, CDKPropertyName.RGB2Int(colorR, colorG, colorB));
                    }
                    catch (FormatException)
                    {
                        throw new IOException("AtomTypeTable.ReadAtypes: " + "Malformed Number");
                    }
                    yield return atomType;
                }
            }
            yield break;
        }
    }
}