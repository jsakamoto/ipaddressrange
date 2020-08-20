using System;
using System.Linq;
using System.Net;
using BenchmarkDotNet.Attributes;

namespace NetTools.Benchmark
{
    [ShortRunJob]
    [MemoryDiagnoser]
    public class IPv4ToUInt32Test
    {
        private readonly IPAddress IPAddress = IPAddress.Parse("192.168.0.1");

        [Benchmark]
        public UInt32 ArrayRevers()
        {
            var addressBytes = IPAddress.GetAddressBytes();
            Array.Reverse(addressBytes);
            return BitConverter.ToUInt32(addressBytes);
        }

        [Benchmark]
        public UInt32 LinqReversToArray()
        {
            var addressBytes = IPAddress.GetAddressBytes();
            return BitConverter.ToUInt32(addressBytes.Reverse().ToArray());
        }

        [Benchmark]
        public UInt32 BitOperation()
        {
            var addressBytes = IPAddress.GetAddressBytes();
            return
                ((UInt32)addressBytes[0]) << 24 |
                ((UInt32)addressBytes[1]) << 16 |
                ((UInt32)addressBytes[2]) << 8 |
                ((UInt32)addressBytes[3]);
        }
    }
}
