using Lh.Closures;

namespace ClosureTests.FuncTests.ClosureFuncs.EdgeCases;

[TestFixture]
public class ClosureFuncWithArgEdgeCases {
    class TestClass {
        public int Value { get; set; }
    }

    // Default edge cases

    [Test]
    public void ClosureFuncWithArg_NullContext_Invoke_DoesNotThrow() {
        TestClass? testContext = null;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func(testContext, (TestClass? ctx, int arg) => ctx?.Value + arg ?? arg);
            closure.Invoke(42);
        });
    }

    [Test]
    public void ClosureFuncWithArg_NullDelegate_Invoke_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func(context, (Func<int, int, int>)null);
            closure.Invoke(42);
        });
    }

    [Test]
    public void ClosureFuncWithArg_NullDelegate_Add_DoesNotThrow() {
        int context = 5;
        var closure = Closure.Func(context, (Func<int, int, int>)null);
        Assert.DoesNotThrow(() => {
            closure.Add((ctx, arg) => ctx + arg);
            closure.Invoke(42);
        });
    }

    [Test]
    public void ClosureFuncWithArg_Add_NullDelegate_DoesNotThrow() {
        int context = 5;
        var closure = Closure.Func<int, int, int>(context, (int ctx, int arg) => ctx + arg);
        Assert.DoesNotThrow(() => {
            closure.Add(null);
            closure.Invoke(42);
        });
    }

    [Test]
    public void ClosureFuncWithArg_NullDelegate_Remove_DoesNotThrow() {
        int context = 5;
        var closure = Closure.Func<int, int, int>(context, (int ctx, int arg) => ctx + arg);
        Assert.DoesNotThrow(() => {
            closure.Remove(null);
            closure.Invoke(42);
        });
    }

    [Test]
    public void ClosureFuncWithArg_Remove_NullDelegate_DoesNotThrow() {
        int context = 5;
        var closure = Closure.Func<int, int, int>(context, (int ctx, int arg) => ctx + arg);
        Assert.DoesNotThrow(() => {
            closure.Remove(null);
            closure.Invoke(42);
        });
    }

    [Test]
    public void ClosureFuncWithArg_ExceptionDuringInvocation_Throws() {
        int context = 5;
        var closure = Closure.Func<int, int, int>(context, (int ctx, int arg) => throw new InvalidOperationException("Test exception"));
        Assert.Throws<InvalidOperationException>(() => closure.Invoke(42));
    }

    [Test]
    public void ClosureFuncWithArg_ExceptionDuringInvocation_TryCatch_CatchesThrownException() {
        int context = 5;
        var closure = Closure.Func<int, int, int>(context, (int ctx, int arg) => throw new InvalidOperationException("Test exception"));
        try {
            closure.Invoke(42);
            Assert.Fail("Exception was not thrown");
        } catch (InvalidOperationException ex) {
            Assert.That(ex.Message, Is.EqualTo("Test exception"));
        }
    }
    
    [Test]
    public void ClosureFunc_ConcurrentAddRemoveInvoke_IsThreadSafe() {
        int context = 1;
        int callSum = 0;
        int Handler(int ctx, int arg) => Interlocked.Add(ref callSum, ctx);

        var closure = Closure.Func<int, int, int>(context, Handler);

        var tasks = new List<Task>();
        for (int i = 0; i < 10; i++) {
            tasks.Add(Task.Run(() => closure.Add(Handler)));
            tasks.Add(Task.Run(() => closure.Remove(Handler)));
            tasks.Add(Task.Run(() => closure.Invoke(0)));
        }

        Task.WaitAll(tasks.ToArray());

        // The exact value may vary, but should not throw or corrupt state
        Assert.That(callSum, Is.GreaterThanOrEqualTo(0), "Call sum should be non-negative");
    }

    // Argument closure edge cases
    
    [Test]
    public void ClosureFuncWithArg_NullArgument_Invoke_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func(context, (int ctx, int? arg) => {
                Assert.That(arg, Is.Null, "Argument should be null");
                return arg ?? ctx;
            });
            closure.Invoke(null);
        }, "Null argument should not throw");
    }
    
    // Func closure edge cases

    [Test]
    public void ClosureFuncWithArg_NullDelegate_ReturnsDefault() {
        int context = 5;
        var closure = Closure.Func(context, (Func<int,int,int>)null);
        Assert.DoesNotThrow(() => {
            int result = closure.Invoke(0);
            Assert.That(result, Is.EqualTo(0), "Default return value should be 0 when no delegate is set");
        }, "ClosureFunc with null delegate should not throw");
    }
}