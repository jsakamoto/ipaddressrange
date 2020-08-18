using System;
using BenchmarkDotNet.Running;

namespace NetTools.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<IPv4EnumerateTest>();
        }
    }
}
