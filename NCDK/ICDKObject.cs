using System;
using System.Collections.Generic;

namespace NCDK
{
    public interface ICDKObject 
        : ICloneable
    {
        IChemObjectBuilder Builder { get; }

        ICDKObject Clone(CDKObjectMap map);
    }

    public class CDKObjectMap
    {
        public CDKObjectMap()
        { }

        IDictionary<IAtom, IAtom> atomMap;
        public IDictionary<IAtom, IAtom> AtomMap
        {
            get
            {
                if (atomMap == null)
                    atomMap = new Dictionary<IAtom, IAtom>();
                return atomMap;
            }
        }

        IDictionary<IBond, IBond> bondMap;
        public IDictionary<IBond, IBond> BondMap
        {
            get
            {
                if (bondMap == null)
                    bondMap = new Dictionary<IBond, IBond>();
                return bondMap;
            }
        }
    }
}
