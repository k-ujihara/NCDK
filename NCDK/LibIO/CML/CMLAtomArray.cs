using NCDK.Common.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCDK.LibIO.CML
{
    public partial class CMLAtomArray 
    {
        public void Add(CMLAtom atom)
        {
            base.Add(atom);
        }

        public string[] ElementType
        {
            get
            {
                var v = Attribute(Attribute_elementType)?.Value;
                if (v == null) return Array.Empty<string>();
                return Strings.Tokenize(v, ' ').ToArray();
            }
            set
            {
                SetAttributeValue(Attribute_elementType, value?.Length == 0 ? null : Concat(value));
            }
        }

        public double[] Count
        {
            get
            {
                var v = Attribute(Attribute_count)?.Value;
                if (v == null) return Array.Empty<double>();
                return Strings.Tokenize(v, ' ').Select(n => double.Parse(n)).ToArray();
            }
            set
            {
                SetAttributeValue(Attribute_count, value?.Length == 0 ? null : Concat(value));
            }
        }

        /// <summary>
        /// sorts atomArray. 
        /// currently only works for array type syntax.
        /// </summary>
        /// <param name="sort"></param>
        public void Sort(CMLFormula.Sorts sort)
        {
            var elems = ElementType;
            var counts = Count;
            var sortS = new List<string>();
            {
                for (var i = 0; i < elems.Length; i++)
                    sortS.Add(elems[i] + " " + counts[i]);
            }
            sortS.Sort();
            if (sort == CMLFormula.Sorts.ALPHABETIC_ELEMENTS)
            {
                ; // already done
            }
            else if (sort == CMLFormula.Sorts.CHFIRST)
            {
                var temp = new List<string>();
                foreach (var s in sortS)
                    if (s.StartsWith("C "))
                        temp.Add(s);
                foreach (var s in sortS)
                    if (s.StartsWith("H "))
                        temp.Add(s);
                foreach (var s in sortS)
                    if (!(s.StartsWith("C ") || s.StartsWith("H ")))
                        temp.Add(s);
                sortS = temp;
            }
            var el = new string[sortS.Count];
            var cl = new double[sortS.Count];
            {
                for (int i = 0; i < sortS.Count; i++)
                {
                    var ss = sortS[i].Split(' ');
                    el[i] = ss[0];
                    cl[i] = double.Parse(ss[1]);
                }
            }
            this.ElementType = el;
            this.Count = cl;
        }

        /// <summary>
        /// generates concise representation. corresponds to concise attribute in
        /// schema. only works if atomArray has elementType and count in array format
        /// </summary>
        /// <param name="formalCharge">(maybe omit this)</param>
        /// <returns>concise string</returns>
        /// <exception cref="ApplicationException">if atomArray of wrong sort</exception>
        public string GenerateConcise(int formalCharge)
        {
            var elems = ElementType ?? Array.Empty<string>();
            var counts = Count ?? Array.Empty<double>();
            if (counts.Length != elems.Length)
            {
                throw new ApplicationException($"atomArray has inconsistent counts/elems {counts.Length} {elems.Length}");
            }
            var sb = new StringBuilder();
            for (int i = 0; i < elems.Length; i++)
            {
                sb.Append(' ').Append(elems[i]).Append(' ');
                sb.Append(Strings.JavaFormat(counts[i], 4, true));
            }
            if (formalCharge != 0)
            {
                sb.Append(' ').Append(formalCharge);
            }
            var concise = (sb.Length == 0) ? "" : sb.ToString().Substring(1);
            return concise;
        }
    }
}
