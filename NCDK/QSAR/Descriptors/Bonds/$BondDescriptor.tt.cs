
// Abstract bond descriptor class with helper functions for descriptors
// that require the whole molecule to calculate the descriptor values,
// which in turn need to be cached for all bonds, so that they can be
// retrieved one by one.
// @cdk.module qsar
// @cdk.githash
    
using NCDK.QSAR.Results;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Bonds
{
    public partial class BondSigmaElectronegativityDescriptor
    {
        private const string PreviousAtomContainer = "previousAtomContainer";

        private IDictionary<object, object> cachedDescriptorValues = null;

        /// <summary>
        /// Returns true if the cached IDescriptorResult's are for the given IAtomContainer.
        /// </summary>
        /// <param name="container"></param>
        /// <returns>false, if the cache is for a different IAtomContainer</returns>
        public bool IsCachedAtomContainer(IAtomContainer container)
        {
            if (cachedDescriptorValues == null) return false;
            return (cachedDescriptorValues[PreviousAtomContainer] == container);
        }

        /// <summary>
        /// Returns the cached DescriptorValue for the given IBond.
        /// </summary>
        /// <param name="bond">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IBond</returns>
        public IDescriptorResult GetCachedDescriptorValue(IBond bond)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[bond];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IBond. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="bond">IBond to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="doubleResult">DescriptorValue for the given IBond</param>
        public void CacheDescriptorValue(IBond bond, IAtomContainer container, IDescriptorResult doubleResult)
        {
            if (cachedDescriptorValues == null)
            {
                cachedDescriptorValues = new Dictionary<object, object>()
                {
                    [PreviousAtomContainer] = container,
                };
            }
            else if (cachedDescriptorValues[PreviousAtomContainer] != container)
            {
                cachedDescriptorValues.Clear();
                cachedDescriptorValues[PreviousAtomContainer] = container;
            }
            cachedDescriptorValues[bond] = doubleResult;
        }

        IDescriptorValue IBondDescriptor.Calculate(IBond bond, IAtomContainer atomContainer)
            => Calculate(bond, atomContainer);
    }
    public partial class BondPartialTChargeDescriptor
    {
        private const string PreviousAtomContainer = "previousAtomContainer";

        private IDictionary<object, object> cachedDescriptorValues = null;

        /// <summary>
        /// Returns true if the cached IDescriptorResult's are for the given IAtomContainer.
        /// </summary>
        /// <param name="container"></param>
        /// <returns>false, if the cache is for a different IAtomContainer</returns>
        public bool IsCachedAtomContainer(IAtomContainer container)
        {
            if (cachedDescriptorValues == null) return false;
            return (cachedDescriptorValues[PreviousAtomContainer] == container);
        }

        /// <summary>
        /// Returns the cached DescriptorValue for the given IBond.
        /// </summary>
        /// <param name="bond">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IBond</returns>
        public IDescriptorResult GetCachedDescriptorValue(IBond bond)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[bond];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IBond. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="bond">IBond to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="doubleResult">DescriptorValue for the given IBond</param>
        public void CacheDescriptorValue(IBond bond, IAtomContainer container, IDescriptorResult doubleResult)
        {
            if (cachedDescriptorValues == null)
            {
                cachedDescriptorValues = new Dictionary<object, object>()
                {
                    [PreviousAtomContainer] = container,
                };
            }
            else if (cachedDescriptorValues[PreviousAtomContainer] != container)
            {
                cachedDescriptorValues.Clear();
                cachedDescriptorValues[PreviousAtomContainer] = container;
            }
            cachedDescriptorValues[bond] = doubleResult;
        }

        IDescriptorValue IBondDescriptor.Calculate(IBond bond, IAtomContainer atomContainer)
            => Calculate(bond, atomContainer);
    }
    public partial class BondPartialSigmaChargeDescriptor
    {
        private const string PreviousAtomContainer = "previousAtomContainer";

        private IDictionary<object, object> cachedDescriptorValues = null;

        /// <summary>
        /// Returns true if the cached IDescriptorResult's are for the given IAtomContainer.
        /// </summary>
        /// <param name="container"></param>
        /// <returns>false, if the cache is for a different IAtomContainer</returns>
        public bool IsCachedAtomContainer(IAtomContainer container)
        {
            if (cachedDescriptorValues == null) return false;
            return (cachedDescriptorValues[PreviousAtomContainer] == container);
        }

        /// <summary>
        /// Returns the cached DescriptorValue for the given IBond.
        /// </summary>
        /// <param name="bond">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IBond</returns>
        public IDescriptorResult GetCachedDescriptorValue(IBond bond)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[bond];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IBond. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="bond">IBond to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="doubleResult">DescriptorValue for the given IBond</param>
        public void CacheDescriptorValue(IBond bond, IAtomContainer container, IDescriptorResult doubleResult)
        {
            if (cachedDescriptorValues == null)
            {
                cachedDescriptorValues = new Dictionary<object, object>()
                {
                    [PreviousAtomContainer] = container,
                };
            }
            else if (cachedDescriptorValues[PreviousAtomContainer] != container)
            {
                cachedDescriptorValues.Clear();
                cachedDescriptorValues[PreviousAtomContainer] = container;
            }
            cachedDescriptorValues[bond] = doubleResult;
        }

        IDescriptorValue IBondDescriptor.Calculate(IBond bond, IAtomContainer atomContainer)
            => Calculate(bond, atomContainer);
    }
    public partial class BondPartialPiChargeDescriptor
    {
        private const string PreviousAtomContainer = "previousAtomContainer";

        private IDictionary<object, object> cachedDescriptorValues = null;

        /// <summary>
        /// Returns true if the cached IDescriptorResult's are for the given IAtomContainer.
        /// </summary>
        /// <param name="container"></param>
        /// <returns>false, if the cache is for a different IAtomContainer</returns>
        public bool IsCachedAtomContainer(IAtomContainer container)
        {
            if (cachedDescriptorValues == null) return false;
            return (cachedDescriptorValues[PreviousAtomContainer] == container);
        }

        /// <summary>
        /// Returns the cached DescriptorValue for the given IBond.
        /// </summary>
        /// <param name="bond">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IBond</returns>
        public IDescriptorResult GetCachedDescriptorValue(IBond bond)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[bond];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IBond. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="bond">IBond to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="doubleResult">DescriptorValue for the given IBond</param>
        public void CacheDescriptorValue(IBond bond, IAtomContainer container, IDescriptorResult doubleResult)
        {
            if (cachedDescriptorValues == null)
            {
                cachedDescriptorValues = new Dictionary<object, object>()
                {
                    [PreviousAtomContainer] = container,
                };
            }
            else if (cachedDescriptorValues[PreviousAtomContainer] != container)
            {
                cachedDescriptorValues.Clear();
                cachedDescriptorValues[PreviousAtomContainer] = container;
            }
            cachedDescriptorValues[bond] = doubleResult;
        }

        IDescriptorValue IBondDescriptor.Calculate(IBond bond, IAtomContainer atomContainer)
            => Calculate(bond, atomContainer);
    }
    public partial class AtomicNumberDifferenceDescriptor
    {
        private const string PreviousAtomContainer = "previousAtomContainer";

        private IDictionary<object, object> cachedDescriptorValues = null;

        /// <summary>
        /// Returns true if the cached IDescriptorResult's are for the given IAtomContainer.
        /// </summary>
        /// <param name="container"></param>
        /// <returns>false, if the cache is for a different IAtomContainer</returns>
        public bool IsCachedAtomContainer(IAtomContainer container)
        {
            if (cachedDescriptorValues == null) return false;
            return (cachedDescriptorValues[PreviousAtomContainer] == container);
        }

        /// <summary>
        /// Returns the cached DescriptorValue for the given IBond.
        /// </summary>
        /// <param name="bond">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IBond</returns>
        public IDescriptorResult GetCachedDescriptorValue(IBond bond)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[bond];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IBond. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="bond">IBond to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="doubleResult">DescriptorValue for the given IBond</param>
        public void CacheDescriptorValue(IBond bond, IAtomContainer container, IDescriptorResult doubleResult)
        {
            if (cachedDescriptorValues == null)
            {
                cachedDescriptorValues = new Dictionary<object, object>()
                {
                    [PreviousAtomContainer] = container,
                };
            }
            else if (cachedDescriptorValues[PreviousAtomContainer] != container)
            {
                cachedDescriptorValues.Clear();
                cachedDescriptorValues[PreviousAtomContainer] = container;
            }
            cachedDescriptorValues[bond] = doubleResult;
        }

        IDescriptorValue IBondDescriptor.Calculate(IBond bond, IAtomContainer atomContainer)
            => Calculate(bond, atomContainer);
    }
}
