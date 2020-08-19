using System.Collections.Generic;
using System.Net;

namespace NetTools.Internals
{
    internal interface IRangeOperator : ICollection<IPAddress>
    {
        bool Contains(IPAddressRange range);
    }
}
