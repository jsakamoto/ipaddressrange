using System.Collections.Generic;
using System.Net;
using System.Numerics;

namespace NetTools.Internals
{
    internal interface IRangeOperator : ICollection<IPAddress>
    {
        bool Contains(IPAddressRange range);

        BigInteger AddressCount { get; }
    }
}
