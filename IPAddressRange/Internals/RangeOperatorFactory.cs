using System;
using System.Net.Sockets;

namespace NetTools.Internals
{
    internal static class RangeOperatorFactory
    {
        public static IRangeOperator Create(IPAddressRange range)
        {
            if (range.Begin.AddressFamily != range.End.AddressFamily) throw new InvalidOperationException("Both Begin and End properties must be of the same address family");

            var beginBytes = range.Begin.GetAddressBytes();
            var endBytes = range.End.GetAddressBytes();
            if (!Bits.GtECore(endBytes, beginBytes)) throw new InvalidOperationException("Begin must be smaller than the End");

            return range.Begin.AddressFamily == AddressFamily.InterNetwork ?
                new IPv4RangeOperator(range) :
                new IPv6RangeOperator(range) as IRangeOperator;
        }
    }
}
