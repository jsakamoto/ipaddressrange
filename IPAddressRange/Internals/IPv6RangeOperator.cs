using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace NetTools.Internals
{
    internal class IPv6RangeOperator : IRangeOperator
    {
        private IPAddressRange Range { get; }

        public IPv6RangeOperator(IPAddressRange range)
        {
            this.Range = range;
        }

        public bool Contains(IPAddress ipaddress)
        {
            var offset = 0;
            if (Range.Begin.IsIPv4MappedToIPv6 && ipaddress.IsIPv4MappedToIPv6)
            {
                offset = 12; //ipv4 has prefix of 10 zero bytes and two 255 bytes. 
            }

            var adrBytes = ipaddress.GetAddressBytes();
            return Bits.LtECore(this.Range.Begin.GetAddressBytes(), adrBytes, offset) && Bits.GtECore(this.Range.End.GetAddressBytes(), adrBytes, offset);
        }

        public bool Contains(IPAddressRange range)
        {
            var offset = 0;
            if (Range.Begin.IsIPv4MappedToIPv6 && range.Begin.IsIPv4MappedToIPv6)
            {
                offset = 12; //ipv4 has prefix of 10 zero bytes and two 255 bytes. 
            }

            return
                Bits.LtECore(Range.Begin.GetAddressBytes(), range.Begin.GetAddressBytes(), offset) &&
                Bits.GtECore(Range.End.GetAddressBytes(), range.End.GetAddressBytes(), offset);
        }

        public IEnumerator<IPAddress> GetEnumerator()
        {
            var first = Range.Begin.GetAddressBytes();
            var last = Range.End.GetAddressBytes();
            for (var ip = first; Bits.LtECore(ip, last); ip = Bits.Increment(ip))
                yield return new IPAddress(ip);
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
