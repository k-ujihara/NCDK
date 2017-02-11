















// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2015-2016  Kazuya Ujihara

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace NCDK.Default
{
    [Serializable]
    public class Reaction
        : ChemObject, IReaction, ICloneable
    {
        private IAtomContainerSet<IAtomContainer> reactants;
        public IAtomContainerSet<IAtomContainer> Reactants => reactants;

        private IAtomContainerSet<IAtomContainer> products;
        public IAtomContainerSet<IAtomContainer> Products => products;

        private IAtomContainerSet<IAtomContainer> agents;
        public IAtomContainerSet<IAtomContainer> Agents => agents;

        private IList<IMapping> mappings;
        public IList<IMapping> Mappings => mappings;

        private ReactionDirection direction;
        public ReactionDirection Direction
        {
            get { return direction; }

            set
            {
                direction = value;
                 NotifyChanged();             }
        }

        public Reaction()
        {
            this.reactants = Builder.CreateAtomContainerSet();
            this.products = Builder.CreateAtomContainerSet();
            this.agents = Builder.CreateAtomContainerSet();
            this.mappings = new List<IMapping>();
            direction = ReactionDirection.Forward;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Reaction(");
            sb.Append(Id);
            sb.Append(", #M:").Append(mappings.Count);
            sb.Append(", reactants=").Append(reactants.ToString());
            sb.Append(", products=").Append(products.ToString());
            sb.Append(", agents=").Append(agents.ToString());
            sb.Append(')');
            return sb.ToString();
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            var clone_reactants = (IAtomContainerSet<IAtomContainer>)reactants.Clone(map);
            var clone_agents = (IAtomContainerSet<IAtomContainer>)agents.Clone(map);
            var clone_products = (IAtomContainerSet<IAtomContainer>)products.Clone(map);

            var clone_mappings = new ObservableCollection<IMapping>();
            foreach (var mapping in mappings)
                clone_mappings.Add((IMapping)mapping.Clone(map));

            Reaction clone = (Reaction)base.Clone(map);
            clone.reactants = clone_reactants;
            clone.agents = clone_agents;
            clone.products = clone_products;
            clone.mappings = clone_mappings;

            return clone;
        }
    }
}
namespace NCDK.Silent
{
    [Serializable]
    public class Reaction
        : ChemObject, IReaction, ICloneable
    {
        private IAtomContainerSet<IAtomContainer> reactants;
        public IAtomContainerSet<IAtomContainer> Reactants => reactants;

        private IAtomContainerSet<IAtomContainer> products;
        public IAtomContainerSet<IAtomContainer> Products => products;

        private IAtomContainerSet<IAtomContainer> agents;
        public IAtomContainerSet<IAtomContainer> Agents => agents;

        private IList<IMapping> mappings;
        public IList<IMapping> Mappings => mappings;

        private ReactionDirection direction;
        public ReactionDirection Direction
        {
            get { return direction; }

            set
            {
                direction = value;
                            }
        }

        public Reaction()
        {
            this.reactants = Builder.CreateAtomContainerSet();
            this.products = Builder.CreateAtomContainerSet();
            this.agents = Builder.CreateAtomContainerSet();
            this.mappings = new List<IMapping>();
            direction = ReactionDirection.Forward;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Reaction(");
            sb.Append(Id);
            sb.Append(", #M:").Append(mappings.Count);
            sb.Append(", reactants=").Append(reactants.ToString());
            sb.Append(", products=").Append(products.ToString());
            sb.Append(", agents=").Append(agents.ToString());
            sb.Append(')');
            return sb.ToString();
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            var clone_reactants = (IAtomContainerSet<IAtomContainer>)reactants.Clone(map);
            var clone_agents = (IAtomContainerSet<IAtomContainer>)agents.Clone(map);
            var clone_products = (IAtomContainerSet<IAtomContainer>)products.Clone(map);

            var clone_mappings = new ObservableCollection<IMapping>();
            foreach (var mapping in mappings)
                clone_mappings.Add((IMapping)mapping.Clone(map));

            Reaction clone = (Reaction)base.Clone(map);
            clone.reactants = clone_reactants;
            clone.agents = clone_agents;
            clone.products = clone_products;
            clone.mappings = clone_mappings;

            return clone;
        }
    }
}
