
// Abstract atomic descriptor class with helper functions for descriptors
// that require the whole molecule to calculate the descriptor values,
// which in turn need to be cached for all atoms, so that they can be
// retrieved one by one.
// @cdk.module qsar
// @cdk.githash

using NCDK.QSAR.Results;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Atomic
{
	public partial class VdWRadiusDescriptor
	{
        private const string PreviousAtomContainer = "previousAtomContainer";
        private IDictionary<object, object> cachedDescriptorValues = null;                   // FIXME: needs a better solution!

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
        /// Returns the cached DescriptorValue for the given IAtom.
        /// </summary>
        /// <param name="atom">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IAtom</returns>
        public IDescriptorResult GetCachedDescriptorValue(IAtom atom)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[atom];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IAtom. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="atom">IAtom to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="value">DescriptorValue for the given IAtom</param>
        public void CacheDescriptorValue(IAtom atom, IAtomContainer container, IDescriptorResult value)
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
            cachedDescriptorValues[atom] = value;
        }

		IDescriptorValue IAtomicDescriptor.Calculate(IAtom atom, IAtomContainer container)
            => Calculate(atom, container);
	}
	public partial class StabilizationPlusChargeDescriptor
	{
        private const string PreviousAtomContainer = "previousAtomContainer";
        private IDictionary<object, object> cachedDescriptorValues = null;                   // FIXME: needs a better solution!

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
        /// Returns the cached DescriptorValue for the given IAtom.
        /// </summary>
        /// <param name="atom">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IAtom</returns>
        public IDescriptorResult GetCachedDescriptorValue(IAtom atom)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[atom];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IAtom. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="atom">IAtom to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="value">DescriptorValue for the given IAtom</param>
        public void CacheDescriptorValue(IAtom atom, IAtomContainer container, IDescriptorResult value)
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
            cachedDescriptorValues[atom] = value;
        }

		IDescriptorValue IAtomicDescriptor.Calculate(IAtom atom, IAtomContainer container)
            => Calculate(atom, container);
	}
	public partial class SigmaElectronegativityDescriptor
	{
        private const string PreviousAtomContainer = "previousAtomContainer";
        private IDictionary<object, object> cachedDescriptorValues = null;                   // FIXME: needs a better solution!

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
        /// Returns the cached DescriptorValue for the given IAtom.
        /// </summary>
        /// <param name="atom">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IAtom</returns>
        public IDescriptorResult GetCachedDescriptorValue(IAtom atom)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[atom];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IAtom. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="atom">IAtom to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="value">DescriptorValue for the given IAtom</param>
        public void CacheDescriptorValue(IAtom atom, IAtomContainer container, IDescriptorResult value)
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
            cachedDescriptorValues[atom] = value;
        }

		IDescriptorValue IAtomicDescriptor.Calculate(IAtom atom, IAtomContainer container)
            => Calculate(atom, container);
	}
	public partial class RDFProtonDescriptor_GSR
	{
        private const string PreviousAtomContainer = "previousAtomContainer";
        private IDictionary<object, object> cachedDescriptorValues = null;                   // FIXME: needs a better solution!

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
        /// Returns the cached DescriptorValue for the given IAtom.
        /// </summary>
        /// <param name="atom">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IAtom</returns>
        public IDescriptorResult GetCachedDescriptorValue(IAtom atom)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[atom];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IAtom. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="atom">IAtom to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="value">DescriptorValue for the given IAtom</param>
        public void CacheDescriptorValue(IAtom atom, IAtomContainer container, IDescriptorResult value)
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
            cachedDescriptorValues[atom] = value;
        }

		IDescriptorValue IAtomicDescriptor.Calculate(IAtom atom, IAtomContainer container)
            => Calculate(atom, container);
	}
	public partial class RDFProtonDescriptor_GHR_topol
	{
        private const string PreviousAtomContainer = "previousAtomContainer";
        private IDictionary<object, object> cachedDescriptorValues = null;                   // FIXME: needs a better solution!

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
        /// Returns the cached DescriptorValue for the given IAtom.
        /// </summary>
        /// <param name="atom">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IAtom</returns>
        public IDescriptorResult GetCachedDescriptorValue(IAtom atom)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[atom];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IAtom. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="atom">IAtom to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="value">DescriptorValue for the given IAtom</param>
        public void CacheDescriptorValue(IAtom atom, IAtomContainer container, IDescriptorResult value)
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
            cachedDescriptorValues[atom] = value;
        }

		IDescriptorValue IAtomicDescriptor.Calculate(IAtom atom, IAtomContainer container)
            => Calculate(atom, container);
	}
	public partial class RDFProtonDescriptor_GHR
	{
        private const string PreviousAtomContainer = "previousAtomContainer";
        private IDictionary<object, object> cachedDescriptorValues = null;                   // FIXME: needs a better solution!

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
        /// Returns the cached DescriptorValue for the given IAtom.
        /// </summary>
        /// <param name="atom">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IAtom</returns>
        public IDescriptorResult GetCachedDescriptorValue(IAtom atom)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[atom];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IAtom. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="atom">IAtom to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="value">DescriptorValue for the given IAtom</param>
        public void CacheDescriptorValue(IAtom atom, IAtomContainer container, IDescriptorResult value)
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
            cachedDescriptorValues[atom] = value;
        }

		IDescriptorValue IAtomicDescriptor.Calculate(IAtom atom, IAtomContainer container)
            => Calculate(atom, container);
	}
	public partial class RDFProtonDescriptor_GDR
	{
        private const string PreviousAtomContainer = "previousAtomContainer";
        private IDictionary<object, object> cachedDescriptorValues = null;                   // FIXME: needs a better solution!

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
        /// Returns the cached DescriptorValue for the given IAtom.
        /// </summary>
        /// <param name="atom">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IAtom</returns>
        public IDescriptorResult GetCachedDescriptorValue(IAtom atom)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[atom];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IAtom. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="atom">IAtom to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="value">DescriptorValue for the given IAtom</param>
        public void CacheDescriptorValue(IAtom atom, IAtomContainer container, IDescriptorResult value)
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
            cachedDescriptorValues[atom] = value;
        }

		IDescriptorValue IAtomicDescriptor.Calculate(IAtom atom, IAtomContainer container)
            => Calculate(atom, container);
	}
	public partial class RDFProtonDescriptor_G3R
	{
        private const string PreviousAtomContainer = "previousAtomContainer";
        private IDictionary<object, object> cachedDescriptorValues = null;                   // FIXME: needs a better solution!

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
        /// Returns the cached DescriptorValue for the given IAtom.
        /// </summary>
        /// <param name="atom">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IAtom</returns>
        public IDescriptorResult GetCachedDescriptorValue(IAtom atom)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[atom];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IAtom. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="atom">IAtom to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="value">DescriptorValue for the given IAtom</param>
        public void CacheDescriptorValue(IAtom atom, IAtomContainer container, IDescriptorResult value)
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
            cachedDescriptorValues[atom] = value;
        }

		IDescriptorValue IAtomicDescriptor.Calculate(IAtom atom, IAtomContainer container)
            => Calculate(atom, container);
	}
	public partial class ProtonTotalPartialChargeDescriptor
	{
        private const string PreviousAtomContainer = "previousAtomContainer";
        private IDictionary<object, object> cachedDescriptorValues = null;                   // FIXME: needs a better solution!

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
        /// Returns the cached DescriptorValue for the given IAtom.
        /// </summary>
        /// <param name="atom">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IAtom</returns>
        public IDescriptorResult GetCachedDescriptorValue(IAtom atom)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[atom];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IAtom. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="atom">IAtom to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="value">DescriptorValue for the given IAtom</param>
        public void CacheDescriptorValue(IAtom atom, IAtomContainer container, IDescriptorResult value)
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
            cachedDescriptorValues[atom] = value;
        }

		IDescriptorValue IAtomicDescriptor.Calculate(IAtom atom, IAtomContainer container)
            => Calculate(atom, container);
	}
	public partial class ProtonAffinityHOSEDescriptor
	{
        private const string PreviousAtomContainer = "previousAtomContainer";
        private IDictionary<object, object> cachedDescriptorValues = null;                   // FIXME: needs a better solution!

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
        /// Returns the cached DescriptorValue for the given IAtom.
        /// </summary>
        /// <param name="atom">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IAtom</returns>
        public IDescriptorResult GetCachedDescriptorValue(IAtom atom)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[atom];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IAtom. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="atom">IAtom to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="value">DescriptorValue for the given IAtom</param>
        public void CacheDescriptorValue(IAtom atom, IAtomContainer container, IDescriptorResult value)
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
            cachedDescriptorValues[atom] = value;
        }

		IDescriptorValue IAtomicDescriptor.Calculate(IAtom atom, IAtomContainer container)
            => Calculate(atom, container);
	}
	public partial class PiElectronegativityDescriptor
	{
        private const string PreviousAtomContainer = "previousAtomContainer";
        private IDictionary<object, object> cachedDescriptorValues = null;                   // FIXME: needs a better solution!

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
        /// Returns the cached DescriptorValue for the given IAtom.
        /// </summary>
        /// <param name="atom">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IAtom</returns>
        public IDescriptorResult GetCachedDescriptorValue(IAtom atom)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[atom];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IAtom. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="atom">IAtom to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="value">DescriptorValue for the given IAtom</param>
        public void CacheDescriptorValue(IAtom atom, IAtomContainer container, IDescriptorResult value)
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
            cachedDescriptorValues[atom] = value;
        }

		IDescriptorValue IAtomicDescriptor.Calculate(IAtom atom, IAtomContainer container)
            => Calculate(atom, container);
	}
	public partial class PeriodicTablePositionDescriptor
	{
        private const string PreviousAtomContainer = "previousAtomContainer";
        private IDictionary<object, object> cachedDescriptorValues = null;                   // FIXME: needs a better solution!

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
        /// Returns the cached DescriptorValue for the given IAtom.
        /// </summary>
        /// <param name="atom">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IAtom</returns>
        public IDescriptorResult GetCachedDescriptorValue(IAtom atom)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[atom];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IAtom. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="atom">IAtom to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="value">DescriptorValue for the given IAtom</param>
        public void CacheDescriptorValue(IAtom atom, IAtomContainer container, IDescriptorResult value)
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
            cachedDescriptorValues[atom] = value;
        }

		IDescriptorValue IAtomicDescriptor.Calculate(IAtom atom, IAtomContainer container)
            => Calculate(atom, container);
	}
	public partial class PartialTChargePEOEDescriptor
	{
        private const string PreviousAtomContainer = "previousAtomContainer";
        private IDictionary<object, object> cachedDescriptorValues = null;                   // FIXME: needs a better solution!

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
        /// Returns the cached DescriptorValue for the given IAtom.
        /// </summary>
        /// <param name="atom">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IAtom</returns>
        public IDescriptorResult GetCachedDescriptorValue(IAtom atom)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[atom];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IAtom. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="atom">IAtom to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="value">DescriptorValue for the given IAtom</param>
        public void CacheDescriptorValue(IAtom atom, IAtomContainer container, IDescriptorResult value)
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
            cachedDescriptorValues[atom] = value;
        }

		IDescriptorValue IAtomicDescriptor.Calculate(IAtom atom, IAtomContainer container)
            => Calculate(atom, container);
	}
	public partial class PartialTChargeMMFF94Descriptor
	{
        private const string PreviousAtomContainer = "previousAtomContainer";
        private IDictionary<object, object> cachedDescriptorValues = null;                   // FIXME: needs a better solution!

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
        /// Returns the cached DescriptorValue for the given IAtom.
        /// </summary>
        /// <param name="atom">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IAtom</returns>
        public IDescriptorResult GetCachedDescriptorValue(IAtom atom)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[atom];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IAtom. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="atom">IAtom to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="value">DescriptorValue for the given IAtom</param>
        public void CacheDescriptorValue(IAtom atom, IAtomContainer container, IDescriptorResult value)
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
            cachedDescriptorValues[atom] = value;
        }

		IDescriptorValue IAtomicDescriptor.Calculate(IAtom atom, IAtomContainer container)
            => Calculate(atom, container);
	}
	public partial class PartialSigmaChargeDescriptor
	{
        private const string PreviousAtomContainer = "previousAtomContainer";
        private IDictionary<object, object> cachedDescriptorValues = null;                   // FIXME: needs a better solution!

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
        /// Returns the cached DescriptorValue for the given IAtom.
        /// </summary>
        /// <param name="atom">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IAtom</returns>
        public IDescriptorResult GetCachedDescriptorValue(IAtom atom)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[atom];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IAtom. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="atom">IAtom to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="value">DescriptorValue for the given IAtom</param>
        public void CacheDescriptorValue(IAtom atom, IAtomContainer container, IDescriptorResult value)
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
            cachedDescriptorValues[atom] = value;
        }

		IDescriptorValue IAtomicDescriptor.Calculate(IAtom atom, IAtomContainer container)
            => Calculate(atom, container);
	}
	public partial class PartialPiChargeDescriptor
	{
        private const string PreviousAtomContainer = "previousAtomContainer";
        private IDictionary<object, object> cachedDescriptorValues = null;                   // FIXME: needs a better solution!

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
        /// Returns the cached DescriptorValue for the given IAtom.
        /// </summary>
        /// <param name="atom">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IAtom</returns>
        public IDescriptorResult GetCachedDescriptorValue(IAtom atom)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[atom];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IAtom. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="atom">IAtom to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="value">DescriptorValue for the given IAtom</param>
        public void CacheDescriptorValue(IAtom atom, IAtomContainer container, IDescriptorResult value)
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
            cachedDescriptorValues[atom] = value;
        }

		IDescriptorValue IAtomicDescriptor.Calculate(IAtom atom, IAtomContainer container)
            => Calculate(atom, container);
	}
	public partial class IsProtonInConjugatedPiSystemDescriptor
	{
        private const string PreviousAtomContainer = "previousAtomContainer";
        private IDictionary<object, object> cachedDescriptorValues = null;                   // FIXME: needs a better solution!

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
        /// Returns the cached DescriptorValue for the given IAtom.
        /// </summary>
        /// <param name="atom">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IAtom</returns>
        public IDescriptorResult GetCachedDescriptorValue(IAtom atom)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[atom];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IAtom. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="atom">IAtom to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="value">DescriptorValue for the given IAtom</param>
        public void CacheDescriptorValue(IAtom atom, IAtomContainer container, IDescriptorResult value)
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
            cachedDescriptorValues[atom] = value;
        }

		IDescriptorValue IAtomicDescriptor.Calculate(IAtom atom, IAtomContainer container)
            => Calculate(atom, container);
	}
	public partial class IsProtonInAromaticSystemDescriptor
	{
        private const string PreviousAtomContainer = "previousAtomContainer";
        private IDictionary<object, object> cachedDescriptorValues = null;                   // FIXME: needs a better solution!

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
        /// Returns the cached DescriptorValue for the given IAtom.
        /// </summary>
        /// <param name="atom">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IAtom</returns>
        public IDescriptorResult GetCachedDescriptorValue(IAtom atom)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[atom];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IAtom. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="atom">IAtom to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="value">DescriptorValue for the given IAtom</param>
        public void CacheDescriptorValue(IAtom atom, IAtomContainer container, IDescriptorResult value)
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
            cachedDescriptorValues[atom] = value;
        }

		IDescriptorValue IAtomicDescriptor.Calculate(IAtom atom, IAtomContainer container)
            => Calculate(atom, container);
	}
	public partial class IPAtomicHOSEDescriptor
	{
        private const string PreviousAtomContainer = "previousAtomContainer";
        private IDictionary<object, object> cachedDescriptorValues = null;                   // FIXME: needs a better solution!

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
        /// Returns the cached DescriptorValue for the given IAtom.
        /// </summary>
        /// <param name="atom">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IAtom</returns>
        public IDescriptorResult GetCachedDescriptorValue(IAtom atom)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[atom];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IAtom. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="atom">IAtom to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="value">DescriptorValue for the given IAtom</param>
        public void CacheDescriptorValue(IAtom atom, IAtomContainer container, IDescriptorResult value)
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
            cachedDescriptorValues[atom] = value;
        }

		IDescriptorValue IAtomicDescriptor.Calculate(IAtom atom, IAtomContainer container)
            => Calculate(atom, container);
	}
	public partial class InductiveAtomicSoftnessDescriptor
	{
        private const string PreviousAtomContainer = "previousAtomContainer";
        private IDictionary<object, object> cachedDescriptorValues = null;                   // FIXME: needs a better solution!

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
        /// Returns the cached DescriptorValue for the given IAtom.
        /// </summary>
        /// <param name="atom">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IAtom</returns>
        public IDescriptorResult GetCachedDescriptorValue(IAtom atom)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[atom];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IAtom. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="atom">IAtom to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="value">DescriptorValue for the given IAtom</param>
        public void CacheDescriptorValue(IAtom atom, IAtomContainer container, IDescriptorResult value)
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
            cachedDescriptorValues[atom] = value;
        }

		IDescriptorValue IAtomicDescriptor.Calculate(IAtom atom, IAtomContainer container)
            => Calculate(atom, container);
	}
	public partial class InductiveAtomicHardnessDescriptor
	{
        private const string PreviousAtomContainer = "previousAtomContainer";
        private IDictionary<object, object> cachedDescriptorValues = null;                   // FIXME: needs a better solution!

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
        /// Returns the cached DescriptorValue for the given IAtom.
        /// </summary>
        /// <param name="atom">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IAtom</returns>
        public IDescriptorResult GetCachedDescriptorValue(IAtom atom)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[atom];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IAtom. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="atom">IAtom to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="value">DescriptorValue for the given IAtom</param>
        public void CacheDescriptorValue(IAtom atom, IAtomContainer container, IDescriptorResult value)
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
            cachedDescriptorValues[atom] = value;
        }

		IDescriptorValue IAtomicDescriptor.Calculate(IAtom atom, IAtomContainer container)
            => Calculate(atom, container);
	}
	public partial class EffectiveAtomPolarizabilityDescriptor
	{
        private const string PreviousAtomContainer = "previousAtomContainer";
        private IDictionary<object, object> cachedDescriptorValues = null;                   // FIXME: needs a better solution!

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
        /// Returns the cached DescriptorValue for the given IAtom.
        /// </summary>
        /// <param name="atom">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IAtom</returns>
        public IDescriptorResult GetCachedDescriptorValue(IAtom atom)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[atom];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IAtom. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="atom">IAtom to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="value">DescriptorValue for the given IAtom</param>
        public void CacheDescriptorValue(IAtom atom, IAtomContainer container, IDescriptorResult value)
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
            cachedDescriptorValues[atom] = value;
        }

		IDescriptorValue IAtomicDescriptor.Calculate(IAtom atom, IAtomContainer container)
            => Calculate(atom, container);
	}
	public partial class DistanceToAtomDescriptor
	{
        private const string PreviousAtomContainer = "previousAtomContainer";
        private IDictionary<object, object> cachedDescriptorValues = null;                   // FIXME: needs a better solution!

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
        /// Returns the cached DescriptorValue for the given IAtom.
        /// </summary>
        /// <param name="atom">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IAtom</returns>
        public IDescriptorResult GetCachedDescriptorValue(IAtom atom)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[atom];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IAtom. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="atom">IAtom to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="value">DescriptorValue for the given IAtom</param>
        public void CacheDescriptorValue(IAtom atom, IAtomContainer container, IDescriptorResult value)
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
            cachedDescriptorValues[atom] = value;
        }

		IDescriptorValue IAtomicDescriptor.Calculate(IAtom atom, IAtomContainer container)
            => Calculate(atom, container);
	}
	public partial class CovalentRadiusDescriptor
	{
        private const string PreviousAtomContainer = "previousAtomContainer";
        private IDictionary<object, object> cachedDescriptorValues = null;                   // FIXME: needs a better solution!

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
        /// Returns the cached DescriptorValue for the given IAtom.
        /// </summary>
        /// <param name="atom">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IAtom</returns>
        public IDescriptorResult GetCachedDescriptorValue(IAtom atom)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[atom];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IAtom. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="atom">IAtom to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="value">DescriptorValue for the given IAtom</param>
        public void CacheDescriptorValue(IAtom atom, IAtomContainer container, IDescriptorResult value)
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
            cachedDescriptorValues[atom] = value;
        }

		IDescriptorValue IAtomicDescriptor.Calculate(IAtom atom, IAtomContainer container)
            => Calculate(atom, container);
	}
	public partial class BondsToAtomDescriptor
	{
        private const string PreviousAtomContainer = "previousAtomContainer";
        private IDictionary<object, object> cachedDescriptorValues = null;                   // FIXME: needs a better solution!

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
        /// Returns the cached DescriptorValue for the given IAtom.
        /// </summary>
        /// <param name="atom">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IAtom</returns>
        public IDescriptorResult GetCachedDescriptorValue(IAtom atom)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[atom];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IAtom. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="atom">IAtom to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="value">DescriptorValue for the given IAtom</param>
        public void CacheDescriptorValue(IAtom atom, IAtomContainer container, IDescriptorResult value)
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
            cachedDescriptorValues[atom] = value;
        }

		IDescriptorValue IAtomicDescriptor.Calculate(IAtom atom, IAtomContainer container)
            => Calculate(atom, container);
	}
	public partial class AtomValenceDescriptor
	{
        private const string PreviousAtomContainer = "previousAtomContainer";
        private IDictionary<object, object> cachedDescriptorValues = null;                   // FIXME: needs a better solution!

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
        /// Returns the cached DescriptorValue for the given IAtom.
        /// </summary>
        /// <param name="atom">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IAtom</returns>
        public IDescriptorResult GetCachedDescriptorValue(IAtom atom)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[atom];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IAtom. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="atom">IAtom to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="value">DescriptorValue for the given IAtom</param>
        public void CacheDescriptorValue(IAtom atom, IAtomContainer container, IDescriptorResult value)
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
            cachedDescriptorValues[atom] = value;
        }

		IDescriptorValue IAtomicDescriptor.Calculate(IAtom atom, IAtomContainer container)
            => Calculate(atom, container);
	}
	public partial class AtomHybridizationVSEPRDescriptor
	{
        private const string PreviousAtomContainer = "previousAtomContainer";
        private IDictionary<object, object> cachedDescriptorValues = null;                   // FIXME: needs a better solution!

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
        /// Returns the cached DescriptorValue for the given IAtom.
        /// </summary>
        /// <param name="atom">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IAtom</returns>
        public IDescriptorResult GetCachedDescriptorValue(IAtom atom)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[atom];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IAtom. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="atom">IAtom to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="value">DescriptorValue for the given IAtom</param>
        public void CacheDescriptorValue(IAtom atom, IAtomContainer container, IDescriptorResult value)
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
            cachedDescriptorValues[atom] = value;
        }

		IDescriptorValue IAtomicDescriptor.Calculate(IAtom atom, IAtomContainer container)
            => Calculate(atom, container);
	}
	public partial class AtomHybridizationDescriptor
	{
        private const string PreviousAtomContainer = "previousAtomContainer";
        private IDictionary<object, object> cachedDescriptorValues = null;                   // FIXME: needs a better solution!

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
        /// Returns the cached DescriptorValue for the given IAtom.
        /// </summary>
        /// <param name="atom">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IAtom</returns>
        public IDescriptorResult GetCachedDescriptorValue(IAtom atom)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[atom];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IAtom. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="atom">IAtom to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="value">DescriptorValue for the given IAtom</param>
        public void CacheDescriptorValue(IAtom atom, IAtomContainer container, IDescriptorResult value)
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
            cachedDescriptorValues[atom] = value;
        }

		IDescriptorValue IAtomicDescriptor.Calculate(IAtom atom, IAtomContainer container)
            => Calculate(atom, container);
	}
	public partial class AtomDegreeDescriptor
	{
        private const string PreviousAtomContainer = "previousAtomContainer";
        private IDictionary<object, object> cachedDescriptorValues = null;                   // FIXME: needs a better solution!

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
        /// Returns the cached DescriptorValue for the given IAtom.
        /// </summary>
        /// <param name="atom">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IAtom</returns>
        public IDescriptorResult GetCachedDescriptorValue(IAtom atom)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[atom];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IAtom. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="atom">IAtom to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="value">DescriptorValue for the given IAtom</param>
        public void CacheDescriptorValue(IAtom atom, IAtomContainer container, IDescriptorResult value)
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
            cachedDescriptorValues[atom] = value;
        }

		IDescriptorValue IAtomicDescriptor.Calculate(IAtom atom, IAtomContainer container)
            => Calculate(atom, container);
	}
}
