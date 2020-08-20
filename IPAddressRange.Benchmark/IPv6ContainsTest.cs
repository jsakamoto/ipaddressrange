using System.Net;
using BenchmarkDotNet.Attributes;

namespace NetTools.Benchmark
{
    [ShortRunJob]
    [MemoryDiagnoser]
    public class IPv6ContainsTest
    {
        private readonly IPAddress[] IPAddresses = new[] {
            IPAddress.Parse("::1"),
            IPAddress.Parse("fe80::0000"),
            IPAddress.Parse("fe80::ABCD"),
            IPAddress.Parse("fe81::0000"),
        };


        [Benchmark]
        public bool UsePrevVersion()
        {
            var range = NetTools.PrevVersion.IPAddressRange.Parse("fe80::0000:0000-fe80::0100:0000");

            var result = false;
            foreach (var ipAddress in IPAddresses)
            {
                result |= range.Contains(ipAddress);
            }
            return result;
        }

        [Benchmark]
        public bool UseLatestVersion()
        {
            var range = NetTools.IPAddressRange.Parse("fe80::0000:0000-fe80::0100:0000");

            var result = false;
            foreach (var ipAddress in IPAddresses)
            {
                result |= range.Contains(ipAddress);
            }
            return result;
        }
    }
}
