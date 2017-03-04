
















// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2015-2017  Kazuya Ujihara

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NCDK.Default
{
    [Serializable]
    public class ChemFile
        : ChemObject, ICloneable, IChemFile, IChemObjectListener
    {
        protected IList<IChemSequence> chemSequences = new List<IChemSequence>();

        public ChemFile()
            : base()
        { }

        public IChemSequence this[int number]
        {
            get { return chemSequences[number]; }
            set { chemSequences[number] = value; }
        }

        public int Count => chemSequences.Count;

        public bool IsReadOnly => chemSequences.IsReadOnly;
        
        public void Add(IChemSequence chemSequence)
        {
 
            chemSequence.Listeners.Add(this);
            chemSequences.Add(chemSequence);
 
            NotifyChanged(); 
        }

        public void Clear()
        {
            foreach (var item in chemSequences)
                item.Listeners.Remove(this);
            chemSequences.Clear();
             NotifyChanged();         }

        public bool Contains(IChemSequence chemSequence)
        {
            return chemSequences.Contains(chemSequence);
        }

        public void CopyTo(IChemSequence[] array, int arrayIndex)
        {
            chemSequences.CopyTo(array, arrayIndex);
        }

        public IEnumerator<IChemSequence> GetEnumerator()
        {
            return chemSequences.GetEnumerator();
        }

        public int IndexOf(IChemSequence chemSequence)
        {
            return chemSequences.IndexOf(chemSequence);
        }

        public void Insert(int index, IChemSequence chemSequence)
        {
 
            chemSequence.Listeners.Add(this);
            chemSequences.Insert(index, chemSequence);
 
            NotifyChanged(); 
        }

        public bool Remove(IChemSequence chemSequence)
        {
            bool ret = chemSequences.Remove(chemSequence);
 
            chemSequence.Listeners.Remove(this);
            NotifyChanged(); 
            return ret;
        }

        public void RemoveAt(int index)
        {
            chemSequences[index].Listeners.Remove(this);
            chemSequences.RemoveAt(index);
 
            NotifyChanged(); 
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("ChemFile(#S=");
            sb.Append(chemSequences.Count);
            if (chemSequences.Count > 0)
            {
                foreach (var chemSequence in chemSequences)
                {
                    sb.Append(", ");
                    sb.Append(chemSequence.ToString());
                }
            }
            sb.Append(')');
            return sb.ToString(); 
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            var clone = (ChemFile)base.Clone(map);
            clone.chemSequences = new List<IChemSequence>();
            foreach (var chemSequence in chemSequences)
                clone.chemSequences.Add((IChemSequence)chemSequence.Clone());
            return clone;
        }

        public void OnStateChanged(ChemObjectChangeEventArgs evt)
        {
             NotifyChanged(evt);         }
    }
}
namespace NCDK.Silent
{
    [Serializable]
    public class ChemFile
        : ChemObject, ICloneable, IChemFile, IChemObjectListener
    {
        protected IList<IChemSequence> chemSequences = new List<IChemSequence>();

        public ChemFile()
            : base()
        { }

        public IChemSequence this[int number]
        {
            get { return chemSequences[number]; }
            set { chemSequences[number] = value; }
        }

        public int Count => chemSequences.Count;

        public bool IsReadOnly => chemSequences.IsReadOnly;
        
        public void Add(IChemSequence chemSequence)
        {
            chemSequences.Add(chemSequence);
        }

        public void Clear()
        {
            chemSequences.Clear();
                    }

        public bool Contains(IChemSequence chemSequence)
        {
            return chemSequences.Contains(chemSequence);
        }

        public void CopyTo(IChemSequence[] array, int arrayIndex)
        {
            chemSequences.CopyTo(array, arrayIndex);
        }

        public IEnumerator<IChemSequence> GetEnumerator()
        {
            return chemSequences.GetEnumerator();
        }

        public int IndexOf(IChemSequence chemSequence)
        {
            return chemSequences.IndexOf(chemSequence);
        }

        public void Insert(int index, IChemSequence chemSequence)
        {
            chemSequences.Insert(index, chemSequence);
        }

        public bool Remove(IChemSequence chemSequence)
        {
            bool ret = chemSequences.Remove(chemSequence);
            return ret;
        }

        public void RemoveAt(int index)
        {
            chemSequences.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("ChemFile(#S=");
            sb.Append(chemSequences.Count);
            if (chemSequences.Count > 0)
            {
                foreach (var chemSequence in chemSequences)
                {
                    sb.Append(", ");
                    sb.Append(chemSequence.ToString());
                }
            }
            sb.Append(')');
            return sb.ToString(); 
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            var clone = (ChemFile)base.Clone(map);
            clone.chemSequences = new List<IChemSequence>();
            foreach (var chemSequence in chemSequences)
                clone.chemSequences.Add((IChemSequence)chemSequence.Clone());
            return clone;
        }

        public void OnStateChanged(ChemObjectChangeEventArgs evt)
        {
                    }
    }
}
