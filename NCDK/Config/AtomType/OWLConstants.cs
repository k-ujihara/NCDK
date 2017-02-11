/* 
 * Copyright (C) 2016-2017 Kazuya Ujihara <chemformatter.com>
 */ 

using System.Xml.Linq;

namespace NCDK.Config.AtomType
{
    internal static class OWLConstants
    {
        public static readonly XNamespace NS_AtomType = "http://cdk.sf.net/ontologies/atomtypes#";
        public static readonly XName XName_AtomType = NS_AtomType + "AtomType";
        public static readonly XName XName_hasElement = NS_AtomType + "hasElement";
        public static readonly XName XName_formalCharge = NS_AtomType + "formalCharge";
        public static readonly XName XName_formalNeighbourCount = NS_AtomType + "formalNeighbourCount";
        public static readonly XName XName_lonePairCount = NS_AtomType + "lonePairCount";
        public static readonly XName XName_singleElectronCount = NS_AtomType + "singleElectronCount";
        public static readonly XName XName_piBondCount = NS_AtomType + "piBondCount";
        public static readonly XName XName_hybridization = NS_AtomType + "hybridization";
        public static readonly XName XName_formalBondType = NS_AtomType + "formalBondType";

        public static readonly XNamespace NS_rdf = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
        public static readonly XName XName_ID = NS_rdf + "ID";
        public static readonly XName XName_rdf_about = NS_rdf + "about";
        public static readonly XName XName_rdf_resource = NS_rdf + "resource";

        public static readonly XNamespace NS_AtomTypeMapping = "http://cdk.sf.net/ontologies/atomtypemappings#";

        public static readonly XName XName_AtomTypeMapping_mapsToType = NS_AtomTypeMapping + "mapsToType";
        public static readonly XName XName_AtomTypeMapping_equivalentAsType = NS_AtomTypeMapping + "equivalentAsType";

        public static readonly XNamespace NS_OWL = "http://www.w3.org/2002/07/owl#";

        public static readonly XName XName_OWL_Thing = NS_OWL + "Thing";
    }
}
