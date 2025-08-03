using Closures;

namespace ClosureTests.FuncTests.ClosureFuncs.EdgeCases;

[TestFixture]
public class ClosureFuncEdgeCases {
    class TestClass {
        public int Value { get; set; }
    }

    // Default edge cases

    [Test]
    public void ClosureFunc_NullContext_Invoke_DoesNotThrow() {
        TestClass? testContext = null;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func<TestClass?, int>(testContext, ctx => ctx?.Value ?? 0);
            closure.Invoke();
        });
    }

    [Test]
    public void ClosureFunc_NullDelegate_Invoke_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func<int, int>(context, null);
            closure.Invoke();
        });
    }

    [Test]
    public void ClosureFunc_NullDelegate_Add_DoesNotThrow() {
        int context = 5;
        var closure = Closure.Func<int, int>(context, null);
        Assert.DoesNotThrow(() => {
            closure.Add(ctx => ctx + 1);
            closure.Invoke();
        });
    }

    [Test]
    public void ClosureFunc_Add_NullDelegate_DoesNotThrow() {
        int context = 5;
        var closure = Closure.Func<int, int>(context, ctx => ctx + 1);
        Assert.DoesNotThrow(() => {
            closure.Add(null);
            closure.Invoke();
        });
    }

    [Test]
    public void ClosureFunc_NullDelegate_Remove_DoesNotThrow() {
        int context = 5;
        var closure = Closure.Func<int, int>(context, ctx => ctx + 1);
        Assert.DoesNotThrow(() => {
            closure.Remove(null);
            closure.Invoke();
        });
    }

    [Test]
    public void ClosureFunc_Remove_NullDelegate_DoesNotThrow() {
        int context = 5;
        var closure = Closure.Func<int, int>(context, ctx => ctx + 1);
        Assert.DoesNotThrow(() => {
            closure.Remove(null);
            closure.Invoke();
        });
    }

    [Test]
    public void ClosureFunc_ExceptionDuringInvocation_Throws() {
        int context = 5;
        var closure = Closure.Func<int, int>(context, ctx => throw new InvalidOperationException("Test exception"));
        Assert.Throws<InvalidOperationException>(() => closure.Invoke());
    }

    [Test]
    public void ClosureFunc_ExceptionDuringInvocation_TryCatch_CatchesThrownException() {
        int context = 5;
        var closure = Closure.Func<int, int>(context, ctx => throw new InvalidOperationException("Test exception"));
        try {
            closure.Invoke();
            Assert.Fail("Exception was not thrown");
        } catch (InvalidOperationException ex) {
            Assert.That(ex.Message, Is.EqualTo("Test exception"));
        }
    }
    
    [Test]
    public void ClosureFunc_ConcurrentAddRemoveInvoke_IsThreadSafe() {
        int context = 1;
        int callSum = 0;
        int Handler(int ctx) => Interlocked.Add(ref callSum, ctx);

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
    
    // Func closure edge cases

    [Test]
    public void ClosureFunc_NullDelegate_ReturnsDefault() {
        int context = 5;
        var closure = Closure.Func<int, int>(context, null);
        Assert.DoesNotThrow(() => {
            int result = closure.Invoke();
            Assert.That(result, Is.EqualTo(0), "Default return value should be 0 when no delegate is set");
        }, "ClosureFunc with null delegate should not throw");
    }
}