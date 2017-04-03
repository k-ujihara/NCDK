

// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2016-2017  Kazuya Ujihara

using System;
using System.Text;

namespace NCDK.Default
{
    [Serializable]
    public class ChemModel
        : ChemObject, IChemModel, IChemObjectListener, ICloneable
    {
        private IAtomContainerSet<IAtomContainer> setOfMolecules = null;
        private IReactionSet setOfReactions = null;
        private IRingSet ringSet = null;
        private ICrystal crystal = null;

        public ChemModel()
            : base()
        {
        }

        public virtual IAtomContainerSet<IAtomContainer> MoleculeSet
        {
            get { return setOfMolecules; }

            set
            {
 
                if (setOfMolecules != null)
                    setOfMolecules.Listeners.Remove(this);
                setOfMolecules = value;
 
                if (setOfMolecules != null)
                    setOfMolecules.Listeners.Add(this);
                NotifyChanged(); 
            }
        }

        public IRingSet RingSet
        {
            get { return ringSet; }

            set
            {
 
                if (ringSet != null)
                    ringSet.Listeners.Remove(this);
                ringSet = value;
 
                if (ringSet != null)
                    ringSet.Listeners.Add(this);
                NotifyChanged();
            }
        }

        public ICrystal Crystal
        {
            get { return crystal; }

            set
            {
 
                if (crystal != null)
                    crystal.Listeners.Remove(this);
                crystal = value;
 
                if (crystal != null)
                    crystal.Listeners.Add(this);
                NotifyChanged();
            }
        }

        public IReactionSet ReactionSet
        {
            get { return setOfReactions; }

            set
            {
 
                if (setOfReactions != null)
                    setOfReactions.Listeners.Remove(this);
                setOfReactions = value;
 
                if (setOfReactions != null)
                    setOfReactions.Listeners.Add(this);
                NotifyChanged();
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("ChemModel(");
            sb.Append(GetHashCode());
            if (MoleculeSet != null)
            {
                sb.Append(", ");
                sb.Append(MoleculeSet.ToString());
            }
            if (Crystal != null)
            {
                sb.Append(", ");
                sb.Append(Crystal.ToString());
            }
            if (ReactionSet != null)
            {
                sb.Append(", ");
                sb.Append(ReactionSet.ToString());
            }
            sb.Append(')');
            return sb.ToString();
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            ChemModel clone = (ChemModel)base.Clone(map);
            clone.setOfMolecules = (IAtomContainerSet<IAtomContainer>)setOfMolecules?.Clone(map);
            clone.setOfReactions = (IReactionSet)setOfReactions?.Clone(map);
            clone.ringSet = (IRingSet)ringSet?.Clone(map);
            clone.crystal = (ICrystal)crystal?.Clone(map);
            return clone;
        }

        public void OnStateChanged(ChemObjectChangeEventArgs evt)
        {
             NotifyChanged(evt);         }

        public bool IsEmpty()
            => (setOfMolecules == null || setOfMolecules.IsEmpty())
            && (setOfReactions == null || setOfReactions.IsEmpty())
            && (ringSet == null || ringSet.IsEmpty())
            && (crystal == null || crystal.IsEmpty());
    }
}
namespace NCDK.Silent
{
    [Serializable]
    public class ChemModel
        : ChemObject, IChemModel, IChemObjectListener, ICloneable
    {
        private IAtomContainerSet<IAtomContainer> setOfMolecules = null;
        private IReactionSet setOfReactions = null;
        private IRingSet ringSet = null;
        private ICrystal crystal = null;

        public ChemModel()
            : base()
        {
        }

        public virtual IAtomContainerSet<IAtomContainer> MoleculeSet
        {
            get { return setOfMolecules; }

            set
            {
                setOfMolecules = value;
            }
        }

        public IRingSet RingSet
        {
            get { return ringSet; }

            set
            {
                ringSet = value;
            }
        }

        public ICrystal Crystal
        {
            get { return crystal; }

            set
            {
                crystal = value;
            }
        }

        public IReactionSet ReactionSet
        {
            get { return setOfReactions; }

            set
            {
                setOfReactions = value;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("ChemModel(");
            sb.Append(GetHashCode());
            if (MoleculeSet != null)
            {
                sb.Append(", ");
                sb.Append(MoleculeSet.ToString());
            }
            if (Crystal != null)
            {
                sb.Append(", ");
                sb.Append(Crystal.ToString());
            }
            if (ReactionSet != null)
            {
                sb.Append(", ");
                sb.Append(ReactionSet.ToString());
            }
            sb.Append(')');
            return sb.ToString();
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            ChemModel clone = (ChemModel)base.Clone(map);
            clone.setOfMolecules = (IAtomContainerSet<IAtomContainer>)setOfMolecules?.Clone(map);
            clone.setOfReactions = (IReactionSet)setOfReactions?.Clone(map);
            clone.ringSet = (IRingSet)ringSet?.Clone(map);
            clone.crystal = (ICrystal)crystal?.Clone(map);
            return clone;
        }

        public void OnStateChanged(ChemObjectChangeEventArgs evt)
        {
                    }

        public bool IsEmpty()
            => (setOfMolecules == null || setOfMolecules.IsEmpty())
            && (setOfReactions == null || setOfReactions.IsEmpty())
            && (ringSet == null || ringSet.IsEmpty())
            && (crystal == null || crystal.IsEmpty());
    }
}
