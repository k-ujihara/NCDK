using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace NCDK.Config.AtomType
{
    public class AtomTypeReader
    {
        private TextReader input;

        public AtomTypeReader(Stream input)
        {
            this.input = new StreamReader(input);
        }

        public AtomTypeReader(TextReader input)
        {
            this.input = input;
        }

        public IList<IAtomType> ReadAtomTypes(IChemObjectBuilder builder)
        {
            var ret = new List<IAtomType>();

            var settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.None;
            settings.ValidationFlags = System.Xml.Schema.XmlSchemaValidationFlags.None;
            XmlReader reader = XmlReader.Create(input, settings);

            var doc = XElement.Load(reader);
            var nAtomType = doc.Name.Namespace + "atomType";
            var nAtom = doc.Name.Namespace + "atom";
            var nLabel = doc.Name.Namespace + "label";
            var nScalar = doc.Name.Namespace + "scalar";
            foreach (var elementAtomType in doc.Elements(nAtomType))
            {
                var atomType = builder.CreateAtomType("R");
                atomType.AtomTypeName = elementAtomType.Attribute("id")?.Value;
                foreach (var elm in elementAtomType.Descendants())
                {
                    // lazzy compare
                    switch (elm.Name.LocalName)
                    {
                        case "atom":
                            atomType.Symbol = elm.Attribute("elementType")?.Value;
                            var sFormalCharge = elm.Attribute("formalCharge")?.Value;
                            if (sFormalCharge != null)
                                atomType.FormalCharge = int.Parse(sFormalCharge);
                            break;
                        case "label":
                            var aValue = elm.Attribute("value");
                            if (aValue != null)
                            {
                                if (atomType.AtomTypeName != null)
                                    atomType.Id = atomType.AtomTypeName;
                                atomType.AtomTypeName = aValue.Value;
                            }
                            break;
                        case "scalar":
                            var dictRef = elm.Attribute("dictRef")?.Value;
                            var value = string.IsNullOrWhiteSpace(elm.Value) ? null : elm.Value;
                            if (value != null)
                            {
                                switch (dictRef)
                                {
                                    case "cdk:bondOrderSum":
                                        atomType.BondOrderSum = double.Parse(value);
                                        break;
                                    case "cdk:maxBondOrder":
                                        atomType.MaxBondOrder = (BondOrder)(int)double.Parse(value);
                                        break;
                                    case "cdk:formalNeighbourCount":
                                        atomType.FormalNeighbourCount = (int)double.Parse(value);
                                        break;
                                    case "cdk:valency":
                                        atomType.Valency = (int)double.Parse(value);
                                        break;
                                    case "cdk:formalCharge":
                                        atomType.FormalCharge = (int)double.Parse(value);
                                        break;
                                    case "cdk:hybridization":
                                        atomType.Hybridization = Hybridization.GetInstance(value);
                                        break;
                                    case "cdk:DA":
                                        switch (value)
                                        {
                                            case "A":
                                                atomType.IsHydrogenBondAcceptor |= true;
                                                break;
                                            case "D":
                                                atomType.IsHydrogenBondDonor |= true;
                                                break;
                                            default:
                                                Trace.TraceWarning($"Unrecognized H-bond donor/acceptor pattern in config file: {value}");
                                                break;
                                        }
                                        break;
                                    case "cdk:sphericalMatcher":
                                        atomType.SetProperty(CDKPropertyName.SphericalMatcher, value);
                                        break;
                                    case "cdk:ringSize":
                                        atomType.SetProperty(CDKPropertyName.PartOfRingOfSize, int.Parse(value));
                                        break;
                                    case "cdk:ringConstant":
                                        atomType.SetProperty(CDKPropertyName.ChemicalGroupConstant, int.Parse(value));
                                        break;
                                    case "cdk:aromaticAtom":
                                        atomType.IsAromatic |= true;
                                        break;
                                    case "emboss:vdwrad":
                                        break;
                                    case "cdk:piBondCount":
                                        atomType.SetProperty(CDKPropertyName.PiBondCount, int.Parse(value));
                                        break;
                                    case "cdk:lonePairCount":
                                        atomType.SetProperty(CDKPropertyName.LonePairCount, int.Parse(value));
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                ret.Add(atomType);
            }
            return ret;
        }
    }
}
