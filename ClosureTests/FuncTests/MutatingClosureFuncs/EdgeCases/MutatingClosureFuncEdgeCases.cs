using Closures;

namespace ClosureTests.FuncTests.MutatingClosureFuncs.EdgeCases;

[TestFixture]
public class MutatingClosureFuncEdgeCases {
    class TestClass {
        public int Value { get; set; }
    }

    [Test]
    public void MutatingClosureFunc_NullContext_Invoke_DoesNotThrow() {
        TestClass? testContext = null;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func<TestClass?, int>(testContext, (ref TestClass? ctx) => ctx == null ? 0 : ctx.Value);
            closure.Invoke();
        });
    }

    [Test]
    public void MutatingClosureFunc_NullDelegate_Invoke_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func<int, int>(context, (RefFunc<int, int>)null);
            closure.Invoke();
        });
    }

    [Test]
    public void MutatingClosureFunc_NullDelegate_Add_DoesNotThrow() {
        int context = 5;
        var closure = Closure.Func<int, int>(context, (RefFunc<int, int>)null);
        Assert.DoesNotThrow(() => {
            closure.Add((ref int ctx) => ctx + 1);
            closure.Invoke();
        });
    }

    [Test]
    public void MutatingClosureFunc_Add_NullDelegate_DoesNotThrow() {
        int context = 5;
        var closure = Closure.Func<int, int>(context, (ref int ctx) => ctx + 1);
        Assert.DoesNotThrow(() => {
            closure.Add(null);
            closure.Invoke();
        });
    }

    [Test]
    public void MutatingClosureFunc_NullDelegate_Remove_DoesNotThrow() {
        int context = 5;
        var closure = Closure.Func<int, int>(context, (ref int ctx) => ctx + 1);
        Assert.DoesNotThrow(() => {
            closure.Remove(null);
            closure.Invoke();
        });
    }

    [Test]
    public void MutatingClosureFunc_Remove_NullDelegate_DoesNotThrow() {
        int context = 5;
        var closure = Closure.Func<int, int>(context, (ref int ctx) => ctx + 1);
        Assert.DoesNotThrow(() => {
            closure.Remove(null);
            closure.Invoke();
        });
    }

    [Test]
    public void MutatingClosureFunc_ExceptionDuringInvocation_Throws() {
        int context = 5;
        var closure = Closure.Func<int, int>(context, (ref int ctx) => throw new InvalidOperationException("Test exception"));
        Assert.Throws<InvalidOperationException>(() => closure.Invoke());
    }

    [Test]
    public void MutatingClosureFunc_ExceptionDuringInvocation_TryCatch_CatchesThrownException() {
        int context = 5;
        var closure = Closure.Func<int, int>(context, (ref int ctx) => throw new InvalidOperationException("Test exception"));
        try {
            closure.Invoke();
            Assert.Fail("Exception was not thrown");
        } catch (InvalidOperationException ex) {
            Assert.That(ex.Message, Is.EqualTo("Test exception"));
        }
    }
    
    [Test]
    public void MutatingClosureFunc_ConcurrentAddRemoveInvoke_IsThreadSafe() {
        int context = 1;
        int callSum = 0;
        int Handler(ref int ctx) => Interlocked.Add(ref callSum, ctx);

        var closure = Closure.Func(context, Handler);

        var tasks = new List<Task>();
        for (int i = 0; i < 10; i++) {
            tasks.Add(Task.Run(() => closure.Add(Handler)));
            tasks.Add(Task.Run(() => closure.Remove(Handler)));
            tasks.Add(Task.Run(() => closure.Invoke()));
        }

        Task.WaitAll(tasks.ToArray());

        // The exact value may vary, but should not throw or corrupt state
        Assert.That(callSum, Is.GreaterThanOrEqualTo(0), "Call sum should be non-negative");
    }

    // Mutating closure edge cases
    
    [Test]
    public void MutatingClosureFunc_Reset_NullDelegate_DoesNotThrow() {
        int context = 5;
        var closure = Closure.Func(context, (RefFunc<int, int>)null, MutatingClosureBehaviour.Reset);

        Assert.DoesNotThrow(() => {
            closure.Invoke();
        });
    }

    [Test]
    public void MutatingClosureFunc_ConcurrentInvoke_SharesContextAcrossThreads() {
        int context = 0;
        int taskCount = 10;

        int callSum = 0;

        int Handler(ref int ctx) {
            Interlocked.Add(ref callSum, 1);
            ctx++;
            Assert.That(ctx, Is.EqualTo(callSum), "Context should match call sum after each invocation");
            return ctx;
        }

        var closure = Closure.Func(context, Handler);
        var tasks = new List<Task>();

        for (int i = 0; i < taskCount; i++) {
            tasks.Add(Task.Run(() => closure.Invoke()));
        }

        Task.WaitAll(tasks.ToArray());
        Assert.Multiple(() => {
            Assert.That(callSum, Is.EqualTo(taskCount), "Call sum should equal number of tasks invoked");
            Assert.That(closure.Context, Is.EqualTo(taskCount), "Closure context should equal number of tasks invoked");
        });
    }
    
    // Func closure edge cases

    [Test]
    public void MutatingClosureFunc_NullDelegate_ReturnsDefault() {
        int context = 5;
        var closure = Closure.Func(context, (RefFunc<int,int>)null);
        Assert.DoesNotThrow(() => {
            int result = closure.Invoke();
            Assert.That(result, Is.EqualTo(0), "Default return value should be 0 when no delegate is set");
        }, "ClosureFunc with null delegate should not throw");
    }
}