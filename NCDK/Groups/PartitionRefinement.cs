namespace NCDK.Groups
{
    /// <summary>
    /// Factory for partition refiners. 
    /// </summary>
    /// <example>
    /// Use like:
    /// 
    /// <code>
    ///     AtomContainerDiscretePartitionRefiner refiner = PartitionRefinement.forAtoms().create();
    /// </code>
    /// 
    /// The methods forAtoms and forBonds return builders with methods to allow setting the
    /// switches for ignoring atom types and/or bond orders.
    /// </example>
    // @author maclean  
    public class PartitionRefinement
    {
        /// <returns>a builder that makes atom refiners</returns>
        public static AtomRefinerBuilder ForAtoms()
        {
            return new AtomRefinerBuilder();
        }

        public class AtomRefinerBuilder
        {
            private bool ignoreAtomTypes;
            private bool ignoreBondOrders;

            public AtomRefinerBuilder IgnoringAtomTypes()
            {
                this.ignoreAtomTypes = true;
                return this;
            }

            public AtomRefinerBuilder IgnoringBondOrders()
            {
                this.ignoreBondOrders = true;
                return this;
            }

            public AtomContainerDiscretePartitionRefiner Create()
            {
                return new AtomDiscretePartitionRefiner(ignoreAtomTypes, ignoreBondOrders);
            }
        }

        /// <returns>a builder that makes bond refiners</returns>
        public BondRefinerBuilder ForBonds()
        {
            return new BondRefinerBuilder();
        }

        public class BondRefinerBuilder
        {
            private bool ignoreBondOrders;

            public BondRefinerBuilder IgnoringBondOrders()
            {
                this.ignoreBondOrders = true;
                return this;
            }

            public AtomContainerDiscretePartitionRefiner Create()
            {
                return new BondDiscretePartitionRefiner(ignoreBondOrders);
            }
        }
    }
}
