using System;
using BenchmarkDotNet.Running;

namespace NetTools.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            //BenchmarkRunner.Run<IPv4EnumerateTest>();
            //BenchmarkRunner.Run<IPv4CountTest>();

            BenchmarkRunner.Run<IPv6EnumerateTest>();
            //BenchmarkRunner.Run<IPv6CountTest>();
        }
    }
}
