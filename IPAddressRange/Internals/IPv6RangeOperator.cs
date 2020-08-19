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

        public IPv6RangeOperator(IPAddressRange range)
        {
            Begin = range.Begin.ToBigInteger();
            End = range.End.ToBigInteger();
        }

        public bool Contains(IPAddress ipaddress)
        {
            var address = ipaddress.ToBigInteger();
            return Begin <= address && address <= End;
        }

        public bool Contains(IPAddressRange range)
        {
            var rangeBegin = range.Begin.ToBigInteger();
            var rangeEnd = range.End.ToBigInteger();
            return Begin <= rangeBegin && rangeEnd <= End;
        }

        public IEnumerator<IPAddress> GetEnumerator()
        {
            for (BigInteger adr = Begin; adr <= End; adr++)
            {
                yield return adr.ToIPv6Address();
            }
        }

        int ICollection<IPAddress>.Count => (int)((End - Begin) + 1);

        bool ICollection<IPAddress>.IsReadOnly => true;

        void ICollection<IPAddress>.Add(IPAddress item) => throw new InvalidOperationException();

        void ICollection<IPAddress>.Clear() => throw new InvalidOperationException();

        bool ICollection<IPAddress>.Contains(IPAddress item)
        {
            return this.Contains(item);
        }

        void ICollection<IPAddress>.CopyTo(IPAddress[] array, int arrayIndex)
        {
            if ((array.Length - arrayIndex) < (this as ICollection<IPAddress>).Count) throw new ArgumentException();

            foreach (var ipAddress in this)
            {
                array[arrayIndex++] = ipAddress;
            }
        }

        bool ICollection<IPAddress>.Remove(IPAddress item) => throw new InvalidOperationException();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
