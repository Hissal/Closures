using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Closures;
using Closures.Anonymous;

namespace ClosureBenchmarks;

public static class Program {
    public static void Main(string[] args) {
        BenchmarkRunner.Run<AnonymousBenchmarks>();
        return;
        var anonmark = new AnonymousBenchmarks();
        
        var anonymous = Closure.Action(20, (int c) => {
            // Simulate some work
            var result = c + 1;
        }).AsAnonymous();
        
        anonymous.Invoke();
        // Run the benchmarks

        // Uncomment to run the ForLoopBenchmarks
        // BenchmarkRunner.Run<ForLoopBenchmarks>();
    }
}

[MemoryDiagnoser(false)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class AnonymousValueBechmarks {
    [Benchmark]
    [BenchmarkCategory("100")]
    public void AnonymousValue_Create_100x() {
        for (int i = 0; i < 100; i++) {
            var anon = AnonymousValue.From(i);
        }
    }
    
    [Benchmark]
    [BenchmarkCategory("100")]
    public void AnonymousValue_CreateAndSet_100x() {
        for (int i = 0; i < 100; i++) {
            var anon = AnonymousValue.From(i);
            anon.SetValue(i + 1);
        }
    }
    
    [Benchmark]
    [BenchmarkCategory("100")]
    public void AnonymousValue_CreateAndSetDifferentType_100x() {
        for (int i = 0; i < 100; i++) {
            var anon = AnonymousValue.From(i);
            anon.SetValue(i + 1.0); // Set a different type (double)
        }
    }

    [Benchmark]
    [BenchmarkCategory("100")]
    public void AnonymousValue_CreateAndCast_100x() {
        for (int i = 0; i < 100; i++) {
            var anon = AnonymousValue.From(i);
            var casted = anon.As<int>();
        }
    }

    [Benchmark]
    [BenchmarkCategory("100")]
    public void AnonymousValue_CreateAndCastWithIsCheck_100x() {
        for (int i = 0; i < 100; i++) {
            var anon = AnonymousValue.From(i);
            if (anon.Is<int>()) {
                var casted = anon.As<int>();
            }
            else {
                throw new InvalidOperationException();
            }
        }
    }

    [Benchmark]
    [BenchmarkCategory("100")]
    public void AnonymousValue_CreateAndCastWithIsCheckAndBoxing_100x() {
        for (int i = 0; i < 100; i++) {
            var anon = AnonymousValue.From((i, 0));
            if (anon.Is<(int, int)>()) {
                var casted = anon.As<(int, int)>();
            }
            else {
                throw new InvalidOperationException();
            }
        }
    }
}

[MemoryDiagnoser(false)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class AnonymousBenchmarks {
    const int c_iterationCount = 10;
    const int c_invocationCount = 10;

    // [Benchmark]
    // [BenchmarkCategory("1NoInvoke")]
    // public void Action_NoInvoke() {
    //     for (int i = 0; i < c_iterationCount; i++) {
    //         var context = i; // Capture the current value of i
    //         var action = new Action(() => {
    //             // Simulate some work
    //             var result = context + 1;
    //         });
    //     }
    // }
    //
    // [Benchmark]
    // [BenchmarkCategory("1NoInvoke")]
    // public void ClosureAction_NoInvoke() {
    //     for (int i = 0; i < c_iterationCount; i++) {
    //         var context = i;
    //         var closure = Closure.Action(context, (int c) => {
    //             // Simulate some work
    //             var result = c + 1;
    //         });
    //     }
    // }
    //
    // [Benchmark]
    // [BenchmarkCategory("1NoInvoke")]
    // public void ClosureAction_AsAction_NoInvoke() {
    //     for (int i = 0; i < c_iterationCount; i++) {
    //         var context = i;
    //         var action = Closure.Action(context, (int c) => {
    //             // Simulate some work
    //             var result = c + 1;
    //         }).AsAction();
    //     }
    // }

    [Benchmark]
    [BenchmarkCategory("1NoInvoke")]
    public void AnonymousClosure_NoInvoke() {
        for (int i = 0; i < c_iterationCount; i++) {
            var context = i;
            var anonymous = AnonymousClosure.Create(AnonymousValue.From(context), (int c) => {
                // Simulate some work
                var result = c + 1;
            });
        }
    }

    // [Benchmark]
    // [BenchmarkCategory("2Single")]
    // public void Action() {
    //     for (int i = 0; i < c_iterationCount; i++) {
    //         var context = i; // Capture the current value of i
    //         var action = new Action(() => {
    //             // Simulate some work
    //             var result = context + 1;
    //         });
    //         action.Invoke();
    //     }
    // }
    //
    // [Benchmark]
    // [BenchmarkCategory("2Single")]
    // public void ClosureAction() {
    //     for (int i = 0; i < c_iterationCount; i++) {
    //         var context = i;
    //         var closure = Closure.Action(context, (int c) => {
    //             // Simulate some work
    //             var result = c + 1;
    //         });
    //         closure.Invoke();
    //     }
    // }
    //
    // [Benchmark]
    // [BenchmarkCategory("2Single")]
    // public void ClosureAction_AsAction() {
    //     for (int i = 0; i < c_iterationCount; i++) {
    //         var context = i;
    //         var action = Closure.Action(context, (int c) => {
    //             // Simulate some work
    //             var result = c + 1;
    //         }).AsAction();
    //         action.Invoke();
    //     }
    // }

    [Benchmark]
    [BenchmarkCategory("2Single")]
    public void AnonymousClosure_SingleInvoke() {
        for (int i = 0; i < c_iterationCount; i++) {
            var context = i;
            var anonymous = AnonymousClosure.Create(AnonymousValue.From(context), (int c) => {
                // Simulate some work
                var result = c + 1;
            });
            anonymous.Invoke();
        }
    }

    // [Benchmark]
    // [BenchmarkCategory("3Multi")]
    // public void Action_MultiInvoke() {
    //     for (int i = 0; i < c_iterationCount; i++) {
    //         var context = i; // Capture the current value of i
    //         var action = new Action(() => {
    //             // Simulate some work
    //             var result = context + 1;
    //         });
    //
    //         for (int j = 0; j < c_invocationCount; j++) {
    //             action.Invoke();
    //         }
    //     }
    // }
    //
    // [Benchmark]
    // [BenchmarkCategory("3Multi")]
    // public void ClosureAction_MultiInvoke() {
    //     for (int i = 0; i < c_iterationCount; i++) {
    //         var context = i;
    //         var closure = Closure.Action(context, (int c) => {
    //             // Simulate some work
    //             var result = c + 1;
    //         });
    //
    //         for (int j = 0; j < c_invocationCount; j++) {
    //             closure.Invoke();
    //         }
    //     }
    // }
    //
    // [Benchmark]
    // [BenchmarkCategory("3Multi")]
    // public void ClosureAction_AsAction_MultiInvoke() {
    //     for (int i = 0; i < c_iterationCount; i++) {
    //         var context = i;
    //         var action = Closure.Action(context, (int c) => {
    //             // Simulate some work
    //             var result = c + 1;
    //         }).AsAction();
    //
    //         for (int j = 0; j < c_invocationCount; j++) {
    //             action.Invoke();
    //         }
    //     }
    // }

    [Benchmark]
    [BenchmarkCategory("3Multi")]
    public void AnonymousClosure_MultiInvoke() {
        for (int i = 0; i < c_iterationCount; i++) {
            var context = i;
            var anonymous = AnonymousClosure.Create(AnonymousValue.From(context), (int c) => {
                // Simulate some work
                var result = c + 1;
            });

            for (int j = 0; j < c_invocationCount; j++) {
                anonymous.Invoke();
            }
        }
    }
    
    // [Benchmark]
    // [BenchmarkCategory("10000")]
    // public void AnonymousClosure_WithArg_10000x() {
    //     for (int i = 0; i < c_iterationCount; i++) {
    //         var context = i;
    //         var anonymous = AnonymousClosure.Create(AnonymousValue.From(context), (int c, int a) => {
    //             // Simulate some work
    //             var result = c + a;
    //         });
    //         anonymous.Invoke(1);
    //     }
    // }
    //
    // [Benchmark]
    // [BenchmarkCategory("10000")]
    // public void AnonymousClosure_WithRefArg_10000x() {
    //     for (int i = 0; i < c_iterationCount; i++) {
    //         var context = i;
    //         var anonymous = AnonymousClosure.Create(AnonymousValue.From(context), (int c, ref int a) => {
    //             // Simulate some work
    //             var result = c + a;
    //         });
    //         var a = 1;
    //         anonymous.Invoke(ref a);
    //     }
    // }
    //
    // [Benchmark]
    // [BenchmarkCategory("10000")]
    // public void AnonymousClosure_WithRefCtxRefArg_10000x() {
    //     for (int i = 0; i < c_iterationCount; i++) {
    //         var context = i;
    //         var anonymous = AnonymousClosure.Create(AnonymousValue.From(context), (ref int c, ref int a) => {
    //             // Simulate some work
    //             c = a;
    //             a = c + 1;
    //         });
    //
    //         var a = 1;
    //         anonymous.Invoke(ref a);
    //     }
    // }
    //
    // [Benchmark]
    // [BenchmarkCategory("10000")]
    // public void AnonymousClosure_Converted_10000x() {
    //     for (int i = 0; i < c_iterationCount; i++) {
    //         var context = i;
    //         var anonymous = Closure.Action(context, (int c) => {
    //             // Simulate some work
    //             var result = c + 1;
    //         }).AsAnonymous();
    //         anonymous.Invoke();
    //     }
    // }
    //
    // [Benchmark]
    // [BenchmarkCategory("10000")]
    // public void AnonymousClosure_ConvertBack_10000x() {
    //     for (int i = 0; i < c_iterationCount; i++) {
    //         var context = i;
    //         var anonymous = Closure.Action(context, (int c) => {
    //             // Simulate some work
    //             var result = c + 1;
    //         }).AsAnonymous();
    //         anonymous.AsClosureAction<int>().Invoke();
    //     }
    // }
    //
    // [Benchmark]
    // [BenchmarkCategory("10000")]
    // public void AnonymousClosure_ConvertBackWithIsCheck_10000x() {
    //     for (int i = 0; i < c_iterationCount; i++) {
    //         var context = i;
    //         var anonymous = Closure.Action(context, (int c) => {
    //             // Simulate some work
    //             var result = c + 1;
    //         }).AsAnonymous();
    //
    //         if (anonymous.Is<ClosureAction<int>>())
    //             anonymous.AsClosureAction<int>().Invoke();
    //         else {
    //             throw new InvalidOperationException();
    //         }
    //     }
    // }
    //
    // [Benchmark]
    // [BenchmarkCategory("10000")]
    // public void AnonymousClosure_BoxedValue_10000x() {
    //     for (int i = 0; i < c_iterationCount; i++) {
    //         var context = (i, 0);
    //         var anonymous = Closure.Action(context, ((int, int) c) => {
    //             // Simulate some work
    //             var result = c.Item1 + 1;
    //         }).AsAnonymous();
    //         anonymous.AsClosureAction<(int, int)>().Invoke();
    //     }
    // }
}

public class ForLoopBenchmarks {
    readonly List<Action> actions3 = new List<Action>(3);
    readonly List<ClosureAction<int>> closureActions3 = new List<ClosureAction<int>>(3);
    
    readonly List<Action> actions100 = new List<Action>(100);
    readonly List<ClosureAction<int>> closureActions100 = new List<ClosureAction<int>>(100);
    
    readonly List<Action> actions10000 = new List<Action>(10000);
    readonly List<ClosureAction<int>> closureActions10000 = new List<ClosureAction<int>>(10000);
    
    void DummyAction(int context) {
        // Simulate some work
        var result = context + 1;
    }
    
    // [Benchmark]
    // [BenchmarkCategory("1")]
    // public void Action_CaptureI_3x() {
    //     actions3.Clear();
    //     
    //     for (int i = 0; i < 3; i++) {
    //         var action = new Action(() => DummyAction(i));
    //         actions3.Add(action);
    //     }
    //     
    //     foreach (var action in actions3) {
    //         action.Invoke();
    //     }
    // }
    
    [Benchmark]
    [BenchmarkCategory("1")]
    public void Action_CaptureTempI_3x() {
        actions3.Clear();

        for (int i = 0; i < 3; i++) {
            var tempI = i; // Capture the current value of i
            var action = new Action(() => DummyAction(tempI));
            actions3.Add(action);
        }
        
        foreach (var action in actions3) {
            action.Invoke();
        }
    }
    
    [Benchmark]
    [BenchmarkCategory("1")]
    public void ClosureAction_CaptureI_3x() {
        closureActions3.Clear();

        for (int i = 0; i < 3; i++) {
            var closure = Closure.Action(i, DummyAction);
            closureActions3.Add(closure);
        }
        
        foreach (var action in closureActions3) {
            action.Invoke();
        }
    }
    
    // [Benchmark]
    // [BenchmarkCategory("100")]
    // public void Action_CaptureI_100x() {
    //     actions100.Clear();
    //
    //     for (int i = 0; i < 100; i++) {
    //         var action = new Action(() => DummyAction(i));
    //         actions100.Add(action);
    //     }
    //     
    //     foreach (var action in actions100) {
    //         action.Invoke();
    //     }
    // }
    
    [Benchmark]
    [BenchmarkCategory("100")]
    public void Action_CaptureTempI_100x() {
        actions100.Clear();

        for (int i = 0; i < 100; i++) {
            var tempI = i; // Capture the current value of i
            var action = new Action(() => DummyAction(tempI));
            actions100.Add(action);
        }
        
        foreach (var action in actions100) {
            action.Invoke();
        }
    }
    
    [Benchmark]
    [BenchmarkCategory("100")]
    public void ClosureAction_CaptureI_100x() {
        closureActions100.Clear();

        for (int i = 0; i < 100; i++) {
            var closure = Closure.Action(i, DummyAction);
            closureActions100.Add(closure);
        }
        
        foreach (var action in closureActions100) {
            action.Invoke();
        }
    }
    
    // [Benchmark]
    // [BenchmarkCategory("10000")]
    // public void Action_CaptureI_10000x() {
    //     actions10000.Clear();
    //
    //     for (int i = 0; i < 10000; i++) {
    //         var action = new Action(() => DummyAction(i));
    //         actions10000.Add(action);
    //     }
    //     
    //     foreach (var action in actions10000) {
    //         action.Invoke();
    //     }
    // }
    
    [Benchmark]
    [BenchmarkCategory("10000")]
    public void Action_CaptureTempI_10000x() {
        actions10000.Clear();

        for (int i = 0; i < 10000; i++) {
            var tempI = i; // Capture the current value of i
            var action = new Action(() => DummyAction(tempI));
            actions10000.Add(action);
        }
        
        foreach (var action in actions10000) {
            action.Invoke();
        }
    }
    
    [Benchmark]
    [BenchmarkCategory("10000")]
    public void ClosureAction_CaptureI_10000x() {
        closureActions10000.Clear();

        for (int i = 0; i < 10000; i++) {
            var closure = Closure.Action(i, DummyAction);
            closureActions10000.Add(closure);
        }
        
        foreach (var action in closureActions10000) {
            action.Invoke();
        }
    }
}