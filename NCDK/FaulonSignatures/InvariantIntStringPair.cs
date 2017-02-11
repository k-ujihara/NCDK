using System;

namespace FaulonSignatures
{

    public class InvariantIntStringPair : IComparable<InvariantIntStringPair>
    {

        public string string_;

        public int value;

        public int originalIndex;

        public InvariantIntStringPair(string string_, int value, int originalIndex)
        {
            this.string_ = string_;
            this.value = value;
            this.originalIndex = originalIndex;
        }

        public bool Equals(string string_, int value)
        {
            return this.value == value && this.string_.Equals(string_);
        }

        public bool Equals(InvariantIntStringPair o)
        {
            if (this.string_ == null || o.string_ == null) return false;
            return this.value == o.value && this.string_.Equals(o.string_);
        }

        public int CompareTo(InvariantIntStringPair o)
        {
            if (this.string_ == null || o.string_ == null) return 0;
            int c = this.string_.CompareTo(o.string_);
            if (c == 0)
            {
                if (this.value < o.value)
                {
                    return -1;
                }
                else if (this.value > o.value)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return c;
            }
        }

        public int GetOriginalIndex()
        {
            return originalIndex;
        }

        public override string ToString()
        {
            return this.string_ + "|" + this.value + "|" + this.originalIndex;
        }
    }
}
