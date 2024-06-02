using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace NetTools.Internals
{
    internal class IPv4RangeOperator : IRangeOperator
    {
        private UInt32 Begin { get; }

        private UInt32 End { get; }

        public IPv4RangeOperator(IPAddressRange range)
        {
            this.Begin = range.Begin.ToUInt32();
            this.End = range.End.ToUInt32();
        }

        public bool Contains(IPAddress ipaddress)
        {
            var address = ipaddress.ToUInt32();
            return this.Begin <= address && address <= this.End;
        }

        public bool Contains(IPAddressRange range)
        {
            var rangeBegin = range.Begin.ToUInt32();
            var rangeEnd = range.End.ToUInt32();
            return this.Begin <= rangeBegin && rangeEnd <= this.End;
        }

        public IEnumerator<IPAddress> GetEnumerator()
        {
            for (var adr = this.Begin; ; adr++)
            {
                yield return adr.ToIPv4Address();
                if (adr == this.End) break;
            }
        }

        int ICollection<IPAddress>.Count => (int)((this.End - this.Begin) + 1);

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
