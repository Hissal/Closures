using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Closures;
using Closures.Anonymous;

namespace ClosureBenchmarks;

[MemoryDiagnoser(false)]
public class ActionVsAnonymousClosureBenchmarks {
    [Benchmark]
    public void Action() {
        var context = 100;
        var action = () => {
            var result = context + 1;
        };

        action.Invoke();
    }

    [Benchmark]
    public void ClosureAction() {
        var context = 100;
        var closure = Closure.Action(context, (int c) => {
            var result = c + 1;
        });

        closure.Invoke();
    }

    [Benchmark]
    public void ClosureAction_AsAnonymous() {
        var context = 100;
        var anonymous = Closure.Action(context, (int c) => {
            var result = c + 1;
        }).AsAnonymous();

        anonymous.Invoke();
    }
}