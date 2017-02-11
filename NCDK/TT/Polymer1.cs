















// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2015-2016  Kazuya Ujihara

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace NCDK.Default
{
    [Serializable]
    public class Polymer
        : AtomContainer, IPolymer
    {
        private IDictionary<string, IMonomer> monomers;

        public Polymer()
        {
            monomers = new Dictionary<string, IMonomer>();
        }

        /// <summary>
        /// Adds the atom oAtom to a specified Monomer.
        /// </summary>
        /// <param name="oAtom">The atom to add</param>
        /// <param name="oMonomer">The monomer the atom belongs to</param>
        public void AddAtom(IAtom oAtom, IMonomer oMonomer)
        {
            if (!Contains(oAtom))
            {
                base.Add(oAtom);	// this calls notify

                if (oMonomer != null)
                { // Not sure what's better here...throw nullpointer exception?
                    oMonomer.Add(oAtom);
                    if (!monomers.ContainsKey(oMonomer.MonomerName))
                    {
                        monomers.Add(oMonomer.MonomerName, oMonomer);
                    }
                }
            }
        }

        public virtual IEnumerable<KeyValuePair<string, IMonomer>> GetMonomerMap()
        {
            return monomers.Where(n => n.Key != "");
        }

        public virtual IMonomer GetMonomer(string cName)
        {
            IMonomer ret;
            if (!monomers.TryGetValue(cName, out ret))
                ret = null;
            return ret;
        }

        public virtual IEnumerable<string> GetMonomerNames()
        {
            return monomers.Keys;
        }

        public virtual void RemoveMonomer(string name)
        {
            IMonomer monomer;
            if (monomers.TryGetValue(name, out monomer))
            {
                Remove(monomer);
                monomers.Remove(name);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Polymer(");
            sb.Append(GetHashCode()).Append(", ");
            sb.Append(base.ToString());
            sb.Append(')');
            return sb.ToString();
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            var clone = (Polymer)base.Clone(map);
            clone.monomers = new Dictionary<string, IMonomer>();
            foreach (var monomerInfo in monomers)
            {
                string name = monomerInfo.Key;
                IMonomer original = monomerInfo.Value;
                IMonomer cloned = (IMonomer)original.Clone(map);
                clone.monomers.Add(name, cloned);
            }
            return clone;
        }
    }
}
namespace NCDK.Silent
{
    [Serializable]
    public class Polymer
        : AtomContainer, IPolymer
    {
        private IDictionary<string, IMonomer> monomers;

        public Polymer()
        {
            monomers = new Dictionary<string, IMonomer>();
        }

        /// <summary>
        /// Adds the atom oAtom to a specified Monomer.
        /// </summary>
        /// <param name="oAtom">The atom to add</param>
        /// <param name="oMonomer">The monomer the atom belongs to</param>
        public void AddAtom(IAtom oAtom, IMonomer oMonomer)
        {
            if (!Contains(oAtom))
            {
                base.Add(oAtom);	// this calls notify

                if (oMonomer != null)
                { // Not sure what's better here...throw nullpointer exception?
                    oMonomer.Add(oAtom);
                    if (!monomers.ContainsKey(oMonomer.MonomerName))
                    {
                        monomers.Add(oMonomer.MonomerName, oMonomer);
                    }
                }
            }
        }

        public virtual IEnumerable<KeyValuePair<string, IMonomer>> GetMonomerMap()
        {
            return monomers.Where(n => n.Key != "");
        }

        public virtual IMonomer GetMonomer(string cName)
        {
            IMonomer ret;
            if (!monomers.TryGetValue(cName, out ret))
                ret = null;
            return ret;
        }

        public virtual IEnumerable<string> GetMonomerNames()
        {
            return monomers.Keys;
        }

        public virtual void RemoveMonomer(string name)
        {
            IMonomer monomer;
            if (monomers.TryGetValue(name, out monomer))
            {
                Remove(monomer);
                monomers.Remove(name);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Polymer(");
            sb.Append(GetHashCode()).Append(", ");
            sb.Append(base.ToString());
            sb.Append(')');
            return sb.ToString();
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            var clone = (Polymer)base.Clone(map);
            clone.monomers = new Dictionary<string, IMonomer>();
            foreach (var monomerInfo in monomers)
            {
                string name = monomerInfo.Key;
                IMonomer original = monomerInfo.Value;
                IMonomer cloned = (IMonomer)original.Clone(map);
                clone.monomers.Add(name, cloned);
            }
            return clone;
        }
    }
}
