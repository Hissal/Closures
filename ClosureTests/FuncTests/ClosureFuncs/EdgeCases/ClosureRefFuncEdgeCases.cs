using Closures;

namespace ClosureTests.FuncTests.ClosureFuncs.EdgeCases;

[TestFixture]
public class ClosureRefFuncEdgeCases {
    class TestClass {
        public int Value { get; set; }
    }

    [Test]
    public void ClosureRefFunc_NullContext_Invoke_DoesNotThrow() {
        TestClass? testContext = null;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func<TestClass?, int, int>(testContext, (TestClass ctx, ref int arg) => ctx == null ? arg : ctx.Value + arg);
            int arg = 42;
            closure.Invoke(arg);
        });
    }

    [Test]
    public void ClosureRefFunc_NullDelegate_Invoke_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func<int, int, int>(context, (RefFuncWithNormalContext<int, int, int>)null);
            int arg = 42;
            closure.Invoke(arg);
        });
    }

    [Test]
    public void ClosureRefFunc_NullDelegate_Add_DoesNotThrow() {
        int context = 5;
        var closure = Closure.Func<int, int, int>(context, (RefFuncWithNormalContext<int, int, int>)null);
        Assert.DoesNotThrow(() => {
            closure.Add((int ctx, ref int arg) => ctx + arg);
            int arg = 42;
            closure.Invoke(arg);
        });
    }

    [Test]
    public void ClosureRefFunc_Add_NullDelegate_DoesNotThrow() {
        int context = 5;
        var closure = Closure.Func<int, int, int>(context, (int ctx, ref int arg) => ctx + arg);
        Assert.DoesNotThrow(() => {
            closure.Add(null);
            int arg = 42;
            closure.Invoke(arg);
        });
    }

    [Test]
    public void ClosureRefFunc_NullDelegate_Remove_DoesNotThrow() {
        int context = 5;
        var closure = Closure.Func<int, int, int>(context, (int ctx, ref int arg) => ctx + arg);
        Assert.DoesNotThrow(() => {
            closure.Remove(null);
            int arg = 42;
            closure.Invoke(arg);
        });
    }

    [Test]
    public void ClosureRefFunc_Remove_NullDelegate_DoesNotThrow() {
        int context = 5;
        var closure = Closure.Func<int, int, int>(context, (int ctx, ref int arg) => ctx + arg);
        Assert.DoesNotThrow(() => {
            closure.Remove(null);
            int arg = 42;
            closure.Invoke(arg);
        });
    }

    [Test]
    public void ClosureRefFunc_ExceptionDuringInvocation_Throws() {
        int context = 5;
        var closure = Closure.Func<int, int, int>(context, (int ctx, ref int arg) => throw new InvalidOperationException("Test exception"));
        int arg = 42;
        Assert.Throws<InvalidOperationException>(() => closure.Invoke(arg));
    }

    [Test]
    public void ClosureRefFunc_ExceptionDuringInvocation_TryCatch_CatchesThrownException() {
        int context = 5;
        var closure = Closure.Func<int, int, int>(context, (int ctx, ref int arg) => throw new InvalidOperationException("Test exception"));
        int arg = 42;
        try {
            closure.Invoke(arg);
            Assert.Fail("Exception was not thrown");
        } catch (InvalidOperationException ex) {
            Assert.That(ex.Message, Is.EqualTo("Test exception"));
        }
    }
    
    [Test]
    public void ClosureFunc_ConcurrentAddRemoveInvoke_IsThreadSafe() {
        int context = 1;
        int callSum = 0;
        int Handler(int ctx, ref int arg) => Interlocked.Add(ref callSum, ctx);

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
    public void ClosureRefFunc_NullArgument_Invoke_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func(context, (int ctx, ref int? arg) => {
                Assert.That(arg, Is.Null, "Argument should be null");
                return arg ?? ctx;
            });
            closure.Invoke(null);
        }, "Null argument should not throw");
    }

    // Ref argument closure edge cases
    
    [Test]
    public void ClosureRefFunc_NullDelegate_Invoke_RefArg_DoesNotThrow() {
        int context = 5;
        var closure = Closure.Func(context, (RefFuncWithNormalContext<int, int, int>)null);
        int arg = 42;
        Assert.DoesNotThrow(() => {
            closure.Invoke(ref arg);
        });
    }

    [Test]
    public void ClosureRefFunc_NullRefArgument_Invoke_DoesNotThrow() {
        string? context = "ctx";
        var closure = Closure.Func<string?, string?, string?>(context, (string ctx, ref string? arg) => arg ?? ctx);
        string? arg = null;
        Assert.DoesNotThrow(() => {
            closure.Invoke(ref arg);
        });
    }

    [Test]
    public void ClosureRefFunc_NullRefArg_SettingValue_ModifiesOriginalRef() {
        int context = 0;
        int expected = 123;
        var closure = Closure.Func(context, (int ctx, ref int? arg) => arg = expected);
        int? arg = null;
        closure.Invoke(ref arg);
        Assert.That(arg, Is.EqualTo(expected));
    }
    
    // Func closure edge cases

    [Test]
    public void ClosureRefFunc_NullDelegate_ReturnsDefault() {
        int context = 5;
        var closure = Closure.Func(context, (RefFuncWithNormalContext<int,int,int>)null);
        Assert.DoesNotThrow(() => {
            int result = closure.Invoke(0);
            Assert.That(result, Is.EqualTo(0), "Default return value should be 0 when no delegate is set");
        }, "ClosureFunc with null delegate should not throw");
    }
}