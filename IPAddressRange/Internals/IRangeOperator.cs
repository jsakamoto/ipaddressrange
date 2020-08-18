using System.Collections.Generic;
using System.Net;

namespace NetTools.Internals
{
    internal interface IRangeOperator : IEnumerable<IPAddress>
    {
        bool Contains(IPAddress ipaddress);
        bool Contains(IPAddressRange range);
    }
}
