using System.Net;
using BenchmarkDotNet.Attributes;

namespace NetTools.Benchmark
{
    [ShortRunJob]
    [MemoryDiagnoser]
    public class IPv4ContainsTest
    {
        private readonly IPAddress[] IPAddresses = new[] {
            IPAddress.Parse("192.168.0.12"),
            IPAddress.Parse("192.168.20.1"),
            IPAddress.Parse("10.0.0.3"),
            IPAddress.Parse("219.165.73.19"),
        };


        [Benchmark]
        public bool UsePrevVersion()
        {
            var range = NetTools.PrevVersion.IPAddressRange.Parse("192.168.0.0/16");

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
            var range = NetTools.IPAddressRange.Parse("192.168.0.0/16");

            var result = false;
            foreach (var ipAddress in IPAddresses)
            {
                result |= range.Contains(ipAddress);
            }
            return result;
        }
    }
}
