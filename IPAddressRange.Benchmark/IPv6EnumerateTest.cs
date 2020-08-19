using BenchmarkDotNet.Attributes;

namespace NetTools.Benchmark
{
    [ShortRunJob]
    public class IPv6EnumerateTest
    {
        [Benchmark]
        public long UsePrevVersion()
        {
            var l = 0L;
            var ipAddressRange = NetTools.PrevVersion.IPAddressRange.Parse("fe80::0000:0000-fe80::0100:0000");
            foreach (var item in ipAddressRange.AsEnumerable())
            {
#pragma warning disable CS0618 // Type or member is obsolete
                l |= item.GetAddressBytes()[0];
#pragma warning restore CS0618 // Type or member is obsolete
            }
            return l;
        }

        [Benchmark]
        public long UseLatestVersion()
        {
            var l = 0L;
            var ipAddressRange = NetTools.IPAddressRange.Parse("fe80::0000:0000-fe80::0100:0000");
            foreach (var item in ipAddressRange.AsEnumerable())
            {
#pragma warning disable CS0618 // Type or member is obsolete
                l |= item.GetAddressBytes()[0];
#pragma warning restore CS0618 // Type or member is obsolete
            }
            return l;
        }
    }
}
