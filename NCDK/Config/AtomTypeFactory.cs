using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Linq;

namespace NCDK.Config
{
    public class AtomTypeFactory
    {
        public const string ID_Structgen = "structgen";
        public const string ID_Modeling = "modeling";
        public const string ID_Jmol = "jmol";

        private const string TXT_EXTENSION = ".txt";
        private const string XML_EXTENSION = ".xml";
        private const string OWL_EXTENSION = ".owl";

        private static object syncLock = new object();
        private static IDictionary<string, AtomTypeFactory> tables = new Dictionary<string, AtomTypeFactory>();
        private IDictionary<string, IAtomType> atomTypes = new Dictionary<string, IAtomType>();

        private AtomTypeFactory(string configFile, IChemObjectBuilder builder)
        {
            ReadConfiguration(configFile, builder);
        }

        private AtomTypeFactory(Stream ins, string format, IChemObjectBuilder builder)
        {
            ReadConfiguration(ins, format, builder);
        }

        public static AtomTypeFactory GetInstance(Stream ins, string format, IChemObjectBuilder builder)
        {
            return new AtomTypeFactory(ins, format, builder);
        }

        public static AtomTypeFactory GetInstance(IChemObjectBuilder builder)
        {
            return GetInstance("NCDK.Config.Data.structgen_atomtypes.xml", builder);
        }

        public static AtomTypeFactory GetInstance(string configFile, IChemObjectBuilder builder)
        {
            lock (syncLock)
            {
                AtomTypeFactory factory;
                if (!tables.TryGetValue(configFile, out factory))
                {
                    factory = new AtomTypeFactory(configFile, builder);
                    tables.Add(configFile, factory);
                }
                return factory;
            }
        }

        private void ReadConfiguration(string fileName, IChemObjectBuilder builder)
        {
            Trace.TraceInformation("Reading config file from " + fileName);
            var ins = ResourceLoader.GetAsStream(fileName);

            string format = Path.GetExtension(fileName);
            switch (format)
            {
                case TXT_EXTENSION:
                case XML_EXTENSION:
                case OWL_EXTENSION:
                    break;
                default:
                    format = XML_EXTENSION;
                    break;
            }

            ReadConfiguration(ins, format, builder);
        }

        private IAtomTypeConfigurator ConstructConfigurator(string format)
        {
            if (!format.StartsWith("."))
                format = "." + format;
            switch (format)
            {
                case TXT_EXTENSION:
                    return new TXTBasedAtomTypeConfigurator();
                case XML_EXTENSION:
                    return new CDKBasedAtomTypeConfigurator();
                case OWL_EXTENSION:
                    return new OWLBasedAtomTypeConfigurator();
                default:
                    return null;
            }
        }

        private void ReadConfiguration(Stream ins, string format, IChemObjectBuilder builder)
        {
            IAtomTypeConfigurator atc = ConstructConfigurator(format);
            if (atc != null)
            {
                atc.Stream = ins;
                try
                {
                    foreach (var type in atc.ReadAtomTypes(builder))
                        atomTypes[type.AtomTypeName] = new ImmutableAtomType(type);
                }
                catch (Exception exc)
                {
                    Trace.TraceError("Could not read AtomType's from file due to: {0}", exc.Message);
                    Debug.WriteLine(exc);
                }
            }
            else
            {
                Debug.WriteLine("AtomTypeConfigurator was null!");
                atomTypes = new Dictionary<string, IAtomType>();
            }
        }

        public int Count
        {
            get { return atomTypes.Count; }
        }

        public IAtomType GetAtomType(string identifier)
        {
            IAtomType type;
            if (identifier != null && atomTypes.TryGetValue(identifier, out type))
                return type;
            throw new NoSuchAtomTypeException($"The AtomType {identifier} could not be found");
        }

        public IEnumerable<IAtomType> GetAtomTypes(string symbol)
        {
            return atomTypes.Values.Where(n => n.Symbol == symbol);
        }

        public IEnumerable<IAtomType> GetAllAtomTypes()
        {
            return atomTypes.Values;
        }

        public IAtom Configure(IAtom atom)
        {
            if (atom is IPseudoAtom) {
                // do not try to configure PseudoAtom's
                return atom;
            }
            try
            {
                IAtomType atomType;
                if (string.IsNullOrEmpty(atom.AtomTypeName))
                    atomType = GetAtomTypes(atom.Symbol).First();
                else
                    atomType = GetAtomType(atom.AtomTypeName);
                atom.Symbol = atomType.Symbol;
                atom.MaxBondOrder = atomType.MaxBondOrder;
                atom.BondOrderSum = atomType.BondOrderSum;
                atom.CovalentRadius = atomType.CovalentRadius;
                atom.Hybridization = atomType.Hybridization;
                atom.SetProperty(CDKPropertyName.Color, atomType.GetProperty<object>(CDKPropertyName.Color));
                atom.AtomicNumber = atomType.AtomicNumber;
                atom.ExactMass = atomType.ExactMass;
            }
            catch (CDKException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new CDKException(exception.Message, exception);
            }
            return atom;
        }
    }
}
