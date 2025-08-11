using BenchmarkDotNet.Running;

namespace ClosureBenchmarks;

public static class Program {
    public static void Main(string[] args) {
        BenchmarkRunner.Run<ActionVsClosureActionAsActionBenchmarks>();
    }
}