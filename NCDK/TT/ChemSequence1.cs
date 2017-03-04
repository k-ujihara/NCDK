
















// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2015-2017  Kazuya Ujihara

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;

namespace NCDK.Default
{
    [Serializable]
    public class ChemSequence
        : ChemObject, IChemSequence, IChemObjectListener, ICloneable
    {
        private IList<IChemModel> chemModels = new List<IChemModel>();

        public ChemSequence()
            : base()
        {
        }

        public void Add(IChemModel chemModel)
        {
            chemModels.Add(chemModel);
 
            chemModel.Listeners.Add(this);
            NotifyChanged(); 
        }

        public bool Remove(IChemModel chemModel)
        {
            var ret = chemModels.Remove(chemModel);
 
            chemModel.Listeners.Add(this);
            NotifyChanged(); 
            return ret;
        }

        public IChemModel this[int index]
        {
            get { return chemModels[index]; }

            set
            {
                chemModels[index] = value;
                 NotifyChanged();             }
        }

        public int Count => chemModels.Count;
        public bool IsReadOnly => chemModels.IsReadOnly;
        public void Clear() => chemModels.Clear();
        public bool Contains(IChemModel chemModel) => chemModels.Contains(chemModel);
        public void CopyTo(IChemModel[] array, int arrayIndex) => chemModels.CopyTo(array, arrayIndex);
        public IEnumerator<IChemModel> GetEnumerator() => chemModels.GetEnumerator();
        public int IndexOf(IChemModel chemModel) => chemModels.IndexOf(chemModel);
        public void Insert(int index, IChemModel chemModel) => chemModels.Insert(index, chemModel);
        public void RemoveAt(int index) => chemModels.RemoveAt(index);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("ChemSequence(#M=");
            sb.Append(Count);
            if (Count > 0)
            {
                sb.Append(", ");
                foreach (var chemModel in chemModels)
                    sb.Append(chemModel.ToString());
            }
            sb.Append(')');
            return sb.ToString();
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            var clone_chemModels = new List<IChemModel>();
            foreach (var chemModel in chemModels)
                clone_chemModels.Add((IChemModel)chemModel.Clone(map));
            var clone = (ChemSequence)base.Clone(map);
            clone.chemModels = clone_chemModels;
            return clone;
        }

        /// <summary>
        ///  Called by objects to which this object has
        ///  registered as a listener.
        ///
        /// <param name="event">A change event pointing to the source of the change</param>
        /// </summary>
        public void OnStateChanged(ChemObjectChangeEventArgs evt)
        {
             NotifyChanged(evt);         }
    }
}
namespace NCDK.Silent
{
    [Serializable]
    public class ChemSequence
        : ChemObject, IChemSequence, IChemObjectListener, ICloneable
    {
        private IList<IChemModel> chemModels = new List<IChemModel>();

        public ChemSequence()
            : base()
        {
        }

        public void Add(IChemModel chemModel)
        {
            chemModels.Add(chemModel);
        }

        public bool Remove(IChemModel chemModel)
        {
            var ret = chemModels.Remove(chemModel);
            return ret;
        }

        public IChemModel this[int index]
        {
            get { return chemModels[index]; }

            set
            {
                chemModels[index] = value;
                            }
        }

        public int Count => chemModels.Count;
        public bool IsReadOnly => chemModels.IsReadOnly;
        public void Clear() => chemModels.Clear();
        public bool Contains(IChemModel chemModel) => chemModels.Contains(chemModel);
        public void CopyTo(IChemModel[] array, int arrayIndex) => chemModels.CopyTo(array, arrayIndex);
        public IEnumerator<IChemModel> GetEnumerator() => chemModels.GetEnumerator();
        public int IndexOf(IChemModel chemModel) => chemModels.IndexOf(chemModel);
        public void Insert(int index, IChemModel chemModel) => chemModels.Insert(index, chemModel);
        public void RemoveAt(int index) => chemModels.RemoveAt(index);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("ChemSequence(#M=");
            sb.Append(Count);
            if (Count > 0)
            {
                sb.Append(", ");
                foreach (var chemModel in chemModels)
                    sb.Append(chemModel.ToString());
            }
            sb.Append(')');
            return sb.ToString();
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            var clone_chemModels = new List<IChemModel>();
            foreach (var chemModel in chemModels)
                clone_chemModels.Add((IChemModel)chemModel.Clone(map));
            var clone = (ChemSequence)base.Clone(map);
            clone.chemModels = clone_chemModels;
            return clone;
        }

        /// <summary>
        ///  Called by objects to which this object has
        ///  registered as a listener.
        ///
        /// <param name="event">A change event pointing to the source of the change</param>
        /// </summary>
        public void OnStateChanged(ChemObjectChangeEventArgs evt)
        {
                    }
    }
}
