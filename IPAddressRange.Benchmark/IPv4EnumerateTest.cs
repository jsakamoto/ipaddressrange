using System.Linq;
using BenchmarkDotNet.Attributes;

namespace NetTools.Benchmark
{
    [ShortRunJob]
    public class IPv4EnumerateTest
    {
        [Benchmark]
        public int UsePrevVersion()
        {
            var ipAddressRange = NetTools.PrevVersion.IPAddressRange.Parse("10.0.0.0/8");
            return ipAddressRange.AsEnumerable().Count();
        }

        [Benchmark]
        public int UseLatestVersion()
        {
            var ipAddressRange = NetTools.IPAddressRange.Parse("10.0.0.0/8");
            return ipAddressRange.AsEnumerable().Count();
        }
    }
}
