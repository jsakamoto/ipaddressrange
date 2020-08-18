using System.Collections.Generic;
using System.Net;

namespace NetTools.Internals
{
    internal interface IRangeOperator
    {
        bool Contains(IPAddress ipaddress);
        bool Contains(IPAddressRange range);
        IEnumerator<IPAddress> GetEnumerator();
    }
}
