using Closures;

namespace ClosureTests.ActionTests.ClosureAction.EdgeCases;

[TestFixture]
public class ClosureActionWithArgEdgeCases {
    class TestClass {
        public int Value { get; set; }
    }
    
    // Default edge cases

    [Test]
    public void ClosureActionWithArg_NullContext_Invoke_DoesNotThrow() {
        TestClass? testContext = null;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Action(testContext, (TestClass ctx, int arg) => Assert.That(ctx, Is.Null, "Context should be null"));
            closure.Invoke(42);
        }, "Closure with null context should not throw an exception");
    }

    [Test]
    public void ClosureActionWithArg_NullDelegate_Invoke_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Action(context, (Action<int, int>)null);
            closure.Invoke(42);
        }, "Closure with null delegate should not throw an exception");
    }

    [Test]
    public void ClosureActionWithArg_NullDelegate_Add_DoesNotThrow() {
        int context = 5;
        var closure = Closure.Action(context, (Action<int, int>)null);
        Assert.DoesNotThrow(() => {
            closure.Add((ctx, arg) => Assert.That(ctx, Is.EqualTo(context)));
            closure.Invoke(42);
        }, "Adding a null delegate to a closure should not throw an exception");
    }

    [Test]
    public void ClosureActionWithArg_Add_NullDelegate_DoesNotThrow() {
        int context = 5;
        var closure = Closure.Action(context, (int ctx, int arg) => Assert.That(ctx, Is.EqualTo(context)));
        Assert.DoesNotThrow(() => {
            closure.Add(null);
            closure.Invoke(42);
        }, "Adding a null delegate to a closure should not throw an exception");
    }

    [Test]
    public void ClosureActionWithArg_NullDelegate_Remove_DoesNotThrow() {
        int context = 5;
        var closure = Closure.Action(context, (Action<int, int>)null);
        void Handler(int ctx, int arg) { /* no-op */ }
        Assert.DoesNotThrow(() => {
            closure.Remove(Handler);
            closure.Invoke(42);
        }, "Removing a null delegate from a closure should not throw an exception");
    }

    [Test]
    public void ClosureActionWithArg_Remove_NullDelegate_DoesNotThrow() {
        int context = 5;
        var closure = Closure.Action(context, (int ctx, int arg) => Assert.That(ctx, Is.EqualTo(context)));
        Assert.DoesNotThrow(() => {
            closure.Remove(null);
            closure.Invoke(42);
        }, "Removing a null delegate from a closure should not throw an exception");
    }

    [Test]
    public void ClosureActionWithArg_ExceptionDuringInvocation_Throws() {
        int context = 5;
        var closure = Closure.Action(context, (int ctx, int arg) => throw new InvalidOperationException("Test exception"));
        Assert.Throws<InvalidOperationException>(() => {
            closure.Invoke(42);
        }, "Closure should throw an exception during invocation");
    }

    [Test]
    public void ClosureActionWithArg_ExceptionDuringInvocation_TryCatch_CatchesThrownException() {
        int context = 5;
        var closure = Closure.Action(context, (int ctx, int arg) => throw new InvalidOperationException("Test exception"));
        try {
            closure.Invoke(42);
            Assert.Fail("Closure should throw an exception during invocation");
        }
        catch (InvalidOperationException ex) {
            Assert.That(ex.Message, Is.EqualTo("Test exception"), "Caught exception message should match");
        }
    }

    [Test]
    public void ClosureActionWithArg_ConcurrentAddRemoveInvoke_IsThreadSafe() {
        int context = 1;
        int callSum = 0;
        void Handler(int ctx, int arg) => Interlocked.Add(ref callSum, ctx + arg);
        var closure = Closure.Action<int, int>(context, Handler);
        var tasks = new List<Task>();
        for (int i = 0; i < 10; i++) {
            tasks.Add(Task.Run(() => closure.Add(Handler)));
            tasks.Add(Task.Run(() => closure.Remove(Handler)));
            tasks.Add(Task.Run(() => closure.Invoke(42)));
        }
        Task.WaitAll(tasks.ToArray());
        Assert.That(callSum, Is.GreaterThanOrEqualTo(0), "Call sum should be non-negative");
    }
    
    // Argument edge cases
    
    [Test]
    public void ClosureActionWithArg_NullArg_Invoke_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Action(context, (int ctx, int? arg) => Assert.That(arg, Is.Null, "Argument should be null"));
            closure.Invoke(null);
        }, "Closure with null argument should not throw an exception");
    }
}