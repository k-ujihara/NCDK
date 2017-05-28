using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCDK.Fingerprints
{
    public abstract class AbstractFingerprinter : IFingerprinter
    {
        public abstract int Count { get; }

        /// <summary>
        /// Base classes should override this method to report the parameters they
        /// are configured with.
        /// </summary>
        /// <returns>The key=value pairs of configured parameters</returns>
        protected virtual IEnumerable<KeyValuePair<string, string>> GetParameters()
        {
            return Array.Empty<KeyValuePair<string, string>>();
        }

        public string GetVersionDescription()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("CDK-")
              .Append(GetType().Name)
              .Append("/")
              .Append(CDK.Version); // could version fingerprints separetely
            foreach (var param in GetParameters())
            {
                sb.Append(' ').Append(param.Key).Append('=').Append(param.Value);
            }
            return sb.ToString();
        }

        /// <inheritdoc/>
        public BitArray GetFingerprint(IAtomContainer mol)
        {
            return GetBitFingerprint(mol).AsBitSet();
        }

        public abstract IBitFingerprint GetBitFingerprint(IAtomContainer container);
        public abstract ICountFingerprint GetCountFingerprint(IAtomContainer container);
        public abstract IDictionary<string, int> GetRawFingerprint(IAtomContainer container);
    }
}
