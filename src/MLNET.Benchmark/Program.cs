using BenchmarkDotNet.Running;

namespace MLNET.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<ClassificationBenchmark>();
            BenchmarkRunner.Run<ClassificationsBenchmark>();
        }
    }
}
