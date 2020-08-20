using BenchmarkDotNet.Running;

namespace NetTools.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            //BenchmarkRunner.Run<IPv4EnumerateTest>();
            //BenchmarkRunner.Run<IPv4CountTest>();
            //BenchmarkRunner.Run<IPv4ContainsTest>();

            //BenchmarkRunner.Run<IPv6EnumerateTest>();
            //BenchmarkRunner.Run<IPv6CountTest>();
            BenchmarkRunner.Run<IPv6ContainsTest>();

            //BenchmarkRunner.Run<IPv4ToUInt32Test>();
        }
    }
}
