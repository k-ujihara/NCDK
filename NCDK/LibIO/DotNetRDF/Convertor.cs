using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VDS.RDF;

namespace NCDK.LibIO.DotNetRDF
{
    /// <summary>
    /// Helper class that converts a CDK <see cref="IChemObject"/> into RDF using a
    /// dotNetRDF model and the CDK data model ontology.
    /// </summary>
    // @cdk.module       iordf
    // @cdk.githash
    // @cdk.keyword      Resource Description Framework
    // @cdk.keyword      Jena
    // @cdk.keyword      RDF
    // @cdk.keyword      Web Ontology Language
    // @cdk.keyword      OWL
    public class Convertor
    {
        private const string URI = "http://cdk.sourceforge.net/model.owl#";

        public const string URI_MOLECULE = URI + "Molecule";
        public const string URI_ATOM = URI + "Atom";
        public const string URI_PSEUDOATOM = URI + "PseudoAtom";
        public const string URI_BOND = URI + "Bond";
        public const string URI_CHEMOBJECT = URI + "ChemObject";
        public const string URI_ELEMENT = URI + "Element";
        public const string URI_ATOMTYPE = URI + "AtomType";
        public const string URI_ISOTOPE = URI + "Isotope";

        // BondOrder
        public const string URI_SINGLEBOND = URI + "SingleBond";
        public const string URI_DOUBLEBOND = URI + "DoubleBond";
        public const string URI_TRIPLEBOND = URI + "TripleBond";
        public const string URI_QUADRUPLEBOND = URI + "QuadrupleBond";

        // Hybridization
        public const string URI_HYBRID_S = URI + "S";
        public const string URI_HYBRID_SP1 = URI + "SP1";
        public const string URI_HYBRID_SP2 = URI + "SP2";
        public const string URI_HYBRID_SP3 = URI + "SP3";
        public const string URI_HYBRID_PLANAR3 = URI + "PLANAR3";
        public const string URI_HYBRID_SP3D1 = URI + "SP3D1";
        public const string URI_HYBRID_SP3D2 = URI + "SP3D2";
        public const string URI_HYBRID_SP3D3 = URI + "SP3D3";
        public const string URI_HYBRID_SP3D4 = URI + "SP3D4";
        public const string URI_HYBRID_SP3D5 = URI + "SP3D5";

        public const string URI_HASATOM = URI + "hasAtom";
        public const string URI_HASBOND = URI + "hasBond";
        public const string URI_BINDSATOM = URI + "bindsAtom";
        public const string URI_HASORDER = URI + "hasOrder";
        public const string URI_SYMBOL = URI + "symbol";
        public const string URI_HASLABEL = URI + "hasLabel";
        public const string URI_IDENTIFIER = URI + "identifier";
        public const string URI_HASATOMICNUMBER = URI + "hasAtomicNumber";
        public const string URI_HASHYBRIDIZATION = URI + "hasHybridization";
        public const string URI_HASATOMTYPENAME = URI + "hasAtomTypeName";
        public const string URI_HASMAXBONDORDER = URI + "hasMaxBondOrder";
        public const string URI_HASFORMALCHARGE = URI + "hasFormalCharge";
        public const string URI_HASMASSNUMBER = URI + "hasMassNumber";
        public const string URI_HASEXACTMASS = URI + "hasExactMass";
        public const string URI_HASNATURALABUNDANCE = URI + "hasNaturalAbundance";
        public const string URI_HASELECTRONCOUNT = URI + "hasElectronCount";

        private readonly static Dictionary<string, Hybridization> RESOURCE_TO_HYBRID
            = new Dictionary<string, Hybridization>()
            {
                    { URI_HYBRID_S, Hybridization.S },
                    { URI_HYBRID_SP1, Hybridization.SP1 },
                    { URI_HYBRID_SP2, Hybridization.SP2 },
                    { URI_HYBRID_SP3, Hybridization.SP3 },
                    { URI_HYBRID_PLANAR3, Hybridization.Planar3 },
                    { URI_HYBRID_SP3D1, Hybridization.SP3D1 },
                    { URI_HYBRID_SP3D2, Hybridization.SP3D2 },
                    { URI_HYBRID_SP3D3, Hybridization.SP3D3 },
                    { URI_HYBRID_SP3D4, Hybridization.SP3D4 },
                    { URI_HYBRID_SP3D5, Hybridization.SP3D5 },
            };

        private readonly static Dictionary<Hybridization, string> HYBRID_TO_RESOURCE 
            = new Dictionary<Hybridization, string>()
            {
                    { Hybridization.S, URI_HYBRID_S },
                    { Hybridization.SP1, URI_HYBRID_SP1 },
                    { Hybridization.SP2, URI_HYBRID_SP2 },
                    { Hybridization.SP3, URI_HYBRID_SP3 },
                    { Hybridization.Planar3, URI_HYBRID_PLANAR3 },
                    { Hybridization.SP3D1, URI_HYBRID_SP3D1 },
                    { Hybridization.SP3D2, URI_HYBRID_SP3D2 },
                    { Hybridization.SP3D3, URI_HYBRID_SP3D3 },
                    { Hybridization.SP3D4, URI_HYBRID_SP3D4 },
                    { Hybridization.SP3D5, URI_HYBRID_SP3D5 },
            };

        IGraph g;

        static IUriNode Resource(IGraph g, string name) => g.CreateUriNode(new Uri(name));
        static IUriNode Property(IGraph g, string name) => g.CreateUriNode(new Uri(name));

        readonly IUriNode P_TYPE;

        readonly IUriNode R_MOLECULE;
        readonly IUriNode R_ATOM;
        readonly IUriNode R_PSEUDOATOM;
        readonly IUriNode R_BOND;
        readonly IUriNode R_CHEMOBJECT;
        readonly IUriNode R_ELEMENT;
        readonly IUriNode R_ATOMTYPE;
        readonly IUriNode R_ISOTOPE;

        // IBond.Order
        readonly IUriNode R_SINGLEBOND;
        readonly IUriNode R_DOUBLEBOND;
        readonly IUriNode R_TRIPLEBOND;
        readonly IUriNode R_QUADRUPLEBOND;

        // IAtomType.Hybridization
        readonly IUriNode R_HYBRID_S;
        readonly IUriNode R_HYBRID_SP1;
        readonly IUriNode R_HYBRID_SP2;
        readonly IUriNode R_HYBRID_SP3;
        readonly IUriNode R_HYBRID_PLANAR3;
        readonly IUriNode R_HYBRID_SP3D1;
        readonly IUriNode R_HYBRID_SP3D2;
        readonly IUriNode R_HYBRID_SP3D3;
        readonly IUriNode R_HYBRID_SP3D4;
        readonly IUriNode R_HYBRID_SP3D5;

        readonly IUriNode P_HASATOM;
        readonly IUriNode P_HASBOND;
        readonly IUriNode P_BINDSATOM;
        readonly IUriNode P_HASORDER;
        readonly IUriNode P_SYMBOL;
        readonly IUriNode P_HASLABEL;
        readonly IUriNode P_IDENTIFIER;
        readonly IUriNode P_HASATOMICNUMBER;
        readonly IUriNode P_HASHYBRIDIZATION;
        readonly IUriNode P_HASATOMTYPENAME;
        readonly IUriNode P_HASMAXBONDORDER;
        readonly IUriNode P_HASFORMALCHARGE;
        readonly IUriNode P_HASMASSNUMBER;
        readonly IUriNode P_HASEXACTMASS;
        readonly IUriNode P_HASNATURALABUNDANCE;
        readonly IUriNode P_HASELECTRONCOUNT;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g">RDF graph to maniturate <see cref="IAtomContainer"/>.</param>
        public Convertor(IGraph g)
        {
            this.g = g;

            g.NamespaceMap.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            P_TYPE = g.CreateUriNode("rdf:type");

            R_MOLECULE = Resource(g, URI_MOLECULE);
            R_ATOM = Resource(g, URI_ATOM);
            R_PSEUDOATOM = Resource(g, URI_PSEUDOATOM);
            R_BOND = Resource(g, URI_BOND);
            R_CHEMOBJECT = Resource(g, URI_CHEMOBJECT);
            R_ELEMENT = Resource(g, URI_ELEMENT);
            R_ATOMTYPE = Resource(g, URI_ATOMTYPE);
            R_ISOTOPE = Resource(g, URI_ISOTOPE);

            // BondOrder
            R_SINGLEBOND = Resource(g, URI_SINGLEBOND);
            R_DOUBLEBOND = Resource(g, URI_DOUBLEBOND);
            R_TRIPLEBOND = Resource(g, URI_TRIPLEBOND);
            R_QUADRUPLEBOND = Resource(g, URI_QUADRUPLEBOND);

            // Hybridization
            R_HYBRID_S = Resource(g, URI_HYBRID_S);
            R_HYBRID_SP1 = Resource(g, URI_HYBRID_SP1);
            R_HYBRID_SP2 = Resource(g, URI_HYBRID_SP2);
            R_HYBRID_SP3 = Resource(g, URI_HYBRID_SP3);
            R_HYBRID_PLANAR3 = Resource(g, URI_HYBRID_PLANAR3);
            R_HYBRID_SP3D1 = Resource(g, URI_HYBRID_SP3D1);
            R_HYBRID_SP3D2 = Resource(g, URI_HYBRID_SP3D2);
            R_HYBRID_SP3D3 = Resource(g, URI_HYBRID_SP3D3);
            R_HYBRID_SP3D4 = Resource(g, URI_HYBRID_SP3D4);
            R_HYBRID_SP3D5 = Resource(g, URI_HYBRID_SP3D5);

            P_HASATOM = Property(g, URI_HASATOM);
            P_HASBOND = Property(g, URI_HASBOND);
            P_BINDSATOM = Property(g, URI_BINDSATOM);
            P_HASORDER = Property(g, URI_HASORDER);
            P_SYMBOL = Property(g, URI_SYMBOL);
            P_HASLABEL = Property(g, URI_HASLABEL);
            P_IDENTIFIER = Property(g, URI_IDENTIFIER);
            P_HASATOMICNUMBER = Property(g, URI_HASATOMICNUMBER);
            P_HASHYBRIDIZATION = Property(g, URI_HASHYBRIDIZATION);
            P_HASATOMTYPENAME = Property(g, URI_HASATOMTYPENAME);
            P_HASMAXBONDORDER = Property(g, URI_HASMAXBONDORDER);
            P_HASFORMALCHARGE = Property(g, URI_HASFORMALCHARGE);
            P_HASMASSNUMBER = Property(g, URI_HASMASSNUMBER);
            P_HASEXACTMASS = Property(g, URI_HASEXACTMASS);
            P_HASNATURALABUNDANCE = Property(g, URI_HASNATURALABUNDANCE);
            P_HASELECTRONCOUNT = Property(g, URI_HASELECTRONCOUNT);
        }

        /// <summary>
        /// Converts a <see cref="IAtomContainer"/> into a <see cref="IGraph"/> representation using the CDK OWL.
        /// </summary>
        /// <param name="molecule"><see cref="IAtomContainer"/> to serialize into a RDF graph.</param>
        /// <returns>the RDF graph representing the <see cref="IAtomContainer"/>.</returns>
        public IGraph Molecule2Model(IAtomContainer molecule)
        {
            var subject = g.CreateUriNode(CreateIdentifier(molecule));
            g.Assert(new Triple(subject, P_TYPE, R_MOLECULE));
            var cdkToRDFAtomMap = new Dictionary<IAtom, INode>();
            foreach (var atom in molecule.Atoms)
            {
                var rdfAtom = g.CreateUriNode(CreateIdentifier(atom));
                cdkToRDFAtomMap[atom] = rdfAtom;
                g.Assert(new Triple(subject, P_HASATOM, rdfAtom));
                if (atom is IPseudoAtom)
                {
                    g.Assert(rdfAtom, P_TYPE, R_PSEUDOATOM);
                    SerializePseudoAtomFields(rdfAtom, (IPseudoAtom)atom);
                }
                else
                {
                    g.Assert(new Triple(rdfAtom, P_TYPE, R_ATOM));
                    SerializeAtomFields(rdfAtom, atom);
                }
            }
            foreach (var bond in molecule.Bonds)
            {
                var rdfBond = g.CreateUriNode(CreateIdentifier(bond));
                g.Assert(new Triple(rdfBond, P_TYPE, R_BOND));
                foreach (var atom in bond.Atoms)
                {
                    g.Assert(new Triple(rdfBond, P_BINDSATOM, cdkToRDFAtomMap[atom]));
                }
                if (bond.Order != BondOrder.Unset)
                {
                    g.Assert(new Triple(rdfBond, P_HASORDER, Order2Resource(bond.Order)));
                }
                g.Assert(new Triple(subject, P_HASBOND, rdfBond));
                SerializeElectronContainerFields(rdfBond, bond);
            }
            return g;
        }

        private void SerializePseudoAtomFields(INode rdfAtom, IPseudoAtom atom)
        {
            SerializeAtomFields(rdfAtom, atom);
            if (atom.Label != null)
                g.Assert(new Triple(rdfAtom, P_HASLABEL, g.CreateLiteralNode(atom.Label)));
        }

        private void SerializeAtomFields(INode rdfAtom, IAtom atom)
        {
            SerializeAtomTypeFields(rdfAtom, atom);
            g.Assert(new Triple(rdfAtom, P_TYPE, R_ATOM));
            if (atom.Symbol != null)
                g.Assert(rdfAtom, P_SYMBOL, g.CreateLiteralNode(atom.Symbol));
        }

        private void SerializeElectronContainerFields(INode rdfBond, IElectronContainer bond)
        {
            SerializeChemObjectFields(rdfBond, bond);
            if (bond.ElectronCount != null)
                g.Assert(new Triple(rdfBond, P_HASELECTRONCOUNT, g.CreateLiteralNode(bond.ElectronCount.ToString())));
        }

        private void SerializeChemObjectFields(INode rdfObject, IChemObject obj)
        {
            if (obj.Id != null) g.Assert(new Triple(rdfObject, P_IDENTIFIER, g.CreateLiteralNode(obj.Id)));
        }

        private void DeserializeChemObjectFields(INode rdfObject, IChemObject obj)
        {
            var identifier = g.GetTriplesWithSubjectPredicate(rdfObject, P_IDENTIFIER).FirstOrDefault();
            if (identifier != null) obj.Id = identifier.Object.ToString();
        }

        private void SerializeElementFields(INode rdfObject, IElement element)
        {
            SerializeChemObjectFields(rdfObject, element);
            if (element.Symbol != null) g.Assert(new Triple(rdfObject, P_SYMBOL, g.CreateLiteralNode(element.Symbol)));
            if (element.AtomicNumber != null)
                g.Assert(new Triple(rdfObject, P_HASATOMICNUMBER, g.CreateLiteralNode(element.AtomicNumber.ToString())));
        }

        private void DeserializeElementFields(INode rdfObject, IAtomType element)
        {
            DeserializeChemObjectFields(rdfObject, element);
            var symbol = g.GetTriplesWithSubjectPredicate(rdfObject, P_SYMBOL).FirstOrDefault();
            if (symbol != null) element.Symbol = symbol.Object.ToString();
            var atomicNumber = g.GetTriplesWithSubjectPredicate(rdfObject, P_HASATOMICNUMBER).FirstOrDefault();
            if (atomicNumber != null) element.AtomicNumber = int.Parse(atomicNumber.Object.ToString());
        }

        private void SerializeAtomTypeFields(INode rdfObject, IAtomType type)
        {
            SerializeIsotopeFields(rdfObject, type);
            if (type.Hybridization != Hybridization.Unset)
            {
                Hybridization hybrid = type.Hybridization;
                if (HYBRID_TO_RESOURCE.ContainsKey(hybrid))
                    g.Assert(new Triple(rdfObject, P_HASHYBRIDIZATION, g.CreateLiteralNode(HYBRID_TO_RESOURCE[hybrid])));
            }
            if (type.AtomTypeName != null)
            {
               g.Assert(new Triple(rdfObject, P_HASATOMTYPENAME, g.CreateLiteralNode(type.AtomTypeName)));
            }
            if (type.FormalCharge != null)
            {
                g.Assert(new Triple(rdfObject, P_HASFORMALCHARGE, g.CreateLiteralNode(type.FormalCharge.ToString())));
            }
            if (type.MaxBondOrder != BondOrder.Unset)
            {
                g.Assert(new Triple(rdfObject, P_HASMAXBONDORDER, Order2Resource(type.MaxBondOrder)));
            }
        }

        private void SerializeIsotopeFields(INode rdfObject, IIsotope isotope)
        {
            SerializeElementFields(rdfObject, isotope);
            if (isotope.MassNumber != null)
            {
                g.Assert(new Triple(rdfObject, P_HASMASSNUMBER, g.CreateLiteralNode(isotope.MassNumber.ToString())));
            }
            if (isotope.ExactMass != null)
            {
                g.Assert(new Triple(rdfObject, P_HASEXACTMASS, g.CreateLiteralNode(isotope.ExactMass.ToString())));
            }
            if (isotope.NaturalAbundance != null)
            {
                g.Assert(new Triple(rdfObject, P_HASNATURALABUNDANCE, g.CreateLiteralNode(isotope.NaturalAbundance.ToString())));
            }
        }

        private void DeserializeAtomTypeFields(INode rdfObject, IAtomType element)
        {
            DeserializeIsotopeFields(rdfObject, element);
            var hybrid = g.GetTriplesWithSubjectPredicate(rdfObject, P_HASHYBRIDIZATION).FirstOrDefault();
            if (hybrid != null)
            {
                var rdfHybrid = hybrid.Object.ToString();
                if (RESOURCE_TO_HYBRID.ContainsKey(rdfHybrid))
                {
                    element.Hybridization = RESOURCE_TO_HYBRID[rdfHybrid];
                }
            }
            var name = g.GetTriplesWithSubjectPredicate(rdfObject, P_HASATOMTYPENAME).FirstOrDefault();
            if (name != null)
            {
                element.AtomTypeName = name.Object.ToString();
            }
            var order = g.GetTriplesWithSubjectPredicate(rdfObject, P_HASMAXBONDORDER).FirstOrDefault();
            if (order != null)
            {
                var maxOrder = order.Object;
                element.MaxBondOrder = Resource2Order(maxOrder);
            }
            var formalCharge = g.GetTriplesWithSubjectPredicate(rdfObject, P_HASFORMALCHARGE).FirstOrDefault();
            if (formalCharge != null)
                element.FormalCharge = int.Parse(formalCharge.Object.ToString());
        }


        private void DeserializeIsotopeFields(INode rdfObject, IAtomType isotope)
        {
            DeserializeElementFields(rdfObject, isotope);
            var massNumber = g.GetTriplesWithSubjectPredicate(rdfObject, P_HASMASSNUMBER).FirstOrDefault();
            if (massNumber != null) isotope.MassNumber = int.Parse(massNumber.Object.ToString());
            var exactMass = g.GetTriplesWithSubjectPredicate(rdfObject, P_HASEXACTMASS).FirstOrDefault();
            if (exactMass != null) isotope.ExactMass = double.Parse(exactMass.Object.ToString());
            var naturalAbundance = g.GetTriplesWithSubjectPredicate(rdfObject, P_HASNATURALABUNDANCE).FirstOrDefault();
            if (naturalAbundance != null) isotope.NaturalAbundance = double.Parse(naturalAbundance.Object.ToString());
        }

        /// <summary>
        /// Converts a <see cref="Resource"/> object into the matching <see cref="BondOrder"/>.
        /// </summary>
        /// <param name="rdfOrder">Resource for which the matching <see cref="BondOrder"/> should be given.</param>
        /// <returns>the matching <see cref="BondOrder"/>.</returns>
        public BondOrder Resource2Order(INode rdfOrder)
        {
            switch (rdfOrder?.ToString())
            {
                case URI_SINGLEBOND:
                    return BondOrder.Single;
                case URI_DOUBLEBOND:
                    return BondOrder.Double;
                case URI_TRIPLEBOND:
                    return BondOrder.Triple;
                case URI_QUADRUPLEBOND:
                    return BondOrder.Quadruple;
                default:
                    return BondOrder.Unset;
            }
        }
        
        /// <summary>
        /// Create the <see cref="INode"/> matching the given <see cref="BondOrder"/>.
        /// </summary>
        /// <param name="order">bond order to return the matching <see cref="INode"/> for.</param>
        /// <returns>the matching <see cref="INode"/>.</returns>
        public INode Order2Resource(BondOrder order)
        {
            switch (order)
            {
                case BondOrder.Single:
                    return R_SINGLEBOND;
                case BondOrder.Double:
                    return R_DOUBLEBOND;
                case BondOrder.Triple:
                    return R_TRIPLEBOND;
                case BondOrder.Quadruple:
                    return R_QUADRUPLEBOND;
                default:
                    return null;
            }
        }

        private Uri CreateIdentifier(IChemObject obj)
        {
            StringBuilder result = new StringBuilder();
            result.Append("http://example.com/");
            result.Append(g.GetHashCode()).Append('/');
            result.Append(obj.GetType().Name).Append('/');
            result.Append(obj.GetHashCode());
            return new Uri(result.ToString());
        }

        private void DeserializeElectronContainerFields(INode rdfObject, IElectronContainer bond)
        {
            DeserializeChemObjectFields(rdfObject, bond);
            var count = g.GetTriplesWithSubjectPredicate(rdfObject, P_HASELECTRONCOUNT).Select(n => n.Object).FirstOrDefault();
            if (count != null) bond.ElectronCount = int.Parse(count.ToString());
        }

        /// <summary>
        /// Converts a <see cref="IGraph"/> into an <see cref="IAtomContainer"/> using the given <see cref="IChemObjectBuilder"/>.
        /// </summary>
        /// <param name="builder"><see cref="IChemObjectBuilder"/> used to create new <see cref="IChemObject"/>s.</param>
        /// <returns>a <see cref="IAtomContainer"/> deserialized from the RDF graph.</returns>
        public IAtomContainer Model2Molecule(IChemObjectBuilder builder)
        {
            var mols = g.GetTriplesWithPredicateObject(P_TYPE, R_MOLECULE);
            IAtomContainer mol = null;
            foreach (var rdfMol in mols.Select(n => n.Subject))
            {
                mol = builder.NewAtomContainer();
                var rdfToCDKAtomMap = new Dictionary<INode, IAtom>();
                var atoms = g.GetTriplesWithSubjectPredicate(rdfMol, P_HASATOM).Select(n => n.Object);
                foreach (var rdfAtom in atoms)
                {
                    IAtom atom = null;
                    if (g.GetTriplesWithSubjectPredicate(rdfAtom, P_TYPE).Where(n => n.Object.Equals(R_PSEUDOATOM)).Any())
                    {
                        atom = builder.NewPseudoAtom();
                        atom.StereoParity = 0;
                        var label = g.GetTriplesWithSubjectPredicate(rdfAtom, P_HASLABEL).Select(n => n.Object).FirstOrDefault();
                        if (label != null)
                            ((IPseudoAtom)atom).Label = label.ToString();
                    }
                    else
                    {
                        atom = builder.NewAtom();
                    }
                    var symbol = g.GetTriplesWithSubjectPredicate(rdfAtom, P_SYMBOL).Select(n => n.Object).FirstOrDefault();
                    if (symbol != null)
                        atom.Symbol = symbol.ToString();
                    rdfToCDKAtomMap[rdfAtom] = atom;
                    DeserializeAtomTypeFields(rdfAtom, atom);
                    mol.Atoms.Add(atom);
                }
                var bonds = g.GetTriplesWithSubjectPredicate(rdfMol, P_HASBOND).Select(n => n.Object);
                foreach (var rdfBond in bonds)
                {
                    var bondAtoms = g.GetTriplesWithSubjectPredicate(rdfBond, P_BINDSATOM).Select(n => n.Object);
                    var atomList = new List<IAtom>();
                    foreach (var rdfAtom in bondAtoms)
                    {
                        IAtom atom = rdfToCDKAtomMap[rdfAtom];
                        atomList.Add(atom);
                    }
                    IBond bond = builder.NewBond(atomList);
                    var order = g.GetTriplesWithSubjectPredicate(rdfBond, P_HASORDER).Select(n => n.Object).FirstOrDefault();
                    bond.Order = Resource2Order(order);
                    mol.Bonds.Add(bond);
                    DeserializeElectronContainerFields(rdfBond, bond);
                }
            }
            return mol;
        }
    }
}
