
















// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2015-2017  Kazuya Ujihara

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NCDK.Default
{
    [Serializable]
    public class ReactionSet
        : ChemObject, IReactionSet, IChemObjectListener, ICloneable
    {
        private IList<IReaction> reactions = new List<IReaction>();

        public ReactionSet()
        {
        }

        public virtual IReaction this[int index]
        {
            get { return reactions[index]; }

            set
            {
                reactions[index] = value;
                value.Listeners.Add(this);
            }
        }

        public int Count => reactions.Count;
        public bool IsReadOnly => reactions.IsReadOnly;
        public void Add(IReaction reaction)
        {
            reactions.Add(reaction);
             NotifyChanged();         }

        public void Clear()
        {
            reactions.Clear();
             NotifyChanged();         }

        public bool Contains(IReaction reaction) => reactions.Contains(reaction);
        public void CopyTo(IReaction[] array, int arrayIndex)
        {
            reactions.CopyTo(array, arrayIndex);
             NotifyChanged();         }

        public IEnumerator<IReaction> GetEnumerator() => reactions.GetEnumerator();
        public int IndexOf(IReaction reaction) => reactions.IndexOf(reaction);

        public void Insert(int index, IReaction reaction)
        {
            reactions.Insert(index, reaction);
             NotifyChanged();         }

        public bool Remove(IReaction reaction)
        {
            bool ret = false;
            while (reactions.Contains(reaction))
            {
                reactions.Remove(reaction);
                ret = true;
            }
             NotifyChanged();             return ret;
        }

        public void RemoveAt(int index)
        {
            reactions.RemoveAt(index);
             NotifyChanged();         }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool IsEmpty => reactions.Count == 0;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("ReactionSet(");
            sb.Append(GetHashCode());
            sb.Append(", R=").Append(Count).Append(", ");
            foreach (var reaction in reactions)
                sb.Append(reaction.ToString());
            sb.Append(')');
            return sb.ToString();
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            var clonedReactions = new List<IReaction>();
            foreach (var reaction in reactions)
                clonedReactions.Add((IReaction)reaction.Clone(map));
            var clone = (ReactionSet)base.Clone(map);
            clone.reactions = clonedReactions;
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
    public class ReactionSet
        : ChemObject, IReactionSet, IChemObjectListener, ICloneable
    {
        private IList<IReaction> reactions = new List<IReaction>();

        public ReactionSet()
        {
        }

        public virtual IReaction this[int index]
        {
            get { return reactions[index]; }

            set
            {
                reactions[index] = value;
                value.Listeners.Add(this);
            }
        }

        public int Count => reactions.Count;
        public bool IsReadOnly => reactions.IsReadOnly;
        public void Add(IReaction reaction)
        {
            reactions.Add(reaction);
                    }

        public void Clear()
        {
            reactions.Clear();
                    }

        public bool Contains(IReaction reaction) => reactions.Contains(reaction);
        public void CopyTo(IReaction[] array, int arrayIndex)
        {
            reactions.CopyTo(array, arrayIndex);
                    }

        public IEnumerator<IReaction> GetEnumerator() => reactions.GetEnumerator();
        public int IndexOf(IReaction reaction) => reactions.IndexOf(reaction);

        public void Insert(int index, IReaction reaction)
        {
            reactions.Insert(index, reaction);
                    }

        public bool Remove(IReaction reaction)
        {
            bool ret = false;
            while (reactions.Contains(reaction))
            {
                reactions.Remove(reaction);
                ret = true;
            }
                        return ret;
        }

        public void RemoveAt(int index)
        {
            reactions.RemoveAt(index);
                    }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool IsEmpty => reactions.Count == 0;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("ReactionSet(");
            sb.Append(GetHashCode());
            sb.Append(", R=").Append(Count).Append(", ");
            foreach (var reaction in reactions)
                sb.Append(reaction.ToString());
            sb.Append(')');
            return sb.ToString();
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            var clonedReactions = new List<IReaction>();
            foreach (var reaction in reactions)
                clonedReactions.Add((IReaction)reaction.Clone(map));
            var clone = (ReactionSet)base.Clone(map);
            clone.reactions = clonedReactions;
            return clone;
        }

        public void OnStateChanged(ChemObjectChangeEventArgs evt)
        {
                    }
    }
}
