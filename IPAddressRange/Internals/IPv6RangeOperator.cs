using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Numerics;

namespace NetTools.Internals
{
    internal class IPv6RangeOperator : IRangeOperator
    {
        private BigInteger Begin { get; }

        private BigInteger End { get; }

        public BigInteger AddressCount => this.End - this.Begin + 1;

        public IPv6RangeOperator(IPAddressRange range)
        {
            this.Begin = range.Begin.ToBigInteger();
            this.End = range.End.ToBigInteger();
        }

        public bool Contains(IPAddress ipaddress)
        {
            var address = ipaddress.ToBigInteger();
            return this.Begin <= address && address <= this.End;
        }

        public bool Contains(IPAddressRange range)
        {
            var rangeBegin = range.Begin.ToBigInteger();
            var rangeEnd = range.End.ToBigInteger();
            return this.Begin <= rangeBegin && rangeEnd <= this.End;
        }

        public IEnumerator<IPAddress> GetEnumerator()
        {
            for (var adr = this.Begin; ; adr++)
            {
                yield return adr.ToIPv6Address();
                if (adr == this.End) break;
            }
        }

        int ICollection<IPAddress>.Count => (int)AddressCount;

        bool ICollection<IPAddress>.IsReadOnly => true;

        void ICollection<IPAddress>.Add(IPAddress item) => throw new InvalidOperationException();

        void ICollection<IPAddress>.Clear() => throw new InvalidOperationException();

        bool ICollection<IPAddress>.Contains(IPAddress item)
        {
            return this.Contains(item);
        }

        void ICollection<IPAddress>.CopyTo(IPAddress[] array, int arrayIndex)
        {
            if ((array.Length - arrayIndex) < AddressCount) throw new ArgumentException();

            foreach (var ipAddress in this)
            {
                array[arrayIndex++] = ipAddress;
            }
        }

        bool ICollection<IPAddress>.Remove(IPAddress item) => throw new InvalidOperationException();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
