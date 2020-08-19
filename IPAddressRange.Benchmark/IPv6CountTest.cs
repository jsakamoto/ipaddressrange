using System.Linq;
using BenchmarkDotNet.Attributes;

namespace NetTools.Benchmark
{
    [ShortRunJob]
    public class IPv6CountTest
    {
        [Benchmark]
        public int UsePrevVersion()
        {
            var ipAddressRange = NetTools.PrevVersion.IPAddressRange.Parse("fe80::0000:0000-fe80::0100:0000");
            return ipAddressRange.AsEnumerable().Count();
        }

        [Benchmark]
        public int UseLatestVersion()
        {
            var ipAddressRange = NetTools.IPAddressRange.Parse("fe80::0000:0000-fe80::0100:0000");
            return ipAddressRange.AsEnumerable().Count();
        }

    }
}
