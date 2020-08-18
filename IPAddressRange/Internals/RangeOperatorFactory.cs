using System.Net.Sockets;

namespace NetTools.Internals
{
    internal static class RangeOperatorFactory
    {
        public static IRangeOperator Create(IPAddressRange range)
        {
            return range.Begin.AddressFamily == AddressFamily.InterNetwork ?
                new IPv4RangeOperator(range) :
                new IPv6RangeOperator(range) as IRangeOperator;
        }
    }
}
