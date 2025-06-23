using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Lh.Closures;

BenchmarkRunner.Run<ForLoopBenchmarks>();

[MemoryDiagnoser(false)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
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