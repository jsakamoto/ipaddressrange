using System;
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
            Begin = range.Begin.ToUInt32();
            End = range.End.ToUInt32();
        }

        public bool Contains(IPAddress ipaddress)
        {
            var address = ipaddress.ToUInt32();
            return Begin <= address && address <= End;
        }

        public bool Contains(IPAddressRange range)
        {
            var rangeBegin = range.Begin.ToUInt32();
            var rangeEnd = range.End.ToUInt32();
            return Begin <= rangeBegin && rangeEnd <= End;
        }

        public IEnumerator<IPAddress> GetEnumerator()
        {
            for (UInt32 adr = Begin; adr <= End; adr++)
            {
                yield return adr.ToIPv4Address();
            }
        }
    }
}
